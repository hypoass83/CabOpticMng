using FastSod.Utilities.Util;
using FatSod.DataContext.Initializer;
using FatSod.Ressources;
using FatSod.Security.Abstracts;
using FatSod.Security.Entities;
using FatSod.Supply.Abstracts;
using FatSod.Supply.Entities;
using CABOPMANAGEMENT.Controllers;
using CABOPMANAGEMENT.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using SaleE = FatSod.Supply.Entities.Sale;
using CABOPMANAGEMENT.Tools;

namespace CABOPMANAGEMENT.Areas.CashRegister.Controllers
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
        private ITillDay _tillDayRepository;

        private ITransactNumber _transactNumbeRepository;

        private BusinessDay businessDay;
        
        public DepositController(
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
        public ActionResult Deposit()
        {
            ViewBag.DisplayForm = 1;
            try
            {
                
            //we ensure that if this user manage cash register. If he manage it, will verify if till is closed else, we ask he to closed it before login off
           
                UserTill userTill = (from td in db.UserTills
                                     where td.UserID == SessionGlobalPersonID
                                     select td).SingleOrDefault();

                if (userTill == null)
                {
                        TempData["Message"] = "Access Denied - You can't do sale operation. If you think no, try again or<br/>contact our administrator for this purpose<code/>.";
                        ViewBag.DisplayForm = 0;
                        return this.View();
                }
                List<BusinessDay> UserBusDays = (List<BusinessDay>)Session["UserBusDays"];
                Session["businessDay"] = UserBusDays.FirstOrDefault();
                DateTime currentDateOp = UserBusDays.FirstOrDefault().BDDateOperation;
                ViewBag.CurrentBranch = UserBusDays.FirstOrDefault().BranchID;
                ViewBag.BusnessDayDate = UserBusDays.FirstOrDefault().BDDateOperation.ToString("yyyy-MM-dd");// businessDay.BDDateOperation;
                Session["BusnessDayDate"] =currentDateOp;

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

                return View(PendingCustomerSale());
            }

            catch (Exception e)
            {
                TempData["Message"] = "Error " + e.Message;
                ViewBag.DisplayForm = 0;
                return this.View();
            }
        }

        
        //retourne les ventes non delivre
        private List<SaleE> ModelReturnAbleSales()
        {

            //double Advanced = 0d;
            //double Remainder = 0d;
            //double SaleTotalPrice = 0d;
            List<SaleE> model = new List<SaleE>();

            //var lstallSales = from bc in db.vcumulRealSales.Where(s=>s.SaleTotalPrice>s.Advanced).ToList();
            var sales = db.vcumulRealSales.Where(s => s.SaleTotalPrice > s.Advanced)/*.OrderBy(o => o.SaleID)*/.ToList();

            foreach (var s in sales)
            {
                //dynamic saleInfo = GetSaleInfo(s.SaleID);
                    model.Add(
                            new SaleE
                            {
                                SaleID = s.SaleID,
                                SaleDate = s.SaleDate,
                                SaleDeliveryDate = (s.SaleDeliveryDate.HasValue) ? s.SaleDeliveryDate.Value : s.SaleDate,
                                CustomerFullName = s.Name,
                                SaleReceiptNumber = s.SaleReceiptNumber,
                                SaleTotalPrice = s.SaleTotalPrice,
                                Advanced = s.Advanced,
                                Remainder = s.SaleTotalPrice - s.Advanced,
                                Remarque = s.Remarque,
                                MedecinTraitant = s.MedecinTraitant,
                                //PhoneNumber = saleInfo.PhoneNumber,
                                //PreferredLanguage = saleInfo.PreferredLanguage
                            }
                          );
                
            }

            /* var lstallSales = from bc in db.viewRealSales
                               group bc by new { bc.SaleID, bc.SaleDate, bc.CustomerID, bc.Name, bc.SaleReceiptNumber, bc.SaleDeliveryDate, bc.Remarque, bc.MedecinTraitant } into g
                               select new
                               {
                                   key = g.Key,
                                   SaleTotalPrice = g.Sum(a => (a.LineUnitPrice * a.SaleQty))
                               };

             foreach (var s in lstallSales.ToList())
             {
                 List<CustomerSlice> lstCustSlice = db.CustomerSlices.Where(sl => sl.SaleID == s.key.SaleID).ToList();
                 if (lstCustSlice != null && lstCustSlice.Count > 0)
                 {
                     Advanced = lstCustSlice.Select(sl => sl.SliceAmount).Sum();
                 }
                 else
                 {
                     Advanced = 0;
                 }
                 Remainder = s.SaleTotalPrice - Advanced;
                 if (Remainder > 0)
                 {
                     model.Add(
                             new SaleE
                             {
                                 SaleID = s.key.SaleID,
                                 SaleDate = s.key.SaleDate,
                                 SaleDeliveryDate = (s.key.SaleDeliveryDate.HasValue) ? s.key.SaleDeliveryDate.Value : s.key.SaleDate,
                                 CustomerFullName = s.key.Name,
                                 SaleReceiptNumber = s.key.SaleReceiptNumber,
                                 SaleTotalPrice = s.SaleTotalPrice,
                                 Remarque = s.key.Remarque,
                                 MedecinTraitant = s.key.MedecinTraitant
                             }
                           );
                 }
             }*/

            //var allSales = db.Sales.Where(sa => (!sa.IsPaid) && (sa.isReturn == false))
            //    .Select(s => new
            //    {
            //        SaleID = s.SaleID,
            //        SaleLines = s.SaleLines,
            //        RateReduction = s.RateReduction,
            //        Transport = s.Transport,
            //        VatRate = s.VatRate,
            //        SaleDate = s.SaleDate,
            //        SaleDeliveryDate = s.SaleDeliveryDate,
            //        CustomerName = s.CustomerName,
            //        SaleReceiptNumber = s.SaleReceiptNumber,
            //        Remarque = s.Remarque,
            //        MedecinTraitant = s.MedecinTraitant,
            //        RateDiscount=s.RateDiscount
            //    }).ToList().OrderBy(c => c.SaleReceiptNumber);

            ////678528832 -mama pepe
            ////il faut mainteant vérifier si la vente à encore au moins une ligne de vente pouvant faire l'objet d'un retour
            //foreach (var s in allSales)
            //{
            //    if (s.SaleLines == null || s.SaleLines.Count <= 0) { continue; }
            //    SaleTotalPrice = Util.ExtraPrices(s.SaleLines.Select(sl => sl.LineAmount).Sum(), s.RateReduction, s.RateDiscount, s.Transport, s.VatRate).TotalTTC;

            //    model.Add(
            //                new SaleE
            //                {
            //                    SaleID = s.SaleID,
            //                    SaleDate = s.SaleDate,
            //                    SaleDeliveryDate = (s.SaleDeliveryDate.HasValue) ? s.SaleDeliveryDate.Value : s.SaleDate,
            //                    CustomerFullName = s.CustomerName,
            //                    SaleReceiptNumber = s.SaleReceiptNumber,
            //                    SaleTotalPrice = SaleTotalPrice,
            //                    //Advanced=Advanced,
            //                    //Remainder=Remainder,
            //                    Remarque = s.Remarque,
            //                    MedecinTraitant = s.MedecinTraitant
            //                }
            //              );
            //}


            return model;
        }

        private dynamic GetSaleInfo(int saleID)
        {
            var sale = db.Sales.Find(saleID);

            return new
            {
                PhoneNumber = FormatPhoneNumber(sale.AdressPhoneNumber),
                PreferredLanguage = sale.Customer.PreferredLanguage != null ? sale.Customer.PreferredLanguage : "FR"
            };
        }

        public string FormatPhoneNumber(string phoneNumber)
        {

            if (String.IsNullOrEmpty(phoneNumber))
                return "BAD-" + phoneNumber;

            var res = phoneNumber;
            var resInt = 0;

            res = res.Trim();

            if(res.Contains("-"))
                res = res.Remove('-');

            if (res.Contains("*"))
                res = res.Remove('*');

            if (res.Length < 9 || !int.TryParse(res, out resInt) || res[0] != '6')
                return "BAD-" + res;

            return res;
        }

        public JsonResult InitializeFields(int SaleID)
        {
            List<object> list = new List<object>();

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
            
            Branch curBranch = db.Branches.Find(sale.BranchID);
            businessDay = _busDayRepo.GetOpenedBusinessDay(curBranch);

            string DepositReference = _transactNumbeRepository.displayTransactNumber("REDP", businessDay);

            list.Add(
                new {
                    DepositReference = DepositReference,
                    Debt= Remainder,
                    CustomerName= sale.CustomerName,
                    BranchID= sale.BranchID,
                    SaleID= sale.SaleID,
                    CustomerID=sale.CustomerID
                });
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult OpenedBusday()
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

            return Json(list, JsonRequestBehavior.AllowGet);

        }

        public JsonResult ChangeBusDay(int? BranchID)
        {
            List<object> list = new List<object>();
            if (BranchID != null && BranchID.HasValue && BranchID.Value > 0)
            {
                lstBusDay = (List<BusinessDay>)Session["UserBusDays"];
                if (lstBusDay == null)
                {
                    lstBusDay = _busDayRepo.GetOpenedBusinessDay(CurrentUser);
                };

                BusinessDay businessD = lstBusDay.FirstOrDefault(bd => bd.BranchID == BranchID && bd.BDStatut);
                list.Add(new { DepositDate= businessD.BDDateOperation.ToString("yyyy-MM-dd") });
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DoDeposit(Deposit deposit,int SaleID,int SaleDeliver)
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

                Session["ReceiveAmoung_Tot"] = deposit.Amount;

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

                if (deposit.DepositReference.Trim() == "" || deposit.DepositReference==null)
                {
                        Message = "Deposit Reference - Sorry, The Deposit reference is null. Please Check the business day or contact your administrator " ;
                        status = false;
                        return new JsonResult { Data = new { status = status, Message = Message } };
                }

                if (deposit.PaymentMethod == CodeValue.Accounting.DefaultCodeAccountingSection.DIGITAL_PAYMENT)
                {
                    var isPrimaryKeyViolated = IsTransactionIdentifierNew(deposit.TransactionIdentifier, deposit.PaymentMethodID);

                    if (isPrimaryKeyViolated)
                        throw new Exception("The Given Transaction Code (" + deposit.TransactionIdentifier + " has already been used");
                }

                _depositRepository.SaleDepositForNonAssure(deposit,SaleID, SessionGlobalPersonID, SaleDeliver);
            
                SetStateParameters(deposit);
                Message = Resources.Success+ " - "+ Resources.DepositSuccess;
                status = true;
                return new JsonResult { Data = new { status = status, Message = Message } };
            }
            catch (Exception e) 
            {
                Message = "Error "+ e.Message + " " + e.StackTrace + " " + e.InnerException;
                status = false;
                return new JsonResult { Data = new { status = status, Message = Message } };
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

        public JsonResult PendingCustomerSale()
        {
            var sales = this.ModelReturnAbleSales();
            var model = new
            {
                data = from sale in sales
                       select new
                       {
                           SaleID = sale.SaleID,
                           SaleDate = sale.SaleDate.ToString("yyyy-MM-dd"),
                           SaleDeliveryDate = sale.SaleDeliveryDate.Value.ToString("yyyy-MM-dd"),
                           CustomerFullName = sale.CustomerFullName,
                           SaleReceiptNumber = sale.SaleReceiptNumber,
                           SaleTotalPrice = sale.SaleTotalPrice,
                           Advanced = sale.Advanced,
                           Remainder = sale.Remainder,
                           Remarque = sale.Remarque,
                           MedecinTraitant = sale.MedecinTraitant
                       }
            };
            //return Json(model, JsonRequestBehavior.AllowGet);
            var jsonResult = Json(model, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;

        }

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


        //This method print a receipt of customer
        public void GenerateReceiptDeposit()
        {
            List<object> model = new List<object>();
            List<object> modelsubRpt = new List<object>();
            //ReportDocument rptH = new ReportDocument();
            //try
            //{
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


            /*if (isValid)
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
            }*/
        
        /*}
            finally
            {
                rptH.Close();
                rptH.Dispose();
            }*/
    }
    }
}