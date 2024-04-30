using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FatSod.Report.WrapReports
{
    [Serializable]
    public class RptBareCode
    {
        public int RptBareCodeID { get; set; }

        [StringLength(10)]
        public string BareCode { get; set; }
        [StringLength(100)]
        public string ProductName { get; set; }
        [StringLength(100)]
        public string ProductDescription { get; set; }
        public double Price { get; set; }
        public byte[] BarcodeImage { get; set; }
    }
}
