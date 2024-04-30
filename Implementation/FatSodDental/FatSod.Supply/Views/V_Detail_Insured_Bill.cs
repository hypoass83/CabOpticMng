using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FatSod.Supply.Entities
{
    [Table("V_Detail_Insured_Bill")]
    public class V_Detail_Insured_Bill
    {
        [Key]
        public Guid Id { get; set; }
        public int CustomerOrderID { get; set; }
        public DateTime CustomerOrderDate { get; set; }
        public string CustomerName { get; set; }
        public string CompanyName { get; set; }
        public string NumeroFacture { get; set; }
        public string PhoneNumber { get; set; }
        public int ProductID { get; set; }
        public string marque { get; set; }
        public string reference { get; set; }
        public string Prescription { get; set; }
        public string CategoryCode { get; set; }
        public int LineID { get; set; }
        public int CategoryID { get; set; }
        public double Plafond { get; set; }
        public int? SellerID { get; set; }
        public string SellerName { get; set; }
        public int? MarketerID { get; set; }
        public string MarketerName { get; set; }
        public DateTime? ValidateBillDate { get; set; }
        public int IsNewCustomer { get; set; }
        public string InsuredCompany { get; set; }
    }
}
