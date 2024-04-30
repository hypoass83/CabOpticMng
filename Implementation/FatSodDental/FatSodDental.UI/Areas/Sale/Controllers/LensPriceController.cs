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

    public class LensPriceController : BaseController
    {
        //Current Controller and current page
        private const string VIEW_NAME = "LensPrice";
        private const string CONTROLLER_NAME = "Sale/" + VIEW_NAME;

        private ILensNumberRangePrice _priceRepository;
        
        public LensPriceController(ILensNumberRangePrice lnrpRepo)
        {
            this._priceRepository = lnrpRepo;
        }
        //[OutputCache(Duration = 3600)] 
        public ActionResult LensPrice()
        {

            //Session["Curent_Page"] = VIEW_NAME;
            //Session["Curent_Controller"] = CONTROLLER_NAME;

            //ExtPartialViewResult rPVResult = new ExtPartialViewResult
            //{
            //    ClearContainer = true,
            //    //RenderMode = RenderMode.AddTo,
            //    ContainerId = "Body",
            //    //WrapByScriptTag = false,
            //    Model = PriceModel()
            //};

            //We test if this user has an autorization to get this sub menu
            //if (!LoadAction.isAutoriseToGetSubMenu(SessionProfileID, CodeValue.Sale.NewSale.RangeNumber, db))
            //{
            //    RedirectToAction("NotAuthorized", "User");
            //}

            return View(PriceModel());
        }

        [HttpPost]
        public ActionResult AddManager()
        {
            

            LensNumberRangePrice price = new LensNumberRangePrice();
            TryUpdateModel(price);

            LensNumberRange sphericalRange = (price.SphericalValueRangeID.HasValue && price.SphericalValueRangeID.Value > 0) ? db.LensNumberRanges.Find(price.SphericalValueRangeID.Value) : null;
            LensNumberRange cylindricalRange = (price.CylindricalValueRangeID.HasValue && price.CylindricalValueRangeID.Value > 0) ? db.LensNumberRanges.Find(price.CylindricalValueRangeID.Value) : null;
            LensNumberRange additionRange = (price.AdditionValueRangeID.HasValue && price.AdditionValueRangeID.Value > 0) ? db.LensNumberRanges.Find(price.AdditionValueRangeID.Value) : null;


            bool a = sphericalRange != null && sphericalRange.LensNumberRangeID > 0 && sphericalRange.IsValid();
            bool b = cylindricalRange != null && cylindricalRange.LensNumberRangeID > 0 && cylindricalRange.IsValid();
            bool c = additionRange != null && additionRange.LensNumberRangeID > 0 && additionRange.IsValid();

            bool d = (a && b && c) || (a && b) || (a && c) || (a) || (b && c) || (b);

            if (!d)
            {
                statusOperation = Resources.er_alert_danger + " " + " One or many Ranges are Invalid";
                X.Msg.Alert(Resources.a_NumberRange, statusOperation).Show();
                return this.Direct();
            }

            //en cas de mise à jour
            if (price.LensNumberRangePriceID > 0)
            {
                return this.UpdatePrice(price);
            }
            else
            {
                return this.AddPrice(price);
            }
        }

        public ActionResult OneByOne()
        {
            this.Reset();
            //ExtPartialViewResult rPVResult = new ExtPartialViewResult
            //{
            //    ClearContainer = true,
            //    //RenderMode = RenderMode.AddTo,
            //    ContainerId = "Body",
            //    //WrapByScriptTag = false,
            //    Model = PriceModel()
            //};
            //return rPVResult;
            return View(PriceModel());
        }

        public ActionResult ByRange()
        {
            this.Reset();
            //ExtPartialViewResult rPVResult = new ExtPartialViewResult
            //{
            //    ClearContainer = true,
            //    //RenderMode = RenderMode.AddTo,
            //    ContainerId = "Body",
            //    //WrapByScriptTag = false,
            //    Model = PriceModel()
            //};
            //return rPVResult;
            return View(PriceModel());

        }

        /// <summary>
        /// cette méthode est appelée lorqu' après avoir clicquer sur l'icone modifier du tableau on modifie le formulaire et clique sur enregistrer
        /// </summary>
        /// <returns></returns>
        public ActionResult UpdatePrice(LensNumberRangePrice price)
        {
            try
            {
                price.LensCategoryIds.Remove(0);
                price.LensCategoryID = price.LensCategoryIds.FirstOrDefault();

                _priceRepository.Update(price, price.LensNumberRangePriceID);
                statusOperation = "The Lens Number Range " + price.ToString() + " has been successfully updated";
                this.AlertSucces(Resources.Success, statusOperation);
                return this.Reset();
                
            }
            catch (Exception e)
            {
                TempData["alertType"] = "alert alert-danger";
                TempData["status"] = @"Une erreur s'est produite lors de la mise à jour, veuillez contactez l'administrateur et/ou essayez à nouveau
                                    <br/>Code : <code>" + e.Message + "</code>";
                return this.Direct(); ;
            }
        }

        public ActionResult AddPrice(LensNumberRangePrice price)
        {
            try
            {
                
                    _priceRepository.CreatePrice(price);
                    statusOperation = Resources.a_NumberRange + " " + price.ToString() + " has been successfully created";
                    this.AlertSucces(Resources.Success, statusOperation);
                    return this.Reset();    
                

            }
            catch (Exception e)
            {
                statusOperation = Resources.er_alert_danger + e.Message;
                X.Msg.Alert(Resources.a_NumberRange, statusOperation).Show();

                return this.Direct();
            }
        }

        [HttpPost]
        public ActionResult DeleteLensCoating(int LensNumberRangePriceID)
        {
            try
            {
                _priceRepository.Delete(LensNumberRangePriceID);

                statusOperation = "The LensCoating " + /*deletedRange.LensCoatingCode +*/ " has been successfully deleted";
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

        public void InitializePurchaseFields(int LensNumberRangePriceID)
        {

            this.GetCmp<FormPanel>("PriceForm").Reset(true);
            this.GetCmp<Store>("PriceListStore").Reload();

            if (LensNumberRangePriceID > 0)
            {
                LensNumberRangePrice price = db.LensNumberRangePrices.Find(LensNumberRangePriceID);

                this.GetCmp<TextField>("LensNumberRangePriceID").Value = price.LensNumberRangePriceID;
                this.GetCmp<MultiCombo>("LensCategoryIds").SelectItem("" + price.LensCategoryID);
                //this.GetCmp<ComboBox>("LensCategoryID").Value = price.LensCategoryID;
                this.GetCmp<ComboBox>("SphericalValueRangeID").Value = price.SphericalValueRangeID;
                this.GetCmp<ComboBox>("CylindricalValueRangeID").Value = price.CylindricalValueRangeID;
                this.GetCmp<ComboBox>("AdditionValueRangeID").Value = price.AdditionValueRangeID;
                this.GetCmp<NumberField>("Price").Value = price.Price;
            }
        }

        /// <summary>
        /// cette méthode est appelée quand on clicque sur l'icone modifier du tableau
        /// </summary>
        /// <param name="localizationID"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult UpdateLensCoating(int LensNumberRangePriceID)
        {
            this.InitializePurchaseFields(LensNumberRangePriceID);

            return this.Direct();

        }

        [HttpPost]
        public ActionResult Reset()
        {
            this.GetCmp<FormPanel>("PriceForm").Reset(true);
            this.GetCmp<Store>("PriceListStore").Reload();
            return this.Direct();
        }

        [HttpPost]
        public StoreResult GetAllPrices()
        {

            return this.Store(PriceModel());
        }

        public List<object> PriceModel()
        {
            List<object> list = new List<object>();

            foreach (LensNumberRangePrice c in db.LensNumberRangePrices.ToList())
            {
                list.Add(
                    new
                    {
                        LensNumberRangePriceID = c.LensNumberRangePriceID,
                        SphericalValueRange = (c.SphericalValueRangeID > 0 ) ? c.SphericalValueRange.ToString() : "",
                        CylindricalValueRange = (c.CylindricalValueRangeID > 0) ? c.CylindricalValueRange.ToString() : "",
                        AdditionValueRange = (c.AdditionValueRangeID > 0) ? c.AdditionValueRange.ToString() : "",
                        Price = c.Price,
                        LensCategory = c.LensCategory.CategoryCode
                    }
                   );
            }

            return list;
        }

    }
}