using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FatSod.Supply.Entities
{
    [Serializable]
    public class LensNumberRangePrice /*: Range<string>*/
    {
        public int LensNumberRangePriceID { get; set; }

        //[Index("IX_RealPrimaryKey", 1, IsUnique = true)]
        public int LensCategoryID { get; set; }
        [ForeignKey("LensCategoryID")]
        public virtual LensCategory LensCategory { get; set; }

        //[Index("IX_RealPrimaryKey", 2, IsUnique = true)]
        public int? SphericalValueRangeID { get; set; }
        [ForeignKey("SphericalValueRangeID")]
        public virtual LensNumberRange SphericalValueRange { get; set; }

        //[Index("IX_RealPrimaryKey", 3, IsUnique = true)]
        public int? CylindricalValueRangeID { get; set; }
        [ForeignKey("CylindricalValueRangeID")]
        public virtual LensNumberRange CylindricalValueRange { get; set; }

        //[Index("IX_RealPrimaryKey", 4, IsUnique = true)]
        public int? AdditionValueRangeID { get; set; }
        [ForeignKey("AdditionValueRangeID")]
        public virtual LensNumberRange AdditionValueRange { get; set; }
        public double Price { get; set; }

        /// <summary>
        /// Cette méthode dit si oui ou non un numéro appartient à l'intervale de prix courante
        /// </summary>
        /// <param name="rangePrice">Intervalle pouvant contenir le numéro</param>
        /// <param name="number">Numéro du verre</param>
        /// <param name="category">Catégorie du verre</param>
        /// <returns></returns>
        public bool Contains(LensNumber number, LensCategory category)
        {
            bool res = false;
            bool a = this.SphericalValueRange != null && this.SphericalValueRange.IsValid();
            bool b = this.CylindricalValueRange != null && this.CylindricalValueRange.IsValid();
            bool c = this.AdditionValueRange != null && this.AdditionValueRange.IsValid();

            bool d = a && b && c;

            if (d) //(1 1 1)
            {
                res = this.LensCategoryID == category.CategoryID &&
                                  this.SphericalValueRange.ContainsValue(number.LensNumberSphericalValue) &&
                                  this.CylindricalValueRange.ContainsValue(number.LensNumberCylindricalValue) &&
                                  this.AdditionValueRange.ContainsValue(number.LensNumberAdditionValue);
                return res;
            }

            if (!d) 
            {
                bool e = a && b;

                if (e)
                {
                    if (!c) //(1 1 0)
                    {
                        res = this.LensCategoryID == category.CategoryID &&
                                  this.SphericalValueRange.ContainsValue(number.LensNumberSphericalValue) &&
                                  this.CylindricalValueRange.ContainsValue(number.LensNumberCylindricalValue);
                        return res;
                    }
                }

                if (!e)
                {
                    if (a) //=> b = faux
                    {

                        if (c) //(1 0 1)
                        {
                            res = this.LensCategoryID == category.CategoryID &&
                                  this.SphericalValueRange.ContainsValue(number.LensNumberSphericalValue) &&
                                  this.AdditionValueRange.ContainsValue(number.LensNumberAdditionValue);
                            return res;

                        }

                        if (!c) //(1 0 0)
                        {
                            res = this.LensCategoryID == category.CategoryID &&
                                  this.SphericalValueRange.ContainsValue(number.LensNumberSphericalValue);
                            return res;

                        }
                    }

                    if (b) //=> a = faux
                    {
                        if (c) //(0 1 1)
                        {
                            res = this.LensCategoryID == category.CategoryID &&
                                  this.CylindricalValueRange.ContainsValue(number.LensNumberCylindricalValue) &&
                                  this.AdditionValueRange.ContainsValue(number.LensNumberAdditionValue);
                            return res;

                        }

                        if (!c) // (0 1 0)
                        {
                            res = this.LensCategoryID == category.CategoryID &&
                                  this.CylindricalValueRange.ContainsValue(number.LensNumberCylindricalValue);
                        }

                    }
                     
                }

            }

            return res;
        }

        [NotMapped]
        public List<int> LensCategoryIds { get; set; }

    }
}
