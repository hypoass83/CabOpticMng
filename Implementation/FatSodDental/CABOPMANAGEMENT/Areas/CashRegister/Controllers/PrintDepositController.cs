using FatSod.Security.Abstracts;
using FatSod.Security.Entities;
using FatSod.Supply.Entities;
using CABOPMANAGEMENT.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using FatSod.Supply.Abstracts;
using CABOPMANAGEMENT.Filters;
using FatSod.Ressources;
using FatSod.Report.WrapReports;

namespace CABOPMANAGEMENT.Areas.CashRegister.Controllers
{
    [Authorize(Order = 1)]
    [TakeBusinessDay(Order = 2)]
    public class PrintDepositController : BaseController
    {
        //private const string CONTROLLER_NAME = "CashRegister/PrintDeposit";
        //*********************
        private IBusinessDay _busDayRepo;
        private IDeposit _depositRepository;

        private ICustomerReturn _customerReturnRepository;
        private ITransactNumber _transactNumberRepository;
        private ISale _saleRepository;

        // GET: CashRegister/State
        public PrintDepositController(
                IBusinessDay busDayRepo,
                IDeposit depositRepository,
                ICustomerReturn customerReturnRepository,
                ITransactNumber transactNumberRepository,
                ISale saleRepository
                )
        {
            this._busDayRepo = busDayRepo;
            this._depositRepository = depositRepository;
            this._customerReturnRepository = customerReturnRepository;
            this._transactNumberRepository = transactNumberRepository;
            this._saleRepository = saleRepository;
        }



        [OutputCache(Duration = 3600)]
        public ActionResult Index()
        {
            try
            {
                Session["SaleLines"] = new List<SaleLine>();
                List<BusinessDay> UserBusDays = (List<BusinessDay>)Session["UserBusDays"];
                DateTime BDDateOperation = UserBusDays.FirstOrDefault().BDDateOperation;

                ViewBag.BusnessDayDate = BDDateOperation.ToString("yyyy-MM-dd");
                return View(/*ModelReturnDeposit(BDDateOperation)*/);
            }

            catch (Exception e)
            {
                TempData["Message"] = "Error " + e.Message;
                return this.View();
            }

        }



        /// <summary>
        /// Liste des depots pour la date en question
        /// </summary>
        private List<AllDeposit> ModelReturnDeposit(DateTime DepositDate)
        {


            List<AllDeposit> model = new List<AllDeposit>();
            BusinessDay currentBD = _busDayRepo.GetOpenedBusinessDay(CurrentUser).FirstOrDefault();

            foreach (AllDeposit s in db.AllDeposits.Where(d => d.AllDepositDate == DepositDate.Date && d.BranchID == currentBD.BranchID).ToList())
            {

                model.Add(
                            new AllDeposit
                            {
                                AllDepositID = s.AllDepositID,
                                AllDepositDate = s.AllDepositDate,
                                Amount = s.Amount,
                                Customer = s.Customer,
                                AllDepositReference = s.AllDepositReference,
                                Representant = s.Representant
                            }
                            );
            }
            return model;
        }



        //[HttpPost]
        public JsonResult ReturnAbleDeposit(DateTime DepositDate)
        {
            var list = new
            {
                data = from s in ModelReturnDeposit(DepositDate)
                       select new
                       {
                           AllDepositID = s.AllDepositID,
                           AllDepositDate = s.AllDepositDate.ToString("yyyy-MM-dd"),
                           Amount = s.Amount,
                           CustomerFullName = s.Customer.Name,
                           AllDepositReference = s.AllDepositReference,
                           Representant = s.Representant
                       }
            };
            return Json(list, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// cette methode permet de suprimer un depot qui a ete valider
        /// Supression successive ds les tables
        /// -Deposit
        /// -AllDeposit
        /// -CustomerSlice
        /// -DepositAccountOperations
        /// -AccountOperation
        /// </summary>
        /// <param name="AllDepositID"></param>
        /// <returns></returns>
        //[HttpPost]
        public ActionResult Delete(int AllDepositID)
        {
            bool status = false;
            string Message = "";
            try
            {
                _depositRepository.DeleteDepositEntry(AllDepositID, SessionGlobalPersonID, SessionBusinessDay(null).BDDateOperation);
                Message = Resources.Success;
                status = true;
            }
            catch (Exception e)
            {
                Message = "Error " + e.Message + " " + e.InnerException;
                status = false;
            }
            return new JsonResult { Data = new { status = status, Message = Message } };
        }


        /// <summary>
        /// Cette méthode est appelée quand un depot est sélectionnée 
        /// </summary>
        /// <param name="ID"> ID du depot sélectionnée par l'utilisateur</param>
        /// <returns></returns>
        //[HttpPost]
        public ActionResult RptReceiptDeposit(int AllDepositID)
        {


            //we take sale and her salelines
            AllDeposit selectedDeposit = db.AllDeposits.Find(AllDepositID);

            Session["Receip_DepositID"] = AllDepositID;
            Session["ReceiveAmoung_Tot"] = selectedDeposit.Amount;
            Session["Receipt_CustomerID"] = selectedDeposit.CustomerID;
            Session["DepositReason"] = selectedDeposit.AllDepositReason;

            var model = GenerateReceiptDeposit();

            return View(model);
            //return RedirectToAction("RptReceiptDeposit", "Deposit");


        }

     
        //This method print a receipt of customer
        public RptReceipt GenerateReceiptDeposit()
        {
            RptReceipt model = new RptReceipt();

            try
            {
                int customerID = (Session["Receipt_CustomerID"] == null) ? 0 : (int)Session["Receipt_CustomerID"];
                int DepositID = (Session["Receip_DepositID"] == null) ? 0 : (int)Session["Receip_DepositID"];
                double receiveAmountTot = (Session["ReceiveAmoung_Tot"] == null) ? 0 : (double)Session["ReceiveAmoung_Tot"];
                string DepositReason = (Session["DepositReason"] == null) ? "" : (string)Session["DepositReason"];


                string defaultImgPath = Server.MapPath("~/Content/Images/App/deboy-logo.png");
                byte[] COMPANY_LOGO = System.IO.File.ReadAllBytes(defaultImgPath);


                Devise devise = new Devise();
                Branch curBranch = new Branch();

                Customer customer = new Customer();

                if (customerID > 0)
                {
                    customer = (from c in db.Customers
                                where c.GlobalPersonID == customerID
                                select c).SingleOrDefault();

                    curBranch = db.Branches.Find(db.UserBranches.Where(ub => ub.UserID == SessionGlobalPersonID).FirstOrDefault().BranchID);
                    BusinessDay businessDay = _busDayRepo.GetOpenedBusinessDay(curBranch);
                    devise = db.Devises.FirstOrDefault(d => d.DefaultDevise);

                    Company cmpny = db.Companies.FirstOrDefault();
                    @ViewBag.CompanyLogoID = cmpny.GlobalPersonID;

                    if (DepositID > 0) //depot d'epargne
                    {
                        AllDeposit deposit = db.AllDeposits.Find(DepositID);
                        //curBranch = deposit.PaymentMethod1.Branch;
                        string TitleDeposit = "";
                        string RptTitle = "";
                        if (deposit.PaymentMethod is Till)
                        {
                            TitleDeposit = "Cash Paid In For " + DepositReason;
                            RptTitle = "CASH RECEIPT";
                        }
                        if (deposit.PaymentMethod is Bank)
                        {
                            TitleDeposit = "Bank Paid In For " + DepositReason;
                            RptTitle = "CHECK RECEIPT";
                        }
                        if (deposit.PaymentMethod is SavingAccount)
                        {
                            TitleDeposit = "Saving Paid In For " + DepositReason;
                            RptTitle = "SAVING RECEIPT";
                        }
                        model = new RptReceipt()
                        {
                            ReceiveAmount = receiveAmountTot,
                            TotalAmount = 0, //montant total de la facture
                            LineUnitPrice = 0, //reste du montant a verser
                            CompanyName = cmpny.Name,
                            CompanyAdress = "Head Quater : " + cmpny.Adress.Quarter.Town.Region.RegionLabel + " - " + cmpny.Adress.Quarter.Town.TownLabel,
                            CompanyTel = "Tel: " + cmpny.Adress.AdressPhoneNumber,
                            BranchName = curBranch.BranchName,
                            BranchAdress = curBranch.Adress.Quarter.QuarterLabel + " - " + curBranch.Adress.Quarter.Town.TownLabel,
                            BranchTel = "Tel: " + curBranch.Adress.AdressPhoneNumber + " - Cel: "+curBranch.Adress.AdressCellNumber,
                            Ref = deposit.AllDepositReference,
                            CompanyCNI = "NO CONT : " + cmpny.CNI,
                            Operator = GetOperator(deposit.Operator), // CurrentUser.Name + " " + CurrentUser.Description,
                            CustomerName = deposit.Representant,//customer.Name,// + " " + customer.Description,
                            CustomerAccount = customer.AccountNumber,
                            SaleDate = deposit.AllDepositDate.Date,
                            Title = TitleDeposit,
                            DeviseLabel = RptTitle,
                            CompanyRC = Company.CompanyTradeRegister,
                            CompanyLogo = db.Files.FirstOrDefault(f => f.GlobalPersonID == cmpny.GlobalPersonID) != null ? db.Files.FirstOrDefault(f => f.GlobalPersonID == cmpny.GlobalPersonID).Content : COMPANY_LOGO
                        };

                    }

                }
                else
                {
                    Response.Write("Nothing Found; Click on Deposit before print receipt");
                }
                return model;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private Company Company
        {
            get
            {
                return db.Companies.FirstOrDefault();
            }
        }
        public string GetOperator(User Operator)
        {
            return Operator != null ? (Operator.Name + " " + Operator.Description) :
                                      (CurrentUser.Name + " " + CurrentUser.Description);
        }
    }

}