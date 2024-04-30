using FatSod.Ressources;
using FatSod.Security.Abstracts;
using FatSod.Security.Entities;
using FatSod.Supply.Abstracts;
using FatSod.Supply.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CABOPMANAGEMENT.Controllers;
using CABOPMANAGEMENT.Filters;

namespace CABOPMANAGEMENT.Areas.SMSMNG.Controllers
{
    [Authorize(Order = 1)]
    [TakeBusinessDay(Order = 2)]
    public class EvenementSMSController : BaseController
    {
        private IBusinessDay _busDayRepo;
        private IHistoSMS _HistoSMSRepository;
        private List<BusinessDay> listBDUser;
        private IExtractSMS _ExtractSMSRepository;

        public EvenementSMSController(
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

            return View(GetAllSelectedSMSCusto(0, "", openedBD));
        }
        public List<ExtractSMS> ModelSelectedSMSCusto(int EvenementID,string EvenementName, DateTime ServerDate)
        {
            List<ExtractSMS> realDataTmp = new List<ExtractSMS>();
            realDataTmp = _ExtractSMSRepository.AddExtractSMS("EVENEMENT", EvenementName, EvenementID, "", ServerDate, SessionGlobalPersonID, ServerDate, SessionBusinessDay(null).BranchID);
            return realDataTmp;
        }


        public JsonResult populateEvement()
        {

            List<object> EvementList = new List<object>();
           
            EvementList.Add(new
            {
                ID = 1,
                Name = Resources.NewYear
            });
           
            EvementList.Add(new
            {
                ID = 2,
                Name = Resources.YouthDay
            });
            EvementList.Add(new
            {
                ID = 3,
                Name = Resources.ValentineDay
            });

            EvementList.Add(new
            {
                ID = 4,
                Name = Resources.WomensDay
            });
            EvementList.Add(new
            {
                ID = 5,
                Name = Resources.Easter
            });

            EvementList.Add(new
            {
                ID = 6,
                Name = Resources.LabourDay
            });
            EvementList.Add(new
            {
                ID = 7,
                Name = Resources.NationalDay
            });

            EvementList.Add(new
            {
                ID = 8,
                Name = Resources.MotherDay
            });
            EvementList.Add(new
            {
                ID = 9,
                Name = Resources.FatherDay
            });

            EvementList.Add(new
            {
                ID = 10,
                Name = Resources.Christmax
            });
            return Json(EvementList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult OnEvenementSelected(int? EvenementID)
        {
            List<object> _InfoList = new List<object>();
            string EvenementName = "";
            

            if ((!EvenementID.HasValue || EvenementID.Value <= 0))
            { return Json(_InfoList, JsonRequestBehavior.AllowGet); }

            switch (EvenementID)
            {
                case 1:
                    EvenementName = Resources.NewYear;
                    break;
                case 2:
                    EvenementName = Resources.YouthDay;
                    break;
                case 3:
                    EvenementName = Resources.ValentineDay;
                    break;
                case 4:
                    EvenementName = Resources.WomensDay;
                    break;
                case 5:
                    EvenementName = Resources.Easter;
                    break;
                case 6:
                    EvenementName = Resources.LabourDay;
                    break;
                case 7:
                    EvenementName = Resources.NationalDay;
                    break;
                case 8:
                    EvenementName = Resources.MotherDay;
                    break;
                case 9:
                    EvenementName = Resources.FatherDay;
                    break;
                case 10:
                    EvenementName = Resources.Christmax;
                    break;
            }

            _InfoList.Add(new
            {
                EvenementName = EvenementName
            });
            return Json(_InfoList, JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetAllSelectedSMSCusto(int EvenementID, string EvenementName, DateTime ServerDate)
        {
            var model = new
            {
                data = from s in ModelSelectedSMSCusto(EvenementID,EvenementName,ServerDate)
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

        /*public JsonResult EnvoiSMS(List<ExtractSMS> SMSItems, DateTime DateEnvoi, string SmsEnvoye)
        {

            bool status = false;
            string Message = "";
            string NewSmsEnvoye = "";
            string initSmsEnvoye = SmsEnvoye;

            string user = "";
            string pwd = "";
            int?[] result;

            var culture = System.Globalization.CultureInfo.GetCultureInfo("fr-FR");
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

                    NewSmsEnvoye = SmsEnvoye.Contains("{}") ? SmsEnvoye.Replace("{}", inLines.TypeAlert) : SmsEnvoye;
                    newDestinataire = inLines.CustomerPhone;
                    result = envoiSMS.envoyerUnSMS(user, pwd, newDestinataire, NewSmsEnvoye, "VALDOZOPTIC");
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
                            inLines.Condition = inLines.Condition;
                            _ExtractSMSRepository.Update(inLines);
                            status = true;
                            Message = "Send SMS successfully!";
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
                    i = i + 1;

                }

                //ecriture histo sms
                HistoSMS histosms = new HistoSMS()
                {
                    DateEnvoi = DateEnvoi,
                    NbreSMS = i,
                    OperatorID = SessionGlobalPersonID,
                    SmsEnvoye = initSmsEnvoye,
                    TypeSms = "EVENEMENT"
                };
                _HistoSMSRepository.AddHistoSMS(histosms, SessionGlobalPersonID, DateEnvoi, SessionBusinessDay(null).BranchID);

                return new JsonResult { Data = new { status = status, Message = Message } };
            }
            catch (Exception e)
            {
                status = false;
                Message = @"Une erreur s'est produite lors de l'opération, veuillez contactez l'administrateur et/ou essayez à nouveau
                                    <br/>Code : <code>" + e.Message + " car " + e.StackTrace + "</code>";
                return new JsonResult { Data = new { status = status, Message = Message } };
            }
        }*/
    }
}