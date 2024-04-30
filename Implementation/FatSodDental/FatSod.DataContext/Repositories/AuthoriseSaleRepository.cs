

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

namespace FatSod.DataContext.Repositories
{
    public class AuthoriseSaleRepository : RepositorySupply<AuthoriseSale>, IAuthoriseSale
    {
        public AuthoriseSaleRepository()
        {
        }

        public AuthoriseSaleRepository(EFDbContext ctx)
            : base(ctx)
        {
            this.context = ctx;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authsale"></param>
        /// <param name="HeureVente"></param>
        /// <param name="UserConect"></param>
        /// <param name="IsInHouseCustomer"></param>
        /// <returns></returns>
        public AuthoriseSale SaveChanges(AuthoriseSale authsale,String HeureVente,int UserConect, bool IsInHouseCustomer)
        {
            //Begin of transaction
           
                try
                {
                    using (TransactionScope ts = new TransactionScope())
                    {
                        //ajout de lheure de la vente
                        string[] tisys = HeureVente.Split(new char[] { ':' });
                        DateTime date = authsale.SaleDate;
                        date = date.AddHours(Convert.ToDouble(tisys[0]));
                        date = date.AddMinutes(Convert.ToDouble(tisys[1]));
                        date = date.AddSeconds(Convert.ToDouble(tisys[2]));
                        //we create a new command
                        authsale.SaleDateHours = date;
                        int defaultseller = 0;
                        if (!authsale.SellerID.HasValue)
                        {
                            GlobalPerson gb = context.GlobalPeople.Where(g => g.Name == "HOUSE" && g.Description == "CUSTOMER").FirstOrDefault();
                            if (gb==null)
                            {
                                throw new Exception("Please Create user HOUSE CUSTOMER before you proceed");
                            }
                            defaultseller = gb.GlobalPersonID;
                        }

                        if (authsale.ConsultDilPrescID.HasValue && authsale.ConsultDilPrescID.Value > 0)
                        {
                            ConsultDilPresc consultDilPresc = context.ConsultDilPrescs.SingleOrDefault(c => c.ConsultDilPrescID == authsale.ConsultDilPrescID);
                            authsale.GestionnaireID = consultDilPresc.Consultation.GestionnaireID;
                        }
                        authsale.SellerID = (authsale.SellerID.HasValue) ? authsale.SellerID : defaultseller; 

                        authsale = PersistSale(authsale, UserConect, IsInHouseCustomer);
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
                    bool res = opSneak.InsertOperation(UserConect, "ERROR", "SALE-REFERENCE " + authsale.SaleReceiptNumber + " FOR CUSTOMER " + authsale.PoliceAssurance, "PersistSale", authsale.SaleDate, authsale.BranchID);
                    if (!res)
                    {
                        throw new Exception("Une erreur s'est produite lors de la journalisation ");
                    }
                    throw new Exception("Une erreur s'est produite lors de la vente : " + "e.Message = " + e.Message );
                }
                return authsale;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="authoriseSale"></param>
        /// <param name="UserConect"></param>
        /// <param name="IsInHouseCustomer"></param>
        /// <returns></returns>
        public AuthoriseSale PersistSale(AuthoriseSale authoriseSale, int UserConect, bool IsInHouseCustomer)
        {

            bool res = false;
            double saleAmount = 0;
           try
            { 
            Customer customerEntity = new Customer();
            if (authoriseSale.CustomerID == null)
            {
                IPerson _personRepository = new PersonRepository();
                IAccount _accountRepository = new AccountRepository();
                ITransactNumber _transactRepository = new TransactNumberRepository();

                //fabrication du nvo cpte
                CollectifAccount colAcct = context.CollectifAccounts.Where(c => c.AccountingSection.AccountingSectionCode == CodeValue.Accounting.DefaultCodeAccountingSection.CODECLIENT).FirstOrDefault();
                int AccountID = _accountRepository.GenerateAccountNumber(colAcct.CollectifAccountID, authoriseSale.CustomerName + " " + Resources.UIAccount, false).AccountID;
                Sex sexe = context.Sexes.FirstOrDefault();
                Adress adresse = context.Adresses.FirstOrDefault();

                string Customernumber = _transactRepository.GenerateUniqueCIN();

                Adress NewAdress = new Adress()
                {
                    AdressPhoneNumber=authoriseSale.Remarque,
                    AdressCellNumber = authoriseSale.Remarque,
                    AdressFullName= adresse.AdressFullName,
                    AdressEmail=adresse.AdressEmail,
                    AdressWebSite= adresse.AdressWebSite,
                    AdressPOBox= adresse.AdressPOBox,
                    AdressFax= adresse.AdressFax,
                    QuarterID= adresse.QuarterID
                };
                Customer customer = new Customer()
                {
                    AccountID = AccountID,
                    Name = authoriseSale.CustomerName,
                    SexID = sexe.SexID,
                    CNI = (authoriseSale.CNI==null || authoriseSale.CNI=="") ? authoriseSale.CustomerName : authoriseSale.CNI,
                    Adress = NewAdress,
                    Dateregister = authoriseSale.SaleDate,
                    CustomerNumber = Customernumber,
                    DateOfBirth=authoriseSale.DateOfBirth,
                    IsInHouseCustomer= IsInHouseCustomer,
                    IsBillCustomer=false,
                    PreferredLanguage = authoriseSale.PreferredLanguage
                };

                customerEntity = (Customer)_personRepository.Create2(customer, UserConect, authoriseSale.SaleDate, authoriseSale.BranchID);
                
            }
            else
            {
                customerEntity= context.Customers.Find(authoriseSale.CustomerID.Value);
                //update de l'adresse du client
                if (!customerEntity.DateOfBirth.HasValue)
                {
                    if (authoriseSale.DateOfBirth.Day==1 && authoriseSale.DateOfBirth.Month==1 && (authoriseSale.DateOfBirth.Year==1 || authoriseSale.DateOfBirth.Year==1900))
                    {
                        throw new Exception("Error : Please contact the reception to define the date of birth of this customer before you proceed ");
                    }
                    else
                    {
                        customerEntity.DateOfBirth = authoriseSale.DateOfBirth;
                    }
                    
                }

                // Correction de Bug - On ne doit pas modifier le numero de telephone du client lors de la commande d'une dilatation
                // ou pour OtherSale
                if (!authoriseSale.IsDilatation && !authoriseSale.IsOtherSale)
                {
                    customerEntity.Adress.AdressPhoneNumber = authoriseSale.Remarque;
                    customerEntity.Adress.AdressCellNumber = authoriseSale.Remarque;
                }
                context.SaveChanges();
                //Adress Updateadresse = context.Adresses.Find(customerEntity.AccountID);
                //Updateadresse.AdressCellNumber = sale.Remarque;
                //Updateadresse.AdressPhoneNumber = sale.Remarque;
                //context.SaveChanges();
            }
            if (customerEntity == null)
            {
                throw new Exception("Erreur : Please you must create the Customer Before Proceed ");
            }
            //this is variable is saleID after determine type of payment method
            int authoriseSaleID = 0;
            //sale.OperatorID = UserConect;
            authoriseSale.PostByID = UserConect;
            int MaxbarCode = 0;
            //generation du numero de recu
            if (context.AuthoriseSales.Count() <= 0)
            {
                MaxbarCode = 0;
            }
            else
            {
                MaxbarCode = context.AuthoriseSales.Where(b => b != null).Max(b => b.CompteurFacture);
            }

            //verification de la longueur du code de l'operation
            if (MaxbarCode == 0)
            {
                MaxbarCode = 0;
            }
            int newCompteur = MaxbarCode + 1;
            string Reference = (newCompteur.ToString().Length < 6) ? newCompteur.ToString().PadLeft(6, '0') : newCompteur.ToString();
            authoriseSale.SaleReceiptNumber = "VPTS "+Reference; 
            authoriseSale.CompteurFacture = newCompteur;
            authoriseSale.CustomerID = customerEntity.GlobalPersonID;


            //persistance

            
            AuthoriseSale authoriseSaleToSave = new AuthoriseSale();

            //ces 2 lignes qui suivent permettent de transformer un ProductModel en Product 
            //create a Map
            Mapper.CreateMap<AuthoriseSale, AuthoriseSale>();
            //use Map
            authoriseSaleToSave = Mapper.Map<AuthoriseSale>(authoriseSale);
            authoriseSaleToSave.AuthoriseSaleLines = null;

            authoriseSaleToSave = context.AuthoriseSales.Add(authoriseSaleToSave);
            context.SaveChanges();
            authoriseSaleID = authoriseSaleToSave.AuthoriseSaleID;

            
            int ProductId = 0;
            //bool isFrame = false;
            //vente sans commande ou vente directe
            foreach (AuthoriseSaleLine authoriseSaleLine in authoriseSale.AuthoriseSaleLines)
            {
                if (authoriseSaleLine.Product.ProductID <=0 )
                { //isFrame = false;

                    //persistance du nouvo verre
                    Lens ProductLens = PersistAuthoriseSaleLine(authoriseSaleLine);
                    ProductId = ProductLens.ProductID;
                }
                else
                { //isFrame = true;
                        ProductId = authoriseSaleLine.Product.ProductID;
                }

                AuthoriseSaleLine authoriseSaleLineToSave = new AuthoriseSaleLine()
                {
                    LocalizationID = authoriseSaleLine.LocalizationID,
                    ProductID = ProductId,
                    AuthoriseSaleID = authoriseSaleID,
                    LineQuantity = authoriseSaleLine.LineQuantity,
                    LineUnitPrice = authoriseSaleLine.LineUnitPrice,
                    OeilDroiteGauche = authoriseSaleLine.OeilDroiteGauche,
                    SpecialOrderLineCode = authoriseSaleLine.SpecialOrderLineCode,
                    marque = authoriseSaleLine.marque,
                    reference = authoriseSaleLine.reference,
                    SupplyingName=authoriseSaleLine.SupplyingName,
                    isGift = authoriseSaleLine.isGift,
                    LensNumberSphericalValue = authoriseSaleLine.LensNumberSphericalValue,
                    LensNumberCylindricalValue = authoriseSaleLine.LensNumberCylindricalValue,
                    Axis = authoriseSaleLine.Axis,
                    Addition = authoriseSaleLine.Addition,
                    NumeroSerie=authoriseSaleLine.NumeroSerie,
                    isCommandGlass=authoriseSaleLine.isCommandGlass,
                    IsVIPRoom = authoriseSaleLine.IsVIPRoom
                };
                saleAmount += authoriseSaleLine.LineQuantity * authoriseSaleLine.LineUnitPrice;
                context.AuthoriseSaleLines.Add(authoriseSaleLineToSave);
                
                context.SaveChanges();
            }

            if (authoriseSale.ConsultDilPrescID.HasValue && authoriseSale.ConsultDilPrescID.Value > 0)
            {
                    if (authoriseSale.IsDilatation)
                    {
                        //check if authorize dilatation
                        ConsultDilatation consuldilatation = context.ConsultDilatations.Where(c => c.ConsultDilPrescID == authoriseSale.ConsultDilPrescID).FirstOrDefault();
                        if (consuldilatation != null)
                        {
                            consuldilatation.isAuthoriseSale = true;
                            context.SaveChanges();
                        }
                    }
                    
                    //update validate prescrition
                    ConsultLensPrescription consultprescription = context.ConsultLensPrescriptions.Where(c=> c.ConsultDilPrescID== authoriseSale.ConsultDilPrescID).FirstOrDefault();
                    if (consultprescription!=null)
                    {
                        consultprescription.isAuthoriseSale = true;
                        context.SaveChanges();
                    }
                    
            }

                //EcritureSneack
                IMouchar opSneak = new MoucharRepository(context);
            res = opSneak.InsertOperation(UserConect, "SUCCESS", "AuthoriseSALE-REFERENCE " + authoriseSale.SaleReceiptNumber + " FOR CUSTOMER " + authoriseSale.PoliceAssurance, "PersistAuthoriseSale", authoriseSale.SaleDate, authoriseSale.BranchID);
            if (!res)
            {
                throw new Exception("Une erreur s'est produite lors de la journalisation ");
            }
            authoriseSale.AuthoriseSaleID = authoriseSaleID;
            }
            catch (Exception e)
            {
                
                throw new Exception("Une erreur : " + e.Message);
            }
                
            return authoriseSale;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="saleLine"></param>
        /// <returns></returns>
        public OrderLens PersistCustomerOrderLine(AuthoriseSaleLine saleLine)
        {
            Product currentProduct = LensConstruction.GetOrderLensByCustOrdLine(saleLine, this.context);
            currentProduct = CreateOrderLens((OrderLens)currentProduct);
            return (OrderLens)currentProduct;
        }
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

            //currentProduct.AccountID = (from a in context.Accounts where a.CollectifAccountID==colAccount.CollectifAccountID
            //                                select a).FirstOrDefault().AccountID;
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


        public Lens PersistAuthoriseSaleLine(AuthoriseSaleLine saleLine)
        {
            Product currentProduct = LensConstruction.GetOrderLensByCustOrdLine(saleLine, this.context);
            currentProduct = LensConstruction.CreateLens((Lens)currentProduct, this.context);
            return (Lens)currentProduct;
        }

        //1-Création du Produit s'il n'existe pas
        





    }
}

