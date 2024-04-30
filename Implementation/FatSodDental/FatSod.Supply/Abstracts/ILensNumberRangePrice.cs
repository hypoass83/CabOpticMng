using FatSod.Supply.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FatSod.Supply.Abstracts
{
    public interface ILensNumberRangePrice : IRepositorySupply<LensNumberRangePrice>
    {
        
        /// <summary>
        /// Permet d'obtenir le prix d'un verre 
        /// </summary>
        /// <param name="number">Numéro du verre</param>
        /// <param name="category">Catégorie du verre</param>
        /// <returns>Prix du verre</returns>
        LensNumberRangePrice GetPrice(LensNumber number, LensCategory category);

        LensNumberRangePrice GetPrice(Lens product);

        LensNumberRangePrice GetPrice(int lensID);
        void CreatePrice(LensNumberRangePrice price);

    }
}
