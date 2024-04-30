using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FatSod.Supply.Entities
{
    [Serializable]
    public class LensMaterial
    {        
        public int LensMaterialID { get; set; }
        [Required]
        [StringLength(100)]
        [Index("LensMaterialCode", IsUnique = true, IsClustered = false)]
        public string LensMaterialCode { get; set; }
        public string LensMaterialLabel { get; set; }
        public string LensMaterialDescription { get; set; }
        public virtual ICollection<LensCategory> Lenses { get; set; }
    }
}
