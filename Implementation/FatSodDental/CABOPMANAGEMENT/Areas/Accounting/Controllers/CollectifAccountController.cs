using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FatSod.DataContext.Concrete;
using FatSod.Supply.Entities;
using CABOPMANAGEMENT.Controllers;
using FatSod.Supply.Abstracts;
using CABOPMANAGEMENT.Tools;
using FatSod.Ressources;
using FatSod.DataContext.Initializer;
using CABOPMANAGEMENT.Filters;

namespace CABOPMANAGEMENT.Areas.Accounting.Controllers
{
    [Authorize(Order = 1)]
    [TakeBusinessDay(Order = 2)]
    public class CollectifAccountController : BaseController
    {
        private IRepositorySupply<AccountingSection> _accountingSectionRepository;
        private IRepositorySupply<CollectifAccount> _collectifAccountRepository;

        public CollectifAccountController(IRepositorySupply<AccountingSection> accountingSectionRepository, IRepositorySupply<CollectifAccount> collectifAccountRepository)
        {
            this._accountingSectionRepository = accountingSectionRepository;
            this._collectifAccountRepository = collectifAccountRepository;
            
        }

        public ActionResult AccountingSection()
        {
            return View(ModelAccountingSection().OrderBy(a=>a.AccountingSectionNumber));
        }
        public List<AccountingSection> ModelAccountingSection()
        {
            List<AccountingSection> list = new List<AccountingSection>();
            db.AccountingSections.ToList().ForEach(c =>
            {
                list.Add(
                        new AccountingSection
                        {
                            AccountingSectionID = c.AccountingSectionID,
                            AccountingSectionCode = c.AccountingSectionCode,
                            AccountingSectionNumber = c.AccountingSectionNumber,
                            AccountingSectionLabel = c.AccountingSectionLabel,
                            ClassAccountID = c.ClassAccountID,
                            ClassAccount = c.ClassAccount
                        }
                    );
            });
            return list;
        }
        public JsonResult populateClassAccount()
        {
            //holds list of ClassAccountss
            List<object> _ClassAccountsList = new List<object>();
            //queries all the ClassAccountss for its ID and Name property.
            var ClassAccountsList = (from s in db.ClassAccounts
                                     select new { s.ClassAccountID, s.ClassAccountLabel, s.ClassAccountNumber }).ToList();

            //save list of ClassAccountss to the _ClassAccountsList
            foreach (var item in ClassAccountsList.OrderBy(i=>i.ClassAccountNumber))
            {
                _ClassAccountsList.Add(new
                {
                    ID = item.ClassAccountID,
                    Name = item.ClassAccountNumber + " " + item.ClassAccountLabel
                });
            }
            //returns the Json result of _ClassAccountsList
            return Json(_ClassAccountsList, JsonRequestBehavior.AllowGet);
        }
        public JsonResult LoadClassAccount(int ClassAccountID)
        {
            var selectClassAccount = db.ClassAccounts.Where(c => c.ClassAccountID == ClassAccountID).Select(c => new { value=c.ClassAccountNumber });
            return Json(selectClassAccount, JsonRequestBehavior.AllowGet);
        }
        //accounting section
        public JsonResult LoadAccountingSection(int AccountingSectionID)
        {
            var selectAccountingSection = db.AccountingSections.Where(c => c.AccountingSectionID == AccountingSectionID).Select(c => new { value = c.AccountingSectionNumber });
            return Json(selectAccountingSection, JsonRequestBehavior.AllowGet);
        }
        public JsonResult populateAccountingSection()
        {
            //holds list of ClassAccountss
            List<object> _AccountingSectionList = new List<object>();
            //queries all the ClassAccountss for its ID and Name property.
            var AccountingSectionsList = (from s in db.AccountingSections
                                     select new { s.AccountingSectionID, s.AccountingSectionLabel, s.AccountingSectionNumber }).ToList();

            //save list of ClassAccountss to the _ClassAccountsList
            foreach (var item in AccountingSectionsList.OrderBy(i => i.AccountingSectionNumber))
            {
                _AccountingSectionList.Add(new
                {
                    ID = item.AccountingSectionID,
                    Name = item.AccountingSectionNumber + " " + item.AccountingSectionLabel
                });
            }
            //returns the Json result of _ClassAccountsList
            return Json(_AccountingSectionList, JsonRequestBehavior.AllowGet);
        }
        //************ Add action
        //add accounting section
        [HttpPost]
        public JsonResult AddAccountingSection(AccountingSection accountingSection, string ASClass, string ASNumber)
        {
            bool status = false;
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
                status = true; 
                //this.AlertSucces( statusOperation);
                return new JsonResult { Data = new { status = status, Message = statusOperation } };
            }
            catch (Exception e)
            {
                status = false; 
                statusOperation = Resources.er_alert_danger + e.Message;
                //this.AlertSucces(statusOperation);
                return new JsonResult { Data = new { status = status, Message = statusOperation } };
            }
        }
        //
        public ActionResult AddCollectifAccount(CollectifAccount collectifAccount, string ACSection, string ACNumber)
        {
            bool status = false;
            try
            {
                string acFinalNumber= (ACSection + ACNumber);
                collectifAccount.CollectifAccountNumber = Convert.ToInt32(acFinalNumber);
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
                status = true;
            }
            catch (Exception e)
            {
                statusOperation = Resources.er_alert_danger + e.Message;
                status = false;
            }
            return new JsonResult { Data = new { status = status, Message = statusOperation } };
        }
        //
        public JsonResult InitializeFieldsCollectif(int ID)
        {
            //int id = Convert.ToInt32(ID);
            CollectifAccount CollectifAccountToUpdate = _collectifAccountRepository.FindAll.FirstOrDefault(b => b.CollectifAccountID == ID);
            List<object> _CollectifAccountList = new List<object>();
            _CollectifAccountList.Add(new
            {
                AccountingSectionID = CollectifAccountToUpdate.AccountingSection.AccountingSectionID,
                CollectifAccountID = CollectifAccountToUpdate.CollectifAccountID,
                ACSection = CollectifAccountToUpdate.CollectifAccountNumber.ToString().Substring(0, 3),
                ACNumber = CollectifAccountToUpdate.CollectifAccountNumber.ToString().Substring(3),
                CollectifAccountLabel = CollectifAccountToUpdate.CollectifAccountLabel
            });
            return Json(_CollectifAccountList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult InitializeFields(int ID)
        {
            //int id = Convert.ToInt32(ID);
            AccountingSection AccSectionToUpdate = _accountingSectionRepository.FindAll.FirstOrDefault(b => b.AccountingSectionID == ID);
            List<object> _AccSectionList = new List<object>();
            _AccSectionList.Add(new
            {
                AccountingSectionCode = AccSectionToUpdate.AccountingSectionCode,
                ClassAccountID = AccSectionToUpdate.ClassAccountID,
                ASClass =  AccSectionToUpdate.AccountingSectionNumber.ToString().Substring(0,1),
                ASNumber = AccSectionToUpdate.AccountingSectionNumber.ToString().Substring(1),
                AccountingSectionLabel = AccSectionToUpdate.AccountingSectionLabel
            });
            return Json(_AccSectionList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteCollectifAccount(int ID)
        {
            bool status = false;
            try
            {
                CollectifAccount collectifAccountToDelete = db.CollectifAccounts.Find(ID);
                db.CollectifAccounts.Remove(collectifAccountToDelete);
                db.SaveChanges();
                statusOperation = collectifAccountToDelete.CollectifAccountLabel + " : " + Resources.AlertDeleteAction;

                status = true;
            }
            catch (Exception e)
            {
                statusOperation = Resources.er_alert_danger + e.Message;
                status = false;
            }
            return new JsonResult { Data = new { status = status, Message = statusOperation } };
        }

        public JsonResult DeleteAccountingSection(int ID)
        {
            bool status = false;
            try
            {
                AccountingSection AccountingSectionToDelete = db.AccountingSections.Find(ID);
                if (AccountingSectionToDelete != null)
                {
                    //assurons ns que la suppression d'un des acc section predefini ne peux etre supprime
                    if (AccountingSectionToDelete.AccountingSectionCode == CodeValue.Accounting.DefaultCodeAccountingSection.CODECLIENT || AccountingSectionToDelete.AccountingSectionCode == CodeValue.Accounting.DefaultCodeAccountingSection.CODEFOURN || AccountingSectionToDelete.AccountingSectionCode == CodeValue.Accounting.DefaultCodeAccountingSection.CODEPROD || AccountingSectionToDelete.AccountingSectionCode == CodeValue.Accounting.DefaultCodeAccountingSection.CODEBANK || AccountingSectionToDelete.AccountingSectionCode == CodeValue.Accounting.DefaultCodeAccountingSection.CODECAIS)
                    {
                        statusOperation = Resources.deleteAcctSecImp;
                        status = false;
                        
                    }
                    else
                    {
                        db.AccountingSections.Remove(AccountingSectionToDelete);
                        db.SaveChanges();
                        status = true;
                        statusOperation = AccountingSectionToDelete.AccountingSectionLabel + " : " + Resources.AlertDeleteAction;
                    }
                }
                else
                {
                    status = false;
                    statusOperation = Resources.er_alert_danger;
                }
            }
            catch (Exception e)
            {
                status = false;
                statusOperation = Resources.er_alert_danger + e.Message;
            }
            return new JsonResult { Data = new { status = status, Message = statusOperation } };
        }

        public ActionResult CollectifAccount()
        {
            return View(ModelCollectifAccount());
        }

        public List<CollectifAccount> ModelCollectifAccount()
        {
            List<CollectifAccount> list = new List<CollectifAccount>();
            _collectifAccountRepository.FindAll.OrderBy(a => a.CollectifAccountNumber).ToList().ForEach(c =>
            {
                list.Add(
                                new CollectifAccount
                                {
                                    AccountingSection = c.AccountingSection,
                                    CollectifAccountID = c.CollectifAccountID,
                                    CollectifAccountNumber = c.CollectifAccountNumber,
                                    CollectifAccountLabel = c.CollectifAccountLabel
                                }
                        );
            });
            return list;
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
