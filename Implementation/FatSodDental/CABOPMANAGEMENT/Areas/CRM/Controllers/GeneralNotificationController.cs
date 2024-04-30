using FatSod.Security.Abstracts;
using FatSod.Supply.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CABOPMANAGEMENT.Controllers;

namespace CABOPMANAGEMENT.Areas.CRM.Controllers
{
    public class GeneralNotificationController : BaseNotificationController
    {

        private IBusinessDay _busDayRepo;

        public GeneralNotificationController(
            IBusinessDay busDayRepo, IHistoSMS HistoSMSRepository, IExtractSMS ExtractSMSRepository,
                INotificationSetting notificationSettingRepository
        ): base(HistoSMSRepository, ExtractSMSRepository, notificationSettingRepository)
        {
            this._busDayRepo = busDayRepo;
        }
        // GET: CRM/GeneralNotification
        public ActionResult Index()
        {
            return View();
            // base.sen
        }

        public JsonResult SenSMS(string smsEnvoyeFR, string smsEnvoyeEN, int SMSLng, string phoneNumber)
        {
            try
            {

            } catch(Exception e)
            {

            }
            List<SMSReceiverModel> selectedPatients = new List<SMSReceiverModel>();
            selectedPatients.Add(new SMSReceiverModel
            {
                CustomerID = int.Parse(phoneNumber),
                CustomerPhone = phoneNumber,
            });
            string TypeSms = "GENERAL";
            return this.EnvoiSMS(smsEnvoyeFR, smsEnvoyeEN, SMSLng, selectedPatients, TypeSms);
        }
    }
}