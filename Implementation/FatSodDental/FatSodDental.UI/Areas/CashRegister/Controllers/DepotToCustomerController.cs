using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using Ext.Net;
using Ext.Net.MVC;
using FastSod.Utilities.Util;
using FatSod.DataContext.Concrete;
using FatSod.DataContext.Initializer;
using FatSod.Ressources;
using FatSod.Security.Abstracts;
using FatSod.Security.Entities;
using FatSod.Supply.Abstracts;
using FatSod.Supply.Entities;
using FatSodDental.UI.Controllers;
using FatSodDental.UI.Filters;
using FatSodDental.UI.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using SaleE = FatSod.Supply.Entities.Sale;

namespace FatSodDental.UI.Areas.CashRegister.Controllers
{
    [Authorize(Order = 1)]
    [TakeBusinessDay(Order = 2)]
    public class DepotToCustomerController : BaseController
    {
        //Current Controller and current page
        private const string CONTROLLER_NAME = "CashRegister/DepotToCustomer" ;
        private const string VIEW_NAME = "Index";

        private IBusinessDay _busDayRepo;
        private IDeposit _depositRepository;
        private ISavingAccount _savingAccountRepository;

        private List<BusinessDay> lstBusDay;


        private ITransactNumber _transactNumbeRepository;
        private ITillDay _tillDayRepository;
        private BusinessDay businessDay;

        public DepotToCustomerController(
            IBusinessDay busDayRepo,
            ISavingAccount saRepo,
            IDeposit depositRepository,
            ITillDay tillDayRepository,
            ITransactNumber transactNumbeRepository
        )
        {
            this._busDayRepo = busDayRepo;
            this._savingAccountRepository = saRepo;
            this._depositRepository = depositRepository;
            this._transactNumbeRepository = transactNumbeRepository;
            this._tillDayRepository = tillDayRepository;
        }

        [OutputCache(Duration = 3600)]
        public ActionResult Index()
        {
            try
            {

                UserTill userTill = (from td in db.UserTills
                                     where td.UserID == SessionGlobalPersonID
                                     select td).SingleOrDefault();
                if (userTill == null || userTill.TillID <= 0)
                {
                    X.Msg.Alert("Access Denied", "You can't do sale operation. If you think no, try again or<br/>contact our administrator for this purpose<code/>.").Show();
                    return this.Direct();
                }
                List<BusinessDay> UserBusDays = (List<BusinessDay>)Session["UserBusDays"];
                Session["businessDay"] = UserBusDays.FirstOrDefault();
                DateTime currentDateOp = UserBusDays.FirstOrDefault().BDDateOperation;
                ViewBag.CurrentBranch = UserBusDays.FirstOrDefault().BranchID;
                ViewBag.BusnessDayDate = UserBusDays.FirstOrDefault().BDDateOperation;// businessDay.BDDateOperation;
                Session["BusnessDayDate"] = currentDateOp;
                TillDayStatus tState = _tillDayRepository.TillDayStatus(userTill.Till);
                if (tState == null)
                {
                    X.Msg.Alert("Error", "Bad Configuration of Cash Register!!! Please call Your database Administrator").Show();
                    return this.Direct();
                }
                if (!tState.IsOpen)
                {
                    X.Msg.Alert("Error", "This Cash Register is Still Close!!! Please Open It Before Proceed").Show();
                    return this.Direct();
                }

                TillDay currentTillDay = (from t in db.TillDays
                                          where
                                              t.TillID == userTill.TillID && t.TillDayDate == tState.TillDayLastOpenDate.Date && t.IsOpen // t.TillDayDate.Day == currentDateOp.Day && t.TillDayDate.Month == currentDateOp.Month && t.TillDayDate.Year == currentDateOp.Year && t.IsOpen
                                          select t).FirstOrDefault();

                if (currentTillDay == null)
                {
                    X.Msg.Alert("Warnnig", "Cash register is closed. You must open it before do any sale<br/>Go at Cash Register module=>Open cash register<code/>").Show();
                    return this.Direct();
                }


                return View(/*ModelPendingCustomerSale(0, null)*/);
            }

            catch (Exception e)
            {
                X.Msg.Alert("Error ", e.Message).Show();
                return this.Direct();
            }
        }

        public ActionResult OpenedBusday()
        {
            List<object> list = new List<object>();

            lstBusDay = (List<BusinessDay>)Session["UserBusDays"];
            if (lstBusDay == null)
            {
                lstBusDay = _busDayRepo.GetOpenedBusinessDay(CurrentUser);
            };

            foreach (BusinessDay busDay in lstBusDay)
            {
                list.Add(
                    new
                    {
                        BranchID = busDay.BranchID,
                        BranchName = busDay.BranchName
                    }
                    );
            }

            return this.Store(list);

        }

        public ActionResult ChangeBusDay(int? BranchID)
        {
            if (BranchID != null && BranchID.HasValue && BranchID.Value > 0)
            {
                lstBusDay = (List<BusinessDay>)Session["UserBusDays"];
                if (lstBusDay == null)
                {
                    lstBusDay = _busDayRepo.GetOpenedBusinessDay(CurrentUser);
                };

                BusinessDay businessD = lstBusDay.FirstOrDefault(bd => bd.BranchID == BranchID && bd.BDStatut);
                this.GetCmp<DateField>("DepositDate").Value = businessD.BDDateOperation;
            }
            return this.Direct();
        }

        public StoreResult LoadThirdPartyAccounts(String DepositReason, int? BranchID)
        {
            List<object> customers = new List<object>();

            if (DepositReason != null && (BranchID != null && BranchID.Value > 0))
            {

                foreach (Customer c in db.Customers.ToList())
                {
                    customers.Add(
                        new
                        {
                            CustomerFullName = c.CustomerFullName,
                            PersonID = c.GlobalPersonID
                        }
                        );
                }

            }
            return this.Store(customers);
        }

        public StoreResult LoadCustomerSales(int? CustomerID)
        {
            this.GetCmp<ComboBox>("PurchaseID").Reset();

            List<object> list = new List<object>();

            if (CustomerID != null && CustomerID.Value > 0)
            {
                List<FatSod.Supply.Entities.Sale> sales = _depositRepository.CustomerAllUnPaidSalesStockLens(db.Customers.Find(CustomerID.Value)).ToList();

                foreach (FatSod.Supply.Entities.Sale s in sales)
                {
                    list.Add(
                            new
                            {
                                SaleFullInformation = s.SaleFullInformation,
                                SaleID = s.SaleID
                            }
                        );
                }

            }
            return this.Store(list);
        }

        public ActionResult Remainder(int? SaleID)
        {
            this.GetCmp<NumberField>("Remainder").Reset();
            this.GetCmp<NumberField>("SaleTotalPriceAdvance").Reset();
            if (SaleID != null && SaleID.Value > 0)
            {
                this.GetCmp<NumberField>("Remainder").SetValue(_depositRepository.SaleRemainder(db.Sales.Find(SaleID.Value)));
                this.GetCmp<NumberField>("SaleTotalPriceAdvance").SetValue(_depositRepository.SaleTotalPriceAdvance(db.Sales.Find(SaleID.Value)));
            }

            return this.Direct();
        }

        public ActionResult DoDeposit(Deposit deposit)
        {
            try
            {
                int depositID = 0; ;
                Session["Receipt_SaleID"] = null;
                Session["Receipt_CustomerID"] = null;
                Session["ReceiveAmoung_ID"] = null;
                Session["Receip_DepositID"] = null;
                Session["TotalDette"] = null;
                Session["DepositReason"] = null;

                Session["ReceiveAmoung_Tot"] = deposit.Amount;

                Session["DepositPaymentMethod"] = deposit.PaymentMethod;
                //choix de la caisse
                if (deposit.PaymentMethod == CodeValue.Accounting.DefaultCodeAccountingSection.CODECAIS)
                {
                    int userTill = db.UserTills.FirstOrDefault(td => td.UserID == SessionGlobalPersonID).TillID;
                    deposit.PaymentMethodID = userTill;
                }

                //Si l'utilisateur souhaite payer en utilisant son compte d'épargne
                if (deposit.PaymentMethod == CodeValue.Supply.DepositReason.SavingAccount)
                {
                    double customerDebt = deposit.Debt;

                    SavingAccount sa = db.SavingAccounts.SingleOrDefault(sa1 => sa1.CustomerID == deposit.CustomerID);

                    if (sa == null || sa.ID == 0)
                    {

                        X.Msg.Alert("No Customer Account",
                            "Sorry, Customer doesn't have a Customer Account. Please contact an administrator").Show();
                        return this.Direct();
                    }

                    //Montant total d'argent dans le compte du client
                    double savingAccountBalance = deposit.SavingAmount;

                    //ne pas règler les achats du client si : 
                    //1 - pas d'argent dans le compte du client; 
                    //2- Facture > Montant total dans la caisse du client
                    //3 - solde du client superieur o mtant a regle
                    //if (savingAccountBalance - customerDebt >= deposit.Amount)
                    //{
                    if (savingAccountBalance <= 0 || deposit.Amount > savingAccountBalance)
                    {
                        X.Msg.Alert("NO Enough Money in Customer Account",
                            "Sorry, Customer doesn't have sufficient Money inside his Account. Please contact an administrator").Show();
                        return this.Direct();
                    }
                    deposit.PaymentMethodID = sa.ID;
                    _savingAccountRepository.PayAllSales(deposit.CustomerID, deposit.Amount, deposit.DepositDate, deposit.Representant, SessionGlobalPersonID, SessionBusinessDay(null).BranchID);
                    SetStateParameters(deposit);
                    return ResetDepositForm();
                    /*}
                    else
                    {
                        X.Msg.Alert("NO Enough Money in Customer Account",
                            "Sorry, The Customer Balance is not Enough to Validate this transaction. Please contact an administrator").Show();
                        return this.Direct();
                    }     */


                }

                //businessDay = (BusinessDay)Session["businessDay"];
                //deposit.DepositReference = _transactNumbeRepository.returnTransactNumber("REDP", businessDay);
                if (deposit.DepositReference.Trim() == "" || deposit.DepositReference == null)
                {
                    X.Msg.Alert("Deposit Reference",
                            "Sorry, The Deposit reference is null. Please Check the business day or contact your administrator").Show();
                    return this.Direct();
                }
                if (deposit.DepositReason == CodeValue.Supply.DepositReason.SalePayment) // || deposit.DepositReason == CodeValue.Supply.DepositReason.SpecialOrderPayment)
                {
                    _depositRepository.SaleDepositCustomer(deposit, SessionGlobalPersonID);
                }
                else
                {
                    X.Msg.Alert("Deposit Reason",
                            "Sorry, This reason for deposit it is not yet configure. Please contact your administrator").Show();
                    return this.Direct();
                }
                SetStateParameters(deposit);
                this.AlertSucces(Resources.Success, Resources.DepositSuccess);
                return this.ResetDepositForm();
            }
            catch (Exception e)
            {
                X.Msg.Alert("Error ", e.Message + " " + e.StackTrace + " " + e.InnerException).Show();
                return this.Direct();
            }

        }

        public void SetStateParameters(Deposit deposit)
        {
            Session["Receipt_SaleID"] = deposit.SaleID;
            Session["Receipt_CustomerID"] = deposit.CustomerID;
            Session["ReceiveAmoung_ID"] = deposit.Amount;
            Session["Receip_DepositID"] = deposit.DepositID;
            Session["CustomerID"] = deposit.CustomerID;
            Session["TotalDette"] = deposit.Debt;
            Session["DepositReason"] = deposit.DepositReason;
        }

        [HttpPost]
        public ActionResult ResetDepositForm()
        {
            //this.GetCmp<FormPanel>("DepositForm").Reset(true);

            int? CustomerID = (int)Session["CustomerID"];

            if (CustomerID != null && CustomerID.HasValue && CustomerID.Value > 0)
            {
                this.GetCmp<ComboBox>("CustomerID").Value = CustomerID.Value;
            }
            this.GetCmp<NumberField>("Amount").Reset();
            this.GetCmp<Button>("btnPrint").Disabled = false;
            this.GetCmp<Button>("btnSave").Disabled = true;

            this.GetCmp<ComboBox>("PaymentMethod").Reset();
            this.GetCmp<ComboBox>("PaymentMethodID").Reset();
            return this.Direct();
        }
        //public StoreResult PendingCustomerSale(string Representant)
        //{
        //    int? CustomerID = (int)Session["CustomerID"];

        //    if (CustomerID != null && CustomerID.HasValue && CustomerID.Value > 0)
        //    {
        //        return this.Store(this.ModelPendingCustomerSale(CustomerID, Representant));
        //    }

        //    return this.Store(new List<object>());
        //}
        public ActionResult LoadCustomerDebt(int? CustomerID, string Representant)
        {
            this.GetCmp<TextField>("Representant").Reset();
            //this.GetCmp<NumberField>("Debt").Reset();
            //this.GetCmp<NumberField>("SavingAmount").Reset();
            //this.GetCmp<NumberField>("Balance").Reset();

            if (CustomerID != null && CustomerID.Value > 0)
            {

                Customer cust = (from c in db.Customers
                                 where c.GlobalPersonID == CustomerID.Value
                                 select c).SingleOrDefault();

                this.GetCmp<TextField>("Representant").SetValue(cust.CustomerFullName);
                this.GetCmp<TextField>("Representant").AllowBlank = true;

                

                //double Debt = _depositRepository.CustomerDebtStockLens(cust);
                //double SavingAmount = _savingAccountRepository.GetSavingAccountBalance(cust);
                //this.GetCmp<NumberField>("SavingAmount").SetValue(SavingAmount);
                //double balance = SavingAmount - Debt;
                //this.GetCmp<NumberField>("Debt").SetValue(Debt);

                //this.GetCmp<NumberField>("Balance").SetValue(balance);

                //generation de la reference
                businessDay = (BusinessDay)Session["businessDay"];
                string trnnum = _transactNumbeRepository.displayTransactNumber("REDP", businessDay);
                this.GetCmp<TextField>("DepositReference").Value = trnnum;
            }
            Session["CustomerID"] = CustomerID;
            this.GetCmp<NumberField>("Amount").Reset();
            //this.GetCmp<Store>("SalesListStore").Reload();
            return this.Direct();
        }

        public ActionResult OnRepresentantBlur(int? CustomerID, string Representant)
        {

            if (CustomerID != null && CustomerID.Value > 0)
            {
                /*
                                Customer cust = db.Customers.Find(CustomerID.Value);

                                if (LoadComponent.IsGeneralPublic(cust))
                                {
                                    this.GetCmp<NumberField>("Debt").Reset();
                                    this.GetCmp<NumberField>("SavingAmount").Reset();
                                    this.GetCmp<NumberField>("Balance").Reset();

                                    double Debt = (Representant == null || Representant.Length <= 0) ? _depositRepository.CustomerDebtStockLens(cust) : _depositRepository.CustomerDebtStockLens(cust, Representant);
                                    this.GetCmp<NumberField>("Debt").SetValue(Debt);
                    
                                    double SavingAmount = (Representant == null || Representant.Length <= 0) ? _savingAccountRepository.GetSavingAccountBalance(db.Customers.Find(CustomerID.Value)) : _savingAccountRepository.GetSavingAccountBalance(db.Customers.Find(CustomerID.Value), Representant);
                                    this.GetCmp<NumberField>("SavingAmount").SetValue(SavingAmount);
                                    double balance = SavingAmount - Debt;
                                    this.GetCmp<NumberField>("Balance").SetValue(balance);
                                }
                                else
                                {
                                    double Debt = _depositRepository.CustomerDebtStockLens(cust);
                                    this.GetCmp<NumberField>("Debt").SetValue(Debt);
                                    this.GetCmp<TextField>("Representant").SetValue(cust.CustomerFullName);
                                    double SavingAmount = _savingAccountRepository.GetSavingAccountBalance(db.Customers.Find(CustomerID.Value));
                                    this.GetCmp<NumberField>("SavingAmount").SetValue(SavingAmount);
                                    double balance = SavingAmount - Debt;
                                    this.GetCmp<NumberField>("Balance").SetValue(balance);
                                }
                */
            }
            Session["CustomerID"] = CustomerID;
            //this.GetCmp<Store>("SalesListStore").Reload();
            return this.Direct();
        }

        public StoreResult PaymentMethods(string PaymentMethod)
        {
            List<object> list = new List<object>();
            if (PaymentMethod == CodeValue.Accounting.DefaultCodeAccountingSection.CODEBANK)
            {
                List<Bank> PaymentMethods = new List<Bank>();

                PaymentMethods = db.Banks.Where(p => p.Account.CollectifAccount.AccountingSection.AccountingSectionCode == PaymentMethod).ToList();

                foreach (PaymentMethod pm in PaymentMethods)
                {
                    list.Add(
                        new
                        {
                            ID = pm.ID,
                            Name = pm.Name
                        }
                        );
                }
            }
            return this.Store(list);
        }

        //This method load a method that print a receip of deposit
        public ActionResult PrintDepositReceipt()
        {
            this.GetCmp<Panel>("Pdf").LoadContent(new ComponentLoader
            {
                Url = Url.Action("GenerateReceiptDeposit"),
                DisableCaching = false,
                Mode = LoadMode.Frame,
            });

            return this.Direct();
        }
        //This method print a receipt of customer
        public void GenerateReceiptDeposit()
        {
            List<object> model = new List<object>();
            ReportDocument rptH = new ReportDocument();
            try
            {
                int saleID = (Session["Receipt_SaleID"] == null) ? 0 : (int)Session["Receipt_SaleID"];
                int customerID = (Session["Receipt_CustomerID"] == null) ? 0 : (int)Session["Receipt_CustomerID"];
                double receiveAmoung = (Session["ReceiveAmoung_ID"] == null) ? 0 : (double)Session["ReceiveAmoung_ID"];
                int DepositID = (Session["Receip_DepositID"] == null) ? 0 : (int)Session["Receip_DepositID"];
                double receiveAmountTot = (Session["ReceiveAmoung_Tot"] == null) ? 0 : (double)Session["ReceiveAmoung_Tot"];
                string DepositReason = (Session["DepositReason"] == null) ? "" : (string)Session["DepositReason"];
                string DepositPaymentMethod = (Session["DepositPaymentMethod"] == null) ? "" : (string)Session["DepositPaymentMethod"];

                string repName = "";
                bool isValid = false;
                double totalAmount = 0d;
                double totalRemaining = 0d;


                string path = "";
                string defaultImgPath = Server.MapPath("~/Content/Images/App/deboy-logo.png");
                byte[] COMPANY_LOGO = System.IO.File.ReadAllBytes(defaultImgPath);

                totalAmount = (Session["TotalDette"] == null) ? 0 : (double)Session["TotalDette"];
                totalRemaining = totalAmount - receiveAmoung;

                Devise devise = new Devise();
                Branch curBranch = new Branch();
                Customer customer = new Customer();

                string TitleDeposit = "";
                string RptTitle = "";

                if (DepositPaymentMethod == CodeValue.Accounting.DefaultCodeAccountingSection.CODECAIS)
                {
                    TitleDeposit = "Cash Paid In";
                    RptTitle = "CASH RECEIPT";
                }
                if (DepositPaymentMethod == CodeValue.Accounting.DefaultCodeAccountingSection.CODEBANK)
                {
                    TitleDeposit = "Check Paid In";
                    RptTitle = "CHECK RECEIPT";
                }
                if (DepositPaymentMethod == CodeValue.Supply.DepositReason.SavingAccount)
                {
                    TitleDeposit = "Saving Paid In";
                    RptTitle = "SAVING RECEIPT";
                }

                if (customerID > 0)
                {
                    customer = (from c in db.Customers
                                where c.GlobalPersonID == customerID
                                select c).SingleOrDefault();



                    curBranch = db.Branches.Find(db.UserBranches.Where(ub => ub.UserID == SessionGlobalPersonID).FirstOrDefault().BranchID);
                    businessDay = _busDayRepo.GetOpenedBusinessDay(curBranch);
                    devise = db.Devises.FirstOrDefault(d => d.DefaultDevise);

                    Company cmpny = db.Companies.FirstOrDefault();
                    if (saleID > 0)//depot pour une vente
                    {
                        double saleAmount = 0d;
                        SaleE currentSale = db.Sales.Find(saleID);
                        saleAmount = currentSale.SaleLines.Select(l => l.LineAmount).Sum();
                        ExtraPrice extra = Util.ExtraPrices(saleAmount, currentSale.RateReduction, currentSale.RateDiscount, currentSale.Transport, currentSale.VatRate);


                        model.Add(
                            new
                            {
                                ReceiveAmount = receiveAmountTot,
                                TotalAmount = totalAmount, //montant total de la facture
                                LineUnitPrice = _depositRepository.SaleRemainder(currentSale), //reste du montant a verser
                                CompanyName = cmpny.Name,
                                CompanyAdress = cmpny.Adress.AdressFullName,// "Head Quater : " + cmpny.Adress.Quarter.Town.Region.RegionLabel + " - " + cmpny.Adress.Quarter.Town.TownLabel,
                                CompanyTel = "Tel: " + cmpny.Adress.AdressPhoneNumber + " Cell:" + cmpny.Adress.AdressCellNumber,
                                BranchName = currentSale.Branch.BranchName,
                                BranchAdress = "B.P " + cmpny.Adress.AdressPOBox + ", " + cmpny.Adress.Quarter.Town.TownLabel + ", Email: " + cmpny.Adress.AdressEmail,//currentSale.Branch.Adress.Quarter.QuarterLabel + " - " + currentSale.Branch.Adress.Quarter.Town.TownLabel,
                                BranchTel = "Tel: " + currentSale.Branch.Adress.AdressPhoneNumber,
                                Ref = currentSale.SaleReceiptNumber,
                                CompanyCNI = "NO CONT : " + cmpny.CNI,
                                Operator = CurrentUser.Name + " " + CurrentUser.Description,
                                CustomerName = customer.Name,// + " " + customer.Description,
                                CustomerAccount = customer.CNI,
                                SaleDate = currentSale.SaleDate,
                                Title = TitleDeposit,
                                DeviseLabel = currentSale.Devise.DeviseLabel,
                                RateTVA = currentSale.VatRate,
                                RateReduction = currentSale.RateReduction,
                                RateDiscount = currentSale.RateDiscount,
                                Transport = currentSale.Transport,
                                CompanyLogo = db.Files.FirstOrDefault(f => f.GlobalPersonID == cmpny.GlobalPersonID) != null ? db.Files.FirstOrDefault(f => f.GlobalPersonID == cmpny.GlobalPersonID).Content : COMPANY_LOGO
                            }
                        );
                        path = Server.MapPath("~/Reports/CashRegister/RptReceiptDeposit.rpt");
                        repName = "RptReceiptDeposit";
                        isValid = true;
                    }

                    if ((saleID == null || saleID == 0) && (DepositID <= 0))//depot pour au moins une vente
                    {

                        model.Add(
                            new
                            {
                                ReceiveAmount = receiveAmountTot,
                                TotalAmount = 0, //montant total de la facture
                                LineUnitPrice = 0, //reste du montant a verser
                                CompanyName = cmpny.Name,
                                CompanyAdress = cmpny.Adress.AdressFullName,// "Head Quater : " + cmpny.Adress.Quarter.Town.Region.RegionLabel + " - " + cmpny.Adress.Quarter.Town.TownLabel,
                                CompanyTel = "Tel: " + cmpny.Adress.AdressPhoneNumber + " Cell:" + cmpny.Adress.AdressCellNumber,
                                BranchName = curBranch.BranchName,
                                BranchAdress = "B.P " + cmpny.Adress.AdressPOBox + ", " + cmpny.Adress.Quarter.Town.TownLabel + ", Email: " + cmpny.Adress.AdressEmail,//currentSale.Branch.Adress.Quarter.QuarterLabel + " - " + currentSale.Branch.Adress.Quarter.Town.TownLabel,
                                BranchTel = "Tel: " + curBranch.Adress.AdressPhoneNumber,
                                Ref = _transactNumbeRepository.returnTransactNumber("REDP", businessDay),// currentSale.SaleReceiptNumber,
                                CompanyCNI = "NO CONT : " + cmpny.CNI,
                                Operator = CurrentUser.Name + " " + CurrentUser.Description,
                                CustomerName = customer.Name,// + " " + customer.Description,
                                CustomerAccount = customer.CNI,
                                SaleDate = businessDay.BDDateOperation,
                                Title = TitleDeposit,
                                DeviseLabel = devise.DeviseLabel,
                                CompanyLogo = db.Files.FirstOrDefault(f => f.GlobalPersonID == cmpny.GlobalPersonID) != null ? db.Files.FirstOrDefault(f => f.GlobalPersonID == cmpny.GlobalPersonID).Content : COMPANY_LOGO
                            }
                        );
                        path = Server.MapPath("~/Reports/CashRegister/RptReceiptSavingDeposit.rpt");
                        repName = "RptReceiptSavingDeposit";
                        isValid = true;
                    }

                    if (DepositID > 0) //depot d'epargne
                    {
                        Deposit deposit = _depositRepository.FindAll.FirstOrDefault(d => d.DepositID == DepositID);
                        curBranch = deposit.PaymentMethod1.Branch;

                        model.Add(
                            new
                            {
                                ReceiveAmount = receiveAmountTot,
                                TotalAmount = 0, //montant total de la facture
                                LineUnitPrice = 0, //reste du montant a verser
                                CompanyName = cmpny.Name,
                                CompanyAdress = cmpny.Adress.AdressFullName,// "Head Quater : " + cmpny.Adress.Quarter.Town.Region.RegionLabel + " - " + cmpny.Adress.Quarter.Town.TownLabel,
                                CompanyTel = "Tel: " + cmpny.Adress.AdressPhoneNumber + " Cell:" + cmpny.Adress.AdressCellNumber,
                                BranchName = curBranch.BranchName,
                                BranchAdress = "B.P " + cmpny.Adress.AdressPOBox + ", " + cmpny.Adress.Quarter.Town.TownLabel + ", Email: " + cmpny.Adress.AdressEmail,//currentSale.Branch.Adress.Quarter.QuarterLabel + " - " + currentSale.Branch.Adress.Quarter.Town.TownLabel,
                                BranchTel = "Tel: " + curBranch.Adress.AdressPhoneNumber,
                                Ref = deposit.DepositReference,// _transactNumbeRepository.returnTransactNumber("REDP", businessDay),// currentSale.SaleReceiptNumber,
                                CompanyCNI = "NO CONT : " + cmpny.CNI,
                                Operator = CurrentUser.Name + " " + CurrentUser.Description,
                                CustomerName = customer.Name,// + " " + customer.Description,
                                CustomerAccount = customer.CNI,
                                SaleDate = businessDay.BDDateOperation,
                                Title = TitleDeposit,
                                DeviseLabel = devise.DeviseLabel,
                                CompanyLogo = db.Files.FirstOrDefault(f => f.GlobalPersonID == cmpny.GlobalPersonID) != null ? db.Files.FirstOrDefault(f => f.GlobalPersonID == cmpny.GlobalPersonID).Content : COMPANY_LOGO
                            }
                        );



                        path = Server.MapPath("~/Reports/CashRegister/RptReceiptSavingDeposit.rpt");
                        repName = "RptReceiptSavingDeposit";
                        isValid = true;
                    }

                    if (isValid)
                    {
                        rptH.Load(path);
                        rptH.SetDataSource(model);
                        rptH.SetParameterValue("RptTitle", RptTitle);
                        bool isDownloadRpt = (bool)Session["isDownloadRpt"];
                        rptH.ExportToHttpResponse(ExportFormatType.PortableDocFormat, System.Web.HttpContext.Current.Response, isDownloadRpt, repName);
                        //return File(stream, "application/pdf");
                    }
                    else
                    {
                        Response.Write("Nothing Found; No Report name found");
                    }
                }
                else
                {
                    Response.Write("Nothing Found; Click on Deposit before print receipt");
                }
            }
            finally
            {
                rptH.Close();
                rptH.Dispose();
            }
        }

    }
}