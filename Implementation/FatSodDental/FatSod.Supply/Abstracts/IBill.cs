using FatSod.Supply.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FatSod.Supply.Abstracts
{
    public interface IBill : IRepositorySupply<Bill>
    {
        Bill PersistBill(Bill bill, int UserConect, int CurrentBranchID);
        bool DeleteBill(int billID, int UserConect, int CurrentBranchID);
    }
}
