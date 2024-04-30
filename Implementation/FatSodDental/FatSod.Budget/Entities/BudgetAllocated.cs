using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using FatSod.Security.Entities;

namespace FatSod.Budget.Entities
{
    [Serializable]
    public class BudgetAllocated
    {
        public int BudgetAllocatedID { get; set; }
        //cle etrangere vers Agence
        public int BranchID { get; set; }
        [ForeignKey("BranchID")]
        public virtual Branch Branch { get; set; }

        //cle etrangere vers FiscalYear
        public int FiscalYearID { get; set; } 
        [ForeignKey("FiscalYearID")]
        public virtual FiscalYear FiscalYear { get; set; }

        //cle etrangere vers BudgetLine
        public int BudgetLineID { get; set; } 
        [ForeignKey("BudgetLineID")]
        public virtual BudgetLine BudgetLine { get; set; }
        public double AllocateAmount { get; set; }
        //public string BudgetAllocatedLabel { get; set; }
        public virtual ICollection<BudgetAllocatedUpdate> BudgetAllocatedUpdates { get; set; }
    }
}
