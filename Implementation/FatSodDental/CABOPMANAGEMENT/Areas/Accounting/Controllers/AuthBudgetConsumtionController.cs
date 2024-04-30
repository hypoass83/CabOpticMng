using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using FatSod.Supply.Abstracts;
using FatSod.Security.Abstracts;
using FatSod.Security.Entities;
using CABOPMANAGEMENT.Controllers;
using FatSod.Ressources;
using FatSod.Budget.Abstracts;
using FatSod.Budget.Entities;
using CABOPMANAGEMENT.Filters;

namespace CABOPMANAGEMENT.Areas.Accounting.Controllers
{
    [Authorize(Order = 1)]
    [TakeBusinessDay(Order = 2)]
    public class AuthBudgetConsumtionController : BaseController
    {
        private IBudgetConsumption _budgetConsumptionRepository;
        private IBusinessDay _busDayRepo;
        private ITransactNumber _transactNumbeRepository;

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

        public ActionResult Index()
        {
            
            ViewBag.Disabled = true;
            listBDUser = (List<BusinessDay>)Session["UserBusDays"];
            if (listBDUser == null)
            {
                listBDUser = _busDayRepo.GetOpenedBusinessDay(CurrentUser);
            }
            if (listBDUser.Count() > 1)
            {
                ViewBag.Disabled = false;
            }
            BusinessDay busDays = listBDUser.FirstOrDefault();
            ViewBag.BusnessDayDate = busDays.BDDateOperation.ToString("yyyy-MM-dd");
            ViewBag.CurrentBranch = busDays.BranchID;
            Session["BusnessDayDate"] = busDays.BDDateOperation;
            
            return View(Model());
        }

        public List<BudgetConsumption> Model()
        {
            List<BudgetConsumption> listBudConsume = db.BudgetConsumptions.Where(b => !b.isValidated).OrderBy(a => a.Reference).ToList();
            List<BudgetConsumption> list = new List<BudgetConsumption>();
            listBudConsume.ForEach(c =>
            {
                list.Add(
                                new BudgetConsumption
                                {
                                    BudgetAllocatedID = c.BudgetAllocatedID,
                                    BudgetAllocated = c.BudgetAllocated,
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
        public JsonResult GetBudgetAllocateds()
        {
            List<object> budgetModel = new List<object>();
            //int year = DateTime.Today.Year;
            foreach (BudgetAllocated dL in db.BudgetAllocateds.Where(q => q.FiscalYear.FiscalYearStatus).ToArray().OrderBy(c=>c.BudgetLine.BudgetLineLabel))
            {
                budgetModel.Add(new
                {
                    BudgetLineLabel=dL.BudgetLine.BudgetLineLabel,
                    BudgetAllocatedID=dL.BudgetAllocatedID
                });
            }
            return Json(budgetModel, JsonRequestBehavior.AllowGet);
            
        }
        public JsonResult InitDate(int? BranchID)
        {
            List<object> _InfoList = new List<object>();
            if (BranchID != null && BranchID.HasValue && BranchID.Value > 0)
            {
                BusinessDay businessD = _busDayRepo.FindAll.FirstOrDefault(bd => bd.BranchID == BranchID && bd.BDStatut);
                _InfoList.Add(new
                {
                    DateOperation = businessD.BDDateOperation.ToString("yyyy-MM-dd")
                });
            }

            return Json(_InfoList, JsonRequestBehavior.AllowGet);
        }
        public JsonResult InitTrnNumber(int? BranchID, int? BudgetAllocatedID)
        {
            double crAmt = 0d;
            double dbAmt = 0d;
            string trnnum = "";
            double AllocatedAmt = 0d;
            double AmountSpend = 0d;
            double leftAmount = 0d;

            List <object> _InfoList = new List<object>();
            if (BranchID > 0)
            {
                listBDUser = (List<BusinessDay>)Session["UserBusDays"];
                if (listBDUser == null)
                {
                    listBDUser = _busDayRepo.GetOpenedBusinessDay(CurrentUser);
                }
                BusinessDay businessDay = listBDUser.FirstOrDefault(b => b.BranchID == BranchID.Value);
                trnnum = _transactNumbeRepository.displayTransactNumber("ABUC", businessDay);
            }
            if (BudgetAllocatedID > 0)
            {
                //chargement des mnts du budget
                //amount allocated
                BudgetAllocated budAll = db.BudgetAllocateds.SingleOrDefault(b => b.BudgetAllocatedID == BudgetAllocatedID);
                AllocatedAmt = (budAll != null) ? budAll.AllocateAmount : 0;
                List<BudgetAllocatedUpdate> LstbudgetAllocatedUpdate = db.BudgetAllocatedUpdates.Where(b => b.BudgetAllocatedID == BudgetAllocatedID).ToList();
                if (LstbudgetAllocatedUpdate.Count > 0)
                {
                    crAmt = LstbudgetAllocatedUpdate.Where(b => b.SensImputation == "CR").Select(b => b.Amount).Sum();
                    dbAmt = LstbudgetAllocatedUpdate.Where(b => b.SensImputation == "DB").Select(b => b.Amount).Sum();
                }

                //spend amount
                List<BudgetConsumption> buconsume = db.BudgetConsumptions.Where(bc => bc.BudgetAllocatedID == budAll.BudgetAllocatedID && bc.isValidated).ToList();
                AmountSpend = (buconsume != null) ? buconsume.Select(b => b.VoucherAmount).Sum() : 0;
                //left amount
                leftAmount = AllocatedAmt + crAmt - dbAmt - AmountSpend;
            }

            _InfoList.Add(new
            {
                Reference = trnnum,
                AmountAllocated= AllocatedAmt + crAmt - dbAmt,
                AmountSpend= AmountSpend,
                AmountLeft= leftAmount
            });
            return Json(_InfoList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteBudConsume(string ID)
        {
            bool status = false;
            string Message = "";
            try
            {
                int id = Convert.ToInt32(ID);
                BudgetConsumption BudgetConsumptionToDelete = db.BudgetConsumptions.Find(id);
                db.BudgetConsumptions.Remove(BudgetConsumptionToDelete);
                db.SaveChanges();
                //_budgetConsumptionRepository.Delete(BudgetConsumptionToDelete);
                statusOperation = BudgetConsumptionToDelete.UIBudgetAllocated + " : " + Resources.AlertDeleteAction;

                status = true;
                Message = Resources.Success+" "+ statusOperation;
            }
            catch (Exception e)
            {
                statusOperation = Resources.er_alert_danger + e.Message;
                Message=Resources.UIBudgetAllocated+ " "+ statusOperation;
                status = false;
            }
            return new JsonResult { Data = new { status = status, Message = Message } };
        }
        public JsonResult AddAuthBudgetConsumtion(BudgetConsumption budgetConsumption)
        {
            bool status = false;
            string Message = "";
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
                status = true;
                Message = statusOperation;
            }
            catch (Exception e)
            {
                Message = "Error "+ e.Message + " " + e.StackTrace + " " + e.InnerException;
                status = false;
            }
            return new JsonResult { Data = new { status = status, Message = Message } };
        }
    }
}