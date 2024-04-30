using FatSod.DataContext.Initializer;
using FatSod.Security.Abstracts;
using FatSod.Security.Entities;
using FatSod.Supply.Entities;
using FatSodDental.UI.Controllers;
using FatSodDental.UI.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using SaleE = FatSod.Supply.Entities.Sale;
using FatSod.Supply.Abstracts;
using FatSod.Ressources;
using FatSodDental.UI.Filters;
using FastSod.Utilities.Util;
using System.Collections;
using System.Web.UI;
using FatSod.DataContext.Concrete;
using ExtPartialViewResult = Ext.Net.MVC.PartialViewResult;
using CrystalDecisions.Shared;
using CrystalDecisions.CrystalReports.Engine;

namespace FatSodDental.UI.Areas.CashRegister.Controllers
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

            //we ensure that if this user manage cash register. If he manage it, will verify if till is closed else, we ask he to closed it before login off
            UserTill userTill = (from td in db.UserTills
                                 where td.UserID == SessionGlobalPersonID
                                 select td).SingleOrDefault();
            if (userTill == null || userTill.TillID <= 0)
            {
                X.Msg.Alert("Access Denied", "You can't do sale operation. If you think no, try again or<br/>contact our administrator for this purpose<code/>.").Show();
                return this.Direct();
            }
            List<BusinessDay> UserBusDays = (List<BusinessDay>)Session["UserBusDays"];
            DateTime currentDateOp = UserBusDays.FirstOrDefault().BDDateOperation;
            ViewBag.CurrentBranch = UserBusDays.FirstOrDefault().BranchID;
            ViewBag.BusnessDayDate = UserBusDays.FirstOrDefault().BDDateOperation;// businessDay.BDDateOperation;
            this.InitTrnNumber(UserBusDays.FirstOrDefault().BranchID.ToString());
            TillDayStatus tState = _tillDayRepository.TillDayStatus(userTill.Till);
            if (tState == null)
            {
                X.Msg.Alert("Error", "Bad Configuration of Cash Register!!! Please call Your database Administrator").Show();
                return this.Direct();
            }
            if (!tState.IsOpen)
            {
                X.Msg.Alert("Error", "This Cash Register is Still Close!!! Please Open It Before Proceed").Show();
                return this.Direct();
            }

            TillDay currentTillDay = (from t in db.TillDays
                                      where
                                          t.TillID == userTill.TillID && t.TillDayDate == tState.TillDayLastOpenDate.Date && t.IsOpen // t.TillDayDate.Day == currentDateOp.Day && t.TillDayDate.Month == currentDateOp.Month && t.TillDayDate.Year == currentDateOp.Year && t.IsOpen
                                      select t).FirstOrDefault();
            if (currentTillDay == null)
            {
                X.Msg.Alert("Warnnig", "Cash register is closed. You must open it before do any sale<br/>Go at Cash Register module=>Open cash register<code/>").Show();
                return this.Direct();
            }
            //}
            ViewBag.CurrentTill = currentTillDay.TillID;


            Session["salelines"] = new List<SaleLine>();
            //this.GetCmp<Panel>("Body").LoadContent(new ComponentLoader
            //{
            //    Url = Url.Action("Index"),
            //    DisableCaching = false,
            //    Mode = LoadMode.Frame
            //});
            Session["isUpdate"] = false;
            Session["MaxValue"] = 500;
            Session["SafetyStock"] = 500;
            return View();
        }
        
        public ActionResult InitTrnNumber(string BranchID)
        {
            listBDUser = (List<BusinessDay>)Session["UserBusDays"];
            if (listBDUser == null)
            {
                listBDUser = _busDayRepo.GetOpenedBusinessDay(CurrentUser);
            }
            BusinessDay businessDay = listBDUser.FirstOrDefault(b => b.BranchID == Convert.ToInt32(BranchID));

            return this.Direct();
        }
        //this method alert user when the quantity in localization is unavailable
        public bool AlertMsgSock(double QuantityValue, double safetyQty)
        {
            bool res = true;
            double maxQuantity = (double)Session["MaxValue"];
            string productLabel = (string)Session["CurrentProduct"];

            if (safetyQty > QuantityValue)
            {
                X.Msg.Alert("Attention", "En vendant cette quantité le seuil de sécurité sera atteind. Veuillez, faire un réapprovisionnement le plutôt possible").Show();
                res = true;
            }
            if (maxQuantity < QuantityValue)
            {
                X.Msg.Alert("Erreur", productLabel + " : " + Resources.EnoughQuantityStock + " " + maxQuantity).Show();
                res = false;
            }
            return res;
        }
        
        [DirectMethod]
        //[HttpPost]
        public ActionResult AddSaline(SaleLine saleLine, double reduction = 0, double discount = 0, double transport = 0)
        {
            try
            {
                bool res = false;

                double currentQteEnStock = (double)Session["MaxValue"];
                List<SaleLine> salelines = (List<SaleLine>)Session["salelines"];
                double ValueOperation = 0d;
                if (salelines.Count == 0) ValueOperation = saleLine.LineAmount;
                else ValueOperation = salelines.Select(sl => sl.LineAmount).Sum() + saleLine.LineAmount;
                ExtraPrice extra = Util.ExtraPrices(ValueOperation, reduction, discount, transport, CodeValue.Accounting.ParamInitAcct.VATRATE);
                
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
                    if (currentQteEnStock - (qtyComNonValide + saleLine.LineQuantity) <= 0) //plus de produit en stock
                    {
                        res = this.AlertMsgSock(qtyComNonValide + saleLine.LineQuantity, safetyQty);
                        if (!res) return this.Direct();
                    }
                    if (currentQteEnStock - (qtyComNonValide + saleLine.LineQuantity) <= safetyQty) //stock de securite atteint
                    {
                        res = this.AlertMsgSock(qtyComNonValide + saleLine.LineQuantity, safetyQty);
                        if (!res) return this.Direct();
                    }
                }

                //if (currentQteEnStock - (qtyComNonValide + customerOderLine.LineQuantity) < 0)
                if (saleLine.LineQuantity <= 0)
                {
                    //statusOperation = Resources.cmdAttenteValidation; ;
                    statusOperation = Resources.cmdMontantObligatoire;
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.WARNING,
                        Title = "Command Order",
                        Message = statusOperation
                    });
                }
                else
                {
                    this.DoYes(saleLine, reduction, discount, transport);
                }
                

                return this.Direct();
            }
            catch (Exception e)
            {
                X.Msg.Alert("Error ", e.Message).Show();
                return this.Direct();
            }
        }
        //This method add a saleline in the current sale
        [DirectMethod]
        public ActionResult DoYes(SaleLine saleLine, double reduction = 0, double discount = 0, double transport = 0)
        {
            try
            {
                List<SaleLine> salelines = (List<SaleLine>)Session["salelines"];
                saleLine.OeilDroiteGauche = EyeSide.N;
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
                double VatRate = CodeValue.Accounting.ParamInitAcct.VATRATE;
                ApplyExtraPrices(salelines, reduction, discount, transport, VatRate);
                Session["salelines"] = salelines;

                this.refreshCmdLine();

                return this.Direct();
            }
            catch (Exception e)
            {
                X.Msg.Notify(new NotificationConfig
                {
                    Icon = Icon.Error,
                    Title = e.Source,
                    Html = "<span style =\"color:red;\">" + e.Message + "<span> "
                }).Show();
                return this.Direct();
            }
        }

        public void refreshCmdLine()
        {
            this.GetCmp<GridPanel>("SalesLines").Disabled = false;
            this.GetCmp<Store>("SaleLineProperties").Reload();

            this.GetCmp<NumberField>("LineQuantity").Clear();
            this.GetCmp<NumberField>("GridState").Value = 1;

        }
        
        [HttpPost]
        public ActionResult OnProductSelected(int? Localization, int? CurrentProduct)
        {
            try
            {

                if ((!Localization.HasValue || Localization.Value <= 0) || (!CurrentProduct.HasValue || CurrentProduct.Value <= 0)) { return this.Direct(); }

                Product product = db.Products.Find(CurrentProduct.Value);
                bool productIsLens = product is Lens;

                if (productIsLens)
                {
                    this.GetCmp<NumberField>("StockQuantity").Value = (double)Session["MaxValue"];
                    //Récupération du prix du verre à partir de son intervalle de numéro
                    LensNumberRangePrice price = _priceRepository.GetPrice(product.ProductID);
                    this.GetCmp<NumberField>("LineUnitPrice").Value = (price != null && price.LensNumberRangePriceID > 0) ? price.Price : 0;
                }
                else
                {
                    var prodLoc = db.ProductLocalizations.Where(pl => pl.ProductID == CurrentProduct.Value && pl.LocalizationID == Localization.Value)
                    .Select(p => new
                    {
                        ProductLocalizationStockQuantity = p.ProductLocalizationStockQuantity,
                        SellingPrice = p.Product.SellingPrice,
                        ProductLabel = p.Product.ProductLabel,
                        ProductLocalizationSafetyStockQuantity = p.ProductLocalizationSafetyStockQuantity
                    }).SingleOrDefault();

                    Session["MaxValue"] = prodLoc.ProductLocalizationStockQuantity;
                    Session["CurrentProduct"] = prodLoc.ProductLabel;
                    Session["SafetyStock"] = prodLoc.ProductLocalizationSafetyStockQuantity;

                    this.GetCmp<NumberField>("StockQuantity").Value = prodLoc.ProductLocalizationStockQuantity;

                    bool isUpdate = (bool)Session["isUpdate"];

                    if (!isUpdate)
                    {
                        this.GetCmp<NumberField>("LineUnitPrice").Value = prodLoc.SellingPrice;
                        Session["isUpdate"] = false;
                    }
                }

                return this.Direct();
            }
            catch (Exception e)
            {
                statusOperation = Resources.er_alert_danger + e.Message + "Source = " + e.Source + "Méthode = OnProductSelected(" + Localization.Value + " " + CurrentProduct.Value + ") " + e.TargetSite + " InnerException = " + e.InnerException;
                X.Msg.Alert(Resources.Productlabel, statusOperation).Show();
                return this.Direct();
            }
        }


        public void ApplyExtraPrices(List<SaleLine> SaleLines, double reduction, double discount, double transport, double vatRate)
        {

            double valueOperation = SaleLines.Select(l => l.LineAmount).Sum();
            //we add extra price
            double new_HT_price = valueOperation;
            double remise = 0;
            double escompte = 0;
            double NetCom = valueOperation;
            //double vatRate = CodeValue.Accounting.ParamInitAcct.VATRATE;

            ExtraPrice extra = Util.ExtraPrices(valueOperation, reduction, discount, transport, vatRate);

            this.GetCmp<NumberField>("InitialHT").Value = valueOperation;
            this.GetCmp<NumberField>("DiscountAmount").Value = extra.DiscountAmount;
            this.GetCmp<NumberField>("NetCom").Value = extra.NetCom;
            this.GetCmp<NumberField>("ReductionAmount").Value = extra.ReductionAmount;
            this.GetCmp<NumberField>("TotalPriceHT").Value = extra.NetFinan;
            this.GetCmp<NumberField>("TVAAmount").Value = extra.TVAAmount;
            this.GetCmp<NumberField>("TotalPriceTTC").Value = extra.TotalTTC;
            this.GetCmp<NumberField>("InitialTTC").Value = extra.TotalTTC;
            //if (valueOperation > 0)
            this.GetCmp<FieldSet>("OperationAmount").Disabled = false;

            this.GetCmp<NumberField>("SliceAmount").Value = 0;
            this.GetCmp<NumberField>("SliceAmount").ReadOnly = false;
            this.GetCmp<NumberField>("RemaingAmount").Value = extra.TotalTTC;
        
        }
        //This method remove saleline
        [HttpPost]
        public ActionResult RemoveSaleLine(int ID, double reduction = 0, double discount = 0, double transport = 0)
        {
            List<SaleLine> salelines = (List<SaleLine>)Session["salelines"];
            salelines.Remove(salelines.FirstOrDefault(s => s.LineID == ID));
            double valueOperation = salelines.Select(l => l.LineAmount).Sum();
            this.GetCmp<NumberField>("InitialHT").Value = valueOperation;

            //we take a extra price
            ExtraPrice extra = Util.ExtraPrices(valueOperation, reduction, discount, transport, CodeValue.Accounting.ParamInitAcct.VATRATE);

            this.GetCmp<NumberField>("DiscountAmount").Value = extra.DiscountAmount;
            this.GetCmp<NumberField>("ReductionAmount").Value = extra.ReductionAmount;
            this.GetCmp<NumberField>("TotalPriceHT").Value = extra.TotalHT;
            //double tva_amount = Math.Round(new_HT_price * CodeValue.Accounting.ParamInitAcct.VATRATE);
            this.GetCmp<NumberField>("TVAAmount").Value = extra.TVAAmount;
            this.GetCmp<NumberField>("TotalPriceTTC").Value = extra.TotalTTC;
            this.GetCmp<NumberField>("InitialTTC").Value = extra.TotalHT + extra.TVAAmount;//new_HT_price + tva_amount;
        
            this.GetCmp<Store>("SaleLineProperties").Reload();
            return this.Direct();
        }
        [HttpPost]
        public ActionResult Reset()
        {

            this.SimpleReset();
            this.SimpleReset2();
            return this.Direct();
        }

        [HttpPost]
        public ActionResult Reset2()
        {
            SimpleReset2();
            return this.Direct();
        }

        private void SimpleReset()
        {
            List<SaleLine> salelines = new List<SaleLine>();
            Session["salelines"] = salelines;
            this.GetCmp<FormPanel>("GlobalSaleForm").Reset(true);

        }

        private void SimpleReset2()
        {
            this.GetCmp<FormPanel>("FormAddSaleLine").Reset(true);
            this.GetCmp<Store>("SaleLineProperties").Reload();

        }
        [HttpPost]
        public ActionResult UpdateLine(int ID)
        {


            try
            {
                //SimpleReset2();

                Session["isUpdate"] = true;


                List<SaleLine> salelines = (List<SaleLine>)Session["salelines"];
                Line lineToUpdate = salelines.FirstOrDefault(l => l.LineID == ID);

                this.GetCmp<TextField>("LineID").SetValue(lineToUpdate.LineID);
                this.GetCmp<ComboBox>("Localization").SetValue(lineToUpdate.LocalizationID);

                this.GetCmp<ComboBox>("ProductID").GetStore().Reload();
                this.GetCmp<ComboBox>("ProductID").SetValue(lineToUpdate.ProductID);

                double qtystock = GetMaxQtyInStock(lineToUpdate.ProductID, lineToUpdate.LocalizationID);
                this.GetCmp<NumberField>("StockQuantity").Value = qtystock;
                Session["MaxValue"] = qtystock;
                Session["CurrentProduct"] = lineToUpdate.ProductLabel;
                this.GetCmp<NumberField>("LineQuantity").SetValue(lineToUpdate.LineQuantity);

                this.GetCmp<NumberField>("LineUnitPrice").SetValue(lineToUpdate.LineUnitPrice);

                return this.Direct();
            }
            catch (Exception e) { X.Msg.Alert("Error ", e.Message).Show(); return this.Direct(); }
        }
        public double GetMaxQtyInStock(int productID, int localizationID)
        {
            double res = 0;

            res = db.ProductLocalizations.SingleOrDefault(pl => pl.ProductID == productID && pl.LocalizationID == localizationID).ProductLocalizationStockQuantity;

            return res;
        }
        //Return salelines of current sale
        [HttpPost]
        public StoreResult SaleLines()
        {
            List<SaleLine> salelines = (List<SaleLine>)Session["salelines"];
            List<object> model = new List<object>();
            salelines.ForEach(c =>
            {
                model.Add(
                        new
                        {
                            LineID = c.LineID,
                            LineAmount = c.LineAmount,
                            LineQuantity = c.LineQuantity,
                            ProductLabel = c.ProductLabel,
                            LineUnitPrice = c.LineUnitPrice
                        }
                    );
            });
            return this.Store(model);
        }
        //This method confirm a sale
        [HttpPost]
        public ActionResult AddSale(SaleE currentSale, CustomerSlice customerSlice, int? PaymentDelay, string BuyType, string heureVente, double TVA = 0)
        {
            
            if (BuyType == "" || BuyType == null)
            {
                X.Msg.Alert("Wrong Payment Mode Select!!!",
                        Resources.MsgErrChoixPayementMethod).Show();
                return this.Direct();
            }
            currentSale.CustomerSlice = customerSlice;
            customerSlice.SliceDate = currentSale.SaleDate;
            currentSale.PaymentDelay = (PaymentDelay == null) ? 0 : PaymentDelay.Value;
            currentSale.IsSpecialOrder = false;

            if (currentSale != null && currentSale.CustomerSlice.SliceAmount > 0 && BuyType == CodeValue.Accounting.DefaultCodeAccountingSection.Credit)
            {
                X.Msg.Alert("Wrong Payment Mode Select!!!",
                        Resources.MsgWrongChoixPayementMethod).Show();
                return this.Direct();
            }

            if (currentSale != null && currentSale.CustomerSlice.SliceAmount == 0 && (BuyType == CodeValue.Accounting.DefaultCodeAccountingSection.CODEBANK || BuyType == CodeValue.Accounting.DefaultCodeAccountingSection.CODECAIS))
            {
                X.Msg.Alert("Wrong Payment Mode Select!!!",
                        Resources.MsgWrongChoixPayementMethod).Show();
                return this.Direct();
            }

            currentSale.SaleLines = (List<SaleLine>)Session["salelines"];
            try
            {
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
                        X.Msg.Alert("More Money Than Expected",
                            "Sorry, You have put more Money Than Expected for this sale").Show();
                        return this.Direct();
                    }

                    SavingAccount sa = db.SavingAccounts.SingleOrDefault(sa1 => sa1.CustomerID == currentSale.CustomerID.Value);

                    if (sa == null || sa.ID == 0)
                    {

                        X.Msg.Alert("No Saving Account",
                            "Sorry, Customer doesn't have a Saving Account. Please contact an administrator").Show();
                        return this.Direct();
                    }

                    Customer savingCusto = db.Customers.Find(currentSale.CustomerID.Value);

                    double savingAccountBalance = _savingAccountRepository.GetSavingAccountBalance(savingCusto);

                    //ne faites ps d'achat en espèce si : 1 - pas d'argent en caisse; 2- Facture > Montant en caisse
                    if (savingAccountBalance <= 0 || customerSlice.SliceAmount > savingAccountBalance)
                    {
                        X.Msg.Alert("NO Enough Money in Saving Account",
                            "Sorry, Customer doesn't have sufficient Money inside his Saving Account. Please contact an administrator").Show();
                        return this.Direct();
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
                    if (currentSale != null && (currentSale.PaymentMethodID == null || currentSale.PaymentMethodID <= 0) && (BuyType == CodeValue.Accounting.DefaultCodeAccountingSection.CODEBANK || BuyType == CodeValue.Accounting.DefaultCodeAccountingSection.CODECAIS))
                    {
                        X.Msg.Alert("Wrong Payment Mode Select!!!",
                                Resources.MsgErrChoixPayementMethod).Show();
                        return this.Direct();
                    }
                    int SaleID = _saleRepository.SaveChanges(currentSale, heureVente, SessionGlobalPersonID).SaleID;
                    Session["Receipt_SaleID"] = SaleID;
                    Session["Receipt_CustomerID"] = currentSale.CustomerName;
                    Session["ReceiveAmoung_Tot"] = (currentSale.CustomerSlice != null) ? currentSale.CustomerSlice.SliceAmount : 0;

                    PrintReset(currentSale.BranchID.ToString(), SaleID, customerSlice.SliceAmount, TVA);
                }

                return this.Direct();
            }
            catch (Exception e)
            {
                X.Msg.Alert("Erreur", e.Message).Show();
                return this.Direct();
            }
        }

        public void PrintReset(string Branch, int SaleID, double SliceAmount, double TVA)
        {
            Session["salelines"] = new List<SaleLine>();
            this.GetCmp<Store>("SaleLineProperties").Reload();
            Session["CurrentProduct"] = new Product();
            Session["MaxValue"] = 0;
            this.GetCmp<FormPanel>("GlobalSaleForm").Reset();

            this.GetCmp<Button>("btnDeliveryOrder").Disabled = false;
            this.GetCmp<Button>("btnReceipt").Disabled = false;

            this.GetCmp<TextField>("marque").AllowBlank = true;
            this.GetCmp<TextField>("reference").AllowBlank = true;
            this.GetCmp<NumberField>("FramePrice").AllowBlank = true;

            this.InitTrnNumber(Branch);

            Session["SaleID"] = SaleID;
            Session["SliceAmount"] = SliceAmount;

            this.AlertSucces(Resources.Success, Resources.SaleNewSale);
        }

        public ActionResult GetQuantityStock(string Localization, string CurrentProduct)
        {
            try
            {
                if ((Localization != null && CurrentProduct != null) && (Localization.Length > 0 && CurrentProduct.Length > 0))
                {
                    int idLoc = Convert.ToInt32(Localization);
                    int idProd = Convert.ToInt32(CurrentProduct);
                    if (idLoc > 0 && idProd > 0)
                    {
                        ProductLocalization productInStock = db.ProductLocalizations.FirstOrDefault(pL => pL.LocalizationID == idLoc && pL.ProductID == idProd);
                        if (productInStock == null || productInStock.ProductLocalizationStockQuantity <= 0)
                        {
                            statusOperation = Resources.NoProductInStock;
                            X.Msg.Alert(Resources.Productlabel, statusOperation).Show();
                            return this.Direct();
                        }
                        else
                        {
                            this.GetCmp<NumberField>("LineQuantity").MaxValue = productInStock.ProductLocalizationStockQuantity;
                            Session["MaxValue"] = productInStock.ProductLocalizationStockQuantity;
                            this.GetCmp<NumberField>("LineUnitPrice").Value = productInStock.ProductLocalizationStockSellingPrice;
                        }

                    }
                }
                return this.Direct();
            }
            catch (Exception e)
            {
                statusOperation = Resources.er_alert_danger + e.Message;
                X.Msg.Alert(Resources.Productlabel, statusOperation).Show();
                return this.Direct();
            }
        }

        public StoreResult PaymentMethods(string BuyTypeCode)
        {
            return this.Store(LoadComponent.SpecificBankPaymentMethod(BuyTypeCode));
        }


        /// <summary>
        /// Return a list of localization of product
        /// </summary>
        /// <param name="BranchID"></param>
        /// <returns></returns>
        public ActionResult GetLocalization(int BranchID = 0)
        {
            List<object> modelStore = new List<object>();
            if (BranchID > 0)
            {
                db.Localizations.Where(pl => pl.BranchID == BranchID).ToList().ForEach(p =>
                {
                    modelStore.Add(
                        new
                        {
                            LocalizationID = p.LocalizationID,
                            LocalizationCode = "[" + p.LocalizationCode + "] " + p.LocalizationLabel
                        }
                     );
                }
                 );
            }
            return this.Store(modelStore);
        }
        [HttpPost]
        public ActionResult GetAllProducts(int DepartureLocalizationID)
        {

            try
            {
                List<Product> list = ModelProductLocalCat(DepartureLocalizationID);

                if (list == null || list.Count == 0)
                {
                    X.Msg.Alert("Product Stock Error", "You Don't Have this product in the selected Warehoure! Please See Administrator").Show();
                    return this.Direct();
                }

                return this.Store(list);
            }
            catch (Exception exc)
            {
                X.Msg.Alert("Price Error ", "Message = " + exc.Message + " Inner Exception = " + exc.InnerException).Show();
                return this.Direct();
            }

        }
        //recup des produits

        public List<Product> ModelProductLocalCat(int DepartureLocalizationID)
        {
            List<Product> model = new List<Product>();


            //On a un produit générique
            if (DepartureLocalizationID > 0 ) //chargement des produits en fct du magasin slt
            {
                return model;
            }
            else //On a un produit de type verre
            {
                

                    ////produit generic

                    var lstLensProduct = db.Products.Join(db.ProductLocalizations, p => p.ProductID, pl => pl.ProductID,
                        (p, pl) => new { p, pl })
                        .Where(lsp => lsp.pl.LocalizationID == DepartureLocalizationID && lsp.p.CategoryID == 2 && !(lsp.p.Category is LensCategory))
                        .Select(s => new
                        {
                            ProductID = s.p.ProductID,
                            ProductCode = s.p.ProductCode,
                            ProductLabel = s.p.ProductLabel,
                            ProductQuantity = s.pl.ProductLocalizationStockQuantity,
                            ProductLocalizationStockSellingPrice = s.pl.ProductLocalizationStockSellingPrice
                        }).ToList();

                    foreach (var pt in lstLensProduct)
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

            return model;
        }
        public DirectResult InitDate(int BranchID)
        {
            listBDUser = (List<BusinessDay>)Session["UserBusDays"];
            if (listBDUser == null)
            {
                listBDUser = _busDayRepo.GetOpenedBusinessDay(CurrentUser);
            }
            DateTime businessD = listBDUser.FirstOrDefault(b => b.BranchID == BranchID).BDDateOperation;
            this.GetCmp<DateField>("CustomerOrderDate").Value = businessD;
            return this.Direct();
        }

        public ActionResult PrintDepositReceipt(string detail)
        {
            Session["detail"] = detail;
            this.GetCmp<Panel>("Pdf").LoadContent(new ComponentLoader
            {
                Url = Url.Action("GenerateReceiptDeposit"),
                DisableCaching = false,
                Mode = LoadMode.Frame,
            });

            return this.Direct();
        }
        //This method print a receipt of customer
        public void GenerateReceiptDeposit()
        {
            //List<RptReceipt> model = new List<RptReceipt>();
            //List<RptPaymentDetail> modelsubRpt = new List<RptPaymentDetail>();
            List<object> model = new List<object>();
            List<object> modelsubRpt = new List<object>();

            ReportDocument rptH = new ReportDocument();
            try
            {
                int saleID = (Session["Receipt_SaleID"] == null) ? 0 : (int)Session["Receipt_SaleID"];
                string customerID = (Session["Receipt_CustomerID"] == null) ? "" : (string)Session["Receipt_CustomerID"];
                double receiveAmountTot = (Session["ReceiveAmoung_Tot"] == null) ? 0 : (double)Session["ReceiveAmoung_Tot"];
                string detail = (Session["detail"] == null) ? "" : (string)Session["detail"];

                string repName = "";
                bool isValid = false;
                double totalAmount = 0d;
                double totalRemaining = 0d;
                double TotalReceiveAmount = 0d;

                string path = "";
                string defaultImgPath = Server.MapPath("~/Content/Images/App/deboy-logo.png");
                byte[] COMPANY_LOGO = System.IO.File.ReadAllBytes(defaultImgPath);



                Devise devise = new Devise();
                Branch curBranch = new Branch();

                string TitleDeposit = "";
                string RptTitle = "";


                curBranch = db.Branches.Find(db.UserBranches.Where(ub => ub.UserID == SessionGlobalPersonID).FirstOrDefault().BranchID);
                BusinessDay businessDay = _busDayRepo.GetOpenedBusinessDay(curBranch);
                devise = db.Devises.FirstOrDefault(d => d.DefaultDevise);

                Company cmpny = db.Companies.FirstOrDefault();
                if (saleID > 0)//depot pour une vente
                {

                    double saleAmount = 0d;
                    SaleE currentSale = db.Sales.Find(saleID);

                    int i = 1;

                    //recuperation des versements
                    List<CustomerSlice> lstCustomerSlice = db.CustomerSlices.Where(sl => sl.SaleID == currentSale.SaleID).ToList();
                    foreach (CustomerSlice cs in lstCustomerSlice)
                    {
                        TotalReceiveAmount = TotalReceiveAmount + cs.SliceAmount;
                        modelsubRpt.Add(
                        new //RptPaymentDetail
                        {
                            Reference = currentSale.SaleReceiptNumber,
                            DepositDate = cs.SliceDate,
                            Description = i.ToString() + " Payment(s)",
                            LineUnitPrice = cs.SliceAmount,
                            RptReceiptPaymentDetailID = 1
                        });
                        i = i + 1;
                    }

                    //recuperation des versements
                    List<SaleLine> lstSaleLine = db.SaleLines.Where(sl => sl.SaleID == currentSale.SaleID).ToList();
                    totalAmount = (lstSaleLine.Count > 0) ? Util.ExtraPrices(lstSaleLine.Select(c => c.LineAmount).Sum(), currentSale.RateReduction, currentSale.RateDiscount, currentSale.Transport, currentSale.VatRate).TotalTTC : 0; //montant du verre
                    totalRemaining = totalAmount - TotalReceiveAmount;

                    foreach (SaleLine custsaleLine in lstSaleLine)
                    {
                        string labelFrame = (custsaleLine.marque != null && custsaleLine.reference != null) ? Resources.Marque + " " + custsaleLine.marque + " " + Resources.Reference + " " + custsaleLine.reference : "";
                        model.Add(
                        new //RptReceipt
                        {
                            RptReceiptID = 1,
                            ReceiveAmount = receiveAmountTot,
                            TotalAmount = totalRemaining, //montant restant de la facture
                            LineUnitPrice = Util.ExtraPrices(custsaleLine.LineAmount, currentSale.RateReduction, currentSale.RateDiscount, currentSale.Transport, currentSale.VatRate).TotalTTC, //montant du verre
                            LineQuantity = custsaleLine.LineQuantity,
                            CompanyName = cmpny.Name,
                            CompanyAdress = cmpny.Adress.AdressFullName,// "Head Quater : " + cmpny.Adress.Quarter.Town.Region.RegionLabel + " - " + cmpny.Adress.Quarter.Town.TownLabel,
                            CompanyTel = "Tel: " + cmpny.Adress.AdressPhoneNumber + " Cell:" + cmpny.Adress.AdressCellNumber,
                            BranchName = currentSale.Branch.BranchName,
                            BranchAdress = "B.P " + cmpny.Adress.AdressPOBox + ", " + cmpny.Adress.Quarter.Town.TownLabel + ", Email: " + cmpny.Adress.AdressEmail,//currentSale.Branch.Adress.Quarter.QuarterLabel + " - " + currentSale.Branch.Adress.Quarter.Town.TownLabel,
                            BranchTel = "Tel: " + currentSale.Branch.Adress.AdressPhoneNumber,
                            Reference = currentSale.SaleReceiptNumber,
                            CompanyCNI = "NO CONT : " + cmpny.CNI,
                            Operator = CurrentUser.Name + " " + CurrentUser.Description,
                            CustomerName = customerID,
                            ProductLabel = (labelFrame.Trim().Length > 0) ? Resources.frame + " " + /*custsaleLine.ProductLabel + " " +*/ labelFrame : custsaleLine.ProductLabel,
                            SaleDate = currentSale.SaleDate,
                            Title = TitleDeposit,
                            DeviseLabel = currentSale.Devise.DeviseLabel,
                            RateTVA = currentSale.VatRate,
                            RateReduction = currentSale.RateReduction,
                            RateDiscount = currentSale.RateDiscount,
                            Transport = currentSale.Transport,
                            RptReceiptPaymentDetailID = 1,
                            CompanyLogo = db.Files.FirstOrDefault(f => f.GlobalPersonID == cmpny.GlobalPersonID) != null ? db.Files.FirstOrDefault(f => f.GlobalPersonID == cmpny.GlobalPersonID).Content : COMPANY_LOGO
                        }
                    );
                        //
                    }

                    if (TotalReceiveAmount > 0)
                    {
                        if (detail == "oui")
                        {
                            path = Server.MapPath("~/Reports/CashRegister/RptReceiptDepositDetail.rpt");
                            repName = "RptReceiptDepositDetail";
                        }
                        else
                        {
                            path = Server.MapPath("~/Reports/CashRegister/RptReceiptDeposit.rpt");
                            repName = "RptReceiptDeposit";
                        }
                    }
                    else
                    {
                        if (detail == "oui")
                        {
                            path = Server.MapPath("~/Reports/CashRegister/RptReceiptSansDepositDetail.rpt");
                            repName = "RptReceiptSansDepositDetail";
                        }
                        else
                        {
                            path = Server.MapPath("~/Reports/CashRegister/RptReceiptSansDeposit.rpt");
                            repName = "RptReceiptSansDeposit";
                        }
                    }

                    isValid = true;
                }


                if (isValid)
                {
                    rptH.Load(path);
                    rptH.SetDataSource(model);
                    if (TotalReceiveAmount > 0)
                    {
                        rptH.OpenSubreport("PaymentDetail").SetDataSource(modelsubRpt);
                    }
                    //rptH.SetParameterValue("RptTitle", RptTitle);
                    bool isDownloadRpt = (bool)Session["isDownloadRpt"];
                    rptH.ExportToHttpResponse(ExportFormatType.PortableDocFormat, System.Web.HttpContext.Current.Response, isDownloadRpt, repName);
                    //return File(stream, "application/pdf");
                }
                else
                {
                    Response.Write("Nothing Found; No Report name found");
                }

            }
            finally
            {
                rptH.Close();
                rptH.Dispose();
            }
        }
    }
}