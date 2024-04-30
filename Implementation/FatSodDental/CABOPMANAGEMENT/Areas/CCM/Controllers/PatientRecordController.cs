using FatSod.DataContext.Repositories;
using FatSod.Security.Abstracts;
using FatSod.Security.Entities;
using FatSod.Supply.Abstracts;
using FatSod.Supply.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CABOPMANAGEMENT.Areas.CRM.ViewModel;
using CABOPMANAGEMENT.Controllers;
using CABOPMANAGEMENT.Filters;
using SaleE = FatSod.Supply.Entities.Sale;

namespace CABOPMANAGEMENT.Areas.CCM.Controllers
{
    [Authorize(Order = 1)]
    [TakeBusinessDay(Order = 2)]
    public class PatientRecordController : BaseController
    {

        private IBusinessDay _busDayRepo;
        private IRepository<FatSod.Security.Entities.File> _fileRepository;
        private ISale _saleRepository;
        private ICustomerReturn _customerReturnRepository;
        private IDeposit _depositRepository;
        string space = "&nbsp";
        string space3 = "";
        string lineBreak = "<br>";
        string lineBreak2 = "";

        List<BusinessDay> bdDay;

        // GET: CashRegister/State
        public PatientRecordController(
            IBusinessDay busDayRepo,
            IRepository<FatSod.Security.Entities.File> fileRepository,
            ISale saleRepository,
            ICustomerReturn CustomerReturnRepository,
            IDeposit depositRepository
            )
        {
            space3 = space + space + space;
            lineBreak2 = lineBreak + lineBreak;
            this._busDayRepo = busDayRepo;
            this._fileRepository = fileRepository;
            this._saleRepository = saleRepository;
            this._customerReturnRepository = CustomerReturnRepository;
            this._depositRepository = depositRepository;
        }
        //Enable to get hitoric of cash register
        [OutputCache(Duration = 3600)]
        public ActionResult Index()
        {
            bdDay = (List<BusinessDay>)Session["UserBusDays"];
            if (bdDay == null)
            {
                bdDay = _busDayRepo.GetOpenedBusinessDay(CurrentUser);
            }
            if (bdDay.Count() > 1)
            {
                ViewBag.Disabled = false;
            }
            DateTime busDays = bdDay.FirstOrDefault().BDDateOperation;

            ViewBag.CurrentBranch = bdDay.FirstOrDefault().BranchID;
            ViewBag.BusnessDayDate = busDays.ToString("yyyy-MM-dd");
            Session["BusnessDayDate"] = busDays;

            return View();
        }

        /*** methodes lors du chargement du formulaire*********************/
        public JsonResult GetOpenedBranches()
        {

            IBusinessDay busDayRepo = new BusinessDayRepository();
            List<object> openedBranchesList = new List<object>();
            List<Branch> openedBranches = busDayRepo.GetOpenedBranches();
            foreach (Branch branch in openedBranches)
            {
                openedBranchesList.Add(new
                {
                    BranchID = branch.BranchID,
                    BranchName = branch.BranchName
                });
            }

            return Json(openedBranchesList, JsonRequestBehavior.AllowGet);
        }
        public JsonResult LoadCustomers(string filter)
        {

            List<object> customersList = new List<object>();
            var customers = db.Customers.Where(c => 
            (c.Description == null ? c.Name : (c.Name + " " + c.Description)).ToLower().StartsWith(filter.ToLower()))
                .ToArray().OrderBy(c => (c.Name + " " + c.Description));
            foreach (Customer customer in customers)
            {
                string itemLabel = "";

                //si le client est une personne physique, on renvoie son nom et son prénom, si c'est une entreprise, on renvoie son nom
                itemLabel = customer.FullName;/*customer.Name + ((customer.Description == null) ? "" : " " + customer.Description);*/

                customersList.Add(new
                {
                    Name = itemLabel,
                    ID = customer.GlobalPersonID
                });
            }

            return Json(customersList, JsonRequestBehavior.AllowGet);
        }

        private List<PatientRecordViewModel> ModelHistoCusto(int BranchID, int? customerID)
        {
            List<PatientRecordViewModel> histories = new List<PatientRecordViewModel>();
            List<PatientRecordViewModel> modelRpt = new List<PatientRecordViewModel>();

            List<PatientRecordViewModel> model = new List<PatientRecordViewModel>();

            Customer customer = (from cust in db.Customers
                                 where cust.GlobalPersonID == customerID
                                 select cust).SingleOrDefault();
            var order = 1;


            //0- Customer Value
            histories.Add(this.GetCustomerValue(customer, ref order));

            // 1- Registration
            histories.Add(this.GetRegistration(customer, ref order));

            // 2- ATCD
            var medicalHistory = this.GetMedicalHistory(customer, ref order);
            if(medicalHistory != null)
                histories.Add(medicalHistory);

            Branch currentBranch = db.Branches.Find(BranchID);// SessionBusinessDay(null).Branch;
            Devise currentDevise = db.Devises.Where(dev => dev.DefaultDevise).FirstOrDefault();

            //recuperation de toutes les historiques de la periode
            if (customerID.HasValue && customerID.Value > 0)
            {

                // 3- Consultation(Consultation -> Parameters -> Dilatation -> Prescription -> Conclusion)
                histories.AddRange(GetCustomerConsultations(customerID.Value, ref order));

                // 4- Outside Prescription <=> Other Sales with Glasses
                histories.AddRange(GetAllSalesPrescriptions(customerID.Value, ref order));

                // 5- Insurance Path
                histories.AddRange(GetAllProformaPrescriptions(customerID.Value, ref order));

            }

            histories = histories.OrderBy(h1 => h1.DateOperationHours).ToList(); //.ThenBy(h2 => h2.Order).ToList();

            return histories;

        }


        public JsonResult GenerateReport(int BranchID, int? CustomerID)
        {


            var model = new
            {
                data = ModelHistoCusto(BranchID, CustomerID.Value)
            };
            /*
            var model = new
            {
                data = this.getSampleData()
            };*/

            //return Json(model, JsonRequestBehavior.AllowGet);
            var jsonResult = Json(model, JsonRequestBehavior.AllowGet);
            // var jsonResult = Json(abc, JsonRequestBehavior.AllowGet);

            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;

        }
        private Company Company
        {
            get
            {
                return db.Companies.FirstOrDefault();
            }
        }

        private PatientRecordViewModel GetRegistration(Customer customer, ref int order)
        {
            PatientRecordViewModel registration = new PatientRecordViewModel()
            {
                OperationId = customer.GlobalPersonID,
                OperationDate = customer.Dateregister.Value.ToString("yyyy-MM-dd"),
                OperationType = "REGISTRATION",
                OperationSummary = "RAS",
                DateOperationHours = customer.Dateregister.Value,
                Order = order++
            };

            return registration;
        }


        #region HOUSE CUSTOMER JOURNEY
        private List<PatientRecordViewModel> GetCustomerConsultations(int customerId, ref int order)
        {
            var res = new List<PatientRecordViewModel>();

            var consultations = db.Consultations.Where(c => c.CustomerID == customerId).OrderByDescending(o => o.DateConsultation).ToList();

            if (consultations == null)
                return res;

            foreach (var consultation in consultations)
            {
                // 1- CONSULTATION
                res.Add(GetConsultationSummary(consultation, ref order));

                // 2- PARAMETERS
                var parameters = GetParameterSummary(consultation, ref order);
                if (parameters != null)
                    res.Add(parameters);

                // 3- PRESCRIPTION
                var prescription = GetPrescriptionSummary(consultation, ref order);
                if (prescription != null)
                    res.Add(prescription);

                // 4- CONCLUSION
                var conclusion = GetConclusionSummary(consultation, ref order);
            }

            return res;
        }

        public PatientRecordViewModel GetCustomerValue(Customer customer, ref int order)
        {
            var busDay = ((DateTime)Session["BusnessDayDate"]);

            var value = (customer.Dateregister == busDay) ? "NEW" : customer.CustomerValueUI;
            var registrationDate = customer.Dateregister.HasValue ? customer.Dateregister.Value : busDay;
            var res = new PatientRecordViewModel()
            {
                DateOperationHours = registrationDate,
                HasBeenPurchased = "RAS",
                OperationAmount = "RAS",
                OperationDate = registrationDate.ToString("yyyy-MM-dd"),
                OperationId = customer.GlobalPersonID,
                OperationReference = "RAS",
                OperationSummary = value,
                OperationType = "CUSTOMER VALUE",
                Operator = "RAS",
                Order = order++
            };

            return res;
        }

        public PatientRecordViewModel GetConsultationSummary(Consultation consultation, ref int order)
        {
            var res = new PatientRecordViewModel()
            {
                Order = order++,
                OperationId = consultation.ConsultationID,
                OperationDate = consultation.DateConsultation.ToString("yyyy-MM-dd"),
                OperationType = "CONSULTATION",
                HasBeenPurchased = "RAS",
                OperationSummary = consultation.RaisonRdv,
                Operator = consultation.MedecintTraitant,
                DateOperationHours = consultation.DateConsultation.AddMinutes(5)
            };

            return res;
        }

        public PatientRecordViewModel GetParameterSummary(Consultation consultation, ref int order)
        {
            var consultOldPrescr = db.ConsultOldPrescrs.FirstOrDefault(c => c.ConsultationID == consultation.ConsultationID);

            if (consultOldPrescr == null)
                return null;

            var res = new PatientRecordViewModel()
            {
                Order = order++,
                OperationId = consultOldPrescr.ConsultOldPrescrID,
                OperationDate = consultOldPrescr.DateConsultOldPres.ToString("yyyy-MM-dd"),
                OperationType = "PARAMETERS",
                HasBeenPurchased = "RAS",
                OperationSummary = GetConsultationParameterSummary(consultOldPrescr, consultation.Customer),
                Operator = consultOldPrescr.ConsultBy.FullName,
                DateOperationHours = consultOldPrescr.DateConsultOldPres.AddMinutes(5)
            };

            return res;
        }

        public PatientRecordViewModel GetPrescriptionSummary(Consultation consultation, ref int order)
        {
            ConsultLensPrescription prescription = db.ConsultLensPrescriptions.FirstOrDefault(c => c.ConsultDilPresc.ConsultationID == consultation.ConsultationID);

            if (prescription == null)
                return null;

            var res = new PatientRecordViewModel() {
                Order = order++,
                OperationId = prescription.ConsultLensPrescriptionID,
                OperationDate = prescription.DatePrescription.ToString("yyyy-MM-dd"),
                OperationType = "PRESCRIPTION",
                HasBeenPurchased = IsPrescriptionBought(prescription) ? "YES" : "NO",
                OperationSummary = GetPrescriptionSummary(prescription),
                Operator = prescription.ConsultBy.FullName,
                DateOperationHours = prescription.DatePrescription.AddMinutes(5)
            };

            return res;
        }

        public bool IsPrescriptionBought(ConsultLensPrescription prescription)
        {
            return db.Sales.Any(s => s.AuthoriseSaleFK.ConsultDilPrescID == prescription.ConsultDilPrescID);
        }

        public PatientRecordViewModel GetConclusionSummary(Consultation consultation, ref int order)
        {
            PrescriptionLStep consultPrescrLastStep = db.PrescriptionLSteps.FirstOrDefault(c => c.ConsultationID == consultation.ConsultationID);

            if (consultPrescrLastStep == null)
                return null;

            var res = new PatientRecordViewModel()
            {
                Order = order++,
                Operator = consultPrescrLastStep.ConsultBy.FullName,
                OperationId = consultPrescrLastStep.PrescriptionLStepID,
                OperationDate = consultPrescrLastStep.DatePrescriptionLStep.ToString("yyyy-MM-dd"),
                OperationType = "CONCLUSION",
                OperationSummary = GetConsultationConclusionSummary(consultPrescrLastStep),
                // en ajoutant 5 minutes, cela fera en sorte que la registration vienne avant la consultation car en bd c'est la meme date sans heure
                DateOperationHours = consultPrescrLastStep.DatePrescriptionLStep.AddMinutes(5)
            };

            return res;
        }

        private string GetConsultationConclusionSummary(PrescriptionLStep consultPrescrLastStep)
        {
            var res = "";

            if (consultPrescrLastStep.Remarque != null && consultPrescrLastStep.Remarque != String.Empty)
                res += "<strong>Remark: " + "</strong>" + consultPrescrLastStep.Remarque;

            if (consultPrescrLastStep.CollyreName != null && consultPrescrLastStep.CollyreName != String.Empty)
                res += "<br>" + "<strong>Collyre: " + "</strong>" + consultPrescrLastStep.CollyreName;

            res += "<br>" + "<strong>Date RDV: " + "</strong>" + consultPrescrLastStep.DateRdv.ToString("yyyy-MM-dd");

            return res;
        }

        private string GetConsultationParameterSummary(ConsultOldPrescr consultOldPrescr, Customer customer)
        {
            var res = "";

            res = "<strong>Birth Date: " + "</strong>" + customer.DateOfBirth?.ToString("yyyy-MM-dd");

            if (consultOldPrescr.PlaintePatient != null && consultOldPrescr.PlaintePatient != String.Empty)
                res += "<br>" + "<strong>Complain: " + "</strong>" + consultOldPrescr.PlaintePatient;

            var dilatation = db.ConsultDilatations.SingleOrDefault(d => d.ConsultDilPresc.ConsultationID == consultOldPrescr.ConsultationID);

            // AVL
            if (consultOldPrescr.LAcuiteVisuelL != null && consultOldPrescr.LAcuiteVisuelLID > 0)
                res += "<br>" + "<strong>AVL Gauche: " + "</strong>" + consultOldPrescr.LAcuiteVisuelL.Name;

            if (consultOldPrescr.RAcuiteVisuelL != null && consultOldPrescr.RAcuiteVisuelLID > 0)
                res += "<br>" + "<strong>AVL Droit: " + "</strong>" + consultOldPrescr.RAcuiteVisuelL.Name;

            // AVP
            if (consultOldPrescr.LAcuiteVisuelP != null && consultOldPrescr.LAcuiteVisuelPID > 0)
                res += "<br>" + "<strong>AVP Gauche: " + "</strong>" + consultOldPrescr.LAcuiteVisuelP.Name;

            if (consultOldPrescr.RAcuiteVisuelP != null && consultOldPrescr.RAcuiteVisuelPID > 0)
                res += "<br>" + "<strong>AVP Droit: " + "</strong>" + consultOldPrescr.RAcuiteVisuelP.Name;

            // TS
            if (consultOldPrescr.LAVLTS != null && consultOldPrescr.LAVLTSID > 0)
                res += "<br>" + "<strong>TS  Gauche: " + "</strong>" + consultOldPrescr.LAVLTS.Name;

            if (consultOldPrescr.RAVLTS != null && consultOldPrescr.RAVLTSID > 0)
                res += "<br>" + "<strong>TS  Droit: " + "</strong>" + consultOldPrescr.RAVLTS.Name;

            return res;
        }

        private string GetPrescriptionSummary(ConsultLensPrescription prescription)
        {

            string summary = "";

            // OD
            summary = LensConstruction.GetLensCodeByPrescription(prescription, EyeSide.OD) + "<br>";

            // OG
            summary = summary + LensConstruction.GetLensCodeByPrescription(prescription, EyeSide.OG);

            return summary;
        }

        #region MEDICAL HISTORY
        private PatientRecordViewModel GetMedicalHistory(Customer customer, ref int order)
        {
            var summary = "";
            var personnalATCD = GetPersonnalMedicalHistorySummary(customer);
            var familyATCD = GetFamilyMedicalHistorySummary(customer);
            var isAllNull = (personnalATCD == null || personnalATCD == String.Empty) &&
                            (familyATCD == null || familyATCD == String.Empty);

            if (isAllNull)
                return null;

            if (personnalATCD != null && personnalATCD != string.Empty)
                summary = "<strong>Personnal Medical History: " + space + "</strong>" + personnalATCD;

            if (familyATCD != null && familyATCD != string.Empty)
                summary += "<strong>Family Medical History: " + space + "</strong>" + familyATCD;

            var medicalHisory = new PatientRecordViewModel()
            {
                OperationId = customer.GlobalPersonID,
                OperationDate = customer.Dateregister.Value.ToString("yyyy-MM-dd"),
                OperationType = "MEDICAL HISTORY",
                OperationSummary = summary,
                DateOperationHours = customer.Dateregister.Value,
                Order = order++,
                HasBeenPurchased = "RAS"
            };

            return medicalHisory;
        }

        public string GetPersonnalMedicalHistorySummary(Customer customer)
        {
            string res = "";
            //suppression de l'existant
            List<ATCDPersonnel> lstAtcdPerso = db.ATCDPersonnels.Where(c => c.CustomerID == customer.GlobalPersonID).ToList();

            if (lstAtcdPerso == null)
                return res;

            lstAtcdPerso.ForEach(personnalATCD =>
            {
                res += personnalATCD.ATCD?.Name + space3;
            });

            return res;
        }

        public string GetFamilyMedicalHistorySummary(Customer customer)
        {
            string res = "";
            //suppression de l'existant
            List<ATCDFamilial> lstAtcdFam = db.ATCDFamiliaux.Where(c => c.CustomerID == customer.GlobalPersonID).ToList();

            if (lstAtcdFam == null)
                return res;

            lstAtcdFam.ForEach(famATCD =>
            {
                res += famATCD.ATCD?.Name + space3;
            });

            return res;
        }
        #endregion
        #endregion

        #region OUTSIDE CUSTOMER
        public List<PatientRecordViewModel> GetAllSalesPrescriptions(int customerId, ref int order)
        {
            var sales = db.Sales.Where(s => s.CustomerID == customerId &&
                         (/*s.AuthoriseSaleFK == null || s.AuthoriseSaleFK.ConsultDilPrescID == null ||*/
                         !s.AuthoriseSaleFK.IsDilatation) &&
                         s.SaleLines.Any(sl => (sl.Product is Lens) || (sl.Product is OrderLens))).ToList();

            if (sales == null)
                return null;

            var prescriptions = new List<PatientRecordViewModel>();

            foreach (var sale in sales)
            {
                prescriptions.Add(GetSaleHistory(sale, ref order));
            }

            return prescriptions;
        }

        public PatientRecordViewModel GetSaleHistory(SaleE sale, ref int order)
        {
            var res = new PatientRecordViewModel() {
                Order = order++,
                OperationId = sale.SaleID,
                OperationDate = sale.SaleDate.ToString("yyyy-MM-dd"),
                OperationType = "PURCHASE",
                OperationSummary = GetSaleSummary(sale),
                DateOperationHours = sale.SaleDate.AddMinutes(5),
                HasBeenPurchased = "YES",
                Operator = "Seller(" + sale.Operator.FullName + ")"
            };

            return res;
        }

        private string GetSaleSummary(SaleE sale)
        {
            var summary = "";

            // Frame
            SaleLine frameSaleLine = sale.SaleLines.SingleOrDefault(sl => sl.marque != null && sl.reference != null);
            string frameLabel = frameSaleLine == null ? "" :
            ("<strong>Frame/Monture: </strong>" + frameSaleLine.marque + " - Reference " + frameSaleLine.reference);

            if (sale.SaleLines.Any(s => (s.Product is Lens) || (s.Product is OrderLens)))
            {
                // Prescription
                summary = this.GetOtherSalePrescriptionSummary(sale);
            }
            else
            {
                var saleLines = sale.SaleLines.Where(sl => sl.LineID != frameSaleLine?.LineID/* && sl.marque == null || sl.reference == null*/);
                foreach (var saleLine in saleLines)
                {
                    summary += saleLine.Product.ProductLabel + "<br>";
                }
            }

            summary = (frameLabel != null) ? frameLabel + "<br>" + summary : summary;

            return summary;
        }

        private string GetOtherSalePrescriptionSummary(SaleE sale)
        {

            string summary = "";
            string labelProduct = "";
            string label1 = "";
            string label2 = "";
            string label3 = "";
            var saleLines = sale.SaleLines.Where(sl => (sl.Product is Lens) || (sl.Product is OrderLens)).ToArray();
            // OD
            if (saleLines != null && saleLines.Length >= 1)
                summary = LensConstruction.GetLensCodeBySaleLine(saleLines[0]) + "<br>";

            // OG
            if (saleLines != null && saleLines.Length >= 2)
                summary = summary + LensConstruction.GetLensCodeBySaleLine(saleLines[1]);

            return summary;
        }

        private string GetProformaPrescriptionSummary(CustomerOrder customerOrder)
        {

            string summary = "";
            string labelProduct = "";
            string label1 = "";
            string label2 = "";
            string label3 = "";
            var customerOrderLines = customerOrder.CustomerOrderLines.Where(sl => (sl.Product is Lens) || (sl.Product is OrderLens)).ToArray();
            // OD
            summary = LensConstruction.GetLensCodeByCustomerOrderLine(customerOrderLines[0], EyeSide.OD) + "<br>";

            // OG
            summary = summary + LensConstruction.GetLensCodeByCustomerOrderLine(customerOrderLines[1], EyeSide.OG);

            return summary;
        }
        #endregion

        #region INSURED CUSTOMER JOURNEY
        public List<PatientRecordViewModel> GetAllProformaPrescriptions(int customerId, ref int order)
        {
            var customerOrders = db.CustomerOrders.Where(c => c.CustomerID == customerId).ToList();

            var prescriptions = new List<PatientRecordViewModel>();

            foreach (var customerOrder in customerOrders)
            {
                prescriptions.Add(GetProformaHistory(customerOrder, ref order));     
            }

            return prescriptions;
        }

        public PatientRecordViewModel GetProformaHistory(CustomerOrder customerOrder, ref int order)
        {

            var res = new PatientRecordViewModel()
            {
                OperationType = "INSURED PRESCRIPTION",
                OperationDate = customerOrder.CustomerOrderDate.ToString("yyyy-MM-dd"),
                DateOperationHours = customerOrder.CustomerOrderDate,
                OperationId = customerOrder.CustomerOrderID,
                HasBeenPurchased = customerOrder.BillState == StatutFacture.Validated ? "YES" : "NO",
                Operator = customerOrder.Operator.FullName,
                Order = order++,
                OperationSummary = GetCustomerOrderSummary(customerOrder)

            };

            return res;
        }

        private string GetCustomerOrderSummary(CustomerOrder customerOrder)
        {
            var summary = "";

            // Frame
            CustomerOrderLine frameSaleLine = customerOrder.CustomerOrderLines.SingleOrDefault(sl => sl.marque != null && sl.reference != null);
            string frameLabel = frameSaleLine == null ? "" :
            ("<strong>Frame/Monture: </strong>" + frameSaleLine.marque + " - Reference " + frameSaleLine.reference);

            if (customerOrder.CustomerOrderLines.Any(s => (s.Product is Lens) || (s.Product is OrderLens)))
            {
                // Prescription
                summary = this.GetProformaPrescriptionSummary(customerOrder);
            }
            else
            {
                var saleLines = customerOrder.CustomerOrderLines.Where(sl => sl.LineID != frameSaleLine?.LineID/* && sl.marque == null || sl.reference == null*/);
                foreach (var saleLine in saleLines)
                {
                    summary += saleLine.Product.ProductLabel + "<br>";
                }
            }

            summary = (frameLabel != null) ? frameLabel + "<br>" + summary : summary;

            return summary;
        }

        #endregion
    }
}