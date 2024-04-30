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
    public class RptPrintStmtController : BaseController
    {
        //Current Controller and current page
        private const string CONTROLLER_NAME = "Accounting/RptPrintStmt";
        private const string VIEW_NAME = "Index";
        //person repository

        
        private Company cmpny;
        

        //
        // GET: /Accounting/RptPrintStmt/
        
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
            //if (!LoadAction.isAutoriseToGetSubMenu(SessionProfileID, CodeValue.Accounting.Report.CODESUBMENU2, db))
            //{
            //    RedirectToAction("NotAuthorized", "User");
            //}

            this.chargeSolde(acctop);
            return View(ModelAcctOp(acctop));
        }
        private void chargeSolde(AccountOperation acctop)
        {
            double Totaldebit = 0d;
            double TotalCredit = 0d;
            //DateTime Bdate = new DateTime(1900,1,1);
            //DateTime Edate = new DateTime(1900, 1, 1);
            int AcctNo = acctop.AccountID;

            //DateTime Bdate = (DateTime)Session["BeginDate"];
            //DateTime Edate = (DateTime)Session["EndDate"];
            //int AcctNo = (int)Session["AcctNo"];

            List<AccountOperation> listAccOp = db.AccountOperations.Where(ao => ao.BranchID == acctop.BranchID && ao.DeviseID == acctop.DeviseID  && ao.AccountID == AcctNo).ToList();
            if (listAccOp != null)
            {
                Totaldebit = listAccOp.Select(s => s.Debit).Sum();
                TotalCredit = listAccOp.Select(s => s.Credit).Sum();
            }

            this.GetCmp<TextField>("TotalDebit").Value = Totaldebit;
            //ViewBag.TotalDebit = Totaldebit;
            this.GetCmp<TextField>("TotalCredit").Value = TotalCredit;
            //ViewBag.TotalCredit = TotalCredit;
            this.GetCmp<TextField>("Solde").Value = TotalCredit - Totaldebit;
            //ViewBag.Solde = TotalCredit-Totaldebit;
        }
        public ActionResult DisplayEntries(AccountOperation acctop, DateTime BeginDate, DateTime EndDate)
        {
            Session["accop"] = acctop;
            Session["BeginDate"] = BeginDate;
            Session["EndDate"] = EndDate;
            Session["AcctNo"] = acctop.AccountID;
            this.PartialReset();
            this.chargeSolde(acctop);
            return this.Direct();
        }
        public void Reset()
        {
            this.GetCmp<FormPanel>("RptPrintStmt").Reset(true);
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
            //recuperation de ttes les operation du type op choisi
            
            DateTime Bdate = (DateTime)Session["BeginDate"];
            DateTime Edate = (DateTime)Session["EndDate"];
            int AcctNo = acctop.AccountID;

            List<AccountOperation> listAccOp = db.AccountOperations.Where(ao => ao.BranchID == acctop.BranchID && ao.DeviseID == acctop.DeviseID && (ao.DateOperation >= Bdate && ao.DateOperation <= Edate) && ao.AccountID == AcctNo
    ).ToList();

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
            //}

            return list;

        }
        private List<object> ModelRepAcctOp(AccountOperation acctop)
        {
            double RepDebit = 0d;
            double RepCredit = 0d;
            double Solde = 0d;
            double CumulCredit=0d;
            double CumulDebit = 0d;

            Branch br = db.Branches.Find(acctop.BranchID);
            List<object> list = new List<object>();
            DateTime Bdate = (DateTime)Session["BeginDate"];
            DateTime Edate = (DateTime)Session["EndDate"];
            int AcctNo = acctop.AccountID;
            
            string defaultImgPath = Server.MapPath("~/Content/Images/App/default-img.png");
            byte[] COMPANY_LOGO = System.IO.File.ReadAllBytes(defaultImgPath);

            cmpny = db.Companies.FirstOrDefault();
            List<AccountOperation> listRepAccOp = db.AccountOperations.Where(ao => ao.BranchID == acctop.BranchID && ao.DeviseID == acctop.DeviseID && (ao.DateOperation < Bdate) && ao.AccountID == AcctNo).ToList();
            if (listRepAccOp != null)
            {
                RepDebit = listRepAccOp.Select(s => s.Debit).Sum();
                RepCredit = listRepAccOp.Select(s => s.Credit).Sum();
            }
            CumulCredit = RepCredit;
            CumulDebit = RepDebit;
            List<AccountOperation> listAccOp = db.AccountOperations.Where(ao => ao.BranchID == acctop.BranchID && ao.DeviseID == acctop.DeviseID && (ao.DateOperation >= Bdate && ao.DateOperation <= Edate) && ao.AccountID == AcctNo ).ToList();
            foreach (AccountOperation c in listAccOp)
            {
                Solde = 0d;
                CumulCredit+=c.Credit;
                CumulDebit += c.Debit;
                Solde = CumulCredit - CumulDebit;

                list.Add(
                             new
                             {
                                 RptEtatsJournalID = c.AccountOperationID,
                                 Agence = c.Branch.BranchCode,// br.BranchCode,
                                 LibAgence = c.Branch.BranchDescription,// br.BranchDescription,
                                 Devise = c.Devise.DeviseCode,
                                 LibDevise = c.Devise.DeviseDescription,
                                 AcctNo = c.Account.AccountNumber.ToString(),
                                 AcctName = c.Account.AccountLabel,
                                 RefOperation = c.Reference,
                                 Description = c.Description,
                                 DateOperation = c.DateOperation,
                                 MtDebit = c.Debit,
                                 MtCredit = c.Credit,
                                 RepDebit = RepDebit,
                                 RepCredit = RepCredit,
                                 Solde = Solde,
                                 Sens = Solde > 0 ? "Cr" : "Db",
                                 LogoBranch = db.Files.SingleOrDefault(f => f.GlobalPersonID == cmpny.GlobalPersonID) != null ? db.Files.SingleOrDefault(f => f.GlobalPersonID == cmpny.GlobalPersonID).Content : COMPANY_LOGO
                             }
                         );
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
                DateTime BeginDate =(DateTime)Session["BeginDate"];
                DateTime EndDate = (DateTime)Session["EndDate"];

                var rptSource = Session["rptSource"];

                if (string.IsNullOrEmpty(strReportName) && rptSource==null)
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
            AccountOperation acctop = (AccountOperation)Session["accop"];
            this.Session["ReportName"] = "Stmt";
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
            this.Session["RepTitle"] = Resources.rptCustAcctHistTitle;
            this.Session["Operator"] = CurrentUser.Name;
            if (acctop != null)
            {
                this.Session["rptSource"] = ModelRepAcctOp(acctop);
            }
            else
            {
                this.Session["rptSource"] = null;
            }

            return RedirectToAction("ShowGenericRpt", "RptPrintStmt");
        }
    }
}