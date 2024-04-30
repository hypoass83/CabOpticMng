using FatSod.DataContext.Initializer;
using FatSod.Security.Abstracts;
using FatSod.Security.Entities;
using FatSod.Supply.Entities;
using CABOPMANAGEMENT.Controllers;
using CABOPMANAGEMENT.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using FatSod.Supply.Abstracts;
using FatSod.Ressources;
using CABOPMANAGEMENT.Filters;
using FatSod.Budget.Abstracts;
using FatSod.Budget.Entities;

namespace CABOPMANAGEMENT.Areas.CashRegister.Controllers
{
    [Authorize(Order = 1)]
    [TakeBusinessDay(Order = 2)]
    public class BudgetExpenseController : BaseController
    {
        //Current Controller and current page
        private const string CONTROLLER_NAME = "CashRegister/BudgetExpense";

        //*********************

        private ITillDay _tillDayRepository;
        private IBusinessDay _busDayRepo;
        private IBudgetConsumption _budgetConsumptionRepository;
        
        public BudgetExpenseController(
            ITillDay tillDayRepository,
            IBudgetConsumption budgetConsumptionRepository,
            IBusinessDay busDayRepo
            )
        {
            this._tillDayRepository = tillDayRepository;
            this._budgetConsumptionRepository = budgetConsumptionRepository;
            this._busDayRepo = busDayRepo;
        }
        
       [OutputCache(Duration = 3600)] 
        public ActionResult Index()
        {
            ViewBag.DisplayForm = 1;
            try
            {

                //we ensure that if this user manage cash register. If he manage it, will verify if till is closed else, we ask he to closed it before login off
                UserTill userTill = (from td in db.UserTills
                            where td.UserID == SessionGlobalPersonID
                            select td).SingleOrDefault();
                if (userTill == null || userTill.TillID <= 0)
                {
                        TempData["Message"] = "Access Denied - You can't do sale operation. If you think no, try again or<br/>contact our administrator for this purpose<code/>.";
                        ViewBag.DisplayForm = 0;
                        return this.View();
                    }
                List<BusinessDay> UserBusDays = (List<BusinessDay>)Session["UserBusDays"];
                DateTime currentDateOp = UserBusDays.FirstOrDefault().BDDateOperation;
                ViewBag.CurrentBranch = UserBusDays.FirstOrDefault().BranchID;
                ViewBag.BusnessDayDate = currentDateOp.ToString("yyyy-MM-dd");


                TillDayStatus tState = _tillDayRepository.TillDayStatus(userTill.TillID);
                if (tState == null)
                {
                    TempData["Message"] = "Error - Bad Configuration of Cash Register!!! Please call Your database Administrator";
                    ViewBag.DisplayForm = 0;
                    return this.View();
                }
                if (!tState.IsOpen)
                {
                    TempData["Message"] = "Error - This Cash Register is Still Close!!! Please Open It Before Proceed";
                    ViewBag.DisplayForm = 0;
                    return this.View();
                }

                TillDay currentTillDay = (from t in db.TillDays
                                          where
                                              t.TillID == userTill.TillID && t.TillDayDate == tState.TillDayLastOpenDate.Date && t.IsOpen // t.TillDayDate.Day == currentDateOp.Day && t.TillDayDate.Month == currentDateOp.Month && t.TillDayDate.Year == currentDateOp.Year && t.IsOpen
                                          select t).FirstOrDefault();
                if (currentTillDay == null)
                {
                    TempData["Message"] = "Warnnig - Cash register is closed. You must open it before do any sale<br/>Go at Cash Register module=>Open cash register<code/>";
                    ViewBag.DisplayForm = 0;
                    return this.View();
                }


                ViewBag.CurrentTill = userTill.TillID;
                int deviseID = (Session["DefaultDeviseID"] == null) ? 0 : (int)Session["DefaultDeviseID"];
                if (deviseID <= 0)
                {
                    InjectUserConfigInSession();
                }
                deviseID = (Session["DefaultDeviseID"] == null) ? 0 : (int)Session["DefaultDeviseID"];
                ViewBag.DefaultDeviseID = deviseID;
                ViewBag.DefaultDevise = (deviseID <= 0) ? "" : db.Devises.Find(deviseID).DeviseCode;
                return View(GetModel());
            }

            catch (Exception e)
            {
                TempData["Message"] = "Error "+ e.Message;
                ViewBag.DisplayForm = 0;
                return this.View();
            }
        }
        public JsonResult GetModel()
        {
            var model = new
            {
                data = from c in LoadComponent.AllExpenseBudgToValidate.OrderBy(c=>c.DateOperation) select
                new
                {
                    //BeneficiaryName = c.BeneficiaryName,
                    //BudgetAllocatedID = c.BudgetAllocatedID,
                    BudgetConsumptionID = c.BudgetConsumptionID,
                    UIBudgetAllocated = c.UIBudgetAllocated,
                    DateOperation = c.DateOperation.ToString("yyyy-MM-dd"),
                    //Justification = c.Justification,
                    //PaymentMethodID = c.PaymentMethodID,
                    Reference = c.Reference,
                    VoucherAmount = c.VoucherAmount
                }
            };
            return Json(model, JsonRequestBehavior.AllowGet);
        }

       
        public ActionResult InitializeCommandFields(int ID)
        {
            List<object> _InfoList = new List<object>();
            try
            {
                Devise devise = db.Devises.Where(d => d.DefaultDevise).FirstOrDefault();
                BudgetConsumption bdConsume = db.BudgetConsumptions.Find(ID);
                _InfoList.Add(new
                {
                    BudgetConsumptionID= bdConsume.BudgetConsumptionID,
                    BudgetAllocatedID = bdConsume.BudgetAllocatedID,
                    BudgetAllocatedName = bdConsume.BudgetAllocated.BudgetLine.BudgetLineLabel,
                    DateOperation = bdConsume.DateOperation.ToString("yyyy-MM-dd"),
                    Reference = bdConsume.Reference,
                    BeneficiaryName = bdConsume.BeneficiaryName,
                    Justification = bdConsume.Justification,
                    BuyType = CodeValue.Accounting.DefaultCodeAccountingSection.CODECAIS,
                    VoucherAmount = bdConsume.VoucherAmount,
                    DeviseID = devise.DeviseID,
                    DeviseCode= devise.DeviseCode
                });
                
                return Json(_InfoList, JsonRequestBehavior.AllowGet);


            }
            catch (Exception e)
            {
                TempData["Message"] = "Error " + e.Message;
                return Json(_InfoList, JsonRequestBehavior.AllowGet);
            }
        }

        //validate
        //[HttpPost]
        public JsonResult BudgetExpenseValidate(BudgetConsumption budgetConsumption, string BuyType)
        {
            bool status = false;
            string Message = "";
            try
            {
                UserTill userTill = db.UserTills.FirstOrDefault(td => td.UserID == SessionGlobalPersonID);
                
                //choix de la caisse
                if (BuyType == CodeValue.Accounting.DefaultCodeAccountingSection.CODECAIS)
                {
                    budgetConsumption.PaymentMethodID = userTill.TillID;
                    bool isTellerControl = (Session["isTellerControl"] == null) ? false  : (bool)Session["isTellerControl"];
                    if (Session["isTellerControl"] == null)
                    {
                        InjectUserConfigInSession();
                    }
                    isTellerControl = (Session["isTellerControl"] == null) ? false : (bool)Session["isTellerControl"];
                    if (isTellerControl)
                    {
                        double tillBalance = _tillDayRepository.TillStatus(userTill.TillID).Ballance;

                        //ne faites ps d'achat en espèce si : 1 - pas d'argent en caisse; 2- Facture > Montant en caisse
                        if (tillBalance <= 0 || budgetConsumption.VoucherAmount > tillBalance)
                        {

                            Message = "NO Cash Availlable" +
                                "Sorry, you can not proceed this cash opérations because you do have sufficient liquidities in your till. Please contact an administrator";
                            status = false;
                            return new JsonResult { Data = new { status = status, Message = Message } };
                        }
                    }

                    Branch currentBranch = db.Branches.Find(db.Tills.Find(userTill.TillID).BranchID);

                    budgetConsumption.ValidateByID = SessionGlobalPersonID;
                    budgetConsumption.PaymentDate = SessionBusinessDay(currentBranch.BranchID).BDDateOperation;// businessDay.BDDateOperation;
                    bool res = _budgetConsumptionRepository.SavebudgetConsumption(budgetConsumption, SessionGlobalPersonID, currentBranch.BranchID);

                    status = res;
                    Message = (res) ? Resources.Success + " - " + Resources.AddBudConsume : "Error while Save Budget Consumption";
                }
                else
                {
                    status = false;
                    Message = "Error Please use teller account to validate this transaction ";
                }

            }
            catch (Exception e)
            {
                status = false;
                Message = "Error " + e.Message + " " + e.StackTrace + " " + e.InnerException;
            }
            return new JsonResult { Data = new { status = status, Message = Message } };
        }
        

        public JsonResult GetAllAuthBudgetConsumtion()
        {
            List<BudgetConsumption> listBudConsume =db.BudgetConsumptions.Where(b => !b.isValidated).OrderBy(a => a.Reference).ToList();
            var list = new
            {
                data = from c in listBudConsume.OrderBy(c=>c.DateOperation)
                    select
                    new
                    {
                        BudgetAllocatedID = c.BudgetAllocatedID,
                        UIBudgetAllocated = c.UIBudgetAllocated,
                        PaymentMethodID = c.PaymentMethodID,
                        VoucherAmount = c.VoucherAmount,
                        DateOperation = c.DateOperation.ToString("yyyy-MM-dd"),
                        Reference = c.Reference,
                        BeneficiaryName = c.BeneficiaryName,
                        Justification = c.Justification,
                        BudgetConsumptionID = c.BudgetConsumptionID
                    }
                };
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        

    }
}