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
using CrystalDecisions.CrystalReports.Engine;
using System.IO;
using FatSodDental.UI.Filters;
using FastSod.Utilities.Util;
using FatSod.Budget.Entities;
using System.Web.UI;

namespace FatSodDental.UI.Areas.Administration.Controllers
{
    [Authorize(Order = 1)]
    [TakeBusinessDay(Order = 2)]
    public class BudgetAllocatedUpdateController : BaseController
    {
        private IRepository<BudgetAllocatedUpdate> _budgetAllUpRepo;

        public BudgetAllocatedUpdateController(IRepository<BudgetAllocatedUpdate> budgetAllocatedUpdateRepo)
        {
            this._budgetAllUpRepo = budgetAllocatedUpdateRepo;
        }
        
        [OutputCache(Duration = 3600)] 
        public ActionResult Index()
        {
            //this.GetCmp<Panel>("Body").LoadContent(new ComponentLoader
            //{
            //    Url = Url.Action("Index"),
            //    DisableCaching = false,
            //    Mode = LoadMode.Frame
            //});
            //Ext.Net.MVC.PartialViewResult rPVResult = new Ext.Net.MVC.PartialViewResult
            //{
            //    ClearContainer = true,
            //    //RenderMode = RenderMode.AddTo,
            //    ContainerId = "Body",
            //    //WrapByScriptTag = false,
            //    Model = ModelBudgetAllocatedUpLine(true)
            //};

            return View(ModelBudgetAllocatedUpLine(true));
        }

        [HttpPost]
        public ActionResult AddBudgetAllUpLine(BudgetAllocatedUpdate budgetAlocatedUpdate, int BudgetAllocatedID)
        {
            budgetAlocatedUpdate.BudgetAllocatedID = BudgetAllocatedID;
            budgetAlocatedUpdate.BudgetAllocated = db.BudgetAllocateds.Find(budgetAlocatedUpdate.BudgetAllocatedID);
            List<BudgetAllocatedUpdate> listAll = (List<BudgetAllocatedUpdate>)Session["BudgetAllocatedUpdateList"];
            if (budgetAlocatedUpdate.BudgetAllocatedUpdateID > 0)
            {
                listAll.Remove(listAll.SingleOrDefault(b => b.BudgetAllocatedUpdateID == budgetAlocatedUpdate.BudgetAllocatedUpdateID));
            }
            if (listAll != null && listAll.Count() > 0)
            {
                var existingLine = listAll.SingleOrDefault(b => b.BudgetAllocatedID == budgetAlocatedUpdate.BudgetAllocatedID);
                if (existingLine != null)
                    listAll.Remove(existingLine);
                budgetAlocatedUpdate.BudgetAllocatedUpdateID = listAll.LastOrDefault() != null ? listAll.LastOrDefault().BudgetAllocatedUpdateID + 1 : 1;
                listAll.Add(budgetAlocatedUpdate);
            }
            else
            {
                listAll = new List<BudgetAllocatedUpdate>();
                budgetAlocatedUpdate.BudgetAllocatedUpdateID = 1;
                listAll.Add(budgetAlocatedUpdate);
            }
            Session["BudgetAllocatedUpdateList"] = listAll;
            this.GetCmp<Store>("BudgetAllocatedUpdateList").Reload();
            return this.Direct();
            return this.Direct();
        }
        [HttpPost]
        public ActionResult AddBudgetAllocatedUpdate(string Justification)
        {
            List<BudgetAllocatedUpdate> listAll = (List<BudgetAllocatedUpdate>)Session["BudgetAllocatedUpdateList"];
            listAll.ForEach(f =>
            {
                _budgetAllUpRepo.Create(
                    new BudgetAllocatedUpdate
                    {
                        BudgetAllocatedID = f.BudgetAllocatedID,
                        Justification = Justification,
                        SensImputation = f.SensImputation,
                        BudgetAllocatedUpdateID = f.BudgetAllocatedUpdateID,
                        Amount = f.Amount
                    }
                 );
            });
            statusOperation = Resources.AlertAddAction;
            Session["BudgetAllocatedUpdateList"] = null;
            this.GetCmp<Store>("BudgetAllocatedUpdateList").Reload();
            //this.GetCmp<Store>("BudgetAllocatedUpdateList").Reload();
            this.AlertSucces(Resources.Success, statusOperation);
            this.GetCmp<FormPanel>("GlobalForm").Reset();
            return this.Direct();
        }
        [HttpPost]
        public StoreResult GetBudgetAllocatedUpdateList()
        {
            return this.Store(ModelBudgetAllocatedUpLine(false));
        }
        [HttpPost]
        public ActionResult DeleteBudgetAllocatedUpdate(int ID)
        {
            List<BudgetAllocatedUpdate> listAll = (List<BudgetAllocatedUpdate>)Session["BudgetAllocatedUpdateList"];
            listAll.Remove(listAll.SingleOrDefault(b => b.BudgetAllocatedUpdateID == ID));
            this.GetCmp<Store>("BudgetAllocatedUpdateList").Reload();
            return this.Direct();
        }
        [HttpPost]
        public ActionResult InitializeFieldsBudgetAllocatedUpdate(int ID)
        {
            var item = _budgetAllUpRepo.Find(ID);
            this.GetCmp<ComboBox>("SensImputation").Value = item.SensImputation;
            this.GetCmp<NumberField>("Amount").Value = item.Amount;
            return this.Direct();
        }
        /// <summary>
        ///  Retrieve data of source datas. 
        /// </summary>
        /// <param name="isDataBase">Make true to retrieve datas to database or false to retreive of session</param>
        /// <returns></returns>
        private List<object> ModelBudgetAllocatedUpLine(bool isDataBase)
        {
            List<BudgetAllocatedUpdate> budgetList = new List<BudgetAllocatedUpdate>();
            if (isDataBase)
            {
                budgetList = _budgetAllUpRepo.FindAll.ToList();
            }
            else
            {
                budgetList = (List<BudgetAllocatedUpdate>)Session["BudgetAllocatedUpdateList"];
            }
            List<object> model = new List<object>();
            budgetList.ForEach(f =>
            {
                model.Add(
                        new
                        {
                            BudgetAllocatedID = f.BudgetAllocatedID,
                            BudgetAllocatedLabel = f.BudgetAllocated.BudgetLine.BudgetLineLabel,
                            Justification = f.Justification,
                            SensImputation = f.SensImputation,
                            BudgetAllocatedUpdateID = f.BudgetAllocatedUpdateID,
                            Amount = f.Amount
                        }
                    );
            });
            return model;
        }
    }
}