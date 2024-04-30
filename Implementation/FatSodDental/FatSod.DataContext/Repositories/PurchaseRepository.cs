using FatSod.Supply.Abstracts;
using FatSod.Supply.Entities;
using FatSod.Supply.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using AutoMapper;
using FastSod.Utilities.Util;
using FatSod.DataContext.Concrete;
using System.Transactions;


namespace FatSod.DataContext.Repositories
{
	public class PurchaseRepository : RepositorySupply<Purchase>, IPurchase
	{
		public PurchaseRepository()
			: base()
		{

		}
		public PurchaseRepository(EFDbContext ctx)
			: base(ctx)
		{

		}
		public Purchase CreatePurchase(Purchase purchase)
		{
			//Begin of transaction
            
				try
				{
                    using (TransactionScope ts = new TransactionScope())
                    {
					SavePurchase(purchase);

                        //transaction.Commit();
                        ts.Complete();
				}
					

				}
				catch (Exception e)
				{
					//If an errors occurs, we cancel all changes in database
                    //transaction.Rollback();
					throw new Exception(e.Message + ": Check " + e.StackTrace);
				}
                return purchase;

			}

		public void ValidatePurchase(SupplierOrder supplierOrder)
		{

		}


		public void CreateProdLocForPurLine(PurchaseLine purLine, Purchase purchase, bool update)
		{
            double OldSafetyStockQuantity = 0d;
            double OldStockQuantity = 0d;
            double OldStockUnitPrice = 0d;

			ProductLocalization prodLoc = new ProductLocalization()
			{
				LocalizationID = purLine.LocalizationID,
				ProductID = purLine.ProductID,
				ProductLocalizationDate = purchase.PurchaseDate,
				ProductLocalizationStockQuantity = purLine.LineQuantity,
				ProductLocalizationStockSellingPrice = purLine.LineUnitPrice,
				ProductLocalizationSafetyStockQuantity = 0,
				AveragePurchasePrice = this.GetAveragePurchasePrice(purLine, purchase)
			};

			ProductLocalization prodLoc1 = this.context.ProductLocalizations.FirstOrDefault(pl2 => pl2.LocalizationID == prodLoc.LocalizationID && pl2.ProductID == prodLoc.ProductID);

			//vérification de l'existence du stock(lieu de stockage)
			if (prodLoc1 != null && prodLoc1.ProductLocalizationID > 0)//le stock existe déjà il faut le mettre à jour
			{
                prodLoc1.ProductLocalizationDate = purchase.PurchaseDate;// prodLoc.ProductLocalizationDate;
                prodLoc1.ProductLocalizationSafetyStockQuantity = prodLoc.ProductLocalizationSafetyStockQuantity;

				//affectation du prix d'achat moyen du stock
                prodLoc1.AveragePurchasePrice = this.GetAveragePurchasePrice(purLine, purchase);

                prodLoc1.ProductLocalizationStockQuantity += prodLoc.ProductLocalizationStockQuantity;

                //prodLoc1.ProductLocalizationDate = prodLoc.ProductLocalizationDate;
				//this.context.ProductLocalizations.Attach(actualProdLoc);
				//context.Entry(actualProdLoc).State = EntityState.Modified;
				this.context.SaveChanges();
                OldSafetyStockQuantity = prodLoc1.ProductLocalizationSafetyStockQuantity;
                OldStockQuantity = prodLoc1.ProductLocalizationStockQuantity;
                OldStockUnitPrice = prodLoc1.ProductLocalizationStockSellingPrice;
			}
			else
			{
				this.context.ProductLocalizations.Add(prodLoc);
				this.context.SaveChanges();
                OldSafetyStockQuantity = 0d;
                OldStockQuantity = 0d;
                OldStockUnitPrice = 0d;
			}

            //HISTORISATION DU STOCK
            DateTime date = purchase.PurchaseDate; // prodLoc.ProductLocalizationDate;
            DateTime actualDate = DateTime.Now;
            date = date.AddHours(actualDate.Hour);
            date = date.AddMinutes(actualDate.Minute);
            date = date.AddSeconds(actualDate.Second);
            InventoryHistoric inventoryHistoric = new InventoryHistoric
            {
                //les nouvelles infos
                NewSafetyStockQuantity = prodLoc.ProductLocalizationSafetyStockQuantity,
                NewStockQuantity = prodLoc.ProductLocalizationStockQuantity,
                NEwStockUnitPrice = prodLoc.ProductLocalizationStockSellingPrice,
                //les anciennes infos
                OldSafetyStockQuantity = OldSafetyStockQuantity,
                OldStockQuantity = OldStockQuantity,
                OldStockUnitPrice = OldStockUnitPrice,
                //Autres informations
                InventoryDate = date,
                inventoryReason = "Product Purchase",
                LocalizationID = prodLoc.LocalizationID,
                ProductID = prodLoc.ProductID,
                RegisteredByID = purchase.PurchaseRegisterID,
                AutorizedByID = purchase.PurchaseRegisterID,
                CountByID = purchase.PurchaseBringerID,
                StockStatus = "INPUT",
                Description = purchase.PurchaseReference,
                Quantity = purLine.LineQuantity
            };
            context.InventoryHistorics.Add(inventoryHistoric);
            context.SaveChanges();
		}

		/// <summary>
		/// cette méthode retourne le prix d'achat d'un PurchaseLine en intégrant les réductions, la tva et le transport
		/// </summary>
		/// <param name="purLine"></param>
		/// <param name="purchase"></param>
		/// <returns></returns>
		public double GetAveragePurchasePrice(PurchaseLine purLine, Purchase purchase)
		{
			ProductLocalization actualProdLoc = this.context.ProductLocalizations.FirstOrDefault(pl2 => pl2.LocalizationID == purLine.LocalizationID && pl2.ProductID == purLine.ProductID);

			double res = purLine.LineUnitPrice;
			if (actualProdLoc != null && actualProdLoc.ProductLocalizationID > 0)
			{
				double oldPurchasePrice = actualProdLoc.AveragePurchasePrice * actualProdLoc.ProductLocalizationStockQuantity;
				double newPurchasePrice = purLine.LineQuantity * purLine.LineUnitPrice;
				double quantity = actualProdLoc.ProductLocalizationStockQuantity + purLine.LineQuantity;
                if (quantity > 0) res = Math.Round((newPurchasePrice + oldPurchasePrice) / quantity);
                else res = 0;
			}


			return res;
		}

		private void CreatePurLine(PurchaseLine purLine, Purchase purchase)
		{
			purLine.PurchaseID = purchase.PurchaseID;
			purLine.LineID = 0;
			this.context.PurchaseLines.Add(purLine);
			this.context.SaveChanges();
		}

		public void CreatePurLine(List<PurchaseLine> PurchaseLines, int purchaseID)
		{
			PurchaseLines.ToList().ForEach(pl =>
			{
				PurchaseLine plSave = new PurchaseLine()
				{
					PurchaseID = purchaseID,
					ProductID = pl.ProductID,
					LineUnitPrice = pl.LineUnitPrice,
					LineQuantity = pl.LineQuantity,
					LocalizationID = pl.LocalizationID
				};
				context.PurchaseLines.Add(plSave);
			});
		}

		public Purchase SavePurchase(Purchase purchase)
		{
			bool res = false;
			double TrancheAmount = 0d;
			//a revoir kd on aura resolu le pb des transactions
			if (purchase.PaymentDelay == 0 && purchase.PaymentMethodID > 0)
			{
				PaymentMethod paymentMethod = context.PaymentMethods.Find(purchase.PaymentMethodID);
				if (purchase.CurrentSupplierSlice.SliceAmount >= purchase.TotalPriceTTC)
				{
					purchase.StatutPurchase = SalePurchaseStatut.Paid;
				}
				else
				{//purchase.CurrentSupplierSlice.SliceAmount < purchase.TotalPriceTTC
					purchase.StatutPurchase = SalePurchaseStatut.Advanced;
				}

				//We determine type of this Purchase
				if (paymentMethod is Till)
				{

					TillPurchase tillPurchase = new TillPurchase();
					//ces 2 lignes qui suivent permettent de transformer un ProductModel en Product 
					//create a Map
					Mapper.CreateMap<Purchase, TillPurchase>();
					//use Map
					tillPurchase = Mapper.Map<TillPurchase>(purchase);
					tillPurchase.TillID = paymentMethod.ID;
					tillPurchase.PurchaseLines = null;
					tillPurchase.CurrentSupplierSlice = null;
					tillPurchase = context.TillPurchases.Add(tillPurchase);
					context.SaveChanges();
					this.CreatePurLine(purchase.PurchaseLines.ToList(), tillPurchase.PurchaseID);
					purchase.PurchaseID = tillPurchase.PurchaseID;

				}


				if (paymentMethod is Bank)
				{

					BankPurchase bankPurchase = new BankPurchase();
					//ces 2 lignes qui suivent permettent de transformer un ProductModel en Product 
					//create a Map
					Mapper.CreateMap<Purchase, BankPurchase>();
					//use Map
					bankPurchase = Mapper.Map<BankPurchase>(purchase);

					bankPurchase.BankID = paymentMethod.ID;

					bankPurchase.PurchaseLines = null;
					bankPurchase.CurrentSupplierSlice = null;

					bankPurchase = context.BankPurchases.Add(bankPurchase);
					context.SaveChanges();

					this.CreatePurLine(purchase.PurchaseLines.ToList(), bankPurchase.PurchaseID);

					purchase.PurchaseID = bankPurchase.PurchaseID;

				}
			}

			//achat à crédit
			if (purchase.PaymentDelay > 0)
			{

				Purchase purchaseToSave = new Purchase();
				//ces 2 lignes qui suivent permettent de transformer un ProductModel en Product 
				//create a Map
				Mapper.CreateMap<Purchase, Purchase>();
				//use Map
				purchaseToSave = Mapper.Map<Purchase>(purchase);
				purchaseToSave.PurchaseLines = null;
				purchaseToSave.CurrentSupplierSlice = null;

				purchaseToSave = context.Purchases.Add(purchaseToSave);
				context.SaveChanges();
				this.CreatePurLine(purchase.PurchaseLines.ToList(), purchaseToSave.PurchaseID);

				purchase.PurchaseID = purchaseToSave.PurchaseID;
			}

			//ProductLocalization modification
			foreach (PurchaseLine purLine in purchase.PurchaseLines)
			{
				this.CreateProdLocForPurLine(purLine, purchase, false);
			}

			if (purchase.PaymentMethodID > 0)
			{

				//persistence de la tranche pour le règlement de la totalité ou partie de la l'achat.
				purchase.CurrentSupplierSlice.PurchaseID = purchase.PurchaseID;
				//purchase.CurrentSupplierSlice.OperatorID = userc;
				context.SupplierSlices.Add(purchase.CurrentSupplierSlice);
				context.SaveChanges();
				TrancheAmount = context.SupplierSlices.SingleOrDefault(s => s.PurchaseID == purchase.PurchaseID).SliceAmount;
			}
			
			//we take a extra price
			double totalPriceHT = purchase.PurchaseLines.Select(pl => (pl.LineQuantity * pl.LineUnitPrice)).Sum();
			ExtraPrice extra = Util.ExtraPrices(totalPriceHT, purchase.RateReduction, purchase.RateDiscount, purchase.Transport, purchase.VatRate);
			//Accountig operations
			Purchase acountablePurchase = new Purchase()
			{
				BranchID = purchase.BranchID,
				SupplierID = purchase.SupplierID,
				PaymentMethodID = purchase.PaymentMethod != null ? purchase.PaymentMethodID : 0,
				PurchaseDate = purchase.PurchaseDate,
				PurchaseID = purchase.PurchaseID,
				TotalPriceHT = extra.TotalHT,//montant brut
				PurchaseReference = purchase.PurchaseReference,
				DeviseID = purchase.DeviseID,
				PaymentDelay = purchase.PaymentDelay,
				RateReduction = purchase.RateReduction,
				RateDiscount = purchase.RateDiscount,
				ReductionAmount = extra.ReductionAmount, //reduction
				DiscountAmount = extra.DiscountAmount, //escompe
				Transport = purchase.Transport, //transport
				TVAAmount = extra.TVAAmount, //tva
				TotalPriceTTC = extra.TotalTTC, //montant net a verser,
				PurchasePriceAdvance = TrancheAmount,
				NetFinancier = extra.NetFinan,
				NetCommercial = extra.NetCom,
				StatutPurchase = purchase.StatutPurchase,
				OldStatutPurchase = purchase.OldStatutPurchase,
				PurchaseValidate = purchase.PurchaseValidate,

			};
			//////comptabilisation
			////IAccountOperation opaccount = new AccountOperationRepository(context);
			////res = opaccount.ecritureComptableFinal(acountablePurchase);
			////if (!res)
			////{
			////	throw new Exception("Une erreur s'est produite lors de comptabilisation de l'achat ");
			////}
			////context.SaveChanges();

			return purchase;
		}

		public bool DeletePurchase(int PurchaseID)
		{
			//Begin of transaction
            bool res = false ;
				try
				{
                    using (TransactionScope ts = new TransactionScope())
                    {
					RemovePurchase(PurchaseID);
                        //transaction.Commit();
                        res = true;
                        ts.Complete();
                    }

				}
				catch (Exception e)
				{
					//If an errors occurs, we cancel all changes in database
                    //transaction.Rollback();
                    res = false;
					throw new Exception("Une erreur s'est produite lors de la suppression de l'achat car : " + e.Message);
				}
                return res;

		}

		public bool RemovePurchase(int PurchaseID)
		{
			bool res = false;


			//réduction du stock concernée
			List<PurchaseLine> purLines = this.context.PurchaseLines.Where(pl => pl.PurchaseID == PurchaseID).ToList();
			foreach (PurchaseLine pl1 in purLines)
			{
				ProductLocalization prodLoc = this.context.ProductLocalizations.FirstOrDefault(prodLoc1 => prodLoc1.ProductID == pl1.ProductID && prodLoc1.LocalizationID == pl1.LocalizationID);
				if (prodLoc != null && prodLoc.ProductLocalizationID > 0)
				{
					ProductLocalization actualProdLoc = this.context.ProductLocalizations.FirstOrDefault(pl2 => pl2.LocalizationID == prodLoc.LocalizationID && pl2.ProductID == prodLoc.ProductID);
					actualProdLoc.ProductLocalizationStockQuantity -= pl1.LineQuantity;
					this.context.ProductLocalizations.Attach(actualProdLoc);
					context.Entry(actualProdLoc).State = EntityState.Modified;
					this.context.SaveChanges();
				}
			}
			//suppression des lignes d'achat
			this.context.PurchaseLines.RemoveRange(purLines);
			this.context.SaveChanges();
			//suppression de l'achat
			this.context.Purchases.Remove(this.context.Purchases.SingleOrDefault(pur => pur.PurchaseID == PurchaseID));
			this.context.SaveChanges();
			res = true;



			return res;
		}

		public Purchase UpdatePurchase(Purchase purchase)
		{
            
				try
				{
                    using (TransactionScope ts = new TransactionScope())
                    {
					//this.CancelPurchase(purchase);
					purchase.PurchaseID = 0;
					this.SavePurchase(purchase);

                        //transaction.Commit();
                        ts.Complete();
                    }

				}
				catch (Exception e)
				{
					//If an errors occurs, we cancel all changes in database
                    //transaction.Rollback();
					throw new Exception("Une erreur s'est produite lors de la mise à jour de l'achat " + e.Message);
				}
			//}


			return purchase;
		}

	}
}

