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
    public class BudgetLineController : BaseController
    {
        private IRepository<BudgetLine> _budgetLineRepo;
        //Current Controller and current page
        private const string CONTROLLER_NAME = "Administration/BudgetLine";

        // GET: Administration/BudgetLine
        public BudgetLineController( 
            IRepository<BudgetLine> budgetLineRepo)
        {
            this._budgetLineRepo = budgetLineRepo;
        }
        
        [OutputCache(Duration = 3600)] 
        public ActionResult Index()
        {
            
            //Ext.Net.MVC.PartialViewResult rPVResult = new Ext.Net.MVC.PartialViewResult
            //{
            //    ClearContainer = true,
            //    //RenderMode = RenderMode.AddTo,
            //    ContainerId = "Body",
            //    //WrapByScriptTag = false,
            //    Model =ModelBudgetLine
            //};
            return View(ModelBudgetLine);
        }

        [HttpPost]
        public ActionResult AddBudgetLine(BudgetLine budgetLine, int BudgetControl)
        {
            budgetLine.BudgetControl = Convert.ToBoolean(BudgetControl);
            if (budgetLine.BudgetLineID > 0)
            {
                statusOperation = Resources.AlertUpdateAction;
                _budgetLineRepo.Update(budgetLine, budgetLine.BudgetLineID);
            }
            else
            {
                statusOperation = Resources.AlertAddAction;
                _budgetLineRepo.Create(budgetLine);
            }
            this.AlertSucces(Resources.Success, statusOperation);
            this.GetCmp<FormPanel>("BudgetLineForm").Reset();
            this.GetCmp<Store>("BudgetLineListStore").Reload();
            return this.Direct();
        }
        [HttpPost]
        public StoreResult GetBudgetLineList()
        {
            return this.Store(ModelBudgetLine);
        }
        [HttpPost]
        public ActionResult DeleteBudgetLine(int ID)
        {
            try
            {
                db.BudgetLines.Remove(db.BudgetLines.Find(ID));
                db.SaveChanges();
                //_budgetLineRepo.Delete(db.BudgetLines.Find(ID));
            }
            catch (Exception e)
            {
                X.Msg.Alert("Can't Delete It", " " + e.Message + "-" + e.StackTrace).Show();
            }
            this.GetCmp<Store>("BudgetLineListStore").Reload();
            return this.Direct();
        }
        [HttpPost]
        public ActionResult InitializeFieldsBudgetLine(int ID)
        {
            BudgetLine budgetLine = db.BudgetLines.Find(ID);
            this.GetCmp<NumberField>("BudgetLineID").Value = budgetLine.BudgetLineID;
            
            this.GetCmp<TextField>("BudgetType").Value = budgetLine.BudgetType;
            this.GetCmp<TextField>("BudgetCode").Value = budgetLine.BudgetCode;
            this.GetCmp<TextField>("BudgetLineLabel").Value = budgetLine.BudgetLineLabel;
            this.GetCmp<ComboBox>("AccountID").Value = budgetLine.AccountID;

            if (budgetLine.BudgetControl) this.GetCmp<Radio>("isBudgetControl").Checked = true;
            else this.GetCmp<Radio>("isBudgetControlNO").Checked = true;


            return this.Direct();
        }
        private List<object> ModelBudgetLine
        {
            get
            {
                List<object> model = new List<object>();
                db.BudgetLines.ToList().ForEach(f =>
                {
                    model.Add(
                            new
                            {
                                BudgetLineID = f.BudgetLineID,
                                BudgetControl = f.BudgetControl,
                                BudgetType = f.BudgetType,
                                BudgetCode = f.BudgetCode,
                                BudgetLineLabel = f.BudgetLineLabel,
                                Account = f.Account.AccountNumber
                            }
                        );
                });
                return model;
            }
        }
    }
}