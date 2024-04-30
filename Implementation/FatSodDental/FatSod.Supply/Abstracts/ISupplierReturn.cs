using FatSod.Security.Entities;
using FatSod.Supply.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FatSod.Supply.Abstracts
{
    public interface ISupplierReturn : IRepositorySupply<SupplierReturn>
    {
        /// <summary>
        /// persiste un retour sale et fait une entree en stock
        /// </summary>
        /// <param name="customerReturn"></param>
        /// <returns></returns>
        bool ReturnPurchase(SupplierReturn supplierReturn);
        /// <summary>
        /// Vérifie si l'achat a au moins une ligne pouvant faire l'objet d'un retour
        /// </summary>
        /// <param name="purchase"></param>
        /// <returns></returns>
        bool IsPurchaseCanBeReturn(Purchase purchase);
        /// <summary>
        /// Cette méthode répond à la question est -ce que toutes les quantités de la ligne de vente ont déjà été retournées
        /// </summary>
        /// <param name="pl"></param>
        /// <returns></returns>
        bool IsAllLineReturn(PurchaseLine pl);
        /// <summary>
        /// Quantité de la ligne qui a déjà été retournée
        /// </summary>
        /// <param name="selectedPurchaseLine"></param>
        /// <returns></returns>
        double PurchaseLineReturnedQuantity(PurchaseLine selectedPurchaseLine);
        /// <summary>
        /// Retourne totalement un achat
        /// </summary>
        /// <param name="customerReturn"></param>
        /// <returns></returns>
        bool ReturnPurchase(Purchase purchase);
        
    }
}
