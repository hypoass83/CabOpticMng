using System;
using System.ComponentModel.DataAnnotations;

namespace FatSod.Report.WrapReports
{
    //[Serializable]
    public class RptDigitalOperation
    {
        public int RptDigitalOperationID { get; set; }
        public string OperationDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime Date { get; set; }
        public string Operation { get; set; } // SALE ; DEPOSIT; BUDGET CONSUMPTION
        public string Type { get; set; } // INPUT ; OUTPUT
        public double Amount { get; set; }
        public string AccountName { get; set; }
        public string Operator { get; set; }
        public string AccountManager { get; set; }
        public string TransactionCode { get; set; }
        public string Reference { get; set; }
        public string Intervenant { get; set; }
        public string BranchName { get; set; }
    }

}
