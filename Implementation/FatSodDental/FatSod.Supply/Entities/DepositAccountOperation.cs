using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FatSod.Supply.Entities
{
    [Serializable]
    public class DepositAccountOperation : AccountOperation
    {

        public int DepositID { get; set; }
        [ForeignKey("DepositID")]
        public virtual Deposit Deposit { get; set; }
    }
}
