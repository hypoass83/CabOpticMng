using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace FatSod.Security.Entities
{
    [Serializable]
    public class ActionMenuProfile
    {
        public int ActionMenuProfileID {get;set;}
        public bool Delete { get; set; }
        public bool Add { get; set; }
        public bool Update { get; set; }
        [Index("IX_RealPrimaryKey", 1, IsUnique = true)] 
        public int MenuID { get; set; }
        [ForeignKey("MenuID")]
        public virtual Menu Menu { get; set; }
        [Index("IX_RealPrimaryKey", 2, IsUnique = true)]
        public int ProfileID { get; set; }
        [ForeignKey("ProfileID")]
        public virtual Profile Profile { get; set; }
    }
}
