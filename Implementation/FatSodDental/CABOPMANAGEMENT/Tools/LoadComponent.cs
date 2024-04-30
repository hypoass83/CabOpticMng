using System;
using System.Collections.Generic;
using System.Linq;
using FatSod.DataContext.Concrete;
using FatSod.Security.Entities;
using FatSod.DataContext.Initializer;
using FatSod.Ressources;
using FatSod.Supply.Entities;
using System.Globalization;
using System.Threading;
using System.Text;
using System.Diagnostics;

namespace CABOPMANAGEMENT.Tools
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

        public static double ConvertToDouble(string s)
        {
            char systemSeparator = Thread.CurrentThread.CurrentCulture.NumberFormat.CurrencyDecimalSeparator[0];
            double result = 0;
            try
            {
                if (s != null)
                    if (!s.Contains(","))
                        result = double.Parse(s, CultureInfo.InvariantCulture);
                    else
                        result = Convert.ToDouble(s.Replace(".", systemSeparator.ToString()).Replace(",", systemSeparator.ToString()));
            }
            catch (Exception e)
            {
                try
                {
                    result = Convert.ToDouble(s);
                }
                catch
                {
                    try
                    {
                        result = Convert.ToDouble(s.Replace(",", ";").Replace(".", ",").Replace(";", "."));
                    }
                    catch
                    {
                        throw new Exception("Wrong string-to-double format");
                    }
                }
            }
            return result;
        }

        public static string Left(this string str, int length)
        {
            str = (str ?? string.Empty);
            return str.Substring(0, Math.Min(length, str.Length));
        }

        public static string Right(this string str, int length)
        {
            str = (str ?? string.Empty);
            return (str.Length >= length)
                ? str.Substring(str.Length - length, length)
                : str;
        }
        ////load menu
        //public static NodeCollection Liste_Menu_Secure(User userConnect, CultureInfo culture)
        //{
        //    try
        //    {
        //        int cptemodule = 0;
        //        int cpteMenu = 0;
        //        int cpteurSMenu = 0;



        //    // -- Initialisation de l'arbre à retourner à l'application -- //
        //    NodeCollection Liste = new Ext.Net.NodeCollection();
        //    //recuperation des modules
        //    var moduleUser = context.Modules.Join(context.Menus, mo => mo.ModuleID, me => me.ModuleID, (mo, me) => new { mo, me })
        //        .Join(context.ActionMenuProfiles, mome => mome.me.MenuID, ap => ap.MenuID, (mome, ap) => new { mome, ap })
        //        .Where(m => m.ap.ProfileID == userConnect.ProfileID)
        //        .Select(s => new
        //        {
        //            ModuleCode = s.mome.mo.ModuleCode,
        //            ModuleID = s.mome.mo.ModuleID
        //        }).Distinct().ToList();
        //    if (moduleUser != null)
        //    {
        //        foreach (var module in moduleUser.OrderBy(m => m.ModuleCode))
        //        {
        //            cptemodule = cptemodule + 1;
        //            Node ModuleNode = new Node(); // Assiduité du personnel
        //            ModuleNode.NodeID = "ModuleNode"+cptemodule;
        //            ModuleNode.Text = "<span style = \"color :#0094ff;\">" + Resources.ResourceManager.GetString(module.ModuleCode, culture) != null ? Resources.ResourceManager.GetString(module.ModuleCode, culture) : module.ModuleCode + "</span>";
        //            ModuleNode.Icon = Icon.UserSuitBlack;

        //            //recuperation des modules de l'user connecte
        //            var menusOfModule = context.ActionMenuProfiles.Join(context.Menus, a => a.MenuID, m => m.MenuID,
        //                    (a, m) => new { a, m })
        //                    .Where(am => am.a.ProfileID == userConnect.ProfileID && am.m.ModuleID==module.ModuleID)
        //                    .Select(s => new
        //                    {
        //                        MenuID = s.m.MenuID,
        //                        MenuCode = s.m.MenuCode,
        //                        MenuDescription = s.m.MenuDescription,
        //                        MenuController = s.m.MenuController,
        //                        //MenuState = s.m.MenuState,
        //                        MenuLabel = s.m.MenuLabel,
        //                        //MenuFlat = s.m.MenuFlat,
        //                        MenuIconName = s.m.MenuIconName,
        //                        MenuPath = s.m.MenuPath,
        //                        //IsChortcut = s.m.IsChortcut,
        //                        Module = s.m.Module,
        //                        SubMenu = s.m.SubMenus
        //                    })
        //                    .ToList();
        //            if (menusOfModule != null)
        //            {
        //                foreach (var menu in menusOfModule.OrderBy(m => m.MenuCode))
        //                {
        //                    cpteMenu = cpteMenu + 1;
        //                    Node MenuNode = new Node(); 
        //                    MenuNode.NodeID = "MenuNode" + cpteMenu;
        //                    MenuNode.Text = "<span style = \"color :#0094ff;\">" + Resources.ResourceManager.GetString(menu.MenuCode, culture) != null ? Resources.ResourceManager.GetString(menu.MenuCode, culture) : menu.MenuLabel + "</span>";
        //                    //MenuNode.IconFile = MENU_ICON_PATH + menu.MenuIconName;
        //                    MenuNode.Icon = Icon.PackageGo;
        //                    if (menu.SubMenu.Count==0)
        //                    {
        //                        MenuNode.Href = menu.Module.ModuleArea + "/" + menu.MenuController+"/"+menu.MenuPath;
        //                        MenuNode.Leaf = true;
        //                        MenuNode.Icon = Icon.ApplicationGo;
        //                    }
        //                    // -------------------------- Ajout du menu ------------------------------- //
        //                    ModuleNode.Children.Add(MenuNode);

        //                    //recuperation des sous menu
        //                    //recuperation des modules de l'user connecte
        //                    var subMenus = context.ActionSubMenuProfiles.Join(context.SubMenus, am => am.SubMenuID, sm => sm.SubMenuID,
        //                    (am, sm) => new { am, sm })
        //                    .Where(asm => asm.sm.MenuID == menu.MenuID && asm.am.ProfileID == userConnect.ProfileID)
        //                    .Select(s => new
        //                    {
        //                        SubMenuID = s.sm.SubMenuID,
        //                        SubMenuCode = s.sm.SubMenuCode,
        //                        SubMenuLabel = s.sm.SubMenuLabel,
        //                        SubMenuDescription = s.sm.SubMenuDescription,
        //                        SubMenuController = s.sm.SubMenuController,
        //                        SubMenuPath = s.sm.SubMenuPath,
        //                        //IsChortcut = s.sm.IsChortcut,
        //                        Menu = s.sm.Menu
        //                    })
        //                    .ToList();

        //                    foreach (var submenu in subMenus.OrderBy(s => s.SubMenuCode))
        //                    {
        //                        cpteurSMenu = cpteurSMenu + 1;
        //                        Node SubMenu = new Node();
        //                        SubMenu.NodeID = "SubMenu" + cpteurSMenu;
        //                        SubMenu.Text = "<span style = \"color :#0094ff;\">" + Resources.ResourceManager.GetString(submenu.SubMenuCode, culture) != null ? Resources.ResourceManager.GetString(submenu.SubMenuCode, culture) : submenu.SubMenuLabel + "</span>";
        //                        //SubMenu.IconFile = MENU_ICON_PATH + submenu.MenuIconName;
        //                        SubMenu.Icon = Icon.ApplicationGo;
        //                        SubMenu.Href = submenu.Menu.Module.ModuleArea + "/" + submenu.SubMenuController + "/" + submenu.SubMenuPath;
        //                        SubMenu.Leaf = true;

        //                        // -------------------------- Ajout du sous-menu ------------------------------- //
        //                        MenuNode.Children.Add(SubMenu);
        //                    }
        //                }

        //            }
        //            // -------------------------- Ajout du module ------------------------------- //
        //            Liste.Add(ModuleNode);
        //        }
        //    }

        //    return Liste;
        //    }
        //    catch (Exception e)
        //    {

        //        X.Msg.Alert("Menus Of Modules Error", e.Message + " " + e.InnerException).Show();
        //        return new Ext.Net.NodeCollection();
        //    }
        //}
        //public static List<LensNumberRange> GetAllSphericalRanges
        //{
        //    get
        //    {
        //        context = new EFDbContext();
        //        List<LensNumberRange> rangeList = new List<LensNumberRange>();
        //        List<LensNumberRange> ranges = context.LensNumberRanges.ToList().Where(lnr => Convert.ToDecimal(lnr.Minimum, enUsCI) >= -20.00m && Convert.ToDecimal(lnr.Minimum, enUsCI) <= +20.00m &&
        //                                                                             Convert.ToDecimal(lnr.Maximum, enUsCI) >= -20.00m && Convert.ToDecimal(lnr.Maximum, enUsCI) <= +20.00m).ToList();
        //        foreach (LensNumberRange lnr in ranges)
        //        {
        //            rangeList.Add(new LensNumberRange { LensNumberRangeName=lnr.ToString(), LensNumberRangeID = lnr.LensNumberRangeID });
        //        }
        //        return rangeList;
        //    }
        //}

        //public static List<LensNumberRange> GetAllCylindricalRanges
        //{
        //    get
        //    {
        //        context = new EFDbContext();
        //        List<LensNumberRange> rangeList = new List<LensNumberRange>();
        //        List<LensNumberRange> ranges = context.LensNumberRanges.ToList().Where(lnr => (Convert.ToDecimal(lnr.Minimum, enUsCI) >= -6.00m && Convert.ToDecimal(lnr.Minimum, enUsCI) <= +6.00m) &&
        //                                                                              (Convert.ToDecimal(lnr.Maximum, enUsCI) >= -6.00m && Convert.ToDecimal(lnr.Maximum, enUsCI) <= +6.00m)).ToList();
        //        foreach (LensNumberRange lnr in ranges)
        //        {
        //            rangeList.Add(new LensNumberRange { LensNumberRangeName=lnr.ToString(), LensNumberRangeID = lnr.LensNumberRangeID });
        //        }
        //        return rangeList;
        //    }
        //}

        //public static List<LensNumberRange> GetAllAdditionRanges
        //{
        //    get
        //    {
        //        context = new EFDbContext();
        //        List<LensNumberRange> rangeList = new List<LensNumberRange>();
        //        List<LensNumberRange> ranges = context.LensNumberRanges.ToList().Where(lnr => Convert.ToDecimal(lnr.Minimum, enUsCI) >= -4.00m && Convert.ToDecimal(lnr.Minimum, enUsCI) <= +4.00m &&
        //                                                                             Convert.ToDecimal(lnr.Maximum, enUsCI) >= -4.00m && Convert.ToDecimal(lnr.Maximum, enUsCI) <= +4.00m).ToList();
        //        foreach (LensNumberRange lnr in ranges)
        //        {
        //            rangeList.Add(new LensNumberRange { LensNumberRangeName=lnr.ToString(), LensNumberRangeID = lnr.LensNumberRangeID });
        //        }
        //        return rangeList;
        //    }
        //}

        public static List<User> Users
        {
            get
            {
                context = new EFDbContext();
                List<User> userList = new List<User>();
                foreach (User user in context.People.OfType<User>().Where(u => u.IsConnected && u.UserAccessLevel <= 5).ToArray())
                {
                    userList.Add(new User { Name=user.UserFullName, GlobalPersonID=user.GlobalPersonID });
                }
                return userList;
            }
        }

        /// <summary>
        /// Cette méthode retourne la liste des employés de l'entreprise
        /// </summary>
        public static List<User> Employees
        {
            get
            {
                context = new EFDbContext();
                List<User> userList = new List<User>();
                foreach (User user in context.People.OfType<User>().ToArray())
                {
                    if (user.UserAccessLevel < 5)
                    {
                        userList.Add(new User { Name=user.Name + " " + user.Description, GlobalPersonID = user.GlobalPersonID });
                }

                }
                return userList;
            }
        }

        /*======= Job list */
        public static List<Job> Jobs
        {
            get
            {
                context = new EFDbContext();
                List<Job> jobList = new List<Job>();
                foreach (Job job in context.Jobs.ToArray())
                {
                    jobList.Add(new Job { JobLabel = job.JobLabel, JobID=job.JobID });
                }
                return jobList;
            }
        }
        /*======= Profiles list */
        public static List<Profile> Profiles
        {
            get
            {
                context = new EFDbContext();
                List<Profile> profilesList = new List<Profile>();
                foreach (Profile profile in context.Profiles.Where(p =>  p.ProfileCode != CodeValue.Security.Profile.ClASS_CODE && p.ProfileCode != CodeValue.Security.Profile.SUPER_ADMIN_PROFILE).ToArray())
                {
                    profilesList.Add(new Profile { ProfileLabel=profile.ProfileLabel, ProfileID = profile.ProfileID });
                }
                return profilesList;
            }
        }
        /*======= Branchs list */
        public static List<Branch> Branchs(int userID)
        {
            /*get
            {*/
            context = new EFDbContext();
            List<UserBranch> userBranchList = context.UserBranches.Where(ub => ub.UserID == userID).ToList();
            List<Branch> branchsList = new List<Branch>();
            userBranchList.ForEach(ub => branchsList.Add(new Branch { BranchName=ub.Branch.BranchName, BranchID = ub.Branch.BranchID }));
            return branchsList;
            //}
        }
        /*======= Quarters list */
        public static List<Quarter> Quarters
        {
            get
            {
                context = new EFDbContext();
                List<Quarter> quartersList = new List<Quarter>();
                foreach (Quarter quarter in context.Quarters.ToArray())
                {
                    quartersList.Add(new Quarter { QuarterLabel=quarter.QuarterLabel, QuarterID = quarter.QuarterID });
                }
                return quartersList;
            }
        }
        /*======= Town list */
        public static List<Town> Towns
        {
            get
            {
                context = new EFDbContext();
                List<Town> townsList = new List<Town>();
                foreach (Town town in context.Towns.ToArray())
                {
                    townsList.Add(new Town {TownLabel= town.TownLabel, TownID=town.TownID });
                }
                return townsList;
            }
        }
        /*======= Town countries */
        public static List<Country> Countries
        {
            get
            {
                context = new EFDbContext();
                List<Country> countriesList = new List<Country>();
                foreach (Country country in context.Countries.ToArray())
                {
                    countriesList.Add(new Country {CountryLabel= country.CountryLabel, CountryID=country.CountryID });
                }
                return countriesList;
            }
        }

        public static List<LensMaterial> LensMaterials
        {
            get
            {
                context = new EFDbContext();
                List<LensMaterial> materialList = new List<LensMaterial>();
                foreach (LensMaterial lm in context.LensMaterials.ToArray())
                {
                    materialList.Add(new LensMaterial{LensMaterialLabel = lm.LensMaterialLabel, LensMaterialCode=lm.LensMaterialCode});
                }
                return materialList;
            }
        }

        public static List<LensCoating> LensCoatings
        {
            get
            {
                context = new EFDbContext();
                List<LensCoating> coatinglList = new List<LensCoating>();
                foreach (LensCoating lc in context.LensCoatings.Where(cl => cl.LensCoatingCode != CodeValue.Supply.DefaultLensCoating).ToArray())
                {
                    coatinglList.Add(new LensCoating { LensCoatingLabel=lc.LensCoatingLabel, LensCoatingCode = lc.LensCoatingCode });
                }
                return coatinglList;
            }
        }

        public static List<LensColour> LensColours
        {
            get
            {
                context = new EFDbContext();
                List<LensColour> colourList = new List<LensColour>();
                foreach (LensColour lc in context.LensColours.Where(cl => cl.LensColourCode != CodeValue.Supply.DefaultLensColour).ToArray())
                {
                    colourList.Add(new LensColour { LensColourLabel=lc.LensColourLabel, LensColourCode = lc.LensColourCode });
                }
                return colourList;
            }
        }

        public static List<LensNumber> LensNumbers
        {
            get
            {
                context = new EFDbContext();
                List<LensNumber> numberList = new List<LensNumber>();
                foreach (LensNumber lc in context.LensNumbers.ToArray())
                {
                    numberList.Add(new LensNumber { LensNumberDescription = lc.LensNumberDescription, LensNumberID = lc.LensNumberID });
                }
                return numberList;
            }
        }

        public static List<String> LensOtherCriterials
        {
            get
            {
                context = new EFDbContext();
                List<String> numberList = new List<String>();
                string Transition = "Transition";
                string PGX = "PGX";
                string PBX = "PBX";

                numberList.Add(Transition);
                numberList.Add(PGX);
                numberList.Add(PBX);

                return numberList;
            }
        }

        public static List<String> LensBifocalCodes
        {
            get
            {
                context = new EFDbContext();
                List<String> numberList = new List<String>();

                numberList.Add("SR");
                numberList.Add("SD");

                return numberList;
            }
        }



        /*======= Town regions */
        public static List<Region> Regions
        {
            get
            {
                context = new EFDbContext();
                List<Region> regionsList = new List<Region>();
                foreach (FatSod.Security.Entities.Region region in context.Regions.ToArray())
                {
                    regionsList.Add(new Region { RegionLabel=region.RegionLabel, RegionID = region.RegionID });
                }
                return regionsList;
            }
        }
        
        
        //suspend users
        public static List<User> SuspendedUsersForStore(User currentUser, int currentBranch)
        {

            context = new EFDbContext();
            List<UserBranch> userBranchListTmp = context.UserBranches.Where(ub => ub.UserID == currentUser.GlobalPersonID && ub.BranchID == currentBranch).ToList();
            List<User> userListFinal = new List<FatSod.Security.Entities.User>();
            foreach (UserBranch ub1 in userBranchListTmp)
            {
                List<UserBranch> userBrTmp = context.UserBranches.Where(ub => ub.BranchID == ub1.BranchID).AsParallel().ToList();
                foreach (UserBranch ub2 in userBrTmp)
                {
                    User u = context.People.OfType<User>().FirstOrDefault(uu => uu.GlobalPersonID == ub2.UserID);
                    if (u != null && (!u.IsConnected || !u.UserAccountState))
                    {
                        userListFinal.Add(ub2.User);
                    }
                }
            }
            return userListFinal;
        }
        ///*======= Menus checkbox for allow set profile */
        //public static List<CheckboxGroup> MenusAndSubMenus(int profileID) 
        //{
        //    context = new EFDbContext();
        //    //List<ActionMenuProfile> menuActionProfiles = context.ActionMenuProfiles.Where(aMP => aMP.ProfileID == profileID).ToList();
        //    List<Module> listModuleProfile = context.Modules.OrderBy(m => m.ModuleCode).ToList();
        //    //List<Module> listModuleProfile = new List<Module>();
        //    //foreach (var modulesTmp in menuActionProfiles)
        //    //{
        //    //    //if (!listModuleProfile.Contains(modulesTmp.Menu.Module))
        //    //    //{
        //    //    //    listModuleProfile.Add(modulesTmp.Menu.Module);
        //    //    //}
        //    //    if (!listModuleProfile.Contains(modulesTmp.ModuleID))
        //    //    {
        //    //        listModuleProfile.Add(modulesTmp.ModuleID);
        //    //    }
        //    //}
        //    /*get
        //    {*/
        //    List<CheckboxGroup> moduleMenuList = new List<CheckboxGroup>();
        //    UInt32 allMenus = 0;
        //    UInt32 allSubMenus = 0;
        //    foreach (FatSod.Security.Entities.Module mod in listModuleProfile/*context.Modules.ToArray()*/)
        //    {
        //        CheckboxGroup module = new CheckboxGroup()
        //        {
        //            ID = mod.ModuleLabel + "" + mod.ModuleID,
        //            FieldLabel = "<span style=\"color:#000000; font-weight: bold;\">" + mod.ModuleLabel.ToUpper() + "</span>",
        //            LabelCls = "module-cls",                   
        //            LabelSeparator = "",                  
        //            Layout = "" + LayoutType.Column,
        //            AnchorHorizontal = "-18",
        //            StyleSpec = "border-bottom: 0.7em solid #808080; margin: 7px 20px 20px 40px;"
        //        };
        //        //Allocation menus list
        //        //List<ActionMenuProfile> menuActionProfilesTmp = menuActionProfiles.Where(acMP => acMP.Menu.ModuleID == mod.ModuleID).ToList();
        //        List<FatSod.Security.Entities.Menu> listMenuProfile = new List<FatSod.Security.Entities.Menu>();
        //        listMenuProfile = context.Menus.Where(m => m.ModuleID == mod.ModuleID).OrderBy(m => m.MenuCode).ToList();
        //        //menuActionProfilesTmp.ForEach(menuAction => listMenuProfile.Add(menuAction.Menu));
        //        //------------------------------------ récupération des menus de ce module
        //        foreach (FatSod.Security.Entities.Menu menu in listMenuProfile/*context.Menus.Where(m => m.ModuleID == mod.ModuleID).ToArray()*/)
        //        {
        //            Container container = new Container() { ColumnWidth = 0.15, Cls = "x-form-check-group-label" };
        //            //we get all subMenu of this profile
        //            //List<ActionSubMenuProfile> actionSubMenuProfilesTmp = context.ActionSubMenuProfiles.Where(actionSM => actionSM.SubMenu.MenuID == menu.MenuID && actionSM.ProfileID == profileID).ToList();
        //            List<SubMenu> listSubMenuProfile = new List<SubMenu>();
        //            listSubMenuProfile = context.SubMenus.Where(sm => sm.MenuID == menu.MenuID).OrderBy(m => m.SubMenuCode).ToList();
        //            //actionSubMenuProfilesTmp.ForEach(asm => listSubMenuProfile.Add(asm.SubMenu));
        //            //if (actionSubMenuProfilesTmp.Count() > 0)
        //            if (listSubMenuProfile.Count()>0)
        //            {
        //                Component cmp = new Component()
        //                {
        //                    Html = Resources.ResourceManager.GetString(menu.MenuCode),
        //                    Cls = "menu-label x-form-check-group-label"
        //                };
        //                container.Add(cmp);
        //                foreach (SubMenu subMenu in listSubMenuProfile/*.Where(sm => sm.MenuID == menu.MenuID).ToArray()*/)
        //                {
        //                    allSubMenus++;
        //                    Checkbox checkSubMenu = new Checkbox()
        //                    {
        //                        BoxLabel = Resources.ResourceManager.GetString(subMenu.SubMenuCode),
        //                        ID = "SubMenu" + subMenu.SubMenuID,
        //                        //Cls = "subMenu",
        //                        BoxLabelCls = "tooltip-cls",
        //                        //IndicatorText = Resources.ResourceManager.GetString(subMenu.SubMenuCode),
        //                        //IndicatorCls = "tooltip-cls",
        //                        BaseBodyCls = "box-base-body-cls",
        //                        InputValue = "" + subMenu.SubMenuID,
        //                        Name = "SubMenu" + allSubMenus
        //                    };
        //                    container.Add(checkSubMenu);

        //                }
        //            }
        //            else
        //            {
        //                allMenus++;
        //                Checkbox checkMenu = new Checkbox()
        //                {
        //                    BoxLabel = Resources.ResourceManager.GetString(menu.MenuCode),
        //                    ID = "Menu" + menu.MenuID,
        //                    BoxLabelCls = "tooltip-cls",
        //                    //IndicatorText = Resources.ResourceManager.GetString(menu.MenuCode),
        //                    //IndicatorCls = "tooltip-cls",
        //                    BaseBodyCls = "box-base-body-cls",
        //                    InputValue = "" + menu.MenuID,
        //                    Name = "Menu" + allMenus
        //                };
        //                container.Add(checkMenu);
        //            }
        //            module.Add(container);
        //        }
        //        moduleMenuList.Add(module);
        //    }
        //    CheckboxGroup defaultModule = new CheckboxGroup()
        //    {
        //        ID = "defaultModule",
        //        Hidden = true
        //    };
        //    Checkbox defaultMenu = new Checkbox()
        //    {

        //        ID = "defaultMenu",
        //        InputValue = "" + allMenus,
        //        Checked = true,
        //        Hidden = true,
        //        Name = "defaultMenu"
        //    };
        //    Checkbox defaultSubMenu = new Checkbox()
        //    {

        //        ID = "defaultSubMenu",
        //        InputValue = "" + allSubMenus,
        //        Checked = true,
        //        Hidden = true,
        //        Name = "defaultSubMenu"
        //    };
        //    defaultModule.Add(defaultMenu);
        //    defaultModule.Add(defaultSubMenu);
        //    moduleMenuList.Add(defaultModule);
        //    return moduleMenuList;
        //    //}
        //}
        ////============ Load menus and submenu of profile gieve in parameters for allow or disabled actions
        //public static List<Container> AllowActionToProfile(int profileID)
        //{
        //    const string MENU_ICON_PATH = "../Content/Images/Icons/";
        //    EFDbContext em = new EFDbContext();
        //    List<FatSod.Security.Entities.ActionMenuProfile> actionMenuList = em.ActionMenuProfiles.Where(am => am.ProfileID == profileID).ToList();
        //    List<FatSod.Security.Entities.ActionSubMenuProfile> actionSubMenuList = em.ActionSubMenuProfiles.Where(aSm => aSm.ProfileID == profileID).ToList();
        //    List<Container> containerListMenu = new List<Container>();
        //    containerListMenu.Clear();
        //    //This 2 variables will content all menu and subMenu localizationID
        //    string menuItem = "0";
        //    string subMenuItem = "0";
        //    //Menu building
        //    foreach (var actionMenu in actionMenuList)
        //    {

        //        if (context.SubMenus.Where(sm => sm.MenuID == actionMenu.MenuID).Count() == 0)
        //        {
        //            menuItem += "_" + actionMenu.ActionMenuProfileID;
        //            Container contentMenu = new Container()
        //            {
        //                ColumnWidth = 0.15
        //            };
        //            Component cmpTitle = new Component()
        //            {
        //                Html = Resources.ResourceManager.GetString(actionMenu.Menu.MenuCode),
        //                Cls = "menu-label x-form-check-group-label"
        //            };
        //            //Add action
        //            Checkbox add = new Checkbox()
        //            {
        //                BoxLabel = Resources.AddCode,
        //                Name = "MenuAdd" + actionMenu.ActionMenuProfileID,
        //                IndicatorText = Resources.AddCode,
        //                IndicatorCls = "tooltip-cls",
        //                BoxLabelCls = "box-add-cls",
        //                BaseBodyCls = "box-base-body-cls",
        //                BoxLabelStyle = "content : url(../../Content/Images/Icons/opt.png)",
        //                InputValue = "" + 1,
        //                Checked = actionMenu.Add
        //            };
        //            //Update action
        //            Checkbox update = new Checkbox()
        //            {
        //                BoxLabel = Resources.UpdateCode,
        //                Name = "MenuUpdate" + actionMenu.ActionMenuProfileID,
        //                IndicatorText = Resources.UpdateCode,
        //                IndicatorCls = "tooltip-cls",
        //                BaseBodyCls = "box-base-body-cls",
        //                BoxLabelCls = "box-update-cls",
        //                BoxLabelStyle = "content : url(../../Content/Images/Icons/pencil.png)",
        //                InputValue = "" + 1,
        //                Checked = actionMenu.Update
        //            };
        //            //Delete action
        //            Checkbox delete = new Checkbox()
        //            {
        //                BoxLabel = Resources.DeleteCode,
        //                Name = "MenuDelete" + actionMenu.ActionMenuProfileID,
        //                IndicatorText = Resources.DeleteCode,
        //                IndicatorCls = "tooltip-cls",
        //                BaseBodyCls = "box-base-body-cls",
        //                BoxLabelCls = "box-delete-cls",
        //                BoxLabelStyle = "content : url(../../Content/Images/Icons/delete.png)",
        //                InputValue = "" + 1,
        //                Checked = actionMenu.Delete
        //            };
        //            //Backdate action
        //            /*Checkbox Backdate = new Checkbox()
        //            {
        //                BoxLabel = Resources.Backdate,
        //                Name = "MenuBackDate" + actionMenu.ActionMenuProfileID,
        //                IndicatorText = Resources.Backdate,
        //                IndicatorCls = "tooltip-cls",
        //                BaseBodyCls = "box-base-body-cls",
        //                BoxLabelCls = "box-BackDate-cls",
        //                BoxLabelStyle = "content : url(../../Content/Images/Icons/BackDate.png)",
        //                InputValue = "" + 1,
        //                Checked = actionMenu.BackDate
        //            };*/
        //            //We add this action to contentMenu
        //            contentMenu.Add(cmpTitle);
        //            contentMenu.Add(add);
        //            contentMenu.Add(update);
        //            contentMenu.Add(delete);
        //            //contentMenu.Add(Backdate);
        //            containerListMenu.Add(contentMenu);
        //        }


        //    }
        //    //SubMenu building
        //    foreach (var actionSubMenu in actionSubMenuList)
        //    {
        //        subMenuItem += "_" + actionSubMenu.ActionSubMenuProfileID;
        //        Container contentSubMenu = new Container()
        //        {
        //            ColumnWidth = 0.15
        //        };
        //        Component cmpTitle = new Component()
        //        {
        //            Html = Resources.ResourceManager.GetString(actionSubMenu.SubMenu.SubMenuCode),
        //            Cls = "menu-label x-form-check-group-label"
        //        };
        //        //Add action
        //        Checkbox add = new Checkbox()
        //        {
        //            BoxLabel = Resources.AddCode,
        //            Name = "SubMenuAdd" + actionSubMenu.ActionSubMenuProfileID,
        //            //FieldCls = "field-cls add-cls",
        //            //FieldBodyCls = "body-cls",
        //            BaseBodyCls = "box-base-body-cls",
        //            IndicatorText = Resources.AddCode,
        //            IndicatorCls = "tooltip-cls",
        //            BoxLabelCls = "box-add-cls",
        //            BoxLabelStyle = "content : url(../Content/Images/Icons/opt.png)",
        //            InputValue = "" + 1,
        //            Checked = actionSubMenu.Add
        //        };
        //        //Update action
        //        Checkbox update = new Checkbox()
        //        {
        //            BoxLabel = Resources.UpdateCode,
        //            Name = "SubMenuUpdate" + actionSubMenu.ActionSubMenuProfileID,
        //            //FieldCls = "field-cls update-cls",
        //            IndicatorText = Resources.UpdateCode,
        //            IndicatorCls = "tooltip-cls",
        //            BaseBodyCls = "box-base-body-cls",
        //            BoxLabelCls = "box-update-cls",
        //            BoxLabelStyle = "content : url(../Content/Images/Icons/pencil.png)",
        //            InputValue = "" + 1,
        //            Checked = actionSubMenu.Update
        //        };
        //        //Delete action
        //        Checkbox delete = new Checkbox()
        //        {
        //            BoxLabel = Resources.DeleteCode,
        //            Name = "SubMenuDelete" + actionSubMenu.ActionSubMenuProfileID,
        //            //FieldCls = "field-cls delete-cls",
        //            IndicatorText = Resources.DeleteCode,
        //            IndicatorCls = "tooltip-cls",
        //            BaseBodyCls = "box-base-body-cls",
        //            BoxLabelCls = "box-delete-cls",
        //            BoxLabelStyle = "content : url(../Content/Images/Icons/delete.png)",
        //            InputValue = "" + 1,
        //            Checked = actionSubMenu.Delete
        //        };
        //        //BackDate action
        //        /*Checkbox BackDate = new Checkbox()
        //        {
        //            BoxLabel = Resources.Backdate,
        //            Name = "SubMenuBackDate" + actionSubMenu.ActionSubMenuProfileID,
        //            //FieldCls = "field-cls delete-cls",
        //            IndicatorText = Resources.Backdate,
        //            IndicatorCls = "tooltip-cls",
        //            BaseBodyCls = "box-base-body-cls",
        //            BoxLabelCls = "box-BackDate-cls",
        //            BoxLabelStyle = "content : url(../Content/Images/Icons/BackDate.png)",
        //            InputValue = "" + 1,
        //            Checked = actionSubMenu.BackDate
        //        };*/
        //        //We add this action to contentMenu
        //        contentSubMenu.Add(cmpTitle);
        //        contentSubMenu.Add(add);
        //        contentSubMenu.Add(update);
        //        contentSubMenu.Add(delete);
        //        //contentSubMenu.Add(BackDate);
        //        containerListMenu.Add(contentSubMenu);
        //    }
        //    //Container for defaults parameters
        //    Container defaultParameter = new Container() { Hidden = true };
        //    Checkbox allMenu = new Checkbox()
        //    {
        //        Hidden = true,
        //        Name = "ActionMenu",
        //        InputValue = menuItem,
        //        Checked = true
        //    };
        //    Checkbox allSubMenu = new Checkbox()
        //    {
        //        Hidden = true,
        //        Name = "ActionSubMenu",
        //        InputValue = subMenuItem,
        //        Checked = true
        //    };
        //    Checkbox profile = new Checkbox()
        //    {
        //        Hidden = true,
        //        Name = "Profile",
        //        InputValue = "" + profileID,
        //        Checked = true
        //    };
        //    defaultParameter.Add(profile);
        //    defaultParameter.Add(allMenu);
        //    defaultParameter.Add(allSubMenu);
        //    containerListMenu.Add(defaultParameter);
        //    return containerListMenu;
        //}


        //retourne la liste des comptes en fonction d'un code representant le code de l'AccountingSection des comptes que nous voulons récupérer
        public static List<Account> Accounts(String code)
        {
            context = new EFDbContext();
            List<Account> accountingList = new List<Account>();
            List<Account> accounts = context.Accounts.Where(a => a.CollectifAccount.AccountingSection.AccountingSectionCode == code).ToList();
            foreach (Account account in accounts)
            {
                accountingList.Add(new Account { AccountLabel=account.AccountLabel, AccountID = account.AccountID });
            }
            return accountingList;
        }
        //retourne la liste des comptes collectif en fonction d'un code representant le code de l'AccountingSection des comptes que nous voulons récupérer
        public static List<CollectifAccount> CollectifAccounts(String code)
        {
            context = new EFDbContext();
            List<CollectifAccount> colAccountingList = new List<CollectifAccount>();
            List<CollectifAccount> colAccounts = context.CollectifAccounts.Where(a => a.AccountingSection.AccountingSectionCode == code).ToList();
            foreach (CollectifAccount colAccount in colAccounts)
            {
                colAccountingList.Add(new CollectifAccount { CollectifAccountLabel = colAccount.CollectifAccountLabel, CollectifAccountID=colAccount.CollectifAccountID });
            }
            return colAccountingList;
        }
        public static List<Account> AccountOfClass(int classe)
        {
            context = new EFDbContext();
            List<Account> accountingList = new List<Account>();
            List<Account> accounts = context.Accounts.Where(a => a.CollectifAccount.AccountingSection.ClassAccount.ClassAccountNumber == classe).ToList();
            foreach (Account account in accounts)
            {
                accountingList.Add(new Account { AccountLabel=account.AccountLabel, AccountID = account.AccountID });
            }
            return accountingList;
        }
        public static User getUser(int ID)
        {
            context = new EFDbContext();
            return context.Users.Where(u => u.GlobalPersonID == ID).FirstOrDefault();
        }

        public static bool IsGeneralPublic(Customer cust)
        {
            //Modification du representant pour adérer à la division des General Public
            string customerNumber = cust.AccountNumber.ToLower().TrimEnd().TrimStart();
            //verifions si c'est un cash customer
            string generalPublicNumber = CodeValue.Sale.Customer.GENERALPUBLICNUMBER.ToLower().TrimEnd().TrimStart();

            return customerNumber.Equals(generalPublicNumber);

        }

        public static string NumberToWords(int number)
        {
            if (number == 0)
                return "zero";

            if (number < 0)
                return "minus " + NumberToWords(Math.Abs(number));

            string words = "";

            if ((number / 1000000) > 0)
            {
                words += NumberToWords(number / 1000000) + " million ";
                number %= 1000000;
            }

            if ((number / 1000) > 0)
            {
                words += NumberToWords(number / 1000) + " thousand ";
                number %= 1000;
            }

            if ((number / 100) > 0)
            {
                words += NumberToWords(number / 100) + " hundred ";
                number %= 100;
            }

            if (number > 0)
            {
                if (words != "")
                    words += "and ";

                var unitsMap = new[] { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen" };
                var tensMap = new[] { "zero", "ten", "twenty", "thirty", "forty", "fifty", "sixty", "seventy", "eighty", "ninety" };

                if (number < 20)
                    words += unitsMap[number];
                else
                {
                    words += tensMap[number / 10];
                    if ((number % 10) > 0)
                        words += "-" + unitsMap[number % 10];
                }
            }

            return words;
        }

        public static string NumToWordBD(Int64 Num)
        {
            string[] Below20 = { "", "One ",
                    "Two ", "Three ", "Four ",
                    "Five ", "Six " , "Seven ",
                    "Eight ", "Nine ", "Ten ", "Eleven ",
                    "Twelve " , "Thirteen ", "Fourteen ","Fifteen ",
                    "Sixteen " , "Seventeen ","Eighteen " , "Nineteen " };
            string[] Below100 = { "", "", "Twenty ", "Thirty ",
                    "Forty ", "Fifty ", "Sixty ",
                    "Seventy ", "Eighty ", "Ninety " };
            string InWords = "";
            if (Num >= 1 && Num < 20)
                InWords += Below20[Num];
            if (Num >= 20 && Num <= 99)
                InWords += Below100[Num / 10] + Below20[Num % 10];
            if (Num >= 100 && Num <= 999)
                InWords += NumToWordBD(Num / 100) + " Hundred " + NumToWordBD(Num % 100);
            if (Num >= 1000 && Num <= 99999)
                InWords += NumToWordBD(Num / 1000) + " Thousand " + NumToWordBD(Num % 1000);
            if (Num >= 100000 && Num <= 9999999)
                InWords += NumToWordBD(Num / 100000) + " Hundred " + NumToWordBD(Num % 100000);
            if (Num >= 10000000)
                InWords += NumToWordBD(Num / 10000000) + " Hundred " + NumToWordBD(Num % 10000000);
            return InWords;
        }

        /// <summary> 
        /// Convertion d'un montant en toutes lettres /// 
        /// </summary> /// 
        /// <param name="values"></param> /// 
        /// <returns></returns> 

        public static String Int2Lettres(Int32 value)
        {
            //en cas de besoin pour vérifier l'orthographe //http://orthonet.sdv.fr/pages/lex_nombres.html 
            Int32 division, reste; StringBuilder sb;
            try {
                //Test l'état null 
                if (value == 0) return "zéro";
                //Décomposition de la valeur en milliards, millions, milliers,... 
                sb = new StringBuilder(); //milliard 
                division = Math.DivRem(value, 1000000000, out reste);
                if (division > 0) {
                    Int2LettresBloc(sb, division);
                    sb.Append(" milliard");
                    if (division > 1) sb.Append('s');
                }
                if (reste > 0) {
                    //million 
                    value = reste;
                    division = Math.DivRem(value, 1000000, out reste);
                    if (division > 0)
                    {
                        if (sb.Length > 0) sb.Append(' ');
                        Int2LettresBloc(sb, division);
                        sb.Append(" million");
                        if (division > 1) sb.Append('s');
                    }
                    if (reste > 0)
                    {
                        //milliers 
                        value = reste;
                        division = Math.DivRem(value, 1000, out reste);
                        if (division > 0)
                        {
                            if (sb.Length > 0) sb.Append(' ');
                            if (division == 1) sb.Append("mille");
                            else
                            {
                                Int2LettresBloc(sb, division);
                                sb.Append(" mille");
                            }
                        }
                        if (reste > 0)
                        {
                            //reste 
                            if (sb.Length > 0) sb.Append(' ');
                            Int2LettresBloc(sb, reste);
                        }
                    }
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return String.Empty;
            }
            finally
            {
                sb = null;
            }
        } 
        /// <summary> /// 
        /// Retourne la conversion d'un bloc de 3 bloc /// 
        /// </summary> /// 
        /// <param name="Value"></param> /// 
        /// <returns></returns> 
        private static void Int2LettresBloc(StringBuilder sb, Int32 value)
        {
            Boolean b_centaines; Int32 division, reste;
            try
            {
                division = Math.DivRem(value, 100, out reste);
                //Test si des centaines sont présentes 
                if (division > 0)
                {
                    //ajout des centaines à la sortie 
                    switch (division)
                    {
                        case 1: { sb.Append("cent"); break; }
                        default: { Int2LettresBase(sb, division); sb.Append(" cent"); break; }
                    }
                b_centaines = true;
                }
                else
                {
                    b_centaines = false;
                }
                //Test si il reste des éléments apres les centaines 
                if (reste > 0)
                {
                    //Introduction d'un espace si on a intégré des centaines 
                    if (b_centaines) sb.Append(' ');
                    //Calcul des dixaines et de leurs reste 
                    value = reste;
                    division = Math.DivRem(value, 10, out reste);
                    switch (division)
                    {
                        case 0:
                        case 1:
                        case 7:
                        case 9: { Int2LettresBase(sb, value); break; }
                        default:
                            {
                                Int2LettresBase(sb, division * 10);
                                if (reste > 0)
                                {
                                    if (reste == 1)
                                        sb.Append(" et un");
                                    else { sb.Append('-');
                                        Int2LettresBase(sb, reste);
                                    }
                                } break;
                            }
                    }
                }
            }
            catch (Exception ex) {
                Debug.WriteLine(ex.Message); }
        }
        private static void Int2LettresBase(StringBuilder sb, Int32 value)
        {
            switch (value)
            {
                case 0: { sb.Append("zéro"); break; }
                case 1: { sb.Append("un"); break; }
                case 2: { sb.Append("deux"); break; }
                case 3: { sb.Append("trois"); break; }
                case 4: { sb.Append("quatre"); break; }
                case 5: { sb.Append("cinq"); break; }
                case 6: { sb.Append("six"); break; }
                case 7: { sb.Append("sept"); break; }
                case 8: { sb.Append("huit"); break; }
                case 9: { sb.Append("neuf"); break; }
                case 10: { sb.Append("dix"); break; }
                case 11: { sb.Append("onze"); break; }
                case 12: { sb.Append("douze"); break; }
                case 13: { sb.Append("treize"); break; }
                case 14: { sb.Append("quatorze"); break; }
                case 15: { sb.Append("quinze"); break; }
                case 16: { sb.Append("seize"); break; }
                case 17: { sb.Append("dix-sept"); break; }
                case 18: { sb.Append("dix-huit"); break; }
                case 19: { sb.Append("dix-neuf"); break; }
                case 20: { sb.Append("vingt"); break; }
                case 30: { sb.Append("trente"); break; }
                case 40: { sb.Append("quarante"); break; }
                case 50: { sb.Append("cinquante"); break; }
                case 60: { sb.Append("soixante"); break; }
                case 70: { sb.Append("soixante-dix"); break; }
                case 71: { sb.Append("soixante et onze"); break; }
                case 72: { sb.Append("soixante-douze"); break; }
                case 73: { sb.Append("soixante-treize"); break; }
                case 74: { sb.Append("soixante-quatorze"); break; }
                case 75: { sb.Append("soixante-quinze"); break; }
                case 76: { sb.Append("soixante-seize"); break; }
                case 77: { sb.Append("soixante-dix-sept"); break; }
                case 78: { sb.Append("soixante-dix-huit"); break; }
                case 79: { sb.Append("soixante-dix-neuf"); break; }
                case 80: { sb.Append("quatre-vingt"); break; }
                case 90: { sb.Append("quatre-vingt-dix"); break; }
                case 91: { sb.Append("quatre-vingt-onze"); break; }
                case 92: { sb.Append("quatre-vingt-douze"); break; }
                case 93: { sb.Append("quatre-vingt-treize"); break; }
                case 94: { sb.Append("quatre-vingt-quatorze"); break; }
                case 95: { sb.Append("quatre-vingt-quinze"); break; }
                case 96: { sb.Append("quatre-vingt-seize"); break; }
                case 97: { sb.Append("quatre-vingt-dix-sept"); break; }
                case 98: { sb.Append("quatre-vingt-dix-huit"); break; }
                case 99: { sb.Append("quatre-vingt-dix-neuf"); break; }
                case 100: { sb.Append("cent"); break; }
                default: { /*RAS*/ break; }
            }
        }
        
    }
}