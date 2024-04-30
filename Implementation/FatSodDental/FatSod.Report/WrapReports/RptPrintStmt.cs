using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FatSod.Report.WrapReports
{
    [Serializable]
    public class RptPrintStmt
    {
        public int Order { get; set; }
        public int RptPrintStmtID { get; set; }
        [StringLength(50)]
        public string Agence { get; set; }
        [StringLength(100)]
        public string LibAgence { get; set; }
        [StringLength(10)]
        public string AcctNo { get; set; }
        [StringLength(100)]
        public string AcctName { get; set; }
        [StringLength(3)]
        public string Devise { get; set; }
        [StringLength(100)]
        public string LibDevise { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime BeginDate { get; set; }
        public string DateOperation { get; set; }
        public string Operator { get; set; }
        [StringLength(30)]
        public string RefOperation { get; set; }
        [StringLength(100)]
        public string Description { get; set; }
        public double RepDebit { get; set; }
        public double RepCredit { get; set; }
        public double Solde { get; set; }
        public double MtDebit { get; set; }
        public double MtCredit { get; set; }
        [StringLength(3)]
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
        public int? SaleID { get; set; }
        public string Remarque { get; set; }
        public string PaymentReason { get; set; }
        // Pour savoir si la prescription a ete achetee ou pas; important pour le dossier patient
        public string HasBeenPurchased { get; set; } = "RAS";
        public DateTime dateOperationHours { get; set; }
    }
}
