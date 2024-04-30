using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FatSod.Supply.Entities
{
    [Serializable]
    public class Devise
    {
        public int DeviseID { get; set; }
        [Required]
        [StringLength(100)]
        [Index("DeviseCode", IsUnique = true, IsClustered = false)]
        public string DeviseCode { get; set; }
        public string DeviseLabel { get; set; }
        public string DeviseDescription { get; set; }
        public bool DefaultDevise { get; set; }
        public virtual ICollection<AccountOperation> AccountOperations { get; set; }
    }
}
