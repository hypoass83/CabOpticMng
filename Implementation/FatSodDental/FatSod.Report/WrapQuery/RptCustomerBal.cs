using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FatSod.Report.WrapQuery
{
    [Serializable]
    public class RptCustomerBal
    {
        public int RptCustomerBalID { get; set; }
        public int BranchID { get; set; }
        public int DeviseID { get; set; }
        public int AccountID { get; set; }
        [StringLength(100)]
        public string UIBranchCode { get; set; }
        [StringLength(100)]
        public string UIDeviseCode { get; set; }
        [StringLength(100)]
        public string UIAccountNumber { get; set; }
        [StringLength(100)]
        public string AccountName { get; set; }
        public double Debit { get; set; }
        public double Credit { get; set; }
        public double Solde { get; set; }
        public int CustomerID { get; set; }
        [StringLength(30)]
        public string Telephone { get; set; }
    }
}
