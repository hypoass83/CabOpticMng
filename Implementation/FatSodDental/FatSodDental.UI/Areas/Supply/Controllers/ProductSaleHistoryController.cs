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
using System.Collections;
using FatSod.DataContext.Concrete;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using FastSod.Utilities.Util;


namespace FatSodDental.UI.Areas.Supply.Controllers
{
    [Authorize(Order = 1)]
    [TakeBusinessDay(Order = 2)]
    public partial class ProductSaleHistoryController : BaseController
    {
        private bool isLens = false;
        //Current Controller and current page
        private const string CONTROLLER_NAME = "Supply/ProductSaleHistory" ;
        private const string VIEW_NAME = "Index";
        private IBusinessDay _busDayRepo;
        private ICustomerReturn _customerReturnRepository;
        private IProductLocalization _productLocRepository;
        private ISale _saleRepository;

        //Construcitor

        private List<BusinessDay> lstBusDay;
        public ProductSaleHistoryController( 
            IBusinessDay bDRepo,
            ICustomerReturn customerReturnRepository,
            ISale sale,
            IProductLocalization productLocRepo
            )
        {
            this._busDayRepo = bDRepo;
            this._customerReturnRepository = customerReturnRepository;
            this._saleRepository = sale;
            this._productLocRepository = productLocRepo;
        }
        [OutputCache(Duration=43200)]
        public ActionResult Index()
        {
            Session["isUpdate"] = false;
            Session["LensNumberFullCode"] = "*";
            
            lstBusDay = (List<BusinessDay>)Session["UserBusDays"];
            if (lstBusDay == null)
            {
                lstBusDay = _busDayRepo.GetOpenedBusinessDay(CurrentUser);
            };
            DateTime BDDateOperation = lstBusDay.FirstOrDefault().BDDateOperation;
            
            ViewBag.BusnessDayDate = BDDateOperation;

            Session["BeginDate"] = new DateTime(1900, 1, 1);
            Session["EndDate"] = new DateTime(1900, 1, 1);
            Session["ProductLocalization"] = new ProductLocalization();

            this.chargeSolde();
            return View(ModelRptProductSale(new DateTime(1900, 1, 1), new DateTime(1900, 1, 1), new ProductLocalization()));

            //return View(ModelPendingProductSaleHistory());
        }

        public ActionResult DisplayEntries(DateTime BeginDate, DateTime EndDate/*,ProductLocalization pl*/)
        {
            Session["BeginDate"] = BeginDate;
            Session["EndDate"] = EndDate;
            //Session["ProductLocalization"] = pl;
            this.PartialReset();
            this.chargeSolde();
            return this.Direct();
        }
        public void Reset()
        {
            this.GetCmp<FormPanel>("GeneralForm").Reset(true);
            this.PartialReset();
        }
        public void PartialReset()
        {
            this.GetCmp<Store>("ProductSaleHistoryGridStoreID").Reload();
        }
        [HttpPost]
        public StoreResult GetAllPendingProductSaleHistorys(DateTime Bdate, DateTime Edate/*, ProductLocalization pl*/)
        {

            Session["BeginDate"] = Bdate;
            Session["EndDate"] = Edate;
            ProductLocalization pl = (ProductLocalization)Session["ProductLocalization"]; // = pl;
            return this.Store(ModelRptProductSale(Bdate, Edate,pl));
        }

        private void chargeSolde()
        {
            double Totaldebit = 0d;
            double TotalQty = 0d;

            DateTime Bdate = (DateTime)Session["BeginDate"];
            DateTime Edate = (DateTime)Session["EndDate"];
            ProductLocalization pl = (ProductLocalization)Session["ProductLocalization"];

            SaleE c = new SaleE();

            //List<SaleE> lstCustoSaleAmt = _saleRepository.FindAll.Where(so => (so.SaleDate.Date >= Bdate.Date && so.SaleDate.Date <= Edate)).ToList();

            var lstCustoSaleAmt = db.Sales.Join(db.SaleLines, s => s.SaleID, sl => sl.SaleID, (s, sl) => new { s, sl })
                .Where(lsp => lsp.sl.ProductID == pl.ProductID && lsp.sl.LocalizationID==pl.LocalizationID && (lsp.s.SaleDate >= Bdate.Date && lsp.s.SaleDate <= Edate.Date))
                .Select(s => new
                {
                    SaleDate = s.s.SaleDate,
                    Sale = s.s,
                    SaleLine = s.sl
                }).ToList();

            foreach (var getSalePeriode in lstCustoSaleAmt.OrderBy(s => s.SaleDate))
            {
                c = _customerReturnRepository.GetRealSale(getSalePeriode.Sale);
                double CustomerOrderTotalPrice = Util.ExtraPrices(getSalePeriode.SaleLine.LineUnitPrice * getSalePeriode.SaleLine.LineQuantity,
                                                                      c.RateReduction, c.RateDiscount, c.Transport, c.VatRate).TotalTTC;

                Totaldebit = Totaldebit + CustomerOrderTotalPrice;
                TotalQty = TotalQty + getSalePeriode.SaleLine.LineQuantity;
            }

            this.GetCmp<TextField>("TotalQty").Value = TotalQty;
            this.GetCmp<TextField>("TotalDebit").Value = Totaldebit;

        }

        private List<object> ModelRptProductSale(DateTime Bdate, DateTime Edate,ProductLocalization pl) 
        {
            double advancedAmount = 0d;
            List<object> list = new List<object>();

            if (Bdate == new DateTime(1900, 01, 01) || Edate == new DateTime(1900, 01, 01) || pl == new ProductLocalization())
            {
                return list;
            }
            SaleE c = new SaleE();

            var lstCustoSaleAmt = db.Sales.Join(db.SaleLines, s => s.SaleID, sl => sl.SaleID, (s, sl) => new { s, sl })
                  .Where(lsp => lsp.sl.ProductID == pl.ProductID && lsp.sl.LocalizationID == pl.LocalizationID && (lsp.s.SaleDate >= Bdate.Date && lsp.s.SaleDate <= Edate.Date))
                  .Select(s => new
                  {
                      SaleDate = s.s.SaleDate,
                      Sale = s.s,
                      SaleLine = s.sl
                  }).ToList();

            foreach (var getSalePeriode in lstCustoSaleAmt.OrderBy(s => s.SaleDate))
                {
                    SaleLine salLine = getSalePeriode.SaleLine;
                    
                    c = _customerReturnRepository.GetRealSale(getSalePeriode.Sale);
                    
                    double CustomerOrderTotalPrice = Util.ExtraPrices((salLine.LineQuantity*salLine.LineUnitPrice),
                                                                          c.RateReduction, c.RateDiscount, c.Transport, c.VatRate).TotalTTC;

                    if (CustomerOrderTotalPrice > 0/* ||*/)
                    {
                    User postedby =  db.Users.Find(c.PostByID.Value) ;
                    User validatedBy = (c.OperatorID != null) ? db.Users.Find(c.OperatorID.Value) : null;

                    
                    //recuperation du detail de la vente
                    //List<SaleLine> lscustorder = db.SaleLines.Where(co => co.SaleID == c.SaleID).ToList();
                    //{
                    //foreach (SaleLine o in lscustorder)
                        list.Add(
                        new
                        {
                            RptGeneSaleID = c.CustomerOrderID,
                            CustomerOrderDate = c.SaleDate,
                            CustomerOrderTotalPrice = CustomerOrderTotalPrice,
                            CustomerOrderNumber = c.SaleReceiptNumber,
                            CustomerName = c.PersonName,
                            Agence = c.Branch.BranchCode,
                            LibAgence = c.Branch.BranchDescription,
                            Devise = c.Devise.DeviseCode,
                            LibDevise = c.Devise.DeviseLabel,
                            CodeClient = c.Customer.Name.Trim(),
                            NomClient = c.Customer.Name ,// c.Customer.Name,
                            OrderStatut = c.StatutSale.ToString(),
                            Code = c.SaleID,
                            LineQuantity = salLine.LineQuantity,
                            AdvancedAmount = salLine.LineQuantity,
                            ProductID = salLine.ProductID,
                            ProductCode = salLine.Product.ProductCode,
                            PostByID = (postedby == null) ? "" : postedby.Name,
                            OperatorID = (validatedBy == null) ? "" : validatedBy.Name
                        }
                    );
                }
            }
            return list;
        }

        public ActionResult OpenedBusday()
        {
            List<object> list = new List<object>();
            lstBusDay = (List<BusinessDay>)Session["UserBusDays"];
            if (lstBusDay == null)
            {
                lstBusDay = _busDayRepo.GetOpenedBusinessDay(CurrentUser);
            };

            foreach (BusinessDay busDay in lstBusDay)
            {
                list.Add(
                    new
                    {
                        BranchID = busDay.BranchID,
                        BranchName = busDay.Branch.BranchCode
                    }
                    );
            }

            return this.Store(list);

        }
        public ActionResult ChangeBusDay(int? BranchID)
        {
            
            return this.Direct();
        }


        /// <summary>
        /// Liste des magasins qui sont approvisionnés dans une branche
        /// </summary>
        /// <param name="BranchID"></param>
        /// <returns></returns>
        public StoreResult GetAllStockedLocations(int BranchID)
        {
            List<object> list = new List<object>();
            
            if (BranchID >0)
            {
                List<Localization> dataTmp = (from loc in db.Localizations
                                              where (loc.BranchID == BranchID)
                                              select loc).ToList();
                if (dataTmp.Count>0)
                {
                    foreach (Localization pt in dataTmp)
                    {
                        list.Add(new
                        {
                            LocalizationID = pt.LocalizationID,
                            LocalizationLabel = pt.LocalizationLabel,
                        });
                    }
                }
                
            }
           
            return this.Store(list);
        }



        public ActionResult DisableNumero(int ProductCategoryID)
        {
            Category catprod = db.Categories.Find(ProductCategoryID);
            if (catprod is LensCategory)
            {
                this.GetCmp<ComboBox>("ProductNumberID").Disabled = false;
                this.GetCmp<ComboBox>("ProductID").HideTrigger = true;

            }
            else
            {
                this.GetCmp<ComboBox>("ProductNumberID").Disabled = true;
                this.GetCmp<ComboBox>("ProductID").HideTrigger = false;

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
                numbers.RemoveAll(number => !number.LensNumberFullCode.ToLower().StartsWith(filter.ToLower()));
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
                    List<LensNumber> lens = new List<LensNumber>();

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
                    this.GetCmp<ComboBox>("ProductID").SetValueAndFireSelect(GetLensID(Localization.Value, ProductCategoryID.Value, ProductNumberID.Value));
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

        

        public int GetLensID(int LocalizationID, int ProductCategoryID, int ProductNumberID)
        {
            
            var lstLensProduct = db.Lenses.Join(db.Products, ls => ls.ProductID, p => p.ProductID,
                        (ls, p) => new { ls, p }).
                        Join(db.ProductLocalizations, pr => pr.p.ProductID, pl => pl.ProductID, (pr, pl) => new { pr, pl })
                        .Where(lsp => lsp.pl.LocalizationID == LocalizationID
                        && lsp.pr.ls.LensNumberID == ProductNumberID && lsp.pr.p.CategoryID == ProductCategoryID)
                        .Select(s => new
                        {
                            ProductID = s.pr.p.ProductID,
                            ProductCode = s.pr.p.ProductCode,
                            ProductLabel = s.pr.p.ProductLabel,
                            ProductQuantity = s.pl.ProductLocalizationStockQuantity,
                            ProductLocalizationStockSellingPrice = s.pl.ProductLocalizationStockSellingPrice ,
                            //ProductLocalization = s.pl
                        }).FirstOrDefault();

            Session["MaxValue"] = lstLensProduct.ProductQuantity;
            Session["LineUnitPrice"] = lstLensProduct.ProductLocalizationStockSellingPrice;
            //Session["ProductLocalization"] = lstLensProduct.ProductLocalization;

            return lstLensProduct.ProductID;

        }

        [HttpPost]
        public ActionResult OnProductSelected(int? Localization, int? CurrentProduct)
        {

            try
            {

                if ((!Localization.HasValue || Localization.Value <= 0) || (!CurrentProduct.HasValue || CurrentProduct.Value <= 0)) { return this.Direct(); }

                ProductLocalization prodLoc = db.ProductLocalizations.Where(pl => pl.ProductID == CurrentProduct.Value && pl.LocalizationID == Localization.Value).FirstOrDefault();
                Session["ProductLocalization"] = prodLoc;

                /*
                Product product = (from prod in db.Products
                                   where prod.ProductID == CurrentProduct.Value
                                   select prod).SingleOrDefault();
                bool productIsLens = product is Lens;

                if (productIsLens)
                {
                    double stockQuantity = (double)Session["MaxValue"];

                    this.GetCmp<NumberField>("StockQuantity").Value = stockQuantity;
                }
                else
                {

                    var prodLoc = db.ProductLocalizations.Where(pl => pl.ProductID == CurrentProduct.Value && pl.LocalizationID == Localization.Value)
                    .Select(p => new
                    {
                        ProductLocalizationStockQuantity = p.ProductLocalizationStockQuantity,
                        SellingPrice = p.Product.SellingPrice
                    }).SingleOrDefault();

                    Session["MaxValue"] = prodLoc.ProductLocalizationStockQuantity;
                    this.GetCmp<NumberField>("StockQuantity").Value = prodLoc.ProductLocalizationStockQuantity;

                    bool isUpdate = (bool)Session["isUpdate"];

                    if (!isUpdate)
                    {
                        Session["isUpdate"] = false;
                    }
                }    */

                return this.Direct();
            }
            catch (Exception e)
            {
                statusOperation = Resources.er_alert_danger + e.Message + "Source = " + e.Source + "Méthode = OnProductSelected(" + Localization.Value + " " + CurrentProduct.Value + ") " + e.TargetSite + " InnerException = " + e.InnerException;
                X.Msg.Alert(Resources.Productlabel, statusOperation).Show();
                return this.Direct();
            }
        }
        [HttpPost]
        public ActionResult GetAllProducts(int? LocalizationID, int? ProductCategoryID, int? ProductNumberID)
        {

            try
            {
                List<Product> list = ModelProductLocalCat(LocalizationID, ProductCategoryID, ProductNumberID);

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

        public List<Product> ModelProductLocalCat(int? LocalizationID, int? ProductCategoryID, int? ProductNumberID)
        {
            List<Product> model = new List<Product>();
            

            //On a un produit générique
            if ((LocalizationID == null || LocalizationID.Value == 0) && (ProductCategoryID == null || ProductCategoryID.Value == 0) && (ProductNumberID == null || ProductNumberID.Value == 0)) //chargement des produits en fct du magasin slt
            {
                return model;
                
            }
            else //On a un produit de type verre
            {
                // verifion si c'est un produit de type verre
                //LensCategory lenprod = db.LensCategories.Find(ProductCategoryID);
                                // ProductLocalization pl = db.ProductLocalizations.Where(p=>p.ProductID)
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
                            .Where(lsp => lsp.pl.LocalizationID == LocalizationID
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

                    ////produit generic

                    var lstLensProduct = db.Products.Join(db.ProductLocalizations, p => p.ProductID, pl => pl.ProductID,
                        (p, pl) => new { p, pl })
                        .Where(lsp => lsp.pl.LocalizationID == LocalizationID && lsp.p.CategoryID == ProductCategoryID && !(lsp.p.Category is LensCategory))
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
            }

            return model;
        }



        private Company cmpny
		{
			get
			{
				return db.Companies.FirstOrDefault();
			}
		}

        //This method load a method that print a receip of deposit
        public ActionResult PrintReport()
        {
            this.GetCmp<Panel>("PanelReport").LoadContent(new ComponentLoader
            {
                Url = Url.Action("ShowGeneric"),
                DisableCaching = false,
                Mode = LoadMode.Frame,
            });

            return this.Direct();
        }
        /// This is used for showing Generic Report(with data and report parameter) in a same window       
        public ActionResult ShowGeneric()
        {
            // Clear all sessions value
            Session["ReportName"] = null;
            Session["CompanyName"] = null;
            Session["TelFax"] = null;

            Session["Adresse"] = null;

            Session["RepTitle"] = null;
            Session["Operator"] = null;


            this.Session["ReportName"] = "RptProductSaleHistory";
            if (cmpny != null)
            {
                this.Session["CompanyName"] = cmpny.Name;
                this.Session["TelFax"] = "Tel: " + cmpny.Adress.AdressPhoneNumber + " Fax : " + cmpny.Adress.AdressFax + " Town :" + cmpny.Adress.Quarter.Town.TownLabel;
                this.Session["Adresse"] = "PO BOX:" + cmpny.Adress.AdressPOBox + " " + cmpny.Adress.Quarter.Town.TownLabel;
            }
            else
            {
                this.Session["CompanyName"] = "NONE";
                this.Session["TelFax"] = "NONE";
            }
            this.Session["RepTitle"] = Resources.RptProductSaleHistory;
            this.Session["Operator"] = CurrentUser.Name;

            DateTime BeginDate = (DateTime)Session["BeginDate"];
            DateTime EndDate = (DateTime)Session["EndDate"];
            ProductLocalization pl = (ProductLocalization)Session["ProductLocalization"];

            this.Session["rptSource"] = ModelRptProductSale(BeginDate, EndDate,pl);

            return RedirectToAction("ShowGenericRpt", "ProductSaleHistory");
        }
        public void ShowGenericRpt()
        {
            ReportDocument rptH = new ReportDocument();
            try
            {
                bool isValid = true;

                string strReportName = Session["ReportName"].ToString();    // Setting ReportName
                string stCompanyName1 = Session["CompanyName"].ToString();     // Setting CompanyName1
                string strTelFax1 = Session["TelFax"].ToString();         // Setting TelFax1
                string strRepTitle1 = Session["RepTitle"].ToString();         // Setting RepTitle1
                string strOperator1 = Session["Operator"].ToString();         // Setting Operator1
                string strAdresse1 = Session["Adresse"].ToString();
                DateTime BeginDate = (DateTime)Session["BeginDate"];
                DateTime EndDate = (DateTime)Session["EndDate"];

                var rptSource = Session["rptSource"];

                if (string.IsNullOrEmpty(strReportName) && rptSource == null)
                {
                    isValid = false;
                }

                if (isValid)
                {

                    string strRptPath = System.Web.HttpContext.Current.Server.MapPath("~/") + "Reports//Supply//" + strReportName + ".rpt";
                    rptH.Load(strRptPath);
                    if (rptSource != null && rptSource.GetType().ToString() != "System.String") rptH.SetDataSource(rptSource);
                    if (!string.IsNullOrEmpty(stCompanyName1)) rptH.SetParameterValue("CompanyName", stCompanyName1);
                    if (!string.IsNullOrEmpty(strTelFax1)) rptH.SetParameterValue("TelFax", strTelFax1);
                    if (!string.IsNullOrEmpty(strRepTitle1)) rptH.SetParameterValue("RepTitle", strRepTitle1);
                    if (!string.IsNullOrEmpty(strOperator1)) rptH.SetParameterValue("Operator", strOperator1);

                    if (!string.IsNullOrEmpty(strAdresse1)) rptH.SetParameterValue("Adresse", strAdresse1);

                    if (BeginDate != null) rptH.SetParameterValue("BeginDate", BeginDate);
                    if (EndDate != null) rptH.SetParameterValue("EndDate", EndDate);
                    bool isDownloadRpt = (bool)Session["isDownloadRpt"];
                    rptH.ExportToHttpResponse(ExportFormatType.PortableDocFormat, System.Web.HttpContext.Current.Response, false, strReportName);

                    // Clear all sessions value
                    Session["ReportName"] = null;
                    Session["CompanyName"] = null;
                    Session["TelFax"] = null;
                    Session["RepTitle"] = null;
                    Session["Operator"] = null;
                    Session["accop"] = null;
                    Session["Adresse"] = null;
                    Session["BeginDate"] = null;
                    Session["EndDate"] = null;
                }
                else
                {
                    Response.Write("Nothing Found; No Report name found");
                }
            }
            catch (Exception ex)
            {
                Response.Write(ex.ToString());
            }
            finally
            {
                rptH.Close();
                rptH.Dispose();
            }
        }


    }
}