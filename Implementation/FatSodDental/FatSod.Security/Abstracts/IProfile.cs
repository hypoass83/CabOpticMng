using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FatSod.Security.Entities;

namespace FatSod.Security.Abstracts
{
    public interface IProfile : IRepository<Profile>
    {
        Profile SaveChanges(Profile profile);
        void Delete(int ID);
        bool AddProfile(List<int> allSubMenu, List<int> allMenu, Profile profile, int ProfileState);
    }
}
