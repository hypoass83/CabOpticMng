using FatSod.Security.Entities;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FatSod.Supply.Entities
{
    [Serializable]
    public class ProductGiftLine
    {

        public int ProductGiftLineID { get; set; }

        public int ProductID { get; set; }
        [ForeignKey("ProductID")]
        public virtual Product Product { get; set; }

        public double LineQuantity { get; set; }
        public double LineUnitPrice { get; set; }

        public int LocalizationID { get; set; }
        [ForeignKey("LocalizationID")]
        public virtual Localization Localization { get; set; }

        public int ProductGiftID { get; set; }
        [ForeignKey("ProductGiftID")]
        public virtual ProductGift ProductGift { get; set; }
        public EyeSide OeilDroiteGauche { get; set; }
        public string ProductGiftReason { get; set; }

        public string NumeroSerie { get; set; }
        public string Marque { get; set; }
        
        //No Mapped fields
        [NotMapped]
        public int TMPID { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null) { return false; }
            if (!(obj is ProductGiftLine)) { return false; }
            ProductGiftLine ptl = (ProductGiftLine)obj;
            return this.ProductID == ptl.ProductID &&
                   this.LocalizationID == ptl.LocalizationID 
                ;
        }

    }
}
