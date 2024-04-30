using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FatSod.Report.WrapReports
{
    [Serializable]
    public class RptBalanceGenerale
    {
        public int RptBalanceGeneraleID { get; set; }
        [StringLength(50)]
        public string Agence { get; set; }
        [StringLength(100)]
        public string LibAgence { get; set; }
        [StringLength(5)]
        public string Devise { get; set; }
        [StringLength(100)]
        public string LibDevise { get; set; }
        [StringLength(50)]
        public string Compte { get; set; }
        [StringLength(100)]
        public string Libelle { get; set; }
        public double SoldeInitDb { get; set; }
        public double SoldeInitCr { get; set; }
        public double DebitMvt { get; set; }
        public double CreditMvt { get; set; }
        public double DebitCum { get; set; }
        public double CreditCum { get; set; }
        public double SoldeFinDb { get; set; }
        public double SoldeFinCr { get; set; }
    }
}
