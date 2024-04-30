using FatSod.Supply.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FatSod.Supply.Abstracts
{
    public interface IProductTransfert : IRepositorySupply<ProductTransfert>
    {
        Purchase CreateProductTransfert(ProductTransfert productTransfert);
        bool DeletePurchase(int PurchaseID);
        Purchase UpdatePurchase(ProductTransfert productTransfert);
        void CreateProdLocForProdTransLine(ProductTransfertLine prodTransLine, ProductTransfert productTransfert, bool update);


    }
}
