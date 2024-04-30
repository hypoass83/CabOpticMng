using FatSod.DataContext.Initializer;
using FatSod.Security.Abstracts;
using FatSod.Security.Entities;
using FatSod.Supply.Entities;
using FatSodDental.UI.Controllers;
using FatSodDental.UI.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using SaleE = FatSod.Supply.Entities.Sale;
using FatSod.Supply.Abstracts;
using FatSod.Ressources;
using FatSodDental.UI.Filters;
using FastSod.Utilities.Util;
using System.Collections;
using FatSod.DataContext.Concrete;
using System.Web.UI;

namespace FatSodDental.UI.Areas.Supply.Controllers
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

            //Session["Curent_Page"] = VIEW_NAME;
            //Session["Curent_Controller"] = CONTROLLER_NAME;

            //We test if this user has an autorization to get this sub menu
            //if (!LoadAction.isAutoriseToGetMenu(SessionProfileID, CodeValue.Supply.SupplierReturnMenu.CODE, db))
            //{
            //    this.NotAuthorized();
            //}

            //this.GetCmp<Panel>("Body").LoadContent(new ComponentLoader
            //{
            //    Url = Url.Action(VIEW_NAME),
            //    DisableCaching = false,
            //    Mode = LoadMode.Frame
            //});

            //List<InventoryDirectoryLine> InventoryDirectoryLines = new List<InventoryDirectoryLine>();
            Session["InventoryDirectoryLines"] = new List<InventoryDirectoryLine>();

            //Ext.Net.MVC.PartialViewResult rPVResult = new Ext.Net.MVC.PartialViewResult
            //{
            //    ClearContainer = true,
            //    //RenderMode = RenderMode.AddTo,
            //    ContainerId = "Body",
            //    //WrapByScriptTag = false,
            //    Model = ModelInvDir
            //};

            //return rPVResult;

            return View(ModelInvDir);
        }
        public List<object> ModelInvDir
        {
            get
            {
                List<object> list = new List<object>();
                db.InventoryDirectories.Where(id => id.InventoryDirectoryStatut == InventoryDirectorySatut.Opened).ToList().ForEach(id =>
                    {
                        list.Add(new
                        {
                            InventoryDirectoryID = id.InventoryDirectoryID,
                            Branch = id.Branch.BranchName,
                            InventoryDirectoryReference = id.InventoryDirectoryReference,
                            InventoryDirectoryCreationDate = id.InventoryDirectoryCreationDate,
                            InventoryDirectoryDescription = id.InventoryDirectoryDescription,
                        });
                    }
                    );
                return list;
            }
        }
        public ActionResult ChangeBusDay(int? BranchID)
        {
            this.GetCmp<DateField>("InventoryDirectoryCreationDate").Reset();

            if (BranchID.HasValue && BranchID.Value > 0)
            {
                bdDay = (List<BusinessDay>)Session["UserBusDays"];
                if (bdDay == null)
                {
                    bdDay = _busDayRepo.GetOpenedBusinessDay(CurrentUser);
                };
                BusinessDay businessDay = bdDay.FirstOrDefault(l => l.BranchID == BranchID.Value);
                this.GetCmp<DateField>("InventoryDirectoryCreationDate").SetValue(businessDay.BDDateOperation);

                this.InitTrnNumber(BranchID.Value);

            }
            return this.Direct();
        }

        public ActionResult OpenedBusday()
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

            return this.Store(list);

        }
        public ActionResult InitTrnNumber(int? BranchID)
        {
            if (BranchID > 0)
            {
                bdDay = (List<BusinessDay>)Session["UserBusDays"];
                if (bdDay == null)
                {
                    bdDay = _busDayRepo.GetOpenedBusinessDay(CurrentUser);
                }

                BusinessDay businessDay = bdDay.FirstOrDefault(b=>b.BranchID==BranchID.Value);
                string trnnum = _transactNumbeRepository.returnTransactNumber("PURC", businessDay);
                this.GetCmp<TextField>("InventoryDirectoryReference").Value = trnnum;
            }
            return this.Direct();
        }
        [HttpPost]
        public ActionResult AddInventoryDirectoryLine(InventoryDirectoryLine inventoryDirectoryLine)
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
                    InventoryDirectoryLine existing = InventoryDirectoryLines.SingleOrDefault(pl => pl.ProductID == inventoryDirectoryLine.ProductID && pl.LocalizationID == inventoryDirectoryLine.LocalizationID);

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
            return this.Reset21();
        }
        [HttpPost]
        public ActionResult AddInventoryDirectory(InventoryDirectory inventoryDirectory)
        {

            List<InventoryDirectoryLine> InventoryDirectoryLines = (List<InventoryDirectoryLine>)Session["InventoryDirectoryLines"];

            inventoryDirectory.InventoryDirectoryLines = InventoryDirectoryLines;

            if (inventoryDirectory.InventoryDirectoryID > 0)
            {
                inventoryDirectoryRepository.UpdateInventoryDirectory(inventoryDirectory, SessionGlobalPersonID);
            }

            if (inventoryDirectory.InventoryDirectoryID == 0)
            {
                inventoryDirectoryRepository.CreateInventoryDirectory(inventoryDirectory,SessionGlobalPersonID);
            }
            Session["InventoryDirectoryLines"] = new List<InventoryDirectoryLine>();
            this.Reset();
            return this.Direct();
        }
        [HttpPost]
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
        }
        [HttpPost]
        public ActionResult RemoveInventoryDirectoryLine(int TMPID)
        {
            //lors de la création
            List<InventoryDirectoryLine> InventoryDirectoryLines = (List<InventoryDirectoryLine>)Session["InventoryDirectoryLines"];

            if (InventoryDirectoryLines != null && InventoryDirectoryLines.Count > 0)
            {
                InventoryDirectoryLine toRemove = InventoryDirectoryLines.SingleOrDefault(pl => pl.TMPID == TMPID);
                InventoryDirectoryLines.Remove(toRemove);
                Session["InventoryDirectoryLines"] = InventoryDirectoryLines;
            }

            return this.Reset2();
        }
        [HttpPost]
        public ActionResult Reset()
        {
            this.GetCmp<FormPanel>("InventoryDirectoryForm").Reset(true);
            this.GetCmp<FormPanel>("FormAddInventoryDirectoryLine").Reset(true);
            this.GetCmp<FormPanel>("GeneralForm").Reset(true);

            this.GetCmp<Store>("InventoryDirectoryLineStore").Reload();
            this.GetCmp<Store>("InventoryDirectoryListStore").Reload();

            Session["InventoryDirectoryLines"] = new List<InventoryDirectoryLine>();
            return this.Direct();
        }
        public ActionResult Reset2()
        {
            this.GetCmp<FormPanel>("FormAddInventoryDirectoryLine").Reset(true);

            this.GetCmp<Store>("InventoryDirectoryLineStore").Reload();

            ManageCady();
            return this.Direct();
        }
        public ActionResult Reset21()
        {
            //this.GetCmp<ComboBox>("ProductCategoryID").Value="";
            this.GetCmp<ComboBox>("ProductNumberID").Value = "";
            this.GetCmp<ComboBox>("ProductID").Value = "";
            this.GetCmp<Store>("InventoryDirectoryLineStore").Reload();

            ManageCady();
            return this.Direct();
        }
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
        public void ManageCady()
        {
            List<InventoryDirectoryLine> InventoryDirectoryLines = (List<InventoryDirectoryLine>)Session["InventoryDirectoryLines"];

            if (InventoryDirectoryLines != null && InventoryDirectoryLines.Count > 0)
            {
                this.GetCmp<TextField>("IsCadyEmpty").SetValue(0);//faux
            }
            if (InventoryDirectoryLines == null || InventoryDirectoryLines.Count == 0)
            {
                this.GetCmp<TextField>("IsCadyEmpty").SetValue(1);//vrai
            }
        }
        [HttpPost]
        public ActionResult InventoryDirectoryLines()
        {
            List<InventoryDirectoryLine> InventoryDirectoryLines = (List<InventoryDirectoryLine>)Session["InventoryDirectoryLines"];

            List<object> list = new List<object>();

            foreach (InventoryDirectoryLine idl in InventoryDirectoryLines)
            {
                list.Add(
                    new
                {
                    TMPID = idl.TMPID,
                    ProductLabel = idl.ProductLabel,
                    LocalizationLabel = idl.LocalizationLabel,
                }
                    );
            }
            return this.Store(list);
        }
        [HttpPost]
        public ActionResult DeleteInventoryDirectory(int InventoryDirectoryID)
        {
            inventoryDirectoryRepository.DeleteInventoryDirectory(InventoryDirectoryID);
            Session["InventoryDirectoryLines"] = new List<InventoryDirectoryLine>();
            this.Reset();

            return this.Direct();
        }
        [HttpPost]
        public StoreResult GetAllInventoryDirectories()
        {
            //un dossier d'inventaire qui est encours ou qui a déjà été fermé ne peut plus être modifiée ou supprimée
           
            return this.Store(ModelInvDir);
        }
       
        public ActionResult DisableNumero(int ProductCategoryID)
        {
            Category catprod = db.Categories.Find(ProductCategoryID);
            if (catprod is LensCategory)
            {
                //isLens = true;
                this.GetCmp<ComboBox>("ProductNumberID").Disabled = false;
                this.GetCmp<ComboBox>("ProductID").HideTrigger = true;
            }
            else
            {
                //isLens = false;
                this.GetCmp<ComboBox>("ProductNumberID").Disabled = true;
                this.GetCmp<ComboBox>("ProductID").HideTrigger = false;
            }
            return this.Direct();
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
            

            //
            if ((LocalizationID == 0 || LocalizationID==null ) && (ProductCategoryID == 0 || ProductCategoryID == null) && (ProductNumberID == 0 || ProductNumberID == null)) //chargement des produits en fct du magasin slt
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
                    if ((ProductNumberID != null || ProductNumberID > 0) && ProductCategoryID > 0 && (LocalizationID != null || LocalizationID > 0)) //desc et numero
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
                    if (ProductCategoryID > 0 && (LocalizationID != null || LocalizationID > 0)) //desc et numero
                    {
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
                    else return model;
                }
            }

            return model;
        }

        public StoreResult GetAllStockedLocations(int? BranchID)
        {
            List<object> list = new List<object>();

            foreach (Localization pt in db.Localizations.Where(l => l.BranchID == BranchID.Value).ToList())
            {
                list.Add(new
                {
                    LocalizationID = pt.LocalizationID,
                    LocalizationLabel = pt.LocalizationLabel,
                });
            }

            return this.Store(list);
        }

        public ActionResult GetAllPagingNumbers(int start, int limit, int page, string query, int? ProductCategory, int? localization)
        {
            bool isUpdate = (bool)Session["isUpdate"];
            string LensNumberFullCode = (string)Session["LensNumberFullCode"];

            query = (isUpdate == true) ? LensNumberFullCode : query;

            Paging<LensNumber> numbers = GetAllNumbers(start, limit, "", "", query, ProductCategory, localization);

            Session["isUpdate"] = false;
            Session["LensNumberFullCode"] = "*";

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
        public ActionResult OnLensNumberSelected(int? Localization, int? ProductNumberID, int? ProductCategoryID)
        {
            try
            {

                if ((Localization.HasValue && Localization.Value > 0) && (ProductNumberID.HasValue && ProductNumberID.Value > 0) && (ProductCategoryID.HasValue && ProductCategoryID.Value > 0))
                {
                    this.GetCmp<ComboBox>("ProductID").SetValueAndFireSelect(GetLensID(Localization.Value, ProductCategoryID.Value, ProductNumberID.Value));
                }
                else
                {
                    X.Msg.Alert(Resources.Productlabel, "This Product does not exist").Show();
                }

                return this.Direct();
            }
            catch (Exception e)
            {
                statusOperation = Resources.er_alert_danger + e.Message + "Méthode = OnLensNumberSelected() " + e.TargetSite;
                X.Msg.Alert(Resources.Productlabel, statusOperation).Show();
                return this.Direct();
            }
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
                            ProductLocalizationStockSellingPrice = s.pl.ProductLocalizationStockSellingPrice
                        }).FirstOrDefault();

            return lstLensProduct.ProductID;

        }

        [HttpPost]
        public ActionResult OnProductSelected(int? Localization, int? CurrentProduct)
        {
            try
            {

                if ((!Localization.HasValue || Localization.Value <= 0) || (!CurrentProduct.HasValue || CurrentProduct.Value <= 0)) 
                { 
                    return this.Direct();
                }

                Product product = db.Products.Find(CurrentProduct.Value);
                bool productIsLens = product is Lens;

                if (productIsLens)
            {
                    
            }
                else
                {

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

    }
}