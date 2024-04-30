using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FatSod.Supply.Entities
{
    public class BarCodePayload
    {
        public string CategoryCode { get; set; }
        public string ProductCode { get; set; }
        public string Marque { get; set; }
        public string NumeroSerie { get; set; }
        public int ProductId { get; set; }
        public int LocationId { get; set; }
        public int ProductLocalisationId { get; set; }
        public string BarCode { get; set; }
        public string BarCodeImage { get; set; }
        public int Quantity { get; set; }
        public double ProductPrice { get; set; }
    }
}
