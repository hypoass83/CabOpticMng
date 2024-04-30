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
    public class LensColourController : BaseController
    {
        private IRepositorySupply<LensColour> LensColourRepository;

        private const string CONTROLLER_NAME = "Supply/" + CodeValue.Supply.LensColour_SM.CONTROLLER;
        private const string VIEW_NAME = CodeValue.Supply.LensColour_SM.PATH;
        

        public LensColourController(IRepositorySupply<LensColour> LensColourRepository)
        {
            this.LensColourRepository = LensColourRepository;
            
        }

        /// <summary>
        /// the view of this action method is stronglyTyped with FatSod.Supply.Entities.LensColour
        /// </summary>
        /// <returns>ActionResult</returns>
        /// 
        //[OutputCache(Duration = 3600)] 
        public ActionResult LensColour(LensColour LensColour)
        {
            //Session["Curent_Page"] = VIEW_NAME;
            //Session["Curent_Controller"] = CONTROLLER_NAME;

            //We verify if the current user has right to access view which this action calls
            //if (!LoadAction.isAutoriseToGetSubMenu(SessionProfileID, CodeValue.Supply.LensColour_SM.CODE, db))
            //{
            //    this.NotAuthorized();
            //}

            //this.GetCmp<Panel>("Body").LoadContent(new ComponentLoader
            //{
            //    Url = Url.Action("LensColour"),
            //    DisableCaching = false,
            //    Mode = LoadMode.Frame
            //});
            //Ext.Net.MVC.PartialViewResult rPVResult = new Ext.Net.MVC.PartialViewResult
            //{
            //    ClearContainer = true,
            //    //RenderMode = RenderMode.AddTo,
            //    ContainerId = "Body",
            //    //WrapByScriptTag = false,
            //    Model = ModellensColour()
            //};

            //return rPVResult;
            return View(ModellensColour());
        }


        [HttpPost]
        public ActionResult AddManager()
        {
            LensColour LensColour = new LensColour();
            TryUpdateModel(LensColour);

            //en cas de mise à jour
            if (LensColour.LensColourID > 0)
            {
                return this.UpdateLensColour(LensColour);
            }
            else
            {
                return this.AddLensColour(LensColour);
            }
        }

        public ActionResult AddLensColour(LensColour LensColour)
        {
            try
            {

                if (((db.LensColours.FirstOrDefault(c => c.LensColourCode == LensColour.LensColourCode || c.LensColourLabel == LensColour.LensColourLabel) == null)))
                {
                    LensColourRepository.Create(LensColour);
                    statusOperation = "The LensColour " + LensColour.LensColourCode + " has been successfully created";
                    this.AlertSucces(Resources.Success, statusOperation);
                    return this.Reset();
                }
                else
                {
                    String statusOperation1 = @"Une catégorie ayant le code " + LensColour.LensColourCode + " et / ou le label " + LensColour.LensColourLabel + " existe déjà!<br/>"
                                        + "veuillez changer de code et / ou le label";

                    statusOperation = Resources.er_alert_danger + statusOperation1;
                    X.Msg.Alert(Resources.c_LensColour, statusOperation).Show();

                    return this.Direct();
                }

            }
            catch (Exception e)
            {
                statusOperation = Resources.er_alert_danger + e.Message;
                X.Msg.Alert(Resources.c_LensColour, statusOperation).Show();

                return this.Direct();
            }
        }

        /// <summary>
        /// cette méthode est appelée quand on clicque sur l'icone modifier du tableau
        /// </summary>
        /// <param name="localizationID"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult UpdateLensColour(int LensColourID)
        {
            this.InitializePurchaseFields(LensColourID);

            return this.Direct();

        }

        /// <summary>
        /// cette méthode est appelée lorqu' après avoir clicquer sur l'icone modifier du tableau on modifie le formulaire et clique sur enregistrer
        /// </summary>
        /// <returns></returns>
        ////[HttpPost]
        public ActionResult UpdateLensColour(LensColour LensColour)
        {
            try
            {
                LensColour existingLensColour = db.LensColours.Find(LensColour.LensColourID);
                List<LensColour> categories = db.LensColours.ToList();
                categories.Remove(existingLensColour);

                if (((categories.FirstOrDefault(c => c.LensColourCode == LensColour.LensColourCode || c.LensColourLabel == LensColour.LensColourLabel) == null)))
                {
                    LensColourRepository.Update(LensColour, LensColour.LensColourID);
                    statusOperation = "The LensColour " + LensColour.LensColourCode + " has been successfully updated";
                    this.AlertSucces(Resources.Success, statusOperation);
                    return this.Reset();
                }
                else
                {
                    statusOperation = @"Une matière ayant le code " + LensColour.LensColourCode + " et / ou le label " + LensColour.LensColourLabel + " existe déjà!<br/>"
                                        + "veuillez changer de code et / ou le label";
                    X.Msg.Alert(Resources.c_LensColour, statusOperation).Show();
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
        public ActionResult DeleteLensColour(int LensColourID)
        {
            try
            {
                LensColour deletedLensColour = db.LensColours.FirstOrDefault(c => c.LensColourID == LensColourID);
                List<LensCategory> LensColourProducts = db.LensCategories.Where(p => p.LensColour == deletedLensColour).ToList();
                if ((LensColourProducts != null) || (LensColourProducts.Count != 0))
                {
                    db.LensColours.Remove(deletedLensColour);
                    db.SaveChanges();
                    //LensColourRepository.Delete(deletedLensColour);
                    TempData["status"] = "The LensColour " + deletedLensColour.LensColourCode + " has been successfully deleted";
                    TempData["alertType"] = "alert alert-success";
                    return this.Reset();
                }
                else
                {
                    TempData["alertType"] = "alert alert-danger";
                    TempData["status"] = @"Désolé vous ne pouvez pas supprimer la matière " + deletedLensColour.LensColourCode + " parcequ'elle contient déjà des produits"
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
            this.GetCmp<FormPanel>("LensColourForm").Reset(true);
            this.GetCmp<Store>("LensColourListStore").Reload();
            return this.Direct();
        }

        public void InitializePurchaseFields(int ID)
        {

            this.GetCmp<FormPanel>("LensColourForm").Reset(true);
            this.GetCmp<Store>("LensColourListStore").Reload();

            if (ID > 0)
            {
                LensColour LensColour = new LensColour();
                LensColour = db.LensColours.Find(ID);

                this.GetCmp<TextField>("LensColourID").Value = LensColour.LensColourID;
                this.GetCmp<TextField>("LensColourCode").Value = LensColour.LensColourCode;
                this.GetCmp<TextField>("LensColourLabel").Value = LensColour.LensColourLabel;
                this.GetCmp<TextArea>("LensColourDescription").Value = LensColour.LensColourDescription;
            }
        }

        private ActionResult NotAuthorized()
        {
            return View();
        }
        public List<object> ModellensColour()
        {
            List<object> list = new List<object>();

            foreach (LensColour c in db.LensColours.Where(cl => cl.LensColourCode != CodeValue.Supply.DefaultLensColour).ToList())
            {
                list.Add(
                    new
                    {
                        LensColourID = c.LensColourID,
                        LensColourCode = c.LensColourCode,
                        LensColourLabel = c.LensColourLabel,
                        LensColourDescription = c.LensColourDescription,
                    }
                   );
            }

            return list;
        }
        [HttpPost]
        public StoreResult GetAllCategories()
        {
            return this.Store(ModellensColour());
        }
    }
}