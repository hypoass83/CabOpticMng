using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using Ext.Net;
using Ext.Net.MVC;
using FastSod.Utilities.Util;
using FatSod.DataContext.Concrete;
using FatSod.DataContext.Initializer;
using FatSod.Ressources;
using FatSod.Security.Abstracts;
using FatSod.Security.Entities;
using FatSod.Supply.Abstracts;
using FatSod.Supply.Entities;
using FatSodDental.UI.Controllers;
using FatSodDental.UI.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SaleE = FatSod.Supply.Entities.Sale;

namespace FatSodDental.UI.Areas.CashRegister.Controllers
{
    public class RptGeneSaleController : BaseController
    {
        //Current Controller and current page
        private const string CONTROLLER_NAME = "CashRegister/RptGeneSale";
        private const string VIEW_NAME = "Index";
        //person repository

        private IBusinessDay _busDayRepo;
        private ISale _saleRepository;
        private IRepository<FatSod.Security.Entities.File> _fileRepository;
        private ICustomerReturn _customerReturnRepository;

        private Company cmpny;
        //double totCustomerOrderTotalPrice = 0d;

        public RptGeneSaleController(
            ISale sale,
            IBusinessDay busDayRepo,
            ICustomerReturn customerReturnRepository,
			IRepository<FatSod.Security.Entities.File> fileRepository
            )
        {
            this._saleRepository = sale;
            this._busDayRepo = busDayRepo;
			this._fileRepository = fileRepository;
            this._customerReturnRepository = customerReturnRepository;
        }
        //
        // GET: /CashRegister/RptGeneSale/
        [OutputCache(Duration = 3600)]
        public ActionResult Index()
        {

            Session["BeginDate"] = new DateTime(1900, 1, 1);
            Session["EndDate"] = new DateTime(1900, 1, 1);
            this.chargeSolde();
            return View(ModelRptGeneSale(new DateTime(1900, 1, 1), new DateTime(1900, 1, 1)));
        }

        public ActionResult DisplayEntries(DateTime BeginDate, DateTime EndDate/*,int ? CategoryID*/)
        {
            //totCustomerOrderTotalPrice = 0d;
            Session["BeginDate"] = BeginDate;
            Session["EndDate"] = EndDate;
            this.PartialReset();
            /*if (CategoryID <= 0 || CategoryID == null)*/ this.chargeSolde();
            //else this.chargeSolde(CategoryID.Value);
            return this.Direct();
        }  
        public void Reset()
        {
            this.GetCmp<FormPanel>("RptGeneSale").Reset(true);
            this.PartialReset();
        }
        public void PartialReset()
        {
            this.GetCmp<Store>("Store").Reload();
        }
        [HttpPost]
        public StoreResult GetList(DateTime Bdate, DateTime Edate/*, int? CategoryID*/)
        {

            Session["BeginDate"] = Bdate;
            Session["EndDate"] = Edate;
            /*if (CategoryID <= 0 || CategoryID == null) */return this.Store(ModelRptGeneSale(Bdate, Edate));
            //else return this.Store(ModelRptGeneSale(Bdate, Edate, CategoryID.Value));
        }

        private void chargeSolde()
        {
            double Totaldebit = 0d;
            double advancedAmount = 0d;
            double TotalAdvanced = 0d;

            DateTime Bdate = (DateTime)Session["BeginDate"];
            DateTime Edate = (DateTime)Session["EndDate"];
            SaleE c = new SaleE();

            List<SaleE> lstCustoSaleAmt = _saleRepository.FindAll.Where(so => (so.SaleDate.Date >= Bdate.Date && so.SaleDate.Date <= Edate)).ToList();
            foreach (SaleE getSalePeriode in lstCustoSaleAmt.OrderBy(s => s.SaleDate))
            {
                c = _customerReturnRepository.GetRealSale(getSalePeriode);
                double CustomerOrderTotalPrice = Util.ExtraPrices(c.SaleLines.Select(sl => sl.LineAmount).Sum(),
                                                                      c.RateReduction, c.RateDiscount, c.Transport, c.VatRate).TotalTTC;

                Totaldebit = Totaldebit + CustomerOrderTotalPrice;

                List<CustomerSlice> lstCustSlice = db.CustomerSlices.Where(sl => sl.SaleID == c.SaleID).ToList();
                if (lstCustSlice != null && lstCustSlice.Count > 0)
                {
                    advancedAmount = lstCustSlice.Select(sl => sl.SliceAmount).Sum();
                }
                else
                {
                    advancedAmount = 0;
                }
                TotalAdvanced = TotalAdvanced + advancedAmount;
            }

            this.GetCmp<TextField>("TotalDebit").Value = Totaldebit;
            this.GetCmp<TextField>("TotalAdvanced").Value = TotalAdvanced;
        }
        
        //affiche la liste des
        private List<object> ModelRptGeneSale(DateTime Bdate, DateTime Edate) //, string BeginDate, string EndDate)
        {
            double advancedAmount = 0d;
            List<object> list = new List<object>();
            
            if (Bdate == new DateTime(1900, 01, 01) && Edate == new DateTime(1900, 01, 01))
            {
                return list;
            }
            var lstCustomer = _saleRepository.FindAll.Where(so => so.SaleDate.Date >= Bdate.Date && so.SaleDate.Date <= Edate)
                .Select(s => new
                {
                    CustomerName = s.CustomerName
                }).Distinct().ToList();
            //_saleRepository.FindAll.Where(so => so.SaleDate.Date >= Bdate.Date && so.SaleDate.Date <= Edate).Select(cus=>cus.Customer).Distinct().ToList()
            foreach (var custo in lstCustomer.OrderBy(o=>o.CustomerName))
                {
                    double totCustomerPrice = 0d;
                    SaleE c = new SaleE();
                    List<SaleE> lstCustoSale = _saleRepository.FindAll.Where(so => (so.SaleDate.Date >= Bdate.Date && so.SaleDate.Date <= Edate.Date) && so.CustomerName == custo.CustomerName).ToList();
                    foreach (SaleE getSalePeriode in lstCustoSale.OrderBy(s => s.SaleDate))
                    {
                        c = _customerReturnRepository.GetRealSale(getSalePeriode);

                        double CustomerOrderTotalPrice = Util.ExtraPrices(c.SaleLines.Select(sl => sl.LineAmount).Sum(),
                                                                              c.RateReduction, c.RateDiscount, c.Transport, c.VatRate).TotalTTC;

                        totCustomerPrice = totCustomerPrice + CustomerOrderTotalPrice;
                    }
                    //traitement pr cahque customer

                    foreach (SaleE getSalePeriode in lstCustoSale.OrderBy(s => s.SaleDate))
                    {

                        c = _customerReturnRepository.GetRealSale(getSalePeriode);
                        double CustomerOrderTotalPrice = Util.ExtraPrices(c.SaleLines.Select(sl => sl.LineAmount).Sum(),
                                                                              c.RateReduction, c.RateDiscount, c.Transport, c.VatRate).TotalTTC;

                        //totCustomerOrderTotalPrice = totCustomerOrderTotalPrice + CustomerOrderTotalPrice;
                        User postedby =  db.Users.Find(c.PostByID.Value) ;
                        User validatedBy = (c.OperatorID != null) ? db.Users.Find(c.OperatorID.Value) : null;

                        List<CustomerSlice> lstCustSlice = db.CustomerSlices.Where(sl => sl.SaleID == c.SaleID).ToList();
                        if (lstCustSlice != null && lstCustSlice.Count > 0)
                        {
                            advancedAmount = lstCustSlice.Select(sl => sl.SliceAmount).Sum();
                        }
                        else
                        {
                            advancedAmount = 0;
                        }
                        double pourcentagePayement = (advancedAmount / CustomerOrderTotalPrice);
                        //recuperation du detail de la vente
                        //List<SaleLine> lscustorder = db.SaleLines.Where(co => co.SaleID == c.SaleID).ToList();//.ForEach(o =>
                        //{
                        foreach (SaleLine o in c.SaleLines)
                        {
                            list.Add(
                              new
                              {
                                  RptGeneSaleID = c.CustomerOrderID,
                                  CustomerOrderDate = c.SaleDate,
                                  CustomerOrderTotalPrice = Util.ExtraPrices(o.LineUnitPrice * o.LineQuantity,
                                                                            c.RateReduction, c.RateDiscount, c.Transport, c.VatRate).TotalTTC,
                                  //o.LineUnitPrice * o.LineQuantity,// CustomerOrderTotalPrice / lscustorder.Count,
                                  CustomerOrderNumber = c.SaleReceiptNumber,
                                  CustomerName = c.PersonName,
                                  Agence = c.Branch.BranchCode,
                                  LibAgence = c.Branch.BranchDescription,
                                  Devise = c.Devise.DeviseCode,
                                  LibDevise = c.Devise.DeviseLabel,
                                  CodeClient = c.CustomerName.Trim() + " - " + Resources.e_Purchase + ": " + totCustomerPrice.ToString(),
                                  NomClient = c.CustomerName ,// c.Customer.Name,
                                  OrderStatut = c.StatutSale.ToString(),
                                  Code = c.SaleID,
                                  LineQuantity = o.LineQuantity,
                                  //DeliveredDate = c.DeliveredDate.Value,
                                  //PostedToSupplierDate = c.PostedToSupplierDate.Value,
                                  //SaleDate = c.SaleDate.Date,
                                  //ReceivedDate = c.ReceivedDate.Value,
                                  AdvancedAmount = Math.Floor(Util.ExtraPrices(o.LineUnitPrice * o.LineQuantity, c.RateReduction, c.RateDiscount, c.Transport, c.VatRate).TotalTTC * pourcentagePayement) > Util.ExtraPrices(o.LineUnitPrice * o.LineQuantity, c.RateReduction, c.RateDiscount, c.Transport, c.VatRate).TotalTTC ? Util.ExtraPrices(o.LineUnitPrice * o.LineQuantity, c.RateReduction, c.RateDiscount, c.Transport, c.VatRate).TotalTTC : Math.Floor(Util.ExtraPrices(o.LineUnitPrice * o.LineQuantity, c.RateReduction, c.RateDiscount, c.Transport, c.VatRate).TotalTTC * pourcentagePayement),
                                  ProductID = o.ProductID,
                                  ProductCode = o.Product.ProductCode,
                                  Balance = Math.Floor(Util.ExtraPrices(o.LineUnitPrice * o.LineQuantity, c.RateReduction, c.RateDiscount, c.Transport, c.VatRate).TotalTTC * pourcentagePayement) > Util.ExtraPrices(o.LineUnitPrice * o.LineQuantity, c.RateReduction, c.RateDiscount, c.Transport, c.VatRate).TotalTTC ? 0 : Math.Floor(Util.ExtraPrices(o.LineUnitPrice * o.LineQuantity,
                                                                           c.RateReduction, c.RateDiscount, c.Transport, c.VatRate).TotalTTC) - Math.Floor(Util.ExtraPrices(o.LineUnitPrice * o.LineQuantity,
                                                                           c.RateReduction, c.RateDiscount, c.Transport, c.VatRate).TotalTTC * pourcentagePayement),
                                  PostByID = (postedby == null) ? "" : postedby.Name,
                                  OperatorID = (validatedBy == null) ? "" : validatedBy.Name
                              }
                          );
                        }
                            
                        //}
                        //);
                        //370
                    }
                }
            return list;
        }


        //This method load a method that print 
        public ActionResult PrintReport()
        {
            this.GetCmp<Panel>("PanelReport").LoadContent(new ComponentLoader
            {
                Url = Url.Action("ShowGeneric"),
                DisableCaching = false,
                Mode = LoadMode.Frame,
            });

            return this.Direct();
        }    
        /// This is used for showing Generic Report(with data and report parameter) in a same window       
        public ActionResult ShowGeneric()
        {
            // Clear all sessions value
            Session["ReportName"] = null;
            Session["CompanyName"] = null;
            Session["TelFax"] = null;

            Session["Adresse"] = null;

            Session["RepTitle"] = null;
            Session["Operator"] = null;

            cmpny = db.Companies.FirstOrDefault();

            this.Session["ReportName"] = "RptSpecialOrderCustomer";
            if (cmpny != null)
            {
                this.Session["CompanyName"] = cmpny.Name;
                this.Session["TelFax"] = "Tel: " + cmpny.Adress.AdressPhoneNumber + " Fax : " + cmpny.Adress.AdressFax + " Town :" + cmpny.Adress.Quarter.Town.TownLabel;
                this.Session["Adresse"] = "PO BOX:" + cmpny.Adress.AdressPOBox + " " + cmpny.Adress.Quarter.Town.TownLabel;
            }
            else
            {
                this.Session["CompanyName"] = "NONE";
                this.Session["TelFax"] = "NONE";
            }
            this.Session["RepTitle"] = Resources.RptGeneSale;
            this.Session["Operator"] = CurrentUser.Name;

            DateTime BeginDate = (DateTime)Session["BeginDate"];
            DateTime EndDate = (DateTime)Session["EndDate"];

            this.Session["rptSource"] = ModelRptGeneSale(BeginDate, EndDate);

            return RedirectToAction("ShowGenericRpt", "RptGeneSale");
        }
        public void ShowGenericRpt()
        {
            ReportDocument rptH = new ReportDocument();
            try
            {
                bool isValid = true;

                string strReportName = Session["ReportName"].ToString();    // Setting ReportName
                string stCompanyName1 = Session["CompanyName"].ToString();     // Setting CompanyName1
                string strTelFax1 = Session["TelFax"].ToString();         // Setting TelFax1
                string strRepTitle1 = Session["RepTitle"].ToString();         // Setting RepTitle1
                string strOperator1 = Session["Operator"].ToString();         // Setting Operator1
                string strAdresse1 = Session["Adresse"].ToString();
                DateTime BeginDate = (DateTime)Session["BeginDate"];
                DateTime EndDate = (DateTime)Session["EndDate"];

                var rptSource = Session["rptSource"];

                if (string.IsNullOrEmpty(strReportName) && rptSource == null)
                {
                    isValid = false;
                }

                if (isValid)
                {

                    string strRptPath = System.Web.HttpContext.Current.Server.MapPath("~/") + "Reports//Sales//" + strReportName + ".rpt";
                    rptH.Load(strRptPath);
                    if (rptSource != null && rptSource.GetType().ToString() != "System.String") rptH.SetDataSource(rptSource);
                    if (!string.IsNullOrEmpty(stCompanyName1)) rptH.SetParameterValue("CompanyName", stCompanyName1);
                    if (!string.IsNullOrEmpty(strTelFax1)) rptH.SetParameterValue("TelFax", strTelFax1);
                    if (!string.IsNullOrEmpty(strRepTitle1)) rptH.SetParameterValue("RepTitle", strRepTitle1);
                    if (!string.IsNullOrEmpty(strOperator1)) rptH.SetParameterValue("Operator", strOperator1);

                    if (!string.IsNullOrEmpty(strAdresse1)) rptH.SetParameterValue("Adresse", strAdresse1);

                    if (BeginDate != null) rptH.SetParameterValue("BeginDate", BeginDate);
                    if (EndDate != null) rptH.SetParameterValue("EndDate", EndDate);
                    bool isDownloadRpt = (bool)Session["isDownloadRpt"];
                    rptH.ExportToHttpResponse(ExportFormatType.PortableDocFormat, System.Web.HttpContext.Current.Response, false, strReportName);

                    // Clear all sessions value
                    Session["ReportName"] = null;
                    Session["CompanyName"] = null;
                    Session["TelFax"] = null;
                    Session["RepTitle"] = null;
                    Session["Operator"] = null;
                    Session["accop"] = null;
                    Session["Adresse"] = null;
                    Session["BeginDate"] = null;
                    Session["EndDate"] = null;
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
    }
}