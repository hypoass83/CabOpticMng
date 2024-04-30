using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
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
using System.Web.Mvc;
using System.Web.UI;

namespace FatSodDental.UI.Areas.Supply.Controllers
{
    [Authorize(Order = 1)]
    [TakeBusinessDay(Order = 2)]
    public class InventoryEntryController : BaseController
    {
        //Current Controller and current page

        private const string CONTROLLER_NAME = "Supply/" + CodeValue.Supply.InventoryEntry_SM.CONTROLLER;
        private const string VIEW_NAME = CodeValue.Supply.InventoryEntry_SM.PATH;
        
        private IInventoryDirectory inventoryDirectoryRepository;
        private IBusinessDay _busDayRepo;

        private List<BusinessDay> lstBusDay;
        //Construcitor
        public InventoryEntryController(
            IBusinessDay busDayRepo,
            IInventoryDirectory inventoryDirectoryRepository
            )
        {
            this.inventoryDirectoryRepository = inventoryDirectoryRepository;
            this._busDayRepo = busDayRepo;
            
        }
        [OutputCache(Duration = 3600)] 
        public ActionResult InventoryEntry()
        {
            //Session["Curent_Page"] = VIEW_NAME;
            //Session["Curent_Controller"] = CONTROLLER_NAME;

            //We test if this user has an autorization to get this sub menu
            //if (!LoadAction.isAutoriseToGetMenu(SessionProfileID, CodeValue.Supply.SupplierReturnMenu.CODE, db))
            //{
            //    this.NotAuthorized();
            //}

            //this.GetCmp<Panel>("Body").LoadContent(new ComponentLoader
            //{
            //    Url = Url.Action(VIEW_NAME),
            //    DisableCaching = false,
            //    Mode = LoadMode.Frame
            //});


            //List<InventoryDirectoryLine> InventoryDirectoryLines = new List<InventoryDirectoryLine>();

            Session["FinalInventoryDirectoryLines"] = new List<InventoryDirectoryLine>();
            Session["InitialInventoryDirectoryLines"] = new List<InventoryDirectoryLine>();
            ////Ext.Net.MVC.PartialViewResult rPVResult = new Ext.Net.MVC.PartialViewResult
            ////{
            ////    ClearContainer = true,
            ////    //RenderMode = RenderMode.AddTo,
            ////    ContainerId = "Body",
            ////    //WrapByScriptTag = false,
            ////    Model = InventoryDirectoriesModel()
            ////};

            ////return rPVResult;
            lstBusDay = (List<BusinessDay>)Session["UserBusDays"];
            if (lstBusDay == null)
            {
                lstBusDay = _busDayRepo.GetOpenedBusinessDay(CurrentUser);
            };
            DateTime BDDateOperation = lstBusDay.FirstOrDefault().BDDateOperation;
            ViewBag.BusnessDayDate = BDDateOperation;
            return View(InventoryDirectoriesModel());
        }

        public ActionResult ReloadInventoryDirectoryListStore()
        {
            this.GetCmp<Store>("StockEntryGridStoreID").Reload();
            return this.Direct();
        }

        [HttpPost]
        public StoreResult GetAllInventoryDirectories()
        {
            return this.Store(InventoryDirectoriesModel());
        }
        [HttpPost]
        public StoreResult GetAllStockDirectory(DateTime InventoryEntryDate)
        {
            return this.Store(InventoryDirectoriesEntryModel(InventoryEntryDate));
        }
        public List<object> InventoryDirectoriesEntryModel(DateTime InventoryEntryDate)
        {
            //un dossier d'inventaire qui est encours ou qui a déjà été fermé ne peut plus être modifiée ou supprimée
            List<InventoryDirectory> dataTmp = inventoryDirectoryRepository.FindAll.Where(id => (id.InventoryDirectoryStatut == InventoryDirectorySatut.Closed)  &&
                                                                                                  (id.RegisteredByID == SessionGlobalPersonID) && id.InventoryDirectoryDate == InventoryEntryDate.Date).ToList();
            List<object> list = new List<object>();

            foreach (InventoryDirectory id in dataTmp)
            {
                list.Add(
                    new
                    {
                        InventoryDirectoryID = id.InventoryDirectoryID,
                        Branch = id.Branch.BranchName,
                        InventoryDirectoryReference = id.InventoryDirectoryReference,
                        InventoryDirectoryCreationDate = id.InventoryDirectoryCreationDate,
                        InventoryDirectoryDescription = id.InventoryDirectoryDescription,

                    }
                   );
            }

            return list;
        }
        public List<object> InventoryDirectoriesModel()
        {
            //un dossier d'inventaire qui est encours ou qui a déjà été fermé ne peut plus être modifiée ou supprimée
            List<InventoryDirectory> dataTmp = inventoryDirectoryRepository.FindAll.Where(id => (id.InventoryDirectoryStatut == InventoryDirectorySatut.Opened) ||
                                                                                                ((id.InventoryDirectoryStatut == InventoryDirectorySatut.InProgess) &&
                                                                                                  (id.RegisteredByID == SessionGlobalPersonID))).ToList();

            List<object> list = new List<object>();

            foreach (InventoryDirectory id in dataTmp)
            {
                list.Add(
                    new
                    {
                        InventoryDirectoryID = id.InventoryDirectoryID,
                        Branch = id.Branch.BranchName,
                        InventoryDirectoryReference = id.InventoryDirectoryReference,
                        InventoryDirectoryCreationDate = id.InventoryDirectoryCreationDate,
                        InventoryDirectoryDescription = id.InventoryDirectoryDescription,

                    }
                   );
            }

            return list;
        }
       

        //From SupplierReturn

        /// <summary>
        /// This method allow to initialize grid panel for to updated one InventoryDirectory line
        /// </summary>
        /// <param name="ID">ID of InventoryDirectoryLine</param>
        /// <returns></returns>
        [HttpPost]
        public DirectResult InitializeIDLineFieldsByIIDL(int InventoryDirectoryLineID)
        {
            
            //we take InventoryDirectory and her InitialInventoryDirectoryLines
            
   
            InventoryDirectoryLine selectedInventoryDirectoryLine = (from selInvDirLine in db.InventoryDirectoryLines
                                                                     where selInvDirLine.InventoryDirectoryLineID == InventoryDirectoryLineID
                                                                     select selInvDirLine).SingleOrDefault();
            ProductLocalization pl = (from pLoc in db.ProductLocalizations
                                      where pLoc.LocalizationID == selectedInventoryDirectoryLine.LocalizationID &&
                                            pLoc.ProductID == selectedInventoryDirectoryLine.ProductID
                                      select pLoc).SingleOrDefault();

            this.GetCmp<NumberField>("InventoryDirectoryLineID").Value = selectedInventoryDirectoryLine.InventoryDirectoryLineID;
            this.GetCmp<TextField>("TMPID").SetValue(0);

            //this.GetCmp<TextField>("LocalizationID").Text = pl.LocalizationLabel;
            //this.GetCmp<TextField>("ProductID").Text = pl.ProductLabel;

            this.GetCmp<TextField>("ProductLabelID").Text = pl.ProductCode;
            this.GetCmp<TextField>("LocalizationLabelID").Text = pl.LocationCode;

            
            this.GetCmp<TextField>("ProductID").SetValue(pl.ProductID);
            this.GetCmp<TextField>("LocalizationID").SetValue(pl.LocalizationID);
            
            this.GetCmp<NumberField>("OldStockQuantity").SetValue(pl.ProductLocalizationStockQuantity);
            this.GetCmp<NumberField>("NewStockQuantity").SetValue(selectedInventoryDirectoryLine.NewStockQuantity);

            this.GetCmp<NumberField>("OldSafetyStockQuantity").SetValue(pl.ProductLocalizationSafetyStockQuantity);
            this.GetCmp<NumberField>("NewSafetyStockQuantity").SetValue(pl.ProductLocalizationSafetyStockQuantity);

            this.GetCmp<NumberField>("StockDifference").SetValue(0);
            this.GetCmp<NumberField>("StockSecurityDifference").SetValue(0);

            this.GetCmp<NumberField>("AveragePurchasePrice").SetValue(selectedInventoryDirectoryLine.AveragePurchasePrice);

            this.ManageCady();

            return this.Direct();
        }

        /// <summary>
        /// Cette méthode est appelée quand un achat est sélectionné et permet de renseigner les champs de formulaire liés à l'achat sélectionné. il s'agit de :
        /// 1-Le formulaire d'achat
        /// 2-le cady d'achat. 
        /// NB : Il reste à l'utilisateur de remplir le cady de retour
        /// </summary>
        /// <param name="ID"> ID de l'achat sélectionné par l'utilisateur</param>
        /// <returns></returns>
        /// 
        public void InitialiaseHearder(int InventoryDirectoryID)
        {
            if (InventoryDirectoryID > 0)
            {
                //we take InventoryDirectory and her InventoryDirectoryLines

                InventoryDirectory selectedInventoryDirectory = (from selInvDir in db.InventoryDirectories
                                                                 where selInvDir.InventoryDirectoryID == InventoryDirectoryID
                                                                 select selInvDir).SingleOrDefault();

                List<InventoryDirectoryLine> InitialInventoryDirectoryLines = (from inInvDirLine in db.InventoryDirectoryLines
                                                                               where inInvDirLine.InventoryDirectoryID == InventoryDirectoryID
                                                                               select inInvDirLine).ToList();

                Session["InitialInventoryDirectoryLines"] = InitialInventoryDirectoryLines;
                Session["FinalInventoryDirectoryLines"] = new List<InventoryDirectoryLine>();

                BusinessDay currentBD = _busDayRepo.GetOpenedBusinessDay(CurrentUser).FirstOrDefault();
                this.GetCmp<DateField>("InventoryDirectoryDate").Value = currentBD.BDDateOperation;
                
                InventoryDirectory inventoryDirectory = new InventoryDirectory();
                inventoryDirectory = db.InventoryDirectories.Find(InventoryDirectoryID);

                this.GetCmp<TextField>("InventoryDirectoryID").Value = inventoryDirectory.InventoryDirectoryID;

                if (inventoryDirectory.BranchID > 0)
                {
                    this.GetCmp<TextField>("BranchID").Value = (inventoryDirectory.BranchID);
                }

                this.GetCmp<DateField>("InventoryDirectoryCreationDate").Value = inventoryDirectory.InventoryDirectoryCreationDate;
                this.GetCmp<TextField>("InventoryDirectoryReference").Value = inventoryDirectory.InventoryDirectoryReference;
                this.GetCmp<TextArea>("InventoryDirectoryDescription").Value = inventoryDirectory.InventoryDirectoryDescription;
                this.GetCmp<TextField>("RegisteredByID").Value = SessionGlobalPersonID;
                //this.ManageCady();
            }
        }
        [HttpPost]
        public DirectResult InitializeFields(int InventoryDirectoryID)
        {
            ResetReturn(0);
            if (InventoryDirectoryID > 0)
            {
                //we take InventoryDirectory and her InventoryDirectoryLines
                 
                InventoryDirectory selectedInventoryDirectory = (from selInvDir in db.InventoryDirectories
                                                                 where selInvDir.InventoryDirectoryID == InventoryDirectoryID
                                                                 select selInvDir).SingleOrDefault();

                List<InventoryDirectoryLine> InitialInventoryDirectoryLines =(from inInvDirLine in db.InventoryDirectoryLines
                                                                                where inInvDirLine.InventoryDirectoryID == InventoryDirectoryID
                                                                                select inInvDirLine).ToList();

                Session["InitialInventoryDirectoryLines"] = InitialInventoryDirectoryLines;
                Session["FinalInventoryDirectoryLines"] = new List<InventoryDirectoryLine>();

                BusinessDay currentBD = _busDayRepo.GetOpenedBusinessDay(CurrentUser).FirstOrDefault();
                this.GetCmp<DateField>("InventoryDirectoryDate").Value = currentBD.BDDateOperation;
                this.GetCmp<Store>("InventoryDirectoryListStore").Reload();

                this.GetCmp<Store>("InitialInventoryDirectoryLineStore").Reload();

                InventoryDirectory inventoryDirectory = new InventoryDirectory();
                inventoryDirectory = db.InventoryDirectories.Find(InventoryDirectoryID);

                this.GetCmp<TextField>("InventoryDirectoryID").Value = inventoryDirectory.InventoryDirectoryID;

                if (inventoryDirectory.BranchID > 0)
                {
                    this.GetCmp<TextField>("BranchID").Value = (inventoryDirectory.BranchID);
                }

                this.GetCmp<DateField>("InventoryDirectoryCreationDate").Value = inventoryDirectory.InventoryDirectoryCreationDate;
                this.GetCmp<TextField>("InventoryDirectoryReference").Value = inventoryDirectory.InventoryDirectoryReference;
                this.GetCmp<TextArea>("InventoryDirectoryDescription").Value = inventoryDirectory.InventoryDirectoryDescription;
                this.GetCmp<TextField>("RegisteredByID").Value = SessionGlobalPersonID;
                //this.GetCmp<DateField>("InventoryDirectoryCreationDate").Value = inventoryDirectory.InventoryDirectoryCreationDate;
                //this.GetCmp<TextField>("Branch").Value = inventoryDirectory.Branch.BranchName;

                inventoryDirectory.InventoryDirectoryStatut = InventoryDirectorySatut.InProgess;
                inventoryDirectory.RegisteredByID = SessionGlobalPersonID;

                inventoryDirectoryRepository.Update(inventoryDirectory, inventoryDirectory.InventoryDirectoryID);

                this.ManageCady();
            }

            this.SimpleReset2();

            return this.Direct();
        }

        private List<object> ModelInitialInventoryDirectoryLines
        {
            get
            {
                List<InventoryDirectoryLine> InventoryDirectoryLines = (List<InventoryDirectoryLine>)Session["InitialInventoryDirectoryLines"];

                List<object> list = new List<object>();
                

                foreach (InventoryDirectoryLine idl in InventoryDirectoryLines)
                {
                    
                  
                    ProductLocalization pl = (from pLoc in db.ProductLocalizations
                                              where pLoc.LocalizationID == idl.LocalizationID &&
                                                    pLoc.ProductID == idl.ProductID
                                              select pLoc).SingleOrDefault();

                    //                                                                                       pl1.ProductID == idl.ProductID);
                    list.Add(
                        new
                        {
                            InventoryDirectoryLineID = idl.InventoryDirectoryLineID,
                            TMPID = idl.TMPID,
                            ProductLabel = pl.Product.GetProductCode(),
                            LocalizationLabel = pl.Localization.LocalizationLabel,
                            OldStockQuantity = pl.ProductLocalizationStockQuantity,
                            OldSafetyStockQuantity = pl.ProductLocalizationSafetyStockQuantity,
                            AveragePurchasePrice=pl.AveragePurchasePrice,
                            ProductID = pl.ProductID,
                            LocalizationID = pl.LocalizationID,
                        }
                        );
                }

                return list;
            }
        }
        [HttpPost]
        public StoreResult InitialInventoryDirectoryLines()
        {
            return this.Store(ModelInitialInventoryDirectoryLines);
        }
        private List<object> ModelFinalInventoryDirectoryLines
        {
            get
            {
                List<object> model = new List<object>();
                List<InventoryDirectoryLine> FinalInventoryDirectoryLines = (List<InventoryDirectoryLine>)Session["FinalInventoryDirectoryLines"];
                if (FinalInventoryDirectoryLines != null && FinalInventoryDirectoryLines.Count > 0)
                {
                    FinalInventoryDirectoryLines.ToList().ForEach(idl =>
                    {
                        InventoryDirectoryLine line = db.InventoryDirectoryLines.Find(idl.InventoryDirectoryLineID);
                        model.Add(
                                new
                                {
                                    TMPID = idl.TMPID,
                                    InventoryDirectoryLineID = idl.InventoryDirectoryLineID,
                                    ProductID = line.ProductID,
                                    ProductLabel = line.Product.GetProductCode(),
                                    LocalizationID = line.LocalizationID,
                                    LocalizationLabel = line.Localization.LocalizationLabel,
                                    OldStockQuantity = idl.OldStockQuantity,
                                    NewStockQuantity = idl.NewStockQuantity,
                                    OldSafetyStockQuantity = idl.OldSafetyStockQuantity,
                                    NewSafetyStockQuantity = idl.NewSafetyStockQuantity,
                                    StockDifference = /*(idl.OldStockQuantity < 0) ? */(idl.NewStockQuantity + idl.OldStockQuantity), //: (idl.NewStockQuantity - idl.OldStockQuantity),
                                    StockSecurityDifference = idl.NewSafetyStockQuantity - idl.OldSafetyStockQuantity,
                                    AveragePurchasePrice = idl.AveragePurchasePrice
                                }
                              );
                    });
                }
                return model;
            }
        }
        [HttpPost]
        public StoreResult FinalInventoryDirectoryLines()
        {
            //we take InventoryDirectory and her InitialInventoryDirectoryLines
            return this.Store(ModelFinalInventoryDirectoryLines);
        }

        
        //This method reset All forms and reloads all gridpanels
        public DirectResult ResetReturn(int? InventoryDirectoryID)
        {

            Session["FinalInventoryDirectoryLines"] = new List<InventoryDirectoryLine>();
            Session["InitialInventoryDirectoryLines"] = new List<InventoryDirectoryLine>();
            this.GetCmp<FormPanel>("IventoryEntryGeneralInformation").Reset(true);
            this.GetCmp<Store>("InventoryDirectoryListStore").Reload();
            SimpleReset2();

            if (InventoryDirectoryID.HasValue && InventoryDirectoryID.Value > 0) {
                InventoryDirectory inventoryDirectory = db.InventoryDirectories.Find(InventoryDirectoryID.Value);
                if (inventoryDirectory.InventoryDirectoryStatut == InventoryDirectorySatut.InProgess)
                {
                    inventoryDirectory.InventoryDirectoryStatut = InventoryDirectorySatut.Opened;
                    inventoryDirectoryRepository.Update(inventoryDirectory, inventoryDirectory.InventoryDirectoryID);
                }
                
            }
            return this.Direct();
        }
     
        [HttpPost]
        public ActionResult AddFinalInventoryDirectoryLine(InventoryDirectoryLine idLine)
        {

            List<InventoryDirectoryLine> FinalInventoryDirectoryLines = (List<InventoryDirectoryLine>)Session["FinalInventoryDirectoryLines"];

            if (idLine.TMPID > 0)
            {

                FinalInventoryDirectoryLines.RemoveAll(idl => idl.TMPID == idLine.TMPID);
            }

            if (FinalInventoryDirectoryLines != null && FinalInventoryDirectoryLines.Count > 0)
            {
                idLine.TMPID = FinalInventoryDirectoryLines.Select(pl => pl.TMPID).Max() + 1;
            }else
            {
                idLine.TMPID = 1;
            }

            FinalInventoryDirectoryLines.Add(idLine);
            Session["FinalInventoryDirectoryLines"] = FinalInventoryDirectoryLines;

            //Il faut le retirer dans l'ancien
            List<InventoryDirectoryLine> InitialInventoryDirectoryLines = (List<InventoryDirectoryLine>)Session["InitialInventoryDirectoryLines"];
            InitialInventoryDirectoryLines.RemoveAll(idl => idl.ProductID == idLine.ProductID && idl.LocalizationID == idLine.LocalizationID);
            Session["InitialInventoryDirectoryLines"] = InitialInventoryDirectoryLines;

            if (InitialInventoryDirectoryLines.Count==0)
            {
                this.GetCmp<Button>("SaveReturn").Disabled = false;
            }
            else
            {
                this.GetCmp<Button>("SaveReturn").Disabled = true;
            }
            this.ManageCady();

            return this.Reset2();

        }

        [HttpPost]
        public ActionResult Reset2()
        {
            SimpleReset2();
            return this.Direct();
        }

        public void SimpleReset2()
        {
            this.GetCmp<FormPanel>("FormAddFinalInventoryDirectoryLine").Reset(true);

            this.GetCmp<Store>("InitialInventoryDirectoryLineStore").Reload();

            this.GetCmp<Store>("FinalInventoryDirectoryLineStore").Reload();

            ManageCady();
        }

        public void SimpleReset3()
        {
            this.GetCmp<FormPanel>("FormAddFinalInventoryDirectoryLine").Reset(true);
            this.GetCmp<Store>("FinalInventoryDirectoryLineStore").Reload();

            ManageCady();
        }
        [HttpPost]
        public ActionResult RemoveSRLine(int TMPID)
        {
            
            List<InventoryDirectoryLine> FinalInventoryDirectoryLines = (List<InventoryDirectoryLine>)Session["FinalInventoryDirectoryLines"];
            InventoryDirectoryLine idLine = FinalInventoryDirectoryLines.SingleOrDefault(fidl => fidl.TMPID == TMPID);
            FinalInventoryDirectoryLines.RemoveAll(idl => idl.ProductID == idLine.ProductID && idl.LocalizationID == idLine.LocalizationID); 
            Session["FinalInventoryDirectoryLines"] = FinalInventoryDirectoryLines;

            //Il faut l'ajouter dans l'ancien
            idLine.TMPID = 0;
            List<InventoryDirectoryLine> InitialInventoryDirectoryLines = (List<InventoryDirectoryLine>)Session["InitialInventoryDirectoryLines"];
            InitialInventoryDirectoryLines.Add(idLine);
            Session["InitialInventoryDirectoryLines"] = InitialInventoryDirectoryLines;

            return this.Reset2();
        }

        [HttpPost]
        public ActionResult UpdateSRLine(int TMPID)
        {
            this.InitializeIDLineFieldsByFIDL(TMPID);

            return this.Direct();
        }
        public void InitializeIDLineFieldsByFIDL(int TMPID)
        {

            this.GetCmp<FormPanel>("FormAddFinalInventoryDirectoryLine").Reset(true);
            this.GetCmp<Store>("FinalInventoryDirectoryLineStore").Reload();

            List<InventoryDirectoryLine> FinalInventoryDirectoryLines = (List<InventoryDirectoryLine>)Session["FinalInventoryDirectoryLines"];

            if (TMPID > 0)
            {
                InventoryDirectoryLine idLine = FinalInventoryDirectoryLines.SingleOrDefault(pl => pl.TMPID == TMPID);
                InventoryDirectoryLine selectedInventoryDirectoryLine = db.InventoryDirectoryLines.Find(idLine.InventoryDirectoryLineID);

                this.GetCmp<NumberField>("InventoryDirectoryLineID").Value = selectedInventoryDirectoryLine.InventoryDirectoryLineID;
                this.GetCmp<TextField>("TMPID").SetValue(idLine.TMPID);

                this.GetCmp<TextField>("LocalizationID").SetValue(selectedInventoryDirectoryLine.LocalizationID);
                this.GetCmp<TextField>("ProductID").SetValue(selectedInventoryDirectoryLine.ProductID);

                this.GetCmp<TextField>("ProductLabelID").Text =db.Products.Find(selectedInventoryDirectoryLine.ProductID).ProductCode;
                this.GetCmp<TextField>("LocalizationLabelID").Text = db.Localizations.Find(selectedInventoryDirectoryLine.LocalizationID).LocalizationCode;

                this.GetCmp<NumberField>("OldStockQuantity").SetValue(idLine.OldStockQuantity);
                this.GetCmp<NumberField>("NewStockQuantity").SetValue(idLine.NewStockQuantity);

                this.GetCmp<NumberField>("OldSafetyStockQuantity").SetValue(idLine.OldSafetyStockQuantity);
                this.GetCmp<NumberField>("NewSafetyStockQuantity").SetValue(idLine.NewSafetyStockQuantity);

                this.GetCmp<NumberField>("StockDifference").SetValue(idLine.OldStockQuantity + idLine.NewStockQuantity);
                this.GetCmp<NumberField>("StockSecurityDifference").SetValue(idLine.StockSecurityDifference);
                this.GetCmp<NumberField>("AveragePurchasePrice").SetValue(idLine.AveragePurchasePrice);

            }

            ManageCady();

        }
        public void ManageCady()
        {
            List<InventoryDirectoryLine> FinalInventoryDirectoryLines = (List<InventoryDirectoryLine>)Session["FinalInventoryDirectoryLines"];

            if (FinalInventoryDirectoryLines != null && FinalInventoryDirectoryLines.Count > 0)
            {
                this.GetCmp<TextField>("IsCadyEmpty").SetValue(0);//faux
                //this.GetCmp<Button>("SaveReturn").Disabled = false;
            }
            if (FinalInventoryDirectoryLines == null || FinalInventoryDirectoryLines.Count == 0)
            {
                this.GetCmp<TextField>("IsCadyEmpty").SetValue(1);//vrai
               // this.GetCmp<Button>("SaveReturn").Disabled = true;

            }
        }

        [HttpPost]
        public ActionResult CloseInventoryDirectory(InventoryDirectory inventoryDirectory)
        {
            try
            {
                inventoryDirectory.InventoryDirectoryLines = null;
                inventoryDirectory.InventoryDirectoryLines = (List<InventoryDirectoryLine>)Session["FinalInventoryDirectoryLines"];
                inventoryDirectory.InventoryDirectoryStatut = InventoryDirectorySatut.Closed;
                InventoryDirectory InventoryDirectory = inventoryDirectoryRepository.CloseInventoryDirectory(inventoryDirectory,SessionGlobalPersonID);
                Session["InventoryDirectoryID"] = InventoryDirectory.InventoryDirectoryID;
                this.SimpleReset2();
                return this.ResetReturn(0);
            }
            catch (System.Exception e) 
            { 
                X.Msg.Alert("Error ", e.Message + " " + e.StackTrace + " " + e.InnerException).Show(); 
                return this.Direct(); 
            }
        
        }

        public DirectResult Edit(int inventoryDirectoryLineID, string propertyName, string oldValue, string newValue, object inventoryDirectoryLine)
        {
           
            //Mise à jour de la liste dans la session
            List<InventoryDirectoryLine> InitialInventoryDirectoryLines = (List<InventoryDirectoryLine>)Session["InitialInventoryDirectoryLines"];
            InventoryDirectoryLine idlToUpdate = InitialInventoryDirectoryLines.SingleOrDefault(idl => idl.InventoryDirectoryLineID == inventoryDirectoryLineID);
            InitialInventoryDirectoryLines.Remove(idlToUpdate);
            //Modification éffective du champ dans la liste située dans la session
            GenericComparer<InventoryDirectoryLine>.SetValue(idlToUpdate, propertyName, newValue);
            InitialInventoryDirectoryLines.Add(idlToUpdate);
            //Mise à jour de la liste située dans la session
            Session["InitialInventoryDirectoryLines"] = InitialInventoryDirectoryLines;

            //Test : Mise à jour du deuxième cady
            Session["FinalInventoryDirectoryLines"] = InitialInventoryDirectoryLines;
            this.GetCmp<Store>("FinalInventoryDirectoryLineStore").Reload();
            

            //On valide de façon visible les modifications
            X.GetCmp<Store>("Store1").GetById(inventoryDirectoryLineID).Commit();

            return this.Direct();
        }
        [HttpGet]
        public ActionResult UpdateInventoryEntry(int InventoryDirectoryID)
        {
            //this.InitializeIDLineFieldsByFIDL(InventoryDirectoryID);
            this.InitialiaseHearder(InventoryDirectoryID);

            List<InventoryDirectoryLine> data = db.InventoryDirectoryLines.Where(ptl => ptl.InventoryDirectoryID == InventoryDirectoryID).ToList();
            List<InventoryDirectoryLine> InventoryDirectoryLines = new List<InventoryDirectoryLine>();
            int i = 0;
            foreach (InventoryDirectoryLine pl in data)
            {
                pl.TMPID = ++i;
                InventoryDirectoryLines.Add(pl);
            }
            Session["InventoryDirectoryID"] = InventoryDirectoryID;
            this.GetCmp<Button>("btnPrint").Disabled = false;
            this.GetCmp<Button>("SaveReturn").Disabled = true;
            Session["FinalInventoryDirectoryLines"] = InventoryDirectoryLines;
            this.SimpleReset3();
            
            return this.Direct();
        }

        private Company Company
        {
            get
            {
                return db.Companies.FirstOrDefault();
            }
        }

        //This method load a method that print a receip of deposit
        public ActionResult PrintInventoryEntry()
        {
            this.GetCmp<Panel>("Pdf").LoadContent(new ComponentLoader
            {
                Url = Url.Action("GenerateReceipt"),
                DisableCaching = false,
                Mode = LoadMode.Frame,
            });

            return this.Direct();
        }

        //This method print a receipt of customer
        public void GenerateReceipt()
        {
            List<object> model = new List<object>();
            ReportDocument rptH = new ReportDocument();
            try
            {
            int InventoryDirectoryID = (int)Session["InventoryDirectoryID"];
            string repName = "";
            bool isValid = false;
            double totalAmount = 0d;
            double totalRemaining = 0d;

           
            string path = "";
            string defaultImgPath = Server.MapPath("~/Content/Images/App/deboy-logo.png");
            byte[] COMPANY_LOGO = System.IO.File.ReadAllBytes(defaultImgPath);

            string DeviseLabel = db.Devises.Where(d => d.DefaultDevise).FirstOrDefault().DeviseLabel;
            InventoryDirectory inventoryDirectory = (from pt in db.InventoryDirectories
                                                 where pt.InventoryDirectoryID == InventoryDirectoryID
                                                 select pt).SingleOrDefault();
            
            var curBranch = db.UserBranches
                        .Where(br => br.UserID == SessionGlobalPersonID)
                        .ToList()
                        .Select(s => new UserBranch
                        {
                            BranchID = s.BranchID,
                            Branch = s.Branch
                        })
                        .AsQueryable()
                        .FirstOrDefault();

            double saleAmount = 0d;
            db.InventoryDirectoryLines.Where(l => l.InventoryDirectoryID == InventoryDirectoryID).ToList().ForEach(c =>
            {
                isValid = true;
                model.Add(
                                new
                                {
                                    AveragePurchasePrice = c.AveragePurchasePrice,
                                    NewStockQuantity = c.NewStockQuantity.Value,
                                    OldStockQuantity =  c.OldStockQuantity,
                                    StockDifference = (c.NewStockQuantity.Value + c.OldStockQuantity),//( (c.OldStockQuantity-c.NewStockQuantity.Value)<0) ? -1*(c.OldStockQuantity-c.NewStockQuantity.Value) : (c.OldStockQuantity-c.NewStockQuantity.Value), //la qte ajouter
                                    ProductLabel = c.Product.ProductLabel,
                                    ProductRef = c.Product.ProductCode,
                                    CompanyName = Company.Name,
                                    CompanyAdress = "Head Quater : " + Company.Adress.Quarter.Town.Region.RegionLabel + " - " + Company.Adress.Quarter.Town.TownLabel,
                                    CompanyTel = "Tel: " + Company.Adress.AdressPhoneNumber,
                                    BranchName = curBranch.Branch.BranchName,
                                    BranchAdress = curBranch.Branch.Adress.Quarter.QuarterLabel + " - " + curBranch.Branch.Adress.Quarter.Town.TownLabel,
                                    BranchTel = "Tel: " + curBranch.Branch.Adress.AdressPhoneNumber,
                                    Ref = inventoryDirectory.InventoryDirectoryReference,
                                    CompanyCNI = "NO CONT : " + Company.CNI,
                                    Operator = CurrentUser.Name + " " + CurrentUser.Description,
                                    InventoryDirectoryDate = inventoryDirectory.InventoryDirectoryDate,
                                    Title = "Inventory Entry Details informations",
                                    DeviseLabel = DeviseLabel,
                                    //AutorizedBy = c.AutorizedBy,
                                    //RegisteredBy = c.RegisteredBy,
                                    //CompanyLogo = db.Files.FirstOrDefault(f => f.GlobalPersonID == Company.GlobalPersonID) != null ? db.Files.FirstOrDefault(f => f.GlobalPersonID == Company.GlobalPersonID).Content : COMPANY_LOGO
                                }
                        );
            }
                    );

            if (isValid)
            {
                path = Server.MapPath("~/Reports/Supply/RptInventoryEntry.rpt");
                repName = "RptInventoryEntry";
                rptH.Load(path);
                rptH.SetDataSource(model);
                bool isDownloadRpt = (bool)Session["isDownloadRpt"];
                rptH.ExportToHttpResponse(ExportFormatType.PortableDocFormat, System.Web.HttpContext.Current.Response, false, repName);
            }
            else
            {
                Response.Write("Nothing Found; No Report name found");
            }
            }
            catch (Exception ex)
            {

                Response.Write("Error Generating report " + ex.Message + " " + ex.StackTrace);
            }
            finally
            {
                rptH.Close();
                rptH.Dispose();
            }
        }
    }



}