using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FatSod.Supply.Entities
{
    [Serializable]
    public class Supplier : FatSod.Security.Entities.Person
    {

        /// <summary>
        /// cpte collectif des fournisseurs
        /// </summary>
        public int AccountID { get; set; }
        [ForeignKey("AccountID")]
        public virtual Account Account { get; set; }
        
        /// <summary>
        /// supplier number
        /// </summary>
        /// 
        [Index("SupplierNumber", IsUnique = true, IsClustered = false)]
        [StringLength(250)]
        public string SupplierNumber { get; set; }

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
        /*---Début de la création des champs permettant de créer les clients morales---*/

        //no map fields
        [NotMapped]
        public string SexLabel1 { 
            get
            {
                return this.Sex.SexLabel;
            }
        }

        [NotMapped]
        public string AdressPhoneNumber1
        {
            get
            {
                return this.Adress.AdressPhoneNumber;
            }
        }

        [NotMapped]
        public string AdressEmail1
        {
            get
            {
                return this.Adress.AdressEmail;
            }
        }

        [NotMapped]
        public string AdressPOBox1
        {
            get
            {
                return this.Adress.AdressPOBox;
            }
        }

        [NotMapped]
        public string AccountLabel1
        {
            get
            {
                return this.Account.AccountNumber.ToString();
            }
        }

        [NotMapped]
        public string SupplierFullName
        {
            get
            {
                return this.Name +
                        ((this.CompanySigle != null && this.CompanySigle.Length > 0) ? "" : " " + this.Description);
            }
        }



    }
}
