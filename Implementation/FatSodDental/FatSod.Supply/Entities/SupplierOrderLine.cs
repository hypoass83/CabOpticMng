using FatSod.Security.Entities;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FatSod.Supply.Entities
{
    [Serializable]
    public class SupplierOrderLine : Line
    {        
        public int SupplierOrderID { get; set; }
        [ForeignKey("SupplierOrderID")]
        public virtual SupplierOrder SupplierOrder { get; set; }
    }
}
