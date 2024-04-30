﻿using FastSod.Utilities.Util;
using FatSod.Ressources;
using FatSod.Security.Abstracts;
using FatSod.Security.Entities;
using FatSod.Supply.Abstracts;
using FatSod.Supply.Entities;
using VALDOZMANAGEMENT.Controllers;
using VALDOZMANAGEMENT.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace VALDOZMANAGEMENT.Areas.CRM.Controllers
{
    [Authorize(Order = 1)]
    [TakeBusinessDay(Order = 2)]
    public class DeliverSpecialOrderController : BaseController
    {
        
        private const string CONTROLLER_NAME = "CRM/DeliverSpecialOrder";
        private const string VIEW_NAME = "Index";

        private ISale _saleRepository;
        private IBusinessDay _busDayRepo;


        private BusinessDay businessDay;

        private LensConstruction lensFrameConstruction = new LensConstruction();

        public DeliverSpecialOrderController(
            ISale saleRepository,
            IBusinessDay busDayRepo
        )
        {
            this._saleRepository = saleRepository;
            this._busDayRepo = busDayRepo;
        }

        // GET: CRM/DeliverSpecialOrder

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
                Session["salelinesnoninsured"] = new List<CumulSaleAndBillLine>();

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

            List<CumulSaleAndBill> lstSale = db.CumulSaleAndBills.Where(c => c.IsProductReveive && !c.IsCustomerRceive && !c.isReturn).ToList();
            var model = new
            {
                data = from c in lstSale.ToList()
                       select
                       new
                       {
                           CumulSaleAndBillID = c.CumulSaleAndBillID,
                           SaleDate = c.SaleDate.ToString("yyyy-MM-dd"),
                           TotalPriceHT = Util.ExtraPrices(c.CumulSaleAndBillLines.Select(sl => sl.LineAmount).Sum(),
                                                   c.RateReduction, c.RateDiscount, c.Transport, c.VatRate).TotalTTC,
                           //CustomerAdvance=db.CustomerSlices.Where(csl=>csl.CumulSaleAndBillID==c.CumulSaleAndBillID).Select(csl=>csl.SliceAmount).Sum(),
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
                                            where c.CumulSaleAndBillID == ID && c.IsProductReveive && !c.IsCustomerRceive
                                 select c).SingleOrDefault();
                if (autSale == null)
                {
                    TempData["Message"] = "Warning - This sale is already validate";
                    return Json(_CommandList, JsonRequestBehavior.AllowGet);
                }


                int LocalizationID = 0;

                string RESphere = "",
                        RECylinder = "",
                        REAxis = "",
                        REAddition = "",
                        LESphere = "",
                        LECylinder = "",
                        LEAxis = "",
                        LEAddition = "";

                string SupplyingName = "", LensCategoryCode = "0", TypeLens = "";

                double LensPrice = 0d, LensLineQuantity = 0d, LineUnitPrice = 0d;
                double FinalLensPrice = 0d;
                List<CumulSaleAndBillLine> SaleLines = db.CumulSaleAndBillLines.Where(co => co.CumulSaleAndBillID == autSale.CumulSaleAndBillID).ToList();

                foreach (CumulSaleAndBillLine authosaleLine in SaleLines)
                {
                    if (!(authosaleLine.Product is OrderLens))
                    {

                    }
                    else
                    {
                        LensCategoryCode = authosaleLine.Product.Category.CategoryCode;
                        LensCategory cat = (from cate in db.LensCategories
                                            where cate.CategoryCode == LensCategoryCode
                                            select cate).SingleOrDefault();

                        SupplyingName = (cat.SupplyingName != null && cat.SupplyingName.Length > 0) ? cat.SupplyingName : cat.CategoryCode;
                        TypeLens = cat.TypeLens;
                        
                        LensPrice += (authosaleLine.LineUnitPrice); //(autSale.SaleID != null) ? (authosaleLine.LineUnitPrice * 2) : (authosaleLine.LineUnitPrice);
                        LensLineQuantity += authosaleLine.LineQuantity;
                        FinalLensPrice += authosaleLine.LineAmount;
                        if (authosaleLine.OeilDroiteGauche == EyeSide.OD)
                        {
                            RESphere = authosaleLine.LensNumberSphericalValue;
                            RECylinder = authosaleLine.LensNumberCylindricalValue;
                            REAxis = authosaleLine.Axis;
                            REAddition = authosaleLine.Addition;
                        }
                        if (authosaleLine.OeilDroiteGauche == EyeSide.OG)
                        {
                            LESphere = authosaleLine.LensNumberSphericalValue;
                            LECylinder = authosaleLine.LensNumberCylindricalValue;
                            LEAxis = authosaleLine.Axis;
                            LEAddition = authosaleLine.Addition;
                        }

                    }
                    LocalizationID = authosaleLine.LocalizationID;
                }

                LineUnitPrice = FinalLensPrice;
                _CommandList.Add(new
                {
                    CumulSaleAndBillID = autSale.CumulSaleAndBillID,
                    CustomerName = autSale.CustomerName,
                    CustomerID = autSale.CustomerID,
                    Remarque = (autSale.Remarque == "" || autSale.Remarque == null) ? db.Customers.Find(autSale.CustomerID.Value).Adress.AdressPhoneNumber : autSale.Remarque,
                    MedecinTraitant = autSale.MedecinTraitant,

                    BranchID = autSale.BranchID,

                    SupplyingName = SupplyingName,
                    LensCategoryCode = LensCategoryCode,
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
                    LESphere = LESphere,
                    LECylinder = LECylinder,
                    LEAxis = LEAxis,
                    LEAddition = LEAddition,
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


        public JsonResult ValideDeliverSpecialOrder(SpecialLensModel slm, CumulSaleAndBill currentSale)
        {
            bool status = false;
            string Message = "";
            try
            {
                Session["Receipt_CumulSaleAndBillID"] = null;
                Session["Receipt_CustomerID"] = null;
                Session["salelinesnoninsured"] = new List<CumulSaleAndBillLine>();
                //fabrication des lignes de commande
                status = this.DoYes(slm);
                if (!status)
                {
                    Message = (string)Session["SessionMessage"];
                    status = false;
                    return new JsonResult { Data = new { status = status, Message = Message } };
                }


                currentSale.CumulSaleAndBillLines = (List<CumulSaleAndBillLine>)Session["salelinesnoninsured"];

                if (currentSale.CumulSaleAndBillLines.Count > 0)
                {


                    int CumulSaleAndBillID = _saleRepository.ValideDeliverSpecialOrder(currentSale, SessionGlobalPersonID).CumulSaleAndBillID;
                    Session["Receipt_CumulSaleAndBillID"] = CumulSaleAndBillID;
                    Session["Receipt_CustomerID"] = currentSale.CustomerName;


                    PrintReset(currentSale.BranchID.ToString(), CumulSaleAndBillID);
                }

                status = true;
                Message = Resources.Success + " - " + Resources.DeliverSpecialOrder;
            }
            catch (Exception e)
            {
                status = false;
                Message = "Error " + e.Message + " " + e.StackTrace + " " + e.InnerException;
            }
            return new JsonResult { Data = new { status = status, Message = Message } };
        }
        public void PrintReset(string Branch, int CumulSaleAndBillID)
        {
            Session["salelinesnoninsured"] = new List<CumulSaleAndBillLine>();
            Session["CurrentProduct"] = new Product();
            Session["MaxValue"] = 0;

            Session["CumulSaleAndBillID"] = CumulSaleAndBillID;

        }

        //This method add a saleline in the current sale

        public bool DoYes(SpecialLensModel slm)
        {
            bool res = false;
            try
            {
                //double VatRate = CodeValue.Accounting.ParamInitAcct.VATRATE;
                Session["SessionMessage"] = "OK";
                List<CumulSaleAndBillLine> salelines = (List<CumulSaleAndBillLine>)Session["salelinesnoninsured"];
                List<CumulSaleAndBillLine> cols = lensFrameConstruction.Get_CUMSALEBILLCOL_From_SLM(slm, new FatSod.DataContext.Concrete.EFDbContext(), 0, 0);
                foreach (CumulSaleAndBillLine saleLine in cols)
                {

                    //Construction du code du produit en fonction de ce qui a été saisie par l'utilisateur
                    saleLine.Product = LensConstruction.GetProductByCumulSaleAndBillLine(saleLine, new FatSod.DataContext.Concrete.EFDbContext());
                    if (saleLine.Product == null)
                    {
                        //ApplyExtraPrices(salelines, reduction, discount, transport, VatRate);
                        Session["salelinesnoninsured"] = salelines;
                        res = true;
                        return res;
                    }

                    if (saleLine.LineID > 0)
                    {
                        //Ce produit existe deja dans le panier, alors on enleve les deux lignes liées au SpecialOrderLineCode dans la ligne
                        //1-Coe c'est une modification, on enlève l'existant de la ligne en cours de modification; on va l'ajouter dans la suite(Drop and Create)

                        salelines.RemoveAll(col => col.LineID == saleLine.LineID);
                        //2-Si actuellement on a une seule ligne dans la collection, il y a une possibilité qu'on en avait deux et l'autre a été supprimée; il faut donc le supprimer dans le panier
                        if (cols.Count <= 1) salelines.RemoveAll(col => col.SpecialOrderLineCode == saleLine.SpecialOrderLineCode);
                    }

                    if (salelines != null && salelines.Count() > 0)
                    {
                        CumulSaleAndBillLine saleLineExist = salelines.FirstOrDefault(s => s.Product.ProductCode == saleLine.Product.ProductCode && s.SpecialOrderLineCode == saleLine.SpecialOrderLineCode && s.EyeSide == saleLine.EyeSide);
                        if (saleLineExist != null)
                        {
                            salelines.Remove(saleLineExist);
                        }

                        int maxLineID = (salelines != null && salelines.Count() > 0) ? salelines.Select(l => l.LineID).Max() : 0;

                        saleLine.LineID = (maxLineID + 1);

                        salelines.Add(saleLine);
                    }
                    else
                    {
                        salelines = new List<CumulSaleAndBillLine>();
                        saleLine.LineID = 1;
                        salelines.Add(saleLine);
                    }
                }

                //double VatRate = CodeValue.Accounting.ParamInitAcct.VATRATE;
                //ApplyExtraPrices(salelines, reduction, discount, transport, VatRate);
                Session["salelinesnoninsured"] = salelines;

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


    }
}