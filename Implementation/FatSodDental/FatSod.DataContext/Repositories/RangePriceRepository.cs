using FatSod.DataContext.Concrete;
using FatSod.Supply.Abstracts;
using FatSod.Supply.Entities;
using System.Collections.Generic;
using System.Linq;

namespace FatSod.DataContext.Repositories
{
    /// <summary>
    /// RangePriceRepository
    /// </summary>
    /// 
    /*
    public class RangePriceRepository : RepositorySupply<LensNumberRangePrice>//, ILensNumberRangePrice
    {
        public RangePriceRepository()
        {

        }

        public RangePriceRepository(EFDbContext ctx)
            : base(ctx)
        {

        }


        /// <summary>
        /// Permet d'obtenir le prix d'un verre 
        /// </summary>
        /// <param name="number">Numéro du verre</param>
        /// <param name="category">Catégorie du verre</param>
        /// <returns>Prix du verre</returns>
        public LensNumberRangePrice GetPrice(LensNumber number, LensCategory category)
        {
            LensNumberRangePrice res = new LensNumberRangePrice();

            res = this.FindAll.FirstOrDefault(lrnp => lrnp.LensCategoryID == category.CategoryID && (lrnp.Contains(number, category)));
            /*
            List<LensNumberRangePrice> posibilities = this.FindAll.Where(lrnp => lrnp.LensCategoryID == category.CategoryID).ToList();
            foreach (LensNumberRangePrice price in posibilities)
            {
                if (price.Contains(number, category))
                {
                    res = price;
                    break;
                }
            }
            */
    /*
            return res;
        }

        public LensNumberRangePrice GetPrice(Lens product)
        {
            return this.GetPrice(product.LensNumber, product.LensCategory);
        }

        public LensNumberRangePrice GetPrice(int lensID)
        {
            Lens product = context.Lenses.Find(lensID);

            return GetPrice(product.LensNumber, product.LensCategory);

        }
    }
*/
}
