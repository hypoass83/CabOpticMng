using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FatSod.Report.WrapReports
{
    [Serializable]
    public class RptInventoryEntry
    {
        public int RptInventoryEntryID { get; set; }
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
        //public string BranchAbbreviation { get; set; }
        public byte[] CompanyLogo { get; set; }
        //End header=============================
        public DateTime InventoryDirectoryDate { get; set; }
        public string ProductLabel { get; set; }
        public string ProductRef { get; set; }

        //=========
        public double StockDifference { get; set; }
        public double OldStockQuantity { get; set; }
        public double NewStockQuantity { get; set; }
        public double AveragePurchasePrice { get; set; }
        public string Operator { get; set; }
        public string DeviseLabel { get; set; }
        

    }
}
