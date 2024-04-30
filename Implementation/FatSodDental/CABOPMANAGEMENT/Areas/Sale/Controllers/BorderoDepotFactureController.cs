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
using CABOPMANAGEMENT.Areas.Sale.Models;
using Microsoft.Ajax.Utilities;

namespace CABOPMANAGEMENT.Areas.Sale.Controllers
{
    [Authorize(Order = 1)]
    [TakeBusinessDay(Order = 2)]
    public class BorderoDepotFactureController : BaseController
    {
        //Current Controller and current page
        private const string CONTROLLER_NAME = "Sale/BorderoDepotFacture";
        private const string VIEW_NAME = "Index";


        private IBusinessDay _busDayRepo;
        private ICompteurBorderoDepot _compteurBorderoDepotRepository;
        private ISale _SaleRepository;

        public BorderoDepotFactureController(
				 IBusinessDay busDayRepo,
                 ICompteurBorderoDepot compteurBorderoDepotRepository,
                 ISale saleRep
				)
		{
			this._busDayRepo = busDayRepo;
            this._compteurBorderoDepotRepository = compteurBorderoDepotRepository;
            this._SaleRepository = saleRep;
		}
        // GET: CashRegister/BorderoDepotFacture
        [OutputCache(Duration = 3600)] 
        public ActionResult Index()
        {
            ViewBag.DisplayForm = 1;
            try
            {

               
                List<BusinessDay> UserBusDays = (List<BusinessDay>)Session["UserBusDays"];
                DateTime currentDateOp = UserBusDays.FirstOrDefault().BDDateOperation;
                ViewBag.CurrentBranch = UserBusDays.FirstOrDefault().BranchID;
                ViewBag.BusnessDayDate = UserBusDays.FirstOrDefault().BDDateOperation.ToString("yyyy-MM-dd");// businessDay.BDDateOperation;
                DateTime BeginingDayMonth = new DateTime(currentDateOp.Year, currentDateOp.Month, 1);
                ViewBag.BeginingDayMonth = BeginingDayMonth.ToString("yyyy-MM-dd");

                Session["BorderoDepotID"] = null;

                return View(/*GetAllPostedOrders()*/);
            }

            catch (Exception e)
            {
                TempData["Message"] = "Error " + e.Message;
                ViewBag.DisplayForm = 0;
                return this.View();
            }
        }

        public JsonResult LoadBorderoDepot(int BranchID, DateTime BeginDate, DateTime EndDate, int AssuranceID, string CompanyID="0",int LieuxdeDepotBorderoID=0)
        {
            List<CustomerOrder> lstCustOrder = new List<CustomerOrder>();
            if ((CompanyID == "0" || CompanyID == null))
            {
                if (LieuxdeDepotBorderoID==0)
                {
                    lstCustOrder = db.CustomerOrders.Where(so => so.BillState == StatutFacture.Validated && (so.BorderoDepotID == null)
                       && so.BranchID == BranchID && (so.ValidateBillDate >= BeginDate.Date && so.ValidateBillDate <= EndDate.Date)
                       && so.AssureurID == AssuranceID).ToList();
                }
                else
                {
                    lstCustOrder = db.CustomerOrders.Where(so => so.BillState == StatutFacture.Validated && (so.BorderoDepotID == null)
                       && so.BranchID == BranchID && (so.ValidateBillDate >= BeginDate.Date && so.ValidateBillDate <= EndDate.Date)
                       && so.AssureurID == AssuranceID && so.LieuxdeDepotBorderoID== LieuxdeDepotBorderoID).ToList();
                }
            }
            else
            {
                if (LieuxdeDepotBorderoID == 0)
                {
                    lstCustOrder = db.CustomerOrders.Where(so => so.BillState == StatutFacture.Validated && (so.BorderoDepotID == null)
                       && so.BranchID == BranchID && (so.ValidateBillDate >= BeginDate.Date && so.ValidateBillDate <= EndDate.Date)
                       && so.AssureurID == AssuranceID && so.CompanyName == CompanyID).ToList();
                }
                else
                {
                    lstCustOrder = db.CustomerOrders.Where(so => so.BillState == StatutFacture.Validated && (so.BorderoDepotID == null)
                       && so.BranchID == BranchID && (so.ValidateBillDate >= BeginDate.Date && so.ValidateBillDate <= EndDate.Date)
                       && so.AssureurID == AssuranceID && so.CompanyName == CompanyID && so.LieuxdeDepotBorderoID == LieuxdeDepotBorderoID).ToList();
                }
            }



            var model = new
            {
                data = from c in lstCustOrder.OrderBy(c => c.Assureur.Name).ThenBy(c => c.CustomerOrderDate)
                       select new
                       {
                           ID = c.CustomerOrderID,
                           InsuranceName = (c.InsurreName == null || c.InsurreName == "") ? c.CustomerName : c.InsurreName,
                           CustomerName = c.CustomerName,
                           CompanyName = c.CompanyName,
                           CustomerOrderDate = c.ValidateBillDate.ToString("yyyy-MM-dd"),
                           NumeroFacture = c.NumeroFacture,
                           PoliceAssurance = c.PoliceAssurance,
                           MntAssureur = c.Plafond,// / (1 - c.RemiseAssurance / 100),
                           DeliverDate = (db.CumulSaleAndBills.Where(d => d.CustomerOrderID == c.CustomerOrderID).ToList().Count > 0) ? ((db.CumulSaleAndBills.Where(d => d.CustomerOrderID == c.CustomerOrderID).FirstOrDefault().ProductDeliverDate.HasValue) 
                           ? (db.CumulSaleAndBills.Where(d => d.CustomerOrderID == c.CustomerOrderID).FirstOrDefault().ProductDeliverDate.Value.ToString("dd-MM-yyyy")) : new DateTime(1900, 1, 1).ToString("dd-MM-yyyy")) : new DateTime(1900, 1, 1).ToString("dd-MM-yyyy")
                       }
            };
            return Json(model, JsonRequestBehavior.AllowGet);
        }


        public JsonResult DisplayCodeBordero(int BranchID, DateTime BeginDate, DateTime EndDate, int AssuranceID, string CompanyID="0", int LieuxdeDepotBorderoID=0, bool isValidated = false)
        {
            List<object> _InfoList = new List<object>();
            string BorderoCode = _compteurBorderoDepotRepository.GenerateBDFCode(AssuranceID,SessionBusinessDay(null).BDDateOperation.Year,CompanyID,LieuxdeDepotBorderoID, isValidated);
            //estimated amount
            List<CustomerOrder> lstCustOrder = new List<CustomerOrder>();
            if ((CompanyID == "0" || CompanyID == null))
            {
                if (LieuxdeDepotBorderoID == 0)
                {
                    lstCustOrder = db.CustomerOrders.Where(so => so.BillState == StatutFacture.Validated && (so.BorderoDepotID == null)
                       && so.BranchID == BranchID && (so.ValidateBillDate >= BeginDate.Date && so.ValidateBillDate <= EndDate.Date)
                       && so.AssureurID == AssuranceID).ToList();
                }
                else
                {
                    lstCustOrder = db.CustomerOrders.Where(so => so.BillState == StatutFacture.Validated && (so.BorderoDepotID == null)
                       && so.BranchID == BranchID && (so.ValidateBillDate >= BeginDate.Date && so.ValidateBillDate <= EndDate.Date)
                       && so.AssureurID == AssuranceID && so.LieuxdeDepotBorderoID == LieuxdeDepotBorderoID).ToList();
                }
            }
            else
            {
                if (LieuxdeDepotBorderoID == 0)
                {
                    lstCustOrder = db.CustomerOrders.Where(so => so.BillState == StatutFacture.Validated && (so.BorderoDepotID == null)
                       && so.BranchID == BranchID && (so.ValidateBillDate >= BeginDate.Date && so.ValidateBillDate <= EndDate.Date)
                       && so.AssureurID == AssuranceID && so.CompanyName == CompanyID).ToList();
                }
                else
                {
                    lstCustOrder = db.CustomerOrders.Where(so => so.BillState == StatutFacture.Validated && (so.BorderoDepotID == null)
                       && so.BranchID == BranchID && (so.ValidateBillDate >= BeginDate.Date && so.ValidateBillDate <= EndDate.Date)
                       && so.AssureurID == AssuranceID && so.CompanyName == CompanyID && so.LieuxdeDepotBorderoID == LieuxdeDepotBorderoID).ToList();
                }
            }
            double EstimatedAmount = (lstCustOrder.Count == 0) ? 0 : lstCustOrder.Sum(c => c.Plafond);// / (1 - c.RemiseAssurance / 100));

            _InfoList.Add(new
            {
                BorderoCode = BorderoCode,
                EstimatedAmount= EstimatedAmount
            });
            return Json(_InfoList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ValiadeBordero(string heureVente,int BranchID,int AssuranceID,DateTime BeginDate,DateTime EndDate,string CodeBordero, List<int> rows_selected, string CompanyID="0",int LieuxdeDepotBorderoID=0)
        {
            bool status = false;
            string Message = "";
            try
            {
                Session["BorderoDepotID"] = null;
                BusinessDay openedBD = _busDayRepo.GetOpenedBusinessDay().FirstOrDefault();
                List<int> rowsID = (rows_selected == null) ? new List<int>() : rows_selected;
                if (rowsID.Count>0)
                {
                    int BorderoID = _SaleRepository.SaveBorderoDepotFacture(heureVente, BranchID, AssuranceID, BeginDate, EndDate, CodeBordero, rowsID, CompanyID, LieuxdeDepotBorderoID, openedBD.BDDateOperation, SessionGlobalPersonID);
                    Session["BorderoDepotID"] = BorderoID;
                    Message = "Validate Bordero OK";
                    status = true;
                }
                else
                {
                    Message = "No Data To Validate";
                    status = true;
                }
                
            }
            catch (Exception e)
            {
                Message = "Error " + e.Message + " " + e.InnerException;
                status = false;
            }
            return new JsonResult { Data = new { status = status, Message = Message } };
        }

        

        /*** methodes lors du chargement du formulaire*********************/
        public JsonResult GetOpenedBranches()
        {

            List<object> openedBranchesList = new List<object>();
            List<Branch> openedBranches = _busDayRepo.GetOpenedBranches();
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

        public JsonResult GetInsurance()
        {

            List<object> insureList = new List<object>();
            List<Assureur> openedInsures = db.Assureurs.ToList();
            foreach (Assureur insu in openedInsures)
            {
                insureList.Add(new
                {
                    ID = insu.GlobalPersonID,
                    Name = insu.Name
                });
            }

            return Json(insureList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult populateCompany(int InsuranceID)
        {
            List<object> companyList = new List<object>();
            string AssuranceCode="";

            var openedCompany = db.CustomerOrders.Join(db.InsuredCompanies, c => c.InsuredCompanyID, l => l.InsuredCompanyID,
            (c, l) => new { c, l }).Where(cl => cl.c.AssureurID == InsuranceID)
            .Select(s => new
            {
                CompanyName = s.l.InsuredCompanyCode
            }).Distinct().ToList().OrderBy(c => c.CompanyName);

            //var openedCompany = db.CustomerOrders.Where(ao => ao.AssureurID == InsuranceID)
            //.Select(a => new
            //{
            //    CompanyName= a.CompanyName
            //}).Distinct().ToList().OrderBy(c => c.CompanyName);

            foreach (var lstcomp in openedCompany)
            {
                companyList.Add(new
                {
                    ID = lstcomp.CompanyName,
                    Name = lstcomp.CompanyName,
                    AssuranceCode= AssuranceCode
                });
            }

            return Json(companyList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult getListofDepositLocations(int InsuranceID)
        {
            List<object> lieuxdepotList = new List<object>();

            var list = db.CustomerOrders.Join(db.LieuxdeDepotBorderos, c => c.LieuxdeDepotBorderoID, l => l.LieuxdeDepotBorderoID,
            (c, l) => new { c, l }).Where(cl=> cl.c.AssureurID == InsuranceID)
            .Select(s => new
            {
                LieuxdeDepotBorderoID=s.l.LieuxdeDepotBorderoID,
                LieuxdeDepotBorderoName= s.l.LieuxdeDepotBorderoName
            }).Distinct().ToList();
            
            
            foreach (var lstdep in list)
            {
                lieuxdepotList.Add(new
                {
                    ID = lstdep.LieuxdeDepotBorderoID,
                    Name = lstdep.LieuxdeDepotBorderoName
                });
            }

            return Json(lieuxdepotList, JsonRequestBehavior.AllowGet);
        }
    }
}