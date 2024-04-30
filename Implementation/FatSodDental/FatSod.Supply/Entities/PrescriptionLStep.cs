using FatSod.Security.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FatSod.Supply.Entities
{
    [Serializable]
    public class PrescriptionLStep
    {
        public int PrescriptionLStepID { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]

        public DateTime DateHeurePrescriptionLStep { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime DatePrescriptionLStep { get; set; }

        public int ConsultationID { get; set; }
        [ForeignKey("ConsultationID")]
        public virtual Consultation Consultation { get; set; }
        
        public int? ConsultByID { get; set; }
        [ForeignKey("ConsultByID")]
        public virtual User ConsultBy { get; set; }
        
        public string Remarque { get; set; }
        public string MedecinTraitant { get; set; }

        public DateTime DateRdv { get; set; }
        public bool PrescriptionCollyre { get; set; }
        public string CollyreName { get; set; }

       
    }
    
}
