using FatSod.Security.Abstracts;
using FatSod.Security.Entities;
using FatSod.Supply.Entities;
using CABOPMANAGEMENT.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using SaleE = FatSod.Supply.Entities.Sale;
using FatSod.Supply.Abstracts;
using CABOPMANAGEMENT.Filters;
using CABOPMANAGEMENT.Areas.Sale.Models;

namespace CABOPMANAGEMENT.Areas.CashRegister.Controllers
{
    [Authorize(Order = 1)]
    [TakeBusinessDay(Order = 2)]
    public class SaleReceiptController : BaseController
    {
        //private const string CONTROLLER_NAME = "CashRegister/SaleReceipt";
        //*********************
        private IBusinessDay _busDayRepo;
        private IDeposit _depositRepository;

        private ICustomerReturn _customerReturnRepository;
        private ITransactNumber _transactNumberRepository;
        private ISale _saleRepository;

        private List<BusinessDay> listBDUser;

        // GET: CashRegister/State
        public SaleReceiptController(
                IBusinessDay busDayRepo,
                IDeposit depositRepository,
                ICustomerReturn customerReturnRepository,
                ITransactNumber transactNumberRepository,
                ISale saleRepository
                )
        {
            this._busDayRepo = busDayRepo;
            this._depositRepository = depositRepository;
            this._customerReturnRepository = customerReturnRepository;
            this._transactNumberRepository = transactNumberRepository;
            this._saleRepository = saleRepository;
        }



        //This method print customer's bill
        [OutputCache(Duration = 3600)]
        public ActionResult SaleReceipt()
        {

            listBDUser = (List<BusinessDay>)Session["UserBusDays"];

            if (listBDUser == null)
            {
                listBDUser = _busDayRepo.GetOpenedBusinessDay(CurrentUser);
            }
            DateTime BDDateOperation = listBDUser.FirstOrDefault().BDDateOperation;

            Session["CustomerReturnLines"] = new List<CustomerReturnLine>();
            Session["ModelSaleLine"] = new List<ModelSaleLine>();
            Session["isDeliverOrder"] = false;
            ViewBag.SoldDate = BDDateOperation.ToString("yyyy-MM-dd");
            return View(ModelReturnAbleSales(BDDateOperation));

        }

        /// <summary>
        /// Liste des ventes donc la garantie court encore et dont tous les éléments de la liste n'ont pas encore été retournés
        /// </summary>
        public JsonResult ModelReturnAbleSales(DateTime SoldDate)
        {
            Session["SoldDate"] = SoldDate.ToString("yyyy-MM-dd");
            listBDUser = (List<BusinessDay>)Session["UserBusDays"];
            if (listBDUser == null)
            {
                listBDUser = _busDayRepo.GetOpenedBusinessDay(CurrentUser);
            }
            //DateTime BDDateOperation = listBDUser.FirstOrDefault().BDDateOperation;
            int currentBD = listBDUser.FirstOrDefault().BranchID;
            //retourne la liste des ventes qui ont deja ete valide
            List<SaleE> allSales = (from sal in db.Sales
                                    where (sal.BranchID == currentBD && sal.SaleDate == SoldDate.Date && (!sal.IsSpecialOrder))
                                    select sal).ToList();

            var model = new
            {
                data = from s in allSales
                       //where (_customerReturnRepository.IsSaleCanBeReturn(s) == true)
                       select
                       new
                       {
                           SaleID = s.SaleID,
                           SaleDate = s.SaleDate.ToString("yyyy-MM-dd"),
                           SaleReceiptNumber = s.SaleReceiptNumber,
                           SaleTotalPrice = _depositRepository.SaleBill(s),
                           PersonName = s.CustomerName
                       }
            };
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        

        public JsonResult ModelSaleLines()
        {
            try
            {
                List<ModelSaleLine> SaleLines = (List<ModelSaleLine>)Session["ModelSaleLine"];

                var model = new
                {
                    data = from sl in SaleLines
                           select new
                           {

                               SaleLineID = sl.SaleLineID,
                               ProductLabel = sl.ProductLabel,
                               LineUnitPrice = sl.LineUnitPrice,
                               LineQuantity = sl.LineQuantity,//SaleLineReturnAbleQuantity(sl),
                               LineAmount = sl.LineAmount,// SaleLineReturnAbleQuantity(sl) * sl.LineUnitPrice,
                               /*QtyToReturn = sl.QtyToReturn,
                               Reason = sl.Reason*/
                           }
                };
                return Json(model, JsonRequestBehavior.AllowGet);


            }
            catch (Exception e)
            {
                List<object> list = new List<object>();
                // TempData["Message"] = "Error " + e.Message;
                return Json(list, JsonRequestBehavior.AllowGet);
            }
        }
        

        private Company Company
        {
            get
            {
                return db.Companies.FirstOrDefault();
            }
        }

        /// <summary>
        /// Quantité de la ligne qui peut encore être retournée
        /// </summary>
        /// <param name="selectedSaleLine"></param>
        /// <returns></returns>
        public double SaleLineReturnAbleQuantity(SaleLine selectedSaleLine)
        {
            double returnedQuantity = this._customerReturnRepository.SaleLineReturnedQuantity(selectedSaleLine);
            return selectedSaleLine.LineQuantity - returnedQuantity;
        }

        /// <summary>
        /// Cette méthode est appelée quand une vente est sélectionnée et permet de renseigner les champs de formulaire liés à la vente sélectionnée. il s'agit de :
        /// 1-Le formulaire de vente
        /// 2-le cady de la vente. 
        /// NB : Il reste à l'utilisateur de remplir le cady de retour
        /// </summary>
        /// <param name="ID"> ID de la vente sélectionnée par l'utilisateur</param>
        /// <returns></returns>
        //[HttpPost]
        public JsonResult InitializeFields(int SaleID)
        {
            List<object> _objectList = new List<object>();
            Session["ModelSaleLine"] = new List<ModelSaleLine>();

            //we take sale and her salelines
            SaleE selectedSale = db.Sales.Find(SaleID);
            double sliceAmount= _depositRepository.SaleTotalPriceAdvance(selectedSale);
            Session["SaleID"] = SaleID;
            Session["SliceAmount"] = sliceAmount;

            Session["Receipt_SaleID"] = null;
            Session["Receipt_CustomerID"] = null;
            Session["ReceiveAmoung_Tot"] = null;

            List<SaleLine> allSaleSLines = db.SaleLines.Where(sl => sl.SaleID == SaleID).ToList();
            List<ModelSaleLine> returnableSaleSLines = new List<ModelSaleLine>();

            foreach (SaleLine sl in allSaleSLines)
            {
                //if (_customerReturnRepository.IsAllLineReturn(sl) == false)
                //{
                    returnableSaleSLines.Add(
                    new ModelSaleLine
                    {
                        SaleLineID = sl.LineID,
                        ProductLabel = sl.ProductLabel,
                        LineUnitPrice = sl.LineUnitPrice,
                        LineQuantity =sl.LineQuantity, //SaleLineReturnAbleQuantity(sl),//sl.LineQuantity,
                        LineAmount = sl.LineAmount,//SaleLineReturnAbleQuantity(sl) * sl.LineUnitPrice,//sl.LineAmount,
                        QtyToReturn = 0,
                        Reason = ""
                    });
                //}
            }
            Session["ModelSaleLine"] = returnableSaleSLines;

            Session["Receipt_SaleID"] = SaleID;
            Session["Receipt_CustomerID"] = selectedSale.CustomerName;
            Session["ReceiveAmoung_Tot"] = sliceAmount;

            _objectList.Add(new
            {
                SaleID = SaleID,
                Name = "ok"
            });
            return Json(_objectList, JsonRequestBehavior.AllowGet);
        }
        
    }

}