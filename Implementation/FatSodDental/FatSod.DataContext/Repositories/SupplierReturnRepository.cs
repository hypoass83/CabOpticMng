using FastSod.Utilities.Util;
using FatSod.DataContext.Concrete;
using FatSod.Supply.Abstracts;
using FatSod.Supply.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;


namespace FatSod.DataContext.Repositories
{
    public class SupplierReturnRepository : RepositorySupply<SupplierReturn>, ISupplierReturn
    {
        public IRepositorySupply<SupplierReturnLine> _supplierReturnLineRepository;

        public SupplierReturnRepository(EFDbContext ctx)
            : base(ctx)
        {
            _supplierReturnLineRepository = new RepositorySupply<SupplierReturnLine>(this.context);
        }
        public SupplierReturnRepository()
            : base()
        {
            _supplierReturnLineRepository = new RepositorySupply<SupplierReturnLine>(this.context);
        }
        public bool IsCustomerReturnExist(SupplierReturn supplierReturn)
        {
            bool res = false;

            SupplierReturn supplierReturn1 = context.SupplierReturns.AsNoTracking().SingleOrDefault(sr => sr.PurchaseID == supplierReturn.PurchaseID);

            if (supplierReturn1 != null && supplierReturn1.SupplierReturnID > 0)
            {
                res = true;
            }

            return res;
        }
        public bool SimpleReturnPurchase(SupplierReturn supplierReturn)
        {

            try
            {
                List<SupplierReturnLine> supplierReturnLines = supplierReturn.SupplierReturnLines.ToList();
                supplierReturn.SupplierReturnLines = null;

                if (IsCustomerReturnExist(supplierReturn) == false)
                {
                    supplierReturn = this.context.SupplierReturns.Add(supplierReturn);
                    this.context.SaveChanges();
                }
                else
                {
                    SupplierReturn supplierReturn1 = context.SupplierReturns.AsNoTracking().SingleOrDefault(sr => sr.PurchaseID == supplierReturn.PurchaseID);
                    supplierReturn.SupplierReturnID = supplierReturn1.SupplierReturnID;
                }

                bool res = false;
                supplierReturnLines.ToList().ForEach(srl =>
                {
                    srl.SupplierReturnID = supplierReturn.SupplierReturnID;
                    srl.Transport = supplierReturn.Transport;
                    //persiter un return sale

                    context.SupplierReturnLines.Add(srl);
                    context.SaveChanges();
                    //On fait une sortie en stock ici
                    ProductLocalization productLocalizationToUpdate = context.ProductLocalizations.FirstOrDefault(pl => pl.ProductID == srl.ProductID && pl.LocalizationID == srl.LocalizationID);
                    productLocalizationToUpdate.ProductLocalizationStockQuantity -= srl.LineQuantity;
                    context.SaveChanges();

                    //on comptabilise le retour en stock d'une ligne de produit
                });

                //Mise à jour de l'achat  pour indiquer q'au moins un retour a été fait sur l'achat ou que toute l'achat a été retourné 
                Purchase currentPurchase = context.Purchases.AsNoTracking().SingleOrDefault(p => p.PurchaseID == supplierReturn.PurchaseID);
                currentPurchase.isReturn = true;
                //Si l'achat peut encore faire l'objet d'un retour, il garde son statut précédent si non il a le statut retourné
                currentPurchase.StatutPurchase = (IsPurchaseCanBeReturn(currentPurchase)) ? currentPurchase.StatutPurchase : SalePurchaseStatut.Returned;
                context.Purchases.Attach(currentPurchase);
                context.Entry(currentPurchase).State = EntityState.Modified;
                context.SaveChanges();

                //on comptabilise le retour en stock de tout le retour


                return res;
            }
            catch (Exception e)
            {
                //If an errors occurs, we cancel all changes in database
                throw new Exception("Une erreur s'est produite lors du retour vente : " + "Message =  " + e.Message + "StackTrace = " + e.StackTrace + "Source =  " + e.Source);
            }
        }
        public bool ReturnPurchase(SupplierReturn supplierReturn)
        {
            bool res = false ;
                try
                {
                    using (TransactionScope ts = new TransactionScope())
                    {
                        res = SimpleReturnPurchase(supplierReturn);

                    //we apply this modifications
                        //transaction.Commit();
                        ts.Complete();
                    }
                }
                catch (Exception e)
                {
                    //If an errors occurs, we cancel all changes in database
                    //transaction.Rollback();
                    res=false ;
                    throw new Exception("Une erreur s'est produite lors du retour vente : " + "Message =  " + e.Message + "StackTrace = " + e.StackTrace + "Source =  " + e.Source);
                }
            return res;

        }

        public bool ReturnPurchase(Purchase purchase)
        {
            this.SimpleReturnPurchase(this.GetSupplierReturnFromPurchase(purchase));

            return true;
        }

        public SupplierReturn GetSupplierReturnFromPurchase(Purchase purchase)
        {
            List<SupplierReturnLine> supplierReturnLines = new List<SupplierReturnLine>();

            foreach (PurchaseLine pl in purchase.PurchaseLines)
            {
                supplierReturnLines.Add(GetSupRetLineFromPurLine(pl));
            }

            SupplierReturn res = new SupplierReturn()
            {
                PurchaseID = purchase.PurchaseID,
                SupplierReturnDate = purchase.PurchaseDate,
                SupplierReturnLines = supplierReturnLines,
                Transport = purchase.Transport,
            };

            return res;
        }

        public SupplierReturnLine GetSupRetLineFromPurLine(PurchaseLine purLine)
        {

            SupplierReturnLine res = new SupplierReturnLine()
            {
                Transport = 0,
                SupplierReturnCauses = "Modification ou suppression de la commande d'un verre de commande",
                PurchaseLineID = purLine.LineID,
                LineQuantity = purLine.LineQuantity,
                ProductID = purLine.ProductID,
                LocalizationID = purLine.LocalizationID,
            };

            return res;

        }

        /// <summary>
        /// Vérifie si l'achat a au moins une ligne pouvant faire l'objet d'un retour
        /// </summary>
        /// <param name="purchase"></param>
        /// <returns></returns>
        public bool IsPurchaseCanBeReturn(Purchase purchase)
        {
            bool res = true;
            SupplierReturn purchaseSR = this.FindAll.SingleOrDefault(sr => sr.PurchaseID == purchase.PurchaseID);

            //il y a déjà eu au moins un retour sur cette vente
            if (purchaseSR != null && purchaseSR.SupplierReturnID > 0)
            {
                res = false;
                foreach (PurchaseLine pl in purchase.PurchaseLines)
                {
                    if (IsAllLineReturn(pl) == false)
                    {
                        res = true;
                        break;
                    }
                }
            }

            return res;
        }

        /// <summary>
        /// la méthode précédente à celle-ci ne fonctionne pas peut être parceque la transaction n'a pas encore été commitée
        /// </summary>
        /// <param name="sale"></param>
        /// <returns></returns>
        public bool IsPurchaseCanBeReturn(Purchase purchase, List<SupplierReturnLine> supplierReturnLines)
        {
            bool res = true;

            res = this.IsPurchaseCanBeReturn(purchase);

            if (res == true)
            {
                res = false;
                //si quantité retournée + qté en cours de retour = qté de la ligne alors le retour est consommé
                foreach (SupplierReturnLine srl in supplierReturnLines)
                {
                    PurchaseLine pl = context.PurchaseLines.Find(srl.PurchaseLineID);
                    if ((PurchaseLineReturnedQuantity(pl) + srl.LineQuantity) < pl.LineQuantity)
                    {
                        res = true;
                        break;
                    }
                }
            }

            return res;
        }

        /// <summary>
        /// Cette méthode répond à la question est -ce que toutes les quantités de la ligne de vente ont déjà été retournées
        /// </summary>
        /// <param name="pl"></param>
        /// <returns></returns>
        public bool IsAllLineReturn(PurchaseLine pl)
        {
            bool res = true;

            //Quantité déjà retournée sur cette ligne de vente
            double returnedQuantity = PurchaseLineReturnedQuantity(pl);

            if (returnedQuantity < pl.LineQuantity)
            {
                res = false;
            }

            return res;
        }

        //// <summary>
        /// Quantité de la ligne qui a déjà été retournée
        /// </summary>
        /// <param name="selectedPurchaseLine"></param>
        /// <returns></returns>
        public double PurchaseLineReturnedQuantity(PurchaseLine selectedPurchaseLine)
        {

            List<SupplierReturnLine> returnList = _supplierReturnLineRepository.FindAll.Where(srl => srl.PurchaseLineID == selectedPurchaseLine.LineID).ToList();
            return (returnList == null || returnList.Count() == 0) ? 0 : returnList.Select(r => r.LineQuantity).Sum();
        }

    }
}
