
using CABOPMANAGEMENT.Controllers;
using System.Web.Mvc;
using FatSod.Security.Abstracts;
using FatSod.Security.Entities;
using CABOPMANAGEMENT.Tools;
using System;
using System.Linq;
using System.Web;
using FatSod.DataContext.Initializer;
using FatSod.Ressources;
using System.Collections.Generic;
using FatSod.Supply.Entities;
using CABOPMANAGEMENT.Filters;
using System.Web.UI;


namespace CABOPMANAGEMENT.Areas.Administration.Controllers
{

    public partial class ParameterController : BaseController
    {
        /***** Company, job management actions ***/
        //************ Add action
        //[HttpPost]
        public JsonResult AddCompany(Company company, Adress adress, HttpPostedFileBase UploadImage, int FileID)
        {
            bool status = false;
            string Message = "";
            try
            {
                company.AdressID = adress.AdressID;
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
                    photo.GlobalPersonID = company.GlobalPersonID;
                    if (FileID > 0)
                    {
                        _fileRepository.Update(photo, FileID);
                    }
                    else
                    {
                        _fileRepository.Create(photo);
                    }

                    //company.Files = new List<FatSod.Security.Entities.File> { photo };
                }
                _adressRepository.Update(adress, adress.AdressID);
                _companyRepository.Update(company, company.GlobalPersonID);
                statusOperation = Resources.AlertUpdateAction;
                
                //We load image if exist
                /*var fileToRetrieve = _fileRepository.FindAll.LastOrDefault(f => f.GlobalPersonID == company.GlobalPersonID);
                if (fileToRetrieve != null)
                {
                    this.GetCmp<Image>("ImageID").Hidden = false;
                    this.GetCmp<TextField>("FileID").Value = fileToRetrieve.FileID;
                    this.GetCmp<Image>("ImageID").ImageUrl = "~/User/File?id=" + fileToRetrieve.FileID;
                }
                else
                {
                    this.GetCmp<Image>("ImageID").ImageUrl = "~/User/File?id=0";
                }*/
                status = true;
                Message = statusOperation;
            }
            catch (Exception e) {
                status = false;
                Message = "Error "+ e.Message+ " "+ e.InnerException;
                }
            return new JsonResult { Data = new { status = status, Message = Message } };
        }
        //this method return a file photo
        public ActionResult File(int id)
        {
            var fileToRetrieve = _fileRepository.Find(id);
            return File(fileToRetrieve.Content, fileToRetrieve.ContentType);
        }
        //[OutputCache(Duration = 3600)] 
        public ActionResult Company()
        {
           
            var company = _companyRepository.FindAll.FirstOrDefault();
            ViewBag.Company = company;

			//this.InitInfo(company);
            var fileToRetrieve = _fileRepository.FindAll.FirstOrDefault(f => f.GlobalPersonID == company.GlobalPersonID);
            ViewBag.CompanyLogoID = fileToRetrieve != null ? fileToRetrieve.FileID : 0;
            return View();
        }
        /**
        * List of Countries to go Inside Select Tab
        **/
        public JsonResult populateCountry()
        {
            //holds list of Countries 
            List<object> _Countries = new List<object>();

            //queries all the Countries for its ID and Name property.
            var Countries = (from s in db.Countries
                             select new { s.CountryID, s.CountryLabel, s.CountryCode }).ToList();

            //save list of Countries to the _Countries
            foreach (var item in Countries.OrderBy(i => i.CountryCode))
            {
                _Countries.Add(new
                {
                    ID = item.CountryID,
                    Name = item.CountryCode + " " + item.CountryLabel
                });
            }
            //returns the Json result of _Countries
            return Json(_Countries, JsonRequestBehavior.AllowGet);
        }

        /**
         * List of Regions to go Inside Select Tab
         * @arg : countryId
         **/
        public JsonResult populateRegion(string countryId)
        {

            int _countryId = Convert.ToInt32(countryId);

            //holds list of Regions 
            List<object> _Regions = new List<object>();

            //queries all the Regions for its ID and Name property.
            var Regions = (from s in db.Regions
                           where s.CountryID == _countryId
                           select new { s.CountryID, s.RegionID, s.RegionLabel, s.RegionCode }).ToList();

            //save list of Regions to the _Regions
            foreach (var item in Regions.OrderBy(i => i.RegionCode))
            {
                _Regions.Add(new
                {
                    ID = item.RegionID,
                    Name = item.RegionCode + " " + item.RegionLabel
                });
            }
            //returns the Json result of _Regions
            return Json(_Regions, JsonRequestBehavior.AllowGet);
        }

        /**
         * List of Towns to go Inside Select Tab
         **/
        public JsonResult populateTown(string regionId)
        {

            int _regionId = Convert.ToInt32(regionId);

            //holds list of Towns 
            List<object> _Towns = new List<object>();

            //queries all the Towns for its ID and Name property.
            var Towns = (from s in db.Towns
                         where s.RegionID == _regionId
                         select new { s.RegionID, s.TownID, s.TownLabel, s.TownCode }).ToList();

            //save list of Towns to the _Towns
            foreach (var item in Towns.OrderBy(i => i.TownCode))
            {
                _Towns.Add(new
                {
                    ID = item.TownID,
                    Name = item.TownCode + " " + item.TownLabel
                });
            }
            //returns the Json result of _Towns
            return Json(_Towns, JsonRequestBehavior.AllowGet);
        }

        /**
         * List of Quarters to go Inside Select Tab
         **/
        public JsonResult populateQuarterList(string townId)
        {

            int _townId = Convert.ToInt32(townId);

            //holds list of Quarters 
            List<object> _Quarters = new List<object>();

            ////queries all the Quarters for ID and Name property.
            //var Quarters = (from s in db.Quarters
            //                where s.TownID == _townId
            //                select new { s.TownID, s.QuarterID, s.QuarterLabel, s.QuarterCode }).ToList();

            ////save list of Towns to the _Quarters
            //foreach (var item in Quarters.OrderBy(i => i.QuarterCode))
            List<Quarter> res = new List<Quarter>();

            IEqualityComparer<Quarter> locationComparer = new GenericComparer<Quarter>("QuarterCode");
            res = _quarterRepository.FindAll.Where(s => s.TownID == _townId)
                .Distinct(locationComparer).ToList();

            //return res;
            //save list of Towns to the _Quarters
            foreach (var item in res.OrderBy(i => i.QuarterCode))
            {
                _Quarters.Add(new
                {
                    TownId = item.TownID,
                    ID = item.QuarterID,
                    Name = item.QuarterCode + " " + item.QuarterLabel
                });
            }
            //returns the Json result of _Quarters
            return Json(_Quarters, JsonRequestBehavior.AllowGet);
        }
        public JsonResult InitInfo(int companyID)
        {
            Company company = db.Companies.Find(companyID);
            //returns the Json result of _BeneficiariesList
            List<object> _companyList = new List<object>();
            _companyList.Add(new
            {
                Country = company.Adress.Quarter.Town.Region.CountryID,
                Region = company.Adress.Quarter.Town.RegionID,
                Town = company.Adress.Quarter.TownID,
                Quarter = company.Adress.QuarterID
            });
            return Json(_companyList, JsonRequestBehavior.AllowGet);
        }
        //List of jobs
        //[OutputCache(Duration = 3600)] 
        public ActionResult Job()
        {
           
            return View(ModelJob);
        }
        //[OutputCache(Duration = 3600)] 
        public ActionResult Branch()
        {
            
            return View(ModelBranch);
        }

        public JsonResult InitializeJobsFields(int ID)
        {
            Job job = _jobRepository.FindAll.SingleOrDefault(j => j.JobID == ID);
            List<object> _jobList = new List<object>();
            _jobList.Add(new
            {
                JobID = job.JobID,
                JobCode = job.JobCode,
                JobLabel = job.JobLabel,
                JobDescription = job.JobDescription
            });
            return Json(_jobList, JsonRequestBehavior.AllowGet);
        }
        //[HttpPost]
        public JsonResult AddJob(Job job)
        {
            bool status = false;
            try
            {
                Company companyToUpdate = _companyRepository.FindAll.FirstOrDefault();
                job.CompanyID = companyToUpdate.GlobalPersonID;
                if (job.JobID > 0)
                {
                    Job jobToUpdate = _jobRepository.FindAll.SingleOrDefault(j => j.JobID == job.JobID);
                    jobToUpdate.JobCode = job.JobCode;
                    jobToUpdate.JobLabel = job.JobLabel;
                    jobToUpdate.JobDescription = job.JobDescription;
                    jobToUpdate.CompanyID = companyToUpdate.GlobalPersonID;
                    _jobRepository.Update(jobToUpdate);
                    statusOperation = Resources.AlertUpdateAction;
                }
                else
                {
                    _jobRepository.Create(job);
                    statusOperation = job.JobLabel + " : " + Resources.AlertAddAction;
                }
                status = true;
            }
            catch (Exception e)
            {
               statusOperation= e.Message;
               status = false;
            }
            return new JsonResult { Data = new { status = status, Message = statusOperation } };
        }
        //Delete job action
        public JsonResult DeleteJob(int ID)
        {
            bool status = false;
            try
            {
                _jobRepository.Delete(ID);
                status = true;
                statusOperation = "Job : " + Resources.AlertDeleteAction;
            }
            catch (Exception e)
            {
                statusOperation = "Error : " + e.Message;
                status = false;
            }
            return new JsonResult { Data = new { status = status, Message = statusOperation } };
        }
        //[HttpPost]
        public JsonResult AddBranch(Branch branch, Adress adress, int QuarterID)
        {
            bool status = false;
            try
            {
                adress.QuarterID = QuarterID;
                branch.Adress = adress;
                if (branch.BranchID > 0)
                {
                    Branch branchToUpdate = _branchRepository.FindAll.SingleOrDefault(c => c.BranchID == branch.BranchID);
                    branchToUpdate.BranchName = branch.BranchName;
                    branchToUpdate.BranchDescription = branch.BranchDescription;
                    branchToUpdate.BranchCode = branch.BranchCode;
                    branchToUpdate.BranchAbbreviation = branch.BranchAbbreviation;
                    _adressRepository.Update(adress, adress.AdressID);
                    _branchRepository.Update(branchToUpdate);
                    statusOperation = Resources.AlertUpdateAction;
                }
                else
                {
                    Company dboyCompany = _companyRepository.FindAll.FirstOrDefault();
                    branch.CompanyID = dboyCompany.GlobalPersonID;

                    branch = _branchRepository.Create(branch);

                    //création du business day pour cette agence
                    BusinessDay openedBusDays = _busDay.GetOpenedBusinessDay(CurrentUser).SingleOrDefault();
                    if (openedBusDays != null)
                    {
                        BusinessDay branchBusDay = new BusinessDay
                        {
                            BDCode = "BusDay_" + branch.BranchCode,
                            BDLabel = "BusDay_" + branch.BranchCode,
                            BDDescription = "this is for " + branch.BranchCode + " branch",
                            BDStatut = false,
                            ClosingDayStarted = false,
                            BDDateOperation = openedBusDays.BDDateOperation.Date,
                            BackDateOperation = openedBusDays.BDDateOperation.Date,
                            BranchID = branch.BranchID,
                        };
                        branchBusDay = _busDay.Create(branchBusDay);


                        //création des tâches de clôture pour cette agence
                        List<ClosingDayTask> allClosingDayTasks = _closingDayTask.FindAll.ToList();
                        foreach (ClosingDayTask cdt in allClosingDayTasks)
                        {
                            BranchClosingDayTask bcdt = new BranchClosingDayTask
                            {
                                BranchID = branch.BranchID,
                                ClosingDayTaskID = cdt.ClosingDayTaskID,
                                Statut = true
                            };
                            _branchClosingDayTask.Create(bcdt);
                        }

                    }
                    //We  allocate this branch to super admin
                    User user = _userRepository.FindAll.FirstOrDefault(u => u.Code == "AdminCode");
                    User super_user = _userRepository.FindAll.FirstOrDefault(u => u.Code == "SuperAdminCode15");
                    UserBranch super_userBranch = new UserBranch()
                    {
                        BranchID = branch.BranchID,
                        UserID = super_user.GlobalPersonID
                    };
                    UserBranch userBranch = new UserBranch()
                    {
                        BranchID = branch.BranchID,
                        UserID = user.GlobalPersonID
                    };
                    _userBranchRepository.Create(super_userBranch);
                    _userBranchRepository.Create(userBranch);

                    statusOperation = branch.BranchName + " : " + Resources.AlertAddAction;
                }
                status = true;
            }
            catch (Exception e)
            {
                statusOperation ="Error "+ e.Message + "-" + e.InnerException;
                status =false;
            }
            return new JsonResult { Data = new { status = status, Message = statusOperation } };
        }
        ////============= differents store of parameters
        //[HttpPost]
        //public StoreResult GetBranchList()
        //{
        //    return this.Store(ModelBranch);
        //}
        //[HttpPost]
        //public StoreResult GetTownsList()
        //{
        //    return this.Store(ModelTown);
        //}
        //[HttpPost]
        //public StoreResult GetQuartersList()
        //{
        //    return this.Store(ModelQuarter);
        //}
        //[HttpPost]
        //public StoreResult GetRegionsList()
        //{
        //    return this.Store(ModelRegion);
        //}
        //[HttpPost]
        //public StoreResult GetCountrysList()
        //{
        //    return this.Store(ModelCountry);
        //}
        //[HttpPost]
        //public StoreResult GetJobsList()
        //{
        //    return this.Store(ModelJob);
        //}

        //===================List of Model of differents enities==================================
        private List<Branch> ModelBranch
        {
            get
            {
                List<Branch> model = new List<Branch>();
                _branchRepository.FindAll.ToList().ForEach(item =>
                {
                    model.Add(
                            new Branch
                            {
                                BranchID = item.BranchID,
                                BranchCode = item.BranchCode,
                                BranchName = item.BranchName,
                                BranchDescription = item.BranchDescription,
                                Adress=item.Adress
                                //AdressEmail = item.Adress.AdressEmail,
                                //AdressPOBox = item.Adress.AdressPOBox,
                                //AdressPhoneNumber = item.Adress.AdressPhoneNumber,
                            }
                        );
                });
                return model;
            }

        }
        private List<Job> ModelJob
        {
            get
            {
                List<Job> model = new List<Job>();
                _jobRepository.FindAll.ToList().ForEach(item =>
                {
                    model.Add(
                            new Job
                            {
                                JobID = item.JobID,
                                JobCode = item.JobCode,
                                JobLabel = item.JobLabel,
                                JobDescription = item.JobDescription
                            }
                        );
                });
                return model;
            }

        }
        private List<Country> ModelCountry
        {
            get
            {
                List<Country> model = new List<Country>();
                _countryRepository.FindAll.ToList().ForEach(item =>
                {
                    model.Add(
                            new Country
                            {
                                CountryID = item.CountryID,
                                CountryCode = item.CountryCode,
                                CountryLabel = item.CountryLabel
                            }
                        );
                });
                return model;
            }

        }
        private List<Region> ModelRegion
        {
            get
            {
                List<Region> model = new List<Region>();
                _regionRepository.FindAll.ToList().ForEach(item =>
                {
                    model.Add(
                            new Region
                            {
                                RegionID = item.RegionID,
                                RegionCode = item.RegionCode,
                                RegionLabel = item.RegionLabel,
                                Country=item.Country
                            }
                        );
                });
                return model;
            }

        }
        private List<Town> ModelTown
        {
            get
            {
                List<Town> model = new List<Town>();
                _townRepository.FindAll.ToList().ForEach(item =>
                {
                    model.Add(
                            new Town
                            {
                                TownID = item.TownID,
                                TownCode = item.TownCode,
                                TownLabel = item.TownLabel,
                                Region=item.Region
                            }
                        );
                });
                return model;
            }

        }
        private List<Quarter> ModelQuarter
        {
            get
            {
                List<Quarter> model = new List<Quarter>();
                _quarterRepository.FindAll.ToList().ForEach(item =>
                {
                    model.Add(
                            new Quarter
                            {
                                QuarterID = item.QuarterID,
                                QuarterCode = item.QuarterCode,
                                QuarterLabel = item.QuarterLabel,
                                Town = item.Town
                            }
                        );
                });
                return model;
            }

        }
        private List<Bank> ModelBank
        {
            get
            {
                List<Bank> model = new List<Bank>();
                _paymentMethodRepository.FindAll.OfType<Bank>().ToList().ForEach(item =>
                {
                    model.Add(
                            new Bank
                            {
                                ID = item.ID,
                                Name = item.Name,
                                Description = item.Description,
                                Code = item.Code,
                                Account = item.Account != null ? item.Account : null,
                                Branch = item.Branch != null ? item.Branch : null
                            }
                        );
                });
                return model;
            }

        }
        private List<Till> ModelTill
        {
            get
            {
                List<Till> model = new List<Till>();
                _paymentMethodRepository.FindAll.OfType<Till>().ToList().ForEach(item =>
                {
                    UserTill userT = _userTillRepository.FindAll.SingleOrDefault(ut => ut.TillID == item.ID && ut.HasAccess);
                    model.Add(
                            new Till
                            {
                                ID = item.ID,
                                Name = item.Name,
                                Description = item.Description,
                                Code = item.Code,
                                Account = (item.Account != null) ? item.Account : null,
                                User = userT != null ? (userT.USer.Name + " " + userT.USer.Description) : "No Ready/No allocate",
                                Branch = (item.Branch != null) ? item.Branch : null
                            }
                        );
                });
                return model;
            }

        }
        //====================== End of list
    }
}