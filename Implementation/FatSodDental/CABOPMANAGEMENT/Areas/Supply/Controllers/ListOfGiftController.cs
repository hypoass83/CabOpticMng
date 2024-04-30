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

using FatSod.Report.WrapReports;

namespace CABOPMANAGEMENT.Areas.Supply.Controllers
{
    [Authorize(Order = 1)]
    [TakeBusinessDay(Order = 2)]
    public class ListOfGiftController : BaseController
    {
        //Current Controller and current page
        private const string CONTROLLER_NAME = "Supply/ListOfGift";
        private const string VIEW_NAME = "Index";



        private IProductGift _productGiftRepository;

        public ListOfGiftController(
                 IProductGift productGiftRepository
                )
        {
            this._productGiftRepository = productGiftRepository;
        }
        // GET: CashRegister/ListOfGift
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

        public JsonResult chargeSolde(int BranchID, DateTime Bdate, DateTime Edate)
        {
            List<object> _InfoList = new List<object>();
            List<ProductGift> lstCustoSaleAmt = new List<ProductGift>();
            double Totaldebit = 0d;

            //int TotalSO = 0;


            lstCustoSaleAmt = _productGiftRepository.FindAll.Where(so => (so.ProductGiftDate.Date >= Bdate.Date && so.ProductGiftDate.Date <= Edate) && so.BranchID == BranchID).ToList();


            foreach (ProductGift c in lstCustoSaleAmt)
            {

                double CustomerOrderTotalPrice = c.ProductGiftLines.Select(sl => (sl.LineQuantity * sl.LineUnitPrice)).Sum();

                Totaldebit = Totaldebit + CustomerOrderTotalPrice;

                //TotalSO = TotalSO + 1;

            }

            _InfoList.Add(new
            {
                TotalDebit = Totaldebit.ToString("0,0"),
                //TotalSO = TotalSO,
            });
            return Json(_InfoList, JsonRequestBehavior.AllowGet);

        }

        public JsonResult ModelProductGift(int BranchID, DateTime Bdate, DateTime Edate)
        {

            List<RptReturnSale> list = new List<RptReturnSale>();


            //retourne la liste des cadeaux sur  la periode de date choisi

            //produit de verre
            var query = db.ProductGifts.Join(db.ProductGiftLines, cr => cr.ProductGiftID, crln => crln.ProductGiftID,
                (cr, crln) => new { cr, crln })
                .Where(crsaline => (crsaline.cr.ProductGiftDate >= Bdate.Date && crsaline.cr.ProductGiftDate <= Edate.Date) && crsaline.cr.BranchID == BranchID)
                .Select(s => new
                {
                    ProductGiftDate = s.cr.ProductGiftDate,
                    ProductGiftID = s.cr.ProductGiftID,
                    ProductGiftReason = s.crln.ProductGiftReason,
                    LineQuantity = s.crln.LineQuantity,
                    LineUnitPrice = s.crln.LineUnitPrice,
                    Localization = s.crln.Localization,
                    Product = s.crln.Product,
                    Branch = s.cr.Branch,
                    ProductGiftReference = s.cr.ProductGiftReference,
                    AuthoriseBy = s.cr.AutorizedBy.Name,
                    ValidatedBy = s.cr.RegisteredBy.Name,
                    Marque=s.crln.Marque,
                    NumeroSerie = s.crln.NumeroSerie
                })
                .AsQueryable()
                .ToList();

            foreach (var c in query.OrderBy(cr => cr.ProductGiftDate).ThenBy(cr => cr.Product.ProductCode))
            {
                double giftAmnt = c.LineQuantity * c.LineUnitPrice;
                list.Add(
                    new RptReturnSale
                    {
                        RptReturnSaleID = c.ProductGiftID,
                        Agence = c.Branch.BranchCode,
                        LibAgence = c.Branch.BranchDescription,
                        CustomerReturnCauses = c.ProductGiftReason,
                        LineQuantity = c.LineQuantity,
                        LineAmount = c.LineUnitPrice,
                        ReturnAmount = giftAmnt,
                        LocalizationCode = c.Localization.LocalizationCode,
                        ProductCode = (c.NumeroSerie == "" || c.NumeroSerie == null) ? c.Product.ProductCode : c.Product.ProductCode + " Ref:" + c.NumeroSerie + " Marque:" + c.Marque,
                        CustomerReturnDate = c.ProductGiftDate,
                        CodeClient = c.ProductGiftReference,
                        AuthoriseBy = c.AuthoriseBy,
                        ValidatedBy = c.ValidatedBy
                    }
                );
            };

            var model = new
            {
                data = from c in list
                       select
                       new
                       {
                           RptReturnSaleID = c.RptReturnSaleID,
                           Agence = c.Agence,
                           LibAgence = c.LibAgence,
                           CustomerReturnCauses = c.CustomerReturnCauses,
                           LineQuantity = c.LineQuantity,
                           LineAmount = c.LineAmount,
                           ReturnAmount = c.ReturnAmount,
                           LocalizationCode = c.LocalizationCode,
                           ProductCode = c.ProductCode,
                           CustomerReturnDate = c.CustomerReturnDate.ToString("dd/MM/yyyy"),
                           CodeClient = c.CodeClient,
                           AuthoriseBy = c.AuthoriseBy,
                           ValidatedBy = c.ValidatedBy
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




    }
}