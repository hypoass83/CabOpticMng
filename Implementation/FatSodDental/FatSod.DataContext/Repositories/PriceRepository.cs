using FastSod.Utilities.Util;
using FatSod.Budget.Entities;
using FatSod.DataContext.Concrete;
using FatSod.DataContext.Initializer;
using FatSod.Ressources;
using FatSod.Security.Abstracts;
using FatSod.Security.Entities;
using FatSod.Supply.Abstracts;
using FatSod.Supply.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FatSod.DataContext.Repositories
{
	public class PriceRepository : RepositorySupply<LensNumberRangePrice>, ILensNumberRangePrice
	{
		public PriceRepository(EFDbContext context)
		{
			this.context = context;
		}
        public PriceRepository()
			: base()
		{

		}

        public LensNumberRangePrice GetPrice(LensNumber number, LensCategory category)
        {
            LensNumberRangePrice res = new LensNumberRangePrice();

            //res = context.LensNumberRangePrices.AsNoTracking().Where(lnp => lnp.LensCategoryID == category.CategoryID).ToList().SingleOrDefault(lnp => lnp.Contains(number, category));
            
            //List<LensNumberRangePrice> possibilities = context.LensNumberRangePrices.AsNoTracking().Where(lnp => lnp.LensCategoryID == category.CategoryID).ToList();
            List<LensNumberRangePrice> possibilities = (from lnp in this.context.LensNumberRangePrices
                                                        where lnp.LensCategoryID == category.CategoryID
                                                        select lnp).ToList();

            foreach (LensNumberRangePrice lnrp in possibilities)
            {
                if (lnrp.Contains(number, category))
                {
                    res = lnrp;
                    break;
                }
            }
            
            return res;
        }

        public LensNumberRangePrice GetPrice(Lens product)
        {
            return this.GetPrice(product.LensNumber, product.LensCategory);
        }

        public LensNumberRangePrice GetPrice(int lensID)
        {
            //Lens product = context.Lenses.Find(lensID);
            Lens product = (from Len in this.context.Lenses
                            where Len.ProductID == lensID
                            select Len).SingleOrDefault();
            if (product != null) return GetPrice(product.LensNumber, product.LensCategory);
            else return new LensNumberRangePrice(); 

        }

        public void CreatePrice(LensNumberRangePrice price)
        {

            price.LensCategoryIds.Remove(0);

            foreach (int lensCategoryId in price.LensCategoryIds)
            {
                price.LensCategoryID = lensCategoryId;
                this.Create(price);
            }
        }

    }
}
