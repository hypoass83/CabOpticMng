using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FatSod.Security.Entities;
using FatSod.Security.Abstracts;
using Ext.Net.MVC;
using Ext.Net;
using FatSodDental.UI.Tools;
using FatSod.Ressources;
using FatSodDental.UI.Filters;
using FatSod.DataContext.Concrete;

namespace FatSodDental.UI.Controllers
{
    /**
        * Menu controller
        * @autor Kdms
        * @Date 07 Mars 2015 V1.0
        * @Copyright FatSod Group
        */
    [Authorize(Order = 1)]
    //[TakeBusinessDay(Order = 2)]
    public class MenuController : BaseController
    {
        private const string CONTROLLER_NAME = "Menu";
        private const string SHARED_MENU_VIEW = "Menus";
        private const string DEFAULT_CONTAINER_BODY = "Menu_Container";
        //This const are the path of content images folders
        private const string MOD_PRESSED_IMG_PATH = "~/Content/Images/App/Modules/Pressed/";
        private const string MOD_IMG_PATH = "~/Content/Images/App/Modules/";
        private const string MOD_DISABLED_IMG_PATH = "~/Content/Images/App/Modules/Pressed/";
        private const string MENU_ICON_PATH = "~/Content/Images/App/Menus/";
        //Entities managers
        private IRepository<FatSod.Security.Entities.Menu> _menuRepository;
        private IRepository<SubMenu> _subMenuRepository;
        private IModule _moduleRepository;
        private IRepository<Profile> _profileRepository;
        private IRepository<ActionMenuProfile> _profileMenuRepository;
        private IRepository<ActionSubMenuProfile> _profileSubMenuRepository;
        private IRepository<User> _userRepository;

        //Current user's parameters
        private User _currentUser;
        private List<Module> _modulessOfCurrentUser;
        private IEnumerable<ActionMenuProfile> _profileMenusOfCurrentUser;
        private IEnumerable<SubMenu> _subMenusOfCurrentUser;
        private IBusinessDay _businessDayRepository;
        
        public MenuController
            (
                IRepository<FatSod.Security.Entities.Menu> menu,
                IModule module,
                IRepository<Profile> profile,
                IRepository<ActionMenuProfile> profileMenu,
                IRepository<ActionSubMenuProfile> profileSubMenuRepository,
                IRepository<User> user,
                IRepository<SubMenu> subMenuRepository,
                IBusinessDay businessDayRepository
            )
        {
            this._menuRepository = menu;
            this._moduleRepository = module;
            this._profileRepository = profile;
            this._profileMenuRepository = profileMenu;
            this._profileSubMenuRepository = profileSubMenuRepository;
            this._userRepository = user;
            this._subMenuRepository = subMenuRepository;
            this._businessDayRepository = businessDayRepository;
            _modulessOfCurrentUser = new List<Module>();
            _profileMenusOfCurrentUser = new List<ActionMenuProfile>();
            _subMenusOfCurrentUser = new List<SubMenu>();
            
        }
        /**
         * This is the defaul view that loaded when we run application
         * Return all defaults elements of applications.
         * 
         */
        [AllowAnonymous]
        public ActionResult Home()
        {
            return RedirectToAction("Login", "Security");
        }
        public ActionResult Index()
        {
            return View();
        }
        //this action load defautl menu that all user has access
        public ActionResult GetDefaultMenus()
        {
            List<AbstractComponent> moduleComponents = new List<AbstractComponent>();
            BusinessDay bsday= _businessDayRepository.GetOpenedBusinessDay(CurrentUser).FirstOrDefault();
            DateTime currentDate = (bsday != null && bsday.BusinessDayID > 0) ? (new DateTime(bsday.BDDateOperation.Year, bsday.BDDateOperation.Month, bsday.BDDateOperation.Day)) : DateTime.Now;
            //We get the current user
            //Default module
            Ext.Net.Button fileButtton = new Ext.Net.Button()
            {
                Flat = true,
                Disabled = true,
                Width = 70,
                ComponentCls = "logo-inventory-cls",
                Flex = 1,
                //IconUrl = "../../Content/Images/Icons/inventoryLogo.png",
                OverCls = "logo-cls",
                MarginSpec = "-3 0 0 0",
                StyleSpec = "color : #bfe9f6; border : 0px; border-radius : 0px;",
                ArrowVisible = false,
                Margin = 0,
                Cls = "logo1"

            };
            Ext.Net.Button btnLangage = new Ext.Net.Button()
            {
                Flat = true,
                Text = "<span style = \"color :#000000; font-size: 1em;\">" + Resources.Lang.ToUpper() + "</span> ",
                //Height = 28,
                //Weight = 92,
                Width = 80,
                ComponentCls = "file-cls",
                OverCls = "file-over-cls",
                ///*BaseCls = "file-base-cls",
                StyleSpec = " color : #000000; border : 0px; border-radius : 0px;",
                ArrowVisible = false,
                MarginSpec = "-3 0 0 0",
                Cls = "btn-info kdms"

            };
            Ext.Net.Button helpButton = new Ext.Net.Button()
            {
                Flat = true,
                Text = "<span style = \"color :#000000; font-size: 1em;\"> HELP</span> ",
                //Height = 28,
                //Weight = 92,
                Width = 50,
                ComponentCls = "file-cls",
                OverCls = "file-over-cls",
                ///*BaseCls = "file-base-cls",
                StyleSpec = "color : #000000; border : 0px; border-radius : 0px;",
                ArrowVisible = false,
                MarginSpec = "-3 0 0 0",
                Cls = "btn-info kdms"
            };
            Ext.Net.Button LabUserConnect = new Ext.Net.Button()
            {
                Flat = true,
                Text = "<span style = \"color :#0000FF; font-size: 1.1em;\">  Welcome " + CurrentUser.Name + " " + CurrentUser.Description + " Your are connected to FSInventory the " + currentDate.ToShortDateString() + "</span> ",
                //Height = 28,
                //Weight = 92,
                Width = 600,
                ComponentCls = "file-cls",
                OverCls = "file-over-cls",
                ///*BaseCls = "file-base-cls",
                StyleSpec = "color : #000000; border : 0px; border-radius : 0px;",
                ArrowVisible = false,
                MarginSpec = "-3 0 0 0",
                Cls = "btn-info kdms"

            };
            //======================
            //=================================This button for select language
            Ext.Net.Menu selectLangageBtn = new Ext.Net.Menu();
            //English Button
            Ext.Net.MenuItem en_US = new Ext.Net.MenuItem(Resources.AppLangEN);
            en_US.DirectEvents.Click.Url = this.Url.Action("SetCulture", "Security");
            en_US.DirectEvents.Click.ExtraParams.Add(new Parameter("culture", "en-US", ParameterMode.Value));
            //French button
            Ext.Net.MenuItem fr_FR = new Ext.Net.MenuItem(Resources.AppLangFR);
            fr_FR.DirectEvents.Click.Url = this.Url.Action("SetCulture", "Security");
            fr_FR.DirectEvents.Click.ExtraParams.Add(new Parameter("culture", "fr", ParameterMode.Value));
            //we add this two language
            selectLangageBtn.Add(en_US);
            selectLangageBtn.Add(fr_FR);
            //=============================================================================================
            //====================== Helps button ==================================
            Ext.Net.Menu wrapHelpButton = new Ext.Net.Menu();
            Ext.Net.MenuItem viewHelp = new Ext.Net.MenuItem(/*Resources.AppLangFR*/"View help");
            Ext.Net.MenuItem technicalSupport = new Ext.Net.MenuItem(/*Resources.AppLangFR*/"Technical support");
            Ext.Net.MenuItem aboutSofware = new Ext.Net.MenuItem(/*Resources.AppLangFR*/"About");
            wrapHelpButton.Add(viewHelp);
            wrapHelpButton.Add(technicalSupport);
            wrapHelpButton.Add(aboutSofware);
            //=================== End menu item of help button================
            btnLangage.Menu.Add(selectLangageBtn);
            helpButton.Menu.Add(wrapHelpButton);
            /*====================================================================*/
            //we add different at default lateral bar
            moduleComponents.Add(fileButtton);
            moduleComponents.Add(btnLangage);
            moduleComponents.Add(helpButton);
            moduleComponents.Add(LabUserConnect);
            return this.ComponentConfig(moduleComponents.AsEnumerable());
        }
        //=========================================================
        //This method load a differents modules that one user has
        public ActionResult GetModules()
        {
            
            List<AbstractComponent> moduleComponents = new List<AbstractComponent>();
            //We get the current user
            int userid = int.Parse(HttpContext.User.Identity.Name);

            //_currentUser = _userRepository.FindAll.FirstOrDefault(u => u.GlobalPersonID == int.Parse(HttpContext.User.Identity.Name));
            var curUser = db.Users.Where(u => u.GlobalPersonID == userid).Select(s => new { ProfileID = s.ProfileID }).SingleOrDefault();
            //Default module
            Ext.Net.Button fileButtton = new Ext.Net.Button()
            {
                Flat = true,
                Text = "<span style = \"color :#0094ff; font-size: 1em;\">" + Resources.File + "</span> ",
                Width = 70,
                ComponentCls = "file-cls",
                OverCls = "file-over-cls",
                StyleSpec = "background : #ffffff; color : #0094ff; border : 0px; border-radius : 0px;",
                ArrowVisible = false,
                Margin = 0,
                Cls = "btn-info kdms"

            };
            
            //======================
            //We build her list menus ===========================================
            //ActionMenuProfile ppm = _profileMenuRepository.FindAll.FirstOrDefault(pm => pm.ProfileID == _SessionProfileID);
            Ext.Net.Menu subMenuButton = new Ext.Net.Menu();
            Ext.Net.MenuItem item = new Ext.Net.MenuItem(Resources.Parameter  + "s ");
            item.DirectEvents.Click.Url = this.Url.Action("Parameters", "Security");
            Ext.Net.MenuItem item2 = new Ext.Net.MenuItem(Resources.Logoff);
            item2.DirectEvents.Click.Url = this.Url.Action("LogOff", "Security");
            subMenuButton.Add(item);
            subMenuButton.Add(item2);
            fileButtton.Menu.Add(subMenuButton);
            //=================================This button for select language
            
            moduleComponents.Add(fileButtton);
            /*====================================================================*/

            //We get all menus that has the current user

            _profileMenusOfCurrentUser = (from pm in db.ActionMenuProfiles
                                          where pm.ProfileID == curUser.ProfileID
                                          select pm).ToArray();
                //_profileMenuRepository.FindAll.Where(pm => pm.ProfileID == curUser.ProfileID);

            //We build all menus of this profile
            _modulessOfCurrentUser.Clear();
            foreach (var profileMenu in _profileMenusOfCurrentUser.ToArray())
            {
                if (!_modulessOfCurrentUser.Contains(profileMenu.Menu.Module))
                {
                    _modulessOfCurrentUser.Add(profileMenu.Menu.Module);
                }
            }
            //We get all module in data base
            foreach (var module in _modulessOfCurrentUser.ToArray())
            {
                //We build a handle action of others modules without the amount module  
                string myHandler = String.Empty;
                foreach (var itemMenu2 in _modulessOfCurrentUser.ToArray())
                {
                    if (!itemMenu2.Equals(module) || itemMenu2.ModuleID != module.ModuleID)
                    {
                        myHandler += "App." + itemMenu2.ModuleLabel + "" + itemMenu2.ModuleID + ".setDisabled(false);";
                    }

                }
                ImageButton imgBt = new ImageButton()
                {
                    Text = module.ModuleLabel,
                    ID = module.ModuleLabel + "" + module.ModuleID,
                    ImageUrl = MOD_IMG_PATH + Resources.ResourceManager.GetString(module.ModuleImagePath),
                    PressedImageUrl = MOD_PRESSED_IMG_PATH + Resources.ResourceManager.GetString(module.ModulePressedImagePath),
                    DisabledImageUrl = MOD_DISABLED_IMG_PATH + Resources.ResourceManager.GetString(module.ModuleDisabledImagePath),

                    Handler = "App." + module.ModuleLabel + "" + module.ModuleID + ".setDisabled(true);" + myHandler
                };
               
                imgBt.DirectEvents.Click.Action = this.Url.Action("MenuContainer", CONTROLLER_NAME);
                imgBt.DirectEvents.Click.ExtraParams.Add(new Parameter("moduleID", module.ModuleID + "", ParameterMode.Value));

                moduleComponents.Add(imgBt);
            }
            
            return this.ComponentConfig(moduleComponents.AsEnumerable());
        }
        //This method load menus of a specify module
        public ActionResult GetMenusOfModule(string module)
        {

            try
            {
                ////We initialize a list that conatain all menus of module
                List<AbstractComponent> menuComponents = new List<AbstractComponent>();
                int moduleID = int.Parse(module);
                ////We get all a menu of module that have been past
                //List<FatSod.Security.Entities.Menu> menusOfModule = new List<FatSod.Security.Entities.Menu>();
                //List<FatSod.Security.Entities.Menu> menusOfModuleTmp = _menuRepository.FindAll.Where(m => m.ModuleID == moduleID).ToList();
                ////We get all menus and subMenus of current user
                //_currentUser = _userRepository.FindAll.FirstOrDefault(u => u.GlobalPersonID == int.Parse(HttpContext.User.Identity.Name));
                //IEnumerable<FatSod.Security.Entities.ActionMenuProfile> menusOfCurrentUser = _profileMenuRepository.FindAll.Where(m => m.ProfileID == _SessionProfileID).ToArray();
                //foreach (var actionMenuProfile in menusOfCurrentUser)
                //{
                //    FatSod.Security.Entities.Menu menuTmp = _menuRepository.FindAll.FirstOrDefault(m => m.MenuID == actionMenuProfile.MenuID);
                //    if (menusOfModuleTmp.Contains(menuTmp))
                //    {
                //        menusOfModule.Add(menuTmp);
                //    }
                //}

                var menusOfModule = db.ActionMenuProfiles.Join(db.Menus, a => a.MenuID, m => m.MenuID,
                    (a, m) => new { a, m })
                    .Where(am => am.a.ProfileID == SessionProfileID && am.m.ModuleID == moduleID)
                    .Select(s => new
                    {
                        MenuID = s.m.MenuID,
                        MenuCode = s.m.MenuCode,
                        MenuDescription = s.m.MenuDescription,
                        MenuController = s.m.MenuController,
                        //MenuState = s.m.MenuState,
                        MenuLabel = s.m.MenuLabel,
                        //MenuFlat = s.m.MenuFlat,
                        MenuIconName = s.m.MenuIconName,
                        MenuPath = s.m.MenuPath,
                        //IsChortcut = s.m.IsChortcut,
                        Module = s.m.Module
                    })
                    .ToList();

                if (menusOfModule != null)
                {
                    foreach (var menu in menusOfModule.OrderBy(m => m.MenuCode))
                    {
                        Ext.Net.Button button = new Ext.Net.Button()
                        {
                            Flat = true,
                            Text = "<span style = \"color :#0094ff;\">" + Resources.ResourceManager.GetString(menu.MenuCode) != null ? Resources.ResourceManager.GetString(menu.MenuCode) : menu.MenuLabel + "</span>",// + " " + moduleID,
                            IconCls = "menu-icon-cls",
                            IconUrl = MENU_ICON_PATH + menu.MenuIconName,
                            IconAlign = IconAlign.Top,
                            ArrowAlign = ArrowAlign.Right,
                            Margin = 1
                        };
                        //We built subMenu of current user
                        var subMenus = db.ActionSubMenuProfiles.Join(db.SubMenus, am => am.SubMenuID, sm => sm.SubMenuID,
                        (am, sm) => new { am, sm })
                        .Where(asm => asm.sm.MenuID == menu.MenuID && asm.am.ProfileID == SessionProfileID)
                        .Select(s => new
                        {
                            SubMenuID = s.sm.SubMenuID,
                            SubMenuCode = s.sm.SubMenuCode,
                            SubMenuLabel = s.sm.SubMenuLabel,
                            SubMenuDescription = s.sm.SubMenuDescription,
                            SubMenuController = s.sm.SubMenuController,
                            SubMenuPath = s.sm.SubMenuPath,
                            //IsChortcut = s.sm.IsChortcut,
                            Menu = s.sm.Menu
                        })
                        .ToList();

                        //List<SubMenu> subMenus = new List<SubMenu>();
                        //List<SubMenu> subMenusTmp = _subMenuRepository.FindAll.Where(sm => sm.MenuID == menu.MenuID).ToList();
                        //IEnumerable<ActionSubMenuProfile> actionSubMenusTmp = _profileSubMenuRepository.FindAll.Where(sm => sm.ProfileID == _SessionProfileID).ToArray();
                        //foreach (ActionSubMenuProfile actionSubMenu in actionSubMenusTmp)
                        //{
                        //    SubMenu subMenuTmp = _subMenuRepository.FindAll.FirstOrDefault(sm => sm.SubMenuID == actionSubMenu.SubMenuID);
                        //    if (subMenusTmp.Contains(subMenuTmp))
                        //    {
                        //        subMenus.Add(subMenuTmp);
                        //    }
                        //}
                        long intM = 0;
                        intM = subMenus.LongCount();
                        if (subMenus != null && intM != 0)
                        {
                            //We build her list menus
                            Ext.Net.Menu subMenuButton = new Ext.Net.Menu();
                            foreach (var subMenu in subMenus.OrderBy(sm => sm.SubMenuCode))
                            {
                                Ext.Net.MenuItem item = new Ext.Net.MenuItem(Resources.ResourceManager.GetString(subMenu.SubMenuCode) != null ? Resources.ResourceManager.GetString(subMenu.SubMenuCode) : subMenu.SubMenuLabel);
                                item.DirectEvents.Click.Url = this.Url.Action(subMenu.SubMenuPath, subMenu.Menu.Module.ModuleArea + "/" + subMenu.SubMenuController);
                                /*
                                item.DirectEvents.Click.Url = this.Url.Action("ServletFaces", "Dental");
                                item.DirectEvents.Click.ExtraParams.Add(new Parameter("Curent_Page", subMenu.SubMenuPath, ParameterMode.Value));
                                item.DirectEvents.Click.ExtraParams.Add(new Parameter("Curent_Controller", subMenu.Menu.Module.ModuleArea + "/" + subMenu.SubMenuController, ParameterMode.Value));
                                */
                                //item.DirectEvents.Click.Url = this.Url.Action(subMenu.SubMenuPath, subMenu.Menu.Module.ModuleArea + "/" + subMenu.SubMenuController);
                                subMenuButton.Add(item);
                            }
                            button.Menu.Add(subMenuButton);
                        }
                        else
                        {
                            /*
                            button.DirectEvents.Click.Url = this.Url.Action("ServletFaces", "Dental");
                            button.DirectEvents.Click.ExtraParams.Add(new Parameter("Curent_Page", menu.MenuPath, ParameterMode.Value));
                            button.DirectEvents.Click.ExtraParams.Add(new Parameter("Curent_Controller", menu.Module.ModuleArea + "/" + menu.MenuController, ParameterMode.Value));
                            button.DirectEvents.Click.ExtraParams.Add(new Parameter("param", menu.MenuPath, ParameterMode.Value));
                            */
                            button.DirectEvents.Click.Url = this.Url.Action(menu.MenuPath, menu.Module.ModuleArea + "/" + menu.MenuController);
                        }
                        menuComponents.Add(button);
                    }

                    //Ext.Net.Button button2 = new Ext.Net.Button()
                    //{
                    //    Flat = true,
                    //    Text = "<span style = \"color :#0094ff;\">" + "Test" + "</span>",// + " " + moduleID,
                    //    IconCls = "menu-icon-cls",
                    //    //IconUrl = MENU_ICON_PATH + menu.MenuIconName,
                    //    IconAlign = IconAlign.Top,
                    //    ArrowAlign = ArrowAlign.Right,
                    //    Margin = 1
                    //};

                    //button2.DirectEvents.Click.Url = this.Url.Action("ServletFaces", "Dental");
                    //button2.DirectEvents.Click.ExtraParams.Add(new Parameter("Curent_Page", "Test", ParameterMode.Value));
                    //button2.DirectEvents.Click.ExtraParams.Add(new Parameter("Curent_Controller", "Sale/Test", ParameterMode.Value));
                    //button2.DirectEvents.Click.ExtraParams.Add(new Parameter("param", "Test", ParameterMode.Value));
                    //menuComponents.Add(button2);
                }
                return this.ComponentConfig(menuComponents.AsEnumerable());
            }
            catch (Exception e)
            {

                X.Msg.Alert("Menus Of Modules Error", e.Message + " " + e.InnerException).Show();
                return this.Direct();
            }
            
        }
        /**
         * This method return a partial view where a menus of one module will be loaded. 
         * It call a partial view.
         * the parameter "param" contain a module that are cliked
        */
        public ActionResult MenuContainer(string moduleID)
        {
            if (moduleID != null)
            {
                Ext.Net.MVC.PartialViewResult partialViewOfMenus = new Ext.Net.MVC.PartialViewResult()
                {
                    ContainerId = DEFAULT_CONTAINER_BODY,
                    ViewName = SHARED_MENU_VIEW,
                    ClearContainer = true,
                };
                partialViewOfMenus.ViewBag.module = moduleID;
                return partialViewOfMenus;
            }
            else
            {
                _currentUser = _userRepository.FindAll.FirstOrDefault(u => u.GlobalPersonID == int.Parse(HttpContext.User.Identity.Name));
                ActionMenuProfile defaultModule = _profileMenuRepository.FindAll.First(pm => pm.ProfileID == _currentUser.ProfileID);

                Ext.Net.MVC.PartialViewResult defaultView = new Ext.Net.MVC.PartialViewResult
                {
                    ContainerId = DEFAULT_CONTAINER_BODY,
                    ViewName = SHARED_MENU_VIEW,
                    WrapByScriptTag = false
                };
                defaultView.ViewBag.module = defaultModule.Menu.ModuleID;
                return defaultView;
            }
        }
    }
}