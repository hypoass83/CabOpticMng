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
    public class Assureur : Person
    {
        /// <summary>
        /// cpte collectif des clients
        /// </summary>
        public int AccountID { get; set; }
        [ForeignKey("AccountID")]
        public virtual Account Account { get; set; }
        
        /*---Début de la création des champs permettant de créer les fournisseurs morales---*/
        public int CompanyCapital { get; set; }
        //[StringLength(100)]
        //[Index("CompanySigle", IsUnique = true, IsClustered = false)]
        public string CompanySigle { get; set; }
        //[StringLength(100)]
        //[Index("CompanyTradeRegister", IsUnique = true)]
        public string CompanyTradeRegister { get; set; }//RCCM
        public string CompanySlogan { get; set; }
        public bool CompanyIsDeletable { get; set; }

        public int CompteurFacture { get; set; }

      
        public int Matricule { get; set; }

        public double Remise { get; set; }

        [NotMapped]
        public string AssureurFullName
        {
            get
            {
                return this.Name;// +
                        //((this.CompanySigle != null && this.CompanySigle.Length > 0) ? "" : " " + this.Description);
            }
        }

        [NotMapped]
        public double Debt { get; set; }

        [NotMapped]
        public string AdressEmail { get; set; }
        [NotMapped]
        public string AdressPOBox { get; set; }
        [NotMapped]
        public string AdressPhoneNumber { get; set; }
        [NotMapped]
        public string SexLabel { get; set; }

    }
   
}
