using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FatSod.Supply.Abstracts;
using FatSod.Supply.Entities;
using FatSod.Security.Entities;
using System.Data.Entity;
using FatSod.DataContext.Concrete;
using FatSod.Security.Abstracts;
using AutoMapper;
using System.Transactions;

namespace FatSod.DataContext.Repositories
{
    /// <summary>
    /// 
    /// </summary>
	public class StockReplacementRepository : RepositorySupply<StockReplacement>, IStockReplacement
	{
		IProductLocalization _plRepository;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="StockReplacement"></param>
        /// <param name="UserConnect"></param>
        /// <returns></returns>
        public StockReplacement DoStockReplacement(StockReplacement StockReplacement,int UserConnect)
		{
			//int res = 0;
				try
				{
                    //Begin of transaction
                    using (TransactionScope ts = new TransactionScope())
                    {
				        //_plRepository = new PLRepository(this.context);

					    StockReplacement doStockReplacement = new StockReplacement();

					    //ces 2 lignes qui suivent permettent de transformer un StockReplacement en StockReplacement 
					    //create a Map
					    Mapper.CreateMap<StockReplacement, StockReplacement>();
					    //use Map
					    doStockReplacement = Mapper.Map<StockReplacement>(StockReplacement);
					    //default receiver 
					    //transfert.ReceivedByID = 2;
					    doStockReplacement.StockReplacementLines = null;
					    this.Create(doStockReplacement);
					    StockReplacement.StockReplacementID = doStockReplacement.StockReplacementID;

					    //Création des lignes de transfert
					    CreateStockReplacementLine(StockReplacement.StockReplacementLines.ToList(), StockReplacement.StockReplacementID);

					    //Mise à jour du stock de départ
					    OutPutStocksUpdate(StockReplacement,UserConnect);

                        //mise a jour du cpteur du transact number
                        TransactNumber trn = context.TransactNumbers.SingleOrDefault(t => t.TransactNumberCode == "STRP");
                        if (trn != null)
                        {
                            //persistance du compteur de l'objet TransactNumber
                            trn.Counter = trn.Counter + 1;
                        }
                        context.SaveChanges();

                        ts.Complete();
				    }
                }
				catch (Exception e)
				{
					//If an errors occurs, we cancel all changes in database
                    //transaction.Rollback();
					throw new Exception(e.Message + ": Check " + e.StackTrace);
				}
			//}
            return StockReplacement;
		}

        /// <summary>
        /// Cette méthode permet de constater la reception des produits par l'agence d'arrivée
        /// </summary>
        /// <param name="pt"></param>
        /// <returns></returns>
        public StockReplacement ValidateStockReplacement(StockReplacement pt)
		{
			//bool res = false;

            StockReplacement StockReplacement = new StockReplacement();
			//Begin of transaction
              
				try
				{
                    using (TransactionScope ts = new TransactionScope())
                    {
                        if (pt.StockReplacementID <= 0)
                        {
                            StockReplacement StockReplacementInsert = new StockReplacement();

                            //ces 2 lignes qui suivent permettent de transformer un StockReplacement en StockReplacement 
                            //create a Map
                            Mapper.CreateMap<StockReplacement, StockReplacement>();
                            //use Map
                            StockReplacementInsert = Mapper.Map<StockReplacement>(pt);
                            //default receiver 
                            StockReplacementInsert.RegisteredByID = pt.RegisteredByID;
                            StockReplacementInsert.AutorizedByID = pt.AutorizedByID;
                            StockReplacementInsert.StockReplacementLines = null;
                            StockReplacementInsert.StockReplacementDate = pt.StockReplacementDate;
                            this.Create(StockReplacementInsert);
                            pt.StockReplacementID = StockReplacementInsert.StockReplacementID;

                            //Création des lignes de transfert
                            CreateStockReplacementLine(pt.StockReplacementLines.ToList(), pt.StockReplacementID);
                            StockReplacementInsert = pt;
                        }
                        else
                        {
                            StockReplacement = context.StockReplacements.AsNoTracking().SingleOrDefault(pt1 => pt1.StockReplacementID == pt.StockReplacementID);
                            StockReplacement.StockReplacementDate = pt.StockReplacementDate;
                            StockReplacement.RegisteredByID = pt.RegisteredByID;
                            StockReplacement.AutorizedByID = pt.AutorizedByID;

                            List<StockReplacementLine> lines = StockReplacement.StockReplacementLines.ToList();

                            StockReplacement.StockReplacementLines = null;

                            context.StockReplacements.Attach(StockReplacement);
                            context.Entry(StockReplacement).State = EntityState.Modified;
                            context.SaveChanges();

                            foreach (StockReplacementLine ptl in lines)
                            {
                                //ptl.ArrivalLocalizationID = ArrivalLocalizationID;
                                context.StockReplacementLines.Attach(ptl);
                                context.Entry(ptl).State = EntityState.Modified;
                                context.SaveChanges();
                            }
                        }

                        //Mise à jour du stock d'arrivé
                        InputStocksUpdate(StockReplacement);

                        ts.Complete();
                    }
				}
				catch (Exception e)
				{
					//If an errors occurs, we cancel all changes in database
					//transaction.Rollback();
					throw new Exception(e.Message + ": Check " + e.StackTrace);
				}
			//}
            return StockReplacement;
		}

       
        /// <summary>
        /// 
        /// </summary>
        /// <param name="StockReplacementID"></param>
        /// <returns></returns>
		public bool CancelStockReplacement(int StockReplacementID)
		{
            bool res = false;
            try
            {
                //Begin of transaction
                using (TransactionScope ts = new TransactionScope())
                {
			        StockReplacement StockReplacement = this.Find(StockReplacementID);
			        
                    //if (StockReplacement != null && StockReplacement.StockReplacementID > 0 )
                    //{
                    //    throw new Exception("Verry Sorry this Operation was already validated. You can not be cancelled");
                    //}

			        if (StockReplacement == null || StockReplacement.StockReplacementID <= 0)
			        {
				        throw new Exception("Sorry! This Transaction doesn't exist");
			        }

                    this.PTLDeleteAndStockUpdate(StockReplacement.StockReplacementLines.ToList(), StockReplacement.StockReplacementDate);

                    this.Delete(StockReplacement);
                    ts.Complete();
                    //transaction.Commit();
                }
			}
			catch (Exception e)
			{
				//If an errors occurs, we cancel all changes in database
                //transaction.Rollback();
				throw new Exception(e.Message + ": Check " + e.StackTrace);
			}
			//}
			return res;
		}
		/// <summary>
		/// Permet de supprimer les lignes de transfert et de réapprovisionner le stock
		/// </summary>
		/// <param name="StockReplacementLines"></param>
		/// <param name="dateOp"></param>
		public void PTLDeleteAndStockUpdate(List<StockReplacementLine> StockReplacementLines, DateTime dateOp)
		{
			//Réapprovisionnement du stock de départ
			foreach (StockReplacementLine ptl in StockReplacementLines)
			{
				ProductLocalization DepartureProdLoc = new ProductLocalization
				{
					LocalizationID = ptl.LocalizationID,
					ProductID = ptl.ProductID,
					ProductLocalizationDate = dateOp,
					ProductLocalizationStockQuantity = ptl.LineQuantity,
                    AveragePurchasePrice = ptl.LineUnitPrice,
                    NumeroSerie = ptl.NumeroSerie,
                    Marque = ptl.Marque,
                    Description=ptl.StockReplacementReason
                };

				_plRepository = new PLRepository(this.context);

				_plRepository.StockInput(DepartureProdLoc, DepartureProdLoc.ProductLocalizationStockQuantity,DepartureProdLoc.AveragePurchasePrice, dateOp,null);

			}

			//Suppression des lignes de transfert
			context.StockReplacementLines.RemoveRange(StockReplacementLines);
			context.SaveChanges();
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="StockReplacementLines"></param>
        /// <param name="StockReplacementID"></param>
		public void CreateStockReplacementLine(List<StockReplacementLine> StockReplacementLines, int StockReplacementID)
		{
			StockReplacementLines.ToList().ForEach(pt =>
			{
				StockReplacementLine ptLine = new StockReplacementLine()
				{
                    LocalizationID = pt.LocalizationID,
                    StockReplacementReason = pt.StockReplacementReason,
					LineQuantity = pt.LineQuantity,
                    LineUnitPrice= pt.LineUnitPrice,
					ProductID = pt.ProductID,
					StockReplacementID = StockReplacementID,
                    NumeroSerie = pt.NumeroSerie,
                    Marque = pt.Marque,
                    ProductDamageID=pt.ProductDamageID,
                    NumeroSerieDamage=pt.NumeroSerieDamage,
                    MarqueDamage=pt.MarqueDamage
                };
				context.StockReplacementLines.Add(ptLine);
			});
			context.SaveChanges();
		}

		/// <summary>
		/// réduire les stocks qui envoient les produits
		/// </summary>
		/// <param name="StockReplacement"></param>
        /// <param name="UserConnect"></param>
		public void OutPutStocksUpdate(StockReplacement StockReplacement,int UserConnect)
		{
            try
            {
                IProductLocalization _plRepository = new PLRepository(this.context);
                foreach (StockReplacementLine StockReplacementLine in StockReplacement.StockReplacementLines)
                {
                    ProductLocalization OutPutProdLoc = new ProductLocalization
                    {
                        LocalizationID = StockReplacementLine.LocalizationID,
                        ProductID = StockReplacementLine.ProductID,
                        ProductLocalizationDate = StockReplacement.StockReplacementDate,
                        ProductLocalizationStockQuantity = StockReplacementLine.LineQuantity,
                        AveragePurchasePrice = StockReplacementLine.LineUnitPrice ,
                        inventoryReason = "Stock Replacement",
                        RegisteredByID = UserConnect,
                        Description =StockReplacementLine.StockReplacementReason,
                        NumeroSerie = StockReplacementLine.NumeroSerie,
                        Marque = StockReplacementLine.Marque
                    };
                    //on enlève les produits du stock de départ
                    _plRepository.StockOutput(OutPutProdLoc, OutPutProdLoc.ProductLocalizationStockQuantity,OutPutProdLoc.AveragePurchasePrice, StockReplacement.StockReplacementDate, UserConnect);
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error : " + " Message = " + e.Message + " StackTrace = " + e.StackTrace);
            }
		}

		/// <summary>
		/// approvisionner les stocks qui recoivent les produits
		/// </summary>
		/// <param name="StockReplacement"></param>
		public void InputStocksUpdate(StockReplacement StockReplacement)
		{
			IProductLocalization _plRepository = new PLRepository(this.context);

			foreach (StockReplacementLine StockReplacementLine in StockReplacement.StockReplacementLines)
			{

				ProductLocalization StockInputProdLoc = new ProductLocalization
				{
                    LocalizationID = StockReplacementLine.LocalizationID,
					ProductID = StockReplacementLine.ProductID,
					ProductLocalizationDate = (DateTime)StockReplacement.StockReplacementDate,
					ProductLocalizationStockQuantity = StockReplacementLine.LineQuantity,
                    AveragePurchasePrice = StockReplacementLine.LineUnitPrice,
                    NumeroSerie = StockReplacementLine.NumeroSerie,
                    Marque = StockReplacementLine.Marque,
                    Description=StockReplacementLine.StockReplacementReason,
                    inventoryReason = "Stock Replacement",
                };
				//on approvisionne le stock d'arrivé
				_plRepository.StockInput(StockInputProdLoc, StockInputProdLoc.ProductLocalizationStockQuantity,StockInputProdLoc.AveragePurchasePrice, StockReplacement.StockReplacementDate,null);
			}
            
            
		}

		/// <summary>
		/// Cette méthode permet de modifier un transfert.
		/// Ici, nous faison un drop and create
		/// </summary>
		/// <param name="StockReplacement"></param>
		/// <returns></returns>
        public StockReplacement UpdateStockReplacement(StockReplacement StockReplacement)
		{
			//bool res = false;
			
           
				try
				{
                    using (TransactionScope ts = new TransactionScope())
                    {
                        
                        //1-Suppression des anciennes lignes de transfert
                        List<StockReplacementLine> StockReplacementLines = this.context.StockReplacementLines.Where(ptl => ptl.StockReplacementID == StockReplacement.StockReplacementID).ToList();
                        this.PTLDeleteAndStockUpdate(StockReplacementLines, StockReplacement.StockReplacementDate);

                        //2-Création des nouvelles lignes de transfert
                        this.CreateStockReplacementLine(StockReplacement.StockReplacementLines.ToList(), StockReplacement.StockReplacementID);

                        //3-Mise à jour des informations sur le transfert
                        this.Update(StockReplacement, StockReplacement.StockReplacementID);
                        //If an errors occurs, we cancel all changes in database
                        //transaction.Commit();
                        ts.Complete();
                    }
				}
				catch (Exception ex)
				{
					//transaction.Rollback();
					throw new Exception(ex.Message + ": Check " + ex.StackTrace);

				}
			//}

            return StockReplacement;
		}
		
	}
}
