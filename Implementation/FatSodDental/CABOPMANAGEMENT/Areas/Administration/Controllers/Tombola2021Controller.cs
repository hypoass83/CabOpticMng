using FastSod.Utilities.Util;
using FatSod.DataContext.Initializer;
using FatSod.Ressources;
using FatSod.Security.Abstracts;
using FatSod.Security.Entities;
using FatSod.Supply.Abstracts;
using FatSod.Supply.Entities;
using CABOPMANAGEMENT.Controllers;
using CABOPMANAGEMENT.Filters;
using CABOPMANAGEMENT.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using FatSod.Report.WrapReports;

namespace CABOPMANAGEMENT.Areas.Administration.Controllers
{
    public class Tombola2021Controller : BaseController
    {
        // GET: Administration/Tombola2021
        public ActionResult Index()
        {
            var StartDate = new DateTime(2021, 1, 1);
            var EndDate = new DateTime(2021, 12, 6);
            return View(/*GetEligibleCustomers(StartDate, EndDate)*/);
        }


        public JsonResult PendingCustomerSale(/*DateTime StartDate, DateTime EndDate*/)
        {
            var StartDate = new DateTime(2021, 1, 1);
            var EndDate = new DateTime(2021, 12, 6);
            var res = new List<Customer>();

            var sales = db.vcumulRealSales.Where(s => s.SaleDate >= StartDate &&
                                                      s.SaleDate <= EndDate && s.Advanced >= 30000)
                .ToList();
            sales.ForEach(sale =>
            {
                dynamic saleInfo = GetSaleInfo(sale.SaleID);
                res.Add(
                new Customer
                {
                    PhoneNumber = saleInfo.PhoneNumber,
                    PreferredLanguage = saleInfo.PreferredLanguage,
                });
            });

            var orders = db.CustomerOrders.Where(sa => sa.ValidateBillDate >= StartDate &&
                                                       sa.ValidateBillDate <= EndDate &&
                                                       sa.BillState == StatutFacture.Validated).ToList();

            orders.ForEach(order =>
            {
                dynamic orderInfo = GetOrderInfo(order);
                res.Add(
                new Customer
                {
                    PhoneNumber = orderInfo.PhoneNumber,
                    PreferredLanguage = orderInfo.PreferredLanguage,
                });
            });

            var model = new
            {
                data = res
            };

            var jsonResult = Json(model, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = Int32.MaxValue;
            return jsonResult;
        }

        private dynamic GetSaleInfo(int saleID)
        {
            var sale = db.Sales.Find(saleID);

            return new
            {
                PhoneNumber = FormatPhoneNumber(sale.AdressPhoneNumber),
                PreferredLanguage = sale.Customer.PreferredLanguage != null ? sale.Customer.PreferredLanguage : "FR"
            };
        }


        private dynamic GetOrderInfo(CustomerOrder order)
        {
            var phoneNumber = "";
            var preferredLanguage = "";
            if(order.Customer == null)
            {
                phoneNumber = FormatPhoneNumber(order.PhoneNumber);
                preferredLanguage = "FR";
            } else
            {
                phoneNumber = FormatPhoneNumber(order.Customer.AdressPhoneNumber);
                preferredLanguage = order.Customer.PreferredLanguage;

            }

            return new
            {
                PhoneNumber = phoneNumber,
                PreferredLanguage = preferredLanguage
            };
        }

        public string FormatPhoneNumber(string phoneNumber)
        {

            if (String.IsNullOrEmpty(phoneNumber))
                return "BAD-" + phoneNumber;

            var res = phoneNumber;
            var resInt = 0;

            res = res.Trim();

            if (res.Contains("-"))
                res = res.Remove('-');

            if (res.Contains("*"))
                res = res.Remove('*');

            if (res.Length < 9 || !int.TryParse(res, out resInt) || res[0] != '6')
                return "BAD-" + res;

            return res;
        }
    }
}