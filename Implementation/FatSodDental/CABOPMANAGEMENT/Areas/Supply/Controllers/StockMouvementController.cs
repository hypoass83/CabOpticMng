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
using FatSod.DataContext.Repositories;
using CABOPMANAGEMENT.Tools;
using CABOPMANAGEMENT.Areas.Sale.Models;

namespace CABOPMANAGEMENT.Areas.Supply.Controllers
{
    [Authorize(Order = 1)]
    [TakeBusinessDay(Order = 2)]
    public partial class StockMouvementController : BaseController
    {
        private bool isLens = false;
        //Current Controller and current page
        private const string CONTROLLER_NAME = "Supply/StockMouvement";
        private const string VIEW_NAME = "Index";
        private IBusinessDay _busDayRepo;
        private IProductLocalization _productLocRepository;
        private IRepository<FatSod.Security.Entities.File> _fileRepository;

        //Construcitor

        private List<BusinessDay> lstBusDay;
        public StockMouvementController(
            IBusinessDay bDRepo,
            IRepository<FatSod.Security.Entities.File> fileRepository,
            IProductLocalization productLocRepo
            )
        {
            this._busDayRepo = bDRepo;
            this._productLocRepository = productLocRepo;
            this._fileRepository = fileRepository;
        }
        [OutputCache(Duration = 3600)]
        public ActionResult Index()
        {
            Session["isUpdate"] = false;
            Session["LensNumberFullCode"] = "*";

            lstBusDay = (List<BusinessDay>)Session["UserBusDays"];
            if (lstBusDay == null)
            {
                lstBusDay = _busDayRepo.GetOpenedBusinessDay(CurrentUser);
            }
            if (lstBusDay.Count() > 1)
            {
                ViewBag.Disabled = false;
            }
            DateTime busDays = lstBusDay.FirstOrDefault().BDDateOperation;

            ViewBag.CurrentBranch = lstBusDay.FirstOrDefault().BranchID;
            ViewBag.BusnessDayDate = busDays.ToString("yyyy-MM-dd");
            Session["BusnessDayDate"] = busDays;
            /*
            Session["BeginDate"] = new DateTime(1900, 1, 1);
            Session["EndDate"] = new DateTime(1900, 1, 1);
            Session["ProductLocalization"] = new ProductLocalization();
            */
            //this.chargeSolde();
            return View(/*ModelRptProductSale(new DateTime(1900, 1, 1), new DateTime(1900, 1, 1), new ProductLocalization())*/);

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
        public JsonResult ChargeQtyBefore(DateTime Bdate, int ProductID, int LocalizationID,string NumeroSerie, string Marque)
        {
            List<object> _InfoList = new List<object>();
            double TotalQtyBefore = 0d;
            List<InventoryHistoric> invHistory = new List<InventoryHistoric>();
            if (ProductID>0)
            {
                Product selectProduct = db.Products.Find(ProductID);
                if (selectProduct.Category.isSerialNumberNull) //frame
                {
                    invHistory = db.InventoryHistorics.Where(ih => ih.InventoryDate < Bdate.Date && ih.ProductID == ProductID && ih.LocalizationID == LocalizationID && ih.NumeroSerie == NumeroSerie && ih.Marque == Marque).OrderByDescending(s => s.InventoryHistoricID).Take(1).ToList();
                }
                else
                {
                    invHistory = db.InventoryHistorics.Where(ih => ih.InventoryDate < Bdate.Date && ih.ProductID == ProductID && ih.LocalizationID == LocalizationID).OrderByDescending(s => s.InventoryHistoricID).Take(1).ToList();
                }

                foreach (InventoryHistoric getHist in invHistory)
                {
                    TotalQtyBefore = TotalQtyBefore + getHist.NewStockQuantity;
                }
            }
           

            _InfoList.Add(new
            {
                TotalQtyBefore = TotalQtyBefore
            });
            return Json(_InfoList, JsonRequestBehavior.AllowGet);
        }
        public JsonResult chargeSolde(DateTime Bdate, DateTime Edate, int ProductID, int LocalizationID)
        {
            List<object> _InfoList = new List<object>();
            double TotalQty = 0d;

            var invHistory = db.InventoryHistorics.Where(ih => ih.InventoryDate <= Edate.Date && ih.ProductID == ProductID && ih.LocalizationID == LocalizationID).OrderByDescending(s => s.InventoryHistoricID).Take(1);
            foreach (var getHist in invHistory)
            {
                TotalQty = TotalQty + getHist.NewStockQuantity;
            }

            _InfoList.Add(new
            {
                TotalQty = TotalQty
            });
            return Json(_InfoList, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAllStockedLocations()
        {
            List<object> localizationsList = new List<object>();
            foreach (Localization Loc in db.Localizations.ToList())
            {
                localizationsList.Add(new
                {
                    LocalizationCode = Loc.LocalizationCode,
                    LocalizationID = Loc.LocalizationID
                });
            }

            return Json(localizationsList, JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetProductCategory()
        {
           
            //return Json(ProductCategory, JsonRequestBehavior.AllowGet);
            List<object> categoryList = new List<object>();
            List<Category> categories = LoadComponent.GetAllGenericCategoryItems();
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
        //
        public JsonResult DisableNumero(int ProductCategoryID)
        {
            List<object> _InfoList = new List<object>();
            Category catprod = db.Categories.Find(ProductCategoryID);
            _InfoList.Add(new
            {
                //LensCategory = (catprod is LensCategory || catprod == null || catprod.isSerialNumberNull) ? 0 : 1
                LensCategory = (catprod == null || catprod.isSerialNumberNull) ? 0 : (catprod is LensCategory) ? 2 : 1
            });
            return Json(_InfoList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAllSerialNumber(string filter, int DepartureLocalizationID = 0)
        {

            List<object> customersList = new List<object>();
            List<ProductLocalization> res = new List<ProductLocalization>();

            res = db.ProductLocalizations.Where(c => c.LocalizationID == DepartureLocalizationID && c.NumeroSerie.Contains(filter.ToLower())).Take(200).ToList();

            foreach (var prodLocalization in res)
            {

                customersList.Add(new
                {
                    NumeroSerie = prodLocalization.NumeroSerie,
                    ProductLocalizationID = prodLocalization.ProductLocalizationID
                });
            }

            return Json(customersList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult InitProductDetail(int ProductLocalizationID)
        {
            List<object> _CommandList = new List<object>();
            double StockQuantity = 0d;
           
            if (ProductLocalizationID > 0)
            {

                ProductLocalization prodLocalization = db.ProductLocalizations.Find(ProductLocalizationID);

                Session["MaxValue"] = prodLocalization.ProductLocalizationStockQuantity;
                Session["CurrentProduct"] = prodLocalization.ProductLabel;
                Session["SafetyStock"] = prodLocalization.ProductLocalizationSafetyStockQuantity;

                StockQuantity = prodLocalization.ProductLocalizationStockQuantity;


                bool isUpdate = (bool)Session["isUpdate"];

                if (!isUpdate)
                {
                   
                    Session["isUpdate"] = false;
                }


                _CommandList.Add(new
                {
                    ProductID = prodLocalization.ProductID,
                    marque = prodLocalization.Marque,
                    StockQuantity = StockQuantity,
                    NumeroSerie = prodLocalization.NumeroSerie
                });

            }
            return Json(_CommandList, JsonRequestBehavior.AllowGet);
        }

        private double TotalProductQtyByDate(DateTime OpDate, ProductLocalization pl)
        {
            double TotalQty = 0d;

            var invHistory = db.InventoryHistorics.Where(ih => ih.InventoryDate <= OpDate.Date && ih.ProductID == pl.ProductID && ih.LocalizationID == pl.LocalizationID).OrderByDescending(s => s.InventoryHistoricID).Take(1);
            foreach (var getHist in invHistory)
            {
                TotalQty = TotalQty + getHist.NewStockQuantity;
            }

            return TotalQty;

        }

        private double TotalOutPutProductQtyByDate(DateTime OpDate, ProductLocalization pl)
        {
            double TotalQty = 0d;

            var invHistory = db.InventoryHistorics.Where(ih => ih.InventoryDate <= OpDate.Date && ih.ProductID == pl.ProductID && ih.LocalizationID == pl.LocalizationID && ih.StockStatus.Trim().ToUpper() == "OUTPUT").OrderByDescending(s => s.InventoryHistoricID).ToList();
            foreach (var getHist in invHistory)
            {
                TotalQty = TotalQty + getHist.Quantity;
            }

            return TotalQty;

        }

        private double TotalInPutProductQtyByDate(DateTime OpDate, ProductLocalization pl)
        {
            double TotalStockReg = 0d;
            double TotalQty = 0d;
            //recuperation du stock de depart
            var invHistoryStockReg = db.InventoryHistorics.Where(ih => ih.InventoryDate <= OpDate.Date && ih.ProductID == pl.ProductID && ih.LocalizationID == pl.LocalizationID && ih.inventoryReason.Trim().ToLower() == "stock regularization").OrderByDescending(s => s.InventoryHistoricID).Take(1);
            foreach (var getStockReg in invHistoryStockReg)
            {
                TotalStockReg = TotalStockReg + getStockReg.NewStockQuantity;
            }

            var invHistory = db.InventoryHistorics.Where(ih => ih.InventoryDate <= OpDate.Date && ih.ProductID == pl.ProductID && ih.LocalizationID == pl.LocalizationID && ih.StockStatus.Trim().ToUpper() == "INPUT").ToList();
            foreach (var getHist in invHistory)
            {
                TotalQty = TotalQty + getHist.Quantity;
            }

            return TotalStockReg + TotalQty;

        }

        public JsonResult ModelRptProductSale(DateTime Bdate, DateTime Edate, int ProductID, int LocalizationID, string NumeroSerie, string Marque)
        {
            if (ProductID > 0)
            {
                Product selectProduct = db.Products.Find(ProductID);
                if (selectProduct.Category.isSerialNumberNull) //frame
                {
                    return ModelRptProductSaleFrame(Bdate, Edate, ProductID, LocalizationID, NumeroSerie, Marque);
                }
                else
                {
                    return ModelRptProductSaleOther(Bdate, Edate, ProductID, LocalizationID);
                }
            }
            else
            {
                return ModelRptProductSaleOther(Bdate, Edate, ProductID, LocalizationID);
            }

        }

        public JsonResult ModelRptProductSaleOther(DateTime Bdate, DateTime Edate, int ProductID, int LocalizationID)
        {
            var list = new
            {
                data = from c in db.InventoryHistorics.AsEnumerable()
                       where (c.InventoryDate.Date >= Bdate.Date && c.InventoryDate.Date <= Edate.Date)
                       && c.ProductID == ProductID && c.LocalizationID == LocalizationID
                       orderby c.InventoryHistoricID
                       select
                       new
                       {
                           RptGeneSaleID = c.InventoryHistoricID,
                           CustomerOrderDate = c.InventoryDate.ToString("yyyy-MM-dd"),
                           CustomerOrderNumber = c.Quantity,
                           CustomerName = c.inventoryReason,
                           CodeClient = c.StockStatus,
                           NomClient = c.Description,
                           Code = c.InventoryHistoricID,
                           LineQuantity = c.NewStockQuantity,
                           CustomerOrderTotalPrice = c.NEwStockUnitPrice,
                           ProductID = c.ProductID,
                           ProductCode = c.Product.ProductCode,
                           PostByID = c.AutorizedBy.Name,
                           OperatorID = c.RegisteredBy.Name
                       }
            };
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ModelRptProductSaleFrame(DateTime Bdate, DateTime Edate, int ProductID, int LocalizationID, string NumeroSerie, string Marque)
        {
            var list = new
            {
                data = from c in db.InventoryHistorics.AsEnumerable()
                       where (c.InventoryDate.Date >= Bdate.Date && c.InventoryDate.Date <= Edate.Date)
                       && c.ProductID == ProductID && c.LocalizationID == LocalizationID && c.NumeroSerie== NumeroSerie && c.Marque== Marque
                       orderby c.InventoryHistoricID
                       select
                       new
                       {
                           RptGeneSaleID = c.InventoryHistoricID,
                           CustomerOrderDate = c.InventoryDate.ToString("yyyy-MM-dd"),
                           CustomerOrderNumber = c.Quantity,
                           CustomerName = c.inventoryReason,
                           CodeClient = c.StockStatus,
                           NomClient = c.Description,
                           Code = c.InventoryHistoricID,
                           LineQuantity = c.NewStockQuantity,
                           CustomerOrderTotalPrice = c.NEwStockUnitPrice,
                           ProductID = c.ProductID,
                           ProductCode = c.Product.ProductCode + " Ref :" + NumeroSerie + " Marque "+ Marque ,
                           PostByID = c.AutorizedBy.Name,
                           OperatorID = c.RegisteredBy.Name
                       }
            };
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAllStockMouvement(DateTime Bdate, DateTime Edate)
        {
            var list = new
            {
                data = from c in db.InventoryHistorics.AsEnumerable()
                       where (c.InventoryDate.Date >= Bdate.Date && c.InventoryDate.Date <= Edate.Date)
                       orderby c.InventoryDate descending
                       select
                       new
                       {
                           RptGeneSaleID = c.InventoryHistoricID,
                           CustomerOrderDate = c.InventoryDate.ToString("yyyy-MM-dd"),
                           CustomerOrderNumber = c.Quantity,
                           CustomerName = c.inventoryReason,
                           CodeClient = c.StockStatus,
                           NomClient = c.Description,
                           Code = c.InventoryHistoricID,
                           LineQuantity = c.NewStockQuantity,
                           CustomerOrderTotalPrice = c.NEwStockUnitPrice,
                           ProductID = c.ProductID,
                           ProductCode = c.Product.ProductCode,
                           PostByID = c.AutorizedBy.Name,
                           OperatorID = c.RegisteredBy.Name
                       }
            };
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAllNumbers(string filter, int? ProductCategory, int? localization)
        {

            List<ModelLensNumber> numbers = ModelLensNumber(filter, ProductCategory.Value, localization.Value);

            return Json(numbers, JsonRequestBehavior.AllowGet);

        }
        public List<ModelLensNumber> ModelLensNumber(string filter, int ProductCategoryID, int Localization)
        {

            List<ModelLensNumber> model = new List<ModelLensNumber>();

            //produit de verre
            try
            {

                //produit de verre

                Category catprod = db.Categories.Find(ProductCategoryID);

                isLens = (catprod is LensCategory);

                if (isLens)
                {
                    LensCategory Lenscatprod = (LensCategory)catprod;

                    string categoryCode = catprod.CategoryCode;

                    //if (categoryCode.ToLower().Contains("SV".ToLower()))
                    if (Lenscatprod.TypeLens.ToUpper() == "SV")
                    {
                        foreach (LensNumber ln in this.GetAllSVNumbers(filter))
                        {

                            ModelLensNumber ln2 = new ModelLensNumber()
                            {
                                LensNumberID = ln.LensNumberID,
                                LensNumberFullCode = ln.LensNumberFullCode
                            };

                            model.Add(ln2);
                        }
                    }

                    //if (categoryCode.ToLower().Contains("BF".ToLower()) || categoryCode.ToLower().Contains("PRO".ToLower()))
                    if (Lenscatprod.TypeLens.ToUpper() == "BIFOCAL" || Lenscatprod.TypeLens.ToUpper() == "PROG")
                    {
                        foreach (LensNumber ln in this.GetADDAllNumbers(filter))
                        {

                            ModelLensNumber ln2 = new ModelLensNumber()
                            {
                                LensNumberID = ln.LensNumberID,
                                LensNumberFullCode = ln.LensNumberFullCode
                            };

                            model.Add(ln2);
                        };
                    }

                }

            }
            catch (Exception e)
            {

                throw e;
            }
            return model;
        }
        public IEnumerable<LensNumber> GetAllSVNumbers(string filter)
        {

            var lstLens = (from c in db.LensNumbers
                           where ((c.LensNumberAdditionValue == null || c.LensNumberAdditionValue.Length <= 0) && c.LensNumberDescription.StartsWith(filter))
                           select c).Take(15);

            return lstLens;
        }
        /// <summary>
        /// Cette méthode retourne la liste des numéros de la base de données
        /// </summary>
        /// <returns></returns>
        public IEnumerable<LensNumber> GetADDAllNumbers(string filter)
        {
            
            var lstLens = (from ln in db.LensNumbers
                           where (ln.LensNumberAdditionValue.Length > 0 && ln.LensNumberDescription.StartsWith(filter.ToLower().Trim()))
                           select ln).Take(15);

            return lstLens;
        }

        public ActionResult OnLensNumberSelected(int? Localization, int? ProductNumberID, int? ProductCategoryID)
        {
            List<object> _InfoList = new List<object>();
            int lensId = 0;
            if ((Localization.HasValue && Localization.Value > 0) && (ProductNumberID.HasValue && ProductNumberID.Value > 0) && (ProductCategoryID.HasValue && ProductCategoryID.Value > 0))
            {
                lensId = GetLensID(Localization.Value, ProductCategoryID.Value, ProductNumberID.Value);
            }

            _InfoList.Add(new
            {
                ProductCode = Session["CurrentProduct"],
                ProductID = Session["CurrentProductID"],
                StockQuantity = Session["MaxValue"]
            });
            return Json(_InfoList, JsonRequestBehavior.AllowGet);
        }

        public int GetLensID(int DepartureLocalizationID, int ProductCategoryID, int ProductNumberID)
        {
            var lstLensProduct = db.Lenses.Join(db.Products, ls => ls.ProductID, p => p.ProductID,
                        (ls, p) => new { ls, p }).
                        Join(db.ProductLocalizations, pr => pr.p.ProductID, pl => pl.ProductID, (pr, pl) => new { pr, pl })
                        .Where(lsp => lsp.pl.LocalizationID == DepartureLocalizationID
                        && lsp.pr.ls.LensNumberID == ProductNumberID && lsp.pr.p.CategoryID == ProductCategoryID)
                        .Select(s => new
                        {
                            ProductID = s.pr.p.ProductID,
                            ProductCode = s.pr.p.ProductCode,
                            ProductLabel = s.pr.p.ProductLabel,
                            ProductQuantity = s.pl.ProductLocalizationStockQuantity,
                            ProductLocalizationStockSellingPrice = s.pl.ProductLocalizationStockSellingPrice,
                            ProductLocalizationSafetyStockQuantity = s.pl.ProductLocalizationSafetyStockQuantity
                        }).FirstOrDefault();
            Session["MaxValue"] = (lstLensProduct == null) ? 0 : lstLensProduct.ProductQuantity;
            Session["CurrentProduct"] = (lstLensProduct == null) ? "" : lstLensProduct.ProductCode;
            Session["CurrentProductID"] = (lstLensProduct == null) ? 0 : lstLensProduct.ProductID;
            Session["SafetyStock"] = (lstLensProduct == null) ? 0 : lstLensProduct.ProductLocalizationSafetyStockQuantity;
            if (lstLensProduct != null)
            {
                return (lstLensProduct == null) ? 0 : lstLensProduct.ProductID;
            }
            else { // Quand c'est un verre de commande
                OrderLens orderLens = db.OrderLenses.FirstOrDefault(x => x.LensCategoryID == ProductCategoryID &&
                                              x.LensNumberID == ProductNumberID);

                if (orderLens != null)
                {
                    var lstOrderLensProduct = db.Lenses.Join(db.Products, ls => ls.ProductID, p => p.ProductID,
                        (ls, p) => new { ls, p }).
                        Join(db.ProductLocalizations, pr => pr.p.ProductID, pl => pl.ProductID, (pr, pl) => new { pr, pl })
                        .Where(lsp => lsp.pl.LocalizationID == DepartureLocalizationID
                        && lsp.pr.ls.LensNumberID == ProductNumberID && lsp.pr.p.CategoryID == ProductCategoryID)
                        .Select(s => new
                        {
                            ProductID = s.pr.p.ProductID,
                            ProductCode = s.pr.p.ProductCode,
                            ProductLabel = s.pr.p.ProductLabel,
                            ProductQuantity = s.pl.ProductLocalizationStockQuantity,
                            ProductLocalizationStockSellingPrice = s.pl.ProductLocalizationStockSellingPrice,
                            ProductLocalizationSafetyStockQuantity = s.pl.ProductLocalizationSafetyStockQuantity
                        }).FirstOrDefault();

                    Session["MaxValue"] = (lstOrderLensProduct == null) ? 0 : lstOrderLensProduct.ProductQuantity;
                    Session["CurrentProduct"] = (lstOrderLensProduct == null) ? "" : lstOrderLensProduct.ProductCode;
                    Session["CurrentProductID"] = (lstOrderLensProduct == null) ? 0 : lstOrderLensProduct.ProductID;
                    Session["SafetyStock"] = (lstOrderLensProduct == null) ? 0 : lstOrderLensProduct.ProductLocalizationSafetyStockQuantity;

                    return (lstOrderLensProduct == null) ? 0 : lstOrderLensProduct.ProductID;
                }
                return (lstLensProduct == null) ? 0 : lstLensProduct.ProductID;
            }

        }

        public JsonResult GetAllProducts(int DepartureLocalizationID, int? ProductCategoryID, int? ProductNumberID)
        {

            List<object> _InfoList = new List<object>();

            List<Product> list = ModelProductLocalCat(DepartureLocalizationID, ProductCategoryID, ProductNumberID);

            foreach (Product s in list.OrderBy(c => c.ProductCode))
            {

                _InfoList.Add(new
                {
                    ProductID = s.ProductID,
                    ProductCode = s.ProductCode,
                });
            }
            return Json(_InfoList, JsonRequestBehavior.AllowGet);


        }

        public List<Product> ModelProductLocalCat(int? LocalizationID, int? ProductCategoryID, int? ProductNumberID)
        {
            List<Product> model = new List<Product>();


            //On a un produit générique
            if ((LocalizationID == null || LocalizationID.Value == 0) && (ProductCategoryID == null || ProductCategoryID.Value == 0) && (ProductNumberID == null || ProductNumberID.Value == 0)) //chargement des produits en fct du magasin slt
            {
                return model;

            }
            else //On a un produit de type verre
            {
                // verifion si c'est un produit de type verre

                Category catprod = db.Categories.Find(ProductCategoryID.Value);

                isLens = (catprod is LensCategory);

                if (isLens)
                //if (lenprod != null) //verre
                {
                    if ((ProductNumberID != null || ProductNumberID > 0) && ProductCategoryID > 0) //desc et numero
                    {

                        //produit de verre

                        var lstLensProduct = db.Lenses.Join(db.Products, ls => ls.ProductID, p => p.ProductID,
                            (ls, p) => new { ls, p }).
                            Join(db.ProductLocalizations, pr => pr.p.ProductID, pl => pl.ProductID, (pr, pl) => new { pr, pl })
                            .Where(lsp => lsp.pl.LocalizationID == LocalizationID
                            && lsp.pr.ls.LensNumberID == ProductNumberID && lsp.pr.p.CategoryID == ProductCategoryID)
                            .Select(s => new
                            {
                                ProductID = s.pr.p.ProductID,
                                ProductCode = s.pr.p.ProductCode,
                                ProductLabel = s.pr.p.ProductLabel,
                                ProductQuantity = s.pl.ProductLocalizationStockQuantity,
                                ProductLocalizationStockSellingPrice = s.pl.ProductLocalizationStockSellingPrice
                            }).ToList();

                        foreach (var pt in lstLensProduct)
                        {
                            model.Add(
                            new Product
                            {
                                ProductID = pt.ProductID,
                                ProductCode = pt.ProductCode,
                                ProductLabel = pt.ProductLabel,
                                ProductQuantity = pt.ProductQuantity,
                                SellingPrice = pt.ProductLocalizationStockSellingPrice
                            }
                           );

                        }

                    }
                    else return model;
                }
                else
                {

                    ////produit generic

                    var lstLensProduct = db.Products.Join(db.ProductLocalizations, p => p.ProductID, pl => pl.ProductID,
                        (p, pl) => new { p, pl })
                        .Where(lsp => lsp.pl.LocalizationID == LocalizationID && lsp.p.CategoryID == ProductCategoryID && !(lsp.p.Category is LensCategory))
                        .Select(s => new
                        {
                            ProductID = s.p.ProductID,
                            ProductCode = s.p.ProductCode,
                            ProductLabel = s.p.ProductLabel,
                            ProductQuantity = s.pl.ProductLocalizationStockQuantity,
                            ProductLocalizationStockSellingPrice = s.pl.ProductLocalizationStockSellingPrice
                        }).ToList();

                    foreach (var pt in lstLensProduct)
                    {
                        model.Add(
                            new Product
                            {
                                ProductID = pt.ProductID,
                                ProductCode = pt.ProductCode,
                                ProductLabel = pt.ProductLabel,
                                ProductQuantity = pt.ProductQuantity,
                                SellingPrice = pt.ProductLocalizationStockSellingPrice
                            }
                           );
                    }

                }
            }

            return model;
        }


        public ActionResult OnProductSelected(int? Localization, int? CurrentProduct)
        {
            List<object> _InfoList = new List<object>();
            double StockQuantity = 0d;
            Session["EquipPrice"] = 0d;

            if ((!Localization.HasValue || Localization.Value <= 0) || (!CurrentProduct.HasValue || CurrentProduct.Value <= 0))
            {
                return Json(_InfoList, JsonRequestBehavior.AllowGet);
            }

            Product product = db.Products.Find(CurrentProduct.Value);
            bool productIsLens = product is Lens;

            if (productIsLens)
            {
                StockQuantity = (double)Session["MaxValue"];
            }
            else
            {
                var prodLoc = db.ProductLocalizations.Where(pl => pl.ProductID == CurrentProduct.Value && pl.LocalizationID == Localization.Value)
                .Select(p => new
                {
                    ProductID = p.ProductID,
                    ProductLocalizationStockQuantity = p.ProductLocalizationStockQuantity,
                    ProductLabel = p.Product.ProductLabel,
                    ProductLocalizationSafetyStockQuantity = p.ProductLocalizationSafetyStockQuantity
                }).SingleOrDefault();

                Session["MaxValue"] = prodLoc.ProductLocalizationStockQuantity;
                Session["CurrentProduct"] = prodLoc.ProductLabel;
                Session["CurrentProductID"] = prodLoc.ProductID;
                Session["SafetyStock"] = prodLoc.ProductLocalizationSafetyStockQuantity;

                StockQuantity = prodLoc.ProductLocalizationStockQuantity;

                bool isUpdate = (bool)Session["isUpdate"];

                if (!isUpdate)
                {
                    Session["isUpdate"] = false;
                }
            }


            _InfoList.Add(new
            {
                StockQuantity = StockQuantity

            });
            return Json(_InfoList, JsonRequestBehavior.AllowGet);
        }
        


    }
}