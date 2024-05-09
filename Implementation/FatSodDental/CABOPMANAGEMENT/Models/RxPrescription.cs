using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FatSod.Security.Entities;

namespace CABOPMANAGEMENT.Models
{
    public class RxPrescription
    {
        public int LineNumber { get; set; }
        public string  Field1 { get; set; }
        public string Field2 { get; set; }
        public string Oeil { get; set; }
        public string Sphere { get; set; }
        public string Cylindre { get; set; }
        public string Axe { get; set; }
        public string Add { get; set; }
    }
}