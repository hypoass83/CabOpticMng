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

namespace FatSodDental.UI.Controllers
{
    public class SecurityController 
        : BaseController
    {
        // GET: Security
        IRepository<User> _userRepository;
        IBusinessDay _bdRepository;
        IRepository<Branch> _branchRepository;
        private IRepositorySupply<TillDay> _tillDayRepository;
        private IRepositorySupply<UserTill> _userTillRepository;
        private IRepository<UserBranch> _userBranchRepository;
        private IRepository<Company> _companyRepository;
        
        public SecurityController(
            IRepository<User> userRepository,
            IRepository<UserBranch> userBranchRepository,
            IBusinessDay bdRepository,
            IRepository<Branch> branchRepository,
            IRepositorySupply<TillDay> tillDayRepository,
            IRepositorySupply<UserTill> userTillRepository,
            IRepository<Company> companyRepository

            )
        {
            this._userRepository = userRepository;
            this._userBranchRepository = userBranchRepository;
            this._bdRepository = bdRepository;
            this._branchRepository = branchRepository;
            this._tillDayRepository = tillDayRepository;
            this._userTillRepository = userTillRepository;
            this._companyRepository = companyRepository;
            
        }
        public ActionResult Index()
        {
            return View();
        }

        //[OutputCache(Duration = 3600)]
        public ActionResult Connect(string login, string password)
        {
            try
            {
            if (ModelState.IsValid)
            {
                //var ConnectedUser = db.Users.Where(u => u.UserLogin == login && u.UserPassword == password).FirstOrDefault();
                var ConnectedUser = db.Users.AsNoTracking().Join(db.Profiles, u => u.ProfileID, p => p.ProfileID, (u, p) => new { u, p })
                    .Where(up => up.u.UserLogin == login && up.u.UserPassword == password && up.p.ProfileState)
                    .Select(cu => new 
                    {
                        ProfileID=cu.p.ProfileID,
                        GlobalPersonID=cu.u.GlobalPersonID,
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
                        if (LoadAction.isAutoriseToGetSubMenu(SessionProfileID, CodeValue.Supply.OpenBD_SM.CODE,db) == true)
                        {
                            Session["Curent_Page"] = CodeValue.Supply.OpenBD_SM.PATH;
                            Session["Curent_Controller"] = "Administration" + "/" + CodeValue.Supply.OpenBD_SM.CONTROLLER;
                            return RedirectToAction("Index", "Dental");
                        }
                        else
                        {
                            Session["Curent_Page"] = "NoBussinessDayOpen";
                            Session["Curent_Controller"] = "Security";
                            return RedirectToAction("Index", "Dental");
                        }
                    }
                    return RedirectToAction("Index", "Dental");
                }
                ModelState.AddModelError("Utilisateur.Prenom", "Prénom et/ou mot de passe incorrect(s)");
                X.Msg.Alert("Utilisateur.Prenom", "Prénom et/ou mot de passe incorrect(s)");
                return RedirectToAction("Login", "Security");
            }
            else
            {
                ModelState.AddModelError("Utilisateur.Login", "Veuillez entrer votre login et votre mot de passe");
                X.Msg.Alert("Utilisateur.Prenom", "Veuillez entrer votre login et votre mot de passe");
                return RedirectToAction("Login", "Security");
            }
            }
            catch (Exception e) { X.Msg.Alert("Error ", e.Message + " " + e.StackTrace + " " + e.InnerException).Show(); 
                return this.Direct(); }
        
        }
        
        
        public ActionResult Login()
        {

            //il est propabla que cette connexion fasse suite à une expiration de sexion mais cela n'est pas toujours vrai car ...
            
            AuthenticateUser userModel = new AuthenticateUser
            {
                Authenticate = HttpContext.User.Identity.IsAuthenticated
            };

            if (HttpContext.User.Identity.IsAuthenticated)
            {
                userModel.User = _userRepository.Find(int.Parse(HttpContext.User.Identity.Name));

                Session["UserProfile"] = userModel.User.ProfileID;
                Session["UserID"] = userModel.User.GlobalPersonID;
                Session["CurrentUser"] = userModel.User;
                HttpCookie user_id_cookie = Request.Cookies["_userID"];
                if (user_id_cookie == null)
                {

                    string userID = (HttpContext.User.Identity.Name);

                    if (userID != null && userID.Length > 0)
                    {
                        this.DisconnectUser(int.Parse(userID));
                    }

                    user_id_cookie = new HttpCookie("_userID");
                    user_id_cookie.Value = userModel.User.GlobalPersonID.ToString();
                    user_id_cookie.Expires = DateTime.Now.AddMinutes(1);

                }
                else
                {
                    user_id_cookie.Expires = DateTime.Now.AddMinutes(1);
                }
                Response.Cookies.Add(user_id_cookie);
                List<BusinessDay> UserBusDays = _bdRepository.GetOpenedBusinessDay(userModel.User);
                if ((UserBusDays == null || UserBusDays.Count == 0))
                {
                    if (LoadAction.isAutoriseToGetSubMenu(userModel.User.ProfileID, CodeValue.Supply.OpenBD_SM.CODE,db) == true)
                    {
                        string Curent_Page = CodeValue.Supply.OpenBD_SM.PATH;
                        string Curent_Controller = "Administration" + "/" + CodeValue.Supply.OpenBD_SM.CONTROLLER;
                        Session["Curent_Page"] = Curent_Page;
                        Session["Curent_Controller"] = Curent_Controller;

                        return RedirectToAction(Curent_Page, Curent_Controller);
                    }
                    else
                    {
                        string Curent_Page = "NoBussinessDayOpen";
                        string Curent_Controller = "Security";
                        Session["Curent_Page"] = Curent_Page;
                        Session["Curent_Controller"] = Curent_Controller;

                        return RedirectToAction(Curent_Page, Curent_Controller);
                    }
                }
                return RedirectToAction("Index", "Dental");
            }
            else
            {
                
                return View();
            }

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
                    if (LoadAction.isAutoriseToGetSubMenu(userModel.User.ProfileID, CodeValue.Supply.OpenBD_SM.CODE,db) == true)
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
                        string Curent_Controller = "Security";
                        Session["Curent_Page"] = Curent_Page;
                        Session["Curent_Controller"] = Curent_Controller;

                        return RedirectToAction("Index", "Dental");
                    }
                }
                return RedirectToAction("Index", "Dental");
            }
            else
            {
                
                return View("Login");
            }

        }

        //[OutputCache(Duration = 60)]
        [Authorize]
        public ActionResult LogOff()
        {
            
            HttpCookie userConnected = Request.Cookies["_userID"];
            if (userConnected != null)
                userConnected.Expires = DateTime.Now.AddSeconds(0);
            FormsAuthentication.SignOut();
            Session.Clear();

            return RedirectToAction("Login", "Security");
            //return RedirectToAction("Index", "Dental");
        }

        [Authorize]
        public ActionResult ChooseBranch()
        {
            User currentUser = _userRepository.FindAll.FirstOrDefault(u => u.GlobalPersonID == int.Parse(HttpContext.User.Identity.Name));
            return View(_userBranchRepository.FindAll.Where(ub => ub.UserID == SessionGlobalPersonID));
        }
        [Authorize]
        public ActionResult NoBussinessDayOpen()
        {
            return View("NoBussinessDayOpen");
        }
        public ActionResult InitReport()
        {
            ReportDocument rptH = new ReportDocument();
            List<object> model = new List<object>();
            string defaultImgPath = Server.MapPath("~/Content/Images/App/default-img.png");
            byte[] COMPANY_LOGO = System.IO.File.ReadAllBytes(defaultImgPath);
            
            for (int i = 0; i < 30; i++)
            {

                model.Add(
                    new
                    {
                        Localization = i,
                        ProductLabel = i,
                        ProductQty = i,
                        ProductUnitPrice = i,
                        
                        BranchName = "p.Localization.Branch.BranchName",
                        BranchAdress = "Branch.Adress.AdressPOBox",
                        BranchTel = "Branch.Adress.AdressPhoneNumber",
                        CompanyName = Company.Name,
                        CompanyAdress = Company.Adress.AdressEmail + Company.Adress.AdressPOBox,
                        CompanyTel = Company.Adress.AdressPhoneNumber,
                        CompanyCNI = Company.CNI,
                        CompanyLogo = COMPANY_LOGO
                    }
                    );
            }
            
            string path = Server.MapPath("~/Reports/Supply/RptInventory.rpt");
            rptH.Load(path);
            rptH.SetDataSource(model);
            Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            return File(stream, "application/pdf");
        }
        private Company Company
        {
            get
            {
                return _companyRepository.FindAll.FirstOrDefault();
                //HttpContext.Request.UserHostAddress
            }
        }
    }
}