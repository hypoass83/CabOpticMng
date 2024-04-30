using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CABOPMANAGEMENT.Areas.Sale.Models
{
    public class ModelSaleLine
    {
        public int SaleLineID { get; set; }
        public string ProductLabel { get; set; }
        public double LineUnitPrice { get; set; }
        public double LineQuantity { get; set; }
        public double LineAmount { get; set; }
        public double QtyToReturn { get; set; }
        public string Reason { get; set; }
    }
}