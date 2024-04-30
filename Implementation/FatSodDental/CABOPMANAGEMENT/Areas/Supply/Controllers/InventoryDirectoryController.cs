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
using FatSod.Ressources;
using CABOPMANAGEMENT.Filters;
using CABOPMANAGEMENT.Tools;

namespace CABOPMANAGEMENT.Areas.Supply.Controllers
{
    [Authorize(Order = 1)]
    [TakeBusinessDay(Order = 2)]
    public class InventoryDirectoryController : BaseController
    {
        //Current Controller and current page
        private bool isLens = false;
        private const string CONTROLLER_NAME = "Supply/" + CodeValue.Supply.InventoryDirectory_SM.CONTROLLER;
        private const string VIEW_NAME = CodeValue.Supply.InventoryDirectory_SM.PATH;
        private IInventoryDirectory inventoryDirectoryRepository;

        private IBusinessDay _busDayRepo;

        private ITransactNumber _transactNumbeRepository;

        private List<BusinessDay> bdDay;
        //Construcitor
        public InventoryDirectoryController(
            IBusinessDay busDayRepo,
            ITransactNumber transactNumbeRepository,
            IInventoryDirectory inventoryDirectoryRepository
            )
        {
            this._busDayRepo = busDayRepo;
            this._transactNumbeRepository = transactNumbeRepository;
            this.inventoryDirectoryRepository = inventoryDirectoryRepository;
        }
        // GET: Sale/Sale Return all a sales historique add enable to create a new sale
        [OutputCache(Duration = 3600)]
        public ActionResult inventoryDirectory()
        {
            Session["isUpdate"] = false;
            Session["LensNumberFullCode"] = "*";

            Session["InventoryDirectoryLines"] = new List<InventoryDirectoryLine>();

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
            List<object> categoryList = new List<object>();
            List<Category> categories = LoadComponent.GetAllGenericCategories();
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




        //[HttpPost]
        public JsonResult AddInventoryDirectoryLine(InventoryDirectoryLine inventoryDirectoryLine)
        {
            bool status = false;
            //string Message = "";
            try
            {
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
                        );

                        if (existing != null && existing.TMPID > 0)
                        {

                            //le prix c'est le prix de la nouvelle ligne
                            //l'id c'est l'id de la ligne existante
                            inventoryDirectoryLine.TMPID = existing.TMPID;
                            inventoryDirectoryLine.InventoryDirectoryLineID = existing.InventoryDirectoryLineID;
                            //on retire l'ancien pour ajouter le nouveau
                            InventoryDirectoryLines.Remove(existing);
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
                inventoryDirectory.InventoryDirectoryLines = InventoryDirectoryLines;
                if (inventoryDirectory.InventoryDirectoryID > 0)
                {
                    inventoryDirectoryRepository.UpdateInventoryDirectory(inventoryDirectory, SessionGlobalPersonID);
                }
                if (inventoryDirectory.InventoryDirectoryID == 0)
                {
                    inventoryDirectoryRepository.CreateInventoryDirectory(inventoryDirectory, SessionGlobalPersonID);
                }
                Session["InventoryDirectoryLines"] = new List<InventoryDirectoryLine>();
                status = true;
                Message = Resources.Success + " - " + statusOperation;
            }
            catch (Exception e)
            {
                status = false;
                Message = "Error " + e.Message + " " + e.StackTrace + " " + e.InnerException;
            }
            return new JsonResult { Data = new { status = status, Message = Message } };
        }
        /*[HttpPost]
        public ActionResult UpdateInventoryDirectory(int InventoryDirectoryID)
        {
            List<InventoryDirectoryLine> realInventoryDirectoryLines = new List<InventoryDirectoryLine>();

            List<InventoryDirectoryLine> InventoryDirectoryLines = db.InventoryDirectoryLines.Where(idl => idl.InventoryDirectoryID == InventoryDirectoryID).ToList();
            
            int i = 0;
            foreach (InventoryDirectoryLine idl in InventoryDirectoryLines)
            {
                idl.TMPID = ++i;
                idl.ProductLabel = idl.Product.GetProductCode();
                idl.LocalizationLabel = idl.Localization.LocalizationLabel;
                realInventoryDirectoryLines.Add(idl);
            }
            Session["InventoryDirectoryLines"] = realInventoryDirectoryLines;
            this.InitializeInventoryDirectoryFields(InventoryDirectoryID);

            return this.Direct();
        }
        [HttpPost]
        public ActionResult UpdateInventoryDirectoryLine(int TMPID)
        {

            this.InitializeInventoryDirectoryLineFields(TMPID);

            return this.Direct();
        }*/
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
                           
                       }
            };
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        /*
        public void InitializeInventoryDirectoryFields(int ID)
        {

            this.GetCmp<FormPanel>("InventoryDirectoryForm").Reset(true);
            this.GetCmp<Store>("InventoryDirectoryListStore").Reload();

            this.GetCmp<Store>("InventoryDirectoryLineStore").Reload();

            if (ID > 0)
            {
                InventoryDirectory inventoryDirectory = new InventoryDirectory();
                inventoryDirectory = db.InventoryDirectories.Find(ID);

                this.GetCmp<TextField>("InventoryDirectoryID").Value = inventoryDirectory.InventoryDirectoryID;

                if (inventoryDirectory.BranchID > 0)
                {
                    this.GetCmp<ComboBox>("BranchID").Value = (inventoryDirectory.BranchID);
                }

                this.GetCmp<DateField>("InventoryDirectoryCreationDate").Value = inventoryDirectory.InventoryDirectoryCreationDate;
                this.GetCmp<TextField>("InventoryDirectoryReference").Value = inventoryDirectory.InventoryDirectoryReference;
                this.GetCmp<TextArea>("InventoryDirectoryDescription").Value = inventoryDirectory.InventoryDirectoryDescription;
                this.ManageCady();
            }
        }
        public void InitializeInventoryDirectoryLineFields(int ID)
        {

            this.GetCmp<FormPanel>("FormAddInventoryDirectoryLine").Reset(true);
            this.GetCmp<Store>("InventoryDirectoryLineStore").Reload();

            List<InventoryDirectoryLine> InventoryDirectoryLines = (List<InventoryDirectoryLine>)Session["InventoryDirectoryLines"];


            if (ID > 0)
            {
                InventoryDirectoryLine inventoryDirectoryLine = new InventoryDirectoryLine();
                inventoryDirectoryLine = InventoryDirectoryLines.SingleOrDefault(idl => idl.TMPID == ID);

                this.GetCmp<TextField>("TMPID").SetValue(inventoryDirectoryLine.TMPID);
                this.GetCmp<TextField>("InventoryDirectoryLineID").SetValue(inventoryDirectoryLine.InventoryDirectoryLineID);
                this.GetCmp<ComboBox>("LocalizationID").SetValueAndFireSelect(inventoryDirectoryLine.LocalizationID);
                this.GetCmp<ComboBox>("ProductID").Value=inventoryDirectoryLine.ProductID;

                if (inventoryDirectoryLine.InventoryDirectoryID > 0)
                {
                    this.InitializeInventoryDirectoryFields(inventoryDirectoryLine.InventoryDirectoryID);
                }

            }

            ManageCady();

        }
        */
        //[HttpPost]
        public JsonResult InventoryDirectoryLines()
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

        


    }
}