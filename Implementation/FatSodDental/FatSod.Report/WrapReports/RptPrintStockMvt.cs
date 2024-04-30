using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FatSod.Report.WrapReports
{
    [Serializable]
    public class RptPrintStockMvt
    {
        public int RptPrintStockMvtID { get; set; }
        [StringLength(50)]
        public string Agence { get; set; }
        [StringLength(100)]
        public string LibAgence { get; set; }
        [StringLength(100)]
        public string LocalizationName { get; set; }
        [StringLength(500)]
        public string ProductName { get; set; }
        [StringLength(3)]
        public string Devise { get; set; }
        [StringLength(100)]
        public string LibDevise { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime BeginDate { get; set; }
        public DateTime DateOperation { get; set; }
        [StringLength(30)]
        public string RefOperation { get; set; }
        [StringLength(100)]
        public string Description { get; set; }
        public double RepOutPut { get; set; }
        public double RepInput { get; set; }
        public double Solde { get; set; }
        public double QteOutPut { get; set; }
        public double QteInput { get; set; }
        [StringLength(10)]
        public string Sens { get; set; }
        [StringLength(255)]
        public string CompanyName { get; set; }
        [StringLength(255)]
        public string RegionCountry { get; set; }
        [StringLength(30)]
        public string Telephone { get; set; }
        [StringLength(30)]
        public string Fax { get; set; }
        [StringLength(255)]
        public string Adresse { get; set; }
        public byte[] LogoBranch { get; set; }
        public int ProductID { get; set; }
        public int LocalizationID { get; set; }
    }
}
