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
using CABOPMANAGEMENT.Filters;

namespace CABOPMANAGEMENT.Areas.CCM.Controllers
{
    [Authorize(Order = 1)]
    [TakeBusinessDay(Order = 2)]
    public class DeliverDeskController : BaseController
    {
        
        
        private ISale _saleRepository;
        private IBusinessDay _busDayRepo;
        private IProductLocalization _prodLocalRepo;

        private LensConstruction lensFrameConstruction = new LensConstruction();

        public DeliverDeskController(
            ISale saleRepository,
            IBusinessDay busDayRepo,
            IProductLocalization prodLocalRepo
        )
        {
            this._saleRepository = saleRepository;
            this._busDayRepo = busDayRepo;
            this._prodLocalRepo = prodLocalRepo;
        }

        // GET: CRM/DeliverDesk
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
                ViewBag.BusnessDayDate = UserBusDays.FirstOrDefault().BDDateOperation.ToString("yyyy-MM-dd");// businessDay.BDDateOperation;

                Session["BusnessDayDate"] = currentDateOp;

                int deviseID = (Session["DefaultDeviseID"] == null) ? 0 : (int)Session["DefaultDeviseID"];
                if (deviseID <= 0)
                {
                    InjectUserConfigInSession();
                }
                deviseID = (Session["DefaultDeviseID"] == null) ? 0 : (int)Session["DefaultDeviseID"];
                ViewBag.DefaultDeviseID = deviseID;
                ViewBag.DefaultDevise = (deviseID <= 0) ? "" : db.Devises.Find(deviseID).DeviseCode;

                Session["DebtInsured"] = 0d;

                ViewBag.SoldDate = currentDateOp.ToString("yyyy-MM-dd");
                return View(ModelSaleValidate());
            }

            catch (Exception e)
            {
                TempData["Message"] = "Error " + e.Message;
                ViewBag.DisplayForm = 0;
                return this.View();
            }
        }

        public JsonResult ModelSaleValidate()
        {
            DateTime date = new DateTime(2020, 4, 1);
            date = date.AddHours(0);
            date = date.AddMinutes(0);
            date = date.AddSeconds(00);

            var model = new
            {
                // Les ventes qui ne contiennent pas les verres peuvent etre livrees sans montage prealable:
                // C'est le cas par example des frames ou des sprays et autres ventes
                data = from c in db.CumulSaleAndBills.Where(c => !c.isReturn && !c.IsProductDeliver &&
                                                                  c.DateOperationHours >= date &&
                      (c.IsMounted || (!c.CumulSaleAndBillLines.Any(l => l.Product is Lens || l.Product is OrderLens)))).ToList()
                       select
                       new
                       {
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
                    CustomerName = (autSale.Customer==null) ? autSale.CustomerName: autSale.Customer.CustomerFullName,
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
                    MountingBy = autSale.MountingBy,
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

        public JsonResult ValideDeliverDesk(CumulSaleAndBill currentSale, string heureOperation, int spray = 0, int cases = 0)
        {
            bool status = false;
            string Message = "";
            try
            {
                //Session["Receipt_CumulSaleAndBillID"] = null;
                Session["Receipt_CustomerID"] = null;
                // Puissequ'on ne faire plus la sortie de stock, je commente ces lignes
                /*
                status = this.DoYes( spray, cases);
                if (!status)
                {
                    Message = (string)Session["SessionMessage"];
                    status = false;
                    return new JsonResult { Data = new { status = status, Message = Message } };
                }*/

                currentSale.ProductDeliverDate = SessionBusinessDay(currentSale.BranchID).BDDateOperation;

                bool res = _prodLocalRepo.ValideDeliverDesk(currentSale, heureOperation, SessionGlobalPersonID);
                    
                Session["Receipt_CustomerID"] = currentSale.CustomerName;


                PrintReset(currentSale.BranchID.ToString());
              

                status = true;
                Message = Resources.Success + " - " + Resources.ProductDeliver;
            }
            catch (Exception e)
            {
                status = false;
                Message = "Error " + e.Message; // + " " + e.StackTrace + " " + e.InnerException;
            }
            return new JsonResult { Data = new { status = status, Message = Message } };
        }
        public void PrintReset(string Branch)
        {
            Session["CurrentProduct"] = new Product();
            Session["MaxValue"] = 0;
        }

        //This method add a saleline in the current sale

        public bool DoYes(int spray, int cases)
        {
            bool res = false;
            try
            {
                //double VatRate = CodeValue.Accounting.ParamInitAcct.VATRATE;
                Session["SessionMessage"] = "OK";
               
                if (spray > 0 || cases > 0) 
                {
                    res = this.CheckQty(spray, cases);
                    if (!res)
                        return res;
                }
              
                res = true;

                return res;
            }
            catch (Exception e)
            {
                res = false;
                Session["SessionMessage"] = "Error " + e.Message;
                return res;
            }
        }

        private double GetSprayQuantityStock(int LocalizationID)
        {
            double res = 0d;
            var lstProduct = db.Products.Join(db.Categories, p => p.CategoryID, c => c.CategoryID,
                                (p, c) => new { p, c })
                                .Where(pc => pc.c.CategoryID == 2 && pc.p.ProductCode.Contains("SPRAY"))
                                .Select(rdv => new
                                {
                                    ProductID = rdv.p.ProductID
                                }).FirstOrDefault();

            int ProductId = lstProduct.ProductID;

            ProductLocalization productInStock = db.ProductLocalizations.FirstOrDefault(pl => pl.ProductID == ProductId && pl.LocalizationID == LocalizationID);
            res = productInStock.ProductLocalizationStockQuantity;

            return res;
        }

        private double GetCaseQuantityStock(int LocalizationID)
        {
            double res = 0d;
            var lstProduct = db.Products.Join(db.Categories, p => p.CategoryID, c => c.CategoryID,
                                (p, c) => new { p, c })
                                .Where(pc => pc.c.CategoryID == 2 && pc.p.ProductCode.Contains("CASE"))
                                .Select(rdv => new
                                {
                                    ProductID = rdv.p.ProductID
                                }).FirstOrDefault();

            int ProductId = lstProduct.ProductID;

            ProductLocalization productInStock = db.ProductLocalizations.FirstOrDefault(pl => pl.ProductID == ProductId && pl.LocalizationID == LocalizationID);
            res=productInStock.ProductLocalizationStockQuantity ;

            return res;
        }

        public bool CheckQty(int spray, int cases)
        {
            bool res = false;
            
            double currentQteSpayEnStock = 0d;
            double currentQteCaseEnStock = 0d;

            Session["SessionMessage"] = "OK";
            try
            {
                int BranchID = SessionBusinessDay(null).BranchID;

                Localization loc = db.Localizations.Where(c => c.BranchID == BranchID).FirstOrDefault();
                int LocalizationID = loc.LocalizationID;

                if (spray > 0 )
                {
                    currentQteSpayEnStock = GetSprayQuantityStock(LocalizationID);
                }
                if (cases > 0)
                {
                    currentQteCaseEnStock = GetCaseQuantityStock(LocalizationID);
                }

                bool isStockControl = (bool)Session["isStockControl"];
                
                if (isStockControl)
                {
                    if (spray > 0 && currentQteSpayEnStock <= 0)
                    {
                        res = false;
                        statusOperation = "Spray not in Stock";
                        Session["SessionMessage"] = statusOperation;

                        return res;
                    }

                    if (cases > 0 && currentQteCaseEnStock <= 0)
                    {
                        res = false;
                        statusOperation = "Case not in Stock";
                        Session["SessionMessage"] = statusOperation;

                        return res;
                    }
                }

                res = true;
                return res;
            }
            catch (Exception e)
            {
                Session["SessionMessage"] = "Error " + e.Message;
                return res;
            }
        }

        

        
    }
}