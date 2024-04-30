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
using FatSod.Ressources;
using CABOPMANAGEMENT.Filters;
using CABOPMANAGEMENT.Areas.Sale.Models;

namespace CABOPMANAGEMENT.Areas.Accounting.Controllers
{
    [Authorize(Order = 1)]
    [TakeBusinessDay(Order = 2)]
    public class SaleReturnController : BaseController
    {
        //Current Controller and current page
        private const string CONTROLLER_NAME = "Accounting/SaleReturn";
        private const string VIEW_NAME = "Index";
        
        private ICustomerReturn _customerReturnRepository;

        private IBusinessDay _busDayRepo;
        private IDeposit _depositRepository;

        private List<BusinessDay> listBDUser;

        public SaleReturnController(
            IDeposit depositRepo,
            ICustomerReturn customerReturnRepository,
            IBusinessDay busDayRepo
            )
        {
            this._customerReturnRepository = customerReturnRepository;
            this._busDayRepo = busDayRepo;
            this._depositRepository = depositRepo;
        }
        // GET: Accounting/SaleReturn
        [OutputCache(Duration = 3600)] 
        public ActionResult Index()
        {
            
            Session["Curent_Page"] = VIEW_NAME;
            Session["Curent_Controller"] = CONTROLLER_NAME;
            

            listBDUser = (List<BusinessDay>)Session["UserBusDays"];

            if (listBDUser == null)
            {
                listBDUser = _busDayRepo.GetOpenedBusinessDay(CurrentUser);
            }
            DateTime BDDateOperation = listBDUser.FirstOrDefault().BDDateOperation;
            
            Session["CustomerReturnLines"] = new List<CustomerReturnLine>();
            Session["ModelSaleLine"] = new List<ModelSaleLine>();

            ViewBag.SoldDate = BDDateOperation.ToString("yyyy-MM-dd"); 
            return View(ModelReturnAbleSales(BDDateOperation));
        }

        public ActionResult SaveInline()
        {
            var request = System.Web.HttpContext.Current.Request;
            string QtyToReturn = request.Form["QtyToReturn"];
            
            return Json(request, JsonRequestBehavior.AllowGet);
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

            Session["SaleID"]= SaleID;
            Session["SliceAmount"] = _depositRepository.SaleTotalPriceAdvance(selectedSale);
            

            List<SaleLine> allSaleSLines = db.SaleLines.Where(sl => sl.SaleID == SaleID).ToList();
            List<ModelSaleLine> returnableSaleSLines = new List<ModelSaleLine>();

            foreach (SaleLine sl in allSaleSLines)
            {
                if (_customerReturnRepository.IsAllLineReturn(sl) == false)
                {
                    returnableSaleSLines.Add(
                    new ModelSaleLine
                    {
                        SaleLineID=sl.LineID,
                        ProductLabel=sl.ProductLabel,
                        LineUnitPrice=sl.LineUnitPrice,
                        LineQuantity= SaleLineReturnAbleQuantity(sl),//sl.LineQuantity,
                        LineAmount= SaleLineReturnAbleQuantity(sl) * sl.LineUnitPrice,//sl.LineAmount,
                        QtyToReturn=0,
                        Reason=""
                    });
                }
            }
            Session["ModelSaleLine"] = returnableSaleSLines;

            _objectList.Add(new
            {
                SaleID = SaleID,
                Name = "ok"
            });
            return Json(_objectList, JsonRequestBehavior.AllowGet);
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
        public JsonResult ReturnAllSale(int SaleID, string CustomerReturnCauses="")
        {
            bool status = false;
            string Message = "";
            try
            {
                _customerReturnRepository.ReturnAllSale(SaleID, CustomerReturnCauses,SessionGlobalPersonID,SessionBusinessDay(null).BranchID);
                status = true;
                Message=Resources.Success+ " Return was done successfuly";
            }
            catch (Exception e)
            {
                status = false;
                Message = "Error " + e.Message + " " + e.StackTrace + " " + e.InnerException;
            }
            return new JsonResult { Data = new { status = status, Message = Message } };
        }

        public JsonResult ReturnSale(CustomerReturn customerReturn)
        {
            bool status = false;
            string Message = "";
            try
            {
                if (customerReturn.CustomerReturnLines==null)
                {
                    status = false;
                    Message = "Error: No data to Return ";
                    return new JsonResult { Data = new { status = status, Message = Message } };
                }
                _customerReturnRepository.ReturnSale(customerReturn, SessionGlobalPersonID, SessionBusinessDay(null).BranchID);
                //_customerReturnRepository.ReturnAllSale(SaleID, CustomerReturnCauses, SessionGlobalPersonID, SessionBusinessDay(null).BranchID);
                status = true;
                Message = Resources.Success + " Return was done successfuly";
            }
            catch (Exception e)
            {
                status = false;
                Message = "Error " + e.Message + " " + e.StackTrace + " " + e.InnerException;
            }
            return new JsonResult { Data = new { status = status, Message = Message } };
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

        public JsonResult ModelSaleLines ()
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
                        QtyToReturn = sl.QtyToReturn,
                        Reason = sl.Reason
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
                                    where (sal.BranchID == currentBD && sal.SaleDate == SoldDate.Date)
                                    select sal).ToList();

            var model = new
            {
                data = from s in allSales where (_customerReturnRepository.IsSaleCanBeReturn(s) == true)
                select
                new
                {
                    SaleID = s.SaleID,
                    SaleDate = s.SaleDate.ToString("yyyy-MM-dd"),
                    SaleReceiptNumber = s.SaleReceiptNumber,
                    SaleTotalPrice = _depositRepository.SaleBill(s),
                    PersonName = s.CustomerName // s.Customer.CustomerFullName
                }
            };
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        
    }

}