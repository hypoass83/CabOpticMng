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
using CABOPMANAGEMENT.Areas.Sale.Models;
using System.IO;
using FatSod.Ressources;
using FatSod.Report.WrapReports;

namespace CABOPMANAGEMENT.Areas.Supply.Controllers
{
    [Authorize(Order = 1)]
    [TakeBusinessDay(Order = 2)]
    public partial class RegProductNumberController : BaseController
    {
        private bool isLens = false;
        //Current Controller and current page
        private const string CONTROLLER_NAME = "Supply/RegProductNumber" ;
        private const string VIEW_NAME = "Index";
        private IRegProductNumber _RegProductNumberRepository;
        private IBusinessDay _busDayRepo;
        private ITransactNumber _transactNumbeRepository;
        private IProductLocalization _productLocRepository;
        private ILensNumberRangePrice _priceRepository;
        //private IEquipmentPrice _equipriceRepository;
        //Construcitor

        private List<BusinessDay> lstBusDay;
        public RegProductNumberController(IRegProductNumber RegProductNumberRepository,
            //IEquipmentPrice equipriceRepository,
            IBusinessDay bDRepo,
            ITransactNumber transactRepo,
            ILensNumberRangePrice priceRepo,
            IProductLocalization productLocRepo
            )
        {
            //this._equipriceRepository = equipriceRepository;
            this._RegProductNumberRepository = RegProductNumberRepository;
            this._busDayRepo = bDRepo;
            this._transactNumbeRepository = transactRepo;
            this._priceRepository = priceRepo;
            this._productLocRepository = productLocRepo;
        }
        [OutputCache(Duration=3600)]
        public ActionResult Index()
        {
           

            Session["isUpdate"] = false;
            Session["MaxValue"] = 500d;
            Session["SafetyStock"] = 500d;
            Session["EquipPrice"] = 0d;
            Session["valeur"] = 1d;
            Session["EquipmentPriceUnit"] = null;
            Session["isApplyToCalculate"] = false;

            List<RegProductNumberLine> RegProductNumberLines = new List<RegProductNumberLine>();
            Session["RegProductNumberLines"] = RegProductNumberLines;
            lstBusDay = (List<BusinessDay>)Session["UserBusDays"];
            if (lstBusDay == null)
            {
                lstBusDay = _busDayRepo.GetOpenedBusinessDay(CurrentUser);
            };
            DateTime BDDateOperation = lstBusDay.FirstOrDefault().BDDateOperation;
            
            ViewBag.BusnessDayDate = BDDateOperation.ToString("yyyy-MM-dd");
            ViewBag.CurrentBranch = lstBusDay.FirstOrDefault().BranchID;

            return View(ModelPendingRegProductNumber(ViewBag.CurrentBranch, BDDateOperation));
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

        //public JsonResult GetEquipmentPriceUnits()
        //{

        //    List<object> EquipmentPriceUnitList = new List<object>();

        //    foreach (EquipmentPriceUnit epu in db.EquipmentPriceUnits.ToList())
        //    {
        //        EquipmentPriceUnitList.Add(new
        //        {
        //            Decription = epu.Decription,
        //            EquipmentPriceUnitID = epu.EquipmentPriceUnitID
        //        });
        //    }

        //    return Json(EquipmentPriceUnitList, JsonRequestBehavior.AllowGet);

        //}

        //public ActionResult GetEquipmentPrice(int? EquipmentPriceUnit, int? CurrentProduct)
        //{
        //    List<object> _InfoList = new List<object>();
        //    if ((!CurrentProduct.HasValue || CurrentProduct.Value <= 0) || (!EquipmentPriceUnit.HasValue || EquipmentPriceUnit.Value <= 0))
        //    {
        //        return Json(_InfoList, JsonRequestBehavior.AllowGet);
        //    }
        //    double priceEquip = 0d;
        //    double valeur = 1d;

        //    Product product = db.Products.Find(CurrentProduct.Value);
        //    bool productIsLens = product is Lens;
        //    if (!productIsLens)
        //    {
        //        EquipmentPrice price = _equipriceRepository.GetPrice(product.ProductID, EquipmentPriceUnit.Value);
        //        if (price != null && price.EquipmentPriceUnitID > 0)
        //        {
        //            Session["isApplyToCalculate"] = true;
        //            priceEquip = price.Price;
        //            valeur = price.EquipmentPriceUnit.Valeur;
        //            Session["EquipmentPriceUnit"] = price.EquipmentPriceUnitID;
        //        }
        //        else
        //        {
        //            Session["isApplyToCalculate"] = false;
        //            priceEquip = (double)Session["EquipPrice"];
        //            valeur = 1d;
        //            Session["EquipmentPriceUnit"] = null;
        //        }
        //        Session["valeur"] = valeur;
        //    }

        //    _InfoList.Add(new
        //    {
        //        LineUnitPrice = priceEquip

        //    });
        //    return Json(_InfoList, JsonRequestBehavior.AllowGet);
        //}

        public JsonResult GetProductCategory()
        {
            List<object> ProductCategory = new List<object>();

            foreach (LensCategory productcat in db.LensCategories.ToList())
            {
                ProductCategory.Add(new
                {
                    CategoryCode = productcat.CategoryCode,
                    CategoryID = productcat.CategoryID
                });
            }

            return Json(ProductCategory, JsonRequestBehavior.AllowGet);
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

        public ActionResult OnProductSelected(int? Localization, int? CurrentProduct)
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

                    string categoryCode = catprod.CategoryCode;

                    if (categoryCode.ToLower().Contains("SV".ToLower()))
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

                    if (categoryCode.ToLower().Contains("BF".ToLower()) || categoryCode.ToLower().Contains("PRO".ToLower()))
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
                LensNumberRangePrice price = _priceRepository.GetPrice(lensId);
                Session["LineUnitPrice"] = (price != null && price.LensNumberRangePriceID > 0) ? price.Price : 0;
            }

            _InfoList.Add(new
            {
                LineUnitPrice = Session["LineUnitPrice"],
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
            Session["LineUnitPrice"] = (lstLensProduct == null) ? 0 : lstLensProduct.ProductLocalizationStockSellingPrice;
            Session["CurrentProduct"] = (lstLensProduct == null) ? "" : lstLensProduct.ProductCode;
            Session["CurrentProductID"] = (lstLensProduct == null) ? 0 : lstLensProduct.ProductID;
            Session["SafetyStock"] = (lstLensProduct == null) ? 0 : lstLensProduct.ProductLocalizationSafetyStockQuantity;
            return (lstLensProduct == null) ? 0 : lstLensProduct.ProductID;

        }
        
        //[HttpPost]
        public JsonResult AddManager(RegProductNumber RegProductNumber)
        {
            bool status = false;
            string Message = "";
            try
            {
                List<RegProductNumberLine> RegProductNumberLines = new List<RegProductNumberLine>();
                RegProductNumberLines = (List<RegProductNumberLine>)Session["RegProductNumberLines"];
                RegProductNumber.RegProductNumberLines = RegProductNumberLines;
                RegProductNumber.RegisteredByID = SessionGlobalPersonID;
                if (RegProductNumber.RegProductNumberID == 0)
                {
                    int RegProductNumberID = _RegProductNumberRepository.DoRegProductNumber(RegProductNumber, SessionGlobalPersonID).RegProductNumberID;
                    Session["RegProductNumberLines"] = new List<RegProductNumberLine>();
                    Message = Resources.Success+ " Products Have Been Successfully Sended ";
                    status = true;
                }
                else if (RegProductNumber.RegProductNumberID > 0)
                {
                    Session["RegProductNumberLines"] = new List<RegProductNumberLine>();
                    status = true;
                    Message = Resources.er_alert_danger+ "This operation not yet possible ";
                    //return UpdateRegProductNumber(RegProductNumber);
                }

            }
            catch (Exception ex)
            {
                status = false;
                Message = Resources.er_alert_danger + " - Products Have Been not Successfully Updated because " + ex.Message;

            }
            return new JsonResult { Data = new { status = status, Message = Message } };
        }

        /*
        public ActionResult InitTrnNumber(int? BranchID)
        {
            if (BranchID.HasValue && BranchID.Value > 0)
            {
                lstBusDay = (List<BusinessDay>)Session["UserBusDays"];
                if (lstBusDay == null)
                {
                    lstBusDay = _busDayRepo.GetOpenedBusinessDay(CurrentUser);
                };
                BusinessDay businessDay = lstBusDay.FirstOrDefault(l => l.BranchID == BranchID.Value);
                string trnnum = _transactNumbeRepository.displayTransactNumber("CPNU", businessDay);
                this.GetCmp<TextField>("RegProductNumberReference").Value = trnnum;
                this.GetCmp<TextField>("RegisteredByID").Value = SessionGlobalPersonID;
            }
            return this.Direct();
        }

        [HttpPost]
        public ActionResult DoRegProductNumber(RegProductNumber RegProductNumber)
        {
            try
            {

                int RegProductNumberID= _RegProductNumberRepository.DoRegProductNumber(RegProductNumber,SessionGlobalPersonID).RegProductNumberID;
                Session["RegProductNumberID"] = RegProductNumberID;
                this.GetCmp<Button>("btnPrint").Disabled=false;
                this.AlertSucces(Resources.Success, "Products Have Been Successfully Sended ");
                return this.Reset();
            }
            catch (Exception ex)
            {

                X.Msg.Alert(Resources.er_alert_danger, "Products Have not Been Successfully Send because " + ex.Message + " " + ex.StackTrace).Show();
                return this.Direct();
            }
        }

        [HttpPost]
        public ActionResult UpdateRegProductNumber(RegProductNumber RegProductNumber)
        {
            try
            {
                int RegProductNumberID= _RegProductNumberRepository.UpdateRegProductNumber(RegProductNumber).RegProductNumberID;
                Session["RegProductNumberID"] = RegProductNumberID;
                this.GetCmp<Button>("btnPrint").Disabled = false;
                this.AlertSucces(Resources.Success, "Products Have Been Successfully Updated ");
                
                return this.Reset();
            }
            catch (Exception ex)
            {

                X.Msg.Alert(Resources.er_alert_danger, "Products Have Been not Successfully Updated because " + ex.Message + " " + ex.StackTrace).Show();
                return this.Direct();
            }
        }
        */
       
        //[HttpPost]
        public JsonResult AddRegProductNumberLine(RegProductNumberLine RegProductNumberLine)
        {
            bool status = false;
            string Message = "";
            try 
            { 
                RegProductNumberLine.OldProduct = db.Products.Find(RegProductNumberLine.OldProductID);
                RegProductNumberLine.Localization = db.Localizations.Find(RegProductNumberLine.LocalizationID);
                RegProductNumberLine.NewProduct = db.Products.Find(RegProductNumberLine.NewProductID);

                List<RegProductNumberLine> RegProductNumberLines = (List<RegProductNumberLine>)Session["RegProductNumberLines"];

                //il s'agit d'une modification alors on fait un drop and create
                if (RegProductNumberLine.TMPID > 0)
                {
                    RegProductNumberLine toRemove = RegProductNumberLines.SingleOrDefault(pl => pl.TMPID == RegProductNumberLine.TMPID);
                    RegProductNumberLine.TMPID = 0;
                    RegProductNumberLines.Remove(toRemove);
                }

                //alors la variable de session n'était pas vide
                if (RegProductNumberLines != null && RegProductNumberLines.Count > 0)
                {
                    //c'est un nouvel ajout dans le panier
                    if (RegProductNumberLine.TMPID == 0)
                    {
                        RegProductNumberLine existing = RegProductNumberLines.SingleOrDefault(pl => pl.OldProductID == RegProductNumberLine.OldProductID && pl.NewProductID == RegProductNumberLine.NewProductID &&
                                                                                                        pl.LocalizationID == RegProductNumberLine.LocalizationID 
                                                                                                    );
                        //existe t-il déjà une ligne de vente ayant le meme produit et le même magasin que celui en création?
                        if (
                            existing != null && existing.TMPID > 0) //cette ligne exixte déjà
                        {
                            //la quantité est la somme des deux quantité
                            RegProductNumberLine.NewLineQuantity += existing.NewLineQuantity;
                            //l'id c'est l'id de la ligne existante
                            RegProductNumberLine.TMPID = existing.TMPID;
                            RegProductNumberLine.RegProductNumberLineID = existing.RegProductNumberLineID;
                            //on retire l'ancien pour ajouter le nouveau
                            RegProductNumberLines.Remove(existing);
                        }

                        if (existing == null || existing.TMPID == 0)//La ligne n'existe pas encore dans le panier
                        {
                            RegProductNumberLine.TMPID = RegProductNumberLines.Select(pl => pl.TMPID).Max() + 1;
                        }
                    }
                }

                //alors la variable de session était vide
                if (RegProductNumberLines == null || RegProductNumberLines.Count == 0)
                {
                    //c'est une nouvelle création pour la création
                    RegProductNumberLines = new List<RegProductNumberLine>();
                    RegProductNumberLine.TMPID = 1;
                }
                //verifiction ds le stock source s'il ya assez de produit a RegProductNumberer
                bool res = _productLocRepository.checkQtyInStock(RegProductNumberLine.OldProductID, RegProductNumberLine.LocalizationID, RegProductNumberLine.NewLineQuantity);
                if (res)
                {
                    RegProductNumberLines.Add(RegProductNumberLine);
                    Session["RegProductNumberLines"] = RegProductNumberLines;
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
            catch (Exception ex)
            {
                status = false;
                Message = Resources.er_alert_danger+ " - Products Have Been not Successfully Updated because " + ex.Message;
               
            }
            return new JsonResult { Data = new { status = status, Message = Message } };
        }

        
        //[HttpPost]
        public JsonResult RemoveRegProductNumberLine(int TMPID)
        {
            List<RegProductNumberLine> RegProductNumberLines = (List<RegProductNumberLine>)Session["RegProductNumberLines"];

            if (RegProductNumberLines != null && RegProductNumberLines.Count > 0)
            {
                RegProductNumberLine toRemove = RegProductNumberLines.SingleOrDefault(pl => pl.TMPID == TMPID);
                RegProductNumberLines.Remove(toRemove);

                Session["RegProductNumberLines"] = RegProductNumberLines;
            }

            var list = new
            {
                data = from pl in RegProductNumberLines
                select new
                {
                    TMPID = pl.TMPID,
                    OldProductLabel = (pl.OldProduct is Lens) ? pl.OldProduct.ProductCode : pl.OldProduct.ProductDescription,
                    NewProductLabel = (pl.NewProduct is Lens) ? pl.NewProduct.ProductCode : pl.NewProduct.ProductDescription,
                    Localization = pl.Localization.LocalizationLabel,
                    NewLineQuantity = pl.NewLineQuantity
                }
            };

            return Json(list, JsonRequestBehavior.AllowGet);
        }
        /*
        [HttpPost]
        public ActionResult Reset2()
        {
            SimpleReset2();
            return this.Direct();
        }

        [HttpPost]
        public ActionResult UpdateRegProductNumberLine(int TMPID)
        {
            this.InitializeRegProductNumberLineFields(TMPID);
            return this.Direct();
        }

        private void SimpleReset2()
        {

            this.GetCmp<ComboBox>("OldProductNumberID").Value = "";
            this.GetCmp<ComboBox>("NewProductNumberID").Value = "";
            this.GetCmp<ComboBox>("OldProductID").Value = "";
            this.GetCmp<ComboBox>("NewProductID").Value = "";
            this.GetCmp<NumberField>("StockQuantity").Value = "";
            this.GetCmp<NumberField>("NewLineQuantity").Value = "";
           
            this.GetCmp<Store>("RegProductNumberLinesStore").Reload();

            ManageCady();
        }
        */
        //Return salelines of current sale
        //[HttpPost]
        public JsonResult RegProductNumberLines()
        {
            List<RegProductNumberLine> RegProductNumberLines = (List<RegProductNumberLine>)Session["RegProductNumberLines"];
            //List<object> list = new List<object>();

            //foreach (RegProductNumberLine pl in RegProductNumberLines)
            //{
            var list = new
            {
                data = from pl in RegProductNumberLines
                select new
                {
                    TMPID = pl.TMPID,
                    OldProductLabel = (pl.OldProduct is Lens) ? pl.OldProduct.ProductCode : pl.OldProduct.ProductDescription,
                    NewProductLabel = (pl.NewProduct is Lens) ? pl.NewProduct.ProductCode : pl.NewProduct.ProductDescription,
                    Localization = pl.Localization.LocalizationLabel,
                    NewLineQuantity = pl.NewLineQuantity
                }
            };
                
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        

        //public JsonResult DeleteRegProductNumber(int RegProductNumberID)
        //{
        //    bool status = false;
        //    string Message = "";
        //    try
        //    {
        //        _RegProductNumberRepository.CancelRegProductNumber(RegProductNumberID,SessionGlobalPersonID);
        //        status = true;
        //        Message = Resources.Success + " product Number regularization Has Been Successfully Deleted ";

        //    }
        //    catch (Exception ex)
        //    {
        //        status = false;
        //        Message = Resources.er_alert_danger+ " - Product Has Not Been Successfully Deleted Because " + ex.Message + " " + ex.StackTrace;
        //    }
        //    return new JsonResult { Data = new { status = status, Message = Message } };
        //}

        
        public JsonResult ChangeBusDay(int? BranchID)
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
                string trnnum = _transactNumbeRepository.displayTransactNumber("CPNU", businessDay);
                _InfoList.Add(new
                {
                    RegProductNumberReference = trnnum,
                    RegisteredByID = SessionGlobalPersonID,
                    RegProductNumberDate = businessDay.BDDateOperation.ToString("yyyy-MM-dd")
                });
            }
            return Json(_InfoList, JsonRequestBehavior.AllowGet);
        }
       
        public List<RegProductNumber> ModelPendingRegProductNumber(int? BranchID, DateTime RegProductNumberDate)
        {
            List<RegProductNumber> list = new List<RegProductNumber>();

            if (BranchID.HasValue && BranchID.Value > 0)
            {

                
                List<RegProductNumber> dataTmp = (from pt in db.RegProductNumbers
                                                      where ( pt.BranchID==BranchID.Value && pt.RegProductNumberDate == RegProductNumberDate.Date)
                                                      select pt).ToList();

                if (dataTmp != null && dataTmp.Count > 0)
                {

                    foreach (RegProductNumber pt in dataTmp)
                    {
                        list.Add(
                            new RegProductNumber
                            {
                                RegProductNumberID = pt.RegProductNumberID,
                                RegProductNumberReference = pt.RegProductNumberReference,
                                RegProductNumberDate = pt.RegProductNumberDate,
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
        public JsonResult GetAllPendingRegProductNumbers(int? BranchID, DateTime ReloadRegProductNumberDate)
        {
            if (BranchID == null) BranchID = SessionBusinessDay(null).BranchID;
            var model = new
            {
                data = from pt in ModelPendingRegProductNumber(BranchID, ReloadRegProductNumberDate)
                select new
                {
                    RegProductNumberID = pt.RegProductNumberID,
                    RegProductNumberReference = pt.RegProductNumberReference,
                    RegProductNumberDate = pt.RegProductNumberDate.ToString("yyyy-MM-dd"),
                    Branch = pt.Branch.BranchName,
                    AutorizedByID = pt.AutorizedBy.Name,
                    //RegisteredByID = pt.RegisteredBy.Name
                }
            };
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /*
        /// <summary>
        /// Liste des magasins qui sont approvisionnés dans une branche
        /// </summary>
        /// <param name="BranchID"></param>
        /// <returns></returns>
        public StoreResult GetAllStockedLocations(int BranchID)
        {
            List<object> list = new List<object>();
            
            if (BranchID >0)
            {
                List<Localization> dataTmp = (from loc in db.Localizations
                                              where (loc.BranchID == BranchID)
                                              select loc).ToList();
                if (dataTmp.Count>0)
                {
                    foreach (Localization pt in dataTmp)
                    {
                        list.Add(new
                        {
                            LocalizationID = pt.LocalizationID,
                            LocalizationLabel = pt.LocalizationLabel,
                        });
                    }
                }
                
            }
           
            return this.Store(list);
        }



        public void InitializeRegProductNumberLineFields(int ID)
        {
            Session["isUpdate"] = true;
            
            this.SimpleReset2();

            List<RegProductNumberLine> RegProductNumberLines = (List<RegProductNumberLine>)Session["RegProductNumberLines"];

            if (ID > 0)
            {
                RegProductNumberLine ptl = new RegProductNumberLine();
                ptl = RegProductNumberLines.SingleOrDefault(pl => pl.TMPID == ID);

                this.GetCmp<TextField>("RegProductNumberLineID").Value = ptl.RegProductNumberLineID;
                this.GetCmp<TextField>("TMPID").Value = ptl.TMPID;

                this.GetCmp<ComboBox>("LocalizationID").GetStore().Reload();
                this.GetCmp<ComboBox>("LocalizationID").Value = ptl.LocalizationID;
                
                this.GetCmp<ComboBox>("ProductCategoryID").SetValue(ptl.OldProduct.CategoryID);

                this.GetCmp<ComboBox>("NewProductCategoryID").SetValue(ptl.NewProduct.CategoryID);

                if (ptl.OldProduct is Lens || ptl.NewProduct is Lens)
                {
                    this.GetCmp<ComboBox>("OldProductNumberID").Disabled = false;
                    this.GetCmp<ComboBox>("NewProductNumberID").Disabled = false;

                    LensNumber Oldnumber = db.Lenses.Find(ptl.OldProductID).LensNumber;
                    Session["OldLensNumberFullCode"] = Oldnumber.LensNumberFullCode;
                    this.GetCmp<ComboBox>("OldProductNumberID").SetValue(Oldnumber.LensNumberID);

                    LensNumber Newnumber = db.Lenses.Find(ptl.NewProductID).LensNumber;
                    Session["NewLensNumberFullCode"] = Newnumber.LensNumberFullCode;
                    this.GetCmp<ComboBox>("NewProductNumberID").SetValue(Newnumber.LensNumberID);

                }

                this.GetCmp<ComboBox>("OldProductID").GetStore().Reload();
                this.GetCmp<ComboBox>("OldProductID").Value = ptl.OldProductID;

                this.GetCmp<ComboBox>("NewProductID").GetStore().Reload();
                this.GetCmp<ComboBox>("NewProductID").Value = ptl.NewProductID;

                double qtystock = GetMaxQtyInStock(ptl.OldProductID, ptl.LocalizationID);
                this.GetCmp<NumberField>("StockQuantity").Value = qtystock;
                Session["MaxValue"] = qtystock;

                this.GetCmp<ComboBox>("NewLineQuantity").Value = ptl.NewLineQuantity;

            }
            ManageCady();

        }

        public double GetMaxQtyInStock(int productID, int localizationID)
        {
            double res = 0;

          
            res = db.ProductLocalizations.SingleOrDefault(pl => pl.ProductID == productID && pl.LocalizationID == localizationID).ProductLocalizationStockQuantity;

            return res;
        }


        public void ManageCady()
        {
            List<RegProductNumberLine> RegProductNumberLines = (List<RegProductNumberLine>)Session["RegProductNumberLines"];

            if (RegProductNumberLines != null && RegProductNumberLines.Count > 0)
            {
                this.GetCmp<TextField>("IsCadyEmpty").SetValue(0);//faux
            }
            if (RegProductNumberLines == null || RegProductNumberLines.Count == 0)
            {
                this.GetCmp<TextField>("IsCadyEmpty").SetValue(1);//vrai
            }
        }

        public ActionResult DisableNumero(int ProductCategoryID)
        {
            Category catprod = db.Categories.Find(ProductCategoryID);
            if (catprod is LensCategory)
            {
                this.GetCmp<ComboBox>("OldProductNumberID").Disabled = false;
                this.GetCmp<ComboBox>("OldProductID").HideTrigger = true;

                this.GetCmp<ComboBox>("NewProductNumberID").Disabled = false;
                this.GetCmp<ComboBox>("NewProductID").HideTrigger = true;
            }
            else
            {
                this.GetCmp<ComboBox>("OldProductNumberID").Disabled = true;
                this.GetCmp<ComboBox>("OldProductID").HideTrigger = false;

                this.GetCmp<ComboBox>("NewProductNumberID").Disabled = true;
                this.GetCmp<ComboBox>("NewProductID").HideTrigger = false;
            }
            return this.Direct();
        }

        public ActionResult GetAllPagingOldNumbers(int start, int limit, int page, string query, int? ProductCategory, int? localization)
        {
            bool isUpdate = (bool)Session["isUpdate"];
            string LensNumberFullCode = (string)Session["OldLensNumberFullCode"];

            query = (isUpdate == true) ? LensNumberFullCode : query;

            Paging<LensNumber> numbers = GetAllNumbers(start, limit, "", "", query, ProductCategory, localization);

            Session["isUpdate"] = false;
            Session["OldLensNumberFullCode"] = "*";

            return this.Store(numbers.Data, numbers.TotalRecords);
        }
        public ActionResult GetAllPagingNewNumbers(int start, int limit, int page, string query, int? ProductCategory, int? localization)
        {
            bool isUpdate = (bool)Session["isUpdate"];
            string LensNumberFullCode = (string)Session["NewLensNumberFullCode"];

            query = (isUpdate == true) ? LensNumberFullCode : query;

            Paging<LensNumber> numbers = GetAllNumbers(start, limit, "", "", query, ProductCategory, localization);

            Session["isUpdate"] = false;
            Session["NewLensNumberFullCode"] = "*";

            return this.Store(numbers.Data, numbers.TotalRecords);
        }

        public Paging<LensNumber> GetAllNumbers(int start, int limit, string sort, string dir, string filter, int? ProductCategory, int? localization)
        {

            List<LensNumber> numbers = ModelLensNumber(ProductCategory.Value, localization.Value);

            if (!string.IsNullOrEmpty(filter) && filter != "*")
            {
                numbers.RemoveAll(number => !number.LensNumberFullCode.ToLower().StartsWith(filter.ToLower()));
            }

            if (!string.IsNullOrEmpty(sort))
            {
                numbers.Sort(delegate(LensNumber x, LensNumber y)
                {
                    object a;
                    object b;

                    int direction = dir == "DESC" ? -1 : 1;

                    a = x.GetType().GetProperty(sort).GetValue(x, null);
                    b = y.GetType().GetProperty(sort).GetValue(y, null);

                    return CaseInsensitiveComparer.Default.Compare(a, b) * direction;
                });
            }

            if ((start + limit) > numbers.Count)
            {
                limit = numbers.Count - start;
            }

            List<LensNumber> rangePlants = (start < 0 || limit < 0) ? numbers : numbers.GetRange(start, limit);

            return new Paging<LensNumber>(rangePlants, numbers.Count);

        }


        public List<LensNumber> ModelLensNumber(int ProductCategoryID, int Localization)
        {

            List<LensNumber> model = new List<LensNumber>();
            //produit de verre
            try
            {
                
               
                Category catprod = db.Categories.Find(ProductCategoryID);

                isLens = (catprod is LensCategory);

                if (isLens)
                {

                    string categoryCode = catprod.CategoryCode;
                    List<LensNumber> lens = new List<LensNumber>();

                    if (categoryCode.ToLower().Contains("SV".ToLower()))
                    {
                        LoadComponent.GetAllSVNumbers().ForEach(ln =>
                        {

                            LensNumber ln2 = new LensNumber()
                            {
                                LensNumberAdditionValue = ln.LensNumberAdditionValue,
                                LensNumberCylindricalValue = ln.LensNumberCylindricalValue,
                                LensNumberDescription = ln.LensNumberDescription,
                                LensNumberID = ln.LensNumberID,
                                LensNumberSphericalValue = ln.LensNumberSphericalValue
                            };

                            model.Add(ln2);
                        });
                    }

                    if (categoryCode.ToLower().Contains("BF".ToLower()) || categoryCode.ToLower().Contains("PRO".ToLower()))
                    {
                        LoadComponent.GetAllNumbers().ForEach(ln =>
                        {

                            LensNumber ln2 = new LensNumber()
                            {
                                LensNumberAdditionValue = ln.LensNumberAdditionValue,
                                LensNumberCylindricalValue = ln.LensNumberCylindricalValue,
                                LensNumberDescription = ln.LensNumberDescription,
                                LensNumberID = ln.LensNumberID,
                                LensNumberSphericalValue = ln.LensNumberSphericalValue
                            };

                            model.Add(ln2);
                        });
                    }

                }

            }
            catch (Exception e)
            {

                throw e;
            }
            return model;
        }


        [HttpPost]
        public ActionResult OnOldLensNumberSelected(int? Localization, int? ProductNumberID, int? ProductCategoryID)
        {
            try
            {

                if ((Localization.HasValue && Localization.Value > 0) && (ProductNumberID.HasValue && ProductNumberID.Value > 0) && (ProductCategoryID.HasValue && ProductCategoryID.Value > 0))
                {
                    this.GetCmp<ComboBox>("OldProductID").SetValueAndFireSelect(GetLensID(Localization.Value, ProductCategoryID.Value, ProductNumberID.Value));
                }
                else
                {
                    X.Msg.Alert(Resources.Productlabel, "This Product does not exist").Show();
                }

                return this.Direct();
            }
            catch (Exception e)
            {
                statusOperation = Resources.er_alert_danger + e.Message + "Méthode = OnOldLensNumberSelected() " + e.TargetSite;
                X.Msg.Alert(Resources.Productlabel, statusOperation).Show();
                return this.Direct();
            }
        }

        [HttpPost]
        public ActionResult OnNewLensNumberSelected(int? Localization, int? ProductNumberID, int? ProductCategoryID)
        {
            try
            {

                if ((Localization.HasValue && Localization.Value > 0) && (ProductNumberID.HasValue && ProductNumberID.Value > 0) && (ProductCategoryID.HasValue && ProductCategoryID.Value > 0))
                {
                    this.GetCmp<ComboBox>("NewProductID").SetValueAndFireSelect(GetLensID(Localization.Value, ProductCategoryID.Value, ProductNumberID.Value));
                }
                else
                {
                    X.Msg.Alert(Resources.Productlabel, "This Product does not exist").Show();
                }

                return this.Direct();
            }
            catch (Exception e)
            {
                statusOperation = Resources.er_alert_danger + e.Message + "Méthode = OnNewLensNumberSelected() " + e.TargetSite;
                X.Msg.Alert(Resources.Productlabel, statusOperation).Show();
                return this.Direct();
            }
        }

        public int GetLensID(int LocalizationID, int ProductCategoryID, int ProductNumberID)
        {
            
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
                        }).FirstOrDefault();

            Session["MaxValue"] = lstLensProduct.ProductQuantity;
            Session["LineUnitPrice"] = lstLensProduct.ProductLocalizationStockSellingPrice;

            return lstLensProduct.ProductID;

        }

        [HttpPost]
        public ActionResult GetAllProducts(int? LocalizationID, int? ProductCategoryID, int? ProductNumberID)
        {

            try
            {
                List<Product> list = ModelProductLocalCat(LocalizationID, ProductCategoryID, ProductNumberID);

                if (list == null || list.Count == 0)
                {
                    X.Msg.Alert("Product Stock Error", "You Don't Have this product in the selected Warehoure! Please See Administrator").Show();
                    return this.Direct();
                }

                return this.Store(list);
            }
            catch (Exception exc)
            {
                X.Msg.Alert("Price Error ", "Message = " + exc.Message + " Inner Exception = " + exc.InnerException).Show();
                return this.Direct();
            }

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
                //LensCategory lenprod = db.LensCategories.Find(ProductCategoryID);

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


        [HttpPost]
        public ActionResult OnProductSelected(int? Localization, int? CurrentProduct)
        {
            
            try
            {

                if ((!Localization.HasValue || Localization.Value <= 0) || (!CurrentProduct.HasValue || CurrentProduct.Value <= 0)) { return this.Direct(); }

               
                Product product = (from prod in db.Products
                                   where prod.ProductID==CurrentProduct.Value
                                   select prod).SingleOrDefault();
                bool productIsLens = product is Lens;

                if (productIsLens)
                {
                    double stockQuantity = (double)Session["MaxValue"];

                    this.GetCmp<NumberField>("StockQuantity").Value = stockQuantity;
                }
                else
                {
                   
                    var prodLoc = db.ProductLocalizations.Where(pl => pl.ProductID == CurrentProduct.Value && pl.LocalizationID == Localization.Value)
                    .Select(p => new
                    {
                        ProductLocalizationStockQuantity = p.ProductLocalizationStockQuantity,
                        SellingPrice = p.Product.SellingPrice
                    }).SingleOrDefault();

                    Session["MaxValue"] = prodLoc.ProductLocalizationStockQuantity;
                    this.GetCmp<NumberField>("StockQuantity").Value = prodLoc.ProductLocalizationStockQuantity;

                    bool isUpdate = (bool)Session["isUpdate"];

                    if (!isUpdate)
                    {
                        Session["isUpdate"] = false;
                    }
                }

                return this.Direct();
            }
            catch (Exception e)
            {
                statusOperation = Resources.er_alert_danger + e.Message + "Source = " + e.Source + "Méthode = OnProductSelected(" + Localization.Value + " " + CurrentProduct.Value + ") " + e.TargetSite + " InnerException = " + e.InnerException;
                X.Msg.Alert(Resources.Productlabel, statusOperation).Show();
                return this.Direct();
            }
        }
        */
        private Company Company
		{
			get
			{
				return db.Companies.FirstOrDefault();
			}
		}


        //This method print a receipt of customer
        public ActionResult GenerateReceipt(int RegProductNumberID)
        {
            List<RptTransfertForm> model = new List<RptTransfertForm>();
            try
            {
                
                @ViewBag.CompanyLogoID = Company.GlobalPersonID;
                string defaultImgPath = Server.MapPath("~/Content/Images/App/deboy-logo.png");
                byte[] COMPANY_LOGO = System.IO.File.ReadAllBytes(defaultImgPath);

                string DeviseLabel = db.Devises.Where(d => d.DefaultDevise).FirstOrDefault().DeviseLabel;
                RegProductNumber RegProductNumber = (from pt in db.RegProductNumbers
                                                     where pt.RegProductNumberID == RegProductNumberID
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
                db.RegProductNumberLines.Where(l => l.RegProductNumberID == RegProductNumberID).ToList().ForEach(c =>
                {
                    i = i + 1;
                    model.Add(
                            new RptTransfertForm
                            {
                                RptTransfertFormID = i,
                                ReceiveAmount = 0,
                                LineQuantity = c.NewLineQuantity,
                                ProductLabel = c.OldProduct.ProductCode,
                                ProductRef = c.NewProduct.ProductCode,
                                CompanyName = Company.Name,
                                CompanyAdress = "Head Quater : " + Company.Adress.Quarter.Town.Region.RegionLabel + " - " + Company.Adress.Quarter.Town.TownLabel,
                                CompanyTel = "Tel: " + Company.Adress.AdressPhoneNumber,
                                BranchName = curBranch.Branch.BranchName,
                                BranchAdress = curBranch.Branch.Adress.Quarter.QuarterLabel + " - " + curBranch.Branch.Adress.Quarter.Town.TownLabel,
                                BranchTel = "Tel: " + curBranch.Branch.Adress.AdressPhoneNumber,
                                Ref = RegProductNumber.RegProductNumberReference,
                                CompanyCNI = "NO CONT : " + Company.CNI,
                                Operator = CurrentUser.Name + " " + CurrentUser.Description,
                                SendindBranchName = RegProductNumber.Branch.BranchName,
                                SendindBranchCode = RegProductNumber.Branch.BranchCode,
                                TransfertDate = RegProductNumber.RegProductNumberDate.Date ,
                                Title = "Product RegProductNumbert lines informations",
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