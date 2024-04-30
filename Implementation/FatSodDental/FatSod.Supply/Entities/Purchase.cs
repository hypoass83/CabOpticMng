using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FatSod.Security.Entities;
using System.ComponentModel.DataAnnotations;

namespace FatSod.Supply.Entities
{
    [Serializable]
    public class Purchase
    {
        public int PurchaseID { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime PurchaseDeliveryDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime PurchaseDate { get; set; }
        public double VatRate { get; set; }
        public double RateReduction { get; set; }
        public double RateDiscount { get; set; }
        public int Guaranteed { get; set; }
        public double Transport { get; set; }
        public int PaymentDelay { get; set; }
        public bool PurchaseValidate { get; set; }
        public bool isReturn { get; set; }
        public SalePurchaseStatut StatutPurchase { get; set; }
        [NotMapped]
        public SalePurchaseStatut OldStatutPurchase { get; set; }
        //l'utilisateur qui enregistre l'achat dans le système
        [Required]
        [StringLength(50)]
        [Index("PurchaseReference", IsUnique = true, IsClustered = false)]
        public string PurchaseReference { get; set; }
        public int PurchaseRegisterID { get; set; }
        [ForeignKey("PurchaseRegisterID")]
        public virtual User PurchaseRegister { get; set; }

        public int BranchID { get; set; }
        [ForeignKey("BranchID")]
        public virtual Branch Branch { get; set; }

        public int DeviseID { get; set; }
        [ForeignKey("DeviseID")]
        public virtual Devise Devise { get; set; }

        //l'employé qui a apporté les produits achetés
        public int PurchaseBringerID { get; set; }
        [ForeignKey("PurchaseBringerID")]
        public virtual User PurchaseBringer { get; set; }
        public int SupplierID { get; set; }
        [ForeignKey("SupplierID")]
        public virtual Supplier Supplier { get; set; }
        public virtual ICollection<PurchaseLine> PurchaseLines { get; set; }

        //Début des champs non persistants

        [NotMapped]
        public int PaymentMethodID { get; set; }
        [NotMapped]
        public String PaymentMethod { get; set; }
        [NotMapped]
        public double TotalPriceHT { get; set; }
        /// <summary>
        /// This a NotMapped attribute is not persiste in database. 
        /// It return a temporary TotalPriceTT value that we want to validate
        /// </summary>
        [NotMapped]
        public double TotalPriceTTC { get; set; }
        [NotMapped]
        public double TVAAmount { get; set; }
        [NotMapped]
        public String SupplierFullName
        {
            get
            {
                return this.Supplier.Name + " " + this.Supplier.Description;
            }
        }
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
        public String PurchaseBringerFullName
        {
            get
            {
                return this.PurchaseBringer.Name + " " + this.PurchaseBringer.Description;
            }
        }
        [NotMapped]
        public double PurchasePriceAdvance { get; set; }
        [NotMapped]
        public SupplierSlice CurrentSupplierSlice { get; set; }

        [NotMapped]
        public String PurchaseRegisterFullName
        {
            get
            {
                return this.PurchaseRegister.Name + " " + this.PurchaseRegister.Description;
            }
        }

        [NotMapped]
        public string SupplierEmail
        {
            get
            {
                return this.Supplier.AdressEmail;
            }
        }
        [NotMapped]
        public string SupplierPhoneNumber
        {
            get
            {
                return this.Supplier.AdressPhoneNumber;
            }
        }
        [NotMapped]
        public double NetFinancier { get; set; }
        [NotMapped]
        public double NetCommercial { get; set; }
    }
}
