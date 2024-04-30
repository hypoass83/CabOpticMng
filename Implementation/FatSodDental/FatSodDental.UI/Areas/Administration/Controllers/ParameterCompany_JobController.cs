using Ext.Net;
using Ext.Net.MVC;
using FatSodDental.UI.Controllers;
using System.Web.Mvc;
using FatSod.Security.Abstracts;
using FatSod.Security.Entities;
using FatSodDental.UI.Tools;
using System;
using System.Linq;
using System.Web;
using FatSod.DataContext.Initializer;
using FatSod.Ressources;
using System.Collections.Generic;
using FatSod.Supply.Entities;
using FatSodDental.UI.Filters;
using System.Web.UI;


namespace FatSodDental.UI.Areas.Administration.Controllers
{

    public partial class ParameterController : BaseController
    {
        /***** Company, job management actions ***/
        //************ Add action
        [HttpPost]
        public ActionResult AddCompany(Company company, Adress adress, HttpPostedFileBase UploadImage, int FileID)
        {
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
                this.AlertSucces(Resources.Success, statusOperation);
                //We load image if exist
                var fileToRetrieve = _fileRepository.FindAll.LastOrDefault(f => f.GlobalPersonID == company.GlobalPersonID);
                if (fileToRetrieve != null)
                {
                    this.GetCmp<Image>("ImageID").Hidden = false;
                    this.GetCmp<TextField>("FileID").Value = fileToRetrieve.FileID;
                    this.GetCmp<Image>("ImageID").ImageUrl = "~/User/File?id=" + fileToRetrieve.FileID;
                }
                else
                {
                    this.GetCmp<Image>("ImageID").ImageUrl = "~/User/File?id=0";
                }
                this.GetCmp<Store>("BranchesList").Reload();
                return this.Direct();
            }
            catch (Exception e) { X.Msg.Alert("Error ", e.Message).Show(); return this.Direct(); }

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
            //Ext.Net.MVC.PartialViewResult rPVResult = new Ext.Net.MVC.PartialViewResult
            //{
            //    ClearContainer = true,
            //    //RenderMode = RenderMode.AddTo,
            //    ContainerId = "Body",
            //    WrapByScriptTag = false
            //};
            //if (!LoadAction.isAutoriseToGetSubMenu(SessionProfileID, CodeValue.Parameter.CompanyCode, db))
            //{
            //    RedirectToAction("NotAuthorized", "User");
            //}
            
            var company = _companyRepository.FindAll.FirstOrDefault();
            ViewBag.Company = company;

			this.InitInfo(company);
            var fileToRetrieve = _fileRepository.FindAll.SingleOrDefault(f => f.GlobalPersonID == company.GlobalPersonID);
            ViewBag.CompanyLogoID = fileToRetrieve != null ? fileToRetrieve.FileID : 0;
            return View();
        }
			public void InitInfo(Company company)
				{
					this.GetCmp<ComboBox>("Country").SetValueAndFireSelect(company.Adress.Quarter.Town.Region.CountryID);
					this.GetCmp<ComboBox>("Region").SetValueAndFireSelect(company.Adress.Quarter.Town.RegionID);
					this.GetCmp<ComboBox>("Town").SetValueAndFireSelect(company.Adress.Quarter.TownID);
					this.GetCmp<ComboBox>("Quarter").SetValue(company.Adress.QuarterID);
				}
        //List of jobs
        //[OutputCache(Duration = 3600)] 
        public ActionResult Job()
        {
            //Ext.Net.MVC.PartialViewResult rPVResult = new Ext.Net.MVC.PartialViewResult
            //{
            //    ClearContainer = true,
            //    //RenderMode = RenderMode.AddTo,
            //    ContainerId = "Body",
            //    //WrapByScriptTag = false,
            //    Model = ModelJob
            //};
            //if (!LoadAction.isAutoriseToGetSubMenu(SessionProfileID, CodeValue.Parameter.JobCode, db))
            //{
            //    RedirectToAction("NotAuthorized", "User");
            //}

            return View(ModelJob);
        }
        //[OutputCache(Duration = 3600)] 
        public ActionResult Branch()
        {
            //Ext.Net.MVC.PartialViewResult rPVResult = new Ext.Net.MVC.PartialViewResult
            //{
            //    ClearContainer = true,
            //    //RenderMode = RenderMode.AddTo,
            //    ContainerId = "Body",
            //    //WrapByScriptTag = false,
            //    Model = ModelBranch
            //};
            //this.Parameters();
            //if (!LoadAction.isAutoriseToGetSubMenu(SessionProfileID, CodeValue.Parameter.Branch.MenuCODE, db))
            //{
            //    RedirectToAction("NotAuthorized", "User");
            //}

            return View(ModelBranch);
        }
        //public StoreResult JobsList()
        //{
        //    return this.Store(ModelJob);
        //}
        public ActionResult InitializeJobsFields(int ID)
        {
            Job job = _jobRepository.FindAll.SingleOrDefault(j => j.JobID == ID);
            this.GetCmp<FormPanel>("JobForm").Reset(true);
            this.GetCmp<TextField>("JobID").Value = job.JobID;
            this.GetCmp<TextField>("JobCode").Value = job.JobCode;
            this.GetCmp<TextField>("JobLabel").Value = job.JobLabel;
            this.GetCmp<TextArea>("JobDescription").Value = job.JobDescription;
            return this.Direct();
        }
        [HttpPost]
        public ActionResult AddJob(Job job)
        {
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
                this.AlertSucces(Resources.Success, statusOperation);
                this.GetCmp<FormPanel>("JobForm").Reset();
                this.GetCmp<Store>("JobStore").Reload();
                return this.Direct();
            }
            catch (Exception e) { X.Msg.Alert("Error ", e.Message).Show(); return this.Direct(); }
        }
        [HttpPost]
        public ActionResult AddBranch(Branch branch, Adress adress, int QuarterID)
        {
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
                            BDDateOperation = openedBusDays.BDDateOperation,
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
                this.AlertSucces(Resources.Success, statusOperation);
                this.GetCmp<FormPanel>("BranchForm").Reset();
                this.GetCmp<Store>("BranchesList").Reload();
                return this.Direct();
            }
            catch (Exception e) { X.Msg.Alert("Error ", e.Message).Show(); return this.Direct(); }
        }
        //============= differents store of parameters
        [HttpPost]
        public StoreResult GetBranchList()
        {
            return this.Store(ModelBranch);
        }
        [HttpPost]
        public StoreResult GetTownsList()
        {
            return this.Store(ModelTown);
        }
        [HttpPost]
        public StoreResult GetQuartersList()
        {
            return this.Store(ModelQuarter);
        }
        [HttpPost]
        public StoreResult GetRegionsList()
        {
            return this.Store(ModelRegion);
        }
        [HttpPost]
        public StoreResult GetCountrysList()
        {
            return this.Store(ModelCountry);
        }
        [HttpPost]
        public StoreResult GetJobsList()
        {
            return this.Store(ModelJob);
        }
        //Delete job action
        public ActionResult DeleteJob(int ID)
        {
            try
            {
                statusOperation = "Job : " + Resources.AlertDeleteAction;
                _jobRepository.Delete(ID);
            }
            catch (Exception e)
            {
                statusOperation = "Error : " + e.Message;
                X.Msg.Alert("Error", statusOperation).Show();
                return this.Direct();
            }
            this.AlertSucces(Resources.Success, statusOperation);
            this.GetCmp<Store>("JobStore").Reload();
            return this.Direct();
        }
        //===================List of Model of differents enities==================================
        private List<object> ModelBranch
        {
            get
            {
                List<object> model = new List<object>();
                _branchRepository.FindAll.ToList().ForEach(item =>
                {
                    model.Add(
                            new
                            {
                                BranchID = item.BranchID,
                                BranchCode = item.BranchCode,
                                BranchName = item.BranchName,
                                BranchDescription = item.BranchDescription,
                                AdressEmail = item.Adress.AdressEmail,
                                AdressPOBox = item.Adress.AdressPOBox,
                                AdressPhoneNumber = item.Adress.AdressPhoneNumber,
                            }
                        );
                });
                return model;
            }

        }
        private List<object> ModelJob
        {
            get
            {
                List<object> model = new List<object>();
                _jobRepository.FindAll.ToList().ForEach(item =>
                {
                    model.Add(
                            new
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
        private List<object> ModelCountry
        {
            get
            {
                List<object> model = new List<object>();
                _countryRepository.FindAll.ToList().ForEach(item =>
                {
                    model.Add(
                            new
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
        private List<object> ModelRegion
        {
            get
            {
                List<object> model = new List<object>();
                _regionRepository.FindAll.ToList().ForEach(item =>
                {
                    model.Add(
                            new
                            {
                                RegionID = item.RegionID,
                                RegionCode = item.RegionCode,
                                RegionLabel = item.RegionLabel
                            }
                        );
                });
                return model;
            }

        }
        private List<object> ModelTown
        {
            get
            {
                List<object> model = new List<object>();
                _townRepository.FindAll.ToList().ForEach(item =>
                {
                    model.Add(
                            new
                            {
                                TownID = item.TownID,
                                TownCode = item.TownCode,
                                TownLabel = item.TownLabel
                            }
                        );
                });
                return model;
            }

        }
        private List<object> ModelQuarter
        {
            get
            {
                List<object> model = new List<object>();
                _quarterRepository.FindAll.ToList().ForEach(item =>
                {
                    model.Add(
                            new
                            {
                                QuarterID = item.QuarterID,
                                QuarterCode = item.QuarterCode,
                                QuarterLabel = item.QuarterLabel,
                                QuarterTown = item.Town.TownLabel
                            }
                        );
                });
                return model;
            }

        }
        private List<object> ModelBank
        {
            get
            {
                List<object> model = new List<object>();
                _paymentMethodRepository.FindAll.OfType<Bank>().ToList().ForEach(item =>
                {
                    model.Add(
                            new
                            {
                                ID = item.ID,
                                Name = item.Name,
                                Description = item.Description,
                                Code = item.Code,
                                AccountID = item.Account != null ? item.Account.AccountNumber : item.AccountID,
                                BranchID = item.Branch != null ? item.Branch.BranchName : "Not Ready"
                            }
                        );
                });
                return model;
            }

        }
        private List<object> ModelTill
        {
            get
            {
                List<object> model = new List<object>();
                _paymentMethodRepository.FindAll.OfType<Till>().ToList().ForEach(item =>
                {
                    UserTill userT = _userTillRepository.FindAll.SingleOrDefault(ut => ut.TillID == item.ID && ut.HasAccess);
                    model.Add(
                            new
                            {
                                ID = item.ID,
                                Name = item.Name,
                                Description = item.Description,
                                Code = item.Code,
                                AccountID = item.Account != null ? item.Account.AccountNumber : item.AccountID,
                                UserManage = userT != null ? (userT.USer.Name + " " + userT.USer.Description) : "No Ready/No allocate",
                                BranchID = item.Branch != null ? item.Branch.BranchName : "Not Ready"
                            }
                        );
                });
                return model;
            }

        }
        //====================== End of list
    }
}