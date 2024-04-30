using FatSod.Security.Abstracts;
using FatSod.Security.Entities;
using FatSod.Supply.Abstracts;
using CABOPMANAGEMENT.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Configuration;

namespace CABOPMANAGEMENT.Controllers
{
    [Authorize(Order = 1)]
    [TakeBusinessDay(Order = 2)]
    public class HomeController : BaseController
    {
        private IBusinessDay _busDayRepo;
        private ISale _saleRepository;
        List<BusinessDay> bdDay;
        // GET: Sale/Command
        public HomeController(
            ISale saleRepository,
            IBusinessDay busDayRepo
           
            )
        {
            this._saleRepository = saleRepository;
            this._busDayRepo = busDayRepo;
            
        }
        // [Authorize]
        [OutputCache(Duration = 1800)]
        public ActionResult Index()
        {
            ViewBag.Disabled = true;
            bdDay = (List<BusinessDay>)Session["UserBusDays"];
            if (bdDay == null)
            {
                bdDay = _busDayRepo.GetOpenedBusinessDay(CurrentUser);
            }
            if (bdDay.Count() > 1)
            {
                ViewBag.Disabled = false;
            }
            DateTime busDays = bdDay.FirstOrDefault().BDDateOperation;

            ViewBag.BusnessDayDate = busDays;
            Session["BusnessDayDate"] = busDays;

            //ViewBag.AppNameP = WebConfigurationManager.AppSettings["AppNameP"];
            //ViewBag.AppNameS = WebConfigurationManager.AppSettings["AppNameS"];

            //ViewBag.todaySales = TodaySales();
            //ViewBag.yesterdaySales = YesterdaySales();
            @ViewBag.CompanyLogoID = FileCompany.FileID;
            return View();
        }

        private File FileCompany
        {
            get
            {
                //GlobalPerson gp = db.GlobalPeople.FirstOrDefault(c=>c.GlobalPersonID==1);
                return db.Files.FirstOrDefault(d=>d.GlobalPersonID!=1);
            }
        }
        /// <summary>
        /// Calculates teh TotalSales Today .
        /// </summary>
        /// <returns>returns total sales made today.</returns>
        public double TodaySales()
        {
            DateTime currentDay = (DateTime) Session["BusnessDayDate"];
            double sales = 0;// _saleRepository.TotalSalePeriode(currentDay, currentDay);

            return sales;
        }
        /// <summary>
        /// Calculates the Total Sales on Yesterday.
        /// </summary>
        /// <returns>returns total yesterday's sales</returns>
        public double YesterdaySales()
        {
            DateTime currentDay = (DateTime) Session["BusnessDayDate"];
            DateTime yesterday = currentDay.AddDays(-1);
            double sales = 0;// _saleRepository.TotalSalePeriode(yesterday, yesterday);
            return sales;
        }
        //****************Charts ********************

        /// <summary>
        /// Returns Json data for yearly sales.
        /// </summary>
        /// <returns></returns>
        public JsonResult ForMorris()
        {
            DateTime currentDay = (DateTime)Session["BusnessDayDate"];
            string year = Convert.ToString(currentDay.Year);

            ReportsHomeController rc = new ReportsHomeController();
            var model = rc.YearlySalesByMonth_forCharts(year).ToList();

            var count = model.Select(i => new Object[] { i.GrandTotal }).ToList();
            return Json(count, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Json data for Monthly sales for this month</returns>
        public JsonResult ForMorris2()
        {
            DateTime currentDay = (DateTime)Session["BusnessDayDate"];
            ReportsHomeController rc = new ReportsHomeController();
            var model = rc.MonthlySalesByDate_forCharts(currentDay.Year, currentDay.Month);

            var value = model.Select(i => new Object[] { i.Day.ToString(), i.Total }).ToArray();
            return Json(value, JsonRequestBehavior.AllowGet);
        }
    }
}
