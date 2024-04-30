using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FatSod.Supply.Entities
{
    /// <summary>
    /// Cette énumération sera utilisée pour savoir à quel stade est une facture
    /// </summary>
    public enum StatutFacture
    {
       Proforma,    //proforma
       Validated,   //valide
       Advanced,  //avance
       Paid,     //paye
       Delete //supprimé
    }
}
