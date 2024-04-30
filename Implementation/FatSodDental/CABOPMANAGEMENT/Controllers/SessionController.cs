using System.Globalization;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Collections.Generic;
using System.Web.Security;
using FatSod.Security.Abstracts;
using FatSod.Security.Entities;
using CABOPMANAGEMENT.Models;
using FatSod.DataContext.Initializer;
using CABOPMANAGEMENT.Tools;
using FatSod.DataContext.Concrete;
using System.Web.Configuration;
using FatSod.Ressources;

namespace CABOPMANAGEMENT.Controllers
{
    //[Authorize]
    public class SessionController : BaseController
    {
        IBusinessDay _bdRepository;
        IRepository<User> _userRepository;
        private IPerson _personRepository;
        IMouchar _opSneak;
        public SessionController(
            IRepository<User> userRepository,
            IBusinessDay bdRepository,
            IMouchar opSneak, IPerson personRepository
            )
        {
            this._bdRepository = bdRepository;
            this._userRepository = userRepository;
            this._opSneak = opSneak;
            this._personRepository = personRepository;
        }

        //
        // GET: /Session/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            ViewBag.AppNameP = WebConfigurationManager.AppSettings["AppNameP"];
            ViewBag.AppNameS = WebConfigurationManager.AppSettings["AppNameS"];
            return PartialView();
        }

       
        
        // POST: /Session/Login
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return PartialView(model);
                }

                var ConnectedUser = db.Users.Join(db.Profiles, u => u.ProfileID, p => p.ProfileID, (u, p) => new { u, p })
                        .Where(up => up.u.UserLogin == model.UserName && up.u.UserPassword == model.Password && up.p.ProfileState)
                        .Select(cu => new
                        {
                            ProfileID = cu.p.ProfileID,
                            GlobalPersonID = cu.u.GlobalPersonID,
                            UserEntity = cu.u //,
                            //UserSessionState= cu.u.UserSessionState
                        }).FirstOrDefault();

                //Restricts login unless email confirmation
                var userid = ConnectedUser.GlobalPersonID;
               
                var result = ConnectedUser.UserEntity.UserAccountState;//await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);

                var user = ConnectedUser.UserEntity;//await UserManager.FindByIdAsync(userid);

                Session["UserProfile"] = ConnectedUser.ProfileID;
                Session["UserID"] = ConnectedUser.GlobalPersonID;
                Session["CurrentUser"] = ConnectedUser.UserEntity;

                switch (result)
                {
                    case true:


                        
                        //This cookie enable to get user ID if we can access an user ID with session
                        HttpCookie user_id_cookie = Request.Cookies["_userID"];
                        if (user_id_cookie == null)
                        {
                            user_id_cookie = new HttpCookie("_userID");
                            user_id_cookie.Value = ConnectedUser.GlobalPersonID.ToString();
                            user_id_cookie.Expires = DateTime.Now.AddHours(1);
                        }
                        else
                        {
                            user_id_cookie.Expires = DateTime.Now.AddHours(1);
                        }
                        Response.Cookies.Add(user_id_cookie);
                        FormsAuthentication.SetAuthCookie(ConnectedUser.GlobalPersonID.ToString(), false);
                        ConnectUser(ConnectedUser.GlobalPersonID);

                        /*
                             vérifion si une journée de travail a déjà été ouverte.
                             il s'agit de vérifier si au moins une des agences de l'utilisateur est ouverte
                            */

                        List<BusinessDay> UserBusDays = _bdRepository.GetOpenedBusinessDay(ConnectedUser.UserEntity);
                        Session["UserBusDays"] = UserBusDays;
                        if ((UserBusDays == null || UserBusDays.Count == 0))
                        {
                            if (LoadAction.isAutoriseToGetSubMenu(SessionProfileID, CodeValue.Supply.OpenBD_SM.CODE, db) == true)
                            {
                                Session["Curent_Page"] = CodeValue.Supply.OpenBD_SM.PATH;
                                Session["Curent_Controller"] = "Administration" + "/" + CodeValue.Supply.OpenBD_SM.CONTROLLER;
                                return RedirectToAction(CodeValue.Supply.OpenBD_SM.PATH, "Administration" + "/" + CodeValue.Supply.OpenBD_SM.CONTROLLER);
                            }
                            else
                            {
                                Session["Curent_Page"] = "NoBussinessDayOpen";
                                Session["Curent_Controller"] = "Session";
                                return RedirectToAction("NoBussinessDayOpen", "Session");
                            }
                        }

                        this.InjectUserConfigInSession();
                        bool res = _opSneak.ConnectOperation(SessionGlobalPersonID, "SUCCESS", "CONNECT BY USER " + model.UserName, "Authentification", DateTime.Now.Date, 0 );
                        if (!res)
                        {
                            throw new Exception("Une erreur s'est produite lors de la journalisation ");
                        }
                        return RedirectToLocal(returnUrl);
                        //return RedirectToAction("Index", "Home");
                    case false:// SignInStatus.LockedOut:
                        return View("Lockout");
                    default:
                        ModelState.AddModelError("", "Invalid login attempt.");
                        return PartialView(model);
                }
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e.Message + "Invalid login attempt.");
                return PartialView(model);
            }
        }

        /*************************/


        ////try using raw sql for efficiency
        public JsonResult CheckExpiry()
        {
            using (EFDbContext dbo = new EFDbContext())
            {
                /*
                int count = (from s in db.Stocks
                              where DateTime.Now > s.ExpiryDate
                              select s).Count();

                */
                /* var notif = (from n in db.Notifications
                             select n).SingleOrDefault();
                 notif.ToExpire = count;
                 db.SaveChanges();*/

                var list = db.ProductLocalizations.SqlQuery("select * from dbo.ProductLocalizations where GETDATE() > ExpiryDate").ToList();
                return Json(list.Count(), JsonRequestBehavior.AllowGet);
            }

        }
        public JsonResult CheckStockLevel()
        {
            using (EFDbContext db = new EFDbContext())
            {
                var list = db.ProductLocalizations.SqlQuery("select * from dbo.ProductLocalizations s, Products i where i.ProductID = s.ProductID and s.ProductLocalizationStockQuantity<s.ProductLocalizationSafetyStockQuantity and s.ProductLocalizationStockQuantity>0;").ToList();
                return Json(list.Count(), JsonRequestBehavior.AllowGet);
            }
        }


        // POST: /Session/LogOff
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            //EcritureSneack
            bool res = _opSneak.DisconnectOperation(SessionGlobalPersonID, "SUCCESS", "DISCONNECT ", "Deconnexion", DateTime.Now.Date ,  0 );
            if (!res)
            {
                throw new Exception("Une erreur s'est produite lors de la journalisation ");
            }
            HttpCookie userConnected = Request.Cookies["_userID"];
            if (userConnected != null)
                userConnected.Expires = DateTime.Now.AddSeconds(0);
            FormsAuthentication.SignOut();
            Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        public ActionResult NoBDOpened()
        {
            AuthenticateUser userModel = new AuthenticateUser
            {
                Authenticate = HttpContext.User.Identity.IsAuthenticated
            };
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                userModel.User = _userRepository.FindAll.FirstOrDefault(u => u.GlobalPersonID == int.Parse(HttpContext.User.Identity.Name));
                Session["UserProfile"] = userModel.User.ProfileID;
                Session["UserID"] = userModel.User.GlobalPersonID;
                Session["CurrentUser"] = userModel.User;
                HttpCookie user_id_cookie = Request.Cookies["_userID"];
                if (user_id_cookie == null)
                {
                    user_id_cookie = new HttpCookie("_userID");
                    user_id_cookie.Value = userModel.User.GlobalPersonID.ToString();
                    user_id_cookie.Expires = DateTime.Now.AddHours(1);
                }
                else
                {
                    user_id_cookie.Expires = DateTime.Now.AddHours(1);
                }
                Response.Cookies.Add(user_id_cookie);
                List<BusinessDay> UserBusDays = _bdRepository.GetOpenedBusinessDay(userModel.User);
                Session["UserBusDays"] = UserBusDays;
                if ((UserBusDays == null || UserBusDays.Count == 0))
                {
                    if (LoadAction.isAutoriseToGetSubMenu(userModel.User.ProfileID, CodeValue.Supply.OpenBD_SM.CODE, db) == true)
                    {
                        string Curent_Page = CodeValue.Supply.OpenBD_SM.PATH;
                        string Curent_Controller =CodeValue.Supply.OpenBD_SM.CONTROLLER;
                        Session["Curent_Page"] = Curent_Page;
                        Session["Curent_Controller"] = Curent_Controller;

                        return RedirectToAction(Curent_Page, Curent_Controller, new { area = "Administration" });
                    }
                    else
                    {
                        string Curent_Page = "NoBussinessDayOpen";
                        string Curent_Controller = "Session";
                        Session["Curent_Page"] = Curent_Page;
                        Session["Curent_Controller"] = Curent_Controller;

                        return RedirectToAction(Curent_Page, Curent_Controller);
                    }
                }
                return RedirectToAction("Index", "Home");
            }
            else
            {

                return View("NoBussinessDayOpen");
            }

        }
        [Authorize]
        public ActionResult NoBussinessDayOpen()
        {
            return RedirectToAction(CodeValue.Supply.OpenBD_SM.PATH, CodeValue.Supply.OpenBD_SM.CONTROLLER, new { area = "Administration" });
        }
        //
        // GET: /Session/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        [OutputCache(Duration = 3600)]
        public ActionResult ChangePassword()
        {
            return View();
        }

        public JsonResult EditUser (string UserLogin,string OldUserPassword,string NewUserPassword,string UserPassword2)
        {
            bool status = false;
            string Message = "";
            string redirectUrl = "";
            try
            {
                //verify if the user exist
                User existUser = db.Users.Where(u => u.UserLogin == UserLogin.Trim() && u.UserPassword == OldUserPassword.Trim()).SingleOrDefault();
                if (existUser==null)
                {
                    status = false;
                    Message = "Please check your entries!!! this user and password does not exist";
                    return new JsonResult { Data = new { status = status, Message = Message, redirectUrl = redirectUrl } };
                }
                //verif if new password its ok
                if (NewUserPassword.Trim()!= UserPassword2.Trim())
                {
                    status = false;
                    Message = "The "+ Resources.NewUserPassword + " it is different from "+ Resources.UserPassword2;
                    return new JsonResult { Data = new { status = status, Message = Message, redirectUrl = redirectUrl } };
                }

                DateTime OperationDate = SessionBusinessDay(null).BDDateOperation;
                int BranchID = SessionBusinessDay(null).BranchID;

                if (existUser.GlobalPersonID > 0)
                {
                    existUser.UserPassword = NewUserPassword.Trim();

                    User userToUpdate = (User)_personRepository.Update2(existUser, SessionGlobalPersonID, OperationDate, BranchID);

                    statusOperation = "Modification Password OK";
                }

                status = true;
                Message = statusOperation;
                redirectUrl = Url.Action("Login", "Session", new { area = "" });
            }
            catch (Exception e)
            {
                status = false;
                Message = @"Error :" + e.Message ;
            }
            return new JsonResult { Data = new { status = status, Message = Message, redirectUrl = redirectUrl } };

        }
        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            try
            {
                if (Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception e)
            {
                return RedirectToAction("Index", "Home");
            }
            
            
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}