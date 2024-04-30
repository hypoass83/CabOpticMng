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

namespace CABOPMANAGEMENT.Areas.CRM.Controllers
{
    public class CustomerSatisfactionReportController : BaseController
    {
        //Current Controller and current page
        private const string CONTROLLER_NAME = "CRM/CustomerSatisfactionReport";
        private const string VIEW_NAME = "Index";

        private ICustomerSatisfaction _CustomerSatisfactionRepository;

        private IBusinessDay _busDayRepo;

        private List<BusinessDay> listBDUser;

        public CustomerSatisfactionReportController(
            IBusinessDay busDayRepo,
            ICustomerSatisfaction CustomerSatisfactionRepository
            )
        {
            this._CustomerSatisfactionRepository = CustomerSatisfactionRepository;
            this._busDayRepo = busDayRepo;
        }

        // GET: CRM/CustomerSatisfactionReport
        public ActionResult Index()
        {
            Session["Curent_Page"] = VIEW_NAME;
            Session["Curent_Controller"] = CONTROLLER_NAME;

            ViewBag.OperationDate = getOperationDate().ToString("yyyy-MM-dd");
            return View();
        }

        public JsonResult getCustomerSatisfactions(DateTime startDate, DateTime endDate)
        {
            List<CustomerSatisfaction> customerSatisfactions = _CustomerSatisfactionRepository.FindAll.Where(cs => 
                cs.SaleDate >= startDate && cs.SaleDate <= endDate
            ).ToList();

            customerSatisfactions.ForEach(cs =>
            {
                if (cs.CumulSaleAndBill.SaleID != null)
                {
                    cs.Customer = cs.CumulSaleAndBill.Customer.CustomerFullName;
                    cs.PhoneNumber = cs.CumulSaleAndBill.Customer.AdressPhoneNumber;
                    cs.CustomerValue = cs.CumulSaleAndBill.Customer.CustomerValueUI;
                }

                if (cs.CumulSaleAndBill.CustomerOrderID != null)
                {
                    // En attendant la migration de CustomerId dans CustomerOrder pour les Insured Customer
                    cs.Customer = cs.CumulSaleAndBill.CustomerOrder.CustomerName;
                    cs.PhoneNumber = cs.CumulSaleAndBill.CustomerOrder.PhoneNumber;
                    cs.CustomerValue = cs.CumulSaleAndBill.CustomerOrder.CustomerValueUI;
                }

                cs.DisplaySaleDate = cs.SaleDate.ToString("yyyy-MM-dd");
                cs.DisplayDate = cs.OperationDate.ToString("yyyy-MM-dd");
                cs.IsSatisfiedDisplay = cs.IsSatisfied ? "YES" : "NO";
                cs.CumulSaleAndBill = null;
            });

            Session["customerSatisfactions"] = customerSatisfactions;

            var model = new
            {
                data = customerSatisfactions
            };
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public DateTime getOperationDate()
        {
            listBDUser = (List<BusinessDay>)Session["UserBusDays"];

            if (listBDUser == null)
            {
                listBDUser = _busDayRepo.GetOpenedBusinessDay(CurrentUser);
            }
            return listBDUser.FirstOrDefault().BDDateOperation;
        }
    }
}