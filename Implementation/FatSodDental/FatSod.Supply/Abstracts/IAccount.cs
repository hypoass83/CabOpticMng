using FatSod.Supply.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FatSod.Supply.Abstracts
{
    public interface IAccount : IRepositorySupply<Account>
    {
        /// <summary>
        /// methode permettant de generer et creer automatiquement un numero de compte
        /// </summary>
        /// <param name="eGL"></param>
        /// <returns></returns>
        Account GenerateAccountNumber(int collectiveAcountID, string AccountLabel, bool isManualPosting);
        /// <summary>
        /// retoune un numero de cpte a partir d'un cpte collectif
        /// </summary>
        /// <param name="collectiveAcountID"></param>
        /// <returns></returns>
        int AfficheAccountNumber(int collectiveAcountID);
        /// <summary>
        /// Permet de générer le numéro de compte 
        /// </summary>
        /// <param name="AccountingSectionCode">Champitre Comptable du compte</param>
        /// <param name="CollectifAccountLabel">Libélé du Collectif compte à créer si neccesaire</param>
        /// <param name="AccountLabel">Libéllé du compte à crééer si necessaire</param>
        /// <returns></returns>
        Account GenerateAccountNumber(string AccountingSectionCode, string CollectifAccountLabel, string AccountLabel, bool isManualPosting);

    }
}
