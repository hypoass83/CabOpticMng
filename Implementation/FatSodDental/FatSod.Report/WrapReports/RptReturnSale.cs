using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FatSod.Report.WrapReports
{
    [Serializable]
    public class RptReturnSale
    {
        public int RptReturnSaleID { get; set; }
        [StringLength(50)]
        public string Agence { get; set; }
        [StringLength(100)]
        public string LibAgence { get; set; }
        [StringLength(5)]
        public string Devise { get; set; }
        [StringLength(100)]
        public string LibDevise { get; set; }
        [StringLength(50)]
        public string CodeClient { get; set; }
        [StringLength(100)]
        public string NomClient { get; set; }
        public string CustomerReturnCauses { get; set; }
        public double LineQuantity { get; set; }
        public double LineAmount { get; set; }
        public double ReturnAmount { get; set; }
        public string LocalizationCode { get; set; }
        public string ProductCode { get; set; }
        public string OeilDroiteGauche { get; set; }
        public DateTime CustomerReturnDate { get; set; }
        public string AuthoriseBy { get; set; }
        public string ValidatedBy { get; set; }
    }
}
