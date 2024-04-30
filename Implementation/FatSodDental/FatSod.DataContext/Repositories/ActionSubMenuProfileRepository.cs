using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FatSod.Security.Abstracts;
using FatSod.Security.Entities;
using FatSod.DataContext.Concrete;

namespace FatSod.DataContext.Repositories
{
    public class ActionSubMenuProfileRepository : Repository<ActionSubMenuProfile>, IActionSubMenuProfile
    {
        public void Update2(ActionSubMenuProfile actionProfileSubMenu)
        {
            ActionSubMenuProfile actionMenuProfileToUpdate = context.ActionSubMenuProfiles.Find(actionProfileSubMenu.ActionSubMenuProfileID);
            actionMenuProfileToUpdate.Add = actionProfileSubMenu.Add;
            actionMenuProfileToUpdate.Delete = actionProfileSubMenu.Delete;
            actionMenuProfileToUpdate.Update = actionProfileSubMenu.Update;
            context.SaveChanges();
        }
    }
}
