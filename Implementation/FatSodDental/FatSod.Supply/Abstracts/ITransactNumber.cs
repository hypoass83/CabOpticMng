using FatSod.Security.Entities;
using FatSod.Supply.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FatSod.Supply.Abstracts
{
    public interface ITransactNumber : IRepositorySupply<TransactNumber>
    {
        string returnTransactNumber(string trncode, BusinessDay bsday);
        string displayTransactNumber(string trncode, BusinessDay bsday);
        bool saveTransactNumber(string trncode, int compteur);
        string displayTransactNumber(string trncode, DateTime BDDateOperation, int BranchID);
        /// <summary>
        /// Cette méthode permet de générer un transact Number, le persister et renvoyer la référence au demandeur
        /// Le problème majeur ici c'est que si la référence n'est pas utilisée par le démandeur, le compteur aura 
        /// quand même été incrémentée et pour rien.
        /// </summary>
        /// <param name="trncode"></param>
        /// <param name="bsday"></param>
        /// <returns></returns>
        string GenerateAndUpdateTransactNumber(string trncode, BusinessDay bsday);
        string GenerateUniqueCIN();
       
    }
}
