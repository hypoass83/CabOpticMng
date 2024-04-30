using FatSod.DataContext.Initializer;
using FatSod.Security.Abstracts;
using FatSod.Security.Entities;
using FatSod.Supply.Abstracts;
using FatSod.Supply.Entities;
using CABOPMANAGEMENT.Controllers;
using CABOPMANAGEMENT.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace CABOPMANAGEMENT.Areas.Supply.Controllers
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
        [OutputCache(Duration = 3600)]
        public ActionResult Location()
        {
            return View(ModelLocal());
        }


        public JsonResult AddLocalization(Localization location)
        {
            bool status = false;
            string Message = "";
            try
            {
                if (((db.Localizations.FirstOrDefault(l => l.LocalizationCode == location.LocalizationCode || l.LocalizationLabel == location.LocalizationLabel) == null)))
                {

                    localizationRepository.Create(location);

                    Message = "The Location " + location.LocalizationCode + " has been successfully created";
                    status = true;

                }
                else
                {
                    status = false;
                    Message = @"Une Location ayant le code " + location.LocalizationCode + " et / ou le label " + location.LocalizationLabel + " existe déjà!<br/>"
                                            + "veuillez changer de code et / ou le label";

                }
                return new JsonResult { Data = new { status = status, Message = Message } };
            }
            catch (Exception e)
            {
                status = false;
                Message = @"Une erreur s'est produite lors de l'opération, veuillez contactez l'administrateur et/ou essayez à nouveau
                                    <br/>Code : <code>" + e.Message + " car " + e.StackTrace + "</code>";
                return new JsonResult { Data = new { status = status, Message = Message } };
            }
        }





        /// <summary>
        /// cette méthode est appelée lorqu' après avoir clicquer sur l'icone modifier du tableau on modifie le formulaire et clique sur enregistrer
        /// </summary>
        /// <returns></returns>

        public JsonResult UpdateLocalization(Localization location)
        {
            bool status = false;
            string Message = "";
            try
            {

                Localization existingLocalization = db.Localizations.Find(location.LocalizationID);
                List<Localization> localizations = db.Localizations.ToList();
                localizations.Remove(existingLocalization);

                if (((localizations.FirstOrDefault(l => l.LocalizationCode == location.LocalizationCode || l.LocalizationLabel == location.LocalizationLabel) == null)))
                {

                    localizationRepository.Update(location, location.LocalizationID);
                    Message = "The Location " + location.LocalizationCode + " has been successfully updated";
                    status = true;

                }
                else
                {
                    status = false;
                    Message = @"Une location ayant le code " + location.LocalizationCode + " et / ou le label " + location.LocalizationLabel + " existe déjà!<br/>"
                                            + "veuillez changer de code et / ou le label";
                }
                return new JsonResult { Data = new { status = status, Message = Message } };

            }
            catch (Exception e)
            {
                status = false;
                Message = @"Une erreur s'est produite lors de la mise à jour, veuillez contactez l'administrateur et/ou essayez à nouveau
                                    <br/>Code : <code>" + e.Message + "</code>";
                return new JsonResult { Data = new { status = status, Message = Message } };
            }
        }


        public JsonResult DeleteLocation(int LocalizationID)
        {
            bool status = false;
            string Message = "";
            try
            {
                Localization deletedLocalization = localizationRepository.Find(LocalizationID);

                if (deletedLocalization != null)
                {
                    localizationRepository.Delete(deletedLocalization);
                    status = true;
                    statusOperation = "The Localization " + deletedLocalization.LocalizationCode + " has been successfully deleted";
                    Message = "Localization: " + statusOperation;
                }
                else
                {

                    statusOperation = @"Désolé vous ne pouvez pas supprimer la catégorie " + deletedLocalization.LocalizationCode + " parcequ'elle contient déjà des produits"
                                      + "pour supprimer cette localiwation, il faut d'abort supprimer ses produits";
                    Message = "Localization: " + statusOperation;
                    status = false;

                }
                return new JsonResult { Data = new { status = status, Message = Message } };
            }
            catch (Exception e)
            {
                status = false;
                statusOperation = @"L'erreur" + e.Message + "s'est produite lors de la suppression! veuillez contactez l'administrateur et/ou essayez à nouveau";
                Message = "Localization:" + statusOperation;
                return new JsonResult { Data = new { status = status, Message = Message } };
            }
        }

        public JsonResult InitializeLocalizationFields(int ID)
        {
            List<object> list = new List<object>();
            if (ID > 0)
            {
                Localization location = new Localization();
                location = db.Localizations.Find(ID);

                list.Add(new
                {
                    LocalizationID = location.LocalizationID,
                    LocalizationCode = location.LocalizationCode,
                    LocalizationLabel = location.LocalizationLabel,
                    LocalizationDescription = location.LocalizationDescription,
                    BranchID = location.BranchID,

                    QuarterID = location.QuarterID,
                    TownID = location.Quarter.TownID,
                    RegionID = location.Quarter.Town.RegionID,
                    CountryID = location.Quarter.Town.Region.CountryID
                });
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }


        public JsonResult AddManager()
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



        private ActionResult NotAuthorized()
        {
            return View();
        }

        public JsonResult getAllBranches()
        {
            List<object> list = new List<object>();

            List<BusinessDay> lstBusDay = (List<BusinessDay>)Session["UserBusDays"];
            if (lstBusDay == null)
            {
                lstBusDay = _busDayRepo.GetOpenedBusinessDay(CurrentUser);
            };

            foreach (BusinessDay busDay in lstBusDay)
            {
                list.Add(
                    new
                    {
                        BranchID = busDay.BranchID,
                        Name = busDay.Branch.BranchName
                    }
                    );
            }

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult getAllCountries()
        {
            List<object> list = new List<object>();
            List<Country> countries = new List<Country>();

            foreach (Country cr in db.Countries)
            {
                list.Add(
                new
                {
                    CountryID = cr.CountryID,
                    Name = cr.CountryLabel
                });
            }

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult getRegions(int? CountryID)
        {
            List<object> list = new List<object>();
            if (CountryID.HasValue)
            {
                List<Region> regions = db.Regions.Where(r => r.CountryID == CountryID.Value).ToList();

                foreach (Region r in regions)
                {
                    list.Add(
                        new
                        {
                            RegionID = r.RegionID,
                            Name = r.RegionLabel
                        }
                        );
                }
            }

            return Json(list, JsonRequestBehavior.AllowGet);

        }
        public JsonResult getTowns(int? RegionID)
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
                            Name = t.TownLabel
                        }
                        );
                }
            }

            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public JsonResult getQuarters(int? TownID)
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
                            Name = q.QuarterLabel
                        }
                        );
                }
            }

            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public List<Localization> ModelLocal()
        {
            List<Localization> realDataTmp = new List<Localization>();

            foreach (Localization l in db.Localizations.ToList())
            {
                realDataTmp.Add(
                    new Localization
                    {
                        LocalizationID = l.LocalizationID,
                        LocalizationCode = l.LocalizationCode,
                        LocalizationLabel = l.LocalizationLabel,
                        Branch = l.Branch,
                        Quarter = l.Quarter,
                        LocalizationDescription = l.LocalizationDescription,
                    }
                    );
            }

            return realDataTmp;
        }

        public JsonResult GetAllLocalizations()
        {
            var model = new
            {
                data = from l in ModelLocal()
                       select new
                       {
                           LocalizationID = l.LocalizationID,
                           LocalizationCode = l.LocalizationCode,
                           LocalizationLabel = l.LocalizationLabel,
                           BranchName = l.Branch.BranchName,
                           QuarterLabel = l.Quarter.QuarterLabel,
                           LocalizationDescription = l.LocalizationDescription,
                       }
            };
            return Json(model, JsonRequestBehavior.AllowGet);
        }


    }
}