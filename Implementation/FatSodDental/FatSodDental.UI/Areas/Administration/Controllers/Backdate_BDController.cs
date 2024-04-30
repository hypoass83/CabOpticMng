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
    public class Backdate_BDController : BaseController
    {
        //Current Controller and current page
        private const string CONTROLLER_NAME = "Administration/Backdate_BD";
        private const string VIEW_NAME = "Index";
        //person repository

        private IBusinessDay _busDayRepo;

        //Construcitor
        public Backdate_BDController(
            IBusinessDay busDayRepo
            )
        {
            this._busDayRepo = busDayRepo;
            
        }
        // GET: Administration/Backdate_BD

        [OutputCache(Duration = 3600)] 
        public ActionResult Index()
        {
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
        public ActionResult OpenBackDate(BusinessDay purchase)
        {

            if (purchase.BusinessDayID > 0)
            {
                _busDayRepo.OpenBackDate(db.Branches.Find(purchase.BranchID), purchase.BDDateOperation);

                if (_busDayRepo.GetClosedBusinessDay() == null || _busDayRepo.GetClosedBusinessDay().Count == 0)
                {
                    return RedirectToAction("Index", "../Main");
                }

                return this.Reset();

            }

            return this.Direct();
        }
        [HttpPost]
        public ActionResult UpdateBSDay(int PurchaseID)
        {

            this.InitializePurchaseFields(PurchaseID);
            return this.Direct();
        }

        [HttpPost]
        public ActionResult Reset()
        {
            this.GetCmp<FormPanel>("PurchaseForm").Reset(true);
            this.GetCmp<FormPanel>("GeneralForm").Reset(true);

            this.GetCmp<Store>("PurchaseListStore").Reload();
            return this.Direct();
        }
        public void InitializePurchaseFields(int ID)
        {

            if (ID > 0)
            {
                BusinessDay busDay = new BusinessDay();
                busDay = db.BusinessDays.Find(ID);

                this.GetCmp<TextField>("BusinessDayID").SetValue(busDay.BusinessDayID);

                this.GetCmp<ComboBox>("BranchID").SetValue(busDay.BranchID);
                this.GetCmp<ComboBox>("BranchID").ReadOnly = true;

                this.GetCmp<DateField>("PurchaseDate").MaxDate = busDay.BDDateOperation;

                if (_busDayRepo.GetOpenedBusinessDay() == null || _busDayRepo.GetOpenedBusinessDay().Count == 0)
                {
                    this.GetCmp<DateField>("PurchaseDate").SetValue(null);
                }


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

            return this.Store(list);
        }
        public ActionResult ChangeBusDay(int? BranchID)
        {
            this.GetCmp<DateField>("PurchaseDate").Reset();
            if (BranchID > 0)
            {
                BusinessDay businessDay = _busDayRepo.GetOpenedBusinessDay(db.Branches.Find(BranchID.Value));
                this.GetCmp<DateField>("PurchaseDate").SetValue(businessDay.BDDateOperation);
            }
            return this.Direct();
        }
    }
}