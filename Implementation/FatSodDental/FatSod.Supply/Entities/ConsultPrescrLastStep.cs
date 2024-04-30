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
    public class ConsultPrescrLastStep
    {
        public int ConsultPrescrLastStepID { get; set; }

        public int ConsultationID { get; set; }
        [ForeignKey("ConsultationID")]
        public virtual Consultation Consultation { get; set; }

        public DateTime DateNextConsultation { get; set; }
        public bool IsCollyre { get; set; }
        public string NomCollyre { get; set; }
        public string Remark { get; set; }

        public int ConsultByID { get; set; }
        [ForeignKey("ConsultByID")]
        public virtual User ConsultBy { get; set; }

        public string HeureConsLastStep { get; set; }

        public int CustomerNumber { get; set; }
    }
}
