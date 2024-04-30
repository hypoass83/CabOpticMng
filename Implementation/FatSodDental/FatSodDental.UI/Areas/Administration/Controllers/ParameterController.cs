using Ext.Net;
using Ext.Net.MVC;
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


namespace FatSodDental.UI.Areas.Administration.Controllers
{
    [Authorize(Order = 1)]
    [TakeBusinessDay(Order = 2)]
    public partial class ParameterController : BaseController
    {
        private IRepository<FatSod.Security.Entities.Country> _countryRepository;
        private IRepository<FatSod.Security.Entities.Region> _regionRepository;
        private IRepository<Town> _townRepository;
        private IRepository<User> _userRepository;
        private IRepository<Quarter> _quarterRepository;
        private IRepository<Branch> _branchRepository;
        private IRepository<UserBranch> _userBranchRepository;
        private IRepository<Company> _companyRepository;
        private IRepository<Adress> _adressRepository;
        private IRepository<Job> _jobRepository;
        private IPaymentMethod _paymentMethodRepository;
        private IBusinessDay _busDay;
        private IRepository<ClosingDayTask> _closingDayTask;
        private IRepository<BranchClosingDayTask> _branchClosingDayTask;
        private IRepository<FatSod.Security.Entities.File> _fileRepository;
        private IRepositorySupply<UserTill> _userTillRepository;
        private ITillDay _tillDayRepo;
        private const string CONTROLLER_NAME = "Administration/Parameter";
        
        public ParameterController(
           IPaymentMethod paymentMethodRepository,
           IRepository<BranchClosingDayTask> branchClosingDayTask,
           IRepository<ClosingDayTask> closingDayTask,
           IBusinessDay busDay, ITillDay tdr,
            IRepository<UserBranch> userBranchRepository,
            IRepository<Company> companyRepository,
            IRepository<Job> jobRepository,
            IRepository<Branch> branchRepository,
            IRepository<User> userRepository,
            IRepository<FatSod.Security.Entities.Region> regionRepository,
            IRepository<Town> townRepository,
            IRepository<Quarter> quarterRepository,
            IRepository<FatSod.Security.Entities.Country> countryRepository,
            IRepositorySupply<UserTill> userTillRepository,
            IRepository<Adress> adressRepository,
            IRepository<FatSod.Security.Entities.File> fileRepository

            )
        {
            this._tillDayRepo = tdr;
            this._countryRepository = countryRepository;
            this._regionRepository = regionRepository;
            this._townRepository = townRepository;
            this._quarterRepository = quarterRepository;
            this._userRepository = userRepository;
            this._branchRepository = branchRepository;
            this._userBranchRepository = userBranchRepository;
            this._jobRepository = jobRepository;
            this._companyRepository = companyRepository;
            this._paymentMethodRepository = paymentMethodRepository;
            this._busDay = busDay;
            this._closingDayTask = closingDayTask;
            this._branchClosingDayTask = branchClosingDayTask;
            this._userTillRepository = userTillRepository;
            this._adressRepository = adressRepository;
            this._fileRepository = fileRepository;
            //j'ai modifier ce commentaire pour créer et résoudre le conflit
            
        }
        // GET: Administration/Parameter
        public ActionResult Index()
        {
            //this.Parameters();
            return View();
        }
        //[OutputCache(Duration = 3600)] 
        public ActionResult Country()
        {
            //Ext.Net.MVC.PartialViewResult rPVResult = new Ext.Net.MVC.PartialViewResult
            //{
            //    ClearContainer = true,
            //    //RenderMode = RenderMode.AddTo,
            //    ContainerId = "Body",
            //    //WrapByScriptTag = false,
            //    Model = ModelCountry
            //};
            //return rPVResult; 
            return View(ModelCountry);
        }
        

        //[OutputCache(Duration = 3600)] 
        public ActionResult Region()
        {
            //Ext.Net.MVC.PartialViewResult rPVResult = new Ext.Net.MVC.PartialViewResult
            //{
            //    ClearContainer = true,
            //    //RenderMode = RenderMode.AddTo,
            //    ContainerId = "Body",
            //    //WrapByScriptTag = false,
            //    Model = ModelRegion
            //};
            //return rPVResult; 
            return View(ModelRegion);
        }
        //[OutputCache(Duration = 3600)] 
        public ActionResult Town()
        {
            //Ext.Net.MVC.PartialViewResult rPVResult = new Ext.Net.MVC.PartialViewResult
            //{
            //    ClearContainer = true,
            //    //RenderMode = RenderMode.AddTo,
            //    ContainerId = "Body",
            //    //WrapByScriptTag = false,
            //    Model = ModelTown
            //};
            //return rPVResult; 
            return View(ModelTown);
        }
        //[OutputCache(Duration = 3600)] 
        public ActionResult Quarter()
        {
            //Ext.Net.MVC.PartialViewResult rPVResult = new Ext.Net.MVC.PartialViewResult
            //{
            //    ClearContainer = true,
            //    //RenderMode = RenderMode.AddTo,
            //    ContainerId = "Body",
            //    //WrapByScriptTag = false,
            //    Model = ModelQuarter
            //};
            //return rPVResult; 
            return View(ModelQuarter);
            
        }
        //************ Add action
        [HttpPost]
        public ActionResult AddCountry(Country country)
        {
            try
            {
                if (country.CountryID > 0)
                {
                    Country countryToupdate = db.Countries.SingleOrDefault(c => c.CountryID == country.CountryID);
                    countryToupdate.CountryLabel = country.CountryLabel;
                    countryToupdate.CountryCode = country.CountryCode;
                    _countryRepository.Update(countryToupdate);
                    statusOperation = Resources.AlertUpdateAction;
                }
                else
                {
                    _countryRepository.Create(country);
                    statusOperation = country.CountryLabel + " : " + Resources.AlertAddAction;
                }
                this.AlertSucces(Resources.Success, statusOperation);
                this.GetCmp<FormPanel>("CountryForm").Reset();
                this.GetCmp<Store>("CountryStore").Reload();
                return this.Direct();
            }
            catch (Exception e) { X.Msg.Alert("Error ", e.Message).Show(); return this.Direct(); }
        }
        [HttpPost]
        public ActionResult AddRegion(FatSod.Security.Entities.Region region, string Country)
        {
            try
            {
                region.CountryID = Convert.ToInt32(Country);
                if (region.RegionID > 0)
                {
                    FatSod.Security.Entities.Region regionToupdate = _regionRepository.FindAll.First(c => c.RegionID == region.RegionID);
                    regionToupdate.RegionLabel = region.RegionLabel;
                    regionToupdate.RegionCode = region.RegionCode;
                    regionToupdate.CountryID = region.CountryID;
                    _regionRepository.Update(regionToupdate);
                    statusOperation = Resources.AlertUpdateAction;
                }
                else
                {
                    _regionRepository.Create(region);
                    statusOperation = region.RegionLabel + " : " + Resources.AlertAddAction;
                }
                this.AlertSucces(Resources.Success, statusOperation);
                this.GetCmp<FormPanel>("RegionForm").Reset();
                this.GetCmp<Store>("RegionStore").Reload();
                return this.Direct();
            }
            catch (Exception e) { X.Msg.Alert("Error ", e.Message).Show(); return this.Direct(); }

        }
        [HttpPost]
        public ActionResult AddTown(Town town, string Region)
        {
            try
            {
                town.RegionID = Convert.ToInt32(Region);
                if (town.TownID > 0)
                {
                    FatSod.Security.Entities.Town townToupdate = _townRepository.FindAll.First(c => c.TownID == town.TownID);
                    townToupdate.TownLabel = town.TownLabel;
                    townToupdate.TownCode = town.TownCode;
                    townToupdate.RegionID = town.RegionID;
                    _townRepository.Update(townToupdate);
                    statusOperation = Resources.AlertUpdateAction;
                }
                else
                {
                    _townRepository.Create(town);
                    statusOperation = town.TownLabel + " : " + Resources.AlertAddAction;
                }
                this.AlertSucces(Resources.Success, statusOperation);
                this.GetCmp<FormPanel>("TownForm").Reset();
                this.GetCmp<Store>("TownStore").Reload();
                return this.Direct();
            }
            catch (Exception e) { X.Msg.Alert("Error ", e.Message).Show(); return this.Direct(); }

        }
        [HttpPost]
        public ActionResult AddQuarter(Quarter quarter, string Town)
        {
            try
            {
                quarter.TownID = Convert.ToInt32(Town);
                if (quarter.QuarterID > 0)
                {
                    FatSod.Security.Entities.Quarter quarterToupdate = _quarterRepository.FindAll.First(c => c.QuarterID == quarter.QuarterID);
                    quarterToupdate.QuarterLabel = quarter.QuarterLabel;
                    quarterToupdate.QuarterCode = quarter.QuarterCode;
                    quarterToupdate.TownID = quarter.TownID;
                    _quarterRepository.Update(quarterToupdate);
                    statusOperation = quarter.QuarterLabel + " : " + Resources.AlertUpdateAction;
                }
                else
                {
                    _quarterRepository.Create(quarter);
                    statusOperation = quarter.QuarterLabel + " : " + Resources.AlertAddAction;
                }
                this.AlertSucces(Resources.Success, statusOperation);
                this.GetCmp<FormPanel>("QuarterForm").Reset();
                this.GetCmp<Store>("QuarterStore").Reload();
                return this.Direct();
            }
            catch (Exception e) { X.Msg.Alert("Error ", e.Message).Show(); return this.Direct(); }

        }
        //Initialize field for update
        public ActionResult UpdateCountry(int ID)
        {
            try
            {
                int id = Convert.ToInt32(ID);
                Country countryToupdate = db.Countries.FirstOrDefault(c => c.CountryID == id);
                this.GetCmp<FormPanel>("CountryForm").Reset(true);
                this.GetCmp<TextField>("CountryID").Value = countryToupdate.CountryID;
                this.GetCmp<TextField>("CountryCode").Value = countryToupdate.CountryCode;
                this.GetCmp<TextField>("CountryLabel").Value = countryToupdate.CountryLabel;
                return this.Direct();
            }
            catch (Exception e) { X.Msg.Alert("Error ", e.Message).Show(); return this.Direct(); }
        }
        public ActionResult UpdateBranch(int ID)
        {
            try
            {
                //int id = Convert.ToInt32(ID);
                Branch branchToUpdate = _branchRepository.FindAll.FirstOrDefault(b => b.BranchID == ID);
                this.GetCmp<FormPanel>("BranchForm").Reset(true);
                this.GetCmp<TextField>("BranchID").Value = branchToUpdate.BranchID;
                this.GetCmp<TextField>("BranchCode").Value = branchToUpdate.BranchCode;
                this.GetCmp<TextField>("BranchName").Value = branchToUpdate.BranchName;
                this.GetCmp<TextField>("BranchDescription").Value = branchToUpdate.BranchDescription;
                this.GetCmp<TextField>("BranchAbbreviation").Value = branchToUpdate.BranchAbbreviation;
                //Adress
                this.GetCmp<TextField>("AdressID").Value = branchToUpdate.Adress.AdressID;
                this.GetCmp<TextField>("AdressFax").Value = branchToUpdate.Adress.AdressFax;
                this.GetCmp<TextField>("AdressPhoneNumber").Value = branchToUpdate.Adress.AdressPhoneNumber;
								this.GetCmp<TextField>("AdressCellNumber").Value = branchToUpdate.Adress.AdressCellNumber;
                this.GetCmp<TextField>("AdressEmail").Value = branchToUpdate.Adress.AdressEmail;
                this.GetCmp<TextField>("AdressPOBox").Value = branchToUpdate.Adress.AdressPOBox;
								this.GetCmp<TextArea>("AdressFullName").Value = branchToUpdate.Adress.AdressFullName;
                //affichage des combo box
                this.GetCmp<ComboBox>("Region").Disabled = false;
                this.GetCmp<ComboBox>("Town").Disabled = false;
                this.GetCmp<ComboBox>("Quarter").Disabled = false;

                //mise à jour de l'affichage
                this.GetCmp<ComboBox>("Country").SetValueAndFireSelect(branchToUpdate.Adress.Quarter.Town.Region.CountryID);
                this.GetCmp<ComboBox>("Region").SetValueAndFireSelect(branchToUpdate.Adress.Quarter.Town.RegionID);
                this.GetCmp<ComboBox>("Town").SetValueAndFireSelect(branchToUpdate.Adress.Quarter.TownID);
                this.GetCmp<ComboBox>("Quarter").SetValue(branchToUpdate.Adress.QuarterID);
                return this.Direct();
            }
            catch (Exception e) { X.Msg.Alert("Error ", e.Message).Show(); return this.Direct(); }
        }
        public ActionResult UpdateRegion(int ID)
        {
            //this.GetCmp<ComboBox>("Country").Text = regionToupdate.Country.CountryLabel;
            try
            {
                int id = Convert.ToInt32(ID);
                FatSod.Security.Entities.Region regionToupdate = _regionRepository.FindAll.FirstOrDefault(c => c.RegionID == id);
                this.GetCmp<FormPanel>("RegionForm").Reset(true);
                this.GetCmp<TextField>("RegionID").Value = regionToupdate.RegionID;
                this.GetCmp<TextField>("RegionCode").Value = regionToupdate.RegionCode;
                this.GetCmp<TextField>("RegionLabel").Value = regionToupdate.RegionLabel;
                this.GetCmp<ComboBox>("Country").SetValue(regionToupdate.CountryID);
            }
            catch (Exception e) { X.Msg.Alert("Error ", e.Message).Show(); return this.Direct(); }
            return this.Direct();
        }
        public ActionResult UpdateTown(int ID)
        {
            try
            {
                int id = Convert.ToInt32(ID);
                FatSod.Security.Entities.Town townToUpdate = _townRepository.FindAll.FirstOrDefault(c => c.TownID == id);
                this.GetCmp<FormPanel>("TownForm").Reset(true);
                this.GetCmp<TextField>("TownID").Value = townToUpdate.RegionID;
                this.GetCmp<TextField>("TownCode").Value = townToUpdate.TownCode;
                this.GetCmp<TextField>("TownLabel").Value = townToUpdate.TownLabel;
                //quartier
                //affichage des combo box
                this.GetCmp<ComboBox>("Region").Disabled = false;
                //this.GetCmp<ComboBox>("Town").Disabled = false;
                //this.GetCmp<ComboBox>("Quarter").Disabled = false;

                //mise à jour de l'affichage
                this.GetCmp<ComboBox>("Country").SetValueAndFireSelect(townToUpdate.Region.CountryID);
                this.GetCmp<ComboBox>("Region").SetValueAndFireSelect(townToUpdate.Region.RegionID);
                //this.GetCmp<ComboBox>("Town").SetValueAndFireSelect(townToUpdate.RegionID);
                //this.GetCmp<ComboBox>("Quarter").SetValue(user.Adress.QuarterID);
                return this.Direct();
            }
            catch (Exception e) { X.Msg.Alert("Error ", e.Message).Show(); return this.Direct(); }
        }
        public ActionResult UpdateQuarter(int ID)
        {
            try
            {
                int id = Convert.ToInt32(ID);
                FatSod.Security.Entities.Quarter quarterToUpdate = _quarterRepository.FindAll.FirstOrDefault(c => c.QuarterID == id);
                this.GetCmp<FormPanel>("QuarterForm").Reset(true);
                this.GetCmp<TextField>("QuarterID").Value = quarterToUpdate.QuarterID;
                this.GetCmp<TextField>("QuarterCode").Value = quarterToUpdate.QuarterCode;
                this.GetCmp<TextField>("QuarterLabel").Value = quarterToUpdate.QuarterLabel;
                //quartier
                //affichage des combo box
                this.GetCmp<ComboBox>("Region").Disabled = false;
                this.GetCmp<ComboBox>("Town").Disabled = false;
                //this.GetCmp<ComboBox>("Quarter").Disabled = false;

                //mise à jour de l'affichage
                this.GetCmp<ComboBox>("Country").SetValueAndFireSelect(quarterToUpdate.Town.Region.CountryID);
                this.GetCmp<ComboBox>("Region").SetValueAndFireSelect(quarterToUpdate.Town.Region.RegionID);
                this.GetCmp<ComboBox>("Town").SetValue(quarterToUpdate.Town.TownID);
                return this.Direct();
            }
            catch (Exception e) { X.Msg.Alert("Error ", e.Message).Show(); return this.Direct(); }
        }
        //Deletes actions
        public ActionResult DeleteCountry(int ID)
        {
            //int id = ID;
            try
            {
                Country countryToDelete = db.Countries.Find(ID);
                db.Countries.Remove(countryToDelete);
                db.SaveChanges();
                //_countryRepository.Delete(countryToDelete);
                statusOperation = countryToDelete.CountryLabel + " : " + Resources.AlertAddAction;
            }
            catch (Exception e)
            {
                statusOperation = "Error : " + e.Message;
                X.Msg.Alert("Error", statusOperation).Show();
                return this.Direct();
            }
            this.AlertSucces(Resources.Success, statusOperation);
            this.GetCmp<Store>("CountryStore").Reload();
            return this.Direct();
        }
        public ActionResult DeleteTown(int ID)
        {
            //int id = ID;
            try
            {
                Town townToDelete = _townRepository.Find(ID);
                _townRepository.Delete(townToDelete);
                statusOperation = townToDelete.TownLabel + " : " + Resources.AlertAddAction;
            }
            catch (Exception e)
            {
                statusOperation = "Error : " + e.Message;
                X.Msg.Alert("Error", statusOperation).Show();
                return this.Direct();
            }
            this.AlertSucces(Resources.Success, statusOperation);
            this.GetCmp<Store>("TownStore").Reload();
            return this.Direct();
        }
        public ActionResult DeleteRegion(int ID)
        {
            //int id = ID;
            try
            {
                FatSod.Security.Entities.Region regionToDelete = _regionRepository.Find(ID);
                _regionRepository.Delete(regionToDelete);
                statusOperation = regionToDelete.RegionLabel + " : " + Resources.AlertAddAction;
            }
            catch (Exception e)
            {
                statusOperation = "Error : " + e.Message;
                X.Msg.Alert("Error", statusOperation).Show();
                return this.Direct();
            }
            this.AlertSucces(Resources.Success, statusOperation);
            this.GetCmp<Store>("RegionStore").Reload();
            return this.Direct();
        }
        public ActionResult DeleteQuarter(int ID)
        {
            //int id = ID;
            try
            {
                Quarter quarterToDelete = _quarterRepository.Find(ID);
                _quarterRepository.Delete(quarterToDelete);
                statusOperation = quarterToDelete.QuarterLabel + " : " + Resources.AlertAddAction;
            }
            catch (Exception e)
            {
                statusOperation = "Error : " + e.Message;
                X.Msg.Alert("Error", statusOperation).Show();
                return this.Direct();
            }
            this.AlertSucces(Resources.Success, statusOperation);
            this.GetCmp<Store>("QuarterStore").Reload();
            return this.Direct();
        }
        public ActionResult DeleteBranch(int ID)
        {
            //int id = ID;
            try
            {
                statusOperation = Resources.AlertDeleteAction;
                Branch branchToDelete = _branchRepository.Find(ID);

                //suppression du bussiness day de la branch
                BusinessDay busDayToDelete = _busDay.FindAll.FirstOrDefault(busDay => busDay.BranchID == branchToDelete.BranchID);
                _busDay.Delete(busDayToDelete);

                //supression des tâches de cloture de journée
                List<BranchClosingDayTask> bcdtToDelete = _branchClosingDayTask.FindAll.Where(bcdt => bcdt.BranchID == branchToDelete.BranchID).ToList();
                _branchClosingDayTask.DeleteAll(bcdtToDelete);

                _branchRepository.Delete(branchToDelete);
            }
            catch (Exception e)
            {
                statusOperation = "Error : " + e.Message;
                X.Msg.Alert("Error", statusOperation).Show();
                return this.Direct();
            }
            this.AlertSucces(Resources.Success, statusOperation);
            this.GetCmp<Store>("BranchesList").Reload();
            return this.Direct();
        }

        //retourne la liste des régions du pays qui a été sélectionné sur la vue de création d'un quartier
        public StoreResult Regions(string countryID)
        {
            List<object> model = new List<object>();
            _regionRepository.FindAll.Where(t => t.CountryID == Convert.ToInt32(countryID)).ToList().ForEach(t =>
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

    }
}