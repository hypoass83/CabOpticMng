using FatSod.Security.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FatSod.Supply.Entities
{
    [Serializable]
    public class InventoryDirectory
    {
        public int InventoryDirectoryID { get; set; }
        public string InventoryDirectoryReference { get; set; }
        public string InventoryDirectoryDescription { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime InventoryDirectoryCreationDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime InventoryDirectoryDate { get; set; }
        public InventoryDirectorySatut InventoryDirectoryStatut { get; set; }
        #region BARCODE
        public bool IsBarcodePrinted { get; set; }
        #endregion
        public int BranchID { get; set; }
        [ForeignKey("BranchID")]
        public virtual Branch Branch { get; set; }
        public int? AutorizedByID { get; set; }
        [ForeignKey("AutorizedByID")]
        public virtual User AutorizedBy { get; set; }
        public int? RegisteredByID { get; set; }
        [ForeignKey("RegisteredByID")]
        public virtual User RegisteredBy { get; set; }
        public virtual ICollection<InventoryDirectoryLine> InventoryDirectoryLines { get; set; }
    }
}
