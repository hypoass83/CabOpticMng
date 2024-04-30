using FatSod.Security.Abstracts;
using FatSod.Security.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FatSod.DataContext.Repositories
{
    public class GlobalPersonRepository : Repository<GlobalPerson>, IGlobalPerson
    {
        public void Remove(int personID)
        {
            throw new NotImplementedException();
        }

        public new void Delete(GlobalPerson person)
        {
            throw new NotImplementedException();
        }

        public GlobalPerson Update2(GlobalPerson person)
        {
            throw new NotImplementedException();
        }
    }
}
