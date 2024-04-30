using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FatSod.Supply.Entities
{
    [Serializable]
    public class BankSale : Sale
    {
        //public int BankSaleID { get; set; }
        public int BankID { get; set; }
        [ForeignKey("BankID")]
        public virtual Bank Bank { get; set; }
        public string BankRef { get; set; }
    }
}
