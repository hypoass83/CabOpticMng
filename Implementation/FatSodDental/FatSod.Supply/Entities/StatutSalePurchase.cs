using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FatSod.Supply.Entities
{
    /// <summary>
    /// Cette énumération sera utilisée pour savoir à quel stade est une vente ou un achat. Il permet principalement de faire la compta.
    /// </summary>
   public  enum SalePurchaseStatut
    {
       Ordered,        //La commande a été passée(vente et achat)
       Delivered,     //les marchandises ont été livrées au client(vente)
       Received,     //les marchandises ont été reçues du fournisseur(achat)
       Returned,    //Une vente ou un achat a été totalement retourné (vente et achat)
       Invoiced,   //la facture a été envoyée au client(Vente) ou reçu du fournisseur(achat) (vente et achat)
       Advanced,  //une avance à été faite sur une vente ou un achat(vente et achat)
       Paid,     //Une vente ou un achat a été totalement réglé(vente et achat)
    }
}
