using FatSod.Report.WrapQuery;
using FatSod.Ressources;
using FatSod.Security.Abstracts;
using FatSod.Security.Entities;
using FatSod.Supply.Abstracts;
using FatSod.Supply.Entities;
using CABOPMANAGEMENT.Controllers;
using CABOPMANAGEMENT.Filters;
using CABOPMANAGEMENT.Tools;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using SaleE = FatSod.Supply.Entities.Sale;
using FastSod.Utilities.Util;

namespace CABOPMANAGEMENT.Areas.CashRegister.Controllers
{
    [Authorize(Order = 1)]
    [TakeBusinessDay(Order = 2)]
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
        private IDeposit _depositRepo;
        
        public RptGeneSaleController(
            ISale sale,
            IBusinessDay busDayRepo,
            ICustomerReturn customerReturnRepository,
			IRepository<FatSod.Security.Entities.File> fileRepository,
            IDeposit depositRepo
            )
        {
            this._saleRepository = sale;
            this._busDayRepo = busDayRepo;
			this._fileRepository = fileRepository;
            this._customerReturnRepository = customerReturnRepository;
            this._depositRepo = depositRepo;
        }
        //
        // GET: /CashRegister/RptGeneSale/
        [OutputCache(Duration = 3600)]
        public ActionResult Index()
        {
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

        public JsonResult chargeSolde(int BranchID,DateTime Bdate, DateTime Edate)
        {
            List<object> _InfoList = new List<object>();
            double Totaldebit = 0d;
            double TotalAdvanced = 0d;
            double advancedAmount = 0d;
            //List<CalculSodeGenerale> res = _depositRepo.CalculSodeGenerale(BranchID,Bdate, Edate).ToList();
            //if (res.Count > 0)
            //{
            //    Totaldebit = res.Select(c => c.TotalDebit).Sum();
            //    TotalAdvanced = res.Select(c => c.TotalAdvanced).Sum();
            //}

            SaleE c = new SaleE();

            List<SaleE> lstCustoSaleAmt = _saleRepository.FindAll.Where(so => (so.SaleDate.Date >= Bdate.Date && so.SaleDate.Date <= Edate) && (!so.IsSpecialOrder)).ToList();
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
            _InfoList.Add(new
            {
                TotalDebit = Totaldebit,
                TotalAdvanced = TotalAdvanced
            });
            return Json(_InfoList, JsonRequestBehavior.AllowGet);
            
        }

        //affiche la liste des
        public JsonResult ModelRptGeneSale(int BranchID, DateTime Begindate, DateTime EndDate)
        {

            
            double advancedAmount = 0d;
            List<TabModelRptGeneSale> listGenSale = new List<TabModelRptGeneSale>();

            var lstCustomer = _saleRepository.FindAll.Where(so => (so.SaleDate.Date >= Begindate.Date && so.SaleDate.Date <= EndDate.Date) && (!so.IsSpecialOrder))
            .Select(s => new
            {
                CustomerID = s.CustomerID
            }).Distinct().ToList();

            foreach (var custo in lstCustomer.OrderBy(o => o.CustomerID))
            {
                double totCustomerPrice = 0d;
                SaleE c = new SaleE();
                List<SaleE> lstCustoSale = _saleRepository.FindAll.Where(so => (so.SaleDate.Date >= Begindate.Date && so.SaleDate.Date <= EndDate.Date) && so.CustomerID == custo.CustomerID && (!so.IsSpecialOrder)).ToList();
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

                    User postedby = db.Users.Find(c.PostByID.Value);
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
                   
                    foreach (SaleLine o in c.SaleLines)
                    {
                        listGenSale.Add(
                          new TabModelRptGeneSale
                          {
                              TabModelRptGeneSaleID = c.SaleID,
                              CustomerOrderDate = c.SaleDate,
                              CustomerOrderTotalPrice = (float) Util.ExtraPrices(o.LineUnitPrice * o.LineQuantity,
                                                                        c.RateReduction, c.RateDiscount, c.Transport, c.VatRate).TotalTTC,
                              CustomerOrderNumber = c.SaleReceiptNumber,
                              CustomerName = c.PersonName,
                              IsInHouseCustomer = c.Customer.IsInHouseCustomer ? "YES" : "NO",
                              Agence = c.Branch.BranchCode,
                              LibAgence = c.Branch.BranchDescription,
                              Devise = c.Devise.DeviseCode,
                              LibDevise = c.Devise.DeviseLabel,
                              CodeClient = c.CustomerName.Trim() + " - " + Resources.e_Purchase + ": " + totCustomerPrice.ToString(),
                              NomClient = c.CustomerName,// c.Customer.Name,
                              
                              OrderStatut = c.StatutSale.ToString(),
                              SaleID = c.SaleID,
                              LineQuantity = (float)o.LineQuantity,
                              
                              AdvancedAmount = Math.Floor(Util.ExtraPrices(o.LineUnitPrice * o.LineQuantity, c.RateReduction, c.RateDiscount, c.Transport, c.VatRate).TotalTTC * pourcentagePayement) > Util.ExtraPrices(o.LineUnitPrice * o.LineQuantity, c.RateReduction, c.RateDiscount, c.Transport, c.VatRate).TotalTTC ? (float)Util.ExtraPrices(o.LineUnitPrice * o.LineQuantity, c.RateReduction, c.RateDiscount, c.Transport, c.VatRate).TotalTTC : (float)Math.Floor(Util.ExtraPrices(o.LineUnitPrice * o.LineQuantity, c.RateReduction, c.RateDiscount, c.Transport, c.VatRate).TotalTTC * pourcentagePayement),
                              ProductID = o.ProductID,
                              ProductCode = o.Product.ProductCode,
                              Balance = Math.Floor(Util.ExtraPrices(o.LineUnitPrice * o.LineQuantity, c.RateReduction, c.RateDiscount, c.Transport, c.VatRate).TotalTTC * pourcentagePayement) > Util.ExtraPrices(o.LineUnitPrice * o.LineQuantity, c.RateReduction, c.RateDiscount, c.Transport, c.VatRate).TotalTTC ? 0 : (float)(Math.Floor(Util.ExtraPrices(o.LineUnitPrice * o.LineQuantity,
                                                                       c.RateReduction, c.RateDiscount, c.Transport, c.VatRate).TotalTTC) - Math.Floor(Util.ExtraPrices(o.LineUnitPrice * o.LineQuantity,
                                                                       c.RateReduction, c.RateDiscount, c.Transport, c.VatRate).TotalTTC * pourcentagePayement)),
                              PostByID = (postedby == null) ? "" : postedby.UserFullName,
                              OperatorID = (validatedBy == null) ? "" : validatedBy.UserFullName
                          }
                      );
                    }

                }
            }

            var list = new
            {
                data = from c in listGenSale.ToList().OrderBy(c => c.NomClient)
                select
                new
                {
                    RptGeneSaleID = c.TabModelRptGeneSaleID,
                    CustomerOrderDate = c.CustomerOrderDate.ToString("yyyy-MM-dd"),//.Day + "/" + c.CustomerOrderDate.Month + "/" + c.CustomerOrderDate.Year,
                    CustomerOrderTotalPrice = c.CustomerOrderTotalPrice,
                    CustomerOrderNumber = c.CustomerOrderNumber,
                    CustomerName = c.CustomerName,
                    IsInHouseCustomer = c.IsInHouseCustomer,
                    Agence = c.Agence,
                    LibAgence = c.LibAgence,
                    Devise = c.Devise,
                    LibDevise = c.LibDevise,
                    CodeClient = c.CodeClient,//c.CustomerName.Trim() + " - " + Resources.e_Purchase + ": " + c.CustomerOrderTotalPrice.ToString(),
                    NomClient = c.NomClient,//CustomerName,
                    //OrderStatut = c.StatutSale,
                    Code = c.SaleID,
                    LineQuantity = c.LineQuantity,
                    AdvancedAmount = c.AdvancedAmount,
                    ProductID = c.ProductID,
                    ProductCode = c.ProductCode,
                    Balance = c.Balance,
                    PostByID = (c.PostByID == null) ? "" : LoadComponent.Left(c.PostByID, 8),
                    OperatorID = (c.OperatorID == null) ? "" : LoadComponent.Left(c.OperatorID, 8)
                }
            };
            var jsonResult = Json(list, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = Int32.MaxValue;
            return jsonResult;

        }


        ////This method load a method that print 
        //public ActionResult PrintReport()
        //{
        //    this.GetCmp<Panel>("PanelReport").LoadContent(new ComponentLoader
        //    {
        //        Url = Url.Action("ShowGeneric"),
        //        DisableCaching = false,
        //        Mode = LoadMode.Frame,
        //    });

        //    return this.Direct();
        //}    
        ///// This is used for showing Generic Report(with data and report parameter) in a same window       
        //public ActionResult ShowGeneric()
        //{
        //    // Clear all sessions value
        //    Session["ReportName"] = null;
        //    Session["CompanyName"] = null;
        //    Session["TelFax"] = null;

        //    Session["Adresse"] = null;

        //    Session["RepTitle"] = null;
        //    Session["Operator"] = null;

        //    cmpny = db.Companies.FirstOrDefault();

        //    this.Session["ReportName"] = "RptSpecialOrderCustomer";
        //    if (cmpny != null)
        //    {
        //        this.Session["CompanyName"] = cmpny.Name;
        //        this.Session["TelFax"] = "Tel: " + cmpny.Adress.AdressPhoneNumber + " Fax : " + cmpny.Adress.AdressFax + " Town :" + cmpny.Adress.Quarter.Town.TownLabel;
        //        this.Session["Adresse"] = "PO BOX:" + cmpny.Adress.AdressPOBox + " " + cmpny.Adress.Quarter.Town.TownLabel;
        //    }
        //    else
        //    {
        //        this.Session["CompanyName"] = "NONE";
        //        this.Session["TelFax"] = "NONE";
        //    }
        //    this.Session["RepTitle"] = Resources.RptGeneSale;
        //    this.Session["Operator"] = CurrentUser.Name;

        //    DateTime BeginDate = (DateTime)Session["BeginDate"];
        //    DateTime EndDate = (DateTime)Session["EndDate"];

        //    this.Session["rptSource"] = ModelRptGeneSale(BeginDate, EndDate);

        //    return RedirectToAction("ShowGenericRpt", "RptGeneSale");
        //}
        //public void ShowGenericRpt()
        //{
        //    ReportDocument rptH = new ReportDocument();
        //    try
        //    {
        //        bool isValid = true;

        //        string strReportName = Session["ReportName"].ToString();    // Setting ReportName
        //        string stCompanyName1 = Session["CompanyName"].ToString();     // Setting CompanyName1
        //        string strTelFax1 = Session["TelFax"].ToString();         // Setting TelFax1
        //        string strRepTitle1 = Session["RepTitle"].ToString();         // Setting RepTitle1
        //        string strOperator1 = Session["Operator"].ToString();         // Setting Operator1
        //        string strAdresse1 = Session["Adresse"].ToString();
        //        DateTime BeginDate = (DateTime)Session["BeginDate"];
        //        DateTime EndDate = (DateTime)Session["EndDate"];

        //        var rptSource = Session["rptSource"];

        //        if (string.IsNullOrEmpty(strReportName) && rptSource == null)
        //        {
        //            isValid = false;
        //        }

        //        if (isValid)
        //        {

        //            string strRptPath = System.Web.HttpContext.Current.Server.MapPath("~/") + "Reports//Sales//" + strReportName + ".rpt";
        //            rptH.Load(strRptPath);
        //            if (rptSource != null && rptSource.GetType().ToString() != "System.String") rptH.SetDataSource(rptSource);
        //            if (!string.IsNullOrEmpty(stCompanyName1)) rptH.SetParameterValue("CompanyName", stCompanyName1);
        //            if (!string.IsNullOrEmpty(strTelFax1)) rptH.SetParameterValue("TelFax", strTelFax1);
        //            if (!string.IsNullOrEmpty(strRepTitle1)) rptH.SetParameterValue("RepTitle", strRepTitle1);
        //            if (!string.IsNullOrEmpty(strOperator1)) rptH.SetParameterValue("Operator", strOperator1);

        //            if (!string.IsNullOrEmpty(strAdresse1)) rptH.SetParameterValue("Adresse", strAdresse1);

        //            if (BeginDate != null) rptH.SetParameterValue("BeginDate", BeginDate);
        //            if (EndDate != null) rptH.SetParameterValue("EndDate", EndDate);
        //            bool isDownloadRpt = (bool)Session["isDownloadRpt"];
        //            rptH.ExportToHttpResponse(ExportFormatType.PortableDocFormat, System.Web.HttpContext.Current.Response, false, strReportName);

        //            // Clear all sessions value
        //            Session["ReportName"] = null;
        //            Session["CompanyName"] = null;
        //            Session["TelFax"] = null;
        //            Session["RepTitle"] = null;
        //            Session["Operator"] = null;
        //            Session["accop"] = null;
        //            Session["Adresse"] = null;
        //            Session["BeginDate"] = null;
        //            Session["EndDate"] = null;
        //        }
        //        else
        //        {
        //            Response.Write("Nothing Found; No Report name found");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Response.Write(ex.ToString());
        //    }
        //    finally
        //    {
        //        rptH.Close();
        //        rptH.Dispose();
        //    }
        //}
    }
}