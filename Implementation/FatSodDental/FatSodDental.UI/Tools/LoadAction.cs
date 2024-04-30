using System.Linq;
using FatSod.DataContext.Concrete;
using FatSod.DataContext.Initializer;
using FatSod.Security.Entities;
using System;
using System.IO;
using System.Drawing;

namespace FatSodDental.UI.Tools
{
    public static class LoadAction
    {
        
        //*********** Load Action for menu
        
        private static bool subAction(MenuAction atcionName, int profileID, string menuCode, EFDbContext context)
        {
            ActionSubMenuProfile action = context.Profiles.Find(profileID).ActionSubMenuProfiles.Where(l => l.SubMenu.SubMenuCode == menuCode).ToList().FirstOrDefault();
            /*
            Profile currentProfile = context.Profiles.First(p => p.ProfileID == profileID);
            FatSod.Security.Entities.SubMenu subMenu = context.SubMenus.FirstOrDefault(m => m.SubMenuCode == menuCode);
            if (subMenu == null)
            {
                return false;
            }
            ActionSubMenuProfile action = context.ActionSubMenuProfiles.FirstOrDefault(a => a.SubMenuID == subMenu.SubMenuID && a.ProfileID == currentProfile.ProfileID);
            */
           // context.Dispose();
            if (action != null)
            {
                switch (atcionName)
                {
                    case MenuAction.ADD:
                        return !action.Add;
                    case MenuAction.DELETE:
                        return !action.Delete;
                    case MenuAction.UPDATE:
                        return !action.Update;
                    default:
                        return false;
                }
            }
            else { return true; }
        }
        private static bool menuAction(MenuAction atcionName, int profileID, string menuCode, EFDbContext context)
        {
            ActionMenuProfile action = context.Profiles.Find(profileID).ActionMenuProfiles.Where(l => l.Menu.MenuCode == menuCode).ToList().FirstOrDefault();
            
            if (action != null)
            {
                switch (atcionName)
                {
                    case MenuAction.ADD:
                        return !action.Add;
                    case MenuAction.DELETE:
                        return !action.Delete;
                    case MenuAction.UPDATE:
                        return !action.Update;
                    default:
                        return false;
                }
            }
            else { return true; }
            
        }
        
        //************* Load Action for Profile
        public static bool Profile(MenuAction atcionName, int profileID, EFDbContext context)
        {
            return menuAction(atcionName, profileID, CodeValue.Security.Profile.CODE,context);

        }
        //************** Load Action for user
        public static bool Utilisateur(MenuAction atcionName, int profileID, EFDbContext context)
        {
            return subAction(atcionName, profileID, CodeValue.Security.User.CODE,context);
        }
        //************** Load Action for user
        public static bool IsSubMenuActionAble(MenuAction atcionName, int profileID, string subMenuCode, EFDbContext context)
        {
            return subAction(atcionName, profileID, subMenuCode,context);
        }
        public static bool IsMenuActionAble(MenuAction atcionName, int profileID, string menuCode, EFDbContext context) 
        {
            return menuAction(atcionName, profileID, menuCode,context);
        }
       
        //************** Determine if the current user (profile) has one menu or subMenu
        public static bool isAutoriseToGetMenu(int profileID, string menuCode, EFDbContext context)
        {
            ActionMenuProfile action = context.Profiles.Find(profileID).ActionMenuProfiles.Where(l => l.Menu.MenuCode == menuCode).ToList().FirstOrDefault();
            /*
            Profile currentProfile = (from pro in context.Profiles
                                      where pro.ProfileID == profileID
                                      select pro).FirstOrDefault(); 
            //context.Profiles.FirstOrDefault(p => p.ProfileID == profileID);
            FatSod.Security.Entities.Menu menu = (from men in context.Menus
                                                  where men.MenuCode == menuCode
                                        select men).FirstOrDefault(); 
            //context.Menus.FirstOrDefault(m => m.MenuCode == menuCode);
            if (menu == null)
            {
                return false;
            }
            ActionMenuProfile action = (from amp in context.ActionMenuProfiles
                                        where amp.MenuID == menu.MenuID && amp.ProfileID == currentProfile.ProfileID
                                        select amp).FirstOrDefault(); */
            //context.ActionMenuProfiles.FirstOrDefault(a => a.MenuID == menu.MenuID && a.ProfileID == currentProfile.ProfileID);
            if (action != null)
                return true;
            else
                return false;
        }
        public static bool isAutoriseToGetSubMenu(int profileID, string menuCode, EFDbContext context)
        {
           
            //Profile currentProfile = context.Profiles.Find(profileID);

            ActionSubMenuProfile action = context.Profiles.Find(profileID).ActionSubMenuProfiles.Where(l => l.SubMenu.SubMenuCode == menuCode).ToList().FirstOrDefault();

            ////context.Profiles.FirstOrDefault(p => p.ProfileID == profileID);
            //FatSod.Security.Entities.SubMenu subMenu = (from subm in context.SubMenus
            //                                            where subm.SubMenuCode == menuCode
            //                                            select subm).FirstOrDefault(); 
            ////context.SubMenus.FirstOrDefault(m => m.SubMenuCode == menuCode);
            //if (subMenu == null)
            //{
            //    return false;
            //}
            //ActionSubMenuProfile action = (from asubmp in context.ActionSubMenuProfiles
            //                               where asubmp.SubMenuID == subMenu.SubMenuID && asubmp.ProfileID == currentProfile.ProfileID
            //                               select asubmp).FirstOrDefault(); 
            ////context.ActionSubMenuProfiles.FirstOrDefault(a => a.SubMenuID == subMenu.SubMenuID && a.ProfileID == currentProfile.ProfileID);
            if (action != null)
                return true;
            else
                return false;
        }
       
    }
}