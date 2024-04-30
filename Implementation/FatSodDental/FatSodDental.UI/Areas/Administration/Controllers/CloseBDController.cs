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
using System.Web.Security;
using FatSodDental.UI.Filters;
using System.Web.UI;
using FatSod.DataContext.Concrete;
using System.Web.Routing;

namespace FatSodDental.UI.Areas.Administration.Controllers
{
    [Authorize(Order = 1)]
    [TakeBusinessDay(Order = 2)]
    public class CloseBDController : BaseController
    {
        //Current Controller and current page
        private const string CONTROLLER_NAME = "Administration/" + CodeValue.Supply.CloseBD_SM.CONTROLLER;
        private const string VIEW_NAME = CodeValue.Supply.CloseBD_SM.PATH;
        //person repository


        private IBusinessDay _busDayRepo;

        //Construcitor
        public CloseBDController(
            IBusinessDay busDayRepo
            )
        {
            this._busDayRepo = busDayRepo;   
        }
        // GET: Sale/Sale Return all a sales historique add enable to create a new sale
        
        [OutputCache(Duration = 3600)] 
        public ActionResult CloseBD()
        {
            //Ext.Net.MVC.PartialViewResult rPVResult = new Ext.Net.MVC.PartialViewResult
            //{
            //    ClearContainer = true,
            //    //RenderMode = RenderMode.AddTo,
            //    ContainerId = "Body",
            //    WrapByScriptTag = false
            //};
            //Session["Curent_Page"] = VIEW_NAME;
            //Session["Curent_Controller"] = CONTROLLER_NAME;

            //We test if this user has an autorization to get this sub menu
            //if (!LoadAction.isAutoriseToGetMenu(SessionProfileID, CodeValue.Supply.SupplyMenu.CODE, db))
            //{
            //    RedirectToAction("NotAuthorized", "User");
            //}

            if (_busDayRepo.GetOpenedBusinessDay() == null || _busDayRepo.GetOpenedBusinessDay().Count == 0)
            {
                X.Msg.Alert(
                         "Good News",
                         "All Business Days Have Been Closed"
                       ).Show();
                return this.Direct();
            }

            return View();
        }

        [HttpPost]
        public ActionResult CloseBranches(int CloseBDID)
        {
            this.InitializeCloseBDFields(CloseBDID);
            return this.Direct();
        }

        [HttpPost]
        public ActionResult BtnCloseBusinessDay(int BusinessDayID)
        {
            
            if (BusinessDayID > 0)
            {
                BusinessDay oldBusDay = db.BusinessDays.Find(BusinessDayID);
                //on ne ferme une agence que si toutes ses caisses sont fermées
                TillDay currentTillDay = db.TillDays.Where(t => t.Till.BranchID == oldBusDay.BranchID &&
                                                                t.IsOpen == true &&
                                                                t.TillDayDate == oldBusDay.BDDateOperation).FirstOrDefault();
                
                //s'il existe au moins une caisse ouverte alors on ne doit pas fermer la journée
                if (currentTillDay != null)
                {
                    X.Msg.Alert("Cash Register "+currentTillDay.Till.Code+" Is Not Yet Closed", Resources.CashRegNotYetClose + currentTillDay.Till.Code).Show();
                    return this.Direct();
                }
                //fermeture du bs day
                _busDayRepo.CloseBusinessDay(oldBusDay.Branch,oldBusDay.BDDateOperation,SessionGlobalPersonID);
                
                List<BusinessDay> userBDList = _busDayRepo.GetOpenedBusinessDay(this.CurrentUser);
                

                return this.ResetCloseBD(userBDList);
            }

            return this.Direct();
        }
        
       

        [HttpPost]
        public ActionResult ResetCloseBD(List<BusinessDay> userBDList)
        {
            
            this.GetCmp<FormPanel>("CloseBDForm").Reset(true);
            this.GetCmp<FormPanel>("GeneralForm").Reset(true);
            this.GetCmp<Store>("CloseBDListStore").Reload();

            if (userBDList == null || userBDList.Count == 0)
            {
                //Session["UserBusDays"] = null;
                X.Msg.Alert("Close Business Day", Resources.msgAllBusinessDayClose).Show();
            }
            this.GetCmp<Button>("Cancel1").Disabled = true;
            return this.Direct();
        }

        public void InitializeCloseBDFields(int ID)
        {

            //this.GetCmp<FormPanel>("CloseBDForm").Reset(true);
            //this.GetCmp<Store>("CloseBDListStore").Reload();

            if (ID > 0)
            {
                BusinessDay businDay = new BusinessDay();
                businDay = db.BusinessDays.Find(ID);

                this.GetCmp<TextField>("BusinessDayID").SetValue(businDay.BusinessDayID);

                //this.GetCmp<ComboBox>("BranchID").ReadOnly = true;
                this.GetCmp<ComboBox>("BranchID").SetValue(businDay.BranchID);
                
                this.GetCmp<DateField>("CloseBDDate").SetValue(businDay.BDDateOperation);
                //this.GetCmp<DateField>("CloseBDDate").ReadOnly = true;
                //this.GetCmp<DateField>("CloseBDDate").MinDate = businDay.BDDateOperation;
            }
        }


        public ActionResult OpenedBusday()
        {
            List<object> list = new List<object>();

            foreach (BusinessDay busDay in _busDayRepo.GetOpenedBusinessDay())
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
        public StoreResult GetAllBusDayOpen(/*int? LocalizationID*/)
        {
            List<object> list = new List<object>();
            List<BusinessDay> busDays = _busDayRepo.GetOpenedBusinessDay();

            foreach (BusinessDay busDay in busDays)
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
            if (busDays == null || busDays.Count==0)
            {
                Session["UserBusDays"] = null;
            }
            return this.Store(list);
        }


    }
}