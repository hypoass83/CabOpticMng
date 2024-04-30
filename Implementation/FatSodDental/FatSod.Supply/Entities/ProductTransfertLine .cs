using FatSod.Security.Entities;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace FatSod.Supply.Entities
{
    [Serializable]
    public class ProductTransfertLine
    {
        public int ProductTransfertLineId { get; set; }

        public int? DepartureStockId { get; set; }
        [ForeignKey("DepartureStockId")]
        public virtual ProductLocalization DepartureStock { get; set; }

        public int? ArrivalStockId { get; set; }
        [ForeignKey("ArrivalStockId")]
        public virtual ProductLocalization ArrivalStock { get; set; }

        public double Quantity { get; set; }
        public double UnitPrice { get; set; }

        public int ProductTransfertId { get; set; }
        [ForeignKey("ProductTransfertId")]
        public virtual ProductTransfert ProductTransfert { get; set; }
        public EyeSide OeilDroiteGauche { get; set; }

        //No Mapped fields
        [NotMapped]
        public int TMPID { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null) { return false; }
            if (!(obj is ProductTransfertLine)) { return false; }
            ProductTransfertLine ptl = (ProductTransfertLine)obj;
            return this.DepartureStockId == ptl.DepartureStockId &&
                   this.ArrivalStockId == ptl.ArrivalStockId;
        }

    }
}
