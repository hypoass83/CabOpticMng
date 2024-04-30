using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using FatSod.Supply.Entities;

namespace FatSod.Budget.Entities
{
    [Serializable]
    public class BudgetLine
    {
        public int BudgetLineID { get; set; }
        [StringLength(10)]
        [Index("BudgetCode", IsUnique = true, IsClustered = false)]
        public string BudgetCode { get; set; }
        public string BudgetLineLabel { get; set; } 
        [StringLength(10)]
        public string BudgetType { get; set; } 
        public bool BudgetControl { get; set; }
        //cle etrangere vers Account
        public int AccountID { get; set; }
        [ForeignKey("AccountID")]
        public virtual Account Account { get; set; }
        public virtual ICollection<BudgetAllocated> BudgetAllocateds { get; set; }
    }
}
