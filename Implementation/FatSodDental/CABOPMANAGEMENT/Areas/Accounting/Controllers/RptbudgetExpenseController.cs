using FatSod.Budget.Entities;
using FatSod.Security.Abstracts;
using FatSod.Security.Entities;
using CABOPMANAGEMENT.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace CABOPMANAGEMENT.Areas.Accounting.Controllers
{
    public class RptbudgetExpenseController : BaseController
    {
        //Current Controller and current page
        private const string CONTROLLER_NAME = "Accounting/RptbudgetExpense";
        private const string VIEW_NAME = "Index";
        //person repository

        private IBusinessDay _busDayRepo;


        private Company cmpny;

        List<BusinessDay> bdDay;

        // GET: CashRegister/State
        public RptbudgetExpenseController(
            IBusinessDay busDayRepo
            )
        {
            this._busDayRepo = busDayRepo;
        }
        //
        // GET: /Sale/RptbudgetExpense/

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

        public JsonResult LoadBudgetAllocateds(string filter)
        {

            List<object> budgetList = new List<object>();
            foreach (BudgetAllocated dL in db.BudgetAllocateds.Where(q => q.FiscalYear.FiscalYearStatus && q.BudgetLine.BudgetLineLabel.StartsWith(filter.ToLower())).ToArray().OrderBy(c => c.BudgetLine.BudgetLineLabel))
            {
                string itemLabel = "";

                itemLabel = dL.BudgetLine.BudgetLineLabel;

                budgetList.Add(new
                {
                    Name = itemLabel,
                    ID = dL.BudgetAllocatedID
                });
            }

            return Json(budgetList, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetList(int BranchID, int DeviseID, int BudgetAllocatedID, DateTime BeginDate, DateTime EndDate)
        {
            var model = new
            {
                data = from c in ModelExpenseOp(BranchID, DeviseID, BudgetAllocatedID, BeginDate, EndDate)
                select new
                {
                    BudgetAllocatedID = c.BudgetAllocatedID,
                    UIBudgetAllocated = c.BudgetAllocated.BudgetLine.BudgetLineLabel,
                    PaymentMethodId = c.PaymentMethodID,
                    VoucherAmount = c.VoucherAmount.ToString("0,0"),
                    DateOperation = c.DateOperation.ToString("yyyy-MM-dd"),
                    Reference = c.Reference,
                    BeneficiaryName = c.BeneficiaryName,
                    Justification = c.Justification,
                    BudgetConsumptionID = c.BudgetConsumptionID,
                    UIBranchCode = "",
                    UIDeviseCode = "",
                    PaymentDate = c.PaymentDate.Value.ToString("yyyy-MM-dd")  ,
                    ValidateBy = (c.ValidateBy == null) ? "" : c.ValidateBy.Name,
                    AutorizeBy = (c.AutorizeBy == null) ? "" : c.AutorizeBy.Name
                }
            };

            return Json(model, JsonRequestBehavior.AllowGet);
            //var jsonResult = Json(model, JsonRequestBehavior.AllowGet);
            //jsonResult.MaxJsonLength = int.MaxValue;
            //return jsonResult;
        }

        public JsonResult chargeSolde(int BranchID, int DeviseID, int BudgetAllocatedID,DateTime BeginDate, DateTime EndDate)
        {
            List<object> _InfoList = new List<object>();
            double TotalDebit = 0d;
            
            if (BranchID <= 0 || DeviseID <= 0)
            {
                TotalDebit = 0d;
            }

            if (BudgetAllocatedID > 0)
            {
                var lstExpenseOperations = from bc in db.BudgetConsumptions
                where (bc.isValidated && bc.BudgetAllocated.BranchID == BranchID && bc.DeviseID == DeviseID && (bc.DateOperation >= BeginDate && bc.DateOperation <= EndDate) && bc.BudgetAllocatedID == BudgetAllocatedID)
                group bc by new { bc.PaymentDate } into g
                select new
                {
                    key = g.Key,
                    VoucherAmount = g.Sum(a => a.VoucherAmount)
                };
                foreach (var exp in lstExpenseOperations.ToList())
                {
                    TotalDebit = TotalDebit + exp.VoucherAmount;
                }
            }
            else
            {
                var lstExpenseOperations = from bc in db.BudgetConsumptions
                    where (bc.isValidated && bc.BudgetAllocated.BranchID == BranchID && bc.DeviseID == DeviseID && (bc.DateOperation >= BeginDate && bc.DateOperation <= EndDate))
                    group bc by new { bc.PaymentDate } into g
                    select new
                    {
                        key = g.Key,
                        VoucherAmount = g.Sum(a => a.VoucherAmount)
                    };
                foreach (var exp in lstExpenseOperations.ToList())
                {
                    TotalDebit = TotalDebit + exp.VoucherAmount;
                }
            }

            _InfoList.Add(new
            {
                TotalDebit = TotalDebit.ToString("0,0")
            });
            return Json(_InfoList, JsonRequestBehavior.AllowGet);
            
        }
        private List<BudgetConsumption> ModelExpenseOp(int BranchID, int DeviseID, int BudgetAllocatedID, DateTime BeginDate, DateTime EndDate)
        {
            
            List<BudgetConsumption> list = new List<BudgetConsumption>();
            List<BudgetConsumption> listBudConsume = new List<BudgetConsumption>();

            if (BranchID <= 0 || DeviseID <= 0)
            {
                return list;
            }
            string UIBranchCode = db.Branches.Find(BranchID).BranchCode;
            string UIDeviseCode = db.Devises.Find(DeviseID).DeviseCode;

            if (BudgetAllocatedID>0)
            {
                listBudConsume = db.BudgetConsumptions.Where(b => b.isValidated && b.BudgetAllocated.BranchID == BranchID && b.DeviseID == DeviseID && (b.DateOperation >= BeginDate && b.DateOperation <= EndDate) && b.BudgetAllocatedID == BudgetAllocatedID).OrderBy(a => a.DateOperation).ToList();
            }
            else
            {
                listBudConsume = db.BudgetConsumptions.Where(b => b.isValidated && b.BudgetAllocated.BranchID == BranchID && b.DeviseID == DeviseID && (b.DateOperation >= BeginDate && b.DateOperation <= EndDate)).OrderBy(a => a.DateOperation).ToList();
            }
            listBudConsume.ForEach(c =>
            {
                list.Add(
                        new BudgetConsumption
                        {
                            BudgetAllocatedID = c.BudgetAllocatedID,
                            BudgetAllocated = c.BudgetAllocated,
                            PaymentMethodID = c.PaymentMethodID.Value,
                            VoucherAmount = c.VoucherAmount,
                            DateOperation = c.DateOperation,
                            Reference = c.Reference,
                            BeneficiaryName = c.BeneficiaryName,
                            Justification = c.Justification,
                            BudgetConsumptionID = c.BudgetConsumptionID,
                            /*UIBranchCode = UIBranchCode,
                            UIDeviseCode = UIDeviseCode,*/
                            PaymentDate = c.PaymentDate  ,
                            ValidateBy=c.ValidateBy,
                            AutorizeBy =  c.AutorizeBy
                        }
                );
            });
            return list;

        }
        
        /*public void ShowGenericRpt()
        {
            ReportDocument rptH = new ReportDocument();
            try
            {
                bool isValid = true;

                string strReportName = Session["ReportName"].ToString();    // Setting ReportName
                string stCompanyName1 = Session["CompanyName"].ToString();     // Setting CompanyName1
                string strTelFax1 = Session["TelFax"].ToString();         // Setting TelFax1
                string strRepTitle1 = Session["RepTitle"].ToString();         // Setting RepTitle1
                string strOperator1 = Session["Operator"].ToString();         // Setting Operator1
                string strRegionCountry1 = Session["RegionCountry"].ToString();
                string strAdresse1 = Session["Adresse"].ToString();
                DateTime BeginDate = (DateTime)Session["BeginDate"];
                DateTime EndDate = (DateTime)Session["EndDate"];

                var rptSource = Session["rptSource"];

                if (string.IsNullOrEmpty(strReportName) && rptSource == null)
                {
                    isValid = false;
                }

                if (isValid)
                {
                    
                    string strRptPath = System.Web.HttpContext.Current.Server.MapPath("~/") + "Reports//Accounting//" + strReportName + ".rpt";
                    rptH.Load(strRptPath);
                    if (rptSource != null && rptSource.GetType().ToString() != "System.String") rptH.SetDataSource(rptSource);
                    if (!string.IsNullOrEmpty(stCompanyName1)) rptH.SetParameterValue("BranchName", stCompanyName1);
                    if (!string.IsNullOrEmpty(strTelFax1)) rptH.SetParameterValue("BranchInfo", "Region "+strRegionCountry1 + " "+ strAdresse1); //+ strTelFax1);
                    if (!string.IsNullOrEmpty(strRepTitle1)) rptH.SetParameterValue("RepTitle", strRepTitle1);
                    if (!string.IsNullOrEmpty(strOperator1)) rptH.SetParameterValue("Operator", strOperator1);

              
                    if (BeginDate != null) rptH.SetParameterValue("BeginDate", BeginDate);
                    if (EndDate != null) rptH.SetParameterValue("EndDate", EndDate);
                    bool isDownloadRpt = (bool)Session["isDownloadRpt"];
                    rptH.ExportToHttpResponse(ExportFormatType.PortableDocFormat, System.Web.HttpContext.Current.Response, false, strReportName);

                    // Clear all sessions value
                    Session["ReportName"] = null;
                    Session["CompanyName"] = null;
                    Session["TelFax"] = null;
                    Session["RepTitle"] = null;
                    Session["Operator"] = null;
                    Session["accop"] = null;
                    Session["RegionCountry"] = null;
                    Session["Adresse"] = null;
                    Session["BeginDate"] = null;
                    Session["EndDate"] = null;
                }
                else
                {
                    Response.Write("Nothing Found; No Report name found");
                }
            }
            catch (Exception ex)
            {
                Response.Write(ex.ToString());
            }
            finally
            {
                rptH.Close();
                rptH.Dispose();
            }
        }*/

      

        /// This is used for showing Generic Report(with data and report parameter) in a same window       
        /*public ActionResult ShowGeneric()
        {
            // Clear all sessions value
            Session["ReportName"] = null;
            Session["CompanyName"] = null;
            Session["TelFax"] = null;

            Session["RegionCountry"] = null;
            Session["Adresse"] = null;

            Session["RepTitle"] = null;
            Session["Operator"] = null;

            cmpny = db.Companies.FirstOrDefault();
            this.Session["ReportName"] = "RptbudgetExpense";
            if (cmpny != null)
            {
                this.Session["CompanyName"] = cmpny.Name;
                this.Session["TelFax"] = "Tel: " + cmpny.Adress.AdressPhoneNumber + " Fax : " + cmpny.Adress.AdressFax + " Town :" + cmpny.Adress.Quarter.Town.TownLabel;
                this.Session["RegionCountry"] = cmpny.Adress.Quarter.Town.Region.RegionLabel;
                this.Session["Adresse"] = "PO BOX:" + cmpny.Adress.AdressPOBox + " " + cmpny.Adress.Quarter.Town.TownLabel;
            }
            else
            {
                this.Session["CompanyName"] = "NONE";
                this.Session["TelFax"] = "NONE";
            }
            this.Session["RepTitle"] = Resources.RptbudgetExpenseTitle;
            this.Session["Operator"] = CurrentUser.Name;
            this.Session["rptSource"] = ModelExpenseOp();
            
            return RedirectToAction("ShowGenericRpt", "RptbudgetExpense");
        }*/
    }
}