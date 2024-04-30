using FatSod.DataContext.Concrete;
using FatSod.Security.Abstracts;
using FatSod.Security.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FatSod.DataContext.Repositories
{
    public class MoucharRepository : Repository<Mouchar>, IMouchar 
    {
        public MoucharRepository(EFDbContext context)
        {
            this.context = context;
        }
        public MoucharRepository()
            : base()
        {

        }
        public bool DeleteOperation(int UserID, string Action, string Description, string ProcedureName, DateTime BusinessDate, int BranchID) 
        {
            bool res = false;
            try
            {
                Mouchar MoucharToSave = new Mouchar()
                {
                    MoucharUserID = UserID,
                    MoucharAction = Action,
                    MoucharDescription = Description,
                    MoucharOperationType = "DELETE",
                    MoucharBranchID = BranchID,
                    MoucharBusinessDate = BusinessDate,
                    MoucharProcedureName = ProcedureName
                };
                context.Mouchars.Add(MoucharToSave);
                context.SaveChanges();
                res = true;
            }
            catch (Exception e)
            {
                res = false;
                throw new Exception("Error : " + "e.Message = " + e.Message + "e.InnerException = " + e.InnerException + "e.StackTrace = " + e.StackTrace);
            }
            return res;
        }
        public bool UpdateOperation(int UserID, string Action, string Description, string ProcedureName, DateTime BusinessDate, int BranchID)
        {
            bool res = false;
            try
            {
                Mouchar MoucharToSave = new Mouchar()
                {
                    MoucharUserID = UserID,
                    MoucharAction = Action,
                    MoucharDescription = Description,
                    MoucharOperationType = "UPDATE",
                    MoucharBranchID = BranchID,
                    MoucharBusinessDate = BusinessDate,
                    MoucharProcedureName = ProcedureName
                };
                context.Mouchars.Add(MoucharToSave);
                context.SaveChanges();
                res = true;
            }
            catch (Exception e)
            {
                res = false;
                throw new Exception("Error : " + "e.Message = " + e.Message + "e.InnerException = " + e.InnerException + "e.StackTrace = " + e.StackTrace);
            }
            return res;
        }
        public bool InsertOperation(int UserID, string Action, string Description, string ProcedureName, DateTime BusinessDate, int BranchID)
        {
            bool res = false;
            try
            {
                Mouchar MoucharToSave = new Mouchar()
                {
                    MoucharUserID = UserID,
                    MoucharAction = Action,
                    MoucharDescription = Description,
                    MoucharOperationType = "INSERT",
                    MoucharBranchID = BranchID,
                    MoucharBusinessDate = BusinessDate,
                    MoucharProcedureName = ProcedureName
                };
                context.Entry(MoucharToSave).State = EntityState.Detached;
                context.Mouchars.Add(MoucharToSave);
                context.SaveChanges();
                res = true;
            }
            catch (Exception e)
            {
                res = false;
                throw new Exception("Error : " + "e.Message = " + e.Message + "e.InnerException = " + e.InnerException + "e.StackTrace = " + e.StackTrace);
            }
            return res;
        }
        public bool ConnectOperation(int UserID, string Action, string Description, string ProcedureName, DateTime BusinessDate, int BranchID)
        {
            bool res = false;
            try
            {
                Mouchar MoucharToSave = new Mouchar()
                {
                    MoucharUserID = UserID,
                    MoucharAction = Action,
                    MoucharDescription = Description,
                    MoucharOperationType = "CONNECT",
                    MoucharBranchID = BranchID,
                    MoucharBusinessDate = BusinessDate,
                    MoucharProcedureName = ProcedureName
                };
                context.Mouchars.Add(MoucharToSave);
                context.SaveChanges();
                res = true;
            }
            catch (Exception e)
            {
                res = false;
                throw new Exception("Error : " + "e.Message = " + e.Message + "e.InnerException = " + e.InnerException + "e.StackTrace = " + e.StackTrace);
            }
            return res;
        }
        public bool DisconnectOperation(int UserID, string Action, string Description, string ProcedureName, DateTime BusinessDate, int BranchID)
        {
            bool res = false;
            try
            {
                Mouchar MoucharToSave = new Mouchar()
                {
                    MoucharUserID = UserID,
                    MoucharAction = Action,
                    MoucharDescription = Description,
                    MoucharOperationType = "DISCONNECT",
                    MoucharBranchID = BranchID,
                    MoucharBusinessDate = BusinessDate,
                    MoucharProcedureName = ProcedureName
                };
                context.Mouchars.Add(MoucharToSave);
                context.SaveChanges();
                res = true;
            }
            catch (Exception e)
            {
                res = false;
                throw new Exception("Error : " + "e.Message = " + e.Message + "e.InnerException = " + e.InnerException + "e.StackTrace = " + e.StackTrace);
            }
            return res;
        }
    }
}
