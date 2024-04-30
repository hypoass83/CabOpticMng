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
    public class ActionMenuProfileRepository : Repository<ActionMenuProfile>, IActionMenuProfile
    {

        public void Update2(ActionMenuProfile actionProfileMenu)
        {
            ActionMenuProfile actionMenuProfileToUpdate = context.ActionMenuProfiles.Find(actionProfileMenu.ActionMenuProfileID);
            actionMenuProfileToUpdate.Add = actionProfileMenu.Add;
            actionMenuProfileToUpdate.Delete = actionProfileMenu.Delete;
            actionMenuProfileToUpdate.Update = actionProfileMenu.Update;
            context.SaveChanges();
        }
    }
}
