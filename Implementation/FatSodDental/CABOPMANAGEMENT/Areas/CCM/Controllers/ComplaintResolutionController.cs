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
using CABOPMANAGEMENT.Tools;

namespace CABOPMANAGEMENT.Areas.CCM.Controllers
{
    public class ComplaintResolutionController : BaseController
    {
        //Current Controller and current page
        private const string CONTROLLER_NAME = "CCM/ComplainRegistration";
        private const string VIEW_NAME = "Index";

        private ICustomerComplaint _customerComplaintRepository;

        private IBusinessDay _busDayRepo;

        private List<BusinessDay> listBDUser;

        public ComplaintResolutionController(
            IBusinessDay busDayRepo,
            ICustomerComplaint CustomerComplaintRepository
            )
        {
            this._customerComplaintRepository = CustomerComplaintRepository;
            this._busDayRepo = busDayRepo;
        }

        // GET: CCM/CustomerComplaint
        public ActionResult Index() // 1 => Resolve et 2 => Control
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
            var profile = (int)Session["UserProfile"];

            startDate = startDate.AddHours(-startDate.Hour);
            startDate = startDate.AddMinutes(-startDate.Minute);
            startDate = startDate.AddSeconds(-startDate.Second);

            endDate = endDate.AddHours(-endDate.Hour);
            endDate = endDate.AddMinutes(-endDate.Minute);
            endDate = endDate.AddSeconds(-endDate.Second);

            endDate = endDate.AddHours(23);
            endDate = endDate.AddMinutes(59);
            endDate = endDate.AddSeconds(59);


            List<CustomerComplaint> customerComplaints = getGeneralSales(busDay.branchId, startDate, endDate, busDay.operationDate, profile);

            Session["customerComplaints"] = customerComplaints;

            var model = new
            {
                data = customerComplaints
            };
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ResolveCustomerComplaint(CustomerComplaint customerComplaint)
        {
            bool status = false;
            string Message = "";
            try
            {
                CustomerComplaint selectedCustomerComplaint = db.CustomerComplaints.Find(customerComplaint.CustomerComplaintId);
                
                #region Heure:Minute:Seconde de la date de resolution de la plainte

                listBDUser = (List<BusinessDay>)Session["UserBusDays"];
                if (listBDUser == null)
                {
                    listBDUser = _busDayRepo.GetOpenedBusinessDay(CurrentUser);
                }
                dynamic busDay = getBusinessDay();
                selectedCustomerComplaint.ResolvedDate = busDay.operationDate;
                DateTime currentDate = DateTime.Now;

                // Suppression si existant
                selectedCustomerComplaint.ResolvedDate = selectedCustomerComplaint.ResolvedDate.Value.AddHours(-selectedCustomerComplaint.ResolvedDate.Value.Hour);
                selectedCustomerComplaint.ResolvedDate = selectedCustomerComplaint.ResolvedDate.Value.AddMinutes(-selectedCustomerComplaint.ResolvedDate.Value.Minute);
                selectedCustomerComplaint.ResolvedDate = selectedCustomerComplaint.ResolvedDate.Value.AddSeconds(-selectedCustomerComplaint.ResolvedDate.Value.Second);

                selectedCustomerComplaint.ResolvedDate = selectedCustomerComplaint.ResolvedDate.Value.AddHours(currentDate.Hour);
                selectedCustomerComplaint.ResolvedDate = selectedCustomerComplaint.ResolvedDate.Value.AddMinutes(currentDate.Minute);
                selectedCustomerComplaint.ResolvedDate = selectedCustomerComplaint.ResolvedDate.Value.AddSeconds(currentDate.Second);
                #endregion

                // Infos remplies par l'utilisateur
                User resolver = db.Users.FirstOrDefault(u => u.GlobalPersonID == customerComplaint.ComplaintResolverId);
                selectedCustomerComplaint.IsSolved = customerComplaint.IsSolved;
                selectedCustomerComplaint.AddResolverComment(customerComplaint.ResolverComment,
                    resolver.UserFullName,
                    selectedCustomerComplaint.ResolvedDate.Value.ToString("yyyy-MM-dd HH:mm:ss"));
                // selectedCustomerComplaint.ResolverComment = customerComplaint.ResolverComment;
                selectedCustomerComplaint.ComplaintResolverId = customerComplaint.ComplaintResolverId;

                if (customerComplaint.IsSolved == false)
                {
                    selectedCustomerComplaint.ResolvedDate = null;
                    selectedCustomerComplaint.ComplaintResolverId = null;
                    selectedCustomerComplaint.ComplaintQuotationId = 0;
                    selectedCustomerComplaint.ComplaintQuotation = null;
                    selectedCustomerComplaint.ComplaintQuotationId = customerComplaint.ComplaintQuotationId;
                    Message = Resources.Success + " Customer Complaint Has Been Successfuly Quotated";

                }

                if (customerComplaint.IsSolved == true)
                {
                    Message = Resources.Success + " Customer Complaint Has Been Successfuly Resolved";
                }

                if (customerComplaint.CustomerComplaintId == 0)
                {
                    status = false;
                    Message = "Error: Please select operation ";
                    return new JsonResult { Data = new { status = status, Message = Message } };
                } 
                db.SaveChanges();
                status = true;
            }
            catch (Exception e)
            {
                status = false;
                Message = "Error " + e.Message + " " + e.StackTrace + " " + e.InnerException;
            }
            return new JsonResult { Data = new { status = status, Message = Message } };
        }


        public List<CustomerComplaint> getGeneralSales(int branchId, DateTime startDate, DateTime endDate, DateTime operationDate, int profileId)
        {
            List<CustomerComplaint> customerComplaints = new List<CustomerComplaint>();

            List<CustomerComplaint> complaints = db.CustomerComplaints.Where(cpt => 
                cpt.ComplaintQuotationId == profileId && cpt.ResolvedDate == null && cpt.RegistrationDate >= startDate &&
                cpt.RegistrationDate <= endDate).ToList();

            complaints.ForEach(complaint =>
            {
                CumulSaleAndBill sale = complaint.CumulSaleAndBill;
                string customerType = "CASH";
                string customerValue = "";
                if (sale != null && sale.SaleID != null)
                {
                    complaint.Customer = complaint.Customer != null ? complaint.Customer :
                                                                sale.Customer.CustomerFullName;
                    complaint.PhoneNumber = complaint.PhoneNumber != null ? complaint.PhoneNumber : 
                                                                  sale.Customer.AdressPhoneNumber;
                    customerValue = sale.Customer.CustomerValueUI;
                }

                if (sale != null && sale.CustomerOrderID != null)
                {
                    // En attendant la migration de CustomerId dans CustomerOrder pour les Insured Customer
                    complaint.Customer = complaint.Customer != null ? complaint.Customer :
                                                                sale.CustomerOrder.CustomerName;
                    complaint.PhoneNumber = complaint.PhoneNumber != null ? complaint.PhoneNumber :
                                                                  sale.CustomerOrder.PhoneNumber;
                    customerType = "INSURED";
                    complaint.Insurance = complaint.Insurance != null ? complaint.Insurance :
                                                              sale.CustomerOrder.Assureur.Name;
                    complaint.InsuredCompany = complaint.InsuredCompany != null ? complaint.InsuredCompany :
                               sale.CustomerOrder.InsuredCompany.InsuredCompanyLabel;
                    customerValue = sale.CustomerOrder.CustomerValueUI;
                }
                CustomerComplaint customerComplaint = new CustomerComplaint
                {
                    DisplayDate = complaint.RegistrationDate.ToString("yyyy-MM-dd"),
                    Customer = complaint.Customer,
                    Complaint = complaint.Complaint,
                    CustomerValue = customerValue,
                    CumulSaleAndBillID = complaint.CumulSaleAndBillID,
                    CustomerComplaintId = complaint.CustomerComplaintId,
                    ComplaintQuotationId = complaint.ComplaintQuotationId,
                    PreviousComment = complaint.ResolverComment,
                    Occurrences = db.CustomerComplaints.Count(cc => cc.CumulSaleAndBillID == complaint.CumulSaleAndBillID)
                };
                customerComplaints.Add(customerComplaint);

            });
            return customerComplaints;
        }

        public JsonResult GetResolvers(int profileId)
        {
            List<object> users = LoadComponent.GetAllEmployeesByProfile(CurrentBranch.BranchID, profileId);
            return Json(users, JsonRequestBehavior.AllowGet);
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

            profiles = (from profile in profiles
                        where ((profile.ProfileCode != "Super-Admin-FSInventory" &&
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

        public JsonResult GetSaleDetails(int CustomerComplaintId)
        {
            List<CustomerComplaint> CustomerComplaints = ((List<CustomerComplaint>)Session["customerComplaints"]);

            CustomerComplaint selectedCustomerComplaint = CustomerComplaints.SingleOrDefault(cs => cs.CustomerComplaintId == CustomerComplaintId);

            return Json(selectedCustomerComplaint, JsonRequestBehavior.AllowGet);
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