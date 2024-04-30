using FatSod.Security.Entities;
using FatSod.Supply.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace FatSod.Supply.Entities
{
    [Serializable]
    public class Localization
    {
        public int LocalizationID { get; set; }
        [Index(IsUnique = true)]
		[StringLength(100)]
        public string LocalizationCode { get; set; }
        public string LocalizationLabel { get; set; }
        public string LocalizationDescription { get; set; }
        public int QuarterID { get; set; }
        [ForeignKey("QuarterID")]
        public virtual Quarter Quarter { get; set; }
        public int BranchID { get; set; }
        [ForeignKey("BranchID")]
        public virtual Branch Branch { get; set; }       
        public virtual ICollection<ProductLocalization> ProductLocalizations { get; set; }
        /// <summary>
        /// Les magaziniers de ce magazin
        /// </summary>
        [NotMapped]       
        public virtual string[] WareHouseMen { get; set; }//id des employés qui ont été choisis comme magasinier
        [NotMapped]
        public int PrincipalWareHouseManID { get; set; } //id du magasinier principal
        [NotMapped]
        public DateTime AssigningDate { get; set; } //date à laquelle le magasin à été assigné
        [NotMapped]
        public bool AssigningToWareHouseMen { get; set; } 


    }
}
