using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FatSod.Security.Entities
{
    [Serializable]
    public class Region
    {
        public int RegionID { get; set; }
        [StringLength(50)]
        [Index("IX_RealPrimaryKey", 1, IsUnique = true)]//clé étrangère composée pour permettre la création de deux régions ayant le même code dans deux pays différents
        public string RegionCode { get; set; }
        public string RegionLabel { get; set; }
        [Index("IX_RealPrimaryKey", 2, IsUnique = true)]//clé étrangère composée pour permettre la création de deux régions ayant le même code dans deux pays différents
        public int CountryID { get; set; }
        [ForeignKey("CountryID")]
        public virtual Country Country { get; set; }
        public virtual ICollection<Town> Towns { get; set; }
    }
}
