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
    /// <summary>
    /// Cette table définie les employés qui sont des magasiniers.
    /// Il n'y a que le magasinier principal qui peut éffectuer les tâches du magasinier
    /// Un magasin ne peut avoir qu'un et un seul magasinier principal
    /// </summary>
    public class EmployeeStock
    {
        public int EmployeeStockID { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime AssigningDate { get; set; }
        public bool IsPrincipalWareHouseMan { get; set; }
        /// <summary>
        /// Magazinier
        /// </summary>
        public int WareHouseManID { get; set; }
        [ForeignKey("WareHouseManID")]
        public virtual User WareHouseMan { get; set; }
        /// <summary>
        /// Lieux de stockage ou entrepôt
        /// </summary>
        public int WareHouseID { get; set; }
        [ForeignKey("WareHouseID")]
        public virtual Localization WareHouse { get; set; }
    }
}
