using FatSod.DataContext.Initializer;
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
using AutoMapper;

namespace CABOPMANAGEMENT.Areas.Supply.Controllers
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
        [OutputCache(Duration = 3600)] 
        public ActionResult GenericProduct(ProductModel productModel)
        {
            
            //ces  lignes qui suivent permettent de transformer un Product en ProductModel 
            //create a Map
            Mapper.CreateMap<GenericProduct, ProductModel>()
                .ForMember(pm => pm.Category, m => m.MapFrom(p => db.Categories.Find(p.CategoryID).CategoryLabel))
                .ForMember(pm => pm.Account, m => m.MapFrom(p => db.Accounts.Find(p.AccountID).AccountLabel));
                
            productModel.Products = ProductModel();
           
            return View(productModel);
        }

        /*** methodes lors du chargement du formulaire*********************/
        public JsonResult GetCategories()
        {

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
        public JsonResult GetAccounts()
        {

            List<object> accountList = new List<object>();
            List<Account> accounts = LoadComponent.Accounts(CodeValue.Accounting.DefaultCodeAccountingSection.CODEPROD);
            foreach (Account acct in accounts)
            {
                accountList.Add(new
                {
                    ID = acct.AccountID,
                    Name = acct.AccountLabel
                });
            }

            return Json(accountList, JsonRequestBehavior.AllowGet);
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
        
        public JsonResult AddManager()
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


        public JsonResult AddProduct(GenericProduct product)
        {
            bool status = false;
            string Message = "";
            try
            {
                List<int> newStore = product.Stores.ToList();
                newStore.Remove(0);
                product.Stores = newStore.ToArray();
                
                if (((db.GenericProducts.FirstOrDefault(c => c.ProductCode == product.ProductCode || c.ProductLabel == product.ProductLabel) == null)))
                {
                    product = genericProductRepository.Create(product);
                    plRepository.CreateStore(product, product.Stores);
                    Message = "The Product " + product.ProductCode + " has been successfully created";
                    status = true;
                    return new JsonResult { Data = new { status = status, Message = Message } };
                }
                else
                {
                    status = false;
                    Message = @"Une catégorie ayant le code " + product.ProductCode + " et / ou le label " + product.ProductLabel + " existe déjà!<br/>"
                                            + "veuillez changer de code et / ou le label";
                    return new JsonResult { Data = new { status = status, Message = Message } };
                }
                
            }
            catch (Exception e)
            {
                status = false;
                Message = @"Une erreur s'est produite lors de l'opération, veuillez contactez l'administrateur et/ou essayez à nouveau<br/>" + e.Message;
                return new JsonResult { Data = new { status = status, Message = Message } };
            }
        }

       

        /// <summary>
        /// cette méthode est appelée lorqu' après avoir clicquer sur l'icone modifier du tableau on modifie le formulaire et clique sur enregistrer
        /// </summary>
        /// <returns></returns>
        
        public JsonResult UpdateProduct(GenericProduct product)
        {
            bool status = false;
            string Message = "";
            try
            {

                /*List<int> newStore = product.Stores.ToList();
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
                }*/

                GenericProduct existingProduct = db.GenericProducts.Find(product.ProductID);
                List<GenericProduct> products = db.GenericProducts.ToList();
                products.Remove(existingProduct);

                if (((products.FirstOrDefault(p => p.ProductCode == product.ProductCode || p.ProductLabel == product.ProductLabel) == null)))
                {
                    genericProductRepository.Update(product, product.ProductID);
                    Message = "The Product " + product.ProductCode + " has been successfully updated";
                    status = true;
                    return new JsonResult { Data = new { status = status, Message = Message } };
                }
                else
                {
                    status = false;
                    Message = @"Une catégorie ayant le code " + product.ProductCode + " et / ou le label " + product.ProductLabel + " existe déjà!<br/>"
                                            + "veuillez changer de code et / ou le label";
                    return new JsonResult { Data = new { status = status, Message = Message } };
                }
                
            }
            catch (Exception e)
            {
                status = false;
                Message = @"Une erreur s'est produite lors de la mise à jour, veuillez contactez l'administrateur et/ou essayez à nouveau
                                    <br/>Code : <code>" + e.Message + "</code>";
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
                GenericProduct deletedProduct = db.GenericProducts.Find(ProductID);

                if (IsNoStockCreated(deletedProduct) == false)
                {
                    status = false;
                    Message = "Product - The Product " + deletedProduct.ProductCode + " has already been used to create one or many stocks!" +
                                         "If you realy want to delete this location, you will firt of all delete all thus stocks";
                    return new JsonResult { Data = new { status = status, Message = Message } };
                }

                if (IsNoCustomerOrderAffected(deletedProduct) == false)
                {
                    status = false;
                    Message = "Product - The Product " + deletedProduct.ProductCode + " has already been used in one or many customer orders!" +
                                         "If you realy want to delete this location, you will firt of all delete all thus customer ordres";
                    return new JsonResult { Data = new { status = status, Message = Message } };
                }

                if (IsNoSupplierOrderAffected(deletedProduct) == false)
                {
                    status = false;
                    Message = "Product - The Product " + deletedProduct.ProductCode + " has already been used in one or many supplier orders!" +
                                         "If you realy want to delete this location, you will firt of all delete all thus supplier ordres";
                    return new JsonResult { Data = new { status = status, Message = Message } };
                }

                if (IsNoSaleAffected(deletedProduct) == false)
                {
                    status = false;
                    Message = "Product - The Product " + deletedProduct.ProductCode + " has already been used in one or many sales!" +
                                         "If you realy want to delete this location, you will firt of all delete all thus sales";
                    return new JsonResult { Data = new { status = status, Message = Message } };
                }

                if (IsNoPurchaseAffected(deletedProduct) == false)
                {
                    status = false;
                    Message = "Product - The Product " + deletedProduct.ProductCode + " has already been used in one or many Purchases!" +
                                         "If you realy want to delete this location, you will firt of all delete all thus Purchases";
                    return new JsonResult { Data = new { status = status, Message = Message } };
                }
                db.GenericProducts.Remove(deletedProduct);
                db.SaveChanges();
                status = true;
                Message = "Product - The Product " + deletedProduct.ProductCode + " has been successfully deleted";
                return new JsonResult { Data = new { status = status, Message = Message } };
            }
            catch (Exception e)
            {
                status = false;
                Message = "Product - L'erreur " + e.Message + " s'est produite lors de la suppression! veuillez contactez l'administrateur et/ou essayez à nouveau";
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
        /// cette méthode vérifie si ce produit n'a pas encore été utilisé dans une commande client
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        bool IsNoCustomerOrderAffected(GenericProduct product)
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

        public JsonResult InitializeProductFields(int ID)
        {
            List<object> list = new List<object>();
            if (ID > 0)
            {
                GenericProduct product = new GenericProduct();
                product = db.GenericProducts.Find(ID);
                
                int[] storeids = plRepository.GetAllStore(product);


                list.Add(new
                {
                    ProductID = product.ProductID,
                    ProductCode = product.ProductCode,
                    ProductLabel = product.ProductLabel,
                    ProductDescription = product.ProductDescription,
                    CategoryID = product.CategoryID,
                    AccountID = product.AccountID,
                    Stores = storeids,
                    SellingPrice=product.SellingPrice
                });
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public List<Product> ProductModel()
        {
            List<Product> list = new List<Product>();

            foreach (GenericProduct p in db.GenericProducts.ToList())
            {
                list.Add(
                    new Product
                    {
                        ProductID = p.ProductID,
                        ProductCode = p.ProductCode,
                        ProductLabel = p.ProductLabel,
                        ProductDescription = p.ProductDescription,
                        Category = p.Category,
                        Account = p.Account,
                        SellingPrice = p.SellingPrice,

                    }
                   );
            }

            return list;
        }

        
        public JsonResult GetAllProducts()
        {
            var model = new
            {
                data = from p in ProductModel()
                select new
                {
                    ProductID = p.ProductID,
                    ProductCode = p.ProductCode,
                    ProductLabel = p.ProductLabel,
                    ProductDescription = p.ProductDescription,
                    CategoryLabel = p.Category.CategoryLabel,
                    AccountLabel = p.Account.AccountLabel,
                    SellingPrice = p.SellingPrice,
                }
            };
            return Json(model, JsonRequestBehavior.AllowGet);
        }

    }
}