using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FatSod.Security.Entities;

namespace FatSod.Security.Abstracts
{
    public interface IModule : IRepository<Module>
    {
        IEnumerable<Module> Modules { get; }
        void SaveChanges(Module module);
        Module Delete(int moduleID);
    }
}
