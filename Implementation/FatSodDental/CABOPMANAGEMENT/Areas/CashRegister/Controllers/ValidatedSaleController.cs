using FastSod.Utilities.Util;
using FatSod.DataContext.Initializer;
using FatSod.Ressources;
using FatSod.Security.Abstracts;
using FatSod.Security.Entities;
using FatSod.Supply.Abstracts;
using FatSod.Supply.Entities;
using CABOPMANAGEMENT.Controllers;
using CABOPMANAGEMENT.Filters;
using CABOPMANAGEMENT.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using FatSod.Report.WrapReports;
using SaleE = FatSod.Supply.Entities.Sale;

namespace CABOPMANAGEMENT.Areas.CashRegister.Controllers
{
    [Authorize(Order = 1)]
    [TakeBusinessDay(Order = 2)]
    public class ValidatedSaleController : BaseController
    {
        //Current Controller and current page
        private const string CONTROLLER_NAME = "CashRegister/ValidatedSale";
        private const string VIEW_NAME = "Index";

        private ISale _saleRepository;
        private IBusinessDay _busDayRepo;
        private ISavingAccount _savingAccountRepository;
        private IAuthoriseSale _authoriseSaleRepository;


        private ITransactNumber _transactNumbeRepository;
        private ITillDay _tillDayRepository;
        

        private LensConstruction lensFrameConstruction = new LensConstruction();

        public ValidatedSaleController(
            ISale saleRepository,
            IBusinessDay busDayRepo,
            ISavingAccount saRepo,
            IAuthoriseSale authoriseSaleRepository,
            ITillDay tillDayRepository,
            ITransactNumber transactNumbeRepository
        )
        {
            this._saleRepository = saleRepository;
            this._busDayRepo = busDayRepo;
            this._savingAccountRepository = saRepo;
            this._transactNumbeRepository = transactNumbeRepository;
            this._authoriseSaleRepository = authoriseSaleRepository;
            this._tillDayRepository = tillDayRepository;
        }

        [OutputCache(Duration = 3600)]
        public ActionResult Index()
        {
            ViewBag.DisplayForm = 1;
            try
            {
                List<BusinessDay> UserBusDays = (List<BusinessDay>)Session["UserBusDays"];
                Session["businessDay"] = UserBusDays.FirstOrDefault();
                DateTime currentDateOp = UserBusDays.FirstOrDefault().BDDateOperation;
                ViewBag.CurrentBranch = UserBusDays.FirstOrDefault().BranchID;
                ViewBag.BusnessDayDate = UserBusDays.FirstOrDefault().BDDateOperation.ToString("yyyy-MM-dd");// businessDay.BDDateOperation;

                Session["BusnessDayDate"] = currentDateOp;

                UserTill userTill = (from td in db.UserTills
                                     where td.UserID == SessionGlobalPersonID
                                     select td).SingleOrDefault();
                if (userTill == null || userTill.TillID <= 0)
                {
                    TempData["Message"] = "Access Denied - You can't do sale operation. If you think no, try again or<br/>contact our administrator for this purpose<code/>.";
                    ViewBag.DisplayForm = 0;
                    return this.View();
                }

                TillDayStatus tState = _tillDayRepository.TillDayStatus(userTill.TillID);
                if (tState == null)
                {
                    TempData["Message"] = "Error - Bad Configuration of Cash Register!!! Please call Your database Administrator";
                    ViewBag.DisplayForm = 0;
                    return this.View();
                }
                if (!tState.IsOpen)
                {
                    TempData["Message"] = "Error - This Cash Register is Still Close!!! Please Open It Before Proceed";
                    ViewBag.DisplayForm = 0;
                    return this.View();
                }

                TillDay currentTillDay = (from t in db.TillDays
                                          where
                                              t.TillID == userTill.TillID && t.TillDayDate == tState.TillDayLastOpenDate.Date && t.IsOpen // t.TillDayDate.Day == currentDateOp.Day && t.TillDayDate.Month == currentDateOp.Month && t.TillDayDate.Year == currentDateOp.Year && t.IsOpen
                                          select t).FirstOrDefault();
                if (currentTillDay == null)
                {
                    TempData["Message"] = "Warnnig - Cash register is closed. You must open it before do any sale<br/>Go at Cash Register module=>Open cash register<code/>";
                    ViewBag.DisplayForm = 0;
                    return this.View();
                }


                ViewBag.CurrentTill = userTill.TillID;

                int deviseID = (Session["DefaultDeviseID"] == null) ? 0 : (int)Session["DefaultDeviseID"];
                if (deviseID <= 0)
                {
                    InjectUserConfigInSession();
                }
                deviseID = (Session["DefaultDeviseID"] == null) ? 0 : (int)Session["DefaultDeviseID"];
                ViewBag.DefaultDeviseID = deviseID;
                ViewBag.DefaultDevise = (deviseID <= 0) ? "" : db.Devises.Find(deviseID).DeviseCode;

                Session["DebtInsured"] = 0d;
                Session["salelinesnoninsured"] = new List<SaleLine>();
                return View(ModelSaleValidate());
            }

            catch (Exception e)
            {
                TempData["Message"] = "Error " + e.Message;
                ViewBag.DisplayForm = 0;
                return this.View();
            }
        }

        public JsonResult ModelSaleValidate()
        {

            var model = new
            {
                data = from c in db.AuthoriseSales.Where(c => !c.IsDelivered && !c.IsSpecialOrder && !c.IsDilatation).ToList()
                select
                new
                {
                    AuthoriseSaleID = c.AuthoriseSaleID,
                    SaleDate = c.SaleDate.ToString("yyyy-MM-dd"),
                    TotalPriceHT = Util.ExtraPrices(c.AuthoriseSaleLines.Select(sl => sl.LineAmount).Sum(),
                                            c.RateReduction, c.RateDiscount, c.Transport, c.VatRate).TotalTTC,
                    SaleReceiptNumber = c.SaleReceiptNumber,
                    CustomerName = c.Customer.CustomerFullName,
                    IsUrgentUI = c.IsUrgent ? "YES" : "NO",
                    IsUrgent = c.IsUrgent
                }
            };
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public JsonResult InitializeCommandFields(int ID)
        {
            List<object> _CommandList = new List<object>();
            try
            {

                AuthoriseSale autSale = (from c in db.AuthoriseSales
                                               where c.AuthoriseSaleID == ID && !c.IsDelivered
                                               select c).SingleOrDefault();
                if (autSale == null)
                {
                    TempData["Message"] = "Warning - This sale is already validate";
                    return Json(_CommandList, JsonRequestBehavior.AllowGet);
                }

                string ProductName = "", marque="", NumeroSerie = "", reference="";
                int ProductID = 0, LocalizationID=0, ProductCategoryID = 0;

                string RESphere = "",
                        RECylinder = "",
                        REAxis = "",
                        REAddition = "",
                        LESphere = "",
                        LECylinder = "",
                        LEAxis = "",
                        LEAddition = "";
                bool isRCommandGlass = false, isLCommandGlass = false;

                string SupplyingName = "", LensCategoryCode="0", TypeLens="";
                double FramePrice = 0d, FrameLineQuantity = 0d, StockQuantity = 0d;
                double LensPrice = 0d, LensLineQuantity = 0d, LineUnitPrice = 0d ;
                double FinalFramePrice = 0d, FinalLensPrice = 0d;
                string Axis = "", Addition = "", LensNumberCylindricalValue = "", LensNumberSphericalValue = "";

                List<AuthoriseSaleLine> authoriseSaleLines = db.AuthoriseSaleLines.Where(co => co.AuthoriseSaleID == ID).ToList();

                foreach (AuthoriseSaleLine authosaleLine in authoriseSaleLines)
                {
                    if (authosaleLine.Product is GenericProduct)
                    {
                        //frame - spray or boitier
                        if (authosaleLine.reference!=null || authosaleLine.marque!=null) //frame
                        {
                            ProductName = authosaleLine.Product.ProductCode;
                            ProductID = authosaleLine.Product.ProductID;
                            StockQuantity=  GetQuantityStock(authosaleLine.Localization.LocalizationID.ToString(), authosaleLine.Product.ProductID.ToString(), authosaleLine.NumeroSerie, authosaleLine.marque);
                            marque = authosaleLine.marque;
                            reference = authosaleLine.reference;
                            NumeroSerie = authosaleLine.NumeroSerie;
                            FramePrice = authosaleLine.LineUnitPrice;
                            FinalFramePrice = authosaleLine.LineAmount;
                            FrameLineQuantity = authosaleLine.LineQuantity;
                        }
                    }

                    if ((authosaleLine.Product is Lens) || (authosaleLine.Product is OrderLens))
                    {
                        LensCategoryCode = authosaleLine.Product.Category.CategoryCode;
                        LensCategory cat = (from cate in db.LensCategories
                                            where cate.CategoryCode == LensCategoryCode
                                            select cate).SingleOrDefault();

                        SupplyingName = (cat.SupplyingName != null && cat.SupplyingName.Length > 0) ? cat.SupplyingName : cat.CategoryCode;
                        TypeLens = cat.TypeLens;
                        LensPrice += (authosaleLine.LineUnitPrice); // (autSale.SaleID != null) ? (authosaleLine.LineUnitPrice*2) : (authosaleLine.LineUnitPrice);
                        LensLineQuantity += authosaleLine.LineQuantity;
                        FinalLensPrice += authosaleLine.LineAmount;

                        ProductCategoryID = cat.CategoryID;


                        if ((authosaleLine.LensNumberSphericalValue == null || authosaleLine.LensNumberSphericalValue == "") && (authosaleLine.LensNumberCylindricalValue == null || authosaleLine.LensNumberCylindricalValue == "") && (authosaleLine.Addition == null || authosaleLine.Addition == ""))
                        {
                            if (authosaleLine.Product is Lens)
                            {
                                Lens lensProduct = db.Lenses.Find(authosaleLine.Product.ProductID);
                                Axis = authosaleLine.Axis;
                                Addition = lensProduct.LensNumber.LensNumberAdditionValue;
                                LensNumberCylindricalValue = lensProduct.LensNumber.LensNumberCylindricalValue;
                                LensNumberSphericalValue = lensProduct.LensNumber.LensNumberSphericalValue;
                            }
                            if (authosaleLine.Product is OrderLens)
                            {
                                OrderLens lensProduct = db.OrderLenses.Find(authosaleLine.Product.ProductID);
                                Axis = lensProduct.Axis;
                                Addition = lensProduct.Addition;
                                LensNumberCylindricalValue = lensProduct.LensNumberCylindricalValue;
                                LensNumberSphericalValue = lensProduct.LensNumberSphericalValue;
                            }


                        }
                        else
                        {
                            LensNumberSphericalValue = authosaleLine.LensNumberSphericalValue;
                            LensNumberCylindricalValue = authosaleLine.LensNumberCylindricalValue;
                            Axis = authosaleLine.Axis;
                            Addition = authosaleLine.Addition;
                        }

                        if (LensNumberSphericalValue == null) LensNumberSphericalValue = "";
                        if (LensNumberCylindricalValue == null) LensNumberCylindricalValue = "";
                        if (Addition == null) Addition = "";
                        if (authosaleLine.OeilDroiteGauche == EyeSide.OD)
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
                            isRCommandGlass = authosaleLine.isCommandGlass;
                        }
                        if (authosaleLine.OeilDroiteGauche == EyeSide.OG)
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
                            isLCommandGlass = authosaleLine.isCommandGlass;
                        }
                    }

                    LocalizationID = authosaleLine.LocalizationID;
                }

                LineUnitPrice = FinalFramePrice + FinalLensPrice;
                _CommandList.Add(new
                {
                    AuthoriseSaleID=autSale.AuthoriseSaleID,
                    CustomerName = (autSale.Customer == null) ? autSale.CustomerName : autSale.Customer.CustomerFullName,
                    CustomerID = autSale.CustomerID,
                    Remarque = autSale.Remarque,
                    MedecinTraitant = autSale.MedecinTraitant,
                    //spray = spray,
                    //boitier = boitier,
                    BranchID = autSale.BranchID,
                    CNI=autSale.CNI,
                    CustomerNumber=autSale.Customer.CustomerNumber,
                    
                    ProductName = ProductName,
                    ProductID = ProductID,
                    StockQuantity=StockQuantity,
                    marque=marque,
                    NumeroSerie = NumeroSerie,
                    FramePrice=FramePrice,
                    FrameLineQuantity=FrameLineQuantity,
                    reference=reference,
                   

                    SupplyingName = SupplyingName,
                    LensCategoryCode= LensCategoryCode,
                    LensPrice = LensPrice,
                    LensLineQuantity= LensLineQuantity,

                    LineUnitPrice= LineUnitPrice,

                    SaleReceiptNumber = autSale.SaleReceiptNumber,
                    SaleDate = autSale.SaleDate.ToString("yyyy-MM-dd"),

                    DateRdv=(autSale.DateRdv.HasValue) ? autSale.DateRdv.Value.ToString("yyyy-MM-dd") : "",
                    //SaleDeliveryDate = SessionBusinessDay(autSale.BranchID).BDDateOperation.ToString("yyyy-MM-dd"),

                    RESphere =RESphere,
                    RECylinder=RECylinder,
                    REAxis=REAxis,
                    REAddition=REAddition,
                    isRCommandGlass = isRCommandGlass,
                    LESphere =LESphere,
                    LECylinder=LECylinder,
                    LEAxis=LEAxis,
                    LEAddition=LEAddition,
                    isLCommandGlass = isLCommandGlass,
                    TypeLens = TypeLens,

                    LocalizationID= LocalizationID,
                    SalesProductsType = 1,
                    PostByID = autSale.PostByID,
                    ProductCategoryID = ProductCategoryID,
                    SellerID = autSale.SellerID
                });
                return Json(_CommandList, JsonRequestBehavior.AllowGet);


            }
            catch (Exception e)
            {
                TempData["Message"] = "Error " + e.Message;
                return Json(_CommandList, JsonRequestBehavior.AllowGet);
            }
        }
        //chargement des combo box
        public JsonResult populateBuyType()
        {
            var paymentMethodTypes = Utilities.PaymentMethodTypes();

            return Json(paymentMethodTypes, JsonRequestBehavior.AllowGet);
        }
        public JsonResult PaymentMethods(string BuyTypeCode)
        {
            List<object> model = new List<object>();
            db.PaymentMethods.OfType<Bank>().Where(p => p.Account.CollectifAccount.AccountingSection.AccountingSectionCode == BuyTypeCode).ToList().ForEach(p =>
            {
                model.Add(
                        new
                        {
                            ID = p.ID,
                            Name = p.Name
                        }
                    );
            });
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DigitalPaymentMethods(string BuyTypeCode)
        {
            List<object> model = new List<object>();
            db.DigitalPaymentMethods.ToList().ForEach(dpm =>
            {
                model.Add(
                        new
                        {
                            ID = dpm.ID,
                            Name = dpm.Name + " (" + dpm.Code + ")"
                        }
                    );
            });
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AddSale(SpecialLensModel slm, SaleE currentSale, CustomerSlice customerSlice, int? PaymentDelay, string BuyType, string heureVente, int spray = 0, int boitier = 0/*, int SaleDeliver = 0*/)
        {
            bool status = false;
            string Message = "";
            try
            {
                Session["Receipt_SaleID"] = null;
                Session["Receipt_CustomerID"] = null;
                Session["ReceiveAmoung_Tot"] = null;

                Session["salelinesnoninsured"] = new List<SaleLine>();

                if (BuyType == "" || BuyType == null)
                {
                    Message = "Wrong Payment Mode Select!!! " + Resources.MsgErrChoixPayementMethod;
                    status = false;
                    return new JsonResult { Data = new { status = status, Message = Message } };
                }

                //fabrication des lignes de commande
                status = this.DoYes(slm, spray, boitier);
                if (!status)
                {
                    Message = (string)Session["SessionMessage"];
                    status = false;
                    return new JsonResult { Data = new { status = status, Message = Message } };
                }

                //if (SaleDeliver == 0) currentSale.SaleDeliver = false;
                //if (SaleDeliver == 1) currentSale.SaleDeliver = true;

                currentSale.CustomerSlice = customerSlice;
                customerSlice.SliceDate = currentSale.SaleDate;
                currentSale.PaymentDelay = (PaymentDelay == null) ? 0 : PaymentDelay.Value;
                currentSale.IsSpecialOrder = false; //vente principale

                if (currentSale != null && currentSale.CustomerSlice.SliceAmount > 0 && BuyType == CodeValue.Accounting.DefaultCodeAccountingSection.Credit)
                {
                    Message = "Wrong Payment Mode Select!!! " + Resources.MsgWrongChoixPayementMethod;
                    status = false;
                    return new JsonResult { Data = new { status = status, Message = Message } };
                }

                var isAnyValidPMTypeSelected = (BuyType == CodeValue.Accounting.DefaultCodeAccountingSection.CODEBANK ||
                                                 BuyType == CodeValue.Accounting.DefaultCodeAccountingSection.CODECAIS ||
                                                 BuyType == CodeValue.Accounting.DefaultCodeAccountingSection.DIGITAL_PAYMENT);

                if (currentSale != null && currentSale.CustomerSlice.SliceAmount == 0 && isAnyValidPMTypeSelected)
                {
                    Message = "Wrong Payment Mode Select!!! " + Resources.MsgWrongChoixPayementMethod;
                    status = false;
                    return new JsonResult { Data = new { status = status, Message = Message } };
                }
                
                currentSale.SaleLines = (List<SaleLine>)Session["salelinesnoninsured"];

                //choix de la caisse
                if (BuyType == CodeValue.Accounting.DefaultCodeAccountingSection.CODECAIS)
                {
                    UserTill userTill = db.UserTills.FirstOrDefault(td => td.UserID == SessionGlobalPersonID);
                    currentSale.PaymentMethodID = userTill.TillID;
                }

                

                //Si l'utilisateur souhaite payer en utilisant son compte d'épargne
                if (BuyType == CodeValue.Supply.DepositReason.SavingAccount)
                {
                    //Si l'aragent versé pour payer les achats est supérieurs au montant restant de l'achat
                    if (customerSlice.SliceAmount > currentSale.TotalPriceTTC)
                    {
                        Message = "More Money Than Expected - Sorry, You have put more Money Than Expected for this sale ";
                        status = false;
                        return new JsonResult { Data = new { status = status, Message = Message } };
                    }
                    SavingAccount sa = db.SavingAccounts.SingleOrDefault(sa1 => sa1.CustomerID == currentSale.CustomerID.Value);

                    if (sa == null || sa.ID == 0)
                    {
                        Message = "No Saving Account - Sorry, Customer doesn't have a Saving Account. Please contact an administrator ";
                        status = false;
                        return new JsonResult { Data = new { status = status, Message = Message } };
                    }

                    Customer savingCusto = db.Customers.Find(currentSale.CustomerID.Value);

                    double savingAccountBalance = _savingAccountRepository.GetSavingAccountBalance(savingCusto);

                    //ne faites ps d'achat en espèce si : 1 - pas d'argent en caisse; 2- Facture > Montant en caisse
                    if (savingAccountBalance <= 0 || customerSlice.SliceAmount > savingAccountBalance)
                    {
                        Message = "NO Enough Money in Saving Account - Sorry, Customer doesn't have sufficient Money inside his Saving Account. Please contact an administrator";
                        status = false;
                        return new JsonResult { Data = new { status = status, Message = Message } };
                    }
                    currentSale.PaymentMethodID = sa.ID;
                }
                
                if (currentSale.SaleLines.Count > 0)
                {
                    //empecher que le cqissier saisisse un montant superieur au montant de la vente
                    if (currentSale.TotalPriceTTC < currentSale.CustomerSlice.SliceAmount)
                    {
                        Message = "Error!!! " + Resources.MsgErrTotalPriceMoreThanSlice;
                        status = false;
                        return new JsonResult { Data = new { status = status, Message = Message } };
                    }

                    //We will test if sale is in two steps here or not
                    if (currentSale.TotalPriceTTC != currentSale.CustomerSlice.SliceAmount)
                    {
                        currentSale.IsPaid = false;
                    }
                    else
                    {
                        currentSale.IsPaid = true;
                    }

                    if (currentSale != null && (currentSale.PaymentMethodID <= 0) && (isAnyValidPMTypeSelected))
                    {
                        Message = "Wrong Payment Mode Select!!! " + Resources.MsgErrChoixPayementMethod;
                        status = false;
                        return new JsonResult { Data = new { status = status, Message = Message } };
                    }
                    currentSale.IsValidatedSale = true;

                    if (BuyType == CodeValue.Accounting.DefaultCodeAccountingSection.DIGITAL_PAYMENT)
                    {
                        var isPrimaryKeyViolated = IsTransactionIdentifierNew(currentSale.PaymentReference, currentSale.PaymentMethodID);

                        if (isPrimaryKeyViolated)
                            throw new Exception("The Given Transaction Code (" + currentSale.PaymentReference + " has already been used" );
                    }

                    int SaleID = _saleRepository.SaveChanges(currentSale, heureVente, SessionGlobalPersonID,true,false).SaleID;
                    Session["Receipt_SaleID"] = SaleID;
                    Session["Receipt_CustomerID"] = String.Concat(currentSale.CustomerName," - CIN : ", currentSale.CustomerNumber.ToString());
                    Session["ReceiveAmoung_Tot"] = (currentSale.CustomerSlice != null) ? currentSale.CustomerSlice.SliceAmount : 0;

                    PrintReset(currentSale.BranchID.ToString(), SaleID, customerSlice.SliceAmount);

                    // Mise a jour de la valeur(VIP | ECO) du client qui vient de faire cet achat
                    //_saleRepository.updateCashCustomerValue(SaleID, currentSale.TotalPriceTTC);
                }

                status = true;
                Message = Resources.Success + " - " + Resources.SaleNewSale;
            }
            catch (Exception e)
            {
                status = false;
                //Message = "Error " + e.Message + " " + e.StackTrace + " " + e.InnerException;
                Message = "ERROR - " + e.Message;
            }
            return new JsonResult { Data = new { status = status, Message = Message } };
        }
        public void PrintReset(string Branch, int SaleID, double SliceAmount)
        {
            Session["salelinesnoninsured"] = new List<SaleLine>();
            Session["CurrentProduct"] = new Product();
            Session["MaxValue"] = 0;


            //this.InitTrnNumber(Branch);

            Session["SaleID"] = SaleID;
            Session["SliceAmount"] = SliceAmount;


        }

        //This method add a saleline in the current sale

        public bool DoYes(SpecialLensModel slm, int spray, int boitier)
        {
            bool res = false;
            try
            {
                //double VatRate = CodeValue.Accounting.ParamInitAcct.VATRATE;
                Session["SessionMessage"] = "OK";
                List<SaleLine> salelines = (List<SaleLine>)Session["salelinesnoninsured"];
                List<SaleLine> cols = lensFrameConstruction.Get_COL_From_SLM(slm, new FatSod.DataContext.Concrete.EFDbContext(), spray, boitier);
                foreach (SaleLine saleLine in cols)
                {
                    //saleLine.isCommandGlass = false;
                    //Construction du code du produit en fonction de ce qui a été saisie par l'utilisateur
                    saleLine.Product = LensConstruction.GetProductBySaleLine(saleLine, new FatSod.DataContext.Concrete.EFDbContext());
                    if (saleLine.Product == null)
                    {
                        //ApplyExtraPrices(salelines, reduction, discount, transport, VatRate);
                        Session["salelinesnoninsured"] = salelines;
                        res = true;
                        return res;
                    }
                    if ((saleLine.Product is GenericProduct))
                    {
                        // if (saleLine.Product.CategoryID==2 )
                        //saleLine.isCommandGlass = false;
                        res = this.CheckQty(saleLine.LocalizationID, saleLine.Product.ProductID,saleLine.NumeroSerie, saleLine.LineQuantity, spray, boitier, saleLine);
                        if (!res)
                            return res;
                    }
                    else
                    {
                        //saleLine.isCommandGlass = true;
                    }
                    if (saleLine.LineID > 0)
                    {
                        //Ce produit existe deja dans le panier, alors on enleve les deux lignes liées au SpecialOrderLineCode dans la ligne
                        //1-Coe c'est une modification, on enlève l'existant de la ligne en cours de modification; on va l'ajouter dans la suite(Drop and Create)

                        salelines.RemoveAll(col => col.LineID == saleLine.LineID);
                        //2-Si actuellement on a une seule ligne dans la collection, il y a une possibilité qu'on en avait deux et l'autre a été supprimée; il faut donc le supprimer dans le panier
                        if (cols.Count <= 1) salelines.RemoveAll(col => col.SpecialOrderLineCode == saleLine.SpecialOrderLineCode);
                    }

                    if (salelines != null && salelines.Count() > 0)
                    {
                        SaleLine saleLineExist = salelines.FirstOrDefault(s => s.Product.ProductCode == saleLine.Product.ProductCode && s.SpecialOrderLineCode == saleLine.SpecialOrderLineCode && s.EyeSide == saleLine.EyeSide);
                        if (saleLineExist != null)
                        {
                            salelines.Remove(saleLineExist);
                        }

                        int maxLineID = (salelines != null && salelines.Count() > 0) ? salelines.Select(l => l.LineID).Max() : 0;

                        saleLine.LineID = (maxLineID + 1);

                        salelines.Add(saleLine);
                    }
                    else
                    {
                        salelines = new List<SaleLine>();
                        saleLine.LineID = 1;
                        salelines.Add(saleLine);
                    }
                }

                //double VatRate = CodeValue.Accounting.ParamInitAcct.VATRATE;
                //ApplyExtraPrices(salelines, reduction, discount, transport, VatRate);
                Session["salelinesnoninsured"] = salelines;

                res = true;

                return res;
            }
            catch (Exception e)
            {
                res = false;
                Session["SessionMessage"] = "Error " + e.Message;
                return res;
            }
        }
        public bool CheckQty(int LocalizationID, int? ProductID,string NumeroSerie, double LineQuantity, int spray, int boitier, SaleLine saleLine)
        {
            bool res = false;
            double currentQteEnStock = 0d;
            Session["SessionMessage"] = "OK";
            try
            {
                //if (spray > 0 || boitier > 0)
                //{
                    currentQteEnStock=GetQuantityStock(LocalizationID.ToString(), ProductID.ToString(), NumeroSerie, saleLine.marque);
                //}
                //else
                //{
                //    currentQteEnStock = (double)Session["MaxValue"];
                //}


                //recherche des qtes commandes en attente de validation pr ce produit et cette localization
                double qtyComNonValide = 0d;

                bool isStockControl = (bool)Session["isStockControl"];

                if (isStockControl)
                {
                    double safetyQty = (double)Session["SafetyStock"];
                    if (currentQteEnStock - (qtyComNonValide + LineQuantity) <= 0) //plus de produit en stock
                    {
                        res = this.AlertMsgSock(qtyComNonValide + LineQuantity, safetyQty);
                        if (!res) return res;
                    }
                    if (currentQteEnStock - (qtyComNonValide + LineQuantity) <= safetyQty) //stock de securite atteint
                    {
                        res = this.AlertMsgSock(qtyComNonValide + LineQuantity, safetyQty);
                        if (!res) return res;
                    }
                }

                if (LineQuantity <= 0)
                {
                    res = false;
                    statusOperation = Resources.cmdMontantObligatoire;
                    Session["SessionMessage"] = statusOperation;

                    return res;
                }

                res = true;
                return res;
            }
            catch (Exception e)
            {
                Session["SessionMessage"] = "Error " + e.Message;
                return res;
            }
        }

        public double GetQuantityStock(string Localization, string CurrentProduct,string NumeroSerie, string marque)
        {
            double LineQuantity = 0d;
            
            if ((Localization != null && CurrentProduct != null) && (Localization.Length > 0 && CurrentProduct.Length > 0) )
            {
                
                int idLoc = Convert.ToInt32(Localization);
                int idProd = Convert.ToInt32(CurrentProduct);

                //check if it is product with serial number
                Category productcat = db.Products.Find(idProd).Category;
                ProductLocalization productInStock = new ProductLocalization();
                if (idLoc > 0 && idProd > 0)
                {
                    if (!(productcat.isSerialNumberNull))
                    {
                        productInStock = db.ProductLocalizations.FirstOrDefault(pL => pL.LocalizationID == idLoc && pL.ProductID == idProd);
                    }
                    else
                    {
                        //if ((NumeroSerie != null && NumeroSerie != null))
                        //{
                            productInStock = db.ProductLocalizations.FirstOrDefault(pL => pL.LocalizationID == idLoc &&
                                                                                         pL.ProductID == idProd &&
                                                                                         pL.NumeroSerie == NumeroSerie &&
                                                                                         pL.Marque == marque);
                        //}
                    }
                    
                    Session["CurrentProduct"] = productInStock.ProductLabel;
                    if (productInStock == null || Math.Abs(productInStock.ProductLocalizationStockQuantity) <= 0)
                    {
                        LineQuantity = 0d;
                        Session["MaxValue"] = 0d;
                        Session["SafetyStock"] = 0d;
                    }
                    else
                    {
                        LineQuantity = productInStock.ProductLocalizationStockQuantity;
                        Session["MaxValue"] = productInStock.ProductLocalizationStockQuantity;
                        Session["SafetyStock"] = productInStock.ProductLocalizationSafetyStockQuantity;
                    }
                }
            }
           
            return LineQuantity;

        }
        //this method alert user when the quantity in localization is unavailable


        public bool AlertMsgSock(double QuantityValue, double safetyQty)
        {
            bool res = true;
            double maxQuantity = (double)Session["MaxValue"];
            string productLabel = (string)Session["CurrentProduct"];
            Session["SessionMessage"] = "";
            if (safetyQty > QuantityValue)
            {
                Session["SessionMessage"] = "Attention - En vendant cette quantité le seuil de sécurité sera atteind. Veuillez, faire un réapprovisionnement le plutôt possible";
                res = true;
            }
            if (maxQuantity < QuantityValue)
            {
                Session["SessionMessage"] = "Erreur - " + productLabel + " : " + Resources.EnoughQuantityStock + " " + maxQuantity;
                res = false;
            }
            return res;
        }
    }
}