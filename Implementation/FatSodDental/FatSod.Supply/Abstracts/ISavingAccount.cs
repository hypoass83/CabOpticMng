using FatSod.Security.Entities;
using FatSod.Supply.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FatSod.Supply.Abstracts
{
    public interface ISavingAccount : IRepositorySupply<SavingAccount>
    {
        SavingAccount CreateSavingAccount(int CustomerID, int BranchID);
        /// <summary>
        /// Cette méthode permet de payer la vente d'un client en utilisant son compte.
        /// Pour le faire, nous allons créer les tranches client ayant comme mode de paiement le compte du client.
        /// Ici, on règle toute la vente ou le montant restant de la vente
        /// </summary>
        /// <param name="sale">Vente dont on souhaite régler</param>
        /// <param name="dateOperation">Date de règlement</param>
        /// <returns></returns>
        bool PayASale(Sale sale, DateTime dateOperation, string representant);
        /// <summary>
        /// Cette méthode permet de payer la vente d'un client en utilisant son compte.
        /// Pour le faire, nous allons créer les tranches client ayant comme mode de paiement le compte du client
        /// </summary>
        /// <param name="sale"></param>
        /// <param name="amount">Montant donc on souhaite payer pour la vente</param>
        /// <returns></returns>
        //bool PayASale(Sale sale, double amount, DateTime dateOperation);

        /// <summary>
        /// Cette méthode permet de régler tous les achats d'un client en utilisant le montant en cas de reste, on fera un dépôt dans le compte du client
        /// </summary>
        /// <param name="amount">Montant d'argent versé par le client</param>
        /// <param name="dateOperation"></param>
        /// <returns></returns>
        bool PayAllSales(int CustomerID, double amount, DateTime dateOperation, string representant, int UserConect, int BranchID);
 
        /// <summary>
        /// Cette méthode retourne la somme d'argent qu'un client a dans son compte.
        /// Dans cette méthode, on ne considère pas qu'il paie ses ventes à crédit
        /// la formule est donc : somme des dépots - somme des tranches clients payés avec le compte du client et non
        /// somme des ventes - (somme des dépots + somme de tranches clients - sommes des tranches client de type cpte client )
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        bool PayAllSales(List<Sale> sales, DateTime dateOperation);
        /// <summary>
        /// Cette méthode retourne la somme d'argent que le client a dans son compte d'épargne.
        /// la formule est : somme des dépôts d'épargne - sommes des tranches clients payées en utilisant le compte d'épargne
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        double GetSavingAccountBalance(Customer customer);

        /// <summary>
        /// Cette méthode retourne la somme d'argent que le client a dans son compte d'épargne Avant une date.
        /// la formule est : somme des dépôts d'épargne - sommes des tranches clients payées en utilisant le compte d'épargne
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        double GetSavingAccountBalance(Customer customer,DateTime BeginDate);

        /// <summary>
        /// Cette méthode retourne la somme d'argent que le client a dans son compte d'épargne. Cette méthode vise beaucoup plus à trouver le client general public 
        /// particulier
        /// la formule est : somme des dépôts d'épargne - sommes des tranches clients payées en utilisant le compte d'épargne
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        double GetSavingAccountBalance(Customer customer, string Representant);
        /// <summary>
        /// la formule est donc : somme des ventes - (somme des dépots + somme de tranches clients - sommes des tranches client de type cpte client )
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        double GetCustomerGeneralBalance(Customer customer);
        /// <summary>
        /// Cette méthode permet de faire un dépot sur le compte du client
        /// </summary>
        /// <param name="deposit"></param>
        /// <returns></returns>
        int DoADeposit(Deposit deposit, bool isAccountable, int userConnect);

    }
}
