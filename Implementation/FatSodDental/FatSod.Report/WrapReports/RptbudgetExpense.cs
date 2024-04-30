using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FatSod.Report.WrapReports
{
    [Serializable]
    public class RptbudgetExpense
    {
        public int RptbudgetExpenseID { get; set; }
        [StringLength(50)]
        public string Agence { get; set; }
        [StringLength(100)]
        public string LibAgence { get; set; }
        public string Devise { get; set; }
        [StringLength(100)]
        public string LibDevise { get; set; }
        [StringLength(100)]
        public string UIBudgetAllocated { get; set; }
        public int PaymentMethodId { get; set; }
        public double VoucherAmount { get; set; }
        public DateTime DateOperation { get; set; }
        [StringLength(30)]
        public string Reference { get; set; }
        [StringLength(100)]
        public string BeneficiaryName { get; set; }
        [StringLength(100)]
        public string Justification { get; set; }
        public int BudgetConsumptionID { get; set; }
        public DateTime PaymentDate { get; set; }
        [StringLength(255)]
        public string CompanyName { get; set; }
        [StringLength(255)]
        public string RegionCountry { get; set; }
        [StringLength(30)]
        public string Telephone { get; set; }
        [StringLength(30)]
        public string Fax { get; set; }
        [StringLength(255)]
        public string Adresse { get; set; }
        public byte[] LogoBranch { get; set; }
    }
}
