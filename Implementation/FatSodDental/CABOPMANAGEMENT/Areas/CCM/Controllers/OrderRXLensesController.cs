using FastSod.Utilities.Util;
using FatSod.Ressources;
using FatSod.Security.Abstracts;
using FatSod.Security.Entities;
using FatSod.Supply.Abstracts;
using FatSod.Supply.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CABOPMANAGEMENT.Controllers;
using CABOPMANAGEMENT.Tools;

namespace CABOPMANAGEMENT.Areas.CCM.Controllers
{
    public class OrderRXLensesController : BaseController
    {

        private ISale _saleRepository;
        private IBusinessDay _busDayRepo;
        private IProductLocalization _prodLocalRepo;
        private StockType CurrentStockType;
        private LensConstruction lensFrameConstruction = new LensConstruction();

        public OrderRXLensesController(
            ISale saleRepository,
            IBusinessDay busDayRepo,
            IProductLocalization prodLocalRepo
        )
        {
            this._saleRepository = saleRepository;
            this._busDayRepo = busDayRepo;
            this._prodLocalRepo = prodLocalRepo;
        }

        // GET: CRM/OrderRXLenses
        public ActionResult Index(StockType StockType)
        {
            ViewBag.CurrentStockType = this.CurrentStockType = StockType;
            ViewBag.ViewName = "OrderRXLenses";
            List<BusinessDay> listBDUser = (List<BusinessDay>)Session["UserBusDays"];
            if (listBDUser == null)
            {
                listBDUser = _busDayRepo.GetOpenedBusinessDay(CurrentUser);
            }
            if (listBDUser.Count() > 1)
            {
                ViewBag.Disabled = false;
            }
            DateTime busDays = listBDUser.FirstOrDefault().BDDateOperation;
            ViewBag.CurrentBranch = listBDUser.FirstOrDefault().BranchID;
            ViewBag.BusnessDayDate = busDays.ToString("yyyy-MM-dd");
            Session["BusnessDayDate"] = busDays;

            return View();
        }

        public JsonResult GetSales(DateTime BeginDate, DateTime EndDate, int stockType, string type = "RX")
        {
            if (stockType == 3) // StockType.RX_ORDER
            {
                if (type == "RX")
                {
                    return this.GetRXSales(BeginDate, EndDate);
                }
                else
                {
                    return this.GetAllSalesForRX(BeginDate, EndDate);
                }
            }

            if (stockType == 2) // StockType.STOCK_ORDER
            {
                if (type == "RX")
                {
                    return this.GetStockOrderSales(BeginDate, EndDate);
                }
                else
                {
                    return this.GetStockOrderSalesForReOrder(BeginDate, EndDate);
                }
            }

            return null;

        }

        public JsonResult GetRXSales(DateTime startDate, DateTime endDate)
        {
            DateTime date = new DateTime(2020, 4, 1);
            date = date.AddHours(0);
            date = date.AddMinutes(0);
            date = date.AddSeconds(00);

            var model = new
            {
                data = from c in db.CumulSaleAndBills.Where(c =>  !c.IsProductDeliver && !c.isReturn && !c.IsReceived && !c.IsOrdered && c.SaleDate >= startDate &&
                                 c.SaleDate <= endDate &&
                                    c.CumulSaleAndBillLines.Any(line => line.isCommandGlass == true)
                       ).ToList()
                       select
                       new
                       {
                           IsCommandGlass = "YES",
                           CumulSaleAndBillID = c.CumulSaleAndBillID,
                           SaleDate = c.SaleDate.ToString("yyyy-MM-dd"),
                           TotalPriceHT = Util.ExtraPrices(c.CumulSaleAndBillLines.Select(sl => sl.LineAmount).Sum(),
                                                   c.RateReduction, c.RateDiscount, c.Transport, c.VatRate).TotalTTC,
                           SaleReceiptNumber = c.SaleReceiptNumber,
                           CustomerName = c.CustomerName,
                           Prescription = CumulSaleAndBill.GetPrescription(c.CumulSaleAndBillLines.ToList())
                       }
            };
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAllSalesForRX(DateTime startDate, DateTime endDate)
        {
            
            var model = new
            {
                // You cannot ordered again if the precious order is not yet received
                data = from c in db.CumulSaleAndBills.Where(c => (c.IsOrdered && c.IsReceived) &&  !c.IsProductDeliver && !c.isReturn && c.SaleDate >= startDate &&
                                 c.SaleDate <= endDate
                       ).ToList()
                       select
                       new
                       {
                           IsCommandGlass = c.CumulSaleAndBillLines.Any(line => line.isCommandGlass == true) ? "YES" : "NO",
                           CumulSaleAndBillID = c.CumulSaleAndBillID,
                           SaleDate = c.SaleDate.ToString("yyyy-MM-dd"),
                           TotalPriceHT = Util.ExtraPrices(c.CumulSaleAndBillLines.Select(sl => sl.LineAmount).Sum(),
                                                   c.RateReduction, c.RateDiscount, c.Transport, c.VatRate).TotalTTC,
                           SaleReceiptNumber = c.SaleReceiptNumber,
                           CustomerName = c.CustomerName
                       }
            };
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetStockOrderSales(DateTime startDate, DateTime endDate)
        {
         
            var model = new
            {
                data = from c in db.CumulSaleAndBills.Where(c =>  !c.IsProductDeliver && !c.isReturn && !c.IsReceived && !c.IsOrdered && c.SaleDate >= startDate &&
                                 c.SaleDate <= endDate &&
                                    c.CumulSaleAndBillLines.All(line => line.isCommandGlass == false)
                       ).ToList()
                       select
                       new
                       {
                           IsCommandGlass = "NO",
                           CumulSaleAndBillID = c.CumulSaleAndBillID,
                           SaleDate = c.SaleDate.ToString("yyyy-MM-dd"),
                           TotalPriceHT = Util.ExtraPrices(c.CumulSaleAndBillLines.Select(sl => sl.LineAmount).Sum(),
                                                   c.RateReduction, c.RateDiscount, c.Transport, c.VatRate).TotalTTC,
                           SaleReceiptNumber = c.SaleReceiptNumber,
                           CustomerName = c.CustomerName,
                           Prescription = CumulSaleAndBill.GetPrescription(c.CumulSaleAndBillLines.ToList())// 
                       }
            };
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetStockOrderSalesForReOrder(DateTime startDate, DateTime endDate)
        {
            var model = new
            {
                data = from c in db.CumulSaleAndBills.Where(c =>  !c.IsProductDeliver && !c.isReturn && (c.IsOrdered && c.IsReceived) && c.SaleDate >= startDate &&
                                 c.SaleDate <= endDate &&
                                    c.CumulSaleAndBillLines.All(line => line.isCommandGlass == false)
                       ).ToList()
                       select
                       new
                       {
                           IsCommandGlass = "NO",
                           CumulSaleAndBillID = c.CumulSaleAndBillID,
                           SaleDate = c.SaleDate.ToString("yyyy-MM-dd"),
                           TotalPriceHT = Util.ExtraPrices(c.CumulSaleAndBillLines.Select(sl => sl.LineAmount).Sum(),
                                                   c.RateReduction, c.RateDiscount, c.Transport, c.VatRate).TotalTTC,
                           SaleReceiptNumber = c.SaleReceiptNumber,
                           CustomerName = c.CustomerName
                       }
            };
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public JsonResult InitializeCommandFields(int ID)
        {
            List<object> _CommandList = new List<object>();
            try
            {

                CumulSaleAndBill autSale = (from c in db.CumulSaleAndBills
                                            where c.CumulSaleAndBillID == ID && !c.IsProductDeliver
                                            select c).SingleOrDefault();
                if (autSale == null)
                {
                    TempData["Message"] = "Warning - This sale is already validate";
                    return Json(_CommandList, JsonRequestBehavior.AllowGet);
                }

                string ProductName = "", marque = "", NumeroSerie = "", reference = "";
                int ProductID = 0, LocalizationID = 0, ProductCategoryID = 0;


                string RESphere = "",
                        RECylinder = "",
                        REAxis = "",
                        REAddition = "",
                        LESphere = "",
                        LECylinder = "",
                        LEAxis = "",
                        LEAddition = "";

                bool isRCommandGlass = false, isLCommandGlass = false;

                string SupplyingName = "", LensCategoryCode = "0", TypeLens = "";
                double FramePrice = 0d, FrameLineQuantity = 0d, StockQuantity = 0d;
                double LensPrice = 0d, LensLineQuantity = 0d, LineUnitPrice = 0d;
                double FinalLensPrice = 0d;
                string Axis = "", Addition = "", LensNumberCylindricalValue = "", LensNumberSphericalValue = "";

                List<CumulSaleAndBillLine> SaleLines = db.CumulSaleAndBillLines.Where(co => co.CumulSaleAndBillID == autSale.CumulSaleAndBillID).ToList();

                foreach (CumulSaleAndBillLine authosaleLine in SaleLines)
                {

                    if (authosaleLine.Product is GenericProduct)
                    {
                        if (authosaleLine.reference != null || authosaleLine.marque != null) //frame
                        {
                            ProductName = authosaleLine.Product.ProductCode;
                            ProductID = authosaleLine.Product.ProductID;
                            marque = authosaleLine.marque;
                            reference = authosaleLine.reference;
                            NumeroSerie = authosaleLine.NumeroSerie;
                            FramePrice = authosaleLine.LineUnitPrice;
                            FrameLineQuantity = authosaleLine.LineQuantity;
                        }
                    }
                    if ((authosaleLine.Product is Lens) || (authosaleLine.Product is OrderLens))
                    {
                        LensCategoryCode = authosaleLine.Product.Category.CategoryCode;
                        LensCategory cat = (from cate in db.LensCategories
                                            where cate.CategoryCode == LensCategoryCode
                                            select cate).SingleOrDefault();

                        SupplyingName = (cat.SupplyingName != null && cat.SupplyingName.Length > 0) ? cat.SupplyingName : cat.CategoryCode;
                        TypeLens = cat.TypeLens;
                        LensPrice += (authosaleLine.LineUnitPrice); // (autSale.SaleID != null) ? (authosaleLine.LineUnitPrice*2) : (authosaleLine.LineUnitPrice);
                        LensLineQuantity += authosaleLine.LineQuantity;
                        FinalLensPrice += authosaleLine.LineAmount;

                        ProductCategoryID = cat.CategoryID;


                        if ((authosaleLine.LensNumberSphericalValue == null || authosaleLine.LensNumberSphericalValue == "") && (authosaleLine.LensNumberCylindricalValue == null || authosaleLine.LensNumberCylindricalValue == "") && (authosaleLine.Addition == null || authosaleLine.Addition == ""))
                        {
                            if (authosaleLine.Product is Lens)
                            {
                                Lens lensProduct = db.Lenses.Find(authosaleLine.Product.ProductID);
                                Axis = authosaleLine.Axis;
                                Addition = lensProduct.LensNumber.LensNumberAdditionValue;
                                LensNumberCylindricalValue = lensProduct.LensNumber.LensNumberCylindricalValue;
                                LensNumberSphericalValue = lensProduct.LensNumber.LensNumberSphericalValue;
                            }
                            if (authosaleLine.Product is OrderLens)
                            {
                                OrderLens lensProduct = db.OrderLenses.Find(authosaleLine.Product.ProductID);
                                Axis = lensProduct.Axis;
                                Addition = lensProduct.Addition;
                                LensNumberCylindricalValue = lensProduct.LensNumberCylindricalValue;
                                LensNumberSphericalValue = lensProduct.LensNumberSphericalValue;
                            }


                        }
                        else
                        {
                            LensNumberSphericalValue = authosaleLine.LensNumberSphericalValue;
                            LensNumberCylindricalValue = authosaleLine.LensNumberCylindricalValue;
                            Axis = authosaleLine.Axis;
                            Addition = authosaleLine.Addition;
                        }

                        if (LensNumberSphericalValue == null) LensNumberSphericalValue = "";
                        if (LensNumberCylindricalValue == null) LensNumberCylindricalValue = "";
                        if (Addition == null) Addition = "";
                        if (authosaleLine.OeilDroiteGauche == EyeSide.OD)
                        {
                            if ((LensNumberSphericalValue == "") && (LensNumberCylindricalValue == ""))
                            {
                                RESphere = "0.00";
                                RECylinder = LensNumberCylindricalValue;
                                REAxis = Axis;
                                REAddition = Addition;
                            }
                            else
                            {
                                RESphere = LensNumberSphericalValue;
                                RECylinder = LensNumberCylindricalValue;
                                REAxis = Axis;
                                REAddition = Addition;
                            }
                            isRCommandGlass = authosaleLine.isCommandGlass;
                        }
                        if (authosaleLine.OeilDroiteGauche == EyeSide.OG)
                        {
                            if ((LensNumberSphericalValue == "") && (LensNumberCylindricalValue == ""))
                            {
                                LESphere = "0.00";
                                LECylinder = LensNumberCylindricalValue;
                                LEAxis = Axis;
                                LEAddition = Addition;
                            }
                            else
                            {
                                LESphere = LensNumberSphericalValue;
                                LECylinder = LensNumberCylindricalValue;
                                LEAxis = Axis;
                                LEAddition = Addition;
                            }
                            isLCommandGlass = authosaleLine.isCommandGlass;
                        }
                    }
                    LocalizationID = authosaleLine.LocalizationID;
                }

                string remarque = "";
                if (autSale.Remarque == "" || autSale.Remarque == null)
                {
                    Adress existadress = db.Customers.Find(autSale.CustomerID.Value).Adress;
                    if (existadress == null || existadress.AdressPhoneNumber == null || existadress.AdressPhoneNumber == "")
                    {
                        remarque = "NONE";
                    }
                    else
                    {
                        remarque = existadress.AdressPhoneNumber;
                    }
                }
                else
                {
                    remarque = autSale.Remarque;
                }

                LineUnitPrice = FinalLensPrice;
                _CommandList.Add(new
                {
                    CumulSaleAndBillID = autSale.CumulSaleAndBillID,
                    CustomerName = autSale.CustomerName,
                    CustomerID = autSale.CustomerID,
                    Remarque = remarque,//(autSale.Remarque == "" || autSale.Remarque == null) ? (db.Customers.Find(autSale.CustomerID.Value).Adress.AdressPhoneNumber=="") ? "NONE" : db.Customers.Find(autSale.CustomerID.Value).Adress.AdressPhoneNumber : autSale.Remarque,
                    MedecinTraitant = autSale.MedecinTraitant,

                    BranchID = autSale.BranchID,

                    ProductName = ProductName,
                    ProductID = ProductID,
                    StockQuantity = StockQuantity,
                    marque = marque,
                    NumeroSerie = NumeroSerie,
                    FramePrice = FramePrice,
                    FrameLineQuantity = FrameLineQuantity,
                    reference = reference,

                    SupplyingName = SupplyingName,
                    LensCategoryCode = LensCategoryCode,
                    ProductCategoryID = ProductCategoryID,
                    LensPrice = LensPrice,
                    LensLineQuantity = LensLineQuantity,

                    LineUnitPrice = LineUnitPrice,

                    SaleReceiptNumber = autSale.SaleReceiptNumber,
                    SaleDate = autSale.SaleDate.ToString("yyyy-MM-dd"),
                    SaleDeliveryDate = SessionBusinessDay(autSale.BranchID).BDDateOperation.ToString("yyyy-MM-dd"),

                    RESphere = RESphere,
                    RECylinder = RECylinder,
                    REAxis = REAxis,
                    REAddition = REAddition,
                    isRCommandGlass = isRCommandGlass,
                    LESphere = LESphere,
                    LECylinder = LECylinder,
                    LEAxis = LEAxis,
                    LEAddition = LEAddition,
                    isLCommandGlass = isLCommandGlass,
                    TypeLens = TypeLens,

                    LocalizationID = LocalizationID,
                    SalesProductsType = 1
                });
                return Json(_CommandList, JsonRequestBehavior.AllowGet);


            }
            catch (Exception e)
            {
                TempData["Message"] = "Error " + e.Message;
                return Json(_CommandList, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult Order(CumulSaleAndBill currentSale, string heureOperation, int stockType, int spray = 0, int cases = 0)
        {
            bool status = false;
            string Message = "";
            try
            {
                currentSale.OrderDate = SessionBusinessDay(currentSale.BranchID).BDDateOperation;
                StockType currentStockType = stockType == 2 ? StockType.STOCK_ORDER : StockType.RX_ORDER;
                bool res = _prodLocalRepo.OrderLenses(currentSale, SessionGlobalPersonID, currentStockType);
                status = true;
                if (stockType == 3) // StockType.RX_ORDER
                {
                    Message = Resources.Success + " - " + "RX Lenses Has Been Successfully Posted";
                }

                if (stockType == 2) // StockType.Stock_ORDER
                {
                    Message = Resources.Success + " - " + "Stock Order Lenses Has Been Successfully Posted";
                }
                    /*Resources.ReceiveSpecialOrder*/;
            }
            catch (Exception e)
            {
                status = false;
                Message = "Error " + e.Message;
            }
            return new JsonResult { Data = new { status = status, Message = Message } };
        }

        public JsonResult GetControllers()
        {
            List<object> controllers = LoadComponent.GetAllEmployees(CurrentBranch.BranchID, SessionGlobalPersonID);
            return Json(controllers, JsonRequestBehavior.AllowGet);
        }
    }
}