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
	public class TransfertRepository : RepositorySupply<ProductTransfert>, ITransfert
	{
		IProductLocalization _plRepository;

        public ProductTransfert Sending(ProductTransfert prodTrans,int UserConnect)
		{
			//int res = 0;
				try
				{
                    //Begin of transaction
                    using (TransactionScope ts = new TransactionScope())
                    {
				        //_plRepository = new PLRepository(this.context);

					    ProductTransfert transfert = new ProductTransfert();

					    //ces 2 lignes qui suivent permettent de transformer un ProductTransfert en ProductTransfert 
					    //create a Map
					    Mapper.CreateMap<ProductTransfert, ProductTransfert>();
					    //use Map
					    transfert = Mapper.Map<ProductTransfert>(prodTrans);
					    //default receiver 
					    //transfert.ReceivedByID = 2;
					    transfert.ProductTransfertLines = null;
					    this.Create(transfert);
					    prodTrans.ProductTransfertId = transfert.ProductTransfertId;

					    //Création des lignes de transfert
					    CreateProdTransLine(prodTrans.ProductTransfertLines.ToList(), prodTrans.ProductTransfertId,true);

					    //Mise à jour du stock de départ
                        DepartureStocksUpdate(prodTrans, UserConnect);

                        //mise a jour du cpteur du transact number
                        TransactNumber trn = context.TransactNumbers.SingleOrDefault(t => t.TransactNumberCode == "TRAN");
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
            return prodTrans;
		}

		/// <summary>
		/// Cette méthode permet de constater la reception des produits par l'agence d'arrivée
		/// </summary>
		/// <param name="prodTrans"></param>
		/// <returns></returns>
        public ProductTransfert Receiving(ProductTransfert pt, int UserConnect)
		{
			//bool res = false;

            ProductTransfert prodTrans = new ProductTransfert();
			//Begin of transaction
              
				try
				{
                    using (TransactionScope ts = new TransactionScope())
                    {
                        if (pt.ProductTransfertId <= 0)
                        {
                            ProductTransfert transfert = new ProductTransfert();

                            //ces 2 lignes qui suivent permettent de transformer un ProductTransfert en ProductTransfert 
                            //create a Map
                            Mapper.CreateMap<ProductTransfert, ProductTransfert>();
                            //use Map
                            transfert = Mapper.Map<ProductTransfert>(pt);
                            //default receiver 
                            //transfert.ReceivedByID = 2;
                            transfert.ProductTransfertLines = null;
                            transfert.IsReceived = true;
                            transfert.ProductTransfertDate = pt.ReceivedDate.Value;
                            this.Create(transfert);
                            pt.ProductTransfertId = transfert.ProductTransfertId;

                            //Création des lignes de transfert
                            CreateProdTransLine(pt.ProductTransfertLines.ToList(), pt.ProductTransfertId,false);
                            prodTrans = pt;
                        }
                        else
                        {
                            prodTrans = context.ProductTransferts.AsNoTracking().SingleOrDefault(pt1 => pt1.ProductTransfertId == pt.ProductTransfertId);
                            prodTrans.ReceivedById = pt.ReceivedById;
                            prodTrans.ReceivedDate = pt.ReceivedDate;
                            prodTrans.IsReceived = true;

                            List<ProductTransfertLine> lines = prodTrans.ProductTransfertLines.ToList();

                            prodTrans.ProductTransfertLines = null;

                            context.ProductTransferts.Attach(prodTrans);
                            context.Entry(prodTrans).State = EntityState.Modified;
                            context.SaveChanges();

                            foreach (ProductTransfertLine ptl in lines)
                            {
                                //ptl.ArrivalLocalizationID = ArrivalLocalizationID;
                                context.ProductTransfertLines.Attach(ptl);
                                context.Entry(ptl).State = EntityState.Modified;
                                context.SaveChanges();
                            }
                        }

                        //Mise à jour du stock d'arrivé
                        ArrivalStocksUpdate(prodTrans, UserConnect);

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
            return prodTrans;
		}

		public bool CancelTransfert(int prodTransID)
		{
            bool res = false;
            try
            {
                //Begin of transaction
                using (TransactionScope ts = new TransactionScope())
                {
			        ProductTransfert prodTrans = this.Find(prodTransID);
			        //_plRepository = new PLRepository(this.context);

			

			        if (prodTrans != null && prodTrans.ProductTransfertId > 0 && prodTrans.IsReceived == true)
			        {
				        throw new Exception("Verry Sorry this transfert was already validated. You can not be cancelled");
			        }

			        if (prodTrans == null || prodTrans.ProductTransfertId <= 0)
			        {
				        throw new Exception("Sorry! This transfert doesn't exist");
			        }

                    this.PTLDeleteAndStockUpdate(prodTrans.ProductTransfertLines.ToList(), prodTrans.ProductTransfertDate);

                    this.Delete(prodTrans);
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
		/// <param name="ProductTransfertLines"></param>
		/// <param name="dateOp"></param>
		public void PTLDeleteAndStockUpdate(List<ProductTransfertLine> ProductTransfertLines, DateTime dateOp)
		{
			//Réapprovisionnement du stock de départ
			foreach (ProductTransfertLine ptl in ProductTransfertLines)
			{
				var DepartureProdLoc = context.ProductLocalizations.AsNoTracking()
										.FirstOrDefault(s => s.ProductLocalizationID == ptl.DepartureStockId);
				DepartureProdLoc.ProductLocalizationDate = dateOp;
				DepartureProdLoc.ProductLocalizationStockQuantity = ptl.Quantity;
				DepartureProdLoc.AveragePurchasePrice = ptl.UnitPrice;
				
				_plRepository = new PLRepository(this.context);

				_plRepository.StockInput(DepartureProdLoc, DepartureProdLoc.ProductLocalizationStockQuantity,DepartureProdLoc.AveragePurchasePrice, dateOp,null);

			}

			//Suppression des lignes de transfert
			context.ProductTransfertLines.RemoveRange(ProductTransfertLines);
			context.SaveChanges();
		}

		public void CreateProdTransLine(List<ProductTransfertLine> prodTransLines, int prodTransID,bool isTransfer)
		{
			prodTransLines.ToList().ForEach(pt =>
			{
				pt.ProductTransfertId = prodTransID;
			});

			context.ProductTransfertLines.AddRange(prodTransLines);
			context.SaveChanges();
		}

		/// <summary>
		/// réduire les stocks qui envoient les produits
		/// </summary>
		/// <param name="prodTrans"></param>
        public void DepartureStocksUpdate(ProductTransfert prodTrans, int UserConnect)
		{
            try
            {
                IProductLocalization _plRepository = new PLRepository(this.context);
                foreach (
                    ProductTransfertLine prodTransLine in 
                    prodTrans.ProductTransfertLines)
                {
					var DepartureProdLoc = context.ProductLocalizations.AsNoTracking()
										   .FirstOrDefault(s => s.ProductLocalizationID == prodTransLine.DepartureStockId);
					DepartureProdLoc.ProductLocalizationDate = prodTrans.ProductTransfertDate;
					DepartureProdLoc.ProductLocalizationStockQuantity = prodTransLine.Quantity;
					DepartureProdLoc.AveragePurchasePrice = prodTransLine.UnitPrice;
					DepartureProdLoc.inventoryReason = "Product Transfer - Sending";
					DepartureProdLoc.RegisteredByID = UserConnect;
					DepartureProdLoc.Description = prodTrans.ProductTransfertReference;
					DepartureProdLoc.inventoryReason = "Stock output - Product Transfer - Sending - prodTrans.ProductTransfertReference";
					//on enlève les produits du stock de départ
					_plRepository.StockOutput(DepartureProdLoc, DepartureProdLoc.ProductLocalizationStockQuantity,DepartureProdLoc.AveragePurchasePrice, prodTrans.ProductTransfertDate, UserConnect);
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
		/// <param name="prodTrans"></param>
        public void ArrivalStocksUpdate(ProductTransfert prodTrans, int UserConnect)
		{
			IProductLocalization _plRepository = new PLRepository(this.context);

			foreach (
                ProductTransfertLine prodTransLine in prodTrans.ProductTransfertLines)
			{
                
				var arrivalProdLoc = context.ProductLocalizations.AsNoTracking()
										   .FirstOrDefault(s => s.ProductLocalizationID == prodTransLine.ArrivalStockId);
				arrivalProdLoc.ProductLocalizationDate = prodTrans.ReceivedDate.Value;
				arrivalProdLoc.ProductLocalizationStockQuantity = prodTransLine.Quantity;
				arrivalProdLoc.AveragePurchasePrice = prodTransLine.UnitPrice;
				arrivalProdLoc.RegisteredByID = UserConnect;
				arrivalProdLoc.Description = prodTrans.ProductTransfertReference;
				arrivalProdLoc.inventoryReason = "Stock output - Product Transfer - Receiving - prodTrans.ProductTransfertReference";

				//on approvisionne le stock d'arrivé
				_plRepository.StockInput(arrivalProdLoc, arrivalProdLoc.ProductLocalizationStockQuantity,arrivalProdLoc.AveragePurchasePrice, prodTrans.ReceivedDate.Value, UserConnect);
			}            
		}

		/// <summary>
		/// Cette méthode permet de modifier un transfert.
		/// Ici, nous faison un drop and create
		/// </summary>
		/// <param name="prodTrans"></param>
		/// <returns></returns>
        public ProductTransfert UpdateTransfert(ProductTransfert prodTrans)
		{
			//bool res = false;
			
           
				try
				{
                    using (TransactionScope ts = new TransactionScope())
                    {
                        if (prodTrans.IsReceived == true)
                        {
                            throw new Exception("You can not update this transfert because it has already be received");
                        }
                        //1-Suppression des anciennes lignes de transfert
                        List<ProductTransfertLine> ProductTransfertLines = this.context.ProductTransfertLines.Where(ptl => ptl.ProductTransfertId == prodTrans.ProductTransfertId).ToList();
                        this.PTLDeleteAndStockUpdate(ProductTransfertLines, prodTrans.ProductTransfertDate);

                        //2-Création des nouvelles lignes de transfert
                        this.CreateProdTransLine(prodTrans.ProductTransfertLines.ToList(), prodTrans.ProductTransfertId,true);

                        //3-Mise à jour des informations sur le transfert
                        this.Update(prodTrans, prodTrans.ProductTransfertId);
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

            return prodTrans;
		}
		/// <summary>
		/// Cette méthode renvoie tous les transferts qui n'ont pas encore été reçus
		/// NB : On ne peut modifier que les transferts qui n'ont pas encore été recçs
		/// </summary>
		/// <param name="branch">Branches de départ des transferts</param>
		/// <returns></returns>
		public List<ProductTransfert> GetAllPendingTransfert(Branch departureBranch)
		{
			if (departureBranch == null || departureBranch.BranchID <= 0)
			{
				return null;
			}
			List<ProductTransfert> res = null;

			res = context.ProductTransferts.Where(pt => pt.DepartureBranchId == departureBranch.BranchID && pt.IsReceived == false).ToList();

			return res;
		}
		/// <summary>
		/// Cette méthode renvoie tous les transferts qui n'ont pas encore été receptionnées par le recepteur
		/// </summary>
		/// <param name="arrivalBranch">Branches d'arrivée des transferts</param>
		/// <returns></returns>
		public List<ProductTransfert> GetAllInProgressTransfert(Branch arrivalBranch)
		{
			List<ProductTransfert> res = null;

			res = context.ProductTransferts.Where(pt => pt.ArrivalBranchId == arrivalBranch.BranchID && pt.IsReceived == false).ToList();

			return res;
		}
	}
}
