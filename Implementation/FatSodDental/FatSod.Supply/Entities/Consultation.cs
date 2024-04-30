using FatSod.Security.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace FatSod.Supply.Entities
{
    [Serializable]
    public class Consultation
    {

       public int ConsultationID { get; set; }
        /// <summary>
        /// FK clients
        /// </summary>
        /// 
        [Index("IX_Consultation",1, IsUnique = true, IsClustered = false)]
        public int CustomerID { get; set; }
        [ForeignKey("CustomerID")]
        public virtual Customer Customer { get; set; }

        public bool IsNewCustomer { get; set; }
        public bool isPrescritionValidate { get; set; }
        

        public string MedecintTraitant { get; set; }
        public string RaisonRdv { get; set; }
        //gestionnaire de compte
        public int? GestionnaireID { get; set; }
        [ForeignKey("GestionnaireID")]
        public virtual User Gestionnaire { get; set; }

        //date consultation
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [Index("IX_Consultation",2, IsUnique = true, IsClustered = false)]
        public DateTime DateConsultation { get; set; }

        public virtual ICollection<Sale> Sales { get; set; }
    }
    
}
