using FatSod.Security.Abstracts;
using FatSod.Security.Entities;
using FatSod.Supply.Entities;
using CABOPMANAGEMENT.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using FatSod.Supply.Abstracts;
using FatSod.Ressources;
using CABOPMANAGEMENT.Filters;
using FatSod.DataContext.Repositories;
using CABOPMANAGEMENT.Tools;

namespace CABOPMANAGEMENT.Areas.Sale.Controllers
{
    [Authorize(Order = 1)]
    [TakeBusinessDay(Order = 2)]
    public class MenuProformaController : BaseProformaController
    {
        //Current Controller and current page
        private const string CONTROLLER_NAME = "Sale/MenuProforma";
        private const string VIEW_NAME = "Index";
        //person repository

        private ICustomerOrder _CustomerOrderRepository;
        private IBusinessDay _busDayRepo;
        
        private ILensNumberRangePrice _priceRepository;

        private List<BusinessDay> listBDUser;
        private IProductLocalization _ProductLocalization;

        private LensConstruction lensFrameConstruction = new LensConstruction();
        //Construcitor
        public MenuProformaController(
            ICustomerOrder CustomerOrderRepository,
            IBusinessDay busDayRepo,
            ILensNumberRangePrice lnrpRepo,
            IProductLocalization ProductLocalization,
            IAccount accountRepository, IPerson personRepository
            ) : base(accountRepository, personRepository)
        {
            this._CustomerOrderRepository = CustomerOrderRepository;
            this._busDayRepo = busDayRepo;
            this._priceRepository = lnrpRepo;
            this._ProductLocalization = ProductLocalization;
        }
        // GET: 
        [OutputCache(Duration = 3600)] 
        public ActionResult Index()
        {

            ViewBag.DisplayForm = 1;
            try
            {
                ViewBag.Disabled = true;

                List<BusinessDay>  bdDay = (List<BusinessDay>)Session["UserBusDays"];
                if (bdDay == null)
                {
                    bdDay = _busDayRepo.GetOpenedBusinessDay(CurrentUser);
                }
                if (bdDay.Count() > 1)
                {
                    TempData["Message"] = "Wrong Business day.<br/>contact our administrator for this purpose<code/>.";
                    ViewBag.Disabled = false;
                    ViewBag.DisplayForm = 0;
                }
                DateTime busDays = bdDay.FirstOrDefault().BDDateOperation;

                ViewBag.BusnessDayDate = busDays;
                ViewBag.CurrentBranch = bdDay.FirstOrDefault().BranchID;

                Session["customerOrderLines"] = new List<CustomerOrderLine>();
            
                Session["isUpdate"] = false;
                Session["MaxValue"] = 500;
                Session["SafetyStock"] = 500;

                int deviseID = (Session["DefaultDeviseID"] == null) ? 0 : (int)Session["DefaultDeviseID"];
                if (deviseID <= 0)
                {
                    InjectUserConfigInSession();
                }

                return View();
            }

            catch (Exception e)
            {
                TempData["Message"] = "Error " + e.Message;
                ViewBag.DisplayForm = 0;
                return this.View();
            }
        }

        public JsonResult LoadCompanyName(string filter)
        {

            List<object> customersList = new List<object>();
            foreach (InsuredCompany customer in db.InsuredCompanies.Where(c => c.InsuredCompanyCode.Contains(filter.ToLower())).Take(100).ToArray().OrderBy(c => c.InsuredCompanyCode))
            {
                string itemLabel = "";

                //si le client est une personne physique, on renvoie son nom et son prénom, si c'est une entreprise, on renvoie son nom
                itemLabel = customer.InsuredCompanyCode;

                customersList.Add(new
                {
                    Name = itemLabel,
                    ID = customer.InsuredCompanyID
                });
            }

            return Json(customersList, JsonRequestBehavior.AllowGet);
        }

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
                    SupplyingName = (cat.SupplyingName != null && cat.SupplyingName.Length > 0) ? cat.SupplyingName : cat.CategoryDescription,
                    TypeLens=cat.TypeLens
                });
            }
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

        public JsonResult GetAllSerialNumber(string filter, int DepartureLocalizationID = 0)
        {

            List<object> customersList = new List<object>();
            List<ProductLocalization> res = new List<ProductLocalization>();
            //IEqualityComparer<ProductLocalization> locationComparer = new GenericComparer<ProductLocalization>("NumeroSerie");

            res = db.ProductLocalizations.Where(c => c.LocalizationID == DepartureLocalizationID && c.NumeroSerie.Contains(filter.ToLower())).Take(200).ToList();
            foreach (ProductLocalization prodLocalization in res)
            {
                string itemLabel = "";

                //si le client est une personne physique, on renvoie son nom et son prénom, si c'est une entreprise, on renvoie son nom
                itemLabel = prodLocalization.NumeroSerie;

                customersList.Add(new
                {
                    Name = itemLabel,
                    ID = prodLocalization.ProductLocalizationID
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
                Session["CurrentProduct"] = prodLocalization.ProductLabel;
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

        public JsonResult populateUsers()
        {
            //List<object> userList = new List<object>();
            //foreach (User user in db.People.OfType<User>().Where(u => /*u.IsConnected &&*/ u.UserAccessLevel >= 1).ToArray())
            /*foreach (User user in db.People.OfType<User>().Where(u => u.IsMarketer).ToArray().OrderBy(c => c.Name))
            {
                userList.Add(new { Name = user.UserFullName, ID = user.GlobalPersonID });
            }*/

            return Json(LoadComponent.GetMarketters(CurrentBranch.BranchID), JsonRequestBehavior.AllowGet);
        }
        public JsonResult populateSellerUsers()
        {
            /*
            List<object> userList = new List<object>();
            foreach (User user in db.People.OfType<User>().Where(u => u.IsSeller).ToArray().OrderBy(c => c.Name))
            {
                userList.Add(new { Name = user.UserFullName, ID = user.GlobalPersonID });
            }*/

            return Json(LoadComponent.GetSellers(CurrentBranch.BranchID), JsonRequestBehavior.AllowGet);
        }
        //This method add a CustomerOrderLine in the current sale
        public bool DoYes(SpecialLensModel slm)
        {
            bool res = false;
            try
            {
                Session["SessionMessage"] = "OK";
                
                List<CustomerOrderLine> customerOrderLines = (List<CustomerOrderLine>)Session["customerOrderLines"];
                List<CustomerOrderLine> cols = lensFrameConstruction.Get_COL_CUSTORDER_From_SLM(slm, new FatSod.DataContext.Concrete.EFDbContext());
                foreach (CustomerOrderLine customerOrderLine in cols)
                {
                    //customerOrderLine.isCommandGlass = false;
                    //Construction du code du produit en fonction de ce qui a été saisie par l'utilisateur
                    customerOrderLine.Product = LensConstruction.GetProductByCustOrderLine(customerOrderLine, new FatSod.DataContext.Concrete.EFDbContext());
                    if (customerOrderLine.Product == null)
                    {
                        Session["customerOrderLines"] = customerOrderLines;
                        res = true;
                        return res;
                    }
                    if (!(customerOrderLine.Product is OrderLens))
                    {
                        //customerOrderLine.isCommandGlass = false;
                        //res = this.CheckQty(customerOrderLine.LocalizationID, customerOrderLine.Product.ProductID, customerOrderLine.NumeroSerie, customerOrderLine.LineQuantity);
                        //if (!res)
                        //    return res;
                    }
                    else
                    {
                        //customerOrderLine.isCommandGlass = true;
                    }
                    if (customerOrderLine.LineID > 0)
                    {
                        //Ce produit existe deja dans le panier, alors on enleve les deux lignes liées au SpecialOrderLineCode dans la ligne
                        //1-Coe c'est une modification, on enlève l'existant de la ligne en cours de modification; on va l'ajouter dans la suite(Drop and Create)

                        customerOrderLines.RemoveAll(col => col.LineID == customerOrderLine.LineID);
                        //2-Si actuellement on a une seule ligne dans la collection, il y a une possibilité qu'on en avait deux et l'autre a été supprimée; il faut donc le supprimer dans le panier
                        if (cols.Count <= 1) customerOrderLines.RemoveAll(col => col.SpecialOrderLineCode == customerOrderLine.SpecialOrderLineCode);
                    }

                    if (customerOrderLines != null && customerOrderLines.Count() > 0)
                    {
                        CustomerOrderLine customerOrderLineExist = customerOrderLines.FirstOrDefault(s => s.Product.ProductCode == customerOrderLine.Product.ProductCode && s.SpecialOrderLineCode == customerOrderLine.SpecialOrderLineCode && s.EyeSide==customerOrderLine.EyeSide);
                        if (customerOrderLineExist != null)
                        {
                            customerOrderLines.Remove(customerOrderLineExist);
                        }

                        int maxLineID = (customerOrderLines != null && customerOrderLines.Count() > 0) ? customerOrderLines.Select(l => l.LineID).Max() : 0;

                        customerOrderLine.LineID = (maxLineID + 1);

                        customerOrderLines.Add(customerOrderLine);
                    }
                    else
                    {
                        customerOrderLines = new List<CustomerOrderLine>();
                        customerOrderLine.LineID = 1;
                        customerOrderLines.Add(customerOrderLine);
                    }
                }

                
                Session["customerOrderLines"] = customerOrderLines;

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
        /*
                public ActionResult RActivateAxe(string CylinderVal)
                {
                    if (CylinderVal==null || CylinderVal.Length==0)
                    {
                        this.GetCmp<TextField>("REAxis").ReadOnly = true;
                    }
                    else
                    {
                        this.GetCmp<TextField>("REAxis").ReadOnly = false;
                    }
                    return this.Direct();
                }
                public ActionResult LActivateAxe(string CylinderVal)
                {
                    if (CylinderVal == null || CylinderVal.Length == 0)
                    {
                        this.GetCmp<TextField>("LEAxis").ReadOnly = true;
                    }
                    else
                    {
                        this.GetCmp<TextField>("LEAxis").ReadOnly = false;
                    }
                    return this.Direct();
                }
                */

        
        // [DirectMethod]
        //[HttpPost]
        public bool CheckQty(int LocalizationID, int? ProductID, string NumeroSerie, double LineQuantity)
        {
            bool res = false;
            Session["SessionMessage"] = "OK";
            try
            {
                this.GetQuantityStock(LocalizationID.ToString(), ProductID.ToString(), NumeroSerie);

                double currentQteEnStock = (double)Session["MaxValue"];

                //recherche des qtes commandes en attente de validation pr ce produit et cette localization
                double qtyComNonValide = 0;

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
        

        
        public JsonResult CustomerOrderLines()
        {
            List<CustomerOrderLine> customerOrderLines = (List<CustomerOrderLine>)Session["customerOrderLines"];
            var model = new
            {
                data = from c in customerOrderLines select
                new
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

        public JsonResult getListofDepositLocations()
        {
            List<LieuxdeDepotBordero> lieuxDeptBord = db.LieuxdeDepotBorderos.OrderBy(c => c.LieuxdeDepotBorderoID).ToList();
            List<object> lieuxDeptBordList = new List<object>();
            foreach (LieuxdeDepotBordero ldb in lieuxDeptBord)
            {
                lieuxDeptBordList.Add(new { Name = ldb.LieuxdeDepotBorderoName, ID = ldb.LieuxdeDepotBorderoID });
            }
            return Json(lieuxDeptBordList, JsonRequestBehavior.AllowGet);
        }
        public JsonResult populateAssurance()
        {
            List<Assureur> assureur = db.Assureurs.Where(ass => ass.Name.ToLower() != "default").OrderBy(ass =>ass.Name.Trim()).ToList();
            List<object> assureurList = new List<object>();
            foreach (Assureur ass in assureur)
            {
                assureurList.Add(new { Name = ass.Name, ID = ass.GlobalPersonID });
            }
            return Json(assureurList, JsonRequestBehavior.AllowGet);
        }

        //This method confirm a sale

        public JsonResult AddAssureSale(SpecialLensModel slm, CustomerOrder customerOrder)
        {
           
            Session["customerOrderLines"] = new List<CustomerOrderLine>();
            bool status = false;
            string Message = "";
            try
            {
                //fabrication des lignes de commande
                status = this.DoYes(slm);
                if (!status)
                {
                    Message = (string)Session["SessionMessage"];
                    status = false;
                    return new JsonResult { Data = new { status = status, Message = Message } };
                }
                #region Creation du client dans la table Customer et utilisation de son ID
                var res = this.CreateCustomer(customerOrder);
                if (!(res is int))
                {

                    Message = (res is String) ? ((String)res) : ((Exception)res).Message;
                    status = false;
                    return new JsonResult { Data = new { status = status, Message = Message } };
                }
                customerOrder.CustomerID = (int)res;
                #endregion

                customerOrder.CustomerOrderLines = (List<CustomerOrderLine>)Session["customerOrderLines"];
                int deviseID = (Session["DefaultDeviseID"] == null) ? 0 : (int)Session["DefaultDeviseID"];
                customerOrder.DeviseID = deviseID;
                customerOrder.BillState = StatutFacture.Proforma;
                customerOrder.ValidateBillDate = SessionBusinessDay(null).BDDateOperation;
                int CustomerId= _CustomerOrderRepository.SaveChanges(customerOrder, SessionGlobalPersonID).CustomerOrderID;
                Session["Receipt_CommandID"] = CustomerId;
                PrintReset();
                status = true;
                Message = Resources.Success + " - " + Resources.NewProforma;
            }
            catch (Exception e)
            {
                status = false;
                Message = "Error " + e.Message + " " + e.StackTrace + " " + e.InnerException;
            }
            return new JsonResult { Data = new { status = status, Message = Message } };
        }

        public void PrintReset()
        {
            Session["customerOrderLines"] = new List<CustomerOrderLine>();
            Session["CurrentProduct"] = new Product();
            Session["MaxValue"] = 0;
           
        }

        
        public double GetQuantityStock(string Localization, string CurrentProduct, string NumeroSerie)
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
                        productInStock = db.ProductLocalizations.FirstOrDefault(pL => pL.LocalizationID == idLoc && pL.ProductID == idProd && pL.NumeroSerie == NumeroSerie);
                        //}
                    }
                    Session["CurrentProduct"] = productInStock.ProductLabel;
                    if (productInStock == null || Math.Abs(productInStock.ProductLocalizationStockQuantity) <= 0)
                    {
                        LineQuantity = 0d;
                        Session["MaxValue"] = 0d;
                    }
                    else
                    {
                        LineQuantity = productInStock.ProductLocalizationStockQuantity;
                        Session["MaxValue"] = productInStock.ProductLocalizationStockQuantity;
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

        public List<Product> ModelProductLocalCat(int DepartureLocalizationID/*, int? ProductCategoryID, int? ProductNumberID*/)
        {
            List<Product> model = new List<Product>();


            //On a un produit générique
            if (DepartureLocalizationID <= 0 /*&& (ProductCategoryID == 0 || ProductCategoryID == null) && (ProductNumberID == 0 || ProductNumberID == null)*/) //chargement des produits en fct du magasin slt
            {
                return model;
            }
            else //On a un produit de type verre
            {
                // verifion si c'est un produit de type verre
              
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

                //}
            }

            return model;
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

        public JsonResult GetFrameMaterial()
        {

            List<object> FrameMaterial = new List<object>();
            //PLASTIQUE
            FrameMaterial.Add(new
            {
                ID = "PLASTIQUE",
                Name = Resources.PLASTIQUE
            });
            //METALLIQUE
            FrameMaterial.Add(new
            {
                ID = "METALLIQUE",
                Name = Resources.METALIQUE
            });

            //MIXTE
            FrameMaterial.Add(new
            {
                ID = "MIXTE",
                Name = Resources.MIX
            });

            return Json(FrameMaterial, JsonRequestBehavior.AllowGet);
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
                CustomerOrderDate = businessDay.BDDateOperation.ToString("yyyy-MM-dd"),
                MedecinTraitant = "",
                LocalizationID = db.Localizations.Where(b => b.BranchID == BranchID).FirstOrDefault().LocalizationID,
                SalesProductsType = 1
            });
            return Json(_InfoList, JsonRequestBehavior.AllowGet);
        }
        
    }
}