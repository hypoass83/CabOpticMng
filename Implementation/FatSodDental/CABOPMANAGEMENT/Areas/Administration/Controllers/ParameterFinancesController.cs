
using CABOPMANAGEMENT.Controllers;
using System.Web.Mvc;
using FatSod.Security.Entities;
using CABOPMANAGEMENT.Tools;
using System;
using System.Linq;
using FatSod.DataContext.Initializer;
using FatSod.Ressources;
using FatSod.Supply.Entities;
using System.Collections.Generic;

namespace CABOPMANAGEMENT.Areas.Administration.Controllers
{
    public partial class ParameterController : BaseController
    {
        //static bool clickUpdate = false;
        //Allow to go to Till view management
        //[OutputCache(Duration = 3600)] 
        public ActionResult Till()
        {
            
            if (LoadComponent.Accounts(CodeValue.Accounting.DefaultCodeAccountingSection.CODECAIS).Count() == 0)
            {
                TempData["Message"] = "Please you must create an Till account<br/> Go to <code>Accountig=>GL or contact our administrator for this purpose<code/>.";
                return View();
            }
            //clickUpdate = false;
            Session["UserTillID"] = null;
            return View(ModelTill); 
        }
        
        //Allow to go to Bank view management
        //[OutputCache(Duration = 3600)] 
        public ActionResult Bank()
        {
            
            if (LoadComponent.Accounts(CodeValue.Accounting.DefaultCodeAccountingSection.CODEBANK).Count() == 0)
            {
                TempData["Message"] = "Please you must create an Bank account<br/> Go to <code>Accountig=>GL or contact our administrator for this purpose<code/>.";
                return View();
            }
            
            //clickUpdate = false;
            Session["UserTillID"] = null;
            return View(ModelBank);
            
        }
        public JsonResult populateUserBranch()
        {
            //holds list of UserBranches 
            List<object> _UserBranches = new List<object>();

            //queries all the Branches for ID and Name property.
            var Branches = (from s in db.Branches
                            select new { s.BranchID, s.BranchName, s.BranchCode }).ToList();

            //queries all the UserBranches for ID and Name property.
            int id = Convert.ToInt32(Session["UserID"]) + 0;
            var UserBranches = (from s in db.UserBranches
                                where s.UserID.Equals(id)
                                select new { s.BranchID }).ToList();


            //save list of UserBranches to the _UserBranches
            foreach (var item in Branches.OrderBy(i => i.BranchCode))
            {
                foreach (var ub in UserBranches)
                {
                    if (item.BranchID == ub.BranchID)
                    {
                        _UserBranches.Add(new
                        {
                            ID = item.BranchID,
                            Name = item.BranchCode + " " + item.BranchName
                        });
                    }
                }
            }
            //returns the Json result of _UserBranches
            return Json(_UserBranches, JsonRequestBehavior.AllowGet);
        }
        /**
         * populate user combo
         * */
        public JsonResult populateUsers()
        {
            
                List<object> userList = new List<object>();
                foreach (User user in db.People.OfType<User>().Where(u => u.IsConnected && u.UserAccessLevel > 0).ToArray())
                {
                    userList.Add(new
                    {
                        ID = user.GlobalPersonID,
                        Name = user.UserFullName
                    });
                }
            return Json(userList, JsonRequestBehavior.AllowGet);
        }
        /**
        * List of Countries to go Inside Select Tab
        **/
        public JsonResult populateBankAccountID()
        {
            //holds list of Countries 
            List<object> _Accounts = new List<object>();

            //queries all the Countries for its ID and Name property.
            var Accounts = (from s in db.Accounts where s.CollectifAccount.AccountingSection.AccountingSectionCode == CodeValue.Accounting.DefaultCodeAccountingSection.CODEBANK
                            select new { s.AccountID, s.AccountLabel, s.AccountNumber }).ToList();

            //save list of Countries to the _Countries
            foreach (var item in Accounts.OrderBy(i => i.AccountNumber))
            {
                _Accounts.Add(new
                {
                    ID = item.AccountID,
                    Name = item.AccountNumber + " " + item.AccountLabel
                });
            }
            //returns the Json result of _Countries
            return Json(_Accounts, JsonRequestBehavior.AllowGet);
        }
        /**
        * List of Countries to go Inside Select Tab
        **/
        public JsonResult populateTillAccountID()
        {
            //holds list of Countries 
            List<object> _Accounts = new List<object>();

            //queries all the Countries for its ID and Name property.
            var Accounts = (from s in db.Accounts
                            where s.CollectifAccount.AccountingSection.AccountingSectionCode == CodeValue.Accounting.DefaultCodeAccountingSection.CODECAIS
                            select new { s.AccountID, s.AccountLabel, s.AccountNumber }).ToList();

            //save list of Countries to the _Countries
            foreach (var item in Accounts.OrderBy(i => i.AccountNumber))
            {
                _Accounts.Add(new
                {
                    ID = item.AccountID,
                    Name = item.AccountNumber + " " + item.AccountLabel
                });
            }
            //returns the Json result of _Countries
            return Json(_Accounts, JsonRequestBehavior.AllowGet);
        }
        //************ Add method AddBank; AddTill and AddOtherPaymentMethod
        [HttpPost]
        public JsonResult AddBank(Bank bank)
        {
            bool status = false;
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
               
                //this.ResetTillForm();
                status = true;
            }
            catch (Exception e) {
                status = false;
                statusOperation = "Error " + e.Message + " " + e.InnerException;
            }
            return new JsonResult { Data = new { status = status, Message = statusOperation } };
        }

        public JsonResult AddTill(Till till, int CreatIdiatly = 0)
        {
            //We verify if user has choiced to assign immediatly an user for manager this till
            bool status = false;
            bool userHasAssignment = false;
            //DateTime veilledateop = new DateTime(1900,1,1);

            userHasAssignment = Convert.ToBoolean(CreatIdiatly);
            BusinessDay bussday = _busDay.GetOpenedBusinessDay().FirstOrDefault();
            if (till.ID > 0)
            {
                UserTill currentCashier = _userTillRepository.FindAll.SingleOrDefault(ut => ut.TillID == till.ID && ut.HasAccess);
                int oldUserID = (currentCashier != null && currentCashier.UserTillID > 0) ? currentCashier.UserID : 0;

                DateTime bd = bussday.BDDateOperation;

                DateTime actualTime = DateTime.Now;
                bd = bd.AddHours(actualTime.Hour);
                bd = bd.AddMinutes(actualTime.Minute);
                bd = bd.AddSeconds(actualTime.Second);

                int newUserID = 0;
                try
                {
                    //Récuperation de l'id du caissier
                    newUserID = Convert.ToInt32(Request.Form["UserID"]);

                    //On ne doit pas modifier une caisse qui est ouverte
                    if (_tillDayRepo.IsTillOpened(till, bd))
                    {
                        statusOperation = "No Such Action in an Opened Till - To Update a Cash Register, You Must Close it Before";
                        return new JsonResult { Data = new { status = status, Message = statusOperation } };
                    }
                }
                catch (Exception e)
                {
                    statusOperation = "You has choose to assigndirectly an user this till. Please try again to select one user : " + e.Message;
                    return new JsonResult { Data = new { status = status, Message = statusOperation } };
                }

                if (userHasAssignment)
                {



                    //A partir d'ici, newUserID > 0


                    if (oldUserID > 0)
                    {
                        if (oldUserID != newUserID) //Il y a remplacement
                        {
                            /*UserTill newUserTill = new UserTill()
                            {
                                HasAccess = true,
                                TillID = till.ID,
                                UserID = newUserID,
                                UserTillDateAssignment = bd,
                            };*/
                            currentCashier.UserID = newUserID;
                            currentCashier.UserTillDateAssignment = bd;
                            currentCashier.TillID = till.ID;

                            try
                            {
                                _userTillRepository.UpdateUserTill(till, currentCashier, SessionGlobalPersonID, bussday.BranchID, bussday.BDDateOperation);
                            }
                            catch (Exception e)
                            {
                                statusOperation = e.Message;
                                return new JsonResult { Data = new { status = status, Message = statusOperation } };
                            }
                        }

                    }
                    else
                    {
                        UserTill newUserTill = new UserTill()
                        {
                            HasAccess = true,
                            TillID = till.ID,
                            UserID = newUserID,
                            UserTillDateAssignment = bd,
                        };

                        try
                        {
                            _userTillRepository.UpdateUserTill(till, newUserTill, SessionGlobalPersonID, bussday.BranchID, bussday.BDDateOperation);
                        }
                        catch (Exception e)
                        {
                            statusOperation = e.Message;
                            return new JsonResult { Data = new { status = status, Message = statusOperation } };
                        }
                    }


                }
                else
                {
                    if (oldUserID > 0)
                    {
                        currentCashier.HasAccess = false;
                        currentCashier.UserTillDisAssignDate = bd;
                        // _userTillRepository.Update(currentCashier, currentCashier.UserTillID);
                    }
                    //Mise à jour effective de la caisse
                    if (currentCashier == null)
                    {
                        currentCashier = new UserTill()
                        {
                            HasAccess = true,
                            TillID = till.ID,
                            UserID = newUserID,
                            UserTillDateAssignment = bd,
                        };
                    }
                    try
                    {
                        _userTillRepository.UpdateUserTill(till, currentCashier, SessionGlobalPersonID, bussday.BranchID, bussday.BDDateOperation);
                    }
                    catch (Exception e)
                    {
                        statusOperation = e.Message;
                        return new JsonResult { Data = new { status = status, Message = statusOperation } };
                    }
                }


                //int tillID = _paymentMethodRepository.Update(till, till.ID).ID;
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
                        statusOperation = "Error in till - You has choose to assigndirectly an user this till. Please try again to select one user : " + e.Message;
                        return new JsonResult { Data = new { status = status, Message = statusOperation } };
                    }

                    //veilledateop = bussday.BDDateOperation.AddDays(-1);
                    //int tillID = _paymentMethodRepository.Create(till).ID;
                    UserTill userTillToSave = new UserTill()
                    {
                        //TillID = tillID,
                        UserTillDateAssignment = bussday.BDDateOperation,
                        HasAccess = true,
                        UserID = user
                    };
                    try
                    {
                        _userTillRepository.CreateUserTill(till, userTillToSave, SessionGlobalPersonID, bussday.BranchID, bussday.BDDateOperation);
                    }
                    catch (Exception e)
                    {
                        statusOperation = e.Message;
                        return new JsonResult { Data = new { status = status, Message = statusOperation } };
                    }
                    statusOperation = "Till and assignment user add successfuly";
                }
                else
                {
                    _paymentMethodRepository.Create(till);
                    statusOperation = "Till add successfuly";
                }
            }

            status = true;
            //this.ResetTillForm();

            return new JsonResult { Data = new { status = status, Message = statusOperation } };

           
        }
       

        //Delete action 
        public JsonResult DeletePaymentMethod(int ID)
        {
            bool status = false;
            try
            {
                int id = Convert.ToInt32(ID);
                _paymentMethodRepository.RemovePaymentMethod(id);
                status = true;
                statusOperation = Resources.AlertDeleteAction;
            }
            catch (Exception e)
            {
                statusOperation = "An error occur when try to delete this payment method. It may be use by another entity <br/> : " + e.Message;
                status = false;
            }
            //this.ResetTillForm();
            return new JsonResult { Data = new { status = status, Message = statusOperation } };
        }
        
        public JsonResult InitializeFieldsBank(int ID)
        {
            //int id = Convert.ToInt32(ID);
            Bank BankToUpdate = _paymentMethodRepository.FindAll.OfType<Bank>().FirstOrDefault(p => p.ID == ID);
            List<object> _BankList = new List<object>();
            _BankList.Add(new
            {
                Name = BankToUpdate.Name,
                Code = BankToUpdate.Code,
                Description = BankToUpdate.Description,
                BranchID = BankToUpdate.BranchID,
                AccountID = BankToUpdate.AccountID,
                ID=BankToUpdate.ID
            });
            return Json(_BankList, JsonRequestBehavior.AllowGet);
        }
        public JsonResult InitializeFieldsTill(int ID)
        {

            Till TillToUpdate = _paymentMethodRepository.FindAll.OfType<Till>().FirstOrDefault(p => p.ID == ID);
            UserTill userTill = _userTillRepository.FindAll.FirstOrDefault(ut => ut.TillID == ID && ut.HasAccess);
            List<object> _TillList = new List<object>();
            _TillList.Add(new
            {
                Name = TillToUpdate.Name,
                Code = TillToUpdate.Code,
                Description = TillToUpdate.Description,
                BranchID = TillToUpdate.BranchID,
                AccountID = TillToUpdate.AccountID,
                ID = TillToUpdate.ID,
                UserID=(userTill == null)?0:userTill.UserID,
                CreatIdiatly = (userTill == null) ? 0 : 1
            });
            return Json(_TillList, JsonRequestBehavior.AllowGet);
            
        }
        //return all user of one branch
        //public StoreResult UserForTill(int BranchID)
        //{
        //    int UserTillID=0;
        //    if (Session["UserTillID"]!=null) UserTillID=(int)Session["UserTillID"];

        //    List<UserBranch> lsUserBr = new List<UserBranch>();
        //    List<object> model = new List<object>();
        //    if (UserTillID>0)
        //    {
        //        lsUserBr = _userBranchRepository.FindAll.Where(ub => ub.BranchID == BranchID && ub.UserID==UserTillID).ToList();
        //    }
        //    else
        //    {
        //        lsUserBr = _userBranchRepository.FindAll.Where(ub => ub.BranchID == BranchID).ToList();
        //    }

        //    lsUserBr.ForEach(r =>
        //    {
        //        //si click sur initialization des champs
        //        if (clickUpdate)
        //        {
        //            UserTill usertil = _userTillRepository.FindAll.FirstOrDefault(ut => ut.UserID == r.UserID && ut.HasAccess);
        //            if (usertil != null)
        //            {
        //                if (r.User.UserAccessLevel < 5)
        //                {
        //                    model.Add(
        //                        new
        //                        {
        //                            GlobalPersonID = r.UserID,
        //                            Name = r.User.Name + " " + r.User.Description + " [" + r.User.ProfileLabel + "]"
        //                        }
        //                    );
        //                }
        //            }
        //            else
        //            {
        //                if (r.User.UserAccessLevel < 5)
        //                {
        //                    model.Add(
        //                        new
        //                        {
        //                            GlobalPersonID = r.UserID,
        //                            Name = r.User.Name + " " + r.User.Description + " [" + r.User.ProfileLabel + "]"
        //                        }
        //                    );
        //                }
        //            }
        //        }
        //        else //si nvlle attribution affiche la liste des caissier pas encore use
        //    {
        //            UserTill usertil = _userTillRepository.FindAll.FirstOrDefault(ut => ut.UserID == r.UserID && ut.HasAccess);
        //            if (usertil == null)
        //        {
        //            if (r.User.UserAccessLevel < 5)
        //            {
        //                model.Add(
        //                    new
        //                    {
        //                        GlobalPersonID = r.UserID,
        //                        Name = r.User.Name + " " + r.User.Description + " [" + r.User.ProfileLabel + "]"
        //                    }
        //                );
        //            }
        //        }
        //        }

        //    });
        //    return this.Store(model);
        //}
        //private Company CompanyTo
        //{
        //    get
        //    {
        //        return _companyRepository.FindAll.FirstOrDefault();
        //    }
        //}

    }
}