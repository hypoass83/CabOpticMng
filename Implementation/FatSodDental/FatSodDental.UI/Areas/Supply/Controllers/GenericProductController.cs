using AutoMapper;
using Ext.Net;
using Ext.Net.MVC;
using FatSod.DataContext.Concrete;
using FatSod.DataContext.Initializer;
using FatSod.Security.Abstracts;
using FatSod.Supply.Abstracts;
using FatSod.Supply.Entities;
using FatSodDental.UI.Areas.Supply.ViewModel;
using FatSodDental.UI.Controllers;
using FatSodDental.UI.Filters;
using FatSodDental.UI.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI;

namespace FatSodDental.UI.Areas.Supply.Controllers
{
    [Authorize(Order = 1)]
    [TakeBusinessDay(Order = 2)]
    public class GenericProductController : BaseController
    {
        private IRepositorySupply<GenericProduct> genericProductRepository;
        
        private IProductLocalization plRepository;

        

        private const string CONTROLLER_NAME = "Supply/" + CodeValue.Supply.GenericProduct_SM.CONTROLLER;
        private const string VIEW_NAME = CodeValue.Supply.GenericProduct_SM.PATH;

        public GenericProductController(IRepositorySupply<GenericProduct> productRepository,
                                        IProductLocalization plRepository)
        {
            this.genericProductRepository = productRepository;
            this.plRepository = plRepository;
            
        }

        /// <summary>
        /// the view of this action method is stronglyTyped with FatSod.Supply.Entities.Product
        /// </summary>
        /// <returns>ActionResult</returns>
        /// 
        //[OutputCache(Duration = 3600)] 
        public ActionResult GenericProduct(ProductModel productModel)
        {
            //Session["Curent_Page"] = VIEW_NAME;
            //Session["Curent_Controller"] = CONTROLLER_NAME;

            //We verify if the current user has right to access view which this action calls
            //if (!LoadAction.isAutoriseToGetSubMenu(SessionProfileID, CodeValue.Supply.GenericProduct_SM.CODE, db))
            //{
            //    this.NotAuthorized();
            //}

            //this.GetCmp<Panel>("Body").LoadContent(new ComponentLoader
            //{
            //    Url = Url.Action("GenericProduct"),
            //    DisableCaching = false,
            //    Mode = LoadMode.Frame
            //});

            //ces  lignes qui suivent permettent de transformer un Product en ProductModel 
            //create a Map
            Mapper.CreateMap<GenericProduct, ProductModel>()
                .ForMember(pm => pm.Category, m => m.MapFrom(p => db.Categories.Find(p.CategoryID).CategoryLabel))
                .ForMember(pm => pm.Account, m => m.MapFrom(p => db.Accounts.Find(p.AccountID).AccountLabel));
                
            productModel.Products = ProductModel();
            productModel.Categories = LoadComponent.GetAllGenericCategoryItems();
            productModel.AccountNumbers = LoadComponent.Accounts(CodeValue.Accounting.DefaultCodeAccountingSection.CODEPROD);

            //Ext.Net.MVC.PartialViewResult rPVResult = new Ext.Net.MVC.PartialViewResult
            //{
            //    ClearContainer = true,
            //    //RenderMode = RenderMode.AddTo,
            //    ContainerId = "Body",
            //    //WrapByScriptTag = false,
            //    Model = productModel
            //};

            return View(productModel);
        }

        [HttpPost]
        public ActionResult AddManager()
        {
            GenericProduct product = new GenericProduct();
            TryUpdateModel(product);

            //en cas de mise à jour
            if (product.ProductID > 0)
            {
                return this.UpdateProduct(product);
            }
            else
            {
                return this.AddProduct(product);
            }
        }


        [HttpPost]
        public ActionResult AddProduct(GenericProduct product)
        {
            try
            {
                List<int> newStore = product.Stores.ToList();
                newStore.Remove(0);
                product.Stores = newStore.ToArray();
                
                    if (((db.GenericProducts.FirstOrDefault(c => c.ProductCode == product.ProductCode || c.ProductLabel == product.ProductLabel) == null)))
                    {
                        product = genericProductRepository.Create(product);
                        plRepository.CreateStore(product, product.Stores);
                        TempData["status"] = "The Product " + product.ProductCode + " has been successfully created";
                        TempData["alertType"] = "alert alert-success";
                        return this.Reset();
                    }
                    else
                    {
                        TempData["alertType"] = "alert alert-danger";
                        TempData["status"] = @"Une catégorie ayant le code " + product.ProductCode + " et / ou le label " + product.ProductLabel + " existe déjà!<br/>"
                                            + "veuillez changer de code et / ou le label";
                        return this.Direct();
                    }
                
            }
            catch (Exception e)
            {
                TempData["alertType"] = "alert alert-danger";
                TempData["status"] = @"Une erreur s'est produite lors de l'opération, veuillez contactez l'administrateur et/ou essayez à nouveau
                                    <br/>Code : <code>" + e.Message + " car " + e.StackTrace + "</code>";
                return this.Direct();
            }
        }

        /// <summary>
        /// cette méthode est appelée quand on clicque sur l'icone modifier du tableau
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult UpdateProduct(int ProductID)
        {
            this.InitializeProductFields(ProductID);
            return this.Direct();
        }

        /// <summary>
        /// cette méthode est appelée lorqu' après avoir clicquer sur l'icone modifier du tableau on modifie le formulaire et clique sur enregistrer
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UpdateProduct(GenericProduct product)
        {
            try
            {

                List<int> newStore = product.Stores.ToList();
                List<int> oldStore = plRepository.GetAllStore(product).ToList();

                newStore.Remove(0);
                oldStore.Remove(0);
                product.Stores = newStore.ToArray();

                bool a = newStore != null && newStore.Count() > 0;
                bool b = oldStore != null && oldStore.Count() > 0;
                bool c = a && b;

                if (!c)
                {

                    //if(a == false && b == false){}

                    if (a == false && b == true)//On a aucun nouveau ou il y avait des anciens
                    {
                        plRepository.DeleteAllStore(product.ProductID);
                    }

                    if (a == true && b == false)//on a des nouveaux ou il n'y avait aucun ancien
                    {
                        plRepository.CreateStore(product, newStore.ToArray());
                    }

                }

                if (c)
                {//il y avait les anciens et il y a les anciens

                    List<int> createdStore = new List<int>(); //dans le nouveau et pas dans l'ancien

                    foreach (int id1 in newStore)
                    {
                        if (oldStore.Contains(id1))     //est dans les 2  => rien à faire
                        {
                            oldStore.Remove(id1);
                        }
                        else                           //est dans les nouveaux seulement => à créer
                        {
                            createdStore.Add(id1);
                        }
                    }

                    //Au sortir de la boucle, oldStore contient ceux qui sont seulement dans les anciens => à Supprimer
                    //Suppression des anciennes qui n'existe plus
                    if (oldStore != null && oldStore.Count() > 0) plRepository.DeleteAllStore(product.ProductID, oldStore.ToArray());
                    //Création des nouvelles qui n'existait pas
                    if (createdStore != null && createdStore.Count() > 0) plRepository.CreateStore(product, createdStore.ToArray());
                    //On ne fait rien pour les nouveaux qui existait
                }

                    GenericProduct existingProduct = db.GenericProducts.Find(product.ProductID);
                    List<GenericProduct> products = db.GenericProducts.ToList();
                    products.Remove(existingProduct);

                    if (((products.FirstOrDefault(p => p.ProductCode == product.ProductCode || p.ProductLabel == product.ProductLabel) == null)))
                    {
                        genericProductRepository.Update(product, product.ProductID);
                        TempData["status"] = "The Product " + product.ProductCode + " has been successfully updated";
                        TempData["alertType"] = "alert alert-success";
                        return this.Reset();
                    }
                    else
                    {
                        TempData["alertType"] = "alert alert-danger";
                        TempData["status"] = @"Une catégorie ayant le code " + product.ProductCode + " et / ou le label " + product.ProductLabel + " existe déjà!<br/>"
                                            + "veuillez changer de code et / ou le label";
                        return this.Direct();
                    }
                
            }
            catch (Exception e)
            {
                TempData["alertType"] = "alert alert-danger";
                TempData["status"] = @"Une erreur s'est produite lors de la mise à jour, veuillez contactez l'administrateur et/ou essayez à nouveau
                                    <br/>Code : <code>" + e.Message + "</code>";
                return this.Direct();
            }
        }

        [HttpPost]
        public ActionResult DeleteProduct(int ProductID)
        {
            try
            {
                plRepository.DeleteAllStore(ProductID);
                GenericProduct deletedProduct = db.GenericProducts.Find(ProductID);

                if (IsNoStockCreated(deletedProduct) == false)
                {
                    //TempData["status"] = "The Product " + deletedProduct.ProductCode + " has already been used to create one or many stocks!" +
                    //                     "If you realy want to delete this location, you will firt of all delete all thus stocks";
                    //TempData["alertType"] = "alert alert-danger";
                    X.Msg.Alert("Product", "The Product " + deletedProduct.ProductCode + " has already been used to create one or many stocks!" +
                                         "If you realy want to delete this location, you will firt of all delete all thus stocks").Show();
                    return this.Direct();
                }

                

                if (IsNoSupplierOrderAffected(deletedProduct) == false)
                {
                    //TempData["status"] = "The Product " + deletedProduct.ProductCode + " has already been used in one or many supplier orders!" +
                    //                     "If you realy want to delete this location, you will firt of all delete all thus supplier ordres";
                    //TempData["alertType"] = "alert alert-danger";
                    X.Msg.Alert("Product", "The Product " + deletedProduct.ProductCode + " has already been used in one or many supplier orders!" +
                                         "If you realy want to delete this location, you will firt of all delete all thus supplier ordres").Show();
                    return this.Direct();
                }

                if (IsNoSaleAffected(deletedProduct) == false)
                {
                    //TempData["status"] = "The Product " + deletedProduct.ProductCode + " has already been used in one or many sales!" +
                    //                     "If you realy want to delete this location, you will firt of all delete all thus sales";
                    //TempData["alertType"] = "alert alert-danger";
                    X.Msg.Alert("Product", "The Product " + deletedProduct.ProductCode + " has already been used in one or many sales!" +
                                         "If you realy want to delete this location, you will firt of all delete all thus sales").Show();
                    return this.Direct();
                }

                if (IsNoPurchaseAffected(deletedProduct) == false)
                {
                    //TempData["status"] = "The Product " + deletedProduct.ProductCode + " has already been used in one or many Purchases!" +
                    //                     "If you realy want to delete this location, you will firt of all delete all thus Purchases";
                    //TempData["alertType"] = "alert alert-danger";
                    X.Msg.Alert("Product", "The Product " + deletedProduct.ProductCode + " has already been used in one or many Purchases!" +
                                         "If you realy want to delete this location, you will firt of all delete all thus Purchases").Show();
                    return this.Direct();
                }
                db.GenericProducts.Remove(deletedProduct);
                db.SaveChanges();
                //genericProductRepository.Delete(deletedProduct);
                //TempData["status"] = "The Product " + deletedProduct.ProductCode + " has been successfully deleted";
                //TempData["alertType"] = "alert alert-success";
                X.Msg.Alert("Product", "The Product " + deletedProduct.ProductCode + " has been successfully deleted").Show();
                return this.Reset();
            }
            catch (Exception e)
            {
                //TempData["alertType"] = "alert alert-danger";
                //TempData["status"] = @"L'erreur " + e.Message + " s'est produite lors de la suppression! veuillez contactez l'administrateur et/ou essayez à nouveau";
                X.Msg.Alert("Product", @"L'erreur " + e.Message + " s'est produite lors de la suppression! veuillez contactez l'administrateur et/ou essayez à nouveau").Show();
                return this.Direct();
            }
        }
        
        private ActionResult NotAuthorized()
        {
            return View();
        }

        //méthodes utiles
        /// <summary>
        /// cette méthode vérifie si aucun stock(ProductLocalisation) n'a été créer pour ce produit
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        bool IsNoStockCreated(GenericProduct product)
        {
            bool res = false;
            List<ProductLocalization> stocks = db.ProductLocalizations.Where(pl => pl.ProductID == product.ProductID).ToList();
            if (stocks == null || stocks.Count == 0)
            {
                res = true;
            }

            return res;
        }

        /// <summary>
        /// cette méthode vérifie si ce produit n'a pas encore été utilisé dans une commande fournisseur
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        bool IsNoSupplierOrderAffected(GenericProduct product)
        {
            bool res = false;
            List<SupplierOrderLine> sol = db.SupplierOrderLines.Where(so => so.ProductID == product.ProductID).ToList();
            if (sol == null || sol.Count == 0)
            {
                res = true;
            }

            return res;
        }

        

        /// <summary>
        /// cette méthode vérifie si ce produit n'a pas encore été utilisé dans une vente
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        bool IsNoSaleAffected(GenericProduct product)
        {
            bool res = false;
            List<SaleLine> sl = db.SaleLines.Where(s => s.ProductID == product.ProductID).ToList();
            if (sl == null || sl.Count == 0)
            {
                res = true;
            }

            return res;
        }

        /// <summary>
        /// cette méthode vérifie si ce produit n'a pas encore été utilisé dans un achat
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        bool IsNoPurchaseAffected(GenericProduct product)
        {
            bool res = false;
            List<PurchaseLine> pl = db.PurchaseLines.Where(p => p.ProductID == product.ProductID).ToList();
            if (pl == null || pl.Count == 0)
            {
                res = true;
            }

            return res;
        }

        public void InitializeProductFields(int ID)
        {

            this.GetCmp<FormPanel>("GenericProductForm").Reset(true);
            this.GetCmp<Store>("ProductListStore").Reload();

            if (ID > 0)
            {
                GenericProduct product = new GenericProduct();
                product = db.GenericProducts.Find(ID);

                this.GetCmp<TextField>("ProductID").Value = product.ProductID;
                this.GetCmp<TextField>("ProductCode").Value = product.ProductCode;
                this.GetCmp<TextField>("ProductLabel").Value = product.ProductLabel;
                this.GetCmp<TextArea>("ProductDescription").Value = product.ProductDescription;
                this.GetCmp<ComboBox>("CategoryID").Value = product.CategoryID;
                this.GetCmp<ComboBox>("AccountID").Value = product.AccountID;
                this.GetCmp<ComboBox>("AccountID").ReadOnly = true;
                this.GetCmp<NumberField>("SellingPrice").Value = product.SellingPrice;

                int[] storeids = plRepository.GetAllStore(product);

                foreach (int id in storeids)
                {
                    this.GetCmp<MultiCombo>("Stores").SelectItem("" + id);
                }

            }
        }

        [HttpPost]
        public ActionResult Reset()
        {
            this.GetCmp<FormPanel>("GenericProductForm").Reset(true);
            this.GetCmp<Store>("ProductListStore").Reload();
            this.GetCmp<ComboBox>("AccountID").ReadOnly = false;

            return this.Direct();
        }

        public List<object> ProductModel()
        {
            List<object> list = new List<object>();

            foreach (GenericProduct p in db.GenericProducts.ToList())
            {
                list.Add(
                    new
                    {
                        ProductID = p.ProductID,
                        ProductCode = p.ProductCode,
                        ProductLabel = p.ProductLabel,
                        ProductDescription = p.ProductDescription,
                        CategoryLabel = p.Category.CategoryLabel,
                        AccountLabel = p.Account.AccountLabel,
                        SellingPrice = p.SellingPrice,

                    }
                   );
            }

            return list;
        }

        [HttpPost]
        public StoreResult GetAllProducts()
        {
            return this.Store(ProductModel());
        }

    }
}