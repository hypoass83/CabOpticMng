using FatSod.Security.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FatSod.Supply.Entities
{
    [Serializable]
    public class AccountOperation
    {
        public long AccountOperationID { get; set; }

        //cle etrangere vers Agence
        public int BranchID { get; set; }
        [ForeignKey("BranchID")]
        public virtual Branch Branch { get; set; }
        
        //cle etrangere vers operation
        public int OperationID { get; set; }
        [ForeignKey("OperationID")]
        public virtual Operation Operation { get; set; }

        //cle etrangere vers cptecollectif
        public int AccountID { get; set; }
        [ForeignKey("AccountID")]
        public virtual Account Account { get; set; }

        //cle etrangere vers cptetier
        public int? AccountTierID { get; set; }
        [ForeignKey("AccountID")]
        public virtual Account AccountTier { get; set; }
        
        //cle etrangere vers devise
        public int DeviseID { get; set; }
        [ForeignKey("DeviseID")]
        public virtual Devise Devise { get; set; }

        [NotMapped]
        public string UIBranchCode 
        {
            get
            {
                if (Branch != null) { return this.Branch.BranchCode.ToString(); }
                else return "";
            }
        }
        [NotMapped]
        public string UIDeviseCode
        {
            get
            {
                if (Devise != null) { return this.Devise.DeviseCode.ToString(); }
                else return "";
            }
        }
        [NotMapped]
        public string UIOperationCode
        {
            get
            {
                if (Operation != null) { return this.Operation.OperationCode.ToString(); }
                else return "";
            }
        }
        [NotMapped]
        public string UIAccountNumber
        {
            get
            {
                if (Account != null) { return this.Account.AccountNumber.ToString(); }
                else return "";
            }
        }
        //attribut propre a acounting opereration
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime DateOperation { get; set; }
        public string Description { get; set; }
        public string Reference { get; set; }
        public string CodeTransaction { get; set; }
        public double Debit { get; set; }
        public double Credit { get; set; }
    }
}
