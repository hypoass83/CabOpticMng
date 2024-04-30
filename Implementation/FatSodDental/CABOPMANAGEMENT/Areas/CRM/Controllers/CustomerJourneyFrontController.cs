
using FastSod.Utilities.Util;
using FatSod.DataContext.Repositories;
using FatSod.Report.WrapReports;
using FatSod.Security.Abstracts;
using FatSod.Security.Entities;
using FatSod.Supply.Abstracts;
using FatSod.Supply.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CABOPMANAGEMENT.Controllers;
using CABOPMANAGEMENT.Filters;
using SaleE = FatSod.Supply.Entities.Sale;

namespace CABOPMANAGEMENT.Areas.CRM.Controllers
{
    [Authorize(Order = 1)]
    [TakeBusinessDay(Order = 2)]
    public class CustomerJourneyFrontController : BaseController
    {

        private IBusinessDay _busDayRepo;
        private IRepository<FatSod.Security.Entities.File> _fileRepository;
        private ISale _saleRepository;
        private ICustomerReturn _customerReturnRepository;
        private IDeposit _depositRepository;

        List<BusinessDay> bdDay;

        // GET: CashRegister/State
        public CustomerJourneyFrontController(
            IBusinessDay busDayRepo,
            IRepository<FatSod.Security.Entities.File> fileRepository,
            ISale saleRepository,
            ICustomerReturn CustomerReturnRepository,
            IDeposit depositRepository
            )
        {
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
            foreach (Customer customer in db.People.OfType<Customer>().Where(c => c.Name.ToLower().StartsWith(filter.ToLower())).ToArray().OrderBy(c => c.Name))
            {
                string itemLabel = "";

                //si le client est une personne physique, on renvoie son nom et son prénom, si c'est une entreprise, on renvoie son nom
                itemLabel = customer.Name + ((customer.Description == null) ? "" : " " + customer.Description);

                customersList.Add(new
                {
                    Name = itemLabel,
                    ID = customer.GlobalPersonID
                });
            }

            return Json(customersList, JsonRequestBehavior.AllowGet);
        }

        private List<RptPrintStmt> ModelHistoCusto(int BranchID, int? customerID)
        {
            List<RptPrintStmt> histories = new List<RptPrintStmt>();
            List<RptPrintStmt> modelRpt = new List<RptPrintStmt>();


            List<RptPrintStmt> model = new List<RptPrintStmt>();

            Customer customer = (from cust in db.Customers
                                 where cust.GlobalPersonID == customerID
                                 select cust).SingleOrDefault();
            var order = 1;

            // 1- Registration
            histories.Add(this.GetRegistration(customer, ref order));

            Branch currentBranch = db.Branches.Find(BranchID);// SessionBusinessDay(null).Branch;
            Devise currentDevise = db.Devises.Where(dev => dev.DefaultDevise).FirstOrDefault();

            //int i = 0;

            //recuperation de toutes les historiques de la periode
            if (customerID.Value > 0)
            {
                // 2- Consultation
                List<RptPrintStmt> consultationHistories = this.GetConsultationHistories(customerID.Value, currentBranch, currentDevise, ref order);
                histories.AddRange(consultationHistories);

                // 3- Dilataion
                List<RptPrintStmt> dilatationHistories = this.GetDilatationHistories(customerID.Value, currentBranch, currentDevise, ref order);
                histories.AddRange(dilatationHistories);

                // 4- Paiement Dilattion
                List<RptPrintStmt> dilatationPaymentHistories = this.GetDilatationPaymentHistories(customerID.Value, currentBranch, currentDevise, ref order);
                histories.AddRange(dilatationPaymentHistories);

                // 5- Prescription
                /*List<RptPrintStmt> prescriptionHistories = this.getPrescriptionHistories(customerID.Value, currentBranch, currentDevise);
                histories.AddRange(prescriptionHistories);*/
                // 5.1- Prescription ayant donner lieu a une vente 
                List<RptPrintStmt> prescriptionSalesHistories = this.GetPrescriptionSalesHistories(customerID.Value, currentBranch, currentDevise, ref order);
                histories.AddRange(prescriptionSalesHistories);
                //5.2- Prescription n'ayant pas donner lieu a une vente
                List<RptPrintStmt> unSoldPrescriptionHistories = this.GetUnSoldPrescriptionHistories(customerID.Value, currentBranch, currentDevise, ref order);
                histories.AddRange(unSoldPrescriptionHistories);

                // 6.1- Recuperation des ventes(Qui ne sont pas des dilatations)
                List<RptPrintStmt> saleHistories = this.GetOtherSaleHistories(customerID.Value, currentBranch, currentDevise, ref order);
                histories.AddRange(saleHistories);


                // 6.2- Recuperations des paiements(par tranche qui ne sont pas les depots et qui ne sont pas les paiements de dilatation)
                List<RptPrintStmt> paymentHistories = this.GetPaymentHistories(customerID.Value, currentBranch, currentDevise, ref order);
                histories.AddRange(paymentHistories);

                // 7- Recuperations des depots
                List<RptPrintStmt> depositHistories = this.GetDepositHistories(customerID.Value, currentBranch, currentDevise, ref order);
                depositHistories = depositHistories.Where(d => !paymentHistories.Any(p => p.RefOperation == d.RefOperation)).ToList();
                histories.AddRange(depositHistories);

                // 8- Recuperations des Don de Cadres
                /*List<RptPrintStmt> caseHistories = this.getCaseHistories(customerID.Value, currentBranch, currentDevise);
                histories.AddRange(caseHistories);
                
                // 9- Recuperations des Don de Spray
                List<RptPrintStmt> sprayHistories = this.getSprayHistories(customerID.Value, currentBranch, currentDevise);
                histories.AddRange(sprayHistories);
                
                // 10- Recuperations des Don de Cordes
                List<RptPrintStmt> robHistories = this.getRobHistories(customerID.Value, currentBranch, currentDevise);
                histories.AddRange(robHistories);*/

                // 11- Recuperations des retours
                List<RptPrintStmt> returnHistories = this.GetReturnHistories(customerID.Value, currentBranch, currentDevise, ref order);
                histories.AddRange(returnHistories);
            }

            //histories = histories.OrderBy(h1 => h1.DateOperation).ThenBy(h2 => h2.Order).ToList();

            return histories;

        }


        public JsonResult GenerateReport(int BranchID, int? CustomerID)
        {


            var model = new
            {
                data = ModelHistoCusto(BranchID, CustomerID.Value).OrderBy(c=>c.DateOperation).ThenBy(c=>c.Order)
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

        private RptPrintStmt GetRegistration(Customer customer, ref int order)
        {
            RptPrintStmt registration = new RptPrintStmt()
            {
                RptPrintStmtID = customer.GlobalPersonID,
                DateOperation = customer.Dateregister.Value.ToString("yyyy-MM-dd"),
                Description = "REGISTRATION",
                RefOperation = customer.CustomerNumber,
                Remarque = "RAS",
                PaymentReason = "RAS",
                dateOperationHours = customer.Dateregister.Value,
                Order = order++
            };

            return registration;
        }

        private List<RptPrintStmt> GetPaymentHistories(int customerID, Branch currentBranch, Devise currentDevise, ref int order)
        {

            List<RptPrintStmt> paymentHistories = new List<RptPrintStmt>();

            //traitement des reglements des ventes par avance sur la periode
            List<CustomerSlice> customerSlices = db.CustomerSlices.Where(c => c.Sale.CustomerID == customerID && /*!c.isDeposit &&*/ !c.Sale.AuthoriseSaleFK.IsDilatation).ToList();

            foreach (var customerSlice in customerSlices)
            {
                if (currentBranch == null)
                {
                    currentBranch = db.Branches.Find(SessionBusinessDay(null).BranchID);
                }
                if (currentDevise == null)
                {
                    currentDevise = db.Devises.FirstOrDefault(d => d.DefaultDevise);
                }
                //ajout des slices ds la table des historiques client
                paymentHistories.Add(this.getPaymentHistory(customerSlice, ref order));
            }

            return paymentHistories;
        }

        private List<RptPrintStmt> GetDilatationPaymentHistories(int customerID, Branch currentBranch, Devise currentDevise, ref int order)
        {

            List<RptPrintStmt> paymentHistories = new List<RptPrintStmt>();

            //traitement des reglements des ventes par avance sur la periode
            List<CustomerSlice> customerSlices = db.CustomerSlices.Where(c => c.Sale.CustomerID == customerID && !c.isDeposit && c.Sale.AuthoriseSaleFK.IsDilatation).ToList();

            foreach (var customerSlice in customerSlices)
            {
                if (currentBranch == null)
                {
                    currentBranch = db.Branches.Find(SessionBusinessDay(null).BranchID);
                }
                if (currentDevise == null)
                {
                    currentDevise = db.Devises.FirstOrDefault(d => d.DefaultDevise);
                }
                //ajout des slices ds la table des historiques client
                paymentHistories.Add(this.getPaymentHistory(customerSlice, ref order, true));
            }

            return paymentHistories;
        }

        private List<RptPrintStmt> GetDepositHistories(int customerID, Branch currentBranch, Devise currentDevise, ref int order)
        {
            List<RptPrintStmt> depositHistories = new List<RptPrintStmt>();

            List<AllDeposit> deposits = db.AllDeposits.Where(dep => dep.CustomerID == customerID && !dep.AllDepositReference.Contains("REMOVENULL")).ToList();

            foreach (var deposit in deposits)
            {
                if (currentBranch == null)
                {
                    currentBranch = deposit.PaymentMethod.Branch;
                }
                if (currentDevise == null)
                {
                    currentDevise = deposit.Devise;
                }
                depositHistories.Add(new RptPrintStmt
                {
                    Order = order++,
                    RptPrintStmtID = deposit.AllDepositID,
                    DateOperation = deposit.AllDepositDate.ToString("yyyy-MM-dd"),
                    Description = "Deposit",
                    RefOperation = deposit.AllDepositReference,
                    Remarque = "" + deposit.Amount,
                    PaymentReason = deposit.AllDepositReason,
                    dateOperationHours = deposit.AllDepositDate
                });
            }


            return depositHistories;
        }

        private List<RptPrintStmt> getCaseHistories(int customerID, Branch currentBranch, Devise currentDevise)
        {
            List<RptPrintStmt> depositHistories = new List<RptPrintStmt>();

            List<CumulSaleAndBill> cumulSaleAndBills = db.CumulSaleAndBills.Where(c => c.CustomerID == customerID && c.cases > 0).ToList();
            cumulSaleAndBills.ForEach(cumulSaleAndBill =>
            {

                depositHistories.Add(new RptPrintStmt
                {
                    RptPrintStmtID = cumulSaleAndBill.CumulSaleAndBillID,
                    DateOperation = cumulSaleAndBill.CustomerDeliverDate.Value.ToString("yyyy-MM-dd"),
                    Description = "Frame Gift",
                    RefOperation = "" + cumulSaleAndBill.CumulSaleAndBillID,
                    Remarque = "RAS",
                    PaymentReason = "RAS"
                });
            });

            return depositHistories;
        }

        private List<RptPrintStmt> getSprayHistories(int customerID, Branch currentBranch, Devise currentDevise)
        {
            List<RptPrintStmt> depositHistories = new List<RptPrintStmt>();

            List<CumulSaleAndBill> cumulSaleAndBills = db.CumulSaleAndBills.Where(c => c.CustomerID == customerID && c.spray > 0).ToList();
            cumulSaleAndBills.ForEach(cumulSaleAndBill =>
            {

                depositHistories.Add(new RptPrintStmt
                {
                    RptPrintStmtID = cumulSaleAndBill.CumulSaleAndBillID,
                    DateOperation = cumulSaleAndBill.CustomerDeliverDate.Value.ToString("yyyy-MM-dd"),
                    Description = "Spray Gift",
                    RefOperation = "" + cumulSaleAndBill.CumulSaleAndBillID,
                    Remarque = "RAS",
                    PaymentReason = "RAS"
                });
            });

            return depositHistories;
        }

        private List<RptPrintStmt> getRobHistories(int customerID, Branch currentBranch, Devise currentDevise)
        {
            List<RptPrintStmt> depositHistories = new List<RptPrintStmt>();

            List<CumulSaleAndBill> cumulSaleAndBills = db.CumulSaleAndBills.Where(c => c.CustomerID == customerID && c.robs > 0).ToList();
            cumulSaleAndBills.ForEach(cumulSaleAndBill =>
            {

                depositHistories.Add(new RptPrintStmt
                {
                    RptPrintStmtID = cumulSaleAndBill.CumulSaleAndBillID,
                    DateOperation = cumulSaleAndBill.CustomerDeliverDate.Value.ToString("yyyy-MM-dd"),
                    Description = "Rob Gift",
                    RefOperation = "" + cumulSaleAndBill.CumulSaleAndBillID,
                    Remarque = "RAS",
                    PaymentReason = "RAS"
                });
            });

            return depositHistories;
        }



        private List<RptPrintStmt> GetConsultationHistories(int customerID, Branch currentBranch, Devise currentDevise, ref int order)
        {
            List<RptPrintStmt> consultationHistories = new List<RptPrintStmt>();

            List<Consultation> consultations = db.Consultations.Where(c => c.CustomerID == customerID).ToList();

            foreach (var consultation in consultations)
            {
                consultationHistories.Add(
                    new RptPrintStmt
                    {
                        Order = order++,
                        RptPrintStmtID = consultation.ConsultationID,
                        DateOperation = consultation.DateConsultation.ToString("yyyy-MM-dd"),
                        Description = "CONSULTATION",
                        RefOperation = "" + consultation.ConsultationID,
                        Remarque = "RAS",
                        PaymentReason = consultation.RaisonRdv,
                        // en ajoutant 5 minutes, cela fera en sorte que la registration vienne avant la consultation car en bd c'est la meme date sans heure
                        dateOperationHours = consultation.DateConsultation.AddMinutes(5)
                    }
                    );
            }

            return consultationHistories;
        }

        private List<RptPrintStmt> GetPrescriptionHistories(int customerID, Branch currentBranch, Devise currentDevise)
        {
            List<RptPrintStmt> consultationHistories = new List<RptPrintStmt>();

            List<ConsultLensPrescription> prescriptions = db.ConsultLensPrescriptions.Where(c => c.ConsultDilPresc.Consultation.CustomerID == customerID).ToList();

            prescriptions.ForEach(prescription =>
            {
                consultationHistories.Add(
                    new RptPrintStmt
                    {
                        RptPrintStmtID = prescription.ConsultLensPrescriptionID,
                        DateOperation = prescription.DatePrescription.ToString("yyyy-MM-dd"),
                        Description = "PRESCRIPTION",
                        RefOperation = "" + prescription.ConsultLensPrescriptionID,
                        Remarque = "RAS",
                        PaymentReason = this.GetPrescriptionSummary(prescription)
                    }
                 );
            });

            return consultationHistories;
        }

        private string GetPrescriptionSummary(ConsultLensPrescription prescription)
        {

            string summary = "";
            string labelProduct = "";
            string label1 = "";
            string label2 = "";
            string label3 = "";

            // OG
            summary = LensConstruction.GetLensCodeByPrescription(prescription, EyeSide.OG) + "<br>";

            // OD
            summary = summary + LensConstruction.GetLensCodeByPrescription(prescription, EyeSide.OD);

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
            // OG
            summary = LensConstruction.GetLensCodeBySaleLine(saleLines[0]) + "<br>";

            // OD
            summary = summary + LensConstruction.GetLensCodeBySaleLine(saleLines[1]);

            return summary;
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

        private List<RptPrintStmt> GetPrescriptionSalesHistories(int customerID, Branch currentBranch, Devise currentDevise, ref int order)
        {
            List<RptPrintStmt> prescriptionSalesHistories = new List<RptPrintStmt>();
            List<SaleE> prescriptionSales = db.Sales.Where(c => c.CustomerID == customerID && c.AuthoriseSaleFK.ConsultDilPrescID != null && !c.AuthoriseSaleFK.IsDilatation).ToList();

            foreach (var prescriptionSale in prescriptionSales)
            {
                string prescription = "";

                SaleLine frameSaleLine = prescriptionSale.SaleLines.SingleOrDefault(sl => sl.marque != null && sl.reference != null);

                string frameLabel = frameSaleLine == null ? "" :
                ("<strong>Frame/Monture: </strong>" + frameSaleLine.marque + " - Reference " + frameSaleLine.reference);

                double TotalTTC = Util.ExtraPrices(prescriptionSale.SaleLines.Select(sl => sl.LineAmount).Sum(),
                                            prescriptionSale.RateReduction, prescriptionSale.RateDiscount, prescriptionSale.Transport, prescriptionSale.VatRate).TotalTTC;


                ConsultLensPrescription lensPrescription = db.ConsultLensPrescriptions.SingleOrDefault(
                                                            clp => clp.ConsultDilPrescID == prescriptionSale.AuthoriseSaleFK.ConsultDilPrescID);

                prescription = frameLabel + "<br>" + this.GetPrescriptionSummary(lensPrescription);
                prescriptionSalesHistories.Add(
                    new RptPrintStmt
                    {
                        Order = order++,
                        RptPrintStmtID = prescriptionSale.SaleID,
                        DateOperation = prescriptionSale.SaleDate.ToString("yyyy-MM-dd"),
                        Description = "SALE",
                        RefOperation = prescriptionSale.SaleReceiptNumber,
                        Remarque = "" + TotalTTC,
                        PaymentReason = prescription,
                        dateOperationHours = prescriptionSale.SaleDateHours
                    }
                );
            }

            return prescriptionSalesHistories;
        }

        private List<RptPrintStmt> GetUnSoldPrescriptionHistories(int customerID, Branch currentBranch, Devise currentDevise, ref int order)
        {
            List<RptPrintStmt> prescriptionSalesHistories = new List<RptPrintStmt>();

            List<ConsultLensPrescription> prescriptions = db.ConsultLensPrescriptions.Where(c => c.ConsultDilPresc.Consultation.CustomerID == customerID).ToList();

            foreach (var prescription in prescriptions)
            {
                DateTime dateOperationHours = prescription.DatePrescription;
                try
                {
                    DateTime HeureLensPrescription = DateTime.ParseExact(prescription.HeureLensPrescription, "HH:mm:ss", CultureInfo.InvariantCulture);
                    dateOperationHours.AddHours(HeureLensPrescription.Hour);
                    dateOperationHours.AddMinutes(HeureLensPrescription.Minute);
                    dateOperationHours.AddSeconds(HeureLensPrescription.Second);
                }
                catch (Exception e)
                {

                }

                prescriptionSalesHistories.Add(
                    new RptPrintStmt
                    {
                        Order = order++,
                        RptPrintStmtID = prescription.ConsultLensPrescriptionID,
                        DateOperation = prescription.DatePrescription.ToString("yyyy-MM-dd"),
                        Description = "PRESCRIPTION",
                        RefOperation = "" + prescription.ConsultLensPrescriptionID,
                        Remarque = "RAS",
                        PaymentReason = this.GetPrescriptionSummary(prescription),
                        dateOperationHours = dateOperationHours
                    }
                 );
            }

            return prescriptionSalesHistories;
        }

        private List<RptPrintStmt> GetDilatationHistories(int customerID, Branch currentBranch, Devise currentDevise, ref int order)
        {
            List<RptPrintStmt> dilatationHistories = new List<RptPrintStmt>();

            List<SaleE> dilatationSales = db.Sales.Where(c => c.CustomerID == customerID && c.AuthoriseSaleFK.IsDilatation == true).ToList();

            foreach (var dilatationSale in dilatationSales)
            {
                double TotalTTC = Util.ExtraPrices(dilatationSale.SaleLines.Select(sl => sl.LineAmount).Sum(),
                                            dilatationSale.RateReduction, dilatationSale.RateDiscount, dilatationSale.Transport, dilatationSale.VatRate).TotalTTC;
                dilatationHistories.Add(
                    new RptPrintStmt
                    {
                        Order = order++,
                        RptPrintStmtID = dilatationSale.SaleID,
                        DateOperation = dilatationSale.SaleDate.ToString("yyyy-MM-dd"),
                        Description = "Dilatation",
                        RefOperation = dilatationSale.SaleReceiptNumber,
                        Remarque = "" + TotalTTC,
                        PaymentReason = "RAS",
                        dateOperationHours = dilatationSale.SaleDateHours
                    }
                    );
            }

            return dilatationHistories;
        }

        private List<RptPrintStmt> GetOtherSaleHistories(int customerID, Branch currentBranch, Devise currentDevise, ref int order)
        {
            List<RptPrintStmt> saleHistories = new List<RptPrintStmt>();

            List<SaleE> sales = db.Sales.Where(s => s.CustomerID == customerID && s.AuthoriseSaleFK.IsDilatation == false &&
                                                    s.AuthoriseSaleFK.ConsultDilPrescID == null).ToList();

            foreach (var sale in sales)
            {
                double TotalTTC = Util.ExtraPrices(sale.SaleLines.Select(sl => sl.LineAmount).Sum(),
                                            sale.RateReduction, sale.RateDiscount, sale.Transport, sale.VatRate).TotalTTC;
                saleHistories.Add(
                    new RptPrintStmt
                    {
                        Order = order++,
                        RptPrintStmtID = sale.SaleID,
                        DateOperation = sale.SaleDate.ToString("yyyy-MM-dd"),
                        Description = "SALE",
                        RefOperation = sale.SaleReceiptNumber,
                        Remarque = "" + TotalTTC,
                        PaymentReason = GetSaleSummary(sale),
                        dateOperationHours = sale.SaleDateHours
                    }
                    );
            }

            return saleHistories;
        }


        private List<RptPrintStmt> GetReturnHistories(int customerID, Branch currentBranch, Devise currentDevise, ref int order)
        {
            List<RptPrintStmt> returnHistories = new List<RptPrintStmt>();
            List<CustomerReturn> returnSales = db.CustomerReturns.Where(r => r.Sale.CustomerID == customerID).ToList();

            foreach (var returnSale in returnSales)
            {
                currentBranch = returnSale.Sale.Branch;
                currentDevise = returnSale.Sale.Devise;
                List<CustomerReturnLine> returnLines = returnSale.CustomerReturnLines.ToList();

                if (returnLines != null && returnLines.Count > 0)
                {
                    double RetsaleAmnt = returnLines.Select(l => (l.SaleLine.LineUnitPrice * l.LineQuantity)).Sum();
                    DateTime returnDate = returnLines.Select(l => l.CustomerReturnDate).FirstOrDefault();
                    var returnCauses = "";
                    returnLines.ForEach(rl =>
                    {
                        returnCauses += rl.CustomerReturnCauses + "<br>";
                    });
                    //ajout des ventes ds la table des etats histo client
                    returnHistories.Add(
                          new RptPrintStmt
                          {
                              Order = order++,
                              RptPrintStmtID = returnSale.CustomerReturnID,
                              DateOperation = returnDate.ToString("yyyy-MM-dd"),
                              RefOperation = "SALE( " + returnSale.Sale.SaleReceiptNumber + " )",
                              Description = "RETURN",
                              Remarque = "" + RetsaleAmnt,
                              PaymentReason = returnCauses,
                              dateOperationHours = returnDate
                          });
                }
            }

            return returnHistories;
        }
        private RptPrintStmt getPaymentHistory(CustomerSlice customerSlice, ref int order, bool isDilatation = false)
        {
            var paymentHistory = new RptPrintStmt
            {
                RptPrintStmtID = customerSlice.SliceID,
                DateOperation = customerSlice.SliceDate.ToString("yyyy-MM-dd"),
                Description = customerSlice.Reference.Contains("REDP") ? "Deposit" : "PAYMENT",
                RefOperation = customerSlice.Reference,
                Remarque = "" + customerSlice.SliceAmount,
                PaymentReason = (isDilatation ? "Dilatation" : "Sale") + "( " + customerSlice.Sale.SaleReceiptNumber + " )",
                dateOperationHours = customerSlice.Reference.Contains("REDP") ? customerSlice.SliceDate : customerSlice.Sale.SaleDateHours
            };
            return paymentHistory;
        }

        private List<RptPrintStmt> getSampleData()
        {
            List<RptPrintStmt> list = new List<RptPrintStmt>();
            int i = 0;
            DateTime DateOperation = DateTime.Now;
            var registration = new RptPrintStmt
            {
                RptPrintStmtID = ++i,
                DateOperation = DateOperation.AddDays(-120).ToString("yyyy-MM-dd"),
                Description = "Registration",
                RefOperation = "" + 15478,
                Remarque = "RAS",
                PaymentReason = "RAS",
            };
            list.Add(registration);

            var consultation = new RptPrintStmt
            {
                RptPrintStmtID = ++i,
                DateOperation = DateOperation.AddDays(-120).ToString("yyyy-MM-dd"),
                Description = "Consultation",
                RefOperation = "" + 45896,
                Remarque = "RAS",
                PaymentReason = "RAS"
            };
            list.Add(consultation);

            var dilatation = new RptPrintStmt
            {
                RptPrintStmtID = ++i,
                DateOperation = DateOperation.AddDays(-120).ToString("yyyy-MM-dd"),
                Description = "Dilatation",
                RefOperation = "VPT 000345",
                Remarque = "3 000",
                PaymentReason = "RAS"
            };
            list.Add(dilatation);

            var dilatationPayment = new RptPrintStmt
            {
                RptPrintStmtID = ++i,
                DateOperation = DateOperation.AddDays(-120).ToString("yyyy-MM-dd"),
                Description = "PAYMENT",
                RefOperation = "DEP57134",
                Remarque = "3 000",
                PaymentReason = "Dilation (VPT 000345)"
            };
            list.Add(dilatationPayment);

            var prescription = new RptPrintStmt
            {
                RptPrintStmtID = ++i,
                DateOperation = DateOperation.AddDays(-120).ToString("yyyy-MM-dd"),
                Description = "PRESCRIPTION",
                RefOperation = "45896",
                Remarque = "RAS",
                PaymentReason = "OD: SV ORGANIQUE PHOTOCHROMIQUE ANTIREFLET +1.00 OG: SV ORGANIQUE PHOTOCHROMIQUE ANTIREFLET +1.25 Frame/Monture NN - Reference 6032"
            };
            list.Add(prescription);

            var salePaiment1 = new RptPrintStmt
            {
                RptPrintStmtID = ++i,
                DateOperation = DateOperation.AddDays(-120).ToString("yyyy-MM-dd"),
                Description = "PAYMENT",
                RefOperation = "DEP5713414",
                Remarque = "15 000",
                PaymentReason = "Sale (VPT 45825)"
            };
            list.Add(salePaiment1);

            var salePaiment2 = new RptPrintStmt
            {
                RptPrintStmtID = ++i,
                DateOperation = DateOperation.AddDays(-120).ToString("yyyy-MM-dd"),
                Description = "PAYMENT",
                RefOperation = "DEP574586",
                Remarque = "15 000",
                PaymentReason = "Sale (VPT 45825)"
            };
            list.Add(salePaiment2);

            var caseGift = new RptPrintStmt
            {
                RptPrintStmtID = ++i,
                DateOperation = DateOperation.AddDays(-120).ToString("yyyy-MM-dd"),
                Description = "Case Gift",
                RefOperation = "" + 45896,
                Remarque = "RAS",
                PaymentReason = "RAS"
            };
            list.Add(caseGift);

            var sprayGift = new RptPrintStmt
            {
                RptPrintStmtID = ++i,
                DateOperation = DateOperation.AddDays(-120).ToString("yyyy-MM-dd"),
                Description = "Spray Gift",
                RefOperation = "" + 45896,
                Remarque = "RAS",
                PaymentReason = "RAS"
            };
            list.Add(sprayGift);

            var robsGift = new RptPrintStmt
            {
                RptPrintStmtID = ++i,
                DateOperation = DateOperation.AddDays(-120).ToString("yyyy-MM-dd"),
                Description = "Robs Gift",
                RefOperation = "" + 45896,
                Remarque = "RAS",
                PaymentReason = "RAS"
            };
            list.Add(robsGift);



            return list;
        }

    }
}