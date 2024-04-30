using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FatSod.Report.WrapReports
{
    [Serializable]
    public class RptInventoryDirectory
    {
        public int RptInventoryDirectoryID { get; set; }
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
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime TransfertDate { get; set; }
        public string ProductLabel { get; set; }
        public string ProductRef { get; set; }
        public double AveragePurchasePrice { get; set; }
        public double NewStockQuantity { get; set; }

        public double OldStockQuantity { get; set; }
        public double StockDifference { get; set; }

        //=========
        public double ReceiveAmount { get; set; }
        public double TotalAmount { get; set; }
        public string Operator { get; set; }
        public string DeviseLabel { get; set; }
        public double Transport { get; set; }

    }
}
