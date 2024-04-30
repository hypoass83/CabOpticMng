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
using FatSod.Security.Entities;
using System.Transactions;
using FatSod.Security.Abstracts;
using FatSod.DataContext.Concrete;

namespace FatSod.DataContext.Repositories
{
    /// <summary>
    /// 
    /// </summary>
    public class InventoryDirectoryRepository : RepositorySupply<InventoryDirectory>, IInventoryDirectory
    {
        IRepositorySupply<InventoryDirectoryLine> invDirLineRepo;
        ITransactNumber _transactNumbeRepository;

        public InventoryDirectoryRepository(EFDbContext context)
        {
            this.context = context;
        }
        public InventoryDirectoryRepository()
            : base()
        {

        }

        public InventoryDirectory CreateAndCloseInventoryDirectoryWithoutTransaction(InventoryDirectory inventoryDirectory, int UserConect)
        {
            try
            {
                SaveInventoryDirectory(inventoryDirectory, UserConect);
                CloseInventoryDirectorySansTransaction(inventoryDirectory, UserConect);
            }
            catch (Exception e)
            {
                //EcritureSneack
                IMouchar opSneak = new MoucharRepository(context);
                bool res1 = opSneak.InsertOperation(UserConect, "ERROR", "SAVE INVENTORY DIRECTORY " + inventoryDirectory.InventoryDirectoryDescription + " REF " + inventoryDirectory.InventoryDirectoryReference + " ERROR:" + e.Message, "SaveInventoryDirectory", inventoryDirectory.InventoryDirectoryCreationDate, inventoryDirectory.BranchID);
                if (!res1)
                {
                    throw new Exception("Une erreur s'est produite lors de la journalisation ");
                }
                //If an errors occurs, we cancel all changes in database
                //transaction.Rollback();
                throw new Exception(e.Message + ": Check " + e.StackTrace);
            }
            return inventoryDirectory;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="inventoryDirectory"></param>
        /// <param name="UserConect"></param>
        /// <returns></returns>
        public InventoryDirectory CreateAndCloseInventoryDirectory(InventoryDirectory inventoryDirectory, int UserConect)
        {
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    CreateAndCloseInventoryDirectoryWithoutTransaction(inventoryDirectory, UserConect);
                    ts.Complete();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message + ": Check " + e.StackTrace);
            }
            return inventoryDirectory;
        }

        public InventoryDirectory CreateInventoryDirectory(InventoryDirectory inventoryDirectory, int UserConect)
        {
            //Begin of transaction
                try
                {
                    using (TransactionScope ts = new TransactionScope())
                    {
                        SaveInventoryDirectory(inventoryDirectory, UserConect);

                        //transaction.Commit();
                        ts.Complete();
                    }
                    
                }
                catch (Exception e)
                {
                    //EcritureSneack
                    IMouchar opSneak = new MoucharRepository(context);
                    bool res1 = opSneak.InsertOperation(UserConect, "ERROR", "SAVE INVENTORY DIRECTORY " + inventoryDirectory.InventoryDirectoryDescription + " REF " + inventoryDirectory.InventoryDirectoryReference + " ERROR:" + e.Message, "SaveInventoryDirectory", inventoryDirectory.InventoryDirectoryCreationDate, inventoryDirectory.BranchID);
                    if (!res1)
                    {
                        throw new Exception("Une erreur s'est produite lors de la journalisation ");
                    }
                    //If an errors occurs, we cancel all changes in database
                    //transaction.Rollback();
                    throw new Exception(e.Message + ": Check " + e.StackTrace);
                }
                return inventoryDirectory;
        }
        private void CreateInvDirLine(InventoryDirectoryLine invDirLineSave, InventoryDirectory inventoryDirectory)
        {
            invDirLineSave.InventoryDirectoryID = inventoryDirectory.InventoryDirectoryID;
            this.context.InventoryDirectoryLines.Add(invDirLineSave);
            this.context.SaveChanges();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="inventoryDirectory"></param>
        /// <param name="UserConect"></param>
        /// <returns></returns>
        public InventoryDirectory CloseInventoryDirectorySansTransaction(InventoryDirectory inventoryDirectory, int UserConect)
        {

            try
            {

                IProductLocalization _plRepository = new PLRepository(this.context);

                List<InventoryDirectoryLine> invDirLines = inventoryDirectory.InventoryDirectoryLines.ToList();

                inventoryDirectory.InventoryDirectoryLines = null;

                this.Update(inventoryDirectory, inventoryDirectory.InventoryDirectoryID);

                invDirLineRepo = new RepositorySupply<InventoryDirectoryLine>(this.context);

                foreach (InventoryDirectoryLine invDirLine in invDirLines)
                {
                    invDirLine.InventoryDirectoryID = inventoryDirectory.InventoryDirectoryID;
                    invDirLineRepo.Update(invDirLine, invDirLine.InventoryDirectoryLineID);

                    ////nous devons nous assurer que le numero de serie est unique pour la categorie et la marque concerne
                    //ProductLocalization oldProdloc = this.context.ProductLocalizations.Where(p => p.LocalizationID == invDirLine.LocalizationID
                    //&& p.NumeroSerie.Trim() == invDirLine.NumeroSerie.Trim()).FirstOrDefault();
                    //if (oldProdloc!=null)
                    //{
                    //    if (oldProdloc.Marque != invDirLine.Marque)
                    //    {
                    //        throw new Exception("Error: This Database cannot accept two different Serial number (" + invDirLine.NumeroSerie + ") with the same Marque/Brand ");
                    //    }

                    //    if (oldProdloc.ProductID != invDirLine.ProductID)
                    //    {
                    //        throw new Exception("Error: This Database cannot accept two different Serial number (" + invDirLine.NumeroSerie + ") with the same Product ");
                    //    }
                    //}
                    
                    ProductLocalization ProdLoc = new ProductLocalization
                    {
                        LocalizationID = invDirLine.LocalizationID,
                        ProductID = invDirLine.ProductID,
                        ProductLocalizationDate = (DateTime)inventoryDirectory.InventoryDirectoryDate,
                        ProductLocalizationStockQuantity = invDirLine.NewStockQuantity.Value,
                        ProductLocalizationSafetyStockQuantity = (invDirLine.NewSafetyStockQuantity == null) ? 0 : invDirLine.NewSafetyStockQuantity.Value,//.StockDifference,
                        AveragePurchasePrice = invDirLine.AveragePurchasePrice,
                        inventoryReason = !inventoryDirectory.InventoryDirectoryDescription.Contains("Order Reception") ? 
                                                             "Inventory Entry" : invDirLine.inventoryReason,
                        CountByID = UserConect,
                        RegisteredByID = (inventoryDirectory.RegisteredByID == null) ? UserConect : inventoryDirectory.RegisteredByID.Value,
                        AutorizedByID = UserConect,
                        Description = inventoryDirectory.InventoryDirectoryDescription,
                        NumeroSerie = invDirLine.NumeroSerie,
                        Marque = invDirLine.Marque
                    };

                    //on approvisionne le stock 
                    _plRepository.StockInput(ProdLoc, ProdLoc.ProductLocalizationStockQuantity, invDirLine.AveragePurchasePrice, inventoryDirectory.InventoryDirectoryDate, UserConect);
                    //}

                }

                //EcritureSneack
                IMouchar opSneak = new MoucharRepository(context);
                bool res1 = opSneak.InsertOperation(UserConect, "SUCCESS", "CLOSE INVENTORY DIRECTORY " + inventoryDirectory.InventoryDirectoryDescription + " REF " + inventoryDirectory.InventoryDirectoryReference, "CloseInventoryDirectory", inventoryDirectory.InventoryDirectoryDate, inventoryDirectory.BranchID);
                if (!res1)
                {
                    throw new Exception("Une erreur s'est produite lors de la journalisation ");
                }

            }
            catch (Exception e)
            {
                //EcritureSneack
                IMouchar opSneak = new MoucharRepository(context);
                bool res1 = opSneak.InsertOperation(UserConect, "ERROR", "CLOSE INVENTORY DIRECTORY " + inventoryDirectory.InventoryDirectoryDescription + " REF " + inventoryDirectory.InventoryDirectoryReference, "CloseInventoryDirectory", inventoryDirectory.InventoryDirectoryDate, inventoryDirectory.BranchID);
                if (!res1)
                {
                    throw new Exception("Une erreur s'est produite lors de la journalisation ");
                }
                //If an errors occurs, we cancel all changes in database
                //transaction.Rollback();
                throw new Exception("Une erreur s'est produite lors de la fermeture du dossier d'inventaire car " + e.Message );
            }
            //}

            return inventoryDirectory;
        }

        public void CreateInvDirLine(List<InventoryDirectoryLine> InventoryDirectoryLines, int InventoryDirectoryID)
        {
            InventoryDirectoryLines.ToList().ForEach(idl =>
                                 {
                                     InventoryDirectoryLine invDirLineSave = new InventoryDirectoryLine();

                                     //ces 2 lignes qui suivent permettent de transformer un ProductModel en Product 
                                     //create a Map
                                     Mapper.CreateMap<InventoryDirectoryLine, InventoryDirectoryLine>();
                                     //use Map
                                     invDirLineSave = Mapper.Map<InventoryDirectoryLine>(idl);

                                     invDirLineSave.InventoryDirectoryID = InventoryDirectoryID;
                                     invDirLineSave.InventoryDirectory = null;
                                     invDirLineSave.Localization = null;
                                     invDirLineSave.Product = null;

                                     context.InventoryDirectoryLines.Add(invDirLineSave);
                                 });
        }
        public InventoryDirectory SaveInventoryDirectory(InventoryDirectory inventoryDirectory, int UserConect)
        {
            //bool res = false;
            
            InventoryDirectory inventoryDirectoryToSave = new InventoryDirectory();
            //ces 2 lignes qui suivent permettent de transformer un ProductModel en Product 
            //create a Map
            Mapper.CreateMap<InventoryDirectory, InventoryDirectory>();
            //use Map
            inventoryDirectoryToSave = Mapper.Map<InventoryDirectory>(inventoryDirectory);
            inventoryDirectoryToSave.InventoryDirectoryLines = null;

            inventoryDirectoryToSave = context.InventoryDirectories.Add(inventoryDirectoryToSave);
            context.SaveChanges();
            this.CreateInvDirLine(inventoryDirectory.InventoryDirectoryLines.ToList(), inventoryDirectoryToSave.InventoryDirectoryID);

            inventoryDirectory.InventoryDirectoryID = inventoryDirectoryToSave.InventoryDirectoryID;

            //EcritureSneack
            IMouchar opSneak = new MoucharRepository(context);
            bool res1 = opSneak.InsertOperation(UserConect, "SUCCESS", "SAVE INVENTORY DIRECTORY " + inventoryDirectory.InventoryDirectoryDescription + " REF " + inventoryDirectory.InventoryDirectoryReference, "SaveInventoryDirectory", inventoryDirectory.InventoryDirectoryCreationDate, inventoryDirectory.BranchID);
            if (!res1)
            {
                throw new Exception("Une erreur s'est produite lors de la journalisation ");
            }
            
            context.SaveChanges();

            return inventoryDirectory;
        }
        public bool DeleteInventoryDirectory(int InventoryDirectoryID)
        {
            bool res = false;
            //Begin of transaction
                try
                {
                    using (TransactionScope ts = new TransactionScope())
                    {
                        RemoveInventoryDirectory(InventoryDirectoryID);
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
                    throw new Exception("Une erreur s'est produite lors de la suppression du dossier d'inventaire car : " + e.Message);
                }
            return res;
        }
        public bool RemoveInventoryDirectory(int InventoryDirectoryID)
        {
            bool res = false;

            InventoryDirectory deletableInvDir = this.context.InventoryDirectories.SingleOrDefault(inDir => inDir.InventoryDirectoryID == InventoryDirectoryID);

            if (deletableInvDir.InventoryDirectoryStatut != InventoryDirectorySatut.Opened && deletableInvDir.InventoryDirectoryLines != null && deletableInvDir.InventoryDirectoryLines.Count > 0)
            {
                throw new Exception("Undeletable Inventory Directory! On ne peut pas supprimer un dossier d'inventaire donc le statut est différent de Ouvert et / ou ayant des lignes d'inventaire");
            }

            DeleteIDL(deletableInvDir.InventoryDirectoryLines.ToList());

            deletableInvDir.InventoryDirectoryLines = null;

            this.context.InventoryDirectories.Remove(deletableInvDir);
            this.context.SaveChanges();
            res = true;

            return res;
        }

        public void DeleteIDL(List<InventoryDirectoryLine> InventoryDirectoryLines)
        {
            this.context.InventoryDirectoryLines.RemoveRange(InventoryDirectoryLines);
            this.context.SaveChanges();
        }

        public void DeleteIDL(int InventoryDirectoryID)
        {
            this.context.InventoryDirectoryLines.RemoveRange(this.context.InventoryDirectoryLines.Where(idl => idl.InventoryDirectoryID == InventoryDirectoryID));
            this.context.SaveChanges();
        }

        public InventoryDirectory CloseInventoryDirectory(InventoryDirectory inventoryDirectory, int UserConect)
        {
            
                try
                {
                    using (TransactionScope ts = new TransactionScope())
                    {
                        IProductLocalization _plRepository = new PLRepository(this.context);

                        List<InventoryDirectoryLine> invDirLines = inventoryDirectory.InventoryDirectoryLines.ToList();

                        inventoryDirectory.InventoryDirectoryLines = null;

                        this.Update(inventoryDirectory, inventoryDirectory.InventoryDirectoryID);

                        invDirLineRepo = new RepositorySupply<InventoryDirectoryLine>(this.context);

                        foreach (InventoryDirectoryLine invDirLine in invDirLines)
                        {
                            invDirLine.InventoryDirectoryID = inventoryDirectory.InventoryDirectoryID;
                            invDirLineRepo.Update(invDirLine, invDirLine.InventoryDirectoryLineID);

                            ProductLocalization ProdLoc = new ProductLocalization
                            {
                                LocalizationID = invDirLine.LocalizationID,
                                ProductID = invDirLine.ProductID,
                                ProductLocalizationDate = (DateTime)inventoryDirectory.InventoryDirectoryDate,
                                ProductLocalizationStockQuantity = invDirLine.NewStockQuantity.Value,
                                ProductLocalizationSafetyStockQuantity = (invDirLine.NewSafetyStockQuantity == null) ? 0 : invDirLine.NewSafetyStockQuantity.Value,//.StockDifference,
                                AveragePurchasePrice = invDirLine.AveragePurchasePrice,
                                inventoryReason = "Inventory Entry",
                                CountByID = UserConect,
                                RegisteredByID = (inventoryDirectory.RegisteredByID == null) ? UserConect : inventoryDirectory.RegisteredByID.Value,
                                AutorizedByID = UserConect,
                                Description = inventoryDirectory.InventoryDirectoryDescription,
                                NumeroSerie = invDirLine.NumeroSerie,
                                Marque = invDirLine.Marque
                            };
                            //if (invDirLine.StockDifference < 0)
                            //{
                            //    //on dimuni le stock 
                            //    _plRepository.StockOutput(ProdLoc, ProdLoc.ProductLocalizationStockQuantity,invDirLine.AveragePurchasePrice);
                            //}
                            //else
                            //{
                                //on approvisionne le stock 
                                _plRepository.StockInput(ProdLoc, ProdLoc.ProductLocalizationStockQuantity,invDirLine.AveragePurchasePrice, inventoryDirectory.InventoryDirectoryDate, UserConect);
                            //}

                        }

                        //EcritureSneack
                        IMouchar opSneak = new MoucharRepository(context);
                        bool res1 = opSneak.InsertOperation(UserConect, "SUCCESS", "CLOSE INVENTORY DIRECTORY " + inventoryDirectory.InventoryDirectoryDescription + " REF " + inventoryDirectory.InventoryDirectoryReference, "CloseInventoryDirectory", inventoryDirectory.InventoryDirectoryDate, inventoryDirectory.BranchID);
                        if (!res1)
                        {
                            throw new Exception("Une erreur s'est produite lors de la journalisation ");
                        }
                        //transaction.Commit();
                        ts.Complete();
                    }
                }
                catch (Exception e)
                {
                    //EcritureSneack
                    IMouchar opSneak = new MoucharRepository(context);
                    bool res1 = opSneak.InsertOperation(UserConect, "ERROR", "CLOSE INVENTORY DIRECTORY " + inventoryDirectory.InventoryDirectoryDescription + " REF " + inventoryDirectory.InventoryDirectoryReference, "CloseInventoryDirectory", inventoryDirectory.InventoryDirectoryDate, inventoryDirectory.BranchID);
                    if (!res1)
                    {
                        throw new Exception("Une erreur s'est produite lors de la journalisation ");
                    }
                    //If an errors occurs, we cancel all changes in database
                    //transaction.Rollback();
                    throw new Exception("Une erreur s'est produite lors de la fermeture du dossier d'inventaire car " + e.Message + " " + e.StackTrace + " " + e.InnerException);
                }
            //}

            return inventoryDirectory;
        }
        /// <summary>
        /// Cette méthode retourne la liste des produits qui sont dans un dossier d'inventaire ayant les statuts opened ou closed.
        /// </summary>
        /// <param name="branch">Agence dans laquelle le dossier d'inventaire a été créée</param>
        /// <returns>liste des produits qui sont dans un dossier d'inventaire ayant les statuts opened ou closed</returns>
        public List<Product> LockedProducts(Branch branch)
        {
            List<Product> res = new List<Product>();

            invDirLineRepo = new RepositorySupply<InventoryDirectoryLine>(this.context);

            IEqualityComparer<Product> locationComparer = new GenericComparer<Product>("ProductCode");

            res = invDirLineRepo.FindAll.Where(idl => idl.InventoryDirectory.BranchID == branch.BranchID && idl.InventoryDirectory.InventoryDirectoryStatut == InventoryDirectorySatut.Opened ||
                                                 idl.InventoryDirectory.InventoryDirectoryStatut == InventoryDirectorySatut.InProgess)
                                                 .Select(idl1 => idl1.Product).Distinct(locationComparer).ToList();

            return res;

        }

        /// <summary>
        /// Cette méthode retourne la liste des produits qui sont dans un dossier d'inventaire ayant les statuts opened ou closed.
        /// </summary>
        /// <param name="location">magasin dans lequel le dossier d'inventaire a été créée</param>
        /// <returns>liste des produits qui sont dans un dossier d'inventaire ayant les statuts opened ou closed</returns>
        public List<Product> LockedProducts(Localization location)
        {
            List<Product> res = new List<Product>();

            invDirLineRepo = new RepositorySupply<InventoryDirectoryLine>(this.context);

            IEqualityComparer<Product> locationComparer = new GenericComparer<Product>("ProductCode");

            res = invDirLineRepo.FindAll.Where(idl => idl.LocalizationID == location.LocalizationID && 
                                                     (idl.InventoryDirectory.InventoryDirectoryStatut == InventoryDirectorySatut.Opened ||
                                                      idl.InventoryDirectory.InventoryDirectoryStatut == InventoryDirectorySatut.InProgess))
                                                        .Select(idl1 => idl1.Product).Distinct(locationComparer).ToList();

            return res;

        }

        /// <summary>
        /// Cette méthode retourne la liste des produits qui sont dans un dossier d'inventaire ayant les statuts opened ou closed.
        /// </summary>
        /// <returns>liste des produits qui sont dans un dossier d'inventaire ayant les statuts opened ou closed</returns>
        public List<Product> LockedProducts()
        {
            List<Product> res = new List<Product>();

            invDirLineRepo = new RepositorySupply<InventoryDirectoryLine>(this.context);

            IEqualityComparer<Product> locationComparer = new GenericComparer<Product>("ProductCode");

           res = invDirLineRepo.FindAll.Where(idl => idl.InventoryDirectory.InventoryDirectoryStatut == InventoryDirectorySatut.Opened ||
                                                idl.InventoryDirectory.InventoryDirectoryStatut == InventoryDirectorySatut.InProgess)
                                                .Select(idl1 => idl1.Product).Distinct(locationComparer).ToList();

            return res;

        }
        public InventoryDirectory UpdateInventoryDirectory(InventoryDirectory inventoryDirectory, int UserConect)
        {
            
                try
                {
                    using (TransactionScope ts = new TransactionScope())
                    {
                        //Suppression des anciennes
                        DeleteIDL(inventoryDirectory.InventoryDirectoryID);

                        //Création des nouvelles
                        CreateInvDirLine(inventoryDirectory.InventoryDirectoryLines.ToList(), inventoryDirectory.InventoryDirectoryID);

                        //Mise à jour du dossier d'inventaire
                        this.Update(inventoryDirectory, inventoryDirectory.InventoryDirectoryID);
                        //If an errors occurs, we cancel all changes in database
                        //transaction.Commit();
                        //EcritureSneack
                        IMouchar opSneak = new MoucharRepository(context);
                        bool res1 = opSneak.UpdateOperation(UserConect, "SUCCESS", "UPDATE INVENTORY DIRECTORY " + inventoryDirectory.InventoryDirectoryDescription + " REF " + inventoryDirectory.InventoryDirectoryReference, "UpdateInventoryDirectory", inventoryDirectory.InventoryDirectoryDate, inventoryDirectory.BranchID);
                        if (!res1)
                        {
                            throw new Exception("Une erreur s'est produite lors de la journalisation ");
                        }
                        ts.Complete();
                    }
                }
                catch (Exception ex)
                {
                    //EcritureSneack
                    IMouchar opSneak = new MoucharRepository(context);
                    bool res1 = opSneak.UpdateOperation(UserConect, "ERROR", "UPDATE INVENTORY DIRECTORY " + inventoryDirectory.InventoryDirectoryDescription + " REF " + inventoryDirectory.InventoryDirectoryReference, "UpdateInventoryDirectory", inventoryDirectory.InventoryDirectoryDate, inventoryDirectory.BranchID);
                    if (!res1)
                    {
                        throw new Exception("Une erreur s'est produite lors de la journalisation ");
                    }
                    //transaction.Rollback();
                    throw new Exception(ex.Message + ": Check " + ex.StackTrace);

                }
            //}
            return inventoryDirectory;
        }

        public List<InventoryDirectoryLine> LockedInventoryDirectoryLines()
        {
            List<InventoryDirectoryLine> res = new List<InventoryDirectoryLine>();

            invDirLineRepo = new RepositorySupply<InventoryDirectoryLine>(this.context);

            IEqualityComparer<InventoryDirectoryLine> locationComparer = new GenericComparer<InventoryDirectoryLine>("ProductID", "LocalizationID");

            res = invDirLineRepo.FindAll.Where(idl => idl.InventoryDirectory.InventoryDirectoryStatut == InventoryDirectorySatut.Opened ||
                                                 idl.InventoryDirectory.InventoryDirectoryStatut == InventoryDirectorySatut.InProgess)
                                                 .Distinct(locationComparer).ToList();

            return res;

        }


        #region Special Order Reception Stock In Put
        public InventoryDirectory SpecialOrderReceptionStockInPut(CumulSaleAndBill cumulSaleAndBill, int UserConect)
        {
            InventoryDirectory inventoryDirectory = getInventoryDirectory(cumulSaleAndBill, UserConect);
            inventoryDirectory.InventoryDirectoryLines = getInventoryDirectorylines(cumulSaleAndBill, UserConect);

            try
            {
                inventoryDirectory = CreateAndCloseInventoryDirectoryWithoutTransaction(inventoryDirectory, UserConect);
            }
            catch (Exception e)
            {
                //EcritureSneack
                IMouchar opSneak = new MoucharRepository(context);
                bool res1 = opSneak.InsertOperation(UserConect, "ERROR", "SAVE INVENTORY DIRECTORY " + inventoryDirectory.InventoryDirectoryDescription + " REF " + inventoryDirectory.InventoryDirectoryReference + " ERROR:" + e.Message, "SaveInventoryDirectory", inventoryDirectory.InventoryDirectoryCreationDate, inventoryDirectory.BranchID);
                if (!res1)
                {
                    throw new Exception("Une erreur s'est produite lors de la journalisation ");
                }
                //If an errors occurs, we cancel all changes in database
                //transaction.Rollback();
                throw new Exception(e.Message + ": Check " + e.StackTrace);
            }
            return inventoryDirectory;
        }

        InventoryDirectory getInventoryDirectory(CumulSaleAndBill cumulSaleAndBill, int userConnect)
        {
            BusinessDay businessDay = new BusinessDay()
            {
                BranchID = cumulSaleAndBill.BranchID,
                BDDateOperation = cumulSaleAndBill.SpecialOrderReceptionDateHeure.Value
            };
            string code = null;
            if (cumulSaleAndBill.CumulSaleAndBillLines.Any(l => l.isCommandGlass))
            {
                code = "RXOR"; // RX Order
            }

            if (code == null)
            {
                if (cumulSaleAndBill.CumulSaleAndBillLines.All(l => !l.isCommandGlass))
                {
                    code = "STKO"; // Stock Order
                }
            }

            if (code == null)
            {
                code = "SPOR"; // Special Order Reception
            }
            string receptionType = code == "STKO" ? "Stock Order Reception" : "RX Order Reception";
            _transactNumbeRepository = new TransactNumberRepository(context);
            string transactNumber = _transactNumbeRepository.returnTransactNumber(code, businessDay);
            InventoryDirectory id = new InventoryDirectory()
            {
                BranchID = cumulSaleAndBill.BranchID,
                InventoryDirectoryCreationDate = cumulSaleAndBill.SpecialOrderReceptionDateHeure.Value,
                InventoryDirectoryDate = cumulSaleAndBill.SpecialOrderReceptionDateHeure.Value,
                InventoryDirectoryDescription = receptionType + "(" + cumulSaleAndBill.SaleReceiptNumber + ")",
                InventoryDirectoryID = 0,
                InventoryDirectoryReference = transactNumber,
                RegisteredByID = userConnect,
            };

            return id;
        }

        public List<InventoryDirectoryLine> getInventoryDirectorylines(CumulSaleAndBill cumulSaleAndBill, int UserConnect)
        {
            List<InventoryDirectoryLine> inventoryDirectoryLines = new List<InventoryDirectoryLine>();

            foreach (CumulSaleAndBillLine csbLine in cumulSaleAndBill.CumulSaleAndBillLines)
            {
                if (csbLine.Product is Lens || csbLine.Product is OrderLens)
                {

                    int ProductID = csbLine.ProductID;

                    if (csbLine.Product is OrderLens)
                    {
                        Lens l = LensConstruction.GetLensFromOrderLens(csbLine.Product.ProductID, context, csbLine);
                        ProductID = l.ProductID;
                    }

                    string inventoryReason = null;
                    if (csbLine.isCommandGlass)
                    {
                        inventoryReason = "RX Lenses Reception"; // RX Order
                    }

                    if (inventoryReason == null)
                    {
                        if (!csbLine.isCommandGlass)
                        {
                            inventoryReason = "Stock Order Reception"; // Stock Order
                        }
                    }

                    if (inventoryReason == null)
                    {
                        inventoryReason = "Special Order Reception"; // Special Order Reception
                    }

                    InventoryDirectoryLine idLine = new InventoryDirectoryLine()
                    {
                        ProductID = ProductID,
                        LocalizationID = csbLine.LocalizationID,
                        AutorizedByID = null,
                        AveragePurchasePrice = 0,
                        CountByID = null,
                        InventoryDirectoryID = 0,
                        inventoryReason = inventoryReason + "(" + cumulSaleAndBill.SaleReceiptNumber + ")",
                        LocalizationLabel = null,
                        NewStockQuantity = csbLine.LineQuantity,
                        RegisteredByID = UserConnect,
                        InventoryDirectoryLineID = 0,
                    };
                    inventoryDirectoryLines.Add(idLine);
                }
            }
            return inventoryDirectoryLines;
        }

        #endregion


        #region Barcode Inventory Reconciliation
        /// <summary>
        /// Faire la reconciliation du stock apres linventaire
        /// </summary>
        /// <param name="reconciliation"></param>
        /// <param name="reconciliationLines"></param>
        public void CreateReconciliation(InventoryReconciliation reconciliation, List<InventoryReconciliationLine> reconciliationLines)
        {
            var inventoryCounting = new InventoryCounting();
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    inventoryCounting = context.InventoryCountings.Find(reconciliation.InventoryCountingId);
                    #region 1- Mise a jour du stock si necessaire
                    IProductLocalization _plRepository = new PLRepository(this.context);

                    reconciliationLines.ForEach(line =>
                    {
                        var stock = context.ProductLocalizations.AsNoTracking()
                                    .SingleOrDefault(s => s.ProductLocalizationID == line.StockId);
                        stock.inventoryReason = "INVENTORY RECONCILIATION - " + inventoryCounting.Reference;

                        // Augmentation du stock
                        if (line.StockQuantity < line.ReconciliationQuantity)
                        {
                            stock.inventoryReason = "Stock Input - " + stock.inventoryReason;
                            var addedQuantity = line.ReconciliationQuantity - line.StockQuantity;
                            _plRepository.StockInput(stock, addedQuantity, stock.AveragePurchasePrice, reconciliation.ReconciliationDate, reconciliation.RegisteredById);
                        }

                        // Reduction du stock
                        if (line.StockQuantity > line.ReconciliationQuantity)
                        {
                            stock.inventoryReason = "Stock Output - " + stock.inventoryReason;
                            var reducedQuantity = line.StockQuantity - line.ReconciliationQuantity;
                            _plRepository.StockOutput(stock, reducedQuantity, stock.AveragePurchasePrice, reconciliation.ReconciliationDate, reconciliation.RegisteredById);
                        }
                    });

                    //on approvisionne le stock 
                    #endregion

                    #region 2- Fermeture du dossier d,inventaire en indiquant la date de fermeture
                    inventoryCounting.ClosedDate = reconciliation.ReconciliationDate;
                    context.SaveChanges();
                    #endregion

                    #region 3- Creation de la Reconciliation
                    var createdReconciliation = context.InventoryReconciliations.Add(reconciliation);
                    context.SaveChanges();
                    #endregion

                    #region 4- Creation des lignes de reconciliation
                    reconciliationLines.ForEach(line =>
                    {
                        line.InventoryReconciliationId = createdReconciliation.InventoryReconciliationId;
                    });

                    context.InventoryReconciliationLines.AddRange(reconciliationLines);
                    context.SaveChanges();
                    #endregion

                    ts.Complete();
                }

            }
            catch (Exception e)
            {
                //EcritureSneack
                IMouchar opSneak = new MoucharRepository(context);
                bool res1 = opSneak.InsertOperation(reconciliation.RegisteredById, "ERROR", "INVENTORY RECONCILIATION " + " ERROR:" + e.Message, "SaveInventoryDirectory", reconciliation.ReconciliationDate, inventoryCounting.BranchId);
                if (!res1)
                {
                    throw new Exception("Une erreur s'est produite lors de la journalisation ");
                }
                //If an errors occurs, we cancel all changes in database
                //transaction.Rollback();
                throw new Exception(e.Message + ": Check " + e.StackTrace);
            }
        }

        #endregion

    }
}

