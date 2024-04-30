using FatSod.DataContext.Initializer;
using FatSod.Ressources;
using FatSod.Supply.Abstracts;
using FatSod.Supply.Entities;
using CABOPMANAGEMENT.Controllers;
using CABOPMANAGEMENT.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace CABOPMANAGEMENT.Areas.Supply.Controllers
{
    [Authorize(Order = 1)]
    [TakeBusinessDay(Order = 2)]
    public class LensCategoryController : BaseController
    {
        private ILensCategory lensCategoryRepository;

        private const string CONTROLLER_NAME = "Supply/" + CodeValue.Supply.LensCategorySM.CONTROLLER;
        private const string VIEW_NAME = CodeValue.Supply.LensCategorySM.PATH;

        public LensCategoryController(ILensCategory categoryRepository)
        {
            this.lensCategoryRepository = categoryRepository;
        }

        /// <summary>
        /// the view of this action method is stronglyTyped with FatSod.Supply.Entities.LensCategory
        /// </summary>
        /// <returns>ActionResult</returns>
        /// 
        [OutputCache(Duration = 3600)]
        public ActionResult LensCategory()
        {
            return View(CategoryModel());
        }


        public JsonResult AddManager(/*int IsSpecialCategory*/)
        {
            LensCategory category = new LensCategory();
            TryUpdateModel(category);
            category.IsSpecialCategory = true; // Convert.ToBoolean(IsSpecialCategory);
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

        public JsonResult AddCategory(LensCategory category)
        {
            bool status = false;
            string Message = "";
            try
            {

                if (((lensCategoryRepository.FindAll.FirstOrDefault(c => c.CategoryCode == category.CategoryCode || c.CategoryLabel == category.CategoryLabel) == null)))
                {
                    lensCategoryRepository.CreateLensCategory(category);
                    statusOperation = "The Lens LensCategory " + category.CategoryCode + " has been successfully created";
                    status = true;
                    Message = Resources.Success + " - " + statusOperation;
                    return new JsonResult { Data = new { status = status, Message = Message } };
                }
                else
                {
                    String statusOperation1 = @"Une catégorie ayant le code " + category.CategoryCode + " et / ou le label " + category.CategoryLabel + " existe déjà!<br/>"
                                        + "veuillez changer de code et / ou le label";

                    statusOperation = Resources.er_alert_danger + statusOperation1;
                    status = false;
                    Message = Resources.b_LensCategory + " - " + statusOperation;

                    return new JsonResult { Data = new { status = status, Message = Message } };
                }

            }
            catch (Exception e)
            {
                statusOperation = Resources.er_alert_danger + e.Message;
                status = false;
                Message = Resources.b_LensCategory + " - " + statusOperation;

                return new JsonResult { Data = new { status = status, Message = Message } };
            }
        }



        /// <summary>
        /// cette méthode est appelée lorqu' après avoir clicquer sur l'icone modifier du tableau on modifie le formulaire et clique sur enregistrer
        /// </summary>
        /// <returns></returns>
        ////[HttpPost]
        public JsonResult UpdateCategory(LensCategory category)
        {
            bool status = false;
            string Message = "";
            try
            {
                LensCategory existingCategory = db.LensCategories.Find(category.CategoryID);
                List<LensCategory> categories = db.LensCategories.ToList();
                categories.Remove(existingCategory);

                if (((categories.FirstOrDefault(c => c.CategoryCode == category.CategoryCode || c.CategoryLabel == category.CategoryLabel) == null)))
                {
                    lensCategoryRepository.UpdateLensCategory(category);
                    statusOperation = "The LensCategory " + category.CategoryCode + " has been successfully updated";
                    status = true;
                    Message = Resources.Success + " - " + statusOperation;
                    return new JsonResult { Data = new { status = status, Message = Message } };
                }
                else
                {
                    statusOperation = @"Une catégorie ayant le code " + category.CategoryCode + " et / ou le label " + category.CategoryLabel + " existe déjà!<br/>"
                                        + "veuillez changer de code et / ou le label";
                    status = false;
                    Message = Resources.b_LensCategory + " - " + statusOperation;
                    return new JsonResult { Data = new { status = status, Message = Message } };
                }

            }
            catch (Exception e)
            {
                statusOperation = @"Une erreur s'est produite lors de la mise à jour, veuillez contactez l'administrateur et/ou essayez à nouveau
                                    <br/>Code : <code>" + e.Message + "</code>";
                status = false;
                Message = Resources.b_LensCategory + " - " + statusOperation;
                return new JsonResult { Data = new { status = status, Message = Message } };
            }
        }


        public JsonResult DeleteCategory(int CategoryID)
        {
            bool status = false;
            string Message = "";
            try
            {
                LensCategory deletedCategoy = db.LensCategories.Find(CategoryID);
                List<Product> categoryProducts = db.Products.Where(p => p.CategoryID == deletedCategoy.CategoryID).ToList();

                if (deletedCategoy.CategoryCode == CodeValue.Supply.lensCategoryCode)
                {
                    status = false;
                    Message = "UnDeletable LensCategory - You can Not Delete this LensCategory";
                    return new JsonResult { Data = new { status = status, Message = Message } };
                }

                if ((categoryProducts != null) || (categoryProducts.Count != 0))
                {
                    db.LensCategories.Remove(deletedCategoy);
                    db.SaveChanges();
                    status = true;
                    Message = "LensCategory - The LensCategory " + deletedCategoy.CategoryCode + " has been successfully deleted";
                    return new JsonResult { Data = new { status = status, Message = Message } };
                }
                else
                {
                    status = false;
                    Message = "LensCategory - Désolé vous ne pouvez pas supprimer la catégorie " + deletedCategoy.CategoryCode + " parcequ'elle contient déjà des produits"
                                      + "pour supprimer cette catégorie, il faut d'abort supprimer ses produits";
                    return new JsonResult { Data = new { status = status, Message = Message } };
                }
            }
            catch (Exception e)
            {
                status = false;
                Message = "LensCategory - L'erreur " + e.Message + " s'est produite lors de la suppression! veuillez contactez l'administrateur et/ou essayez à nouveau";
                return new JsonResult { Data = new { status = status, Message = Message } };
            }
        }


        public JsonResult InitializePurchaseFields(int ID)
        {

            List<object> list = new List<object>();

            if (ID > 0)
            {
                LensCategory category = new LensCategory();
                category = db.LensCategories.Find(ID);

                list.Add(new
                {
                    CategoryID = category.CategoryID,
                    CategoryCode = category.CategoryCode,
                    CategoryLabel = category.CategoryLabel,
                    CategoryDescription = category.CategoryDescription,
                    SupplyingName = (category.SupplyingName != null && category.SupplyingName.Length > 0) ? category.SupplyingName : category.CategoryCode,
                    LensIndex = category.LensIndex,
                    LensDiameter = category.LensDiameter,
                    TypeLens =category.TypeLens //(category.IsSpecialCategory) ? true : false
                });
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        private ActionResult NotAuthorized()
        {
            return View();
        }

        public List<LensCategory> CategoryModel()
        {
            List<LensCategory> list = new List<LensCategory>();

            foreach (LensCategory c in db.LensCategories.ToList())
            {
                list.Add(
                    new LensCategory
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
                       }
            };
            return Json(model, JsonRequestBehavior.AllowGet);
        }
    }
}