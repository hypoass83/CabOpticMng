using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FatSod.Supply.Entities
{
    [Serializable]
    public class SaleAccountOperation : AccountOperation
    {
        //cle etranngere vers sale
        public int SaleID { get; set; }
        [ForeignKey("SaleID")]
        public virtual Sale Sale { get; set; }
    }
}
