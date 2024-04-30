using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FatSod.Supply.Entities
{
    [Table("CutomertotalpricePerDay")]
    public class CutomertotalpricePerDay
    {
        [Key]
        public Guid Id { get; set; }
        public int CustomerID { get; set; }
        public DateTime SaleDate { get; set; }
        public double totCustDayPrice { get; set; }
    }
}
