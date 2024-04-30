using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FatSod.Supply.Entities
{
    [Serializable]
    public class LensColour
    {        
        public int LensColourID { get; set; }
        [Required]
        [StringLength(100)]
        [Index("LensColourCode", IsUnique = true, IsClustered = false)]
        public string LensColourCode { get; set; }
        public string LensColourLabel { get; set; }
        public string LensColourDescription { get; set; }
        public virtual ICollection<LensCategory> Lenses { get; set; }
    }
}
