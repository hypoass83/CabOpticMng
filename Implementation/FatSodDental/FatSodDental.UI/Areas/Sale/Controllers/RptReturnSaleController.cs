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
using ExtPartialViewResult = Ext.Net.MVC.PartialViewResult;

namespace FatSodDental.UI.Areas.Sale.Controllers
{
    public class RptReturnSaleController : BaseController
    {
        //Current Controller and current page
        private const string CONTROLLER_NAME = "Sale/RptReturnSale";
        private const string VIEW_NAME = "Index";
        //person repository
        
        private IAccountOperation _accountOperationRepository;
        private IRepositorySupply<Operation> _operationRepository;
       
        private Company cmpny;
        
        //
        // GET: /Sale/RptReturnSale/
        [OutputCache(Duration = 3600)] 
        public ActionResult Index()
        {

            //Session["Curent_Page"] = VIEW_NAME;
            //Session["Curent_Controller"] = CONTROLLER_NAME;

            //ExtPartialViewResult rPVResult = new ExtPartialViewResult
            //{
            //    ClearContainer = true,
            //    //RenderMode = RenderMode.AddTo,
            //    ContainerId = "Body",
            //    //WrapByScriptTag = false,
            //    Model = ModelSaleReturn(new DateTime(1900, 1, 1), new DateTime(1900, 1, 1))
            //};

            //We test if this user has an autorization to get this sub menu
            //if (!LoadAction.isAutoriseToGetSubMenu(SessionProfileID, CodeValue.Sale.Report.SRCODERETSALE, db))
            //{
            //    RedirectToAction("NotAuthorized", "User");
            //}

            Session["BeginDate"] = new DateTime(1900, 1, 1);
            Session["EndDate"] = new DateTime(1900, 1, 1);

            return View(ModelSaleReturn(new DateTime(1900, 1, 1), new DateTime(1900, 1, 1)));
        }
        
        public ActionResult DisplayEntries(DateTime BeginDate, DateTime EndDate)
        {
            Session["BeginDate"] = BeginDate;
            Session["EndDate"] = EndDate;
            this.PartialReset();
            return this.Direct();
        }
        public void Reset()
        {
            this.GetCmp<FormPanel>("RptReturnSale").Reset(true);
            this.PartialReset();
        }
        public void PartialReset()
        {
            this.GetCmp<Store>("Store").Reload();
        }
        [HttpPost]
        public StoreResult GetList(DateTime Bdate, DateTime Edate)
        {

            Session["BeginDate"] = Bdate;
            Session["EndDate"] = Edate;

            return this.Store(ModelSaleReturn(Bdate,Edate));
        }

        //affiche la liste des
        private List<object> ModelSaleReturn(DateTime Bdate,DateTime Edate) //, string BeginDate, string EndDate)
        {
            List<object> list = new List<object>();
            if (Bdate==new DateTime(1900,01,01) && Edate==new DateTime(1900,01,01))
            {
                return list;
            }
            //retourne la liste des retour sur vente pr la periode de date choisi
            
            //produit de verre
            var query = db.CustomerReturns.Join(db.CustomerReturnLines, cr => cr.CustomerReturnID, crln => crln.CustomerReturnID,
                (cr, crln) => new { cr, crln }).
                Join(db.SaleLines, crsl => crsl.crln.SaleLineID, sal => sal.LineID, (crsl, sal) => new { crsl, sal })
                .Where(crsaline => crsaline.crsl.crln.CustomerReturnDate >= Bdate.Date && crsaline.crsl.crln.CustomerReturnDate <= Edate.Date)
                .Select(s => new
                {
                    CustomerReturnDate = s.crsl.crln.CustomerReturnDate,
                    CustomerReturnID = s.crsl.cr.CustomerReturnID,
                    CustomerReturnCauses = s.crsl.crln.CustomerReturnCauses,
                    LineQuantity=s.crsl.crln.LineQuantity,
                    LineAmount=s.sal.LineUnitPrice,
                    Localization=s.sal.Localization,
                    OeilDroiteGauche=s.sal.OeilDroiteGauche,
                    Product=s.sal.Product,
                    Sale = s.crsl.cr.Sale,
                    CustomerID = s.crsl.cr.Sale.CustomerID,
                    RateReduction = s.crsl.cr.Sale.RateReduction,
                    RateDiscount = s.crsl.cr.Sale.RateDiscount,
                    Transport = s.crsl.cr.Sale.Transport,
                    VatRate = s.crsl.cr.Sale.VatRate,
                })
                .AsQueryable()
                .ToList();

            foreach (var c in query.OrderBy(cr => cr.CustomerReturnDate).ThenBy(cr => cr.CustomerID))
            {
                double returnAmnt = c.LineQuantity * c.LineAmount;
                ExtraPrice extra = Util.ExtraPrices(returnAmnt, c.RateReduction, c.RateDiscount, c.Transport, c.VatRate);
                list.Add(
                            new
                            {
                                RptReturnSaleID = c.CustomerReturnID,
                                Agence = c.Localization.Branch.BranchCode,
                                LibAgence = c.Localization.Branch.BranchDescription,
                                Devise = c.Sale.Devise.DeviseCode,
                                LibDevise = c.Sale.SaleReceiptNumber,
                                CodeClient = c.Sale.Customer.CNI,
                                NomClient = c.Sale.Customer.Name,
                                CustomerReturnCauses = c.CustomerReturnCauses,
                                LineQuantity = c.LineQuantity,
                                LineAmount = c.LineAmount,
                                ReturnAmount = extra.TotalTTC,// c.LineQuantity*c.LineAmount,
                                LocalizationCode = c.Localization.LocalizationCode,
                                ProductCode = c.Product.ProductCode,
                                OeilDroiteGauche = c.OeilDroiteGauche,
                                CustomerReturnDate = c.CustomerReturnDate
                            }
                );
            };

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
            
            this.Session["ReportName"] = "RptReturnSale";
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
            this.Session["RepTitle"] = Resources.RptReturnSale;
            this.Session["Operator"] = CurrentUser.Name;

            DateTime BeginDate = (DateTime)Session["BeginDate"];
            DateTime EndDate = (DateTime)Session["EndDate"];

            this.Session["rptSource"] = ModelSaleReturn(BeginDate, EndDate);
            
            return RedirectToAction("ShowGenericRpt", "RptReturnSale");
        }
    }
}