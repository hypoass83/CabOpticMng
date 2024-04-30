using FatSod.DataContext.Initializer;
using FatSod.DataContext.Repositories;
using FatSod.Ressources;
using FatSod.Security.Abstracts;
using FatSod.Security.Entities;
using FatSod.Supply.Abstracts;
using FatSod.Supply.Abstracts.BarCode;
using FatSod.Supply.Entities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CABOPMANAGEMENT.Areas.Sale.Models;
using CABOPMANAGEMENT.Controllers;
using CABOPMANAGEMENT.Filters;
using CABOPMANAGEMENT.Tools;
using ZXing;

namespace CABOPMANAGEMENT.Areas.BarCode.Controllers
{
    [Authorize(Order = 1)]
    [TakeBusinessDay(Order = 2)]
    public class PrintBarcodeController : BaseController
    {

        //Current Controller and current page
        private const string CONTROLLER_NAME = "Supply/ListOfStockInput";
        private const string VIEW_NAME = "Index";

        private IInventoryDirectory _inventoryDirectoryRepository;
        private readonly IBarCodeService barCodeService;

        public PrintBarcodeController(
                 IInventoryDirectory inventoryDirectoryRepository,
                 IBarCodeService barCodeService
                )
        {
            this._inventoryDirectoryRepository = inventoryDirectoryRepository;
            this.barCodeService = barCodeService;
        }
        // GET: CashRegister/ListOfStockInput
        [OutputCache(Duration = 3600)]
        public ActionResult Index()
        {
            ViewBag.DisplayForm = 1;
            try
            {

                //we ensure that if this user manage cash register. If he manage it, will verify if till is closed else, we ask he to closed it before login off

                List<BusinessDay> UserBusDays = (List<BusinessDay>)Session["UserBusDays"];
                DateTime currentDateOp = UserBusDays.FirstOrDefault().BDDateOperation;
                ViewBag.CurrentBranch = UserBusDays.FirstOrDefault().BranchID;
                ViewBag.BusnessDayDate = UserBusDays.FirstOrDefault().BDDateOperation.ToString("yyyy-MM-dd");// businessDay.BDDateOperation;


                //ViewBag.CurrentTill = userTill.TillID;

                return View();
            }

            catch (Exception e)
            {
                TempData["Message"] = "Error " + e.Message;
                ViewBag.DisplayForm = 0;
                return this.View();
            }
        }



        public JsonResult ModelInventoryDirectory(int BranchID, DateTime Bdate, DateTime Edate, bool IsPrintedBarcodeShown = true)
        {

            var list = new List<InventoryDirectory>();
            if (IsPrintedBarcodeShown)
            {
                list = _inventoryDirectoryRepository.FindAll.Where(id =>
                                                        !id.InventoryDirectoryDescription.Contains("RX Order Reception") &&
                                                        id.BranchID == BranchID &&
                                                        (id.InventoryDirectoryDate >= Bdate.Date &&
                                                        id.InventoryDirectoryDate <= Edate.Date)).ToList();
            }
            else
            {
                list = _inventoryDirectoryRepository.FindAll.Where(id =>
                                                        !id.IsBarcodePrinted &&
                                                        !id.InventoryDirectoryDescription.Contains("RX Order Reception") &&
                                                        id.BranchID == BranchID &&
                                                        (id.InventoryDirectoryDate >= Bdate.Date &&
                                                        id.InventoryDirectoryDate <= Edate.Date)).ToList();
            }


            var model = new
            {
                data = from id in list
                       select
                       new
                       {
                           InventoryDirectoryID = id.InventoryDirectoryID,
                           Branch = id.Branch.BranchName,
                           InventoryDirectoryReference = id.InventoryDirectoryReference,
                           InventoryDirectoryCreationDate = id.InventoryDirectoryCreationDate.ToString("yyyy-MM-dd"),
                           InventoryDirectoryDescription = id.InventoryDirectoryDescription,
                           Operator = id.RegisteredBy?.FullName,
                           IsBarcodePrinted = id.IsBarcodePrinted ? "YES" : "NO"
                       }
            };

            return Json(model, JsonRequestBehavior.AllowGet);

        }
        /*** methodes lors du chargement du formulaire*********************/
        public JsonResult GetOpenedBranches()
        {

            IBusinessDay busDayRepo = new BusinessDayRepository();
            List<object> openedBranchesList = new List<object>();
            List<Branch> openedBranches = busDayRepo.GetOpenedBranches();
            foreach (Branch branch in openedBranches)
            {
                openedBranchesList.Add(new
                {
                    BranchID = branch.BranchID,
                    BranchName = branch.BranchName
                });
            }

            return Json(openedBranchesList, JsonRequestBehavior.AllowGet);
        }

        private Company Company
        {
            get
            {
                return db.Companies.FirstOrDefault();
            }
        }

        public JsonResult CreatePayloads(int InventoryDirectoryID)
        {
            bool status = false;
            string Message = "";
            var payloads = new List<BarCodePayload>();
            try
            {
                var InventoryDirectoryLines = db.InventoryDirectoryLines.Where(l =>
                                                                        l.InventoryDirectoryID == InventoryDirectoryID).ToList();

                if (InventoryDirectoryLines != null && InventoryDirectoryLines.Count() > 0)
                {

                    InventoryDirectoryLines.ForEach(line =>
                    {
                        var payload = GenerateBarCode(line, 1);
                        payloads.Add(payload);
                    });

                    var inventoryDirectory = db.InventoryDirectories.SingleOrDefault(id => id.InventoryDirectoryID == InventoryDirectoryID);
                    inventoryDirectory.IsBarcodePrinted = true;
                    db.SaveChanges();

                    Session["InventoryDirectoryLines"] = new List<InventoryDirectoryLine>();
                    status = true;
                    Message = Resources.Success + " - " + statusOperation;
                }
                else
                {
                    status = false;
                    Message = Resources.AlertError + " - " + "Please Add Products to your Cady";
                }

            }
            catch (Exception e)
            {
                status = false;
                Message = "Error " + e.Message + " " + e.StackTrace + " " + e.InnerException;
            }
            Session["payloads"] = payloads;
            return new JsonResult { Data = new { status = status, Message = Message } };
        }

        public BarCodePayload GenerateBarCode(InventoryDirectoryLine invDirLine, int isSerialNumberNull = 0)
        {
            var status = true;
            var statusOperation = "Success";

            var payload = new BarCodePayload()
            {
                Quantity = int.Parse("" + invDirLine.NewStockQuantity.Value),
            };

            #region MyRegion
            if (isSerialNumberNull > 0)
            {
                //nous devons nous assurer que le numero de serie est unique pour la categorie et la marque concerner
                ProductLocalization stock = db.ProductLocalizations.SingleOrDefault(p =>
                                                                p.LocalizationID == invDirLine.LocalizationID &&
                                                                p.ProductID == invDirLine.ProductID &&
                                                                p.NumeroSerie.Trim() == invDirLine.NumeroSerie.Trim() &&
                                                                p.Marque == invDirLine.Marque);
                if (stock == null)
                {
                    // Creation du stock
                    stock = CreateFrameStock(invDirLine.ProductID, invDirLine.LocalizationID, invDirLine.NumeroSerie, invDirLine.Marque);
                    stock = db.ProductLocalizations.SingleOrDefault(p => p.ProductLocalizationID == stock.ProductLocalizationID);
                }

                payload.ProductId = stock.ProductID;
                payload.LocationId = stock.LocalizationID;
                payload.ProductPrice = stock.Product.SellingPrice;
                payload.Marque = stock.Marque;
                payload.NumeroSerie = stock.NumeroSerie;

                payload.BarCode = barCodeService.GetBarCode(payload);

                #region Improvement
                if (stock.NumeroSerie == null)
                {
                    payload.NumeroSerie = stock.Product.ProductLabel;
                }

                if (stock.Marque == null)
                {
                    payload.Marque = stock.Product.Category.CategoryCode;
                }

                if (stock.NumeroSerie != null && (stock.Product is GenericProduct))
                {
                    if (stock.Product.ProductCode == "ECONOMIQUE")
                    {
                        payload.Marque += " (E)";
                    }

                    if (stock.Product.ProductCode == "ECO+")
                    {
                        payload.Marque += " (E+)";
                    }
                }
                #endregion

                payload.Marque = payload.Marque?.Length <= 20 ? payload.Marque : payload.Marque?.Substring(0, 20);
                payload.NumeroSerie = payload.NumeroSerie?.Length <= 29 ? payload.NumeroSerie : payload.NumeroSerie?.Substring(0, 29);
            }
            #endregion


            var barCodeWritter = new BarcodeWriter();
            barCodeWritter.Format = BarcodeFormat.CODE_128;
            barCodeWritter.Options.Height = 25;
            barCodeWritter.Options.Width = 130;
            barCodeWritter.Options.PureBarcode = true;

            using (MemoryStream memoryStream = new MemoryStream())
            {
                var barCodeData = barCodeService.GetPayload(payload);
                barCodeWritter.Write(barCodeData).Save(memoryStream, ImageFormat.Jpeg);
                payload.BarCodeImage = "data:image/png;base64," + Convert.ToBase64String(memoryStream.ToArray());
            }

            return payload;
        }

        public ProductLocalization CreateFrameStock(int productId, int locationId, string numeroSerie, string marque)
        {
            try
            {
                ProductLocalization stock = new ProductLocalization()
                {
                    ProductID = productId,
                    LocalizationID = locationId,
                    NumeroSerie = numeroSerie,
                    Marque = marque,
                    ProductLocalizationDate = (DateTime)Session["BusnessDayDate"]
                };
                db.ProductLocalizations.Add(stock);
                db.SaveChanges();
                stock.Product = db.Products.Find(stock.ProductID);
                stock.Localization = db.Localizations.Find(stock.LocalizationID);

                return stock;
            }
            catch (Exception e)
            {
                throw e;
            }
        }


    }
}