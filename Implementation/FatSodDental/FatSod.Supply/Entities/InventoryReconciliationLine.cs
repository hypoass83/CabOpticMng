using FatSod.Security.Entities;
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

    public class InventoryReconciliationLine
    {
        public int InventoryReconciliationLineId { get; set; }
        public double ReconciliationQuantity { get; set; }
        public string ReconciliationComment { get; set; }
        public int StockId { get; set; }
        [ForeignKey("StockId")]
        public virtual ProductLocalization Stock { get; set; }
        public int InventoryReconciliationId { get; set; }
        [ForeignKey("InventoryReconciliationId")]
        public virtual InventoryReconciliation InventoryReconciliation { get; set; }

        [NotMapped]
        public int StockQuantity { get; set; }

    }
}
