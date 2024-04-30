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
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using SaleE = FatSod.Supply.Entities.Sale;

namespace FatSodDental.UI.Areas.CashRegister.Controllers
{
    [Authorize(Order = 1)]
    [TakeBusinessDay(Order = 2)]
    public class DepositController : BaseController
    {
        //Current Controller and current page
        private const string CONTROLLER_NAME = "CashRegister" + "/" + CodeValue.Supply.DepositMenu.CONTROLLER;
        private const string VIEW_NAME = CodeValue.Supply.DepositMenu.PATH;
             
        private IBusinessDay _busDayRepo;
        private IDeposit _depositRepository;
        private ISavingAccount _savingAccountRepository;

       
        private List<BusinessDay> lstBusDay;


        private ITransactNumber _transactNumbeRepository;

        private BusinessDay businessDay;
        
        public DepositController(
            IBusinessDay busDayRepo, 
            ISavingAccount saRepo,
            IDeposit depositRepository,
            ITransactNumber transactNumbeRepository
        )
        {
            this._busDayRepo = busDayRepo;
            this._savingAccountRepository = saRepo;
            this._depositRepository = depositRepository;
            this._transactNumbeRepository = transactNumbeRepository;
            
        }
        
        [OutputCache(Duration = 3600)] 
        public ActionResult Deposit()
        {
            try
            {
                
            //we ensure that if this user manage cash register. If he manage it, will verify if till is closed else, we ask he to closed it before login off
            int userTill = (from td in db.UserTills
                            where td.UserID == SessionGlobalPersonID
                            select td).FirstOrDefault().TillID;
            if (userTill == null || userTill <= 0)
            {
                X.Msg.Alert("Access Denied", "You can't do sale operation. If you think no, try again or<br/>contact our administrator for this purpose<code/>.").Show();
                return this.Direct();
            }
            List<BusinessDay> UserBusDays = (List<BusinessDay>)Session["UserBusDays"];
            Session["businessDay"] = UserBusDays.FirstOrDefault();
            DateTime currentDateOp = UserBusDays.FirstOrDefault().BDDateOperation;
            ViewBag.CurrentBranch = UserBusDays.FirstOrDefault().BranchID;
            ViewBag.BusnessDayDate = UserBusDays.FirstOrDefault().BDDateOperation;// businessDay.BDDateOperation;
            Session["BusnessDayDate"] =currentDateOp;

            TillDay currentTillDay = (from t in db.TillDays
                                      where
                                          t.TillID == userTill && t.TillDayDate.Day == currentDateOp.Day && t.TillDayDate.Month == currentDateOp.Month && t.TillDayDate.Year == currentDateOp.Year && t.IsOpen
                                      select t).FirstOrDefault();

            if (currentTillDay == null)
            {
                X.Msg.Alert("Warnnig", "Cash register is closed. You must open it before do any sale<br/>Go at Cash Register module=>Open cash register<code/>").Show();
                return this.Direct();
            }


            return View(ModelReturnAbleSales(ViewBag.CurrentBranch,currentDateOp));
            }

            catch (Exception e)
            {
                X.Msg.Alert("Error ", e.Message).Show();
                return this.Direct();
            }
        }

        private List<object> ModelReturnAbleSales(int BranchID, String SearchOption, String SearchValue)
        {
            double Advanced = 0d;
            double Remainder = 0d;
            double SaleTotalPrice = 0d;
            List<SaleE> dataTmp = new List<SaleE>();

            if (SearchValue == null || SearchValue == "" || SearchOption == null || SearchOption == "")
            {
                dataTmp = (from saleOrd in db.Sales
                           where !saleOrd.IsPaid
                           select saleOrd)
                               .OrderByDescending(a => a.CustomerName)
                                         .Take(1000)
                                         .ToList();
            }
            else
            {
                if (SearchOption == "NAME") //si recherche par NAME
                {
                    dataTmp = (from saleOrd in db.Sales
                               where saleOrd.CustomerName.Contains(SearchValue) && !saleOrd.IsPaid
                               select saleOrd)
                                   .OrderByDescending(a => a.CustomerName)
                                             .Take(1000)
                                             .ToList();
                }

                if (SearchOption == "NUMBER") //si recherche par ref number
                {
                    dataTmp = (from saleOrd in db.Sales
                               where saleOrd.SaleReceiptNumber.Contains(SearchValue) && !saleOrd.IsPaid
                               select saleOrd)
                                   .OrderByDescending(a => a.SaleReceiptNumber)
                                             .Take(1000)
                                             .ToList();
                }


            }


            List<object> realDataTmp = new List<object>();

            foreach (var sale in dataTmp)
            {
                SaleTotalPrice = Util.ExtraPrices(sale.SaleLines.Select(sl => sl.LineAmount).Sum(), sale.RateReduction, sale.RateDiscount, sale.Transport, sale.VatRate).TotalTTC;
                List<CustomerSlice> lstCustSlice = db.CustomerSlices.Where(sl => sl.SaleID == sale.SaleID).ToList();
                if (lstCustSlice != null && lstCustSlice.Count > 0)
                {
                    Advanced = lstCustSlice.Select(sl => sl.SliceAmount).Sum();
                }
                else
                {
                    Advanced = 0;
                }
                Remainder = SaleTotalPrice - Advanced;
                realDataTmp.Add(
                            new
                            {
                                SaleID = sale.SaleID,
                                SaleDate = sale.SaleDate,
                                SaleDeliveryDate = sale.SaleDeliveryDate,
                                CustomerFullName = sale.CustomerName,
                                SaleReceiptNumber = sale.SaleReceiptNumber,
                                SaleTotalPrice = SaleTotalPrice,
                                Advanced = Advanced,
                                Remainder = Remainder,
                                Remarque = sale.Remarque,
                                MedecinTraitant = sale.MedecinTraitant
                            }
                          );

            }
            return realDataTmp;
        }
        //retourne les ventes non delivre
        private List<object> ModelReturnAbleSales(int BranchID,DateTime SoldDate)
        {

            double Advanced = 0d;
            double Remainder = 0d;
            double SaleTotalPrice = 0d;
            List<object> model = new List<object>();
           
            var allSales = db.Sales
                    .Where(sa => sa.BranchID == BranchID && sa.SaleDate == SoldDate.Date && !sa.IsPaid)
                    .ToList()
                    .Select(s => new SaleE
                    {
                        SaleID = s.SaleID
                    }).AsQueryable();

            //il faut mainteant vérifier si la vente à encore au moins une ligne de vente pouvant faire l'objet d'un retour
            foreach (SaleE s in allSales)
            {
                SaleE sale = db.Sales.Find(s.SaleID);
                if (sale.SaleLines == null || sale.SaleLines.Count <= 0) { continue; }
                SaleTotalPrice = Util.ExtraPrices(sale.SaleLines.Select(sl => sl.LineAmount).Sum(), sale.RateReduction, sale.RateDiscount, sale.Transport, sale.VatRate).TotalTTC;
                List<CustomerSlice> lstCustSlice = db.CustomerSlices.Where(sl => sl.SaleID == sale.SaleID).ToList();
                if (lstCustSlice != null && lstCustSlice.Count > 0)
                {
                    Advanced = lstCustSlice.Select(sl => sl.SliceAmount).Sum();
                }
                else
                {
                    Advanced = 0;
                }
                Remainder = SaleTotalPrice - Advanced;
                model.Add(
                            new
                            {
                                SaleID = sale.SaleID,
                                SaleDate = sale.SaleDate,
                                SaleDeliveryDate = sale.SaleDeliveryDate,
                                CustomerFullName = sale.CustomerName,
                                SaleReceiptNumber = sale.SaleReceiptNumber,
                                SaleTotalPrice = SaleTotalPrice,
                                Advanced=Advanced,
                                Remainder=Remainder,
                                Remarque = sale.Remarque,
                                MedecinTraitant = sale.MedecinTraitant
                            }
                          );
            }


            return model;
        }

        public ActionResult loadGrid()
        {
            this.GetCmp<Store>("SalesListStore").Reload();
            return this.Direct();
        }

        [HttpPost]
        public ActionResult InitializeFields(int SaleID)
        {
            try
            {
            double Advanced = 0d;
            double Remainder = 0d;
            double SaleTotalPrice = 0d;
            //we take sale and her salelines
            SaleE sale = db.Sales.Find(SaleID);

            if (sale.SaleLines == null || sale.SaleLines.Count <= 0)
            { SaleTotalPrice = 0; }
            else { SaleTotalPrice = Util.ExtraPrices(sale.SaleLines.Select(sl => sl.LineAmount).Sum(), sale.RateReduction, sale.RateDiscount, sale.Transport, sale.VatRate).TotalTTC; }

            List<CustomerSlice> lstCustSlice = db.CustomerSlices.Where(sl => sl.SaleID == sale.SaleID).ToList();
            if (lstCustSlice != null && lstCustSlice.Count > 0)
            {
                Advanced = lstCustSlice.Select(sl => sl.SliceAmount).Sum();
            }
            else
            {
                Advanced = 0;
            }
            Remainder = SaleTotalPrice - Advanced;

            this.GetCmp<TextField>("SaleID").Value = sale.SaleID;
            this.GetCmp<ComboBox>("BranchID").Value = sale.BranchID;
            this.GetCmp<TextField>("CustomerName").Value = sale.CustomerName;
            this.GetCmp<NumberField>("Debt").Value = Remainder;
            Branch curBranch = db.Branches.Find(sale.BranchID);
            businessDay = _busDayRepo.GetOpenedBusinessDay(curBranch);
            this.GetCmp<TextField>("DepositReference").Value = _transactNumbeRepository.displayTransactNumber("REDP", businessDay);
            
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

        public ActionResult DoDeposit(Deposit deposit,int SaleID)
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
            
            
            if (deposit.DepositReference.Trim() == "" || deposit.DepositReference==null)
            {
                X.Msg.Alert("Deposit Reference",
                        "Sorry, The Deposit reference is null. Please Check the business day or contact your administrator").Show();
                return this.Direct();
            }
            
            _depositRepository.SaleDepositForNonAssure(deposit,SaleID, SessionGlobalPersonID,0);
            
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
            Session["Receipt_CustomerID"] = deposit.Representant;
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

            this.GetCmp<NumberField>("Amount").Reset();
            this.GetCmp<Button>("btnPrint").Disabled = false;
            this.GetCmp<Button>("btnSave").Disabled = true;

            this.GetCmp<ComboBox>("PaymentMethod").Reset();
            this.GetCmp<ComboBox>("PaymentMethodID").Reset();
            this.GetCmp<TextField>("DepositReference").Value = "";
            this.GetCmp<TextField>("DepositReference").Value = "";
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
                return this.Store(this.ModelReturnAbleSales(BranchID, SoldDate));
            }
            else
            {
                return this.Store(this.ModelReturnAbleSales(BranchID, SearchOption, SearchValue));
            }
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
        public ActionResult PrintDepositReceipt(string detail)
        {
            Session["detail"] = detail;
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
            List<object> modelsubRpt = new List<object>();
            ReportDocument rptH = new ReportDocument();
            try
            {
            int saleID = (Session["Receipt_SaleID"]==null)? 0 : (int)Session["Receipt_SaleID"];
            string customerID = (Session["Receipt_CustomerID"] == null) ? "" : (string)Session["Receipt_CustomerID"];
            double receiveAmountTot = (Session["ReceiveAmoung_Tot"] == null) ? 0 : (double)Session["ReceiveAmoung_Tot"];
            string detail = (Session["detail"] == null) ? "" : (string)Session["detail"];

            string repName = "";
            bool isValid = false;
            double totalAmount = 0d;
            double totalRemaining = 0d;

            
            string path = "";
            string defaultImgPath = Server.MapPath("~/Content/Images/App/deboy-logo.png");
            byte[] COMPANY_LOGO = System.IO.File.ReadAllBytes(defaultImgPath);

            

            Devise devise = new Devise();
            Branch curBranch = new Branch();

            string TitleDeposit = "";
            string RptTitle = "";

            
            curBranch = db.Branches.Find(db.UserBranches.Where(ub => ub.UserID == SessionGlobalPersonID).FirstOrDefault().BranchID);
            businessDay = _busDayRepo.GetOpenedBusinessDay(curBranch);
            devise = db.Devises.FirstOrDefault(d => d.DefaultDevise);

            Company cmpny = db.Companies.FirstOrDefault();
            if (saleID > 0)//depot pour une vente
            {
                
                double saleAmount = 0d;
                SaleE currentSale = db.Sales.Find(saleID);

                int i = 1;
                double TotalReceiveAmount = 0d;
                //recuperation des versements
                List<CustomerSlice> lstCustomerSlice = db.CustomerSlices.Where(sl => sl.SaleID == currentSale.SaleID).ToList();
                foreach (CustomerSlice cs in lstCustomerSlice)
                {
                    TotalReceiveAmount = TotalReceiveAmount+cs.SliceAmount;
                    modelsubRpt.Add(
                    new 
                    {
                        Reference = currentSale.SaleReceiptNumber,
                        DepositDate = cs.SliceDate,
                        Description = i.ToString() + " Payment(s)",
                        LineUnitPrice = cs.SliceAmount,
                        RptReceiptPaymentDetailID = 1
                    });
                    i = i + 1;
                }
                
                //recuperation des versements
                List<SaleLine> lstSaleLine = db.SaleLines.Where(sl => sl.SaleID == currentSale.SaleID).ToList();
                totalAmount = (lstSaleLine.Count>0) ? Util.ExtraPrices(lstSaleLine.Select(c => c.LineAmount).Sum(), currentSale.RateReduction, currentSale.RateDiscount, currentSale.Transport, currentSale.VatRate).TotalTTC : 0; //montant du verre
                totalRemaining = totalAmount - TotalReceiveAmount;

                foreach (SaleLine custsaleLine in lstSaleLine)
                {
                    string labelFrame = (custsaleLine.marque != null && custsaleLine.reference != null) ? Resources.Marque + " " + custsaleLine.marque + " " + Resources.Reference + " " + custsaleLine.reference : "";
                    model.Add(
                    new 
                    {
                        RptReceiptID=1,
                        ReceiveAmount = receiveAmountTot,
                        TotalAmount = totalRemaining, //montant restant de la facture
                        LineUnitPrice = Util.ExtraPrices(custsaleLine.LineAmount, currentSale.RateReduction, currentSale.RateDiscount, currentSale.Transport, currentSale.VatRate).TotalTTC, //montant du verre
                        LineQuantity = custsaleLine.LineQuantity,
                        CompanyName = cmpny.Name,
                        CompanyAdress = cmpny.Adress.AdressFullName,// "Head Quater : " + cmpny.Adress.Quarter.Town.Region.RegionLabel + " - " + cmpny.Adress.Quarter.Town.TownLabel,
                        CompanyTel = "Tel: " + cmpny.Adress.AdressPhoneNumber + " Cell:" + cmpny.Adress.AdressCellNumber,
                        BranchName = currentSale.Branch.BranchName,
                        BranchAdress = "B.P " + cmpny.Adress.AdressPOBox + ", " + cmpny.Adress.Quarter.Town.TownLabel + ", Email: " + cmpny.Adress.AdressEmail,//currentSale.Branch.Adress.Quarter.QuarterLabel + " - " + currentSale.Branch.Adress.Quarter.Town.TownLabel,
                        BranchTel = "Tel: " + currentSale.Branch.Adress.AdressPhoneNumber,
                        Reference = currentSale.SaleReceiptNumber,
                        CompanyCNI = "NO CONT : " + cmpny.CNI,
                        Operator = CurrentUser.Name + " " + CurrentUser.Description,
                        CustomerName = customerID,
                        ProductLabel = (labelFrame.Trim().Length > 0) ? Resources.frame + " " /*+ custsaleLine.ProductLabel + " "*/ + labelFrame : custsaleLine.ProductLabel,
                        SaleDate = currentSale.SaleDate,
                        Title = TitleDeposit,
                        DeviseLabel = currentSale.Devise.DeviseLabel,
                        RateTVA = currentSale.VatRate,
                        RateReduction = currentSale.RateReduction,
                        RateDiscount = currentSale.RateDiscount,
                        Transport = currentSale.Transport,
                        RptReceiptPaymentDetailID=1,
                        CompanyLogo = db.Files.FirstOrDefault(f => f.GlobalPersonID == cmpny.GlobalPersonID) != null ? db.Files.FirstOrDefault(f => f.GlobalPersonID == cmpny.GlobalPersonID).Content : COMPANY_LOGO
                    }
                );
                    //
                }

                if (detail == "oui")
                {
                    path = Server.MapPath("~/Reports/CashRegister/RptReceiptDepositDetail.rpt");
                    repName = "RptReceiptDepositDetail";
                }
                else
                {
                    path = Server.MapPath("~/Reports/CashRegister/RptReceiptDeposit.rpt");
                    repName = "RptReceiptDeposit";
                }
                isValid = true;
            }


            if (isValid)
            {
                rptH.Load(path);
                rptH.SetDataSource(model);
               
                rptH.OpenSubreport("PaymentDetail").SetDataSource(modelsubRpt);
                
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