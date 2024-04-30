using FatSod.DataContext.Initializer;
using FatSod.Ressources;
using FatSod.Security.Abstracts;
using FatSod.Security.Entities;
using FatSod.Supply.Abstracts;
using FatSod.Supply.Abstracts.BarCode;
using FatSod.Supply.Entities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CABOPMANAGEMENT.Areas.Sale.Models;
using CABOPMANAGEMENT.Controllers;
using CABOPMANAGEMENT.Filters;
using CABOPMANAGEMENT.Tools;
using ZXing;

namespace CABOPMANAGEMENT.Areas.BarCode.Controllers
{
    [Authorize(Order = 1)]
    [TakeBusinessDay(Order = 2)]
    public class BarCodeStockInputController : BaseController
    {
        //Current Controller and current page
        private bool isLens = false;
        private const string CONTROLLER_NAME = "Supply/" + CodeValue.Supply.InventoryDirectory_SM.CONTROLLER;
        private const string VIEW_NAME = CodeValue.Supply.InventoryDirectory_SM.PATH;
        private IInventoryDirectory inventoryDirectoryRepository;
        private readonly IBarCodeService barCodeService;
        private IBusinessDay _busDayRepo;

        private ITransactNumber _transactNumbeRepository;

        private List<BusinessDay> bdDay;
        //Construcitor
        public BarCodeStockInputController(
            IBusinessDay busDayRepo,
            ITransactNumber transactNumbeRepository,
            IInventoryDirectory inventoryDirectoryRepository,
            IBarCodeService barCodeService
            )
        {
            this._busDayRepo = busDayRepo;
            this._transactNumbeRepository = transactNumbeRepository;
            this.inventoryDirectoryRepository = inventoryDirectoryRepository;
            this.barCodeService = barCodeService;
        }
        // GET: Sale/Sale Return all a sales historique add enable to create a new sale
        [OutputCache(Duration = 3600)]
        public ActionResult InventoryEntry()
        {
            Session["isUpdate"] = false;
            Session["LensNumberFullCode"] = "*";

            Session["InventoryDirectoryLines"] = new List<InventoryDirectoryLine>();

            Session["MaxValue"] = 0;
            Session["CurrentProduct"] = "";
            Session["CurrentProductID"] = 0;
            Session["SafetyStock"] = 0;

            InjectUserConfigInSession();


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


            return View(ModelInvDir);
        }

        [OutputCache(Duration = 3600)]
        public ActionResult Index()
        {
            Session["isUpdate"] = false;
            Session["LensNumberFullCode"] = "*";

            Session["InventoryDirectoryLines"] = new List<InventoryDirectoryLine>();

            InjectUserConfigInSession();


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


            return View(ModelInvDir);
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
                    NumeroSerie = prodLocalization.NumeroSerie,
                    ProductCode = prodLocalization.Product.ProductCode,
                });

            }
            return Json(_CommandList, JsonRequestBehavior.AllowGet);
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
                    Session["isUpdate"] = false;
                }
            }


            _InfoList.Add(new
            {
                StockQuantity = StockQuantity

            });
            return Json(_InfoList, JsonRequestBehavior.AllowGet);
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

        public List<InventoryDirectory> ModelInvDir
        {
            get
            {
                List<InventoryDirectory> list = new List<InventoryDirectory>();
                db.InventoryDirectories.Where(id => id.InventoryDirectoryStatut == InventoryDirectorySatut.Opened).ToList().ForEach(id =>
                {
                    list.Add(new InventoryDirectory
                    {
                        InventoryDirectoryID = id.InventoryDirectoryID,
                        Branch = id.Branch,
                        InventoryDirectoryReference = id.InventoryDirectoryReference,
                        InventoryDirectoryCreationDate = id.InventoryDirectoryCreationDate,
                        InventoryDirectoryDescription = id.InventoryDirectoryDescription,
                    });
                }
                    );
                return list;
            }
        }

        public JsonResult GetAllInventoryDirectories()
        {
            var model = new
            {
                data = from id in ModelInvDir
                       select new
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
                string trnnum = _transactNumbeRepository.returnTransactNumber("PURC", businessDay);
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
                    CategoryCode = cat.CategoryLabel
                });
            }

            return Json(categoryList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DisableNumero(int ProductCategoryID)
        {
            List<object> _InfoList = new List<object>();
            //0 = frame
            //1 = equipement
            //2 = lens
            Category catprod = db.Categories.Find(ProductCategoryID);
            _InfoList.Add(new
            {
                LensCategory = (catprod == null || catprod.isSerialNumberNull) ? 0 : (catprod is LensCategory) ? 2 : 1
            });
            return Json(_InfoList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAllSerialNumber(string filter, int DepartureLocalizationID = 0)
        {

            List<object> customersList = new List<object>();
            List<ProductLocalization> res = new List<ProductLocalization>();

            res = db.ProductLocalizations.Where(c => c.LocalizationID == DepartureLocalizationID && c.NumeroSerie.Contains(filter.ToLower())).Take(10).ToList();


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
                        .GroupBy(ac => new
                        {
                            ac.p.ProductID,
                            ac.p.ProductCode,
                            ac.p.ProductLabel
                        })
                        .Select(s => new
                        {
                            ProductID = s.Key.ProductID,
                            ProductCode = s.Key.ProductCode,
                            ProductLabel = s.Key.ProductLabel,
                            ProductQuantity = s.Sum(acs => acs.pl.ProductLocalizationStockQuantity) //s.pl.ProductLocalizationStockQuantity,
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
                            }
                           );
                    }

                }
            }

            return model;
        }



        //[HttpPost]
        public JsonResult AddInventoryDirectoryLine(InventoryDirectoryLine inventoryDirectoryLine, int isSerialNumberNull = 0)
        {
            bool status = false;
            //string Message = "";
            try
            {
                if (isSerialNumberNull > 0)
                {
                    //nous devons nous assurer que le numero de serie est unique pour la categorie et la marque concerne
                    ProductLocalization oldProdloc = db.ProductLocalizations.Where(p => p.LocalizationID == inventoryDirectoryLine.LocalizationID
                    && p.NumeroSerie.Trim() == inventoryDirectoryLine.NumeroSerie.Trim()).FirstOrDefault();
                    if (oldProdloc != null)
                    {
                        if (oldProdloc.Marque != inventoryDirectoryLine.Marque)
                        {
                            status = false;
                            statusOperation = "Error: This Database cannot accept two different Serial number (" + inventoryDirectoryLine.NumeroSerie + ") with the same Marque/Brand (" + oldProdloc.Marque + ") ";
                            return new JsonResult { Data = new { status = status, Message = statusOperation } };
                        }

                        if (oldProdloc.ProductID != inventoryDirectoryLine.ProductID)
                        {
                            status = false;
                            statusOperation = "Error: This Database cannot accept two different Serial number (" + inventoryDirectoryLine.NumeroSerie + ") with the same Product (" + oldProdloc.Product.ProductCode + ")";
                            return new JsonResult { Data = new { status = status, Message = statusOperation } };
                        }
                    }
                }

                inventoryDirectoryLine.ProductLabel = db.Products.Find(inventoryDirectoryLine.ProductID).GetProductCode();
                inventoryDirectoryLine.LocalizationLabel = db.Localizations.Find(inventoryDirectoryLine.LocalizationID).LocalizationLabel;


                List<InventoryDirectoryLine> InventoryDirectoryLines = (List<InventoryDirectoryLine>)Session["InventoryDirectoryLines"];

                //il s'agit d'une modification alors on fait un drop and create
                if (inventoryDirectoryLine.TMPID > 0)
                {
                    InventoryDirectoryLine toRemove = InventoryDirectoryLines.SingleOrDefault(pl => pl.TMPID == inventoryDirectoryLine.TMPID);
                    InventoryDirectoryLines.Remove(toRemove);
                }

                //alors la variable de session n'était pas vide
                if (InventoryDirectoryLines != null && InventoryDirectoryLines.Count > 0)
                {
                    //c'est un nouvel ajout dans le panier
                    if (inventoryDirectoryLine.TMPID == 0)
                    {
                        //existe t-il déjà une ligne de vente ayant le meme produit et le même magasin que celui en création?
                        InventoryDirectoryLine existing = InventoryDirectoryLines.SingleOrDefault(pl => pl.ProductID == inventoryDirectoryLine.ProductID && pl.LocalizationID == inventoryDirectoryLine.LocalizationID
                        && pl.NumeroSerie == inventoryDirectoryLine.NumeroSerie && pl.Marque == inventoryDirectoryLine.Marque);

                        if (existing != null && existing.TMPID > 0)
                        {
                            double NewStockQuantity = existing.NewStockQuantity.Value + inventoryDirectoryLine.NewStockQuantity.Value;
                            //le prix c'est le prix de la nouvelle ligne
                            //l'id c'est l'id de la ligne existante
                            inventoryDirectoryLine.TMPID = existing.TMPID;
                            inventoryDirectoryLine.InventoryDirectoryLineID = existing.InventoryDirectoryLineID;
                            // on retire l'ancien pour ajouter le nouveau
                            InventoryDirectoryLines.Remove(existing);

                            inventoryDirectoryLine.NewStockQuantity = NewStockQuantity;

                        }

                        if (existing == null || existing.TMPID == 0)
                        {
                            inventoryDirectoryLine.TMPID = InventoryDirectoryLines.Select(pl => pl.TMPID).Max() + 1;
                        }
                    }
                    InventoryDirectoryLines.Add(inventoryDirectoryLine);
                }

                //alors la variable de session était vide
                if (InventoryDirectoryLines == null || InventoryDirectoryLines.Count == 0)
                {
                    //c'est bon pour la création mais pas pour les modifications
                    InventoryDirectoryLines = new List<InventoryDirectoryLine>();
                    if (inventoryDirectoryLine.TMPID == 0)
                    {
                        inventoryDirectoryLine.TMPID = 1;
                    }
                    InventoryDirectoryLines.Add(inventoryDirectoryLine);
                }
                Session["InventoryDirectoryLines"] = InventoryDirectoryLines;
                statusOperation = "ok";
                status = true;
            }
            catch (Exception e)
            {
                status = false;
                statusOperation = "Error " + e.Message + " " + e.StackTrace + " " + e.InnerException;
            }
            return new JsonResult { Data = new { status = status, Message = statusOperation } };
        }

        //[HttpPost]
        public JsonResult AddInventoryDirectory(InventoryDirectory inventoryDirectory)
        {
            bool status = false;
            string Message = "";
            try
            {
                List<InventoryDirectoryLine> InventoryDirectoryLines = (List<InventoryDirectoryLine>)Session["InventoryDirectoryLines"];
                if (InventoryDirectoryLines != null && InventoryDirectoryLines.Count() > 0)
                {
                    inventoryDirectory.InventoryDirectoryLines = InventoryDirectoryLines;
                    inventoryDirectory.RegisteredByID = SessionGlobalPersonID;
                    if (inventoryDirectory.InventoryDirectoryID > 0)
                    {
                        inventoryDirectoryRepository.UpdateInventoryDirectory(inventoryDirectory, SessionGlobalPersonID);
                    }
                    if (inventoryDirectory.InventoryDirectoryID == 0)
                    {
                        inventoryDirectoryRepository.CreateAndCloseInventoryDirectory(inventoryDirectory, SessionGlobalPersonID);
                    }

                    var payloads = new List<BarCodePayload>();
                    InventoryDirectoryLines.ForEach(line =>
                    {
                        var payload = GenerateBarCode(line, 1);
                        payloads.Add(payload);
                    });
                    Session["payloads"] = payloads;

                    Session["InventoryDirectoryLines"] = new List<InventoryDirectoryLine>();
                    status = true;
                    Message = Resources.Success + " - " + statusOperation;
                }
                else
                {
                    status = false;
                    Message = Resources.AlertError + " - " + "Please Add Products to your Inventory";
                }

            }
            catch (Exception e)
            {
                status = false;
                Message = "Error " + e.Message + " " + e.StackTrace + " " + e.InnerException;
            }
            return new JsonResult { Data = new { status = status, Message = Message } };
        }

        //[HttpPost]
        public JsonResult RemoveInventoryDirectoryLine(int TMPID)
        {
            //lors de la création
            List<InventoryDirectoryLine> InventoryDirectoryLines = (List<InventoryDirectoryLine>)Session["InventoryDirectoryLines"];

            if (InventoryDirectoryLines != null && InventoryDirectoryLines.Count > 0)
            {
                InventoryDirectoryLine toRemove = InventoryDirectoryLines.SingleOrDefault(pl => pl.TMPID == TMPID);
                InventoryDirectoryLines.Remove(toRemove);
                Session["InventoryDirectoryLines"] = InventoryDirectoryLines;
            }

            var model = new
            {
                data = from idl in InventoryDirectoryLines
                       select new
                       {
                           TMPID = idl.TMPID,
                           ProductLabel = idl.ProductLabel,
                           LocalizationLabel = idl.LocalizationLabel,
                           NumeroSerie = idl.NumeroSerie,
                           Marque = idl.Marque
                       }
            };
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        //[HttpPost]
        public JsonResult AddInventoryDirectoryLines()
        {
            List<InventoryDirectoryLine> InventoryDirectoryLines = (List<InventoryDirectoryLine>)Session["InventoryDirectoryLines"];

            var model = new
            {
                data = from idl in InventoryDirectoryLines
                       select new
                       {
                           TMPID = idl.TMPID,
                           ProductLabel = idl.ProductLabel,
                           LocalizationLabel = idl.LocalizationLabel,
                           NumeroSerie = idl.NumeroSerie,
                           Marque = idl.Marque,
                           NewStockQuantity = idl.NewStockQuantity,
                           Price = idl.AveragePurchasePrice
                       }
            };
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        //[HttpPost]
        public JsonResult DeleteInventoryDirectory(int InventoryDirectoryID)
        {
            bool status = false;
            string Message = "";
            try
            {
                inventoryDirectoryRepository.DeleteInventoryDirectory(InventoryDirectoryID);
                Session["InventoryDirectoryLines"] = new List<InventoryDirectoryLine>();
                status = true;
                Message = Resources.Success + " - Directory Inventory has been deleted";
            }
            catch (Exception e)
            {
                status = false;
                Message = "Error " + e.Message + " " + e.StackTrace + " " + e.InnerException;
            }
            return new JsonResult { Data = new { status = status, Message = Message } };
        }

        public JsonResult GetAllNumbers(string filter, int? ProductCategory, int? localization)
        {

            List<ModelLensNumber> numbers = ModelLensNumber(filter, ProductCategory.Value, localization.Value);

            return Json(numbers, JsonRequestBehavior.AllowGet);

        }
        public List<ModelLensNumber> ModelLensNumber(string filter, int ProductCategoryID, int Localization)
        {
            /*
            try
            {
                foreach (LensNumber ln in db.LensNumbers)
                {
                    if (ln.LensNumberDescription != ln.LensNumberFullCode)
                    {
                        ln.LensNumberDescription = ln.LensNumberFullCode;
                    }
                }
                db.SaveChanges();
            } catch(Exception e)
            {

            }
            */


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
                    LensCategory Lenscatprod = (LensCategory)catprod;
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
            /*goto StockExists;
            StockExists:*/
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
            if (lstLensProduct != null)
            {
                Session["MaxValue"] = (lstLensProduct == null) ? 0 : lstLensProduct.ProductQuantity;
                Session["CurrentProduct"] = (lstLensProduct == null) ? "" : lstLensProduct.ProductCode;
                Session["CurrentProductID"] = (lstLensProduct == null) ? 0 : lstLensProduct.ProductID;
                Session["SafetyStock"] = (lstLensProduct == null) ? 0 : lstLensProduct.ProductLocalizationSafetyStockQuantity;
                return (lstLensProduct == null) ? 0 : lstLensProduct.ProductID;
                // goto StockNotExists;
            }
            else
            {
                this.CreateLensStock(DepartureLocalizationID, ProductCategoryID, ProductNumberID);

                var lstLensProduct1 = db.Lenses.Join(db.Products, ls => ls.ProductID, p => p.ProductID,
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
                Session["MaxValue"] = (lstLensProduct1 == null) ? 0 : lstLensProduct1.ProductQuantity;
                Session["CurrentProduct"] = (lstLensProduct1 == null) ? "" : lstLensProduct1.ProductCode;
                Session["CurrentProductID"] = (lstLensProduct1 == null) ? 0 : lstLensProduct1.ProductID;
                Session["SafetyStock"] = (lstLensProduct1 == null) ? 0 : lstLensProduct1.ProductLocalizationSafetyStockQuantity;
                return (lstLensProduct1 == null) ? 0 : lstLensProduct1.ProductID;
                // goto Finish;
            }

            // StockNotExists:
            // Create Stock
            // goto StockExists;
            // Finish:


        }
        public void CreateLensStock(int LocalizationID, int ProductCategoryID, int ProductNumberID)
        {
            try
            {
                Lens lens = db.Lenses.SingleOrDefault(l => l.LensNumberID == ProductNumberID && l.LensCategoryID == ProductCategoryID);
                ProductLocalization stock = new ProductLocalization()
                {
                    ProductID = lens.ProductID,
                    LocalizationID = LocalizationID,
                    ProductLocalizationDate = (DateTime)Session["BusnessDayDate"]
                };
                stock = db.ProductLocalizations.Add(stock);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public JsonResult populateMarque()
        {
            //holds list of ClassAccountss
            List<object> _Marque = new List<object>();
            //queries all the ProductBrands for its ID and Name property.
            var Marque = (from s in db.ProductBrands
                          select new { s.ProductBrandID, s.ProductBrandCode }).ToList();

            foreach (var item in Marque.OrderBy(i => i.ProductBrandCode))
            {
                _Marque.Add(new
                {
                    ID = item.ProductBrandCode,
                    Name = item.ProductBrandCode
                });
            }
            return Json(_Marque, JsonRequestBehavior.AllowGet);
        }
        public ProductLocalization CreateFrameStock(int productId, int locationId, string numeroSerie, string marque)
        {
            try
            {
                ProductLocalization stock = new ProductLocalization()
                {
                    ProductID = productId,
                    LocalizationID = locationId,
                    NumeroSerie = numeroSerie,
                    Marque = marque,
                    ProductLocalizationDate = (DateTime)Session["BusnessDayDate"]
                };
                db.ProductLocalizations.Add(stock);
                db.SaveChanges();
                stock.Product = db.Products.Find(stock.ProductID);
                stock.Localization = db.Localizations.Find(stock.LocalizationID);

                return stock;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public BarCodePayload GenerateBarCode(InventoryDirectoryLine invDirLine, int isSerialNumberNull = 0)
        {
            var status = true;
            var statusOperation = "Success";

            var payload = new BarCodePayload()
            {
                Quantity = int.Parse("" + invDirLine.NewStockQuantity.Value),
            };

            #region MyRegion
            if (isSerialNumberNull > 0)
            {
                //nous devons nous assurer que le numero de serie est unique pour la categorie et la marque concerner
                ProductLocalization stock = db.ProductLocalizations.SingleOrDefault(p =>
                                                                p.LocalizationID == invDirLine.LocalizationID &&
                                                                p.ProductID == invDirLine.ProductID &&
                                                                p.NumeroSerie.Trim() == invDirLine.NumeroSerie.Trim() &&
                                                                p.Marque == invDirLine.Marque);
                if (stock == null)
                {
                    stock = db.ProductLocalizations.SingleOrDefault(p =>
                                                                p.LocalizationID == invDirLine.LocalizationID &&
                                                                p.NumeroSerie.Trim() == invDirLine.NumeroSerie.Trim() &&
                                                                p.Marque == invDirLine.Marque);

                    if (stock == null)
                    {
                        // Creation du stock
                        stock = CreateFrameStock(invDirLine.ProductID, invDirLine.LocalizationID, invDirLine.NumeroSerie, invDirLine.Marque);
                        stock = db.ProductLocalizations.SingleOrDefault(p => p.ProductLocalizationID == stock.ProductLocalizationID);
                    }
                }

                payload.ProductId = stock.ProductID;
                payload.LocationId = stock.LocalizationID;
                payload.ProductPrice = stock.Product.SellingPrice;
                payload.Marque = stock.Marque;
                payload.NumeroSerie = stock.NumeroSerie;

                payload.BarCode = barCodeService.GetBarCode(payload);

                #region Improvement
                if (stock.NumeroSerie == null)
                {
                    payload.NumeroSerie = stock.Product.ProductLabel;
                }

                if (stock.Marque == null)
                {
                    payload.Marque = stock.Product.Category.CategoryCode;
                }

                if (stock.NumeroSerie != null && (stock.Product is GenericProduct))
                {
                    if (stock.Product.ProductCode == "ECONOMIQUE")
                    {
                        payload.Marque += " (E)";
                    }

                    if (stock.Product.ProductCode == "ECO+")
                    {
                        payload.Marque += " (E+)";
                    }
                }
                #endregion

                payload.Marque = payload.Marque?.Length <= 20 ? payload.Marque : payload.Marque?.Substring(0, 20);
                payload.NumeroSerie = payload.NumeroSerie?.Length <= 29 ? payload.NumeroSerie : payload.NumeroSerie?.Substring(0, 29);
            }
            #endregion


            var barCodeWritter = new BarcodeWriter();
            barCodeWritter.Format = BarcodeFormat.CODE_128;
            barCodeWritter.Options.Height = 25;
            barCodeWritter.Options.Width = 130;
            barCodeWritter.Options.PureBarcode = true;

            using (MemoryStream memoryStream = new MemoryStream())
            {
                var barCodeData = barCodeService.GetPayload(payload);
                barCodeWritter.Write(barCodeData).Save(memoryStream, ImageFormat.Jpeg);
                payload.BarCodeImage = "data:image/png;base64," + Convert.ToBase64String(memoryStream.ToArray());
            }

            return payload;
        }

        public ActionResult BarCodePreview()
        {
            var payloads = ((List<BarCodePayload>)Session["payloads"]);
            return View(payloads);
        }
    }
}