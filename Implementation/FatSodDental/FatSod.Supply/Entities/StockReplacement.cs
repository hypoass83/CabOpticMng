using FatSod.Security.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FatSod.Supply.Entities
{
    [Serializable]
     public class StockReplacement
     {
         public int StockReplacementID { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime StockReplacementDate { get; set; }
         [Required]
         [StringLength(50)]
         [Index("StockReplacementReference", IsUnique = true, IsClustered = false)]
         public string StockReplacementReference { get; set; }
         public int BranchID { get; set; }
         [ForeignKey("BranchID")]
         public virtual Branch Branch { get; set; }
           
         public int? AutorizedByID { get; set; }
         [ForeignKey("AutorizedByID")]
         public virtual FatSod.Security.Entities.User AutorizedBy { get; set; }
         public int RegisteredByID { get; set; }
         [ForeignKey("RegisteredByID")]
         //c'est le nom de l'employé de l'agence de départ qui a saisir le transfert
         public virtual FatSod.Security.Entities.User RegisteredBy { get; set; }
         
         public virtual ICollection<StockReplacementLine> StockReplacementLines { get; set; }
         
     }
}
