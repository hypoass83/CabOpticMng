using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FatSod.Supply.Entities
{
    [Table("V_Summary_Sales")]
    public class V_Summary_Sales
    {
        [Key]
        public Guid Id { get; set; }
        public int SaleID { get; set; }
        public DateTime SaleDate { get; set; }
        public int CustomerID { get; set; }
        public string SaleReceiptNumber { get; set; }
        public double totPrice { get; set; }
        public int? MarketerID { get; set; }
        public string MarketerName { get; set; }
        public string CustomerName { get; set; }
        public string Phone { get; set; }
        public int? SellerID { get; set; }
        public string SellerName { get; set; }
        public int IsNewCustomer { get; set; }
        public bool IsInHouseCustomer { get; set; }
    }
}
