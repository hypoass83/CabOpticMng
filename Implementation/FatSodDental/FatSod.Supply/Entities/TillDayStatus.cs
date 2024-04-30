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
    public class TillDayStatus
    {
        public int TillDayStatusID { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime TillDayLastOpenDate{ get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime TillDayLastClosingDate { get; set; }
        public bool IsOpen { get; set; }
        public int TillID { get; set; } 
        [ForeignKey("TillID")]
        public virtual Till Till { get; set; }
    }
}
