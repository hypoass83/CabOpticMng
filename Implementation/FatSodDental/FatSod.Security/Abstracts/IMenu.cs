using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FatSod.Security.Entities;

namespace FatSod.Security.Abstracts
{
    public interface IMenu
    {
        IEnumerable<Menu> Menus { get; }
        void SaveChanges(Menu menu);
        Menu Delete(int menuID);
    }
}
