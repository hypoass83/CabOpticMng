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
using FastSod.Utilities.Util;

using FatSod.Budget.Entities;


//Pour l'impression
using System.Data;

using FatSod.Report.WrapReports;
using System.Data.SqlClient;
using System.IO;

namespace CABOPMANAGEMENT.Areas.CashRegister.Controllers
{
    [Authorize(Order = 1)]
	[TakeBusinessDay(Order = 2)]
	public class StateController : BaseController
	{
		private const string CONTROLLER_NAME = "CashRegister/State";
		//*********************
		private IPerson _personRepository;
		private IRepositorySupply<CustomerOrderLine> _customerOrderLineRepository;
		private IRepositorySupply<CustomerOrder> _customerOrderRepository;
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
				IRepositorySupply<CustomerOrderLine> customerOrderLineRepository,
				ITillDay tillDayRepository,
				ISale saleRepository,
                IBill billRepository,
                ISavingAccount SavingAccountRepo,
				IRepositorySupply<Purchase> purchaselRepository,
				IRepositorySupply<UserTill> userTillRepository,
				IRepository<Branch> branchRepository,
				IRepositorySupply<CustomerOrder> customerOrderRepository,
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
			this._customerOrderLineRepository = customerOrderLineRepository;
			this._customerOrderRepository = customerOrderRepository;
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
            ViewBag.DisplayForm = 1;
            try
            {

                ViewBag.Disabled = true;

                List<BusinessDay> listBDUser =(List<BusinessDay>)Session["UserBusDays"];
                if (listBDUser == null)
                {
                    listBDUser = _busDayRepo.GetOpenedBusinessDay(CurrentUser);
                }
                if (listBDUser.Count() > 1)
                {
                    ViewBag.Disabled = false;
                }
                DateTime busDays = listBDUser.FirstOrDefault().BDDateOperation;
                ViewBag.CurrentBranch = listBDUser.FirstOrDefault().BranchID;
                ViewBag.BusnessDayDate = busDays.ToString("yyyy-MM-dd");
                Session["BusnessDayDate"] = busDays;

                return View();
            }

            catch (Exception e)
            {
                TempData["Message"] = "Error " + e.Message;
                ViewBag.DisplayForm = 0;
                return this.View();
            }
		}

        [OutputCache(Duration = 3600)]
        public ActionResult BankHistOperation()
        {
            ViewBag.DisplayForm = 1;
            try
            {

                ViewBag.Disabled = true;

                List<BusinessDay> listBDUser = (List<BusinessDay>)Session["UserBusDays"];
                if (listBDUser == null)
                {
                    listBDUser = _busDayRepo.GetOpenedBusinessDay(CurrentUser);
                }
                if (listBDUser.Count() > 1)
                {
                    ViewBag.Disabled = false;
                }
                DateTime busDays = listBDUser.FirstOrDefault().BDDateOperation;
                ViewBag.CurrentBranch = listBDUser.FirstOrDefault().BranchID;
                ViewBag.BusnessDayDate = busDays.ToString("yyyy-MM-dd");
                Session["BusnessDayDate"] = busDays;

                return View();
            }

            catch (Exception e)
            {
                TempData["Message"] = "Error " + e.Message;
                ViewBag.DisplayForm = 0;
                return this.View();
            }
        }
        //This method return a list of tills of one branch
        public JsonResult GetTillOfBanch(int BranchID)
        {
            //int i = 0;
            //List<Till> model = new List<Till>();
            //UserTill userTill = db.UserTills.FirstOrDefault(td => td.UserID == SessionGlobalPersonID);
            //if (userTill == null)
            //{
            //    db.Tills.Where(t => t.BranchID == BranchID).ToList().ForEach(t =>
            //    {
            //        i++;
            //        model.Add(new Till { Name = t.Name, ID = t.ID });
            //    }
            //  );
            //}
            //else
            //{
            //    i++;
            //    model.Add(new Till { Name = userTill.Till.Name, ID = userTill.Till.ID });
            //}

            int i = 0;
            List<Till> model = new List<Till>();
            /*List<UserTill> userTill = db.UserTills.Where(td => td.HasAccess && td.USer.IsConnected && td.USer.UserAccountState).ToList();
            foreach (UserTill ustill in userTill)
            {*/
                db.Tills.Where(t => t.BranchID == BranchID /*&& t.ID == ustill.TillID*/).ToList().ForEach(t =>
                {
                    i++;
                    model.Add(new Till { Name = t.Name, ID = t.ID });
                }
              );
            //}

            return Json(model, JsonRequestBehavior.AllowGet);
        }
        /*** methodes lors du chargement du formulaire*********************/
        public JsonResult GetOpenedBranches()
        {

            List<object> openedBranchesList = new List<object>();
            List<Branch> openedBranches = _busDayRepo.GetOpenedBranches();
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
        //[OutputCache(Duration = 3600)]
        public ActionResult DisplayFacture()
        {
            ViewBag.DisplayForm = 1;
            try
            {

                ViewBag.Disabled = true;

                List<BusinessDay> listBDUser = (List<BusinessDay>)Session["UserBusDays"];
                if (listBDUser.Count() > 1)
                {
                    ViewBag.Disabled = false;
                }
                DateTime busDays = listBDUser.FirstOrDefault().BDDateOperation;
                ViewBag.BusnessDayDate = busDays.ToString("yyyy-MM-dd");
                Session["BusnessDayDate"] = busDays;

                return View(ModelReturnBill(busDays));
            }

            catch (Exception e)
            {
                TempData["Message"] = "Error " + e.Message;
                ViewBag.DisplayForm = 0;
                return this.View();
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
        //[HttpPost]
        public ActionResult InitializeFields(int BillID)
        {
            List<object> _InitList = new List<object>();

            //we take sale and her salelines
            Bill selectedBill = db.Bills.Find(BillID);
            
            _InitList.Add(new
            {
                BillID = selectedBill.BillID,
                CustomerID = selectedBill.CustomerID,
                CustomerName = selectedBill.Customer.Name,
                BillNumber = selectedBill.BillNumber,
                BillDate = selectedBill.BillDate,
                BeginDate = selectedBill.BeginDate,
                EndDate = selectedBill.EndDate

            });

            return Json(_InitList, JsonRequestBehavior.AllowGet);
        }
        //[HttpPost]
       /* public StoreResult ReturnAbleBill(DateTime BillDate)
        {
            return this.Store(ModelReturnBill(BillDate));
        }*/
       /* 
        public ActionResult ReloadBillListStore()
        {
            this.GetCmp<Store>("BillListStore").Reload();
            this.GetCmp<FormPanel>("FormBillIdentification").Reset();
            return this.Direct();
        }*/
        //[HttpPost]
        /*public ActionResult PrintAfficheBill(DateTime BeginDate, DateTime EndDate, string BillNumber)
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
        }*/
        public void AfficheGenerateBillReport()
        {
            
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

                    /*string path = Server.MapPath("~/Reports/CashRegister/RptBillDBoy.rpt");
                    rptH.Load(path);
                    rptH.SetDataSource(model);
                    rptH.SetParameterValue("BalanceBefore", BalanceBefore);
                    rptH.SetParameterValue("TotalDeposit", TotalDepositPeriode);
                    rptH.SetParameterValue("BeginDate", BeginDate);
                    rptH.SetParameterValue("EndDate", EndDate);
                    rptH.SetParameterValue("ServerDate", SessionBusinessDay(null).BDDateOperation);

                    bool isDownloadRpt = (bool)Session["isDownloadRpt"];
                    rptH.ExportToHttpResponse(ExportFormatType.PortableDocFormat, System.Web.HttpContext.Current.Response, isDownloadRpt, "RptBillDBoy");*/
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
                /*this.GetCmp<Store>("BillListStore").Reload();
                this.GetCmp<FormPanel>("FormBillIdentification").Reset();*/
                return this.View();
            }
            catch (Exception e)
            {
                TempData["Message"] = "Error "+ e.Message;
                return this.View();
            }
        }
		//Generate a state of cash register operations in time interval
		/*public ActionResult GenerateState(DateTime BeginDate, DateTime EndDate, int BranchID,int TillID, string GeneratedType)
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
		}*/
		///// <summary>
		///// Generate list of cash operation in time internal in pdf format
		///// </summary>
		///// <returns>File</returns>
		//public ActionResult PdfType()
		//{
		//	List<object> model = new List<object>();
		//	DateTime To = (DateTime)Session["BeginDate"];
		//	DateTime At = (DateTime)Session["EndDate"];
		//	int tillID = (int)Session["TillID"];
		//	int branchID = (int)Session["BranchID"];

		//	this.GenerateReportOperationPeriodeOfDay(To, At, tillID);

		//	return this.View();

		//}
		///// <summary>
		///// Generate list of cash operation in time internal in Grid format
		///// </summary>
		///// <returns>Grid</returns>
		//public ActionResult GridType()
		//{
		//	DateTime To = (DateTime)Session["BeginDate"];
		//	DateTime At = (DateTime)Session["EndDate"];
		//	int tillID = (int)Session["TillID"];
		//	ViewBag.ListSalesDay = ModelSaleDay(To, At, tillID);
		//	ViewBag.ListPurchasseDay = ModelPurchaseDay(To, At, tillID);
		//	return View("RenderState");
		//}

		//This method load a method that print a receip of sale

		/*public ActionResult LoadCMP()
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
		}*/
       /* public ActionResult ShowGeneric()
        {
            return RedirectToAction("GenerateReceipt");
        }*/
       /* public ActionResult PrintSaleDeliveryOrder(string exportFormat)
		{
            int SaleID = (int)Session["SaleID"];
            
            Session["path"] = Server.MapPath("~/Reports/CashRegister/RptDeliveryOrder.rpt");
            Session["rptname"] = "RptDeliveryOrder";
            Session["exportFormat"] = exportFormat;
			//this.LoadCMP();
			return this.View();
		}*/
		
        /*
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
		}*/

       /* public ActionResult PrintSaleReceipt(string exportFormat)
		{
            int SaleID = (int)Session["SaleID"];
            Session["path"] = Server.MapPath("~/Reports/CashRegister/RptReceipt.rpt");
            Session["rptname"] = "RptReceipt";
            Session["exportFormat"] = exportFormat;
			//this.LoadCMP();
			return this.View();
		}*/

		//This method print a receipt of customer
        public ActionResult GenerateReceipt(bool isDeliverOrder)
		{
            //bool isValid = false;
			//ReportDocument rptH = new ReportDocument();
            try
            {
                Session["path"] = (isDeliverOrder) ? Server.MapPath("~/Reports/CashRegister/RptDeliveryOrder.rpt") : Server.MapPath("~/Reports/CashRegister/RptReceipt.rpt");
                string path = (string)Session["path"];
               int saleID = (int)Session["SaleID"];
               //string exportFormat = (string)Session["exportFormat"];
               double receiveAmoung = (double)Session["SliceAmount"];

               string QtyDetail = "";
               List<object> model = new List<object>();

               string defaultImgPath = Server.MapPath("~/Content/Images/App/deboy-logo.png");
               byte[] COMPANY_LOGO = System.IO.File.ReadAllBytes(defaultImgPath);
               SaleE currentSale = (from sal in db.Sales
                                    where sal.SaleID == saleID
                                    select sal).SingleOrDefault();
               
               /*Customer customer = (from cust in db.Customers
                                    where cust.GlobalPersonID == currentSale.CustomerID
                                    select cust).SingleOrDefault();*/
               
               double saleAmount = 0d;
               //_lineRepository.FindAll.OfType<SaleLine>().Where(l => l.SaleID == saleID).ToList().ForEach(c =>
               db.SaleLines.Where(l => l.SaleID == saleID).ToList().ForEach(c =>
               {
                   //isValid = true;
                   saleAmount += c.LineAmount;
                   QtyDetail = "";
                  
                   
                   model.Add(
                                   new
                                   {
                                       ReceiveAmount = receiveAmoung,
                                       LineQuantity =  c.LineQuantity,
                                       NewLineQty =  c.LineQuantity ,
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
                                       CustomerName = currentSale.Customer.CustomerFullName,// customer.Name/* + (LoadComponent.IsGeneralPublic(customer) ? (" / " + currentSale.Representant) : "")*/,
                                       CustomerAccount = currentSale.CustomerName,// customer.CustomerNumber,
                                       SaleDate = currentSale.SaleDeliveryDate,
                                       Title = currentSale.CustomerName/*.Substring(1,currentSale.Representant.Length)*/,
                                       DeviseLabel = QtyDetail,  //currentSale.Devise.DeviseLabel,
                                       CustomerAdress = "Email:" + currentSale.Branch.Adress.AdressEmail,
                                       CompanyLogo = _fileRepository.FindAll.FirstOrDefault(f => f.GlobalPersonID == Company.GlobalPersonID) != null ? _fileRepository.FindAll.FirstOrDefault(f => f.GlobalPersonID == Company.GlobalPersonID).Content : COMPANY_LOGO
                                   }
                           );
               }
                       );

               

                Response.Buffer = false;
                Response.ClearContent();
                Response.ClearHeaders();

                return View(model);
            }
            catch (Exception ex)
            {
                throw;
            }
            //finally
            //{
            //    rptH.Close();
            //    rptH.Dispose();
            //}
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
        
        public JsonResult LoadCashOperation(int BranchID, DateTime? Begindate, DateTime? EndDate, int? tillID)
        {
            var model = new
            {
                data = from c in GenerateReportOperationPeriodeOfDay(BranchID, Begindate, EndDate, tillID).OrderBy(d=>d.OperationDate)
                select new
                {
                    RptCashDayOperationID = c.RptCashDayOperationID,
                    TransactionNumber = c.TransactionNumber,
                    Operation = c.Operation,
                    Intervenant = c.Intervenant,
                    InputAmount = c.InputAmount,
                    OutPutAmount = c.OutPutAmount,
                    OperationDate = c.OperationDate.ToString("yyyy-MM-dd")
                }
            };
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCashOperationTotal(int BranchID, DateTime? Begindate, DateTime? EndDate, int? tillID)
        {
            List<object> _InfoList = new List<object>();
            double TotalInput = 0d;
            double TotalSortie = 0d;
            double Balance = 0d;
            double CashHandCloseDay = 0d;

            //foreach (Category productcat in db.Categories.ToList())
            List<RptCashDayOperation> cashop = GenerateReportOperationPeriodeOfDay(BranchID, Begindate, EndDate, tillID).OrderBy(d => d.OperationDate).ToList();
            if (cashop.Count>0)
            {
                TotalInput = cashop.Select(c => c.InputAmount).Sum();
                TotalSortie = cashop.Select(c => c.OutPutAmount).Sum();
                Balance = TotalInput - TotalSortie;
                CashHandCloseDay = cashop.Select(c => c.CashHandCloseDay).FirstOrDefault();
            }
            
            _InfoList.Add(new
            {
                TotalInput = TotalInput,
                TotalSortie = TotalSortie,
                Balance=Balance,
                CashHandCloseDay = CashHandCloseDay
            });
            return Json(_InfoList, JsonRequestBehavior.AllowGet);
        }

        //This method print report of all operations of cash during a period of day
        public List<RptCashDayOperation> GenerateReportOperationPeriodeOfDay(int BranchID,DateTime? To, DateTime? At, int? tillID)
		{
            
            List<RptCashDayOperation> model = new List<RptCashDayOperation>();
            
           if (BranchID>0 )
            {
                //Date and tillDayID
                Branch currentBranch = (from t in db.Branches
                                    where t.BranchID == BranchID
                                    select t).SingleOrDefault();

                //=================
                //Branch currentBranch = currentTill.Branch;
                string defaultImgPath = Server.MapPath("~/Content/Images/App/deboy-logo.png");
                byte[] COMPANY_LOGO = System.IO.File.ReadAllBytes(defaultImgPath);
                //cash operation
                List<ReportOperationPeriodeOfDay> lstCashOperationOfDay = db.Database.SqlQuery<ReportOperationPeriodeOfDay>("SELECT * FROM DBO.[GenerateReportOperationPeriodeOfDayTill] (@To,@At,@tillID) ", new SqlParameter("@To", To.Value), new SqlParameter("@At", At.Value), new SqlParameter("@tillID", tillID.Value)).ToList();
                lstCashOperationOfDay.ForEach(sl =>
                {

                    model.Add(
                        new RptCashDayOperation
                        {
                            //Variables fields
                            RptCashDayOperationID = sl.ReportOperationPeriodeOfDayID,
                            OperationDate = sl.OperationDate,
                            GroupingDate = new DateTime(sl.OperationDate.Year, sl.OperationDate.Month, sl.OperationDate.Day).ToOADate(),
                            BeginDate = To.Value,
                            EndDate = At.Value,
                            InputAmount = sl.InputAmount,//  _depositRepository.SaleTotalPriceAdvance(sd),
                            OutPutAmount = sl.OutPutAmount,
                            Intervenant = sl.Intervenant,
                            Solde = sl.Solde, //this field represent a real amount of operation
                            Operation = sl.Operation,
                            TransactionNumber = sl.TransactionNumber,
                            Description = sl.Description,// "No Comment",
                                                                //Head fields
                            Teller = CurrentUser.Name + " " + CurrentUser.Description,
                            CashRegisterName = sl.CashRegisterName,// currentTill.Name + " " + currentTill.Code,
                            RptTitle = Resources.TellerHistDayOp,
                            OpeningCashAmount = sl.OpeningCashAmount,
                            ClosingCashAmount = sl.ClosingCashAmount,
                            BranchName = currentBranch.BranchName,
                            BranchAdress = currentBranch.Adress.Quarter.QuarterLabel + " - " + currentBranch.Adress.Quarter.Town.TownLabel,
                            BranchTel = "Tel: " + currentBranch.Adress.AdressPhoneNumber + "PO Box :" + currentBranch.Adress.AdressPOBox,
                            CompanyName = Company.Name,
                            CompanyAdress = "Head Quater : " + Company.Adress.Quarter.Town.Region.RegionLabel + " - " + Company.Adress.Quarter.Town.TownLabel,
                            CompanyTel = "Tel: " + Company.Adress.AdressPhoneNumber,
                            CompanyCNI = "NO CONT : " + Company.CNI,
                            CompanyLogo = _fileRepository.FindAll.FirstOrDefault(f => f.GlobalPersonID == Company.GlobalPersonID) != null ? _fileRepository.FindAll.FirstOrDefault(f => f.GlobalPersonID == Company.GlobalPersonID).Content : COMPANY_LOGO,
                            PaymentMethod = sl.PaymentMethod,
                            CashHandCloseDay = sl.CashHandCloseDay.Value
                        }
                    );
                });
                
            }


            return model;

        }

        public JsonResult LoadBankOperation(int BranchID, DateTime? Begindate, DateTime? EndDate)
        {
            var model = new
            {
                data = from c in GenerateBankReportOperationPeriodeOfDay(BranchID, Begindate, EndDate).OrderBy(d => d.RptCashDayOperationID)
                       select new
                       {
                           RptCashDayOperationID = c.RptCashDayOperationID,
                           TransactionNumber = c.TransactionNumber,
                           Operation = c.Operation,
                           Intervenant = c.Intervenant,
                           InputAmount = c.InputAmount,
                           OutPutAmount = c.OutPutAmount,
                           OperationDate = c.OperationDate.ToString("yyyy-MM-dd"),
                           Bank=c.PaymentMethod
                       }
            };
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetBankOperationTotal(int BranchID, DateTime? Begindate, DateTime? EndDate)
        {
            List<object> _InfoList = new List<object>();
            double TotalInput = 0d;
            double TotalSortie = 0d;
            double Balance = 0d;

            //foreach (Category productcat in db.Categories.ToList())
            List<RptCashDayOperation> cashop = GenerateBankReportOperationPeriodeOfDay(BranchID, Begindate, EndDate).OrderBy(d => d.OperationDate).ToList();
            if (cashop.Count > 0)
            {
                TotalInput = cashop.Select(c => c.InputAmount).Sum();
                TotalSortie = cashop.Select(c => c.OutPutAmount).Sum();
                Balance = TotalInput - TotalSortie;
            }

            _InfoList.Add(new
            {
                TotalInput = TotalInput,
                TotalSortie = TotalSortie,
                Balance = Balance
            });
            return Json(_InfoList, JsonRequestBehavior.AllowGet);
        }
        //This method print report of all operations of cash during a period of day
        public List<RptCashDayOperation> GenerateBankReportOperationPeriodeOfDay(int BranchID, DateTime? To, DateTime? At)
        {

            List<RptCashDayOperation> model = new List<RptCashDayOperation>();

            if (BranchID > 0 )
            {
                
                //=================
                Branch currentBranch = db.Branches.Find(BranchID);
                string defaultImgPath = Server.MapPath("~/Content/Images/App/deboy-logo.png");
                byte[] COMPANY_LOGO = System.IO.File.ReadAllBytes(defaultImgPath);
                
                //bank operation
                List<ReportOperationPeriodeOfDay> lstBankOperationOfDay = db.Database.SqlQuery<ReportOperationPeriodeOfDay>("SELECT * FROM DBO.[GenerateReportOperationPeriodeOfDayBank] (@To,@At) ", new SqlParameter("@To", To.Value), new SqlParameter("@At", At.Value)).ToList();
                lstBankOperationOfDay.ForEach(sl =>
                {

                    model.Add(
                        new RptCashDayOperation
                        {
                            //Variables fields
                            RptCashDayOperationID = sl.ReportOperationPeriodeOfDayID,
                            OperationDate = sl.OperationDate,
                            GroupingDate = new DateTime(sl.OperationDate.Year, sl.OperationDate.Month, sl.OperationDate.Day).ToOADate(),
                            BeginDate = To.Value,
                            EndDate = At.Value,
                            InputAmount = sl.InputAmount,//  _depositRepository.SaleTotalPriceAdvance(sd),
                            OutPutAmount = sl.OutPutAmount,
                            Intervenant = sl.Intervenant,
                            Solde = sl.Solde, //this field represent a real amount of operation
                            Operation = sl.Operation,
                            TransactionNumber = sl.TransactionNumber,
                            Description = sl.Description,// "No Comment",
                                                         //Head fields
                            Teller = CurrentUser.Name + " " + CurrentUser.Description,
                            CashRegisterName = sl.CashRegisterName,// currentTill.Name + " " + currentTill.Code,
                            RptTitle = Resources.TellerHistDayOp,
                            OpeningCashAmount = sl.OpeningCashAmount,
                            ClosingCashAmount = sl.ClosingCashAmount,
                            BranchName = currentBranch.BranchName,
                            BranchAdress = currentBranch.Adress.Quarter.QuarterLabel + " - " + currentBranch.Adress.Quarter.Town.TownLabel,
                            BranchTel = "Tel: " + currentBranch.Adress.AdressPhoneNumber + "PO Box :" + currentBranch.Adress.AdressPOBox,
                            CompanyName = Company.Name,
                            CompanyAdress = "Head Quater : " + Company.Adress.Quarter.Town.Region.RegionLabel + " - " + Company.Adress.Quarter.Town.TownLabel,
                            CompanyTel = "Tel: " + Company.Adress.AdressPhoneNumber,
                            CompanyCNI = "NO CONT : " + Company.CNI,
                            CompanyLogo = _fileRepository.FindAll.FirstOrDefault(f => f.GlobalPersonID == Company.GlobalPersonID) != null ? _fileRepository.FindAll.FirstOrDefault(f => f.GlobalPersonID == Company.GlobalPersonID).Content : COMPANY_LOGO,
                            PaymentMethod = sl.PaymentMethod
                        }
                    );
                });

            }


            return model;

        }
        //This method print customer's bill
        //[HttpGet]
        [OutputCache(Duration = 3600)] 
		public ActionResult Bill()
		{
            try
            {
                ViewBag.Disabled = true;
                List<BusinessDay> listBDUser  =(List<BusinessDay>)Session["UserBusDays"];
                if (listBDUser.Count() > 1)
                {
                    ViewBag.Disabled = false;
                }
                BusinessDay busDays = listBDUser.FirstOrDefault();
                ViewBag.BusnessDayDate = busDays.BDDateOperation.ToString("yyyy-MM-dd");
                ViewBag.CurrentBranch = busDays.BranchID;
                
                Session["BusnessDayDate"] = busDays.BDDateOperation;
                return View();
            }

            catch (Exception e)
            {
                TempData["Message"] = "Error "+ e.Message;
                return this.View();
            }
		}

        public JsonResult LoadCustomers(string filter)
        {

            List<object> customersList = new List<object>();
            foreach (Customer customer in db.People.OfType<Customer>().Where(c => c.Name.StartsWith(filter.ToLower())).Take(100).ToArray().OrderBy(c => c.Name))
            {
                string itemLabel = "";

                //si le client est une personne physique, on renvoie son nom et son prénom, si c'est une entreprise, on renvoie son nom
                itemLabel = customer.Name ;

                customersList.Add(new
                {
                    Name = itemLabel,
                    ID = customer.GlobalPersonID
                });
            }

            return Json(customersList, JsonRequestBehavior.AllowGet);
        }

        //***************************************************


        public void GenerateNewBillRecapReport()
        {
            //ReportDocument rptH = new ReportDocument();
            try
            {
                bool isValid = false;

                DateTime BeginDate = (DateTime)Session["BeginDate"];
                DateTime EndDate = (DateTime)Session["EndDate"];
                int customerID = (int)Session["CustomerID"];
                BusinessDay bsday = SessionBusinessDay(null);
                string BillNumber = _transactNumberRepository.displayTransactNumber("FACT", bsday);

                Customer customer = (from cust in db.Customers
                                     where cust.GlobalPersonID == customerID
                                     select cust).SingleOrDefault();

                //verifions si cette facture existe deja
                Bill bill = db.Bills.FirstOrDefault(b => b.CustomerID == customerID && b.BeginDate == BeginDate.Date && b.EndDate == EndDate.Date);
                if (bill != null)
                {
                    _billRepository.DeleteBill(bill.BillID, SessionGlobalPersonID, bsday.BranchID);
                }
                
                List<object> model = new List<object>();

                string defaultImgPath = Server.MapPath("~/Content/Images/App/deboy-logo.png");
                byte[] COMPANY_LOGO = System.IO.File.ReadAllBytes(defaultImgPath);

                model = this.ModelBillCustoRecap(BeginDate, EndDate, customer, BillNumber);
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
                    //rptH.Load(path);
                    //rptH.SetDataSource(model);
                    //rptH.SetParameterValue("BalanceBefore", BalanceBefore);
                    //rptH.SetParameterValue("TotalDeposit", TotalDepositPeriode);
                    //rptH.SetParameterValue("BeginDate", BeginDate);
                    //rptH.SetParameterValue("EndDate", EndDate);
                    //rptH.SetParameterValue("ServerDate", bsday.BDDateOperation);

                    //bool isDownloadRpt = (bool)Session["isDownloadRpt"];
                    //rptH.ExportToHttpResponse(ExportFormatType.PortableDocFormat, System.Web.HttpContext.Current.Response, isDownloadRpt, "RptBillDBoy");//  strReportName);

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
            catch (Exception ex)
            {
                Response.Write(ex.ToString());
            }
            //finally
            //{
            //    rptH.Close();
            //    rptH.Dispose();
            //}
        }
        private List<object> ModelBillCustoRecap(DateTime BeginDate, DateTime EndDate, Customer customer, string BillNumber)
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
           

            double totalDepositBefore = _depositRepository.TotalDepotSliceBefore(customer, BeginDate);
            double achatTTCBefore = _saleRepository.TotalAchatBefore(customer, BeginDate);
            BalanceBefore = totalDepositBefore - achatTTCBefore;

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
            string QtyDetail = "";

            //traitement historique pour la periode choisie
            SaleE getSalePeriode = new SaleE();
            
            //recuperation des ttes les ventes de la periode
            List<SaleE> salelist = db.Sales.Where(sa => sa.CustomerID == customer.GlobalPersonID && (sa.SaleDate >= BeginDate.Date && sa.SaleDate <= EndDate.Date)).ToList();
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
                            DateCommande = allSales.SaleDate,
                            NumeroCommande = allSales.SaleReceiptNumber,
                            LineUnitPrice = sl.LineUnitPrice,
                            LineQuantity = sl.LineQuantity,
                            //NewLineQty = sl.LineQuantity,
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
                              NewLineQty = sl.LineQuantity ,
                              CustomerName = customer.Name , //customer.Name,//+ " " + customer.Description,
                              CustomerAdress = "B.P: " + customer.AdressPOBox + " " + customer.Adress.Quarter.Town.TownLabel + " Tél:" + customer.AdressPhoneNumber,
                              CustomerAccount = customer.AccountNumber,
                              VatRate = getSalePeriode.VatRate,
                              NumeroCde = getSalePeriode.SaleID.ToString(),
                              DateCde = getSalePeriode.SaleDate.Date,
                              SaleID = getSalePeriode.SaleID,
                              BillNumber = BillNumber,
                              CompanyTown = currentBranch.Adress.Quarter.Town.TownLabel,
                              BranchAbbreviation = QtyDetail
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

        //***************************************************
		/*[HttpPost]
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
		}*/

        public JsonResult PrintBill(int BranchID, int CustomerID, DateTime BeginDate, DateTime EndDate)
        {
            bool status = false;
            string Message = "";
            RptBill model = new RptBill();
            Bill SaveBill = new Bill();
            try
            {
                Session["BranchID"] = null;
                Session["CustomerID"] = null;
                Session["BeginDate"] = null;
                Session["EndDate"] = null;


                BusinessDay bsday = SessionBusinessDay(BranchID);
                string BillNumber = _transactNumberRepository.displayTransactNumber("FACT", bsday);

                Customer customer = (from cust in db.Customers
                                     where cust.GlobalPersonID == CustomerID
                                     select cust).SingleOrDefault();
                //verifions si cette facture existe deja
                Bill bill = db.Bills.FirstOrDefault(b => b.CustomerID == CustomerID && b.BeginDate == BeginDate.Date && b.EndDate == EndDate.Date);
                if (bill != null)
                {
                    _billRepository.DeleteBill(bill.BillID, SessionGlobalPersonID, bsday.BranchID);
                }

                model = this.ModelBillCusto(BeginDate, EndDate, customer, BillNumber);
                if (model!=null)
                {
                    double TotalDepositPeriode = (double)Session["TotalDepositPeriode"];

                    double BalanceBefore =(Session["BalanceBefore"]==null)?0d: (double)Session["BalanceBefore"];
                    double TauxRemise = (Session["TauxRemise"] == null) ? 0d : (double)Session["TauxRemise"];
                    double MontantRemise = (Session["MontantRemise"] == null) ? 0d : (double)Session["MontantRemise"];
                    double TauxEscompte = (Session["TauxEscompte"] == null) ? 0d : (double)Session["TauxEscompte"];
                    double MontantEscompte = (Session["MontantEscompte"] == null) ? 0d : (double)Session["MontantEscompte"];
                    double Transport = (Session["Transport"] == null) ? 0d : (double)Session["Transport"];
                    double TauxTva = (Session["TauxTva"] == null) ? 0d : (double)Session["TauxTva"];
                    double ValeurTva = (Session["ValeurTva"] == null) ? 0d : (double)Session["ValeurTva"];
                    double TotalTTC = (Session["TotalTTC"] == null) ? 0d : (double)Session["TotalTTC"];
                    double TotalDepot = (Session["TotalDepositPeriode"] == null) ? 0d : (double)Session["TotalDepositPeriode"];
                    double NetApayer = TotalTTC - TotalDepot;
                    List<BillDetail> BillDetails = (List<BillDetail>)Session["BillDetails"];

                    //persistance de la facture
                    SaveBill = new Bill()
                    {
                        BillNumber = BillNumber,
                        CustomerID = customer.GlobalPersonID,
                        BeginDate = BeginDate,
                        BillDate = bsday.BDDateOperation,
                        EndDate = EndDate,
                        BalanceBefore = BalanceBefore,
                        MontantRemise = MontantRemise,
                        MontantEscompte = MontantEscompte,
                        Transport = Transport,
                        TauxTva = TauxTva,
                        TotalDepot = TotalDepositPeriode,
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
                    Message = "Nothing Found; No Report name found";
                    status = false;
                    return new JsonResult { Data = new { status = status, Message = Message } };
                }

                Session["BranchID"] = BranchID;
                Session["CustomerID"] = CustomerID;
                Session["BeginDate"] = BeginDate;
                Session["EndDate"] = EndDate;

                Message = "OK";
                status = true;
            }
            catch (Exception e)
            {
                Message = "Error " + e.Message + " " + e.InnerException;
                status = false;
            }
            return new JsonResult { Data = new { status = status, Message = Message } };
        }

        public ActionResult GenerateNewBillReport()
        {
            ViewBag.CompanyLogoID = Company.GlobalPersonID;
            RptBill model = (Session["RptBill"]==null)?new RptBill(): (RptBill)Session["RptBill"];
            return View(model);
        }

        //recap
        public JsonResult PrintBillRecap(int BranchID, int CustomerID, DateTime BeginDate, DateTime EndDate)
        {
            bool status = false;
            string Message = "";
            RptBill model = new RptBill();
            Bill SaveBill = new Bill();
            try
            {
                Session["BranchID"] = null;
                Session["CustomerID"] = null;
                Session["BeginDate"] = null;
                Session["EndDate"] = null;

                BusinessDay bsday = SessionBusinessDay(BranchID);
                string BillNumber = _transactNumberRepository.displayTransactNumber("FACT", bsday);

                Customer customer = (from cust in db.Customers
                                     where cust.GlobalPersonID == CustomerID
                                     select cust).SingleOrDefault();
                //verifions si cette facture existe deja
                Bill bill = db.Bills.FirstOrDefault(b => b.CustomerID == CustomerID && b.BeginDate == BeginDate.Date && b.EndDate == EndDate.Date);
                if (bill != null)
                {
                    _billRepository.DeleteBill(bill.BillID, SessionGlobalPersonID, bsday.BranchID);
                }

                model = this.ModelBillCusto(BeginDate, EndDate, customer, BillNumber);
                if (model != null)
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

                    //persistance de la facture
                    SaveBill = new Bill()
                    {
                        BillNumber = BillNumber,
                        CustomerID = customer.GlobalPersonID,
                        BeginDate = BeginDate,
                        BillDate = bsday.BDDateOperation,
                        EndDate = EndDate,
                        BalanceBefore = BalanceBefore,
                        MontantRemise = MontantRemise,
                        MontantEscompte = MontantEscompte,
                        Transport = Transport,
                        TauxTva = TauxTva,
                        TotalDepot = TotalDepositPeriode,
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
                    Message = "Nothing Found; No Report name found";
                    status = false;
                    return new JsonResult { Data = new { status = status, Message = Message } };
                }

                Session["BranchID"] = BranchID;
                Session["CustomerID"] = CustomerID;
                Session["BeginDate"] = BeginDate;
                Session["EndDate"] = EndDate;

                Message = "OK";
                status = true;
            }
            catch (Exception e)
            {
                Message = "Error " + e.Message + " " + e.InnerException;
                status = false;
            }
            return new JsonResult { Data = new { status = status, Message = Message } };
        }

        public ActionResult GenerateNewBillReportRecap()
        {
            ViewBag.CompanyLogoID = Company.GlobalPersonID;
            RptBill model = (Session["RptBill"] == null) ? new RptBill() : (RptBill)Session["RptBill"];
            return View(model);
        }

        private RptBill ModelBillCusto(DateTime BeginDate, DateTime EndDate, Customer customer, string BillNumber)
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
            Session["RptBill"] = null;

            double BalanceBefore = 0d;
            double PeriodDeposit = 0d;
            double PeriodConsumption = 0d;
            double AmountToPaid = 0d;

           
            double TotalTTC = 0d;
            List<BillDetail> BillDetails = new List<BillDetail>();

            RptBill model =null;

            List<ReceiptLine> receiptline = new List<ReceiptLine>();

            string defaultImgPath = Server.MapPath("~/Content/Images/App/deboy-logo.png");
            byte[] COMPANY_LOGO = System.IO.File.ReadAllBytes(defaultImgPath);

            Branch currentBranch = new Branch();
            Devise currentDevise = new Devise();

            double MontantRemise = 0d;
            double MontantEscompte = 0d;
            double Transport = 0d;
            double ValeurTva = 0d;
            string QtyDetail = "";
            int i = 0;
            double saleAmount = 0d;
            double TotalHT = 0d;
            double RealTVAAmount = 0d;
            DateTime ServerDate = new DateTime();

            
            //solde avant la periode
            BalanceBefore =  _depositRepository.NewTotalDepotSliceBefore(customer.GlobalPersonID, BeginDate)- _depositRepository.NewTotalAchatBefore(customer.GlobalPersonID, BeginDate);
            //depot sur la periode
            PeriodDeposit = _depositRepository.NewTotalDepotSlicePeriode(customer.GlobalPersonID, BeginDate, EndDate);

            //traitement historique pour la periode choisie
            SaleE getSalePeriode = new SaleE();
            

            List <SaleE> salelist = db.Sales.Where(sa => sa.CustomerID == customer.GlobalPersonID && (sa.SaleDate >= BeginDate.Date && sa.SaleDate <= EndDate.Date)).ToList();
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

                //recuperation des lignes de vente
                //SpecialOrder currentSO = null;
                List<SaleLine> slLine = getSalePeriode.SaleLines.ToList();
                slLine.ForEach(sl =>
                {
                    MontantEscompte = MontantEscompte + ((allSales.RateDiscount * sl.LineAmount) / 100);
                    MontantRemise = MontantRemise + ((allSales.RateReduction * sl.LineAmount) / 100);
                    saleAmount += sl.LineAmount;
                    //details qty
                    QtyDetail = "";
                    i += 1;

                    
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
                    });
                    receiptline.Add(
                    new ReceiptLine
                    {
                        ReceiptLineID = i,
                        ReceiptLineQuantity = sl.LineQuantity,
                        ReceiptLineUnitPrice = sl.LineUnitPrice,
                        ReceiptLineAmount = sl.LineAmount,
                        Designation = sl.Product.ProductCode,
                        SaleDate = allSales.SaleDate,
                        CommandDate = allSales.SaleDate,
                        Reference  = allSales.SaleReceiptNumber,
                        DetailQty = (sl.Product is Lens || (allSales.SaleReceiptNumber.StartsWith("SOSA")) ) ? "V" :  (QtyDetail=="")? "Piece(s)" : QtyDetail + "(s)"
                    });
                });

                ServerDate = SessionBusinessDay(currentBranch.BranchID).BDDateOperation;

                TotalHT = saleAmount - MontantEscompte - MontantRemise + Transport;
                RealTVAAmount = ValeurTva;
                TotalTTC = TotalHT + RealTVAAmount;
                PeriodConsumption = TotalTTC;

                AmountToPaid = PeriodConsumption - PeriodDeposit - BalanceBefore;
                //ajout des ventes ds la table des etats histo client
                model = new RptBill()
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
                    //ReductionAmount = MontantRemise,
                    //DiscountAmount = MontantEscompte,
                    Transport = Transport,
                    //ProductLabel = sl.Product.ProductLabel,
                    //ProductRef = sl.Product.ProductCode,
                    //LineUnitPrice = sl.LineUnitPrice,
                    //LineQuantity = sl.LineQuantity,
                    //NewLineQty = (sl.UniteDeCalcul == 0) ? sl.LineQuantity : sl.UniteDeCalcul,
                    CustomerName = customer.Name ,
                    CustomerAdress = "B.P: " + customer.AdressPOBox + " " + customer.Adress.Quarter.Town.TownLabel + " Tél:" + customer.AdressPhoneNumber,
                    CustomerAccount = customer.AccountNumber,
                    VatRate = getSalePeriode.VatRate,
                    NumeroCde = getSalePeriode.SaleID.ToString(),
                    DateCde = getSalePeriode.SaleDate.Date,
                    SaleID = getSalePeriode.SaleID,
                    BillNumber = BillNumber,
                    CompanyTown = currentBranch.Adress.Quarter.Town.TownLabel,
                    BranchAbbreviation = currentBranch.BranchAbbreviation,
                    BalanceBefore = BalanceBefore,
                    PeriodDeposit = PeriodDeposit,
                    PeriodConsumption = PeriodConsumption,
                    AmountToPaid = AmountToPaid,
                    ReceiptLines = receiptline,
                    ServerDate = ServerDate,
                    BeginDate = BeginDate,
                    EndDate = EndDate,
                    SaleAmount = saleAmount,
                    Discount= MontantRemise+MontantEscompte
                };
            }
        );

            

            Session["MontantRemise"] = MontantRemise;
            Session["MontantEscompte"] = MontantEscompte;
            Session["Transport"] = Transport;
            Session["ValeurTva"] = ValeurTva;

            Session["BalanceBefore"] = BalanceBefore;
            Session["TotalTTC"] = TotalTTC;
            Session["BillDetails"] = BillDetails;
            Session["TotalDepositPeriode"] = PeriodDeposit;
            Session["RptBill"] = model;
            return model;
        }

		public void GenerateBillReport()
		{
            List<object> model = new List<object>();
            //ReportDocument rptH = new ReportDocument();
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

                TotalDeposit = TotalDeposit + newAmtDepEpargne;
				if (isValid)
				{
					string path = Server.MapPath("~/Reports/CashRegister/RptBillDBoy.rpt");
					//rptH.Load(path);
					//rptH.SetDataSource(model);
					//rptH.SetParameterValue("PurchaseTTC", purchaseTTC);
					//rptH.SetParameterValue("TotalRemaingAmountParam", TotalReaming);
     //               rptH.SetParameterValue("TotalDeposit", TotalDeposit);
					//rptH.SetParameterValue("DeviseLabel", deviseLabel);
					//rptH.SetParameterValue("BeginDate", BeginDate);
					//rptH.SetParameterValue("EndDate", EndDate);
     //               bool isDownloadRpt = (bool)Session["isDownloadRpt"];
     //               rptH.ExportToHttpResponse(ExportFormatType.PortableDocFormat, System.Web.HttpContext.Current.Response, isDownloadRpt, "RptBillDBoy");//  strReportName);

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
            //finally
            //{
            //    rptH.Close();
            //    rptH.Dispose();
            //}
		}
		public void GenerateSaleBill()
		{
			//ReportDocument rptH = new ReportDocument();
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
					//rptH.Load(path);
					//rptH.SetDataSource(model);
					//rptH.SetParameterValue("PurchaseTTC", purchaseTTC);
					//rptH.SetParameterValue("TotalRemaingAmountParam", TotalReaming);
					//rptH.SetParameterValue("DeviseLabel", deviseLabel);
					//rptH.SetParameterValue("BeginDate", currentSale.SaleDate);
					//rptH.SetParameterValue("EndDate", currentSale.SaleDate);
     //               bool isDownloadRpt = (bool)Session["isDownloadRpt"];
					//rptH.ExportToHttpResponse(ExportFormatType.PortableDocFormat, System.Web.HttpContext.Current.Response, isDownloadRpt, "RptBill");

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
            //finally
            //{
            //    rptH.Close();
            //    rptH.Dispose();
            //}
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

			//return this.Store(list);
            return Json(list, JsonRequestBehavior.AllowGet);

        }
		public JsonResult LoadThirdPartyAccounts(int? BranchID)
		{
			List<object> customers = new List<object>();
			List<Customer> customers1 = new List<Customer>();
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
            return Json(customers, JsonRequestBehavior.AllowGet);
		}
        

	}

}