using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FatSod.Supply.Entities
{
    [Serializable]
    public class Category
    {        
        public int CategoryID { get; set; }
        [Required]
        [StringLength(100)]
        [Index("CategoryCode", IsUnique = true, IsClustered = false)]
        public string CategoryCode { get; set; }
        public string CategoryLabel { get; set; }
        public string CategoryDescription { get; set; }
        public virtual ICollection<Product> Products { get; set; }
        public bool isSerialNumberNull { get; set; }
    }
}
