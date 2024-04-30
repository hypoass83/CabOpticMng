using FatSod.Ressources;
using FatSod.Security.Abstracts;
using FatSod.Security.Entities;
using FatSod.Supply.Abstracts;
using FatSod.Supply.Entities;
using CABOPMANAGEMENT.Controllers;
using CABOPMANAGEMENT.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using FatSod.DataContext.Repositories;
using FastSod.Utilities.Util;

namespace CABOPMANAGEMENT.Areas.Sale.Controllers
{
    [Authorize(Order = 1)]
    [TakeBusinessDay(Order = 2)]
    public class RptProformaLstController : BaseController
    {
        private IBusinessDay _busDayRepo;

        private ICustomerOrder _CustomerOrderRepository;
        public RptProformaLstController(
                 IBusinessDay busDayRepo,
                 ICustomerOrder CustomerOrderRepository
                )
        {
            this._busDayRepo = busDayRepo;
            this._CustomerOrderRepository = CustomerOrderRepository;
        }


        // GET: Sale/RptProformaLst
        [OutputCache(Duration = 3600)]
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

                return View();
            }

            catch (Exception e)
            {
                TempData["Message"] = "Error " + e.Message;
                ViewBag.DisplayForm = 0;
                return this.View();
            }
        }

                
        public JsonResult ModelBillInsure(int BranchID, DateTime Begindate, DateTime EndDate, int AssureurID = 0)
        {
            List<object> list = new List<object>();
            List<CustomerOrder> listBillInsuredOp = new List<CustomerOrder>();

            if (AssureurID == 0)
            {
                listBillInsuredOp = db.CustomerOrders.Where(co => co.BranchID == BranchID && (co.CustomerOrderDate >= Begindate && co.CustomerOrderDate <= EndDate) && co.BillState!=StatutFacture.Delete).ToList();
            }
            else
            {
                listBillInsuredOp = db.CustomerOrders.Where(co => co.BranchID == BranchID && (co.CustomerOrderDate >= Begindate && co.CustomerOrderDate <= EndDate) && co.AssureurID == AssureurID && co.BillState != StatutFacture.Delete).ToList();
            }

            var model = new
            {
                data = from c in listBillInsuredOp
                       select
                       new
                       {
                           CustomerOrderID = c.CustomerOrderID,
                           BranchID = c.BranchID,
                           CustomerName = c.CustomerName,
                           CompanyName = c.CompanyName,
                           CustomerOrderDate = c.CustomerOrderDate.ToString("dd/MM/yyyy"),
                           UIBranchCode = c.Branch.BranchName,
                           CustomerOrderNumber = c.CustomerOrderNumber,
                           PoliceAssurance = c.PoliceAssurance,
                           NumeroFacture = c.NumeroFacture,
                           PhoneNumber = c.PhoneNumber,
                           MntAssureur = c.Plafond,
                           InsuranceCompany = (c.Assureur != null) ? c.Assureur.Name : db.Assureurs.Find(c.AssureurID.Value).Name,
                           MntValidate = c.MntValidate,
                           Remainder = c.Remainder,
                           BillState = c.BillState.ToString(),
                           Insured = c.InsurreName,
                           Proforma= Util.ExtraPrices(c.CustomerOrderLines.Select(sl => sl.LineAmount).Sum(), c.RateReduction, c.RateDiscount, c.Transport, c.VatRate).TotalTTC
        }
            };

            return Json(model, JsonRequestBehavior.AllowGet);

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
        public JsonResult LoadAssureurs(string filter)
        {

            List<object> assureursList = new List<object>();
            foreach (Assureur assureur in db.People.OfType<Assureur>().Where(c => c.Name.StartsWith(filter.ToLower())).ToArray().OrderBy(c => c.Name))
            {
                string itemLabel = "";

                //si le client est une personne physique, on renvoie son nom et son prénom, si c'est une entreprise, on renvoie son nom
                itemLabel = assureur.Name +
                    ((assureur.CompanySigle != null && assureur.CompanySigle.Length > 0) ? "" : " " + assureur.Description);

                assureursList.Add(new
                {
                    Name = itemLabel,
                    ID = assureur.GlobalPersonID
                });
            }

            return Json(assureursList, JsonRequestBehavior.AllowGet);
        }
    }
}