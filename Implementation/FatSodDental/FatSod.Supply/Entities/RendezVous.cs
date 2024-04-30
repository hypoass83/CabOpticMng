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
    public class RendezVous
    {
        public int RendezVousID { get; set; }
        
        /// <summary>
        /// Customer FK
        /// </summary>
        public int CustomerID { get; set; }
        [ForeignKey("CustomerID")]
        public virtual Customer Customer { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime DateRdv { get; set; }
        public string RaisonRdv { get; set; }

        /// <summary>
        /// Sale FK
        /// </summary>
        public int? SaleID { get; set; }
        [ForeignKey("SaleID")]
        public virtual Sale Sale { get; set; }

        /// <summary>
        /// product FK
        /// </summary>
        public int? ProductID { get; set; }
        [ForeignKey("ProductID")]
        public virtual Product Product { get; set; }
    }

}
