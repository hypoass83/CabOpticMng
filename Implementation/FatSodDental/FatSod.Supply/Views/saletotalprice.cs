using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FatSod.Supply.Entities
{
    [Table("saletotalprice")]
    public class saletotalprice
    {
        [Key]
        public Guid Id { get; set; }
        public int SaleID { get; set; }
        public int LineID { get; set; }
        public int CustomerID { get; set; }
        public double TotPrice { get; set; }
    }
}
