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
using FatSodDental.UI.Filters;
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
    [Authorize(Order = 1)]
    [TakeBusinessDay(Order = 2)]
    public class rptAccoutingEntriesController : BaseController
    {
        //Current Controller and current page
        private const string CONTROLLER_NAME = "Accounting/rptAccoutingEntries";
        private const string VIEW_NAME = "Index";
        //person repository

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

            //We test if this user has an autorization to get this sub menu
            //if (!LoadAction.isAutoriseToGetSubMenu(SessionProfileID, CodeValue.Accounting.Report.CODESUBMENU1, db))
            //{
            //    RedirectToAction("NotAuthorized", "User");
            //}
            this.chargeSolde(acctop);
            return View(ModelAcctOp(acctop));
        }
        private void chargeSolde(AccountOperation acctop)
        {
            double Totaldebit = 0;
            double TotalCredit = 0;

            DateTime Bdate = (DateTime)Session["BeginDate"];
            DateTime Edate = (DateTime)Session["EndDate"];

            //recuperation des operation corespondant o type choisi
            foreach (Operation op in db.Operations.Where(o => o.OperationTypeID == acctop.OperationID).ToList())
            {
                List<AccountOperation> listAccOp = db.AccountOperations.Where(ao => ao.BranchID == acctop.BranchID && ao.DeviseID == acctop.DeviseID
                    && ao.OperationID == op.OperationID && (ao.DateOperation >= Bdate && ao.DateOperation <= Edate)).ToList();
                if (listAccOp != null)
                {
                    Totaldebit += listAccOp.Select(s => s.Debit).Sum();
                    TotalCredit += listAccOp.Select(s => s.Credit).Sum();
            }

               }
            this.GetCmp<TextField>("TotalDebit").Value = Totaldebit;
            //ViewBag.TotalDebit = Totaldebit;
            this.GetCmp<TextField>("TotalCredit").Value = TotalCredit;
            //ViewBag.TotalCredit = TotalCredit;
            this.GetCmp<TextField>("Solde").Value = TotalCredit - Totaldebit; 
            //ViewBag.Solde = TotalCredit - Totaldebit;
        }
        public ActionResult DisplayEntries(AccountOperation acctop, DateTime BeginDate, DateTime EndDate)
        {
            Session["accop"] = acctop;
            Session["BeginDate"] = BeginDate;
            Session["EndDate"] = EndDate;
            this.PartialReset();
            this.chargeSolde(acctop);
            return this.Direct();
        }
        public void Reset()
        {
            this.GetCmp<FormPanel>("rptAccoutingEntries").Reset(true);
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

        private List<object> ModelAcctOp(AccountOperation acctop)
        {
            List<object> list = new List<object>();
            DateTime Bdate = (DateTime)Session["BeginDate"];
            DateTime Edate = (DateTime)Session["EndDate"];
            //recuperation de ttes les operation du type op choisi
            foreach (
                Operation op in 
                db.Operations.Where(o => o.OperationTypeID == acctop.OperationID).ToList())
            {
                List<AccountOperation> listAccOp = db.AccountOperations.Where(ao => ao.BranchID == acctop.BranchID && ao.DeviseID == acctop.DeviseID && ao.OperationID == op.OperationID && (ao.DateOperation >= Bdate && ao.DateOperation <= Edate)).ToList();
                
                listAccOp.ForEach(c =>
                {
                    list.Add(
                                                    new
                                                    {
                                                        AccountOperationID = c.AccountOperationID,
                                                        BranchID = c.BranchID,
                                                        OperationID = c.OperationID,
                                                        AccountID = c.AccountID,
                                                        DeviseID = c.DeviseID,
                                                        UIBranchCode = c.UIBranchCode,
                                                        UIDeviseCode = c.UIDeviseCode,
                                                        UIOperationCode = c.UIOperationCode,
                                                        UIAccountNumber = c.UIAccountNumber,
                                                        DateOperation = c.DateOperation,
                                                        Description = c.Description,
                                                        Reference = c.Reference,
                                                        CodeTransaction = c.CodeTransaction,
                                                        Debit = c.Debit,
                                                        Credit = c.Credit
                                                    }
                                    );
                });
            }

            return list;

        }
        private List<object> ModelRepAcctOp(AccountOperation acctop)
        {
            DateTime Bdate = (DateTime)Session["BeginDate"];
            DateTime Edate = (DateTime)Session["EndDate"];

            Branch br = db.Branches.Find(acctop.BranchID);
            List<object> list = new List<object>();
            foreach (Operation op in db.Operations.Where(o => o.OperationTypeID == acctop.OperationID).ToList())
            {
                List<AccountOperation> listAccOp = db.AccountOperations.Where(ao => ao.BranchID == acctop.BranchID && ao.DeviseID == acctop.DeviseID && ao.OperationID == op.OperationID && (ao.DateOperation >= Bdate && ao.DateOperation <= Edate)).ToList();

                listAccOp.ForEach(c =>
                {
                    list.Add(
                                                    new
                                                    {
                                                        RptEtatsJournalID = c.AccountOperationID,
                                                        Agence = c.Branch.BranchCode,// br.BranchCode,
                                                        LibAgence = c.Branch.BranchName,// br.BranchDescription,
                                                        Devise = c.Devise.DeviseCode,
                                                        LibDevise = c.Devise.DeviseDescription,
                                                        CompteCle = c.Account.AccountNumber.ToString(),
                                                        LibelleCpte = c.Account.AccountLabel,
                                                        CodeOperation = op.OperationType.operationTypeCode,// c.Operation.OperationCode,
                                                        LibelleOperation = op.OperationType.operationTypeDescription,// c.Operation.OperationDescription,
                                                        Reference = c.Reference,
                                                        Desription = c.Description,
                                                        DateOperation = c.DateOperation,
                                                        MontantDB = c.Debit,
                                                        MontantCR = c.Credit
                                                    }
                                    );
                });
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
            this.Session["ReportName"] = "RptJnlTrans";
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
            this.Session["RepTitle"] = "LIST OF GENERAL LEDGER JOURNAL";
            this.Session["Operator"] = CurrentUser.Name;
            if (acctop != null)
            {
                this.Session["rptSource"] = ModelRepAcctOp(acctop);
            }
            else
            {
                this.Session["rptSource"] = null;
            }
            return RedirectToAction("ShowGenericRpt", "rptAccoutingEntries");
        }

    }
}