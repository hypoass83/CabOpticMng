using FatSod.DataContext.Initializer;
using FatSod.Security.Abstracts;
using FatSod.Security.Entities;
using FatSod.Supply.Entities;
using FatSodDental.UI.Controllers;
using FatSodDental.UI.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using SaleE = FatSod.Supply.Entities.Sale;
using FatSod.Supply.Abstracts;
using FatSod.Ressources;
using CrystalDecisions.CrystalReports.Engine;
using System.IO;
using FatSodDental.UI.Filters;
using FastSod.Utilities.Util;
using FatSod.Budget.Entities;
using FatSod.DataContext.Concrete;
using System.Web.UI;
using ExtPartialViewResult = Ext.Net.MVC.PartialViewResult;
using System.Web.Routing;

namespace FatSodDental.UI.Areas.CashRegister.Controllers
{
    [Authorize(Order = 1)]
    [TakeBusinessDay(Order = 2)]
    public class CashRegisterController : BaseController
    {
        //Current Controller and current page
        private const string CONTROLLER_NAME = "CashRegister/CashRegister";
        private const string VIEW_NAME_OPEN = "Open";
        private const string VIEW_NAME_CLOSE = "Close";

        private ITillDay _tillDayRepository;

        private IDeposit _depositRepository;
        IMouchar _opSneak;

        public CashRegisterController(
            ITillDay tillDayRepository,
            IDeposit depositRepository,
            IMouchar opSneak
            )
        {
            this._tillDayRepository = tillDayRepository;
            this._depositRepository = depositRepository;
            this._opSneak = opSneak;
        }
        //this enable to open till to allow sales operations
        
        [OutputCache(Duration = 3600)]
        public ActionResult Open()
        {
            List<object> model = new List<object>();
            //ExtPartialViewResult rPVResult = new ExtPartialViewResult
            //{
            //    ClearContainer = true,
            //    //RenderMode = RenderMode.AddTo,
            //    ContainerId = "Body",
            //    //WrapByScriptTag = false,
            //    Model = model
            //};

            //We test if this user has an autorization to get this sub menu
            //if (!LoadAction.isAutoriseToGetMenu(SessionProfileID, CodeValue.CashRegister.MENU_OPEN_CODE, db))
            //{
            //    RedirectToAction("NotAuthorized", "User");
            //}
            
            try
            {
                //vefifion si le user est un caissier
                UserTill userTill = (from td in db.UserTills
                                     where td.UserID == SessionGlobalPersonID && td.HasAccess
                                     select td).SingleOrDefault();

                //if userTill then he not has access for cash register
                if (userTill == null)
                {
                    X.Msg.Alert("Access denied", "You don't have access or cash register don't exist. Please contact our administrator").Show();
                    return this.Direct();
                }
                
                List<BusinessDay> businessDay = (List<BusinessDay>)Session["UserBusDays"];
                DateTime currentDateOp = businessDay.FirstOrDefault().BDDateOperation;
                //we test if business is opened
                if (businessDay == null || businessDay.Count<=0) 
                { 
                    X.Msg.Alert("Error Business", "Business day is closed").Show();
                    return this.Direct();
                }
                ViewBag.BusnessDayDate = currentDateOp;

                

                /*TillDay currentTillDay = (from t in db.TillDays
                                          where t.TillID == userTill.Till.ID && (t.TillDayDate.Day == currentDateOp.Day && t.TillDayDate.Month == currentDateOp.Month && t.TillDayDate.Year == currentDateOp.Year) && t.IsOpen 
                                              select t).FirstOrDefault();
                //we verify if cash register is closed
                if (currentTillDay != null)
                {
                    X.Msg.Alert("Error", "Cash register already opened in this day. You must close it before open").Show();
                    return this.Direct();
                } */
                ViewBag.TillID = userTill.TillID;

                TillDayStatus tState = _tillDayRepository.TillDayStatus(userTill.Till);
                if (tState == null)
                {
                    TillDayStatus tStateInsert = new TillDayStatus();
                    tStateInsert.TillDayLastOpenDate = currentDateOp.Date;
                    tStateInsert.TillDayLastClosingDate = currentDateOp.Date;
                    tStateInsert.TillID = userTill.TillID;
                    tStateInsert.IsOpen = false;
                    db.TillDayStatus.Add(tStateInsert);
                    db.SaveChanges();
                    tState = tStateInsert;
                    //X.Msg.Alert("Error", "Bad Configuration of Cash Register!!! Please call Your database Administrator").Show();
                    //return this.Direct();
                }
                if (tState.IsOpen)
                {
                    X.Msg.Alert("Error", "This Cash Register is Still Open!!! Please Close It Before Proceed").Show();
                    return this.Direct();
                }
                //TillDay yesterDayTillDay = _tillDayRepository.FindAll.LastOrDefault(td => td.TillID == userTill.Till.ID && !td.IsOpen);
                TillDay yesterDayTillDay = _tillDayRepository.FindAll.SingleOrDefault(td => td.TillID == userTill.TillID && td.TillDayDate.Date == tState.TillDayLastClosingDate.Date); 
                if (yesterDayTillDay != null)
                {
                    ViewBag.CashInitialization = 0;
                    ViewBag.YesterdayClosingPrice = yesterDayTillDay.TillDayClosingPrice;
                }
                else
                {
                    ViewBag.YesterdayClosingPrice = 0;
                    ViewBag.CashInitialization = 1;
                }
                Session["TillName"] = userTill.Till.Name;
                Session["TillID"] = userTill.Till.ID;

                return View(model);
            }
            catch (Exception e) 
            { 
                X.Msg.Alert("Error", "An error occure when we try to give response : " + e.Message).Show();
                return this.Direct();
            }

            
        }
        //this enable to Close till to allow sales operations
        [OutputCache(Duration = 3600)] 
        public ActionResult Close()
        {
            try
            { 
            //Session["Curent_Page"] = VIEW_NAME_CLOSE;
            //Session["Curent_Controller"] = CONTROLLER_NAME;
            List<object> model = new List<object>();

            //Ext.Net.MVC.PartialViewResult rPVResult = new Ext.Net.MVC.PartialViewResult
            //{
            //    ClearContainer = true,
            //    //RenderMode = RenderMode.AddTo,
            //    ContainerId = "Body",
            //    //WrapByScriptTag = false,
            //    Model = model
            //};
            //We test if this user has an autorization to get this sub menu
            //if (!LoadAction.isAutoriseToGetMenu(SessionProfileID, CodeValue.CashRegister.MENU_CLOSE_CODE, db))
            //{
            //    RedirectToAction("NotAuthorized", "User");
            //}
            
            //vefifion si le user est un caissier
            UserTill userTill = (from td in db.UserTills
                                 where td.UserID == SessionGlobalPersonID && td.HasAccess
                                 select td).FirstOrDefault();
            
            if (userTill == null)
            {
                X.Msg.Alert("Access denied", "You don't have access or cash register don't exist. Please contact our administrator").Show();
                return this.Direct();
            }
            Session["UserTill"] = userTill.TillID;

            /*
            Till till = (from t in db.Tills where t.ID ==userTill.TillID 
                             select t).SingleOrDefault();
            */

            List<BusinessDay> businessDay = (List<BusinessDay>)Session["UserBusDays"];
            DateTime currentDateOp = businessDay.FirstOrDefault().BDDateOperation;

            ViewBag.BusnessDayDate = currentDateOp;
            Session["BusnessDayDate"] = currentDateOp;
            TillDay currentTillDay = _tillDayRepository.FindAll.SingleOrDefault(t => t.TillID == userTill.TillID && (t.TillDayDate.Day == currentDateOp.Day && t.TillDayDate.Month == currentDateOp.Month && t.TillDayDate.Year == currentDateOp.Year) && t.IsOpen);
                //(from t in db.TillDays
                //                      where t.TillID == userTill.TillID && (t.TillDayDate.Day == currentDateOp.Day && t.TillDayDate.Month == currentDateOp.Month && t.TillDayDate.Year == currentDateOp.Year) && t.IsOpen
                //                      select t).SingleOrDefault();
            if (currentTillDay == null)
            {
                X.Msg.Alert("Error", "You don't have beforehand opened cash register").Show();
                return this.Direct();
            }
            
            //ViewBag.ListSalesDay = ModelSaleDay;
            //ViewBag.ListPurchasseDay = ModelPurchaseDay;
            ViewBag.YesterdayTillDayClosingPrice = currentTillDay.TillDayOpenPrice;

            //======================================== test
            TillSatut tillStatus = _tillDayRepository.TillStatus(currentTillDay.Till);
            //======================================= end test
            ViewBag.TillSatut = tillStatus;
            
            //TempData["till"] = currentTillDay;
            ViewBag.tillDay = currentTillDay;

            return View(model);
                }
            catch (Exception ex)
            {
                X.Msg.Alert("Close teller:", ex.Message).Show();
                return this.Direct();
            }
        }
        //** This actions allow to persiste information of initialize till at one date
        [HttpPost]
        public ActionResult CloseDay(TillDay tillDay, double InputCash, double OutputCash, double YesterdayTillDayClosingPrice)
        {
            try
            {
                //if (tillDay.TillDayID > 0)
                //{
                //    //we get all total of transaction interraction with a till of now day
                //    double totalOfSales = InputCash;
                //    double totalOfPurchases = OutputCash;
                //    double cashContainTill = YesterdayTillDayClosingPrice + totalOfSales;
                //    //we determine cash on hand
                //    cashContainTill -= totalOfPurchases;
                //    if (cashContainTill != tillDay.TillDayClosingPrice)
                //    {
                //        X.Msg.Alert("Closig Till Error", "Closing price amount are not correct. Please contact an administrator if you think no").Show();
                //        bool res1 = _opSneak.InsertOperation(SessionGlobalPersonID, "ERROR", "Closing price amount are not correct. Please contact an administrator if you think no", "CloseDay", (SessionBusinessDay(null) == null) ? DateTime.Now.Date : SessionBusinessDay(null).BDDateOperation, (SessionBusinessDay(null) == null) ? 0 : SessionBusinessDay(null).BranchID);
                //        if (!res1)
                //        {
                //            throw new Exception("Une erreur s'est produite lors de la journalisation ");
                //        }
                //        return this.Direct();
                //    }
                //    double updateCashBalance = _tillDayRepository.TillStatus(db.Tills.Find(tillDay.TillID)).Ballance;
                //    if (updateCashBalance != tillDay.TillDayClosingPrice)
                //    {
                //        X.Msg.Alert("Closig Till Error", "Closing price amount is different from database closing price. Please Close this panel and open it aigain").Show();
                //        bool res1 = _opSneak.InsertOperation(SessionGlobalPersonID, "ERROR", "Closing price amount is different from database closing price. Please Close this panel and open it aigain", "CloseDay", (SessionBusinessDay(null) == null) ? DateTime.Now.Date : SessionBusinessDay(null).BDDateOperation, (SessionBusinessDay(null) == null) ? 0 : SessionBusinessDay(null).BranchID);
                //        if (!res1)
                //        {
                //            throw new Exception("Une erreur s'est produite lors de la journalisation ");
                //        }
                //        return this.Direct();
                //    }
                //    tillDay.IsOpen = false;
                //    _tillDayRepository.Update(tillDay, tillDay.TillDayID);
                //    bool res = _opSneak.InsertOperation(SessionGlobalPersonID, "SUCCESS", "CLOSE CASH REGISTER", "CloseDay", (SessionBusinessDay(null) == null) ? DateTime.Now.Date : SessionBusinessDay(null).BDDateOperation, (SessionBusinessDay(null) == null) ? 0 : SessionBusinessDay(null).BranchID);
                //    if (!res)
                //    {
                //        throw new Exception("Une erreur s'est produite lors de la journalisation ");
                //    }
                //    this.refreshClose();
                //    statusOperation = "Till is closed";
                //}
                Till till = db.Tills.Find(tillDay.TillID);
                tillDay.Till = till;
                bool res = _tillDayRepository.CloseDay(tillDay, InputCash, OutputCash, YesterdayTillDayClosingPrice, SessionGlobalPersonID, SessionBusinessDay(null).BDDateOperation, SessionBusinessDay(null).BranchID);
                if (res)
                {
                    this.refreshClose();
                }
                return this.Direct();
            }
            catch (Exception e) { X.Msg.Alert("Error ", e.Message).Show(); return this.Direct(); }
        }
        [HttpPost]
        public ActionResult OpenDay(TillDay tillDay, double YesterdayClosingPrice = 0, double CashInitialization = 0)
        {
           
            try
            {
                Till till = db.Tills.Find(tillDay.TillID);
                tillDay.Till = till;
                bool res = _tillDayRepository.OpenDay(tillDay, YesterdayClosingPrice, CashInitialization, SessionGlobalPersonID, SessionBusinessDay(null).BDDateOperation, SessionBusinessDay(null).BranchID);
                if (res)
                {
                    this.refreshOpen();
                }
                return this.Direct();
            }
            catch (Exception e) { X.Msg.Alert("Error ", e.Message).Show(); return this.Direct(); }
        }
        public void refreshOpen()
        {
            X.Msg.Alert("Openning Teller ", Resources.MsgOpenTeller).Show();
            this.GetCmp<Button>("btnSave").Disabled = true;
            this.GetCmp<Button>("btnCancel").Disabled = true;
        }
        public void refreshClose()
        {
            X.Msg.Alert("Closing Teller ", Resources.MsgCloseTeller).Show();
            this.GetCmp<Button>("btnSave").Disabled = true;
            this.GetCmp<Button>("btnPrint").Disabled = true;
            this.GetCmp<Button>("btnCancel").Disabled = true;
        }
        //Return list of tillDay
        public StoreResult GetTillDayList()
        {
            List<object> model = new List<object>();
            
            int tillID = (int)Session["TillID"];
            List<TillDay> lstTillDay = (from td in db.TillDays
                                        where td.TillID == tillID
                                        select td)
                                        .OrderByDescending(t=>t.TillDayDate)
                                        .Take(8)
                                        .ToList();
            lstTillDay.ForEach(td=>
            {
                model.Add(
                        new
                        {
                            TillDayID = td.TillDayID,
                            TillName = (string)Session["TillName"],
                            TillDayOpenPrice = td.TillDayOpenPrice,
                            TillDayDate = td.TillDayDate,
                            TillDayClosingPrice = td.TillDayClosingPrice,
                            IsOpen = td.IsOpen ? "Opened" : "Closed"
                        }
                    );
            });
            return this.Store(model);
        }
        //Return list of all sales of day
        public StoreResult GetSalesDayList()
        {
            return this.Store(ModelSaleDay);
        }
        //Return list of all purchase of day
        public StoreResult GetAllPurchaseDayList()
        {
            return this.Store(ModelPurchaseDay);
        }
        //Verify if a price that user open the day is equal with yesterday's price closed
        public ActionResult IsCorrectOpenDayPrice(double? OpenPriceOfDay, double YesterdayClosingPrice, double CashInitialization)
        {
            if (CashInitialization == 1)
            {
                this.GetCmp<Button>("btnSave").Disabled = false;
                return this.Direct();
            }
            if (OpenPriceOfDay != YesterdayClosingPrice)
            {
                this.GetCmp<Button>("btnSave").Disabled = true;
                //X.Msg.Alert("", " ").Show();
                this.AlertError("Opening price", "Opening amount is wrong");
                return this.Direct();
            }
            else
            {
                this.GetCmp<Button>("btnSave").Disabled = false;
                return this.Direct();
            }

        }
        //Verify if a price that user closen the day is equal with all transaction that cash register do in day
        public ActionResult IsCorrectClosingDayPrice(double InputCash, double OutputCash, double YesterdayTillDayClosingPrice, double closingPriceOfDay = 0)
        {
            //we get all total of transaction interraction with a till of now day
            double totalOfSales = InputCash;
            double totalOfPurchases = OutputCash;
            double cashContainTill = YesterdayTillDayClosingPrice + totalOfSales;
            //we determine cash on hand
            cashContainTill -= totalOfPurchases;
            if (cashContainTill != closingPriceOfDay)
            {
                this.GetCmp<Button>("btnPrint").Disabled = true;
                this.GetCmp<Button>("btnSave").Disabled = true;
                this.AlertError("Closing price", "Closing amount is wrong");
                return this.Direct();
            }
            else
            {
                this.GetCmp<Button>("btnPrint").Disabled = false;
                this.GetCmp<Button>("btnSave").Disabled = false;
                return this.Direct();
            }
        }
        //Return a list af sales days

        //2-depot d'epargne par caisse

        //3-Reception d'argent d'une caisse

        //4-Reception d'argent d'une banque
        private List<object> ModelSaleDay
        {
            get
            {
                var businessDayDate = (DateTime)Session["BusnessDayDate"];// businessDay.BDDateOperation;
                int currentTill = (int)Session["UserTill"];
                double ecart = 0d;
                List<object> model = new List<object>();

                //1 - reglement par caisse
                model.AddRange(GetAllSaleDay(businessDayDate, currentTill));

                //2 - reglement par caisse : commande speciale
                //model.AddRange(GetAllSpecialOrderDay(businessDayDate, currentTill));

                //3 - depot d'epargne par caisse
                model.AddRange(getAllSavingAccountDay(businessDayDate, currentTill));

                //4 - recuperation des excés de caisse de la jrne
                model.AddRange(GetAllOverageDay(businessDayDate, currentTill));

                return model;
            }
        }

        /// <summary>
        /// Liste des ventes réglées par caisse pour cette journée
        /// </summary>
        /// <param name="businessDayDate"></param>
        /// <param name="currentTill"></param>
        /// <returns></returns>
        public List<object> GetAllSaleDay(DateTime businessDayDate, int currentTill)
        {

            List<object> model = new List<object>();

            List<CustomerSlice> lstCustSlice = db.CustomerSlices.Where(sl => (sl.SliceDate.Day == businessDayDate.Day && sl.SliceDate.Month == businessDayDate.Month && sl.SliceDate.Year == businessDayDate.Year) && (sl.PaymentMethodID == currentTill) && !sl.isDeposit).ToList();
            lstCustSlice.ForEach(sl =>
            {
                //recuperation de la vente concerne
                SaleE sd = db.Sales.SingleOrDefault(s => s.SaleID == sl.SaleID);
               
                double saleAmount = sd.SaleLines.Select(l => l.LineAmount).Sum();
                //we take a extra price
                ExtraPrice extra = Util.ExtraPrices(saleAmount, sd.RateReduction, sd.RateDiscount, sd.Transport, sd.VatRate);
                model.Add(
                        new
                        {
                            SaleDate = sl.SliceDate,
                            PersonName = sd.Customer.CustomerFullName + " / " + sl.Representant,
                            SaleReceiptNumber = sd.SaleReceiptNumber,
                            SaleTotalPrice = extra.TotalTTC,
                            CashReceived = sl.SliceAmount,
                            SaleOperation =  "SALE" 
                        }
                    );
            });

            return model;
        }

        

        /// <summary>
        /// Liste des dépôts d'épargne par caisse de la journée
        /// </summary>
        /// <returns></returns>
        public List<object> getAllSavingAccountDay(DateTime businessDayDate, int currentTill)
        {

            List<object> model = new List<object>();

            //List<Deposit> lstDeposit = db.Deposits.Where(d => (d.DepositDate.Day == businessDayDate.Day && d.DepositDate.Month == businessDayDate.Month && d.DepositDate.Year == businessDayDate.Year) && (d.PaymentMethodID == currentTill)).ToList();
            List<AllDeposit> lstDeposit = db.AllDeposits.Where(d => (d.AllDepositDate.Day == businessDayDate.Day && d.AllDepositDate.Month == businessDayDate.Month && d.AllDepositDate.Year == businessDayDate.Year) && (d.PaymentMethodID == currentTill)).ToList();            
            lstDeposit.ForEach(ldep =>
            {
                model.Add(
                        new
                        {
                            SaleDate = ldep.AllDepositDate,
                            PersonName =ldep.Customer./* ldep.SavingAccount.Customer.*/CustomerFullName + " / " + ldep.Representant,
                            SaleReceiptNumber =ldep.AllDepositReference,// "DEP" + ldep.AllDepositID,
                            SaleTotalPrice = 0,
                            CashReceived = ldep.Amount,
                            SaleOperation = "DEPOSIT"
                        }
                    );
            });

            return model;

        }

        /// <summary>
        /// Liste des entrées en caisse constatées par les ajustements de la caisse
        /// </summary>
        /// <param name="businessDayDate"></param>
        /// <param name="currentTill"></param>
        /// <returns></returns>
        public List<object> GetAllOverageDay(DateTime businessDayDate, int currentTill)
        {

            List<object> model = new List<object>();

            List<TillAdjust> allDayAdjusts = db.TillAdjusts.Where(t => (t.TillAdjustDate.Day == businessDayDate.Day && t.TillAdjustDate.Month == businessDayDate.Month && t.TillAdjustDate.Year == businessDayDate.Year) && t.TillID == currentTill && ((t.PhysicalPrice > t.ComputerPrice))).ToList();
            allDayAdjusts.ForEach(ta =>
            {
                ta.ecart = ta.PhysicalPrice - ta.ComputerPrice;

                model.Add(

                        new
                        {
                            SaleDate = ta.TillAdjustDate,
                            PersonName = ta.Till.Code,
                            SaleReceiptNumber = "OVERAGE",
                            SaleTotalPrice = ta.ComputerPrice,
                            CashReceived = ta.ecart,
                            SaleOperation = "OVERAGE"
                        }
                    );

            });

            return model;
        }

        private List<object> ModelPurchaseDay
        {
            get
            {
                double ecart = 0d;
                DateTime businessDayDate = (DateTime)Session["BusnessDayDate"];
                int currentTill = (int)Session["UserTill"];
                List<object> model = new List<object>();
                //1 - Liste des achats réglés par caisse
                model.AddRange(GetAllPurchaseDay(businessDayDate, currentTill));

                //2 - Liste des dépenses budgetaires de la journée
                model.AddRange(GetAllBudgetExpenseDay(businessDayDate, currentTill));

                //3 - Liste des Sorties constatées par un ajustement de la caisse
                model.AddRange(GetAllShortageDay(businessDayDate, currentTill));

                //4 - Liste des remboursements liées aux retours sur vente aux clients
                model.AddRange(GetAllReturnSlices(businessDayDate, currentTill));
                //5-Liste des Transfert vers la banque de la jne
                model.AddRange(getAllTransferToBankDay(businessDayDate, currentTill));
                return model;
            }
        }

        public List<object> getAllTransferToBankDay(DateTime businessDayDate, int currentTill)
        {
            
            List<object> model = new List<object>();

            List<TreasuryOperation> lstTranfbank = (from t in db.TreasuryOperations
                                                    where ((t.OperationDate.Day == businessDayDate.Day && t.OperationDate.Month == businessDayDate.Month && t.OperationDate.Year == businessDayDate.Year) && t.TillID == currentTill && t.OperationType == CodeValue.Accounting.TreasuryOperation.TransfertToBank)
                                                    select t).AsQueryable().ToList();
            lstTranfbank.ForEach(sd =>
                {
                    model.Add(
                            new
                            {
                                //Variables fields
                                PurchaseReference = sd.OperationRef,
                                SupplierFullName = (sd.BankID != null) ? sd.Bank.Name : (sd.TillDestID!=null) ? sd.TillDest.Name : "NONE" ,
                                SupplierOperation = "TRANSFER TO BANK",
                                PurchaseBringerFullName = sd.Justification,
                                PurchaseTotalAmount = sd.OperationAmount,
                                CashReceivedOupted = sd.OperationAmount
                            }
                );
                });

            return model;
        }

        public List<object> GetAllPurchaseDay(DateTime businessDayDate, int currentTill)
        {

            List<object> model = new List<object>();

            List<SupplierSlice> lstSupSlice = db.SupplierSlices.Where(sl => (sl.SliceDate.Day == businessDayDate.Day && sl.SliceDate.Month == businessDayDate.Month && sl.SliceDate.Year == businessDayDate.Year) && (/*sl.PaymentMethod is Till && */sl.PaymentMethodID == currentTill)).ToList();
            lstSupSlice.ForEach(sl =>
            {
                //recuperation de l'achat concerne
                Purchase sd = db.Purchases.SingleOrDefault(s => s.PurchaseID == sl.PurchaseID);
                double purchaseAmount = sd.PurchaseLines.Select(l => l.LineAmount).Sum();
                //we take a extra price
                ExtraPrice extra = Util.ExtraPrices(purchaseAmount, sd.RateReduction, sd.RateDiscount, sd.Transport, sd.VatRate);
                model.Add(
                        new
                        {
                            PurchaseReference = sd.PurchaseReference,
                            SupplierFullName = sd.SupplierFullName,
                            SupplierOperation = "PURCHASE",
                            PurchaseBringerFullName = sd.PurchaseBringerFullName,
                            PurchaseTotalAmount = extra.TotalTTC,
                            CashReceivedOupted = sl.SliceAmount
                        }
                    );
            });

            return model;

        }

        public List<object> GetAllBudgetExpenseDay(DateTime businessDayDate, int currentTill)
        {

            List<object> model = new List<object>();

            db.BudgetConsumptions.Where(b => (b.PaymentDate.Value.Day == businessDayDate.Day && b.PaymentDate.Value.Month == businessDayDate.Month && b.PaymentDate.Value.Year == businessDayDate.Year) && (/*b.PaymentMethod is Till && */b.PaymentMethodID == currentTill)).ToList().ForEach(sd =>
            {
                model.Add(
                      new
                      {
                          //Variables fields
                          PurchaseReference = sd.Reference,
                          SupplierFullName = sd.BudgetAllocated.BudgetLine.BudgetLineLabel,
                          SupplierOperation = "BUDGET EXPENSE",
                          PurchaseBringerFullName = sd.BeneficiaryName,
                          PurchaseTotalAmount = sd.VoucherAmount,
                          CashReceivedOupted = sd.VoucherAmount
                      }
                );

            });

            return model;

        }

        public List<object> GetAllShortageDay(DateTime businessDayDate, int currentTill)
        {

            List<object> model = new List<object>();

            List<TillAdjust> allDayShortageAdjusts = db.TillAdjusts.Where(t => (t.TillAdjustDate.Day == businessDayDate.Day && t.TillAdjustDate.Month == businessDayDate.Month && t.TillAdjustDate.Year == businessDayDate.Year) && t.TillID == currentTill && ((t.PhysicalPrice < t.ComputerPrice))).ToList();

            allDayShortageAdjusts.ForEach(ta =>
            {

                ta.ecart = ta.ComputerPrice - ta.PhysicalPrice ;

                model.Add(
                    new
                    {
                        //Variables fields
                        PurchaseReference = "TELLER ADJUST",
                        SupplierFullName = ta.Till.Code,
                        SupplierOperation = "TELLER ADJUST",
                        PurchaseBringerFullName = ta.Justification,
                        PurchaseTotalAmount = ta.ComputerPrice,
                        CashReceivedOupted = ta.ecart
                    }
                    );

            });


            return model;

        }

        public List<object> GetAllReturnSlices(DateTime businessDayDate, int currentTill)
        {

            List<object> model = new List<object>();

            List<CustomerReturnSlice> returnSlices = db.CustomerReturnSlices.Where(sl => (sl.SliceDate.Day == businessDayDate.Day && sl.SliceDate.Month == businessDayDate.Month && sl.SliceDate.Year == businessDayDate.Year) && (sl.PaymentMethodID == currentTill)).ToList();

            returnSlices.ForEach(ta =>
            {
                SaleE sale = _depositRepository.ApplyExtraPrice(ta.CustomerReturn.Sale);

                model.Add(
                    new
                    {
                        PurchaseReference = sale.SaleReceiptNumber,
                        SupplierFullName = sale.Customer.CustomerFullName,
                        SupplierOperation = "SALE RETURN",
                        PurchaseBringerFullName = "SALE RETURN",
                        PurchaseTotalAmount = sale.TotalPriceTTC,
                        CashReceivedOupted = ta.SliceAmount
                    }
                    );
            });

            return model;

        }

        //Close cash and print report
        [HttpPost]
        public ActionResult CloseCashAndPrintReport(TillDay tillDay, double InputCash, double OutputCash, double YesterdayTillDayClosingPrice, double closingPriceOfDay = 0)
        {
            try
            {
                /*if (tillDay.TillDayID > 0)
                {
                    //we get all total of transaction interraction with a till of now day
                    double totalOfSales = InputCash;
                    double totalOfPurchases = OutputCash;
                    double cashContainTill = YesterdayTillDayClosingPrice + totalOfSales;
                    //we determine cash on hand
                    cashContainTill -= totalOfPurchases;
                    if (cashContainTill != tillDay.TillDayClosingPrice)
                    {
                        X.Msg.Alert("Closig Till Error", Resources.MsgCloseTellerErrAmt).Show();
                        bool res = _opSneak.InsertOperation(SessionGlobalPersonID, "ERROR", Resources.MsgCloseTellerErrAmt, "CloseCashAndPrintReport", (SessionBusinessDay(null) == null) ? DateTime.Now.Date : SessionBusinessDay(null).BDDateOperation, (SessionBusinessDay(null) == null) ? 0 : SessionBusinessDay(null).BranchID);
                        if (!res)
                        {
                            throw new Exception("Une erreur s'est produite lors de la journalisation ");
                        }
                        return this.Direct();
                    }
                    tillDay.IsOpen = false;
                    _tillDayRepository.Update(tillDay, tillDay.TillDayID);
                    bool res1 = _opSneak.InsertOperation(SessionGlobalPersonID, "SUCCESS", "CLOSE CASH REGISTER", "CloseCashAndPrintReport", (SessionBusinessDay(null) == null) ? DateTime.Now.Date : SessionBusinessDay(null).BDDateOperation, (SessionBusinessDay(null) == null) ? 0 : SessionBusinessDay(null).BranchID);
                    if (!res1)
                    {
                        throw new Exception("Une erreur s'est produite lors de la journalisation ");
                    }
                    this.refreshClose();
                    statusOperation = "Till is closed";
                } */
                Till till = db.Tills.Find(tillDay.TillID);
                tillDay.Till = till;
                bool res = _tillDayRepository.CloseDay(tillDay, InputCash, OutputCash, YesterdayTillDayClosingPrice, SessionGlobalPersonID, SessionBusinessDay(null).BDDateOperation, SessionBusinessDay(null).BranchID);
                if (res)
                {
                    //////////
                    this.GetCmp<FormPanel>("TillDayForm").Hidden = true;
                    this.GetCmp<Container>("DEContainer").Hidden = true;
                    this.GetCmp<Panel>("RptCashOp").Hidden = false;
                    Session["tillDayID"] = tillDay.TillDayID;
                    Session["tillDayDate"] = tillDay.TillDayDate;
                    this.GetCmp<Panel>("RptCashOp").LoadContent(new ComponentLoader
                    {
                        Url = Url.Action("GenerateReportOperationOfDay", "State"),
                        DisableCaching = false,
                        Mode = LoadMode.Frame,
                        Params = { new Parameter("tillDay", tillDay) }
                    });
                    this.refreshClose();
                }
                
                return this.Direct();
                ////////////
            }
            catch (Exception e) { X.Msg.Alert("Error ", e.Message).Show(); return this.Direct(); }

        }

        [HttpPost]
        public ActionResult PrintReport(TillDay tillDay, double InputCash, double OutputCash, double YesterdayTillDayClosingPrice, double closingPriceOfDay = 0)
        {
 
            try{
            //////////
                //this.GetCmp<FormPanel>("TillDayForm").Hidden = true;
                //this.GetCmp<Container>("DEContainer").Hidden = true;
                this.GetCmp<Panel>("RptCashOp").Hidden = false;
                Session["tillDayID"] = tillDay.TillDayID;
                Session["tillDayDate"] = tillDay.TillDayDate;
                this.GetCmp<Panel>("RptCashOp").LoadContent(new ComponentLoader
                {
                    Url = Url.Action("GenerateReportOperationOfDay", "State"),
                    DisableCaching = false,
                    Mode = LoadMode.Frame,
                    Params = { new Parameter("tillDay", tillDay) }
                });
                return this.Direct();
                ////////////
            }
            catch (Exception e) { X.Msg.Alert("Error ", e.Message).Show(); return this.Direct(); }


        }

        //This method return a list of tills of one branch
        public ActionResult GetTillOfBanch(int BranchID)
        {
            int i = 0;
            List<object> model = new List<object>();
            UserTill userTill = db.UserTills.FirstOrDefault(td => td.UserID == SessionGlobalPersonID);
            if (userTill == null)
            {
                db.Tills.Where(t => t.BranchID == BranchID).ToList().ForEach(t =>
                {
                    i++;
                    model.Add(new { Name = t.Name, ID = t.ID });
                }
              );
            }
            else
            {
                i++;
                model.Add(new { Name = userTill.Till.Name, ID = userTill.Till.ID });
            }
            
            if (i == 0)
            {
                this.GetCmp<ComboBox>("TillID").Disabled = true;
                return this.Direct();
            }
            this.GetCmp<ComboBox>("TillID").Disabled = false;
            return this.Store(model);
        }
    }
}