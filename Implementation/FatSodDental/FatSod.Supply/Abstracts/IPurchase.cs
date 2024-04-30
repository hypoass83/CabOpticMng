using FatSod.Supply.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FatSod.Supply.Abstracts
{
    public interface IPurchase : IRepositorySupply<Purchase>
    {
        //double PurchaseTotalPriceAdvance(Purchase purchase);
        Purchase CreatePurchase(Purchase purchase);
        bool DeletePurchase(int PurchaseID);
        Purchase UpdatePurchase(Purchase purchase);
		//Purchase CancelPurchase(Purchase purchase);
        void CreateProdLocForPurLine(PurchaseLine purLine, Purchase purchase, bool update);
        /// <summary>
        /// Cette méthode permet de créér un achat sauf qu'elle peut être appelée dans une transaction
        /// </summary>
        /// <param name="purchase"></param>
        /// <returns></returns>
        Purchase SavePurchase(Purchase purchase);
        /// <summary>
        /// Cette méthode permet de supprimer un achat; à la seule différence qu'elle peut être utilisée dans une transaction
        /// </summary>
        /// <param name="PurchaseID"></param>
        /// <returns></returns>
        bool RemovePurchase(int PurchaseID);
        void ValidatePurchase(SupplierOrder supplierOrder);



    }
}
