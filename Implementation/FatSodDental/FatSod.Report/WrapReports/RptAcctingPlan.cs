using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FatSod.Report.WrapReports
{
    [Serializable]
    public class RptAcctingPlan
    {
        public int RptAcctingPlanID { get; set; }

        [StringLength(10)]
        public string CompteCle { get; set; }
        [StringLength(100)]
        public string LibelleCpte { get; set; }
        public bool ManualPosting { get; set; }
        [StringLength(3)]
        public string Devise { get; set; }
    }
}
