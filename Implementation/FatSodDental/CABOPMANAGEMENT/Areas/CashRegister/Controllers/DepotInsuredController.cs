using FastSod.Utilities.Util;
using FatSod.DataContext.Initializer;
using FatSod.Ressources;
using FatSod.Security.Abstracts;
using FatSod.Security.Entities;
using FatSod.Supply.Abstracts;
using FatSod.Supply.Entities;
using CABOPMANAGEMENT.Controllers;
using CABOPMANAGEMENT.Filters;
using CABOPMANAGEMENT.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using FatSod.Report.WrapReports;

namespace CABOPMANAGEMENT.Areas.CashRegister.Controllers
{
    [Authorize(Order = 1)]
    [TakeBusinessDay(Order = 2)]
    public class DepotInsuredController : BaseController
    {
        //Current Controller and current page
        private const string CONTROLLER_NAME = "CashRegister/DepotInsured";
        private const string VIEW_NAME = "Index";
             
        private IBusinessDay _busDayRepo;
        private IDeposit _depositRepository;
        private ISavingAccount _savingAccountRepository;
        private ICustomerOrder _customerOrderRepository;
       
       
        private ITransactNumber _transactNumbeRepository;
        private ITillDay _tillDayRepository;
        private BusinessDay businessDay;
        
        public DepotInsuredController(
            IBusinessDay busDayRepo, 
            ISavingAccount saRepo,
            IDeposit depositRepository,
            ICustomerOrder customerOrderRepository,
            ITillDay tillDayRepository,
            ITransactNumber transactNumbeRepository
        )
        {
            this._busDayRepo = busDayRepo;
            this._savingAccountRepository = saRepo;
            this._depositRepository = depositRepository;
            this._transactNumbeRepository = transactNumbeRepository;
            this._customerOrderRepository = customerOrderRepository;
            this._tillDayRepository = tillDayRepository;
        }
        
        [OutputCache(Duration = 3600)] 
        public ActionResult Index()
        {
            ViewBag.DisplayForm = 1;
            try
            {
                
           
            List<BusinessDay> UserBusDays = (List<BusinessDay>)Session["UserBusDays"];
            Session["businessDay"] = UserBusDays.FirstOrDefault();
            DateTime currentDateOp = UserBusDays.FirstOrDefault().BDDateOperation;
            ViewBag.CurrentBranch = UserBusDays.FirstOrDefault().BranchID;
            ViewBag.BusnessDayDate = UserBusDays.FirstOrDefault().BDDateOperation;// businessDay.BDDateOperation;
            Session["BusnessDayDate"] =currentDateOp;

            UserTill userTill = (from td in db.UserTills
                                 where td.UserID == SessionGlobalPersonID
                                 select td).SingleOrDefault();
            if (userTill == null || userTill.TillID <= 0)
            {
                    TempData["Message"] = "Access Denied - You can't do sale operation. If you think no, try again or<br/>contact our administrator for this purpose<code/>.";
                    ViewBag.DisplayForm = 0;
                    return this.View();
                }

                TillDayStatus tState = _tillDayRepository.TillDayStatus(userTill.TillID);
                if (tState == null)
                {
                    TempData["Message"] = "Error - Bad Configuration of Cash Register!!! Please call Your database Administrator";
                    ViewBag.DisplayForm = 0;
                    return this.View();
                }
                if (!tState.IsOpen)
                {
                    TempData["Message"] = "Error - This Cash Register is Still Close!!! Please Open It Before Proceed";
                    ViewBag.DisplayForm = 0;
                    return this.View();
                }

                TillDay currentTillDay = (from t in db.TillDays
                                          where
                                              t.TillID == userTill.TillID && t.TillDayDate == tState.TillDayLastOpenDate.Date && t.IsOpen // t.TillDayDate.Day == currentDateOp.Day && t.TillDayDate.Month == currentDateOp.Month && t.TillDayDate.Year == currentDateOp.Year && t.IsOpen
                                          select t).FirstOrDefault();
                if (currentTillDay == null)
                {
                    TempData["Message"] = "Warnnig - Cash register is closed. You must open it before do any sale<br/>Go at Cash Register module=>Open cash register<code/>";
                    ViewBag.DisplayForm = 0;
                    return this.View();
                }


                ViewBag.CurrentTill = userTill.TillID;

                int deviseID = (Session["DefaultDeviseID"] == null) ? 0 : (int)Session["DefaultDeviseID"];
                if (deviseID <= 0)
                {
                    InjectUserConfigInSession();
                }
                deviseID = (Session["DefaultDeviseID"] == null) ? 0 : (int)Session["DefaultDeviseID"];
                ViewBag.DefaultDeviseID = deviseID;
                ViewBag.DefaultDevise = (deviseID <= 0) ? "" : db.Devises.Find(deviseID).DeviseCode;

                Session["DebtInsured"] = 0d;
            Session["CommandOderLines"] = new List<CustomerOrderLine>();
            return View(PendingCustomerCommand());
            }

            catch (Exception e)
            {
                TempData["Message"] = "Error " + e.Message;
                ViewBag.DisplayForm = 0;
                return this.View();
            }
        }

       


        public JsonResult PendingCustomerCommand()
        {
            var model = new
            {
                data = from command in this.ModelReturnAbleCommands()
                       select new
                       {
                           CustomerOrderID = command.CustomerOrderID,
                           CustomerOrderDate = command.CustomerOrderDate.ToString("yyyy-MM-dd"),
                           CustomerFullName = command.CustomerFullName,
                           CustomerOrderNumber = command.CustomerOrderNumber,
                           SaleTotalPrice = command.SaleTotalPrice,
                           Remainder = command.Remainder,
                           Remarque = command.Remarque,
                           MedecinTraitant = command.MedecinTraitant,
                           Advanced = command.Advanced
                       }
            };

            var jsonResult = Json(model, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = Int32.MaxValue;
            return jsonResult;

            // return Json(model, JsonRequestBehavior.AllowGet);

        }
        //retourne les ventes non delivre
        private List<CustomerOrder> ModelReturnAbleCommands()
        {
            double Advanced = 0d;
            //double Remainder = 0d;
            double SaleTotalPrice = 0d;
            List<CustomerOrder> model = new List<CustomerOrder>();
            try
            {

                var allSCommands = db.CustomerOrders.Where(sa => sa.BillState==StatutFacture.Validated)
                    .Select(a => new
                    {
                        CustomerOrderID = a.CustomerOrderID,
                        CustomerOrderLines=a.CustomerOrderLines,
                        RateReduction=a.RateReduction,
                        RateDiscount=a.RateDiscount,
                        Transport=a.Transport,
                        VatRate=a.VatRate,
                        CustomerOrderDate=a.CustomerOrderDate,
                        MedecinTraitant=a.MedecinTraitant,
                        Remarque=a.Remarque,
                        CustomerOrderNumber=a.CustomerOrderNumber,
                        CustomerName=a.CustomerName,
                        Plafond=a.Plafond
                    }).ToList().OrderBy(c => c.CustomerOrderNumber);

                //var allSCommands = db.CustomerOrders
                //        .Where(sa => sa.BillState == StatutFacture.Validated)
                //        .ToList()
                //        .Select(s => new CustomerOrder
                //        {
                //            CustomerOrderID = s.CustomerOrderID,
                //            //CustomerOrderLines=s.CustomerOrderLines,
                //        }).AsQueryable();
            
                //il faut mainteant vérifier si la vente à encore au moins une ligne de vente pouvant faire l'objet d'un retour
                foreach (var s in allSCommands)
                {
                    Advanced = 0d;
                    //Remainder = 0d;
                    SaleTotalPrice = 0d;

                    //CustomerOrder command = db.CustomerOrders.Find(s.CustomerOrderID);
                    if (s.CustomerOrderLines == null || s.CustomerOrderLines.Count <= 0) { continue; }
                    SaleTotalPrice = Util.ExtraPrices(s.CustomerOrderLines.Select(sl => sl.LineUnitPrice).Sum(), s.RateReduction, s.RateDiscount, s.Transport, s.VatRate).TotalTTC;
                    List<AllDeposit> listdepotInsure = db.AllDeposits.Where(c => c.CustomerOrderID == s.CustomerOrderID).ToList();
                    Advanced = (listdepotInsure.Count == 0) ? 0 : listdepotInsure.Select(c => c.Amount).Sum();

                    double plafond = (s.Plafond == null) ? 0 : s.Plafond;
                    double InsuredPart = SaleTotalPrice -(plafond+Advanced);

                    //Remainder = InsuredPart - Advanced;
                    //if (Remainder<=0)
                    //{
                    //    CustomerOrder existcustorder = db.CustomerOrders.Find(s.CustomerOrderID);
                    //    existcustorder.CustomerPartPaid = true;
                    //    db.SaveChanges();
                    //}

                    //if (InsuredPart > 0)
                    //{
                    //if (InsuredPart > 0)
                        model.Add(
                        new CustomerOrder
                        {
                            CustomerOrderID = s.CustomerOrderID,
                            CustomerOrderDate = s.CustomerOrderDate,
                            CustomerFullName = s.CustomerName,
                            CustomerOrderNumber = s.CustomerOrderNumber,
                            SaleTotalPrice = SaleTotalPrice,
                            Remainder = InsuredPart,
                            TotalMalade = SaleTotalPrice - plafond,
                            Remarque = s.Remarque,
                            MedecinTraitant = s.MedecinTraitant,
                            Advanced = Advanced
                        }
                        );
                    //}
                
                }

            }
            catch (Exception e)
            {
            }
            return model;
        }


        public JsonResult InitializeFields(int CustomerOrderID)
        {
            List<object> list = new List<object>();
            try
            {
                double Advanced = 0d;
                double Remainder = 0d;
                double SaleTotalPrice = 0d;

                if (CustomerOrderID==null || CustomerOrderID<=0)
                {
                     return Json(list, JsonRequestBehavior.AllowGet);
                }

                List<CustomerOrderLine> customerLineOderLines = db.CustomerOrderLines.Where(co => co.CustomerOrderID == CustomerOrderID).ToList();

                Session["CommandOderLines"] = customerLineOderLines;

                //we take sale and her salelines
                CustomerOrder customerOrder = db.CustomerOrders.Find(CustomerOrderID);
            
                if (customerOrder.CustomerOrderLines == null || customerOrder.CustomerOrderLines.Count <= 0)
                { 
                    SaleTotalPrice = 0; 
                }
                else { SaleTotalPrice = Util.ExtraPrices(customerOrder.CustomerOrderLines.Select(sl => sl.LineUnitPrice).Sum(), customerOrder.RateReduction, customerOrder.RateDiscount, customerOrder.Transport, customerOrder.VatRate).TotalTTC; }

                List<AllDeposit> listdepotInsure = db.AllDeposits.Where(c => c.CustomerOrderID == customerOrder.CustomerOrderID).ToList();
                Advanced = (listdepotInsure.Count == 0) ? 0 : listdepotInsure.Select(c => c.Amount).Sum();

                double plafond = (customerOrder.Plafond == null) ? 0 : customerOrder.Plafond;
                double InsuredPart = (SaleTotalPrice - plafond) < 0 ? 0 : SaleTotalPrice - plafond;

                Remainder = (InsuredPart - Advanced)<0 ?0: InsuredPart - Advanced;
                
                Session["DebtInsured"] = Remainder;
            
                Branch curBranch = db.Branches.Find(customerOrder.BranchID);
                businessDay = _busDayRepo.GetOpenedBusinessDay(curBranch);
            
                Session["Receipt_CustomerOrderID"] = CustomerOrderID;

                string DepositReference = _transactNumbeRepository.displayTransactNumber("REDP", businessDay);

                list.Add(
                new
                {
                    CustomerOrderID= CustomerOrderID,
                    DepositReference = DepositReference,
                    Debt = Remainder,
                    CompteurFacture = customerOrder.CompteurFacture,
                    BranchID = customerOrder.BranchID,
                    //CustomerID=customerOrder.CustomerID,
                    CustomerName = customerOrder.CustomerName,
                    CompanyName = customerOrder.CompanyName,
                    AssureurID = customerOrder.Assureur.AssureurFullName,
                    PoliceAssurance = customerOrder.PoliceAssurance,
                    CustomerOrderNumber= customerOrder.CustomerOrderNumber,
                    NumeroFacture= customerOrder.NumeroFacture,
                    CustomerOrderDate = customerOrder.CustomerOrderDate,
                    NumeroBonPriseEnCharge = customerOrder.NumeroBonPriseEnCharge
                });

                //var jsonResult = Json(list, JsonRequestBehavior.AllowGet);
                //jsonResult.MaxJsonLength = Int32.MaxValue;
                //return jsonResult;

                return Json(list, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(list, JsonRequestBehavior.AllowGet);
            }
        }

        
        public JsonResult DoDepositInsured(Deposit deposit)
        {
            bool status = false;
            string Message = "";
            try
            {
                Session["Receipt_SaleID"] = null;
                Session["Receipt_CustomerID"] = null;
                Session["ReceiveAmoung_ID"] = null;
                Session["Receip_DepositID"] = null;
                Session["TotalDette"] = null;
                Session["DepositReason"] = null;

                double SvDebtInsured = (double)Session["DebtInsured"];
                deposit.Debt = SvDebtInsured;

                Session["ReceiveAmoung_Tot"] = deposit.Amount;

                int customerOrderID = (int)Session["Receipt_CustomerOrderID"];
                deposit.CustomerOrderID = customerOrderID;

                deposit.DepositDate = SessionBusinessDay(null).BDDateOperation;

                Session["DepositPaymentMethod"] = deposit.PaymentMethod;
                //choix de la caisse
                if (deposit.PaymentMethod == CodeValue.Accounting.DefaultCodeAccountingSection.CODECAIS)
                {
                    int userTill = db.UserTills.FirstOrDefault(td => td.UserID == SessionGlobalPersonID).TillID;
                    deposit.PaymentMethodID = userTill;
                }

                if (deposit.PaymentMethod == CodeValue.Accounting.DefaultCodeAccountingSection.DIGITAL_PAYMENT)
                {
                    var manager = db.DigitalPaymentMethods.Find(deposit.PaymentMethodID);
                    deposit.DigitalAccountManagerId = manager?.AccountManagerId;
                }

                if (deposit.DepositReference.Trim() == "" || deposit.DepositReference == null)
                {
                    Message = "Deposit Reference - Sorry, The Deposit reference is null. Please Check the business day or contact your administrator ";
                    status = false;
                    return new JsonResult { Data = new { status = status, Message = Message } };
                }

                if (deposit.PaymentMethod == CodeValue.Accounting.DefaultCodeAccountingSection.DIGITAL_PAYMENT)
                {
                    var isPrimaryKeyViolated = IsTransactionIdentifierNew(deposit.TransactionIdentifier, deposit.PaymentMethodID);

                    if (isPrimaryKeyViolated)
                        throw new Exception("The Given Transaction Code (" + deposit.TransactionIdentifier + " has already been used");
                }

                deposit.DepositReason = "DEPOSIT FOR INSURED CUSTOMER";
                AllDeposit alldep= _depositRepository.SaleDepositForInsured(deposit, SessionGlobalPersonID);
               
                SetStateParameters(alldep);
                Message = Resources.Success + " - " + Resources.DepositSuccess;
                status = true;
                return new JsonResult { Data = new { status = status, Message = Message } };
            }
            catch (Exception e)
            {
                Message = "Error " + e.Message + " " + e.StackTrace + " " + e.InnerException;
                status = false;
                return new JsonResult { Data = new { status = status, Message = Message } };
            }

        }

        public void SetStateParameters(AllDeposit deposit)
        {
            //Session["Receipt_SaleID"] = deposit.SaleID;
            Session["Receipt_CustomerID"] = deposit.CustomerID;
            Session["ReceiveAmoung_ID"] = deposit.Amount;
            Session["Receip_DepositID"] = deposit.AllDepositID;
            Session["CustomerID"] = deposit.CustomerID;
            //Session["TotalDette"] = deposit.Debt;
            Session["DepositReason"] = deposit.AllDepositReason;
        }

        //chargement des combo box
        public JsonResult populatePaymentMethod()
        {
            var paymentMethodTypes = Utilities.PaymentMethodTypes();

            return Json(paymentMethodTypes, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DigitalPaymentMethods(string BuyTypeCode)
        {
            List<object> model = new List<object>();
            db.DigitalPaymentMethods.ToList().ForEach(dpm =>
            {
                model.Add(
                        new
                        {
                            ID = dpm.ID,
                            Name = dpm.Name + " (" + dpm.Code + ")"
                        }
                    );
            });
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public JsonResult PaymentMethods(string PaymentMethod)
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
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public ActionResult RptReceiptDeposit()
        {

            int AllDepositID = (int)Session["Receip_DepositID"];
            //we take sale and her salelines
            AllDeposit selectedDeposit = db.AllDeposits.Find(AllDepositID);

            Session["Receip_DepositID"] = AllDepositID;
            Session["ReceiveAmoung_Tot"] = selectedDeposit.Amount;
            Session["Receipt_CustomerID"] = selectedDeposit.CustomerID;
            Session["DepositReason"] = selectedDeposit.AllDepositReason;

            var model = GenerateReceiptDeposit();

            return View(model);

        }

        //This method print a receipt of customer
        public RptReceipt GenerateReceiptDeposit()
        {
            RptReceipt model = new RptReceipt();

            try
            {
                int customerID = (Session["Receipt_CustomerID"] == null) ? 0 : (int)Session["Receipt_CustomerID"];
                int DepositID = (Session["Receip_DepositID"] == null) ? 0 : (int)Session["Receip_DepositID"];
                double receiveAmountTot = (Session["ReceiveAmoung_Tot"] == null) ? 0 : (double)Session["ReceiveAmoung_Tot"];
                string DepositReason = (Session["DepositReason"] == null) ? "" : (string)Session["DepositReason"];


                string defaultImgPath = Server.MapPath("~/Content/Images/App/deboy-logo.png");
                byte[] COMPANY_LOGO = System.IO.File.ReadAllBytes(defaultImgPath);


                Devise devise = new Devise();
                Branch curBranch = new Branch();

                Customer customer = new Customer();

                if (customerID > 0)
                {
                    customer = (from c in db.Customers
                                where c.GlobalPersonID == customerID
                                select c).SingleOrDefault();

                    curBranch = db.Branches.Find(db.UserBranches.Where(ub => ub.UserID == SessionGlobalPersonID).FirstOrDefault().BranchID);
                    BusinessDay businessDay = _busDayRepo.GetOpenedBusinessDay(curBranch);
                    devise = db.Devises.FirstOrDefault(d => d.DefaultDevise);

                    Company cmpny = db.Companies.FirstOrDefault();
                    @ViewBag.CompanyLogoID = cmpny.GlobalPersonID;

                    if (DepositID > 0) //depot d'epargne
                    {
                        AllDeposit deposit = db.AllDeposits.Find(DepositID);
                        //curBranch = deposit.PaymentMethod1.Branch;
                        string TitleDeposit = "";
                        string RptTitle = "";
                        if (deposit.PaymentMethod is Till)
                        {
                            TitleDeposit = "Cash Paid In For " + DepositReason;
                            RptTitle = "CASH RECEIPT";
                        }
                        if (deposit.PaymentMethod is Bank)
                        {
                            TitleDeposit = "Bank Paid In For " + DepositReason;
                            RptTitle = "CHECK RECEIPT";
                        }
                        if (deposit.PaymentMethod is SavingAccount)
                        {
                            TitleDeposit = "Saving Paid In For " + DepositReason;
                            RptTitle = "SAVING RECEIPT";
                        }
                        model = new RptReceipt()
                        {
                            ReceiveAmount = receiveAmountTot,
                            TotalAmount = 0, //montant total de la facture
                            LineUnitPrice = 0, //reste du montant a verser
                            CompanyName = cmpny.Name,
                            CompanyAdress = "Head Quater : " + cmpny.Adress.Quarter.Town.Region.RegionLabel + " - " + cmpny.Adress.Quarter.Town.TownLabel,
                            CompanyTel = "Tel: " + cmpny.Adress.AdressPhoneNumber,
                            BranchName = curBranch.BranchName,
                            BranchAdress = curBranch.Adress.Quarter.QuarterLabel + " - " + curBranch.Adress.Quarter.Town.TownLabel,
                            BranchTel = "Tel: " + curBranch.Adress.AdressPhoneNumber + " - Cel: " + curBranch.Adress.AdressCellNumber,
                            Ref = deposit.AllDepositReference,
                            CompanyCNI = "NO CONT : " + cmpny.CNI,
                            Operator = CurrentUser.Name + " " + CurrentUser.Description,
                            CustomerName = deposit.Representant,// customer.Name,// + " " + customer.Description,
                            CustomerAccount = customer.AccountNumber,
                            SaleDate = deposit.AllDepositDate.Date,
                            SaleDateUI = deposit.AllDepositDate.Date.ToString("dd/MM/yyyy"),
                            Title = TitleDeposit,
                            DeviseLabel = RptTitle,
                            CompanyRC = Company.CompanyTradeRegister,
                            CompanyLogo = db.Files.FirstOrDefault(f => f.GlobalPersonID == cmpny.GlobalPersonID) != null ? db.Files.FirstOrDefault(f => f.GlobalPersonID == cmpny.GlobalPersonID).Content : COMPANY_LOGO
                        };

                    }

                }
                else
                {
                    Response.Write("Nothing Found; Click on Deposit before print receipt");
                }
                return model;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private Company Company
        {
            get
            {
                return db.Companies.FirstOrDefault();
            }
        }

    }
}