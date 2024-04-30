using FatSod.Security.Entities;
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
    public class HistoSMS
    {
        public int HistoSMSID { get; set; }
        public int NbreSMS { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime DateEnvoi { get; set; }
        public string SmsEnvoye { get; set; }
        public string TypeSms { get; set; }
        public int ? CashCustomerID { get; set; }
        [ForeignKey("CashCustomerID")]
        public virtual Customer Customer { get; set; }
        public int? InsuredCustomerID { get; set; }
        [ForeignKey("InsuredCustomerID")]
        public virtual CustomerOrder CustomerOrder { get; set; }
        public int OperatorID { get; set; }
        [ForeignKey("OperatorID")]
        public virtual User Operator { get; set; }
    }

}
