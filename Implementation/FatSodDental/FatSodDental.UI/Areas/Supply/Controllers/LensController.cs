using AutoMapper;
using Ext.Net;
using Ext.Net.MVC;
using FatSod.DataContext.Concrete;
using FatSod.DataContext.Initializer;
using FatSod.Ressources;
using FatSod.Security.Abstracts;
using FatSod.Supply.Abstracts;
using FatSod.Supply.Entities;
using FatSodDental.UI.Areas.Supply.ViewModel;
using FatSodDental.UI.Controllers;
using FatSodDental.UI.Filters;
using FatSodDental.UI.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI;
using ExtPartialViewResult = Ext.Net.MVC.PartialViewResult;

namespace FatSodDental.UI.Areas.Supply.Controllers
{
    [Authorize(Order = 1)]
    [TakeBusinessDay(Order = 2)]
    public class LensController : BaseController
    {
        private ILens lensRepository;
        private IProductLocalization plRepository;

        private const string CONTROLLER_NAME = "Supply/" + CodeValue.Supply.LensProduct_SM.CONTROLLER;
        private const string VIEW_NAME = CodeValue.Supply.LensProduct_SM.PATH;

        private const bool dejacharger = true;
        
        public LensController(ILens productRepository,
                                IProductLocalization plRepository
            )
        {
            this.lensRepository = productRepository;
            this.plRepository = plRepository;
            
        }

        /// <summary>
        /// the view of this action method is stronglyTyped with FatSod.Supply.Entities.Product
        /// </summary>
        /// <returns>ActionResult</returns>
        /// 
        [OutputCache(Duration = 3600)] 
        public ActionResult Lens()
        {

            //We verify if the current user has right to access view which this action calls
            //if (!LoadAction.isAutoriseToGetSubMenu(SessionProfileID, CodeValue.Supply.LensProduct_SM.CODE, db))
            //{
            //    this.NotAuthorized();
            //}

            //Session["Curent_Page"] = VIEW_NAME;
            //Session["Curent_Controller"] = CONTROLLER_NAME;

            Session["dejacharge"] = dejacharger;

            

            //ExtPartialViewResult rPVResult = new ExtPartialViewResult
            //{
            //    ClearContainer = true,
            //    //RenderMode = RenderMode.AddTo,
            //    ContainerId = "Body",
            //    //WrapByScriptTag = false,
            //    Model = GetProductModel()
            //};

            return View(GetProductModel());
        }

        public ActionResult loadGrid()
        {
            this.GetCmp<Store>("Store").Reload();
            return this.Direct();
        }
        public ProductModel GetProductModel()
        {
                ProductModel res = new ProductModel();
                res.Categories = LoadComponent.GetLensCategories();
                res.AccountNumbers = LoadComponent.Accounts(CodeValue.Accounting.DefaultCodeAccountingSection.CODEPROD);
                res.Products = ModelLens("","");
                return res;
            
        }

        public ActionResult OneByOne()
        {
            this.Reset();
            //Ext.Net.MVC.PartialViewResult rPVResult = new Ext.Net.MVC.PartialViewResult
            //{
            //    ClearContainer = true,
            //    //RenderMode = RenderMode.AddTo,
            //    ContainerId = "Body",
            //    //WrapByScriptTag = false,
            //    Model = GetProductModel()
            //};

            //return rPVResult;
            return View(GetProductModel());
        }

        public ActionResult ByRange()
        {
            this.Reset();
            //Ext.Net.MVC.PartialViewResult rPVResult = new Ext.Net.MVC.PartialViewResult
            //{
            //    ClearContainer = true,
            //    //RenderMode = RenderMode.AddTo,
            //    ContainerId = "Body",
            //    //WrapByScriptTag = false,
            //    Model = GetProductModel()
            //};

            //return rPVResult;
            return View(GetProductModel());

        }

        public List<object> ModelLens(String SearchOption, String SearchValue)
        {

            

            List<Lens> dataTmp = new List<Lens>();

            if (SearchValue == null || SearchValue == "" || SearchOption == null || SearchOption == "")
            {
                dataTmp = (from ls in db.Lenses
                               select ls)
                               .OrderBy(a => a.ProductCode)
                                         .Take(5000)
                                         .ToList();
            }
            else
            {
                if (SearchOption == "CODE") //si recherche par code
                {
                    dataTmp = (from ls in db.Lenses
                                   where ls.ProductCode.Contains(SearchValue)
                                   select ls)
                                   .OrderBy(a => a.ProductCode)
                                             .Take(5000)
                                             .ToList();
                }

                if (SearchOption == "LN") //si recherche par lens number
                {
                    dataTmp = (from ls in db.Lenses
                                   where ls.LensNumber.LensNumberDescription.Contains(SearchValue)
                                   select ls)
                                   .OrderBy(a => a.ProductCode)
                                             .Take(5000)
                                             .ToList();
                }


            }
            
            List<object> list =  new List<object>();

            foreach (var p in dataTmp)
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
                        LensCategoryName = p.LensCategoryName,
                        LensNumberFullCode = p.LensNumberFullCode
                    }
                   );
            }

            return list;
        }
        [HttpPost]
        public ActionResult AddManager(Lens product, LensNumber number)
        {
            //product.Stores = Stores;
            //en cas de mise à jour
            if (product.ProductID > 0)
            {
                product.LensNumber = number;
                return this.UpdateProduct(product);
            }
            else
            {
                product.LensNumber = number;
                return this.AddProduct(product);
            }
        }

        [HttpPost]
        public ActionResult AddRange(LensRangeModel product)
        {
            try
            {
                List<int> newStore = product.Stores.ToList();
                newStore.Remove(0);
                product.Stores = newStore.ToArray();

                LensNumberRange sphericalRange = new LensNumberRange(product.SphMin, product.SphMax);
                LensNumberRange cylindricalRange = new LensNumberRange(product.CylMin, product.CylMax);
                LensNumberRange additionRange = new LensNumberRange(product.AddMin, product.AddMax);


                bool a = sphericalRange != null && sphericalRange.IsValid();
                bool b = cylindricalRange != null && cylindricalRange.IsValid();
                bool c = additionRange != null && additionRange.IsValid();

                bool d =    (a && b && c)   //Verres Composés avec addition
                         || (a && b)       //Verres Composés sans addition
                         || (a && c)      //Verres sphériques avec addition
                         || (a)          //Verres simplement sphériques
                         || (b && c)    //Verres cylindrique avec addition
                         || (b);       //Verres simplement cylindrique

                if (!d)
                {
                    statusOperation = Resources.er_alert_danger + " " + " One or many Ranges are Invalid";
                    X.Msg.Alert(Resources.a_NumberRange, statusOperation).Show();
                    return this.Direct();
                }


                LensCategory categoryCode = db.LensCategories.Find(product.LensCategoryID);

                /*int lensecreated =*/
                lensRepository.CreateLens(categoryCode.CategoryCode, sphericalRange, cylindricalRange, additionRange, product.Stores);

                statusOperation = "Lenses Have Been successfully created";
                this.AlertSucces(Resources.Success, statusOperation);

                return this.Reset();

            }
            catch (Exception e)
            {
                statusOperation = Resources.er_alert_danger + e.Message + " Inner Exception = " + e.InnerException;
                X.Msg.Alert(Resources.c_LensColour, statusOperation).Show();

                return this.Direct();
            }

        }

        private int GetLensMaterialID(string LensMaterialCode)
        {
            return db.LensMaterials.SingleOrDefault(lm => lm.LensMaterialCode == LensMaterialCode).LensMaterialID;
        }

        private int GetLensColourID(string LensColourCode)
        {
            if (LensColourCode == null || LensColourCode.Length == 0)
            {
                return db.LensColours.SingleOrDefault(lm => lm.LensColourCode == CodeValue.Supply.DefaultLensColour).LensColourID;
            }

            if (LensColourCode != null && LensColourCode.Length > 0)
            {
                return db.LensColours.SingleOrDefault(lm => lm.LensColourCode == LensColourCode).LensColourID;
            }
            return 0;
        }

        int GetLensCoatingID(string LensCoatingCode)
        {
            if (LensCoatingCode == null || LensCoatingCode.Length == 0)
            {
                return db.LensCoatings.SingleOrDefault(lm => lm.LensCoatingCode == CodeValue.Supply.DefaultLensCoating).LensCoatingID;
            }

            if (LensCoatingCode != null && LensCoatingCode.Length > 0)
            {
                return db.LensCoatings.SingleOrDefault(lm => lm.LensCoatingCode == LensCoatingCode).LensCoatingID;
            }
            return 0;
        }
        
        [HttpPost]
        public ActionResult AddProduct(Lens product)
        {
            LensCategory categoryCode = db.LensCategories.Find(product.CategoryID);
           
            try
            {
                
                List<int> newStore = product.Stores.ToList();
                newStore.Remove(0);

                product.Stores = newStore.ToArray();
                product = lensRepository.CreateLens(categoryCode.CategoryCode, product.LensNumber, product.Stores);

                statusOperation = "The Lens " + product.ProductCode + " has been successfully created";
                this.AlertSucces(Resources.Success, statusOperation);

                return this.Reset();
                //}
                //else
                //{
                //    statusOperation = @"Une catégorie ayant le code " + product.ProductCode + " et / ou le label " + product.ProductLabel + " existe déjà!<br/>"
                //                        + "veuillez changer de code et / ou le label";
                //    X.Msg.Alert(Resources.Success, statusOperation);

                //    return this.Direct();
                //}

            }
            catch (Exception e)
            {
                statusOperation = Resources.er_alert_danger + e.Message + " Inner Exception = " + e.InnerException;
                X.Msg.Alert(Resources.c_LensColour, statusOperation).Show();

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
        public ActionResult UpdateProduct(Lens product)
        {

            List<int> newStore = product.Stores.ToList();
            List<int> oldStore = plRepository.GetAllStore(product).ToList();

            newStore.Remove(0);
            oldStore.Remove(0);
            product.Stores = newStore.ToArray();

            bool a = newStore != null && newStore.Count() > 0;
            bool b = oldStore != null && oldStore.Count() > 0;
            bool c = a && b;

            if(!c){

                //if(a == false && b == false){}

                if(a == false && b == true)//On a aucun nouveau ou il y avait des anciens
                {
                    plRepository.DeleteAllStore(product.ProductID);
                }

                if(a == true && b == false)//on a des nouveaux ou il n'y avait aucun ancien
                {
                    plRepository.CreateStore(product, newStore.ToArray());
                }

            }

            if(c){//il y avait les anciens et il y a les anciens

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

            LensCategory categoryCode = db.LensCategories.Find(product.CategoryID);
            product.ProductCode = categoryCode.CategoryCode + " " + product.LensNumber.LensNumberFullCode;
            product.ProductLabel = categoryCode.CategoryCode;

            try
            {
                Lens existingProduct = db.Lenses.Find(product.ProductID);
                List<Lens> products = db.Lenses.ToList();
                products.Remove(existingProduct);

                lensRepository.CreateLens(categoryCode.CategoryCode, product.LensNumber, product.Stores);
                statusOperation = "The Product " + product.ProductCode + " has been successfully updated";
                this.AlertSucces(Resources.Success, statusOperation);
                return this.Reset();
                //}
                //else
                //{
                //    TempData["alertType"] = "alert alert-danger";
                //    TempData["status"] = @"Une catégorie ayant le code " + product.ProductCode + " et / ou le label " + product.ProductLabel + " existe déjà!<br/>"
                //                        + "veuillez changer de code et / ou le label";
                //    return this.Direct();
                //}

            }
            catch (Exception e)
            {
                string status = @"Une erreur s'est produite lors de la mise à jour, veuillez contactez l'administrateur et/ou essayez à nouveau
                
                    <br/>Code : <code>" + e.Message + "</code>";

                statusOperation = Resources.er_alert_danger + status + " car " + e.Message;
                X.Msg.Alert(Resources.Product, statusOperation).Show();

                return this.Direct();
            }
        }

        [HttpPost]
        public ActionResult DeleteProduct(int ProductID)
        {
            try
            {
                plRepository.DeleteAllStore(ProductID);
                Lens deletedProduct = db.Lenses.FirstOrDefault(c => c.ProductID == ProductID);


                if (IsNoStockCreated(deletedProduct) == false)
                {
                    string status = "The Product " + deletedProduct.ProductCode + " has already been used to create one or many stocks!" +
                                         "If you realy want to delete this location, you will firt of all delete all thus stocks";
                    statusOperation = Resources.er_alert_danger + status;
                    X.Msg.Alert(Resources.Product, statusOperation).Show();
                    return this.Direct();
                }

               

                if (IsNoSupplierOrderAffected(deletedProduct) == false)
                {
                    string status = "The Product " + deletedProduct.ProductCode + " has already been used in one or many supplier orders!" +
                                         "If you realy want to delete this location, you will firt of all delete all thus supplier ordres";
                    statusOperation = Resources.er_alert_danger + status;
                    X.Msg.Alert(Resources.Product, statusOperation).Show();
                    return this.Direct();
                }

                if (IsNoSaleAffected(deletedProduct) == false)
                {
                    string status = "The Product " + deletedProduct.ProductCode + " has already been used in one or many sales!" +
                                         "If you realy want to delete this location, you will firt of all delete all thus sales";

                    statusOperation = Resources.er_alert_danger + status;
                    X.Msg.Alert(Resources.Product, statusOperation).Show();

                    return this.Direct();
                }

                if (IsNoPurchaseAffected(deletedProduct) == false)
                {
                    string status = "The Product " + deletedProduct.ProductCode + " has already been used in one or many Purchases!" +
                                         "If you realy want to delete this location, you will firt of all delete all thus Purchases";
                    statusOperation = Resources.er_alert_danger + status;
                    X.Msg.Alert(Resources.Product, statusOperation).Show();
                    return this.Direct();
                }
                db.Lenses.Remove(deletedProduct);
                db.SaveChanges();
                //lensRepository.Delete(deletedProduct);
                statusOperation = "The Product " + deletedProduct.ProductCode + " has been successfully deleted";
                this.AlertSucces(Resources.Success, statusOperation);
                return this.Reset();
            }
            catch (Exception e)
            {
                string status = @"L'erreur " + e.Message + " s'est produite lors de la suppression! veuillez contactez l'administrateur et/ou essayez à nouveau";

                statusOperation = Resources.er_alert_danger + status;
                X.Msg.Alert(Resources.Product, statusOperation).Show();

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
        bool IsNoStockCreated(Lens product)
        {
            bool res = false;
            List<ProductLocalization> stocks = plRepository.FindAll.Where(pl => pl.ProductID == product.ProductID).ToList();
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
        bool IsNoSupplierOrderAffected(Lens product)
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
        bool IsNoSaleAffected(Lens product)
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
        bool IsNoPurchaseAffected(Lens product)
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

            this.GetCmp<FormPanel>("LensForm").Reset(true);
            this.GetCmp<Store>("Store").Reload();

            if (ID > 0)
            {
                Lens product = new Lens();
                product = db.Lenses.Find(ID);

                this.GetCmp<TextField>("ProductID").Value = product.ProductID;

                this.GetCmp<ComboBox>("CategoryID").Value = product.CategoryID;
                //Numéro du produit
                this.GetCmp<TextField>("LensNumberID").Value = product.LensNumber.LensNumberID;
                this.GetCmp<TextField>("LensNumberSphericalValue").Value = product.LensNumber.LensNumberSphericalValue;
                this.GetCmp<TextField>("LensNumberCylindricalValue").Value = product.LensNumber.LensNumberCylindricalValue;
                this.GetCmp<TextField>("LensNumberAdditionValue").Value = product.LensNumber.LensNumberAdditionValue;

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
            this.GetCmp<FormPanel>("LensForm").Reset(true);
            this.GetCmp<Store>("Store").Reload();
            //this.GetCmp<ComboBox>("AccountID").ReadOnly = false;
            return this.Direct();
        }

        [HttpPost]
        public StoreResult GetAllProducts(String SearchOption, String SearchValue)
        {
            return this.Store(ModelLens(SearchOption, SearchValue));
        }
    }
}