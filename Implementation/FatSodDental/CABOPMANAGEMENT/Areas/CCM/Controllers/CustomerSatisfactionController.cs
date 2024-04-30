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

namespace CABOPMANAGEMENT.Areas.CCM.Controllers
{
    public class CustomerSatisfactionController : BaseController
    {
        //Current Controller and current page
        private const string CONTROLLER_NAME = "CRM/CustomerSatisfaction";
        private const string VIEW_NAME = "Index";

        private ICustomerSatisfaction _CustomerSatisfactionRepository;

        private IBusinessDay _busDayRepo;

        private List<BusinessDay> listBDUser;

        public CustomerSatisfactionController(
            IBusinessDay busDayRepo,
            ICustomerSatisfaction CustomerSatisfactionRepository
            )
        {
            this._CustomerSatisfactionRepository = CustomerSatisfactionRepository;
            this._busDayRepo = busDayRepo;
        }

        // GET: CRM/CustomerSatisfaction
        public ActionResult Index()
        {
            Session["Curent_Page"] = VIEW_NAME;
            Session["Curent_Controller"] = CONTROLLER_NAME;
            dynamic busDay = getBusinessDay();
            ViewBag.OperationDate = busDay.operationDate.ToString("yyyy-MM-dd");
            return View();
        }

        public JsonResult getCustomerSatisfactions(DateTime startDate, DateTime endDate)
        {
            listBDUser = (List<BusinessDay>)Session["UserBusDays"];
            if (listBDUser == null)
            {
                listBDUser = _busDayRepo.GetOpenedBusinessDay(CurrentUser);
            }
            dynamic busDay = getBusinessDay();
            List<CustomerSatisfaction> customerSatisfactions = getGeneralSales(busDay.branchId, startDate, endDate, busDay.operationDate);

            Session["customerSatisfactions"] = customerSatisfactions;

            var model = new
            {
                data = customerSatisfactions
            };
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AddCustomerSatisfaction(CustomerSatisfaction customerSatisfaction)
        {
            bool status = false;
            string Message = "";
            try
            {
                List<CustomerSatisfaction> CustomerSatisfactions = ((List<CustomerSatisfaction>)Session["CustomerSatisfactions"]);

                CustomerSatisfaction selectedCustomerSatisfaction = CustomerSatisfactions.SingleOrDefault(cs => cs.CumulSaleAndBillID == customerSatisfaction.CumulSaleAndBillID);
                customerSatisfaction.OperationDate = selectedCustomerSatisfaction.OperationDate;
                customerSatisfaction.SaleDate = selectedCustomerSatisfaction.SaleDate;

                if (customerSatisfaction.CumulSaleAndBillID == 0)
                {
                    status = false;
                    Message = "Error: Please select operation ";
                    return new JsonResult { Data = new { status = status, Message = Message } };
                }
                _CustomerSatisfactionRepository.Create(customerSatisfaction);
                status = true;
                Message = Resources.Success + " Customer Satisfaction Has been created successfuly";
            }
            catch (Exception e)
            {
                status = false;
                Message = "Error " + e.Message + " " + e.StackTrace + " " + e.InnerException;
            }
            return new JsonResult { Data = new { status = status, Message = Message } };
        }


        public List<CustomerSatisfaction> getGeneralSales(int branchId, DateTime startDate, DateTime endDate, DateTime operationDate)
        {
            List<CustomerSatisfaction> customerSatisfactions = new List<CustomerSatisfaction>();

            List<CumulSaleAndBill> sales = db.CumulSaleAndBills.Where(csb => csb.SaleDate >= startDate &&
                                                                             csb.SaleDate <= endDate &&
                                                                             /*csb.IsProductDeliver &&*/
                !db.CustomerSatisfactions.Any(cs => cs.CumulSaleAndBillID == csb.CumulSaleAndBillID)).ToList();

            sales.ForEach(sale =>
            {
                string customerName = "";
                string phoneNumber = "";
                string customerType = "CASH";
                string insurance = "";
                string company = "";
                string customerValue = "";
                if(sale.SaleID != null)
                {
                    customerName = sale.Customer.CustomerFullName;
                    phoneNumber = sale.Customer.AdressPhoneNumber;
                    customerValue = sale.Customer.CustomerValueUI;
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
                }
                CustomerSatisfaction satisfaction = new CustomerSatisfaction
                {
                    DisplayDate = sale.SaleDate.ToString("yyyy-MM-dd"),
                    Customer = customerName,
                    PhoneNumber = phoneNumber,
                    CumulSaleAndBillID = sale.CumulSaleAndBillID,
                    OperationDate = operationDate,
                    SaleDate = sale.SaleDate,
                    IsSatisfied = false,
                    Comment = "", //getTypeOperation(sale.CumulSaleAndBillID), //"",
                    ContactChannel = "",
                    CustomerSatisfactionId = 0,
                    CustomerType = customerType,
                    Insurance = insurance,
                    InsuredCompany = company,
                    CustomerValue = customerValue,
                    DeliveryDate = sale.ProductDeliverDate.HasValue ? sale.ProductDeliverDate.Value.ToString("yyyy-MM-dd") : "RAS",
                    IsDelivered = sale.ProductDeliverDate.HasValue ? "YES" : "NO"

                };

                /*if (sale.SaleID != null)
                {
                    customerSatisfactions.Add(satisfaction);
                }

                if (sale.CustomerOrderID != null && sale.CustomerOrder.BillState == StatutFacture.Validated)
                {
                    customerSatisfactions.Add(satisfaction);
                }*/

                customerSatisfactions.Add(satisfaction);

            });
            return customerSatisfactions;
        }

        private string getTypeOperation(int CumulSaleAndBillID)
        {
            string summary = "";
            
            //accessories
            List<Category> listCategory = db.Categories.Where(cat => !(cat is LensCategory) && cat.CategoryCode != "RECAPPRODUCT"
            && db.CumulSaleAndBillLines.Any(l=>l.Product.CategoryID == cat.CategoryID && l.CumulSaleAndBillID== CumulSaleAndBillID)).ToList();
            if (listCategory.Count()>0)
            {
                summary = "ACCESSORIES";
            }
            //DILATION
            List<Category> listDILATION = db.Categories.Where(cat => !(cat is LensCategory) && !(cat.isSerialNumberNull) && cat.CategoryCode.ToUpper() == "DILATATION"
            && db.CumulSaleAndBillLines.Any(l => l.Product.CategoryID == cat.CategoryID && l.CumulSaleAndBillID == CumulSaleAndBillID)).ToList();
            if (listDILATION.Count() > 0)
            {
                summary = "DILATION";
            }

            //Frame
            List<Category> listFrame = db.Categories.Where(cat => !(cat is LensCategory) && !(cat.isSerialNumberNull) && (cat.CategoryCode.ToUpper() != "RECAPPRODUCT" && cat.CategoryCode.ToUpper() != "DILATATION")
            && db.CumulSaleAndBillLines.Any(l => l.Product.CategoryID == cat.CategoryID && l.CumulSaleAndBillID == CumulSaleAndBillID)).ToList();
            

            //lens
            List<Category> listLens = db.Categories.Where(cat => (cat is LensCategory) 
            && db.CumulSaleAndBillLines.Any(l => l.Product.CategoryID == cat.CategoryID && l.CumulSaleAndBillID == CumulSaleAndBillID)).ToList();
            
            if (listFrame.Count() > 0)
            {
                if (listLens.Count() > 0)
                {
                    summary = "LENS/FRAME";
                }
                else
                {
                    summary = "FRAME";
                }
            }
            else
            {
                if (listLens.Count() > 0)
                {
                    summary = "LENS";
                }
                else
                {
                    summary = "";
                }
            }

            //summary

            return summary;
        }

        public object getBusinessDay()
        {
            listBDUser = (List<BusinessDay>)Session["UserBusDays"];

            if (listBDUser == null)
            {
                listBDUser = _busDayRepo.GetOpenedBusinessDay(CurrentUser);
            }
            DateTime operationDate = listBDUser.FirstOrDefault().BDDateOperation;
            int branchId = listBDUser.FirstOrDefault().BranchID;
            return new { operationDate = operationDate, branchId = branchId };
        }
    }
}