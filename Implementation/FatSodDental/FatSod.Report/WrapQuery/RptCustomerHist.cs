using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FatSod.Report.WrapQuery
{
    [Serializable]
    public class RptCustomerHist
    {
        public int RptCustomerHistID { get; set; }
        [StringLength(100)]
        public string Agence { get; set; }
        [StringLength(100)]
        public string LibAgence { get; set; }
        [StringLength(10)]
        public string Devise { get; set; }
        [StringLength(100)]
        public string LibDevise { get; set; }
        public int? SaleID { get; set; }        
        [StringLength(100)]
        public string AcctNo { get; set; }
        [StringLength(100)]
        public string AccountName { get; set; }
        public double MtDebit { get; set; }
        public double MtCredit { get; set; }
        [StringLength(100)]
        public string RefOperation { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime DateOperation { get; set; }
        public string Description { get; set; }
    }
}
