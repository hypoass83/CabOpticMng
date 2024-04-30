using FatSod.Security.Entities;
using FatSod.Supply.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FatSod.Supply.Abstracts
{
    public interface ICustomerReturn : IRepositorySupply<CustomerReturn>
    {
        /// <summary>
        /// persiste un retour sale et fait une entree en stock
        /// </summary>
        /// <param name="customerReturn"></param>
        /// <returns></returns>
        bool ReturnSale(CustomerReturn customerReturn, int UserConect, int BranchID);
        /// <summary>
        /// Quantité de la ligne qui a déjà été retournée
        /// </summary>
        /// <param name="selectedSaleLine"></param>
        /// <returns></returns>
        double SaleLineReturnedQuantity(SaleLine selectedSaleLine);
        /// <summary>
        /// Cette méthode répond à la question est -ce que toutes les quantités de la ligne de vente ont déjà été retournées
        /// </summary>
        /// <param name="sl"></param>
        /// <returns></returns>
        bool IsAllLineReturn(SaleLine sl);
        
        /// <summary>
        /// Vérifie si la vente a au moins une ligne pouvant faire l'objet d'un retour
        /// </summary>
        /// <param name="sale"></param>
        /// <returns></returns>
        bool IsSaleCanBeReturn(Sale sale);

        bool ReturnAllSale(int saleID, string CustomerReturnCauses, int UserConect, int BranchID);
        /// <summary>
        /// Cette méthode retourne une vente qui, si elle a subit un retour, les quantités retournées ou les produits retournées sont prises en charge
        /// </summary>
        /// <param name="sale"></param>
        /// <returns></returns>
        Sale GetRealSale(Sale sale);
        /// <summary>
        /// Cette méthode retourne une vente qui, si elle a subit un retour, les quantités retournées ou les produits retournées sont prises en charge
        /// </summary>
        /// <param name="saleID">Vente</param>
        /// <returns>Retourne la vente réelle</returns>
        Sale GetRealSale(int saleID);

    }
}
