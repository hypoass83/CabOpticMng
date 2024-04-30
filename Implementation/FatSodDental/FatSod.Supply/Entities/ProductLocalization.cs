using FatSod.Security.Entities;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FatSod.Supply.Entities
{
    [Serializable]
    public class ProductLocalization
    {
        public int ProductLocalizationID { get; set; }
        public string BarCode { get; set; }
        // Printed barcode which has not been used for many reasons: wrong correspondance or other reason
        public int ReservedBarCode { get; set; }
        public double ProductLocalizationStockQuantity { get; set; }
        public double ProductLocalizationSafetyStockQuantity { get; set; }
        public double ProductLocalizationStockSellingPrice { get; set; }
        //c'est le prix d'achat moyen. il est important afin de comptabiliser les dépréciations de stocks pendant l'inventaire par exemple
        public double AveragePurchasePrice { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime ProductLocalizationDate { get; set; }
        [Index("IX_RealPrimaryKey", 1, IsUnique = true)]
        public int ProductID { get; set; }
        [ForeignKey("ProductID")]
        public virtual Product Product { get; set; }
        [Index("IX_RealPrimaryKey", 2, IsUnique = true)]
        public int LocalizationID { get; set; }
        [ForeignKey("LocalizationID")]
        public virtual Localization Localization { get; set; }

        public string NumeroSerie { get; set; }
        public string Marque { get; set; }
        

        public override bool Equals(object obj)
        {
            if (obj == null) { return false; }
            if (!(obj is ProductLocalization)) { return false; }
            ProductLocalization l = (ProductLocalization)obj;
            return this.ProductID == l.ProductID &&
                   this.LocalizationID == l.LocalizationID
                ;
        }

        public bool isDeliver { get; set; }

        //No Map fields
        [NotMapped]
        public double AddedQuantity { get; set; }
        [NotMapped]
        public string LocationCode
        {
            get
            {
                return this.Localization.LocalizationCode;
            }
        }
        [NotMapped]
        public string BranchName { get; set; }
        [NotMapped]
        public string ProductCode
        {
            get
            {
                return this.Product.ProductCode;
            }
        }
        [NotMapped]
        public string ProductCodePrint
        {
            get;
            set;
        }
        [NotMapped]
        public string inventoryReason { get; set; }
        [NotMapped]
        public int AutorizedByID { get; set; }
        [NotMapped]
        public int CountByID { get; set; }
        [NotMapped]
        public int RegisteredByID { get; set; }

        [NotMapped]
        public String ProductLabel 
        { 
            get
            {
                return this.Product.ProductLabel;
            } 
        }

        
        [NotMapped]
        public String LocalizationLabel
        {
            get
            {
                return this.Localization.LocalizationLabel;
            }
        }

        [NotMapped]
        public String LocalizationLabelPrint
        {
            get;
            set;
        }

        [NotMapped]
        public double Amount
        {
            get
            {
                return this.ProductLocalizationStockQuantity * this.ProductLocalizationStockSellingPrice;
            }
        }

        [NotMapped]
        public bool IsSafQuantStockReached
        {
            get
            {
                return this.ProductLocalizationSafetyStockQuantity >= this.ProductLocalizationStockQuantity;
            }
        }
        [NotMapped]
        public bool isStockInput{get;set;}
        [NotMapped]
        public int BranchID { get; set; }
        [NotMapped]
        public int DeviseID { get; set; }
        [NotMapped]
        public string Description { get; set; }
        [NotMapped]
        public string SellingReference { get; set; }

    }
}
