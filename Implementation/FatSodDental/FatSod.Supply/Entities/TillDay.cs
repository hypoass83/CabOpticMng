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
    public class TillDay
    {
        public int TillDayID { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime TillDayDate{ get; set; }
        public bool IsOpen { get; set; }
        public double TillDayOpenPrice { get; set; }
        public double TillDayClosingPrice { get; set; }
        public int TillID { get; set; } 
        [ForeignKey("TillID")]
        public virtual Till Till { get; set; }

        public double TillDayCashHand { get; set; }
    }
}
