using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FatSod.Security.Abstracts;
using FatSod.Security.Entities;
using FatSod.DataContext.Concrete;
using System.Data.Entity;
using FatSod.Supply.Entities;
using System.Transactions;
using FatSod.Supply.Abstracts;
using FatSod.Ressources;

namespace FatSod.DataContext.Repositories
{
    public class BusinessDayRepository : Repository<BusinessDay>, IBusinessDay
    {
        /// <summary>
        /// cette méthode retourne le BusinessDay si la journée de travail est ouverte.
        /// la méthode retournera null si le businessday de la branche n'est pas ouverte.
        /// </summary>
        /// <param name="branch">Agence donc dont veut le BusinessDay ouvert</param>
        /// <returns></returns>
        public BusinessDay GetOpenedBusinessDay(Branch branch)
        {
            return this.FindAll.Where(bd => bd.BranchID == branch.BranchID && bd.BDStatut == true && bd.ClosingDayStarted == false).SingleOrDefault();
        }
        /// <summary>
        /// cette méthode retourne la liste des businessday ouverte correspondant à une liste d'agences passées en paramètre
        /// </summary>
        /// <param name="branches">Liste des agences dont on veut les BusinessDay équivalent s'il sont ouvert</param>
        /// <returns></returns>
        public List<BusinessDay> GetOpenedBusinessDay(List<Branch> branches)
        {
            List<BusinessDay> res = new List<BusinessDay>();
            foreach (Branch branch in branches)
            {
                res.Add(GetOpenedBusinessDay(branch));
            }
            return res;
        }
        /// <summary>
        /// cette méthode retourne la liste des business day ouverte de l'utilisateur
        /// </summary>
        /// <param name="user">l'utilisateur dont on veut la liste des BusinessDay ouverts</param>
        /// <returns></returns>
        public List<BusinessDay> GetOpenedBusinessDay(User user)
        {
            List<BusinessDay> res = new List<BusinessDay>();

            foreach (UserBranch ub in this.context.UserBranches.Where(ub => ub.UserID == user.GlobalPersonID).ToList())
            {
                res.AddRange(
                    ub.Branch.BusinessDays.Where(bd => (bd.BDStatut == true) && (bd.ClosingDayStarted == false)).ToList()
                );
                //BusinessDay busDay = this.FindAll.Where(bd => (bd.BranchID == ub.BranchID) && (bd.BDStatut == true) && (bd.ClosingDayStarted == false)).SingleOrDefault();
                //if (busDay != null) { res.Add(busDay); }
            }

            return res;
        }

        public List<BusinessDay> GetOpenedBackDate(User user)
        {
            List<BusinessDay> res = new List<BusinessDay>();

            foreach (UserBranch ub in this.context.UserBranches.Where(ub => ub.UserID == user.GlobalPersonID).ToList())
            {
                res.AddRange(
                    ub.Branch.BusinessDays.Where(bd => (bd.BDStatut == true) && (bd.ClosingDayStarted == false) && (bd.BackDStatut==true)).ToList()
                );
            }

            return res;
        }

        /// <summary>
        /// cette méthode retourne la liste des agences ouvertes
        /// </summary>
        /// <returns></returns>
        public List<Branch> GetOpenedBranches()
        {
            List<Branch> res = new List<Branch>();
            List<Branch> allBranches = context.Branches.ToList();
            foreach (Branch branch in allBranches)
            {
                BusinessDay openedBusDay = GetOpenedBusinessDay(branch);
                if (openedBusDay != null && openedBusDay.BusinessDayID > 0)
                {
                    res.Add(openedBusDay.Branch);
                }
            }
            return res;
        }

        public List<BusinessDay> GetOpenedBusinessDay()
        {
            return this.FindAll.Where(bd => (bd.BDStatut == true) && (bd.ClosingDayStarted == false)).ToList();
        }

        public List<BusinessDay> GetClosedBusinessDay()
        {
            return this.FindAll.Where(bd => (bd.BDStatut == false) || (bd.ClosingDayStarted == true)).ToList();
        }

        /// <summary>
        /// renvoie la liste des branches donc le businessday est ouvert et donc la branch appartient à l'utilisateur
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public List<Branch> GetUserOpenedBranch(User user)
        {
            List<Branch> openedBranches = new List<Branch>();
            List<BusinessDay> openedBusDays = this.GetOpenedBusinessDay(user);

            if (openedBusDays != null && openedBusDays.Count > 0)
            {
                foreach (BusinessDay busDay in openedBusDays)
                {
                    openedBranches.Add(busDay.Branch);
                }
            }
            return openedBranches;

        }

        public List<BusinessDay> GetClosedBusinessDay(User user)
        {
            List<BusinessDay> res = new List<BusinessDay>();

            List<UserBranch> allUserBranches = this.context.UserBranches.Where(ub => ub.UserID == user.GlobalPersonID).ToList();
            foreach (UserBranch ub in allUserBranches)
            {
                BusinessDay busDay = this.FindAll.Where(bd => (bd.BranchID == ub.BranchID) && ((bd.BDStatut == false) || (bd.ClosingDayStarted == true))).SingleOrDefault();
                if (busDay != null) { res.Add(busDay); }

            }

            return res;
        }

        public Boolean OpenBusinessDay(Branch branch, DateTime dateOperation, int UserConect)
        {
            bool res = false;

            try
            {
                using (TransactionScope ts = new TransactionScope())
                {

                    ////mise à jour du business day
                    //recuperation du business day a modifier
                    BusinessDay busDayToUpdate = (from bsDay in this.context.BusinessDays
                                                  where bsDay.BranchID == branch.BranchID
                                                  select bsDay).SingleOrDefault();
                    if (busDayToUpdate == null)
                    {
                        BusinessDay newbusDay = new BusinessDay();
                        newbusDay.BackDateOperation = dateOperation;
                        newbusDay.BackDStatut = true;
                        newbusDay.BDCode = "BusDay_" + branch.BranchAbbreviation;
                        newbusDay.BDDateOperation = dateOperation;
                        newbusDay.BDDescription = "Business Date for Branch " + branch.BranchAbbreviation;
                        newbusDay.BDLabel = "BusDay_" + branch.BranchAbbreviation;
                        newbusDay.BranchID = branch.BranchID;
                        newbusDay.BDStatut = true;
                        busDayToUpdate = context.BusinessDays.Add(newbusDay);
                    }
                    else
                    {
                        //evirons le backdate
                        if (busDayToUpdate.BDDateOperation > dateOperation)
                        {
                            throw new Exception("The system does not accept Backdate ");
                        }
                        else if (busDayToUpdate.BDStatut == true)
                        {
                            throw new Exception("The last business date still open. please close it before !!! ");
                        }
                        else
                        {
                            //champ a modifier
                            busDayToUpdate.BDStatut = true;
                            busDayToUpdate.BDDateOperation = dateOperation;
                            busDayToUpdate.BackDateOperation = dateOperation;
                        }

                    }
                    context.SaveChanges();
                    //modification du transaction number table
                    List<TransactNumber> trnNumber = (from trNum in this.context.TransactNumbers
                                                      where trNum.BranchID == branch.BranchID
                                                      select trNum).ToList();
                    //champ a modifier
                    foreach (TransactNumber tn in trnNumber)
                    {
                        if (tn.DateOperation > busDayToUpdate.BDDateOperation)
                        {
                            tn.DateOperation = busDayToUpdate.BDDateOperation;
                            tn.Counter = 0;
                        }

                    }
                    context.SaveChanges();

                    //EcritureSneack
                    IMouchar opSneak = new MoucharRepository(context);
                    res = opSneak.InsertOperation(UserConect, "SUCCESS", "OPEN BUSINESS DAY OF " + dateOperation, "OpenBusinessDay", dateOperation, branch.BranchID);
                    if (!res)
                    {
                        throw new Exception("Une erreur s'est produite lors de la journalisation ");
                    }
                    //transaction.Commit();
                    ts.Complete();
                    res = true;
                }
            }
            catch (Exception e)
            {
                //transaction.Rollback();
                //EcritureSneack
                IMouchar opSneak = new MoucharRepository(context);
                res = opSneak.InsertOperation(UserConect, "ERROR", "OPEN BUSINESS DAY OF " + dateOperation, "OpenBusinessDay", dateOperation, branch.BranchID);
                if (!res)
                {
                    throw new Exception("Une erreur s'est produite lors de la journalisation ");
                }
                throw new Exception("Error : " + e.Message);
            }
            //}

            return res;
        }

        public Boolean OpenBackDate(Branch branch, DateTime dateOperation)
        {
            bool res = false;

            try
            {
                using (TransactionScope ts = new TransactionScope())
                {

                    ////mise à jour du business day
                    //recuperation du business day a modifier
                    BusinessDay busDayToUpdate = (from bsDay in this.context.BusinessDays
                                                  where bsDay.BranchID == branch.BranchID
                                                  select bsDay).SingleOrDefault();
                    //champ a modifier
                    busDayToUpdate.BackDStatut = true;
                    //busDayToUpdate.BackDateOperation = dateOperation;
                    busDayToUpdate.BDDateOperation = dateOperation;

                    context.SaveChanges();
                    
                    ts.Complete();
                    res = true;
                }
            }
            catch (Exception e)
            {
                //transaction.Rollback();
                throw new Exception("Error : " + e.Message);
            }
            //}

            return res;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="branch"></param>
        /// <param name="dateOperation"></param>
        /// <param name="UserConect"></param>
        /// <returns></returns>
        public Boolean CloseBusinessDay(Branch branch, DateTime dateOperation, int UserConect)
        {
            bool res = false;
            string Message = "";

            try
                {
                    using (TransactionScope ts = new TransactionScope())
                    {

                    
                    //on ne ferme une agence que si toutes ses caisses sont fermées
                    TillDayStatus currentTillDay = context.TillDayStatus.Where(t => 
                                                                    t.IsOpen == true).FirstOrDefault();

                    //s'il existe au moins une caisse ouverte alors on ne doit pas fermer la journée
                    if (currentTillDay != null)
                    {
                        res = false;

                        Message = /*"Cash Register " + currentTillDay.Till.Code + " Is Not Yet Closed " +*/ Resources.CashRegNotYetClose + currentTillDay.Till.Code;
                        throw new Exception(Message);
                    }
                    

                    //recuperation du business day a modifier
                    BusinessDay busDayToUpdate = (from bsDay in this.context.BusinessDays
                                                      where bsDay.BranchID == branch.BranchID
                                                      select bsDay).SingleOrDefault();
                        //champ a modifier
                        busDayToUpdate.BDStatut = false;
                        busDayToUpdate.BackDStatut = false;
                        
                        //busDayToUpdate.BDDateOperation = busDayToUpdate.BackDateOperation;

                        context.SaveChanges();
                        //modification du transaction number table
                        List<TransactNumber> trnNumber = (from trNum in this.context.TransactNumbers
                                                          where trNum.BranchID == branch.BranchID
                                                          select trNum).ToList();
                        //champ a modifier
                        foreach (TransactNumber tn in trnNumber)
                        {
                            tn.Counter = 0;
                        }
                        context.SaveChanges();

                        
                        //EcritureSneack
                        IMouchar opSneak = new MoucharRepository(context);
                        res = opSneak.InsertOperation(UserConect, "SUCCESS", "CLOSE BUSINESS DAY OF " + busDayToUpdate.BDDateOperation, "OpenBusinessDay", dateOperation, branch.BranchID);
                        if (!res)
                        {
                            throw new Exception("Une erreur s'est produite lors de la journalisation ");
                        }
                        ts.Complete();
                        res = true;
                    }
                }
                catch (Exception e)
                {
                    //EcritureSneack
                    IMouchar opSneak = new MoucharRepository(context);
                    res = opSneak.InsertOperation(UserConect, "ERROR", "CLOSE BUSINESS DAY OF " + dateOperation, "OpenBusinessDay", dateOperation, branch.BranchID);
                    if (!res)
                    {
                        throw new Exception("Une erreur s'est produite lors de la journalisation ");
                    }
                    //transaction.Rollback();
                    throw new Exception("Error : " + e.Message);
                }
            //}

            return res;
        }

        private void InitTransactNumber()
        {

        }


    }
}
