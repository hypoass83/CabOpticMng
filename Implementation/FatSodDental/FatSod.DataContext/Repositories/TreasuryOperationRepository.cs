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
	public class TreasuryOperationRepository : RepositorySupply<TreasuryOperation>, ITreasuryOperation
	{
        public bool SaveTreasuryOperation(TreasuryOperation treasuryOperation, int UserConect, DateTime OperationDate, int BranchID)
		{
			bool res = false;
			
			//Begin of transaction
				try
				{
                    using (TransactionScope ts = new TransactionScope())
                    {
                        //mise a jour du montant en caisse
                        if (treasuryOperation.TreasuryOperationID==0)
                        {
                            //ecriture ds la table d'ajustement
                            context.TreasuryOperations.Add(treasuryOperation);
                            context.SaveChanges();
                            //comptabilisation du nouveau montant

                            //Accountig operations
                            TreasuryOperation acountableTreasuryOperation = new TreasuryOperation()
                            {
                                TreasuryOperationID = treasuryOperation.TreasuryOperationID,
                                TillID = treasuryOperation.TillID,
                                //Till = context.PaymentMethods.OfType<Till>().FirstOrDefault(t=>t.ID== treasuryOperation.TillID),
                                OperationDate = treasuryOperation.OperationDate,
                                Justification = treasuryOperation.Justification,
                                ComputerPrice = treasuryOperation.ComputerPrice,
                                OperationAmount = treasuryOperation.OperationAmount,
                                DeviseID = treasuryOperation.DeviseID,
                                OperationRef = treasuryOperation.OperationRef,
                                OperationType = treasuryOperation.OperationType,
                                BankID = treasuryOperation.BankID,
                                //Bank = context.PaymentMethods.OfType<Bank>().FirstOrDefault(t => t.ID == treasuryOperation.TillID),
                                BranchID = treasuryOperation.BranchID,
                            };
                            //////comptabilisation
                            ////IAccountOperation opaccount = new AccountOperationRepository(context);
                            ////res = opaccount.ecritureComptableFinal(acountableTreasuryOperation);
                            ////if (!res)
                            ////{
                            ////    res = false;
                            ////    throw new Exception("Une erreur s'est produite lors de la comptabilisation de l'adjustement de la Caisse ");
                            ////}
                            ////context.SaveChanges();
                            //EcritureSneack
                            IMouchar opSneak = new MoucharRepository(context);
                            res = opSneak.InsertOperation(UserConect, "SUCCESS", " SAVE TREASURY OPERATION REF" + treasuryOperation.OperationRef + " -JUSTIFICATION- " + treasuryOperation.Justification, "SaveTreasuryOperation", OperationDate, BranchID);
                            if (!res)
                            {
                                res = false;
                                throw new Exception("Une erreur s'est produite lors de la journalisation ");
                            }
                        }
                        else
                        {
                            res = false;
                            throw new Exception("You Cannot Update This Transaction!!! Contact your Administrator if you want to update this transaction ");
                        }
                        //transaction.Commit();
                        res = true;
                        ts.Complete();
                    }
					
				}
				catch (Exception ex)
				{
					res = false;
                    //transaction.Rollback();
					throw new Exception(ex.Message);
				}
                return res;

		}

	}
}
