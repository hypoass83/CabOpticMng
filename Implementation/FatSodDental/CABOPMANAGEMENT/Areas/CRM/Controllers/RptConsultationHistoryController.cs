using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FatSod.Report.WrapQuery;
using FatSod.Ressources;
using FatSod.Security.Abstracts;
using FatSod.Security.Entities;
using FatSod.Supply.Abstracts;
using FatSod.Supply.Entities;
using CABOPMANAGEMENT.Controllers;
using CABOPMANAGEMENT.Filters;
using CABOPMANAGEMENT.Tools;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using SaleE = FatSod.Supply.Entities.Sale;
using FastSod.Utilities.Util;


namespace CABOPMANAGEMENT.Areas.CRM.Controllers
{
    [Authorize(Order = 1)]
    [TakeBusinessDay(Order = 2)]
    public class RptConsultationHistoryController : BaseController
    {

        //person repository

        private IBusinessDay _busDayRepo;
        private ISale _saleRepository;
        private IRepository<FatSod.Security.Entities.File> _fileRepository;
        private ICustomerReturn _customerReturnRepository;
        private IDeposit _depositRepo;

        public RptConsultationHistoryController(
            ISale sale,
            IBusinessDay busDayRepo,
            ICustomerReturn customerReturnRepository,
            IRepository<FatSod.Security.Entities.File> fileRepository,
            IDeposit depositRepo
            )
        {
            this._saleRepository = sale;
            this._busDayRepo = busDayRepo;
            this._fileRepository = fileRepository;
            this._customerReturnRepository = customerReturnRepository;
            this._depositRepo = depositRepo;
        }
        //
        // GET: /CashRegister/RptGeneSale/
        [OutputCache(Duration = 3600)]
        public ActionResult Index()
        {
            List<BusinessDay> listBDUser = (List<BusinessDay>)Session["UserBusDays"];
            if (listBDUser == null)
            {
                listBDUser = _busDayRepo.GetOpenedBusinessDay(CurrentUser);
            }
            if (listBDUser.Count() > 1)
            {
                ViewBag.Disabled = false;
            }
            DateTime busDays = listBDUser.FirstOrDefault().BDDateOperation;
            ViewBag.CurrentBranch = listBDUser.FirstOrDefault().BranchID;
            ViewBag.BusnessDayDate = busDays.ToString("yyyy-MM-dd");
            Session["BusnessDayDate"] = busDays;

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

        //affiche la liste des
        public JsonResult ModelRptGeneSale(int BranchID, DateTime BeginDate, DateTime EndDate)
        {


            double advancedAmount = 0d;
            List<TabModelRptGeneSale> listGenSale = new List<TabModelRptGeneSale>();

            List<PrescriptionLStep> prescriptionLSteps = db.PrescriptionLSteps.Where(so => (so.DatePrescriptionLStep >= BeginDate && so.DatePrescriptionLStep <= EndDate)).ToList();
            List<object> data = new List<object>();
            int i = 0;
            foreach (var prescriptionLStep in prescriptionLSteps.OrderBy(o => o.DateHeurePrescriptionLStep))
            {
                bool isBillCustomer = prescriptionLStep.Consultation.Customer.IsBillCustomer;
                bool isNewCustomer = prescriptionLStep.Consultation.IsNewCustomer;
                ConsultLensPrescription prescription = db.ConsultLensPrescriptions.FirstOrDefault(c => c.ConsultDilPresc.ConsultationID == prescriptionLStep.ConsultationID);
                
                var item = new
                {
                    number = ++i,
                    customerValue = prescriptionLStep.Consultation.Customer.CustomerValueUI,
                    customerType = isBillCustomer == true ? "INSURED" : "CASH",
                    isNewCustomer = isNewCustomer == true ? "YES" : "NO",
                    operationDate = prescriptionLStep.Consultation.DateConsultation.ToString("yyyy-MM-dd"),
                    dateRDV = prescriptionLStep.DateRdv.ToString("yyyy-MM-dd"),
                    patient = prescriptionLStep.Consultation.Customer.CustomerFullName,
                    consultant = prescriptionLStep.ConsultBy.UserFullName,
                    remark = prescriptionLStep.Remarque,
                    prescription = prescription != null ? LensConstruction.getPrescriptionSummary(prescription) : "RAS"
                };
                data.Add(item);
            }

            var list = new
            {
                data = data
            };
            var jsonResult = Json(list, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = Int32.MaxValue;
            return jsonResult;

        }
    }
}