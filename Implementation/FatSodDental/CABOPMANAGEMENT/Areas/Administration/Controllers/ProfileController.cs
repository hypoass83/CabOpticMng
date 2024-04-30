
using FatSod.DataContext.Concrete;
using FatSod.DataContext.Initializer;
using FatSod.Ressources;
using FatSod.Security.Abstracts;
using FatSod.Security.Entities;
using CABOPMANAGEMENT.Areas.Administration.Models;
using CABOPMANAGEMENT.Controllers;
using CABOPMANAGEMENT.Filters;
using CABOPMANAGEMENT.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using FatSod.Supply.Entities;

namespace CABOPMANAGEMENT.Areas.Administration.Controllers
{
    [Authorize(Order = 1)]
    //[TakeBusinessDay(Order = 2)]
    public class ProfileController : BaseController
    {
        private IRepository<Profile> profileRepository;
        private IProfile _profileRepository2;
        private IRepository<FatSod.Security.Entities.Menu> _menuRepository;
        private IUser _userRepository;
        private IRepository<SubMenu> _subMenuRepository;
        private IRepository<ActionMenuProfile> _actionMenuProfileRepository;
        private IActionMenuProfile _actionRepo2;
        private IActionSubMenuProfile _actionSubRepo2;
        private IRepository<ActionSubMenuProfile> _actionSubMenuProfileRepository;
        

        public ProfileController(IUser userRepository, IRepository<Profile> profileRepository, IRepository<ActionMenuProfile> actionMenuProfileRepository, IRepository<ActionSubMenuProfile> actionSubMenuProfileRepository, IRepository<FatSod.Security.Entities.Menu> menuRepository, IRepository<SubMenu> subMenuRepository, IProfile profileRepository2, IActionMenuProfile actionRepo2, IActionSubMenuProfile actionSubRepo2)
        {
            this._userRepository = userRepository;
            this.profileRepository = profileRepository;
            this._actionMenuProfileRepository = actionMenuProfileRepository;
            this._menuRepository = menuRepository;
            this._profileRepository2 = profileRepository2;
            this._actionSubMenuProfileRepository = actionSubMenuProfileRepository;
            this._subMenuRepository = subMenuRepository;
            this._actionRepo2 = actionRepo2;
            this._actionSubRepo2 = actionSubRepo2;


            this.LensNumberDescriptionFormat();

        }
        /**
         * @param listMenu contient les menu du profile courant
         * @param listSubMenu contient les sous-menus du profile courant
         * @param listMenuTmp et listSubMenu contiennent respectivement les menus et sous-menus d'un profile autre
         * 
         * 
        */
        // GET: Administration/Profile
        [OutputCache(Duration = 3600)] 
        public ActionResult Profile()
        {
            //string culture = (Resources.Culture == null) ? "en-US" : Resources.Culture.ToString();
            //Session["culture"] = culture;
            return View(ModelProfile);
            
        }


        private List<int> RequestMenu(int allMenu)
        {
            List<int> tabmenuIndex = new List<int>(); ;
            int menuIndex = 0;
            for (menuIndex = 1; menuIndex <= allMenu; menuIndex++)
            {
                int currentMenuID = 0;
                try
                {
                    currentMenuID = Convert.ToInt32(Request.Form["Menu" + menuIndex]);
                    tabmenuIndex.Add(currentMenuID);
                }
                catch (Exception e) { }
            }
            return tabmenuIndex;
        }
        private List<int> RequestSubMenu(int allSubMenu)
        {
            List<int> tabSubmenuIndex = new List<int>(); ;
            int SubmenuIndex = 0;
            for (SubmenuIndex = 1; SubmenuIndex <= allSubMenu; SubmenuIndex++)
            {
                int currentSubMenuID = 0;
                try
                {
                    currentSubMenuID = Convert.ToInt32(Request.Form["SubMenu" + SubmenuIndex]);
                    tabSubmenuIndex.Add(currentSubMenuID);
                }
                catch (Exception e) { }
            }
            return tabSubmenuIndex;
        }
        /// <summary>
        /// Allocate menu and submenu. This method allocate menu and subMenu to profile
        /// </summary>
        /// <param name="allMenu"></param>
        /// <param name="allSubMenu"></param>
        /// <param name="profileSave"></param>


        /// <summary>
        /// Enable to add new profile or update
        /// </summary>
        /// <param name="defaultSubMenu"></param>
        /// <param name="defaultMenu"></param>
        /// <param name="profile"></param>
        /// <returns>ActionResult</returns>
        public ActionResult Add(List<int> defaultSubMenu, List<int> defaultMenu, Profile profile, int ProfileState)
        {
            bool status = false;
            string Message = "";
            try
            {

                //int allSubMenu = Convert.ToInt32(defaultSubMenu);
                //int allMenu = Convert.ToInt32(defaultMenu);
                List<int> allSubMenu = (defaultSubMenu == null) ? new List<int>() : defaultSubMenu;
                List<int> allMenu = (defaultMenu == null) ? new List<int>() : defaultMenu;
                //bool res = _profileRepository2.AddProfile(this.RequestSubMenu(allSubMenu), this.RequestMenu(allMenu), profile, ProfileState);
                bool res = _profileRepository2.AddProfile(allSubMenu, allMenu, profile, ProfileState);

                //int menuIndex = 0;
                if (profile.ProfileID > 0)
                {
                    //We want to update profile
                    /*Profile profileSave = _profileRepository2.FindAll.SingleOrDefault(p => p.ProfileID == profile.ProfileID);
                    this.DeleteMenusAndSubMenu(profile.ProfileID);
                    this.AllocateMenuAndSubMenuToProfile(allMenu, allSubMenu, profileSave);
                    profileSave.ProfileCode = profile.ProfileCode;
                    profileSave.ProfileLabel = profile.ProfileLabel;
                    profileSave.ProfileDescription = profile.ProfileDescription;
                    profileSave.ProfileState = profile.ProfileState;
                    profileSave.PofilLevel = profile.PofilLevel;
                    _profileRepository2.Update(profileSave);*/
                    statusOperation = profile.ProfileLabel + " : " + Resources.AlertUpdateAction;
                }
                else
                {
                    //We want to create a new profile
                    //Profile profileSave = _profileRepository2.SaveChanges(profile);
                    statusOperation = profile.ProfileLabel + " : " + Resources.AlertAddAction;
                    //this.AllocateMenuAndSubMenuToProfile(allMenu, allSubMenu, profileSave);
                }
                
                status = true;
                Message = Resources.Success + "-" + statusOperation;
            }
            catch (Exception e)
            {
                status = false;
                Message = "Error " + e.Message;
            }
            return new JsonResult { Data = new { status = status, Message = Message } };
           
        }
        ///**
        // * @param ID of profile
        // * Return a list of checked menu and submenus of this profile 
        // * */
        public JsonResult CheckedMenus(string ID)
        {
            List<ModelProfile> modprof = new List<ModelProfile>();
            List<ModelCheckSubMenu> _ModelCheckSubMenuList = new List<ModelCheckSubMenu>();
            List<ModelCheckMenu> _ModelCheckMenuList = new List<ModelCheckMenu>();
            try
            {
                
                int profileID = Convert.ToInt32(ID);

                //On récupère le profile, ses menus te ses sous menus
                Profile profileToUpdate = profileRepository.FindAll.First(p => p.ProfileID == profileID);
                List<ActionMenuProfile> menuOfProfileToUpdate = _actionMenuProfileRepository.FindAll.Where(m => m.ProfileID == profileID).ToList();
                List<ActionSubMenuProfile> subMenuOfProfileToUpdate = _actionSubMenuProfileRepository.FindAll.Where(m => m.ProfileID == profileID).ToList();
                //On initialise toutes les cases avec les anciennes valeurs
               

                foreach (var actionM in menuOfProfileToUpdate)
                {
                    if (_subMenuRepository.FindAll.Where(sm => sm.MenuID == actionM.MenuID).Count() == 0)
                    {
                        
                        _ModelCheckMenuList.Add(new ModelCheckMenu
                        {
                            CheckMenuID = actionM.MenuID,
                            CheckMenuName = "Menu" + actionM.MenuID
                        });
                        
                    }
                }
                foreach (var actionSM in subMenuOfProfileToUpdate)
                {
                    
                    _ModelCheckSubMenuList.Add(new ModelCheckSubMenu
                    {
                        CheckSubMenuID = actionSM.SubMenuID,
                        CheckSubMenuName = "SubMenu" + actionSM.SubMenuID
                    });
                    
                }
                modprof.Add(new ModelProfile
                {
                    ProfileID = profileToUpdate.ProfileID,
                    ProfileCode = profileToUpdate.ProfileCode,
                    ProfileLabel = profileToUpdate.ProfileLabel,
                    ProfileDescription = profileToUpdate.ProfileDescription,
                    RadioAccess = profileToUpdate.PofilLevel,
                    RadioProfileState = (profileToUpdate.ProfileState) ? 1 : 0,
                    ModelCheckMenus = _ModelCheckMenuList,
                    ModelCheckSubMenus = _ModelCheckSubMenuList
                });

                }
            catch (Exception e)
            {
                TempData["alertType"] = "alert alert-danger";
                TempData["status"] = "Code Erreur " + e.Message + " : Impossible de satisfaire à la demande" + ID + ".";
            }
            return Json(modprof, JsonRequestBehavior.AllowGet);
        }
        /**
         * Return a list of profile
         * */
        //[OutputCache(Duration = 3600)] 
        public ActionResult Edit(int id, String profileCode)
        {
            Session["profilID"] = id;
            Session["profilCode"] = profileCode;
            return PartialView("AvancedProfileDetail");
        }
        public ActionResult AvancedProfile()
        {
            Session["profilID"] = 0;
            return View(ModelProfile);
            
        }
            
        private List<Profile> ModelProfile
        {
            get
            {
                List<Profile> model = new List<Profile>();
                profileRepository.FindAll.Where(p => p.PofilLevel < CurrentUser.UserAccessLevel/*p.ProfileCode != CodeValue.Security.Profile.ClASS_CODE && p.ProfileCode != CodeValue.Security.Profile.SUPER_ADMIN_PROFILE*/)
                .ToList().ForEach(c =>
                {
                   model.Add(
                        new Profile
                        {
                            ProfileID = c.ProfileID,
                            ProfileLabel = c.ProfileLabel,
                            ProfileCode = c.ProfileCode,
                            ProfileDescription = c.ProfileDescription,
                            PofilLevel = c.PofilLevel
                        }
                    );
                });
                return model;
            }
        }

        public JsonResult Delete(int ID)
        {
            bool status = false;
            string Message = "";
            try
            {
                _profileRepository2.Delete(ID);
                statusOperation = ID + " : " + Resources.AlertDeleteAction;
                status = true;
                Message = Resources.Success + "-" + statusOperation;
            }
            catch (Exception e)
            {
                status = false;
                Message = "Action denied " + "Code " + e.Message + " : Suppression du profile d'identifiant " + ID + " échouée.";
            }
            return new JsonResult { Data = new { status = status, Message = Message } };
        }
        
        [HttpPost]
        //****** This methode set a differents actions in a menu and subMenus of one profile
        public ActionResult AllowActionToProfile(String ActionMenu, String ActionSubMenu, string Profile)/*List<String> AddMenu, List<String> AddSubMenu, List<String> UpdateMenu, List<String> UpdateSubMenu, List<String> DeleteMenu, List<String> DeleteSubMenu,*/
        {

            //******************************************
            //Operation over Menu of profile
            //initialization of actionMenuProfiles number
            string[] listMenusProfileToUpdate = ActionMenu.Split('_');
            int actionMenusProfileNumber = 0;
            actionMenusProfileNumber = listMenusProfileToUpdate.Length;
            int currentActionMenuProfileIdINDEX = 1;
            while (currentActionMenuProfileIdINDEX < actionMenusProfileNumber)
            {
                //We get a current actionMenu in database
                int profileIDToUpdate = 0;
                profileIDToUpdate = Convert.ToInt32(listMenusProfileToUpdate[currentActionMenuProfileIdINDEX]);
                int addValue = 0;
                int updateValue = 0;
                int deleteValue = 0;
                //int backDateValue = 0;

                ActionMenuProfile actionMenu = _actionMenuProfileRepository.FindAll.First(am => am.ActionMenuProfileID == profileIDToUpdate);
                
                addValue = Convert.ToInt32(Request.Form["MenuAdd" + actionMenu.ActionMenuProfileID]);
                updateValue = Convert.ToInt32(Request.Form["MenuUpdate" + actionMenu.ActionMenuProfileID]);
                deleteValue = Convert.ToInt32(Request.Form["MenuDelete" + actionMenu.ActionMenuProfileID]);
                

                actionMenu.Add = Convert.ToBoolean(addValue);
                actionMenu.Update = Convert.ToBoolean(updateValue);
                actionMenu.Delete = Convert.ToBoolean(deleteValue);
                

                _actionMenuProfileRepository.Update(actionMenu);
                currentActionMenuProfileIdINDEX++;

            }
            //********************************
            string[] listSubMenusProfileToUpdate = ActionSubMenu.Split('_');
            //initialization of actionSubMenuProfiles number
            int actionSubMenusProfileNumber = 0;
            actionSubMenusProfileNumber = listSubMenusProfileToUpdate.Length;
            int currentActionSubMenuProfileIdINDEX = 1;
            //Operation over SubMenu of profile
            while (currentActionSubMenuProfileIdINDEX < actionSubMenusProfileNumber)
            {
                //We get a current actionSubMenu in database
                int subMenuProfileIDToUpdate = 0;
                subMenuProfileIDToUpdate = Convert.ToInt32(listSubMenusProfileToUpdate[currentActionSubMenuProfileIdINDEX]);
                int addValue = 0;
                int updateValue = 0;
                int deleteValue = 0;
                //int backDateValue = 0;
                ActionSubMenuProfile actionSubMenu = _actionSubMenuProfileRepository.FindAll.First(am => am.ActionSubMenuProfileID == subMenuProfileIDToUpdate);

                addValue = Convert.ToInt32(Request.Form["SubMenuAdd" + actionSubMenu.ActionSubMenuProfileID]);
                updateValue = Convert.ToInt32(Request.Form["SubMenuUpdate" + actionSubMenu.ActionSubMenuProfileID]);
                deleteValue = Convert.ToInt32(Request.Form["SubMenuDelete" + actionSubMenu.ActionSubMenuProfileID]);
                ////backDateValue = Convert.ToInt32(Request.Form["SubMenuBackDate" + actionSubMenu.ActionSubMenuProfileID]);

                actionSubMenu.Add = Convert.ToBoolean(addValue);
                actionSubMenu.Update = Convert.ToBoolean(updateValue);
                actionSubMenu.Delete = Convert.ToBoolean(deleteValue);
                //actionSubMenu.BackDate = Convert.ToBoolean(backDateValue);
                //We save changes
                //actionSubRepo2.Update2(actionSubMenu);
                _actionSubMenuProfileRepository.Update(actionSubMenu);
                currentActionSubMenuProfileIdINDEX++;
            }
            statusOperation = Resources.AlertUpdateAction;
            this.AlertSucces( statusOperation);
            return View("AvancedProfileDetail");// this.Direct();
        }
        //**** This method delete menus and subemus of one profile
        private void DeleteMenusAndSubMenu(int profileID)
        {
            foreach (var actionSM in _actionSubMenuProfileRepository.FindAll.Where(asm => asm.ProfileID == profileID))
            {
                _actionSubMenuProfileRepository.Delete(actionSM);
            }
            foreach (var actionM in _actionMenuProfileRepository.FindAll.Where(am => am.ProfileID == profileID))
            {
                _actionMenuProfileRepository.Delete(actionM);
            }
        }

        private void LensNumberDescriptionFormat()
        {
            foreach (LensNumber ln in db.LensNumbers)
            {
                if (ln.LensNumberDescription != ln.LensNumberFullCode)
                {
                    ln.LensNumberDescription = ln.LensNumberFullCode;
                }
            }
            db.SaveChanges();
        }
        //****************** This method load all Profile in data base
        //[HttpPost]
        //public StoreResult GetAllProfiles()
        //{
        //    //List<ActionMenuProfile> actionMenuOfCurrentProfile = new List<ActionMenuProfile>();
        //    //List<FatSod.Security.Entities.Menu> listMenu = new List<FatSod.Security.Entities.Menu>();
        //    //List<FatSod.Security.Entities.Menu> listMenuTmp = new List<FatSod.Security.Entities.Menu>();
        //    //List<FatSod.Security.Entities.SubMenu> listSubMenu = new List<FatSod.Security.Entities.SubMenu>();
        //    //List<FatSod.Security.Entities.SubMenu> listSubMenuTmp = new List<FatSod.Security.Entities.SubMenu>();

        //    //actionMenuOfCurrentProfile = CurrentUser.Profile.ActionMenuProfiles.ToList();

        //    //actionMenuOfCurrentProfile.ForEach(ac => listMenu.Add(_menuRepository.FindAll.First(m => m.MenuID == ac.Menu.MenuID)));

        //    //List<ActionSubMenuProfile> actionSubMenuOfCurrentProfile = new List<ActionSubMenuProfile>();
        //    //actionSubMenuOfCurrentProfile = CurrentUser.Profile.ActionSubMenuProfiles.ToList();

        //    //actionSubMenuOfCurrentProfile.ForEach(sac => listSubMenu.Add(_subMenuRepository.FindAll.First(sb => sb.SubMenuID == sac.SubMenu.SubMenuID)));
        //    List<object> model = new List<object>();
        //    //List<Profile> listProfile = new List<Profile>();
        //    List<Profile> listProfileTmp = new List<Profile>();
        //    listProfileTmp = profileRepository.FindAll.Where(p => p.PofilLevel < CurrentUser.UserAccessLevel /*p.ProfileCode != CodeValue.Security.Profile.ClASS_CODE && p.ProfileCode != CodeValue.Security.Profile.SUPER_ADMIN_PROFILE*/).ToList();
        //    listProfileTmp.ForEach(p =>
        //    {
        //        //status permet d'avoir l'état du test sur le menu précédent
        //        bool status = true;
        //        //listMenuTmp.Clear();
        //        //p.ActionMenuProfiles.ToList().ForEach(ac => listMenuTmp.Add(_menuRepository.FindAll.First(m => m.MenuID == ac.Menu.MenuID)));
        //        ////On se rassure avant que le nombre de menus contenus dans la liste des menus d'un profile 
        //        ////contenu dans la liste de profile est au plus égal à celui du profile connecté
        //        //if (listMenu.Count() >= listMenuTmp.Count())
        //        //{
        //        //    //Vérfication de la présence du menu dans la liste des menus du profile connecté
        //        //    listMenuTmp.ForEach(menuTmp =>
        //        //    {
        //        //        if (listMenu.Contains(menuTmp) && status)
        //        //        {
        //        //            status = true;
        //        //        }
        //        //        else
        //        //        {
        //        //            status = false;
        //        //        }
        //        //    });
        //        //}
        //        //else { status = false; }

        //        //listSubMenuTmp.Clear();
        //        //p.ActionSubMenuProfiles.ToList().ForEach(sm => listSubMenuTmp.Add(_subMenuRepository.FindAll.First(sb => sb.SubMenuID == sm.SubMenu.SubMenuID)));
        //        ///*  On se rassure avant que le nombre de sous-menus contenus dans la liste des menus d'un profile 
        //        // contenu dans la liste de profile est au plus égal à celui du profile connecté*/
        //        //if (listSubMenu.Count() >= listSubMenuTmp.Count())
        //        //{
        //        //    listSubMenuTmp.ForEach(subMenuTmp =>
        //        //    {
        //        //        //Vérfication de la présence du menu dans la liste des menus du profile connecté
        //        //        if (listSubMenu.Contains(subMenuTmp) && status)
        //        //        {
        //        //            status = true;
        //        //        }
        //        //        else
        //        //        {
        //        //            status = false;
        //        //        }
        //        //    });
        //        //}
        //        //else { status = false; }
        //        //*/
        //        //si le status est toujours bon, on ajoute ce profile dans la liste des profiles consultable par le profile connecté
        //        if (status)
        //        {
        //            //listProfile.Add(p);
        //            model.Add(
        //                                new
        //                                {
        //                                    ProfileID = p.ProfileID,
        //                                    ProfileLabel = p.ProfileLabel,
        //                                    ProfileDescription = p.ProfileDescription,
        //                                    ProfileCode = p.ProfileCode,
        //                                    PofilLevel = p.PofilLevel
        //                                }
        //                            );
        //        }
        //    }
        //     );
        //    //List<Profile> data = profileRepository.FindAll.ToList();
        //    return this.Store(model);
        //}




    }
}