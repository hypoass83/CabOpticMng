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
using FatSod.DataContext.Concrete;
using System.Web.UI;

namespace FatSodDental.UI.Areas.Administration.Controllers
{
    [Authorize(Order = 1)]
    [TakeBusinessDay(Order = 2)]
    public class FiscalYearController : BaseController
    {
        private IRepository<FiscalYear> _fiscalYearRepo;
        
        public FiscalYearController(IRepository<FiscalYear> fiscalYearRepo)
        {
            this._fiscalYearRepo = fiscalYearRepo;
            
        }
        // GET: Administration/FiscalYear
        
        [OutputCache(Duration = 3600)] 
        public ActionResult Index()
        {
            //Ext.Net.MVC.PartialViewResult rPVResult = new Ext.Net.MVC.PartialViewResult
            //{
            //    ClearContainer = true,
            //    //RenderMode = RenderMode.AddTo,
            //    ContainerId = "Body",
            //    //WrapByScriptTag = false,
            //    Model = ModelFiscalYear
            //};
            return View(ModelFiscalYear);
        }

        [HttpPost]
        public ActionResult AddFiscalYear(FiscalYear fiscalYear, int isOpen)
        {
            fiscalYear.FiscalYearStatus = Convert.ToBoolean(isOpen);
            if (fiscalYear.FiscalYearID > 0)
            {
                statusOperation = Resources.AlertUpdateAction;
                _fiscalYearRepo.Update(fiscalYear, fiscalYear.FiscalYearID);
            }
            else
            {
                statusOperation = Resources.AlertAddAction;
                _fiscalYearRepo.Create(fiscalYear);
            }
            this.AlertSucces(Resources.Success, statusOperation);
            this.GetCmp<FormPanel>("FiscalForm").Reset();
            this.GetCmp<Store>("FiscalYearListStore").Reload();
            return this.Direct();
        }
        [HttpPost]
        public StoreResult GetFiscalYearList()
        {
            return this.Store(ModelFiscalYear);
        }
        [HttpPost]
        public ActionResult DeleteFiscalYear(int ID)
        {
            try
            {
                db.FiscalYears.Remove(db.FiscalYears.Find(ID));
                db.SaveChanges();
                //_fiscalYearRepo.Delete(db.FiscalYears.Find(ID));
            }
            catch (Exception e)
            {
                X.Msg.Alert("Can't Delete It"," "+e.Message).Show();
            }
            return this.Direct();
        }
        [HttpPost]
        public ActionResult InitializeFieldsFiscalYear(int ID)
        {
            FiscalYear fiscal =  db.FiscalYears.Find(ID);
            this.GetCmp<NumberField>("FiscalYearID").Value = fiscal.FiscalYearID;
            this.GetCmp<NumberField>("FiscalYearNumber").Value = fiscal.FiscalYearNumber;
            //this.GetCmp<TextField>("FiscalYearStatus").Value = fiscal.FiscalYearStatus;
            this.GetCmp<TextField>("FiscalYearLabel").Value = fiscal.FiscalYearLabel;
            this.GetCmp<DateField>("StartFrom").Value = fiscal.StartFrom;
            this.GetCmp<DateField>("EndFrom").Value = fiscal.EndFrom;

            if (fiscal.FiscalYearStatus) this.GetCmp<Radio>("isOpen").Checked = true;
            else this.GetCmp<Radio>("isOpenNo").Checked = true;

            return this.Direct();
        }
        private List<object> ModelFiscalYear
        {
            get
            {
                
                List<object> model = new List<object>();
                
                 db.FiscalYears.ToList().ForEach(f =>
                {
                    model.Add(
                            new
                            {
                                FiscalYearID = f.FiscalYearID,
                                FiscalYearNumber = f.FiscalYearNumber,
                                FiscalYearStatus = f.FiscalYearStatus,
                                FiscalYearLabel = f.FiscalYearLabel,
                                StartFrom = f.StartFrom,
                                EndFrom = f.EndFrom
                            }                       
                        );
                });
                return model;
            }
        }
    }
}