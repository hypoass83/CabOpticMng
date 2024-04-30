using FatSod.Security.Entities;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FatSod.Supply.Entities
{
    [Serializable]
    public class CustomerReturnLine
    {
        public int CustomerReturnLineID { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime CustomerReturnDate { get; set; }
        //C'est le montant du transport saisi par l'utilisateur
        public double? Transport { get; set; }        
        public string CustomerReturnCauses { get; set; }
        public int SaleLineID { get; set; }
        [ForeignKey("SaleLineID")]
        public virtual SaleLine SaleLine { get; set; }
        public int CustomerReturnID { get; set; }
        [ForeignKey("CustomerReturnID")]
        public virtual CustomerReturn CustomerReturn { get; set; }
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
