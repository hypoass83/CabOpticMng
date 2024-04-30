using System.Linq;
using FatSod.DataContext.Concrete;
using FatSod.DataContext.Initializer;
using FatSod.Security.Entities;
using System;

namespace CABOPMANAGEMENT.Tools
{
    public static class LoadAction
    {
        
        //*********** Load Action for menu
        
        private static bool subAction(MenuAction atcionName, int profileID, string menuCode, EFDbContext context)
        {
            ActionSubMenuProfile action = context.Profiles.Find(profileID).ActionSubMenuProfiles.Where(l => l.SubMenu.SubMenuCode == menuCode).ToList().FirstOrDefault();
          
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
            if (action != null)
                return true;
            else
                return false;
        }
        public static bool isAutoriseToGetSubMenu(int profileID, string menuCode, EFDbContext context)
        {
          
            ActionSubMenuProfile action = context.Profiles.Find(profileID).ActionSubMenuProfiles.Where(l => l.SubMenu.SubMenuCode == menuCode).ToList().FirstOrDefault();

             if (action != null)
                return true;
            else
                return false;
        }
    }
}