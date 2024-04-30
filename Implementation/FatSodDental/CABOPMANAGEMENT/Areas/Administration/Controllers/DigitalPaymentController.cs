using FatSod.DataContext.Initializer;
using FatSod.Ressources;
using FatSod.Security.Abstracts;
using FatSod.Security.Entities;
using FatSod.Supply.Abstracts;
using FatSod.Supply.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CABOPMANAGEMENT.Controllers;
using CABOPMANAGEMENT.Tools;

namespace CABOPMANAGEMENT.Areas.Administration.Controllers
{
    public class DigitalPaymentController : BaseController
    {
        private readonly IPaymentMethod _paymentMethodRepository;
        private readonly IUserTill _userTillRepository;
        private readonly ITillDay _tillDayRepo;
        private readonly IBusinessDay _busDay;
        private readonly IDigitalPayment _digitalPaymentRepository;

        public DigitalPaymentController(IPaymentMethod paymentMethodRepository, IUserTill userTillRepository, ITillDay tillDayRepo, IBusinessDay busDay,
                                        IDigitalPayment digitalPaymentRepository)
        {
            this._paymentMethodRepository = paymentMethodRepository;
            this._userTillRepository = userTillRepository;
            this._tillDayRepo = tillDayRepo;
            this._busDay = busDay;
            this._digitalPaymentRepository = digitalPaymentRepository;
        }
        // GET: Administration/DigitalPaymentMethod
        public ActionResult Index()
        {
            if (LoadComponent.Accounts(CodeValue.Accounting.DefaultCodeAccountingSection.CODECAIS).Count() == 0)
            {
                TempData["Message"] = "Please you must create an Till account<br/> Go to <code>Accountig=>GL or contact our administrator for this purpose<code/>.";
                return View();
            }
            //clickUpdate = false;
            Session["UserTillID"] = null;
            return View(DigitalPaymentMethods);
        }

        private List<DigitalPaymentMethod> DigitalPaymentMethods
        {
            get
            {
                List<DigitalPaymentMethod> model = new List<DigitalPaymentMethod>();
                _digitalPaymentRepository.FindAll.ToList().ForEach(dpm =>
                {
                    model.Add(
                            new DigitalPaymentMethod
                            {
                                ID = dpm.ID,
                                Name = dpm.Name,
                                Description = dpm.Description,
                                Code = dpm.Code,
                                Account = (dpm.Account != null) ? dpm.Account : null,
                                AccountManager = (dpm.AccountManager != null) ? dpm.AccountManager : null,
                                Branch = (dpm.Branch != null) ? dpm.Branch : null
                            }
                        );
                });
                return model;
            }

        }

        public JsonResult AddDigitalPayment(DigitalPaymentMethod digitalPayment, int IsManagerDefined = 0)
        {
            var status = false;

            try
            {
                if (digitalPayment.ID > 0)
                {
                    _digitalPaymentRepository.Update(digitalPayment, digitalPayment.ID);
                    statusOperation = "SUCCESS - " + digitalPayment.Name + " (" + digitalPayment.Code + ") " + "Was successfuly Modified";
                }
                else
                {
                    _digitalPaymentRepository.Create(digitalPayment);
                    statusOperation = "SUCCESS - " + digitalPayment.Name + " (" + digitalPayment.Code + ") " + "Was successfuly Created";
                }
                status = true;
            }
            catch (Exception e)
            {
                statusOperation = "ERROR - " + e.Message;
            }

            return new JsonResult { Data = new { status = status, Message = statusOperation } };
        }

        public JsonResult DeletePaymentMethod(int ID)
        {
            bool status = false;
            try
            {
                _digitalPaymentRepository.Delete(ID);
                status = true;
                statusOperation = Resources.AlertDeleteAction;
            }
            catch (Exception e)
            {
                statusOperation = "An error occur when try to delete this payment method. It may be use by another entity <br/> : " + e.Message;
            }
            return new JsonResult { Data = new { status = status, Message = statusOperation } };
        }

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

        /// <summary>
        /// TODO - We should not be able to change DPM Name(Provider) and Code(Receiver Identifier) if it has already been use to do a sale
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public JsonResult InitializeFieldsTill(int ID)
        {
            var existingDPM = _digitalPaymentRepository.Find(ID);
            var model = new List<object>();
            model.Add(new
            {
                Name = existingDPM.Name,
                Code = existingDPM.Code,
                Description = existingDPM.Description,
                BranchID = existingDPM.BranchID,
                AccountID = existingDPM.AccountID,
                ID = existingDPM.ID,
                UserID = (existingDPM.AccountManager == null) ? 0 : existingDPM.AccountManagerId,
                IsManagerDefined = (existingDPM.AccountManager == null) ? 0 : 1
            });
            return Json(model, JsonRequestBehavior.AllowGet);
        }


    }
}