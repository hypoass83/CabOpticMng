using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FatSod.Report.WrapReports
{
    [Serializable]
    public class RptCashOpHist
    {
        public int RptCashOpHistID { get; set; }
        public DateTime OperationDate { get; set; }
        public DateTime BeginDate { get; set; }
        public DateTime EndDate { get; set; }
        public string RptTitle { get; set; }
        public double Solde { get; set; }
        public double GroupingDate { get; set; }
        public double InputAmount { get; set; }
        public double RealOperationAmount { get; set; }
        public double OutPutAmount { get; set; }
        public double OpeningCashAmount { get; set; }
        public double ClosingCashAmount { get; set; }
        public string Teller { get; set; }
        [StringLength(100)]
        [Required]
        public string TransactionNumber { get; set; }
        public string CashRegisterName { get; set; }
        public string Intervenant { get; set; }
        public string Operation { get; set; }
        public string BranchName { get; set; }
        public string BranchAdress { get; set; }
        public string BranchTel { get; set; }
        public string CompanyName { get; set; }
        public string CompanyAdress { get; set; }
        public string CompanyTel { get; set; }
        public string CompanyCNI { get; set; }
        public byte[] CompanyLogo { get; set; }
        public int PaymentMethod { get; set; }
    }
}
