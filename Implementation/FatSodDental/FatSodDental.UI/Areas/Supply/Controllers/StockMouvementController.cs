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
    public partial class StockMouvementController : BaseController
    {
        private bool isLens = false;
        //Current Controller and current page
        private const string CONTROLLER_NAME = "Supply/StockMouvement" ;
        private const string VIEW_NAME = "Index";
        private IBusinessDay _busDayRepo;
        private IProductLocalization _productLocRepository;
        private IRepository<FatSod.Security.Entities.File> _fileRepository;

        //Construcitor

        private List<BusinessDay> lstBusDay;
        public StockMouvementController( 
            IBusinessDay bDRepo,
            IRepository<FatSod.Security.Entities.File> fileRepository,
            IProductLocalization productLocRepo
            )
        {
            this._busDayRepo = bDRepo;
            this._productLocRepository = productLocRepo;
            this._fileRepository = fileRepository;
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

        }

        public ActionResult DisplayEntries(DateTime BeginDate, DateTime EndDate)
        {
            Session["BeginDate"] = BeginDate;
            Session["EndDate"] = EndDate;
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
            this.GetCmp<Store>("StockMouvementGridStoreID").Reload();
        }
        [HttpPost]
        public StoreResult GetAllPendingStockMouvements(DateTime Bdate, DateTime Edate)
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

            var invHistory = db.InventoryHistorics.Where(ih => ih.InventoryDate <= Edate.Date && ih.ProductID == pl.ProductID && ih.LocalizationID == pl.LocalizationID).OrderByDescending(s => s.InventoryHistoricID).Take(1);
           foreach(var getHist in invHistory)
           {
               TotalQty = TotalQty + getHist.NewStockQuantity;
           }
            
            this.GetCmp<TextField>("TotalQty").Value = TotalQty;

        }
        private double TotalProductQtyByDate(DateTime OpDate, ProductLocalization pl)
        {
            double TotalQty = 0d;

            var invHistory = db.InventoryHistorics.Where(ih => ih.InventoryDate <= OpDate.Date && ih.ProductID == pl.ProductID && ih.LocalizationID == pl.LocalizationID).OrderByDescending(s => s.InventoryHistoricID).Take(1);
            foreach (var getHist in invHistory)
            {
                TotalQty = TotalQty + getHist.NewStockQuantity;
            }

            return  TotalQty;

        }

        private double TotalOutPutProductQtyByDate(DateTime OpDate, ProductLocalization pl)
        {
            double TotalQty = 0d;

            var invHistory = db.InventoryHistorics.Where(ih => ih.InventoryDate <= OpDate.Date && ih.ProductID == pl.ProductID && ih.LocalizationID == pl.LocalizationID && ih.StockStatus.Trim().ToUpper() == "OUTPUT").OrderByDescending(s => s.InventoryHistoricID).ToList();
            foreach (var getHist in invHistory)
            {
                TotalQty = TotalQty + getHist.Quantity;
            }

            return TotalQty;

        }

        private double TotalInPutProductQtyByDate(DateTime OpDate, ProductLocalization pl)
        {
            double TotalStockReg = 0d;
            double TotalQty = 0d;
            //recuperation du stock de depart
            var invHistoryStockReg = db.InventoryHistorics.Where(ih => ih.InventoryDate <= OpDate.Date && ih.ProductID == pl.ProductID && ih.LocalizationID == pl.LocalizationID && ih.inventoryReason.Trim().ToLower() == "stock regularization").OrderByDescending(s => s.InventoryHistoricID).Take(1);
            foreach (var getStockReg in invHistoryStockReg)
            {
                TotalStockReg = TotalStockReg + getStockReg.NewStockQuantity;
            }

            var invHistory = db.InventoryHistorics.Where(ih => ih.InventoryDate <= OpDate.Date && ih.ProductID == pl.ProductID && ih.LocalizationID == pl.LocalizationID && ih.StockStatus.Trim().ToUpper() == "INPUT").ToList();
            foreach (var getHist in invHistory)
            {
                TotalQty = TotalQty + getHist.Quantity;
            }

            return TotalStockReg+TotalQty;

        }
        public ActionResult ChangeBusDay(int? BranchID)
        {

            return this.Direct();
        }
        private List<object> ModelRptProductSale(DateTime Bdate, DateTime Edate,ProductLocalization pl) 
        {
            double advancedAmount = 0d;
            List<object> list = new List<object>();

            if (Bdate == new DateTime(1900, 01, 01) || Edate == new DateTime(1900, 01, 01) || pl == new ProductLocalization())
            {
                return list;
            }

            var LstinvHistory = db.InventoryHistorics.Where(ih => ih.InventoryDate >= Bdate.Date && ih.InventoryDate <= Edate.Date && ih.ProductID == pl.ProductID && ih.LocalizationID == pl.LocalizationID).ToList().OrderBy(s => s.InventoryHistoricID);

            foreach (var c in LstinvHistory)
            {
                    list.Add(
                    new
                    {
                        RptGeneSaleID = c.InventoryHistoricID,
                        CustomerOrderDate = c.InventoryDate,
                        CustomerOrderNumber = c.Quantity,
                        CustomerName = c.inventoryReason,
                        CodeClient = c.StockStatus,
                        NomClient = c.Description,
                        Code = c.InventoryHistoricID,
                        LineQuantity = c.NewStockQuantity,
                        CustomerOrderTotalPrice = c.NEwStockUnitPrice,
                        ProductID = c.ProductID,
                        ProductCode = c.Product.ProductCode,
                        PostByID = c.AutorizedBy.Name,
                        OperatorID = c.RegisteredBy.Name
                    }
                );
            }
            return list;
        }

        private List<Object> ModelHistoProduct(DateTime BeginDate, DateTime EndDate, ProductLocalization pl)
        {
            double advancedAmount = 0d;
            List<object> list = new List<object>();

            List<BusinessDay> UserBusDays = (List<BusinessDay>)Session["UserBusDays"];

            if (BeginDate == new DateTime(1900, 01, 01) || EndDate == new DateTime(1900, 01, 01) || pl == new ProductLocalization())
            {
                return list;
            }

            string defaultImgPath = Server.MapPath("~/Content/Images/App/deboy-logo.png");
            byte[] COMPANY_LOGO = System.IO.File.ReadAllBytes(defaultImgPath);

            Branch currentBranch = UserBusDays.FirstOrDefault().Branch;
            Devise currentDevise = db.Devises.Where(d=>d.DefaultDevise).FirstOrDefault();


            //double totalQtyBefore = TotalProductQtyByDate(BeginDate,pl);
            double RepOutPut = TotalOutPutProductQtyByDate(BeginDate, pl);
            double RepInPut = TotalInPutProductQtyByDate(BeginDate, pl);
            
            //traitement historique pour la periode choisie
            var LstinvHistory = db.InventoryHistorics.Where(ih => ih.InventoryDate >= BeginDate.Date && ih.InventoryDate <= EndDate.Date && ih.ProductID == pl.ProductID && ih.LocalizationID == pl.LocalizationID).ToList().OrderBy(s => s.InventoryHistoricID);
            foreach (var invHist in LstinvHistory)
            {
                list.Add(
                    new 
                    {
                        RptPrintStockMvtID=invHist.InventoryHistoricID,
                        Agence=currentBranch.BranchCode,
                        LibAgence=currentBranch.BranchName,
                        LocalizationName=invHist.Localization.LocalizationCode,
                        ProductName=invHist.Product.ProductCode,
                        Devise=currentDevise.DeviseCode,
                        LibDevise=currentDevise.DeviseLabel,
                        EndDate=EndDate.Date,
                        BeginDate = BeginDate.Date,
                        DateOperation=invHist.InventoryDate,
                        RefOperation=invHist.inventoryReason,
                        Description = (invHist.Description == null) ? invHist.inventoryReason : invHist.Description,
                        RepOutPut = RepOutPut,
                        RepInPut = RepInPut,
                        Solde = invHist.NewStockQuantity,
                        QteOutPut=(invHist.StockStatus.ToUpper()=="OUTPUT") ? invHist.Quantity:0,
                        QteInput=(invHist.StockStatus.ToUpper()=="INPUT") ? invHist.Quantity:0,
                        Sens=invHist.StockStatus.ToUpper(),
                        LogoBranch = _fileRepository.FindAll.FirstOrDefault(f => f.GlobalPersonID == cmpny.GlobalPersonID) != null ? _fileRepository.FindAll.FirstOrDefault(f => f.GlobalPersonID == cmpny.GlobalPersonID).Content : COMPANY_LOGO,
                        ProductID=invHist.ProductID,
                        LocalizationID = invHist.LocalizationID
                    }
                    );
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
            Session["RegionCountry"] = null;
            Session["RepTitle"] = null;
            Session["Operator"] = null;


            this.Session["ReportName"] = "RptStockMouvement";
            if (cmpny != null)
            {
                this.Session["CompanyName"] = cmpny.Name;
                this.Session["TelFax"] = "Tel: " + cmpny.Adress.AdressPhoneNumber + " Fax : " + cmpny.Adress.AdressFax + " Town :" + cmpny.Adress.Quarter.Town.TownLabel;
                this.Session["Adresse"] = "PO BOX:" + cmpny.Adress.AdressPOBox + " " + cmpny.Adress.Quarter.Town.TownLabel;
                this.Session["RegionCountry"]= cmpny.Adress.Quarter.Town.Region.RegionLabel;
            }
            else
            {
                this.Session["CompanyName"] = "NONE";
                this.Session["TelFax"] = "NONE";
            }
            this.Session["RepTitle"] = Resources.RptStockMouvement;
            this.Session["Operator"] = CurrentUser.Name;

            DateTime BeginDate = (DateTime)Session["BeginDate"];
            DateTime EndDate = (DateTime)Session["EndDate"];
            ProductLocalization pl = (ProductLocalization)Session["ProductLocalization"];

            this.Session["rptSource"] = ModelHistoProduct(BeginDate, EndDate, pl);

            return RedirectToAction("ShowGenericRpt", "StockMouvement");
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
                string strRegionCountry1 = Session["RegionCountry"].ToString();

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
                    //string strRptPath = Server.MapPath("~/Reports/Supply/RptStockMouvement.rpt");
                    rptH.Load(strRptPath);
                    if (rptSource != null && rptSource.GetType().ToString() != "System.String") rptH.SetDataSource(rptSource);
                    if (!string.IsNullOrEmpty(stCompanyName1)) rptH.SetParameterValue("CompanyName", stCompanyName1);
                    if (!string.IsNullOrEmpty(strTelFax1)) rptH.SetParameterValue("TelFax", strTelFax1);
                    if (!string.IsNullOrEmpty(strRepTitle1)) rptH.SetParameterValue("RepTitle", strRepTitle1);
                    if (!string.IsNullOrEmpty(strOperator1)) rptH.SetParameterValue("Operator", strOperator1);
                    if (!string.IsNullOrEmpty(strRegionCountry1)) rptH.SetParameterValue("RegionCountry", strRegionCountry1);
                    if (!string.IsNullOrEmpty(strAdresse1)) rptH.SetParameterValue("Adresse", strAdresse1);

                    if (BeginDate != null) rptH.SetParameterValue("BeginDate", BeginDate);
                    if (EndDate != null) rptH.SetParameterValue("EndDate", EndDate);
                    
                    bool isDownloadRpt = (bool)Session["isDownloadRpt"];
                    rptH.ExportToHttpResponse(ExportFormatType.PortableDocFormat, System.Web.HttpContext.Current.Response, false, "RptStockMouvement");

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
                    Session["RegionCountry"] = null;
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