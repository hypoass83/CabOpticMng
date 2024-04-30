using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FatSod.Supply.Entities
{
    [Serializable]
    public class Bill
    {
        public int BillID { get; set; }
        [StringLength(100)]
        [Index(IsUnique = true)]
        public string BillNumber { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime BillDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime BeginDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime EndDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public int CustomerID { get; set; }
        [ForeignKey("CustomerID")]
        public virtual Customer Customer { get; set; }
        //valeur end of bill
        //public double TotalHT { get; set; }
        //public double TauxRemise { get; set; }
        public double MontantRemise { get; set; }
        //public double TauxEscompte { get; set; }
        public double MontantEscompte { get; set; }
        public double Transport { get; set; }
        public double TauxTva { get; set; }
        //public double ValeurTva { get; set; }
        public double BalanceBefore { get; set; }
        public double TotalDepot { get; set; }
        //public double NetApayer { get; set; }
        public virtual ICollection<BillDetail> BillDetails { get; set; }
        public bool IsNegoce { get; set; }
    }
}
