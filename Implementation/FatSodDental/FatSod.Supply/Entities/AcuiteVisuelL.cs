using FatSod.Security.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FatSod.Supply.Entities
{
    [Serializable]
    public class AcuiteVisuelL
    {
        public int AcuiteVisuelLID { get; set; }
        [Required]
        public string Name { get; set; }
    }
}
