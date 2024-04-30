using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FatSod.Supply.Entities
{
    [Serializable]
    public class AVLTS 
    {
        public int AVLTSID { get; set; }
        [Required]
        public string Name { get; set; }
    }
}
