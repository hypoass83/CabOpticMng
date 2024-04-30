using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FatSod.Report.WrapReports
{
    [Serializable]
    public class RptTransfertForm
    {
        public int RptTransfertFormID { get; set; }
        public string Ref { get; set; }
        //Header
        public string Title { get; set; }
        public string BranchName { get; set; }
        public string BranchAdress { get; set; }
        public string BranchTel { get; set; }
        public string CompanyName { get; set; }
        public string CompanyAdress { get; set; }
        public string CompanyTel { get; set; }
        public string CompanyCNI { get; set; }
        public string BranchAbbreviation { get; set; }
        public byte[] CompanyLogo { get; set; }
        //End header=============================
        public DateTime TransfertDate { get; set; }
        public string ProductLabel { get; set; }
        public string ProductRef { get; set; }
        public double LineUnitPrice { get; set; }
        public double LineQuantity { get; set; }
        //===== Customer identification
        public string SendindBranchCode { get; set; }
        public string SendindBranchName { get; set; } 
        public string ReceivingBranchCode { get; set; }
        public string ReceivingBranchName { get; set; }
        //=========
        public double ReceiveAmount { get; set; }
        public double TotalAmount { get; set; }
        public string Operator { get; set; }
        public string DeviseLabel { get; set; }
        public double Transport { get; set; }

    }
}
