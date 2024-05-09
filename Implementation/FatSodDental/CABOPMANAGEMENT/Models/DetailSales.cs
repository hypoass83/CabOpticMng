using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FatSod.Security.Entities;

namespace CABOPMANAGEMENT.Models
{
    public class DetailSales
    {
        public int LineNumber { get; set; }
        public string  Designation { get; set; }
        public string Qte { get; set; }
        public string PUHT { get; set; }
        public string PercRed { get; set; }
        public string MntHT { get; set; }
        
    }
}