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
    public class Slice
    {
        public int SliceID { get; set; }
        public double SliceAmount { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime SliceDate { get; set; }
        public int DeviseID { get; set; }
        [ForeignKey("DeviseID")] 
        public virtual Devise Devise { get; set; }
        public int PaymentMethodID { get; set; }
        [ForeignKey("PaymentMethodID")]
        public virtual PaymentMethod PaymentMethod { get; set; }
        //C'est le nom de celui qui est venu avec l'argent; cest aussi le general public specifique
        public string Representant { get; set; }
        public int? OperatorID { get; set; }
        [ForeignKey("OperatorID")]
        public virtual User Operator { get; set; }
        #region DIGITAL PAYMENT METHOD
        public string TransactionIdentifier { get; set; }
        public int? DigitalAccountManagerId { get; set; }
        [ForeignKey("DigitalAccountManagerId")]
        public virtual User DigitalAccountManager { get; set; }
        #endregion

    }
}
