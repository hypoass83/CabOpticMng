using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FatSod.Security.Entities
{
    [Serializable]
    public abstract class GlobalPerson
    {
        public int GlobalPersonID { get; set; }

        /// <summary>
        /// ce Champ représente : 
        /// - Le Nom d'une personne physique 
        /// - Le Nom de l'entreprise ou d'une personne morale
        /// </summary>
        [Required]
        public string Name { get; set; }
        /// <summary>
        /// This property represent : 
        /// - The surname of a physical person 
        /// - Description of a moral person
        /// </summary>
        /// 
        public string Tiergroup { get; set; }
        public string Description { get; set; }
        /// <summary>
        /// cette propriété represente : 
        /// - La CNI d'une personne physique 
        /// - le Numéro de contribuable de l'entreprise ou d'une personne morale
        /// </summary>
        [Required]
        [StringLength(100)]
        [Index("CNI",IsUnique = true, IsClustered = false)]
        public string CNI { get; set; }

        public int AdressID { get; set; } 
        [ForeignKey("AdressID")]
        public virtual Adress Adress { get; set; }
        public virtual ICollection<File> Files { get; set; }

        //si le client est une personne physique, on renvoie son nom et son prénom, si c'est une entreprise, on renvoie son nom
        [NotMapped]
        public string FullName { 
            get {
                return this.Name + ((this.Description == null) ? "" : " " + this.Description);
            }
        }

    }
}
