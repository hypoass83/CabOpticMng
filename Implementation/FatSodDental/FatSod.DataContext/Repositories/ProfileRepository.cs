using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FatSod.Security.Abstracts;
using FatSod.Security.Entities;
using FatSod.DataContext.Concrete;
using System.Transactions;

namespace FatSod.DataContext.Repositories
{
    public class ProfileRepository : Repository<Profile>, IProfile
    {
        public Profile SaveChanges(Profile profile)
        {
            context.Profiles.Add(profile);
            context.SaveChanges();
            return profile;
        }
        public void Delete(int ID)
        {

                try
                {
                    using (TransactionScope ts = new TransactionScope())
                    {
                        Profile profileToDelete = context.Profiles.SingleOrDefault(p => p.ProfileID == ID);

                        this.DeleteMenusAndSubMenu(profileToDelete.ProfileID);
                        context.Profiles.Remove(profileToDelete);
                        context.SaveChanges();
                        //transaction.Commit();
                        ts.Complete();
                    }
                }
                catch (Exception e)
                {
                    //transaction.Rollback();
                    throw new Exception("We can't delete this profile. It maybe allocate to one user. If you think no, please try again or contact our administrator : "+e.Message);
                }
            //}

        }
        private void DeleteMenusAndSubMenu(int profileID)
        {
            foreach (var actionSM in context.ActionSubMenuProfiles.Where(asm => asm.ProfileID == profileID))
            {
                context.ActionSubMenuProfiles.Remove(actionSM);
            }
            foreach (var actionM in context.ActionMenuProfiles.Where(am => am.ProfileID == profileID))
            {
                context.ActionMenuProfiles.Remove(actionM);
            }
            context.SaveChanges();
        }

        public bool AddProfile(List<int> allSubMenu, List<int> allMenu, Profile profile, int ProfileState)
        {
            bool res = false;
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                //int allSubMenu = Convert.ToInt32(defaultSubMenu);
                //int allMenu = Convert.ToInt32(defaultMenu);
                profile.ProfileState = Convert.ToBoolean(ProfileState);
                //int menuIndex = 0;
                if (profile.ProfileID > 0)
                {
                    //We want to update profile
                    Profile profileSave = this.FindAll.SingleOrDefault(p => p.ProfileID == profile.ProfileID);
                    this.DeleteMenusAndSubMenu(profile.ProfileID);
                    this.AllocateMenuAndSubMenuToProfile(allMenu, allSubMenu, profileSave);
                    profileSave.ProfileCode = profile.ProfileCode;
                    profileSave.ProfileLabel = profile.ProfileLabel;
                    profileSave.ProfileDescription = profile.ProfileDescription;
                    profileSave.ProfileState = profile.ProfileState;
                    profileSave.PofilLevel = profile.PofilLevel;
                    this.Update(profileSave);
                    res = true;
                }
                else
                {
                    //We want to create a new profile
                    Profile profileSave = this.SaveChanges(profile);
                    this.AllocateMenuAndSubMenuToProfile(allMenu, allSubMenu, profileSave);
                    res = true;
                }
                ts.Complete();
                }
            }
            catch (Exception e)
            {
                res = false;
                throw new Exception("Error delete : " + e.Message);
            }
            
            return res;
        }
        /// <summary>
        /// Allocate menu and submenu. This method allocate menu and subMenu to profile
        /// </summary>
        /// <param name="allMenu"></param>
        /// <param name="allSubMenu"></param>
        /// <param name="profileSave"></param>
        /// 

        private void AllocateMenuAndSubMenuToProfile(List<int> allMenu, List<int> allSubMenu, Profile profileSave)
        {
            //int menuIndex = 0;
            //int subMenuIndex = 0;
            foreach (var currentMenuID in allMenu)
            {
                
                if (0 < currentMenuID)
                {
                    Menu Menu = context.Menus.Find(currentMenuID);
                    if (Menu != null)
                    {
                        ActionMenuProfile menuProfile = new ActionMenuProfile()
                        {
                            Add = true,
                            Delete = true,
                            Update = true,
                            MenuID = currentMenuID,
                            ProfileID = profileSave.ProfileID
                        };
                        context.ActionMenuProfiles.Add(menuProfile);
                        context.SaveChanges();
                    }
                }
            }
            //SubMenu Allocation
            //int subMenuIndex = 0;
            foreach (var currentSubMenuID in allSubMenu)
            {
                
                if (0 < currentSubMenuID)
                {
                    //** pour tous les sous menus alloués dans ActionSubMenuProfile, on crée également leurs menus parent dans ActionMenuProfile
                    SubMenu Submenu = context.SubMenus.Find(currentSubMenuID);
                    if (Submenu!=null)
                    {
                        ActionMenuProfile menuProfile = new ActionMenuProfile()
                        {
                            Add = true,
                            Delete = true,
                            Update = true,
                            MenuID = Submenu.MenuID,
                            ProfileID = profileSave.ProfileID
                        };
                        if (context.ActionMenuProfiles.Where(aMp => aMp.MenuID == Submenu.MenuID && aMp.ProfileID == profileSave.ProfileID).Count() == 0)
                        {
                            context.ActionMenuProfiles.Add(menuProfile);
                            context.SaveChanges();
                        }

                        //*********************

                        ActionSubMenuProfile subMenuProfile = new ActionSubMenuProfile()
                        {
                            Add = true,
                            Delete = true,
                            Update = true,
                            SubMenuID = currentSubMenuID,
                            ProfileID = profileSave.ProfileID
                        };
                        context.ActionSubMenuProfiles.Add(subMenuProfile);
                        context.SaveChanges();
                    }
                    
                    
                }
            }
            
        }
    }
}
