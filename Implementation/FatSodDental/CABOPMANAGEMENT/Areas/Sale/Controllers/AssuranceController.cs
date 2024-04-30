using CABOPMANAGEMENT.Controllers;
using CABOPMANAGEMENT.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using FatSod.Security.Entities;
using FatSod.Security.Abstracts;
using FatSod.Supply.Entities;
using FatSod.Ressources;
using System.IO;
using CABOPMANAGEMENT.Filters;
using FatSod.Supply.Abstracts;

namespace CABOPMANAGEMENT.Areas.Sale.Controllers
{
    [Authorize(Order = 1)]
    [TakeBusinessDay(Order = 2)]
    public class AssuranceController : BaseController
    {
        private IPerson _personRepository;

        private IBusinessDay _busDayRepo;
        private ITransactNumber _transactNumbeRepository;
        private IAccount _accountRepository;
        List<BusinessDay> bdDay;
        private IRepository<Quarter> _quarterRepository;

        public AssuranceController( 
            IPerson personRepository,
            IBusinessDay busDayRepo,
            IAccount accountRepository,
            IRepository<Quarter> quarterRepository,
            ITransactNumber transactNumbeRepository)
        {
            this._personRepository = personRepository;
            this._busDayRepo = busDayRepo;
            this._transactNumbeRepository = transactNumbeRepository;
            this._accountRepository = accountRepository;
            this._quarterRepository = quarterRepository;
        }
        //Current Controller and current page
        private const string CONTROLLER_NAME = "Sale/Assurance";
        private const string VIEW_NAME = "Index";
        // GET: Sale/Assurance
        
        [OutputCache(Duration = 3600)]
        public ActionResult Index()
        {

            bdDay = (List<BusinessDay>)Session["UserBusDays"];
            if (bdDay == null)
            {
                bdDay = _busDayRepo.GetOpenedBusinessDay(CurrentUser);
            }

            DateTime currentDateOp = bdDay.FirstOrDefault().BDDateOperation;
            ViewBag.CurrentBranch = bdDay.FirstOrDefault().BranchID;
            ViewBag.BusnessDayDate = bdDay.FirstOrDefault().BDDateOperation;

            return View(ModelAssurance);
        }

        //retourne la liste des comptes collectif en fonction d'un code representant le code de l'AccountingSection des comptes que nous voulons récupérer
        public JsonResult CollectifAccounts(String code)
        {
            List<object> colAccountingList = new List<object>();
            List<CollectifAccount> colAccounts = db.CollectifAccounts.Where(a => a.AccountingSection.AccountingSectionCode == code).ToList();
            foreach (CollectifAccount colAccount in colAccounts)
            {
                colAccountingList.Add(new
                {
                    Name = colAccount.CollectifAccountLabel,
                    ID = colAccount.CollectifAccountID
                });
            }
            return Json(colAccountingList, JsonRequestBehavior.AllowGet);
        }

        //This method print inentory of day
        /* public ActionResult GenerateAssuranceList()
         {
             List<object> model = new List<object>();
             ReportDocument rptH = new ReportDocument();
             BusinessDay buDay = SessionBusinessDay(null);
             Company cmpny = db.Companies.FirstOrDefault();

             var dataTmp = db.Assureurs
                         .Select(s => new
                         {
                             GlobalPersonID = s.GlobalPersonID,
                             AssureurFullName = s.Name,
                             AdressPhoneNumber = s.Adress.AdressPhoneNumber,
                             AdressFullName = s.Adress.AdressFullName,
                             CNI = s.CNI
                         }).ToList();

             string strOperator1 = CurrentUser.Name;

             foreach (var p in dataTmp.OrderBy(c => c.AssureurFullName))
             {

                 model.Add(
                     new
                     {
                         RptEtatsJournalID = p.GlobalPersonID,
                         Agence = buDay.Branch.BranchCode,
                         LibAgence = buDay.Branch.BranchDescription,
                         Devise = "",
                         LibDevise = "",
                         CompteCle = p.CNI.ToString(),
                         LibelleCpte = p.AssureurFullName,
                         LibelleOperation = "",
                         Reference = p.AdressPhoneNumber,
                         Desription = p.AdressFullName,
                         DateOperation = buDay.BDDateOperation,
                         MontantDB = 0,
                         MontantCR = 0
                     }
                     );
             }
             string path = Server.MapPath("~/Reports/Sales/RptAssuranceList.rpt");
             rptH.Load(path);
             rptH.SetDataSource(model);

             string stBranchName1 = cmpny.Name;
             string strBranchInfo1 = "Tel: " + cmpny.Adress.AdressPhoneNumber + " Fax : " + cmpny.Adress.AdressFax + " Town :" + cmpny.Adress.Quarter.Town.TownLabel;
             string strRepTitle1 = "LIST OF INSURANCES";
             if (!string.IsNullOrEmpty(stBranchName1)) rptH.SetParameterValue("BranchName", stBranchName1);
             if (!string.IsNullOrEmpty(strBranchInfo1)) rptH.SetParameterValue("BranchInfo", strBranchInfo1);
             if (!string.IsNullOrEmpty(strRepTitle1)) rptH.SetParameterValue("RepTitle", strRepTitle1);
             if (!string.IsNullOrEmpty(strOperator1)) rptH.SetParameterValue("Operator", strOperator1);

             Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
             return File(stream, "application/pdf");
         }*/
        //Méthode d'ajout d'un client
        public JsonResult Add(Adress adress, Assureur assureur, int Account, int? Quarter)
        {
            bool status = false;
            string Message = "";
            try
            {
                adress.QuarterID = (Quarter == null || Quarter <= 0) ? 1 : Quarter.Value;
                assureur.Adress = adress;
                assureur.SexID = 1;
                assureur.CNI = assureur.Name;

                if (assureur.GlobalPersonID > 0)
                {
                    Assureur existingAssureur = db.Assureurs.SingleOrDefault(c => c.GlobalPersonID == assureur.GlobalPersonID);
                    //recuperation du cpte existant
                    assureur.AccountID = existingAssureur.AccountID;
                    _personRepository.Update2(assureur, SessionGlobalPersonID, (SessionBusinessDay(null) == null) ? DateTime.Now.Date : SessionBusinessDay(null).BDDateOperation, (SessionBusinessDay(null) == null) ? 0 : SessionBusinessDay(null).BranchID);
                    statusOperation = assureur.Name + " : " + Resources.AlertUpdateAction;
                }
                else
                {
                    //fabrication du nvo cpte
                    assureur.AccountID = _accountRepository.GenerateAccountNumber(Account, assureur.Name + " " + Resources.UIAccount, false).AccountID;
                
                    _personRepository.Create2Assurance(assureur, SessionGlobalPersonID, (SessionBusinessDay(null) == null) ? DateTime.Now.Date : SessionBusinessDay(null).BDDateOperation, (SessionBusinessDay(null) == null) ? 0 : SessionBusinessDay(null).BranchID);
                    statusOperation = assureur.Name + " : " + Resources.AlertAddAction;
                }

                status = true;
                Message = Resources.Success + "-" + statusOperation;
            }
            catch (Exception e)
            {
                status = false;
                Message = "Error " + e.Message;
            }
            return new JsonResult { Data = new { status = status, Message = Message } };
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
        //Initialize fields to update

        public JsonResult InitializeFields(int ID)
        {
            List<object> _InfoList = new List<object>();
            Assureur assureur = _personRepository.FindAll.OfType<Assureur>().FirstOrDefault(c => c.GlobalPersonID == ID);
            _InfoList.Add(new
            {
                //mise à jour de l'affichage
                Country = assureur.Adress.Quarter.Town.Region.CountryID,
                Region = assureur.Adress.Quarter.Town.RegionID,
                Town = assureur.Adress.Quarter.TownID,
                Quarter = assureur.Adress.QuarterID,
                //Adress 
                AdressID = assureur.Adress.AdressID,
                AdressFax = assureur.Adress.AdressFax,
                AdressPhoneNumber = assureur.Adress.AdressPhoneNumber,
                AdressEmail = assureur.Adress.AdressEmail,
                AdressPOBox = assureur.Adress.AdressPOBox,
                CompteurFacture = assureur.CompteurFacture,
                RemiseDeAssurance = assureur.Remise,
                Account = assureur.Account.CollectifAccountID,
                CompanySigle = assureur.CompanySigle,
                CompanyTradeRegister = assureur.CompanyTradeRegister,
                GlobalPersonID = assureur.GlobalPersonID,
                Name = assureur.Name,
                Matricule = assureur.Matricule
            });
            return Json(_InfoList, JsonRequestBehavior.AllowGet);
        }
        //Delete Action
        
        public JsonResult Delete(int ID)
        {
            bool status = false;
            string Message = "";
            
            try
            {
                Person personToDelete = _personRepository.Find(ID);
                _personRepository.Delete(personToDelete, SessionGlobalPersonID, (SessionBusinessDay(null) == null) ? DateTime.Now.Date : SessionBusinessDay(null).BDDateOperation, (SessionBusinessDay(null) == null) ? 0 : SessionBusinessDay(null).BranchID);
                status = true;
                Message = Resources.Success + "-" + Resources.AlertDeleteAction;
            }
            catch (Exception e)
            {
                status = false;
                Message = "Error " + e.Message;
            }

            return new JsonResult { Data = new { status = status, Message = Message } };
        }
       
       
        /// <summary>
        /// Return a Model that use in view
        /// </summary>
        private List<Assureur> ModelAssurance
        {
            get
            {
                List<Assureur> model = new List<Assureur>();
                LoadComponent.GetAssurancesForStore.ForEach(u =>
                {
                    model.Add(
                            new Assureur
                            {
                                GlobalPersonID = u.GlobalPersonID,
                                Name = u.Name,
                                AdressEmail = u.AdressEmail,
                                AdressPOBox = u.AdressPOBox,
                                AdressPhoneNumber = u.AdressPhoneNumber,
                                SexLabel = u.SexLabel,
                                Description = u.CompanySigle,
                                CNI = u.CNI,
                                Matricule=u.Matricule
                            }
                        );
                });
                return model;
            }
        }
    }
}