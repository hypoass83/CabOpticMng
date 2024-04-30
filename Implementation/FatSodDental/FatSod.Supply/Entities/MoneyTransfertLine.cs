using FatSod.Security.Entities;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FatSod.Supply.Entities
{
    [Serializable]
    public class MoneyTransfertLine
    {
        public int MoneyTransfertLineID { get; set; }
        public int LineAmount { get; set; } 

        public int DepartureTillID { get; set; }
        [ForeignKey("DepartureTillID")]
        public virtual Till DepartureTill { get; set; }        

        public int ArrivalTillID { get; set; }
        [ForeignKey("ArrivalTillID")]
        public virtual Localization ArrivalTill { get; set; }

        public int MoneyTransfertID { get; set; }
        [ForeignKey("ProductTransfertID")]
        public virtual MoneyTransfert MoneyTransfert { get; set; }

        //No Mapped fields
        [NotMapped]
        public int TMPID { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null) { return false; }
            if ( !( obj is MoneyTransfertLine )) { return false; }
            MoneyTransfertLine ptl = (MoneyTransfertLine)obj;
            return this.DepartureTillID == ptl.DepartureTillID &&
                   this.ArrivalTillID == ptl.ArrivalTillID;
        }

    }
}
