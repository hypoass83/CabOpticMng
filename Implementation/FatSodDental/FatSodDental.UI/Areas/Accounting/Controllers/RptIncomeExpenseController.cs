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

namespace FatSodDental.UI.Areas.Accounting.Controllers
{
    public class RptIncomeExpenseController : BaseController
    {
        //Current Controller and current page
        private const string CONTROLLER_NAME = "Accounting/RptIncomeExpense";
        private const string VIEW_NAME = "Index";
        //person repository
       
        //
        // GET: /Accounting/RptIncomeExpense/
        
        [OutputCache(Duration = 3600)] 
        public ActionResult Index()
        {
            AccountOperation acctop = new AccountOperation();
            Session["BeginDate"] = new DateTime(1900, 1, 1);
            Session["EndDate"] = new DateTime(1900, 1, 1);

            //Ext.Net.MVC.PartialViewResult rPVResult = new Ext.Net.MVC.PartialViewResult
            //{
            //    ClearContainer = true,
            //    //RenderMode = RenderMode.AddTo,
            //    ContainerId = "Body",
            //    //WrapByScriptTag = false,
            //    Model = ModelAcctOp(acctop)
            //};
            //Session["Curent_Page"] = VIEW_NAME;
            //Session["Curent_Controller"] = CONTROLLER_NAME;

            //We test if this user has an autorization to get this sub menu
            //if (!LoadAction.isAutoriseToGetSubMenu(SessionProfileID, CodeValue.Accounting.Report.CODESUBMENU4, db))
            //{
            //    RedirectToAction("NotAuthorized", "User");
            //}

            return View(ModelAcctOp(acctop));
        }

        public ActionResult DisplayEntries(AccountOperation acctop, DateTime BeginDate, DateTime EndDate)
        {
            Session["accop"] = acctop;
            Session["BeginDate"] = BeginDate;
            Session["EndDate"] = EndDate;
            this.PartialReset();
            return this.Direct();
        }
        public void Reset()
        {
            this.GetCmp<FormPanel>("RptIncomeExpense").Reset(true);
            this.PartialReset();
        }
        public void PartialReset()
        {
            this.GetCmp<Store>("Store").Reload();
        }
        [HttpPost]
        public StoreResult GetList()
        {
            return this.Store(ModelAcctOp((AccountOperation)Session["accop"]));
        }

        private List<object> ModelAcctOp(AccountOperation acctop) //, string BeginDate, string EndDate)
        {
            List<object> list = new List<object>();

            DateTime Bdate = (DateTime)Session["BeginDate"];
            DateTime Edate = (DateTime)Session["EndDate"];
            String AccountType="";
            double MonthTotal = 0d;
            double MonthCumul = 0d;

            if (acctop==null || (Bdate==new DateTime(1900,1,1) && Edate==new DateTime(1900,1,1)) )
            {
                return list;
            }
            Branch br = db.Branches.FirstOrDefault(b => b.BranchID == acctop.BranchID);
            Devise dev = db.Devises.FirstOrDefault(d => d.DeviseID == acctop.DeviseID);

            double earningsmonth = (db.AccountOperations.Where(ao => ao.BranchID == acctop.BranchID && ao.DeviseID == acctop.DeviseID
                && (ao.Account.CollectifAccount.AccountingSection.ClassAccount.ClassAccountNumber == 7) && (ao.DateOperation >= Bdate && ao.DateOperation <= Edate))
                .Select(s => s.Credit - s.Debit).Sum() -
                db.AccountOperations.Where(ao => ao.BranchID == acctop.BranchID && ao.DeviseID == acctop.DeviseID
                && (ao.Account.CollectifAccount.AccountingSection.ClassAccount.ClassAccountNumber == 6) && (ao.DateOperation >= Bdate && ao.DateOperation <= Edate))
                .Select(s => s.Debit - s.Credit).Sum());
            double earningscumul = (db.AccountOperations.Where(ao => ao.BranchID == acctop.BranchID && ao.DeviseID == acctop.DeviseID
                && (ao.Account.CollectifAccount.AccountingSection.ClassAccount.ClassAccountNumber == 7) && (ao.DateOperation <= Edate))
                .Select(s => s.Credit - s.Debit).Sum() -
                db.AccountOperations.Where(ao => ao.BranchID == acctop.BranchID && ao.DeviseID == acctop.DeviseID
                && (ao.Account.CollectifAccount.AccountingSection.ClassAccount.ClassAccountNumber == 6) && (ao.DateOperation <= Edate))
                .Select(s => s.Debit - s.Credit).Sum());


            //retourne la liste distinct des cpte ds le grand livre des classe 6 et 7
            var listDistAcc = db.AccountOperations.Where(ao => ao.BranchID == acctop.BranchID && ao.DeviseID == acctop.DeviseID
                && (ao.Account.CollectifAccount.AccountingSection.ClassAccount.ClassAccountNumber == 6 || ao.Account.CollectifAccount.AccountingSection.ClassAccount.ClassAccountNumber == 7))
                .Select(a => new 
                { 
                    Account = a.Account
                }).Distinct().ToList();


            listDistAcc.ForEach(c =>
            {
                AccountType = "";
                MonthTotal = 0d;
                MonthCumul = 0d;
                List<AccountOperation> listAccMvt = db.AccountOperations.Where(ao => ao.BranchID == acctop.BranchID && ao.DeviseID == acctop.DeviseID && (ao.DateOperation >= Bdate && ao.DateOperation <= Edate) && ao.AccountID == c.Account.AccountID).ToList();
                List<AccountOperation> listAccfin = db.AccountOperations.Where(ao => ao.BranchID == acctop.BranchID && ao.DeviseID == acctop.DeviseID && (ao.DateOperation <= Edate) && ao.AccountID == c.Account.AccountID).ToList();
                if (c.Account.CollectifAccount.AccountingSection.ClassAccount.ClassAccountNumber == 6)
                { 
                    AccountType = Resources.Expense;
                    MonthTotal = listAccMvt.Select(s => s.Debit - s.Credit).Sum();
                    MonthCumul = listAccfin.Select(s => s.Debit - s.Credit).Sum();
                }
                else if (c.Account.CollectifAccount.AccountingSection.ClassAccount.ClassAccountNumber == 7)
                {
                    AccountType = Resources.Income;
                    MonthTotal = listAccMvt.Select(s => s.Credit - s.Debit).Sum();
                    MonthCumul = listAccfin.Select(s => s.Credit - s.Debit).Sum();
                }
                list.Add(
                            new
                            {
                                RptIncomeExpenseID = 1,
                                Agence = br.BranchCode,
                                LibAgence = br.BranchName,
                                Devise = dev.DeviseCode,
                                LibDevise = dev.DeviseLabel,
                                AcctNumber = c.Account.AccountNumber.ToString(),
                                AcctName = c.Account.AccountLabel,
                                AccountType=AccountType,
                                MonthTotal = MonthTotal,
                                MonthCumul = MonthCumul,
                                earningsmonth = earningsmonth,
                                earningscumul = earningscumul
                            }
                );
            });

            return list;

        }

        public void ShowGenericRpt()
        {
            ReportDocument rptH = new ReportDocument();
            try
            {
                bool isValid = true;

                string strReportName = Session["ReportName"].ToString();    // Setting ReportName
                string stBranchName1 = Session["BranchName"].ToString();     // Setting BranchName1
                string strBranchInfo1 = Session["BranchInfo"].ToString();         // Setting BranchInfo1
                string strRepTitle1 = Session["RepTitle"].ToString();         // Setting RepTitle1
                string strOperator1 = Session["Operator"].ToString();         // Setting Operator1

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
                    if (!string.IsNullOrEmpty(stBranchName1)) rptH.SetParameterValue("BranchName", stBranchName1);
                    if (!string.IsNullOrEmpty(strBranchInfo1)) rptH.SetParameterValue("BranchInfo", strBranchInfo1);
                    if (!string.IsNullOrEmpty(strRepTitle1)) rptH.SetParameterValue("RepTitle", strRepTitle1);
                    if (!string.IsNullOrEmpty(strOperator1)) rptH.SetParameterValue("Operator", strOperator1);

                    if (BeginDate != null) rptH.SetParameterValue("BeginDate", BeginDate);
                    if (EndDate != null) rptH.SetParameterValue("EndDate", EndDate);
                    bool isDownloadRpt = (bool)Session["isDownloadRpt"];
                    rptH.ExportToHttpResponse(ExportFormatType.PortableDocFormat, System.Web.HttpContext.Current.Response, false, strReportName);

                    // Clear all sessions value
                    Session["ReportName"] = null;
                    Session["BranchName"] = null;
                    Session["BranchInfo"] = null;
                    Session["RepTitle"] = null;
                    Session["Operator"] = null;
                    Session["accop"] = null;
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
            Session["BranchName"] = null;
            Session["BranchInfo"] = null;
            Session["RepTitle"] = null;
            Session["Operator"] = null;

            Company cmpny = db.Companies.FirstOrDefault();
            AccountOperation acctop = (AccountOperation)Session["accop"];
            this.Session["ReportName"] = "RptIncomeExpense";
            if (cmpny != null)
            {
                this.Session["BranchName"] = cmpny.Name;
                this.Session["BranchInfo"] = "Tel: " + cmpny.Adress.AdressPhoneNumber + " Fax : " + cmpny.Adress.AdressFax + " Town :" + cmpny.Adress.Quarter.Town.TownLabel;
            }
            else
            {
                this.Session["BranchName"] = "NONE";
                this.Session["BranchInfo"] = "NONE";
            }
            this.Session["RepTitle"] = Resources.submnuAcct7rpt4;
            this.Session["Operator"] = CurrentUser.Name;
            if (acctop != null)
            {
                this.Session["rptSource"] = ModelAcctOp(acctop);
            }
            else
            {
                this.Session["rptSource"] = null;
            }
            return RedirectToAction("ShowGenericRpt", "RptIncomeExpense");
        }
    }
}