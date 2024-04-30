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
    /// Cette table contiendra l'historique de l'affectation et la désaffectation des magasins aux magasiniers principaux.
    /// cette table sera peuplée chaque fois qu'on : 
    /// 1 - changera le magasinier principal d'un magasinier (si le magasinier principal dévient simple magasinier)
    /// 2 - retirera le magasin au magasinier.( si la ligne EmployeeStock du magasinier principal est supprimée )
    /// </summary>
    public class EmployeeStockHistoric
    {
        public int EmployeeStockHistoricID { get; set; }
        /// <summary>
        /// date vennant de la table EmployeeStock
        /// </summary>
        /// 
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime AssigningDate { get; set; }
        /// <summary>
        /// Date du jour où le magasin à été enlever au magasinier principal
        /// </summary>
        /// 
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime RemovingDate { get; set; }
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