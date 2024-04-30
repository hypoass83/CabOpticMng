using FatSod.Security.Entities;
using FatSod.Supply.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FatSod.Supply.Abstracts
{
    public interface IProductDamage : IRepositorySupply<ProductDamage>
    {
        ProductDamage DoProductDamage(ProductDamage productDamage,int UserConnect);
        ProductDamage ValidateProductDamage(ProductDamage productDamage);
        bool CancelProductDamage(int productDamageID);
        ProductDamage UpdateProductDamage(ProductDamage productDamage);
    }
}
