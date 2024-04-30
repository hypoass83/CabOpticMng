using FatSod.Supply.Abstracts;
using FatSod.Supply.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FatSod.DataContext.Concrete;
using FatSod.DataContext.Initializer;
using System.Data.Entity;
using System.Transactions;
using FatSod.Security.Abstracts;
using AutoMapper;

namespace FatSod.DataContext.Repositories
{
    public class UserTillRepository : RepositorySupply<UserTill>, IUserTill
    {
        //public UserTillRepository(EFDbContext context1)
        //{
        //    this.context = context1;

        //}
         public UserTillRepository(EFDbContext context)
        {
            this.context = context;
        }
         public UserTillRepository()
            : base()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="till"></param>
        /// <param name="userTill"></param>
        /// <param name="UserConect"></param>
        /// <param name="BranchID"></param>
        /// <param name="DateOperation"></param>
        /// <returns></returns>
        public UserTill CreateUserTill(Till till, UserTill userTill, int UserConect, int BranchID, DateTime DateOperation)
        {
            UserTill newuserTill = new UserTill();
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    //verifions si cet user na pas encore de caisse
                    UserTill newoldUserTill = context.UserTills.FirstOrDefault(ut => ut.UserID == userTill.UserID);
                    if (newoldUserTill != null)
                    {
                        throw new Exception("We cannot assign more than one till to the same user ");
                    }
                    //creation du till
                    Till newtill = new Till();
                    newtill = context.Tills.Add(till);
                    context.SaveChanges();

                    int tillID = newtill.ID;
                    //ajout du user till

                    userTill.TillID = tillID;
                    newuserTill = context.UserTills.Add(userTill);
                    context.SaveChanges();
                    //creation du till day
                    TillDay newtillday = new TillDay()
                    {
                        IsOpen = false,
                        TillDayClosingPrice = 0,
                        TillDayDate = DateOperation.AddDays(-1),
                        TillDayOpenPrice = 0,
                        TillID = tillID
                    };
                    context.TillDays.Add(newtillday);
                    context.SaveChanges();
                    //creation du till day status
                    TillDayStatus newtilldaystatus = new TillDayStatus()
                    {
                        IsOpen = false,
                        TillDayLastClosingDate = DateOperation.AddDays(-1),
                        TillDayLastOpenDate = DateOperation.AddDays(-1),
                        TillID = tillID
                    };
                    context.TillDayStatus.Add(newtilldaystatus);
                    context.SaveChanges();
                    //EcritureSneack
                    IMouchar opSneak = new MoucharRepository(context);
                    bool res = opSneak.InsertOperation(UserConect, "SUCCESS", "INSERT TILL FOR USER ID " + userTill.UserID, "CreateUserTill", DateOperation, BranchID);
                    if (!res)
                    {
                        throw new Exception("Une erreur s'est produite lors de la journalisation ");
                    }
                    ts.Complete();
                }
            }
            catch (Exception e)
            {
                //If an errors occurs, we cancel all changes in database
                //EcritureSneack
                IMouchar opSneak = new MoucharRepository(context);
                bool res = opSneak.InsertOperation(UserConect, "ERROR", "INSERT TILL FOR USER " + till.Code, "CreateUserTill", DateOperation, BranchID);
                if (!res)
                {
                    throw new Exception("Une erreur s'est produite lors de la journalisation ");
                }
                throw new Exception("Une erreur s'est produite lors de la creation du caissier : " + "e.Message = " + e.Message + "e.InnerException = " + e.InnerException + "e.StackTrace = " + e.StackTrace);
            }
            return newuserTill;
        }

        public UserTill UpdateUserTill(Till till, UserTill userTill, int UserConect, int BranchID, DateTime DateOperation)
        {
            UserTill existinguserTill = new UserTill();
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    //update du till
                    Till newtill = context.Tills.Find(till.ID);
                    newtill.Name = till.Name;
                    newtill.Code = till.Code;
                    context.SaveChanges();

                    int tillID = newtill.ID;

                    if (userTill.HasAccess)
                    {

                        if (userTill.UserTillID > 0 /*|| userTill.UserTillID!=null*/)
                        {
                            //update du user till
                            existinguserTill = this.Update(userTill, userTill.UserTillID);
                        }
                        else
                        {
                            //creation du user till
                            //verifions si cet user na pas encore de caisse
                            UserTill newoldUserTill = context.UserTills.FirstOrDefault(ut => ut.UserID == userTill.UserID);
                            if (newoldUserTill != null)
                            {
                                throw new Exception("We cannot assign more than one till to the same user ");
                            }
                            existinguserTill = this.Create(userTill);
                        }

                        //creation du till day s'il n'existe pas
                        TillDay existingtillday = context.TillDays.Where(t => t.TillID == tillID).FirstOrDefault();
                        if (existingtillday == null)
                        {
                            TillDay newtillday = new TillDay()
                            {
                                IsOpen = false,
                                TillDayClosingPrice = 0,
                                TillDayDate = DateOperation.AddDays(-1),
                                TillDayOpenPrice = 0,
                                TillID = tillID
                            };
                            context.TillDays.Add(newtillday);
                            context.SaveChanges();
                        }

                        //creation du till day status s'il n'existe pas
                        TillDayStatus existingtilldaystatus = context.TillDayStatus.Where(t => t.TillID == tillID).FirstOrDefault();
                        if (existingtilldaystatus == null)
                        {
                            TillDayStatus newtilldaystatus = new TillDayStatus()
                            {
                                IsOpen = false,
                                TillDayLastClosingDate = DateOperation.AddDays(-1),
                                TillDayLastOpenDate = DateOperation.AddDays(-1),
                                TillID = tillID
                            };
                            context.TillDayStatus.Add(newtilldaystatus);
                            context.SaveChanges();
                        }
                    }
                    else //si l'on retire la caisse o user
                    {
                        //delete user till
                        this.Delete(userTill.UserTillID);
                    }
                    //EcritureSneack
                    IMouchar opSneak = new MoucharRepository(context);
                    bool res = opSneak.InsertOperation(UserConect, "SUCCESS", "UPDATE TILL FOR USER ID " + userTill.UserID, "UPDATEUserTill", DateOperation, BranchID);
                    if (!res)
                    {
                        throw new Exception("Une erreur s'est produite lors de la journalisation ");
                    }
                    ts.Complete();
                }
            }
            catch (Exception e)
            {
                //If an errors occurs, we cancel all changes in database
                //EcritureSneack
                IMouchar opSneak = new MoucharRepository(context);
                bool res = opSneak.InsertOperation(UserConect, "ERROR", "UPDATE TILL FOR USER " + till.Code, "UPDATEUserTill", DateOperation, BranchID);
                if (!res)
                {
                    throw new Exception("Une erreur s'est produite lors de la journalisation ");
                }
                throw new Exception("Une erreur s'est produite lors de la creation du caissier : " + "e.Message = " + e.Message);
            }
            return existinguserTill;
        }
    }
}
