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
using System.Drawing.Printing;
using System.Drawing;
using System.Management;
using System.Printing;


namespace FatSodDental.UI.Areas.CashRegister.Controllers
{
	[Authorize(Order = 1)]
	[TakeBusinessDay(Order = 2)]
    public class SaleReceiptController : BaseController
	{
        //private const string CONTROLLER_NAME = "CashRegister/SaleReceipt";
		//*********************
		private IBusinessDay _busDayRepo;
		private IDeposit _depositRepository;

		private ICustomerReturn _customerReturnRepository;
		private ITransactNumber _transactNumberRepository;
        private ISale _saleRepository;

		// GET: CashRegister/State
		public SaleReceiptController(
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
        public ActionResult PrintDepositReceipt(string detail)
        {
            Session["detail"] = detail;
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
            //List<RptReceipt> model = new List<RptReceipt>();
            //List<RptPaymentDetail> modelsubRpt = new List<RptPaymentDetail>();
            List<object> model = new List<object>();
            List<object> modelsubRpt = new List<object>();

            ReportDocument rptH = new ReportDocument();
            try
            {
                int saleID = (Session["Receipt_SaleID"] == null) ? 0 : (int)Session["Receipt_SaleID"];
                string customerID = (Session["Receipt_CustomerID"] == null) ? "" : (string)Session["Receipt_CustomerID"];
                double receiveAmountTot = (Session["ReceiveAmoung_Tot"] == null) ? 0 : (double)Session["ReceiveAmoung_Tot"];
                string detail = (Session["detail"] == null) ? "" : (string)Session["detail"];

                string repName = "";
                bool isValid = false;
                double totalAmount = 0d;
                double totalRemaining = 0d;
                double TotalReceiveAmount = 0d;

                string path = "";
                string defaultImgPath = Server.MapPath("~/Content/Images/App/deboy-logo.png");
                byte[] COMPANY_LOGO = System.IO.File.ReadAllBytes(defaultImgPath);



                Devise devise = new Devise();
                Branch curBranch = new Branch();

                string TitleDeposit = "";
                string RptTitle = "";


                curBranch = db.Branches.Find(db.UserBranches.Where(ub => ub.UserID == SessionGlobalPersonID).FirstOrDefault().BranchID);
                BusinessDay businessDay = _busDayRepo.GetOpenedBusinessDay(curBranch);
                devise = db.Devises.FirstOrDefault(d => d.DefaultDevise);

                Company cmpny = db.Companies.FirstOrDefault();
                if (saleID > 0)//depot pour une vente
                {

                    double saleAmount = 0d;
                    SaleE currentSale = db.Sales.Find(saleID);

                    int i = 1;

                    //recuperation des versements
                    List<CustomerSlice> lstCustomerSlice = db.CustomerSlices.Where(sl => sl.SaleID == currentSale.SaleID).ToList();
                    foreach (CustomerSlice cs in lstCustomerSlice)
                    {
                        TotalReceiveAmount = TotalReceiveAmount + cs.SliceAmount;
                        modelsubRpt.Add(
                        new //RptPaymentDetail
                        {
                            Reference = currentSale.SaleReceiptNumber,
                            DepositDate = cs.SliceDate,
                            Description = i.ToString() + " Payment(s)",
                            LineUnitPrice = cs.SliceAmount,
                            RptReceiptPaymentDetailID = 1
                        });
                        i = i + 1;
                    }

                    //recuperation des versements
                    List<SaleLine> lstSaleLine = db.SaleLines.Where(sl => sl.SaleID == currentSale.SaleID).ToList();
                    totalAmount = (lstSaleLine.Count > 0) ? Util.ExtraPrices(lstSaleLine.Select(c => c.LineAmount).Sum(), currentSale.RateReduction, currentSale.RateDiscount, currentSale.Transport, currentSale.VatRate).TotalTTC : 0; //montant du verre
                    totalRemaining = totalAmount - TotalReceiveAmount;

                    foreach (SaleLine custsaleLine in lstSaleLine)
                    {
                        string labelFrame = (custsaleLine.marque != null && custsaleLine.reference != null) ? Resources.Marque + " " + custsaleLine.marque + " " + Resources.Reference + " " + custsaleLine.reference : "";
                        model.Add(
                        new //RptReceipt
                        {
                            RptReceiptID = 1,
                            ReceiveAmount = receiveAmountTot,
                            TotalAmount = totalRemaining, //montant restant de la facture
                            LineUnitPrice = Util.ExtraPrices(custsaleLine.LineAmount, currentSale.RateReduction, currentSale.RateDiscount, currentSale.Transport, currentSale.VatRate).TotalTTC, //montant du verre
                            LineQuantity = custsaleLine.LineQuantity,
                            CompanyName = cmpny.Name,
                            CompanyAdress = cmpny.Adress.AdressFullName,// "Head Quater : " + cmpny.Adress.Quarter.Town.Region.RegionLabel + " - " + cmpny.Adress.Quarter.Town.TownLabel,
                            CompanyTel = "Tel: " + cmpny.Adress.AdressPhoneNumber + " Cell:" + cmpny.Adress.AdressCellNumber,
                            BranchName = currentSale.Branch.BranchName,
                            BranchAdress = "B.P " + cmpny.Adress.AdressPOBox + ", " + cmpny.Adress.Quarter.Town.TownLabel + ", Email: " + cmpny.Adress.AdressEmail,//currentSale.Branch.Adress.Quarter.QuarterLabel + " - " + currentSale.Branch.Adress.Quarter.Town.TownLabel,
                            BranchTel = "Tel: " + currentSale.Branch.Adress.AdressPhoneNumber,
                            Reference = currentSale.SaleReceiptNumber,
                            CompanyCNI = "NO CONT : " + cmpny.CNI,
                            Operator = CurrentUser.Name + " " + CurrentUser.Description,
                            CustomerName = customerID,
                            ProductLabel = (labelFrame.Trim().Length > 0) ? Resources.frame + " " + /*custsaleLine.ProductLabel + " " +*/ labelFrame : custsaleLine.ProductLabel,
                            SaleDate = currentSale.SaleDate,
                            Title = TitleDeposit,
                            DeviseLabel = currentSale.Devise.DeviseLabel,
                            RateTVA = currentSale.VatRate,
                            RateReduction = currentSale.RateReduction,
                            RateDiscount = currentSale.RateDiscount,
                            Transport = currentSale.Transport,
                            RptReceiptPaymentDetailID = 1,
                            CompanyLogo = db.Files.FirstOrDefault(f => f.GlobalPersonID == cmpny.GlobalPersonID) != null ? db.Files.FirstOrDefault(f => f.GlobalPersonID == cmpny.GlobalPersonID).Content : COMPANY_LOGO
                        }
                    );
                        //
                    }

                    if (TotalReceiveAmount > 0)
                    {
                        if (detail == "oui")
                        {
                            path = Server.MapPath("~/Reports/CashRegister/RptReceiptDepositDetail.rpt");
                            repName = "RptReceiptDepositDetail";
                        }
                        else
                        {
                            path = Server.MapPath("~/Reports/CashRegister/RptReceiptDeposit.rpt");
                            repName = "RptReceiptDeposit";
                        }
                    }
                    else
                    {
                        path = Server.MapPath("~/Reports/CashRegister/RptReceiptSansDeposit.rpt");
                        repName = "RptReceiptSansDeposit";
                    }

                    isValid = true;
                }


                if (isValid)
                {
                    rptH.Load(path);
                    rptH.SetDataSource(model);
                    if (TotalReceiveAmount > 0)
                    {
                        rptH.OpenSubreport("PaymentDetail").SetDataSource(modelsubRpt);
                    }
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
		
		//This method print customer's bill
		//[HttpGet]
        public ActionResult SaleReceipt()
		{
            try
            {
            Session["SaleLines"] = new List<SaleLine>();
            List<BusinessDay> UserBusDays = (List<BusinessDay>)Session["UserBusDays"];
            DateTime BDDateOperation = UserBusDays.FirstOrDefault().BDDateOperation;
           
            ViewBag.BusnessDayDate = BDDateOperation;
            return View(ModelReturnAbleSales(BDDateOperation));
            }

            catch (Exception e)
            {
                X.Msg.Alert("Error ", e.Message).Show();
                return this.Direct();
            }

		}

        public ActionResult ReloadSalesListStore()
        {
            this.GetCmp<Store>("CommadListStore").Reload();
            return this.Direct();
        }

        /// <summary>
        /// Liste des ventes donc la garantie court encore et dont tous les éléments de la liste n'ont pas encore été retournés
        /// </summary>
        private List<object> ModelReturnAbleSales(DateTime SoldDate)
        {

            
            List<object> model = new List<object>();
            BusinessDay currentBD = _busDayRepo.GetOpenedBusinessDay(CurrentUser).FirstOrDefault();
            
            var allSales = db.Sales
                    .Where(sa => sa.BranchID == currentBD.BranchID && sa.SaleDate == SoldDate.Date)
                    .ToList()
                    .Select(s => new SaleE
                    {
                        SaleID = s.SaleID
                    }).AsQueryable();

            //il faut mainteant vérifier si la vente à encore au moins une ligne de vente pouvant faire l'objet d'un retour
            foreach (SaleE s in allSales)
            {
                SaleE sale = _customerReturnRepository.GetRealSale(s.SaleID);

                if (sale.SaleLines == null || sale.SaleLines.Count <= 0) { continue; }

                //if ((sale.SaleDate.Date.Equals(SoldDate.Date)))
                //{
                    model.Add(
                                new
                                {
                                    SaleID = sale.SaleID,
                                    SaleDate = sale.SaleDate,
                                    SaleDeliveryDate = sale.SaleDeliveryDate,
                                    CustomerFullName = sale.CustomerName,
                                    SaleReceiptNumber = sale.SaleReceiptNumber,
                                    SaleTotalPrice = Util.ExtraPrices(sale.SaleLines.Select(sl => sl.LineAmount).Sum(), sale.RateReduction, sale.RateDiscount, sale.Transport, sale.VatRate).TotalTTC,
                                    Representant = sale.CustomerName
                                   
                                }
                              );
                //}
            }

         
            return model;
        }

        [HttpPost]
        public StoreResult SaleLines()
        {
            return this.Store(ModelSaleLines);
        }

        [HttpPost]
        public StoreResult ReturnAbleSales(DateTime SoldDate)
        {
            return this.Store(ModelReturnAbleSales(SoldDate));
        }

        private List<object> ModelSaleLines
        {
            get
            {
                List<object> model = new List<object>();
                List<SaleLine> SaleLines = (List<SaleLine>)Session["SaleLines"];
                if (SaleLines != null && SaleLines.Count > 0)
                {
                    foreach (SaleLine sl in SaleLines)
                    {
                        //Si toutes les lignes de la ligne de vente ont déjà été retournées, on ne l'ajoute pas dans le tableau des salelines
                        SaleE sale = db.Sales.Find(sl.SaleID);
                        model.Add(
                                new
                                {
                                    SaleLineID = sl.LineID,
                                    ProductLabel = sl.Product.GetProductCode(),
                                    LineUnitPrice = sl.LineUnitPrice,
                                    LineQuantity = sl.LineQuantity,
                                    LineAmount = sl.LineQuantity * sl.LineUnitPrice,
                                    ReturnPrice = Util.ExtraPrices(sl.LineQuantity * sl.LineUnitPrice, sale.RateReduction, sale.RateDiscount, sale.Transport, sale.VatRate)
                                }
                              );
                    }
                }
                return model;
            }
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
        /// cette methode permet de suprimer une operation qui a ete valider
        /// Supression successive ds les tables
        /// -Sale
        /// -SaleLine
        /// -CustomerSlice
        /// -SaleAccountOperation
        /// </summary>
        /// <param name="SaleID"></param>
        /// <returns></returns>
        //[HttpPost]
        public ActionResult Delete(int SaleID)
        {
            try
            {
                _saleRepository.SaleDeleteDoubleEntry(SaleID, SessionGlobalPersonID,SessionBusinessDay(null).BDDateOperation);
                this.GetCmp<Store>("CommadListStore").Reload();
                this.GetCmp<FormPanel>("GlobalSaleForm").Reset(true);
                return this.Direct();
            }
            catch (Exception e)
            {
                X.Msg.Alert("Error ", e.Message).Show();
                return this.Direct();
            }
        }


        /// <summary>
        /// Cette méthode est appelée quand une vente est sélectionnée et permet de renseigner les champs de formulaire liés à la vente sélectionnée. il s'agit de :
        /// 1-Le formulaire de vente
        /// 2-le cady de la vente. 
        /// NB : Il reste à l'utilisateur de remplir le cady de retour
        /// </summary>
        /// <param name="ID"> ID de la vente sélectionnée par l'utilisateur</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult InitializeFields(int SaleID)
        {
            ResetReturn();


            //we take sale and her salelines
            SaleE selectedSale = _customerReturnRepository.GetRealSale(SaleID);

            Session["SaleID"] = SaleID;
            Session["SaleLines"] = selectedSale.SaleLines.ToList();
            Session["SliceAmount"] = _depositRepository.SaleTotalPriceAdvance(selectedSale);

            Session["Receipt_SaleID"] = SaleID;
            Session["Receipt_CustomerID"] = selectedSale.CustomerName;
            Session["ReceiveAmoung_Tot"] = (selectedSale.CustomerSlice != null) ? selectedSale.CustomerSlice.SliceAmount : 0;

            //Session["CashCustomer"] = selectedSale.Customer.IsCashCustomer == CashCustomer.Cash;


            //this.GetCmp<Container>("Pdf").Hidden = true;
            this.GetCmp<NumberField>("BuyField").Value = 10;
            //this.GetCmp<TextField>("CashCustomer").Value = selectedSale.Customer.IsCashCustomer;

            this.GetCmp<ComboBox>("BuyType").Value = "Credit";
            //Session["CommandOderLines"] = customerLineOderLines;

            this.GetCmp<Store>("CommandOderLines").Reload();
            this.GetCmp<GridPanel>("CommandLinesGrid").Disabled = false;
            this.GetCmp<Panel>("SliceAmountForm").Disabled = false;
            this.GetCmp<FormPanel>("CommandTotalAmount").Disabled = false;
            this.GetCmp<TextField>("SaleID").Value = selectedSale.SaleID;
            this.GetCmp<TextField>("BranchID").Value = selectedSale.BranchID;
            this.GetCmp<TextField>("Representant").Value = selectedSale.CustomerName;
            //this.GetCmp<TextField>("CustomerID").Value = selectedSale.CustomerID;
            this.GetCmp<ComboBox>("DeviseID").Value = selectedSale.DeviseID;
            this.GetCmp<TextField>("CustomerName").Value = selectedSale.CustomerName;
            this.GetCmp<TextField>("SaleReceiptNumber").Value = selectedSale.SaleReceiptNumber;

            this.GetCmp<DateField>("SaleDate").Value = selectedSale.SaleDate;
            this.GetCmp<DateField>("SaleDeliveryDate").Value = selectedSale.SaleDeliveryDate;

            this.GetCmp<TextField>("Representant").Value = selectedSale.CustomerName;
                
            ApplyExtraPrices(selectedSale, selectedSale.SaleLines.ToList(), selectedSale.RateReduction, selectedSale.RateDiscount, selectedSale.Transport, selectedSale.VatRate);

            return this.Direct();

        }

        public void ApplyExtraPrices(SaleE sale, List<SaleLine> CustomerOrderLines, double reduction, double discount, double transport, double vatRate)
        {

            double valueOperation = CustomerOrderLines.Select(l => l.LineAmount).Sum();
            //we add extra price
            double new_HT_price = valueOperation;
            double remise = 0;
            double escompte = 0;
            double NetCom = valueOperation;
            //double vatRate = CodeValue.Accounting.ParamInitAcct.VATRATE;

            ExtraPrice extra = Util.ExtraPrices(valueOperation, reduction, discount, transport, vatRate);

            this.GetCmp<NumberField>("InitialHT").Value = valueOperation;
            this.GetCmp<NumberField>("DiscountAmount").Value = extra.DiscountAmount;
            this.GetCmp<NumberField>("NetCom").Value = extra.NetCom;
            this.GetCmp<NumberField>("ReductionAmount").Value = extra.ReductionAmount;
            this.GetCmp<NumberField>("TotalPriceHT").Value = extra.NetFinan;
            this.GetCmp<NumberField>("TVAAmount").Value = extra.TVAAmount;
            this.GetCmp<NumberField>("TotalPriceTTC").Value = extra.TotalTTC;
            this.GetCmp<NumberField>("InitialTTC").Value = extra.TotalTTC;

            this.GetCmp<NumberField>("Reduction").Value = reduction;
            this.GetCmp<NumberField>("Discount").Value = discount;

            this.GetCmp<NumberField>("Transport").Value = transport;
            this.GetCmp<NumberField>("VatRate").Value = vatRate;

            //bool iscashCusto = (bool)Session["CashCustomer"];
            this.GetCmp<NumberField>("SliceAmount").Value = _depositRepository.SaleTotalPriceAdvance(sale);
            this.GetCmp<NumberField>("RemaingAmount").Value = _depositRepository.SaleRemainder(sale);
            
        }

	}

}