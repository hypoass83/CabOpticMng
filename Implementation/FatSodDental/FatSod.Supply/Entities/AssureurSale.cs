using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FatSod.Supply.Entities
{
    [Serializable]
    public class AssureurSale : Sale
    {
        public int AssureurPMID { get; set; }
        [ForeignKey("AssureurPMID")]
        public virtual AssureurPM AssureurPM { get; set; }
    }
}
