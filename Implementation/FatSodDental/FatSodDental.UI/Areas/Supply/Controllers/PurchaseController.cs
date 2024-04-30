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
    public class PurchaseController : BaseController
    {
        //Current Controller and current page
        private const string CONTROLLER_NAME = "Supply/" + CodeValue.Supply.SupplyMenu.CONTROLLER;
        private const string VIEW_NAME = CodeValue.Supply.SupplyMenu.PATH;
        //person repository
        private IPurchase _purchaseRepository;

        private IPaymentMethod _paymentMethodeRepository;
        private IInventoryDirectory _invDirRepo;
        private ITillDay _tillDayRepository;
        private IBusinessDay _busDayRepo;
        private ITransactNumber _transactNumbeRepository;
        //Construcitor

        private List<BusinessDay> lstBusDay;

        public PurchaseController(
            IPurchase purchaseRepository,
            IPaymentMethod paymentMethodeRepository,
            ITillDay tillDayRepository,
            IBusinessDay busDayRepo,
            ITransactNumber transactNumbeRepository,
            IInventoryDirectory idRepo
            )
        {
            this._paymentMethodeRepository = paymentMethodeRepository;
            this._purchaseRepository = purchaseRepository;
            this._tillDayRepository = tillDayRepository;
            this._busDayRepo = busDayRepo;
            this._transactNumbeRepository = transactNumbeRepository;
            this._invDirRepo = idRepo;
            
        }
        // GET: Sale/Sale Return all a sales historique add enable to create a new sale
        [OutputCache(Duration = 3600)] 
        public ActionResult Purchase()
        {

            //Session["Curent_Page"] = VIEW_NAME;
            //Session["Curent_Controller"] = CONTROLLER_NAME;


            //We test if this user has an autorization to get this menu
            //if (!LoadAction.isAutoriseToGetMenu(SessionProfileID, CodeValue.Supply.SupplyMenu.CODE, db))
            //{
            //    RedirectToAction("NotAuthorized", "User");
            //}
            if (_paymentMethodeRepository.FindAll.FirstOrDefault() == null)
            {
                X.Msg.Alert(
                    "Error in Purchase",
                    "To create a Purchase, you have to create a Payment Method : Cash Register, Bank, etc...; to achieve that, Go to <code>Parameters=>tresaury or contact our administrator <code/><br/>."
                    + "You also need to open a Till; to achieve this, Go to <code>CASH REGISTER =>Open Cash Register or contact our administrator <code/><br/>."
                  ).Show();
                return this.Direct();
            }

            UserTill userTill = db.UserTills.SingleOrDefault(td => td.UserID == SessionGlobalPersonID && td.HasAccess);

            if (userTill == null)
            {
                X.Msg.Alert("Access denied", "You don't have right to do cash opérations in this système. Please contact an administrator").Show();
                return this.Direct();
            }

            lstBusDay = (List<BusinessDay>)Session["UserBusDays"];
            if (lstBusDay == null)
            {
                lstBusDay = _busDayRepo.GetOpenedBusinessDay(CurrentUser);
            };

            BusinessDay tillBD = lstBusDay.FirstOrDefault();

            if(tillBD == null || tillBD.BusinessDayID == 0)
            {
                X.Msg.Alert("Error", "This Cash Register Branch is not yet opened. Open Bussiness Day of " + tillBD.BranchName + " Branch" ).Show();
                return this.Direct();
            }

            TillDay currentTillDay = db.TillDays.SingleOrDefault(t => t.TillID == userTill.TillID && t.IsOpen && t.TillDayDate == tillBD.BDDateOperation);

            if (currentTillDay == null)
            {
                X.Msg.Alert("Error", "Cash register is not yet opened. To do cash operations, You must open a Cash Register").Show();
                return this.Direct();
            }

            //this.GetCmp<Panel>("Body").LoadContent(new ComponentLoader
            //{
            //    Url = Url.Action(VIEW_NAME),
            //    DisableCaching = false,
            //    Mode = LoadMode.Frame
            //});

            List<PurchaseLine> PurchaseLines = new List<PurchaseLine>();
            BusinessDay businessDay = _busDayRepo.GetOpenedBusinessDay(CurrentUser).FirstOrDefault();

            Session["PurchaseLines"] = PurchaseLines;

            //Ext.Net.MVC.PartialViewResult rPVResult = new Ext.Net.MVC.PartialViewResult
            //{
            //    ClearContainer = true,
            //    //RenderMode = RenderMode.AddTo,
            //    ContainerId = "Body",
            //    //WrapByScriptTag = false,
            //    Model = PurchaseModel()
            //};
            //return rPVResult;

            return View(PurchaseModel());
        }

        [HttpPost]
        public ActionResult AddPurchaseLine(PurchaseLine purchaseLine, double reduction = 0, double discount = 0, double transport = 0)
        {
            purchaseLine.Product = db.Products.Find(purchaseLine.ProductID);
            purchaseLine.Localization = db.Localizations.Find(purchaseLine.LocalizationID);

            List<PurchaseLine> PurchaseLines = (List<PurchaseLine>)Session["PurchaseLines"];

            //il s'agit d'une modification alors on fait un drop and create
            if (purchaseLine.TMPID > 0)
            {
                PurchaseLine toRemove = PurchaseLines.SingleOrDefault(pl => pl.TMPID == purchaseLine.TMPID);
                //purchaseLine.TMPID = 0;
                PurchaseLines.Remove(toRemove);
            }

            //alors la variable de session n'était pas vide
            if (PurchaseLines != null && PurchaseLines.Count > 0)
            {
                //c'est un nouvel ajout dans le panier
                if (purchaseLine.TMPID == 0)
                {
                    //existe t-il déjà une ligne de vente ayant le meme produit et le même magasin que celui en création?
                    PurchaseLine existing = PurchaseLines.SingleOrDefault(pl => pl.ProductID == purchaseLine.ProductID && pl.LocalizationID == purchaseLine.LocalizationID);

                    if (existing != null && existing.TMPID > 0)
                    {
                        //la quantité est la somme des deux quantité
                        purchaseLine.LineQuantity += existing.LineQuantity;
                        //le prix c'est le prix de la nouvelle ligne
                        //l'id c'est l'id de la ligne existante
                        purchaseLine.TMPID = existing.TMPID;
                        purchaseLine.LineID = existing.LineID;
                        //on retire l'ancien pour ajouter le nouveau
                        PurchaseLines.Remove(existing);
                    }

                    if (existing == null || existing.TMPID == 0)
                    {
                        purchaseLine.TMPID = PurchaseLines.Select(pl => pl.TMPID).Max() + 1;
                    }
                }
                PurchaseLines.Add(purchaseLine);
            }

            //alors la variable de session était vide
            if (PurchaseLines == null || PurchaseLines.Count == 0)
            {
                //c'est bon pour la création mais pas pour les modifications
                PurchaseLines = new List<PurchaseLine>();
                if (purchaseLine.TMPID == 0)
                {
                    purchaseLine.TMPID = 1;
                }
                PurchaseLines.Add(purchaseLine);
            }

            ApplyExtraPrices(PurchaseLines, reduction, discount, transport);
            
            Session["PurchaseLines"] = PurchaseLines;
            return this.Reset2();
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
                BusinessDay businessDay = lstBusDay.FirstOrDefault(l=>l.BranchID==BranchID.Value);
                string trnnum = _transactNumbeRepository.displayTransactNumber("PURC", businessDay);
                this.GetCmp<TextField>("PurchaseReference").Value = trnnum;
            }
            return this.Direct();
        }
        
        public void ApplyExtraPrices(List<PurchaseLine> PurchaseLines, double reduction, double discount, double transport)
        {

            double valueOperation = (PurchaseLines != null && PurchaseLines.Count > 0) ? PurchaseLines.Select(l => l.LineAmount).Sum() : 0;
            //we add extra price
            double new_HT_price = valueOperation;
            double remise = 0;
            double escompte = 0;
            double NetCom = valueOperation;
            double vatRate = CodeValue.Accounting.ParamInitAcct.VATRATE;

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
        public ActionResult AddPurchase(Purchase purchase)
        {
            List<PurchaseLine> PurchaseLines = (List<PurchaseLine>)Session["PurchaseLines"];
            //choix de la caisse
            if (purchase.PaymentMethod == CodeValue.Accounting.DefaultCodeAccountingSection.CODECAIS)
            {
                UserTill userTill = db.UserTills.SingleOrDefault(td => td.UserID == SessionGlobalPersonID && td.HasAccess);

                double tillBalance = _tillDayRepository.TillStatus(userTill.Till).Ballance;

                //ne faites ps d'achat en espèce si : 1 - pas d'argent en caisse; 2- Facture > Montant en caisse
                if (tillBalance <= 0 || purchase.TotalPriceTTC > tillBalance)
                {

                    X.Msg.Alert("NO Cash Availlable",
                        "Sorry, you can not proceed this cash opérations because you do have sufficient liquidities in your till. Please contact an administrator").Show();
                    return this.Direct();
                }

                purchase.PaymentMethodID = userTill.TillID;
            }
            purchase.PurchaseLines = PurchaseLines;

            if (purchase.PaymentDelay == 0)
            {
                SupplierSlice supplierSlice = new SupplierSlice()
                {
                    DeviseID = purchase.DeviseID,
                    PaymentMethodID = purchase.PaymentMethodID,
                    SliceAmount = purchase.TotalPriceTTC,
                    SliceDate = purchase.PurchaseDate,
                };

                purchase.CurrentSupplierSlice = supplierSlice;
            }

            if (purchase.PurchaseID == 0)
            {
                try
                {
                    _purchaseRepository.CreatePurchase(purchase);
                    return this.Reset();
                }
                catch (Exception e)
                {
                    X.Msg.Alert("Purchase Error Because : Message = ", e.Message + "StackTrace = " + e.StackTrace).Show();
                    return this.Direct();
                }
            }

            return this.Direct();
        }

        [HttpPost]
        public ActionResult UpdatePurchaseLine(int TMPID)
        {
            this.InitializePurchaseLineFields(TMPID);

            return this.Direct();
        }

        [HttpPost]
        public ActionResult RemovePurchaseLine(int TMPID, double reduction = 0, double discount = 0, double transport = 0)
        {
            //lors de la création
            List<PurchaseLine> PurchaseLines = (List<PurchaseLine>)Session["PurchaseLines"];

            if (PurchaseLines != null && PurchaseLines.Count > 0)
            {
                PurchaseLine toRemove = PurchaseLines.SingleOrDefault(pl => pl.TMPID == TMPID);
                PurchaseLines.Remove(toRemove);
                ApplyExtraPrices(PurchaseLines, reduction, discount, transport);
                Session["PurchaseLines"] = PurchaseLines;
            }

            return this.Reset2();
        }

        [HttpPost]
        public ActionResult Reset()
        {
            this.GetCmp<FormPanel>("PurchaseForm").Reset(true);
            this.GetCmp<FormPanel>("FormAddPurchaseLine").Reset(true);
            this.GetCmp<FormPanel>("GeneralForm").Reset(true);

            this.GetCmp<Store>("PurchaseLinesStore").Reload();
            this.GetCmp<Store>("PurchaseListStore").Reload();

            Session["PurchaseLines"] = new List<PurchaseLine>();
            return this.Direct();
        }

        public ActionResult Reset2()
        {
            this.GetCmp<FormPanel>("FormAddPurchaseLine").Reset(true);

            this.GetCmp<Store>("PurchaseLinesStore").Reload();

            List<PurchaseLine> PurchaseLines = (List<PurchaseLine>)Session["PurchaseLines"];

            if (PurchaseLines != null && PurchaseLines.Count > 0)
            {
                this.GetCmp<TextField>("IsCadyEmpty").SetValue(1);
            }
            if (PurchaseLines == null || PurchaseLines.Count == 0)
            {
                this.GetCmp<TextField>("IsCadyEmpty").SetValue(0);
            }
            //ApplyExtraPrices(null, 0, 0, 0);
            return this.Direct();
        }

        public void InitializePurchaseFields(int ID)
        {

            this.GetCmp<FormPanel>("PurchaseForm").Reset(true);
            this.GetCmp<Store>("PurchaseListStore").Reload();

            this.GetCmp<Store>("PurchaseLinesStore").Reload();

            if (ID > 0)
            {
                Purchase purchase = new Purchase();
                purchase = _purchaseRepository.Find(ID);

                this.GetCmp<TextField>("PurchaseID").Value = purchase.PurchaseID;
                if (purchase.SupplierID > 0)
                {
                    this.GetCmp<ComboBox>("SupplierID").SetValue(purchase.SupplierID);
                }
                this.GetCmp<NumberField>("PaymentDelay").Value = purchase.PaymentDelay;
                this.GetCmp<TextField>("PurchaseReference").Value = purchase.PurchaseReference;
                if (purchase.PaymentDelay == 0)
                {
                    this.GetCmp<ComboBox>("PaymentMethod").Disabled = false;
                    this.GetCmp<ComboBox>("PaymentMethodID").Disabled = false;
                    this.GetCmp<ComboBox>("PaymentMethod").SetValueAndFireSelect(_paymentMethodeRepository.GetPaymentMethod(purchase));
                    this.GetCmp<ComboBox>("PaymentMethodID").SetValue(_paymentMethodeRepository.GetPaymentMethodID(purchase.PurchaseID));
                }
                this.GetCmp<ComboBox>("PurchaseBringerID").SetValue(purchase.PurchaseBringerID);
                this.GetCmp<ComboBox>("PurchaseRegisterID").SetValue(purchase.PurchaseRegisterID);
                this.GetCmp<ComboBox>("BranchID").SetValue(purchase.BranchID);
                this.GetCmp<ComboBox>("DeviseID").SetValue(purchase.DeviseID);
            }
        }

        public void InitializePurchaseLineFields(int ID)
        {

            this.GetCmp<FormPanel>("FormAddPurchaseLine").Reset(true);
            this.GetCmp<Store>("PurchaseLinesStore").Reload();

            List<PurchaseLine> PurchaseLines = (List<PurchaseLine>)Session["PurchaseLines"];


            if (ID > 0)
            {
                PurchaseLine purchaseLine = new PurchaseLine();
                purchaseLine = PurchaseLines.SingleOrDefault(pl => pl.TMPID == ID);

                this.GetCmp<TextField>("TMPID").SetValue(purchaseLine.TMPID);
                this.GetCmp<TextField>("LineID").SetValue(purchaseLine.LineID);
                this.GetCmp<ComboBox>("LocalizationID").SetValueAndFireSelect(purchaseLine.LocalizationID);
                this.GetCmp<ComboBox>("ProductID").Value=purchaseLine.ProductID;
                this.GetCmp<NumberField>("LineQuantity").Value = purchaseLine.LineQuantity;
                this.GetCmp<NumberField>("LineUnitPrice").Value = purchaseLine.LineUnitPrice;

                if (purchaseLine.PurchaseID > 0)
                {
                    this.InitializePurchaseFields(purchaseLine.PurchaseID);
                }
            }

            if (PurchaseLines != null && PurchaseLines.Count > 0)
            {
                this.GetCmp<TextField>("IsCadyEmpty").SetValue(1);
            }
            if (PurchaseLines == null || PurchaseLines.Count == 0)
            {
                this.GetCmp<TextField>("IsCadyEmpty").SetValue(0);
            }

        }

        //Return salelines of current sale
        [HttpPost]
        public ActionResult PurchaseLines()
        {
            List<PurchaseLine> purchaseLines = (List<PurchaseLine>)Session["PurchaseLines"];
            List<object> list = new List<object>();

            foreach (PurchaseLine pl in purchaseLines)
            {
                list.Add(
                    new
                    {
                        TMPID = pl.TMPID,
                        ProductLabel = (pl.Product is Lens) ? pl.Product.ProductCode : pl.Product.ProductLabel,
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
        public ActionResult DeletePurchase(int PurchaseID)
        {
            //réduction du stock concernée
            //suppression des lignes d'achat
            //suppression de l'achat
            //_purchaseRepository.CancelPurchase(_purchaseRepository.Find(PurchaseID));
            Session["PurchaseLines"] = new List<PurchaseLine>();
            this.Reset();

            return this.Direct();
        }

        //this method return a different payment method of one type
        public StoreResult PaymentMethods(string PaymentMethod)
        {
            //if (PaymentMethod == CodeValue.Accounting.DefaultCodeAccountingSection.CODECAIS)
            return this.Store(LoadComponent.SpecificBankPaymentMethod(PaymentMethod));
            //if (PaymentMethod == CodeValue.Accounting.DefaultCodeAccountingSection.CODEBANK)
            //    return this.Store(LoadComponent.SpecificBankPaymentMethod(PaymentMethod));
            //if (PaymentMethod == CodeValue.Accounting.DefaultCodeAccountingSection.CODEPOSTCHECK)
            //    return this.Store(LoadComponent.SpecificBankPaymentMethod(PaymentMethod));
            //else
            //    return this.Store(LoadComponent.SpecificBankPaymentMethod(PaymentMethod));
        }

        public ActionResult OpenedBusday()
        {
            List<object> list = new List<object>();
            List<BusinessDay> busDays = _busDayRepo.GetOpenedBusinessDay(CurrentUser);

            foreach (BusinessDay busDay in busDays)
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

        [HttpPost]
        public StoreResult GetAllPurchases()
        {
            return this.Store(PurchaseModel());
        }

        public List<object> PurchaseModel()
        {
            List<Purchase> dataTmp = _purchaseRepository.FindAll.ToList();
            List<object> list = new List<object>();

            foreach (Purchase p in dataTmp)
            {
                list.Add(
                    new
                    {
                        PurchaseID = p.PurchaseID,
                        PurchaseReference = p.PurchaseReference,
                        PurchaseBringerFullName = p.PurchaseBringerFullName,
                        PurchaseDate = p.PurchaseDate,
                        SupplierFullName = p.SupplierFullName,
                        SupplierEmail = p.SupplierEmail,
                        SupplierPhoneNumber = p.SupplierPhoneNumber,
                        PurchaseTotalAmount = db.PurchaseLines.Where(pl => pl.PurchaseID == p.PurchaseID).Select(pl2 => pl2.LineAmount).Sum()

                    }
                   );
            }

            return list;
        }

        public List<ListItem> GetBranches()
        {
            List<BusinessDay> realDataTmp = this._busDayRepo.GetOpenedBusinessDay(CurrentUser);

            List<ListItem> branchList = new List<ListItem>();
            foreach (BusinessDay busDay in realDataTmp)
            {
                branchList.Add(new ListItem(busDay.Branch.BranchName, busDay.BDDateOperation));
            }
            return branchList;
        }

        public ActionResult ChangeBusDay(int? BranchID)
        {
            this.GetCmp<DateField>("PurchaseDate").Reset();
            this.GetCmp<DateField>("PurchaseDeliveryDate").Reset();
            if (BranchID > 0)
            {
                BusinessDay businessDay = _busDayRepo.GetOpenedBusinessDay(db.Branches.Find(BranchID.Value));
                this.GetCmp<DateField>("PurchaseDate").SetValue(businessDay.BDDateOperation);
                this.GetCmp<DateField>("PurchaseDeliveryDate").SetValue(businessDay.BDDateOperation);
            }
            return this.Direct();
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