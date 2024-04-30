using FatSod.DataContext.Concrete;
using FatSod.Security.Abstracts;
using FatSod.Security.Entities;
using FatSodDental.UI.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using System.Web.Security;
using FatSod.DataContext.Repositories;
using FatSod.DataContext.Initializer;

namespace FatSodDental.UI.Controllers
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
       /* protected override IAsyncResult BeginExecuteCore(AsyncCallback callback, object state)
        {
            string controller = (string)Request.RequestContext.RouteData.Values["controller"];
            string action = (string)Request.RequestContext.RouteData.Values["action"];
            int i = 0;
            i++;
            FatSod.Security.Entities.User currentUser = this.CurrentUser;
            if (currentUser != null && SessionGlobalPersonID > 0)
            {
                Session["UserProfile"] = SessionProfileID;
                Session["UserID"] = SessionGlobalPersonID;
            }
            
            string cultureName = null;

            // Attempt to read the culture cookie from Request
            HttpCookie cultureCookie = Request.Cookies["_culture"];
            if (cultureCookie != null)
                cultureName = cultureCookie.Value;
            else
                cultureName = Request.UserLanguages != null && Request.UserLanguages.Length > 0 ?
                        Request.UserLanguages[0] :  // obtain it from HTTP header AcceptLanguages
                        null;
            // Validate culture name
            cultureName = CultureHelper.GetImplementedCulture(cultureName); // This is safe

            // Modify current thread's cultures            
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(cultureName);
            Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;
            
            return base.BeginExecuteCore(callback, state);
        }*/
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
            //string defaultController = (string)Session["Curent_Controller"];
            //string current_page = (string)Session["Curent_Page"];
            return RedirectToAction("Index", "Main");
        }
        //Fin de la méthode permettat de gérer la langue

        //************** Alloacate and Delete all branch of one user       
        /*protected User CurrentUser
        {
            get
            {
                try
                {
                    int Id = Convert.ToInt32(HttpContext.User.Identity.Name);
                    return LoadComponent.getUser(Id);
                }
                catch (Exception e)
                {
                    HttpCookie user_from_cookie = Request.Cookies["_userID"];
                    if (user_from_cookie != null)
                        return LoadComponent.getUser(Convert.ToInt32(user_from_cookie.Value));
                    return null;
                }
                //return (User)Session["CurrentUser"];

            }
        }*/
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
                int userBranch = bdDay.FirstOrDefault().Branch.BranchID;// _busDayRepository.GetOpenedBusinessDay(CurrentUser).FirstOrDefault().Branch;
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
        }

        protected void AlertSucces(string title, string content)
        {
            X.Msg.Info(new InfoPanel
            {
                OffsetX = -100,
                Title = title,
                Icon = Icon.Accept,
                HideDelay = 20000,
                Closable = true,
                Visible = true,
                Width = 100,
                Html = content
            }).Show();
        }
        protected void AlertError(string title, string content)
        {
            X.Msg.Info(new InfoPanel
            {
                OffsetX = -100,
                Title = title,
                Icon = Icon.Accept,
                HideDelay = 20000,
                Closable = true,
                Visible = true,
                Width = 100,
                Cls = "error-alert",
                ComponentCls = "cmp-error-alert-cls",
                Html = content
            }).Show();
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

    }
}