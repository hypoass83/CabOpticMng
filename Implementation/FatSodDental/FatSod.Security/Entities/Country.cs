using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FatSod.Security.Entities
{
    [Serializable]
    public class Country
    {
        public int CountryID { get; set; }
        [StringLength(100)]
        [Index("CountryCode", IsUnique = true, IsClustered = false)]
        public string CountryCode { get; set; }
        public string CountryLabel { get; set; }
        public virtual ICollection<Region> Regions { get; set; }
    }
}
