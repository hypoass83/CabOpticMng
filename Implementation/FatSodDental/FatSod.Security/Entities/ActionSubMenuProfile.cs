using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace FatSod.Security.Entities
{
    [Serializable]
    public class ActionSubMenuProfile
    {
        public int ActionSubMenuProfileID { get; set; }

        public bool Delete { get; set; }
        public bool Add { get; set; }
        public bool Update { get; set; }
        [Index("IX_RealPrimaryKey", 1, IsUnique = true)]
        public int SubMenuID { get; set; }
        [ForeignKey("SubMenuID")]
        public virtual SubMenu SubMenu { get; set; }
        [Index("IX_RealPrimaryKey", 2, IsUnique = true)]
        public int ProfileID { get; set; }
        [ForeignKey("ProfileID")]
        public virtual Profile Profile { get; set; }
    }
}
