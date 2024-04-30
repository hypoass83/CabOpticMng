using Ext.Net;
using Ext.Net.MVC;
using FatSod.DataContext.Initializer;
using FatSod.Security.Abstracts;
using FatSod.Security.Entities;
using FatSod.Supply.Entities;
using FatSod.Supply.Abstracts;
using FatSodDental.UI.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using FatSod.Ressources;
using FatSodDental.UI.Controllers;
using FatSodDental.UI.Filters;
using System.Web.UI;
using FatSod.DataContext.Concrete;

namespace FatSodDental.UI.Areas.Accounting.Controllers
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
        [HttpPost]
        public ActionResult GetList()
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
            return this.Store(list);
        }
        //[OutputCache(Duration = 3600)] 
        public ActionResult Index()
        {
            //Ext.Net.MVC.PartialViewResult rPVResult = new Ext.Net.MVC.PartialViewResult
            //{
            //    ClearContainer = true,
            //    //RenderMode = RenderMode.AddTo,
            //    ContainerId = "Body",
            //    //WrapByScriptTag = false,
            //    Model = ModelAccount()
            //};
            //Session["Curent_Page"] = VIEW_NAME;
            //Session["Curent_Controller"] = CONTROLLER_NAME;
            //We verify is the current has right to access of this action
            //if (!LoadAction.isAutoriseToGetMenu(SessionProfileID, CodeValue.Accounting.AccountingTask.CODE, db))
            //{
            //    RedirectToAction("NotAuthorized", "User");
            //}
            return View(ModelAccount());
        }

        public List<object> ModelAccount()
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
            return list;
        }

        [HttpPost]
        public ActionResult AddAccountingTask(AccountingTask accountingTask)
        {
            try
            {
                if (accountingTask.AccountingTaskID > 0)
                {
                    //update de la table accounting task
                    _accountingTaskRepository.Update(accountingTask, accountingTask.AccountingTaskID);
                    statusOperation = accountingTask.AccountingTaskDescription + " : " + Resources.AlertUpdateAction;
                    this.GetCmp<FormPanel>("AccountingTask").Reset(true);
                }
                else
                {
                    _accountingTaskRepository.Create(accountingTask);
                    statusOperation = accountingTask.AccountingTaskDescription + " : " + Resources.AlertAddAction;
                    this.GetCmp<ComboBox>("AccountingSectionID").Reset();
                    this.GetCmp<ComboBox>("AccountingTaskSens").Reset();
                    this.GetCmp<ComboBox>("cbAccountID").Reset();
                    this.GetCmp<ComboBox>("ApplyVat").Reset();
                    this.GetCmp<ComboBox>("VatAccountID").Reset();
                    this.GetCmp<ComboBox>("DiscountAccountID").Reset();
                    this.GetCmp<ComboBox>("TransportAccountID").Reset();
                }

                this.GetCmp<Store>("Store").Reload();
                this.AlertSucces(Resources.Success, statusOperation);
                return this.Direct();
            }
            catch (Exception e)
            {
                statusOperation = Resources.er_alert_danger + e.Message;
                X.Msg.Alert(Resources.UIAccountingTask, statusOperation).Show();
                return this.Direct();
            }

        }
        [HttpPost]
        public ActionResult InitializeFields(string ID)
        {
            bool res = false;
            try
            {
                int id = Convert.ToInt32(ID);
                AccountingTask accountingTaskSelect = db.AccountingTasks.SingleOrDefault(c => c.AccountingTaskID == id);

                this.GetCmp<FormPanel>("AccountingTask").Reset(true);
                this.GetCmp<TextField>("AccountingTaskID").Value = accountingTaskSelect.AccountingTaskID;
                this.GetCmp<ComboBox>("OperationID").SetValueAndFireSelect(accountingTaskSelect.OperationID);
                this.GetCmp<TextField>("AccountingTaskDescription").Value = accountingTaskSelect.AccountingTaskDescription;
                this.GetCmp<ComboBox>("AccountingSectionID").SetValueAndFireSelect(accountingTaskSelect.AccountingSectionID);
                this.GetCmp<ComboBox>("AccountingTaskSens").Value = accountingTaskSelect.AccountingTaskSens;
                this.GetCmp<ComboBox>("ApplyVat").SetValueAndFireSelect(accountingTaskSelect.ApplyVat);
                this.DesableValue(accountingTaskSelect.ApplyVat.ToString());

                if (accountingTaskSelect.VatAccountID != null)
                {
                    this.GetCmp<ComboBox>("VatAccountID").Disabled = false;
                    this.GetCmp<ComboBox>("VatAccountID").SetValue(accountingTaskSelect.VatAccountID);
                }
                else
                {
                    this.GetCmp<ComboBox>("VatAccountID").Disabled = true;
                    this.GetCmp<ComboBox>("VatAccountID").Clear();
                }

                if (accountingTaskSelect.AccountID != null)
                {
                    this.GetCmp<ComboBox>("cbAccountID").Disabled = false;
                    this.GetCmp<ComboBox>("cbAccountID").SetValue(accountingTaskSelect.AccountID);
                }
                else
                {
                    this.GetCmp<ComboBox>("cbAccountID").Disabled = true;
                    this.GetCmp<ComboBox>("cbAccountID").Clear();
                }
                Operation operationEntity = db.Operations.SingleOrDefault(o => o.OperationID == accountingTaskSelect.OperationID);
                res = this.disablefieldReglement(operationEntity.MacroOperationID, "DiscountAccountID");
                if (res == false) this.GetCmp<ComboBox>("DiscountAccountID").SetValue(accountingTaskSelect.DiscountAccountID);
                res = this.disablefieldReglement(operationEntity.MacroOperationID, "TransportAccountID");
                if (res == false) this.GetCmp<ComboBox>("TransportAccountID").SetValue(accountingTaskSelect.TransportAccountID);
                
            }
            catch (Exception e)
            {
                statusOperation = Resources.er_alert_danger + e.Message;
                X.Msg.Alert(Resources.UIAccountingTask, statusOperation).Show();
                return this.Direct();
            }
            return this.Direct();
        }
        [HttpPost]
        public ActionResult GetVATProperty(string OperationID) //charge le champs apply vat : 443(deductible) si achat et 445(Collecte) si vente
        {
            if (string.IsNullOrEmpty(OperationID))
            {
                return this.Direct();
            }
            List<object> list = new List<object>();
            list.Add(
                    new
                    {
                        VatAcctSectionCode = "NONE",
                        VatAcctLabel = Resources.UINONE
                    }
            );
            int id=Convert.ToInt32(OperationID);
            //determinns le type d'operation
            int optypeId = db.Operations.SingleOrDefault(o => o.OperationID ==id ).OperationTypeID;
            if (optypeId > 0)
            {
                OperationType opType = db.OperationTypes.SingleOrDefault(opt => opt.operationTypeID == optypeId);
                if (opType != null)
                {
                    //si achat
                    if (opType.operationTypeCode == CodeValue.Accounting.InitOperationType.CODEPURCHASE 
                        || opType.operationTypeCode == CodeValue.Accounting.InitOperationType.CODEPURCHASERETURN)
                    {

                        list.Add(
                                new
                                {
                                    VatAcctSectionCode = CodeValue.Accounting.DefaultCodeAccountingSection.CODETVADEDUC,
                                    VatAcctLabel = Resources.UIDeductVat
                                }
                        );

                    }
                    //si vente
                    else if (opType.operationTypeCode == CodeValue.Accounting.InitOperationType.CODESALE 
                        || opType.operationTypeCode == CodeValue.Accounting.InitOperationType.CODESALERETURN
                        )
                    {
                        list.Add(
                                new
                                {
                                    VatAcctSectionCode = CodeValue.Accounting.DefaultCodeAccountingSection.CODETVACOLECT,
                                    VatAcctLabel = Resources.UICollectVat
                                }
                        );
                    }
                }
            }
            return this.Store(list);
        }
        [HttpPost]
        public ActionResult GetDiscountAccount(string OperationID) //retourne le compte d'escompte
        {
            if (string.IsNullOrEmpty(OperationID))
            {
                return this.Direct();
            }
            List<object> list = new List<object>();
            this.GetCmp<ComboBox>("DiscountAccountID").Clear();
            int id =Convert.ToInt32(OperationID);
            Operation operationEntity = db.Operations.SingleOrDefault(o => o.OperationID ==id );
            //determination de la macro operation
            int opMacroId = operationEntity.MacroOperationID;
            bool res = this.disablefieldReglement(opMacroId, "DiscountAccountID");
            if (res == false)
            {
                //determinns le type d'operation
                int optypeId = operationEntity.OperationTypeID;
                if (optypeId > 0)
                {
                    OperationType opType = db.OperationTypes.SingleOrDefault(opt => opt.operationTypeID == optypeId);
                    if (opType != null)
                    {
                        //si achat
                        if (opType.operationTypeCode == CodeValue.Accounting.InitOperationType.CODEPURCHASE || opType.operationTypeCode == CodeValue.Accounting.InitOperationType.CODEPURCHASERETURN)
                        {
                            List<Account> listAccount = db.Accounts.Where(t => t.CollectifAccount.AccountingSection.AccountingSectionCode == CodeValue.Accounting.DefaultCodeAccountingSection.CODEESCPTEACHAT).ToList();
                            listAccount.ForEach(c =>
                            {
                                list.Add(
                                            new
                                            {
                                                AccountID = c.AccountID,
                                                AccountNumber = c.AccountNumber.ToString() + "-" + c.AccountLabel
                                            }
                                    );
                            });

                        }
                        //si vente
                        else if (opType.operationTypeCode == CodeValue.Accounting.InitOperationType.CODESALE || opType.operationTypeCode == CodeValue.Accounting.InitOperationType.CODESALERETURN)
                        {
                            List<Account> listAccount = db.Accounts.Where(t => t.CollectifAccount.AccountingSection.AccountingSectionCode == CodeValue.Accounting.DefaultCodeAccountingSection.CODEESCPTEVENTE).ToList();
                            listAccount.ForEach(c =>
                            {
                                list.Add(
                                            new
                                            {
                                                AccountID = c.AccountID,
                                                AccountNumber = c.AccountNumber.ToString() + "-" + c.AccountLabel
                                            }
                                        );
                            });
                        }
                    }
                }
            }
            return this.Store(list);
        }
        [HttpPost]
        public ActionResult GetTransPortAccount(string OperationID) //retourne le compte de transport
        {
            bool res = false;
            if (string.IsNullOrEmpty(OperationID))
            {
                return this.Direct();
            }
            List<object> list = new List<object>();
            this.GetCmp<ComboBox>("TransportAccountID").Clear();
            int id=Convert.ToInt32(OperationID);
            Operation operationEntity = db.Operations.SingleOrDefault(o => o.OperationID ==id );
            //determination de la macro operation
            int opMacroId = operationEntity.MacroOperationID;
            res = this.disablefieldReglement(opMacroId, "TransportAccountID");
            if (res == false)
            {
                //determinns le type d'operation
                int optypeId = operationEntity.OperationTypeID;
                if (optypeId > 0)
                {
                    OperationType opType = db.OperationTypes.SingleOrDefault(opt => opt.operationTypeID == optypeId);
                    if (opType != null)
                    {
                        //si achat
                        if (opType.operationTypeCode == CodeValue.Accounting.InitOperationType.CODEPURCHASE || opType.operationTypeCode == CodeValue.Accounting.InitOperationType.CODEPURCHASERETURN)
                        {
                            List<Account> listAccount = db.Accounts.Where(t => t.CollectifAccount.AccountingSection.AccountingSectionCode == CodeValue.Accounting.DefaultCodeAccountingSection.CODEPURCTRANSP).ToList();
                            listAccount.ForEach(c =>
                            {
                                list.Add(
                                            new
                                            {
                                                AccountID = c.AccountID,
                                                AccountNumber = c.AccountNumber.ToString() + "-" + c.AccountLabel
                                            }
                                    );
                            });

                        }
                        //si vente
                        else if (opType.operationTypeCode == CodeValue.Accounting.InitOperationType.CODESALE)
                        {
                            List<Account> listAccount = db.Accounts.Where(t => t.CollectifAccount.AccountingSection.AccountingSectionCode == CodeValue.Accounting.DefaultCodeAccountingSection.CODESALETRANSP).ToList();
                            listAccount.ForEach(c =>
                            {
                                list.Add(
                                            new
                                            {
                                                AccountID = c.AccountID,
                                                AccountNumber = c.AccountNumber.ToString() + "-" + c.AccountLabel
                                            }
                                        );
                            });
                        }
                        else if (opType.operationTypeCode == CodeValue.Accounting.InitOperationType.CODESALERETURN)
                        {
                            List<Account> listAccount = db.Accounts.Where(t => t.CollectifAccount.AccountingSection.AccountingSectionCode == CodeValue.Accounting.DefaultCodeAccountingSection.CODERETSALETRANSP).ToList();
                            listAccount.ForEach(c =>
                            {
                                list.Add(
                                            new
                                            {
                                                AccountID = c.AccountID,
                                                AccountNumber = c.AccountNumber.ToString() + "-" + c.AccountLabel
                                            }
                                        );
                            });
                        }
                    }
                }
            }
            return this.Store(list);
        }

        public bool disablefieldReglement(int opMacroId, string field)
        {
            bool res = false;
            this.GetCmp<ComboBox>(field).Clear();
            if (opMacroId > 0)
            {
                MacroOperation macroOp = db.MacroOperations.SingleOrDefault(mo => mo.MacroOperationID == opMacroId);
                if (macroOp != null)
                {
                    //si reglement il fo desactiver les objetc
                    if (macroOp.MacroOperationCode == CodeValue.Accounting.InitMacroOperation.CODEPAYMENT ||
                        macroOp.MacroOperationCode == CodeValue.Accounting.InitMacroOperation.CODEADVANCED)
                    {
                        this.GetCmp<ComboBox>(field).Disabled = true;
                        res = true;
                    }
                    else //sinon les desactiver
                    {
                        this.GetCmp<ComboBox>(field).Disabled = false;
                        res = false;
                    }
                }
            }
            return res;
        }
        public bool enabledfieldReglement(int opMacroId, string field)
        {
            bool res = false;
            this.GetCmp<ComboBox>(field).Clear();
            if (opMacroId > 0)
            {
                MacroOperation macroOp = db.MacroOperations.SingleOrDefault(mo => mo.MacroOperationID == opMacroId);
                if (macroOp != null)
                {
                    //si reglement il fo desactiver les objetc
                    if (macroOp.MacroOperationCode == CodeValue.Accounting.InitMacroOperation.CODEPAYMENT)
                    {
                        this.GetCmp<ComboBox>(field).Disabled = false;
                        res = true;
                    }
                    else //sinon les desactiver
                    {
                        this.GetCmp<ComboBox>(field).Disabled = true;
                        res = false;
                    }
                }
            }
            return res;
        }
        public ActionResult DesableValue(string ApplyVat)
        {
            try
            {
                if (ApplyVat == "NONE")
                {
                    this.GetCmp<ComboBox>("VatAccountID").Disabled = true;
                }
                else
                {
                    AccountingSection accountingSectionSelect = db.AccountingSections.SingleOrDefault(c => (c.AccountingSectionCode == ApplyVat));
                    if (accountingSectionSelect != null)
                    {
                        this.GetCmp<ComboBox>("VatAccountID").Disabled = false;
                    }
                    else this.GetCmp<ComboBox>("VatAccountID").Disabled = true;
                }
            }
            catch (Exception e)
            {
                statusOperation = Resources.er_alert_danger + e.Message;
                X.Msg.Alert(Resources.UIAccountingTask, statusOperation).Show();
                return this.Direct();
            }
            return this.Direct();
        }
        public ActionResult DesableAccount(string AccountingSectionID)
        {
            try
            {
                int id =Convert.ToInt32(AccountingSectionID);
                AccountingSection accountingSectionSelect = db.AccountingSections.SingleOrDefault(c => (c.AccountingSectionID == id ));
                if (accountingSectionSelect != null)
                {
                    if (accountingSectionSelect.AccountingSectionCode == CodeValue.Accounting.DefaultCodeAccountingSection.CODECLIENT || accountingSectionSelect.AccountingSectionCode == CodeValue.Accounting.DefaultCodeAccountingSection.CODEFOURN || accountingSectionSelect.AccountingSectionCode == CodeValue.Accounting.DefaultCodeAccountingSection.CODEPROD || accountingSectionSelect.AccountingSectionCode == CodeValue.Accounting.DefaultCodeAccountingSection.CODEBANK || accountingSectionSelect.AccountingSectionCode == CodeValue.Accounting.DefaultCodeAccountingSection.CODECAIS || accountingSectionSelect.AccountingSectionCode == CodeValue.Accounting.DefaultCodeAccountingSection.CODEPOSTCHECK)
                    {
                        this.GetCmp<ComboBox>("cbAccountID").Disabled = true;
                    }
                    else
                    {
                        this.GetCmp<ComboBox>("cbAccountID").Disabled = false;
                    }
                }
                else this.GetCmp<ComboBox>("cbAccountID").Disabled = false;
            }
            catch (Exception e)
            {
                statusOperation = Resources.er_alert_danger + e.Message;
                X.Msg.Alert(Resources.UIAccountingTask, statusOperation).Show();
                return this.Direct();
            }
            return this.Direct();
        }
        [HttpPost]
        public ActionResult GetAccountNumber(string AccountingSectionID)
        {
            try
            {
                int id = Convert.ToInt32(AccountingSectionID);
                List<Account> listAccount = db.Accounts.Where(t => t.CollectifAccount.AccountingSectionID == id).ToList();
                List<object> list = new List<object>();
                listAccount.ForEach(c =>
                {
                    list.Add(
                                                    new
                                                    {
                                                        AccountID = c.AccountID,
                                                        AccountNumber = c.AccountNumber.ToString() + "-" + c.AccountLabel
                                                    }
                                    );
                });
                return this.Store(list);

            }
            catch (Exception e)
            {
                statusOperation = Resources.er_alert_danger + e.Message;
                X.Msg.Alert(Resources.UIAccountingTask, statusOperation).Show();
                return this.Direct();
            }
        }
        [HttpPost]
        public ActionResult GetVATAccountNumber(string ApplyVat)
        {
            try
            {
                List<object> list = new List<object>();

                if (ApplyVat == "NONE")
                {
                }
                else
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
                                                                AccountID = c.AccountID,
                                                                AccountNumber = c.AccountNumber.ToString() + "-" + c.AccountLabel
                                                            }
                                            );
                        });
                    }
                }
                return this.Store(list);
            }
            catch (Exception e)
            {
                statusOperation = Resources.er_alert_danger + e.Message;
                X.Msg.Alert(Resources.UIAccountingTask, statusOperation).Show();
                return this.Direct();
            }
        }

        //***************Delete user
        [HttpPost]
        public ActionResult Delete(int ID)
        {
            try
            {
                int id = Convert.ToInt32(ID);
                AccountingTask accountingTaskToDelete = db.AccountingTasks.Find(id);
                db.AccountingTasks.Remove(accountingTaskToDelete);
                db.SaveChanges();
                //_accountingTaskRepository.Delete(accountingTaskToDelete);
                statusOperation = accountingTaskToDelete.AccountingTaskDescription + " : " + Resources.AlertDeleteAction;
                this.GetCmp<FormPanel>("AccountingTask").Reset();
                this.GetCmp<Store>("Store").Reload();
                this.AlertSucces(Resources.Success, statusOperation);
                return this.Direct();
                //
            }
            catch (Exception e)
            {
                statusOperation = Resources.er_alert_danger + e.Message;
                X.Msg.Alert(Resources.UIAccountingTask, statusOperation).Show();
                return this.Direct();
            }
        }


        //This action load a default view for not authorize user
        public ActionResult NotAuthorized()
        {
            return View();
        }
    }
}