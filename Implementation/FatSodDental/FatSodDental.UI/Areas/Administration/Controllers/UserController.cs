using Ext.Net;
using Ext.Net.MVC;
using FatSod.DataContext.Initializer;
using FatSod.Security.Abstracts;
using FatSod.Security.Entities;
using FatSodDental.UI.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using FatSod.Ressources;
using FatSodDental.UI.Controllers;
using System.Text;
using FatSodDental.UI.Filters;
using System.Web.UI;
using FatSod.DataContext.Concrete;

namespace FatSodDental.UI.Areas.Administration.Controllers
{
    [Authorize(Order = 1)]
    [TakeBusinessDay(Order=2)]
    public class UserController : BaseController
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
        
        public UserController(IRepository<File> fileRepository, IRepository<User> userRepository, IPerson personRepository, IRepository<Profile> profileRepository, IRepository<FatSod.Security.Entities.Region> regionRepository, IRepository<Town> townRepository, IRepository<Quarter> quarterRepository, IRepository<Adress> adressRepository, IRepository<UserBranch> userBranchRepository)
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
        
        public ActionResult Index()
        {
            return View();
        }
        [OutputCache(Duration = 3600)] 
        public ActionResult Utilisateur()
        {
            try
            {

                //Ext.Net.MVC.PartialViewResult rPVResult = new Ext.Net.MVC.PartialViewResult
                //{
                //    ClearContainer = true,
                //    //RenderMode = RenderMode.AddTo,
                //    ContainerId = "Body",
                //    //WrapByScriptTag = false,
                //    Model = ModelUser
                //};
                //Session["Curent_Page"] = VIEW_NAME;
                //Session["Curent_Controller"] = CONTROLLER_NAME;
                //We verify is the current has right to access of this action
                //if (!LoadAction.isAutoriseToGetMenu(SessionProfileID, CodeValue.Security.User.CODE, db))
                //{
                //    RedirectToAction("NotAuthorized", "User");
                //}
                //return rPVResult;
                return View(ModelUser);
            }

            catch (Exception e)
            {
                X.Msg.Alert("Error ", e.Message).Show();
                return this.Direct();
            }

        }
        /// <summary>
        /// Return a Model that use in view
        /// </summary>
        private List<object> ModelUser
        {
            get
            {
                List<object> model = new List<object>();
                LoadComponent.UsersForStore(CurrentUser).ForEach(u =>
                {
                    model.Add(
                            new
                            {
                                GlobalPersonID = u.GlobalPersonID,
                                Name = u.Name,
                                UserLogin = u.UserLogin,
                                AdressEmail = u.AdressEmail,
                                AdressPOBox = u.AdressPOBox,
                                AdressPhoneNumber = u.AdressPhoneNumber,
                                SexLabel = u.SexLabel,
                                Description = u.Description,
                                ProfileLabel = u.ProfileLabel,
                                JobLabel = u.JobLabel,
                                CNI = u.CNI
                            }
                        );
                });
                return model;
            }
        }
        //****************** This method load all users in data base
        [HttpPost]
        public StoreResult GetUsersList()
        {
            return this.Store(ModelUser);
        }

        [HttpPost]
        public ActionResult Add(Adress adress, User user, string[] Branch, int IsConnected, int SexID, HttpPostedFileBase UploadImage, int? ProfileID)
        {
            try
            {
                int i = 0;
                i++;
                user.SexID = SexID;
                user.IsConnected = Convert.ToBoolean(IsConnected);
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
                user.UserAccountState = true;
                if (user.GlobalPersonID > 0)
                {
                    User userToUpdate = (User)_personRepository.Update2(user,SessionGlobalPersonID,SessionBusinessDay(null).BDDateOperation,SessionBusinessDay(null).BranchID);
                    //userToUpdate = (User)_personRepository.Update(user, user.GlobalPersonID);
                    //we update userBranch of this user
                    this.RemoveUserBranch(userToUpdate);
                    this.AllocateUserBranch(Branch, userToUpdate);
                    statusOperation = userToUpdate.Name + " : " + Resources.AlertUpdateAction;
                }
                else
                {
                    _personRepository.Create(user);
                    //we create a differents branch that had allocated to this newUser
                    this.AllocateUserBranch(Branch, user);
                    statusOperation = user.Name + " : " + Resources.AlertAddAction;
                }
                this.AlertSucces(Resources.Success, statusOperation);
                this.GetCmp<FormPanel>("User").Reset();
                this.GetCmp<Store>("UserListToLoad").Reload();
                return this.Direct();
            }
            catch (Exception e)
            {
                X.Msg.Alert("Error", e.Message).Show();
                return this.Direct();
            }

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
                string defaultImgPath = Server.MapPath("~/Content/Images/App/default-img.png");
                byte[] DEFAULT_IMAGE = System.IO.File.ReadAllBytes(defaultImgPath);
                return File(DEFAULT_IMAGE, DEFAULT_CONTENT_TYPE);
            }
            
        }
        [HttpPost]
        public ActionResult InitializeFields(int ID)
        {
            try
            {
                User user = _personRepository.FindAll.OfType<User>().FirstOrDefault(u => u.GlobalPersonID == Convert.ToInt32(ID));
                //Person parameters
                this.GetCmp<FormPanel>("User").Reset(true);
                this.GetCmp<FormPanel>("User").ClearContent();
                this.GetCmp<TextField>("Name").Value = user.Name;
                this.GetCmp<TextField>("GlobalPersonID").Value = user.GlobalPersonID;
                this.GetCmp<TextField>("Description").Value = user.Description;
                this.GetCmp<TextField>("CNI").Value = user.CNI;
                this.GetCmp<TextField>("Matricule").Value = user.Code;
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

                //this.GetCmp<Image>("ImageID");
                this.GetCmp<Radio>(user.Sex.SexLabel).Checked = true;
                if (this.GetCmp<Radio>("Access" + user.UserAccessLevel) != null)
                {
                    this.GetCmp<Radio>("Access" + user.UserAccessLevel).Checked = true;
                }
                if (user.IsConnected)
                {
                    this.GetCmp<Radio>("IsConnected").Checked = true;
                }
                else
                {
                    this.GetCmp<Radio>("NoConnected").Checked = true;
                }
                this.GetCmp<TextField>("AdressID").Value = user.Adress.AdressID;
                this.GetCmp<TextField>("AdressFax").Value = user.Adress.AdressFax;
                this.GetCmp<TextField>("AdressPhoneNumber").Value = user.Adress.AdressPhoneNumber;
                this.GetCmp<TextField>("AdressEmail").Value = user.Adress.AdressEmail;
                this.GetCmp<TextField>("AdressPOBox").Value = user.Adress.AdressPOBox;
                //quartier
                //affichage des combo box
                this.GetCmp<ComboBox>("Region").Disabled = false;
                this.GetCmp<ComboBox>("Town").Disabled = false;
                this.GetCmp<ComboBox>("Quarter").Disabled = false;

                //mise à jour de l'affichage
                this.GetCmp<ComboBox>("Country").SetValueAndFireSelect(user.Adress.Quarter.Town.Region.CountryID);
                this.GetCmp<ComboBox>("Region").SetValueAndFireSelect(user.Adress.Quarter.Town.RegionID);
                this.GetCmp<ComboBox>("Town").SetValueAndFireSelect(user.Adress.Quarter.TownID);
                this.GetCmp<ComboBox>("Quarter").SetValue(user.Adress.QuarterID);
                //Job
                this.GetCmp<ComboBox>("Job").Text = user.Job.JobLabel;
                this.GetCmp<ComboBox>("Job").Value = user.Job.JobID;
                //User parameters
                this.GetCmp<TextField>("UserLogin").Hidden = !user.IsConnected;
                //this.GetCmp<TextField>("UserID").Value = user.GlobalPersonID;
                this.GetCmp<ComboBox>("Profile").Hidden = !user.IsConnected;
                this.GetCmp<TextField>("UserPassword").Hidden = !user.IsConnected;
                this.GetCmp<TextField>("UserPassword2").Hidden = !user.IsConnected;
                this.GetCmp<TextField>("UserLogin").Value = user.UserLogin;
                this.GetCmp<TextField>("UserPassword").Value = user.UserPassword;
                this.GetCmp<TextField>("UserPassword2").Value = user.UserPassword;
                this.GetCmp<ComboBox>("Profile").Text = user.Profile.ProfileLabel;
                this.GetCmp<ComboBox>("Profile").Value = user.Profile.ProfileID;
                foreach (var userBranch in _userBranchRepository.FindAll.Where(ub => ub.UserID == user.GlobalPersonID).ToList())
                {
                    this.GetCmp<MultiCombo>("Branch").SelectItem("" + userBranch.BranchID);
                }
            }
            catch (Exception e)
            {
                statusOperation = @"Une erreur s'est produite lors de l'opération, veuillez contactez l'administrateur et/ou essayez à nouveau
                                     <br/>Code : <code>" + e.Message + "</code>";
                X.Msg.Alert("Error", statusOperation).Show();
                return this.Direct();
            }
            return this.Direct();
        }
        //***************Delete user
        [HttpPost]
        public ActionResult Delete(int ID)
        {
            Person user = new User() { GlobalPersonID = ID };
            try
            {
                _personRepository.Delete(user);
            }
            catch (Exception e)
            {
                X.Msg.Alert("Error", e.Message).Show();
                return this.Direct();
            }
            this.AlertSucces(Resources.Success, "" + Resources.AlertDeleteAction);
            this.GetCmp<Store>("UserListToLoad").Reload();
            return this.Direct();
        }
        ////****** This method return a towns list of one country
        //****** This method return a towns list of one country
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
        public StoreResult Towns(string regionID)
        {
            List<object> model = new List<object>();
            _townRepository.FindAll.Where(t => t.RegionID == Convert.ToInt32(regionID)).ToList().ForEach(t =>
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
            _quarterRepository.FindAll.Where(t => t.TownID == Convert.ToInt32(townID)).ToList().ForEach(t =>
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
        //************** Alloacate and Delete all branch of one user
        private void AllocateUserBranch(string[] Branch, User user)
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
        //This action load a default view for not authorize user
        public ActionResult NotAuthorized()
        {
            Ext.Net.MVC.PartialViewResult rPVResult = new Ext.Net.MVC.PartialViewResult
            {
                ContainerId = "Body",
                ClearContainer = true
            };
            return rPVResult;
        }
    }
}