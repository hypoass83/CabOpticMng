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
    public class PurchaseLine : Line
    {
        public int PurchaseID { get; set; }
        [ForeignKey("PurchaseID")]
        public virtual Purchase Purchase { get; set; }
        
        //propriétés non persistantes

        [NotMapped]
        public double SafetyStockQuantity { get; set; }

    }
}
