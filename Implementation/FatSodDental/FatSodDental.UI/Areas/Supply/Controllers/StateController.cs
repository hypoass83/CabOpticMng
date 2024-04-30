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
using FatSod.DataContext.Concrete;

namespace FatSodDental.UI.Areas.Supply.Controllers
{
    [Authorize(Order = 1)]
    [TakeBusinessDay(Order = 2)]
    public class StateController : BaseController
    {

        // GET: Supply/State
        public ActionResult Index()
        {
            return View();
        }
        //This method print inentory of day
        public ActionResult GenerateInventoryReport()
        {
            List<object> model = new List<object>();
            ReportDocument rptH = new ReportDocument();
            string defaultImgPath = Server.MapPath("~/Content/Images/App/deboy-logo.png");
            byte[] COMPANY_LOGO = System.IO.File.ReadAllBytes(defaultImgPath);
            
            List<ProductLocalization> dataTmp = (from prodloc in db.ProductLocalizations
                           select prodloc)
                               .OrderBy(a => a.Product.ProductCode)
                                .ToList();
            string strOperator1 = CurrentUser.Name;
            //List<object> realDataTmp = new List<object>();
            //int companyID = CurrentUser.UserBranches.First().Branch.CompanyID;
            foreach (ProductLocalization p in dataTmp)
            {

                model.Add(
                    new
                    {
                        Localization = p.LocalizationLabel,
                        ProductLabel = p.ProductCode,
                        ProductQty = p.ProductLocalizationStockQuantity,
                        ProductUnitPrice = p.ProductLocalizationStockSellingPrice,
                        ProductLocalizationSafetyStockQuantity = p.ProductLocalizationSafetyStockQuantity,
                        Amount = p.Amount,
                        BranchName = p.Localization.Branch.BranchName,
                        IsSafQuantStockReached = p.IsSafQuantStockReached,
                        BranchAdress = p.Localization.Branch.Adress.AdressEmail + "/" + p.Localization.Branch.Adress.AdressPOBox,
                        BranchTel = p.Localization.Branch.Adress.AdressPhoneNumber,
                        CompanyName = Company.Name,
                        CompanyAdress = Company.Adress.AdressEmail +"/"+ Company.Adress.AdressPOBox,
                        CompanyTel = Company.Adress.AdressPhoneNumber,
                        CompanyCNI = Company.CNI,
                        CompanyLogo = db.Files.FirstOrDefault(f => f.GlobalPersonID == Company.GlobalPersonID) != null ? db.Files.FirstOrDefault(f => f.GlobalPersonID == Company.GlobalPersonID).Content : COMPANY_LOGO
                    }
                    );
            }
            string path = Server.MapPath("~/Reports/Supply/RptInventory.rpt");
            rptH.Load(path);
            rptH.SetDataSource(model);
            if (!string.IsNullOrEmpty(strOperator1)) rptH.SetParameterValue("Operator", strOperator1);
            Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            return File(stream, "application/pdf");
        }
        private Company Company
        {
            get
            {
                return db.Companies.FirstOrDefault();
            }
        }
    }
}