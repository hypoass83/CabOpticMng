using FatSod.DataContext.Initializer;
using FatSod.Security.Abstracts;
using FatSod.Security.Entities;
using CABOPMANAGEMENT.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using FatSod.Ressources;
using CABOPMANAGEMENT.Controllers;
using System.Text;
using CABOPMANAGEMENT.Filters;
using System.Web.UI;
using FatSod.DataContext.Concrete;
using FatSod.Supply.Entities;

namespace CABOPMANAGEMENT.Areas.Administration.Controllers
{
    [Authorize(Order = 1)]
    [TakeBusinessDay(Order = 2)]
    public class SuspendedUserController : BaseController
    {
        //private IUser userRepository;
        private IPerson _personRepository;
        private IRepository<Adress> _adressRepository;
        private IRepository<User> _userRepository;
        private IRepository<Profile> _profileRepository;
        private IRepository<File> _fileRepository;
        private IRepository<FatSod.Security.Entities.Region> _regionRepository;
        private IRepository<Town> _townRepository;
        private IRepository<Quarter> _quarterRepository;
        private IRepository<UserBranch> _userBranchRepository;
        // GET: Administration/User
        //Current Controller and current page
        private const string CONTROLLER_NAME = "Administration/User";
        private const string VIEW_NAME = "Utilisateur";
        //this is a default image in a view
        private const string DEFAULT_CONTENT_TYPE = "image/png";
        private const string defaultImgPath = "";
        private const string DEFAULT_IMAGE_PATH = "";

        public SuspendedUserController(IRepository<File> fileRepository, IRepository<User> userRepository, IPerson personRepository, IRepository<Profile> profileRepository, IRepository<FatSod.Security.Entities.Region> regionRepository, IRepository<Town> townRepository, IRepository<Quarter> quarterRepository, IRepository<Adress> adressRepository, IRepository<UserBranch> userBranchRepository)
        {
            this._userRepository = userRepository;
            this._personRepository = personRepository;
            this._profileRepository = profileRepository;
            this._regionRepository = regionRepository;
            this._townRepository = townRepository;
            this._quarterRepository = quarterRepository;
            this._adressRepository = adressRepository;
            this._fileRepository = fileRepository;
            this._userBranchRepository = userBranchRepository;

        }



        /**
         * List of Jobs to go Inside Select Tab
         **/
        public JsonResult populateJobID()
        {
            //holds list of Jobs 
            List<object> _Jobs = new List<object>();

            //queries all the Jobs for its ID and Name property.
            var Jobs = (from s in db.Jobs
                        select new { s.JobID, s.JobLabel, s.JobCode }).ToList();

            //save list of Jobs to the _Jobs
            foreach (var item in Jobs.OrderBy(i => i.JobCode))
            {
                _Jobs.Add(new
                {
                    ID = item.JobID,
                    Name = item.JobCode + " " + item.JobLabel
                });
            }
            //returns the Json result of _Jobs
            return Json(_Jobs, JsonRequestBehavior.AllowGet);
        }

        /**
         * List of Profile to go Inside Select Tab
         **/
        public JsonResult populateProfileID()
        {
            //holds list of Jobs 
            List<object> _ProfileID = new List<object>();

            //queries all the Jobs for its ID and Name property.
            var Profiles = (from s in db.Profiles
                            select new { s.ProfileID, s.ProfileLabel, s.ProfileCode }).ToList();

            //save list of Jobs to the _Jobs
            foreach (var item in Profiles.OrderBy(i => i.ProfileCode))
            {
                _ProfileID.Add(new
                {
                    ID = item.ProfileID,
                    Name = item.ProfileCode + " " + item.ProfileLabel
                });
            }
            //returns the Json result of _Jobs
            return Json(_ProfileID, JsonRequestBehavior.AllowGet);
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
            //var Quarters = (from s in db.Quarters where s.TownID == _townId
            //                select new { s.TownID, s.QuarterID, s.QuarterLabel, s.QuarterCode }).ToList();

            ////save list of Towns to the _Quarters
            //foreach (var item in Quarters.OrderBy(i => i.QuarterCode))
            List<Quarter> res = new List<Quarter>();

            IEqualityComparer<Quarter> locationComparer = new GenericComparer<Quarter>("QuarterCode");
            res = _quarterRepository.FindAll.Where(s => s.TownID == _townId)
                .Distinct(locationComparer).ToList();
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


        /**
        * List of UserBranches to go Inside Select Tab
        **/
        public JsonResult populateUserBranch()
        {
            //holds list of UserBranches 
            List<object> _UserBranches = new List<object>();

            //queries all the Branches for ID and Name property.
            var Branches = (from s in db.Branches
                            select new { s.BranchID, s.BranchName, s.BranchCode }).ToList();

            //queries all the UserBranches for ID and Name property.
            int id = Convert.ToInt32(Session["UserID"]) + 0;
            var UserBranches = (from s in db.UserBranches
                                where s.UserID.Equals(id)
                                select new { s.BranchID }).ToList();


            //save list of UserBranches to the _UserBranches
            foreach (var item in Branches.OrderBy(i => i.BranchCode))
            {
                foreach (var ub in UserBranches)
                {
                    if (item.BranchID == ub.BranchID)
                    {
                        _UserBranches.Add(new
                        {
                            ID = item.BranchID,
                            Name = item.BranchCode + " " + item.BranchName
                        });
                    }
                }
            }
            //returns the Json result of _UserBranches
            return Json(_UserBranches, JsonRequestBehavior.AllowGet);
        }

        [OutputCache(Duration = 3600)]
        public ActionResult Index()
        {
            return View(ModelUser);
        }
        /// <summary>
        /// Return a Model that use in view
        /// </summary>
        private List<User> ModelUser
        {
            get
            {
                List<User> model = new List<User>();
                LoadComponent.SuspendedUsersForStore(CurrentUser, SessionBusinessDay(null).BranchID).ForEach(u =>
                {
                    model.Add(
                            new User
                            {
                                GlobalPersonID = u.GlobalPersonID,
                                Name = u.Name,
                                UserLogin = u.UserLogin,
                                Adress = u.Adress,
                                Sex = u.Sex,
                                Description = u.Description,
                                Profile = u.Profile,
                                Job = u.Job,
                                CNI = u.CNI,
                                UserAccountState = u.UserAccountState
                            }
                        );
                });
                return model;
            }
        }
        //****************** This method load all users in data base


        //[HttpPost]
        public JsonResult AddUser(Adress adress, User user, string[] Branch, int IsConnected, int SexID, HttpPostedFileBase UploadImage, int? ProfileID, int UserAccountState)
        {
            bool status = false;
            string Message = "";
            try
            {
                int i = 0;
                i++;
                user.SexID = SexID;
                user.IsConnected = Convert.ToBoolean(IsConnected);
                //user.IsUserConnected = false;
                //We try to upload an user photo
                if (UploadImage != null && UploadImage.ContentLength > 0)
                {
                    var photo = new File
                    {
                        FileName = System.IO.Path.GetFileName(UploadImage.FileName),
                        FileType = FileType.Photo,
                        ContentType = UploadImage.ContentType
                    };
                    using (var reader = new System.IO.BinaryReader(UploadImage.InputStream))
                    {
                        photo.Content = reader.ReadBytes(UploadImage.ContentLength);
                    }
                    user.Files = new List<File> { photo };
                }
                if (!user.IsConnected)
                {
                    user.ProfileID = _profileRepository.FindAll.FirstOrDefault(p => p.ProfileCode == CodeValue.Security.Profile.ClASS_CODE).ProfileID;
                    user.UserLogin = "userLogin";
                    user.UserPassword = "userPass";
                }
                else
                {
                    user.ProfileID = (int)ProfileID;
                }
                user.Adress = adress;
                user.UserAccountState = Convert.ToBoolean(UserAccountState);// true;

                DateTime OperationDate = SessionBusinessDay(null).BDDateOperation;
                int BranchID = SessionBusinessDay(null).BranchID;

                if (user.GlobalPersonID > 0)
                {

                    User userToUpdate = (User)_personRepository.Update2(user, SessionGlobalPersonID, OperationDate, BranchID);
                    //userToUpdate = (User)_personRepository.Update(user, user.GlobalPersonID);
                    //we update userBranch of this user
                    this.RemoveUserBranch(userToUpdate);
                    this.AllocatedUserBranch(Branch, userToUpdate);
                    statusOperation = userToUpdate.Name + " : " + Resources.AlertUpdateAction;
                }
                else
                {
                    //_personRepository.Create(user);
                    _personRepository.Create2User(user, SessionGlobalPersonID, OperationDate, BranchID);
                    //we create a differents branch that had allocated to this newUser
                    this.AllocatedUserBranch(Branch, user);
                    statusOperation = user.Name + " : " + Resources.AlertAddAction;
                }

                status = true;
                Message = statusOperation;
            }
            catch (Exception e)
            {
                status = false;
                Message = @"Une erreur s'est produite lors de l'opération, veuillez contactez l'administrateur et/ou essayez à nouveau
                                     <br/>Code : <code>" + e.Message + "</code>";

            }
            return new JsonResult { Data = new { status = status, Message = Message } };
        }


        [AllowAnonymous]
        public ActionResult File(int id)
        {
            if (id > 0)
            {
                var fileToRetrieve = _fileRepository.FindAll.FirstOrDefault(f => f.FileID == id);
                return File(fileToRetrieve.Content, fileToRetrieve.ContentType);
            }
            else
            {
                string defaultImgPath = Server.MapPath("~/Images/users/default-avatar.png");
                byte[] DEFAULT_IMAGE = System.IO.File.ReadAllBytes(defaultImgPath);
                return File(DEFAULT_IMAGE, DEFAULT_CONTENT_TYPE);
            }

        }
        public JsonResult Edit(int id)
        {
            User userEntity = _userRepository.Find(id);
            //recuperation du branch du id
            Branch bruser = db.UserBranches.Where(u => u.UserID == userEntity.GlobalPersonID).FirstOrDefault().Branch;
            //returns the Json result of _BeneficiariesList
            List<object> _UserList = new List<object>();
            _UserList.Add(new
            {
                GlobalPersonID = userEntity.GlobalPersonID,
                Name = userEntity.Name,
                Description = userEntity.Description,
                Code = userEntity.Code,
                CNI = userEntity.CNI,
                SexID = userEntity.SexID,
                JobID = userEntity.JobID,
                IsConnected = Convert.ToInt16(userEntity.IsConnected),
                AdressPhoneNumber = userEntity.Adress.AdressPhoneNumber,
                AdressEmail = userEntity.Adress.AdressEmail,
                AdressPOBox = userEntity.Adress.AdressPOBox,
                AdressFax = userEntity.Adress.AdressFax,
                Country = userEntity.Adress.Quarter.Town.Region.Country.CountryID,
                Region = userEntity.Adress.Quarter.Town.Region.RegionID,
                Town = userEntity.Adress.Quarter.Town.TownID,
                QuarterID = userEntity.Adress.Quarter.QuarterID,
                UserLogin = userEntity.UserLogin,
                UserPassword = userEntity.UserPassword,
                ProfileID = userEntity.ProfileID,
                Branch = bruser.BranchID,
                UserAccessLevel = userEntity.UserAccessLevel,
                UserAccountState = userEntity.UserAccountState,
            });
            return Json(_UserList, JsonRequestBehavior.AllowGet);
        }

        //***************Delete user
        //    [HttpPost]
        public JsonResult Delete(int ID)
        {
            //Person user =  new User() { GlobalPersonID = ID };
            bool status = false;
            User user = _personRepository.FindAll.OfType<User>().FirstOrDefault(u => u.GlobalPersonID == ID);
            try
            {
                //remove user branch before user
                RemoveUserBranch(user);
                _personRepository.Delete(user);
                status = true;
                statusOperation = user.Name + " : " + Resources.AlertDeleteAction;
            }
            catch (Exception e)
            {
                status = false;
                statusOperation = user.Name + " : " + e.Message + " - " + e.InnerException;
                //return View("Utilisateur");
            }

            return new JsonResult { Data = new { status = status, Message = statusOperation } };
        }

        //************** Alloacate and Delete all branch of one user
        private void AllocatedUserBranch(string[] Branch, User user)
        {
            int i = 0;
            while (i < Branch.Length)
            {
                UserBranch userBranch = new UserBranch() { UserID = user.GlobalPersonID, BranchID = Convert.ToInt32(Branch[i]) };
                _userBranchRepository.Create(userBranch);
                i++;
            }
        }
        private void RemoveUserBranch(User user)
        {
            int i = 0;
            foreach (var userBranch in _userBranchRepository.FindAll.Where(ub => ub.UserID == user.GlobalPersonID))
            {
                UserBranch userBranchToDelete = _userBranchRepository.FindAll.First(ub => ub.BranchID == userBranch.BranchID && ub.UserID == user.GlobalPersonID);
                _userBranchRepository.Delete(userBranchToDelete);
                i++;
            }
        }

    }
}