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
    public class ComplaintRegistrationController : BaseController
    {
        //Current Controller and current page
        private const string CONTROLLER_NAME = "CCM/ComplainRegistration";
        private const string VIEW_NAME = "Index";

        private ICustomerComplaint _customerComplaintRepository;

        private IBusinessDay _busDayRepo;

        private List<BusinessDay> listBDUser;

        public ComplaintRegistrationController(
            IBusinessDay busDayRepo,
            ICustomerComplaint CustomerComplaintRepository
            )
        {
            this._customerComplaintRepository = CustomerComplaintRepository;
            this._busDayRepo = busDayRepo;
        }

        // GET: CCM/CustomerComplaint
        public ActionResult Index()
        {
            Session["Curent_Page"] = VIEW_NAME;
            Session["Curent_Controller"] = CONTROLLER_NAME;
            dynamic busDay = getBusinessDay();
            ViewBag.OperationDate = busDay.operationDate.ToString("yyyy-MM-dd");
            return View();
        }

        public JsonResult getCustomerComplaints(DateTime startDate, DateTime endDate)
        {
            listBDUser = (List<BusinessDay>)Session["UserBusDays"];
            if (listBDUser == null)
            {
                listBDUser = _busDayRepo.GetOpenedBusinessDay(CurrentUser);
            }
            dynamic busDay = getBusinessDay();
            List<CustomerComplaint> customerComplaints = getGeneralSales(busDay.branchId, startDate, endDate, busDay.operationDate);

            Session["customerComplaints"] = customerComplaints;

            var model = new
            {
                data = customerComplaints
            };
            var jsonResult = Json(model, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        public JsonResult GetSaleDetails(int CumulSaleAndBillID)
        {
            List<CustomerComplaint> CustomerComplaints = ((List<CustomerComplaint>)Session["CustomerComplaints"]);

            CustomerComplaint selectedCustomerComplaint = CustomerComplaints.SingleOrDefault(cs => cs.CumulSaleAndBillID == CumulSaleAndBillID);
            
            return Json(selectedCustomerComplaint, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AddCustomerComplaint(CustomerComplaint customerComplaint)
        {
            bool status = false;
            string Message = "";
            try
            {
                CustomerComplaint selectedCustomerComplaint = null;
                if (customerComplaint.CumulSaleAndBillID == 0 && customerComplaint.IsCustomerNotFound == false)
                {
                    status = false;
                    Message = "Error: Please select operation ";
                    return new JsonResult { Data = new { status = status, Message = Message } };
                }

                List<CustomerComplaint> CustomerComplaints = ((List<CustomerComplaint>)Session["CustomerComplaints"]);

                selectedCustomerComplaint = CustomerComplaints.SingleOrDefault(cs => cs.CumulSaleAndBillID == customerComplaint.CumulSaleAndBillID);

                if (selectedCustomerComplaint == null && customerComplaint.IsCustomerNotFound == true)
                {
                    selectedCustomerComplaint = customerComplaint;
                    dynamic busDay = getBusinessDay();
                    selectedCustomerComplaint.RegistrationDate = busDay.operationDate;
                    selectedCustomerComplaint.CumulSaleAndBillID = null;
                } else
                {
                    selectedCustomerComplaint.PhoneNumber = customerComplaint.PhoneNumber;
                    selectedCustomerComplaint.Insurance = customerComplaint.Insurance;
                    selectedCustomerComplaint.InsuredCompany = customerComplaint.InsuredCompany;
                    selectedCustomerComplaint.Customer = customerComplaint.Customer;
                    selectedCustomerComplaint.PurchaseDate = customerComplaint.PurchaseDate;

                    selectedCustomerComplaint.IsCashCustomer = customerComplaint.IsCashCustomer;
                    selectedCustomerComplaint.IsCashOtherCustomer = customerComplaint.IsCashOtherCustomer;
                    selectedCustomerComplaint.IsInsuredCustomer = customerComplaint.IsInsuredCustomer;


                }

                // Infos remplies par l'utilisateur
                selectedCustomerComplaint.Complaint = customerComplaint.Complaint;
                selectedCustomerComplaint.ComplaintQuotationId = customerComplaint.ComplaintQuotationId;

                #region Heure:Minute:Seconde de la date d'enregistrement de la plainte
                DateTime currentDate = DateTime.Now;

                // Suppression si existant
                selectedCustomerComplaint.RegistrationDate = selectedCustomerComplaint.RegistrationDate.AddHours(-selectedCustomerComplaint.RegistrationDate.Hour);
                selectedCustomerComplaint.RegistrationDate = selectedCustomerComplaint.RegistrationDate.AddMinutes(-selectedCustomerComplaint.RegistrationDate.Minute);
                selectedCustomerComplaint.RegistrationDate = selectedCustomerComplaint.RegistrationDate.AddSeconds(-selectedCustomerComplaint.RegistrationDate.Second);

                selectedCustomerComplaint.RegistrationDate = selectedCustomerComplaint.RegistrationDate.AddHours(currentDate.Hour);
                selectedCustomerComplaint.RegistrationDate = selectedCustomerComplaint.RegistrationDate.AddMinutes(currentDate.Minute);
                selectedCustomerComplaint.RegistrationDate = selectedCustomerComplaint.RegistrationDate.AddSeconds(currentDate.Second);
                #endregion

                
                _customerComplaintRepository.Create(selectedCustomerComplaint);
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


        public List<CustomerComplaint> getGeneralSales(int branchId, DateTime startDate, DateTime endDate, DateTime operationDate)
        {
            List<CustomerComplaint> customerComplaints = new List<CustomerComplaint>();

            List<CumulSaleAndBill> sales = db.CumulSaleAndBills.Where(csb => csb.SaleDate >= startDate &&
                                                                             csb.SaleDate <= endDate /*&&
                                                                             csb.IsProductDeliver &&*/
                /*!db.CustomerSatisfactions.Any(cs => cs.CumulSaleAndBillID == csb.CumulSaleAndBillID)*/).ToList();

            sales.ForEach(sale =>
            {
                string customerName = "";
                string phoneNumber = "";
                string customerType = "CASH";
                string insurance = "";
                string company = "";
                string customerValue = "";
                bool IsInHouseCustomer = false;
                if (sale.SaleID != null && sale.Customer != null)
                {
                    customerName = sale.Customer.CustomerFullName;
                    phoneNumber = sale.Customer.AdressPhoneNumber;
                    customerValue = sale.Customer.CustomerValueUI;
                    IsInHouseCustomer = sale.Customer.IsInHouseCustomer;
                }

                if (sale.CustomerOrderID != null)
                {
                    // En attendant la migration de CustomerId dans CustomerOrder pour les Insured Customer
                    customerName = sale.CustomerOrder.CustomerName;
                    phoneNumber = sale.CustomerOrder.PhoneNumber;
                    customerType = "INSURED";
                    insurance = sale.CustomerOrder.Assureur == null ? "" : sale.CustomerOrder.Assureur.Name;
                    company = sale.CustomerOrder.InsuredCompany == null ? "" : 
                              sale.CustomerOrder.InsuredCompany.InsuredCompanyLabel;
                    customerValue = sale.CustomerOrder.CustomerValueUI;
                }
                CustomerComplaint satisfaction = new CustomerComplaint
                {
                    DisplayDate = sale.SaleDate == null ? "" : sale.SaleDate.ToString("yyyy-MM-dd"),
                    Customer = customerName,
                    PhoneNumber = phoneNumber,
                    CumulSaleAndBillID = sale.CumulSaleAndBillID,
                    RegistrationDate = operationDate,
                    // SaleDate = sale.SaleDate, // new DateTime(sale.SaleDate.Millisecond),
                    Complaint = "",
                    CustomerComplaintId = 0,
                    ComplaintQuotationId = 0,
                    CustomerType = customerType,
                    Insurance = insurance,
                    InsuredCompany = company,
                    CustomerValue = customerValue,
                    IsCashCustomer = IsInHouseCustomer && customerType == "CASH",
                    IsCashOtherCustomer = !IsInHouseCustomer && customerType == "CASH",
                    IsInsuredCustomer = customerType == "INSURED",
                    Occurrences = db.CustomerComplaints.Count(cc => cc.CumulSaleAndBillID == sale.CumulSaleAndBillID)
                };

                if (sale.SaleID != null)
                {
                    customerComplaints.Add(satisfaction);
                }

                if (sale.CustomerOrderID != null && sale.CustomerOrder.BillState == StatutFacture.Validated)
                {
                    customerComplaints.Add(satisfaction);
                }


            });
            return customerComplaints;
        }


        public JsonResult GetProfiles(int occurrences)
        {
            List<Profile> profiles = new List<Profile>();
            if (occurrences == 0)
            {
                profiles = db.Profiles.AsNoTracking().Where(p => p.PofilLevel <= 4).ToList();
            }

            if (occurrences >= 1)
            {
                profiles = db.Profiles.Where(p => p.PofilLevel >= 4).ToList();
            }

            profiles = (from profile in profiles where ((profile.ProfileCode != "Super-Admin-FSInventory" &&
                                                         profile.ProfileCode != "admin-Dental" &&
                                                         profile.ProfileLabel != "CUSTOMER RELATION MANAGER" &&
                                                         profile.ProfileLabel != "Employé" &&
                                                         profile.ProfileLabel != "WareHouse"))
            select new Profile
            {
                ProfileID = profile.ProfileID,
                ProfileLabel = profile.ProfileLabel
            }).ToList();

            profiles = profiles.OrderBy(p => p.ProfileLabel).ToList();

            return Json(profiles, JsonRequestBehavior.AllowGet);
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