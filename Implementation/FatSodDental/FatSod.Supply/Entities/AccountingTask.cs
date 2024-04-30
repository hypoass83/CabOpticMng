using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace FatSod.Supply.Entities
{
    [Serializable]
    public class AccountingTask
    {
        public int AccountingTaskID { get; set; }
        public string AccountingTaskSens { get; set; }
        public string AccountingTaskDescription { get; set; }

        //cle etrangere vers operation
        public int OperationID { get; set; }
        [ForeignKey("OperationID")]
        public virtual Operation Operation { get; set; }
        [NotMapped]
        public string UIOperationCode
        {
            get; set;
            //get
            //{
            //    if (Operation != null) { return this.Operation.OperationCode.ToString(); }
            //    else return "";
            //}
            //set { this.UIOperationCode = value; }

        }

        //cle etrangere vers chapitre comptable
        public int AccountingSectionID { get; set; }
        [ForeignKey("AccountingSectionID")]
        public virtual AccountingSection AccountingSection { get; set; }

        public string ApplyVat { get; set; }

        public int? AccountID { get; set; }
        [ForeignKey("AccountID")]
        public virtual Account Account { get; set; }

        public int? VatAccountID { get; set; }
        [ForeignKey("VatAccountID")]
        public virtual Account AccountVAT { get; set; }
        [NotMapped]
        public string UIVatAccountNumber
        {
            get; set;
            //get
            //{
            //    if (AccountVAT != null) { return this.AccountVAT.AccountNumber.ToString(); }
            //    else return "";
            //}
            //set { UIVatAccountNumber = value; }
        }
        public int? DiscountAccountID { get; set; }
        [ForeignKey("DiscountAccountID")]
        public virtual Account AccountDiscount { get; set; }
        [NotMapped]
        public string UIDiscountAccountNumber
        {
            get; set;
            //get
            //{
            //    if (AccountDiscount != null) { return this.AccountDiscount.AccountNumber.ToString(); }
            //    else return "";
            //}
            //set { UIDiscountAccountNumber = value; }
        }
        public int? TransportAccountID { get; set; }
        [ForeignKey("TransportAccountID")]
        public virtual Account AccountTransport { get; set; }

        [NotMapped]
        public string UITransportAccountNumber
        {
            get; set;
            //get
            //{
            //    if (AccountTransport != null) { return this.AccountTransport.AccountNumber.ToString(); }
            //    else return "";
            //}
            //set { UITransportAccountNumber = value; }
        }

        
        
        [NotMapped]
        public string UIAccountingSectionNumber
        {
            get; set;
            //get
            //{
            //    if (AccountingSection != null) { return this.AccountingSection.AccountingSectionNumber.ToString(); }
            //    else return "";
            //}
            //set { UIAccountingSectionNumber = value; }
        }
        [NotMapped]
        public string UIAccountNumber
        {
            get; set;
            //get
            //{
            //    if (Account != null) { return this.Account.AccountNumber.ToString(); }
            //    else return "";
            //}
            //set { UIAccountNumber = value; }
        }
    }
}
