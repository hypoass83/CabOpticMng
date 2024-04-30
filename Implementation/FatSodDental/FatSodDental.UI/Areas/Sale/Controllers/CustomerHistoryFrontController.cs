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
    public class CustomerHistoryFrontController : BaseController
    {
        private const string CONTROLLER_NAME = "Sale/CustomerHistoryFront";
        private const string VIEW_NAME = "Index";
        //*********************

        private IBusinessDay _busDayRepo;
        private IRepository<FatSod.Security.Entities.File> _fileRepository;
        private ISale _saleRepository;
        private ICustomerReturn _customerReturnRepository;
        private IDeposit _depositRepository;
        // GET: CashRegister/State
        public CustomerHistoryFrontController(
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

            return View();
        }


        [HttpPost]
        public ActionResult PrintBill(int CustomerID, DateTime BeginDate, DateTime EndDate)
        {
            Session["BeginDate"] = BeginDate;
            Session["EndDate"] = EndDate;
            Session["CustomerID"] = CustomerID;
            this.GetCmp<Panel>("PanelReport").Hidden = false;
            this.GetCmp<Panel>("PanelReport").LoadContent(new ComponentLoader
            {
                Url = Url.Action("GenerateReport"),
                DisableCaching = false,
                Mode = LoadMode.Frame
            });
            return this.Direct();
        }
        /*
/// <summary>
/// Retourne le montant total des dépôts éffectués par le client avant la date
/// </summary>
/// <param name="customer"></param>
/// <returns></returns>
public double TotalDepotSliceBefore(Customer customer, DateTime BeginDate)
{
   double res = 0;

   //Somme des dépôts direct sur achat
   List<CustomerSlice> lstCustSliceBefore = db.CustomerSlices.Where(cs => cs.Sale.CustomerID == customer.GlobalPersonID &&
                                                                                cs.SliceDate < BeginDate.Date && !cs.isDeposit).ToList();

   double depotCustomerSliceBefore = lstCustSliceBefore != null ? lstCustSliceBefore.Select(cs1 => cs1.SliceAmount).Sum() : 0;

   //recupartion des depot apres achat
   List<AllDeposit> lstAllDeposit = db.AllDeposits.Where(ad => ad.CustomerID == customer.GlobalPersonID && ad.AllDepositDate < BeginDate.Date && !ad.AllDepositReference.Contains("REMOVENULL")).ToList();
   double depotAllDepBefore = lstAllDeposit != null ? lstAllDeposit.Select(dep => dep.Amount).Sum() : 0;

   double depotSliceBefore = depotCustomerSliceBefore + depotAllDepBefore;
   res = depotSliceBefore;

   return res;
}
        

/// <summary>
/// Retourne le montant total des achats  du clients avant la date
/// </summary>
/// <param name="customer"></param>
/// <param name="BeginDate"></param>
/// <returns></returns>
public double TotalAchatBefore(Customer customer, DateTime BeginDate)
{
   double res = 0;
   SaleE getSaleBefore = new SaleE();
   //recuperation du solde avant la date debut
   //1-recup du solde du client sans tenir en cpte les depots avant la date
   List<SaleE> venteRegleBefore = db.Sales.Where(c => c.CustomerID == customer.GlobalPersonID && c.SaleDate < BeginDate).ToList();
   venteRegleBefore.ForEach(regsale =>
   {
       //considerons les ventes sans retours
       getSaleBefore = _customerReturnRepository.GetRealSale(regsale);
       double saleAmnt = getSaleBefore.SaleLines.Select(l => l.LineAmount).Sum();
       ExtraPrice extra = Util.ExtraPrices(saleAmnt, getSaleBefore.RateReduction, getSaleBefore.RateDiscount, getSaleBefore.Transport, getSaleBefore.VatRate);
       res += extra.TotalTTC;
   });

   return res;
}

 */

        private List<RptPrintStmt> ModelHistoCusto(DateTime BeginDate, DateTime EndDate)
        {
            string Sens = "";
            double depotEpargneBefore = 0d;

            List<RptPrintStmt> modelRpt = new List<RptPrintStmt>();
            //DateTime BeginDate = (DateTime)Session["BeginDate"];
            //DateTime EndDate = (DateTime)Session["EndDate"];

            List<BusinessDay> UserBusDays = (List<BusinessDay>)Session["UserBusDays"];

            int customerID = (int)Session["CustomerID"];
            Customer customer = (from cust in db.Customers
                                 where cust.GlobalPersonID == customerID
                                 select cust).SingleOrDefault();
            List<RptPrintStmt> model = new List<RptPrintStmt>();

            List<CustomerSlice> slicelist = new List<CustomerSlice>();
            List<AllDeposit> depositlist = new List<AllDeposit>();

            string defaultImgPath = Server.MapPath("~/Content/Images/App/deboy-logo.png");
            byte[] COMPANY_LOGO = System.IO.File.ReadAllBytes(defaultImgPath);

            Branch currentBranch = new Branch();
            Devise currentDevise = new Devise();


            double totalDepositBefore = _depositRepository.TotalDepotSliceBefore(customer, BeginDate);
            double achatTTCBefore = _saleRepository.TotalAchatBefore(customer, BeginDate);

            double soldeReglementOuverture = achatTTCBefore - totalDepositBefore;


            //traitement historique pour la periode choisie

            //recuperation des ttes les ventes de la periode
            List<SaleE> salelist = db.Sales.Where(c => c.CustomerID == customerID && (c.SaleDate >= BeginDate.Date && c.SaleDate <= EndDate.Date)).ToList();
            salelist.ForEach(allSales =>
            {
                currentBranch = allSales.Branch;
                currentDevise = allSales.Devise;
                double saleAmnt = allSales.SaleLines.Select(l => l.LineAmount).Sum();
                ExtraPrice extra = Util.ExtraPrices(saleAmnt, allSales.RateReduction, allSales.RateDiscount, allSales.Transport, allSales.VatRate);

                //ajout des ventes ds la table des etats histo client
                model.Add(
                      new RptPrintStmt
                      {
                          AcctNo = customer.CNI,
                          AcctName = customer.Name,
                          Devise = currentDevise.DeviseCode,
                          LibDevise = currentDevise.DeviseLabel,
                          DateOperation = allSales.SaleDate.Date,
                          RefOperation = allSales.SaleReceiptNumber,
                          Description =  "Sold to " + customer.Name + "/" + allSales.PoliceAssurance ,
                          RepDebit = achatTTCBefore,
                          RepCredit = totalDepositBefore,
                          MtDebit = extra.TotalTTC,
                          MtCredit = 0,
                          Agence = currentBranch.BranchCode,
                          LibAgence = currentBranch.BranchName,
                          SaleID = allSales.SaleID,
                          LogoBranch = _fileRepository.FindAll.FirstOrDefault(f => f.GlobalPersonID == Company.GlobalPersonID) != null ? _fileRepository.FindAll.FirstOrDefault(f => f.GlobalPersonID == Company.GlobalPersonID).Content : COMPANY_LOGO
                      }
         );
            }
        );
            
            //traitement des reglements des ventes par avance sur la periode
            List<CustomerSlice> perCustomerSlice = db.CustomerSlices.Where(c => c.Sale.CustomerID == customerID && (c.SliceDate >= BeginDate.Date && c.SliceDate <= EndDate.Date) && !c.isDeposit).ToList();
            perCustomerSlice.ForEach(allslices =>
                {
                    if (currentBranch==null)
                    {
                        currentBranch = db.Branches.Find(SessionBusinessDay(null).BranchID);
                    }
                    if (currentDevise == null)
                    {
                        currentDevise = db.Devises.FirstOrDefault(d=>d.DefaultDevise);
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
                            DateOperation = allslices.SliceDate,
                            RefOperation = "DEP" + allslices.SliceID,
                            Description = "Reg Bill " + allslices.Sale.SaleReceiptNumber,
                            RepDebit = achatTTCBefore,
                            RepCredit = totalDepositBefore,
                            MtDebit = 0,
                            MtCredit = sliceAmount,
                            Agence = currentBranch.BranchCode,
                            LibAgence = currentBranch.BranchName,
                            SaleID = allslices.SaleID,
                            LogoBranch = _fileRepository.FindAll.FirstOrDefault(f => f.GlobalPersonID == Company.GlobalPersonID) != null ? _fileRepository.FindAll.FirstOrDefault(f => f.GlobalPersonID == Company.GlobalPersonID).Content : COMPANY_LOGO
                        });
            });
            /****** traitement des depots   * *****/

            depositlist = db.AllDeposits.Where(dep => (dep.AllDepositDate >= BeginDate.Date && dep.AllDepositDate <= EndDate.Date) && dep.CustomerID == customerID && !dep.AllDepositReference.Contains("REMOVENULL")).ToList();
            depositlist.ForEach(alldeposits =>
            {
                double saleAmnt = alldeposits.Amount;
                if (currentBranch==null)
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
                        DateOperation = alldeposits.AllDepositDate,
                        RefOperation = alldeposits.AllDepositReference,
                        Description = alldeposits.AllDepositReason == null ? "DEP by " + customer.Name : "DEPOSIT FOR "+ alldeposits.AllDepositReason,
                        RepDebit = achatTTCBefore,
                        RepCredit = totalDepositBefore,
                        MtDebit = 0,
                        MtCredit = saleAmnt,
                        Agence = currentBranch.BranchCode,
                        LibAgence = currentBranch.BranchName,
                        SaleID = alldeposits.AllDepositID,
                        LogoBranch = _fileRepository.FindAll.FirstOrDefault(f => f.GlobalPersonID == Company.GlobalPersonID) != null ? _fileRepository.FindAll.FirstOrDefault(f => f.GlobalPersonID == Company.GlobalPersonID).Content : COMPANY_LOGO
                    }
                    );
                //}
                
                    }
                );

            /****** traitement des retours   * *****/
            //recuperation des ttes les ventes de la periode
            List<CustomerReturn> RetSale = db.CustomerReturns.Where(c => c.Sale.CustomerID == customerID && (c.Sale.SaleDate >= BeginDate.Date && c.Sale.SaleDate <= EndDate.Date)).ToList();
            RetSale.ForEach(allRestSales =>
            {
                currentBranch = allRestSales.Sale.Branch;
                currentDevise = allRestSales.Sale.Devise;
                //double RetsaleAmnt = allRestSales.Sale.SaleLines.Select(l => l.LineAmount).Sum();
                double RetsaleAmnt = allRestSales.CustomerReturnLines.Select(l=>l.SaleLine.LineAmount).Sum();
                ExtraPrice extra = Util.ExtraPrices(RetsaleAmnt, allRestSales.RateReduction, allRestSales.RateDiscount, allRestSales.Transport, allRestSales.VatRate);

                //ajout des ventes ds la table des etats histo client
                model.Add(
                      new RptPrintStmt
                      {
                          AcctNo = customer.CNI,
                          AcctName = customer.Name,
                          Devise = currentDevise.DeviseCode,
                          LibDevise = currentDevise.DeviseLabel,
                          DateOperation = allRestSales.CustomerReturnLines.Select(l=>l.CustomerReturnDate).FirstOrDefault().Date,
                          RefOperation = allRestSales.Sale.SaleReceiptNumber,
                          Description = "Return For " + customer.Name ,
                          RepDebit = achatTTCBefore,
                          RepCredit = totalDepositBefore,
                          MtDebit = 0,
                          MtCredit = extra.TotalTTC,
                          Agence = currentBranch.BranchCode,
                          LibAgence = currentBranch.BranchName,
                          SaleID =allRestSales.SaleID,
                          LogoBranch = _fileRepository.FindAll.FirstOrDefault(f => f.GlobalPersonID == Company.GlobalPersonID) != null ? _fileRepository.FindAll.FirstOrDefault(f => f.GlobalPersonID == Company.GlobalPersonID).Content : COMPANY_LOGO
                      }
         );
            }
        );
                          /*
            var query = db.CustomerReturns.Join(db.CustomerReturnLines, cr => cr.CustomerReturnID, crln => crln.CustomerReturnID,
                (cr, crln) => new { cr, crln }).
                Join(db.SaleLines, crsl => crsl.crln.SaleLineID, sal => sal.LineID, (crsl, sal) => new { crsl, sal })
                .Where(crsaline => crsaline.crsl.crln.CustomerReturnDate >= BeginDate.Date && crsaline.crsl.crln.CustomerReturnDate <= EndDate.Date
                 && crsaline.crsl.cr.Sale.CustomerID == customerID)
                .Select(s => new
                {
                    CustomerReturnDate = s.crsl.crln.CustomerReturnDate,
                    CustomerReturnID = s.crsl.cr.CustomerReturnID,
                    LineQuantity = s.crsl.crln.LineQuantity,
                    LineAmount = s.sal.LineUnitPrice,
                    Sale = s.crsl.cr.Sale,
                    CustomerID = s.crsl.cr.Sale.CustomerID,
                    RateReduction = s.crsl.cr.Sale.RateReduction,
                    RateDiscount = s.crsl.cr.Sale.RateDiscount,
                    Transport = s.crsl.cr.Sale.Transport,
                    VatRate = s.crsl.cr.Sale.VatRate,
                    SaleReceiptNumber = s.crsl.cr.Sale.SaleReceiptNumber
                })
                .AsQueryable()
                .ToList();

            foreach (var c in query.OrderBy(cr => cr.CustomerReturnDate))
            {
                double returnAmnt = c.LineQuantity * c.LineAmount;
                ExtraPrice extra = Util.ExtraPrices(returnAmnt, c.RateReduction, c.RateDiscount, c.Transport, c.VatRate);

                if (currentBranch == null)
                {
                    currentBranch = db.Branches.Find(SessionBusinessDay(null).BranchID);
                }
                if (currentDevise == null)
                {
                    currentDevise = db.Devises.FirstOrDefault(d => d.DefaultDevise);
                }
                model.Add
                (
                    new RptPrintStmt
                    {
                        AcctNo = customer.CNI,
                        AcctName = customer.Name,
                        Devise = currentDevise.DeviseCode,
                        LibDevise = currentDevise.DeviseLabel,
                        DateOperation = c.CustomerReturnDate,
                        RefOperation = c.SaleReceiptNumber,
                        Description = "Return For " + c.Sale.Customer.Name,
                        RepDebit = achatTTCBefore,
                        RepCredit = totalDepositBefore,
                        MtDebit = 0,
                        MtCredit = extra.TotalTTC,
                        Agence = currentBranch.BranchCode,
                        LibAgence = currentBranch.BranchName,
                        LogoBranch = _fileRepository.FindAll.FirstOrDefault(f => f.GlobalPersonID == Company.GlobalPersonID) != null ? _fileRepository.FindAll.FirstOrDefault(f => f.GlobalPersonID == Company.GlobalPersonID).Content : COMPANY_LOGO
                    }
                    );
            };
                                                    */

            /****** traitement des depot d'epargne provenant des retours   * *****/
            //recuperation des ttes les ventes de la periode
            //recuperation du saving acct du customer
            SavingAccount savAcct = db.SavingAccounts.Where(s => s.CustomerID == customerID).SingleOrDefault();
            if (savAcct!=null)
            {
                List<Deposit> Retdeposit = db.Deposits.Where(c => c.SavingAccountID == savAcct.ID && (c.DepositDate >= BeginDate.Date && c.DepositDate <= EndDate.Date) && c.DepositReference.StartsWith("SADE")).ToList();
                Retdeposit.ForEach(allRestDep =>
                {
                    currentBranch = allRestDep.PaymentMethod1.Branch;
                    currentDevise = allRestDep.Devise;

                    double RetDepAmnt = allRestDep.Amount;

                    //ajout des ventes ds la table des etats histo client
                    model.Add(
                          new RptPrintStmt
                          {
                              AcctNo = customer.CNI,
                              AcctName = customer.Name,
                              Devise = currentDevise.DeviseCode,
                              LibDevise = currentDevise.DeviseLabel,
                              DateOperation = allRestDep.DepositDate.Date,
                              RefOperation = allRestDep.DepositReference,
                              Description = "Return For " + customer.Name,
                              RepDebit = achatTTCBefore,
                              RepCredit = totalDepositBefore,
                              MtDebit = RetDepAmnt,
                              MtCredit = 0,
                              Agence = currentBranch.BranchCode,
                              LibAgence = currentBranch.BranchName,
                              SaleID = allRestDep.DepositID,
                              LogoBranch = _fileRepository.FindAll.FirstOrDefault(f => f.GlobalPersonID == Company.GlobalPersonID) != null ? _fileRepository.FindAll.FirstOrDefault(f => f.GlobalPersonID == Company.GlobalPersonID).Content : COMPANY_LOGO
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
                          Devise = currentDevise.DeviseCode,
                          LibDevise = currentDevise.DeviseLabel,
                          RepDebit = achatTTCBefore,
                          RepCredit = totalDepositBefore,
                          DateOperation = BeginDate,
                          MtDebit = 0,
                          MtCredit = 0,
                          Sens = "",
                          Agence = currentBranch.BranchCode,
                          LibAgence = currentBranch.BranchName,
                          SaleID = 0,
                          LogoBranch = _fileRepository.FindAll.FirstOrDefault(f => f.GlobalPersonID == Company.GlobalPersonID) != null ? _fileRepository.FindAll.FirstOrDefault(f => f.GlobalPersonID == Company.GlobalPersonID).Content : COMPANY_LOGO
                      }
                 );

            }
            return model;
        }
        public void GenerateReport()
        {
            bool isValid = false;
            ReportDocument rptH = new ReportDocument();
            try
            {
                
                DateTime BeginDate = (DateTime)Session["BeginDate"];
                DateTime EndDate = (DateTime)Session["EndDate"];
                double Solde = 0;
                List<object> model = new List<object>();

                double CumulCredit = ModelHistoCusto(BeginDate, EndDate).Select(m => m.RepCredit).Max();
                double CumulDebit = ModelHistoCusto(BeginDate, EndDate).Select(m => m.RepDebit).Max();

                List<RptPrintStmt> listCustOp = ModelHistoCusto(BeginDate, EndDate).OrderBy(m => m.DateOperation).ThenBy(m => m.SaleID).ToList();
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
                                     Sens = (Solde == 0) ? "" : (Solde>0) ? "Cr" : "Db",
                                     SaleID = c.SaleID.Value,
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
            finally
            {
                rptH.Close();
                rptH.Dispose();
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