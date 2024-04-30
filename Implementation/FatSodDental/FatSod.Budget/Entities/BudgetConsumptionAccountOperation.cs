using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using FatSod.Supply.Entities;

namespace FatSod.Budget.Entities
{
    [Serializable]
    public class BudgetConsumptionAccountOperation : AccountOperation
    {
        //cle etranngere vers BudgetConsumption
        public int BudgetConsumptionID { get; set; }
        [ForeignKey("BudgetConsumptionID")]
        public virtual BudgetConsumption BudgetConsumption { get; set; }
    }
}
