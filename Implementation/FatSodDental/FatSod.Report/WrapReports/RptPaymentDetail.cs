using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FatSod.Report.WrapReports
{
    [Serializable]
    public class RptPaymentDetail
    {
        public int RptPaymentDetailID { get; set; }
        [Required]
        [StringLength(100)]
        public string Reference { get; set; }
        public DateTime DepositDate { get; set; }
        public string Description { get; set; }
        public double LineUnitPrice { get; set; }
        public int RptReceiptPaymentDetailID { get; set; }
        

    }
}
