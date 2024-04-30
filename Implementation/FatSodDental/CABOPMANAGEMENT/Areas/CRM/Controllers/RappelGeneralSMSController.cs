using FatSod.Ressources;
using FatSod.Security.Abstracts;
using FatSod.Security.Entities;
using FatSod.Supply.Abstracts;
using FatSod.Supply.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CABOPMANAGEMENT.Areas.CRM.Models;
using CABOPMANAGEMENT.Controllers;
using CABOPMANAGEMENT.Filters;
using CABOPMANAGEMENT.SMSGOLD;
using SaleE = FatSod.Supply.Entities.Sale;

namespace CABOPMANAGEMENT.Areas.CRM.Controllers
{
    [Authorize(Order = 1)]
    [TakeBusinessDay(Order = 2)]
    public class RappelGeneralSMSController : BaseController
    {
        private IBusinessDay _busDayRepo;
        private IHistoSMS _HistoSMSRepository;
        private List<BusinessDay> listBDUser;
        private IExtractSMS _ExtractSMSRepository;

        public RappelGeneralSMSController(
            IHistoSMS HistoSMSRepository,
            IExtractSMS ExtractSMSRepository,
            IBusinessDay busDayRepo
            )
        {
            this._HistoSMSRepository = HistoSMSRepository;
            this._busDayRepo = busDayRepo;
            this._ExtractSMSRepository = ExtractSMSRepository;
        }

        // GET: CRM/RappelGeneralSMS
        [OutputCache(Duration = 3600)]
        public ActionResult Index()
        {
            listBDUser = (List<BusinessDay>)Session["UserBusDays"];
            if (listBDUser == null)
            {
                listBDUser = _busDayRepo.GetOpenedBusinessDay(CurrentUser);
            }
            DateTime openedBD = listBDUser.FirstOrDefault().BDDateOperation;


            ViewBag.BusnessDayDate = openedBD.ToString("yyyy-MM-dd");//.BDDateOperation;
            
            return View(GetAllSelectedSMSCusto(0, openedBD));
        }
        
        public List<ExtractSMS> ModelSelectedSMSCusto(int NbreMois, DateTime ServerDate)
        {
            List<ExtractSMS> realDataTmp = new List<ExtractSMS>();
            realDataTmp = _ExtractSMSRepository.AddExtractSMS("GENERAL", "GENERAL", NbreMois, "MONTHS", ServerDate, SessionGlobalPersonID, ServerDate, SessionBusinessDay(null).BranchID);
            return realDataTmp;
        }


        public JsonResult GetAllSelectedSMSCusto(int NbreMois, DateTime ServerDate)
        {
            var model = new
            {
                data = from s in ModelSelectedSMSCusto(NbreMois, ServerDate)
                       select new
                       {
                           ID = s.ExtractSMSID,
                           CustomerID = s.CustomerID,
                           CustomerName = s.CustomerName,
                           CustomerPhone = s.CustomerPhone,
                           CustomerQuater = s.CustomerQuater,
                           SaleDeliveryDate = s.SaleDeliveryDate.ToString("dd/MM/yyyy")
                       }
            };

            var jsonResult = Json(model, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        public JsonResult Delete(int ExtractSMSID)
        {
            bool status = false;
            string Message = "";
            try
            {
                BusinessDay secbusday = SessionBusinessDay(null);
                _ExtractSMSRepository.DeleteExtractSMS(ExtractSMSID, SessionGlobalPersonID, secbusday.BDDateOperation, secbusday.BranchID);
                status = true;
                Message = Resources.Success + " Data has been deleted";
            }
            catch (Exception e)
            {
                Message = "Error " + e.Message;
                status = false;
            }
            return new JsonResult { Data = new { status = status, Message = Message } };
        }
        public JsonResult UpdatePhone(List<ExtractSMS> SMSItems)
        {

            bool status = false;
            string Message = "";

            try
            {
                int i = 0;

                foreach (ExtractSMS inLines in SMSItems)
                {
                    BusinessDay secbusday = SessionBusinessDay(null);
                    _ExtractSMSRepository.UpdateExtractSMS(inLines.ExtractSMSID, inLines.CustomerPhone, SessionGlobalPersonID, secbusday.BDDateOperation, secbusday.BranchID);

                    i = i + 1;

                }

                status = true;
                Message = "Update Phone Number successfully!";
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
        public JsonResult EnvoiSMS(List<ExtractSMS> SMSItems, DateTime DateEnvoi, string SmsEnvoye,int NbreMois)
        {

            bool status = false;
            string Message = "";
            string NewSmsEnvoye = "";
            string initSmsEnvoye = SmsEnvoye;

            DateTime dateRdv = new DateTime();
            string user = "";
            string pwd = "";
            int?[] result;
            //String[] destinatires; // 'Tableau de destinaitres

            ServiceSMSClient envoiSMS = new ServiceSMSClient();

            user = "VALDOZ";
            pwd = "lj4TXVEK";


            try
            {
                string[] destinatires = new string[SMSItems.Count];
                int i = 0;
                string newDestinataire = "";
                foreach (ExtractSMS inLines in SMSItems)
                {
                    ExtractSMS rdvous = db.ExtractSMSs.Find(inLines.ExtractSMSID);
                    inLines.CustomerID = rdvous.CustomerID;
                    inLines.CustomerName = rdvous.CustomerName;
                    inLines.CustomerPhone = rdvous.CustomerPhone;
                    inLines.CustomerQuater = rdvous.CustomerQuater;
                    inLines.SaleDeliveryDate = rdvous.SaleDeliveryDate;
                    dateRdv = inLines.SaleDeliveryDate.AddMonths(NbreMois);
                    NewSmsEnvoye = SmsEnvoye.Contains("{}") ? SmsEnvoye.Replace("{}", dateRdv.ToString("dd/MM/yyyy")):SmsEnvoye;
                    newDestinataire = rdvous.CustomerPhone;
                    result =  envoiSMS.envoyerUnSMS(user, pwd, newDestinataire, NewSmsEnvoye, "VALDOZOPTIC");
                    switch (result[3].Value)
                    {
                        case 0:
                            status = false;
                            Message = "Une exception s'est produite dans le système!";
                            break;
                        case 1:

                            //update send sms status
                            inLines.isSmsSent = true;
                            inLines.SendSMSDate = DateEnvoi;
                            inLines.Condition = inLines.Condition + " MONTHS";
                            _ExtractSMSRepository.Update(inLines, inLines.ExtractSMSID);
                            status = true;
                            Message = "Send SMS successfully!";
                            i = i + 1;
                            continue;
                        case 100:
                            status = false;
                            Message = "L’utilisateur passé en paramètre n'a pas assez de crédit SMS pour envoyer ce message!";
                            break;
                        case 404:
                            status = false;
                            Message = "login ou le mot de passe est incorrect!";
                            break;
                        default:
                            status = false;
                            Message = "login ou le mot de passe est incorrect!";
                            break;
                    }
                    

                }

                //ecriture histo sms
                HistoSMS histosms = new HistoSMS()
                {
                    DateEnvoi=DateEnvoi,
                    NbreSMS=i,
                    OperatorID=SessionGlobalPersonID,
                    SmsEnvoye=initSmsEnvoye,
                    TypeSms= "GENERAL"
                };
                _HistoSMSRepository.AddHistoSMS(histosms,SessionGlobalPersonID,DateEnvoi,SessionBusinessDay(null).BranchID);

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
    }
}