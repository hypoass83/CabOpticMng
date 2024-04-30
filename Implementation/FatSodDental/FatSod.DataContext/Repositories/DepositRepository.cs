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
using FastSod.Utilities.Util;
using System.Transactions;
using FatSod.DataContext.Initializer;
using FatSod.Security.Abstracts;
using System.Data.SqlClient;
using System.Data;

namespace FatSod.DataContext.Repositories
{
    public class DepositRepository : RepositorySupply<Deposit>, IDeposit
    {

        //protected EFDbContext context;
        public DepositRepository()
        {

            //context = new EFDbContext();
        }

        public DepositRepository(EFDbContext ctx)
            : base(ctx)
        {

            this.context = ctx;
        }

        public List<Sale> OtherCustomerAllUnPaidSales(Customer customer)
        {
            List<Sale> res = new List<Sale>();

            //liste de toutes les ventes
            List<Sale> sales1 = context.Sales.Where(s => s.CustomerID == customer.GlobalPersonID && !s.IsSpecialOrder /*&& !s.IsPaid*/).ToList();

            List<Sale> sales = new List<Sale>();

            foreach (Sale sale in sales1)
            {
                //calcul du montant déjà perçu pour la vente
                sale.SaleTotalPriceAdvance = this.SaleTotalPriceAdvance(sale);
                sale.SaleTotalPriceRemainder = this.SaleRemainder(sale);


                if (sale.SaleTotalPriceRemainder > 0)
                {//récupération des ventes qui n'ont pas encore été totalement réglées
                    Sale currentSale = this.ApplyExtraPrice(sale);
                    res.Add(currentSale);
                }

            }

            return res;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="deposit"></param>
        /// <param name="UserConect"></param>
        /// <returns></returns>
        public AllDeposit SaleDepositForInsured(Deposit deposit, int UserConect)
        {
            Boolean res = false;
            AllDeposit InsureDep = new AllDeposit();
            context = new EFDbContext();
            //Begin of transaction

            try
            {
                using (TransactionScope ts = new TransactionScope())
                {

                    //Montant déposé par le client

                    double TotAmount = deposit.Amount;
                    Customer cust = (deposit.CustomerID == null || deposit.CustomerID == 0) ? context.Customers.Where(c => c.Name.ToLower() == "default").FirstOrDefault() : context.Customers.Find(deposit.CustomerID);
                    if (cust == null) { throw new Exception("Please create default customer before proceed"); }
                    //ecriture ds la table generale des depots
                    AllDeposit alldeposits = new AllDeposit()
                    {
                        Amount = TotAmount,
                        AllDepositDate = deposit.DepositDate,
                        PaymentMethodID = deposit.PaymentMethodID,
                        // DIGITAL PAYMENT INFO
                        TransactionIdentifier = deposit.TransactionIdentifier,
                        DigitalAccountManagerId = deposit.DigitalAccountManagerId,
                        DeviseID = deposit.DeviseID,
                        CustomerID = cust.GlobalPersonID,//deposit.CustomerID,
                        Representant = deposit.Representant,
                        AllDepositReference = deposit.DepositReference,
                        BranchID = deposit.BranchID,
                        AllDepositReason = deposit.DepositReason,
                        IsSpecialOrder = false,
                        CustomerOrderID = deposit.CustomerOrderID,
                        OperatorID = UserConect
                    };
                    InsureDep = context.AllDeposits.Add(alldeposits);
                    context.SaveChanges();

                    /*
                    if (totalResteVente < resteMntVente)
                    {
                        //Ici, On fait une avance pour la vente
                        depositAmount = totalResteVente;

                        //Mise à jour de la vente pour indiquer que la vente a été avancée
                        currentSale.OldStatutSale = SalePurchaseStatut.Invoiced;// currentSale.StatutSale;
                        currentSale.StatutSale = SalePurchaseStatut.Advanced;
                        context.SaveChanges();

                    }
                    else // il s'agit du cas totalResteVente >= resteMntVente  
                    {
                        //Ici, on règle totalement la vente ou le montant restant de la vente
                        depositAmount = resteMntVente;
                        //Mise à jour de la vente pour indiquer que la vente a été totalement payée 
                        currentSale.OldStatutSale = (MontantDejaAdvance == 0) ? SalePurchaseStatut.Paid : SalePurchaseStatut.Advanced;// currentSale.StatutSale;
                        currentSale.StatutSale = SalePurchaseStatut.Paid;
                        currentSale.IsPaid = true;
                        context.SaveChanges();
                    }

                    //comptabilisation du paiement de la tranche par le client
                    Sale acountableSale = new Sale()
                    {
                        BranchID = deposit.BranchID,
                        CustomerID = cust.GlobalPersonID,
                        PaymentMethodID = deposit.PaymentMethodID,
                        SaleDate = deposit.DepositDate,
                        SaleID = depSale.SaleID,
                        TotalPriceTTC = TotalPriceTTC,// currentSale.TotalPriceTTC,
                        SaleReceiptNumber = depSale.SaleReceiptNumber,
                        SaleTotalPriceAdvance = depositAmount,
                        DeviseID = deposit.DeviseID,
                        StatutSale = currentSale.StatutSale,
                        OldStatutSale = currentSale.OldStatutSale,
                        MontantClientDeposit = depositAmount,
                        MontantTotalClientAdvance = MontantDejaAdvance

                    };
                    IAccountOperation opaccount = new AccountOperationRepository(context);
                    res = opaccount.ecritureComptableFinal(acountableSale);
                    if (!res)
                    {
                        //transaction.Rollback();
                        throw new Exception("Une erreur s'est produite lors de comptabilisation du dépôt pour le règlement de la vente ");
                    }
                    */
                    //mise a jour du cpteur du transact number
                    TransactNumber trn = context.TransactNumbers.SingleOrDefault(t => t.TransactNumberCode == "REDP");
                    if (trn != null)
                    {
                        //persistance du compteur de l'objet TransactNumber
                        trn.Counter = trn.Counter + 1;
                    }
                    context.SaveChanges();

                    //EcritureSneack
                    IMouchar opSneak = new MoucharRepository(context);
                    res = opSneak.InsertOperation(UserConect, "SUCCESS", " CUSTOMER " + deposit.Representant + " DEPOSIT REF " + deposit.DepositReference + " -AMOUNT- " + TotAmount, "SaleDeposit", deposit.DepositDate, deposit.BranchID);
                    if (!res)
                    {
                        throw new Exception("Une erreur s'est produite lors de la journalisation ");
                    }
                    res = true;
                    //transaction.Commit();
                    ts.Complete();
                }
            }
            catch (Exception e)
            {
                //If an errors occurs, we cancel all changes in database
                res = false;
                //transaction.Rollback();
                throw new Exception("Error : " + e.Message + " - " + e.StackTrace);
            }

            return InsureDep;

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="deposit"></param>
        /// <param name="UserConect"></param>
        /// <returns></returns>
        public Boolean SaleDepositCustomer(Deposit deposit, int UserConect)
        {
            Boolean res = false;
            double resteMntVente = 0d;
            double totalResteVente = 0d;
            //double totalAvance = 0d;
            double depositAmount = 0d;
            context = new EFDbContext();
            //Begin of transaction

            try
            {
                using (TransactionScope ts = new TransactionScope())
                {

                    //Montant déposé par le client
                    totalResteVente = deposit.Amount;
                    double TotAmount = deposit.Amount;

                    //recuperation des ventes correspondant au montant saisi par le user
                    Customer cust = context.Customers.Find(deposit.CustomerID);
                    List<Sale> sales = this.OtherCustomerAllUnPaidSales(cust).ToList();


                    foreach (Sale depSale in sales)
                    {

                        //recuperation du montant restant de la vente
                        resteMntVente = this.OtherSaleRemainder(depSale);

                        Sale currentSale = context.Sales.Find(depSale.SaleID);

                        if (totalResteVente < resteMntVente)
                        {
                            //Ici, On fait une avance pour la vente
                            depositAmount = totalResteVente;

                            //Mise à jour de la vente pour indiquer que la vente a été avancée
                            currentSale.OldStatutSale = SalePurchaseStatut.Invoiced;// currentSale.StatutSale;
                            currentSale.StatutSale = SalePurchaseStatut.Advanced;
                            context.SaveChanges();

                        }
                        else // il s'agit du cas totalResteVente >= resteMntVente  
                        {
                            //Ici, on règle totalement la vente ou le montant restant de la vente
                            depositAmount = resteMntVente;

                            //Mise à jour de la vente pour indiquer que la vente a été totalement payée 

                            currentSale.OldStatutSale = SalePurchaseStatut.Invoiced;// currentSale.StatutSale;
                            currentSale.StatutSale = SalePurchaseStatut.Paid;
                            currentSale.IsPaid = true;
                            context.SaveChanges();
                        }

                        //construction du customer slice
                        CustomerSlice customerSlice = new CustomerSlice();
                        customerSlice.DeviseID = deposit.DeviseID;
                        customerSlice.PaymentMethodID = deposit.PaymentMethodID;
                        customerSlice.SaleID = depSale.SaleID;
                        customerSlice.SliceAmount = depositAmount;// deposit.Amount,
                        customerSlice.SliceDate = deposit.DepositDate;
                        customerSlice.Representant = deposit.Representant;
                        customerSlice.isDeposit = true;
                        customerSlice.Reference = deposit.DepositReference;
                        customerSlice.OperatorID = UserConect;
                        //persistence de la tranche pour le règlement de la totalité ou partie de la vente.
                        context.CustomerSlices.Add(customerSlice);
                        context.SaveChanges();

                        double TotalPriceTTC = this.SaleBill(depSale);
                        //comptabilisation du paiement de la tranche par le client
                        Sale acountableSale = new Sale()
                        {
                            BranchID = deposit.BranchID,
                            CustomerID = deposit.CustomerID,
                            PaymentMethodID = deposit.PaymentMethodID,
                            SaleDate = deposit.DepositDate,
                            SaleID = depSale.SaleID,
                            TotalPriceTTC = TotalPriceTTC,// currentSale.TotalPriceTTC,
                            SaleReceiptNumber = depSale.SaleReceiptNumber,
                            SaleTotalPriceAdvance = depositAmount,
                            DeviseID = deposit.DeviseID,
                            StatutSale = currentSale.StatutSale,
                            OldStatutSale = currentSale.OldStatutSale
                        };
                        ////IAccountOperation opaccount = new AccountOperationRepository(context);
                        ////res = opaccount.ecritureComptableFinal(acountableSale);
                        ////if (!res)
                        ////{
                        ////    //transaction.Rollback();
                        ////    throw new Exception("Une erreur s'est produite lors de comptabilisation du dépôt pour le règlement de la vente ");
                        ////}

                        totalResteVente = totalResteVente - depositAmount;

                        //si le montant verse par le client est totalement use alors break
                        if (totalResteVente <= 0) break;
                    }

                    //On fait un dépôt d'épargne sur le compte du client
                    if (totalResteVente > 0)
                    {
                        ISavingAccount savingAccountRepository = new SavingAccountRepository(context);
                        deposit.Amount = totalResteVente;
                        savingAccountRepository.DoADeposit(deposit, true, UserConect);
                    }

                    //ecriture ds la table generale des depots
                    AllDeposit alldeposits = new AllDeposit()
                    {
                        Amount = TotAmount,
                        AllDepositDate = deposit.DepositDate,
                        PaymentMethodID = deposit.PaymentMethodID,
                        DeviseID = deposit.DeviseID,
                        CustomerID = deposit.CustomerID,
                        Representant = deposit.Representant,
                        AllDepositReference = deposit.DepositReference,
                        BranchID = deposit.BranchID,
                        AllDepositReason = deposit.DepositReason,
                        IsSpecialOrder = false,
                        OperatorID = UserConect
                    };
                    context.AllDeposits.Add(alldeposits);
                    context.SaveChanges();

                    //mise a jour du cpteur du transact number
                    TransactNumber trn = context.TransactNumbers.SingleOrDefault(t => t.TransactNumberCode == "REDP");
                    if (trn != null)
                    {
                        //persistance du compteur de l'objet TransactNumber
                        trn.Counter = trn.Counter + 1;
                    }
                    context.SaveChanges();

                    //EcritureSneack
                    IMouchar opSneak = new MoucharRepository(context);
                    res = opSneak.InsertOperation(UserConect, "SUCCESS", " CUSTOMER " + deposit.Representant + " DEPOSIT REF " + deposit.DepositReference + " -AMOUNT- " + TotAmount, "SaleDeposit", deposit.DepositDate, deposit.BranchID);
                    if (!res)
                    {
                        throw new Exception("Une erreur s'est produite lors de la journalisation ");
                    }
                    res = true;
                    //transaction.Commit();
                    ts.Complete();
                }
            }
            catch (Exception e)
            {
                //If an errors occurs, we cancel all changes in database
                res = false;
                //transaction.Rollback();
                throw new Exception("Error : " + e.Message + " - " + e.StackTrace);
            }

            return res;

        }
        /// <summary>
        /// retourne le montant total qui a déjà été payé sur un achat
        /// </summary>
        /// <param name="purchase"></param>
        /// <returns></returns>
        public double PurchaseTotalPriceAdvance(Purchase purchase)
        {
            double res = 0;
            List<SupplierSlice> payedSlices = context.SupplierSlices.Where(cs => cs.PurchaseID == purchase.PurchaseID).ToList();

            if (payedSlices != null && payedSlices.Count() > 0)
            {
                //res = context.SupplierSlices.Where(pur => pur.PurchaseID == purchase.PurchaseID).Select(ss => ss.SliceAmount).Sum();
                res = payedSlices.Select(ss => ss.SliceAmount).Sum();
            }

            return res;

        }

        /// <summary>
        /// retourne le montant total qui a déjà été payé sur une vente.
        /// C'est la somme des CustomerLice de la vente moins la somme des CustomerReturnSlice de la meme vente.
        /// C'est a dire que la somme avancée sur une vente 
        /// </summary>
        /// <param name="sale"></param>
        /// <returns></returns>
        public double SaleTotalPriceAdvance(Sale sale)
        {
            double res = 0;

            List<CustomerSlice> payedSlices = context.CustomerSlices.Where(cs => cs.SaleID == sale.SaleID).ToList();


            if (payedSlices != null && payedSlices.Count() > 0)
            {
                //Totalité des avances sur cette vente
                double realAdvance = payedSlices.Select(cs => cs.SliceAmount).Sum();

                //Argent déjà remboursé sur cette vente à cause des retours
                double returnAdvance = SalePaidReturn(sale);

                //Avance effective sur la vente
                res = realAdvance - returnAdvance;
            }

            return res;
        }

        /// <summary>
        /// Montant d'argent déjà remboursé au client à cause des retours ayant eu lieu sur la vente courante
        /// </summary>
        /// <param name="sale"></param>
        /// <returns></returns>
        public double SalePaidReturn(Sale sale)
        {
            double SaleTotalPriceReturn = 0;

            CustomerReturn custRet = context.CustomerReturns.SingleOrDefault(cr => cr.SaleID == sale.SaleID);

            //Cette vente a déjà subit au moins un retour
            if (custRet != null && custRet.CustomerReturnID > 0)
            {
                //Argent déjà versé pour ce retour
                List<CustomerReturnSlice> slices = context.CustomerReturnSlices.Where(crs => crs.CustomerReturnID == custRet.CustomerReturnID).ToList();

                SaleTotalPriceReturn = (slices != null && slices.Count > 0) ? slices.Select(crs => crs.SliceAmount).Sum() : 0;

            }

            return SaleTotalPriceReturn;
        }

        /// <summary>
        /// Cette méthode retourne la liste des clients qui ont au moins une vente qui n'a pas encore été totallement réglée
        /// </summary>
        /// <param name="branch">agence concernée</param>
        /// <returns>Liste des clients qui on une dette en vers l'entreprise</returns>
        public List<Customer> CustomersDebtors(Branch branch)
        {
            List<Customer> res = new List<Customer>();
            using (var context = new EFDbContext())
            {
                context.Configuration.ProxyCreationEnabled = false;
                context.Configuration.LazyLoadingEnabled = false;
                //récupération de toutes les ventes qui n'ont pas encore été totalement réglées de la branche
                List<Sale> sales = this.AllUnPaidSales().Where(sale => sale.BranchID == branch.BranchID).ToList();

                //1-Récupération des clients qui ont au moins une vente non totalement réglées
                //2-indication de la dette du client envers l'entreprise
                foreach (Sale sale in sales)
                {
                    Customer customer = new Customer();
                    customer = context.Customers.Find(sale.CustomerID);

                    if (res.Contains(customer))
                    {
                        continue;
                    }
                    customer.Debt = sales.Where(s => s.CustomerID == customer.GlobalPersonID).Select(sale1 => sale1.SaleTotalPriceRemainder).Sum();
                    res.Add(customer);
                }
            }

            return res;

        }

        /// <summary>
        /// cette méthode retourne la dette du client
        /// </summary>
        /// <param name="customer">Client dont on veut la dette </param>
        /// <returns>Dette du client envers l'entreprise</returns>
        public double CustomerDebtStockLens(Customer customer)
        {
            double res = 0;
            //using (var context = new EFDbContext())
            //{
            //récupération de toutes les ventes du client qui n'ont pas encore été totalement réglées
            //List<Sale> sales = this.UnPaidSales(customer);
            List<Sale> sales = this.CustomerAllUnPaidSalesStockLens(customer);
            //Récupération de la dette du client
            res = sales.Sum(sale1 => sale1.SaleTotalPriceRemainder);

            //Que faire des dettes liées verres de commande commandées mais non livrées
            //}

            return res;

        }
        /// <summary>
        /// cette méthode retourne la dette du client Avant une date
        /// </summary>
        /// <param name="customer">Client dont on veut la dette </param>
        /// <returns>Dette du client envers l'entreprise</returns>
        public double CustomerDebtStockLens(Customer customer, DateTime BeginDate)
        {
            double res = 0;

            //récupération de toutes les ventes du client qui n'ont pas encore été totalement réglées
            List<Sale> sales = this.CustomerAllUnPaidSalesStockLens(customer, BeginDate);
            //Récupération de la dette du client
            res = sales.Sum(sale1 => sale1.SaleTotalPriceRemainder);

            return res;

        }
        public double CustomerDebtSpecOrder(Customer customer)
        {
            double res = 0;
            //récupération de toutes les ventes du client qui n'ont pas encore été totalement réglées
            List<Sale> sales = this.CustomerAllUnPaidSalesSpecialOrder(customer);
            //Récupération de la dette du client
            res = sales.Sum(sale1 => sale1.SaleTotalPriceRemainder);

            return res;

        }
        public double CustomerDebtSpecOrder(Customer customer, DateTime BeginDate)
        {
            double res = 0;
            //récupération de toutes les ventes du client qui n'ont pas encore été totalement réglées
            List<Sale> sales = this.CustomerAllUnPaidSalesSpecialOrder(customer, BeginDate);
            //Récupération de la dette du client
            res = sales.Sum(sale1 => sale1.SaleTotalPriceRemainder);

            return res;

        }
        /// <summary>
        /// cette méthode retourne la dette du client.
        /// Elle permet beaucoup plus de récupérer la dette d'un géréral public particulier.
        /// Pour obtenir cette dette, on récupera toutes les ventes non payées dont le nom du représentant est celui en paramètre
        /// </summary>
        /// <param name="customer">Client dont on veut la dette </param>
        /// <param name="representant">General public particulier dont on veut la dette </param>
        /// <returns>Dette du client envers l'entreprise</returns>
        public double CustomerDebtStockLens(Customer customer, string representant)
        {
            double res = 0;
            //using (var context = new EFDbContext())
            //{
            //récupération de toutes les ventes du client qui n'ont pas encore été totalement réglées
            List<Sale> representantSales = new List<Sale>();
            List<Sale> sales = this.CustomerAllUnPaidSalesStockLens(customer);
            representant = representant.ToLower().TrimStart().TrimEnd();

            foreach (Sale s in sales)
            {
                string saleRepresentant = s.PoliceAssurance.ToLower().TrimStart().TrimEnd();

                if (representant.Equals(saleRepresentant))
                {
                    representantSales.Add(s);
                }
            }

            //Récupération de la dette du général public particulier
            res = representantSales.Sum(sale1 => sale1.SaleTotalPriceRemainder);
            //}

            return res;

        }

        public double CustomerDebtSpecOrder(Customer customer, string representant)
        {
            double res = 0;
            //using (var context = new EFDbContext())
            //{
            //récupération de toutes les ventes du client qui n'ont pas encore été totalement réglées
            List<Sale> representantSales = new List<Sale>();
            List<Sale> sales = this.CustomerAllUnPaidSalesSpecialOrder(customer);
            representant = representant.ToLower().TrimStart().TrimEnd();

            foreach (Sale s in sales)
            {
                string saleRepresentant = s.PoliceAssurance.ToLower().TrimStart().TrimEnd();

                if (representant.Equals(saleRepresentant))
                {
                    representantSales.Add(s);
                }
            }

            //Récupération de la dette du général public particulier
            res = representantSales.Sum(sale1 => sale1.SaleTotalPriceRemainder);
            //}

            return res;

        }
        /// <summary>
        /// permet de retrouver le montant restant d'une vente
        /// </summary>
        /// <param name="sale"></param>
        /// <returns></returns>
        public double SaleRemainder(Sale sale)
        {

            //Facture réélle du client sur la vente
            double res = SaleBill(sale);

            //somme des avances ayant eu lieu sur cette vente
            double advance = SaleTotalPriceAdvance(sale);

            //Reste à payer
            res = res - advance;

            res = (res > 0) ? res : 0;

            return res;

        }

        /// <summary>
        /// Quel est en argent la valeur des retours ayant eu lieu sur cette vente?
        /// </summary>
        /// <returns></returns>
        public double SaleReturnTotalAmount(Sale sale)
        {

            double res = 0;

            CustomerReturn custRet = context.CustomerReturns.SingleOrDefault(cr => cr.SaleID == sale.SaleID);

            //La vente a déjà subit au moins un retour
            if (custRet != null && custRet.CustomerReturnID > 0)
            {
                res = ApplyExtraPrice(custRet).TotalPriceReturn;
            }

            return res;

        }

        /// <summary>
        /// permet de retrouver le montant restant d'une vente
        /// </summary>
        /// <param name="sale"></param>
        /// <returns></returns>
        public double OtherSaleRemainder(Sale sale)
        {

            return SaleRemainder(sale);

        }

        /// <summary>
        /// 1-
        /// Liste des ventes non totalement réglées
        /// </summary>
        /// <returns></returns>
        public List<Sale> AllUnPaidSales()
        {
            List<Sale> res = new List<Sale>();
            //using (context = new EFDbContext())
            //    {
            context.Configuration.ProxyCreationEnabled = false;
            context.Configuration.LazyLoadingEnabled = false;

            //liste de toutes les ventes
            List<Sale> sales1 = context.Sales.ToList();
            //List<Sale> sales1 = from sa in context.Sales
            //                    select sa;
            List<Sale> sales = new List<Sale>();

            foreach (Sale sale in sales1)
            {
                //calcul du montant déjà perçu pour la vente
                sale.SaleTotalPriceAdvance = this.SaleTotalPriceAdvance(sale);

                sale.SaleLines = null;
                sale.CustomerSlices = null;

                sales.Add(this.ApplyExtraPrice(sale));

            }
            //récupération des ventes qui n'ont pas encore été totalement réglées
            res = sales.Where(sale => sale.SaleTotalPriceRemainder > 0).ToList();
            //}

            return res;
        }

        /// <summary>
        ///Liste des ventes non totalement réglées d'un client
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        public List<Sale> CustomerAllUnPaidSalesStockLens(Customer customer)
        {
            List<Sale> res = new List<Sale>();

            //liste de toutes les ventes
            List<Sale> sales1 = context.Sales.Where(s => s.CustomerID == customer.GlobalPersonID && !s.IsSpecialOrder).ToList();

            List<Sale> sales = new List<Sale>();

            foreach (Sale sale in sales1)
            {
                //calcul du montant déjà perçu pour la vente
                sale.SaleTotalPriceAdvance = this.SaleTotalPriceAdvance(sale);
                sale.SaleTotalPriceRemainder = this.SaleRemainder(sale);

                sale.SaleLines = null;
                sale.CustomerSlices = null;

                //sales.Add(this.ApplyExtraPrice(sale));

                if (sale.SaleTotalPriceRemainder > 0)
                {
                    //récupération des ventes qui n'ont pas encore été totalement réglées
                    Sale currentSale = this.ApplyExtraPrice(sale);
                    res.Add(currentSale);
                }

            }

            return res;
        }

        /// <summary>
        ///Liste des ventes non totalement réglées d'un client Avant une date
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        public List<Sale> CustomerAllUnPaidSalesStockLens(Customer customer, DateTime BeginDate)
        {
            List<Sale> res = new List<Sale>();

            //liste de toutes les ventes
            List<Sale> sales1 = context.Sales.Where(s => s.CustomerID == customer.GlobalPersonID && !s.IsSpecialOrder && s.SaleDate < BeginDate.Date).ToList();

            List<Sale> sales = new List<Sale>();

            foreach (Sale sale in sales1)
            {
                //calcul du montant déjà perçu pour la vente
                sale.SaleTotalPriceAdvance = this.SaleTotalPriceAdvance(sale);
                sale.SaleTotalPriceRemainder = this.SaleRemainder(sale);

                sale.SaleLines = null;
                sale.CustomerSlices = null;

                //sales.Add(this.ApplyExtraPrice(sale));

                if (sale.SaleTotalPriceRemainder > 0)
                {
                    //récupération des ventes qui n'ont pas encore été totalement réglées
                    Sale currentSale = this.ApplyExtraPrice(sale);
                    res.Add(currentSale);
                }

            }

            return res;
        }
        public List<Sale> CustomerAllUnPaidSalesSpecialOrder(Customer customer)
        {
            List<Sale> res = new List<Sale>();

            //liste de toutes les ventes
            List<Sale> sales1 = context.Sales.Where(s => s.CustomerID == customer.GlobalPersonID && s.IsSpecialOrder).ToList();

            List<Sale> sales = new List<Sale>();

            foreach (Sale sale in sales1)
            {
                //calcul du montant déjà perçu pour la vente
                sale.SaleTotalPriceAdvance = this.SaleTotalPriceAdvance(sale);
                sale.SaleTotalPriceRemainder = this.SaleRemainder(sale);

                sale.SaleLines = null;
                sale.CustomerSlices = null;

                //sales.Add(this.ApplyExtraPrice(sale));

                if (sale.SaleTotalPriceRemainder > 0)
                { //récupération des ventes qui n'ont pas encore été totalement réglées
                    Sale currentSale = this.ApplyExtraPrice(sale);
                    res.Add(currentSale);
                }

            }

            return res;
        }
        public List<Sale> CustomerAllUnPaidSalesSpecialOrder(Customer customer, DateTime BeginDate)
        {
            List<Sale> res = new List<Sale>();

            //liste de toutes les ventes
            List<Sale> sales1 = context.Sales.Where(s => s.CustomerID == customer.GlobalPersonID && s.IsSpecialOrder && s.SaleDate < BeginDate.Date).ToList();

            List<Sale> sales = new List<Sale>();

            foreach (Sale sale in sales1)
            {
                //calcul du montant déjà perçu pour la vente
                sale.SaleTotalPriceAdvance = this.SaleTotalPriceAdvance(sale);
                sale.SaleTotalPriceRemainder = this.SaleRemainder(sale);

                sale.SaleLines = null;
                sale.CustomerSlices = null;

                //sales.Add(this.ApplyExtraPrice(sale));

                if (sale.SaleTotalPriceRemainder > 0)
                { //récupération des ventes qui n'ont pas encore été totalement réglées
                    Sale currentSale = this.ApplyExtraPrice(sale);
                    res.Add(currentSale);
                }

            }

            return res;
        }
        /// <summary>
        ///Liste des ventes non totalement réglées d'un général public particulier
        /// </summary>
        /// <param name="customer"></param>
        /// <param name="representant">général public particulier</param>
        /// <returns></returns>
        public List<Sale> CustomerAllUnPaidSalesStockLens(Customer customer, string representant)
        {
            List<Sale> res = new List<Sale>();

            //liste de toutes les ventes
            List<Sale> sales1 = context.Sales.Where(s => s.CustomerID == customer.GlobalPersonID && !s.IsSpecialOrder).ToList();

            List<Sale> sales = new List<Sale>();

            representant = representant.ToLower().TrimStart().TrimEnd();

            foreach (Sale sale in sales1)
            {
                string saleRepresentant = sale.PoliceAssurance.ToLower().TrimStart().TrimEnd();

                //calcul du montant déjà perçu pour la vente
                sale.SaleTotalPriceAdvance = this.SaleTotalPriceAdvance(sale);
                sale.SaleTotalPriceRemainder = this.SaleRemainder(sale);

                sale.SaleLines = null;
                sale.CustomerSlices = null;

                //sales.Add(this.ApplyExtraPrice(sale));

                if (sale.SaleTotalPriceRemainder > 0 && (saleRepresentant.Equals(representant)))
                {
                    //récupération des ventes qui n'ont pas encore été totalement réglées et appartenant au général public particulier
                    Sale currentSale = this.ApplyExtraPrice(sale);
                    res.Add(currentSale);
                }

            }

            return res;
        }

        public List<Sale> CustomerAllUnPaidSalesSpecialOrder(Customer customer, string representant)
        {
            List<Sale> res = new List<Sale>();

            //liste de toutes les ventes
            List<Sale> sales1 = context.Sales.Where(s => s.CustomerID == customer.GlobalPersonID && s.IsSpecialOrder).ToList();

            List<Sale> sales = new List<Sale>();

            representant = representant.ToLower().TrimStart().TrimEnd();

            foreach (Sale sale in sales1)
            {
                string saleRepresentant = sale.PoliceAssurance.ToLower().TrimStart().TrimEnd();

                //calcul du montant déjà perçu pour la vente
                sale.SaleTotalPriceAdvance = this.SaleTotalPriceAdvance(sale);
                sale.SaleTotalPriceRemainder = this.SaleRemainder(sale);

                sale.SaleLines = null;
                sale.CustomerSlices = null;

                //sales.Add(this.ApplyExtraPrice(sale));

                if (sale.SaleTotalPriceRemainder > 0 && (saleRepresentant.Equals(representant)))
                {
                    //récupération des ventes qui n'ont pas encore été totalement réglées et appartenant au général public particulier
                    Sale currentSale = this.ApplyExtraPrice(sale);
                    res.Add(currentSale);
                }

            }

            return res;
        }
        public List<Sale> CustomerAllPeriodSales(Customer customer, DateTime beginDate, DateTime endDate)
        {
            return context.Sales.Where(s => s.CustomerID == customer.GlobalPersonID && s.SaleDate >= beginDate && s.SaleDate <= endDate).ToList(); ;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="SaleID"></param>
        /// <returns></returns>
        public List<Sale> OtherCustomerAllUnPaidSalesForNonAssure(int SaleID)
        {
            List<Sale> res = new List<Sale>();

            //liste de toutes les ventes
            List<Sale> sales1 = context.Sales.Where(s => s.SaleID == SaleID && !s.IsSpecialOrder /* && !s.IsPaid*/).ToList();

            List<Sale> sales = new List<Sale>();

            foreach (Sale sale in sales1)
            {
                //calcul du montant déjà perçu pour la vente
                sale.SaleTotalPriceAdvance = this.SaleTotalPriceAdvance(sale);
                sale.SaleTotalPriceRemainder = this.SaleRemainder(sale);


                if (sale.SaleTotalPriceRemainder > 0)
                {//récupération des ventes qui n'ont pas encore été totalement réglées
                    Sale currentSale = this.ApplyExtraPrice(sale);
                    res.Add(currentSale);
                }

            }

            return res;
        }
        public List<Sale> OtherCustomerAllUnPaidSalesSpecialOrder(Customer customer)
        {
            List<Sale> res = new List<Sale>();

            //liste de toutes les ventes
            List<Sale> sales1 = context.Sales.Where(s => s.CustomerID == customer.GlobalPersonID && s.IsSpecialOrder).ToList();

            List<Sale> sales = new List<Sale>();

            foreach (Sale sale in sales1)
            {
                //calcul du montant déjà perçu pour la vente
                sale.SaleTotalPriceAdvance = this.SaleTotalPriceAdvance(sale);
                sale.SaleTotalPriceRemainder = this.SaleRemainder(sale);


                if (sale.SaleTotalPriceRemainder > 0)
                { //récupération des ventes qui n'ont pas encore été totalement réglées
                    Sale currentSale = this.ApplyExtraPrice(sale);
                    res.Add(currentSale);
                }

            }

            return res;
        }
        /// <summary>
        /// Permet essentiellement de récupérer le montant TTC d'une vente
        /// </summary>
        /// <param name="sale"></param>
        /// <returns></returns>
        public Sale ApplyExtraPrice(Sale sale)
        {
            ExtraPrice ep = new ExtraPrice();
            double grossAmount = context.SaleLines.Where(sl => sl.SaleID == sale.SaleID).ToList().Select(sl2 => (sl2.LineQuantity * sl2.LineUnitPrice)).Sum();

            ep = Util.ExtraPrices(grossAmount, sale.RateReduction, sale.RateDiscount, sale.Transport, sale.VatRate);

            sale.ReductionAmount = ep.ReductionAmount;
            sale.TotalPriceHT = ep.TotalHT;
            sale.TotalPriceTTC = ep.TotalTTC;
            sale.TVAAmount = ep.TVAAmount;

            return sale;
        }

        /// <summary>
        /// Permet essentiellement de récupérer le montant TTC d'un achat
        /// </summary>
        /// <param name="sale"></param>
        /// <returns></returns>
        Purchase ApplyExtraPrice(Purchase purchase)
        {
            ExtraPrice ep = new ExtraPrice();
            double grossAmount = context.PurchaseLines.Where(sl => sl.PurchaseID == purchase.PurchaseID).Select(sl2 => (sl2.LineQuantity * sl2.LineUnitPrice)).Sum();

            ep = Util.ExtraPrices(grossAmount, purchase.RateReduction, purchase.RateDiscount, purchase.Transport, purchase.VatRate);

            purchase.ReductionAmount = ep.ReductionAmount;
            purchase.TotalPriceHT = ep.TotalHT;
            purchase.TotalPriceTTC = ep.TotalTTC;
            purchase.TVAAmount = ep.TVAAmount;

            return purchase;
        }

        /// <summary>
        /// Permet essentiellement de récupérer le montant TTC d'un retour
        /// </summary>
        /// <param name="cr"></param>
        /// <returns></returns>
        public CustomerReturn ApplyExtraPrice(CustomerReturn cr)
        {
            ExtraPrice ep = new ExtraPrice();
            double grossAmount = context.CustomerReturnLines.Where(sl => sl.CustomerReturnID == cr.CustomerReturnID).ToList().Select(sl2 => (sl2.LineQuantity * sl2.SaleLine.LineUnitPrice)).Sum();

            ep = Util.ExtraPrices(grossAmount, cr.Sale.RateReduction, cr.Sale.RateDiscount, cr.Sale.Transport, cr.Sale.VatRate);

            CustomerReturn acountableCustomerReturn = new CustomerReturn()
            {
                CustomerReturnDate = cr.CustomerReturnDate,
                TotalPriceMarchandise = grossAmount,
                TotalPriceReturn = ep.TotalTTC,
                DiscountAmount = ep.DiscountAmount,
                Transport = cr.Transport,
                TVAAmount = ep.TVAAmount,
                CustomerReturnID = cr.CustomerReturnID,
                Sale = cr.Sale,
                SaleID = cr.Sale.SaleID
            };

            return acountableCustomerReturn;
        }

        /// <summary>
        /// 2-
        /// Liste des ventes non totalement réglées d'un client
        /// </summary>
        /// <param name="customer">Client dont on veut ses ventes non réglées</param>
        /// <returns></returns>
        public List<Sale> UnPaidSales(Customer customer)
        {
            List<Sale> res = new List<Sale>();

            //récupération des ventes non totalement réglées du client
            //res = this.AllUnPaidSales().Where(sale => sale.CustomerID == customer.GlobalPersonID).ToList();
            res = this.CustomerAllUnPaidSalesStockLens(customer);
            return res;

        }

        /// <summary>
        /// cette méthode retourne le montant total d'une vente
        /// </summary>
        /// <param name="sale"></param>
        /// <returns></returns>
        public double BillAmount(Sale sale)
        {
            return this.ApplyExtraPrice(sale).TotalPriceTTC;
        }

        /// <summary>
        /// cette méthode retourne le montant total d'un achat
        /// </summary>
        /// <param name="purchase"></param>
        /// <returns></returns>
        public double BillAmount(Purchase purchase)
        {
            return this.ApplyExtraPrice(purchase).TotalPriceTTC;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="deposit"></param>
        /// <param name="SaleID"></param>
        /// <param name="UserConect"></param>
        /// <param name="SaleDeliver"></param>
        /// <returns></returns>
        public Boolean SaleDepositForNonAssure(Deposit deposit, int SaleID, int UserConect, int SaleDeliver)
        {
            Boolean res = false;
            double resteMntVente = 0d;
            double totalResteVente = 0d;
            //double totalAvance = 0d;
            double depositAmount = 0d;
            double MontantDejaAdvance = 0d;
            context = new EFDbContext();
            //Begin of transaction

            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    //Montant déposé par le client
                    totalResteVente = deposit.Amount;
                    double TotAmount = deposit.Amount;

                    Customer customerEntity = (deposit.CustomerID == null || deposit.CustomerID == 0) ? context.Customers.Where(c => c.Name.ToLower() == "default").FirstOrDefault() : context.Customers.Find(deposit.CustomerID);
                    PaymentMethod paymentMethod = context.PaymentMethods.Find(deposit.PaymentMethodID);
                    if (paymentMethod is DigitalPaymentMethod)
                    {
                        deposit.DigitalAccountManagerId = ((DigitalPaymentMethod)paymentMethod).AccountManagerId;
                    }

                    //recuperation des ventes correspondant au montant saisi par le user
                    List<Sale> sales = this.OtherCustomerAllUnPaidSalesForNonAssure(SaleID).ToList();

                    foreach (Sale depSale in sales)
                    {

                        MontantDejaAdvance = depSale.SaleTotalPriceAdvance;

                        //recuperation du montant restant de la vente
                        resteMntVente = depSale.SaleTotalPriceRemainder;// this.OtherSaleRemainder(depSale);

                        Sale currentSale = context.Sales.Find(depSale.SaleID);

                        if (totalResteVente < resteMntVente)
                        {
                            //Ici, On fait une avance pour la vente
                            depositAmount = totalResteVente;

                            //Mise à jour de la vente pour indiquer que la vente a été avancée
                            currentSale.OldStatutSale = SalePurchaseStatut.Invoiced;// currentSale.StatutSale;
                            currentSale.StatutSale = SalePurchaseStatut.Advanced;
                            context.SaveChanges();

                        }
                        else // il s'agit du cas totalResteVente >= resteMntVente  
                        {
                            //Ici, on règle totalement la vente ou le montant restant de la vente
                            depositAmount = resteMntVente;
                            //Mise à jour de la vente pour indiquer que la vente a été totalement payée 
                            currentSale.OldStatutSale = (MontantDejaAdvance == 0) ? SalePurchaseStatut.Paid : SalePurchaseStatut.Advanced;// currentSale.StatutSale;
                            currentSale.StatutSale = SalePurchaseStatut.Paid;
                            currentSale.IsPaid = true;
                            context.SaveChanges();
                        }

                        //construction du customer slice
                        CustomerSlice customerSlice = new CustomerSlice();
                        customerSlice.DeviseID = deposit.DeviseID;
                        customerSlice.PaymentMethodID = deposit.PaymentMethodID;
                        customerSlice.SaleID = depSale.SaleID;
                        customerSlice.SliceAmount = depositAmount;// deposit.Amount,
                        customerSlice.SliceDate = deposit.DepositDate;
                        customerSlice.Representant = deposit.Representant;
                        customerSlice.isDeposit = true;
                        customerSlice.Reference = deposit.DepositReference;
                        customerSlice.OperatorID = UserConect;
                        customerSlice.DigitalAccountManagerId = deposit.DigitalAccountManagerId;
                        customerSlice.TransactionIdentifier = deposit.TransactionIdentifier;
                        
                        //persistence de la tranche pour le règlement de la totalité ou partie de la vente.
                        context.CustomerSlices.Add(customerSlice);
                        context.SaveChanges();


                        double TotalPriceTTC = this.SaleBill(depSale);

                        //a revoir apres les tests

                        //comptabilisation du paiement de la tranche par le client
                        Sale acountableSale = new Sale()
                        {
                            BranchID = deposit.BranchID,
                            CustomerID = customerEntity.GlobalPersonID,
                            PaymentMethodID = deposit.PaymentMethodID,
                            SaleDate = deposit.DepositDate,
                            SaleID = depSale.SaleID,
                            TotalPriceTTC = TotalPriceTTC,// currentSale.TotalPriceTTC,
                            SaleReceiptNumber = depSale.SaleReceiptNumber,
                            SaleTotalPriceAdvance = depositAmount,
                            DeviseID = deposit.DeviseID,
                            StatutSale = currentSale.StatutSale,
                            OldStatutSale = currentSale.OldStatutSale,
                            MontantClientDeposit = depositAmount,
                            MontantTotalClientAdvance = MontantDejaAdvance

                        };
                        ////IAccountOperation opaccount = new AccountOperationRepository(context);
                        ////res = opaccount.ecritureComptableFinal(acountableSale);
                        ////if (!res)
                        ////{
                        ////    //transaction.Rollback();
                        ////    throw new Exception("Une erreur s'est produite lors de comptabilisation du dépôt pour le règlement de la vente ");
                        ////}

                        totalResteVente = totalResteVente - depositAmount;

                        //si le montant verse par le client est totalement use alors break
                        if (totalResteVente <= 0) break;
                    }

                    //On fait un dépôt d'épargne sur le compte du client
                    if (totalResteVente > 0)
                    {
                        ISavingAccount savingAccountRepository = new SavingAccountRepository(context);
                        deposit.Amount = totalResteVente;



                        savingAccountRepository.DoADeposit(deposit, true, UserConect);
                    }
                    //}


                    if (TotAmount > 0)
                    {
                        //ecriture ds la table generale des depots
                        AllDeposit alldeposits = new AllDeposit()
                        {
                            Amount = TotAmount,
                            AllDepositDate = deposit.DepositDate,
                            PaymentMethodID = deposit.PaymentMethodID,
                            DeviseID = deposit.DeviseID,
                            CustomerID = customerEntity.GlobalPersonID,
                            Representant = deposit.Representant,
                            AllDepositReference = deposit.DepositReference,
                            BranchID = deposit.BranchID,
                            AllDepositReason = "DEPOSIT",
                            IsSpecialOrder = false,
                            OperatorID = UserConect
                        };

                        if (paymentMethod is DigitalPaymentMethod)
                        {
                            alldeposits.DigitalAccountManagerId = ((DigitalPaymentMethod)paymentMethod).AccountManagerId;
                            alldeposits.TransactionIdentifier = deposit.TransactionIdentifier;
                        }

                        context.AllDeposits.Add(alldeposits);
                        context.SaveChanges();

                        //mise a jour du cpteur du transact number
                        TransactNumber trn = context.TransactNumbers.SingleOrDefault(t => t.TransactNumberCode == "REDP");
                        if (trn != null)
                        {
                            //persistance du compteur de l'objet TransactNumber
                            trn.Counter = trn.Counter + 1;
                        }
                        context.SaveChanges();
                    }


                    //EcritureSneack
                    IMouchar opSneak = new MoucharRepository(context);
                    res = opSneak.InsertOperation(UserConect, "SUCCESS", " CUSTOMER " + deposit.Representant + " DEPOSIT REF " + deposit.DepositReference + " -AMOUNT- " + TotAmount, "SaleDeposit", deposit.DepositDate, deposit.BranchID);
                    if (!res)
                    {
                        throw new Exception("Une erreur s'est produite lors de la journalisation ");
                    }
                    res = true;
                    //transaction.Commit();
                    ts.Complete();
                }
            }
            catch (Exception e)
            {
                //If an errors occurs, we cancel all changes in database
                res = false;
                //transaction.Rollback();
                throw new Exception("Error : " + e.Message);
            }

            return res;

        }
        public Boolean SaleDepositSpecialOrder(Deposit deposit, int UserConect)
        {
            Boolean res = false;
            double resteMntVente = 0d;
            double totalResteVente = 0d;
            //double totalAvance = 0d;
            double depositAmount = 0d;
            context = new EFDbContext();
            //Begin of transaction

            try
            {
                using (TransactionScope ts = new TransactionScope())
                {

                    //Montant déposé par le client
                    totalResteVente = deposit.Amount;
                    double TotAmount = deposit.Amount;

                    //recuperation des ventes correspondant au montant saisi par le user
                    Customer cust = context.Customers.Find(deposit.CustomerID);
                    List<Sale> sales = this.OtherCustomerAllUnPaidSalesSpecialOrder(cust).ToList();

                    if (IsGeneralPublic(cust))
                    {
                        string representant = deposit.Representant.TrimEnd().TrimStart().ToLower();
                        List<Sale> sales2 = sales.Where(s => s.PoliceAssurance.TrimEnd().TrimStart().ToLower() == representant).ToList();
                        sales = sales2;
                    }
                    foreach (Sale depSale in sales)
                    {

                        //recuperation du montant restant de la vente
                        resteMntVente = this.OtherSaleRemainder(depSale);

                        Sale currentSale = context.Sales.Find(depSale.SaleID);

                        if (totalResteVente < resteMntVente)
                        {
                            //Ici, On fait une avance pour la vente
                            depositAmount = totalResteVente;

                            //Mise à jour de la vente pour indiquer que la vente a été avancée
                            currentSale.OldStatutSale = SalePurchaseStatut.Invoiced;// currentSale.StatutSale;
                            currentSale.StatutSale = SalePurchaseStatut.Advanced;
                            context.SaveChanges();

                        }
                        else // il s'agit du cas totalResteVente >= resteMntVente  
                        {
                            //Ici, on règle totalement la vente ou le montant restant de la vente
                            depositAmount = resteMntVente;

                            //Mise à jour de la vente pour indiquer que la vente a été totalement payée 

                            currentSale.OldStatutSale = SalePurchaseStatut.Invoiced;// currentSale.StatutSale;
                            currentSale.StatutSale = SalePurchaseStatut.Paid;
                            context.SaveChanges();
                        }

                        //construction du customer slice
                        CustomerSlice customerSlice = new CustomerSlice();
                        customerSlice.DeviseID = deposit.DeviseID;
                        customerSlice.PaymentMethodID = deposit.PaymentMethodID;
                        customerSlice.SaleID = depSale.SaleID;
                        customerSlice.SliceAmount = depositAmount;// deposit.Amount,
                        customerSlice.SliceDate = deposit.DepositDate;
                        customerSlice.Representant = deposit.Representant;
                        customerSlice.isDeposit = true;
                        customerSlice.Reference = deposit.DepositReference;
                        customerSlice.OperatorID = UserConect;
                        //persistence de la tranche pour le règlement de la totalité ou partie de la vente.
                        context.CustomerSlices.Add(customerSlice);
                        context.SaveChanges();

                        double TotalPriceTTC = this.SaleBill(depSale);
                        //comptabilisation du paiement de la tranche par le client
                        Sale acountableSale = new Sale()
                        {
                            BranchID = deposit.BranchID,
                            CustomerID = deposit.CustomerID,
                            PaymentMethodID = deposit.PaymentMethodID,
                            SaleDate = deposit.DepositDate,
                            SaleID = depSale.SaleID,
                            TotalPriceTTC = TotalPriceTTC,// currentSale.TotalPriceTTC,
                            SaleReceiptNumber = depSale.SaleReceiptNumber,
                            SaleTotalPriceAdvance = depositAmount,
                            DeviseID = deposit.DeviseID,
                            StatutSale = currentSale.StatutSale,
                            OldStatutSale = currentSale.OldStatutSale
                        };
                        ////IAccountOperation opaccount = new AccountOperationRepository(context);
                        ////res = opaccount.ecritureComptableFinal(acountableSale);
                        ////if (!res)
                        ////{
                        ////    //transaction.Rollback();
                        ////    throw new Exception("Une erreur s'est produite lors de comptabilisation du dépôt pour le règlement de la vente ");
                        ////}

                        totalResteVente = totalResteVente - depositAmount;

                        //si le montant verse par le client est totalement use alors break
                        if (totalResteVente <= 0) break;
                    }


                    //On fait un dépôt d'épargne sur le compte du client
                    if (totalResteVente > 0)
                    {
                        ISavingAccount savingAccountRepository = new SavingAccountRepository(context);
                        deposit.Amount = totalResteVente;
                        savingAccountRepository.DoADeposit(deposit, true, UserConect);
                    }

                    //ecriture ds la table generale des depots
                    AllDeposit alldeposits = new AllDeposit()
                    {
                        Amount = TotAmount,
                        AllDepositDate = deposit.DepositDate,
                        PaymentMethodID = deposit.PaymentMethodID,
                        DeviseID = deposit.DeviseID,
                        CustomerID = deposit.CustomerID,
                        Representant = deposit.Representant,
                        AllDepositReference = deposit.DepositReference,
                        BranchID = deposit.BranchID,
                        OperatorID = UserConect,
                        AllDepositReason = deposit.DepositReason,
                        IsSpecialOrder = true,
                    };
                    context.AllDeposits.Add(alldeposits);
                    context.SaveChanges();

                    //mise a jour du cpteur du transact number
                    TransactNumber trn = context.TransactNumbers.SingleOrDefault(t => t.TransactNumberCode == "REDP");
                    if (trn != null)
                    {
                        //persistance du compteur de l'objet TransactNumber
                        trn.Counter = trn.Counter + 1;
                    }
                    context.SaveChanges();

                    //EcritureSneack
                    IMouchar opSneak = new MoucharRepository(context);
                    res = opSneak.InsertOperation(UserConect, "SUCCESS", " CUSTOMER " + deposit.Representant + " DEPOSIT REF " + deposit.DepositReference + " -AMOUNT- " + TotAmount, "SaleDeposit", deposit.DepositDate, deposit.BranchID);
                    if (!res)
                    {
                        throw new Exception("Une erreur s'est produite lors de la journalisation ");
                    }
                    res = true;
                    //transaction.Commit();
                    ts.Complete();
                }
            }
            catch (Exception e)
            {
                //If an errors occurs, we cancel all changes in database
                res = false;
                //transaction.Rollback();
                throw new Exception("Error : " + e.Message + " - " + e.StackTrace);
            }

            return res;

        }

        /// <summary>
        /// Cette méthode renvoie la facture à payée pour une vente.
        /// C'est égale à la facture de la vente moins les retours
        /// </summary>
        /// <param name="sale"></param>
        /// <returns></returns>
        public double SaleBill(Sale sale)
        {
            //Facture de base de la vente
            double res = ApplyExtraPrice(sale).TotalPriceTTC;

            //Facture réelle de la vente
            res = res - SaleReturnTotalAmount(sale);

            return res;

        }

        public static bool IsGeneralPublic(Customer cust)
        {
            //Modification du representant pour adérer à la division des General Public
            string customerNumber = cust.CNI.ToLower().TrimEnd().TrimStart();
            string generalPublicNumber = CodeValue.Sale.Customer.GENERALPUBLICNUMBER.ToLower().TrimEnd().TrimStart();

            return customerNumber.Equals(generalPublicNumber);
        }
        /// <summary>
        /// cette methode permet de suprimer un depot qui a ete valider
        /// Supression successive ds les tables
        /// -Deposit
        /// -AllDeposit
        /// -CustomerSlice
        /// -DepositAccountOperations
        /// -AccountOperation
        /// </summary>
        /// <param name="AllDepositID"></param>
        /// <param name="UserID"></param>
        /// <returns></returns>
        public bool DeleteDepositEntry(int AllDepositID, int UserID, DateTime ServerDate)
        {
            bool res = false;
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    //RECUPERATION DE LA REFERENCE DEPOT D'EPARGNE
                    AllDeposit Alldepot = context.AllDeposits.Find(AllDepositID);
                    //if (ServerDate!=Alldepot.AllDepositDate)
                    //{
                    //    res = false;
                    //    throw new Exception("Error: You can only delete Deposit for "+new DateTime(ServerDate.Year,ServerDate.Month,ServerDate.Day));
                    //}
                    //DepositAccountOperations PR LES DEPOTS D'EPARGNE
                    context.AccountOperations.OfType<DepositAccountOperation>().Where(a => a.Reference == Alldepot.AllDepositReference).ToList().ForEach(ol =>
                    {
                        context.AccountOperations.Remove(ol);
                        context.SaveChanges();
                    });

                    //recuperation des ventes ki ont  ete regle par ce depot
                    List<CustomerSlice> lstPaidSale = context.CustomerSlices.Where(ls => ls.Reference == Alldepot.AllDepositReference).ToList();
                    lstPaidSale.ForEach(csl =>
                    {
                        //SUPRESSION DES ECRITURE COMPTABLE LIE A CES ECRITURE
                        context.AccountOperations.OfType<SaleAccountOperation>().Where(a => a.SaleID == csl.SaleID).ToList().ForEach(accOp =>
                        {
                            context.AccountOperations.Remove(accOp);
                            context.SaveChanges();
                        });
                        //supression des customer slice
                        context.CustomerSlices.Remove(csl);
                        context.SaveChanges();
                    });
                    //
                    //Depot d'epargne
                    List<Deposit> lstSavingDep = context.Deposits.Where(ld => ld.DepositReference == Alldepot.AllDepositReference).ToList();
                    lstSavingDep.ForEach(ldep =>
                    {
                        context.Deposits.Remove(ldep);
                        //context.SaveChanges();
                    });

                    //supression du depot complet
                    context.AllDeposits.Remove(Alldepot);
                    context.SaveChanges();

                    //recuperation de la caisse ki a efectuer la transaction
                    Till till = context.Tills.Find(Alldepot.PaymentMethodID);

                    //mise a jour des solde de la caisse
                    DateTime DateDernierOp = new DateTime(1900, 01, 01);
                    DateTime DateFuturOp = new DateTime(1900, 01, 01);
                    ITillDay _tillDayRepository = new TillDayRepository();

                    DateTime DateVeilleOp = Alldepot.AllDepositDate.Date.AddDays(-1);
                    var tillDayDernierOp = context.TillDays.Where(tdd => tdd.TillID == till.ID && tdd.TillDayDate <= DateVeilleOp.Date).OrderByDescending(s => s.TillDayDate).Take(1);
                    foreach (var getTillDay in tillDayDernierOp)
                    {
                        DateVeilleOp = getTillDay.TillDayDate;
                    }
                    TillSatut tillstatusVeilleOp = _tillDayRepository.TillStatus(till, DateVeilleOp);
                    double VeilleOpAmt = 0;// tillstatusVeilleOp.OpenningPrice;

                    DateTime dateop = Alldepot.AllDepositDate.Date;
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

                    context.SaveChanges();
                    IMouchar opSneak = new MoucharRepository(context);
                    //si pas de depot d'epargne ni de payement avec ce depot alors supression impossible
                    if (lstPaidSale.Count == 0 && lstSavingDep.Count == 0)
                    {
                        res = opSneak.InsertOperation(UserID, "ERROR", "ERROR WHILE DELETE DEPOSIT ENTRY FOR REFERENCE " + Alldepot.AllDepositReference + " FOR CUSTOMER " + Alldepot.Representant + " BECAUSE NO SAVING DEPOSIT AND NO SALE PAYMENT", "DeleteDepositEntry", Alldepot.AllDepositDate, Alldepot.BranchID);
                        if (!res)
                        {
                            res = false;
                            throw new Exception("Une erreur s'est produite lors de la journalisation ");
                        }
                    }


                    //EcritureSneack
                    res = opSneak.InsertOperation(UserID, "SUCCESS", "DELETE DEPOSIT ENTRY FOR REFERENCE " + Alldepot.AllDepositReference + " FOR CUSTOMER " + Alldepot.Representant, "DeleteDepositEntry", Alldepot.AllDepositDate, Alldepot.BranchID);
                    if (!res)
                    {
                        res = false;
                        throw new Exception("Une erreur s'est produite lors de la journalisation ");
                    }
                    res = true;
                    ts.Complete();
                }
            }
            catch (Exception e)
            {
                res = false;
                throw new Exception("Error while delete Deposit : " + "e.Message = " + e.Message + "e.InnerException = " + e.InnerException + "e.StackTrace = " + e.StackTrace);
            }
            return res;
        }

        public double TotalDepotSliceBefore(Customer customer, DateTime BeginDate)
        {
            double res = 0;

            //Somme des dépôts direct sur achat
            List<CustomerSlice> lstCustSliceBefore = context.CustomerSlices.Where(cs => cs.Sale.CustomerID == customer.GlobalPersonID &&
                                                                                         cs.SliceDate < BeginDate.Date && !cs.isDeposit).ToList();

            double depotCustomerSliceBefore = lstCustSliceBefore != null ? lstCustSliceBefore.Select(cs1 => cs1.SliceAmount).Sum() : 0;

            //recupartion des depot apres achat
            List<AllDeposit> lstAllDeposit = context.AllDeposits.Where(ad => ad.CustomerID == customer.GlobalPersonID && ad.AllDepositDate < BeginDate.Date && !ad.AllDepositReference.Contains("REMOVENULL")).ToList();
            double depotAllDepBefore = lstAllDeposit != null ? lstAllDeposit.Select(dep => dep.Amount).Sum() : 0;

            double depotSliceBefore = depotCustomerSliceBefore + depotAllDepBefore;
            res = depotSliceBefore;

            return res;
        }

        public double TotalDepotSlicePeriode(Customer customer, DateTime BeginDate, DateTime EndDate)
        {
            double res = 0;

            //Somme des dépôts direct sur achat
            List<CustomerSlice> lstCustSlicePeriode = context.CustomerSlices.Where(cs => cs.Sale.CustomerID == customer.GlobalPersonID &&
                                                                                         (cs.SliceDate >= BeginDate.Date && cs.SliceDate <= EndDate.Date) && !cs.isDeposit).ToList();

            double depotCustomerSlicePeriode = lstCustSlicePeriode != null ? lstCustSlicePeriode.Select(cs1 => cs1.SliceAmount).Sum() : 0;

            //recupartion des depot apres achat
            List<AllDeposit> lstAllDeposit = context.AllDeposits.Where(ad => ad.CustomerID == customer.GlobalPersonID && (ad.AllDepositDate >= BeginDate.Date && ad.AllDepositDate <= EndDate.Date) && !ad.AllDepositReference.Contains("REMOVENULL")).ToList();
            double depotAllDepPeriode = lstAllDeposit != null ? lstAllDeposit.Select(dep => dep.Amount).Sum() : 0;

            double depotSlicePeriode = depotCustomerSlicePeriode + depotAllDepPeriode;
            res = depotSlicePeriode;

            return res;
        }

        public double TotalDepotSlice(Customer customer)
        {
            double res = 0;

            //Somme des dépôts direct sur achat
            List<CustomerSlice> lstCustSlice = context.CustomerSlices.Where(cs => cs.Sale.CustomerID == customer.GlobalPersonID &&
                                                                                         !cs.isDeposit).ToList();

            double depotCustomerSlice = lstCustSlice != null ? lstCustSlice.Select(cs1 => cs1.SliceAmount).Sum() : 0;

            //recupartion des depot apres achat
            List<AllDeposit> lstAllDeposit = context.AllDeposits.Where(ad => ad.CustomerID == customer.GlobalPersonID && !ad.AllDepositReference.Contains("REMOVENULL")).ToList();
            double depotAllDep = lstAllDeposit != null ? lstAllDeposit.Select(dep => dep.Amount).Sum() : 0;

            double depotSlice = depotCustomerSlice + depotAllDep;
            res = depotSlice;

            return res;
        }


        //modif avec le changement des formulaires vers jquery

        /// <summary>
        /// new total sale before
        /// </summary>
        /// <param name="BranchID"></param>
        /// <param name="CustomerID"></param>
        /// <param name="Begindate"></param>
        /// <returns></returns>

        public double NewTotalAchat(int? customerID)
        {

            double MtDebit = 0;
            try
            {
                //recuperation des ttes les ventes de la periode
                if (customerID.Value > 0)
                {
                    List<Sale> salelist = context.Sales.Where(c => c.CustomerID == customerID).ToList();
                    foreach (Sale allSales in salelist)
                    {
                        double saleAmnt = (allSales.SaleLines == null) ? 0 : allSales.SaleLines.Select(l => l.LineAmount).Sum();
                        if (saleAmnt == null) saleAmnt = 0;
                        ExtraPrice extra = Util.ExtraPrices(saleAmnt, (allSales.RateReduction == null) ? 0 : allSales.RateReduction, (allSales.RateDiscount == null) ? 0 : allSales.RateDiscount, (allSales.Transport == null) ? 0 : allSales.Transport, (allSales.VatRate == null) ? 0 : allSales.VatRate);
                        MtDebit += (extra.TotalTTC == null) ? 0 : extra.TotalTTC;
                    }
                    //    salelist.ForEach(allSales =>
                    //{
                    //    double saleAmnt = allSales.SaleLines.Select(l => l.LineAmount).Sum();
                    //    if (saleAmnt == null) saleAmnt = 0;
                    //    ExtraPrice extra = Util.ExtraPrices(saleAmnt, (allSales.RateReduction == null) ? 0 : allSales.RateReduction, (allSales.RateDiscount == null) ? 0 : allSales.RateDiscount, (allSales.Transport == null) ? 0 : allSales.Transport, (allSales.VatRate == null) ? 0 : allSales.VatRate);
                    //    MtDebit += (extra.TotalTTC == null) ? 0 : extra.TotalTTC;

                    //});

                    /****** traitement des depot d'epargne provenant des retours   * *****/
                    //recuperation des ttes les ventes de la periode
                    //recuperation du saving acct du customer
                    SavingAccount savAcct = context.SavingAccounts.Where(s => s.CustomerID == customerID).SingleOrDefault();
                    if (savAcct != null)
                    {
                        List<Deposit> Retdeposit = context.Deposits.Where(c => c.SavingAccountID == savAcct.ID && c.DepositReference.StartsWith("SADE")).ToList();
                        Retdeposit.ForEach(allRestDep =>
                        {
                            MtDebit += allRestDep.Amount;
                        });
                    }
                    else
                    {
                        List<AllDeposit> Retdeposit = context.AllDeposits.Where(c => c.CustomerID == customerID).ToList();
                        Retdeposit.ForEach(allRestDep =>
                        {
                            MtDebit += allRestDep.Amount;
                        });
                    }
                }

            }
            catch (Exception e)
            {
                throw new Exception("Error : " + e.Message + " - " + e.StackTrace);
            }
            return MtDebit;

        }
        /// <summary>
        /// new  total deposit before
        /// </summary>
        /// <param name="BranchID"></param>
        /// <param name="CustomerID"></param>
        /// <param name="Begindate"></param>
        /// <returns></returns>
        /// 
        public double NewTotalDepotSlice(int? customerID)
        {

            double MtCredit = 0;

            //recuperation des ttes les ventes de la periode
            if (customerID.Value > 0)
            {

                //traitement des reglements des ventes par avance sur la periode
                List<CustomerSlice> perCustomerSlice = context.CustomerSlices.Where(c => c.Sale.CustomerID == customerID && !c.isDeposit).ToList();
                perCustomerSlice.ForEach(allslices =>
                {
                    MtCredit += allslices.SliceAmount;
                });
                /****** traitement des depots   * *****/

                List<AllDeposit> depositlist = context.AllDeposits.Where(dep => dep.CustomerID == customerID && !dep.AllDepositReference.Contains("REMOVENULL")).ToList();
                depositlist.ForEach(alldeposits =>
                {
                    MtCredit += alldeposits.Amount;
                });

                /****** traitement des retours   * *****/
                //recuperation des ttes les ventes de la periode
                List<CustomerReturn> RetSale = context.CustomerReturns.Where(c => c.Sale.CustomerID == customerID).ToList();
                RetSale.ForEach(allRestSales =>
                {

                    double RetsaleAmnt = allRestSales.CustomerReturnLines.Select(l => l.SaleLine.LineAmount).Sum();
                    ExtraPrice extra = Util.ExtraPrices(RetsaleAmnt, allRestSales.RateReduction, allRestSales.RateDiscount, allRestSales.Transport, allRestSales.VatRate);
                    MtCredit += extra.TotalTTC;
                });


            }

            return MtCredit;

        }

        /// <summary>
        /// new total sale before
        /// </summary>
        /// <param name="BranchID"></param>
        /// <param name="CustomerID"></param>
        /// <param name="Begindate"></param>
        /// <returns></returns>

        public double NewTotalAchatBefore(int? customerID, DateTime BeginDate)
        {

            double MtDebit = 0;

            //recuperation des ttes les ventes de la periode
            if (customerID.Value > 0)
            {
                List<Sale> salelist = context.Sales.Where(c => c.CustomerID == customerID && (c.SaleDate < BeginDate.Date)).ToList();
                foreach (Sale allSales in salelist)
                {
                    double saleAmnt = (allSales.SaleLines == null) ? 0 : allSales.SaleLines.Select(l => l.LineAmount).Sum();
                    if (saleAmnt == null) saleAmnt = 0;
                    ExtraPrice extra = Util.ExtraPrices(saleAmnt, (allSales.RateReduction == null) ? 0 : allSales.RateReduction, (allSales.RateDiscount == null) ? 0 : allSales.RateDiscount, (allSales.Transport == null) ? 0 : allSales.Transport, (allSales.VatRate == null) ? 0 : allSales.VatRate);
                    MtDebit += (extra.TotalTTC == null) ? 0 : extra.TotalTTC;
                }

                //salelist.ForEach(allSales =>
                //{
                //    double saleAmnt = allSales.SaleLines.Select(l => l.LineAmount).Sum();
                //    ExtraPrice extra = Util.ExtraPrices(saleAmnt, allSales.RateReduction, allSales.RateDiscount, allSales.Transport, allSales.VatRate);
                //    MtDebit += extra.TotalTTC;

                //});

                /****** traitement des depot d'epargne provenant des retours   * *****/
                //recuperation des ttes les ventes de la periode
                //recuperation du saving acct du customer
                SavingAccount savAcct = context.SavingAccounts.Where(s => s.CustomerID == customerID).SingleOrDefault();
                if (savAcct != null)
                {
                    List<Deposit> Retdeposit = context.Deposits.Where(c => c.SavingAccountID == savAcct.ID && (c.DepositDate < BeginDate.Date) && c.DepositReference.StartsWith("SADE")).ToList();
                    Retdeposit.ForEach(allRestDep =>
                    {
                        MtDebit += allRestDep.Amount;
                    });
                }
                else
                {
                    List<AllDeposit> Retdeposit = context.AllDeposits.Where(c => c.CustomerID == customerID && (c.AllDepositDate < BeginDate.Date)).ToList();
                    Retdeposit.ForEach(allRestDep =>
                    {
                        MtDebit += allRestDep.Amount;
                    });
                }
            }
            return MtDebit;

        }

        /// <summary>
        /// new  total deposit before
        /// </summary>
        /// <param name="BranchID"></param>
        /// <param name="CustomerID"></param>
        /// <param name="Begindate"></param>
        /// <returns></returns>
        /// 
        public double NewTotalDepotSliceBefore(int? customerID, DateTime BeginDate)
        {

            double MtCredit = 0;

            //recuperation des ttes les ventes de la periode
            if (customerID.Value > 0)
            {

                //traitement des reglements des ventes par avance sur la periode
                List<CustomerSlice> perCustomerSlice = context.CustomerSlices.Where(c => c.Sale.CustomerID == customerID && (c.SliceDate < BeginDate.Date) && !c.isDeposit).ToList();
                perCustomerSlice.ForEach(allslices =>
                {
                    MtCredit += allslices.SliceAmount;
                });
                /****** traitement des depots   * *****/

                List<AllDeposit> depositlist = context.AllDeposits.Where(dep => (dep.AllDepositDate < BeginDate.Date) && dep.CustomerID == customerID && !dep.AllDepositReference.Contains("REMOVENULL")).ToList();
                depositlist.ForEach(alldeposits =>
                {
                    MtCredit += alldeposits.Amount;
                });

                /****** traitement des retours   * *****/
                //recuperation des ttes les ventes de la periode
                List<CustomerReturn> RetSale = context.CustomerReturns.Where(c => c.Sale.CustomerID == customerID && (c.Sale.SaleDate < BeginDate.Date)).ToList();
                RetSale.ForEach(allRestSales =>
                {

                    double RetsaleAmnt = allRestSales.CustomerReturnLines.Select(l => l.SaleLine.LineAmount).Sum();
                    ExtraPrice extra = Util.ExtraPrices(RetsaleAmnt, allRestSales.RateReduction, allRestSales.RateDiscount, allRestSales.Transport, allRestSales.VatRate);
                    MtCredit += extra.TotalTTC;
                });


            }

            return MtCredit;

        }

        /// <summary>
        /// new total sale before
        /// </summary>
        /// <param name="BranchID"></param>
        /// <param name="CustomerID"></param>
        /// <param name="Begindate"></param>
        /// <returns></returns>

        public double NewTotalAchatPeriode(int? customerID, DateTime BeginDate, DateTime EndDate)
        {

            double MtDebit = 0;

            //recuperation des ttes les ventes de la periode
            if (customerID.Value > 0)
            {
                List<Sale> salelist = context.Sales.Where(c => c.CustomerID == customerID && (c.SaleDate >= BeginDate.Date && c.SaleDate <= EndDate)).ToList();
                //salelist.ForEach(allSales =>
                //{
                //    double saleAmnt = allSales.SaleLines.Select(l => l.LineAmount).Sum();
                //    ExtraPrice extra = Util.ExtraPrices(saleAmnt, allSales.RateReduction, allSales.RateDiscount, allSales.Transport, allSales.VatRate);
                //    MtDebit += extra.TotalTTC;

                //});

                foreach (Sale allSales in salelist)
                {
                    double saleAmnt = (allSales.SaleLines == null) ? 0 : allSales.SaleLines.Select(l => l.LineAmount).Sum();
                    if (saleAmnt == null) saleAmnt = 0;
                    ExtraPrice extra = Util.ExtraPrices(saleAmnt, (allSales.RateReduction == null) ? 0 : allSales.RateReduction, (allSales.RateDiscount == null) ? 0 : allSales.RateDiscount, (allSales.Transport == null) ? 0 : allSales.Transport, (allSales.VatRate == null) ? 0 : allSales.VatRate);
                    MtDebit += (extra.TotalTTC == null) ? 0 : extra.TotalTTC;
                }

                /****** traitement des depot d'epargne provenant des retours   * *****/
                //recuperation des ttes les ventes de la periode
                //recuperation du saving acct du customer
                SavingAccount savAcct = context.SavingAccounts.Where(s => s.CustomerID == customerID).SingleOrDefault();
                if (savAcct != null)
                {
                    List<Deposit> Retdeposit = context.Deposits.Where(c => c.SavingAccountID == savAcct.ID && (c.DepositDate >= BeginDate.Date && c.DepositDate <= EndDate.Date) && c.DepositReference.StartsWith("SADE")).ToList();
                    Retdeposit.ForEach(allRestDep =>
                    {
                        MtDebit += allRestDep.Amount;
                    });
                }
                else
                {
                    List<AllDeposit> Retdeposit = context.AllDeposits.Where(c => c.CustomerID == customerID && (c.AllDepositDate >= BeginDate.Date && c.AllDepositDate <= EndDate.Date)).ToList();
                    Retdeposit.ForEach(allRestDep =>
                    {
                        MtDebit += allRestDep.Amount;
                    });
                }
            }
            return MtDebit;

        }

        /// <summary>
        /// new  total deposit before
        /// </summary>
        /// <param name="BranchID"></param>
        /// <param name="CustomerID"></param>
        /// <param name="Begindate"></param>
        /// <returns></returns>
        /// 
        public double NewTotalDepotSlicePeriode(int? customerID, DateTime BeginDate, DateTime EndDate)
        {

            double MtCredit = 0;

            //recuperation des ttes les ventes de la periode
            if (customerID.Value > 0)
            {

                //traitement des reglements des ventes par avance sur la periode
                List<CustomerSlice> perCustomerSlice = context.CustomerSlices.Where(c => c.Sale.CustomerID == customerID && (c.SliceDate >= BeginDate.Date && c.SliceDate <= EndDate) && !c.isDeposit).ToList();
                perCustomerSlice.ForEach(allslices =>
                {
                    MtCredit += allslices.SliceAmount;
                });
                /****** traitement des depots   * *****/

                List<AllDeposit> depositlist = context.AllDeposits.Where(dep => (dep.AllDepositDate >= BeginDate.Date && dep.AllDepositDate <= EndDate) && dep.CustomerID == customerID && !dep.AllDepositReference.Contains("REMOVENULL")).ToList();
                depositlist.ForEach(alldeposits =>
                {
                    MtCredit += alldeposits.Amount;
                });

                /****** traitement des retours   * *****/
                //recuperation des ttes les ventes de la periode
                List<CustomerReturn> RetSale = context.CustomerReturns.Where(c => c.Sale.CustomerID == customerID && (c.Sale.SaleDate >= BeginDate.Date && c.Sale.SaleDate <= EndDate)).ToList();
                RetSale.ForEach(allRestSales =>
                {

                    double RetsaleAmnt = allRestSales.CustomerReturnLines.Select(l => l.SaleLine.LineAmount).Sum();
                    ExtraPrice extra = Util.ExtraPrices(RetsaleAmnt, allRestSales.RateReduction, allRestSales.RateDiscount, allRestSales.Transport, allRestSales.VatRate);
                    MtCredit += extra.TotalTTC;
                });


            }

            return MtCredit;

        }

        public List<CalculSodeGenerale> CalculSodeGenerale(DateTime BeginDate, DateTime EndDate)
        {
            List<CalculSodeGenerale> res = new List<CalculSodeGenerale>();

            SqlParameter P_BeginDate = new SqlParameter("@BeginDate", BeginDate);
            SqlParameter P_EndDate = new SqlParameter("@EndDate", EndDate);
            SqlParameter P_GlobalPersonID = new SqlParameter("@GlobalPersonID", DBNull.Value);
            //res = context.Database.SqlQuery<CalculSodeGenerale>("CalculSodeGenerale @BeginDate,@EndDate", P_BeginDate, P_EndDate).ToList();

            var RetTotalDebit = new SqlParameter
            {
                ParameterName = "TotalDebit",
                SqlDbType = SqlDbType.Float,
                Direction = ParameterDirection.Output,
                Value = 0
            };
            var RetTotalAdvanced = new SqlParameter
            {
                ParameterName = "TotalAdvanced",
                SqlDbType = SqlDbType.Float,
                Direction = ParameterDirection.Output,
                Value = 0
            };

            context.Database.ExecuteSqlCommand("exec [dbo].CalculSodeGenerale @BeginDate,@EndDate,@GlobalPersonID, @TotalDebit  out,@TotalAdvanced out", P_BeginDate, P_EndDate, P_GlobalPersonID, RetTotalDebit, RetTotalAdvanced);
            context.SaveChanges();

            object ReturnValueTotalDebit = ((SqlParameter)RetTotalDebit).Value;
            double TotalDebit = (ReturnValueTotalDebit == DBNull.Value) ? 0 : (double)ReturnValueTotalDebit;

            object ReturnValueTotalAdvanced = ((SqlParameter)RetTotalAdvanced).Value;
            double TotalAdvanced = (ReturnValueTotalAdvanced == DBNull.Value) ? 0 : (double)ReturnValueTotalAdvanced;

            res.Add(new CalculSodeGenerale()
            {
                TotalDebit = TotalDebit,
                TotalAdvanced = TotalAdvanced
            });
            return res;
        }
        public List<CalculSodeGenerale> CalculSodeGenerale(DateTime BeginDate, DateTime EndDate, int GlobalPersonID)
        {
            List<CalculSodeGenerale> res = new List<CalculSodeGenerale>();

            SqlParameter P_BeginDate = new SqlParameter("@BeginDate", BeginDate);
            SqlParameter P_EndDate = new SqlParameter("@EndDate", EndDate);
            SqlParameter P_GlobalPersonID = new SqlParameter("@GlobalPersonID", GlobalPersonID);

            //res = context.Database.SqlQuery<CalculSodeGenerale>("CalculSodeGenerale @BeginDate,@EndDate,@GlobalPersonID", P_BeginDate, P_EndDate, P_GlobalPersonID).ToList();

            var RetTotalDebit = new SqlParameter
            {
                ParameterName = "TotalDebit",
                SqlDbType = SqlDbType.Float,
                Direction = ParameterDirection.Output,
                Value = 0
            };
            var RetTotalAdvanced = new SqlParameter
            {
                ParameterName = "TotalAdvanced",
                SqlDbType = SqlDbType.Float,
                Direction = ParameterDirection.Output,
                Value = 0
            };

            context.Database.ExecuteSqlCommand("exec [dbo].CalculSodeGenerale @BeginDate,@EndDate,@GlobalPersonID, @TotalDebit  out,@TotalAdvanced out", P_BeginDate, P_EndDate, P_GlobalPersonID, RetTotalDebit, RetTotalAdvanced);
            context.SaveChanges();

            object ReturnValueTotalDebit = ((SqlParameter)RetTotalDebit).Value;
            double TotalDebit = (ReturnValueTotalDebit == DBNull.Value) ? 0 : (double)ReturnValueTotalDebit;

            object ReturnValueTotalAdvanced = ((SqlParameter)RetTotalAdvanced).Value;
            double TotalAdvanced = (ReturnValueTotalAdvanced == DBNull.Value) ? 0 : (double)ReturnValueTotalAdvanced;

            res.Add(new CalculSodeGenerale()
            {
                TotalDebit = TotalDebit,
                TotalAdvanced = TotalAdvanced
            });
            return res;
        }
        public List<CalculSodeGenerale> CalculSodeGenerale(int BranchID, DateTime BeginDate, DateTime EndDate)
        {
            List<CalculSodeGenerale> res = new List<CalculSodeGenerale>();

            SqlParameter P_BeginDate = new SqlParameter("@BeginDate", BeginDate);
            SqlParameter P_EndDate = new SqlParameter("@EndDate", EndDate);
            SqlParameter P_BranchID = new SqlParameter("@BranchID", BranchID);

            var RetTotalDebit = new SqlParameter
            {
                ParameterName = "TotalDebit",
                SqlDbType = SqlDbType.Float,
                Direction = ParameterDirection.Output,
                Value = 0
            };
            var RetTotalAdvanced = new SqlParameter
            {
                ParameterName = "TotalAdvanced",
                SqlDbType = SqlDbType.Float,
                Direction = ParameterDirection.Output,
                Value = 0
            };

            context.Database.ExecuteSqlCommand("exec [dbo].CalculSodeGeneraleBranch @BeginDate,@EndDate,@BranchID, @TotalDebit  out,@TotalAdvanced out", P_BeginDate, P_EndDate, P_BranchID, RetTotalDebit, RetTotalAdvanced);
            context.SaveChanges();

            object ReturnValueTotalDebit = ((SqlParameter)RetTotalDebit).Value;
            double TotalDebit = (ReturnValueTotalDebit == DBNull.Value) ? 0 : (double)ReturnValueTotalDebit;

            object ReturnValueTotalAdvanced = ((SqlParameter)RetTotalAdvanced).Value;
            double TotalAdvanced = (ReturnValueTotalAdvanced == DBNull.Value) ? 0 : (double)ReturnValueTotalAdvanced;

            res.Add(new CalculSodeGenerale()
            {
                TotalDebit = TotalDebit,
                TotalAdvanced = TotalAdvanced
            });
            return res;
        }
    }
}
