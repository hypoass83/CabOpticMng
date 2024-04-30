using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FatSod.Supply.Entities
{
    [Serializable]
    public class SupplierReturn
    {
        public int SupplierReturnID { get; set; }
        public int PurchaseID { get; set; }
        [ForeignKey("PurchaseID")]
        public virtual Purchase Purchase { get; set; }
        public virtual ICollection<PurchaseReturnAccountOperation> PurchaseReturnAccountOperations { get; set; }
        public virtual ICollection<SupplierReturnLine> SupplierReturnLines { get; set; }
        [NotMapped]
        public DateTime SupplierReturnDate { get; set; }
        [NotMapped]
        public double RateReduction { get { return (Purchase != null && Purchase.PurchaseID > 0) ? Purchase.RateReduction : 0; } }
        [NotMapped]
        public double RateDiscount { get { return (Purchase != null && Purchase.PurchaseID > 0) ? Purchase.RateDiscount : 0; } }
        [NotMapped]
        //C'est le montant du transport saisi par l'utilisateur
        public double Transport { get; set; }
        [NotMapped]
        public double VatRate { get { return (Purchase != null && Purchase.PurchaseID > 0) ? Purchase.VatRate : 0; } }
    }
}
