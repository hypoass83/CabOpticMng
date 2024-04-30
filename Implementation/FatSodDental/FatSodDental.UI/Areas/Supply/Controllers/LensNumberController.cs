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
    public class LensNumberController : BaseController
    {
        private IRepositorySupply<LensNumber> LensNumberRepository;
        

        private const string CONTROLLER_NAME = "Supply/" + CodeValue.Supply.LensNumber_SM.CONTROLLER;
        private const string VIEW_NAME = CodeValue.Supply.LensNumber_SM.PATH;

        public LensNumberController(IRepositorySupply<LensNumber> LensNumberRepository)
        {
            this.LensNumberRepository = LensNumberRepository;
            
        }

        /// <summary>
        /// the view of this action method is stronglyTyped with FatSod.Supply.Entities.LensNumber
        /// </summary>
        /// <returns>ActionResult</returns>
        /// 
        [OutputCache(Duration = 3600)] 
        public ActionResult LensNumber(LensNumber LensNumber)
        {

            //We verify if the current user has right to access view which this action calls
            //if (!LoadAction.isAutoriseToGetSubMenu(SessionProfileID, CodeValue.Supply.LensNumber_SM.CODE, db))
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
            //    Model = ModelNum()
            //};

            return View(ModelNum());
        }


        [HttpPost]
        public ActionResult AddManager()
        {
            LensNumber LensNumber = new LensNumber();
            TryUpdateModel(LensNumber);

            //en cas de mise à jour
            if (LensNumber.LensNumberID > 0)
            {
                return this.UpdateLensNumber(LensNumber);
            }
            else
            {
                return this.AddLensNumber(LensNumber);
            }
        }

        public ActionResult AddLensNumber(LensNumber LensNumber)
        {
            try
            {

                //if (((LensNumberRepository.FindAll.FirstOrDefault(c => c.LensNumberCylindricalValue == LensNumber.LensNumberCylindricalValue || c.LensNumberLabel == LensNumber.LensNumberLabel) == null)))
                //{
                    LensNumberRepository.Create(LensNumber);
                    statusOperation = "The LensNumber " + LensNumber.LensNumberFullCode + " has been successfully created";
                    this.AlertSucces(Resources.Success, statusOperation);
                    return this.Reset();
                //}
                //else
                //{
                //    String statusOperation1 = @"Une catégorie ayant le code " + LensNumber.LensNumberCode + " et / ou le label " + LensNumber.LensNumberLabel + " existe déjà!<br/>"
                //                        + "veuillez changer de code et / ou le label";

                //    statusOperation = Resources.er_alert_danger + statusOperation1;
                //    X.Msg.Alert(Resources.c_LensNumber, statusOperation).Show();

                //    return this.Direct();
                //}

            }
            catch (Exception e)
            {
                statusOperation = Resources.er_alert_danger + e.Message;
                X.Msg.Alert(Resources.d_LensNumber, statusOperation).Show();

                return this.Direct();
            }
        }

        /// <summary>
        /// cette méthode est appelée quand on clicque sur l'icone modifier du tableau
        /// </summary>
        /// <param name="localizationID"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult UpdateLensNumber(int LensNumberID)
        {
            this.InitializePurchaseFields(LensNumberID);

            return this.Direct();

        }

        /// <summary>
        /// cette méthode est appelée lorqu' après avoir clicquer sur l'icone modifier du tableau on modifie le formulaire et clique sur enregistrer
        /// </summary>
        /// <returns></returns>
        ////[HttpPost]
        public ActionResult UpdateLensNumber(LensNumber LensNumber)
        {
            try
            {
                LensNumber existingLensNumber = db.LensNumbers.Find(LensNumber.LensNumberID);
                List<LensNumber> categories = db.LensNumbers.ToList();
                categories.Remove(existingLensNumber);

                //if (((categories.FirstOrDefault(c => c.LensNumberCode == LensNumber.LensNumberCode || c.LensNumberLabel == LensNumber.LensNumberLabel) == null)))
                //{
                    LensNumberRepository.Update(LensNumber, LensNumber.LensNumberID);
                    statusOperation = "The LensNumber " + LensNumber.LensNumberFullCode + " has been successfully Updated";
                    this.AlertSucces(Resources.Success, statusOperation);
                    return this.Reset();
                //}
                //else
                //{
                //    statusOperation = @"Une matière ayant le code " + LensNumber.LensNumberCode + " et / ou le label " + LensNumber.LensNumberLabel + " existe déjà!<br/>"
                //                        + "veuillez changer de code et / ou le label";
                //    X.Msg.Alert(Resources.c_LensNumber, statusOperation).Show();
                //    return this.Direct();
                //}

                return this.Direct(); ;

            }
            catch (Exception e)
            {
                statusOperation = Resources.er_alert_danger + e.Message;
                X.Msg.Alert(Resources.d_LensNumber, statusOperation).Show();

                return this.Direct(); ;
            }
        }

        [HttpPost]
        public ActionResult DeleteLensNumber(int LensNumberID)
        {
            try
            {
                LensNumber deletedLensNumber = db.LensNumbers.FirstOrDefault(c => c.LensNumberID == LensNumberID);
                List<Lens> LensNumberProducts = db.Lenses.Where(p => p.LensNumber == deletedLensNumber).ToList();
                if ((LensNumberProducts != null) || (LensNumberProducts.Count != 0))
                {
                    db.LensNumbers.Remove(deletedLensNumber);
                    db.SaveChanges();
                    //LensNumberRepository.Delete(deletedLensNumber);
                    statusOperation = "The LensNumber " + deletedLensNumber.LensNumberFullCode + " has been successfully deleted";
                    X.Msg.Alert("LensNumber", statusOperation).Show();
                    return this.Reset();
                }
                else
                {
                    statusOperation = @"Désolé vous ne pouvez pas supprimer la matière " + deletedLensNumber.LensNumberFullCode + " parcequ'elle contient déjà des produits"
                                      + "pour supprimer cette catégorie, il faut d'abort supprimer ses produits";
                    X.Msg.Alert("LensNumber", statusOperation).Show();
                    return this.Direct();
                }
            }
            catch (Exception e)
            {
                statusOperation = @"L'erreur" + e.Message + "s'est produite lors de la suppression! veuillez contactez l'administrateur et/ou essayez à nouveau";
                X.Msg.Alert("LensNumber", statusOperation).Show();
                return this.Direct();
            }
        }

        [HttpPost]
        public ActionResult Reset()
        {
            this.GetCmp<FormPanel>("LensNumberForm").Reset(true);
            this.GetCmp<Store>("LensNumberListStore").Reload();
            return this.Direct();
        }

        public void InitializePurchaseFields(int ID)
        {

            this.GetCmp<FormPanel>("LensNumberForm").Reset(true);
            this.GetCmp<Store>("LensNumberListStore").Reload();

            if (ID > 0)
            {
                LensNumber LensNumber = new LensNumber();
                LensNumber = LensNumberRepository.Find(ID);

                this.GetCmp<TextField>("LensNumberID").Value = LensNumber.LensNumberID;
                this.GetCmp<TextField>("LensNumberCylindricalValue").Value = LensNumber.LensNumberCylindricalValue;
                this.GetCmp<TextField>("LensNumberSphericalValue").Value = LensNumber.LensNumberSphericalValue;
                this.GetCmp<TextField>("LensNumberAdditionValue").Value = LensNumber.LensNumberAdditionValue;

                this.GetCmp<TextArea>("LensNumberDescription").Value = LensNumber.LensNumberDescription;
            }
        }

        private ActionResult NotAuthorized()
        {
            return View();
        }
        private List<object> ModelNum()
        {
            List<object> list = new List<object>();

            foreach (LensNumber c in db.LensNumbers.ToList())
            {
                list.Add(
                    new
                    {
                        LensNumberID = c.LensNumberID,
                        LensNumberCylindricalValue = c.LensNumberCylindricalValue,
                        LensNumberSphericalValue = c.LensNumberSphericalValue,
                        LensNumberDescription = c.LensNumberDescription,
                        LensNumberFullCode = c.LensNumberFullCode,
                        LensNumberAdditionValue = c.LensNumberAdditionValue
                    }
                   );
            }
            return list;
        }
        [HttpPost]
        public StoreResult GetAllCategories()
        {
            return this.Store(ModelNum());
        }
    }
}