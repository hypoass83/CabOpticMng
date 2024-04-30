using FatSod.Supply.Abstracts;
using FatSod.Supply.Entities;
using System;
using System.Linq;
using FatSod.DataContext.Concrete;

namespace FatSod.DataContext.Repositories
{
    public class CollectifAccountRepository : RepositorySupply<CollectifAccount>, ICollectifAccount
    {
        

        public CollectifAccountRepository(EFDbContext context1)
            : base(context1)
        {
            this.context = context1;

        }

        public CollectifAccountRepository()
            : base()
        {


        }
        


        /// <summary>
        /// methode permettant de generer et creer automatiquement un compte Collectif.
        /// </summary>
        /// <param name="AccountingSectionCode">Chapitre Comptable du CollectifAccount dont on souhaite obtenir le compte collectif</param>
        /// <param name="CollectifAccountLabel">Label du CollectifAccount dont on souhaite obtenir le compte collectif</param>
        /// <returns></returns>
        public CollectifAccount GetCollectifAccount(string AccountingSectionCode, string CollectifAccountLabel)
        {


            CollectifAccount res = new CollectifAccount();

            res = this.FindAll.SingleOrDefault(ca => ca.CollectifAccountLabel == CollectifAccountLabel);
            //Si le compte collectif existe déjà, on sort très rapidement
            if (res != null && res.CollectifAccountID > 0) { return res; }

            int maxCollectifAccountNumber = this.FindAll.Where(ca1 => ca1.AccountingSection.AccountingSectionCode == AccountingSectionCode).Max(ca => ca.CollectifAccountNumber);
            maxCollectifAccountNumber += 1;

            res = context.CollectifAccounts.AsNoTracking().SingleOrDefault(ca => ca.CollectifAccountNumber == maxCollectifAccountNumber);
            //Si le compte collectif existe déjà, on sort très rapidement
            if (res != null && res.CollectifAccountID > 0) { return res; }

            AccountingSection accSec = context.AccountingSections.AsNoTracking().SingleOrDefault(acs => acs.AccountingSectionCode == AccountingSectionCode);

            if (accSec != null && accSec.AccountingSectionID > 0)
            {

                CollectifAccount collectifAccount = new CollectifAccount()
                {
                    CollectifAccountNumber = maxCollectifAccountNumber,
                    CollectifAccountLabel = CollectifAccountLabel,
                    AccountingSectionID = accSec.AccountingSectionID
                };

                res = this.Create(collectifAccount);
            }
            else
            {
                throw new Exception("The Given Accounting Section doesn't Exist");
            }

            return res;

        }

        public int getCompteCollectifID(string AccountingSectionCode, string CollectifAccountLabel)
        {

            int finalres = 0;
            CollectifAccount res = new CollectifAccount();

            res = this.FindAll.SingleOrDefault(ca => ca.CollectifAccountLabel == CollectifAccountLabel);
            //Si le compte collectif existe déjà, on sort très rapidement
            if (res != null && res.CollectifAccountID > 0) {
                finalres = res.CollectifAccountID;
                return finalres; }

            int maxCollectifAccountNumber = this.FindAll.Where(ca1 => ca1.AccountingSection.AccountingSectionCode == AccountingSectionCode).Max(ca => ca.CollectifAccountNumber);
            maxCollectifAccountNumber += 1;

            res = context.CollectifAccounts.AsNoTracking().SingleOrDefault(ca => ca.CollectifAccountNumber == maxCollectifAccountNumber);
            //Si le compte collectif existe déjà, on sort très rapidement
            if (res != null && res.CollectifAccountID > 0) {
                finalres = res.CollectifAccountID;
                return finalres;
            }

            AccountingSection accSec = context.AccountingSections.AsNoTracking().SingleOrDefault(acs => acs.AccountingSectionCode == AccountingSectionCode);

            if (accSec != null && accSec.AccountingSectionID > 0)
            {

                CollectifAccount collectifAccount = new CollectifAccount()
                {
                    CollectifAccountNumber = maxCollectifAccountNumber,
                    CollectifAccountLabel = CollectifAccountLabel,
                    AccountingSectionID = accSec.AccountingSectionID
                };

                res = this.Create(collectifAccount);
                finalres = res.CollectifAccountID;
            }
            else
            {
                throw new Exception("The Given Accounting Section doesn't Exist");
            }

            return finalres;

        }

    }
}
