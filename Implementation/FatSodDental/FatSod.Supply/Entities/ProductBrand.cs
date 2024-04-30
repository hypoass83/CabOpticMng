using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FatSod.Supply.Entities
{
    [Serializable]
    public class ProductBrand
    {        
        public int ProductBrandID { get; set; }
        [Required]
        [StringLength(100)]
        [Index("ProductBrandCode", IsUnique = true, IsClustered = false)]
        public string ProductBrandCode { get; set; }
        public string ProductBrandLabel { get; set; }
        
        public virtual ICollection<ProductLocalization> ProductLocalizations { get; set; }
    }
}
