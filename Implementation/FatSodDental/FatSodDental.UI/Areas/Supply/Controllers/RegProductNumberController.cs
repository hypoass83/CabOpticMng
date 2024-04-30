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
//using SaleE = FatSod.Supply.Entities.Sale;
using FatSod.Supply.Abstracts;
using FatSod.Ressources;
using FatSodDental.UI.Filters;
using System.Collections;
using FatSod.DataContext.Concrete;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;


namespace FatSodDental.UI.Areas.Supply.Controllers
{
    [Authorize(Order = 1)]
    [TakeBusinessDay(Order = 2)]
    public partial class RegProductNumberController : BaseController
    {
        private bool isLens = false;
        //Current Controller and current page
        private const string CONTROLLER_NAME = "Supply/RegProductNumber" ;
        private const string VIEW_NAME = "Index";
        private IRegProductNumber _RegProductNumberRepository;
        private IBusinessDay _busDayRepo;
        private ITransactNumber _transactNumbeRepository;
        private IProductLocalization _productLocRepository;
        private ILensNumberRangePrice _priceRepository;
        //Construcitor

        private List<BusinessDay> lstBusDay;
        public RegProductNumberController(IRegProductNumber RegProductNumberRepository, 
            IBusinessDay bDRepo,
            ITransactNumber transactRepo,
            ILensNumberRangePrice priceRepo,
            IProductLocalization productLocRepo
            )
        {
            this._RegProductNumberRepository = RegProductNumberRepository;
            this._busDayRepo = bDRepo;
            this._transactNumbeRepository = transactRepo;
            this._priceRepository = priceRepo;
            this._productLocRepository = productLocRepo;
        }
        [OutputCache(Duration=43200)]
        public ActionResult Index()
        {
            Session["isUpdate"] = false;
            Session["OldLensNumberFullCode"] = "*";
            Session["NewLensNumberFullCode"] = "*";
            List<RegProductNumberLine> RegProductNumberLines = new List<RegProductNumberLine>();
            Session["RegProductNumberLines"] = RegProductNumberLines;
            lstBusDay = (List<BusinessDay>)Session["UserBusDays"];
            if (lstBusDay == null)
            {
                lstBusDay = _busDayRepo.GetOpenedBusinessDay(CurrentUser);
            };
            DateTime BDDateOperation = lstBusDay.FirstOrDefault().BDDateOperation;
            
            ViewBag.BusnessDayDate = BDDateOperation;
            //return rPVResult;
            return View(ModelPendingRegProductNumber(lstBusDay.FirstOrDefault().BranchID,BDDateOperation));
        }

        [HttpPost]
        public ActionResult AddManager(RegProductNumber RegProductNumber)
        {
            List<RegProductNumberLine> RegProductNumberLines = new List<RegProductNumberLine>();
            RegProductNumberLines = (List<RegProductNumberLine>)Session["RegProductNumberLines"];
            RegProductNumber.RegProductNumberLines = RegProductNumberLines;
            
            if (RegProductNumber.RegProductNumberID == 0)
            {
                return DoRegProductNumber(RegProductNumber);
            }

            if (RegProductNumber.RegProductNumberID > 0)
            {
                X.Msg.Alert(Resources.er_alert_danger, "This operation not yet possible ").Show();
                return this.Direct();
                //return UpdateRegProductNumber(RegProductNumber);
            }
            
            X.Msg.Alert(Resources.er_alert_danger, "RegProductNumberID can not be negative ").Show();
            return this.Direct();

        }

        public ActionResult InitTrnNumber(int? BranchID)
        {
            if (BranchID.HasValue && BranchID.Value > 0)
            {
                lstBusDay = (List<BusinessDay>)Session["UserBusDays"];
                if (lstBusDay == null)
                {
                    lstBusDay = _busDayRepo.GetOpenedBusinessDay(CurrentUser);
                };
                BusinessDay businessDay = lstBusDay.FirstOrDefault(l => l.BranchID == BranchID.Value);
                string trnnum = _transactNumbeRepository.displayTransactNumber("CPNU", businessDay);
                this.GetCmp<TextField>("RegProductNumberReference").Value = trnnum;
                this.GetCmp<TextField>("RegisteredByID").Value = SessionGlobalPersonID;
            }
            return this.Direct();
        }

        [HttpPost]
        public ActionResult DoRegProductNumber(RegProductNumber RegProductNumber)
        {
            try
            {

                int RegProductNumberID= _RegProductNumberRepository.DoRegProductNumber(RegProductNumber,SessionGlobalPersonID).RegProductNumberID;
                Session["RegProductNumberID"] = RegProductNumberID;
                this.GetCmp<Button>("btnPrint").Disabled=false;
                this.AlertSucces(Resources.Success, "Products Have Been Successfully Sended ");
                return this.Reset();
            }
            catch (Exception ex)
            {

                X.Msg.Alert(Resources.er_alert_danger, "Products Have not Been Successfully Send because " + ex.Message + " " + ex.StackTrace).Show();
                return this.Direct();
            }
        }

        [HttpPost]
        public ActionResult UpdateRegProductNumber(RegProductNumber RegProductNumber)
        {
            try
            {
                int RegProductNumberID= _RegProductNumberRepository.UpdateRegProductNumber(RegProductNumber).RegProductNumberID;
                Session["RegProductNumberID"] = RegProductNumberID;
                this.GetCmp<Button>("btnPrint").Disabled = false;
                this.AlertSucces(Resources.Success, "Products Have Been Successfully Updated ");
                
                return this.Reset();
            }
            catch (Exception ex)
            {

                X.Msg.Alert(Resources.er_alert_danger, "Products Have Been not Successfully Updated because " + ex.Message + " " + ex.StackTrace).Show();
                return this.Direct();
            }
        }

        [HttpPost]
        public ActionResult AddRegProductNumberLine(RegProductNumberLine RegProductNumberLine)
        {
            try 
            { 
            RegProductNumberLine.OldProduct = db.Products.Find(RegProductNumberLine.OldProductID);
            RegProductNumberLine.Localization = db.Localizations.Find(RegProductNumberLine.LocalizationID);
            RegProductNumberLine.NewProduct = db.Products.Find(RegProductNumberLine.NewProductID);

            List<RegProductNumberLine> RegProductNumberLines = (List<RegProductNumberLine>)Session["RegProductNumberLines"];

            //il s'agit d'une modification alors on fait un drop and create
            if (RegProductNumberLine.TMPID > 0)
            {
                RegProductNumberLine toRemove = RegProductNumberLines.SingleOrDefault(pl => pl.TMPID == RegProductNumberLine.TMPID);
                RegProductNumberLine.TMPID = 0;
                RegProductNumberLines.Remove(toRemove);
            }

            //alors la variable de session n'était pas vide
            if (RegProductNumberLines != null && RegProductNumberLines.Count > 0)
            {
                //c'est un nouvel ajout dans le panier
                if (RegProductNumberLine.TMPID == 0)
                {
                    RegProductNumberLine existing = RegProductNumberLines.SingleOrDefault(pl => pl.OldProductID == RegProductNumberLine.OldProductID && pl.NewProductID == RegProductNumberLine.NewProductID &&
                                                                                                    pl.LocalizationID == RegProductNumberLine.LocalizationID 
                                                                                                );
                    //existe t-il déjà une ligne de vente ayant le meme produit et le même magasin que celui en création?
                    if (/*RegProductNumberLines.Contains(RegProductNumberLine) && RegProductNumberLine.TMPID > 0*/
                        existing != null && existing.TMPID > 0) //cette ligne exixte déjà
                    {
                        //la quantité est la somme des deux quantité
                        RegProductNumberLine.NewLineQuantity += existing.NewLineQuantity;
                        //l'id c'est l'id de la ligne existante
                        RegProductNumberLine.TMPID = existing.TMPID;
                        RegProductNumberLine.RegProductNumberLineID = existing.RegProductNumberLineID;
                        //on retire l'ancien pour ajouter le nouveau
                        RegProductNumberLines.Remove(existing);
                    }

                    if (existing == null || existing.TMPID == 0)//La ligne n'existe pas encore dans le panier
                    {
                        RegProductNumberLine.TMPID = RegProductNumberLines.Select(pl => pl.TMPID).Max() + 1;
                    }
                }
            }

            //alors la variable de session était vide
            if (RegProductNumberLines == null || RegProductNumberLines.Count == 0)
            {
                //c'est une nouvelle création pour la création
                RegProductNumberLines = new List<RegProductNumberLine>();
                RegProductNumberLine.TMPID = 1;
            }
            //verifiction ds le stock source s'il ya assez de produit a RegProductNumberer
            bool res = _productLocRepository.checkQtyInStock(RegProductNumberLine.OldProductID, RegProductNumberLine.LocalizationID, RegProductNumberLine.NewLineQuantity);
            if (res)
            {
                RegProductNumberLines.Add(RegProductNumberLine);
                Session["RegProductNumberLines"] = RegProductNumberLines;
            }
            return this.Reset2();
            }
            catch (Exception ex)
            {

                X.Msg.Alert(Resources.er_alert_danger, "Products Have Been not Successfully Updated because " + ex.Message ).Show();
                return this.Direct();
            }
        }

        [HttpPost]
        public ActionResult RemoveRegProductNumberLine(int TMPID)
        {
            List<RegProductNumberLine> RegProductNumberLines = (List<RegProductNumberLine>)Session["RegProductNumberLines"];

            if (RegProductNumberLines != null && RegProductNumberLines.Count > 0)
            {
                RegProductNumberLine toRemove = RegProductNumberLines.SingleOrDefault(pl => pl.TMPID == TMPID);
                RegProductNumberLines.Remove(toRemove);

                Session["RegProductNumberLines"] = RegProductNumberLines;
            }

            return this.Reset2();
        }

        [HttpPost]
        public ActionResult Reset2()
        {
            SimpleReset2();
            return this.Direct();
        }

        [HttpPost]
        public ActionResult UpdateRegProductNumberLine(int TMPID)
        {
            this.InitializeRegProductNumberLineFields(TMPID);
            return this.Direct();
        }

        private void SimpleReset2()
        {

            this.GetCmp<ComboBox>("OldProductNumberID").Value = "";
            this.GetCmp<ComboBox>("NewProductNumberID").Value = "";
            this.GetCmp<ComboBox>("OldProductID").Value = "";
            this.GetCmp<ComboBox>("NewProductID").Value = "";
            this.GetCmp<NumberField>("StockQuantity").Value = "";
            this.GetCmp<NumberField>("NewLineQuantity").Value = "";
           
            this.GetCmp<Store>("RegProductNumberLinesStore").Reload();

            ManageCady();
        }

        //Return salelines of current sale
        [HttpPost]
        public ActionResult RegProductNumberLines()
        {
            List<RegProductNumberLine> RegProductNumberLines = (List<RegProductNumberLine>)Session["RegProductNumberLines"];
            List<object> list = new List<object>();

            foreach (RegProductNumberLine pl in RegProductNumberLines)
            {
                list.Add(
                    new
                    {
                        TMPID = pl.TMPID,
                        OldProductLabel = (pl.OldProduct is Lens) ? pl.OldProduct.ProductCode : pl.OldProduct.ProductLabel,
                        NewProductLabel = (pl.NewProduct is Lens) ? pl.NewProduct.ProductCode : pl.NewProduct.ProductLabel,
                        Localization = pl.Localization.LocalizationLabel,
                        NewLineQuantity = pl.NewLineQuantity
                    }
                    );
            }
            return this.Store(list);
        }

        [HttpGet]
        public ActionResult UpdateRegProductNumber(int RegProductNumberID)
        {
            this.InitializeRegProductNumberFields(RegProductNumberID);
            List<RegProductNumberLine> data = db.RegProductNumberLines.Where(ptl => ptl.RegProductNumberID == RegProductNumberID).ToList();
            List<RegProductNumberLine> RegProductNumberLines = new List<RegProductNumberLine>();
            int i = 0;
            foreach (RegProductNumberLine pl in data)
            {
                pl.TMPID = ++i;
                RegProductNumberLines.Add(pl);
            }
            Session["RegProductNumberID"] = RegProductNumberID;
            this.GetCmp<Button>("btnPrint").Disabled = false;
            Session["RegProductNumberLines"] = RegProductNumberLines;
            this.SimpleReset2();

            return this.Direct();
        }
        [HttpPost]
        public ActionResult Reset()
        {

            this.SimpleReset();
            this.SimpleReset2();
            return this.Direct();
        }

        private void SimpleReset()
        {
            List<RegProductNumberLine> RegProductNumberLines = new List<RegProductNumberLine>();
            Session["RegProductNumberLines"] = RegProductNumberLines;
            this.GetCmp<FormPanel>("GeneralForm").Reset(true);
            this.GetCmp<Store>("RegProductNumberGridStoreID").Reload();

        }


        public void InitializeRegProductNumberFields(int ID)
        {
            this.SimpleReset();

            if (ID == 0)
            {
                return;
            }

            RegProductNumber RegProductNumber = db.RegProductNumbers.Find(ID);
            this.GetCmp<TextField>("RegProductNumberID").Value = RegProductNumber.RegProductNumberID;

            this.GetCmp<ComboBox>("BranchID").Value = RegProductNumber.BranchID;
            this.GetCmp<ComboBox>("AutorizedByID").Value = RegProductNumber.AutorizedByID;
            this.GetCmp<TextField>("RegisteredByID").Value = RegProductNumber.RegisteredByID;

            this.GetCmp<TextField>("RegProductNumberReference").Value = RegProductNumber.RegProductNumberReference;
            this.GetCmp<DateField>("RegProductNumberDate").Value = RegProductNumber.RegProductNumberDate;

            this.ManageCady();

        }
        [HttpPost]
        public ActionResult DeleteRegProductNumber(int RegProductNumberID)
        {
            try
            {
                _RegProductNumberRepository.CancelRegProductNumber(RegProductNumberID);
                this.AlertSucces(Resources.Success, "product Number regularization Has Been Successfully Deleted ");
                return this.Reset();

            }
            catch (Exception ex)
            {
                X.Msg.Alert(Resources.er_alert_danger, "Product Has Not Been Successfully Deleted Because " + ex.Message + " " + ex.StackTrace).Show();
                return this.Direct();
            }

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
            this.GetCmp<DateField>("RegProductNumberDate").Reset();

            if (BranchID.HasValue && BranchID.Value > 0)
            {
                lstBusDay = (List<BusinessDay>)Session["UserBusDays"];
                if (lstBusDay == null)
                {
                    lstBusDay = _busDayRepo.GetOpenedBusinessDay(CurrentUser);
                };
                BusinessDay businessDay = lstBusDay.FirstOrDefault(l => l.BranchID == BranchID.Value);
                this.GetCmp<DateField>("RegProductNumberDate").SetValue(businessDay.BDDateOperation);

                this.InitTrnNumber(BranchID.Value);

            }
            return this.Direct();
        }

        public double GetMaxValue(int? ProductID, int? LocalizationID, int? TMPID)
        {
            double maxvalue = 0;
            if ((LocalizationID.HasValue && ProductID.HasValue) && (LocalizationID.Value > 0 && ProductID.Value > 0))
            {
                ProductLocalization prodLoc = db.ProductLocalizations.SingleOrDefault(pl => pl.ProductID == ProductID.Value && pl.LocalizationID == LocalizationID.Value);
                maxvalue = (prodLoc != null && prodLoc.ProductLocalizationID > 0) ? prodLoc.ProductLocalizationStockQuantity : 0;

                //enlevons les quantités déjà prise pour faire d'autres lignes de RegProductNumbert
                List<RegProductNumberLine> RegProductNumberLines = (List<RegProductNumberLine>)Session["RegProductNumberLines"];
                if (RegProductNumberLines != null && RegProductNumberLines.Count > 0)
                {
                    double existing = RegProductNumberLines.Where(ptl => ptl.OldProductID == ProductID.Value && ptl.LocalizationID == LocalizationID.Value).Select(ptl1 => ptl1.NewLineQuantity).Sum();
                    maxvalue = maxvalue - existing;

                    //en cas de modification de la ligne, il ne faut pas prendre en compte la quantité de la ligne en cours de modification
                    if (TMPID.Value > 0 && TMPID.Value > 0)
                    {
                        double existing2 = RegProductNumberLines.SingleOrDefault(ptl => ptl.TMPID == TMPID.Value).NewLineQuantity;
                        maxvalue = maxvalue + existing2;
                    }
                }
            }

            return maxvalue;
        }

        public ActionResult LoadMaxQuantity(int? ProductID, int? LocalizationID, int? TMPID)
        {
            this.GetCmp<NumberField>("NewLineQuantity").SetMaxValue(GetMaxValue(ProductID, LocalizationID, TMPID));
            return this.Direct();
        }
        public ActionResult ReloadRegProductNumberListStore()
        {
            this.GetCmp<Store>("RegProductNumberGridStoreID").Reload();
            return this.Direct();
        }
        public List<object> ModelPendingRegProductNumber(int? BranchID, DateTime RegProductNumberDate)
        {
            List<object> list = new List<object>();

            if (BranchID.HasValue && BranchID.Value > 0)
            {

                
                List<RegProductNumber> dataTmp = (from pt in db.RegProductNumbers
                                                      where ( pt.BranchID==BranchID.Value && pt.RegProductNumberDate == RegProductNumberDate.Date)
                                                      select pt).ToList();

                if (dataTmp != null && dataTmp.Count > 0)
                {

                    foreach (RegProductNumber pt in dataTmp)
                    {
                        list.Add(
                            new
                            {
                                RegProductNumberID = pt.RegProductNumberID,
                                RegProductNumberReference = pt.RegProductNumberReference,
                                RegProductNumberDate = pt.RegProductNumberDate,
                                Branch = pt.Branch.BranchName,
                                AutorizedBy = pt.AutorizedBy.UserFullName,
                                RegisteredBy = pt.RegisteredBy.UserFullName,
                            }
                           );
                    }

                }

                
            }

            return list;
        }
        //[HttpPost]
        public StoreResult GetAllPendingRegProductNumbers(int? BranchID, DateTime ReloadRegProductNumberDate)
        {
            if (BranchID == null) BranchID = SessionBusinessDay(null).BranchID;
            return this.Store(ModelPendingRegProductNumber(BranchID, ReloadRegProductNumberDate));
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



        public void InitializeRegProductNumberLineFields(int ID)
        {
            Session["isUpdate"] = true;
            
            this.SimpleReset2();

            List<RegProductNumberLine> RegProductNumberLines = (List<RegProductNumberLine>)Session["RegProductNumberLines"];

            if (ID > 0)
            {
                RegProductNumberLine ptl = new RegProductNumberLine();
                ptl = RegProductNumberLines.SingleOrDefault(pl => pl.TMPID == ID);

                this.GetCmp<TextField>("RegProductNumberLineID").Value = ptl.RegProductNumberLineID;
                this.GetCmp<TextField>("TMPID").Value = ptl.TMPID;

                this.GetCmp<ComboBox>("LocalizationID").GetStore().Reload();
                this.GetCmp<ComboBox>("LocalizationID").Value = ptl.LocalizationID;
                
                this.GetCmp<ComboBox>("ProductCategoryID").SetValue(ptl.OldProduct.CategoryID);

                this.GetCmp<ComboBox>("NewProductCategoryID").SetValue(ptl.NewProduct.CategoryID);

                if (ptl.OldProduct is Lens || ptl.NewProduct is Lens)
                {
                    this.GetCmp<ComboBox>("OldProductNumberID").Disabled = false;
                    this.GetCmp<ComboBox>("NewProductNumberID").Disabled = false;

                    LensNumber Oldnumber = db.Lenses.Find(ptl.OldProductID).LensNumber;
                    Session["OldLensNumberFullCode"] = Oldnumber.LensNumberFullCode;
                    this.GetCmp<ComboBox>("OldProductNumberID").SetValue(Oldnumber.LensNumberID);

                    LensNumber Newnumber = db.Lenses.Find(ptl.NewProductID).LensNumber;
                    Session["NewLensNumberFullCode"] = Newnumber.LensNumberFullCode;
                    this.GetCmp<ComboBox>("NewProductNumberID").SetValue(Newnumber.LensNumberID);

                }

                this.GetCmp<ComboBox>("OldProductID").GetStore().Reload();
                this.GetCmp<ComboBox>("OldProductID").Value = ptl.OldProductID;

                this.GetCmp<ComboBox>("NewProductID").GetStore().Reload();
                this.GetCmp<ComboBox>("NewProductID").Value = ptl.NewProductID;

                double qtystock = GetMaxQtyInStock(ptl.OldProductID, ptl.LocalizationID);
                this.GetCmp<NumberField>("StockQuantity").Value = qtystock;
                Session["MaxValue"] = qtystock;

                this.GetCmp<ComboBox>("NewLineQuantity").Value = ptl.NewLineQuantity;

            }
            ManageCady();

        }

        public double GetMaxQtyInStock(int productID, int localizationID)
        {
            double res = 0;

          
            res = db.ProductLocalizations.SingleOrDefault(pl => pl.ProductID == productID && pl.LocalizationID == localizationID).ProductLocalizationStockQuantity;

            return res;
        }


        public void ManageCady()
        {
            List<RegProductNumberLine> RegProductNumberLines = (List<RegProductNumberLine>)Session["RegProductNumberLines"];

            if (RegProductNumberLines != null && RegProductNumberLines.Count > 0)
            {
                this.GetCmp<TextField>("IsCadyEmpty").SetValue(0);//faux
            }
            if (RegProductNumberLines == null || RegProductNumberLines.Count == 0)
            {
                this.GetCmp<TextField>("IsCadyEmpty").SetValue(1);//vrai
            }
        }

        public ActionResult DisableNumero(int ProductCategoryID)
        {
            Category catprod = db.Categories.Find(ProductCategoryID);
            if (catprod is LensCategory)
            {
                this.GetCmp<ComboBox>("OldProductNumberID").Disabled = false;
                this.GetCmp<ComboBox>("OldProductID").HideTrigger = true;

                this.GetCmp<ComboBox>("NewProductNumberID").Disabled = false;
                this.GetCmp<ComboBox>("NewProductID").HideTrigger = true;
            }
            else
            {
                this.GetCmp<ComboBox>("OldProductNumberID").Disabled = true;
                this.GetCmp<ComboBox>("OldProductID").HideTrigger = false;

                this.GetCmp<ComboBox>("NewProductNumberID").Disabled = true;
                this.GetCmp<ComboBox>("NewProductID").HideTrigger = false;
            }
            return this.Direct();
        }

        public ActionResult GetAllPagingOldNumbers(int start, int limit, int page, string query, int? ProductCategory, int? localization)
        {
            bool isUpdate = (bool)Session["isUpdate"];
            string LensNumberFullCode = (string)Session["OldLensNumberFullCode"];

            query = (isUpdate == true) ? LensNumberFullCode : query;

            Paging<LensNumber> numbers = GetAllNumbers(start, limit, "", "", query, ProductCategory, localization);

            Session["isUpdate"] = false;
            Session["OldLensNumberFullCode"] = "*";

            return this.Store(numbers.Data, numbers.TotalRecords);
        }
        public ActionResult GetAllPagingNewNumbers(int start, int limit, int page, string query, int? ProductCategory, int? localization)
        {
            bool isUpdate = (bool)Session["isUpdate"];
            string LensNumberFullCode = (string)Session["NewLensNumberFullCode"];

            query = (isUpdate == true) ? LensNumberFullCode : query;

            Paging<LensNumber> numbers = GetAllNumbers(start, limit, "", "", query, ProductCategory, localization);

            Session["isUpdate"] = false;
            Session["NewLensNumberFullCode"] = "*";

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
        public ActionResult OnOldLensNumberSelected(int? Localization, int? ProductNumberID, int? ProductCategoryID)
        {
            try
            {

                if ((Localization.HasValue && Localization.Value > 0) && (ProductNumberID.HasValue && ProductNumberID.Value > 0) && (ProductCategoryID.HasValue && ProductCategoryID.Value > 0))
                {
                    this.GetCmp<ComboBox>("OldProductID").SetValueAndFireSelect(GetLensID(Localization.Value, ProductCategoryID.Value, ProductNumberID.Value));
                }
                else
                {
                    X.Msg.Alert(Resources.Productlabel, "This Product does not exist").Show();
                }

                return this.Direct();
            }
            catch (Exception e)
            {
                statusOperation = Resources.er_alert_danger + e.Message + "Méthode = OnOldLensNumberSelected() " + e.TargetSite;
                X.Msg.Alert(Resources.Productlabel, statusOperation).Show();
                return this.Direct();
            }
        }

        [HttpPost]
        public ActionResult OnNewLensNumberSelected(int? Localization, int? ProductNumberID, int? ProductCategoryID)
        {
            try
            {

                if ((Localization.HasValue && Localization.Value > 0) && (ProductNumberID.HasValue && ProductNumberID.Value > 0) && (ProductCategoryID.HasValue && ProductCategoryID.Value > 0))
                {
                    this.GetCmp<ComboBox>("NewProductID").SetValueAndFireSelect(GetLensID(Localization.Value, ProductCategoryID.Value, ProductNumberID.Value));
                }
                else
                {
                    X.Msg.Alert(Resources.Productlabel, "This Product does not exist").Show();
                }

                return this.Direct();
            }
            catch (Exception e)
            {
                statusOperation = Resources.er_alert_danger + e.Message + "Méthode = OnNewLensNumberSelected() " + e.TargetSite;
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
                            ProductLocalizationStockSellingPrice = s.pl.ProductLocalizationStockSellingPrice
                        }).FirstOrDefault();

            Session["MaxValue"] = lstLensProduct.ProductQuantity;
            Session["LineUnitPrice"] = lstLensProduct.ProductLocalizationStockSellingPrice;

            return lstLensProduct.ProductID;

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


        [HttpPost]
        public ActionResult OnProductSelected(int? Localization, int? CurrentProduct)
        {
            
            try
            {

                if ((!Localization.HasValue || Localization.Value <= 0) || (!CurrentProduct.HasValue || CurrentProduct.Value <= 0)) { return this.Direct(); }

               
                Product product = (from prod in db.Products
                                   where prod.ProductID==CurrentProduct.Value
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

        private Company Company
		{
			get
			{
				return db.Companies.FirstOrDefault();
			}
		}

        //This method load a method that print a receip of deposit
        public ActionResult PrintRegProductNumberReceipt()
        {
            this.GetCmp<Panel>("Pdf").LoadContent(new ComponentLoader
            {
                Url = Url.Action("GenerateReceipt"),
                DisableCaching = false,
                Mode = LoadMode.Frame,
            });

            return this.Direct();
        }

        //This method print a receipt of customer
        public void GenerateReceipt()
        {
            List<object> model = new List<object>();
            ReportDocument rptH = new ReportDocument();
            try
            {
            int RegProductNumberID = (int)Session["RegProductNumberID"];
            
            string repName = "";
            bool isValid = false;
            double totalAmount = 0d;
            double totalRemaining = 0d;

            
            string path = "";
            string defaultImgPath = Server.MapPath("~/Content/Images/App/deboy-logo.png");
            byte[] COMPANY_LOGO = System.IO.File.ReadAllBytes(defaultImgPath);

            string DeviseLabel = db.Devises.Where(d => d.DefaultDevise).FirstOrDefault().DeviseLabel;
            RegProductNumber RegProductNumber = (from pt in db.RegProductNumbers
                                                 where pt.RegProductNumberID == RegProductNumberID
                                   select pt).SingleOrDefault();
               
            var curBranch = db.UserBranches
                        .Where(br => br.UserID==SessionGlobalPersonID)
                        .ToList()
                        .Select(s => new UserBranch
                        {
                            BranchID = s.BranchID,
                            Branch = s.Branch
                        })
                        .AsQueryable()
                        .FirstOrDefault(); 

            double saleAmount = 0d;
            db.RegProductNumberLines.Where(l => l.RegProductNumberID == RegProductNumberID).ToList().ForEach(c =>
            {
                isValid = true;
                model.Add(
                                new
                                {
                                    ReceiveAmount = 0,
                                    LineQuantity = c.NewLineQuantity,
                                    ProductLabel = c.OldProduct.ProductCode,
                                    ProductRef = c.NewProduct.ProductCode,
                                    CompanyName = Company.Name,
                                    CompanyAdress = "Head Quater : " + Company.Adress.Quarter.Town.Region.RegionLabel + " - " + Company.Adress.Quarter.Town.TownLabel,
                                    CompanyTel = "Tel: " + Company.Adress.AdressPhoneNumber,
                                    BranchName = curBranch.Branch.BranchName,
                                    BranchAdress = curBranch.Branch.Adress.Quarter.QuarterLabel + " - " + curBranch.Branch.Adress.Quarter.Town.TownLabel,
                                    BranchTel = "Tel: " + curBranch.Branch.Adress.AdressPhoneNumber,
                                    Ref = RegProductNumber.RegProductNumberReference,
                                    CompanyCNI = "NO CONT : " + Company.CNI,
                                    Operator = CurrentUser.Name + " " + CurrentUser.Description,
                                    SendindBranchName = RegProductNumber.Branch.BranchName,
                                    SendindBranchCode = RegProductNumber.Branch.BranchCode,
                                    RegProductNumbertDate = RegProductNumber.RegProductNumberDate.Date,
                                    Title = "Product RegProductNumbert lines informations",
                                    DeviseLabel = DeviseLabel,
                                    CompanyLogo = db.Files.FirstOrDefault(f => f.GlobalPersonID == Company.GlobalPersonID) != null ? db.Files.FirstOrDefault(f => f.GlobalPersonID == Company.GlobalPersonID).Content : COMPANY_LOGO
                                }
                        );
            }
                    );
                
            if (isValid)
            {
                path = Server.MapPath("~/Reports/Supply/RptReceiptRegProductNumbert.rpt");
                repName = "RptReceiptRegProductNumbert";
                rptH.Load(path);
                rptH.SetDataSource(model);
                bool isDownloadRpt = (bool)Session["isDownloadRpt"];
                rptH.ExportToHttpResponse(ExportFormatType.PortableDocFormat, System.Web.HttpContext.Current.Response, false, repName);
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