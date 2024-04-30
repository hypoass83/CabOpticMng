using FatSod.Supply.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FatSod.Supply.Abstracts
{
    public interface ICollectifAccount : IRepositorySupply<CollectifAccount>
    {
        /// <summary>
        /// methode permettant de generer et creer automatiquement un compte Collectif.
        /// </summary>
        /// <param name="AccountingSectionCode">Chapitre Comptable du CollectifAccount dont on souhaite obtenir le compte collectif</param>
        /// <param name="CollectifAccountLabel">Code du CollectifAccount dont on souhaite obtenir le compte collectif</param>
        /// <returns></returns>
        CollectifAccount GetCollectifAccount(string AccountingSectionCode, string CollectifAccountLabel);

        /// <summary>
        /// methode permettant de generer et creer automatiquement un compte Collectif.
        /// </summary>
        /// <param name="AccountingSectionCode">Chapitre Comptable du CollectifAccount dont on souhaite obtenir le compte collectif</param>
        /// <param name="CollectifAccountLabel">Code du CollectifAccount dont on souhaite obtenir le compte collectif</param>
        /// <returns></returns>
        int getCompteCollectifID(string AccountingSectionCode, string CollectifAccountLabel);

    }
}
