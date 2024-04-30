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
    public class CommandGlassNotificationController : BaseNotificationController
    {

        private IBusinessDay _busDayRepo;
        public CommandGlassNotificationController(
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
            var res = db.CumulSaleAndBills.Where(c => c.SaleDate >= startDate && c.SaleDate <= endDate
                      && !c.isReturn && !c.IsProductDeliver &&
                      c.CumulSaleAndBillLines.Any(line => line.isCommandGlass == true)).ToList();
            List<object> sales = new List<object>();
            res.ForEach(sale =>
            {
                string customerName = "";
                string phoneNumber = "";
                string customerType = "CASH";
                string insurance = "";
                string company = "";
                string customerValue = "";
                string CivilityUI = "";
                string Civility = "";


                if (sale.SaleID != null)
                {
                    customerName = sale.Customer.CustomerFullName;
                    phoneNumber = sale.Customer.AdressPhoneNumber;
                    customerValue = sale.Customer.CustomerValueUI;
                    Civility = sale.Customer.Sex.Civility;
                    CivilityUI = Civility == "Civility_M" ? @Resources.Civility_M : @Resources.Civility_F;
                }

                if (sale.CustomerOrderID != null)
                {
                    // En attendant la migration de CustomerId dans CustomerOrder pour les Insured Customer
                    customerName = sale.CustomerOrder.CustomerName;
                    phoneNumber = sale.CustomerOrder.PhoneNumber;
                    customerType = "INSURED";
                    insurance = sale.CustomerOrder.Assureur.Name;
                    company = sale.CustomerOrder.InsuredCompany.InsuredCompanyLabel;
                    customerValue = sale.CustomerOrder.CustomerValueUI;

                    if (sale.CustomerOrder.CustomerID != null)
                    {
                        Civility = sale.CustomerOrder.Customer.Sex.Civility;
                        CivilityUI = Civility == "Civility_M" ? @Resources.Civility_M : @Resources.Civility_F;
                    } else
                    {
                        Civility = null;
                        Civility = null;
                        CivilityUI = null;
                    }
                }

                string PreferredLanguage = sale.CustomerID != null ? sale.Customer.PreferredLanguage : "FR";


                var csb = new
                {
                    dateRDV = sale.SaleDate.ToString("dd/MM/yyyy"),
                    ID = sale.CumulSaleAndBillID,
                    CustomerID = sale.CustomerID,
                    CustomerName = customerName,
                    CustomerPhone = phoneNumber,
                    SMSLng = getPreferredLanaguage(PreferredLanguage),
                    CustomerType = customerType,
                    Insurance = insurance,
                    InsuredCompany = company,
                    IsReceived = sale.IsReceived ? "YES" : "NO",
                    CustomerValue = customerValue,
                    CivilityUI = CivilityUI,
                    Civility = Civility
                };
                sales.Add(csb);
            });
            var jsonResult = Json(sales, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = Int32.MaxValue;
            return jsonResult;
        }
    }
}