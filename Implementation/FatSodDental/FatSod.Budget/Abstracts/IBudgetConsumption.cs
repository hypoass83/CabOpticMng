using FatSod.Budget.Entities;
using FatSod.Supply.Abstracts;
using FatSod.Supply.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FatSod.Budget.Abstracts
{
    public interface IBudgetConsumption : IRepositorySupply<BudgetConsumption>
    {
        /// <summary>
        /// creation de l'autorisation d'utilisation du budget
        /// </summary>
        /// <param name="budgetConsumption"></param>
        /// <returns></returns>
        BudgetConsumption CreateBudgetConsumption(BudgetConsumption budgetConsumption);
        BudgetConsumption UpdateBudgetConsumption(BudgetConsumption budgetConsumption);
        /// <summary>
        /// validation du budget : mise a jour du statut et choix de la methode de payement
        /// </summary>
        /// <param name="budgetConsumption"></param>
        /// <param name="paymentMethodId"></param>
        /// <returns></returns>
        bool SavebudgetConsumption(BudgetConsumption BudgetConsumption, int UserConect, int BranchID);
    }
}
