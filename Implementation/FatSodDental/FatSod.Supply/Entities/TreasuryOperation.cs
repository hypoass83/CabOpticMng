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
    public class TreasuryOperation
    {
        public int TreasuryOperationID { get; set; }
        public int BranchID { get; set; }
        [ForeignKey("BranchID")]
        public virtual Branch Branch { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime OperationDate { get; set; }
        public double ComputerPrice { get; set; }
        public string OperationRef { get; set; }
        public string OperationType { get; set; }
        public string Justification { get; set; }
        public double OperationAmount { get; set; }
        public int TillID { get; set; }
        [ForeignKey("TillID")]
        public virtual Till Till { get; set; }
        public int? TillDestID { get; set; }
        [ForeignKey("TillDestID")]
        public virtual Till TillDest { get; set; }
        //cle etrangere vers Devise
        public int DeviseID { get; set; }
        [ForeignKey("DeviseID")]
        public virtual Devise Devise { get; set; }
        public int? BankID { get; set; }
        [ForeignKey("BankID")]
        public virtual Bank Bank { get; set; }
        public virtual ICollection<TreasuryOperationAccountOperation> TreasuryOperationAccountOperations { get; set; }
    }
}
