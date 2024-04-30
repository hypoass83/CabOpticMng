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
    public class ProductTransferAccountOperation : AccountOperation
    {
        //cle etranngere vers ProductTransfert
        public int ProductTransfertID { get; set; }
        [ForeignKey("ProductTransfertID")]
        public virtual ProductTransfert ProductTransfert { get; set; }
    }
}
