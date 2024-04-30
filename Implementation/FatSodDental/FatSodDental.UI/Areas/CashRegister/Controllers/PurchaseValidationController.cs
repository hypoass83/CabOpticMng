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
using FatSodDental.UI.Filters;
using FastSod.Utilities.Util;
using System.Web.UI;
using FatSod.DataContext.Concrete;

namespace FatSodDental.UI.Areas.CashRegister.Controllers
{
    [Authorize(Order = 1)]
    [TakeBusinessDay(Order = 2)]
    public class PurchaseValidationController : BaseController
    {
        //Current Controller and current page
        private const string CONTROLLER_NAME = "CashRegister/PurchaseValidation";
        private const string VIEW_NAME = "PurchaseValidation";

        //*********************

        private ISupplierOrder _supplierOrderRepository;
        private ITillDay _tillDayRepository;
        private IPurchase _purchaseRepository;
        // GET: CashRegister/Validate
        
        public PurchaseValidationController(
            ITillDay tillDayRepository,
            ISupplierOrder supplierOrderRepository, 
            IPurchase purchaseRepository
            )
        {
            this._tillDayRepository = tillDayRepository;
            this._supplierOrderRepository = supplierOrderRepository;
            this._purchaseRepository = purchaseRepository;
            
        }

        [OutputCache(Duration = 3600)] 
        public ActionResult PurchaseValidation()
        {
            //We test if this user has an autorization to get this sub menu
            //if (!LoadAction.isAutoriseToGetMenu(SessionProfileID, CodeValue.CashRegister.MENU_VALIDATE_SALE, db))
            //{
            //    RedirectToAction("NotAuthorized", "User");
            //}
            //Ext.Net.MVC.PartialViewResult rPVResult = new Ext.Net.MVC.PartialViewResult
            //{
            //    ClearContainer = true,
            //    //RenderMode = RenderMode.AddTo,
            //    ContainerId = "Body",
            //    //WrapByScriptTag = false,
            //    Model = ModelPurchase()
            //};
            //we ensure that if this user manage cash register. If he manage it, will verify if till is closed else, we ask he to closed it before login off
            UserTill userTill = db.UserTills.FirstOrDefault(td => td.UserID == SessionGlobalPersonID);
            if (userTill == null)
            {
                X.Msg.Alert("Access Denied", "You can't do sale operation. If you think no, try again or<br/>contact our administrator for this purpose<code/>.").Show();
                return this.Direct();
            }
            List<BusinessDay> UserBusDays = (List<BusinessDay>)Session["UserBusDays"];

            ViewBag.CurrentBranch = UserBusDays.FirstOrDefault().BranchID;// currentBranch.BranchID;
            BusinessDay businessDay = UserBusDays.FirstOrDefault();
            ViewBag.BusnessDayDate = businessDay.BDDateOperation;// businessDay.BDDateOperation;
            TillDay currentTillDay = db.TillDays.FirstOrDefault(t => t.TillID == userTill.TillID && t.TillDayDate.Day == businessDay.BDDateOperation.Day && t.TillDayDate.Month == businessDay.BDDateOperation.Month && t.TillDayDate.Year == businessDay.BDDateOperation.Year && t.IsOpen);
            if (currentTillDay == null)
            {
                X.Msg.Alert("Warnnig", "Cash register is closed. You must open it before do any sale<br/>Go at Cash Register module=>Open cash register<code/>").Show();
                return this.Direct();
            }
            //}
            ViewBag.CurrentTill = currentTillDay.TillID;
            //Session["Curent_Page"] = VIEW_NAME;
            //Session["Curent_Controller"] = CONTROLLER_NAME;
            Session["CommandOderLines"] = new List<SupplierOrderLine>();
            //this.GetCmp<Panel>("Body").LoadContent(new ComponentLoader
            //{
            //    Url = Url.Action(VIEW_NAME),
            //    DisableCaching = false,
            //    Mode = LoadMode.Frame
            //});
            
            return View(ModelPurchase());
        }
        public List<object> ModelPurchase()
        {
            List<object> model = new List<object>();
            LoadComponent.AllSupplierOrders.ForEach(c =>
            {
                c.SupplierOrderTotalAmount = c.SupplierOrderLines.Select(pl2 => pl2.LineAmount).Sum();
                model.Add(
                        new
                        {
                            CustomerOrderID = c.SupplierOrderID,
                            CustomerOrderDate = c.SupplierOrderDate,
                            CustomerOrderTotalPrice = c.SupplierOrderTotalAmount,
                            CustomerOrderNumber = c.SupplierOrderReference,
                            CustomerName = c.SupplierFullName
                        }
                    );
            });
            return model;
        }
        [HttpPost]
        public StoreResult GetAllCommand()
        {
            /*List<object> model = new List<object>();
            LoadComponent.AllSupplierOrders.ForEach(c =>
            {
                c.SupplierOrderTotalAmount = c.SupplierOrderLines.Select(pl2 => pl2.LineAmount).Sum();
                model.Add(
                        new
                        {
                            CustomerOrderID = c.SupplierOrderID,
                            CustomerOrderDate = c.SupplierOrderDate,
                            CustomerOrderTotalPrice = c.SupplierOrderTotalAmount,
                            CustomerOrderNumber = c.SupplierOrderReference,
                            CustomerName = c.SupplierFullName
                        }
                    );
            });*/
            return this.Store(ModelPurchase());
        }
        [HttpPost]
        public StoreResult CommandOderLines(int? CustomerOrderID)
        {

            List<SupplierOrderLine> dataTmp = (List<SupplierOrderLine>)Session["CommandOderLines"];
            
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
                            LineUnitPrice = c.LineUnitPrice,
                            Location = c.LocalizationLabel
                        }
                    );
            });
            return this.Store(model);
        }
        public ActionResult InitializeCommandFields(int ID)
        {
            try
            {
                this.Reset1();

                if (ID > 0)
                {
                    SupplierOrder customerOrder = db.SupplierOrders.Find(ID);
                    List<SupplierOrderLine> customerLineOderLines = db.SupplierOrderLines.Where(co => co.SupplierOrderID == ID).ToList();
                    Session["CommandOderLines"] = customerLineOderLines;
                    this.GetCmp<Store>("CommandOderLines").Reload();
                    //extra price
                    double totalPriceHT = customerLineOderLines.Select(col => col.LineAmount).Sum();
                    //we take a extra price
                    ExtraPrice extra = Util.ExtraPrices(totalPriceHT, customerOrder.RateReduction, customerOrder.RateDiscount, customerOrder.Transport, customerOrder.VatRate);
                    this.GetCmp<NumberField>("InitialHT").Value = totalPriceHT;

                    this.GetCmp<GridPanel>("CommandLinesGrid").Disabled = false;
                    this.GetCmp<Panel>("SliceAmountForm").Disabled = false;
                    this.GetCmp<FormPanel>("CommandTotalAmount").Disabled = false;
                    this.GetCmp<TextField>("CustomerOrderID").Value = customerOrder.SupplierOrderID;
                    this.GetCmp<TextField>("BranchID").Value = customerOrder.BranchID;
                    this.GetCmp<TextField>("CustomerID").Value = customerOrder.SupplierID;
                    this.GetCmp<TextField>("SaleReceiptNumber").Value = customerOrder.SupplierOrderReference;
                    this.GetCmp<TextField>("CustomerName").Value = customerOrder.SupplierFullName;
                    this.GetCmp<DateField>("CommandDate").Value = customerOrder.SupplierOrderDate;
                    this.GetCmp<ComboBox>("DeviseID").Value = customerOrder.DeviseID;

                    //extra price
                    this.GetCmp<NumberField>("Discount").Value = customerOrder.RateDiscount;
                    this.GetCmp<NumberField>("Reduction").Value = customerOrder.RateReduction;
                    this.GetCmp<NumberField>("Transport").Value = customerOrder.Transport;

                    this.GetCmp<NumberField>("ReductionAmount").Value = extra.ReductionAmount;
                    this.GetCmp<NumberField>("DiscountAmount").Value = extra.DiscountAmount;
                    this.GetCmp<NumberField>("TotalPriceHT").Value = extra.TotalHT;
                    this.GetCmp<NumberField>("TVAAmount").Value = extra.TVAAmount; 
                    this.GetCmp<NumberField>("TotalPriceTTC").Value = extra.TotalTTC;
                }

                return this.Direct();
            }
            catch (Exception e) { X.Msg.Alert("Error ", e.Message).Show(); return this.Direct(); }
        }
        //this method return a different payment method of one type
        public StoreResult PaymentMethods(string BuyTypeCode)
        {
                return this.Store(LoadComponent.SpecificBankPaymentMethod(BuyTypeCode));
        }

        [HttpPost]
        public ActionResult Reset()
        {
            this.Reset1();

            return this.Direct();
        }

        public void Reset1()
        {
            this.GetCmp<FormPanel>("Unknow").Reset(true);
            this.GetCmp<FormPanel>("GlobalSaleForm").Reset(true);
            this.GetCmp<FormPanel>("FormCustomerIdentification").Reset(true);
            this.GetCmp<FormPanel>("SaleLineForm").Reset(true);
            this.GetCmp<FormPanel>("CommandTotalAmount").Reset(true);
            this.GetCmp<Store>("CommadListStore").Reload();
            this.GetCmp<Store>("CommandOderLines").Reload();

        }

        //This method confirm a sale
        [HttpPost]
        public ActionResult ValidatePurchase(SaleE currentSale, CustomerSlice customerSlice, int PurchaseBringerID, int CustomerOrderID, int BuyField, string BuyType, double TVA = 0)
        {

            try
            {
                //Sorcellerie provisoire : transformation de sale en purchase
                SupplierOrder supplierOrder = db.SupplierOrders.Find(CustomerOrderID);

                Purchase purchase = new Purchase
                {
                    PaymentMethodID = currentSale.PaymentMethodID,
                    PurchaseBringerID = PurchaseBringerID,
                    PurchaseDate = currentSale.SaleDate.Date,
                    PurchaseReference = currentSale.SaleReceiptNumber,
                    PurchaseRegisterID = SessionGlobalPersonID,
                    SupplierID = currentSale.CustomerID.Value,
                    VatRate = currentSale.VatRate,
                    Guaranteed = currentSale.Guaranteed,
                    RateReduction = currentSale.RateReduction,
                    RateDiscount = currentSale.RateDiscount,
                    Transport = currentSale.Transport,
                    PaymentDelay = BuyField,
                    BranchID = supplierOrder.BranchID,
                    DeviseID = customerSlice.DeviseID,
                    PaymentMethod = BuyType,
                    TotalPriceTTC=currentSale.TotalPriceTTC,
                    StatutPurchase = currentSale.StatutSale,
                    PurchaseValidate = currentSale.SaleValidate,
                    PurchaseDeliveryDate = currentSale.SaleDeliveryDate,
                    OldStatutPurchase = currentSale.OldStatutSale
                };

                //choix de la caisse
                if (purchase.PaymentMethod == CodeValue.Accounting.DefaultCodeAccountingSection.CODECAIS)
                {
                    UserTill userTill = db.UserTills.SingleOrDefault(td => td.UserID == SessionGlobalPersonID && td.HasAccess);

                    //purchase.TotalPriceHT = PurchaseLines.Select(pl2 => pl2.LineAmount).Sum();
                    double tillBalance = _tillDayRepository.TillStatus(userTill.Till).Ballance;

                    //ne faites ps d'achat en espèce si : 1 - pas d'argent en caisse; 2- Facture > Montant en caisse
                    if (tillBalance <= 0 || purchase.TotalPriceTTC > tillBalance)
                    {

                        X.Msg.Alert("NO Cash Availlable",
                            "Sorry, you can not proceed this cash opérations because you do have sufficient liquidities in your till. Please contact an administrator").Show();
                        return this.Direct();
                    }

                    purchase.PaymentMethodID = userTill.TillID;
                }

                List<PurchaseLine> PurchaseLines = new List<PurchaseLine>();
                foreach (SupplierOrderLine sol in supplierOrder.SupplierOrderLines)
                {
                    PurchaseLines.Add(

                            new PurchaseLine
                {
                                LineQuantity = sol.LineQuantity,
                                LineUnitPrice = sol.LineUnitPrice,
                                LocalizationID = sol.LocalizationID,
                                ProductID = sol.ProductID,
                }
                        );
                }

                purchase.PurchaseLines = PurchaseLines;

                SupplierSlice supplierSlice = new SupplierSlice()
                {
                    DeviseID = purchase.DeviseID,
                    PaymentMethodID = purchase.PaymentMethodID,
                    SliceAmount = customerSlice.SliceAmount,
                    SliceDate = purchase.PurchaseDate,
                };

                purchase.CurrentSupplierSlice = supplierSlice;

                _purchaseRepository.CreatePurchase(purchase);
                //mise à jour de la commande
                supplierOrder.IsDelivered = true;
                _supplierOrderRepository.Update(supplierOrder, supplierOrder.SupplierOrderID);

                Session["CommandOderLines"] = new List<SupplierOrderLine>();
                return this.Reset();
                
            }
            catch (Exception e) { X.Msg.Alert("Error ", e.Message + " " + e.StackTrace).Show(); return this.Direct(); }
        }

    }
}