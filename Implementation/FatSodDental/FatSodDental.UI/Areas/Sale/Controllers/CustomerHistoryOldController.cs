using FatSod.DataContext.Initializer;
using FatSod.Security.Abstracts;
using FatSod.Security.Entities;
using FatSod.Supply.Entities;
using FatSodDental.UI.Controllers;
using FatSodDental.UI.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using SaleE = FatSod.Supply.Entities.Sale;
using FatSod.Supply.Abstracts;
using FatSod.Ressources;
using System.IO;
using CrystalDecisions.CrystalReports.Engine;
using FatSodDental.UI.Filters;
using FastSod.Utilities.Util;
using CrystalDecisions.Shared;
using FatSod.Report.WrapReports;
using FatSod.DataContext.Concrete;
using System.Web.UI;
using ExtPartialViewResult = Ext.Net.MVC.PartialViewResult;

namespace FatSodDental.UI.Areas.Sale.Controllers
{
    [Authorize(Order = 1)]
    [TakeBusinessDay(Order = 2)]
    public class CustomerHistoryOldController : BaseController
    {
        private const string CONTROLLER_NAME = "Sale/CustomerHistory";
        private const string VIEW_NAME = "Index";
        //*********************

        private IBusinessDay _busDayRepo;
        
        // GET: CashRegister/State
        public CustomerHistoryOldController(
            IBusinessDay busDayRepo
            )
        {
            this._busDayRepo = busDayRepo;
        }
        //Enable to get hitoric of cash register
        [OutputCache(Duration = 3600)] 
        public ActionResult Index()
        {

            //ExtPartialViewResult rPVResult = new ExtPartialViewResult
            //{
            //    ClearContainer = true,
            //    //RenderMode = RenderMode.AddTo,
            //    ContainerId = "Body",
            //    WrapByScriptTag = false
            //};

            //Session["Curent_Controller"] = CONTROLLER_NAME;
            //Session["Curent_Page"] = VIEW_NAME;
            //We test if this user has an autorization to get this sub menu
            //if (!LoadAction.isAutoriseToGetSubMenu(SessionProfileID, CodeValue.Sale.Report.SMCODECUSTHIST, db))
            //{
            //    RedirectToAction("NotAuthorized", "User");
            //}
            //Branch currentBranch = _branchRepository.Find(_userBranch.Find(SessionGlobalPersonID).BranchID);
            //BusinessDay businessDay = _busDayRepo.GetOpenedBusinessDay(currentBranch);
            //ViewBag.BusnessDayDate = businessDay.BDDateOperation;
            //Session["BusnessDayDate"] = businessDay.BDDateOperation;
            //this.GetCmp<Panel>("Body").LoadContent(new ComponentLoader
            //{
            //    Url = Url.Action("Index"),
            //    DisableCaching = false,
            //    Mode = LoadMode.Frame
            //});
            return View();
        }


        [HttpPost]
        public ActionResult PrintBill(int CustomerID, DateTime BeginDate, DateTime EndDate)
        {
            Session["BeginDate"] = BeginDate;
            Session["EndDate"] = EndDate;
            Session["CustomerID"] = CustomerID;
            this.GetCmp<Panel>("RptCustHist").Hidden = false;
            this.GetCmp<Panel>("RptCustHist").LoadContent(new ComponentLoader
            {
                Url = Url.Action("GenerateReport"),
                DisableCaching = false,
                Mode = LoadMode.Frame
            });
            return this.Direct();
        }

        /// <summary>
        /// Retourne le montant total des dépôts éffectués par le client avant la date
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        public double TotalDepotSliceBefore(Customer customer, DateTime BeginDate)
        {
            double res = 0;

            //Somme des dépôts qui n'utilisent pas le compte d'épargne
            List<CustomerSlice> lstCustSliceBefore = db.CustomerSlices.Where(cs => cs.Sale.CustomerID == customer.GlobalPersonID &&
                                                                                         cs.SliceDate < BeginDate && (cs.PaymentMethod is SavingAccount) == false).ToList();

            double depotSliceBefore = lstCustSliceBefore!=null ? lstCustSliceBefore.Select(cs1 => cs1.SliceAmount).Sum() :0;
            
           
            res = depotSliceBefore ;

            return res;
        }

        /// <summary>
        /// Retourne le montant total des achats du clients avant la date
        /// </summary>
        /// <param name="customer"></param>
        /// <param name="BeginDate"></param>
        /// <returns></returns>
        public double TotalAchatBefore(Customer customer, DateTime BeginDate)
        {
            double res = 0;

            //recuperation du solde avant la date debut
            //1-recup du solde du client sans tenir en cpte les depots avant la date
            List<SaleE> venteRegleBefore = db.Sales.Where(c => c.CustomerID == customer.GlobalPersonID && c.SaleDate < BeginDate).ToList();
            venteRegleBefore.ForEach(regsale =>
            {
                double AmtRegByAccount = 0;
                double AmtRegAutrement = 0;
                double saleAmnt = regsale.SaleLines.Select(l => l.LineAmount).Sum();
                ExtraPrice extra = Util.ExtraPrices(saleAmnt, regsale.RateReduction, regsale.RateDiscount, regsale.Transport, regsale.VatRate);
                res += extra.TotalTTC;
            });

            return res;
        }

        private List<RptPrintStmt> ModelHistoCusto(DateTime BeginDate, DateTime EndDate)
        {
            string Sens = "";
            double depotEpargneBefore = 0d;
            
            List<RptPrintStmt> modelRpt = new List<RptPrintStmt>();
            //DateTime BeginDate = (DateTime)Session["BeginDate"];
            //DateTime EndDate = (DateTime)Session["EndDate"];

            int customerID = (int)Session["CustomerID"];
            Customer customer = (from cust in db.Customers
                                 where cust.GlobalPersonID == customerID
                                 select cust).SingleOrDefault();
            List<RptPrintStmt> model = new List<RptPrintStmt>();
            List<CustomerSlice> slicelist = new List<CustomerSlice>();
            List<Deposit> depositlist = new List<Deposit>();

            string defaultImgPath = Server.MapPath("~/Content/Images/App/deboy-logo.png");
            byte[] COMPANY_LOGO = System.IO.File.ReadAllBytes(defaultImgPath);

            Branch beginBranch = new Branch();
            Devise beginDevise = new Devise();


            double SoldeMntRegleSansDepot = this.TotalDepotSliceBefore(customer, BeginDate);
            double achatTTCBefore = this.TotalAchatBefore(customer, BeginDate);

            //recuperation du saving acct
            SavingAccount saveAcct = db.SavingAccounts.SingleOrDefault(sa => sa.CustomerID == customer.GlobalPersonID);
            
            //2-recup de ts les depots du client avant la date debut
            //Somme des dépôts d"épargne du client before
            if (saveAcct != null)
            {
                List<Deposit> lstdepotEpargneBefore = db.Deposits.Where(d => d.SavingAccountID == saveAcct.ID && d.DepositDate < BeginDate).ToList();
                depotEpargneBefore = lstdepotEpargneBefore != null ? lstdepotEpargneBefore.Select(d => d.Amount).Sum() : 0;
            }
            else { depotEpargneBefore = 0; }

            double totalDepositBefore = SoldeMntRegleSansDepot + depotEpargneBefore;
            double soldeReglementOuverture = achatTTCBefore - totalDepositBefore;

            
            //traitement historique pour la periode choisie

            //recuperation des ttes les ventes de la periode
            List<SaleE> salelist = db.Sales.Where(c => c.CustomerID == customer.GlobalPersonID && (c.SaleDate >= BeginDate && c.SaleDate <= EndDate)).ToList();
            salelist.ForEach(allSales =>
                {
                    Branch currentBranch = allSales.Branch;
                    double saleAmnt = allSales.SaleLines.Select(l => l.LineAmount).Sum();
                    ExtraPrice extra = Util.ExtraPrices(saleAmnt, allSales.RateReduction, allSales.RateDiscount, allSales.Transport, allSales.VatRate);

                    //ajout des ventes ds la table des etats histo client
                    model.Add(
                          new RptPrintStmt
                          {
                              AcctNo = customer.CNI,
                              AcctName = customer.Name,
                              Devise = allSales.Devise.DeviseCode,
                              LibDevise = allSales.Devise.DeviseLabel,
                              DateOperation = allSales.SaleDate.Date,
                              RefOperation = allSales.SaleReceiptNumber,
                              Description = "Sale By " + allSales.Representant,
                              RepDebit = achatTTCBefore,
                              RepCredit = totalDepositBefore,
                              MtDebit = extra.TotalTTC,
                              MtCredit = 0,
                              Agence = currentBranch.BranchCode,
                              LibAgence = currentBranch.BranchName,
                              LogoBranch = db.Files.FirstOrDefault(f => f.GlobalPersonID == Company.GlobalPersonID) != null ? db.Files.FirstOrDefault(f => f.GlobalPersonID == Company.GlobalPersonID).Content : COMPANY_LOGO
                          }
             );
                }
        );

            //traitement des reglements pas par Cpte du client
            List<SaleE> salelistCustomer = db.Sales.Where(c => c.CustomerID == customer.GlobalPersonID).ToList();
            salelistCustomer.ForEach(allSales =>
                {
                    Branch currentBranch = allSales.Branch;
                    if (saveAcct != null)
                    {
                        slicelist = allSales.CustomerSlices.Where(sl => saveAcct != null && sl.PaymentMethodID != saveAcct.ID && (sl.SliceDate >= BeginDate && sl.SliceDate <= EndDate)).ToList();
                    }
                    else
                    {
                        slicelist = allSales.CustomerSlices.Where(sl => (sl.SliceDate >= BeginDate && sl.SliceDate <= EndDate)).ToList();
                    }

                    slicelist.ForEach(allslices =>
                    {
                        //ajout des slices ds la table des historiques client
                        double sliceAmount = allslices.SliceAmount;
                        model.Add(
                            new RptPrintStmt
                            {
                                AcctNo = customer.CNI,
                                AcctName = customer.Name,
                                Devise = allSales.Devise.DeviseCode,
                                LibDevise = allSales.Devise.DeviseLabel,
                                DateOperation = allslices.SliceDate,
                                RefOperation = "SLIC" + allslices.SliceID,
                                Description = "Reg Bill " + allslices.Sale.SaleReceiptNumber,
                                RepDebit = achatTTCBefore,
                                RepCredit = totalDepositBefore,
                                MtDebit = 0,
                                MtCredit = sliceAmount,
                                Agence = currentBranch.BranchCode,
                                LibAgence = currentBranch.BranchName,
                                LogoBranch = db.Files.FirstOrDefault(f => f.GlobalPersonID == Company.GlobalPersonID) != null ? db.Files.FirstOrDefault(f => f.GlobalPersonID == Company.GlobalPersonID).Content : COMPANY_LOGO
                            });
                    });
                });
            /****** traitement des depots d'epargnes    * *****/
            //we take her all sales
            if (saveAcct != null)
            {
                depositlist = db.Deposits.Where(dep => dep.SavingAccountID == saveAcct.ID && (dep.DepositDate >= BeginDate && dep.DepositDate <= EndDate)).ToList();
            depositlist.ForEach(alldeposits =>
                    {
                        double saleAmnt = alldeposits.Amount;
                        Branch currentBranch = alldeposits.PaymentMethod1.Branch;
                        model.Add(
                      new RptPrintStmt
                              {
                                  AcctNo = customer.CNI,
                                  AcctName = customer.Name,
                                  Devise = alldeposits.Devise.DeviseCode,
                                  LibDevise = alldeposits.Devise.DeviseLabel,
                                  DateOperation = alldeposits.DepositDate,
                                  RefOperation = "DEP" + alldeposits.DepositID,
                                  Description = alldeposits.DepositReason == null ? "DEP by " + customer.Name : alldeposits.DepositReason,
                                  RepDebit = achatTTCBefore,
                                  RepCredit = totalDepositBefore,
                                  MtDebit = saleAmnt < 0 ? -1 * saleAmnt : 0,
                                      MtCredit = saleAmnt > 0 ? saleAmnt : 0,
                                  Agence = currentBranch.BranchCode,
                                  LibAgence = currentBranch.BranchName,
                                  LogoBranch = db.Files.FirstOrDefault(f => f.GlobalPersonID == Company.GlobalPersonID) != null ? db.Files.FirstOrDefault(f => f.GlobalPersonID == Company.GlobalPersonID).Content : COMPANY_LOGO
                              }
         );
                    }
                          );
            }
            if (salelist.Count == 0 && depositlist.Count == 0)
            {
                //ecriture bbf
                model.Add(
                      new RptPrintStmt
                      {
                          AcctNo = customer.CNI,
                          AcctName = customer.Name,
                          Devise = beginDevise.DeviseCode,
                          LibDevise = beginDevise.DeviseLabel,
                          RepDebit = achatTTCBefore,
                          RepCredit = totalDepositBefore,
                          DateOperation = BeginDate,
                          MtDebit = 0,
                          MtCredit = 0,
                          Sens = "",
                          Agence = beginBranch.BranchCode,
                          LibAgence = beginBranch.BranchName,
                          LogoBranch = db.Files.FirstOrDefault(f => f.GlobalPersonID == Company.GlobalPersonID) != null ? db.Files.FirstOrDefault(f => f.GlobalPersonID == Company.GlobalPersonID).Content : COMPANY_LOGO
                      }
                 );

            }
            return model;
        }
        public void GenerateReport()
        {
            try
            {
                bool isValid = false;
                ReportDocument rptH = new ReportDocument();

                DateTime BeginDate = (DateTime)Session["BeginDate"];
                DateTime EndDate = (DateTime)Session["EndDate"];
                double Solde = 0;
                List<object> model = new List<object>();

                double CumulCredit = ModelHistoCusto(BeginDate, EndDate).Select(m => m.RepCredit).Max();
                double CumulDebit = ModelHistoCusto(BeginDate, EndDate).Select(m => m.RepDebit).Max();

                List<RptPrintStmt> listCustOp = ModelHistoCusto(BeginDate, EndDate).OrderBy(m => m.DateOperation).ThenBy(m => m.RefOperation).ToList();
                foreach (RptPrintStmt c in listCustOp)
                {
                    Solde = 0d;
                    CumulCredit += c.MtCredit;
                    CumulDebit += c.MtDebit;
                    Solde = CumulCredit - CumulDebit;

                    model.Add(
                                 new
                                 {
                                     RptEtatsJournalID = c.RptPrintStmtID,
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
                                     RepDebit = c.RepDebit,
                                     RepCredit = c.RepCredit,
                                     Solde = Solde,
                                     Sens = Solde > 0 ? "Cr" : "Db",
                                     LogoBranch = c.LogoBranch
                                 }
            );
                    isValid = true;
                }

                if (isValid)
                {
                    string path = Server.MapPath("~/Reports/Accounting/Stmt.rpt");
                    rptH.Load(path);
                    rptH.SetDataSource(model);

                    rptH.SetParameterValue("CompanyName", Company.Name);
                    rptH.SetParameterValue("TelFax", "Tel:" + Company.Adress.AdressPhoneNumber + " Fax:" + Company.Adress.AdressFax);
                    rptH.SetParameterValue("RepTitle", Resources.rptCustHistTitle);
                    rptH.SetParameterValue("Operator", CurrentUser.Name);

                    rptH.SetParameterValue("RegionCountry", Company.Adress.Quarter.Town.Region.RegionLabel);
                    rptH.SetParameterValue("Adresse", Company.Adress.Quarter.Town.Region.Country.CountryLabel + " - " + Company.Adress.Quarter.Town.TownLabel);

                    rptH.SetParameterValue("BeginDate", BeginDate);
                    rptH.SetParameterValue("EndDate", EndDate);
                    bool isDownloadRpt = (bool)Session["isDownloadRpt"];
                    rptH.ExportToHttpResponse(ExportFormatType.PortableDocFormat, System.Web.HttpContext.Current.Response, false, "Stmt");

                    // Clear all sessions value
                    Session["CustomerID"] = null;
                }
                else
                {
                    Response.Write("Nothing Found; No Report name found");
                }
            }
            catch (Exception ex)
            {
                Response.Write(ex.ToString());
            }
        }
        private Company Company
        {
            get
            {
                return db.Companies.FirstOrDefault();
            }
        }
        //============== Method for customers's bill
        public ActionResult OpenedBusday()
        {
            List<object> list = new List<object>();
            List<BusinessDay> busDays = _busDayRepo.GetOpenedBusinessDay(CurrentUser);

            foreach (BusinessDay busDay in busDays)
            {
                list.Add(
                    new
                    {
                        BranchID = busDay.BranchID,
                        BranchName = busDay.BranchName
                    }
                    );
            }

            return this.Store(list);

        }
        public StoreResult LoadThirdPartyAccounts(int? BranchID)
        {
            List<object> customers = new List<object>();
            List<Customer> customers1 = new List<Customer>();
            customers1 = db.Customers.ToList();
            foreach (Customer c in customers1)
            {
                customers.Add(
                    new
                    {
                        CustomerFullName = c.CustomerFullName,
                        PersonID = c.GlobalPersonID
                    }
                );

            }
            return this.Store(customers);
        }
    }

}