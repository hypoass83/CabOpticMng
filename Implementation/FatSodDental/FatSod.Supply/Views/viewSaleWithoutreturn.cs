using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FatSod.Supply.Entities
{
    [Table("viewSaleWithoutreturn")]
    public class viewSaleWithoutreturn
    {
        [Key]
        public Guid Id { get; set; }
        public int SaleID { get; set; }
        public DateTime SaleDate { get; set; }
        public string SaleReceiptNumber { get; set; }
        public double saleLineQty { get; set; }
        public int ProductID { get; set; }
        public int LocalizationID { get; set; }
        public double LineUnitPrice { get; set; }
        public double UniteDeCalcul { get; set; }
        public int retlineqty { get; set; }
        public double retTransport { get; set; }
        public double Transport { get; set; }
        public double SaleQty { get; set; }
        public double RateDiscount { get; set; }
        public double RateReduction { get; set; }
        public double VatRate { get; set; }
        public int CustomerID { get; set; }
        public string Name { get; set; }
        public string ProductCode { get; set; }
        public string LocalizationCode { get; set; }
        public int BranchID { get; set; }
        public string BranchCode { get; set; }
        public string BranchDescription { get; set; }
        public int LineID { get; set; }
        public int StatutSale { get; set; }
        public int PostByID { get; set; }
        public int OperatorID { get; set; }
        public DateTime? SaleDeliveryDate { get; set; }
        public string Remarque { get; set; }
        public string MedecinTraitant { get; set; }
    }
}
