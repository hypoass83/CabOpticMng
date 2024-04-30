using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using Ext.Net;
using Ext.Net.MVC;
using FastSod.Utilities.Util;
using FatSod.DataContext.Concrete;
using FatSod.DataContext.Initializer;
using FatSod.Report.WrapReports;
using FatSod.Ressources;
using FatSod.Security.Abstracts;
using FatSod.Security.Entities;
using FatSod.Supply.Abstracts;
using FatSod.Supply.Entities;
using FatSodDental.UI.Controllers;
using FatSodDental.UI.Filters;
using FatSodDental.UI.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using SaleE = FatSod.Supply.Entities.Sale;

namespace FatSodDental.UI.Areas.Sale.Controllers
{
    [Authorize(Order = 1)]
    [TakeBusinessDay(Order = 2)]
    public class MenuValideProformaController : BaseController
    {
        //Current Controller and current page
        private const string CONTROLLER_NAME = "Sale/MenuValideProforma";
        private const string VIEW_NAME = "Index";
             
        private IBusinessDay _busDayRepo;
        private IDeposit _depositRepository;
        private ISavingAccount _savingAccountRepository;
        private ICustomerOrder _customerOrderRepository;
       
        private List<BusinessDay> lstBusDay;


        private ITransactNumber _transactNumbeRepository;

        private BusinessDay businessDay;
        
        public MenuValideProformaController(
            IBusinessDay busDayRepo, 
            ISavingAccount saRepo,
            IDeposit depositRepository,
            ICustomerOrder customerOrderRepository,
            ITransactNumber transactNumbeRepository
        )
        {
            this._busDayRepo = busDayRepo;
            this._savingAccountRepository = saRepo;
            this._depositRepository = depositRepository;
            this._transactNumbeRepository = transactNumbeRepository;
            this._customerOrderRepository = customerOrderRepository;
        }
        
        [OutputCache(Duration = 3600)] 
        public ActionResult Index()
        {
            try
            {
                
           
            List<BusinessDay> UserBusDays = (List<BusinessDay>)Session["UserBusDays"];
            Session["businessDay"] = UserBusDays.FirstOrDefault();
            DateTime currentDateOp = UserBusDays.FirstOrDefault().BDDateOperation;
            ViewBag.CurrentBranch = UserBusDays.FirstOrDefault().BranchID;
            ViewBag.BusnessDayDate = UserBusDays.FirstOrDefault().BDDateOperation;// businessDay.BDDateOperation;
            Session["BusnessDayDate"] =currentDateOp;

            
            return View(ModelReturnAbleCommands(ViewBag.CurrentBranch,currentDateOp));
            }

            catch (Exception e)
            {
                X.Msg.Alert("Error ", e.Message).Show();
                return this.Direct();
            }
        }

        //[HttpPost]
        public ActionResult GenerateBill(int Assureur)
        {
            int numerofacture = 0;
            Assureur assure = db.Assureurs.Find(Assureur);
            if (assure!=null)
            {
                numerofacture = assure.CompteurFacture;
            }
            string Numerofacture =_customerOrderRepository.generateBill(numerofacture);

            this.GetCmp<TextField>("NumeroFacture").Value = Numerofacture;
            return this.Direct();
        }
        //retourne les ventes non delivre
        private List<object> ModelReturnAbleCommands(int BranchID,DateTime SoldDate)
        {

            double Advanced = 0d;
            double Remainder = 0d;
            double SaleTotalPrice = 0d;
            List<object> model = new List<object>();
           
             
            var allSCommands = db.CustomerOrders
                    .Where(sa => sa.BranchID == BranchID && sa.CustomerOrderDate == SoldDate.Date && !sa.IsDelivered)
                    .ToList()
                    .Select(s => new CustomerOrder
                    {
                        CustomerOrderID = s.CustomerOrderID
                    }).AsQueryable();
            
            //il faut mainteant vérifier si la vente à encore au moins une ligne de vente pouvant faire l'objet d'un retour
            foreach (CustomerOrder s in allSCommands)
            {
                CustomerOrder command = db.CustomerOrders.Find(s.CustomerOrderID);
                if (command.CustomerOrderLines == null || command.CustomerOrderLines.Count <= 0) { continue; }
                SaleTotalPrice = Util.ExtraPrices(command.CustomerOrderLines.Select(sl => sl.LineUnitPrice).Sum(), command.RateReduction, command.RateDiscount, command.Transport, command.VatRate).TotalTTC;
                
                Remainder = SaleTotalPrice - Advanced;
                model.Add(
                            new
                            {
                                CustomerOrderID = command.CustomerOrderID,
                                CustomerOrderDate = command.CustomerOrderDate,
                                CustomerFullName = command.CustomerName,
                                CustomerOrderNumber = command.CustomerOrderNumber,
                                SaleTotalPrice = SaleTotalPrice,
                                Remainder=Remainder,
                                Remarque = command.Remarque,
                                MedecinTraitant = command.MedecinTraitant
                            }
                          );
            }


            return model;
        }

        private List<object> ModelReturnAbleCommands(int BranchID, String SearchOption, String SearchValue)
        {
            double Advanced = 0d;
            double Remainder = 0d;
            double SaleTotalPrice = 0d;
            List<CustomerOrder> dataTmp = new List<CustomerOrder>();

            if (SearchValue == null || SearchValue == "" || SearchOption == null || SearchOption == "")
            {
                dataTmp = (from custOrd in db.CustomerOrders
                           where !custOrd.IsDelivered
                           select custOrd)
                               .OrderByDescending(a => a.CustomerName)
                                         .Take(1000)
                                         .ToList();
            }
            else
            {
                if (SearchOption == "NAME") //si recherche par NAME
                {
                    dataTmp = (from custOrd in db.CustomerOrders
                               where custOrd.CustomerName.Contains(SearchValue) && !custOrd.IsDelivered
                               select custOrd)
                                   .OrderByDescending(a => a.CustomerName)
                                             .Take(1000)
                                             .ToList();
                }

                if (SearchOption == "NUMBER") //si recherche par ref number
                {
                    dataTmp = (from custOrd in db.CustomerOrders
                               where custOrd.CustomerOrderNumber.Contains(SearchValue) && !custOrd.IsDelivered
                               select custOrd)
                                   .OrderByDescending(a => a.CustomerOrderNumber)
                                             .Take(1000)
                                             .ToList();
                }


            }


            List<object> realDataTmp = new List<object>();

            foreach (var command in dataTmp)
            {
                SaleTotalPrice = Util.ExtraPrices(command.CustomerOrderLines.Select(sl => sl.LineUnitPrice).Sum(), command.RateReduction, command.RateDiscount, command.Transport, command.VatRate).TotalTTC;

                Remainder = SaleTotalPrice - Advanced;
                realDataTmp.Add(
                            new
                            {
                                CustomerOrderID = command.CustomerOrderID,
                                CustomerOrderDate = command.CustomerOrderDate,
                                CustomerFullName = command.CustomerName,
                                CustomerOrderNumber = command.CustomerOrderNumber,
                                SaleTotalPrice = SaleTotalPrice,
                                Remainder = Remainder,
                                Remarque = command.Remarque,
                                MedecinTraitant = command.MedecinTraitant
                            }
                          );
                
            }
            return realDataTmp;
        }

        [HttpPost]
        public ActionResult Delete(int CustomerOrderID)
        {
            try
            {
                db.CustomerOrderLines.Where(ol => ol.CustomerOrderID == CustomerOrderID).ToList().ForEach(ol =>
                {
                    db.CustomerOrderLines.Remove(ol);
                    db.SaveChanges();
                });
                //_customerOrderRepository.Delete(ID);
                db.CustomerOrders.Remove(db.CustomerOrders.Find(CustomerOrderID));
                db.SaveChanges();
                this.AlertSucces(Resources.Success, "Proforma has been deleted");
                return this.ResetDepositForm();
                return this.Direct();
            }
            catch (Exception e)
            {
                X.Msg.Alert("Error", e.Message).Show();
                return this.Direct();
            }
        }

        public ActionResult loadGrid()
        {
            this.GetCmp<Store>("SalesListStore").Reload();
            return this.Direct();
        }

        [HttpPost]
        public ActionResult InitializeFields(int CustomerOrderID)
        {
            try
            {
            double Advanced = 0d;
            double Remainder = 0d;
            double SaleTotalPrice = 0d;
            if (CustomerOrderID==null || CustomerOrderID<=0)
            {
                X.Msg.Alert("Select", "Error while select customer Order").Show();
                return this.Direct();
            }
            //we take sale and her salelines
            CustomerOrder customerOrder = db.CustomerOrders.Find(CustomerOrderID);
            
            if (customerOrder.CustomerOrderLines == null || customerOrder.CustomerOrderLines.Count <= 0)
            { SaleTotalPrice = 0; }
            else { SaleTotalPrice = Util.ExtraPrices(customerOrder.CustomerOrderLines.Select(sl => sl.LineUnitPrice).Sum(), customerOrder.RateReduction, customerOrder.RateDiscount, customerOrder.Transport, customerOrder.VatRate).TotalTTC; }
            //Customer customerSelect = db.Customers.Find(customerOrder.AssureurID);

            Remainder = SaleTotalPrice - Advanced;

            this.GetCmp<TextField>("CustomerOrderID").Value = customerOrder.CustomerOrderID;
            this.GetCmp<ComboBox>("BranchID").Value = customerOrder.BranchID;
            this.GetCmp<TextField>("CustomerName").Value = customerOrder.CustomerName;
            this.GetCmp<TextField>("CompanyName").Value = customerOrder.CompanyName;
            this.GetCmp<ComboBox>("Assureur").Value = customerOrder.AssureurID;
            this.GetCmp<TextField>("PoliceAssurance").Value = customerOrder.PoliceAssurance;
            this.GetCmp<TextField>("CompteurFacture").Value = customerOrder.CompteurFacture;
            this.GetCmp<TextField>("PhoneNumber").Value = customerOrder.PhoneNumber;
            this.GetCmp<TextField>("Remarque").Value = customerOrder.Remarque;
            this.GetCmp<TextField>("MedecinTraitant").Value = customerOrder.MedecinTraitant;
            this.GetCmp<TextField>("NumeroBonPriseEnCharge").Value = customerOrder.NumeroBonPriseEnCharge;
            this.GetCmp<TextField>("CustomerOrderNumber").Value = customerOrder.CustomerOrderNumber;
            

            this.GetCmp<Radio>("NoDetail").Value = customerOrder.DatailBill;
            this.GetCmp<Radio>("IsDetail").Value = customerOrder.DatailBill;
            if (customerOrder.DatailBill==0)
            {
                this.GetCmp<NumberField>("VerreAssurance").Hidden=true; 
                this.GetCmp<NumberField>("MontureAssurance").Hidden=true;
                this.GetCmp<Label>("lbLens").Hidden=true;
                this.GetCmp<Label>("lbMonture").Hidden = true;
                this.GetCmp<Label>("lbLensP").Hidden = true;
                this.GetCmp<Label>("lbMontureP").Hidden = true;
                this.GetCmp<NumberField>("Plafond").ReadOnly=false; 
                this.GetCmp<NumberField>("VerreMalade").Hidden=true;
                this.GetCmp<NumberField>("MontureMalade").Hidden=true; 
            }
            else
            {
                this.GetCmp<NumberField>("VerreAssurance").Hidden = false;
                this.GetCmp<NumberField>("MontureAssurance").Hidden = false;
                this.GetCmp<Label>("lbLens").Hidden = false;
                this.GetCmp<Label>("lbMonture").Hidden = false;
                this.GetCmp<Label>("lbLensP").Hidden = false;
                this.GetCmp<Label>("lbMontureP").Hidden = false;
                this.GetCmp<NumberField>("Plafond").ReadOnly = true;
                this.GetCmp<NumberField>("VerreMalade").Hidden = false;
                this.GetCmp<NumberField>("MontureMalade").Hidden = false; 
            }

            double LensPrice = Util.ExtraPrices(customerOrder.CustomerOrderLines.Where(l => l.Product.Category.CategoryID != 1 && l.Product.Category.CategoryID != 2).Select(sl => sl.LineUnitPrice).Sum(), customerOrder.RateReduction, customerOrder.RateDiscount, customerOrder.Transport, customerOrder.VatRate).TotalTTC;
            double MonturePrice = Util.ExtraPrices(customerOrder.CustomerOrderLines.Where(l => l.Product.Category.CategoryID == 1).Select(sl => sl.LineUnitPrice).Sum(), customerOrder.RateReduction, customerOrder.RateDiscount, customerOrder.Transport, customerOrder.VatRate).TotalTTC;
            //double OtherPrice = Util.ExtraPrices(customerOrder.CustomerOrderLines.Where(l => l.Product.Category.CategoryID == 2).Select(sl => sl.LineAmount).Sum(), customerOrder.RateReduction, customerOrder.RateDiscount, customerOrder.Transport, customerOrder.VatRate).TotalTTC;

            this.GetCmp<NumberField>("Debt").Value = Remainder;
            this.GetCmp<TextField>("DeviseID").Value = customerOrder.DeviseID;
            this.GetCmp<NumberField>("Verres").Value = LensPrice;
            this.GetCmp<NumberField>("Monture").Value = MonturePrice;

            double partAssuVerre = (customerOrder.VerreAssurance == null) ? 0 : customerOrder.VerreAssurance;
            double partAssMonture= (customerOrder.MontureAssurance == null) ? 0 : customerOrder.MontureAssurance ;
            double plafond = (customerOrder.Plafond == null) ? 0 : customerOrder.Plafond; //; partAssMonture + partAssuVerre;

            this.GetCmp<NumberField>("VerreAssurance").Value = partAssuVerre;
            this.GetCmp<NumberField>("MontureAssurance").Value = partAssMonture;
            this.GetCmp<NumberField>("Plafond").Value = plafond;
                
            

            //double txtpriseencharge = (customerOrder.RateReduction == null || customerOrder.RateReduction <= 0) ? 100 : customerOrder.RateReduction;
            //this.GetCmp<NumberField>("TauxPrisenCharge").Value = txtpriseencharge;
            //this.ApplyInsurancePrices(LensPrice, MonturePrice, OtherPrice, txtpriseencharge);

            //Branch curBranch = db.Branches.Find(customerOrder.BranchID);
            //businessDay = _busDayRepo.GetOpenedBusinessDay(curBranch);
            //this.GetCmp<TextField>("DepositReference").Value = _transactNumbeRepository.displayTransactNumber("REDP", businessDay);
            
            if (customerOrder.BillState==StatutFacture.Validated)
            {
                this.GetCmp<Button>("btnPrintFacture").Disabled = false;
                this.GetCmp<Button>("btnPrintBordDep").Disabled = false;
            }
            else
            {
                this.GetCmp<Button>("btnPrintFacture").Disabled = true;
                this.GetCmp<Button>("btnPrintBordDep").Disabled = true;
            }
            Session["Receipt_CustomerOrderID"] = CustomerOrderID;
            

            return this.Direct();
            }
            catch (Exception e) { X.Msg.Alert("Error ", e.Message).Show(); return this.Direct(); }
        }

        public void ApplyInsurancePrices(double Verres, double Monture, double Autre,double lensInsPartprice,double frameInsPartprice,double otherInsPartPrice)
        {
            double  valueLens = Verres;
            double valueFrame = Monture;
            double valueOther = Autre;

        
            double lensCustPartprice = 0;
            double frameCustPartprice = 0;
            double otherCustPartPrice = 0;

            //distribution des parts           
        
            lensCustPartprice=valueLens-lensInsPartprice;
            frameCustPartprice=valueFrame-frameInsPartprice;
            otherCustPartPrice=valueOther-otherInsPartPrice;     
        
            this.GetCmp<NumberField>("VerreAssurance").Value = lensInsPartprice;
            this.GetCmp<NumberField>("MontureAssurance").Value = frameInsPartprice;
            this.GetCmp<NumberField>("AutreAssurance").Value =otherInsPartPrice;
            this.GetCmp<NumberField>("VerreMalade").Value = lensCustPartprice;
            this.GetCmp<NumberField>("MontureMalade").Value = frameCustPartprice;
            this.GetCmp<NumberField>("AutreMalade").Value = otherCustPartPrice;

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

        public ActionResult ChangeBusDay(int? BranchID)
        {
            if (BranchID != null && BranchID.HasValue && BranchID.Value > 0)
            {
                lstBusDay = (List<BusinessDay>)Session["UserBusDays"];
                if (lstBusDay == null)
                {
                    lstBusDay = _busDayRepo.GetOpenedBusinessDay(CurrentUser);
                };

                BusinessDay businessD = lstBusDay.FirstOrDefault(bd => bd.BranchID == BranchID && bd.BDStatut);
                this.GetCmp<DateField>("DepositDate").Value = businessD.BDDateOperation;
            }
            return this.Direct();
        }

        public ActionResult ValidateProforma(CustomerOrder currentOrder)
        {
            try
            {
                currentOrder.VatRate = CodeValue.Accounting.ParamInitAcct.VATRATE;
                currentOrder.IsDelivered = false;
                currentOrder.BillState = StatutFacture.Validated;
                
                //Nom de l'employé de DBOY qui à saisie la commande
                //currentOrder.Operator = this.CurrentUser;
                currentOrder.OperatorID = this.SessionGlobalPersonID;

                if (currentOrder.CustomerOrderID > 0)
                {
                    List<CustomerOrderLine> newsCustomerOrderLine = db.CustomerOrderLines.Where(col => col.CustomerOrderID == currentOrder.CustomerOrderID).ToList();
                    _customerOrderRepository.UpdateCustomerOrder(currentOrder, newsCustomerOrderLine, SessionGlobalPersonID);
                    statusOperation = Resources.CmdUpdated;
                }
                else
                {
                    X.Msg.Alert("Error ", "Select Customer before validate").Show();
                    return this.Direct();
                }
                this.AlertSucces(Resources.Success, statusOperation);
                this.ResetDepositForm();
                
                return this.Direct();
            }
            catch (Exception e) { X.Msg.Alert("Error ", e.Message + " " + e.StackTrace + " " + e.InnerException).Show(); return this.Direct(); }
        


        }
        
        public ActionResult Reset()
        {
            this.ReloadSalesListStore();
            this.GetCmp<FormPanel>("DepositForm").Reset();
            return this.Direct();
        }
        [HttpPost]
        public ActionResult ResetDepositForm()
        {
          
            this.ReloadSalesListStore();

            //this.GetCmp<NumberField>("Amount").Reset();
            //this.GetCmp<Button>("btnPrint").Disabled = false;
            this.GetCmp<Button>("btnSave").Disabled = true;
            this.GetCmp<Button>("btnPrintFacture").Disabled = false;
            this.GetCmp<Button>("btnPrintBordDep").Disabled = false;
            //this.GetCmp<TextField>("DepositReference").Value = "";
            //this.GetCmp<TextField>("DepositReference").Value = "";
            return this.Direct();
        }
        public ActionResult ReloadSalesListStore()
        {
            this.GetCmp<Store>("SalesListStore").Reload();
            return this.Direct();
        }
        public StoreResult PendingCustomerSale(int BranchID, DateTime SoldDate, String SearchOption, String SearchValue)
        {
            if (SearchValue.Trim().Length == 0)
            {
                return this.Store(this.ModelReturnAbleCommands(BranchID, SoldDate));
            }
            else
            {
                return this.Store(this.ModelReturnAbleCommands(BranchID, SearchOption, SearchValue));
            }
        }
       

        //This method load a method that print a receip of deposit
        public ActionResult PrintFacture(double VerreAssurance, double MontureAssurance)
        {
            //Session["TxtPrisEnChargeAssureur"] = TauxPrisenCharge;
            Session["VerreAssureur"] = VerreAssurance;
            Session["MontureAssurance"] = MontureAssurance;
            this.GetCmp<Panel>("Pdf").LoadContent(new ComponentLoader
            {
                Url = Url.Action("GenerateFacture"),
                DisableCaching = false,
                Mode = LoadMode.Frame,
            });

            return this.Direct();
        }
        public ActionResult PrintBorderoDepotLunette()
        {
            this.GetCmp<Panel>("Pdf").LoadContent(new ComponentLoader
            {
                Url = Url.Action("GenerateBorderoDepotLunette"),
                DisableCaching = false,
                Mode = LoadMode.Frame,
            });

            return this.Direct();
        }
        public void GenerateBorderoDepotLunette()
        {
            List<object> model = new List<object>();
            List<object> modelsubRpt = new List<object>();
            ReportDocument rptH = new ReportDocument();
            try
            {
                int customerOrderID = (Session["Receipt_CustomerOrderID"] == null) ? 0 : (int)Session["Receipt_CustomerOrderID"];

                string repName = "";
                bool isValid = false;
              

                string path = "";
                string defaultImgPath = Server.MapPath("~/Content/Images/App/deboy-logo.png");
                byte[] COMPANY_LOGO = System.IO.File.ReadAllBytes(defaultImgPath);

                string TitleDeposit = "";
                string RptTitle = "";

                Company cmpny = db.Companies.FirstOrDefault();
                if (customerOrderID > 0)//depot pour une vente
                {

                    CustomerOrder currentOrder = db.CustomerOrders.Find(customerOrderID);

                    string Prescription = "";
                    //recuperation des versements
                    List<CustomerOrderLine> lstOrderLine = db.CustomerOrderLines.Where(sl => sl.CustomerOrderID == currentOrder.CustomerOrderID).ToList();
                   
                    foreach (CustomerOrderLine custOrderLine in lstOrderLine)
                    {
                        string labelFrame = (custOrderLine.marque != null && custOrderLine.reference != null) ? Resources.Marque.ToUpper() + " " + custOrderLine.marque + " " + Resources.Reference.ToUpper() + " " + custOrderLine.reference : "";
                        int i = (custOrderLine.marque != null && custOrderLine.reference != null) ? 2 : 1;
                        if (labelFrame.Trim().Length > 0)
                        {
                            Prescription = labelFrame;
                        }
                        else
                        {
                            if (custOrderLine.Product.Prescription != null)
                            {
                                Prescription = custOrderLine.Product.Prescription;
                            }
                            else
                            {
                                Prescription = (custOrderLine.Product.ProductCode.Contains(" HD ")) ? custOrderLine.Product.ProductCode.Replace(custOrderLine.Product.Category.CategoryCode + " HD", "") : custOrderLine.Product.ProductCode.Replace(custOrderLine.Product.Category.CategoryCode, "");
                            }
                        }
                        model.Add(
                        new //RptReceipt
                        {
                            RptReceiptID = 1,
                            ReceiveAmount = (labelFrame.Trim().Length > 0) ? currentOrder.MontureAssurance : Math.Round(currentOrder.VerreAssurance / 2), //montant du verre TotalReceiveAmount, //part assureur
                            TotalAmount = 0, //montant restant de la facture
                            LineUnitPrice = (labelFrame.Trim().Length > 0) ? currentOrder.MontureAssurance : Math.Round(currentOrder.VerreAssurance / 2), //montant du verre
                            CompanyName = cmpny.Adress.AdressFullName,
                            CompanyAdress = "B.P. " + cmpny.Adress.AdressPOBox + ", " + cmpny.Adress.Quarter.Town.TownLabel + ", Email:" + cmpny.Adress.AdressEmail,// "Head Quater : " + cmpny.Adress.Quarter.Town.Region.RegionLabel + " - " + cmpny.Adress.Quarter.Town.TownLabel,
                            CompanyTel = "Tel: " + cmpny.Adress.AdressPhoneNumber + ", Fax: " + cmpny.Adress.AdressFax + ", Cell: " + cmpny.Adress.AdressCellNumber,
                            CustomerAdress = currentOrder.NumeroBonPriseEnCharge,// "No.ONOC:87 / ONOC",
                            BranchName = currentOrder.Branch.BranchName,
                            BranchAdress = currentOrder.Branch.Adress.Quarter.QuarterLabel + " - " + currentOrder.Branch.Adress.Quarter.Town.TownLabel,
                            BranchTel = "Tel: " + currentOrder.Branch.Adress.AdressPhoneNumber,
                            Reference = currentOrder.NumeroFacture,
                            CompanyCNI = "NO CONT : " + cmpny.CNI,
                            Operator = CurrentUser.Name + " " + CurrentUser.Description,
                            CustomerName = currentOrder.CustomerName,// customerRpt.Name + " " + customerRpt.Description,
                            ProductLabel = Prescription, //(labelFrame.Trim().Length > 0) ? labelFrame : custOrderLine.Product.Prescription,
                            SaleDate = currentOrder.CustomerOrderDate,
                            Title = cmpny.Adress.Quarter.Town.TownLabel,
                            MontantLettre = "",//MONTANT FACTURE,
                            RateTVA = currentOrder.VatRate,
                            RateReduction = currentOrder.RateReduction,
                            RateDiscount = currentOrder.RateDiscount,
                            Transport = currentOrder.Transport,
                            RptReceiptPaymentDetailID = i,
                            LineQuantity = (labelFrame.Trim().Length > 0) ? custOrderLine.LineQuantity : custOrderLine.LineQuantity * 2,
                            CustomerAccount = currentOrder.Assureur.Name, //nom de la societe assureur
                            BranchAbbreviation = currentOrder.CompanyName, //NOM DE LA SOCIETE DU CLIENT
                            DeviseLabel = currentOrder.PoliceAssurance,//police d'assurance
                            ProductRef = (labelFrame.Trim().Length > 0) ? "MONTURE MATIERE :" + custOrderLine.FrameCategory : custOrderLine.Product.Category.CategoryDescription,
                            CompanyLogo = db.Files.FirstOrDefault(f => f.GlobalPersonID == cmpny.GlobalPersonID) != null ? db.Files.FirstOrDefault(f => f.GlobalPersonID == cmpny.GlobalPersonID).Content : COMPANY_LOGO
                        }
                    );
                        //
                    }

                    path = Server.MapPath("~/Reports/Sales/RptBorderoDepotLunette.rpt");
                    repName = "RptBorderoDepotLunette";
                    
                    isValid = true;
                }


                if (isValid)
                {
                    rptH.Load(path);
                    rptH.SetDataSource(model);

                    bool isDownloadRpt = (bool)Session["isDownloadRpt"];
                    rptH.ExportToHttpResponse(ExportFormatType.PortableDocFormat, System.Web.HttpContext.Current.Response, isDownloadRpt, repName);
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
        //This method print a receipt of customer
        public void GenerateFacture()
        {
            List<object> model = new List<object>();
            List<object> modelsubRpt = new List<object>();
            ReportDocument rptH = new ReportDocument();
            try
            {
            int customerOrderID = (Session["Receipt_CustomerOrderID"] == null) ? 0 : (int)Session["Receipt_CustomerOrderID"];
           
            string repName = "";
            bool isValid = false;
            double totalAmount = 0d;
            double totalRemaining = 0d;

           
            string path = "";
            string defaultImgPath = Server.MapPath("~/Content/Images/App/deboy-logo.png");
            byte[] COMPANY_LOGO = System.IO.File.ReadAllBytes(defaultImgPath);

            string TitleDeposit = "";
            string RptTitle = "";

            Company cmpny = db.Companies.FirstOrDefault();
            if (customerOrderID > 0)//depot pour une vente
            {

                double saleAmount = 0d;
                CustomerOrder currentOrder = db.CustomerOrders.Find(customerOrderID);

                string Prescription = "";
                //recuperation des versements
                List<CustomerOrderLine> lstOrderLine = db.CustomerOrderLines.Where(sl => sl.CustomerOrderID == currentOrder.CustomerOrderID).ToList();
                //totalAmount = (lstOrderLine.Count > 0) ? Util.ExtraPrices(lstOrderLine.Select(c => c.LineUnitPrice).Sum(), currentOrder.RateReduction, currentOrder.RateDiscount, currentOrder.Transport, currentOrder.VatRate).TotalTTC : 0; //montant du verre
                //totalRemaining = totalAmount - TotalReceiveAmount;
                totalAmount = currentOrder.Plafond; // currentOrder.MontureAssurance + currentOrder.VerreAssurance;
                string montantLettre = NumberConverter.Spell((ulong)totalAmount).ToUpper();
                foreach (CustomerOrderLine custOrderLine in lstOrderLine)
                {
                    string labelFrame = (custOrderLine.marque != null && custOrderLine.reference != null) ? Resources.Marque.ToUpper() + " " + custOrderLine.marque + " " + Resources.Reference.ToUpper() + " " + custOrderLine.reference : "";
                    int i = (custOrderLine.marque != null && custOrderLine.reference != null) ? 2 : 1;
                    if (labelFrame.Trim().Length > 0)
                    {
                        Prescription = labelFrame;
                    }
                    else
                    {
                        if (custOrderLine.Product.Prescription!=null)
                        {
                            Prescription = custOrderLine.Product.Prescription;
                        }
                        else
                        {
                            Prescription = (custOrderLine.Product.ProductCode.Contains(" HD ")) ? custOrderLine.Product.ProductCode.Replace(custOrderLine.Product.Category.CategoryCode + " HD", "") : custOrderLine.Product.ProductCode.Replace(custOrderLine.Product.Category.CategoryCode, "");
                        }
                    }
                    model.Add(
                    new //RptReceipt
                    {
                        RptReceiptID = 1,
                        ReceiveAmount = (labelFrame.Trim().Length > 0) ? currentOrder.MontureAssurance : Math.Round(currentOrder.VerreAssurance / 2), //montant du verre TotalReceiveAmount, //part assureur
                        TotalAmount = totalAmount, //montant restant de la facture
                        LineUnitPrice = (labelFrame.Trim().Length > 0) ? currentOrder.MontureAssurance : Math.Round(currentOrder.VerreAssurance / 2), //montant du verre
                        CompanyName = cmpny.Adress.AdressFullName,
                        CompanyAdress = "B.P. " + cmpny.Adress.AdressPOBox + ", " + cmpny.Adress.Quarter.Town.TownLabel + ", Email:" + cmpny.Adress.AdressEmail,// "Head Quater : " + cmpny.Adress.Quarter.Town.Region.RegionLabel + " - " + cmpny.Adress.Quarter.Town.TownLabel,
                        CompanyTel = "Tel: " + cmpny.Adress.AdressPhoneNumber + ", Fax: " + cmpny.Adress.AdressFax + ", Cell: " + cmpny.Adress.AdressCellNumber,
                        CustomerAdress = currentOrder.NumeroBonPriseEnCharge,// "No.ONOC:87 / ONOC",
                        BranchName = currentOrder.Branch.BranchName,
                        BranchAdress = currentOrder.Branch.Adress.Quarter.QuarterLabel + " - " + currentOrder.Branch.Adress.Quarter.Town.TownLabel,
                        BranchTel = "Tel: " + currentOrder.Branch.Adress.AdressPhoneNumber,
                        Reference = currentOrder.NumeroFacture,
                        CompanyCNI = "NO CONT : " + cmpny.CNI,
                        Operator = CurrentUser.Name + " " + CurrentUser.Description,
                        CustomerName = currentOrder.CustomerName,// customerRpt.Name + " " + customerRpt.Description,
                        ProductLabel = Prescription, //(labelFrame.Trim().Length > 0) ? labelFrame : custOrderLine.Product.Prescription,
                        SaleDate = currentOrder.CustomerOrderDate,
                        Title = cmpny.Adress.Quarter.Town.TownLabel,
                        MontantLettre = montantLettre,//MONTANT FACTURE,
                        RateTVA = currentOrder.VatRate,
                        RateReduction = currentOrder.RateReduction,
                        RateDiscount = currentOrder.RateDiscount,
                        Transport = currentOrder.Transport,
                        RptReceiptPaymentDetailID = i,
                        LineQuantity = (labelFrame.Trim().Length > 0) ? custOrderLine.LineQuantity : custOrderLine.LineQuantity * 2,
                        CustomerAccount = currentOrder.Assureur.Name, //nom de la societe assureur
                        BranchAbbreviation = currentOrder.CompanyName, //NOM DE LA SOCIETE DU CLIENT
                        DeviseLabel = currentOrder.PoliceAssurance,//police d'assurance
                        ProductRef = (labelFrame.Trim().Length > 0) ? "MONTURE MATIERE :" + custOrderLine.FrameCategory : custOrderLine.Product.Category.CategoryDescription,
                        CompanyLogo = db.Files.FirstOrDefault(f => f.GlobalPersonID == cmpny.GlobalPersonID) != null ? db.Files.FirstOrDefault(f => f.GlobalPersonID == cmpny.GlobalPersonID).Content : COMPANY_LOGO
                    }
                );
                    //
                }

                if (currentOrder.DatailBill==0)
                {
                    path = Server.MapPath("~/Reports/CashRegister/RptFactureAssurance.rpt");
                    repName = "RptFactureAssurance";
                }
                else
                {
                    path = Server.MapPath("~/Reports/CashRegister/RptFactureAssuranceDetail.rpt");
                    repName = "RptFactureAssuranceDetail";
                }
                

                isValid = true;
            }


            if (isValid)
            {
                FatSod.Ressources.Resources.Culture = System.Globalization.CultureInfo.CreateSpecificCulture("fr");
                rptH.Load(path);
                rptH.SetDataSource(model);
               
                //rptH.SetParameterValue("RptTitle", RptTitle);
                bool isDownloadRpt = (bool)Session["isDownloadRpt"];
                rptH.ExportToHttpResponse(ExportFormatType.PortableDocFormat, System.Web.HttpContext.Current.Response, isDownloadRpt, repName);
                //return File(stream, "application/pdf");
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