using FatSod.DataContext.Repositories;
using FatSod.Ressources;
using FatSod.Security.Abstracts;
using FatSod.Security.Entities;
using FatSod.Supply.Abstracts;
using FatSod.Supply.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CABOPMANAGEMENT.Areas.CRM.Models;
using CABOPMANAGEMENT.Controllers;
using CABOPMANAGEMENT.Filters;

namespace CABOPMANAGEMENT.Areas.CRM.Controllers
{
    [Authorize(Order = 1)]
    [TakeBusinessDay(Order = 2)]
    public class ConsultationController : BaseController
    {
        private IBusinessDay _busDayRepo;

        private ICustomerOrder _customerOrderRepository;
        private IPrescriptionLStep _prescriptionRepository;

        private LensConstruction lensFrameConstruction = new LensConstruction();

        public ConsultationController(
            IPrescriptionLStep prescriptionRepository,
            IBusinessDay busDayRepo,
            ICustomerOrder customerOrderRepository
        )
        {
            this._prescriptionRepository = prescriptionRepository;
            this._busDayRepo = busDayRepo;
            this._customerOrderRepository = customerOrderRepository;
        }
        // GET: CRM/Consultation
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
                Session["Receipt_GlobalPersonID"] = null;
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
            DateTime currentdate = SessionBusinessDay(null).BDDateOperation;
            var list = (from l1 in db.Customers
                        join lc in db.Consultations on l1.GlobalPersonID equals lc.CustomerID
                        where !lc.isPrescritionValidate && (lc.DateConsultation == currentdate) && l1.IsInHouseCustomer
                        select new
                        {
                            GlobalPersonID = l1.GlobalPersonID,
                            Dateregister = (l1.Dateregister.HasValue) ? l1.Dateregister.Value : new DateTime(1900, 1, 1),
                            DateOfBirth = (l1.DateOfBirth.HasValue) ? l1.DateOfBirth.Value : new DateTime(1900, 1, 1),
                            CustomerFullName = l1.Name,
                            Description = l1.Description, // (l1.Adress!=null) ? l1.Adress.AdressPhoneNumber : "",
                            CustomerNumber = l1.CustomerNumber,
                            Customer = l1,
                            CNI = l1.CNI,
                            QuarterLabel = (l1.Adress != null) ? (l1.Adress.Quarter != null) ? l1.Adress.Quarter.QuarterLabel : "" : ""
                        }).ToList();


            var model = new
            {
                data = from l1 in list.ToList()
                       select new
                       {
                           GlobalPersonID = l1.GlobalPersonID,
                           Dateregister = l1.Dateregister.ToString("yyyy-MM-dd"),
                           DateOfBirth = l1.DateOfBirth.ToString("yyyy-MM-dd"),
                           CustomerFullName = l1.CustomerFullName,
                           Surname = l1.Description,
                           CustomerNumber = l1.CustomerNumber,
                           QuarterLabel = l1.QuarterLabel,
                           CNI = l1.CNI,
                           CustomerValue = l1.Customer.CustomerValueUI
                       }
            };
            var jsonResult = Json(model, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = Int32.MaxValue;
            return jsonResult;
        }

        public JsonResult LoadAVLTSName(string filter)
        {

            List<object> AVLTSList = new List<object>();
            foreach (AVLTS AVLts in db.AVLTSs.Where(c => c.Name.Contains(filter.ToLower())).ToArray().OrderBy(c => c.Name))
            {
                string itemLabel = "";

                itemLabel = AVLts.Name;

                AVLTSList.Add(new
                {
                    Name = itemLabel,
                    ID = AVLts.AVLTSID
                });
            }

            return Json(AVLTSList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult LoadAVLName(string filter)
        {

            List<object> AVLList = new List<object>();
            foreach (AcuiteVisuelL AVL in db.AcuiteVisuelLs.Where(c => c.Name.Contains(filter.ToLower())).ToArray().OrderBy(c => c.Name))
            {
                string itemLabel = "";

                itemLabel = AVL.Name;

                AVLList.Add(new
                {
                    Name = itemLabel,
                    ID = AVL.AcuiteVisuelLID
                });
            }

            return Json(AVLList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult LoadAVPName(string filter)
        {

            List<object> AVLList = new List<object>();
            foreach (AcuiteVisuelP AVL in db.AcuiteVisuelPs.Where(c => c.Name.Contains(filter.ToLower())).ToArray().OrderBy(c => c.Name))
            {
                string itemLabel = "";

                itemLabel = AVL.Name;

                AVLList.Add(new
                {
                    Name = itemLabel,
                    ID = AVL.AcuiteVisuelPID
                });
            }

            return Json(AVLList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult InitializeFields(int GlobalPersonID)
        {

            List<ModelCheckATCDFam> _ModelCheckATCDFamList = new List<ModelCheckATCDFam>();
            List<ModelCheckATCDPerso> _ModelCheckATCDPersoList = new List<ModelCheckATCDPerso>();
            List<object> list = new List<object>();
            Session["Receipt_GlobalPersonID"] = null;
            try
            {
                if (GlobalPersonID > 0)
                {
                    //we select customer detail
                    Customer customer = db.Customers.Find(GlobalPersonID);
                    DateTime Currentdate = SessionBusinessDay(null).BDDateOperation;

                    var consultinfo = (from l1 in db.Customers
                                       join lc in db.Consultations on l1.GlobalPersonID equals lc.CustomerID
                                       where l1.GlobalPersonID == GlobalPersonID && lc.DateConsultation == Currentdate
                                       select new
                                       {
                                           GlobalPersonID = l1.GlobalPersonID,
                                           ConsultationID = lc.ConsultationID,
                                           RaisonRdv = lc.RaisonRdv
                                       }).ToList().LastOrDefault();

                    //info old prescription
                    var consultOldPrescr = (from l2 in db.Consultations
                                            join lc in db.ConsultOldPrescrs on l2.ConsultationID equals lc.ConsultationID
                                            where l2.CustomerID == GlobalPersonID && l2.ConsultationID == consultinfo.ConsultationID
                                            select new
                                            {
                                                ConsultOldPrescrID = lc.ConsultOldPrescrID,
                                                CategoryID = lc.CategoryID,
                                                LAxis = lc.LAxis,
                                                LAddition = lc.LAddition,
                                                LIndex = lc.LIndex,
                                                LCylValue = lc.LCylValue,
                                                LSphValue = lc.LSphValue,
                                                RAxis = lc.RAxis,
                                                RAddition = lc.RAddition,
                                                RIndex = lc.RIndex,
                                                RCylValue = lc.RCylValue,
                                                RSphValue = lc.RSphValue,

                                                DateDernierConsultation = lc.DateDernierConsultation,
                                                OldDilatation = lc.IsDilatation,
                                                OldCollyre = lc.IsCollyre,
                                                OldNomCollyre = lc.NomCollyre,
                                                PlaintePatient = lc.PlaintePatient,
                                                OldPlaintePatient = lc.OldPlaintePatient,
                                                AcuiteVisuelL = lc.AcuiteVisuelL,
                                                AcuiteVisuelP = lc.AcuiteVisuelP,
                                                OldAcuiteVisuelL = lc.OldAcuiteVisuelL,
                                                OldRAcuiteVisuelL = lc.OldRAcuiteVisuelL,
                                                OldLAcuiteVisuelL = lc.OldLAcuiteVisuelL,
                                                OldAcuiteVisuelP = lc.OldAcuiteVisuelP,
                                                OldRAcuiteVisuelP = lc.OldRAcuiteVisuelP,
                                                OldLAcuiteVisuelP = lc.OldLAcuiteVisuelP,
                                                RAcuiteVisuelL = lc.RAcuiteVisuelL,
                                                LAcuiteVisuelL = lc.LAcuiteVisuelL,
                                                RAcuiteVisuelP = lc.RAcuiteVisuelP,
                                                LAcuiteVisuelP = lc.LAcuiteVisuelP,

                                                //OldRAVLTS = lc.OldRAVLTS,
                                                //OldLAVLTS = lc.OldLAVLTS,
                                                RAVLTS = lc.RAVLTS,
                                                LAVLTS = lc.LAVLTS
                                            }).ToList().LastOrDefault();
                    //info perso medical histo
                    var ConsultPersonalMedHisto = (from l2 in db.Consultations
                                                   join lc in db.ConsultPersonalMedHistos on l2.ConsultationID equals lc.ConsultationID
                                                   where l2.CustomerID == GlobalPersonID && l2.ConsultationID == consultinfo.ConsultationID
                                                   select new
                                                   {
                                                       ConsultPersonalMedHistoID = lc.ConsultPersonalMedHistoID,
                                                   }).ToList().LastOrDefault();
                    //info current dilatation and current prescr
                    var CurrentDilPrescrip = (from l1 in db.Consultations
                                              join lp in db.ConsultDilPrescs on l1.ConsultationID equals lp.ConsultationID
                                              join lc in db.ConsultDilatations on lp.ConsultDilPrescID equals lc.ConsultDilPrescID
                                              where l1.CustomerID == GlobalPersonID && l1.ConsultationID == consultinfo.ConsultationID //&& lc.DateConsultation == Currentdate
                                              select new
                                              {
                                                  ConsultDilPrescID = lp.ConsultDilPrescID,
                                                  ConsultDilatationID = lc.ConsultDilatationID,
                                              }).ToList().LastOrDefault();

                    //info current prescription
                    var consultCurrentPrescr = (from l2 in db.Consultations
                                                join lp in db.ConsultDilPrescs on l2.ConsultationID equals lp.ConsultationID
                                                join lc in db.ConsultLensPrescriptions on lp.ConsultDilPrescID equals lc.ConsultDilPrescID
                                                where l2.CustomerID == GlobalPersonID && l2.ConsultationID == consultinfo.ConsultationID
                                                select new
                                                {
                                                    ConsultDilPrescID = lp.ConsultDilPrescID,
                                                    ConsultLensPrescriptionID = lc.ConsultLensPrescriptionID,
                                                    LAxis = lc.LAxis,
                                                    LAddition = lc.LAddition,
                                                    LIndex = lc.LIndex,
                                                    LCylValue = lc.LCylValue,
                                                    LSphValue = lc.LSphValue,
                                                    RAxis = lc.RAxis,
                                                    RAddition = lc.RAddition,
                                                    RIndex = lc.RIndex,
                                                    RCylValue = lc.RCylValue,
                                                    RSphValue = lc.RSphValue,
                                                    CategoryID = lc.CategoryID,
                                                    SupplyingName = lc.SupplyingName

                                                }).ToList().LastOrDefault();

                    //infor consult last step prescription
                    var CurrentPrescripLastStep = (from l1 in db.Customers
                                                   join lc in db.Consultations on l1.GlobalPersonID equals lc.CustomerID
                                                   join lp in db.PrescriptionLSteps on lc.ConsultationID equals lp.ConsultationID
                                                   where l1.GlobalPersonID == GlobalPersonID && lc.ConsultationID == consultinfo.ConsultationID //&& lc.DateConsultation == Currentdate
                                                   select new
                                                   {
                                                       PrescriptionLStepID = lp.PrescriptionLStepID,
                                                       DateProchaineCunsultation = lp.DateRdv,
                                                       Medecin = lp.MedecinTraitant,
                                                       Collyre = lp.PrescriptionCollyre,
                                                       NomCollyre = lp.CollyreName,
                                                       Remarque = lp.Remarque
                                                   }).ToList().LastOrDefault();

                    string Medecin = (CurrentPrescripLastStep == null) ? "" : (CurrentPrescripLastStep.Medecin != null) ? CurrentPrescripLastStep.Medecin : "";
                    string Remarque = (CurrentPrescripLastStep == null) ? "" : (CurrentPrescripLastStep.Remarque != null) ? CurrentPrescripLastStep.Remarque : "";
                    string DateProchaineCunsultation = (CurrentPrescripLastStep == null) ? new DateTime(1900, 1, 1).ToString("yyyy-MM-dd") : (CurrentPrescripLastStep.DateProchaineCunsultation != null) ? CurrentPrescripLastStep.DateProchaineCunsultation.ToString("dd-mm-yyyy") : new DateTime(1900, 1, 1).ToString("dd-mm-yyyy");

                    int Dilatation = (CurrentDilPrescrip == null) ? 0 : 1;

                    int Collyre = (CurrentPrescripLastStep == null) ? 0 : Convert.ToInt32(CurrentPrescripLastStep.Collyre);
                    string NomCollyre = (CurrentPrescripLastStep == null) ? "" : CurrentPrescripLastStep.NomCollyre;
                    //recupartion de l'ancienne prescription

                    string RESphere = "",
                            RECylinder = "",
                            REAxis = "",
                            REAddition = "",
                            LESphere = "",
                            LECylinder = "",
                            LEAxis = "",
                            LEAddition = "";

                    string SupplyingName = "", LensCategoryCode = "0", TypeLens = "";//, plainte = "";
                    string ProductName = "", marque = "", NumeroSerie = "", reference = "";
                    int ProductCatID = 0;

                    string Axis = "", Addition = "", LensNumberCylindricalValue = "", LensNumberSphericalValue = "";


                    List<ATCDPersonnel> lstATCDPersonnel = db.ATCDPersonnels.Where(c => c.CustomerID == GlobalPersonID).ToList();
                    foreach (var actionSM in lstATCDPersonnel)
                    {

                        _ModelCheckATCDPersoList.Add(new ModelCheckATCDPerso
                        {
                            CheckATCDPersoID = actionSM.ATCDID,
                            CheckATCDPersoName = "ATCDPerso" + actionSM.ATCDID,
                            CheckATCDPersoValRmq = actionSM.Remarques
                        });

                    }


                    List<ATCDFamilial> lstATCDFamiliaux = db.ATCDFamiliaux.Where(c => c.CustomerID == GlobalPersonID).ToList();
                    foreach (var actionSM in lstATCDFamiliaux)
                    {

                        _ModelCheckATCDFamList.Add(new ModelCheckATCDFam
                        {
                            CheckATCDFamID = actionSM.ATCDID,
                            CheckATCDFamName = "ATCDFam" + actionSM.ATCDID,
                            CheckATCDFamValRmq = actionSM.Remarques
                        });

                    }


                    //old prescription
                    var OldPrescript = (from l1 in db.Customers
                                        join lc in db.Consultations on l1.GlobalPersonID equals lc.CustomerID
                                        join l2 in db.Sales on l1.GlobalPersonID equals l2.CustomerID
                                        join l3 in db.SaleLines on l2.SaleID equals l3.SaleID
                                        where l1.GlobalPersonID == GlobalPersonID && lc.ConsultationID == consultinfo.ConsultationID
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

                    string DateDernierConsultation = (OldPrescript.ToList().Count == 0) ? "" : (OldPrescript.FirstOrDefault().DateDernierConsultation != null) ? OldPrescript.FirstOrDefault().DateDernierConsultation.Value.ToString("dd-MM-yyyy") : "";
                    foreach (var ancsaleLine in OldPrescript)
                    {

                        if ((ancsaleLine.Product is Lens) || (ancsaleLine.Product is OrderLens))
                        {
                            LensCategoryCode = ancsaleLine.Product.Category.CategoryCode;
                            LensCategory cat = (from cate in db.LensCategories
                                                where cate.CategoryCode == LensCategoryCode
                                                select cate).SingleOrDefault();

                            SupplyingName = (cat.SupplyingName != null && cat.SupplyingName.Length > 0) ? cat.SupplyingName : cat.CategoryCode;

                            ProductCatID = cat.CategoryID;

                            TypeLens = cat.TypeLens;

                            if ((ancsaleLine.LensNumberSphericalValue == null || ancsaleLine.LensNumberSphericalValue == "") && (ancsaleLine.LensNumberCylindricalValue == null || ancsaleLine.LensNumberCylindricalValue == "") && (ancsaleLine.Addition == null || ancsaleLine.Addition == ""))
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
                    if (customer.DateOfBirth.HasValue)
                    {
                        CustomerAge = (SessionBusinessDay(null) != null) ? SessionBusinessDay(null).BDDateOperation.Year - customer.DateOfBirth.Value.Year : DateTime.Today.Year - customer.DateOfBirth.Value.Year;
                    }
                    else
                    {
                        CustomerAge = 0;
                    }

                    list.Add(
                        new
                        {
                            GlobalPersonID = customer.GlobalPersonID,
                            CustomerName = string.Concat(customer.Name, " ", (customer.Description == "") ? "" : customer.Description),
                            AdressPhoneNumber = (customer.Adress == null) ? "" : customer.Adress.AdressPhoneNumber,
                            CustomerNumber = customer.CustomerNumber,
                            QuarterLabel = (customer.Adress.Quarter == null) ? "" : customer.Adress.Quarter.QuarterLabel,
                            DateOfBirth = (customer.DateOfBirth.HasValue) ? ((customer.DateOfBirth.Value.ToString("yyyy-MM-dd") == "1900-01-01") ? "" : customer.DateOfBirth.Value.ToString("yyyy-MM-dd")) : "",
                            Dateregister = (customer.Dateregister.HasValue) ? customer.Dateregister.Value.ToString("yyyy-MM-dd") : null,
                            Medecin = (Session["UserConnect"] != null) ? (string)Session["UserConnect"].ToString() : db.Users.Where(c => c.GlobalPersonID == SessionGlobalPersonID).FirstOrDefault().UserFullName,
                            plainte = (consultOldPrescr != null) ? consultOldPrescr.PlaintePatient : ((consultinfo != null) ? consultinfo.RaisonRdv : ""),
                            // plainte = (consultinfo !=null) ? consultinfo.RaisonRdv : ( (consultOldPrescr != null) ? consultOldPrescr.PlaintePatient : ""),
                            oldPlainte = (consultOldPrescr != null) ? consultOldPrescr.OldPlaintePatient : "",
                            AVLName = (consultOldPrescr == null) ? "" : (consultOldPrescr.AcuiteVisuelL == null) ? "" : consultOldPrescr.AcuiteVisuelL.Name,
                            OldAVLName = (consultOldPrescr == null) ? "" : (consultOldPrescr.OldAcuiteVisuelL == null) ? "" : consultOldPrescr.OldAcuiteVisuelL.Name,
                            OldAcuiteVisuelLID = (consultOldPrescr == null) ? 0 : (consultOldPrescr.OldAcuiteVisuelL == null) ? 0 : consultOldPrescr.OldAcuiteVisuelL.AcuiteVisuelLID,
                            AcuiteVisuelLID = (consultOldPrescr == null) ? 0 : (consultOldPrescr.AcuiteVisuelL == null) ? 0 : consultOldPrescr.AcuiteVisuelL.AcuiteVisuelLID,

                            OldAVPName = (consultOldPrescr == null) ? "" : (consultOldPrescr.OldAcuiteVisuelP == null) ? "" : consultOldPrescr.OldAcuiteVisuelP.Name,
                            AVPName = (consultOldPrescr == null) ? "" : (consultOldPrescr.AcuiteVisuelP == null) ? "" : consultOldPrescr.AcuiteVisuelP.Name,
                            AcuiteVisuelPID = (consultOldPrescr == null) ? 0 : (consultOldPrescr.AcuiteVisuelP == null) ? 0 : consultOldPrescr.AcuiteVisuelP.AcuiteVisuelPID,
                            OldAcuiteVisuelPID = (consultOldPrescr == null) ? 0 : (consultOldPrescr.OldAcuiteVisuelP == null) ? 0 : consultOldPrescr.OldAcuiteVisuelP.AcuiteVisuelPID,


                            OldSupplyingName = SupplyingName,
                            OldLensCategoryCode = LensCategoryCode,
                            OldProductCat = (consultOldPrescr != null) ? consultOldPrescr.CategoryID : ProductCatID,

                            OldRESphere = (consultOldPrescr != null) ? consultOldPrescr.RSphValue : RESphere,
                            OldRECylinder = (consultOldPrescr != null) ? consultOldPrescr.RCylValue : RECylinder,
                            OldREAxis = (consultOldPrescr != null) ? consultOldPrescr.RAxis : REAxis,
                            OldREAddition = (consultOldPrescr != null) ? consultOldPrescr.RAddition : REAddition,
                            OldLESphere = (consultOldPrescr != null) ? consultOldPrescr.LSphValue : LESphere,
                            OldLECylinder = (consultOldPrescr != null) ? consultOldPrescr.LCylValue : LECylinder,
                            OldLEAxis = (consultOldPrescr != null) ? consultOldPrescr.LAxis : LEAxis,
                            OldLEAddition = (consultOldPrescr != null) ? consultOldPrescr.LAddition : LEAddition,
                            OldTypeLens = TypeLens,

                            PrescriptionLStepID = (CurrentPrescripLastStep == null) ? 0 : CurrentPrescripLastStep.PrescriptionLStepID,
                            ConsultationID = consultinfo.ConsultationID,

                            ModelCheckATCDFam = _ModelCheckATCDFamList,
                            ModelCheckATCDPerso = _ModelCheckATCDPersoList,

                            ProductName = ProductName,
                            marque = marque,
                            NumeroSerie = NumeroSerie,
                            reference = reference,

                            DateDernierConsultation = (consultOldPrescr != null) ? consultOldPrescr.DateDernierConsultation.ToString("dd-mm-yyyy") : DateDernierConsultation,

                            OldMedecin = Medecin,
                            OldDilatation = (consultOldPrescr != null) ? Convert.ToInt32(consultOldPrescr.OldDilatation) : Dilatation,
                            OldCollyre = (consultOldPrescr != null) ? Convert.ToInt32(consultOldPrescr.OldCollyre) : 0,
                            OldNomCollyre = (consultOldPrescr != null) ? consultOldPrescr.OldNomCollyre : "",

                            Collyre = Collyre,
                            NomCollyre = NomCollyre,
                            Remarque = Remarque,
                            DateProchaineCunsultation = DateProchaineCunsultation,

                            ConsultOldPrescrID = (consultOldPrescr != null) ? consultOldPrescr.ConsultOldPrescrID : 0,
                            ConsultPersonalMedHistoID = (ConsultPersonalMedHisto != null) ? ConsultPersonalMedHisto.ConsultPersonalMedHistoID : 0,
                            ConsultDilPrescID = (CurrentDilPrescrip != null) ? CurrentDilPrescrip.ConsultDilPrescID : (consultCurrentPrescr != null) ? consultCurrentPrescr.ConsultDilPrescID : 0,
                            ConsultDilatationID = (CurrentDilPrescrip != null) ? CurrentDilPrescrip.ConsultDilatationID : 0,

                            //new prescription
                            LensCategoryCode = (consultCurrentPrescr != null) ? consultCurrentPrescr.CategoryID : 0,
                            SupplyingName = (consultCurrentPrescr != null) ? consultCurrentPrescr.SupplyingName : "",
                            RESphere = (consultCurrentPrescr != null) ? consultCurrentPrescr.RSphValue : "",
                            RECylinder = (consultCurrentPrescr != null) ? consultCurrentPrescr.RCylValue : "",
                            REAxis = (consultCurrentPrescr != null) ? consultCurrentPrescr.RAxis : "",
                            REAddition = (consultCurrentPrescr != null) ? consultCurrentPrescr.RAddition : "",
                            LESphere = (consultCurrentPrescr != null) ? consultCurrentPrescr.LSphValue : "",
                            LECylinder = (consultCurrentPrescr != null) ? consultCurrentPrescr.LCylValue : "",
                            LEAxis = (consultCurrentPrescr != null) ? consultCurrentPrescr.LAxis : "",
                            LEAddition = (consultCurrentPrescr != null) ? consultCurrentPrescr.LAddition : "",
                            ConsultLensPrescriptionID = (consultCurrentPrescr != null) ? consultCurrentPrescr.ConsultLensPrescriptionID : 0,

                            // Old AVL(R & L)
                            OldRAVLName = (consultOldPrescr == null) ? "" : (consultOldPrescr.OldRAcuiteVisuelL == null) ? "" : consultOldPrescr.OldRAcuiteVisuelL.Name,
                            OldRAcuiteVisuelLID = (consultOldPrescr == null) ? 0 : (consultOldPrescr.OldRAcuiteVisuelL == null) ? 0 : consultOldPrescr.OldRAcuiteVisuelL.AcuiteVisuelLID,
                            OldLAVLName = (consultOldPrescr == null) ? "" : (consultOldPrescr.OldLAcuiteVisuelL == null) ? "" : consultOldPrescr.OldLAcuiteVisuelL.Name,
                            OldLAcuiteVisuelLID = (consultOldPrescr == null) ? 0 : (consultOldPrescr.OldLAcuiteVisuelL == null) ? 0 : consultOldPrescr.OldLAcuiteVisuelL.AcuiteVisuelLID,

                            // Old AVP(R & L)
                            OldRAVPName = (consultOldPrescr == null) ? "" : (consultOldPrescr.OldRAcuiteVisuelP == null) ? "" : consultOldPrescr.OldRAcuiteVisuelP.Name,
                            OldRAcuiteVisuelPID = (consultOldPrescr == null) ? 0 : (consultOldPrescr.OldRAcuiteVisuelP == null) ? 0 : consultOldPrescr.OldRAcuiteVisuelP.AcuiteVisuelPID,
                            OldLAVPName = (consultOldPrescr == null) ? "" : (consultOldPrescr.OldLAcuiteVisuelP == null) ? "" : consultOldPrescr.OldLAcuiteVisuelP.Name,
                            OldLAcuiteVisuelPID = (consultOldPrescr == null) ? 0 : (consultOldPrescr.OldLAcuiteVisuelP == null) ? 0 : consultOldPrescr.OldLAcuiteVisuelP.AcuiteVisuelPID,

                            //  AVL(R & L)
                            RAVLName = (consultOldPrescr == null) ? "" : (consultOldPrescr.RAcuiteVisuelL == null) ? "" : consultOldPrescr.RAcuiteVisuelL.Name,
                            RAcuiteVisuelLID = (consultOldPrescr == null) ? 0 : (consultOldPrescr.RAcuiteVisuelL == null) ? 0 : consultOldPrescr.RAcuiteVisuelL.AcuiteVisuelLID,
                            LAVLName = (consultOldPrescr == null) ? "" : (consultOldPrescr.LAcuiteVisuelL == null) ? "" : consultOldPrescr.LAcuiteVisuelL.Name,
                            LAcuiteVisuelLID = (consultOldPrescr == null) ? 0 : (consultOldPrescr.LAcuiteVisuelL == null) ? 0 : consultOldPrescr.LAcuiteVisuelL.AcuiteVisuelLID,

                            //  AVP(R & L)
                            RAVPName = (consultOldPrescr == null) ? "" : (consultOldPrescr.RAcuiteVisuelP == null) ? "" : consultOldPrescr.RAcuiteVisuelP.Name,
                            RAcuiteVisuelPID = (consultOldPrescr == null) ? 0 : (consultOldPrescr.RAcuiteVisuelP == null) ? 0 : consultOldPrescr.RAcuiteVisuelP.AcuiteVisuelPID,
                            LAVPName = (consultOldPrescr == null) ? "" : (consultOldPrescr.LAcuiteVisuelP == null) ? "" : consultOldPrescr.LAcuiteVisuelP.Name,
                            LAcuiteVisuelPID = (consultOldPrescr == null) ? 0 : (consultOldPrescr.LAcuiteVisuelP == null) ? 0 : consultOldPrescr.LAcuiteVisuelP.AcuiteVisuelPID,

                            //  AVLTS(R & L)
                            RAVLTSName = (consultOldPrescr == null) ? "" : (consultOldPrescr.RAVLTS == null) ? "" : consultOldPrescr.RAVLTS.Name,
                            RAVLTSID = (consultOldPrescr == null) ? 0 : (consultOldPrescr.RAVLTS == null) ? 0 : consultOldPrescr.RAVLTS.AVLTSID,
                            LAVLTSName = (consultOldPrescr == null) ? "" : (consultOldPrescr.LAVLTS == null) ? "" : consultOldPrescr.LAVLTS.Name,
                            LAVLTSID = (consultOldPrescr == null) ? 0 : (consultOldPrescr.LAVLTS == null) ? 0 : consultOldPrescr.LAVLTS.AVLTSID,

                            CustomerAge = CustomerAge,
                            IsBillCustomer = customer.IsBillCustomer
                            //  OldAVLTS(R & L)
                            //OldRAVLTSName = (consultOldPrescr == null) ? "" : (consultOldPrescr.OldRAVLTS == null) ? "" : consultOldPrescr.OldRAVLTS.Name,
                            //OldRAVLTSID = (consultOldPrescr == null) ? 0 : (consultOldPrescr.OldRAVLTS == null) ? 0 : consultOldPrescr.OldRAVLTS.AVLTSID,
                            //OldLAVLTSName = (consultOldPrescr == null) ? "" : (consultOldPrescr.OldLAVLTS == null) ? "" : consultOldPrescr.OldLAVLTS.Name,
                            //OldLAVLTSID = (consultOldPrescr == null) ? 0 : (consultOldPrescr.OldLAVLTS == null) ? 0 : consultOldPrescr.OldLAVLTS.AVLTSID,


                        });
                }

                Session["Receipt_GlobalPersonID"] = GlobalPersonID;
                ViewBag.GlobalPersonID = GlobalPersonID;
                return Json(list, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                string message = e.Message;
                list = new List<object>();
                Session["Receipt_GlobalPersonID"] = null;
                return Json(list, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                _ModelCheckATCDFamList = new List<ModelCheckATCDFam>();
                _ModelCheckATCDPersoList = new List<ModelCheckATCDPerso>();
            }


        }
        //public JsonResult GetOptionOldDilatation()
        //{
        //    List<object> OptionList = new List<object>();
        //    OptionList.Add(
        //        new
        //        {
        //            ID=0,
        //            Name="False"
        //        });
        //    OptionList.Add(
        //        new
        //        {
        //            ID = 1,
        //            Name = "True"
        //        });
        //    return Json(OptionList, JsonRequestBehavior.AllowGet);
        //}

        public JsonResult GetAllLensCategories()
        {

            List<object> LensCategorList = new List<object>();

            IRepositorySupply<LensCategory> prod = new RepositorySupply<LensCategory>(db);

            prod.FindAll.ToList().ForEach(productcat =>
            {
                LensCategorList.Add(new
                {
                    CategoryCode = productcat.CategoryCode,
                    CategoryID = productcat.CategoryID
                });
            });

            return Json(LensCategorList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult checkifDilation(int CustomerNumber, int? ConsultDilPrescID = 0)
        {
            List<object> _InfoList = new List<object>();
            if (CustomerNumber > 0)
            {
                //check if this customer its insured customer
                Customer cust = db.Customers.Where(c => c.GlobalPersonID == CustomerNumber).SingleOrDefault();
                if (cust != null)
                {
                    if (cust.IsBillCustomer)
                    {
                        _InfoList.Add(new
                        {
                            // Car les clients assurees ne paient pas la dilatation
                            CodeDilation = cust.GlobalPersonID,
                            IsBillCustomer = cust.IsBillCustomer
                        });
                    }
                    else
                    {
                        if (ConsultDilPrescID.HasValue && ConsultDilPrescID.Value > 0)
                        {

                            ConsultDilatation prescDilatation = (from p in db.ConsultDilatations
                                                                 where p.ConsultDilPrescID == ConsultDilPrescID
                                                                 select p).SingleOrDefault();
                            if (prescDilatation != null)
                            {
                                _InfoList.Add(new
                                {
                                    CodeDilation = prescDilatation.CodeDilation,
                                    ConsultDilatationID = prescDilatation.ConsultDilatationID
                                });
                            }
                        }
                    }

                }

            }

            return Json(_InfoList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SetSupplyingName(int LensCategoryCode = 0)
        {

            List<object> _InfoList = new List<object>();
            if (LensCategoryCode > 0)
            {
                LensCategory cat = (from cate in db.LensCategories
                                    where cate.CategoryID == LensCategoryCode
                                    select cate).SingleOrDefault();
                if (cat != null)
                {
                    _InfoList.Add(new
                    {
                        //LensLineQuantity = 2,
                        SupplyingName = (cat.SupplyingName != null && cat.SupplyingName.Length > 0) ? cat.SupplyingName : cat.CategoryCode,
                        TypeLens = cat.TypeLens
                    });
                }
            }
            else
            {
                _InfoList.Add(new
                {
                    SupplyingName = "",
                    TypeLens = ""
                });
            }

            return Json(_InfoList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SetOldProductCat(int OldProductCat = 0)
        {

            List<object> _InfoList = new List<object>();
            if (OldProductCat > 0)
            {
                LensCategory cat = (from cate in db.LensCategories
                                    where cate.CategoryID == OldProductCat
                                    select cate).SingleOrDefault();
                if (cat != null)
                {
                    _InfoList.Add(new
                    {
                        TypeLens = cat.TypeLens
                    });
                }
            }
            else
            {
                _InfoList.Add(new
                {
                    TypeLens = ""
                });
            }

            return Json(_InfoList, JsonRequestBehavior.AllowGet);
        }


        public JsonResult ValideStepTwo(ConsultOldPrescr consultoldpres, DateTime DateOfBirth, int IsOldCollyre = 0, int IsOldDilatation = 0)
        {
            bool status = false;
            string Message = "";
            int ConsultOldPrescrID = 0;
            try
            {


                consultoldpres.CategoryID = (consultoldpres.CategoryID == 0) ? null : consultoldpres.CategoryID;

                consultoldpres.OldAcuiteVisuelLID = (consultoldpres.OldAcuiteVisuelLID == 0) ? null : consultoldpres.OldAcuiteVisuelLID;
                consultoldpres.OldRAcuiteVisuelLID = (consultoldpres.OldRAcuiteVisuelLID == 0) ? null : consultoldpres.OldRAcuiteVisuelLID;
                consultoldpres.OldLAcuiteVisuelLID = (consultoldpres.OldLAcuiteVisuelLID == 0) ? null : consultoldpres.OldLAcuiteVisuelLID;

                consultoldpres.OldAcuiteVisuelPID = (consultoldpres.OldAcuiteVisuelPID == 0) ? null : consultoldpres.OldAcuiteVisuelPID;
                consultoldpres.OldRAcuiteVisuelPID = (consultoldpres.OldRAcuiteVisuelPID == 0) ? null : consultoldpres.OldRAcuiteVisuelPID;
                consultoldpres.OldLAcuiteVisuelPID = (consultoldpres.OldLAcuiteVisuelPID == 0) ? null : consultoldpres.OldLAcuiteVisuelPID;

                consultoldpres.DateConsultOldPres = SessionBusinessDay(null).BDDateOperation;
                consultoldpres.ConsultByID = SessionGlobalPersonID;
                consultoldpres.IsCollyre = Convert.ToBoolean(IsOldCollyre);
                consultoldpres.IsDilatation = Convert.ToBoolean(IsOldDilatation);
                consultoldpres.DateDernierConsultation = (consultoldpres.DateDernierConsultation <= new DateTime(1900, 1, 1)) ? new DateTime(1900, 1, 1) : consultoldpres.DateDernierConsultation;
                consultoldpres.DateOfBirth = DateOfBirth;

                if (consultoldpres.CategoryID > 0)
                {
                    ConsultLensPrescription lensPrescription = LensConstruction.getConsultLensPrescriptionFromConsultOldPrescr(consultoldpres);
                    LensValidationError lensValidation = LensConstruction.validateLens(lensPrescription);
                    if (lensValidation.code != LensValidationErrorCode.SUCCESS)
                    {
                        status = false;
                        Message = "Error " + lensValidation.errorMessage;
                        return new JsonResult { Data = new { status = status, Message = Message, ConsultDilPrescID = 0 } };
                    }
                }

                ConsultOldPrescrID = consultoldpres.ConsultOldPrescrID;
                if (consultoldpres.ConsultOldPrescrID == 0)
                {
                    ConsultOldPrescrID = _prescriptionRepository.SaveConsultoldpres(consultoldpres, SessionGlobalPersonID, SessionBusinessDay(null).BranchID).ConsultOldPrescrID;
                }
                else
                {
                    ConsultOldPrescrID = _prescriptionRepository.UpdateConsultoldpres(consultoldpres, SessionGlobalPersonID, SessionBusinessDay(null).BranchID).ConsultOldPrescrID;
                }

                status = true;
                Message = Resources.Success + " - " + Resources.AnciennePrescription + " / " + Resources.Plainte;
            }
            catch (Exception e)
            {
                status = false;
                Message = "Error " + e.Message;
            }

            return new JsonResult { Data = new { status = status, Message = Message, ConsultOldPrescrID = ConsultOldPrescrID } };
        }

        public JsonResult ValideStepThree(ConsultPersonalMedHisto consultPersonalMedHisto, List<int> defaultATCDPerso, List<int> defaultATCDFam, string ATCDPersoAutre, string ATCDFamAutre)
        {
            bool status = false;
            string Message = "";
            int consultPersonalMedHistoID = 0;
            try
            {

                List<int> allATCDPerso = (defaultATCDPerso == null) ? new List<int>() : defaultATCDPerso;
                List<int> allATCDFam = (defaultATCDFam == null) ? new List<int>() : defaultATCDFam;

                consultPersonalMedHisto.ATCDPersoAutre = ATCDPersoAutre;
                consultPersonalMedHisto.ATCDFamAutre = ATCDFamAutre;

                consultPersonalMedHisto.DateConsultPersonalMedHisto = SessionBusinessDay(null).BDDateOperation;
                consultPersonalMedHisto.ConsultByID = SessionGlobalPersonID;

                if (consultPersonalMedHisto.ConsultPersonalMedHistoID == 0)
                {
                    consultPersonalMedHistoID = _prescriptionRepository.SaveChangesConsultPersonalMedHisto(consultPersonalMedHisto, allATCDPerso, allATCDFam, SessionGlobalPersonID, SessionBusinessDay(null).BranchID).ConsultPersonalMedHistoID;
                }
                else
                {
                    consultPersonalMedHistoID = _prescriptionRepository.UpdateChangesconsultPersonalMedHisto(consultPersonalMedHisto, allATCDPerso, allATCDFam, SessionGlobalPersonID, SessionBusinessDay(null).BranchID).ConsultPersonalMedHistoID;
                }

                status = true;
                Message = Resources.Success + " - " + Resources.AntedecentPersonnel;
            }
            catch (Exception e)
            {
                List<int> allATCDPerso = new List<int>();
                List<int> allATCDFam = new List<int>();
                status = false;
                Message = "Error " + e.Message; // + " " + e.StackTrace + " " + e.InnerException;
            }
            finally
            {
                List<int> allATCDPerso = new List<int>();
                List<int> allATCDFam = new List<int>();
            }
            return new JsonResult { Data = new { status = status, Message = Message, ConsultPersonalMedHistoID = consultPersonalMedHistoID } };
        }

        public JsonResult ValideStepFour(ConsultDilPresc consultDilPresc, ConsultDilatation newDilation, ConsultLensPrescription newLensPrescrip, bool fielsetprescripActivate, bool fieldactivePrescriptionLens, bool isDilation, string CodeDilation, string heureVente)
        {
            bool status = false;
            string Message = "";
            int ConsultDilPrescID = 0;
            try
            {

                if (isDilation) //prescription of dilation
                {
                    newDilation.ConsultByID = SessionGlobalPersonID;
                    newDilation.DateDilation = SessionBusinessDay(null).BDDateOperation;
                    newDilation.HeureConsultDilatation = heureVente;
                    if (consultDilPresc.ConsultDilPrescID == 0)
                    {
                        ConsultDilPrescID = _prescriptionRepository.SaveConsultDilatation(consultDilPresc, newDilation, SessionGlobalPersonID, SessionBusinessDay(null).BranchID).ConsultDilPrescID;
                    }
                    else
                    {
                        newDilation.ConsultDilPrescID = consultDilPresc.ConsultDilPrescID;
                        ConsultDilPrescID = _prescriptionRepository.UpdateConsultDilatation(newDilation, SessionGlobalPersonID, SessionBusinessDay(null).BranchID).ConsultDilPrescID;
                    }

                }
                else
                {
                    if (consultDilPresc.ConsultDilPrescID > 0)
                    { // Si la dilatation a ete supprimee
                      // Suppression de la dilatation existante
                        _prescriptionRepository.DeleteConsultDilatation(consultDilPresc.ConsultDilPrescID, SessionGlobalPersonID, SessionBusinessDay(null).BranchID, SessionBusinessDay(null).BDDateOperation);

                    }
                }

                if (fielsetprescripActivate)
                {
                    if (fieldactivePrescriptionLens)
                    {
                        LensValidationError lensValidation = LensConstruction.validateLens(newLensPrescrip);
                        if (lensValidation.code != LensValidationErrorCode.SUCCESS)
                        {
                            status = false;
                            Message = "Error " + lensValidation.errorMessage;
                            return new JsonResult { Data = new { status = status, Message = Message, ConsultDilPrescID = ConsultDilPrescID } };
                        }
                        newLensPrescrip.ConsultByID = SessionGlobalPersonID;
                        newLensPrescrip.HeureLensPrescription = heureVente;
                        newLensPrescrip.DatePrescription = SessionBusinessDay(null).BDDateOperation;

                        if (consultDilPresc.ConsultDilPrescID == 0) //prescription without dilation
                        {
                            ConsultDilPrescID = _prescriptionRepository.SaveConsultLensPrescription(consultDilPresc, newLensPrescrip, SessionGlobalPersonID, SessionBusinessDay(null).BranchID).ConsultDilPrescID;
                        }
                        else // prescription with dilation
                        {
                            newLensPrescrip.ConsultDilPrescID = consultDilPresc.ConsultDilPrescID;
                            ConsultDilPrescID = _prescriptionRepository.UpdateConsultLensPrescription(newLensPrescrip, SessionGlobalPersonID, SessionBusinessDay(null).BranchID).ConsultDilPrescID;
                        }
                    }
                    else
                    {
                        if (consultDilPresc.ConsultDilPrescID > 0)
                        { // Si la prescription a ete supprimee
                          // Suppression de la prescription existante
                            _prescriptionRepository.DeleteConsultLensPrescription(consultDilPresc.ConsultDilPrescID, SessionGlobalPersonID, SessionBusinessDay(null).BranchID, SessionBusinessDay(null).BDDateOperation);
                        }
                    }
                }
                else
                {
                    /*if (consultDilPresc.ConsultDilPrescID > 0)
                    { // Si la prescription a ete supprimee
                      // Suppression de la prescription existante
                      _prescriptionRepository.DeleteConsultLensPrescription(consultDilPresc.ConsultDilPrescID, SessionGlobalPersonID, SessionBusinessDay(null).BranchID, SessionBusinessDay(null).BDDateOperation);
                    }*/
                }

                status = true;
                Message = Resources.Success + " - " + Resources.Dilatation + " / " + Resources.prescription;
            }
            catch (Exception e)
            {
                status = false;
                Message = "Error " + e.Message;
            }

            return new JsonResult { Data = new { status = status, Message = Message, ConsultDilPrescID = ConsultDilPrescID } };
        }


        public JsonResult AddPrescriptionLStep(PrescriptionLStep currentPrescription, bool fieldactivePrescriptionLens, string heureVente)
        {
            bool status = false;
            string Message = "";
            try
            {


                currentPrescription.DatePrescriptionLStep = SessionBusinessDay(null).BDDateOperation;

                if (currentPrescription.DateRdv > currentPrescription.DatePrescriptionLStep || (currentPrescription.Remarque != null || currentPrescription.Remarque.Trim() != ""))
                {
                    fieldactivePrescriptionLens = true;
                }
                int prescriptionID = 0;
                if (currentPrescription.PrescriptionLStepID == 0)
                {
                    prescriptionID = _prescriptionRepository.SaveChanges(currentPrescription, fieldactivePrescriptionLens, heureVente, SessionGlobalPersonID, SessionBusinessDay(null).BranchID).PrescriptionLStepID;
                }
                else
                {
                    prescriptionID = _prescriptionRepository.UpdateChanges(currentPrescription, fieldactivePrescriptionLens, heureVente, SessionGlobalPersonID, SessionBusinessDay(null).BranchID).PrescriptionLStepID;
                }

                status = true;
                Message = Resources.Success + " - " + Resources.NEWPresciption;
            }
            catch (Exception e)
            {
                status = false;
                Message = "Error " + e.Message;
            }
            finally
            {
                Session["Receipt_GlobalPersonID"] = null;
            }
            return new JsonResult { Data = new { status = status, Message = Message } };
        }
    }
}