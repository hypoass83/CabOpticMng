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
    public class LensCoatingController : BaseController
    {
        private IRepositorySupply<LensCoating> LensCoatingRepository;

        private const string CONTROLLER_NAME = "Supply/" + CodeValue.Supply.LensCoating_SM.CONTROLLER;
        private const string VIEW_NAME = CodeValue.Supply.LensCoating_SM.PATH;
        

        public LensCoatingController(IRepositorySupply<LensCoating> LensCoatingRepository)
        {
            this.LensCoatingRepository = LensCoatingRepository;
            
        }

        /// <summary>
        /// the view of this action method is stronglyTyped with FatSod.Supply.Entities.LensCoating
        /// </summary>
        /// <returns>ActionResult</returns>
        /// 
        //[OutputCache(Duration = 3600)] 
        public ActionResult LensCoating(LensCoating LensCoating)
        {
            //Session["Curent_Page"] = VIEW_NAME;
            //Session["Curent_Controller"] = CONTROLLER_NAME;

            //We verify if the current user has right to access view which this action calls
            //if (!LoadAction.isAutoriseToGetSubMenu(SessionProfileID, CodeValue.Supply.LensCoating_SM.CODE, db))
            //{
            //    this.NotAuthorized();
            //}

            //this.GetCmp<Panel>("Body").LoadContent(new ComponentLoader
            //{
            //    Url = Url.Action("LensCoating"),
            //    DisableCaching = false,
            //    Mode = LoadMode.Frame
            //});
            //Ext.Net.MVC.PartialViewResult rPVResult = new Ext.Net.MVC.PartialViewResult
            //{
            //    ClearContainer = true,
            //    //RenderMode = RenderMode.AddTo,
            //    ContainerId = "Body",
            //    //WrapByScriptTag = false,
            //    Model = ModellensCoaring()
            //};

            //return rPVResult;
            return View(ModellensCoaring());
        }


        [HttpPost]
        public ActionResult AddManager()
        {
            LensCoating LensCoating = new LensCoating();
            TryUpdateModel(LensCoating);

            //en cas de mise à jour
            if (LensCoating.LensCoatingID > 0)
            {
                return this.UpdateLensCoating(LensCoating);
            }
            else
            {
                return this.AddLensCoating(LensCoating);
            }
        }

        public ActionResult AddLensCoating(LensCoating LensCoating)
        {
            try
            {

                if (((db.LensCoatings.FirstOrDefault(c => c.LensCoatingCode == LensCoating.LensCoatingCode || c.LensCoatingLabel == LensCoating.LensCoatingLabel) == null)))
                {
                    LensCoatingRepository.Create(LensCoating);
                    statusOperation = "The LensCoating " + LensCoating.LensCoatingCode + " has been successfully created";
                    this.AlertSucces(Resources.Success, statusOperation);
                    return this.Reset();
                }
                else
                {
                    String statusOperation1 = @"Une catégorie ayant le code " + LensCoating.LensCoatingCode + " et / ou le label " + LensCoating.LensCoatingLabel + " existe déjà!<br/>"
                                        + "veuillez changer de code et / ou le label";

                    statusOperation = Resources.er_alert_danger + statusOperation1;
                    X.Msg.Alert(Resources.b_LensCoating, statusOperation).Show();

                    return this.Direct();
                }

            }
            catch (Exception e)
            {
                statusOperation = Resources.er_alert_danger + e.Message;
                X.Msg.Alert(Resources.b_LensCoating, statusOperation).Show();

                return this.Direct();
            }
        }

        /// <summary>
        /// cette méthode est appelée quand on clicque sur l'icone modifier du tableau
        /// </summary>
        /// <param name="localizationID"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult UpdateLensCoating(int LensCoatingID)
        {
            this.InitializePurchaseFields(LensCoatingID);

            return this.Direct();

        }

        /// <summary>
        /// cette méthode est appelée lorqu' après avoir clicquer sur l'icone modifier du tableau on modifie le formulaire et clique sur enregistrer
        /// </summary>
        /// <returns></returns>
        ////[HttpPost]
        public ActionResult UpdateLensCoating(LensCoating LensCoating)
        {
            try
            {
                LensCoating existingLensCoating = db.LensCoatings.Find(LensCoating.LensCoatingID);
                List<LensCoating> categories = db.LensCoatings.ToList();
                categories.Remove(existingLensCoating);

                if (((categories.FirstOrDefault(c => c.LensCoatingCode == LensCoating.LensCoatingCode || c.LensCoatingLabel == LensCoating.LensCoatingLabel) == null)))
                {
                    LensCoatingRepository.Update(LensCoating, LensCoating.LensCoatingID);
                    statusOperation = "The LensCoating " + LensCoating.LensCoatingCode + " has been successfully updated";
                    this.AlertSucces(Resources.Success, statusOperation);
                    return this.Reset();
                }
                else
                {
                    statusOperation = @"Une matière ayant le code " + LensCoating.LensCoatingCode + " et / ou le label " + LensCoating.LensCoatingLabel + " existe déjà!<br/>"
                                        + "veuillez changer de code et / ou le label";
                    X.Msg.Alert(Resources.b_LensCoating, statusOperation).Show();
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
        public ActionResult DeleteLensCoating(int LensCoatingID)
        {
            try
            {
                LensCoating deletedLensCoating = db.LensCoatings.FirstOrDefault(c => c.LensCoatingID == LensCoatingID);
                List<LensCategory> LensCoatingProducts = db.LensCategories.Where(p => p.LensCoating == deletedLensCoating).ToList();
                if ((LensCoatingProducts != null) || (LensCoatingProducts.Count != 0))
                {
                    db.LensCoatings.Remove(deletedLensCoating);
                    db.SaveChanges();
                    //LensCoatingRepository.Delete(deletedLensCoating);
                    TempData["status"] = "The LensCoating " + deletedLensCoating.LensCoatingCode + " has been successfully deleted";
                    TempData["alertType"] = "alert alert-success";
                    return this.Reset();
                }
                else
                {
                    TempData["alertType"] = "alert alert-danger";
                    TempData["status"] = @"Désolé vous ne pouvez pas supprimer la matière " + deletedLensCoating.LensCoatingCode + " parcequ'elle contient déjà des produits"
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
            this.GetCmp<FormPanel>("LensCoatingForm").Reset(true);
            this.GetCmp<Store>("LensCoatingListStore").Reload();
            return this.Direct();
        }

        public void InitializePurchaseFields(int ID)
        {

            this.GetCmp<FormPanel>("LensCoatingForm").Reset(true);
            this.GetCmp<Store>("LensCoatingListStore").Reload();

            if (ID > 0)
            {
                LensCoating LensCoating = new LensCoating();
                LensCoating = db.LensCoatings.Find(ID);

                this.GetCmp<TextField>("LensCoatingID").Value = LensCoating.LensCoatingID;
                this.GetCmp<TextField>("LensCoatingCode").Value = LensCoating.LensCoatingCode;
                this.GetCmp<TextField>("LensCoatingLabel").Value = LensCoating.LensCoatingLabel;
                this.GetCmp<TextArea>("LensCoatingDescription").Value = LensCoating.LensCoatingDescription;
            }
        }

        private ActionResult NotAuthorized()
        {
            return View();
        }
        public List<object> ModellensCoaring()
        {
            List<object> list = new List<object>();

            foreach (LensCoating c in db.LensCoatings.Where(cl => cl.LensCoatingCode != CodeValue.Supply.DefaultLensCoating).ToList())
            {
                list.Add(
                    new
                    {
                        LensCoatingID = c.LensCoatingID,
                        LensCoatingCode = c.LensCoatingCode,
                        LensCoatingLabel = c.LensCoatingLabel,
                        LensCoatingDescription = c.LensCoatingDescription,
                    }
                   );
            }

            return list;
        }
        [HttpPost]
        public StoreResult GetAllCategories()
        {
            return this.Store(ModellensCoaring());
        }
    }
}