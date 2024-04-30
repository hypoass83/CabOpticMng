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

namespace FatSodDental.UI.Areas.Supply.Controllers
{
    [Authorize(Order = 1)]
    [TakeBusinessDay(Order = 2)]
    public class SupplierOrderController : BaseController
    {
        //Current Controller and current page
        private const string CONTROLLER_NAME = "Supply/" + CodeValue.Supply.SupplierOrderMenu.CONTROLLER;
        private const string VIEW_NAME = CodeValue.Supply.SupplierOrderMenu.PATH;
        //person repository

        private ISupplierOrder _supplierOrderRepository;
        private IBusinessDay _busDayRepo;

        private IRepositorySupply<TillDay> _tillDayRepository;


        private ITransactNumber _transactNumbeRepository;
        IInventoryDirectory _invDirRepo;

        private List<BusinessDay> lstBusDay;

        //Construcitor
        public SupplierOrderController(
            IBusinessDay busDayRepo,
            ISupplierOrder supplierOrderRepository, 
            ITransactNumber transactNumbeRepository,
            IRepositorySupply<TillDay> tillDayRepository, IInventoryDirectory idRepo
            )
        {
            this._supplierOrderRepository = supplierOrderRepository;
            this._tillDayRepository = tillDayRepository;
            this._busDayRepo = busDayRepo;
            this._transactNumbeRepository = transactNumbeRepository;
            this._invDirRepo = idRepo;
            
        }
        // GET: Sale/Sale Return all a sales historique add enable to create a new sale
        [OutputCache(Duration = 3600)] 
        public ActionResult SupplierOrder()
        {
            //Session["Curent_Page"] = VIEW_NAME;
            //Session["Curent_Controller"] = CONTROLLER_NAME;

            //We test if this user has an autorization to get this sub menu
            //if (!LoadAction.isAutoriseToGetMenu(SessionProfileID, CodeValue.Supply.SupplierReturnMenu.CODE, db))
            //{
            //    RedirectToAction("NotAuthorized", "User");
            //}

            //this.GetCmp<Panel>("Body").LoadContent(new ComponentLoader
            //{
            //    Url = Url.Action(VIEW_NAME),
            //    DisableCaching = false,
            //    Mode = LoadMode.Frame
            //});
            //List<SupplierOrder> dataTmp = _supplierOrderRepository.FindAll.ToList();

            List<SupplierOrderLine> SupplierOrderLines = new List<SupplierOrderLine>();
            Session["SupplierOrderLines"] = SupplierOrderLines;

            return View(ModelSupOder());

            //Ext.Net.MVC.PartialViewResult rPVResult = new Ext.Net.MVC.PartialViewResult
            //{
            //    ClearContainer = true,
            //    //RenderMode = RenderMode.AddTo,
            //    ContainerId = "Body",
            //    //WrapByScriptTag = false,
            //    Model = ModelSupOder()
            //};
            //return rPVResult;
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
                string trnnum = _transactNumbeRepository.displayTransactNumber("PURC", businessDay);
                this.GetCmp<TextField>("SupplierOrderReference").Value = trnnum;
            }
            return this.Direct();
        }
        [HttpPost]
        public ActionResult AddSupplierOrderLine(SupplierOrderLine supplierOrderLine, double reduction = 0, double discount = 0, double transport = 0)
        {
            supplierOrderLine.Product = db.Products.Find(supplierOrderLine.ProductID);
            supplierOrderLine.Localization = db.Localizations.Find(supplierOrderLine.LocalizationID);

            List<SupplierOrderLine> SupplierOrderLines = (List<SupplierOrderLine>)Session["SupplierOrderLines"];

            //il s'agit d'une modification alors on fait un drop and create
            if (supplierOrderLine.TMPID > 0)
            {
                SupplierOrderLine toRemove = SupplierOrderLines.SingleOrDefault(pl => pl.TMPID == supplierOrderLine.TMPID);
                SupplierOrderLines.Remove(toRemove);
            }

            //alors la variable de session n'était pas vide
            if (SupplierOrderLines != null && SupplierOrderLines.Count > 0)
            {
                //c'est un nouvel ajout dans le panier
                if (supplierOrderLine.TMPID == 0)
                {
                    //existe t-il déjà une ligne de vente ayant le meme produit et le même magasin que celui en création?
                    SupplierOrderLine existing = SupplierOrderLines.SingleOrDefault(pl => pl.ProductID == supplierOrderLine.ProductID && pl.LocalizationID == supplierOrderLine.LocalizationID);

                    if (existing != null && existing.TMPID > 0)
                    {
                        //la quantité est la somme des deux quantité
                        supplierOrderLine.LineQuantity += existing.LineQuantity;
                        //le prix c'est le prix de la nouvelle ligne
                        //l'id c'est l'id de la ligne existante
                        supplierOrderLine.TMPID = existing.TMPID;
                        supplierOrderLine.LineID = existing.LineID;
                        //on retire l'ancien pour ajouter le nouveau
                        SupplierOrderLines.Remove(existing);
                    }

                    if (existing == null || existing.TMPID == 0)
                    {
                        supplierOrderLine.TMPID = SupplierOrderLines.Select(pl => pl.TMPID).Max() + 1;
                    }
                }
                SupplierOrderLines.Add(supplierOrderLine);
            }

            //alors la variable de session était vide
            if (SupplierOrderLines == null || SupplierOrderLines.Count == 0)
            {
                //c'est bon pour la création mais pas pour les modifications
                SupplierOrderLines = new List<SupplierOrderLine>();
                if (supplierOrderLine.TMPID == 0)
                {
                    supplierOrderLine.TMPID = 1;
                }
                SupplierOrderLines.Add(supplierOrderLine);
            }

            double VatRate = CodeValue.Accounting.ParamInitAcct.VATRATE;

            ApplyExtraPrices(SupplierOrderLines, reduction, discount, transport, VatRate);

            Session["SupplierOrderLines"] = SupplierOrderLines;
            return this.Reset2();
        }

        public void ApplyExtraPrices(List<SupplierOrderLine> SupplierOrderLines, double reduction, double discount, double transport, double vatRate)
        {

            double valueOperation = SupplierOrderLines.Select(l => l.LineAmount).Sum();
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
            //else
            //    this.GetCmp<FieldSet>("OperationAmount").Disabled = true;

        }

        [HttpPost]
        public ActionResult AddSupplierOrder(SupplierOrder supplierOrder)
        {

            List<SupplierOrderLine> SupplierOrderLines = (List<SupplierOrderLine>)Session["SupplierOrderLines"];

            supplierOrder.SupplierOrderLines = SupplierOrderLines;

            if (supplierOrder.SupplierOrderID > 0)
            {
                _supplierOrderRepository.UpdateSupplierOrder(supplierOrder);
            }

            if (supplierOrder.SupplierOrderID == 0)
            {
                _supplierOrderRepository.CreateSupplierOrder(supplierOrder);
            }
            Session["SupplierOrderLines"] = new List<SupplierOrderLine>();
            this.Reset();
            return this.Direct();
        }

        [HttpPost]
        public ActionResult UpdateSupplierOrder(int SupplierOrderID)
        {
            List<SupplierOrderLine> SupplierOrderLines = db.SupplierOrderLines.Where(sol => sol.SupplierOrderID == SupplierOrderID).ToList();
            List<SupplierOrderLine> realSupplierOrderLines = new List<SupplierOrderLine>();
            int i = 0;
            foreach (SupplierOrderLine pl in SupplierOrderLines)
            {
                pl.TMPID = ++i;
                realSupplierOrderLines.Add(pl);
            }
            Session["SupplierOrderLines"] = realSupplierOrderLines;
            this.InitializeSupplierOrderFields(SupplierOrderID);

            return this.Direct();
        }

        [HttpPost]
        public ActionResult UpdateSupplierOrderLine(int TMPID)
        {

            this.InitializeSupplierOrderLineFields(TMPID);

            return this.Direct();
        }

        [HttpPost]
        public ActionResult RemoveSupplierOrderLine(int TMPID, double reduction = 0, double discount = 0, double transport = 0, double VatRate = 0)
        {
            //lors de la création
            List<SupplierOrderLine> SupplierOrderLines = (List<SupplierOrderLine>)Session["SupplierOrderLines"];

            if (SupplierOrderLines != null && SupplierOrderLines.Count > 0)
            {
                SupplierOrderLine toRemove = SupplierOrderLines.SingleOrDefault(pl => pl.TMPID == TMPID);
                SupplierOrderLines.Remove(toRemove);
                Session["SupplierOrderLines"] = SupplierOrderLines;
                ApplyExtraPrices(SupplierOrderLines, reduction, discount, transport, VatRate);
            }

            return this.Reset2();
        }

        [HttpPost]
        public ActionResult Reset()
        {
            this.GetCmp<FormPanel>("SupplierOrderForm").Reset(true);
            this.GetCmp<FormPanel>("FormAddSupplierOrderLine").Reset(true);
            this.GetCmp<FormPanel>("GeneralForm").Reset(true);

            this.GetCmp<Store>("SupplierOrderLinesStore").Reload();
            this.GetCmp<Store>("SupplierOrderListStore").Reload();

            //this.GetCmp<FieldSet>("OperationAmount").Disabled = true;

            Session["SupplierOrderLines"] = new List<SupplierOrderLine>();
            return this.Direct();
        }

        public ActionResult Reset2()
        {
            this.GetCmp<FormPanel>("FormAddSupplierOrderLine").Reset(true);

            this.GetCmp<Store>("SupplierOrderLinesStore").Reload();

            ManageCady();
            return this.Direct();
        }

        public void InitializeSupplierOrderFields(int ID)
        {

            this.GetCmp<FormPanel>("SupplierOrderForm").Reset(true);
            this.GetCmp<Store>("SupplierOrderListStore").Reload();

            this.GetCmp<Store>("SupplierOrderLinesStore").Reload();

            if (ID > 0)
            {
                SupplierOrder supplierOrder = new SupplierOrder();
                supplierOrder = _supplierOrderRepository.Find(ID);

                this.GetCmp<TextField>("SupplierOrderID").Value = supplierOrder.SupplierOrderID;
                if (supplierOrder.SupplierID > 0)
                {
                    this.GetCmp<ComboBox>("SupplierID").SetValue(supplierOrder.SupplierID);
                }

                if (supplierOrder.BranchID > 0)
                {
                    this.GetCmp<ComboBox>("BranchID").Value = (supplierOrder.BranchID);
                }

                if (supplierOrder.DeviseID > 0)
                {
                    this.GetCmp<ComboBox>("DeviseID").SetValue(supplierOrder.DeviseID);
                }

                this.GetCmp<DateField>("SupplierOrderDate").Value = supplierOrder.SupplierOrderDate;
                this.GetCmp<TextField>("SupplierOrderReference").Value = supplierOrder.SupplierOrderReference;
                this.ApplyExtraPrices(supplierOrder.SupplierOrderLines.ToList(), supplierOrder.RateReduction, supplierOrder.RateDiscount, supplierOrder.Transport, supplierOrder.VatRate);
                this.ManageCady();
            }
        }

        public void InitializeSupplierOrderLineFields(int ID)
        {

            this.GetCmp<FormPanel>("FormAddSupplierOrderLine").Reset(true);
            this.GetCmp<Store>("SupplierOrderLinesStore").Reload();

            List<SupplierOrderLine> SupplierOrderLines = (List<SupplierOrderLine>)Session["SupplierOrderLines"];


            if (ID > 0)
            {
                SupplierOrderLine SupplierOrderLine = new SupplierOrderLine();
                SupplierOrderLine = SupplierOrderLines.SingleOrDefault(pl => pl.TMPID == ID);

                this.GetCmp<TextField>("TMPID").SetValue(SupplierOrderLine.TMPID);
                this.GetCmp<TextField>("LineID").SetValue(SupplierOrderLine.LineID);
                this.GetCmp<ComboBox>("LocalizationID").SetValueAndFireSelect(SupplierOrderLine.LocalizationID);
                this.GetCmp<ComboBox>("ProductID").Value=SupplierOrderLine.ProductID;
                this.GetCmp<NumberField>("LineQuantity").Value = SupplierOrderLine.LineQuantity;
                this.GetCmp<NumberField>("LineUnitPrice").Value = SupplierOrderLine.LineUnitPrice;

                if (SupplierOrderLine.SupplierOrderID > 0)
                {
                    this.InitializeSupplierOrderFields(SupplierOrderLine.SupplierOrderID);
                }

            }

            ManageCady();

        }

        public void ManageCady()
        {
            List<SupplierOrderLine> SupplierOrderLines = (List<SupplierOrderLine>)Session["SupplierOrderLines"];

            if (SupplierOrderLines != null && SupplierOrderLines.Count > 0)
            {
                this.GetCmp<TextField>("IsCadyEmpty").SetValue(0);//faux
            }
            if (SupplierOrderLines == null || SupplierOrderLines.Count == 0)
            {
                this.GetCmp<TextField>("IsCadyEmpty").SetValue(1);//vrai
            }
        }


        //Return salelines of current sale
        [HttpPost]
        public ActionResult SupplierOrderLines()
        {
            List<SupplierOrderLine> SupplierOrderLines = (List<SupplierOrderLine>)Session["SupplierOrderLines"];

            List<object> list = new List<object>();

            foreach (SupplierOrderLine pl in SupplierOrderLines)
            {
                list.Add(
                    new
                    {
                        TMPID = pl.TMPID,
                        ProductLabel = pl.ProductLabel,
                        LocalizationLabel = pl.LocalizationLabel,
                        LineUnitPrice = pl.LineUnitPrice,
                        LineQuantity = pl.LineQuantity,
                        LineAmount = pl.LineAmount
                    }
                    );
            }
            return this.Store(list);
        }

        [HttpPost]
        public ActionResult DeleteSupplierOrder(int SupplierOrderID)
        {
            _supplierOrderRepository.DeleteSupplierOrder(SupplierOrderID);
            Session["SupplierOrderLines"] = new List<SupplierOrderLine>();
            this.Reset();

            return this.Direct();
        }
        public List <object> ModelSupOder()
        {
            List<SupplierOrder> dataTmp = _supplierOrderRepository.FindAll.Where(so => so.IsDelivered == false).ToList();

            List<object> list = new List<object>();

            foreach (SupplierOrder p in dataTmp)
            {
                p.SupplierOrderTotalAmount = p.SupplierOrderLines.Select(pl2 => pl2.LineAmount).Sum();
                list.Add(
                    new
                    {
                        SupplierOrderID = p.SupplierOrderID,
                        SupplierOrderReference = p.SupplierOrderReference,
                        SupplierOrderDate = p.SupplierOrderDate,
                        SupplierFullName = p.SupplierFullName,
                        SupplierEmail = p.SupplierEmail,
                        SupplierPhoneNumber = p.SupplierPhoneNumber,
                        SupplierOrderTotalAmount = p.SupplierOrderTotalAmount

                    }
                   );
            }

            return list;
        }
        [HttpPost]
        public StoreResult GetAllSupplierOrders()
        {
            //une commande qui a déjà été validée ne peut plus être modifiée ou supprimée
            /*List<SupplierOrder> dataTmp = _supplierOrderRepository.FindAll.Where(so => so.IsDelivered == false).ToList();
            
            List<object> list = new List<object>();

            foreach (SupplierOrder p in dataTmp)
            {
                p.SupplierOrderTotalAmount = p.SupplierOrderLines.Select(pl2 => pl2.LineAmount).Sum();
                list.Add(
                    new
                    {
                        SupplierOrderID = p.SupplierOrderID,
                        SupplierOrderReference = p.SupplierOrderReference,
                        SupplierOrderDate = p.SupplierOrderDate,
                        SupplierFullName = p.SupplierFullName,
                        SupplierEmail = p.SupplierEmail,
                        SupplierPhoneNumber = p.SupplierPhoneNumber,
                        SupplierOrderTotalAmount = p.SupplierOrderTotalAmount

                    }
                   );
            }
            */
            return this.Store(ModelSupOder());
        }

        public ActionResult ChangeBusDay(int? BranchID)
        {
            this.GetCmp<DateField>("SupplierOrderDate").Reset();
            if (BranchID > 0)
            {
                lstBusDay = (List<BusinessDay>)Session["UserBusDays"];
                if (lstBusDay == null)
                {
                    lstBusDay = _busDayRepo.GetOpenedBusinessDay(CurrentUser);
                };
                BusinessDay businessDay = lstBusDay.FirstOrDefault(l => l.BranchID == BranchID.Value);
                this.GetCmp<DateField>("SupplierOrderDate").SetValue(businessDay.BDDateOperation);
            }
            return this.Direct();
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
                        BranchName = busDay.BranchName
                    }
                    );
            }
            return this.Store(list);
        }

        public ActionResult GetAllProducts(int start, int limit, int page, string query, int? DepartureLocalizationID)
        {

            Paging<Product> products = ProductsPaging(start, limit, "ProductCode", "ASC", query, DepartureLocalizationID);


            return this.Store(products.Data, products.TotalRecords);
        }
        public Paging<Product> ProductsPaging(int start, int limit, string sort, string dir, string filter, int? DepartureLocalizationID)
        {
            List<Product> products = new List<Product>();

            if (DepartureLocalizationID.HasValue && DepartureLocalizationID.Value > 0)
            {

                List<Product> dataTmp = db.Products.ToList();

                //IL faut exclure tous les produits qui sont dans un dossier d'inventaire ayant le statut ouvers ou en cours
                List<Product> lockedProducts = _invDirRepo.LockedProducts(db.Localizations.Find(DepartureLocalizationID.Value));
                dataTmp.RemoveAll(p => lockedProducts.Contains(p));

                foreach (Product pt in dataTmp)
                {
                    products.Add(
                        new Product
                        {
                            ProductID = pt.ProductID,
                            ProductCode = pt.GetProductCode(),
                            ProductLabel = pt.ProductLabel,
                        }
                       );
                }

            }


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

            return new Paging<Product>(rangeProducts, products.Count);

        }

    }
}