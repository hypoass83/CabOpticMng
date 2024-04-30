using FatSod.Supply.Abstracts;
using FatSod.Supply.Entities;
using FatSod.Supply.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FatSod.DataContext.Initializer;
using System.Transactions;

namespace FatSod.DataContext.Repositories
{
    public class PMRepository : RepositorySupply<PaymentMethod>, IPaymentMethod
    {
        public String GetPaymentMethod(Purchase purchase)
        {
            String res = "";

            Purchase purchase1 = context.Purchases.Find(purchase.PurchaseID);

            if (purchase1 != null && purchase1.PurchaseID > 0)
            {
                if (purchase1 is TillPurchase)
                { res = CodeValue.Accounting.DefaultCodeAccountingSection.CODECAIS; }
                if (purchase1 is BankPurchase)
                { res = CodeValue.Accounting.DefaultCodeAccountingSection.CODEBANK; }
            }
            return res;
        }

        public int GetPaymentMethodID(int purchaseID)
        {
            int res = 0;

            Purchase purchase1 = context.Purchases.Find(purchaseID);

            if (purchase1 != null && purchase1.PurchaseID > 0)
            {
                if (purchase1 is TillPurchase)
                {
                    TillPurchase tillPur = context.TillPurchases.Find(purchaseID);
                    res = tillPur.Till.ID;
                }

                if (purchase1 is BankPurchase)
                {
                    BankPurchase bankPur = context.BankPurchases.Find(purchaseID);
                    res = bankPur.Bank.ID;
                }
            }
            return res;
        }

        public void RemovePaymentMethod(int paymentMethodID)
        {

                try
                {
                    using (TransactionScope ts = new TransactionScope())
                    {
                        PaymentMethod paymentMethodFind = context.PaymentMethods.Find(paymentMethodID);
                        if (paymentMethodFind != null)
                        {
                            if (paymentMethodFind is Till)
                            {
                                UserTill usertill = context.UserTills.Where(u => u.TillID == paymentMethodFind.ID).SingleOrDefault();
                                if (usertill != null)
                                {
                                    context.UserTills.Remove(usertill);
                                }

                                Till paymentMethodToDelete = (Till)paymentMethodFind;
                                paymentMethodToDelete.Account = null;
                                paymentMethodToDelete.Branch = null;
                                context.PaymentMethods.Remove(paymentMethodToDelete);
                            }
                            else if (paymentMethodFind is Bank)
                            {
                                
                                Bank paymentMethodToDelete = (Bank)paymentMethodFind;
                                paymentMethodToDelete.Account = null;
                                paymentMethodToDelete.Branch = null;
                                context.PaymentMethods.Remove(paymentMethodToDelete);
                            }
                            else if (paymentMethodFind is SavingAccount)
                            {
                                SavingAccount paymentMethodToDelete = (SavingAccount)paymentMethodFind;
                                paymentMethodToDelete.Customer = null;
                                paymentMethodToDelete.Branch = null;
                                context.PaymentMethods.Remove(paymentMethodToDelete);
                            }
                        }
                        context.SaveChanges();
                        //transaction.Commit();
                        ts.Complete();
                    }
                }
                catch (Exception e)
                {
                    //transaction.Rollback();
                    throw new Exception("Error delete : " + e.Message);

                }

            //}
        }

    }
}
