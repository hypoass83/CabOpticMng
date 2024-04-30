using FatSod.Security.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FatSod.Supply.Entities
{
    [Serializable]
    public class Sale
    {
        public int SaleID { get; set; }
        public int CompteurFacture { get; set; }
        public bool SaleDeliver { get; set; }
        public double VatRate { get; set; }
        public double RateReduction { get; set; }
        public double RateDiscount { get; set; }
        public double Transport { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? SaleDeliveryDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime SaleDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime SaleDateHours { get; set; }
        public bool SaleValidate { get; set; }
        public string PoliceAssurance { get; set; }
        public int PaymentDelay { get; set; }
        public int Guaranteed { get; set; }
        public string Patient { get; set; }
        public int DeviseID { get; set; }
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
        public virtual ICollection<SaleLine> SaleLines { get; set; }
        public virtual ICollection<SaleAccountOperation> SaleAccountOperations { get; set; }

        //les paiements d'une vente
        public virtual ICollection<CustomerSlice> CustomerSlices { get; set; }

        public int? OperatorID { get; set; }
        [ForeignKey("OperatorID")]
        public virtual User Operator { get; set; }

        public int? PostByID { get; set; }
        [ForeignKey("PostByID")]
        public virtual User PostBy { get; set; }

        public int? SellerID { get; set; }
        [ForeignKey("SellerID")]
        public virtual User Seller { get; set; }

        public int? ConsultDilPrescID { get; set; }
        [ForeignKey("ConsultDilPrescID")]
        public virtual ConsultDilPresc ConsultDilPresc { get; set; }

        public bool IsSpecialOrder { get; set; } //pour ne pas m'enbrouiller jai use cette variable pour distinguer les factures pour assurance avec les ventes cash
        public string Remarque { get; set; }
        public string MedecinTraitant { get; set; }

        public int? CustomerOrderID { get; set; }
        [ForeignKey("CustomerOrderID")]
        public virtual CustomerOrder CustomerOrderFK { get; set; }

        public int? AuthoriseSaleID { get; set; }
        [ForeignKey("AuthoriseSaleID")]
        public virtual AuthoriseSale AuthoriseSaleFK { get; set; }
        //gestionnaire de compte
        public int? GestionnaireID { get; set; }
        [ForeignKey("GestionnaireID")]
        public virtual User Gestionnaire { get; set; }
        /*************** variable pour la gestion des rendez vs pour les verres de commande *******************/

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? DateRdv { get; set; } //date de rendez vous de reception du produit


        public bool IsRendezVous { get; set; } //cette variable permet de determiner si une vente sera considerer comme commande speciale
        public int? AwaitingDay { get; set; } //nombre probable de jour d'attente
        public bool IsProductReveive { get; set; } //determine la reception du produit
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? EffectiveReceiveDate { get; set; } //date effective de reception du produit
        public bool IsCustomerRceive { get; set; } //si reception par le client
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? CustomerDeliverDate { get; set; } //date de reception par le client

        public int? PostSOByID { get; set; }
        [ForeignKey("PostSOByID")]
        public virtual User PostSOBy { get; set; }

        public int? ReceiveSOByID { get; set; }
        [ForeignKey("ReceiveSOByID")]
        public virtual User ReceiveSOBy { get; set; }
        public bool IsUrgent { get; set; }

        [StringLength(100)]
        public string CNI { get; set; }

        [NotMapped]
        public bool isDilation { get; set; }
        [NotMapped]
        public string CustomerNumber { get; set; }
        [NotMapped]
        public SalePurchaseStatut OldStatutSale { get; set; }
        /// <summary>
        /// This a NotMapped attribute is not persiste in database. 
        /// It return a CustomerSlice value of this sale
        /// </summary>
        [NotMapped]
        public CustomerSlice CustomerSlice { get; set; }
        /// <summary>
        /// This a NotMapped attribute is not persiste in database. 
        /// It return a temporary value of payment method that user has choiced
        /// </summary>
        [NotMapped]
        public int PaymentMethodID { get; set; }

        [NotMapped]
        public string PaymentReference { get; set; }

        /// <summary>
        /// Reste à payer sur la facture de vente
        /// </summary>
        [NotMapped]
        public double SaleTotalPriceRemainder
        {
            get
                /*{
                    return this.TotalPriceTTC - this.SaleTotalPriceAdvance;
                }*/
                ;
            set;
        }

        [NotMapped]
        public double SaleTotalPriceAdvance { get; set; }
        /// <summary>
        /// This a NotMapped attribute is not persiste in database. 
        /// It return a temporary CustomerOderID value that we want to validate
        /// </summary>
        //[NotMapped]
        
        /// <summary>
        /// This a NotMapped attribute is not persiste in database. 
        /// It return a temporary RemaingAmount value that we want to validate
        /// </summary>
        [NotMapped]
        public double RemaingAmount { get; set; }
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
        public bool IsOtherSale { get; set; }
        [NotMapped]
        public bool IsValidatedSale { get; set; }
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
                    return this.Customer.Name;
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
                {
                    var res = this.Customer.Adress.AdressPhoneNumber;
                    res = !String.IsNullOrEmpty(res) ? res : this.Customer.Adress.AdressCellNumber;
                    return res;
                }
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
        public string CustomerFullName { get; set; }
        [NotMapped]
        public double SaleTotalPrice { get; set; }
        [NotMapped]
        public double Advanced { get; set; }
        [NotMapped]
        public double Remainder { get; set; }
        [NotMapped]
        public double MontantClientDeposit { get; set; }
        [NotMapped]
        public double MontantTotalClientAdvance { get; set; }


        [NotMapped]
        public string PhoneNumber { get; set; }
        [NotMapped]
        public string PreferredLanguage { get; set; }


    }
    //public enum StatutSale {Order, Delivery, Billing, Advance, Payment, Return, CancelAdvanced };
}
