using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FatSod.Supply.Entities
{
    public class CustomerSatisfaction
    {
        public int CustomerSatisfactionId { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime OperationDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime SaleDate { get; set; }

        public bool IsSatisfied { get; set; }

        public string Comment { get; set; }
        public string ContactChannel { get; set; } // SMS | Call | Email | ...

        public int CumulSaleAndBillID { get; set; }
        [ForeignKey("CumulSaleAndBillID")]
        public virtual CumulSaleAndBill CumulSaleAndBill { get; set; }
        
        [NotMapped]
        public string Customer { get; set; }
        [NotMapped]
        public string DisplayDate { get; set; }
        [NotMapped]
        public string DisplaySaleDate { get; set; }
        [NotMapped]
        public string DeliveryDate { get; set; }
        [NotMapped]
        public string IsDelivered { get; set; }
        [NotMapped]
        public string PhoneNumber { get; set; }
        [NotMapped]
        public string IsSatisfiedDisplay { get; set; }
        [NotMapped]
        public string CustomerType { get; set; }
        [NotMapped]
        public string Insurance { get; set; }
        [NotMapped]
        public string InsuredCompany { get; set; }
        [NotMapped]
        public string CustomerValue { get; set; }
    }
}
