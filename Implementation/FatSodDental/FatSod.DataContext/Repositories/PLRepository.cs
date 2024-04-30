using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FatSod.Security.Abstracts;
using FatSod.Security.Entities;
using FatSod.DataContext.Concrete;
using FatSod.Supply.Entities;
using FatSod.Supply.Abstracts;
using System.Data.Entity;
using AutoMapper;
using System.Transactions;
using FatSod.Ressources;
using FastSod.Utilities.Util;

namespace FatSod.DataContext.Repositories
{
    public class PLRepository : RepositorySupply<ProductLocalization>, IProductLocalization
    {
        IInventoryDirectory iventoryDirectoryRepository;
        public PLRepository(EFDbContext context)
        {
            this.context = context;
        }
        public bool IsPLExist(ProductLocalization pl)
        {
            bool res = false;
            ProductLocalization pl1 = new ProductLocalization();
            Product existprod = this.context.Products.Find(pl.ProductID);
            if (!(existprod.Category.isSerialNumberNull))
            {
                pl1 = this.context.ProductLocalizations.FirstOrDefault(pl2 => pl2.LocalizationID == pl.LocalizationID && pl2.ProductID == pl.ProductID);
            }
            else
            {
                pl1 = this.context.ProductLocalizations.FirstOrDefault(pl2 => pl2.LocalizationID == pl.LocalizationID && pl2.ProductID == pl.ProductID
                && pl2.NumeroSerie == pl.NumeroSerie && pl2.Marque == pl.Marque);
            }
            //ProductLocalization pl1 = this.context.ProductLocalizations.FirstOrDefault(pl2 => pl2.LocalizationID == pl.LocalizationID && pl2.ProductID == pl.ProductID
            //&& pl2.NumeroSerie == pl.NumeroSerie && pl2.Marque == pl.Marque);
            if (pl1 != null && pl1.ProductLocalizationID > 0) { res = true; }
            return res;

        }
        public bool checkQtyInStock(int productID, int localizationID, double entryQty)
        {
            bool res = true;
            ProductLocalization prodLoc = this.context.ProductLocalizations.FirstOrDefault(pl => pl.ProductID == productID && pl.LocalizationID == localizationID);

            double stockQty = (prodLoc != null) ? prodLoc.ProductLocalizationStockQuantity : 0;
            double safetyQty = (prodLoc != null) ? prodLoc.ProductLocalizationSafetyStockQuantity : 0;

            if (stockQty - entryQty < 0) //plus de produit en stock
            {
                res = false;
                throw new Exception(prodLoc.ProductLabel + " : " + Resources.EnoughQuantityStock + " " + (stockQty));

            }
            
            return res;
        }

        public bool checkQtyInStock(int productID, int localizationID, double entryQty, string NumeroSerie, string Marque)
        {
            bool res = true;
            ProductLocalization prodLoc = this.context.ProductLocalizations.FirstOrDefault(pl => pl.ProductID == productID && pl.LocalizationID == localizationID
            && pl.NumeroSerie==NumeroSerie && pl.Marque==Marque);

            double stockQty = (prodLoc != null) ? prodLoc.ProductLocalizationStockQuantity : 0;
            double safetyQty = (prodLoc != null) ? prodLoc.ProductLocalizationSafetyStockQuantity : 0;

            if (stockQty - entryQty < 0) //plus de produit en stock
            {
                res = false;
                throw new Exception(prodLoc.ProductLabel + " : " + Resources.EnoughQuantityStock + " " + (stockQty));

            }
           
            return res;
        }


        /// <summary>
        /// Cette méthode permet de faire une entrée en stock
        /// </summary>
        /// <param name="pl"></param>
        /// <param name="quantity"></param>
        /// <param name="ProductPrice"></param>
        /// <param name="serverDate"></param>
        /// <param name="UserConnect"></param>
        /// <returns></returns>
        public bool StockInput(ProductLocalization pl, double quantity, double ProductPrice, DateTime serverDate,int ? UserConnect)
        {
            bool res = false;
            int productLocalizationId = 0;
            double OldSafetyStockQuantity = 0d;
            double OldStockQuantity = 0d;
            double OldStockUnitPrice = 0d;

            if (this.IsPLExist(pl) == true)
            {
                //int isSerialNumberNull = this.context.Categories.Where(c=>c.Products.);
                
                ProductLocalization pl1 = new ProductLocalization();
                Product existprod = this.context.Products.Find(pl.ProductID);
                if (!(existprod.Category.isSerialNumberNull))
                {
                     pl1 = this.context.ProductLocalizations.FirstOrDefault(pl2 => pl2.LocalizationID == pl.LocalizationID && pl2.ProductID == pl.ProductID);
                }
                else
                {
                     pl1 = this.context.ProductLocalizations.FirstOrDefault(pl2 => pl2.LocalizationID == pl.LocalizationID && pl2.ProductID == pl.ProductID
                     && pl2.NumeroSerie == pl.NumeroSerie && pl2.Marque == pl.Marque);
                }
               
                
                OldSafetyStockQuantity = pl1.ProductLocalizationSafetyStockQuantity;
                OldStockQuantity = pl1.ProductLocalizationStockQuantity;
                OldStockUnitPrice = pl1.ProductLocalizationStockSellingPrice;

                pl.ProductLocalizationStockQuantity =  quantity + pl1.ProductLocalizationStockQuantity;
                pl.ProductLocalizationID = pl1.ProductLocalizationID;
                pl.ProductLocalizationDate = serverDate;
                // Sans Cette ligne, le code barre est supprimer apres l'entree en stock
                pl.BarCode = pl1.BarCode;
                this.Update(pl, pl.ProductLocalizationID);
                productLocalizationId = pl1.ProductLocalizationID;

               
                res = true;

            }
            else
            {
                pl.ProductLocalizationDate = serverDate;
                pl.ProductLocalizationStockQuantity =  quantity;
                pl = context.ProductLocalizations.Add(pl);
                context.SaveChanges();
                productLocalizationId = pl.ProductLocalizationID;
                OldSafetyStockQuantity = 0d;
                OldStockQuantity = 0d;
                OldStockUnitPrice = 0d;
            }

            //HISTORISATION DU STOCK
            DateTime date = serverDate;// pl.ProductLocalizationDate;
            DateTime actualDate = DateTime.Now;
            // Il faudra enlever les heures, minutes et secondes existantes; c'est eu un cas ou en ajoutant, 
            // la date du jours a incrementer en passant de 02/01/2021 a 03/01/2021
            date = date.AddHours(-serverDate.Hour);
            date = date.AddMinutes(-serverDate.Minute);
            date = date.AddSeconds(-serverDate.Second);

            date = date.AddHours(actualDate.Hour);
            date = date.AddMinutes(actualDate.Minute);
            date = date.AddSeconds(actualDate.Second);
            InventoryHistoric inventoryHistoric = new InventoryHistoric
            {
                //les nouvelles infos
                NewSafetyStockQuantity = pl.ProductLocalizationSafetyStockQuantity,
                NewStockQuantity = pl.ProductLocalizationStockQuantity,
                NEwStockUnitPrice = ProductPrice,
                //les anciennes infos
                OldSafetyStockQuantity = OldSafetyStockQuantity,
                OldStockQuantity = OldStockQuantity,
                OldStockUnitPrice = OldStockUnitPrice,
                //Autres informations
                InventoryDate = date,
                inventoryReason =  (pl.inventoryReason == null) ? "StockInput" : pl.inventoryReason,
                LocalizationID = pl.LocalizationID,
                ProductID = pl.ProductID,
                RegisteredByID = (pl.RegisteredByID == 0) ? UserConnect.Value : pl.RegisteredByID,
                AutorizedByID = (pl.AutorizedByID == 0) ? ((pl.RegisteredByID == 0) ? UserConnect.Value : pl.RegisteredByID) : pl.AutorizedByID,
                CountByID = (pl.CountByID == 0) ? ((pl.RegisteredByID == 0) ? UserConnect.Value : pl.RegisteredByID) : pl.CountByID,
                StockStatus = "INPUT",
                Description = (pl.Description != null) ? pl.Description : (pl.inventoryReason == null) ? "StockInput" : pl.inventoryReason,
                Quantity = quantity,
                NumeroSerie = pl.NumeroSerie,
                Marque = pl.Marque
            };
            context.InventoryHistorics.Add(inventoryHistoric);
            context.SaveChanges();

            //comptabilisation de l'entree en stock
            pl.Localization = this.context.Localizations.SingleOrDefault(lc => lc.LocalizationID == pl.LocalizationID);
            pl.Product = this.context.Products.SingleOrDefault(p => p.ProductID == pl.ProductID);
            int devise = this.context.Devises.SingleOrDefault(d => d.DefaultDevise).DeviseID;
            ProductLocalization acountableProdLoc = new ProductLocalization
            {
                LocalizationID = pl.LocalizationID,
                ProductID = pl.ProductID,
                ProductLocalizationDate = pl.ProductLocalizationDate.Date,
                ProductLocalizationStockQuantity = quantity,
                BranchID = pl.Localization.BranchID,
                DeviseID = devise,
                Product = pl.Product,
                AveragePurchasePrice = ProductPrice,
                ProductLocalizationID = productLocalizationId,
                isStockInput=true
            };
            ////IAccountOperation opaccount = new AccountOperationRepository(context);
            ////res = opaccount.ecritureComptableFinal(acountableProdLoc);
            ////if (!res)
            ////{
            ////    //transaction.Rollback();
            ////    throw new Exception("Une erreur s'est produite lors de comptabilisation de la sortie en stock des produits ");
            ////}

            return res;
        }
        /// <summary>
        /// Cette méthode permet de faire une sortie de stock
        /// </summary>
        /// <param name="pl"></param>
        /// <param name="quantity"></param>
        /// <param name="ProductPrice"></param>
        /// <param name="serverDate"></param>
        /// <param name="UserConnect"></param>
        /// <returns></returns>
        public bool StockOutput(ProductLocalization pl, double quantity,double ProductPrice, DateTime serverDate,int ? UserConnect)
        {
            bool res = false;
            double OldSafetyStockQuantity = 0d;
            double OldStockQuantity = 0d;
            double OldStockUnitPrice = 0d;

            if (this.IsPLExist(pl) == true)
            {
                ProductLocalization pl1 = new ProductLocalization();
                Product existprod = this.context.Products.Find(pl.ProductID);
                if (!(existprod.Category.isSerialNumberNull))
                {
                    pl1 = this.context.ProductLocalizations.FirstOrDefault(pl2 => pl2.LocalizationID == pl.LocalizationID && pl2.ProductID == pl.ProductID);
                }
                else
                {
                    pl1 = this.context.ProductLocalizations.FirstOrDefault(pl2 => pl2.LocalizationID == pl.LocalizationID && pl2.ProductID == pl.ProductID
                    && pl2.NumeroSerie == pl.NumeroSerie && pl2.Marque == pl.Marque);
                }

                //ProductLocalization pl1 = this.context.ProductLocalizations.FirstOrDefault(pl2 => pl2.LocalizationID == pl.LocalizationID && pl2.ProductID == pl.ProductID
                //&& pl2.NumeroSerie == pl.NumeroSerie && pl2.Marque == pl.Marque);
                OldSafetyStockQuantity = pl1.ProductLocalizationSafetyStockQuantity;
                OldStockQuantity = pl1.ProductLocalizationStockQuantity;
                OldStockUnitPrice = pl1.ProductLocalizationStockSellingPrice;

                pl.ProductLocalizationStockQuantity = pl1.ProductLocalizationStockQuantity - quantity;
                pl.ProductLocalizationID = pl1.ProductLocalizationID;
                pl.ProductLocalizationDate = serverDate;
                pl.BarCode = pl1.BarCode;
                this.Update(pl, pl.ProductLocalizationID);


                //HISTORISATION DU STOCK
                DateTime date = serverDate; // pl.ProductLocalizationDate;
                DateTime actualDate = DateTime.Now;
                date = date.AddHours(actualDate.Hour);
                date = date.AddMinutes(actualDate.Minute);
                date = date.AddSeconds(actualDate.Second);
                string inventoryReason = (pl.inventoryReason == null) ? "StockOutput" : pl.inventoryReason;
                // Si la sortie en stock fait suite a une vente
                inventoryReason = pl.SellingReference != null ? "Sale(" + pl.SellingReference + ")" :
                                  inventoryReason;
                InventoryHistoric inventoryHistoric = new InventoryHistoric
                {
                    //les nouvelles infos
                    NewSafetyStockQuantity = pl.ProductLocalizationSafetyStockQuantity,
                    NewStockQuantity = pl.ProductLocalizationStockQuantity,
                    NEwStockUnitPrice = ProductPrice,
                    //les anciennes infos
                    OldSafetyStockQuantity = OldSafetyStockQuantity,
                    OldStockQuantity = OldStockQuantity,
                    OldStockUnitPrice = OldStockUnitPrice,
                    //Autres informations
                    InventoryDate = date,
                    inventoryReason = inventoryReason,
                    LocalizationID = pl.LocalizationID,
                    ProductID = pl.ProductID,
                    RegisteredByID = (pl.RegisteredByID == 0) ? UserConnect.Value : pl.RegisteredByID,
                    AutorizedByID = (pl.AutorizedByID == 0 ) ? ((pl.RegisteredByID==0)? UserConnect.Value : pl.RegisteredByID) : pl.AutorizedByID,
                    CountByID = (pl.CountByID == 0 ) ? ((pl.RegisteredByID == 0) ? UserConnect.Value : pl.RegisteredByID) : pl.CountByID,
                    StockStatus = "OUTPUT",
                    Description = (pl.Description != null) ? pl.Description : (pl.inventoryReason == null) ? "StockOutput" : pl.inventoryReason,
                    Quantity = quantity,
                    NumeroSerie = pl.NumeroSerie,
                    Marque = pl.Marque
                };
                context.InventoryHistorics.Add(inventoryHistoric);
                context.SaveChanges();

                pl.Localization = this.context.Localizations.SingleOrDefault(lc => lc.LocalizationID == pl.LocalizationID);
                pl.Product = this.context.Products.SingleOrDefault(p => p.ProductID == pl.ProductID);
                int devise = this.context.Devises.SingleOrDefault(d => d.DefaultDevise).DeviseID;
                //comptabilisation de la sortie en stock
                ProductLocalization acountableProdLoc = new ProductLocalization
                {
                    LocalizationID = pl.LocalizationID,
                    ProductID = pl.ProductID,
                    ProductLocalizationDate = pl.ProductLocalizationDate.Date,
                    ProductLocalizationStockQuantity = quantity,
                    BranchID = pl.Localization.BranchID,
                    DeviseID = devise,
                    Product = pl.Product,
                    AveragePurchasePrice = ProductPrice,
                    ProductLocalizationID = pl.ProductLocalizationID,
                    isStockInput=false,
                    SellingReference=pl.SellingReference
                };
                ////IAccountOperation opaccount = new AccountOperationRepository(context);
                ////res = opaccount.ecritureComptableFinal(acountableProdLoc);
                ////if (!res)
                ////{
                ////    //transaction.Rollback();
                ////    throw new Exception("Une erreur s'est produite lors de comptabilisation de l'entree en stock des produits ");
                ////}

                res = true;
                //return res;
            }
            return res;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pl"></param>
        /// <param name="serverDate"></param>
        /// <param name="newQty"></param>
        /// <returns></returns>
        public bool UpdateProductLocalizationAddedQty(ProductLocalization pl, DateTime serverDate, double newQty)
        {
            bool res = false;
            try
            {

                using (TransactionScope ts = new TransactionScope())
                {
                    //AsNoTracking permet de "détacher l'entity du context" on peut aussi utiliser Context.Entry(oldProdLoc).State = EntityState.Detached après la requete
                    ProductLocalization oldProdLoc = (from prodLoc in context.ProductLocalizations
                                                      where prodLoc.ProductLocalizationID == pl.ProductLocalizationID
                                                      select prodLoc).SingleOrDefault();
                    double OldQty = (oldProdLoc == null) ? 0 : oldProdLoc.ProductLocalizationStockQuantity;
                    //mise a jour de l'ancien stock a zero
                    //oldProdLoc.ProductLocalizationStockQuantity = OldQty;
                    //context.SaveChanges();

                    double finalqty = newQty; //+ OldQty;// oldProdLoc.ProductLocalizationStockQuantity;

                    //update du stock
                    if (finalqty < 0)
                    {
                        //on dimuni le stock 
                        this.StockOutput(pl, -1 * finalqty, pl.AveragePurchasePrice, serverDate,null);
                    }
                    if (finalqty > 0)
                    {
                        //on approvisionne le stock 
                        this.StockInput(pl, finalqty, pl.AveragePurchasePrice, serverDate,null);
                    }

                    //we apply this modifications
                    //transaction.Commit();
                    ts.Complete();
                }
            }
            catch (Exception e)
            {
                //If an errors occurs, we cancel all changes in database
                //transaction.Rollback();
                res = false;
                throw new Exception("Une erreur s'est produite lors de la mise a jour d'un produit : " + " Message = " + e.Message + " StackTrace = " + e.StackTrace);
            }
            return res;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="CategoryID"></param>
        /// <param name="Serverdate"></param>
        /// <param name="UserConect"></param>
        /// <param name="Stores"></param>
        /// <param name="BranchID"></param>
        /// <returns></returns>
        public bool InitialiseStock(int CategoryID, int Stores, DateTime Serverdate, int UserConect, int BranchID)
        {
            bool res = false;
            double plProductLocalizationID = 0d;
            try
            {

                //using (TransactionScope ts = new TransactionScope())
                //{
                List<ProductLocalization> dataTmp = (from ls in context.ProductLocalizations
                                                     where ls.Product.CategoryID == CategoryID && ls.LocalizationID == Stores
                                                     select ls)
                       .ToList();



                if (dataTmp.Count > 0)
                {
                    foreach (ProductLocalization pl in dataTmp)
                    {
                        plProductLocalizationID = pl.ProductLocalizationID;
                        if (pl.ProductLocalizationStockQuantity != 0)
                        {
                            if (pl.Product is OrderLens)
                            {

                            }
                            else
                            {
                                ProductLocalization oldProdLoc = context.ProductLocalizations.Find(pl.ProductLocalizationID);
                                if (oldProdLoc != null)
                                {
                                    /*On veut que l'initialisation du stock puisse apparaitre dans Stock Mouvement*/
                                    // oldProdLoc.ProductLocalizationStockQuantity = 0d;
                                    double stockQuantity = oldProdLoc.ProductLocalizationStockQuantity;
                                    oldProdLoc.inventoryReason = "STOCK INITIALISATION";
                                    //update du stock
                                    if (stockQuantity < 0)
                                    {
                                        //on approvisionne le stock afin de le ramener a 0 
                                        this.StockInput(oldProdLoc, -1 * stockQuantity, oldProdLoc.AveragePurchasePrice, Serverdate, UserConect);
                                    }

                                    if (stockQuantity > 0)
                                    {
                                        //on diminu le stock afin de le ramener a 0 
                                        this.StockOutput(oldProdLoc, stockQuantity, oldProdLoc.AveragePurchasePrice, Serverdate, UserConect);
                                    }

                                }
                            }
                            context.Database.CommandTimeout = 0;
                            context.SaveChanges();
                        }



                    }

                    //recuperation de la list des cummul sale and bill pour cette category
                    List<CumulSaleAndBill> lstcumsalbill = (from ls in context.CumulSaleAndBills
                                                            where ls.CumulSaleAndBillLines.FirstOrDefault().Product.CategoryID == CategoryID 
                                                            select ls).ToList();

                    if (lstcumsalbill.Count > 0)
                    {
                        foreach (CumulSaleAndBill pl in lstcumsalbill)
                        {
                            if (pl.CumulSaleAndBillID != 0)
                            {

                                CumulSaleAndBill exitcumsalebill = context.CumulSaleAndBills.Find(pl.CumulSaleAndBillID);
                                if (exitcumsalebill != null)
                                {
                                    exitcumsalebill.IsProductDeliver = true;
                                }

                                context.Database.CommandTimeout = 0;
                                context.SaveChanges();
                            }

                        }

                    }
                    //EcritureSneack
                    IMouchar opSneak = new MoucharRepository(context);
                    bool res1 = opSneak.InsertOperation(UserConect, "SUCCESS", " INITIALISATION STOCK CATEGORYID " + CategoryID + " FOR STORE " + Stores, "InitialiseStock", Serverdate, BranchID);
                    if (!res1)
                    {
                        throw new Exception("Une erreur s'est produite lors de la journalisation ");
                    }
                }

                //    //we apply this modifications
                //    ts.Complete();
                //}
            }
            catch (Exception e)
            {
                //If an errors occurs, we cancel all changes in database
                //transaction.Rollback();
                res = false;
                throw new Exception("Error : " + " Message = " + plProductLocalizationID + ":" + e.Message + " StackTrace = " + e.StackTrace);
            }
            return res;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pl"></param>
        /// <param name="serverDate"></param>
        /// <param name="newQty"></param>
        /// <returns></returns>
        public bool UpdateProductLocalization(ProductLocalization pl, DateTime serverDate, double newQty)
        {
            bool res = false;
            try
            {

                using (TransactionScope ts = new TransactionScope())
                {
                    //AsNoTracking permet de "détacher l'entity du context" on peut aussi utiliser Context.Entry(oldProdLoc).State = EntityState.Detached après la requete
                    ProductLocalization oldProdLoc = (from prodLoc in context.ProductLocalizations
                                                      where prodLoc.ProductLocalizationID == pl.ProductLocalizationID
                                                      select prodLoc).SingleOrDefault();
                    double OldQty = (oldProdLoc == null) ? 0 : oldProdLoc.ProductLocalizationStockQuantity;
                    //mise a jour de l'ancien stock a zero
                    //oldProdLoc.ProductLocalizationStockQuantity = OldQty;
                    //context.SaveChanges();

                    double stockDiff = newQty - OldQty;// oldProdLoc.ProductLocalizationStockQuantity;

                    //update du stock
                    if (stockDiff < 0)
                    {
                        //on dimuni le stock 
                        this.StockOutput(pl, -1 * stockDiff, pl.AveragePurchasePrice, serverDate,null);
                    }
                    if (stockDiff > 0)
                    {
                        //on approvisionne le stock 
                        this.StockInput(pl, stockDiff, pl.AveragePurchasePrice, serverDate, null);
                    }

                    //we apply this modifications
                    //transaction.Commit();
                    ts.Complete();
                }
            }
            catch (Exception e)
            {
                //If an errors occurs, we cancel all changes in database
                //transaction.Rollback();
                res = false;
                throw new Exception("Une erreur s'est produite lors de la mise a jour d'un produit : " + " Message = " + e.Message + " StackTrace = " + e.StackTrace);
            }
            return res;
        }
        /// <summary>
        /// Cette méthode retourne la liste des id des magasins dans lesquels se trouve un produit
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        public int[] GetAllStore(Product product)
        {
            int[] ids;
            ids = this.FindAll.Where(pl => pl.ProductID == product.ProductID).Select(pt => pt.LocalizationID).ToArray();

            return ids;
        }

        public void DeleteAllStore(int productID)
        {
            List<ProductLocalization> allPL = context.ProductLocalizations/*.AsNoTracking()*/.Where(pl => pl.ProductID == productID).ToList();

            context.ProductLocalizations.RemoveRange(allPL);

            context.SaveChanges();
        }

        public void CreateStore(Product product, int[] ids)
        {

            if (ids != null && ids.Count() > 0)
            {
                foreach (int id in ids)
                {
                    ProductLocalization pl = context.ProductLocalizations.AsNoTracking().SingleOrDefault(pdl => pdl.LocalizationID == id &&
                                                                                                               pdl.ProductID == product.ProductID);

                    if (pl == null || pl.ProductLocalizationID == 0)
                    {
                        pl = new ProductLocalization()
                        {
                            LocalizationID = id,
                            ProductID = product.ProductID,
                            ProductLocalizationStockQuantity = 0,
                            ProductLocalizationSafetyStockQuantity = 0,
                            ProductLocalizationStockSellingPrice = 0,
                            AveragePurchasePrice = 0,
                            ProductLocalizationDate = context.BusinessDays.FirstOrDefault(bd => (bd.BDStatut == true) && (bd.ClosingDayStarted == false)).BDDateOperation,
                        };

                        context.ProductLocalizations.Add(pl);
                        context.SaveChanges();

                    }
                }
            }

            return ;
        }

        public void DeleteAllStore(int productID, int[] ids)
        {
            List<ProductLocalization> allPL = new List<ProductLocalization>();

            if (ids != null && ids.Count() > 0)
            {
                foreach (int id in ids)
                {
                    allPL.Add(context.ProductLocalizations/*.AsNoTracking()*/.SingleOrDefault(pl => pl.ProductID == productID && pl.LocalizationID == id));
                }
                
            }

            context.ProductLocalizations.RemoveRange(allPL);
            context.SaveChanges();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentSale"></param>
        /// <param name="HeureOperation"></param>
        /// <param name="UserConnect"></param>
        /// <param name="RELineID">c'est le CumulSaleAndBillLineId du cote droit</param>
        /// <param name="LELineID">c'est le CumulSaleAndBillLineId du cote gauche</param>
        /// <returns></returns>
        public bool ValideStockOutPut(CumulSaleAndBill currentSale, String HeureOperation, int UserConnect, int RELineID, int LELineID) {
            //Begin of transaction
            bool res = false;
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    //ajout de lheure de la vente
                    string[] tisys = HeureOperation.Split(new char[] { ':' });
                    DateTime date = currentSale.DeliverDate.Value;
                    date = date.AddHours(Convert.ToDouble(tisys[0]));
                    date = date.AddMinutes(Convert.ToDouble(tisys[1]));
                    date = date.AddSeconds(Convert.ToDouble(tisys[2]));
                    //we create a new command
                    currentSale.DateOperationHours = date;

                    res = PersistStockOutPut(currentSale, UserConnect, RELineID, LELineID);

                    #region Stock out put after a Lens Mounting Damage; this is necessary to allow mounting or damage again
                    var productDamage = this.context.ProductDamages.SingleOrDefault(pd => pd.CumulSaleAndBillID == currentSale.CumulSaleAndBillID &&
                                                                                                      pd.IsLensMountingDamage && !pd.IsStockOutPut);
                    if (productDamage != null)
                    {
                        productDamage.IsStockOutPut = true;
                        this.context.SaveChanges();
                    }
                    #endregion

                    ts.Complete();
                }
            }
            catch (Exception e)
            {
                res = false;
                IMouchar opSneak = new MoucharRepository(context);
                bool ress = opSneak.InsertOperation(UserConnect, "ERROR", "REFERENCE " + currentSale.SaleReceiptNumber + " FOR CUSTOMER " + currentSale.CustomerName, "ValideStockOutPut", currentSale.DeliverDate.Value, currentSale.BranchID);
                if (!ress)
                {
                    throw new Exception("Une erreur s'est produite lors de la journalisation ");
                }
                throw new Exception("Une erreur s'est produite lors de la vente : " + "e.Message = " + e.Message);
            }
            return res;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentSale"></param>
        /// <param name="HeureOperation"></param>
        /// <param name="UserConnect"></param>
        /// <returns></returns>
        public bool ValideDeliverDesk(CumulSaleAndBill currentSale, String HeureOperation, int UserConnect)
        {
            //Begin of transaction
            bool res = false;
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    //ajout de lheure de la vente
                    string[] tisys = HeureOperation.Split(new char[] { ':' });
                    DateTime date = currentSale.ProductDeliverDate.Value;
                    date = date.AddHours(Convert.ToDouble(tisys[0]));
                    date = date.AddMinutes(Convert.ToDouble(tisys[1]));
                    date = date.AddSeconds(Convert.ToDouble(tisys[2]));
                    //we create a new command
                    currentSale.ProductDeliverDateHeure = date;

                    res = PersistDeliverDesk(currentSale, UserConnect);
                    ts.Complete();
                }
            }
            catch (Exception e)
            {
                res = false;
                IMouchar opSneak = new MoucharRepository(context);
                bool ress = opSneak.InsertOperation(UserConnect, "ERROR", "REFERENCE " + currentSale.SaleReceiptNumber + " FOR CUSTOMER " + currentSale.CustomerName, "ValideDeliverDesk", currentSale.ProductDeliverDate.Value, currentSale.BranchID);
                if (!ress)
                {
                    throw new Exception("Une erreur s'est produite lors de la journalisation ");
                }
                throw new Exception("Une erreur s'est produite lors de la vente : " + "e.Message = " + e.Message);
            }
            return res;
        }

        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentSale"></param>
        /// <param name="UserConect"></param>
        /// <returns></returns>
        public bool PersistDeliverDesk(CumulSaleAndBill currentSale, int UserConect)
        {

            bool res = false;
            ISale _saleRep = new SaleRepository();
            
            // Etant donne qu'on ne fait plus la sortie en stock ici, on ne doit plus controller le stock ici
            
            CumulSaleAndBill cumulSaleAndBill = context.CumulSaleAndBills.Where(auth => auth.CumulSaleAndBillID == currentSale.CumulSaleAndBillID).SingleOrDefault();
            if (cumulSaleAndBill == null)
            {
                throw new Exception("Error while updated command sale. please call your administrator ");
            }
            cumulSaleAndBill.IsProductDeliver = true;
            cumulSaleAndBill.DeliverProductByID = UserConect;
            cumulSaleAndBill.ProductDeliverDateHeure = currentSale.ProductDeliverDateHeure;
            cumulSaleAndBill.ProductDeliverDate = currentSale.ProductDeliverDate.Value;
            cumulSaleAndBill.ControlBy = currentSale.ControlBy;
            cumulSaleAndBill.LensMountingComment = currentSale.LensMountingComment;
            cumulSaleAndBill.spray = currentSale.spray;
            cumulSaleAndBill.cases = currentSale.cases;

            ManageSaleAccessoryStockOutput(currentSale, UserConect);

            // cumulSaleLine.MountingBy = currentSale.MountingBy;
            // cumulSaleLine.ControlBy = currentSale.ControlBy;
            context.SaveChanges();

            //EcritureSneack
            IMouchar opSneak = new MoucharRepository(context);
            res = opSneak.InsertOperation(UserConect, "SUCCESS", "SALE-REFERENCE " + currentSale.SaleReceiptNumber + " FOR CUSTOMER " + currentSale.CustomerName, "PersistDeliverDesk", currentSale.ProductDeliverDate.Value, currentSale.BranchID);
            if (!res)
            {
                throw new Exception("Une erreur s'est produite lors de la journalisation ");
            }

            res = true;
            return res;
        }


        public void ManageSaleAccessoryStockOutput(CumulSaleAndBill currentSale, int UserConect)
        {

            int ProductId = 0;
            int LocalizationID = 0;

            Localization loc = context.Localizations.Where(c => c.BranchID == currentSale.BranchID).FirstOrDefault();
            LocalizationID = loc.LocalizationID;

            if (currentSale.cases > 0)
            {

                var lstProduct = context.Products.Join(context.Categories, p => p.CategoryID, c => c.CategoryID,
                                (p, c) => new { p, c })
                                .Where(pc => pc.c.CategoryID == 2 && pc.p.ProductCode.Contains("CASE"))
                                .Select(rdv => new
                                {
                                    ProductID = rdv.p.ProductID
                                }).FirstOrDefault();

                ProductId = lstProduct.ProductID;

                ProductLocalization productInStock = context.ProductLocalizations.FirstOrDefault(pl => pl.ProductID == ProductId && pl.LocalizationID == LocalizationID);

                productInStock.SellingReference = currentSale.SaleReceiptNumber;
                productInStock.inventoryReason = "SALE ACCESSORY";

                this.StockOutput(productInStock, 1d, productInStock.AveragePurchasePrice, currentSale.ProductDeliverDate.Value, UserConect);

            }

            if (currentSale.spray > 0)
            {

                var lstProduct = context.Products.Join(context.Categories, p => p.CategoryID, c => c.CategoryID,
                            (p, c) => new { p, c })
                            .Where(pc => pc.c.CategoryID == 2 && pc.p.ProductCode.Contains("SPRAY"))
                            .Select(rdv => new
                            {
                                ProductID = rdv.p.ProductID
                            }).FirstOrDefault();

                ProductId = lstProduct.ProductID;
                ProductLocalization productInStock = context.ProductLocalizations.FirstOrDefault(pl => pl.ProductID == ProductId && pl.LocalizationID == LocalizationID);

                productInStock.SellingReference = currentSale.SaleReceiptNumber;
                productInStock.inventoryReason = "SALE ACCESSORY";

                this.StockOutput(productInStock, 1d, productInStock.AveragePurchasePrice, currentSale.ProductDeliverDate.Value, UserConect);

            }
        }

        #region Reception verre de commande
        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentSale"></param>
        /// <param name="HeureOperation"></param>
        /// <param name="UserConnect"></param>
        /// <returns></returns>
        public bool ReceiveOrder(CumulSaleAndBill currentSale, String HeureOperation, int UserConnect)
        {
            //Begin of transaction
            bool res = false;
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    //ajout de lheure de la vente
                    string[] tisys = HeureOperation.Split(new char[] { ':' });
                    DateTime date = currentSale.ProductDeliverDate.Value;
                    date = date.AddHours(Convert.ToDouble(tisys[0]));
                    date = date.AddMinutes(Convert.ToDouble(tisys[1]));
                    date = date.AddSeconds(Convert.ToDouble(tisys[2]));
                    //we create a new command
                    currentSale.SpecialOrderReceptionDateHeure = date;

                    res = PersistSpecialOrderReception(currentSale, UserConnect);
                    ts.Complete();
                }
            }
            catch (Exception e)
            {
                res = false;
                IMouchar opSneak = new MoucharRepository(context);
                bool ress = opSneak.InsertOperation(UserConnect, "ERROR", "REFERENCE " + currentSale.SaleReceiptNumber + " FOR CUSTOMER " + currentSale.CustomerName, "ValidateSpecialOrderReception", currentSale.ProductDeliverDate.Value, currentSale.BranchID);
                if (!ress)
                {
                    throw new Exception("Une erreur s'est produite lors de la journalisation ");
                }
                throw new Exception("Une erreur s'est produite lors de la reception du verre de commande : " + "e.Message = " + e.Message);
            }
            return res;
        }
        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentSale"></param>
        /// <param name="UserConect"></param>
        /// <returns></returns>
        public bool PersistSpecialOrderReception(CumulSaleAndBill currentSale, int UserConect)
        {

            CumulSaleAndBill existingCumulSaleAndBill = context.CumulSaleAndBills.Find(currentSale.CumulSaleAndBillID);
            existingCumulSaleAndBill.IsReceived = true;
            existingCumulSaleAndBill.ReceiverID = UserConect;
            existingCumulSaleAndBill.SpecialOrderReceptionDateHeure = currentSale.SpecialOrderReceptionDateHeure;
            context.SaveChanges();
            // 1- Creation du Stock (ProductLocalisation)  s'il n'existe pas encore en base de donnees

            // 2- Mise a jour du nombre de produit en stock

            // 1 & 2
            iventoryDirectoryRepository = new InventoryDirectoryRepository(context);
            iventoryDirectoryRepository.SpecialOrderReceptionStockInPut(existingCumulSaleAndBill, UserConect);

            return true;
        }
        #endregion

        #region Lenses Order
        public bool OrderLenses(CumulSaleAndBill currentSale, int UserConnect, StockType stockType)
        {
            bool res = false;
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    CumulSaleAndBill existingCSB = context.CumulSaleAndBills.Find(currentSale.CumulSaleAndBillID);
                    if (existingCSB == null)
                    {
                        throw new Exception("Please Select a Sale");
                    }
                    
                    existingCSB.OrderById = UserConnect;
                    existingCSB.IsOrdered = true;
                    existingCSB.OrderDate = currentSale.OrderDate;
                    existingCSB.OrderControllerId = currentSale.OrderControllerId;

                    if (stockType == StockType.STOCK_ORDER)
                    {
                        existingCSB.CumulSaleAndBillLines.ToList().ForEach(csbl => csbl.StockType = stockType);
                    }

                    #region ReOrder And Order What was not mark as command glass in the show room; to implement later
                    existingCSB.UpdateReOrderDates(currentSale.OrderDate.Value);
                    existingCSB.UpdateReOrderReasons(currentSale.ReOrderReason);
                    existingCSB.NumberOfTimesOrdered++;
                    if (currentSale.IsREOrdered)
                    {
                        CumulSaleAndBillLine csblRE = context.CumulSaleAndBillLines.Find(currentSale.RELineId);
                        csblRE.IsOrdered = true;

                        if (!currentSale.IsLEOrdered)
                        {
                            CumulSaleAndBillLine csblLE = context.CumulSaleAndBillLines.Find(currentSale.LELineId);
                            csblLE.IsOrdered = false;
                        }
                    }

                    if (currentSale.IsLEOrdered)
                    {
                        CumulSaleAndBillLine csblLE = context.CumulSaleAndBillLines.Find(currentSale.LELineId);
                        csblLE.IsOrdered = true;

                        if (!currentSale.IsREOrdered)
                        {
                            CumulSaleAndBillLine csblRE = context.CumulSaleAndBillLines.Find(currentSale.RELineId);
                            csblRE.IsOrdered = false;
                        }

                    }

                    // 
                    if (currentSale.IsRECommandGlass)
                    {
                        CumulSaleAndBillLine csbl = context.CumulSaleAndBillLines.Find(currentSale.RELineId);
                        csbl.isCommandGlass = true;
                    }

                    if (currentSale.IsLECommandGlass)
                    {
                        CumulSaleAndBillLine csbl = context.CumulSaleAndBillLines.Find(currentSale.LELineId);
                        csbl.isCommandGlass = true;
                    }
                    #endregion
                    context.SaveChanges();
                    res = true;
                    ts.Complete();
                }
            }
            catch (Exception e)
            {
                res = false;
                IMouchar opSneak = new MoucharRepository(context);
                bool ress = opSneak.InsertOperation(UserConnect, "ERROR", "REFERENCE " + currentSale.SaleReceiptNumber + " FOR CUSTOMER " + currentSale.CustomerName, "ValidateSpecialOrderReception", currentSale.ProductDeliverDate.Value, currentSale.BranchID);
                if (!ress)
                {
                    throw new Exception("Une erreur s'est produite lors de la journalisation ");
                }
                throw new Exception("Une erreur s'est produite lors de la reception du verre de commande : " + "e.Message = " + e.Message);
            }
            return res;
        }

        public void UpdateStockFields(SpecialLensModel slm, CumulSaleAndBill currentSale, bool IsStockOutPutITF)
        {
            CumulSaleAndBill csb = context.CumulSaleAndBills.Find(currentSale.CumulSaleAndBillID);
            CumulSaleAndBillLine rightLine = context.CumulSaleAndBillLines.Find(slm.RELineID);
            if (rightLine != null)
            {
                rightLine.Supplier = slm.RESupplier;
                rightLine.Manufacturer = slm.REManufacturer;
                // l'idee ici c'est de conserver le fait qu'un verre est passer par la commande
                if (rightLine.StockType == StockType.NONE ||
                    (slm.REStockType != StockType.STOCK))
                {
                    rightLine.StockType = slm.REStockType;
                }
                rightLine.isCommandGlass = slm.REStockType == StockType.RX_ORDER;
                if (rightLine.isCommandGlass)
                {
                    csb.IsRendezVous = true;
                }
            }

            CumulSaleAndBillLine leftLine = context.CumulSaleAndBillLines.Find(slm.LELineID);
            if (leftLine != null)
            {
                leftLine.Supplier = slm.LESupplier;
                leftLine.Manufacturer = slm.LEManufacturer;
                // l'idee ici c'est de conserver le fait qu'un verre est passer par la commande
                if (leftLine.StockType == StockType.NONE ||
                    (slm.LEStockType != StockType.STOCK))
                {
                    leftLine.StockType = slm.LEStockType;
                }
                leftLine.isCommandGlass = slm.LEStockType == StockType.RX_ORDER;
                if (leftLine.isCommandGlass)
                {
                    csb.IsRendezVous = true;
                }
            }

            if (IsStockOutPutITF == true && slm.REStockType == StockType.STOCK && slm.LEStockType == StockType.STOCK)
            {
                csb.IsStockOutPut = true;
            }

            context.SaveChanges();
        }

        #endregion

        #region Montage du verre
        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentSale"></param>
        /// <param name="HeureOperation"></param>
        /// <param name="UserConnect"></param>
        /// <returns></returns>
        public bool ValidateLensMounting(CumulSaleAndBill currentSale, String HeureOperation, int UserConnect)
        {
            //Begin of transaction
            bool res = false;
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    //ajout de lheure de la vente
                    string[] tisys = HeureOperation.Split(new char[] { ':' });
                    DateTime date = currentSale.ProductDeliverDate.Value;
                    date = date.AddHours(Convert.ToDouble(tisys[0]));
                    date = date.AddMinutes(Convert.ToDouble(tisys[1]));
                    date = date.AddSeconds(Convert.ToDouble(tisys[2]));
                    //we create a new command
                    currentSale.LensMountingDateHeure = date;

                    res = PersistLensMounting(currentSale, HeureOperation, UserConnect);
                    ts.Complete();
                }
            }
            catch (Exception e)
            {
                res = false;
                IMouchar opSneak = new MoucharRepository(context);
                bool ress = opSneak.InsertOperation(UserConnect, "ERROR", "REFERENCE " + currentSale.SaleReceiptNumber + " FOR CUSTOMER " + currentSale.CustomerName, "ValidateSpecialOrderReception", currentSale.ProductDeliverDate.Value, currentSale.BranchID);
                if (!ress)
                {
                    throw new Exception("Une erreur s'est produite lors de la journalisation ");
                }
                throw new Exception("Une erreur s'est produite lors du montage du verre : " + "e.Message = " + e.Message);
            }
            return res;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentSale"></param>
        /// <param name="UserConnect"></param>
        /// <param name="HeureOperation"></param>
        /// <returns></returns>
        public bool PersistLensMounting(CumulSaleAndBill currentSale, String HeureOperation, int UserConnect)
        {

            //                     bool res =  _prodLocalRepo.ValideStockOutPut(currentSale, heureOperation, SessionGlobalPersonID);


            CumulSaleAndBill existingCumulSaleAndBill = context.CumulSaleAndBills.Find(currentSale.CumulSaleAndBillID);
            existingCumulSaleAndBill.IsMounted = true;
            existingCumulSaleAndBill.MountingBy = currentSale.MountingBy;
            existingCumulSaleAndBill.ControlBy = currentSale.ControlBy;
            existingCumulSaleAndBill.LensMountingDateHeure = currentSale.LensMountingDateHeure;
            context.SaveChanges();

            currentSale.DateOperationHours = currentSale.LensMountingDateHeure.Value;
            // currentSale.StockOutPutOperationHours = .LensMountingDateHeure.Value;
            // Puissequ'on ne faire plus la sortie de stock, je commente ces lignes
            // bool res = PersistLensMountingStockOutPut(currentSale, UserConnect);

            return true;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentSale"></param>
        /// <param name="UserConect"></param>
        /// <returns></returns>
        public bool PersistLensMountingStockOutPut(CumulSaleAndBill currentSale, int UserConect)
        {

            bool res = false;
            ISale _saleRep = new SaleRepository();
            int ProductId = 0;
            //bool isFrame = false;
            //vente sans commande ou vente directe
            foreach (CumulSaleAndBillLine saleLine in currentSale.CumulSaleAndBillLines)
            {

                ProductId = saleLine.ProductID;

                //desormais c'est le comptable qui effectue tous les sorties du produits
                //si c'est un verre de commande on s'assure ki a deja été recepcionné
                //pas encore car le boss n'a pas valider cette etape
                //pour linstant on va creer une inteerface d'achat des produits pour gerer ce cas
                //if (saleLine.Product is OrderLens)
                //{
                //    List<CumulSaleAndBill> lstSale = context.CumulSaleAndBills.Where(c => c.IsProductReveive && !c.isReturn && c.CumulSaleAndBillID == currentSale.CumulSaleAndBillID).ToList();
                //    if (lstSale.Count==0)
                //    {
                //        throw new Exception("The product "+saleLine.Product.ProductCode + " not yet receive in the systeme. please receive it before proceed");
                //    }
                //    OrderLens ProductLens = _saleRep.PersistCustomerOrderLine(saleLine);
                //    ProductId = ProductLens.ProductID;

                //}
                //seul les verres seront enlever du stock
                //if (saleLine.Product is Lens)
                if (!saleLine.isCommandGlass && !(saleLine.Product.Category.isSerialNumberNull))
                {
                    ProductLocalization productInStock = new ProductLocalization();
                    if (!(saleLine.Product.Category.isSerialNumberNull))
                    {
                        //recuperation du verre du stock
                        productInStock = context.ProductLocalizations.FirstOrDefault(pl => pl.ProductID == ProductId && pl.LocalizationID == saleLine.LocalizationID);
                    }
                    else
                    {
                        //recuperation du frame
                        productInStock = context.ProductLocalizations.FirstOrDefault(pl => pl.ProductID == ProductId && pl.LocalizationID == saleLine.LocalizationID && pl.NumeroSerie == saleLine.NumeroSerie);
                    }

                    if (productInStock == null)
                    {
                        throw new Exception("Error Stock Output : product " + saleLine.Product.ProductCode + " not yet put on the stock; Please contact your accountant");
                    }
                    else
                    {
                        //sortie en stock
                        productInStock.SellingReference = currentSale.SaleReceiptNumber;
                        if (productInStock.ProductLocalizationStockQuantity < saleLine.LineQuantity)
                        {
                            throw new Exception("Error Stock Output : Insufficient stock for product " + saleLine.Product.ProductCode + " ; Please contact your accountant");
                        }
                        this.StockOutput(productInStock, saleLine.LineQuantity, productInStock.AveragePurchasePrice, currentSale.LensMountingDateHeure.Value, UserConect);
                    }

                    ProductId = 0;

                }

            }


           // _saleRep.ValideDeliverSpecialOrder(currentSale, UserConect);

            //CumulSaleAndBill cumulSaleLine = context.CumulSaleAndBills.Where(auth => auth.CumulSaleAndBillID == currentSale.CumulSaleAndBillID).SingleOrDefault();
            //if (cumulSaleLine == null)
            //{
            //    throw new Exception("Error while updated command sale. please call your administrator ");
            //}
            //cumulSaleLine.IsDeliver = true;
            //cumulSaleLine.DeliverByID = UserConect;
            //cumulSaleLine.DateOperationHours = currentSale.DateOperationHours;
            //cumulSaleLine.DeliverDate = currentSale.DeliverDate.Value;
            //context.SaveChanges();

            //EcritureSneack
            IMouchar opSneak = new MoucharRepository(context);
            res = opSneak.InsertOperation(UserConect, "SUCCESS", "SALE-REFERENCE " + currentSale.SaleReceiptNumber + " FOR CUSTOMER " + currentSale.CustomerName, "PersistStockOutPut", currentSale.LensMountingDateHeure.Value, currentSale.BranchID);
            if (!res)
            {
                throw new Exception("Une erreur s'est produite lors de la journalisation ");
            }

            res = true;
            return res;
        }
        #endregion
        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentSale"></param>
        /// <param name="UserConect"></param>
        /// <returns></returns>
        public bool PersistStockOutPut(CumulSaleAndBill currentSale, int UserConect, int RELineID, int LELineID)
        {

            bool res = false;
            ISale _saleRep = new SaleRepository();
            int ProductId = 0;
            //bool isFrame = false;
            //vente sans commande ou vente directe
            foreach (CumulSaleAndBillLine saleLine in currentSale.CumulSaleAndBillLines)
            {
               
                ProductId = saleLine.ProductID;
                if (saleLine.Product is OrderLens)
                {
                    Lens l = LensConstruction.GetLensFromOrderLens(saleLine.Product.ProductID, context, saleLine);
                    ProductId = l.ProductID;
                }
                if (ProductId == 0 && (saleLine.Product is OrderLens))
                {
                    CumulSaleAndBillLine originalCSBL = null;
                    if (saleLine.EyeSide == EyeSide.OD)
                    {
                        originalCSBL = context.CumulSaleAndBillLines.Find(RELineID);
                    }

                    if (saleLine.EyeSide == EyeSide.OG)
                    {
                        originalCSBL = context.CumulSaleAndBillLines.Find(LELineID);
                    }

                    if (originalCSBL != null)
                    {
                        ProductId = originalCSBL.ProductID;
                    }
                }

                //desormais c'est le comptable qui effectue tous les sorties du produits
                //si c'est un verre de commande on s'assure ki a deja été recepcionné
                //pas encore car le boss n'a pas valider cette etape
                //pour linstant on va creer une inteerface d'achat des produits pour gerer ce cas
                //if (saleLine.Product is OrderLens)
                //{
                //    List<CumulSaleAndBill> lstSale = context.CumulSaleAndBills.Where(c => c.IsProductReveive && !c.isReturn && c.CumulSaleAndBillID == currentSale.CumulSaleAndBillID).ToList();
                //    if (lstSale.Count==0)
                //    {
                //        throw new Exception("The product "+saleLine.Product.ProductCode + " not yet receive in the systeme. please receive it before proceed");
                //    }
                //    OrderLens ProductLens = _saleRep.PersistCustomerOrderLine(saleLine);
                //    ProductId = ProductLens.ProductID;

                //}
                //seul les verres seront enlever du stock
                //if (saleLine.Product is Lens)
                // Puisse qu'a la reception des verres de commande, le comptable doit fait l'entree en stock
                // nous activons ici la reduction du stock quand c'est un verre de commande
                if (/*!saleLine.isCommandGlass &&*/ !(saleLine.Product.Category.isSerialNumberNull))
                {
                    ProductLocalization productInStock = new ProductLocalization();
                    if (!(saleLine.Product.Category.isSerialNumberNull))
                    {
                        //recuperation du verre du stock
                        productInStock = context.ProductLocalizations.FirstOrDefault(pl => pl.ProductID == ProductId && pl.LocalizationID == saleLine.LocalizationID);   
                    }
                    else
                    {
                        //recuperation du frame
                        productInStock = context.ProductLocalizations.FirstOrDefault(pl => pl.ProductID == ProductId && pl.LocalizationID == saleLine.LocalizationID && pl.NumeroSerie == saleLine.NumeroSerie);
                    }

                    if (productInStock == null || productInStock.ProductLocalizationStockQuantity < saleLine.LineQuantity)
                    {
                        // s'il y a un stock viable ou de replacement, c'est a cet endroit qu'il y aura la sortie en stock
                        productInStock = this.GetPlanOrZeroReplacement(saleLine, productInStock);
                    }
                    
                    //sortie en stock
                    productInStock.SellingReference = currentSale.SaleReceiptNumber;
                    this.StockOutput(productInStock, saleLine.LineQuantity, productInStock.AveragePurchasePrice, currentSale.DeliverDate.Value, UserConect);

                    ProductId = 0;

                }
                
            }

            
            // _saleRep.ValideDeliverSpecialOrder(currentSale,  UserConect);

            //CumulSaleAndBill cumulSaleLine = context.CumulSaleAndBills.Where(auth => auth.CumulSaleAndBillID == currentSale.CumulSaleAndBillID).SingleOrDefault();
            //if (cumulSaleLine == null)
            //{
            //    throw new Exception("Error while updated command sale. please call your administrator ");
            //}
            //cumulSaleLine.IsDeliver = true;
            //cumulSaleLine.DeliverByID = UserConect;
            //cumulSaleLine.DateOperationHours = currentSale.DateOperationHours;
            //cumulSaleLine.DeliverDate = currentSale.DeliverDate.Value;
            //context.SaveChanges();

            //EcritureSneack
            IMouchar opSneak = new MoucharRepository(context);
            res = opSneak.InsertOperation(UserConect, "SUCCESS", "SALE-REFERENCE " + currentSale.SaleReceiptNumber + " FOR CUSTOMER " + currentSale.CustomerName, "PersistStockOutPut", currentSale.DeliverDate.Value, currentSale.BranchID);
            if (!res)
            {
                throw new Exception("Une erreur s'est produite lors de la journalisation ");
            }

            res = true;
            return res;
        }

        /// <summary>
        /// Context: on a en stock deux numeros qui representent en fait un seule: 0.00 et PLAN
        /// Cette fonction permet de retrouver l'equivalent avec PLAN si on a 0.00 ou
        /// l'equivalent avec 0.00 si on a PLAN
        /// Exemple; on peut avoir quantite en stock de a (0.00 +0.25 = 0) alors que 
        /// la quantite en stock de b (PLAN +0.25 = 5)
        /// dans ce cas, si on essaie de faire la sortie en stock de a, il faudra verifier le stock de b et reduire 
        /// la quantite en stock de b
        /// </summary>
        /// <param name="saleLine">Ligne dont on souhaite faire la sortie en stock</param>
        /// <param name="productInStock">Stock actuel pour pouvoir gerer le cas ou la quantite en stock est inferieure a 
        /// la quantite voulue pour les verres qui ne sont n'y 0.00 ou PLAN; puisque la verification n'est plus faite en haute</param>
        /// <returns></returns>
        public ProductLocalization GetPlanOrZeroReplacement(CumulSaleAndBillLine saleLine, ProductLocalization productInStock)
        {

            if ((saleLine.Product is Lens))
            {
                 Lens lens = (Lens)saleLine.Product;
                string sph = lens.LensNumber.LensNumberSphericalValue;

                if(sph != null && sph.ToLower() == "plan")
                {
                    productInStock = GetZeroFromPlan(lens, saleLine.LocalizationID);
                }

                if (sph != null && sph == "0.00")
                {
                    productInStock = GetPlanFromZero(lens, saleLine.LocalizationID);
                }                  
            }

            if (productInStock == null)
            {
                throw new Exception("Error Stock Output : product " + saleLine.Product.ProductCode + " not yet put on the stock; Please contact your accountant");
            }

            if (productInStock.ProductLocalizationStockQuantity < saleLine.LineQuantity)
            {
                throw new Exception("Error Stock Output : Insufficient stock for product " + saleLine.Product.ProductCode + " ; Please contact your accountant");
            }

            return productInStock;
        }

        /// <summary>
        /// retourne le stock ayant 0.00 au lieu de PLAN
        /// </summary>
        /// <param name="lens"></param>
        /// <param name="locationId"></param>
        /// <returns></returns>
        public ProductLocalization GetZeroFromPlan(Lens lens, int locationId)
        {
            ProductLocalization pl = null;
            string productCode = lens.ProductCode;
            productCode = productCode.Replace("PLAN", "0.00");
            pl = context.ProductLocalizations.FirstOrDefault(p => p.LocalizationID == locationId &&
                p.Product.ProductCode == productCode);

            return pl;
        }

        /// <summary>
        /// retourne le stock ayant PLAN au lieu de 0.00
        /// </summary>
        /// <param name="lens"></param>
        /// <param name="locationId"></param>
        /// <returns></returns>
        public ProductLocalization GetPlanFromZero(Lens lens, int locationId)
        {
            ProductLocalization pl = null;
            string productCode = lens.ProductCode;
            productCode = productCode.Replace("0.00", "PLAN");
            pl = context.ProductLocalizations.FirstOrDefault(p => p.LocalizationID == locationId &&
                p.Product.ProductCode == productCode);

            return pl;
        }
    }
}
