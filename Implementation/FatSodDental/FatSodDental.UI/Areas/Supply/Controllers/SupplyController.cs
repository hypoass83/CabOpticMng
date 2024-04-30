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
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FatSodDental.UI.Areas.Supply.Controllers
{
    [Authorize(Order = 1)]
    [TakeBusinessDay(Order = 2)]
    public class SupplyController : BaseController
    {
        private IRepositorySupply<Supplier> _supplierRepository;
        private IRepository<Adress> _adresseRepository;

        private IAccount _accountRepository;
        

        //Current Controller and current page
        private const string CONTROLLER_NAME = "Supply/" + CodeValue.Supply.SupplierMenu.CONTROLLER;
        private const string VIEW_NAME = CodeValue.Supply.SupplierMenu.PATH;

        public SupplyController(IRepository<Adress> adresseRepository,
            IRepositorySupply<Supplier> supplierRepository,
            IAccount accountRepository)
        {
            this._supplierRepository = supplierRepository;
            this._adresseRepository = adresseRepository;
            this._accountRepository = accountRepository;
            
        }

        public ActionResult Supply()
        {

            //Session["Curent_Page"] = VIEW_NAME;
            //Session["Curent_Controller"] = CONTROLLER_NAME;
            //We verify is the current has right to access of this action
            //if (!LoadAction.isAutoriseToGetMenu(SessionProfileID, CodeValue.Supply.SupplierMenu.CODE, db))
            //{
            //    RedirectToAction("NotAuthorized", "User");
            //}
            //this.GetCmp<Panel>("Body").LoadContent(new ComponentLoader
            //{
            //    Url = Url.Action("Supply"),
            //    DisableCaching = false,
            //    Mode = LoadMode.Frame
            //});
            
            //Ext.Net.MVC.PartialViewResult rPVResult = new Ext.Net.MVC.PartialViewResult
            //{
            //    ClearContainer = true,
            //    //RenderMode = RenderMode.AddTo,
            //    ContainerId = "Body",
            //    //WrapByScriptTag = false,
            //    Model = ModelSupplier()
            //};
            return View(ModelSupplier());
        }

        public ActionResult Add(Adress adress, Supplier supplier, int Account, HttpPostedFileBase UploadImage)
        {
          
            //We add image if exist in sending form
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
                supplier.Files = new List<File> { photo };
            }
            if (supplier.GlobalPersonID > 0)
            {
                //mise à jour
                supplier.Adress = null;
                supplier.AdressID = 0;
                _adresseRepository.Update(adress, adress.AdressID);
                supplier.Adress = adress;
                supplier.AdressID = adress.AdressID;
                //recuperation du cpte existant
                supplier.AccountID = db.Suppliers.SingleOrDefault(c => c.GlobalPersonID == supplier.GlobalPersonID).AccountID;
                //est il possible de changer le numro de cpte d'un founisseur?
                _supplierRepository.Update(supplier, supplier.GlobalPersonID);
                this.AlertSucces(Resources.Success, "Supplier has been updated");
            }
            else
            {
                //ajout
                try
                {
                    supplier.Adress = adress;
                    //fabrication du nvo cpte
                    supplier.AccountID = _accountRepository.GenerateAccountNumber(Account, supplier.SupplierFullName + " " + Resources.UIAccount, false).AccountID;
                    _supplierRepository.Create(supplier);
                    this.AlertSucces(Resources.Success, "Supplier has been added");
                }
                catch (SqlException ex)
                {
                    if (ex.Message.Contains("UniqueConstraint"))
                    {
                        X.Msg.Alert("Unique Constraint Exception",
                                    "You Have Create One or Many items which alredy exist. Check your fields for modificationsp "
                        ).Show();
                    }

                } 
            }

            return this.Reset();
        }

       
        public ActionResult Reset()
        {
            this.GetCmp<FormPanel>("SupplierForm").Reset();
            this.GetCmp<ComboBox>("PersonType").ReadOnly = false;
            this.GetCmp<Store>("SupplierListStore").Reload();
            this.GetCmp<NumberField>("SupplierNumber").ReadOnly = false;
            this.GetCmp<ComboBox>("Account").ReadOnly = false;
            return this.Direct();
        }
        //this method return a file photo
        public ActionResult File(int id)
        {
            var fileToRetrieve = db.Files.Find(id);
            return File(fileToRetrieve.Content, fileToRetrieve.ContentType);
        }
        //Initialize fields to update
        [HttpPost]
        public ActionResult InitializeFields(int ID)
        {

            Supplier supplier = db.Suppliers.Find(ID);
            //Person parameters
            this.GetCmp<FormPanel>("SupplierForm").Reset(true);
            
            //informations générales à toutes les personnes
            this.GetCmp<TextField>("GlobalPersonID").Value = supplier.GlobalPersonID;
            this.GetCmp<TextField>("Name").Value = supplier.Name;
            this.GetCmp<TextField>("CNI").Value = supplier.CNI;
            this.GetCmp<TextField>("Description").Value = supplier.Description;


            if (supplier.CompanySigle != null)
            {
                //on a une personne morale
                this.GetCmp<NumberField>("CompanyCapital").Value = supplier.CompanyCapital;
                this.GetCmp<TextField>("CompanyTradeRegister").Value = supplier.CompanyTradeRegister;
                this.GetCmp<TextField>("CompanySigle").Value = supplier.CompanySigle;
                this.GetCmp<TextField>("CompanySlogan").Value = supplier.CompanySlogan;

                this.GetCmp<ComboBox>("PersonType").SetValueAndFireSelect(2);
            }

            if (supplier.CompanySigle == null)
            {
                //on a une personne physique              
                //sex
                this.GetCmp<Radio>("Masculin").Value = supplier.Sex.SexID;
                this.GetCmp<Radio>("Feminin").Value = supplier.Sex.SexID;

                this.GetCmp<ComboBox>("PersonType").SetValueAndFireSelect(1);
            }

            this.GetCmp<ComboBox>("PersonType").ReadOnly = true;


            
            //We load image if exist
            var fileToRetrieve = db.Files.FirstOrDefault(f => f.GlobalPersonID == ID);
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

            //this.GetCmp<Radio>(supplier.Sex.SexLabel).Checked = true;
            //Adress 
            this.GetCmp<TextField>("AdressID").Value = supplier.Adress.AdressID;
            this.GetCmp<TextField>("AdressFax").Value = supplier.Adress.AdressFax;
            this.GetCmp<TextField>("AdressPhoneNumber").Value = supplier.Adress.AdressPhoneNumber;
            this.GetCmp<TextField>("AdressEmail").Value = supplier.Adress.AdressEmail;
            this.GetCmp<TextField>("AdressPOBox").Value = supplier.Adress.AdressPOBox;
            //quartier
            //affichage des combo box
            this.GetCmp<ComboBox>("Region").Disabled = false;
            this.GetCmp<ComboBox>("Town").Disabled = false;
            this.GetCmp<ComboBox>("Quarter").Disabled = false;

            //mise à jour de l'affichage
            this.GetCmp<ComboBox>("Country").SetValueAndFireSelect(supplier.Adress.Quarter.Town.Region.CountryID);
            this.GetCmp<ComboBox>("Region").SetValueAndFireSelect(supplier.Adress.Quarter.Town.RegionID);
            this.GetCmp<ComboBox>("Town").SetValueAndFireSelect(supplier.Adress.Quarter.TownID);
            this.GetCmp<ComboBox>("Quarter").SetValue(supplier.Adress.QuarterID);

            //Account parameters

            this.GetCmp<TextField>("SupplierNumber").Value = supplier.SupplierNumber;
            this.GetCmp<TextField>("SupplierNumber").ReadOnly = true;
            this.GetCmp<ComboBox>("Account").Value = supplier.AccountID;
            this.GetCmp<ComboBox>("Account").ReadOnly = true;

            return this.Direct();
        }
        //Delete Action
        [HttpPost]
        public ActionResult Delete(int ID)
        {
            _supplierRepository.Delete(ID);
            this.AlertSucces(Resources.Success, "Supplier has been deleted");
            this.GetCmp<FormPanel>("SupplierForm").Reset();
            this.GetCmp<Store>("SupplierListStore").Reload();
            return this.Direct();
        }
        public List<object> ModelSupplier()
        {
            List<Supplier> listSupplier = _supplierRepository.FindAll.ToList();
            List<object> list = new List<object>();
            listSupplier.ForEach(c =>
            {
                list.Add(
                                                new
                                                {
                                                    Name = c.Name,
                                                    Description = c.Description,
                                                    CNI = c.CNI,
                                                    SexLabel1 = c.SexLabel1,
                                                    AdressPhoneNumber1 = c.AdressPhoneNumber1,
                                                    AdressPOBox1 = c.AdressPOBox1,
                                                    AdressEmail1 = c.AdressEmail1,
                                                    AccountLabel1 = c.AccountLabel1,
                                                    GlobalPersonID = c.GlobalPersonID
                                                }
                                );
            });
            return list;
        }
        [HttpGet]
        public StoreResult GetAllSuppliers()
        {
            
            return this.Store(ModelSupplier());
        }
        public ActionResult Regions(string countryID)
        {
            int id=Convert.ToInt32(countryID);
            List<FatSod.Security.Entities.Region> listregion = db.Regions.Where(t => t.CountryID == id).ToList();
            List<object> list = new List<object>();
            listregion.ForEach(c =>
            {
                list.Add(
                                                new
                                                {
                                                    RegionID = c.RegionID,
                                                    RegionLabel = c.RegionLabel
                                                }
                                );
            });
            return this.Store(list);
        }
        public ActionResult Towns(string regionID)
        {
            int id =Convert.ToInt32(regionID);
            List<Town> listregion = db.Towns.Where(t => t.RegionID == id).ToList();
            List<object> list = new List<object>();
            listregion.ForEach(c =>
            {
                list.Add(
                                                new
                                                {
                                                    TownID = c.TownID,
                                                    TownLabel = c.TownLabel
                                                }
                                );
            });
            return this.Store(list);
        }
        public ActionResult Quarters(string townID)
        {
            int id =Convert.ToInt32(townID);
            List<Quarter> listQuarter = db.Quarters.Where(t => t.TownID == id).ToList();
            List<object> list = new List<object>();
            listQuarter.ForEach(c =>
            {
                list.Add(
                                                new
                                                {
                                                    QuarterID = c.QuarterID,
                                                    QuarterLabel = c.QuarterLabel
                                                }
                                );
            });
            return this.Store(list);
        }
    }
}