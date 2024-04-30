using FatSod.Ressources;
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
using CABOPMANAGEMENT.Tools;
using SaleE = FatSod.Supply.Entities.Sale;

namespace CABOPMANAGEMENT.Areas.CRM.Controllers
{
    [Authorize(Order = 1)]
    [TakeBusinessDay(Order = 2)]
    public class ConsultRDVController : BaseController
    {
        private IRendezVous _RendezVousRepository;
        private IPerson _PersonRepository;
        private ISale _SaleRepository;
        private IBusinessDay _busDayRepo;

        //private List<BusinessDay> listBDUser;

        //Construcitor
        public ConsultRDVController(
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
            ViewBag.DisplayForm = 1;
            try
            {
                List<BusinessDay> bdDay = (List<BusinessDay>)Session["UserBusDays"];
                if (bdDay == null)
                {
                    bdDay = _busDayRepo.GetOpenedBusinessDay(CurrentUser);
                }
                if (bdDay.Count() > 1)
                {
                    TempData["Message"] = "Wrong Business day.<br/>contact our administrator for this purpose<code/>.";
                    ViewBag.Disabled = false;
                    ViewBag.DisplayForm = 0;
                }
                Session["businessDay"] = bdDay.FirstOrDefault();
                DateTime currentDateOp = bdDay.FirstOrDefault().BDDateOperation;
                ViewBag.CurrentBranch = bdDay.FirstOrDefault().BranchID;
                ViewBag.BusnessDayDate = bdDay.FirstOrDefault().BDDateOperation.ToString("yyyy-MM-dd");// businessDay.BDDateOperation;

                Session["BusnessDayDate"] = currentDateOp;
                int deviseID = (Session["DefaultDeviseID"] == null) ? 0 : (int)Session["DefaultDeviseID"];
                if (deviseID <= 0)
                {
                    InjectUserConfigInSession();
                }

                return View(PendingConsultationCustomer());
            }

            catch (Exception e)
            {
                TempData["Message"] = "Error " + e.Message;
                ViewBag.DisplayForm = 0;
                return this.View();
            }
        }

        
        public JsonResult PendingConsultationCustomer()
        {
            try {
                DateTime currentDate = SessionBusinessDay(null).BDDateOperation;

                var dateilya6mois = currentDate.AddMonths(-6);
                //tous les patients ki ont payer leur dernierres consultation
                var list = (from l1 in db.Customers
                                //join lc in db.Sales on l1.GlobalPersonID equals lc.CustomerID
                                //join l2 in db.Sales on lc.CustomerID equals l2.CustomerID into leftJ
                                //from lj in leftJ.DefaultIfEmpty() 
                                //where !lc.isPrescritionValidate
                            where (l1.Dateregister.HasValue /*&&  l1.Dateregister.Value <= dateilya6mois*/)
                            select new
                            {
                                GlobalPersonID = l1.GlobalPersonID,
                                Dateregister = (l1.Dateregister.HasValue) ? l1.Dateregister.Value : new DateTime(1900, 1, 1),
                                DateOfBirth = (l1.DateOfBirth.HasValue) ? l1.DateOfBirth.Value : new DateTime(1900, 1, 1),
                                CustomerFullName = l1.Name,
                                Description = l1.Description, // (l1.Adress!=null) ? l1.Adress.AdressPhoneNumber : "",
                                CustomerNumber = l1.CustomerNumber,
                                QuarterLabel = (l1.Adress != null) ? (l1.Adress.Quarter != null) ? l1.Adress.Quarter.QuarterLabel : "" : "",
                                CNI = l1.CNI,
                                CustomerValue = l1.CustomerValue
                            }).Distinct().ToList();


                var model = new
                {
                    data = from l1 in list.ToList()
                           select new
                           {
                               GlobalPersonID = l1.GlobalPersonID,
                               Dateregister = l1.Dateregister.ToString("dd-MM-yyyy"),
                               DateOfBirth = l1.DateOfBirth.ToString("dd-MM-yyyy"),
                               CustomerFullName = l1.CustomerFullName,
                               Surname = l1.Description,
                               CustomerNumber = l1.CustomerNumber,
                               QuarterLabel = l1.QuarterLabel,
                               CNI = l1.CNI,
                               CustomerValue = l1.CustomerValue == CustomerValue.ECO ? "ECO" : "VIP"
                           }
                };
                var jsonResult = Json(model, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = Int32.MaxValue;
                return jsonResult;
            }
            catch(Exception exception)
            {
                
                return null;
            }
            
        }

        public JsonResult InitializeFields(int GlobalPersonID)
        {

            List<object> list = new List<object>();
            try
            {
                if (GlobalPersonID > 0)
            {
                //we select customer detail
                Customer customerOrder = db.Customers.Find(GlobalPersonID);
                //recupartion de l'ancienne prescription

                var CurrentPrescrip = (from l1 in db.Customers
                                       join lc in db.Consultations on l1.GlobalPersonID equals lc.CustomerID
                                       join lp in db.PrescriptionLSteps on lc.ConsultationID equals lp.ConsultationID
                                       where l1.GlobalPersonID == GlobalPersonID
                                       select new
                                       {
                                           PrescriptionID = lp.PrescriptionLStepID,
                                           Medecin=lp.MedecinTraitant,
                                           DateProchaineCunsultation = (lp.DateRdv!=null) ? lp.DateRdv : new DateTime(1900,1,1)
                                       }).ToList().LastOrDefault();

                    string Medecin =(CurrentPrescrip==null) ? "" : (CurrentPrescrip.Medecin != null) ? CurrentPrescrip.Medecin : "";
                    
                    string DateProchaineCunsultation = (CurrentPrescrip == null) ? new DateTime(1900, 1, 1).ToString("dd-MM-yyyy") : (CurrentPrescrip.DateProchaineCunsultation != null) ? CurrentPrescrip.DateProchaineCunsultation.ToString("dd-MM-yyyy") : new DateTime(1900, 1, 1).ToString("dd-MM-yyyy");

                string RESphere = "",
                        RECylinder = "",
                        REAxis = "",
                        REAddition = "",
                        LESphere = "",
                        LECylinder = "",
                        LEAxis = "",
                        LEAddition = "";

                string SupplyingName = "", LensCategoryCode = "0", TypeLens = "";


                string Axis = "", Addition = "", LensNumberCylindricalValue = "", LensNumberSphericalValue = "";
                string ProductName = "", marque = "", NumeroSerie = "", reference = "";


                var OldPrescript = (from l1 in db.Customers
                                    join l2 in db.Sales on l1.GlobalPersonID equals l2.CustomerID
                                    join l3 in db.SaleLines on l2.SaleID equals l3.SaleID
                                    where l1.GlobalPersonID == GlobalPersonID
                                    select new
                                    {
                                        GlobalPersonID = l1.GlobalPersonID,
                                        DateDernierConsultation = (l1.Dateregister != null) ? l1.Dateregister : new DateTime(1900, 1, 1),
                                        Product = l3.Product,
                                        LensNumberSphericalValue = l3.LensNumberSphericalValue,
                                        Axis = l3.Axis,
                                        LensNumberCylindricalValue = l3.LensNumberCylindricalValue,
                                        Addition = l3.Addition,
                                        OeilDroiteGauche = l3.OeilDroiteGauche,
                                        SaleDate = l2.SaleDate,
                                        SaleID = l2.SaleID,
                                        reference = l3.reference,
                                        marque = l3.marque,
                                        NumeroSerie = l3.NumeroSerie
                                    }).ToList().OrderBy(c => c.SaleID);


                    string DateDernierConsultation = (OldPrescript.ToList().Count == 0) ? new DateTime(1900, 1, 1).ToString("dd-MM-yyyy") : (OldPrescript.FirstOrDefault().DateDernierConsultation != null) ? OldPrescript.FirstOrDefault().DateDernierConsultation.Value.ToString("dd-MM-yyyy") : new DateTime(1900, 1, 1).ToString("dd-MM-yyyy");

                    foreach (var ancsaleLine in OldPrescript)
                {
                    if (ancsaleLine.Product is GenericProduct)
                    {
                        //frame - spray or boitier
                        if (ancsaleLine.reference != null || ancsaleLine.marque != null) //frame
                        {
                            ProductName = ancsaleLine.Product.ProductCode;
                            marque = ancsaleLine.marque;
                            reference = ancsaleLine.reference;
                            NumeroSerie = ancsaleLine.NumeroSerie;
                        }
                    }
                    if ((ancsaleLine.Product is Lens) || (ancsaleLine.Product is OrderLens))
                    {
                        LensCategoryCode = ancsaleLine.Product.Category.CategoryCode;
                        LensCategory cat = (from cate in db.LensCategories
                                            where cate.CategoryCode == LensCategoryCode
                                            select cate).SingleOrDefault();

                        SupplyingName = (cat.SupplyingName != null && cat.SupplyingName.Length > 0) ? cat.SupplyingName : cat.CategoryCode;
                        TypeLens = cat.TypeLens;

                        if ((ancsaleLine.LensNumberSphericalValue == null || ancsaleLine.LensNumberSphericalValue == "") && (ancsaleLine.LensNumberCylindricalValue == null || ancsaleLine.LensNumberCylindricalValue == "") && (ancsaleLine.Addition == null || ancsaleLine.Addition == ""))
                        {
                            

                            if (ancsaleLine.Product is Lens)
                            {
                                Lens lensProduct = db.Lenses.Find(ancsaleLine.Product.ProductID);
                                Axis = ancsaleLine.Axis;
                                Addition = lensProduct.LensNumber.LensNumberAdditionValue;
                                LensNumberCylindricalValue = lensProduct.LensNumber.LensNumberCylindricalValue;
                                LensNumberSphericalValue = lensProduct.LensNumber.LensNumberSphericalValue;
                            }
                            if (ancsaleLine.Product is OrderLens)
                            {
                                OrderLens lensProduct = db.OrderLenses.Find(ancsaleLine.Product.ProductID);
                                Axis = lensProduct.Axis;
                                Addition = lensProduct.Addition;
                                LensNumberCylindricalValue = lensProduct.LensNumberCylindricalValue;
                                LensNumberSphericalValue = lensProduct.LensNumberSphericalValue;
                            }


                        }
                        else
                        {
                            LensNumberSphericalValue = ancsaleLine.LensNumberSphericalValue;
                            LensNumberCylindricalValue = ancsaleLine.LensNumberCylindricalValue;
                            Axis = ancsaleLine.Axis;
                            Addition = ancsaleLine.Addition;
                        }

                        if (LensNumberSphericalValue == null) LensNumberSphericalValue = "";
                        if (LensNumberCylindricalValue == null) LensNumberCylindricalValue = "";
                        if (Addition == null) Addition = "";
                        if (ancsaleLine.OeilDroiteGauche == EyeSide.OD)
                        {
                            if ((LensNumberSphericalValue == "") && (LensNumberCylindricalValue == ""))
                            {
                                RESphere = "0.00";
                                RECylinder = LensNumberCylindricalValue;
                                REAxis = Axis;
                                REAddition = Addition;
                            }
                            else
                            {
                                RESphere = LensNumberSphericalValue;
                                RECylinder = LensNumberCylindricalValue;
                                REAxis = Axis;
                                REAddition = Addition;
                            }
                        }
                        if (ancsaleLine.OeilDroiteGauche == EyeSide.OG)
                        {
                            if ((LensNumberSphericalValue == "") && (LensNumberCylindricalValue == ""))
                            {
                                LESphere = "0.00";
                                LECylinder = LensNumberCylindricalValue;
                                LEAxis = Axis;
                                LEAddition = Addition;
                            }
                            else
                            {
                                LESphere = LensNumberSphericalValue;
                                LECylinder = LensNumberCylindricalValue;
                                LEAxis = Axis;
                                LEAddition = Addition;
                            }
                        }
                    }

                }

                int CustomerAge = 0;
                if (customerOrder.DateOfBirth.HasValue)
                {
                    CustomerAge = (SessionBusinessDay(null) != null) ? SessionBusinessDay(null).BDDateOperation.Year - customerOrder.DateOfBirth.Value.Year : DateTime.Today.Year - customerOrder.DateOfBirth.Value.Year;
                }
                else
                {
                    CustomerAge = 0;
                }

                    list.Add(
                    new
                    {
                        GlobalPersonID = customerOrder.GlobalPersonID,
                        CustomerName = string.Concat(customerOrder.Name, " ", (customerOrder.Description == "") ? "" : customerOrder.Description),
                        PreferredLanguage = customerOrder.PreferredLanguage,
                        AdressPhoneNumber =  (customerOrder.Adress == null) ? "" : (customerOrder.Adress.AdressPhoneNumber==null) ? "" : customerOrder.Adress.AdressPhoneNumber,
                        CustomerNumber = customerOrder.CustomerNumber,
                        QuarterLabel = (customerOrder.Adress.Quarter == null) ? "" : customerOrder.Adress.Quarter.QuarterLabel,
                        DateOfBirth = (customerOrder.DateOfBirth.HasValue) ? customerOrder.DateOfBirth.Value.ToString("dd-MM-yyyy") : new DateTime(1900,1,1).ToString("dd-MM-yyyy"),
                        Dateregister = (customerOrder.Dateregister.HasValue) ? customerOrder.Dateregister.Value.ToString("dd-MM-yyyy") : new DateTime(1900, 1, 1).ToString("dd-MM-yyyy"),
                        Medecin = Medecin, //(Session["UserConnect"] != null) ? (string)Session["UserConnect"].ToString() : db.Users.Where(c => c.GlobalPersonID == SessionGlobalPersonID).FirstOrDefault().UserFullName,

                        OldSupplyingName = SupplyingName,
                        OldLensCategoryCode = LensCategoryCode,

                        OldRESphere = RESphere,
                        OldRECylinder = RECylinder,
                        OldREAxis = REAxis,
                        OldREAddition = REAddition,
                        OldLESphere = LESphere,
                        OldLECylinder = LECylinder,
                        OldLEAxis = LEAxis,
                        OldLEAddition = LEAddition,
                        OldTypeLens = TypeLens,

                        ProductName = ProductName,
                        marque = marque,
                        NumeroSerie = NumeroSerie,
                        reference = reference,

                        DateDernierConsultation = DateDernierConsultation, //(CurrentPrescrip.DateDernierConsultation!=null) ? CurrentPrescrip.DateDernierConsultation.ToString("dd-MM-yyyy") : new DateTime(1900, 1, 1).ToString("dd-MM-yyyy"),
                        DateProchaineCunsultation = DateProchaineCunsultation,
                        CustomerAge = CustomerAge
                    });
            }

            Session["Receipt_GlobalPersonID"] = GlobalPersonID;

            return Json(list, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                string message = e.Message;
                list = new List<object>();
                return Json(list, JsonRequestBehavior.AllowGet);
            }
        }

        //Méthode d'ajout d'un client
        public JsonResult AddRendezVous(ModelRendezVous modelRDV, string CustomerDateOfBirth, string PreferredLanguage) // int GlobalPersonID, DateTime DateOfBirth)
        {
            bool status = false;
            string Message = "";
            try
            {
                string[] listDOB = CustomerDateOfBirth.Split('-');

                int dayDOB = Convert.ToInt32(listDOB[0]);
                int monthDOB = Convert.ToInt32(listDOB[1]);
                int yearDOB = Convert.ToInt32(listDOB[2]);

                modelRDV.DateOfBirth = new DateTime(yearDOB,monthDOB,dayDOB);
                if (modelRDV.DateOfBirth<= new DateTime(1900, 1, 1))
                {
                    status = false;
                    Message = "Error: Wrong Date of Birth Format";
                    return new JsonResult { Data = new { status = status, Message = Message } };
                }
                BusinessDay SBusinessDay = SessionBusinessDay(null);
                if (modelRDV.CustomerID > 0)
                {
                    Person customer = _PersonRepository.UpdatePersonForRdv(modelRDV.CustomerID, modelRDV.DateOfBirth, SessionGlobalPersonID, (SBusinessDay == null) ? DateTime.Now.Date : SBusinessDay.BDDateOperation, (SBusinessDay == null) ? 0 : SBusinessDay.BranchID,  modelRDV.RaisonRdv, modelRDV.GestionnaireID, PreferredLanguage);
                    statusOperation = customer.Name + " : " + Resources.AlertUpdateAction;
                }
                else
                {
                    status = false;
                    Message = "Error: " + Resources.Customerdoesnotexist;
                    return new JsonResult { Data = new { status = status, Message = Message } };
                }

                status = true;
                Message = Resources.Success + "-" + statusOperation;
            }
            catch (Exception e)
            {
                status = false;
                Message = "Error " + e.Message;
            }
            return new JsonResult { Data = new { status = status, Message = Message } };

        }

        public JsonResult populateMarqueters()
        {
            /*
            List<object> marketers = new List<object>();
            foreach (User user in db.People.OfType<User>().Where(u => u.IsMarketer).ToArray().OrderBy(c => c.Name))
            {
                marketers.Add(new { Name = user.UserFullName, ID = user.GlobalPersonID });
            }*/

            return Json(LoadComponent.GetMarketters(CurrentBranch.BranchID), JsonRequestBehavior.AllowGet);
        }
    }

    
}