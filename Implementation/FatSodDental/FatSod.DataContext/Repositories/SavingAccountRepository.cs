﻿using System;
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
using FatSod.Security.Abstracts;


namespace FatSod.DataContext.Repositories
{
    public class SavingAccountRepository : RepositorySupply<SavingAccount>, ISavingAccount
    {
        private IDeposit _depositRepositiry;

        public SavingAccountRepository(EFDbContext ctx)
            : base(ctx)
        {
            this.context = ctx;
            _depositRepositiry = new DepositRepository(ctx);


            //context = new EFDbContext();
        }

        public SavingAccountRepository()
        {
            _depositRepositiry = new DepositRepository();

            //context = new EFDbContext();
        }

        /// <summary>
        /// Cette méthode permet de payer la vente d'un client en utilisant son compte.
        /// Pour le faire, nous allons créer les tranches client ayant comme mode de paiement le compte du client.
        /// Ici, on règle toute la vente ou le montant restant de la vente
        /// </summary>
        /// <param name="sale">Vente dont on souhaite régler</param>
        /// <param name="dateOperation">Date de règlement</param>
        /// <returns></returns>
        public bool PayASale(Sale sale, DateTime dateOperation, string representant)
        {
            bool res = false;
            double remainderAmount = _depositRepositiry.SaleRemainder(sale);
            this.PayASale(sale, remainderAmount, dateOperation, representant);
            return res;
        }
        public bool PayASale(Sale sale, double amount, DateTime dateOperation, string representant)
        {
            bool res = false;

            Customer currentCustomer = context.Customers.SingleOrDefault(c => c.GlobalPersonID == sale.CustomerID);
            if (this.IsCustomerSAExist(currentCustomer) == false)
            {
                throw new Exception("Sorry! " + currentCustomer.CustomerFullName + " " + "Doesn't have a Saving Account");
            }
            SavingAccount csa = this.GetCustSavAccount(currentCustomer);
            if (this.GetSavingAccountBalance(currentCustomer) < amount)
            {
                throw new Exception("Sorry! " + currentCustomer.CustomerFullName + " " + "Doesn't have sufficient money in his Saving Account");
            }

            double saleRemainder = _depositRepositiry.SaleRemainder(sale);

            //si le montant donc on souhaite versé pour le paiement d'une vente est supérieur au montant restant, erreur : que faire de l'excédent
            if (saleRemainder < amount)
            {
                throw new Exception("We don't know what to do with the Surplus amount! "
                                    + "the sale remainder amount is " + saleRemainder + " but you enter " + amount +
                                       " the surplus amount is " + (amount - saleRemainder));
            }

            CustomerSlice cs = new CustomerSlice
            {
                DeviseID = sale.DeviseID,
                PaymentMethodID = csa.ID,
                SaleID = sale.SaleID,
                SliceAmount = amount,
                SliceDate = dateOperation,
                Representant  = representant,
                Reference = sale.SaleReceiptNumber,
                //OperatorID = SessionGlobalPersonID
            };
            context.CustomerSlices.Add(cs);
            context.SaveChanges();
            
            return res;
        }
        public bool PayAllSales(List<Sale> sales, DateTime dateOperation)
        {
            bool res = false;
            foreach (Sale s in sales)
            {
                res = this.PayASale(s, dateOperation, s.PoliceAssurance);
            }
            return res;
        }

        /// <summary>
        /// Cette méthode retourne la somme d'argent qu'un client a dans son compte.
        /// Dans cette méthode, on ne considère pas qu'il paie ses ventes à crédit
        /// la formule est donc : somme des dépots - somme des tranches clients payés avec le compte du client et non
        /// somme des ventes - (somme des dépots + somme de tranches clients - sommes des tranches client de type cpte client )
        /// </summary>
        /// <param name="customer">Client dont on souhaite obtenir son solde</param>
        /// <returns></returns>
        public double GetSavingAccountBalance(Customer customer)
        {
            double res = 0;
            if (this.IsCustomerSAExist(customer) == false)
            {
                return res;
            }
            SavingAccount csa = this.GetCustSavAccount(customer);
            //somme des dépôts du client
            List<Deposit> listSeposit =  context.Deposits.Where(d => d.SavingAccountID == csa.ID).ToList();
            double depositSum = (listSeposit != null && listSeposit.Count > 0) ? listSeposit.Sum(d1 => d1.Amount) : 0;
            //somme des paiements éffectués à l'aide du compte d'épargne du client
            List<CustomerSlice> listSlices = context.CustomerSlices.Where(cs => cs.PaymentMethodID == csa.ID).ToList();
            double saCustSlice = (listSlices != null && listSlices.Count > 0) ? listSlices.Sum(csa1 => csa1.SliceAmount) : 0;
            res = depositSum - saCustSlice;
            return res;
        }
        public double GetSavingAccountBalance(Customer customer,DateTime BeginDate)
        {
            double res = 0;
            if (this.IsCustomerSAExist(customer) == false)
            {
                return res;
            }
            SavingAccount csa = this.GetCustSavAccount(customer);
            //somme des dépôts du client exclu les dpot de rtour
            List<Deposit> listSeposit =  context.Deposits.Where(d => d.SavingAccountID == csa.ID && d.DepositDate < BeginDate.Date).ToList();
            double depositSum = (listSeposit != null && listSeposit.Count > 0) ? listSeposit.Sum(d1 => d1.Amount) : 0;
            //somme des paiements éffectués à l'aide du compte d'épargne du client
            List<CustomerSlice> listSlices = context.CustomerSlices.Where(cs => cs.PaymentMethodID == csa.ID && cs.SliceDate<BeginDate.Date).ToList();
            double saCustSlice = (listSlices != null && listSlices.Count > 0) ? listSlices.Sum(csa1 => csa1.SliceAmount) : 0;
            res = depositSum - saCustSlice;
            return res;
        }
        /// <summary>
        /// Cette méthode retourne la somme d'argent que le client a dans son compte d'épargne. Cette méthode vise beaucoup plus à trouver le client general public 
        /// particulier
        /// la formule est : somme des dépôts d'épargne - sommes des tranches clients payées en utilisant le compte d'épargne
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        public double GetSavingAccountBalance(Customer customer, string Representant)
        {
            double res = 0;
            if (this.IsCustomerSAExist(customer) == false)
            {
                throw new Exception("Sorry! " + customer.CustomerFullName + " " + "Doesn't have a Saving Account");
            }
            SavingAccount csa = this.GetCustSavAccount(customer);
            //somme des dépôts du client
            List<Deposit> listSeposit = context.Deposits.Where(d => d.SavingAccountID == csa.ID).ToList();

            Representant = Representant.TrimEnd().TrimStart().ToLower();

            double depositSum = (listSeposit != null && listSeposit.Count > 0) ? listSeposit.Where(d => d.Representant.TrimEnd().TrimStart().ToLower() == Representant).ToList().Sum(d1 => d1.Amount) : 0;
            //somme des paiements éffectués à l'aide du compte d'épargne du client
            List<CustomerSlice> listSlices = context.CustomerSlices.Where(cs => cs.PaymentMethodID == csa.ID).ToList();
            double saCustSlice = (listSlices != null && listSlices.Count > 0) ? listSlices.Where(csa2 => csa2.Representant.TrimEnd().TrimStart().ToLower() == Representant).ToList().Sum(csa1 => csa1.SliceAmount) : 0;
            res = depositSum - saCustSlice;
            return res;
        }
        public SavingAccount GetCustSavAccount(Customer customer)
        {
            return this.context.SavingAccounts.SingleOrDefault(sa => sa.CustomerID == customer.GlobalPersonID);
        }
        /// <summary>
        /// la formule est donc : somme des ventes - (somme des dépots + somme de tranches clients - sommes des tranches client de type cpte client )
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        public double GetCustomerGeneralBalance(Customer customer)
        {
            double res = 0;

            return res;
        }

        public SavingAccount CreateSavingAccount(int CustomerID, int BranchID)
        {
            SavingAccount csa = null;
            try
            {
                Customer cust = this.context.Customers.Find(CustomerID);
                if (IsCustomerSAExist(cust))
                {
                    csa = this.FindAll.SingleOrDefault(sa => sa.CustomerID == CustomerID);
                }
                else
                {
                    csa = new SavingAccount
                    {
                        BranchID= BranchID,
                        Code = cust.CustomerFullName + " " + cust.CNI,
                        CustomerID = CustomerID,
                        Description = "This is the saving account of " + cust.CustomerFullName,
                        Name = cust.CustomerFullName + " " + cust.CNI + " Saving Account",
                    };
                    this.context.SavingAccounts.Add(csa);
                    this.context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                //If an errors occurs, we cancel all changes in database
                throw new Exception("Une erreur s'est produite lors de la création de la tranche car : " + e.StackTrace);
            }
            return csa;
        }
        /// <summary>
        /// Cette méthode permet de faire un dépot sur le compte du client
        /// </summary>
        /// <param name="deposit"></param>
        /// <returns></returns>        
        public int DoADeposit(Deposit deposit, bool isAccountable, int userConnect)
        {
            context = new EFDbContext();
            int depositID = 0;
            //Begin of transaction
           
                try
                {
                    //bool res = false;
                    SavingAccount csa = null;
                    Customer cust = this.context.Customers.Find(deposit.CustomerID);
                    if (IsCustomerSAExist(cust))
                    {
                        csa = this.FindAll.SingleOrDefault(sa => sa.CustomerID == deposit.CustomerID);
                        deposit.SavingAccountID = csa.ID;
                    }
                    else
                    {
                        csa = new SavingAccount
                        {
                            Code = cust.CustomerFullName + " " + cust.CNI,
                            CustomerID = deposit.CustomerID,
                            Description = "This is the saving account of " + cust.CustomerFullName,
                            Name = cust.CustomerFullName + " " + cust.CNI + " Saving Account",
                            BranchID = deposit.BranchID
                        };
                        //csa = this.Create(csa);
                        this.context.SavingAccounts.Add(csa);
                        this.context.SaveChanges();
                        deposit.SavingAccountID = csa.ID;
                    }
                    deposit.OperatorID = userConnect;
                this.context.Entry(deposit).State = EntityState.Detached;
                    this.context.Deposits.Add(deposit);
                    this.context.SaveChanges();

                    depositID = deposit.DepositID;
                    if (isAccountable) 
                    { 
                        //comptabilisation des depots d'epargne
                        Deposit depositAccountable = new Deposit
                        {
                            BranchID = deposit.BranchID,
                            DepositID = deposit.DepositID,
                            Amount = deposit.Amount,
                            DepositDate = deposit.DepositDate,
                            PaymentMethodID = deposit.PaymentMethodID,
                            DeviseID=deposit.DeviseID,
                            StatutSale = SalePurchaseStatut.Advanced, //on considere ke le client fait une avance
                            SavingAccountID = deposit.SavingAccountID,
                            Representant = deposit.Representant,
                            DepositReference = deposit.DepositReference,
                            CustomerID = deposit.CustomerID
                        };
                        ////IAccountOperation opaccount = new AccountOperationRepository(context);
                        ////bool res = opaccount.ecritureComptableFinal(depositAccountable);
                        ////if (!res)
                        ////{
                        ////    throw new Exception("Une erreur s'est produite lors de comptabilisation du dépôt d'epargne ");
                        ////}
                    }
                }
                catch (Exception e)
                {
                    //If an errors occurs, we cancel all changes in database
                    throw new Exception("Une erreur s'est produite lors de la création de la tranche car : " + e.StackTrace);
                }
                
                return depositID;
            
            
        }
        
        /// <summary>
        /// Cette méthode vérifie si le compte d'épargne de ce client existe
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        bool IsCustomerSAExist(Customer customer)
        {
            bool res = false;
            SavingAccount csa = this.FindAll.SingleOrDefault(sa => sa.CustomerID == customer.GlobalPersonID);

            if (csa != null && csa.ID > 0) { res = true; }

            return res;

        }

        /// <summary>
        /// Cette méthode permet de régler tous les achats d'un client en utilisant le montant en cas de reste, on fera un dépôt dans le compte du client
        /// </summary>
        /// <param name="amount">Montant d'argent versé par le client</param>
        /// <param name="dateOperation"></param>
        /// <returns></returns>
        public bool PayAllSales(int CustomerID, double amount, DateTime dateOperation, string representant, int UserConect, int BranchID)
        {
            bool res = false;

            List<Sale> customerAllUnPaidSales = _depositRepositiry.CustomerAllUnPaidSalesStockLens(context.Customers.Find(CustomerID));

            foreach (Sale s in customerAllUnPaidSales)
            {

                if (amount > 0 && s.SaleTotalPriceRemainder <= amount)   //Le montant de dépôt a retirer du compte du client peut permettre de régler 
                                                                        //plusieurs ventes
                {
                    this.PayASale(s, s.SaleTotalPriceRemainder, dateOperation, representant);
                    amount -= s.SaleTotalPriceRemainder;

                    //Comptabilisation

                    continue;
                }

                if (amount > 0 && s.SaleTotalPriceRemainder > amount)//le montant saisit ne peut pas permettre de régler plusieurs ventes
                {
                    this.PayASale(s, amount, dateOperation, representant);
                    //Comptabilisation
                    break;
                }
            }

            //EcritureSneack
            IMouchar opSneak = new MoucharRepository(context);
            res = opSneak.InsertOperation(UserConect, "SUCCESS", "PAID WITH SAVING ACCOUNT FOR CUSTOMER " + representant + " -AMOUNT- " + amount, "PayAllSales", dateOperation, BranchID);
            if (!res)
            {
                throw new Exception("Une erreur s'est produite lors de la journalisation ");
            }

            return res;

        }

        /// <summary>
        /// Cette méthode permet de régler tous les achats d'un client en utilisant le montant en cas de reste, on fera un dépôt dans le compte du client
        /// </summary>
        /// <param name="amount">Montant d'argent versé par le client</param>
        /// <param name="dateOperation"></param>
        /// <returns></returns>
        public bool PayAllSales(int CustomerID, DateTime dateOperation, int UserConect, int BranchID)
        {
            bool res = false;

            Customer customer = context.Customers.Find(CustomerID);

            double amount = this.GetSavingAccountBalance(customer);
            PayAllSales(CustomerID, amount, dateOperation, customer.CustomerFullName, UserConect, BranchID);

            return res;
        }

    }
}

