using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FatSod.Security.Entities
{
    [Serializable]
    public class SubMenu
    {
        public int SubMenuID { get; set; }
        // Ordre d'apparition du sous menu dans la liste des menus
        public int AppearanceOrder { get; set; }
        public ModuleStatus SubMenuStatus { get; set; }
        [Required]
        [StringLength(100)]
        [Index("SubMenuCode", IsUnique = true, IsClustered = false)]
        public string SubMenuCode{get;set;}
        public string SubMenuLabel { get; set; }
        public string SubMenuDescription { get; set; }
        public string SubMenuController { get; set; }
        public string SubMenuPath { get; set; }
        public bool IsChortcut { get; set; }
        public int MenuID { get; set; }
        [ForeignKey("MenuID")]
        public virtual Menu Menu { get; set; }
        public SubMenu()
        {
            this.IsChortcut = false;
        }

        public virtual ICollection<ActionSubMenuProfile> ActionSubMenuProfiles { get; set; }

    }
}
