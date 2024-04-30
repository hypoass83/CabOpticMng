using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FatSod.Security.Entities;

namespace FatSod.Security.Abstracts
{
    public interface IActionSubMenuProfile : IRepository<ActionSubMenuProfile>
    {
        void Update2(ActionSubMenuProfile actionProfileSubMenu);
    }
}
