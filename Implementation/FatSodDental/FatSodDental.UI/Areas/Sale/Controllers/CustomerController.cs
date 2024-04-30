using FatSod.DataContext.Initializer;
using FatSodDental.UI.Controllers;
using FatSodDental.UI.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using FatSod.Security.Entities;
using FatSod.Security.Abstracts;
using FatSod.Supply.Entities;
using FatSod.Ressources;
using FatSodDental.UI.Reports.Sales;
using System.IO;
using CrystalDecisions.CrystalReports.Engine;
using FatSodDental.UI.Filters;
using FatSod.Supply.Abstracts;
using System.Web.UI;
using FatSod.DataContext.Concrete;
using ExtPartialViewResult = Ext.Net.MVC.PartialViewResult;

namespace FatSodDental.UI.Areas.Sale.Controllers
{
    [Authorize(Order = 1)]
    [TakeBusinessDay(Order = 2)]
    public class CustomerController : BaseController
    {
        private IPerson _personRepository;
        private IRepository<FatSod.Security.Entities.File> _fileRepository;
        private IAccount _accountRepository;

        private IBusinessDay _busDayRepo;
        private ITransactNumber _transactNumbeRepository;

        List<BusinessDay> bdDay;
        

        public CustomerController(IRepository<FatSod.Security.Entities.File> fileRepository, 
            IPerson personRepository,
            IAccount accountRepository,
            IBusinessDay busDayRepo,
            ITransactNumber transactNumbeRepository)
        {
            this._personRepository = personRepository;
            this._fileRepository = fileRepository;
            this._accountRepository = accountRepository;
            this._busDayRepo = busDayRepo;
            this._transactNumbeRepository = transactNumbeRepository;
        }
        //Current Controller and current page
        private const string CONTROLLER_NAME = "Sale/Customer";
        private const string VIEW_NAME = "Index";
        // GET: Sale/Customer
        
        [OutputCache(Duration = 3600)]
        public ActionResult Index()
        {

            bdDay = (List<BusinessDay>)Session["UserBusDays"];
            if (bdDay == null)
            {
                bdDay = _busDayRepo.GetOpenedBusinessDay(CurrentUser);
            }
            if (bdDay.Count() <= 0)
            {
                X.Msg.Alert("Error", "Your branch is currently closed").Show();
                return this.Direct();
            }
            DateTime currentDateOp = bdDay.FirstOrDefault().BDDateOperation;
            ViewBag.CurrentBranch = bdDay.FirstOrDefault().BranchID;
            //ViewBag.BusnessDayDate = bdDay.FirstOrDefault().BDDateOperation;

            return View(ModelCustomer);
        }

        public ActionResult PrintCustomerList()
        {
            this.GetCmp<Panel>("RptPrint").Hidden = false;
            this.GetCmp<Panel>("RptPrint").LoadContent(new ComponentLoader
            {
                Url = Url.Action("GenerateCustomerList"),
                DisableCaching = false,
                Mode = LoadMode.Frame
            });
            return this.Direct();
        }
        //This method print inentory of day
        public ActionResult GenerateCustomerList()
        {
            List<object> model = new List<object>();
            ReportDocument rptH = new ReportDocument();
            BusinessDay buDay = SessionBusinessDay(null);
            Company cmpny = db.Companies.FirstOrDefault();

            var dataTmp = db.Customers
                        .Select(s => new
                        {
                            GlobalPersonID = s.GlobalPersonID,
                            CustomerNumber = s.CNI,
                            CustomerFullName = s.Name ,//+ ((s.CompanySigle != null && s.CompanySigle.Length > 0) ? "" : " " + s.Description),
                            AdressPhoneNumber = s.Adress.AdressPhoneNumber,
                            AdressFullName = s.Adress.AdressFullName,
                            LimitAmount = (s.LimitAmount==null) ? 0 : s.LimitAmount,
                            CNI = s.CNI
                        }).ToList();

            string strOperator1 = CurrentUser.Name;

            foreach (var p in dataTmp.OrderBy(c => c.CustomerFullName))
            {

                model.Add(
                    new
                    {
                        RptEtatsJournalID = p.GlobalPersonID,
                        Agence = buDay.Branch.BranchCode,
                        LibAgence = buDay.Branch.BranchDescription,
                        Devise = "",
                        LibDevise = "",
                        CompteCle = p.CNI.ToString(),
                        LibelleCpte = p.CustomerFullName,
                        //CodeOperation = p.PoliceAssurance.ToString(),
                        LibelleOperation = "",
                        Reference = p.AdressPhoneNumber,
                        Desription = p.AdressFullName,
                        DateOperation = buDay.BDDateOperation,
                        MontantDB = p.LimitAmount,
                        MontantCR = 0
                    }
                    );
            }
            string path = Server.MapPath("~/Reports/Sales/RptCustomerList.rpt");
            rptH.Load(path);
            rptH.SetDataSource(model);
            
            string stBranchName1 = cmpny.Name;
            string strBranchInfo1 = "Tel: " + cmpny.Adress.AdressPhoneNumber + " Fax : " + cmpny.Adress.AdressFax + " Town :" + cmpny.Adress.Quarter.Town.TownLabel;
            string strRepTitle1 = "LIST OF CUSTOMER'S";
            if (!string.IsNullOrEmpty(stBranchName1)) rptH.SetParameterValue("BranchName", stBranchName1);
            if (!string.IsNullOrEmpty(strBranchInfo1)) rptH.SetParameterValue("BranchInfo", strBranchInfo1);
            if (!string.IsNullOrEmpty(strRepTitle1)) rptH.SetParameterValue("RepTitle", strRepTitle1);
            if (!string.IsNullOrEmpty(strOperator1)) rptH.SetParameterValue("Operator", strOperator1);

            Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            return File(stream, "application/pdf");
        }
        //Méthode d'ajout d'un client
        public ActionResult Add(Adress adress, Customer customer, int Quarter, int Account, int Assureur, int SexID, HttpPostedFileBase UploadImage)
        {
            adress.QuarterID = Quarter;
            customer.Adress = adress;
            customer.SexID = SexID;
            customer.AssureurID = Assureur;

            //We add image if exist in sending form
            if (UploadImage != null && UploadImage.ContentLength > 0)
            {
                var photo = new FatSod.Security.Entities.File
                {
                    FileName = System.IO.Path.GetFileName(UploadImage.FileName),
                    FileType = FileType.Photo,
                    ContentType = UploadImage.ContentType
                };
                using (var reader = new System.IO.BinaryReader(UploadImage.InputStream))
                {
                    photo.Content = reader.ReadBytes(UploadImage.ContentLength);
                }
                customer.Files = new List<FatSod.Security.Entities.File> { photo };
            }
            if (customer.GlobalPersonID > 0)
            {
                Customer existingCustomer  = db.Customers.SingleOrDefault(c=>c.GlobalPersonID==customer.GlobalPersonID);
                //recuperation du cpte existant
                customer.AccountID = existingCustomer.AccountID;
                _personRepository.Update2(customer,SessionGlobalPersonID,(SessionBusinessDay(null) == null) ? DateTime.Now.Date : SessionBusinessDay(null).BDDateOperation, (SessionBusinessDay(null) == null) ? 0 : SessionBusinessDay(null).BranchID);
                statusOperation = customer.Name + " : " + Resources.AlertUpdateAction;
            }
            else
            {
                //fabrication du nvo cpte
                customer.AccountID = _accountRepository.GenerateAccountNumber(Account,customer.CustomerFullName+" "+Resources.UIAccount,false).AccountID;
                _personRepository.Create2(customer, SessionGlobalPersonID, (SessionBusinessDay(null) == null) ? DateTime.Now.Date : SessionBusinessDay(null).BDDateOperation, (SessionBusinessDay(null) == null) ? 0 : SessionBusinessDay(null).BranchID);
                statusOperation = customer.Name + " : " + Resources.AlertAddAction;
            }
            
            this.AlertSucces(Resources.Success, statusOperation);

            return this.Reset();

        }

        public ActionResult Reset()
        {
            this.GetCmp<FormPanel>("CustomerForm").Reset();
            this.GetCmp<Store>("Store").Reload();
            this.GetCmp<ComboBox>("Account").ReadOnly = false;
            return this.Direct();
        }

       
        //this method return a file photo
        public ActionResult File(int id)
        {
            var fileToRetrieve = _fileRepository.Find(id);
            return File(fileToRetrieve.Content, fileToRetrieve.ContentType);
        }
        //****** This method return a towns list of one country
        public StoreResult Regions(string countryID)
        {
            List<object> model = new List<object>();
            int id =Convert.ToInt32(countryID);
            db.Regions.Where(t => t.CountryID == id ).ToList().ForEach(t =>
            {
                model.Add(
                        new
                        {
                            RegionID = t.RegionID,
                            RegionLabel = t.RegionLabel
                        }
                    );
            });
            return this.Store(model);
        }
        public StoreResult Towns(string regionID)
        {
            List<object> model = new List<object>();
            int id =Convert.ToInt32(regionID);
            db.Towns.Where(t => t.RegionID == id).ToList().ForEach(t =>
            {
                model.Add(
                        new
                        {
                            TownID = t.TownID,
                            TownLabel = t.TownLabel
                        }
                    );
            });
            return this.Store(model);
        }
        public StoreResult Quarters(string townID)
        {
            List<object> model = new List<object>();
            int id=Convert.ToInt32(townID);
            db.Quarters.Where(t => t.TownID == id).ToList().ForEach(t =>
            {
                model.Add(
                        new
                        {
                            QuarterID = t.QuarterID,
                            QuarterLabel = t.QuarterLabel
                        }
                    );
            });
            return this.Store(model);
        }
        //Initialize fields to update
        [HttpPost]
        public ActionResult InitializeFields(int ID)
        {
            //Customer customer = _personRepository.FindAll.OfType<Customer>().FirstOrDefault(c => c.GlobalPersonID == ID);
            Customer customer = db.Customers.Find(ID); // _personRepository.FindAll.OfType<Customer>().FirstOrDefault(c => c.GlobalPersonID == ID);
            //Person parameters
            this.GetCmp<FormPanel>("CustomerForm").Reset(true);
            this.GetCmp<TextField>("Name").Value = customer.Name;
            this.GetCmp<TextField>("GlobalPersonID").Value = customer.GlobalPersonID;
            this.GetCmp<TextField>("Description").Value = customer.Description;
            this.GetCmp<TextField>("CNI").Value = customer.CNI;
            this.GetCmp<TextField>("CompanyName").Value = customer.CompanyName;
            
            //on a une personne physique              
            //sex
            this.GetCmp<Radio>("Masculin").Value = customer.Sex.SexID;
            this.GetCmp<Radio>("Feminin").Value = customer.Sex.SexID;


            //We load image if exist
            var fileToRetrieve = _fileRepository.FindAll.FirstOrDefault(f => f.GlobalPersonID == ID);
            if (fileToRetrieve != null)
            {
                this.GetCmp<Image>("ImageID").Hidden = false;
                this.GetCmp<TextField>("FileID").Value = fileToRetrieve.FileID;
                this.GetCmp<Image>("ImageID").ImageUrl = "~/User/File?id=" + fileToRetrieve.FileID;
            }
            else
            {
                this.GetCmp<TextField>("FileID").Value = 0;
                this.GetCmp<Image>("ImageID").Hidden = true;
            }
            //Adress 
            this.GetCmp<TextField>("AdressID").Value = customer.Adress.AdressID;
            this.GetCmp<TextField>("AdressFax").Value = customer.Adress.AdressFax;
            this.GetCmp<TextField>("AdressPhoneNumber").Value = customer.Adress.AdressPhoneNumber;
            this.GetCmp<TextField>("AdressEmail").Value = customer.Adress.AdressEmail;
            this.GetCmp<TextField>("AdressPOBox").Value = customer.Adress.AdressPOBox;
            //quartier
            //affichage des combo box
            this.GetCmp<ComboBox>("Region").Disabled = false;
            this.GetCmp<ComboBox>("Town").Disabled = false;
            this.GetCmp<ComboBox>("Quarter").Disabled = false;

            //mise à jour de l'affichage
            this.GetCmp<ComboBox>("Country").SetValueAndFireSelect(customer.Adress.Quarter.Town.Region.CountryID);
            this.GetCmp<ComboBox>("Region").SetValueAndFireSelect(customer.Adress.Quarter.Town.RegionID);
            this.GetCmp<ComboBox>("Town").SetValueAndFireSelect(customer.Adress.Quarter.TownID);
            this.GetCmp<ComboBox>("Quarter").SetValue(customer.Adress.QuarterID);

            //Account parameters
            this.GetCmp<ComboBox>("Assureur").Value = (customer.Assureur!=null) ? customer.Assureur.GlobalPersonID : 0;
            this.GetCmp<TextField>("PoliceAssurance").Value = (customer.PoliceAssurance);
            this.GetCmp<ComboBox>("Account").Value = customer.Account.CollectifAccountID;//.AccountID;
            this.GetCmp<ComboBox>("Account").ReadOnly = true;

            this.GetCmp<ComboBox>("GestionnaireID").Value = customer.GestionnaireID;
            this.GetCmp<NumberField>("LimitAmount").Value = customer.LimitAmount;
            return this.Direct();
        }
        //Delete Action
        [HttpPost]
        public ActionResult Delete(int ID)
        {
            Person personToDelete = _personRepository.Find(ID);
            try
            {
                _personRepository.Delete(personToDelete, SessionGlobalPersonID, (SessionBusinessDay(null) == null) ? DateTime.Now.Date : SessionBusinessDay(null).BDDateOperation, (SessionBusinessDay(null) == null) ? 0 : SessionBusinessDay(null).BranchID);
            }
            catch (Exception e)
            {
                X.Msg.Alert("Error", e.Message).Show();
                return this.Direct();
            }
            this.GetCmp<Store>("Store").Reload();
            this.GetCmp<FormPanel>("CustomerForm").Reset();
            this.AlertSucces(Resources.Success, Resources.AlertDeleteAction);
            this.GetCmp<Store>("Store").Reload();
            return this.Direct();
        }
        [HttpPost]
        public StoreResult GetList()
        {
            return this.Store(ModelCustomer);
        }
        
        /// <summary>
        /// Return a Model that use in view
        /// </summary>
        private List<object> ModelCustomer
        {
            get
            {
                List<object> model = new List<object>();
                LoadComponent.GetCustomersForStore.ForEach(u =>
                {
                    model.Add(
                            new
                            {
                                GlobalPersonID = u.GlobalPersonID,
                                Name = u.Name,
                                AdressEmail = u.AdressEmail,
                                AdressPOBox = u.AdressPOBox,
                                AdressPhoneNumber = u.AdressPhoneNumber,
                                SexLabel = u.SexLabel,
                                Description = u.Description,
                                CNI = u.CNI,
                                AccountNumber = u.AccountNumber,
                                AccountLabel = u.AccountLabel,
                                PoliceAssurance = u.PoliceAssurance,
                                AssureurName = u.AssureurName
                            }
                        );
                });
                return model;
            }
        }
    }
}