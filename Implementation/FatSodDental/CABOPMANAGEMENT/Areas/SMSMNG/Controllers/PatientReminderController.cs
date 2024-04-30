using FatSod.Ressources;
using FatSod.Security.Abstracts;
using FatSod.Security.Entities;
using FatSod.Supply.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CABOPMANAGEMENT.Controllers;
using CABOPMANAGEMENT.Filters;

namespace CABOPMANAGEMENT.Areas.SMSMNG.Controllers
{
    [Authorize(Order = 1)]
    [TakeBusinessDay(Order = 2)]
    public class PatientReminderController : BaseNotificationController
    {
        private IBusinessDay _busDayRepo;       

        public PatientReminderController(
            IBusinessDay busDayRepo, IHistoSMS HistoSMSRepository, IExtractSMS ExtractSMSRepository,
                INotificationSetting notificationSettingRepository
        ) : base(HistoSMSRepository, ExtractSMSRepository, notificationSettingRepository)
        {
            this._busDayRepo = busDayRepo;
        }
        // GET: CRM/Consultation
        [OutputCache(Duration = 3600)]
        public ActionResult Index()
        {
            ViewBag.DisplayForm = 1;
            try
            {
                List<BusinessDay> bdDay = (List<BusinessDay>)Session["UserBusDays"];
                if (bdDay == null)
                {
                    bdDay = _busDayRepo.GetOpenedBusinessDay(CurrentUser);
                }
                if (bdDay.Count() > 1)
                {
                    TempData["Message"] = "Wrong Business day.<br/>contact our administrator for this purpose<code/>.";
                    ViewBag.Disabled = false;
                    ViewBag.DisplayForm = 0;
                }
                Session["businessDay"] = bdDay.FirstOrDefault();
                DateTime currentDateOp = bdDay.FirstOrDefault().BDDateOperation;
                ViewBag.CurrentBranch = bdDay.FirstOrDefault().BranchID;
                ViewBag.BusnessDayDate = bdDay.FirstOrDefault().BDDateOperation.ToString("yyyy-MM-dd");// businessDay.BDDateOperation;

                Session["BusnessDayDate"] = currentDateOp;
                int deviseID = (Session["DefaultDeviseID"] == null) ? 0 : (int)Session["DefaultDeviseID"];
                if (deviseID <= 0)
                {
                    InjectUserConfigInSession();
                }
                Session["Receipt_GlobalPersonID"] = null;
            }
            catch (Exception e)
            {
                TempData["Message"] = "Error " + e.Message;
                ViewBag.DisplayForm = 0;
            }
            return this.View();
        }

        public JsonResult GetPatients(int days)
        {
            BusinessDay secbusday = SessionBusinessDay(null);
            DateTime dateRDVStart = secbusday.BDDateOperation;
            DateTime dateRDVEnd = dateRDVStart.AddDays(+days);

            var res = (from pls in db.PrescriptionLSteps
                               join c in db.Consultations on pls.ConsultationID equals c.ConsultationID
                               where pls.DateRdv >= dateRDVStart && pls.DateRdv <= dateRDVEnd
                            select new
                               {
                                   dateRDV = pls.DateRdv,
                                   ID = c.Customer.GlobalPersonID,
                                   CustomerID = c.Customer.GlobalPersonID,
                                   CustomerName = (c.Customer.Description!=null) ? c.Customer.Name.Trim() + " " + c.Customer.Description.Trim() : c.Customer.Name.Trim(),
                                   CustomerPhone = c.Customer.Adress != null ? c.Customer.Adress.AdressPhoneNumber : "",
                                   CustomerQuater = c.Customer.Adress != null && c.Customer.Adress.Quarter != null ? c.Customer.Adress.Quarter.QuarterLabel : "",
                                   SaleDeliveryDate = "",
                                   PreferredLanguage = c.Customer.PreferredLanguage,
                                   Customer = c.Customer

                            }).ToList();
            List<object> patients = new List<object>();
            res.ForEach(p =>
            {
                string CivilityUI = p.Customer.Sex.Civility == "Civility_M" ? @Resources.Civility_M : @Resources.Civility_F;
                string Civility = p.Customer.Sex.Civility;

                var patient = new
                {
                    dateRDV = p.dateRDV.ToString("dd/MM/yyyy"),
                    ID = p.ID,
                    CustomerID = p.CustomerID,
                    CustomerName = p.CustomerName,
                    CustomerPhone = p.CustomerPhone,
                    CustomerQuater = p.CustomerQuater,
                    SaleDeliveryDate = p.dateRDV.ToString("dd/MM/yyyy"),
                    SMSLng = getPreferredLanaguage(p.PreferredLanguage),
                    CustomerValue = p.Customer.CustomerValueUI,
                    CivilityUI = CivilityUI,
                    Civility = Civility
                };
                patients.Add(patient);
            });
            var jsonResult = Json(patients, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = Int32.MaxValue;
            return jsonResult;
        }
    }
}