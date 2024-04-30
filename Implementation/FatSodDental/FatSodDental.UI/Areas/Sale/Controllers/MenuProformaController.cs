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
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;


namespace FatSodDental.UI.Areas.Sale.Controllers
{
    [Authorize(Order = 1)]
    [TakeBusinessDay(Order = 2)]
    public class MenuProformaController : BaseController
    {
        //Current Controller and current page
        private const string CONTROLLER_NAME = "Sale/MenuProforma";
        private const string VIEW_NAME = "Index";
        //person repository
        private ISavingAccount _savingAccountRepository;
        private ICustomerOrder _CustomerOrderRepository;
        private IBusinessDay _busDayRepo;
        private ITransactNumber _transactNumbeRepository;
        private ILensNumberRangePrice _priceRepository;
        private IDeposit _depositRepository;
        private List<BusinessDay> listBDUser;
        private bool isLens = false;
        private ITillDay _tillDayRepository;
        private LensConstruction lensFrameConstruction = new LensConstruction();
        //Construcitor
        public MenuProformaController(
            ITillDay tillDayRepository,
            ICustomerOrder CustomerOrderRepository,
            IBusinessDay busDayRepo,
            ILensNumberRangePrice lnrpRepo,
            ISavingAccount SavingAccountRepo,
            IDeposit depositRepository,
            ITransactNumber transactNumbeRepository
            )
        {
            this._tillDayRepository = tillDayRepository;
            this._CustomerOrderRepository = CustomerOrderRepository;
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


            ViewBag.Disabled = true;

            List<BusinessDay>  bdDay = (List<BusinessDay>)Session["UserBusDays"];
            if (bdDay == null)
            {
                bdDay = _busDayRepo.GetOpenedBusinessDay(CurrentUser);
            }
            if (bdDay.Count() > 1)
            {
                ViewBag.Disabled = false;
            }
            DateTime busDays = bdDay.FirstOrDefault().BDDateOperation;

            ViewBag.BusnessDayDate = busDays;
            
            Session["customerOrderLines"] = new List<CustomerOrderLine>();
            
            Session["isUpdate"] = false;
            Session["MaxValue"] = 500;
            Session["SafetyStock"] = 500;
            return View();
        }
        public ActionResult InitTrnNumber(string BranchID, int CustomerID)
        {
            listBDUser = (List<BusinessDay>)Session["UserBusDays"];
            if (listBDUser == null)
            {
                listBDUser = _busDayRepo.GetOpenedBusinessDay(CurrentUser);
            }
            BusinessDay businessDay = listBDUser.FirstOrDefault(b => b.BranchID == Convert.ToInt32(BranchID));

            //string trnnum = _transactNumbeRepository.displayTransactNumber("FPRO", businessDay);
            //this.GetCmp<TextField>("CustomerOrderNumber").Value = trnnum;
            
            //Customer customer = db.Customers.Find(CustomerID);
            //this.GetCmp<TextField>("Assurance").Value = customer.Assureur.Name;
            //this.GetCmp<TextField>("PoliceAssurance").Value = customer.PoliceAssurance;
            //this.GetCmp<TextField>("CompanyName").Value = customer.CompanyName;
            
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
        /*public bool AlertMsgNotSold(double ProductAmount)
        {
            bool res = false;
            double LimitAmount = (double)Session["LimitAmount"];
            double CustomerDebt = (double)Session["CustomerDebt"];

            if (LimitAmount > 0)
            {
                if (CustomerDebt >= 0) res = true;
                else if ((CustomerDebt * -1 + ProductAmount) >= LimitAmount)
                {
                    X.Msg.Alert("Sorry", Resources.MsgLimitAmount).Show();
                    res = false;
                }
                else res = true;
            }
            else res = true;
            return res;
        }*/
        //[DirectMethod]
        //[HttpPost]
        public ActionResult AddCustomerOrderLine(SpecialLensModel slm, int? ProductID, double LineUnitPrice, int LineID, double reduction = 0, double discount = 0, double transport = 0)
        {
            try
            {

                this.DoYes(slm, reduction, discount, transport);
                return this.Reset2();
            }
            catch (Exception e) { X.Msg.Alert("Error ", e.Message).Show(); return this.Direct(); }
        }
        public ActionResult DisplayCommand(int? SalesProductsType)
        {
            try
            {
                if (SalesProductsType == null) return this.Direct();
                if (SalesProductsType == 1) //verre-cadre
                {
                    this.GetCmp<FieldSet>("frameProperties").Hidden=false;
                    this.GetCmp<FieldSet>("lensProperties").Hidden = false;
                    this.GetCmp<FieldSet>("OtherProperties").Hidden = true;
                }
                else 
                {
                    this.GetCmp<FieldSet>("frameProperties").Hidden = true;
                    this.GetCmp<FieldSet>("lensProperties").Hidden = true;
                    this.GetCmp<FieldSet>("OtherProperties").Hidden = false;
                }
                return this.Direct();
            }
            catch (Exception e) { X.Msg.Alert("Error ", e.Message).Show(); return this.Direct(); }
        }
        
        //This method add a CustomerOrderLine in the current sale
        [DirectMethod]
        public ActionResult DoYes(SpecialLensModel slm, double reduction = 0, double discount = 0, double transport = 0)
        {
            try
            {
                double VatRate = CodeValue.Accounting.ParamInitAcct.VATRATE;
                slm.LineQuantity = 1;
                List<CustomerOrderLine> customerOrderLines = (List<CustomerOrderLine>)Session["customerOrderLines"];
                List<CustomerOrderLine> cols = lensFrameConstruction.Get_COL_CUSTORDER_From_SLM(slm, new FatSod.DataContext.Concrete.EFDbContext());
                foreach (CustomerOrderLine customerOrderLine in cols)
                {
                    //Construction du code du produit en fonction de ce qui a été saisie par l'utilisateur
                    customerOrderLine.Product = LensConstruction.GetProductByCustOrderLine(customerOrderLine, new FatSod.DataContext.Concrete.EFDbContext());
                    if (customerOrderLine.Product == null)
                    {
                        ApplyExtraPrices(customerOrderLines, reduction, discount, transport, VatRate);
                        Session["customerOrderLines"] = customerOrderLines;
                        return this.Direct();
                    }
                    if (!(customerOrderLine.Product is OrderLens))
                    {
                        bool res = this.CheckQty(customerOrderLine.LocalizationID, customerOrderLine.Product.ProductID, customerOrderLine.LineQuantity);
                        if (!res) return this.Direct();
                    }
                    if (customerOrderLine.LineID > 0)
                    {
                        //Ce produit existe deja dans le panier, alors on enleve les deux lignes liées au SpecialOrderLineCode dans la ligne
                        //1-Coe c'est une modification, on enlève l'existant de la ligne en cours de modification; on va l'ajouter dans la suite(Drop and Create)

                        customerOrderLines.RemoveAll(col => col.LineID == customerOrderLine.LineID);
                        //2-Si actuellement on a une seule ligne dans la collection, il y a une possibilité qu'on en avait deux et l'autre a été supprimée; il faut donc le supprimer dans le panier
                        if (cols.Count <= 1) customerOrderLines.RemoveAll(col => col.SpecialOrderLineCode == customerOrderLine.SpecialOrderLineCode);
                    }

                    if (customerOrderLines != null && customerOrderLines.Count() > 0)
                    {
                        CustomerOrderLine customerOrderLineExist = customerOrderLines.FirstOrDefault(s => s.Product.ProductCode == customerOrderLine.Product.ProductCode && s.SpecialOrderLineCode == customerOrderLine.SpecialOrderLineCode);
                        if (customerOrderLineExist != null)
                        {
                            customerOrderLines.Remove(customerOrderLineExist);
                        }

                        int maxLineID = (customerOrderLines != null && customerOrderLines.Count() > 0) ? customerOrderLines.Select(l => l.LineID).Max() : 0;

                        customerOrderLine.LineID = (maxLineID + 1);

                        customerOrderLines.Add(customerOrderLine);
                    }
                    else
                    {
                        customerOrderLines = new List<CustomerOrderLine>();
                        customerOrderLine.LineID = 1;
                        customerOrderLines.Add(customerOrderLine);
                    }
                }

                //double VatRate = CodeValue.Accounting.ParamInitAcct.VATRATE;
                ApplyExtraPrices(customerOrderLines, reduction, discount, transport, VatRate);
                Session["customerOrderLines"] = customerOrderLines;

                //this.refreshCmdLine();

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

        public ActionResult RActivateAxe(string CylinderVal)
        {
            if (CylinderVal==null || CylinderVal.Length==0)
            {
                this.GetCmp<TextField>("REAxis").ReadOnly = true;
            }
            else
            {
                this.GetCmp<TextField>("REAxis").ReadOnly = false;
            }
            return this.Direct();
        }
        public ActionResult LActivateAxe(string CylinderVal)
        {
            if (CylinderVal == null || CylinderVal.Length == 0)
            {
                this.GetCmp<TextField>("LEAxis").ReadOnly = true;
            }
            else
            {
                this.GetCmp<TextField>("LEAxis").ReadOnly = false;
            }
            return this.Direct();
        }
        public ActionResult SetSupplyingName(string CategoryCode)
        {


            LensCategory cat = (from cate in db.LensCategories
                                where cate.CategoryCode == CategoryCode
                                select cate).SingleOrDefault();
            
            this.SimpleResetLens();
            //si simple vision 
            if (cat.TypeLens=="SV")
            {
                this.GetCmp<TextField>("REAddition").ReadOnly=true;
                this.GetCmp<TextField>("LEAddition").ReadOnly = true;
            }
            else
            {
                this.GetCmp<TextField>("REAddition").ReadOnly = false;
                this.GetCmp<TextField>("LEAddition").ReadOnly = false;
            }
            return this.Direct();
        }

        [DirectMethod]
        //[HttpPost]
        public bool CheckQty(int LocalizationID, int? ProductID, double LineQuantity)
        {
            bool res = false;
            try
            {

                double currentQteEnStock = (double)Session["MaxValue"];
                //List<CustomerOrderLine> customerOrderLines = (List<CustomerOrderLine>)Session["customerOrderLines"];

                //recherche des qtes commandes en attente de validation pr ce produit et cette localization
                double qtyComNonValide = 0;


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
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.WARNING,
                        Title = "Command Order",
                        Message = statusOperation
                    });
                    return res;
                }

                res = true;
                return res;
            }
            catch (Exception e)
            {
                X.Msg.Alert("Error ", e.Message).Show();
                return res;
            }
        }
        
        [HttpPost]
        public ActionResult Reset2()
        {
            SimpleReset2();
            return this.Direct();
        }
        private void SimpleReset()
        {
            List<CustomerOrderLine> customerOrderLines = new List<CustomerOrderLine>();
            Session["customerOrderLines"] = customerOrderLines;
            this.GetCmp<FormPanel>("GlobalAssureSaleForm").Reset(true);
            this.GetCmp<Store>("CustomerOrderLineProperties").Reload();

        }
        private void SimpleReset2()
        {
            this.GetCmp<FormPanel>("FormAddCustomerOrderLine").Reset(true);
            this.GetCmp<Store>("CustomerOrderLineProperties").Reload();
            this.GetCmp<NumberField>("GridState").Value = 1;
        }
        private void SimpleResetLens()
        {
            //cote droit
            this.GetCmp<TextField>("RESphere").Value="";
            this.GetCmp<TextField>("RECylinder").Value = "";
            this.GetCmp<TextField>("REAxis").Value = "";
            this.GetCmp<TextField>("REAddition").Value = "";
            this.GetCmp<TextField>("REIndex").Value = "";

            //cote gauche
            this.GetCmp<TextField>("LESphere").Value = "";
            this.GetCmp<TextField>("LECylinder").Value = "";
            this.GetCmp<TextField>("LEAxis").Value = "";
            this.GetCmp<TextField>("LEAddition").Value = "";
            this.GetCmp<TextField>("LEIndex").Value = "";
        }
        public ActionResult DisableNumero(int ProductCategoryID)
        {
            Category catprod = db.Categories.Find(ProductCategoryID);
            if (catprod is LensCategory)
            {
                this.GetCmp<Container>("EyeSide").Disabled = false;
                this.GetCmp<ComboBox>("ProductNumberID").Disabled = false;
                this.GetCmp<ComboBox>("Product").HideTrigger = true;
            }
            else
            {
                this.GetCmp<Container>("EyeSide").Disabled = true;
                this.GetCmp<ComboBox>("ProductNumberID").Disabled = true;
                this.GetCmp<ComboBox>("Product").HideTrigger = false;
            }
            return this.Direct();
        }

        public ActionResult GetAllPagingNumbers(int start, int limit, int page, string query, int? ProductCategory, int? localization)
        {
            bool isUpdate = (bool)Session["isUpdate"];
            string LensNumberFullCode = (string)Session["LensNumberFullCode"];

            query = (isUpdate == true) ? LensNumberFullCode : query;

            Paging<LensNumber> numbers = GetAllNumbers(start, limit, "", "", query, ProductCategory, localization);

            Session["isUpdate"] = false;
            Session["LensNumberFullCode"] = "*";

            return this.Store(numbers.Data, numbers.TotalRecords);
        }
        public Paging<LensNumber> GetAllNumbers(int start, int limit, string sort, string dir, string filter, int? ProductCategory, int? localization)
        {

            List<LensNumber> numbers = ModelLensNumber(ProductCategory.Value, localization.Value);

            if (!string.IsNullOrEmpty(filter) && filter != "*")
            {
                numbers.RemoveAll(number => !number.LensNumberFullCode.ToLower().StartsWith(filter.ToLower().Trim()));
            }

            if (!string.IsNullOrEmpty(sort))
            {
                numbers.Sort(delegate(LensNumber x, LensNumber y)
                {
                    object a;
                    object b;

                    int direction = dir == "DESC" ? -1 : 1;

                    a = x.GetType().GetProperty(sort).GetValue(x, null);
                    b = y.GetType().GetProperty(sort).GetValue(y, null);

                    return CaseInsensitiveComparer.Default.Compare(a, b) * direction;
                });
            }

            if ((start + limit) > numbers.Count)
            {
                limit = numbers.Count - start;
            }

            List<LensNumber> rangePlants = (start < 0 || limit < 0) ? numbers : numbers.GetRange(start, limit);

            return new Paging<LensNumber>(rangePlants, numbers.Count);

        }
        public List<LensNumber> ModelLensNumber(int ProductCategoryID, int Localization)
        {

            List<LensNumber> model = new List<LensNumber>();

            //produit de verre
            try
            {

                Category catprod = db.Categories.Find(ProductCategoryID);

                isLens = (catprod is LensCategory);

                if (isLens)
                {
                    string categoryCode = catprod.CategoryCode;

                    if (categoryCode.ToLower().Contains("SV".ToLower()))
                    {
                        LoadComponent.GetAllSVNumbers().ForEach(ln =>
                        {

                            LensNumber ln2 = new LensNumber()
                            {
                                LensNumberAdditionValue = ln.LensNumberAdditionValue,
                                LensNumberCylindricalValue = ln.LensNumberCylindricalValue,
                                LensNumberDescription = ln.LensNumberDescription,
                                LensNumberID = ln.LensNumberID,
                                LensNumberSphericalValue = ln.LensNumberSphericalValue
                            };

                            model.Add(ln2);
                        });
                    }

                    if (categoryCode.ToLower().Contains("BF".ToLower()) || categoryCode.ToLower().Contains("PRO".ToLower()))
                    {
                        LoadComponent.GetAllNumbers().ForEach(ln =>
                        {

                            LensNumber ln2 = new LensNumber()
                            {
                                LensNumberAdditionValue = ln.LensNumberAdditionValue,
                                LensNumberCylindricalValue = ln.LensNumberCylindricalValue,
                                LensNumberDescription = ln.LensNumberDescription,
                                LensNumberID = ln.LensNumberID,
                                LensNumberSphericalValue = ln.LensNumberSphericalValue
                            };

                            model.Add(ln2);
                        });
                    }

                }

            }
            catch (Exception e)
            {

                throw e;
            }
            return model;
        }

        [HttpPost]
        public ActionResult OnLensNumberSelected(int? Localization, int? ProductNumberID, int? ProductCategoryID)
        {
            try
            {
                if ((Localization.HasValue && Localization.Value > 0) && (ProductNumberID.HasValue && ProductNumberID.Value > 0) && (ProductCategoryID.HasValue && ProductCategoryID.Value > 0))
                {
                    this.GetCmp<ComboBox>("Product").SetValueAndFireSelect(GetLensID(Localization.Value, ProductCategoryID.Value, ProductNumberID.Value));
                }
                else
                {
                    X.Msg.Alert(Resources.Productlabel, "This Product does not exist").Show();
                }
                return this.Direct();
            }
            catch (Exception e)
            {
                statusOperation = Resources.er_alert_danger + e.Message + "Méthode = OnLensNumberSelected() " + e.TargetSite;
                X.Msg.Alert(Resources.Productlabel, statusOperation).Show();
                return this.Direct();
            }
        }


        public int GetLensID(int DepartureLocalizationID, int ProductCategoryID, int ProductNumberID)
        {
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
                            ProductLocalizationStockSellingPrice = s.pl.ProductLocalizationStockSellingPrice,
                            ProductLocalizationSafetyStockQuantity = s.pl.ProductLocalizationSafetyStockQuantity
                        }).FirstOrDefault();

            Session["MaxValue"] = lstLensProduct.ProductQuantity;
            Session["LineUnitPrice"] = lstLensProduct.ProductLocalizationStockSellingPrice;
            Session["CurrentProduct"] = lstLensProduct.ProductLabel;
            Session["SafetyStock"] = lstLensProduct.ProductLocalizationSafetyStockQuantity;
            return lstLensProduct.ProductID;

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
                    //this.GetCmp<NumberField>("LineUnitPrice").Value = (price != null && price.LensNumberRangePriceID > 0) ? price.Price : 0;
                    this.GetCmp<NumberField>("FramePrice").Value = (price != null && price.LensNumberRangePriceID > 0) ? price.Price : 0;
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

                    this.GetCmp<TextField>("marque").AllowBlank = false;
                    this.GetCmp<TextField>("reference").AllowBlank = false;
                    this.GetCmp<NumberField>("FramePrice").AllowBlank = false;
                    this.GetCmp<NumberField>("FrameCategory").AllowBlank = false;

                    if (!isUpdate)
                    {
                        //this.GetCmp<NumberField>("LineUnitPrice").Value = prodLoc.SellingPrice;
                        this.GetCmp<NumberField>("FramePrice").Value = prodLoc.SellingPrice;
                        
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

       // [HttpPost]
        [DirectMethod]
        public ActionResult OnBarCodeReader(int? Localization, string BarCode)
        {
            try
            {
                 
                if ((!Localization.HasValue || Localization.Value <= 0) || (BarCode==null)) { return this.Direct(); }

                BarCodeGenerator barcodegen = db.BarCodeGenerators.FirstOrDefault(bc=>bc.CodeBar==BarCode);
                if (barcodegen == null) { return this.Direct(); }

                int CurrentProduct=barcodegen.ProductID;
                Product product = db.Products.Find(CurrentProduct);

                //propriete frame
                this.GetCmp<ComboBox>("ProductID").GetStore().Reload();
                this.GetCmp<ComboBox>("ProductID").SetValue(product.ProductID);

                bool productIsLens = product is Lens;

                if (productIsLens)
                {
                    this.GetCmp<NumberField>("StockQuantity").Value = (double)Session["MaxValue"];
                    //Récupération du prix du verre à partir de son intervalle de numéro
                    LensNumberRangePrice price = _priceRepository.GetPrice(product.ProductID);
                    //this.GetCmp<NumberField>("LineUnitPrice").Value = (price != null && price.LensNumberRangePriceID > 0) ? price.Price : 0;
                }
                else
                {
                    var prodLoc = db.ProductLocalizations.Where(pl => pl.ProductID == CurrentProduct && pl.LocalizationID == Localization.Value)
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
                        //this.GetCmp<NumberField>("LineUnitPrice").Value = prodLoc.SellingPrice;
                        Session["isUpdate"] = false;
                    }
                }

                return this.Direct();
            }
            catch (Exception e)
            {
                statusOperation = Resources.er_alert_danger + e.Message + "Source = " + e.Source + "Méthode = OnBarCodeReader(" + Localization.Value + " " + BarCode + ") " + e.TargetSite + " InnerException = " + e.InnerException;
                X.Msg.Alert(Resources.Productlabel, statusOperation).Show();
                return this.Direct();
            }
        }

        public void ApplyExtraPrices(List<CustomerOrderLine> customerOrderLines, double reduction, double discount, double transport, double vatRate)
        {

            double valueOperation = customerOrderLines.Select(l => l.LineUnitPrice).Sum(); //PARCEKE ICI C'EST 1 VERRE LA QTE
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
            //}
            
            

        }
        //This method remove CustomerOrderLine
        [HttpPost]
        public ActionResult RemoveCustomerOrderLine(int ID, double reduction = 0, double discount = 0, double transport = 0)
        {
            try
            {
                List<CustomerOrderLine> customerOrderLines = (List<CustomerOrderLine>)Session["customerOrderLines"];
                CustomerOrderLine lineToUpdate = customerOrderLines.FirstOrDefault(l => l.LineID == ID);
                //suppression des verres
                if (lineToUpdate.SpecialOrderLineCode != null)
                {
                    customerOrderLines.RemoveAll(col => col.SpecialOrderLineCode == lineToUpdate.SpecialOrderLineCode);
                }
                else
                {
                    //remove other
                    customerOrderLines.RemoveAll(col => col.LineID == ID);
                }

                ApplyExtraPrices(customerOrderLines, reduction, discount, transport, CodeValue.Accounting.ParamInitAcct.VATRATE);

                this.GetCmp<Store>("CustomerOrderLineProperties").Reload();
                return this.Reset2();
            }
            catch (Exception e) { X.Msg.Alert("Error ", e.Message).Show(); return this.Direct(); }
        }
        
        [HttpPost]
        public ActionResult Reset()
        {

            this.SimpleReset();
            this.SimpleReset2();
            return this.Direct();
        }

        [HttpPost]
        public ActionResult UpdateLine(int ID)
        {
            try
            {
                List<CustomerOrderLine> customerOrderLines = (List<CustomerOrderLine>)Session["customerOrderLines"];

                CustomerOrderLine lineToUpdate = customerOrderLines.FirstOrDefault(l => l.LineID == ID);

                if (lineToUpdate.SpecialOrderLineCode != null)
                {
                    this.GetCmp<FieldSet>("frameProperties").Hidden = false;
                    this.GetCmp<FieldSet>("lensProperties").Hidden = false;
                    this.GetCmp<FieldSet>("OtherProperties").Hidden = true;

                    List<CustomerOrderLine> cols = customerOrderLines.Where(l => l.SpecialOrderLineCode == lineToUpdate.SpecialOrderLineCode).ToList();
                    SpecialLensModel slm = lensFrameConstruction.Get_SLM_From_COL(cols);

                    this.GetCmp<TextField>("LineID").SetValue(slm.LineID);
                    this.GetCmp<TextField>("LELineID").SetValue(slm.LELineID);
                    this.GetCmp<TextField>("RELineID").SetValue(slm.RELineID);
                    this.GetCmp<TextField>("FRLineID").SetValue(slm.FRLineID);
                    this.GetCmp<TextField>("SpecialOrderLineCode").SetValue(slm.SpecialOrderLineCode);
                    this.GetCmp<ComboBox>("LocalizationID").SetValue(slm.LocalizationID);
                    this.GetCmp<ComboBox>("LensCategoryCode").SetValue(slm.LensCategoryCode);
                    this.GetCmp<ComboBox>("FrameCategory").SetValue(slm.FrameCategory);

                    //Côté Gauche de l'oeil
                    this.GetCmp<TextField>("LESphere").Value = slm.LESphere;
                    this.GetCmp<TextField>("LECylinder").Value = slm.LECylinder;
                    this.GetCmp<TextField>("LEAxis").Value = slm.LEAxis;
                    this.GetCmp<TextField>("LEAddition").Value = slm.LEAddition;
                    this.GetCmp<TextField>("LEIndex").Value = slm.LEIndex;
                    this.GetCmp<NumberField>("LEPrice").Value = slm.LEPrice;

                    //Côté Droit de l'oeil
                    this.GetCmp<TextField>("RESphere").Value = slm.RESphere;
                    this.GetCmp<TextField>("RECylinder").Value = slm.RECylinder;
                    this.GetCmp<TextField>("REAxis").Value = slm.REAxis;
                    this.GetCmp<TextField>("REAddition").Value = slm.REAddition;
                    this.GetCmp<TextField>("REIndex").Value = slm.REIndex;
                    this.GetCmp<NumberField>("REPrice").Value = slm.REPrice;

                    this.GetCmp<NumberField>("LensPrice").Value = slm.LEPrice + slm.REPrice;

                    //propriete frame
                    if (slm.FrameProductID > 0)
                    {
                        this.GetCmp<ComboBox>("ProductID").GetStore().Reload();
                        this.GetCmp<ComboBox>("ProductID").SetValue(slm.FrameProductID);

                        this.GetCmp<TextField>("marque").Value = slm.marque;
                        this.GetCmp<TextField>("reference").Value = slm.reference;
                        this.GetCmp<NumberField>("FramePrice").Value = slm.FramePrice;

                        double qtystock = GetMaxQtyInStock(slm.FrameProductID, slm.LocalizationID);
                        this.GetCmp<NumberField>("StockQuantity").Value = qtystock;
                        Session["MaxValue"] = qtystock;
                        Session["CurrentProduct"] = lineToUpdate.ProductLabel;
                    }

                }
                else
                {
                    this.GetCmp<FieldSet>("frameProperties").Hidden = true;
                    this.GetCmp<FieldSet>("lensProperties").Hidden = true;
                    this.GetCmp<FieldSet>("OtherProperties").Hidden = false;

                    //CustomerOrderLine lineFRAMEToUpdate = customerOrderLines.FirstOrDefault(l => l.LineID == ID);
                    ////propriete frame
                    //this.GetCmp<ComboBox>("ProductID").GetStore().Reload();
                    //this.GetCmp<ComboBox>("ProductID").SetValue(lineFRAMEToUpdate.ProductID);

                    //double qtystock = GetMaxQtyInStock(lineFRAMEToUpdate.ProductID, lineFRAMEToUpdate.LocalizationID);
                    //this.GetCmp<NumberField>("StockQuantity").Value = qtystock;
                    //Session["MaxValue"] = qtystock;
                    //Session["CurrentProduct"] = lineFRAMEToUpdate.ProductLabel;
                    //this.GetCmp<NumberField>("LineQuantity").Value = slm.LineQuantity;

                }



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
        //Return customerOrderLines of current sale
        [HttpPost]
        public StoreResult CustomerOrderLines()
        {
            List<CustomerOrderLine> customerOrderLines = (List<CustomerOrderLine>)Session["customerOrderLines"];
            List<object> model = new List<object>();
            customerOrderLines.ForEach(c =>
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
        public ActionResult AddAssureSale(CustomerOrder customerOrder)
        {
            if (customerOrder.Remarque == "" || customerOrder.Remarque == null)
            {
                X.Msg.Alert("Field is compulsory",
                        "Remarque Field is compulsory!!!").Show();
                return this.Direct();
            }
            if (customerOrder.MedecinTraitant == "" || customerOrder.MedecinTraitant == null)
            {
                X.Msg.Alert("Field is compulsory",
                        "Medecin Traitant Field is compulsory!!!").Show();
                return this.Direct();
            }

            customerOrder.CustomerOrderLines = (List<CustomerOrderLine>)Session["customerOrderLines"];
            try
            {
                customerOrder.BillState = StatutFacture.Proforma;
               int CustomerId= _CustomerOrderRepository.SaveChanges(customerOrder, SessionGlobalPersonID).CustomerOrderID;
               Session["Receipt_CommandID"] = CustomerId;
                PrintReset();
                return this.Direct();
            }
            catch (Exception e)
            {
                X.Msg.Alert("Erreur", e.Message).Show();
                return this.Direct();
            }
        }

        public void PrintReset()
        {
            Session["customerOrderLines"] = new List<CustomerOrderLine>();
            this.GetCmp<Store>("CustomerOrderLineProperties").Reload();
            Session["CurrentProduct"] = new Product();
            Session["MaxValue"] = 0;
            this.GetCmp<FormPanel>("GlobalAssureSaleForm").Reset();
 
            this.GetCmp<Button>("btnDeliveryOrder").Disabled = false;
            this.GetCmp<Button>("btnProformaDetail").Disabled = false;

            this.GetCmp<TextField>("marque").AllowBlank = true;
            this.GetCmp<TextField>("reference").AllowBlank = true;
            this.GetCmp<NumberField>("FramePrice").AllowBlank = true;
            this.GetCmp<NumberField>("FrameCategory").AllowBlank = true;

            this.AlertSucces(Resources.Success, Resources.SaleNewSale);
        }


        public ActionResult PrintFactureProforma(string detail)
        {
            Session["detail"] = detail;
            this.GetCmp<Panel>("Pdf").LoadContent(new ComponentLoader
            {
                Url = Url.Action("GenerateFactureProforma"),
                DisableCaching = false,
                Mode = LoadMode.Frame,
            });

            return this.Direct();
        }
        //This method print a receipt of customer
        public void GenerateFactureProforma()
        {
            //List<RptReceipt> model = new List<RptReceipt>();
            //List<RptPaymentDetail> modelsubRpt = new List<RptPaymentDetail>();
            List<object> model = new List<object>();
            List<object> modelsubRpt = new List<object>();

            ReportDocument rptH = new ReportDocument();
            try
            {
                int CommandID = (Session["Receipt_CommandID"] == null) ? 0 : (int)Session["Receipt_CommandID"];
                string detail = (Session["detail"] == null) ? "" : (string)Session["detail"]; 

                string repName = "";
                bool isValid = false;
                double totalAmount = 0d;
                double totalRemaining = 0d;
                double TotalReceiveAmount = 0d;

                string path = "";
                string defaultImgPath = Server.MapPath("~/Content/Images/App/deboy-logo.png");
                byte[] COMPANY_LOGO = System.IO.File.ReadAllBytes(defaultImgPath);

                
                //Devise devise = new Devise();
                //Branch curBranch = new Branch();

                string TitleDeposit = "";
                string RptTitle = "";


                //curBranch = db.Branches.Find(db.UserBranches.Where(ub => ub.UserID == SessionGlobalPersonID).FirstOrDefault().BranchID);
                //BusinessDay businessDay = _busDayRepo.GetOpenedBusinessDay(curBranch);
                //devise = db.Devises.FirstOrDefault(d => d.DefaultDevise);

                Company cmpny = db.Companies.FirstOrDefault();
                if (CommandID > 0)//depot pour une vente
                {

                    double saleAmount = 0d;
                    CustomerOrder currentOrder = db.CustomerOrders.Find(CommandID);
                    //Customer customerRpt = db.Customers.Find(currentOrder.AssureurID);

                    string Prescription = "";
                    
                    //recuperation des versements
                    List<CustomerOrderLine> lstOrderLine = db.CustomerOrderLines.Where(sl => sl.CustomerOrderID == currentOrder.CustomerOrderID).ToList();
                    totalAmount = (lstOrderLine.Count > 0) ? Util.ExtraPrices(lstOrderLine.Select(c => c.LineUnitPrice).Sum(), currentOrder.RateReduction, currentOrder.RateDiscount, currentOrder.Transport, currentOrder.VatRate).TotalTTC : 0; //montant du verre
                    //FatSod.Ressources.Resources.Culture = System.Globalization.CultureInfo.CreateSpecificCulture("fr");
                    string montantLettre = NumberConverter.Spell((ulong)totalAmount).ToUpper();
                    totalRemaining = totalAmount - TotalReceiveAmount;

                    foreach (CustomerOrderLine custOrderLine in lstOrderLine)
                    {
                        string labelFrame = (custOrderLine.marque != null && custOrderLine.reference != null) ? Resources.Marque.ToUpper() + " :" + custOrderLine.marque + " " + Resources.Reference.ToUpper() + " :" + custOrderLine.reference : "";
                        int i = (custOrderLine.marque != null && custOrderLine.reference != null) ? 2 : 1;
                        if (labelFrame.Trim().Length > 0)
                        {
                            Prescription = labelFrame;
                        }
                        else
                        {
                            if (custOrderLine.Product.Prescription != null)
                            {
                                Prescription = custOrderLine.Product.Prescription;
                            }
                            else
                            {
                                Prescription = (custOrderLine.Product.ProductCode.EndsWith("HD")) ? custOrderLine.Product.ProductCode.Replace(custOrderLine.Product.Category.CategoryCode + " HD", "") : custOrderLine.Product.ProductCode.Replace(custOrderLine.Product.Category.CategoryCode, "");
                            }
                        }
                        model.Add(
                        new //RptReceipt
                        {
                            RptReceiptID = 1,
                            ReceiveAmount = 0,
                            TotalAmount = totalAmount, //montant restant de la facture
                            LineUnitPrice = Util.ExtraPrices(custOrderLine.LineUnitPrice, currentOrder.RateReduction, currentOrder.RateDiscount, currentOrder.Transport, currentOrder.VatRate).TotalTTC, //montant du verre
                            CompanyName = cmpny.Adress.AdressFullName,
                            CompanyAdress ="B.P. "+cmpny.Adress.AdressPOBox+", "+cmpny.Adress.Quarter.Town.TownLabel+", Email:"+cmpny.Adress.AdressEmail,// "Head Quater : " + cmpny.Adress.Quarter.Town.Region.RegionLabel + " - " + cmpny.Adress.Quarter.Town.TownLabel,
                            CompanyTel = "Tel: " + cmpny.Adress.AdressPhoneNumber+", Fax: "+cmpny.Adress.AdressFax+", Cell: "+cmpny.Adress.AdressCellNumber,
                            CustomerAdress =currentOrder.PhoneNumber, // "No.ONOC:87 / ONOC",
                            BranchName = currentOrder.Branch.BranchName,
                            BranchAdress = currentOrder.Branch.Adress.Quarter.QuarterLabel + " - " + currentOrder.Branch.Adress.Quarter.Town.TownLabel,
                            BranchTel = "Tel: " + currentOrder.Branch.Adress.AdressPhoneNumber,
                            Reference = currentOrder.CustomerOrderNumber,
                            CompanyCNI = "NO CONT : " + cmpny.CNI,
                            Operator = CurrentUser.Name + " " + CurrentUser.Description,
                            CustomerName = currentOrder.CustomerName,// customerRpt.Name+ " "+customerRpt.Description,
                            ProductLabel = Prescription, //(labelFrame.Trim().Length > 0) ? labelFrame : custOrderLine.Product.Prescription,
                            SaleDate = currentOrder.CustomerOrderDate,
                            Title = cmpny.Adress.Quarter.Town.TownLabel,
                            DeviseLabel = cmpny.CompanyTradeRegister,//currentOrder.Devise.DeviseLabel,
                            RateTVA = currentOrder.VatRate,
                            RateReduction = currentOrder.RateReduction,
                            RateDiscount = currentOrder.RateDiscount,
                            Transport = currentOrder.Transport,
                            RptReceiptPaymentDetailID = i,
                            LineQuantity=(labelFrame.Trim().Length > 0) ? custOrderLine.LineQuantity : custOrderLine.LineQuantity*2,
                            //CustomerAccount = currentOrder.AssureurName, //nom de la societe assureur
                            BranchAbbreviation = currentOrder.CompanyName, //NOM DE LA SOCIETE DU CLIENT
                            MontantLettre=montantLettre, //total montant en lettre
                            ProductRef = (labelFrame.Trim().Length > 0) ? "MATIERE :" +custOrderLine.FrameCategory : custOrderLine.Product.Category.CategoryDescription,
                            CompanyLogo = db.Files.FirstOrDefault(f => f.GlobalPersonID == cmpny.GlobalPersonID) != null ? db.Files.FirstOrDefault(f => f.GlobalPersonID == cmpny.GlobalPersonID).Content : COMPANY_LOGO
                        }
                    );
                        //
                    }
                    if (detail=="oui")
                    {
                        path = Server.MapPath("~/Reports/CashRegister/RptProformaDetail.rpt");
                        repName = "RptProformaDetail";
                    }
                    else
                    {
                        path = Server.MapPath("~/Reports/CashRegister/RptProforma.rpt");
                        repName = "RptProforma";
                    }
                    
                   
                    isValid = true;
                }


                if (isValid)
                {
                    rptH.Load(path);
                    rptH.SetDataSource(model);
                    
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
                            //this.GetCmp<NumberField>("LineUnitPrice").Value = productInStock.ProductLocalizationStockSellingPrice;
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
        public ActionResult GetAllProducts(int DepartureLocalizationID/*, int? ProductCategoryID, int? ProductNumberID*/)
        {

            try
            {

                List<Product> list = ModelProductLocalCat(DepartureLocalizationID/*, ProductCategoryID, ProductNumberID*/);

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

        public List<Product> ModelProductLocalCat(int DepartureLocalizationID/*, int? ProductCategoryID, int? ProductNumberID*/)
        {
            List<Product> model = new List<Product>();


            //On a un produit générique
            if (DepartureLocalizationID <= 0 /*&& (ProductCategoryID == 0 || ProductCategoryID == null) && (ProductNumberID == 0 || ProductNumberID == null)*/) //chargement des produits en fct du magasin slt
            {
                return model;
            }
            else //On a un produit de type verre
            {
                // verifion si c'est un produit de type verre
              /*
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
                    else return model;
                }
                else
                {
                    */
                    ////produit generic

                    var lstLensProduct = db.GenericProducts.Join(db.ProductLocalizations, p => p.ProductID, pl => pl.ProductID,
                        (p, pl) => new { p, pl })
                        .Where(lsp => lsp.pl.LocalizationID == DepartureLocalizationID && lsp.p.CategoryID == 1 && !(lsp.p.Category is LensCategory))
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

                //}
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
    }
}