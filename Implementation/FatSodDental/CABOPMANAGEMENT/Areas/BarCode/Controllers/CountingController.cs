using AutoMapper;
using FatSod.Ressources;
using FatSod.Security.Abstracts;
using FatSod.Security.Entities;
using FatSod.Supply.Abstracts;
using FatSod.Supply.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CABOPMANAGEMENT.Controllers;
using CABOPMANAGEMENT.Filters;
using FatSod.Supply.Abstracts.BarCode;

namespace CABOPMANAGEMENT.Areas.BarCode.Controllers
{
    public class CountingController : BaseController
    {
        private readonly IBusinessDay busDayRepo;
        private readonly IBarCodeService barCodeService;
        private List<BusinessDay> bdDay;

        public CountingController(IBusinessDay busDayRepo, IBarCodeService barCodeService)
        {
            this.busDayRepo = busDayRepo;
            this.barCodeService = barCodeService;
        }

        // GET: BarCode/Counting
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

        public JsonResult AddInventoryCountingLine(string barcode)
        {
            bool status = true;
            string Message = Resources.Success + " - " + "Line has been successfully added";
            var inventoryCountingLines = (List<InventoryCountingLine>)Session["inventoryCountingLines"];

            try
            {
                if (inventoryCountingLines == null)
                    inventoryCountingLines = new List<InventoryCountingLine>();

                var inventoryCountingLine = GetInventoryCountingLine(barcode);
                inventoryCountingLine.CountedQuantity = 1;

                var existingLine = inventoryCountingLines.SingleOrDefault(l => l.StockId == inventoryCountingLine.StockId);

                if (existingLine != null)
                    existingLine.CountedQuantity += 1;
                else
                    inventoryCountingLines.Add(inventoryCountingLine);

                Session["inventoryCountingLines"] = inventoryCountingLines;
            }
            catch (Exception e)
            {
                status = false;
                Message = "Error " + e.Message;
            }
            return new JsonResult { Data = new { status = status, Message = Message } };
        }

        public JsonResult DeleteInventoryCountingLine(int stockId)
        {
            bool status = true;
            string Message = Resources.Success + " - " + "Line has been successfully removed";
            var inventoryCountingLines = (List<InventoryCountingLine>)Session["inventoryCountingLines"];

            try
            {
                if (inventoryCountingLines == null)
                    inventoryCountingLines = new List<InventoryCountingLine>();

                var existingLine = inventoryCountingLines.SingleOrDefault(l => l.StockId == stockId);

                if (existingLine != null)
                    inventoryCountingLines.Remove(existingLine);

                Session["inventoryCountingLines"] = inventoryCountingLines;
            }
            catch (Exception e)
            {
                status = false;
                Message = "Error " + e.Message;
            }
            return new JsonResult { Data = new { status = status, Message = Message } };
        }


        public JsonResult IncrementLine(int stockId, bool isIncrement)
        {
            bool status = true;
            string Message = Resources.Success + " - " + "Line has been successfully updated";
            var inventoryCountingLines = (List<InventoryCountingLine>)Session["inventoryCountingLines"];

            try
            {
                if (inventoryCountingLines == null)
                    inventoryCountingLines = new List<InventoryCountingLine>();

                var existingLine = inventoryCountingLines.SingleOrDefault(l => l.StockId == stockId);

                if (existingLine != null)
                {
                    if (isIncrement)
                        ++existingLine.CountedQuantity;
                    else
                        --existingLine.CountedQuantity;
                }

                Session["inventoryCountingLines"] = inventoryCountingLines;
            }
            catch (Exception e)
            {
                status = false;
                Message = "Error " + e.Message;
            }
            return new JsonResult { Data = new { status = status, Message = Message } };
        }

        public JsonResult AddInventoryCountingLines(InventoryCountingLine inventoryCounting)
        {
            bool status = true;
            string Message = Resources.Success + " - " + "Lines have been successfully saved";
            try
            {
                var inventoryCountingLines = new List<InventoryCountingLine>((List<InventoryCountingLine>)Session["inventoryCountingLines"]);

                if (inventoryCountingLines == null)
                    throw new Exception("Please Add lines befor proceeed");

                inventoryCountingLines.ForEach(line =>
                {
                    line.Stock = null;
                    line.InventoryCountingId = inventoryCounting.InventoryCountingId;
                    line.AuthorizedById = inventoryCounting.AuthorizedById;
                    line.CountedById = inventoryCounting.CountedById;
                    line.RegisteredById = inventoryCounting.RegisteredById;
                    line.RegistrationDate = inventoryCounting.RegistrationDate;
                });

                db.InventoryCountingLines.AddRange(inventoryCountingLines);
                db.SaveChanges();

                Session["inventoryCountingLines"] = null;

            }
            catch (Exception e)
            {
                status = false;
                Message = "Error " + e.Message;
            }
            return new JsonResult { Data = new { status = status, Message = Message } };
        }

        InventoryCountingLine GetInventoryCountingLine(string barcode)
        {
            var res = new InventoryCountingLine();

            var stock = barCodeService.GetProductLocalization(barcode);
            if (stock == null)
                throw new Exception("There is no product with the given Barcode " + barcode + " please try Again");

            res.Stock = stock;
            res.StockId = stock.ProductLocalizationID;

            return res;
        }

        public JsonResult GetInventoryCounting(string reference)
        {
            List<object> list = new List<object>();
            var inventoryCounting = new InventoryCounting();

            #region Derniere Dossier d inventaire
            if (reference == null || reference.Length == 0)
            {
                inventoryCounting = GetLastInventoryCounting();
            }
            #endregion

            #region Inventaire dont la reference est indiquee
            if (reference != null && reference.Length > 0)
            {
                inventoryCounting = GetInventoryCountingByReference(reference);
            }
            #endregion

            if (inventoryCounting == null)
                inventoryCounting = new InventoryCounting();

            return Json(inventoryCounting, JsonRequestBehavior.AllowGet);
        }

        public InventoryCounting GetLastInventoryCounting()
        {
            // Last created Barcode Inventory
            return db.InventoryCountings.AsNoTracking().Where(b => b.ClosedDate == null).OrderByDescending(bi => bi.InventoryCountingId).FirstOrDefault();
        }

        public InventoryCounting GetInventoryCountingByReference(string reference)
        {
            var res = db.InventoryCountings.SingleOrDefault(bi => bi.Reference == reference);

            return new InventoryCounting()
            {
                Reference = res.Reference,
                Description = res.Description,
                BranchId = res.BranchId,
                AuthorizedById = res.AuthorizedById,
                InventoryCountingId = res.InventoryCountingId,
            };
        }

        // Barcode Inventory Lines venant de la base de donnees
        public JsonResult GetExistingInventoryCountingLines(int inventoryCountingId)
        {
            var lines = db.InventoryCountingLines.Where(l => l.InventoryCountingId == inventoryCountingId).OrderBy(l => l.InventoryCountingLineId).ToList();
            var model = new
            {
                data = from line in lines
                       select new
                       {
                           InventoryCountingLineId = line.InventoryCountingLineId,
                           Marque = line.Stock.Marque,
                           NumeroSerie = line.Stock.NumeroSerie,
                           Barcode = line.Stock.BarCode,
                           ProductLabel = line.Stock.Product.ProductLabel,
                           CountedQuantity = line.CountedQuantity,
                           AuthorizedBy = line.AuthorizedBy.UserFullName,
                           CountedBy = line.CountedBy.UserFullName,
                           Registeredy = line.RegisteredBy.UserFullName,
                           RegistrationDate = line.RegistrationDate.ToString("dd-MM-yyyy")
                       }
            };
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteExistingInventoryCountingLine(int inventoryCountingLineId)
        {
            bool status = true;
            string Message = Resources.Success + " - " + "Line has been successfully removed";

            try
            {
                var existingLine = db.InventoryCountingLines.SingleOrDefault(l => l.InventoryCountingLineId == inventoryCountingLineId);
                db.InventoryCountingLines.Remove(existingLine);
                db.SaveChanges();
               
            }
            catch (Exception e)
            {
                status = false;
                Message = "Error " + e.Message;
            }
            return new JsonResult { Data = new { status = status, Message = Message } };
        }

        // Barcode Inventory Line Venant de la liste en cours de creations
        public JsonResult GetInventoryCountingLines()
        {
            var inventoryCountingLines = (List<InventoryCountingLine>)Session["inventoryCountingLines"];

            inventoryCountingLines = inventoryCountingLines != null ? inventoryCountingLines : new List<InventoryCountingLine>();

            var model = new
            {
                data = from line in inventoryCountingLines
                       select new
                       {
                           InventoryCountingLineId = line.Stock.ProductLocalizationID,
                           Marque = line.Stock.Marque,
                           NumeroSerie = line.Stock.NumeroSerie,
                           ProductLabel = line.Stock.Product.ProductLabel,
                           CountedQuantity = line.CountedQuantity,
                           Barcode = line.Stock.BarCode
                       }
            };
            return Json(model, JsonRequestBehavior.AllowGet);
        }


    }
}