using AutoMapper;
using Ext.Net;
using Ext.Net.MVC;
using FatSod.DataContext.Concrete;
using FatSod.DataContext.Initializer;
using FatSod.Security.Abstracts;
using FatSod.Security.Entities;
using FatSod.Supply.Abstracts;
using FatSod.Supply.Entities;
using FatSodDental.UI.Areas.Supply.ViewModel;
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
    public class LocationController : BaseController
    {
        private IRepositorySupply<Localization> localizationRepository;
        private IRepositorySupply<EmployeeStock> _employeeStockRepository;
        private IRepositorySupply<EmployeeStockHistoric> _employeeStockHistoricRepository;
        private IBusinessDay _busDayRepo;
        

        private const string CONTROLLER_NAME = "Supply/" + CodeValue.Supply.Location_SM.CONTROLLER;
        private const string VIEW_NAME = CodeValue.Supply.Location_SM.PATH;

        public LocationController(IBusinessDay busDayRepo,
                                  IRepositorySupply<Localization> localizationRepository, 
                                  IRepositorySupply<EmployeeStock> employeeStockRepository,
                                  IRepositorySupply<EmployeeStockHistoric> employeeStockHistoricRepository
            )
        {
            this.localizationRepository = localizationRepository;

            this._employeeStockHistoricRepository = employeeStockHistoricRepository;
            this._employeeStockRepository = employeeStockRepository;
            this._busDayRepo = busDayRepo;
            
        }

        /// <summary>
        /// the view of this action method is stronglyTyped with FatSod.Supply.Entities.Category
        /// </summary>
        /// <returns>ActionResult</returns>
        /// 
        //[OutputCache(Duration = 3600)] 
        public ActionResult Location()
        {            
            //Session["Curent_Page"] = VIEW_NAME;
            //Session["Curent_Controller"] = CONTROLLER_NAME;

            //We verify if the current user has right to access view which this action calls
            //if (!LoadAction.isAutoriseToGetSubMenu(SessionProfileID, CodeValue.Supply.Location_SM.CODE, db))
            //{
            //    this.NotAuthorized();
            //}

            //this.GetCmp<Panel>("Body").LoadContent(new ComponentLoader
            //{
            //    Url = Url.Action(VIEW_NAME),
            //    DisableCaching = false,
            //    Mode = LoadMode.Frame
            //});

            //return View();
            //Ext.Net.MVC.PartialViewResult rPVResult = new Ext.Net.MVC.PartialViewResult
            //{
            //    ClearContainer = true,
            //    //RenderMode = RenderMode.AddTo,
            //    ContainerId = "Body",
            //    //WrapByScriptTag = false,
            //    Model = ModelLocal()
            //};

            return View(ModelLocal());
        }

        [HttpPost]
        public ActionResult AddLocalization(Localization location)
        {
            try
            {
                    if (((db.Localizations.FirstOrDefault(l => l.LocalizationCode == location.LocalizationCode || l.LocalizationLabel == location.LocalizationLabel) == null)))
                    {

                        localizationRepository.Create(location);
                        
                        if(location.AssigningToWareHouseMen == true){

                            this.CreateNewWareHouseMen(location);
                        }

                        TempData["status"] = "The Location " + location.LocalizationCode + " has been successfully created";
                        TempData["alertType"] = "alert alert-success";
                        return this.Reset();
                    }
                    else
                    {
                        TempData["alertType"] = "alert alert-danger";
                        TempData["status"] = @"Une Location ayant le code " + location.LocalizationCode + " et / ou le label " + location.LocalizationLabel + " existe déjà!<br/>" 
                                            +"veuillez changer de code et / ou le label";
                        return this.Direct();
                    }                    
               
            }
            catch (Exception e)
            {
                TempData["alertType"] = "alert alert-danger";
                TempData["status"] = @"Une erreur s'est produite lors de l'opération, veuillez contactez l'administrateur et/ou essayez à nouveau
                                    <br/>Code : <code>" + e.Message + " car " + e.StackTrace + "</code>";
                return this.Direct();
            }            
        }

        private void CreateNewWareHouseMen(Localization location)
        {
            //création de EmployeeStock
            List<EmployeeStock> employeeStocks = new List<EmployeeStock>();

            if (location.PrincipalWareHouseManID > 0)
            {
                employeeStocks.Add(new EmployeeStock
                {
                    AssigningDate = location.AssigningDate,
                    //permet de dire qui est magasinier principal
                    IsPrincipalWareHouseMan = true,
                    WareHouseID = location.LocalizationID,
                    WareHouseManID = location.PrincipalWareHouseManID,
                });
                location.WareHouseMen = location.WareHouseMen.Where(n => n != (location.PrincipalWareHouseManID + "") ).ToArray();
            }

            foreach (string id in location.WareHouseMen)
            {
                employeeStocks.Add(new EmployeeStock
                {
                    AssigningDate = location.AssigningDate,
                    WareHouseID = location.LocalizationID,
                    WareHouseManID = Convert.ToInt32(id),
                });
            }
            _employeeStockRepository.CreateAll(employeeStocks);
        }

        /// <summary>
        /// cette méthode est appelée quand on clicque sur l'icone modifier du tableau
        /// </summary>
        /// <param name="localizationID"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult UpdateLocalization(int LocalizationID)
        {
            this.InitializeLocalizationFields(LocalizationID);
            return this.Direct();
        }

        /// <summary>
        /// cette méthode est appelée lorqu' après avoir clicquer sur l'icone modifier du tableau on modifie le formulaire et clique sur enregistrer
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UpdateLocalization(Localization location)
        {
            try
            {

                    Localization existingLocalization = db.Localizations.Find(location.LocalizationID);
                    List<Localization> localizations = db.Localizations.ToList();
                    localizations.Remove(existingLocalization);
                    
                    if (((localizations.FirstOrDefault(l => l.LocalizationCode == location.LocalizationCode || l.LocalizationLabel == location.LocalizationLabel) == null)))
                    {
                        if (_employeeStockRepository.FindAll.Where(es => es.WareHouseID == existingLocalization.LocalizationID) != null && _employeeStockRepository.FindAll.Where(es => es.WareHouseID == existingLocalization.LocalizationID).Count() > 0)
                        {
                            existingLocalization.AssigningToWareHouseMen = true;
                        }

                        localizationRepository.Update(location, location.LocalizationID);

                        if (existingLocalization.AssigningToWareHouseMen == true)//ce magasin avait déjà des magasinier
                        {

                            if (location.AssigningToWareHouseMen == false || location.WareHouseMen.Length == 0)//ce magasin n'a plus de magasinier
                            {
                                //historization du magasinier principal avant la suppression
                                this.HistorizeEmployeeStock(location);
                                //suppression des anciens
                                this.DeleteOldEmployeeStock(existingLocalization);
                            }

                            if (location.AssigningToWareHouseMen == true && location.WareHouseMen.Length > 0)//ce magasin a des magasinier
                            {
                                EmployeeStock actualWareHouseMan = _employeeStockRepository.FindAll.SingleOrDefault(es => es.WareHouseID == location.LocalizationID && es.IsPrincipalWareHouseMan == true);

                                //on doit donc supprimer les anciens sauf le principal et créer les nouveaux
                                //doit on supprimer le principal?
                                if ( ( actualWareHouseMan != null && actualWareHouseMan.WareHouseManID > 0 ) && (location.PrincipalWareHouseManID != actualWareHouseMan.WareHouseManID))//oui
                                {
                                    //historization du magasinier principal avant la suppression
                                    this.HistorizeEmployeeStock(location);
                                    //suppression des anciens
                                    this.DeleteOldEmployeeStock(existingLocalization);
                                }

                                if ( ( actualWareHouseMan != null && actualWareHouseMan.WareHouseManID > 0 ) && location.PrincipalWareHouseManID == actualWareHouseMan.WareHouseManID)//non
                                {
                                    //Suppression des anciens
                                    List<EmployeeStock> toDelete = _employeeStockRepository.FindAll.Where(es => es.WareHouseID == existingLocalization.LocalizationID && es.EmployeeStockID != actualWareHouseMan.EmployeeStockID).ToList();
                                    _employeeStockRepository.DeleteAll(toDelete);
                                    location.WareHouseMen = location.WareHouseMen.Where(wh => Convert.ToInt32(wh) != actualWareHouseMan.WareHouseManID).ToArray();

                                    //pour éviter que le magasinier principal soit encore créé
                                    location.PrincipalWareHouseManID = -100;
                                }
                                //création des nouveaux
                                this.CreateNewWareHouseMen(location);
                            }
                        }

                        if (existingLocalization.AssigningToWareHouseMen == false && location.AssigningToWareHouseMen == true)//ce magasin qui n'avait aucun magasinier a mainteant des magasinier
                        {
                            //création des nouveaux
                            this.CreateNewWareHouseMen(location);
                        }
                        TempData["status"] = "The Location " + location.LocalizationCode + " has been successfully updated";
                        TempData["alertType"] = "alert alert-success";
                        return this.Reset();
                    }
                    else
                    {
                        TempData["alertType"] = "alert alert-danger";
                        TempData["status"] = @"Une location ayant le code " + location.LocalizationCode + " et / ou le label " + location.LocalizationLabel + " existe déjà!<br/>"
                                            + "veuillez changer de code et / ou le label";
                        return this.Direct();
                    }
                
            }
            catch (Exception e)
            {
                TempData["alertType"] = "alert alert-danger";
                TempData["status"] = @"Une erreur s'est produite lors de la mise à jour, veuillez contactez l'administrateur et/ou essayez à nouveau
                                    <br/>Code : <code>" + e.Message + "</code>";
                return this.Direct();
            }
        }

        private void HistorizeEmployeeStock(Localization location)
        {
            //Historisation
            //1-Récupération du magasinier principal
            EmployeeStock toHistoric = _employeeStockRepository.FindAll.SingleOrDefault(es => es.WareHouseID == location.LocalizationID && es.IsPrincipalWareHouseMan == true);
            if (toHistoric != null && toHistoric.EmployeeStockID > 0)
            {
                //2-historization
                EmployeeStockHistoric employeeStockHistoric = new EmployeeStockHistoric
                {
                    AssigningDate = toHistoric.AssigningDate,
                    RemovingDate = location.AssigningDate,
                    WareHouseID = toHistoric.WareHouseID,
                    WareHouseManID = toHistoric.WareHouseManID
                };
                _employeeStockHistoricRepository.Create(employeeStockHistoric);
            }
        }

        //cette méthode supprime tous les 
        private void DeleteOldEmployeeStock(String[] WareHouseMen)
        {
            //on doit donc supprimer les anciens
            foreach (string id in WareHouseMen)
            {
                _employeeStockRepository.Delete(Convert.ToInt32(id));
            }
           
        }

        //cette méthode supprime tous les 
        private void DeleteOldEmployeeStock(Localization existingLocalization)
        {
            List<EmployeeStock> toDelete = _employeeStockRepository.FindAll.Where(es => es.WareHouseID == existingLocalization.LocalizationID).ToList();
            _employeeStockRepository.DeleteAll(toDelete);
        }

        [HttpPost]
        public ActionResult DeleteLocation(int LocalizationID)
        {
            try
            {
                Localization deletedLocalization = localizationRepository.Find(LocalizationID);

                if (true)
                {
                    localizationRepository.Delete(deletedLocalization);
                    statusOperation = "The Localization " + deletedLocalization.LocalizationCode + " has been successfully deleted";
                    X.Msg.Alert("Localization", statusOperation).Show();
                    return this.Reset();
                }
                else
                {
                    statusOperation = @"Désolé vous ne pouvez pas supprimer la catégorie " + deletedLocalization.LocalizationCode + " parcequ'elle contient déjà des produits"
                                      + "pour supprimer cette localiwation, il faut d'abort supprimer ses produits";
                    X.Msg.Alert("Localization", statusOperation).Show();
                    return this.Direct();
                }
            }
            catch (Exception e)
            {
                statusOperation = @"L'erreur" + e.Message + "s'est produite lors de la suppression! veuillez contactez l'administrateur et/ou essayez à nouveau";
                X.Msg.Alert("Localization", statusOperation).Show();
                return this.Direct();
            }            
        }

        public void InitializeLocalizationFields(int ID)
        {
            this.GetCmp<FormPanel>("LocalizationForm").Reset(true);
            this.GetCmp<Store>("LocationListStore").Reload();

            if (ID > 0)
            {
                Localization location = new Localization();
                location = db.Localizations.Find(ID);

                this.GetCmp<TextField>("LocalizationID").SetValue(location.LocalizationID);
                this.GetCmp<TextField>("LocalizationCode").SetValue(location.LocalizationCode);
                this.GetCmp<TextField>("LocalizationLabel").SetValue(location.LocalizationLabel);
                this.GetCmp<TextField>("LocalizationDescription").SetValue(location.LocalizationDescription);

                this.GetCmp<ComboBox>("BranchID").SetValue(location.BranchID);

                //affichage des combo box
                
                
               

                //mise à jour de l'affichage
                this.GetCmp<ComboBox>("CountryID").SetValueAndFireSelect(location.Quarter.Town.Region.CountryID);

                this.GetCmp<ComboBox>("RegionID").Disabled = false;
                this.GetCmp<ComboBox>("RegionID").SetValueAndFireSelect(location.Quarter.Town.RegionID);

                this.GetCmp<ComboBox>("TownID").Disabled = false;
                this.GetCmp<ComboBox>("TownID").SetValueAndFireSelect(location.Quarter.TownID);

                this.GetCmp<ComboBox>("QuarterID").Disabled = false;
                this.GetCmp<ComboBox>("QuarterID").SetValue(location.QuarterID);

                if (_employeeStockRepository.FindAll.Where(es => es.WareHouseID == location.LocalizationID) != null && _employeeStockRepository.FindAll.Where(es => es.WareHouseID == location.LocalizationID).Count() > 0)
                {
                    location.AssigningToWareHouseMen = true;
                }

                //gestion du magasinier
                if (location.AssigningToWareHouseMen == true)
                {
                    this.GetCmp<Checkbox>("AssigningToWareHouseMen").SetValue(location.AssigningToWareHouseMen);
 
                    this.GetCmp<DateField>("AssigningDate").SetValue(location.AssigningDate);

                    EmployeeStock actualWareHouseMan = _employeeStockRepository.FindAll.SingleOrDefault(es => es.WareHouseID == location.LocalizationID && es.IsPrincipalWareHouseMan == true);

                    if (actualWareHouseMan != null && actualWareHouseMan.WareHouseManID > 0)
                    {
                        this.GetCmp<ComboBox>("PrincipalWareHouseManID").SetValue(actualWareHouseMan.WareHouseManID);
                    }
                    
                    foreach (EmployeeStock empStock in _employeeStockRepository.FindAll.Where(es => es.WareHouseID == location.LocalizationID).ToList())
                    {
                        this.GetCmp<MultiCombo>("WareHouseMen").SelectItem("" + empStock.WareHouseManID);
                    }
                }
                
            }
        }

        [HttpPost]
        public ActionResult AddManager()
        {
            Localization location = new Localization();
            TryUpdateModel(location);

            //en cas de mise à jour
            if (location.LocalizationID > 0)
            {
                return this.UpdateLocalization(location);
            }
            else
            {
                return this.AddLocalization(location);
            }
        }


        public ActionResult Reset()
        {
            this.GetCmp<FormPanel>("LocalizationForm").Reset(true);
            this.GetCmp<Store>("LocationListStore").Reload();
            return this.Direct();
        }

        private ActionResult NotAuthorized()
        {
            return View();
        }

        public ActionResult Regions(int? CountryID)
        {
            List<object> list = new List<object>();
            if (CountryID.HasValue)
            {
                List<FatSod.Security.Entities.Region> regions = db.Regions.Where(r => r.CountryID == CountryID.Value).ToList();

                foreach (FatSod.Security.Entities.Region r in regions)
                {
                    list.Add(
                        new
                        {
                            RegionID = r.RegionID,
                            RegionLabel = r.RegionLabel
                        }
                        );
                }
            }

            return this.Store(list);

        }
        public ActionResult Towns(int? RegionID)
        {
            List<object> list = new List<object>();
            if (RegionID.HasValue)
            {
                List<Town> towns = db.Towns.Where(t => t.RegionID == RegionID.Value).ToList();

                foreach (Town t in towns)
                {
                    list.Add(
                        new
                        {
                            TownID = t.TownID,
                            TownLabel = t.TownLabel
                        }
                        );
                }
            }

            return this.Store(list);
        }
        public ActionResult Quarters(int? TownID)
        {
            List<object> list = new List<object>();
            if (TownID.HasValue)
            {

                foreach (Quarter q in db.Quarters.Where(q => q.TownID == TownID.Value).ToList())
                {
                    list.Add(
                        new
                        {
                            QuarterID = q.QuarterID,
                            QuarterLabel = q.QuarterLabel
                        }
                        );
                }
            }

            return this.Store(list);
        }
        public List<object> ModelLocal()
        {
            List<object> realDataTmp = new List<object>();

            foreach (Localization l in db.Localizations.ToList())
            {
                EmployeeStock principalWareHouseMan = _employeeStockRepository.FindAll.SingleOrDefault(es => es.WareHouseID == l.LocalizationID && es.IsPrincipalWareHouseMan == true);
                realDataTmp.Add(
                    new
                    {
                        LocalizationID = l.LocalizationID,
                        LocalizationCode = l.LocalizationCode,
                        LocalizationLabel = l.LocalizationLabel,
                        BranchName = l.Branch.BranchName,
                        QuarterLabel = l.Quarter.QuarterLabel,
                        LocalizationDescription = l.LocalizationDescription,
                        PrincipalWareHouseMan = (principalWareHouseMan != null && principalWareHouseMan.EmployeeStockID > 0) ? (principalWareHouseMan.WareHouseMan.Name + " " + principalWareHouseMan.WareHouseMan.Description) : ""
                    }
                    );
            }

            return realDataTmp;
        }
        [HttpPost]
        public StoreResult GetAllLocalizations()
        {
            return this.Store(ModelLocal());
        }


    }
}