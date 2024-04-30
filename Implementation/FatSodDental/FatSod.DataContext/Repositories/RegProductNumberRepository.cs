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
	public class RegProductNumberRepository : RepositorySupply<RegProductNumber>, IRegProductNumber
	{
		IProductLocalization _plRepository;

        public RegProductNumber DoRegProductNumber(RegProductNumber RegProductNumber,int UserConnect)
		{
			//int res = 0;
				try
				{
                    //Begin of transaction
                    using (TransactionScope ts = new TransactionScope())
                    {
				        //_plRepository = new PLRepository(this.context);

					    RegProductNumber doRegProductNumber = new RegProductNumber();

					    //ces 2 lignes qui suivent permettent de transformer un RegProductNumber en RegProductNumber 
					    //create a Map
					    Mapper.CreateMap<RegProductNumber, RegProductNumber>();
					    //use Map
					    doRegProductNumber = Mapper.Map<RegProductNumber>(RegProductNumber);
					    //default receiver 
					    doRegProductNumber.RegProductNumberLines = null;
					    this.Create(doRegProductNumber);
					    RegProductNumber.RegProductNumberID = doRegProductNumber.RegProductNumberID;

					    //Création des lignes de transfert
					    CreateRegProductNumberLine(RegProductNumber.RegProductNumberLines.ToList(), RegProductNumber.RegProductNumberID);

					    //Reduction du stock de départ
					    OutPutStocksUpdateDepart(RegProductNumber,UserConnect);

                        //augmentation du stock d'arrivee
                        InputStocksUpdateArrive(RegProductNumber,UserConnect);


                        //mise a jour du cpteur du transact number
                        TransactNumber trn = context.TransactNumbers.SingleOrDefault(t => t.TransactNumberCode == "CPNU");
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
            return RegProductNumber;
		}

		

		public bool CancelRegProductNumber(int RegProductNumberID)
		{
            bool res = false;
            try
            {
                //Begin of transaction
                using (TransactionScope ts = new TransactionScope())
                {
			        RegProductNumber RegProductNumber = this.Find(RegProductNumberID);

			        if (RegProductNumber == null || RegProductNumber.RegProductNumberID <= 0)
			        {
				        throw new Exception("Sorry! This Transaction doesn't exist");
			        }

                    this.PTLDeleteAndStockArriveUpdate(RegProductNumber.RegProductNumberLines.ToList(), RegProductNumber.RegProductNumberDate);

                    this.Delete(RegProductNumber);
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
		/// <param name="RegProductNumberLines"></param>
		/// <param name="dateOp"></param>
		public void PTLDeleteAndStockArriveUpdate(List<RegProductNumberLine> RegProductNumberLines, DateTime dateOp)
		{
			//Réapprovisionnement du stock de départ
			foreach (RegProductNumberLine ptl in RegProductNumberLines)
			{
				ProductLocalization DepartureProdLoc = new ProductLocalization
				{
					LocalizationID = ptl.LocalizationID,
					ProductID = ptl.NewProductID,
					ProductLocalizationDate = dateOp,
					ProductLocalizationStockQuantity = ptl.NewLineQuantity,
                    AveragePurchasePrice = context.ProductLocalizations.FirstOrDefault(p=>p.ProductID==ptl.OldProductID && p.LocalizationID==ptl.LocalizationID).AveragePurchasePrice
				};

				_plRepository = new PLRepository(this.context);

				_plRepository.StockInput(DepartureProdLoc, DepartureProdLoc.ProductLocalizationStockQuantity,DepartureProdLoc.AveragePurchasePrice, dateOp,null);

			}

			//Suppression des lignes de transfert
			context.RegProductNumberLines.RemoveRange(RegProductNumberLines);
			context.SaveChanges();
		}


		public void CreateRegProductNumberLine(List<RegProductNumberLine> RegProductNumberLines, int RegProductNumberID)
		{
			RegProductNumberLines.ToList().ForEach(pt =>
			{
				RegProductNumberLine ptLine = new RegProductNumberLine()
				{
                    LocalizationID = pt.LocalizationID,
                    OldProductID = pt.OldProductID,
					NewLineQuantity = pt.NewLineQuantity,
                    NewProductID = pt.NewProductID,
					RegProductNumberID = RegProductNumberID
				};
				context.RegProductNumberLines.Add(ptLine);
			});
			context.SaveChanges();
		}

		/// <summary>
		/// réduire les stocks qui envoient les produits
		/// </summary>
		/// <param name="RegProductNumber"></param>
		public void OutPutStocksUpdateDepart(RegProductNumber RegProductNumber,int UserConnect)
		{
            try
            {
                IProductLocalization _plRepository = new PLRepository(this.context);
                foreach (RegProductNumberLine RegProductNumberLine in RegProductNumber.RegProductNumberLines)
                    {
                    ProductLocalization OutPutProdLoc = new ProductLocalization
                    {
                        LocalizationID = RegProductNumberLine.LocalizationID,
                        ProductID = RegProductNumberLine.OldProductID,
                        ProductLocalizationDate = RegProductNumber.RegProductNumberDate,
                        ProductLocalizationStockQuantity = RegProductNumberLine.NewLineQuantity,
                        AveragePurchasePrice = context.ProductLocalizations.FirstOrDefault(p => p.ProductID == RegProductNumberLine.OldProductID && p.LocalizationID == RegProductNumberLine.LocalizationID).AveragePurchasePrice,
                        inventoryReason="Product Regularization" ,
                        RegisteredByID = UserConnect,
                        Description = RegProductNumber.RegProductNumberReference + " - " + RegProductNumberLine.OldProduct.ProductCode + " To " + RegProductNumberLine.NewProduct.ProductCode
                    };
                    //on enlève les produits du stock de départ
                    _plRepository.StockOutput(OutPutProdLoc, OutPutProdLoc.ProductLocalizationStockQuantity,OutPutProdLoc.AveragePurchasePrice, RegProductNumber.RegProductNumberDate, UserConnect);
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
		/// <param name="RegProductNumber"></param>
		public void InputStocksUpdateArrive(RegProductNumber RegProductNumber,int UserConnect)
		{
			IProductLocalization _plRepository = new PLRepository(this.context);

			foreach (RegProductNumberLine RegProductNumberLine in RegProductNumber.RegProductNumberLines)
			{

				ProductLocalization StockInputProdLoc = new ProductLocalization
				{
                    LocalizationID = RegProductNumberLine.LocalizationID,
					ProductID = RegProductNumberLine.NewProductID,
					ProductLocalizationDate = RegProductNumber.RegProductNumberDate,
					ProductLocalizationStockQuantity = RegProductNumberLine.NewLineQuantity,
                    AveragePurchasePrice = context.ProductLocalizations.FirstOrDefault(p => p.ProductID == RegProductNumberLine.OldProductID && p.LocalizationID == RegProductNumberLine.LocalizationID).AveragePurchasePrice,
                    inventoryReason = "Product Regularization" ,
                    RegisteredByID=UserConnect,
                    Description = RegProductNumber.RegProductNumberReference + " - "+ RegProductNumberLine.OldProduct.ProductCode +" To "+ RegProductNumberLine.NewProduct.ProductCode
				};
				//on approvisionne le stock d'arrivé
				_plRepository.StockInput(StockInputProdLoc, StockInputProdLoc.ProductLocalizationStockQuantity,StockInputProdLoc.AveragePurchasePrice, RegProductNumber.RegProductNumberDate, UserConnect);
			}
            
            
		}

		/// <summary>
		/// Cette méthode permet de modifier un transfert.
		/// Ici, nous faison un drop and create
		/// </summary>
		/// <param name="RegProductNumber"></param>
		/// <returns></returns>
        public RegProductNumber UpdateRegProductNumber(RegProductNumber RegProductNumber)
		{
			//bool res = false;
			
           
				try
				{
                    using (TransactionScope ts = new TransactionScope())
                    {
                        
                        //1-Suppression des anciennes lignes de transfert
                        List<RegProductNumberLine> RegProductNumberLines = this.context.RegProductNumberLines.Where(ptl => ptl.RegProductNumberID == RegProductNumber.RegProductNumberID).ToList();
                        this.PTLDeleteAndStockArriveUpdate(RegProductNumberLines, RegProductNumber.RegProductNumberDate);

                        //2-Création des nouvelles lignes de transfert
                        this.CreateRegProductNumberLine(RegProductNumber.RegProductNumberLines.ToList(), RegProductNumber.RegProductNumberID);

                        //3-Mise à jour des informations sur le transfert
                        this.Update(RegProductNumber, RegProductNumber.RegProductNumberID);
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

            return RegProductNumber;
		}
		
	}
}
