using FatSod.DataContext.Repositories;
using FatSod.Report.WrapReports;
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

namespace CABOPMANAGEMENT.Areas.Supply.Controllers
{
    [Authorize(Order = 1)]
    [TakeBusinessDay(Order = 2)]
    public class ListOfStockInputController : BaseController
    {

        //Current Controller and current page
        private const string CONTROLLER_NAME = "Supply/ListOfStockInput";
        private const string VIEW_NAME = "Index";

        private IInventoryDirectory _inventoryDirectoryRepository;

        public ListOfStockInputController(
                 IInventoryDirectory inventoryDirectoryRepository
                )
        {
            this._inventoryDirectoryRepository = inventoryDirectoryRepository;
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



        public JsonResult ModelInventoryDirectory(int BranchID, DateTime Bdate, DateTime Edate)
        {

            List<InventoryDirectory> list = _inventoryDirectoryRepository.FindAll.Where(id => /*(id.InventoryDirectoryStatut == InventoryDirectorySatut.Closed) &&*/ id.BranchID == BranchID &&
                                                                                                  (id.InventoryDirectoryDate >= Bdate.Date && id.InventoryDirectoryDate <= Edate.Date)).ToList();

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

        //This method print a receipt of customer
        public ActionResult GenerateReceipt(int InventoryDirectoryID)
        {

            List<RptInventoryDirectory> model = new List<RptInventoryDirectory>();

            try
            {


                string defaultImgPath = Server.MapPath("~/Content/Images/App/deboy-logo.png");
                byte[] COMPANY_LOGO = System.IO.File.ReadAllBytes(defaultImgPath);

                @ViewBag.CompanyLogoID = Company.GlobalPersonID;

                string DeviseLabel = db.Devises.Where(d => d.DefaultDevise).FirstOrDefault().DeviseLabel;

                InventoryDirectory invDirect = (from pt in db.InventoryDirectories
                                                where pt.InventoryDirectoryID == InventoryDirectoryID
                                                select pt).SingleOrDefault();

                var curBranch = db.UserBranches
                .Where(br => br.UserID == SessionGlobalPersonID)
                .ToList()
                .Select(s => new UserBranch
                {
                    BranchID = s.BranchID,
                    Branch = s.Branch
                })
                .AsQueryable()
                .FirstOrDefault();

                int i = 0;

                db.InventoryDirectoryLines.Where(l => l.InventoryDirectoryID == InventoryDirectoryID).ToList().ForEach(c =>
                {
                    i = i + 1;
                    model.Add(
                            new RptInventoryDirectory
                            {
                                RptInventoryDirectoryID = i,
                                AveragePurchasePrice = c.AveragePurchasePrice,
                                NewStockQuantity = c.NewStockQuantity.Value,
                                OldStockQuantity = c.OldStockQuantity,
                                StockDifference = (c.NewStockQuantity.Value + c.OldStockQuantity),
                                ProductLabel = c.Product.ProductLabel,
                                ProductRef = (c.Product is GenericProduct) ? c.Product.ProductCode + " Ref:"+ c.NumeroSerie + " Marque:"+c.Marque : c.Product.ProductCode,
                                CompanyName = Company.Name,
                                CompanyAdress = "Head Quater : " + Company.Adress.Quarter.Town.Region.RegionLabel + " - " + Company.Adress.Quarter.Town.TownLabel,
                                CompanyTel = "Tel: " + Company.Adress.AdressPhoneNumber,
                                BranchName = curBranch.Branch.BranchName,
                                BranchAdress = curBranch.Branch.Adress.Quarter.QuarterLabel + " - " + curBranch.Branch.Adress.Quarter.Town.TownLabel,
                                BranchTel = "Tel: " + curBranch.Branch.Adress.AdressPhoneNumber,
                                Ref = invDirect.InventoryDirectoryReference,
                                CompanyCNI = "NO CONT : " + Company.CNI,
                                Operator = c.RegisteredBy?.FullName,
                                TransfertDate = invDirect.InventoryDirectoryCreationDate.Date,
                                Title = "Stock Input lines informations",
                                DeviseLabel = DeviseLabel,
                            });
                });


                return View(model);
            }
            catch (Exception ex)
            {
                throw;
            }


        }


    }
}