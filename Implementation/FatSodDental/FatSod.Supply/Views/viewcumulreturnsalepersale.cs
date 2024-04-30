using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FatSod.Supply.Entities
{
    [Table("viewcumulreturnsalepersale")]
    public class viewcumulreturnsalepersale
    {
        [Key]
        public Guid Id { get; set; }
        public double totretqty { get; set; }
        public int SaleID { get; set; }
        public int ProductID { get; set; }
        public int LocalizationID { get; set; }
        public int SaleLineID { get; set; }
        public double retTransport { get; set; }
    }
}
