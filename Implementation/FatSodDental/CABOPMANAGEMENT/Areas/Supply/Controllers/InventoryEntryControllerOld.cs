using FatSod.DataContext.Initializer;
using FatSod.Security.Abstracts;
using FatSod.Security.Entities;
using FatSod.Supply.Abstracts;
using FatSod.Supply.Entities;
using VALDOZMANAGEMENT.Controllers;
using VALDOZMANAGEMENT.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace VALDOZMANAGEMENT.Areas.Supply.Controllers
{
    [Authorize(Order = 1)]
    [TakeBusinessDay(Order = 2)]
    public class InventoryEntryControllerOld : BaseController
    {
        //Current Controller and current page

        private const string CONTROLLER_NAME = "Supply/" + CodeValue.Supply.InventoryEntry_SM.CONTROLLER;
        private const string VIEW_NAME = CodeValue.Supply.InventoryEntry_SM.PATH;

        private IInventoryDirectory inventoryDirectoryRepository;
        private IBusinessDay _busDayRepo;

        private List<BusinessDay> lstBusDay;
        //Construcitor
        public InventoryEntryControllerOld(
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

            Session["FinalInventoryDirectoryLines"] = new List<InventoryDirectoryLine>();
            Session["InitialInventoryDirectoryLines"] = new List<InventoryDirectoryLine>();

            lstBusDay = (List<BusinessDay>)Session["UserBusDays"];
            if (lstBusDay == null)
            {
                lstBusDay = _busDayRepo.GetOpenedBusinessDay(CurrentUser);
            };
            DateTime BDDateOperation = lstBusDay.FirstOrDefault().BDDateOperation;
            ViewBag.BusnessDayDate = BDDateOperation;
            return View(InventoryDirectoriesModel());
        }
        /*
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
        }*/
        public List<object> InventoryDirectoriesEntryModel(DateTime InventoryEntryDate)
        {
            //un dossier d'inventaire qui est encours ou qui a déjà été fermé ne peut plus être modifiée ou supprimée
            List<InventoryDirectory> dataTmp = inventoryDirectoryRepository.FindAll.Where(id => (id.InventoryDirectoryStatut == InventoryDirectorySatut.Closed) &&
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
        public JsonResult InventoryDirectoriesModel()
        {
            //un dossier d'inventaire qui est encours ou qui a déjà été fermé ne peut plus être modifiée ou supprimée
            List<InventoryDirectory> dataTmp = inventoryDirectoryRepository.FindAll.Where(id => (id.InventoryDirectoryStatut == InventoryDirectorySatut.Opened) ||
                                                                                                ((id.InventoryDirectoryStatut == InventoryDirectorySatut.InProgess) &&
                                                                                                  (id.RegisteredByID == SessionGlobalPersonID))).ToList();

            var model = new
            {
                data = from id in dataTmp
                       select
                       new
                       {
                           InventoryDirectoryID = id.InventoryDirectoryID,
                           Branch = id.Branch.BranchName,
                           InventoryDirectoryReference = id.InventoryDirectoryReference,
                           InventoryDirectoryCreationDate = id.InventoryDirectoryCreationDate.ToString("yyyy-MM-dd"),
                           InventoryDirectoryDescription = id.InventoryDirectoryDescription,

                       }
            };

            return Json(model, JsonRequestBehavior.AllowGet);
        }




        /// <summary>
        /// This method allow to initialize grid panel for to updated one InventoryDirectory line
        /// </summary>
        /// <param name="ID">ID of InventoryDirectoryLine</param>
        /// <returns></returns>
        //[HttpPost]
        public JsonResult InitializeIDLineFieldsByIIDL(int InventoryDirectoryLineID)
        {

            //we take InventoryDirectory and her InitialInventoryDirectoryLines

            List<object> _InfoList = new List<object>();
            if (InventoryDirectoryLineID > 0)
            {
                InventoryDirectoryLine selectedInventoryDirectoryLine = (from selInvDirLine in db.InventoryDirectoryLines
                                                                         where selInvDirLine.InventoryDirectoryLineID == InventoryDirectoryLineID
                                                                         select selInvDirLine).SingleOrDefault();
                ProductLocalization pl = (from pLoc in db.ProductLocalizations
                                          where pLoc.LocalizationID == selectedInventoryDirectoryLine.LocalizationID &&
                                                pLoc.ProductID == selectedInventoryDirectoryLine.ProductID
                                          select pLoc).SingleOrDefault();

                _InfoList.Add(new
                {
                    InventoryDirectoryLineID = selectedInventoryDirectoryLine.InventoryDirectoryLineID,
                    TMPID = 0,
                    ProductLabelID = pl.ProductCode,
                    LocalizationLabelID = pl.LocationCode,
                    ProductID = pl.ProductID,
                    LocalizationID = pl.LocalizationID,
                    OldStockQuantity = pl.ProductLocalizationStockQuantity,
                    NewStockQuantity = selectedInventoryDirectoryLine.NewStockQuantity,

                    OldSafetyStockQuantity = pl.ProductLocalizationSafetyStockQuantity,
                    NewSafetyStockQuantity = pl.ProductLocalizationSafetyStockQuantity,
                    StockDifference = 0,
                    StockSecurityDifference = 0,
                    AveragePurchasePrice = selectedInventoryDirectoryLine.AveragePurchasePrice,
                });
            }
            return Json(_InfoList, JsonRequestBehavior.AllowGet);
        }

        /*
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
        }*/
        //[HttpPost]
        public JsonResult InitializeFields(int InventoryDirectoryID)
        {
            Session["FinalInventoryDirectoryLines"] = new List<InventoryDirectoryLine>();
            Session["InitialInventoryDirectoryLines"] = new List<InventoryDirectoryLine>();
            List<object> _InfoList = new List<object>();
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

                InventoryDirectory inventoryDirectory = new InventoryDirectory();
                inventoryDirectory = db.InventoryDirectories.Find(InventoryDirectoryID);

                _InfoList.Add(new
                {
                    RegisteredByID = SessionGlobalPersonID,
                    InventoryDirectoryStatut = InventoryDirectorySatut.InProgess,
                    InventoryDirectoryDescription = inventoryDirectory.InventoryDirectoryDescription,
                    InventoryDirectoryReference = inventoryDirectory.InventoryDirectoryReference,
                    InventoryDirectoryCreationDate = inventoryDirectory.InventoryDirectoryCreationDate.ToString("yyyy-MM-dd"),
                    BranchID = inventoryDirectory.BranchID,
                    InventoryDirectoryID = inventoryDirectory.InventoryDirectoryID,
                    InventoryDirectoryDate = currentBD.BDDateOperation.ToString("yyyy-MM-dd"),


                });

            }
            return Json(_InfoList, JsonRequestBehavior.AllowGet);

        }



        private List<InventoryDirectoryLine> ModelInitialInventoryDirectoryLines()
        {

            List<InventoryDirectoryLine> InventoryDirectoryLines = (List<InventoryDirectoryLine>)Session["InitialInventoryDirectoryLines"];
            List<InventoryDirectoryLine> list = new List<InventoryDirectoryLine>();

            foreach (InventoryDirectoryLine idl in InventoryDirectoryLines)
            {

                ProductLocalization pl = (from pLoc in db.ProductLocalizations
                                          where pLoc.LocalizationID == idl.LocalizationID &&
                                              pLoc.ProductID == idl.ProductID
                                          select pLoc).SingleOrDefault();

                list.Add(
                    new InventoryDirectoryLine
                    {
                        InventoryDirectoryLineID = idl.InventoryDirectoryLineID,
                        TMPID = idl.TMPID,
                        ProductLabel = pl.Product.GetProductCode(),
                        LocalizationLabel = pl.Localization.LocalizationLabel,
                        OldStockQuantity = pl.ProductLocalizationStockQuantity,
                        OldSafetyStockQuantity = pl.ProductLocalizationSafetyStockQuantity,
                        AveragePurchasePrice = pl.AveragePurchasePrice,
                        ProductID = pl.ProductID,
                        LocalizationID = pl.LocalizationID,
                    }
                );
            }

            return list;
        }
        //chargement des anciennes data after click
        public JsonResult InitialInventoryDirectoryLines()
        {
            var model = new
            {
                data = from id in ModelInitialInventoryDirectoryLines()
                       select
                       new
                       {
                           InventoryDirectoryLineID = id.InventoryDirectoryLineID,
                           TMPID = id.TMPID,
                           ProductLabel = id.ProductLabel,
                           LocalizationLabel = id.LocalizationLabel,
                           OldStockQuantity = id.OldStockQuantity,

                       }
            };

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        private List<InventoryDirectoryLine> ModelFinalInventoryDirectoryLines()
        {
            int StockDifference = 0;
            int StockSecurityDifference = 0;
            List<InventoryDirectoryLine> model = new List<InventoryDirectoryLine>();
            List<InventoryDirectoryLine> FinalInventoryDirectoryLines = (List<InventoryDirectoryLine>)Session["FinalInventoryDirectoryLines"];
            if (FinalInventoryDirectoryLines != null && FinalInventoryDirectoryLines.Count > 0)
            {
                FinalInventoryDirectoryLines.ToList().ForEach(idl =>
                {
                    StockDifference = (idl.NewStockQuantity == null) ? (int)idl.OldStockQuantity : (int)idl.NewStockQuantity.Value + (int)idl.OldStockQuantity;
                    StockSecurityDifference = (idl.NewSafetyStockQuantity == null) ? 0 : (int)idl.NewSafetyStockQuantity.Value - (int)idl.OldSafetyStockQuantity;

                    InventoryDirectoryLine line = db.InventoryDirectoryLines.Where(c => c.ProductID == idl.ProductID && c.LocalizationID == idl.LocalizationID).FirstOrDefault(); //.Find(idl.InventoryDirectoryLineID);
                    model.Add(
                            new InventoryDirectoryLine
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
                                NewSafetyStockQuantity = (idl.NewSafetyStockQuantity == null) ? 0 : idl.NewSafetyStockQuantity,
                                StockDifference = StockDifference,
                                StockSecurityDifference = StockSecurityDifference,
                                AveragePurchasePrice = idl.AveragePurchasePrice
                            }
                            );
                });
            }
            return model;

        }

        public JsonResult FinalInventoryDirectoryLines()
        {
            //we take InventoryDirectory and her InitialInventoryDirectoryLines

            var model = new
            {
                data = from idl in ModelFinalInventoryDirectoryLines()
                       select
                       new
                       {
                           InventoryDirectoryLineID = idl.InventoryDirectoryLineID,
                           ProductID = idl.ProductID,
                           ProductLabel = idl.ProductLabel,
                           LocalizationID = idl.LocalizationID,
                           LocalizationLabel = idl.LocalizationLabel,
                           OldStockQuantity = idl.OldStockQuantity,
                           NewStockQuantity = idl.NewStockQuantity,
                           OldSafetyStockQuantity = idl.OldSafetyStockQuantity,
                           NewSafetyStockQuantity = idl.NewSafetyStockQuantity,
                           StockDifference = idl.StockDifference,
                           StockSecurityDifference = idl.StockSecurityDifference,
                           AveragePurchasePrice = idl.AveragePurchasePrice,
                           TMPID = idl.TMPID
                       }
            };

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public JsonResult populateUsers()
        {

            List<object> userList = new List<object>();
            foreach (User user in db.People.OfType<User>().Where(u => u.IsConnected && u.UserAccessLevel <= 5).ToArray())
            {
                userList.Add(new
                {
                    UserFullName = user.UserFullName,
                    GlobalPersonID = user.GlobalPersonID
                });
            }
            return Json(userList, JsonRequestBehavior.AllowGet);

        }


        /*
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
     */
        // [HttpPost]
        public JsonResult AddFinalInventoryDirectoryLine(InventoryDirectoryLine idLine)
        {

            bool status = false;
            string Message = "";
            try
            {
                List<InventoryDirectoryLine> FinalInventoryDirectoryLines = (List<InventoryDirectoryLine>)Session["FinalInventoryDirectoryLines"];

                if (idLine.TMPID > 0)
                {

                    FinalInventoryDirectoryLines.RemoveAll(idl => idl.TMPID == idLine.TMPID);
                }

                if (FinalInventoryDirectoryLines != null && FinalInventoryDirectoryLines.Count > 0)
                {
                    idLine.TMPID = FinalInventoryDirectoryLines.Select(pl => pl.TMPID).Max() + 1;
                }
                else
                {
                    idLine.TMPID = 1;
                }

                FinalInventoryDirectoryLines.Add(idLine);
                Session["FinalInventoryDirectoryLines"] = FinalInventoryDirectoryLines;

                //Il faut le retirer dans l'ancien
                List<InventoryDirectoryLine> InitialInventoryDirectoryLines = (List<InventoryDirectoryLine>)Session["InitialInventoryDirectoryLines"];
                InitialInventoryDirectoryLines.RemoveAll(idl => idl.ProductID == idLine.ProductID && idl.LocalizationID == idLine.LocalizationID);
                Session["InitialInventoryDirectoryLines"] = InitialInventoryDirectoryLines;
                status = true;
                Message = "Ok";
            }
            catch (Exception e)
            {
                status = false;
                Message = "Error " + e.Message + " " + e.StackTrace + " " + e.InnerException;
            }
            return new JsonResult { Data = new { status = status, Message = Message } };
        }



        //[HttpPost]
        public JsonResult RemoveSRLine(int TMPID)
        {
            bool status = false;
            string Message = "";
            try
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
                status = true;
                Message = "Ok";
            }
            catch (Exception e)
            {
                status = false;
                Message = "Error " + e.Message + " " + e.StackTrace + " " + e.InnerException;
            }
            return new JsonResult { Data = new { status = status, Message = Message } };
        }
        /*
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
        */
        //[HttpPost]
        public JsonResult CloseInventoryDirectory(InventoryDirectory inventoryDirectory)
        {
            bool status = false;
            string Message = "";
            try
            {
                inventoryDirectory.InventoryDirectoryLines = null;
                inventoryDirectory.InventoryDirectoryLines = (List<InventoryDirectoryLine>)Session["FinalInventoryDirectoryLines"];
                inventoryDirectory.InventoryDirectoryStatut = InventoryDirectorySatut.Closed;
                InventoryDirectory InventoryDirectory = inventoryDirectoryRepository.CloseInventoryDirectory(inventoryDirectory, SessionGlobalPersonID);
                Session["InventoryDirectoryID"] = InventoryDirectory.InventoryDirectoryID;

                Session["FinalInventoryDirectoryLines"] = new List<InventoryDirectoryLine>();
                Session["InitialInventoryDirectoryLines"] = new List<InventoryDirectoryLine>();

                status = true;
                Message = "Ok";
            }
            catch (Exception e)
            {
                status = false;
                Message = "Error " + e.Message + " " + e.StackTrace + " " + e.InnerException;
            }
            return new JsonResult { Data = new { status = status, Message = Message } };

        }

        /*
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
        */
        private Company Company
        {
            get
            {
                return db.Companies.FirstOrDefault();
            }
        }

        /*
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
        */
        //This method print a receipt of customer
        public ActionResult GenerateReceipt()
        {
            List<object> model = new List<object>();

            try
            {
                int InventoryDirectoryID = (int)Session["InventoryDirectoryID"];

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


                db.InventoryDirectoryLines.Where(l => l.InventoryDirectoryID == InventoryDirectoryID).ToList().ForEach(c =>
                {

                    model.Add(
                                    new
                                    {
                                        AveragePurchasePrice = c.AveragePurchasePrice,
                                        NewStockQuantity = c.NewStockQuantity.Value,
                                        OldStockQuantity = c.OldStockQuantity,
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
                                    }
                            );
                });
            }
            catch (Exception ex)
            {

                Response.Write("Error Generating report " + ex.Message + " " + ex.StackTrace);
            }
            return View(model);
        }
    }



}