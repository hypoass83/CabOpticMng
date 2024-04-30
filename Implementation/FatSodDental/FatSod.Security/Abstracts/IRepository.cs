using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FatSod.Security.Abstracts
{

    public interface IRepository<T> where T : class
    {

        IEnumerable<T> FindAll { get; }
        T Update(T obj);
        T Create(T obj);
        T Delete(T obj);
        void Delete(int id);
        T Update(T updated, int key);
        T Find(int key);
        IEnumerable<T> CreateAll(IEnumerable<T> tList);
        IEnumerable<T> DeleteAll(IEnumerable<T> tList);
        IEnumerable<T> FindAllForStore { get; }

    }
}
