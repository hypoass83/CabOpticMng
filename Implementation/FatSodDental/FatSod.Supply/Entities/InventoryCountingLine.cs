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

    public class InventoryCountingLine
    {
        public int InventoryCountingLineId { get; set; }
        public double CountedQuantity { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime RegistrationDate { get; set; }
        public int StockId { get; set; }
        [ForeignKey("StockId")]
        public virtual ProductLocalization Stock { get; set; }
        public int AuthorizedById { get; set; }
        [ForeignKey("AuthorizedById")]
        public virtual User AuthorizedBy { get; set; }
        public int CountedById { get; set; }
        [ForeignKey("CountedById")]
        public virtual User CountedBy { get; set; }
        public int RegisteredById { get; set; }
        [ForeignKey("RegisteredById")]
        public virtual User RegisteredBy { get; set; }
        public int InventoryCountingId { get; set; }
        [ForeignKey("InventoryCountingId")]
        public virtual InventoryCounting InventoryCounting { get; set; }
    }
}
