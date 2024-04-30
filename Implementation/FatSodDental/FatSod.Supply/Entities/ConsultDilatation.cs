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
    public class ConsultDilatation 
    {
        public int ConsultDilatationID { get; set; }

        public int ConsultDilPrescID { get; set; }
        [ForeignKey("ConsultDilPrescID")]
        public virtual ConsultDilPresc ConsultDilPresc { get; set; }

        public string CodeDilation { get; set; }

        public int ConsultByID { get; set; }
        [ForeignKey("ConsultByID")]
        public virtual User ConsultBy { get; set; }

        public string HeureConsultDilatation { get; set; }
        public DateTime DateDilation { get; set; }

        public bool isAuthoriseSale { get; set; }
    }
}
