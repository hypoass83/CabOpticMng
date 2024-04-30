using FatSod.DataContext.Initializer;
using FatSod.Security.Abstracts;
using FatSod.Security.Entities;
using FatSod.Supply.Entities;
using FatSodDental.UI.Controllers;
using FatSodDental.UI.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using SaleE = FatSod.Supply.Entities.Sale;
using FatSod.Supply.Abstracts;
using FatSod.Ressources;
using FatSodDental.UI.Filters;
using FastSod.Utilities.Util;
using System.Collections;
using FatSod.DataContext.Concrete;
using System.Web.UI;
using ExtPartialViewResult = Ext.Net.MVC.PartialViewResult;

namespace FatSodDental.UI.Areas.Sale.Controllers
{
    [Authorize(Order = 1)]
    [TakeBusinessDay(Order = 2)]

    public class RangeNumberController : BaseController
    {
        //Current Controller and current page
        private const string VIEW_NAME = "RangeNumber";
        private const string CONTROLLER_NAME = "Sale/" + VIEW_NAME;

        private IRepositorySupply<LensNumberRange> _lensNumberRangeRepository;
        

        public RangeNumberController(IRepositorySupply<LensNumberRange> lnRepo

            )
        {
            this._lensNumberRangeRepository = lnRepo;
            
        }
        //
        [OutputCache(Duration = 3600)] 
        public ActionResult RangeNumber()
        {

            //ExtPartialViewResult rPVResult = new ExtPartialViewResult
            //{
            //    ClearContainer = true,
            //    //RenderMode = RenderMode.AddTo,
            //    ContainerId = "Body",
            //    //WrapByScriptTag = false,
            //    Model = RangeModel()
            //};

            //We test if this user has an autorization to get this sub menu
            //if (!LoadAction.isAutoriseToGetSubMenu(SessionProfileID, CodeValue.Sale.NewSale.RangeNumber, db))
            //{
            //    RedirectToAction("NotAuthorized", "User");
            //}

            return View(RangeModel());
        }

        [HttpPost]
        public ActionResult AddManager(LensNumberRange range)
        {

            if (!range.IsValid())
            {
                statusOperation = Resources.er_alert_danger + " " + " One or many Ranges are Invalid! The Maximum should at least be equal to Maximum ";
                X.Msg.Alert(Resources.a_NumberRange, statusOperation).Show();
                return this.Direct();
            }

            //en cas de mise à jour
            if (range.LensNumberRangeID > 0)
            {
                return this.UpdateRange(range);
            }
            else
            {
                return this.AddRange(range);
            }
        }

        /// <summary>
        /// cette méthode est appelée lorqu' après avoir clicquer sur l'icone modifier du tableau on modifie le formulaire et clique sur enregistrer
        /// </summary>
        /// <returns></returns>
        public ActionResult UpdateRange(LensNumberRange range)
        {
            try
            {
                if (range.IsValid())
                {
                    _lensNumberRangeRepository.Update(range, range.LensNumberRangeID);
                    statusOperation = "The Lens Number Range " + range.ToString() + " has been successfully updated";
                    this.AlertSucces(Resources.Success, statusOperation);
                    return this.Reset();
                }
                throw new Exception("Invalid Range");
            }
            catch (Exception e)
            {
                TempData["alertType"] = "alert alert-danger";
                TempData["status"] = @"Une erreur s'est produite lors de la mise à jour, veuillez contactez l'administrateur et/ou essayez à nouveau
                                    <br/>Code : <code>" + e.Message + "</code>";
                return this.Direct(); ;
            }
        }

        public ActionResult AddRange(LensNumberRange range)
        {
            try
            {
                if (range.IsValid())
                {

                    range = _lensNumberRangeRepository.Create(range);
                    statusOperation = Resources.a_NumberRange + " " + range.ToString() + " has been successfully created";
                    this.AlertSucces(Resources.Success, statusOperation);
                    return this.Reset();    
                }

                throw new Exception("Invalid Range");

            }
            catch (Exception e)
            {
                statusOperation = Resources.er_alert_danger + e.Message;
                X.Msg.Alert(Resources.a_NumberRange, statusOperation).Show();

                return this.Direct();
            }
        }

        [HttpPost]
        public ActionResult DeleteLensCoating(int LensNumberRangeID)
        {
            try
            {
                _lensNumberRangeRepository.Delete(LensNumberRangeID);

                statusOperation = "The LensCoating " + /*deletedRange.Minimum +*/ " has been successfully deleted";
                this.AlertSucces(Resources.Success, statusOperation);
                return this.Reset();

            }
            catch (Exception e)
            {
                string status = @"L'erreur" + e.Message + "s'est produite lors de la suppression! veuillez contactez l'administrateur et/ou essayez à nouveau";
                statusOperation = Resources.er_alert_danger + status + e.Message;
                X.Msg.Alert(Resources.a_NumberRange, statusOperation).Show();
                return this.Direct();
            }
        }

        [HttpGet]
        public ActionResult UpdateLensNumberRange(int LensNumberRangeID)
        {
            this.InitializePurchaseFields(LensNumberRangeID);

            return this.Direct();

        }

        public void InitializePurchaseFields(int LensNumberRangeID)
        {

            this.GetCmp<FormPanel>("RangeNumberForm").Reset(true);
            this.GetCmp<Store>("RangeNumberListStore").Reload();

            if (LensNumberRangeID > 0)
            {
                LensNumberRange range = _lensNumberRangeRepository.Find(LensNumberRangeID);

                this.GetCmp<TextField>("LensNumberRangeID").Value = range.LensNumberRangeID;
                this.GetCmp<TextField>("Minimum").Value = range.Minimum;
                this.GetCmp<TextField>("Maximum").Value = range.Maximum;
            }
        }

        [HttpPost]
        public ActionResult Reset()
        {
            this.GetCmp<FormPanel>("RangeNumberForm").Reset(true);
            this.GetCmp<Store>("RangeNumberListStore").Reload();
            return this.Direct();
        }

        [HttpPost]
        public StoreResult GetAllRanges()
        {

            return this.Store(RangeModel());
        }

        public List<object> RangeModel()
        {
            List<LensNumberRange> dataTmp = _lensNumberRangeRepository.FindAll.ToList();
            List<object> list = new List<object>();

            foreach (LensNumberRange c in dataTmp)
            {
                list.Add(
                    new
                    {
                        LensNumberRangeID = c.LensNumberRangeID,
                        Minimum = c.Minimum,
                        Maximum = c.Maximum,
                        FullName = c.ToString(),
                    }
                   );
            }

            return list;
        }

    }
}