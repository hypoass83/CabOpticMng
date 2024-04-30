using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FatSod.Supply.Entities
{
    [Serializable]
    public class ProductLocalizationAccountOperation : AccountOperation
    {
        //cle etranngere vers ProductLocalization
        public int ProductLocalizationID { get; set; }
        [ForeignKey("ProductLocalizationID")]
        public virtual ProductLocalization ProductLocalization { get; set; }
    }
}
