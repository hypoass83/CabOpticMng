using FatSod.Security.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FatSod.Supply.Entities
{
    [Serializable]
    public class AssureurPM : PaymentMethod
    {
        public int AssureurID { get; set; }
        [ForeignKey("AssureurID")]
        public virtual Assureur Assureur { get; set; }
        public virtual ICollection<AssureurSale> AssureurSales { get; set; }
    }
}
