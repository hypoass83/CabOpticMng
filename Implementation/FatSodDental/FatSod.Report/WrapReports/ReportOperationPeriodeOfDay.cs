using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FatSod.Report.WrapReports
{
    [Serializable]
    public class ReportOperationPeriodeOfDay
    {
        public int ReportOperationPeriodeOfDayID { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime OperationDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime BeginDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime EndDate { get; set; }
        public double InputAmount { get; set; }
        public double OutPutAmount { get; set; }
        public string Intervenant { get; set; }
        public double Solde { get; set; }
        public string Operation { get; set; }
        [StringLength(100)]
        [Required]
        public string TransactionNumber { get; set; }
        public string Description { get; set; }
        public double OpeningCashAmount { get; set; }
        public double ClosingCashAmount { get; set; }
        public string PaymentMethod { get; set; }
        public string CashRegisterName { get; set; }

        public double? CashHandCloseDay { get; set; }
    }
}
