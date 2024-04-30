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
using FatSod.Supply.Abstracts;
using FatSod.Ressources;
using System.IO;
using CrystalDecisions.CrystalReports.Engine;
using FatSodDental.UI.Filters;
using FastSod.Utilities.Util;
using CrystalDecisions.Shared;
using FatSod.Budget.Entities;
using FatSod.DataContext.Concrete;
using System.Drawing.Printing;
using System.Drawing;
using System.Management;
using System.Printing;


namespace FatSodDental.UI.Areas.CashRegister.Controllers
{
	[Authorize(Order = 1)]
	[TakeBusinessDay(Order = 2)]
    public class PrintDepositController : BaseController
	{
        //private const string CONTROLLER_NAME = "CashRegister/PrintDeposit";
		//*********************
		private IBusinessDay _busDayRepo;
		private IDeposit _depositRepository;

		private ICustomerReturn _customerReturnRepository;
		private ITransactNumber _transactNumberRepository;
        private ISale _saleRepository;

		// GET: CashRegister/State
		public PrintDepositController(
				IBusinessDay busDayRepo,
				IDeposit depositRepository,
				ICustomerReturn customerReturnRepository,
			    ITransactNumber transactNumberRepository,
                ISale saleRepository
				)
		{
			this._busDayRepo = busDayRepo;
			this._depositRepository = depositRepository;
			this._customerReturnRepository = customerReturnRepository;
			this._transactNumberRepository = transactNumberRepository;
            this._saleRepository = saleRepository;
		}
		
		
		//This method load a method that print a receip of sale

		public void LoadCMP(/*string Component, string action*/)
		{
            int SaleID = (int)Session["SaleID"];

			this.GetCmp<Panel>("Pdf").LoadContent(new ComponentLoader
			{
				Url = Url.Action("GenerateReceipt"),
				DisableCaching = false,
				Mode = LoadMode.Frame,
				//Params = { new Parameter("sale", sale) }
			});
		}

        public ActionResult Index()
		{
            try
            {
            Session["SaleLines"] = new List<SaleLine>();
            List<BusinessDay> UserBusDays = (List<BusinessDay>)Session["UserBusDays"];
            DateTime BDDateOperation = UserBusDays.FirstOrDefault().BDDateOperation;
            
            ViewBag.BusnessDayDate = BDDateOperation;
            return View(ModelReturnDeposit(BDDateOperation));
            }

            catch (Exception e)
            {
                X.Msg.Alert("Error ", e.Message).Show();
                return this.Direct();
            }

		}

        public ActionResult ReloadDepositListStore()
        {
            this.GetCmp<Store>("DepositListStore").Reload();
            return this.Direct();
        }

        /// <summary>
        /// Liste des depots pour la date en question
        /// </summary>
        private List<object> ModelReturnDeposit(DateTime DepositDate)
        {

            
            List<object> model = new List<object>();
            BusinessDay currentBD = _busDayRepo.GetOpenedBusinessDay(CurrentUser).FirstOrDefault();
          
            foreach (AllDeposit s in db.AllDeposits.Where(d=>d.AllDepositDate==DepositDate.Date && d.BranchID==currentBD.BranchID).ToList())
            {
                
            model.Add(
                        new
                        {
                            AllDepositID = s.AllDepositID,
                            AllDepositDate = s.AllDepositDate,
                            Amount = s.Amount,
                            CustomerFullName = s.Customer.Name,
                            AllDepositReference = s.AllDepositReference,
                            Representant = s.Representant     
                        }
                        );
            }
            return model;
        }

       

        [HttpPost]
        public StoreResult ReturnAbleDeposit(DateTime DepositDate)
        {
            return this.Store(ModelReturnDeposit(DepositDate));
        }
		
		private Company Company
		{
			get
			{
				return db.Companies.FirstOrDefault();
			}
		}
		
        public DirectResult ResetReturn()
        {

            return this.Direct();
        }

        /// <summary>
        /// cette methode permet de suprimer un depot qui a ete valider
        /// Supression successive ds les tables
        /// -Deposit
        /// -AllDeposit
        /// -CustomerSlice
        /// -DepositAccountOperations
        /// -AccountOperation
        /// </summary>
        /// <param name="AllDepositID"></param>
        /// <returns></returns>
        //[HttpPost]
        public ActionResult Delete(int AllDepositID)
        {
            try
            {
                _depositRepository.DeleteDepositEntry(AllDepositID, SessionGlobalPersonID,SessionBusinessDay(null).BDDateOperation);
                this.GetCmp<Store>("DepositListStore").Reload();
                this.GetCmp<FormPanel>("GlobalDepositForm").Reset(true);
                return this.Direct();
            }
            catch (Exception e)
            {
                X.Msg.Alert("Error ", e.Message).Show();
                return this.Direct();
            }
        }


        /// <summary>
        /// Cette méthode est appelée quand un depot est sélectionnée 
        /// </summary>
        /// <param name="ID"> ID du depot sélectionnée par l'utilisateur</param>
        /// <returns></returns>
        [HttpPost]
        public DirectResult InitializeFields(int AllDepositID)
        {
            //ResetReturn();

            //we take sale and her salelines
            AllDeposit selectedDeposit = db.AllDeposits.Find(AllDepositID);

            Session["Receip_DepositID"] = AllDepositID;
            Session["ReceiveAmoung_Tot"] = selectedDeposit.Amount;
            Session["Receipt_CustomerID"] = selectedDeposit.CustomerID;
            Session["DepositReason"] = selectedDeposit.AllDepositReason;

            this.GetCmp<Store>("DepositListStore").Reload();
            this.GetCmp<GridPanel>("DepositList").Disabled = false;
            this.GetCmp<FormPanel>("GlobalDepositForm").Disabled = false;
            this.GetCmp<TextField>("AllDepositID").Value = selectedDeposit.AllDepositID;
            this.GetCmp<TextField>("BranchID").Value = selectedDeposit.BranchID;
            //this.GetCmp<TextField>("Representant").Value = selectedDeposit.Representant;
            this.GetCmp<TextField>("CustomerID").Value = selectedDeposit.CustomerID;
            this.GetCmp<TextField>("CustomerName").Value = selectedDeposit.Customer.Name;
            this.GetCmp<TextField>("AllDepositReference").Value = selectedDeposit.AllDepositReference;
            this.GetCmp<DateField>("AllDepositDate").Value = selectedDeposit.AllDepositDate;
            this.GetCmp<NumberField>("Amount").Value = selectedDeposit.Amount;
            return this.Direct();

        
        }

        //This method load a method that print a receip of deposit
        public ActionResult PrintDepositReceipt()
        {
            this.GetCmp<Panel>("Pdf").LoadContent(new ComponentLoader
            {
                Url = Url.Action("GenerateReceiptDeposit"),
                DisableCaching = false,
                Mode = LoadMode.Frame,
            });

            return this.Direct();
        }
        //This method print a receipt of customer
        public void GenerateReceiptDeposit()
        {
             bool isValid = false;
            List<object> model = new List<object>();
            ReportDocument rptH = new ReportDocument();
            try
            {
            int customerID = (Session["Receipt_CustomerID"] == null) ? 0 : (int)Session["Receipt_CustomerID"];
            int DepositID = (Session["Receip_DepositID"] == null) ? 0 : (int)Session["Receip_DepositID"];
            double receiveAmountTot = (Session["ReceiveAmoung_Tot"] == null) ? 0 : (double)Session["ReceiveAmoung_Tot"];
            string DepositReason = (Session["DepositReason"] == null) ? "" : (string)Session["DepositReason"];

            string repName = "";
           
            string path = "";
            string defaultImgPath = Server.MapPath("~/Content/Images/App/deboy-logo.png");
            byte[] COMPANY_LOGO = System.IO.File.ReadAllBytes(defaultImgPath);


            Devise devise = new Devise();
            Branch curBranch = new Branch();

            Customer customer = new Customer();

            if (customerID > 0)
            {
                customer = (from c in db.Customers
                            where c.GlobalPersonID == customerID
                            select c).SingleOrDefault();



                curBranch = db.Branches.Find(db.UserBranches.Where(ub => ub.UserID == SessionGlobalPersonID).FirstOrDefault().BranchID);
                BusinessDay businessDay = _busDayRepo.GetOpenedBusinessDay(curBranch);
                devise = db.Devises.FirstOrDefault(d => d.DefaultDevise);

                Company cmpny = db.Companies.FirstOrDefault();
                
                if (DepositID > 0) //depot d'epargne
                {
                    AllDeposit deposit = db.AllDeposits.Find(DepositID);
                    //curBranch = deposit.PaymentMethod1.Branch;
                    string TitleDeposit = "";
                    if (deposit.PaymentMethod is Till) TitleDeposit = "Cash Paid In For " + DepositReason;
                    if (deposit.PaymentMethod is Bank) TitleDeposit = "Bank Paid In For " + DepositReason;
                    if (deposit.PaymentMethod is SavingAccount) TitleDeposit = "Saving Paid In For " + DepositReason;
                    model.Add(
                        new
                        {
                            ReceiveAmount = receiveAmountTot,
                            TotalAmount = 0, //montant total de la facture
                            LineUnitPrice = 0, //reste du montant a verser
                            CompanyName = cmpny.Name,
                            CompanyAdress = "Head Quater : " + cmpny.Adress.Quarter.Town.Region.RegionLabel + " - " + cmpny.Adress.Quarter.Town.TownLabel,
                            CompanyTel = "Tel: " + cmpny.Adress.AdressPhoneNumber,
                            BranchName = curBranch.BranchName,
                            BranchAdress = curBranch.Adress.Quarter.QuarterLabel + " - " + curBranch.Adress.Quarter.Town.TownLabel,
                            BranchTel = "Tel: " + curBranch.Adress.AdressPhoneNumber,
                            Ref = deposit.AllDepositReference,
                            CompanyCNI = "NO CONT : " + cmpny.CNI,
                            Operator = CurrentUser.Name + " " + CurrentUser.Description,
                            CustomerName = customer.Name,// + " " + customer.Description,
                            CustomerAccount = customer.CNI,
                            SaleDate =deposit.AllDepositDate.Date,
                            Title = TitleDeposit,
                            DeviseLabel = devise.DeviseLabel,
                            CompanyLogo = db.Files.FirstOrDefault(f => f.GlobalPersonID == cmpny.GlobalPersonID) != null ? db.Files.FirstOrDefault(f => f.GlobalPersonID == cmpny.GlobalPersonID).Content : COMPANY_LOGO
                        }
                    );



                    path = Server.MapPath("~/Reports/CashRegister/RptReceiptSavingDeposit.rpt");
                    repName = "RptReceiptSavingDeposit";
                    isValid = true;
                }

                if (isValid)
                {
                    rptH.Load(path);
                    rptH.SetDataSource(model);
                    bool isDownloadRpt = (bool)Session["isDownloadRpt"];
                    rptH.ExportToHttpResponse(ExportFormatType.PortableDocFormat, System.Web.HttpContext.Current.Response, isDownloadRpt, repName);
                    //return File(stream, "application/pdf");
                }
                else
                {
                    Response.Write("Nothing Found; No Report name found");
                }
            }
            else
            {
                Response.Write("Nothing Found; Click on Deposit before print receipt");
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