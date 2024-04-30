using FatSod.DataContext.Concrete;
using FatSod.DataContext.Initializer;
using FatSod.Supply.Abstracts;
using FatSod.Supply.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FatSod.DataContext.Repositories
{
    public class AccountRepository : RepositorySupply<Account>, IAccount
    {
        //private IAccount @object;

        //public AccountRepository(EFDbContext context1)
        //{
        //    this.context = context1;

        //}
        private ICollectifAccount _colAccRepo;// = new CollectifAccountRepository();

       
        public AccountRepository(EFDbContext context)
        {
            this.context = context;
        }
         public AccountRepository()
            : base()
        {

        }

        public AccountRepository(ICollectifAccount colAccRepo)
           : base()
        {
            this._colAccRepo = colAccRepo;
        }

        //public AccountRepository(IAccount @object)
        //{
        //    this.@object = @object;
        //}

        public bool CheckIfAcctExit( int accountNumber)
        {
            bool res = false;
            Account acct = context.Accounts.SingleOrDefault(acc => acc.AccountNumber == accountNumber);
            if (acct != null)  res=true;
            return res;
        }
        public Account GenerateAccountNumber(int collectiveAcountID, string AccountLabel, bool isManualPosting)
        {

            //si le compte existe pas besoin de parcourir tout le monde entier pour
            /*Account res = context.Accounts.SingleOrDefault(acc => acc.CollectifAccountID == collectiveAcountID && acc.AccountLabel == AccountLabel);
            if (res != null && res.AccountID > 0) return res;
               
            int acctNumber = 0;
            //il s'agit de retourner un numero unique de 9 caracteres en fonction du CollectiveAcount
            //recuperation du plus grand account number 
            List<Account> lstmaxAcct = this.context.Accounts.Where(c=>c.CollectifAccountID==collectiveAcountID).ToList();//.OrderBy(o=>o.AccountNumber).LastOrDefault();
            Account maxAcct = lstmaxAcct!=null? lstmaxAcct.OrderBy(o=>o.AccountNumber).LastOrDefault():null;
            if (maxAcct==null)
            {
                //recup du collective acct number
                CollectifAccount colActNum=this.context.CollectifAccounts.Find(collectiveAcountID);
                if (colActNum==null)
                {
                    throw new Exception("Please you must create the collective Account before proceed");
                }
                else 
                {
                    //fabrication du new acct number
                    acctNumber = Convert.ToInt32(colActNum.CollectifAccountNumber.ToString() + "0001");
                }
            }
            else //si existance d'un cpte de ce collective acct
            {
                acctNumber=maxAcct.AccountNumber+1;
            }

            if (acctNumber.ToString().Length!=9)
            {
                throw new Exception("System Error : Wrong length Account Number. Please Contact your Provider");
            }
            */
            int acctNumber = AfficheAccountNumber(collectiveAcountID);
            bool checkacct = CheckIfAcctExit(acctNumber);
            if (checkacct)
            {
                throw new Exception("This Account already exist");
            }
            //persitance du cpte
            Account accToPersit = new Account 
            {
                AccountNumber=acctNumber,
                AccountLabel = AccountLabel,
                isManualPosting = isManualPosting,
                CollectifAccountID=collectiveAcountID
            };
            context.Accounts.Add(accToPersit);
            context.SaveChanges();
            return accToPersit;
               
        }

        public int AfficheAccountNumber(int collectiveAcountID)
        {

            int acctNumber = 0;
            //il s'agit de retourner un numero unique de 9 caracteres en fonction du CollectiveAcount
            //recuperation du plus grand account number 
            List<Account> lstmaxAcct = this.context.Accounts.Where(c => c.CollectifAccountID == collectiveAcountID).ToList();
            //Account maxAcct = lstmaxAcct != null ? lstmaxAcct.OrderBy(o => o.AccountNumber).LastOrDefault() : null;
            if (lstmaxAcct.Count == 0)
            {
                //recup du collective acct number
                CollectifAccount colActNum = this.context.CollectifAccounts.Find(collectiveAcountID);
                if (colActNum == null)
                {
                    throw new Exception("Please you must create the collective Account before proceed");
                }
                else
                {
                    //fabrication du new acct number
                    acctNumber = Convert.ToInt32(colActNum.CollectifAccountNumber.ToString() + "0001");
                }
            }
            else //si existance d'un cpte de ce collective acct
            {
                acctNumber = lstmaxAcct.Max(s=>s.AccountNumber) + 1;
            }

            if (acctNumber.ToString().Length != 9)
            {
                throw new Exception("System Error : Wrong length Account Number. Please Contact your Provider");
            }

            return acctNumber;

        }
        /// <summary>
        /// Permet de générer le numéro de compte 
        /// </summary>
        /// <param name="AccountingSectionCode">Champitre Comptable du compte</param>
        /// <param name="CollectifAccountLabel">Libélé du compte compte à créer si neccesaire</param>
        /// <param name="AccountLabel">Libéllé du compte à crééer si necessaire</param>
        /// <returns></returns>
        public Account GenerateAccountNumber(string AccountingSectionCode, string CollectifAccountLabel, string AccountLabel, bool isManualPosting)
        {

            Account res = new Account();
            ICollectifAccount _colAccRepo = new CollectifAccountRepository(context);

            CollectifAccount cAcc = _colAccRepo.GetCollectifAccount(AccountingSectionCode, CollectifAccountLabel);

            //S'il s'agit d'autre chose qu'un produit, cette méthode fonctionne comme à la coutumé
            if (AccountingSectionCode != CodeValue.Accounting.DefaultCodeAccountingSection.CODEPROD)
            {
                res = GenerateAccountNumber(cAcc.CollectifAccountID, AccountLabel, isManualPosting);
                return res;
            }

            //recuperation du compte unique du produit 
            res = this.context.Accounts.SingleOrDefault(c => c.CollectifAccountID == cAcc.CollectifAccountID && c.AccountLabel == AccountLabel);

            //le compte unique n'existe pas il faut le créér
            if (res == null || res.AccountID <= 0)
            {
                res = GenerateAccountNumber(cAcc.CollectifAccountID, AccountLabel, isManualPosting);
            }

            return res;

        }
    }
}
