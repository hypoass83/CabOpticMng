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
    public class ProductDamageRepository : RepositorySupply<ProductDamage>, IProductDamage
    {
        IProductLocalization _plRepository;

        public ProductDamage DoProductDamage(ProductDamage productDamage, int UserConnect)
        {
            //int res = 0;
            try
            {
                //Begin of transaction
                using (TransactionScope ts = new TransactionScope())
                {
                    //_plRepository = new PLRepository(this.context);

                    ProductDamage doProductDamage = new ProductDamage();

                    //ces 2 lignes qui suivent permettent de transformer un ProductDamage en ProductDamage 
                    //create a Map
                    Mapper.CreateMap<ProductDamage, ProductDamage>();
                    //use Map
                    doProductDamage = Mapper.Map<ProductDamage>(productDamage);
                    //default receiver 
                    //transfert.ReceivedByID = 2;
                    doProductDamage.ProductDamageLines = null;
                    this.Create(doProductDamage);
                    productDamage.ProductDamageID = doProductDamage.ProductDamageID;

                    //Création des lignes de transfert
                    CreateproductDamageLine(productDamage.ProductDamageLines.ToList(), productDamage.ProductDamageID);

                    //Mise à jour du stock de départ
                    if (!productDamage.IsLensMountingDamage)
                    {
                        OutPutStocksUpdate(productDamage, UserConnect);
                    }

                    //mise a jour du cpteur du transact number
                    TransactNumber trn = context.TransactNumbers.SingleOrDefault(t => t.TransactNumberCode == "PRDA");
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
            return productDamage;
        }

        /// <summary>
        /// Cette méthode permet de constater la reception des produits par l'agence d'arrivée
        /// </summary>
        /// <param name="productDamage"></param>
        /// <returns></returns>
        public ProductDamage ValidateProductDamage(ProductDamage pt)
        {
            //bool res = false;

            ProductDamage productDamage = new ProductDamage();
            //Begin of transaction

            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    if (pt.ProductDamageID <= 0)
                    {
                        ProductDamage productDamageInsert = new ProductDamage();

                        //ces 2 lignes qui suivent permettent de transformer un ProductDamage en ProductDamage 
                        //create a Map
                        Mapper.CreateMap<ProductDamage, ProductDamage>();
                        //use Map
                        productDamageInsert = Mapper.Map<ProductDamage>(pt);
                        //default receiver 
                        productDamageInsert.RegisteredByID = pt.RegisteredByID;
                        productDamageInsert.AutorizedByID = pt.AutorizedByID;
                        productDamageInsert.ProductDamageLines = null;
                        productDamageInsert.ProductDamageDate = pt.ProductDamageDate;
                        this.Create(productDamageInsert);
                        pt.ProductDamageID = productDamageInsert.ProductDamageID;

                        //Création des lignes de transfert
                        CreateproductDamageLine(pt.ProductDamageLines.ToList(), pt.ProductDamageID);
                        productDamageInsert = pt;
                    }
                    else
                    {
                        productDamage = context.ProductDamages.AsNoTracking().SingleOrDefault(pt1 => pt1.ProductDamageID == pt.ProductDamageID);
                        productDamage.ProductDamageDate = pt.ProductDamageDate;
                        productDamage.RegisteredByID = pt.RegisteredByID;
                        productDamage.AutorizedByID = pt.AutorizedByID;

                        List<ProductDamageLine> lines = productDamage.ProductDamageLines.ToList();

                        productDamage.ProductDamageLines = null;

                        context.ProductDamages.Attach(productDamage);
                        context.Entry(productDamage).State = EntityState.Modified;
                        context.SaveChanges();

                        foreach (ProductDamageLine ptl in lines)
                        {
                            //ptl.ArrivalLocalizationID = ArrivalLocalizationID;
                            context.ProductDamageLines.Attach(ptl);
                            context.Entry(ptl).State = EntityState.Modified;
                            context.SaveChanges();
                        }
                    }

                    //Mise à jour du stock d'arrivé
                    InputStocksUpdate(productDamage);

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
            return productDamage;
        }



        public bool CancelProductDamage(int productDamageID)
        {
            bool res = false;
            try
            {
                //Begin of transaction
                using (TransactionScope ts = new TransactionScope())
                {
                    ProductDamage productDamage = this.Find(productDamageID);

                    //if (productDamage != null && productDamage.ProductDamageID > 0 )
                    //{
                    //    throw new Exception("Verry Sorry this Operation was already validated. You can not be cancelled");
                    //}

                    if (productDamage == null || productDamage.ProductDamageID <= 0)
                    {
                        throw new Exception("Sorry! This Transaction doesn't exist");
                    }

                    this.PTLDeleteAndStockUpdate(productDamage.ProductDamageLines.ToList(), productDamage.ProductDamageDate);

                    this.Delete(productDamage);
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
        /// <param name="ProductDamageLines"></param>
        /// <param name="dateOp"></param>
        public void PTLDeleteAndStockUpdate(List<ProductDamageLine> ProductDamageLines, DateTime dateOp)
        {
            //Réapprovisionnement du stock de départ
            foreach (ProductDamageLine ptl in ProductDamageLines)
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
                    Description = ptl.ProductDamageReason
                };

                _plRepository = new PLRepository(this.context);

                _plRepository.StockInput(DepartureProdLoc, DepartureProdLoc.ProductLocalizationStockQuantity, DepartureProdLoc.AveragePurchasePrice, dateOp, null);

            }

            //Suppression des lignes de transfert
            context.ProductDamageLines.RemoveRange(ProductDamageLines);
            context.SaveChanges();
        }


        public void CreateproductDamageLine(List<ProductDamageLine> productDamageLines, int productDamageID)
        {
            productDamageLines.ToList().ForEach(pt =>
            {
                ProductDamageLine ptLine = new ProductDamageLine()
                {
                    LocalizationID = pt.LocalizationID,
                    ProductDamageReason = pt.ProductDamageReason,
                    LineQuantity = pt.LineQuantity,
                    LineUnitPrice = pt.LineUnitPrice,
                    ProductID = pt.ProductID,
                    ProductDamageID = productDamageID,
                    NumeroSerie = pt.NumeroSerie,
                    Marque = pt.Marque
                };
                context.ProductDamageLines.Add(ptLine);
            });
            context.SaveChanges();
        }

        /// <summary>
        /// réduire les stocks qui envoient les produits
        /// </summary>
        /// <param name="productDamage"></param>
        public void OutPutStocksUpdate(ProductDamage productDamage, int UserConnect)
        {
            try
            {
                IProductLocalization _plRepository = new PLRepository(this.context);
                foreach (
                    ProductDamageLine productDamageLine in
                    productDamage.ProductDamageLines)
                {
                    ProductLocalization OutPutProdLoc = new ProductLocalization
                    {
                        LocalizationID = productDamageLine.LocalizationID,
                        ProductID = productDamageLine.ProductID,
                        ProductLocalizationDate = productDamage.ProductDamageDate,
                        ProductLocalizationStockQuantity = productDamageLine.LineQuantity,
                        AveragePurchasePrice = productDamageLine.LineUnitPrice,
                        inventoryReason = "Product Damage",
                        RegisteredByID = UserConnect,
                        Description = productDamageLine.ProductDamageReason,
                        NumeroSerie = productDamageLine.NumeroSerie,
                        Marque = productDamageLine.Marque
                    };
                    //on enlève les produits du stock de départ
                    _plRepository.StockOutput(OutPutProdLoc, OutPutProdLoc.ProductLocalizationStockQuantity, OutPutProdLoc.AveragePurchasePrice, productDamage.ProductDamageDate, UserConnect);
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
        /// <param name="productDamage"></param>
        public void InputStocksUpdate(ProductDamage productDamage)
        {
            IProductLocalization _plRepository = new PLRepository(this.context);

            foreach (
                ProductDamageLine productDamageLine in productDamage.ProductDamageLines)
            {

                ProductLocalization StockInputProdLoc = new ProductLocalization
                {
                    LocalizationID = productDamageLine.LocalizationID,
                    ProductID = productDamageLine.ProductID,
                    ProductLocalizationDate = (DateTime)productDamage.ProductDamageDate,
                    ProductLocalizationStockQuantity = productDamageLine.LineQuantity,
                    AveragePurchasePrice = productDamageLine.LineUnitPrice,
                    NumeroSerie = productDamageLine.NumeroSerie,
                    Marque = productDamageLine.Marque,
                    Description = productDamageLine.ProductDamageReason
                };
                //on approvisionne le stock d'arrivé
                _plRepository.StockInput(StockInputProdLoc, StockInputProdLoc.ProductLocalizationStockQuantity, StockInputProdLoc.AveragePurchasePrice, productDamage.ProductDamageDate, null);
            }


        }

        /// <summary>
        /// Cette méthode permet de modifier un transfert.
        /// Ici, nous faison un drop and create
        /// </summary>
        /// <param name="productDamage"></param>
        /// <returns></returns>
        public ProductDamage UpdateProductDamage(ProductDamage productDamage)
        {
            //bool res = false;


            try
            {
                using (TransactionScope ts = new TransactionScope())
                {

                    //1-Suppression des anciennes lignes de transfert
                    List<ProductDamageLine> ProductDamageLines = this.context.ProductDamageLines.Where(ptl => ptl.ProductDamageID == productDamage.ProductDamageID).ToList();
                    this.PTLDeleteAndStockUpdate(ProductDamageLines, productDamage.ProductDamageDate);

                    //2-Création des nouvelles lignes de transfert
                    this.CreateproductDamageLine(productDamage.ProductDamageLines.ToList(), productDamage.ProductDamageID);

                    //3-Mise à jour des informations sur le transfert
                    this.Update(productDamage, productDamage.ProductDamageID);
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

            return productDamage;
        }

    }
}
