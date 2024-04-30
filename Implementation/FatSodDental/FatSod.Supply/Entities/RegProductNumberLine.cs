using FatSod.Security.Entities;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FatSod.Supply.Entities
{
    [Serializable]
    public class RegProductNumberLine
    {

        public int RegProductNumberLineID { get; set; }

        public int OldProductID { get; set; }
        [ForeignKey("OldProductID")]
        public virtual Product OldProduct { get; set; }

        public int NewProductID { get; set; }
        [ForeignKey("NewProductID")]
        public virtual Product NewProduct { get; set; }

        public double OldLineQuantity { get; set; }
        public double NewLineQuantity { get; set; }

        public int LocalizationID { get; set; }
        [ForeignKey("LocalizationID")]
        public virtual Localization Localization { get; set; }

        public int RegProductNumberID { get; set; }
        [ForeignKey("RegProductNumberID")]
        public virtual RegProductNumber RegProductNumber { get; set; }
       
        //No Mapped fields
        [NotMapped]
        public int TMPID { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null) { return false; }
            if (!(obj is RegProductNumberLine)) { return false; }
            RegProductNumberLine ptl = (RegProductNumberLine)obj;
            return this.OldProductID == ptl.OldProductID &&
                   this.LocalizationID == ptl.LocalizationID 
                ;
        }

    }
}
