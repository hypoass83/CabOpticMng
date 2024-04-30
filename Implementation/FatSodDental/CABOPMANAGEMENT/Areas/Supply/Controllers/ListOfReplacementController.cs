﻿using FatSod.Ressources;
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
    public class ListOfReplacementController : BaseController
    {
        //Current Controller and current page
        private const string CONTROLLER_NAME = "Supply/ListOfReplacement";
        private const string VIEW_NAME = "Index";



        private IStockReplacement _productReplacementRepository;

        public ListOfReplacementController(
                 IStockReplacement productReplacementRepository
                )
        {
            this._productReplacementRepository = productReplacementRepository;
        }
        // GET: CashRegister/ListOfReplacement
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
            List<StockReplacement> lstCustoSaleAmt = new List<StockReplacement>();
            double Totaldebit = 0d;

            //int TotalSO = 0;


            lstCustoSaleAmt = _productReplacementRepository.FindAll.Where(so => (so.StockReplacementDate.Date >= Bdate.Date && so.StockReplacementDate.Date <= Edate) && so.BranchID == BranchID).ToList();


            foreach (StockReplacement c in lstCustoSaleAmt)
            {

                double CustomerOrderTotalPrice = c.StockReplacementLines.Select(sl => (sl.LineQuantity * sl.LineUnitPrice)).Sum();

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

        public JsonResult ModelStockReplacement(int BranchID, DateTime Bdate, DateTime Edate)
        {

            List<RptReturnSale> list = new List<RptReturnSale>();


            //retourne la liste des cadeaux sur  la periode de date choisi

            //produit de verre
            var query = db.StockReplacements.Join(db.StockReplacementLines, cr => cr.StockReplacementID, crln => crln.StockReplacementID,
                (cr, crln) => new { cr, crln })
                .Where(crsaline => (crsaline.cr.StockReplacementDate >= Bdate.Date && crsaline.cr.StockReplacementDate <= Edate.Date) && crsaline.cr.BranchID == BranchID)
                .Select(s => new
                {
                    StockReplacementDate = s.cr.StockReplacementDate,
                    StockReplacementID = s.cr.StockReplacementID,
                    StockReplacementReason = s.crln.StockReplacementReason,
                    LineQuantity = s.crln.LineQuantity,
                    LineUnitPrice = s.crln.LineUnitPrice,
                    Localization = s.crln.Localization,
                    Product = s.crln.Product,
                    Branch = s.cr.Branch,
                    StockReplacementReference = s.cr.StockReplacementReference,
                    AuthoriseBy = s.cr.AutorizedBy.Name,
                    ValidatedBy = s.cr.RegisteredBy.Name,
                    Marque = s.crln.Marque,
                    NumeroSerie = s.crln.NumeroSerie,
                    MarqueDamage = s.crln.MarqueDamage,
                    NumeroSerieDamage = s.crln.NumeroSerieDamage
                })
                .AsQueryable()
                .ToList();

            foreach (var c in query.OrderBy(cr => cr.StockReplacementDate).ThenBy(cr => cr.Product.ProductCode))
            {
                double replacementAmnt = c.LineQuantity * c.LineUnitPrice;
                list.Add(
                    new RptReturnSale
                    {
                        RptReturnSaleID = c.StockReplacementID,
                        Agence = c.Branch.BranchCode,
                        LibAgence = c.Branch.BranchDescription,
                        CustomerReturnCauses = c.StockReplacementReason,
                        LineQuantity = c.LineQuantity,
                        LineAmount = c.LineUnitPrice,
                        ReturnAmount = replacementAmnt,
                        LocalizationCode = c.Localization.LocalizationCode,
                        ProductCode = (c.NumeroSerie == "" || c.NumeroSerie == null) ? c.Product.ProductCode : c.Product.ProductCode + " Ref:" + c.NumeroSerie + " Marque:" + c.Marque,
                        NomClient  = (c.NumeroSerieDamage == "" || c.NumeroSerieDamage == null) ? c.Product.ProductCode : c.Product.ProductCode + " Ref:" + c.NumeroSerieDamage + " Marque:" + c.MarqueDamage,
                        CustomerReturnDate = c.StockReplacementDate,
                        CodeClient = c.StockReplacementReference,
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
                           ValidatedBy = c.ValidatedBy,
                           ProductCodeReplacement=c.NomClient
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