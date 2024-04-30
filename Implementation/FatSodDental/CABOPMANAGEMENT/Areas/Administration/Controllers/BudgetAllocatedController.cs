using FatSod.Security.Abstracts;
using FatSod.Security.Entities;
using CABOPMANAGEMENT.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using FatSod.Ressources;
using CABOPMANAGEMENT.Filters;
using FatSod.Budget.Entities;

namespace CABOPMANAGEMENT.Areas.Administration.Controllers
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
        
       // [OutputCache(Duration = 3600)] 
        public ActionResult Index()
        {
            //rPVResult.ViewBag.fiscalYear = currentFiscalYear;
            Session["BudgetAllocatedList"] = new List<BudgetAllocated>();
            return View(ModelBudgetAllocatedLine(true));
        }

        [HttpGet]
        public JsonResult GetBudgetAllocatedList()
        {

            var model = new
            {
                data = from obj in ModelBudgetAllocatedLine(false) select new { BudgetAllocatedID = obj.BudgetAllocatedID, Branch = obj.Branch.BranchName, FiscalYear = obj.FiscalYear.FiscalYearNumber.ToString(), BudgetLine = obj.BudgetLine.BudgetLineLabel, AllocateAmount = obj.AllocateAmount }
            };
            
            return Json(model , JsonRequestBehavior.AllowGet);
         
        }
        /*
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
        */

        public JsonResult GetBudgetLine(string filter, int? FiscalYearID)
        {

            List<BudgetLine> budlines = ModelBudgetLine(FiscalYearID.Value);

            if (!string.IsNullOrEmpty(filter) && filter != "*")
            {
                budlines.RemoveAll(bl => !bl.BudgetLineLabel.ToLower().StartsWith(filter.ToLower()));
            }


            return Json(budlines, JsonRequestBehavior.AllowGet);

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
        //[HttpPost]
        public JsonResult AllocateBudget()
        {
            bool status = false;
            try
            {
                List<BudgetAllocated> listAll = (List<BudgetAllocated>)Session["BudgetAllocatedList"];
                if (listAll.Count>0)
                {
                    listAll.ForEach(f =>
                    {
                        _budgetAllocatedRepo.Create(
                            new BudgetAllocated
                            {
                                BranchID = f.BranchID,
                                FiscalYearID = f.FiscalYearID,
                                BudgetLineID = f.BudgetLineID,
                                AllocateAmount = f.AllocateAmount,
                            }
                         );
                    });
                    statusOperation = Resources.AlertAddAction;
                    Session["BudgetAllocatedList"] = null;
                    status = true;
                }
                else
                {
                    status = false;
                    statusOperation = "The Grid must have a value";
                }
                
                
            }
            catch (Exception e)
            {
                status = false;
                statusOperation = e.Message + " " + e.InnerException;
            }
            return new JsonResult { Data = new { status = status, Message = statusOperation } };
        }
        //[HttpPost]
        public JsonResult AddBudgetAllocatedLine(BudgetAllocated budgetAllocated, int BranchID = 0, int FiscalYearID = 0)
        {
            bool status = false;
            try
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
                status = true;
                statusOperation = Resources.AlertAddAction;
            }
            catch (Exception e)
            {
                status = false;
                statusOperation = e.Message + " " + e.InnerException;
            }

            return new JsonResult { Data = new { status = status, Message = statusOperation } };
        }
        
       
        //[HttpPost]
        public JsonResult DeleteBudgetAllocatedLine(int ID)
        {
            bool status = false;
            try
            {
                List<BudgetAllocated> listAll = (List<BudgetAllocated>)Session["BudgetAllocatedList"];
                listAll.Remove(listAll.SingleOrDefault(b => b.BudgetAllocatedID == ID));
                status = true;
                statusOperation = Resources.AlertDeleteAction;
            }
            catch (Exception e)
            {
                status = false;
                statusOperation = "Can't Delete It " + e.Message + " " + e.InnerException;
            }
            return new JsonResult { Data = new { status = status, Message = statusOperation } };
        }
        //[HttpPost]
        public ActionResult InitializeFieldsBudgetAllocated(int ID)
        {
            var item = db.BudgetAllocateds.Find(ID);
            List<object> _BudgetAllocatedList = new List<object>();
            _BudgetAllocatedList.Add(new
            {
                BudgetLineID = item.BudgetLineID,
                AllocateAmount = item.AllocateAmount
            });
            return Json(_BudgetAllocatedList, JsonRequestBehavior.AllowGet);
            
        }
        /// <summary>
        ///  Retrieve data of source datas. 
        /// </summary>
        /// <param name="isDataBase">Make true to retrieve datas to database or false to retreive of session</param>
        /// <returns></returns>
        private List<BudgetAllocated> ModelBudgetAllocatedLine(bool isDataBase)
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
            List<BudgetAllocated> model = new List<BudgetAllocated>();
            if (budgetAllocatedList!=null)
            {
                budgetAllocatedList.ForEach(f =>
                {
                    model.Add(
                            new BudgetAllocated
                            {
                                BudgetAllocatedID = f.BudgetAllocatedID,
                                Branch = f.Branch,
                                BranchID = f.BranchID,
                                FiscalYear = f.FiscalYear,
                                FiscalYearID = f.FiscalYearID,
                                BudgetLine = f.BudgetLine,
                                BudgetLineID = f.BudgetLineID,
                                AllocateAmount = f.AllocateAmount
                            }
                        );
                });
            }
            
            return model;
        }
        public JsonResult GetBranchOpenedBusday()
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
                            ID = c.BranchID,
                            Name = c.BranchName
                        }
                    );
            });

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public JsonResult FiscalYears()
        {
            List<object> budgetModel = new List<object>();
            foreach (FiscalYear dL in db.FiscalYears.Where(t => t.FiscalYearStatus).ToArray())
            {
                budgetModel.Add(new
                {
                    ID = dL.FiscalYearID,
                    Name = dL.FiscalYearLabel
                });
            }
            return Json(budgetModel, JsonRequestBehavior.AllowGet);
        }
    }
}