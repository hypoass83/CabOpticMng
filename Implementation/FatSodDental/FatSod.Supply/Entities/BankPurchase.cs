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
    /// pour les achats dont le mode de paiement est la banque
    /// </summary>
    public class BankPurchase : Purchase
    {
        public int BankID { get; set; }
        [ForeignKey("BankID")]
        public virtual Bank Bank { get; set; }
    }
}
