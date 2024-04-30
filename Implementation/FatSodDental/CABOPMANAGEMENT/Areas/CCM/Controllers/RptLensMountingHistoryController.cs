using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using FatSod.Report.WrapQuery;
using FatSod.Security.Abstracts;
using FatSod.Security.Entities;
using FatSod.Supply.Abstracts;
using FatSod.Supply.Entities;
using CABOPMANAGEMENT.Controllers;
using CABOPMANAGEMENT.Filters;
using System.Data.Entity;

namespace CABOPMANAGEMENT.Areas.CCM.Controllers
{
    [Authorize(Order = 1)]
    [TakeBusinessDay(Order = 2)]
    public class RptLensMountingHistoryController : BaseController
    {

        //person repository

        private IBusinessDay _busDayRepo;
        private ISale _saleRepository;
        private IRepository<FatSod.Security.Entities.File> _fileRepository;
        private ICustomerReturn _customerReturnRepository;
        private IDeposit _depositRepo;

        public RptLensMountingHistoryController(
            ISale sale,
            IBusinessDay busDayRepo,
            ICustomerReturn customerReturnRepository,
            IRepository<FatSod.Security.Entities.File> fileRepository,
            IDeposit depositRepo
            )
        {
            this._saleRepository = sale;
            this._busDayRepo = busDayRepo;
            this._fileRepository = fileRepository;
            this._customerReturnRepository = customerReturnRepository;
            this._depositRepo = depositRepo;
        }
        //
        // GET: /CashRegister/RptGeneSale/
        [OutputCache(Duration = 3600)]
        public ActionResult Index()
        {
            List<BusinessDay> listBDUser = (List<BusinessDay>)Session["UserBusDays"];
            if (listBDUser == null)
            {
                listBDUser = _busDayRepo.GetOpenedBusinessDay(CurrentUser);
            }
            if (listBDUser.Count() > 1)
            {
                ViewBag.Disabled = false;
            }
            DateTime busDays = listBDUser.FirstOrDefault().BDDateOperation;
            ViewBag.CurrentBranch = listBDUser.FirstOrDefault().BranchID;
            ViewBag.BusnessDayDate = busDays.ToString("yyyy-MM-dd");
            Session["BusnessDayDate"] = busDays;

            return View();
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

        //affiche la liste des
        public JsonResult ModelRptGeneSale(int BranchID, DateTime Begindate, DateTime EndDate)
        {
            
            List<TabModelRptGeneSale> listGenSale = new List<TabModelRptGeneSale>();
            List<CumulSaleAndBill> cumulSaleAndBills = db.CumulSaleAndBills.Where(so => (DbFunctions.TruncateTime(so.LensMountingDateHeure) >= DbFunctions.TruncateTime(Begindate) && DbFunctions.TruncateTime(so.LensMountingDateHeure) <= DbFunctions.TruncateTime(EndDate)) && so.IsMounted == true).ToList();
            List<object> data = new List<object>();
            int i = 0;
            foreach (CumulSaleAndBill cumulSaleAndBill in cumulSaleAndBills.OrderBy(o => o.LensMountingDateHeure))
            {
                string prescriptionSummary = "RAS";
                string patient = "RAS";
                if (cumulSaleAndBill.SaleID != null)
                {
                    patient = cumulSaleAndBill.Customer != null ? cumulSaleAndBill.Customer.CustomerFullName : "RAS";
                }

                if (cumulSaleAndBill.CustomerOrderID != null)
                {
                    patient = cumulSaleAndBill.CustomerOrder.CustomerName;
                }
                try
                {
                    CumulSaleAndBillLine frameSaleLine = cumulSaleAndBill.CumulSaleAndBillLines.SingleOrDefault(sl => sl.marque != null && sl.reference != null);
                    string frameLabel = "";
                    string odLabel = "";
                    string ogLabel = "";

                    if (frameSaleLine != null)
                    {
                        if (frameSaleLine != null)
                        {
                            frameLabel = "<strong>Frame/Monture: </strong>" + frameSaleLine.marque + " - Reference " + frameSaleLine.reference;
                        }                        
                    }

                    List<CumulSaleAndBillLine> lensLines = frameSaleLine == null ? 
                        cumulSaleAndBill.CumulSaleAndBillLines.ToList() :
                        cumulSaleAndBill.CumulSaleAndBillLines.Where(l => l.LineID != frameSaleLine.LineID).ToList();

                    foreach(CumulSaleAndBillLine saleLine  in lensLines)
                    {
                        if (saleLine.ProductLabel.Contains("OD:"))
                        {
                            odLabel = saleLine.ProductLabel;
                            continue;
                        }

                        if (saleLine.ProductLabel.Contains("OG:"))
                        {
                            ogLabel = saleLine.ProductLabel;
                            continue;
                        }

                        if (!saleLine.ProductLabel.Contains("OD:") && saleLine.OeilDroiteGauche == EyeSide.OD)
                        {
                            odLabel = "OD: " + saleLine.ProductLabel;
                            continue;
                        }

                        if (!saleLine.ProductLabel.Contains("OG:") && saleLine.OeilDroiteGauche == EyeSide.OG)
                        {
                            ogLabel = "OG: " + saleLine.ProductLabel;
                            continue;
                        }

                        if (!saleLine.ProductLabel.Contains("OD:") && !saleLine.ProductLabel.Contains("OG:") && 
                            saleLine.OeilDroiteGauche == EyeSide.ODG)
                        {
                            if (odLabel == "")
                            {
                                odLabel = "ODG: " + saleLine.ProductLabel;
                            }else
                            {
                                ogLabel = "ODG: " + saleLine.ProductLabel;
                            }
                            continue;
                        }

                        if (saleLine.OeilDroiteGauche == EyeSide.N)
                        {
                            CumulSaleAndBillLine first = cumulSaleAndBill.CumulSaleAndBillLines.ToArray()[1];
                            odLabel = "OD: " + saleLine.ProductLabel;

                            CumulSaleAndBillLine second = cumulSaleAndBill.CumulSaleAndBillLines.ToArray()[2];
                            if (second != null)
                            {
                                odLabel = "OG: " + saleLine.ProductLabel;
                            }
                        }
                    }
                    odLabel = odLabel.Replace("OD: ", "<strong>OD: </strong>");
                    ogLabel = ogLabel.Replace("OG: ", "<strong>OG: </strong>");
                    odLabel = odLabel.Replace("ODG: ", "<strong>ODG: </strong>");
                    ogLabel = ogLabel.Replace("ODG: ", "<strong>ODG: </strong>");
                    prescriptionSummary = frameLabel + "<br>" + odLabel + "<br>" + ogLabel;

                    var item = new
                    {
                        operationDate = cumulSaleAndBill.LensMountingDateHeure.Value.ToString("yyyy-MM-dd"),
                        number = ++i,
                        patient = patient,
                        mounter = cumulSaleAndBill.MountingBy,
                        controller = cumulSaleAndBill.ControlBy,
                        prescription = prescriptionSummary
                    };
                    data.Add(item);
                }
                catch (Exception e)
                {
                    var a = e;
                }
            }

            var list = new
            {
                data = data
            };
            var jsonResult = Json(list, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = Int32.MaxValue;
            return jsonResult;

        }
    }
}