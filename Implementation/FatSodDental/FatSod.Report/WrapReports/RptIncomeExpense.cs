using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FatSod.Report.WrapReports
{
    [Serializable]
    public class RptIncomeExpense
    {
        public int RptIncomeExpenseID { get; set; }
        [StringLength(50)]
        public string Agence { get; set; }
        [StringLength(100)]
        public string LibAgence { get; set; }
        [StringLength(50)]
        public string Devise { get; set; }
        [StringLength(10)]
        public string LibDevise { get; set; }
        [StringLength(10)]
        public string AcctNumber { get; set; }
        [StringLength(100)]
        public string AcctName { get; set; }
        public double MonthTotal { get; set; }
        public double MonthCumul { get; set; }
        public double earningsmonth { get; set; }
        public double earningscumul { get; set; }
        [StringLength(25)]
        public string AccountType { get; set; }
    }
}
