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
    public class PurchaseReturnAccountOperation : AccountOperation
    {

        public int SupplierReturnID { get; set; }
        [ForeignKey("SupplierReturnID")]
        public virtual SupplierReturn SupplierReturn { get; set; }
    }
}
