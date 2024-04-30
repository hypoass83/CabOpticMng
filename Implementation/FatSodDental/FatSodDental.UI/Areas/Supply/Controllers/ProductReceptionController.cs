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
using System.Web.UI;
using FatSod.DataContext.Concrete;

namespace FatSodDental.UI.Areas.Supply.Controllers
{
    [Authorize(Order = 1)]
    [TakeBusinessDay(Order = 2)]
    public partial class ProductReceptionController : BaseController
    {
        //Current Controller and current page
        private const string CONTROLLER_NAME = "Supply/" + CodeValue.Supply.ProductReception_SM.CONTROLLER;
        private const string VIEW_NAME = CodeValue.Supply.ProductReception_SM.PATH;
        private ITransfert _transfertRepository;
        private IBusinessDay _busDayRepo;
        private IRepositorySupply<Localization> _localizationRepository;

        List<BusinessDay> lstBusDay;
        public ProductReceptionController(ITransfert transRepo, 
            IBusinessDay bDRepo,
            IRepositorySupply<Localization> locRepo

            )
    {
            this._transfertRepository = transRepo;
            this._busDayRepo = bDRepo;
            this._localizationRepository = locRepo;
            
        }
        //[OutputCache(Duration = 3600)] 
        public ActionResult ProductReception()
        {

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

            //return View(ModelDeparturePendingTransfer(0));
            //Ext.Net.MVC.PartialViewResult rPVResult = new Ext.Net.MVC.PartialViewResult
            //{
            //    ClearContainer = true,
            //    //RenderMode = RenderMode.AddTo,
            //    ContainerId = "Body",
            //    //WrapByScriptTag = false,
            //    Model = ModelDeparturePendingTransfer(0)
            //};

            return View(ModelDeparturePendingTransfer(0));
        }

        [HttpPost]
        public ActionResult AddManager(ProductTransfert transfer, int ArrivalLocalizationID)
        {
            List<ProductTransfertLine> ProductTransfertLines = new List<ProductTransfertLine>();
            ProductTransfertLines = (List<ProductTransfertLine>)Session["ProductTransfertLines"];
            transfer.ProductTransfertLines = ProductTransfertLines;

           
            if (transfer.ProductTransfertID > 0)
            {
                return ValidateTransfer(transfer);
            }

            X.Msg.Alert(Resources.er_alert_danger, "ProductTranfertID can not be negative ").Show();
            return this.Direct();

        }

        [HttpPost]
        public ActionResult ValidateTransfer(ProductTransfert transfer)
        {
            try
            {

                _transfertRepository.ValidateTransfert(transfer,SessionGlobalPersonID);
                this.AlertSucces(Resources.Success, "Products Have Been Successfully Received ");
                return this.Reset();
            }
            catch (Exception ex)
            {

                X.Msg.Alert(Resources.er_alert_danger, "Products Have not Been Successfully Received because " + ex.Message + " " + ex.StackTrace).Show();
                return this.Direct();
            }
        }




        private void SimpleReset2()
        {

            this.GetCmp<Store>("ProductTransfertLinesStore").Reload();

            ManageCady();
            }

        public StoreResult GetAllArrivalLocations(int? ArrivalBranchID)
            {
            List<object> list = new List<object>();

            if (ArrivalBranchID.HasValue && ArrivalBranchID.Value > 0)
                    {
                List<Localization> dataTmp = _localizationRepository.FindAll.Where(pl => pl.BranchID == ArrivalBranchID.Value).ToList();

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
                        DepartureLocation = pl.DepartureLocalization.LocalizationLabel,
                        //ArrivalLocation = pl.ArrivalLocalization.LocalizationLabel,
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
            this.InitializeProductTransmissionFields(ProductTransfertID);
            List<ProductTransfertLine> data = db.ProductTransfertLines.Where(ptl => ptl.ProductTransfertID == ProductTransfertID).ToList();
            List<ProductTransfertLine> ProductTransfertLines = new List<ProductTransfertLine>();
            int i = 0;
            foreach (ProductTransfertLine pl in data)
            {
                pl.TMPID = ++i;
                ProductTransfertLines.Add(pl);
            }

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


        public void InitializeProductTransmissionFields(int ID)
        {
            this.SimpleReset();

            if (ID == 0)
            {
                return;
            }

            ProductTransfert transfer = _transfertRepository.Find(ID);
            this.GetCmp<TextField>("ProductTransfertID").Value = transfer.ProductTransfertID;

            this.GetCmp<TextField>("DepartureBranchID").Value = transfer.DepartureBranch.BranchName;
            this.GetCmp<TextField>("DepartureBranchID").ReadOnly = true;

            this.GetCmp<TextField>("OrderedByID").Value = transfer.OrderedBy.UserFullName;
            this.GetCmp<TextField>("OrderedByID").ReadOnly = true;

            this.GetCmp<TextField>("RegisteredByID").Value = transfer.RegisteredBy.UserFullName;
            this.GetCmp<TextField>("RegisteredByID").ReadOnly = true;

            this.GetCmp<TextField>("ArrivalBranchID").Value = transfer.ArrivalBranchID;

            this.GetCmp<TextField>("ArrivalBranchName").Value = transfer.ArrivalBranch.BranchName;
            this.GetCmp<TextField>("ArrivalBranchName").ReadOnly = true;

            this.GetCmp<TextField>("AskedByID").Value = transfer.AskedBy.UserFullName;
            this.GetCmp<TextField>("AskedByID").ReadOnly = true;


            this.GetCmp<TextField>("ProductTransfertReference").Value = transfer.ProductTransfertReference;
            this.GetCmp<TextField>("ProductTransfertReference").ReadOnly = true;

            this.GetCmp<DateField>("ProductTransfertDate").Value = transfer.ProductTransfertDate;
            this.GetCmp<DateField>("ProductTransfertDate").ReadOnly = true;

            try 
	        {	        
		        DateTime ReceivedDate = _busDayRepo.GetOpenedBusinessDay(/*transfer.ArrivalBranch*/).FirstOrDefault().BDDateOperation;
                this.GetCmp<DateField>("ReceivedDate").Value = ReceivedDate;
                this.GetCmp<DateField>("ReceivedDate").ReadOnly = true;

	        }
	        catch (NullReferenceException ex)
	        {
		
		        throw new Exception("Business Day Of Arrival Branch " + transfer.ArrivalBranch.BranchName + " is not yet Open. See Administrator" );
	        }

            this.GetCmp<Store>("ArrivallLocationStoreID").Reload();

          
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
                        BranchName = busDay.BranchName
                    }
                    );
            }

            return this.Store(list);

        }
        public ActionResult ChangeBusDay(int? DepartureBranchID)
        {
            this.GetCmp<DateField>("ProductTransfertDate").Reset();

            if (DepartureBranchID.HasValue && DepartureBranchID > 0)
            {
                lstBusDay = (List<BusinessDay>)Session["UserBusDays"];
                if (lstBusDay == null)
                {
                    lstBusDay = _busDayRepo.GetOpenedBusinessDay(CurrentUser);
                };
                BusinessDay businessDay = lstBusDay.FirstOrDefault(l => l.BranchID == DepartureBranchID.Value);

                this.GetCmp<DateField>("ProductTransfertDate").SetValue(businessDay.BDDateOperation);
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

        public ActionResult LoadMaxQuantity(int? ProductID, int? DepartureLocalizationID, int? TMPID)
        {
            //this.GetCmp<NumberField>("LineQuantity").Reset();
            this.GetCmp<NumberField>("LineQuantity").SetMaxValue(GetMaxValue(ProductID, DepartureLocalizationID, TMPID));
            return this.Direct();
        }

        public List<object> ModelDeparturePendingTransfer(int? DepartureBranchID)
        {
            List<object> list = new List<object>();
            if (/*DepartureBranchID.HasValue && DepartureBranchID.Value > 0*/true == true)
            {
                
                foreach (ProductTransfert pt in _transfertRepository.FindAll.Where(pt => pt.IsReceived == false).ToList())
                {
                    list.Add(
                        new
                        {
                            ProductTransfertID = pt.ProductTransfertID,
                            ProductTransfertReference = pt.ProductTransfertReference,
                            ProductTransfertDate = pt.ProductTransfertDate,
                            DepartureBranch = pt.DepartureBranch.BranchName,
                            ArrivalBranch = pt.ArrivalBranch.BranchName,
                            OrderedBy = pt.OrderedBy.UserFullName,
                            AskedBy = pt.AskedBy.UserFullName,
                            RegisteredBy = pt.RegisteredBy.UserFullName,
                        }
                       );
                }
            }

            return list;
        }
        [HttpPost]
        public StoreResult GetAllDeparturePendingTransfers(int? DepartureBranchID)
        {
            return this.Store(ModelDeparturePendingTransfer(DepartureBranchID));
        }

        /// <summary>
        /// Liste des magasins qui sont approvisionnés dans une branche
        /// </summary>
        /// <param name="DepartureBranchID"></param>
        /// <returns></returns>
        public StoreResult GetAllStockedLocations(int? DepartureBranchID)
        {
            List<object> list = new List<object>();
            IEqualityComparer<ProductLocalization> locationComparer = new GenericComparer<ProductLocalization>("LocationCode");


            foreach (ProductLocalization pt in db.ProductLocalizations.Where(pl => pl.Localization.BranchID == DepartureBranchID.Value && pl.ProductLocalizationStockQuantity > 0).Distinct(locationComparer).ToList())
            {
                list.Add(new
                {
                    LocalizationID = pt.LocalizationID,
                    LocalizationLabel = pt.LocalizationLabel,
                });
            }

            return this.Store(list);
        }

        public ActionResult GetAllProducts(int start, int limit, int page, string query, int? DepartureLocalizationID)
        {

            Paging<object> products = ProductsPaging(start, limit, "ProductCode", "ASC", query, DepartureLocalizationID);


            return this.Store(products.Data, products.TotalRecords);
        }

        public Paging<object> ProductsPaging(int start, int limit, string sort, string dir, string filter, int? DepartureLocalizationID)
        {
            List<Product> list = new List<Product>();

            if (DepartureLocalizationID.HasValue && DepartureLocalizationID.Value > 0)
            {
                IEqualityComparer<ProductLocalization> locationComparer = new GenericComparer<ProductLocalization>("ProductCode");


                foreach (ProductLocalization pt in db.ProductLocalizations.Where(pl => pl.Localization.LocalizationID == DepartureLocalizationID.Value && pl.ProductLocalizationStockQuantity > 0).Distinct(locationComparer).ToList())
                {
                    list.Add(
                        new Product
                        {
                            ProductID = pt.ProductID,
                            ProductLabel = pt.Product.GetProductCode(),
                        }
                       );
                }

            }

            List<Product> products = list;

            //selection en fonction de ce qui a été saisie
            if (!string.IsNullOrEmpty(filter) && filter != "*")
            {
                products.RemoveAll(p => !p.GetProductCode().ToLower().Contains(filter.ToLower()));
            }

            //ordonné
            if (!string.IsNullOrEmpty(sort))
            {
                products.Sort(delegate(Product x, Product y)
                {
                    object a;
                    object b;

                    int direction = dir == "DESC" ? -1 : 1;

                    a = x.GetType().GetProperty(sort).GetValue(x, null);
                    b = y.GetType().GetProperty(sort).GetValue(y, null);

                    return CaseInsensitiveComparer.Default.Compare(a, b) * direction;
                });
            }

            //quantité pour la page
            if ((start + limit) > products.Count)
            {
                limit = products.Count - start;
            }

            List<Product> rangeProducts = (start < 0 || limit < 0) ? products : products.GetRange(start, limit);

            List<object> rangeObjects = new List<object>();

            foreach (Product pt in rangeProducts)
            {
                rangeObjects.Add(
                    new
                    {
                        ProductID = pt.ProductID,
                        ProductLabel = pt.GetProductCode(),
                    }
                   );
            }

            return new Paging<object>(rangeObjects, products.Count);

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


}
}