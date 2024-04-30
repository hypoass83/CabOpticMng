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
	public class TillAdjust
	{
		public int TillAdjustID { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime TillAdjustDate { get; set; }
		public double ComputerPrice { get; set; }
		public double PhysicalPrice { get; set; }
		public string Justification { get; set; }
		public int TillID { get; set; }
		[ForeignKey("TillID")]
		public virtual Till Till { get; set; }
		//cle etrangere vers Devise
		public int DeviseID { get; set; }
		[ForeignKey("DeviseID")]
		public virtual Devise Devise { get; set; }
        public virtual ICollection<TillAdjustAccountOperation> TillAdjustAccountOperations { get; set; }
		[NotMapped]
		public double ecart {get;set;}
	}
}
