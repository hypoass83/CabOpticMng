using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;


namespace FatSod.Supply.Entities
{
    [NotMapped]
    public class CalculSodeGenerale
    {
        public double TotalDebit { get; set; }
        public double TotalAdvanced { get; set; }
    }
}
