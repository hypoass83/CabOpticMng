using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CABOPMANAGEMENT.Areas.CashRegister.Models
{
    public class BorderoDepotModel
    {
        public int ID { get; set; }
        public string InsuranceName { get; set; }
        public string CustomerName { get; set; }
        public string CompanyName { get; set; }
        
        public string CustomerOrderDate { get; set; }
        public string NumeroFacture { get; set; }

        public string PoliceAssurance { get; set; }
        public double MntAssureur { get; set; }
        
        public bool isSelectdRow { get; set; }
    }
}