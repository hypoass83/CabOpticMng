using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FatSod.Supply.Entities
{
    [Serializable]
    public class TreasuryOperationAccountOperation : AccountOperation
	{
		//cle etranngere vers TreasuryOperation
		public int TreasuryOperationID { get; set; }
		[ForeignKey("TreasuryOperationID")]
		public virtual TreasuryOperation TreasuryOperation { get; set; }
	}
}
