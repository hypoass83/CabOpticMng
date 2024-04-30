using System.Collections.Generic;
using System.Linq;
using FatSod.DataContext.Concrete;
using System.Data.Entity;
using FatSod.Security.Abstracts;
using System;

namespace FatSod.DataContext.Repositories
{
    public class Repository<T> : FatSod.Security.Abstracts.IRepository<T> where T : class
    {
        protected EFDbContext context;
        private bool _disposed = false;
        private DbSet<T> _table;
        public Repository()
        {
            context = new EFDbContext();
            _table = context.Set<T>();
        }

        public T Find(int key)
        {
            return context.Set<T>().Find(key);
        }

        public IEnumerable<T> FindAll
        {
            get
            {
                //save();
                _table = context.Set<T>();
                return _table.ToList();


            }
        }
        public T Update(T obj)
        {
            _table.Attach(obj);
            context.Entry(obj).State = EntityState.Modified;
            save();
            return obj;
        }

        public T Update(T updated, int key)
        {
            if (updated == null)
                return null;

            T existing = context.Set<T>().Find(key);
            if (existing != null)
            {
                context.Entry(existing).CurrentValues.SetValues(updated);
                save();
            }
            return existing;
        }

        public T Create(T obj)
        {
            _table.Add(obj);
            save();
            return obj;
        }

        public IEnumerable<T> CreateAll(IEnumerable<T> tList)
        {
            context.Set<T>().AddRange(tList);
            context.SaveChanges();
            return tList;
        }

        public T Delete(T obj)
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

        public IEnumerable<T> DeleteAll(IEnumerable<T> tList)
        {
            foreach (T t in tList)
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

       public IEnumerable<T> FindAllForStore
        {
            get
            {

                context.Dispose();
                List<T> res = new List<T>();
                using (context = new EFDbContext())
                {
                    _table = context.Set<T>();

                    context.Configuration.ProxyCreationEnabled = false;
                    context.Configuration.LazyLoadingEnabled = false;

                    res = _table.ToList();
                }
                return res;
            }
        }

    }
}
