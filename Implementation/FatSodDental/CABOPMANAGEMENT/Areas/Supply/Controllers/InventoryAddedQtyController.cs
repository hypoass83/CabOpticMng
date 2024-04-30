using FatSod.DataContext.Initializer;
using FatSod.Security.Abstracts;
using FatSod.Security.Entities;
using FatSod.Supply.Entities;
using CABOPMANAGEMENT.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using FatSod.Supply.Abstracts;
using CABOPMANAGEMENT.Filters;
using System.IO;
using CABOPMANAGEMENT.Tools;
using CABOPMANAGEMENT.Areas.Supply.Models;

namespace CABOPMANAGEMENT.Areas.Supply.Controllers
{
    [Authorize(Order = 1)]
    [TakeBusinessDay(Order = 2)]
    public class InventoryAddedQtyController : BaseController
    {
        //Current Controller and current page
        private const string CONTROLLER_NAME = "Supply/" + CodeValue.Supply.Inventory_SM.CONTROLLER;
        private const string VIEW_NAME = CodeValue.Supply.Inventory_SM.PATH;
        //person repository
        private IProductLocalization _plRepository;
        private IBusinessDay _busDayRepo;

        private IRepository<FatSod.Security.Entities.File> _fileRepository;
        private ILensNumberRangePrice _priceRepository;

        //Construcitor
        public InventoryAddedQtyController(
            ILensNumberRangePrice lnrpRepo,
            IBusinessDay busDayRepo,
            IProductLocalization plRepository,
            IRepository<FatSod.Security.Entities.File> fileRepository
            )
        {
            this._plRepository = plRepository;
            this._busDayRepo = busDayRepo;
            this._fileRepository = fileRepository;
            this._priceRepository = lnrpRepo;
        }
        // GET: Sale/Sale Return all a sales historique add enable to create a new sale
        [OutputCache(Duration = 3600)]
        public ActionResult Index()
        {
            return View(ModelPL(0, 0));
        }

        /*** methodes lors du chargement du formulaire*********************/
        public JsonResult GetCategories()
        {

            List<object> categoryList = new List<object>();
            List<Category> categories = LoadComponent.GetAllGenericCategoryItems();// .GetAllGenericCategories();
            foreach (Category cat in categories)
            {
                categoryList.Add(new
                {
                    CategoryID = cat.CategoryID,
                    Name = cat.CategoryLabel
                });
            }

            return Json(categoryList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetStores()
        {
            List<object> storesList = new List<object>();
            List<Localization> stores = db.Localizations.ToList();
            foreach (Localization loc in stores)
            {
                storesList.Add(new
                {
                    ID = loc.LocalizationID,
                    Name = loc.LocalizationCode
                });
            }

            return Json(storesList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult EditLineSaisi(List<InventoryLines> inventoryLines) //int ProductLocalizationID, int ProductLocalizationStockQuantity, int ProductLocalizationSafetyStockQuantity)
        {

            bool status = false;
            string Message = "";

            try
            {
                foreach (InventoryLines inLines in inventoryLines)
                {
                    if (inLines.AddedQuantity != 0)
                    {
                        ProductLocalization prodLoc = new ProductLocalization();
                        prodLoc = _plRepository.Find(inLines.ProductLocalizationID);
                        DateTime serverdate = SessionBusinessDay(null).BDDateOperation;
                        if (prodLoc.ProductLocalizationID > 0)
                        {
                            //prodLoc.ProductLocalizationSafetyStockQuantity = Convert.ToDouble(inLines.ProductLocalizationSafetyStockQuantity);
                            //prodLoc.ProductLocalizationStockQuantity = Convert.ToDouble(ProductLocalizationStockQuantity);*/
                            prodLoc.inventoryReason = "Stock Inventory";
                            prodLoc.RegisteredByID = SessionGlobalPersonID;
                            prodLoc.Description = "Stock Inventory";
                            _plRepository.UpdateProductLocalizationAddedQty(prodLoc, serverdate, inLines.AddedQuantity);
                        }
                    }
                }


                status = true;
                Message = "Updated rows successfully!";
                return new JsonResult { Data = new { status = status, Message = Message } };
            }
            catch (Exception e)
            {
                status = false;
                Message = @"Une erreur s'est produite lors de l'opération, veuillez contactez l'administrateur et/ou essayez à nouveau
                                    <br/>Code : <code>" + e.Message + " car " + e.StackTrace + "</code>";
                return new JsonResult { Data = new { status = status, Message = Message } };
            }
        }

       
        


        public JsonResult InitializePurchaseFields(int ID)
        {
            List<object> list = new List<object>();
            if (ID > 0)
            {
                ProductLocalization prodLoc = new ProductLocalization();
                prodLoc = db.ProductLocalizations.Find(ID);

                BusinessDay businessDay = _busDayRepo.GetOpenedBusinessDay(db.Branches.Find(prodLoc.Localization.BranchID));

                list.Add(new
                {
                    AveragePurchasePrice = prodLoc.AveragePurchasePrice,
                    ProductLocalizationStockSellingPrice = prodLoc.ProductLocalizationStockSellingPrice,
                    OldQuantityID = prodLoc.ProductLocalizationStockQuantity,
                    ProductLocalizationStockQuantity = prodLoc.ProductLocalizationStockQuantity,
                    ProductLocalizationSafetyStockQuantity = prodLoc.ProductLocalizationSafetyStockQuantity,
                    BranchID = prodLoc.Localization.Branch.BranchName,
                    Localization = prodLoc.LocalizationLabel,
                    Product = (prodLoc.Product is Lens) ? prodLoc.Product.ProductCode : prodLoc.ProductCode,
                    ProductID = prodLoc.ProductID,
                    ProductLocalizationID = prodLoc.ProductLocalizationID,
                    ProductLocalizationDate = businessDay.BDDateOperation.ToString("yyyy-MM-dd"),
                    Marque = prodLoc.Marque,
                    NumeroSerie = prodLoc.NumeroSerie,
                    AddedQuantity=0
                });
            }

            return Json(list, JsonRequestBehavior.AllowGet);

        }


        public JsonResult OpenedBusday()
        {
            List<object> list = new List<object>();
            List<BusinessDay> busDays = _busDayRepo.GetOpenedBusinessDay(CurrentUser);

            foreach (BusinessDay busDay in busDays)
            {
                list.Add(
                    new
                    {
                        BranchID = busDay.BranchID,
                        BranchName = busDay.BranchName
                    }
                    );
            }

            return Json(list, JsonRequestBehavior.AllowGet);

        }
        private List<ProductLocalization> ModelPL(int CategoryID, int Stores)
        {


            List<ProductLocalization> dataTmp = new List<ProductLocalization>();

            if (CategoryID > 0 && Stores > 0)
            {
                dataTmp = (from ls in db.ProductLocalizations
                           where ls.Product.CategoryID == CategoryID && ls.LocalizationID == Stores
                           select ls)
                            .OrderBy(a => a.Product.ProductCode)
                            .ToList();
            }

            /*dataTmp = (from prodloc in db.ProductLocalizations
                        select prodloc)
                        .OrderByDescending(a => a.Product.ProductCode)
                        //.Take(1000)
                        .ToList();*/


            List<ProductLocalization> realDataTmp = new List<ProductLocalization>();

            foreach (ProductLocalization p in dataTmp)
            {

                realDataTmp.Add(
                    new ProductLocalization
                    {
                        ProductLocalizationID = p.ProductLocalizationID,
                        Localization = p.Localization,
                        Product = p.Product,
                        ProductLocalizationStockQuantity = p.ProductLocalizationStockQuantity,
                        ProductLocalizationStockSellingPrice = p.ProductLocalizationStockSellingPrice,
                        ProductLocalizationSafetyStockQuantity = p.ProductLocalizationSafetyStockQuantity,
                        AveragePurchasePrice = p.AveragePurchasePrice,
                        Marque = p.Marque,
                        AddedQuantity = p.AddedQuantity,
                        NumeroSerie = p.NumeroSerie
                    });
            }

            return realDataTmp;
        }

        private void ModelAllPLReport()
        {

            var dataTmp = db.ProductLocalizations
                        .Select(s => new
                        {
                            ProductLocalizationID = s.ProductLocalizationID,
                            LocalizationLabel = s.Localization.LocalizationLabel,
                            ProductCode = s.Product.ProductCode,
                            ProductLocalizationStockQuantity = s.ProductLocalizationStockQuantity,
                            ProductLocalizationStockSellingPrice = s.ProductLocalizationStockSellingPrice,
                            ProductLocalizationSafetyStockQuantity = s.ProductLocalizationSafetyStockQuantity,
                            //Amount = s.Amount,
                            BranchName = s.Localization.Branch.BranchName,
                            AveragePurchasePrice = s.AveragePurchasePrice,
                            Marque = s.Marque,
                            NumeroSerie = s.NumeroSerie
                        }).ToList();

            //Session["rptSource"] = dataTmp;
            List<ProductLocalization> realDataTmp = new List<ProductLocalization>();
            Session["rptSource"] = realDataTmp;


        }
        

        public JsonResult GetAllProductLocalizations(int CategoryID, int Stores)
        {
            var model = new
            {
                data = from s in ModelPL(CategoryID, Stores)
                       select new
                       {
                           ProductLocalizationID = s.ProductLocalizationID,
                           LocalizationLabel = s.Localization.LocalizationLabel,
                           ProductCode = s.Product.ProductCode,
                           ProductLocalizationStockQuantity = s.ProductLocalizationStockQuantity,
                           ProductLocalizationStockSellingPrice = s.ProductLocalizationStockSellingPrice,
                           ProductLocalizationSafetyStockQuantity = s.ProductLocalizationSafetyStockQuantity,
                           Amount = s.Amount,
                           BranchName = s.Localization.Branch.BranchName,
                           AveragePurchasePrice = s.AveragePurchasePrice,
                           Category = s.Product.Category.CategoryLabel,
                           Marque = s.Marque,
                           AddedQuantity = s.AddedQuantity,
                           NumeroSerie = s.NumeroSerie
                       }
            };

            var jsonResult = Json(model, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        
        //This method print inentory of day
        public ActionResult GenerateInventoryAllReport()
        {
            List<object> model = new List<object>();
            //ReportDocument rptH = new ReportDocument();
            string defaultImgPath = Server.MapPath("~/Content/Images/App/deboy-logo.png");
            byte[] COMPANY_LOGO = System.IO.File.ReadAllBytes(defaultImgPath);

            var dataTmp = db.ProductLocalizations
                        .Select(s => new
                        {
                            ProductLocalizationID = s.ProductLocalizationID,
                            Localization = s.Localization,
                            Product = s.Product,
                            ProductLocalizationStockQuantity = s.ProductLocalizationStockQuantity,
                            ProductLocalizationStockSellingPrice = s.ProductLocalizationStockSellingPrice,
                            ProductLocalizationSafetyStockQuantity = s.ProductLocalizationSafetyStockQuantity,
                            //Amount = s.Amount,
                            BranchName = s.Localization.Branch.BranchName,
                            AveragePurchasePrice = s.AveragePurchasePrice,
                            Marque = s.Marque,
                            NumeroSerie = s.NumeroSerie
                        }).ToList();

            string strOperator1 = CurrentUser.Name;

            foreach (var p in dataTmp.OrderBy(c => c.Product.ProductCode))
            {

                model.Add(
                    new
                    {
                        Localization = p.Localization.LocalizationLabel,
                        ProductLabel = p.Product.ProductCode,
                        ProductQty = p.ProductLocalizationStockQuantity,
                        ProductUnitPrice = p.ProductLocalizationStockSellingPrice,
                        ProductLocalizationSafetyStockQuantity = p.ProductLocalizationSafetyStockQuantity,
                        //Amount = p.Amount,
                        BranchName = p.Localization.Branch.BranchName,
                        //IsSafQuantStockReached = p.IsSafQuantStockReached,
                        BranchAdress = p.Localization.Branch.Adress.AdressEmail + "/" + p.Localization.Branch.Adress.AdressPOBox,
                        BranchTel = p.Localization.Branch.Adress.AdressPhoneNumber,
                        CompanyName = Company.Name,
                        CompanyAdress = Company.Adress.AdressEmail + "/" + Company.Adress.AdressPOBox,
                        CompanyTel = Company.Adress.AdressPhoneNumber,
                        CompanyCNI = Company.CNI,
                        CompanyLogo = _fileRepository.FindAll.FirstOrDefault(f => f.GlobalPersonID == Company.GlobalPersonID) != null ? _fileRepository.FindAll.FirstOrDefault(f => f.GlobalPersonID == Company.GlobalPersonID).Content : COMPANY_LOGO,
                        Marque = p.Marque,
                        NumeroSerie = p.NumeroSerie
                    }
                    );
            }

            return View(model);
        }

        public JsonResult InitialiseStock(int CategoryID, int Stores)
        {
            bool status = false;
            string Message = "";

            try
            {
                if (CategoryID > 0 && Stores > 0)
                {
                    _plRepository.InitialiseStock(CategoryID, Stores, SessionBusinessDay(null).BDDateOperation, SessionGlobalPersonID, SessionBusinessDay(null).BranchID);
                }
                status = true;
                Message = "Initialise Stock successfully!";
                return new JsonResult { Data = new { status = status, Message = Message } };
            }
            catch (Exception e)
            {
                status = false;
                Message = @"Une erreur s'est produite lors de l'opération, veuillez contactez l'administrateur et/ou essayez à nouveau
                                    <br/>Code : <code>" + e.Message + " car " + e.StackTrace + "</code>";
                return new JsonResult { Data = new { status = status, Message = Message } };
            }
        }

        //This method print inentory of day
        public ActionResult GenerateGenericInventoryReport()
        {
            List<object> model = new List<object>();
            //ReportDocument rptH = new ReportDocument();
            string defaultImgPath = Server.MapPath("~/Content/Images/App/deboy-logo.png");
            byte[] COMPANY_LOGO = System.IO.File.ReadAllBytes(defaultImgPath);


            var dataTmp = db.ProductLocalizations.Join(db.GenericProducts, pl => pl.ProductID, p => p.ProductID,
                        (pl, p) => new { pl, p })
                        .Where(pg => !pg.p.ProductCode.Contains("RECAP".ToUpper()))
                        .Select(s => new
                        {
                            ProductLocalizationID = s.pl.ProductLocalizationID,
                            Localization = s.pl.Localization,
                            Product = s.pl.Product,
                            ProductLocalizationStockQuantity = s.pl.ProductLocalizationStockQuantity,
                            ProductLocalizationStockSellingPrice = s.pl.ProductLocalizationStockSellingPrice,
                            ProductLocalizationSafetyStockQuantity = s.pl.ProductLocalizationSafetyStockQuantity,
                            //Amount = s.pl.Amount,
                            BranchName = s.pl.Localization.Branch.BranchName,
                            AveragePurchasePrice = s.pl.AveragePurchasePrice,
                            Marque = s.pl.Marque,
                            NumeroSerie = s.pl.NumeroSerie
                        }).ToList();
            string strOperator1 = CurrentUser.Name;
            //List<object> realDataTmp = new List<object>();
            //int companyID = CurrentUser.UserBranches.First().Branch.CompanyID;
            foreach (var p in dataTmp.OrderBy(c => c.Product.ProductCode))
            {

                model.Add(
                    new
                    {
                        Localization = p.Localization.LocalizationLabel,
                        ProductLabel = p.Product.ProductCode,
                        ProductQty = p.ProductLocalizationStockQuantity,
                        ProductUnitPrice = p.ProductLocalizationStockSellingPrice,
                        ProductLocalizationSafetyStockQuantity = p.ProductLocalizationSafetyStockQuantity,
                        //Amount = p.Amount,
                        BranchName = p.Localization.Branch.BranchName,
                        //IsSafQuantStockReached = p.IsSafQuantStockReached,
                        BranchAdress = p.Localization.Branch.Adress.AdressEmail + "/" + p.Localization.Branch.Adress.AdressPOBox,
                        BranchTel = p.Localization.Branch.Adress.AdressPhoneNumber,
                        CompanyName = Company.Name,
                        CompanyAdress = Company.Adress.AdressEmail + "/" + Company.Adress.AdressPOBox,
                        CompanyTel = Company.Adress.AdressPhoneNumber,
                        CompanyCNI = Company.CNI,
                        CompanyLogo = _fileRepository.FindAll.FirstOrDefault(f => f.GlobalPersonID == Company.GlobalPersonID) != null ? _fileRepository.FindAll.FirstOrDefault(f => f.GlobalPersonID == Company.GlobalPersonID).Content : COMPANY_LOGO,
                        Marque = p.Marque,
                        NumeroSerie = p.NumeroSerie
                    }
                    );
            }

            return View(model);
        }

        //This method print inventory 
        public ActionResult GenerateLensInventoryReport()
        {
            List<object> model = new List<object>();
            //ReportDocument rptH = new ReportDocument();
            string defaultImgPath = Server.MapPath("~/Content/Images/App/deboy-logo.png");
            byte[] COMPANY_LOGO = System.IO.File.ReadAllBytes(defaultImgPath);

            string CategoryCode = (string)Session["CategoryCode"];

            var dataTmp = db.ProductLocalizations.Join(db.Products, pl => pl.ProductID, p => p.ProductID,
                        (pl, p) => new { pl, p })
                        .Where(pll => pll.p.Category.CategoryCode == CategoryCode)
                        .Select(s => new
                        {
                            ProductLocalizationID = s.pl.ProductLocalizationID,
                            Localization = s.pl.Localization,
                            Product = s.pl.Product,
                            ProductLocalizationStockQuantity = s.pl.ProductLocalizationStockQuantity,
                            ProductLocalizationStockSellingPrice = s.pl.ProductLocalizationStockSellingPrice,
                            ProductLocalizationSafetyStockQuantity = s.pl.ProductLocalizationSafetyStockQuantity,
                            //Amount = s.pl.Amount,
                            BranchName = s.pl.Localization.Branch.BranchName,
                            AveragePurchasePrice = s.pl.AveragePurchasePrice,
                            Marque = s.pl.Marque,
                            NumeroSerie = s.pl.NumeroSerie
                        }).ToList();
            string strOperator1 = CurrentUser.Name;
            //List<object> realDataTmp = new List<object>();
            //int companyID = CurrentUser.UserBranches.First().Branch.CompanyID;
            foreach (var p in dataTmp.OrderBy(c => c.Product.ProductCode))
            {
                //recuperation du prix de vente du produit
                LensNumberRangePrice price = _priceRepository.GetPrice(p.Product.ProductID);
                double SellingPrice = (price != null && price.LensNumberRangePriceID > 0) ? price.Price : 0;
                model.Add(
                    new
                    {
                        Localization = p.Localization.LocalizationLabel,
                        ProductLabel = p.Product.ProductCode,
                        ProductQty = p.ProductLocalizationStockQuantity,
                        ProductUnitPrice = SellingPrice, //p.ProductLocalizationStockSellingPrice,
                        ProductLocalizationSafetyStockQuantity = p.ProductLocalizationSafetyStockQuantity,
                        //Amount = p.Amount,
                        BranchName = p.Localization.Branch.BranchName,
                        //IsSafQuantStockReached = p.IsSafQuantStockReached,
                        BranchAdress = p.Localization.Branch.Adress.AdressEmail + "/" + p.Localization.Branch.Adress.AdressPOBox,
                        BranchTel = p.Localization.Branch.Adress.AdressPhoneNumber,
                        CompanyName = Company.Name,
                        CompanyAdress = Company.Adress.AdressEmail + "/" + Company.Adress.AdressPOBox,
                        CompanyTel = Company.Adress.AdressPhoneNumber,
                        CompanyCNI = Company.CNI,
                        CompanyLogo = _fileRepository.FindAll.FirstOrDefault(f => f.GlobalPersonID == Company.GlobalPersonID) != null ? _fileRepository.FindAll.FirstOrDefault(f => f.GlobalPersonID == Company.GlobalPersonID).Content : COMPANY_LOGO,
                        Marque = p.Marque,
                        NumeroSerie = p.NumeroSerie
                    }
                    );
            }

            return View(model);
        }

        //This method print inventory 
        public ActionResult GenerateLensInventoryStockReport()
        {
            List<object> model = new List<object>();
            //ReportDocument rptH = new ReportDocument();
            string defaultImgPath = Server.MapPath("~/Content/Images/App/deboy-logo.png");
            byte[] COMPANY_LOGO = System.IO.File.ReadAllBytes(defaultImgPath);

            string CategoryCode = (string)Session["CategoryCode"];

            var dataTmp = db.ProductLocalizations.Join(db.Products, pl => pl.ProductID, p => p.ProductID,
                        (pl, p) => new { pl, p })
                        .Where(pll => pll.p.Category.CategoryCode == CategoryCode && pll.pl.ProductLocalizationStockQuantity > 0)
                        .Select(s => new
                        {
                            ProductLocalizationID = s.pl.ProductLocalizationID,
                            Localization = s.pl.Localization,
                            Product = s.pl.Product,
                            ProductLocalizationStockQuantity = s.pl.ProductLocalizationStockQuantity,
                            ProductLocalizationStockSellingPrice = s.pl.ProductLocalizationStockSellingPrice,
                            ProductLocalizationSafetyStockQuantity = s.pl.ProductLocalizationSafetyStockQuantity,
                            //Amount = s.pl.Amount,
                            BranchName = s.pl.Localization.Branch.BranchName,
                            AveragePurchasePrice = s.pl.AveragePurchasePrice,
                            Marque = s.pl.Marque,
                            NumeroSerie = s.pl.NumeroSerie
                        }).ToList();
            string strOperator1 = CurrentUser.Name;
            //List<object> realDataTmp = new List<object>();
            //int companyID = CurrentUser.UserBranches.First().Branch.CompanyID;
            foreach (var p in dataTmp.OrderBy(c => c.Product.ProductCode))
            {
                //recuperation du prix de vente du produit
                LensNumberRangePrice price = _priceRepository.GetPrice(p.Product.ProductID);
                double SellingPrice = (price != null && price.LensNumberRangePriceID > 0) ? price.Price : 0;
                model.Add(
                    new
                    {
                        Localization = p.Localization.LocalizationLabel,
                        ProductLabel = p.Product.ProductCode,
                        ProductQty = p.ProductLocalizationStockQuantity,
                        ProductUnitPrice = SellingPrice, //p.ProductLocalizationStockSellingPrice,
                        ProductLocalizationSafetyStockQuantity = p.ProductLocalizationSafetyStockQuantity,
                        //Amount = p.Amount,
                        BranchName = p.Localization.Branch.BranchName,
                        //IsSafQuantStockReached = p.IsSafQuantStockReached,
                        BranchAdress = p.Localization.Branch.Adress.AdressEmail + "/" + p.Localization.Branch.Adress.AdressPOBox,
                        BranchTel = p.Localization.Branch.Adress.AdressPhoneNumber,
                        CompanyName = Company.Name,
                        CompanyAdress = Company.Adress.AdressEmail + "/" + Company.Adress.AdressPOBox,
                        CompanyTel = Company.Adress.AdressPhoneNumber,
                        CompanyCNI = Company.CNI,
                        CompanyLogo = _fileRepository.FindAll.FirstOrDefault(f => f.GlobalPersonID == Company.GlobalPersonID) != null ? _fileRepository.FindAll.FirstOrDefault(f => f.GlobalPersonID == Company.GlobalPersonID).Content : COMPANY_LOGO,
                        Marque = p.Marque,
                        NumeroSerie = p.NumeroSerie
                    }
                    );
            }

            return View(model);

        }
        private Company Company
        {
            get
            {
                return db.Companies.FirstOrDefault();
            }
        }

    }
}