using CABOPMANAGEMENT.Controllers;
using CABOPMANAGEMENT.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using FatSod.Security.Entities;
using FatSod.Security.Abstracts;
using FatSod.Supply.Entities;
using System.IO;

using CABOPMANAGEMENT.Filters;
using FatSod.Supply.Abstracts;
using FatSod.Ressources;
using System.Web;

namespace CABOPMANAGEMENT.Areas.CRM.Controllers
{
    [Authorize(Order = 1)]
    [TakeBusinessDay(Order = 2)]
    public class CustomerController : BaseController
    {
        private IPerson _personRepository;
        private IRepository<FatSod.Security.Entities.File> _fileRepository;
        private IAccount _accountRepository;
        private IRepository<Quarter> _quarterRepository;
        private IBusinessDay _busDayRepo;
        private ITransactNumber _transactNumbeRepository;

        List<BusinessDay> bdDay;


        public CustomerController(IRepository<FatSod.Security.Entities.File> fileRepository,
            IPerson personRepository,
            IAccount accountRepository,
            IBusinessDay busDayRepo,
            IRepository<Quarter> quarterRepository,
            ITransactNumber transactNumbeRepository)
        {
            this._personRepository = personRepository;
            this._fileRepository = fileRepository;
            this._accountRepository = accountRepository;
            this._busDayRepo = busDayRepo;
            this._transactNumbeRepository = transactNumbeRepository;
            this._quarterRepository = quarterRepository;
        }
        //Current Controller and current page
        private const string CONTROLLER_NAME = "CRM/Customer";
        private const string VIEW_NAME = "Index";
        // GET: Sale/Customer

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
            ViewBag.BusnessDayDate = bdDay.FirstOrDefault().BDDateOperation.ToString("yyyy-MM-dd");

            return View(ModelRptListOfCustomer(currentDateOp, currentDateOp));
        }

        public ActionResult About()
        {
            return View();
        }

        //This method print inentory of day
        public ActionResult GenerateCustomerList()
        {
            List<object> model = new List<object>();
            //ReportDocument rptH = new ReportDocument();
            BusinessDay buDay = SessionBusinessDay(null);
            Company cmpny = db.Companies.FirstOrDefault();

            var dataTmp = db.Customers
            .Select(s => new
            {
                GlobalPersonID = s.GlobalPersonID,
                //CustomerNumber = s.CustomerNumber,
                CustomerFullName = s.Name,
                AdressPhoneNumber = s.Adress.AdressPhoneNumber,
                AdressFullName = s.Adress.AdressFullName,
                //IsInHouseCustomer = s.IsInHouseCustomer,
                LimitAmount = (s.LimitAmount == 0) ? 0 : s.LimitAmount,
                CNI = s.CNI,
                IsBillCustomer = s.IsBillCustomer ? "YES" : "NO",
                IsInHouseCustomer = s.IsInHouseCustomer ? "YES" : "NO",
            }).ToList();

            string strOperator1 = CurrentUser.Name;

            foreach (var p in dataTmp.OrderBy(c => c.CustomerFullName))
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
                        LibelleCpte = p.CustomerFullName,
                        CodeOperation = p.IsInHouseCustomer.ToString(),
                        LibelleOperation = "",
                        Reference = p.AdressPhoneNumber,
                        Desription = p.AdressFullName,
                        DateOperation = buDay.BDDateOperation,
                        MontantDB = p.LimitAmount,
                        MontantCR = 0
                    }
                    );
            }
            string path = Server.MapPath("~/Reports/Sales/RptCustomerList.rpt");
            //rptH.Load(path);
            //rptH.SetDataSource(model);

            string stBranchName1 = cmpny.Name;
            string strBranchInfo1 = "Tel: " + cmpny.Adress.AdressPhoneNumber + " Fax : " + cmpny.Adress.AdressFax + " Town :" + cmpny.Adress.Quarter.Town.TownLabel;
            string strRepTitle1 = "LIST OF CUSTOMER'S";
            //if (!string.IsNullOrEmpty(stBranchName1)) rptH.SetParameterValue("BranchName", stBranchName1);
            //if (!string.IsNullOrEmpty(strBranchInfo1)) rptH.SetParameterValue("BranchInfo", strBranchInfo1);
            //if (!string.IsNullOrEmpty(strRepTitle1)) rptH.SetParameterValue("RepTitle", strRepTitle1);
            //if (!string.IsNullOrEmpty(strOperator1)) rptH.SetParameterValue("Operator", strOperator1);


            return View(model);
        }
        //Méthode d'ajout d'un client
        public JsonResult Add(Adress adress, Customer customer, int Quarter, int Account, int SexID, HttpPostedFileBase UploadImage, string DateOfBirth, int IsBillCustomer = 0)
        {
            bool status = false;
            string Message = "";
            try
            {
                adress.QuarterID = Quarter;
                customer.Adress = adress;
                customer.SexID = SexID;

                string[] listDOB = DateOfBirth.Split('-');

                int yearDOB = Convert.ToInt32(listDOB[0]);
                int monthDOB = Convert.ToInt32(listDOB[1]);
                int dayDOB = Convert.ToInt32(listDOB[2]);

                customer.DateOfBirth = new DateTime(yearDOB, monthDOB, dayDOB);
                if (customer.DateOfBirth <= new DateTime(1900, 1, 1))
                {
                    status = false;
                    Message = "Error: Wrong Date of Birth Format";
                    return new JsonResult { Data = new { status = status, Message = Message } };
                }

                //if (DateOfBirth==null || DateOfBirth.ToString()=="" || DateOfBirth.ToString() == "01-01-1900" || DateOfBirth.ToString() == "01/01/1900")
                //{
                //    status = false;
                //    Message = "Wrong Custumer Date Of Birth Format";
                //    return new JsonResult { Data = new { status = status, Message = Message } };
                //}
                //customer.DateOfBirth = DateOfBirth;
                customer.IsBillCustomer = Convert.ToBoolean(IsBillCustomer); ;

                customer.IsInHouseCustomer = true;

                //if (IsInHouseCustomer == 0) customer.IsInHouseCustomer = CashCustomer.Cash;
                //if (IsInHouseCustomer == 1) customer.IsCashCustomer = CashCustomer.NonCash;

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
                    customer.Files = new List<FatSod.Security.Entities.File> { photo };
                }
                BusinessDay SBusinessDay = SessionBusinessDay(null);
                if (customer.GlobalPersonID > 0)
                {
                    Customer existingCustomer = db.Customers.SingleOrDefault(c => c.GlobalPersonID == customer.GlobalPersonID);
                    //recuperation du cpte existant
                    customer.AccountID = existingCustomer.AccountID;
                    customer.IsInHouseCustomer = existingCustomer.IsInHouseCustomer;

                    _personRepository.Update2(customer, SessionGlobalPersonID, (SBusinessDay == null) ? DateTime.Now.Date : SBusinessDay.BDDateOperation, (SBusinessDay == null) ? 0 : SBusinessDay.BranchID);
                    statusOperation = customer.Name + " : " + Resources.AlertUpdateAction;
                }
                else
                {

                    customer.Dateregister = (SBusinessDay == null) ? DateTime.Now.Date : SBusinessDay.BDDateOperation;
                    //fabrication du nvo cpte
                    customer.AccountID = _accountRepository.GenerateAccountNumber(Account, customer.CustomerFullName + " " + Resources.UIAccount, false).AccountID;
                    _personRepository.Create2(customer, SessionGlobalPersonID, (SBusinessDay == null) ? DateTime.Now.Date : SBusinessDay.BDDateOperation, (SBusinessDay == null) ? 0 : SBusinessDay.BranchID);
                    statusOperation = customer.Name + " : " + Resources.AlertAddAction;
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



        public JsonResult InitTrnNumber(int? BranchID)
        {
            List<object> _InfoList = new List<object>();
            if (BranchID > 0)
            {
                bdDay = (List<BusinessDay>)Session["UserBusDays"];
                if (bdDay == null)
                {
                    bdDay = _busDayRepo.GetOpenedBusinessDay(CurrentUser);
                }
                BusinessDay businessDay = bdDay.FirstOrDefault(b => b.BranchID == BranchID.Value);

                string trnnum = _transactNumbeRepository.GenerateUniqueCIN(); //.displayTransactNumber("CUST", businessDay);

                int nbrezero = 0;
                nbrezero = 6 - Convert.ToInt32(trnnum).ToString().Length;
                string valzero = "";
                for (int nbre = 1; nbre <= nbrezero; nbre++)
                {
                    valzero = String.Concat(valzero, "0");
                }
                string LastCustomerID = String.Concat(valzero, Convert.ToInt64(trnnum) - 1);

                _InfoList.Add(new
                {
                    CustomerNumber = trnnum,
                    LastCustomerID = LastCustomerID
                });
            }
            return Json(_InfoList, JsonRequestBehavior.AllowGet);
        }

        ////this method return a file photo
        //public ActionResult File(int id)
        //{
        //    var fileToRetrieve = _fileRepository.Find(id);
        //    return File(fileToRetrieve.Content, fileToRetrieve.ContentType);
        //}

        ////Initialize fields to update
        //[HttpPost]
        public JsonResult InitializeFields(int ID)
        {
            List<object> _InfoList = new List<object>();
            Customer customer = _personRepository.FindAll.OfType<Customer>().FirstOrDefault(c => c.GlobalPersonID == ID);
            string cni = "";
            int lencni = customer.CNI.Trim().Length;
            int lencusid = customer.CustomerNumber.ToString().Length;
            double num;
            string valzero = "";
            if (lencni < lencusid)
            {
                if (double.TryParse(customer.CNI.Trim(), out num))
                {
                    for (int nbre = 1; nbre <= lencni; nbre++)
                    {
                        valzero = String.Concat(valzero, "0");
                    }

                    cni = (customer.CNI.Trim() == valzero) ? customer.CustomerNumber.ToString() : customer.CNI.Trim();
                }
                else
                {
                    cni = (customer.CustomerNumber.ToString().Substring(1, lencni - 1) != customer.CNI.Substring(1, lencni - 1)) ? customer.CustomerNumber : customer.CNI;
                }
            }
            else
            {
                if (double.TryParse(customer.CNI.Trim(), out num))
                {
                    for (int nbre = 1; nbre <= lencni; nbre++)
                    {
                        valzero = String.Concat(valzero, "0");
                    }

                    cni = (customer.CNI.Trim() == valzero) ? customer.CustomerNumber.ToString() : customer.CNI.Trim();
                }
                else
                {
                    cni = (customer.CustomerNumber.ToString().Substring(1, lencusid - 1) != customer.CNI.Substring(1, lencusid - 1)) ? customer.CustomerNumber : customer.CNI;
                }
            }
            //recuperation du status du client
            //V_CustomerStatus vcustSate = db.V_CustomerStatus.Where(c => c.CustomerID == ID).FirstOrDefault();
            int CustomerAge = 0;
            if (customer.DateOfBirth.HasValue)
            {
                CustomerAge = (SessionBusinessDay(null) != null) ? SessionBusinessDay(null).BDDateOperation.Year - customer.DateOfBirth.Value.Year : DateTime.Today.Year - customer.DateOfBirth.Value.Year;
            }
            else
            {
                CustomerAge = 0;
            }

            _InfoList.Add(new
            {
                GestionnaireID = customer.GestionnaireID,
                LimitAmount = customer.LimitAmount,
                IsInHouseCustomer = !(customer.IsInHouseCustomer) ? 0 : 1,
                PreferredLanguage = customer.PreferredLanguage,
                Account = customer.Account.CollectifAccountID,
                CustomerNumber = customer.CustomerNumber,
                Quarter = customer.Adress.QuarterID,
                Town = customer.Adress.Quarter.TownID,
                Region = customer.Adress.Quarter.Town.RegionID,
                Country = customer.Adress.Quarter.Town.Region.CountryID,
                PersonType = 1,
                AdressFax = customer.Adress.AdressFax,
                AdressPhoneNumber = customer.Adress.AdressPhoneNumber,
                AdressEmail = customer.Adress.AdressEmail,
                AdressPOBox = customer.Adress.AdressPOBox,
                AdressID = customer.Adress.AdressID,
                SexID = ((customer.Sex.SexCode == "M") ? 1 : 2),
                CompanyCapital = 0,
                CompanyTradeRegister = "",
                CompanySigle = "",
                CompanySlogan = "",
                CNI = cni,
                Description = customer.Description,
                GlobalPersonID = customer.GlobalPersonID,
                Name = customer.Name,
                DateOfBirth = (customer.DateOfBirth.HasValue) ? customer.DateOfBirth.Value.ToString("yyyy-MM-dd") : null,
                Profession = customer.Profession,
                IsBillCustomer = (customer.IsBillCustomer) ? 1 : 0,
                CustomerAge = CustomerAge
                //CustomerStatus=(vcustSate==null) ? "" : vcustSate.CustomerStatus
            });
            return Json(_InfoList, JsonRequestBehavior.AllowGet);
        }
        //Delete Action
        //[HttpPost]
        public JsonResult Delete(int ID)
        {
            bool status = false;
            string Message = "";
            try
            {
                BusinessDay SBusinessDay = SessionBusinessDay(null);
                Person personToDelete = _personRepository.Find(ID);
                _personRepository.Delete(personToDelete, SessionGlobalPersonID, (SBusinessDay == null) ? DateTime.Now.Date : SBusinessDay.BDDateOperation, (SBusinessDay == null) ? 0 : SBusinessDay.BranchID);
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

        /*
        public JsonResult GetPersonType()
        {
            
            List<object> GetPersonTypeList = new List<object>();
               
            GetPersonTypeList.Add(new { ID = 1, Name= Resources.Physical });
            GetPersonTypeList.Add(new { ID = 2, Name = Resources.Moral });
            return Json(GetPersonTypeList, JsonRequestBehavior.AllowGet);
            
        }
        */
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

        public JsonResult populateUsers()
        {
            /*List<object> userList = new List<object>();
            foreach (User user in db.People.OfType<User>().Where(u => u.IsMarketer /*u.ProfileID>2 && u.UserAccessLevel >= 1*///).ToArray().OrderBy(c=>c.Name))
            /*{
                userList.Add(new { Name = user.UserFullName, ID = user.GlobalPersonID });
            }*/

            return Json(LoadComponent.GetMarketters(CurrentBranch.BranchID), JsonRequestBehavior.AllowGet);
        }

        public JsonResult ModelRptListOfCustomer(DateTime BeginDate, DateTime EndDate)
        {
            // db.Database.CommandTimeout = 0;
            try
            {
                var list = new
                {
                    //data = from u in db.viewCustomers.Where(c => c.IsInHouseCustomer).ToList().OrderBy(uc => uc.Name)
                    data = from u in db.viewCustomers.Where(c=> c.IsInHouseCustomer && (c.DateRegister.Value >= BeginDate && c.DateRegister.Value <= EndDate)).ToList().OrderBy(uc => uc.Name)
                           select
                           new
                           {
                               GlobalPersonID = u.GlobalPersonID,
                               Name = u.Name,
                               //Adress = u.Adress,
                               AdressPOBox = (u.AdressPOBox == null) ? "" : u.AdressPOBox,
                               AdressEmail = (u.AdressEmail == null) ? "" : u.AdressEmail,
                               AdressPhoneNumber = (u.AdressPhoneNumber == null) ? "" : u.AdressPhoneNumber,
                               QuarterLabel = (u.QuarterLabel == null) ? "" : u.QuarterLabel,
                               //Sex = u.Sex,
                               Description = (u.Description == null) ? "" : u.Description,
                               CNI = u.CNI,
                               CustomerNumber = u.CustomerNumber,
                               DateRegister = (u.DateRegister.HasValue) ? u.DateRegister.Value.ToString("dd/MM/yyyy") : null,
                               LastCustomerValue = u.LastCustomerValue,
                               CustomerValue = u.CustomerValue,
                               AccountLabel = u.AccountLabel,
                               IsBillCustomer = u.IsBillCustomer ? "YES" : "NO",
                               IsInHouseCustomer = u.IsInHouseCustomer ? "YES" : "NO",
                               //CollectifAccountNumber = u.CollectifAccountNumber
                               // CustomerValue = u.GlobalPersonID % 3 == 0 ? "VIP" : "ECO" //u.CustomerValue == CustomerValue.VIP ? 
                           }
                };
                var jsonResult = Json(list, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = Int32.MaxValue;
                return jsonResult;
            }
            catch (Exception e)
            {
                return Json(new { }, JsonRequestBehavior.AllowGet);
            }

        }

        public JsonResult ModelRptListOfCustomerPerDay()
        {

            db.Database.CommandTimeout = 0;

            bdDay = (List<BusinessDay>)Session["UserBusDays"];
            if (bdDay == null)
            {
                bdDay = _busDayRepo.GetOpenedBusinessDay(CurrentUser);
            }
            BusinessDay businessDay = bdDay.FirstOrDefault();

            var list = new
            {

                data = from u in db.viewCustomers.Where(c => c.IsInHouseCustomer && (c.DateRegister.Value == businessDay.BDDateOperation)).ToList().OrderBy(uc => uc.Name) // LoadComponent.GetCustomersForStore.OrderBy(uc=>uc.Name).ToList()
                       select
                       new
                       {
                           GlobalPersonID = u.GlobalPersonID,
                           Name = u.Name,
                    //Adress = u.Adress,
                    AdressPOBox = (u.AdressPOBox == null) ? "" : u.AdressPOBox,
                           AdressEmail = (u.AdressEmail == null) ? "" : u.AdressEmail,
                           AdressPhoneNumber = (u.AdressPhoneNumber == null) ? "" : u.AdressPhoneNumber,
                           QuarterLabel = (u.QuarterLabel == null) ? "" : u.QuarterLabel,
                    //Sex = u.Sex,
                    Description = (u.Description == null) ? "" : u.Description,
                           CNI = u.CNI,
                           CustomerNumber = u.CustomerNumber,
                           DateRegister = (u.DateRegister.HasValue) ? u.DateRegister.Value.ToString("dd/MM/yyyy") : null,
                           LastCustomerValue = u.LastCustomerValue,
                           CustomerValue = u.CustomerValue,
                           AccountLabel = u.AccountLabel,
                    //IsBillCustomer = u.IsBillCustomer,
                    IsBillCustomer = u.IsBillCustomer ? "YES" : "NO",
                           IsInHouseCustomer = u.IsInHouseCustomer ? "YES" : "NO",
                       }
            };
            var jsonResult = Json(list, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = Int32.MaxValue;
            return jsonResult;

        }

        /// <summary>
        /// Return a Model that use in view
        /// </summary>
        private List<Customer> ModelCustomer
        {
            get
            {
                List<Customer> model = new List<Customer>();
                LoadComponent.GetCustomersForStore.ForEach(u =>
                {
                    model.Add(
                            new Customer
                            {
                                GlobalPersonID = u.GlobalPersonID,
                                Name = u.Name,
                                Adress = u.Adress,
                                //AdressPOBox = u.AdressPOBox,
                                //AdressPhoneNumber = u.AdressPhoneNumber,
                                Sex = u.Sex,
                                Description = u.Description,
                                CNI = u.CNI,
                                Account = u.Account
                                //AccountNumber = u.AccountNumber,
                                //AccountLabel = u.AccountLabel//,
                                //CollectifAccountNumber = u.CollectifAccountNumber
                            }
                        );
                });
                return model;
            }
        }
    }
}