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

namespace FatSodDental.UI.Areas.Accounting.Controllers
{
    public class RptBalanceGeneraleController : BaseController
    {
        //Current Controller and current page
        private const string CONTROLLER_NAME = "Accounting/RptBalanceGenerale";
        private const string VIEW_NAME = "Index";
       
        private Company cmpny;
        
        //
        // GET: /Accounting/RptBalanceGenerale/
        [OutputCache(Duration = 3600)]
        public ActionResult Index()
        {
            //Ext.Net.MVC.PartialViewResult rPVResult = new Ext.Net.MVC.PartialViewResult
            //{
            //    ClearContainer = true,
            //    //RenderMode = RenderMode.AddTo,
            //    ContainerId = "Body",
            //    //WrapByScriptTag = false,
            //    Model = ModelAcctOp(0, 0)
            //};
            //Session["Curent_Page"] = VIEW_NAME;
            //Session["Curent_Controller"] = CONTROLLER_NAME;

            //We test if this user has an autorization to get this sub menu
            //if (!LoadAction.isAutoriseToGetSubMenu(SessionProfileID, CodeValue.Accounting.Report.CODESUBMENU3, db))
            //{
            //    RedirectToAction("NotAuthorized", "User");
            //}

            Session["BeginDate"] = new DateTime(1900, 1, 1);
            Session["EndDate"] = new DateTime(1900, 1, 1);

            return View(ModelAcctOp(0, 0));
        }

        public ActionResult DisplayEntries(/*AccountOperation acctop,*/ DateTime BeginDate, DateTime EndDate)
        {
            //Session["accop"] = acctop;
            Session["BeginDate"] = BeginDate;
            Session["EndDate"] = EndDate;
            this.PartialReset();
            return this.Direct();
        }
        public void Reset()
        {
            this.GetCmp<FormPanel>("RptBalanceGenerale").Reset(true);
            this.PartialReset();
        }
        public void PartialReset()
        {
            this.GetCmp<Store>("Store").Reload();
        }
        [HttpPost]
        public StoreResult GetList(int? BranchID, int? DeviseID)
        {
            return this.Store(ModelAcctOp(BranchID, DeviseID));
        }

        private List<object> ModelAcctOp(int? BranchID, int? DeviseID) //, string BeginDate, string EndDate)
        {
            List<object> list = new List<object>();
            try
            {
                DateTime Bdate = (DateTime)Session["BeginDate"];
                DateTime Edate = (DateTime)Session["EndDate"];
                if ((Bdate == new DateTime(1900, 1, 1) && Edate == new DateTime(1900, 1, 1)) || BranchID == 0 || DeviseID == 0)
                {
                    this.Session["rptSource"] = null;
                    return list;
                }
                
                Branch br = (from b in db.Branches where b.BranchID == BranchID select b).SingleOrDefault();
                Devise dev = (from d in db.Devises where d.DeviseID == DeviseID select d).SingleOrDefault();
                
                var listDistAcc = (from ao in db.AccountOperations
                                   where ao.BranchID == BranchID && ao.DeviseID == DeviseID
                                   group ao by ao.AccountID into groupingao
                                   select new 
                                   { 
                                       groupingao.Key,
                                       NumberAccount=groupingao.Count()
                                   }).ToList();
                foreach (var c in listDistAcc)
                //listDistAcc.ForEach(c =>
                {
                    
                    List<AccountOperation> listAccDeb = (from ao in db.AccountOperations
                                                         where ao.BranchID == BranchID && ao.DeviseID == DeviseID && (ao.DateOperation < Bdate) && ao.AccountID == c.Key// .AccountID
                                                         select ao).ToList();
                    List<AccountOperation> listAccMvt = (from ao in db.AccountOperations
                                                         where ao.BranchID == BranchID && ao.DeviseID == DeviseID && (ao.DateOperation >= Bdate && ao.DateOperation <= Edate) && ao.AccountID == c.Key//c.AccountID
                                                         select ao).ToList();
                    List<AccountOperation> listAccfin = (from ao in db.AccountOperations
                                                         where ao.BranchID == BranchID && ao.DeviseID == DeviseID && (ao.DateOperation <= Edate) && ao.AccountID == c.Key//c.AccountID
                                                         select ao).ToList();
                    
                    Account acc = (from ac in db.Accounts
                                   where ac.AccountID == c.Key
                                   select ac).SingleOrDefault();
                        list.Add(
                                new
                                {
                                    RptBalanceGeneraleID = 1,
                                    Agence = br.BranchID,// listAccfin.Select(s => s.BranchID),
                                    LibAgence =br.BranchName,// listAccfin.Select(s => s.UIBranchCode),
                                    Devise = dev.DeviseID,// listAccfin.Select(s => s.DeviseID),
                                    LibDevise =dev.DeviseLabel,// listAccfin.Select(s => s.UIDeviseCode),
                                    Compte = acc.AccountNumber.ToString(),//listAccfin.Select (s=>s.UIAccountNumber),
                                    Libelle = acc.AccountLabel,// listAccfin.Select(s => s.Account.AccountLabel),
                                    SoldeInitDb = listAccDeb.Select(s => s.Debit - s.Credit).Sum() > 0 ? listAccDeb.Select(s => s.Debit - s.Credit).Sum() : 0,
                                    SoldeInitCr = listAccDeb.Select(s => s.Credit - s.Debit).Sum() > 0 ? listAccDeb.Select(s => s.Credit - s.Debit).Sum() : 0,
                                    DebitMvt = listAccMvt.Select(s => s.Debit).Sum(),// > 0 ? listAccMvt.Select(s => s.Debit - s.Credit).Sum() : 0,
                                    CreditMvt = listAccMvt.Select(s => s.Credit).Sum(),// > 0 ? listAccMvt.Select(s => s.Credit - s.Debit).Sum() : 0,
                                    SoldeFinDb = listAccfin.Select(s => s.Debit - s.Credit).Sum() > 0 ? listAccfin.Select(s => s.Debit - s.Credit).Sum() : 0,
                                    SoldeFinCr = listAccfin.Select(s => s.Credit - s.Debit).Sum() > 0 ? listAccfin.Select(s => s.Credit - s.Debit).Sum() : 0,
                                }
                    );
                    //}
                    
                }

            }
            catch (Exception e)
            {
                X.Msg.Alert("Error", "An error occure when we try to give response : " + e.Message).Show();
            }
            this.Session["rptSource"] = list;
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

            Session["Adresse"] = null;

            Session["RepTitle"] = null;
            Session["Operator"] = null;

            cmpny = db.Companies.FirstOrDefault();
            //AccountOperation acctop = (AccountOperation)Session["accop"];
            this.Session["ReportName"] = "RptBalanceGenerale";
            if (cmpny != null)
            {
                this.Session["CompanyName"] = cmpny.Name;
                this.Session["TelFax"] = "Tel: " + cmpny.Adress.AdressPhoneNumber + " Fax : " + cmpny.Adress.AdressFax + " Town :" + cmpny.Adress.Quarter.Town.TownLabel;
                this.Session["Adresse"] = "PO BOX:" + cmpny.Adress.AdressPOBox + " " + cmpny.Adress.Quarter.Town.TownLabel;
            }
            else
            {
                this.Session["CompanyName"] = "NONE";
                this.Session["TelFax"] = "NONE";
            }
            this.Session["RepTitle"] = Resources.rptBalanceGenerale;
            this.Session["Operator"] = CurrentUser.Name;
            //if (acctop != null)
            //{
            //this.Session["rptSource"] = ModelAcctOp(acctop.BranchID, acctop.DeviseID);
            //}
            //else
            //{
            //    this.Session["rptSource"] = null;
            //}

            return RedirectToAction("ShowGenericRpt", "RptBalanceGenerale");
        }
    }
}