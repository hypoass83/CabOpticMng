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
    public class StockReplacementController : BaseController
    {
        private bool isLens = false;
        //Current Controller and current page
        private const string CONTROLLER_NAME = "Supply/StockReplacement";
        private const string VIEW_NAME = "Index";
        private IStockReplacement _StockReplacementRepository;
        private IBusinessDay _busDayRepo;
        private ITransactNumber _transactNumbeRepository;
        private IProductLocalization _productLocRepository;
        private ILensNumberRangePrice _priceRepository;

        //Construcitor

        private List<BusinessDay> lstBusDay;
        public StockReplacementController(IStockReplacement StockReplacementRepository,
            IBusinessDay bDRepo,
            ITransactNumber transactRepo,
            ILensNumberRangePrice priceRepo,
            IProductLocalization productLocRepo
            )
        {
            this._StockReplacementRepository = StockReplacementRepository;
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

            List<StockReplacementLine> StockReplacementLines = new List<StockReplacementLine>();
            Session["StockReplacementLines"] = StockReplacementLines;
            lstBusDay = (List<BusinessDay>)Session["UserBusDays"];
            if (lstBusDay == null)
            {
                lstBusDay = _busDayRepo.GetOpenedBusinessDay(CurrentUser);
            };
            DateTime BDDateOperation = lstBusDay.FirstOrDefault().BDDateOperation;

            ViewBag.BusnessDayDate = BDDateOperation.ToString("yyyy-MM-dd");
            InjectUserConfigInSession();
            //return rPVResult;
            return View(ModelPendingStockReplacement(lstBusDay.FirstOrDefault().BranchID, BDDateOperation));
        }

        //[HttpPost]
        public JsonResult AddManager(StockReplacement StockReplacement)
        {
            bool status = false;
            string Message = "";
            try
            {
                List<StockReplacementLine> StockReplacementLines = new List<StockReplacementLine>();
                StockReplacementLines = (List<StockReplacementLine>)Session["StockReplacementLines"];
                StockReplacement.StockReplacementLines = StockReplacementLines;

                if (StockReplacement.StockReplacementID == 0)
                {
                    int StockReplacementID = _StockReplacementRepository.DoStockReplacement(StockReplacement, SessionGlobalPersonID).StockReplacementID;
                    status = true;
                    SimpleReset();
                    Message = Resources.Success + " Products Have Been Successfully Sended ";
                    return new JsonResult { Data = new { status = status, Message = Message } };
                }

                if (StockReplacement.StockReplacementID > 0)
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
                Message = "Error " + e.Message; // + " " + e.StackTrace + " " + e.InnerException;
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
                string trnnum = _transactNumbeRepository.displayTransactNumber("STRP", businessDay);

                _InfoList.Add(new
                {
                    StockReplacementReference = trnnum,
                    RegisteredByID = SessionGlobalPersonID,
                    StockReplacementDate = businessDay.BDDateOperation.ToString("yyyy-MM-dd")
                });
            }
            return Json(_InfoList, JsonRequestBehavior.AllowGet);
        }



        //[HttpPost]
        public ActionResult AddStockReplacementLine(StockReplacementLine StockReplacementLine)
        {
            bool status = false;
            string Message = "";
            try
            {
                StockReplacementLine.Product = db.Products.Find(StockReplacementLine.ProductID);
                StockReplacementLine.Localization = db.Localizations.Find(StockReplacementLine.LocalizationID);

                StockReplacementLine.LineUnitPrice = (StockReplacementLine.Product is Lens) ? (StockReplacementLine.LineUnitPrice / 2) : StockReplacementLine.LineUnitPrice;

                List<StockReplacementLine> StockReplacementLines = (List<StockReplacementLine>)Session["StockReplacementLines"];

                //il s'agit d'une modification alors on fait un drop and create
                if (StockReplacementLine.TMPID > 0)
                {
                    StockReplacementLine toRemove = StockReplacementLines.SingleOrDefault(pl => pl.TMPID == StockReplacementLine.TMPID);
                    StockReplacementLine.TMPID = 0;
                    StockReplacementLines.Remove(toRemove);
                }

                //alors la variable de session n'était pas vide
                if (StockReplacementLines != null && StockReplacementLines.Count > 0)
                {
                    //c'est un nouvel ajout dans le panier
                    if (StockReplacementLine.TMPID == 0)
                    {

                        Category catprod = db.Categories.Find(StockReplacementLine.Product.CategoryID);

                        int LensCategory = (catprod is LensCategory || catprod == null || catprod.isSerialNumberNull) ? 0 : 1;

                        StockReplacementLine existing = (LensCategory == 0) ? StockReplacementLines.SingleOrDefault(pl => pl.ProductID == StockReplacementLine.ProductID &&
                                                                                                         pl.LocalizationID == StockReplacementLine.LocalizationID && pl.Marque == StockReplacementLine.Marque && pl.NumeroSerie == StockReplacementLine.NumeroSerie
                                                                                                    ) : StockReplacementLines.SingleOrDefault(pl => pl.ProductID == StockReplacementLine.ProductID &&
                                                                                                        pl.LocalizationID == StockReplacementLine.LocalizationID
                                                                                                    );
                        //existe t-il déjà une ligne de vente ayant le meme produit et le même magasin que celui en création?
                        if (
                            existing != null && existing.TMPID > 0) //cette ligne exixte déjà
                        {
                            //la quantité est la somme des deux quantité
                            StockReplacementLine.LineQuantity += existing.LineQuantity;
                            //l'id c'est l'id de la ligne existante
                            StockReplacementLine.TMPID = existing.TMPID;
                            StockReplacementLine.StockReplacementLineID = existing.StockReplacementLineID;
                            //on retire l'ancien pour ajouter le nouveau
                            StockReplacementLines.Remove(existing);
                        }

                        if (existing == null || existing.TMPID == 0)//La ligne n'existe pas encore dans le panier
                        {
                            StockReplacementLine.TMPID = StockReplacementLines.Select(pl => pl.TMPID).Max() + 1;
                        }
                    }
                }

                //alors la variable de session était vide
                if (StockReplacementLines == null || StockReplacementLines.Count == 0)
                {
                    //c'est une nouvelle création pour la création
                    StockReplacementLines = new List<StockReplacementLine>();
                    StockReplacementLine.TMPID = 1;
                }
                bool isStockControl = (bool)Session["isStockControl"];
                bool res = true;
                if (isStockControl)
                {
                    //verifiction ds le stock source s'il ya assez de produit a StockReplacementer
                    if (StockReplacementLine.Product.Category.isSerialNumberNull) //frame
                    {
                        res = _productLocRepository.checkQtyInStock(StockReplacementLine.ProductID, StockReplacementLine.LocalizationID, StockReplacementLine.LineQuantity, StockReplacementLine.NumeroSerie, StockReplacementLine.Marque);
                    }
                    else
                    {
                        res = _productLocRepository.checkQtyInStock(StockReplacementLine.ProductID, StockReplacementLine.LocalizationID, StockReplacementLine.LineQuantity);
                    }

                }
                if (res)
                {
                    StockReplacementLines.Add(StockReplacementLine);
                    Session["StockReplacementLines"] = StockReplacementLines;
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
        public JsonResult RemoveStockReplacementLine(int TMPID)
        {
            List<StockReplacementLine> StockReplacementLines = (List<StockReplacementLine>)Session["StockReplacementLines"];

            if (StockReplacementLines != null && StockReplacementLines.Count > 0)
            {
                StockReplacementLine toRemove = StockReplacementLines.SingleOrDefault(pl => pl.TMPID == TMPID);
                StockReplacementLines.Remove(toRemove);

                Session["StockReplacementLines"] = StockReplacementLines;
            }

            var list = new
            {
                data = from pl in StockReplacementLines
                       select new
                       {
                           TMPID = pl.TMPID,
                           ProductLabel = (pl.Product is Lens) ? pl.Product.ProductCode : pl.Product.ProductDescription,
                           Localization = pl.Localization.LocalizationLabel,
                           LineQuantity = pl.LineQuantity,
                           LineUnitPrice = pl.LineUnitPrice,
                           StockReplacementReason = pl.StockReplacementReason
                       }
            };

            return Json(list, JsonRequestBehavior.AllowGet);
        }


        //Return salelines of current sale
        // [HttpPost]
        public JsonResult StockReplacementLines()
        {
            List<StockReplacementLine> StockReplacementLines = (List<StockReplacementLine>)Session["StockReplacementLines"];
            var list = new
            {
                data = from pl in StockReplacementLines
                       select new
                       {
                           TMPID = pl.TMPID,
                           ProductLabel = (pl.Product is Lens) ? pl.Product.ProductCode : pl.Product.ProductDescription,
                           Localization = pl.Localization.LocalizationLabel,
                           LineQuantity = pl.LineQuantity,
                           LineUnitPrice = pl.LineUnitPrice,
                           StockReplacementReason = pl.StockReplacementReason,
                           NumeroSerie = pl.NumeroSerie,
                           Marque = pl.Marque
                       }
            };

            return Json(list, JsonRequestBehavior.AllowGet);

        }



        private void SimpleReset()
        {
            List<StockReplacementLine> StockReplacementLines = new List<StockReplacementLine>();
            Session["StockReplacementLines"] = StockReplacementLines;

        }


        public JsonResult DeleteStockReplacement(int StockReplacementID)
        {
            bool status = false;
            string Message = "";
            try
            {
                _StockReplacementRepository.CancelStockReplacement(StockReplacementID);
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

        public List<StockReplacement> ModelPendingStockReplacement(int? BranchID, DateTime StockReplacementDate)
        {
            List<StockReplacement> list = new List<StockReplacement>();

            if (BranchID.HasValue && BranchID.Value > 0)
            {


                List<StockReplacement> dataTmp = (from pt in db.StockReplacements
                                               where (pt.BranchID == BranchID.Value && pt.StockReplacementDate == StockReplacementDate.Date)
                                               select pt).ToList();

                if (dataTmp != null && dataTmp.Count > 0)
                {

                    foreach (StockReplacement pt in dataTmp)
                    {
                        list.Add(
                            new StockReplacement
                            {
                                StockReplacementID = pt.StockReplacementID,
                                StockReplacementReference = pt.StockReplacementReference,
                                StockReplacementDate = pt.StockReplacementDate,
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
        public JsonResult GetAllPendingStockReplacements(int? BranchID, DateTime ReloadStockReplacementDate)
        {
            if (BranchID == null) BranchID = SessionBusinessDay(null).BranchID;

            var list = new
            {
                data = from pt in ModelPendingStockReplacement(BranchID, ReloadStockReplacementDate)
                       select new
                       {
                           StockReplacementID = pt.StockReplacementID,
                           StockReplacementReference = pt.StockReplacementReference,
                           StockReplacementDate = pt.StockReplacementDate.ToString("yyyy-MM-dd"),
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
            foreach (User user in db.People.OfType<User>().Where(u => u.IsConnected && u.ProfileID > 2 && u.UserAccessLevel > 1).ToArray())
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
            Category catprod = db.Categories.Find(ProductCategoryID);
            _InfoList.Add(new
            {
                // LensCategory = (catprod is LensCategory || catprod ==null || catprod.isSerialNumberNull) ? 0 : 1
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
            return (lstLensProduct == null) ? 0 : lstLensProduct.ProductID;

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
        public ActionResult GenerateReceipt(int StockReplacementID)
        {
            List<RptTransfertForm> model = new List<RptTransfertForm>();
            System.IO.Stream stream = new System.IO.MemoryStream();
            try
            {

                @ViewBag.CompanyLogoID = Company.GlobalPersonID;
                string defaultImgPath = Server.MapPath("~/Content/Images/App/deboy-logo.png");
                byte[] COMPANY_LOGO = System.IO.File.ReadAllBytes(defaultImgPath);

                string DeviseLabel = db.Devises.Where(d => d.DefaultDevise).FirstOrDefault().DeviseLabel;
                StockReplacement StockReplacement = (from pt in db.StockReplacements
                                               where pt.StockReplacementID == StockReplacementID
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
                db.StockReplacementLines.Where(l => l.StockReplacementID == StockReplacementID).ToList().ForEach(c =>
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
                                ProductLabel = c.StockReplacementReason,
                                ProductRef = (c.NumeroSerie == "" || c.NumeroSerie == null) ? c.Product.ProductCode : c.Product.ProductCode + " Ref:" + c.NumeroSerie + " Marque:" + c.Marque,//(c.OeilDroiteGauche == EyeSide.N) ? c.Product.ProductCode : c.OeilDroiteGauche + ":" + c.Product.ProductCode,
                                CompanyName = Company.Name,
                                CompanyAdress = "Head Quater : " + Company.Adress.Quarter.Town.Region.RegionLabel + " - " + Company.Adress.Quarter.Town.TownLabel,
                                CompanyTel = "Tel: " + Company.Adress.AdressPhoneNumber,
                                BranchName = curBranch.Branch.BranchName,
                                BranchAdress = curBranch.Branch.Adress.Quarter.QuarterLabel + " - " + curBranch.Branch.Adress.Quarter.Town.TownLabel,
                                BranchTel = "Tel: " + curBranch.Branch.Adress.AdressPhoneNumber,
                                Ref = (c.NumeroSerieDamage == "" || c.NumeroSerieDamage == null) ? c.ProductDamage.ProductCode : c.ProductDamage.ProductCode + " Ref:" + c.NumeroSerieDamage + " Marque:" + c.MarqueDamage,//StockReplacement.StockReplacementReference,
                                CompanyCNI = "NO CONT : " + Company.CNI,
                                Operator = CurrentUser.Name + " " + CurrentUser.Description,
                                SendindBranchName = StockReplacement.Branch.BranchName,
                                SendindBranchCode = StockReplacement.Branch.BranchCode,
                                TransfertDate = StockReplacement.StockReplacementDate.Date,
                                Title = "Product StockReplacementt lines informations",
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