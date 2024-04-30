using FatSod.Security.Abstracts;
using FatSod.Security.Entities;
using FatSod.Supply.Entities;
using CABOPMANAGEMENT.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using FatSod.Supply.Abstracts;
using FatSod.Ressources;
using CABOPMANAGEMENT.Filters;
using FatSod.Report.WrapReports;
using CABOPMANAGEMENT.Tools;
using CABOPMANAGEMENT.Areas.Sale.Models;

namespace CABOPMANAGEMENT.Areas.Supply.Controllers
{
    [Authorize(Order = 1)]
    [TakeBusinessDay(Order = 2)]
    public class ProductGiftController : BaseController
    {
        private bool isLens = false;
        //Current Controller and current page
        private const string CONTROLLER_NAME = "Supply/ProductGift";
        private const string VIEW_NAME = "Index";
        private IProductGift _productGiftRepository;
        private IBusinessDay _busDayRepo;
        private ITransactNumber _transactNumbeRepository;
        private IProductLocalization _productLocRepository;
        private ILensNumberRangePrice _priceRepository;

        //Construcitor

        private List<BusinessDay> lstBusDay;
        public ProductGiftController(IProductGift productGiftRepository,
            IBusinessDay bDRepo,
            ITransactNumber transactRepo,
            ILensNumberRangePrice priceRepo,
            IProductLocalization productLocRepo
            )
        {
            this._productGiftRepository = productGiftRepository;
            this._busDayRepo = bDRepo;
            this._transactNumbeRepository = transactRepo;
            this._priceRepository = priceRepo;
            this._productLocRepository = productLocRepo;
        }
        [OutputCache(Duration = 3600)]
        public ActionResult Index()
        {
            Session["isUpdate"] = false;
            Session["LensNumberFullCode"] = "*";

            Session["MaxValue"] = 0;
            Session["CurrentProduct"] = "";
            Session["CurrentProductID"] = 0;
            Session["SafetyStock"] = 0;

            List<ProductGiftLine> ProductGiftLines = new List<ProductGiftLine>();
            Session["ProductGiftLines"] = ProductGiftLines;
            lstBusDay = (List<BusinessDay>)Session["UserBusDays"];
            if (lstBusDay == null)
            {
                lstBusDay = _busDayRepo.GetOpenedBusinessDay(CurrentUser);
            };
            DateTime BDDateOperation = lstBusDay.FirstOrDefault().BDDateOperation;

            ViewBag.BusnessDayDate = BDDateOperation.ToString("yyyy-MM-dd");
            InjectUserConfigInSession();
            //return rPVResult;
            return View(ModelPendingProductGift(lstBusDay.FirstOrDefault().BranchID, BDDateOperation));
        }

        //[HttpPost]
        public JsonResult AddManager(ProductGift productGift)
        {
            bool status = false;
            string Message = "";
            try
            {
                List<ProductGiftLine> ProductGiftLines = new List<ProductGiftLine>();
                ProductGiftLines = (List<ProductGiftLine>)Session["ProductGiftLines"];
                productGift.ProductGiftLines = ProductGiftLines;

                if (productGift.ProductGiftID == 0)
                {
                    int ProductGiftID = _productGiftRepository.DoProductGift(productGift, SessionGlobalPersonID).ProductGiftID;
                    status = true;
                    SimpleReset();
                    Message = Resources.Success + " Products Have Been Successfully Sended ";
                    return new JsonResult { Data = new { status = status, Message = Message } };
                }

                if (productGift.ProductGiftID > 0)
                {
                    SimpleReset();
                    status = true;
                    Message = Resources.er_alert_danger + " This operation not yet possible ";
                    return new JsonResult { Data = new { status = status, Message = Message } };
                }

            }
            catch (Exception e)
            {
                status = false;
                Message = "Error " + e.Message + " " + e.StackTrace + " " + e.InnerException;
            }
            return new JsonResult { Data = new { status = status, Message = Message } };

        }

        public JsonResult InitTrnNumber(int? BranchID)
        {
            List<object> _InfoList = new List<object>();
            if (BranchID.HasValue && BranchID.Value > 0)
            {
                lstBusDay = (List<BusinessDay>)Session["UserBusDays"];
                if (lstBusDay == null)
                {
                    lstBusDay = _busDayRepo.GetOpenedBusinessDay(CurrentUser);
                };
                BusinessDay businessDay = lstBusDay.FirstOrDefault(l => l.BranchID == BranchID.Value);
                string trnnum = _transactNumbeRepository.displayTransactNumber("PRGI", businessDay);

                _InfoList.Add(new
                {
                    ProductGiftReference = trnnum,
                    RegisteredByID = SessionGlobalPersonID,
                    ProductGiftDate = businessDay.BDDateOperation.ToString("yyyy-MM-dd")
                });
            }
            return Json(_InfoList, JsonRequestBehavior.AllowGet);
        }



        //[HttpPost]
        public ActionResult AddproductGiftLine(ProductGiftLine productGiftLine)
        {
            bool status = false;
            string Message = "";
            try
            {
                productGiftLine.Product = db.Products.Find(productGiftLine.ProductID);
                productGiftLine.Localization = db.Localizations.Find(productGiftLine.LocalizationID);

                productGiftLine.LineUnitPrice = (productGiftLine.Product is Lens) ? (productGiftLine.LineUnitPrice / 2) : productGiftLine.LineUnitPrice;

                List<ProductGiftLine> ProductGiftLines = (List<ProductGiftLine>)Session["ProductGiftLines"];

                //il s'agit d'une modification alors on fait un drop and create
                if (productGiftLine.TMPID > 0)
                {
                    ProductGiftLine toRemove = ProductGiftLines.SingleOrDefault(pl => pl.TMPID == productGiftLine.TMPID);
                    productGiftLine.TMPID = 0;
                    ProductGiftLines.Remove(toRemove);
                }

                //alors la variable de session n'était pas vide
                if (ProductGiftLines != null && ProductGiftLines.Count > 0)
                {
                    //c'est un nouvel ajout dans le panier
                    if (productGiftLine.TMPID == 0)
                    {

                        Category catprod = db.Categories.Find(productGiftLine.Product.CategoryID);

                        int LensCategory = (catprod is LensCategory || catprod == null || catprod.isSerialNumberNull) ? 0 : 1;

                        ProductGiftLine existing = (LensCategory == 0) ? ProductGiftLines.SingleOrDefault(pl => pl.ProductID == productGiftLine.ProductID &&
                                                                                                         pl.LocalizationID == productGiftLine.LocalizationID && pl.Marque == productGiftLine.Marque && pl.NumeroSerie == productGiftLine.NumeroSerie
                                                                                                    ) : ProductGiftLines.SingleOrDefault(pl => pl.ProductID == productGiftLine.ProductID &&
                                                                                                        pl.LocalizationID == productGiftLine.LocalizationID
                                                                                                    );
                        //existe t-il déjà une ligne de vente ayant le meme produit et le même magasin que celui en création?
                        if (
                            existing != null && existing.TMPID > 0) //cette ligne exixte déjà
                        {
                            //la quantité est la somme des deux quantité
                            productGiftLine.LineQuantity += existing.LineQuantity;
                            //l'id c'est l'id de la ligne existante
                            productGiftLine.TMPID = existing.TMPID;
                            productGiftLine.ProductGiftLineID = existing.ProductGiftLineID;
                            //on retire l'ancien pour ajouter le nouveau
                            ProductGiftLines.Remove(existing);
                        }

                        if (existing == null || existing.TMPID == 0)//La ligne n'existe pas encore dans le panier
                        {
                            productGiftLine.TMPID = ProductGiftLines.Select(pl => pl.TMPID).Max() + 1;
                        }
                    }
                }

                //alors la variable de session était vide
                if (ProductGiftLines == null || ProductGiftLines.Count == 0)
                {
                    //c'est une nouvelle création pour la création
                    ProductGiftLines = new List<ProductGiftLine>();
                    productGiftLine.TMPID = 1;
                }
                bool isStockControl = (bool)Session["isStockControl"];
                bool res = true;
                if (isStockControl)
                {
                    //verifiction ds le stock source s'il ya assez de produit a productGifter
                    if (productGiftLine.Product.Category.isSerialNumberNull) //frame
                    {
                        res = _productLocRepository.checkQtyInStock(productGiftLine.ProductID, productGiftLine.LocalizationID, productGiftLine.LineQuantity, productGiftLine.NumeroSerie, productGiftLine.Marque);
                    }
                    else
                    {
                        res = _productLocRepository.checkQtyInStock(productGiftLine.ProductID, productGiftLine.LocalizationID, productGiftLine.LineQuantity);
                    }

                }
                if (res)
                {
                    ProductGiftLines.Add(productGiftLine);
                    Session["ProductGiftLines"] = ProductGiftLines;
                }
                else
                {
                    status = false;
                    Message = Resources.NoProductInStock;
                    return new JsonResult { Data = new { status = status, Message = Message } };
                }
                status = true;
                Message = "Ok";
            }
            catch (Exception e)
            {
                status = false;
                Message = "Error " + e.Message + " " + e.StackTrace + " " + e.InnerException;
            }
            return new JsonResult { Data = new { status = status, Message = Message } };
        }

        // [HttpPost]
        public JsonResult RemoveProductGiftLine(int TMPID)
        {
            List<ProductGiftLine> ProductGiftLines = (List<ProductGiftLine>)Session["ProductGiftLines"];

            if (ProductGiftLines != null && ProductGiftLines.Count > 0)
            {
                ProductGiftLine toRemove = ProductGiftLines.SingleOrDefault(pl => pl.TMPID == TMPID);
                ProductGiftLines.Remove(toRemove);

                Session["ProductGiftLines"] = ProductGiftLines;
            }

            var list = new
            {
                data = from pl in ProductGiftLines
                       select new
                       {
                           TMPID = pl.TMPID,
                           ProductLabel = (pl.Product is Lens) ? pl.Product.ProductCode : pl.Product.ProductDescription,
                           Localization = pl.Localization.LocalizationLabel,
                           LineQuantity = pl.LineQuantity,
                           LineUnitPrice = pl.LineUnitPrice,
                           ProductGiftReason = pl.ProductGiftReason
                       }
            };

            return Json(list, JsonRequestBehavior.AllowGet);
        }


        //Return salelines of current sale
        // [HttpPost]
        public JsonResult ProductGiftLines()
        {
            List<ProductGiftLine> ProductGiftLines = (List<ProductGiftLine>)Session["ProductGiftLines"];
            var list = new
            {
                data = from pl in ProductGiftLines
                       select new
                       {
                           TMPID = pl.TMPID,
                           ProductLabel = (pl.Product is Lens) ? pl.Product.ProductCode : pl.Product.ProductDescription,
                           Localization = pl.Localization.LocalizationLabel,
                           LineQuantity = pl.LineQuantity,
                           LineUnitPrice = pl.LineUnitPrice,
                           ProductGiftReason = pl.ProductGiftReason,
                           NumeroSerie = pl.NumeroSerie,
                           Marque = pl.Marque
                       }
            };

            return Json(list, JsonRequestBehavior.AllowGet);

        }


        public JsonResult BarCodeScanned(string BarCode)
        {
            bool status = true;
            statusOperation = "ok";

            try
            {

                var stock = db.ProductLocalizations.SingleOrDefault(pl => pl.BarCode == BarCode);

                if (stock == null || stock.NumeroSerie == null)
                {
                    status = false;
                    statusOperation = "There is no Frame in the system with the given barcode. Please try again or see administrator for more details";
                    return new JsonResult { Data = new { status = status, Message = statusOperation } };
                }

                var res = new
                {
                    NumeroSerie = stock.NumeroSerie,
                    Marque = stock.Marque,
                    LocalizationID = stock.LocalizationID,
                    ProductID = stock.ProductID,
                    BarCode = stock.BarCode,
                    ProductLocalizationID = stock.ProductLocalizationID,
                    ProductCategory = stock.Product.CategoryID,
                    status = true
                };

                return Json(res, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                status = false;
                statusOperation = "Error " + e.Message + " " + e.StackTrace + " " + e.InnerException;
                return new JsonResult { Data = new { status = status, Message = statusOperation } };
            }
        }

        private void SimpleReset()
        {
            List<ProductGiftLine> ProductGiftLines = new List<ProductGiftLine>();
            Session["ProductGiftLines"] = ProductGiftLines;
            
        }


        public JsonResult DeleteProductGift(int ProductGiftID)
        {
            bool status = false;
            string Message = "";
            try
            {
                _productGiftRepository.CancelProductGift(ProductGiftID);
                status = true;
                Message = Resources.Success + " - product Damage Has Been Successfully Deleted ";
            }
            catch (Exception e)
            {
                status = false;
                Message = "Error " + e.Message + " " + e.StackTrace + " " + e.InnerException;
            }
            return new JsonResult { Data = new { status = status, Message = Message } };
        }

        public List<ProductGift> ModelPendingProductGift(int? BranchID, DateTime ProductGiftDate)
        {
            List<ProductGift> list = new List<ProductGift>();

            if (BranchID.HasValue && BranchID.Value > 0)
            {


                List<ProductGift> dataTmp = (from pt in db.ProductGifts
                                               where (pt.BranchID == BranchID.Value && pt.ProductGiftDate == ProductGiftDate.Date)
                                               select pt).ToList();

                if (dataTmp != null && dataTmp.Count > 0)
                {

                    foreach (ProductGift pt in dataTmp)
                    {
                        list.Add(
                            new ProductGift
                            {
                                ProductGiftID = pt.ProductGiftID,
                                ProductGiftReference = pt.ProductGiftReference,
                                ProductGiftDate = pt.ProductGiftDate,
                                Branch = pt.Branch,
                                AutorizedBy = pt.AutorizedBy,
                                RegisteredBy = pt.RegisteredBy,
                            }
                           );
                    }

                }


            }

            return list;
        }
        //[HttpPost]
        public JsonResult GetAllPendingProductGifts(int? BranchID, DateTime ReloadProductGiftDate)
        {
            if (BranchID == null) BranchID = SessionBusinessDay(null).BranchID;

            var list = new
            {
                data = from pt in ModelPendingProductGift(BranchID, ReloadProductGiftDate)
                       select new
                       {
                           ProductGiftID = pt.ProductGiftID,
                           ProductGiftReference = pt.ProductGiftReference,
                           ProductGiftDate = pt.ProductGiftDate.ToString("yyyy-MM-dd"),
                           Branch = pt.Branch.BranchName,
                           AutorizedByID = pt.AutorizedByID,
                           AutorizedBy = pt.AutorizedBy.UserFullName,
                           RegisteredBy = pt.RegisteredBy.UserFullName
                       }
            };

            return Json(list, JsonRequestBehavior.AllowGet);

        }

        private Company Company
        {
            get
            {
                return db.Companies.FirstOrDefault();
            }
        }


        public JsonResult OpenedBusday()
        {
            List<object> list = new List<object>();
            lstBusDay = (List<BusinessDay>)Session["UserBusDays"];
            if (lstBusDay == null)
            {
                lstBusDay = _busDayRepo.GetOpenedBusinessDay(CurrentUser);
            };

            foreach (BusinessDay busDay in lstBusDay)
            {
                list.Add(
                    new
                    {
                        BranchID = busDay.BranchID,
                        BranchName = busDay.Branch.BranchName
                    }
                    );
            }

            return Json(list, JsonRequestBehavior.AllowGet);

        }
        public JsonResult populateUsers()
        {

            List<object> userList = new List<object>();
            foreach (User user in db.People.OfType<User>().Where(u => u.IsConnected && u.ProfileID>2 && u.UserAccessLevel>1).ToArray())
            {
                userList.Add(new
                {
                    UserFullName = user.UserFullName,
                    GlobalPersonID = user.GlobalPersonID
                });
            }
            return Json(userList, JsonRequestBehavior.AllowGet);

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



        public JsonResult GetEquipmentPrice(int? CurrentProduct)
        {
            List<object> _InfoList = new List<object>();
            if ((!CurrentProduct.HasValue || CurrentProduct.Value <= 0))
            {
                return Json(_InfoList, JsonRequestBehavior.AllowGet);
            }
            double priceEquip = 0d;
            double valeur = 1d;

            Product product = db.Products.Find(CurrentProduct.Value);
            bool productIsLens = product is Lens;
            if (!productIsLens)
            {

                Session["isApplyToCalculate"] = true;
                priceEquip = product.SellingPrice;
                valeur = 1d;

                Session["valeur"] = valeur;
            }

            _InfoList.Add(new
            {
                LineUnitPrice = priceEquip

            });
            return Json(_InfoList, JsonRequestBehavior.AllowGet);
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
                    CategoryCode = cat.CategoryLabel
                });
            }

            return Json(categoryList, JsonRequestBehavior.AllowGet);
        }

        //
        public JsonResult DisableNumero(int ProductCategoryID)
        {
            List<object> _InfoList = new List<object>();
            //0 = frame
            //1 = equipement
            //2 = lens
            Category catprod = db.Categories.Find(ProductCategoryID);
            _InfoList.Add(new
            {
                //LensCategory = (catprod is LensCategory || catprod == null || catprod.isSerialNumberNull) ? 0 : 1
                LensCategory = (catprod == null || catprod.isSerialNumberNull) ? 0 : (catprod is LensCategory) ? 2 : 1
            });
            return Json(_InfoList, JsonRequestBehavior.AllowGet);
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
            return (lstLensProduct == null) ? 0 : lstLensProduct.ProductID;

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
                    if (Lenscatprod.TypeLens.ToUpper()=="SV")
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

        public JsonResult InitProductDetail(int ProductLocalizationID)
        {
            List<object> _CommandList = new List<object>();
            double StockQuantity = 0d;
            double FramePrice = 0d;

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
                    FramePrice = prodLocalization.Product.SellingPrice;

                    Session["isUpdate"] = false;
                }


                _CommandList.Add(new
                {
                    ProductID = prodLocalization.ProductID,
                    marque = prodLocalization.Marque,
                    StockQuantity = StockQuantity,
                    FramePrice = FramePrice,
                    ProductCode = prodLocalization.Product.ProductCode,
                    NumeroSerie = prodLocalization.NumeroSerie
                });

            }
            return Json(_CommandList, JsonRequestBehavior.AllowGet);
        }

        //recup des produits

        public List<Product> ModelProductLocalCat(int DepartureLocalizationID, int? ProductCategoryID, int? ProductNumberID)
        {
            List<Product> model = new List<Product>();


            //On a un produit générique
            if (DepartureLocalizationID > 0 && (ProductCategoryID == 0 || ProductCategoryID == null) && (ProductNumberID == 0 || ProductNumberID == null)) //chargement des produits en fct du magasin slt
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
                            .Where(lsp => lsp.pl.LocalizationID == DepartureLocalizationID
                            && lsp.pr.ls.LensNumberID == ProductNumberID && lsp.pr.p.CategoryID == ProductCategoryID)
                            .Select(s => new
                            {
                                ProductID = s.pr.p.ProductID,
                                ProductCode = s.pr.p.ProductCode,
                                ProductLabel = s.pr.p.ProductLabel,
                                ProductQuantity = s.pl.ProductLocalizationStockQuantity,
                                ProductLocalizationStockSellingPrice = s.pl.ProductLocalizationStockSellingPrice
                            }).ToList();

                        foreach (var pt in lstLensProduct.OrderBy(c => c.ProductCode))
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
                        .Where(lsp => lsp.pl.LocalizationID == DepartureLocalizationID && lsp.p.CategoryID == ProductCategoryID && !(lsp.p.Category is LensCategory))
                        .Select(s => new
                        {
                            ProductID = s.p.ProductID,
                            ProductCode = s.p.ProductCode,
                            ProductLabel = s.p.ProductLabel,
                            ProductQuantity = s.pl.ProductLocalizationStockQuantity,
                            ProductLocalizationStockSellingPrice = s.pl.ProductLocalizationStockSellingPrice
                        }).ToList();

                    foreach (var pt in lstLensProduct.OrderBy(c => c.ProductCode))
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

        public JsonResult OnProductSelected(int? Localization, int? CurrentProduct)
        {
            List<object> _InfoList = new List<object>();
            double StockQuantity = 0d;
            double LineUnitPrice = 0d;
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
                //Récupération du prix du verre à partir de son intervalle de numéro
                LensNumberRangePrice price = _priceRepository.GetPrice(product.ProductID);
                LineUnitPrice = (price != null && price.LensNumberRangePriceID > 0) ? price.Price : 0;
            }
            else
            {
                var prodLoc = db.ProductLocalizations.Where(pl => pl.ProductID == CurrentProduct.Value && pl.LocalizationID == Localization.Value)
                .Select(p => new
                {
                    ProductID = p.ProductID,
                    ProductLocalizationStockQuantity = p.ProductLocalizationStockQuantity,
                    SellingPrice = p.Product.SellingPrice,
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
                    Session["EquipPrice"] = prodLoc.SellingPrice;
                    LineUnitPrice = prodLoc.SellingPrice;
                    Session["isUpdate"] = false;
                }
            }


            _InfoList.Add(new
            {
                StockQuantity = StockQuantity,
                LineUnitPrice = LineUnitPrice

            });
            return Json(_InfoList, JsonRequestBehavior.AllowGet);
        }


        //This method print a receipt of customer
        public ActionResult GenerateReceipt(int ProductGiftID)
        {
            List<RptTransfertForm> model = new List<RptTransfertForm>();
            System.IO.Stream stream = new System.IO.MemoryStream();
            try
            {

                @ViewBag.CompanyLogoID = Company.GlobalPersonID;
                string defaultImgPath = Server.MapPath("~/Content/Images/App/deboy-logo.png");
                byte[] COMPANY_LOGO = System.IO.File.ReadAllBytes(defaultImgPath);

                string DeviseLabel = db.Devises.Where(d => d.DefaultDevise).FirstOrDefault().DeviseLabel;
                ProductGift ProductGift = (from pt in db.ProductGifts
                                               where pt.ProductGiftID == ProductGiftID
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
                double saleAmount = 0d;
                db.ProductGiftLines.Where(l => l.ProductGiftID == ProductGiftID).ToList().ForEach(c =>
                {
                    i = i + 1;
                    saleAmount += c.LineUnitPrice;
                    model.Add(
                            new RptTransfertForm
                            {
                                RptTransfertFormID = i,
                                ReceiveAmount = 0,
                                LineQuantity = c.LineQuantity,
                                LineUnitPrice = c.LineUnitPrice,
                                ProductLabel = c.ProductGiftReason,
                                ProductRef = (c.NumeroSerie=="" || c.NumeroSerie==null) ? c.Product.ProductCode : c.Product.ProductCode + " Ref:"+c.NumeroSerie+" Marque:"+c.Marque,//(c.OeilDroiteGauche == EyeSide.N) ? c.Product.ProductCode : c.OeilDroiteGauche + ":" + c.Product.ProductCode,
                                CompanyName = Company.Name,
                                CompanyAdress = "Head Quater : " + Company.Adress.Quarter.Town.Region.RegionLabel + " - " + Company.Adress.Quarter.Town.TownLabel,
                                CompanyTel = "Tel: " + Company.Adress.AdressPhoneNumber,
                                BranchName = curBranch.Branch.BranchName,
                                BranchAdress = curBranch.Branch.Adress.Quarter.QuarterLabel + " - " + curBranch.Branch.Adress.Quarter.Town.TownLabel,
                                BranchTel = "Tel: " + curBranch.Branch.Adress.AdressPhoneNumber,
                                Ref = ProductGift.ProductGiftReference,
                                CompanyCNI = "NO CONT : " + Company.CNI,
                                Operator = CurrentUser.Name + " " + CurrentUser.Description,
                                SendindBranchName = ProductGift.Branch.BranchName,
                                SendindBranchCode = ProductGift.Branch.BranchCode,
                                TransfertDate = ProductGift.ProductGiftDate.Date,
                                Title = "Product productGiftt lines informations",
                                DeviseLabel = DeviseLabel,
                                //CompanyLogo = db.Files.FirstOrDefault(f => f.GlobalPersonID == Company.GlobalPersonID) != null ? db.Files.FirstOrDefault(f => f.GlobalPersonID == Company.GlobalPersonID).Content : COMPANY_LOGO
                            }
                    );
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