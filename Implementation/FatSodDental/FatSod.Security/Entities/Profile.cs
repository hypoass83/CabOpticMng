using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FatSod.Security.Entities
{
    [Serializable]
    public class Profile
    {
        public int ProfileID { get; set; }
        [Required]
        [StringLength(100)]
        [Index("ProfileCode", IsUnique = true, IsClustered = false)]
        public string ProfileCode { get; set; }
        public string ProfileLabel { get; set; }
        public string ProfileDescription { get; set; }
        public bool ProfileState { get; set; }
        public int PofilLevel { get; set; }
        public virtual ICollection<User> Users { get; set; }
        public virtual ICollection<ActionMenuProfile> ActionMenuProfiles { get; set; }
        public virtual ICollection<ActionSubMenuProfile> ActionSubMenuProfiles { get; set; }
    }
}
