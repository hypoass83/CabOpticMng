using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using Ext.Net;
using Ext.Net.MVC;
using FastSod.Utilities.Util;
using FatSod.DataContext.Concrete;
using FatSod.DataContext.Initializer;
using FatSod.Report.WrapReports;
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
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using SaleE = FatSod.Supply.Entities.Sale;

namespace FatSodDental.UI.Areas.CashRegister.Controllers
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
       
        private List<BusinessDay> lstBusDay;


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
                X.Msg.Alert("Access Denied", "You can't do sale operation. If you think no, try again or<br/>contact our administrator for this purpose<code/>.").Show();
                return this.Direct();
            }
            //TillDayStatus tState = _tillDayRepository.TillDayStatus(userTill.Till);
            //if (tState == null)
            //{
            //    X.Msg.Alert("Error", "Bad Configuration of Cash Register!!! Please call Your database Administrator").Show();
            //    return this.Direct();
            //}
            //if (!tState.IsOpen)
            //{
            //    X.Msg.Alert("Error", "This Cash Register is Still Close!!! Please Open It Before Proceed").Show();
            //    return this.Direct();
            //}

            //TillDay currentTillDay = (from t in db.TillDays
            //                          where
            //                              t.TillID == userTill.TillID && t.TillDayDate == tState.TillDayLastOpenDate.Date && t.IsOpen // t.TillDayDate.Day == currentDateOp.Day && t.TillDayDate.Month == currentDateOp.Month && t.TillDayDate.Year == currentDateOp.Year && t.IsOpen
            //                          select t).FirstOrDefault();

            //if (currentTillDay == null)
            //{
            //    X.Msg.Alert("Warnnig", "Cash register is closed. You must open it before do any sale<br/>Go at Cash Register module=>Open cash register<code/>").Show();
            //    return this.Direct();
            //}
            Session["DebtInsured"] = 0d;
            Session["CommandOderLines"] = new List<CustomerOrderLine>();
            return View(ModelReturnAbleCommands(ViewBag.CurrentBranch,currentDateOp));
            }

            catch (Exception e)
            {
                X.Msg.Alert("Error ", e.Message).Show();
                return this.Direct();
            }
        }

        [HttpPost]
        public StoreResult CommandOderLines()
        {
            List<CustomerOrderLine> dataTmp = (List<CustomerOrderLine>)Session["CommandOderLines"];
            List<object> model = new List<object>();
            dataTmp.ForEach(c =>
            {
                model.Add(
                        new
                        {
                            LineID = c.LineID,
                            LineAmount = c.LineAmount,
                            LineQuantity = c.LineQuantity,
                            ProductLabel = c.ProductLabel,
                            LineUnitPrice = c.LineUnitPrice
                        }
                    );
            });
            return this.Store(model);
        }
        
        public ActionResult GenerateBill(int Assureur)
        {
            int numerofacture = 0;
            Assureur assure = db.Assureurs.Find(Assureur);
            if (assure!=null)
            {
                numerofacture = assure.CompteurFacture;
            }
            string Numerofacture =_customerOrderRepository.generateBill(numerofacture);

            this.GetCmp<TextField>("NumeroFacture").Value = Numerofacture;
            return this.Direct();
        }
        //retourne les ventes non delivre
        private List<object> ModelReturnAbleCommands(int BranchID,DateTime SoldDate)
        {

            double Advanced = 0d;
            double Remainder = 0d;
            double SaleTotalPrice = 0d;
            List<object> model = new List<object>();
           
             
            var allSCommands = db.CustomerOrders
                    .Where(sa => sa.BranchID == BranchID && sa.CustomerOrderDate == SoldDate.Date && !sa.IsDelivered && sa.BillState == StatutFacture.Validated)
                    .ToList()
                    .Select(s => new CustomerOrder
                    {
                        CustomerOrderID = s.CustomerOrderID
                    }).AsQueryable();
            
            //il faut mainteant vérifier si la vente à encore au moins une ligne de vente pouvant faire l'objet d'un retour
            foreach (CustomerOrder s in allSCommands)
            {
                CustomerOrder command = db.CustomerOrders.Find(s.CustomerOrderID);
                if (command.CustomerOrderLines == null || command.CustomerOrderLines.Count <= 0) { continue; }
                SaleTotalPrice = Util.ExtraPrices(command.CustomerOrderLines.Select(sl => sl.LineUnitPrice).Sum(), command.RateReduction, command.RateDiscount, command.Transport, command.VatRate).TotalTTC;
                
                Remainder = SaleTotalPrice - Advanced;
                model.Add(
                            new
                            {
                                CustomerOrderID = command.CustomerOrderID,
                                CustomerOrderDate = command.CustomerOrderDate,
                                CustomerFullName = command.CustomerName,
                                CustomerOrderNumber = command.CustomerOrderNumber,
                                SaleTotalPrice = SaleTotalPrice,
                                Remainder=Remainder,
                                Remarque = command.Remarque,
                                MedecinTraitant = command.MedecinTraitant
                            }
                          );
            }


            return model;
        }

        private List<object> ModelReturnAbleCommands(int BranchID, String SearchOption, String SearchValue)
        {
            double Advanced = 0d;
            double Remainder = 0d;
            double SaleTotalPrice = 0d;
            List<CustomerOrder> dataTmp = new List<CustomerOrder>();

            if (SearchValue == null || SearchValue == "" || SearchOption == null || SearchOption == "")
            {
                dataTmp = (from custOrd in db.CustomerOrders
                           where !custOrd.IsDelivered && custOrd.BillState == StatutFacture.Validated
                           select custOrd)
                               .OrderByDescending(a => a.CustomerName)
                                         .Take(1000)
                                         .ToList();
            }
            else
            {
                if (SearchOption == "NAME") //si recherche par NAME
                {
                    dataTmp = (from custOrd in db.CustomerOrders
                               where custOrd.CustomerName.Contains(SearchValue) && !custOrd.IsDelivered
                                && custOrd.BillState == StatutFacture.Validated
                               select custOrd)
                                   .OrderByDescending(a => a.CustomerName)
                                             .Take(1000)
                                             .ToList();
                }

                if (SearchOption == "NUMBER") //si recherche par ref number
                {
                    dataTmp = (from custOrd in db.CustomerOrders
                               where custOrd.CustomerOrderNumber.Contains(SearchValue) && !custOrd.IsDelivered
                                && custOrd.BillState == StatutFacture.Validated
                               select custOrd)
                                   .OrderByDescending(a => a.CustomerOrderNumber)
                                             .Take(1000)
                                             .ToList();
                }


            }


            List<object> realDataTmp = new List<object>();

            foreach (var command in dataTmp)
            {
                SaleTotalPrice = Util.ExtraPrices(command.CustomerOrderLines.Select(sl => sl.LineUnitPrice).Sum(), command.RateReduction, command.RateDiscount, command.Transport, command.VatRate).TotalTTC;

                Remainder = SaleTotalPrice - Advanced;
                realDataTmp.Add(
                            new
                            {
                                CustomerOrderID = command.CustomerOrderID,
                                CustomerOrderDate = command.CustomerOrderDate,
                                CustomerFullName = command.CustomerName,
                                CustomerOrderNumber = command.CustomerOrderNumber,
                                SaleTotalPrice = SaleTotalPrice,
                                Remainder = Remainder,
                                Remarque = command.Remarque,
                                MedecinTraitant = command.MedecinTraitant
                            }
                          );
                
            }
            return realDataTmp;
        }


        [HttpPost]
        public ActionResult InitializeFields(int CustomerOrderID)
        {
            try
            {
            double Advanced = 0d;
            double Remainder = 0d;
            double SaleTotalPrice = 0d;
            if (CustomerOrderID==null || CustomerOrderID<=0)
            {
                X.Msg.Alert("Select", "Error while select customer Order").Show();
                return this.Direct();
            }

            List<CustomerOrderLine> customerLineOderLines = db.CustomerOrderLines.Where(co => co.CustomerOrderID == CustomerOrderID).ToList();

            Session["CommandOderLines"] = customerLineOderLines;

            //we take sale and her salelines
            CustomerOrder customerOrder = db.CustomerOrders.Find(CustomerOrderID);
            
            if (customerOrder.CustomerOrderLines == null || customerOrder.CustomerOrderLines.Count <= 0)
            { SaleTotalPrice = 0; }
            else { SaleTotalPrice = Util.ExtraPrices(customerOrder.CustomerOrderLines.Select(sl => sl.LineUnitPrice).Sum(), customerOrder.RateReduction, customerOrder.RateDiscount, customerOrder.Transport, customerOrder.VatRate).TotalTTC; }
            //Customer customerSelect = db.Customers.Find(customerOrder.AssureurID);

            Remainder = SaleTotalPrice - Advanced;

            this.GetCmp<TextField>("CompteurFacture").Value = customerOrder.CompteurFacture;
            this.GetCmp<TextField>("BranchID").Value = customerOrder.BranchID;
            this.GetCmp<TextField>("CustomerName").Value = customerOrder.CustomerName;
            this.GetCmp<TextField>("CompanyName").Value = customerOrder.CompanyName;
            this.GetCmp<TextField>("AssureurID").Value = customerOrder.Assureur.AssureurFullName;//.AssureurID;
            this.GetCmp<TextField>("PoliceAssurance").Value = customerOrder.PoliceAssurance;
            this.GetCmp<TextField>("CompteurFacture").Value = customerOrder.CompteurFacture;
            this.GetCmp<TextField>("CustomerOrderNumber").Value = customerOrder.CustomerOrderNumber;
            this.GetCmp<TextField>("NumeroFacture").Value = customerOrder.NumeroFacture;
            this.GetCmp<DateField>("CustomerOrderDate").Value = customerOrder.CustomerOrderDate;
            this.GetCmp<TextField>("NumeroBonPriseEnCharge").Value = customerOrder.NumeroBonPriseEnCharge;
            this.GetCmp<TextField>("CustomerOrderNumber").Value = customerOrder.CustomerOrderNumber;

            double LensPrice = Util.ExtraPrices(customerOrder.CustomerOrderLines.Where(l => l.Product.Category.CategoryID != 1 && l.Product.Category.CategoryID != 2).Select(sl => sl.LineUnitPrice).Sum(), customerOrder.RateReduction, customerOrder.RateDiscount, customerOrder.Transport, customerOrder.VatRate).TotalTTC;
            double MonturePrice = Util.ExtraPrices(customerOrder.CustomerOrderLines.Where(l => l.Product.Category.CategoryID == 1).Select(sl => sl.LineUnitPrice).Sum(), customerOrder.RateReduction, customerOrder.RateDiscount, customerOrder.Transport, customerOrder.VatRate).TotalTTC;
            double TotalPrice = LensPrice + MonturePrice;
            double partAssuVerre = (customerOrder.VerreAssurance == null) ? 0 : customerOrder.VerreAssurance;
            double partAssMonture = (customerOrder.MontureAssurance == null) ? 0 : customerOrder.MontureAssurance;
            double TotalPartAssurance = partAssuVerre + partAssMonture;
            double TotalPartMalade = TotalPrice - TotalPartAssurance;
            string SpecialOrderLineCode = customerOrder.CustomerOrderLines.FirstOrDefault().SpecialOrderLineCode;
            //calcul du montant deja verse
            List<SaleLine> lstSale = db.SaleLines.Where(s => s.SpecialOrderLineCode == SpecialOrderLineCode).ToList();

            double DejaVerse = Util.ExtraPrices(lstSale.Select(l => l.LineAmount).Sum(), customerOrder.RateReduction, customerOrder.RateDiscount, customerOrder.Transport, customerOrder.VatRate).TotalTTC;
            this.GetCmp<NumberField>("Debt").Value = TotalPartMalade - DejaVerse;
            Session["DebtInsured"] = TotalPartMalade - DejaVerse;
            
            Branch curBranch = db.Branches.Find(customerOrder.BranchID);
            businessDay = _busDayRepo.GetOpenedBusinessDay(curBranch);
            this.GetCmp<TextField>("DepositReference").Value = _transactNumbeRepository.displayTransactNumber("REDP", businessDay);
            
            Session["Receipt_CustomerOrderID"] = CustomerOrderID;
            
            return this.Direct();
            }
            catch (Exception e) { X.Msg.Alert("Error ", e.Message).Show(); return this.Direct(); }
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

        //this method return a different payment method of one type
        public StoreResult PaymentMethods(string BuyTypeCode)
        {
            return this.Store(LoadComponent.SpecificBankPaymentMethod(BuyTypeCode));
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
                this.GetCmp<DateField>("CustomerOrderDate").Value = businessD.BDDateOperation;
            }
            return this.Direct();
        }

        public ActionResult DoDepositInsured(Deposit deposit)
        {
            try
            {
                int depositID = 0; 
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

                    double SavingAmount = _savingAccountRepository.GetSavingAccountBalance(db.Customers.Find(deposit.CustomerID));
                    //Montant total d'argent dans le compte du client
                    double savingAccountBalance = SavingAmount; // deposit.SavingAmount;

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

                }

                _depositRepository.SaleDepositForInsured(deposit, SessionGlobalPersonID);
               
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
        
        public ActionResult Reset()
        {
            this.ReloadSalesListStore();
            this.GetCmp<FormPanel>("DepositForm").Reset();
            return this.Direct();
        }
        [HttpPost]
        public ActionResult ResetDepositForm()
        {
          
            this.ReloadSalesListStore();

            this.GetCmp<Button>("btnSave").Disabled = true;
            this.GetCmp<Button>("btnPrintFacture").Disabled = false;
            return this.Direct();
        }
        public ActionResult ReloadSalesListStore()
        {
            this.GetCmp<Store>("SalesListStore").Reload();
            return this.Direct();
        }
        public StoreResult PendingCustomerSale(int BranchID, DateTime SoldDate, String SearchOption, String SearchValue)
        {
            if (SearchValue.Trim().Length == 0)
            {
                return this.Store(this.ModelReturnAbleCommands(BranchID, SoldDate));
            }
            else
            {
                return this.Store(this.ModelReturnAbleCommands(BranchID, SearchOption, SearchValue));
            }
        }
       

        //This method load a method that print a receip of deposit
        public ActionResult PrintFacture(double VerreAssurance, double MontureAssurance)
        {
            //Session["TxtPrisEnChargeAssureur"] = TauxPrisenCharge;
            Session["VerreAssureur"] = VerreAssurance;
            Session["MontureAssurance"] = MontureAssurance;
            this.GetCmp<Panel>("Pdf").LoadContent(new ComponentLoader
            {
                Url = Url.Action("GenerateFacture"),
                DisableCaching = false,
                Mode = LoadMode.Frame,
            });

            return this.Direct();
        }
        //This method print a receipt of customer
        public void GenerateFacture()
        {
            List<object> model = new List<object>();
            List<object> modelsubRpt = new List<object>();
            ReportDocument rptH = new ReportDocument();
            try
            {
            int customerOrderID = (Session["Receipt_CustomerOrderID"]==null)? 0 : (int)Session["Receipt_CustomerOrderID"];
            //double VerreAssurance = (Session["VerreAssurance"] == null) ? 0 : (double)Session["VerreAssurance"];
            //double MontureAssurance = (Session["MontureAssurance"] == null) ? 0 : (double)Session["MontureAssurance"];

            //double PartAssureur= VerreAssurance+MontureAssurance;
            
            string repName = "";
            bool isValid = false;
            double totalAmount = 0d;
            double totalRemaining = 0d;

            //double TotalReceiveAmount = VerreAssurance + MontureAssurance;

            
            string path = "";
            string defaultImgPath = Server.MapPath("~/Content/Images/App/deboy-logo.png");
            byte[] COMPANY_LOGO = System.IO.File.ReadAllBytes(defaultImgPath);

            string TitleDeposit = "";
            string RptTitle = "";

            Company cmpny = db.Companies.FirstOrDefault();
            if (customerOrderID > 0)//depot pour une vente
            {

                double saleAmount = 0d;
                CustomerOrder currentOrder = db.CustomerOrders.Find(customerOrderID);

                string Prescription = "";
                //recuperation des versements
                List<CustomerOrderLine> lstOrderLine = db.CustomerOrderLines.Where(sl => sl.CustomerOrderID == currentOrder.CustomerOrderID).ToList();
                //totalAmount = (lstOrderLine.Count > 0) ? Util.ExtraPrices(lstOrderLine.Select(c => c.LineUnitPrice).Sum(), currentOrder.RateReduction, currentOrder.RateDiscount, currentOrder.Transport, currentOrder.VatRate).TotalTTC : 0; //montant du verre
                //totalRemaining = totalAmount - TotalReceiveAmount;
                totalAmount = currentOrder.Plafond; // currentOrder.MontureAssurance + currentOrder.VerreAssurance;
                string montantLettre = NumberConverter.Spell((ulong)totalAmount).ToUpper();
                foreach (CustomerOrderLine custOrderLine in lstOrderLine)
                {
                    string labelFrame = (custOrderLine.marque != null && custOrderLine.reference != null) ? Resources.Marque.ToUpper() + " " + custOrderLine.marque + " " + Resources.Reference.ToUpper() + " " + custOrderLine.reference : "";
                    int i = (custOrderLine.marque != null && custOrderLine.reference != null) ? 2 : 1;
                    if (labelFrame.Trim().Length > 0)
                    {
                        Prescription = labelFrame;
                    }
                    else
                    {
                        if (custOrderLine.Product.Prescription!=null)
                        {
                            Prescription = custOrderLine.Product.Prescription;
                        }
                        else
                        {
                            Prescription = (custOrderLine.Product.ProductCode.Contains(" HD ")) ? custOrderLine.Product.ProductCode.Replace(custOrderLine.Product.Category.CategoryCode + " HD", "") : custOrderLine.Product.ProductCode.Replace(custOrderLine.Product.Category.CategoryCode, "");
                        }
                    }
                    model.Add(
                    new //RptReceipt
                    {
                        RptReceiptID = 1,
                        ReceiveAmount = (labelFrame.Trim().Length > 0) ? currentOrder.MontureAssurance : Math.Round(currentOrder.VerreAssurance / 2), //montant du verre TotalReceiveAmount, //part assureur
                        TotalAmount = totalAmount, //montant restant de la facture
                        LineUnitPrice = (labelFrame.Trim().Length > 0) ? currentOrder.MontureAssurance : Math.Round(currentOrder.VerreAssurance / 2), //montant du verre
                        CompanyName = cmpny.Adress.AdressFullName,
                        CompanyAdress = "B.P. " + cmpny.Adress.AdressPOBox + ", " + cmpny.Adress.Quarter.Town.TownLabel + ", Email:" + cmpny.Adress.AdressEmail,// "Head Quater : " + cmpny.Adress.Quarter.Town.Region.RegionLabel + " - " + cmpny.Adress.Quarter.Town.TownLabel,
                        CompanyTel = "Tel: " + cmpny.Adress.AdressPhoneNumber + ", Fax: " + cmpny.Adress.AdressFax + ", Cell: " + cmpny.Adress.AdressCellNumber,
                        CustomerAdress = currentOrder.NumeroBonPriseEnCharge,// "No.ONOC:87 / ONOC",
                        BranchName = currentOrder.Branch.BranchName,
                        BranchAdress = currentOrder.Branch.Adress.Quarter.QuarterLabel + " - " + currentOrder.Branch.Adress.Quarter.Town.TownLabel,
                        BranchTel = "Tel: " + currentOrder.Branch.Adress.AdressPhoneNumber,
                        Reference = currentOrder.NumeroFacture,
                        CompanyCNI = "NO CONT : " + cmpny.CNI,
                        Operator = CurrentUser.Name + " " + CurrentUser.Description,
                        CustomerName = currentOrder.CustomerName,// customerRpt.Name + " " + customerRpt.Description,
                        ProductLabel = Prescription, //(labelFrame.Trim().Length > 0) ? labelFrame : custOrderLine.Product.Prescription,
                        SaleDate = currentOrder.CustomerOrderDate,
                        Title = cmpny.Adress.Quarter.Town.TownLabel,
                        MontantLettre = montantLettre,//MONTANT FACTURE,
                        RateTVA = currentOrder.VatRate,
                        RateReduction = currentOrder.RateReduction,
                        RateDiscount = currentOrder.RateDiscount,
                        Transport = currentOrder.Transport,
                        RptReceiptPaymentDetailID = i,
                        LineQuantity = (labelFrame.Trim().Length > 0) ? custOrderLine.LineQuantity : custOrderLine.LineQuantity * 2,
                        CustomerAccount = currentOrder.Assureur.Name, //nom de la societe assureur
                        BranchAbbreviation = currentOrder.CompanyName, //NOM DE LA SOCIETE DU CLIENT
                        DeviseLabel = currentOrder.PoliceAssurance,//police d'assurance
                        ProductRef = (labelFrame.Trim().Length > 0) ? "MONTURE MATIERE :" + custOrderLine.FrameCategory : custOrderLine.Product.Category.CategoryDescription,
                        CompanyLogo = db.Files.FirstOrDefault(f => f.GlobalPersonID == cmpny.GlobalPersonID) != null ? db.Files.FirstOrDefault(f => f.GlobalPersonID == cmpny.GlobalPersonID).Content : COMPANY_LOGO
                    }
                );
                    //
                }

                if (currentOrder.DatailBill==0)
                {
                    path = Server.MapPath("~/Reports/CashRegister/RptFactureAssurance.rpt");
                    repName = "RptFactureAssurance";
                }
                else
                {
                    path = Server.MapPath("~/Reports/CashRegister/RptFactureAssuranceDetail.rpt");
                    repName = "RptFactureAssuranceDetail";
                }
                

                isValid = true;
            }


            if (isValid)
            {
                FatSod.Ressources.Resources.Culture = System.Globalization.CultureInfo.CreateSpecificCulture("fr");
                rptH.Load(path);
                rptH.SetDataSource(model);
               
                //rptH.SetParameterValue("RptTitle", RptTitle);
                bool isDownloadRpt = (bool)Session["isDownloadRpt"];
                rptH.ExportToHttpResponse(ExportFormatType.PortableDocFormat, System.Web.HttpContext.Current.Response, isDownloadRpt, repName);
                //return File(stream, "application/pdf");
            }
            else
            {
                Response.Write("Nothing Found; No Report name found");
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