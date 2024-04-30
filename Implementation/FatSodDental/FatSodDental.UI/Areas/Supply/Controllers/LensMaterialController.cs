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

namespace FatSodDental.UI.Areas.Supply.Controllers
{
    [Authorize(Order = 1)]
    [TakeBusinessDay(Order = 2)]
    public class LensMaterialController : BaseController
    {
        private IRepositorySupply<LensMaterial> LensMaterialRepository;
        private IRepositorySupply<Lens> LensRepository;
        private IRepositorySupply<LensCategory> LensCategoryRepository;

        private const string CONTROLLER_NAME = "Supply/" + CodeValue.Supply.LensMaterial_SM.CONTROLLER;
        private const string VIEW_NAME = CodeValue.Supply.LensMaterial_SM.PATH;

        

        public LensMaterialController(IRepositorySupply<LensMaterial> LensMaterialRepository,
            IRepositorySupply<Lens> LensRepository, IRepositorySupply<LensCategory> LensCategoryRepository)
        {
            this.LensMaterialRepository = LensMaterialRepository;
            this.LensRepository = LensRepository;
            this.LensCategoryRepository = LensCategoryRepository;
            
        }

        /// <summary>
        /// the view of this action method is stronglyTyped with FatSod.Supply.Entities.LensMaterial
        /// </summary>
        /// <returns>ActionResult</returns>
        /// 
        //[OutputCache(Duration = 3600)] 
        public ActionResult LensMaterial(LensMaterial LensMaterial)
        {
            //Session["Curent_Page"] = VIEW_NAME;
            //Session["Curent_Controller"] = CONTROLLER_NAME;

            //We verify if the current user has right to access view which this action calls
            //if (!LoadAction.isAutoriseToGetSubMenu(SessionProfileID, CodeValue.Supply.LensMaterial_SM.CODE, db))
            //{
            //    this.NotAuthorized();
            //}

            //this.GetCmp<Panel>("Body").LoadContent(new ComponentLoader
            //{
            //    Url = Url.Action("LensMaterial"),
            //    DisableCaching = false,
            //    Mode = LoadMode.Frame
            //});
            //Ext.Net.MVC.PartialViewResult rPVResult = new Ext.Net.MVC.PartialViewResult
            //{
            //    ClearContainer = true,
            //    //RenderMode = RenderMode.AddTo,
            //    ContainerId = "Body",
            //    //WrapByScriptTag = false,
            //    Model = ModellensMaterial()
            //};

            //return rPVResult;
            return View(ModellensMaterial());
        }


        [HttpPost]
        public ActionResult AddManager()
        {
            LensMaterial LensMaterial = new LensMaterial();
            TryUpdateModel(LensMaterial);

            //en cas de mise à jour
            if (LensMaterial.LensMaterialID > 0)
            {
                return this.UpdateLensMaterial(LensMaterial);
            }
            else
            {
                return this.AddLensMaterial(LensMaterial);
            }
        }

        public ActionResult AddLensMaterial(LensMaterial LensMaterial)
        {
            try
            {

                if (((LensMaterialRepository.FindAll.FirstOrDefault(c => c.LensMaterialCode == LensMaterial.LensMaterialCode || c.LensMaterialLabel == LensMaterial.LensMaterialLabel) == null)))
                {
                    LensMaterialRepository.Create(LensMaterial);
                    statusOperation = "The LensMaterial " + LensMaterial.LensMaterialCode + " has been successfully created";
                    this.AlertSucces(Resources.Success, statusOperation);
                    return this.Reset();
                }
                else
                {
                    String statusOperation1 = @"Une catégorie ayant le code " + LensMaterial.LensMaterialCode + " et / ou le label " + LensMaterial.LensMaterialLabel + " existe déjà!<br/>"
                                        + "veuillez changer de code et / ou le label";

                    statusOperation = Resources.er_alert_danger + statusOperation1;
                    X.Msg.Alert(Resources.a_LensMaterial, statusOperation).Show();

                    return this.Direct();
                }

            }
            catch (Exception e)
            {
                statusOperation = Resources.er_alert_danger + e.Message;
                X.Msg.Alert(Resources.a_LensMaterial, statusOperation).Show();

                return this.Direct();
            }
        }

        /// <summary>
        /// cette méthode est appelée quand on clicque sur l'icone modifier du tableau
        /// </summary>
        /// <param name="localizationID"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult UpdateLensMaterial(int LensMaterialID)
        {
            this.InitializePurchaseFields(LensMaterialID);

            return this.Direct();

        }

        /// <summary>
        /// cette méthode est appelée lorqu' après avoir clicquer sur l'icone modifier du tableau on modifie le formulaire et clique sur enregistrer
        /// </summary>
        /// <returns></returns>
        ////[HttpPost]
        public ActionResult UpdateLensMaterial(LensMaterial LensMaterial)
        {
            try
            {
                LensMaterial existingLensMaterial = LensMaterialRepository.Find(LensMaterial.LensMaterialID);
                List<LensMaterial> categories = LensMaterialRepository.FindAll.ToList();
                categories.Remove(existingLensMaterial);

                if (((categories.FirstOrDefault(c => c.LensMaterialCode == LensMaterial.LensMaterialCode || c.LensMaterialLabel == LensMaterial.LensMaterialLabel) == null)))
                {
                    LensMaterialRepository.Update(LensMaterial, LensMaterial.LensMaterialID);
                    statusOperation = "The LensMaterial " + LensMaterial.LensMaterialCode + " has been successfully updated";
                    this.AlertSucces(Resources.Success, statusOperation);
                    return this.Reset();
                }
                else
                {
                    statusOperation = @"Une matière ayant le code " + LensMaterial.LensMaterialCode + " et / ou le label " + LensMaterial.LensMaterialLabel + " existe déjà!<br/>"
                                        + "veuillez changer de code et / ou le label";
                    X.Msg.Alert(Resources.a_LensMaterial, statusOperation).Show();
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
        public ActionResult DeleteLensMaterial(int LensMaterialID)
        {
            try
            {
                LensMaterial deletedLensMaterial = LensMaterialRepository.FindAll.FirstOrDefault(c => c.LensMaterialID == LensMaterialID);
                List<LensCategory> LensMaterialProducts = LensCategoryRepository.FindAll.Where(p => p.LensMaterial == deletedLensMaterial).ToList();
                if ((LensMaterialProducts != null) || (LensMaterialProducts.Count != 0))
                {
                    LensMaterialRepository.Delete(deletedLensMaterial);
                    TempData["status"] = "The LensMaterial " + deletedLensMaterial.LensMaterialCode + " has been successfully deleted";
                    TempData["alertType"] = "alert alert-success";
                    return this.Reset();
                }
                else
                {
                    TempData["alertType"] = "alert alert-danger";
                    TempData["status"] = @"Désolé vous ne pouvez pas supprimer la matière " + deletedLensMaterial.LensMaterialCode + " parcequ'elle contient déjà des produits"
                                      + "pour supprimer cette catégorie, il faut d'abort supprimer ses produits";
                    return this.Direct();
                }
            }
            catch (Exception e)
            {
                TempData["alertType"] = "alert alert-danger";
                TempData["status"] = @"L'erreur" + e.Message + "s'est produite lors de la suppression! veuillez contactez l'administrateur et/ou essayez à nouveau";
                return this.Direct();
            }
        }

        [HttpPost]
        public ActionResult Reset()
        {
            this.GetCmp<FormPanel>("LensMaterialForm").Reset(true);
            this.GetCmp<Store>("LensMaterialListStore").Reload();
            return this.Direct();
        }

        public void InitializePurchaseFields(int ID)
        {

            this.GetCmp<FormPanel>("LensMaterialForm").Reset(true);
            this.GetCmp<Store>("LensMaterialListStore").Reload();

            if (ID > 0)
            {
                LensMaterial LensMaterial = new LensMaterial();
                LensMaterial = LensMaterialRepository.Find(ID);

                this.GetCmp<TextField>("LensMaterialID").Value = LensMaterial.LensMaterialID;
                this.GetCmp<TextField>("LensMaterialCode").Value = LensMaterial.LensMaterialCode;
                this.GetCmp<TextField>("LensMaterialLabel").Value = LensMaterial.LensMaterialLabel;
                this.GetCmp<TextArea>("LensMaterialDescription").Value = LensMaterial.LensMaterialDescription;
            }
        }

        private ActionResult NotAuthorized()
        {
            return View();
        }

        public List<object> ModellensMaterial()
        {
            List<LensMaterial> dataTmp = LensMaterialRepository.FindAll.ToList();
            List<object> list = new List<object>();

            foreach (LensMaterial c in dataTmp)
            {
                list.Add(
                    new
                    {
                        LensMaterialID = c.LensMaterialID,
                        LensMaterialCode = c.LensMaterialCode,
                        LensMaterialLabel = c.LensMaterialLabel,
                        LensMaterialDescription = c.LensMaterialDescription,
                    }
                   );
            }

            return list;
        }

        [HttpPost]
        public StoreResult GetAllCategories()
        {
            /*List<LensMaterial> dataTmp = LensMaterialRepository.FindAll.ToList();
            List<object> list = new List<object>();

            foreach (LensMaterial c in dataTmp)
            {
                list.Add(
                    new
                    {
                        LensMaterialID = c.LensMaterialID,
                        LensMaterialCode = c.LensMaterialCode,
                        LensMaterialLabel = c.LensMaterialLabel,
                        LensMaterialDescription = c.LensMaterialDescription,
                    }
                   );
            }
            */
            return this.Store(ModellensMaterial());
        }
    }
}