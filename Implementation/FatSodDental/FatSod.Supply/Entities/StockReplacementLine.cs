using FatSod.Security.Entities;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FatSod.Supply.Entities
{
    [Serializable]
    public class StockReplacementLine
    {

        public int StockReplacementLineID { get; set; }

        public int ProductID { get; set; }
        [ForeignKey("ProductID")]
        public virtual Product Product { get; set; }

        public double LineQuantity { get; set; }
        public double LineUnitPrice { get; set; }

        public int LocalizationID { get; set; }
        [ForeignKey("LocalizationID")]
        public virtual Localization Localization { get; set; }

        public int StockReplacementID { get; set; }
        [ForeignKey("StockReplacementID")]
        public virtual StockReplacement StockReplacement { get; set; }
        public EyeSide OeilDroiteGauche { get; set; }
        public string StockReplacementReason { get; set; }

        public string NumeroSerie { get; set; }
        public string Marque { get; set; }

        //info damage
        public int ProductDamageID { get; set; }
        [ForeignKey("ProductDamageID")]
        public virtual Product ProductDamage { get; set; }

        public string NumeroSerieDamage { get; set; }
        public string MarqueDamage { get; set; }

        //No Mapped fields
        [NotMapped]
        public int TMPID { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null) { return false; }
            if (!(obj is StockReplacementLine)) { return false; }
            StockReplacementLine ptl = (StockReplacementLine)obj;
            return this.ProductID == ptl.ProductID &&
                   this.LocalizationID == ptl.LocalizationID 
                ;
        }

    }
}
