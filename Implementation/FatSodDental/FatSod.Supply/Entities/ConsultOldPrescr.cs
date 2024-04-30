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
    public class ConsultOldPrescr
    {
        public int ConsultOldPrescrID { get; set; }

        public int ConsultationID { get; set; }
        [ForeignKey("ConsultationID")]
        public virtual Consultation Consultation { get; set; }
       
        //info pr le verre
        //left site
        public string LAxis { get; set; }
        public string LAddition { get; set; }
        public string LIndex { get; set; }
        public string LCylValue { get; set; }
        public string LSphValue { get; set; }

        //right site
        public string RAxis { get; set; }
        public string RAddition { get; set; }
        public string RIndex { get; set; }
        public string RCylValue { get; set; }
        public string RSphValue { get; set; }

        //product category
        public int? CategoryID { get; set; }
        [ForeignKey("CategoryID")]
        public virtual Category Category { get; set; }

        public DateTime DateDernierConsultation { get; set; }
        public bool IsDilatation { get; set; }
        public bool IsCollyre { get; set; }
        public string NomCollyre { get; set; }
        public string PlaintePatient { get; set; }
        public string OldPlaintePatient { get; set; }

        public int ConsultByID { get; set; }
        [ForeignKey("ConsultByID")]
        public virtual User ConsultBy { get; set; }

        public DateTime DateConsultOldPres { get; set; }
        public string HeureConsOldPres { get; set; }

        public int CustomerNumber { get; set; }

        public int? AcuiteVisuelLID { get; set; }
        [ForeignKey("AcuiteVisuelLID")]
        public virtual AcuiteVisuelL AcuiteVisuelL { get; set; }

        public int? RAcuiteVisuelLID { get; set; }
        [ForeignKey("RAcuiteVisuelLID")]
        public virtual AcuiteVisuelL RAcuiteVisuelL { get; set; }

        public int? LAcuiteVisuelLID { get; set; }
        [ForeignKey("LAcuiteVisuelLID")]
        public virtual AcuiteVisuelL LAcuiteVisuelL { get; set; }

        public int? RAVLTSID { get; set; }
        [ForeignKey("RAVLTSID")]
        public virtual AVLTS RAVLTS { get; set; }

        public int? LAVLTSID { get; set; }
        [ForeignKey("LAVLTSID")]
        public virtual AVLTS LAVLTS { get; set; }

        public int? AcuiteVisuelPID { get; set; }
        [ForeignKey("AcuiteVisuelPID")]
        public virtual AcuiteVisuelP AcuiteVisuelP { get; set; }


        public int? RAcuiteVisuelPID { get; set; }
        [ForeignKey("RAcuiteVisuelPID")]
        public virtual AcuiteVisuelP RAcuiteVisuelP { get; set; }

        public int? LAcuiteVisuelPID { get; set; }
        [ForeignKey("LAcuiteVisuelPID")]
        public virtual AcuiteVisuelP LAcuiteVisuelP { get; set; }

        public int? OldAcuiteVisuelLID { get; set; }
        [ForeignKey("OldAcuiteVisuelLID")]
        public virtual AcuiteVisuelL OldAcuiteVisuelL { get; set; }

        public int? OldRAcuiteVisuelLID { get; set; }
        [ForeignKey("OldRAcuiteVisuelLID")]
        public virtual AcuiteVisuelL OldRAcuiteVisuelL { get; set; }

        public int? OldLAcuiteVisuelLID { get; set; }
        [ForeignKey("OldLAcuiteVisuelLID")]
        public virtual AcuiteVisuelL OldLAcuiteVisuelL { get; set; }

        //public int? OldRAVLTSID { get; set; }
        //[ForeignKey("OldRAVLTSID")]
        //public virtual AVLTS OldRAVLTS { get; set; }

        //public int? OldLAVLTSID { get; set; }
        //[ForeignKey("OldLAVLTSID")]
        //public virtual AVLTS OldLAVLTS { get; set; }

        public int? OldAcuiteVisuelPID { get; set; }
        [ForeignKey("OldAcuiteVisuelPID")]
        public virtual AcuiteVisuelP OldAcuiteVisuelP { get; set; }

        public int? OldRAcuiteVisuelPID { get; set; }
        [ForeignKey("OldRAcuiteVisuelPID")]
        public virtual AcuiteVisuelP OldRAcuiteVisuelP { get; set; }

        public int? OldLAcuiteVisuelPID { get; set; }
        [ForeignKey("OldLAcuiteVisuelPID")]
        public virtual AcuiteVisuelP OldLAcuiteVisuelP { get; set; }


        [NotMapped]
        public DateTime DateOfBirth { get; set; }
    }
}
