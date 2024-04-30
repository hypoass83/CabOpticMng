using AutoMapper;
using FatSod.Budget.Abstracts;
using FatSod.Budget.Entities;
using FatSod.DataContext.Concrete;
using FatSod.Security.Abstracts;
using FatSod.Supply.Abstracts;
using FatSod.Supply.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace FatSod.DataContext.Repositories
{
	public class BudgetConsumptionRepository : RepositorySupply<BudgetConsumption>, IBudgetConsumption
	{
		/// <summary>
		/// methode permettant de persister l'autorisation du budget
		/// </summary>
		/// <param name="budgetConsumption"></param>
		/// <returns></returns>
		public BudgetConsumption CreateBudgetConsumption(BudgetConsumption budgetConsumption)
		{
                BudgetConsumption buConsumInsert;
				try
				{
                    using (TransactionScope ts = new TransactionScope())
                    {
                        //insertion de la consommation du budget
                        buConsumInsert = new BudgetConsumption();

                        buConsumInsert.BudgetAllocatedID = budgetConsumption.BudgetAllocatedID;
                        buConsumInsert.PaymentMethodID = budgetConsumption.PaymentMethodID;
                        buConsumInsert.VoucherAmount = budgetConsumption.VoucherAmount;
                        buConsumInsert.DateOperation = budgetConsumption.DateOperation;
                        buConsumInsert.Reference = budgetConsumption.Reference;
                        buConsumInsert.isValidated = budgetConsumption.isValidated;

                        buConsumInsert.BeneficiaryName = budgetConsumption.BeneficiaryName;
                        buConsumInsert.Justification = budgetConsumption.Justification;
                        buConsumInsert.AutorizeByID = budgetConsumption.AutorizeByID;
                        context.BudgetConsumptions.Add(buConsumInsert);

                        context.SaveChanges();

                        //metre a jour le transaction number
                        int compteur = Convert.ToInt32(budgetConsumption.Reference.Substring(12));
                        ITransactNumber trnNumber = new TransactNumberRepository(context);
                        bool res = trnNumber.saveTransactNumber("ABUC", compteur);
                        if (!res)
                        {
                            throw new Exception("Une erreur s'est produite lors de la mise a jour du compteur du transact number ");
                        }
                        //transaction.Commit();
                        ts.Complete();
                    }
					
				}
				catch (Exception e)
				{
					//If an errors occurs, we cancel all changes in database
                    //transaction.Rollback();
					throw new Exception("Une erreur s'est produite lors de l'authorisation de la comsommation du budget : " + e.Message);
				}
                return buConsumInsert;
		}
		/// <summary>
		/// methode permettant de mettre a jour l'autorisation du budget
		/// </summary>
		/// <param name="budgetConsumption"></param>
		/// <returns></returns>
		public BudgetConsumption UpdateBudgetConsumption(BudgetConsumption budgetConsumption)
		{
                BudgetConsumption buConsumUpdate;
				try
				{
                    using (TransactionScope ts = new TransactionScope())
                    {
                        //we update it
                        buConsumUpdate = context.BudgetConsumptions.Find(budgetConsumption.BudgetConsumptionID);
                        if (buConsumUpdate != null)
                        {
                            buConsumUpdate.BudgetAllocatedID = budgetConsumption.BudgetAllocatedID;
                            buConsumUpdate.PaymentMethodID = budgetConsumption.PaymentMethodID;
                            buConsumUpdate.VoucherAmount = budgetConsumption.VoucherAmount;
                            buConsumUpdate.DateOperation = budgetConsumption.DateOperation;
                            buConsumUpdate.Reference = budgetConsumption.Reference;
                            buConsumUpdate.isValidated = budgetConsumption.isValidated;

                            buConsumUpdate.BeneficiaryName = budgetConsumption.BeneficiaryName;
                            buConsumUpdate.Justification = budgetConsumption.Justification;
                            buConsumUpdate.AutorizeByID = budgetConsumption.AutorizeByID;
                            context.SaveChanges();
                        }
                        //transaction.Commit();
                        ts.Complete();
                    }
				}
				catch (Exception e)
				{
					//If an errors occurs, we cancel all changes in database
                    //transaction.Rollback();
					throw new Exception("Une erreur s'est produite lors de l'authorisation de la comsommation du budget : " + e.Message);
				}
                return buConsumUpdate;
		}

        public bool SavebudgetConsumption(BudgetConsumption BudgetConsumption, int UserConect, int BranchID)
		{
			    bool res = false;
				try
				{
                    using (TransactionScope ts = new TransactionScope())
                    {
                        //mise a jour du statu
                        BudgetConsumption buConsumUpdate = context.BudgetConsumptions.Find(BudgetConsumption.BudgetConsumptionID);
                        if (buConsumUpdate != null)
                        {
                            buConsumUpdate.isValidated = true;
                            buConsumUpdate.PaymentMethodID = BudgetConsumption.PaymentMethodID;
                            buConsumUpdate.DeviseID = BudgetConsumption.DeviseID.Value;
                            buConsumUpdate.PaymentDate = BudgetConsumption.PaymentDate.Value;
                            buConsumUpdate.ValidateByID = BudgetConsumption.ValidateByID;
                        }
                        context.SaveChanges();
                        //Accountig operations
                        BudgetConsumption acountableBudgetConsumption = new BudgetConsumption()
                        {
                            BudgetConsumptionID = buConsumUpdate.BudgetConsumptionID,
                            BudgetAllocatedID = buConsumUpdate.BudgetAllocatedID,
                            BudgetAllocated = context.BudgetAllocateds.Find(buConsumUpdate.BudgetAllocatedID),
                            PaymentMethodID = buConsumUpdate.PaymentMethodID,
                            VoucherAmount = buConsumUpdate.VoucherAmount,
                            DateOperation = buConsumUpdate.PaymentDate.Value,
                            Reference = buConsumUpdate.Reference,
                            BeneficiaryName = buConsumUpdate.BeneficiaryName,
                            Justification = buConsumUpdate.Justification,
                            isValidated = buConsumUpdate.isValidated,
                            DeviseID = buConsumUpdate.DeviseID.Value
                        };
                        //////comptabilisation
                        ////IAccountOperation opaccount = new AccountOperationRepository(context);
                        ////res = opaccount.ecritureComptableFinal(acountableBudgetConsumption);
                        ////if (!res)
                        ////{
                        ////    //transaction.Rollback();
                        ////    throw new Exception("Une erreur s'est produite lors de la validation de la comsommation du budget ");
                        ////}
                        ////context.SaveChanges();
                        //EcritureSneack
                        IMouchar opSneak = new MoucharRepository(context);
                        res = opSneak.InsertOperation(UserConect, "SUCCESS", " SAVE BUDGET CONSUMPTION REF" + BudgetConsumption.Reference + " -JUSTIFICATION- " + BudgetConsumption.Justification, "SavebudgetConsumption", BudgetConsumption.DateOperation, BranchID);
                        if (!res)
                        {
                            throw new Exception("Une erreur s'est produite lors de la journalisation ");
                        }
                        //transaction.Commit();
                        ts.Complete();
                    }
				}
				catch (Exception e)
				{
					//If an errors occurs, we cancel all changes in database
					res = false;
                    //transaction.Rollback();
					throw new Exception("Une erreur s'est produite lors de la validation de la comsommation du budget : " + e.Message);
				}
				return res;
			}
		//}
	}
}
