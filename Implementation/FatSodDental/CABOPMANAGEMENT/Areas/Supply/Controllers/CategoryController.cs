using FatSod.DataContext.Initializer;
using FatSod.Ressources;
using FatSod.Supply.Abstracts;
using FatSod.Supply.Entities;
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
        public ActionResult Categorie()
        {
            return View(CategoryModel());
        }


        //[HttpPost]
        public JsonResult AddManager(int isSerialNumberNull=0)
        {
            Category category = new Category();
            TryUpdateModel(category);
            category.isSerialNumberNull = Convert.ToBoolean(isSerialNumberNull);
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

        public JsonResult AddCategory(Category category)
        {
            bool status = false;
            string Message = "";
            try
            {

                if (((db.Categories.FirstOrDefault(c => c.CategoryCode == category.CategoryCode || c.CategoryLabel == category.CategoryLabel) == null)))
                {
                    categoryRepository.Create(category);
                    statusOperation = "The Category " + category.CategoryCode + " has been successfully created";
                    Message = Resources.Success + "-" + statusOperation;
                    status = true;
                    return new JsonResult { Data = new { status = status, Message = Message } };
                }
                else
                {
                    String statusOperation1 = @"Une catégorie ayant le code " + category.CategoryCode + " et / ou le label " + category.CategoryLabel + " existe déjà!<br/>"
                                        + "veuillez changer de code et / ou le label";

                    statusOperation = Resources.er_alert_danger + statusOperation1;
                    Message = Resources.Category + "-" + statusOperation;
                    status = false;
                    return new JsonResult { Data = new { status = status, Message = Message } };
                }

            }
            catch (Exception e)
            {
                statusOperation = Resources.er_alert_danger + e.Message;
                Message = Resources.Category + "-" + statusOperation;
                status = false;
                return new JsonResult { Data = new { status = status, Message = Message } };
            }
        }

        
        /// <summary>
        /// cette méthode est appelée lorqu' après avoir clicquer sur l'icone modifier du tableau on modifie le formulaire et clique sur enregistrer
        /// </summary>
        /// <returns></returns>
        ////[HttpPost]
        public JsonResult UpdateCategory(Category category)
        {
            bool status = false;
            string Message = "";
            try
            {
                Category existingCategory = db.Categories.Find(category.CategoryID);
                List<Category> categories = db.Categories.ToList();
                categories.Remove(existingCategory);

                if (((categories.FirstOrDefault(c => c.CategoryCode == category.CategoryCode || c.CategoryLabel == category.CategoryLabel) == null)))
                {
                    categoryRepository.Update(category, category.CategoryID);
                    statusOperation = "The Category " + category.CategoryCode + " has been successfully updated";
                    status = true;
                    Message = Resources.Success+"-"+ statusOperation;
                    return new JsonResult { Data = new { status = status, Message = Message } };
                }
                else
                {
                    statusOperation = @"Une catégorie ayant le code " + category.CategoryCode + " et / ou le label " + category.CategoryLabel + " existe déjà!<br/>"
                                        + "veuillez changer de code et / ou le label";
                    Message = Resources.Category + "-" + statusOperation;
                    status = false;
                    return new JsonResult { Data = new { status = status, Message = Message } };
                }

                

            }
            catch (Exception e)
            {
                status = false;
                Message =  @"Une erreur s'est produite lors de la mise à jour, veuillez contactez l'administrateur et/ou essayez à nouveau
                                    <br/>Code : <code>" + e.Message + "</code>";
                return new JsonResult { Data = new { status = status, Message = Message } };
            }
        }

        //[HttpPost]
        public JsonResult DeleteCategory(int CategoryID)
        {
            bool status = false;
            string Message = "";
            try
            {
                Category deletedCategoy = db.Categories.Find(CategoryID);
                List<Product> categoryProducts = db.Products.Where(p => p.CategoryID == deletedCategoy.CategoryID).ToList();

                if (deletedCategoy.CategoryCode == CodeValue.Supply.lensCategoryCode)
                {
                    status = false;
                    Message = "UnDiletable Category - You can Not Delete this Category";
                    return new JsonResult { Data = new { status = status, Message = Message } };
                }

                if ((categoryProducts != null) || (categoryProducts.Count != 0))
                {
                    db.Categories.Remove(deletedCategoy);
                    db.SaveChanges();
                    Message = "Category - The Category " + deletedCategoy.CategoryCode + " has been successfully deleted";
                    status =true;
                    return new JsonResult { Data = new { status = status, Message = Message } };
                }
                else
                {

                    Message = "Category - Désolé vous ne pouvez pas supprimer la catégorie " + deletedCategoy.CategoryCode + " parcequ'elle contient déjà des produits"
                                      + "pour supprimer cette catégorie, il faut d'abort supprimer ses produits";
                    status = false;
                    return new JsonResult { Data = new { status = status, Message = Message } };
                }
            }
            catch (Exception e)
            {
                Message = "Category - L'erreur " + e.Message + " s'est produite lors de la suppression! veuillez contactez l'administrateur et/ou essayez à nouveau";
                status = false;
                return new JsonResult { Data = new { status = status, Message = Message } };
            }
        }



        public JsonResult InitializePurchaseFields(int ID)
        {

            List<object> list = new List<object>();

            if (ID > 0)
            {
                Category category = db.Categories.Find(ID);
                
                list.Add(new
                {
                    CategoryID = category.CategoryID,
                    CategoryCode = category.CategoryCode,
                    CategoryLabel = category.CategoryLabel,
                    CategoryDescription= category.CategoryDescription,
                    isSerialNumberNull=category.isSerialNumberNull
                });
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        private ActionResult NotAuthorized()
        {
            return View();
        }

        public List<Category> CategoryModel()
        {
            List<Category> dataTmp = LoadComponent.GetAllGenericCategories().ToList();
            List<Category> list = new List<Category>();

            foreach (Category c in dataTmp)
            {
                list.Add(
                    new Category
                    {
                        CategoryID = c.CategoryID,
                        CategoryCode = c.CategoryCode,
                        CategoryLabel = c.CategoryLabel,
                        CategoryDescription = c.CategoryDescription,
                        isSerialNumberNull=c.isSerialNumberNull
                    }
                   );
            }

            return list;
        }

        //[HttpPost]
        public JsonResult GetAllCategories()
        {
            var model = new
            {
                data = from c in CategoryModel()
                select new
                {
                    CategoryID = c.CategoryID,
                    CategoryCode = c.CategoryCode,
                    CategoryLabel = c.CategoryLabel,
                    CategoryDescription = c.CategoryDescription,
                    isSerialNumberNull = c.isSerialNumberNull
                }
            };
            return Json(model, JsonRequestBehavior.AllowGet);
        }
    }
}