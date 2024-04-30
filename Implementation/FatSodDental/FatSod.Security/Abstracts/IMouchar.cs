using FatSod.Security.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FatSod.Security.Abstracts
{
    public interface IMouchar : IRepository<Mouchar>
    {
        bool DeleteOperation(int UserID, string Action, string Description, string ProcedureName, DateTime BusinessDate, int BranchID);
        bool UpdateOperation(int UserID, string Action, string Description, string ProcedureName, DateTime BusinessDate, int BranchID);
        bool InsertOperation(int UserID, string Action, string Description, string ProcedureName, DateTime BusinessDate, int BranchID);
        bool ConnectOperation(int UserID, string Action, string Description, string ProcedureName, DateTime BusinessDate, int BranchID);
        bool DisconnectOperation(int UserID, string Action, string Description, string ProcedureName, DateTime BusinessDate, int BranchID);
    }
}
