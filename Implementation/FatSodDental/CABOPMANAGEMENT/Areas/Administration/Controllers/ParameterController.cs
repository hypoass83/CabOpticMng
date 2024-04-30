
using FatSod.DataContext.Concrete;
using FatSod.DataContext.Initializer;
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
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;


namespace CABOPMANAGEMENT.Areas.Administration.Controllers
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
        private IUserTill _userTillRepository;
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
            IUserTill userTillRepository,
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
            
            return View(ModelCountry);
        }
        

        //[OutputCache(Duration = 3600)] 
        public ActionResult Region()
        {
            
            return View(ModelRegion);
        }
        //[OutputCache(Duration = 3600)] 
        public ActionResult Town()
        {
            
            return View(ModelTown);
        }
        //[OutputCache(Duration = 3600)] 
        public ActionResult Quarter()
        {
            
            return View(ModelQuarter);
            
        }
        //************ Add action
        //[HttpPost]
        public JsonResult AddCountry(Country country)
        {
            bool status = false;
            try
            {
                if (country.CountryID > 0)
                {
                    Country countryToupdate = db.Countries.SingleOrDefault(c => c.CountryID == country.CountryID);
                    countryToupdate.CountryLabel = country.CountryLabel;
                    countryToupdate.CountryCode = country.CountryCode;
                    _countryRepository.Update(countryToupdate, country.CountryID);
                    statusOperation = Resources.AlertUpdateAction;
                }
                else
                {
                    _countryRepository.Create(country);
                    statusOperation = country.CountryLabel + " : " + Resources.AlertAddAction;
                }
                status = true;
            }
            catch (Exception e) { statusOperation="Error "+ e.Message+" "+e.InnerException; status = false;  }
            return new JsonResult { Data = new { status = status, Message = statusOperation } };
        }
        //[HttpPost]
        public JsonResult AddRegion(FatSod.Security.Entities.Region region, string Country)
        {
            bool status = false;
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
                status = true;
            }
            catch (Exception e) { statusOperation = "Error " + e.Message + " " + e.InnerException; status = false; }
            return new JsonResult { Data = new { status = status, Message = statusOperation } };
        }
        //[HttpPost]
        public JsonResult AddTown(Town town, string Region)
        {
            bool status = false;
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
                status = true;
            }
            catch (Exception e) { statusOperation = "Error " + e.Message + " " + e.InnerException; status = false; }
            return new JsonResult { Data = new { status = status, Message = statusOperation } };

        }
        //[HttpPost]
        public JsonResult AddQuarter(Quarter quarter, string Town)
        {
            bool status = false;
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
                status = true;
            }
            catch (Exception e) { statusOperation = "Error " + e.Message + " " + e.InnerException; status = false; }
            return new JsonResult { Data = new { status = status, Message = statusOperation } };
        }
        //Initialize field for update
        public ActionResult UpdateCountry(int ID)
        {
                int id = Convert.ToInt32(ID);
                Country countryToupdate = db.Countries.FirstOrDefault(c => c.CountryID == id);
                List<object> _countryList = new List<object>();
                _countryList.Add(new
                {
                    CountryID = countryToupdate.CountryID,
                    CountryLabel = countryToupdate.CountryLabel,
                    CountryCode = countryToupdate.CountryCode
                });
                return Json(_countryList, JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateBranch(int ID)
        {
                //int id = Convert.ToInt32(ID);
                Branch branchToUpdate = _branchRepository.FindAll.FirstOrDefault(b => b.BranchID == ID);
                List<object> _branchList = new List<object>();
                _branchList.Add(new
                {
                    BranchID = branchToUpdate.BranchID,
                    BranchCode = branchToUpdate.BranchCode,
                    BranchName = branchToUpdate.BranchName,
                    BranchDescription = branchToUpdate.BranchDescription,
                    BranchAbbreviation = branchToUpdate.BranchAbbreviation,
                    //Adress
                    AdressID = branchToUpdate.Adress.AdressID,
                    AdressFax = branchToUpdate.Adress.AdressFax,
                    AdressPhoneNumber = branchToUpdate.Adress.AdressPhoneNumber,
                    AdressCellNumber = branchToUpdate.Adress.AdressCellNumber,
                    AdressEmail = branchToUpdate.Adress.AdressEmail,
                    AdressPOBox = branchToUpdate.Adress.AdressPOBox,
                    AdressFullName = branchToUpdate.Adress.AdressFullName,
                    //mise à jour de l'affichage
                    Country=branchToUpdate.Adress.Quarter.Town.Region.CountryID,
                    Region=branchToUpdate.Adress.Quarter.Town.RegionID,
                    Town=branchToUpdate.Adress.Quarter.TownID,
                    Quarter=branchToUpdate.Adress.QuarterID
            });
            return Json(_branchList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateRegion(int ID)
        {
            
            int id = Convert.ToInt32(ID);
            FatSod.Security.Entities.Region regionToupdate = _regionRepository.FindAll.FirstOrDefault(c => c.RegionID == id);
            List<object> _regionList = new List<object>();
            _regionList.Add(new
            {
                RegionID = regionToupdate.RegionID,
                RegionCode = regionToupdate.RegionCode,
                RegionLabel = regionToupdate.RegionLabel,
                Country= regionToupdate.CountryID
            });
            return Json(_regionList, JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateTown(int ID)
        {

                int id = Convert.ToInt32(ID);
                FatSod.Security.Entities.Town townToUpdate = _townRepository.FindAll.FirstOrDefault(c => c.TownID == id);
                List<object> _townList = new List<object>();
                _townList.Add(new
                {
                    TownID = townToUpdate.TownID,
                    TownCode = townToUpdate.TownCode,
                    TownLabel = townToUpdate.TownLabel,
                    Region = townToUpdate.Region.RegionID,
                    Country = townToUpdate.Region.CountryID
                });
                return Json(_townList, JsonRequestBehavior.AllowGet);
                
        }
        public JsonResult UpdateQuarter(int ID)
        {
            
            int id = Convert.ToInt32(ID);
            FatSod.Security.Entities.Quarter quarterToUpdate = _quarterRepository.FindAll.FirstOrDefault(c => c.QuarterID == id);
            List<object> _townList = new List<object>();
            _townList.Add(new
            {
                QuarterID = quarterToUpdate.QuarterID,
                QuarterCode = quarterToUpdate.QuarterCode,
                QuarterLabel = quarterToUpdate.QuarterLabel,
                Region = quarterToUpdate.Town.Region.RegionID,
                Town= quarterToUpdate.Town.TownID,
                Country = quarterToUpdate.Town.Region.CountryID
            });
            return Json(_townList, JsonRequestBehavior.AllowGet);
                
        }
        //Deletes actions
        public JsonResult DeleteCountry(int ID)
        {
            bool status = false;
            try
            {
                Country countryToDelete = db.Countries.Find(ID);
                db.Countries.Remove(countryToDelete);
                db.SaveChanges();
                status = true;
                statusOperation = countryToDelete.CountryLabel + " : " + Resources.AlertDeleteAction;
            }
            catch (Exception e)
            {
                statusOperation = "Error : " + e.Message;
                status = false;
            }
            return new JsonResult { Data = new { status = status, Message = statusOperation } };
        }
        public JsonResult DeleteTown(int ID)
        {
            bool status = false;
            try
            {
                Town townToDelete = _townRepository.Find(ID);
                _townRepository.Delete(townToDelete);
                status = true;
                statusOperation = townToDelete.TownLabel + " : " + Resources.AlertDeleteAction;
            }
            catch (Exception e)
            {
                statusOperation = "Error : " + e.Message + " "+e.InnerException;
                status = false;
            }
            return new JsonResult { Data = new { status = status, Message = statusOperation } };
        }
        public JsonResult DeleteRegion(int ID)
        {
            bool status = false;
            try
            {
                FatSod.Security.Entities.Region regionToDelete = _regionRepository.Find(ID);
                _regionRepository.Delete(regionToDelete);
                statusOperation = regionToDelete.RegionLabel + " : " + Resources.AlertDeleteAction;
            }
            catch (Exception e)
            {
                statusOperation = "Error : " + e.Message + " " + e.InnerException;
                status = false;
            }
            return new JsonResult { Data = new { status = status, Message = statusOperation } };
        }
        public JsonResult DeleteQuarter(int ID)
        {
            bool status = false;
            try
            {
                Quarter quarterToDelete = _quarterRepository.Find(ID);
                _quarterRepository.Delete(quarterToDelete);
                status = true;
                statusOperation = quarterToDelete.QuarterLabel + " : " + Resources.AlertDeleteAction;
            }
            catch (Exception e)
            {
                statusOperation = "Error : " + e.Message + " " + e.InnerException;
                status = false;
            }
            return new JsonResult { Data = new { status = status, Message = statusOperation } };
        }
        public JsonResult DeleteBranch(int ID)
        {
            bool status = false;
            try
            {
                
                Branch branchToDelete = _branchRepository.Find(ID);

                //suppression du bussiness day de la branch
                BusinessDay busDayToDelete = _busDay.FindAll.FirstOrDefault(busDay => busDay.BranchID == branchToDelete.BranchID);
                if (busDayToDelete!=null) { _busDay.Delete(busDayToDelete); }
                
                //supression des tâches de cloture de journée
                List<BranchClosingDayTask> bcdtToDelete = _branchClosingDayTask.FindAll.Where(bcdt => bcdt.BranchID == branchToDelete.BranchID).ToList();
                if (bcdtToDelete != null) { _branchClosingDayTask.DeleteAll(bcdtToDelete); }
               
                _branchRepository.Delete(branchToDelete);
                status = true;
                statusOperation = Resources.AlertDeleteAction;
            }
            catch (Exception e)
            {
                statusOperation = "Error : " + e.Message;
                status=false;
            }
            return new JsonResult { Data = new { status = status, Message = statusOperation } };
        }

        ////retourne la liste des régions du pays qui a été sélectionné sur la vue de création d'un quartier
        //public StoreResult Regions(string countryID)
        //{
        //    List<object> model = new List<object>();
        //    _regionRepository.FindAll.Where(t => t.CountryID == Convert.ToInt32(countryID)).ToList().ForEach(t =>
        //    {
        //        model.Add(
        //                new
        //                {
        //                    RegionID = t.RegionID,
        //                    RegionLabel = t.RegionLabel
        //                }
        //            );
        //    });
        //    return this.Store(model);
        //}

    }
}