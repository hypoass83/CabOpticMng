using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using FatSod.Supply.Entities;
using FatSod.Security.Entities;

namespace FatSod.Budget.Entities
{
    [Serializable]
    public class BudgetConsumption
    {
        public int BudgetConsumptionID { get; set; }
        //cle etrangere vers BudgetLine
        public int BudgetAllocatedID { get; set; }
        [ForeignKey("BudgetAllocatedID")]
        public virtual BudgetAllocated BudgetAllocated { get; set; }
        [NotMapped]
        public string UIBudgetAllocated
        {
            get
            {
                if (BudgetAllocated != null)
                {
                    return this.BudgetAllocated.BudgetLine.BudgetLineLabel.ToString();
                }
                else return "";
            }
            //set { this.UIBudgetAllocated = value; }
        }

        public int? PaymentMethodID { get; set; }
        [ForeignKey("PaymentMethodID")]
        public virtual PaymentMethod PaymentMethod { get; set; }
        public double VoucherAmount { get; set; }
        public DateTime DateOperation { get; set; }
        public DateTime? PaymentDate { get; set; }
        public string Reference { get; set; }
        public bool isValidated { get; set; }
        public string BeneficiaryName { get; set; }
        public string Justification { get; set; }

        //cle etrangere vers Devise
        public int? DeviseID { get; set; }
        [ForeignKey("DeviseID")]
        public virtual Devise Devise { get; set; }

        public int? AutorizeByID { get; set; }
        [ForeignKey("AutorizeByID")]
        public virtual User AutorizeBy { get; set; }

        public int? ValidateByID { get; set; }
        [ForeignKey("ValidateByID")]
        public virtual User ValidateBy { get; set; }
        [NotMapped]
        public double AmountAllocated { get; set; }
        [NotMapped]
        public double AmountSpend { get; set; }
        [NotMapped]
        public double AmountLeft { get; set; }
        [NotMapped]
        public double RemainingBalance { get; set; }

    }
}
