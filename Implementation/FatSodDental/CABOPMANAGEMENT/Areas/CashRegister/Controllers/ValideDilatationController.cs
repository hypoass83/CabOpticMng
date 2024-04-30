using FatSod.DataContext.Initializer;
using FatSod.Security.Abstracts;
using FatSod.Security.Entities;
using FatSod.Supply.Entities;
using CABOPMANAGEMENT.Controllers;
using CABOPMANAGEMENT.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

using SaleE = FatSod.Supply.Entities.Sale;
using FatSod.Supply.Abstracts;
using FatSod.Ressources;
using CABOPMANAGEMENT.Filters;
using FastSod.Utilities.Util;

namespace CABOPMANAGEMENT.Areas.CashRegister.Controllers
{
    [Authorize(Order = 1)]
    [TakeBusinessDay(Order = 2)]
    public class ValideDilatationController : BaseController
    {
        //Current Controller and current page
        private const string CONTROLLER_NAME = "CashRegister/ValideDilatation";


        //*********************


        private ISale _saleRepository;
        private IBusinessDay _busDayRepo;
        private ISavingAccount _savingAccountRepository;
        // GET: CashRegister/Validate

        private IDeposit _depositRepository;
        private ITillDay _tillDayRepository;

        public ValideDilatationController(
            ISale saleRepository,
            IBusinessDay busDayRepo,
            IDeposit depositRepository,
            ITillDay tillDayRepository,
            ISavingAccount saRepo
            )
        {
            this._saleRepository = saleRepository;
            this._busDayRepo = busDayRepo;
            this._savingAccountRepository = saRepo;
            this._depositRepository = depositRepository;
            this._tillDayRepository = tillDayRepository;
        }


        // GET: CashRegister/ValideDilatation
        [OutputCache(Duration = 3600)]
        public ActionResult Index()
        {
            ViewBag.DisplayForm = 1;
            try
            {

                UserTill userTill = (from td in db.UserTills
                                     where td.UserID == SessionGlobalPersonID
                                     select td).SingleOrDefault();
                if (userTill == null || userTill.TillID <= 0)
                {
                    TempData["Message"] = "Access Denied - You can't do sale operation. If you think no, try again or<br/>contact our administrator for this purpose<code/>.";
                    ViewBag.DisplayForm = 0;
                    return this.View();
                }
                List<BusinessDay> UserBusDays = (List<BusinessDay>)Session["UserBusDays"];
                DateTime currentDateOp = UserBusDays.FirstOrDefault().BDDateOperation;
                ViewBag.CurrentBranch = UserBusDays.FirstOrDefault().BranchID;
                ViewBag.BusnessDayDate = currentDateOp.ToString("yyyy-MM-dd");

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


                Session["salelines"] = new List<SaleLine>();
                Session["isDeliverOrder"] = false;
                return View(ModelSaleValidate());
            }

            catch (Exception e)
            {
                TempData["Message"] = "Error " + e.Message + e.InnerException;
                ViewBag.DisplayForm = 0;
                return this.View();
            }


        }


        public JsonResult ModelSaleValidate()
        {

            var model = new
            {
                data = from c in db.AuthoriseSales.Where(c => !c.IsDelivered && c.IsDilatation && c.ConsultDilPrescID.HasValue).ToList()
                       select
                       new
                       {
                           AuthoriseSaleID = c.AuthoriseSaleID,
                           SaleDate = c.SaleDate.ToString("yyyy-MM-dd"),
                           TotalPriceHT = Util.ExtraPrices(c.AuthoriseSaleLines.Select(sl => sl.LineAmount).Sum(),
                                            c.RateReduction, c.RateDiscount, c.Transport, c.VatRate).TotalTTC,
                           SaleReceiptNumber = c.SaleReceiptNumber,
                           CustomerName = c.Customer.CustomerFullName,
                           ConsultDilPrescID=c.ConsultDilPrescID.Value
                       }
            };
            return Json(model, JsonRequestBehavior.AllowGet);


        }

        //[HttpPost]
        public JsonResult CommandOderLines()
        {

            try
            {
                List<SaleLine> dataTmp = (List<SaleLine>)Session["salelines"];
                var model = new
                {
                    data = from c in dataTmp
                           select new
                           {
                               LineID = c.LineID,
                               LineAmount = c.LineAmount,
                               LineQuantity = c.LineQuantity,
                               ProductLabel = c.ProductLabel,
                               LineUnitPrice = c.LineUnitPrice
                           }
                };

                return Json(model, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                List<object> list = new List<object>();
                // TempData["Message"] = "Error " + e.Message;
                return Json(list, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult InitializeCommandFields(int ID)
        {
            List<object> _CommandList = new List<object>();
            try
            {

                AuthoriseSale customerOrder = (from c in db.AuthoriseSales
                                               where c.AuthoriseSaleID == ID && !c.IsDelivered
                                               select c).SingleOrDefault();
                if (customerOrder == null)
                {
                    TempData["Message"] = "Warning - This sale is already validate";
                    return Json(_CommandList, JsonRequestBehavior.AllowGet);
                }



                _CommandList.Add(new
                {
                    BuyType = "Credit",
                    AuthoriseSaleID = customerOrder.AuthoriseSaleID,
                    BranchID = customerOrder.BranchID,
                    ConsultDilPrescID = (customerOrder.ConsultDilPrescID.HasValue) ? customerOrder.ConsultDilPrescID.Value:0,
                    CustomerID = customerOrder.CustomerID,
                    DeviseID = customerOrder.DeviseID,
                    CustomerName = (customerOrder.Customer==null) ? customerOrder.CustomerName : customerOrder.Customer.CustomerFullName,
                    SaleReceiptNumber = customerOrder.SaleReceiptNumber,
                    SaleDate = customerOrder.SaleDate.ToString("yyyy-MM-dd"),
                    SaleDeliveryDate = SessionBusinessDay(customerOrder.BranchID).BDDateOperation.ToString("yyyy-MM-dd"),
                    Representant = (customerOrder.Customer == null) ? customerOrder.CustomerName : customerOrder.Customer.CustomerFullName,
                    PostByID = customerOrder.PostByID,
                    SellerID = customerOrder.SellerID
                });
                return Json(_CommandList, JsonRequestBehavior.AllowGet);


            }
            catch (Exception e)
            {
                TempData["Message"] = "Error " + e.Message;
                return Json(_CommandList, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult ApplyExtraPrices(int ID)
        {
            List<object> _CommandList = new List<object>();
            try
            {
                List<SaleLine> authosalelines = (List<SaleLine>)Session["salelines"];

                AuthoriseSale customerOrder = (from c in db.AuthoriseSales
                                               where c.AuthoriseSaleID == ID && !c.IsDelivered
                                               select c).SingleOrDefault();
                if (customerOrder == null)
                {
                    TempData["Message"] = "Warning - This sale is already validate";
                    return Json(_CommandList, JsonRequestBehavior.AllowGet);
                }

                List<AuthoriseSaleLine> customerLineOderLines = db.AuthoriseSaleLines.Where(co => co.AuthoriseSaleID == ID).ToList();
                foreach (AuthoriseSaleLine authsal in customerLineOderLines)
                {
                    Product existProd = db.Products.Find(authsal.ProductID);

                    SaleLine saleline = new SaleLine()
                    {
                        isGift = false,
                        LineQuantity = authsal.LineQuantity,
                        LineUnitPrice = authsal.LineUnitPrice,
                        LocalizationID = authsal.LocalizationID,
                        //ProductCategoryID=authsal.ProductCategoryID,
                        ProductID = authsal.ProductID,
                        //reference=authsal.reference
                        Product = existProd,
                        //ProductLabel= existProd.
                    };
                    authosalelines.Add(saleline);
                }
                Session["salelines"] = authosalelines;
                double valueOperation = customerLineOderLines.Select(l => l.LineAmount).Sum();
                //we add extra price
                double new_HT_price = valueOperation;

                double NetCom = valueOperation;

                ExtraPrice extra = Util.ExtraPrices(valueOperation, customerOrder.RateReduction, customerOrder.RateDiscount, customerOrder.Transport, customerOrder.VatRate);

                _CommandList.Add(new
                {
                    InitialHT = valueOperation,
                    DiscountAmount = extra.DiscountAmount,
                    NetCom = extra.NetCom,
                    ReductionAmount = extra.ReductionAmount,
                    TotalPriceHT = extra.NetFinan,
                    TVAAmount = extra.TVAAmount,
                    TotalPriceTTC = extra.TotalTTC,
                    InitialTTC = extra.TotalTTC,
                    Reduction = customerOrder.RateReduction,
                    Discount = customerOrder.RateDiscount,
                    Transport = customerOrder.Transport,
                    VatRate = customerOrder.VatRate,
                    SliceAmount = extra.TotalTTC,
                    RemaingAmount = 0
                });

                return Json(_CommandList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                TempData["Message"] = "Error " + e.Message;
                return Json(_CommandList, JsonRequestBehavior.AllowGet);
            }
        }

        //chargement des combo box
        public JsonResult populateBuyType()
        {
            var paymentMethodTypes = Utilities.PaymentMethodTypes();

            return Json(paymentMethodTypes, JsonRequestBehavior.AllowGet);
        }

        public JsonResult PaymentMethods(string BuyTypeCode)
        {
            List<object> model = new List<object>();
            db.PaymentMethods.OfType<Bank>().Where(p => p.Account.CollectifAccount.AccountingSection.AccountingSectionCode == BuyTypeCode).ToList().ForEach(p =>
            {
                model.Add(
                        new
                        {
                            ID = p.ID,
                            Name = p.Name
                        }
                    );
            });
            return Json(model, JsonRequestBehavior.AllowGet);
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


        //This method confirm a sale
        //[HttpPost]
        public JsonResult ValidateSale(SaleE currentSale, string RateReduction, string RateDiscount, CustomerSlice customerSlice, int PaymentDelay, string BuyType, string heureVente)
        {
            bool status = false;
            string Message = "";
            try
            {
                Session["Receipt_SaleID"] = null;
                Session["Receipt_CustomerID"] = null;
                Session["ReceiveAmoung_Tot"] = null;

                currentSale.isDilation = true;

                currentSale.RateReduction = LoadComponent.ConvertToDouble(RateReduction);
                currentSale.RateDiscount = LoadComponent.ConvertToDouble(RateDiscount);
                //verif si limit montant atteint
                double ValueOperation = currentSale.TotalPriceTTC;

                currentSale.SaleLines = (List<SaleLine>)Session["salelines"];

                currentSale.CustomerSlice = customerSlice;
                customerSlice.SliceDate = currentSale.SaleDate;
                currentSale.PaymentDelay = PaymentDelay;
                currentSale.IsSpecialOrder = false; //vente principale ---- true; //vente autre produit
                if (currentSale != null && currentSale.CustomerSlice.SliceAmount > 0 && BuyType == CodeValue.Accounting.DefaultCodeAccountingSection.Credit)
                {
                    Message = "Wrong Payment Mode Select!!!" + Resources.MsgWrongChoixPayementMethod;
                    status = false;
                    return new JsonResult { Data = new { status = status, Message = Message } };
                }

                var isAnyValidPMTypeSelected = (BuyType == CodeValue.Accounting.DefaultCodeAccountingSection.CODEBANK ||
                                                 BuyType == CodeValue.Accounting.DefaultCodeAccountingSection.CODECAIS ||
                                                 BuyType == CodeValue.Accounting.DefaultCodeAccountingSection.DIGITAL_PAYMENT);

                if (currentSale != null && currentSale.CustomerSlice.SliceAmount == 0 && isAnyValidPMTypeSelected)
                {
                    Message = "Wrong Payment Mode Select!!! " + Resources.MsgWrongChoixPayementMethod;
                    status = false;
                    return new JsonResult { Data = new { status = status, Message = Message } };
                }

                if (BuyType == CodeValue.Accounting.DefaultCodeAccountingSection.CODECAIS)
                {
                    Session["PaymentMethod"] = "CASH";
                }

                if (BuyType == CodeValue.Accounting.DefaultCodeAccountingSection.CODEBANK)
                {
                    Session["PaymentMethod"] = "CHECK";
                }

                if (BuyType == CodeValue.Accounting.DefaultCodeAccountingSection.DIGITAL_PAYMENT)
                {
                    Session["PaymentMethod"] = CodeValue.Accounting.DefaultCodeAccountingSection.DIGITAL_PAYMENT;
                }

                if (BuyType == CodeValue.Supply.DepositReason.SavingAccount)
                {
                    Session["PaymentMethod"] = "SAVING ACCOUNT";
                }
                
                if (BuyType == CodeValue.Accounting.DefaultCodeAccountingSection.Credit)
                {
                    Session["PaymentMethod"] = "CREDIT";
                }

                //choix de la caisse
                if (BuyType == CodeValue.Accounting.DefaultCodeAccountingSection.CODECAIS)
                {
                    int userTill = db.UserTills.FirstOrDefault(td => td.UserID == SessionGlobalPersonID).TillID;
                    currentSale.PaymentMethodID = userTill;
                }

                //empecher que le cqissier saisisse un montant superieur au montant de la vente
                if (currentSale.TotalPriceTTC < currentSale.CustomerSlice.SliceAmount)
                {
                    Message = "Error!!! " + Resources.MsgErrTotalPriceMoreThanSlice;
                    status = false;
                    return new JsonResult { Data = new { status = status, Message = Message } };
                }

                //We will test if sale is in two steps here or not
                if (currentSale.TotalPriceTTC != currentSale.CustomerSlice.SliceAmount)
                {
                    currentSale.IsPaid = false;
                }
                else
                {
                    currentSale.IsPaid = true;
                }

                if (currentSale != null && (currentSale.PaymentMethodID <= 0) && (isAnyValidPMTypeSelected))
                {
                    status = false;
                    Message = "Wrong Payment Mode Select!!! " + Resources.MsgErrChoixPayementMethod;
                    return new JsonResult { Data = new { status = status, Message = Message } };
                }

                if (BuyType == CodeValue.Accounting.DefaultCodeAccountingSection.DIGITAL_PAYMENT)
                {
                    var isPrimaryKeyViolated = IsTransactionIdentifierNew(currentSale.PaymentReference, currentSale.PaymentMethodID);

                    if (isPrimaryKeyViolated)
                        throw new Exception("The Given Transaction Code (" + currentSale.PaymentReference + " has already been used");
                }

                int SaleID = _saleRepository.SaveChanges(currentSale, heureVente, SessionGlobalPersonID, true, true).SaleID;
                Session["Receipt_SaleID"] = SaleID;
                Session["Receipt_CustomerID"] = currentSale.CustomerName;
                Session["ReceiveAmoung_Tot"] = (currentSale.CustomerSlice != null) ? currentSale.CustomerSlice.SliceAmount : 0;

                SaleReset(customerSlice, SaleID);
                Message = Resources.Success + " - " + Resources.SaleNewSale;
                status = true;

            }
            catch (Exception e)
            {
                status = false;
                Message = "Error " + e.Message + " " + e.StackTrace + " " + e.InnerException;
            }
            return new JsonResult { Data = new { status = status, Message = Message } };
        }

        public void SaleReset(CustomerSlice customerSlice, int SaleID)
        {
            Session["salelines"] = new List<SaleLine>();

            Session["CurrentProduct"] = new Product();
            Session["MaxValue"] = 0;

            Session["SaleID"] = SaleID;
            Session["SliceAmount"] = customerSlice.SliceAmount;
            Session["isDeliverOrder"] = false;
        }
    }
}