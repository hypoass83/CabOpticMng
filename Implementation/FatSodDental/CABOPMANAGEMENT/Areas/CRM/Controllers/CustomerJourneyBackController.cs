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

namespace CABOPMANAGEMENT.Areas.CRM.Controllers
{
    [Authorize(Order = 1)]
    [TakeBusinessDay(Order = 2)]
    public class CustomerJourneyBackController : BaseController
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
        public CustomerJourneyBackController(
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

            Branch currentBranch = db.Branches.Find(BranchID);// SessionBusinessDay(null).Branch;
            Devise currentDevise = db.Devises.Where(dev => dev.DefaultDevise).FirstOrDefault();

            //recuperation de toutes les historiques de la periode
            if (customerID.HasValue && customerID.Value > 0)
            {

                // 3- Consultation(Consultation -> Parameters -> Dilatation -> Prescription -> Conclusion)
                histories.AddRange(GetCustomerConsultations(customerID.Value, ref order));

                // 4- Outside Prescription <=> Other Sales with Glasses
                histories.AddRange(GetAllSaleHistories(customerID.Value, ref order));

                // 5- Insurance Path
                histories.AddRange(GetAllProformaPrescriptions(customerID.Value, ref order));
            }

            //histories = histories.OrderBy(h1 => h1.DateOperationHours).ToList();//.ThenBy(h2 => h2.Order).ToList();

            return histories;

        }



        public PatientRecordViewModel GetCustomerValue(Customer customer, ref int order)
        {
            var busDay = ((DateTime)Session["BusnessDayDate"]);

            var value =  (customer.Dateregister == busDay) ? "NEW" : customer.CustomerValueUI;
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
            }

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

        #endregion

        #region OUTSIDE CUSTOMER
        public List<PatientRecordViewModel> GetAllSaleHistories(int customerId, ref int order)
        {
            var sales = db.Sales.Where(s => s.CustomerID == customerId).ToList();

            if (sales == null)
                return null;

            var saleHistories = new List<PatientRecordViewModel>();

            foreach (var sale in sales)
            {
                saleHistories.Add(GetSaleHistory(sale, ref order));
                var payments = db.CustomerSlices.Where(s => s.SaleID == sale.SaleID);
                foreach (var payment in payments)
                {
                    saleHistories.Add(GetPaymentHistory(payment, ref order));
                }
            }

            return saleHistories;
        }

        public PatientRecordViewModel GetSaleHistory(SaleE sale, ref int order)
        {
            var res = new PatientRecordViewModel() {
                Order = order++,
                OperationId = sale.SaleID,
                OperationDate = sale.SaleDate.ToString("yyyy-MM-dd"),
                OperationType = "PURCHASE",
                OperationAmount = "" + _depositRepository.SaleBill(sale),
                OperationReference = sale.SaleReceiptNumber,
                OperationSummary = GetSaleSummary(sale),
                DateOperationHours = sale.SaleDate.AddMinutes(5),
                HasBeenPurchased = "YES",
                Operator = "Seller(" + sale.Operator.FullName + ")"
            };

            return res;
        }

        public PatientRecordViewModel GetPaymentHistory(CustomerSlice slice,ref int order)
        {
            return new PatientRecordViewModel()
            {
                OperationType = "PAYMENT",
                DateOperationHours = slice.SliceDate.AddMinutes(10),
                OperationAmount = "" + slice.SliceAmount,
                OperationDate = slice.SliceDate.ToString("yyyy-MM-dd"),
                OperationId = slice.SliceID,
                OperationReference = slice.Reference,
                Operator = "",
                Order = order++,
                OperationSummary = "PURCHASE( " + slice.Sale.SaleReceiptNumber + " )"
            };
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

            var proformaHistories = new List<PatientRecordViewModel>();

            foreach (var customerOrder in customerOrders)
            {
                proformaHistories.Add(GetProformaHistory(customerOrder, ref order));
                var payments = GetAllInsuredDeposits(customerOrder, ref order);
                if (payments != null && payments.Count > 0)
                    proformaHistories.AddRange(payments);
            }

            return proformaHistories;
        }

        public PatientRecordViewModel GetProformaHistory(CustomerOrder customerOrder, ref int order)
        {

            var res = new PatientRecordViewModel()
            {
                OperationType = "INSURED PRESCRIPTION",
                OperationDate = GetInsuredOperationDate(customerOrder),
                DateOperationHours = customerOrder.CustomerOrderDate.AddMinutes(5),
                OperationId = customerOrder.CustomerOrderID,
                HasBeenPurchased = customerOrder.BillState == StatutFacture.Validated ? "YES" : "NO",
                Operator = customerOrder.Operator.FullName,
                Order = order++,
                OperationReference = customerOrder.CustomerOrderNumber,
                OperationSummary = GetCustomerOrderSummary(customerOrder),
                OperationAmount = GetInsuredOperationAmount(customerOrder),
            };

            return res;
        }


        public string GetInsuredOperationDate(CustomerOrder proforma)
        {
            var res = "";

            res = "<strong>Proforma Date: </strong>" + proforma.CustomerOrderDate.ToString("yyyy-MM-dd") + "<br>" +
                  "<strong>Validation Date: </strong>" + (proforma.BillState == StatutFacture.Validated ?
                                proforma.ValidateBillDate.ToString("yyyy-MM-dd") : "PENDING");

            return res;
        }

        public string GetInsuredOperationAmount(CustomerOrder proforma)
        {
            var res = "";

            res = "<strong>Plafond: </strong>" + proforma.Plafond + "<br>" +
                  "<strong>Total Malade: </strong>" + proforma.TotalMalade;
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

        private List<PatientRecordViewModel> GetAllInsuredDeposits(CustomerOrder customerOrder, ref int order)
        {
            var res = new List<PatientRecordViewModel>();

            var deposits = db.AllDeposits.Where(d => d.CustomerOrderID == customerOrder.CustomerOrderID).ToList();

            foreach (var deposit in deposits)
            {
                res.Add(GetDepositHistory(deposit, ref order));
            }

            return res;
        }

        public PatientRecordViewModel GetDepositHistory(AllDeposit deposit, ref int order)
        {
            var res = new PatientRecordViewModel() {
                OperationAmount = "" + deposit.Amount,
                DateOperationHours = deposit.CustomerOrder.CustomerOrderDate.AddMinutes(10),
                OperationDate = deposit.AllDepositDate.ToString("yyyy-MM-dd"),
                OperationId = deposit.AllDepositID,
                OperationReference = deposit.AllDepositReference,
                OperationSummary = "PROFORMA(" + deposit.CustomerOrder.CustomerOrderNumber + ")",
                OperationType = "PAYMENT",
                Order = order++,
            };

            return res;
        }
        #endregion
    }
}