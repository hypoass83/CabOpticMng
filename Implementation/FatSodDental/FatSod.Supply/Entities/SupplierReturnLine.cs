using FatSod.Security.Entities;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FatSod.Supply.Entities
{
    [Serializable]
    public class SupplierReturnLine
    {
        public int SupplierReturnLineID { get; set; }
        public int SupplierReturnID { get; set; }
        [ForeignKey("SupplierReturnID")]
        public virtual SupplierReturn SupplierReturn { get; set; }
        //public DateTime SupplierReturnDate { get; set; }
        //C'est le montant du transport saisi par l'utilisateur
        public double? Transport { get; set; }
        public string SupplierReturnCauses { get; set; }
        public int PurchaseLineID { get; set; }
        [ForeignKey("PurchaseLineID")]
        public virtual PurchaseLine PurchaseLine { get; set; }
        //Il s'agit de la quantité retournée
        public double LineQuantity { get; set; }


        [NotMapped]
        public double LineUnitPrice { get; set; }
        [NotMapped]
        public int LocalizationID { get; set; }
        [NotMapped]
        public int ProductID { get; set; }
        [NotMapped]
        public int TMPID { get; set; }
        [NotMapped]
        public double LineAmount
        {
            get
            {
                return LineQuantity * LineUnitPrice;
            }
        }

    }
}
