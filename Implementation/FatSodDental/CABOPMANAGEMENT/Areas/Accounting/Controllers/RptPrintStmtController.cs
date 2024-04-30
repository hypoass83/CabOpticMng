
using FatSod.DataContext.Concrete;
using FatSod.DataContext.Initializer;
using FatSod.Ressources;
using FatSod.Security.Abstracts;
using FatSod.Security.Entities;
using FatSod.Supply.Abstracts;
using FatSod.Supply.Entities;
using CABOPMANAGEMENT.Controllers;
using CABOPMANAGEMENT.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using FatSod.Report.WrapReports;
using FatSod.DataContext.Repositories;
using CABOPMANAGEMENT.Filters;

namespace CABOPMANAGEMENT.Areas.Accounting.Controllers
{
    [Authorize(Order = 1)]
    [TakeBusinessDay(Order = 2)]
    public class RptPrintStmtController : BaseController
    {
        //Current Controller and current page
        private const string CONTROLLER_NAME = "Accounting/RptPrintStmt";
        private const string VIEW_NAME = "Index";
        //person repository

        
        private Company cmpny;

        private IBusinessDay _busDayRepo;

        List<BusinessDay> bdDay;
        //
        // GET: /Accounting/RptPrintStmt/

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

            IBusinessDay busDayRepo = new BusinessDayRepository();
            List<object> openedBranchesList = new List<object>();
            List<Branch> openedBranches = busDayRepo.GetOpenedBranches();
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

        public JsonResult LoadGLAccounts(string filter)
        {

            List<object> glList = new List<object>();
            var listDistAcc = db.AccountOperations
                .Where(c=>c.Account.AccountNumber.ToString().StartsWith(filter.ToLower()))
               .Select(a => new
               {
                   Account = a.Account
               }).Distinct().ToList().OrderBy(c => c.Account.AccountNumber);

            foreach (var account in listDistAcc.ToList())
            {
                string itemLabel = "";

                //si le client est une personne physique, on renvoie son nom et son prénom, si c'est une entreprise, on renvoie son nom
                itemLabel = account.Account.AccountNumber +" - "+ account.Account.AccountLabel;

                glList.Add(new
                {
                    Name = itemLabel,
                    ID = account.Account.AccountID
                });
            }

            return Json(glList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult chargeSoldeBefore(int BranchID, int DeviseID, int AccountID, DateTime Bdate)
        {
            List<object> _InfoList = new List<object>();

            double TotalDebitBefore = 0d;
            double TotalCreditBefore = 0d;
            double SoldeBefore = 0;

            List<AccountOperation> listAccOp = db.AccountOperations.Where(ao => ao.BranchID == BranchID && ao.DeviseID == DeviseID  && ao.AccountID == AccountID && ao.DateOperation<Bdate).ToList();
            if (listAccOp != null)
            {
                TotalDebitBefore = listAccOp.Select(s => s.Debit).Sum();
                TotalCreditBefore = listAccOp.Select(s => s.Credit).Sum();
            }

            SoldeBefore = TotalCreditBefore - TotalDebitBefore;

            _InfoList.Add(new
            {
                TotalDebit = TotalDebitBefore.ToString("0.0"),
                TotalCredit = TotalCreditBefore.ToString("0.0"),
                Solde = SoldeBefore.ToString("0.0")
            });

            return Json(_InfoList, JsonRequestBehavior.AllowGet);
        }
       


        public JsonResult GetList(int BranchID, int DeviseID, int AccountID, DateTime Bdate, DateTime Edate)
        {
            /*var model = new
            {
                data = from c in ModelRepAcctOp(BranchID, DeviseID, AccountID, Bdate, Edate)
                       select new
                       {
                           RptPrintStmtID = c.RptPrintStmtID,
                           Agence = c.Agence,
                           LibAgence = c.LibAgence,
                           Devise = c.Devise,
                           LibDevise = c.LibDevise,
                           AcctNo = c.AcctNo,
                           AcctName = c.AcctName,
                           RefOperation = c.RefOperation,
                           Description = c.Description,
                           DateOperation = c.DateOperation.ToString("yyyy-MM-dd"),
                           MtDebit = c.MtDebit,
                           MtCredit = c.MtCredit,
                           RepDebit = c.RepDebit,
                           RepCredit = c.RepCredit,
                           Solde = c.Solde,
                           Sens = c.Sens
                       }
            };*/

            var model = new
            {
                data = ModelRepAcctOp(BranchID, DeviseID, AccountID, Bdate, Edate)
            };

            return Json(model, JsonRequestBehavior.AllowGet);
        }

       
        private List<RptPrintStmt> ModelRepAcctOp(int BranchID, int DeviseID, int AccountID, DateTime Bdate, DateTime Edate)
        {
            double RepDebit = 0d;
            double RepCredit = 0d;
            double Solde = 0d;
            double CumulCredit=0d;
            double CumulDebit = 0d;

            Branch br = db.Branches.Find(BranchID);

            List<RptPrintStmt> list = new List<RptPrintStmt>();

           
            int AcctNo = AccountID;
            
           
            cmpny = db.Companies.FirstOrDefault();
            List<AccountOperation> listRepAccOp = db.AccountOperations.Where(ao => ao.BranchID == BranchID && ao.DeviseID == DeviseID && (ao.DateOperation < Bdate) && ao.AccountID == AcctNo).ToList();
            if (listRepAccOp != null)
            {
                RepDebit = listRepAccOp.Select(s => s.Debit).Sum();
                RepCredit = listRepAccOp.Select(s => s.Credit).Sum();
            }
            CumulCredit = RepCredit;
            CumulDebit = RepDebit;
            List<AccountOperation> listAccOp = db.AccountOperations.Where(ao => ao.BranchID == BranchID && ao.DeviseID == DeviseID && (ao.DateOperation >= Bdate && ao.DateOperation <= Edate) && ao.AccountID == AcctNo ).ToList();
            foreach (AccountOperation c in listAccOp)
            {
                Solde = 0d;
                CumulCredit+=c.Credit;
                CumulDebit += c.Debit;
                Solde = CumulCredit - CumulDebit;

                list.Add(
                             new RptPrintStmt
                             {
                                 RptPrintStmtID = (int)c.AccountOperationID,
                                 Agence = c.Branch.BranchCode,// br.BranchCode,
                                 LibAgence = c.Branch.BranchDescription,// br.BranchDescription,
                                 Devise = c.Devise.DeviseCode,
                                 LibDevise = c.Devise.DeviseDescription,
                                 AcctNo = c.Account.AccountNumber.ToString(),
                                 AcctName = c.Account.AccountLabel,
                                 RefOperation = c.Reference,
                                 Description = c.Description,
                                 DateOperation = c.DateOperation.ToString("yyy-"),
                                 MtDebit = c.Debit,
                                 MtCredit = c.Credit,
                                 RepDebit = RepDebit,
                                 RepCredit = RepCredit,
                                 Solde = Solde,
                                 Sens = Solde > 0 ? "Cr" : "Db"
                             }
                         );
            }
            
            return list;

        }

        
    }
}