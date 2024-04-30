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

namespace CABOPMANAGEMENT.Areas.BarCode.Controllers
{
    [Authorize(Order = 1)]
    [TakeBusinessDay(Order = 2)]
    public class DirectoryController : BaseController
    {
        private List<BusinessDay> bdDay;
        private IBusinessDay _busDayRepo;
        private ITransactNumber _transactNumbeRepository;

        public DirectoryController(
            IBusinessDay busDayRepo,
            ITransactNumber transactNumbeRepository
            )
        {
            this._busDayRepo = busDayRepo;
            this._transactNumbeRepository = transactNumbeRepository;
        }

        // GET: BarCode/Directory
        public ActionResult Index()
        {
            bdDay = (List<BusinessDay>)Session["UserBusDays"];
            if (bdDay == null)
            {
                bdDay = _busDayRepo.GetOpenedBusinessDay(CurrentUser);
            }

            if (bdDay.Count() > 1)
            {
                ViewBag.Disabled = false;
            }
            DateTime busDays = bdDay.FirstOrDefault().BDDateOperation;

            ViewBag.CurrentBranch = bdDay.FirstOrDefault().BranchID;
            ViewBag.BusnessDayDate = busDays.ToString("yyyy-MM-dd");
            Session["BusnessDayDate"] = busDays;
            Session["ConnectedUserId"] = CurrentUser.GlobalPersonID;

            return View();
        }

        public JsonResult AddInventoryCounting(InventoryCounting inventoryCounting)
        {
            bool status = true;
            string Message = Resources.Success + " - " + "Inventory Directory has been successfully created";
            try
            {
                if(inventoryCounting.InventoryCountingId == 0)
                {
                    db.InventoryCountings.Add(inventoryCounting);
                }

                if (inventoryCounting.InventoryCountingId > 0)
                {
                    var existingDirectory = db.InventoryCountings.Find(inventoryCounting.InventoryCountingId);
                    existingDirectory.BranchId = inventoryCounting.BranchId;
                    existingDirectory.AuthorizedById = inventoryCounting.AuthorizedById;
                    existingDirectory.Description = inventoryCounting.Description;
                }

                db.SaveChanges();
            }
            catch (Exception e)
            {
                status = false;
                Message = "Error " + e.Message + " " + e.StackTrace + " " + e.InnerException;
            }
            return new JsonResult { Data = new { status = status, Message = Message } };
        }

        public JsonResult DeleteInventoryCounting(int inventoryCountingId)
        {
            bool status = true;
            string Message = Resources.Success + " - " + "Inventory Directory has been successfully deleted";
            try
            {
                var inventoryCounting = db.InventoryCountings.Find(inventoryCountingId);
                if (inventoryCounting?.InventoryCountingLines == null || inventoryCounting?.InventoryCountingLines?.Count == 0)
                {
                    db.InventoryCountings.Remove(inventoryCounting);
                    db.SaveChanges();
                } else
                {
                    status = false;
                    Message = "You can not delte because it contains informations";
                }
                
            }
            catch (Exception e)
            {
                status = false;
                Message = "Error " + e.Message + " " + e.StackTrace + " " + e.InnerException;
            }
            return new JsonResult { Data = new { status = status, Message = Message } };
        }

        public JsonResult UpdateInventoryCounting(InventoryCounting inventoryCounting)
        {
            bool status = true;
            string Message = Resources.Success + " - " + statusOperation;
            try
            {
                Mapper.CreateMap<InventoryCounting, InventoryCounting>();

                var existingInventoryCounting = db.InventoryCountings.Find(inventoryCounting.InventoryCountingId);

                //use Map
                existingInventoryCounting = Mapper.Map<InventoryCounting>(inventoryCounting);

                db.SaveChanges();

            }
            catch (Exception e)
            {
                status = false;
                Message = "Error " + e.Message + " " + e.StackTrace + " " + e.InnerException;
            }
            return new JsonResult { Data = new { status = status, Message = Message } };
        }

        public JsonResult OpenedBusday()
        {
            List<object> list = new List<object>();

            bdDay = (List<BusinessDay>)Session["UserBusDays"];
            if (bdDay == null)
            {
                bdDay = _busDayRepo.GetOpenedBusinessDay(CurrentUser);
            };

            foreach (BusinessDay busDay in bdDay)
            {
                list.Add(
                    new
                    {
                        BranchID = busDay.BranchID,
                        BranchName = busDay.Branch.BranchCode
                    }
                    );
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult populateUsers()
        {
            List<object> userList = new List<object>();
            var users = db.People.OfType<User>().Where(u => u.IsConnected && u.ProfileID > 2 && u.UserAccessLevel > 1);
            foreach (var user in users)
            {
                userList.Add(new
                {
                    UserFullName = user.UserFullName,
                    GlobalPersonID = user.GlobalPersonID
                });
            }
            return Json(userList, JsonRequestBehavior.AllowGet);

        }

        public JsonResult ChangeBusDay(int? BranchID)
        {
            List<object> list = new List<object>();

            if (BranchID.HasValue && BranchID.Value > 0)
            {
                bdDay = (List<BusinessDay>)Session["UserBusDays"];
                if (bdDay == null)
                {
                    bdDay = _busDayRepo.GetOpenedBusinessDay(CurrentUser);
                };

                BusinessDay businessDay = bdDay.FirstOrDefault(b => b.BranchID == BranchID.Value);
                string trnnum = _transactNumbeRepository.returnTransactNumber("INVE", businessDay);
                list.Add(
                new
                {
                    InventoryDirectoryCreationDate = businessDay.BDDateOperation.ToString("yyyy-MM-dd"),
                    InventoryDirectoryReference = trnnum
                }
                );
            }

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAllBarcodeInventories()
        {
            var directories = db.InventoryCountings.AsNoTracking().ToList();
            var model = new
            {
                data = from directory in directories
                       select new
                       {
                           InventoryCountingId = directory.InventoryCountingId,
                           BranchName = directory.Branch.BranchName,
                           Reference = directory.Reference,
                           Description = directory.Description,
                           CreatedDate = directory.CreatedDate.ToString("dd-MM-yyyy"),
                           IsOpened = directory.ClosedDate == null
                       }
            };
            return Json(model, JsonRequestBehavior.AllowGet);
        }

    }
}