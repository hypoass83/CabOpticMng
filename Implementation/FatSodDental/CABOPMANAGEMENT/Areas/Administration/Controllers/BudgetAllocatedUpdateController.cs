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
    public class BudgetAllocatedUpdateController : BaseController
    {
        private IRepository<BudgetAllocatedUpdate> _budgetAllUpRepo;
        private List<BusinessDay> bdDay;
        private IBusinessDay _busDayRepo;

        public BudgetAllocatedUpdateController(IBusinessDay busDayRepo, 
            IRepository<BudgetAllocatedUpdate> budgetAllocatedUpdateRepo)
        {
            this._budgetAllUpRepo = budgetAllocatedUpdateRepo;
            this._busDayRepo = busDayRepo;
        }
        
        [OutputCache(Duration = 3600)] 
        public ActionResult Index()
        {
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

            ViewBag.CurrentBranch = bdDay.FirstOrDefault().BranchID;
            ViewBag.BusnessDayDate = busDays.ToString("yyyy-MM-dd");
            Session["BusnessDayDate"] = busDays;
            return View(ModelBudgetAllocatedUpLine(true));
        }

        public JsonResult BudgetAllocateds()
        {
            
            List<object> budgetModel = new List<object>();
            //int year = DateTime.Today.Year;
            foreach (BudgetAllocated dL in db.BudgetAllocateds.Where(q => q.FiscalYear.FiscalYearStatus).ToArray().OrderBy(c=>c.BudgetLine.BudgetLineLabel))
            {
                budgetModel.Add(new {
                    BudgetLineLabel =dL.BudgetLine.BudgetLineLabel,
                    BudgetAllocatedID=dL.BudgetAllocatedID });
            }
            return Json(budgetModel, JsonRequestBehavior.AllowGet);

        }

        public JsonResult OpenedBusday()
        {
            List<object> list = new List<object>();

            bdDay = (List<BusinessDay>)Session["UserBusDays"];
            if (bdDay == null)
            {
                bdDay = _busDayRepo.GetOpenedBusinessDay(CurrentUser);
            };

            foreach (BusinessDay busDay in bdDay)
            {
                list.Add(
                    new
                    {
                        BranchID = busDay.BranchID,
                        BranchName = busDay.Branch.BranchCode
                    }
                    );
            }

            return Json(list, JsonRequestBehavior.AllowGet);

        }
        //[HttpPost]
        public JsonResult AddBudgetAllUpLine(BudgetAllocatedUpdate budgetAlocatedUpdate, int BudgetAllocatedID)
        {
            bool status = false;
            string Message = "";
            try
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
                Message = "ok";
                status = true;
            }
            catch (Exception e)
            {
                status = false;
                Message = "Error " + e.Message + " " + e.StackTrace + " " + e.InnerException;
            }
            return new JsonResult { Data = new { status = status, Message = statusOperation } };
        }
        //[HttpPost]
        public JsonResult AddBudgetAllocatedUpdate(string Justification)
        {
            bool status = false;
            string Message = "";
            try
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
                status = true;
                Message = Resources.Success + " - " + statusOperation;
            }
            catch (Exception e)
            {
                status = false;
                Message = "Error " + e.Message + " " + e.StackTrace + " " + e.InnerException;
            }
            return new JsonResult { Data = new { status = status, Message = Message } };
        }
        //[HttpPost]
        public JsonResult GetBudgetAllocatedUpdateList()
        {
            var model = new
            {
                data = from f in ModelBudgetAllocatedUpLine(false)
                select new
                {
                    BudgetAllocatedID = f.BudgetAllocatedID,
                    BudgetAllocatedLabel = f.BudgetAllocated.BudgetLine.BudgetLineLabel,
                    Justification = f.Justification,
                    SensImputation = f.SensImputation,
                    BudgetAllocatedUpdateID = f.BudgetAllocatedUpdateID,
                    Amount = f.Amount
                }
            };
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        //[HttpPost]
        public JsonResult DeleteBudgetAllocatedUpdate(int ID)
        {
            List<BudgetAllocatedUpdate> listAll = (List<BudgetAllocatedUpdate>)Session["BudgetAllocatedUpdateList"];
            listAll.Remove(listAll.SingleOrDefault(b => b.BudgetAllocatedUpdateID == ID));
            var model = new
            {
                data = from f in ModelBudgetAllocatedUpLine(false)
                       select new
                       {
                           BudgetAllocatedID = f.BudgetAllocatedID,
                           BudgetAllocatedLabel = f.BudgetAllocated.BudgetLine.BudgetLineLabel,
                           Justification = f.Justification,
                           SensImputation = f.SensImputation,
                           BudgetAllocatedUpdateID = f.BudgetAllocatedUpdateID,
                           Amount = f.Amount
                       }
            };
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        //[HttpPost]
        public JsonResult InitializeFieldsBudgetAllocatedUpdate(int ID)
        {
            List<object> _InfoList = new List<object>();
            var item = _budgetAllUpRepo.Find(ID);

            _InfoList.Add(new
            {
                SensImputation = item.SensImputation,
                Amount = item.Amount
            });
            return Json(_InfoList, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        ///  Retrieve data of source datas. 
        /// </summary>
        /// <param name="isDataBase">Make true to retrieve datas to database or false to retreive of session</param>
        /// <returns></returns>
        private List<BudgetAllocatedUpdate> ModelBudgetAllocatedUpLine(bool isDataBase)
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
            List<BudgetAllocatedUpdate> model = new List<BudgetAllocatedUpdate>();
            if (budgetList != null)
            {
                budgetList.ForEach(f =>
                {
                    model.Add(
                            new BudgetAllocatedUpdate
                            {
                                BudgetAllocatedID = f.BudgetAllocatedID,
                                BudgetAllocated = f.BudgetAllocated,
                                Justification = f.Justification,
                                SensImputation = f.SensImputation,
                                BudgetAllocatedUpdateID = f.BudgetAllocatedUpdateID,
                                Amount = f.Amount
                            }
                        );
                });
            }
            
            return model;
        }
    }
}