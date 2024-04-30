using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FatSod.Security.Entities;

namespace FatSod.Security.Abstracts
{
    public interface IBusinessDay : IRepository<BusinessDay>
    {
        BusinessDay GetOpenedBusinessDay(Branch branch);
        List<BusinessDay> GetOpenedBusinessDay(List<Branch> branches);
        List<BusinessDay> GetOpenedBusinessDay(User user);
        List<BusinessDay> GetOpenedBusinessDay();
        List<Branch> GetOpenedBranches();
        List<BusinessDay> GetClosedBusinessDay();
        List<BusinessDay> GetClosedBusinessDay(User user);
        /// <summary>
        ///  Cette méthode permet : 
        ///  1- d'ouvrir une journée de travail : Toutes les agences de la société doivent être ouvertes avec la même date.
        ///  2- Réinitialiser la TransactionNumber lors de l'ouverture de la première branche de la journée
        /// NB : 
        /// 1 - Pendant l'ouverture d'une branche, s'il existe au moins une branche ouverte, la date d'ouverture est la date d'ouverture de celle qui est déjà ouverte.
        /// 2 - Pour que la date d'ouverture d'une agence (BusinessDay change, il faut d'abord fermer toutes les agences ouvertes)
        /// </summary>
        /// <param name="branch">C'est la branche dont on souhaite ouvrir</param>
        /// <param name="dateOperation"> C'est la date avec laquelle on souhaite ouvrir la journée de trvail</param>
        /// <returns>Valeur disant si l'ouverture s'est bien déroulée</returns>
        Boolean OpenBusinessDay(Branch branch, DateTime dateOperation, int UserConect);
        /// <summary>
        /// Cette méthode permet de fermer une journée de travail
        /// </summary>
        /// <param name="branch">Branch dont on souhaite fermer</param>
        /// <returns>Valeur disant si la fermeture s'est bien déroulée</returns>
        Boolean CloseBusinessDay(Branch branch, DateTime dateOperation, int UserConect);

        Boolean OpenBackDate(Branch branch, DateTime dateOperation);
        List<BusinessDay> GetOpenedBackDate(User user);
    }
}
