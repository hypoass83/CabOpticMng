using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using Ext.Net;
using Ext.Net.MVC;
using FatSod.DataContext.Concrete;
using FatSod.DataContext.Initializer;
using FatSod.Ressources;
using FatSod.Security.Abstracts;
using FatSod.Security.Entities;
using FatSod.Supply.Abstracts;
using FatSod.Supply.Entities;
using FatSodDental.UI.Controllers;
using FatSodDental.UI.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace FatSodDental.UI.Areas.Sale.Controllers
{
    public class RptCustomerCreditorsController : BaseController
    {
        //Current Controller and current page
        private const string CONTROLLER_NAME = "Sale/RptCustomerCreditors";
        private const string VIEW_NAME = "Index";
        //person repository


        private Company cmpny;


        //
        // GET: /Sale/RptCustomerCreditors/

        [OutputCache(Duration = 3600)]
        public ActionResult Index()
        {

            AccountOperation acctop = new AccountOperation();
            Session["BeginDate"] = new DateTime(1900, 1, 1);
            Session["EndDate"] = new DateTime(1900, 1, 1);
            Session["BranchID"] = 0;
            Session["DeviseID"] = 0;

            this.chargeSolde();
            return View(ModelAcctOp());
        }

        public ActionResult DisplayEntries(int BranchID, int DeviseID, DateTime BeginDate, DateTime EndDate)
        {
            Session["BranchID"] = BranchID;
            Session["DeviseID"] = DeviseID;
            Session["BeginDate"] = BeginDate;
            Session["EndDate"] = EndDate;

            this.PartialReset();
            this.chargeSolde();
            return this.Direct();
        }
        public void Reset()
        {
            this.GetCmp<FormPanel>("RptRptCustomerCreditors").Reset(true);
            this.PartialReset();
        }
        public void PartialReset()
        {
            this.GetCmp<Store>("Store").Reload();
        }
        [HttpPost]
        public StoreResult GetList()
        {
            return this.Store(ModelAcctOp());
        }

        private void chargeSolde()
        {
            double TotalCredit = 0d;
            DateTime Bdate = (DateTime)Session["BeginDate"];
            DateTime Edate = (DateTime)Session["EndDate"];
            int BranchID = (int)Session["BranchID"];
            int DeviseID = (int)Session["DeviseID"];

            if (BranchID <= 0 || DeviseID <= 0)
            {
                TotalCredit = 0d;
            }
            
            var lstAccountOperations = from ao in db.AccountOperations
                                       where (ao.BranchID == BranchID && ao.DeviseID == DeviseID && (ao.DateOperation >= Bdate && ao.DateOperation <= Edate) && ao.AccountTierID != null)
                                       group ao by new { ao.AccountTierID } into g
                                       where g.Sum(a => a.Credit - a.Debit) >= 0
                                       select new
                                       {
                                           key = g.Key,
                                           AccountTierID = g.Key.AccountTierID,
                                           Solde = g.Sum(a => a.Credit - a.Debit)
                                       };
            foreach (var creditor in lstAccountOperations.ToList())
            {
                TotalCredit = TotalCredit + creditor.Solde;
            }
            this.GetCmp<TextField>("TotalCredit").Value = TotalCredit;
        }
        private List<object> ModelAcctOp()
        {
            //int AccountID = 0;

            string UIAccountNumber = "";
            string AccountName = "";

            double TotalCredit = 0d;
            double Totaldebit = 0d;

            List<object> list = new List<object>();
            //recuperation de ttes les operation du type op choisi

            DateTime Bdate = (DateTime)Session["BeginDate"];
            DateTime Edate = (DateTime)Session["EndDate"];
            int BranchID = (int)Session["BranchID"];
            int DeviseID = (int)Session["DeviseID"];

            if (BranchID <= 0 || DeviseID <= 0)
            {
                return list;
            }
            string UIBranchCode = db.Branches.Find(BranchID).BranchCode;
            string UIDeviseCode = db.Devises.Find(DeviseID).DeviseCode;

            var balanceQuery = db.Accounts.Join(db.Customers, ac => ac.AccountID, cu => cu.AccountID, (ac, cu) => new { ac, cu })
                    .Select(s => new
                    {
                        AccountLabel = s.ac.AccountLabel,
                        AccountNumber = s.ac.AccountNumber,
                        AccountTierID = s.ac.AccountID
                    }).ToList();
            foreach (var c in balanceQuery.OrderBy(m => m.AccountNumber))
            {
                //Account acctnumber = db.Accounts.Find(c.AccountID);
                List<AccountOperation> listAccOp = db.AccountOperations.Where(ao => ao.BranchID == BranchID && ao.DeviseID == DeviseID && ao.AccountTierID == c.AccountTierID && (ao.DateOperation >= Bdate && ao.DateOperation <= Edate)).ToList();
                if (listAccOp != null)
                {
                    Totaldebit = listAccOp.Select(s => s.Debit).Sum();
                    TotalCredit = listAccOp.Select(s => s.Credit).Sum();
                }
                if (TotalCredit - Totaldebit >= 0)
                {
                    list.Add(
                            new
                            {
                                /*BranchID = BranchID,
                                DeviseID = DeviseID,*/
                                AccountID = c.AccountTierID,
                                UIBranchCode = UIBranchCode,
                                UIDeviseCode = UIDeviseCode,
                                UIAccountNumber = c.AccountNumber.ToString(),//*/ c.Account.AccountNumber.ToString(),
                                AccountName = c.AccountLabel,// */c.Account.AccountLabel,
                                Debit = Totaldebit,//c.Debit,
                                Credit = TotalCredit,//c.Credit,
                                Solde = TotalCredit - Totaldebit//c.Credit - c.Debit
                            }
                );
                }

            }

            return list;

        }
        private List<object> ModelRepAcctOp()
        {
            double Solde = 0d;
            double TotalCredit = 0d;
            double Totaldebit = 0d;

            List<object> list = new List<object>();

            DateTime Bdate = (DateTime)Session["BeginDate"];
            DateTime Edate = (DateTime)Session["EndDate"];
            int BranchID = (int)Session["BranchID"];
            int DeviseID = (int)Session["DeviseID"];

            Branch br = db.Branches.Find(BranchID);
            Devise dev = db.Devises.Find(DeviseID);
            cmpny = db.Companies.FirstOrDefault();

            string defaultImgPath = Server.MapPath("~/Content/Images/App/default-img.png");
            byte[] COMPANY_LOGO = System.IO.File.ReadAllBytes(defaultImgPath);

            var balanceQuery = db.Accounts.Join(db.Customers, ac => ac.AccountID, cu => cu.AccountID, (ac, cu) => new { ac, cu })
                   .Select(s => new
                   {
                       AccountLabel = s.ac.AccountLabel,
                       AccountNumber = s.ac.AccountNumber,
                       AccountTierID = s.ac.AccountID
                   }).ToList();

            foreach (var c in balanceQuery.OrderBy(m => m.AccountNumber))
            {
                List<AccountOperation> listAccOp = db.AccountOperations.Where(ao => ao.BranchID == BranchID && ao.DeviseID == DeviseID && ao.AccountTierID == c.AccountTierID && (ao.DateOperation >= Bdate && ao.DateOperation <= Edate)).ToList();
                if (listAccOp != null)
                {
                    Totaldebit = listAccOp.Select(s => s.Debit).Sum();
                    TotalCredit = listAccOp.Select(s => s.Credit).Sum();
                }
                Solde = TotalCredit - Totaldebit;
                if (Solde >= 0)
                {
                    list.Add(
                                 new
                                 {
                                     RptPrintStmtID = 1,
                                     Agence = br.BranchCode,
                                     LibAgence = br.BranchDescription,
                                     Devise = dev.DeviseCode,
                                     LibDevise = dev.DeviseDescription,
                                     AcctNo = c.AccountNumber.ToString(),
                                     AcctName = c.AccountLabel,
                                     RefOperation = "",
                                     Description = "",
                                     DateOperation = new DateTime(1900, 1, 1),
                                     MtDebit = Totaldebit,
                                     MtCredit = TotalCredit,
                                     RepDebit = 0,
                                     RepCredit = 0,
                                     Solde = Solde,
                                     Sens = Solde >= 0 ? "Cr" : "Db",
                                     //LogoBranch = db.Files.SingleOrDefault(f => f.GlobalPersonID == cmpny.GlobalPersonID) != null ? db.Files.SingleOrDefault(f => f.GlobalPersonID == cmpny.GlobalPersonID).Content : COMPANY_LOGO
                                 }
                             );
                }
            }

            return list;

        }


        public void ShowGenericRpt()
        {
            ReportDocument rptH = new ReportDocument();
            try
            {
                bool isValid = true;

                string strReportName = Session["ReportName"].ToString();    // Setting ReportName
                string stCompanyName1 = Session["CompanyName"].ToString();     // Setting CompanyName1
                string strTelFax1 = Session["TelFax"].ToString();         // Setting TelFax1
                string strRepTitle1 = Session["RepTitle"].ToString();         // Setting RepTitle1
                string strOperator1 = Session["Operator"].ToString();         // Setting Operator1
                string strRegionCountry1 = Session["RegionCountry"].ToString();
                string strAdresse1 = Session["Adresse"].ToString();
                DateTime BeginDate = (DateTime)Session["BeginDate"];
                DateTime EndDate = (DateTime)Session["EndDate"];

                var rptSource = Session["rptSource"];

                if (string.IsNullOrEmpty(strReportName) && rptSource == null)
                {
                    isValid = false;
                }

                if (isValid)
                {
                    
                    string strRptPath = System.Web.HttpContext.Current.Server.MapPath("~/") + "Reports//Accounting//" + strReportName + ".rpt";
                    rptH.Load(strRptPath);
                    if (rptSource != null && rptSource.GetType().ToString() != "System.String") rptH.SetDataSource(rptSource);
                    if (!string.IsNullOrEmpty(stCompanyName1)) rptH.SetParameterValue("CompanyName", stCompanyName1);
                    if (!string.IsNullOrEmpty(strTelFax1)) rptH.SetParameterValue("TelFax", strTelFax1);
                    if (!string.IsNullOrEmpty(strRepTitle1)) rptH.SetParameterValue("RepTitle", strRepTitle1);
                    if (!string.IsNullOrEmpty(strOperator1)) rptH.SetParameterValue("Operator", strOperator1);

                    if (!string.IsNullOrEmpty(strRegionCountry1)) rptH.SetParameterValue("RegionCountry", strRegionCountry1);
                    if (!string.IsNullOrEmpty(strAdresse1)) rptH.SetParameterValue("Adresse", strAdresse1);

                    if (BeginDate != null) rptH.SetParameterValue("BeginDate", BeginDate);
                    if (EndDate != null) rptH.SetParameterValue("EndDate", EndDate);
                    bool isDownloadRpt = (bool)Session["isDownloadRpt"];
                    rptH.ExportToHttpResponse(ExportFormatType.PortableDocFormat, System.Web.HttpContext.Current.Response, false, strReportName);

                    // Clear all sessions value
                    Session["ReportName"] = null;
                    Session["CompanyName"] = null;
                    Session["TelFax"] = null;
                    Session["RepTitle"] = null;
                    Session["Operator"] = null;
                    Session["accop"] = null;
                    Session["RegionCountry"] = null;
                    Session["Adresse"] = null;
                    Session["BeginDate"] = null;
                    Session["EndDate"] = null;
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

        //This method load a method that print 
        public ActionResult PrintReport()
        {
            this.GetCmp<Panel>("PanelReport").LoadContent(new ComponentLoader
            {
                Url = Url.Action("ShowGeneric"),
                DisableCaching = false,
                Mode = LoadMode.Frame,
            });

            return this.Direct();
        }

        /// This is used for showing Generic Report(with data and report parameter) in a same window       
        public ActionResult ShowGeneric()
        {
            // Clear all sessions value
            Session["ReportName"] = null;
            Session["CompanyName"] = null;
            Session["TelFax"] = null;

            Session["RegionCountry"] = null;
            Session["Adresse"] = null;

            Session["RepTitle"] = null;
            Session["Operator"] = null;

            cmpny = db.Companies.FirstOrDefault();
            //AccountOperation acctop = (AccountOperation)Session["accop"];
            this.Session["ReportName"] = "RptCustomerBalance";
            if (cmpny != null)
            {
                this.Session["CompanyName"] = cmpny.Name;
                this.Session["TelFax"] = "Tel: " + cmpny.Adress.AdressPhoneNumber + " Fax : " + cmpny.Adress.AdressFax + " Town :" + cmpny.Adress.Quarter.Town.TownLabel;
                this.Session["RegionCountry"] = cmpny.Adress.Quarter.Town.Region.RegionLabel;
                this.Session["Adresse"] = "PO BOX:" + cmpny.Adress.AdressPOBox + " " + cmpny.Adress.Quarter.Town.TownLabel;
            }
            else
            {
                this.Session["CompanyName"] = "NONE";
                this.Session["TelFax"] = "NONE";
            }
            this.Session["RepTitle"] = Resources.RptCustomerCreditorsTitle;
            this.Session["Operator"] = CurrentUser.Name;
            //if (acctop != null)
            //{
            this.Session["rptSource"] = ModelRepAcctOp();
            //}
            //else
            //{
            //    this.Session["rptSource"] = null;
            //}

            return RedirectToAction("ShowGenericRpt", "RptCustomerCreditors");
        }
    }
}