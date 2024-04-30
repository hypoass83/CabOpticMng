using FatSod.Security.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FatSod.Supply.Entities
{
    [Serializable]
    //represente tous les depots dans le systeme
    public class AllDeposit
    {

        public int AllDepositID { get; set; }
        public double Amount { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime AllDepositDate { get; set; }
        public int? OperatorID { get; set; }
        [ForeignKey("OperatorID")]
        public virtual User Operator { get; set; }
        public int PaymentMethodID { get; set; }
        [ForeignKey("PaymentMethodID")]
        public virtual PaymentMethod PaymentMethod { get; set; }

        #region DIGITAL PAYMENT
        public int? DigitalAccountManagerId { get; set; }
        [ForeignKey("DigitalAccountManagerId")]
        public virtual User DigitalAccountManager { get; set; }
        public string TransactionIdentifier { get; set; }
        #endregion
        public int DeviseID { get; set; }
        [ForeignKey("DeviseID")]
        public virtual Devise Devise { get; set; }
        public int CustomerID { get; set; }
        [ForeignKey("CustomerID")]
        public virtual Customer Customer { get; set; }
        //C'est le nom de celui qui est venu avec l'argent
        public string Representant { get; set; }
        [Required]
        [StringLength(100)]
        [Index(IsUnique = true, IsClustered = false)]
        public string AllDepositReference { get; set; }
        public int BranchID { get; set; }
        [ForeignKey("BranchID")]
        public virtual Branch Branch { get; set; }
        public String AllDepositReason { get; set; }
        public bool IsSpecialOrder { get; set; }
        //ds le cas d'un depot pour assuré
        public int? CustomerOrderID { get; set; }
        [ForeignKey("CustomerOrderID")]
        public virtual CustomerOrder CustomerOrder { get; set; }
    }
}
