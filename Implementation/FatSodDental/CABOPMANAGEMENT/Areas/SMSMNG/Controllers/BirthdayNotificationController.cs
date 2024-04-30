using FatSod.Ressources;
using FatSod.Security.Abstracts;
using FatSod.Security.Entities;
using FatSod.Supply.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CABOPMANAGEMENT.Controllers;

namespace CABOPMANAGEMENT.Areas.SMSMNG.Controllers
{
    public class BirthdayNotificationController : BaseNotificationController
    {

        private IBusinessDay _busDayRepo;
        public BirthdayNotificationController(
            IBusinessDay busDayRepo, IHistoSMS HistoSMSRepository, IExtractSMS ExtractSMSRepository,
                INotificationSetting notificationSettingRepository
        ): base(HistoSMSRepository, ExtractSMSRepository, notificationSettingRepository)
        {
            this._busDayRepo = busDayRepo;
        }

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

        public JsonResult GetSales(DateTime startDate, DateTime endDate)
        {

            var res = (from customer in db.Customers
                       where customer.DateOfBirth != null &&
                       (customer.DateOfBirth.Value.Month >= startDate.Month && customer.DateOfBirth.Value.Month <= endDate.Month) &&
                       (customer.DateOfBirth.Value.Day >= startDate.Day && customer.DateOfBirth.Value.Day <= endDate.Day)
                       select new
                       {
                           dateRDV = customer.DateOfBirth,
                           ID = customer.GlobalPersonID,
                           CustomerID = customer.GlobalPersonID,
                           CustomerName = (customer.Description != null) ? customer.Name.Trim() + " " + customer.Description.Trim() : customer.Name.Trim(),
                           CustomerPhone = customer.Adress != null ? customer.Adress.AdressPhoneNumber : "",
                           CustomerQuater = customer.Adress != null && customer.Adress.Quarter != null ? customer.Adress.Quarter.QuarterLabel : "",
                           SaleDeliveryDate = "",
                           PreferredLanguage = customer.PreferredLanguage,
                           Customer = customer
                       }).ToList();
            List<object> sales = new List<object>();
            res.ForEach(p =>
            {
                string CivilityUI = p.Customer.Sex.Civility == "Civility_M" ? @Resources.Civility_M :
                                    p.Customer.Sex.Civility == "Civility_F" ? @Resources.Civility_F : null;

                var sale = new
                {
                    dateRDV = p.dateRDV.Value.ToString("dd/MM/yyyy"),
                    ID = p.ID,
                    CustomerID = p.CustomerID,
                    CustomerName = p.CustomerName,
                    CustomerPhone = p.CustomerPhone,
                    CustomerQuater = p.CustomerQuater,
                    SaleDeliveryDate = p.dateRDV.Value.ToString("dd/MM/yyyy"),
                    SMSLng = getPreferredLanaguage(p.PreferredLanguage),
                    CustomerValue = p.Customer.CustomerValueUI,
                    Civility = p.Customer.Sex.Civility,
                    CivilityUI = CivilityUI
                };
                sales.Add(sale);
            });
            var jsonResult = Json(sales, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = Int32.MaxValue;
            return jsonResult;
        }
    }
}