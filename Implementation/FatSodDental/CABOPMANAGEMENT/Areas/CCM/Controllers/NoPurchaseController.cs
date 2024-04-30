using FatSod.Security.Abstracts;
using FatSod.Security.Entities;
using FatSod.Supply.Entities;
using CABOPMANAGEMENT.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using FatSod.Supply.Abstracts;
using FatSod.Ressources;
using CABOPMANAGEMENT.Filters;
using System.Globalization;

namespace CABOPMANAGEMENT.Areas.CCM.Controllers
{
    [Authorize(Order = 1)]
    [TakeBusinessDay(Order = 2)]
    public class NoPurchaseController : BaseController
    {
        //Current Controller and current page
        private const string CONTROLLER_NAME = "CCM/NoPurchase";
        private const string VIEW_NAME = "Index";

        private INoPurchase _noPurchaseRepository;

        private IBusinessDay _busDayRepo;

        private List<BusinessDay> listBDUser;

        // public NoPurchaseController() { }

        public NoPurchaseController(
            IBusinessDay busDayRepo,
            INoPurchase noPurchaseRepository
            )
        {
            this._noPurchaseRepository = noPurchaseRepository;
            this._busDayRepo = busDayRepo;
        }
        // GET: CRM/NoPurchase
        [OutputCache(Duration = 3600)]
        public ActionResult Index()
        {
            Session["Curent_Page"] = VIEW_NAME;
            Session["Curent_Controller"] = CONTROLLER_NAME;
            
            listBDUser = (List<BusinessDay>)Session["UserBusDays"];

            if (listBDUser == null)
            {
                listBDUser = _busDayRepo.GetOpenedBusinessDay(CurrentUser);
            }
            DateTime BDDateOperation = listBDUser.FirstOrDefault().BDDateOperation;

            ViewBag.OperationDate = BDDateOperation.ToString("yyyy-MM-dd");
            return View();
        }
        
        public JsonResult AddNoPurchase(int id, string reason)
        {
            bool status = false;
            string Message = "";
            try
            {
                List<NoPurchase> noPurchases = ((List<NoPurchase>)Session["noPurchases"]);
                listBDUser = (List<BusinessDay>)Session["UserBusDays"];

                if (listBDUser == null)
                {
                    listBDUser = _busDayRepo.GetOpenedBusinessDay(CurrentUser);
                }
                DateTime BDDateOperation = listBDUser.FirstOrDefault().BDDateOperation;
                NoPurchase selectedNoPurchase = noPurchases.SingleOrDefault(np => np.Id == id);
                selectedNoPurchase.DeliveryDeskReason = reason;
                selectedNoPurchase.OperationDate = BDDateOperation;

                if (selectedNoPurchase.ConsultDilatationId == null && selectedNoPurchase.ConsultLensPrescriptionID == null)
                {
                    status = false;
                    Message = "Error: Please select operation ";
                    return new JsonResult { Data = new { status = status, Message = Message } };
                }
                _noPurchaseRepository.Create(selectedNoPurchase);
                status = true;
                Message = Resources.Success + " No Purchase was done successfuly";
            }
            catch (Exception e)
            {
                status = false;
                Message = "Error " + e.Message + " " + e.StackTrace + " " + e.InnerException;
            }
            return new JsonResult { Data = new { status = status, Message = Message } };
        }
        
        public JsonResult getNoPurchases(DateTime OperationDate)
        {
            Session["OperationDate"] = OperationDate.ToString("yyyy-MM-dd");
            listBDUser = (List<BusinessDay>)Session["UserBusDays"];
            if (listBDUser == null)
            {
                listBDUser = _busDayRepo.GetOpenedBusinessDay(CurrentUser);
            }
            int currentBD = listBDUser.FirstOrDefault().BranchID;
            List<NoPurchase> noPurchases = new List<NoPurchase>();
            int id = 0;
            List<NoPurchase> prescriptions = this.getUnSoldPrescriptionHistories(OperationDate, ref id);
            noPurchases.AddRange(prescriptions);

            List<NoPurchase> dilatations = this.getUnSoldDilatationHistories(OperationDate, ref id);
            noPurchases.AddRange(dilatations);

            Session["noPurchases"] = noPurchases;
            
            var model = new
            {
                data = noPurchases
            };
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        private List<NoPurchase> getUnSoldPrescriptionHistories(DateTime OperationDate, ref int id)
        {
            List<NoPurchase> prescriptionSalesHistories = new List<NoPurchase>();

            List<ConsultLensPrescription> prescriptions = db.ConsultLensPrescriptions.Where(clp => clp.DatePrescription == OperationDate && !clp.ConsultDilPresc.Consultation.Customer.IsBillCustomer && !clp.isAuthoriseSale &&
                !db.AuthoriseSales.Any(s => s.ConsultDilPrescID == clp.ConsultDilPrescID && !s.IsDilatation) &&
                !db.NoPurchases.Any(np => np.ConsultLensPrescriptionID == clp.ConsultLensPrescriptionID)).ToList();

            int Id = id;

            prescriptions.ForEach(prescription =>
            {
                prescriptionSalesHistories.Add(
                    new NoPurchase
                    {
                        Customer = prescription.ConsultDilPresc.Consultation.Customer.CustomerFullName,
                        CustomerValue = prescription.ConsultDilPresc.Consultation.Customer.CustomerValueUI, 
                        ConsultLensPrescriptionID = prescription.ConsultLensPrescriptionID,
                        ConsultDilatationId = null,
                        OperationDate = prescription.DatePrescription,
                        ConsultationDate = prescription.ConsultDilPresc.Consultation.DateConsultation,
                        DisplayDate = prescription.DatePrescription.ToString("yyyy-MM-dd"),
                        DeliveryDeskReason = "",
                        CustomerServiceReason = "",
                        PrescriptionSummary = this.getPrescriptionSummary(prescription),
                        DilatationCode = "",
                        Consultant = prescription.ConsultBy.UserFullName,
                        Id = ++Id ,
                        //IsInsuredCustomer = prescription.ConsultDilPresc.Consultation.Customer.IsBillCustomer
                    }
                 );
            });

            id = Id;

            return prescriptionSalesHistories;
        }

        private List<NoPurchase> getUnSoldDilatationHistories(DateTime OperationDate, ref int id)
        {
            List<NoPurchase> prescriptionSalesHistories = new List<NoPurchase>();

            List<ConsultDilatation> dilatations = db.ConsultDilatations.Where(dil => dil.DateDilation == OperationDate && !dil.ConsultDilPresc.Consultation.Customer.IsBillCustomer && !dil.isAuthoriseSale &&
                !db.AuthoriseSales.Any(s => s.ConsultDilPrescID == dil.ConsultDilPrescID && s.IsDilatation) &&
                !db.NoPurchases.Any(np => np.ConsultDilatationId == dil.ConsultDilatationID)).ToList();
            int Id = id;
            dilatations.ForEach(dilatation =>
            {
                
                DateTime dateOperationHours = dilatation.DateDilation;
                prescriptionSalesHistories.Add(
                    new NoPurchase
                    {
                        ConsultLensPrescriptionID = null,
                        Consultant = dilatation.ConsultBy.UserFullName,
                        CustomerValue = dilatation.ConsultDilPresc.Consultation.Customer.CustomerValueUI,
                        ConsultDilatationId = dilatation.ConsultDilatationID,
                        OperationDate = dilatation.DateDilation,
                        ConsultationDate = dilatation.ConsultDilPresc.Consultation.DateConsultation,
                        DisplayDate = dilatation.DateDilation.ToString("yyyy-MM-dd"),
                        DeliveryDeskReason = "",
                        CustomerServiceReason = "",
                        PrescriptionSummary = "Dilatation",
                        DilatationCode = dilatation.CodeDilation,
                        Customer = dilatation.ConsultDilPresc.Consultation.Customer.CustomerFullName,
                        Id = ++Id,
                        //IsInsuredCustomer=dilatation.ConsultDilPresc.Consultation.Customer.IsBillCustomer
                    }
                 );
            });

            id = Id;

            return prescriptionSalesHistories;
        }

        private string getPrescriptionSummary(ConsultLensPrescription prescription)
        {
            string summary = "";
 
            // OG
            summary = LensConstruction.GetLensCodeByPrescription(prescription, EyeSide.OG) + "<br>";

            // OD
            summary = summary + LensConstruction.GetLensCodeByPrescription(prescription, EyeSide.OD);

            return summary;
        }

    }

}