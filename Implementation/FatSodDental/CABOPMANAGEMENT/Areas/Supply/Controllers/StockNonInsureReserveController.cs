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
using SaleE = FatSod.Supply.Entities.Sale;
using FastSod.Utilities.Util;
using FatSod.DataContext.Initializer;

namespace CABOPMANAGEMENT.Areas.Supply.Controllers
{
    [Authorize(Order = 1)]
    [TakeBusinessDay(Order = 2)]
    public class StockNonInsureReserveController : BaseController
    {
        private List<BusinessDay> listBDUser;
        private IBusinessDay _busDayRepo;
        private ISavingAccount _savingAccountRepository;
        private IAccount _accountRepository;
        private IPerson _personRepository;

        private LensConstruction lensFrameConstruction = new LensConstruction();
        private ISale _saleRepository;
        //Construcitor
        public StockNonInsureReserveController(
            IPerson personRepository,
            ISale saleRepository,
            ISavingAccount saRepo,
            IBusinessDay busDayRepo,
            IAccount accountRepository
            )
        {
            this._personRepository = personRepository;
            this._saleRepository = saleRepository;
            this._busDayRepo = busDayRepo;
            this._savingAccountRepository = saRepo;
            this._accountRepository = accountRepository;
        }

        // GET: Supply/StockNonInsureReserve
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

                Session["salelinesnoninsured"] = new List<SaleLine>();

                Session["isUpdate"] = false;
                Session["MaxValue"] = 500;
                Session["SafetyStock"] = 500;
                //return View(ModelCommand(currentDateOp));
                return View();
            }

            catch (Exception e)
            {
                TempData["Message"] = "Error " + e.Message;
                ViewBag.DisplayForm = 0;
                return this.View();
            }
        }

        public JsonResult ModelCommand(DateTime Begindate, DateTime EndDate)
        {
            var model = new
            {
                data = from sale in this.ModelReturnAbleSales(Begindate, EndDate)
                    select new
                    {
                        SaleID = sale.SaleID,
                        SaleDate = sale.SaleDate.ToString("yyyy-MM-dd"),
                        SaleDeliveryDate = sale.SaleDeliveryDate.Value.ToString("yyyy-MM-dd"),
                        CustomerFullName = sale.CustomerFullName,
                        SaleReceiptNumber = sale.SaleReceiptNumber,
                        SaleTotalPrice = sale.SaleTotalPrice,
                        Advanced = sale.Advanced,
                        Remainder = sale.Remainder
                    }
            };
            //return Json(model, JsonRequestBehavior.AllowGet);
            var jsonResult = Json(model, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
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
            //RECUPERATION DU PRODUIT SPECIAL POUR LES RESERVES
            _InfoList.Add(new
            {
                SaleDate = businessDay.BDDateOperation.ToString("yyyy-MM-dd"),
                MedecinTraitant = "VALDOZ OPTIC",
                LocalizationID = db.Localizations.Where(b => b.BranchID == BranchID).FirstOrDefault().LocalizationID,
                SalesProductsType = 1
            });
            return Json(_InfoList, JsonRequestBehavior.AllowGet);
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
                var lstLensProduct = db.GenericProducts
                .Where(lsp => lsp.CategoryID == 1 && !(lsp.Category is LensCategory))
                .Select(s => new
                {
                    ProductID = s.ProductID,
                    ProductCode = s.ProductCode,
                    ProductLabel = s.ProductLabel
                }).ToList();
                /*
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
                */
                foreach (var pt in lstLensProduct)
                {
                    model.Add(
                        new Product
                        {
                            ProductID = pt.ProductID,
                            ProductCode = pt.ProductCode,
                            ProductLabel = pt.ProductLabel
                        }
                        );
                }

            }

            return model;
        }

        //retourne les ventes non delivre
        private List<SaleE> ModelReturnAbleSales(DateTime To, DateTime At)
        {

            double Advanced = 0d;
            double Remainder = 0d;

            List<SaleE> model = new List<SaleE>();

            /*foreach (var s in db.vcumulRealSales.Where(s => (s.SaleDate >= To.Date && s.SaleDate <= At.Date) && s.SaleTotalPrice > s.Advanced).OrderBy(o => o.SaleID).ToList())
                {
                    
                        model.Add(
                                new SaleE
                                {
                                    SaleID = s.SaleID,
                                    SaleDate = s.SaleDate,
                                    SaleDeliveryDate = (s.SaleDeliveryDate.HasValue) ? s.SaleDeliveryDate.Value : s.SaleDate,
                                    CustomerFullName = s.Name,
                                    SaleReceiptNumber = s.SaleReceiptNumber,
                                    SaleTotalPrice = s.SaleTotalPrice,
                                    Advanced = s.Advanced,
                                    Remainder = s.SaleTotalPrice - s.Advanced
                                }
                                );
                    }
                
            */
            
                            var lstallSales = from bc in db.viewRealSales
                                            where ((bc.SaleDate >= To.Date && bc.SaleDate <= At.Date)) // && (!bc.IsPaid) && (bc.isReturn == false))
                                            group bc by new { bc.SaleID, bc.SaleDate, bc.CustomerID, bc.Name, bc.SaleReceiptNumber, bc.SaleDeliveryDate, bc.Remarque, bc.MedecinTraitant } into g
                                            select new
                                            {
                                                key = g.Key,
                                                SaleTotalPrice = g.Sum(a => (a.LineUnitPrice * a.SaleQty))
                                            };



                        foreach (var s in lstallSales.ToList())
                        {
                            List<CustomerSlice> lstCustSlice = db.CustomerSlices.Where(sl => sl.SaleID == s.key.SaleID).ToList();
                            if (lstCustSlice != null && lstCustSlice.Count > 0)
                            {
                                Advanced = lstCustSlice.Select(sl => sl.SliceAmount).Sum();
                            }
                            else
                            {
                                Advanced = 0;
                            }
                            Remainder = s.SaleTotalPrice - Advanced;
                            if (Remainder > 0)
                            {
                                model.Add(
                                        new SaleE
                                        {
                                            SaleID = s.key.SaleID,
                                            SaleDate = s.key.SaleDate,
                                            SaleDeliveryDate = (s.key.SaleDeliveryDate.HasValue) ? s.key.SaleDeliveryDate.Value : s.key.SaleDate,
                                            CustomerFullName = s.key.Name,
                                            SaleReceiptNumber = s.key.SaleReceiptNumber,
                                            SaleTotalPrice = s.SaleTotalPrice,
                                            Advanced = Advanced,
                                            Remainder = Remainder
                                        }
                                        );
                            }
                        }

                            

            //double SaleTotalPrice = 0d;
            //var allSales = db.Sales
            //        .Where(sa => (sa.SaleDate >= To.Date && sa.SaleDate <= At.Date) && (!sa.IsPaid /*|| !sa.SaleDeliver*/) && (sa.isReturn == false))
            //        .ToList()
            //        .Select(s => new SaleE
            //        {
            //            SaleID = s.SaleID,
            //            SaleLines = s.SaleLines,
            //            RateReduction = s.RateReduction,
            //            Transport = s.Transport,
            //            VatRate = s.VatRate,
            //            SaleDate = s.SaleDate,
            //            SaleDeliveryDate = s.SaleDeliveryDate,
            //            CustomerName = s.CustomerName,
            //            SaleReceiptNumber = s.SaleReceiptNumber
            //        }).AsQueryable();

            ////il faut mainteant vérifier si la vente à encore au moins une ligne de vente pouvant faire l'objet d'un retour
            //foreach (SaleE s in allSales)
            //{

            //    if (s.SaleLines == null || s.SaleLines.Count <= 0) { continue; }
            //    SaleTotalPrice = Util.ExtraPrices(s.SaleLines.Select(sl => sl.LineAmount).Sum(), s.RateReduction, s.RateDiscount, s.Transport, s.VatRate).TotalTTC;
            //    List<CustomerSlice> lstCustSlice = db.CustomerSlices.Where(sl => sl.SaleID == s.SaleID).ToList();
            //    if (lstCustSlice != null && lstCustSlice.Count > 0)
            //    {
            //        Advanced = lstCustSlice.Select(sl => sl.SliceAmount).Sum();
            //    }
            //    else
            //    {
            //        Advanced = 0;
            //    }
            //    Remainder = SaleTotalPrice - Advanced;
            //    if (Remainder > 0)
            //    {
            //        model.Add(
            //            new SaleE
            //            {
            //                SaleID = s.SaleID,
            //                SaleDate = s.SaleDate,
            //                SaleDeliveryDate = (s.SaleDeliveryDate == null) ? new DateTime(1900, 1, 1) : s.SaleDeliveryDate,
            //                CustomerFullName = (s.CustomerName == null) ? "" : s.CustomerName,
            //                SaleReceiptNumber = (s.SaleReceiptNumber == null) ? "" : s.SaleReceiptNumber,
            //                SaleTotalPrice = SaleTotalPrice,
            //                Advanced = Advanced,
            //                Remainder = Remainder
            //            }
            //        );
            //    }

            //}


            return model;
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
                    ID = customer.GlobalPersonID
                });
            }

            return Json(customersList, JsonRequestBehavior.AllowGet);
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

        public JsonResult AddSale(SpecialLensModel slm, SaleE currentSale, CustomerSlice customerSlice, int? PaymentDelay,  string heureVente, int spray = 0, int boitier = 0, int SaleDeliver = 0,int FrameProductID=0)
        {
            bool status = false;
            string Message = "";
            try
            {
                Session["Receipt_SaleID"] = null;
                Session["Receipt_CustomerID"] = null;
                Session["ReceiveAmoung_Tot"] = null;
                Session["salelinesnoninsured"] = new List<SaleLine>();
                string BuyType = CodeValue.Supply.DepositReason.SavingAccount;

                if (BuyType == "" || BuyType == null)
                {
                    Message = "Wrong Payment Mode Select!!! " + Resources.MsgErrChoixPayementMethod;
                    status = false;
                    return new JsonResult { Data = new { status = status, Message = Message } };
                }

               
                //fabrication des lignes de commande
                status = this.DoYes(slm, spray, boitier);
                if (!status)
                {
                    Message = (string)Session["SessionMessage"];
                    status = false;
                    return new JsonResult { Data = new { status = status, Message = Message } };
                }
                currentSale.PostByID = SessionGlobalPersonID;

                if (SaleDeliver == 0) currentSale.SaleDeliver = false;
                if (SaleDeliver == 1) currentSale.SaleDeliver = true;

                currentSale.CustomerSlice = customerSlice;
                customerSlice.SliceDate = currentSale.SaleDate;
                currentSale.PaymentDelay = (PaymentDelay == null) ? 0 : PaymentDelay.Value;
                currentSale.IsSpecialOrder = false; //vente principale

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

                currentSale.SaleLines = (List<SaleLine>)Session["salelinesnoninsured"];

               

                //Si l'utilisateur souhaite payer en utilisant son compte d'épargne
                if (BuyType == CodeValue.Supply.DepositReason.SavingAccount)
                {

                    //Si l'aragent versé pour payer les achats est supérieurs au montant restant de l'achat
                    if (customerSlice.SliceAmount > currentSale.TotalPriceTTC)
                    {
                        Message = "More Money Than Expected - Sorry, You have put more Money Than Expected for this sale ";
                        status = false;
                        return new JsonResult { Data = new { status = status, Message = Message } };
                    }



                    //Customer savingCusto = db.Customers.Find(currentSale.CustomerID.Value);
                    
                    if (currentSale.CustomerID == null || currentSale.CustomerID.Value<=0)
                    {
                        ////creation du cpte client
                        //Customer customerEntity = new Customer();
                        
                        //fabrication du nvo cpte
                        CollectifAccount colAcct = db.CollectifAccounts.Where(c => c.AccountingSection.AccountingSectionCode == CodeValue.Accounting.DefaultCodeAccountingSection.CODECLIENT).FirstOrDefault();
                        int AccountID = _accountRepository.GenerateAccountNumber(colAcct.CollectifAccountID, currentSale.CustomerName + " " + Resources.UIAccount, false).AccountID;
                        Sex sexe = db.Sexes.FirstOrDefault();
                        Adress adresse = db.Adresses.FirstOrDefault();
                        Adress NewAdress = new Adress()
                        {
                            AdressPhoneNumber = currentSale.Remarque,
                            AdressCellNumber = currentSale.Remarque,
                            AdressFullName = adresse.AdressFullName,
                            AdressEmail = adresse.AdressEmail,
                            AdressWebSite = adresse.AdressWebSite,
                            AdressPOBox = adresse.AdressPOBox,
                            AdressFax = adresse.AdressFax,
                            QuarterID = adresse.QuarterID
                        };
                        Customer customer = new Customer()
                        {
                            AccountID = AccountID,
                            Name = currentSale.CustomerName,
                            SexID = sexe.SexID,
                            CNI = currentSale.CustomerName,
                            Adress = NewAdress
                        };

                        Customer savingCusto = (Customer)_personRepository.Create2(customer, SessionGlobalPersonID, currentSale.SaleDate, currentSale.BranchID);
                        currentSale.CustomerID = savingCusto.GlobalPersonID;
                    }

                    SavingAccount sa = db.SavingAccounts.SingleOrDefault(sa1 => sa1.CustomerID == currentSale.CustomerID.Value);

                    if (sa == null || sa.ID == 0)
                    {
                        //Message = "No Saving Account - Sorry, Customer doesn't have a Saving Account. Please contact an administrator ";
                        //status = false;
                        //return new JsonResult { Data = new { status = status, Message = Message } };
                        sa = _savingAccountRepository.CreateSavingAccount(currentSale.CustomerID.Value, currentSale.BranchID);
                    }

                    //double savingAccountBalance = _savingAccountRepository.GetSavingAccountBalance(savingCusto);

                    ////ne faites ps d'achat en espèce si : 1 - pas d'argent en caisse; 2- Facture > Montant en caisse
                    //if (savingAccountBalance <= 0 || customerSlice.SliceAmount > savingAccountBalance)
                    //{
                    //    Message = "NO Enough Money in Saving Account - Sorry, Customer doesn't have sufficient Money inside his Saving Account. Please contact an administrator";
                    //    status = false;
                    //    return new JsonResult { Data = new { status = status, Message = Message } };
                    //}
                    currentSale.PaymentMethodID = sa.ID;
                }
                if (currentSale.SaleLines.Count > 0)
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
                    int SaleID = _saleRepository.SaveChanges(currentSale, heureVente, SessionGlobalPersonID, false,true).SaleID;
                    Session["Receipt_SaleID"] = SaleID;
                    Session["Receipt_CustomerID"] = currentSale.CustomerName;
                    Session["ReceiveAmoung_Tot"] = (currentSale.CustomerSlice != null) ? currentSale.CustomerSlice.SliceAmount : 0;

                    PrintReset(currentSale.BranchID.ToString(), SaleID, customerSlice.SliceAmount);
                }

                status = true;
                Message = Resources.Success + " - " + Resources.SaleNewSale;
            }
            catch (Exception e)
            {
                Session["salelinesnoninsured"] = new List<SaleLine>();
                status = false;
                Message = "Error " + e.Message + " " + e.StackTrace + " " + e.InnerException;
            }
            return new JsonResult { Data = new { status = status, Message = Message } };
        }

        public JsonResult DeleteStockInsureReserve(int SaleID)
        {
            bool status = false;
            string Message = "";
            try
            {
                BusinessDay secbusday = SessionBusinessDay(null);
                _saleRepository.DeleteStockInsureReserve(SaleID, SessionGlobalPersonID, secbusday.BDDateOperation, secbusday.BranchID);
                status = true;
                Message = Resources.Success + " Data has been deleted";
            }
            catch (Exception e)
            {
                Message = "Error " + e.Message;
                status = false;
            }
            return new JsonResult { Data = new { status = status, Message = Message } };
        }

        public void PrintReset(string Branch, int SaleID, double SliceAmount)
        {
            Session["salelinesnoninsured"] = new List<SaleLine>();
            Session["CurrentProduct"] = new Product();
            Session["MaxValue"] = 0;


            //this.InitTrnNumber(Branch);

            Session["SaleID"] = SaleID;
            Session["SliceAmount"] = SliceAmount;


        }

        //This method add a saleline in the current sale

        public bool DoYes(SpecialLensModel slm, int spray, int boitier)
        {
            bool res = false;
            try
            {
                //double VatRate = CodeValue.Accounting.ParamInitAcct.VATRATE;
                Session["SessionMessage"] = "OK";
                List<SaleLine> salelines = (List<SaleLine>)Session["salelinesnoninsured"];
                List<SaleLine> cols = lensFrameConstruction.Get_COL_From_SLM(slm, new FatSod.DataContext.Concrete.EFDbContext(), spray, boitier);
                foreach (SaleLine saleLine in cols)
                {

                    //Construction du code du produit en fonction de ce qui a été saisie par l'utilisateur
                    saleLine.Product = LensConstruction.GetProductBySaleLine(saleLine, new FatSod.DataContext.Concrete.EFDbContext());
                    if (saleLine.Product == null)
                    {
                        //ApplyExtraPrices(salelines, reduction, discount, transport, VatRate);
                        Session["salelinesnoninsured"] = salelines;
                        res = true;
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

                Session["salelinesnoninsured"] = salelines;

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
    }
}