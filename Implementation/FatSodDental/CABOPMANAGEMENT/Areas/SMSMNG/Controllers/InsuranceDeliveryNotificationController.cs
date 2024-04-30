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
    public class InsuranceDeliveryNotificationController : BaseNotificationController
    {
        private IBusinessDay _busDayRepo;

        public InsuranceDeliveryNotificationController(
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

        public JsonResult GetSales(DateTime startDate, DateTime endDate)
        {
            var res = (from csb in db.CumulSaleAndBills
                       join cor in db.CustomerOrders on csb.CustomerOrderID equals cor.CustomerOrderID
                       // join c in db.Customers on cor.CustomerID equals c.GlobalPersonID
                       where csb.ProductDeliverDate >= startDate && csb.ProductDeliverDate <= endDate
                       select new
                       {
                           dateRDV = csb.ProductDeliverDate,
                           ID = csb.CumulSaleAndBillID,
                           CustomerID = "",
                           /*CustomerName = (cor.Customer.Description != null) ? cor.Customer.Name.Trim() + " " + cor.Customer.Description.Trim() : cor.Customer.Name.Trim(),
                           CustomerPhone = cor.Customer.Adress != null ? cor.Customer.Adress.AdressPhoneNumber : "",
                           CustomerQuater = cor.Customer.Adress != null && cor.Customer.Adress.Quarter != null ? cor.Customer.Adress.Quarter.QuarterLabel : "",*/
                           CustomerName = cor.CustomerName,
                           CustomerPhone = cor.PhoneNumber,
                           CustomerQuater = "",
                           SaleDeliveryDate = "",
                           Insurance = cor.Assureur.Name,
                           InsuredCompany = cor.InsuredCompany.InsuredCompanyLabel,
                           CustomerOrder = cor,
                           CumulSaleAndBill = csb
                       }).ToList();
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
                    dateRDV = p.dateRDV.Value.ToString("dd/MM/yyyy"),
                    ID = p.ID,
                    CustomerID = p.CustomerID,
                    CustomerName = p.CustomerName,
                    CustomerPhone = p.CustomerPhone,
                    CustomerQuater = p.CustomerQuater,
                    SaleDeliveryDate = p.dateRDV.Value.ToString("dd/MM/yyyy"),
                    Insurance = p.Insurance,
                    InsuredCompany = p.InsuredCompany,
                    CustomerValue = p.CustomerOrder.CustomerValueUI,
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