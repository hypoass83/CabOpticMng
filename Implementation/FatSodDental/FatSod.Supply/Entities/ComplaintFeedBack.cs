using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FatSod.Security.Entities;

namespace FatSod.Supply.Entities
{
    public class ComplaintFeedBack
    {
        public int ComplaintFeedBackId { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime OperationDate { get; set; }

        public bool IsSatisfied { get; set; }

        public string Comment { get; set; }
        public string ContactChannel { get; set; } // SMS | Call | Email | ...

        public int CustomerComplaintId { get; set; }
        [ForeignKey("CustomerComplaintId")]
        public virtual CustomerComplaint CustomerComplaint { get; set; }

        public int OperatorID { get; set; }
        [ForeignKey("OperatorID")]
        public virtual User Operator { get; set; }
        [NotMapped]
        public string Complaint { get; set; }
        [NotMapped]
        public string ResolverComment { get; set; }
        [NotMapped]
        public string Customer { get; set; }
        [NotMapped]
        public string DisplayDate { get; set; }
        [NotMapped]
        public string DisplaySaleDate { get; set; }
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
