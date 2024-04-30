using FatSod.Ressources;
using FatSod.Security.Abstracts;
using FatSod.Supply.Abstracts;
using FatSod.Supply.Entities;
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
    public class NotificationSettingController : BaseController
    {
        private IBusinessDay _busDayRepo;

        private INotificationSetting _notificationSettingRepository;
        public NotificationSettingController(
            INotificationSetting notificationSettingRepository
            )
        {
            this._notificationSettingRepository = notificationSettingRepository;
        }
        // GET: CRM/Consultation
        [OutputCache(Duration = 3600)]
        public ActionResult Index()
        {
            return this.View();
        }

        public JsonResult getNotificationSettings()
        {
            List<NotificationSetting> notificationSettings = _notificationSettingRepository.FindAll.ToList();
            notificationSettings.ForEach(s =>
            {
                if (s.NotificationType == NotificationType.RDV_CONSULTATION)
                {
                    s.NotificationTypeUI = @Resources.PatientReminder;
                }

                if (s.NotificationType == NotificationType.DELIVERY)
                {
                    s.NotificationTypeUI = @Resources.DeliveryNotification;
                }

                if (s.NotificationType == NotificationType.INSURANCE_DELIVERY)
                {
                    s.NotificationTypeUI = @Resources.InsuranceDeliveryNotification;
                }

                if (s.NotificationType == NotificationType.BIRTHDAY)
                {
                    s.NotificationTypeUI = @Resources.BirthdayNotification;
                }

                if (s.NotificationType == NotificationType.COMMAND_GLASS)
                {
                    s.NotificationTypeUI = @Resources.CommandGlassNotification;
                }

                if (s.NotificationType == NotificationType.PURCHASE)
                {
                    s.NotificationTypeUI = @Resources.PurchaseNotification;
                }
            });
            Session["notificationSettings"] = notificationSettings;
            return Json(notificationSettings, JsonRequestBehavior.AllowGet);
        }

        public JsonResult getNotificationSetting(int notificationSettingId)
        {
            List<NotificationSetting> notificationSettings = ((List<NotificationSetting>)Session["notificationSettings"]);
            NotificationSetting selectedNotificationSetting = notificationSettings.Find(x => x.NotificationSettingId == notificationSettingId);
            return Json(selectedNotificationSetting, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AddNotificationSetting(NotificationSetting notificationSetting)
        {
            bool status = false;
            string Message = "";
            try
            {
                NotificationSetting existingSetting = _notificationSettingRepository.FindAll.SingleOrDefault(s => s.NotificationType == notificationSetting.NotificationType);

                if (existingSetting == null)
                {
                    _notificationSettingRepository.Create(notificationSetting);
                    Message = Resources.Success + " Notification Setting has been successfuly created";
                }

                if (existingSetting != null)
                {
                    existingSetting.FrenchMessage = notificationSetting.FrenchMessage;
                    existingSetting.EnglishMessage = notificationSetting.EnglishMessage;
                    _notificationSettingRepository.Update(existingSetting);
                    Message = Resources.Success + " Notification Setting has been successfuly updated";
                }

                status = true;
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