using FastSod.Utilities.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FatSod.Supply.Entities
{
    [Serializable]
    public class CustomerReturn
    {
        public int CustomerReturnID { get; set; }
        [Index("SaleID_IX", IsUnique = true, IsClustered = false)]
        public int SaleID { get; set; }
        [ForeignKey("SaleID")]
        public virtual Sale Sale { get; set; }
        public virtual ICollection<CustomerReturnLine> CustomerReturnLines { get; set; }
        public virtual ICollection<SaleReturnAccountOperation> SaleReturnAccountOperations { get; set; }
        public virtual ICollection<CustomerReturnSlice> CustomerReturnSlices { get; set; }

        [NotMapped]
        public DateTime CustomerReturnDate { get; set; }

        [NotMapped]
        public double RateReduction { get { return (Sale != null && Sale.SaleID > 0) ? Sale.RateReduction : 0; } }
        [NotMapped]
        public double RateDiscount { get { return (Sale != null && Sale.SaleID > 0) ? Sale.RateDiscount : 0; } }
        [NotMapped]
        //C'est le montant du transport saisi par l'utilisateur
        public double Transport { get; set; }
        [NotMapped]
        public double VatRate { get { return (Sale != null && Sale.SaleID > 0) ? Sale.VatRate : 0;} }
        [NotMapped]
        public double TotalPriceMarchandise { get; set; }
        [NotMapped]
        public double TotalPriceReturn { get; set; }
        [NotMapped]
        public double DiscountAmount { get; set; }
        [NotMapped]
        public double TVAAmount { get; set; }
    }
}
