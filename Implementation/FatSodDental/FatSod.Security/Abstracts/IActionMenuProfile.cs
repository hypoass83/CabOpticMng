using FatSod.Security.Entities;
using System;

namespace FatSod.Security.Abstracts
{
    public interface IActionMenuProfile : IRepository<ActionMenuProfile>
    {
        void Update2(ActionMenuProfile actionProfileMenu);
    }
}
