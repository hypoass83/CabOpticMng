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
using FatSodDental.UI.Filters;
using FatSod.DataContext.Concrete;
using CrystalDecisions.CrystalReports.Engine;
using System.IO;
using System.Web.UI;

namespace FatSodDental.UI.Areas.Supply.Controllers
{
    [Authorize(Order = 1)]
    [TakeBusinessDay(Order = 2)]
    public class InventoryController : BaseController
    {
        //Current Controller and current page
        private const string CONTROLLER_NAME = "Supply/" + CodeValue.Supply.Inventory_SM.CONTROLLER;
        private const string VIEW_NAME = CodeValue.Supply.Inventory_SM.PATH;
        //person repository
        private IProductLocalization _plRepository;
        private IBusinessDay _busDayRepo;

        private IRepository<FatSod.Security.Entities.File> _fileRepository;
        private ILensNumberRangePrice _priceRepository;
        
        //Construcitor
        public InventoryController(
            ILensNumberRangePrice lnrpRepo,
            IBusinessDay busDayRepo, 
            IProductLocalization plRepository,
            IRepository<FatSod.Security.Entities.File> fileRepository
            )
        {
            this._plRepository = plRepository;
            this._busDayRepo = busDayRepo;
            this._fileRepository = fileRepository;
            this._priceRepository = lnrpRepo;
        }
        // GET: Sale/Sale Return all a sales historique add enable to create a new sale
        [OutputCache(Duration = 3600)] 
        public ActionResult Inventory()
        {
            //Session["Curent_Page"] = VIEW_NAME;
            //Session["Curent_Controller"] = CONTROLLER_NAME;

            //We test if this user has an autorization to get this sub menu
            //if (!LoadAction.isAutoriseToGetMenu(SessionProfileID, CodeValue.Supply.SupplyMenu.CODE, db))
            //{
            //    RedirectToAction("NotAuthorized", "User");
            //}

            //this.GetCmp<Panel>("Body").LoadContent(new ComponentLoader
            //{
            //    Url = Url.Action(VIEW_NAME),
            //    DisableCaching = false,
            //    Mode = LoadMode.Frame
            //});

            //Ext.Net.MVC.PartialViewResult rPVResult = new Ext.Net.MVC.PartialViewResult
            //{
            //    ClearContainer = true,
            //    //RenderMode = RenderMode.AddTo,
            //    ContainerId = "Body",
            //    //WrapByScriptTag = false,
            //    Model = ModelPL("", "")
            //};

            //return rPVResult;

            return View(ModelPL("", ""));
        }
        [HttpPost]
        public DirectResult Edit(int id, string field, string oldValue, string newValue, object customer)
        {
            DateTime serverdate = SessionBusinessDay(null).BDDateOperation;
            ProductLocalization prodLoc = new ProductLocalization();
            prodLoc = db.ProductLocalizations.Find(id);
            if (prodLoc.ProductLocalizationID > 0)
            {
                prodLoc.ProductLocalizationStockQuantity = Convert.ToDouble(newValue);
                prodLoc.inventoryReason = "Stock Inventory";
                prodLoc.RegisteredByID = SessionGlobalPersonID;
                prodLoc.Description = "Stock Inventory";
                _plRepository.UpdateProductLocalization(prodLoc, serverdate, Convert.ToDouble(newValue));

                Reset1();
            }

            return this.Direct();

            //string message = "<b>Property:</b> {0}<br /><b>Field:</b> {1}<br /><b>Old Value:</b> {2}<br /><b>New Value:</b> {3}";

            //// Send Message...
            //X.Msg.Notify(new NotificationConfig()
            //{
            //    Title = "Edit Record #" + id.ToString(),
            //    Html = string.Format(message, id, field, oldValue, newValue),
            //    Width = 250
            //}).Show();

            //X.GetCmp<Store>("Store").GetById(id).Commit();

            //return this.Direct();
        }
        public ActionResult loadGrid()
        {
            this.GetCmp<Store>("Store").Reload();
            return this.Direct();
        }
        //[HttpPost]
        public ActionResult AddInventory(ProductLocalization plLocalization)
        {
            DateTime serverdate = SessionBusinessDay(null).BDDateOperation;
            if (plLocalization.ProductLocalizationID > 0)
            {
                plLocalization.inventoryReason = "Stock Inventory";
                plLocalization.RegisteredByID = SessionGlobalPersonID;
                plLocalization.Description = "Stock Inventory";
                _plRepository.UpdateProductLocalization(plLocalization, serverdate, plLocalization.ProductLocalizationStockQuantity);
               
                return this.Reset();
            }

            return this.Direct();
        }

        [HttpPost]
        public ActionResult UpdatePurchase(int PurchaseID)
        {
            ProductLocalization prodLoc = new ProductLocalization();
            prodLoc = db.ProductLocalizations.Find(PurchaseID);

            //vérification si le businessday de la location du produit est ouvert
            if (_busDayRepo.GetOpenedBusinessDay(prodLoc.Localization.Branch) == null || _busDayRepo.GetOpenedBusinessDay(prodLoc.Localization.Branch).BusinessDayID <= 0)
            {
                X.Msg.Alert(
                    "Error in Business Day",
                    "Sorry Business Day of branch of this product is not yet opened"
                  ).Show();
                return this.Direct();
            }

            this.InitializePurchaseFields(PurchaseID);
            return this.Direct();
        }

        [HttpPost]
        public ActionResult Reset()
        {
            Reset1();
            return this.Direct();
        }

        public void Reset1()
        {
            this.GetCmp<FormPanel>("InventoryForm").Reset(true);
            this.GetCmp<FormPanel>("GeneralForm").Reset(true);
            this.GetCmp<Panel>("RptInventory").Hidden = true;

            this.GetCmp<Store>("Store").Reload();
        }

        public void InitializePurchaseFields(int ID)
        {
            Reset1();

            if (ID > 0)
            {
                ProductLocalization prodLoc = new ProductLocalization();
                prodLoc = db.ProductLocalizations.Find(ID);

                this.GetCmp<DateField>("ProductLocalizationDate").Reset();
                BusinessDay businessDay = _busDayRepo.GetOpenedBusinessDay(db.Branches.Find(prodLoc.Localization.BranchID));
                this.GetCmp<DateField>("ProductLocalizationDate").SetValue(businessDay.BDDateOperation);

                this.GetCmp<TextField>("ProductLocalizationID").SetValue(prodLoc.ProductLocalizationID);
                this.GetCmp<TextField>("ProductID").SetValue(prodLoc.ProductID);

                this.GetCmp<TextField>("LocalizationID").SetValue(prodLoc.LocalizationID);
                this.GetCmp<TextField>("LocalizationID").ReadOnly = true;

                this.GetCmp<TextField>("Product").SetValue((prodLoc.Product is Lens) ? prodLoc.Product.ProductCode : prodLoc.ProductLabel);
                this.GetCmp<TextField>("Product").ReadOnly = true;

                this.GetCmp<TextField>("Localization").SetValue(prodLoc.LocalizationLabel);
                this.GetCmp<TextField>("Localization").ReadOnly = true;

                this.GetCmp<TextField>("BranchID").ReadOnly = true;
                this.GetCmp<TextField>("BranchID").SetValue(prodLoc.Localization.Branch.BranchName);


                this.GetCmp<NumberField>("ProductLocalizationSafetyStockQuantity").Reset();
                this.GetCmp<NumberField>("ProductLocalizationSafetyStockQuantity").SetValue(prodLoc.ProductLocalizationSafetyStockQuantity);
                this.GetCmp<NumberField>("ProductLocalizationSafetyStockQuantity").SetMinValue(0);
                this.GetCmp<NumberField>("ProductLocalizationSafetyStockQuantity").ReadOnly = false;

                this.GetCmp<NumberField>("ProductLocalizationStockQuantity").Reset();
                this.GetCmp<NumberField>("ProductLocalizationStockQuantity").SetValue(prodLoc.ProductLocalizationStockQuantity);
                this.GetCmp<NumberField>("ProductLocalizationStockQuantity").SetMinValue(0);
                this.GetCmp<NumberField>("ProductLocalizationStockQuantity").ReadOnly = false;

                this.GetCmp<NumberField>("OldQuantityID").Reset();
                this.GetCmp<NumberField>("OldQuantityID").SetValue(prodLoc.ProductLocalizationStockQuantity);

                this.GetCmp<NumberField>("ProductLocalizationStockSellingPrice").Reset();
                this.GetCmp<NumberField>("ProductLocalizationStockSellingPrice").SetValue(prodLoc.ProductLocalizationStockSellingPrice);
                this.GetCmp<NumberField>("ProductLocalizationStockSellingPrice").SetMinValue(0);
                this.GetCmp<NumberField>("ProductLocalizationStockSellingPrice").ReadOnly = false;

                this.GetCmp<NumberField>("AveragePurchasePrice").Reset();
                this.GetCmp<NumberField>("AveragePurchasePrice").SetValue(prodLoc.AveragePurchasePrice);
                this.GetCmp<NumberField>("AveragePurchasePrice").SetMinValue(0);
                this.GetCmp<NumberField>("AveragePurchasePrice").ReadOnly = false;

                return;
            }



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
        private List<object> ModelPL(String SearchOption, String SearchValue)
        {


            List<ProductLocalization> dataTmp = new List<ProductLocalization>();

            if (SearchValue == null || SearchValue == "" || SearchOption == null || SearchOption == "")
            {
                dataTmp = (from prodloc in db.ProductLocalizations
                           select prodloc)
                               .OrderByDescending(a => a.Product.ProductCode)
                                         .Take(1000)
                                         .ToList();
            }
            else
            {
                if (SearchOption == "CODE") //si recherche par code
                {
                    dataTmp = (from prodloc in db.ProductLocalizations
                               where prodloc.Product.ProductCode.Contains(SearchValue)
                               select prodloc)
                                   .OrderByDescending(a => a.Product.ProductCode)
                                             .Take(1000)
                                             .ToList();
                }

                if (SearchOption == "CAT") //si recherche par lens number
                {
                    dataTmp = (from prodloc in db.ProductLocalizations
                               where prodloc.Product.Category.CategoryCode.Contains(SearchValue)
                               select prodloc)
                                   .OrderByDescending(a => a.Product.ProductCode)
                                             .Take(1000)
                                             .ToList();
                }


            }


            List<object> realDataTmp = new List<object>();

            foreach (var p in dataTmp)
            {

                realDataTmp.Add(
                    new
                    {
                        ProductLocalizationID = p.ProductLocalizationID,
                        LocalizationLabel = p.LocalizationLabel,
                        ProductCode = p.ProductCode,
                        ProductLocalizationStockQuantity = p.ProductLocalizationStockQuantity,
                        ProductLocalizationStockSellingPrice = p.ProductLocalizationStockSellingPrice,
                        ProductLocalizationSafetyStockQuantity = p.ProductLocalizationSafetyStockQuantity,
                        Amount = p.Amount,
                        BranchName = p.Localization.Branch.BranchName,
                        Category = p.Product.Category.CategoryCode,
                        AveragePurchasePrice = p.AveragePurchasePrice
                        //InventoryReason = (lastIH != null) ? lastIH.inventoryReason : "",
                        //AutorizedBy = (lastIH != null) ? lastIH.AutorizedBy.UserFullName : "",
                        //CountBy = (lastIH != null) ? lastIH.CountBy.UserFullName : "",
                        //RegisteredBy = (lastIH != null) ? lastIH.RegisteredBy.UserFullName : "",
                    }
                    );
            }
            //Session["rptSource"] = dataTmp;
            return realDataTmp;
        }
        private void ModelAllPLReport()
        {

            var dataTmp = db.ProductLocalizations
                        .Select(s => new
                        {
                            ProductLocalizationID = s.ProductLocalizationID,
                            LocalizationLabel = s.Localization.LocalizationLabel,
                            ProductCode = s.Product.ProductCode,
                            ProductLocalizationStockQuantity = s.ProductLocalizationStockQuantity,
                            ProductLocalizationStockSellingPrice = s.ProductLocalizationStockSellingPrice,
                            ProductLocalizationSafetyStockQuantity = s.ProductLocalizationSafetyStockQuantity,
                            //Amount = s.Amount,
                            BranchName = s.Localization.Branch.BranchName,
                            AveragePurchasePrice = s.AveragePurchasePrice
                        }).ToList();

            //Session["rptSource"] = dataTmp;
            List<ProductLocalization> realDataTmp = new List<ProductLocalization>();

            //foreach (var p in dataTmp)
            //{
               
            //    realDataTmp.Add(
            //        new ProductLocalization
            //        {
            //            ProductLocalizationID = p.ProductLocalizationID,
            //            LocalizationLabelPrint = p.LocalizationLabelPrint,
            //            ProductCodePrint = p.ProductCodePrint,
            //            ProductLocalizationStockQuantity = p.ProductLocalizationStockQuantity,
            //            ProductLocalizationStockSellingPrice = p.ProductLocalizationStockSellingPrice,
            //            ProductLocalizationSafetyStockQuantity = p.ProductLocalizationSafetyStockQuantity,
            //            //Amount = p.Amount,
            //            BranchName = p.BranchName,
            //            AveragePurchasePrice = p.AveragePurchasePrice
            //        }
            //        );
            //}
            Session["rptSource"] = realDataTmp;

        }
        private void ModelPLReport(DateTime OperationDate) 
        {
            
            var dataTmp = db.ProductLocalizations.Join(db.InventoryHistorics, pl => pl.ProductID, inhist => inhist.ProductID,
                        (pl, inhist) => new { pl, inhist })
                        .Where(lsp => lsp.inhist.InventoryDate==OperationDate)
                        .Select(s => new
                        {
                            ProductLocalizationID = s.pl.ProductLocalizationID,
                            LocalizationLabel = s.pl.Localization.LocalizationLabel,
                            ProductCode = s.pl.Product.ProductCode,
                            ProductLocalizationStockQuantity = s.pl.ProductLocalizationStockQuantity,
                            ProductLocalizationStockSellingPrice = s.pl.ProductLocalizationStockSellingPrice,
                            ProductLocalizationSafetyStockQuantity = s.pl.ProductLocalizationSafetyStockQuantity,
                            //Amount = s.pl.Amount,
                            BranchName = s.pl.Localization.Branch.BranchName,
                            AveragePurchasePrice = s.pl.AveragePurchasePrice
                        }).ToList();


            List<ProductLocalization> realDataTmp = new List<ProductLocalization>();

            //foreach (var p in dataTmp)
            //{
               
            //    realDataTmp.Add(
            //        new ProductLocalization
            //        {
            //            ProductLocalizationID = p.ProductLocalizationID,
            //            LocalizationLabelPrint = p.LocalizationLabelPrint,
            //            ProductCodePrint = p.ProductCodePrint,
            //            ProductLocalizationStockQuantity = p.ProductLocalizationStockQuantity,
            //            ProductLocalizationStockSellingPrice = p.ProductLocalizationStockSellingPrice,
            //            ProductLocalizationSafetyStockQuantity = p.ProductLocalizationSafetyStockQuantity,
            //            //Amount = p.Amount,
            //            BranchName = p.BranchName,
            //            AveragePurchasePrice = p.AveragePurchasePrice
            //        }
            //        );
            //}

            Session["rptSource"] = realDataTmp;
            
        }
        [HttpPost]
        public StoreResult GetAllProductLocalizations(String SearchOption, String SearchValue)
        {
            return this.Store(ModelPL(SearchOption, SearchValue));
        }

        public ActionResult ChangeBusDay(int? BranchID)
        {
            this.GetCmp<DateField>("ProductLocalizationDate").Reset();
            if (BranchID > 0)
            {
                BusinessDay businessDay = _busDayRepo.GetOpenedBusinessDay(db.Branches.Find(BranchID.Value));
                //Session["businessDay"] = businessDay;
                this.GetCmp<DateField>("ProductLocalizationDate").SetValue(businessDay.BDDateOperation);
            }
            return this.Direct();
        }
        //this method print a list of inventory
        public ActionResult PrintGenericInventory()
        {
            //this.GetCmp<GridPanel>("InventoryList").Hidden = true;
            this.GetCmp<Panel>("RptInventory").Hidden = false;
            this.GetCmp<Panel>("RptInventory").LoadContent(new ComponentLoader
            {
                Url = Url.Action("GenerateGenericInventoryReport"),
                DisableCaching = false,
                Mode = LoadMode.Frame
                //Params = { new Parameter("Till", tillDay) }
            });
            return this.Direct();
        }

        public ActionResult PrintAllInventory()
        {
            //this.GetCmp<GridPanel>("InventoryList").Hidden = true;
            this.GetCmp<Panel>("RptInventory").Hidden = false;
            this.GetCmp<Panel>("RptInventory").LoadContent(new ComponentLoader
            {
                Url = Url.Action("GenerateInventoryAllReport"),
                DisableCaching = false,
                Mode = LoadMode.Frame
                //Params = { new Parameter("Till", tillDay) }
            });
            return this.Direct();
        }
        //[HttpPost]
        public ActionResult PrintLensInventory(string CategoryCode )
        {
            if (CategoryCode == null || CategoryCode.Length<=0)
            {
                X.Msg.Alert("Print Lens Inventory", "Please Choose Lens Category Before Print").Show();
                return this.Direct();
            }
            Session["CategoryCode"] = CategoryCode;
            //this.GetCmp<GridPanel>("InventoryList").Hidden = true;
            this.GetCmp<Panel>("RptInventory").Hidden = false;
            this.GetCmp<Panel>("RptInventory").LoadContent(new ComponentLoader
            {
                Url = Url.Action("GenerateLensInventoryReport"),
                DisableCaching = false,
                Mode = LoadMode.Frame
                //Params = { new Parameter("Till", tillDay) }
            });
            return this.Direct();
        }
        public ActionResult PrintLensInventoryStock(string CategoryCode)
        {
            if (CategoryCode == null || CategoryCode.Length <= 0)
            {
                X.Msg.Alert("Print Lens Inventory", "Please Choose Lens Category Before Print").Show();
                return this.Direct();
            }
            Session["CategoryCode"] = CategoryCode;
            //this.GetCmp<GridPanel>("InventoryList").Hidden = true;
            this.GetCmp<Panel>("RptInventory").Hidden = false;
            this.GetCmp<Panel>("RptInventory").LoadContent(new ComponentLoader
            {
                Url = Url.Action("GenerateLensInventoryStockReport"),
                DisableCaching = false,
                Mode = LoadMode.Frame
                //Params = { new Parameter("Till", tillDay) }
            });
            return this.Direct();
        }
        //This method print inentory of day
        public ActionResult GenerateInventoryAllReport()
        {
            List<object> model = new List<object>();
            ReportDocument rptH = new ReportDocument();
            string defaultImgPath = Server.MapPath("~/Content/Images/App/deboy-logo.png");
            byte[] COMPANY_LOGO = System.IO.File.ReadAllBytes(defaultImgPath);
            
            //List<ProductLocalization> dataTmp = (from prodloc in db.ProductLocalizations
            //                                     select prodloc)
            //                   .OrderBy(a => a.Product.ProductCode)
            //                    .ToList();

            //ModelAllPLReport();
            //List<ProductLocalization> dataTmp =   (List<ProductLocalization>)Session["rptSource"];
            var dataTmp = db.ProductLocalizations
                        .Select(s => new
                        {
                            ProductLocalizationID = s.ProductLocalizationID,
                            Localization = s.Localization,
                            Product = s.Product,
                            ProductLocalizationStockQuantity = s.ProductLocalizationStockQuantity,
                            ProductLocalizationStockSellingPrice = s.ProductLocalizationStockSellingPrice,
                            ProductLocalizationSafetyStockQuantity = s.ProductLocalizationSafetyStockQuantity,
                            //Amount = s.Amount,
                            BranchName = s.Localization.Branch.BranchName,
                            AveragePurchasePrice = s.AveragePurchasePrice
                        }).ToList();

            string strOperator1 = CurrentUser.Name;
            //List<object> realDataTmp = new List<object>();
            //int companyID = CurrentUser.UserBranches.First().Branch.CompanyID;
            foreach (var p in dataTmp.OrderBy(c => c.Product.ProductCode))
            {

                model.Add(
                    new
                    {
                        Localization = p.Localization.LocalizationLabel,
                        ProductLabel = p.Product.ProductCode,
                        ProductQty = p.ProductLocalizationStockQuantity,
                        ProductUnitPrice = p.ProductLocalizationStockSellingPrice,
                        ProductLocalizationSafetyStockQuantity = p.ProductLocalizationSafetyStockQuantity,
                        //Amount = p.Amount,
                        BranchName = p.Localization.Branch.BranchName,
                        //IsSafQuantStockReached = p.IsSafQuantStockReached,
                        BranchAdress = p.Localization.Branch.Adress.AdressEmail + "/" + p.Localization.Branch.Adress.AdressPOBox,
                        BranchTel = p.Localization.Branch.Adress.AdressPhoneNumber,
                        CompanyName = Company.Name,
                        CompanyAdress = Company.Adress.AdressEmail + "/" + Company.Adress.AdressPOBox,
                        CompanyTel = Company.Adress.AdressPhoneNumber,
                        CompanyCNI = Company.CNI,
                        CompanyLogo = _fileRepository.FindAll.FirstOrDefault(f => f.GlobalPersonID == Company.GlobalPersonID) != null ? _fileRepository.FindAll.FirstOrDefault(f => f.GlobalPersonID == Company.GlobalPersonID).Content : COMPANY_LOGO
                    }
                    );
            }
            if (model.Count>0)
            {
                string path = Server.MapPath("~/Reports/Supply/RptInventory.rpt");
                rptH.Load(path);
                rptH.SetDataSource(model);
                if (!string.IsNullOrEmpty(strOperator1)) rptH.SetParameterValue("Operator", strOperator1);
                Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                return File(stream, "application/pdf");
            }
            else
            {
                Response.Write("Nothing Found; No Report name found");
                return this.Direct();
            }
        }

        //This method print inentory of day
        public ActionResult GenerateGenericInventoryReport()
        {
            List<object> model = new List<object>();
            ReportDocument rptH = new ReportDocument();
            string defaultImgPath = Server.MapPath("~/Content/Images/App/deboy-logo.png");
            byte[] COMPANY_LOGO = System.IO.File.ReadAllBytes(defaultImgPath);

        
            var dataTmp = db.ProductLocalizations.Join(db.GenericProducts, pl => pl.ProductID, p => p.ProductID,
                        (pl, p) => new { pl, p })
                        .Where(pg=>!pg.p.ProductCode.Contains("RECAP".ToUpper()))
                        .Select(s => new
                        {
                            ProductLocalizationID = s.pl.ProductLocalizationID,
                            Localization = s.pl.Localization,
                            Product = s.pl.Product,
                            ProductLocalizationStockQuantity = s.pl.ProductLocalizationStockQuantity,
                            ProductLocalizationStockSellingPrice = s.pl.ProductLocalizationStockSellingPrice,
                            ProductLocalizationSafetyStockQuantity = s.pl.ProductLocalizationSafetyStockQuantity,
                            //Amount = s.pl.Amount,
                            BranchName = s.pl.Localization.Branch.BranchName,
                            AveragePurchasePrice = s.pl.AveragePurchasePrice
                        }).ToList();
            string strOperator1 = CurrentUser.Name;
            //List<object> realDataTmp = new List<object>();
            //int companyID = CurrentUser.UserBranches.First().Branch.CompanyID;
            foreach (var p in dataTmp.OrderBy(c=>c.Product.ProductCode))
            {

                model.Add(
                    new
                    {
                        Localization = p.Localization.LocalizationLabel,
                        ProductLabel = p.Product.ProductCode,
                        ProductQty = p.ProductLocalizationStockQuantity,
                        ProductUnitPrice = p.ProductLocalizationStockSellingPrice,
                        ProductLocalizationSafetyStockQuantity = p.ProductLocalizationSafetyStockQuantity,
                        //Amount = p.Amount,
                        BranchName = p.Localization.Branch.BranchName,
                        //IsSafQuantStockReached = p.IsSafQuantStockReached,
                        BranchAdress = p.Localization.Branch.Adress.AdressEmail + "/" + p.Localization.Branch.Adress.AdressPOBox,
                        BranchTel = p.Localization.Branch.Adress.AdressPhoneNumber,
                        CompanyName = Company.Name,
                        CompanyAdress = Company.Adress.AdressEmail + "/" + Company.Adress.AdressPOBox,
                        CompanyTel = Company.Adress.AdressPhoneNumber,
                        CompanyCNI = Company.CNI,
                        CompanyLogo = _fileRepository.FindAll.FirstOrDefault(f => f.GlobalPersonID == Company.GlobalPersonID) != null ? _fileRepository.FindAll.FirstOrDefault(f => f.GlobalPersonID == Company.GlobalPersonID).Content : COMPANY_LOGO
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

        //This method print inventory 
        public ActionResult GenerateLensInventoryReport()
        {
            List<object> model = new List<object>();
            ReportDocument rptH = new ReportDocument();
            string defaultImgPath = Server.MapPath("~/Content/Images/App/deboy-logo.png");
            byte[] COMPANY_LOGO = System.IO.File.ReadAllBytes(defaultImgPath);

            string CategoryCode = (string)Session["CategoryCode"];

            var dataTmp = db.ProductLocalizations.Join(db.Products, pl => pl.ProductID, p => p.ProductID,
                        (pl, p) => new { pl, p })
                        .Where(pll=>pll.p.Category.CategoryCode==CategoryCode)
                        .Select(s => new
                        {
                            ProductLocalizationID = s.pl.ProductLocalizationID,
                            Localization = s.pl.Localization,
                            Product = s.pl.Product,
                            ProductLocalizationStockQuantity = s.pl.ProductLocalizationStockQuantity,
                            ProductLocalizationStockSellingPrice = s.pl.ProductLocalizationStockSellingPrice,
                            ProductLocalizationSafetyStockQuantity = s.pl.ProductLocalizationSafetyStockQuantity,
                            //Amount = s.pl.Amount,
                            BranchName = s.pl.Localization.Branch.BranchName,
                            AveragePurchasePrice = s.pl.AveragePurchasePrice
                        }).ToList();
            string strOperator1 = CurrentUser.Name;
            //List<object> realDataTmp = new List<object>();
            //int companyID = CurrentUser.UserBranches.First().Branch.CompanyID;
            foreach (var p in dataTmp.OrderBy(c => c.Product.ProductCode))
            {
                //recuperation du prix de vente du produit
                LensNumberRangePrice price = _priceRepository.GetPrice(p.Product.ProductID);
                double SellingPrice = (price != null && price.LensNumberRangePriceID > 0) ? price.Price : 0;
                model.Add(
                    new
                    {
                        Localization = p.Localization.LocalizationLabel,
                        ProductLabel = p.Product.ProductCode,
                        ProductQty = p.ProductLocalizationStockQuantity,
                        ProductUnitPrice = SellingPrice, //p.ProductLocalizationStockSellingPrice,
                        ProductLocalizationSafetyStockQuantity = p.ProductLocalizationSafetyStockQuantity,
                        //Amount = p.Amount,
                        BranchName = p.Localization.Branch.BranchName,
                        //IsSafQuantStockReached = p.IsSafQuantStockReached,
                        BranchAdress = p.Localization.Branch.Adress.AdressEmail + "/" + p.Localization.Branch.Adress.AdressPOBox,
                        BranchTel = p.Localization.Branch.Adress.AdressPhoneNumber,
                        CompanyName = Company.Name,
                        CompanyAdress = Company.Adress.AdressEmail + "/" + Company.Adress.AdressPOBox,
                        CompanyTel = Company.Adress.AdressPhoneNumber,
                        CompanyCNI = Company.CNI,
                        CompanyLogo = _fileRepository.FindAll.FirstOrDefault(f => f.GlobalPersonID == Company.GlobalPersonID) != null ? _fileRepository.FindAll.FirstOrDefault(f => f.GlobalPersonID == Company.GlobalPersonID).Content : COMPANY_LOGO
                    }
                    );
            }
            if (model.Count > 0)
            {
                string path = Server.MapPath("~/Reports/Supply/RptInventory.rpt");
                rptH.Load(path);
                rptH.SetDataSource(model);
                if (!string.IsNullOrEmpty(strOperator1)) rptH.SetParameterValue("Operator", strOperator1);
                Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                return File(stream, "application/pdf");
            }
            else
            {
                Response.Write("Nothing Found; No Report name found");
                return this.Direct();
            }
        }

        //This method print inventory 
        public ActionResult GenerateLensInventoryStockReport()
        {
            List<object> model = new List<object>();
            ReportDocument rptH = new ReportDocument();
            string defaultImgPath = Server.MapPath("~/Content/Images/App/deboy-logo.png");
            byte[] COMPANY_LOGO = System.IO.File.ReadAllBytes(defaultImgPath);

            string CategoryCode = (string)Session["CategoryCode"];

            var dataTmp = db.ProductLocalizations.Join(db.Products, pl => pl.ProductID, p => p.ProductID,
                        (pl, p) => new { pl, p })
                        .Where(pll => pll.p.Category.CategoryCode == CategoryCode && pll.pl.ProductLocalizationStockQuantity>0)
                        .Select(s => new
                        {
                            ProductLocalizationID = s.pl.ProductLocalizationID,
                            Localization = s.pl.Localization,
                            Product = s.pl.Product,
                            ProductLocalizationStockQuantity = s.pl.ProductLocalizationStockQuantity,
                            ProductLocalizationStockSellingPrice = s.pl.ProductLocalizationStockSellingPrice,
                            ProductLocalizationSafetyStockQuantity = s.pl.ProductLocalizationSafetyStockQuantity,
                            //Amount = s.pl.Amount,
                            BranchName = s.pl.Localization.Branch.BranchName,
                            AveragePurchasePrice = s.pl.AveragePurchasePrice
                        }).ToList();
            string strOperator1 = CurrentUser.Name;
            //List<object> realDataTmp = new List<object>();
            //int companyID = CurrentUser.UserBranches.First().Branch.CompanyID;
            foreach (var p in dataTmp.OrderBy(c => c.Product.ProductCode))
            {
                //recuperation du prix de vente du produit
                LensNumberRangePrice price = _priceRepository.GetPrice(p.Product.ProductID);
                double SellingPrice = (price != null && price.LensNumberRangePriceID > 0) ? price.Price : 0;
                model.Add(
                    new
                    {
                        Localization = p.Localization.LocalizationLabel,
                        ProductLabel = p.Product.ProductCode,
                        ProductQty = p.ProductLocalizationStockQuantity,
                        ProductUnitPrice = SellingPrice, //p.ProductLocalizationStockSellingPrice,
                        ProductLocalizationSafetyStockQuantity = p.ProductLocalizationSafetyStockQuantity,
                        //Amount = p.Amount,
                        BranchName = p.Localization.Branch.BranchName,
                        //IsSafQuantStockReached = p.IsSafQuantStockReached,
                        BranchAdress = p.Localization.Branch.Adress.AdressEmail + "/" + p.Localization.Branch.Adress.AdressPOBox,
                        BranchTel = p.Localization.Branch.Adress.AdressPhoneNumber,
                        CompanyName = Company.Name,
                        CompanyAdress = Company.Adress.AdressEmail + "/" + Company.Adress.AdressPOBox,
                        CompanyTel = Company.Adress.AdressPhoneNumber,
                        CompanyCNI = Company.CNI,
                        CompanyLogo = _fileRepository.FindAll.FirstOrDefault(f => f.GlobalPersonID == Company.GlobalPersonID) != null ? _fileRepository.FindAll.FirstOrDefault(f => f.GlobalPersonID == Company.GlobalPersonID).Content : COMPANY_LOGO
                    }
                    );
            }
            if (model.Count>0)
            {
                string path = Server.MapPath("~/Reports/Supply/RptInventory.rpt");
                rptH.Load(path);
                rptH.SetDataSource(model);
                if (!string.IsNullOrEmpty(strOperator1)) rptH.SetParameterValue("Operator", strOperator1);
                Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                return File(stream, "application/pdf");
            }
            else
            {
                Response.Write("Nothing Found; No Report name found");
                return this.Direct();
            }
            
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