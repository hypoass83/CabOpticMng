using FatSod.DataContext.Initializer;
using FatSod.Security.Abstracts;
using FatSod.Security.Entities;
using FatSod.Supply.Entities;
using CABOPMANAGEMENT.Controllers;
using CABOPMANAGEMENT.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using SaleE = FatSod.Supply.Entities.Sale;
using FatSod.Supply.Abstracts;
using FatSod.Ressources;
using CABOPMANAGEMENT.Filters;
using FastSod.Utilities.Util;
using FatSod.DataContext.Repositories;

namespace CABOPMANAGEMENT.Areas.CashRegister.Controllers
{
    [Authorize(Order = 1)]
    [TakeBusinessDay(Order = 2)]
    public class OtherSaleController : BaseController
    {
        //Current Controller and current page
        private const string CONTROLLER_NAME = "CashRegister/OtherSale";
        private const string VIEW_NAME = "Index";
        //person repository
        private ISavingAccount _savingAccountRepository;
        private ISale _saleRepository;
        private IBusinessDay _busDayRepo;
        private ITransactNumber _transactNumbeRepository;
        private ILensNumberRangePrice _priceRepository;
        private IDeposit _depositRepository;
        private List<BusinessDay> listBDUser;
        private bool isLens = false;
        private ITillDay _tillDayRepository;

        //Construcitor
        public OtherSaleController(
            ITillDay tillDayRepository,
            ISale saleRepository,
            IBusinessDay busDayRepo,
            ILensNumberRangePrice lnrpRepo,
            ISavingAccount SavingAccountRepo,
            IDeposit depositRepository,
            ITransactNumber transactNumbeRepository
            )
        {
            this._tillDayRepository = tillDayRepository;
            this._saleRepository = saleRepository;
            this._busDayRepo = busDayRepo;
            this._transactNumbeRepository = transactNumbeRepository;
            this._savingAccountRepository = SavingAccountRepo;
            this._priceRepository = lnrpRepo;
            this._depositRepository = depositRepository;
        }
        // GET: Sale/Sale Return all a sales historique add enable to create a new sale
        [OutputCache(Duration = 3600)]
        public ActionResult Index()
        {
            ViewBag.DisplayForm = 1;
            try
            {
                //we ensure that if this user manage cash register. If he manage it, will verify if till is closed else, we ask he to closed it before login off
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


                List<BusinessDay> UserBusDays = (List<BusinessDay>)Session["UserBusDays"];
                DateTime currentDateOp = UserBusDays.FirstOrDefault().BDDateOperation;
                ViewBag.CurrentBranch = UserBusDays.FirstOrDefault().BranchID;
                ViewBag.BusnessDayDate = UserBusDays.FirstOrDefault().BDDateOperation.ToString("yyyy-MM-dd");// businessDay.BDDateOperation;

                
                int deviseID = (Session["DefaultDeviseID"] == null) ? 0 : (int)Session["DefaultDeviseID"];
                if (deviseID <= 0)
                {
                    InjectUserConfigInSession();
                }
                deviseID = (Session["DefaultDeviseID"] == null) ? 0 : (int)Session["DefaultDeviseID"];
                ViewBag.DefaultDeviseID = deviseID;
                ViewBag.DefaultDevise = (deviseID <= 0) ? "" : db.Devises.Find(deviseID).DeviseCode;

                Session["salelines"] = new List<SaleLine>();

                Session["isUpdate"] = false;
                Session["MaxValue"] = 500d;
                Session["SafetyStock"] = 500d;
                Session["EquipPrice"] = 0d;
                Session["valeur"] = 1d;
                
                Session["isApplyToCalculate"] = false;
                Session["SessionMessage"] = "";

                Session["LimitAmount"] = 0d;
                Session["CustomerDebt"] = 0d;

                return View();
            }

            catch (Exception e)
            {
                TempData["Message"] = "Error " + e.Message + e.InnerException;
                ViewBag.DisplayForm = 0;
                return this.View();
            }
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

        public ActionResult GetEquipmentPrice(int? CurrentProduct)
        {
            List<object> _InfoList = new List<object>();
            if ((!CurrentProduct.HasValue || CurrentProduct.Value <= 0))
            {
                return Json(_InfoList, JsonRequestBehavior.AllowGet);
            }
            double priceEquip = 0d;
            double valeur = 1d;

            Product product = db.Products.Find(CurrentProduct.Value);
            bool productIsLens = product is Lens;
            if (!productIsLens)
            {
                
                Session["isApplyToCalculate"] = true;
                priceEquip = product.SellingPrice;
                valeur = 1d;
                
                Session["valeur"] = valeur;
            }

            _InfoList.Add(new
            {
                LineUnitPrice = priceEquip

            });
            return Json(_InfoList, JsonRequestBehavior.AllowGet);
        }

        /*public JsonResult InitTrnNumber(int? BranchID, int CustomerID)
        {
            List<object> _CommandList = new List<object>();

            if (BranchID > 0)
            {
                listBDUser = (List<BusinessDay>)Session["UserBusDays"];
                if (listBDUser == null)
                {
                    listBDUser = _busDayRepo.GetOpenedBusinessDay(CurrentUser);
                }
                BusinessDay businessDay = listBDUser.FirstOrDefault(b => b.BranchID == BranchID.Value);
                double balance = 0d;
                double SavingAmount = 0d;
                //string trnnum = _transactNumbeRepository.displayTransactNumber("SALE", businessDay);

                Customer customer = db.Customers.Find(CustomerID);
                Session["CashCustomer"] = customer.IsCashCustomer;
                Session["LimitAmount"] = customer.LimitAmount;

                Session["CustomerDebt"] = 0d;
               
                SavingAmount = 0;
                balance = SavingAmount;
                Session["CustomerDebt"] = balance;
                //}
                _CommandList.Add(new
                {
                    CustomerDebt = balance,
                    LimitAmount = customer.LimitAmount,
                    IsCashCustomer = (customer.IsCashCustomer == CashCustomer.Cash) ? "Cash" : "NonCash",
                    //SaleReceiptNumber = trnnum,
                    SavingAmount = SavingAmount,
                    Representant = customer.CustomerFullName
                });

            }
            return Json(_CommandList, JsonRequestBehavior.AllowGet);
        }*/

        //chargement des combo box
        public JsonResult populateBuyType()
        {

            List<object> BuyTypeList = new List<object>();
            //cash
            BuyTypeList.Add(new
            {
                ID = CodeValue.Accounting.DefaultCodeAccountingSection.CODECAIS,
                Name = Resources.CASH
            });
            //bank
            BuyTypeList.Add(new
            {
                ID = CodeValue.Accounting.DefaultCodeAccountingSection.CODEBANK,
                Name = Resources.BANK
            });
            //savingAccount
            //BuyTypeList.Add(new
            //{
            //    ID = CodeValue.Supply.DepositReason.SavingAccount,
            //    Name = Resources.SavingAccount
            //});
            //credit
            BuyTypeList.Add(new
            {
                ID = CodeValue.Accounting.DefaultCodeAccountingSection.Credit,
                Name = CodeValue.Accounting.DefaultCodeAccountingSection.Credit
            });
            return Json(BuyTypeList, JsonRequestBehavior.AllowGet);
        }
        //
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
        public bool AlertMsgNotSold(double ProductAmount)
        {
            bool res = false;
            double LimitAmount = (double)Session["LimitAmount"];
            double CustomerDebt = (double)Session["CustomerDebt"];
            Session["SessionMessage"] = "OK";
            if (LimitAmount > 0)
            {
                if (CustomerDebt >= 0) res = true;
                else if ((CustomerDebt * -1 + ProductAmount) >= LimitAmount)
                {
                    Session["SessionMessage"] = "Sorry - " + Resources.MsgLimitAmount;
                    res = false;
                }
                else res = true;
            }
            else res = true;
            return res;
        }

        //[HttpPost]
        public JsonResult AddSaline(SaleLine saleLine, int? OeilDroiteGauche, string reduction, string discount, double transport = 0)
        {
            bool status = false;
            string Message = "";
            try
            {
                bool res = false;
                if ((OeilDroiteGauche == null) || !(saleLine.Product is Lens)) saleLine.OeilDroiteGauche = EyeSide.N;
                if (OeilDroiteGauche == 0) saleLine.OeilDroiteGauche = EyeSide.OD;
                if (OeilDroiteGauche == 1) saleLine.OeilDroiteGauche = EyeSide.OG;
                if (OeilDroiteGauche == 2) saleLine.OeilDroiteGauche = EyeSide.ODG;
                double lineQty = (double)Session["valeur"];

                Product p = db.Products.Find(saleLine.ProductID);

                
                saleLine.LineUnitPrice = (p is Lens) ? (saleLine.LineUnitPrice / 2) : saleLine.LineUnitPrice;

                double currentQteEnStock = (double)Session["MaxValue"];
                List<SaleLine> salelines = (List<SaleLine>)Session["salelines"];
                double ValueOperation = 0d;
                if (salelines.Count == 0) ValueOperation = saleLine.LineAmount;
                else ValueOperation = salelines.Select(sl => sl.LineAmount).Sum() + saleLine.LineAmount;
                ExtraPrice extra = Util.ExtraPrices(ValueOperation, LoadComponent.ConvertToDouble(reduction), LoadComponent.ConvertToDouble(discount), transport, CodeValue.Accounting.ParamInitAcct.VATRATE);
                bool valideOp = AlertMsgNotSold(extra.TotalTTC);
                if (valideOp)
                {
                    //recherche des qtes commandes en attente de validation pr ce produit et cette localization
                    double qtyComNonValide = 0;

                    var lscustOrder = db.CustomerOrders.Join(db.CustomerOrderLines, co => co.CustomerOrderID, col => col.CustomerOrderID,
                    (co, col) => new { co, col })
                    .Where(lsp => !lsp.co.IsDelivered && lsp.col.ProductID == saleLine.ProductID && lsp.col.LocalizationID == saleLine.LocalizationID && !lsp.col.isPost)
                    .Select(s => new
                    {
                        LineQuantity = s.col.LineQuantity,
                    })
                    .ToList();

                    lscustOrder.ForEach(lc =>
                    {
                        qtyComNonValide = qtyComNonValide + lc.LineQuantity;
                    });

                    List<SaleLine> qteSaleline = salelines.Where(c => c.ProductID == saleLine.ProductID && c.LocalizationID == saleLine.LocalizationID).ToList();
                    double qteSale = 0;
                    if (qteSaleline == null)
                    {
                        qteSale = 0;
                    }
                    else
                    {
                        qteSale = qteSaleline.Select(c => c.LineQuantity).Sum();
                    }
                    qtyComNonValide = qtyComNonValide + qteSale;

                    bool isStockControl = (bool)Session["isStockControl"];
                    if (isStockControl)
                    {
                        double safetyQty = (double)Session["SafetyStock"];
                        if (currentQteEnStock - (qtyComNonValide + (saleLine.LineQuantity * lineQty)) <= 0) //plus de produit en stock
                        {
                            res = this.AlertMsgSock(qtyComNonValide + (saleLine.LineQuantity * lineQty), safetyQty);
                            if (!res)
                            {
                                status = false;
                                Message = (string)Session["SessionMessage"];
                                return new JsonResult { Data = new { status = status, Message = Message } };
                            }
                        }
                        if (currentQteEnStock - (qtyComNonValide + (saleLine.LineQuantity * lineQty)) <= safetyQty) //stock de securite atteint
                        {
                            res = this.AlertMsgSock(qtyComNonValide + (saleLine.LineQuantity * lineQty), safetyQty);
                            if (!res)
                            {
                                status = false;
                                Message = (string)Session["SessionMessage"];
                                return new JsonResult { Data = new { status = status, Message = Message } };
                            }
                        }
                    }

                    if ((saleLine.LineQuantity * lineQty) <= 0)
                    {
                        statusOperation = Resources.cmdMontantObligatoire;
                        status = false;
                        Message = statusOperation;
                        return new JsonResult { Data = new { status = status, Message = Message } };
                    }
                    else
                    {
                        status = this.DoYes(saleLine, LoadComponent.ConvertToDouble(reduction), LoadComponent.ConvertToDouble(discount), transport);
                        Message = (string)Session["SessionMessage"];
                    }
                }
            }
            catch (Exception e)
            {
                status = false;
                Message = "Error " + e.Message + " " + e.StackTrace + " " + e.InnerException;
            }
            return new JsonResult { Data = new { status = status, Message = Message } };
        }
        //This method add a saleline in the current sale

        public bool DoYes(SaleLine saleLine, double reduction = 0, double discount = 0, double transport = 0)
        {
            bool res = false;
            try
            {
                double lineQty = (double)Session["valeur"];
                Session["SessionMessage"] = "OK";
                
                List<SaleLine> salelines = (List<SaleLine>)Session["salelines"];
                if (saleLine.LineID > 0)
                {
                    //this product already exist in session
                    salelines.Remove(salelines.FirstOrDefault(cl => cl.LineID == saleLine.LineID));
                }
                if (salelines != null && salelines.Count() > 0)
                {
                    SaleLine saleLineExist = salelines.FirstOrDefault(s => s.ProductID == saleLine.ProductID && s.OeilDroiteGauche == saleLine.OeilDroiteGauche);
                    if (saleLineExist != null)
                    {
                        salelines.Remove(saleLineExist);
                    }
                    SaleLine newArticle = new SaleLine()
                    {
                        LineID = salelines.LastOrDefault() != null ? 1 + salelines.LastOrDefault().LineID : 1,
                        LineUnitPrice = saleLine.LineUnitPrice,
                        LineQuantity = saleLine.LineQuantity,
                        ProductID = saleLine.ProductID,
                        Product = db.Products.Find(saleLine.ProductID),
                        LocalizationID = saleLine.LocalizationID,
                        Localization = db.Localizations.Find(saleLine.LocalizationID),
                        OeilDroiteGauche = saleLine.OeilDroiteGauche
                    };
                    salelines.Add(newArticle);

                }
                else
                {
                    salelines = new List<SaleLine>();
                    SaleLine newArticle = new SaleLine()
                    {
                        LineID = 1,
                        LineUnitPrice = saleLine.LineUnitPrice,
                        LineQuantity = saleLine.LineQuantity,
                        ProductID = saleLine.ProductID,
                        Product = db.Products.Find(saleLine.ProductID),
                        LocalizationID = saleLine.LocalizationID,
                        Localization = db.Localizations.Find(saleLine.LocalizationID),
                        OeilDroiteGauche = saleLine.OeilDroiteGauche
                    };
                    salelines.Add(newArticle);
                }

                Session["salelines"] = salelines;

                this.refreshCmdLine();

                res = true;
            }
            catch (Exception e)
            {
                res = false;
                Session["SessionMessage"] = "Error " + e.Message;

            }
            return res;
        }



        public void refreshCmdLine()
        {
            Session["SessionMessage"] = "OK";
            Session["EquipPrice"] = 0d;
            Session["valeur"] = 1d;
            
        }
        //

        public JsonResult ApplyExtraPrices(double reduction, double discount, double transport)
        {
            List<object> _CommandList = new List<object>();
            List<SaleLine> SaleLines = (List<SaleLine>)Session["salelines"];

            double valueOperation = SaleLines.Select(l => l.LineAmount).Sum();
            //we add extra price
            double new_HT_price = valueOperation;
            double SliceAmount = 0d;
            double RemaingAmount = 0d;

            double NetCom = valueOperation;
            double vatRate = CodeValue.Accounting.ParamInitAcct.VATRATE;
            ExtraPrice extra = Util.ExtraPrices(valueOperation, reduction, discount, transport, vatRate);

            //CashCustomer iscashCusto = (CashCustomer)Session["CashCustomer"];
            //if (iscashCusto == CashCustomer.Cash)
            //{
                SliceAmount = extra.TotalTTC;
                RemaingAmount = 0d;
            //}
            //else
            //{
            //    SliceAmount = 0d;
            //    RemaingAmount = extra.TotalTTC;
            //}

            _CommandList.Add(new
            {
                InitialHT = valueOperation,
                DiscountAmount = extra.DiscountAmount,
                NetCom = extra.NetCom,
                ReductionAmount = extra.ReductionAmount,
                TotalPriceHT = extra.NetFinan,
                TVAAmount = extra.TVAAmount,
                TotalPriceTTC = extra.TotalTTC,
                InitialTTC = extra.TotalTTC,
                SliceAmount = SliceAmount,
                RemaingAmount = RemaingAmount,
            });

            return Json(_CommandList, JsonRequestBehavior.AllowGet);
        }


        //This method remove saleline
        //[HttpPost]
        public JsonResult RemoveSaleLine(int ID)
        {
            List<SaleLine> salelines = (List<SaleLine>)Session["salelines"];
            salelines.Remove(salelines.FirstOrDefault(s => s.LineID == ID));

            var model = new
            {
                data = from c in salelines
                       select new
                       {
                           LineID = c.LineID,
                           LineAmount = c.LineAmount,
                           LineQuantity = c.LineQuantity,
                           ProductLabel = c.ProductLabel,
                           LineUnitPrice = c.LineUnitPrice
                       }
            };
            return Json(model, JsonRequestBehavior.AllowGet);
        }


        private void SimpleReset()
        {
            List<SaleLine> salelines = new List<SaleLine>();
            Session["salelines"] = salelines;
        }



        public JsonResult UpdateLine(int ID)
        {


            List<object> _CommandList = new List<object>();

            Session["isUpdate"] = true;
            Session["LensNumberFullCode"] = "";

            List<SaleLine> salelines = (List<SaleLine>)Session["salelines"];
            Line lineToUpdate = salelines.FirstOrDefault(l => l.LineID == ID);
            int ProductID = 0;

            if (lineToUpdate.Product is Lens)
            {
                Lens number = (from l in db.Lenses
                               where l.ProductID == lineToUpdate.ProductID
                               select l).FirstOrDefault();
                Session["LensNumberFullCode"] = number.LensNumberFullCode;
                ProductID = number.LensNumberID;
            }

            double qtystock = GetMaxQtyInStock(lineToUpdate.ProductID, lineToUpdate.LocalizationID);
            Session["MaxValue"] = qtystock;
            Session["CurrentProduct"] = lineToUpdate.ProductLabel;

            _CommandList.Add(new
            {
                LineUnitPrice = lineToUpdate.LineUnitPrice,
                LineQuantity = lineToUpdate.LineQuantity,
                StockQuantity = qtystock,
                Product = lineToUpdate.ProductID,
                ProductNumberID = (lineToUpdate.Product is Lens) ? ProductID : lineToUpdate.ProductID,
                ProductCategoryID = lineToUpdate.Product.CategoryID,
                Localization = lineToUpdate.LocalizationID,
                LineID = lineToUpdate.LineID,
                LensCategory = (lineToUpdate.Product is Lens) ? 0 : 1,
                ProductCode = lineToUpdate.Product.ProductCode,
                ProductNumber = Session["LensNumberFullCode"]

            });

            return Json(_CommandList, JsonRequestBehavior.AllowGet);

        }
        public double GetMaxQtyInStock(int productID, int localizationID)
        {
            double res = 0;

            res = db.ProductLocalizations.SingleOrDefault(pl => pl.ProductID == productID && pl.LocalizationID == localizationID).ProductLocalizationStockQuantity;

            return res;
        }
        //Return salelines of current sale

        public JsonResult SaleLines()
        {
            List<SaleLine> salelines = (List<SaleLine>)Session["salelines"];
            var model = new
            {
                data = from c in salelines
                       select new
                       {
                           LineID = c.LineID,
                           LineAmount = c.LineAmount,
                           LineQuantity = c.LineQuantity,
                           ProductLabel = c.ProductLabel,
                           LineUnitPrice = c.LineUnitPrice
                       }
            };

            return Json(model, JsonRequestBehavior.AllowGet);
        }
        //This method confirm a sale
        //[HttpPost]
        public JsonResult AddSale(SaleE currentSale, CustomerSlice customerSlice, int? PaymentDelay, string BuyType, string heureVente, string RateReduction, double TVA = 0, int SaleDeliver = 0)
        {
            bool status = false;
            string Message = "";
            try
            {
                Session["Receipt_SaleID"] = null;
                Session["Receipt_CustomerID"] = null;
                Session["ReceiveAmoung_Tot"] = null;

                if (BuyType == "" || BuyType == null)
                {
                    Message = "Wrong Payment Mode Select!!!" + Resources.MsgErrChoixPayementMethod;
                    status = false;
                    return new JsonResult { Data = new { status = status, Message = Message } };
                }

                if (SaleDeliver == 0) currentSale.SaleDeliver = false;
                if (SaleDeliver == 1) currentSale.SaleDeliver = true;

                currentSale.RateReduction = LoadComponent.ConvertToDouble(RateReduction);
                currentSale.VatRate = CodeValue.Accounting.ParamInitAcct.VATRATE;
                currentSale.CustomerSlice = customerSlice;
                customerSlice.SliceDate = currentSale.SaleDate;
                currentSale.PaymentDelay = (PaymentDelay == null) ? 0 : PaymentDelay.Value;
                currentSale.IsSpecialOrder = false;

                if (currentSale != null && currentSale.CustomerSlice.SliceAmount > 0 && BuyType == CodeValue.Accounting.DefaultCodeAccountingSection.Credit)
                {
                    Message = "Wrong Payment Mode Select!!!" + Resources.MsgWrongChoixPayementMethod;
                    status = false;
                    return new JsonResult { Data = new { status = status, Message = Message } };
                }

                if (currentSale != null && currentSale.CustomerSlice.SliceAmount == 0 && (BuyType == CodeValue.Accounting.DefaultCodeAccountingSection.CODEBANK || BuyType == CodeValue.Accounting.DefaultCodeAccountingSection.CODECAIS))
                {
                    Message = "Wrong Payment Mode Select!!! " + Resources.MsgWrongChoixPayementMethod;
                    status = false;
                    return new JsonResult { Data = new { status = status, Message = Message } };
                }

                currentSale.SaleLines = (List<SaleLine>)Session["salelines"];

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
                        Message = "More Money Than Expected - Sorry, You have put more Money Than Expected for this sale";
                        status = false;
                        return new JsonResult { Data = new { status = status, Message = Message } };
                    }

                    SavingAccount sa = db.SavingAccounts.SingleOrDefault(sa1 => sa1.CustomerID == currentSale.CustomerID);

                    if (sa == null || sa.ID == 0)
                    {

                        Message = "No Saving Account - Sorry, Customer doesn't have a Saving Account. Please contact an administrator";
                        status = false;
                        return new JsonResult { Data = new { status = status, Message = Message } };
                    }

                    Customer savingCusto = db.Customers.Find(currentSale.CustomerID);

                    double savingAccountBalance = 0;

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
                    //We will test if sale is in two steps here or not
                    if (currentSale.TotalPriceTTC != currentSale.CustomerSlice.SliceAmount)
                    {
                        currentSale.IsPaid = false;
                    }
                    else
                    {
                        currentSale.IsPaid = true;
                    }
                    if (currentSale != null && (currentSale.PaymentMethodID <= 0) && (BuyType == CodeValue.Accounting.DefaultCodeAccountingSection.CODEBANK || BuyType == CodeValue.Accounting.DefaultCodeAccountingSection.CODECAIS))
                    {
                        Message = "Wrong Payment Mode Select!!! " + Resources.MsgErrChoixPayementMethod;
                        status = false;
                        return new JsonResult { Data = new { status = status, Message = Message } };
                    }
                    currentSale.IsOtherSale = true;
                    int SaleID = _saleRepository.SaveChanges(currentSale, heureVente, SessionGlobalPersonID,false,true).SaleID;
                    PrintReset(SaleID, customerSlice.SliceAmount, TVA,currentSale.CustomerName);
                }

                status = true;
                Message = Resources.Success + " - " + Resources.SaleNewSale;
            }
            catch (Exception e)
            {
                status = false;
                Message = "Error " + e.Message + " " + e.StackTrace + " " + e.InnerException;
            }
            return new JsonResult { Data = new { status = status, Message = Message } };
        }

        public void PrintReset(int SaleID, double SliceAmount, double TVA,string CustomerID)
        {
            Session["salelines"] = new List<SaleLine>();

            Session["CurrentProduct"] = new Product();
            Session["MaxValue"] = 0;

            Session["SaleID"] = SaleID;
            Session["SliceAmount"] = SliceAmount;

            Session["Receipt_SaleID"]= SaleID;
            Session["Receipt_CustomerID"]= CustomerID;
            Session["ReceiveAmoung_Tot"]= SliceAmount;

        }

        public JsonResult InitDate(int? BranchID)
        {
            List<object> _InfoList = new List<object>();
            if (BranchID != null && BranchID.HasValue && BranchID.Value > 0)
            {
                listBDUser = (List<BusinessDay>)Session["UserBusDays"];
                if (listBDUser == null)
                {
                    listBDUser = _busDayRepo.GetOpenedBusinessDay(CurrentUser);
                }
                DateTime businessD = listBDUser.FirstOrDefault(b => b.BranchID == BranchID).BDDateOperation;
                _InfoList.Add(new
                {
                    SaleDate = businessD.ToString("yyyy-MM-dd")
                });
            }

            return Json(_InfoList, JsonRequestBehavior.AllowGet);
        }

        //*****************---------------------------************************************//
        public JsonResult LoadCustomers(string filter)
        {

            List<object> customersList = new List<object>();
            foreach (Customer customer in db.People.OfType<Customer>().Where(c => c.Name.StartsWith(filter.ToLower())).Take(100).ToArray().OrderBy(c => c.Name))
            {
                string itemLabel = "";

                //si le client est une personne physique, on renvoie son nom et son prénom, si c'est une entreprise, on renvoie son nom
                itemLabel = customer.Name +
                    (" " + customer.Description);

                customersList.Add(new
                {
                    Name = itemLabel,
                    ID = customer.GlobalPersonID
                });
            }

            return Json(customersList, JsonRequestBehavior.AllowGet);
        }

        ////////////////////
        public JsonResult GetAllStockedLocations()
        {
            List<object> localizationsList = new List<object>();
            foreach (Localization Loc in db.Localizations.ToList())
            {
                localizationsList.Add(new
                {
                    LocalizationCode = Loc.LocalizationCode,
                    LocalizationID = Loc.LocalizationID
                });
            }

            return Json(localizationsList, JsonRequestBehavior.AllowGet);

        }
        //
        

        //
        public JsonResult GetProductCategory()
        {
            List<object> categoryList = new List<object>();
            List<Category> categories = LoadComponent.GetAllGenericCategories();
            foreach (Category cat in categories)
            {
                categoryList.Add(new
                {
                    CategoryID = cat.CategoryID,
                    CategoryCode = cat.CategoryLabel
                });
            }

            return Json(categoryList, JsonRequestBehavior.AllowGet);
        }
        //
        public JsonResult DisableNumero(int ProductCategoryID)
        {
            List<object> _InfoList = new List<object>();
            Category catprod = db.Categories.Find(ProductCategoryID);
            _InfoList.Add(new
            {
                LensCategory = (catprod is LensCategory) ? 0 : 1
            });


            return Json(_InfoList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAllProducts(int DepartureLocalizationID, int? ProductCategoryID, int? ProductNumberID)
        {

            List<object> _InfoList = new List<object>();

            List<Product> list = ModelProductLocalCat(DepartureLocalizationID, ProductCategoryID, ProductNumberID);

            foreach (Product s in list.OrderBy(c => c.ProductCode))
            {

                _InfoList.Add(new
                {
                    ProductID = s.ProductID,
                    ProductCode = s.ProductCode,

                });
            }
            return Json(_InfoList, JsonRequestBehavior.AllowGet);
        }

        //recup des produits

        public List<Product> ModelProductLocalCat(int DepartureLocalizationID, int? ProductCategoryID, int? ProductNumberID)
        {
            List<Product> model = new List<Product>();


            //On a un produit générique
            if (DepartureLocalizationID > 0 && (ProductCategoryID == 0 || ProductCategoryID == null) && (ProductNumberID == 0 || ProductNumberID == null)) //chargement des produits en fct du magasin slt
            {
                return model;
            }
            else //On a un produit de type verre
            {
                // verifion si c'est un produit de type verre

                Category catprod = db.Categories.Find(ProductCategoryID.Value);

                isLens = (catprod is LensCategory);

                if (isLens)
                //if (lenprod != null) //verre
                {
                    if ((ProductNumberID != null || ProductNumberID > 0) && ProductCategoryID > 0) //desc et numero
                    {

                        //produit de verre

                        var lstLensProduct = db.Lenses.Join(db.Products, ls => ls.ProductID, p => p.ProductID,
                            (ls, p) => new { ls, p }).
                            Join(db.ProductLocalizations, pr => pr.p.ProductID, pl => pl.ProductID, (pr, pl) => new { pr, pl })
                            .Where(lsp => lsp.pl.LocalizationID == DepartureLocalizationID
                            && lsp.pr.ls.LensNumberID == ProductNumberID && lsp.pr.p.CategoryID == ProductCategoryID)
                            .Select(s => new
                            {
                                ProductID = s.pr.p.ProductID,
                                ProductCode = s.pr.p.ProductCode,
                                ProductLabel = s.pr.p.ProductLabel,
                                ProductQuantity = s.pl.ProductLocalizationStockQuantity,
                                ProductLocalizationStockSellingPrice = s.pl.ProductLocalizationStockSellingPrice
                            }).ToList();

                        foreach (var pt in lstLensProduct.OrderBy(c => c.ProductCode))
                        {
                            model.Add(
                            new Product
                            {
                                ProductID = pt.ProductID,
                                ProductCode = pt.ProductCode,
                                ProductLabel = pt.ProductLabel,
                                ProductQuantity = pt.ProductQuantity,
                                SellingPrice = pt.ProductLocalizationStockSellingPrice
                            }
                           );

                        }

                    }
                    else return model;
                }
                else
                {

                    ////produit generic

                    var lstLensProduct = db.Products.Join(db.ProductLocalizations, p => p.ProductID, pl => pl.ProductID,
                        (p, pl) => new { p, pl })
                        .Where(lsp => lsp.pl.LocalizationID == DepartureLocalizationID && lsp.p.CategoryID == ProductCategoryID && !(lsp.p.Category is LensCategory))
                        .Select(s => new
                        {
                            ProductID = s.p.ProductID,
                            ProductCode = s.p.ProductCode,
                            ProductLabel = s.p.ProductLabel,
                            ProductQuantity = s.pl.ProductLocalizationStockQuantity,
                            ProductLocalizationStockSellingPrice = s.pl.ProductLocalizationStockSellingPrice
                        }).ToList();

                    foreach (var pt in lstLensProduct.OrderBy(c => c.ProductCode))
                    {
                        model.Add(
                            new Product
                            {
                                ProductID = pt.ProductID,
                                ProductCode = pt.ProductCode,
                                ProductLabel = pt.ProductLabel,
                                ProductQuantity = pt.ProductQuantity,
                                SellingPrice = pt.ProductLocalizationStockSellingPrice
                            }
                           );
                    }

                }
            }

            return model;
        }

        public ActionResult OnProductSelected(int? Localization, int? CurrentProduct)
        {
            List<object> _InfoList = new List<object>();
            double StockQuantity = 0d;
            double LineUnitPrice = 0d;
            Session["EquipPrice"] = 0d;

            if ((!Localization.HasValue || Localization.Value <= 0) || (!CurrentProduct.HasValue || CurrentProduct.Value <= 0))
            {
                return Json(_InfoList, JsonRequestBehavior.AllowGet);
            }

            Product product = db.Products.Find(CurrentProduct.Value);
            bool productIsLens = product is Lens;

            if (productIsLens)
            {
                StockQuantity = (double)Session["MaxValue"];
                //Récupération du prix du verre à partir de son intervalle de numéro
                LensNumberRangePrice price = _priceRepository.GetPrice(product.ProductID);
                LineUnitPrice = (price != null && price.LensNumberRangePriceID > 0) ? price.Price : 0;
            }
            else
            {
                var prodLoc = db.ProductLocalizations.Where(pl => pl.ProductID == CurrentProduct.Value && pl.LocalizationID == Localization.Value)
                .Select(p => new
                {
                    ProductID = p.ProductID,
                    ProductLocalizationStockQuantity = p.ProductLocalizationStockQuantity,
                    SellingPrice = p.Product.SellingPrice,
                    ProductLabel = p.Product.ProductLabel,
                    ProductLocalizationSafetyStockQuantity = p.ProductLocalizationSafetyStockQuantity
                }).SingleOrDefault();

                Session["MaxValue"] = prodLoc.ProductLocalizationStockQuantity;
                Session["CurrentProduct"] = prodLoc.ProductLabel;
                Session["CurrentProductID"] = prodLoc.ProductID;
                Session["SafetyStock"] = prodLoc.ProductLocalizationSafetyStockQuantity;

                StockQuantity = prodLoc.ProductLocalizationStockQuantity;

                bool isUpdate = (bool)Session["isUpdate"];

                if (!isUpdate)
                {
                    Session["EquipPrice"] = prodLoc.SellingPrice;
                    LineUnitPrice = prodLoc.SellingPrice;
                    Session["isUpdate"] = false;
                }
            }


            _InfoList.Add(new
            {
                StockQuantity = StockQuantity,
                LineUnitPrice = LineUnitPrice

            });
            return Json(_InfoList, JsonRequestBehavior.AllowGet);
        }

        
        


    }
}