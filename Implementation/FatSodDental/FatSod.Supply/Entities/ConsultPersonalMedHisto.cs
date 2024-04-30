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
    public class ConsultPersonalMedHisto
    {
        public int ConsultPersonalMedHistoID { get; set; }

        public int ConsultationID { get; set; }
        [ForeignKey("ConsultationID")]
        public virtual Consultation Consultation { get; set; }
       
        public int ConsultByID { get; set; }
        [ForeignKey("ConsultByID")]
        public virtual User ConsultBy { get; set; }

        public DateTime DateConsultPersonalMedHisto { get; set; }
        public string HeureConsultPersonalMedHisto { get; set; }

        public int CustomerNumber { get; set; }

        //public int ATCDFamilialID { get; set; }
        //[ForeignKey("ATCDFamilialID")]
        //public virtual ATCDFamilial ATCDFamilial { get; set; }

        //public int ATCDPersonnelID { get; set; }
        //[ForeignKey("ATCDPersonnelID")]
        //public virtual ATCDPersonnel ATCDPersonnel { get; set; }

        [NotMapped]
        public string ATCDPersoAutre { get; set; }
        [NotMapped]
        public string ATCDFamAutre { get; set; }
    }
}
