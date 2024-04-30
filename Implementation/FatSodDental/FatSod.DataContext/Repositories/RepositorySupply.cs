using System.Collections.Generic;
using System.Linq;
using FatSod.DataContext.Concrete;
using System.Data.Entity;
using System;

namespace FatSod.DataContext.Repositories
{
    /// <summary>
    /// RepositorySupply
    /// </summary>
    /// <typeparam name="H"></typeparam>
    public class RepositorySupply<H> : FatSod.Supply.Abstracts.IRepositorySupply<H> where H : class
    {
        protected EFDbContext context;
        private bool _disposed;
        private DbSet<H> _table;

        public RepositorySupply(EFDbContext ctxt)
        {
            context = ctxt;
            _table = context.Set<H>();
        }

        public RepositorySupply()
        {
            context = new EFDbContext();
            _table = context.Set<H>();
        }
        public IEnumerable<H> FindAll
        {
            get
            {
                return _table.ToList();
            }
        }

       public IEnumerable<H> FindAllForStore 
        {
            get
            {
                
                context.Dispose();
                List<H> res = new List<H>();
                using (context = new EFDbContext()){
                    _table = context.Set<H>();

                    context.Configuration.ProxyCreationEnabled = false;
                    context.Configuration.LazyLoadingEnabled = false;

                    res = _table.ToList();
                }
                return res;
            }
        }

        public H Update(H obj)
        {
            _table.Attach(obj);
            context.Entry(obj).State = EntityState.Modified;
            save();
            return obj;
        }

        public H Create(H obj)
        {
            _table.Add(obj);
            save();
            return obj;
        }
        public H Find(int key)
        {
            return context.Set<H>().Find(key);
        }
        public H Update(H updated, int key)
        {
            if (updated == null)
                return null;
            H existing = context.Set<H>().Find(key);
            if (existing != null)
            {
                context.Entry(existing).CurrentValues.SetValues(updated);
                save();
            }
            return existing;
        }

        public H Delete(H obj)
        {
            _table.Remove(obj);
            save();
            return obj;
        }

        public void Delete(int key)
        {
            _table.Remove(Find(key));
            save();
        }

        public IEnumerable<H> CreateAll(IEnumerable<H> tList)
        {
            context.Set<H>().AddRange(tList);
            context.SaveChanges();
            return tList;
        }
        public IEnumerable<H> DeleteAll(IEnumerable<H> tList)
        {
            foreach (H t in tList)
            {
                this.Delete(t);
            }
            return tList;
        }

        /*public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing == true)
            {
                context.Dispose();
            }
        }*/
        private void save()
        {
            context.SaveChanges();
        }
    }
}
