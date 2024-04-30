using FatSod.Ressources;
using FatSod.Security.Entities;
using FatSod.Supply.Entities;
using CABOPMANAGEMENT.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using FatSod.Security.Abstracts;
using FatSod.Report.WrapReports;
using CABOPMANAGEMENT.Filters;

namespace CABOPMANAGEMENT.Areas.Accounting.Controllers
{
    [Authorize(Order = 1)]
    [TakeBusinessDay(Order = 2)]
    public class RptBalanceGeneraleController : BaseController
    {
        //Current Controller and current page
        private const string CONTROLLER_NAME = "Accounting/RptBalanceGenerale";
        private const string VIEW_NAME = "Index";

        private IBusinessDay _busDayRepo;

        List<BusinessDay> bdDay;

        // GET: CashRegister/State
        public RptBalanceGeneraleController(
            IBusinessDay busDayRepo
            )
        {
            this._busDayRepo = busDayRepo;
        }

        //
        // GET: /Accounting/RptBalanceGenerale/
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
                           RptBalanceGeneraleID = c.RptBalanceGeneraleID,
                           Agence = c.Agence,
                           LibAgence = c.LibAgence,
                           Devise = c.Devise,
                           LibDevise = c.LibDevise,
                           Compte = c.Compte,
                           Libelle = c.Libelle,
                           SoldeInitDb = c.SoldeInitDb,
                           SoldeInitCr = c.SoldeInitCr,
                           DebitMvt = c.DebitMvt,
                           CreditMvt = c.CreditMvt,
                           SoldeFinDb = c.SoldeFinDb,
                           SoldeFinCr =c.SoldeFinCr
                       }
            };

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        private List<RptBalanceGenerale> ModelAcctOp(int BranchID, int DeviseID, DateTime Bdate, DateTime Edate) 
        {
            List<RptBalanceGenerale> list = new List<RptBalanceGenerale>();
            try
            {
                if (Bdate.Date.Year>1900 && Edate.Date.Year>1900)
                {

                
               
                Branch br = (from b in db.Branches where b.BranchID == BranchID select b).FirstOrDefault();
                Devise dev = (from d in db.Devises where d.DeviseID == DeviseID select d).FirstOrDefault();

                    double  SoldeInitDb = 0d,
                            SoldeInitCr = 0d,
                            DebitMvt = 0d,
                            CreditMvt = 0d,
                            SoldeFinDb = 0d,
                            SoldeFinCr = 0d;

                var listDistAcc = db.AccountOperations.Where(ao => ao.BranchID == BranchID && ao.DeviseID == DeviseID)
                .Select(a => new
                {
                    Account = a.Account
                }).Distinct().ToList().OrderBy(c=>c.Account.AccountNumber);

                foreach (var c in listDistAcc)
                {


                        /*List<AccountOperation> listAccDeb = (from ao in db.AccountOperations
                                                             where ao.BranchID == BranchID && ao.DeviseID == DeviseID && (ao.DateOperation < Bdate) && ao.AccountID == c.Account.AccountID
                                                             select ao).ToList();
                        List<AccountOperation> listAccMvt = (from ao in db.AccountOperations
                                                             where ao.BranchID == BranchID && ao.DeviseID == DeviseID && (ao.DateOperation >= Bdate && ao.DateOperation <= Edate) && ao.AccountID == c.Account.AccountID
                                                             select ao).ToList();
                        List<AccountOperation> listAccfin = (from ao in db.AccountOperations
                                                             where ao.BranchID == BranchID && ao.DeviseID == DeviseID && (ao.DateOperation <= Edate) && ao.AccountID == c.Account.AccountID
                                                             select ao).ToList();

                        SoldeInitDb = listAccDeb.Select(s => s.Debit - s.Credit).Sum() > 0 ? listAccDeb.Select(s => s.Debit - s.Credit).Sum() : 0;
                        SoldeInitCr = listAccDeb.Select(s => s.Credit - s.Debit).Sum() > 0 ? listAccDeb.Select(s => s.Credit - s.Debit).Sum() : 0;
                        DebitMvt = listAccMvt.Select(s => s.Debit).Sum();
                        CreditMvt = listAccMvt.Select(s => s.Credit).Sum();
                        SoldeFinDb = listAccfin.Select(s => s.Debit - s.Credit).Sum() > 0 ? listAccfin.Select(s => s.Debit - s.Credit).Sum() : 0;
                        SoldeFinCr = listAccfin.Select(s => s.Credit - s.Debit).Sum() > 0 ? listAccfin.Select(s => s.Credit - s.Debit).Sum() : 0;*/

                        List<AccountOperation> listAcc = (from ao in db.AccountOperations
                                                             where ao.BranchID == BranchID && ao.DeviseID == DeviseID && (ao.DateOperation <= Edate) && ao.AccountID == c.Account.AccountID
                                                             select ao).ToList();

                        SoldeInitDb = listAcc.Where(ao=> ao.DateOperation < Bdate).Select(s => s.Debit - s.Credit).Sum() > 0 ? listAcc.Select(s => s.Debit - s.Credit).Sum() : 0;
                        SoldeInitCr = listAcc.Where(ao => ao.DateOperation < Bdate).Select(s => s.Credit - s.Debit).Sum() > 0 ? listAcc.Select(s => s.Credit - s.Debit).Sum() : 0;
                        DebitMvt = listAcc.Where(ao => (ao.DateOperation >= Bdate && ao.DateOperation <= Edate)).Select(s => s.Debit).Sum();
                        CreditMvt = listAcc.Where(ao => (ao.DateOperation >= Bdate && ao.DateOperation <= Edate)).Select(s => s.Credit).Sum();
                        SoldeFinDb = listAcc.Where(ao => ao.DateOperation <= Edate).Select(s => s.Debit - s.Credit).Sum() > 0 ? listAcc.Select(s => s.Debit - s.Credit).Sum() : 0;
                        SoldeFinCr = listAcc.Where(ao => ao.DateOperation <= Edate).Select(s => s.Credit - s.Debit).Sum() > 0 ? listAcc.Select(s => s.Credit - s.Debit).Sum() : 0;

                        if (SoldeInitDb > 0 || SoldeInitCr > 0 || DebitMvt>0 || CreditMvt > 0 || SoldeFinDb > 0 || SoldeFinCr > 0)
                        { 
                        list.Add(
                                new RptBalanceGenerale
                                {
                                    RptBalanceGeneraleID = 1,
                                    Agence = br.BranchCode,// listAccfin.Select(s => s.BranchID),
                                    LibAgence =br.BranchName,// listAccfin.Select(s => s.UIBranchCode),
                                    Devise = dev.DeviseCode,// listAccfin.Select(s => s.DeviseID),
                                    LibDevise =dev.DeviseLabel,// listAccfin.Select(s => s.UIDeviseCode),
                                    Compte = c.Account.AccountNumber.ToString(),//listAccfin.Select (s=>s.UIAccountNumber),
                                    Libelle = c.Account.AccountLabel,// listAccfin.Select(s => s.Account.AccountLabel),
                                    SoldeInitDb = SoldeInitDb,//,
                                    SoldeInitCr = SoldeInitCr,//,
                                    DebitMvt = DebitMvt,//listAccMvt.Select(s => s.Debit).Sum(),// > 0 ? listAccMvt.Select(s => s.Debit - s.Credit).Sum() : 0,
                                    CreditMvt = CreditMvt,//listAccMvt.Select(s => s.Credit).Sum(),// > 0 ? listAccMvt.Select(s => s.Credit - s.Debit).Sum() : 0,
                                    SoldeFinDb = SoldeFinDb,//,
                                    SoldeFinCr = SoldeFinCr,//,
                                }
                            );
                        }
                    
                    }
                }
            }
            catch (Exception e)
            {
                Response.Write(e.ToString());
            }
            return list;

        }

       
    }
}