using FatSod.Security.Abstracts;
using FatSod.Supply.Entities;
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
    public class BudgetLineController : BaseController
    {
        private IRepository<BudgetLine> _budgetLineRepo;
       
        // GET: Administration/BudgetLine
        public BudgetLineController( 
            IRepository<BudgetLine> budgetLineRepo)
        {
            this._budgetLineRepo = budgetLineRepo;
        }
        
        public ActionResult Index()
        {
           
            return View(ModelBudgetLine);
        }

        public JsonResult AccountOfClass(int classe)
        {
            List<object> accountingList = new List<object>();
            List<Account> accounts = db.Accounts.Where(a => a.CollectifAccount.AccountingSection.ClassAccount.ClassAccountNumber == classe).ToList();
            foreach (Account account in accounts)
            {
                accountingList.Add(new
                {
                    ID= account.AccountID,
                    Name= account.AccountLabel + "-" + account.AccountNumber.ToString()
                });
            }
            return Json(accountingList, JsonRequestBehavior.AllowGet);
        }

        
        //[HttpPost]
        public JsonResult AddBudgetLine(BudgetLine budgetLine, int BudgetControl)
        {
            bool status = false;
            try
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
                status = true;
            }
            catch (Exception e)
            {
                status = false;
                statusOperation = e.Message + " " + e.InnerException;
            }

            return new JsonResult { Data = new { status = status, Message = statusOperation } };
        }
        
        //[HttpPost]
        public JsonResult DeleteBudgetLine(int ID)
        {
            bool status = false;
            try
            {
                db.BudgetLines.Remove(db.BudgetLines.Find(ID));
                db.SaveChanges();
                status = true;
                statusOperation = Resources.AlertDeleteAction;
                //_budgetLineRepo.Delete(db.BudgetLines.Find(ID));
            }
            catch (Exception e)
            {
                status = false;
                statusOperation = "Can't Delete It " + e.Message + " " + e.InnerException;
            }
            return new JsonResult { Data = new { status = status, Message = statusOperation } };
        }
        //[HttpPost]
        public ActionResult InitializeFieldsBudgetLine(int ID)
        {
            BudgetLine budgetLine = db.BudgetLines.Find(ID);
            List<object> _budgetLineList = new List<object>();
            _budgetLineList.Add(new
            {
                BudgetLineID = budgetLine.BudgetLineID,
                BudgetType = budgetLine.BudgetType,
                BudgetCode = budgetLine.BudgetCode,
                BudgetLineLabel = budgetLine.BudgetLineLabel,
                AccountID = budgetLine.AccountID,
                BudgetControl = Convert.ToInt16( budgetLine.BudgetControl)
            });
            return Json(_budgetLineList, JsonRequestBehavior.AllowGet);
            
        }
        private List<BudgetLine> ModelBudgetLine
        {
            get
            {
                List<BudgetLine> model = new List<BudgetLine>();
                db.BudgetLines.ToList().ForEach(f =>
                {
                    model.Add(
                            new BudgetLine
                            {
                                BudgetLineID = f.BudgetLineID,
                                BudgetControl = f.BudgetControl,
                                BudgetType = f.BudgetType,
                                BudgetCode = f.BudgetCode,
                                BudgetLineLabel = f.BudgetLineLabel,
                                Account = f.Account
                            }
                        );
                });
                return model;
            }
        }

        private ActionResult TableData()
        {
            
            List<BudgetLine> model = new List<BudgetLine>();
            db.BudgetLines.ToList().ForEach(f =>
            {
                model.Add(
                        new BudgetLine
                        {
                            BudgetLineID = f.BudgetLineID,
                            BudgetControl = f.BudgetControl,
                            BudgetType = f.BudgetType,
                            BudgetCode = f.BudgetCode,
                            BudgetLineLabel = f.BudgetLineLabel,
                            Account = f.Account
                        }
                    );
            });

            return PartialView("TableData", model);
        }
    }
}