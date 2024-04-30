using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FatSod.Report.WrapReports
{
    [Serializable]
    public class RptSpecialOrder
		{
        public int RptSpecialOrderID { get; set; }
		[StringLength(50)]
		public string Agence { get; set; }
		[StringLength(100)]
		public string LibAgence { get; set; }
		[StringLength(3)]
		public string Devise { get; set; }
		[StringLength(100)]
		public string LibDevise { get; set; }
        public DateTime CustomerOrderDate { get; set; }
        public double  CustomerOrderTotalPrice { get; set; }
        public string CustomerOrderNumber { get; set; }
        public string CustomerName { get; set; }
        [StringLength(100)]
        [Required]
        public string CodeClient { get; set; }
        public string NomClient { get; set; }
        public string OrderStatut { get; set; }
        public int Code { get; set; }
        public DateTime? ValidatedDate { get; set; }
        public DateTime? DeliveredDate { get; set; }
        public DateTime? PostedToSupplierDate { get; set; }
        public DateTime? SaleDate { get; set; }
        public DateTime? ReceivedDate { get; set; }
        public double AdvancedAmount { get; set; }
        public int ProductID { get; set; }
        public string ProductCode { get; set; }
        public double Balance { get; set; }
		}
	}
