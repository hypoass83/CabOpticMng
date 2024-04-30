using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CABOPMANAGEMENT.Areas.Supply.Models
{
    public class InventoryLines
    {
        public int ProductLocalizationID { get; set; }
        public double ProductLocalizationStockQuantity { get; set; }
        public double ProductLocalizationSafetyStockQuantity { get; set; }
        public double AddedQuantity { get; set; }
    }
}