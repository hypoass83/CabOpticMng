using FatSod.Security.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FatSod.Supply.Entities
{
    [Serializable]
    //represente les depots. seul les depots d'epargne seront persites
    public class Deposit
    {
        public int DepositID { get; set; }
        public double Amount { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime DepositDate { get; set; }
        public int PaymentMethodID { get; set; }
        [ForeignKey("PaymentMethodID")]
        public virtual PaymentMethod PaymentMethod1 { get; set; }
        public int DeviseID { get; set; }
        [ForeignKey("DeviseID")]
        public virtual Devise Devise { get; set; }
        public int SavingAccountID { get; set; }
        [ForeignKey("SavingAccountID")]
        public virtual SavingAccount SavingAccount { get; set; }
        //C'est le nom de celui qui est venu avec l'argent
        public string Representant { get; set; }
        public int? OperatorID { get; set; }
        [ForeignKey("OperatorID")]
        public virtual User Operator { get; set; }

        #region DIGITAL PAYMENT
        public int? DigitalAccountManagerId { get; set; }
        [ForeignKey("DigitalAccountManagerId")]
        public virtual User DigitalAccountManager { get; set; }
        public string TransactionIdentifier { get; set; }
        #endregion

        [Required]
        [StringLength(100)]
        [Index(IsUnique = true, IsClustered = false)]
        public string DepositReference { get; set; }
        public virtual ICollection<DepositAccountOperation> DepositAccountOperations { get; set; }
        //Champs non persistants
        [NotMapped]
        public int BranchID { get; set; }
        [NotMapped]
        public String DepositReason { get; set; }
        [NotMapped]
        public int CustomerID { get; set; }
        [NotMapped]
        public int SaleID { get; set; }
        [NotMapped]
        public double Debt { get; set; }
        [NotMapped]
        public double SavingAmount { get; set; }
        [NotMapped]
        public double Remainder { get; set; }
        [NotMapped]
        public string PaymentMethod { get; set; }
        [NotMapped]
        public SalePurchaseStatut StatutSale { get; set; }
        [NotMapped]
        public SalePurchaseStatut OldStatutSale { get; set; }
        [NotMapped]
        public int CustomerOrderID { get; set; }
    }
}
