using FatSod.Supply.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FatSod.Supply.Abstracts
{
    public interface IAuthoriseSale : IRepositorySupply<AuthoriseSale>
    {
        AuthoriseSale SaveChanges(AuthoriseSale sale, String HeureVente, int UserConect,bool IsInHouseCustomer);
       
        //Double SaleTotalPriceAdvance(Sale sale);
        AuthoriseSale PersistSale(AuthoriseSale sale, int UserConect, bool IsInHouseCustomer);

        OrderLens PersistCustomerOrderLine(AuthoriseSaleLine saleLine);
        OrderLens CreateOrderLens(OrderLens currentProduct);
    }
}
