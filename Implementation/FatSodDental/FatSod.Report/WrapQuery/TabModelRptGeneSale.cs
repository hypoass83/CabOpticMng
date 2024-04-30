using System;
using System.ComponentModel.DataAnnotations;

namespace FatSod.Report.WrapQuery
{
    [Serializable]
    public class TabModelRptGeneSale
    {
        public int TabModelRptGeneSaleID { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime CustomerOrderDate { get; set; }
        public float CustomerOrderTotalPrice { get; set; }
        [StringLength(50)]
        public string CustomerOrderNumber { get; set; }
        [StringLength(200)]
        public string CustomerName { get; set; }
        [StringLength(100)]
        public string Agence { get; set; }
        [StringLength(100)]
        public string LibAgence { get; set; }
        [StringLength(100)]
        public string Devise { get; set; }
        [StringLength(100)]
        public string LibDevise { get; set; }
        public double totCustomerPrice { get; set; }
        public string StatutSale { get; set; }
        public int SaleID { get; set; }
        public float LineQuantity { get; set; }
        public float AdvancedAmount { get; set; }
        public int ProductID { get; set; }
        public string ProductCode { get; set; }
        public float Balance { get; set; }
        public string PostByID { get; set; }
        public string OperatorID { get; set; }
        [StringLength(100)]
        public string CodeClient { get; set; }
        public string NomClient { get; set; }
        public string OrderStatut { get; set; }
        public string IsInHouseCustomer { get; set; }

    }
}
