using FastSod.Utilities.Util;
using FatSod.DataContext.Repositories;
using FatSod.Report.WrapReports;
using FatSod.Security.Abstracts;
using FatSod.Security.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CABOPMANAGEMENT.Controllers;
using CABOPMANAGEMENT.Filters;

namespace CABOPMANAGEMENT.Areas.Accounting.Controllers
{
    [Authorize(Order = 1)]
    [TakeBusinessDay(Order = 2)]
    public class RptLstReturnSalesController : BaseController
    {
        private IBusinessDay _busDayRepo;

        public RptLstReturnSalesController(
                 IBusinessDay busDayRepo

                )
        {
            this._busDayRepo = busDayRepo;

        }


        // GET: Accounting/RptLstReturnSales
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
        public JsonResult ModelSaleReturn(int BranchID, DateTime Bdate, DateTime Edate)
        {
            List<RptReturnSale> list = new List<RptReturnSale>();

            //retourne la liste des retour sur vente pr la periode de date choisi

            //produit de verre
            var query = db.CustomerReturns.Join(db.CustomerReturnLines, cr => cr.CustomerReturnID, crln => crln.CustomerReturnID,
                (cr, crln) => new { cr, crln }).
                Join(db.SaleLines, crsl => crsl.crln.SaleLineID, sal => sal.LineID, (crsl, sal) => new { crsl, sal })
                .Where(crsaline => (crsaline.crsl.crln.CustomerReturnDate >= Bdate.Date && crsaline.crsl.crln.CustomerReturnDate <= Edate.Date) && crsaline.sal.Sale.BranchID == BranchID)
                .Select(s => new
                {
                    CustomerReturnDate = s.crsl.crln.CustomerReturnDate,
                    CustomerReturnID = s.crsl.cr.CustomerReturnID,
                    CustomerReturnCauses = s.crsl.crln.CustomerReturnCauses,
                    LineQuantity = s.crsl.crln.LineQuantity,
                    LineAmount = s.sal.LineUnitPrice,
                    Localization = s.sal.Localization,
                    OeilDroiteGauche = s.sal.OeilDroiteGauche,
                    Product = s.sal.Product,
                    Sale = s.crsl.cr.Sale,
                    CustomerID = s.crsl.cr.Sale.CustomerID,
                    RateReduction = s.crsl.cr.Sale.RateReduction,
                    RateDiscount = s.crsl.cr.Sale.RateDiscount,
                    Transport = s.crsl.cr.Sale.Transport,
                    VatRate = s.crsl.cr.Sale.VatRate
                })
                .AsQueryable()
                .ToList();

            foreach (var c in query.OrderBy(cr => cr.CustomerReturnDate).ThenBy(cr => cr.CustomerID))
            {
                double returnAmnt = c.LineQuantity * c.LineAmount;
                ExtraPrice extra = Util.ExtraPrices(returnAmnt, c.RateReduction, c.RateDiscount, c.Transport, c.VatRate);
                list.Add(
                            new RptReturnSale
                            {
                                RptReturnSaleID = c.CustomerReturnID,
                                Agence = c.Localization.Branch.BranchCode,
                                LibAgence = c.Localization.Branch.BranchDescription,
                                Devise = c.Sale.Devise.DeviseCode,
                                LibDevise = c.Sale.SaleReceiptNumber,
                                CodeClient = c.Sale.Customer.CNI,
                                NomClient = c.Sale.Customer.Name,
                                CustomerReturnCauses = c.CustomerReturnCauses,
                                LineQuantity = c.LineQuantity,
                                LineAmount = c.LineAmount,
                                ReturnAmount = extra.TotalTTC,// c.LineQuantity*c.LineAmount,
                                LocalizationCode = c.Localization.LocalizationCode,
                                ProductCode = c.Product.ProductCode,
                                OeilDroiteGauche = c.OeilDroiteGauche.ToString(),
                                CustomerReturnDate = c.CustomerReturnDate
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
                           Devise = c.Devise,
                           LibDevise = c.LibDevise,
                           CodeClient = c.CodeClient,
                           NomClient = c.NomClient,
                           CustomerReturnCauses = c.CustomerReturnCauses,
                           LineQuantity = c.LineQuantity,
                           LineAmount = c.LineAmount,
                           ReturnAmount = c.ReturnAmount,
                           LocalizationCode = c.LocalizationCode,
                           ProductCode = c.ProductCode,
                           OeilDroiteGauche = c.OeilDroiteGauche,
                           CustomerReturnDate = c.CustomerReturnDate.ToString("dd/MM/yyyy")
                       }
            };

            return Json(model, JsonRequestBehavior.AllowGet);

        }

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