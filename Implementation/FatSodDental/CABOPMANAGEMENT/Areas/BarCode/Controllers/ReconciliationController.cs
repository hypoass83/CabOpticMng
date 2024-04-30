using FatSod.Ressources;
using FatSod.Security.Abstracts;
using FatSod.Security.Entities;
using FatSod.Supply.Abstracts;
using FatSod.Supply.Abstracts.BarCode;
using FatSod.Supply.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CABOPMANAGEMENT.Controllers;

namespace CABOPMANAGEMENT.Areas.BarCode.Controllers
{
    public class ReconciliationController : BaseController
    {

        private readonly IBusinessDay busDayRepo;
        private readonly IBarCodeService barCodeService;
        private readonly IInventoryDirectory inventoryDirectoryRepository;
        private List<BusinessDay> bdDay;

        public ReconciliationController(IBusinessDay busDayRepo, IBarCodeService barCodeService, IInventoryDirectory inventoryDirectoryRepository)
        {
            this.busDayRepo = busDayRepo;
            this.barCodeService = barCodeService;
            this.inventoryDirectoryRepository = inventoryDirectoryRepository;
        }

        // GET: BarCode/Conciliation
        public ActionResult Index()
        {
            bdDay = (List<BusinessDay>)Session["UserBusDays"];
            if (bdDay == null)
            {
                bdDay = busDayRepo.GetOpenedBusinessDay(CurrentUser);
            }

            if (bdDay.Count() > 1)
            {
                ViewBag.Disabled = false;
            }
            DateTime busDays = bdDay.FirstOrDefault().BDDateOperation;

            ViewBag.CurrentBranch = bdDay.FirstOrDefault().BranchID;
            ViewBag.BusnessDayDate = busDays.ToString("yyyy-MM-dd");
            ViewBag.Reference = GetLastInventoryCounting()?.Reference;
            Session["BusnessDayDate"] = busDays;
            Session["inventoryCountingLines"] = null;

            return View();
        }

        public JsonResult GetExistingInventoryCountingLines(string reference)
        {
            var lines = db.InventoryCountingLines.Where(l => l.InventoryCounting.Reference == reference)
                .GroupBy(l1 => l1.StockId)
                .Select(
                    group => new
                    {
                        StockId = group.Key,
                        Marque = group.FirstOrDefault().Stock.Marque,
                        NumeroSerie = group.FirstOrDefault().Stock.NumeroSerie,
                        Barcode = group.FirstOrDefault().Stock.BarCode,
                        ProductLabel = group.FirstOrDefault().Stock.Product.ProductLabel,
                        CountedQuantity = group.Sum(l3 => l3.CountedQuantity),
                        StockQuantity = group.FirstOrDefault().Stock.ProductLocalizationStockQuantity,
                        Stock = group.FirstOrDefault().Stock
                    }
                ).ToList();

            var data = new List<object>();
            foreach (var line in lines)
            {
                var Input = GetInventoryPeriodInput(line.Stock, reference);
                var Output = GetInventoryPeriodOutput(line.Stock, reference);
                var physicalQuantity = line.CountedQuantity + Input - Output;
                var Shortage = (line.StockQuantity > physicalQuantity) ? (line.StockQuantity - physicalQuantity) : 0;
                var Surplus = (line.StockQuantity < physicalQuantity) ? (physicalQuantity - line.StockQuantity) : 0;

                //if (Shortage < 0 && Surplus == 0)
                //{
                //    Surplus = -Shortage;
                //    Shortage = 0;
                //}

                //if (Surplus < 0 && Shortage == 0)
                //{
                //    Shortage = -Surplus;
                //    Surplus = 0;
                //}

                data.Add(new
                {
                    Shortage,
                    Surplus,
                    StockId = line.StockId,
                    Marque = line.Marque,
                    NumeroSerie = line.NumeroSerie,
                    Barcode = line.Barcode,
                    ProductLabel = line.ProductLabel,
                    StockQuantity = line.StockQuantity,
                    CountedQuantity = line.CountedQuantity,
                    ReconciliationQuantity = physicalQuantity,
                    Input,
                    Output,
                    ReconciliationComment = line.StockQuantity == physicalQuantity ? "RAS" : ""
                });
            }
            var model = new { data };
            /*var model = new
            {
                data = from line in lines
                       select new
                       {
                           StockId = line.StockId,
                           Marque = line.Marque,
                           NumeroSerie = line.NumeroSerie,
                           Barcode = line.Barcode,
                           ProductLabel = line.ProductLabel,
                           StockQuantity = line.StockQuantity,
                           CountedQuantity = line.CountedQuantity,
                           Shortage = (line.StockQuantity > line.CountedQuantity) ? (line.StockQuantity - line.CountedQuantity) : 0,
                           Surplus = (line.StockQuantity < line.CountedQuantity) ? (line.CountedQuantity - line.StockQuantity) : 0,
                           ReconciliationQuantity = line.CountedQuantity,
                           Input = GetInventoryPeriodInput(line.Stock,reference),
                           Output = GetInventoryPeriodOutput(line.Stock, reference),
                           ReconciliationComment = line.StockQuantity == line.CountedQuantity ? "RAS" : ""
                       }
            };*/
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        private double GetInventoryPeriodOutput(ProductLocalization stock, string reference)
        {
            var res = 0d;

            var directory = db.InventoryCountings.SingleOrDefault(d => d.Reference == reference);
            var startedDate = directory.CreatedDate;
            var endedDate = ((DateTime)Session["BusnessDayDate"]);

            var outputs = db.InventoryHistorics.Where(hist => hist.ProductID == stock.ProductID &&
                                                              hist.LocalizationID == stock.LocalizationID &&
                                                              hist.Marque == stock.Marque &&
                                                              hist.NumeroSerie == stock.NumeroSerie &
                                                              (hist.InventoryDate >= startedDate.Date && hist.InventoryDate <= endedDate.Date) &&
                                                              hist.StockStatus.Trim().ToUpper() == "OUTPUT"
                                                              ).ToList();


            if (outputs != null && outputs.Count > 0)
                res = outputs.Sum(o => o.Quantity);

            return res;
        }

        private double GetInventoryPeriodInput(ProductLocalization stock, string reference)
        {
            var res = 0d;

            var directory = db.InventoryCountings.SingleOrDefault(d => d.Reference == reference);
            var startedDate = directory.CreatedDate;
            var endedDate = ((DateTime)Session["BusnessDayDate"]);

            var inputs = db.InventoryHistorics.Where(hist => hist.ProductID == stock.ProductID &&
                                                              hist.LocalizationID == stock.LocalizationID &&
                                                              hist.Marque == stock.Marque &&
                                                              hist.NumeroSerie == stock.NumeroSerie &
                                                              (hist.InventoryDate >= startedDate.Date && hist.InventoryDate <= endedDate.Date) &&
                                                              hist.StockStatus.Trim().ToUpper() == "INPUT"
                                                              ).ToList();


            if (inputs != null && inputs.Count > 0)
                res = inputs.Sum(i => i.Quantity);

            return res;
        }

        public InventoryCounting GetLastInventoryCounting()
        {
            // Last created Barcode Inventory
            return db.InventoryCountings.AsNoTracking().Where(b => b.ClosedDate == null).OrderByDescending(bi => bi.InventoryCountingId).FirstOrDefault();
        }

        public JsonResult AddReconciliation(InventoryReconciliation inventoryReconciliation, List<InventoryReconciliationLine> inventoryReconciliationLines)
        {
            bool status = true;
            string Message = Resources.Success + " - " + "Lines have been successfully saved";

            try
            {
                inventoryDirectoryRepository.CreateReconciliation(inventoryReconciliation, inventoryReconciliationLines);
            }
            catch (Exception e)
            {
                status = false;
                Message = "Error " + e.Message;
            }
            return new JsonResult { Data = new { status = status, Message = Message } };
        }
    }
}