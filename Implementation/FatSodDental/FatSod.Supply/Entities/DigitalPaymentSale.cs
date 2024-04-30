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
    public class DigitalPaymentSale : Sale
    {
        // Id de celui qui envoie; c est le numero de telephone dans le cas le cas des operateurs mobile
        //[Index("IX_RealPrimaryKey", 1, IsUnique = true)]
        //[StringLength(50)]
        //public string SenderIdentifier { get; set; }

        // Identifiant de la transaction
        [Index("IX_RealPrimaryKey", 1, IsUnique = true)]
        [StringLength(50)]
        public string TransactionIdentifier { get; set; }

        [Index("IX_RealPrimaryKey", 2, IsUnique = true)]
        public int DigitalPaymentMethodId { get; set; }
        [ForeignKey("DigitalPaymentMethodId")]
        public virtual DigitalPaymentMethod DigitalPaymentMethod { get; set; }

        public int DigitalAccountManagerId { get; set; }
        [ForeignKey("DigitalAccountManagerId")]
        public virtual User DigitalAccountManager { get; set; }
    }
}
