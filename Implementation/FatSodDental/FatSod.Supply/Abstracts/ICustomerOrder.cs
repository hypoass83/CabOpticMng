using FatSod.Supply.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FatSod.Supply.Abstracts
{
    public interface ICustomerOrder : IRepositorySupply<CustomerOrder>
    {
        CustomerOrder CreateCustomerOrder(CustomerOrder customerOrder, List<CustomerOrderLine> newsCustomerOrderLine, int UserConect);
        CustomerOrder UpdateCustomerOrder(CustomerOrder customerOrder,string heureVente, List<CustomerOrderLine> newsCustomerOrderLine, int UserConect, int spray, int boitier);
        CustomerOrder UpdateCustomerOrderLensOrder(CustomerOrder customerOrder, List<CustomerOrderLine> newsCustomerOrderLine);
        CustomerOrder CreateCustomerOrderLensOrder(CustomerOrder customerOrder, List<CustomerOrderLine> newsCustomerOrderLine);
        void DeleteOrderLensOrder(int CustomerOrderID);
        CustomerOrder SaveChanges(CustomerOrder customerOrder, int UserConect);
        string generateBill(int numeroFacture);

        CustomerOrder SaveDirectChanges(CustomerOrder customerOrder,string heureVente, int UserConect, int spray, int boitier, bool isDirectBill = false);

        CustomerOrder DeleteValidatedBill(int ID, string DeleteReason, int UserConect,DateTime OperationDate);
    }
}
