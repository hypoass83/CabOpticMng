using System.Linq;
using System.Web;
using System.Web.Mvc;
using FatSod.Security.Entities;
using FatSod.Security.Abstracts;
using System.Web.Security;
using FatSodDental.UI.Models;
using System.Collections.Generic;
using FatSod.DataContext.Initializer;
using FatSodDental.UI.Tools;
using FatSod.Supply.Entities;
using FatSod.Supply.Abstracts;
using System;
using System.Web.Mvc;
using Ext.Net.MVC;
using Ext.Net;
using Ext.Net.Utilities;
using System.Threading;
using CrystalDecisions.CrystalReports.Engine;
using System.IO;
using FatSod.DataContext.Concrete;
using System.Web.UI;
using FatSod.Ressources;

namespace FatSodDental.UI.Controllers
{
    public class SessionController : BaseController
    {
        // GET: Session
        IBusinessDay _bdRepository;
        IRepository<User> _userRepository;
        IMouchar _opSneak;
        public SessionController(
            IRepository<User> userRepository,
            IBusinessDay bdRepository,
            IMouchar opSneak
            )
        {
            this._bdRepository = bdRepository;
            this._userRepository = userRepository;
            this._opSneak=opSneak;
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Authentification(string login, string password)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var ConnectedUser = db.Users.AsNoTracking().Join(db.Profiles, u => u.ProfileID, p => p.ProfileID, (u, p) => new { u, p })
                        .Where(up => up.u.UserLogin == login && up.u.UserPassword == password && up.p.ProfileState)
                        .Select(cu => new
                        {
                            ProfileID = cu.p.ProfileID,
                            GlobalPersonID = cu.u.GlobalPersonID,
                            UserEntity = cu.u
                        }).FirstOrDefault();

                    /*if (ConnectedUser != null && ConnectedUser.GlobalPersonID > 0 && IsUserConnected(ConnectedUser.GlobalPersonID))
                    {
                        X.Msg.Alert(
                            "Connected Users",
                            "Sorry  " + ConnectedUser.UserFullName + " Is already Connected !!! " +
                            "See Administrator For More details"
                            ).Show();

                        return this.Direct();
                    }*/

                    if (ConnectedUser != null)
                    {

                        //if (ConnectedUser.IsConnected && ConnectedUser.Profile.ProfileState)

                        Session["UserProfile"] = ConnectedUser.ProfileID;
                        Session["UserID"] = ConnectedUser.GlobalPersonID;
                        Session["CurrentUser"] = ConnectedUser.UserEntity;
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
                        /*
                        int firstUserBranch=UserBusDays.FirstOrDefault().BranchID;
                        // -- Gestion des préférences de l'utilisateur, langue -- //
                        if (db.UserConfigurations.FirstOrDefault(l => l.DefaultBranchID == firstUserBranch).DefaultCulture == "fr")
                        {
                            //Thread.CurrentThread.CurrentUICulture = System.Globalization.CultureInfo.CreateSpecificCulture("fr");
                            Resources.Culture = System.Globalization.CultureInfo.CreateSpecificCulture("fr");
                        }
                        else
                        {
                            //Thread.CurrentThread.CurrentUICulture = System.Globalization.CultureInfo.CreateSpecificCulture("en");
                            Resources.Culture = System.Globalization.CultureInfo.CreateSpecificCulture("en");
                        }
                        */
                        this.InjectUserConfigInSession();
                        bool res = _opSneak.ConnectOperation(SessionGlobalPersonID, "SUCCESS", "CONNECT BY USER "+login, "Authentification", (SessionBusinessDay(null) == null) ? DateTime.Now.Date : SessionBusinessDay(null).BDDateOperation, (SessionBusinessDay(null) == null) ? 0 : SessionBusinessDay(null).BranchID);
                        if (!res)
                        {
                            throw new Exception("Une erreur s'est produite lors de la journalisation ");
                        }
                        return RedirectToAction("Index", "Main");
                    }
                    this.GetCmp<FormPanel>("Form").Reset();
                    bool res1 = _opSneak.ConnectOperation(SessionGlobalPersonID, "ERROR", "Wrong Username or wrong Password by user " + login, "Authentification", (SessionBusinessDay(null) == null) ? DateTime.Now.Date : SessionBusinessDay(null).BDDateOperation, (SessionBusinessDay(null) == null) ? 0 : SessionBusinessDay(null).BranchID);
                    if (!res1)
                    {
                        throw new Exception("Une erreur s'est produite lors de la journalisation ");
                    }
                    X.Msg.Alert("Utilisateur.Prenom", "Wrong Username or wrong Password by user").Show();
                    
                }
                else
                {
                    this.GetCmp<FormPanel>("Form").Reset();
                    bool res2 = _opSneak.ConnectOperation(SessionGlobalPersonID, "ERROR", "Veuillez entrer votre login et votre mot de passe", "Authentification", (SessionBusinessDay(null) == null) ? DateTime.Now.Date : SessionBusinessDay(null).BDDateOperation, (SessionBusinessDay(null) == null) ? 0 : SessionBusinessDay(null).BranchID);
                    if (!res2)
                    {
                        throw new Exception("Une erreur s'est produite lors de la journalisation ");
                    }
                    X.Msg.Alert("Utilisateur.Prenom", "Veuillez entrer votre login et votre mot de passe").Show();
                }
            }
            catch (Exception e)
            {
                bool res2 = _opSneak.ConnectOperation(SessionGlobalPersonID, "ERROR", e.Message, "Authentification", (SessionBusinessDay(null) == null) ? DateTime.Now.Date : SessionBusinessDay(null).BDDateOperation, (SessionBusinessDay(null) == null) ? 0 : SessionBusinessDay(null).BranchID);
                if (!res2)
                {
                    throw new Exception("Une erreur s'est produite lors de la journalisation ");
                }
                X.Msg.Alert("Error ", e.Message + " " + e.StackTrace + " " + e.InnerException).Show();
                return this.Direct();
            }
            return this.Direct();
        }

        [Authorize]
        public ActionResult Deconnexion()
        {
            //EcritureSneack
            bool res = _opSneak.DisconnectOperation(SessionGlobalPersonID, "SUCCESS", "DISCONNECT ", "Deconnexion", (SessionBusinessDay(null) == null) ? DateTime.Now.Date : SessionBusinessDay(null).BDDateOperation, (SessionBusinessDay(null) == null) ? 0 : SessionBusinessDay(null).BranchID);
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
                if ((UserBusDays == null || UserBusDays.Count == 0))
                {
                    if (LoadAction.isAutoriseToGetSubMenu(userModel.User.ProfileID, CodeValue.Supply.OpenBD_SM.CODE, db) == true)
                    {
                        string Curent_Page = CodeValue.Supply.OpenBD_SM.PATH;
                        string Curent_Controller = "Administration" + "/" + CodeValue.Supply.OpenBD_SM.CONTROLLER;
                        Session["Curent_Page"] = Curent_Page;
                        Session["Curent_Controller"] = Curent_Controller;

                        return RedirectToAction("Index", "Main");
                    }
                    else
                    {
                        string Curent_Page = "NoBussinessDayOpen";
                        string Curent_Controller = "Session";
                        Session["Curent_Page"] = Curent_Page;
                        Session["Curent_Controller"] = Curent_Controller;

                        return RedirectToAction("Index", "Main");
                    }
                }
                return RedirectToAction("Index", "Main");
            }
            else
            {

                return View();
            }

        }
        [Authorize]
        public ActionResult NoBussinessDayOpen()
        {
            return View("NoBussinessDayOpen");
        }
    }
}