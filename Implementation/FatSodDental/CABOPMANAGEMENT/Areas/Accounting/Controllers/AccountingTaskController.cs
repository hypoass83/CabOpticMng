using FatSod.DataContext.Initializer;
using FatSod.Supply.Entities;
using FatSod.Supply.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using FatSod.Ressources;
using CABOPMANAGEMENT.Controllers;
using CABOPMANAGEMENT.Filters;

namespace CABOPMANAGEMENT.Areas.Accounting.Controllers
{
    [Authorize(Order = 1)]
    [TakeBusinessDay(Order = 2)]
    public class AccountingTaskController : BaseController
    {
        private IRepositorySupply<AccountingTask> _accountingTaskRepository;
        // GET: Administration/User
        //Current Controller and current page
        private const string CONTROLLER_NAME = "Accounting/AccountingTask";
        private const string VIEW_NAME = "Index";
        

        public AccountingTaskController(IRepositorySupply<AccountingTask> accountingTaskRepository)
        {
            this._accountingTaskRepository = accountingTaskRepository;
        }

        //[OutputCache(Duration = 3600)]
        public ActionResult Index()
        {
            return View(ModelAccount());
        }
        //[HttpPost]
        /*public JsonResult GetList()
        {
            List<object> list = new List<object>();
            db.AccountingTasks.ToList().ForEach(c =>
            {
                list.Add(
                            new
                            {
                                UIOperationCode = c.UIOperationCode,
                                UIAccountingSectionNumber = c.UIAccountingSectionNumber,
                                UIAccountNumber = c.UIAccountNumber,
                                AccountingTaskID = c.AccountingTaskID,
                                AccountingTaskSens = c.AccountingTaskSens,
                                AccountingTaskDescription = c.AccountingTaskDescription,
                                OperationID = c.OperationID,
                                AccountID = c.AccountID,
                                AccountingSectionID = c.AccountingSectionID,
                                ApplyVat = c.ApplyVat,
                                VatAccountID = c.VatAccountID,
                                UIVatAccountNumber = c.UIVatAccountNumber,
                                DiscountAccountID = c.DiscountAccountID,
                                UIDiscountAccountNumber = c.UIDiscountAccountNumber,
                                TransportAccountID = c.TransportAccountID,
                                UITransportAccountNumber = c.UITransportAccountNumber
                            }
                );
            });
            return Json(list, JsonRequestBehavior.AllowGet);
        }*/

        

        //chargement des combobox
        public JsonResult populateOperation()
        {

            List<object> _List = new List<object>();
            //queries 
            var queryList = (from s in db.Operations
                           select new { s.OperationID, s.OperationLabel, s.OperationCode }).ToList();

            foreach (var item in queryList.OrderBy(i => i.OperationCode))
            {
                _List.Add(new
                {
                    ID = item.OperationID,
                    Name = item.OperationCode
                });
            }
            //returns the Json result of _Journal
            return Json(_List, JsonRequestBehavior.AllowGet);
        }
        //charcgemlent des account section
        public JsonResult populateAccountingSection()
        {
            List<object> _List = new List<object>();
            //queries 
            var queryList = (from s in db.AccountingSections
                             select new { s.AccountingSectionID, s.AccountingSectionNumber, s.AccountingSectionLabel }).ToList();

            foreach (var item in queryList.OrderBy(i => i.AccountingSectionNumber))
            {
                _List.Add(new
                {
                    ID = item.AccountingSectionID,
                    Name = item.AccountingSectionNumber +" - "+ item.AccountingSectionLabel
                });
            }
            //returns the Json result of _Journal
            return Json(_List, JsonRequestBehavior.AllowGet);
        }

        public List<AccountingTask> ModelAccount()
        {
            List<AccountingTask> list = new List<AccountingTask>();
            db.AccountingTasks.ToList().ForEach(c =>
            {
                list.Add(
                                new AccountingTask
                                {
                                    UIOperationCode = (c.Operation==null) ? "" : c.Operation.OperationCode,
                                    UIAccountingSectionNumber = (c.AccountingSection == null) ? "" : c.AccountingSection.AccountingSectionNumber.ToString(),
                                    UIAccountNumber = (c.Account == null) ? "" : c.Account.AccountNumber.ToString(),
                                    AccountingTaskID = c.AccountingTaskID,
                                    AccountingTaskSens = c.AccountingTaskSens,
                                    AccountingTaskDescription = c.AccountingTaskDescription,
                                    OperationID = c.OperationID,
                                    AccountID = c.AccountID,
                                    AccountingSectionID = c.AccountingSectionID,
                                    ApplyVat = c.ApplyVat,
                                    VatAccountID = c.VatAccountID,
                                    UIVatAccountNumber = (c.AccountVAT == null) ? "" : c.AccountVAT.AccountNumber.ToString(),
                                    DiscountAccountID = c.DiscountAccountID,
                                    UIDiscountAccountNumber = (c.AccountDiscount == null) ? "" : c.AccountDiscount.AccountNumber.ToString(),
                                    TransportAccountID = c.TransportAccountID,
                                    UITransportAccountNumber = (c.AccountTransport == null) ? "" : c.AccountTransport.AccountNumber.ToString()
                                }
                );
            });
            return list;
        }

        //[HttpPost]
        public ActionResult AddAccountingTask(AccountingTask accountingTask)
        {
            bool status = false;
            try
            {
                if (accountingTask.AccountingTaskID > 0)
                {
                    //update de la table accounting task
                    _accountingTaskRepository.Update(accountingTask, accountingTask.AccountingTaskID);
                    statusOperation = Resources.Success + ":" +accountingTask.AccountingTaskDescription + " : " + Resources.AlertUpdateAction;
                    
                }
                else
                {
                    _accountingTaskRepository.Create(accountingTask);
                    statusOperation = Resources.Success +":"+ accountingTask.AccountingTaskDescription + " : " + Resources.AlertAddAction;
                    
                }

               
                status = true;
                return new JsonResult { Data = new { status = status, Message = statusOperation } };
            }
            catch (Exception e)
            {
                statusOperation = Resources.UIAccountingTask+":"+ Resources.er_alert_danger + e.Message;
                status = false;
                return new JsonResult { Data = new { status = status, Message = statusOperation } };
            }

        }
        //[HttpPost]
        public JsonResult InitializeFields(int id)
        {
            List<object> _List = new List<object>();
           
            
            AccountingTask accountingTaskSelect = db.AccountingTasks.SingleOrDefault(c => c.AccountingTaskID == id);
            
            _List.Add(new
            {
                AccountingTaskID = accountingTaskSelect.AccountingTaskID,
                OperationID = accountingTaskSelect.OperationID,
                AccountingTaskDescription = accountingTaskSelect.AccountingTaskDescription,
                AccountingSectionID = accountingTaskSelect.AccountingSectionID,
                AccountingTaskSens = accountingTaskSelect.AccountingTaskSens,
                ApplyVat = accountingTaskSelect.ApplyVat,
                VatAccountID = accountingTaskSelect.VatAccountID,
                AccountID = accountingTaskSelect.AccountID,
                DiscountAccountID = accountingTaskSelect.DiscountAccountID,
                TransportAccountID = accountingTaskSelect.TransportAccountID
            });
            return Json(_List, JsonRequestBehavior.AllowGet);
            
           
        }
        

        public JsonResult GetDiscountAccount(int OperationID) //retourne le compte d'escompte
        {
            List<object> list = new List<object>();
            
            Operation operationEntity = db.Operations.SingleOrDefault(o => o.OperationID == OperationID);
           
            //determinns le type d'operation
            int optypeId = (operationEntity==null) ? 0 : operationEntity.OperationTypeID;
            if (optypeId > 0)
            {
                OperationType opType = db.OperationTypes.SingleOrDefault(opt => opt.operationTypeID == optypeId);
                if (opType != null)
                {
                    //si achat
                    if (CodeValue.Accounting.InitOperationType.CODEPURCHASE.Contains(opType.operationTypeCode) || CodeValue.Accounting.InitOperationType.CODEPURCHASERETURN.Contains(opType.operationTypeCode))
                    {
                        List<Account> listAccount = db.Accounts.Where(t => t.CollectifAccount.AccountingSection.AccountingSectionCode == CodeValue.Accounting.DefaultCodeAccountingSection.CODEESCPTEACHAT).ToList();
                        listAccount.ForEach(c =>
                        {
                            list.Add(
                                        new
                                        {
                                            ID = c.AccountID,
                                            Name = c.AccountNumber.ToString() + "-" + c.AccountLabel
                                        }
                                );
                        });

                    }
                    //si vente
                    else if (CodeValue.Accounting.InitOperationType.CODESALE.Contains(opType.operationTypeCode) || CodeValue.Accounting.InitOperationType.CODESALERETURN.Contains(opType.operationTypeCode))
                    {
                        List<Account> listAccount = db.Accounts.Where(t => t.CollectifAccount.AccountingSection.AccountingSectionCode == CodeValue.Accounting.DefaultCodeAccountingSection.CODEESCPTEVENTE).ToList();
                        listAccount.ForEach(c =>
                        {
                            list.Add(
                                        new
                                        {
                                            ID = c.AccountID,
                                            Name = c.AccountNumber.ToString() + "-" + c.AccountLabel
                                        }
                                    );
                        });
                    }
                }
            }
            
            return Json(list, JsonRequestBehavior.AllowGet);
        }
       

        public JsonResult GetTransPortAccount(int OperationID) //retourne le compte de transport
        {
            List<object> list = new List<object>();
            
            Operation operationEntity = db.Operations.SingleOrDefault(o => o.OperationID == OperationID);
            
                //determinns le type d'operation
                int optypeId = (operationEntity==null) ?0 : operationEntity.OperationTypeID;
                if (optypeId > 0)
                {
                    OperationType opType = db.OperationTypes.SingleOrDefault(opt => opt.operationTypeID == optypeId);
                    if (opType != null)
                    {
                        //si achat
                        if (CodeValue.Accounting.InitOperationType.CODEPURCHASE.Contains(opType.operationTypeCode) ||CodeValue.Accounting.InitOperationType.CODEPURCHASERETURN.Contains(opType.operationTypeCode))
                        {
                            List<Account> listAccount = db.Accounts.Where(t => t.CollectifAccount.AccountingSection.AccountingSectionCode == CodeValue.Accounting.DefaultCodeAccountingSection.CODEPURCTRANSP).ToList();
                            listAccount.ForEach(c =>
                            {
                                list.Add(
                                            new
                                            {
                                                ID = c.AccountID,
                                                Name = c.AccountNumber.ToString() + "-" + c.AccountLabel
                                            }
                                    );
                            });

                        }
                        //si vente
                        else if (CodeValue.Accounting.InitOperationType.CODESALE.Contains(opType.operationTypeCode))
                        {
                            List<Account> listAccount = db.Accounts.Where(t => t.CollectifAccount.AccountingSection.AccountingSectionCode == CodeValue.Accounting.DefaultCodeAccountingSection.CODESALETRANSP).ToList();
                            listAccount.ForEach(c =>
                            {
                                list.Add(
                                    new
                                    {
                                        ID = c.AccountID,
                                        Name = c.AccountNumber.ToString() + "-" + c.AccountLabel
                                    }
                                );
                            });
                        }
                        else if (CodeValue.Accounting.InitOperationType.CODESALERETURN.Contains(opType.operationTypeCode))
                        {
                            List<Account> listAccount = db.Accounts.Where(t => t.CollectifAccount.AccountingSection.AccountingSectionCode == CodeValue.Accounting.DefaultCodeAccountingSection.CODERETSALETRANSP).ToList();
                            listAccount.ForEach(c =>
                            {
                                list.Add(
                                    new
                                    {
                                        ID = c.AccountID,
                                        Name = c.AccountNumber.ToString() + "-" + c.AccountLabel
                                    }
                                );
                            });
                        }
                    }
                }
                return Json(list, JsonRequestBehavior.AllowGet);
            }

        

        public JsonResult GetAccountNumber(int AccountingSectionID)
        {
            
                List<Account> listAccount = db.Accounts.Where(t => t.CollectifAccount.AccountingSectionID == AccountingSectionID).ToList();
                List<object> list = new List<object>();
                listAccount.ForEach(c =>
                {
                    list.Add(
                        new
                        {
                            ID = c.AccountID,
                            Name = c.AccountNumber.ToString() + "-" + c.AccountLabel
                        }
                    );
                });
            return Json(list, JsonRequestBehavior.AllowGet);

            
        }
        

        public JsonResult GetVATAccountNumber(int OperationID)
        {
            
            List<object> list = new List<object>();
            string ApplyVat = "";

            //determinns le type d'operation
            int optypeId = db.Operations.SingleOrDefault(o => o.OperationID == OperationID).OperationTypeID;
            if (optypeId > 0)
            {
                OperationType opType = db.OperationTypes.SingleOrDefault(opt => opt.operationTypeID == optypeId);
                if (opType != null)
                {
                    //si achat
                    if (CodeValue.Accounting.InitOperationType.CODEPURCHASE.Contains(opType.operationTypeCode)
                        ||  CodeValue.Accounting.InitOperationType.CODEPURCHASERETURN.Contains(opType.operationTypeCode))
                    {
                        ApplyVat = CodeValue.Accounting.DefaultCodeAccountingSection.CODETVADEDUC;
                    }
                    //si vente
                    else if (CodeValue.Accounting.InitOperationType.CODESALE.Contains(opType.operationTypeCode)
                        || CodeValue.Accounting.InitOperationType.CODESALERETURN.Contains(opType.operationTypeCode)
                        )
                    {
                        ApplyVat = CodeValue.Accounting.DefaultCodeAccountingSection.CODETVACOLECT;
                    }
                }
            }

            if (ApplyVat.Length>0)
            {
                AccountingSection accountingSectionSelect = db.AccountingSections.SingleOrDefault(c => (c.AccountingSectionCode == ApplyVat));
                if (accountingSectionSelect != null)
                {
                    List<Account> listAccount = db.Accounts.Where(t => t.CollectifAccount.AccountingSectionID == accountingSectionSelect.AccountingSectionID).ToList();
                    listAccount.ForEach(c =>
                    {
                        list.Add(
                            new
                            {
                                ID = c.AccountID,
                                Name = c.AccountNumber.ToString() + "-" + c.AccountLabel
                            }
                        );
                    });
                }
            }
            
                
            return Json(list, JsonRequestBehavior.AllowGet);

        }

        //***************Delete user
        //[HttpPost]
        public ActionResult DeleteTask(int ID)
        {
            bool status = false;
            try
            {
                
                AccountingTask accountingTaskToDelete = db.AccountingTasks.Find(ID);
                db.AccountingTasks.Remove(accountingTaskToDelete);
                db.SaveChanges();
                //_accountingTaskRepository.Delete(accountingTaskToDelete);
                statusOperation = Resources.Success +":"+ accountingTaskToDelete.AccountingTaskDescription + " : " + Resources.AlertDeleteAction;

                status = true;
                return new JsonResult { Data = new { status = status, Message = statusOperation } };

               
            }
            catch (Exception e)
            {
                statusOperation = Resources.UIAccountingTask +":"+ Resources.er_alert_danger + e.Message;
                status = false;
                return new JsonResult { Data = new { status = status, Message = statusOperation } };
               
            }
        }
        
    }
}