using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FatSod.Supply.Entities;
using FatSod.Supply.Abstracts;
using FatSod.Security.Abstracts;
using FatSod.Security.Entities;
using Ext.Net;
using Ext.Net.MVC;
using FatSodDental.UI.Tools;
using FatSodDental.UI.Controllers;
using FatSod.DataContext.Initializer;
using FatSod.Ressources;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using FatSodDental.UI.Filters;
using FatSod.Budget.Entities;
using FatSod.Budget.Abstracts;
using System.Web.UI;
using FatSod.DataContext.Concrete;

namespace FatSodDental.UI.Areas.Accounting.Controllers
{
    [Authorize(Order = 1)]
    [TakeBusinessDay(Order = 2)]
    public class AuthBudgetConsumtionController : BaseController
    {
        private IBudgetConsumption _budgetConsumptionRepository;
        private IBusinessDay _busDayRepo;
        private ITransactNumber _transactNumbeRepository;
        //Current Controller and current page
        private const string CONTROLLER_NAME = "Accounting/AuthBudgetConsumtion";

        private List<BusinessDay> listBDUser;

        public AuthBudgetConsumtionController(
            ITransactNumber transactNumbeRepository,
            IBusinessDay busDayRepo,
            IBudgetConsumption budgetConsumptionRepository
            )
        {
            this._budgetConsumptionRepository = budgetConsumptionRepository;
            this._busDayRepo = busDayRepo;
            this._transactNumbeRepository = transactNumbeRepository;
            
        }

        //
        // GET: /Accounting/AuthBudgetConsumtion/
        
        [OutputCache(Duration = 3600)] 
        public ActionResult Index()
        {
            //Ext.Net.MVC.PartialViewResult rPVResult = new Ext.Net.MVC.PartialViewResult
            //{
            //    ClearContainer = true,
            //    //RenderMode = RenderMode.AddTo,
            //    ContainerId = "Body",
            //    //WrapByScriptTag = false,
            //    Model = Model()
            //};
            //SessionParameters();
            //Session["Curent_Page"] = "Index";
            //if (!LoadAction.isAutoriseToGetMenu(SessionProfileID, CodeValue.Accounting.BudgetConsume.CODEAUTHBUDCONSUME, db))
            //{
            //    RedirectToAction("NotAuthorized", "User");
            //}
            
            ViewBag.Disabled = true;
            listBDUser = (List<BusinessDay>)Session["UserBusDays"]; 
            if (listBDUser.Count() > 1)
            {
                ViewBag.Disabled = false;
            }
            BusinessDay busDays = listBDUser.FirstOrDefault();
            ViewBag.BusnessDayDate = busDays.BDDateOperation;
            Session["BusnessDayDate"] = busDays.BDDateOperation;

            return View(Model());
        }
        public List<object> Model()
        {
            List<BudgetConsumption> listBudConsume = db.BudgetConsumptions.Where(b => !b.isValidated).OrderBy(a => a.Reference).ToList();
            List<object> list = new List<object>();
            listBudConsume.ForEach(c =>
            {
                list.Add(
                                new
                                {
                                    BudgetAllocatedID = c.BudgetAllocatedID,
                                    UIBudgetAllocated = c.UIBudgetAllocated,
                                    //PaymentMethodId = c.PaymentMethodID,
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
            return this.Store(Model());
        }
        public DirectResult InitDate(int BranchID)
        {
            listBDUser = (List<BusinessDay>)Session["UserBusDays"];
            if (listBDUser == null)
            {
                listBDUser = _busDayRepo.GetOpenedBusinessDay(CurrentUser);
            }

            DateTime businessDay = listBDUser.FirstOrDefault(b => b.BranchID == BranchID).BDDateOperation;
            this.GetCmp<DateField>("DateOperation").Value = businessDay;
            return this.Direct();
        }
        public ActionResult InitTrnNumber(int? BranchID,int? BudgetAllocatedID)
        {
            if (BranchID > 0)
            {
                listBDUser = (List<BusinessDay>)Session["UserBusDays"];
                if (listBDUser == null)
                {
                    listBDUser = _busDayRepo.GetOpenedBusinessDay(CurrentUser);
                }

                BusinessDay businessDay = listBDUser.FirstOrDefault(b => b.BranchID == BranchID.Value);

                string trnnum = _transactNumbeRepository.displayTransactNumber("ABUC", businessDay);
                this.GetCmp<TextField>("Reference").Value = trnnum;
            }
            if (BudgetAllocatedID>0)
            {
                double crAmt = 0d;
                double dbAmt = 0d;
                //chargement des mnts du budget
                //amount allocated
                BudgetAllocated budAll = db.BudgetAllocateds.SingleOrDefault(b => b.BudgetAllocatedID == BudgetAllocatedID);
                double AllocatedAmt = (budAll != null) ? budAll.AllocateAmount : 0;
                List<BudgetAllocatedUpdate> LstbudgetAllocatedUpdate = db.BudgetAllocatedUpdates.Where(b => b.BudgetAllocatedID == BudgetAllocatedID).ToList();
                if (LstbudgetAllocatedUpdate.Count>0)
                {
                    crAmt = LstbudgetAllocatedUpdate.Where(b => b.SensImputation == "CR").Select(b => b.Amount).Sum();
                    dbAmt = LstbudgetAllocatedUpdate.Where(b => b.SensImputation == "DB").Select(b => b.Amount).Sum();
                }

                this.GetCmp<NumberField>("AmountAllocated").Value = AllocatedAmt + crAmt - dbAmt;
                //spend amount
                List<BudgetConsumption> buconsume = db.BudgetConsumptions.Where(bc => bc.BudgetAllocatedID == budAll.BudgetAllocatedID && bc.isValidated).ToList();
                double AmountSpend = (buconsume != null) ? buconsume.Select(b => b.VoucherAmount).Sum() : 0;
                this.GetCmp<NumberField>("AmountSpend").Value = AmountSpend;
                //left amount
                double leftAmount = AllocatedAmt + crAmt - dbAmt - AmountSpend;
                this.GetCmp<NumberField>("AmountLeft").Value = leftAmount;
            }
                
            return this.Direct();
        }
        public ActionResult GetBranchOpenedBusday()
        {
            List<object> model = new List<object>();
            listBDUser = (List<BusinessDay>)Session["UserBusDays"];
            if (listBDUser == null)
            {
                listBDUser = _busDayRepo.GetOpenedBusinessDay(CurrentUser);
            }

            listBDUser.ForEach(c =>
            {
                model.Add(
                        new
                        {
                            BranchID = c.BranchID,
                            BranchName = c.BranchName
                        }
                    );
            });
            return this.Store(model);
        }
        public ActionResult AddAuthBudgetConsumtion(BudgetConsumption budgetConsumption)
        {
            try
            {
                budgetConsumption.AutorizeByID = SessionGlobalPersonID;
                if (budgetConsumption.BudgetConsumptionID > 0)
                {
                    _budgetConsumptionRepository.UpdateBudgetConsumption(budgetConsumption);
                    statusOperation = Resources.CmdUpdated;
                }
                else
                {
                    _budgetConsumptionRepository.CreateBudgetConsumption(budgetConsumption);
                    statusOperation = Resources.CmdAdded;
                }
                //initialization des champs
                /*this.GetCmp<ComboBox>("BudgetAllocatedID").Clear();
                this.GetCmp<TextField>("Reference").Clear();
                this.GetCmp<TextField>("BeneficiaryName").Clear();
                this.GetCmp<TextArea>("Justification").Clear();
                //this.GetCmp<ComboBox>("MethodePayment").Clear();
                this.GetCmp<NumberField>("AmountAllocated").Value = 0;
                this.GetCmp<NumberField>("AmountSpend").Value = 0;
                this.GetCmp<NumberField>("AmountLeft").Value = 0;
                this.GetCmp<NumberField>("VoucherAmount").Value = 0;
                //this.GetCmp<NumberField>("RemainingBalance").Value = 0;
                this.GetCmp<Store>("BudgetConsumptionListStore").Reload();
                */
                this.GetCmp<FormPanel>("GlobalAuthBudgetForm").Reset();
                this.GetCmp<Store>("BudgetConsumptionListStore").Reload();
                return this.Direct(); 
            }
            catch (Exception e) 
            { 
                X.Msg.Alert("Error ", e.Message + " " + e.StackTrace + " " + e.InnerException).Show();
                return this.Direct(); 
            }
        }
        public ActionResult InitializeFieldsBudConsume(int ID)
        {
            BudgetConsumption bdConsume = db.BudgetConsumptions.Find(ID);
            this.GetCmp<TextField>("BudgetConsumptionID").Value = bdConsume.BudgetConsumptionID;
            this.GetCmp<ComboBox>("BudgetAllocatedID").Value = bdConsume.BudgetAllocatedID;
            this.GetCmp<TextField>("Reference").Value = bdConsume.Reference;
            this.GetCmp<TextField>("BeneficiaryName").Value = bdConsume.BeneficiaryName;
            this.GetCmp<TextArea>("Justification").Value = bdConsume.Justification;
            //this.GetCmp<ComboBox>("MethodePayment").Value = bdConsume.PaymentMethodID;
            //this.InitTrnNumber(0, bdConsume.BudgetAllocatedID);
            //this.GetCmp<NumberField>("AmountAllocated").Value = 0;
            //this.GetCmp<NumberField>("AmountSpend").Value = 0;
            //this.GetCmp<NumberField>("AmountLeft").Value = 0;
            this.GetCmp<NumberField>("VoucherAmount").Value = bdConsume.VoucherAmount;
            //this.GetCmp<NumberField>("RemainingBalance").Value = bdConsume.AmountSpend-;
            return this.Direct(); 
        }

        public ActionResult Reset()
        {
            this.GetCmp<FormPanel>("GlobalAuthBudgetForm").Reset();
            this.GetCmp<Store>("BudgetConsumptionListStore").Reload();
            return this.Direct();
        }

        public ActionResult DeleteBudConsume(string ID)
        {
            try
            {
                int id = Convert.ToInt32(ID);
                BudgetConsumption BudgetConsumptionToDelete = db.BudgetConsumptions.Find(id);
                db.BudgetConsumptions.Remove(BudgetConsumptionToDelete);
                db.SaveChanges();
                //_budgetConsumptionRepository.Delete(BudgetConsumptionToDelete);
                statusOperation = BudgetConsumptionToDelete.UIBudgetAllocated + " : " + Resources.AlertDeleteAction;

                this.GetCmp<FormPanel>("GlobalAuthBudgetForm").Reset();
                this.GetCmp<Store>("BudgetConsumptionListStore").Reload();
                this.AlertSucces(Resources.Success, statusOperation);
                return this.Direct();
            }
            catch (Exception e)
            {
                statusOperation = Resources.er_alert_danger + e.Message;
                X.Msg.Alert(Resources.UIBudgetAllocated, statusOperation).Show();
                return this.Direct();
            }
        }
    }
}