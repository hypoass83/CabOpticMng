using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FatSod.Ressources;
using System.Globalization;

namespace FatSod.Supply.Entities
{
    [Serializable]
   public class LensNumberRange /*: Range<string>*/
    {

        public LensNumberRange() { }
        public LensNumberRange(string min, string max)
        {
            this.Minimum = min;
            this.Maximum = max;
        }

        public int LensNumberRangeID { get; set; }
        /// <summary>
        /// Valeur Minimale de l'intervalle
        /// </summary>
        [StringLength(10)]
        [Index("IX_RealPrimaryKey", 1, IsUnique = true)]
        public string Minimum { get; set; }

        /// <summary>
        /// Valeur Maximale de l'intervale
        /// </summary>
        [StringLength(10)]
        [Index("IX_RealPrimaryKey", 2, IsUnique = true)]
        public string Maximum { get; set; }
        [NotMapped]
        private CultureInfo enUsCI = CultureInfo.GetCultureInfo("en-US");

/*
        /// <summary>
        /// Attribut pour dire si cette intervalle est celle d'un verre addition.
        /// ce qui veut dire que la valeur de l'addition de ce verre est dans cet intervalle.
        /// Ceci parce que le contenu des intervalles des numéros avec addition lors de la fixation des prix n'a pas la même longueur(8) que 
        /// les sphériques et les cylindrique
        /// </summary>
        [Index("IX_RealPrimaryKey", 3, IsUnique = true)]
        public bool IsAdditionRange { get; set; }
*/
        /// <summary>
        /// Presents the Range in readable format
        /// </summary>
        /// <returns>String representation of the Range</returns>
        public string ToString() { return String.Format("{0} " + Resources.To + " {1}", Minimum, Maximum); }

        /// <summary>
        /// Determine si l'intervalle est valide.
        /// L'intervalle est valide si : 
        /// 1 - la valeur maximale est supérieur ou égale à la valeur minimale
        /// 2 - les valeur minimale et maximum sont des multiple de 0.25
        /// </summary>
        /// <returns>True if range is valid, else false</returns>
        public Boolean IsValid()
        {
            if (Minimum==null || Maximum==null)
            {
                return false;
            }
            
            decimal min = (Convert.ToDecimal(Minimum, enUsCI));
            decimal max = (Convert.ToDecimal(Maximum, enUsCI));
            bool superiority = max >= min;
            bool multiplicity = (min % 0.25m == 0) && (max % 0.25m == 0);

            return (superiority && multiplicity); 
        }

        /// <summary>
        /// Determine si une valeur est dans l'intervale
        /// </summary>
        /// <param name="value">The value to test</param>
        /// <returns>True if the value is inside Range, else false</returns>
        public Boolean ContainsValue(decimal value)
        {
            decimal min = (Convert.ToDecimal(Minimum, enUsCI)) ;
            decimal max = (Convert.ToDecimal(Maximum, enUsCI));
            decimal testValue = (Convert.ToDecimal(value, enUsCI));

            bool multiplicity = (testValue % 0.25m == 0);
            bool validity = IsValid();
            bool contain = (min <= testValue && testValue <= max);

            return (multiplicity && validity && contain);
        }

        /// <summary>
        /// Determine si une valeur est dans l'intervale
        /// </summary>
        /// <param name="value">The value to test</param>
        /// <returns>True if the value is inside Range, else false</returns>
        public Boolean ContainsValue(string value)
        {
            decimal testValue = 0;
            decimal min = (Convert.ToDecimal(Minimum, enUsCI));
            decimal max = (Convert.ToDecimal(Maximum, enUsCI));
            if (value != "")
            {
                testValue = (Convert.ToDecimal(value, enUsCI));
            }
            
            bool multiplicity = (testValue % 0.25m == 0);
            bool validity = IsValid();
            bool contain = (min <= testValue && testValue <= max);

            return (multiplicity && validity && contain);
        }

        /// <summary>
        /// Determine si l'intervalle courante est incluse dans celle passée en paramètre
        /// </summary>
        /// <param name="Range">L'intervalle parent ou la plus grande</param>
        /// <returns>True if range is inclusive, else false</returns>
        public Boolean IsInsideRange(LensNumberRange range)
        {

            return this.IsValid() && range.IsValid() && range.ContainsValue(this.Minimum) && range.ContainsValue(this.Maximum);
        }

        /// <summary>
        /// Determine si l'intervalle courante(parent) contient l'intervalle passée en pparamètre(fils)
        /// </summary>
        /// <param name="Range">l'intervalle fils à tester</param>
        /// <returns>True if range is inside, else false</returns>
        public Boolean ContainsRange(LensNumberRange Range)
        {
            return this.IsValid() && Range.IsValid() && this.ContainsValue(Range.Minimum) && this.ContainsValue(Range.Maximum);
        }
    }
}
