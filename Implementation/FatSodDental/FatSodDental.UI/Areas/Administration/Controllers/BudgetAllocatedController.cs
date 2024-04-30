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
using System.Collections;
using FatSod.DataContext.Concrete;
using System.Web.UI;

namespace FatSodDental.UI.Areas.Administration.Controllers
{
    [Authorize(Order = 1)]
    [TakeBusinessDay(Order = 2)]
    public class BudgetAllocatedController : BaseController
    {
        private IRepository<BudgetAllocated> _budgetAllocatedRepo;

        private IBusinessDay _busDayRepo;
        
        public BudgetAllocatedController(IBusinessDay busDayRepo,IRepository<BudgetAllocated> budgetAllocatedRepo)
        {
            this._budgetAllocatedRepo = budgetAllocatedRepo;
            this._busDayRepo = busDayRepo;
            
        }
        
        [OutputCache(Duration = 3600)] 
        public ActionResult Index()
        {
            try
            {

            //Ext.Net.MVC.PartialViewResult rPVResult = new Ext.Net.MVC.PartialViewResult
            //{
            //    ClearContainer = true,
            //    //RenderMode = RenderMode.AddTo,
            //    ContainerId = "Body",
            //    //WrapByScriptTag = false,
            //    Model =ModelBudgetAllocatedLine(true)
            //};
            
            //rPVResult.ViewBag.fiscalYear = currentFiscalYear;
            Session["BudgetAllocatedList"] = new List<BudgetAllocated>();
            return View(ModelBudgetAllocatedLine(true));
            }

            catch (Exception e)
            {
                X.Msg.Alert("Error ", e.Message).Show();
                return this.Direct();
            }
        }

        public ActionResult GetAllBudgetLine(int start, int limit, int page, string query, int? FiscalYearID)
        {
            //bool isUpdate = (bool)Session["isUpdate"];
            //string BudgetLineLabel = (string)Session["BudgetLineLabel"];

            //query = (isUpdate == true) ? BudgetLineLabel : query;

            Paging<BudgetLine> budgetLine = GetBudgetLine(start, limit, "", "", query, FiscalYearID);

            //Session["isUpdate"] = false;
            //Session["BudgetLineLabel"] = "*";

            return this.Store(budgetLine.Data, budgetLine.TotalRecords);
        }

        public Paging<BudgetLine> GetBudgetLine(int start, int limit, string sort, string dir, string filter, int? FiscalYearID)
        {

            List<BudgetLine> budlines = ModelBudgetLine(FiscalYearID.Value);

            if (!string.IsNullOrEmpty(filter) && filter != "*")
            {
                budlines.RemoveAll(bl => !bl.BudgetLineLabel.ToLower().StartsWith(filter.ToLower()));
            }

            if (!string.IsNullOrEmpty(sort))
            {
                budlines.Sort(delegate(BudgetLine x, BudgetLine y)
                {
                    object a;
                    object b;

                    int direction = dir == "DESC" ? -1 : 1;

                    a = x.GetType().GetProperty(sort).GetValue(x, null);
                    b = y.GetType().GetProperty(sort).GetValue(y, null);

                    return CaseInsensitiveComparer.Default.Compare(a, b) * direction;
                });
            }

            if ((start + limit) > budlines.Count)
            {
                limit = budlines.Count - start;
            }

            List<BudgetLine> rangePlants = (start < 0 || limit < 0) ? budlines : budlines.GetRange(start, limit);

            return new Paging<BudgetLine>(rangePlants, budlines.Count);

        }

        public List<BudgetLine> ModelBudgetLine(int FiscalYearID)
        {
            
            List<BudgetLine> budgetModel = new List<BudgetLine>();

            var query =
            from bl in db.BudgetLines
            where !(from ba in db.BudgetAllocateds
                    where ba.FiscalYearID == FiscalYearID
                    select ba.BudgetLineID)
                   .Contains(bl.BudgetLineID)
            select bl;

            //il fo retourner la liste des lignes qui nont pas ete programmer pr l'annee en cours
            foreach (var dL in 
                query) 
            {
                BudgetLine ln2 = new BudgetLine()
                {
                    BudgetLineLabel = dL.BudgetLineLabel,
                    BudgetLineID = dL.BudgetLineID
                };
                budgetModel.Add(ln2);
            }
            return budgetModel;
        }
        [HttpPost]
        public ActionResult AllocateBudget()
        {
            List<BudgetAllocated> listAll = (List<BudgetAllocated>)Session["BudgetAllocatedList"];
            listAll.ForEach(f =>
            {
                _budgetAllocatedRepo.Create(
                    new BudgetAllocated
                    {
                        BranchID = f.BranchID,
                        FiscalYearID = f.FiscalYearID,
                        BudgetLineID = f.BudgetLineID,
                        //BudgetAllocatedLabel = f.BudgetAllocatedLabel,
                        AllocateAmount = f.AllocateAmount,                        
                    }
                 );
            });
            statusOperation = Resources.AlertAddAction;
            Session["BudgetAllocatedList"] = null;
			this.GetCmp<FormPanel>("GlobalForm").Reset();
            this.GetCmp<Store>("BudgetAllocatedList").Reload();
            //this.GetCmp<Store>("BudgetAllocatedSavingList").Reload();

            this.AlertSucces(Resources.Success, statusOperation);
            return this.Direct();
        }
        [HttpPost]
        public ActionResult AddBudgetAllocatedLine(BudgetAllocated budgetAllocated, int BranchID = 0, int FiscalYearID = 0)
        {
            budgetAllocated.BranchID = BranchID;
            budgetAllocated.Branch =db.Branches.Find(BranchID);
            budgetAllocated.FiscalYear = db.FiscalYears.Find(FiscalYearID);
            budgetAllocated.FiscalYearID = FiscalYearID;

            budgetAllocated.BudgetLine =db.BudgetLines.Find(budgetAllocated.BudgetLineID);
            
            List<BudgetAllocated> listAll = (List<BudgetAllocated>)Session["BudgetAllocatedList"];
            if (budgetAllocated.BudgetAllocatedID > 0)
            {
                listAll.Remove(listAll.SingleOrDefault(b => b.BudgetAllocatedID == budgetAllocated.BudgetAllocatedID));
            }
            if (listAll != null && listAll.Count() > 0)
            {
                var existingLine = listAll.SingleOrDefault(b => b.BudgetLineID == budgetAllocated.BudgetLineID);
                if (existingLine != null)
                    listAll.Remove(existingLine);
                budgetAllocated.BudgetAllocatedID = listAll.LastOrDefault() != null ? listAll.LastOrDefault().BudgetAllocatedID + 1 : 1;
                listAll.Add(budgetAllocated);
            }
            else
            {
                listAll = new List<BudgetAllocated>();
                budgetAllocated.BudgetAllocatedID = 1;
                listAll.Add(budgetAllocated);
            }
            Session["BudgetAllocatedList"] = listAll;
            this.GetCmp<Store>("BudgetAllocatedList").Reload();
            return this.Direct();
        }
        [HttpPost]
        public StoreResult GetBudgetAllocatedList()
        {
            return this.Store(ModelBudgetAllocatedLine(false));
        }
        //[HttpPost]
        //public StoreResult GetBudgetAllocatedSavingList()
        //{
        //    return this.Store(ModelBudgetAllocatedLine(true));
        //}
        [HttpPost]
        public ActionResult DeleteBudgetAllocatedLine(int ID)
        {
            List<BudgetAllocated> listAll = (List<BudgetAllocated>)Session["BudgetAllocatedList"];
            listAll.Remove(listAll.SingleOrDefault(b => b.BudgetAllocatedID == ID));
            this.GetCmp<Store>("BudgetAllocatedList").Reload();
            return this.Direct();
        }
        [HttpPost]
        public ActionResult InitializeFieldsBudgetAllocated(int ID)
        {
            var item = db.BudgetAllocateds.Find(ID);
            this.GetCmp<ComboBox>("BudgetLineID").Value = item.BudgetLineID;
            this.GetCmp<NumberField>("AllocateAmount").Value = item.AllocateAmount;
            return this.Direct();
        }
        /// <summary>
        ///  Retrieve data of source datas. 
        /// </summary>
        /// <param name="isDataBase">Make true to retrieve datas to database or false to retreive of session</param>
        /// <returns></returns>
        private List<object> ModelBudgetAllocatedLine(bool isDataBase)
        {
            List<BudgetAllocated> budgetAllocatedList = new List<BudgetAllocated>();
            if (isDataBase)
            {
                budgetAllocatedList = db.BudgetAllocateds.ToList();
            }
            else
            {
                budgetAllocatedList = (List<BudgetAllocated>)Session["BudgetAllocatedList"];
            }
            List<object> model = new List<object>();
            if (budgetAllocatedList!=null)
            {
                budgetAllocatedList.ForEach(f =>
                {
                    model.Add(
                            new
                            {
                                BudgetAllocatedID = f.BudgetAllocatedID,
                                BranchName = f.Branch.BranchName,
                                BranchID = f.BranchID,
                                FiscalYearLabel = f.FiscalYear.FiscalYearLabel,
                                FiscalYearID = f.FiscalYearID,
                                BudgetLine = f.BudgetLine.BudgetLineLabel,
                                BudgetLineID = f.BudgetLineID,
                                BudgetAllocatedLabel = f.BudgetLine.BudgetLineLabel,
                                AllocateAmount = f.AllocateAmount
                            }
                        );
                });
            }
            
            return model;
        }
        public ActionResult GetBranchOpenedBusday()
        {
            List<object> model = new List<object>();
            List<BusinessDay> listBDUser = (List<BusinessDay>)Session["UserBusDays"];
            if (listBDUser == null)
            {
                listBDUser = _busDayRepo.GetOpenedBusinessDay(CurrentUser);
            }
            listBDUser.ForEach(c =>
            {
                model.Add(
                        new
                        {
                            BranchID = c.BranchID,
                            BranchName = c.BranchName
                        }
                    );
            });
            return this.Store(model);
        }
    }
}