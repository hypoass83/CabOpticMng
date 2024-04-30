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
    public class PendingSaleController : BaseController
    {
        //Current Controller and current page
        private const string CONTROLLER_NAME = "Sale/PendingSale";
        private const string VIEW_NAME = "Index";
        //*********************

        private IBusinessDay _busDayRepo;

        private bool isLens = false;
        List<BusinessDay> bdDay;
        // GET: Sale/PendingSale
        public PendingSaleController(
            IBusinessDay busDayRepo
            )
        {
            this._busDayRepo = busDayRepo;
        }
        [OutputCache(Duration = 3600)]
        public ActionResult Index()
        {

            ViewBag.Disabled = true;

            bdDay = (List<BusinessDay>)Session["UserBusDays"];
            if (bdDay == null)
            {
                bdDay = _busDayRepo.GetOpenedBusinessDay(CurrentUser);
            }
            if (bdDay.Count() > 1)
            {
                ViewBag.Disabled = false;
            }
            DateTime busDays = bdDay.FirstOrDefault().BDDateOperation;

            ViewBag.BusnessDayDate = busDays;
            Session["BusnessDayDate"] = busDays;

           
            return View(ModelPendingSale);
        }

       
        //****************** This method load all CustomersOrders in data base
        [HttpPost]
        public StoreResult GetAllPendingSales()
        {
            return this.Store(ModelPendingSale);
        }
        private List<object> ModelPendingSale
        {
            get
            {
                List<object> model = new List<object>();
                /*LoadComponent.AllCommandsForStore
                .ToList().ForEach(c =>
                {
                    double CustomerOrderTotalPrice = Util.ExtraPrices(c.CustomerOrderLines.Sum(sl => sl.LineAmount),
                                                                      c.RateReduction, c.RateDiscount, c.Transport, c.VatRate).TotalTTC;
                    model.Add(
                        new
                        {
                            CustomerOrderID = c.CustomerOrderID,
                            CustomerOrderDate = c.CustomerOrderDate,
                            CustomerOrderTotalPrice = CustomerOrderTotalPrice,
                            CustomerOrderNumber = c.CustomerOrderNumber,
                            CustomerName = c.CustomerName
                        }
                    );
                });*/
                return model;
            }
        }

    }
}