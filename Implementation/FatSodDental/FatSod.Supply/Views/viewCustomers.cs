using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FatSod.Supply.Entities
{
    [Table("viewCustomers")]
    public class viewCustomers
    {
        [Key]
        public Guid Id { get; set; }
        public int GlobalPersonID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string CNI { get; set; }
        public string CustomerNumber { get; set; }
        public DateTime? DateRegister { get; set; }
        public string AdressPOBox { get; set; }
        public string AdressEmail { get; set; }
        public string AdressPhoneNumber { get; set; }
        public string QuarterLabel { get; set; }
        public int AdressID { get; set; }
        public string QuarterCode { get; set; }
        public int QuarterID { get; set; }
        public bool IsBillCustomer { get; set; }
        public int AccountID { get; set; }
        public int AccountNumber { get; set; }
        public string AccountLabel { get; set; }
        public int SexID { get; set; }
        public string SexCode { get; set; }
        public string SexLabel { get; set; }

        public string CustomerValue { get; set; }
        public string LastCustomerValue { get; set; }
        public bool IsInHouseCustomer { get; set; }
    }
}
