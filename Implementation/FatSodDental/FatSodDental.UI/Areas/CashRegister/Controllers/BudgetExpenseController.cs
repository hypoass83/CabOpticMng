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
using FatSod.Supply.Abstracts;
using FatSod.Ressources;
using FatSodDental.UI.Filters;
using FastSod.Utilities.Util;
using FatSod.Budget.Abstracts;
using FatSod.Budget.Entities;
using FatSod.DataContext.Concrete;
using System.Web.UI;

namespace FatSodDental.UI.Areas.CashRegister.Controllers
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
            try
            {

           
            //we ensure that if this user manage cash register. If he manage it, will verify if till is closed else, we ask he to closed it before login off
                UserTill userTill = (from td in db.UserTills
                            where td.UserID == SessionGlobalPersonID
                            select td).SingleOrDefault();
            if (userTill == null || userTill.TillID <= 0)
            {
                X.Msg.Alert("Access Denied", "You can't do sale operation. If you think no, try again or<br/>contact our administrator for this purpose<code/>.").Show();
                return this.Direct();
            }
            List<BusinessDay> UserBusDays = (List<BusinessDay>)Session["UserBusDays"];
            DateTime currentDateOp = UserBusDays.FirstOrDefault().BDDateOperation;
            ViewBag.CurrentBranch = UserBusDays.FirstOrDefault().BranchID;
            ViewBag.BusnessDayDate = UserBusDays.FirstOrDefault().BDDateOperation;// businessDay.BDDateOperation;

            TillDayStatus tState = _tillDayRepository.TillDayStatus(userTill.Till);
            if (tState == null)
            {
                X.Msg.Alert("Error", "Bad Configuration of Cash Register!!! Please call Your database Administrator").Show();
                return this.Direct();
            }
            if (!tState.IsOpen)
            {
                X.Msg.Alert("Error", "This Cash Register is Still Close!!! Please Open It Before Proceed").Show();
                return this.Direct();
            }

            TillDay currentTillDay = (from t in db.TillDays
                                      where
                                          t.TillID == userTill.TillID && t.TillDayDate == tState.TillDayLastOpenDate.Date && t.IsOpen // t.TillDayDate.Day == currentDateOp.Day && t.TillDayDate.Month == currentDateOp.Month && t.TillDayDate.Year == currentDateOp.Year && t.IsOpen
                                      select t).FirstOrDefault(); 
            if (currentTillDay == null)
            {
                X.Msg.Alert("Warnnig", "Cash register is closed. You must open it before do any sale<br/>Go at Cash Register module=>Open cash register<code/>").Show();
                return this.Direct();
            }
            
            ViewBag.CurrentTill = currentTillDay.TillID;
            return View(Model());
            }

            catch (Exception e)
            {
                X.Msg.Alert("Error ", e.Message).Show();
                return this.Direct();
            }
        }
        public List<object> Model()
        {
            List<object> model = new List<object>();
            LoadComponent.AllExpenseBudgToValidate.ForEach(c =>
            {
                model.Add(
                        new
                        {
                            BeneficiaryName = c.BeneficiaryName,
                            BudgetAllocatedID = c.BudgetAllocatedID,
                            BudgetConsumptionID = c.BudgetConsumptionID,
                            UIBudgetAllocated = c.UIBudgetAllocated,
                            DateOperation = c.DateOperation,
                            Justification = c.Justification,
                            PaymentMethodID = c.PaymentMethodID,
                            Reference = c.Reference,
                            VoucherAmount = c.VoucherAmount
                        }
                    );
            });
            return model;
        }

        [HttpPost]
        public StoreResult GetAllOderBudgetConsume()
        {
            return this.Store(Model());
        }

        public ActionResult InitializeCommandFields(int ID)
        {
            try
            {
                
                BudgetConsumption bdConsume = db.BudgetConsumptions.Find(ID);
                this.GetCmp<TextField>("BudgetConsumptionID").Value = bdConsume.BudgetConsumptionID;
                this.GetCmp<TextField>("BudgetAllocatedID").Value = bdConsume.BudgetAllocatedID;
                this.GetCmp<TextField>("BudgetAllocatedName").Value = bdConsume.BudgetAllocated.BudgetLine.BudgetLineLabel;
                this.GetCmp<DateField>("DateOperation").Value = bdConsume.DateOperation;
                this.GetCmp<TextField>("Reference").Value = bdConsume.Reference;
                this.GetCmp<TextField>("BeneficiaryName").Value = bdConsume.BeneficiaryName;
                this.GetCmp<TextArea>("Justification").Value = bdConsume.Justification;
                this.GetCmp<ComboBox>("BuyType").Value = CodeValue.Accounting.DefaultCodeAccountingSection.CODECAIS;//bdConsume.PaymentMethodID;
                this.GetCmp<TextField>("VoucherAmount").Value = bdConsume.VoucherAmount;

                this.GetCmp<ComboBox>("DeviseID").Value = db.Devises.Where(d=>d.DefaultDevise).FirstOrDefault().DeviseID;
                //this.GetCmp<ComboBox>("PaymentMethod").Value = bdConsume.;
                return this.Direct();
            }
            catch (Exception e) { X.Msg.Alert("Error ", e.Message).Show(); return this.Direct(); }
        }

        //validate
        [HttpPost]
        public ActionResult BudgetExpenseValidate(BudgetConsumption budgetConsumption, string BuyType)
        {
            try
            {
                UserTill userTill = db.UserTills.FirstOrDefault(td => td.UserID == SessionGlobalPersonID);
                
                //choix de la caisse
                if (BuyType == CodeValue.Accounting.DefaultCodeAccountingSection.CODECAIS)
                { 
                    budgetConsumption.PaymentMethodID = userTill.TillID;
                    double tillBalance = _tillDayRepository.TillStatus(userTill.Till).Ballance;

                    //ne faites ps d'achat en espèce si : 1 - pas d'argent en caisse; 2- Facture > Montant en caisse
                    if (tillBalance <= 0 || budgetConsumption.VoucherAmount > tillBalance)
                    {

                        X.Msg.Alert("NO Cash Availlable",
                            "Sorry, you can not proceed this cash opérations because you do have sufficient liquidities in your till. Please contact an administrator").Show();
                        return this.Direct();
                    }
                }
                
                Branch currentBranch = db.Branches.Find(db.Tills.Find(userTill.TillID).BranchID);
                //BusinessDay businessDay = _busDayRepo.GetOpenedBusinessDay(currentBranch);
                budgetConsumption.ValidateByID = SessionGlobalPersonID;
                budgetConsumption.PaymentDate = SessionBusinessDay(currentBranch.BranchID).BDDateOperation;// businessDay.BDDateOperation;
                bool res = _budgetConsumptionRepository.SavebudgetConsumption(budgetConsumption,SessionGlobalPersonID,currentBranch.BranchID);
                this.BCReset();
                return this.Direct();
            }
            catch (Exception e) { X.Msg.Alert("Error ", e.Message + " " + e.StackTrace + " " + e.InnerException).Show(); return this.Direct(); }
        }

        public void BCReset()
        {
            this.GetCmp<Store>("BudgetExpenseListStore").Reload();
            this.GetCmp<FormPanel>("GlobalBudgetExpenseForm").Reset();
            this.GetCmp<FormPanel>("FormBudgetExpenseId").Reset();

            this.AlertSucces(Resources.Success, Resources.AddBudConsume);
        }

        public List<object> ModelAuth()
        {
            List<BudgetConsumption> listBudConsume =db.BudgetConsumptions.Where(b => !b.isValidated).OrderBy(a => a.Reference).ToList();
            List<object> list = new List<object>();
            listBudConsume.ForEach(c =>
                {
                    list.Add(
                                new
                                {
                                    BudgetAllocatedID = c.BudgetAllocatedID,
                                    UIBudgetAllocated = c.UIBudgetAllocated,
                                    PaymentMethodID = c.PaymentMethodID,
                                    VoucherAmount = c.VoucherAmount,
                                    DateOperation = c.DateOperation,
                                    Reference = c.Reference,
                                    BeneficiaryName = c.BeneficiaryName,
                                    Justification = c.Justification,
                                    BudgetConsumptionID = c.BudgetConsumptionID
                                }
                            );
                });
            return list;
        }
        public ActionResult GetAllAuthBudgetConsumtion()
        {
            return this.Store(ModelAuth());
        }

    }
}