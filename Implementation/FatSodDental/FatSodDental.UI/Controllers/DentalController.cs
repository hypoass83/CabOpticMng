using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FatSod.Security.Entities;
using FatSod.Security.Abstracts;
using Ext.Net.MVC;
using Ext.Net.Utilities;
using Ext.Net;
using FatSodDental.UI.Filters;
using FatSod.DataContext.Concrete;
using System.Web.UI;

namespace FatSodDental.UI.Controllers
{
    [Authorize(Order = 1)]
    //[TakeBusinessDay(Order = 2)]
    public class DentalController : BaseController
    {
        // GET: Dental
        private IModule _moduleRepository;
        private IUser _userRepository;
        private IRepository<ActionMenuProfile> _profileMenuRepository;
        private IRepository<UserBranch> _userBranchRepository;
        

        public DentalController
            (
                IModule moduleRepository,
                IUser userRepository,
                IRepository<ActionMenuProfile> profileMenuRepository,
                IRepository<UserBranch> userBranchRepository

            )
        {
            
            this._moduleRepository = moduleRepository;
            this._userRepository = userRepository;
            this._profileMenuRepository = profileMenuRepository;
            this._userBranchRepository = userBranchRepository;
        }
        //[OutputCache(Duration = 60)]
        public ActionResult Index(string Branch)
        {
            int profilId = this.SessionProfileID;
            int id_defaultModule = db.ActionMenuProfiles.Where(pm => pm.ProfileID == profilId).FirstOrDefault().Menu.ModuleID;

            //ActionMenuProfile defaultModule = _profileMenuRepository.FindAll.First(pm => pm.ProfileID == CurrentUser().ProfileID);
            Session["defaultModule"] = id_defaultModule;
            //we test if this user has more that one branch

            return View(ModelModule);//_moduleRepository.FindAll);
        }
        private List<object> ModelModule
        {
            get
            {
                List<object> model = new List<object>();
                //var module = db.Modules.Select(mod=> new
                //{
                //    ModuleID = mod.ModuleID,
                //    ModuleCode = mod.ModuleCode,
                //    ModuleLabel = mod.ModuleLabel,
                //    ModuleDescription = mod.ModuleDescription,
                //    ModuleArea = mod.ModuleArea,
                //    ModuleState = mod.ModuleState
                //}).ToList();

                db.Modules.ToList().ForEach(c =>
                {
                    model.Add(
                        new
                        {
                            ModuleID = c.ModuleID,
                            ModuleCode = c.ModuleCode,
                            ModuleLabel = c.ModuleLabel,
                            ModuleDescription = c.ModuleDescription,
                            ModuleArea = c.ModuleArea,
                            ModuleState = c.ModuleState
                        }
                    );
                });

                return model;
            }
        }
        public ActionResult Home()
        {
            return View();
        }
        public ActionResult AfterRedirect()
        {
            return View();
        }
        private User CurrentUser()
        {
            int userid=int.Parse(HttpContext.User.Identity.Name);
            return (from u in db.Users
                    where u.GlobalPersonID == userid
                    select u).FirstOrDefault();
            //return _userRepository.FindAll.FirstOrDefault(u => u.GlobalPersonID == int.Parse(HttpContext.User.Identity.Name));
        }
        //[DirectMethod]
        public ActionResult ServletFaces(string Curent_Page, string Curent_Controller)
        {

            //Curent_Page = "Test";
            //Curent_Controller = "Dental";

            Session["Curent_Page"] = Curent_Page;
            Session["Curent_Controller"] = Curent_Controller;
            /*
            Ext.Net.MVC.PartialViewResult partialViewOfMenus = new Ext.Net.MVC.PartialViewResult()
            {
                ContainerId = "Body",
                ViewName = Curent_Page,
                ClearContainer = true,
            };
            
            return partialViewOfMenus;
            */
            //return new Ext.Net.MVC.PartialViewResult("Body");

            Panel panel = this.GetCmp<Panel>("Body");
            panel.Loader = new ComponentLoader
            {
                Url = Url.Action(Curent_Page, Curent_Controller),
                DisableCaching = true,
                Mode = LoadMode.Frame,
            };
            panel.Loader.SuspendScripting();
            panel.LoadContent();

            return this.Direct();
            //View(/*"~/Views/Dental/Index.cshtml"*/);
        }

        public ActionResult Test()
        {
            return View();
        }

    }
}