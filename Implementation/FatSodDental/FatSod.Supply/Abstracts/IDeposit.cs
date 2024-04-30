using FatSod.Security.Entities;
using FatSod.Supply.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FatSod.Supply.Abstracts
{
    public interface IDeposit : IRepositorySupply<Deposit>
    {
        /// <summary>
        /// list des clients ayant au moins une vente et une dette liée à un achat dans la branche
        /// </summary>
        /// <param name="branch">branch d'appartenance des clients</param>
        /// <returns>La list des clients ayant au moins une vente et une dette liée à un achat dans la branche</returns>
        List<Customer> CustomersDebtors(Branch branch);
        /// <summary>
        /// cette méthode retourne la liste des achats(vente) non totalement réglées d'un client
        /// </summary>
        /// <param name="customer">Client dont on veut la liste de ses ventes non réglées</param>
        /// <returns>Liste des ventes non réglées du client.</returns>
        List<Sale> UnPaidSales(Customer customer);

        /// <summary>
        /// cette méthode retourne la dette du client
        /// </summary>
        /// <param name="customer">Client dont on veut la dette </param>
        /// <returns>Dette du client envers l'entreprise</returns>
        double CustomerDebtStockLens(Customer customer);
        /// <summary>
        /// cette méthode retourne la dette du client avant une date
        /// </summary>
        /// <param name="customer">Client dont on veut la dette </param>
        /// <returns>Dette du client envers l'entreprise</returns>
        double CustomerDebtStockLens(Customer customer,DateTime BeginDate);

        /// <summary>
        /// cette méthode retourne la dette Special Order du client
        /// </summary>
        /// <param name="customer">Client dont on veut la dette Special Order </param>
        /// <returns>Dette Special Order du client envers l'entreprise</returns>
        double CustomerDebtSpecOrder(Customer customer);
        /// <summary>
        /// cette méthode retourne la dette Special Order du client Avant une date
        /// </summary>
        /// <param name="customer">Client dont on veut la dette Special Order </param>
        /// <returns>Dette Special Order du client envers l'entreprise</returns>
        double CustomerDebtSpecOrder(Customer customer,DateTime BeginDate);

        /// <summary>
        /// cette méthode retourne la dette du client.
        /// Elle permet beaucoup plus de récupérer la dette d'un géréral public particulier.
        /// Pour obtenir cette dette, on récupera toutes les ventes non payées dont le nom du représentant est celui en paramètre
        /// </summary>
        /// <param name="customer">Client dont on veut la dette </param>
        /// <param name="representant">General public particulier dont on veut la dette </param>
        /// <returns>Dette du client envers l'entreprise</returns>
        double CustomerDebtStockLens(Customer customer, string representant);
        double CustomerDebtSpecOrder(Customer customer, string representant);
        /// <summary>
        /// cette méthode retourne la liste des ventes non totalement payées
        /// </summary>
        /// <returns>liste des ventes non totalement payées</returns>
        List<Sale> AllUnPaidSales();

        /// <summary>
        /// Cette méthode renvoie le reste à payer d'une vente
        /// </summary>
        /// <param name="sale">Vente dont on souhaite obtenir le reste à payer</param>
        /// <returns>Le reste à payer de la vente</returns>
        double SaleRemainder(Sale sale);

        /// <summary>
        /// cette méthode permet de faire un dépôt pour régler une vente
        /// </summary>
        /// <param name="deposit">Objet contenant les information nécessaire au dépot et à la compta</param>
        /// <returns>Un booléan pour dire si le dépôt s'est bien passé</returns>
        Boolean SaleDepositForNonAssure(Deposit deposit, int SaleID, int UserConect, int SaleDeliver);

        /// <summary>
        /// cette méthode permet de faire un dépôt pour special order pour régler une vente
        /// </summary>
        /// <param name="deposit">Objet contenant les information nécessaire au dépot et à la compta</param>
        /// <returns>Un booléan pour dire si le dépôt s'est bien passé</returns>
        Boolean SaleDepositSpecialOrder(Deposit deposit, int UserConect);

        /// <summary>
        /// retourne le montant total qui a déjà été payé sur un achat
        /// </summary>
        /// <param name="purchase"></param>
        /// <returns></returns>
        double PurchaseTotalPriceAdvance(Purchase purchase);
                /// <summary>
        /// retourne le montant total qui a déjà été payé sur une vente
        /// </summary>
        /// <param name="sale"></param>
        /// <returns></returns>
        double SaleTotalPriceAdvance(Sale sale);
        /// <summary>
        ///Liste des ventes non totalement réglées d'un client
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        List<Sale> CustomerAllUnPaidSalesStockLens(Customer customer);
        /// <summary>
        ///Liste des ventes non totalement réglées d'un client avant une Date
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        List<Sale> CustomerAllUnPaidSalesStockLens(Customer customer,DateTime BeginDate);

        /// <summary>
        ///Liste des ventes Special Order non totalement réglées d'un client
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        List<Sale> CustomerAllUnPaidSalesSpecialOrder(Customer customer);
        /// <summary>
        ///Liste des ventes Special Order non totalement réglées d'un client Avant une date
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        List<Sale> CustomerAllUnPaidSalesSpecialOrder(Customer customer,DateTime BeginDate);
        /// <summary>
        ///Liste des ventes non totalement réglées d'un général public particulier
        /// </summary>
        /// <param name="customer"></param>
        /// <param name="representant">général public particulier</param>
        /// <returns></returns>
        List<Sale> CustomerAllUnPaidSalesStockLens(Customer customer, string representant);

        /// <summary>
        ///Liste des ventes SpecialOrder non totalement réglées d'un général public particulier
        /// </summary>
        /// <param name="customer"></param>
        /// <param name="representant">général public particulier</param>
        /// <returns></returns>
        List<Sale> CustomerAllUnPaidSalesSpecialOrder(Customer customer, string representant);

        List<Sale> OtherCustomerAllUnPaidSalesForNonAssure(int saleID);
        List<Sale> OtherCustomerAllUnPaidSalesSpecialOrder(Customer customer);
        List<Sale> CustomerAllPeriodSales(Customer customer,DateTime beginDate,DateTime endDate);
        /// <summary>
        /// cette méthode retourne le montant total d'un achat
        /// </summary>
        /// <param name="purchase"></param>
        /// <returns></returns>
        double BillAmount(Purchase purchase);
        /// <summary>
        /// cette méthode retourne le montant total d'une vente
        /// </summary>
        /// <param name="sale"></param>
        /// <returns></returns>
        double BillAmount(Sale sale);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sale"></param>
        /// <returns></returns>
        Sale ApplyExtraPrice(Sale sale);

         /// <summary>
        /// Cette méthode renvoie la facture à payée pour une vente.
        /// C'est égale à la facture de la vente moins les retours
        /// </summary>
        /// <param name="sale"></param>
        /// <returns></returns>
        double SaleBill(Sale sale);
        /// <summary>
        /// Permet essentiellement de récupérer le montant TTC d'un retour
        /// </summary>
        /// <param name="cr"></param>
        /// <returns></returns>
        CustomerReturn ApplyExtraPrice(CustomerReturn cr);

        bool DeleteDepositEntry(int AllDepositID, int UserID,DateTime ServerDate);

        /// <summary>
        /// methode permetant de determiner tous les depots effectues par un client avant une periode
        /// </summary>
        /// <param name="customer"></param>
        /// <param name="BeginDate"></param>
        /// <returns></returns>
        double TotalDepotSliceBefore(Customer customer, DateTime BeginDate);
        /// <summary>
        /// methode permetant de determiner tous les depots effectues par un client pendant une periode
        /// </summary>
        /// <param name="customer"></param>
        /// <param name="BeginDate"></param>
        /// <returns></returns>
        double TotalDepotSlicePeriode(Customer customer, DateTime BeginDate,DateTime EndDate);
        /// <summary>
        /// methode permetant de determiner tous les depots effectues par un client
        /// </summary>
        /// <param name="customer"></param>
        /// <param name="BeginDate"></param>
        /// <returns></returns>
        double TotalDepotSlice(Customer customer);
        /// <summary>
        /// cette méthode permet de faire un dépôt pour régler une vente
        /// </summary>
        /// <param name="deposit">Objet contenant les information nécessaire au dépot et à la compta</param>
        /// <returns>Un booléan pour dire si le dépôt s'est bien passé</returns>
        Boolean SaleDepositCustomer(Deposit deposit, int UserConect);
        /// <summary>
        /// methode permetant de faire un depot pour un client assure dont la facture a ete valide
        /// </summary>
        /// <param name="deposit"></param>
        /// <param name="UserConect"></param>
        /// <returns></returns>
        AllDeposit SaleDepositForInsured(Deposit deposit , int UserConect);

        //modif after change ext to jquery
        double NewTotalDepotSlice(int? customerID);
        double NewTotalAchatBefore(int? customerID, DateTime BeginDate);
        double NewTotalDepotSliceBefore(int? customerID, DateTime BeginDate);

        double NewTotalAchat(int? customerID);
        double NewTotalAchatPeriode(int? customerID, DateTime BeginDate, DateTime EndDate);
        double NewTotalDepotSlicePeriode(int? customerID, DateTime BeginDate, DateTime EndDate);

        List<CalculSodeGenerale> CalculSodeGenerale(DateTime BeginDate, DateTime EndDate);
        List<CalculSodeGenerale> CalculSodeGenerale(DateTime BeginDate, DateTime EndDate, int GlobalPersonID);
        List<CalculSodeGenerale> CalculSodeGenerale(int BranchID, DateTime BeginDate, DateTime EndDate);
    }
}
