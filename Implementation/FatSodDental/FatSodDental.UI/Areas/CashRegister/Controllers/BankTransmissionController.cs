using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using Ext.Net;
using Ext.Net.MVC;
using FastSod.Utilities.Util;
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
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace FatSodDental.UI.Areas.CashRegister.Controllers
{
    [Authorize(Order = 1)]
    [TakeBusinessDay(Order = 2)]
    public class BankTransmissionController : BaseController
    {
        //Current Controller and current page
        private const string CONTROLLER_NAME = "CashRegister/BankReception";
        private const string VIEW_NAME = "Index";

        private List<BusinessDay> lstBusDay;

		private ITillDay _tillDayRepository;
		private IBusinessDay _busDayRepo;
		private ITreasuryOperation _TreasuryOperation;
        

        public BankTransmissionController(
				 ITillDay tillDayRepository,
				 IBusinessDay busDayRepo,
				ITreasuryOperation TreasuryOperation
				)
		{
			this._tillDayRepository = tillDayRepository;
			this._busDayRepo = busDayRepo;
			this._TreasuryOperation = TreasuryOperation;
            
		}
        // GET: CashRegister/BankTransmission
        [OutputCache(Duration = 3600)] 
        public ActionResult Index()
        {
            try
            {
                List<object> model = new List<object>();
                Session["TreasuryOperationID"]=0;
                return View(model);
            }

            catch (Exception e)
            {
                X.Msg.Alert("Error ", e.Message).Show();
                return this.Direct();
            }
        }
        public ActionResult ChangeBusDay(int? BranchID)
        {
            if (BranchID != null && BranchID.HasValue && BranchID.Value > 0)
            {
                lstBusDay = (List<BusinessDay>)Session["UserBusDays"];
                if (lstBusDay == null)
                {
                    lstBusDay = _busDayRepo.GetOpenedBusinessDay(CurrentUser);
                };
                BusinessDay businessDay = lstBusDay.FirstOrDefault(l => l.BranchID == BranchID.Value);

                Session["businessday"] = businessDay.BDDateOperation;
                this.GetCmp<DateField>("OperationDate").Value = businessDay.BDDateOperation;
            }
            return this.Direct();
        }
        public ActionResult LoadBankTransmissionInfo(int TillID, int DeviseID)
        {
            try
            {
                
                //vefifion si le user est un caissier
                UserTill userTill = (from td in db.UserTills where td.TillID == TillID && td.HasAccess
                                         select td).SingleOrDefault();
                  
                //if userTill then he not has access for cash register
                if (userTill == null)
                {
                    X.Msg.Alert("Access denied", "Sorry Please choose a Teller").Show();
                    return this.Direct();
                }
                DateTime bsDay = (DateTime)Session["businessday"];
                if (bsDay==null )
                {
                    X.Msg.Alert("Access denied", "Please choose branch before proceed").Show();
                    return this.Direct();
                }
                //verifions si la caisse est ouverte
                TillDay currentTillDay = (from t in db.TillDays
                                          where (t.TillID == userTill.Till.ID &&
                                              t.TillDayDate == bsDay.Date &&
                                              t.IsOpen)
                                          select t).SingleOrDefault();
                if (currentTillDay == null)
                {
                    X.Msg.Alert("Access denied", "Open this Teller before Transfer Amount to bank").Show();
                    return this.Direct();
                }
                //bool  openTeller = userTill.
                
                double tillBalance = _tillDayRepository.TillStatus(userTill.Till).Ballance;
                this.GetCmp<NumberField>("ComputerPrice").Value = tillBalance;

                return this.Direct();
            }
            catch (Exception e)
            {
                X.Msg.Alert("Error", "An error occure when we try to give response : " + e.Message).Show();
                return this.Direct();
            }

        }
        //Return list of BankTransmissionList
        public StoreResult GetBankTransmissionList()
        {
            List<object> model = new List<object>();
            
            
            List<TreasuryOperation> lstTreasuryOperation = (from to in db.TreasuryOperations
                                        where to.OperationType==CodeValue.Accounting.TreasuryOperation.TransfertToBank
                                        select to)
                                        .OrderByDescending(t => t.OperationDate)
                                        .ToList();
            lstTreasuryOperation.ForEach(td =>
            {
                model.Add(
                        new
                        {
                            TreasuryOperationID = td.TreasuryOperationID,
                            Justification = td.Justification,
                            OperationAmount = td.OperationAmount,
                            OperationDate = td.OperationDate,
                            OperationRef = td.OperationRef
                        }
                    );
            });
            return this.Store(model);
        }
        //Initialize field for update
        public ActionResult IniatializeFieldAccount(string ID)
        {
           
            int id = Convert.ToInt32(ID);
            TreasuryOperation treasuryOperationToUpdate = (from tr in db.TreasuryOperations where tr.TreasuryOperationID==id
                                                               select tr).SingleOrDefault();
            this.GetCmp<FormPanel>("BankTransmission").Reset(true);
            this.GetCmp<ComboBox>("BranchID").Value = treasuryOperationToUpdate.BranchID;
            this.GetCmp<ComboBox>("TillID").SetValue(treasuryOperationToUpdate.TillID);
            this.GetCmp<ComboBox>("DeviseID").Value = treasuryOperationToUpdate.DeviseID;
            this.GetCmp<DateField>("OperationDate").Value = treasuryOperationToUpdate.OperationDate;

            this.GetCmp<NumberField>("ComputerPrice").Value = treasuryOperationToUpdate.ComputerPrice;
            this.GetCmp<NumberField>("OperationAmount").Value = treasuryOperationToUpdate.OperationAmount;
            this.GetCmp<TextField>("OperationRef").Value = treasuryOperationToUpdate.OperationRef;

            this.GetCmp<TextArea>("Justification").Value = treasuryOperationToUpdate.Justification;
            this.GetCmp<ComboBox>("BankID").Value = treasuryOperationToUpdate.BankID;

            if (treasuryOperationToUpdate==null || treasuryOperationToUpdate.TreasuryOperationID == 0) this.GetCmp<Button>("btnPrint").Disabled = true;
            else this.GetCmp<Button>("btnPrint").Disabled = false;

            Session["TreasuryOperationID"] = treasuryOperationToUpdate.TreasuryOperationID;
            
            return this.Direct();
        }
        public ActionResult AddBankTransmission(TreasuryOperation treasuryOperation)
        {
            try
            {
                if (treasuryOperation.OperationAmount>=treasuryOperation.ComputerPrice)
                {
                    X.Msg.Alert("Bank Transmission", Resources.alertTreasuryAmt).Show();
                    return this.Direct();
                }
                treasuryOperation.OperationType = CodeValue.Accounting.TreasuryOperation.TransfertToBank;
                int TreasuryOperationID = (int)Session["TreasuryOperationID"];
                if (TreasuryOperationID > 0)
                {
                    X.Msg.Alert("Bank Transmission", "Modification Impossible!!! Contacter votre Administrateur").Show();
                    return this.Direct();
                }
                bool res = _TreasuryOperation.SaveTreasuryOperation(treasuryOperation, SessionGlobalPersonID, (SessionBusinessDay(null) == null) ? DateTime.Now.Date : SessionBusinessDay(null).BDDateOperation, (SessionBusinessDay(null) == null) ? 0 : SessionBusinessDay(null).BranchID);
                if (res)
                {
                    Session["TreasuryOperationID"] = treasuryOperation.TreasuryOperationID;
                    this.GetCmp<Button>("btnPrint").Disabled = false;
                    statusOperation = treasuryOperation.Justification + "-" + Resources.AlertUpdateSuccess;
                    this.AlertSucces(Resources.Success, statusOperation);
                }
                else
                {
                    statusOperation = treasuryOperation.Justification + "-" + Resources.AlertError;
                    this.AlertError(Resources.er_alert_danger, statusOperation);
                }
                this.GetCmp<FormPanel>("BankTransmission").Reset();
                this.GetCmp<Store>("Store").Reload();
                Session["TreasuryOperationID"] = 0;
                return this.Direct();
            }
            catch (Exception e)
            {
                statusOperation = Resources.er_alert_danger + e.Message;
                X.Msg.Alert(Resources.TreasuryOperation, statusOperation).Show();
                return this.Direct();
            }
        }
        [HttpPost]
        public ActionResult Reset()
        {
            this.GetCmp<FormPanel>("BankTransmission").Reset();
            this.GetCmp<Store>("Store").Reload();
            Session["TreasuryOperationID"] = 0;
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
        public ActionResult PrintReceipt()
        {
            this.GetCmp<Panel>("PanelReport").LoadContent(new ComponentLoader
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
            int TreasuryOperationID = (int)Session["TreasuryOperationID"];
            
            string repName = "";
            bool isValid = false;
            double totalAmount = 0d;
            double totalRemaining = 0d;

            string path = "";
            string defaultImgPath = Server.MapPath("~/Content/Images/App/deboy-logo.png");
            byte[] COMPANY_LOGO = System.IO.File.ReadAllBytes(defaultImgPath);

            //string DeviseLabel = db.Devises.Where(d => d.DefaultDevise).FirstOrDefault().DeviseLabel;
            TreasuryOperation treasuryOperation = (from to in db.TreasuryOperations
                                                 where to.TreasuryOperationID == TreasuryOperationID
                                                 select to).SingleOrDefault();

            var curBranch = treasuryOperation.Branch;
            if (treasuryOperation !=null)
            {
                isValid = true;
                model.Add(
                                new
                                {
                                    ReceiveAmount = treasuryOperation.OperationAmount,
                                    TotalAmount = 0, //montant total de la facture
                                    LineUnitPrice = 0, //reste du montant a verser
                                    CompanyName = Company.Name,
                                    CompanyAdress = "Head Quater : " + Company.Adress.Quarter.Town.Region.RegionLabel + " - " + Company.Adress.Quarter.Town.TownLabel,
                                    CompanyTel = "Tel: " + Company.Adress.AdressPhoneNumber,
                                    BranchName = curBranch.BranchName,
                                    BranchAdress = curBranch.Adress.Quarter.QuarterLabel + " - " + curBranch.Adress.Quarter.Town.TownLabel,
                                    BranchTel = "Tel: " + curBranch.Adress.AdressPhoneNumber,
                                    Ref = treasuryOperation.OperationRef,
                                    CompanyCNI = "NO CONT : " + Company.CNI,
                                    Operator = CurrentUser.Name + " " + CurrentUser.Description,
                                    CustomerName = treasuryOperation.Bank.Name, //bank receptrice
                                    CustomerAccount = treasuryOperation.Bank.Account.AccountNumber.ToString(),
                                    SaleDate = treasuryOperation.OperationDate,
                                    Title = "Bank Transfer",
                                    DeviseLabel = treasuryOperation.Devise.DeviseCode,
                                    CustomerAdress=treasuryOperation.Till.Name, //caisse emetrice
                                    CompanyLogo = db.Files.FirstOrDefault(f => f.GlobalPersonID == Company.GlobalPersonID) != null ? db.Files.FirstOrDefault(f => f.GlobalPersonID == Company.GlobalPersonID).Content : COMPANY_LOGO
                                }
                        );
            }

            if (isValid)
            {
                path = Server.MapPath("~/Reports/CashRegister/RptBankTransfer.rpt");
                repName = "RptBankTransfer";
                rptH.Load(path);
                rptH.SetDataSource(model);
                bool isDownloadRpt = (bool)Session["isDownloadRpt"];
                rptH.ExportToHttpResponse(ExportFormatType.PortableDocFormat, System.Web.HttpContext.Current.Response, isDownloadRpt, repName);
            }
            else
            {
                Response.Write("Nothing Found; No Report name found");
            }
        }
        finally
            {
                rptH.Close();
                rptH.Dispose();
            }
        }
    }
}