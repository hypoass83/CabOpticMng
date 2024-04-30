using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FatSod.Report.WrapReports
{
    [Serializable]
    public class RptBill
    {
        public int RptBillID { get; set; }
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
        public byte[] CompanyLogo { get; set; }
        public string BranchAbbreviation { get; set; }
        //End header=============================
        public DateTime SaleDate { get; set; }
        public double RateRedution { get; set; }
        public double RateDiscount { get; set; }
				public double Redution { get; set; }
				public double Discount { get; set; }
        public double Transport { get; set; }
        public string ProductLabel { get; set; }
        public string ProductRef { get; set; }
        public double LineUnitPrice { get; set; }
        public double LineQuantity { get; set; }
        public double ReceiveAmount { get; set; }
        public int SaleID { get; set; }
        //===== Customer identification
        public string CustomerName { get; set; }
        public string CustomerAdress { get; set; }
        public string CustomerAccount { get; set; }
        public double VatRate { get; set; }
        public double TotalRemainingUnpaid { get; set; }
				public string CompanyEmail { get; set; }
				public string CompanyTradeRegister { get; set; }
				public double DepositAmount { get; set; }
				public string CompanyTown { get; set; }
				public string BillNumber { get; set; }

        public virtual ICollection<ReceiptLine> ReceiptLines { get; set; }
        public double Balance { get; set; }
        public double ReductionAmount { get; set; }
        public double DiscountAmount { get; set; }
        public double TotalAmountTTC { get; set; }
        public double TotalAvance { get; set; }

        public double BalanceBefore { get; set; }
        public double PeriodDeposit { get; set; }
        public double PeriodConsumption { get; set; }
        public double AmountToPaid { get; set; }
        public string NumeroCde { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime DateCde { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime ServerDate { get; set; }
        public DateTime BeginDate { get; set; }
        public DateTime EndDate { get; set; }
        public double SaleAmount { get; set; }
    }
}

