using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FatSod.Supply.Entities
{
    [Serializable]
    public class LensCoating
    {        
        public int LensCoatingID { get; set; }
        [Required]
        [StringLength(100)]
        [Index("LensCoatingCode", IsUnique = true, IsClustered = false)]
        public string LensCoatingCode { get; set; }
        public string LensCoatingLabel { get; set; }
        public string LensCoatingDescription { get; set; }
        public virtual ICollection<LensCategory> Lenses { get; set; }
    }
}
