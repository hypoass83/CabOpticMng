using FatSod.Supply.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FatSod.Supply.Abstracts
{
    public interface IPaymentMethod : IRepositorySupply<PaymentMethod>
    {
        String GetPaymentMethod(Purchase purchase);
        int GetPaymentMethodID(int purchaseID);
        void RemovePaymentMethod(int paymentMethodID);
    }
}
