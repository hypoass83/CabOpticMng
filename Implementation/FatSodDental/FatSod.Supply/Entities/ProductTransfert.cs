using FatSod.Security.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FatSod.Supply.Entities
{
    [Serializable]
    public class ProductTransfert
    {
        /// <summary>
        ///cette propriété dis si oui ou non le transfert a été validé ou reçu par l'agence qui doi recevoir.
        //c'est elle qui permet de modifier le stock du magasin qui reçois et de comptabiliser le ttransfert         
        /// </summary>
        public bool IsReceived { get; set; }
        public int ProductTransfertId { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime ProductTransfertDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? ReceivedDate { get; set; }
        [Required]
        [StringLength(50)]
        [Index("ProductTransfertReference", IsUnique = true, IsClustered = false)]
        public string ProductTransfertReference { get; set; }
        public int DepartureBranchId { get; set; }
        [ForeignKey("DepartureBranchId")]
        public virtual Branch DepartureBranch { get; set; }
        public int ArrivalBranchId { get; set; }
        [ForeignKey("ArrivalBranchId")]
        public virtual Branch ArrivalBranch { get; set; }


        public int? AskedById { get; set; }
        [ForeignKey("AskedById")]
        //C'est le nom de l'employé de l'agence d'arrivé qui a demandé le transfert
        public virtual User AskedBy { get; set; }

        public int? OrderedByID { get; set; }
        [ForeignKey("OrderedByID")]
        //C'est le nom de l'employé de l'agence de départ qui a ordonné le transfert
        public virtual User AuthorizedBy { get; set; }
        public int RegisteredById { get; set; }
        [ForeignKey("RegisteredById")]
        //c'est le nom de l'employé de l'agence de départ qui a saisir le transfert
        public virtual User RegisteredBy { get; set; }
        public int? ReceivedById { get; set; }
        [ForeignKey("ReceivedById")]
        //c'est le nom de l'employé de l'agence de d'arrivée qui a reçu le transfert
        public virtual User ReceivedBy { get; set; }
        public virtual ICollection<ProductTransfertLine> ProductTransfertLines { get; set; }
        public virtual ICollection<ProductTransferAccountOperation> ProductTransferAccountOperations { get; set; }
    }
}
