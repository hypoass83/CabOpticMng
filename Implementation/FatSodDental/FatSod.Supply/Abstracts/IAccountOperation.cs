using FatSod.Security.Entities;
using FatSod.Supply.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FatSod.Supply.Abstracts
{
    public interface IAccountOperation : IRepositorySupply<AccountOperation>
    {

        bool ecritureComptableFinal(object o);
        //bool ecritureComptableFinalValidateSpecialOrder(object o);
        /// <summary>
        /// methode d'ecriture automatique des operations comptable dans le grand livre
        /// </summary>
        /// <param name="eGL"></param>
        /// <returns></returns>
        //bool EcritureHistoGrandLivre(EcritureGLHist eGL, double AmountPaid);
        /// <summary>
        /// Ecriture d'une ligne d'ecriture (Debit ou Credit) dans le Grand livre
        /// </summary>
        /// <param name="accountOperation"></param>
        /// <param name="Montant"></param>
        /// <param name="operationTypeID"></param>
        /// <param name="sensop"></param>
        /// <returns></returns>
        bool ecritureLigneGL(AccountOperation accountOperation, double MontantPrincDB, double MontantPrincCR, int operationTypeID, string sensop,
            int idSalePurchage, double MontantTVA, int VatAccountID, double Discount, int DiscountAccountID, double Transport, int TransportAccountID, bool isReturn);
        /// <summary>
        /// Ecriture Manuelle Histo Grand Livre (ecriture d'une piece comptable ds le GL)
        /// </summary>
        /// <returns></returns>
        bool EcritureManuelleHistoGrandLivre(string CodeTransaction);
        /// <summary>
        /// ecriture histo grand livre pour operation manuelle
        /// </summary>
        /// <param name="accountOperation"></param>
        /// <param name="Montant"></param>
        /// <param name="operationTypeID"></param>
        /// <param name="sensop"></param>
        /// <param name="PieceID"></param>
        /// <returns></returns>
        bool ecritureManualLigneGL(AccountOperation accountOperation, double Montant, int operationTypeID, string sensop, long PieceID);
        
    }
}
