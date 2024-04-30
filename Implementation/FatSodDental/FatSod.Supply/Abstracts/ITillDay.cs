using FatSod.Security.Entities;
using FatSod.Supply.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FatSod.Supply.Abstracts
{
    public interface ITillDay : IRepositorySupply<TillDay>
    {
        /// <summary>
        /// Cette méthode retourne la balance de la caisse à un moment donné
        /// </summary>
        /// <returns></returns>
        //Double TillBalance(Till till);
        TillSatut TillStatus(int tillID);
        TillSatut TillStatus(Till till, DateTime bussinessDayDate);
        TillSatut TillStatus(Till till, DateTime bussinessDayDate,double LastClosingAmount);
        /// <summary>
        /// Cette méthode répond à la question est ce que cette caisse est ouverte ce jour
        /// </summary>
        /// <param name="till"></param>
        /// <param name="bussinessDayDate"></param>
        /// <returns></returns>
        bool IsTillOpened(Till till, DateTime bussinessDayDate);

        TillDayStatus TillDayStatus(int tillID);

        /// <summary>
        /// methode qui s'execute a l'ouverture d'une caisse
        /// </summary>
        /// <param name="tillDay"></param>
        /// <param name="YesterdayClosingPrice"></param>
        /// <param name="CashInitialization"></param>
        /// <returns></returns>
        bool OpenDay(TillDay tillDay, double YesterdayClosingPrice, double CashInitialization, int UserConnect, DateTime BusinessDate, int BranchID);

        /// <summary>
        /// methode qui s'execute a la fermeture de la caisse
        /// </summary>
        /// <param name="tillDay"></param>
        /// <param name="InputCash"></param>
        /// <param name="OutputCash"></param>
        /// <param name="YesterdayTillDayClosingPrice"></param>
        /// <returns></returns>
        bool CloseDay(TillDay tillDay, double InputCash, double OutputCash, double YesterdayTillDayClosingPrice, int UserConnect, DateTime BusinessDate, int BranchID);

        bool ForceOpenDay(TillDay TillDays, double YesterdayClosingPrice, double CashInitialization, int UserConnect, DateTime BusinessDate, int BranchID);
    }
}
