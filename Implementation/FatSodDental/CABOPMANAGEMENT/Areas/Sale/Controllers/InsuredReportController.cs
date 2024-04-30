using FatSod.DataContext.Repositories;
using FatSod.Security.Abstracts;
using FatSod.Security.Entities;
using FatSod.Supply.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CABOPMANAGEMENT.Controllers;
using CABOPMANAGEMENT.Filters;

namespace CABOPMANAGEMENT.Areas.Sale.Controllers
{
    [Authorize(Order = 1)]
    [TakeBusinessDay(Order = 2)]
    public class InsuredReportController : BaseController
    {
        private IBusinessDay _busDayRepo;


        public InsuredReportController(
                 IBusinessDay busDayRepo
                )
        {
            this._busDayRepo = busDayRepo;
        }
        

        [OutputCache(Duration = 3600)]
        // GET: Sale/InsuredReport
        public ActionResult Index()
        {
            ViewBag.DisplayForm = 1;
            try
            {

                //we ensure that if this user manage cash register. If he manage it, will verify if till is closed else, we ask he to closed it before login off

                List<BusinessDay> UserBusDays = (List<BusinessDay>)Session["UserBusDays"];
                DateTime currentDateOp = UserBusDays.FirstOrDefault().BDDateOperation;
                ViewBag.CurrentBranch = UserBusDays.FirstOrDefault().BranchID;
                ViewBag.BusnessDayDate = UserBusDays.FirstOrDefault().BDDateOperation.ToString("yyyy-MM-dd");// businessDay.BDDateOperation;


                //ViewBag.CurrentTill = userTill.TillID;

                return View();
            }

            catch (Exception e)
            {
                TempData["Message"] = "Error " + e.Message;
                ViewBag.DisplayForm = 0;
                return this.View();
            }
        }

        public JsonResult chargeSolde( DateTime Begindate, DateTime EndDate,int MarketerID=0)
        {
            List<object> _InfoList = new List<object>();
            
            double TotMntFacture = 0d;
            db.Database.CommandTimeout = 0;
            List<V_Summary_Insured_Bill> listBillInsuredOp = new List<V_Summary_Insured_Bill>();
            if (MarketerID == 0)
            {
                listBillInsuredOp = db.V_Summary_Insured_Bill.Where(co => (co.CustomerOrderDate >= Begindate && co.CustomerOrderDate <= EndDate)).ToList();
            }
            else
            {
                listBillInsuredOp = db.V_Summary_Insured_Bill.Where(co => co.MarketerID.Value == MarketerID && (co.CustomerOrderDate >= Begindate && co.CustomerOrderDate <= EndDate)).ToList();
            }


            //var listBillInsuredOp = db.V_Summary_Insured_Bill.Where(co => (co.CustomerOrderDate >= Begindate && co.CustomerOrderDate <= EndDate) ).ToList();
            
            foreach (var getbill in listBillInsuredOp)
            {
                TotMntFacture = TotMntFacture + getbill.Plafond;
            }

            _InfoList.Add(new
            {
                TotMntFacture = TotMntFacture.ToString("N0")
            });
            return Json(_InfoList, JsonRequestBehavior.AllowGet);

        }
        public JsonResult ModelBillInsure_Summary( DateTime Begindate, DateTime EndDate,int MarketerID=0)
        {
            db.Database.CommandTimeout = 0;
            List<V_Summary_Insured_Bill> listBillInsuredOp = new List<V_Summary_Insured_Bill>();
            if (MarketerID == 0)
            {
                listBillInsuredOp = db.V_Summary_Insured_Bill.Where(co => (co.CustomerOrderDate >= Begindate && co.CustomerOrderDate <= EndDate)).ToList();
            }
            else
            {
                listBillInsuredOp = db.V_Summary_Insured_Bill.Where(co => co.MarketerID.Value==MarketerID && (co.CustomerOrderDate >= Begindate && co.CustomerOrderDate <= EndDate)).ToList();
            }

            var list = new
            {
                data = from c in listBillInsuredOp.OrderBy(uc => uc.MarketerName)
                       select
                       new
                       {
                           CustomerOrderID = c.CustomerOrderID,
                           CustomerName = c.CustomerName,
                           IsNewCustomer = c.IsNewCustomer == 1 ? "YES" : "NO",
                           CompanyName = c.CompanyName,
                           CustomerOrderDate = c.CustomerOrderDate.ToString("dd/MM/yyyy"),
                           ValidateBillDate = (c.ValidateBillDate.HasValue) ? c.ValidateBillDate.Value.ToString("dd/MM/yyyy") : "01/01/1900",
                           SellerName = (c.SellerName != null) ? c.SellerName : "",
                           MarketerName = (c.MarketerName != null) ? c.MarketerName : "",
                           NumeroFacture = c.NumeroFacture,
                           PhoneNumber = c.PhoneNumber,
                           MntAssureur = c.Plafond,
                           SellerID = (c.SellerID != null) ? c.SellerID : 0,
                           MarketerID = (c.MarketerID != null) ? c.MarketerID :0,
                           InsuredCompany=c.InsuredCompany
                       }
            };
            var jsonResult = Json(list, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = Int32.MaxValue;
            return jsonResult;

        }

        public JsonResult ModelBillInsure_Detail(DateTime Begindate, DateTime EndDate, int MarketerID = 0)
        {
            db.Database.CommandTimeout = 0;
            List<V_Detail_Insured_Bill> listBillInsuredOp = new List<V_Detail_Insured_Bill>();
            if (MarketerID == 0)
            {
                listBillInsuredOp = db.V_Detail_Insured_Bill.Where(co => (co.CustomerOrderDate >= Begindate && co.CustomerOrderDate <= EndDate)).ToList();
            }
            else
            {
                listBillInsuredOp = db.V_Detail_Insured_Bill.Where(co => co.MarketerID.Value == MarketerID && (co.CustomerOrderDate >= Begindate && co.CustomerOrderDate <= EndDate)).ToList();
            }

            var list = new
            {
                data = from c in listBillInsuredOp.OrderBy(uc => uc.MarketerName)
                       select
                       new
                       {
                           CustomerOrderID = c.CustomerOrderID,
                           CustomerName = c.CustomerName,
                           IsNewCustomer = c.IsNewCustomer == 1 ? "YES" : "NO",
                           CompanyName = c.CompanyName,
                           CustomerOrderDate = c.CustomerOrderDate.ToString("dd/MM/yyyy"),
                           ValidateBillDate = (c.ValidateBillDate.HasValue) ? c.ValidateBillDate.Value.ToString("dd/MM/yyyy") : "01/01/1900",
                           SellerName = (c.SellerName != null) ? c.SellerName : "",
                           MarketerName = (c.MarketerName != null) ? c.MarketerName : "",
                           NumeroFacture = c.NumeroFacture,
                           PhoneNumber = c.PhoneNumber,
                           MntAssureur = c.Plafond,
                           SellerID = (c.SellerID != null) ? c.SellerID : 0,
                           MarketerID = (c.MarketerID != null) ? c.MarketerID : 0,

                           CategoryCode = c.CategoryCode,
                           Prescription = c.Prescription,
                           marque = c.marque,
                           reference = c.reference,
                           InsuredCompany=c.InsuredCompany
                       }
            };
            var jsonResult = Json(list, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = Int32.MaxValue;
            return jsonResult;

        }
        /*** methodes lors du chargement du formulaire*********************/
        public JsonResult GetOpenedBranches()
        {

            IBusinessDay busDayRepo = new BusinessDayRepository();
            List<object> openedBranchesList = new List<object>();
            List<Branch> openedBranches = busDayRepo.GetOpenedBranches();
            foreach (Branch branch in openedBranches)
            {
                openedBranchesList.Add(new
                {
                    BranchID = branch.BranchID,
                    BranchName = branch.BranchName
                });
            }

            return Json(openedBranchesList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult LoadMarketers(string filter)
        {

            List<object> assureursList = new List<object>();
            foreach (User marketer in db.People.OfType<User>().Where(c =>c.IsMarketer && c.Name.StartsWith(filter.ToLower())).ToArray().OrderBy(c => c.Name))
            {
                string itemLabel = "";

                //si le client est une personne physique, on renvoie son nom et son prénom, si c'est une entreprise, on renvoie son nom
                itemLabel = marketer.Name +
                    ((marketer.Description == null && marketer.Description.Length > 0) ? "" : " " + marketer.Description);

                assureursList.Add(new
                {
                    Name = itemLabel,
                    ID = marketer.GlobalPersonID
                });
            }

            return Json(assureursList, JsonRequestBehavior.AllowGet);
        }
    }
}