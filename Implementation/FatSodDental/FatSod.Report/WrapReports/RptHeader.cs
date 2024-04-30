using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FatSod.Report.WrapReports
{
    [Serializable]
    public class RptHeader
    {
        public int RptHeaderID { get; set; }
        //company info
        public string CompanySigle { get; set; }
        public string CompanyTradeRegister { get; set; }
        public string CompanySlogan { get; set; }
        public string CompanyName { get; set; }
        public string CompanyAdress { get; set; } //POBox + Town
        public string CompanyTel { get; set; }
        public string CompanyFax { get; set; }
        public string CompanyEmail { get; set; }
        public string CompanyWebSite { get; set; }
        public byte[] CompanyLogo { get; set; }
        //Branch info
        public string BranchName { get; set; }
        public string BranchCode { get; set; }
        public string BranchAdress { get; set; }   //POBox + Town
        public string BranchTel { get; set; }
        public string BranchFax { get; set; }
        public string BranchEmail { get; set; }
    }
}
