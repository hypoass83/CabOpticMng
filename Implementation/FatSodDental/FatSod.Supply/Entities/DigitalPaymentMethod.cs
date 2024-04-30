using FatSod.Security.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FatSod.Supply.Entities
{
    [Serializable]
    public class DigitalPaymentMethod : PaymentMethod
    {
        // Provider             <====>  Name => Yoomee Money; Orange Money; MTN Money; Bit Coin; Liyeplimal; Simbcoin; etc
        // ReceiverIdentifier   <====> Code (Phone Number; S) //=> 
        // Usage during payment <====> Provider(Code) <====> Orange Money(690 96 06 82): MTN Money(678 048 798); YUP(678 048 798); Liyeplimal(abdghf); BitCoin(sdsdnsds)
        public int AccountID { get; set; }
        [ForeignKey("AccountID")]
        public virtual Account Account { get; set; }

        public int AccountManagerId { get; set; }
        [ForeignKey("AccountManagerId")]
        public virtual User AccountManager { get; set; }

        public bool IsEnable { get; set; }
        public virtual ICollection<DigitalPaymentSale> DigitalPaymentSales { get; set; }

        [NotMapped]
        public string FullName
        {
            get
            {
                return this.Name + " (" + this.Code + ")";
            }
        }
    }
}
