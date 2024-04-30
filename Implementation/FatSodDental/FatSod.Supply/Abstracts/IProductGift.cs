using FatSod.Security.Entities;
using FatSod.Supply.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FatSod.Supply.Abstracts
{
    public interface IProductGift : IRepositorySupply<ProductGift>
    {
        ProductGift DoProductGift(ProductGift ProductGift,int UserConnect);
        ProductGift ValidateProductGift(ProductGift ProductGift);
        bool CancelProductGift(int ProductGiftID);
        ProductGift UpdateProductGift(ProductGift ProductGift);
    }
}
