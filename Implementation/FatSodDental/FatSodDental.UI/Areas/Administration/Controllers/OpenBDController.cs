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
using System.Web.UI;
using FatSod.DataContext.Concrete;

namespace FatSodDental.UI.Areas.Administration.Controllers
{
    [Authorize(Order = 1)]
    //[TakeBusinessDay(Order = 2)]
    public class OpenBDController : BaseController
    {
        //Current Controller and current page
        private const string CONTROLLER_NAME = "Administration/" + CodeValue.Supply.OpenBD_SM.CONTROLLER;
        private const string VIEW_NAME = CodeValue.Supply.OpenBD_SM.PATH;
        //person repository

        private IBusinessDay _busDayRepo;

        //Construcitor
        public OpenBDController(
            IBusinessDay busDayRepo
            )
        {
            this._busDayRepo = busDayRepo;
            
        }
        // GET: Sale/Sale Return all a sales historique add enable to create a new sale
        
        [OutputCache(Duration = 3600)] 
        public ActionResult OpenBD()
        {
            //Session["Curent_Page"] = VIEW_NAME;
            //Session["Curent_Controller"] = CONTROLLER_NAME;
            //Ext.Net.MVC.PartialViewResult rPVResult = new Ext.Net.MVC.PartialViewResult
            //{
            //    ClearContainer = true,
            //    //RenderMode = RenderMode.AddTo,
            //    ContainerId = "Body",
            //    WrapByScriptTag = false
            //};
            //We test if this user has an autorization to get this sub menu
            //if (!LoadAction.isAutoriseToGetMenu(SessionProfileID, CodeValue.Supply.SupplyMenu.CODE, db))
            //{
            //    RedirectToAction("NotAuthorized", "User");
            //}

            if (_busDayRepo.GetClosedBusinessDay() == null || _busDayRepo.GetClosedBusinessDay().Count == 0)
            {
                X.Msg.Alert(
                         "Good News",
                         "All Business Days are Opened"
                       ).Show();
                return this.Direct();
            }

            //au moins un business day de l'utilisateur est ouverte et il peut commencer à travailler
            if (_busDayRepo.GetOpenedBusinessDay(CurrentUser) != null && _busDayRepo.GetOpenedBusinessDay(CurrentUser).Count > 0)
            {
                this.GetCmp<Button>("BeginWork").Disabled = false;
            }

            return View();
        }

        [HttpPost]
        public ActionResult OpenBusinessDay(BusinessDay OpenBD)
        {
            

            if (OpenBD.BusinessDayID > 0)
            {
                _busDayRepo.OpenBusinessDay(db.Branches.Find(OpenBD.BranchID), OpenBD.BDDateOperation,SessionGlobalPersonID);

                if (_busDayRepo.GetClosedBusinessDay() == null || _busDayRepo.GetClosedBusinessDay().Count == 0)
                {
                    //return RedirectToAction("Index", "../Main");
                    this.Reset();
                }

                //au moins un business day de l'utilisateur est ouverte et il peut commencer à travailler
                if (_busDayRepo.GetOpenedBusinessDay(CurrentUser) != null && _busDayRepo.GetOpenedBusinessDay(CurrentUser).Count > 0)
                {
                    this.GetCmp<Button>("BeginWork").Disabled = false;
                }

                return this.Reset();

            }

            return this.Direct();
        }

        [HttpPost]
        public ActionResult UpdateBSDay(int OpenBDID)
        {
            this.InitializeOpenBDFields(OpenBDID);
            return this.Direct();
        }

        [HttpPost]
        public ActionResult Reset()
        {
            this.GetCmp<FormPanel>("OpenBDForm").Reset(true);
            this.GetCmp<FormPanel>("GeneralForm").Reset(true);

            this.GetCmp<Store>("OpenBDListStore").Reload();
            return this.Direct();
        }

        [HttpPost]
        public ActionResult BeginWork()
        {

            if (_busDayRepo.GetOpenedBusinessDay(CurrentUser) == null && _busDayRepo.GetOpenedBusinessDay(CurrentUser).Count > 0)
            {
                X.Msg.Alert(
                    "Bad New",
                    "None User Branch is Opened! You can begin work only if you have opened a branch."
                ).Show();
            }
            return RedirectToAction("Index", "../Home");
        }

        public void InitializeOpenBDFields(int ID)
        {

            if (ID > 0)
            {
                BusinessDay busDay = new BusinessDay();
                busDay = db.BusinessDays.Find(ID);

                this.GetCmp<TextField>("BusinessDayID").SetValue(busDay.BusinessDayID);

                this.GetCmp<ComboBox>("BranchID").SetValue(busDay.BranchID);
                this.GetCmp<ComboBox>("BranchID").ReadOnly = true;

                this.GetCmp<DateField>("OpenBDDate").MinDate = busDay.BDDateOperation;
                
                if (_busDayRepo.GetOpenedBusinessDay() == null || _busDayRepo.GetOpenedBusinessDay().Count == 0)
                {
                    this.GetCmp<DateField>("OpenBDDate").SetValue(null);
                }

                if (_busDayRepo.GetOpenedBusinessDay() != null && _busDayRepo.GetOpenedBusinessDay().Count > 0)
                {
                    this.GetCmp<DateField>("OpenBDDate").SetValue(_busDayRepo.GetOpenedBusinessDay().FirstOrDefault().BDDateOperation);
                    this.GetCmp<DateField>("OpenBDDate").ReadOnly = true;
                }

            }

            //au moins un business day de l'utilisateur est ouverte et il peut commencer à travailler
            if (_busDayRepo.GetOpenedBusinessDay(CurrentUser) != null && _busDayRepo.GetOpenedBusinessDay(CurrentUser).Count > 0)
            {
                this.GetCmp<Button>("BeginWork").Disabled = false;
            }

        }

        public ActionResult OpenedBusday()
        {
            List<object> list = new List<object>();

            foreach (BusinessDay busDay in _busDayRepo.GetClosedBusinessDay())
            {
                list.Add(
                    new
                    {
                        BranchID = busDay.BranchID,
                        BranchName = busDay.BranchName
                    }
                    );
            }

            return this.Store(list);

        }

        [HttpPost]
        public StoreResult GetAllCloseBusinessDay(/*int? LocalizationID*/)
        {

            List<object> list = new List<object>();

            foreach (BusinessDay busDay in _busDayRepo.GetClosedBusinessDay())
            {
                list.Add(
                    new
                    {
                        BranchID = busDay.BranchID,
                        BusinessDayID = busDay.BusinessDayID,
                        BDDateOperation = busDay.BDDateOperation,
                        ClosingDayStarted = busDay.ClosingDayStarted,
                        BDStatut = busDay.BDStatut,
                        BranchName = busDay.BranchName
                    }
                    );
            }

            return this.Store(list);



        }

        public ActionResult ChangeBusDay(int? BranchID)
        {
            this.GetCmp<DateField>("OpenBDDate").Reset();
            if (BranchID > 0)
            {
                BusinessDay businessDay = _busDayRepo.GetOpenedBusinessDay(db.Branches.Find(BranchID.Value));
                this.GetCmp<DateField>("OpenBDDate").SetValue(businessDay.BDDateOperation);
            }
            return this.Direct();
        }

    }
}