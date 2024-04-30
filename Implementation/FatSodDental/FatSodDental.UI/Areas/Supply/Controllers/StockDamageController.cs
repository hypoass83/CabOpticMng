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
    public partial class StockDamageController : BaseController
    {
        private bool isLens = false;
        //Current Controller and current page
        private const string CONTROLLER_NAME = "Supply/StockDamage" ;
        private const string VIEW_NAME = "Index";
        private IProductDamage _productDamageRepository;
        private IBusinessDay _busDayRepo;
        private ITransactNumber _transactNumbeRepository;
        private IProductLocalization _productLocRepository;
        private ILensNumberRangePrice _priceRepository;
        //Construcitor

        private List<BusinessDay> lstBusDay;
        public StockDamageController(IProductDamage productDamageRepository, 
            IBusinessDay bDRepo,
            ITransactNumber transactRepo,
            ILensNumberRangePrice priceRepo,
            IProductLocalization productLocRepo
            )
        {
            this._productDamageRepository = productDamageRepository;
            this._busDayRepo = bDRepo;
            this._transactNumbeRepository = transactRepo;
            this._priceRepository = priceRepo;
            this._productLocRepository = productLocRepo;
        }
        [OutputCache(Duration=43200)]
        public ActionResult Index()
        {
            Session["isUpdate"] = false;
            Session["LensNumberFullCode"] = "*";

            List<ProductDamageLine> ProductDamageLines = new List<ProductDamageLine>();
            Session["ProductDamageLines"] = ProductDamageLines;
            lstBusDay = (List<BusinessDay>)Session["UserBusDays"];
            if (lstBusDay == null)
            {
                lstBusDay = _busDayRepo.GetOpenedBusinessDay(CurrentUser);
            };
            DateTime BDDateOperation = lstBusDay.FirstOrDefault().BDDateOperation;
            
            ViewBag.BusnessDayDate = BDDateOperation;
            //return rPVResult;
            return View(ModelPendingProductDamage(lstBusDay.FirstOrDefault().BranchID,BDDateOperation));
        }

        [HttpPost]
        public ActionResult AddManager(ProductDamage productDamage)
        {
            List<ProductDamageLine> ProductDamageLines = new List<ProductDamageLine>();
            ProductDamageLines = (List<ProductDamageLine>)Session["ProductDamageLines"];
            productDamage.ProductDamageLines = ProductDamageLines;
            
            if (productDamage.ProductDamageID == 0)
            {
                return DoproductDamage(productDamage);
            }

            if (productDamage.ProductDamageID > 0)
            {
                X.Msg.Alert(Resources.er_alert_danger, "This operation not yet possible ").Show();
                return this.Direct();
                //return UpdateproductDamage(productDamage);
            }
            
            X.Msg.Alert(Resources.er_alert_danger, "productDamageID can not be negative ").Show();
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
                string trnnum = _transactNumbeRepository.displayTransactNumber("PRDA", businessDay);
                this.GetCmp<TextField>("ProductDamageReference").Value = trnnum;
                this.GetCmp<TextField>("RegisteredByID").Value = SessionGlobalPersonID;
            }
            return this.Direct();
        }

        [HttpPost]
        public ActionResult DoproductDamage(ProductDamage productDamage)
        {
            try
            {

                int StockDamageID = _productDamageRepository.DoProductDamage(productDamage, SessionGlobalPersonID).ProductDamageID;
                Session["StockDamageID"] = StockDamageID;
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
        public ActionResult UpdateproductDamage(ProductDamage productDamage)
        {
            try
            {
                int StockDamageID= _productDamageRepository.UpdateProductDamage(productDamage).ProductDamageID;
                Session["StockDamageID"] = StockDamageID;
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
        public ActionResult AddproductDamageLine(ProductDamageLine productDamageLine)
        {
            try 
            { 
            productDamageLine.Product = db.Products.Find(productDamageLine.ProductID);
            productDamageLine.Localization = db.Localizations.Find(productDamageLine.LocalizationID);
            

            List<ProductDamageLine> ProductDamageLines = (List<ProductDamageLine>)Session["ProductDamageLines"];

            //il s'agit d'une modification alors on fait un drop and create
            if (productDamageLine.TMPID > 0)
            {
                ProductDamageLine toRemove = ProductDamageLines.SingleOrDefault(pl => pl.TMPID == productDamageLine.TMPID);
                productDamageLine.TMPID = 0;
                ProductDamageLines.Remove(toRemove);
            }

            //alors la variable de session n'était pas vide
            if (ProductDamageLines != null && ProductDamageLines.Count > 0)
            {
                //c'est un nouvel ajout dans le panier
                if (productDamageLine.TMPID == 0)
                {
                    ProductDamageLine existing = ProductDamageLines.SingleOrDefault(pl => pl.ProductID == productDamageLine.ProductID &&
                                                                                                    pl.LocalizationID == productDamageLine.LocalizationID 
                                                                                                );
                    //existe t-il déjà une ligne de vente ayant le meme produit et le même magasin que celui en création?
                    if (/*ProductDamageLines.Contains(productDamageLine) && productDamageLine.TMPID > 0*/
                        existing != null && existing.TMPID > 0) //cette ligne exixte déjà
                    {
                        //la quantité est la somme des deux quantité
                        productDamageLine.LineQuantity += existing.LineQuantity;
                        //l'id c'est l'id de la ligne existante
                        productDamageLine.TMPID = existing.TMPID;
                        productDamageLine.ProductDamageLineID = existing.ProductDamageLineID;
                        //on retire l'ancien pour ajouter le nouveau
                        ProductDamageLines.Remove(existing);
                    }

                    if (existing == null || existing.TMPID == 0)//La ligne n'existe pas encore dans le panier
                    {
                        productDamageLine.TMPID = ProductDamageLines.Select(pl => pl.TMPID).Max() + 1;
                    }
                }
            }

            //alors la variable de session était vide
            if (ProductDamageLines == null || ProductDamageLines.Count == 0)
            {
                //c'est une nouvelle création pour la création
                ProductDamageLines = new List<ProductDamageLine>();
                productDamageLine.TMPID = 1;
            }
            //verifiction ds le stock source s'il ya assez de produit a productDamageer
            bool res = _productLocRepository.checkQtyInStock(productDamageLine.ProductID, productDamageLine.LocalizationID, productDamageLine.LineQuantity);
            if (res)
            {
                ProductDamageLines.Add(productDamageLine);
                Session["ProductDamageLines"] = ProductDamageLines;
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
        public ActionResult RemoveProductDamageLine(int TMPID)
        {
            List<ProductDamageLine> ProductDamageLines = (List<ProductDamageLine>)Session["ProductDamageLines"];

            if (ProductDamageLines != null && ProductDamageLines.Count > 0)
            {
                ProductDamageLine toRemove = ProductDamageLines.SingleOrDefault(pl => pl.TMPID == TMPID);
                ProductDamageLines.Remove(toRemove);

                Session["ProductDamageLines"] = ProductDamageLines;
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
        public ActionResult UpdateProductDamageLine(int TMPID)
        {
            this.InitializeProductDamageLineFields(TMPID);
            return this.Direct();
        }

        private void SimpleReset2()
        {

            this.GetCmp<ComboBox>("ProductNumberID").Value = "";
            this.GetCmp<ComboBox>("ProductID").Value = "";
            this.GetCmp<NumberField>("StockQuantity").Value = "";
            this.GetCmp<NumberField>("LineQuantity").Value = "";
            this.GetCmp<NumberField>("LineUnitPrice").Value = "";
            this.GetCmp<TextArea>("ProductDamageReason").Value = "";

            this.GetCmp<Store>("ProductDamageLinesStore").Reload();

            ManageCady();
        }

        //Return salelines of current sale
        [HttpPost]
        public ActionResult ProductDamageLines()
        {
            List<ProductDamageLine> ProductDamageLines = (List<ProductDamageLine>)Session["ProductDamageLines"];
            List<object> list = new List<object>();

            foreach (ProductDamageLine pl in ProductDamageLines)
            {
                list.Add(
                    new
                    {
                        TMPID = pl.TMPID,
                        ProductLabel = (pl.Product is Lens) ? pl.Product.ProductCode : pl.Product.ProductLabel,
                        Localization = pl.Localization.LocalizationLabel,
                        LineQuantity = pl.LineQuantity,
                        LineUnitPrice = pl.LineUnitPrice,
                        ProductDamageReason=pl.ProductDamageReason
                    }
                    );
            }
            return this.Store(list);
        }

        [HttpGet]
        public ActionResult UpdateProductDamage(int ProductDamageID)
        {
            this.InitializeStockDamageFields(ProductDamageID);
            List<ProductDamageLine> data = db.ProductDamageLines.Where(ptl => ptl.ProductDamageID == ProductDamageID).ToList();
            List<ProductDamageLine> ProductDamageLines = new List<ProductDamageLine>();
            int i = 0;
            foreach (ProductDamageLine pl in data)
            {
                pl.TMPID = ++i;
                ProductDamageLines.Add(pl);
            }
            Session["StockDamageID"] = ProductDamageID;
            this.GetCmp<Button>("btnPrint").Disabled = false;
            Session["ProductDamageLines"] = ProductDamageLines;
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
            List<ProductDamageLine> ProductDamageLines = new List<ProductDamageLine>();
            Session["ProductDamageLines"] = ProductDamageLines;
            this.GetCmp<FormPanel>("GeneralForm").Reset(true);
            this.GetCmp<Store>("ProductDamageGridStoreID").Reload();

        }


        public void InitializeStockDamageFields(int ID)
        {
            this.SimpleReset();

            if (ID == 0)
            {
                return;
            }

            ProductDamage productDamage = db.ProductDamages.Find(ID);
            this.GetCmp<TextField>("ProductDamageID").Value = productDamage.ProductDamageID;

            this.GetCmp<ComboBox>("BranchID").Value = productDamage.BranchID;
            this.GetCmp<ComboBox>("AutorizedByID").Value = productDamage.AutorizedByID;
            this.GetCmp<TextField>("RegisteredByID").Value = productDamage.RegisteredByID;

            this.GetCmp<TextField>("ProductDamageReference").Value = productDamage.ProductDamageReference;
            this.GetCmp<DateField>("ProductDamageDate").Value = productDamage.ProductDamageDate;

            this.ManageCady();

        }
        [HttpPost]
        public ActionResult DeleteProductDamage(int ProductDamageID)
        {
            try
            {
                _productDamageRepository.CancelProductDamage(ProductDamageID);
                this.AlertSucces(Resources.Success, "product Damage Has Been Successfully Deleted ");
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
            this.GetCmp<DateField>("ProductDamageDate").Reset();

            if (BranchID.HasValue && BranchID.Value > 0)
            {
                lstBusDay = (List<BusinessDay>)Session["UserBusDays"];
                if (lstBusDay == null)
                {
                    lstBusDay = _busDayRepo.GetOpenedBusinessDay(CurrentUser);
                };
                BusinessDay businessDay = lstBusDay.FirstOrDefault(l => l.BranchID == BranchID.Value);
                this.GetCmp<DateField>("ProductDamageDate").SetValue(businessDay.BDDateOperation);

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

                //enlevons les quantités déjà prise pour faire d'autres lignes de productDamaget
                List<ProductDamageLine> ProductDamageLines = (List<ProductDamageLine>)Session["ProductDamageLines"];
                if (ProductDamageLines != null && ProductDamageLines.Count > 0)
                {
                    double existing = ProductDamageLines.Where(ptl => ptl.ProductID == ProductID.Value && ptl.LocalizationID == LocalizationID.Value).Select(ptl1 => ptl1.LineQuantity).Sum();
                    maxvalue = maxvalue - existing;

                    //en cas de modification de la ligne, il ne faut pas prendre en compte la quantité de la ligne en cours de modification
                    if (TMPID.Value > 0 && TMPID.Value > 0)
                    {
                        double existing2 = ProductDamageLines.SingleOrDefault(ptl => ptl.TMPID == TMPID.Value).LineQuantity;
                        maxvalue = maxvalue + existing2;
                    }
                }
            }

            return maxvalue;
        }

        public ActionResult LoadMaxQuantity(int? ProductID, int? LocalizationID, int? TMPID)
        {
            //this.GetCmp<NumberField>("LineQuantity").Reset();
            this.GetCmp<NumberField>("LineQuantity").SetMaxValue(GetMaxValue(ProductID, LocalizationID, TMPID));
            return this.Direct();
        }
        public ActionResult ReloadproductDamageListStore()
        {
            this.GetCmp<Store>("ProductDamageGridStoreID").Reload();
            return this.Direct();
        }
        public List<object> ModelPendingProductDamage(int? BranchID, DateTime ProductDamageDate)
        {
            List<object> list = new List<object>();

            if (BranchID.HasValue && BranchID.Value > 0)
            {

                
                List<ProductDamage> dataTmp = (from pt in db.ProductDamages
                                                      where ( pt.BranchID==BranchID.Value && pt.ProductDamageDate == ProductDamageDate.Date)
                                                      select pt).ToList();

                if (dataTmp != null && dataTmp.Count > 0)
                {

                    foreach (ProductDamage pt in dataTmp)
                    {
                        list.Add(
                            new
                            {
                                ProductDamageID = pt.ProductDamageID,
                                ProductDamageReference = pt.ProductDamageReference,
                                ProductDamageDate = pt.ProductDamageDate,
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
        public StoreResult GetAllPendingProductDamages(int? BranchID, DateTime ReloadProductDamageDate)
        {
            if (BranchID == null) BranchID = SessionBusinessDay(null).BranchID;
            return this.Store(ModelPendingProductDamage(BranchID, ReloadProductDamageDate));
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



        public void InitializeProductDamageLineFields(int ID)
        {
            Session["isUpdate"] = true;
            
            this.SimpleReset2();

            List<ProductDamageLine> ProductDamageLines = (List<ProductDamageLine>)Session["ProductDamageLines"];

            if (ID > 0)
            {
                ProductDamageLine ptl = new ProductDamageLine();
                ptl = ProductDamageLines.SingleOrDefault(pl => pl.TMPID == ID);

                this.GetCmp<TextField>("ProductDamageLineID").Value = ptl.ProductDamageLineID;
                this.GetCmp<TextField>("TMPID").Value = ptl.TMPID;

                this.GetCmp<ComboBox>("LocalizationID").GetStore().Reload();
                this.GetCmp<ComboBox>("LocalizationID").Value = ptl.LocalizationID;
                
                this.GetCmp<ComboBox>("ProductCategoryID").SetValue(ptl.Product.CategoryID);


                if (ptl.Product is Lens)
                {
                    this.GetCmp<ComboBox>("ProductNumberID").Disabled = false;
                    //this.GetCmp<ComboBox>("ProductNumberID").GetStore().Reload();

                    LensNumber number = db.Lenses.Find(ptl.ProductID).LensNumber;
                    Session["LensNumberFullCode"] = number.LensNumberFullCode;

                    this.GetCmp<ComboBox>("ProductNumberID").SetValue(number.LensNumberID);

                    this.GetCmp<Container>("EyeSide").Disabled = false;
                    this.GetCmp<Radio>("OD").Checked = (ptl.OeilDroiteGauche == EyeSide.OD) ? true : false;
                    this.GetCmp<Radio>("OG").Checked = (ptl.OeilDroiteGauche == EyeSide.OG) ? true : false;
                    this.GetCmp<Radio>("ODG").Checked = (ptl.OeilDroiteGauche == EyeSide.ODG) ? true : false;

                }

                this.GetCmp<ComboBox>("ProductID").GetStore().Reload();
                this.GetCmp<ComboBox>("ProductID").Value = ptl.ProductID;

                double qtystock = GetMaxQtyInStock(ptl.ProductID, ptl.LocalizationID);
                this.GetCmp<NumberField>("StockQuantity").Value = qtystock;
                Session["MaxValue"] = qtystock;

                this.GetCmp<ComboBox>("LineQuantity").Value = ptl.LineQuantity;
                this.GetCmp<ComboBox>("LineUnitPrice").Value = ptl.LineUnitPrice;
                this.GetCmp<TextArea>("ProductDamageReason").Value = ptl.ProductDamageReason;
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
            List<ProductDamageLine> ProductDamageLines = (List<ProductDamageLine>)Session["ProductDamageLines"];

            if (ProductDamageLines != null && ProductDamageLines.Count > 0)
            {
                this.GetCmp<TextField>("IsCadyEmpty").SetValue(0);//faux
            }
            if (ProductDamageLines == null || ProductDamageLines.Count == 0)
            {
                this.GetCmp<TextField>("IsCadyEmpty").SetValue(1);//vrai
            }
        }

        public ActionResult DisableNumero(int ProductCategoryID)
        {
            Category catprod = db.Categories.Find(ProductCategoryID);
            if (catprod is LensCategory)
            //if (isLens)
            {
                //isLens = true;
                this.GetCmp<Container>("EyeSide").Disabled = false;
                this.GetCmp<ComboBox>("ProductNumberID").Disabled = false;
                this.GetCmp<ComboBox>("ProductID").HideTrigger = true;
            }
            else
            {
                //isLens = false;
                this.GetCmp<Container>("EyeSide").Disabled = true;
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
                    //Récupération du prix du verre à partir de son intervalle de numéro
                    LensNumberRangePrice price = _priceRepository.GetPrice(product.ProductID);

                    double LineUnitPrice = (price != null && price.LensNumberRangePriceID > 0) ? price.Price : 0;

                    this.GetCmp<NumberField>("LineUnitPrice").Value = LineUnitPrice;
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

        private Company Company
		{
			get
			{
				return db.Companies.FirstOrDefault();
			}
		}

        //This method load a method that print a receip of deposit
        public ActionResult PrintproductDamageReceipt()
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
            int StockDamageID = (int)Session["StockDamageID"];
            
            string repName = "";
            bool isValid = false;
            double totalAmount = 0d;
            double totalRemaining = 0d;

           
            string path = "";
            string defaultImgPath = Server.MapPath("~/Content/Images/App/deboy-logo.png");
            byte[] COMPANY_LOGO = System.IO.File.ReadAllBytes(defaultImgPath);

            string DeviseLabel = db.Devises.Where(d => d.DefaultDevise).FirstOrDefault().DeviseLabel;
            ProductDamage ProductDamage = (from pt in db.ProductDamages
                                                 where pt.ProductDamageID == StockDamageID
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
            db.ProductDamageLines.Where(l => l.ProductDamageID == StockDamageID).ToList().ForEach(c =>
            {
                isValid = true;
                saleAmount += c.LineUnitPrice;
                model.Add(
                                new
                                {
                                    ReceiveAmount = 0,
                                    LineQuantity = c.LineQuantity,
                                    LineUnitPrice = c.LineUnitPrice,
                                    ProductLabel = c.ProductDamageReason,
                                    ProductRef = c.Product.ProductCode,//(c.OeilDroiteGauche == EyeSide.N) ? c.Product.ProductCode : c.OeilDroiteGauche + ":" + c.Product.ProductCode,
                                    CompanyName = Company.Name,
                                    CompanyAdress = "Head Quater : " + Company.Adress.Quarter.Town.Region.RegionLabel + " - " + Company.Adress.Quarter.Town.TownLabel,
                                    CompanyTel = "Tel: " + Company.Adress.AdressPhoneNumber,
                                    BranchName = curBranch.Branch.BranchName,
                                    BranchAdress = curBranch.Branch.Adress.Quarter.QuarterLabel + " - " + curBranch.Branch.Adress.Quarter.Town.TownLabel,
                                    BranchTel = "Tel: " + curBranch.Branch.Adress.AdressPhoneNumber,
                                    Ref = ProductDamage.ProductDamageReference,
                                    CompanyCNI = "NO CONT : " + Company.CNI,
                                    Operator = CurrentUser.Name + " " + CurrentUser.Description,
                                    SendindBranchName = ProductDamage.Branch.BranchName,
                                    SendindBranchCode = ProductDamage.Branch.BranchCode,
                                    productDamagetDate = ProductDamage.ProductDamageDate.Date,
                                    Title = "Product productDamaget lines informations",
                                    DeviseLabel = DeviseLabel,
                                    CompanyLogo = db.Files.FirstOrDefault(f => f.GlobalPersonID == Company.GlobalPersonID) != null ? db.Files.FirstOrDefault(f => f.GlobalPersonID == Company.GlobalPersonID).Content : COMPANY_LOGO
                                }
                        );
            }
                    );
                
            if (isValid)
            {
                path = Server.MapPath("~/Reports/Supply/RptReceiptproductDamage.rpt");
                repName = "RptReceiptproductDamage";
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