using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using Ext.Net;
using Ext.Net.MVC;
using FatSod.Budget.Entities;
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
using System.Web.UI;

namespace FatSodDental.UI.Areas.Accounting.Controllers
{
    public class RptbudgetExpenseController : BaseController
    {
        //Current Controller and current page
        private const string CONTROLLER_NAME = "Accounting/RptbudgetExpense";
        private const string VIEW_NAME = "Index";
        //person repository


        private Company cmpny;


        //
        // GET: /Sale/RptbudgetExpense/

        [OutputCache(Duration = 3600)]
        public ActionResult Index()
        {

            Session["BeginDate"] = new DateTime(1900, 1, 1);
            Session["EndDate"] = new DateTime(1900, 1, 1);
            Session["BranchID"] = 0;
            Session["DeviseID"] = 0;
            Session["BudgetAllocatedID"] = 0;
            this.chargeSolde();
            return View(ModelExpenseOp());
        }

        public ActionResult DisplayEntries(int BranchID, int DeviseID, DateTime BeginDate, DateTime EndDate, int? BudgetAllocatedID)
        {
            Session["BranchID"] = BranchID;
            Session["DeviseID"] = DeviseID;
            Session["BeginDate"] = BeginDate;
            Session["EndDate"] = EndDate;
            Session["BudgetAllocatedID"] = (BudgetAllocatedID==null) ? 0 : BudgetAllocatedID;
            this.PartialReset();
            this.chargeSolde();
            return this.Direct();
        }
        public void Reset()
        {
            this.GetCmp<FormPanel>("RptbudgetExpense").Reset(true);
            this.PartialReset();
        }
        public void PartialReset()
        {
            this.GetCmp<Store>("Store").Reload();
        }
        [HttpPost]
        public StoreResult GetList()
        {
            return this.Store(ModelExpenseOp());
        }

        private void chargeSolde()
        {
            double Totaldebit = 0d;
            DateTime Bdate = (DateTime)Session["BeginDate"];
            DateTime Edate = (DateTime)Session["EndDate"];
            int BranchID = (int)Session["BranchID"];
            int DeviseID = (int)Session["DeviseID"];
            int BudgetAllocatedID = (int)Session["BudgetAllocatedID"];

            if (BranchID <= 0 || DeviseID <= 0)
            {
                Totaldebit = 0d;
            }
           
            if (BudgetAllocatedID>0)
            {
                var lstExpenseOperations = from bc in db.BudgetConsumptions
                                           where (bc.isValidated && bc.BudgetAllocated.BranchID == BranchID && bc.DeviseID == DeviseID && (bc.DateOperation >= Bdate && bc.DateOperation <= Edate) && bc.BudgetAllocatedID == BudgetAllocatedID)
                                           group bc by new { bc.PaymentDate } into g
                                           select new
                                           {
                                               key = g.Key,
                                               VoucherAmount = g.Sum(a => a.VoucherAmount)
                                           };
                foreach (var exp in lstExpenseOperations.ToList())
                {
                    Totaldebit = Totaldebit + exp.VoucherAmount;
                }
            }
            else
            {
                var lstExpenseOperations = from bc in db.BudgetConsumptions
                                           where (bc.isValidated && bc.BudgetAllocated.BranchID == BranchID && bc.DeviseID == DeviseID && (bc.DateOperation >= Bdate && bc.DateOperation <= Edate))
                                           group bc by new { bc.PaymentDate } into g
                                           select new
                                           {
                                               key = g.Key,
                                               VoucherAmount = g.Sum(a => a.VoucherAmount)
                                           };
                foreach (var exp in lstExpenseOperations.ToList())
                {
                    Totaldebit = Totaldebit + exp.VoucherAmount;
                }
            }
            
            this.GetCmp<TextField>("TotalDebit").Value = Totaldebit;
        }
        private List<object> ModelExpenseOp()
        {
            //int AccountID = 0;

            string UIAccountNumber = "";
            string AccountName = "";

            double TotalCredit = 0d;
            double Totaldebit = 0d;

            List<object> list = new List<object>();
            List<BudgetConsumption> listBudConsume = new List<BudgetConsumption>();
            //recuperation de ttes les operation du type op choisi

            DateTime Bdate = (DateTime)Session["BeginDate"];
            DateTime Edate = (DateTime)Session["EndDate"];
            int BranchID = (int)Session["BranchID"];
            int DeviseID = (int)Session["DeviseID"];
            int BudgetAllocatedID = (int)Session["BudgetAllocatedID"];

            if (BranchID <= 0 || DeviseID <= 0)
            {
                return list;
            }
            string UIBranchCode = db.Branches.Find(BranchID).BranchCode;
            string UIDeviseCode = db.Devises.Find(DeviseID).DeviseCode;

            if (BudgetAllocatedID>0)
            {
                listBudConsume = db.BudgetConsumptions.Where(b => b.isValidated && b.BudgetAllocated.BranchID == BranchID && b.DeviseID == DeviseID && (b.DateOperation >= Bdate && b.DateOperation <= Edate) && b.BudgetAllocatedID == BudgetAllocatedID).OrderBy(a => a.DateOperation).ToList();
            }
            else
            {
                listBudConsume = db.BudgetConsumptions.Where(b => b.isValidated && b.BudgetAllocated.BranchID == BranchID && b.DeviseID == DeviseID && (b.DateOperation >= Bdate && b.DateOperation <= Edate)).OrderBy(a => a.DateOperation).ToList();
            }
            listBudConsume.ForEach(c =>
            {
                list.Add(
                                new
                                {
                                    BudgetAllocatedID = c.BudgetAllocatedID,
                                    UIBudgetAllocated = c.UIBudgetAllocated,
                                    PaymentMethodId = c.PaymentMethodID,
                                    VoucherAmount = c.VoucherAmount,
                                    DateOperation = c.DateOperation,
                                    Reference = c.Reference,
                                    BeneficiaryName = c.BeneficiaryName,
                                    Justification = c.Justification,
                                    BudgetConsumptionID = c.BudgetConsumptionID,
                                    UIBranchCode = UIBranchCode,
                                    UIDeviseCode = UIDeviseCode,
                                    PaymentDate = c.PaymentDate  ,
                                    ValidateBy=(c.ValidateByID ==null)?"":c.ValidateBy.Name,
                                    AutorizeBy = (c.AutorizeByID == null) ? "" : c.AutorizeBy.Name
                                }
                        );
            });
            return list;

        }
        private List<object> ModelRepExpenseOp()
        {
            double Solde = 0d;
            double TotalCredit = 0d;
            double Totaldebit = 0d;

            List<object> list = new List<object>();
            List<BudgetConsumption> listBudConsume = new List<BudgetConsumption>();

            DateTime Bdate = (DateTime)Session["BeginDate"];
            DateTime Edate = (DateTime)Session["EndDate"];
            int BranchID = (int)Session["BranchID"];
            int DeviseID = (int)Session["DeviseID"];
            int BudgetAllocatedID = (int)Session["BudgetAllocatedID"];

            Branch br = db.Branches.Find(BranchID);
            Devise dev = db.Devises.Find(DeviseID);
            cmpny = db.Companies.FirstOrDefault();
            if (BudgetAllocatedID > 0)
            {
                listBudConsume = db.BudgetConsumptions.Where(b => b.isValidated && b.BudgetAllocated.BranchID == BranchID && b.DeviseID == DeviseID && (b.DateOperation >= Bdate && b.DateOperation <= Edate) && b.BudgetAllocatedID == BudgetAllocatedID).OrderBy(a => a.DateOperation).ToList();
            }
            else
            {
                listBudConsume = db.BudgetConsumptions.Where(b => b.isValidated && b.BudgetAllocated.BranchID == BranchID && b.DeviseID == DeviseID && (b.DateOperation >= Bdate && b.DateOperation <= Edate)).OrderBy(a => a.DateOperation).ToList();
            }
            listBudConsume.ForEach(c =>
            {
                list.Add(
                                new
                                {
                                    BudgetAllocatedID = c.BudgetAllocatedID,
                                    UIBudgetAllocated = c.UIBudgetAllocated,
                                    PaymentMethodId = c.PaymentMethodID.Value,
                                    VoucherAmount = c.VoucherAmount,
                                    DateOperation = c.DateOperation,
                                    Reference = c.Reference,
                                    BeneficiaryName = c.BeneficiaryName,
                                    Justification = c.Justification,
                                    BudgetConsumptionID = c.BudgetConsumptionID,
                                    Agence=br.BranchCode,
                                    LibAgence = br.BranchName,
                                    Devise=dev.DeviseCode,
                                    LibDevise = dev.DeviseLabel,
                                    PaymentDate = c.PaymentDate.Value
                                }
                        );
            });

            return list;

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
                    
                    string strRptPath = System.Web.HttpContext.Current.Server.MapPath("~/") + "Reports//Accounting//" + strReportName + ".rpt";
                    rptH.Load(strRptPath);
                    if (rptSource != null && rptSource.GetType().ToString() != "System.String") rptH.SetDataSource(rptSource);
                    if (!string.IsNullOrEmpty(stCompanyName1)) rptH.SetParameterValue("BranchName", stCompanyName1);
                    if (!string.IsNullOrEmpty(strTelFax1)) rptH.SetParameterValue("BranchInfo", "Region "+strRegionCountry1 + " "+ strAdresse1); //+ strTelFax1);
                    if (!string.IsNullOrEmpty(strRepTitle1)) rptH.SetParameterValue("RepTitle", strRepTitle1);
                    if (!string.IsNullOrEmpty(strOperator1)) rptH.SetParameterValue("Operator", strOperator1);

                    //if (!string.IsNullOrEmpty(strRegionCountry1)) rd.SetParameterValue("RegionCountry", strRegionCountry1);
                    //if (!string.IsNullOrEmpty(strAdresse1)) rd.SetParameterValue("Adresse", strAdresse1);

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

            cmpny = db.Companies.FirstOrDefault();
            this.Session["ReportName"] = "RptbudgetExpense";
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
            this.Session["RepTitle"] = Resources.RptbudgetExpenseTitle;
            this.Session["Operator"] = CurrentUser.Name;
            this.Session["rptSource"] = ModelRepExpenseOp();
            
            return RedirectToAction("ShowGenericRpt", "RptbudgetExpense");
        }
    }
}