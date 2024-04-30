using System;

namespace FatSod.Report.WrapReports
{
    [Serializable]
    public class PaymentDetail
    {
        public int PaymentDetailID { get; set; }
        public double PaymentDetailAmount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string Reference { get; set; }
    }
}
