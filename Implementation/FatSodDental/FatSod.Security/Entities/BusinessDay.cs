
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace FatSod.Security.Entities
{
    [Serializable]
    public class BusinessDay
    {
        public int BusinessDayID { get; set; }
        [Required]
        [StringLength(100)]
        [Index("BDCode", IsUnique = true, IsClustered = false)]
        public string BDCode { get; set; }
        public string BDLabel { get; set; }
        public string BDDescription { get; set; }
        /// <summary>
        /// c'est cette date qui sera utilisée dans toutes les opérations éffectuées dans l'application
        /// lors de la création d'un nouveau BusinessDay, la nouvelle date doit être supérieure à celle-ci
        /// </summary>
        public DateTime BDDateOperation { get; set; }
        /// <summary>
        /// Cette variable permet de dire si une journée de travail est ouverte(true) ou fermée(false).
        /// Avant d'éffectuer quelque action que ce soit dans l'aplication, cette valeur doit être à true.
        /// Si cette valeur à true lors de la connexion d'un utilisateur, on doit vérifier si celui-ci a le droit d'ouvrir une journée de travail
        /// s'il a le droit d'ouvrir une journée de travail, on lui affichera l'interface d'ouverture des journées de travail
        /// s'il n'a pas le droit d'ouvrir une journée de travail, on lui affichera l'interface lui disant d'informer une personne pouvant ouvrir une journée de travail
        /// </summary>
        public bool BDStatut { get; set; }

        /// <summary>
        /// cette attribut permet de savoir si la cloture de la journée a été initiée afin d'éviter que les 
        /// utilisateurs ne continue à travailler sur une journée de travail qui est sensée avoir été fermée
        /// elle est importante dans le cas où la cloture ne se passera pas bien (coupure de lumière et autre)
        /// elle doit être mise à true dès l'éxécution de la première tâche de cloture et à false à la fin de la dernière
        /// </summary>
        public bool ClosingDayStarted { get; set; }
        
        public int BranchID { get; set; }
        [ForeignKey("BranchID")]
        public virtual Branch Branch { get; set; }
        /// <summary>
        /// backdate parameters
        /// </summary>
        /// 
        public DateTime BackDateOperation { get; set; }
        public bool BackDStatut { get; set; }

        [NotMapped]
        public string BranchName
        {
            get
            {
                return this.Branch.BranchName;
            }

        }

    }
}
