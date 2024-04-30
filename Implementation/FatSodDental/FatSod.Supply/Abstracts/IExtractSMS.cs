using FatSod.Supply.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FatSod.Supply.Abstracts
{
    public interface IExtractSMS : IRepositorySupply<ExtractSMS>
    {
        /// <summary>
        /// Methode permetant d'extraire les sms a envoyer
        /// </summary>
        /// 
        List<ExtractSMS> AddExtractSMS(string AlertDescrip, string TypeAlert,int condition, string MoisOuJour, DateTime SendSMSDate, int userConnet, DateTime serverDate, int CurrentBranchID);
        /// <summary>
        /// Methode permetant de modifier le numero de telephone avant d'envoyer les sms
        /// </summary>
        /// 
        bool UpdateExtractSMS(int ExtractSMSID,string Telephone, int userConnet, DateTime serverDate, int CurrentBranchID);

        bool DeleteExtractSMS(int ExtractSMSID, int userConnet, DateTime serverDate, int CurrentBranchID);
    }
}
