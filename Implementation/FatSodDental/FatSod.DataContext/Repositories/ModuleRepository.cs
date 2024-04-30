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
    public class ModuleRepository : Repository<Module>, IModule
    {
        
        public IEnumerable<Security.Entities.Module> Modules
        {
            get { return base.context.Modules; }
        }

        public void SaveChanges(Security.Entities.Module module)
        {
            throw new NotImplementedException();
        }

        public Security.Entities.Module Delete(int moduleID)
        {
            throw new NotImplementedException();
        }

        /*protected  override void Dispose(bool disposing)
        {
            if (disposing == true)
            {
                base.context.Dispose();
            }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }*/
    }
}
