using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FatSod.Security.Abstracts;
using FatSod.DataContext.Concrete;

namespace FatSod.DataContext.Repositories
{
    public class MenuRepository : IMenu, IDisposable
    {
        private EFDbContext context = new EFDbContext();
        public IEnumerable<Security.Entities.Menu> Menus
        {
            get { return context.Menus; }
        }

        public void SaveChanges(Security.Entities.Menu menu)
        {
            throw new NotImplementedException();
        }

        public Security.Entities.Menu Delete(int menuID)
        {
            throw new NotImplementedException();
        }
        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing == true)
            {
                context.Dispose();
                //context = null;
            }
        }
        ~MenuRepository()
        {
            Dispose(false);
        }
    }
}
