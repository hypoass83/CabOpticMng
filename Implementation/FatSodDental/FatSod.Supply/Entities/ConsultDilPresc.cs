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
    public class ConsultDilPresc
    {
        public int ConsultDilPrescID { get; set; }

        public int ConsultationID { get; set; }
        [ForeignKey("ConsultationID")]
        public virtual Consultation Consultation { get; set; }

        public int CustomerNumber { get; set; }
        
    }
}
