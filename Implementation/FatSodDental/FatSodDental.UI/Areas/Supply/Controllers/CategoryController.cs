using Ext.Net;
using Ext.Net.MVC;
using FatSod.DataContext.Concrete;
using FatSod.DataContext.Initializer;
using FatSod.Ressources;
using FatSod.Security.Abstracts;
using FatSod.Supply.Abstracts;
using FatSod.Supply.Entities;
using FatSodDental.UI.Controllers;
using FatSodDental.UI.Filters;
using FatSodDental.UI.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI;
using ExtPartialViewResult = Ext.Net.MVC.PartialViewResult;

namespace FatSodDental.UI.Areas.Supply.Controllers
{
    [Authorize(Order = 1)]
    [TakeBusinessDay(Order = 2)]
    public class CategoryController : BaseController
    {
        private IRepositorySupply<Category> categoryRepository;

        private const string CONTROLLER_NAME = "Supply/" + CodeValue.Supply.CategoryMenu.CONTROLLER;
        private const string VIEW_NAME = CodeValue.Supply.CategoryMenu.PATH;
        

        public CategoryController(IRepositorySupply<Category> categoryRepository)
        {
            this.categoryRepository = categoryRepository;
            
        }

        /// <summary>
        /// the view of this action method is stronglyTyped with FatSod.Supply.Entities.Category
        /// </summary>
        /// <returns>ActionResult</returns>
        [OutputCache(Duration = 3600)] 
        public ActionResult Categorie(Category category)
        {

            //We verify if the current user has right to access view which this action calls
            //if (!LoadAction.isAutoriseToGetMenu(SessionProfileID, CodeValue.Supply.CategoryMenu.CODE, db))
            //{
            //    this.NotAuthorized();
            //}

            //Session["Curent_Page"] = VIEW_NAME;
            //Session["Curent_Controller"] = CONTROLLER_NAME;

            //ExtPartialViewResult rPVResult = new ExtPartialViewResult
            //{
            //    ClearContainer = true,
            //    //RenderMode = RenderMode.AddTo,
            //    ContainerId = "Body",
            //    //WrapByScriptTag = false,
            //    Model = CategoryModel()
            //};

            return View (CategoryModel());
        }


        [HttpPost]
        public ActionResult AddManager()
        {
            Category category = new Category();
            TryUpdateModel(category);

            //en cas de mise à jour
            if (category.CategoryID > 0)
            {
                return this.UpdateCategory(category);
            }
            else
            {
                return this.AddCategory(category);
            }
        }

        public ActionResult AddCategory(Category category)
        {
            try
            {

                if (((db.Categories.FirstOrDefault(c => c.CategoryCode == category.CategoryCode || c.CategoryLabel == category.CategoryLabel) == null)))
                {
                    categoryRepository.Create(category);
                    statusOperation = "The Category " + category.CategoryCode + " has been successfully created";
                    this.AlertSucces(Resources.Success, statusOperation);
                    return this.Reset();
                }
                else
                {
                    String statusOperation1 = @"Une catégorie ayant le code " + category.CategoryCode + " et / ou le label " + category.CategoryLabel + " existe déjà!<br/>"
                                        + "veuillez changer de code et / ou le label";

                    statusOperation = Resources.er_alert_danger + statusOperation1;
                    X.Msg.Alert(Resources.Category, statusOperation).Show();

                    return this.Direct();
                }

            }
            catch (Exception e)
            {
                statusOperation = Resources.er_alert_danger + e.Message;
                X.Msg.Alert(Resources.Category, statusOperation).Show();

                return this.Direct();
            }
        }

        /// <summary>
        /// cette méthode est appelée quand on clicque sur l'icone modifier du tableau
        /// </summary>
        /// <param name="localizationID"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult UpdateCategory(int CategoryID)
        {
            this.InitializePurchaseFields(CategoryID);

            return this.Direct();

        }

        /// <summary>
        /// cette méthode est appelée lorqu' après avoir clicquer sur l'icone modifier du tableau on modifie le formulaire et clique sur enregistrer
        /// </summary>
        /// <returns></returns>
        ////[HttpPost]
        public ActionResult UpdateCategory(Category category)
        {
            try
            {
                Category existingCategory = db.Categories.Find(category.CategoryID);
                List<Category> categories = db.Categories.ToList();
                categories.Remove(existingCategory);

                if (((categories.FirstOrDefault(c => c.CategoryCode == category.CategoryCode || c.CategoryLabel == category.CategoryLabel) == null)))
                {
                    categoryRepository.Update(category, category.CategoryID);
                    statusOperation = "The Category " + category.CategoryCode + " has been successfully updated";
                    this.AlertSucces(Resources.Success, statusOperation);
                    return this.Reset();
                }
                else
                {
                    statusOperation = @"Une catégorie ayant le code " + category.CategoryCode + " et / ou le label " + category.CategoryLabel + " existe déjà!<br/>"
                                        + "veuillez changer de code et / ou le label";
                    X.Msg.Alert(Resources.Category, statusOperation).Show();
                    return this.Direct();
                }

                return this.Direct(); ;

            }
            catch (Exception e)
            {
                TempData["alertType"] = "alert alert-danger";
                TempData["status"] = @"Une erreur s'est produite lors de la mise à jour, veuillez contactez l'administrateur et/ou essayez à nouveau
                                    <br/>Code : <code>" + e.Message + "</code>";
                return this.Direct(); ;
            }
        }

        [HttpPost]
        public ActionResult DeleteCategory(int CategoryID)
        {
            try
            {
                Category deletedCategoy = db.Categories.Find(CategoryID);
                List<Product> categoryProducts = db.Products.Where(p => p.CategoryID == deletedCategoy.CategoryID).ToList();

                if (deletedCategoy.CategoryCode == CodeValue.Supply.lensCategoryCode)
                {
                    X.Msg.Alert("UnDiletable Category", "You can Not Delete this Category").Show();
                    return this.Direct();
                }

                if ((categoryProducts != null) || (categoryProducts.Count != 0))
                {
                    db.Categories.Remove(deletedCategoy);
                    db.SaveChanges();
                    //categoryRepository.Delete(deletedCategoy);
                    //TempData["status"] = "The Category " + deletedCategoy.CategoryCode + " has been successfully deleted";
                    //TempData["alertType"] = "alert alert-success";
                    X.Msg.Alert("Category", "The Category " + deletedCategoy.CategoryCode + " has been successfully deleted").Show();
                    return this.Reset();
                }
                else
                {
                    //TempData["alertType"] = "alert alert-danger";
                    //TempData["status"] = @"Désolé vous ne pouvez pas supprimer la catégorie " + deletedCategoy.CategoryCode + " parcequ'elle contient déjà des produits"
                    //                  + "pour supprimer cette catégorie, il faut d'abort supprimer ses produits";
                    X.Msg.Alert("Category", @"Désolé vous ne pouvez pas supprimer la catégorie " + deletedCategoy.CategoryCode + " parcequ'elle contient déjà des produits"
                                      + "pour supprimer cette catégorie, il faut d'abort supprimer ses produits").Show();
                    return this.Direct();
                }
            }
            catch (Exception e)
            {
                //TempData["alertType"] = "alert alert-danger";
                //TempData["status"] = @"L'erreur" + e.Message + "s'est produite lors de la suppression! veuillez contactez l'administrateur et/ou essayez à nouveau";
                X.Msg.Alert("Category",  @"L'erreur " + e.Message + " s'est produite lors de la suppression! veuillez contactez l'administrateur et/ou essayez à nouveau").Show();
                return this.Direct();
            }
        }

        [HttpPost]
        public ActionResult Reset()
        {
            this.GetCmp<FormPanel>("CategoryForm").Reset(true);
            this.GetCmp<Store>("CategoryListStore").Reload();
            return this.Direct();
        }

        public void InitializePurchaseFields(int ID)
        {

            this.GetCmp<FormPanel>("CategoryForm").Reset(true);
            this.GetCmp<Store>("CategoryListStore").Reload();

            if (ID > 0)
            {
                 Category category = db.Categories.Find(ID);

                this.GetCmp<TextField>("CategoryID").Value = category.CategoryID;
                this.GetCmp<TextField>("CategoryCode").Value = category.CategoryCode;
                this.GetCmp<TextField>("CategoryLabel").Value = category.CategoryLabel;
                this.GetCmp<TextArea>("CategoryDescription").Value = category.CategoryDescription;
                if (category.CategoryCode == CodeValue.Supply.lensCategoryCode)
                {
                    this.GetCmp<TextField>("CategoryCode").ReadOnly = true;
                }
            }
        }

        private ActionResult NotAuthorized()
        {
            return View();
        }

        public List<object> CategoryModel()
        {
            List<Category> dataTmp = LoadComponent.GetAllGenericCategories().ToList();
            List<object> list = new List<object>();

            foreach (Category c in dataTmp)
            {
                list.Add(
                    new
                    {
                        CategoryID = c.CategoryID,
                        CategoryCode = c.CategoryCode,
                        CategoryLabel = c.CategoryLabel,
                        CategoryDescription = c.CategoryDescription,
                    }
                   );
            }

            return list;
        }

        [HttpPost]
        public StoreResult GetAllCategories()
        {

            return this.Store(CategoryModel());
        }
    }
}