using FatSod.Security.Entities;
using FatSod.Supply.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FatSod.Supply.Abstracts
{
    public interface ITransfert : IRepositorySupply<ProductTransfert>
    {
        ProductTransfert Sending(ProductTransfert prodTrans, int UserConnect);
        ProductTransfert Receiving(ProductTransfert prodTrans, int UserConnect);
        //bool ValidateTransfert(int prodTransID, int ArrivalLocalizationID);
        bool CancelTransfert(int prodTransID);
        ProductTransfert UpdateTransfert(ProductTransfert prodTrans);
        /// <summary>
        /// Cette méthode renvoie tous les transferts qui n'ont pas encore été reçus
        /// </summary>
        /// <param name="departureBranch">Branches de départ des transferts</param>
        /// <returns></returns>
        List<ProductTransfert> GetAllPendingTransfert(Branch departureBranch);
        /// <summary>
        /// Cette méthode renvoie tous les transferts qui n'ont pas encore été receptionnées par le destinataire
        /// </summary>
        /// <param name="arrivalBranch">Branches d'arrivée des transferts</param>
        /// <returns></returns>
        List<ProductTransfert> GetAllInProgressTransfert(Branch arrivalBranch);



    }
}
