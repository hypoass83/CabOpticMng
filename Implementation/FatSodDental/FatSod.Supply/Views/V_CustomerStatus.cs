using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FatSod.Supply.Entities
{
    [Table("V_CustomerStatus")]
    public class V_CustomerStatus
    {
        [Key]
        public Guid Id { get; set; }
        public int CustomerID { get; set; }
        public string CustomerName { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime SaleDate { get; set; }
        public string SaleReceiptNumber { get; set; }
        public string reference { get; set; }
        public string marque { get; set; }
        public double FramePrice { get; set; }
        public string TypeLens { get; set; }
        public double LensPrice { get; set; }
        public string CustomerStatus { get; set; }
        public int? SellerID { get; set; }
        public string SellerName { get; set; }
        public int? MarketerID { get; set; }
        public string MarketerName { get; set; }
    }
}
