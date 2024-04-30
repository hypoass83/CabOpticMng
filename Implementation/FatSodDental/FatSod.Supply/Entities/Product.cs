using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml;
using System.Web;
using System.Globalization;

namespace FatSod.Supply.Entities
{
    [Serializable]
    public class Product
    {
        public int ProductID { get; set; }
        [Required]
        [StringLength(100)]
        [Index("ProductCode", IsUnique = true, IsClustered = false)]
        public string ProductCode { get; set; }
        public string ProductLabel { get; set; }
        [NotMapped]
        public double ProductSafetyStockQuantity { get; set; }
        public string ProductDescription { get; set; }
        public int CategoryID { get; set; }
        [ForeignKey("CategoryID")]
        public virtual Category Category { get; set; }
        public int AccountID { get; set; }
        [ForeignKey("AccountID")]
        public virtual Account Account { get; set; }
        public virtual ICollection<ProductLocalization> ProductLocalizations { get; set; }
        public double SellingPrice { get; set; }

        public string ProductCategoryCode { get; set; }
        public string Prescription { get; set; }

        public string GetProductCode()
        {
            if (this == null) return "";
            return ((this is Lens) || (this is OrderLens)) ? this.ProductCode : this.ProductLabel;
        }
        public virtual ICollection<BarCodeGenerator> BarCodeGenerators { get; set; }

        [NotMapped]
        public double ProductQuantity { get; set; }
        [NotMapped]
        public int LocalizationID { get; set; }//Utilisée pour empécher les entrées et sorties d'un produits dans un magasin pendant l'inventaire
        public override bool Equals(object obj)
        {
            if (obj == null) { return false; }
            if (!(obj is Product)) { return false; }
            Product p = (Product)obj;
            return this.ProductID == p.ProductID &&
                   this.ProductCode == p.ProductCode 
                ;
        }
        [NotMapped]
        public string Localization { get; set; }

        [NotMapped]
        public int[] Stores { get; set; }
       
    }
}
