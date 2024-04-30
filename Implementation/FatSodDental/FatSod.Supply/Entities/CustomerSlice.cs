using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FatSod.Supply.Entities
{
    [Serializable]
    public class CustomerSlice : Slice
    {
        public int SaleID { get; set; }
        [ForeignKey("SaleID")]
        public virtual Sale Sale { get; set; }
        public bool isDeposit { get; set; }
        [StringLength(100)]
        public string Reference { get; set; }
    }


    public class CustomerReturnSlice : Slice
    {
        public int CustomerReturnID { get; set; }
        [ForeignKey("CustomerReturnID")]
        public virtual CustomerReturn CustomerReturn { get; set; }
    }


}
