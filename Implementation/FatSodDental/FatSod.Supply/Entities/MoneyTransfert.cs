using FatSod.Security.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FatSod.Supply.Entities
{
    [Serializable]
    public class MoneyTransfert
    {
        /// <summary>
        ///cette propriété dis si oui ou non le transfert a été validé ou reçu par l'agence qui doi recevoir.
        ///c'est elle qui permet de modifier la quantité d'argent dans la caisse et de comptabiliser le transfert        
        /// </summary>

        /*-------------Propriétés d'un transfert de caisse à caisse-------------*/
        public int MoneyTransfertID { get; set; }
        public bool IsReceived { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime SendDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime ReceivedDate { get; set; }
        [Required]
        [StringLength(50)]
        [Index("MoneyTransfertReference", IsUnique = true, IsClustered = false)]
        public string MoneyTransfertReference { get; set; }
        /// <summary>
        /// Employé qui a demandé le transfert
        /// </summary>
        public int? AskedByID { get; set; }
        [ForeignKey("AskedByID")]
        //C'est le nom de l'employé de l'agence d'arrivé qui a demandé le transfert
        public virtual FatSod.Security.Entities.User AskedBy { get; set; }
        /// <summary>
        /// Pour envoyer de l'argent à la banque, il faut un ordre de quelqu'un mais pour en recevoir de la banque, pas nécessaire.
        /// </summary> 
        public int OrderedByID { get; set; }
        [ForeignKey("OrderedByID")]
        //C'est le nom de l'employé de l'agence de départ qui a ordonné le transfert
        public virtual FatSod.Security.Entities.User OrderedBy { get; set; }
        /// <summary>
        ///c'est le nom de l'employé de l'agence de départ qui a saisir le transfert
        /// </summary>
        public int RegisteredByID { get; set; }
        [ForeignKey("RegisteredByID")]
        public virtual FatSod.Security.Entities.User RegisteredBy { get; set; }
        /// <summary>
        ///c'est le nom de l'employé de l'agence de d'arrivée qui a reçu le transfert
        /// </summary>
        public int? ReceivedByID { get; set; }
        [ForeignKey("ReceivedByID")]
        public virtual FatSod.Security.Entities.User ReceivedBy { get; set; }
        /// <summary>
        ///Quand on reçois l'argent de la banque, la branche de départ est null
        /// </summary>
        public int DepartureBranchID { get; set; }
        [ForeignKey("DepartureBranchID")]
        public virtual Branch DepartureBranch { get; set; }
        /// <summary>
        ///Quand on envoie l'argent à la banque, la branche d'arrivée est null
        /// </summary>
        public int ArrivalBranchID { get; set; }
        [ForeignKey("ArrivalBranchID")]
        public virtual Branch ArrivalBranch { get; set; }
        public virtual ICollection<MoneyTransfertLine> MoneyTransfertLines { get; set; }
    }
}
