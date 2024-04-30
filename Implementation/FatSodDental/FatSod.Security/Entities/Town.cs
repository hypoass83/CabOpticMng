using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FatSod.Security.Entities
{
    [Serializable]
    public class Town
    {
        public int TownID { get; set; }
        [StringLength(100)]
        [Index("TownCode", IsUnique = true, IsClustered = false)]
        public string TownCode { get; set; }
        public string TownLabel { get; set; }
        public int RegionID { get; set; }
        [ForeignKey("RegionID")]
        public virtual Region Region { get; set; }
        public virtual ICollection<Quarter> Quarters { get; set; }
    }
}
