using FatSod.DataContext.Concrete;
using FatSod.Security.Abstracts;
using FatSod.Security.Entities;
using CABOPMANAGEMENT.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

using System.Web.Security;
using FatSod.DataContext.Repositories;
using FatSod.DataContext.Initializer;

namespace CABOPMANAGEMENT.Controllers
{
    public class BaseController : Controller
    {
        protected EFDbContext db;

        //private EFDbContext context;
        protected string statusOperation;
        protected Thread thread;
        private IRepository<UserConfiguration> _UserConfigurationRepo;
        private IRepository<UserBranch> _userBranchRepository;
        private IRepository<Branch> _branchRepository;
        private IBusinessDay _busDayRepository;
        List<BusinessDay> bdDay;
        public BaseController(/*IRepository<UserConfiguration> ucConfRepo, IRepository<UserBranch> ubRepo, IRepository<Branch> branchRepository*/)
        {
            _UserConfigurationRepo = new Repository<UserConfiguration>();
            this._userBranchRepository = new Repository<UserBranch>();
            this._branchRepository = new Repository<Branch>();
            this._busDayRepository = new BusinessDayRepository();
            this.db = new EFDbContext();
        }
        // GET: Base
       
        //Cette méthode permet de déterminer la langue choisie par l'utilisateur
        public ActionResult SetCulture(string culture)
        {
            // Validate input
            culture = CultureHelper.GetImplementedCulture(culture);
            // Save culture in a cookie
            HttpCookie cookie = Request.Cookies["_culture"];
            if (cookie != null)
                cookie.Value = culture;   // update cookie value
            else
            {
                cookie = new HttpCookie("_culture");
                cookie.Value = culture;
                cookie.Expires = DateTime.Now.AddYears(1);
            }
            Response.Cookies.Add(cookie);

            CultureHelper.Changer_langue(culture);
            //string defaultController = (string)Session["Curent_Controller"];
            //string current_page = (string)Session["Curent_Page"];
            return RedirectToAction("Index", "Home");
        }
        //Fin de la méthode permettat de gérer la langue

        
        protected int SessionProfileID 
        {
            get
            {
                try
                {
                    return (int)Session["UserProfile"];
                }
                catch (Exception e)
                {
                    HttpCookie user_from_cookie = Request.Cookies["_userID"];
                    if (user_from_cookie != null)
                        return LoadComponent.getUser(Convert.ToInt32(user_from_cookie.Value)).ProfileID;
                    return 0;
                }
            }
            
        }
        protected int SessionGlobalPersonID
        {
            get
            {
                try
                {
                    return (int)Session["UserID"];
                }
                catch (Exception e)
                {
                    HttpCookie user_from_cookie = Request.Cookies["_userID"];
                    if (user_from_cookie != null)
                        return LoadComponent.getUser(Convert.ToInt32(user_from_cookie.Value)).GlobalPersonID;
                    return 0;
                }
            }

        }
        protected User CurrentUser
        {
            get
            {
                try
                {
                    User connectUser = (User)Session["CurrentUser"];
                    if (connectUser==null)
                    {
                        int Id = Convert.ToInt32(HttpContext.User.Identity.Name);
                        connectUser = LoadComponent.getUser(Id);
                    }
                    return connectUser;
                }
                catch (Exception e)
                {
                    HttpCookie user_from_cookie = Request.Cookies["_userID"];
                    if (user_from_cookie != null)
                        return LoadComponent.getUser(Convert.ToInt32(user_from_cookie.Value));
                    return null;
                }
            }

        }

        protected Branch CurrentBranch {
            get
            {
                BusinessDay bDay = SessionBusinessDay(null);
                if (bDay != null)
                {
                    return bDay.Branch;
                }
                return null;
            }    
        }

        
        protected BusinessDay SessionBusinessDay(int? BranchID)
        {
                BusinessDay busday = new BusinessDay();
                try
                {
                    List<BusinessDay> lstbudDay = (List<BusinessDay>)Session["UserBusDays"];
                    if (lstbudDay == null)
                    {
                        lstbudDay = _busDayRepository.GetOpenedBusinessDay(CurrentUser);
                    }
                    if (BranchID!=null || BranchID>0)
                    {
                        busday = lstbudDay.FirstOrDefault(b => b.BranchID == BranchID);
                    }
                    else
                    {
                        busday = lstbudDay.FirstOrDefault();
                    }
                    return busday;
                }
                catch (Exception e)
                {
                    HttpCookie user_from_cookie = Request.Cookies["_userID"];
                    if (user_from_cookie != null)
                    {
                        bdDay = _busDayRepository.GetOpenedBusinessDay(CurrentUser);
                        return (bdDay==null) ? null : bdDay.FirstOrDefault();
                    }
                    return null;
                }
        }
        //this methode provide a alert message to notify a user about her operation

        public void InjectUserConfigInSession()
        {

            //Les valeurs par défaut afin de réduire les clicques sur l'interface de commande
            UserConfiguration currentUserConfig = CurrentUser.UserConfiguration;

            if (currentUserConfig == null || currentUserConfig.UserConfigurationID <= 0)
            {
                //On prend les config de l'une de ses branches
                bdDay = (List<BusinessDay>)Session["UserBusDays"];
                if (bdDay == null)
                {
                    bdDay = _busDayRepository.GetOpenedBusinessDay(CurrentUser);
                }
                //Première branche de l'utilisateur qui est ouverte
                int userBranch = (bdDay.Count == 0) ? db.UserBranches.Where(u => u.UserID == CurrentUser.GlobalPersonID).FirstOrDefault().BranchID : bdDay.FirstOrDefault().Branch.BranchID;// _busDayRepository.GetOpenedBusinessDay(CurrentUser).FirstOrDefault().Branch;
                currentUserConfig = _UserConfigurationRepo.FindAll.Where(uc => uc.DefaultBranch.BranchID == userBranch/*.BranchID*/).FirstOrDefault();
                //Pour les branches crées en live
                if (currentUserConfig == null || currentUserConfig.UserConfigurationID <= 0)
                {
                    //Branch defaultBranch = _busDayRepository.GetOpenedBusinessDay().FirstOrDefault(/*br => br.Branch.BranchName == CodeValue.Parameter.Branch.YdeBranchName*/).Branch;
                    currentUserConfig = _UserConfigurationRepo.FindAll.FirstOrDefault(uc => uc.DefaultBranchID == userBranch/*.BranchID  defaultBranch.BranchID*/);
                }
            }

            Session["DefaultBranchID"] = currentUserConfig.DefaultBranchID;
            Session["DefaultDeviseID"] = currentUserConfig.DefaultDeviseID;
            Session["DefaultLocationID"] = currentUserConfig.DefaultLocationID;
            Session["isStockControl"] = currentUserConfig.isStockControl;
            Session["isDownloadRpt"] = currentUserConfig.isDownloadRpt;
            Session["isLimitAmountControl"] = currentUserConfig.isLimitAmountControl;
            Session["isTellerControl"] = currentUserConfig.isTellerControl;
            
        }

        protected JsonResult AlertSucces(string content)
        {
            return Json(content, JsonRequestBehavior.AllowGet);
        }
        protected JsonResult AlertError(string content)
        {
            return Json(content, JsonRequestBehavior.AllowGet);
        }

        protected ActionResult NotAuthorized()
        {
            return RedirectToAction("NotAuthorized", "User");
        }

        public List<int> GetConnectedUserIds()
        {
            List<int> res = (List<int>)HttpContext.Application["ConnectedUserIds"];
            return res;
        }

        public void DisconnectUser(int id)
        {
            System.Web.HttpContext.Current.Application.Lock();
            List<int> res = (List<int>)HttpContext.Application["ConnectedUserIds"];
            res.Remove(id);
            System.Web.HttpContext.Current.Application["ConnectedUserIds"] = res;
            System.Web.HttpContext.Current.Application.UnLock();

            System.Web.HttpContext.Current.Session.Clear();

        }

        public Boolean IsUserConnected(int id)
        {
            bool res = false;

            List<int> connectedUser = GetConnectedUserIds();
            if (connectedUser != null && connectedUser.Count!=0) 
                res = connectedUser.Contains(id);

            return res;
        }

        public void ConnectUser(int id)
        {
            List<int> res = (List<int>)System.Web.HttpContext.Current.Application["ConnectedUserIds"];

            if (res == null)
            {
                res = new List<int>();

                System.Web.HttpContext.Current.Application.Lock();

            }

            if (!res.Contains(id))
            {
                res.Add(id);

                System.Web.HttpContext.Current.Application["ConnectedUserIds"] = res;
                System.Web.HttpContext.Current.Application.UnLock();

                System.Web.HttpContext.Current.Session["ConnectedUserID"] = id;
            }

        }


        public bool IsTransactionIdentifierNew(string transactionIdentifier, int paymentMethodId)
        {
            var isTransactionIdentifierUsed = db.DigitalPaymentSales.Any(dps => dps.TransactionIdentifier == transactionIdentifier && 
                                              dps.DigitalPaymentMethodId == paymentMethodId);
            if (isTransactionIdentifierUsed)
                return isTransactionIdentifierUsed;

            isTransactionIdentifierUsed = db.AllDeposits.Any(d => d.TransactionIdentifier == transactionIdentifier &&
                                              d.PaymentMethodID == paymentMethodId);

            if (isTransactionIdentifierUsed)
                return isTransactionIdentifierUsed;

            isTransactionIdentifierUsed = db.Deposits.Any(d => d.TransactionIdentifier == transactionIdentifier &&
                                              d.PaymentMethodID == paymentMethodId);

            if (isTransactionIdentifierUsed)
                return isTransactionIdentifierUsed;

            isTransactionIdentifierUsed = db.Slices.Any(s => s.TransactionIdentifier == transactionIdentifier &&
                                              s.PaymentMethodID == paymentMethodId);

            return isTransactionIdentifierUsed;
        }

    }
}