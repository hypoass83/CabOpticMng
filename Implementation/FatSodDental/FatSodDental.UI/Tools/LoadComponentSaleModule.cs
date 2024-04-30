using System;
using System.Collections.Generic;
using System.Linq;
using FatSod.DataContext.Concrete;
using FatSod.Security.Entities;
using Ext.Net;
using FatSod.DataContext.Initializer;
using FatSod.Ressources;
using FatSod.Supply.Entities;
using FatSod.DataContext.Repositories;
using FatSod.Supply.Abstracts;
using FatSod.Budget.Entities;

namespace FatSodDental.UI.Tools
{
    public static partial class LoadComponent
    {
        /*returns all paiement mode*/
        public static List<ListItem> PaymentMethods
        {
            get
            {
                List<ListItem> paymentMethodList = PurchasePaymentMethods;
                //ListItem savingAccount = new ListItem(Resources.SavingAccount, CodeValue.Supply.DepositReason.SavingAccount);
                //paymentMethodList.Add(savingAccount);
                return paymentMethodList;
            }
        }

        /*returns all paiement mode*/
        public static List<ListItem> SalePaymentMethods
        {
            get
            {
                List<ListItem> paymentMethodList = new List<ListItem>();
                ListItem cash = new ListItem(Resources.CASH, CodeValue.Accounting.DefaultCodeAccountingSection.CODECAIS);
                ListItem bank = new ListItem(Resources.BANK, CodeValue.Accounting.DefaultCodeAccountingSection.CODEBANK);
                ListItem savingAccount = new ListItem(Resources.SavingAccount, CodeValue.Supply.DepositReason.SavingAccount);
                paymentMethodList.Add(cash);
                paymentMethodList.Add(bank);
                paymentMethodList.Add(savingAccount);
                return paymentMethodList;
            }
        }

        /*returns all paiement mode with Insurance*/
        public static List<ListItem> SaleAllPaymentMethods
        {
            get
            {
                List<ListItem> paymentMethodList = new List<ListItem>();
                ListItem cash = new ListItem(Resources.CASH, CodeValue.Accounting.DefaultCodeAccountingSection.CODECAIS);
                ListItem bank = new ListItem(Resources.BANK, CodeValue.Accounting.DefaultCodeAccountingSection.CODEBANK);
                ListItem Insurance = new ListItem(Resources.Insurance, CodeValue.Supply.DepositReason.Insurance);
                paymentMethodList.Add(cash);
                paymentMethodList.Add(bank);
                paymentMethodList.Add(Insurance);
                return paymentMethodList;
            }
        }

        /*returns all paiement mode*/
        public static List<ListItem> Banks
        {
            get
            {
                List<ListItem> paymentMethodList = new List<ListItem>();
                context = new EFDbContext();

                List<Bank> banks = context.PaymentMethods.OfType<Bank>().ToList();
                foreach (Bank bk in banks)
                {
                    paymentMethodList.Add( new ListItem ( bk.Name, bk.ID) );
                }

                return paymentMethodList;
            }
        }

        public static List<ListItem> PurchasePaymentMethods
        {
            get
            {
                List<ListItem> paymentMethodList = new List<ListItem>();
                ListItem cash = new ListItem(Resources.CASH, CodeValue.Accounting.DefaultCodeAccountingSection.CODECAIS);
                ListItem bank = new ListItem(Resources.BANK, CodeValue.Accounting.DefaultCodeAccountingSection.CODEBANK);
                ListItem checque = new ListItem(Resources.POSTALCHECK, CodeValue.Accounting.DefaultCodeAccountingSection.CODEPOSTCHECK);
                ListItem savingAccount = new ListItem(Resources.SavingAccount, CodeValue.Supply.DepositReason.SavingAccount);
                ListItem credit = new ListItem(CodeValue.Accounting.DefaultCodeAccountingSection.Credit, CodeValue.Accounting.DefaultCodeAccountingSection.Credit);
                paymentMethodList.Add(cash);
                paymentMethodList.Add(bank);
                paymentMethodList.Add(savingAccount);
                paymentMethodList.Add(credit);
                return paymentMethodList;
            }
        }

        public static List<ListItem> GetPersonType
        {
            get
            {
                List<ListItem> GetPersonTypeList = new List<ListItem>();
                ListItem physical = new ListItem("Physical", 1);
                ListItem moral = new ListItem("Moral", 2);

                GetPersonTypeList.Add(physical);
                GetPersonTypeList.Add(moral);
                return GetPersonTypeList;
            }
        }

        public static List<ListItem> DepositReasonsStockLens
        {
             get
            {
                List<ListItem> depositReasons = new List<ListItem>();
                ListItem salePayment = new ListItem(Resources.SalePayment, CodeValue.Supply.DepositReason.SalePayment);
               
                depositReasons.Add(salePayment);
                return depositReasons;
            }

        }

        public static List<ListItem> DepositReasonsSpecialOrder
        {
            get
            {
                List<ListItem> depositReasons = new List<ListItem>();
                ListItem SpecialOrderPayment = new ListItem(Resources.SpecialOrderPayment, CodeValue.Supply.DepositReason.SpecialOrderPayment);
                depositReasons.Add(SpecialOrderPayment);

                return depositReasons;
            }

        }
        /*======= Return Customer listItem */
        public static List<ListItem> Customers
        {
            get
            {
                context = new EFDbContext();
                List<ListItem> customersList = new List<ListItem>();
                foreach (Customer customer in context.People.OfType<Customer>().Where(c=>c.Name.ToLower()!="default").ToArray())
                {
                    string itemLabel = "";
                   
                    //si le client est une personne physique, on renvoie son nom et son prénom, si c'est une entreprise, on renvoie son nom
                    itemLabel = customer.Name + " " + customer.Description ;

                    customersList.Add(new ListItem(itemLabel, customer.GlobalPersonID));
                }
                return customersList;
            }
        }

        public static List<ListItem> Suppliers
        {
            get
            {
                context = new EFDbContext();
                List<ListItem> suppliersList = new List<ListItem>();
                foreach (Supplier supplier in context.Suppliers.ToArray())
                {
                    suppliersList.Add(new ListItem(supplier.SupplierFullName, supplier.GlobalPersonID));
                }
                return suppliersList;
            }
        }

        /*======= Return GenericProducts listItem */
        public static List<ListItem> Products
        {
            get
            {
                context = new EFDbContext();
                List<ListItem> productsList = new List<ListItem>();
                try
                {
                    foreach (ProductLocalization product in context.ProductLocalizations.Where(pl=>pl.ProductLocalizationStockQuantity > 0).ToList())
                    {
                        string productLabel = (product.Product is Lens) ? product.Product.ProductCode : product.Product.ProductLabel;
                        productsList.Add(new ListItem(productLabel, product.ProductID));
                    }
                }
                catch (Exception)
                {
                    throw;
                }

                return productsList;
            }
        }

        /*======= Return Products listItem */
        public static List<ListItem> PurchaseProducts
        {
            get
            {
                context = new EFDbContext();
                IInventoryDirectory _invDirRepo = new InventoryDirectoryRepository();
                List<ListItem> productsList = new List<ListItem>();
                try
                {
                    List<Product> products = context.Products.ToList();
                    ////IL faut exclure tous les produits qui sont dans un dossier d'inventaire ayant le statut ouvers ou en cours
                    //List<Product> lockedProducts = _invDirRepo.LockedProducts();
                    //products.Except(lockedProducts);

                    foreach (Product product in products)
                    {
                        string productLabel = (product is Lens) ? product.ProductCode : product.ProductLabel;
                        productsList.Add(new ListItem(productLabel, product.ProductID));
                    }
                }
                catch (Exception)
                {
                    throw;
                }

                return productsList;
            }
        }

        /*======= Return Localizations listItem */
        public static List<ListItem> Localizations
        {
            get
            {
                context = new EFDbContext();
                List<ListItem> localizationsList = new List<ListItem>();
                foreach (Localization localization in context.Localizations.ToArray())
                {
                    localizationsList.Add(new ListItem(localization.LocalizationCode, localization.LocalizationID));
                }
                return localizationsList;
            }
        }

        public static List<ListItem> GetAllStockedLocations()
        {
            context = new EFDbContext();
            List<Localization> magasin = context.Localizations.ToList();
            List<ListItem> localizationsList = new List<ListItem>();
            foreach (Localization Loc in magasin)
            {
                localizationsList.Add(new ListItem(Loc.LocalizationCode, Loc.LocalizationID));
            }
            //IEqualityComparer<ProductLocalization> locationComparer = new GenericComparer<ProductLocalization>("LocationCode");

            //List<ProductLocalization> dataTmp = context.ProductLocalizations.Where(pl => pl.ProductLocalizationStockQuantity > 0).ToList();
            //dataTmp = dataTmp.Distinct(locationComparer).ToList();

            //List<ListItem> localizationsList = new List<ListItem>();
            //foreach (ProductLocalization prodLoc in dataTmp)
            //{
            //    localizationsList.Add(new ListItem(prodLoc.Localization.LocalizationCode, prodLoc.Localization.LocalizationID));
            //}
            return localizationsList;

        }

        public static List<Sale> AllSales
        {
            get
            {
                context = new EFDbContext();
                return context.Sales.ToList();
            }
        }

        /// <summary>
        /// Cette méthode retourne la liste des numéros n'ayant pas l'addition
        /// </summary>
        /// <returns></returns>
        public static List<LensNumber> GetAllSVNumbers()
        {
            context = new EFDbContext();
            List<LensNumber> lstLens = context.LensNumbers.AsNoTracking().Where(ln => ln.LensNumberAdditionValue == null || ln.LensNumberAdditionValue.Length <= 0).ToList();
            return lstLens;
        }

        /// <summary>
        /// Cette méthode retourne la liste des numéros de la base de données
        /// </summary>
        /// <returns></returns>
        public static List<LensNumber> GetAllNumbers()
        {
            context = new EFDbContext();
            List<LensNumber> lstLens= context.LensNumbers.AsNoTracking().Where(ln => ln.LensNumberAdditionValue.Length > 0).ToList();
            return lstLens;
        }

       

       

        public static List<BudgetConsumption> AllExpenseBudgToValidate
        {
            get
            {
                context = new EFDbContext();
                return context.BudgetConsumptions.Where(b => !b.isValidated).ToList();
            }
        }
        public static List<SupplierOrder> AllSupplierOrders
        {
            get
            {
                context = new EFDbContext();
                return context.SupplierOrders.Where(c => !c.IsDelivered).ToList();
            }
        }

        //all users
        public static List<User> UsersForStore(User currentUser)
        {

            context = new EFDbContext();
            List<UserBranch> userBranchListTmp1 = context.UserBranches.ToList();
            List<UserBranch> userBranchListTmp = context.UserBranches.Where(ub => ub.UserID == currentUser.GlobalPersonID).ToList();
            List<User> userListFinal = new List<FatSod.Security.Entities.User>();
            foreach (UserBranch ub1 in userBranchListTmp)
            {
                List<UserBranch> userBrTmp = context.UserBranches.Where(ub => ub.BranchID == ub1.BranchID).AsParallel().ToList();
                foreach (UserBranch ub2 in userBrTmp)
                {
                    User u = context.People.OfType<User>().FirstOrDefault(uu => uu.GlobalPersonID == ub2.UserID);
                    if (u != null && u.UserAccessLevel < currentUser.UserAccessLevel)
                    {
                        userListFinal.Add(ub2.User);
                    }
                }
            }
            return userListFinal;

        }
        public static List<Customer> GetCustomersForStore
        {
            get
            {
                context = new EFDbContext();
                return context.People.OfType<Customer>().ToList();
            }
        }
        public static List<Assureur> GetAssurancesForStore
        {
            get
            {
                context = new EFDbContext();
                return context.People.OfType<Assureur>().ToList();
            }
        }
        public static List<ListItem> GetAllAssureus()
        {
            context = new EFDbContext();
            List<Assureur> assureur = context.Assureurs.Where(ass=>ass.Name.ToLower()!="default").ToList();
            List<ListItem> assureurList = new List<ListItem>();
            foreach (Assureur ass in assureur)
            {
                assureurList.Add(new ListItem(ass.Name, ass.GlobalPersonID));
            }
            return assureurList;

        }
    }
}