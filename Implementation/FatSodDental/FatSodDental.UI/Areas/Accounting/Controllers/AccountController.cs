﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FatSod.Supply.Entities;
using FatSod.Supply.Abstracts;
using FatSod.Security.Abstracts;
using FatSod.Security.Entities;
using Ext.Net;
using Ext.Net.MVC;
using FatSodDental.UI.Tools;
using FatSodDental.UI.Controllers;
using FatSod.DataContext.Initializer;
using FatSod.Ressources;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using FatSodDental.UI.Filters;
using FatSod.DataContext.Concrete;
using System.Web.UI;

namespace FatSodDental.UI.Areas.Accounting.Controllers
{
    [Authorize(Order = 1)]
    [TakeBusinessDay(Order = 2)]
    public class AccountController : BaseController
    {
        private IAccount _accountRepository;
        
        
        //
        //Current Controller and current page
        private const string CONTROLLER_NAME = "Accounting/Account";

        public AccountController(IAccount accountRepository)
        {
            this._accountRepository = accountRepository;
        }

        // GET: /Accounting/Account/
        public ActionResult Index()
        {
            //SessionParameters();
            return View();
        }

        //display account gl
        [OutputCache(Duration = 3600)] 
        public ActionResult Account()
        {
            //Ext.Net.MVC.PartialViewResult rPVResult = new Ext.Net.MVC.PartialViewResult
            //{
            //    ClearContainer = true,
            //    //RenderMode = RenderMode.AddTo,
            //    ContainerId = "Body",
            //    //WrapByScriptTag = false,
            //    Model = ModelAccount()
            //};
            //this.SessionParameters();
            //Session["Curent_Page"] = "Account";
            //if (!LoadAction.isAutoriseToGetMenu(SessionProfileID, CodeValue.Accounting.Account.CODE, db))
            //{
            //    RedirectToAction("NotAuthorized", "User");
            //}


            //return rPVResult;
            return View(ModelAccount());
        }
        public List<object> ModelAccount()
        {
            List<object> list = new List<object>();


            (from acct in db.Accounts
             select acct)
                               .OrderBy(a => a.AccountNumber)
                                .Take(100)
                                .ToList().ForEach(c =>
            {
                list.Add(
                                new
                                {
                                    UICollectifAccountNumber = c.UICollectifAccountNumber,
                                    AccountID = c.AccountID,
                                    AccountLabel = c.AccountLabel,
                                    AccountNumber = c.AccountNumber,
                                    CollectifAccountID = c.CollectifAccountID,
                                    isManualPosting = c.isManualPosting
                                }
                        );
            });
            return list;
        }

        public DirectResult loadGrid()
        {
            this.GetCmp<Store>("Store").Reload();
            return this.Direct();
        }
        public ActionResult GetListAccount(String SearchOption, String SearchValue)
        {
            
            List<Account> listAccount = new List<Account>();
            
            if (SearchValue == null || SearchValue == "" || SearchOption == null || SearchOption=="")
            {
                listAccount = (from acct in db.Accounts
                               select acct)
                               .OrderBy(a => a.AccountNumber)
                                         .Take(25)
                                         .ToList();
            }
            else
            {
                if (SearchOption == "AC") //si recherche par numero de compte
                {
                        listAccount = (from acct in db.Accounts
                                       where acct.AccountNumber.ToString().StartsWith(SearchValue)
                                       select acct)
                                       .OrderBy(a => a.AccountNumber)
                                                 .Take(25)
                                                 .ToList();
                }

                if (SearchOption == "AN") //si recherche par Nom de compte
                {
                        listAccount = (from acct in db.Accounts
                                       where acct.AccountLabel.Contains(SearchValue)
                                       select acct)
                                       .OrderBy(a => a.AccountNumber)
                                                 .Take(25)
                                                 .ToList();
                }
            

            }
            
            List<object> list = new List<object>();
            listAccount.ForEach(c =>
            {
                list.Add(
                                new
                                {
                                    UICollectifAccountNumber = c.UICollectifAccountNumber,
                                    AccountID = c.AccountID,
                                    AccountLabel = c.AccountLabel,
                                    AccountNumber = c.AccountNumber,
                                    CollectifAccountID = c.CollectifAccountID,
                                    isManualPosting = c.isManualPosting
                                }
                        );
            });
            return this.Store(list);
        }
        //add gl account
        [HttpPost]
        public DirectResult AddAccount(Account account, int isManualPosting)
        {
            try
            {
                account.isManualPosting = Convert.ToBoolean(isManualPosting);
                if (account.AccountID > 0)
                {
                    _accountRepository.Update(account, account.AccountID);
                    statusOperation = account.AccountLabel + " : " + Resources.AlertUpdateAction;
                }
                else
                {
                    _accountRepository.Create(account);
                    statusOperation = account.AccountLabel + " : " + Resources.AlertAddAction;
                }
                this.GetCmp<FormPanel>("Account").Reset();
                this.GetCmp<Store>("Store").Reload();
                this.AlertSucces(Resources.Success, statusOperation);
                return this.Direct();
            }
            catch (Exception e)
            {
                statusOperation = Resources.er_alert_danger + e.Message;
                X.Msg.Alert(Resources.UIAccount, statusOperation).Show();
                return this.Direct();
            }
        }

        public DirectResult generateAccount(string CollectifAccountID)
        {
            int id = Convert.ToInt32(CollectifAccountID);
            int accountNumber = _accountRepository.AfficheAccountNumber(id);
            if (accountNumber != null)
            {
                this.GetCmp<TextField>("AccountNumber").Value = accountNumber;
            }
            else { this.GetCmp<TextField>("AccountNumber").Clear(); }
            return this.Direct();
        }

        //Initialize field for update
        public ActionResult IniatializeFieldAccount(string ID)
        {
            int id = Convert.ToInt32(ID);
            Account accountToUpdate = _accountRepository.FindAll.SingleOrDefault(c => c.AccountID == id);
            this.GetCmp<FormPanel>("Account").Reset(true);
            this.GetCmp<TextField>("AccountID").Value = accountToUpdate.AccountID;
            this.GetCmp<TextField>("AccountNumber").Value = accountToUpdate.AccountNumber;
            this.GetCmp<TextField>("AccountLabel").Value = accountToUpdate.AccountLabel;
            this.GetCmp<ComboBox>("CollectifAccountID").Value = accountToUpdate.CollectifAccountID;

            if (accountToUpdate.isManualPosting) this.GetCmp<Radio>("isManualPosting").Checked = true;
            else this.GetCmp<Radio>("isManualPostingNO").Checked = true;

            return this.Direct();
        }

        //Deletes actions
        [HttpPost]
        public ActionResult DeleteAccount(string ID)
        {
            try
            {
                int id = Convert.ToInt32(ID);
                
                Account accountToDelete = db.Accounts.Find(id);
                db.Accounts.Remove(accountToDelete);
                db.SaveChanges();
                //_accountRepository.Delete(accountToDelete);
                statusOperation = accountToDelete.AccountLabel + " : " + Resources.AlertDeleteAction;

                this.GetCmp<FormPanel>("Account").Reset();
                this.GetCmp<Store>("Store").Reload();
                this.AlertSucces(Resources.Success, statusOperation);
                return this.Direct();
            }
            catch (Exception e)
            {
                statusOperation = Resources.er_alert_danger + e.Message;
                X.Msg.Alert(Resources.UIAccount, statusOperation).Show();
                return this.Direct();
            }
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

                var rptSource = System.Web.HttpContext.Current.Session["rptSource"];

                if (string.IsNullOrEmpty(strReportName))
                {
                    isValid = false;
                }

                if (isValid)
                {
                    
                    string strRptPath = System.Web.HttpContext.Current.Server.MapPath("~/") + "Reports//Accounting//" + strReportName + ".rpt";
                    rptH.Load(strRptPath);
                    if (rptSource != null && rptSource.GetType().ToString() != "System.String")
                    {
                        rptH.SetDataSource(rptSource);
                    }
                    if (!string.IsNullOrEmpty(stBranchName1)) rptH.SetParameterValue("BranchName", stBranchName1);
                    if (!string.IsNullOrEmpty(strBranchInfo1)) rptH.SetParameterValue("BranchInfo", strBranchInfo1);
                    if (!string.IsNullOrEmpty(strRepTitle1)) rptH.SetParameterValue("RepTitle", strRepTitle1);
                    if (!string.IsNullOrEmpty(strOperator1)) rptH.SetParameterValue("Operator", strOperator1);
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
            Session["accop"] = null;

            Company cmpny = db.Companies.FirstOrDefault();

            this.Session["ReportName"] = "RptPlanCpta";
            this.Session["BranchName"] = cmpny.Name;
            this.Session["BranchInfo"] = "Tel: " + cmpny.Adress.AdressPhoneNumber + " Fax : " + cmpny.Adress.AdressFax + " Town :" + cmpny.Adress.Quarter.Town.TownLabel;
            this.Session["RepTitle"] = "GL ACCOUNTING PLAN";
            this.Session["Operator"] = CurrentUser.Name;

            Session["rptSource"] = ModelReport();

            return RedirectToAction("ShowGenericRpt");
        }
        private List<object> ModelReport()
        {
            List<object> list = new List<object>();
            db.Accounts.OrderBy(a => a.AccountNumber).ToList().ForEach(c =>
            {
                list.Add(
                                new
                                {
                                    CompteCle = c.AccountNumber.ToString(),
                                    LibelleCpte = c.AccountLabel
                                }
                        );
            });

            return list;

        }

        [HttpPost]
        public ActionResult ResetAccountForm()
        {
            this.GetCmp<FormPanel>("Account").Reset(true);
            this.GetCmp<Store>("Store").Reload();
            return this.Direct();
        }

	}
}