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
    public class RptValidatedBDFController : BaseController
    {
        private IBusinessDay _busDayRepo;

        
        public RptValidatedBDFController(
                 IBusinessDay busDayRepo
                )
        {
            this._busDayRepo = busDayRepo;
        }

        // GET: Accounting/RptValidatedBDF
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

        public JsonResult ModelBorderoDepot(int BranchID, DateTime Begindate, DateTime EndDate, int AssureurID = 0)
        {
            List<object> list = new List<object>();
            List<BorderoDepot> listBorderoDepots = new List<BorderoDepot>();

            if (AssureurID == 0)
            {
                listBorderoDepots = db.BorderoDepots.Where(co => (co.BorderoDepotDate >= Begindate && co.BorderoDepotDate <= EndDate) ).ToList();
            }
            else
            {
                listBorderoDepots = db.BorderoDepots.Where(co => (co.BorderoDepotDate >= Begindate && co.BorderoDepotDate <= EndDate) && co.AssureurID == AssureurID ).ToList();
            }

            var model = new
            {
                data = from c in listBorderoDepots
                       select
                       new
                       {
                           BorderoDepotID = c.BorderoDepotID,
                           CompanyName = c.CompanyID,
                           BorderoDepotDate = c.BorderoDepotDate.ToString("dd/MM/yyyy"),
                           ValideBorderoDepot = (c.ValideBorderoDepot) ? true :false,
                           ValidBorderoDepotDate = (c.ValidBorderoDepotDate!=null || c.ValidBorderoDepotDate.HasValue) ? c.ValidBorderoDepotDate.Value.ToString("dd/MM/yyyy") : "",
                           LieuxdeDepotBordero =(db.LieuxdeDepotBorderos.Find(c.LieuxdeDepotBorderoID)!=null) ? db.LieuxdeDepotBorderos.Find(c.LieuxdeDepotBorderoID).LieuxdeDepotBorderoName:"",
                           CodeBorderoDepot = c.CodeBorderoDepot,
                           InsuranceCompany = (c.Assureur != null) ? c.Assureur.Name : db.Assureurs.Find(c.AssureurID).Name, //c.Assureur.Name,
                           BillAmount = ((db.CustomerOrders.Where(d => d.BorderoDepotID == c.BorderoDepotID).ToList())!=null) ? (db.CustomerOrders.Where(d=>d.BorderoDepotID==c.BorderoDepotID).Sum(s=>s.Plafond)) : 0,
                           MntValideBordero = ((db.CustomerOrders.Where(d => d.BorderoDepotID == c.BorderoDepotID).ToList()) != null) ? (db.CustomerOrders.Where(d => d.BorderoDepotID == c.BorderoDepotID).Sum(s => s.MntValideBordero)):0
                       }
            };


            //return Json(model, JsonRequestBehavior.AllowGet);
            var jsonResult = Json(model, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
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