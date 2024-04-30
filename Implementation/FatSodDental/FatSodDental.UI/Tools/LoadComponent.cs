using System;
using System.Collections.Generic;
using System.Linq;
using FatSod.DataContext.Concrete;
using FatSod.Security.Entities;
using Ext.Net;
using FatSod.DataContext.Initializer;
using FatSod.Ressources;
using FatSod.Supply.Entities;
using System.Globalization;

namespace FatSodDental.UI.Tools
{
    public static partial class LoadComponent
    {
        private static EFDbContext context = new EFDbContext();

        private static CultureInfo enUsCI = CultureInfo.GetCultureInfo("en-US");

        //This const are the path of content images folders
        private const string MOD_PRESSED_IMG_PATH = "../Content/Images/App/Modules/Pressed/";
        private const string MOD_IMG_PATH = "../Content/Images/App/Modules/";
        private const string MOD_DISABLED_IMG_PATH = "../Content/Images/App/Modules/Pressed/";
        private const string MENU_ICON_PATH = "../Content/Images/App/Menus/";

        /*public static NodeCollection Liste_Menu_SecureBackDate(User userConnect, CultureInfo culture)
        {
            try
            {
                int cptemodule = 0;
                int cpteMenu = 0;
                int cpteurSMenu = 0;
                
                // -- Initialisation de l'arbre à retourner à l'application -- //
                NodeCollection Liste = new Ext.Net.NodeCollection();
                //recuperation des modules
                var moduleUser = context.Modules.Join(context.Menus, mo => mo.ModuleID, me => me.ModuleID, (mo, me) => new { mo, me })
                    .Join(context.ActionMenuProfiles, mome => mome.me.MenuID, ap => ap.MenuID, (mome, ap) => new { mome, ap })
                    .Where(m => m.ap.ProfileID == userConnect.ProfileID)
                    .Select(s => new
                    {
                        ModuleCode = s.mome.mo.ModuleCode,
                        ModuleID = s.mome.mo.ModuleID
                    }).Distinct().ToList();
                if (moduleUser != null)
                {
                    foreach (var module in moduleUser.OrderBy(m => m.ModuleCode))
                    {
                        cptemodule = cptemodule + 1;
                        Node ModuleNode = new Node(); // Assiduité du personnel
                        ModuleNode.NodeID = "ModuleNode" + cptemodule;
                        ModuleNode.Text = "<span style = \"color :#0094ff;\">" + Resources.ResourceManager.GetString(module.ModuleCode, culture) != null ? Resources.ResourceManager.GetString(module.ModuleCode, culture) : module.ModuleCode + "</span>";
                        ModuleNode.Icon = Icon.UserSuitBlack;

                        //recuperation des modules de l'user connecte
                        
                            var menusOfModule = context.ActionMenuProfiles.Join(context.Menus, a => a.MenuID, m => m.MenuID,
                                (a, m) => new { a, m })
                                .Where(am => am.a.ProfileID == userConnect.ProfileID && am.m.ModuleID == module.ModuleID
                                    && am.a.BackDate)
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
                                    Module = s.m.Module,
                                    SubMenu = s.m.SubMenus
                                })
                                .ToList();
                        
                        
                        if (menusOfModule != null)
                        {
                            foreach (var menu in menusOfModule.OrderBy(m => m.MenuCode))
                            {
                                cpteMenu = cpteMenu + 1;
                                Node MenuNode = new Node();
                                MenuNode.NodeID = "MenuNode" + cpteMenu;
                                MenuNode.Text = "<span style = \"color :#0094ff;\">" + Resources.ResourceManager.GetString(menu.MenuCode, culture) != null ? Resources.ResourceManager.GetString(menu.MenuCode, culture) : menu.MenuLabel + "</span>";
                                //MenuNode.IconFile = MENU_ICON_PATH + menu.MenuIconName;
                                MenuNode.Icon = Icon.PackageGo;
                                if (menu.SubMenu.Count == 0)
                                {
                                    MenuNode.Href = menu.Module.ModuleArea + "/" + menu.MenuController + "/" + menu.MenuPath;
                                    MenuNode.Leaf = true;
                                    MenuNode.Icon = Icon.ApplicationGo;
                                }
                                // -------------------------- Ajout du menu ------------------------------- //
                                ModuleNode.Children.Add(MenuNode);

                                //recuperation des sous menu
                                //recuperation des modules de l'user connecte
                                var subMenus = context.ActionSubMenuProfiles.Join(context.SubMenus, am => am.SubMenuID, sm => sm.SubMenuID,
                                (am, sm) => new { am, sm })
                                .Where(asm => asm.sm.MenuID == menu.MenuID && asm.am.ProfileID == userConnect.ProfileID && asm.am.BackDate)
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

                                foreach (var submenu in subMenus.OrderBy(s => s.SubMenuCode))
                                {
                                    cpteurSMenu = cpteurSMenu + 1;
                                    Node SubMenu = new Node();
                                    SubMenu.NodeID = "SubMenu" + cpteurSMenu;
                                    SubMenu.Text = "<span style = \"color :#0094ff;\">" + Resources.ResourceManager.GetString(submenu.SubMenuCode, culture) != null ? Resources.ResourceManager.GetString(submenu.SubMenuCode, culture) : submenu.SubMenuLabel + "</span>";
                                    //SubMenu.IconFile = MENU_ICON_PATH + submenu.MenuIconName;
                                    SubMenu.Icon = Icon.ApplicationGo;
                                    SubMenu.Href = submenu.Menu.Module.ModuleArea + "/" + submenu.SubMenuController + "/" + submenu.SubMenuPath;
                                    SubMenu.Leaf = true;

                                    // -------------------------- Ajout du sous-menu ------------------------------- //
                                    MenuNode.Children.Add(SubMenu);
                                }
                            }

                        }
                        // -------------------------- Ajout du module ------------------------------- //
                        Liste.Add(ModuleNode);
                    }
                }

                return Liste;
            }
            catch (Exception e)
            {

                X.Msg.Alert("Menus Of Modules Error", e.Message + " " + e.InnerException).Show();
                return new Ext.Net.NodeCollection();
            }
        }*/
        //load menu
        public static NodeCollection Liste_Menu_Secure(User userConnect, CultureInfo culture)
        {
            try
            {
                int cptemodule = 0;
                int cpteMenu = 0;
                int cpteurSMenu = 0;

               
            
            // -- Initialisation de l'arbre à retourner à l'application -- //
            NodeCollection Liste = new Ext.Net.NodeCollection();
            //recuperation des modules
            var moduleUser = context.Modules.Join(context.Menus, mo => mo.ModuleID, me => me.ModuleID, (mo, me) => new { mo, me })
                .Join(context.ActionMenuProfiles, mome => mome.me.MenuID, ap => ap.MenuID, (mome, ap) => new { mome, ap })
                .Where(m => m.ap.ProfileID == userConnect.ProfileID)
                .Select(s => new
                {
                    ModuleCode = s.mome.mo.ModuleCode,
                    ModuleID = s.mome.mo.ModuleID
                }).Distinct().ToList();
            if (moduleUser != null)
            {
                foreach (var module in moduleUser.OrderBy(m => m.ModuleCode))
                {
                    cptemodule = cptemodule + 1;
                    Node ModuleNode = new Node(); // Assiduité du personnel
                    ModuleNode.NodeID = "ModuleNode"+cptemodule;
                    ModuleNode.Text = "<span style = \"color :#0094ff;\">" + Resources.ResourceManager.GetString(module.ModuleCode, culture) != null ? Resources.ResourceManager.GetString(module.ModuleCode, culture) : module.ModuleCode + "</span>";
                    ModuleNode.Icon = Icon.UserSuitBlack;
                    
                    //recuperation des modules de l'user connecte
                    var menusOfModule = context.ActionMenuProfiles.Join(context.Menus, a => a.MenuID, m => m.MenuID,
                            (a, m) => new { a, m })
                            .Where(am => am.a.ProfileID == userConnect.ProfileID && am.m.ModuleID==module.ModuleID)
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
                                Module = s.m.Module,
                                SubMenu = s.m.SubMenus
                            })
                            .ToList();
                    if (menusOfModule != null)
                    {
                        foreach (var menu in menusOfModule.OrderBy(m => m.MenuCode))
                        {
                            cpteMenu = cpteMenu + 1;
                            Node MenuNode = new Node(); 
                            MenuNode.NodeID = "MenuNode" + cpteMenu;
                            MenuNode.Text = "<span style = \"color :#0094ff;\">" + Resources.ResourceManager.GetString(menu.MenuCode, culture) != null ? Resources.ResourceManager.GetString(menu.MenuCode, culture) : menu.MenuLabel + "</span>";
                            //MenuNode.IconFile = MENU_ICON_PATH + menu.MenuIconName;
                            MenuNode.Icon = Icon.PackageGo;
                            if (menu.SubMenu.Count==0)
                            {
                                MenuNode.Href = menu.Module.ModuleArea + "/" + menu.MenuController+"/"+menu.MenuPath;
                                MenuNode.Leaf = true;
                                MenuNode.Icon = Icon.ApplicationGo;
                            }
                            // -------------------------- Ajout du menu ------------------------------- //
                            ModuleNode.Children.Add(MenuNode);
                            
                            //recuperation des sous menu
                            //recuperation des modules de l'user connecte
                            var subMenus = context.ActionSubMenuProfiles.Join(context.SubMenus, am => am.SubMenuID, sm => sm.SubMenuID,
                            (am, sm) => new { am, sm })
                            .Where(asm => asm.sm.MenuID == menu.MenuID && asm.am.ProfileID == userConnect.ProfileID)
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

                            foreach (var submenu in subMenus.OrderBy(s => s.SubMenuCode))
                            {
                                cpteurSMenu = cpteurSMenu + 1;
                                Node SubMenu = new Node();
                                SubMenu.NodeID = "SubMenu" + cpteurSMenu;
                                SubMenu.Text = "<span style = \"color :#0094ff;\">" + Resources.ResourceManager.GetString(submenu.SubMenuCode, culture) != null ? Resources.ResourceManager.GetString(submenu.SubMenuCode, culture) : submenu.SubMenuLabel + "</span>";
                                //SubMenu.IconFile = MENU_ICON_PATH + submenu.MenuIconName;
                                SubMenu.Icon = Icon.ApplicationGo;
                                SubMenu.Href = submenu.Menu.Module.ModuleArea + "/" + submenu.SubMenuController + "/" + submenu.SubMenuPath;
                                SubMenu.Leaf = true;
                                
                                // -------------------------- Ajout du sous-menu ------------------------------- //
                                MenuNode.Children.Add(SubMenu);
                            }
                        }

                    }
                    // -------------------------- Ajout du module ------------------------------- //
                    Liste.Add(ModuleNode);
                }
            }
            
            return Liste;
            }
            catch (Exception e)
            {

                X.Msg.Alert("Menus Of Modules Error", e.Message + " " + e.InnerException).Show();
                return new Ext.Net.NodeCollection();
            }
        }
        public static List<ListItem> GetAllSphericalRanges
        {
            get
            {
                context = new EFDbContext();
                List<ListItem> rangeList = new List<ListItem>();
                List<LensNumberRange> ranges = context.LensNumberRanges.ToList().Where(lnr => Convert.ToDecimal(lnr.Minimum, enUsCI) >= -20.00m && Convert.ToDecimal(lnr.Minimum, enUsCI) <= +20.00m &&
                                                                                     Convert.ToDecimal(lnr.Maximum, enUsCI) >= -20.00m && Convert.ToDecimal(lnr.Maximum, enUsCI) <= +20.00m).ToList();
                foreach (LensNumberRange lnr in ranges)
                {
                    rangeList.Add(new ListItem(lnr.ToString(), lnr.LensNumberRangeID));
                }
                return rangeList;
            }
        }

        public static List<ListItem> GetAllCylindricalRanges
        {
            get
            {
                context = new EFDbContext();
                List<ListItem> rangeList = new List<ListItem>();
                List<LensNumberRange> ranges = context.LensNumberRanges.ToList().Where(lnr => (Convert.ToDecimal(lnr.Minimum, enUsCI) >= -6.00m && Convert.ToDecimal(lnr.Minimum, enUsCI) <= +6.00m) &&
                                                                                      (Convert.ToDecimal(lnr.Maximum, enUsCI) >= -6.00m && Convert.ToDecimal(lnr.Maximum, enUsCI) <= +6.00m)).ToList();
                foreach (LensNumberRange lnr in ranges)
                {
                    rangeList.Add(new ListItem(lnr.ToString(), lnr.LensNumberRangeID));
                }
                return rangeList;
            }
        }

        public static List<ListItem> GetAllAdditionRanges
        {
            get
            {
                context = new EFDbContext();
                List<ListItem> rangeList = new List<ListItem>();
                List<LensNumberRange> ranges = context.LensNumberRanges.ToList().Where(lnr => Convert.ToDecimal(lnr.Minimum, enUsCI) >= -4.00m && Convert.ToDecimal(lnr.Minimum, enUsCI) <= +4.00m &&
                                                                                     Convert.ToDecimal(lnr.Maximum, enUsCI) >= -4.00m && Convert.ToDecimal(lnr.Maximum, enUsCI) <= +4.00m).ToList();
                foreach (LensNumberRange lnr in ranges)
                {
                    rangeList.Add(new ListItem(lnr.ToString(), lnr.LensNumberRangeID));
                }
                return rangeList;
            }
        }

        public static List<ListItem> Users
        {
            get
            {
                context = new EFDbContext();
                List<ListItem> userList = new List<ListItem>();
                foreach (User user in context.People.OfType<User>().Where(u => u.IsConnected && u.UserAccessLevel <= 5).ToArray())
                {
                    userList.Add(new ListItem(user.UserFullName, user.GlobalPersonID));
                }
                return userList;
            }
        }

        /// <summary>
        /// Cette méthode retourne la liste des employés de l'entreprise
        /// </summary>
        public static List<ListItem> Employees
        {
            get
            {
                context = new EFDbContext();
                List<ListItem> userList = new List<ListItem>();
                foreach (User user in context.People.OfType<User>().ToArray())
                {
                    if (user.UserAccessLevel < 5)
                    {
                    userList.Add(new ListItem(user.Name + " " + user.Description, user.GlobalPersonID));
                }

                }
                return userList;
            }
        }

        /*======= Job list */
        public static List<ListItem> Jobs
        {
            get
            {
                context = new EFDbContext();
                List<ListItem> jobList = new List<ListItem>();
                foreach (Job job in context.Jobs.ToArray())
                {
                    jobList.Add(new ListItem(job.JobLabel, job.JobID));
                }
                return jobList;
            }
        }
        /*======= Profiles list */
        public static List<ListItem> Profiles
        {
            get
            {
                context = new EFDbContext();
                List<ListItem> profilesList = new List<ListItem>();
                foreach (Profile profile in context.Profiles.Where(p =>  p.ProfileCode != CodeValue.Security.Profile.ClASS_CODE && p.ProfileCode != CodeValue.Security.Profile.SUPER_ADMIN_PROFILE).ToArray())
                {
                    profilesList.Add(new ListItem(profile.ProfileLabel, profile.ProfileID));
                }
                return profilesList;
            }
        }
        /*======= Branchs list */
        public static List<ListItem> Branchs(int userID)
        {
            /*get
            {*/
            context = new EFDbContext();
            List<UserBranch> userBranchList = context.UserBranches.Where(ub => ub.UserID == userID).ToList();
            List<ListItem> branchsList = new List<ListItem>();
            userBranchList.ForEach(ub => branchsList.Add(new ListItem(ub.Branch.BranchName, ub.Branch.BranchID)));
            return branchsList;
            //}
        }
        /*======= Quarters list */
        public static List<ListItem> Quarters
        {
            get
            {
                context = new EFDbContext();
                List<ListItem> quartersList = new List<ListItem>();
                foreach (Quarter quarter in context.Quarters.ToArray())
                {
                    quartersList.Add(new ListItem(quarter.QuarterLabel, quarter.QuarterID));
                }
                return quartersList;
            }
        }
        /*======= Town list */
        public static List<ListItem> Towns
        {
            get
            {
                context = new EFDbContext();
                List<ListItem> townsList = new List<ListItem>();
                foreach (Town town in context.Towns.ToArray())
                {
                    townsList.Add(new ListItem(town.TownLabel, town.TownID));
                }
                return townsList;
            }
        }
        /*======= Town countries */
        public static List<ListItem> Countries
        {
            get
            {
                context = new EFDbContext();
                List<ListItem> countriesList = new List<ListItem>();
                foreach (Country country in context.Countries.ToArray())
                {
                    countriesList.Add(new ListItem(country.CountryLabel, country.CountryID));
                }
                return countriesList;
            }
        }

        public static List<ListItem> LensMaterials
        {
            get
            {
                context = new EFDbContext();
                List<ListItem> materialList = new List<ListItem>();
                foreach (LensMaterial lm in context.LensMaterials.ToArray())
                {
                    materialList.Add(new ListItem(lm.LensMaterialLabel, lm.LensMaterialCode));
                }
                return materialList;
            }
        }

        public static List<ListItem> LensCoatings
        {
            get
            {
                context = new EFDbContext();
                List<ListItem> coatinglList = new List<ListItem>();
                foreach (LensCoating lc in context.LensCoatings.Where(cl => cl.LensCoatingCode != CodeValue.Supply.DefaultLensCoating).ToArray())
                {
                    coatinglList.Add(new ListItem(lc.LensCoatingLabel, lc.LensCoatingCode));
                }
                return coatinglList;
            }
        }

        public static List<ListItem> LensColours
        {
            get
            {
                context = new EFDbContext();
                List<ListItem> colourList = new List<ListItem>();
                foreach (LensColour lc in context.LensColours.Where(cl => cl.LensColourCode != CodeValue.Supply.DefaultLensColour).ToArray())
                {
                    colourList.Add(new ListItem(lc.LensColourLabel, lc.LensColourCode));
                }
                return colourList;
            }
        }

        public static List<ListItem> LensNumbers
        {
            get
            {
                context = new EFDbContext();
                List<ListItem> numberList = new List<ListItem>();
                foreach (LensNumber lc in context.LensNumbers.ToArray())
                {
                    numberList.Add(new ListItem(lc.LensNumberFullCode, lc.LensNumberFullCode));
                }
                return numberList;
            }
        }

        public static List<ListItem> LensOtherCriterials
        {
            get
            {
                context = new EFDbContext();
                List<ListItem> numberList = new List<ListItem>();
                string Transition = "Transition";
                string PGX = "PGX";
                string PBX = "PBX";

                numberList.Add(new ListItem(Transition, Transition));
                numberList.Add(new ListItem(PGX, PGX));
                numberList.Add(new ListItem(PBX, PBX));

                return numberList;
            }
        }

        public static List<ListItem> LensBifocalCodes
        {
            get
            {
                context = new EFDbContext();
                List<ListItem> numberList = new List<ListItem>();
               
                numberList.Add(new ListItem("RoundTop", "SR"));
                numberList.Add(new ListItem("DTop", "SD"));

                return numberList;
            }
        }



        /*======= Town regions */
        public static List<ListItem> Regions
        {
            get
            {
                context = new EFDbContext();
                List<ListItem> regionsList = new List<ListItem>();
                foreach (FatSod.Security.Entities.Region region in context.Regions.ToArray())
                {
                    regionsList.Add(new ListItem(region.RegionLabel, region.RegionID));
                }
                return regionsList;
            }
        }
        /*======= Menus checkbox for allow set profile */
        public static List<CheckboxGroup> MenusAndSubMenus(int profileID) 
        {
            context = new EFDbContext();
            //List<ActionMenuProfile> menuActionProfiles = context.ActionMenuProfiles.Where(aMP => aMP.ProfileID == profileID).ToList();
            List<Module> listModuleProfile = context.Modules.OrderBy(m => m.ModuleCode).ToList();
            //List<Module> listModuleProfile = new List<Module>();
            //foreach (var modulesTmp in menuActionProfiles)
            //{
            //    //if (!listModuleProfile.Contains(modulesTmp.Menu.Module))
            //    //{
            //    //    listModuleProfile.Add(modulesTmp.Menu.Module);
            //    //}
            //    if (!listModuleProfile.Contains(modulesTmp.ModuleID))
            //    {
            //        listModuleProfile.Add(modulesTmp.ModuleID);
            //    }
            //}
            /*get
            {*/
            List<CheckboxGroup> moduleMenuList = new List<CheckboxGroup>();
            UInt32 allMenus = 0;
            UInt32 allSubMenus = 0;
            foreach (FatSod.Security.Entities.Module mod in listModuleProfile/*context.Modules.ToArray()*/)
            {
                CheckboxGroup module = new CheckboxGroup()
                {
                    ID = mod.ModuleLabel + "" + mod.ModuleID,
                    FieldLabel = "<span style=\"color:#000000; font-weight: bold;\">" + Resources.ResourceManager.GetString(mod.ModuleCode) /*mod.ModuleLabel.ToUpper()*/ + "</span>",
                    LabelCls = "module-cls",                   
                    LabelSeparator = "",                  
                    Layout = "" + LayoutType.Column,
                    AnchorHorizontal = "-18",
                    StyleSpec = "border-bottom: 0.7em solid #808080; margin: 7px 20px 20px 40px;"
                };
                //Allocation menus list
                //List<ActionMenuProfile> menuActionProfilesTmp = menuActionProfiles.Where(acMP => acMP.Menu.ModuleID == mod.ModuleID).ToList();
                List<FatSod.Security.Entities.Menu> listMenuProfile = new List<FatSod.Security.Entities.Menu>();
                listMenuProfile = context.Menus.Where(m => m.ModuleID == mod.ModuleID).OrderBy(m => m.MenuCode).ToList();
                //menuActionProfilesTmp.ForEach(menuAction => listMenuProfile.Add(menuAction.Menu));
                //------------------------------------ récupération des menus de ce module
                foreach (FatSod.Security.Entities.Menu menu in listMenuProfile/*context.Menus.Where(m => m.ModuleID == mod.ModuleID).ToArray()*/)
                {
                    Container container = new Container() { ColumnWidth = 0.15, Cls = "x-form-check-group-label" };
                    //we get all subMenu of this profile
                    //List<ActionSubMenuProfile> actionSubMenuProfilesTmp = context.ActionSubMenuProfiles.Where(actionSM => actionSM.SubMenu.MenuID == menu.MenuID && actionSM.ProfileID == profileID).ToList();
                    List<SubMenu> listSubMenuProfile = new List<SubMenu>();
                    listSubMenuProfile = context.SubMenus.Where(sm => sm.MenuID == menu.MenuID).OrderBy(m => m.SubMenuCode).ToList();
                    //actionSubMenuProfilesTmp.ForEach(asm => listSubMenuProfile.Add(asm.SubMenu));
                    //if (actionSubMenuProfilesTmp.Count() > 0)
                    if (listSubMenuProfile.Count()>0)
                    {
                        Component cmp = new Component()
                        {
                            Html = Resources.ResourceManager.GetString(menu.MenuCode),
                            Cls = "menu-label x-form-check-group-label"
                        };
                        container.Add(cmp);
                        foreach (SubMenu subMenu in listSubMenuProfile/*.Where(sm => sm.MenuID == menu.MenuID).ToArray()*/)
                        {
                            allSubMenus++;
                            Checkbox checkSubMenu = new Checkbox()
                            {
                                BoxLabel = Resources.ResourceManager.GetString(subMenu.SubMenuCode),
                                ID = "SubMenu" + subMenu.SubMenuID,
                                //Cls = "subMenu",
                                BoxLabelCls = "tooltip-cls",
                                //IndicatorText = Resources.ResourceManager.GetString(subMenu.SubMenuCode),
                                //IndicatorCls = "tooltip-cls",
                                BaseBodyCls = "box-base-body-cls",
                                InputValue = "" + subMenu.SubMenuID,
                                Name = "SubMenu" + allSubMenus
                            };
                            container.Add(checkSubMenu);

                        }
                    }
                    else
                    {
                        allMenus++;
                        Checkbox checkMenu = new Checkbox()
                        {
                            BoxLabel = Resources.ResourceManager.GetString(menu.MenuCode),
                            ID = "Menu" + menu.MenuID,
                            BoxLabelCls = "tooltip-cls",
                            //IndicatorText = Resources.ResourceManager.GetString(menu.MenuCode),
                            //IndicatorCls = "tooltip-cls",
                            BaseBodyCls = "box-base-body-cls",
                            InputValue = "" + menu.MenuID,
                            Name = "Menu" + allMenus
                        };
                        container.Add(checkMenu);
                    }
                    module.Add(container);
                }
                moduleMenuList.Add(module);
            }
            CheckboxGroup defaultModule = new CheckboxGroup()
            {
                ID = "defaultModule",
                Hidden = true
            };
            Checkbox defaultMenu = new Checkbox()
            {

                ID = "defaultMenu",
                InputValue = "" + allMenus,
                Checked = true,
                Hidden = true,
                Name = "defaultMenu"
            };
            Checkbox defaultSubMenu = new Checkbox()
            {

                ID = "defaultSubMenu",
                InputValue = "" + allSubMenus,
                Checked = true,
                Hidden = true,
                Name = "defaultSubMenu"
            };
            defaultModule.Add(defaultMenu);
            defaultModule.Add(defaultSubMenu);
            moduleMenuList.Add(defaultModule);
            return moduleMenuList;
            //}
        }
        //============ Load menus and submenu of profile gieve in parameters for allow or disabled actions
        public static List<Container> AllowActionToProfile(int profileID)
        {
            const string MENU_ICON_PATH = "../Content/Images/Icons/";
            EFDbContext em = new EFDbContext();
            List<FatSod.Security.Entities.ActionMenuProfile> actionMenuList = em.ActionMenuProfiles.Where(am => am.ProfileID == profileID).ToList();
            List<FatSod.Security.Entities.ActionSubMenuProfile> actionSubMenuList = em.ActionSubMenuProfiles.Where(aSm => aSm.ProfileID == profileID).ToList();
            List<Container> containerListMenu = new List<Container>();
            containerListMenu.Clear();
            //This 2 variables will content all menu and subMenu localizationID
            string menuItem = "0";
            string subMenuItem = "0";
            //Menu building
            foreach (var actionMenu in actionMenuList)
            {

                if (context.SubMenus.Where(sm => sm.MenuID == actionMenu.MenuID).Count() == 0)
                {
                    menuItem += "_" + actionMenu.ActionMenuProfileID;
                    Container contentMenu = new Container()
                    {
                        ColumnWidth = 0.15
                    };
                    Component cmpTitle = new Component()
                    {
                        Html = Resources.ResourceManager.GetString(actionMenu.Menu.MenuCode),
                        Cls = "menu-label x-form-check-group-label"
                    };
                    //Add action
                    Checkbox add = new Checkbox()
                    {
                        BoxLabel = Resources.AddCode,
                        Name = "MenuAdd" + actionMenu.ActionMenuProfileID,
                        IndicatorText = Resources.AddCode,
                        IndicatorCls = "tooltip-cls",
                        BoxLabelCls = "box-add-cls",
                        BaseBodyCls = "box-base-body-cls",
                        BoxLabelStyle = "content : url(../../Content/Images/Icons/opt.png)",
                        InputValue = "" + 1,
                        Checked = actionMenu.Add
                    };
                    //Update action
                    Checkbox update = new Checkbox()
                    {
                        BoxLabel = Resources.UpdateCode,
                        Name = "MenuUpdate" + actionMenu.ActionMenuProfileID,
                        IndicatorText = Resources.UpdateCode,
                        IndicatorCls = "tooltip-cls",
                        BaseBodyCls = "box-base-body-cls",
                        BoxLabelCls = "box-update-cls",
                        BoxLabelStyle = "content : url(../../Content/Images/Icons/pencil.png)",
                        InputValue = "" + 1,
                        Checked = actionMenu.Update
                    };
                    //Delete action
                    Checkbox delete = new Checkbox()
                    {
                        BoxLabel = Resources.DeleteCode,
                        Name = "MenuDelete" + actionMenu.ActionMenuProfileID,
                        IndicatorText = Resources.DeleteCode,
                        IndicatorCls = "tooltip-cls",
                        BaseBodyCls = "box-base-body-cls",
                        BoxLabelCls = "box-delete-cls",
                        BoxLabelStyle = "content : url(../../Content/Images/Icons/delete.png)",
                        InputValue = "" + 1,
                        Checked = actionMenu.Delete
                    };
                    //Backdate action
                    /*Checkbox Backdate = new Checkbox()
                    {
                        BoxLabel = Resources.Backdate,
                        Name = "MenuBackDate" + actionMenu.ActionMenuProfileID,
                        IndicatorText = Resources.Backdate,
                        IndicatorCls = "tooltip-cls",
                        BaseBodyCls = "box-base-body-cls",
                        BoxLabelCls = "box-BackDate-cls",
                        BoxLabelStyle = "content : url(../../Content/Images/Icons/BackDate.png)",
                        InputValue = "" + 1,
                        Checked = actionMenu.BackDate
                    };*/
                    //We add this action to contentMenu
                    contentMenu.Add(cmpTitle);
                    contentMenu.Add(add);
                    contentMenu.Add(update);
                    contentMenu.Add(delete);
                    //contentMenu.Add(Backdate);
                    containerListMenu.Add(contentMenu);
                }


            }
            //SubMenu building
            foreach (var actionSubMenu in actionSubMenuList)
            {
                subMenuItem += "_" + actionSubMenu.ActionSubMenuProfileID;
                Container contentSubMenu = new Container()
                {
                    ColumnWidth = 0.15
                };
                Component cmpTitle = new Component()
                {
                    Html = Resources.ResourceManager.GetString(actionSubMenu.SubMenu.SubMenuCode),
                    Cls = "menu-label x-form-check-group-label"
                };
                //Add action
                Checkbox add = new Checkbox()
                {
                    BoxLabel = Resources.AddCode,
                    Name = "SubMenuAdd" + actionSubMenu.ActionSubMenuProfileID,
                    //FieldCls = "field-cls add-cls",
                    //FieldBodyCls = "body-cls",
                    BaseBodyCls = "box-base-body-cls",
                    IndicatorText = Resources.AddCode,
                    IndicatorCls = "tooltip-cls",
                    BoxLabelCls = "box-add-cls",
                    BoxLabelStyle = "content : url(../Content/Images/Icons/opt.png)",
                    InputValue = "" + 1,
                    Checked = actionSubMenu.Add
                };
                //Update action
                Checkbox update = new Checkbox()
                {
                    BoxLabel = Resources.UpdateCode,
                    Name = "SubMenuUpdate" + actionSubMenu.ActionSubMenuProfileID,
                    //FieldCls = "field-cls update-cls",
                    IndicatorText = Resources.UpdateCode,
                    IndicatorCls = "tooltip-cls",
                    BaseBodyCls = "box-base-body-cls",
                    BoxLabelCls = "box-update-cls",
                    BoxLabelStyle = "content : url(../Content/Images/Icons/pencil.png)",
                    InputValue = "" + 1,
                    Checked = actionSubMenu.Update
                };
                //Delete action
                Checkbox delete = new Checkbox()
                {
                    BoxLabel = Resources.DeleteCode,
                    Name = "SubMenuDelete" + actionSubMenu.ActionSubMenuProfileID,
                    //FieldCls = "field-cls delete-cls",
                    IndicatorText = Resources.DeleteCode,
                    IndicatorCls = "tooltip-cls",
                    BaseBodyCls = "box-base-body-cls",
                    BoxLabelCls = "box-delete-cls",
                    BoxLabelStyle = "content : url(../Content/Images/Icons/delete.png)",
                    InputValue = "" + 1,
                    Checked = actionSubMenu.Delete
                };
                //BackDate action
                /*Checkbox BackDate = new Checkbox()
                {
                    BoxLabel = Resources.Backdate,
                    Name = "SubMenuBackDate" + actionSubMenu.ActionSubMenuProfileID,
                    //FieldCls = "field-cls delete-cls",
                    IndicatorText = Resources.Backdate,
                    IndicatorCls = "tooltip-cls",
                    BaseBodyCls = "box-base-body-cls",
                    BoxLabelCls = "box-BackDate-cls",
                    BoxLabelStyle = "content : url(../Content/Images/Icons/BackDate.png)",
                    InputValue = "" + 1,
                    Checked = actionSubMenu.BackDate
                };*/
                //We add this action to contentMenu
                contentSubMenu.Add(cmpTitle);
                contentSubMenu.Add(add);
                contentSubMenu.Add(update);
                contentSubMenu.Add(delete);
                //contentSubMenu.Add(BackDate);
                containerListMenu.Add(contentSubMenu);
            }
            //Container for defaults parameters
            Container defaultParameter = new Container() { Hidden = true };
            Checkbox allMenu = new Checkbox()
            {
                Hidden = true,
                Name = "ActionMenu",
                InputValue = menuItem,
                Checked = true
            };
            Checkbox allSubMenu = new Checkbox()
            {
                Hidden = true,
                Name = "ActionSubMenu",
                InputValue = subMenuItem,
                Checked = true
            };
            Checkbox profile = new Checkbox()
            {
                Hidden = true,
                Name = "Profile",
                InputValue = "" + profileID,
                Checked = true
            };
            defaultParameter.Add(profile);
            defaultParameter.Add(allMenu);
            defaultParameter.Add(allSubMenu);
            containerListMenu.Add(defaultParameter);
            return containerListMenu;
        }


        //retourne la liste des comptes en fonction d'un code representant le code de l'AccountingSection des comptes que nous voulons récupérer
        public static List<ListItem> Accounts(String code)
        {
            context = new EFDbContext();
            List<ListItem> accountingList = new List<ListItem>();
            List<Account> accounts = context.Accounts.Where(a => a.CollectifAccount.AccountingSection.AccountingSectionCode == code).ToList();
            foreach (Account account in accounts)
            {
                accountingList.Add(new ListItem(account.AccountLabel, account.AccountID));
            }
            return accountingList;
        }
        //retourne la liste des comptes collectif en fonction d'un code representant le code de l'AccountingSection des comptes que nous voulons récupérer
        public static List<ListItem> CollectifAccounts(String code)
        {
            context = new EFDbContext();
            List<ListItem> colAccountingList = new List<ListItem>();
            List<CollectifAccount> colAccounts = context.CollectifAccounts.Where(a => a.AccountingSection.AccountingSectionCode == code).ToList();
            foreach (CollectifAccount colAccount in colAccounts)
            {
                colAccountingList.Add(new ListItem(colAccount.CollectifAccountLabel, colAccount.CollectifAccountID));
            }
            return colAccountingList;
        }
        public static List<ListItem> AccountOfClass(int classe)
        {
            context = new EFDbContext();
            List<ListItem> accountingList = new List<ListItem>();
            List<Account> accounts = context.Accounts.Where(a => a.CollectifAccount.AccountingSection.ClassAccount.ClassAccountNumber == classe).ToList();
            foreach (Account account in accounts)
            {
                accountingList.Add(new ListItem(account.AccountLabel, account.AccountID));
            }
            return accountingList;
        }
        public static User getUser(int ID)
        {
            context = new EFDbContext();
            return context.Users.Where(u => u.GlobalPersonID == ID).FirstOrDefault();
        }

    }
}