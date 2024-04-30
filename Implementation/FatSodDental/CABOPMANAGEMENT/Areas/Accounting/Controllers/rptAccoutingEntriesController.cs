using FatSod.Security.Abstracts;
using FatSod.Security.Entities;
using FatSod.Supply.Entities;
using CABOPMANAGEMENT.Controllers;
using CABOPMANAGEMENT.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using FatSod.Report.WrapReports;

namespace CABOPMANAGEMENT.Areas.Accounting.Controllers
{
    [Authorize(Order = 1)]
    [TakeBusinessDay(Order = 2)]
    public class rptAccoutingEntriesController : BaseController
    {
        //Current Controller and current page
        private const string CONTROLLER_NAME = "Accounting/rptAccoutingEntries";
        private const string VIEW_NAME = "Index";

        private IBusinessDay _busDayRepo;

        List<BusinessDay> bdDay;

        // GET: CashRegister/State
        public rptAccoutingEntriesController(
            IBusinessDay busDayRepo
            )
        {
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
        public JsonResult chargeSolde(int OperationID, int BranchID, int DeviseID, DateTime Bdate, DateTime Edate)
        {
            List<object> _InfoList = new List<object>();
            
            double TotalDebit = 0;
            double TotalCredit = 0;
            double Solde = 0;

            /*DateTime Bdate = (DateTime)Session["BeginDate"];
            DateTime Edate = (DateTime)Session["EndDate"];*/

            //recuperation des operation corespondant o type choisi
            foreach (Operation op in db.Operations.Where(o => o.JournalID == OperationID).ToList())
            {
                List<AccountOperation> listAccOp = db.AccountOperations.Where(ao => ao.BranchID == BranchID && ao.DeviseID == DeviseID
                    && ao.OperationID == op.OperationID && (ao.DateOperation >= Bdate && ao.DateOperation <= Edate)).ToList();
                if (listAccOp != null)
                {
                    TotalDebit += listAccOp.Select(s => s.Debit).Sum();
                    TotalCredit += listAccOp.Select(s => s.Credit).Sum();
                }
            }
            Solde = TotalCredit - TotalDebit;

            _InfoList.Add(new
            {
                TotalDebit = TotalDebit.ToString("0.0"),
                TotalCredit = TotalCredit.ToString("0.0"),
                Solde = Solde.ToString("0.0")
            });

            return Json(_InfoList, JsonRequestBehavior.AllowGet);
            
            
        }
        
        
        //[HttpPost]
        public JsonResult GetList(int OperationID, int BranchID, int DeviseID, DateTime Bdate, DateTime Edate)
        {
            var model = new
            {
                data = from c in ModelRepAcctOp(OperationID, BranchID, DeviseID, Bdate, Edate)
                       select new
                       {
                           RptEtatsJournalID = c.RptEtatsJournalID,
                           Agence = c.Agence,// br.BranchCode,
                           LibAgence = c.LibAgence,// br.BranchDescription,
                           Devise = c.Devise,
                           LibDevise = c.LibDevise,
                           CompteCle = c.CompteCle,
                           LibelleCpte = c.LibelleCpte,
                           CodeOperation = c.CodeOperation,// c.Operation.OperationCode,
                           LibelleOperation = c.LibelleOperation,// c.Operation.OperationDescription,
                           Reference = c.Reference,
                           Description = c.Desription,
                           DateOperation = c.DateOperation.ToString("yyyy-MM-dd"),
                           MontantDB = c.MontantDB,
                           MontantCR = c.MontantCR,
                           CodeTransaction=c.CodeTransaction,
                           Journal = c.Journal
                       }
            };

            return Json(model, JsonRequestBehavior.AllowGet);
            
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

        //list of operation type
        public JsonResult GetJournal()
        {
            
            List<object> operationTypeList = new List<object>();
            foreach (Journal operationType in db.Journals.ToArray().OrderBy(m => m.JournalCode))
            {
                operationTypeList.Add(new
                {
                    Journal=operationType.JournalLabel,
                    OperationID=operationType.JournalID
                });
            }
                
            return Json(operationTypeList, JsonRequestBehavior.AllowGet);
        }

        
        
        private List<RptEtatsJournal> ModelRepAcctOp(int OperationID, int BranchID, int DeviseID, DateTime Bdate, DateTime Edate)
        {
            List<RptEtatsJournal> list = new List<RptEtatsJournal>();
            try
            {

                Branch br = db.Branches.Find(BranchID);
                
                foreach (Operation op in db.Operations.Where(o => o.JournalID == OperationID).ToList())
                {
                    List<AccountOperation> listAccOp = db.AccountOperations.Where(ao => ao.BranchID == BranchID && ao.DeviseID == DeviseID && ao.OperationID == op.OperationID && (ao.DateOperation >= Bdate && ao.DateOperation <= Edate)).ToList();

                    listAccOp.ForEach(c =>
                    {
                        list.Add(
                                new RptEtatsJournal
                                {
                                    RptEtatsJournalID = (int)c.AccountOperationID,
                                    Agence = c.Branch.BranchCode,// br.BranchCode,
                                    LibAgence = c.Branch.BranchName,// br.BranchDescription,
                                    Devise = c.Devise.DeviseCode,
                                    LibDevise = c.Devise.DeviseDescription,
                                    CompteCle = c.Account.AccountNumber.ToString(),
                                    LibelleCpte = c.Account.AccountLabel,
                                    CodeOperation = op.OperationType.operationTypeCode,// c.Operation.OperationCode,
                                    LibelleOperation = op.OperationType.operationTypeDescription,// c.Operation.OperationDescription,
                                    Reference = c.Reference,
                                    Desription = c.Description,
                                    DateOperation = c.DateOperation,
                                    MontantDB = c.Debit,
                                    MontantCR = c.Credit,
                                    CodeTransaction=c.CodeTransaction,
                                    Journal=c.Operation.Journal.JournalCode
                                }
                     );
                    });
                }
            }
            catch (Exception ex)
            {
                Response.Write(ex.ToString());
            }
            return list;

        }

        
    }
}