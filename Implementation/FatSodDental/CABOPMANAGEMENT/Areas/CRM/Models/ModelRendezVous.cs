using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CABOPMANAGEMENT.Areas.CRM.Models
{
    
    public class ModelRendezVous
    {
        public int RendezVousID { get; set; }
        
        public int CustomerID { get; set; }
        public string CustomerName { get; set; }
        public string Telephone { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime DateRdv { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime SaleDate { get; set; }
        public string RaisonRdv { get; set; }
        public int GestionnaireID { get; set; }

        public int SaleID { get; set; }
        public string SaleRef { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
        public DateTime DateOfBirth { get; set; }
    }
}