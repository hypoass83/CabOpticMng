
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
using System.Web.UI;
using FatSod.DataContext.Concrete;


namespace FatSodDental.UI.Areas.Accounting.Controllers
{
    [Authorize(Order = 1)]
    [TakeBusinessDay(Order = 2)]
    public class CollectifAccountController : BaseController
    {
        private IRepositorySupply<AccountingSection> _accountingSectionRepository;
        private IRepositorySupply<CollectifAccount> _collectifAccountRepository;
        //
        //Current Controller and current page
        private const string CONTROLLER_NAME = "Accounting/CollectifAccount";
        

        public CollectifAccountController(IRepositorySupply<AccountingSection> accountingSectionRepository, IRepositorySupply<CollectifAccount> collectifAccountRepository)
        {
            this._accountingSectionRepository = accountingSectionRepository;
            this._collectifAccountRepository = collectifAccountRepository;
            
        }

        //private void SessionParameters()
        //{
        //    Session["UserProfile"] = SessionProfileID;
        //    Session["UserID"] = SessionGlobalPersonID;
        //    //Session["Curent_Controller"] = CONTROLLER_NAME;
        //}
        // GET: /Accounting/Account/
        public ActionResult Index()
        {
            //SessionParameters();
            return View();
        }
        //display account section
        //[OutputCache(Duration = 3600)] 
        public ActionResult AccountingSection()
        {
            //Ext.Net.MVC.PartialViewResult rPVResult = new Ext.Net.MVC.PartialViewResult
            //{
            //    ClearContainer = true,
            //    //RenderMode = RenderMode.AddTo,
            //    ContainerId = "Body",
            //    //WrapByScriptTag = false,
            //    Model = ModelAccountingSection()
            //};
            //this.SessionParameters();
            //Session["Curent_Page"] = "AccountingSection";
            //if (!LoadAction.isAutoriseToGetMenu(SessionProfileID, CodeValue.Accounting.AccountingSection.CODE, db))
            //{
            //    RedirectToAction("NotAuthorized", "User");
            //}

            return View(ModelAccountingSection());
        }

        public List<object> ModelAccountingSection()
        {
            List<object> list = new List<object>();
            db.AccountingSections.ToList().ForEach(c =>
            {
                list.Add(
                        new
                        {
                            AccountingSectionID = c.AccountingSectionID,
                            AccountingSectionCode = c.AccountingSectionCode,
                            AccountingSectionNumber = c.AccountingSectionNumber,
                            AccountingSectionLabel = c.AccountingSectionLabel,
                            ClassAccountID = c.ClassAccountID,
                            UIClassAccountNumber = c.UIClassAccountNumber
                        }
                    );
            });
            return list;
        }
        //display account gl
        //[OutputCache(Duration = 3600)] 
        public ActionResult CollectifAccount()
        {
            //Ext.Net.MVC.PartialViewResult rPVResult = new Ext.Net.MVC.PartialViewResult
            //{
            //    ClearContainer = true,
            //    //RenderMode = RenderMode.AddTo,
            //    ContainerId = "Body",
            //    //WrapByScriptTag = false,
            //    Model = ModelCollectifAccount()
            //};
            //this.SessionParameters();
            //Session["Curent_Page"] = "CollectifAccount";
            //if (!LoadAction.isAutoriseToGetMenu(SessionProfileID, CodeValue.Accounting.CollectifAccount.CODE, db))
            //{
            //    RedirectToAction("NotAuthorized", "User");
            //}

            return View(ModelCollectifAccount());
        }
        public List<object> ModelCollectifAccount()
        {
            List<object> list = new List<object>();
            _collectifAccountRepository.FindAll.OrderBy(a => a.CollectifAccountNumber).ToList().ForEach(c =>
            {
                list.Add(
                                new
                                {
                                    UIAccountingSectionNumber = c.UIAccountingSectionNumber,
                                    AccountingSectionID = c.AccountingSectionID,
                                    CollectifAccountID = c.CollectifAccountID,
                                    CollectifAccountNumber = c.CollectifAccountNumber,
                                    CollectifAccountLabel = c.CollectifAccountLabel
                                }
                        );
            });
            return list;
        }
        //************ Add action
        //add accounting section
        [HttpPost]
        public ActionResult AddAccountingSection(AccountingSection accountingSection, string ASClass, string ASNumber)
        {
            try
            {
                accountingSection.AccountingSectionNumber = Convert.ToInt32(ASClass + ASNumber);
                if (accountingSection.AccountingSectionID > 0)
                {
                    AccountingSection accountingSectionTOUpdate = db.AccountingSections.SingleOrDefault(c => c.AccountingSectionID == accountingSection.AccountingSectionID);
                    accountingSection.AccountingSectionCode = accountingSectionTOUpdate.AccountingSectionCode;

                    _accountingSectionRepository.Update(accountingSection, accountingSection.AccountingSectionID);
                    statusOperation = accountingSection.AccountingSectionLabel + " : " + Resources.AlertUpdateAction;
                }
                else
                {
                    _accountingSectionRepository.Create(accountingSection);

                    statusOperation = accountingSection.AccountingSectionLabel + " : " + Resources.AlertAddAction;
                }

                this.GetCmp<Store>("Store").Reload();
                this.GetCmp<FormPanel>("AccountingSection").Reset();
                this.AlertSucces(Resources.Success, statusOperation);
                return this.Direct();
            }
            catch (Exception e)
            {
                statusOperation = Resources.er_alert_danger + e.Message;
                X.Msg.Alert(Resources.UIAccountingSectionID, statusOperation).Show();
                return this.Direct();
            }
        }
        //add gl account
        [HttpPost]
        public ActionResult AddCollectifAccount(CollectifAccount collectifAccount, string ACSection, string ACNumber)
        {
            try
            {
                collectifAccount.CollectifAccountNumber = Convert.ToInt32(ACSection + ACNumber);
                if (collectifAccount.CollectifAccountID > 0)
                {
                    _collectifAccountRepository.Update(collectifAccount, collectifAccount.CollectifAccountID);
                    statusOperation = collectifAccount.CollectifAccountLabel + " : " + Resources.AlertUpdateAction;
                }
                else
                {
                    _collectifAccountRepository.Create(collectifAccount);
                    statusOperation = collectifAccount.CollectifAccountLabel + " : " + Resources.AlertAddAction;
                }
                this.GetCmp<FormPanel>("CollectifAccount").Reset();
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


        public ActionResult LoadClassAccount(string ClassAccountID)
        {
            int id = Convert.ToInt32(ClassAccountID);
            ClassAccount selectClassAccount = db.ClassAccounts.SingleOrDefault(c => c.ClassAccountID == id);
            if (selectClassAccount != null)
            {
                this.GetCmp<TextField>("ASClass").Value = selectClassAccount.ClassAccountNumber;
            }
            else { this.GetCmp<TextField>("ASClass").Clear(); }
            return this.Direct();
        }
        public ActionResult LoadAccountSectionNumber(string accountingSectionID)
        {
            int id = Convert.ToInt32(accountingSectionID);
            AccountingSection selectAccountingSection = db.AccountingSections.SingleOrDefault(c => c.AccountingSectionID == id);
            if (selectAccountingSection != null)
            {
                this.GetCmp<TextField>("ACSection").Value = selectAccountingSection.AccountingSectionNumber;
            }
            else { this.GetCmp<TextField>("ACSection").Clear(); }
            return this.Direct();
        }
        //Initialize field for update
        public ActionResult ClickUpdateCollectifAccount(string ID)
        {
            int id = Convert.ToInt32(ID);
            CollectifAccount collectifAccountToUpdate = db.CollectifAccounts.SingleOrDefault(c => c.CollectifAccountID == id);
            this.GetCmp<FormPanel>("CollectifAccount").Reset(true);
            this.GetCmp<TextField>("CollectifAccountID").Value = collectifAccountToUpdate.CollectifAccountID;
            this.GetCmp<ComboBox>("AccountingSectionID").Value = collectifAccountToUpdate.AccountingSectionID;
            this.GetCmp<TextField>("ACSection").Value = collectifAccountToUpdate.UIAccountingSectionNumber;
            this.GetCmp<TextField>("ACNumber").Value = collectifAccountToUpdate.CollectifAccountNumber.ToString().Substring(collectifAccountToUpdate.UIAccountingSectionNumber.Length);
            this.GetCmp<TextField>("CollectifAccountLabel").Value = collectifAccountToUpdate.CollectifAccountLabel;
           
            //if (collectifAccountToUpdate.isManualPosting) this.GetCmp<Radio>("isManualPosting").Checked = true;
            //else this.GetCmp<Radio>("isManualPostingNO").Checked = true;
            
            return this.Direct();
        }
        public ActionResult ClickUpdateAccountingSection(string ID)
        {
            int id = Convert.ToInt32(ID);
            AccountingSection accountingSectionToUpdate = db.AccountingSections.SingleOrDefault(c => c.AccountingSectionID == id);
            this.GetCmp<FormPanel>("AccountingSection").Reset(true);
            this.GetCmp<TextField>("AccountingSectionID").Value = accountingSectionToUpdate.AccountingSectionID;
            if (
                   accountingSectionToUpdate.AccountingSectionCode == CodeValue.Accounting.DefaultCodeAccountingSection.CODEPROD
                || accountingSectionToUpdate.AccountingSectionCode == CodeValue.Accounting.DefaultCodeAccountingSection.CODEFOURN
                || accountingSectionToUpdate.AccountingSectionCode == CodeValue.Accounting.DefaultCodeAccountingSection.CODECLIENT
                || accountingSectionToUpdate.AccountingSectionCode == CodeValue.Accounting.DefaultCodeAccountingSection.CODEBANK
                || accountingSectionToUpdate.AccountingSectionCode == CodeValue.Accounting.DefaultCodeAccountingSection.CODECAIS
                || accountingSectionToUpdate.AccountingSectionCode == CodeValue.Accounting.DefaultCodeAccountingSection.CODEPOSTCHECK
                || accountingSectionToUpdate.AccountingSectionCode == CodeValue.Accounting.DefaultCodeAccountingSection.CODETVACOLECT
                || accountingSectionToUpdate.AccountingSectionCode == CodeValue.Accounting.DefaultCodeAccountingSection.CODETVADEDUC
                || accountingSectionToUpdate.AccountingSectionCode == CodeValue.Accounting.DefaultCodeAccountingSection.CODESALETRANSP
                || accountingSectionToUpdate.AccountingSectionCode == CodeValue.Accounting.DefaultCodeAccountingSection.CODEPURCTRANSP
                || accountingSectionToUpdate.AccountingSectionCode == CodeValue.Accounting.DefaultCodeAccountingSection.CODERETSALETRANSP
                || accountingSectionToUpdate.AccountingSectionCode == CodeValue.Accounting.DefaultCodeAccountingSection.CODEESCPTEACHAT
                || accountingSectionToUpdate.AccountingSectionCode == CodeValue.Accounting.DefaultCodeAccountingSection.CODEESCPTEVENTE
                || accountingSectionToUpdate.AccountingSectionCode == CodeValue.Accounting.DefaultCodeAccountingSection.CODEACHATMSE
                || accountingSectionToUpdate.AccountingSectionCode == CodeValue.Accounting.DefaultCodeAccountingSection.CODEVENTEMSE
                || accountingSectionToUpdate.AccountingSectionCode == CodeValue.Accounting.DefaultCodeAccountingSection.CODESTOCKVAR
                )
            {
                this.GetCmp<TextField>("AccountingSectionCode").Disabled = true;
            }
            else
            {
                this.GetCmp<TextField>("AccountingSectionCode").Disabled = false;
            }

            this.GetCmp<TextField>("AccountingSectionCode").Value = accountingSectionToUpdate.AccountingSectionCode;
            this.GetCmp<ComboBox>("ClassAccountID").Value = accountingSectionToUpdate.ClassAccountID;
            this.GetCmp<TextField>("ASClass").Value = accountingSectionToUpdate.ClassAccount.ClassAccountNumber;
            this.GetCmp<TextField>("ASNumber").Value = accountingSectionToUpdate.AccountingSectionNumber.ToString().Substring(accountingSectionToUpdate.ClassAccount.ClassAccountNumber.ToString().Length);
            this.GetCmp<TextField>("AccountingSectionLabel").Value = accountingSectionToUpdate.AccountingSectionLabel;

            return this.Direct();
        }

        //Deletes actions
        [HttpPost]
        public ActionResult DeleteCollectifAccount(string ID)
        {
            try
            {
                int id = Convert.ToInt32(ID);
                CollectifAccount collectifAccountToDelete = db.CollectifAccounts.Find(id);
                db.CollectifAccounts.Remove(collectifAccountToDelete);
                db.SaveChanges();
                //_collectifAccountRepository.Delete(collectifAccountToDelete);
                statusOperation = collectifAccountToDelete.CollectifAccountLabel + " : " + Resources.AlertDeleteAction;

                this.GetCmp<FormPanel>("CollectifAccount").Reset();
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
        [HttpPost]
        public ActionResult DeleteAccountingSection(string ID)
        {
            try
            {
                int id = Convert.ToInt32(ID);
                AccountingSection AccountingSectionToDelete =db.AccountingSections.Find(id);
                if (AccountingSectionToDelete != null)
                {
                    //assurons ns que la suppression d'un des acc section predefini ne peux etre supprime
                    if (AccountingSectionToDelete.AccountingSectionCode == CodeValue.Accounting.DefaultCodeAccountingSection.CODECLIENT || AccountingSectionToDelete.AccountingSectionCode == CodeValue.Accounting.DefaultCodeAccountingSection.CODEFOURN || AccountingSectionToDelete.AccountingSectionCode == CodeValue.Accounting.DefaultCodeAccountingSection.CODEPROD || AccountingSectionToDelete.AccountingSectionCode == CodeValue.Accounting.DefaultCodeAccountingSection.CODEBANK || AccountingSectionToDelete.AccountingSectionCode == CodeValue.Accounting.DefaultCodeAccountingSection.CODECAIS)
                    {
                        statusOperation = Resources.deleteAcctSecImp;
                        X.Msg.Alert(Resources.UIAccountingSectionID, statusOperation).Show();
                        return this.Direct();
                    }
                    else
                    {
                        db.AccountingSections.Remove(AccountingSectionToDelete);
                        db.SaveChanges();
                        //_accountingSectionRepository.Delete(AccountingSectionToDelete);
                        statusOperation = AccountingSectionToDelete.AccountingSectionLabel + " : " + Resources.AlertDeleteAction;
                    }
                }
                else
                {
                    statusOperation = Resources.er_alert_danger;
                    X.Msg.Alert(Resources.UIAccountingSectionID, statusOperation).Show();
                    return this.Direct();
                }
                this.GetCmp<Store>("Store").Reload();
                this.GetCmp<FormPanel>("AccountingSection").Reset();

                this.AlertSucces(Resources.Success, statusOperation);
                return this.Direct();
            }
            catch (Exception e)
            {
                statusOperation = Resources.er_alert_danger + e.Message;
                X.Msg.Alert(Resources.UIAccountingSectionID, statusOperation).Show();
                return this.Direct();
            }
        }

        [HttpPost]
        public ActionResult GetListCollectifAccount()
        {
            List<object> list = new List<object>();
            db.CollectifAccounts.OrderBy(a => a.CollectifAccountNumber).ToList().ForEach(c =>
            {
                list.Add(
                                new
                                {
                                    UIAccountingSectionNumber = c.UIAccountingSectionNumber,
                                    AccountingSectionID = c.AccountingSectionID,
                                    CollectifAccountID = c.CollectifAccountID,
                                    CollectifAccountNumber = c.CollectifAccountNumber,
                                    CollectifAccountLabel = c.CollectifAccountLabel
                                }
                        );
            });
            return this.Store(list);
        }
        [HttpPost]
        public ActionResult GetListAccountingSection()
        {
            List<object> list = new List<object>();
            db.AccountingSections.ToList().ForEach(c =>
            {
                list.Add(
                                new
                                {
                                    AccountingSectionID = c.AccountingSectionID,
                                    AccountingSectionCode = c.AccountingSectionCode,
                                    AccountingSectionNumber = c.AccountingSectionNumber,
                                    AccountingSectionLabel = c.AccountingSectionLabel,
                                    ClassAccountID = c.ClassAccountID,
                                    UIClassAccountNumber = c.UIClassAccountNumber
                                }
                        );
            });
            return this.Store(list);
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
            this.Session["RepTitle"] = "COLLECTIVE ACCOUNTING";
            this.Session["Operator"] = CurrentUser.Name;

            Session["rptSource"] = ModelReport();

            return RedirectToAction("ShowGenericRpt");
        }
        private List<object> ModelReport()
        {
            List<object> list = new List<object>();
            db.CollectifAccounts.OrderBy(a => a.CollectifAccountNumber).ToList().ForEach(c =>
            {
                list.Add(
                                new
                                {
                                    CompteCle = c.CollectifAccountNumber.ToString(),
                                    LibelleCpte = c.CollectifAccountLabel
                                }
                        );
            });

            return list;

        }
        [HttpPost]
        public ActionResult ResetCollectifAccountForm()
        {
            this.GetCmp<FormPanel>("CollectifAccount").Reset(true);
            this.GetCmp<Store>("Store").Reload();
            return this.Direct();
        }
        [HttpPost]
        public ActionResult ResetAcctSectForm()
        {
            this.GetCmp<FormPanel>("AccountingSection").Reset(true);
            this.GetCmp<Store>("Store").Reload();
            return this.Direct();
        }
    }
}