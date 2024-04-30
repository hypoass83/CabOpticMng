

using FatSod.Supply.Abstracts;
using FatSod.Supply.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FastSod.Utilities.Util;
using AutoMapper;
using FatSod.DataContext.Concrete;
using System.Transactions;
using FatSod.Security.Abstracts;
using System.Data.Entity;
using FatSod.Ressources;
using FatSod.DataContext.Initializer;
using FatSod.Security.Entities;
using FatSod.Report.WrapReports;

namespace FatSod.DataContext.Repositories
{
    public class SaleRepository : RepositorySupply<Sale>, ISale
    {
        /// <summary>
        /// 
        /// </summary>
        public SaleRepository()
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctx"></param>
        public SaleRepository(EFDbContext ctx)
            : base(ctx)
        {
            this.context = ctx;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="SaleID"></param>
        /// <param name="SessionGlobalPersonID"></param>
        /// <param name="BDDateOperation"></param>
        /// <param name="BranchID"></param>
        
        /// <returns></returns>
        public bool DeleteStockInsureReserve(int SaleID,  int SessionGlobalPersonID, DateTime BDDateOperation, int BranchID)
        {
            bool res = false;
            try
            {

                
                using (TransactionScope ts = new TransactionScope())
                {
                    //recuperation de la ligne a modifier
                    Sale updatedSale = context.Sales.Find(SaleID);
                    if (updatedSale != null)
                    {
                        updatedSale.IsPaid = true;
                        updatedSale.SaleDeliver = true;
                        context.SaveChanges();

                        //payement total de la facture
                        IDeposit depositRepo = new DepositRepository(context);
                        ISavingAccount _savingAccountRepository = new SavingAccountRepository(context);

                        //Reste à payé sur la vente en considérant le retour courant 
                        double RemaindAmount = depositRepo.SaleRemainder(updatedSale);

                        // we will use the saving account to paid this amount
                        SavingAccount sa = context.SavingAccounts.SingleOrDefault(sa1 => sa1.CustomerID == updatedSale.CustomerID.Value);
                        PaymentMethod paymentMethod = sa;
                        if (paymentMethod == null || paymentMethod.ID == 0)
                        {
                            paymentMethod = _savingAccountRepository.CreateSavingAccount(updatedSale.CustomerID.Value, updatedSale.BranchID);
                        }
                        //We save slice of this customer with the sale's slice amount price
                        if (paymentMethod is SavingAccount)
                        {
                            CustomerSlice customerSlice = new CustomerSlice()
                            {
                                DeviseID = updatedSale.DeviseID,
                                PaymentMethodID = paymentMethod.ID,
                                SaleID = SaleID,
                                SliceAmount = RemaindAmount,
                                SliceDate = updatedSale.SaleDate,
                                Representant = updatedSale.CustomerName ,
                                Reference = updatedSale.SaleReceiptNumber,
                                isDeposit = false,
                                OperatorID = SessionGlobalPersonID
                            };
                            context.CustomerSlices.Add(customerSlice);
                            context.SaveChanges();
                        }
                        else
                        {
                            throw new Exception("Wrong Payment Method !!! Contact your Software Provider ");
                        }

                        //EcritureSneack
                        IMouchar opSneak = new MoucharRepository(context);
                        res = opSneak.InsertOperation(SessionGlobalPersonID, "SUCCESS", "Delete ID " + SaleID + " FOR CUSTOMER " + updatedSale.CustomerName, "DeleteExtractSMS", BDDateOperation, BranchID);
                        if (!res)
                        {
                            throw new Exception("Une erreur s'est produite lors de la journalisation ");
                        }
                    }
                    else
                    {
                        res = false;
                        throw new Exception("No data to Delete!!!");
                    }

                    res = true;
                    ts.Complete();

                    return res;
                }
            }
            catch (Exception e)
            {
                res = false;
                throw new Exception(e.Message);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sale"></param>
        /// <param name="UserConect"></param>
        /// <returns></returns>
        //methode permetant de valider les vente coe special order
        public CumulSaleAndBill ValidePostToSpecialOrder(CumulSaleAndBill sale, int UserConect)
        {
            try
            {
                IPerson _personRepository = new PersonRepository();
                IAccount _accountRepository = new AccountRepository();

                using (TransactionScope ts = new TransactionScope())
                {
                    //update du phone number
                    Customer customerEntity = new Customer();
                    if (sale.CustomerID == null)
                    {
                        //fabrication du nvo cpte
                        CollectifAccount colAcct = context.CollectifAccounts.Where(c => c.AccountingSection.AccountingSectionCode == CodeValue.Accounting.DefaultCodeAccountingSection.CODECLIENT).FirstOrDefault();
                        int AccountID = _accountRepository.GenerateAccountNumber(colAcct.CollectifAccountID, sale.CustomerName + " " + Resources.UIAccount, false).AccountID;
                        Sex sexe = context.Sexes.FirstOrDefault();
                        Adress adresse = context.Adresses.FirstOrDefault();
                        Adress NewAdress = new Adress()
                        {
                            AdressPhoneNumber = sale.Remarque,
                            AdressCellNumber = sale.Remarque,
                            AdressFullName = adresse.AdressFullName,
                            AdressEmail = adresse.AdressEmail,
                            AdressWebSite = adresse.AdressWebSite,
                            AdressPOBox = adresse.AdressPOBox,
                            AdressFax = adresse.AdressFax,
                            QuarterID = adresse.QuarterID
                        };
                        Customer customer = new Customer()
                        {
                            AccountID = AccountID,
                            Name = sale.CustomerName,
                            SexID = sexe.SexID,
                            CNI = sale.CustomerName,
                            Adress = NewAdress,
                            Dateregister = sale.SaleDate,
                            IsBillCustomer = true,
                        };

                        customerEntity = (Customer)_personRepository.Create2(customer, UserConect, sale.SaleDate, sale.BranchID);

                    }
                    else
                    {
                        customerEntity = context.Customers.Find(sale.CustomerID.Value);
                        //update de l'adresse du client
                        customerEntity.Adress.AdressPhoneNumber = sale.Remarque;
                        context.SaveChanges();

                    }
                    if (customerEntity == null)
                    {
                        throw new Exception("Erreur : Please you must create the Customer Before Proceed ");
                    }

                    //update du purchase price

                    List<CumulSaleAndBillLine> newCumSL = context.CumulSaleAndBillLines.Where(c => c.CumulSaleAndBillID == sale.CumulSaleAndBillID).ToList();
                    foreach (CumulSaleAndBillLine cumSL in newCumSL)
                    {
                        if (cumSL.Product is OrderLens)
                        {
                            cumSL.PurchaseLineUnitPrice = sale.CumulSaleAndBillLines.FirstOrDefault().PurchaseLineUnitPrice;
                            cumSL.ProductCategoryID = sale.CumulSaleAndBillLines.FirstOrDefault().ProductCategoryID;

                            if ((cumSL.LensNumberSphericalValue == null || cumSL.LensNumberSphericalValue == "") && (cumSL.LensNumberCylindricalValue == null || cumSL.LensNumberCylindricalValue == "") && (cumSL.Addition == null || cumSL.Addition == ""))
                            {

                                OrderLens lensProduct = context.OrderLenses.Find(cumSL.Product.ProductID);
                                cumSL.Axis = lensProduct.Axis;
                                cumSL.Addition = lensProduct.Addition;
                                cumSL.LensNumberCylindricalValue = lensProduct.LensNumberCylindricalValue;
                                cumSL.LensNumberSphericalValue = ((lensProduct.LensNumberSphericalValue == null || lensProduct.LensNumberSphericalValue == "") && (lensProduct.LensNumberCylindricalValue == null || lensProduct.LensNumberCylindricalValue == "")) ? "0.00" : lensProduct.LensNumberSphericalValue;
                            }

                            context.SaveChanges();
                        }

                    }
                    //this.Update()
                    //newCumSL.PurchaseLineUnitPrice = cumSL.PurchaseLineUnitPrice;
                    //context.SaveChanges();


                    CumulSaleAndBill NewSale = context.CumulSaleAndBills.Find(sale.CumulSaleAndBillID);
                    if (NewSale == null)
                    {
                        throw new Exception("Erreur : This sale does not exit!!! please contact your administrator ");
                    }
                    NewSale.CustomerID = customerEntity.GlobalPersonID;
                    NewSale.IsRendezVous = true;
                    NewSale.PostSOByID = UserConect;
                    NewSale.AwaitingDay = sale.AwaitingDay;
                    NewSale.Remarque = sale.Remarque;
                    //NewSale.CumulSaleAndBillLines = sale.CumulSaleAndBillLines;
                    context.SaveChanges();


                    ts.Complete();
                }
            }
            catch (Exception e)
            {
                IMouchar opSneak = new MoucharRepository(context);
                bool res = opSneak.InsertOperation(UserConect, "ERROR", "SALE-REFERENCE " + sale.SaleReceiptNumber + " FOR CUSTOMER " + sale.CustomerName, "ValidePostToSpecialOrder", sale.SaleDate, sale.BranchID);
                if (!res)
                {
                    throw new Exception("Une erreur s'est produite lors de la journalisation ");
                }
                throw new Exception("Une erreur s'est produite lors de la validation : " + "e.Message = " + e.Message);
            }
            return sale;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sale"></param>
        /// <param name="UserConect"></param>
        /// <returns></returns>

        //methode permetant de valider les vente coe special order
        public CumulSaleAndBill ValideReceiveSpecialOrder(CumulSaleAndBill sale, int UserConect)
        {
            try
            {
                IProductLocalization _PLRepository = new PLRepository(context);
                // IAccount _accountRepository = new AccountRepository();

                using (TransactionScope ts = new TransactionScope())
                {

                    CumulSaleAndBill NewSale = context.CumulSaleAndBills.Find(sale.CumulSaleAndBillID);
                    if (NewSale == null)
                    {
                        throw new Exception("Erreur : This sale does not exit!!! please contact your administrator ");
                    }
                    NewSale.IsProductReveive = true;
                    NewSale.ReceiveSOByID = UserConect;
                    NewSale.EffectiveReceiveDate = sale.EffectiveReceiveDate;
                    context.SaveChanges();

                    //entre du verre de commande en stock

                    int ProductId = 0;
                    //bool isFrame = false;
                    //vente sans commande ou vente directe
                    foreach (CumulSaleAndBillLine saleLine in NewSale.CumulSaleAndBillLines)
                    {

                        ProductId = saleLine.ProductID;
                        if (saleLine.Product is Lens || saleLine.Product is OrderLens)
                        {
                            ProductLocalization productInStock = new ProductLocalization();
                            //ecriture du stock
                            productInStock = context.ProductLocalizations.FirstOrDefault(pl => pl.ProductID == ProductId && pl.LocalizationID == saleLine.LocalizationID);
                            if (productInStock == null)
                            {
                                //isNew = true;
                                ProductLocalization newPL = new ProductLocalization();
                                newPL.ProductLocalizationStockQuantity = 0d;
                                newPL.AutorizedByID = UserConect;
                                newPL.ProductLocalizationSafetyStockQuantity = 0d;
                                newPL.ProductLocalizationStockSellingPrice = 0d;
                                newPL.AveragePurchasePrice = saleLine.PurchaseLineUnitPrice;
                                newPL.ProductLocalizationDate = sale.EffectiveReceiveDate.Value;
                                newPL.ProductID = ProductId;
                                newPL.LocalizationID = saleLine.LocalizationID;
                                newPL.isDeliver = false;
                                newPL.NumeroSerie = saleLine.NumeroSerie;
                                newPL.Marque = saleLine.marque;

                                context.ProductLocalizations.Add(newPL);
                                context.SaveChanges();
                                productInStock = newPL;

                            }

                            //entre du produit en stock 
                            _PLRepository.StockInput(productInStock, saleLine.LineQuantity, saleLine.PurchaseLineUnitPrice, sale.EffectiveReceiveDate.Value, UserConect);
                        }

                        context.SaveChanges();
                    }


                    ts.Complete();
                }
            }
            catch (Exception e)
            {
                IMouchar opSneak = new MoucharRepository(context);
                bool res = opSneak.InsertOperation(UserConect, "ERROR", "SALE-REFERENCE " + sale.SaleReceiptNumber + " FOR CUSTOMER " + sale.CustomerName, "ValideReceiveSpecialOrder", sale.SaleDate, sale.BranchID);
                if (!res)
                {
                    throw new Exception("Une erreur s'est produite lors de la journalisation ");
                }
                throw new Exception("Une erreur s'est produite lors de la validation : " + "e.Message = " + e.Message);
            }
            return sale;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sale"></param>
        /// <param name="UserConect"></param>
        /// <returns></returns>
        //methode permetant de valider les livraison client des special order
        public CumulSaleAndBill ValideDeliverSpecialOrder(CumulSaleAndBill sale, int UserConect)
        {
            try
            {
                //IPerson _personRepository = new PersonRepository();
                //IAccount _accountRepository = new AccountRepository();

                //using (TransactionScope ts = new TransactionScope())
                //{

                CumulSaleAndBill NewSale = context.CumulSaleAndBills.Find(sale.CumulSaleAndBillID);
                if (NewSale == null)
                {
                    throw new Exception("Erreur : This sale does not exit!!! please contact your administrator ");
                }
                NewSale.IsCustomerRceive = true;
                NewSale.CustomerDeliverDate = sale.DeliverDate.Value;

                NewSale.IsDeliver = true;
                NewSale.DeliverByID = UserConect;
                NewSale.DateOperationHours = sale.DateOperationHours;
                NewSale.DeliverDate = sale.DeliverDate.Value;

                context.SaveChanges();
                //remove this cutomer under sms list
                ExtractSMS checkextract = context.ExtractSMSs.Where(c => c.CustomerID == sale.CustomerID.Value && c.AlertDescrip == "SPECIALORDER" && c.isSmsSent == false).FirstOrDefault();
                if (checkextract != null)
                {
                    checkextract.isDelete = true;
                    context.SaveChanges();
                }

                //    ts.Complete();
                //}
            }
            catch (Exception e)
            {
                IMouchar opSneak = new MoucharRepository(context);
                bool res = opSneak.InsertOperation(UserConect, "ERROR", "SALE-REFERENCE " + sale.SaleReceiptNumber + " FOR CUSTOMER " + sale.CustomerName, "ValideReceiveSpecialOrder", sale.SaleDate, sale.BranchID);
                if (!res)
                {
                    throw new Exception("Une erreur s'est produite lors de la journalisation ");
                }
                throw new Exception("Une erreur s'est produite lors de la validation : " + "e.Message = " + e.Message);
            }
            return sale;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sale"></param>
        /// <param name="HeureVente"></param>
        /// <param name="UserConect"></param>
        /// <param name="isCommand"></param>
        /// <param name="isOtherSale"></param>
        /// <returns></returns>
        public Sale SaveChanges(Sale sale, String HeureVente, int UserConect, bool isCommand, bool isOtherSale)
        {
            //Begin of transaction

            try
            {
                using (TransactionScope ts = new TransactionScope())
                {

                    if (sale.AuthoriseSaleID.HasValue && sale.AuthoriseSaleID.Value > 0)
                    {
                        AuthoriseSale authoriseSale = context.AuthoriseSales.SingleOrDefault(c => c.AuthoriseSaleID == sale.AuthoriseSaleID);
                        sale.GestionnaireID = authoriseSale.GestionnaireID;
                        sale.IsUrgent = authoriseSale.IsUrgent;
                    }

                    //ajout de lheure de la vente
                    string[] tisys = HeureVente.Split(new char[] { ':' });
                    DateTime date = sale.SaleDate;
                    date = date.AddHours(Convert.ToDouble(tisys[0]));
                    date = date.AddMinutes(Convert.ToDouble(tisys[1]));
                    date = date.AddSeconds(Convert.ToDouble(tisys[2]));
                    //we create a new command
                    sale.SaleDateHours = date;

                    sale = PersistSale(sale, UserConect, isCommand, isOtherSale);
                    this.updateCashCustomerValue(sale.SaleID, sale.TotalPriceTTC);
                    UpdateNoPurchase(sale);
                    //transaction.Commit();
                    ts.Complete();
                }
            }
            catch (Exception e)
            {
                //If an errors occurs, we cancel all changes in database
                //transaction.Rollback();
                //EcritureSneack
                IMouchar opSneak = new MoucharRepository(context);
                bool res = opSneak.InsertOperation(UserConect, "ERROR", "SALE-REFERENCE " + sale.SaleReceiptNumber + " FOR CUSTOMER " + sale.PoliceAssurance, "PersistSale", sale.SaleDate, sale.BranchID);
                if (!res)
                {
                    throw new Exception("Une erreur s'est produite lors de la journalisation ", e);
                }
                throw new Exception("Une erreur s'est produite lors de la vente : ", e);
            }
            return sale;
        }

        private void UpdateNoPurchase(Sale sale)
        {

            sale = context.Sales.SingleOrDefault(s => s.SaleID == sale.SaleID);

            if (sale.AuthoriseSaleFK?.ConsultDilPresc == null)
                return;

            if (sale.isDilation)
                return;

            var noPurchase = context.NoPurchases.SingleOrDefault(
                np => np.ConsultLensPrescription.ConsultDilPrescID == sale.AuthoriseSaleFK.ConsultDilPrescID/* ||
                        np.ConsultDilatationId == sale.ConsultDilPresc.*/);

            if (noPurchase != null)
                noPurchase.HasBeenPurchased = true;

            context.SaveChanges();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sale"></param>
        /// <param name="UserConect"></param>
        /// <param name="isCommand"></param>
        /// <param name="isOtherSale"></param>
        /// <returns></returns>
        private Sale PersistSale(Sale sale, int UserConect, bool isCommand,bool isOtherSale)
        {

            bool res = false;
            double saleAmount = 0;
            double TrancheAmount = 0d;

            int cumulSaleAndBillID = 0;
            //double OldSafetyStockQuantity = 0d;
            //double OldStockQuantity = 0d;
            //double OldStockUnitPrice = 0d;



            

            PaymentMethod paymentMethod = context.PaymentMethods.Find(sale.PaymentMethodID);


            //: context.GlobalPeople.Where(c => c.Name == "Default").FirstOrDefault().GlobalPersonID
            //Customer customerEntity = (sale.CustomerID==null) ? context.Customers.Where(c=>c.Name.ToLower()=="default").FirstOrDefault() : context.Customers.Find(sale.CustomerID.Value);
            

            Customer customerEntity = new Customer();
            if (sale.CustomerID == null)
            {
                IPerson _personRepository = new PersonRepository();
                IAccount _accountRepository = new AccountRepository();
                ITransactNumber _transactRepository = new TransactNumberRepository();

                //fabrication du nvo cpte
                CollectifAccount colAcct = context.CollectifAccounts.Where(c => c.AccountingSection.AccountingSectionCode == CodeValue.Accounting.DefaultCodeAccountingSection.CODECLIENT).FirstOrDefault();
                int AccountID = _accountRepository.GenerateAccountNumber(colAcct.CollectifAccountID, sale.CustomerName + " " + Resources.UIAccount, false).AccountID;
                Sex sexe = context.Sexes.FirstOrDefault();
                Adress adresse = context.Adresses.FirstOrDefault();
                string Customernumber = _transactRepository.GenerateUniqueCIN();

                Adress NewAdress = new Adress()
                {
                    AdressPhoneNumber = sale.Remarque,
                    AdressCellNumber = sale.Remarque,
                    AdressFullName = adresse.AdressFullName,
                    AdressEmail = adresse.AdressEmail,
                    AdressWebSite = adresse.AdressWebSite,
                    AdressPOBox = adresse.AdressPOBox,
                    AdressFax = adresse.AdressFax,
                    QuarterID = adresse.QuarterID
                };
                Customer customer = new Customer()
                {
                    AccountID = AccountID,
                    Name = sale.CustomerName,
                    SexID = sexe.SexID,
                    CNI = sale.CNI,
                    Adress = NewAdress,
                    Dateregister = sale.SaleDate,
                    IsBillCustomer = false,
                    CustomerNumber= Customernumber
                };

                customerEntity = (Customer)_personRepository.Create2(customer, UserConect, sale.SaleDate, sale.BranchID);

            }
            else
            {
                customerEntity = context.Customers.Find(sale.CustomerID.Value);
                //update de l'adresse du client
                // Correction de Bug -
                // 1- On ne modifie pas le numero de telephone du client lors de la validation d'une dilatation
                // 2- On ne modifie pas le numero de telephone du client lors de la validation d'un Other Sale
                // 3- On ne modifie pas le numero de telephone du client lors de la Validation d'un Cash Sale
                if (!sale.isDilation && !sale.IsOtherSale && !sale.IsValidatedSale)
                {
                    customerEntity.Adress.AdressPhoneNumber = sale.Remarque;
                    customerEntity.Adress.AdressCellNumber = sale.Remarque;
                }
                context.SaveChanges();

            }
            if (customerEntity == null)
            {
                throw new Exception("Erreur : Please you must create the Customer Before Proceed ");
            }
            //assurons ns qu'une reference pareil n'existe pas deja
            if (sale.SaleReceiptNumber != null || sale.SaleReceiptNumber.Trim() == "")
            {
                Sale OldSale = context.Sales.Where(s => s.SaleReceiptNumber == sale.SaleReceiptNumber.Trim()).FirstOrDefault();
                if (OldSale != null)
                {
                    throw new Exception("Erreur : This reference number (" + sale.SaleReceiptNumber + ") already exit in the database. please contact your administrator ");
                }
            }
            sale.CustomerName = (sale.isDilation) ? sale.CustomerName + "(DILATATION)" : sale.CustomerName;
            sale.CNI = customerEntity.CNI;

            //this is variable is saleID after determine type of payment method
            int saleID = 0;
            sale.OperatorID = UserConect;
            sale.PostByID = (!sale.PostByID.HasValue) ? UserConect : sale.PostByID.Value;
            int MaxbarCode = 0;
            //generation du numero de recu
            if (context.Sales.Count() <= 0)
            {
                MaxbarCode = 0;
            }
            else
            {
                MaxbarCode = context.Sales.Where(b => b != null).Max(b => b.CompteurFacture);
            }

            //verification de la longueur du code de l'operation
            if (MaxbarCode == 0)
            {
                MaxbarCode = 0;
            }
            int newCompteur = MaxbarCode + 1;
            string Reference = (newCompteur.ToString().Length < 6) ? newCompteur.ToString().PadLeft(6, '0') : newCompteur.ToString();
            sale.SaleReceiptNumber = "VPT " + Reference;
            sale.CompteurFacture = newCompteur;
            sale.CustomerID = customerEntity.GlobalPersonID;

            sale.OldStatutSale = SalePurchaseStatut.Ordered;

            if (paymentMethod != null)
            {

                if (sale.CustomerSlice.SliceAmount >= sale.TotalPriceTTC)
                {
                    sale.StatutSale = SalePurchaseStatut.Paid;
                }
                else if (sale.CustomerSlice.SliceAmount < sale.TotalPriceTTC && sale.CustomerSlice.SliceAmount > 0)
                {//
                    sale.StatutSale = SalePurchaseStatut.Advanced;
                }
                else
                {
                    sale.StatutSale = SalePurchaseStatut.Delivered;
                }
                //Si c'est une vente réglée par le compte d'épargne
                if (paymentMethod is SavingAccount)
                {

                    SavingAccountSale saSale = new SavingAccountSale();

                    //ces 2 lignes qui suivent permettent de transformer un ProductModel en Product 
                    //create a Map
                    Mapper.CreateMap<Sale, SavingAccountSale>();
                    //use Map
                    saSale = Mapper.Map<SavingAccountSale>(sale);
                    saSale.SavingAccountID = paymentMethod.ID;
                    saSale.SaleLines = null;

                    saSale = context.SavingAccountSales.Add(saSale);
                    context.SaveChanges();
                    saleID = saSale.SaleID;

                }

                //Si c'est une vente réglée par espèce
                if (paymentMethod is Till)
                {

                    TillSale tillSale = new TillSale();

                    //ces 2 lignes qui suivent permettent de transformer un ProductModel en Product 
                    //create a Map
                    Mapper.CreateMap<Sale, TillSale>();
                    //use Map
                    tillSale = Mapper.Map<TillSale>(sale);
                    tillSale.TillID = paymentMethod.ID;
                    tillSale.SaleLines = null;

                    tillSale = context.TillSales.Add(tillSale);
                    context.SaveChanges();
                    saleID = tillSale.SaleID;

                }

                //si c'est une vente réglèe par la banque
                if (paymentMethod is Bank)
                {

                    BankSale bankSale = new BankSale();

                    //ces 2 lignes qui suivent permettent de transformer un ProductModel en Product 
                    //create a Map
                    Mapper.CreateMap<Sale, BankSale>();
                    //use Map
                    bankSale = Mapper.Map<BankSale>(sale);
                    bankSale.BankID = paymentMethod.ID;
                    bankSale.SaleLines = null;

                    bankSale = context.BankSales.Add(bankSale);
                    context.SaveChanges();
                    saleID = bankSale.SaleID;
                }

                //si c'est une vente réglèe par la banque
                if (paymentMethod is DigitalPaymentMethod)
                {

                    var digitalPaymentSale = new DigitalPaymentSale();

                    //ces 2 lignes qui suivent permettent de transformer un ProductModel en Product 
                    //create a Map
                    Mapper.CreateMap<Sale, DigitalPaymentSale>();
                    //use Map
                    digitalPaymentSale = Mapper.Map<DigitalPaymentSale>(sale);
                    digitalPaymentSale.DigitalAccountManagerId = ((DigitalPaymentMethod)paymentMethod).AccountManagerId;
                    digitalPaymentSale.TransactionIdentifier = digitalPaymentSale.PaymentReference;
                    digitalPaymentSale.SaleLines = null;
                    digitalPaymentSale.DigitalPaymentMethodId = paymentMethod.ID;


                    digitalPaymentSale = context.DigitalPaymentSales.Add(digitalPaymentSale);
                    context.SaveChanges();
                    saleID = digitalPaymentSale.SaleID;
                }

                //We save slice of this customer with the sale's slice amount price
                if (paymentMethod is SavingAccount)
                {
                    CustomerSlice customerSlice = new CustomerSlice()
                    {
                        DeviseID = sale.CustomerSlice.DeviseID,
                        PaymentMethodID = sale.PaymentMethodID,
                        SaleID = saleID,
                        SliceAmount = sale.CustomerSlice.SliceAmount,
                        SliceDate = sale.SaleDate,//.SaleDeliveryDate.Date,
                        Representant = sale.PoliceAssurance,
                        Reference = sale.SaleReceiptNumber,
                        isDeposit = true,
                        OperatorID = UserConect
                    };
                    context.CustomerSlices.Add(customerSlice);
                    context.SaveChanges();
                }
                else
                {
                    CustomerSlice customerSlice = new CustomerSlice()
                    {
                        DeviseID = sale.CustomerSlice.DeviseID,
                        PaymentMethodID = sale.PaymentMethodID,
                        SaleID = saleID,
                        SliceAmount = sale.CustomerSlice.SliceAmount,
                        SliceDate = sale.SaleDate,//.SaleDeliveryDate.Date,
                        Representant = sale.PoliceAssurance,
                        Reference = sale.SaleReceiptNumber,
                        OperatorID = UserConect,
                    };

                    if (paymentMethod is DigitalPaymentMethod)
                    {
                        customerSlice.DigitalAccountManagerId = ((DigitalPaymentMethod)paymentMethod).AccountManagerId;
                        customerSlice.TransactionIdentifier = sale.PaymentReference;
                    }

                    context.CustomerSlices.Add(customerSlice);
                    context.SaveChanges();
                }

                TrancheAmount = sale.CustomerSlice.SliceAmount;
            }
            else
            {

                throw new Exception("Error: Please choose payment method before proceed");

            }
            if (!isOtherSale)
            {
                //ecriture de cumulsale and bill table
                CumulSaleAndBill newcumlsalebill = new CumulSaleAndBill()
                {
                    VatRate = sale.VatRate,
                    RateReduction = sale.RateReduction,
                    RateDiscount = sale.RateDiscount,
                    Transport = sale.Transport,
                    SaleDate = sale.SaleDate,
                    SaleReceiptNumber = sale.SaleReceiptNumber,
                    BranchID = sale.BranchID,
                    // CustomerName = customerEntity.Name,
                    CustomerName = customerEntity.CustomerFullName,
                    CustomerID = customerEntity.GlobalPersonID,
                    OriginSale = OriginSaleOperation.Sale,
                    Remarque = sale.Remarque,
                    MedecinTraitant = sale.MedecinTraitant,
                    OperatorID = sale.OperatorID.Value,
                    SaleID = saleID,
                    DateOperationHours = sale.SaleDateHours,
                    IsUrgent = sale.IsUrgent
                };
                context.CumulSaleAndBills.Add(newcumlsalebill);
                context.SaveChanges();
                cumulSaleAndBillID = newcumlsalebill.CumulSaleAndBillID;
            }
            

            int ProductId = 0;
            //bool isFrame = false;
            //vente sans commande ou vente directe
            foreach (SaleLine saleLine in sale.SaleLines)
            {
                if (saleLine.Product.ProductID <= 0)
                {
                    //persistance du nouvo verre
                    //isFrame = false;
                    Lens ProductLens = this.PersistCustomerOrderLine(saleLine);
                    ProductId = ProductLens.ProductID;
                    //saleLine.isCommandGlass = true;
                }
                else
                {
                    //isFrame = true;
                    //saleLine.isCommandGlass = false;
                    ProductId = saleLine.Product.ProductID;
                    //saleLine.Product.ProductCode=
                }

                SaleLine salineToSave = new SaleLine()
                {
                    LocalizationID = saleLine.LocalizationID,
                    ProductID = ProductId,
                    SaleID = saleID,
                    LineQuantity = saleLine.LineQuantity,
                    LineUnitPrice = saleLine.LineUnitPrice,
                    OeilDroiteGauche = saleLine.OeilDroiteGauche,
                    SpecialOrderLineCode = saleLine.SpecialOrderLineCode,
                    marque = saleLine.marque,
                    reference = saleLine.reference,
                    SupplyingName = saleLine.SupplyingName,
                    isGift = saleLine.isGift,
                    NumeroSerie = saleLine.NumeroSerie,
                    LensNumberSphericalValue = saleLine.LensNumberSphericalValue,
                    LensNumberCylindricalValue = saleLine.LensNumberCylindricalValue,
                    Axis = saleLine.Axis,
                    Addition = saleLine.Addition,
                    isCommandGlass = saleLine.isCommandGlass
                };
                saleAmount += saleLine.LineQuantity * saleLine.LineUnitPrice;
                context.SaleLines.Add(salineToSave);

                //ecriture du rdv si verre de commande
                if (saleLine.isCommandGlass)
                {
                    RendezVous newRDV = new RendezVous()
                    {
                        CustomerID = sale.CustomerID.Value,
                        DateRdv = sale.DateRdv.Value,
                        RaisonRdv = "Delivery of Lens " + saleLine.ProductLabel,
                        SaleID = saleID,
                        ProductID= ProductId
                    };
                    IRendezVous _rdvRepository = new RendezVousRepository(context);
                    res = _rdvRepository.AddRendezVous(newRDV, saleID, UserConect, sale.SaleDate, sale.BranchID);
                    if (!res)
                    {
                        throw new Exception("Error while create Rendez Vous ");
                    }
                }

                if (!isOtherSale)
                {
                    //duplicata pour lles lignes de la table de cumul
                    CumulSaleAndBillLine cumulSaleAndBillLineToSave = new CumulSaleAndBillLine()
                    {
                        LocalizationID = saleLine.LocalizationID,
                        ProductID = ProductId,
                        CumulSaleAndBillID = cumulSaleAndBillID,
                        LineQuantity = saleLine.LineQuantity,
                        LineUnitPrice = saleLine.LineUnitPrice,
                        OeilDroiteGauche = saleLine.OeilDroiteGauche,
                        SpecialOrderLineCode = saleLine.SpecialOrderLineCode,
                        marque = saleLine.marque,
                        reference = saleLine.reference,
                        SupplyingName = saleLine.SupplyingName,
                        isGift = saleLine.isGift,
                        LensNumberSphericalValue = saleLine.LensNumberSphericalValue,
                        LensNumberCylindricalValue = saleLine.LensNumberCylindricalValue,
                        Axis = saleLine.Axis,
                        Addition = saleLine.Addition,
                        NumeroSerie = saleLine.NumeroSerie,
                        ProductCategoryID = saleLine.Product.Category.CategoryID,
                        isCommandGlass = saleLine.isCommandGlass
                    };
                    context.CumulSaleAndBillLines.Add(cumulSaleAndBillLineToSave);
                    context.SaveChanges();
                }

                if (saleLine.Product is GenericProduct) //alors on le sort du stock
                {
                    ProductLocalization productInStock = new ProductLocalization();
                    if (!(saleLine.Product.Category.isSerialNumberNull))
                    {
                        //ecriture du stock
                        productInStock = context.ProductLocalizations.FirstOrDefault(pl => pl.ProductID == ProductId && pl.LocalizationID == saleLine.LocalizationID);
                    }
                    else
                    {
                        //ecriture du stock
                        // productInStock = context.ProductLocalizations.FirstOrDefault(pl => pl.ProductID == ProductId && pl.LocalizationID == saleLine.LocalizationID && pl.NumeroSerie == saleLine.NumeroSerie);

                        productInStock = context.ProductLocalizations.FirstOrDefault(pl => pl.ProductID == ProductId &&
                                         pl.NumeroSerie == saleLine.NumeroSerie && pl.Marque == saleLine.marque &&
                                         pl.LocalizationID == saleLine.LocalizationID);
                    }

                    if (productInStock == null)
                    {
                        //isNew = true;
                        ProductLocalization newPL = new ProductLocalization();
                        newPL.ProductLocalizationStockQuantity = saleLine.LineQuantity;
                        newPL.AutorizedByID = sale.OperatorID.Value;
                        newPL.ProductLocalizationSafetyStockQuantity = 0d;
                        newPL.ProductLocalizationStockSellingPrice = 0d;
                        newPL.AveragePurchasePrice = 0d;
                        newPL.ProductLocalizationDate = sale.SaleDate;
                        newPL.ProductID = ProductId;
                        newPL.LocalizationID = saleLine.LocalizationID;
                        newPL.isDeliver = false;
                        newPL.NumeroSerie = saleLine.NumeroSerie;
                        newPL.Marque = saleLine.marque;

                        context.ProductLocalizations.Add(newPL);
                        context.SaveChanges();
                        productInStock = newPL;
                    }
                    //sortie en stock
                    productInStock.SellingReference = sale.SaleReceiptNumber;
                    IProductLocalization _PLocalizationRep = new PLRepository(context);
                    _PLocalizationRep.StockOutput(productInStock, saleLine.LineQuantity, productInStock.AveragePurchasePrice, sale.SaleDate, UserConect);
                }
                //desormais c'est le comptable qui effectue tous les sorties du produits
                /*if (sale.SaleDeliver && isFrame)
                {
                    ProductLocalization productInStock = new ProductLocalization();
                    if (!(saleLine.Product.Category.isSerialNumberNull))
                    {
                        //ecriture du stock
                        productInStock = context.ProductLocalizations.FirstOrDefault(pl => pl.ProductID == ProductId && pl.LocalizationID == saleLine.LocalizationID);
                    }
                    else
                    {
                        //ecriture du stock
                        productInStock = context.ProductLocalizations.FirstOrDefault(pl => pl.ProductID == ProductId && pl.LocalizationID == saleLine.LocalizationID && pl.NumeroSerie == saleLine.NumeroSerie);
                    }

                    if (productInStock == null)
                    {
                        //isNew = true;
                        ProductLocalization newPL = new ProductLocalization();
                        newPL.ProductLocalizationStockQuantity = saleLine.LineQuantity;
                        newPL.AutorizedByID = sale.OperatorID.Value;
                        newPL.ProductLocalizationSafetyStockQuantity = 0d;
                        newPL.ProductLocalizationStockSellingPrice = 0d;
                        newPL.AveragePurchasePrice = 0d;
                        newPL.ProductLocalizationDate = sale.SaleDate;
                        newPL.ProductID = ProductId;
                        newPL.LocalizationID = saleLine.LocalizationID;
                        newPL.isDeliver = false;
                        newPL.NumeroSerie = saleLine.NumeroSerie;
                        newPL.Marque = saleLine.marque;
                       
                        context.ProductLocalizations.Add(newPL);
                        context.SaveChanges();
                        productInStock = newPL;
                        OldSafetyStockQuantity = 0d;
                        OldStockQuantity = 0d;
                        OldStockUnitPrice = 0d;
                    }
                    else
                    {
                        OldSafetyStockQuantity = productInStock.ProductLocalizationSafetyStockQuantity;
                        OldStockQuantity = productInStock.ProductLocalizationStockQuantity;
                        OldStockUnitPrice = productInStock.ProductLocalizationStockSellingPrice;
                    }
                    //sortie en stock
                    productInStock.ProductLocalizationStockQuantity = productInStock.ProductLocalizationStockQuantity - saleLine.LineQuantity;
                    //update si deliver
                    //HISTORISATION DU STOCK
                    DateTime date = sale.SaleDate;// productInStock.ProductLocalizationDate;
                    DateTime actualDate = DateTime.Now;
                    date = date.AddHours(actualDate.Hour);
                    date = date.AddMinutes(actualDate.Minute);
                    date = date.AddSeconds(actualDate.Second);
                    InventoryHistoric inventoryHistoric = new InventoryHistoric
                    {
                        //les nouvelles infos
                        NewSafetyStockQuantity = productInStock.ProductLocalizationSafetyStockQuantity,
                        NewStockQuantity = productInStock.ProductLocalizationStockQuantity,
                        NEwStockUnitPrice = productInStock.ProductLocalizationStockSellingPrice,
                        //les anciennes infos
                        OldSafetyStockQuantity = OldSafetyStockQuantity,
                        OldStockQuantity = OldStockQuantity,
                        OldStockUnitPrice = OldStockUnitPrice,
                        //Autres informations
                        InventoryDate = date,
                        inventoryReason = "Product Sales",
                        LocalizationID = productInStock.LocalizationID,
                        ProductID = productInStock.ProductID,
                        RegisteredByID = (sale.Operator != null) ? sale.OperatorID.Value : UserConect,
                        AutorizedByID = (sale.PostBy != null) ? sale.PostByID.Value : UserConect,
                        CountByID = (sale.PostBy != null) ? sale.PostByID.Value : UserConect,
                        StockStatus = "OUTPUT",
                        Description = (sale.CustomerID == null) ? "Product Sales for" + customerEntity.Name : "Product Sales for " + sale.CustomerName,
                        Quantity = saleLine.LineQuantity,
                        Marque=saleLine.marque,
                        NumeroSerie=saleLine.NumeroSerie
                    };
                    context.InventoryHistorics.Add(inventoryHistoric);
                }*/

                context.SaveChanges();
            }

            //metre a jour le transaction number
            //int compteur = Convert.ToInt32(sale.SaleReceiptNumber.Substring(12));
            //ITransactNumber trnNumber = new TransactNumberRepository(context);
            //res = trnNumber.saveTransactNumber("SALE", compteur);
            //if (!res)
            //{
            //    throw new Exception("Une erreur s'est produite lors de la mise a jour du compteur du transact number ");
            //}
            //we take a extra price
            double totalPriceHT = saleAmount;
            ExtraPrice extra = Util.ExtraPrices(totalPriceHT, sale.RateReduction, sale.RateDiscount, sale.Transport, sale.VatRate);
            //Accountig operations
            if (sale.CustomerID == null) sale.CustomerID = customerEntity.GlobalPersonID;
            Sale acountableSale = new Sale()
            {
                BranchID = sale.BranchID,
                CustomerID = sale.CustomerID.Value,
                CustomerOrderID = sale.CustomerOrderID,
                PaymentMethodID = paymentMethod != null ? sale.PaymentMethodID : 0,
                SaleDate = sale.SaleDate,
                DeviseID = sale.DeviseID,
                SaleID = saleID,
                TotalPriceHT = extra.TotalHT, //montant brut
                TotalPriceTTC = extra.TotalTTC, //net a payer
                SaleReceiptNumber = sale.SaleReceiptNumber,
                SaleTotalPriceAdvance = TrancheAmount,  //versement client
                RateReduction = sale.RateReduction,
                RateDiscount = sale.RateDiscount,
                ReductionAmount = extra.ReductionAmount, //reduction
                DiscountAmount = extra.DiscountAmount, //escompe
                Transport = sale.Transport, //Transport
                TVAAmount = extra.TVAAmount, //vat amount,
                NetFinancier = extra.NetFinan,
                NetCommercial = extra.NetCom,
                StatutSale = sale.StatutSale,
                OldStatutSale = sale.OldStatutSale
            };

            //////comptabilisation
            //////seule la reception de l'argent sera comptabiliser a ce niveau
            ////IAccountOperation opaccount = new AccountOperationRepository(context);
            ////res = opaccount.ecritureComptableFinal(acountableSale);
            ////if (!res)
            ////{
            ////    throw new Exception("Une erreur s'est produite lors de comptabilisation de la vente ");
            ////}
            
            //si commande update du status
            if (isCommand)
            {
                AuthoriseSale authSaleLine = context.AuthoriseSales.Where(auth => sale.AuthoriseSaleID.HasValue && auth.AuthoriseSaleID == sale.AuthoriseSaleID.Value).SingleOrDefault();
                if (authSaleLine == null)
                {
                    throw new Exception("Error while updated command sale. please call your administrator ");
                }
                authSaleLine.IsDelivered = true;
                context.SaveChanges();
            }
            //si validation dilation
            if (sale.isDilation && sale.ConsultDilPrescID.HasValue)
            {
                //recuperation de la consulation de ce client pour cette journée
                //Consultation Consult = context.Consultations.Where(c => c.CustomerID == sale.CustomerID.Value && c.DateConsultation == sale.SaleDate).FirstOrDefault();
                //recherche de la prescription correspondant a cette consultation 
                //Prescription existPrescript = context.Prescriptions.Where(c => c.ConsultationID == Consult.ConsultationID && c.isDilation).FirstOrDefault();
                //if (existPrescript == null)
                //{
                //    throw new Exception("Error while updated command sale for prescription - Dilation. please call your administrator ");
                //}
                //existPrescript.CodeDilation = sale.SaleReceiptNumber ;

                ConsultDilatation consultdilatation = context.ConsultDilatations.Where(c=>c.ConsultDilPrescID == sale.ConsultDilPrescID.Value).FirstOrDefault();
                if (consultdilatation == null)
                {
                    throw new Exception("Error while updated command sale for prescription - Dilation. please call your administrator ");
                }
                consultdilatation.CodeDilation = sale.SaleReceiptNumber;
                context.SaveChanges();

            }
            //EcritureSneack
            IMouchar opSneak = new MoucharRepository(context);
            res = opSneak.InsertOperation(UserConect, "SUCCESS", "SALE-REFERENCE " + sale.SaleReceiptNumber + " FOR CUSTOMER " + sale.PoliceAssurance, "PersistSale", sale.SaleDate, sale.BranchID);
            if (!res)
            {
                throw new Exception("Une erreur s'est produite lors de la journalisation ");
            }
            sale.SaleID = saleID;
            return sale;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="custOrdLine"></param>
        /// <returns></returns>
        public SaleLine GetSaleLineFromOrdLine(CustomerOrderLine custOrdLine)
        {
            SaleLine res = new SaleLine();

            //ces 2 lignes qui suivent permettent de transformer un CustomerOrderLine en PurchaseLine 
            //create a Map
            Mapper.CreateMap<CustomerOrderLine, SaleLine>();
            //use Map
            res = Mapper.Map<SaleLine>(custOrdLine);
            //res.LineUnitPrice = custOrdLine.PurchaseLineUnitPrice;

            return res;

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="custord"></param>
        /// <param name="HeureVente"></param>
        /// <param name="UserConect"></param>
        /// <param name="ServerDate"></param>
        /// <param name="BranchID"></param>
        /// <param name="PaymentMethodID"></param>
        /// <param name="Reference"></param>
        /// <param name="BorderoDepotID"></param>
        /// <returns></returns>
        public bool ValidateBorderoDepotFacture(List<BorderoItems> custord,int BorderoDepotID, string HeureVente,int BranchID, int UserConect, DateTime ServerDate,int PaymentMethodID,string Reference)
        {
            //Begin of transaction
            bool res = false;
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    //ajout de lheure de la vente
                    string[] tisys = HeureVente.Split(new char[] { ':' });
                    DateTime date = ServerDate;
                    date = date.AddHours(Convert.ToDouble(tisys[0]));
                    date = date.AddMinutes(Convert.ToDouble(tisys[1]));
                    date = date.AddSeconds(Convert.ToDouble(tisys[2]));

                    

                    foreach (BorderoItems inLines in custord)
                    {
                        CustomerOrder custOrder = context.CustomerOrders.Find(inLines.ID);
                        if (custOrder.CustomerOrderID > 0)
                        {
                            if (custOrder.AssureurID == null)
                            {
                                string Message = "The Bill Ref " + inLines.NumeroFacture + " not yet been validated. Please consult your Accountant";
                                throw new Exception(Message);
                            }
                            custOrder.MntValidate = Convert.ToDouble(inLines.MntValideBordero);

                            custOrder.PaymentMethodID = PaymentMethodID;
                            custOrder.PaymentReference = Reference;

                            if (custOrder.MntValidate > 0)
                            {
                                this.SaveChangeBorderoLines(custOrder, date, UserConect, ServerDate);
                            }

                        }
                    }



                    //si toutes les lignes du bordero ont ete valider alors on valide le bordero

                    if (BorderoDepotID > 0)
                    {
                        //verifions si ttes les lignes ont ete validé
                        List<CustomerOrder> nonValidatedBill = context.CustomerOrders.Where(c => !c.isMntValideBordero && c.BorderoDepotID.HasValue && c.BorderoDepotID.Value == BorderoDepotID).ToList();

                        if (nonValidatedBill.Count == 0)
                        {
                            BorderoDepot existbordero = context.BorderoDepots.Find(BorderoDepotID);
                            existbordero.HeureValidateBordero = HeureVente;
                            existbordero.ValidBorderoDepotDate = ServerDate;
                            existbordero.ValideBorderoDepot = true;
                            existbordero.ValidateByID = UserConect;
                            context.SaveChanges();
                        }
                    }
                    else
                    {
                        throw new Exception("This Bordero Not yet been Validated!!! please consult your Administrator ");
                    }


                    res = true;
                    //transaction.Commit();
                    ts.Complete();
                }
            }
            catch (Exception e)
            {
                //If an errors occurs, we cancel all changes in database
                //EcritureSneack
                IMouchar opSneak = new MoucharRepository(context);
                res = opSneak.InsertOperation(UserConect, "ERROR", "VALIDATE BORDERO " + BorderoDepotID , "PersistSale", ServerDate, BranchID);
                if (!res)
                {
                    throw new Exception("Une erreur s'est produite lors de la journalisation ");
                }
                throw new Exception("Une erreur s'est produite lors de la vente : " + "e.Message = " + e.Message + "e.InnerException = " + e.InnerException + "e.StackTrace = " + e.StackTrace);
            }
            return res;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="custord"></param>
        /// <param name="DateHeureVente"></param>
        /// <param name="UserConect"></param>
        /// <param name="ServerDate"></param>
        /// <returns></returns>
        public bool SaveChangeBorderoLines(CustomerOrder custord, DateTime DateHeureVente, int UserConect, DateTime ServerDate)
        {
            //Begin of transaction
            bool res = false;
            try
            {
                ITransactNumber trnNumber = new TransactNumberRepository(context);
                string SaleReceiptNumber = trnNumber.displayTransactNumber("SABI", ServerDate, custord.BranchID);

                Sale sale = new Sale()
                {
                    SaleDateHours = DateHeureVente,
                    CompteurFacture = custord.CompteurFacture,
                    SaleDeliver = true,
                    VatRate = custord.VatRate,
                    Transport = custord.Transport,
                    SaleDeliveryDate = ServerDate,
                    SaleDate = ServerDate,
                    SaleValidate = true,
                    PoliceAssurance = custord.PoliceAssurance,
                    DeviseID = custord.DeviseID,
                    IsPaid = true,
                    SaleReceiptNumber = SaleReceiptNumber,
                    BranchID = custord.BranchID,
                    CustomerOrderID = custord.CustomerOrderID,
                    CustomerName = custord.CustomerName,
                    isReturn = false,
                    OperatorID = custord.OperatorID,
                    PostByID = UserConect,
                    Remarque = custord.Remarque,
                    MedecinTraitant = custord.MedecinTraitant,
                    PaymentMethodID = custord.PaymentMethodID ,//AssureurID.Value,
                    //SaleLines=saleLines,
                    SaleTotalPrice = custord.MntValidate,
                    PaymentReference = custord.PaymentReference,
                    IsSpecialOrder=false
                };
                    //context.Sales.Add(sale);

                    sale = PersistSaleBill(sale, UserConect, custord.AssureurID.Value);

                    //update du customer order du bill state
                   
                    if (custord.CustomerOrderID > 0)
                    {

                        //vente avec commande au préalable
                        foreach (CustomerOrderLine customerOrderLine in context.CustomerOrderLines.Where(cl => cl.CustomerOrderID == custord.CustomerOrderID).ToList())
                        {
                            SaleLine salineToSave = new SaleLine()
                            {
                                LocalizationID = customerOrderLine.LocalizationID,
                                ProductID = customerOrderLine.ProductID,
                                SaleID = sale.SaleID,
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
                                isCommandGlass=customerOrderLine.isCommandGlass,
                                isGift = customerOrderLine.isGift,
                                
                            };

                            context.SaleLines.Add(salineToSave);
                            context.SaveChanges();
                        }
                        //We update Customer Order statut
                        CustomerOrder customerOderToUpdate = context.CustomerOrders.Find(custord.CustomerOrderID);
                        customerOderToUpdate.BillState = StatutFacture.Paid;
                        customerOderToUpdate.MntValidate = custord.MntValidate;
                        customerOderToUpdate.ValidateBillDate = ServerDate;
                        customerOderToUpdate.isMntValideBordero = true;
                        customerOderToUpdate.MntValideBordero = custord.MntValidate;
                        context.SaveChanges();
                    }
                    
                    
                    res = true;
                   
            }
            catch (Exception e)
            {
                //If an errors occurs, we cancel all changes in database
                //transaction.Rollback();
                //EcritureSneack
                IMouchar opSneak = new MoucharRepository(context);
                res = opSneak.InsertOperation(UserConect, "ERROR", "SALE-REFERENCE " + custord.CustomerOrderNumber + " FOR CUSTOMER " + custord.PoliceAssurance, "PersistSale", custord.CustomerOrderDate, custord.BranchID);
                if (!res)
                {
                    throw new Exception("Une erreur s'est produite lors de la journalisation ");
                }
                throw new Exception("Une erreur s'est produite lors de la vente : " + "e.Message = " + e.Message + "e.InnerException = " + e.InnerException + "e.StackTrace = " + e.StackTrace);
            }
            return res;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sale"></param>
        /// <param name="UserConect"></param>
        /// <param name="AssureurID"></param>
        /// <returns></returns>
        private Sale PersistSaleBill(Sale sale, int UserConect, int AssureurID)
        {

            bool res = false;
            //double saleAmount = 0;
            double TrancheAmount = 0d;

            //AssureurPM assPM = context.AssureurPMs.Where(c=>c.AssureurID== AssureurID).FirstOrDefault();

            //PaymentMethod paymentMethod = context.AssureurPMs.Where(c => c.AssureurID == AssureurID).FirstOrDefault();

            //payement par cheque
            PaymentMethod paymentMethod = context.PaymentMethods.Find(sale.PaymentMethodID);

            Customer customerEntity = context.Customers.Where(c => c.Name.ToUpper() == "DEFAULT").FirstOrDefault();
            if (customerEntity == null)
            {
                throw new Exception("Erreur : Please you must create the default Customer Before Proceed ");
            }
            //this is variable is saleID after determine type of payment method
            int saleID = 0;


            if (paymentMethod != null)
            {

                sale.StatutSale = SalePurchaseStatut.Paid;
                sale.IsSpecialOrder = true;
                if (sale.CustomerID == null) sale.CustomerID = customerEntity.GlobalPersonID;
                //la vente sera réglée par le compte de l'assureur
                if (paymentMethod is AssureurPM)
                {

                    AssureurSale saSale = new AssureurSale();

                    //ces 2 lignes qui suivent permettent de transformer un ProductModel en Product 
                    //create a Map
                    Mapper.CreateMap<Sale, AssureurSale>();
                    //use Map
                    saSale = Mapper.Map<AssureurSale>(sale);
                    saSale.AssureurPMID = paymentMethod.ID;
                    saSale.SaleLines = null;

                    saSale = context.AssureurSales.Add(saSale);
                    context.SaveChanges();
                    saleID = saSale.SaleID;

                }

                //Si c'est une vente réglée par espèce
                if (paymentMethod is Till)
                {

                    TillSale tillSale = new TillSale();

                    //ces 2 lignes qui suivent permettent de transformer un ProductModel en Product 
                    //create a Map
                    Mapper.CreateMap<Sale, TillSale>();
                    //use Map
                    tillSale = Mapper.Map<TillSale>(sale);
                    tillSale.TillID = paymentMethod.ID;
                    tillSale.SaleLines = null;

                    tillSale = context.TillSales.Add(tillSale);
                    context.SaveChanges();
                    saleID = tillSale.SaleID;

                }

                //si c'est une vente réglèe par la banque
                if (paymentMethod is Bank)
                {

                    BankSale bankSale = new BankSale();

                    //ces 2 lignes qui suivent permettent de transformer un ProductModel en Product 
                    //create a Map
                    Mapper.CreateMap<Sale, BankSale>();
                    //use Map
                    bankSale = Mapper.Map<BankSale>(sale);
                    bankSale.BankID = paymentMethod.ID;
                    bankSale.BankRef = sale.PaymentReference;

                    bankSale.SaleLines = null;

                    bankSale = context.BankSales.Add(bankSale);
                    context.SaveChanges();
                    saleID = bankSale.SaleID;
                }

                //We save slice of this customer with the sale's slice amount price
                if (paymentMethod is AssureurPM)
                {
                    CustomerSlice customerSlice = new CustomerSlice()
                    {
                        DeviseID = sale.DeviseID,
                        PaymentMethodID = paymentMethod.ID,
                        SaleID = saleID,
                        SliceAmount = sale.SaleTotalPrice,
                        SliceDate = sale.SaleDate,//.SaleDeliveryDate.Date,
                        Representant = sale.PoliceAssurance,
                        Reference = sale.SaleReceiptNumber,
                        isDeposit = true,
                        OperatorID = UserConect
                    };
                    context.CustomerSlices.Add(customerSlice);
                    context.SaveChanges();
                }
                else
                {
                    CustomerSlice customerSlice = new CustomerSlice()
                    {
                        DeviseID = sale.DeviseID,
                        PaymentMethodID = paymentMethod.ID,
                        SaleID = saleID,
                        SliceAmount = sale.SaleTotalPrice,
                        SliceDate = sale.SaleDate,//.SaleDeliveryDate.Date,
                        Representant = sale.PoliceAssurance,
                        Reference = sale.SaleReceiptNumber,
                        OperatorID = UserConect
                    };
                    context.CustomerSlices.Add(customerSlice);
                    context.SaveChanges();
                }

                TrancheAmount = sale.SaleTotalPrice;
            }
            else
            {

                throw new Exception("Wrong payment Method !!!! Please contact your Administrator");

            }


            //metre a jour le transaction number
            int compteur = Convert.ToInt32(sale.SaleReceiptNumber.Substring(12));
            ITransactNumber trnNumber = new TransactNumberRepository(context);
            res = trnNumber.saveTransactNumber("SABI", compteur);
            if (!res)
            {
                throw new Exception("Une erreur s'est produite lors de la mise a jour du compteur du transact number ");
            }

            //we take a extra price
            double totalPriceHT = sale.SaleTotalPrice;
            ExtraPrice extra = Util.ExtraPrices(totalPriceHT, sale.RateReduction, sale.RateDiscount, sale.Transport, sale.VatRate);
            //Accountig operations
            Sale acountableSale = new Sale()
            {
                BranchID = sale.BranchID,
                CustomerID = sale.CustomerID.Value,
                CustomerOrderID = sale.CustomerOrderID,
                PaymentMethodID = paymentMethod != null ? paymentMethod.ID : 0,
                SaleDate = sale.SaleDate,
                DeviseID = sale.DeviseID,
                SaleID = saleID,
                TotalPriceHT = extra.TotalHT, //montant brut
                TotalPriceTTC = extra.TotalTTC, //net a payer
                SaleReceiptNumber = sale.SaleReceiptNumber,
                SaleTotalPriceAdvance = TrancheAmount,  //versement client
                RateReduction = sale.RateReduction,
                RateDiscount = sale.RateDiscount,
                ReductionAmount = extra.ReductionAmount, //reduction
                DiscountAmount = extra.DiscountAmount, //escompe
                Transport = sale.Transport, //Transport
                TVAAmount = extra.TVAAmount, //vat amount,
                NetFinancier = extra.NetFinan,
                NetCommercial = extra.NetCom,
                StatutSale = sale.StatutSale,
                OldStatutSale = SalePurchaseStatut.Delivered
            };

            //////comptabilisation
            ////IAccountOperation opaccount = new AccountOperationRepository(context);
            ////res = opaccount.ecritureComptableFinal(acountableSale);
            ////if (!res)
            ////{
            ////    throw new Exception("Une erreur s'est produite lors de comptabilisation de la vente ");
            ////}
            //EcritureSneack
            IMouchar opSneak = new MoucharRepository(context);
            res = opSneak.InsertOperation(UserConect, "SUCCESS", "SALE-REFERENCE " + sale.SaleReceiptNumber + " FOR CUSTOMER " + sale.PoliceAssurance, "PersistSale", sale.SaleDate, sale.BranchID);
            if (!res)
            {
                throw new Exception("Une erreur s'est produite lors de la journalisation ");
            }
            sale.SaleID = saleID;

            return sale;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="saleLine"></param>
        /// <returns></returns>
        public Lens PersistCustomerOrderLine(CumulSaleAndBillLine saleLine)
        {
            Product currentProduct = LensConstruction.GetOrderLensByCustOrdLine(saleLine, this.context);
            currentProduct = LensConstruction.CreateLens((Lens)currentProduct, this.context);
            return (Lens)currentProduct;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="saleLine"></param>
        /// <returns></returns>
        public Lens PersistCustomerOrderLine(SaleLine saleLine)
        {
            Product currentProduct = LensConstruction.GetOrderLensByCustOrdLine(saleLine, this.context);
            currentProduct = LensConstruction.CreateLens((Lens)currentProduct, this.context);
            return (Lens)currentProduct;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentProduct"></param>
        /// <returns></returns>
        //1-Création du Produit s'il n'existe pas
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
            //recuperation  du premier cpte de cette category
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
        /// <param name="SaleID"></param>
        /// <param name="UserConect"></param>
        /// <param name="ServerDate"></param>
        /// <returns></returns>
        public bool SaleDeleteDoubleEntry(int SaleID, int UserConect, DateTime ServerDate)
        {
            bool res = false;
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    //SaleAccountOperation
                    context.AccountOperations.OfType<SaleAccountOperation>().Where(a => a.SaleID == SaleID).ToList().ForEach(ol =>
                    {
                        context.AccountOperations.Remove(ol);
                        context.SaveChanges();
                    });
                    //CustomerSlices
                    List<PaymentMethod> lstPayMethod = new List<PaymentMethod>();
                    context.CustomerSlices.Where(a => a.SaleID == SaleID).ToList().ForEach(ol =>
                    {
                        lstPayMethod.Add(ol.PaymentMethod);
                        context.CustomerSlices.Remove(ol);
                        context.SaveChanges();
                    });
                    //SaleLines
                    context.SaleLines.Where(a => a.SaleID == SaleID).ToList().ForEach(ol =>
                    {
                        context.SaleLines.Remove(ol);
                        context.SaveChanges();
                    });
                    //Sales
                    Sale sale = context.Sales.Where(a => a.SaleID == SaleID).FirstOrDefault();
                    //.ToList().ForEach(ol =>
                    //{
                    context.Sales.Remove(sale);
                    context.SaveChanges();

                    foreach (PaymentMethod paidmeth in lstPayMethod)
                    {
                        //recuperation de la caisse ki a efectuer la transaction
                        Till till = context.Tills.Find(paidmeth.ID);

                        //mise a jour des solde de la caisse
                        DateTime DateDernierOp = new DateTime(1900, 01, 01);
                        DateTime DateFuturOp = new DateTime(1900, 01, 01);
                        ITillDay _tillDayRepository = new TillDayRepository();

                        DateTime DateVeilleOp = sale.SaleDate.Date.AddDays(-1);
                        var tillDayDernierOp = context.TillDays.Where(tdd => tdd.TillID == till.ID && tdd.TillDayDate <= DateVeilleOp.Date).OrderByDescending(s => s.TillDayDate).Take(1);
                        foreach (var getTillDay in tillDayDernierOp)
                        {
                            DateVeilleOp = getTillDay.TillDayDate;
                        }
                        TillSatut tillstatusVeilleOp = _tillDayRepository.TillStatus(till, DateVeilleOp);
                        double VeilleOpAmt = 0;// tillstatusVeilleOp.OpenningPrice;

                        DateTime dateop = sale.SaleDate.Date;
                        do
                        {
                            TillDay ltd = context.TillDays.Where(td => td.TillID == till.ID && td.TillDayDate == dateop.Date).SingleOrDefault();
                            if (ltd != null)
                            {

                                TillSatut tillStatus = _tillDayRepository.TillStatus(till, ltd.TillDayDate, VeilleOpAmt);

                                ltd.TillDayOpenPrice = VeilleOpAmt;
                                ltd.TillDayClosingPrice = VeilleOpAmt + tillStatus.Inputs - tillStatus.Ouputs;

                                this.context.TillDays.Attach(ltd);
                                context.Entry(ltd).State = EntityState.Modified;

                                VeilleOpAmt = 0;// VeilleOpAmt + tillStatus.Inputs - tillStatus.Ouputs;
                                context.SaveChanges();
                            }

                            dateop = dateop.AddDays(1);
                        } while (dateop <= ServerDate.Date);

                    }

                    context.SaveChanges();
                    IMouchar opSneak = new MoucharRepository(context);
                    res = opSneak.InsertOperation(UserConect, "SUCCESS", "DELETE DOUBLE ENTRY FOR SALE-REFERENCE " + sale.SaleReceiptNumber + " FOR CUSTOMER " + sale.PoliceAssurance, "SaleDeleteDoubleEntry", sale.SaleDate, sale.BranchID);
                    if (!res)
                    {
                        throw new Exception("Une erreur s'est produite lors de la journalisation ");
                    }
                    res = true;
                    ts.Complete();
                }
            }
            catch (Exception e)
            {
                res = false;
                //EcritureSneack
                Sale sale = context.Sales.Where(a => a.SaleID == SaleID).FirstOrDefault();
                IMouchar opSneak = new MoucharRepository(context);
                res = opSneak.InsertOperation(UserConect, "ERROR", "DELETE DOUBLE ENTRY FOR SALE-REFERENCE " + sale.SaleReceiptNumber + " FOR CUSTOMER " + sale.PoliceAssurance, "PersistSale", sale.SaleDate, sale.BranchID);
                if (!res)
                {
                    throw new Exception("Une erreur s'est produite lors de la journalisation ");
                }
                throw new Exception("Error while delete Double Sales : " + "e.Message = " + e.Message + "e.InnerException = " + e.InnerException + "e.StackTrace = " + e.StackTrace);
            }
            return res;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="customer"></param>
        /// <param name="BeginDate"></param>
        /// <returns></returns>
        public double TotalAchatBefore(Customer customer, DateTime BeginDate)
        {
            double res = 0;
            Sale getSaleBefore = new Sale();
            ICustomerReturn _customerReturnRepository = new CustomerReturnRepository(context);
            //recuperation du solde avant la date debut
            //1-recup du solde du client sans tenir en cpte les depots avant la date
            List<Sale> venteRegleBefore = context.Sales.Where(c => c.CustomerID == customer.GlobalPersonID && c.SaleDate < BeginDate).ToList();
            venteRegleBefore.ForEach(regsale =>
            {
                //considerons les ventes sans retours
                getSaleBefore = _customerReturnRepository.GetRealSale(regsale);
                double saleAmnt = getSaleBefore.SaleLines.Select(l => l.LineAmount).Sum();
                ExtraPrice extra = Util.ExtraPrices(saleAmnt, getSaleBefore.RateReduction, getSaleBefore.RateDiscount, getSaleBefore.Transport, getSaleBefore.VatRate);
                res += extra.TotalTTC;
            });

            return res;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="customer"></param>
        /// <param name="BeginDate"></param>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        public double TotalAchatPeriode(Customer customer, DateTime BeginDate, DateTime EndDate)
        {
            double res = 0;
            Sale getSalePeriode = new Sale();
            ICustomerReturn _customerReturnRepository = new CustomerReturnRepository(context);
            //recuperation du solde avant la date debut
            //1-recup du solde du client sans tenir en cpte les depots avant la date
            List<Sale> venteReglePeriode = context.Sales.Where(c => c.CustomerID == customer.GlobalPersonID && (c.SaleDate >= BeginDate && c.SaleDate <= EndDate)).ToList();
            venteReglePeriode.ForEach(regsale =>
            {
                //considerons les ventes sans retours
                getSalePeriode = _customerReturnRepository.GetRealSale(regsale);
                double saleAmnt = getSalePeriode.SaleLines.Select(l => l.LineAmount).Sum();
                ExtraPrice extra = Util.ExtraPrices(saleAmnt, getSalePeriode.RateReduction, getSalePeriode.RateDiscount, getSalePeriode.Transport, getSalePeriode.VatRate);
                res += extra.TotalTTC;
            });

            return res;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        public double TotalAchat(Customer customer)
        {
            double res = 0;
            Sale getSale = new Sale();
            ICustomerReturn _customerReturnRepository = new CustomerReturnRepository(context);
            //recuperation du solde avant la date debut
            //1-recup du solde du client sans tenir en cpte les depots avant la date
            List<Sale> venteRegle = context.Sales.Where(c => c.CustomerID == customer.GlobalPersonID).ToList();
            venteRegle.ForEach(regsale =>
            {
                //considerons les ventes sans retours
                getSale = _customerReturnRepository.GetRealSale(regsale);
                double saleAmnt = getSale.SaleLines.Select(l => l.LineAmount).Sum();
                ExtraPrice extra = Util.ExtraPrices(saleAmnt, getSale.RateReduction, getSale.RateDiscount, getSale.Transport, getSale.VatRate);
                res += extra.TotalTTC;
            });

            return res;
        }

        //ecriture bordero de depot de facture
        /// <summary>
        /// 
        /// </summary>
        /// <param name="heureVente"></param>
        /// <param name="BranchID"></param>
        /// <param name="AssuranceID"></param>
        /// <param name="BeginDate"></param>
        /// <param name="EndDate"></param>
        /// <param name="CodeBordero"></param>
        /// <param name="CompanyID"></param>
        /// <param name="LieuxdeDepotBorderoID"></param>
        /// <param name="DateOperation"></param>
        /// <param name="UserConnected"></param>
        /// <param name="rows_ID"></param>
        public int SaveBorderoDepotFacture(string heureVente, int BranchID, int AssuranceID, DateTime BeginDate, DateTime EndDate,  string CodeBordero, List<int> rows_ID, string CompanyID, int LieuxdeDepotBorderoID, DateTime DateOperation,int UserConnected)
        {
            int res = 0;
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    
                   res= SimpleBorderoDepotFacture(heureVente, BranchID, AssuranceID, BeginDate, EndDate, CodeBordero, rows_ID, CompanyID, LieuxdeDepotBorderoID, DateOperation, UserConnected);
                    
                   ts.Complete();
                }
            }
            catch (Exception e)
            {
                //If an errors occurs, we cancel all changes in database
                //transaction.Rollback();
                throw new Exception("Une erreur s'est produite lors de la creation du bordero de depot : " + e.Message);
            }
            return res;
        }

        private int SimpleBorderoDepotFacture(string heureVente, int BranchID, int AssuranceID, DateTime BeginDate, DateTime EndDate, string CodeBordero,List<int> rows_ID, string CompanyID, int LieuxdeDepotBorderoID, DateTime DateOperation,int UserConnected)
        {
            int borderoID = 0; 
            
            try
            {
                ICompteurBorderoDepot _compteurBorderoDepotRepository = new CompteurBorderoDepotRepository(context);

                //verifions si ce bordero existe deja
                BorderoDepot existBordero = context.BorderoDepots.Where(c => c.CodeBorderoDepot == CodeBordero && c.AssureurID == AssuranceID && c.CompanyID == CompanyID && c.LieuxdeDepotBorderoID == LieuxdeDepotBorderoID).FirstOrDefault();

                if (existBordero == null)
                {
                    //creation du bordero de depot
                    BorderoDepot newBordero = new BorderoDepot()
                    {
                        CodeBorderoDepot = CodeBordero,
                        BorderoDepotDate = DateOperation,
                        AssureurID = AssuranceID,
                        CompanyID = CompanyID,
                        LieuxdeDepotBorderoID = LieuxdeDepotBorderoID,
                        HeureGenerateBordero= heureVente,
                        GenerateByID= UserConnected
                    };
                    existBordero = context.BorderoDepots.Add(newBordero);
                    context.SaveChanges();
                    borderoID = existBordero.BorderoDepotID;
                }

                List<CustomerOrder> lstCustOrder = new List<CustomerOrder>();
                lstCustOrder = context.CustomerOrders.Where(so => so.BillState == StatutFacture.Validated && (so.BorderoDepotID == null)
                           && so.BranchID == BranchID && rows_ID.Contains(so.CustomerOrderID)).ToList();

                //if ((CompanyID == "0" || CompanyID == null))
                //{
                //    if (LieuxdeDepotBorderoID == 0)
                //    {
                //        lstCustOrder = context.CustomerOrders.Where(so => so.BillState == StatutFacture.Validated && (so.BorderoDepotID == null)
                //           && so.BranchID == BranchID && rows_ID.Contains(so.CustomerOrderID) // (so.ValidateBillDate >= BeginDate.Date && so.ValidateBillDate <= EndDate.Date)
                //           && so.AssureurID == AssuranceID).ToList();
                //    }
                //    else
                //    {
                //        lstCustOrder = context.CustomerOrders.Where(so => so.BillState == StatutFacture.Validated && (so.BorderoDepotID == null)
                //           && so.BranchID == BranchID && rows_ID.Contains(so.CustomerOrderID) //(so.ValidateBillDate >= BeginDate.Date && so.ValidateBillDate <= EndDate.Date)
                //           && so.AssureurID == AssuranceID && so.LieuxdeDepotBorderoID == LieuxdeDepotBorderoID).ToList();
                //    }
                //}
                //else
                //{
                //    if (LieuxdeDepotBorderoID == 0)
                //    {
                //        lstCustOrder = context.CustomerOrders.Where(so => so.BillState == StatutFacture.Validated && (so.BorderoDepotID == null)
                //           && so.BranchID == BranchID && rows_ID.Contains(so.CustomerOrderID) //(so.ValidateBillDate >= BeginDate.Date && so.ValidateBillDate <= EndDate.Date)
                //           && so.AssureurID == AssuranceID && so.CompanyName == CompanyID).ToList();
                //    }
                //    else
                //    {
                //        lstCustOrder = context.CustomerOrders.Where(so => so.BillState == StatutFacture.Validated && (so.BorderoDepotID == null)
                //           && so.BranchID == BranchID && rows_ID.Contains(so.CustomerOrderID) //(so.ValidateBillDate >= BeginDate.Date && so.ValidateBillDate <= EndDate.Date)
                //           && so.AssureurID == AssuranceID && so.CompanyName == CompanyID && so.LieuxdeDepotBorderoID == LieuxdeDepotBorderoID).ToList();
                //    }
                //}

                foreach (CustomerOrder CustOrder in lstCustOrder)
                {

                    //CustomerOrder ExistingCustOrder = context.CustomerOrders.Find(CustOrder.CustomerOrderID);
                    CustOrder.InsurreName = (CustOrder.InsurreName == null || CustOrder.InsurreName == "") ? CustOrder.CustomerName : CustOrder.InsurreName;
                    //update du customer order
                    CustOrder.BorderoDepotID = existBordero.BorderoDepotID;
                    //ExistingCustOrder.BorderoDepotID = existBordero.BorderoDepotID;
                    context.SaveChanges();
                    
                }

                //update du compteur
                string CompteurCode = CodeBordero.Substring(0, 4);
                int Compteur = Convert.ToInt32(CompteurCode)+1;
                string code = _compteurBorderoDepotRepository.GenerateBDFCode(AssuranceID, DateOperation.Year, CompanyID, LieuxdeDepotBorderoID, true, Compteur);

                //EcritureSneack
                IMouchar opSneak = new MoucharRepository(context);
                bool res = opSneak.InsertOperation(UserConnected, "SUCCESS", "BORDERO " + CodeBordero + " FOR INSURANCE " + AssuranceID, "SaveBorderoDepotFacture", DateOperation, BranchID);
                if (!res)
                {
                    throw new Exception("Une erreur s'est produite lors de la journalisation ");
                }
                
            }
            catch (Exception e)
            {
                IMouchar opSneak = new MoucharRepository(context);
                bool res = opSneak.InsertOperation(UserConnected, "ERROR", "BORDERO " + CodeBordero + " FOR INSURANCE " + AssuranceID, "SaveBorderoDepotFacture", DateOperation, BranchID);
                if (!res)
                {
                    throw new Exception("Une erreur s'est produite lors de la journalisation ");
                }
                throw new Exception("Une erreur s'est produite lors de la creation du bordero de depot : " + e.Message);
            }
            return borderoID;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="BranchID"></param>
        /// <param name="BorderoDepotID"></param>
        /// <param name="DateOperation"></param>
        /// <param name="UserConnected"></param>
        /// <returns></returns>
        public bool  DeleteCancelBordero( int BranchID, int BorderoDepotID, DateTime DateOperation, int UserConnected)
        {

            bool resDel = false;
            try
            {
                
                using (TransactionScope ts = new TransactionScope())
                {
                    //so.BillState == StatutFacture.Validated &&
                    List<CustomerOrder> lstCustOrder = new List<CustomerOrder>();
                    lstCustOrder = context.CustomerOrders.Where(so =>  (so.BorderoDepotID == BorderoDepotID)
                               && so.BranchID == BranchID).ToList();

                    foreach (CustomerOrder CustOrder in lstCustOrder)
                    {

                        //update du customer order
                        CustOrder.BorderoDepotID = null;
                        context.SaveChanges();

                    }

                    //verifions si ce bordero existe deja
                    BorderoDepot existBordero = context.BorderoDepots.Where(c => c.BorderoDepotID == BorderoDepotID).FirstOrDefault();

                    if (existBordero != null)
                    {
                        //suppression du bordero en question
                        context.BorderoDepots.Remove(existBordero);
                        context.SaveChanges();
                    }

                    

                    //EcritureSneack
                    IMouchar opSneak = new MoucharRepository(context);
                    bool res = opSneak.InsertOperation(UserConnected, "SUCCESS", "BORDERO " + existBordero.CodeBorderoDepot + " FOR INSURANCE " + existBordero.AssureurID, "DeleteCancelBordero", DateOperation, BranchID);
                    if (!res)
                    {
                        resDel = res;
                        throw new Exception("Une erreur s'est produite lors de la journalisation ");
                    }

                    resDel = true;
                    ts.Complete();

                    return resDel;
                }
                
            }
            catch (Exception e)
            {
                resDel = false;
                IMouchar opSneak = new MoucharRepository(context);
                bool res = opSneak.InsertOperation(UserConnected, "ERROR", "BORDERO ID " + BorderoDepotID , "DeleteCancelBordero", DateOperation, BranchID);
                if (!res)
                {
                    throw new Exception("Une erreur s'est produite lors de la journalisation ");
                }
                throw new Exception("Une erreur s'est produite lors de la creation du bordero de depot : " + e.Message);
            }
            
        }

        // ceci gere les ventes cash par prescription et les ventes cash directe
        /// <summary>
        /// 
        /// </summary>
        /// <param name="saledId"></param>
        /// <param name="TotalTTC"></param>
        public void updateCashCustomerValue (int saledId, double TotalTTC)
        {
            Sale sale = context.Sales.Find(saledId);

            /*int TotalTTC = Util.ExtraPrices(sale.SaleLines.Select(sl => sl.LineAmount).Sum(),
                                            c.RateReduction, c.RateDiscount, c.Transport, c.VatRate).TotalTTC;*/
            //CustomerValue v = new CustomerValue();

            int categoryId = sale.SaleLines.FirstOrDefault().Product.CategoryID;
            LensCategory lc = context.LensCategories.Where(c=>c.CategoryID== categoryId).FirstOrDefault();

            CustomerValue v = Customer.getCustomerValue(TotalTTC, lc, false);

            Customer customer = context.Customers.Find(sale.CustomerID);

            if (v == CustomerValue.VIP && customer.CustomerValue != CustomerValue.VIP)
            {
                customer.CustomerValue = CustomerValue.VIP;
            }
            customer.LastCustomerValue = v;

            context.SaveChanges();
        }

        public void updateInsuredCustomerValue(int customerOrderId)
        {

        }




    }
}

