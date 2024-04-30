using FatSod.Ressources;
using FatSod.Security.Entities;
using FatSod.Supply.Entities;
using CABOPMANAGEMENT.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CABOPMANAGEMENT.Filters;
using FatSod.Security.Abstracts;
using FatSod.Report.WrapReports;

namespace CABOPMANAGEMENT.Areas.Accounting.Controllers
{
    [Authorize(Order = 1)]
    [TakeBusinessDay(Order = 2)]
    public class RptIncomeExpenseController : BaseController
    {
        //Current Controller and current page
        private const string CONTROLLER_NAME = "Accounting/RptIncomeExpense";
        private const string VIEW_NAME = "Index";
        //person repository

        private IBusinessDay _busDayRepo;

        List<BusinessDay> bdDay;

        // GET: CashRegister/State
        public RptIncomeExpenseController(
            IBusinessDay busDayRepo
            )
        {
            this._busDayRepo = busDayRepo;
        }

        //
        // GET: /Accounting/RptIncomeExpense/

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

            int deviseID = (Session["DefaultDeviseID"] == null) ? 0 : (int)Session["DefaultDeviseID"];
            if (deviseID <= 0)
            {
                InjectUserConfigInSession();
            }
            deviseID = (Session["DefaultDeviseID"] == null) ? 0 : (int)Session["DefaultDeviseID"];
            ViewBag.DefaultDeviseID = deviseID;
            ViewBag.DefaultDevise = (deviseID <= 0) ? "" : db.Devises.Find(deviseID).DeviseCode;
            return View();
        }

        /*** methodes lors du chargement du formulaire*********************/
        public JsonResult GetOpenedBranches()
        {

            List<object> openedBranchesList = new List<object>();
            List<Branch> openedBranches = _busDayRepo.GetOpenedBranches();
            foreach (Branch branch in openedBranches)
            {
                openedBranchesList.Add(new
                {
                    BranchID = branch.BranchID,
                    BranchName = branch.BranchName
                });
            }

            return Json(openedBranchesList, JsonRequestBehavior.AllowGet);
        }

        

        public JsonResult GetList(int BranchID, int DeviseID, DateTime Bdate, DateTime Edate)
        {
            var model = new
            {
                data = from c in ModelAcctOp(BranchID, DeviseID, Bdate, Edate)
                       select new
                       {
                           RptIncomeExpenseID = c.RptIncomeExpenseID,
                           Agence = c.Agence,
                           LibAgence = c.LibAgence,
                           Devise = c.Devise,
                           LibDevise = c.LibDevise,
                           AcctNumber = c.AcctNumber,
                           AcctName = c.AcctName,
                           AccountType = c.AccountType,
                           MonthTotal = c.MonthTotal,
                           MonthCumul = c.MonthCumul,
                           earningsmonth = c.earningsmonth,
                           earningscumul = c.earningscumul
                       }
            };

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        private List<RptIncomeExpense> ModelAcctOp(int BranchID, int DeviseID, DateTime Bdate, DateTime Edate) //, string BeginDate, string EndDate)
        {
            List<RptIncomeExpense> list = new List<RptIncomeExpense>();

            

            string AccountType="";
            double MonthTotal = 0d;
            double MonthCumul = 0d;

           
            Branch br = db.Branches.FirstOrDefault(b => b.BranchID == BranchID);
            Devise dev = db.Devises.FirstOrDefault(d => d.DeviseID == DeviseID);

            List<AccountOperation> lstearningsmonth7 = db.AccountOperations.Where(ao => ao.BranchID == BranchID && ao.DeviseID == DeviseID
                && (ao.Account.CollectifAccount.AccountingSection.ClassAccount.ClassAccountNumber == 7) && (ao.DateOperation >= Bdate && ao.DateOperation <= Edate)).ToList();

            List<AccountOperation> lstearningsmonth6 = db.AccountOperations.Where(ao => ao.BranchID == BranchID && ao.DeviseID == DeviseID
                && (ao.Account.CollectifAccount.AccountingSection.ClassAccount.ClassAccountNumber == 6) && (ao.DateOperation >= Bdate && ao.DateOperation <= Edate)).ToList();

            double earningsmonth = ((lstearningsmonth7==null) ? 0: lstearningsmonth7.Select(s => s.Debit - s.Credit).Sum()) -
                ((lstearningsmonth6 == null) ? 0 : lstearningsmonth6.Select(s => s.Debit - s.Credit).Sum());


            List<AccountOperation> earningscumul7 = db.AccountOperations.Where(ao => ao.BranchID == BranchID && ao.DeviseID == DeviseID
                && (ao.Account.CollectifAccount.AccountingSection.ClassAccount.ClassAccountNumber == 7) && (ao.DateOperation <= Edate)).ToList();

            List<AccountOperation> earningscumul6 = db.AccountOperations.Where(ao => ao.BranchID == BranchID && ao.DeviseID == DeviseID
                && (ao.Account.CollectifAccount.AccountingSection.ClassAccount.ClassAccountNumber == 6) && (ao.DateOperation <= Edate)).ToList();

            double earningscumul = ((earningscumul7 == null) ? 0 : earningscumul7.Select(s => s.Debit - s.Credit).Sum()) -
                ((earningscumul6 == null) ? 0 : earningscumul6.Select(s => s.Debit - s.Credit).Sum());

            

            //retourne la liste distinct des cpte ds le grand livre des classe 6 et 7
            var listDistAcc = db.AccountOperations.Where(ao => ao.BranchID == BranchID && ao.DeviseID == DeviseID
                && (ao.Account.CollectifAccount.AccountingSection.ClassAccount.ClassAccountNumber == 6 || ao.Account.CollectifAccount.AccountingSection.ClassAccount.ClassAccountNumber == 7))
                .Select(a => new 
                { 
                    Account = a.Account
                }).Distinct().ToList();


            listDistAcc.ForEach(c =>
            {
                AccountType = "";
                MonthTotal = 0d;
                MonthCumul = 0d;
                List<AccountOperation> listAccMvt = db.AccountOperations.Where(ao => ao.BranchID == BranchID && ao.DeviseID == DeviseID && (ao.DateOperation >= Bdate && ao.DateOperation <= Edate) && ao.AccountID == c.Account.AccountID).ToList();
                List<AccountOperation> listAccfin = db.AccountOperations.Where(ao => ao.BranchID == BranchID && ao.DeviseID == DeviseID && (ao.DateOperation <= Edate) && ao.AccountID == c.Account.AccountID).ToList();
                if (c.Account.CollectifAccount.AccountingSection.ClassAccount.ClassAccountNumber == 6)
                { 
                    AccountType = Resources.Expense;
                    MonthTotal = (listAccMvt==null) ?0 : listAccMvt.Select(s => s.Debit - s.Credit).Sum();
                    MonthCumul = (listAccfin == null) ? 0 : listAccfin.Select(s => s.Debit - s.Credit).Sum();
                }
                else if (c.Account.CollectifAccount.AccountingSection.ClassAccount.ClassAccountNumber == 7)
                {
                    AccountType = Resources.Income;
                    MonthTotal = (listAccMvt == null) ? 0 : listAccMvt.Select(s => s.Credit - s.Debit).Sum();
                    MonthCumul = (listAccfin == null) ? 0 : listAccfin.Select(s => s.Credit - s.Debit).Sum();
                }
                list.Add(
                            new RptIncomeExpense
                            {
                                RptIncomeExpenseID = 1,
                                Agence = br.BranchCode,
                                LibAgence = br.BranchName,
                                Devise = dev.DeviseCode,
                                LibDevise = dev.DeviseLabel,
                                AcctNumber = c.Account.AccountNumber.ToString(),
                                AcctName = c.Account.AccountLabel,
                                AccountType=AccountType,
                                MonthTotal = MonthTotal,
                                MonthCumul = MonthCumul,
                                earningsmonth = earningsmonth,
                                earningscumul = earningscumul
                            }
                );
            });

            return list;

        }

       
    }
}