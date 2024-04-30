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

namespace CABOPMANAGEMENT.Areas.Sale.Controllers
{
    [Authorize(Order = 1)]
    [TakeBusinessDay(Order = 2)]
    public class RptRegularizeBillController : BaseController
    {
        //Current Controller and current page
        private const string CONTROLLER_NAME = "Sale/RptRegularize";
        private const string VIEW_NAME = "Index";


        private IBusinessDay _busDayRepo;
        //private ICustomerOrder _CORepository;
        private ISale _SaleRepository;

        public RptRegularizeBillController(
                 IBusinessDay busDayRepo,
                 //       ICustomerOrder CoRepository,
                 ISale saleRep
                )
        {
            this._busDayRepo = busDayRepo;
            //this._CORepository = CoRepository;
            this._SaleRepository = saleRep;
        }
        // GET: CashRegister/BorderoDepotFacture
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

        public JsonResult chargeSolde(int BranchID, DateTime Bdate, DateTime Edate,int AssureurID=0)
        {
            List<object> _InfoList = new List<object>();
            List<CustomerOrder> listBillInsuredOp = new List<CustomerOrder>();
            double TotMntFacture = 0d;
            double TotalMntRegle = 0d;
            double TotBalance = 0d;

            if (AssureurID==0)
            {
                listBillInsuredOp = db.CustomerOrders.Where(co => co.BranchID == BranchID && (co.ValidateBillDate >= Bdate && co.ValidateBillDate <= Edate) && co.BillState == StatutFacture.Paid).ToList();
            }
            else
            {
                listBillInsuredOp = db.CustomerOrders.Where(co => co.BranchID == BranchID && (co.ValidateBillDate >= Bdate && co.ValidateBillDate <= Edate) && co.AssureurID == AssureurID && co.BillState == StatutFacture.Paid).ToList();
            }
            
            foreach (CustomerOrder getbill in listBillInsuredOp)
            {
                TotMntFacture = TotMntFacture + getbill.Plafond;
                TotalMntRegle = TotalMntRegle + getbill.MntValidate;
            }

            TotBalance = TotMntFacture - TotalMntRegle;
            _InfoList.Add(new
            {
                TotMntFacture = TotMntFacture,
                TotalMntRegle = TotalMntRegle,
                TotBalance= TotBalance
            });
            return Json(_InfoList, JsonRequestBehavior.AllowGet);

        }

        public JsonResult ModelBillInsure(int BranchID, DateTime Begindate, DateTime EndDate, int AssureurID=0)
        {
            List<object> list = new List<object>();
            List<CustomerOrder> listBillInsuredOp = new List<CustomerOrder>();
            if (AssureurID == 0)
            {
                listBillInsuredOp = db.CustomerOrders.Where(co => co.BranchID == BranchID && (co.ValidateBillDate >= Begindate && co.ValidateBillDate <= EndDate) && co.BillState == StatutFacture.Paid).ToList();
            }
            else
            {
                listBillInsuredOp = db.CustomerOrders.Where(co => co.BranchID == BranchID && (co.ValidateBillDate >= Begindate && co.ValidateBillDate <= EndDate) && co.AssureurID == AssureurID && co.BillState == StatutFacture.Paid).ToList();
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
                           ValidateBillDate = c.ValidateBillDate.ToString("dd/MM/yyyy"),
                           UIBranchCode = c.Branch.BranchName,
                           CustomerOrderNumber = c.CustomerOrderNumber,
                           PoliceAssurance = c.PoliceAssurance,
                           NumeroFacture = c.NumeroFacture,
                           PhoneNumber = c.PhoneNumber,
                           MntAssureur = c.Plafond,
                           InsuranceCompany = (c.Assureur != null) ? c.Assureur.Name : db.Assureurs.Find(c.AssureurID.Value).Name, //c.Assureur.Name,
                           MntValidate = c.MntValidate,
                           Remainder = (c.Plafond- c.MntValidate),
                           GestionnaireID=(c.Gestionnaire!=null) ? c.Gestionnaire.Code : (c.GestionnaireID==null)? "": db.Users.Find(c.GestionnaireID).Code
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