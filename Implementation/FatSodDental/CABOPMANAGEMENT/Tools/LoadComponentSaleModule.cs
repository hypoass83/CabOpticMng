using System;
using System.Collections.Generic;
using System.Linq;
using FatSod.DataContext.Concrete;
using FatSod.Security.Entities;
using FatSod.DataContext.Initializer;
using FatSod.Ressources;
using FatSod.Supply.Entities;
using FatSod.DataContext.Repositories;
using FatSod.Supply.Abstracts;
using FatSod.Budget.Entities;

namespace CABOPMANAGEMENT.Tools
{
    public static partial class LoadComponent
    {
        /*returns all paiement mode*/
        public static List<String> PaymentMethods
        {
            get
            {
                List<String> paymentMethodList = PurchasePaymentMethods;
                
                return paymentMethodList;
            }
        }

       
        /*returns all paiement mode*/
        public static List<String> SalePaymentMethods
        {
            get
            {
                List<String> paymentMethodList = new List<String>();
                String cash = CodeValue.Accounting.DefaultCodeAccountingSection.CODECAIS;
                String bank = CodeValue.Accounting.DefaultCodeAccountingSection.CODEBANK;
                String savingAccount = CodeValue.Supply.DepositReason.SavingAccount;

                paymentMethodList.Add(cash);
                paymentMethodList.Add(bank);
                paymentMethodList.Add(savingAccount);
                return paymentMethodList;
            }
        }

        /*returns all paiement mode*/
        public static List<Bank> Banks
        {
            get
            {
                List<Bank> paymentMethodList = new List<Bank>();
                context = new EFDbContext();

                List<Bank> banks = context.PaymentMethods.OfType<Bank>().ToList();
                foreach (Bank bk in banks)
                {
                    paymentMethodList.Add(new Bank { Name = bk.Name, ID=bk.ID });
                }

                return paymentMethodList;
            }
        }

        public static List<String> PurchasePaymentMethods
        {
            get
            {
                List<String> paymentMethodList = new List<String>();
                String cash = CodeValue.Accounting.DefaultCodeAccountingSection.CODECAIS;
                String bank = CodeValue.Accounting.DefaultCodeAccountingSection.CODEBANK;
                //String checque = CodeValue.Accounting.DefaultCodeAccountingSection.CODEPOSTCHECK;
                String savingAccount = CodeValue.Supply.DepositReason.SavingAccount;
                String credit =  CodeValue.Accounting.DefaultCodeAccountingSection.Credit;
                paymentMethodList.Add(cash);
                paymentMethodList.Add(bank);
                paymentMethodList.Add(savingAccount);
                paymentMethodList.Add(credit);
                return paymentMethodList;
            }
        }

        public static List<String> GetPersonType
        {
            get
            {
                List<String> GetPersonTypeList = new List<String>();
                String physical = "Physical";
                String moral = "Moral";

                GetPersonTypeList.Add(physical);
                GetPersonTypeList.Add(moral);
                return GetPersonTypeList;
            }
        }

        public static List<String> DepositReasonsStockLens
        {
             get
            {
                List<String> depositReasons = new List<String>();
                String salePayment = CodeValue.Supply.DepositReason.SalePayment;
               
                depositReasons.Add(salePayment);
                return depositReasons;
            }

        }

        public static List<String> DepositReasonsSpecialOrder
        {
            get
            {
                List<String> depositReasons = new List<String>();
                String SpecialOrderPayment = CodeValue.Supply.DepositReason.SpecialOrderPayment;
                depositReasons.Add(SpecialOrderPayment);

                return depositReasons;
            }

        }
        /*======= Return Customer listItem */
        public static List<Customer> Customers
        {
            get
            {
                context = new EFDbContext();
                List<Customer> customersList = new List<Customer>();
                foreach (Customer customer in context.People.OfType<Customer>().ToArray())
                {
                    string itemLabel = "";
                    
                    //si le client est une personne physique, on renvoie son nom et son prénom, si c'est une entreprise, on renvoie son nom
                    itemLabel = customer.Name + 
                        (  customer.Description );

                    customersList.Add(new Customer { Name=itemLabel, GlobalPersonID = customer.GlobalPersonID });
                }
                return customersList;
            }
        }

        public static List<Supplier> Suppliers
        {
            get
            {
                context = new EFDbContext();
                List<Supplier> suppliersList = new List<Supplier>();
                foreach (Supplier supplier in context.Suppliers.ToArray())
                {
                    suppliersList.Add(new Supplier { Name=supplier.SupplierFullName, GlobalPersonID = supplier.GlobalPersonID });
                }
                return suppliersList;
            }
        }

        /*======= Return GenericProducts listItem */
        public static List<Product> Products
        {
            get
            {
                context = new EFDbContext();
                List<Product> productsList = new List<Product>();
                try
                {
                    foreach (ProductLocalization product in context.ProductLocalizations.Where(pl=>pl.ProductLocalizationStockQuantity > 0).ToList())
                    {
                        string productLabel = (product.Product is Lens) ? product.Product.ProductCode : product.Product.ProductLabel;
                        productsList.Add(new Product { ProductLabel = productLabel, ProductID = product.ProductID });
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
        public static List<Product> PurchaseProducts
        {
            get
            {
                context = new EFDbContext();
                IInventoryDirectory _invDirRepo = new InventoryDirectoryRepository();
                List<Product> productsList = new List<Product>();
                try
                {
                    List<Product> products = context.Products.ToList();
                    ////IL faut exclure tous les produits qui sont dans un dossier d'inventaire ayant le statut ouvers ou en cours
                    //List<Product> lockedProducts = _invDirRepo.LockedProducts();
                    //products.Except(lockedProducts);

                    foreach (Product product in products)
                    {
                        string productLabel = (product is Lens) ? product.ProductCode : product.ProductLabel;
                        productsList.Add(new Product { ProductLabel=productLabel, ProductID = product.ProductID });
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
        public static List<Localization> Localizations
        {
            get
            {
                context = new EFDbContext();
                List<Localization> localizationsList = new List<Localization>();
                foreach (Localization localization in context.Localizations.ToArray())
                {
                    localizationsList.Add(new Localization { LocalizationCode=localization.LocalizationCode, LocalizationID = localization.LocalizationID });
                }
                return localizationsList;
            }
        }

        public static List<Localization> GetAllStockedLocations()
        {
            context = new EFDbContext();
            List<Localization> magasin = context.Localizations.ToList();
            List<Localization> localizationsList = new List<Localization>();
            foreach (Localization Loc in magasin)
            {
                localizationsList.Add(new Localization { LocalizationCode = Loc.LocalizationCode, LocalizationID=Loc.LocalizationID });
            }
            
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
        public static List<LensNumber> GetAllSVNumbers(string filter)
        {
            context = new EFDbContext();
            //List<LensNumber> lstLens = context.LensNumbers.AsNoTracking().Where(ln => ln.LensNumberAdditionValue == null || ln.LensNumberAdditionValue.Length <= 0).StartsWith(filter.ToLower().Trim()).ToList();
            List<LensNumber> lstLens = context.LensNumbers/*.AsNoTracking()*/.Where(ln => (ln.LensNumberAdditionValue == null || ln.LensNumberAdditionValue.Length <= 0) && ln.LensNumberDescription.StartsWith(filter.ToLower().Trim())).ToList();
            return lstLens;
        }

        /// <summary>
        /// Cette méthode retourne la liste des numéros de la base de données
        /// </summary>
        /// <returns></returns>
        public static List<LensNumber> GetAllNumbers(string filter)
        {
            context = new EFDbContext();
            List<LensNumber> lstLens= context.LensNumbers/*.AsNoTracking()*/.Where(ln => ln.LensNumberAdditionValue.Length > 0 && ln.LensNumberDescription.StartsWith(filter.ToLower().Trim())).ToList();
            return lstLens;
        }

        public static List<Sale> allSales (int currentBD, DateTime BDDateOperation)
        {
            context = new EFDbContext();
           
            //retourne la liste des ventes qui ont deja ete valide
            List<Sale> allSales = (from sal in context.Sales
                                    where (sal.BranchID == currentBD && sal.SaleDate == BDDateOperation.Date)
                                    select sal).ToList();
            return allSales;
            
        }

        public static List<CustomerOrder> AllCommandsForStore
        {
            get
            {
                context = new EFDbContext();
                return context.CustomerOrders.Where(c =>  !c.IsDelivered).ToList();
            }
        }

        public static List<CustomerOrder> AllCommandsForStoreView (int startIndex, int count, string sorting)
        {
           
            context = new EFDbContext();
            IEnumerable<CustomerOrder> query = context.CustomerOrders.Where(c =>  !c.IsDelivered).ToList();
            if (string.IsNullOrEmpty(sorting) || sorting.Equals("CustomerName ASC"))
            {
                query = query.OrderBy(p => p.CustomerName);
            }

            return count > 0
                       ? query.Skip(startIndex).Take(count).ToList() //Paging
                       : query.ToList(); //No paging

            //return context.CustomerOrders.Where(c => !(c is SpecialOrder) && !c.IsDelivered).ToList();
            
        }

        public static List<CustomerOrder> AllOrderLensOrders
        {
            get
            {
                context = new EFDbContext();
                return context.CustomerOrders.Where(c => !c.IsDelivered).ToList();
            }
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
                    if (u != null && u.UserAccessLevel < currentUser.UserAccessLevel && u.IsConnected && u.UserAccountState)
                    {
                        userListFinal.Add(ub2.User);
                    }
                }
            }
            return userListFinal;

        }

        public static List<object> GetSellers(int currentUserId)
        {
            context = new EFDbContext();
            List<User> employees = context.UserBranches.Where(ub => ub.BranchID == currentUserId && ub.User.IsSeller)
                .Select(u => u.User).OrderBy(s => s.Name).ToList();
            List<object> sellers = new List<object>();
            
            foreach (User employee in employees)
            {
                sellers.Add(new { Name = employee.UserFullName, ID = employee.GlobalPersonID });
            }
            return sellers;
        }

        public static List<object> GetMarketters(int currentUserId)
        {
            context = new EFDbContext();
            List<User> employees = context.UserBranches.Where(ub => ub.BranchID == currentUserId && ub.User.IsMarketer)
                .Select(u => u.User).OrderBy(s => s.Name).ToList();
            List<object> marketters = new List<object>();

            foreach (User employee in employees)
            {
                marketters.Add(new { Name = employee.UserFullName, ID = employee.GlobalPersonID });
            }
            return marketters;
        }


        public static List<object> GetAllEmployees(int currentUserId, int userToRemoveId = 0)
        {
            context = new EFDbContext();
            List<User> employees = context.UserBranches.Where(ub => !ub.IsDeleted && ub.BranchID == currentUserId &&
                                    ub.User.GlobalPersonID != userToRemoveId)
                .Select(u => u.User).OrderBy(s => s.Name).ToList();
            List<object> marketters = new List<object>();

            foreach (User employee in employees)
            {
                marketters.Add(new { Name = employee.UserFullName, ID = employee.GlobalPersonID });
            }
            return marketters;
        }

        public static List<object> GetAllEmployeesByProfile(int currentUserId, int profileId)
        {
            context = new EFDbContext();
            List<User> employees = context.UserBranches.Where(ub => ub.BranchID == currentUserId &&
                                    ub.User.ProfileID == profileId)
                .Select(u => u.User).OrderBy(s => s.Name).ToList();
            List<object> marketters = new List<object>();

            foreach (User employee in employees)
            {
                marketters.Add(new { Name = employee.UserFullName, Id = employee.GlobalPersonID });
            }
            return marketters;
        }


        public static List<Customer> GetCustomersForStore
        {
            get
            {
                context = new EFDbContext();
                return context.Customers.Where(c=>!c.IsBillCustomer).ToList();
            }
        }
        public static List<Assureur> GetAssurancesForStore
        {
            get
            {
                context = new EFDbContext();
                return context.Assureurs.ToList();
            }
        }
    }
}