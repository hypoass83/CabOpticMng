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
using System.IO;
using CrystalDecisions.CrystalReports.Engine;
using FatSodDental.UI.Filters;
using FastSod.Utilities.Util;
using CrystalDecisions.Shared;
using FatSod.Budget.Entities;
using FatSod.DataContext.Concrete;


//Pour l'impression
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.Windows.Forms;
using System.Data;
using System.Drawing.Printing;
using System.Management;
using System.Printing;
using System.Security.Principal;


using System.IO;
using System.Drawing;
using System.Drawing.Printing;
using System.Web.UI;

namespace FatSodDental.UI.Areas.CashRegister.Controllers
{
	[Authorize(Order = 1)]
	[TakeBusinessDay(Order = 2)]
	public class StateController : BaseController
	{
		private const string CONTROLLER_NAME = "CashRegister/State";
		//*********************
		private IPerson _personRepository;
        //private IRepositorySupply<CustomerOrderLine> _customerOrderLineRepository;
        //private IRepositorySupply<CustomerOrder> _customerOrderRepository;
		private IRepositorySupply<Product> _productRepository;
		private IRepositorySupply<Devise> _deviseRepository;
		private IRepositorySupply<ProductLocalization> _productLocalizationRepository;
		private ITillDay _tillDayRepository;
        private IRepository<UserBranch> _userBranch;
		private IRepository<Branch> _branchRepository;
		private IRepository<Company> _companyRepository;
		private IRepositorySupply<PaymentMethod> _paymentMethodRepository;
		private IRepository<BudgetConsumption> _bdgetCumptnRepository;
		private IRepositorySupply<UserTill> _userTillRepository;
		private IBusinessDay _busDayRepo;
		private IRepositorySupply<Till> _tillRepository;
		private IRepositorySupply<Purchase> _purchaseRepository;
		private ISale _saleRepository;
		private IRepositorySupply<Line> _lineRepository;
		private IRepository<FatSod.Security.Entities.File> _fileRepository;
		private IDeposit _depositRepository;
		private IRepositorySupply<Customer> _customerRepository;
		private IRepositorySupply<Slice> _sliceRepository;

		private ICustomerReturn _customerReturnRepository;
		private ITransactNumber _transactNumberRepository;
		private ITillAdjust _tillAdjust;
        private IBill _billRepository;
        private ISavingAccount _savingAccountRepository;
		// GET: CashRegister/State
		public StateController(
				IRepositorySupply<ProductLocalization> productLocalizationRepository,
				IRepositorySupply<Product> productRepository,
				IRepositorySupply<Devise> deviseRepository,
				IPerson personRepository,
                //IRepositorySupply<CustomerOrderLine> customerOrderLineRepository,
				ITillDay tillDayRepository,
				ISale saleRepository,
                IBill billRepository,
                ISavingAccount SavingAccountRepo,
				IRepositorySupply<Purchase> purchaselRepository,
				IRepositorySupply<UserTill> userTillRepository,
				IRepository<Branch> branchRepository,
                //IRepositorySupply<CustomerOrder> customerOrderRepository,
				IBusinessDay busDayRepo,
				IRepositorySupply<Line> lineRepository,
				IRepository<FatSod.Security.Entities.File> fileRepository,
				IRepositorySupply<Till> tillRepository,
				IDeposit depositRepository,
				IRepository<Company> companyRepository,
				IRepositorySupply<Customer> customerRepository,
				IRepository<BudgetConsumption> bdgetCumptnRepository,
				IRepositorySupply<PaymentMethod> paymentMethodRepository,
				IRepositorySupply<Slice> sliceRepository,
				ICustomerReturn customerReturnRepository,
			ITransactNumber transactNumberRepository,
            IRepository<UserBranch> userBranch,
			ITillAdjust tillAdjust
				)
		{
			this._personRepository = personRepository;
            //this._customerOrderLineRepository = customerOrderLineRepository;
            //this._customerOrderRepository = customerOrderRepository;
			this._productRepository = productRepository;
			this._productLocalizationRepository = productLocalizationRepository;
			this._tillDayRepository = tillDayRepository;
			this._userTillRepository = userTillRepository;
			this._branchRepository = branchRepository;
			this._saleRepository = saleRepository;
			this._busDayRepo = busDayRepo;
			this._tillRepository = tillRepository;
			this._lineRepository = lineRepository;
			this._purchaseRepository = purchaselRepository;
			this._fileRepository = fileRepository;
			this._companyRepository = companyRepository;
			this._depositRepository = depositRepository;
			this._deviseRepository = deviseRepository;
			this._customerRepository = customerRepository;
			this._bdgetCumptnRepository = bdgetCumptnRepository;
			this._paymentMethodRepository = paymentMethodRepository;
			this._sliceRepository = sliceRepository;
			this._customerReturnRepository = customerReturnRepository;
			this._transactNumberRepository = transactNumberRepository;
            this._userBranch = userBranch;
			this._tillAdjust = tillAdjust;
            this._billRepository = billRepository;
            this._savingAccountRepository = SavingAccountRepo;
		}
		//Enable to get hitoric of cash register
        [OutputCache(Duration = 3600)] 
		public ActionResult Index()
		{
            try
            {

            //Ext.Net.MVC.PartialViewResult rPVResult = new Ext.Net.MVC.PartialViewResult
            //{
            //    ClearContainer = true,
            //    //RenderMode = RenderMode.AddTo,
            //    ContainerId = "Body",
            //    WrapByScriptTag = false
            //};
            //We test if this user has an autorization to get this sub menu
            //if (!LoadAction.isAutoriseToGetSubMenu(SessionProfileID, CodeValue.CashRegister.SUBMENU_STATE_CASH_HISOTRIC, db))
            //{
            //    RedirectToAction("NotAuthorized", "User");
            //}
            ViewBag.Disabled = true;

            List<BusinessDay> listBDUser =(List<BusinessDay>)Session["UserBusDays"];
            if (listBDUser.Count() > 1)
            {
                ViewBag.Disabled = false;
            }
            DateTime busDays = listBDUser.FirstOrDefault().BDDateOperation;
            ViewBag.BusnessDayDate = busDays;
            Session["BusnessDayDate"] = busDays;

            return View();
            }

            catch (Exception e)
            {
                X.Msg.Alert("Error ", e.Message).Show();
                return this.Direct();
            }
		}
        [OutputCache(Duration = 3600)]
        public ActionResult DisplayFacture()
        {
            try
            {

                ViewBag.Disabled = true;

                List<BusinessDay> listBDUser = (List<BusinessDay>)Session["UserBusDays"];
                if (listBDUser.Count() > 1)
                {
                    ViewBag.Disabled = false;
                }
                DateTime busDays = listBDUser.FirstOrDefault().BDDateOperation;
                ViewBag.BusnessDayDate = busDays;
                Session["BusnessDayDate"] = busDays;

                return View(ModelReturnBill(busDays));
            }

            catch (Exception e)
            {
                X.Msg.Alert("Error ", e.Message).Show();
                return this.Direct();
            }
        }
        /// <summary>
        /// Liste des factures negoce
        /// </summary>
        private List<object> ModelReturnBill(DateTime BillDate)
        {
            List<object> model = new List<object>();
            BusinessDay currentBD = _busDayRepo.GetOpenedBusinessDay(CurrentUser).FirstOrDefault();

            foreach (Bill s in db.Bills.Where(b => b.BillDate == BillDate.Date).ToList())
            {

                model.Add(
                            new
                            {
                                BillID = s.BillID,
                                BillDate = s.BillDate,
                                BeginDate = s.BeginDate,
                                EndDate = s.EndDate,
                                CustomerFullName = s.Customer.Name,
                                BillNumber = s.BillNumber
                            }
                        );
            }
            return model;
        }
        [HttpPost]
        public DirectResult InitializeFields(int BillID)
        {
            //ResetReturn();

            //we take sale and her salelines
            Bill selectedBill = db.Bills.Find(BillID);

            this.GetCmp<Store>("BillListStore").Reload();
            this.GetCmp<GridPanel>("BillList").Disabled = false;
            this.GetCmp<FormPanel>("GlobalBillForm").Disabled = false;
            this.GetCmp<TextField>("BillID").Value = selectedBill.BillID;
            this.GetCmp<TextField>("CustomerID").Value = selectedBill.CustomerID;
            this.GetCmp<TextField>("CustomerName").Value = selectedBill.Customer.Name;
            this.GetCmp<TextField>("BillNumber").Value = selectedBill.BillNumber;
            this.GetCmp<DateField>("BillDate").Value = selectedBill.BillDate;
            this.GetCmp<DateField>("BeginDate").Value = selectedBill.BeginDate;
            this.GetCmp<DateField>("EndDate").Value = selectedBill.EndDate;
            return this.Direct();
        }
        [HttpPost]
        public StoreResult ReturnAbleBill(DateTime BillDate)
        {
            return this.Store(ModelReturnBill(BillDate));
        }
        
        public ActionResult ReloadBillListStore()
        {
            this.GetCmp<Store>("BillListStore").Reload();
            this.GetCmp<FormPanel>("FormBillIdentification").Reset();
            return this.Direct();
        }
        [HttpPost]
        public ActionResult PrintAfficheBill(DateTime BeginDate, DateTime EndDate, string BillNumber)
        {
            Session["BeginDate"] = BeginDate;
            Session["EndDate"] = EndDate;
            Session["BillNumber"] = BillNumber;
            this.GetCmp<Panel>("Pdf").Hidden = false;
            this.GetCmp<Panel>("Pdf").LoadContent(new ComponentLoader
            {
                Url = Url.Action("AfficheGenerateBillReport"),
                DisableCaching = false,
                Mode = LoadMode.Frame
            });
            return this.Direct();
        }
        public void AfficheGenerateBillReport()
        {
            ReportDocument rptH = new ReportDocument();
            try
            {
                bool isValid = false;

                DateTime BeginDate = (DateTime)Session["BeginDate"];
                DateTime EndDate = (DateTime)Session["EndDate"];
                string BillNumber = (string)Session["BillNumber"];


                List<object> model = new List<object>();

                string defaultImgPath = Server.MapPath("~/Content/Images/App/deboy-logo.png");
                byte[] COMPANY_LOGO = System.IO.File.ReadAllBytes(defaultImgPath);

                model = this.AfficheModelBillCusto(BillNumber);
                if (model.Count > 0)
                {
                    double TotalDepositPeriode = (double)Session["TotalDepositPeriode"];
                    double BalanceBefore = (double)Session["BalanceBefore"] ;

                    string path = Server.MapPath("~/Reports/CashRegister/RptBillDBoy.rpt");
                    rptH.Load(path);
                    rptH.SetDataSource(model);
                    rptH.SetParameterValue("BalanceBefore", BalanceBefore);
                    rptH.SetParameterValue("TotalDeposit", TotalDepositPeriode);
                    rptH.SetParameterValue("BeginDate", BeginDate);
                    rptH.SetParameterValue("EndDate", EndDate);
                    rptH.SetParameterValue("ServerDate", SessionBusinessDay(null).BDDateOperation);

                    bool isDownloadRpt = (bool)Session["isDownloadRpt"];
                    rptH.ExportToHttpResponse(ExportFormatType.PortableDocFormat, System.Web.HttpContext.Current.Response, isDownloadRpt, "RptBillDBoy");
                }
                else
                {
                    Response.Write("Nothing Found; No Report name found");
                }

            }
            catch (Exception ex)
            {
                Response.Write(ex.ToString());
            }
            finally
            {
                rptH.Close();
                rptH.Dispose();
            }
        }
        
        private List<object> AfficheModelBillCusto(string BillNumber)
        {

            double TotalHT = 0d;
            double TotalTTC = 0d;

            List<object> model = new List<object>();
            double TotalDepositPeriode = 0d;
            string defaultImgPath = Server.MapPath("~/Content/Images/App/deboy-logo.png");
            byte[] COMPANY_LOGO = System.IO.File.ReadAllBytes(defaultImgPath);

            Branch currentBranch = SessionBusinessDay(null).Branch;
            Devise currentDevise = db.Devises.FirstOrDefault(d => d.DefaultDevise);

            double MontantRemise = 0d;
            double MontantEscompte = 0d;
            double Transport = 0d;
            double ValeurTva = 0d;
            double BalanceBefore = 0d;
            //recuperation des ttes les ventes de la periode
            List<Bill> billlist = db.Bills.Where(c => c.IsNegoce && c.BillNumber == BillNumber).ToList();
            billlist.ForEach(allBills =>
            {
                BalanceBefore = allBills.BalanceBefore;
                TotalDepositPeriode = allBills.TotalDepot;

                MontantEscompte = allBills.MontantEscompte;
                MontantRemise = allBills.MontantRemise;
                Customer customer = db.Customers.Find(allBills.CustomerID);

                //recuperation des lignes de vente

                List<BillDetail> slLine = allBills.BillDetails.ToList();
                slLine.ForEach(sl =>
                {
                    model.Add(
                          new
                          {
                              Title = Resources.Billreport,
                              Ref = BillNumber,
                              BranchName = currentBranch.BranchName,
                              BranchAdress = currentBranch.Adress.AdressFullName,
                              BranchTel = currentBranch.Adress.AdressPhoneNumber,
                              CompanyName = Company.Name,
                              CompanyAdress = "Head Quater : " + Company.Adress.Quarter.Town.Region.RegionLabel + " - " + Company.Adress.Quarter.Town.TownLabel,
                              CompanyEmail = "Email: " + Company.Adress.AdressEmail,
                              CompanyTradeRegister = Company.CompanyTradeRegister,
                              CompanyTel = " Tel: " + Company.Adress.AdressPhoneNumber + " Fax: " + Company.Adress.AdressFax + " Cell: " + Company.Adress.AdressCellNumber,
                              CompanyCNI = "NO CONT : " + Company.CNI,
                              CompanyLogo = _fileRepository.FindAll.FirstOrDefault(f => f.GlobalPersonID == Company.GlobalPersonID) != null ? _fileRepository.FindAll.FirstOrDefault(f => f.GlobalPersonID == Company.GlobalPersonID).Content : COMPANY_LOGO,
                              SaleDate = allBills.BillDate.Date,
                              Redution = MontantRemise,
                              Discount = MontantEscompte,
                              Transport = Transport,
                              ProductLabel = sl.Product.ProductLabel,
                              ProductRef = sl.Product.ProductCode,
                              LineUnitPrice = sl.LineUnitPrice,
                              LineQuantity = sl.LineQuantity,
                              CustomerName = customer.Name , //customer.Name,//+ " " + customer.Description,
                              CustomerAdress = "B.P: " + customer.AdressPOBox + " " + customer.Adress.Quarter.Town.TownLabel + " Tél:" + customer.AdressPhoneNumber,
                              CustomerAccount = customer.AccountNumber,
                              VatRate = allBills.TauxTva,
                              NumeroCde = sl.NumeroCommande,
                              DateCde = sl.DateCommande,
                              SaleID = sl.SaleID,
                              BillNumber = BillNumber,
                              CompanyTown = currentBranch.Adress.Quarter.Town.TownLabel
                          }
             );

                }
                );

            }
        );
            Session["BalanceBefore"] = BalanceBefore;
            Session["TotalDepositPeriode"] = TotalDepositPeriode;
            return model;
        }
        public ActionResult Delete(int BillID)
        {
            try
            {
                _billRepository.DeleteBill(BillID, SessionGlobalPersonID, SessionBusinessDay(null).BranchID);
                this.GetCmp<Store>("BillListStore").Reload();
                this.GetCmp<FormPanel>("FormBillIdentification").Reset();
                return this.Direct();
            }
            catch (Exception e)
            {
                X.Msg.Alert("Error ", e.Message).Show();
                return this.Direct();
            }
        }
		//Generate a state of cash register operations in time interval
		public ActionResult GenerateState(DateTime BeginDate, DateTime EndDate, int BranchID,int TillID, string GeneratedType)
		{
			Session["BeginDate"] = BeginDate;
			Session["EndDate"] = EndDate;
            Session["TillID"] = TillID;
			Session["BranchID"] = BranchID;
			this.GetCmp<Panel>(GeneratedType).LoadContent(new ComponentLoader
			{
				Url = Url.Action(GeneratedType),
				DisableCaching = false,
				Mode = LoadMode.Frame
				//Params = { new Parameter("Till", tillDay) }
			});
			return this.Direct();
		}
		/// <summary>
		/// Generate list of cash operation in time internal in pdf format
		/// </summary>
		/// <returns>File</returns>
		public ActionResult PdfType()
		{
			List<object> model = new List<object>();
			DateTime To = (DateTime)Session["BeginDate"];
			DateTime At = (DateTime)Session["EndDate"];
			int tillID = (int)Session["TillID"];
			int branchID = (int)Session["BranchID"];

			this.GenerateReportOperationPeriodeOfDay(To, At, tillID);

			return this.Direct();

		}
		/// <summary>
		/// Generate list of cash operation in time internal in Grid format
		/// </summary>
		/// <returns>Grid</returns>
		public ActionResult GridType()
		{
			DateTime To = (DateTime)Session["BeginDate"];
			DateTime At = (DateTime)Session["EndDate"];
			int tillID = (int)Session["TillID"];
			ViewBag.ListSalesDay = ModelSaleDay(To, At, tillID);
			ViewBag.ListPurchasseDay = ModelPurchaseDay(To, At, tillID);
			return View("RenderState");
		}

		//This method load a method that print a receip of sale

		public ActionResult LoadCMP(/*string Component, string action*/)
		{
            //int SaleID = (int)Session["SaleID"];
			this.GetCmp<Panel>("Pdf").LoadContent(new ComponentLoader
			{
                Url = Url.Action("ShowGeneric"),
				DisableCaching = false,
				Mode = LoadMode.Frame,
				//Params = { new Parameter("sale", sale) }
			});
            return this.Direct();
		}
        public ActionResult ShowGeneric()
        {
            return RedirectToAction("GenerateReceipt");
        }
        public ActionResult PrintSaleDeliveryOrder(string exportFormat)
		{
            int SaleID = (int)Session["SaleID"];
            Session["path"] = Server.MapPath("~/Reports/CashRegister/RptDeliveryOrder.rpt");
            Session["rptname"] = "RptDeliveryOrder";
            Session["exportFormat"] = exportFormat;
			this.LoadCMP();
            //this.GenerateReceipt();
			return this.Direct();
		}
		//public ActionResult printDoc()
		//{
		//	string s = "Hello"; // device-dependent string, need a FormFeed?

		//	// Allow the user to select a printer.
		//	PrintDialog pd = new PrintDialog();
		//	pd.PrinterSettings = new PrinterSettings();
		//	if (DialogResult.OK == pd.ShowDialog(this))
		//	{
		//		// Send a printer-specific to the printer.
		//		RawPrinterHelper.SendStringToPrinter(pd.PrinterSettings.PrinterName, s);
		//	}
		//	return this.Direct();
		//}
		public ActionResult PrintSaleBill()
		{
			this.GetCmp<Panel>("Pdf").LoadContent(new ComponentLoader
			{
				Url = Url.Action("GenerateSaleBill"),
				DisableCaching = false,
				Mode = LoadMode.Frame,
				//Params = { new Parameter("sale", sale) }
			});
			return this.Direct();
		}

        public ActionResult PrintSaleReceipt(string exportFormat)
		{
            int SaleID = (int)Session["SaleID"];
            Session["path"] = Server.MapPath("~/Reports/CashRegister/RptReceipt.rpt");
            Session["rptname"] = "RptReceipt";
            Session["exportFormat"] = exportFormat;
			this.LoadCMP();
            //this.GenerateReceipt();
			return this.Direct();
		}

		//This method print a receipt of customer
        public void GenerateReceipt()
		{
            bool isValid = false;
			ReportDocument rptH = new ReportDocument();
           try
           {
               string path = (string)Session["path"];
               int saleID = (int)Session["SaleID"];
               string exportFormat = (string)Session["exportFormat"];
               double receiveAmoung = (double)Session["SliceAmount"];
               List<object> model = new List<object>();

               //string path = "";
               string defaultImgPath = Server.MapPath("~/Content/Images/App/deboy-logo.png");
               byte[] COMPANY_LOGO = System.IO.File.ReadAllBytes(defaultImgPath);
               SaleE currentSale = (from sal in db.Sales
                                    where sal.SaleID == saleID
                                    select sal).SingleOrDefault();
               //_saleRepository.FindAll.FirstOrDefault(s => s.SaleID == saleID);
               Customer customer = (from cust in db.Customers
                                    where cust.GlobalPersonID == currentSale.CustomerID
                                    select cust).SingleOrDefault();
               //_personRepository.FindAll.OfType<Customer>().FirstOrDefault(p => p.GlobalPersonID == currentSale.CustomerID);
               double saleAmount = 0d;
               _lineRepository.FindAll.OfType<SaleLine>().Where(l => l.SaleID == saleID).ToList().ForEach(c =>
               {
                   isValid = true;
                   saleAmount += c.LineAmount;
                   model.Add(
                                   new
                                   {
                                       ReceiveAmount = receiveAmoung,
                                       LineQuantity = c.LineQuantity,
                                       LineUnitPrice = c.LineUnitPrice,
                                       ProductLabel = c.ProductLabel,
                                       ProductRef = c.Product.ProductCode,//(c.OeilDroiteGauche == EyeSide.N) ? c.Product.ProductCode : c.OeilDroiteGauche + ":" + c.Product.ProductCode,
                                       CompanyName = Company.Name,
                                       CompanyAdress = "Head Quater : " + Company.Adress.Quarter.Town.Region.RegionLabel + " - " + Company.Adress.Quarter.Town.TownLabel,
                                       CompanyTel = "Tel: " + Company.Adress.AdressPhoneNumber,
                                       BranchName = currentSale.Branch.BranchName,
                                       BranchAdress = currentSale.Branch.Adress.Quarter.QuarterLabel + " - " + currentSale.Branch.Adress.Quarter.Town.TownLabel,
                                       BranchTel = "Tel: " + currentSale.Branch.Adress.AdressCellNumber + "/" + currentSale.Branch.Adress.AdressPhoneNumber,
                                       Ref = currentSale.SaleReceiptNumber,
                                       CompanyCNI = "NO CONT : " + Company.CNI,
                                       Operator = CurrentUser.Name + " " + CurrentUser.Description,
                                       CustomerName = customer.Name/* + (LoadComponent.IsGeneralPublic(customer) ? (" / " + currentSale.Representant) : "")*/,
                                       CustomerAccount = customer.CNI,
                                       SaleDate = currentSale.SaleDeliveryDate,
                                       Title = currentSale.PoliceAssurance/*.Substring(1,currentSale.Representant.Length)*/,
                                       DeviseLabel = currentSale.Devise.DeviseLabel,
                                       CustomerAdress = "Email:" + currentSale.Branch.Adress.AdressEmail,
                                       CompanyLogo = _fileRepository.FindAll.FirstOrDefault(f => f.GlobalPersonID == Company.GlobalPersonID) != null ? _fileRepository.FindAll.FirstOrDefault(f => f.GlobalPersonID == Company.GlobalPersonID).Content : COMPANY_LOGO
                                   }
                           );
               }
                       );

               rptH.Load(path);
               rptH.SetDataSource(model);
               //rptH.SetParameterValue("DeviseLabel", currentSale.Devise.DeviseLabel);
               rptH.SetParameterValue("RateTVA", currentSale.VatRate);
               rptH.SetParameterValue("Reduction", currentSale.RateReduction);
               rptH.SetParameterValue("Discount", currentSale.RateDiscount);
               rptH.SetParameterValue("Transport", currentSale.Transport);
               string reportName = (string)Session["rptname"];
               string applicationName;
               bool isDownloadRpt = (bool)Session["isDownloadRpt"];
               switch (exportFormat)
               {
                   case "pdf":
                       //stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                       //applicationName= "application/pdf";
                       rptH.ExportToHttpResponse(ExportFormatType.PortableDocFormat, System.Web.HttpContext.Current.Response, isDownloadRpt, reportName);
                       break;
                   case "txt":
                       //stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.Text);
                       //applicationName= "application/txt";
                       rptH.ExportToHttpResponse(ExportFormatType.Text, System.Web.HttpContext.Current.Response, isDownloadRpt, reportName);
                       break;
                   case "word":
                       //stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.RichText);
                       //applicationName= "application/rtf";
                       rptH.ExportToHttpResponse(ExportFormatType.WordForWindows, System.Web.HttpContext.Current.Response, isDownloadRpt, reportName);
                       break;
                   default:
                       //stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                       //applicationName= "application/pdf";
                       rptH.ExportToHttpResponse(ExportFormatType.PortableDocFormat, System.Web.HttpContext.Current.Response, isDownloadRpt, reportName);
                       break;
               }

           }
            finally
            {
                rptH.Close();
                rptH.Dispose();
            }
		}



		//Return a list af sales days
		private List<object> ModelSaleDay(DateTime To, DateTime At, int tillID)
		{
			double ecart = 0d;
			List<object> model = new List<object>();
			//1-reglement par caisse
			List<CustomerSlice> lstCustSlice = _sliceRepository.FindAll.OfType<CustomerSlice>().Where(sl => sl.SliceDate >= To && sl.SliceDate <= At && (sl.PaymentMethod is Till && sl.PaymentMethodID == tillID)).ToList();
			lstCustSlice.ForEach(sl =>
			{
				//recuperation de la vente concerne
				SaleE sd = _saleRepository.FindAll.SingleOrDefault(s => s.SaleID == sl.SaleID);
                double saleAmount = sd.SaleLines.Select(l => l.LineAmount).Sum();
				//we take a extra price
				ExtraPrice extra = Util.ExtraPrices(saleAmount, sd.RateReduction, sd.RateDiscount, sd.Transport, sd.VatRate);
				model.Add(
								new
								{
									SaleDate = sl.SliceDate,
									PersonName = sd.PersonName,
									SaleReceiptNumber = sd.SaleReceiptNumber,
									SaleTotalPrice = extra.TotalTTC,
									CashReceived = sl.SliceAmount,// _depositRepository.SaleTotalPriceAdvance(sd),
                                    SaleOperation = "SALE" 
								}
						);
			});
			
			//2-depot d'epargne par caisse
			List<Deposit> lstDeposit = _depositRepository.FindAll.Where(dp => dp.DepositDate >= To && dp.DepositDate <= At && (dp.PaymentMethod1 is Till && dp.PaymentMethodID == tillID)).ToList();
			lstDeposit.ForEach(ldep =>
			{
				model.Add(
								new
								{
									SaleDate = ldep.DepositDate,
									PersonName = ldep.SavingAccount.Customer.CustomerFullName,
									SaleReceiptNumber = "DEP" + ldep.DepositID,
									SaleTotalPrice = ldep.Amount,
									CashReceived = ldep.Amount,
									SaleOperation = "DEPOSIT"
								}
						);
			});

			//recuperation du manquant de la jrne
			List<TillAdjust> lstTillAdjust = _tillAdjust.FindAll.Where(t => t.TillAdjustDate.Date >= To && t.TillAdjustDate.Date <= At && t.TillID == tillID).ToList();
			lstTillAdjust.ForEach(lstad =>
			{
				ecart = lstad.PhysicalPrice - lstad.ComputerPrice;
				if (lstad.PhysicalPrice > lstad.ComputerPrice)
				{
					model.Add(
								new
								{
									//Variables fields
									PurchaseReference = "OVERAGE",
									SupplierFullName = lstad.Till.Code,
									SupplierOperation = "OVERAGE",
									PurchaseBringerFullName = lstad.Justification,
									PurchaseTotalAmount = lstad.PhysicalPrice,
									CashReceivedOupted = ecart
								}
					);
				}
			});
			return model;

		}
		private List<object> ModelPurchaseDay(DateTime To, DateTime At, int tillID)
		{
			double ecart = 0d;
            
			List<object> model = new List<object>();
			List<SupplierSlice> lstSuplierSlice = _sliceRepository.FindAll.OfType<SupplierSlice>().Where(sl => sl.SliceDate >= To && sl.SliceDate <= At && (sl.PaymentMethod is Till && sl.PaymentMethodID == tillID)).ToList();
			lstSuplierSlice.ForEach(sl =>
			{
				//recuperation de l'achat concerne
				Purchase sd = _purchaseRepository.FindAll.SingleOrDefault(s => s.PurchaseID == sl.PurchaseID);
				double purchaseAmount = sd.PurchaseLines.Select(l => l.LineAmount).Sum();
				//we take a extra price
				ExtraPrice extra = Util.ExtraPrices(purchaseAmount, sd.RateReduction, sd.RateDiscount, sd.Transport, sd.VatRate);
				model.Add(
								new
								{
									PurchaseReference = sd.PurchaseReference,
									SupplierFullName = sd.SupplierFullName,
									SupplierOperation = "PURCHASE",
									PurchaseBringerFullName = sd.PurchaseBringerFullName,
									PurchaseTotalAmount = extra.TotalTTC,
									CashReceivedOupted = sl.SliceAmount
								}
						);
			});

            _bdgetCumptnRepository.FindAll.Where(s => (s.PaymentMethod is Till && s.PaymentMethodID == tillID) && s.PaymentDate.Value.Date >= To && s.PaymentDate.Value.Date <= At).ToList().ForEach(sd =>
			{
				model.Add(
							new
							{
								//Variables fields
								PurchaseReference = sd.Reference,
								SupplierFullName = sd.BudgetAllocated.BudgetLine.BudgetLineLabel,
								SupplierOperation = "BUDGET EXPENSE",
								PurchaseBringerFullName = sd.BeneficiaryName,
								PurchaseTotalAmount = sd.VoucherAmount,
								CashReceivedOupted = sd.VoucherAmount
							}
				);
			});

			//recuperation du manquant de la jrne
			List<TillAdjust> lstTillAdjust = _tillAdjust.FindAll.Where(t => t.TillAdjustDate.Date >= To && t.TillAdjustDate.Date <= At && t.TillID == tillID).ToList();
			lstTillAdjust.ForEach(lstad =>
				{
					ecart = lstad.PhysicalPrice - lstad.ComputerPrice;
					if (ecart < 0)
					{
						model.Add(
									new
									{
										//Variables fields
										PurchaseReference = "TELLER ADJUST",
										SupplierFullName = lstad.Till.Code,
										SupplierOperation = "TELLER ADJUST",
										PurchaseBringerFullName = lstad.Justification,
										PurchaseTotalAmount = lstad.PhysicalPrice,
										CashReceivedOupted = ecart * -1
									}
						);
					}
				});

            //recuperation des sorties vers banque
            List<TreasuryOperation> lstTranfbank = (from t in db.TreasuryOperations
                                                    where ((t.OperationDate >= To.Date && t.OperationDate <= At.Date) && t.TillID == tillID && t.OperationType == CodeValue.Accounting.TreasuryOperation.TransfertToBank)
                                                    select t).AsQueryable().ToList();
            lstTranfbank.ForEach(sd =>
                {
                    model.Add(
                            new
                            {
                                //Variables fields
                                PurchaseReference = sd.OperationRef,
                                SupplierFullName = sd.Justification,
                                SupplierOperation = "TRANSFER TO BANK",
                                PurchaseBringerFullName = sd.Justification,
                                PurchaseTotalAmount = sd.OperationAmount,
                                CashReceivedOupted = sd.OperationAmount
                            }
                );
                });

			return model;
		}
		//public ActionResult PrintCloseTeller()
		//{
		//    //this.GetCmp<Panel>("Pdf").LoadContent(new ComponentLoader
		//    //{
		//    //    Url = Url.Action("GenerateReportOperationOfDay"),
		//    //    DisableCaching = false,
		//    //    Mode = LoadMode.Frame,
		//    //    //Params = { new Parameter("sale", sale) }
		//    //});
		//    this.GenerateReportOperationOfDay();
		//    return this.Direct();
		//}
		//This method print report of all operations of cash during a day
		public void GenerateReportOperationOfDay()
		{
           
			//Date and tillDayID
            int tDID = (int)Session["UserTill"];// Session["tillDayID"];
			DateTime tillDayDate = (DateTime)Session["tillDayDate"];
            this.GenerateReportOperationPeriodeOfDay(tillDayDate, tillDayDate, tDID);

		}

		//This method print report of all operations of cash during a period of day
		public void GenerateReportOperationPeriodeOfDay(DateTime To, DateTime At, int tillID)
		{
            bool isValid = false;
            List<object> model = new List<object>();
            ReportDocument rptH = new ReportDocument();
            try
            {
			
            
			//Date and tillDayID
			Till currentTill = (from t in db.Tills where t.ID==tillID
                                    select t).SingleOrDefault();
            //_tillRepository.Find(tillID);
			//=================
			Branch currentBranch = currentTill.Branch;
			string defaultImgPath = Server.MapPath("~/Content/Images/App/deboy-logo.png");
			byte[] COMPANY_LOGO = System.IO.File.ReadAllBytes(defaultImgPath);
			//List of operation of current date

            
            DateTime dateop = To.Date;
            do
            {

                TillDay tillDay = _tillDayRepository.FindAll.SingleOrDefault(td => td.TillID == tillID && td.TillDayDate == dateop.Date);
                double TillDayOpenPrice = tillDay != null ? tillDay.TillDayOpenPrice : 0;
                double TillDayClosingPrice = tillDay != null ? tillDay.TillDayClosingPrice : 0;

                if (tillDay != null)
                {


                    //INPUTS OPERATION HISTORIC
                    //1-reglement par caisse des ventes
                    List<CustomerSlice> lstCustSlice = _sliceRepository.FindAll.OfType<CustomerSlice>().Where(sl => sl.SliceDate.Date == dateop.Date /* >= To.Date && sl.SliceDate.Date <= At.Date*/ && (sl.PaymentMethod is Till && sl.PaymentMethodID == tillID) && !sl.isDeposit).ToList();
                    lstCustSlice.ForEach(sl =>
                    {
                        isValid = true;

                        //SaleE sd=_customerReturnRepository.GetRealSale(sl.Sale);

                        //recuperation de la vente concerne
                        SaleE sd = _saleRepository.FindAll.SingleOrDefault(s => s.SaleID == sl.SaleID);
                       
                        double saleAmount = sd.SaleLines.Select(l => l.LineAmount).Sum();
                        //we take a extra price
                        ExtraPrice extra = Util.ExtraPrices(saleAmount, sd.RateReduction, sd.RateDiscount, sd.Transport, sd.VatRate);
                        model.Add(
                                    new
                                    {
                                        //Variables fields
                                        RptCashDayOperationID = 0,
                                        OperationDate = sl.SliceDate,
                                        GroupingDate = new DateTime(sl.SliceDate.Year, sl.SliceDate.Month, sl.SliceDate.Day).ToOADate(),
                                        BeginDate = To,
                                        EndDate = At,
                                        InputAmount = sl.SliceAmount,//  _depositRepository.SaleTotalPriceAdvance(sd),
                                        OutPutAmount = 0,
                                        Intervenant = sd.CustomerName,
                                        Solde = extra.TotalTTC, //this field represent a real amount of operation
                                        Operation = "SALE" ,
                                        TransactionNumber = sd.SaleReceiptNumber,
                                        Description = sd.SaleFullInformation,// "No Comment",
                                        //Head fields
                                        Teller = CurrentUser.Name + " " + CurrentUser.Description,
                                        CashRegisterName = currentTill.Name + " " + currentTill.Code,
                                        RptTitle = Resources.TellerHistDayOp,
                                        OpeningCashAmount = TillDayOpenPrice,
                                        ClosingCashAmount = TillDayClosingPrice,
                                        BranchName = currentBranch.BranchName,
                                        BranchAdress = currentBranch.Adress.Quarter.QuarterLabel + " - " + currentBranch.Adress.Quarter.Town.TownLabel,
                                        BranchTel = "Tel: " + currentBranch.Adress.AdressPhoneNumber + "PO Box :" + currentBranch.Adress.AdressPOBox,
                                        CompanyName = Company.Name,
                                        CompanyAdress = "Head Quater : " + Company.Adress.Quarter.Town.Region.RegionLabel + " - " + Company.Adress.Quarter.Town.TownLabel,
                                        CompanyTel = "Tel: " + Company.Adress.AdressPhoneNumber,
                                        CompanyCNI = "NO CONT : " + Company.CNI,
                                        CompanyLogo = _fileRepository.FindAll.FirstOrDefault(f => f.GlobalPersonID == Company.GlobalPersonID) != null ? _fileRepository.FindAll.FirstOrDefault(f => f.GlobalPersonID == Company.GlobalPersonID).Content : COMPANY_LOGO,
                                        PaymentMethod = currentTill.Code
                                    }
                        );
                    });
                    
                    //2-tous les depots par la caisse ----depot d'epargne par caisse
                    //List<Deposit> lstDeposit = _depositRepository.FindAll.Where(dp => (dp.DepositDate.Date >= To.Date && dp.DepositDate.Date <= At.Date) && (dp.PaymentMethod1 is Till && dp.PaymentMethodID == tillID)).ToList();
                    List<AllDeposit> lstDeposit = db.AllDeposits.Where(dp => (dp.AllDepositDate == dateop.Date /* >= To.Date && dp.AllDepositDate <= At.Date*/) && (dp.PaymentMethod is Till && dp.PaymentMethodID == tillID)).ToList();
                    lstDeposit.ForEach(ldep =>
                    {
                        isValid = true;
                        
                        model.Add(
                                        new
                                        {
                                            RptCashDayOperationID = 0,
                                            OperationDate = ldep.AllDepositDate,
                                            GroupingDate = new DateTime(ldep.AllDepositDate.Year, ldep.AllDepositDate.Month, ldep.AllDepositDate.Day).ToOADate(),
                                            BeginDate = To,
                                            EndDate = At,
                                            InputAmount = ldep.Amount,
                                            OutPutAmount = 0,
                                            Intervenant = ldep.Representant,
                                            Solde = ldep.Amount, //this field represent a real amount of operation
                                            Operation = "DEPOSIT",
                                            TransactionNumber = ldep.AllDepositReference,
                                            Description = ldep.AllDepositReason,// "No Comment",
                                            //Head fields
                                            Teller = CurrentUser.Name + " " + CurrentUser.Description,
                                            CashRegisterName = currentTill.Name + " " + currentTill.Code,
                                            RptTitle = Resources.TellerHistDayOp,
                                            OpeningCashAmount = TillDayOpenPrice,
                                            ClosingCashAmount = TillDayClosingPrice,
                                            BranchName = currentBranch.BranchName,
                                            BranchAdress = currentBranch.Adress.Quarter.QuarterLabel + " - " + currentBranch.Adress.Quarter.Town.TownLabel,
                                            BranchTel = "Tel: " + currentBranch.Adress.AdressPhoneNumber + "PO Box :" + currentBranch.Adress.AdressPOBox,
                                            CompanyName = Company.Name,
                                            CompanyAdress = "Head Quater : " + Company.Adress.Quarter.Town.Region.RegionLabel + " - " + Company.Adress.Quarter.Town.TownLabel,
                                            CompanyTel = "Tel: " + Company.Adress.AdressPhoneNumber,
                                            CompanyCNI = "NO CONT : " + Company.CNI,
                                            CompanyLogo = _fileRepository.FindAll.FirstOrDefault(f => f.GlobalPersonID == Company.GlobalPersonID) != null ? _fileRepository.FindAll.FirstOrDefault(f => f.GlobalPersonID == Company.GlobalPersonID).Content : COMPANY_LOGO,
                                            PaymentMethod = currentTill.Code
                                        }
                                );
                    });

                    //SURPLUS DE CAISSE PAR AJUSTEMENT
                    //TillAdjust lstTillAdjustOverage = _tillAdjust.FindAll.LastOrDefault(t => t.TillAdjustDate.Date >= To && t.TillAdjustDate.Date <= At && t.TillID == currentTill.ID);
                    //if (lstTillAdjustOverage != null)
                    List<TillAdjust> lstTillAdjustOveragePeriod = _tillAdjust.FindAll.Where(t => t.TillAdjustDate.Date >= To && t.TillAdjustDate.Date <= At && t.TillID == currentTill.ID).ToList();
                    lstTillAdjustOveragePeriod.ForEach(lstTillAdjustOverage =>
                    {

                        double ecart = lstTillAdjustOverage.PhysicalPrice - lstTillAdjustOverage.ComputerPrice;
                        if (ecart > 0)
                        {
                            isValid = true;
                            model.Add(
                                        new
                                        {
                                            RptCashDayOperationID = 0,
                                            OperationDate = lstTillAdjustOverage.TillAdjustDate,
                                            GroupingDate = new DateTime(lstTillAdjustOverage.TillAdjustDate.Year, lstTillAdjustOverage.TillAdjustDate.Month, lstTillAdjustOverage.TillAdjustDate.Day).ToOADate(),
                                            BeginDate = To,
                                            EndDate = At,
                                            InputAmount = ecart,
                                            OutPutAmount = 0,
                                            Intervenant = lstTillAdjustOverage.Justification,
                                            Solde = lstTillAdjustOverage.PhysicalPrice, //this field represent a real amount of operation
                                            Operation = "OVERAGE",
                                            TransactionNumber = "OVE" + lstTillAdjustOverage.TillID,
                                            Description = lstTillAdjustOverage.Justification,// "No Comment",
                                            //Head fields
                                            Teller = CurrentUser.Name + " " + CurrentUser.Description,
                                            CashRegisterName = currentTill.Name + " " + currentTill.Code,
                                            RptTitle = Resources.TellerHistDayOp,
                                            OpeningCashAmount = TillDayOpenPrice,
                                            ClosingCashAmount = TillDayClosingPrice,
                                            BranchName = currentBranch.BranchName,
                                            BranchAdress = currentBranch.Adress.Quarter.QuarterLabel + " - " + currentBranch.Adress.Quarter.Town.TownLabel,
                                            BranchTel = "Tel: " + currentBranch.Adress.AdressPhoneNumber + "PO Box :" + currentBranch.Adress.AdressPOBox,
                                            CompanyName = Company.Name,
                                            CompanyAdress = "Head Quater : " + Company.Adress.Quarter.Town.Region.RegionLabel + " - " + Company.Adress.Quarter.Town.TownLabel,
                                            CompanyTel = "Tel: " + Company.Adress.AdressPhoneNumber,
                                            CompanyCNI = "NO CONT : " + Company.CNI,
                                            CompanyLogo = _fileRepository.FindAll.FirstOrDefault(f => f.GlobalPersonID == Company.GlobalPersonID) != null ? _fileRepository.FindAll.FirstOrDefault(f => f.GlobalPersonID == Company.GlobalPersonID).Content : COMPANY_LOGO,
                                            PaymentMethod = currentTill.Code
                                        }
                            );
                        }
                    });

                    //recupartion des versements caisse

                    //recuperation des versement bancaire

                    //recuperation retour achat

                    //OUTPUTS OPERATIONS HISTORIQUE
                    //purchase
                    List<SupplierSlice> lstSuplierSlice = _sliceRepository.FindAll.OfType<SupplierSlice>().Where(supl => (supl.SliceDate.Date == dateop.Date /* >= To.Date && supl.SliceDate.Date <= At.Date*/) && (supl.PaymentMethod is Till && supl.PaymentMethodID == tillID)).ToList();
                    lstSuplierSlice.ForEach(sl =>
                    {
                        isValid = true;
                        
                        //recuperation de l'achat concerne
                        Purchase sd = _purchaseRepository.FindAll.SingleOrDefault(s => s.PurchaseID == sl.PurchaseID);
                        double purchaseAmount = sd.PurchaseLines.Select(l => l.LineAmount).Sum();
                        //we take a extra price
                        ExtraPrice extra = Util.ExtraPrices(purchaseAmount, sd.RateReduction, sd.RateDiscount, sd.Transport, sd.VatRate);
                        model.Add(
                                    new
                                    {
                                        //Variables fields
                                        RptCashDayOperationID = 12456,
                                        OperationDate = sl.SliceDate,
                                        GroupingDate = new DateTime(sl.SliceDate.Year, sl.SliceDate.Month, sl.SliceDate.Day).ToOADate(),
                                        BeginDate = To,
                                        EndDate = At,
                                        InputAmount = 0,
                                        Solde = extra.TotalTTC, //this field represent a real amount of operation
                                        OutPutAmount = sl.SliceAmount,
                                        Intervenant = sd.Supplier.Name,
                                        Operation = "PURCHASE",
                                        TransactionNumber = sd.PurchaseReference,
                                        Description = sd.PurchaseRegister,//"No Comment",
                                        //Head fields
                                        Teller = CurrentUser.Name + " " + CurrentUser.Description,
                                        CashRegisterName = currentTill.Name + " " + currentTill.Code,
                                        RptTitle = Resources.TellerHistDayOp,
                                        OpeningCashAmount = TillDayOpenPrice,
                                        ClosingCashAmount = TillDayClosingPrice,
                                        BranchName = currentBranch.BranchName,
                                        BranchAdress = currentBranch.Adress.Quarter.QuarterLabel + " - " + currentBranch.Adress.Quarter.Town.TownLabel,
                                        BranchTel = "Tel: " + currentBranch.Adress.AdressPhoneNumber + "PO Box :" + currentBranch.Adress.AdressPOBox,
                                        CompanyName = Company.Name,
                                        CompanyAdress = "Head Quater : " + Company.Adress.Quarter.Town.Region.RegionLabel + " - " + Company.Adress.Quarter.Town.TownLabel,
                                        CompanyTel = "Tel: " + Company.Adress.AdressPhoneNumber,
                                        CompanyCNI = "NO CONT : " + Company.CNI,
                                        CompanyLogo = _fileRepository.FindAll.FirstOrDefault(f => f.GlobalPersonID == Company.GlobalPersonID) != null ? _fileRepository.FindAll.FirstOrDefault(f => f.GlobalPersonID == Company.GlobalPersonID).Content : COMPANY_LOGO,
                                        PaymentMethod = currentTill.Code
                                    }
                        );
                    });
                    //BUDGET CONSUMTION
                    //recupartion des consommation du budget
                    _bdgetCumptnRepository.FindAll.Where(s => (s.PaymentDate.HasValue && s.PaymentDate.Value.Date == dateop.Date /*>= To.Date && s.PaymentDate.Value.Date <= At.Date*/) && (s.PaymentMethod is Till && s.PaymentMethodID == tillID)).ToList().ForEach(sd =>
                    {
                        isValid = true;
                        
                        var payment = _paymentMethodRepository.FindAll.OfType<Till>().FirstOrDefault(p => p.ID == sd.PaymentMethodID);
                        if (payment != null && payment.ID == currentTill.ID)
                        {
                            model.Add(
                                        new
                                        {
                                            //Variables fields
                                            RptCashDayOperationID = 12456,
                                            OperationDate = sd.PaymentDate.Value,
                                            GroupingDate = new DateTime(sd.PaymentDate.Value.Year, sd.PaymentDate.Value.Month, sd.PaymentDate.Value.Day).ToOADate(),
                                            BeginDate = To,
                                            EndDate = At,
                                            InputAmount = 0,
                                            Solde = sd.VoucherAmount, //this field represent a real amount of operation
                                            OutPutAmount = sd.VoucherAmount,
                                            Intervenant = sd.BeneficiaryName + " / " + sd.Justification,
                                            Operation = "BUDGET CONSUMPTION ",// + sd.BudgetLine.BudgetLineLabel,
                                            TransactionNumber = sd.Reference.ToString(),
                                            Description = sd.BudgetAllocated.BudgetLine.BudgetLineLabel,// "No Comment",
                                            //Head fields
                                            Teller = CurrentUser.Name + " " + CurrentUser.Description,
                                            CashRegisterName = currentTill.Name + " " + currentTill.Code,
                                            RptTitle = Resources.TellerHistDayOp,
                                            OpeningCashAmount = TillDayOpenPrice,
                                            ClosingCashAmount = TillDayClosingPrice,
                                            BranchName = currentBranch.BranchName,
                                            BranchAdress = currentBranch.Adress.Quarter.QuarterLabel + " - " + currentBranch.Adress.Quarter.Town.TownLabel,
                                            BranchTel = "Tel: " + currentBranch.Adress.AdressPhoneNumber + "PO Box :" + currentBranch.Adress.AdressPOBox,
                                            CompanyName = Company.Name,
                                            CompanyAdress = "Head Quater : " + Company.Adress.Quarter.Town.Region.RegionLabel + " - " + Company.Adress.Quarter.Town.TownLabel,
                                            CompanyTel = "Tel: " + Company.Adress.AdressPhoneNumber,
                                            CompanyCNI = "NO CONT : " + Company.CNI,
                                            CompanyLogo = _fileRepository.FindAll.FirstOrDefault(f => f.GlobalPersonID == Company.GlobalPersonID) != null ? _fileRepository.FindAll.FirstOrDefault(f => f.GlobalPersonID == Company.GlobalPersonID).Content : COMPANY_LOGO,
                                            PaymentMethod = currentTill.Code
                                        }
                            );
                        }
                    });

                    //MANQUANT DE CAISSE PAR AJUSTEMENT
                    //TillAdjust lstTillAdjustSh = _tillAdjust.FindAll.LastOrDefault(t => t.TillAdjustDate.Date >= To && t.TillAdjustDate.Date <= At && t.TillID == currentTill.ID);
                    List<TillAdjust> lstTillAdjustShPeriod = _tillAdjust.FindAll.Where(t => t.TillAdjustDate.Date >= To && t.TillAdjustDate.Date <= At && t.TillID == currentTill.ID).ToList();
                    //if (lstTillAdjustSh != null)
                    lstTillAdjustShPeriod.ForEach(lstTillAdjustSh =>
                    {

                        double ecart = lstTillAdjustSh.PhysicalPrice - lstTillAdjustSh.ComputerPrice;
                        if (ecart < 0)
                        {
                            isValid = true;
                            model.Add(
                                        new
                                        {
                                            RptCashDayOperationID = 0,
                                            OperationDate = lstTillAdjustSh.TillAdjustDate,
                                            GroupingDate = new DateTime(lstTillAdjustSh.TillAdjustDate.Year, lstTillAdjustSh.TillAdjustDate.Month, lstTillAdjustSh.TillAdjustDate.Day).ToOADate(),
                                            BeginDate = To,
                                            EndDate = At,
                                            InputAmount = 0,
                                            OutPutAmount = ecart * -1,
                                            Intervenant = lstTillAdjustSh.Justification,
                                            Solde = lstTillAdjustSh.PhysicalPrice, //this field represent a real amount of operation
                                            Operation = "TELLER ADJUST",
                                            TransactionNumber = "TADJ" + lstTillAdjustSh.TillID,
                                            Description = lstTillAdjustSh.Justification,// "No Comment",
                                            //Head fields
                                            Teller = CurrentUser.Name + " " + CurrentUser.Description,
                                            CashRegisterName = currentTill.Name + " " + currentTill.Code,
                                            RptTitle = Resources.TellerHistDayOp,
                                            OpeningCashAmount = TillDayOpenPrice,
                                            ClosingCashAmount = TillDayClosingPrice,
                                            BranchName = currentBranch.BranchName,
                                            BranchAdress = currentBranch.Adress.Quarter.QuarterLabel + " - " + currentBranch.Adress.Quarter.Town.TownLabel,
                                            BranchTel = "Tel: " + currentBranch.Adress.AdressPhoneNumber + "PO Box :" + currentBranch.Adress.AdressPOBox,
                                            CompanyName = Company.Name,
                                            CompanyAdress = "Head Quater : " + Company.Adress.Quarter.Town.Region.RegionLabel + " - " + Company.Adress.Quarter.Town.TownLabel,
                                            CompanyTel = "Tel: " + Company.Adress.AdressPhoneNumber,
                                            CompanyCNI = "NO CONT : " + Company.CNI,
                                            CompanyLogo = _fileRepository.FindAll.FirstOrDefault(f => f.GlobalPersonID == Company.GlobalPersonID) != null ? _fileRepository.FindAll.FirstOrDefault(f => f.GlobalPersonID == Company.GlobalPersonID).Content : COMPANY_LOGO,
                                            PaymentMethod = currentTill.Code
                                        }
                            );
                        }
                    });

                    //recuperation des sorties vers caisse

                    //recuperation des sorties vers banque
                    List<TreasuryOperation> lstTranfbank = (from t in db.TreasuryOperations
                                                            where ((t.OperationDate == dateop.Date /*>= To.Date && t.OperationDate <= At.Date*/) && t.TillID == currentTill.ID && t.OperationType == CodeValue.Accounting.TreasuryOperation.TransfertToBank)
                                                            select t).AsQueryable().ToList();
                    lstTranfbank.ForEach(sd =>
                    {
                        model.Add(
                                    new
                                    {
                                        //Variables fields
                                        RptCashDayOperationID = 124567,
                                        OperationDate = sd.OperationDate,
                                        GroupingDate = new DateTime(sd.OperationDate.Year, sd.OperationDate.Month, sd.OperationDate.Day).ToOADate(),
                                        BeginDate = To,
                                        EndDate = At,
                                        InputAmount = 0,
                                        Solde = sd.OperationAmount, //this field represent a real amount of operation
                                        OutPutAmount = sd.OperationAmount,
                                        Intervenant = sd.Justification,
                                        Operation = "BANK TRANSFER",// + sd.BudgetLine.BudgetLineLabel,
                                        TransactionNumber = sd.OperationRef,
                                        Description = sd.Justification,// "No Comment",
                                        //Head fields
                                        Teller = CurrentUser.Name + " " + CurrentUser.Description,
                                        CashRegisterName = currentTill.Name + " " + currentTill.Code,
                                        RptTitle = Resources.TellerHistDayOp,
                                        OpeningCashAmount = TillDayOpenPrice,
                                        ClosingCashAmount = TillDayClosingPrice,
                                        BranchName = currentBranch.BranchName,
                                        BranchAdress = currentBranch.Adress.Quarter.QuarterLabel + " - " + currentBranch.Adress.Quarter.Town.TownLabel,
                                        BranchTel = "Tel: " + currentBranch.Adress.AdressPhoneNumber + "PO Box :" + currentBranch.Adress.AdressPOBox,
                                        CompanyName = Company.Name,
                                        CompanyAdress = "Head Quater : " + Company.Adress.Quarter.Town.Region.RegionLabel + " - " + Company.Adress.Quarter.Town.TownLabel,
                                        CompanyTel = "Tel: " + Company.Adress.AdressPhoneNumber,
                                        CompanyCNI = "NO CONT : " + Company.CNI,
                                        CompanyLogo = _fileRepository.FindAll.FirstOrDefault(f => f.GlobalPersonID == Company.GlobalPersonID) != null ? _fileRepository.FindAll.FirstOrDefault(f => f.GlobalPersonID == Company.GlobalPersonID).Content : COMPANY_LOGO,
                                        PaymentMethod = currentTill.Code
                                    }
                        );
                    });

                    
                    
                    //recuperation des retours sur ventes
                    List<CustomerReturnSlice> returnSlices = (from r in db.CustomerReturnSlices
                                                              where ((r.SliceDate == dateop.Date ) && (r.PaymentMethodID == tillID))
                                                              select r).AsQueryable().ToList();
                 
                    returnSlices.ForEach(sd =>
                    {
                        SaleE sale = _depositRepository.ApplyExtraPrice(sd.CustomerReturn.Sale);
                   
                        model.Add(
                                        new
                                        {
                                            //Variables fields
                                            RptCashDayOperationID = 124567,
                                            OperationDate = sd.SliceDate,
                                            GroupingDate = new DateTime(sd.SliceDate.Year, sd.SliceDate.Month, sd.SliceDate.Day).ToOADate(),
                                            BeginDate = To,
                                            EndDate = At,
                                            InputAmount = 0,
                                            Solde = sd.SliceAmount,// sale.TotalPriceReturn,// sale.TotalPriceTTC, //this field represent a real amount of operation
                                            OutPutAmount = sd.SliceAmount,//sale.TotalPriceReturn,//sale.TotalPriceTTC,
                                            Intervenant = sale.CustomerName,
                                            Operation = "SALE RETURN",
                                            TransactionNumber = sale.SaleReceiptNumber,
                                            Description = (sd.Representant == null) ? sale.CustomerName : sd.Representant,// "No Comment",
                                            //Head fields
                                            Teller = CurrentUser.Name + " " + CurrentUser.Description,
                                            CashRegisterName = currentTill.Name + " " + currentTill.Code,
                                            RptTitle = Resources.TellerHistDayOp,
                                            OpeningCashAmount = TillDayOpenPrice,
                                            ClosingCashAmount = TillDayClosingPrice,
                                            BranchName = currentBranch.BranchName,
                                            BranchAdress = currentBranch.Adress.Quarter.QuarterLabel + " - " + currentBranch.Adress.Quarter.Town.TownLabel,
                                            BranchTel = "Tel: " + currentBranch.Adress.AdressPhoneNumber + "PO Box :" + currentBranch.Adress.AdressPOBox,
                                            CompanyName = Company.Name,
                                            CompanyAdress = "Head Quater : " + Company.Adress.Quarter.Town.Region.RegionLabel + " - " + Company.Adress.Quarter.Town.TownLabel,
                                            CompanyTel = "Tel: " + Company.Adress.AdressPhoneNumber,
                                            CompanyCNI = "NO CONT : " + Company.CNI,
                                            CompanyLogo = _fileRepository.FindAll.FirstOrDefault(f => f.GlobalPersonID == Company.GlobalPersonID) != null ? _fileRepository.FindAll.FirstOrDefault(f => f.GlobalPersonID == Company.GlobalPersonID).Content : COMPANY_LOGO,
                                            PaymentMethod = currentTill.Code
                                        }
                            );
                    });
                     
                }
                    dateop = dateop.AddDays(1);
                }   while (dateop <= At.Date)    ;


                //TRATEMENT BANK
                DateTime dateopbnk = To.Date;
                do
                {

                    //INPUTS OPERATION HISTORIC
                    //1-reglement par caisse des ventes
                    List<CustomerSlice> lstCustBnkSlice = _sliceRepository.FindAll.OfType<CustomerSlice>().Where(sl => sl.SliceDate.Date == dateopbnk.Date /*>= To.Date && sl.SliceDate.Date <= At.Date*/ && (sl.PaymentMethod is Bank) && !sl.isDeposit).ToList();
                    lstCustBnkSlice.ForEach(sl =>
                    {
                        isValid = true;

                        var payment = _paymentMethodRepository.FindAll.OfType<Bank>().FirstOrDefault(p => p.ID == sl.PaymentMethodID);
                        //recuperation de la vente concerne
                        SaleE sd = _saleRepository.FindAll.SingleOrDefault(s => s.SaleID == sl.SaleID);
                        double saleAmount = sd.SaleLines.Select(l => l.LineAmount).Sum();
                        //we take a extra price
                        ExtraPrice extra = Util.ExtraPrices(saleAmount, sd.RateReduction, sd.RateDiscount, sd.Transport, sd.VatRate);
                        model.Add(
                                    new
                                    {
                                        //Variables fields
                                        RptCashDayOperationID = 0,
                                        OperationDate = sl.SliceDate,
                                        GroupingDate = new DateTime(sl.SliceDate.Year, sl.SliceDate.Month, sl.SliceDate.Day).ToOADate(),
                                        BeginDate = To,
                                        EndDate = At,
                                        InputAmount = sl.SliceAmount,//  _depositRepository.SaleTotalPriceAdvance(sd),
                                        OutPutAmount = 0,
                                        Intervenant = sd.CustomerName,
                                        Solde = extra.TotalTTC, //this field represent a real amount of operation
                                        Operation = "SALE",
                                        TransactionNumber = sd.SaleReceiptNumber,
                                        Description = sd.SaleFullInformation,// "No Comment",
                                        //Head fields
                                        Teller = CurrentUser.Name + " " + CurrentUser.Description,
                                        CashRegisterName = payment.Name + " " + payment.Code,
                                        RptTitle = Resources.TellerHistDayOp,
                                        OpeningCashAmount = 0,
                                        ClosingCashAmount = 0,
                                        BranchName = currentBranch.BranchName,
                                        BranchAdress = currentBranch.Adress.Quarter.QuarterLabel + " - " + currentBranch.Adress.Quarter.Town.TownLabel,
                                        BranchTel = "Tel: " + currentBranch.Adress.AdressPhoneNumber + "PO Box :" + currentBranch.Adress.AdressPOBox,
                                        CompanyName = Company.Name,
                                        CompanyAdress = "Head Quater : " + Company.Adress.Quarter.Town.Region.RegionLabel + " - " + Company.Adress.Quarter.Town.TownLabel,
                                        CompanyTel = "Tel: " + Company.Adress.AdressPhoneNumber,
                                        CompanyCNI = "NO CONT : " + Company.CNI,
                                        CompanyLogo = _fileRepository.FindAll.FirstOrDefault(f => f.GlobalPersonID == Company.GlobalPersonID) != null ? _fileRepository.FindAll.FirstOrDefault(f => f.GlobalPersonID == Company.GlobalPersonID).Content : COMPANY_LOGO,
                                        PaymentMethod = payment.Code
                                    }
                        );
                    });

                    
                    //2-tous les depots par la caisse ----depot d'epargne par caisse
                    List<AllDeposit> lstDepositBnk = db.AllDeposits.Where(dp => (dp.AllDepositDate == dateopbnk.Date /*>= To.Date && dp.AllDepositDate <= At.Date*/) && (dp.PaymentMethod is Bank)).ToList();
                    lstDepositBnk.ForEach(ldep =>
                    {
                        isValid = true;
                        var payment = _paymentMethodRepository.FindAll.OfType<Bank>().FirstOrDefault(p => p.ID == ldep.PaymentMethodID);
                        
                        model.Add(
                                        new
                                        {
                                            RptCashDayOperationID = 0,
                                            OperationDate = ldep.AllDepositDate,
                                            GroupingDate = new DateTime(ldep.AllDepositDate.Year, ldep.AllDepositDate.Month, ldep.AllDepositDate.Day).ToOADate(),
                                            BeginDate = To,
                                            EndDate = At,
                                            InputAmount = ldep.Amount,
                                            OutPutAmount = 0,
                                            Intervenant = ldep.Representant,
                                            Solde = ldep.Amount, //this field represent a real amount of operation
                                            Operation = "DEPOSIT",
                                            TransactionNumber = ldep.AllDepositReference,
                                            Description = ldep.AllDepositReason,// "No Comment",
                                            //Head fields
                                            Teller = CurrentUser.Name + " " + CurrentUser.Description,
                                            CashRegisterName = payment.Name + " " + payment.Code,
                                            RptTitle = Resources.TellerHistDayOp,
                                            OpeningCashAmount = 0,
                                            ClosingCashAmount = 0,
                                            BranchName = currentBranch.BranchName,
                                            BranchAdress = currentBranch.Adress.Quarter.QuarterLabel + " - " + currentBranch.Adress.Quarter.Town.TownLabel,
                                            BranchTel = "Tel: " + currentBranch.Adress.AdressPhoneNumber + "PO Box :" + currentBranch.Adress.AdressPOBox,
                                            CompanyName = Company.Name,
                                            CompanyAdress = "Head Quater : " + Company.Adress.Quarter.Town.Region.RegionLabel + " - " + Company.Adress.Quarter.Town.TownLabel,
                                            CompanyTel = "Tel: " + Company.Adress.AdressPhoneNumber,
                                            CompanyCNI = "NO CONT : " + Company.CNI,
                                            CompanyLogo = _fileRepository.FindAll.FirstOrDefault(f => f.GlobalPersonID == Company.GlobalPersonID) != null ? _fileRepository.FindAll.FirstOrDefault(f => f.GlobalPersonID == Company.GlobalPersonID).Content : COMPANY_LOGO,
                                            PaymentMethod = payment.Code
                                        }
                                );
                    });


                    //recupartion des versements BANQUE

                    //OUTPUTS OPERATIONS HISTORIQUE
                    //purchase
                    List<SupplierSlice> lstSuplierSliceBnk = _sliceRepository.FindAll.OfType<SupplierSlice>().Where(supl => (supl.SliceDate.Date == dateopbnk.Date /* >= To.Date && supl.SliceDate.Date <= At.Date*/) && (supl.PaymentMethod is Bank)).ToList();
                    lstSuplierSliceBnk.ForEach(sl =>
                    {
                        isValid = true;
                        var payment = _paymentMethodRepository.FindAll.OfType<Bank>().FirstOrDefault(p => p.ID == sl.PaymentMethodID);
                        
                        //recuperation de l'achat concerne
                        Purchase sd = _purchaseRepository.FindAll.SingleOrDefault(s => s.PurchaseID == sl.PurchaseID);
                        double purchaseAmount = sd.PurchaseLines.Select(l => l.LineAmount).Sum();
                        //we take a extra price
                        ExtraPrice extra = Util.ExtraPrices(purchaseAmount, sd.RateReduction, sd.RateDiscount, sd.Transport, sd.VatRate);
                        model.Add(
                                    new
                                    {
                                        //Variables fields
                                        RptCashDayOperationID = 12456,
                                        OperationDate = sl.SliceDate,
                                        GroupingDate = new DateTime(sl.SliceDate.Year, sl.SliceDate.Month, sl.SliceDate.Day).ToOADate(),
                                        BeginDate = To,
                                        EndDate = At,
                                        InputAmount = 0,
                                        Solde = extra.TotalTTC, //this field represent a real amount of operation
                                        OutPutAmount = sl.SliceAmount,
                                        Intervenant = sd.Supplier.Name + " " + sd.Supplier.Description,
                                        Operation = "PURCHASE",
                                        TransactionNumber = sd.PurchaseReference,
                                        Description = sd.PurchaseRegister,//"No Comment",
                                        //Head fields
                                        Teller = CurrentUser.Name + " " + CurrentUser.Description,
                                        CashRegisterName = payment.Name + " " + payment.Code,
                                        RptTitle = Resources.TellerHistDayOp,
                                        OpeningCashAmount = 0,
                                        ClosingCashAmount = 0,
                                        BranchName = currentBranch.BranchName,
                                        BranchAdress = currentBranch.Adress.Quarter.QuarterLabel + " - " + currentBranch.Adress.Quarter.Town.TownLabel,
                                        BranchTel = "Tel: " + currentBranch.Adress.AdressPhoneNumber + "PO Box :" + currentBranch.Adress.AdressPOBox,
                                        CompanyName = Company.Name,
                                        CompanyAdress = "Head Quater : " + Company.Adress.Quarter.Town.Region.RegionLabel + " - " + Company.Adress.Quarter.Town.TownLabel,
                                        CompanyTel = "Tel: " + Company.Adress.AdressPhoneNumber,
                                        CompanyCNI = "NO CONT : " + Company.CNI,
                                        CompanyLogo = _fileRepository.FindAll.FirstOrDefault(f => f.GlobalPersonID == Company.GlobalPersonID) != null ? _fileRepository.FindAll.FirstOrDefault(f => f.GlobalPersonID == Company.GlobalPersonID).Content : COMPANY_LOGO,
                                        PaymentMethod = payment.Code
                                    }
                        );
                    });
                    //BUDGET CONSUMTION
                    //recupartion des consommation du budget
                    _bdgetCumptnRepository.FindAll.Where(s => (s.PaymentDate.HasValue && s.PaymentDate.Value.Date == dateopbnk.Date /* >= To.Date && s.PaymentDate.Value.Date <= At.Date*/) && (s.PaymentMethod is Bank)).ToList().ForEach(sd =>
                    {
                        isValid = true;
                        var payment = _paymentMethodRepository.FindAll.OfType<Bank>().FirstOrDefault(p => p.ID == sd.PaymentMethodID);
                        if (payment != null && payment.ID == currentTill.ID)
                        {
                            model.Add(
                                        new
                                        {
                                            //Variables fields
                                            RptCashDayOperationID = 12456,
                                            OperationDate = sd.PaymentDate.Value,
                                            GroupingDate = new DateTime(sd.PaymentDate.Value.Year, sd.PaymentDate.Value.Month, sd.PaymentDate.Value.Day).ToOADate(),
                                            BeginDate = To,
                                            EndDate = At,
                                            InputAmount = 0,
                                            Solde = sd.VoucherAmount, //this field represent a real amount of operation
                                            OutPutAmount = sd.VoucherAmount,
                                            Intervenant = sd.BeneficiaryName + " / " + sd.Justification,
                                            Operation = "BUDGET CONSUMPTION ",// + sd.BudgetLine.BudgetLineLabel,
                                            TransactionNumber = sd.Reference.ToString(),
                                            Description = sd.BudgetAllocated.BudgetLine.BudgetLineLabel,// "No Comment",
                                            //Head fields
                                            Teller = CurrentUser.Name + " " + CurrentUser.Description,
                                            CashRegisterName = payment.Name + " " + payment.Code,
                                            RptTitle = Resources.TellerHistDayOp,
                                            OpeningCashAmount = 0,
                                            ClosingCashAmount = 0,
                                            BranchName = currentBranch.BranchName,
                                            BranchAdress = currentBranch.Adress.Quarter.QuarterLabel + " - " + currentBranch.Adress.Quarter.Town.TownLabel,
                                            BranchTel = "Tel: " + currentBranch.Adress.AdressPhoneNumber + "PO Box :" + currentBranch.Adress.AdressPOBox,
                                            CompanyName = Company.Name,
                                            CompanyAdress = "Head Quater : " + Company.Adress.Quarter.Town.Region.RegionLabel + " - " + Company.Adress.Quarter.Town.TownLabel,
                                            CompanyTel = "Tel: " + Company.Adress.AdressPhoneNumber,
                                            CompanyCNI = "NO CONT : " + Company.CNI,
                                            CompanyLogo = _fileRepository.FindAll.FirstOrDefault(f => f.GlobalPersonID == Company.GlobalPersonID) != null ? _fileRepository.FindAll.FirstOrDefault(f => f.GlobalPersonID == Company.GlobalPersonID).Content : COMPANY_LOGO,
                                            PaymentMethod = payment.Code
                                        }
                            );
                        }
                    });

                dateopbnk = dateopbnk.AddDays(1);
            } while (dateopbnk <= At.Date);
           

            if (model.Count>0)
			{
				string path = Server.MapPath("~/Reports/CashRegister/RptCashHistOp.rpt");
				rptH.Load(path);
				rptH.SetDataSource(model);
                bool isDownloadRpt = (bool)Session["isDownloadRpt"];
				rptH.ExportToHttpResponse(ExportFormatType.PortableDocFormat, System.Web.HttpContext.Current.Response, isDownloadRpt, "RptCashHistOp");
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
		//This method print customer's bill
		//[HttpGet]
        [OutputCache(Duration = 3600)] 
		public ActionResult Bill()
		{
            try
            {

            //Ext.Net.MVC.PartialViewResult rPVResult = new Ext.Net.MVC.PartialViewResult
            //{
            //    ClearContainer = true,
            //    //RenderMode = RenderMode.AddTo,
            //    ContainerId = "Body",
            //    WrapByScriptTag = false
            //};
            //We test if this user has an autorization to get this sub menu
            //if (!LoadAction.isAutoriseToGetSubMenu(SessionProfileID, CodeValue.CashRegister.SUBMENU_STATE_CASH_BILL, db))
            //{
            //    RedirectToAction("NotAuthorized", "User");
            //}
            ViewBag.Disabled = true;
            List<BusinessDay> listBDUser  =(List<BusinessDay>)Session["UserBusDays"];
            if (listBDUser.Count() > 1)
            {
                ViewBag.Disabled = false;
            }
            BusinessDay busDays = listBDUser.FirstOrDefault();
            ViewBag.BusnessDayDate = busDays.BDDateOperation;
            Session["BusnessDayDate"] = busDays.BDDateOperation;
			
            return View();
            }

            catch (Exception e)
            {
                X.Msg.Alert("Error ", e.Message).Show();
                return this.Direct();
            }
		}
		[HttpPost]
		public ActionResult PrintBill(int CustomerID, DateTime BeginDate, DateTime EndDate)
		{
			Session["BeginDate"] = BeginDate;
			Session["EndDate"] = EndDate;
			Session["CustomerID"] = CustomerID;
			this.GetCmp<Panel>("RptBill").Hidden = false;
			this.GetCmp<Panel>("RptBill").LoadContent(new ComponentLoader
			{
				Url = Url.Action("GenerateNewBillReport"),
				DisableCaching = false,
				Mode = LoadMode.Frame
			});
			return this.Direct();
		}

        public void GenerateNewBillReport()
        {
            ReportDocument rptH = new ReportDocument();
            try
            {
                bool isValid = false;

                DateTime BeginDate = (DateTime)Session["BeginDate"];
                DateTime EndDate = (DateTime)Session["EndDate"];
                int customerID = (int)Session["CustomerID"];
                BusinessDay bsday = SessionBusinessDay(null);
                string BillNumber = _transactNumberRepository.displayTransactNumber("FACT", bsday);
                //string BillNumber = (string)Session["BillNumber"];

                Customer customer = (from cust in db.Customers
                                     where cust.GlobalPersonID == customerID
                                     select cust).SingleOrDefault();
                //verifions si cette facture existe deja
                Bill bill = db.Bills.FirstOrDefault(b => b.CustomerID==customerID && b.BeginDate==BeginDate.Date && b.EndDate==EndDate.Date);
                if (bill != null)
                {
                    Response.Write("La Facture du Client " + customer.CustomerFullName + " pour la periode du " + bill.BeginDate.Date + " Au " + bill.EndDate + " a déjà été généré.");
                }
                else
                {

                    List<object> model = new List<object>();

                    string defaultImgPath = Server.MapPath("~/Content/Images/App/deboy-logo.png");
                    byte[] COMPANY_LOGO = System.IO.File.ReadAllBytes(defaultImgPath);

                    model = this.ModelBillCusto(BeginDate, EndDate, customer, BillNumber);
                    if (model.Count > 0)
                    {
                        double TotalDepositPeriode = (double)Session["TotalDepositPeriode"];

                        double BalanceBefore = (double)Session["BalanceBefore"];
                        double TauxRemise = (double)Session["TauxRemise"];
                        double MontantRemise = (double)Session["MontantRemise"];
                        double TauxEscompte = (double)Session["TauxEscompte"];
                        double MontantEscompte = (double)Session["MontantEscompte"];
                        double Transport = (double)Session["Transport"];
                        double TauxTva = (double)Session["TauxTva"];
                        double ValeurTva = (double)Session["ValeurTva"];
                        double TotalTTC = (double)Session["TotalTTC"];
                        double TotalDepot = (double)Session["TotalDepositPeriode"];
                        double NetApayer = TotalTTC - TotalDepot;
                        List<BillDetail> BillDetails = (List<BillDetail>)Session["BillDetails"];

                        string path = Server.MapPath("~/Reports/CashRegister/RptBillDBoy.rpt");
                        rptH.Load(path);
                        rptH.SetDataSource(model);
                        rptH.SetParameterValue("BalanceBefore", BalanceBefore);
                        rptH.SetParameterValue("TotalDeposit", TotalDepositPeriode);
                        rptH.SetParameterValue("BeginDate", BeginDate);
                        rptH.SetParameterValue("EndDate", EndDate);
                        rptH.SetParameterValue("ServerDate", bsday.BDDateOperation);

                        bool isDownloadRpt = (bool)Session["isDownloadRpt"];
                        rptH.ExportToHttpResponse(ExportFormatType.PortableDocFormat, System.Web.HttpContext.Current.Response, isDownloadRpt, "RptBillDBoy");//  strReportName);

                        //persistance de la facture
                        FatSod.Supply.Entities.Bill SaveBill = new FatSod.Supply.Entities.Bill()
                        {
                            BillNumber = BillNumber,
                            CustomerID = customer.GlobalPersonID,
                            BeginDate = BeginDate,
                            BillDate = bsday.BDDateOperation,
                            EndDate = EndDate,
                            BalanceBefore = BalanceBefore,
                            //TauxRemise = TauxRemise,
                            MontantRemise = MontantRemise,
                            //TauxEscompte = TauxEscompte,
                            MontantEscompte = MontantEscompte,
                            Transport = Transport,
                            TauxTva = TauxTva,
                            //ValeurTva = ValeurTva,
                            //TotalTTC = TotalTTC,
                            TotalDepot = TotalDepositPeriode,
                            //NetApayer = NetApayer,
                            BillDetails = BillDetails,
                            IsNegoce = true
                        };
                        _billRepository.PersistBill(SaveBill, SessionGlobalPersonID, bsday.BranchID);
                        // Clear all sessions value
                        Session["CustomerID"] = null;
                        Session["BeginDate"] = null;
                        Session["EndDate"] = null;
                        Session["BillNumber"] = null;
                        Session["TotalHT"] = null;
                        Session["TauxRemise"] = null;
                        Session["MontantRemise"] = null;
                        Session["TauxEscompte"] = null;
                        Session["MontantEscompte"] = null;
                        Session["Transport"] = null;
                        Session["TauxTva"] = null;
                        Session["ValeurTva"] = null;
                        Session["TotalTTC"] = null;
                        Session["BillDetails"] = null;
                    }
                    else
                    {
                        Response.Write("Nothing Found; No Report name found");
                    }
                }

            }
            catch (Exception ex)
            {
                Response.Write(ex.ToString());
            }
            finally
            {
                rptH.Close();
                rptH.Dispose();
            }
        }
        private List<object> ModelBillCusto(DateTime BeginDate, DateTime EndDate, Customer customer, string BillNumber)
        {

            Session["BalanceBefore"] = null;
            Session["TauxRemise"] = null;
            Session["MontantRemise"] = null;
            Session["TauxEscompte"] = null;
            Session["MontantEscompte"] = null;
            Session["Transport"] = null;
            Session["TauxTva"] = null;
            Session["ValeurTva"] = null;
            Session["TotalTTC"] = null;
            Session["BillDetails"] = null;

            double BalanceBefore = 0d;
            /*
            double DebtBefore = _depositRepository.CustomerDebtStockLens(customer, BeginDate) + _depositRepository.CustomerDebtSpecOrder(customer, BeginDate);
            double SavingAmountBefore = _savingAccountRepository.GetSavingAccountBalance(customer, BeginDate);
            BalanceBefore = SavingAmountBefore - DebtBefore;
            */

            double totalDepositBefore = _depositRepository.TotalDepotSliceBefore(customer, BeginDate);
            double achatTTCBefore = _saleRepository.TotalAchatBefore(customer, BeginDate);
            BalanceBefore =  totalDepositBefore-achatTTCBefore;

            //depot sur la periode
            double TotalDepositPeriode = 0d;
            TotalDepositPeriode = _depositRepository.TotalDepotSlicePeriode(customer, BeginDate, EndDate);

            double TotalTTC = 0d;
            List<BillDetail> BillDetails = new List<BillDetail>();

            List<object> model = new List<object>();
            
            string defaultImgPath = Server.MapPath("~/Content/Images/App/deboy-logo.png");
            byte[] COMPANY_LOGO = System.IO.File.ReadAllBytes(defaultImgPath);

            Branch currentBranch = new Branch();
            Devise currentDevise = new Devise();

            double MontantRemise = 0d;
            double MontantEscompte = 0d;
            double Transport = 0d;
            double ValeurTva = 0d;
            //traitement historique pour la periode choisie
            SaleE getSalePeriode = new SaleE();
            //recuperation des ttes les ventes de la periode
            List<SaleE> salelist = db.Sales.Where(c => c.CustomerID == customer.GlobalPersonID && /*!c.IsPaid &&*/ (c.SaleDate >= BeginDate.Date && c.SaleDate <= EndDate.Date)).ToList();
            salelist.ForEach(allSales =>
            {
                

                currentBranch = allSales.Branch;
                currentDevise = allSales.Devise;
                //exclusion des retours
                getSalePeriode = _customerReturnRepository.GetRealSale(allSales);
                Session["TauxRemise"] = getSalePeriode.RateDiscount;
                Session["TauxEscompte"] = getSalePeriode.RateReduction;
                Transport = Transport + getSalePeriode.Transport;
                Session["TauxTva"] = getSalePeriode.VatRate;
                ValeurTva = ValeurTva + getSalePeriode.TVAAmount;
                /*
           //DEPOT DEJA EFFECTUER PR CETTE VENTE
           //Somme des dépôts direct sur achat
           List<CustomerSlice> lstCustSlicePeriode = db.CustomerSlices.Where(cs => cs.Sale.CustomerID == customer.GlobalPersonID &&
                                                                                        (cs.SliceDate >= BeginDate.Date && cs.SliceDate <= EndDate.Date) && cs.SaleID == getSalePeriode.SaleID).ToList();
                              
           double depotCustomerSlicePeriode = lstCustSlicePeriode != null ? lstCustSlicePeriode.Select(cs1 => cs1.SliceAmount).Sum() : 0;
           TotalDepositPeriode = TotalDepositPeriode + depotCustomerSlicePeriode;
                                        */
                //recuperation des lignes de vente

                List<SaleLine> slLine = getSalePeriode.SaleLines.ToList();
                slLine.ForEach(sl =>
                {
                    MontantEscompte = MontantEscompte + ((allSales.RateDiscount * sl.LineAmount) / 100);
                    MontantRemise = MontantRemise + ((allSales.RateReduction * sl.LineAmount) / 100);

                    BillDetails.Add(
                        new BillDetail
                        {
                            DateVente = allSales.SaleDate,
                            DateCommande = allSales.SaleDate ,
                            NumeroCommande = allSales.SaleReceiptNumber,
                            LineUnitPrice = sl.LineUnitPrice,
                            LineQuantity = sl.LineQuantity,
                            ProductID = sl.ProductID,
                            SaleID = sl.SaleID
                        }
                        );
                    //ajout des ventes ds la table des etats histo client
                    model.Add(
                          new
                          {
                              Title = Resources.Billreport,
                              Ref = getSalePeriode.SaleReceiptNumber,
                              BranchName = currentBranch.BranchName,
                              BranchAdress = currentBranch.Adress.AdressFullName,
                              BranchTel = currentBranch.Adress.AdressPhoneNumber,
                              CompanyName = Company.Name,
                              CompanyAdress = "Head Quater : " + Company.Adress.Quarter.Town.Region.RegionLabel + " - " + Company.Adress.Quarter.Town.TownLabel,
                              CompanyEmail = "Email: " + Company.Adress.AdressEmail,
                              CompanyTradeRegister = Company.CompanyTradeRegister,
                              CompanyTel = " Tel: " + Company.Adress.AdressPhoneNumber + " Fax: " + Company.Adress.AdressFax + " Cell: " + Company.Adress.AdressCellNumber,
                              CompanyCNI = "NO CONT : " + Company.CNI,
                              CompanyLogo = _fileRepository.FindAll.FirstOrDefault(f => f.GlobalPersonID == Company.GlobalPersonID) != null ? _fileRepository.FindAll.FirstOrDefault(f => f.GlobalPersonID == Company.GlobalPersonID).Content : COMPANY_LOGO,
                              SaleDate = getSalePeriode.SaleDate.Date,
                              RateRedution = getSalePeriode.RateReduction,
                              RateDiscount = getSalePeriode.RateDiscount,
                              Redution = MontantRemise,
                              Discount = MontantEscompte,
                              Transport = Transport,
                              ProductLabel = sl.Product.ProductLabel,
                              ProductRef = /*(sl.OeilDroiteGauche == EyeSide.N) ?*/ sl.Product.ProductCode /*: sl.OeilDroiteGauche + ":" + sl.Product.ProductCode*/,
                              //ProductRef = sl.Product.ProductCode,
                              LineUnitPrice = sl.LineUnitPrice,
                              LineQuantity = sl.LineQuantity,
                              CustomerName = customer.Name , //customer.Name,//+ " " + customer.Description,
                              CustomerAdress = "B.P: " + customer.AdressPOBox + " " + customer.Adress.Quarter.Town.TownLabel + " Tél:" + customer.AdressPhoneNumber,
                              CustomerAccount = customer.AccountNumber,
                              VatRate = getSalePeriode.VatRate,
                              NumeroCde = getSalePeriode.SaleID.ToString(),
                              DateCde = getSalePeriode.SaleDate.Date,
                              SaleID = getSalePeriode.SaleID,
                              BillNumber = BillNumber,
                              CompanyTown = currentBranch.Adress.Quarter.Town.TownLabel
                          }
             );

                }
                );
            }
        );

            Session["MontantRemise"] = MontantRemise;
            Session["MontantEscompte"] = MontantEscompte;
            Session["Transport"] = Transport;
            Session["ValeurTva"] = ValeurTva;

            Session["BalanceBefore"] = BalanceBefore;
            Session["TotalTTC"] = TotalTTC;
            Session["BillDetails"] = BillDetails;
            Session["TotalDepositPeriode"] = TotalDepositPeriode;
            return model;
        }
		public void GenerateBillReport()
		{
            List<object> model = new List<object>();
            ReportDocument rptH = new ReportDocument();
            bool isValid = false;
			try
			{
				
                
				DateTime BeginDate = (DateTime)Session["BeginDate"];
				DateTime EndDate = (DateTime)Session["EndDate"];

				int customerID = (int)Session["CustomerID"];
				Customer customer = (from cust in db.Customers where cust.GlobalPersonID==customerID
                                         select cust).SingleOrDefault();

				
				string defaultImgPath = Server.MapPath("~/Content/Images/App/deboy-logo.png");
				byte[] COMPANY_LOGO = System.IO.File.ReadAllBytes(defaultImgPath);
				double TotalReaming = 0;
				string deviseLabel = "";
				double purchaseTTC = 0;
				double TotalDepositPeriode = 0d;
				double TotalRemainingUnpaid = 0;
				double retAmountBefore = 0d;
				double retAmountPeriod = 0d;
				double depotEpargneAvant = 0d;

                double TotalDeposit = 0d;

				//generation du numero de la facture
				UserBranch Branch = db.UserBranches.Where(ub => ub.UserID == SessionGlobalPersonID).FirstOrDefault();
                int BranchID = Branch.BranchID;
				BusinessDay bsday = db.BusinessDays.Where(bd => bd.BranchID == BranchID && bd.BDStatut == true && bd.ClosingDayStarted == false).SingleOrDefault();

				string BillNumber = _transactNumberRepository.returnTransactNumber("FACT", bsday);

                //recuperation du depot d'epargne
                //List<Deposit> lstDepotEpargne = (from dep in db.Deposits
                //                                 where dep.SavingAccountID == customer.GlobalPersonID && dep.DepositDate < BeginDate.Date
                //                                 select dep).ToList();

                var lstDepotEpargne = db.Deposits.Join(db.SavingAccounts, dep => dep.SavingAccountID, sav => sav.ID,
                        (dep, sav) => new { dep, sav })
                        .Where(odep => odep.sav.CustomerID == customer.GlobalPersonID && odep.dep.DepositDate < BeginDate.Date)
                        .Select(s => new
                        {
                            Amount = s.dep.Amount
                        }).ToList();

                double oldAmtDepEpargne = (lstDepotEpargne==null || lstDepotEpargne.Count==0)? 0 : lstDepotEpargne.Select(d => d.Amount).Sum();
  
				//determiner le total des impayes avant la date de debut de la periode
                List<SaleE> RemainingUnpaid = (from remsal in db.Sales
                                               where remsal.CustomerID == customer.GlobalPersonID && remsal.SaleDate < BeginDate.Date
                                               select remsal).AsQueryable().ToList();
                    //_saleRepository.FindAll.Where(c => c.CustomerID == customer.GlobalPersonID && c.SaleDate < BeginDate).ToList();
				RemainingUnpaid.ForEach(unpaidSale =>
				{
					//recuperation des retours sur vente
                    SaleE getRemainOldSale = _customerReturnRepository.GetRealSale(unpaidSale);
                    double finalAmnt = getRemainOldSale.SaleLines.Select(l => l.LineAmount).Sum();
                    double FinalTransport = getRemainOldSale.Transport;

                    ExtraPrice extra = Util.ExtraPrices(finalAmnt, getRemainOldSale.RateReduction, getRemainOldSale.RateDiscount, FinalTransport, getRemainOldSale.VatRate);
                    double RemainingUnpaidAmt = extra.TotalTTC - getRemainOldSale.CustomerSlices.Select(l => l.SliceAmount).Sum() ;
					TotalRemainingUnpaid += RemainingUnpaidAmt;
                    //isValid = true;
				});
                TotalRemainingUnpaid = TotalRemainingUnpaid - oldAmtDepEpargne;
                //recuperation du depot d'epargne
                //List<Deposit> lstNewDepotEpargne = (from dep in db.Deposits
                //                                 where dep.SavingAccountID == customer.GlobalPersonID && (dep.DepositDate >= BeginDate.Date && dep.DepositDate<=EndDate.Date)
                //                                 select dep).ToList();
                var lstNewDepotEpargne = db.Deposits.Join(db.SavingAccounts, dep => dep.SavingAccountID, sav => sav.ID,
                        (dep, sav) => new { dep, sav })
                        .Where(odep => odep.sav.CustomerID == customer.GlobalPersonID && (odep.dep.DepositDate >= BeginDate.Date && odep.dep.DepositDate <= EndDate.Date))
                        .Select(s => new
                        {
                            Amount = s.dep.Amount
                        }).ToList();
                double newAmtDepEpargne = (lstNewDepotEpargne == null || lstNewDepotEpargne.Count == 0) ? 0 : lstNewDepotEpargne.Select(d => d.Amount).Sum();

                //TotalDeposit = newAmtDepEpargne;

				int saleNumber = 0;
				//we take her unpaid sales
				List<SaleE> salelist = _depositRepository.CustomerAllPeriodSales(customer, BeginDate, EndDate);
				//List<SaleE> salelist = _saleRepository.FindAll.Where(c => c.CustomerID == customer.GlobalPersonID && (c.SaleDate >= BeginDate && c.SaleDate <= EndDate)).ToList();
				salelist.ForEach(unpaidSale =>
				{

                   
                    SaleE getRemainSale = _customerReturnRepository.GetRealSale(unpaidSale);
                    double FinalSaleAmnt = getRemainSale.SaleLines.Select(l => l.LineAmount).Sum();
                    double FinalTransport = getRemainSale.Transport;

                    ExtraPrice extra = Util.ExtraPrices(FinalSaleAmnt, getRemainSale.RateReduction, getRemainSale.RateDiscount, FinalTransport, getRemainSale.VatRate);
                    double unpaidSlice = getRemainSale.CustomerSlices.Select(l => l.SliceAmount).Sum();
					double currentSaleRemainder = (unpaidSlice > 0) ? extra.TotalTTC - unpaidSlice : extra.TotalTTC;

					TotalReaming += currentSaleRemainder;
					purchaseTTC += extra.TotalTTC;
					deviseLabel = _deviseRepository.Find(unpaidSale.DeviseID).DeviseLabel;

					saleNumber = saleNumber + 1;
                    TotalDeposit = TotalDeposit + unpaidSlice;
					//we take all her salelines
                    getRemainSale.SaleLines.ToList().ForEach(uL =>
				{
					//Branch currentBranch = _branchRepository.Find(unpaidSale.BranchID);
                    
					//double ReturnLineQuantity = lstFinalSaleLIne.Where(p => p.ProductID == uL.ProductID).Select(fsl => fsl.LineQuantity).Sum();
					Branch currentBranch = unpaidSale.Branch;
					model.Add(
								new
								{
									Title = Resources.Billreport,// "BILL FOR UNPAID SALES",
                                    SaleDate = getRemainSale.SaleDate,
                                    ProductRef = uL.Product.ProductCode,//(uL.OeilDroiteGauche == EyeSide.N) ? uL.Product.ProductCode : uL.OeilDroiteGauche + ":" + uL.Product.ProductCode,
									DepositAmount = unpaidSlice,//deposit amount for sale --currentSaleRemainder,
									RptBillID = saleNumber,//currentSaleRemainder, //reste du montant sur un achat
									ProductLabel = uL.ProductLabel,
									LineQuantity = uL.LineQuantity, //- ReturnLineQuantity,
									LineUnitPrice = uL.LineUnitPrice,
                                    Ref = getRemainSale.SaleReceiptNumber.Trim(),//.Patient,//,
                                    SaleID = getRemainSale.SaleID,
                                    RateRedution = getRemainSale.RateReduction,
                                    Redution = (getRemainSale.RateReduction * ((uL.LineQuantity /*- ReturnLineQuantity*/) * uL.LineUnitPrice)) / 100,
                                    RateDiscount = getRemainSale.RateDiscount,
                                    Discount = (getRemainSale.RateDiscount * ((uL.LineQuantity /*- ReturnLineQuantity*/) * uL.LineUnitPrice)) / 100,
									Transport = FinalTransport,//unpaidSale.Transport,
									CustomerName = customer.Name,// + " " + customer.Description,
									CustomerAdress = customer.AdressEmail,
									CustomerAccount = customer.CNI,
									BranchName = currentBranch.BranchName,
									BranchAdress = (currentBranch.Adress.AdressFullName != null || currentBranch.Adress.AdressFullName != "") ? currentBranch.Adress.AdressFullName : currentBranch.Adress.Quarter.Town.Region.Country.CountryLabel + " B.P." + currentBranch.Adress.AdressPOBox + " - " + currentBranch.Adress.Quarter.Town.TownLabel,//currentBranch.Adress.AdressEmail + "/" + currentBranch.Adress.AdressPOBox,
                                    BranchTel = " Tel: " + currentBranch.Adress.AdressPhoneNumber,
									CompanyName = Company.Name,
                                    CompanyAdress = "Head Quater : " + Company.Adress.Quarter.Town.Region.RegionLabel + " - " + Company.Adress.Quarter.Town.TownLabel,
									CompanyEmail = "Email: " + Company.Adress.AdressEmail,
									CompanyTradeRegister = Company.CompanyTradeRegister,
									CompanyTel = " Tel: " + Company.Adress.AdressPhoneNumber + " Fax: " + Company.Adress.AdressFax + " Cell: " + Company.Adress.AdressCellNumber,
                                    CompanyCNI = "NO CONT : " + Company.CNI,
									CompanyLogo = _fileRepository.FindAll.FirstOrDefault(f => f.GlobalPersonID == Company.GlobalPersonID) != null ? _fileRepository.FindAll.FirstOrDefault(f => f.GlobalPersonID == Company.GlobalPersonID).Content : COMPANY_LOGO,
                                    VatRate = getRemainSale.VatRate,
									TotalRemainingUnpaid = TotalRemainingUnpaid,
                                    CompanyTown = currentBranch.Adress.Quarter.Town.TownLabel,
									BillNumber = BillNumber
								}
						);
				}
			 );
					isValid = true;
					//}


				}
		);
				//if (TotalDepositPeriode < 0) TotalReaming = TotalReaming + (-1 * TotalDepositPeriode);
                TotalDeposit = TotalDeposit + newAmtDepEpargne;
				if (isValid)
				{
					string path = Server.MapPath("~/Reports/CashRegister/RptBillDBoy.rpt");
					rptH.Load(path);
					rptH.SetDataSource(model);
					rptH.SetParameterValue("PurchaseTTC", purchaseTTC);
					rptH.SetParameterValue("TotalRemaingAmountParam", TotalReaming);
                    rptH.SetParameterValue("TotalDeposit", TotalDeposit);
					rptH.SetParameterValue("DeviseLabel", deviseLabel);
					rptH.SetParameterValue("BeginDate", BeginDate);
					rptH.SetParameterValue("EndDate", EndDate);
                    bool isDownloadRpt = (bool)Session["isDownloadRpt"];
                    rptH.ExportToHttpResponse(ExportFormatType.PortableDocFormat, System.Web.HttpContext.Current.Response, isDownloadRpt, "RptBillDBoy");//  strReportName);

					// Clear all sessions value
					Session["CustomerID"] = null;
				}
				else
				{
					Response.Write("Nothing Found; No Report name found");
				}
			}
			catch (Exception ex)
			{
				Response.Write(ex.ToString());
			}
            finally
            {
                rptH.Close();
                rptH.Dispose();
            }
		}
		public void GenerateSaleBill()
		{
			ReportDocument rptH = new ReportDocument();
			try
			{
				bool isValid = false;

				//we take current sale in session
				int saleID = (int)Session["SaleID"];
				SaleE currentSale = _saleRepository.Find(saleID);

				Customer customer = (Customer)_personRepository.Find(currentSale.CustomerID.Value);
				List<object> model = new List<object>();
				string defaultImgPath = Server.MapPath("~/Content/Images/App/deboy-logo.png");
				byte[] COMPANY_LOGO = System.IO.File.ReadAllBytes(defaultImgPath);
				double TotalReaming = 0;
				string deviseLabel = "";
				double purchaseTTC = 0;

				double saleAmnt = currentSale.SaleLines.Select(l => l.LineAmount).Sum();
				ExtraPrice extra = Util.ExtraPrices(saleAmnt, currentSale.RateReduction, currentSale.RateDiscount, currentSale.Transport, currentSale.VatRate);
				double currentSaleRemainder = extra.TotalTTC - currentSale.CustomerSlices.Select(l => l.SliceAmount).Sum();

				TotalReaming += currentSaleRemainder;
				purchaseTTC += extra.TotalTTC;
				deviseLabel = _deviseRepository.Find(currentSale.DeviseID).DeviseLabel;


				//we take all her salelines
				currentSale.SaleLines.ToList().ForEach(uL =>
				{
					//Branch currentBranch = _branchRepository.Find(unpaidSale.BranchID);
					Branch currentBranch = currentSale.Branch;
					model.Add(
								new
								{
									Title = "BILL FOR UNPAID SALES",
									SaleDate = currentSale.SaleDate,
									SaleDateTime = currentSale.SaleDate,
                                    ProductRef = uL.Product.ProductCode,//(uL.OeilDroiteGauche == EyeSide.N) ? uL.Product.ProductCode : uL.Product.ProductCode + " -" + Resources.EyeSide + ":" + uL.OeilDroiteGauche,
									RptBillID = currentSale.RemaingAmount,
									ProductLabel = uL.ProductLabel,
									LineQuantity = uL.LineQuantity,
									LineUnitPrice = uL.LineUnitPrice,
									Ref = currentSale.SaleReceiptNumber,
									SaleID = currentSale.SaleID,
									RateRedution = currentSale.RateReduction,
									RateDiscount = currentSale.RateDiscount,
									Transport = currentSale.Transport,
									CustomerName = customer.Name + " " + customer.Description,
									CustomerAdress = customer.AdressEmail,
									CustomerAccount = customer.AccountNumber,
									BranchName = currentBranch.BranchName,
									BranchAdress = currentBranch.Adress.AdressEmail + "/" + currentBranch.Adress.AdressPOBox,
                                    BranchTel = " Tel: " + currentBranch.Adress.AdressPhoneNumber,
									CompanyName = Company.Name,
                                    CompanyAdress = "Head Quater : " + Company.Adress.Quarter.Town.Region.RegionLabel + " - " + Company.Adress.Quarter.Town.TownLabel,
                                    CompanyTel = " Tel: " + Company.Adress.AdressPhoneNumber,
                                    CompanyCNI = "NO CONT : " + Company.CNI,
									CompanyLogo = _fileRepository.FindAll.FirstOrDefault(f => f.GlobalPersonID == Company.GlobalPersonID) != null ? _fileRepository.FindAll.FirstOrDefault(f => f.GlobalPersonID == Company.GlobalPersonID).Content : COMPANY_LOGO,
									VatRate = currentSale.VatRate,
									TotalRemainingUnpaid = 0
								}
						);
				}
		 );
				isValid = true;

				if (isValid)
				{
					string path = Server.MapPath("~/Reports/CashRegister/RptBill.rpt");
					rptH.Load(path);
					rptH.SetDataSource(model);
					rptH.SetParameterValue("PurchaseTTC", purchaseTTC);
					rptH.SetParameterValue("TotalRemaingAmountParam", TotalReaming);
					rptH.SetParameterValue("DeviseLabel", deviseLabel);
					rptH.SetParameterValue("BeginDate", currentSale.SaleDate);
					rptH.SetParameterValue("EndDate", currentSale.SaleDate);
                    bool isDownloadRpt = (bool)Session["isDownloadRpt"];
					rptH.ExportToHttpResponse(ExportFormatType.PortableDocFormat, System.Web.HttpContext.Current.Response, isDownloadRpt, "RptBill");

				}
				else
				{
					Response.Write("Nothing Found; No Report name found");
				}

			}
			catch (Exception ex)
			{
				Response.Write(ex.ToString());
			}
            finally
            {
                rptH.Close();
                rptH.Dispose();
            }
		}
		private Company Company
		{
			get
			{
				return _companyRepository.FindAll.FirstOrDefault();
			}
		}
		//============== Method for customers's bill
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
		public StoreResult LoadThirdPartyAccounts(int? BranchID)
		{
			List<object> customers = new List<object>();
			List<Customer> customers1 = new List<Customer>();
			//customers1 = _depositRepository.CustomersDebtors(_branchRepository.Find(BranchID.Value));
			customers1 = _customerRepository.FindAll.ToList();
			foreach (Customer c in customers1)
			{
				//if (_depositRepository.CustomerDebt(c) > 0)
				//{
				customers.Add(
						new
						{
							CustomerFullName = c.CustomerFullName,
							PersonID = c.GlobalPersonID
						}
				);
				//}

			}
			return this.Store(customers);
		}



        //Test de generatioon direct de l'impression
        public ActionResult btnPrintReciept_Click(object sender, EventArgs e)
        {


            PrintDocument printDocument = new PrintDocument();





            try
            {
                //printDocument.PrinterSettings.PrinterName = GetDefaultPrinterName();
             
                    //PrintingPermission perm = new PrintingPermission(System.Security.Permissions.PermissionState.Unrestricted);
                    //perm.Level = PrintingPermissionLevel.AllPrinting;
                    printDocument.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(CreateReceipt); //add an event handler that will do the printing
                    //PrinterSettings ps = new PrinterSettings();
                    

                    printDocument.Print();

                X.Msg.Alert("Success ", "Default Printer is : " + printDocument.PrinterSettings.PrinterName
                    

                    ).Show();

            }
            catch (Exception ex)
            {

                X.Msg.Alert("Error ", ex.Message + ex.StackTrace
                            
                    ).Show();
            }



            return this.Direct();

            // }
        }

        public string GetDefaultPrinter()
        {
            PrinterSettings settings = new PrinterSettings();

            string defaultPrinter = "";

            foreach (string printer in PrinterSettings.InstalledPrinters)
            {
                settings.PrinterName = printer;

                if (settings.IsDefaultPrinter)

                    defaultPrinter = printer;

            }

            return defaultPrinter;

        }

        public PrinterSettings GetDefaultPrinterSettings()
        {
            PrinterSettings settings = new PrinterSettings();

            string defaultPrinter = "";

            //PrinterSettings.InstalledPrinters

            foreach (string printer in PrinterSettings.InstalledPrinters)
            {
                settings.PrinterName = printer;

                if (settings.PrinterName.Contains("P1102"))

                    return settings;

            }

            return settings;

        }


        public string GetAllPrinters()
        {
            PrinterSettings settings = new PrinterSettings();

            string printers = "";
            int i = 0;
            foreach (string printer in PrinterSettings.InstalledPrinters)
            {
                printers += ++i + "-" + printer + " ";
            }

            return printers;

        }

        public void CreateReceipt(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {

            int total = 0;
            float cash = 200000;
            float change = 0.00f;

            //this prints the reciept

            Graphics graphic = e.Graphics;

            Font font = new Font("Courier New", 12); //must use a mono spaced font as the spaces need to line up

            float fontHeight = font.GetHeight();

            int startX = 10;
            int startY = 10;
            int offset = 40;

            graphic.DrawString(" Wangoland Coffee Shop", new Font("Courier New", 18), new SolidBrush(Color.Black), startX, startY);
            string top = "Item Name".PadRight(30) + "Price";
            graphic.DrawString(top, font, new SolidBrush(Color.Black), startX, startY + offset);
            offset = offset + (int)fontHeight; //make the spacing consistent
            graphic.DrawString("----------------------------------", font, new SolidBrush(Color.Black), startX, startY + offset);
            offset = offset + (int)fontHeight + 5; //make the spacing consistent

            float totalprice = 0.00f;

            List<string> listBox1 = new List<string>() { "Banane", "Manioc", "Ignanme" };

            foreach (string item in listBox1)
            {
                //create the string to print on the reciept
                string productDescription = item;
                string productTotal = item.Substring(item.Length - 6, 6);
                //float productPrice = float.Parse(item.Substring(item.Length - 5, 5));

                //MessageBox.Show(item.Substring(item.Length - 5, 5) + "PROD TOTAL: " + productTotal);


                //totalprice += productPrice;

                if (productDescription.Contains("  -"))
                {
                    string productLine = productDescription.Substring(0, 24);

                    graphic.DrawString(productLine, new Font("Courier New", 12, FontStyle.Italic), new SolidBrush(Color.Red), startX, startY + offset);

                    offset = offset + (int)fontHeight + 5; //make the spacing consistent
                }
                else
                {
                    string productLine = productDescription;

                    graphic.DrawString(productLine, font, new SolidBrush(Color.Black), startX, startY + offset);

                    offset = offset + (int)fontHeight + 5; //make the spacing consistent
                }

            }

            change = (cash - totalprice);

            //when we have drawn all of the items add the total

            offset = offset + 20; //make some room so that the total stands out.

            graphic.DrawString("Total to pay ".PadRight(30) + String.Format("{0:c}", totalprice), new Font("Courier New", 12, FontStyle.Bold), new SolidBrush(Color.Black), startX, startY + offset);

            offset = offset + 30; //make some room so that the total stands out.
            graphic.DrawString("CASH ".PadRight(30) + String.Format("{0:c}", cash), font, new SolidBrush(Color.Black), startX, startY + offset);
            offset = offset + 15;
            graphic.DrawString("CHANGE ".PadRight(30) + String.Format("{0:c}", change), font, new SolidBrush(Color.Black), startX, startY + offset);
            offset = offset + 30; //make some room so that the total stands out.
            graphic.DrawString("     Thank-you for your custom,", font, new SolidBrush(Color.Black), startX, startY + offset);
            offset = offset + 15;
            graphic.DrawString("       please come back soon!", font, new SolidBrush(Color.Black), startX, startY + offset);

        }

        public static string GetDefaultPrinterName()
        { //http://stackoverflow.com/questions/7809992/printersettings-installedprinters-not-getting-network-printers
            //PrintingPermission perm = new PrintingPermission(System.Security.Permissions.PermissionState.Unrestricted);
            //perm.Level = PrintingPermissionLevel.AllPrinting;

            var query = new ObjectQuery("SELECT * FROM Win32_Printer");
            var searcher = new ManagementObjectSearcher(query);

            foreach (ManagementObject mo in searcher.Get())
            {
                if (((bool?)mo["Default"]) ?? false)
                {
                    //Win32_Printer pt = (Win32_Printer)mo;
                    return mo["Name"] as string;
                }
            }

            return null;
        }

        private string LocalPrinterList()
        {
            // USING WMI. (WINDOWS MANAGEMENT INSTRUMENTATION)
            string printers = "";
            int i = 0;

            System.Management.ManagementScope objMS =
                new System.Management.ManagementScope(ManagementPath.DefaultPath);
            objMS.Connect();

            SelectQuery objQuery = new SelectQuery("SELECT * FROM Win32_Printer");
            ManagementObjectSearcher objMOS = new ManagementObjectSearcher(objMS, objQuery);
            System.Management.ManagementObjectCollection objMOC = objMOS.Get();

            foreach (ManagementObject Printers in objMOC)
            {
                if (Convert.ToBoolean(Printers["Local"]))       // LOCAL PRINTERS.
                {
                    printers += ++i + "-" + Printers["Name"] + " ";
                }
                if (Convert.ToBoolean(Printers["Network"]))     // ALL NETWORK PRINTERS.
                {
                    printers += ++i + "-" + Printers["Name"] + " ";
                }
            }

            return printers;

        }

        private string NetworkPrinterList()
        {
            // USING WMI. (WINDOWS MANAGEMENT INSTRUMENTATION)
            string printers = "";
            int i = 0;

            System.Management.ManagementScope objMS =
                new System.Management.ManagementScope(ManagementPath.DefaultPath);
            objMS.Connect();

            SelectQuery objQuery = new SelectQuery("SELECT * FROM Win32_Printer");
            ManagementObjectSearcher objMOS = new ManagementObjectSearcher(objMS, objQuery);
            System.Management.ManagementObjectCollection objMOC = objMOS.Get();

            foreach (ManagementObject Printers in objMOC)
            {
                /* if (Convert.ToBoolean(Printers["Local"]))       // LOCAL PRINTERS.
                 {
                     printers += ++i + "-" + Printers["Name"] + " ";
                 }
                 if (Convert.ToBoolean(Printers["Network"]))     // ALL NETWORK PRINTERS.
                 {*/
                printers += ++i + "-" + Printers["Name"] + " ";
                //}
            }

            return printers;

        }



        public string GetPrinter()
        {
            string defaultPrinter;
            using (var printServer = new LocalPrintServer())
            {
                defaultPrinter = printServer.DefaultPrintQueue.FullName;
            }

            return defaultPrinter;
        }


        public ActionResult ImprimerEtiquettes(/*int patientId, int venueId, string uf*/)
        {
            //do some business logic to have a Stream with parameters

            //using (StreamReader sr = new StreamReader(fStream))
            //{
                var doc = new PrintDocument();
                doc.PrinterSettings.PrinterName = GetDefaultPrinter();
                System.Drawing.Printing.PaperSource ps = doc.PrinterSettings.PaperSources[2];
                doc.DefaultPageSettings.PaperSource = ps;
                doc.PrintPage += doc_PrintPage;
                doc.Print();
            //}

            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        void doc_PrintPage(object sender, PrintPageEventArgs e)
        {

        }







	}

}