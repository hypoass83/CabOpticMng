using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FatSod.Supply.Entities
{
    [Table("vcumulRealSales")]
    public class vcumulRealSales
    {
        [Key]
        public Guid Id { get; set; }
        public int SaleID { get; set; }
        public DateTime SaleDate { get; set; }
        public int CustomerID { get; set; }
        public string Name { get; set; }
        public string SaleReceiptNumber { get; set; }
        public DateTime? SaleDeliveryDate { get; set; }
        public string Remarque { get; set; }
        public string MedecinTraitant { get; set; }
        public double SaleTotalPrice { get; set; }
        public double Advanced { get; set; }
    }
}
