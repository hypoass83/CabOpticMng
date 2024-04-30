using FatSod.Security.Abstracts;
using FatSod.Security.Entities;
using FatSod.Supply.Abstracts;
using FatSod.Supply.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CABOPMANAGEMENT.Areas.CRM.Models;
using CABOPMANAGEMENT.Controllers;
using CABOPMANAGEMENT.Filters;
using SaleE = FatSod.Supply.Entities.Sale;

namespace CABOPMANAGEMENT.Areas.CRM.Controllers
{
    [Authorize(Order = 1)]
    [TakeBusinessDay(Order = 2)]
    public class RendezVousController : BaseController
    {
        private IRendezVous _RendezVousRepository;
        private IPerson _PersonRepository;
        private ISale _SaleRepository;
        private IBusinessDay _busDayRepo;

        private List<BusinessDay> listBDUser;

        //Construcitor
        public RendezVousController(
            IRendezVous RendezVousRepo,
            IPerson PersonRepository,
            ISale SaleRepository,
            IBusinessDay busDayRepo
            )
        {
            this._SaleRepository = SaleRepository;
            this._PersonRepository = PersonRepository;
            this._RendezVousRepository = RendezVousRepo;
            this._busDayRepo = busDayRepo;
        }
        // GET: CRM/RendezVous
        [OutputCache(Duration = 3600)]
        public ActionResult Index()
        {
            listBDUser = (List<BusinessDay>)Session["UserBusDays"];

            if (listBDUser == null)
            {
                listBDUser = _busDayRepo.GetOpenedBusinessDay(CurrentUser);
            }
            DateTime BDDateOperation = listBDUser.FirstOrDefault().BDDateOperation;
            ViewBag.CurrentDate = BDDateOperation.ToString("yyyy-MM-dd");
            return View(GetAllRDV(0, BDDateOperation));
        }

        private List<ModelRendezVous> ModelRDV(int NbreJours,DateTime ServerDate)
        {

            //List<RendezVous> dataTmp = new List<RendezVous>();
            List<ModelRendezVous> realDataTmp = new List<ModelRendezVous>();
            if (NbreJours == 0)
            {
                /*var lstLensRdv = db.RendezVous.Join(db.Sales, r => r.SaleID, s => s.SaleID,
                (r, s) => new { r, s })
                .Where(rs => !(rs.s.SaleDeliver) && ((DbFunctions.DiffDays(rs.r.DateRdv,ServerDate)) <= NbreJours))*/
                var lstLensRdv = db.Sales.Where(rs=> ((DbFunctions.DiffDays(rs.SaleDate, ServerDate)) <= 45 && !(rs.IsSpecialOrder)) 
                && !db.RendezVous.Any(rd=>rd.SaleID==rs.SaleID) && rs.DateRdv!=null /*&& (db.AuthoriseSales.FirstOrDefault(rd=>rd.AuthoriseSaleID ==rs.AuthoriseSaleID).IsSpecialOrder)*/)
                .Select(rdv => new
                {
                    CustomerID = rdv.CustomerID,
                    RendezVousID = rdv.SaleID,// r.RendezVousID,
                    DateRdv = rdv.SaleDeliveryDate,
                    SaleDate = rdv.SaleDate,
                    RaisonRdv = "Delivery of Lens",
                    SaleReceiptNumber=rdv.SaleReceiptNumber
                }).ToList();

                //dataTmp = (from ls in db.RendezVous
                //           where ((DbFunctions.DiffDays(ServerDate, ls.DateRdv)) <= NbreJours
                //           && !(db.Sales.Find(ls.SaleID).SaleDeliver))
                //           select ls)
                //            .OrderBy(a => a.DateRdv)
                //            .ToList();
            
                
            foreach (var p in lstLensRdv)
            {
                Customer rdvCust = db.Customers.Find(p.CustomerID);
                    //SaleE rdvSale = db.Sales.Find(p.RaisonRdv);// db.HistoRendezVous.Where(h=>h.RendezVousID==p.RendezVousID).FirstOrDefault().SaleID);
                    realDataTmp.Add(
                    new ModelRendezVous
                    {
                        RendezVousID = p.RendezVousID,
                        CustomerID = rdvCust.GlobalPersonID,
                        CustomerName = rdvCust.Name,
                        DateRdv =p.DateRdv.Value,
                        SaleDate=p.SaleDate,
                        RaisonRdv = p.RaisonRdv,
                        SaleID = p.RendezVousID,//rdvSale.SaleID,
                        SaleRef = p.SaleReceiptNumber,//rdvSale.SaleReceiptNumber,
                        Telephone= rdvCust.AdressPhoneNumber
                    });
                }
            }
            return realDataTmp;
        }

        public JsonResult GetAllRDV(int NbreJours, DateTime ServerDate)
        {
            var model = new
            {
                data = from s in ModelRDV(NbreJours, ServerDate)
                select new
                {
                    RendezVousID = s.RendezVousID,
                    CustomerID = s.CustomerID,
                    CustomerName = s.CustomerName,
                    DateRdv = s.DateRdv.ToString("dd/MM/yyyy"),
                    RaisonRdv = s.RaisonRdv,
                    SaleID = s.SaleID,
                    SaleRef = s.SaleRef,
                    Telephone=s.Telephone,
                    SaleDate = s.SaleDate.ToString("dd/MM/yyyy")
                }
            };

            var jsonResult = Json(model, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        public JsonResult InitializePurchaseFields(int ID)
        {
            List<object> list = new List<object>();
            List<object> prescription = new List<object>();
            if (ID > 0)
            {
                //RendezVous selectedRdv = new RendezVous();
                //selectedRdv = db.RendezVous.Find(ID);

                SaleE rdvSale = db.Sales.Find(ID);
                Customer rdvCust = db.Customers.Find(rdvSale.CustomerID);
                string labelProduct = "";
                string label1 = "";
                string label2 = "";
                string label3 = "";
                //SaleE rdvSale = db.Sales.Find(db.HistoRendezVous.Where(h => h.RendezVousID == selectedRdv.RendezVousID).FirstOrDefault().SaleID);
                int i = 0;
                foreach (SaleLine c in rdvSale.SaleLines)
                {
                    labelProduct = (c.marque != null && c.reference != null) ? "Frame/Monture " + c.marque + " - Reference " + c.reference : (c.Product is OrderLens) ? (c.SupplyingName == null) ? c.Product.ProductCode : c.SupplyingName : c.Product.ProductLabel;
                    if (i==0)
                    {
                        label1 = labelProduct;
                    }
                    if (i == 1)
                    {
                        label2 = labelProduct;
                    }
                    if (i == 2)
                    {
                        label3 = labelProduct;
                    }
                    i += 1;
                }
                list.Add(new
                {
                    RendezVousID = rdvSale.SaleID,// selectedRdv.RendezVousID,
                    CustomerID = rdvSale.CustomerID,// selectedRdv.CustomerID,
                    CustomerName = rdvCust.Name,
                    DateRdv = rdvSale.SaleDeliveryDate.Value.ToString("dd/MM/yyyy"), //selectedRdv.DateRdv.ToString("dd/MM/yyyy"),
                    RaisonRdv = "Delivery of Lens",// selectedRdv.RaisonRdv,
                    SaleID = rdvSale.SaleID,
                    SaleRef = rdvSale.SaleReceiptNumber,
                    Telephone = rdvCust.AdressPhoneNumber,
                    SaleDate = rdvSale.SaleDate.ToString("dd/MM/yyyy"),
                    label1 = label1,
                    label2 = label2,
                    label3 = label3
                });
            }

            return Json(list, JsonRequestBehavior.AllowGet);

        }

        public JsonResult EditLine(int RendezVousID, DateTime DateRdv, string RaisonRdv,int SaleID)
        {

            bool status = false;
            string Message = "";


            try
            {

                RendezVous selectedRdv = new RendezVous();
                selectedRdv = _RendezVousRepository.Find(RendezVousID);
                BusinessDay sessBday = SessionBusinessDay(null);
                DateTime serverdate = sessBday.BDDateOperation;
                if (selectedRdv!=null)
                {
                    _RendezVousRepository.UpdateRendezVous(RendezVousID,SaleID, DateRdv,RaisonRdv, SessionGlobalPersonID, serverdate, sessBday.BranchID);
                }
                else
                {
                    SaleE selectedSale = db.Sales.Find(SaleID);
                    RendezVous NewRdv = new RendezVous();
                    NewRdv.CustomerID = selectedSale.CustomerID.Value;
                    NewRdv.DateRdv = DateRdv;
                    NewRdv.RaisonRdv = RaisonRdv;
                    NewRdv.SaleID = SaleID;
                    _RendezVousRepository.AddRendezVous(NewRdv, SaleID, SessionGlobalPersonID, serverdate, sessBday.BranchID);
                }

                status = true;
                Message = "Updated rows successfully!";
                return new JsonResult { Data = new { status = status, Message = Message } };
            }
            catch (Exception e)
            {
                status = false;
                Message = @"Une erreur s'est produite lors de l'opération, veuillez contactez l'administrateur et/ou essayez à nouveau
                                    <br/>Code : <code>" + e.Message + " car " + e.StackTrace + "</code>";
                return new JsonResult { Data = new { status = status, Message = Message } };
            }
        }
    }
}