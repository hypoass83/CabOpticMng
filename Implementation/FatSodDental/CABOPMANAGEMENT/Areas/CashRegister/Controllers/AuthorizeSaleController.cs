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
using FastSod.Utilities.Util;
using CABOPMANAGEMENT.Tools;

namespace CABOPMANAGEMENT.Areas.CashRegister.Controllers
{
    [Authorize(Order = 1)]
    [TakeBusinessDay(Order = 2)]
    public class AuthorizeSaleController : BaseController
    {
        //Current Controller and current page
        private const string CONTROLLER_NAME = "CashRegister/AuthorizeSale";
        private const string VIEW_NAME = "Index";
        //person repository
        private ISavingAccount _savingAccountRepository;
        private IAuthoriseSale _authoriseSaleRepository;
        private IBusinessDay _busDayRepo;
        private IProductLocalization _ProductLocalization;

        private ILensNumberRangePrice _priceRepository;

        private List<BusinessDay> listBDUser;


        private LensConstruction lensFrameConstruction = new LensConstruction();
        //Construcitor
        public AuthorizeSaleController(
            IAuthoriseSale authoriseSaleRepository,
            IBusinessDay busDayRepo,
            ILensNumberRangePrice lnrpRepo,
            ISavingAccount SavingAccountRepo,
            IProductLocalization ProductLocalization
            )
        {
            this._authoriseSaleRepository = authoriseSaleRepository;
            this._busDayRepo = busDayRepo;
            this._savingAccountRepository = SavingAccountRepo;
            this._priceRepository = lnrpRepo;
            this._ProductLocalization = ProductLocalization;
        }
        // GET: Sale/Sale Return all a sales historique add enable to create a new sale
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
                Session["MaxValue"] = 500;
                Session["SafetyStock"] = 500;
                return View(ModelCommand);
            }

            catch (Exception e)
            {
                TempData["Message"] = "Error " + e.Message;
                ViewBag.DisplayForm = 0;
                return this.View();
            }
        }

        public JsonResult populateUsers()
        {
            /*
            List<object> userList = new List<object>();
            foreach (User user in db.People.OfType<User>().Where(u => u.IsSeller).ToArray().OrderBy(c => c.Name))
            {
                userList.Add(new { Name = user.UserFullName, ID = user.GlobalPersonID });
            }
            */
            return Json(LoadComponent.GetSellers(CurrentBranch.BranchID), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetTotalPrice(int LensLineQuantity, int FramePrice, int LensPrice, int FrameLineQuantity,
            string LECylinder, string LESphere, string RECylinder, string RESphere)
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
                        LensPrice = (int)(slm.LensPrice * LensQty);
                    }
                    else LensPrice = 0;
                }
                else
                {
                    LensQty = slm.LensLineQuantity;
                    LensPrice = (int)((slm.LensPrice / 2) * LensQty);
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
                    else LensPrice = 0;
                }
                else
                {
                    LensQty = slm.LensLineQuantity;
                    LensPrice = (int)((slm.LensPrice / 2) * LensQty);
                }
            }
            LineUnitPrice = LensPrice + (FramePrice * FrameLineQuantity);
            infoList.Add(new
            {
                LineUnitPrice = LineUnitPrice
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
                itemLabel = customer.Name + " " + customer.Description;

                customersList.Add(new
                {
                    Name = itemLabel,
                    ID = customer.GlobalPersonID,
                    DateOfBirth=(customer.DateOfBirth.HasValue) ? customer.DateOfBirth.Value.ToString("yyyy-MM-dd") : "" // new DateTime(1900,1,1).ToString("yyyy-MM-dd")
                });
            }

            return Json(customersList, JsonRequestBehavior.AllowGet);
        }

       
        public JsonResult GetAllSerialNumber(string filter, int DepartureLocalizationID=0)
        {

            List<object> customersList = new List<object>();
            List<ProductLocalization> res = new List<ProductLocalization>();
            //IEqualityComparer<ProductLocalization> locationComparer = new GenericComparer<ProductLocalization>("NumeroSerie");

            //res = _ProductLocalization.FindAll.Where(c => c.LocalizationID == DepartureLocalizationID && c.NumeroSerie!=null && c.NumeroSerie.Contains(filter.ToLower()))
            //.Distinct(locationComparer)
            //                                     .ToList();

            res = db.ProductLocalizations.Where(c => c.LocalizationID == DepartureLocalizationID && c.NumeroSerie.Contains(filter.ToLower())).Take(200).ToList();

            //var res = (from pl in db.ProductLocalizations where 
            //           (pl.LocalizationID==DepartureLocalizationID && pl.NumeroSerie.Contains(filter.ToLower()))
            //           orderby pl.NumeroSerie
            //           select new
            //           {
            //               LocalizationID =pl.LocalizationID,
            //               ProductID=pl.ProductID,
            //               NumeroSerie=pl.NumeroSerie
            //           }).Distinct().ToList();

            foreach (var prodLocalization in res)
            {
                
                customersList.Add(new
                {
                    NumeroSerie = prodLocalization.NumeroSerie,
                    ProductLocalizationID = prodLocalization.ProductLocalizationID
                });
            }

            return Json(customersList, JsonRequestBehavior.AllowGet);
        }




        public JsonResult InitProductDetail(int ProductLocalizationID)
        {
            List<object> _CommandList = new List<object>();
            double StockQuantity = 0d;
            double FramePrice = 0d;
            double FrameLineQuantity = 0d;

            if (ProductLocalizationID > 0)
            {

                ProductLocalization prodLocalization = db.ProductLocalizations.Find(ProductLocalizationID);

                Session["MaxValue"] = prodLocalization.ProductLocalizationStockQuantity;
                Session["CurrentProduct"] = prodLocalization.ProductCode;
                Session["SafetyStock"] = prodLocalization.ProductLocalizationSafetyStockQuantity;

                StockQuantity = prodLocalization.ProductLocalizationStockQuantity;
                FrameLineQuantity = 1;


                bool isUpdate = (bool)Session["isUpdate"];

                if (!isUpdate)
                {
                    FramePrice = prodLocalization.Product.SellingPrice;

                    Session["isUpdate"] = false;
                }


                _CommandList.Add(new
                {
                    ProductID = prodLocalization.ProductID,
                    ProductCode = prodLocalization.Product.ProductCode,
                    marque = prodLocalization.Marque,
                    StockQuantity = StockQuantity,
                    FramePrice = FramePrice,
                    FrameLineQuantity = FrameLineQuantity,
                    reference = prodLocalization.NumeroSerie,
                    NumeroSerie = prodLocalization.NumeroSerie
                });

            }
            return Json(_CommandList, JsonRequestBehavior.AllowGet);
        }


        public JsonResult InitTrnNumber(int CustomerID)
        {
            List<object> _CommandList = new List<object>();

            if (CustomerID > 0)
            {

                Customer customer = db.Customers.Find(CustomerID);

                _CommandList.Add(new
                {
                    Remarque = customer.AdressPhoneNumber,
                    CNI=customer.CNI
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
                LocalizationID = db.Localizations.Where(b => b.BranchID == BranchID).FirstOrDefault().LocalizationID,
                SalesProductsType = 1
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


        //This method add a saleline in the current sale

        public bool DoYes(SpecialLensModel slm, int spray, int boitier)
        {
            bool res = false;
            try
            {
                
                Session["SessionMessage"] = "OK";
                List<AuthoriseSaleLine> authosalelines = (List<AuthoriseSaleLine>)Session["authorisesalelines"];
                List<AuthoriseSaleLine> cols = lensFrameConstruction.Get_AUTHCOL_From_SLM(slm, new FatSod.DataContext.Concrete.EFDbContext(), spray, boitier);
                foreach (AuthoriseSaleLine authosaleLine in cols)
                {

                    //Construction du code du produit en fonction de ce qui a été saisie par l'utilisateur
                    authosaleLine.Product = LensConstruction.GetProductByAuthSaleLine(authosaleLine, new FatSod.DataContext.Concrete.EFDbContext());
                    if (authosaleLine.Product == null)
                    {
                        Session["authorisesalelines"] = authosalelines;
                        res = true;
                        return res;
                    }
                    if ((authosaleLine.Product is GenericProduct))
                    {
                        // if (saleLine.Product.CategoryID==2 )
                        res = this.CheckQty(authosaleLine.LocalizationID, authosaleLine.Product.ProductID, authosaleLine.NumeroSerie, authosaleLine.marque, authosaleLine.LineQuantity, spray, boitier);
                        if (!res)
                            return res;
                    }
                    if (authosaleLine.LineID > 0)
                    {
                        //Ce produit existe deja dans le panier, alors on enleve les deux lignes liées au SpecialOrderLineCode dans la ligne
                        //1-Coe c'est une modification, on enlève l'existant de la ligne en cours de modification; on va l'ajouter dans la suite(Drop and Create)

                        authosalelines.RemoveAll(col => col.LineID == authosaleLine.LineID);
                        //2-Si actuellement on a une seule ligne dans la collection, il y a une possibilité qu'on en avait deux et l'autre a été supprimée; il faut donc le supprimer dans le panier
                        if (cols.Count <= 1) authosalelines.RemoveAll(col => col.SpecialOrderLineCode == authosaleLine.SpecialOrderLineCode);
                    }

                    if (authosalelines != null && authosalelines.Count() > 0)
                    {
                        AuthoriseSaleLine saleLineExist = authosalelines.FirstOrDefault(s => s.Product.ProductCode == authosaleLine.Product.ProductCode && s.SpecialOrderLineCode == authosaleLine.SpecialOrderLineCode && s.EyeSide == authosaleLine.EyeSide);
                        if (saleLineExist != null)
                        {
                            authosalelines.Remove(saleLineExist);
                        }

                        int maxLineID = (authosalelines != null && authosalelines.Count() > 0) ? authosalelines.Select(l => l.LineID).Max() : 0;

                        authosaleLine.LineID = (maxLineID + 1);

                        authosalelines.Add(authosaleLine);
                    }
                    else
                    {
                        authosalelines = new List<AuthoriseSaleLine>();
                        authosaleLine.LineID = 1;
                        authosalelines.Add(authosaleLine);
                    }
                }

                //double VatRate = CodeValue.Accounting.ParamInitAcct.VATRATE;
                //ApplyExtraPrices(salelines, reduction, discount, transport, VatRate);
                Session["authorisesalelines"] = authosalelines;

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
        public bool CheckQty(int LocalizationID, int? ProductID, string NumeroSerie, string Marque, double LineQuantity, int spray, int boitier)
        {
            bool res = false;
            Session["SessionMessage"] = "OK";
            try
            {
                //if (spray > 0 || boitier > 0)
                //{
                double currentQteEnStock = this.GetQuantityStock(LocalizationID.ToString(), ProductID.ToString(), NumeroSerie, Marque);
                //}


                //double currentQteEnStock = (double)Session["MaxValue"];

                //recherche des qtes commandes en attente de validation pr ce produit et cette localization
                double qtyComNonValide = 0d;

                bool isStockControl = (bool)Session["isStockControl"];

                if (isStockControl)
                {
                    double safetyQty = (double)Session["SafetyStock"];
                    if (currentQteEnStock - (qtyComNonValide + LineQuantity) <= 0) //plus de produit en stock
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
                Session["SessionMessage"] = "Error " + e.Message;
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


        //public int GetLensID(int DepartureLocalizationID, int ProductCategoryID, int ProductNumberID)
        //{
        //    var lstLensProduct = db.Lenses.Join(db.Products, ls => ls.ProductID, p => p.ProductID,
        //                (ls, p) => new { ls, p }).
        //                Join(db.ProductLocalizations, pr => pr.p.ProductID, pl => pl.ProductID, (pr, pl) => new { pr, pl })
        //                .Where(lsp => lsp.pl.LocalizationID == DepartureLocalizationID
        //                && lsp.pr.ls.LensNumberID == ProductNumberID && lsp.pr.p.CategoryID == ProductCategoryID)
        //                .Select(s => new
        //                {
        //                    ProductID = s.pr.p.ProductID,
        //                    ProductCode = s.pr.p.ProductCode,
        //                    ProductLabel = s.pr.p.ProductLabel,
        //                    ProductQuantity = s.pl.ProductLocalizationStockQuantity,
        //                    ProductLocalizationStockSellingPrice = s.pl.ProductLocalizationStockSellingPrice,
        //                    ProductLocalizationSafetyStockQuantity = s.pl.ProductLocalizationSafetyStockQuantity
        //                }).FirstOrDefault();

        //    Session["MaxValue"] = lstLensProduct.ProductQuantity;
        //    Session["LineUnitPrice"] = lstLensProduct.ProductLocalizationStockSellingPrice;
        //    Session["CurrentProduct"] = lstLensProduct.ProductLabel;
        //    Session["SafetyStock"] = lstLensProduct.ProductLocalizationSafetyStockQuantity;
        //    return lstLensProduct.ProductID;

        //}


        /*public JsonResult OnProductSelected(int? Localization, int? CurrentProduct)
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
                FrameLineQuantity = FrameLineQuantity
            });
            return Json(_InfoList, JsonRequestBehavior.AllowGet);

        }*/

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
                    LensLineQuantity = 2,
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
        public JsonResult AuthoSaleLines()
        {
            List<AuthoriseSaleLine> authosalelines = (List<AuthoriseSaleLine>)Session["authorisesalelines"];
            var model = new
            {
                data = from c in authosalelines
                select
                new
                {
                    LineID = c.LineID,
                    LineAmount = c.LineAmount,
                    LineQuantity = c.LineQuantity,
                    ProductLabel = c.ProductLabel,
                    LineUnitPrice = c.LineUnitPrice,
                    SupplyingName = c.SupplyingName
                }
            };
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        //This method confirm a sale
        //[HttpPost]
        public JsonResult AddAuthoriseSale(SpecialLensModel slm, AuthoriseSale currentSale,  string heureVente,DateTime DateOfBirth, int spray = 0, int boitier = 0)
        {
            bool status = false;
            string Message = "";
            try
            {
                var profile = (int)Session["UserProfile"];
                bool cannotAdd = LoadAction.IsMenuActionAble(MenuAction.ADD, @profile, CodeValue.CashRegister.AuthorizeSale, db);

                if (cannotAdd == true)
                {
                    Message = "Sorry You don't have right to add. Please See Administrator for more details";
                    status = false;
                    return new JsonResult { Data = new { status = status, Message = Message } };
                }

                Session["SaleID"] = 0;
                Session["authorisesalelines"] = new List<AuthoriseSaleLine>();
                //fabrication des lignes de commande
                status = this.DoYes(slm, spray, boitier);
                if (!status)
                {
                    Message = (string)Session["SessionMessage"];
                    status = false;
                    return new JsonResult { Data = new { status = status, Message = Message } };
                }

               
                currentSale.IsSpecialOrder = false;
                

                currentSale.AuthoriseSaleLines = (List<AuthoriseSaleLine>)Session["authorisesalelines"];
                currentSale.DateOfBirth = DateOfBirth;


                if (currentSale.AuthoriseSaleLines.Count > 0)
                {
                    
                    int SaleID = _authoriseSaleRepository.SaveChanges(currentSale, heureVente, SessionGlobalPersonID,false).AuthoriseSaleID;
                    
                    PrintReset(currentSale.BranchID.ToString(), SaleID);
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

        public void PrintReset(string Branch, int SaleID)
        {
            Session["authorisesalelines"] = new List<AuthoriseSaleLine>();
            Session["CurrentProduct"] = new Product();
            Session["MaxValue"] = 0;

            Session["SaleID"] = SaleID;
           
        }

        private List<AuthoriseSale> ModelCommand
        {
            get
            {
                List<AuthoriseSale> model = new List<AuthoriseSale>();
                db.AuthoriseSales.Where(c => !c.IsDelivered && !c.IsSpecialOrder)
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

        public JsonResult DeleteCommand(int ID)
        {
            bool status = false;
            string Message = "";
            try
            {
                var profile = (int)Session["UserProfile"];
                bool cannotDelete = LoadAction.IsMenuActionAble(MenuAction.DELETE, @profile, CodeValue.CashRegister.AuthorizeSale, db);

                if (cannotDelete == true)
                {
                    Message = "Sorry You don't have right to delete. Please See Administrator for more details";
                    status = false;
                    return new JsonResult { Data = new { status = status, Message = Message } };
                }
                AuthoriseSale custord = db.AuthoriseSales.Find(ID);

                //if (custord.SaleDate == SessionBusinessDay(null).BDDateOperation)
                //{
                    db.AuthoriseSaleLines.Where(ol => ol.AuthoriseSaleID == ID).ToList().ForEach(ol =>
                    {
                        db.AuthoriseSaleLines.Remove(ol);
                        db.SaveChanges();
                    });
                    db.AuthoriseSales.Remove(custord);
                    db.SaveChanges();
                    status = true;
                    Message = Resources.Success + " - Command has been deleted";
                //}
                //else
                //{
                //    status = false;
                //    Message = "Error to delete command beacause of different bussiness day";
                //}

            }
            catch (Exception e)
            {
                Message = "Error " + e.Message;
                status = false;
            }
            return new JsonResult { Data = new { status = status, Message = Message } };
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

        public JsonResult InitializeCommandFields(int ID)
        {
            List<object> _CommandList = new List<object>();
            try
            {

                AuthoriseSale autSale = (from c in db.AuthoriseSales
                                         where c.AuthoriseSaleID == ID && !c.IsDelivered
                                         select c).SingleOrDefault();
                if (autSale == null)
                {
                    TempData["Message"] = "Warning - This sale is already validate";
                    return Json(_CommandList, JsonRequestBehavior.AllowGet);
                }

                string ProductName = "", marque = "", reference = "", NumeroSerie="";
                int ProductID = 0, LocalizationID = 0;
                //int spray = 0;
                //int boitier = 0;
                string RESphere = "",
                        RECylinder = "",
                        REAxis = "",
                        REAddition = "",
                        LESphere = "",
                        LECylinder = "",
                        LEAxis = "",
                        LEAddition = "";

                string SupplyingName = "", LensCategoryCode = "0", TypeLens = "";
                double FramePrice = 0d, FrameLineQuantity = 0d, StockQuantity = 0d;
                double LensPrice = 0d, LensLineQuantity = 0d, LineUnitPrice = 0d;

                List<AuthoriseSaleLine> authoriseSaleLines = db.AuthoriseSaleLines.Where(co => co.AuthoriseSaleID == ID).ToList();

                foreach (AuthoriseSaleLine authosaleLine in authoriseSaleLines)
                {
                    if (!(authosaleLine.Product is OrderLens))
                    {
                        //frame - spray or boitier
                        if (authosaleLine.reference != null || authosaleLine.marque != null) //frame
                        {
                            ProductName = authosaleLine.Product.ProductCode;
                            ProductID = authosaleLine.Product.ProductID;
                            StockQuantity = GetQuantityStock(authosaleLine.Localization.LocalizationID.ToString(), authosaleLine.Product.ProductID.ToString(), authosaleLine.NumeroSerie,authosaleLine.marque);
                            marque = authosaleLine.marque;
                            reference = authosaleLine.reference;
                            NumeroSerie = authosaleLine.NumeroSerie;
                            FramePrice = authosaleLine.LineAmount;
                            FrameLineQuantity = authosaleLine.LineQuantity;
                        }
                       
                    }
                    else
                    {
                        LensCategoryCode = authosaleLine.Product.Category.CategoryCode;
                        LensCategory cat = (from cate in db.LensCategories
                                            where cate.CategoryCode == LensCategoryCode
                                            select cate).SingleOrDefault();

                        SupplyingName = (cat.SupplyingName != null && cat.SupplyingName.Length > 0) ? cat.SupplyingName : cat.CategoryCode;
                        TypeLens = cat.TypeLens;
                        LensPrice += authosaleLine.LineAmount;
                        LensLineQuantity += authosaleLine.LineQuantity;

                        if (authosaleLine.OeilDroiteGauche == EyeSide.OD)
                        {
                            RESphere = authosaleLine.LensNumberSphericalValue;
                            RECylinder = authosaleLine.LensNumberCylindricalValue;
                            REAxis = authosaleLine.Axis;
                            REAddition = authosaleLine.Addition;
                        }
                        if (authosaleLine.OeilDroiteGauche == EyeSide.OG)
                        {
                            LESphere = authosaleLine.LensNumberSphericalValue;
                            LECylinder = authosaleLine.LensNumberCylindricalValue;
                            LEAxis = authosaleLine.Axis;
                            LEAddition = authosaleLine.Addition;
                        }

                    }
                    LocalizationID = authosaleLine.LocalizationID;
                }

                LineUnitPrice = FramePrice + LensPrice;
                _CommandList.Add(new
                {
                    AuthoriseSaleID = autSale.AuthoriseSaleID,
                    CustomerName = autSale.Customer.Name,
                    PreferredLanguage = autSale.Customer.PreferredLanguage,
                    CustomerID = autSale.CustomerID,
                    Remarque = autSale.Remarque,
                    MedecinTraitant = autSale.MedecinTraitant,
                    //spray = spray,
                    //boitier = boitier,
                    BranchID = autSale.BranchID,

                    ProductName = ProductName,
                    ProductID = ProductID,
                    StockQuantity = StockQuantity,
                    marque = marque,
                    reference = reference,
                    FramePrice = FramePrice,
                    FrameLineQuantity = FrameLineQuantity,
                    NumeroSerie=NumeroSerie,

                    SupplyingName = SupplyingName,
                    LensCategoryCode = LensCategoryCode,
                    LensPrice = LensPrice,
                    LensLineQuantity = LensLineQuantity,

                    LineUnitPrice = LineUnitPrice,

                    SaleReceiptNumber = autSale.SaleReceiptNumber,
                    SaleDate = autSale.SaleDate.ToString("yyyy-MM-dd"),
                    SaleDeliveryDate = SessionBusinessDay(autSale.BranchID).BDDateOperation.ToString("yyyy-MM-dd"),

                    RESphere = RESphere,
                    RECylinder = RECylinder,
                    REAxis = REAxis,
                    REAddition = REAddition,
                    LESphere = LESphere,
                    LECylinder = LECylinder,
                    LEAxis = LEAxis,
                    LEAddition = LEAddition,
                    TypeLens = TypeLens,

                    LocalizationID = LocalizationID,
                    SalesProductsType = 1
                });
                return Json(_CommandList, JsonRequestBehavior.AllowGet);


            }
            catch (Exception e)
            {
                TempData["Message"] = "Error " + e.Message;
                return Json(_CommandList, JsonRequestBehavior.AllowGet);
            }
        }

        public double GetQuantityStock(string Localization, string CurrentProduct, string NumeroSerie,string Marque)
        {
            double LineQuantity = 0d;
            
            if ((Localization != null && CurrentProduct != null) && (Localization.Length > 0 && CurrentProduct.Length > 0))
            {
                int idLoc = Convert.ToInt32(Localization);
                int idProd = Convert.ToInt32(CurrentProduct);
                //check if it is product with serial number
                Category productcat = db.Products.Find(idProd).Category;
                ProductLocalization productInStock = new ProductLocalization();

                if (idLoc > 0 && idProd > 0)
                {
                   // ProductLocalization productInStock = db.ProductLocalizations.FirstOrDefault(pL => pL.LocalizationID == idLoc && pL.ProductID == idProd);
                    if (!(productcat.isSerialNumberNull))
                    {
                        productInStock = db.ProductLocalizations.FirstOrDefault(pL => pL.LocalizationID == idLoc && pL.ProductID == idProd);
                    }
                    else
                    {
                        //if ((NumeroSerie != null && NumeroSerie != null))
                        //{
                        productInStock = db.ProductLocalizations.FirstOrDefault(pL => pL.LocalizationID == idLoc && pL.ProductID == idProd && pL.NumeroSerie == NumeroSerie && pL.Marque== Marque);
                        //}
                    }
                    Session["CurrentProduct"] = productInStock.ProductCode;
                    if (productInStock == null || Math.Abs(productInStock.ProductLocalizationStockQuantity) <= 0)
                    {
                        LineQuantity = 0d;
                        Session["MaxValue"] = 0d;
                        Session["SafetyStock"] = 0d;
                    }
                    else
                    {
                        LineQuantity = productInStock.ProductLocalizationStockQuantity;
                        Session["MaxValue"] = productInStock.ProductLocalizationStockQuantity;
                        Session["SafetyStock"] = productInStock.ProductLocalizationSafetyStockQuantity;
                    }
                }
            }
            
            return LineQuantity;

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
            if (DepartureLocalizationID <= 0) //chargement des produits en fct du magasin slt
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

        public JsonResult populateMarqueters()
        {
            List<object> marketers = new List<object>();
            foreach (User user in db.People.OfType<User>().Where(u => u.IsMarketer).ToArray().OrderBy(c => c.Name))
            {
                marketers.Add(new { Name = user.UserFullName, ID = user.GlobalPersonID });
            }

            return Json(marketers, JsonRequestBehavior.AllowGet);
        }

    }
}