using FatSod.Supply.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FatSod.Supply.Abstracts
{
	public interface ISupplierOrder : IRepositorySupply<SupplierOrder>
    {
	    SupplierOrder CreateSupplierOrder(SupplierOrder supplierOrder);
	    bool DeleteSupplierOrder(int supplierOrderID);
	    SupplierOrder UpdateSupplierOrder(SupplierOrder supplierOrder);
        SupplierOrder SaveSupplierOrder(SupplierOrder supplierOrder);
        bool RemoveSupplierOrder(int SupplierOrderID);
    }
}
