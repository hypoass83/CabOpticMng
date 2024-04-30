using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FatSod.Budget.Entities
{
    [Serializable]
    public class BudgetAllocatedUpdate
    {
        public int BudgetAllocatedUpdateID { get; set; }
        //cle etrangere vers BudgetAllocated
        public int BudgetAllocatedID { get; set; }
        [ForeignKey("BudgetAllocatedID")] 
        public virtual BudgetAllocated BudgetAllocated { get; set; }
        public string SensImputation { get; set; } 
        public string Justification { get; set; }
        public double Amount { get; set; }
    }
}
