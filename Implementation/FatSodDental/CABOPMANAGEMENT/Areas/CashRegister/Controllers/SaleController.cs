using FatSod.DataContext.Initializer;
using FatSod.Security.Abstracts;
using FatSod.Security.Entities;
using FatSod.Supply.Entities;
using CABOPMANAGEMENT.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using SaleE = FatSod.Supply.Entities.Sale;
using FatSod.Supply.Abstracts;
using FatSod.Ressources;
using CABOPMANAGEMENT.Filters;
using FatSod.DataContext.Repositories;

namespace CABOPMANAGEMENT.Areas.CashRegister.Controllers
{
    [Authorize(Order = 1)]
    [TakeBusinessDay(Order = 2)]
    public class SaleController : BaseController
    {
        //Current Controller and current page
        private const string CONTROLLER_NAME = "CashRegister/Sale";
        private const string VIEW_NAME = "Index";
        //person repository
        private ISavingAccount _savingAccountRepository;
        private ISale _saleRepository;
        private IBusinessDay _busDayRepo;
        
        private ILensNumberRangePrice _priceRepository;
        
        private List<BusinessDay> listBDUser;

        private ITillDay _tillDayRepository;
        private LensConstruction lensFrameConstruction = new LensConstruction();
        //Construcitor
        public SaleController(
            ISale saleRepository,
            IBusinessDay busDayRepo,
            ILensNumberRangePrice lnrpRepo,
            ITillDay tillDayRepository,
            ISavingAccount SavingAccountRepo
            )
        {
            this._saleRepository = saleRepository;
            this._busDayRepo = busDayRepo;
            this._savingAccountRepository = SavingAccountRepo;
            this._priceRepository = lnrpRepo;
            this._tillDayRepository = tillDayRepository;
        }
        // GET: Sale/Sale Return all a sales historique add enable to create a new sale
        [OutputCache(Duration = 3600)] 
        public ActionResult Index()
        {
            ViewBag.DisplayForm = 1;
            try
            {
                //we ensure that if this user manage cash register. If he manage it, will verify if till is closed else, we ask he to closed it before login off
                UserTill userTill = (from td in db.UserTills
                                 where td.UserID == SessionGlobalPersonID
                                 select td).SingleOrDefault();
                if (userTill == null || userTill.TillID <= 0)
                {
                    TempData["Message"] = "Access Denied - You can't do sale operation. If you think no, try again or<br/>contact our administrator for this purpose<code/>.";
                    ViewBag.DisplayForm = 0;
                    return this.View();
                }
                List<BusinessDay> UserBusDays = (List<BusinessDay>)Session["UserBusDays"];
                DateTime currentDateOp = UserBusDays.FirstOrDefault().BDDateOperation;
                ViewBag.CurrentBranch = UserBusDays.FirstOrDefault().BranchID;
                ViewBag.BusnessDayDate = UserBusDays.FirstOrDefault().BDDateOperation.ToString("yyyy-MM-dd");// businessDay.BDDateOperation;

                TillDayStatus tState = _tillDayRepository.TillDayStatus(userTill.TillID);
                if (tState == null)
                {
                    TempData["Message"] = "Error - Bad Configuration of Cash Register!!! Please call Your database Administrator";
                    ViewBag.DisplayForm = 0;
                    return this.View();
                }
                if (!tState.IsOpen)
                {
                    TempData["Message"] = "Error - This Cash Register is Still Close!!! Please Open It Before Proceed";
                    ViewBag.DisplayForm = 0;
                    return this.View();
                }

                TillDay currentTillDay = (from t in db.TillDays
                                          where
                                              t.TillID == userTill.TillID && t.TillDayDate == tState.TillDayLastOpenDate.Date && t.IsOpen // t.TillDayDate.Day == currentDateOp.Day && t.TillDayDate.Month == currentDateOp.Month && t.TillDayDate.Year == currentDateOp.Year && t.IsOpen
                                          select t).FirstOrDefault();
                if (currentTillDay == null)
                {
                    TempData["Message"] = "Warnnig - Cash register is closed. You must open it before do any sale<br/>Go at Cash Register module=>Open cash register<code/>";
                    ViewBag.DisplayForm = 0;
                    return this.View();
                }


                ViewBag.CurrentTill = userTill.TillID;

                int deviseID = (Session["DefaultDeviseID"] == null) ? 0 : (int)Session["DefaultDeviseID"];
                if (deviseID <= 0)
                {
                    InjectUserConfigInSession();
                }
                deviseID = (Session["DefaultDeviseID"] == null) ? 0 : (int)Session["DefaultDeviseID"];
                ViewBag.DefaultDeviseID = deviseID;
                ViewBag.DefaultDevise = (deviseID <= 0) ? "" : db.Devises.Find(deviseID).DeviseCode;

                Session["salelines"] = new List<SaleLine>();
            
                Session["isUpdate"] = false;
                Session["MaxValue"] = 500;
                Session["SafetyStock"] = 500;
                return View();
            }

            catch (Exception e)
            {
                TempData["Message"] = "Error " + e.Message;
                ViewBag.DisplayForm = 0;
                return this.View();
            }
        }
        public JsonResult GetTotalPrice (int LensLineQuantity, int FramePrice, int LensPrice, int FrameLineQuantity,
            string LECylinder, string LESphere,string RECylinder,string RESphere)
        {
            List<object> infoList = new List<object>();

            Int32 division, reste;
            int LineUnitPrice = 0;
            double LensQty;

            SpecialLensModel slm = new SpecialLensModel()
            {
                LensLineQuantity = LensLineQuantity,
                LensPrice = LensPrice,
                FrameLineQuantity = FrameLineQuantity,
                LECylinder = LECylinder,
                LESphere = LESphere,
                RECylinder = RECylinder,
                RESphere = RESphere
            };

            if (lensFrameConstruction.IsREValid(slm))
            {
                LensQty = 0d;
                if (lensFrameConstruction.IsLEValid(slm))
                {
                    division = (Math.DivRem((int)slm.LensLineQuantity, 2, out reste));
                    if (reste == 0) //qte pair
                    {
                        LensQty = slm.LensLineQuantity / 2;
                        LensPrice = (int) (slm.LensPrice * LensQty);
                    }
                    else LensPrice =0;
                }
                else
                {
                    LensQty = slm.LensLineQuantity;
                    LensPrice = (int)((slm.LensPrice/2) * LensQty);
                }
                
            }
            //Ajout des infos pour le côté gauche de l'oeil

            if (lensFrameConstruction.IsLEValid(slm))
            {
                LensQty = 0d;
                if (lensFrameConstruction.IsREValid(slm))
                {
                    division = (Math.DivRem((int)slm.LensLineQuantity, 2, out reste));
                    if (reste == 0) //qte pair
                    {
                        LensQty = slm.LensLineQuantity / 2;
                        LensPrice = (int)(slm.LensPrice * LensQty);
                    }
                    else LensPrice =0;
                }
                else
                {
                    LensQty = slm.LensLineQuantity;
                    LensPrice = (int)((slm.LensPrice/2) * LensQty);
                }
            }
            LineUnitPrice = LensPrice + (FramePrice* FrameLineQuantity);
            infoList.Add(new
            {
                LineUnitPrice= LineUnitPrice
            });

            return Json(infoList, JsonRequestBehavior.AllowGet);
        }
        public JsonResult LoadCustomers(string filter)
        {

            List<object> customersList = new List<object>();
            foreach (Customer customer in db.People.OfType<Customer>().Where(c => c.Name.Contains(filter.ToLower())).Take(100).ToArray().OrderBy(c => c.Name))
            {
                string itemLabel = "";

                //si le client est une personne physique, on renvoie son nom et son prénom, si c'est une entreprise, on renvoie son nom
                itemLabel = customer.Name +" "+ customer.Description;

                customersList.Add(new
                {
                    Name = itemLabel,
                    ID = customer.GlobalPersonID
                });
            }

            return Json(customersList, JsonRequestBehavior.AllowGet);
        }
        public JsonResult InitTrnNumber(int CustomerID)
        {
            List<object> _CommandList = new List<object>();

            if (CustomerID > 0)
            {
                
                Customer customer = db.Customers.Find(CustomerID);
               
                _CommandList.Add(new
                {
                    Remarque = customer.AdressPhoneNumber
                });

            }
            return Json(_CommandList, JsonRequestBehavior.AllowGet);
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

        public JsonResult InitDate(int BranchID)
        {
            List<object> _InfoList = new List<object>();
            listBDUser = (List<BusinessDay>)Session["UserBusDays"];
            if (listBDUser == null)
            {
                listBDUser = _busDayRepo.GetOpenedBusinessDay(CurrentUser);
            }
            BusinessDay businessDay = listBDUser.FirstOrDefault(b => b.BranchID == BranchID);
            _InfoList.Add(new
            {
                SaleDate = businessDay.BDDateOperation.ToString("yyyy-MM-dd"),
                MedecinTraitant = "VALDOZ OPTIC",
                LocalizationID= db.Localizations.Where(b=>b.BranchID== BranchID).FirstOrDefault().LocalizationID,
                SalesProductsType=1
            });
            return Json(_InfoList, JsonRequestBehavior.AllowGet);
        }

        /* public ActionResult InitTrnNumber(string BranchID)
         {
             listBDUser = (List<BusinessDay>)Session["UserBusDays"];
             if (listBDUser == null)
             {
                 listBDUser = _busDayRepo.GetOpenedBusinessDay(CurrentUser);
             }
             BusinessDay businessDay = listBDUser.FirstOrDefault(b => b.BranchID == Convert.ToInt32(BranchID));

             //string trnnum = _transactNumbeRepository.displayTransactNumber("SALE", businessDay);
             //this.GetCmp<TextField>("SaleReceiptNumber").Value = trnnum;

             return this.Direct();
         }*/

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

       
        //This method add a saleline in the current sale
        
        public bool DoYes(SpecialLensModel slm,int spray,int boitier)
        {
            bool res = false;
            try
            {
                //double VatRate = CodeValue.Accounting.ParamInitAcct.VATRATE;
                Session["SessionMessage"] = "OK";
                List<SaleLine> salelines = (List<SaleLine>)Session["salelines"];
                List<SaleLine> cols = lensFrameConstruction.Get_COL_From_SLM(slm, new FatSod.DataContext.Concrete.EFDbContext(), spray, boitier);
                foreach (SaleLine saleLine in cols)
                {
                    
                    //Construction du code du produit en fonction de ce qui a été saisie par l'utilisateur
                    saleLine.Product = LensConstruction.GetProductBySaleLine(saleLine, new FatSod.DataContext.Concrete.EFDbContext());
                    if (saleLine.Product == null) 
                    {
                        //ApplyExtraPrices(salelines, reduction, discount, transport, VatRate);
                        Session["salelines"] = salelines;
                        res = true;
                        return res; 
                    }
                    if (!(saleLine.Product is OrderLens))
                    {
                       // if (saleLine.Product.CategoryID==2 )
                       res= this.CheckQty(saleLine.LocalizationID, saleLine.Product.ProductID, saleLine.LineQuantity, spray, boitier);
                       if (!res)
                            return res;
                    }
                    if (saleLine.LineID > 0)
                    {
                        //Ce produit existe deja dans le panier, alors on enleve les deux lignes liées au SpecialOrderLineCode dans la ligne
                        //1-Coe c'est une modification, on enlève l'existant de la ligne en cours de modification; on va l'ajouter dans la suite(Drop and Create)
                        
                        salelines.RemoveAll(col => col.LineID == saleLine.LineID);
                        //2-Si actuellement on a une seule ligne dans la collection, il y a une possibilité qu'on en avait deux et l'autre a été supprimée; il faut donc le supprimer dans le panier
                        if (cols.Count <= 1) salelines.RemoveAll(col => col.SpecialOrderLineCode == saleLine.SpecialOrderLineCode);
                    }

                    if (salelines != null && salelines.Count() > 0)
                    {
                        SaleLine saleLineExist = salelines.FirstOrDefault(s => s.Product.ProductCode == saleLine.Product.ProductCode && s.SpecialOrderLineCode == saleLine.SpecialOrderLineCode && s.EyeSide == saleLine.EyeSide);
                        if (saleLineExist != null)
                        {
                            salelines.Remove(saleLineExist);
                        }

                        int maxLineID = (salelines != null && salelines.Count() > 0) ? salelines.Select(l => l.LineID).Max() : 0;

                        saleLine.LineID = (maxLineID + 1);

                        salelines.Add(saleLine);
                    }
                    else
                    {
                        salelines = new List<SaleLine>();
                        saleLine.LineID = 1;
                        salelines.Add(saleLine);
                    }
                }

                //double VatRate = CodeValue.Accounting.ParamInitAcct.VATRATE;
                //ApplyExtraPrices(salelines, reduction, discount, transport, VatRate);
                Session["salelines"] = salelines;

                res = true;

                return res;
            }
            catch (Exception e)
            {
                res = false;
                Session["SessionMessage"] = "Error " + e.Message;
                return res;
            }
        }
        

       // [DirectMethod]
        //[HttpPost]
        public bool CheckQty(int LocalizationID, int? ProductID, double LineQuantity, int spray, int boitier)
        {
            bool res = false;
            Session["SessionMessage"] = "OK";
            try
            {
                if (spray>0 || boitier>0)
                {
                    this.GetQuantityStock(LocalizationID.ToString(), ProductID.ToString());
                }
                

                double currentQteEnStock = (double)Session["MaxValue"];
                
                //recherche des qtes commandes en attente de validation pr ce produit et cette localization
                double qtyComNonValide = 0d;
              
                bool isStockControl = (bool)Session["isStockControl"];
                
                if (isStockControl)
                {
                    double safetyQty = (double)Session["SafetyStock"];
                    if (currentQteEnStock - (qtyComNonValide +LineQuantity) <= 0) //plus de produit en stock
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
                    Session["SessionMessage"] = statusOperation;
                    
                    return res;
                }

                res = true;
                return res;
            }
            catch (Exception e)
            {
                Session["SessionMessage"] = "Error "+ e.Message;
                return res;
            }
        }
        
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

        
        public JsonResult OnProductSelected(int? Localization, int? CurrentProduct)
        {
            List<object> _InfoList = new List<object>();
            double StockQuantity = 0d;
            double FramePrice = 0d;
            double FrameLineQuantity = 0d;
            

            if ((!Localization.HasValue || Localization.Value <= 0) || (!CurrentProduct.HasValue || CurrentProduct.Value <= 0))
            { return Json(_InfoList, JsonRequestBehavior.AllowGet); }

            Product product = db.Products.Find(CurrentProduct.Value);
            bool productIsLens = product is Lens;

            if (productIsLens)
            {
                StockQuantity = (double)Session["MaxValue"];
                //Récupération du prix du verre à partir de son intervalle de numéro
                LensNumberRangePrice price = _priceRepository.GetPrice(product.ProductID);
                //this.GetCmp<NumberField>("LineUnitPrice").Value = (price != null && price.LensNumberRangePriceID > 0) ? price.Price : 0;
                FramePrice = (price != null && price.LensNumberRangePriceID > 0) ? price.Price : 0;
                FrameLineQuantity = 1;
            }
            else
            {
                var prodLoc = db.ProductLocalizations.Where(pl => pl.ProductID == CurrentProduct.Value && pl.LocalizationID == Localization.Value)
                .Select(p => new
                {
                    ProductLocalizationStockQuantity = p.ProductLocalizationStockQuantity,
                    SellingPrice = p.Product.SellingPrice,
                    ProductLabel = p.Product.ProductLabel,
                    ProductLocalizationSafetyStockQuantity = p.ProductLocalizationSafetyStockQuantity,
                }).SingleOrDefault();

                Session["MaxValue"] = prodLoc.ProductLocalizationStockQuantity;
                Session["CurrentProduct"] = prodLoc.ProductLabel;
                Session["SafetyStock"] = prodLoc.ProductLocalizationSafetyStockQuantity;

                StockQuantity = prodLoc.ProductLocalizationStockQuantity;
                FrameLineQuantity = 1;
                    

                bool isUpdate = (bool)Session["isUpdate"];

                if (!isUpdate)
                {
                    //this.GetCmp<NumberField>("LineUnitPrice").Value = prodLoc.SellingPrice;
                    FramePrice = prodLoc.SellingPrice;
                        
                    Session["isUpdate"] = false;
                }
            }

            _InfoList.Add(new
            {
                StockQuantity = StockQuantity,
                FramePrice = FramePrice,
                FrameLineQuantity= FrameLineQuantity
            });
            return Json(_InfoList, JsonRequestBehavior.AllowGet);
            
        }

        //
        public JsonResult GetAllLensCategories()
        {

            List<object> LensCategorList = new List<object>();

            IRepositorySupply<LensCategory> prod = new RepositorySupply<LensCategory>(db);

            prod.FindAll.ToList().ForEach(productcat =>
            {
                LensCategorList.Add(new
                {
                    CategoryCode = productcat.CategoryCode,
                    CategoryID = productcat.CategoryID
                });
            });

            return Json(LensCategorList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SetSupplyingName(string LensCategoryCode)
        {

            List<object> _InfoList = new List<object>();
            LensCategory cat = (from cate in db.LensCategories
                                where cate.CategoryCode == LensCategoryCode
                                select cate).SingleOrDefault();
            if (cat != null)
            {
                _InfoList.Add(new
                {
                    LensLineQuantity=2,
                    SupplyingName = (cat.SupplyingName != null && cat.SupplyingName.Length > 0) ? cat.SupplyingName : cat.CategoryCode,
                    TypeLens = cat.TypeLens
                });
            }
            return Json(_InfoList, JsonRequestBehavior.AllowGet);
        }
        
        public double GetMaxQtyInStock(int productID, int localizationID)
        {
            double res = 0;

            res = db.ProductLocalizations.SingleOrDefault(pl => pl.ProductID == productID && pl.LocalizationID == localizationID).ProductLocalizationStockQuantity;

            return res;
        }
        //Return salelines of current sale
        //[HttpPost]
        public JsonResult SaleLines()
        {
            List<SaleLine> salelines = (List<SaleLine>)Session["salelines"];
            var model = new
            {
                data = from c in salelines select
                new
                {
                    LineID = c.LineID,
                    LineAmount = c.LineAmount,
                    LineQuantity = c.LineQuantity,
                    ProductLabel = c.ProductLabel,
                    LineUnitPrice = c.LineUnitPrice,
                    SupplyingName=c.SupplyingName
                }
            };
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        //This method confirm a sale
        //[HttpPost]
        public JsonResult AddSale(SpecialLensModel slm, SaleE currentSale, CustomerSlice customerSlice, int? PaymentDelay, string BuyType, string heureVente, int spray = 0, int boitier = 0,int SaleDeliver=0)
        {
            bool status = false;
            string Message = "";
            try
            {
                Session["Receipt_SaleID"] = null;
                Session["Receipt_CustomerID"] = null;
                Session["ReceiveAmoung_Tot"] = null;

                Session["salelines"] = new List<SaleLine>();

                if (BuyType == "" || BuyType==null)
                {
                    Message = "Wrong Payment Mode Select!!! " + Resources.MsgErrChoixPayementMethod;
                    status = false;
                    return new JsonResult { Data = new { status = status, Message = Message } };
                }

                //fabrication des lignes de commande
                status = this.DoYes(slm,spray, boitier);
                if (!status)
                {
                    Message = (string)Session["SessionMessage"];
                    status = false;
                    return new JsonResult { Data = new { status = status, Message = Message } };
                }

                if (SaleDeliver == 0) currentSale.SaleDeliver = false;
                if (SaleDeliver == 1) currentSale.SaleDeliver = true;

                currentSale.CustomerSlice = customerSlice;
                customerSlice.SliceDate = currentSale.SaleDate;
                currentSale.PaymentDelay = (PaymentDelay==null) ? 0 : PaymentDelay.Value;
                currentSale.IsSpecialOrder = false;

                if (currentSale != null && currentSale.CustomerSlice.SliceAmount > 0 && BuyType == CodeValue.Accounting.DefaultCodeAccountingSection.Credit)
                {
                    Message = "Wrong Payment Mode Select!!! " + Resources.MsgWrongChoixPayementMethod;
                    status = false;
                    return new JsonResult { Data = new { status = status, Message = Message } };
                }

                if (currentSale != null && currentSale.CustomerSlice.SliceAmount == 0 && (BuyType == CodeValue.Accounting.DefaultCodeAccountingSection.CODEBANK || BuyType == CodeValue.Accounting.DefaultCodeAccountingSection.CODECAIS))
                {
                    Message = "Wrong Payment Mode Select!!! " + Resources.MsgWrongChoixPayementMethod;
                    status = false;
                    return new JsonResult { Data = new { status = status, Message = Message } };
                }

                currentSale.SaleLines = (List<SaleLine>)Session["salelines"];
            
                //choix de la caisse
                if (BuyType == CodeValue.Accounting.DefaultCodeAccountingSection.CODECAIS)
                {
                    UserTill userTill = db.UserTills.FirstOrDefault(td => td.UserID == SessionGlobalPersonID);
                    currentSale.PaymentMethodID = userTill.TillID;
                }

                //Si l'utilisateur souhaite payer en utilisant son compte d'épargne
                if (BuyType == CodeValue.Supply.DepositReason.SavingAccount)
                {

                    //Si l'aragent versé pour payer les achats est supérieurs au montant restant de l'achat
                    if (customerSlice.SliceAmount > currentSale.TotalPriceTTC)
                    {
                        Message = "More Money Than Expected - Sorry, You have put more Money Than Expected for this sale " ;
                        status = false;
                        return new JsonResult { Data = new { status = status, Message = Message } };
                    }

                    SavingAccount sa = db.SavingAccounts.SingleOrDefault(sa1 => sa1.CustomerID == currentSale.CustomerID.Value);

                    if (sa == null || sa.ID == 0)
                    {
                        Message = "No Saving Account - Sorry, Customer doesn't have a Saving Account. Please contact an administrator ";
                        status = false;
                        return new JsonResult { Data = new { status = status, Message = Message } };
                    }

                    Customer savingCusto = db.Customers.Find(currentSale.CustomerID.Value);

                    double savingAccountBalance = _savingAccountRepository.GetSavingAccountBalance(savingCusto);

                    //ne faites ps d'achat en espèce si : 1 - pas d'argent en caisse; 2- Facture > Montant en caisse
                    if (savingAccountBalance <= 0 || customerSlice.SliceAmount > savingAccountBalance)
                    {
                        Message = "NO Enough Money in Saving Account - Sorry, Customer doesn't have sufficient Money inside his Saving Account. Please contact an administrator";
                        status = false;
                        return new JsonResult { Data = new { status = status, Message = Message } };
                    }
                    currentSale.PaymentMethodID = sa.ID;
                }
                if (currentSale.SaleLines.Count>0)
                {
                    //We will test if sale is in two steps here or not
                    if (currentSale.TotalPriceTTC != currentSale.CustomerSlice.SliceAmount)
                    {
                        currentSale.IsPaid = false;
                    }
                    else
                    {
                        currentSale.IsPaid = true;
                    }
                    if (currentSale != null && (currentSale.PaymentMethodID == null || currentSale.PaymentMethodID <= 0) && (BuyType == CodeValue.Accounting.DefaultCodeAccountingSection.CODEBANK || BuyType == CodeValue.Accounting.DefaultCodeAccountingSection.CODECAIS))
                    {
                        Message = "Wrong Payment Mode Select!!! " + Resources.MsgErrChoixPayementMethod;
                        status = false;
                        return new JsonResult { Data = new { status = status, Message = Message } };
                    }
                    currentSale.IsValidatedSale = true;
                    int SaleID = _saleRepository.SaveChanges(currentSale, heureVente, SessionGlobalPersonID,false,false).SaleID;
                    Session["Receipt_SaleID"] = SaleID;
                    Session["Receipt_CustomerID"] = currentSale.CustomerName;
                    Session["ReceiveAmoung_Tot"] = (currentSale.CustomerSlice != null) ? currentSale.CustomerSlice.SliceAmount : 0;

                    PrintReset(currentSale.BranchID.ToString(),SaleID, customerSlice.SliceAmount);
                }

                status = true;
                Message = Resources.Success + " - " + Resources.SaleNewSale;
            }
            catch (Exception e)
            {
                status = false;
                Message = "Error " + e.Message + " " + e.StackTrace + " " + e.InnerException;
            }
            return new JsonResult { Data = new { status = status, Message = Message } };
        }

        public void PrintReset(string Branch,int SaleID, double SliceAmount)
        {
            Session["salelines"] = new List<SaleLine>();
            Session["CurrentProduct"] = new Product();
            Session["MaxValue"] = 0;
            

            //this.InitTrnNumber(Branch);

            Session["SaleID"] = SaleID;
            Session["SliceAmount"] = SliceAmount;

           
        }

        public JsonResult GetQuantityStock(string Localization, string CurrentProduct)
        {
            double LineQuantity = 0d;
            List<object> _InfoList = new List<object>();
            if ((Localization != null && CurrentProduct != null) && (Localization.Length > 0 && CurrentProduct.Length > 0))
            {
                int idLoc = Convert.ToInt32(Localization);
                int idProd = Convert.ToInt32(CurrentProduct);
                if (idLoc > 0 && idProd > 0)
                {
                    ProductLocalization productInStock = db.ProductLocalizations.FirstOrDefault(pL => pL.LocalizationID == idLoc && pL.ProductID == idProd);
                    Session["CurrentProduct"] = productInStock.ProductLabel;
                    if (productInStock == null || Math.Abs(productInStock.ProductLocalizationStockQuantity) <= 0)
                    {
                        LineQuantity = 0d;
                        Session["MaxValue"] =0d;
                    }
                    else
                    {
                        LineQuantity = productInStock.ProductLocalizationStockQuantity;
                        Session["MaxValue"] = productInStock.ProductLocalizationStockQuantity;
                    }
                }
            }
            _InfoList.Add(new
            {
                LineQuantity = LineQuantity
            });
            return Json(_InfoList, JsonRequestBehavior.AllowGet);

        }


        //chargement des combo box
        public JsonResult populateBuyType()
        {

            List<object> BuyTypeList = new List<object>();
            //cash
            BuyTypeList.Add(new
            {
                ID = CodeValue.Accounting.DefaultCodeAccountingSection.CODECAIS,
                Name = Resources.CASH
            });
            //bank
            BuyTypeList.Add(new
            {
                ID = CodeValue.Accounting.DefaultCodeAccountingSection.CODEBANK,
                Name = Resources.BANK
            });
           
            return Json(BuyTypeList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult PaymentMethods(string BuyTypeCode)
        {
            List<object> model = new List<object>();
            db.PaymentMethods.OfType<Bank>().Where(p => p.Account.CollectifAccount.AccountingSection.AccountingSectionCode == BuyTypeCode).ToList().ForEach(p =>
            {
                model.Add(
                        new
                        {
                            ID = p.ID,
                            Name = p.Name
                        }
                    );
            });
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Return a list of localization of product
        /// </summary>
        /// <param name="BranchID"></param>
        /// <returns></returns>
        public JsonResult GetLocalization(int BranchID = 0)
        {
            List<object> model = new List<object>();
            if (BranchID > 0)
            {
                db.Localizations.Where(pl => pl.BranchID == BranchID).ToList().ForEach(p =>
                    {
                        model.Add(
                            new
                            {
                                LocalizationID = p.LocalizationID,
                                LocalizationCode = "[" + p.LocalizationCode + "] " + p.LocalizationLabel
                            }
                         );
                    }
                 );
            }
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        

        public JsonResult GetAllProducts(int DepartureLocalizationID)
        {

            List<object> _InfoList = new List<object>();

            List<Product> list = ModelProductLocalCat(DepartureLocalizationID);

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

        public List<Product> ModelProductLocalCat(int DepartureLocalizationID)
        {
            List<Product> model = new List<Product>();

            //On a un produit générique
            if (DepartureLocalizationID <= 0 ) //chargement des produits en fct du magasin slt
            {
                return model;
            }
            else //On a un produit de type verre
            {
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

            }

            return model;
        }
        
    }
}