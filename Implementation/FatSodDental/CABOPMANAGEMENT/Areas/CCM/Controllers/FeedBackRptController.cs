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
    public class FeedBackRptController : BaseController
    {
        //Current Controller and current page
        private const string CONTROLLER_NAME = "CCM/ComplainRegistration";
        private const string VIEW_NAME = "Index";

        private IComplaintFeedBack _complaintFeedBackRepository;

        private IBusinessDay _busDayRepo;

        private List<BusinessDay> listBDUser;

        public FeedBackRptController(
            IBusinessDay busDayRepo,
            IComplaintFeedBack complaintFeedBackRepository
            )
        {
            this._complaintFeedBackRepository = complaintFeedBackRepository;
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

        public JsonResult GetComplaintFeedBacks(DateTime startDate, DateTime endDate)
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

            List<ComplaintFeedBack> complaintFeedBacks = getGeneralSales(busDay.branchId, startDate, endDate, busDay.operationDate, profile);

            Session["ComplaintFeedBacks"] = complaintFeedBacks;

            var model = new
            {
                data = complaintFeedBacks
            };
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AddComplaintFeedBack(ComplaintFeedBack complaintFeedBack)
        {
            bool status = false;
            string Message = "";
            try
            {
                List<ComplaintFeedBack> ComplaintFeedBacks = ((List<ComplaintFeedBack>)Session["ComplaintFeedBacks"]);

                ComplaintFeedBack selectedComplaintFeedBack = ComplaintFeedBacks.FirstOrDefault
                    (cfb => cfb.CustomerComplaintId == complaintFeedBack.CustomerComplaintId);

                // Infos remplies par l'utilisateur
                selectedComplaintFeedBack.Comment = complaintFeedBack.Comment;
                selectedComplaintFeedBack.ContactChannel = complaintFeedBack.ContactChannel;
                selectedComplaintFeedBack.IsSatisfied = complaintFeedBack.IsSatisfied;

                if (selectedComplaintFeedBack.CustomerComplaintId == 0)
                {
                    status = false;
                    Message = "Error: Please select operation ";
                    return new JsonResult { Data = new { status = status, Message = Message } };
                }
                db.ComplaintFeedBacks.Add(selectedComplaintFeedBack);
                db.SaveChanges();
                status = true;
                Message = Resources.Success + " Customer Complaint Has Been Successfuly Controlled";
            }
            catch (Exception e)
            {
                status = false;
                Message = "Error " + e.Message + " " + e.StackTrace + " " + e.InnerException;
            }
            return new JsonResult { Data = new { status = status, Message = Message } };
        }

        public List<ComplaintFeedBack> getGeneralSales(int branchId, DateTime startDate, DateTime endDate, DateTime operationDate, int profileId)
        {
            List<ComplaintFeedBack> ComplaintFeedBacks = new List<ComplaintFeedBack>();

            List<ComplaintFeedBack> feedBacks = db.ComplaintFeedBacks.Where(cpt =>
                                                 cpt.OperationDate >= startDate &&
                                                 cpt.OperationDate <= endDate).ToList();

            feedBacks.ForEach(feedBack =>
            {
                CumulSaleAndBill sale = feedBack.CustomerComplaint.CumulSaleAndBill;
                string customerType = "CASH";
                string customerValue = "";
                if (sale != null && sale.SaleID != null)
                {
                    feedBack.CustomerComplaint.Customer = feedBack.CustomerComplaint.Customer != null ? feedBack.CustomerComplaint.Customer :
                                                                sale.Customer.CustomerFullName;
                    feedBack.CustomerComplaint.PhoneNumber = feedBack.CustomerComplaint.PhoneNumber != null ? feedBack.CustomerComplaint.PhoneNumber :
                                                                  sale.Customer.AdressPhoneNumber;
                    customerValue = sale.Customer.CustomerValueUI;
                }

                if (sale != null && sale.CustomerOrderID != null)
                {
                    // En attendant la migration de CustomerId dans CustomerOrder pour les Insured Customer
                    feedBack.CustomerComplaint.Customer = feedBack.CustomerComplaint.Customer != null ? feedBack.CustomerComplaint.Customer :
                                                                sale.CustomerOrder.CustomerName;
                    feedBack.CustomerComplaint.PhoneNumber = feedBack.CustomerComplaint.PhoneNumber != null ? feedBack.CustomerComplaint.PhoneNumber :
                                                                  sale.CustomerOrder.PhoneNumber;
                    customerType = "INSURED";
                    feedBack.CustomerComplaint.Insurance = feedBack.CustomerComplaint.Insurance != null ? feedBack.CustomerComplaint.Insurance :
                                                              sale.CustomerOrder.Assureur.Name;
                    feedBack.CustomerComplaint.InsuredCompany = feedBack.CustomerComplaint.InsuredCompany != null ? feedBack.CustomerComplaint.InsuredCompany :
                               sale.CustomerOrder.InsuredCompany.InsuredCompanyLabel;
                    customerValue = sale.CustomerOrder.CustomerValueUI;
                }
                string state = "Registered";
                state = feedBack.CustomerComplaint.ComplaintResolverId != null ? "Solved" : state;
                state = feedBack.CustomerComplaint.ComplaintControllerId != null ? "Controlled" : state;

                ComplaintFeedBack complaintFeedBack = new ComplaintFeedBack
                {
                    DisplayDate = feedBack.OperationDate.ToString("yyyy-MM-dd"),
                    PhoneNumber = feedBack.CustomerComplaint.PhoneNumber,
                    Customer = feedBack.CustomerComplaint.Customer,
                    Comment = feedBack.Comment,
                    Complaint = feedBack.CustomerComplaint.Complaint,
                    OperationDate = feedBack.OperationDate,
                    OperatorID = feedBack.OperatorID,
                    ResolverComment = feedBack.CustomerComplaint.ResolverComment,
                    CustomerValue = customerValue,
                    CustomerComplaintId = feedBack.CustomerComplaintId,
                    CustomerType = customerType,
                    Insurance = feedBack.CustomerComplaint.Insurance,
                    InsuredCompany = feedBack.CustomerComplaint.InsuredCompany,
                    ComplaintFeedBackId = feedBack.ComplaintFeedBackId,
                    IsSatisfiedDisplay = feedBack.IsSatisfied ? "YES" : "NO"
                };
                ComplaintFeedBacks.Add(complaintFeedBack);

            });
            return ComplaintFeedBacks;
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