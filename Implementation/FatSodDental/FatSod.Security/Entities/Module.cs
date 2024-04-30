using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FatSod.Security.Entities
{
    [Serializable]
    public class Module
    {
        public int ModuleID { get; set; }
        [Required]
        [StringLength(100)]
        [Index("ModuleCode", IsUnique = true, IsClustered = false)]
        public string ModuleCode { get; set; }
        public string ModuleLabel { get; set; }
        public string ModuleDescription { get; set; }
        public string ModuleImagePath { get; set; }
        public string ModulePressedImagePath { get; set; }
        public string ModuleDisabledImagePath { get; set; }
        public string ModuleArea { get; set; }
        public int ModuleImageHeight { get; set; }
        public int ModuleImageWeight { get; set; }
        public bool ModuleState { get; set; }
        // Ordre d'apparition du module dans la liste des Modules
        public int AppearanceOrder { get; set; }
        public ModuleStatus ModuleStatus { get; set; }
        public virtual ICollection<Menu> Menus { get; set; }
    }

    public enum ModuleStatus
    {
        ACTIVATED,
        DESACTIVATED,
        HIDDEN,
        DELETED,
        ARCHIVED,
    }
}
