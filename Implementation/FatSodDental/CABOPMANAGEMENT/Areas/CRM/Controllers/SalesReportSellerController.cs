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

namespace CABOPMANAGEMENT.Areas.CRM.Controllers
{
    [Authorize(Order = 1)]
    [TakeBusinessDay(Order = 2)]
    public class SalesReportSellerController : BaseController
    {
        private IBusinessDay _busDayRepo;


        public SalesReportSellerController(
                 IBusinessDay busDayRepo
                )
        {
            this._busDayRepo = busDayRepo;
        }


        [OutputCache(Duration = 3600)]
        // GET: Sale/SalesReportSeller
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

        public JsonResult chargeSolde(DateTime Begindate, DateTime EndDate, int SellerID = 0)
        {
            List<object> _InfoList = new List<object>();

            double TotMntFacture = 0d;

            db.Database.CommandTimeout = 0;
            List<V_Summary_Sales> listBillInsuredOp = new List<V_Summary_Sales>();
            if (SellerID == 0)
            {
                listBillInsuredOp = db.V_Summary_Sales.Where(co => (co.SaleDate >= Begindate && co.SaleDate <= EndDate)).ToList();
            }
            else
            {
                listBillInsuredOp = db.V_Summary_Sales.Where(co => co.SellerID.Value == SellerID && (co.SaleDate >= Begindate && co.SaleDate <= EndDate)).ToList();
            }

            foreach (var getbill in listBillInsuredOp)
            {
                TotMntFacture = TotMntFacture + getbill.totPrice;
            }

            _InfoList.Add(new
            {
                TotMntFacture = TotMntFacture,
            });
            return Json(_InfoList, JsonRequestBehavior.AllowGet);

        }
        public JsonResult ModelBillInsure_Summary(DateTime Begindate, DateTime EndDate, int SellerID = 0)
        {
            db.Database.CommandTimeout = 0;
            List<V_Summary_Sales> listBillInsuredOp = new List<V_Summary_Sales>();
            if (SellerID == 0)
            {
                listBillInsuredOp = db.V_Summary_Sales.Where(co => (co.SaleDate >= Begindate && co.SaleDate <= EndDate)).ToList();
            }
            else
            {
                listBillInsuredOp = db.V_Summary_Sales.Where(co => co.SellerID.Value == SellerID && (co.SaleDate >= Begindate && co.SaleDate <= EndDate)).ToList();
            }

            var list = new
            {
                data = from c in listBillInsuredOp.OrderBy(uc => uc.SellerName)
                       select
                       new
                       {
                           SaleID = c.SaleID,
                           CustomerName = c.CustomerName,
                           IsNewCustomer = c.IsNewCustomer == 1 ? "YES" : "NO",
                           SaleDate = c.SaleDate.ToString("dd/MM/yyyy"),
                           SellerName = (c.SellerName != null) ? c.SellerName : "",
                           MarketerName = (c.MarketerName != null) ? c.MarketerName : "",
                           SaleReceiptNumber = c.SaleReceiptNumber,
                           PhoneNumber = c.Phone,
                           totPrice = c.totPrice,
                           SellerID = (c.SellerID != null) ? c.SellerID : 0,
                           MarketerID = (c.MarketerID != null) ? c.MarketerID : 0,
                           IsInHouseCustomer = c.IsInHouseCustomer == true ? "YES" : "NO"
                       }
            };
            var jsonResult = Json(list, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = Int32.MaxValue;
            return jsonResult;

        }

        public JsonResult ModelBillInsure_Detail(DateTime Begindate, DateTime EndDate, int SellerID = 0)
        {
            db.Database.CommandTimeout = 0;
            List<V_Detail_Sales> listBillInsuredOp = new List<V_Detail_Sales>();
            if (SellerID == 0)
            {
                listBillInsuredOp = db.V_Detail_Sales.Where(co => (co.SaleDate >= Begindate && co.SaleDate <= EndDate)).ToList();
            }
            else
            {
                listBillInsuredOp = db.V_Detail_Sales.Where(co => co.SellerID.Value == SellerID && (co.SaleDate >= Begindate && co.SaleDate <= EndDate)).ToList();
            }

            var list = new
            {
                data = from c in listBillInsuredOp.OrderBy(uc => uc.SellerName)
                       select
                       new
                       {
                           SaleID = c.SaleID,
                           CustomerName = c.CustomerName,
                           IsNewCustomer = c.IsNewCustomer == 1 ? "YES" : "NO",
                           SaleDate = c.SaleDate.ToString("dd/MM/yyyy"),
                           SellerName = (c.SellerName != null) ? c.SellerName : "",
                           MarketerName = (c.MarketerName != null) ? c.MarketerName : "",
                           SaleReceiptNumber = c.SaleReceiptNumber,
                           PhoneNumber = c.Phone,
                           totPrice = c.totPrice,
                           SellerID = (c.SellerID != null) ? c.SellerID : 0,
                           MarketerID = (c.MarketerID != null) ? c.MarketerID : 0,

                           CategoryCode = c.CategoryCode,
                           Prescription = c.Prescription,
                           marque = c.marque,
                           reference = c.reference,
                           IsInHouseCustomer = c.IsInHouseCustomer == true ? "YES" : "NO"
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

        public JsonResult LoadSellers(string filter)
        {

            List<object> assureursList = new List<object>();
            foreach (User seller in db.People.OfType<User>().Where(c => c.IsSeller && c.Name.StartsWith(filter.ToLower())).ToArray().OrderBy(c => c.Name))
            {
                string itemLabel = "";

                //si le client est une personne physique, on renvoie son nom et son prénom, si c'est une entreprise, on renvoie son nom
                itemLabel = seller.Name +
                    ((seller.Description == null && seller.Description.Length > 0) ? "" : " " + seller.Description);

                assureursList.Add(new
                {
                    Name = itemLabel,
                    ID = seller.GlobalPersonID
                });
            }

            return Json(assureursList, JsonRequestBehavior.AllowGet);
        }
    }
}