using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FatSod.Security.Entities
{
    [Serializable]
    public class Menu
    {
        public int MenuID { get; set; }
        // Ordre d'apparition du menu dans la liste des Modules
        public int AppearanceOrder { get; set; }
        public ModuleStatus MenuStatus { get; set; }
        [Required]
        [StringLength(100)]
        [Index("MenuCode", IsUnique = true, IsClustered = false)]
        public string MenuCode { get; set; }
        public string MenuDescription { get; set; }
        public string MenuController { get; set; }
        public bool MenuState { get; set; }
        public string MenuLabel { get; set; }
        public bool MenuFlat { get; set; }
        public string MenuIconName { get; set; }
        public string MenuPath { get; set; }
        public bool IsChortcut { get; set; }
        public int ModuleID { get; set; }
        [ForeignKey("ModuleID")]
        public virtual Module Module { get; set; }
        public virtual ICollection<SubMenu> SubMenus { get; set; }
        public Menu()
        {
            this.IsChortcut = false;
        }
    }
}
