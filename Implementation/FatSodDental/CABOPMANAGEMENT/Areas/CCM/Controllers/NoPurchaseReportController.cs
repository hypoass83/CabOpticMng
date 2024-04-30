using FatSod.Ressources;
using FatSod.Security.Abstracts;
using FatSod.Security.Entities;
using FatSod.Supply.Abstracts;
using FatSod.Supply.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CABOPMANAGEMENT.Controllers;

namespace CABOPMANAGEMENT.Areas.CCM.Controllers
{
    public class NoPurchaseReportController : BaseController
    {
        private const string CONTROLLER_NAME = "CCM/NoPurchaseReport";
        private const string VIEW_NAME = "Index";
        private INoPurchase _noPurchaseRepository;
        private IBusinessDay _busDayRepo;
        private List<BusinessDay> listBDUser;

        public NoPurchaseReportController(
            IBusinessDay busDayRepo,
            INoPurchase noPurchaseRepository
            )
        {
            this._noPurchaseRepository = noPurchaseRepository;
            this._busDayRepo = busDayRepo;
        }

        // GET: CRM/NoPurchaseReport
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

        public JsonResult getNoPurchases(DateTime startDate, DateTime endDate)
        {

            List<NoPurchase> noPurchases = this._noPurchaseRepository.FindAll.Where(np => np.ConsultationDate >= startDate &&
                                                                                          np.ConsultationDate <= endDate).ToList();

            noPurchases.ForEach(np =>
            {
                if (np.ConsultDilatationId != null)
                {
                    np.Customer = np.ConsultDilatation.ConsultDilPresc.Consultation.Customer.CustomerFullName;
                    np.PrescriptionSummary = np.ConsultDilatation.ConsultDilPresc.Consultation.Customer.AdressPhoneNumber;
                    np.CustomerValue = np.ConsultDilatation.ConsultDilPresc.Consultation.Customer.CustomerValueUI;
                }

                if (np.ConsultLensPrescriptionID != null)
                {
                    np.Customer = np.ConsultLensPrescription.ConsultDilPresc.Consultation.Customer.CustomerFullName;
                    np.PrescriptionSummary = np.ConsultLensPrescription.ConsultDilPresc.Consultation.Customer.AdressPhoneNumber;
                    np.CustomerValue = np.ConsultLensPrescription.ConsultDilPresc.Consultation.Customer.CustomerValueUI;
                    np.Consultant = np.ConsultLensPrescription.ConsultBy.UserFullName;
                }

                np.DisplayDate = np.OperationDate.ToString("yyyy-MM-dd");
                np.DisplayConsultationDate = np.ConsultationDate.ToString("yyyy-MM-dd");
                
                np.OperationType = np.ConsultDilatationId != null ? "Dilatation" : "Prescription";

                np.ConsultLensPrescription = null;
                np.ConsultDilatation = null;
            });

            Session["noPurchases"] = noPurchases;

            var model = new
            {
                data = noPurchases
            };
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateNoPurchase(int NoPurchaseId, string CustomerServiceReason)
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
                NoPurchase selectedNoPurchase = noPurchases.SingleOrDefault(np => np.NoPurchaseId == NoPurchaseId);
                selectedNoPurchase.CustomerServiceReason = CustomerServiceReason;
                selectedNoPurchase.CSOperationDate = BDDateOperation;

                if (selectedNoPurchase.NoPurchaseId == 0)
                {
                    status = false;
                    Message = "Error: Please select operation ";
                    return new JsonResult { Data = new { status = status, Message = Message } };
                }
                _noPurchaseRepository.CustomerServiceReasonUpdate(selectedNoPurchase);
                status = true;
                Message = Resources.Success + " No Purchase was updated successfuly";
            }
            catch (Exception e)
            {
                status = false;
                Message = "Error " + e.Message + " " + e.StackTrace + " " + e.InnerException;
            }
            return new JsonResult { Data = new { status = status, Message = Message } };
        }


    }


}