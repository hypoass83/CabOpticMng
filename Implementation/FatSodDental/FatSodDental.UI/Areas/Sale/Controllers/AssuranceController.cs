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
    public class AssuranceController : BaseController
    {
        private IPerson _personRepository;

        private IBusinessDay _busDayRepo;
        private ITransactNumber _transactNumbeRepository;
        private IAccount _accountRepository;
        List<BusinessDay> bdDay;
        

        public AssuranceController( 
            IPerson personRepository,
            IBusinessDay busDayRepo,
            IAccount accountRepository,
            ITransactNumber transactNumbeRepository)
        {
            this._personRepository = personRepository;
            this._busDayRepo = busDayRepo;
            this._transactNumbeRepository = transactNumbeRepository;
            this._accountRepository = accountRepository;
        }
        //Current Controller and current page
        private const string CONTROLLER_NAME = "Sale/Assurance";
        private const string VIEW_NAME = "Index";
        // GET: Sale/Assurance
        
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

            return View(ModelAssurance);
        }

        public ActionResult PrintAssuranceList()
        {
            this.GetCmp<Panel>("RptPrint").Hidden = false;
            this.GetCmp<Panel>("RptPrint").LoadContent(new ComponentLoader
            {
                Url = Url.Action("GenerateAssuranceList"),
                DisableCaching = false,
                Mode = LoadMode.Frame
            });
            return this.Direct();
        }
        //This method print inentory of day
        public ActionResult GenerateAssuranceList()
        {
            List<object> model = new List<object>();
            ReportDocument rptH = new ReportDocument();
            BusinessDay buDay = SessionBusinessDay(null);
            Company cmpny = db.Companies.FirstOrDefault();

            var dataTmp = db.Assureurs
                        .Select(s => new
                        {
                            GlobalPersonID = s.GlobalPersonID,
                            AssureurFullName = s.Name,
                            AdressPhoneNumber = s.Adress.AdressPhoneNumber,
                            AdressFullName = s.Adress.AdressFullName,
                            CNI = s.CNI
                        }).ToList();

            string strOperator1 = CurrentUser.Name;

            foreach (var p in dataTmp.OrderBy(c => c.AssureurFullName))
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
                        LibelleCpte = p.AssureurFullName,
                        LibelleOperation = "",
                        Reference = p.AdressPhoneNumber,
                        Desription = p.AdressFullName,
                        DateOperation = buDay.BDDateOperation,
                        MontantDB = 0,
                        MontantCR = 0
                    }
                    );
            }
            string path = Server.MapPath("~/Reports/Sales/RptAssuranceList.rpt");
            rptH.Load(path);
            rptH.SetDataSource(model);
            
            string stBranchName1 = cmpny.Name;
            string strBranchInfo1 = "Tel: " + cmpny.Adress.AdressPhoneNumber + " Fax : " + cmpny.Adress.AdressFax + " Town :" + cmpny.Adress.Quarter.Town.TownLabel;
            string strRepTitle1 = "LIST OF INSURANCES";
            if (!string.IsNullOrEmpty(stBranchName1)) rptH.SetParameterValue("BranchName", stBranchName1);
            if (!string.IsNullOrEmpty(strBranchInfo1)) rptH.SetParameterValue("BranchInfo", strBranchInfo1);
            if (!string.IsNullOrEmpty(strRepTitle1)) rptH.SetParameterValue("RepTitle", strRepTitle1);
            if (!string.IsNullOrEmpty(strOperator1)) rptH.SetParameterValue("Operator", strOperator1);

            Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            return File(stream, "application/pdf");
        }
        //Méthode d'ajout d'un client
        public ActionResult Add(Adress adress, Assureur assureur, int Account, int? Quarter)
        {
            try
            {
                adress.QuarterID = (Quarter == null || Quarter <= 0) ? 1 : Quarter.Value;
                assureur.Adress = adress;
                assureur.SexID = 1;
                assureur.CNI = assureur.Name;

                if (assureur.GlobalPersonID > 0)
                {
                    Assureur existingAssureur = db.Assureurs.SingleOrDefault(c => c.GlobalPersonID == assureur.GlobalPersonID);
                    //recuperation du cpte existant
                    assureur.AccountID = existingAssureur.AccountID;
                    _personRepository.Update2(assureur, SessionGlobalPersonID, (SessionBusinessDay(null) == null) ? DateTime.Now.Date : SessionBusinessDay(null).BDDateOperation, (SessionBusinessDay(null) == null) ? 0 : SessionBusinessDay(null).BranchID);
                    statusOperation = assureur.Name + " : " + Resources.AlertUpdateAction;
                }
                else
                {
                    //fabrication du nvo cpte
                    assureur.AccountID = _accountRepository.GenerateAccountNumber(Account, assureur.Name + " " + Resources.UIAccount, false).AccountID;
                
                    _personRepository.Create2Assurance(assureur, SessionGlobalPersonID, (SessionBusinessDay(null) == null) ? DateTime.Now.Date : SessionBusinessDay(null).BDDateOperation, (SessionBusinessDay(null) == null) ? 0 : SessionBusinessDay(null).BranchID);
                    statusOperation = assureur.Name + " : " + Resources.AlertAddAction;
                }

                this.AlertSucces(Resources.Success, statusOperation);
                return this.Reset();
            }
            catch (Exception e)
            {
                X.Msg.Alert("Error", e.Message).Show();
                return this.Direct();
            }
        }

        public ActionResult Reset()
        {
            this.GetCmp<FormPanel>("AssuranceForm").Reset();
            this.GetCmp<Store>("Store").Reload();
            return this.Direct();
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
            Assureur assureur = _personRepository.FindAll.OfType<Assureur>().FirstOrDefault(c => c.GlobalPersonID == ID);
            //Person parameters
            this.GetCmp<FormPanel>("AssuranceForm").Reset(true);
            this.GetCmp<TextField>("Name").Value = assureur.Name;
            this.GetCmp<TextField>("GlobalPersonID").Value = assureur.GlobalPersonID;
            this.GetCmp<TextField>("CompanyTradeRegister").Value = assureur.CompanyTradeRegister;
            this.GetCmp<TextField>("CompanySigle").Value = assureur.CompanySigle;
            this.GetCmp<ComboBox>("Account").Value = assureur.Account.CollectifAccountID;//.AccountID;
            //this.GetCmp<ComboBox>("Account").ReadOnly = true;

            //Adress 
            this.GetCmp<TextField>("AdressID").Value = assureur.Adress.AdressID;
            this.GetCmp<TextField>("AdressFax").Value = assureur.Adress.AdressFax;
            this.GetCmp<TextField>("AdressPhoneNumber").Value = assureur.Adress.AdressPhoneNumber;
            this.GetCmp<TextField>("AdressEmail").Value = assureur.Adress.AdressEmail;
            this.GetCmp<TextField>("AdressPOBox").Value = assureur.Adress.AdressPOBox;
            this.GetCmp<NumberField>("CompteurFacture").Value = assureur.CompteurFacture;
            
            //quartier
            //affichage des combo box
            this.GetCmp<ComboBox>("Region").Disabled = false;
            this.GetCmp<ComboBox>("Town").Disabled = false;
            this.GetCmp<ComboBox>("Quarter").Disabled = false;

            //mise à jour de l'affichage
            this.GetCmp<ComboBox>("Country").SetValueAndFireSelect(assureur.Adress.Quarter.Town.Region.CountryID);
            this.GetCmp<ComboBox>("Region").SetValueAndFireSelect(assureur.Adress.Quarter.Town.RegionID);
            this.GetCmp<ComboBox>("Town").SetValueAndFireSelect(assureur.Adress.Quarter.TownID);
            this.GetCmp<ComboBox>("Quarter").SetValue(assureur.Adress.QuarterID);

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
            this.GetCmp<FormPanel>("AssuranceForm").Reset();
            this.AlertSucces(Resources.Success, Resources.AlertDeleteAction);
            this.GetCmp<Store>("Store").Reload();
            return this.Direct();
        }
        [HttpPost]
        public StoreResult GetList()
        {
            return this.Store(ModelAssurance);
        }
       
        /// <summary>
        /// Return a Model that use in view
        /// </summary>
        private List<object> ModelAssurance
        {
            get
            {
                List<object> model = new List<object>();
                LoadComponent.GetAssurancesForStore.ForEach(u =>
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
                                Description = u.CompanySigle,
                                CNI = u.CNI
                            }
                        );
                });
                return model;
            }
        }
    }
}