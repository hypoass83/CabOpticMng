using FatSod.Security.Entities;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FatSod.Supply.Entities
{
    [Serializable]
    public class ProductDamageLine
    {

        public int ProductDamageLineID { get; set; }

        public int ProductID { get; set; }
        [ForeignKey("ProductID")]
        public virtual Product Product { get; set; }

        public int? CumulSaleAndBillLineID { get; set; }
        [ForeignKey("CumulSaleAndBillLineID")]
        public virtual CumulSaleAndBillLine CumulSaleAndBillLine { get; set; }

        public double LineQuantity { get; set; }
        public double LineUnitPrice { get; set; }

        public int LocalizationID { get; set; }
        [ForeignKey("LocalizationID")]
        public virtual Localization Localization { get; set; }

        public int ProductDamageID { get; set; }
        [ForeignKey("ProductDamageID")]
        public virtual ProductDamage ProductDamage { get; set; }
        public EyeSide OeilDroiteGauche { get; set; }
        public string ProductDamageReason { get; set; }

        public string NumeroSerie { get; set; }
        public string Marque { get; set; }


        //No Mapped fields
        [NotMapped]
        public int TMPID { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null) { return false; }
            if (!(obj is ProductDamageLine)) { return false; }
            ProductDamageLine ptl = (ProductDamageLine)obj;
            return this.ProductID == ptl.ProductID &&
                   this.LocalizationID == ptl.LocalizationID 
                ;
        }

    }
}
