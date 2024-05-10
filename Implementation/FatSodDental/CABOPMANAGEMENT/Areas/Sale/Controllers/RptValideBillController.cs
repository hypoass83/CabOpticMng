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
using CABOPMANAGEMENT.Areas.Sale.Models;
using System.Web.Configuration;

namespace CABOPMANAGEMENT.Areas.Sale.Controllers
{
    [Authorize(Order = 1)]
    [TakeBusinessDay(Order = 2)]
    public class RptValideBillController : BaseController
    {
        //Current Controller and current page
        private const string CONTROLLER_NAME = "Sale/RptValideBill";
        private const string VIEW_NAME = "Index";


        private IBusinessDay _busDayRepo;
        
        //private ISale _SaleRepository;
        private ICustomerOrder _CustomerOrderRepository;
        public RptValideBillController(
                 IBusinessDay busDayRepo,
                 ICustomerOrder CustomerOrderRepository//,
                 //ISale saleRep
                )
        {
            this._busDayRepo = busDayRepo;
            this._CustomerOrderRepository = CustomerOrderRepository;
            //this._SaleRepository = saleRep;
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

                ViewBag.currentcompany = WebConfigurationManager.AppSettings["AppNameP"];

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

        public JsonResult InitializeCommandFields(int ID)
        {
            List<object> _CommandList = new List<object>();
            try
            {

                CustomerOrder valBill = _CustomerOrderRepository.Find(ID);
                if (valBill == null)
                {
                    TempData["Message"] = "Warning - This sale is already validate";
                    return Json(_CommandList, JsonRequestBehavior.AllowGet);
                }
                
                _CommandList.Add(new
                {
                    DeleteID = valBill.CustomerOrderID,
                    BillNumber = valBill.NumeroFacture

                });
                return Json(_CommandList, JsonRequestBehavior.AllowGet);
               

            }
            catch (Exception e)
            {
                TempData["Message"] = "Error " + e.Message;
                return Json(_CommandList, JsonRequestBehavior.AllowGet);
            }
        }
        //[HttpPost]
        public ActionResult DeleteItem(int DeleteID,string DeleteReason="")
        {
            bool status = false;
            try
            {
                ////suppression histo
                
                CustomerOrder customerOrdersToDelete = _CustomerOrderRepository.DeleteValidatedBill(DeleteID, DeleteReason,SessionGlobalPersonID,SessionBusinessDay(null).BDDateOperation);
                if(customerOrdersToDelete==null)
                {
                    statusOperation = "Error Delete Validated Bill ID "+DeleteID.ToString();
                    status = false;
                }
                else
                {
                    statusOperation = customerOrdersToDelete.CustomerName + " for bill " + customerOrdersToDelete.NumeroFacture + " : " + Resources.AlertDeleteAction;
                    status = true;
                }
                
                return new JsonResult { Data = new { status = status, Message = statusOperation } };
            }
            catch (Exception e)
            {
                statusOperation =  Resources.er_alert_danger + e.Message;
                status = false;
                return new JsonResult { Data = new { status = status, Message = statusOperation } };
            }
        }

        public JsonResult chargeSolde(int BranchID, DateTime Bdate, DateTime Edate, int AssureurID=0)
        {
            List<object> _InfoList = new List<object>();
            List<CustomerOrder> listBillInsuredOp = new List<CustomerOrder>();
            double TotMntFacture = 0d;
           
            if (AssureurID==0)
            {
                listBillInsuredOp = db.CustomerOrders.Where(co => co.BranchID == BranchID && (co.CustomerOrderDate >= Bdate && co.CustomerOrderDate <= Edate) && co.BillState == StatutFacture.Validated).ToList();
            }
            else
            {
                listBillInsuredOp = db.CustomerOrders.Where(co => co.BranchID == BranchID && (co.CustomerOrderDate >= Bdate && co.CustomerOrderDate <= Edate) && co.AssureurID == AssureurID && co.BillState == StatutFacture.Validated).ToList();
            }
            
            foreach (CustomerOrder getbill in listBillInsuredOp)
            {
                TotMntFacture = TotMntFacture + getbill.Plafond;
            }

            _InfoList.Add(new
            {
                TotMntFacture = TotMntFacture.ToString("N0"),
            });
            return Json(_InfoList, JsonRequestBehavior.AllowGet);

        }

        public List<ModelInsuredPart> getRemainderMaladepart(CustomerOrder customerOrder)
        {
            double RemainCustomerPart = 0d, TotalDeposit=0d, MntProforma=0d;
            List<ModelInsuredPart> InsuredPartLst = new List<ModelInsuredPart>();

            if (customerOrder.CustomerOrderLines == null || customerOrder.CustomerOrderLines.Count <= 0)
            { 
                MntProforma = 0;
                InsuredPartLst.Add(new ModelInsuredPart
                {
                    InsuredPart = 0d,
                    RemainInsuredPart = 0d,
                    AdvancedInsuredPart = 0d
                }) ;
                return InsuredPartLst;
            }
            else { MntProforma = Util.ExtraPrices(customerOrder.CustomerOrderLines.Select(sl => sl.LineUnitPrice).Sum(), customerOrder.RateReduction, customerOrder.RateDiscount, customerOrder.Transport, customerOrder.VatRate).TotalTTC; }

            List<AllDeposit> listdepotInsure = db.AllDeposits.Where(c => c.CustomerOrderID == customerOrder.CustomerOrderID).ToList();
            TotalDeposit = (listdepotInsure.Count == 0) ? 0 : listdepotInsure.Select(c => c.Amount).Sum();

            double plafond = (customerOrder.Plafond == null) ? 0 : customerOrder.Plafond;
            double InsuredPart = (MntProforma - plafond)<0 ?0 : MntProforma - plafond;

            RemainCustomerPart = (InsuredPart - TotalDeposit)<0 ?0: InsuredPart - TotalDeposit;

            InsuredPartLst.Add(new ModelInsuredPart
            {
                InsuredPart = InsuredPart,
                AdvancedInsuredPart=TotalDeposit,
                RemainInsuredPart = RemainCustomerPart
            });

            return InsuredPartLst;
        }
        public JsonResult ModelBillInsure(int BranchID, DateTime Begindate, DateTime EndDate, int AssureurID=0) 
        {
            //List<object> list = new List<object>();
            List<CustomerOrder> listBillInsuredOp = new List<CustomerOrder>();

            if (AssureurID == 0)
            {
                listBillInsuredOp = db.CustomerOrders.Where(co => co.BranchID == BranchID && (co.CustomerOrderDate >= Begindate && co.CustomerOrderDate <= EndDate) && co.BillState == StatutFacture.Validated).ToList();
            }
            else
            {
                listBillInsuredOp = db.CustomerOrders.Where(co => co.BranchID == BranchID && (co.CustomerOrderDate >= Begindate && co.CustomerOrderDate <= EndDate) && co.AssureurID == AssureurID && co.BillState == StatutFacture.Validated).ToList();
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
                    InsuranceCompany = (c.Assureur != null) ? c.Assureur.Name : db.Assureurs.Find(c.AssureurID.Value).Name, //c.Assureur.Name,
                    MntValidate = c.MntValidate,
                    AdvanceMalade = getRemainderMaladepart(c).Select(d => d.AdvancedInsuredPart),
                    TotalMalade = getRemainderMaladepart(c).Select(d => d.InsuredPart),
                    RemainderMalade= getRemainderMaladepart(c).Select(d=>d.RemainInsuredPart)
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