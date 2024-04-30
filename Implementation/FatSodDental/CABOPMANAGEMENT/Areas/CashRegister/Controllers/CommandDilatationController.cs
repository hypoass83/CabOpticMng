using FatSod.DataContext.Initializer;
using FatSod.Security.Abstracts;
using FatSod.Security.Entities;
using FatSod.Supply.Entities;
using CABOPMANAGEMENT.Controllers;
using CABOPMANAGEMENT.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using FatSod.Supply.Abstracts;
using FatSod.Ressources;
using CABOPMANAGEMENT.Filters;
using FastSod.Utilities.Util;
using FatSod.DataContext.Repositories;

namespace CABOPMANAGEMENT.Areas.CashRegister.Controllers
{
    [Authorize(Order = 1)]
    [TakeBusinessDay(Order = 2)]
    public class CommandDilatationController : BaseController
    {
        //Current Controller and current page
        private const string CONTROLLER_NAME = "CashRegister/CommandDilatation";
        private const string VIEW_NAME = "Index";
        //person repository

        private IAuthoriseSale _authoriseSaleRepository;
        private IBusinessDay _busDayRepo;
        private ITransactNumber _transactNumbeRepository;

        private ILensNumberRangePrice _priceRepository;

        private List<BusinessDay> listBDUser;
        private bool isLens = false;
        private ITillDay _tillDayRepository;

        //Construcitor
        public CommandDilatationController(
            ITillDay tillDayRepository,
            IAuthoriseSale authoriseSaleRepository,
            IBusinessDay busDayRepo,
            ILensNumberRangePrice lnrpRepo,
            ITransactNumber transactNumbeRepository
            )
        {
            this._tillDayRepository = tillDayRepository;
            this._authoriseSaleRepository = authoriseSaleRepository;
            this._busDayRepo = busDayRepo;
            this._transactNumbeRepository = transactNumbeRepository;
            this._priceRepository = lnrpRepo;
        }
        // GET: CashRegister/CommandDilatation
        [OutputCache(Duration = 3600)]
        public ActionResult Index()
        {
            ViewBag.DisplayForm = 1;
            try
            {

                List<BusinessDay> UserBusDays = (List<BusinessDay>)Session["UserBusDays"];
                DateTime currentDateOp = UserBusDays.FirstOrDefault().BDDateOperation;
                ViewBag.CurrentBranch = UserBusDays.FirstOrDefault().BranchID;
                ViewBag.BusnessDayDate = UserBusDays.FirstOrDefault().BDDateOperation.ToString("yyyy-MM-dd");// businessDay.BDDateOperation;


                int deviseID = (Session["DefaultDeviseID"] == null) ? 0 : (int)Session["DefaultDeviseID"];
                if (deviseID <= 0)
                {
                    InjectUserConfigInSession();
                }
                deviseID = (Session["DefaultDeviseID"] == null) ? 0 : (int)Session["DefaultDeviseID"];
                ViewBag.DefaultDeviseID = deviseID;
                ViewBag.DefaultDevise = (deviseID <= 0) ? "" : db.Devises.Find(deviseID).DeviseCode;

                Session["authorisesalelines"] = new List<AuthoriseSaleLine>();

                Session["isUpdate"] = false;
                Session["MaxValue"] = 500d;
                Session["SafetyStock"] = 500d;
                Session["EquipPrice"] = 0d;
                Session["valeur"] = 1d;

                Session["isApplyToCalculate"] = false;
                Session["SessionMessage"] = "";

                Session["LimitAmount"] = 0d;
                Session["CustomerDebt"] = 0d;

                return View();
            }

            catch (Exception e)
            {
                TempData["Message"] = "Error " + e.Message + e.InnerException;
                ViewBag.DisplayForm = 0;
                return this.View(ModelCommand);
            }
        }

        private List<AuthoriseSale> ModelCommand
        {
            get
            {
                List<AuthoriseSale> model = new List<AuthoriseSale>();
                db.AuthoriseSales.Where(c => !c.IsDelivered && c.IsDilatation)
                .ToList().ForEach(c =>
                {
                    double CustomerOrderTotalPrice = Util.ExtraPrices(c.AuthoriseSaleLines.Sum(sl => sl.LineAmount),
                                                                      c.RateReduction, c.RateDiscount, c.Transport, c.VatRate).TotalTTC;
                    model.Add(
                        new AuthoriseSale
                        {
                            AuthoriseSaleID = c.AuthoriseSaleID,
                            SaleDate = c.SaleDate,
                            TotalPriceHT = (int)CustomerOrderTotalPrice,
                            SaleReceiptNumber = c.SaleReceiptNumber,
                            Customer = c.Customer
                        }
                    );
                });
                return model;
            }
        }

        public JsonResult LoadPendingCustomerOder()
        {
            var model = new
            {
                data = from c in ModelCommand
                       select new
                       {
                           AuthoriseSaleID = c.AuthoriseSaleID,
                           SaleDate = c.SaleDate.ToString("yyyy-MM-dd"),
                           CustomerFullName = c.Customer.CustomerFullName,
                           SaleReceiptNumber = c.SaleReceiptNumber,
                           TotalPriceHT = c.TotalPriceHT
                       }
            };
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteCommand(int ID)
        {
            bool status = false;
            string Message = "";
            try
            {
                AuthoriseSale custord = db.AuthoriseSales.Find(ID);

                db.AuthoriseSaleLines.Where(ol => ol.AuthoriseSaleID == ID).ToList().ForEach(ol =>
                {
                    db.AuthoriseSaleLines.Remove(ol);
                    db.SaveChanges();
                });
                db.AuthoriseSales.Remove(custord);
                db.SaveChanges();
                status = true;
                Message = Resources.Success + " - Command has been deleted";


            }
            catch (Exception e)
            {
                Message = "Error " + e.Message;
                status = false;
            }
            return new JsonResult { Data = new { status = status, Message = Message } };
        }
        /*** methodes lors du chargement du formulaire*********************/
        public JsonResult GetOpenedBranches()
        {

            IBusinessDay busDayRepo = new BusinessDayRepository();
            List<object> openedBranchesList = new List<object>();
            List<Branch> openedBranches = busDayRepo.GetOpenedBranches();
            foreach (Branch branch in openedBranches)
            {
                openedBranchesList.Add(new
                {
                    BranchID = branch.BranchID,
                    BranchName = branch.BranchName
                });
            }

            return Json(openedBranchesList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetEquipmentPrice(int? CurrentProduct)
        {
            List<object> _InfoList = new List<object>();
            if ((!CurrentProduct.HasValue || CurrentProduct.Value <= 0))
            {
                return Json(_InfoList, JsonRequestBehavior.AllowGet);
            }
            double priceEquip = 0d;
            double valeur = 1d;

            Product product = db.Products.Find(CurrentProduct.Value);
            bool productIsLens = product is Lens;
            if (!productIsLens)
            {

                Session["isApplyToCalculate"] = true;
                priceEquip = product.SellingPrice;
                valeur = 1d;

                Session["valeur"] = valeur;
            }

            _InfoList.Add(new
            {
                LineUnitPrice = priceEquip

            });
            return Json(_InfoList, JsonRequestBehavior.AllowGet);
        }
        


        //this method alert user when the quantity in localization is unavailable
        public bool AlertMsgSock(double QuantityValue, double safetyQty)
        {
            bool res = true;
            double maxQuantity = (double)Session["MaxValue"];
            string productLabel = (string)Session["CurrentProduct"];
            Session["SessionMessage"] = "";
            if (safetyQty > QuantityValue)
            {
                Session["SessionMessage"] = "Attention - En vendant cette quantité le seuil de sécurité sera atteind. Veuillez, faire un réapprovisionnement le plutôt possible";
                res = true;
            }
            if (maxQuantity < QuantityValue)
            {
                Session["SessionMessage"] = "Erreur - " + productLabel + " : " + Resources.EnoughQuantityStock + " " + maxQuantity;
                res = false;
            }
            return res;
        }
        public bool AlertMsgNotSold(double ProductAmount)
        {
            bool res = false;
            double LimitAmount = (double)Session["LimitAmount"];
            double CustomerDebt = (double)Session["CustomerDebt"];
            Session["SessionMessage"] = "OK";
            if (LimitAmount > 0)
            {
                if (CustomerDebt >= 0) res = true;
                else if ((CustomerDebt * -1 + ProductAmount) >= LimitAmount)
                {
                    Session["SessionMessage"] = "Sorry - " + Resources.MsgLimitAmount;
                    res = false;
                }
                else res = true;
            }
            else res = true;
            return res;
        }

        //[HttpPost]
        public JsonResult AddAuthoriseSaleLine(AuthoriseSaleLine authoriseSaleLine, int? OeilDroiteGauche, string reduction, string discount, double transport = 0)
        {
            bool status = false;
            string Message = "";
            try
            {
                bool res = false;
                if ((OeilDroiteGauche == null) || !(authoriseSaleLine.Product is Lens)) authoriseSaleLine.OeilDroiteGauche = EyeSide.N;
                if (OeilDroiteGauche == 0) authoriseSaleLine.OeilDroiteGauche = EyeSide.OD;
                if (OeilDroiteGauche == 1) authoriseSaleLine.OeilDroiteGauche = EyeSide.OG;
                if (OeilDroiteGauche == 2) authoriseSaleLine.OeilDroiteGauche = EyeSide.ODG;
                double lineQty = (double)Session["valeur"];

                Product p = db.Products.Find(authoriseSaleLine.ProductID);


                authoriseSaleLine.LineUnitPrice = (p is Lens) ? (authoriseSaleLine.LineUnitPrice / 2) : authoriseSaleLine.LineUnitPrice;

                double currentQteEnStock = (double)Session["MaxValue"];
                List<AuthoriseSaleLine> authoriseSaleLines = (List<AuthoriseSaleLine>)Session["authorisesalelines"];
                double ValueOperation = 0d;
                if (authoriseSaleLines.Count == 0) ValueOperation = authoriseSaleLine.LineAmount;
                else ValueOperation = authoriseSaleLines.Select(sl => sl.LineAmount).Sum() + authoriseSaleLine.LineAmount;
                ExtraPrice extra = Util.ExtraPrices(ValueOperation, LoadComponent.ConvertToDouble(reduction), LoadComponent.ConvertToDouble(discount), transport, CodeValue.Accounting.ParamInitAcct.VATRATE);
                bool valideOp = AlertMsgNotSold(extra.TotalTTC);
                if (valideOp)
                {
                    //recherche des qtes commandes en attente de validation pr ce produit et cette localization
                    double qtyComNonValide = 0;

                    var lscustOrder = db.AuthoriseSales.Join(db.AuthoriseSaleLines, co => co.AuthoriseSaleID, col => col.AuthoriseSaleID,
                    (co, col) => new { co, col })
                    .Where(lsp => !lsp.co.IsDelivered && lsp.col.ProductID == authoriseSaleLine.ProductID && lsp.col.LocalizationID == authoriseSaleLine.LocalizationID && !lsp.col.isPost)
                    .Select(s => new
                    {
                        LineQuantity = s.col.LineQuantity,
                    })
                    .ToList();

                    lscustOrder.ForEach(lc =>
                    {
                        qtyComNonValide = qtyComNonValide + lc.LineQuantity;
                    });

                    List<AuthoriseSaleLine> qteSaleline = authoriseSaleLines.Where(c => c.ProductID == authoriseSaleLine.ProductID && c.LocalizationID == authoriseSaleLine.LocalizationID).ToList();
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

                    bool isStockControl = (p.Category.CategoryCode.ToUpper() == "DILATATION") ? false : (bool)Session["isStockControl"];
                    if (isStockControl)
                    {
                        double safetyQty = (double)Session["SafetyStock"];
                        if (currentQteEnStock - (qtyComNonValide + (authoriseSaleLine.LineQuantity * lineQty)) <= 0) //plus de produit en stock
                        {
                            res = this.AlertMsgSock(qtyComNonValide + (authoriseSaleLine.LineQuantity * lineQty), safetyQty);
                            if (!res)
                            {
                                status = false;
                                Message = (string)Session["SessionMessage"];
                                return new JsonResult { Data = new { status = status, Message = Message } };
                            }
                        }
                        if (currentQteEnStock - (qtyComNonValide + (authoriseSaleLine.LineQuantity * lineQty)) <= safetyQty) //stock de securite atteint
                        {
                            res = this.AlertMsgSock(qtyComNonValide + (authoriseSaleLine.LineQuantity * lineQty), safetyQty);
                            if (!res)
                            {
                                status = false;
                                Message = (string)Session["SessionMessage"];
                                return new JsonResult { Data = new { status = status, Message = Message } };
                            }
                        }
                    }

                    if ((authoriseSaleLine.LineQuantity * lineQty) <= 0)
                    {
                        statusOperation = Resources.cmdMontantObligatoire;
                        status = false;
                        Message = statusOperation;
                        return new JsonResult { Data = new { status = status, Message = Message } };
                    }
                    else
                    {
                        status = this.DoYes(authoriseSaleLine, LoadComponent.ConvertToDouble(reduction), LoadComponent.ConvertToDouble(discount), transport);
                        Message = (string)Session["SessionMessage"];
                    }
                }
            }
            catch (Exception e)
            {
                status = false;
                Message = "Error " + e.Message + " " + e.StackTrace + " " + e.InnerException;
            }
            return new JsonResult { Data = new { status = status, Message = Message } };
        }
        //This method add a saleline in the current sale

        public bool DoYes(AuthoriseSaleLine authoriseSaleLine, double reduction = 0, double discount = 0, double transport = 0)
        {
            bool res = false;
            try
            {
                double lineQty = (double)Session["valeur"];
                Session["SessionMessage"] = "OK";

                List<AuthoriseSaleLine> authoriseSales = (List<AuthoriseSaleLine>)Session["authorisesalelines"];
                if (authoriseSaleLine.LineID > 0)
                {
                    //this product already exist in session
                    authoriseSales.Remove(authoriseSales.FirstOrDefault(cl => cl.LineID == authoriseSaleLine.LineID));
                }
                if (authoriseSales != null && authoriseSales.Count() > 0)
                {
                    AuthoriseSaleLine saleLineExist = authoriseSales.FirstOrDefault(s => s.ProductID == authoriseSaleLine.ProductID && s.OeilDroiteGauche == authoriseSaleLine.OeilDroiteGauche);
                    if (saleLineExist != null)
                    {
                        authoriseSales.Remove(saleLineExist);
                    }
                    AuthoriseSaleLine newArticle = new AuthoriseSaleLine()
                    {
                        LineID = authoriseSales.LastOrDefault() != null ? 1 + authoriseSales.LastOrDefault().LineID : 1,
                        LineUnitPrice = authoriseSaleLine.LineUnitPrice,
                        LineQuantity = authoriseSaleLine.LineQuantity,
                        ProductID = authoriseSaleLine.ProductID,
                        Product = db.Products.Find(authoriseSaleLine.ProductID),
                        LocalizationID = authoriseSaleLine.LocalizationID,
                        Localization = db.Localizations.Find(authoriseSaleLine.LocalizationID),
                        OeilDroiteGauche = authoriseSaleLine.OeilDroiteGauche
                    };
                    authoriseSales.Add(newArticle);

                }
                else
                {
                    authoriseSales = new List<AuthoriseSaleLine>();
                    AuthoriseSaleLine newArticle = new AuthoriseSaleLine()
                    {
                        LineID = 1,
                        LineUnitPrice = authoriseSaleLine.LineUnitPrice,
                        LineQuantity = authoriseSaleLine.LineQuantity,
                        ProductID = authoriseSaleLine.ProductID,
                        Product = db.Products.Find(authoriseSaleLine.ProductID),
                        LocalizationID = authoriseSaleLine.LocalizationID,
                        Localization = db.Localizations.Find(authoriseSaleLine.LocalizationID),
                        OeilDroiteGauche = authoriseSaleLine.OeilDroiteGauche
                    };
                    authoriseSales.Add(newArticle);
                }

                Session["authorisesalelines"] = authoriseSales;

                this.refreshCmdLine();

                res = true;
            }
            catch (Exception e)
            {
                res = false;
                Session["SessionMessage"] = "Error " + e.Message;

            }
            return res;
        }



        public void refreshCmdLine()
        {
            Session["SessionMessage"] = "OK";
            Session["EquipPrice"] = 0d;
            Session["valeur"] = 1d;

        }
        //

        public JsonResult ApplyExtraPrices(double reduction, double discount, double transport)
        {
            List<object> _CommandList = new List<object>();
            List<AuthoriseSaleLine> SaleLines = (List<AuthoriseSaleLine>)Session["authorisesalelines"];

            double valueOperation = SaleLines.Select(l => l.LineAmount).Sum();
            //we add extra price
            double new_HT_price = valueOperation;
            double SliceAmount = 0d;
            double RemaingAmount = 0d;

            double NetCom = valueOperation;
            double vatRate = CodeValue.Accounting.ParamInitAcct.VATRATE;
            ExtraPrice extra = Util.ExtraPrices(valueOperation, reduction, discount, transport, vatRate);

            SliceAmount = extra.TotalTTC;
            RemaingAmount = 0d;

            _CommandList.Add(new
            {
                InitialHT = valueOperation,
                DiscountAmount = extra.DiscountAmount,
                NetCom = extra.NetCom,
                ReductionAmount = extra.ReductionAmount,
                TotalPriceHT = extra.NetFinan,
                TVAAmount = extra.TVAAmount,
                TotalPriceTTC = extra.TotalTTC,
                InitialTTC = extra.TotalTTC,
                SliceAmount = SliceAmount,
                RemaingAmount = RemaingAmount,
            });

            return Json(_CommandList, JsonRequestBehavior.AllowGet);
        }


        //This method remove saleline
        //[HttpPost]
        public JsonResult RemoveSaleLine(int ID)
        {
            List<AuthoriseSaleLine> authoriseSaleLines = (List<AuthoriseSaleLine>)Session["authorisesalelines"];
            authoriseSaleLines.Remove(authoriseSaleLines.FirstOrDefault(s => s.LineID == ID));

            var model = new
            {
                data = from c in authoriseSaleLines
                       select new
                       {
                           LineID = c.LineID,
                           LineAmount = c.LineAmount,
                           LineQuantity = c.LineQuantity,
                           ProductLabel = c.ProductLabel,
                           LineUnitPrice = c.LineUnitPrice
                       }
            };
            return Json(model, JsonRequestBehavior.AllowGet);
        }


        private void SimpleReset()
        {
            List<AuthoriseSaleLine> authoriseSaleLines = new List<AuthoriseSaleLine>();
            Session["authorisesalelines"] = authoriseSaleLines;
        }


        public JsonResult UpdateLine(int ID)
        {


            List<object> _CommandList = new List<object>();

            Session["isUpdate"] = true;
            Session["LensNumberFullCode"] = "";

            List<AuthoriseSaleLine> salelines = (List<AuthoriseSaleLine>)Session["authorisesalelines"];
            Line lineToUpdate = salelines.FirstOrDefault(l => l.LineID == ID);
            int ProductID = 0;

            if (lineToUpdate.Product is Lens)
            {
                Lens number = (from l in db.Lenses
                               where l.ProductID == lineToUpdate.ProductID
                               select l).FirstOrDefault();
                Session["LensNumberFullCode"] = number.LensNumberFullCode;
                ProductID = number.LensNumberID;
            }

            double qtystock = GetMaxQtyInStock(lineToUpdate.ProductID, lineToUpdate.LocalizationID);
            Session["MaxValue"] = qtystock;
            Session["CurrentProduct"] = lineToUpdate.ProductLabel;

            _CommandList.Add(new
            {
                LineUnitPrice = lineToUpdate.LineUnitPrice,
                LineQuantity = lineToUpdate.LineQuantity,
                StockQuantity = qtystock,
                Product = lineToUpdate.ProductID,
                ProductNumberID = (lineToUpdate.Product is Lens) ? ProductID : lineToUpdate.ProductID,
                ProductCategoryID = lineToUpdate.Product.CategoryID,
                Localization = lineToUpdate.LocalizationID,
                LineID = lineToUpdate.LineID,
                LensCategory = (lineToUpdate.Product is Lens) ? 0 : 1,
                ProductCode = lineToUpdate.Product.ProductCode,
                ProductNumber = Session["LensNumberFullCode"]

            });

            return Json(_CommandList, JsonRequestBehavior.AllowGet);

        }
        public double GetMaxQtyInStock(int productID, int localizationID)
        {
            double res = 0;

            res = db.ProductLocalizations.SingleOrDefault(pl => pl.ProductID == productID && pl.LocalizationID == localizationID).ProductLocalizationStockQuantity;

            return res;
        }
        //Return salelines of current sale

        public JsonResult SaleLines()
        {
            List<AuthoriseSaleLine> salelines = (List<AuthoriseSaleLine>)Session["authorisesalelines"];
            var model = new
            {
                data = from c in salelines
                       select new
                       {
                           LineID = c.LineID,
                           LineAmount = c.LineAmount,
                           LineQuantity = c.LineQuantity,
                           ProductLabel = c.ProductLabel,
                           LineUnitPrice = c.LineUnitPrice
                       }
            };

            return Json(model, JsonRequestBehavior.AllowGet);
        }
        //This method confirm a sale
        //[HttpPost]
        public JsonResult AddAuthoriseSale(AuthoriseSale currentSale, string heureVente, string RateReduction, double TVA = 0)
        {
            bool status = false;
            string Message = "";
            try
            {
                Session["Receipt_SaleID"] = null;
                Session["Receipt_CustomerID"] = null;
                Session["ReceiveAmoung_Tot"] = null;


                currentSale.RateReduction = LoadComponent.ConvertToDouble(RateReduction);
                currentSale.VatRate = CodeValue.Accounting.ParamInitAcct.VATRATE;

                currentSale.IsDilatation = true;


                currentSale.AuthoriseSaleLines = (List<AuthoriseSaleLine>)Session["authorisesalelines"];


                if (currentSale.AuthoriseSaleLines.Count > 0)
                {

                    int SaleID = _authoriseSaleRepository.SaveChanges(currentSale, heureVente, SessionGlobalPersonID,true).AuthoriseSaleID;
                    PrintReset(SaleID, TVA, currentSale.CustomerName);
                }

                status = true;
                Message = Resources.Success + " - " + Resources.NEWCOMMAND;
            }
            catch (Exception e)
            {
                status = false;
                Message = "Error " + e.Message + " " + e.StackTrace + " " + e.InnerException;
            }
            return new JsonResult { Data = new { status = status, Message = Message } };
        }

        public void PrintReset(int SaleID, double TVA, string CustomerID)
        {
            Session["authorisesalelines"] = new List<AuthoriseSaleLine>();

            Session["CurrentProduct"] = new Product();
            Session["MaxValue"] = 0;

            Session["SaleID"] = SaleID;

            Session["Receipt_SaleID"] = SaleID;
            Session["Receipt_CustomerID"] = CustomerID;


        }

        public JsonResult InitDate(int? BranchID)
        {
            List<object> _InfoList = new List<object>();
            if (BranchID != null && BranchID.HasValue && BranchID.Value > 0)
            {
                listBDUser = (List<BusinessDay>)Session["UserBusDays"];
                if (listBDUser == null)
                {
                    listBDUser = _busDayRepo.GetOpenedBusinessDay(CurrentUser);
                }
                DateTime businessD = listBDUser.FirstOrDefault(b => b.BranchID == BranchID).BDDateOperation;
                _InfoList.Add(new
                {
                    SaleDate = businessD.ToString("yyyy-MM-dd")
                });
            }

            return Json(_InfoList, JsonRequestBehavior.AllowGet);
        }

        //*****************---------------------------************************************//
        public JsonResult LoadCustomers(string filter)
        {

            List<object> customersList = new List<object>();

            var list = (from l1 in db.Customers
                        join lc in db.Consultations on l1.GlobalPersonID equals lc.CustomerID
                        join lp in db.ConsultDilPrescs on lc.ConsultationID equals lp.ConsultationID
                        join ld in db.ConsultDilatations on lp.ConsultDilPrescID equals ld.ConsultDilPrescID
                        where !l1.IsBillCustomer && (ld.CodeDilation==null || ld.CodeDilation=="" || !ld.isAuthoriseSale) && l1.Name.Contains(filter.ToLower())
                        select new
                        {
                            GlobalPersonID = l1.GlobalPersonID,
                            Name = l1.Name,
                            Description = l1.Description,
                            ConsultDilPrescID = lp.ConsultDilPrescID
                        }).ToList();

            foreach (var customer in list)
            {
                string itemLabel = "";

                //si le client est une personne physique, on renvoie son nom et son prénom, si c'est une entreprise, on renvoie son nom
                itemLabel = customer.Name +
                    (" " + customer.Description);

                customersList.Add(new
                {
                    Name = itemLabel,
                    ID = customer.GlobalPersonID,
                    ConsultDilPrescID=customer.ConsultDilPrescID
                });
            }

            return Json(customersList, JsonRequestBehavior.AllowGet);
        }

        ////////////////////
        public JsonResult GetAllStockedLocations()
        {
            List<object> localizationsList = new List<object>();
            foreach (Localization Loc in db.Localizations.ToList())
            {
                localizationsList.Add(new
                {
                    LocalizationCode = Loc.LocalizationCode,
                    LocalizationID = Loc.LocalizationID
                });
            }

            return Json(localizationsList, JsonRequestBehavior.AllowGet);

        }
        //


        //
        public JsonResult GetProductCategory()
        {
            List<object> categoryList = new List<object>();
            List<Category> categories = LoadComponent.GetAllDILATIONCategories();
            foreach (Category cat in categories)
            {
                categoryList.Add(new
                {
                    CategoryID = cat.CategoryID,
                    CategoryCode = cat.CategoryLabel
                });
            }

            return Json(categoryList, JsonRequestBehavior.AllowGet);
        }
        //
        public JsonResult DisableNumero(int ProductCategoryID)
        {
            List<object> _InfoList = new List<object>();
            Category catprod = db.Categories.Find(ProductCategoryID);
            _InfoList.Add(new
            {
                LensCategory = (catprod is LensCategory) ? 0 : 1
            });


            return Json(_InfoList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAllProducts(int DepartureLocalizationID, int? ProductCategoryID, int? ProductNumberID)
        {

            List<object> _InfoList = new List<object>();

            List<Product> list = ModelProductLocalCat(DepartureLocalizationID, ProductCategoryID, ProductNumberID);

            foreach (Product s in list.OrderBy(c => c.ProductCode))
            {

                _InfoList.Add(new
                {
                    ProductID = s.ProductID,
                    ProductCode = s.ProductCode,

                });
            }
            return Json(_InfoList, JsonRequestBehavior.AllowGet);
        }

        //recup des produits

        public List<Product> ModelProductLocalCat(int DepartureLocalizationID, int? ProductCategoryID, int? ProductNumberID)
        {
            List<Product> model = new List<Product>();


            //On a un produit générique
            if (DepartureLocalizationID > 0 && (ProductCategoryID == 0 || ProductCategoryID == null) && (ProductNumberID == 0 || ProductNumberID == null)) //chargement des produits en fct du magasin slt
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

                        foreach (var pt in lstLensProduct.OrderBy(c => c.ProductCode))
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

                    foreach (var pt in lstLensProduct.OrderBy(c => c.ProductCode))
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

        public ActionResult OnProductSelected(int? Localization, int? CurrentProduct)
        {
            List<object> _InfoList = new List<object>();
            double StockQuantity = 0d;
            double LineUnitPrice = 0d;
            Session["EquipPrice"] = 0d;

            if ((!Localization.HasValue || Localization.Value <= 0) || (!CurrentProduct.HasValue || CurrentProduct.Value <= 0))
            {
                return Json(_InfoList, JsonRequestBehavior.AllowGet);
            }

            Product product = db.Products.Find(CurrentProduct.Value);
            bool productIsLens = product is Lens;

            if (productIsLens)
            {
                StockQuantity = (double)Session["MaxValue"];
                //Récupération du prix du verre à partir de son intervalle de numéro
                LensNumberRangePrice price = _priceRepository.GetPrice(product.ProductID);
                LineUnitPrice = (price != null && price.LensNumberRangePriceID > 0) ? price.Price : 0;
            }
            else
            {
                var prodLoc = db.ProductLocalizations.Where(pl => pl.ProductID == CurrentProduct.Value && pl.LocalizationID == Localization.Value)
                .Select(p => new
                {
                    ProductID = p.ProductID,
                    ProductLocalizationStockQuantity = p.ProductLocalizationStockQuantity,
                    SellingPrice = p.Product.SellingPrice,
                    ProductLabel = p.Product.ProductLabel,
                    ProductLocalizationSafetyStockQuantity = p.ProductLocalizationSafetyStockQuantity
                }).SingleOrDefault();

                Session["MaxValue"] = prodLoc.ProductLocalizationStockQuantity;
                Session["CurrentProduct"] = prodLoc.ProductLabel;
                Session["CurrentProductID"] = prodLoc.ProductID;
                Session["SafetyStock"] = prodLoc.ProductLocalizationSafetyStockQuantity;

                StockQuantity = prodLoc.ProductLocalizationStockQuantity;

                bool isUpdate = (bool)Session["isUpdate"];

                if (!isUpdate)
                {
                    Session["EquipPrice"] = prodLoc.SellingPrice;
                    LineUnitPrice = prodLoc.SellingPrice;
                    Session["isUpdate"] = false;
                }
            }

            _InfoList.Add(new
            {
                StockQuantity = StockQuantity,
                LineUnitPrice = LineUnitPrice

            });
            return Json(_InfoList, JsonRequestBehavior.AllowGet);
        }
    }
}