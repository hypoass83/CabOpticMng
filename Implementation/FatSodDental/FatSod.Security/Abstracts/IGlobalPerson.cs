using FatSod.Security.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FatSod.Security.Abstracts
{
    public interface IGlobalPerson: IRepository<GlobalPerson>
    {
        void Remove(int personID);
        void Delete(GlobalPerson person);
        GlobalPerson Update2(GlobalPerson person);
    }
}
