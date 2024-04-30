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
    public class ATCDPersonnel 
    {
        public int ATCDPersonnelID { get; set; }

        public int ATCDID { get; set; }
        [ForeignKey("ATCDID")]
        public virtual ATCD ATCD { get; set; }

        public int CustomerID { get; set; }
        [ForeignKey("CustomerID")]
        public virtual Customer Customer { get; set; }
        public string Remarques { get; set; }
    }
}
