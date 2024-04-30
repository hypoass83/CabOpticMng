using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FatSod.Supply.Entities
{
    [Serializable]
	public class TillAdjustAccountOperation : AccountOperation
	{
		//cle etranngere vers TillAdjust
		public int TillAdjustID { get; set; }
		[ForeignKey("TillAdjustID")]
		public virtual TillAdjust TillAdjust { get; set; }
	}
}
