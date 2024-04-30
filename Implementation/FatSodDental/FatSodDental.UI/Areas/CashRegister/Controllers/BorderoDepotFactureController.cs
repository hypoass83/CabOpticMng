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
using FatSodDental.UI.Filters;
using FatSodDental.UI.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace FatSodDental.UI.Areas.CashRegister.Controllers
{
    [Authorize(Order = 1)]
    [TakeBusinessDay(Order = 2)]
    public class BorderoDepotFactureController : BaseController
    {
        //Current Controller and current page
        private const string CONTROLLER_NAME = "CashRegister/BorderoDepotFacture";
        private const string VIEW_NAME = "Index";

        private List<BusinessDay> lstBusDay;

        private ITillDay _tillDayRepository;
        private IBusinessDay _busDayRepo;
        private ICustomerOrder _CORepository;
        
        public BorderoDepotFactureController(
				 ITillDay tillDayRepository,
				 IBusinessDay busDayRepo,
                 ICustomerOrder CoRepository
				)
		{
			this._tillDayRepository = tillDayRepository;
			this._busDayRepo = busDayRepo;
            this._CORepository = CoRepository;
		}
        // GET: CashRegister/BorderoDepotFacture
        [OutputCache(Duration = 3600)] 
        public ActionResult Index()
        {
            try
            {

                //we ensure that if this user manage cash register. If he manage it, will verify if till is closed else, we ask he to closed it before login off
                UserTill userTill = (from td in db.UserTills
                                     where td.UserID == SessionGlobalPersonID
                                     select td).SingleOrDefault();
                if (userTill == null || userTill.TillID <= 0)
                {
                    X.Msg.Alert("Access Denied", "You can't do sale operation. If you think no, try again or<br/>contact our administrator for this purpose<code/>.").Show();
                    return this.Direct();
                }
                List<BusinessDay> UserBusDays = (List<BusinessDay>)Session["UserBusDays"];
                DateTime currentDateOp = UserBusDays.FirstOrDefault().BDDateOperation;
                ViewBag.CurrentBranch = UserBusDays.FirstOrDefault().BranchID;
                ViewBag.BusnessDayDate = UserBusDays.FirstOrDefault().BDDateOperation;// businessDay.BDDateOperation;

                TillDayStatus tState = _tillDayRepository.TillDayStatus(userTill.Till);
                if (tState == null)
                {
                    X.Msg.Alert("Error", "Bad Configuration of Cash Register!!! Please call Your database Administrator").Show();
                    return this.Direct();
                }
                if (!tState.IsOpen)
                {
                    X.Msg.Alert("Error", "This Cash Register is Still Close!!! Please Open It Before Proceed").Show();
                    return this.Direct();
                }

                TillDay currentTillDay = (from t in db.TillDays
                                          where
                                              t.TillID == userTill.TillID && t.TillDayDate == tState.TillDayLastOpenDate.Date && t.IsOpen // t.TillDayDate.Day == currentDateOp.Day && t.TillDayDate.Month == currentDateOp.Month && t.TillDayDate.Year == currentDateOp.Year && t.IsOpen
                                          select t).FirstOrDefault();
                if (currentTillDay == null)
                {
                    X.Msg.Alert("Warnnig", "Cash register is closed. You must open it before do any sale<br/>Go at Cash Register module=>Open cash register<code/>").Show();
                    return this.Direct();
                }

                ViewBag.CurrentTill = currentTillDay.TillID;

                Session["BeginDate"] = new DateTime(1900, 1, 1);
                Session["EndDate"] = new DateTime(1900, 1, 1);
                return View(ModelBillInsure(0));
            }

            catch (Exception e)
            {
                X.Msg.Alert("Error ", e.Message).Show();
                return this.Direct();
            }
        }

        public ActionResult DisplayBill(int AssureurID, DateTime BeginDate, DateTime EndDate)
        {
            Session["AssureurID"] = AssureurID;
            Session["BeginDate"] = BeginDate;
            Session["EndDate"] = EndDate;
            this.PartialReset();
            return this.Direct();
        }
       
        public void PartialReset()
        {
            this.GetCmp<Store>("Store").Reload();
        }
        [HttpPost]
        public StoreResult GetList()
        {
            return this.Store(ModelBillInsure((int)Session["AssureurID"]));
        }
        private List<object> ModelBillInsure(int AssureurID) //, string BeginDate, string EndDate)
        {
            List<object> list = new List<object>();
            //recuperation de ttes les operation du type op choisi

            DateTime Bdate = (DateTime)Session["BeginDate"];
            DateTime Edate = (DateTime)Session["EndDate"];
            int BranchID = SessionBusinessDay(null).BranchID;

            List<CustomerOrder> listBillInsuredOp = db.CustomerOrders.Where(co => co.BranchID == BranchID && (co.CustomerOrderDate >= Bdate && co.CustomerOrderDate <= Edate) && co.AssureurID == AssureurID && co.BillState==StatutFacture.Validated).ToList();

            listBillInsuredOp.ForEach(c =>
            {
                list.Add(
                            new
                            {
                                CustomerOrderID = c.CustomerOrderID,
                                BranchID = c.BranchID,
                                CustomerName = c.CustomerName,
                                CompanyName = c.CompanyName,
                                CustomerOrderDate = c.CustomerOrderDate,
                                UIBranchCode = c.Branch.BranchName,
                                CustomerOrderNumber = c.CustomerOrderNumber,
                                NumeroBonPriseEnCharge = c.NumeroBonPriseEnCharge,
                                NumeroFacture = c.NumeroFacture,
                                PhoneNumber = c.PhoneNumber,
                                MntAssureur = c.Plafond,
                                ReductionAmount = c.MntValidate,
                                InsuranceCompany=c.Assureur.Name
                            }
                        );

            });
           
            return list;

        }
        public StoreResult LoadThirdPartyAccounts(int? BranchID)
        {
            List<object> Assureurs = new List<object>();
            List<Assureur> Assureurs1 = new List<Assureur>();
            Assureurs1 = db.Assureurs.Where(ass=>ass.Name.ToLower()!="default").ToList();
            foreach (Assureur c in Assureurs1)
            {
                Assureurs.Add(
                    new
                    {
                        AssureurFullName = c.AssureurFullName,
                        PersonID = c.GlobalPersonID
                    }
                );

            }
            return this.Store(Assureurs);
        }
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

        //for editing

        [HttpPost]
        public DirectResult Edit(int id, string field, string oldValue, string newValue, object customer)
        {
            CustomerOrder custOrder = new CustomerOrder();
            custOrder = db.CustomerOrders.Find(id);
            if (custOrder.CustomerOrderID > 0)
            {
                custOrder.MntValidate = Convert.ToDouble(newValue);
                _CORepository.Update(custOrder,custOrder.CustomerOrderID);

                PartialReset();
            }

            return this.Direct();
        }

        //for printing
        

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

            Session["RegionCountry"] = null;
            Session["Adresse"] = null;

            Session["RepTitle"] = null;
            Session["Operator"] = null;

            Company cmpny = db.Companies.FirstOrDefault();
            int AssureurID = (int)Session["AssureurID"];
            this.Session["ReportName"] = "RptBorderoDepotFacture";
            if (cmpny != null)
            {
                this.Session["CompanyName"] = cmpny.Name;
                this.Session["TelFax"] = "Tel: " + cmpny.Adress.AdressPhoneNumber + " Fax : " + cmpny.Adress.AdressFax + " Town :" + cmpny.Adress.Quarter.Town.TownLabel;
                this.Session["RegionCountry"] = cmpny.Adress.Quarter.Town.Region.RegionLabel;
                this.Session["Adresse"] = "PO BOX:" + cmpny.Adress.AdressPOBox + " " + cmpny.Adress.Quarter.Town.TownLabel;
            }
            else
            {
                this.Session["CompanyName"] = "NONE";
                this.Session["TelFax"] = "NONE";
            }
            this.Session["RepTitle"] = Resources.BorderoDepotFacture;
            this.Session["Operator"] = CurrentUser.Name;
            if (AssureurID != null)
            {
                this.Session["rptSource"] = ModelBillInsure(AssureurID);
            }
            else
            {
                this.Session["rptSource"] = null;
            }

            return RedirectToAction("ShowGenericRpt");
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
                string strRegionCountry1 = Session["RegionCountry"].ToString();
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

                    string strRptPath = System.Web.HttpContext.Current.Server.MapPath("~/") + "Reports//CashRegister//" + strReportName + ".rpt";
                    rptH.Load(strRptPath);
                    if (rptSource != null && rptSource.GetType().ToString() != "System.String") rptH.SetDataSource(rptSource);
                    if (!string.IsNullOrEmpty(stCompanyName1)) rptH.SetParameterValue("CompanyName", stCompanyName1);
                    if (!string.IsNullOrEmpty(strTelFax1)) rptH.SetParameterValue("TelFax", strTelFax1);
                    if (!string.IsNullOrEmpty(strRepTitle1)) rptH.SetParameterValue("RepTitle", strRepTitle1);
                    if (!string.IsNullOrEmpty(strOperator1)) rptH.SetParameterValue("Operator", strOperator1);

                    if (!string.IsNullOrEmpty(strRegionCountry1)) rptH.SetParameterValue("RegionCountry", strRegionCountry1);
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
                    Session["AssureurID"] = null;
                    Session["RegionCountry"] = null;
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