using AutoMapper;
using FatSod.DataContext.Concrete;
using FatSod.DataContext.Initializer;
using FatSod.Security.Abstracts;
using FatSod.Supply.Abstracts;
using FatSod.Supply.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace FatSod.DataContext.Repositories
{
    /// <summary>
    /// classe permetant de gerer les factures pour les assurances
    /// </summary>
    public class CustomerOrderRepository : RepositorySupply<CustomerOrder>, ICustomerOrder
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="customerOrder"></param>
        /// <param name="heureVente"></param>
        /// <param name="newsCustomerOrderLine"></param>
        /// <param name="UserConect"></param>
        /// <param name="spray"></param>
        /// <param name="boitier"></param>
        /// <returns></returns>
        public CustomerOrder UpdateCustomerOrder(CustomerOrder customerOrder, string heureVente, List<CustomerOrderLine> newsCustomerOrderLine, int UserConect,int spray, int boitier)
        {
                try
                {
                    using (TransactionScope ts = new TransactionScope())
                    {
                        Product existProduct = new Product();
                        Assureur assure = context.Assureurs.Find(customerOrder.AssureurID);
                        if (assure == null)
                        {
                            throw new Exception("You must create this Insurance company before proceed");
                        }
                        string NumeroFacture = generateBill(assure.CompteurFacture);
                        customerOrder.NumeroFacture =NumeroFacture+ "/" + customerOrder.CustomerOrderDate.Year.ToString();

                        //ajout de lheure de la vente
                        string[] tisys = heureVente.Split(new char[] { ':' });
                        DateTime date = customerOrder.CustomerOrderDate;
                        date = date.AddHours(Convert.ToDouble(tisys[0]));
                        date = date.AddMinutes(Convert.ToDouble(tisys[1]));
                        date = date.AddSeconds(Convert.ToDouble(tisys[2]));
                        //we create a new command
                        customerOrder.CustomerDateHours = date;

                    /*
                    //ajout du spray et du case
                    //Ajout des infos pour le spray
                    if (spray > 0)
                    {
                        Product lensSpray = context.Products.Where(p => p.ProductCode.ToUpper().Trim() == "LENS SPRAY").FirstOrDefault();
                        if (lensSpray == null) throw new Exception("Product LENS SPRAY Not yet create in the Database");
                        //check stock quantity
                        ProductLocalization PLLensSpay = context.ProductLocalizations.FirstOrDefault(pl => pl.ProductID == lensSpray.ProductID);
                        if (PLLensSpay.ProductLocalizationStockQuantity <= 0)
                        {
                            throw new Exception("Insuffisant Product LENS SPRAY on stock!! Please add product on stock before proceed");
                        }
                        newsCustomerOrderLine.Add(
                            new CustomerOrderLine()
                            {
                                EyeSide = EyeSide.N,
                                LineQuantity = 1,
                                LineUnitPrice = 0,
                                LocalizationID = PLLensSpay.LocalizationID,
                                OeilDroiteGauche = EyeSide.N,
                                ProductID = lensSpray.ProductID,
                                PurchaseLineUnitPrice = 0,
                                LineID = 0,
                                SpecialOrderLineCode = null,
                                LensCategoryCode = null,
                                isGift = true
                            }
                            );
                    }
                    //Ajout des infos pour le boitier
                    if (boitier > 0)
                    {
                        Product lensCases = context.Products.Where(p => p.ProductCode.ToUpper().Trim() == "LENS CASES").FirstOrDefault();
                        if (lensCases == null) throw new Exception("Product LENS CASES Not yet create in the Database");
                        //check stock quantity
                        ProductLocalization PLLensCase = context.ProductLocalizations.FirstOrDefault(pl => pl.ProductID == lensCases.ProductID);
                        if (PLLensCase.ProductLocalizationStockQuantity<=0)
                        {
                            throw new Exception("Insuffisant Product LENS CASE on stock!! Please add product on stock before proceed");
                        }
                        newsCustomerOrderLine.Add(
                            new CustomerOrderLine()
                            {
                                EyeSide = EyeSide.N,
                                LineQuantity = 1,
                                LineUnitPrice = 0,
                                LocalizationID = PLLensCase.LocalizationID,
                                OeilDroiteGauche = EyeSide.N,
                                ProductID = lensCases.ProductID,
                                PurchaseLineUnitPrice = 0,
                                LineID = 0,
                                SpecialOrderLineCode = null,
                                LensCategoryCode = null,
                                isGift = true
                            });
                    }*/


                    //Mise à jour de la commande
                    this.Update(customerOrder, customerOrder.CustomerOrderID);

                    //ecriture de cumulsale and bill table
                    CumulSaleAndBill newcumlsalebill = new CumulSaleAndBill()
                    {
                        VatRate = customerOrder.VatRate,
                        RateReduction = customerOrder.RateReduction,
                        RateDiscount = customerOrder.RateDiscount,
                        Transport = customerOrder.Transport,
                        SaleDate = customerOrder.ValidateBillDate,
                        SaleReceiptNumber = customerOrder.CustomerOrderNumber,
                        BranchID = customerOrder.BranchID,
                        CustomerName = customerOrder.CustomerName,
                        OriginSale = OriginSaleOperation.Bill,
                        Remarque = customerOrder.Remarque,
                        MedecinTraitant = customerOrder.MedecinTraitant,
                        OperatorID = customerOrder.OperatorID,
                        CustomerOrderID = customerOrder.CustomerOrderID,
                        DateOperationHours = customerOrder.CustomerDateHours.Value,
                        CustomerID = customerOrder.CustomerID
                    };
                    newcumlsalebill=context.CumulSaleAndBills.Add(newcumlsalebill);
                    context.SaveChanges();
                    int cumulSaleAndBillID = newcumlsalebill.CumulSaleAndBillID;

                    //Mise à jour des lignes de commande : Je ne veux pas de stress : Suppression des anciennes et création des nouvelles
                    //1-Suppression des anciennes
                    List<CustomerOrderLine> oldCustomerOrderLines = context.CustomerOrderLines.Where(ol => ol.CustomerOrderID == customerOrder.CustomerOrderID).ToList();
                    context.CustomerOrderLines.RemoveRange(oldCustomerOrderLines);

                    //2- Création des nouvelles
                    foreach (CustomerOrderLine col in newsCustomerOrderLine)
                    {
                        
                        CustomerOrderLine newLineToCreate = new CustomerOrderLine()
                        {
                            ProductID = col.ProductID,
                            LocalizationID = col.LocalizationID,
                            LineUnitPrice = col.LineUnitPrice,
                            LineQuantity = col.LineQuantity,
                            CustomerOrderID = customerOrder.CustomerOrderID,
                            OeilDroiteGauche = col.OeilDroiteGauche,
                            SpecialOrderLineCode = col.SpecialOrderLineCode,
                            marque = col.marque,
                            reference = col.reference,
                            FrameCategory=col.FrameCategory,
                            NumeroSerie = col.NumeroSerie,
                            LensNumberSphericalValue = col.LensNumberSphericalValue,
                            LensNumberCylindricalValue = col.LensNumberCylindricalValue,
                            Axis = col.Axis,
                            Addition = col.Addition,
                            isCommandGlass = col.isCommandGlass
                        };
                        context.CustomerOrderLines.Add(newLineToCreate);

                        CumulSaleAndBillLine CumulSaleAndBillLineToSave = new CumulSaleAndBillLine()
                        {
                            LocalizationID = col.LocalizationID,
                            ProductID = col.ProductID,
                            CumulSaleAndBillID = cumulSaleAndBillID,
                            LineQuantity = col.LineQuantity,
                            LineUnitPrice = col.LineUnitPrice,
                            OeilDroiteGauche = col.OeilDroiteGauche,
                            SpecialOrderLineCode = col.SpecialOrderLineCode,
                            marque = col.marque,
                            reference = col.reference,
                            SupplyingName = col.SupplyingName,
                            isGift = col.isGift,
                            LensNumberSphericalValue = col.LensNumberSphericalValue,
                            LensNumberCylindricalValue = col.LensNumberCylindricalValue,
                            Axis = col.Axis,
                            Addition = col.Addition,
                            NumeroSerie = col.NumeroSerie,
                            isCommandGlass = col.isCommandGlass
                        };
                        context.CumulSaleAndBillLines.Add(CumulSaleAndBillLineToSave);
                        context.SaveChanges();


                        existProduct = this.context.Products.Find(col.ProductID);

                        //DESormais seul le comptable pourra sortir le produit en stock
                        // sortie du produit en stock
                        IProductLocalization _plRepository = new PLRepository(this.context);
                        //sortie en stock

                        if (existProduct is GenericProduct) //il s'agit d'un frame
                        {
                            ProductLocalization ProdLoc = new ProductLocalization();
                            ProdLoc = (existProduct.Category.isSerialNumberNull) ? context.ProductLocalizations.Where(p => p.ProductID == col.ProductID && p.LocalizationID == col.LocalizationID
                           && p.NumeroSerie == col.NumeroSerie && p.Marque == col.marque).FirstOrDefault() : context.ProductLocalizations.Where(p => p.ProductID == col.ProductID && p.LocalizationID == col.LocalizationID).FirstOrDefault();
                            if (ProdLoc.ProductLocalizationStockQuantity <= 0)
                            {
                                if (existProduct.Category.isSerialNumberNull)
                                {
                                    throw new Exception("Insuffisant selected Product " + col.NumeroSerie + " Marque " + col.marque + " on stock!! Please add product on stock before proceed");
                                }
                                else
                                {
                                    throw new Exception("Insuffisant selected Product " + existProduct.ProductCode + " on stock!! Please add product on stock before proceed");
                                }
                            }

                            ProdLoc.Description = "For Insurance / Bill Ref :" + customerOrder.NumeroFacture;
                            _plRepository.StockOutput(ProdLoc, col.LineQuantity, (customerOrder.MontureAssurance), customerOrder.CustomerOrderDate, UserConect);
                        }

                    }

                    
                    //3 - update du compteur ds la table des assureurs

                    if (assure != null)
                    {
                        assure.CompteurFacture = Convert.ToInt32(NumeroFacture);
                    }
                    //we apply this modifications
                    context.SaveChanges();

                    // Mise a jour de la valeur(VIP | ECO) du client qui vient de faire cet achat
                    this.updateInsuredCustomerValue(customerOrder.CustomerOrderID, customerOrder.TotalPriceTTC);
                    //EcritureSneack
                    IMouchar opSneak = new MoucharRepository(context);
                    bool res1 = opSneak.InsertOperation(UserConect, "SUCCESS", "UPDATE ORDER CUSTOMER " + customerOrder.CustomerName + "REFERENCE " + customerOrder.CustomerOrderNumber, "UpdateCustomerOrder", customerOrder.CustomerOrderDate, customerOrder.BranchID);
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
                bool res1 = opSneak.InsertOperation(UserConect, "ERROR", "UPDATE ORDER CUSTOMER " + customerOrder.CustomerName, "UpdateCustomerOrder", customerOrder.CustomerOrderDate, customerOrder.BranchID);
                if (!res1)
                {
                    throw new Exception("Une erreur s'est produite lors de la journalisation ");
                }
                //If an errors occurs, we cancel all changes in database
                //transaction.Rollback();
                throw new Exception("Error : " + e.Message);
            }
            return customerOrder;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="customerOrder"></param>
        /// <param name="UserConect"></param>
        /// <returns></returns>
        public CustomerOrder SaveChanges(CustomerOrder customerOrder, int UserConect)
        {
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {

                    customerOrder = PersistCustomerOrder(customerOrder, UserConect);
                    ts.Complete();
                }
            }
            catch (Exception e)
            {
                throw new Exception("Une erreur s'est produite lors de la vente : " + "e.Message = " + e.Message + "e.InnerException = " + e.InnerException + "e.StackTrace = " + e.StackTrace);
            }
            return customerOrder;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="DeleteReason"></param>
        /// <param name="UserConect"></param>
        /// <param name="OperationDate"></param>
        /// <returns></returns>
        public CustomerOrder DeleteValidatedBill(int ID, string DeleteReason, int UserConect, DateTime OperationDate)
        {
            CustomerOrder customerOrdersToDelete = new CustomerOrder();
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {

                    //suppression histo

                    List<CumulSaleAndBill> cumulSaleAndBillsToDelete = context.CumulSaleAndBills.Where(c => c.CustomerOrderID == ID).ToList();
                    if (cumulSaleAndBillsToDelete.Count > 0)
                    {
                        foreach (CumulSaleAndBill c in cumulSaleAndBillsToDelete)
                        {
                            //suppression de l'histo
                            List<CumulSaleAndBillLine> lstCumulSaleAndBillLines = context.CumulSaleAndBillLines.Where(cs => cs.CumulSaleAndBillID == c.CumulSaleAndBillID).ToList();
                            if (lstCumulSaleAndBillLines != null)
                            {
                                foreach (CumulSaleAndBillLine cumsbilne in lstCumulSaleAndBillLines)
                                {
                                    //IL FAUT FAIRE LE RETOUR EN STOCK DU FRAME

                                    if (cumsbilne.NumeroSerie != null)
                                    {
                                        ProductLocalization productLocalizationToUpdate = context.ProductLocalizations.FirstOrDefault(pl => pl.ProductID == cumsbilne.ProductID && pl.LocalizationID == cumsbilne.LocalizationID
                                        && pl.NumeroSerie == cumsbilne.NumeroSerie);
                                        if (productLocalizationToUpdate != null)
                                        {
                                            productLocalizationToUpdate.ProductLocalizationStockQuantity += cumsbilne.LineQuantity;

                                            //HISTORISATION DU STOCK
                                            DateTime date = OperationDate; // productLocalizationToUpdate.ProductLocalizationDate;
                                            DateTime actualDate = DateTime.Now;
                                            date = date.AddHours(actualDate.Hour);
                                            date = date.AddMinutes(actualDate.Minute);
                                            date = date.AddSeconds(actualDate.Second);
                                            InventoryHistoric inventoryHistoric = new InventoryHistoric
                                            {
                                                //les nouvelles infos
                                                NewSafetyStockQuantity = productLocalizationToUpdate.ProductLocalizationSafetyStockQuantity,
                                                NewStockQuantity = productLocalizationToUpdate.ProductLocalizationStockQuantity,
                                                NEwStockUnitPrice = productLocalizationToUpdate.ProductLocalizationStockSellingPrice,
                                                //les anciennes infos
                                                OldSafetyStockQuantity = productLocalizationToUpdate.ProductLocalizationSafetyStockQuantity,
                                                OldStockQuantity = productLocalizationToUpdate.ProductLocalizationStockQuantity - cumsbilne.LineQuantity,
                                                OldStockUnitPrice = productLocalizationToUpdate.ProductLocalizationStockSellingPrice,
                                                //Autres informations
                                                InventoryDate = date,
                                                inventoryReason = "Frame Return After delete Bill",
                                                LocalizationID = productLocalizationToUpdate.LocalizationID,
                                                ProductID = productLocalizationToUpdate.ProductID,
                                                RegisteredByID = UserConect,
                                                AutorizedByID = UserConect,
                                                CountByID = UserConect,
                                                StockStatus = "INPUT",
                                                Description = "Return Frame after delete bill ref "+ cumsbilne.NumeroSerie,
                                                Quantity = cumsbilne.LineQuantity,
                                                NumeroSerie = cumsbilne.NumeroSerie,
                                                Marque = cumsbilne.marque

                                            };
                                            context.InventoryHistorics.Add(inventoryHistoric);
                                            context.SaveChanges();
                                        }

                                    }
                                    // supression des lignes
                                    context.CumulSaleAndBillLines.Remove(cumsbilne);
                                    context.SaveChanges();
                                }
                                
                            }

                            context.CumulSaleAndBills.Remove(c);
                            context.SaveChanges();

                            

                        }

                    }

                   

                    //suppression des lignes de commande
                    //List<CustomerOrderLine> lstCustOrderLines = db.CustomerOrderLines.Where(c => c.CustomerOrderID == ID).ToList();
                    //if (lstCustOrderLines.Count>0)
                    //{
                    //    db.CustomerOrderLines.RemoveRange(lstCustOrderLines);
                    //    db.SaveChanges();
                    //}

                    customerOrdersToDelete = context.CustomerOrders.Find(ID);
                    customerOrdersToDelete.BillState = StatutFacture.Delete;
                    customerOrdersToDelete.DeleteReason = DeleteReason;
                    customerOrdersToDelete.DeleteBillDate = OperationDate;
                    customerOrdersToDelete.DeletedByID = UserConect;
                    customerOrdersToDelete.InsurreName = (customerOrdersToDelete.InsurreName == "" || customerOrdersToDelete.InsurreName == null) ? customerOrdersToDelete.CustomerName: customerOrdersToDelete.InsurreName;
                    context.SaveChanges();

                    //EcritureSneack
                    IMouchar opSneak = new MoucharRepository(context);
                    bool res = opSneak.InsertOperation(UserConect, "SUCCESS", "DELETE BILL-REFERENCE " + customerOrdersToDelete.CustomerOrderNumber + " FOR CUSTOMER " + customerOrdersToDelete.CustomerName, "DeleteValidatedBill", OperationDate, customerOrdersToDelete.BranchID);
                    if (!res)
                    {
                        throw new Exception("Une erreur s'est produite lors de la journalisation ");
                    }

                    ts.Complete();
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error : " + "e.Message = " + e.Message );
            }
            return customerOrdersToDelete;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="customerOrder"></param>
        /// <param name="heureVente"></param>
        /// <param name="UserConect"></param>
        /// <param name="spray"></param>
        /// <param name="boitier"></param>
        /// <param name="isDirectBill">Si cette variable est true, il faudra ajouter le Customer Value(ECO | VIP) a la commande</param>
        /// <returns></returns>
        public CustomerOrder SaveDirectChanges(CustomerOrder customerOrder,string heureVente, int UserConect, int spray, int boitier, bool isDirectBill = false)
        {
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {

                    customerOrder = PersistDirectCustomerOrder(customerOrder, heureVente, UserConect, spray, boitier);
                    if (isDirectBill == true)
                    {
                        // Mise a jour de la valeur(VIP | ECO) du client qui vient de faire cet achat
                        this.updateInsuredCustomerValue(customerOrder.CustomerOrderID, customerOrder.TotalPriceTTC);
                    }
                    ts.Complete();
                }
            }
            catch (Exception e)
            {
                throw new Exception("Une erreur s'est produite lors de la vente : " + "e.Message = " + e.Message + "e.InnerException = " + e.InnerException + "e.StackTrace = " + e.StackTrace);
            }
            return customerOrder;
        }


        // Mise a jour de la valeur(VIP | ECO) du client qui vient de faire cet achat
        public void updateInsuredCustomerValue(int customerOrderId, double TotalTTC) {
            
            CustomerOrder customerOrder = context.CustomerOrders.Find(customerOrderId);
            TotalTTC = customerOrder.Plafond + customerOrder.TotalMalade;

            /*int categoryId = customerOrder.CustomerOrderLines.FirstOrDefault().Product.CategoryID;
            LensCategory lc = context.LensCategories.Find(categoryId);*/

            CustomerValue customerValue = Customer.getCustomerValue(TotalTTC, null, true);
            // CustomerOrder lastOrder = context.CustomerOrders.Where(x => x.)

            customerOrder.CustomerValue = customerValue;
            customerOrder.LastCustomerValue = customerValue;

            context.SaveChanges();
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="customerOrder"></param>
        /// <param name="UserConect"></param>
        /// <param name="spray"></param>
        /// <param name="boitier"></param>
        /// <param name="heureVente"></param>
        /// <returns></returns>
        public CustomerOrder PersistDirectCustomerOrder(CustomerOrder customerOrder,string heureVente, int UserConect, int spray, int boitier)
        {

            bool res = false;

           
            //this is variable is saleID after determine type of payment method
            int customerOrderID = 0;
            customerOrder.OperatorID = UserConect;

            int MaxbarCode = 0;
            //generation du numero de recu
            if (context.CustomerOrders.Count() <= 0)
            {
                MaxbarCode = 0;
            }
            else
            {
                MaxbarCode = context.CustomerOrders.Where(b => b != null).Max(b => b.CompteurFacture);
            }

            //verification de la longueur du code de l'operation
            if (MaxbarCode == 0)
            {
                MaxbarCode = 0;
            }
            int newCompteur = MaxbarCode + 1;
            string Reference = (newCompteur.ToString().Length < 10) ? newCompteur.ToString().PadLeft(10, '0') : newCompteur.ToString();
            customerOrder.CompteurFacture = newCompteur;
            customerOrder.CustomerOrderNumber = Reference;
            

            CustomerOrder CustomerOrderToSave = new CustomerOrder();

            //ces 2 lignes qui suivent permettent de transformer un ProductModel en Product 
            //create a Map
            Mapper.CreateMap<CustomerOrder, CustomerOrder>();
            //use Map
            CustomerOrderToSave = Mapper.Map<CustomerOrder>(customerOrder);
            CustomerOrderToSave.CustomerOrderLines = null;

            if (CustomerOrderToSave.CustomerID == null || CustomerOrderToSave.CustomerID <= 0)
            {
                throw new Exception("Customer Id Cannot Be Null; Call IT for More Details");
            }
            CustomerOrderToSave = context.CustomerOrders.Add(CustomerOrderToSave);
            context.SaveChanges();
            customerOrderID = CustomerOrderToSave.CustomerOrderID;
            

            int ProductId = 0;
            //vente sans commande ou vente directe
            foreach (CustomerOrderLine customerOrderLine in customerOrder.CustomerOrderLines)
            {
                if (customerOrderLine.Product.ProductID <= 0)
                {
                    //persistance du nouvo verre
                    Lens ProductLens = this.PersistCustomerOrderLine(customerOrderLine);
                    ProductId = ProductLens.ProductID;
                }
                else
                {
                    ProductId = customerOrderLine.Product.ProductID;
                }

                CustomerOrderLine customerOrderToSave = new CustomerOrderLine()
                {
                    LocalizationID = customerOrderLine.LocalizationID,
                    ProductID = ProductId,
                    CustomerOrderID = customerOrderID,
                    LineQuantity = customerOrderLine.LineQuantity,
                    LineUnitPrice = customerOrderLine.LineUnitPrice,
                    OeilDroiteGauche = customerOrderLine.OeilDroiteGauche,
                    SpecialOrderLineCode = customerOrderLine.SpecialOrderLineCode,
                    marque = customerOrderLine.marque,
                    reference = customerOrderLine.reference,
                    FrameCategory = customerOrderLine.FrameCategory,
                    SupplyingName = customerOrderLine.SupplyingName,
                    NumeroSerie =customerOrderLine.NumeroSerie,
                    LensNumberSphericalValue = customerOrderLine.LensNumberSphericalValue,
                    LensNumberCylindricalValue = customerOrderLine.LensNumberCylindricalValue,
                    Axis = customerOrderLine.Axis,
                    Addition = customerOrderLine.Addition,
                    isCommandGlass=customerOrderLine.isCommandGlass
                };
                context.CustomerOrderLines.Add(customerOrderToSave);
                context.SaveChanges();


                context.SaveChanges();
            }
            if (CustomerOrderToSave.Plafond<=0)
            {
                throw new Exception("ERROR: Please enter customer plafond ");
            }
            //update facture
            this.UpdateCustomerOrder(CustomerOrderToSave, heureVente, CustomerOrderToSave.CustomerOrderLines.ToList(), UserConect, spray, boitier);

            //EcritureSneack
            IMouchar opSneak = new MoucharRepository(context);
            res = opSneak.InsertOperation(UserConect, "SUCCESS", "BILL-REFERENCE " + customerOrder.CustomerOrderNumber + " FOR CUSTOMER " + customerOrder.CustomerName, "PersistDirectCustomerOrder", customerOrder.CustomerOrderDate, customerOrder.BranchID);
            if (!res)
            {
                throw new Exception("Une erreur s'est produite lors de la journalisation ");
            }
            customerOrder.CustomerOrderID = customerOrderID;
            return customerOrder;
        }
        public string generateBill(int CpteurFacture)
        {
            int MaxbarCode = CpteurFacture;
            
            //verification de la longueur du code de l'operation
            if (MaxbarCode == 0)
            {
                MaxbarCode = 0;
            }
            int newCompteur = MaxbarCode + 1;
            string Reference = (newCompteur.ToString().Length < 5) ? newCompteur.ToString().PadLeft(5, '0') : newCompteur.ToString();
            return Reference;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="customerOrder"></param>
        /// <param name="UserConect"></param>
        /// <returns></returns>
        public CustomerOrder PersistCustomerOrder(CustomerOrder customerOrder, int UserConect)
        {

            bool res = false;
           
            //Customer customerEntity =  context.Customers.Find(customerOrder.AssureurID);

            //this is variable is saleID after determine type of payment method
            int customerOrderID = 0;
            customerOrder.OperatorID = UserConect;

            int MaxbarCode = 0;
            //generation du numero de recu
            if (context.CustomerOrders.Count() <= 0)
            {
                MaxbarCode = 0;
            }
            else
            {
                MaxbarCode = context.CustomerOrders.Where(b => b != null).Max(b => b.CompteurFacture);
            }

            //verification de la longueur du code de l'operation
            if (MaxbarCode == 0)
            {
                MaxbarCode = 0;
            }
            int newCompteur = MaxbarCode + 1;
            string Reference = (newCompteur.ToString().Length < 10) ? newCompteur.ToString().PadLeft(10, '0') : newCompteur.ToString();
            customerOrder.CompteurFacture = newCompteur;
            customerOrder.CustomerOrderNumber = Reference;


            CustomerOrder CustomerOrderToSave = new CustomerOrder();

            //ces 2 lignes qui suivent permettent de transformer un ProductModel en Product 
            //create a Map
            Mapper.CreateMap<CustomerOrder, CustomerOrder>();
            //use Map
            CustomerOrderToSave = Mapper.Map<CustomerOrder>(customerOrder);
            CustomerOrderToSave.CustomerOrderLines = null;
            if (CustomerOrderToSave.CustomerID == null || CustomerOrderToSave.CustomerID <= 0)
            {
                throw new Exception("Customer Id Cannot Be Null; Call IT for More Details");
            }
            CustomerOrderToSave = context.CustomerOrders.Add(CustomerOrderToSave);
            context.SaveChanges();
            customerOrderID = CustomerOrderToSave.CustomerOrderID;

            
            int ProductId = 0;
            //vente sans commande ou vente directe
            foreach (CustomerOrderLine customerOrderLine in customerOrder.CustomerOrderLines)
            {
                if (customerOrderLine.Product.ProductID <= 0)
                {
                    //persistance du nouvo verre
                    Lens ProductLens = this.PersistCustomerOrderLine(customerOrderLine);
                    ProductId = ProductLens.ProductID;
                }
                else
                {
                    ProductId = customerOrderLine.Product.ProductID;
                }

                CustomerOrderLine customerOrderToSave = new CustomerOrderLine()
                {
                    LocalizationID = customerOrderLine.LocalizationID,
                    ProductID = ProductId,
                    CustomerOrderID = customerOrderID,
                    LineQuantity = customerOrderLine.LineQuantity,
                    LineUnitPrice = customerOrderLine.LineUnitPrice,
                    OeilDroiteGauche = customerOrderLine.OeilDroiteGauche,
                    SpecialOrderLineCode=customerOrderLine.SpecialOrderLineCode,
                    marque=customerOrderLine.marque,
                    reference = customerOrderLine.reference,
                    FrameCategory = customerOrderLine.FrameCategory,
                    SupplyingName=customerOrderLine.SupplyingName,
                    NumeroSerie = customerOrderLine.NumeroSerie,
                    LensNumberSphericalValue = customerOrderLine.LensNumberSphericalValue,
                    LensNumberCylindricalValue = customerOrderLine.LensNumberCylindricalValue,
                    Axis = customerOrderLine.Axis,
                    Addition = customerOrderLine.Addition,
                    isCommandGlass = customerOrderLine.isCommandGlass,
                    IsVIPRoom = customerOrderLine.IsVIPRoom

                };
                context.CustomerOrderLines.Add(customerOrderToSave);

                context.SaveChanges();
            }

            //metre a jour le transaction number
            //int compteur = Convert.ToInt32(customerOrder.CustomerOrderNumber.Substring(12));
            //ITransactNumber trnNumber = new TransactNumberRepository(context);
            //res = trnNumber.saveTransactNumber("FPRO", compteur);
            //if (!res)
            //{
            //    throw new Exception("Une erreur s'est produite lors de la mise a jour du compteur du transact number ");
            //}
            
            //EcritureSneack
            IMouchar opSneak = new MoucharRepository(context);
            res = opSneak.InsertOperation(UserConect, "SUCCESS", "PROFORMA-REFERENCE " + customerOrder.CustomerOrderNumber + " FOR CUSTOMER " + customerOrder.CustomerName, "PersistCustomerOrder", customerOrder.CustomerOrderDate, customerOrder.BranchID);
            if (!res)
            {
                throw new Exception("Une erreur s'est produite lors de la journalisation ");
            }
            customerOrder.CustomerOrderID = customerOrderID;
            return customerOrder;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="customerOrder"></param>
        /// <param name="newsCustomerOrderLine"></param>
        /// <param name="UserConect"></param>
        /// <returns></returns>
        public CustomerOrder CreateCustomerOrder(CustomerOrder customerOrder, List<CustomerOrderLine> newsCustomerOrderLine, int UserConect)
        {
                try 
                {
                    using (TransactionScope ts = new TransactionScope())
                    {
                        ////we create a new command
                        //customerOrder.CustomerOrderDate = date;
                        context.CustomerOrders.Add(customerOrder);
                        foreach (CustomerOrderLine customerOrderLine in newsCustomerOrderLine)
                        {
                            CustomerOrderLine customerOrderLineToSave = new CustomerOrderLine()
                            {
                                LocalizationID = customerOrderLine.LocalizationID,
                                ProductID = customerOrderLine.ProductID,
                                CustomerOrderID = customerOrderLine.CustomerOrderID,
                                LineQuantity = customerOrderLine.LineQuantity,
                                LineUnitPrice = customerOrderLine.LineUnitPrice,
                                OeilDroiteGauche = customerOrderLine.OeilDroiteGauche,
                                NumeroSerie = customerOrderLine.NumeroSerie,
                                marque = customerOrderLine.marque,
                                reference = customerOrderLine.reference,
                                LensNumberSphericalValue = customerOrderLine.LensNumberSphericalValue,
                                LensNumberCylindricalValue = customerOrderLine.LensNumberCylindricalValue,
                                Axis = customerOrderLine.Axis,
                                Addition = customerOrderLine.Addition,
                                isCommandGlass = customerOrderLine.isCommandGlass
                            };
                            context.CustomerOrderLines.Add(customerOrderLineToSave);

                        }
                        //mise a jour du cpteur du transact number
                        TransactNumber trn = context.TransactNumbers.SingleOrDefault(t => t.TransactNumberCode == "SALE");
                        if (trn != null)
                        {
                            //persistance du compteur de l'objet TransactNumber
                            trn.Counter = trn.Counter + 1;
                        }
                        context.SaveChanges();
                        //we apply this modifications

                        //EcritureSneack
                        IMouchar opSneak = new MoucharRepository(context);
                        bool res1 = opSneak.InsertOperation(UserConect, "SUCCESS", "CREATE ORDER CUSTOMER " + customerOrder.CustomerName + "REFERENCE "+customerOrder.CustomerOrderNumber, "CreateCustomerOrder", customerOrder.CustomerOrderDate, customerOrder.BranchID);
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
                    
                    throw new Exception("Une erreur s'est produite lors de la vente : " + e.Message);
                }
                return customerOrder;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="customerOrderLine"></param>
        /// <returns></returns>
        public Lens PersistCustomerOrderLine(CustomerOrderLine customerOrderLine)
        {
            Lens currentProduct = (Lens)LensConstruction.GetOrderLensByCustOrdLine(customerOrderLine, this.context);
            currentProduct = LensConstruction.CreateLens((Lens)currentProduct, this.context);
            return currentProduct;
        }
        //1-Création du Produit s'il n'existe pas
        /// <summary>
        /// 
        /// </summary>
        /// <param name="customerOrder"></param>
        /// <param name="newsCustomerOrderLines"></param>
        /// <returns></returns>
        public CustomerOrder UpdateCustomerOrderLensOrder(CustomerOrder customerOrder, List<CustomerOrderLine> newsCustomerOrderLines)
        {
            
                try
                {
                    using (TransactionScope ts = new TransactionScope())
                    {
                        List<CustomerOrderLine> oldCustomerOrderLines = context.CustomerOrderLines
                                                                               .Where(ol => ol.CustomerOrderID == customerOrder.CustomerOrderID).ToList();
                       
                        //1-On supprime la commande
                        this.RemoveOrderLens(customerOrder.CustomerOrderID);
                        customerOrder.CustomerOrderID = 0;
                        //2-On recrée la commande
                        customerOrder = this.CreateOrderLensOrder(customerOrder, newsCustomerOrderLines);
                        
                        //we apply this modifications
                        context.SaveChanges();
                        //transaction.Commit();
                        ts.Complete();
                    }
                }
                catch (Exception e)
                {
                    //If an errors occurs, we cancel all changes in database
                    //transaction.Rollback();
                    throw new Exception("Une erreur s'est produite lors de la vente : " + e.Message);
                }
                return customerOrder;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="customerOrder"></param>
        public void ReturnPurchase(CustomerOrder customerOrder)
        {
            IPurchase pRepo = new PurchaseRepository(this.context);
            ISupplierReturn srRepo = new SupplierReturnRepository(this.context);
            //1-On retourne toute l'achat
            Purchase currentPurchase = this.context.Purchases.AsNoTracking().Single(p => p.PurchaseReference == customerOrder.CustomerOrderNumber);
            currentPurchase.PurchaseDate = customerOrder.CustomerOrderDate;
            srRepo.ReturnPurchase(currentPurchase);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="CustomerOrderID"></param>
        public void DeleteOrderLensOrder(int CustomerOrderID)
        {
           
                try
                {
                    using (TransactionScope ts = new TransactionScope())
                    {
                        RemoveOrderLens(CustomerOrderID);
                        ts.Complete();
                    }
                }
                catch (Exception e)
                {
                    //If an errors occurs, we cancel all changes in database
                    //transaction.Rollback();
                    throw new Exception("Une erreur s'est produite lors de la vente : " + e.Message);
                }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="CustomerOrderID"></param>
        public void RemoveOrderLens(int CustomerOrderID)
        {

            IRepositorySupply<CustomerOrderLine> _customerOrderLineRepository = new RepositorySupply<CustomerOrderLine>(this.context);

            CustomerOrder customerOrder = context.CustomerOrders.Find(CustomerOrderID);
            
            _customerOrderLineRepository.FindAll.OfType<CustomerOrderLine>().Where(ol => ol.CustomerOrderID == CustomerOrderID).ToList().ForEach(ol =>
            {
                _customerOrderLineRepository.Delete(ol.LineID);
            });
            this.Delete(CustomerOrderID);

            ISupplierOrder soRepo = new SupplierOrderRepository(context);
            SupplierOrder so = context.SupplierOrders.AsNoTracking().SingleOrDefault(so1 => so1.SupplierOrderReference == customerOrder.CustomerOrderNumber);
            soRepo.RemoveSupplierOrder(so.SupplierOrderID);

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="customerOrder"></param>
        /// <param name="newsCustomerOrderLines"></param>
        /// <returns></returns>
        public CustomerOrder CreateCustomerOrderLensOrder(CustomerOrder customerOrder, List<CustomerOrderLine> newsCustomerOrderLines)
        {
                try
                {
                    using (TransactionScope ts = new TransactionScope())
                    {
                        //CreatePurchaseForOrder(customerOrder, newsCustomerOrderLines);
                        CreateSupplierOrderForOrder(customerOrder, newsCustomerOrderLines);
                        customerOrder = CreateOrderLensOrder(customerOrder, newsCustomerOrderLines);
                        //transaction.Commit();
                        ts.Complete();
                    }
                }
                catch (Exception e)
                {
                    //If an errors occurs, we cancel all changes in database
                    //transaction.Rollback();
                    throw new Exception("Une erreur s'est produite lors de la vente : " + e.Message);
                }
                return customerOrder;
        }
/*
        private void CreatePurchaseForOrder(CustomerOrder customerOrder, List<CustomerOrderLine> newsCustomerOrderLines)
        {
            IPurchase purchaseRepository = new PurchaseRepository(this.context);
            purchaseRepository.SavePurchase(this.GetPurchaseFromOrder(customerOrder, newsCustomerOrderLines));

        }
*/
        private void CreateSupplierOrderForOrder(CustomerOrder customerOrder, List<CustomerOrderLine> newsCustomerOrderLines)
        {
            ISupplierOrder soRepo = new SupplierOrderRepository(this.context);
            soRepo.SaveSupplierOrder(this.GetSupplierOrderFromOrder(customerOrder, newsCustomerOrderLines));

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="customerOrder"></param>
        /// <param name="newsCustomerOrderLines"></param>
        /// <returns></returns>
        public Purchase GetPurchaseFromOrder(CustomerOrder customerOrder, List<CustomerOrderLine> newsCustomerOrderLines)
        {
            ITransactNumber _transactNumbeRepository = new TransactNumberRepository(this.context);
            List<PurchaseLine> purchaseLines = new List<PurchaseLine>();

            foreach (CustomerOrderLine col in newsCustomerOrderLines)
            {
                purchaseLines.Add(GetPurLineFromOrdLine(col));
            }

            Purchase res = new Purchase() 
            {
                BranchID = customerOrder.BranchID,
                DeviseID = customerOrder.DeviseID,
                Guaranteed = 7,
                PaymentDelay = 7,
                PurchaseBringerID = customerOrder.OperatorID,
                PurchaseDate = customerOrder.CustomerOrderDate,
                PurchaseDeliveryDate = customerOrder.CustomerOrderDate,
                PurchaseLines = purchaseLines,
                PurchaseReference = customerOrder.CustomerOrderNumber,
                PurchaseRegisterID = customerOrder.OperatorID,
                PurchaseValidate = true,
                StatutPurchase = SalePurchaseStatut.Received,
                SupplierID = this.context.Suppliers.SingleOrDefault().GlobalPersonID,
                TotalPriceHT = customerOrder.TotalPriceHT,
                TotalPriceTTC = customerOrder.TotalPriceTTC,
                Transport = customerOrder.Transport,
                TVAAmount = customerOrder.TVAAmount,
                VatRate = customerOrder.VatRate,
            };

            return res;
        }

        public SupplierOrder GetSupplierOrderFromOrder(CustomerOrder customerOrder, List<CustomerOrderLine> newsCustomerOrderLines)
        {
            ITransactNumber _transactNumbeRepository = new TransactNumberRepository(this.context);
            List<SupplierOrderLine> supplierOrderLines = new List<SupplierOrderLine>();

            foreach (CustomerOrderLine col in newsCustomerOrderLines)
            {
                supplierOrderLines.Add(GetSupOrdLineFromOrdLine(col));
            }

            SupplierOrder res = new SupplierOrder()
            {
                BranchID = customerOrder.BranchID,
                DeviseID = customerOrder.DeviseID,
                IsDelivered = false,
                RateDiscount = customerOrder.RateDiscount,
                RateReduction = customerOrder.RateDiscount,
                SupplierID = context.Suppliers.FirstOrDefault().GlobalPersonID,
                SupplierOrderDate = customerOrder.CustomerOrderDate,
                SupplierOrderLines = supplierOrderLines,
                SupplierOrderReference = customerOrder.CustomerOrderNumber,
                SupplierOrderTotalAmount = customerOrder.TotalPriceTTC,
                SupplierOrderTotalPrice = customerOrder.TotalPriceTTC,
                Transport = customerOrder.Transport,
                VatRate = customerOrder.VatRate,
            };

            return res;
        }

        public PurchaseLine GetPurLineFromOrdLine(CustomerOrderLine custOrdLine)
        {
            PurchaseLine res = new PurchaseLine();

            //Création du produit en cours de commande : 

            OrderLens currentProduct = (OrderLens) LensConstruction.GetOrderLensByCustOrderLine(custOrdLine, this.context);

            //1 - Création du numéro de Verre
            LensNumber currentLensNumber = LensConstruction.GetLensNumber(currentProduct.LensNumber, this.context);
            currentProduct.LensNumberID = currentLensNumber.LensNumberID;
            currentProduct.LensNumber = null;

            //2 - Numéro de compte du produit
            IAccount accountRepo = new AccountRepository(context);
            CollectifAccount colAccount = context.CollectifAccounts.SingleOrDefault(cac => cac.CollectifAccountLabel == "Warehouse stocks");
            currentProduct.AccountID = accountRepo.GenerateAccountNumber(colAccount.CollectifAccountID, currentProduct.ProductCode, false).AccountID;

            //3 - Création du produit
            currentProduct = CreateOrderLens(currentProduct);
            custOrdLine.ProductID = currentProduct.ProductID;

            //ces 2 lignes qui suivent permettent de transformer un CustomerOrderLine en PurchaseLine 
            //create a Map
            Mapper.CreateMap<CustomerOrderLine, PurchaseLine>();
            //use Map
            res = Mapper.Map<PurchaseLine>(custOrdLine);
            res.LineUnitPrice = custOrdLine.PurchaseLineUnitPrice;

            return res;

        }

        public SupplierOrderLine GetSupOrdLineFromOrdLine(CustomerOrderLine custOrdLine)
        {
            SupplierOrderLine res = new SupplierOrderLine();

            //Création du produit en cours de commande : 
            OrderLens currentProduct = (OrderLens) LensConstruction.GetOrderLensByCustOrderLine(custOrdLine, this.context);

            //3 - Création du produit
            currentProduct = CreateOrderLens(currentProduct);
            custOrdLine.ProductID = currentProduct.ProductID;

            //ces 2 lignes qui suivent permettent de transformer un CustomerOrderLine en SupplierOrderLine 
            //create a Map
            Mapper.CreateMap<CustomerOrderLine, SupplierOrderLine>();
            //use Map
            res = Mapper.Map<SupplierOrderLine>(custOrdLine);
            res.LineUnitPrice = custOrdLine.PurchaseLineUnitPrice;

            return res;

        }

        public CustomerOrder CreateOrderLensOrder(CustomerOrder customerOrder, List<CustomerOrderLine> newsCustomerOrderLines)
        {
            //we create a new command
            customerOrder.Operator = null;
            context.CustomerOrders.Add(customerOrder);
            context.SaveChanges();
            foreach (CustomerOrderLine customerOrderLine in newsCustomerOrderLines)
            {
                PersistCustomerOrderLine(customerOrderLine, customerOrder);
            }
            //mise a jour du cpteur du transact number
            TransactNumber trn = context.TransactNumbers.SingleOrDefault(t => t.TransactNumberCode == "SALE");
            if (trn != null)
            {
                //persistance du compteur de l'objet TransactNumber
                trn.Counter = trn.Counter + 1;
            }
            context.SaveChanges();

            return customerOrder;
        }

        public OrderLens CreateOrderLens(OrderLens currentProduct)
        {
            OrderLens product = context.OrderLenses.FirstOrDefault(pdt => pdt.ProductCode == currentProduct.ProductCode);

            if (product != null && product.ProductID > 0)
            {
                return product;
            }

            //1 - Création du numéro de Verre
            LensNumber currentLensNumber = LensConstruction.GetLensNumber(currentProduct.LensNumber, this.context);
            currentProduct.LensNumberID = currentLensNumber.LensNumberID;
            currentProduct.LensNumber = null;

             //2-Création de la catégorie du produit
            if (currentProduct.LensCategoryID <= 0)
            {
                LensCategory lensCategory = LensConstruction.PersistLensCategory(currentProduct.LensCategory.CategoryCode, context);
                currentProduct.LensCategoryID = lensCategory.CategoryID;
                currentProduct.CategoryID = lensCategory.CategoryID;
                currentProduct.LensCategory = lensCategory;
                currentProduct.Category = lensCategory;
            }

            //3 - Numéro de compte du produit
            
            CollectifAccount colAccount = currentProduct.LensCategory.CollectifAccount;
            Account Acct = (from a in context.Accounts
                            where a.CollectifAccountID == colAccount.CollectifAccountID
                            select a).FirstOrDefault();

            if (Acct == null)
            {
                IAccount accountRepo = new AccountRepository(context);
                currentProduct.AccountID = accountRepo.GenerateAccountNumber(colAccount.CollectifAccountID, currentProduct.Category.CategoryCode, false).AccountID;
            }
            else
            {
                currentProduct.AccountID = Acct.AccountID;
            }

            currentProduct.Category = null;
            currentProduct.LensCategory = null;
            currentProduct = context.OrderLenses.Add(currentProduct);
            context.SaveChanges();

            return currentProduct;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="customerOrderLine"></param>
        /// <param name="customerOrder"></param>
        public void PersistCustomerOrderLine(CustomerOrderLine customerOrderLine, CustomerOrder customerOrder)
        {
            OrderLens currentProduct = (OrderLens)LensConstruction.GetOrderLensByCustOrderLine(customerOrderLine, this.context);
            currentProduct = context.OrderLenses.SingleOrDefault(pdt => pdt.ProductCode == currentProduct.ProductCode);

            CustomerOrderLine customerOrderLineToSave = new CustomerOrderLine();
            //ces 2 lignes qui suivent permettent de transformer un CustomerOrderLine en CustomerOrderLine 
            //create a Map
            Mapper.CreateMap<CustomerOrderLine, CustomerOrderLine>();
            //use Map
            customerOrderLineToSave = Mapper.Map<CustomerOrderLine>(customerOrderLine);
            customerOrderLineToSave.Product = null;
            customerOrderLineToSave.ProductID = currentProduct.ProductID;
            customerOrderLineToSave.CustomerOrderID = customerOrder.CustomerOrderID;
            customerOrderLineToSave.LineID = 0;
            customerOrderLineToSave.Localization = null;
            customerOrderLineToSave.CustomerOrder = null;

            context.CustomerOrderLines.Add(customerOrderLineToSave);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="customerOrderLine"></param>
        /// <param name="customerOrder"></param>
        public void UpdateCustomerOrderLine(CustomerOrderLine customerOrderLine, CustomerOrder customerOrder)
        {
            IRepositorySupply<CustomerOrderLine> custordRepoLine = new RepositorySupply<CustomerOrderLine>(context);

            IAccount accountRepo = new AccountRepository(context);
            OrderLens currentProduct = (OrderLens)LensConstruction.GetOrderLensByCustOrderLine(customerOrderLine, this.context);
            //Création du numéro de Verre
            LensNumber currentLensNumber = LensConstruction.GetLensNumber(currentProduct.LensNumber, this.context);

            //Numéro du verre
            currentProduct.LensNumberID = currentLensNumber.LensNumberID;
            currentProduct.LensNumber = null;

            currentProduct = CreateOrderLens(currentProduct);

            CustomerOrderLine customerOrderLineToSave = new CustomerOrderLine();
            //ces 2 lignes qui suivent permettent de transformer un ProductModel en Product 
            //create a Map
            Mapper.CreateMap<Purchase, BankPurchase>();
            //use Map
            customerOrderLineToSave = Mapper.Map<CustomerOrderLine>(customerOrderLine);
            customerOrderLineToSave.Product = null;
            customerOrderLineToSave.ProductID = currentProduct.ProductID;
            customerOrderLineToSave.CustomerOrderID = customerOrder.CustomerOrderID;
            customerOrderLineToSave.Localization = null;

            custordRepoLine.Update(customerOrderLineToSave, customerOrderLineToSave.LineID);

        }

    }
}

