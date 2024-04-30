using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FatSod.Supply.Entities
{
    [Serializable]
    public class SavingAccountSale: Sale
    {
        public int SavingAccountID { get; set; }
        [ForeignKey("SavingAccountID")]
        public virtual SavingAccount SavingAccount { get; set; }
    }
}
