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
	public class ProductGiftRepository : RepositorySupply<ProductGift>, IProductGift
	{
		IProductLocalization _plRepository;

        public ProductGift DoProductGift(ProductGift ProductGift,int UserConnect)
		{
			//int res = 0;
				try
				{
                    //Begin of transaction
                    using (TransactionScope ts = new TransactionScope())
                    {
				        //_plRepository = new PLRepository(this.context);

					    ProductGift doProductGift = new ProductGift();

					    //ces 2 lignes qui suivent permettent de transformer un ProductGift en ProductGift 
					    //create a Map
					    Mapper.CreateMap<ProductGift, ProductGift>();
					    //use Map
					    doProductGift = Mapper.Map<ProductGift>(ProductGift);
					    //default receiver 
					    //transfert.ReceivedByID = 2;
					    doProductGift.ProductGiftLines = null;
					    this.Create(doProductGift);
					    ProductGift.ProductGiftID = doProductGift.ProductGiftID;

					    //Création des lignes de transfert
					    CreateProductGiftLine(ProductGift.ProductGiftLines.ToList(), ProductGift.ProductGiftID);

					    //Mise à jour du stock de départ
					    OutPutStocksUpdate(ProductGift,UserConnect);

                        //mise a jour du cpteur du transact number
                        TransactNumber trn = context.TransactNumbers.SingleOrDefault(t => t.TransactNumberCode == "PRGI");
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
            return ProductGift;
		}

		/// <summary>
		/// Cette méthode permet de constater la reception des produits par l'agence d'arrivée
		/// </summary>
		/// <param name="ProductGift"></param>
		/// <returns></returns>
        public ProductGift ValidateProductGift(ProductGift pt)
		{
			//bool res = false;

            ProductGift ProductGift = new ProductGift();
			//Begin of transaction
              
				try
				{
                    using (TransactionScope ts = new TransactionScope())
                    {
                        if (pt.ProductGiftID <= 0)
                        {
                            ProductGift ProductGiftInsert = new ProductGift();

                            //ces 2 lignes qui suivent permettent de transformer un ProductGift en ProductGift 
                            //create a Map
                            Mapper.CreateMap<ProductGift, ProductGift>();
                            //use Map
                            ProductGiftInsert = Mapper.Map<ProductGift>(pt);
                            //default receiver 
                            ProductGiftInsert.RegisteredByID = pt.RegisteredByID;
                            ProductGiftInsert.AutorizedByID = pt.AutorizedByID;
                            ProductGiftInsert.ProductGiftLines = null;
                            ProductGiftInsert.ProductGiftDate = pt.ProductGiftDate;
                            this.Create(ProductGiftInsert);
                            pt.ProductGiftID = ProductGiftInsert.ProductGiftID;

                            //Création des lignes de transfert
                            CreateProductGiftLine(pt.ProductGiftLines.ToList(), pt.ProductGiftID);
                            ProductGiftInsert = pt;
                        }
                        else
                        {
                            ProductGift = context.ProductGifts.AsNoTracking().SingleOrDefault(pt1 => pt1.ProductGiftID == pt.ProductGiftID);
                            ProductGift.ProductGiftDate = pt.ProductGiftDate;
                            ProductGift.RegisteredByID = pt.RegisteredByID;
                            ProductGift.AutorizedByID = pt.AutorizedByID;

                            List<ProductGiftLine> lines = ProductGift.ProductGiftLines.ToList();

                            ProductGift.ProductGiftLines = null;

                            context.ProductGifts.Attach(ProductGift);
                            context.Entry(ProductGift).State = EntityState.Modified;
                            context.SaveChanges();

                            foreach (ProductGiftLine ptl in lines)
                            {
                                //ptl.ArrivalLocalizationID = ArrivalLocalizationID;
                                context.ProductGiftLines.Attach(ptl);
                                context.Entry(ptl).State = EntityState.Modified;
                                context.SaveChanges();
                            }
                        }

                        //Mise à jour du stock d'arrivé
                        InputStocksUpdate(ProductGift);

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
            return ProductGift;
		}

       

		public bool CancelProductGift(int ProductGiftID)
		{
            bool res = false;
            try
            {
                //Begin of transaction
                using (TransactionScope ts = new TransactionScope())
                {
			        ProductGift ProductGift = this.Find(ProductGiftID);
			        
                    //if (ProductGift != null && ProductGift.ProductGiftID > 0 )
                    //{
                    //    throw new Exception("Verry Sorry this Operation was already validated. You can not be cancelled");
                    //}

			        if (ProductGift == null || ProductGift.ProductGiftID <= 0)
			        {
				        throw new Exception("Sorry! This Transaction doesn't exist");
			        }

                    this.PTLDeleteAndStockUpdate(ProductGift.ProductGiftLines.ToList(), ProductGift.ProductGiftDate);

                    this.Delete(ProductGift);
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
		/// <param name="ProductGiftLines"></param>
		/// <param name="dateOp"></param>
		public void PTLDeleteAndStockUpdate(List<ProductGiftLine> ProductGiftLines, DateTime dateOp)
		{
			//Réapprovisionnement du stock de départ
			foreach (ProductGiftLine ptl in ProductGiftLines)
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
                    Description=ptl.ProductGiftReason
                };

				_plRepository = new PLRepository(this.context);

				_plRepository.StockInput(DepartureProdLoc, DepartureProdLoc.ProductLocalizationStockQuantity,DepartureProdLoc.AveragePurchasePrice, dateOp,null);

			}

			//Suppression des lignes de transfert
			context.ProductGiftLines.RemoveRange(ProductGiftLines);
			context.SaveChanges();
		}


		public void CreateProductGiftLine(List<ProductGiftLine> ProductGiftLines, int ProductGiftID)
		{
			ProductGiftLines.ToList().ForEach(pt =>
			{
				ProductGiftLine ptLine = new ProductGiftLine()
				{
                    LocalizationID = pt.LocalizationID,
                    ProductGiftReason = pt.ProductGiftReason,
					LineQuantity = pt.LineQuantity,
                    LineUnitPrice= pt.LineUnitPrice,
					ProductID = pt.ProductID,
					ProductGiftID = ProductGiftID,
                    NumeroSerie = pt.NumeroSerie,
                    Marque = pt.Marque
                };
				context.ProductGiftLines.Add(ptLine);
			});
			context.SaveChanges();
		}

		/// <summary>
		/// réduire les stocks qui envoient les produits
		/// </summary>
		/// <param name="ProductGift"></param>
		public void OutPutStocksUpdate(ProductGift ProductGift,int UserConnect)
		{
            try
            {
                IProductLocalization _plRepository = new PLRepository(this.context);
                foreach (
                    ProductGiftLine ProductGiftLine in 
                    ProductGift.ProductGiftLines)
                {
                    ProductLocalization OutPutProdLoc = new ProductLocalization
                    {
                        LocalizationID = ProductGiftLine.LocalizationID,
                        ProductID = ProductGiftLine.ProductID,
                        ProductLocalizationDate = ProductGift.ProductGiftDate,
                        ProductLocalizationStockQuantity = ProductGiftLine.LineQuantity,
                        AveragePurchasePrice = ProductGiftLine.LineUnitPrice ,
                        inventoryReason = "Product Gift",
                        RegisteredByID = UserConnect,
                        Description =ProductGiftLine.ProductGiftReason,
                        NumeroSerie = ProductGiftLine.NumeroSerie,
                        Marque = ProductGiftLine.Marque
                    };
                    //on enlève les produits du stock de départ
                    _plRepository.StockOutput(OutPutProdLoc, OutPutProdLoc.ProductLocalizationStockQuantity,OutPutProdLoc.AveragePurchasePrice, ProductGift.ProductGiftDate, UserConnect);
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
		/// <param name="ProductGift"></param>
		public void InputStocksUpdate(ProductGift ProductGift)
		{
			IProductLocalization _plRepository = new PLRepository(this.context);

			foreach (
                ProductGiftLine ProductGiftLine in ProductGift.ProductGiftLines)
			{
                
                ProductLocalization StockInputProdLoc = new ProductLocalization
				{
                    LocalizationID = ProductGiftLine.LocalizationID,
					ProductID = ProductGiftLine.ProductID,
					ProductLocalizationDate = (DateTime)ProductGift.ProductGiftDate,
					ProductLocalizationStockQuantity = ProductGiftLine.LineQuantity,
                    AveragePurchasePrice = ProductGiftLine.LineUnitPrice,
                    NumeroSerie = ProductGiftLine.NumeroSerie,
                    Marque = ProductGiftLine.Marque,
                    Description=ProductGiftLine.ProductGiftReason
                };
				//on approvisionne le stock d'arrivé
				_plRepository.StockInput(StockInputProdLoc, StockInputProdLoc.ProductLocalizationStockQuantity,StockInputProdLoc.AveragePurchasePrice, ProductGift.ProductGiftDate,null);
			}
            
            
		}

		/// <summary>
		/// Cette méthode permet de modifier un transfert.
		/// Ici, nous faison un drop and create
		/// </summary>
		/// <param name="ProductGift"></param>
		/// <returns></returns>
        public ProductGift UpdateProductGift(ProductGift ProductGift)
		{
			//bool res = false;
			
           
				try
				{
                    using (TransactionScope ts = new TransactionScope())
                    {
                        
                        //1-Suppression des anciennes lignes de transfert
                        List<ProductGiftLine> ProductGiftLines = this.context.ProductGiftLines.Where(ptl => ptl.ProductGiftID == ProductGift.ProductGiftID).ToList();
                        this.PTLDeleteAndStockUpdate(ProductGiftLines, ProductGift.ProductGiftDate);

                        //2-Création des nouvelles lignes de transfert
                        this.CreateProductGiftLine(ProductGift.ProductGiftLines.ToList(), ProductGift.ProductGiftID);

                        //3-Mise à jour des informations sur le transfert
                        this.Update(ProductGift, ProductGift.ProductGiftID);
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

            return ProductGift;
		}
		
	}
}
