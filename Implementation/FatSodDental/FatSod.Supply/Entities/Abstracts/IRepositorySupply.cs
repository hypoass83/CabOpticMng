using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FatSod.Security.Abstracts;

namespace FatSod.Supply.Abstracts
{

    public interface IRepositorySupply<H> where H : class
    {

        IEnumerable<H> FindAll { get; }
        H Update(H obj);
        H Create(H obj);
        H Delete(H obj);
        H Update(H updated, int key);
        H Find(int key);
        IEnumerable<H> CreateAll(IEnumerable<H> tList);
        IEnumerable<H> DeleteAll(IEnumerable<H> tList);
    }
}
