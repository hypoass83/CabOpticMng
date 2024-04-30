using FatSod.Security.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FatSod.Supply.Entities
{
    [Serializable]
    public class InventoryCounting
    {
        public int InventoryCountingId { get; set; }
        public string Reference { get; set; }
        public string Description { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime CreatedDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? ClosedDate { get; set; }
        public int BranchId { get; set; }
        [ForeignKey("BranchId")]
        public virtual Branch Branch { get; set; }
        public int AuthorizedById { get; set; }
        [ForeignKey("AuthorizedById")]
        public virtual User AuthorizedBy { get; set; }
        public int? CreatedById { get; set; }
        [ForeignKey("CreatedById")]
        public virtual User CreatedBy { get; set; }
        public virtual ICollection<InventoryCountingLine> InventoryCountingLines { get; set; }
    }
}
