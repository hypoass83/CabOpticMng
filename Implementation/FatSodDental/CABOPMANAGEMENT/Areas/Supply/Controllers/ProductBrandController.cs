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
    public class ProductBrandController : BaseController
    {
        private IRepositorySupply<ProductBrand> _ProductBrandRepository;

        public ProductBrandController(
                 IRepositorySupply<ProductBrand> productBrandRepository
                )
        {
            this._ProductBrandRepository = productBrandRepository;
            
        }

       

        /// <summary>
        /// the view of this action method is stronglyTyped with FatSod.Supply.Entities.ProductBrand
        /// </summary>
        /// <returns>ActionResult</returns>
        [OutputCache(Duration = 3600)]
        public ActionResult Index()
        {
            return View(ProductBrandModel());
        }


        //[HttpPost]
        public JsonResult AddManager()
        {
            ProductBrand ProductBrand = new ProductBrand();
            TryUpdateModel(ProductBrand);

            //en cas de mise à jour
            if (ProductBrand.ProductBrandID > 0)
            {
                return this.UpdateProductBrand(ProductBrand);
            }
            else
            {
                return this.AddProductBrand(ProductBrand);
            }
        }

        public JsonResult AddProductBrand(ProductBrand ProductBrand)
        {
            bool status = false;
            string Message = "";
            try
            {

                if (((db.ProductBrands.FirstOrDefault(c => c.ProductBrandCode == ProductBrand.ProductBrandCode || c.ProductBrandLabel == ProductBrand.ProductBrandLabel) == null)))
                {
                    _ProductBrandRepository.Create(ProductBrand);
                    statusOperation = "The Product Brand " + ProductBrand.ProductBrandCode + " has been successfully created";
                    Message = Resources.Success + "-" + statusOperation;
                    status = true;
                    return new JsonResult { Data = new { status = status, Message = Message } };
                }
                else
                {
                    String statusOperation1 = @"Une Marque ayant le code " + ProductBrand.ProductBrandCode + " et / ou le label " + ProductBrand.ProductBrandLabel + " existe déjà!<br/>"
                                        + "veuillez changer de code et / ou le label";

                    statusOperation = Resources.er_alert_danger + statusOperation1;
                    Message = Resources.ProductBrand + "-" + statusOperation;
                    status = false;
                    return new JsonResult { Data = new { status = status, Message = Message } };
                }

            }
            catch (Exception e)
            {
                statusOperation = Resources.er_alert_danger + e.Message;
                Message = Resources.ProductBrand + "-" + statusOperation;
                status = false;
                return new JsonResult { Data = new { status = status, Message = Message } };
            }
        }


        /// <summary>
        /// cette méthode est appelée lorqu' après avoir clicquer sur l'icone modifier du tableau on modifie le formulaire et clique sur enregistrer
        /// </summary>
        /// <returns></returns>
        ////[HttpPost]
        public JsonResult UpdateProductBrand(ProductBrand ProductBrand)
        {
            bool status = false;
            string Message = "";
            try
            {
                ProductBrand existingProductBrand = db.ProductBrands.Find(ProductBrand.ProductBrandID);
                List<ProductBrand> productBrands = db.ProductBrands.ToList();
                productBrands.Remove(existingProductBrand);

                if (((productBrands.FirstOrDefault(c => c.ProductBrandCode == ProductBrand.ProductBrandCode || c.ProductBrandLabel == ProductBrand.ProductBrandLabel) == null)))
                {
                    _ProductBrandRepository.Update(ProductBrand, ProductBrand.ProductBrandID);
                    statusOperation = "The Product Brand " + ProductBrand.ProductBrandCode + " has been successfully updated";
                    status = true;
                    Message = Resources.Success + "-" + statusOperation;
                    return new JsonResult { Data = new { status = status, Message = Message } };
                }
                else
                {
                    statusOperation = @"Une Marque ayant le code " + ProductBrand.ProductBrandCode + " et / ou le label " + ProductBrand.ProductBrandLabel + " existe déjà!<br/>"
                                        + "veuillez changer de code et / ou le label";
                    Message = Resources.ProductBrand + "-" + statusOperation;
                    status = false;
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

        //[HttpPost]
        public JsonResult DeleteProductBrand(int ProductBrandID)
        {
            bool status = false;
            string Message = "";
            try
            {
                ProductBrand deletedProductBrand = db.ProductBrands.Find(ProductBrandID);
                List<ProductLocalization> ProductBrandProducts = db.ProductLocalizations.Where(p => p.Marque.Trim() == deletedProductBrand.ProductBrandCode.Trim()).ToList();

                if ((ProductBrandProducts == null) || (ProductBrandProducts.Count == 0))
                {
                    db.ProductBrands.Remove(deletedProductBrand);
                    db.SaveChanges();
                    Message = "ProductBrand - The Product Brand " + deletedProductBrand.ProductBrandCode + " has been successfully deleted";
                    status = true;
                    return new JsonResult { Data = new { status = status, Message = Message } };
                }
                else
                {

                    Message = "ProductBrand - Désolé vous ne pouvez pas supprimer la marque " + deletedProductBrand.ProductBrandCode + " parcequ'elle contient déjà des produits"
                                      + "pour supprimer cette Marque, il faut d'abort supprimer ses produits";
                    status = false;
                    return new JsonResult { Data = new { status = status, Message = Message } };
                }
            }
            catch (Exception e)
            {
                Message = "ProductBrand - L'erreur " + e.Message + " s'est produite lors de la suppression! veuillez contactez l'administrateur et/ou essayez à nouveau";
                status = false;
                return new JsonResult { Data = new { status = status, Message = Message } };
            }
        }



        public JsonResult InitializePurchaseFields(int ID)
        {

            List<object> list = new List<object>();

            if (ID > 0)
            {
                ProductBrand ProductBrand = db.ProductBrands.Find(ID);

                list.Add(new
                {
                    ProductBrandID = ProductBrand.ProductBrandID,
                    ProductBrandCode = ProductBrand.ProductBrandCode,
                    ProductBrandLabel = ProductBrand.ProductBrandLabel,
                });
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        private ActionResult NotAuthorized()
        {
            return View();
        }

        public List<ProductBrand> ProductBrandModel()
        {
            List<ProductBrand> dataTmp = LoadComponent.GetAllGenericProductBrands().ToList();
            List<ProductBrand> list = new List<ProductBrand>();

            foreach (ProductBrand c in dataTmp)
            {
                list.Add(
                    new ProductBrand
                    {
                        ProductBrandID = c.ProductBrandID,
                        ProductBrandCode = c.ProductBrandCode,
                        ProductBrandLabel = c.ProductBrandLabel,
                    }
                   );
            }

            return list;
        }

        //[HttpPost]
        public JsonResult GetAlllistProductBrands()
        {
            var model = new
            {
                data = from c in ProductBrandModel()
                       select new
                       {
                           ProductBrandID = c.ProductBrandID,
                           ProductBrandCode = c.ProductBrandCode,
                           ProductBrandLabel = c.ProductBrandLabel,
                       }
            };
            return Json(model, JsonRequestBehavior.AllowGet);
        }
    }
}