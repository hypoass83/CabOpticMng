using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FatSod.Report.WrapReports
{
    [Serializable]
    public class RptCustomerPayment
    {
        public int RptCustomerPaymentID { get; set; }
        public string Date { get; set; }
        public string Type { get; set; }
        public string RptTitle { get; set; }
        public double Solde { get; set; }
        public double TillOpeningAmoung { get; set; }
        public double InputAmount { get; set; }
        public double OutPutAmount { get; set; }
        public string Teller { get; set; }
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
    }
}
