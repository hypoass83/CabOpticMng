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
    public class ConsultLensPrescription 
    {
        public int ConsultLensPrescriptionID { get; set; }

        public int ConsultDilPrescID { get; set; }
        [ForeignKey("ConsultDilPrescID")]
        public virtual ConsultDilPresc ConsultDilPresc { get; set; }
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
        public int CategoryID { get; set; }
        [ForeignKey("CategoryID")]
        public virtual Category Category { get; set; }

        public string SupplyingName { get; set; }

        public DateTime DatePrescription { get; set; }

        public int ConsultByID { get; set; }
        [ForeignKey("ConsultByID")]
        public virtual User ConsultBy { get; set; }

        public string HeureLensPrescription { get; set; }

        public bool isAuthoriseSale { get; set; }
    }
}
