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
    public class HistoRendezVous
    {
        public int HistoRendezVousID { get; set; }
        
        /// <summary>
        /// sale FK
        /// </summary>
        public int SaleID { get; set; }
        [ForeignKey("SaleID")]
        public virtual Sale Sale { get; set; }

        /// <summary>
        /// RendezVous FK
        /// </summary>
        public int RendezVousID { get; set; }
        [ForeignKey("RendezVousID")]
        public virtual RendezVous RendezVous { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime DateRdv { get; set; }
        public string Remarque { get; set; }
        public int? OperatorID { get; set; }
        [ForeignKey("OperatorID")]
        public virtual User Operator { get; set; }
    }

}
