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
	public class TillAdjustRepository : RepositorySupply<TillAdjust>, ITillAdjust
	{
        public bool SaveTillAdjust(TillAdjust tillAdjust, int UserConect, int BranchID)
		{
			bool res = false;
			

				try
				{
                    using (TransactionScope ts = new TransactionScope())
                    {
					//mise a jour du montant en caisse

					//ecriture ds la table d'ajustement
					context.TillAdjusts.Add(tillAdjust);
					context.SaveChanges();
					//comptabilisation du nouveau montant
					//recuperation de l'ecart de la jrne

					double ecart = tillAdjust.PhysicalPrice - tillAdjust.ComputerPrice;

					//Accountig operations
					TillAdjust acountableTillAdjust = new TillAdjust()
					{
						TillAdjustID = tillAdjust.TillAdjustID,
						TillID = tillAdjust.TillID,
                        Till = context.PaymentMethods.OfType<Till>().FirstOrDefault(t => t.ID == tillAdjust.TillID),
						TillAdjustDate = tillAdjust.TillAdjustDate,
						Justification = tillAdjust.Justification,
						ComputerPrice = tillAdjust.ComputerPrice,
						PhysicalPrice = tillAdjust.PhysicalPrice,
						DeviseID = tillAdjust.DeviseID,
						ecart = ecart
					};
					//////comptabilisation
					////IAccountOperation opaccount = new AccountOperationRepository(context);
					////res = opaccount.ecritureComptableFinal(acountableTillAdjust);
					////if (!res)
					////{
     ////                       //transaction.Rollback();
					////	throw new Exception("Une erreur s'est produite lors de la comptabilisation de l'adjustement de la Caisse ");
					////}

					////context.SaveChanges();
                        //transaction.Commit();
                    //EcritureSneack
                    IMouchar opSneak = new MoucharRepository(context);
                    bool res1 = opSneak.InsertOperation(UserConect, "SUCCESS", "TILL ADJUST FOR TELLER " + acountableTillAdjust.Till.Name + " JUSTIFICATION " + tillAdjust.Justification, "SaveTillAdjust", tillAdjust.TillAdjustDate, BranchID);
                    if (!res1)
                    {
                        throw new Exception("Une erreur s'est produite lors de la journalisation ");
                    }
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
