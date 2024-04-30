using FatSod.Security.Entities;
using FatSod.Supply.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FatSod.Supply.Abstracts
{
    public interface IRegProductNumber : IRepositorySupply<RegProductNumber>
    {
        RegProductNumber DoRegProductNumber(RegProductNumber RegProductNumber,int UserConnect);
        bool CancelRegProductNumber(int RegProductNumberID);
        RegProductNumber UpdateRegProductNumber(RegProductNumber RegProductNumber);
    }
}
