using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FatSod.Supply.Entities
{
    [Table("viewcustomerSlice")]
    public class viewcustomerSlice
    {
        [Key]
        public Guid Id { get; set; }
        public int SaleID { get; set; }
        public int CustomerID { get; set; }
        public double SliceAmount { get; set; }
    }
}
