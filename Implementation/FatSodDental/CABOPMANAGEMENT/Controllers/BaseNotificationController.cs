using FatSod.Security.Entities;
using FatSod.Supply.Abstracts;
using FatSod.Supply.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace CABOPMANAGEMENT.Controllers
{
    public class BaseNotificationController : BaseController
    {
        public IHistoSMS _HistoSMSRepository;
        public IExtractSMS _ExtractSMSRepository;
        public INotificationSetting _notificationSettingRepository;

        public BaseNotificationController(IHistoSMS HistoSMSRepository, IExtractSMS ExtractSMSRepository,
                INotificationSetting notificationSettingRepository)
        {
            this._HistoSMSRepository = HistoSMSRepository;
            this._ExtractSMSRepository = ExtractSMSRepository;
            this._notificationSettingRepository = notificationSettingRepository;
        }

        public JsonResult getNotificationSetting(NotificationType type)
        {
            NotificationSetting pr = _notificationSettingRepository.FindAll.FirstOrDefault(ns => ns.NotificationType == type);

            if(pr == null)
            {
                return Json(new { FrenchMessage = "", EnglishMessage = ""}, JsonRequestBehavior.AllowGet);
            }

            return Json(pr, JsonRequestBehavior.AllowGet);
        }

        #region api.web2sms237.com
        public int SendSMSToURL(string userPhone, int CustomerID, DateTime dateEnvoi, string typeSms, string accessToken, string message, string senderid)
        {

            string SentResult = String.Empty;
            int StatusCode = 0;
            //string Message = "";

            // We remove all white spaces in the string
            userPhone = userPhone.Replace(" ", String.Empty);
            // If 
            userPhone = userPhone.Length == 9 ? userPhone : userPhone.Substring(0, 9);
            userPhone = "+237" + userPhone;

            //vefif if this sms not yet send for this date
            HistoSMS histoSMS = db.HistoSMSs.Where(c => c.CashCustomerID == CustomerID && c.DateEnvoi == dateEnvoi && c.TypeSms == typeSms).FirstOrDefault();
            if (histoSMS != null)
            {
                StatusCode = 100;
                return StatusCode;
            }
            var getUri = "https://api.web2sms237.com/sms/send";
            List<object> SMSResult = new List<object>();

            var postData = "sender_id=" + Uri.EscapeDataString(senderid);
            postData += "&phone=" + Uri.EscapeDataString(userPhone);
            postData += "&text=" + Uri.EscapeDataString(message);

            var data = Encoding.ASCII.GetBytes(postData);

            var webRequest = (HttpWebRequest)WebRequest.Create(getUri);
            webRequest.ContentLength = data.Length;
            webRequest.Method = "POST";
            webRequest.ContentType = "application/x-www-form-urlencoded";
            //webRequest.ContentType = "application/json";
            webRequest.Headers.Add("Authorization", "Bearer " + accessToken);
            using (var stream = webRequest.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            var webResponse = (HttpWebResponse)webRequest.GetResponse();

            switch (webResponse.StatusCode)
            {
                case HttpStatusCode.OK:
                    StatusCode = 200;
                    //Message=webResponse.m
                    break;
                case HttpStatusCode.Unauthorized:
                    StatusCode = 2;
                    break;
                case HttpStatusCode.PaymentRequired:
                    StatusCode = 201;
                    break;
                case HttpStatusCode.BadRequest:
                    StatusCode = 404;
                    break;

                default:
                    break;
            }

            var content = "";

            using (var stream = new StreamReader(webResponse.GetResponseStream()))
            {
                content = stream.ReadToEnd();
            }


            //var serializer = new JavaScriptSerializer();
            //dynamic objectData = serializer.DeserializeObject(content);


            //int StartIndex = 0;
            //int LastIndex = resultmsg.Length;

            //if (LastIndex > 0)
            //    SentResult = resultmsg.Substring(StartIndex, LastIndex);

            //if (SentResult.Contains("error")) StatusCode = 0;
            //else if (SentResult.ToLower().Contains("solde insuffisant")) StatusCode = 201;
            //else if (SentResult.ToLower().Contains("mot de passe incorrect")) StatusCode = 2;
            //else if (SentResult.ToLower().Contains("destinataire incorrect")) StatusCode = 404;
            //else if (SentResult.ToLower().Contains(userPhone)) StatusCode = 200;

            return StatusCode;

        }
        public JsonResult EnvoiSMS(string smsEnvoyeFR, string smsEnvoyeEN, int SMSLng, List<SMSReceiverModel> selectedPatients, string TypeSms)
        {

            bool status = false;
            string Message = "";
            string NewSmsEnvoye = "";
            var apiUserId = "67d5e8f3-3b1d-430e-883e-e06a00b4e868";
            var apiUserSecret = "3680d0eb9fa59a8614a04cd291b16f0d";
            var encodedSecretAndApi = "NjdkNWU4ZjMtM2IxZC00MzBlLTg4M2UtZTA2YTAwYjRlODY4OjM2ODBkMGViOWZhNTlhODYxNGEwNGNkMjkxYjE2ZjBk";
            
            
            //string user = "";
            //string pwd = "";
            //int?[] result;

            BusinessDay Envoibusday = SessionBusinessDay(null);
            DateTime DateEnvoi = Envoibusday.BDDateOperation;

            //ServiceSMSClient envoiSMS = new ServiceSMSClient();

            //user = "VALDOZ";
            //pwd = "lj4TXVEK";

            string AppNameP = WebConfigurationManager.AppSettings["AppNameP"];

            var cultureInfo = System.Threading.Thread.CurrentThread.CurrentCulture;

            AppNameP = cultureInfo.TextInfo.ToTitleCase(AppNameP.ToLower());
            string senderid = AppNameP;

            //TextInfo ti = CultureInfo.CurrentCulture.TextInfo;
            //string senderid = ti.ToTitleCase(AppNameP);

            var token = (senderid=="Valdoz") ? "OTyMnbatmUZCRXrEsrQgY3z1xQM=" : "fS5LfUmRAnz9JHKjMj95MkQ9A=";
            try
            {
                string[] destinatires = new string[selectedPatients.Count];
                int i = 0;
                string newDestinataire = "";

                string[] bannedNumbers = db.BannedNumbers.Select(bn => bn.Number).ToArray();
                    // { "691491729", "675488938", "690289009" };

                selectedPatients = selectedPatients.Where(p => !bannedNumbers.Contains(p.CustomerPhone.Trim())).ToList();

                foreach (SMSReceiverModel inLines in selectedPatients)
                {

                    if (SMSLng == 0 || inLines.SMSLng == 0)
                    {
                        NewSmsEnvoye = smsEnvoyeFR.Contains("{name}") ? smsEnvoyeFR.Replace("{name}", inLines.CustomerName) : smsEnvoyeFR;
                        NewSmsEnvoye = NewSmsEnvoye.Contains("{date}") ? NewSmsEnvoye.Replace("{date}", inLines.SaleDeliveryDate) : NewSmsEnvoye;
                        string civility = inLines.Civility == "Civility_M" ? "M." :
                                          (inLines.Civility == "Civility_F" ? "Mme." : "M./Mme.");
                        NewSmsEnvoye = NewSmsEnvoye.Contains("{civility}") ? NewSmsEnvoye.Replace("{civility}", civility) : NewSmsEnvoye;
                    }

                    if (SMSLng == 1 || inLines.SMSLng == 1)
                    {
                        NewSmsEnvoye = smsEnvoyeEN.Contains("{name}") ? smsEnvoyeEN.Replace("{name}", inLines.CustomerName) : smsEnvoyeEN;
                        NewSmsEnvoye = NewSmsEnvoye.Contains("{date}") ? NewSmsEnvoye.Replace("{date}", inLines.SaleDeliveryDate) : NewSmsEnvoye;
                        string civility = inLines.Civility == "Civility_M" ? "Mr." :
                                          (inLines.Civility == "Civility_F" ? "Mrs." : "Mr./Mrs.");
                        NewSmsEnvoye = NewSmsEnvoye.Contains("{civility}") ? NewSmsEnvoye.Replace("{civility}", civility) : NewSmsEnvoye;
                    }

                    newDestinataire = inLines.CustomerPhone; //"679209406"; // 

                    var UrlRes = SendSMSToURL(newDestinataire, inLines.CustomerID, DateEnvoi, TypeSms, token, NewSmsEnvoye, senderid);

                    switch (UrlRes) // (result[3].Value)
                    {
                        case 0:
                            status = false;
                            Message = "Une exception s'est produite dans le système!";
                            break;
                        case 200:
                            //ecriture histo sms
                            HistoSMS histosms = new HistoSMS()
                            {
                                DateEnvoi = DateEnvoi,
                                NbreSMS = 1,
                                OperatorID = SessionGlobalPersonID,
                                SmsEnvoye = NewSmsEnvoye,
                                CashCustomerID = inLines.CustomerID,
                                TypeSms = TypeSms
                            };
                            _HistoSMSRepository.AddHistoSMS(histosms, SessionGlobalPersonID, DateEnvoi, SessionBusinessDay(null).BranchID);

                            Message = "Send SMS successfully!";
                            continue;
                        case 2:
                            status = false;
                            Message = "login or Password incorrect!";
                            break;
                        case 100:
                            status = false;
                            Message = "SMS Alredy send to this customer!";
                            continue;
                        case 201:
                            status = false;
                            Message = "insufficient balance!";
                            break;
                        case 404:
                            status = false;
                            Message = newDestinataire + " Wrong phone number!";
                            continue;
                        case 422:
                            status = false;
                            Message = newDestinataire + " Wrong phone number!";
                            continue;
                        default:
                            status = false;
                            Message = newDestinataire + " login or Password incorrect!";
                            break;
                    }
                    if (UrlRes != 200)
                    {
                        Message += "<br>";
                    }
                    i = i + 1;

                }

                return new JsonResult { Data = new { status = status, Message = Message } };
            }
            catch (Exception e)
            {
                status = false;
                Message = @"Error : <br/>Error : <code>" + e.Message + "</code>";
                return new JsonResult { Data = new { status = status, Message = Message } };
            }
        }

        #endregion
        #region sms.etech-keys.com
        public int OldSendSMSToURL(string getUri, string userPhone, int CustomerID, DateTime dateEnvoi, string typeSms)
        {

            string SentResult = String.Empty;
            int StatusCode = 0;

            //vefif if this sms not yet send for this date
            HistoSMS histoSMS = db.HistoSMSs.Where(c => c.CashCustomerID == CustomerID && c.DateEnvoi == dateEnvoi && c.TypeSms == typeSms).FirstOrDefault();
            if (histoSMS != null)
            {
                StatusCode = 100;
                return StatusCode;
            }

            List<object> SMSResult = new List<object>();

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(getUri);

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader responseReader = new StreamReader(response.GetResponseStream());

            String resultmsg = responseReader.ReadToEnd();
            responseReader.Close();

            int StartIndex = 0;
            int LastIndex = resultmsg.Length;

            if (LastIndex > 0)
                SentResult = resultmsg.Substring(StartIndex, LastIndex);

            if (SentResult.Contains("error")) StatusCode = 0;
            else if (SentResult.ToLower().Contains("solde insuffisant")) StatusCode = 201;
            else if (SentResult.ToLower().Contains("mot de passe incorrect")) StatusCode = 2;
            else if (SentResult.ToLower().Contains("destinataire incorrect")) StatusCode = 404;
            else if (SentResult.ToLower().Contains(userPhone)) StatusCode = 200;


            return StatusCode;

        }
        public JsonResult OldEnvoiSMS(string smsEnvoyeFR, string smsEnvoyeEN, int SMSLng, List<SMSReceiverModel> selectedPatients, string TypeSms)
        {

            bool status = false;
            string Message = "";
            string NewSmsEnvoye = "";


            //string user = "";
            //string pwd = "";
            //int?[] result;

            BusinessDay Envoibusday = SessionBusinessDay(null);
            DateTime DateEnvoi = Envoibusday.BDDateOperation;

            //ServiceSMSClient envoiSMS = new ServiceSMSClient();

            //user = "VALDOZ";
            //pwd = "lj4TXVEK";

            string AppNameP = WebConfigurationManager.AppSettings["AppNameP"];
            try
            {
                string[] destinatires = new string[selectedPatients.Count];
                int i = 0;
                string newDestinataire = "";
                foreach (SMSReceiverModel inLines in selectedPatients)
                {

                    if (SMSLng == 0 || inLines.SMSLng == 0)
                    {
                        NewSmsEnvoye = smsEnvoyeFR.Contains("{name}") ? smsEnvoyeFR.Replace("{name}", inLines.CustomerName) : smsEnvoyeFR;
                        NewSmsEnvoye = NewSmsEnvoye.Contains("{date}") ? NewSmsEnvoye.Replace("{date}", inLines.SaleDeliveryDate) : NewSmsEnvoye;
                        string civility = inLines.Civility == "Civility_M" ? "M." :
                                          (inLines.Civility == "Civility_F" ? "Mme." : "M./Mme.");
                        NewSmsEnvoye = NewSmsEnvoye.Contains("{civility}") ? NewSmsEnvoye.Replace("{civility}", civility) : NewSmsEnvoye;
                    }

                    if (SMSLng == 1 || inLines.SMSLng == 1)
                    {
                        NewSmsEnvoye = smsEnvoyeEN.Contains("{name}") ? smsEnvoyeEN.Replace("{name}", inLines.CustomerName) : smsEnvoyeEN;
                        NewSmsEnvoye = NewSmsEnvoye.Contains("{date}") ? NewSmsEnvoye.Replace("{date}", inLines.SaleDeliveryDate) : NewSmsEnvoye;
                        string civility = inLines.Civility == "Civility_M" ? "Mr." :
                                          (inLines.Civility == "Civility_F" ? "Mrs." : "Mr./Mrs.");
                        NewSmsEnvoye = NewSmsEnvoye.Contains("{civility}") ? NewSmsEnvoye.Replace("{civility}", civility) : NewSmsEnvoye;
                    }

                    newDestinataire = inLines.CustomerPhone; //"679209406"; // 

                    //string linkmsg = "https://sms.etech-keys.com/ss/api.php?login=695060926&password=hyppo2019&sender_id="+ AppNameP + "&destinataire=" + newDestinataire + "&message=" + NewSmsEnvoye + "&ext_id=0123456&programmation=0";

                    string linkmsg = "https://sms.etech-keys.com/ss/api.php?login=677517601&password=Valdoz2020&sender_id=" + AppNameP + "&destinataire=" + newDestinataire + "&message=" + NewSmsEnvoye + "&ext_id=0123456&programmation=0";

                    int UrlRes = OldSendSMSToURL(linkmsg, newDestinataire, inLines.CustomerID, DateEnvoi, TypeSms);

                    switch (UrlRes) // (result[3].Value)
                    {
                        case 0:
                            status = false;
                            Message = "Une exception s'est produite dans le système!";
                            break;
                        case 200:
                            //ecriture histo sms
                            HistoSMS histosms = new HistoSMS()
                            {
                                DateEnvoi = DateEnvoi,
                                NbreSMS = 1,
                                OperatorID = SessionGlobalPersonID,
                                SmsEnvoye = NewSmsEnvoye,
                                CashCustomerID = inLines.CustomerID,
                                TypeSms = TypeSms
                            };
                            _HistoSMSRepository.AddHistoSMS(histosms, SessionGlobalPersonID, DateEnvoi, SessionBusinessDay(null).BranchID);

                            Message = "Send SMS successfully!";
                            continue;
                        case 2:
                            status = false;
                            Message = "login or Password incorrect!";
                            break;
                        case 100:
                            status = false;
                            Message = "SMS Alredy send to this customer!";
                            continue;
                        case 201:
                            status = false;
                            Message = "insufficient balance!";
                            break;
                        case 404:
                            status = false;
                            Message = "Wrong phone number!";
                            break;
                        default:
                            status = false;
                            Message = "login or Password incorrect!";
                            break;
                    }
                    i = i + 1;

                }


                return new JsonResult { Data = new { status = status, Message = Message } };
            }
            catch (Exception e)
            {
                status = false;
                Message = @"Une erreur s'est produite lors de l'opération, veuillez contactez l'administrateur et/ou essayez à nouveau
                                    <br/>Error : <code>" + e.Message + "</code>";
                return new JsonResult { Data = new { status = status, Message = Message } };
            }
        }
        #endregion

        public int getPreferredLanaguage(string language)
        {
            int PreferredLanguage = -1;
            if (language == "FR" || language == "EN")
            {
                PreferredLanguage = language == "FR" ? 0 : 1;
            }

            return PreferredLanguage;
        }

    }

    public class SMSReceiverModel
    {
        public DateTime dateRDV { get; set; }
        public int ID { get; set; }
        public int CustomerID { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public string CustomerQuater { get; set; }
        public string SaleDeliveryDate { get; set; }
        public string Civility { get; set; }
        public string CivilityUI { get; set; }
        // -1 Ni l'anglais ni le francais; 0 => FR; 1 => EN
        public int SMSLng { get; set; } = -1;
    }
}