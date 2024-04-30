using Ext.Net;
using Ext.Net.MVC;
using FatSodDental.UI.Controllers;
using System.Web.Mvc;
using FatSod.Security.Abstracts;
using FatSod.Security.Entities;
using FatSodDental.UI.Tools;
using System;
using System.Linq;
using System.Web;
using FatSod.DataContext.Initializer;
using FatSod.Ressources;
using FatSod.Supply.Entities;
using System.Collections.Generic;
using CrystalDecisions.CrystalReports.Engine;
using System.IO;
using FatSodDental.UI.Filters;
using System.Text;
using System.Web.UI;

namespace FatSodDental.UI.Areas.Administration.Controllers
{
    public partial class ParameterController : BaseController
    {
        static bool clickUpdate = false;
        //Allow to go to Till view management
        //[OutputCache(Duration = 3600)] 
        public ActionResult Till()
        {

            //Ext.Net.MVC.PartialViewResult rPVResult = new Ext.Net.MVC.PartialViewResult
            //{
            //    ClearContainer = true,
            //    //RenderMode = RenderMode.AddTo,
            //    ContainerId = "Body",
            //    //WrapByScriptTag = false,
            //    Model = ModelTill
            //};
            
            //if (!LoadAction.isAutoriseToGetSubMenu(SessionProfileID, CodeValue.Parameter.MoneyManagement.TillCODE, db))
            //{
            //    RedirectToAction("NotAuthorized", "User");
            //}
            if (LoadComponent.Accounts(CodeValue.Accounting.DefaultCodeAccountingSection.CODECAIS).Count() == 0)
            {
                X.Msg.Alert("Error in till", "Please you must create an account<br/> Go to <code>Accountig=>GL or contact our administrator for this purpose<code/>.").Show();
                return this.Direct();
            }
            clickUpdate = false;
            Session["UserTillID"] = null;
            return View(ModelTill); 
        }
        //Allow to go to OtherPaymentMethod view management
        //[OutputCache(Duration = 3600)] 
        public ActionResult OtherPaymentMethod()
        {
            //Ext.Net.MVC.PartialViewResult rPVResult = new Ext.Net.MVC.PartialViewResult
            //{
            //    ClearContainer = true,
            //    //RenderMode = RenderMode.AddTo,
            //    ContainerId = "Body",
            //    //WrapByScriptTag = false,
            //    Model = ModelOtherPaye
            //};
            //if (!LoadAction.isAutoriseToGetSubMenu(SessionProfileID, CodeValue.Parameter.MoneyManagement.OTCODE, db))
            //{
            //    RedirectToAction("NotAuthorized", "User");
            //}
            if (LoadComponent.Accounts(CodeValue.Accounting.DefaultCodeAccountingSection.CODEPOSTCHECK).Count() == 0)
            {
                X.Msg.Alert("Error in till", "Please you must create an account<br/> Go to <code>Accountig=>GL or contact our administrator for this purpose<code/>.").Show();
                return this.Direct();
            }
            //Session["Curent_Page"] = "OtherPaymentMethod";
            //Session["Curent_Controller"] = CONTROLLER_NAME;

            clickUpdate = false;
            Session["UserTillID"] = null;
            //return rPVResult;

            return View(ModelOtherPaye);
        }
        private List<object> ModelOtherPaye
        {
            get
            {
                List<object> model = new List<object>();
                db.SavingAccounts.ToList().ForEach(item =>
                {
                    model.Add(
                            new
                            {
                                ID = item.ID,
                                Name = item.Name,
                                Description = item.Description,
                                Code = item.Code,
                                AccountID = item.Customer.Account != null ? item.Customer.Account.AccountNumber : item.Customer.AccountID,
                                BranchID = item.Branch != null ? item.Branch.BranchName : "Not Ready"
                            }
                        );
                });
                return model;
            }

        }
        //Allow to go to Bank view management
        //[OutputCache(Duration = 3600)] 
        public ActionResult Bank()
        {
            //Ext.Net.MVC.PartialViewResult rPVResult = new Ext.Net.MVC.PartialViewResult
            //{
            //    ClearContainer = true,
            //    //RenderMode = RenderMode.AddTo,
            //    ContainerId = "Body",
            //    //WrapByScriptTag = false,
            //    Model = ModelBank
            //};

            //if (!LoadAction.isAutoriseToGetSubMenu(SessionProfileID, CodeValue.Parameter.MoneyManagement.BankCODE, db))
            //{
            //    RedirectToAction("NotAuthorized", "User");
            //}
            if (LoadComponent.Accounts(CodeValue.Accounting.DefaultCodeAccountingSection.CODEBANK).Count() == 0)
            {
                X.Msg.Alert("Error in till", "Please you must create an account<br/> Go to <code>Accountig=>GL or contact our administrator for this purpose<code/>.").Show();
                return this.Direct();
            }
            
            clickUpdate = false;
            Session["UserTillID"] = null;
            return View(ModelBank);
            
        }
        //************ Add method AddBank; AddTill and AddOtherPaymentMethod
        [HttpPost]
        public ActionResult AddBank(Bank bank)
        {
            try
            {
                if (bank.ID > 0)
                {
                    statusOperation = Resources.AlertUpdateAction;
                    _paymentMethodRepository.Update(bank, bank.ID);
                }
                else
                {
                    statusOperation = Resources.AlertAddAction;
                    _paymentMethodRepository.Create(bank);
                }
                this.AlertSucces(Resources.Success, statusOperation);
                this.ResetTillForm();
                return this.Direct();
            }
            catch (Exception e) { X.Msg.Alert("Error ", e.Message).Show(); return this.Direct(); }
        }

        public ActionResult AddTill(Till till, int CreatIdiatly = 0)
        {
            //We verify if user has choiced to assign immediatly an user for manager this till
            bool userHasAssignment = false;
            userHasAssignment = Convert.ToBoolean(CreatIdiatly);
            if (till.ID > 0)
            {
                UserTill currentCashier = _userTillRepository.FindAll.SingleOrDefault(ut => ut.TillID == till.ID && ut.HasAccess);
                int oldUserID = (currentCashier != null && currentCashier.UserTillID > 0) ? currentCashier.UserID : 0;

                DateTime bd = _busDay.GetOpenedBusinessDay().FirstOrDefault().BDDateOperation;

                DateTime actualTime = DateTime.Now;
                bd = bd.AddHours(actualTime.Hour);
                bd = bd.AddMinutes(actualTime.Minute);
                bd = bd.AddSeconds(actualTime.Second);

                if (userHasAssignment)
                {
                    
                    int newUserID = 0;
                    try
                    {
                        //Récuperation de l'id du caissier
                        newUserID = Convert.ToInt32(Request.Form["UserID"]);
                    }
                    catch (Exception e)
                    {
                        X.Msg.Alert("Error in till", "You has choose to assigndirectly an user this till. Please try again to select one user : " + e.Message).Show();
                        return this.Direct();
                    }

                    
                    //On ne doit pas modifier une caisse qui est ouverte
                    if (_tillDayRepo.IsTillOpened(till, bd))
                    {
                        X.Msg.Alert("No Such Action in an Opened Till", "To Update a Cash Register, You Must Clos it Before").Show();
                        return this.Direct();
                    }

                    //A partir d'ici, newUserID > 0
                    
                    

                    if (oldUserID == 0)
                    {
                        UserTill newUserTill = new UserTill()
                        {
                            HasAccess = true,
                            TillID = till.ID,
                            UserID = newUserID,
                            UserTillDateAssignment = bd,
                        };

                        _userTillRepository.Create(newUserTill);

                    }

                    if (oldUserID > 0)
                    {
                        if (oldUserID != newUserID) //Il y a remplacement
                        {
                            UserTill newUserTill = new UserTill()
                            {
                                HasAccess = true,
                                TillID = till.ID,
                                UserID = newUserID,
                                UserTillDateAssignment = bd,
                            };

                            _userTillRepository.Create(newUserTill);

                            currentCashier.HasAccess = false;
                            currentCashier.UserTillDisAssignDate = bd;
                            _userTillRepository.Update(currentCashier, currentCashier.UserTillID);

                        }

                    }

                    
                }
                else
                {
                    if (oldUserID > 0)
                    {
                        currentCashier.HasAccess = false;
                        currentCashier.UserTillDisAssignDate = bd;
                        _userTillRepository.Update(currentCashier, currentCashier.UserTillID);
                    }

                }

                //Mise à jour effective de la caisse
                int tillID = _paymentMethodRepository.Update(till, till.ID).ID;
                statusOperation = "Till Was successfuly Modified";

            }//End off update
            else
            {
                //User want to add a new till
                if (userHasAssignment)
                {
                    int user = 0;
                    try
                    {
                        user = Convert.ToInt32(Request.Form["UserID"]);
                    }
                    catch (Exception e)
                    {
                        X.Msg.Alert("Error in till", "You has choose to assigndirectly an user this till. Please try again to select one user : " + e.Message).Show();
                        return this.Direct();
                    }

                    int tillID = _paymentMethodRepository.Create(till).ID;
                    UserTill userTillToSave = new UserTill()
                    {
                        TillID = tillID,
                        UserTillDateAssignment = DateTime.Now,
                        HasAccess = true,
                        UserID = user
                    };
                    _userTillRepository.Create(userTillToSave);
                    statusOperation = "Till and assignment user add successfuly";
                }
                else
                {
                    _paymentMethodRepository.Create(till);
                    statusOperation = "Till add successfuly";
                }
            }

            this.AlertSucces(Resources.Success, statusOperation);
            this.ResetTillForm();
            
            return this.Direct();
        }
        public ActionResult AddOtherPaymentMethod(SavingAccount otherPaymentMethod)
        {
            try
            {
                if (otherPaymentMethod.ID > 0)
                {
                    statusOperation = Resources.AlertUpdateAction;
                    _paymentMethodRepository.Update(otherPaymentMethod, otherPaymentMethod.ID);
                }
                else
                {
                    statusOperation = Resources.AlertAddAction;
                    _paymentMethodRepository.Create(otherPaymentMethod);
                }

                this.AlertSucces(Resources.Success, statusOperation);
                this.ResetTillForm();
                
                return this.Direct();
            }
            catch (Exception e) { X.Msg.Alert("Error ", e.Message).Show(); return this.Direct(); }
        }
        //This actions are methods that proxy use to load datas
        public StoreResult GetTillsList()
        {
            return this.Store(ModelTill);
        }
        public StoreResult GetBanksList()
        {
            return this.Store(ModelBank);
        }
        public StoreResult GetOtherPaymentMethodsList()
        {
            return this.Store(
                new Parameter("data", JSON.Serialize(_paymentMethodRepository.FindAll.OfType<SavingAccount>()), ParameterMode.Auto)
                );
        }
        //Delete action 
        public ActionResult DeletePaymentMethod(int ID)
        {
            try
            {
                statusOperation = Resources.AlertDeleteAction;
                int id = Convert.ToInt32(ID);
                //PaymentMethod paymentMethodToDelete = _paymentMethodRepository.FindAll.FirstOrDefault(p => p.ID == id);
                _paymentMethodRepository.RemovePaymentMethod(id);
            }
            catch (Exception e)
            {
                statusOperation = "An error occur when try to delete this payment method. It may be use by another entity <br/> : " + e.Message;
                this.AlertSucces(Resources.Success, statusOperation);
                this.AlertSucces(Resources.Success, statusOperation);
                return this.Direct();
            }
            statusOperation = Resources.AlertDeleteAction;
            this.AlertSucces(Resources.Success, statusOperation);
            this.ResetTillForm();
            
            return this.Direct();
        }
        public ActionResult ResetTillForm()
        {
            this.GetCmp<FormPanel>("PaymentMethodForm").Reset();
            this.GetCmp<Store>("PaymentMethodFormStore").Reload();
            this.GetCmp<TextField>("CreateImediatlyUser").Value = 0;
            this.GetCmp<Radio>("CreatIdiatly").Checked = false;
            this.GetCmp<Radio>("CreatIdiatlyNO").Checked = true;
            this.GetCmp<ComboBox>("UserID").Hidden = true;
            clickUpdate = false;
            Session["UserTillID"] = null;
            return this.Direct();
        }
        public ActionResult InitializeFieldsPaymentMethod(int ID)
        {
            try
            {
                clickUpdate = true;
                statusOperation = Resources.AlertDeleteAction;
                PaymentMethod paymentToUpdate = _paymentMethodRepository.Find(ID);
                this.GetCmp<TextField>("ID").Value = paymentToUpdate.ID;
                this.GetCmp<TextField>("Name").Value = paymentToUpdate.Name;
                this.GetCmp<TextField>("Code").Value = paymentToUpdate.Code;
                this.GetCmp<TextField>("Description").Value = paymentToUpdate.Description;
                this.GetCmp<ComboBox>("BranchID").SetValueAndFireSelect(paymentToUpdate.BranchID);
                if ( paymentToUpdate is Bank)
                    this.GetCmp<ComboBox>("AccountID").SetValueAndFireSelect(((Bank)paymentToUpdate).AccountID);
                if (paymentToUpdate is Till)
                    this.GetCmp<ComboBox>("AccountID").SetValueAndFireSelect(((Till)paymentToUpdate).AccountID);
                if (paymentToUpdate is SavingAccount)
                    this.GetCmp<ComboBox>("AccountID").SetValueAndFireSelect(((SavingAccount)paymentToUpdate).Customer.AccountID);

                //if (this.GetCmp<Radio>("CreatIdiatly") != null)
                //{
                int userTillID = 0;
                UserTill userTill = _userTillRepository.FindAll.FirstOrDefault(ut => ut.TillID == paymentToUpdate.ID && ut.HasAccess);
                if (userTill!=null)
                    {
                    userTillID = userTill.UserID;
                    Session["UserTillID"] = userTillID;
                        this.GetCmp<Radio>("CreatIdiatly").Checked = true;
                        this.GetCmp<Radio>("CreatIdiatlyNO").Checked = false;
                        this.GetCmp<ComboBox>("UserID").Hidden = false;
                    this.GetCmp<ComboBox>("UserID").SetValueAndFireSelect(userTillID);
                    this.GetCmp<TextField>("CreateImediatlyUser").Value = 0;
                    }
                    else
                    {
                    Session["UserTillID"] = null;
                        this.GetCmp<Radio>("CreatIdiatly").Checked = false;
                        this.GetCmp<Radio>("CreatIdiatlyNO").Checked = true;
                        this.GetCmp<ComboBox>("UserID").Hidden = true;
                    this.GetCmp<TextField>("CreateImediatlyUser").Value = 1;
                    }
                    
            }
            catch (Exception e)
            {
                statusOperation = "An error occur when try to initialize fields for update.<br/> : " + e.Message;
                return this.Direct();
            }
            return this.Direct();
        }
        //return all user of one branch
        public StoreResult UserForTill(int BranchID)
        {
            int UserTillID=0;
            if (Session["UserTillID"]!=null) UserTillID=(int)Session["UserTillID"];

            List<UserBranch> lsUserBr = new List<UserBranch>();
            List<object> model = new List<object>();
            if (UserTillID>0)
            {
                lsUserBr = _userBranchRepository.FindAll.Where(ub => ub.BranchID == BranchID && ub.UserID==UserTillID).ToList();
            }
            else
            {
                lsUserBr = _userBranchRepository.FindAll.Where(ub => ub.BranchID == BranchID).ToList();
            }
            
            lsUserBr.ForEach(r =>
            {
                //si click sur initialization des champs
                if (clickUpdate)
                {
                    UserTill usertil = _userTillRepository.FindAll.FirstOrDefault(ut => ut.UserID == r.UserID && ut.HasAccess);
                    if (usertil != null)
                    {
                        if (r.User.UserAccessLevel < 5)
                        {
                            model.Add(
                                new
                                {
                                    GlobalPersonID = r.UserID,
                                    Name = r.User.Name + " " + r.User.Description + " [" + r.User.ProfileLabel + "]"
                                }
                            );
                        }
                    }
                    else
                    {
                        if (r.User.UserAccessLevel < 5)
                        {
                            model.Add(
                                new
                                {
                                    GlobalPersonID = r.UserID,
                                    Name = r.User.Name + " " + r.User.Description + " [" + r.User.ProfileLabel + "]"
                                }
                            );
                        }
                    }
                }
                else //si nvlle attribution affiche la liste des caissier pas encore use
            {
                    UserTill usertil = _userTillRepository.FindAll.FirstOrDefault(ut => ut.UserID == r.UserID && ut.HasAccess);
                    if (usertil == null)
                {
                    if (r.User.UserAccessLevel < 5)
                    {
                        model.Add(
                            new
                            {
                                GlobalPersonID = r.UserID,
                                Name = r.User.Name + " " + r.User.Description + " [" + r.User.ProfileLabel + "]"
                            }
                        );
                    }
                }
                }
                
            });
            return this.Store(model);
        }
        private Company CompanyTo
        {
            get
            {
                return _companyRepository.FindAll.FirstOrDefault();
            }
        }

    }
}