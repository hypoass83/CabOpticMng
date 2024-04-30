using FatSod.Security.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FatSod.Supply.Entities
{
    [Serializable]
    public class InventoryReconciliation
    {
        public int InventoryReconciliationId { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime ReconciliationDate { get; set; }
        public int AuthorizedById { get; set; }
        [ForeignKey("AuthorizedById")]
        public virtual User AuthorizedBy { get; set; }
        public int RegisteredById { get; set; }
        [ForeignKey("RegisteredById")]
        public virtual User RegisteredBy { get; set; }
        public int InventoryCountingId { get; set; }
        [ForeignKey("InventoryCountingId")]
        public virtual InventoryCounting InventoryCounting { get; set; }
        public virtual ICollection<InventoryReconciliationLine> InventoryReconciliationLines { get; set; }
    }
}
