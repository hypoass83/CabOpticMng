using FatSod.DataContext.Initializer;
using FatSod.Ressources;
using FatSod.Supply.Abstracts;
using FatSod.Supply.Entities;
using CABOPMANAGEMENT.Areas.Supply.ViewModel;
using CABOPMANAGEMENT.Controllers;
using CABOPMANAGEMENT.Filters;
using CABOPMANAGEMENT.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace CABOPMANAGEMENT.Areas.Supply.Controllers
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
            Session["dejacharge"] = dejacharger;
            Session["LensCategoryID"] = 0;
            Session["Stores"] = 0;
            return View(GetProductModel());
        }

        
        public ProductModel GetProductModel()
        {
                ProductModel res = new ProductModel();
                int LensCategoryID = (int)Session["LensCategoryID"];
                int Stores = (int)Session["Stores"];
                res.Lenses = ModelLens(LensCategoryID, Stores);
                return res;
            
        }

        /*** methodes lors du chargement du formulaire*********************/
        public JsonResult GetCategories()
        {

            List<object> categoryList = new List<object>();
            List<LensCategory> categories = LoadComponent.GetLensCategories();
            foreach (LensCategory cat in categories)
            {
                categoryList.Add(new
                {
                    CategoryID = cat.CategoryID,
                    Name = cat.CategoryCode
                });
            }

            return Json(categoryList, JsonRequestBehavior.AllowGet);
        }
       
        public JsonResult GetStores()
        {
            List<object> storesList = new List<object>();
            List<Localization> stores = db.Localizations.ToList();
            foreach (Localization loc in stores)
            {
                storesList.Add(new
                {
                    ID = loc.LocalizationID,
                    Name = loc.LocalizationCode
                });
            }

            return Json(storesList, JsonRequestBehavior.AllowGet);
        }

        /* public ActionResult OneByOne()
         {
             return View(GetProductModel());
         }

         public ActionResult ByRange()
         {
             return View(GetProductModel());

         }*/

        public List<Lens> ModelLens(int LensCategoryID,int Stores)
        {

            List<Lens> dataTmp = new List<Lens>();

            if (LensCategoryID>0 && Stores >0)
            {
                dataTmp = (from ls in db.Lenses
                           where ls.LensCategoryID == LensCategoryID && ls.LocalizationID == Stores
                           select ls)
                            .OrderBy(a => a.ProductCode)
                            //.Take(5000)
                            .ToList();
            }
                
            
            
            List<Lens> list =  new List<Lens>();

            foreach (var p in dataTmp)
            {
                list.Add(
                    new Lens
                    {
                        ProductID = p.ProductID,
                        ProductCode = p.ProductCode,
                        ProductLabel = p.ProductLabel,
                        ProductDescription = p.ProductDescription,
                        Category = p.Category,
                        Account = p.Account,
                        LensCategoryName = p.LensCategoryName,
                        LensNumberFullCode = p.LensNumberFullCode
                    }
                   );
            }

            return list;
        }
        
        public JsonResult AddManager(Lens product, LensNumber number)
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

        
        public JsonResult AddRange(LensRangeModel product)
        {
            bool status = false;
            string Message = "";
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
                    Message = Resources.a_NumberRange + "-" + statusOperation;
                    status = false;
                    return new JsonResult { Data = new { status = status, Message = Message } };
                }


                LensCategory categoryCode = db.LensCategories.Find(product.LensCategoryID);

                /*int lensecreated =*/
                lensRepository.CreateLens(categoryCode.CategoryCode, sphericalRange, cylindricalRange, additionRange, product.Stores);

                Session["LensCategoryID"] = product.LensCategoryID;
                Session["Stores"] = product.Stores;

                statusOperation = "Lenses Have Been successfully created";
                Message = Resources.Success + "-" + statusOperation;
                status = true;
                return new JsonResult { Data = new { status = status, Message = Message } };

            }
            catch (Exception e)
            {
                statusOperation = Resources.er_alert_danger + e.Message + " Inner Exception = " + e.InnerException;
                Message = Resources.d_LensNumber + "-" + statusOperation;
                status = false;
                return new JsonResult { Data = new { status = status, Message = Message } };
            }

        }
        
     
        public JsonResult AddProduct(Lens product)
        {
            bool status = false;
            string Message = "";

            LensCategory categoryCode = db.LensCategories.Find(product.CategoryID);
           
            try
            {
                
                List<int> newStore = product.Stores.ToList();
                newStore.Remove(0);

                product.Stores = newStore.ToArray();
                product = lensRepository.CreateLens(categoryCode.CategoryCode, product.LensNumber, product.Stores);

                statusOperation = "The Lens " + product.ProductCode + " has been successfully created";
                Message = Resources.Success + "-" + statusOperation;
                status = true;
                return new JsonResult { Data = new { status = status, Message = Message } };


            }
            catch (Exception e)
            {
                statusOperation = Resources.er_alert_danger + e.Message + " Inner Exception = " + e.InnerException;
                Message = Resources.c_LensColour + "-" + statusOperation;
                status = false;
                return new JsonResult { Data = new { status = status, Message = Message } };
            }
        }

        

        /// <summary>
        /// cette méthode est appelée lorqu' après avoir clicquer sur l'icone modifier du tableau on modifie le formulaire et clique sur enregistrer
        /// </summary>
        /// <returns></returns>
      
        public JsonResult UpdateProduct(Lens product)
        {
            bool status = false;
            string Message = "";

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
                status = true;
                Message = Resources.Success + "-" + statusOperation;
                return new JsonResult { Data = new { status = status, Message = Message } };


            }
            catch (Exception e)
            {
                status = false;
                string statusmsg = @"Une erreur s'est produite lors de la mise à jour, veuillez contactez l'administrateur et/ou essayez à nouveau
                
                    <br/>Code : <code>" + e.Message + "</code>";

                statusOperation = Resources.er_alert_danger + statusmsg + " car " + e.Message;
                Message = Resources.Product + "-" + statusOperation;

                return new JsonResult { Data = new { status = status, Message = Message } };
            }
        }

        
        public JsonResult DeleteProduct(int ProductID)
        {
            bool status = false;
            string Message = "";
            try
            {
                plRepository.DeleteAllStore(ProductID);
                Lens deletedProduct = db.Lenses.FirstOrDefault(c => c.ProductID == ProductID);


                if (IsNoStockCreated(deletedProduct) == false)
                {
                    status = false;
                    string statusmsg = "The Product " + deletedProduct.ProductCode + " has already been used to create one or many stocks!" +
                                         "If you realy want to delete this location, you will firt of all delete all thus stocks";
                    statusOperation = Resources.er_alert_danger + statusmsg;
                    Message = Resources.Product+"-"+statusOperation;
                    return new JsonResult { Data = new { status = status, Message = Message } };
                }

                if (IsNoCustomerOrderAffected(deletedProduct) == false)
                {
                    status = false;
                    string statusmsg = "The Product " + deletedProduct.ProductCode + " has already been used in one or many customer orders!" +
                                         "If you realy want to delete this location, you will firt of all delete all thus customer ordres";
                    statusOperation = Resources.er_alert_danger + statusmsg;
                    Message = Resources.Product + "-" + statusOperation;
                    return new JsonResult { Data = new { status = status, Message = Message } };
                }

                if (IsNoSupplierOrderAffected(deletedProduct) == false)
                {
                    status = false;
                    string statusmsg = "The Product " + deletedProduct.ProductCode + " has already been used in one or many supplier orders!" +
                                         "If you realy want to delete this location, you will firt of all delete all thus supplier ordres";
                    statusOperation = Resources.er_alert_danger + statusmsg;
                    Message = Resources.Product + "-" + statusOperation;
                    return new JsonResult { Data = new { status = status, Message = Message } };
                }

                if (IsNoSaleAffected(deletedProduct) == false)
                {
                    status = false;
                    string statusmsg = "The Product " + deletedProduct.ProductCode + " has already been used in one or many sales!" +
                                         "If you realy want to delete this location, you will firt of all delete all thus sales";

                    statusOperation = Resources.er_alert_danger + statusmsg;
                    Message = Resources.Product + "-" + statusOperation;

                    return new JsonResult { Data = new { status = status, Message = Message } };
                }

                if (IsNoPurchaseAffected(deletedProduct) == false)
                {
                    status = false;
                    string statusmsg = "The Product " + deletedProduct.ProductCode + " has already been used in one or many Purchases!" +
                                         "If you realy want to delete this location, you will firt of all delete all thus Purchases";
                    statusOperation = Resources.er_alert_danger + statusmsg;
                    Message = Resources.Product + "-" + statusOperation;
                    return new JsonResult { Data = new { status = status, Message = Message } };
                }
                db.Lenses.Remove(deletedProduct);
                db.SaveChanges();
                
                statusOperation = "The Product " + deletedProduct.ProductCode + " has been successfully deleted";
                Message = Resources.Success + "-" + statusOperation;
                return new JsonResult { Data = new { status = status, Message = Message } };
            }
            catch (Exception e)
            {
                status = false;
                string statusmsg = @"L'erreur " + e.Message + " s'est produite lors de la suppression! veuillez contactez l'administrateur et/ou essayez à nouveau";

                statusOperation = Resources.er_alert_danger + statusmsg;
                Message = Resources.Product + "-" + statusOperation;

                return new JsonResult { Data = new { status = status, Message = Message } };
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
        /// cette méthode vérifie si ce produit n'a pas encore été utilisé dans une commande client
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        bool IsNoCustomerOrderAffected(Lens product)
        {
            bool res = false;
            List<CustomerOrderLine> ol = db.CustomerOrderLines.Where(col => col.ProductID == product.ProductID).ToList();
            if (ol == null || ol.Count == 0)
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

       

        public JsonResult InitializeProductFields(int ID)
        {

            List<object> list = new List<object>();

            if (ID > 0)
            {
                Lens product = new Lens();
                product = db.Lenses.Find(ID);
                
                int[] storeids = plRepository.GetAllStore(product);

                
                list.Add(new
                {
                    ProductID = product.ProductID,
                    CategoryID = product.CategoryID,

                    LensNumberID = product.LensNumber.LensNumberID,
                    LensNumberSphericalValue = product.LensNumber.LensNumberSphericalValue,
                    LensNumberCylindricalValue = product.LensNumber.LensNumberCylindricalValue,
                    LensNumberAdditionValue = product.LensNumber.LensNumberAdditionValue,
                    Stores = storeids
                });
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }

       
        public JsonResult GetAllProducts(int LensCategoryID, int Stores)
        {
            var model = new
            {
                data = from p in ModelLens(LensCategoryID, Stores)
                select new
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
            };
            return Json(model, JsonRequestBehavior.AllowGet);
        }
    }
}