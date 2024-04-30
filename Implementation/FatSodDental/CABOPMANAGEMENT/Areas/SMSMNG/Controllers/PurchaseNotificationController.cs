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
    public class PurchaseNotificationController : BaseNotificationController
    {
        private IBusinessDay _busDayRepo;

        public PurchaseNotificationController(
            IBusinessDay busDayRepo, IHistoSMS HistoSMSRepository, IExtractSMS ExtractSMSRepository,
                INotificationSetting notificationSettingRepository
        ): base(HistoSMSRepository, ExtractSMSRepository, notificationSettingRepository)
        {
            this._busDayRepo = busDayRepo;
        }
        // GET: CRM/PurchaseNotification
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
            var res = (from csb in db.CumulSaleAndBills
                       join s in db.Sales on csb.SaleID equals s.SaleID
                       join c in db.Customers on s.CustomerID equals c.GlobalPersonID
                       where csb.SaleDate >= startDate && csb.SaleDate <= endDate
                       select new
                       {
                           dateRDV = csb.SaleDate,
                           ID = csb.CumulSaleAndBillID,
                           CustomerID = s.Customer.GlobalPersonID,
                           CustomerName = (s.Customer.Description != null) ? s.Customer.Name.Trim() + " " + s.Customer.Description.Trim() : s.Customer.Name.Trim(),
                           CustomerPhone = s.Customer != null ? s.Customer.Adress.AdressPhoneNumber :
                           (s.CustomerOrderFK != null ? s.CustomerOrderFK.PhoneNumber : ""),
                           CustomerQuater = s.Customer.Adress != null && s.Customer.Adress.Quarter != null ? s.Customer.Adress.Quarter.QuarterLabel : "",
                           SaleDeliveryDate = "",
                           PreferredLanguage = s.Customer.PreferredLanguage,
                           Customer = s.Customer,
                           CumulSaleAndBill = csb

                       })/*.Distinct()*/.ToList();
            List<object> sales = new List<object>();
            res.ForEach(p =>
            {
                string CivilityUI = "";
                string Civility = "";

                if (p.CumulSaleAndBill.SaleID != null)
                {

                    Civility = p.CumulSaleAndBill.Customer.Sex.Civility;
                    CivilityUI = Civility == "Civility_M" ? @Resources.Civility_M : @Resources.Civility_F;
                }

                if (p.CumulSaleAndBill.CustomerOrderID != null)
                {
                    if (p.CumulSaleAndBill.CustomerOrder.CustomerID != null)
                    {
                        Civility = p.CumulSaleAndBill.CustomerOrder.Customer.Sex.Civility;
                        CivilityUI = Civility == "Civility_M" ? @Resources.Civility_M : @Resources.Civility_F;
                    }
                    else
                    {
                        Civility = null;
                        Civility = null;
                        CivilityUI = null;
                    }
                }

                var sale = new
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
                sales.Add(sale);
            });
            var jsonResult = Json(sales, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = Int32.MaxValue;
            return jsonResult;
        }
    }
}