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
    public class PurchaseAccountOperation : AccountOperation
    {
        //cle etranngere vers Purchase
        public int PurchaseID { get; set; }
        [ForeignKey("PurchaseID")]
        public virtual Purchase Purchase { get; set; }
    }
}
