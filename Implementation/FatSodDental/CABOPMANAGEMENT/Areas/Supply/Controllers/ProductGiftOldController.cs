using FatSod.Security.Abstracts;
using FatSod.Security.Entities;
using FatSod.Supply.Entities;
using VALDOZMANAGEMENT.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using FatSod.Supply.Abstracts;
using FatSod.Ressources;
using VALDOZMANAGEMENT.Filters;
using FatSod.Report.WrapReports;
using VALDOZMANAGEMENT.Tools;

namespace VALDOZMANAGEMENT.Areas.Supply.Controllers
{
    [Authorize(Order = 1)]
    [TakeBusinessDay(Order = 2)]
    public partial class ProductGiftOldController : BaseController
    {
        private bool isLens = false;
        //Current Controller and current page
        private const string CONTROLLER_NAME = "Supply/ProductGift";
        private const string VIEW_NAME = "Index";
        private IProductGift _ProductGiftRepository;
        private IBusinessDay _busDayRepo;
        private ITransactNumber _transactNumbeRepository;
        private IProductLocalization _productLocRepository;
        private ILensNumberRangePrice _priceRepository;
        
        //Construcitor

        private List<BusinessDay> lstBusDay;
        public ProductGiftOldController(IProductGift ProductGiftRepository,
            IBusinessDay bDRepo,
            ITransactNumber transactRepo,
            ILensNumberRangePrice priceRepo,
            IProductLocalization productLocRepo
            )
        {
            this._ProductGiftRepository = ProductGiftRepository;
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

            List<ProductGiftLine> ProductGiftLines = new List<ProductGiftLine>();
            Session["ProductGiftLines"] = ProductGiftLines;
            lstBusDay = (List<BusinessDay>)Session["UserBusDays"];
            if (lstBusDay == null)
            {
                lstBusDay = _busDayRepo.GetOpenedBusinessDay(CurrentUser);
            };

            DateTime BDDateOperation = lstBusDay.FirstOrDefault().BDDateOperation;
            ViewBag.BusnessDayDate = BDDateOperation.ToString("yyyy-MM-dd");
            ViewBag.CurrentBranch = lstBusDay.FirstOrDefault().BranchID;
            InjectUserConfigInSession();
            return View(ModelPendingProductGift(ViewBag.CurrentBranch, BDDateOperation));
        }

        //[HttpPost]
        public JsonResult AddManager(ProductGift ProductGift)
        {
            bool status = false;
            string Message = "";
            try
            {
                List<ProductGiftLine> ProductGiftLines = new List<ProductGiftLine>();
                ProductGiftLines = (List<ProductGiftLine>)Session["ProductGiftLines"];
                ProductGift.ProductGiftLines = ProductGiftLines;

                if (ProductGift.ProductGiftID == 0)
                {
                    int ProductGiftID = _ProductGiftRepository.DoProductGift(ProductGift, SessionGlobalPersonID).ProductGiftID;
                    status = true;
                    SimpleReset();
                    Message = Resources.Success + " Products Have Been Successfully Sended ";
                    return new JsonResult { Data = new { status = status, Message = Message } };
                }

                if (ProductGift.ProductGiftID > 0)
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
        public JsonResult AddProductGiftLine(ProductGiftLine ProductGiftLine)
        {
            bool status = false;
            string Message = "";
            try
            {
                ProductGiftLine.Product = db.Products.Find(ProductGiftLine.ProductID);
                ProductGiftLine.Localization = db.Localizations.Find(ProductGiftLine.LocalizationID);

                ProductGiftLine.LineUnitPrice = (ProductGiftLine.Product is Lens) ? (ProductGiftLine.LineUnitPrice / 2) : ProductGiftLine.LineUnitPrice;

                List<ProductGiftLine> ProductGiftLines = (List<ProductGiftLine>)Session["ProductGiftLines"];

                //il s'agit d'une modification alors on fait un drop and create
                if (ProductGiftLine.TMPID > 0)
                {
                    ProductGiftLine toRemove = ProductGiftLines.SingleOrDefault(pl => pl.TMPID == ProductGiftLine.TMPID);
                    ProductGiftLine.TMPID = 0;
                    ProductGiftLines.Remove(toRemove);
                }

                //alors la variable de session n'était pas vide
                if (ProductGiftLines != null && ProductGiftLines.Count > 0)
                {
                    //c'est un nouvel ajout dans le panier
                    if (ProductGiftLine.TMPID == 0)
                    {
                        ProductGiftLine existing = ProductGiftLines.SingleOrDefault(pl => pl.ProductID == ProductGiftLine.ProductID &&
                                                                                                        pl.LocalizationID == ProductGiftLine.LocalizationID
                                                                                                    );
                        //existe t-il déjà une ligne de vente ayant le meme produit et le même magasin que celui en création?
                        if (/*ProductGiftLines.Contains(ProductGiftLine) && ProductGiftLine.TMPID > 0*/
                            existing != null && existing.TMPID > 0) //cette ligne exixte déjà
                        {
                            //la quantité est la somme des deux quantité
                            ProductGiftLine.LineQuantity += existing.LineQuantity;
                            //l'id c'est l'id de la ligne existante
                            ProductGiftLine.TMPID = existing.TMPID;
                            ProductGiftLine.ProductGiftLineID = existing.ProductGiftLineID;
                            //on retire l'ancien pour ajouter le nouveau
                            ProductGiftLines.Remove(existing);
                        }

                        if (existing == null || existing.TMPID == 0)//La ligne n'existe pas encore dans le panier
                        {
                            ProductGiftLine.TMPID = ProductGiftLines.Select(pl => pl.TMPID).Max() + 1;
                        }
                    }
                }

                //alors la variable de session était vide
                if (ProductGiftLines == null || ProductGiftLines.Count == 0)
                {
                    //c'est une nouvelle création pour la création
                    ProductGiftLines = new List<ProductGiftLine>();
                    ProductGiftLine.TMPID = 1;
                }

                bool isStockControl = (bool)Session["isStockControl"];
                bool res = true;
                if (isStockControl)
                {
                    //verifiction ds le stock source s'il ya assez de produit a ProductGifter
                    res = _productLocRepository.checkQtyInStock(ProductGiftLine.ProductID, ProductGiftLine.LocalizationID, ProductGiftLine.LineQuantity, ProductGiftLine.NumeroSerie, ProductGiftLine.Marque);
                }
                if (res)
                {
                    ProductGiftLines.Add(ProductGiftLine);
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

        //[HttpPost]
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
                           ProductLabel = (pl.Product is Lens) ? pl.Product.ProductCode : pl.Product.ProductLabel,
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
                           ProductLabel = (pl.Product is Lens) ? pl.Product.ProductCode : pl.Product.ProductLabel,
                           Localization = pl.Localization.LocalizationLabel,
                           LineQuantity = pl.LineQuantity,
                           LineUnitPrice = pl.LineUnitPrice,
                           ProductGiftReason = pl.ProductGiftReason
                       }
            };

            return Json(list, JsonRequestBehavior.AllowGet);
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
                _ProductGiftRepository.CancelProductGift(ProductGiftID);
                status = true;
                Message = Resources.Success + " product Damage Has Been Successfully Deleted ";
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
                           RegisteredBy = pt.RegisteredBy.UserFullName,
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
            foreach (User user in db.People.OfType<User>().Where(u => u.IsConnected && u.UserAccessLevel <= 5).ToArray())
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
            //List<object> ProductCategory = new List<object>();

            //foreach (Category productcat in db.Categories.ToList())
            //{
            //    ProductCategory.Add(new
            //    {
            //        CategoryCode = productcat.CategoryCode,
            //        CategoryID = productcat.CategoryID
            //    });
            //}

            //return Json(ProductCategory, JsonRequestBehavior.AllowGet);

            List<object> categoryList = new List<object>();
            List<Category> categories = LoadComponent.GetAllGenericCategories();
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
                LensCategory = (catprod is LensCategory) ? 0 : 1
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
                            ProductRef = c.Product.ProductCode,//(c.OeilDroiteGauche == EyeSide.N) ? c.Product.ProductCode : c.OeilDroiteGauche + ":" + c.Product.ProductCode,
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
                            Title = "Product ProductGiftt lines informations",
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