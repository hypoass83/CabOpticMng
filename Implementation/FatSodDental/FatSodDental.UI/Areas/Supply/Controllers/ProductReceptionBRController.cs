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
using System.Web.UI;

namespace FatSodDental.UI.Areas.Supply.Controllers
{
    [Authorize(Order = 1)]
    [TakeBusinessDay(Order = 2)]
    public partial class ProductReceptionBRController : BaseController
    {
        private bool isLens = false;
        //Current Controller and current page
        private const string CONTROLLER_NAME = "Supply/" + CodeValue.Supply.ProductReception_BR.CONTROLLER;
        private const string VIEW_NAME = CodeValue.Supply.ProductReception_BR.PATH;
        private ITransfert _transfertRepository;
        
        private IBusinessDay _busDayRepo;

        private ITransactNumber _transactNumbeRepository;
        private ILensNumberRangePrice _priceRepository;

        private IRepository<FatSod.Security.Entities.File> _fileRepository;
        //Construcitor

        private List<BusinessDay> lstBusDay;
        public ProductReceptionBRController(ITransfert transRepo, IBusinessDay bDRepo,
                                           ITransactNumber transactRepo,
                                           ILensNumberRangePrice priceRepo, 
                                           IRepository<FatSod.Security.Entities.File> fileRepository
            )
        {
            this._transfertRepository = transRepo;
            this._busDayRepo = bDRepo;
            this._transactNumbeRepository = transactRepo;
            this._priceRepository = priceRepo;
            this._fileRepository = fileRepository;
        }
        [OutputCache(Duration = 3600)] 
        public ActionResult ProductReceptionBR()
        {
            Session["isUpdate"] = false;
            Session["LensNumberFullCode"] = "*";

            //Session["Curent_Page"] = VIEW_NAME;
            //Session["Curent_Controller"] = CONTROLLER_NAME;

            //We test if this user has an autorization to get this sub menu
            //if (!LoadAction.isAutoriseToGetMenu(SessionProfileID, CodeValue.Supply.SupplyMenu.CODE, db))
            //{
            //    RedirectToAction("NotAuthorized", "User");
            //}

            //this.GetCmp<Panel>("Body").LoadContent(new ComponentLoader
            //{
            //    Url = Url.Action(VIEW_NAME),
            //    DisableCaching = false,
            //    Mode = LoadMode.Frame
            //});

            List<ProductTransfertLine> ProductTransfertLines = new List<ProductTransfertLine>();
            Session["ProductTransfertLines"] = ProductTransfertLines;

            ////return View(ModelDeparturePendingTransfer(0));
            //Ext.Net.MVC.PartialViewResult rPVResult = new Ext.Net.MVC.PartialViewResult
            //{
            //    ClearContainer = true,
            //    //RenderMode = RenderMode.AddTo,
            //    ContainerId = "Body",
            //    //WrapByScriptTag = false,
            //    Model = ModelDeparturePendingTransfer(0)
            //};


            lstBusDay = (List<BusinessDay>)Session["UserBusDays"];
            if (lstBusDay == null)
            {
                lstBusDay = _busDayRepo.GetOpenedBusinessDay(CurrentUser);
            };
            DateTime BDDateOperation = lstBusDay.FirstOrDefault().BDDateOperation;

         
            ViewBag.BusnessDayDate = BDDateOperation;
            
            //return View(ModelDeparturePendingTransfer(BDDateOperation));


            return View(ModelDeparturePendingTransfer(BDDateOperation, BDDateOperation));
        }

        [HttpPost]
        public ActionResult AddManager(ProductTransfert transfer)
        {
            List<ProductTransfertLine> ProductTransfertLines = new List<ProductTransfertLine>();
            ProductTransfertLines = (List<ProductTransfertLine>)Session["ProductTransfertLines"];
            transfer.ProductTransfertLines = ProductTransfertLines;
            

            if (transfer.ProductTransfertID == 0)
            {
                transfer.ReceivedDate = SessionBusinessDay(null).BDDateOperation.Date;
                return DoReception(transfer);
            }

            if (transfer.ProductTransfertID > 0)
            {
                if (transfer.ReceivedDate.Value.Date == SessionBusinessDay(null).BDDateOperation.Date) return UpdateReception(transfer);
                else
                {
                    X.Msg.Alert(Resources.er_alert_danger, "You cannot Update this Reception because the Reception date is different from the Business Day ").Show();
                    return this.Direct();
                }
            }
            
            X.Msg.Alert(Resources.er_alert_danger, "ProductTranfertID can not be negative ").Show();
            return this.Direct();

        }

        public ActionResult InitTrnNumber(int? BranchID)
        {
            if (BranchID > 0)
            {
                lstBusDay = (List<BusinessDay>)Session["UserBusDays"];
                if (lstBusDay == null)
                {
                    lstBusDay = _busDayRepo.GetOpenedBusinessDay(CurrentUser);
                };
                BusinessDay businessDay = lstBusDay.FirstOrDefault(l => l.BranchID == BranchID.Value);
                string trnnum = _transactNumbeRepository.displayTransactNumber("RECE", businessDay);
                this.GetCmp<TextField>("ProductTransfertReference").Value = trnnum;
                this.GetCmp<TextField>("RegisteredByID").Value = SessionGlobalPersonID;
            }
            return this.Direct();
        }

        [HttpPost]
        public ActionResult DoReception(ProductTransfert transfer)
        {
            try
            {

                int ProductReceptionID= _transfertRepository.ValidateTransfert(transfer,SessionGlobalPersonID).ProductTransfertID;
                Session["ProductReceptionID"] = ProductReceptionID;
                this.GetCmp<Button>("btnPrint").Disabled=false;
                this.AlertSucces(Resources.Success, "Products Have Been Successfully Received ");
                return this.Reset();
            }
            catch (Exception ex)
            {

                X.Msg.Alert(Resources.er_alert_danger, "Products Have not Been Successfully Receive because " + ex.Message + " " + ex.StackTrace).Show();
                return this.Direct();
            }
        }

        [HttpPost]
        public ActionResult UpdateReception(ProductTransfert transfer)
        {
            try
            {
                int ProductReceptionID= _transfertRepository.ValidateTransfert(transfer,SessionGlobalPersonID).ProductTransfertID;
                Session["ProductReceptionID"] = ProductReceptionID;
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
        public ActionResult AddTransferLine(ProductTransfertLine transferLine)
        {

            transferLine.Product = db.Products.Find(transferLine.ProductID);
            transferLine.DepartureLocalization = db.Localizations.Find(transferLine.ArrivalLocalizationID);
            transferLine.ArrivalLocalization = db.Localizations.Find(transferLine.ArrivalLocalizationID.Value);

            List<ProductTransfertLine> ProductTransfertLines = (List<ProductTransfertLine>)Session["ProductTransfertLines"];

            //il s'agit d'une modification alors on fait un drop and create
            if (transferLine.TMPID > 0)
            {
                ProductTransfertLine toRemove = ProductTransfertLines.SingleOrDefault(pl => pl.TMPID == transferLine.TMPID);
                transferLine.TMPID = 0;
                ProductTransfertLines.Remove(toRemove);
            }

            //alors la variable de session n'était pas vide
            if (ProductTransfertLines != null && ProductTransfertLines.Count > 0)
            {
                //c'est un nouvel ajout dans le panier
                if (transferLine.TMPID == 0)
                {
                    ProductTransfertLine existing = ProductTransfertLines.SingleOrDefault(pl => pl.ProductID == transferLine.ProductID &&
                                                                                                    pl.DepartureLocalizationID == transferLine.DepartureLocalizationID &&
                                                                                                    pl.ArrivalLocalizationID == transferLine.ArrivalLocalizationID.Value
                                                                                                );
                    //existe t-il déjà une ligne de vente ayant le meme produit et le même magasin que celui en création?
                    if (/*ProductTransfertLines.Contains(transferLine) && transferLine.TMPID > 0*/
                        existing != null && existing.TMPID > 0) //cette ligne exixte déjà
                    {
                        //la quantité est la somme des deux quantité
                        transferLine.LineQuantity += existing.LineQuantity;
                        //l'id c'est l'id de la ligne existante
                        transferLine.TMPID = existing.TMPID;
                        transferLine.ProductTransfertLineID = existing.ProductTransfertLineID;
                        //on retire l'ancien pour ajouter le nouveau
                        ProductTransfertLines.Remove(existing);
                    }

                    if (existing == null || existing.TMPID == 0)//La ligne n'existe pas encore dans le panier
                    {
                        transferLine.TMPID = ProductTransfertLines.Select(pl => pl.TMPID).Max() + 1;
                    }
                }
            }

            //alors la variable de session était vide
            if (ProductTransfertLines == null || ProductTransfertLines.Count == 0)
            {
                //c'est une nouvelle création pour la création
                ProductTransfertLines = new List<ProductTransfertLine>();
                transferLine.TMPID = 1;
            }

            ProductTransfertLines.Add(transferLine);
            Session["ProductTransfertLines"] = ProductTransfertLines;
            return this.Reset2();
        }

        [HttpPost]
        public ActionResult RemoveProductTransfertLine(int TMPID)
        {
            List<ProductTransfertLine> ProductTransfertLines = (List<ProductTransfertLine>)Session["ProductTransfertLines"];

            if (ProductTransfertLines != null && ProductTransfertLines.Count > 0)
            {
                ProductTransfertLine toRemove = ProductTransfertLines.SingleOrDefault(pl => pl.TMPID == TMPID);
                ProductTransfertLines.Remove(toRemove);

                Session["ProductTransfertLines"] = ProductTransfertLines;
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
        public ActionResult UpdateProductTransfertLine(int TMPID)
        {
            this.InitializeProductTransfertLineFields(TMPID);
            return this.Direct();
        }

        private void SimpleReset2()
        {
            //this.GetCmp<FormPanel>("FormAddTransferLine").Reset(true);

            this.GetCmp<ComboBox>("ProductNumberID").Value = "";
            this.GetCmp<ComboBox>("ProductID").Value = "";
            this.GetCmp<NumberField>("StockQuantity").Value = "";
            this.GetCmp<NumberField>("LineQuantity").Value = "";
            this.GetCmp<NumberField>("LineUnitPrice").Value = "";

            this.GetCmp<Store>("ProductTransfertLinesStore").Reload();

            ManageCady();
        }

        //Return salelines of current sale
        [HttpPost]
        public ActionResult ProductTransfertLines()
        {
            List<ProductTransfertLine> ProductTransfertLines = (List<ProductTransfertLine>)Session["ProductTransfertLines"];
            List<object> list = new List<object>();

            foreach (ProductTransfertLine pl in ProductTransfertLines)
            {
                list.Add(
                    new
                    {
                        TMPID = pl.TMPID,
                        ProductLabel = (pl.Product is Lens) ? pl.Product.ProductCode : pl.Product.ProductLabel,
                        DepartureLocation = pl.ArrivalLocalization.LocalizationLabel,
                        ArrivalLocation = pl.ArrivalLocalization.LocalizationLabel,
                        LineQuantity = pl.LineQuantity,
                        LineUnitPrice = pl.LineUnitPrice
                    }
                    );
            }
            return this.Store(list);
        }

        [HttpGet]
        public ActionResult UpdateTransfer(int ProductTransfertID)
        {
            this.InitializeProductReceptionFields(ProductTransfertID);
            List<ProductTransfertLine> data = db.ProductTransfertLines.Where(ptl => ptl.ProductTransfertID == ProductTransfertID).ToList();
            List<ProductTransfertLine> ProductTransfertLines = new List<ProductTransfertLine>();
            int i = 0;
            foreach (ProductTransfertLine pl in data)
            {
                pl.TMPID = ++i;
                ProductTransfertLines.Add(pl);
            }
            Session["ProductReceptionID"] = ProductTransfertID;
            this.GetCmp<Button>("btnPrint").Disabled = false;
            Session["ProductTransfertLines"] = ProductTransfertLines;
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
            List<ProductTransfertLine> ProductTransfertLines = new List<ProductTransfertLine>();
            Session["ProductTransfertLines"] = ProductTransfertLines;
            this.GetCmp<FormPanel>("GeneralForm").Reset(true);
            this.GetCmp<Store>("TransmissionGridStoreID").Reload();

        }


        public void InitializeProductReceptionFields(int ID)
        {
            this.SimpleReset();

            if (ID == 0)
            {
                return;
            }

            ProductTransfert transfer = _transfertRepository.Find(ID);
            this.GetCmp<TextField>("ProductTransfertID").Value = transfer.ProductTransfertID;

            this.GetCmp<ComboBox>("DepartureBranchID").Value = transfer.DepartureBranchID;
            //this.GetCmp<ComboBox>("OrderedByID").Value = transfer.OrderedByID;
            this.GetCmp<TextField>("RegisteredByID").Value = transfer.RegisteredByID;

            this.GetCmp<ComboBox>("ArrivalBranchID").Value = transfer.ArrivalBranchID;
            this.GetCmp<ComboBox>("AskedByID").Value = transfer.AskedByID;

            this.GetCmp<TextField>("ProductTransfertReference").Value = transfer.ProductTransfertReference;
            this.GetCmp<DateField>("ReceivedDate").Value = transfer.ReceivedDate;

            this.ManageCady();

        }
        [HttpPost]
        public ActionResult DeleteTransfer(int ProductTransfertID)
        {
            try
            {
                _transfertRepository.CancelTransfert(ProductTransfertID);
                this.AlertSucces(Resources.Success, "Transfer Has Been Successfully Deleted ");
                return this.Reset();

            }
            catch (Exception ex)
            {
                X.Msg.Alert(Resources.er_alert_danger, "Transfer Has Not Been Successfully Deleted Because " + ex.Message + " " + ex.StackTrace).Show();
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
                        DepartureBranchID = busDay.BranchID,
                        BranchName = busDay.Branch.BranchCode
                    }
                    );
            }

            return this.Store(list);

        }
        public ActionResult ChangeBusDay(int? ArrivalBranchID)
        {
            //this.GetCmp<DateField>("ReceivedDate").Reset();

            if (ArrivalBranchID.HasValue && ArrivalBranchID.Value > 0)
            {
                /*lstBusDay = (List<BusinessDay>)Session["UserBusDays"];
                if (lstBusDay == null)
                {
                    lstBusDay = _busDayRepo.GetOpenedBusinessDay(CurrentUser);
                };
                BusinessDay businessDay = lstBusDay.FirstOrDefault(l => l.BranchID == ArrivalBranchID.Value);
                if (businessDay==null)
                {
                    X.Msg.Alert(Resources.er_alert_danger, "The business day of this branch is not open ").Show();
                    return this.Direct();
                }
                this.GetCmp<DateField>("ReceivedDate").SetValue(businessDay.BDDateOperation);  */
                this.InitTrnNumber(ArrivalBranchID.Value);

            }
            return this.Direct();
        }

        public double GetMaxValue(int? ProductID, int? DepartureLocalizationID, int? TMPID)
        {
            double maxvalue = 0;
            if ((DepartureLocalizationID.HasValue && ProductID.HasValue) && (DepartureLocalizationID.Value > 0 && ProductID.Value > 0))
            {
                ProductLocalization prodLoc = db.ProductLocalizations.SingleOrDefault(pl => pl.ProductID == ProductID.Value && pl.LocalizationID == DepartureLocalizationID.Value);
                maxvalue = (prodLoc != null && prodLoc.ProductLocalizationID > 0) ? prodLoc.ProductLocalizationStockQuantity : 0;

                //enlevons les quantités déjà prise pour faire d'autres lignes de transfert
                List<ProductTransfertLine> ProductTransfertLines = (List<ProductTransfertLine>)Session["ProductTransfertLines"];
                if (ProductTransfertLines != null && ProductTransfertLines.Count > 0)
                {
                    double existing = ProductTransfertLines.Where(ptl => ptl.ProductID == ProductID.Value && ptl.DepartureLocalizationID == DepartureLocalizationID.Value).Select(ptl1 => ptl1.LineQuantity).Sum();
                    maxvalue = maxvalue - existing;

                    //en cas de modification de la ligne, il ne faut pas prendre en compte la quantité de la ligne en cours de modification
                    if (TMPID.Value > 0 && TMPID.Value > 0)
                    {
                        double existing2 = ProductTransfertLines.SingleOrDefault(ptl => ptl.TMPID == TMPID.Value).LineQuantity;
                        maxvalue = maxvalue + existing2;
                    }
                }
            }

            return maxvalue;
        }

        public ActionResult ReloadTransferListStore()
        {
            this.GetCmp<Store>("TransmissionGridStoreID").Reload();
            return this.Direct();
        }

        public ActionResult LoadMaxQuantity(int? ProductID, int? DepartureLocalizationID, int? TMPID)
        {
            //this.GetCmp<NumberField>("LineQuantity").Reset();
            this.GetCmp<NumberField>("LineQuantity").SetMaxValue(GetMaxValue(ProductID, DepartureLocalizationID, TMPID));
            return this.Direct();
        }

        public List<object> ModelDeparturePendingTransfer(DateTime BeginDate, DateTime ReceivedDate)
        {
            List<object> list = new List<object>();
               
                List<ProductTransfert> dataTmp = (from pt in db.ProductTransferts
                                                  where (pt.AskedByID.HasValue && pt.AskedByID > 0 && (pt.ReceivedDate.Value >= BeginDate.Date && pt.ReceivedDate.Value <= ReceivedDate.Date))
                                                      select pt).ToList();

                if (dataTmp != null && dataTmp.Count > 0)
                {

                    foreach (ProductTransfert pt in dataTmp)
                    {
                        list.Add(
                            new
                            {
                                ProductTransfertID = pt.ProductTransfertID,
                                ProductTransfertReference = pt.ProductTransfertReference,
                                ReceivedDate = pt.ReceivedDate,
                                DepartureBranch = pt.DepartureBranch.BranchName,
                                ArrivalBranch = pt.ArrivalBranch.BranchName,
                                //OrderedBy = pt.OrderedBy.UserFullName,
                                AskedBy = (pt.AskedBy != null) ? pt.AskedBy.UserFullName : "",
                                RegisteredBy = pt.RegisteredBy.UserFullName,
                            }
                           );
                    }

                }

                
            //}

            return list;
        }
        [HttpPost]
        public StoreResult GetAllDeparturePendingTransfers(DateTime BeginDate, DateTime ReceptionDate)
        {
            return this.Store(ModelDeparturePendingTransfer(BeginDate, ReceptionDate));
        }

        public StoreResult GetAllArrivalLocations(int? ArrivalBranchID)
        {
            List<object> list = new List<object>();

            if (ArrivalBranchID.HasValue && ArrivalBranchID.Value > 0)
            {

                
                List<Localization> dataTmp = (from loc in db.Localizations
                                              where (loc.BranchID == ArrivalBranchID.Value)
                                              select loc).ToList();
                
                foreach (Localization pt in dataTmp)
                {
                    list.Add(
                        new
                        {
                            LocalizationID = pt.LocalizationID,
                            LocalizationLabel = pt.LocalizationLabel,
                        }
                       );
                }

            }

            return this.Store(list);
        }
        /// <summary>
        /// Liste des magasins qui sont approvisionnés dans une branche
        /// </summary>
        /// <param name="DepartureBranchID"></param>
        /// <returns></returns>
        public StoreResult GetAllStockedLocations(int? DepartureBranchID)
        {
            List<object> list = new List<object>();
           
           if (DepartureBranchID.Value >0)
           {
               List<Localization> dataTmp = (from loc in db.Localizations
                                             where (loc.BranchID == DepartureBranchID.Value)
                                             select loc).ToList();
               if (dataTmp.Count > 0)
               {
                   foreach (Localization pt
                       in
                       dataTmp)
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



        public void InitializeProductTransfertLineFields(int ID)
        {
            Session["isUpdate"] = true;
            
            this.SimpleReset2();

            List<ProductTransfertLine> ProductTransfertLines = (List<ProductTransfertLine>)Session["ProductTransfertLines"];

            if (ID > 0)
            {
                ProductTransfertLine ptl = new ProductTransfertLine();
                ptl = ProductTransfertLines.SingleOrDefault(pl => pl.TMPID == ID);

                this.GetCmp<TextField>("ProductTransfertLineID").Value = ptl.ProductTransfertLineID;
                this.GetCmp<TextField>("TMPID").Value = ptl.TMPID;

                //this.GetCmp<ComboBox>("DepartureLocalizationID").GetStore().Reload();
                //this.GetCmp<ComboBox>("DepartureLocalizationID").Value = ptl.DepartureLocalizationID;
                
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

                double qtystock = GetMaxQtyInStock(ptl.ProductID, ptl.DepartureLocalizationID.Value);
                this.GetCmp<NumberField>("StockQuantity").Value = qtystock;
                Session["MaxValue"] = qtystock;

                this.GetCmp<ComboBox>("LineQuantity").Value = ptl.LineQuantity;
                this.GetCmp<ComboBox>("LineUnitPrice").Value = ptl.LineUnitPrice;

                this.GetCmp<ComboBox>("ArrivalLocalizationID").GetStore().Reload();
                this.GetCmp<ComboBox>("ArrivalLocalizationID").Value = ptl.ArrivalLocalizationID.Value;
            }
            ManageCady();

        }

        public double GetMaxQtyInStock(int productID, int localizationID)
        {
            double res = 0;

            //res = _productLocalizationRepository.FindAll.SingleOrDefault(pl => pl.ProductID == productID && pl.LocalizationID == localizationID).ProductLocalizationStockQuantity;

            
            res = db.ProductLocalizations.SingleOrDefault(pl => pl.ProductID == productID && pl.LocalizationID == localizationID).ProductLocalizationStockQuantity;

            return res;
        }


        public void ManageCady()
        {
            List<ProductTransfertLine> ProductTransfertLines = (List<ProductTransfertLine>)Session["ProductTransfertLines"];

            if (ProductTransfertLines != null && ProductTransfertLines.Count > 0)
            {
                this.GetCmp<TextField>("IsCadyEmpty").SetValue(0);//faux
            }
            if (ProductTransfertLines == null || ProductTransfertLines.Count == 0)
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
                    int lenNumber = GetLensID(Localization.Value, ProductCategoryID.Value, ProductNumberID.Value);
                    if (lenNumber>0)
                    {
                        this.GetCmp<ComboBox>("ProductID").SetValueAndFireSelect(lenNumber);
                    }
                    else
                    {
                        X.Msg.Alert(Resources.Productlabel, "This Product does not exist in that Location").Show();
                    }
                    
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
                            ProductLocalizationStockSellingPrice = s.pl.ProductLocalizationStockSellingPrice
                        }).FirstOrDefault();

            if (lstLensProduct != null)
            {
                Session["MaxValue"] = lstLensProduct.ProductQuantity;
                Session["LineUnitPrice"] = lstLensProduct.ProductLocalizationStockSellingPrice;
                return lstLensProduct.ProductID;
            }
            
            else
            {
                X.Msg.Alert("Get LensID", "There is no lens product under that location");
                return 0;
            }
            

        }

        [HttpPost]
        public ActionResult GetAllProducts(int? DepartureLocalizationID, int? ProductCategoryID, int? ProductNumberID)
        {

            try
            {
                List<Product> list = ModelProductLocalCat(DepartureLocalizationID, ProductCategoryID, ProductNumberID);

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

        public List<Product> ModelProductLocalCat(int? DepartureLocalizationID, int? ProductCategoryID, int? ProductNumberID)
        {
            List<Product> model = new List<Product>();
            

            //On a un produit générique
            if ( (DepartureLocalizationID == null || ProductCategoryID.Value == 0) && (ProductCategoryID == null || ProductCategoryID.Value == 0) && (ProductNumberID == null || ProductNumberID.Value == 0)) //chargement des produits en fct du magasin slt
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
                        .Where(lsp => lsp.pl.LocalizationID == DepartureLocalizationID && lsp.p.CategoryID == ProductCategoryID && !(lsp.p.Category is LensCategory))
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
        public ActionResult PrintTransferReceipt()
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
            int ProductReceptionID = (int)Session["ProductReceptionID"];
            
            string repName = "";
            bool isValid = false;
            double totalAmount = 0d;
            double totalRemaining = 0d;

            
            string path = "";
            string defaultImgPath = Server.MapPath("~/Content/Images/App/deboy-logo.png");
            byte[] COMPANY_LOGO = System.IO.File.ReadAllBytes(defaultImgPath);

            string DeviseLabel = db.Devises.Where(d => d.DefaultDevise).FirstOrDefault().DeviseLabel;
            ProductTransfert productTransfert = (from pt in db.ProductTransferts
                                                 where pt.ProductTransfertID == ProductReceptionID
                                   select pt).SingleOrDefault();
                //_transfertRepository.FindAll.Where(pt => pt.ProductTransfertID == ProductReceptionID).SingleOrDefault();
            //Branch curBranch = new Branch();
            
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
            db.ProductTransfertLines.Where(l => l.ProductTransfertID == ProductReceptionID).ToList().ForEach(c =>
            {
                isValid = true;
                saleAmount += c.LineUnitPrice;
                model.Add(
                                new
                                {
                                    ReceiveAmount = 0,
                                    LineQuantity = c.LineQuantity,
                                    LineUnitPrice = c.LineUnitPrice,
                                    ProductLabel = c.Product.ProductLabel,
                                    ProductRef = c.Product.ProductCode,//(c.OeilDroiteGauche == EyeSide.N) ? c.Product.ProductCode : c.OeilDroiteGauche + ":" + c.Product.ProductCode,
                                    CompanyName = Company.Name,
                                    CompanyAdress = "Head Quater : " + Company.Adress.Quarter.Town.Region.RegionLabel + " - " + Company.Adress.Quarter.Town.TownLabel,
                                    CompanyTel = "Tel: " + Company.Adress.AdressPhoneNumber,
                                    BranchName = curBranch.Branch.BranchName,
                                    BranchAdress = curBranch.Branch.Adress.Quarter.QuarterLabel + " - " + curBranch.Branch.Adress.Quarter.Town.TownLabel,
                                    BranchTel = "Tel: " + curBranch.Branch.Adress.AdressPhoneNumber,
                                    Ref = productTransfert.ProductTransfertReference,
                                    CompanyCNI = "NO CONT : " + Company.CNI,
                                    Operator = CurrentUser.Name + " " + CurrentUser.Description,
                                    SendindBranchName = productTransfert.DepartureBranch.BranchName,
                                    SendindBranchCode = productTransfert.DepartureBranch.BranchCode,
                                    TransfertDate = productTransfert.ReceivedDate.Value.Date,
                                    Title = "Product Transfert lines informations",
                                    DeviseLabel = DeviseLabel,
                                    ReceivingBranchCode = productTransfert.ArrivalBranch.BranchCode,
                                    ReceivingBranchName = productTransfert.ArrivalBranch.BranchName,
                                    CompanyLogo = _fileRepository.FindAll.FirstOrDefault(f => f.GlobalPersonID == Company.GlobalPersonID) != null ? _fileRepository.FindAll.FirstOrDefault(f => f.GlobalPersonID == Company.GlobalPersonID).Content : COMPANY_LOGO
                                }
                        );
            }
                    );
                
            if (isValid)
            {
                path = Server.MapPath("~/Reports/Supply/RptReceiptTransfert.rpt");
                repName = "RptReceiptTransfert";
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