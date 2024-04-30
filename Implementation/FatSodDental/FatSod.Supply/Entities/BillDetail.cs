using FatSod.Security.Entities;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FatSod.Supply.Entities
{
    [Serializable]
    public class BillDetail
    {
        public int BillDetailID { get; set; }
        public int BillID { get; set; }
        [ForeignKey("BillID")]
        public virtual Bill Bill { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime DateVente { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime DateCommande { get; set; }
        public string NumeroCommande { get; set; }
        public double LineUnitPrice { get; set; }
        public double LineQuantity { get; set; }
        public int ProductID { get; set; }
        [ForeignKey("ProductID")]
        public virtual Product Product { get; set; }
        public int SaleID { get; set; }
        [ForeignKey("SaleID")]
        public virtual Sale Sale { get; set; }
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
