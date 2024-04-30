using FastSod.Utilities.Util;
using FatSod.DataContext.Repositories;
using FatSod.Report.WrapReports;
using FatSod.Security.Abstracts;
using FatSod.Security.Entities;
using FatSod.Supply.Abstracts;
using FatSod.Supply.Entities;
using System;
using System.Collections.Generic;
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
    public class CustomerJourneyController : BaseController
    {
        
        private IBusinessDay _busDayRepo;
        private IRepository<FatSod.Security.Entities.File> _fileRepository;
        private ISale _saleRepository;
        private ICustomerReturn _customerReturnRepository;
        private IDeposit _depositRepository;

        List<BusinessDay> bdDay;

        // GET: CashRegister/State
        public CustomerJourneyController(
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
            foreach (Customer customer in db.People.OfType<Customer>().Where(c => c.Name.StartsWith(filter.ToLower())).ToArray().OrderBy(c => c.Name))
            {
                string itemLabel = "";

                //si le client est une personne physique, on renvoie son nom et son prénom, si c'est une entreprise, on renvoie son nom
                itemLabel = customer.Name + ((customer.Description==null) ? "" : " " + customer.Description) ;

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

            List<RptPrintStmt> modelRpt = new List<RptPrintStmt>();


            List<RptPrintStmt> model = new List<RptPrintStmt>();

            Customer customer = (from cust in db.Customers
                                 where cust.GlobalPersonID == customerID
                                 select cust).SingleOrDefault();
           
            Branch currentBranch = db.Branches.Find(BranchID);// SessionBusinessDay(null).Branch;
            Devise currentDevise = db.Devises.Where(dev => dev.DefaultDevise).FirstOrDefault();

            int i = 0;

            //recuperation des ttes les ventes de la periode
            if (customerID.Value > 0)
            {
                //consultation
                List<Customer> consultlist = db.Customers.Where(c => c.GlobalPersonID == customerID).ToList();
                consultlist.ForEach(allconsult =>
                {
                    string prescription = "";
                    List <SaleE> consultsalelist = db.Sales.Where(c => c.CustomerID == customerID).ToList();
                    consultsalelist.ForEach(allSales =>
                    {
                        string labelProduct = "";
                        string label1 = "";
                        string label2 = "";
                        string label3 = "";

                        foreach (SaleLine c in allSales.SaleLines.ToList())
                        {
                            labelProduct = (c.marque != null && c.reference != null) ? "Frame/Monture " + c.marque + " - Reference " + c.reference : (c.Product is OrderLens) ? (c.SupplyingName == null) ? c.Product.ProductCode : c.SupplyingName : c.Product.ProductLabel;
                            if (i == 0)
                            {
                                label1 = labelProduct;
                            }
                            if (i == 1)
                            {
                                label2 = labelProduct;
                            }
                            if (i == 2)
                            {
                                label3 = labelProduct;
                            }
                            i += 1;
                        }
                        prescription = label1 + " " + label2 + " " + label3;
                    });
                    
                    //ajout des ventes ds la table des etats histo client
                    model.Add(
                          new RptPrintStmt
                          {
                              AcctNo = customer.CNI,
                              AcctName = customer.Name,
                              Devise = currentDevise.DeviseCode,
                              LibDevise = currentDevise.DeviseLabel,
                              DateOperation = allconsult.Dateregister.Value.ToString("yyyy-MM-dd"),
                              RefOperation = allconsult.CNI,
                              Description = "Customer Consultation " + customer.Name,
                              MtDebit = 0,
                              MtCredit = 0,
                              Agence = currentBranch.BranchCode,
                              LibAgence = currentBranch.BranchName,
                              SaleID = 0,
                              Remarque= prescription
                          });
                });

                
                List<SaleE> salelist = db.Sales.Where(c => c.CustomerID == customerID).ToList();
                salelist.ForEach(allSales =>
                {
                    currentBranch = allSales.Branch;
                    currentDevise = allSales.Devise;
                    double saleAmnt = allSales.SaleLines.Select(l => l.LineAmount).Sum();
                    ExtraPrice extra = Util.ExtraPrices(saleAmnt, allSales.RateReduction, allSales.RateDiscount, allSales.Transport, allSales.VatRate);

                    
                    //foreach( SaleLine newsaleline in allSales.SaleLines.ToList())
                    //{
                    //    string prescription = newsaleline.OeilDroiteGauche
                    //}

                    //ajout des ventes ds la table des etats histo client
                    model.Add(
                          new RptPrintStmt
                          {
                              AcctNo = customer.CNI,
                              AcctName = customer.Name,
                              Devise = currentDevise.DeviseCode,
                              LibDevise = currentDevise.DeviseLabel,
                              DateOperation = allSales.SaleDate.Date.ToString("yyyy-MM-dd"),
                              RefOperation = allSales.SaleReceiptNumber,
                              Description = "Sale to " + customer.Name ,
                              MtDebit = extra.TotalTTC,
                              MtCredit = 0,
                              Agence = currentBranch.BranchCode,
                              LibAgence = currentBranch.BranchName,
                              SaleID = allSales.SaleID,
                              Remarque = extra.TotalTTC.ToString("0.0")
                          });
                });

                

                //traitement des reglements des ventes par avance sur la periode
                List<CustomerSlice> perCustomerSlice = db.CustomerSlices.Where(c => c.Sale.CustomerID == customerID && !c.isDeposit).ToList();
                perCustomerSlice.ForEach(allslices =>
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
                    double sliceAmount = allslices.SliceAmount;
                    model.Add(
                        new RptPrintStmt
                        {
                            AcctNo = customer.CNI,
                            AcctName = customer.Name,
                            Devise = currentDevise.DeviseCode,
                            LibDevise = currentDevise.DeviseLabel,
                            DateOperation = allslices.SliceDate.ToString("yyyy-MM-dd"),
                            RefOperation = "DEP" + allslices.SliceID,
                            Description = "Reg Bill " + allslices.Sale.SaleReceiptNumber,
                            MtDebit = 0,
                            MtCredit = sliceAmount,
                            Agence = currentBranch.BranchCode,
                            LibAgence = currentBranch.BranchName,
                            SaleID = allslices.SaleID,
                            Remarque= sliceAmount.ToString("0.0")
                        });
                });
                /****** traitement des depots   * *****/

                List<AllDeposit> depositlist = db.AllDeposits.Where(dep => dep.CustomerID == customerID && !dep.AllDepositReference.Contains("REMOVENULL")).ToList();
                depositlist.ForEach(alldeposits =>
                {
                    double saleAmnt = alldeposits.Amount;
                    if (currentBranch == null)
                    {
                        currentBranch = alldeposits.PaymentMethod.Branch;
                    }
                    if (currentDevise == null)
                    {
                        currentDevise = alldeposits.Devise;
                    }
                    //if (!alldeposits.AllDepositReference.Contains("REMOVENULL"))
                    //{
                    model.Add
                (
                    new RptPrintStmt
                    {
                        AcctNo = customer.CNI,
                        AcctName = customer.Name,
                        Devise = currentDevise.DeviseCode,
                        LibDevise = currentDevise.DeviseLabel,
                        DateOperation = alldeposits.AllDepositDate.ToString("yyyy-MM-dd"),
                        RefOperation = alldeposits.AllDepositReference,
                        Description = alldeposits.AllDepositReason == null ? "DEP by " + customer.Name : "DEPOSIT FOR " + alldeposits.AllDepositReason,
                        MtDebit = 0,
                        MtCredit = saleAmnt,
                        Agence = currentBranch.BranchCode,
                        LibAgence = currentBranch.BranchName,
                        SaleID = alldeposits.AllDepositID,
                        Remarque = saleAmnt.ToString("0.0")
                    }
                    );
                }
                    );

                /****** traitement des retours   * *****/
                //recuperation des ttes les ventes de la periode
                List<CustomerReturn> RetSale = db.CustomerReturns.Where(c => c.Sale.CustomerID == customerID).ToList();

                RetSale.ToList().ForEach(allRestSales =>
                {
                    currentBranch = allRestSales.Sale.Branch;
                    currentDevise = allRestSales.Sale.Devise;
                    List<CustomerReturnLine> custRetSalList = allRestSales.CustomerReturnLines.ToList();

                    if (custRetSalList != null && custRetSalList.Count > 0)
                    {
                        double RetsaleAmnt = custRetSalList.Select(l => (l.SaleLine.LineUnitPrice * l.LineQuantity)).Sum();
                        ExtraPrice extra = Util.ExtraPrices(RetsaleAmnt, allRestSales.RateReduction, allRestSales.RateDiscount, allRestSales.Transport, allRestSales.VatRate);
                        //ajout des ventes ds la table des etats histo client
                        model.Add(
                              new RptPrintStmt
                              {
                                  AcctNo = customer.CNI,
                                  AcctName = customer.Name,
                                  Devise = currentDevise.DeviseCode,
                                  LibDevise = currentDevise.DeviseLabel,
                                  DateOperation = custRetSalList.Select(l => l.CustomerReturnDate).FirstOrDefault().Date.ToString("yyyy-MM-dd"),
                                  RefOperation = allRestSales.Sale.SaleReceiptNumber,
                                  Description =  "Return For " + customer.Name,
                                  MtDebit = 0,
                                  MtCredit = extra.TotalTTC,
                                  Agence = currentBranch.BranchCode,
                                  LibAgence = currentBranch.BranchName,
                                  SaleID = allRestSales.SaleID,
                                  Remarque = extra.TotalTTC.ToString("0.0")
                              });
                    }

                }
            );




            }

            
            i = 0;
            List<RptPrintStmt> modellist = new List<RptPrintStmt>();

            //tri et calcul des cumul
            List<RptPrintStmt> listCustOp = model.OrderBy(m => m.DateOperation).ThenBy(m => m.SaleID).ToList();
            foreach (RptPrintStmt c in listCustOp)
            {
                i += 1;
                modellist.Add(
                        new RptPrintStmt
                        {
                            RptPrintStmtID = i,
                            Agence = c.Agence,
                            LibAgence = c.LibAgence,
                            Devise = c.Devise,
                            LibDevise = c.LibDevise,
                            AcctNo = c.AcctNo,
                            AcctName = c.AcctName,
                            RefOperation = c.RefOperation,
                            Description = c.Description,
                            DateOperation = c.DateOperation,
                            MtDebit = c.MtDebit,
                            MtCredit = c.MtCredit,
                            Remarque = c.Remarque,
                            SaleID = c.SaleID.Value
                        }
                    );

            }
            return modellist;

        }


        public JsonResult GenerateReport(int BranchID, int? CustomerID)
        {


            var model = new
            {
                data = from c in ModelHistoCusto(BranchID, CustomerID.Value).ToList()
                       select new
                       {

                           RptPrintStmtID = c.RptPrintStmtID,
                           Agence = c.Agence,
                           LibAgence = c.LibAgence,
                           Devise = c.Devise,
                           LibDevise = c.LibDevise,
                           AcctNo = c.AcctNo,
                           AcctName = c.AcctName,
                           RefOperation = c.RefOperation,
                           Description = c.Description,
                           DateOperation = c.DateOperation,
                           Remarque = c.Remarque,
                           SaleID = c.SaleID.Value
                       }
            };

            //return Json(model, JsonRequestBehavior.AllowGet);
            var jsonResult = Json(model, JsonRequestBehavior.AllowGet);
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

    }
}