using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FatSod.Supply.Entities
{
    [Serializable]
    public class TillSale : Sale
    {
        //public int TillSaleID { get; set; }
        public int TillID { get; set; }
        [ForeignKey("TillID")]
        public virtual Till Till { get; set; }
    }
}
