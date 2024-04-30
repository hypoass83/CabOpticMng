using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FatSod.Supply.Entities
{
    [Serializable]
    /// <summary>
    /// pour les achats donc le mode de paiement est espèce
    /// </summary>
    public class TillPurchase : Purchase
    {
        /// <summary>
        /// c'est une méthode de paiement
        /// </summary>
        public int TillID { get; set; }
        [ForeignKey("TillID")]
        public virtual Till Till { get; set; }
    }
}
