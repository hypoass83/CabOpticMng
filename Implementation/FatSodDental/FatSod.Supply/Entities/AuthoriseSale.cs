using FatSod.Security.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FatSod.Supply.Entities
{
    [Serializable]
    public class AuthoriseSale
    {
        public int AuthoriseSaleID { get; set; }
        public int CompteurFacture { get; set; }
        public bool SaleDeliver { get; set; }
        public double VatRate { get; set; }
        public double RateReduction { get; set; }
        public double RateDiscount { get; set; }
        public double Transport { get; set; }
        /// <summary>
        /// Il est important d'indiquer si une vente est urgente
        /// </summary>
        public bool IsUrgent { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime SaleDeliveryDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime SaleDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime SaleDateHours { get; set; }
        public bool SaleValidate { get; set; }
        public string PoliceAssurance { get; set; }
        public int PaymentDelay { get; set; }
        public int Guaranteed { get; set; }
        public string Patient { get; set; }
        public int ? DeviseID { get; set; }
        [ForeignKey("DeviseID")]
        public virtual Devise Devise { get; set; }
        public bool IsPaid { get; set; }
        [Index(IsUnique = true)]
        [StringLength(100)]
        public string SaleReceiptNumber { get; set; }
        public int BranchID { get; set; }
        [ForeignKey("BranchID")]
        public virtual Branch Branch { get; set; }
        public int? CustomerID { get; set; }
        [ForeignKey("CustomerID")]
        public virtual Customer Customer { get; set; }
        public string CustomerName { get; set; }
        public bool isReturn { get; set; }
        public SalePurchaseStatut StatutSale { get; set; }
        public virtual ICollection<AuthoriseSaleLine> AuthoriseSaleLines { get; set; }
       
        public int? OperatorID { get; set; }
        [ForeignKey("OperatorID")]
        public virtual User Operator { get; set; }

        public int? PostByID { get; set; }
        [ForeignKey("PostByID")]
        public virtual User PostBy { get; set; }

        public int? ConsultDilPrescID { get; set; }
        [ForeignKey("ConsultDilPrescID")]
        public virtual ConsultDilPresc ConsultDilPresc { get; set; }

        public bool IsSpecialOrder { get; set; } //true=for other sales -- false for frame and lens sales
        public string Remarque { get; set; }
        public string MedecinTraitant { get; set; }

        public bool IsDelivered { get; set; }

        public bool IsDilatation { get; set; }

        [StringLength(100)]
        public string CNI { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? DateRdv { get; set; } //date de rendez vous de reception du produit

        //gestionnaire de compte
        public int? GestionnaireID { get; set; }
        [ForeignKey("GestionnaireID")]
        public virtual User Gestionnaire { get; set; }

        public int? SellerID { get; set; }
        [ForeignKey("SellerID")]
        public virtual User Seller { get; set; }

        [NotMapped]
        public SalePurchaseStatut OldStatutSale { get; set; }
       
        /// <summary>
        /// This a NotMapped attribute is not persiste in database. 
        /// It return a temporary value of sum of salelines price to display in view
        /// </summary>
        [NotMapped]
        public double TotalPriceHT { get; set; }
        /// <summary>
        /// This a NotMapped attribute is not persiste in database. 
        /// It return a temporary TotalPriceTT value that we want to validate
        /// </summary>
        [NotMapped]
        public double TotalPriceTTC { get; set; }
        [NotMapped]
        public bool IsOtherSale { get; set; }

        /// <summary>
        /// This a NotMapped attribute is not persiste in database. 
        /// It return a temporary ReductionAmount 
        /// </summary>
        [NotMapped]
        public double ReductionAmount { get; set; }
        /// <summary>
        /// This a NotMapped attribute is not persiste in database. 
        /// It return a temporary DiscountAmount 
        /// </summary>
        [NotMapped]
        public double DiscountAmount { get; set; }
        [NotMapped]
        public double TVAAmount { get; set; }
        /// <summary>
        /// This a NotMapped attribute is not persiste in database. 
        /// It return a temporary value of name of customer price to display in view
        /// </summary>
        [NotMapped]
        public string PersonName
        {
            get
            {
                if (this.Customer != null)
                    return string.Concat( this.Customer.Name," ", (this.Customer.Description==null) ? "": this.Customer.Description) ;
                return "";
            }
        }

        

        [NotMapped]
        public string SaleFullInformation
        {
            get
            {
                return this.SaleReceiptNumber + " " + this.SaleDate.Date.ToShortDateString() + " " + this.TotalPriceTTC;
            }
        }
        /// <summary>
        /// This a NotMapped attribute is not persiste in database. 
        /// It return a temporary value of name of AdressEmail to display in view
        /// </summary>
        [NotMapped]
        public string AdressEmail
        {
            get
            {
                if (this.Customer != null && this.Customer.Adress != null && this.Customer.Adress.AdressEmail != null && this.Customer.Adress.AdressEmail.Length > 0)
                    return this.Customer.Adress.AdressEmail.ToString();
                return "";

            }
        }
        [NotMapped]
        public string AdressPhoneNumber
        {
            get
            {
                if (this.Customer != null && this.Customer.Adress != null)
                    return this.Customer.Adress.AdressPhoneNumber;
                return "";
            }
        }
        [NotMapped]
        public double NetFinancier { get; set; }
        [NotMapped]
        public double NetCommercial { get; set; }
        [NotMapped]
        public bool isPartInsured { get; set; }
        [NotMapped]
        public string PreferredLanguage { get; set; }
        [NotMapped]
        public string CustomerFullName { get; set; }
        [NotMapped]
        public double SaleTotalPrice { get; set; }
        [NotMapped]
        public double Advanced { get; set; }
        [NotMapped]
        public double Remainder { get; set; }
        [NotMapped]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime DateOfBirth {get; set;}

    }
    //public enum StatutSale {Order, Delivery, Billing, Advance, Payment, Return, CancelAdvanced };
}
