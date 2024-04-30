using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FatSod.Supply.Entities
{
    [Table("V_InsureStatus")]
    public class V_InsureStatus
    {
        [Key]
        public Guid Id { get; set; }
        public string CustomerName { get; set; }
        public string CompanyName { get; set; }
        public string InsurreName { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime CustomerOrderDate { get; set; }
        public DateTime? ValidateBillDate { get; set; }
        public string NumeroFacture { get; set; }
        public string reference { get; set; }
        public string marque { get; set; }
        public double Plafond { get; set; }
        public string TypeLens { get; set; }
        public string CustomerStatus { get; set; }
        public int? MarketerID { get; set; }
        public string MarketerName { get; set; }
        public int? SellerID { get; set; }
        public string SellerName { get; set; }
    }
}
