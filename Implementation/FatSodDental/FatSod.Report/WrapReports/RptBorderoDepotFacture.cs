using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FatSod.Report.WrapReports
{
    [Serializable]
    public class RptBorderoDepotFacture
    {
        public int RptBorderoDepotFactureID { get; set; }
        public int CustomerOrderID { get; set; }
        public int BranchID { get; set; }
        [StringLength(100)]
        public string UIBranchCode { get; set; }
        [StringLength(100)]
        public string CustomerName { get; set; }
        [StringLength(100)]
        public string CompanyName { get; set; }
        [StringLength(50)]
        public string CustomerOrderNumber { get; set; }
        [StringLength(100)]
        public string NumeroBonPriseEnCharge { get; set; }
        public DateTime CustomerOrderDate { get; set; }
        [StringLength(30)]
        public string NumeroFacture { get; set; }
        [StringLength(30)]
        public string PhoneNumber { get; set; }
        public double MntAssureur { get; set; }
        public double ReductionAmount { get; set; }
        public byte[] LogoBranch { get; set; }
        [StringLength(100)]
        public string InsuranceCompany { get; set; }
        
    }
}
