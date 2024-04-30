using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FatSod.Budget.Entities
{
    [Serializable]
    public class FiscalYear
    {
        public int FiscalYearID { get; set; }

        [Index("FiscalYearNumber", IsUnique = true, IsClustered = false)]
        public int FiscalYearNumber { get; set; }
        [Required]
        [Index("FiscalYearStatus", IsUnique = true, IsClustered = false)]
        public bool FiscalYearStatus { get; set; }
        public string FiscalYearLabel { get; set; }
        public DateTime StartFrom { get; set; }
        public DateTime EndFrom { get; set; }
        public virtual ICollection<BudgetAllocated> BudgetAllocateds { get; set; }
    }
}
