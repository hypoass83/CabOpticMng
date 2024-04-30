﻿using FastSod.Utilities.Util;
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
    public class LensMountingController : BaseController
    {
        
        
        private ISale _saleRepository;
        private IBusinessDay _busDayRepo;
        private IProductLocalization _prodLocalRepo;

        private LensConstruction lensFrameConstruction = new LensConstruction();

        public LensMountingController(
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

                data = from c in db.CumulSaleAndBills.Where(c => c.DateOperationHours >= date && !c.IsMounted &&
                       !c.isReturn /*&& !c.IsProductDeliver*/ &&
                        (
                        // Ce n'est pas un verre de commande
                        c.CumulSaleAndBillLines.All(line => line.isCommandGlass == false) ||
                        // C'est un verre de commande et sa a ete recu
                         (c.CumulSaleAndBillLines.Any(line => line.isCommandGlass == true) && c.IsReceived)
                        )
                        ).ToList()
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
                    CustomerName = (autSale.Customer == null) ? autSale.CustomerName : autSale.Customer.CustomerFullName,
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

        public JsonResult ValideDeliverDesk(SpecialLensModel slm, CumulSaleAndBill currentSale, string heureOperation, int spray = 0, int cases = 0)
        {
            bool status = false;
            string Message = "";
            try
            {
                //Session["Receipt_CumulSaleAndBillID"] = null;
                Session["Receipt_CustomerID"] = null;
                Session["CumulSaleAndBillLine"] = new List<CumulSaleAndBillLine>();


                status = this.DoYes(slm, 0, 0);
                if (!status)
                {
                    Message = (string)Session["SessionMessage"];
                    status = false;
                    return new JsonResult { Data = new { status = status, Message = Message } };
                }
                currentSale.CumulSaleAndBillLines = (List<CumulSaleAndBillLine>)Session["CumulSaleAndBillLine"];
                currentSale.ProductDeliverDate = SessionBusinessDay(currentSale.BranchID).BDDateOperation;

                bool res = _prodLocalRepo.ValidateLensMounting(currentSale, heureOperation, SessionGlobalPersonID);
                    
                Session["Receipt_CustomerID"] = currentSale.CustomerName;


                PrintReset(currentSale.BranchID.ToString());
              

                status = true;
                Message = Resources.Success + " - " + Resources.LensMounting;
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

        public bool DoYes(SpecialLensModel slm, int spray, int boitier)
        {
            bool res = false;
            try
            {
                //double VatRate = CodeValue.Accounting.ParamInitAcct.VATRATE;
                Session["SessionMessage"] = "OK";
                List<CumulSaleAndBillLine> cumulSaleAndBillLines = (List<CumulSaleAndBillLine>)Session["CumulSaleAndBillLine"];


                List<CumulSaleAndBillLine> cols = lensFrameConstruction.Get_CUMSALEBILLCOL_From_SLM(slm, new FatSod.DataContext.Concrete.EFDbContext(), spray, boitier);
                foreach (CumulSaleAndBillLine saleLine in cols)
                {

                    //Construction du code du produit en fonction de ce qui a été saisie par l'utilisateur
                    saleLine.Product = LensConstruction.GetProductByCumulSaleAndBillLineSOUT(saleLine, new FatSod.DataContext.Concrete.EFDbContext());
                    if (saleLine.Product == null)
                    {
                        //ApplyExtraPrices(salelines, reduction, discount, transport, VatRate);
                        Session["CumulSaleAndBillLine"] = cumulSaleAndBillLines;
                        Session["SessionMessage"] = "Error - This product with category " + saleLine.LensCategoryCode + " not yet created";
                        res = false;
                        return res;
                    }
                    saleLine.ProductID = saleLine.Product.ProductID;
                    // Puissequ'on ne faire plus la sortie de stock, je commente ces lignes
                    /*
                    if ((saleLine.Product is Lens) && saleLine.ProductID > 0) // && (spray > 0 || boitier > 0))
                    {
                        res = this.CheckQty(saleLine.LocalizationID, saleLine.Product.ProductID, saleLine.NumeroSerie, saleLine.LineQuantity, spray, boitier);
                        if (!res)
                            return res;
                    }
                    */

                    if (saleLine.LineID > 0)
                    {
                        //Ce produit existe deja dans le panier, alors on enleve les deux lignes liées au SpecialOrderLineCode dans la ligne
                        //1-Coe c'est une modification, on enlève l'existant de la ligne en cours de modification; on va l'ajouter dans la suite(Drop and Create)

                        cumulSaleAndBillLines.RemoveAll(col => col.LineID == saleLine.LineID);
                        //2-Si actuellement on a une seule ligne dans la collection, il y a une possibilité qu'on en avait deux et l'autre a été supprimée; il faut donc le supprimer dans le panier
                        if (cols.Count <= 1) cumulSaleAndBillLines.RemoveAll(col => col.SpecialOrderLineCode == saleLine.SpecialOrderLineCode);
                    }

                    if (cumulSaleAndBillLines != null && cumulSaleAndBillLines.Count() > 0)
                    {
                        CumulSaleAndBillLine saleLineExist = cumulSaleAndBillLines.FirstOrDefault(s => s.Product.ProductCode == saleLine.Product.ProductCode && s.SpecialOrderLineCode == saleLine.SpecialOrderLineCode && s.EyeSide == saleLine.EyeSide);
                        if (saleLineExist != null)
                        {
                            cumulSaleAndBillLines.Remove(saleLineExist);
                        }

                        int maxLineID = (cumulSaleAndBillLines != null && cumulSaleAndBillLines.Count() > 0) ? cumulSaleAndBillLines.Select(l => l.LineID).Max() : 0;

                        saleLine.LineID = (maxLineID + 1);

                        cumulSaleAndBillLines.Add(saleLine);
                    }
                    else
                    {
                        cumulSaleAndBillLines = new List<CumulSaleAndBillLine>();
                        saleLine.LineID = 1;
                        cumulSaleAndBillLines.Add(saleLine);
                    }
                }

                //double VatRate = CodeValue.Accounting.ParamInitAcct.VATRATE;
                //ApplyExtraPrices(salelines, reduction, discount, transport, VatRate);
                Session["CumulSaleAndBillLine"] = cumulSaleAndBillLines;

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

        public bool CheckQty(int LocalizationID, int? ProductID, string NumeroSerie, double LineQuantity, int spray, int boitier)
        {
            bool res = false;
            double currentQteEnStock = 0d;
            Session["SessionMessage"] = "OK";
            try
            {
                //if (spray > 0 || boitier > 0)
                //{
                currentQteEnStock = GetQuantityStock(LocalizationID.ToString(), ProductID.ToString(), NumeroSerie);
                //}
                //else
                //{
                //    currentQteEnStock = (double)Session["MaxValue"];
                //}


                //recherche des qtes commandes en attente de validation pr ce produit et cette localization
                double qtyComNonValide = 0d;

                bool isStockControl = (bool)Session["isStockControl"];

                if (isStockControl)
                {
                    double safetyQty = (double)Session["SafetyStock"];
                    if (currentQteEnStock - (qtyComNonValide + LineQuantity) <= 0) //plus de produit en stock
                    {
                        res = this.AlertMsgSock(qtyComNonValide + LineQuantity, safetyQty);
                        if (!res) return res;
                    }
                    if (currentQteEnStock - (qtyComNonValide + LineQuantity) <= safetyQty) //stock de securite atteint
                    {
                        res = this.AlertMsgSock(qtyComNonValide + LineQuantity, safetyQty);
                        if (!res) return res;
                    }
                }

                if (LineQuantity <= 0)
                {
                    res = false;
                    statusOperation = Resources.cmdMontantObligatoire;
                    Session["SessionMessage"] = statusOperation;

                    return res;
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

        public bool AlertMsgSock(double QuantityValue, double safetyQty)
        {
            bool res = true;
            double maxQuantity = (double)Session["MaxValue"];
            string productLabel = (string)Session["CurrentProduct"];
            Session["SessionMessage"] = "";
            if (safetyQty > QuantityValue)
            {
                Session["SessionMessage"] = "Attention - En vendant cette quantité le seuil de sécurité sera atteind. Veuillez, faire un réapprovisionnement le plutôt possible";
                res = true;
            }
            if (maxQuantity < QuantityValue)
            {
                Session["SessionMessage"] = "Erreur - " + productLabel + " : " + Resources.EnoughQuantityStock + " " + maxQuantity;
                res = false;
            }
            return res;
        }

        public double GetQuantityStock(string Localization, string CurrentProduct, string NumeroSerie)
        {
            double LineQuantity = 0d;

            if ((Localization != null && CurrentProduct != null) && (Localization.Length > 0 && CurrentProduct.Length > 0))
            {

                int idLoc = Convert.ToInt32(Localization);
                int idProd = Convert.ToInt32(CurrentProduct);

                //check if it is product with serial number
                Category productcat = db.Products.Find(idProd).Category;
                ProductLocalization productInStock = new ProductLocalization();
                if (idLoc > 0 && idProd > 0)
                {
                    if (!(productcat.isSerialNumberNull))
                    {
                        productInStock = db.ProductLocalizations.FirstOrDefault(pL => pL.LocalizationID == idLoc && pL.ProductID == idProd);
                    }
                    else
                    {
                        //if ((NumeroSerie != null && NumeroSerie != null))
                        //{
                        productInStock = db.ProductLocalizations.FirstOrDefault(pL => pL.LocalizationID == idLoc && pL.ProductID == idProd && pL.NumeroSerie == NumeroSerie);
                        //}
                    }

                    Session["CurrentProduct"] = productInStock.ProductCode + " " + productInStock.NumeroSerie;
                    if (productInStock == null || Math.Abs(productInStock.ProductLocalizationStockQuantity) <= 0)
                    {
                        LineQuantity = 0d;
                        Session["MaxValue"] = 0d;
                        Session["SafetyStock"] = 0d;
                    }
                    else
                    {
                        LineQuantity = productInStock.ProductLocalizationStockQuantity;
                        Session["MaxValue"] = productInStock.ProductLocalizationStockQuantity;
                        Session["SafetyStock"] = productInStock.ProductLocalizationSafetyStockQuantity;
                    }
                }
            }

            return LineQuantity;

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
                    if (currentQteSpayEnStock>0)
                    {
                    }
                    else
                    {
                        res = false;
                        statusOperation = "Spray not in Stock";
                        Session["SessionMessage"] = statusOperation;

                        return res;
                    }

                    if (currentQteCaseEnStock > 0)
                    {
                    }
                    else
                    {
                        res = false;
                        statusOperation = "Frame not in Stock";
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