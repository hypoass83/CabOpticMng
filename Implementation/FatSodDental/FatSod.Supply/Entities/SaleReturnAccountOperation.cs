using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FatSod.Supply.Entities
{
    [Serializable]
    public class SaleReturnAccountOperation : AccountOperation
    {

        public int CustomerReturnID { get; set; }
        [ForeignKey("CustomerReturnID")]
        public virtual CustomerReturn CustomerReturn { get; set; }
    }
}
