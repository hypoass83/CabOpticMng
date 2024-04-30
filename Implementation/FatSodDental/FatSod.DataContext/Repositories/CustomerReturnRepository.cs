using FastSod.Utilities.Util;
using FatSod.DataContext.Concrete;
using FatSod.Security.Abstracts;
using FatSod.Security.Entities;
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
    public class CustomerReturnRepository : RepositorySupply<CustomerReturn>, ICustomerReturn
    {
        IProductLocalization _plRepository;
        public IRepositorySupply<CustomerReturnLine> _customerReturnLineRepository;
        private IBusinessDay bdRepo;
        private ISavingAccount saRepo;
        private ITransactNumber _transactNumbeRepository;
        public IRepositorySupply<CustomerReturnSlice> _CustomerReturnSliceRepository;

        public CustomerReturnRepository(EFDbContext ctext)
		{
			this.context = ctext;
		}
		public CustomerReturnRepository()
			: base()
		{
            _customerReturnLineRepository = new RepositorySupply<CustomerReturnLine>();
            bdRepo = new BusinessDayRepository();
            saRepo = new SavingAccountRepository();
            _CustomerReturnSliceRepository = new RepositorySupply<CustomerReturnSlice>();
            _transactNumbeRepository = new TransactNumberRepository();
		}
        //public CustomerReturnRepository()
        //{
        //    _customerReturnLineRepository = new RepositorySupply<CustomerReturnLine>();
        //    bdRepo = new BusinessDayRepository();
        //    saRepo = new SavingAccountRepository();
        //}
        /// <summary>
        /// Cette méthode existe si la vente liéee à ce retour a déjà subit un retour. C'est parcequ'on ne créé qu'un et un seul CustomerReturn
        /// pour une vente; dans lequel on chargera les CustomerReturnLine pour les différents retours de la vente
        /// </summary>
        /// <param name="customerReturn"></param>
        /// <returns></returns>
        public bool IsCustomerReturnExist(CustomerReturn customerReturn)
        {
            bool res = false;

            CustomerReturn customerReturn1 = context.CustomerReturns.AsNoTracking().SingleOrDefault(cr => cr.SaleID == customerReturn.SaleID);

            if (customerReturn1 != null && customerReturn1.CustomerReturnID > 0)
            {
                res = true;
            }

            return res;
        }

        /// <summary>
        /// Cette méthode existe si la vente liéee à ce retour a déjà subit un retour. C'est parcequ'on ne créé qu'un et un seul CustomerReturn
        /// pour une vente; dans lequel on chargera les CustomerReturnLine pour les différents retours de la vente
        /// </summary>
        /// <param name="customerReturn"></param>
        /// <returns></returns>
        public bool IsCustomerReturnExist(int SaleID)
        {
            bool res = false;

            CustomerReturn customerReturn1 = (from cusret in context.CustomerReturns
                                              where cusret.SaleID == SaleID
                                              select cusret).SingleOrDefault();
                //context.CustomerReturns.AsNoTracking().SingleOrDefault(cr => cr.SaleID == SaleID);

            if (customerReturn1 != null && customerReturn1.CustomerReturnID > 0)
            {
                res = true;
            }

            return res;
        }

        /// <summary>
        /// Cete méthode enregistre les informations sur le retour.
        /// </summary>
        /// <param name="customerReturn"></param>
        /// <param name="BranchID"></param>
        /// <param name="UserConect"></param>
        /// <returns></returns>
        public bool ReturnSale(CustomerReturn customerReturn, int UserConect, int BranchID)
        {

            //Begin of transaction
            bool res = false;
            double TotalPriceMarchandise = 0d;
                try
                {
                    using (TransactionScope ts = new TransactionScope())
                    {
                        List<CustomerReturnLine> customerReturnLines = customerReturn.CustomerReturnLines.ToList();
                        customerReturn.CustomerReturnLines = null;

                        if (IsCustomerReturnExist(customerReturn) == false)
                        {
                            customerReturn = this.context.CustomerReturns.Add(customerReturn);
                            this.context.SaveChanges();
                        }
                        else
                        {
                            CustomerReturn customerReturn1 = context.CustomerReturns.AsNoTracking().SingleOrDefault(cr => cr.SaleID == customerReturn.SaleID);
                            customerReturn.CustomerReturnID = customerReturn1.CustomerReturnID;
                        }

                    Sale currentSale = context.Sales.Find(customerReturn.SaleID);

                   foreach (CustomerReturnLine crl in customerReturnLines.ToList()) //customerReturnLines.ToList().ForEach(crl =>
                        {
                            crl.CustomerReturn = null;
                            crl.SaleLine = null;
                            crl.CustomerReturnID = customerReturn.CustomerReturnID;
                            crl.CustomerReturnDate = customerReturn.CustomerReturnDate;
                            crl.Transport = customerReturn.Transport;
                            //persiter un return sale

                            context.CustomerReturnLines.Add(crl);
                            context.SaveChanges();

                            SaleLine sl = context.SaleLines.Find(crl.SaleLineID);
                            TotalPriceMarchandise += crl.LineQuantity * sl.LineUnitPrice;

                            //On fait une rentrée en stock ici si le produit etait sorti
                            // si c'est un frame 
                            //if (currentSale.SaleDeliver )
                            if (crl.SaleLine.NumeroSerie!=null)
                            {
                                ProductLocalization productLocalizationToUpdate = context.ProductLocalizations.FirstOrDefault(pl => pl.ProductID == crl.ProductID && pl.LocalizationID == crl.LocalizationID 
                                && pl.NumeroSerie==crl.SaleLine.NumeroSerie);
                                if (productLocalizationToUpdate!=null)
                                {
                                    productLocalizationToUpdate.ProductLocalizationStockQuantity += crl.LineQuantity;



                                //HISTORISATION DU STOCK
                                    DateTime date = customerReturn.CustomerReturnDate;// productLocalizationToUpdate.ProductLocalizationDate;
                                    DateTime actualDate = DateTime.Now;
                                    date = date.AddHours(actualDate.Hour);
                                    date = date.AddMinutes(actualDate.Minute);
                                    date = date.AddSeconds(actualDate.Second);
                                    InventoryHistoric inventoryHistoric = new InventoryHistoric
                                    {
                                        //les nouvelles infos
                                        NewSafetyStockQuantity = productLocalizationToUpdate.ProductLocalizationSafetyStockQuantity,
                                        NewStockQuantity = productLocalizationToUpdate.ProductLocalizationStockQuantity,
                                        NEwStockUnitPrice = productLocalizationToUpdate.ProductLocalizationStockSellingPrice,
                                        //les anciennes infos
                                        OldSafetyStockQuantity = productLocalizationToUpdate.ProductLocalizationSafetyStockQuantity,
                                        OldStockQuantity = productLocalizationToUpdate.ProductLocalizationStockQuantity - crl.LineQuantity,
                                        OldStockUnitPrice = productLocalizationToUpdate.ProductLocalizationStockSellingPrice,
                                        //Autres informations
                                        InventoryDate = date,
                                        inventoryReason = "Product Return Sales",
                                        LocalizationID = productLocalizationToUpdate.LocalizationID,
                                        ProductID = productLocalizationToUpdate.ProductID,
                                        RegisteredByID = UserConect,
                                        AutorizedByID = UserConect,
                                        CountByID = UserConect,
                                        StockStatus = "INPUT",
                                        Description = sl.Sale.PersonName,
                                        Quantity = crl.LineQuantity,
                                        NumeroSerie=crl.SaleLine.NumeroSerie,
                                        Marque=crl.SaleLine.marque
                                        
                                    };
                                    context.InventoryHistorics.Add(inventoryHistoric);
                                    context.SaveChanges();
                                }
                                
                            }
                            

                        };

                        //Mise à jour de la vente pour indiquer q'au moins un retour a été fait sur la vente ou que toute la vente à 
                        //Sale currentSale = context.Sales.AsNoTracking().SingleOrDefault(s => s.SaleID == customerReturn.SaleID);
                        //currentSale.isReturn = true;
                        ////Si la vente peut encore faire l'objet d'un retour, elle garde son statut précédent si non elle a le statut retournée
                        //currentSale.StatutSale = (IsSaleCanBeReturn(currentSale)) ? currentSale.StatutSale : SalePurchaseStatut.Returned;//Ne marche pas encore le 02/09/2015
                        //context.Sales.Attach(currentSale);
                        //context.Entry(currentSale).State = EntityState.Modified;

                        //Sale currentSale = context.Sales.Find(customerReturn.SaleID);
                        if (currentSale != null)
                        {
                            currentSale.isReturn = true;
                            //Si la vente peut encore faire l'objet d'un retour, elle garde son statut précédent si non elle a le statut retournée
                            currentSale.StatutSale = (IsSaleCanBeReturn(currentSale)) ? currentSale.StatutSale : SalePurchaseStatut.Returned;//Ne marche pas encore le 02/09/2015
                            context.SaveChanges();
                        }
                        //mise a jour CumulSaleAndBill
                        CumulSaleAndBill existingCumulSaleAndBill = context.CumulSaleAndBills.Where(cb => cb.SaleID == customerReturn.SaleID).FirstOrDefault();
                        if (existingCumulSaleAndBill!=null)
                        {
                            existingCumulSaleAndBill.isReturn = true;
                            context.SaveChanges();
                        }
                        //EcritureSneack
                        Mouchar MoucharToSave = new Mouchar()
                        {
                            MoucharUserID = UserConect,
                            MoucharAction = "SUCCESS",
                            MoucharDescription = "RETURN SALE FOR CUSTOMER " + currentSale.PoliceAssurance + " REF " + currentSale.SaleReceiptNumber,
                            MoucharOperationType = "INSERT",
                            MoucharBranchID = BranchID,
                            MoucharBusinessDate = customerReturn.CustomerReturnDate,
                            MoucharProcedureName = "ReturnSale"
                        };
                        context.Entry(MoucharToSave).State = EntityState.Detached;
                        context.Mouchars.Add(MoucharToSave);
                        context.SaveChanges();

                        //IMouchar opSneak = new MoucharRepository(this.context);
                        //bool res1 = opSneak.InsertOperation(UserConect, "SUCCESS", "RETURN SALE FOR CUSTOMER " + currentSale.Representant + " REF " + currentSale.SaleReceiptNumber, "ReturnSale", customerReturn.CustomerReturnDate, BranchID);
                        //if (!res1)
                        //{
                        //    throw new Exception("Une erreur s'est produite lors de la journalisation ");
                        //}
                        ExtraPrice extra = Util.ExtraPrices(TotalPriceMarchandise, currentSale.RateReduction, currentSale.RateDiscount, customerReturn.Transport, currentSale.VatRate);

                        //on comptabilise le retour en stock de tout le retour
                        
                        CustomerReturn acountableCustomerReturn = new CustomerReturn()
                        {
                            CustomerReturnDate = customerReturn.CustomerReturnDate,
                            TotalPriceMarchandise = TotalPriceMarchandise,
                            TotalPriceReturn = extra.TotalTTC,
                            DiscountAmount = extra.DiscountAmount,
                            Transport = customerReturn.Transport,
                            TVAAmount = extra.TVAAmount,
                            CustomerReturnID = customerReturn.CustomerReturnID,
                            Sale = currentSale,
                            SaleID = customerReturn.SaleID
                        };
                    //a revoir quand la compta sera plus claire dans mon exprit
                    //if (extra.TotalTTC > 0)
                    //{
                    //    IAccountOperation opaccount = new AccountOperationRepository(context);
                    //    res = opaccount.ecritureComptableFinal(acountableCustomerReturn);
                    //    if (!res)
                    //    {
                    //        //transaction.Rollback();
                    //        throw new Exception("Une erreur s'est produite lors de comptabilisation de la vente ");
                    //    }
                    //}
                    //UserTill currenteller = context.UserTills.Where(c => c.HasAccess /*c.UserID == currentSale.OperatorID*/).FirstOrDefault();

                    //recuperation de la methode de payment ki avait ete utiliser pour effectué le payment
                    int payMethodId = 0;
                    PaymentMethod pMethod = context.CustomerSlices.FirstOrDefault(p => p.SaleID == customerReturn.SaleID).PaymentMethod;
                    if(pMethod == null)
                    {
                        throw new Exception("Wrong Payment Method!!! Contact your Administrator ");
                    }
                    if (pMethod is Till)
                    {
                        //recuperation de la premiere caisse ouverte
                        TillDayStatus tillDay = context.TillDayStatus.FirstOrDefault(td => td.IsOpen);
                        if (tillDay == null)
                        {
                            throw new Exception("At Least One Cash Register Must Open Before Proceed ");
                        }
                        payMethodId = tillDay.TillID; // context.UserTills.Where(c => c.TillID == tillDay.TillID).FirstOrDefault().TillID;
                    }
                    else
                    {
                        payMethodId = pMethod.ID;
                    }


                    //On transfert l'argent qui a été utilisé pour régler la facture du client dans le compte du client
                    //1-Retrait de l'argent dans la caisse
                    //cette methode sera appliker slt pr les client noncash
                    //CashCustomer isCashCusto = currentSale.Customer.IsCashCustomer; // context.Customers.Find(currentSale.Customer.GlobalPersonID).IsCashCustomer;
                    double Amount = this.ReturnCustomerMoney(acountableCustomerReturn, payMethodId, this.context, UserConect);
                    //2-On met l'argent dans le compte du client = c'est un dépôt d'épargne avec comme mode de paiement la caisse
                    if (Amount > 0)
                    {
                        this.SaveCustomerReturnAmount(acountableCustomerReturn, payMethodId, Amount, this.context, UserConect);
                    }
                    

                        //we apply this modifications
                        //transaction.Commit();
                        
                        ts.Complete();
                    }
                }
                catch (Exception e)
                {
                    
                    //If an errors occurs, we cancel all changes in database
                    //transaction.Rollback();
                    throw new Exception("Une erreur s'est produite lors du retour vente : " + "Message =  " + e.Message + "StackTrace = " + e.StackTrace + "Source =  " + e.Source);
                }
                return res;
        }

        /// <summary>
        /// Cette Méthode permet de remboursé l'équivalent en argent des retours de marchandise faits par un client.
        /// L'argent sort de l'une des caisses de la branches dans laquelle la marchandise a été achétée.
        /// NB : On ne retourne que la somme déjà versée moins la somme des retours déjà reçus.
        /// On rembourse de l'argent au client si et seulement si le montant déjà avancé est supérieur au montant de la nouvelle facture;
        /// c'est à dire après le retour
        /// </summary>
        /// <param name="cr"></param>
        /// <param name="ctx"></param>
        /// <param name="PaymentMode"></param>
        /// <param name="userConnect"></param>
        /// <returns></returns>
        public double ReturnCustomerMoney(CustomerReturn cr,int PaymentMode, EFDbContext ctx, int userConnect)
        {
            IDeposit depositRepo = new DepositRepository(ctx);

            double res = 0;
            
            //Argent déjà versé pour une vente
            double advancedAmount = depositRepo.SaleTotalPriceAdvance(cr.Sale);

            //Reste à payé sur la vente en considérant le retour courant 
            double saleRemaining = depositRepo.SaleRemainder(cr.Sale);

            //On ne travaille pas avec un reste à payer négatif qui arrive quand le client ayant pris les marchandises de 
            // 100 000, avancé 40 000, en retournant les marchandises de 80 000, un bug survient.
            //Cette ligne permet de corriger ce bug
            saleRemaining = (saleRemaining > 0) ? saleRemaining : 0;

            //Montant à rembourser au client sur la vente après le retour courant
            advancedAmount = advancedAmount - saleRemaining;

            if (advancedAmount > 0)//Le montant total des marchandises retournées est supérieur au montant d'argent déjà versé. Remboursement
            {
                double SliceAmount = 0;

                if (advancedAmount >= cr.TotalPriceReturn)//Il a droit à tout son argent
                {
                    SliceAmount = cr.TotalPriceReturn;
                }
                else//Il prend ce qui reste
                {
                    //On ne doit pas rembourser toute l'avance du client sans retirer le montant de la facture
                    //ce qui arrive quand le client ayant pris les marchandises de 
                    // 100 000, avancé 40 000, en retournant les marchandises de 80 000, un bug survient. on rembourse 40 000 au client au lieu de 20000
                    //Cette ligne permet de corriger ce bug
                    advancedAmount = (saleRemaining <= 0) ? (advancedAmount - depositRepo.SaleBill(cr.Sale)) : advancedAmount;
                    SliceAmount = advancedAmount;
                }

                if (SliceAmount > 0)
                {

                    CustomerSlice cslice = ctx.CustomerSlices.Where(c => c.SaleID == cr.Sale.SaleID).FirstOrDefault();
                    CustomerReturnSlice slice = new CustomerReturnSlice()
                    {
                        CustomerReturnID = cr.CustomerReturnID,
                        DeviseID = ctx.Devises.FirstOrDefault(dv => dv.DefaultDevise).DeviseID,
                        PaymentMethodID = PaymentMode,// ctx.Tills.FirstOrDefault(t => t.BranchID == cr.Sale.BranchID).ID,
                        SliceAmount = SliceAmount,
                        //SliceDate = bdRepo.GetOpenedBusinessDay().FirstOrDefault().BDDateOperation,
                        SliceDate = ctx.BusinessDays.Where(bd => (bd.BDStatut == true) && (bd.ClosingDayStarted == false)).FirstOrDefault().BDDateOperation,
                        Representant=cr.Sale?.CustomerName,
                        TransactionIdentifier= cslice?.TransactionIdentifier,
                        OperatorID=userConnect
                    };
                    _CustomerReturnSliceRepository.Create(slice);
                    ctx.SaveChanges();
                    res = SliceAmount;
                    //slice = ctx.CustomerReturnSlices.Add(slice);
                    //ctx.SaveChanges();
                    //res = slice.SliceAmount;
                    
                }


            }


            return res;
        }

        /// <summary>
        ///Etant donné que dans un système commercial, on ne rend jamais l'argent au client, cette méthode remet l'argent du client
        ///issue du dépôt dans son compte.
        /// L'argent rentre dans l'une des caisses de la branches dans laquelle la marchandise a été achétée.
        /// NB : On fait un dépôt de la somme déjà versée moins la somme des retours déjà reçus.
        /// C'est l'argent du dernier CustomerReturnSlice de cette vente ou de ce retour
        /// </summary>
        /// <param name="cr"></param>
        /// <param name="Amount"></param>
        /// <param name="ctx"></param>
        /// <param name="TillID"></param>
        /// <param name="userConnect"></param>
        public void SaveCustomerReturnAmount(CustomerReturn cr,int TillID, double Amount, EFDbContext ctx, int userConnect)
        {
            BusinessDay businessDay = bdRepo.GetOpenedBusinessDay().FirstOrDefault();
            //recuperation de la premiere caisse ouverte
            //TillDay tillDay = context.TillDays.FirstOrDefault(td => td.IsOpen);
            //if (tillDay == null)
            //{
            //    throw new Exception("At Least One Cash Register Must Open Before Proceed ");
            //}
                Deposit deposit = new Deposit()
                {
                    Amount = Amount,
                    BranchID = cr.Sale.BranchID,
                    CustomerID = cr.Sale.CustomerID.Value,
                    DepositDate = businessDay.BDDateOperation,
                    DeviseID = ctx.Devises.FirstOrDefault(dv => dv.DefaultDevise).DeviseID,
                    PaymentMethodID = TillID,//ctx.Tills.FirstOrDefault(t => t.BranchID == cr.Sale.BranchID).ID,
                    Representant = cr.Sale?.PoliceAssurance,
                    DepositReference = _transactNumbeRepository.returnTransactNumber("SADE", businessDay)
                };
                saRepo.DoADeposit(deposit, false, userConnect);
            //}
         }


        /// <summary>
        /// Quantité de la ligne qui a déjà été retournée
        /// </summary>
        /// <param name="selectedSaleLine"></param>
        /// <returns></returns>
        public double SaleLineReturnedQuantity(SaleLine selectedSaleLine)
        {
            List<CustomerReturnLine> returnList = (from crl in context.CustomerReturnLines
                                                   where crl.SaleLineID == selectedSaleLine.LineID
                                                   select crl).ToList();
                //_customerReturnLineRepository.FindAll.Where(crl => crl.SaleLineID == selectedSaleLine.LineID).ToList();
            return (returnList == null || returnList.Count() == 0) ? 0 : returnList.Select(r => r.LineQuantity).Sum();
        }

        /// <summary>
        /// Vérifie si la vente a au moins une ligne pouvant faire l'objet d'un retour
        /// </summary>
        /// <param name="sale"></param>
        /// <returns></returns>
        public bool IsSaleCanBeReturn(Sale sale)
        {
            bool res = true;
            CustomerReturn saleCR = this.FindAll.SingleOrDefault(cr => cr.SaleID == sale.SaleID);

            //il y a déjà eu au moins un retour sur cette vente
            if (saleCR != null && saleCR.CustomerReturnID > 0)
            {
                res = false;
                foreach (SaleLine sl in sale.SaleLines)
                {
                    if (IsAllLineReturn(sl) == false)
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
        public bool IsSaleCanBeReturn(Sale sale, List<CustomerReturnLine> customerReturnLines)
        {
            bool res = true;

            res = this.IsSaleCanBeReturn(sale);

            if (res == true)
            {
                res = false;
                //si quantité retournée + qté en cours de retour = qté de la ligne alors le retour est consommé
                foreach (CustomerReturnLine crl in customerReturnLines)
                {
                    SaleLine sl = context.SaleLines.Find(crl.SaleLineID);
                    if ((SaleLineReturnedQuantity(sl) + crl.LineQuantity) < sl.LineQuantity)
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
        /// <param name="sl"></param>
        /// <returns></returns>
        public bool IsAllLineReturn(SaleLine sl)
        {
            bool res = true;
            //Liste des retour
            List<CustomerReturnLine> saleLineCRList = context.CustomerReturnLines.Where(crl => crl.SaleLineID == sl.LineID).ToList();
            //Quantité déjà retournée sur cette ligne de vente
            double returnedQuantity = (saleLineCRList != null && saleLineCRList.Count > 0) ? saleLineCRList.Select(crl => crl.LineQuantity).Sum() : 0;

            if (returnedQuantity < sl.LineQuantity)
            {
                res = false;
            }

            return res;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="saleID"></param>
        /// <param name="CustomerReturnCauses"></param>
        /// <param name="UserConect"></param>
        /// <param name="BranchID"></param>
        /// <returns></returns>
        public bool ReturnAllSale(int saleID, string CustomerReturnCauses, int UserConect, int BranchID)
        {


            bool res = false;

            res = ReturnSale(GetCustRetFromSale(saleID, CustomerReturnCauses), UserConect,BranchID);

            return res;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="saleid"></param>
        /// <param name="CustomerReturnCauses"></param>
        /// <returns></returns>
        public CustomerReturn GetCustRetFromSale(int saleid, string CustomerReturnCauses)
        {

            Sale sale = context.Sales.Find(saleid);

            BusinessDay openedBD = bdRepo.GetOpenedBusinessDay().FirstOrDefault();
            DateTime BDDateOperation = openedBD.BDDateOperation;

            List<CustomerReturnLine> lines = new List<CustomerReturnLine>();

            foreach (SaleLine line in sale.SaleLines)
            {
                if (line.LineQuantity- SaleLineReturnedQuantity(line) > 0)
                {
                    lines.Add(GetCusRetLineFromSaleLine(line, CustomerReturnCauses, BDDateOperation));
                }
            }

            CustomerReturn custRet = new CustomerReturn()
            {
                CustomerReturnDate = BDDateOperation,
                SaleID = sale.SaleID,
                CustomerReturnLines = lines
            };


            return custRet;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sl"></param>
        /// <param name="CustomerReturnCauses"></param>
        /// <param name="BDDateOperation"></param>
        /// <returns></returns>
        public CustomerReturnLine GetCusRetLineFromSaleLine(SaleLine sl, string CustomerReturnCauses, DateTime BDDateOperation)
        {

            CustomerReturnLine crl = new CustomerReturnLine()
            {
                CustomerReturnCauses = CustomerReturnCauses,
                CustomerReturnDate = BDDateOperation,
                LineQuantity = sl.LineQuantity- SaleLineReturnedQuantity(sl),
                LineUnitPrice = sl.LineUnitPrice,// SaleLineReturnedQuantity(sl),
                LocalizationID = sl.LocalizationID,
                ProductID = sl.ProductID,
                SaleLineID = sl.LineID,
            };

            return crl;
        }

        /// <summary>
        /// Cette méthode retourne une vente qui, si elle a subit un retour, les quantités retournées ou les produits retournées sont prises en charge
        /// </summary>
        /// <param name="sale"></param>
        /// <returns></returns>
        public Sale GetRealSale(Sale sale)
        {
            Sale realSale = new Sale();
            realSale = sale;
            //realSale.SaleLines.Clear();
            if (this.IsCustomerReturnExist(sale.SaleID) == true)
            {
                //realSale.SaleLines.Clear();
                List<SaleLine> reemoveSaleLine = new List<SaleLine>();
                foreach (SaleLine sl in sale.SaleLines)
                {
                    sl.LineQuantity = sl.LineQuantity - SaleLineReturnedQuantity(sl);
                    
                    if (sl.LineQuantity<=0)
                    {
                        //realSale.SaleLines.Remove(sl);
                        reemoveSaleLine.Add(sl);
                    }
                    //if (IsAllLineReturn(sl) == false)
                    //{
                    //    sl.LineQuantity = sl.LineQuantity - SaleLineReturnedQuantity(sl);
                    //    realSale.SaleLines.Add(sl);
                    //}
                }
                foreach(SaleLine sl in reemoveSaleLine)
                {
                    realSale.SaleLines.Remove(sl);
                }
            }

            return realSale;

        }
        /// <summary>
        /// Cette méthode retourne une vente qui, si elle a subit un retour, les quantités retournées ou les produits retournées sont prises en charge
        /// </summary>
        /// <param name="saleID">Vente</param>
        /// <returns>Retourne la vente réelle</returns>
        public Sale GetRealSale(int saleID)
        {
            return this.GetRealSale(context.Sales.AsNoTracking().SingleOrDefault(s => s.SaleID == saleID));
        }

    }
}
