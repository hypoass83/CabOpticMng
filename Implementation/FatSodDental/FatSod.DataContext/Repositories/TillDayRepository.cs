using System;
using System.Collections.Generic;
using System.Linq;
using FatSod.Supply.Abstracts;
using FatSod.Supply.Entities;
using FatSod.Security.Entities;
using FatSod.Security.Abstracts;
using FatSod.Budget.Abstracts;
using FatSod.DataContext.Initializer;
using FatSod.Ressources;
using System.Transactions;
using FatSod.Budget.Entities;

namespace FatSod.DataContext.Repositories
{
    /// <summary>
    /// 
    /// </summary>
    public class TillDayRepository : RepositorySupply<TillDay>, ITillDay
    {

        /// <summary>
        /// Cette méthode retourne le montant qui est dans la caisse à un instant t d'une journée précise
        /// </summary>
        /// <param name="till">Caisse concernée</param>
        /// <returns>Montant présent dans la caisse à cet instant</returns>
        //public Double TillBalance(Till till)
        //{
        //    IBusinessDay _busDayRepo = new BusinessDayRepository();
        //    ISale _saleRepository = new SaleRepository();
        //    IPurchase _purchaseRepository = new PurchaseRepository(this.context);
        //    IDeposit _depositRepository = new DepositRepository();
        //    IBudgetConsumption _bdgetCumptnRepository = new BudgetConsumptionRepository();

        //    Branch currentBranch = till.Branch;
        //    BusinessDay businessDay = _busDayRepo.GetOpenedBusinessDay(currentBranch);
        //    DateTime businessDayDate = businessDay.BDDateOperation;
        //    TillDay currentTillDay = this.FindAll.SingleOrDefault(t =>
        //                                                                                                                            t.TillDayDate.Day == businessDayDate.Day &&
        //                                                                                                                            t.TillDayDate.Month == businessDayDate.Month &&
        //                                                                                                                            t.TillDayDate.Year == businessDayDate.Year &&
        //                                                                                                                            t.IsOpen &&
        //                                                                                                                            t.TillID == till.ID
        //            );

        //    double intPutCash = 0;
        //    double outPutCash = 0;
        //    double balanceTill = 0;

        //    _saleRepository.FindAll.OfType<TillSale>().Where(s => s.SaleDate.Day == businessDayDate.Day &&
        //                                                                                                                s.SaleDate.Month == businessDayDate.Month &&
        //                                                                                                                s.SaleDate.Year == businessDayDate.Year).ToList().ForEach(sd =>
        //    {
        //        intPutCash += _depositRepository.SaleTotalPriceAdvance(sd);
        //    });


        //    _purchaseRepository.FindAll.OfType<TillPurchase>().Where(s => s.PurchaseDate.Day == businessDayDate.Day &&
        //                                                                                                                                s.PurchaseDate.Month == businessDayDate.Month &&
        //                                                                                                                                s.PurchaseDate.Year == businessDayDate.Year).ToList().ForEach(sd =>
        //    {
        //        outPutCash = _depositRepository.PurchaseTotalPriceAdvance(sd);
        //    });
        //    //depense budgetaire
        //    _bdgetCumptnRepository.FindAll.Where(b => b.PaymentDate.HasValue && b.PaymentDate.Value.Date == businessDayDate.Date && b.PaymentMethod is Till).ToList().ForEach(sd =>
        //    {
        //        outPutCash += sd.VoucherAmount;
        //    });

        //    balanceTill = intPutCash - outPutCash + currentTillDay.TillDayOpenPrice;

        //    return balanceTill;
        //}


        /// <summary>
        /// Cette méthode retourne le status d'une caisse, ses entrées, ses sorties, ainsi que sa balance
        /// qui est dans la caisse à un instant t d'une journée précise
        /// </summary>
        /// <param name="tillID">Caisse concernée</param>
        /// <returns>Montant présent dans la caisse à cet instant</returns>
        public TillSatut TillStatus(int tillID)
        {

            //TillSatut tillstatus;
            IBusinessDay _busDayRepo = new BusinessDayRepository();
            ISale _saleRepository = new SaleRepository();
            IPurchase _purchaseRepository = new PurchaseRepository(this.context);
            IDeposit _depositRepository = new DepositRepository();
            IRepositorySupply<Slice> _sliceRepository = new RepositorySupply<Slice>();

            IBudgetConsumption _bdgetCumptnRepository = new BudgetConsumptionRepository();
            ITillAdjust _tillAdjust = new TillAdjustRepository();
            
            //Devise currentCurrency = till.de
            Till till = context.Tills.Find(tillID);

            Branch currentBranch = till.Branch;
            BusinessDay businessDay = _busDayRepo.GetOpenedBusinessDay(currentBranch);


            TillDay currentTillDay = this.FindAll.SingleOrDefault(t =>
                    t.TillID == till.ID && t.IsOpen &&
                    t.TillDayDate.Year == businessDay.BDDateOperation.Year &&
                    t.TillDayDate.Month == businessDay.BDDateOperation.Month &&
                    t.TillDayDate.Day == businessDay.BDDateOperation.Day
                    );



            var businessDayDate = businessDay.BDDateOperation;

            double intPutCash = 0d;
            double sliceCustomerAmount = 0d;
            double depositAmount = 0d;
            double receiveCaisse = 0d;
            double receiveBanque = 0d;
            double retourAchat = 0d;
            double overage = 0d;
            

            double outPutCash = 0d;
            double sliceSupplierAmount = 0d;
            double budgetAmount = 0d;
            double sendCaisse = 0d;
            double sendBanque = 0d;
            double retourVente = 0d;
            double shortage = 0;
            

            double balanceTill = 0d;
            //double ecart = 0d;

            //traitement des entrees
            //1 - recuperation des sclices de vente des la jnee qui ont ete regle par caisse
            List<CustomerSlice> lstCustSlice = _sliceRepository.FindAll.OfType<CustomerSlice>().Where(sl => sl.SliceDate.Date == businessDayDate.Date && (/*sl.PaymentMethod is Till && */sl.PaymentMethodID == till.ID) && !sl.isDeposit).ToList();
            lstCustSlice.ForEach(sl =>
            {
                //exclure les retours
                sliceCustomerAmount += sl.SliceAmount;
                //else sliceCustomerAmount += sl.SliceAmount - retSlices.SliceAmount;
            });

            //3 - recuperation des depots d'epargne
            List<AllDeposit> lstDeposit = context.AllDeposits.Where(d => d.AllDepositDate == businessDayDate.Date && (/*d.PaymentMethod1 is Till && */d.PaymentMethodID == till.ID)).ToList();
            lstDeposit.ForEach(ld =>
            {
                depositAmount += ld.Amount;
            });
            //recupartion des versements caisse

            //recuperation des versement bancaire

            //recuperation retour achat

            //4 - Les ajustements positifs de la caisse = TillAdjust
            List<TillAdjust> allDayAdjusts = _tillAdjust.FindAll.Where(t => t.TillAdjustDate.Date == businessDayDate.Date && t.TillID == till.ID && ((t.PhysicalPrice > t.ComputerPrice))).ToList();
            allDayAdjusts.ForEach(ta =>
            {
                overage += (ta.PhysicalPrice - ta.ComputerPrice);
            });
            
            intPutCash = sliceCustomerAmount + depositAmount + receiveCaisse + receiveBanque + retourAchat + overage;

            //traitement des sorties

            //1 - recuperation des sclices d'achat des la jnee qui ont ete regle par caisse
            List<SupplierSlice> lstSupSlice = _sliceRepository.FindAll.OfType<SupplierSlice>().Where(sl => sl.SliceDate.Date == businessDayDate.Date && (/*sl.PaymentMethod is Till && */sl.PaymentMethodID == till.ID)).ToList();
            lstSupSlice.ForEach(sl =>
            {
                sliceSupplierAmount += sl.SliceAmount;
            });

            //2 - recupartion des consommation du budget
            _bdgetCumptnRepository.FindAll.Where(b => b.PaymentDate.HasValue && b.PaymentDate.Value.Date == businessDayDate.Date && (/*b.PaymentMethod is Till && */b.PaymentMethodID == till.ID)).ToList().ForEach(sd =>
            {
                budgetAmount += sd.VoucherAmount;
            });
            //recuperation des sorties vers caisse

            //recuperation des sorties vers banque
            List<TreasuryOperation> lstTranfbank = (from t in context.TreasuryOperations
                                                    where ((t.OperationDate.Day == businessDayDate.Day && t.OperationDate.Month == businessDayDate.Month && t.OperationDate.Year == businessDayDate.Year) && t.TillID == till.ID && t.OperationType == CodeValue.Accounting.TreasuryOperation.TransfertToBank)
                                                    select t).AsQueryable().ToList();
            lstTranfbank.ForEach(s =>
            {
                sendBanque += s.OperationAmount;
            }
                );

            // recuperation des manquants de la caisse

            //3 - Les déficites de caisse de la journée
            List<TillAdjust> allDayShortageAdjusts = _tillAdjust.FindAll.Where(t => t.TillAdjustDate.Date == businessDayDate.Date && t.TillID == till.ID && ((t.PhysicalPrice < t.ComputerPrice))).ToList();

            allDayShortageAdjusts.ForEach(ta =>
            {
                shortage += ta.ComputerPrice - ta.PhysicalPrice;

            });

            //4 - Liste des retours sur vente de la journée
            List<CustomerReturnSlice> returnSlices = _sliceRepository.FindAll.OfType<CustomerReturnSlice>().Where(sl => sl.SliceDate.Date == businessDayDate.Date && (/*sl.PaymentMethod is Till && */sl.PaymentMethodID == till.ID)).ToList();
            returnSlices.ForEach(ta =>
            {
                retourVente += ta.SliceAmount;
            });
            

            outPutCash = sliceSupplierAmount + budgetAmount + sendCaisse + sendBanque + retourVente + shortage ;


            if (currentTillDay == null)
            {
                balanceTill = intPutCash - outPutCash;
                return new TillSatut() { Inputs = intPutCash, Ouputs = outPutCash, Ballance = balanceTill, OpenningPrice = 0, ClosiningPrice = 0 };
            }
            else
            {
                balanceTill = currentTillDay.TillDayOpenPrice + intPutCash - outPutCash;
                return new TillSatut() { Inputs = intPutCash, Ouputs = outPutCash, Ballance = balanceTill, OpenningPrice = currentTillDay.TillDayOpenPrice, ClosiningPrice = currentTillDay.TillDayClosingPrice };
            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="till"></param>
        /// <param name="bussinessDayDate"></param>
        /// <returns></returns>
        public TillSatut TillStatus(Till till, DateTime bussinessDayDate)
        {
            //TillSatut tillstatus;
            
            ISale _saleRepository = new SaleRepository();
            IPurchase _purchaseRepository = new PurchaseRepository(this.context);
            IDeposit _depositRepository = new DepositRepository();
            IRepositorySupply<Slice> _sliceRepository = new RepositorySupply<Slice>();

            IBudgetConsumption _bdgetCumptnRepository = new BudgetConsumptionRepository();
            ITillAdjust _tillAdjust = new TillAdjustRepository();

            //Devise currentCurrency = till.de

            Branch currentBranch = till.Branch;

            DateTime businessDayDate = bussinessDayDate;

            TillDay currentTillDay = this.FindAll.SingleOrDefault(t =>
                    t.TillID == till.ID /*&& t.IsOpen*/ &&
                    t.TillDayDate.Year == businessDayDate.Year &&
                    t.TillDayDate.Month == businessDayDate.Month &&
                    t.TillDayDate.Day == businessDayDate.Day
                    );


            double intPutCash = 0d;
            double sliceCustomerAmount = 0d;
            double depositAmount = 0d;
            double receiveCaisse = 0d;
            double receiveBanque = 0d;
            double retourAchat = 0d;
            double overage = 0d;

            double outPutCash = 0d;
            double sliceSupplierAmount = 0d;
            double budgetAmount = 0d;
            double sendCaisse = 0d;
            double sendBanque = 0d;
            double retourVente = 0d;
            double shortage = 0;

            double balanceTill = 0d;
            //double ecart = 0d;

            //traitement des entrees
            //1 - recuperation des sclices de vente des la jnee qui ont ete regle par caisse
            List<CustomerSlice> lstCustSlice = _sliceRepository.FindAll.OfType<CustomerSlice>().Where(sl => sl.SliceDate.Date == businessDayDate.Date && (/*sl.PaymentMethod is Till && */sl.PaymentMethodID == till.ID) && !sl.isDeposit).ToList();
            lstCustSlice.ForEach(sl =>
            {
                //exclure les retours
                sliceCustomerAmount += sl.SliceAmount;
                //else sliceCustomerAmount += sl.SliceAmount - retSlices.SliceAmount;
            });

            //3 - recuperation des depots d'epargne
            List<AllDeposit> lstDeposit = context.AllDeposits.Where(d => d.AllDepositDate == businessDayDate.Date && (/*d.PaymentMethod1 is Till && */d.PaymentMethodID == till.ID)).ToList();
            lstDeposit.ForEach(ld =>
            {
                depositAmount += ld.Amount;
            });
            //recupartion des versements caisse

            //recuperation des versement bancaire

            //recuperation retour achat

            //4 - Les ajustements positifs de la caisse = TillAdjust
            List<TillAdjust> allDayAdjusts = _tillAdjust.FindAll.Where(t => t.TillAdjustDate.Date == businessDayDate.Date && t.TillID == till.ID && ((t.PhysicalPrice > t.ComputerPrice))).ToList();
            allDayAdjusts.ForEach(ta =>
            {
                overage += (ta.PhysicalPrice - ta.ComputerPrice);
            });

            intPutCash = sliceCustomerAmount + depositAmount + receiveCaisse + receiveBanque + retourAchat + overage;

            //traitement des sorties

            //1 - recuperation des sclices d'achat des la jnee qui ont ete regle par caisse
            List<SupplierSlice> lstSupSlice = _sliceRepository.FindAll.OfType<SupplierSlice>().Where(sl => sl.SliceDate.Date == businessDayDate.Date && (/*sl.PaymentMethod is Till && */sl.PaymentMethodID == till.ID)).ToList();
            lstSupSlice.ForEach(sl =>
            {
                sliceSupplierAmount += sl.SliceAmount;
            });

            //2 - recupartion des consommation du budget
            _bdgetCumptnRepository.FindAll.Where(b => b.PaymentDate.HasValue && b.PaymentDate.Value.Date == businessDayDate.Date && (/*b.PaymentMethod is Till && */b.PaymentMethodID == till.ID)).ToList().ForEach(sd =>
            {
                budgetAmount += sd.VoucherAmount;
            });
            //recuperation des sorties vers caisse

            //recuperation des sorties vers banque
            List<TreasuryOperation> lstTranfbank = (from t in context.TreasuryOperations
                                                    where ((t.OperationDate.Day == businessDayDate.Day && t.OperationDate.Month == businessDayDate.Month && t.OperationDate.Year == businessDayDate.Year) && t.TillID == till.ID && t.OperationType == CodeValue.Accounting.TreasuryOperation.TransfertToBank)
                                                    select t).AsQueryable().ToList();
            lstTranfbank.ForEach(s =>
            {
                sendBanque += s.OperationAmount;
            }
                );

            // recuperation des manquants de la caisse

            //3 - Les déficites de caisse de la journée
            List<TillAdjust> allDayShortageAdjusts = _tillAdjust.FindAll.Where(t => t.TillAdjustDate.Date == businessDayDate.Date && t.TillID == till.ID && ((t.PhysicalPrice < t.ComputerPrice))).ToList();

            allDayShortageAdjusts.ForEach(ta =>
            {
                shortage += ta.ComputerPrice - ta.PhysicalPrice;

            });

            //4 - Liste des retours sur vente de la journée
            List<CustomerReturnSlice> returnSlices = _sliceRepository.FindAll.OfType<CustomerReturnSlice>().Where(sl => sl.SliceDate.Date == businessDayDate.Date && (/*sl.PaymentMethod is Till && */sl.PaymentMethodID == till.ID)).ToList();
            returnSlices.ForEach(ta =>
            {
                retourVente += ta.SliceAmount;
            });


            outPutCash = sliceSupplierAmount + budgetAmount + sendCaisse + sendBanque + retourVente + shortage;


            if (currentTillDay == null)
            {
                balanceTill = intPutCash - outPutCash;
                return new TillSatut() { Inputs = intPutCash, Ouputs = outPutCash, Ballance = balanceTill, OpenningPrice = 0, ClosiningPrice = 0 };
            }
            else
            {
                balanceTill = currentTillDay.TillDayOpenPrice + intPutCash - outPutCash;
                return new TillSatut() { Inputs = intPutCash, Ouputs = outPutCash, Ballance = balanceTill, OpenningPrice = currentTillDay.TillDayOpenPrice, ClosiningPrice = currentTillDay.TillDayClosingPrice };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="till"></param>
        /// <param name="bussinessDayDate"></param>
        /// <param name="LastClosingAmount"></param>
        /// <returns></returns>
        public TillSatut TillStatus(Till till, DateTime bussinessDayDate, double LastClosingAmount)
        {
            //TillSatut tillstatus;

            ISale _saleRepository = new SaleRepository();
            IPurchase _purchaseRepository = new PurchaseRepository(this.context);
            IDeposit _depositRepository = new DepositRepository();
            IRepositorySupply<Slice> _sliceRepository = new RepositorySupply<Slice>();

            IBudgetConsumption _bdgetCumptnRepository = new BudgetConsumptionRepository();
            ITillAdjust _tillAdjust = new TillAdjustRepository();

            //Devise currentCurrency = till.de

            Branch currentBranch = till.Branch;

            DateTime businessDayDate = bussinessDayDate;

            TillDay currentTillDay = this.FindAll.SingleOrDefault(t =>
                    t.TillID == till.ID /*&& t.IsOpen*/ &&
                    t.TillDayDate.Year == businessDayDate.Year &&
                    t.TillDayDate.Month == businessDayDate.Month &&
                    t.TillDayDate.Day == businessDayDate.Day
                    );


            double intPutCash = 0d;
            double sliceCustomerAmount = 0d;
            double depositAmount = 0d;
            double receiveCaisse = 0d;
            double receiveBanque = 0d;
            double retourAchat = 0d;
            double overage = 0d;

            double outPutCash = 0d;
            double sliceSupplierAmount = 0d;
            double budgetAmount = 0d;
            double sendCaisse = 0d;
            double sendBanque = 0d;
            double retourVente = 0d;
            double shortage = 0;

            double balanceTill = 0d;
            //double ecart = 0d;

            //traitement des entrees
            //1 - recuperation des sclices de vente des la jnee qui ont ete regle par caisse
            List<CustomerSlice> lstCustSlice = _sliceRepository.FindAll.OfType<CustomerSlice>().Where(sl => sl.SliceDate.Date == businessDayDate.Date && (/*sl.PaymentMethod is Till && */sl.PaymentMethodID == till.ID) && !sl.isDeposit).ToList();
            lstCustSlice.ForEach(sl =>
            {
                //exclure les retours
                sliceCustomerAmount += sl.SliceAmount;
                //else sliceCustomerAmount += sl.SliceAmount - retSlices.SliceAmount;
            });

            //3 - recuperation des depots d'epargne
            List<AllDeposit> lstDeposit = context.AllDeposits.Where(d => d.AllDepositDate == businessDayDate.Date && (/*d.PaymentMethod1 is Till && */d.PaymentMethodID == till.ID)).ToList();
            lstDeposit.ForEach(ld =>
            {
                depositAmount += ld.Amount;
            });
            //recupartion des versements caisse

            //recuperation des versement bancaire

            //recuperation retour achat

            //4 - Les ajustements positifs de la caisse = TillAdjust
            List<TillAdjust> allDayAdjusts = _tillAdjust.FindAll.Where(t => t.TillAdjustDate.Date == businessDayDate.Date && t.TillID == till.ID && ((t.PhysicalPrice > t.ComputerPrice))).ToList();
            allDayAdjusts.ForEach(ta =>
            {
                overage += (ta.PhysicalPrice - ta.ComputerPrice);
            });

            intPutCash = sliceCustomerAmount + depositAmount + receiveCaisse + receiveBanque + retourAchat + overage;

            //traitement des sorties

            //1 - recuperation des sclices d'achat des la jnee qui ont ete regle par caisse
            List<SupplierSlice> lstSupSlice = _sliceRepository.FindAll.OfType<SupplierSlice>().Where(sl => sl.SliceDate.Date == businessDayDate.Date && (/*sl.PaymentMethod is Till && */sl.PaymentMethodID == till.ID)).ToList();
            lstSupSlice.ForEach(sl =>
            {
                sliceSupplierAmount += sl.SliceAmount;
            });

            //2 - recupartion des consommation du budget
            _bdgetCumptnRepository.FindAll.Where(b => b.PaymentDate.HasValue && b.PaymentDate.Value.Date == businessDayDate.Date && (/*b.PaymentMethod is Till && */b.PaymentMethodID == till.ID)).ToList().ForEach(sd =>
            {
                budgetAmount += sd.VoucherAmount;
            });
            //recuperation des sorties vers caisse

            //recuperation des sorties vers banque
            List<TreasuryOperation> lstTranfbank = (from t in context.TreasuryOperations
                                                    where ((t.OperationDate.Day == businessDayDate.Day && t.OperationDate.Month == businessDayDate.Month && t.OperationDate.Year == businessDayDate.Year) && t.TillID == till.ID && t.OperationType == CodeValue.Accounting.TreasuryOperation.TransfertToBank)
                                                    select t).AsQueryable().ToList();
            lstTranfbank.ForEach(s =>
            {
                sendBanque += s.OperationAmount;
            }
                );

            // recuperation des manquants de la caisse

            //3 - Les déficites de caisse de la journée
            List<TillAdjust> allDayShortageAdjusts = _tillAdjust.FindAll.Where(t => t.TillAdjustDate.Date == businessDayDate.Date && t.TillID == till.ID && ((t.PhysicalPrice < t.ComputerPrice))).ToList();

            allDayShortageAdjusts.ForEach(ta =>
            {
                shortage += ta.ComputerPrice - ta.PhysicalPrice;

            });

            //4 - Liste des retours sur vente de la journée
            List<CustomerReturnSlice> returnSlices = _sliceRepository.FindAll.OfType<CustomerReturnSlice>().Where(sl => sl.SliceDate.Date == businessDayDate.Date && (/*sl.PaymentMethod is Till && */sl.PaymentMethodID == till.ID)).ToList();
            returnSlices.ForEach(ta =>
            {
                retourVente += ta.SliceAmount;
            });


            outPutCash = sliceSupplierAmount + budgetAmount + sendCaisse + sendBanque + retourVente + shortage;


            if (currentTillDay == null)
            {
                balanceTill = intPutCash - outPutCash;
                return new TillSatut() { Inputs = intPutCash, Ouputs = outPutCash, Ballance = balanceTill, OpenningPrice = 0, ClosiningPrice = 0 };
            }
            else
            {
                balanceTill = LastClosingAmount + intPutCash - outPutCash;
                return new TillSatut() { Inputs = intPutCash, Ouputs = outPutCash, Ballance = balanceTill, OpenningPrice = currentTillDay.TillDayOpenPrice, ClosiningPrice = currentTillDay.TillDayClosingPrice };
            }
        }

        /// <summary>
        /// Cette méthode répond à la question est ce que cette caisse est ouverte ce jour
        /// </summary>
        /// <param name="till"></param>
        /// <param name="bussinessDayDate"></param>
        /// <returns></returns>
        public bool IsTillOpened(Till till, DateTime bussinessDayDate)
        {

            TillDay tday = context.TillDays.Where(td => td.TillID == till.ID).ToList().SingleOrDefault(td1 => (td1.TillDayDate.Day == bussinessDayDate.Day && td1.TillDayDate.Month == bussinessDayDate.Month && td1.TillDayDate.Year == bussinessDayDate.Year));

            return (tday != null && tday.TillDayID > 0) ? tday.IsOpen : false;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tillID"></param>
        /// <returns></returns>
        public TillDayStatus TillDayStatus(int tillID)
        {
            TillDayStatus tStatus = new TillDayStatus();
            tStatus = context.TillDayStatus.SingleOrDefault(td => td.TillID == tillID);
            return (tStatus != null) ? tStatus : null;

        }

        /// <summary>
        /// ouverture de la caisse en debut de journée
        /// </summary>
        /// <param name="tillDay"></param>
        /// <param name="YesterdayClosingPrice"></param>
        /// <param name="CashInitialization"></param>
        /// <param name="UserConnect"></param>
        /// <param name="BusinessDate"></param>
        /// <param name="BranchID"></param>
        /// <returns></returns>
        public bool OpenDay(TillDay tillDay, double YesterdayClosingPrice, double CashInitialization, int UserConnect, DateTime BusinessDate, int BranchID)
        {
            bool res = false;
            bool resMouch = false;
            try
            {
                //EFDbContext context = new EFDbContext();
                using (TransactionScope ts = new TransactionScope())
                {
                    //tillDay.IsOpen = true;
                    //tillDay.TillDayClosingPrice = 0;

                    IMouchar _opSneak = new MoucharRepository(context);
                    //verification du status de la caisse
                    TillDayStatus tState = TillDayStatus(tillDay.TillID);
                    if (tState == null)
                    {
                        res = false;
                        throw new Exception("Bad Configuration of Cash Register!!! Please call Your database Administrator");
                    }
                    //verifions si cette caisse a deja été ouverte au courant de cette journée
                    List<TillDay> verifNbreOpenTill = context.TillDays.Where(t => t.TillID == tillDay.TillID && t.TillDayDate == BusinessDate.Date).ToList();
                    if (verifNbreOpenTill.Count > 0)
                    {
                        throw new Exception("You cannot open your cash more than one time in the same Business Day !!! Please call your administrator");
                    }
                    //if (tState.IsOpen)
                    //{
                    //    res = false;
                    //    throw new Exception("This Cash Register is Still Open!!! Please Close It Before Proceed");
                    //}
                    if (CashInitialization == 1)    //premiere fois
                    {
                        //VERIFICATION DE L'UNICITE
                        List<TillDay> verifTilDay = context.TillDays.Where(t => t.TillID == tillDay.TillID && t.TillDayDate == BusinessDate.Date).ToList();
                        if (verifTilDay.Count > 1)
                        {
                            throw new Exception("More that one Cash Register is Open Please call Your database Administrator to fix this problem");
                        }
                        if (verifTilDay.Count == 1)
                        {
                            //this.Update(tillDay, tillDay.TillDayID);
                            TillDay OpenTillDay = context.TillDays.SingleOrDefault(t => t.TillID == tillDay.TillID && t.TillDayDate == BusinessDate.Date);
                            OpenTillDay.TillDayOpenPrice = YesterdayClosingPrice;
                            OpenTillDay.TillDayClosingPrice = 0;
                            OpenTillDay.IsOpen = true;
                            context.SaveChanges();
                        }
                        else
                        {
                            //this.Create(tillDay);
                            TillDay OpenTillDayFirst = new TillDay();
                            OpenTillDayFirst.TillDayOpenPrice = YesterdayClosingPrice;
                            OpenTillDayFirst.TillDayClosingPrice = 0;
                            OpenTillDayFirst.IsOpen = true;
                            OpenTillDayFirst.TillDayDate = BusinessDate.Date;
                            OpenTillDayFirst.TillID = tillDay.TillID;
                            context.TillDays.Add(OpenTillDayFirst);
                            context.SaveChanges();
                        }
                        resMouch = _opSneak.InsertOperation(UserConnect, "SUCCESS", "OPEN CASH REGISTER FOR CASHIER", "OpenDay", BusinessDate, BranchID);
                        if (!resMouch)
                        {
                            res = false;
                            throw new Exception("Une erreur s'est produite lors de la journalisation ");
                        }
                    }
                    else
                    {
                        TillDay ClosingTillDay = this.FindAll.SingleOrDefault(td => td.TillID == tillDay.TillID && td.TillDayDate.Date == tState.TillDayLastClosingDate.Date); // this.FindAll.LastOrDefault(td => td.TillID == tillDay.TillID && !td.IsOpen);
                        if (ClosingTillDay != null)
                        {
                            if (ClosingTillDay.TillDayDate.Day == tillDay.TillDayDate.Day && ClosingTillDay.TillDayDate.Month == tillDay.TillDayDate.Month && ClosingTillDay.TillDayDate.Year == tillDay.TillDayDate.Year)
                            {
                                if (tillDay.TillDayOpenPrice == YesterdayClosingPrice)
                                {
                                    //ClosingTillDay.TillDayClosingPrice = 0;
                                    ClosingTillDay.IsOpen = true;
                                    this.Update(ClosingTillDay, ClosingTillDay.TillDayID);
                                    resMouch = _opSneak.InsertOperation(UserConnect, "SUCCESS", "OPEN CASH REGISTER FOR CASHIER", "OpenDay", BusinessDate, BranchID);
                                    if (!resMouch)
                                    {
                                        res = false;
                                        throw new Exception("Une erreur s'est produite lors de la journalisation ");
                                    }
                                }
                                else
                                {
                                    resMouch = _opSneak.InsertOperation(UserConnect, "ERROR", Resources.MsgOpenTellerErrAmt, "OpenDay", BusinessDate, BranchID);
                                    if (!resMouch)
                                    {
                                        throw new Exception("Une erreur s'est produite lors de la journalisation ");
                                    }
                                    res = false;
                                    throw new Exception(Resources.MsgOpenTellerErrAmt);
                                }
                            }
                            else
                            {
                                if (tillDay.TillDayOpenPrice == YesterdayClosingPrice)
                                {
                                    //VERIFICATION DE L'UNICITE
                                    List<TillDay> verifTilDay = context.TillDays.Where(t => t.TillID == tillDay.TillID && t.TillDayDate == BusinessDate.Date).ToList();
                                    if (verifTilDay.Count > 1)
                                    {
                                        res = false;
                                        throw new Exception("More that one Cash Register Open Please call the database Administrator to fix this problem");
                                    }
                                    if (verifTilDay.Count == 1)
                                    {

                                        //int TillDayID = verifTilDay.FirstOrDefault().TillDayID;
                                        TillDay OpenTillDay = context.TillDays.SingleOrDefault(t => t.TillID == tillDay.TillID && t.TillDayDate == BusinessDate.Date);
                                        OpenTillDay.TillDayOpenPrice = YesterdayClosingPrice;
                                        OpenTillDay.TillDayClosingPrice = 0;
                                        OpenTillDay.IsOpen = true;
                                        context.SaveChanges();
                                        //this.Update(tillDay, TillDayID);
                                    }
                                    else
                                    {
                                        //tillDay.IsOpen = true;
                                        //this.Create(tillDay);

                                        TillDay OpenTillDayFirst = new TillDay();
                                        OpenTillDayFirst.TillDayOpenPrice = 0;//YesterdayClosingPrice;
                                        OpenTillDayFirst.TillDayClosingPrice = 0;
                                        OpenTillDayFirst.IsOpen = true;
                                        OpenTillDayFirst.TillDayDate = BusinessDate.Date;
                                        OpenTillDayFirst.TillID = tillDay.TillID;
                                        context.TillDays.Add(OpenTillDayFirst);
                                        context.SaveChanges();
                                    }

                                    resMouch = _opSneak.InsertOperation(UserConnect, "SUCCESS", "OPEN CASH REGISTER FOR CASHIER", "OpenDay", BusinessDate, BranchID);
                                    if (!resMouch)
                                    {
                                        res = false;
                                        throw new Exception("Une erreur s'est produite lors de la journalisation ");
                                    }

                                }
                                else //si les montants ne correpondent pas
                                {
                                    resMouch = _opSneak.InsertOperation(UserConnect, "ERROR", Resources.MsgOpenTellerErrAmt, "OpenDay", BusinessDate, BranchID);
                                    if (!resMouch)
                                    {
                                        throw new Exception("Une erreur s'est produite lors de la journalisation ");
                                    }
                                    res = false;
                                    throw new Exception(Resources.MsgOpenTellerErrAmt);
                                }
                            }
                        }
                        else
                        {
                            if (tillDay.TillDayOpenPrice == YesterdayClosingPrice)
                            {
                                //VERIFICATION DE L'UNICITE
                                List<TillDay> verifTilDay = context.TillDays.Where(t => t.TillID == tillDay.TillID && t.TillDayDate == BusinessDate.Date).ToList();
                                if (verifTilDay.Count > 1)
                                {
                                    res = false;
                                    throw new Exception("More that one Cash Register Open Please call the database Administrator to fix this problem");
                                }
                                if (verifTilDay.Count == 1)
                                {

                                    //int TillDayID = verifTilDay.FirstOrDefault().TillDayID;
                                    TillDay OpenTillDay = context.TillDays.SingleOrDefault(t => t.TillID == tillDay.TillID && t.TillDayDate == BusinessDate.Date);
                                    OpenTillDay.TillDayOpenPrice = YesterdayClosingPrice;
                                    OpenTillDay.TillDayClosingPrice = 0;
                                    OpenTillDay.IsOpen = true;
                                    context.SaveChanges();
                                    //this.Update(tillDay, TillDayID);
                                }
                                else
                                {
                                    //tillDay.IsOpen = true;
                                    //this.Create(tillDay);

                                    TillDay OpenTillDayFirst = new TillDay();
                                    OpenTillDayFirst.TillDayOpenPrice = 0;//YesterdayClosingPrice;
                                    OpenTillDayFirst.TillDayClosingPrice = 0;
                                    OpenTillDayFirst.IsOpen = true;
                                    OpenTillDayFirst.TillDayDate = BusinessDate.Date;
                                    OpenTillDayFirst.TillID = tillDay.TillID;
                                    context.TillDays.Add(OpenTillDayFirst);
                                    context.SaveChanges();
                                }

                                resMouch = _opSneak.InsertOperation(UserConnect, "SUCCESS", "OPEN CASH REGISTER FOR CASHIER", "OpenDay", BusinessDate, BranchID);
                                if (!resMouch)
                                {
                                    res = false;
                                    throw new Exception("Une erreur s'est produite lors de la journalisation ");
                                }

                            }
                            else
                            {
                                resMouch = _opSneak.InsertOperation(UserConnect, "ERROR", Resources.MsgOpenTellerErrAmt, "OpenDay", BusinessDate, BranchID);
                                if (!resMouch)
                                {
                                    throw new Exception("Une erreur s'est produite lors de la journalisation ");
                                }
                                res = false;
                                throw new Exception(Resources.MsgOpenTellerErrAmt);
                            }
                        }

                    }
                    //mise a our de la table des statu de caisse
                    tState.IsOpen = true;
                    tState.TillDayLastOpenDate = BusinessDate.Date;
                    context.SaveChanges();

                    res = true;
                    ts.Complete();
                }
            }
            catch (Exception e)
            {
                res = false;
                throw new Exception("Une erreur s'est produite lors de l'ouverture de la caisse : " + "e.Message = " + e.Message);
            }

            return res;
        }

        

        /// <summary>
        /// methode de fermeture de la caisse en prenant en compte les differentes verifications de fin de journée
        /// </summary>
        /// <param name="tillDay"></param>
        /// <param name="InputCash"></param>
        /// <param name="OutputCash"></param>
        /// <param name="YesterdayTillDayClosingPrice"></param>
        /// <param name="UserConnect"></param>
        /// <param name="BusinessDate"></param>
        /// <param name="BranchID"></param>
        /// <returns></returns>
        public bool CloseDay(TillDay tillDay, double InputCash, double OutputCash, double YesterdayTillDayClosingPrice, int UserConnect, DateTime BusinessDate, int BranchID)
        {
            bool res = false;
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    IMouchar _opSneak = new MoucharRepository(context);
                    //checking des operations de fermeture de la caisse
                    //1- verifions ke toutes les commandes ont été validé
                    List<AuthoriseSale> lstCustorer = context.AuthoriseSales.Where(c => !c.IsDelivered).ToList();
                    if (lstCustorer.Count > 0)
                    {
                        res = false;
                        throw new Exception("There is a pending Authorise Sale!!! Please validate or delete it before proceed");
                    }
                    //////////2- verifions ke tous les verres de commande postés ont été validé
                    ////////List<SpecialOrder> lstpendvalidpostSO = (from so in context.SpecialOrders
                    ////////                                         where (so.OrderStatut == SpecialOrderStatut.Posted)
                    ////////                                         select so).ToList();
                    ////////if (lstpendvalidpostSO.Count > 0)
                    ////////{
                    ////////    res = false;
                    ////////    throw new Exception("There is a pending Validate Command lens !!! Please validate or delete it before proceed");
                    ////////}
                    //////////3- verifions ke tous les verres de commande pour livraision ont été validé
                    ////////List<SpecialOrder> lstpenddeliverSO = (from so in context.SpecialOrders
                    ////////                                       where (so.OrderStatut == SpecialOrderStatut.Delivered && (so.returnStatut == null || so.returnStatut == SpecialOrderStatut.PartialReturned))
                    ////////                                       select so).ToList();
                    ////////if (lstpenddeliverSO.Count > 0)
                    ////////{
                    ////////    res = false;
                    ////////    throw new Exception("There is a pending Deliver Command lens !!! Please validate it before proceed");
                    ////////}
                    //////////4- verifions ke tous les autorisation pour impayé ont été validé

                    ////////List<NonPaidCash> listNonPaidCash = context.NonPaidCashes.Where(b => !b.isValidated).OrderBy(a => a.DateOperation).ToList();
                    ////////if (listNonPaidCash.Count > 0)
                    ////////{
                    ////////    res = false;
                    ////////    throw new Exception("There is a pending unvalidated Non Paid Cash !!! Please validate or Delete it before proceed");
                    ////////}
                    //5 - verifions ke tous les depenses ont ete valider
                    List<BudgetConsumption> listBudConsume = context.BudgetConsumptions.Where(b => !b.isValidated).OrderBy(a => a.Reference).ToList();
                    if (listBudConsume.Count > 0)
                    {
                        res = false;
                        throw new Exception("There is a pending unvalidated expense !!! Please validate or Delete it before proceed");
                    }

                    //verification du status de la caisse
                    TillDayStatus tState = this.TillDayStatus(tillDay.TillID);
                    if (tState == null)
                    {
                        res = false;
                        throw new Exception("Bad Configuration of Cash Register!!! Please call Your database Administrator");
                    }
                    if (!tState.IsOpen)
                    {
                        res = false;
                        throw new Exception("This Cash Register is Still Close!!! Please Open It Before Proceed");
                    }
                    if (tillDay.TillDayID > 0)
                    {
                        //we get all total of transaction interraction with a till of now day
                        double totalOfSales = InputCash;
                        double totalOfPurchases = OutputCash;
                        double cashContainTill = YesterdayTillDayClosingPrice + totalOfSales;
                        //we determine cash on hand
                        cashContainTill -= totalOfPurchases;
                        tillDay.TillDayCashHand = tillDay.TillDayClosingPrice;
                       
                        //Till till = context.Tills.Find(tillDay.TillID);
                        double updateCashBalance = this.TillStatus(tillDay.TillID).Ballance;
                        if (updateCashBalance != cashContainTill) // tillDay.TillDayClosingPrice)
                        {
                            res = _opSneak.InsertOperation(UserConnect, "ERROR", "Closing price amount is different from database closing price. Please Close this panel and open it aigain", "CloseDay", BusinessDate, BranchID);
                            if (!res)
                            {
                                throw new Exception("Une erreur s'est produite lors de la journalisation ");
                            }
                            res = false;
                            throw new Exception("Closing price amount is different from database closing price. Please Close this panel and open it aigain");
                        }

                        tillDay.TillDayClosingPrice = cashContainTill;

                        tillDay.IsOpen = false;
                        this.Update(tillDay, tillDay.TillDayID);
                        res = _opSneak.InsertOperation(UserConnect, "SUCCESS", "CLOSE CASH REGISTER", "CloseDay", BusinessDate, BranchID);
                        if (!res)
                        {
                            res = false;
                            throw new Exception("Une erreur s'est produite lors de la journalisation ");
                        }

                        //mise a our de la table des statu de caisse
                        tState.IsOpen = false;
                        tState.TillDayLastClosingDate = BusinessDate.Date;
                        context.SaveChanges();
                    }
                    else
                    {
                        res = false;
                        throw new Exception("Error: Wrong Till day!!! please contact your administrator ");
                    }

                    res = true;
                    ts.Complete();
                }
            }
            catch (Exception e)
            {
                throw new Exception("Une erreur s'est produite lors de la fermeture de la caisse : " + "e.Message = " + e.Message);
            }

            return res;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="TillDays"></param>
        /// <param name="YesterdayClosingPrice"></param>
        /// <param name="CashInitialization"></param>
        /// <param name="UserConnect"></param>
        /// <param name="BusinessDate"></param>
        /// <param name="BranchID"></param>
        /// <returns></returns>
        public bool ForceOpenDay(TillDay TillDays, double YesterdayClosingPrice, double CashInitialization, int UserConnect, DateTime BusinessDate, int BranchID)
        {
            bool res = false;

            try
            {
                //EFDbContext context = new EFDbContext();
                using (TransactionScope ts = new TransactionScope())
                {

                    //verifions is ce caissier a ete assigner a une caisse
                    UserTill uTill = context.UserTills.Where(u => u.TillID == TillDays.TillID).FirstOrDefault();
                    if (uTill == null)
                    {
                        res = false;
                        throw new Exception("Please Asssign User to this Cash before you proceed !!!!");
                    }
                    else if (!uTill.HasAccess)
                    {
                        uTill.HasAccess = true;
                        context.SaveChanges();
                    }

                    //verification du status de la caisse
                    TillDayStatus tState = TillDayStatus(TillDays.TillID);
                    if (tState == null)
                    {
                        res = false;
                        throw new Exception("Bad Configuration of Cash Register!!! Please call Your database Administrator");
                    }

                    //mise a our de la table des statu de caisse
                    tState.IsOpen = true;
                    context.SaveChanges();


                    //verification du detail de la caisse
                    TillDay tTillDayTab = context.TillDays.Find(TillDays.TillDayID);
                    //Where(t=>t.TillID== TillID).ToList().OrderByDescending(t=>t.TillDayDate).LastOrDefault();
                    if (tTillDayTab == null)
                    {
                        res = false;
                        throw new Exception("Bad Configuration of Cash Register!!! Please call Your database Administrator");
                    }

                    //mise a our de la table des statu de caisse
                    tTillDayTab.IsOpen = true;
                    context.SaveChanges();

                    res = true;
                    ts.Complete();
                }
            }
            catch (Exception e)
            {
                res = false;
                throw new Exception("Une erreur s'est produite lors de l'ouverture de la caisse : " + "e.Message = " + e.Message);
            }

            return res;
        }
    }
}
