using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FatSod.Report.WrapReports
{
    [Serializable]
    public class RptInventory
    {
        public int ID { get; set; }
        public string BranchName { get; set; }
        public string BranchAdress { get; set; }
        public string BranchTel { get; set; }
        public string CompanyName { get; set; }
        public string CompanyAdress { get; set; }
        public string CompanyTel { get; set; }
        public string CompanyCNI { get; set; }
        public byte[] CompanyLogo { get; set; }
        public string ProductLabel { get; set; }
        public double ProductQty { get; set; }
        public double ProductUnitPrice { get; set; }
        public string Localization { get; set; }
    }
}
