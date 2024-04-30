using FatSod.Supply.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FatSod.Supply.Abstracts
{
    public interface IHistoSMS : IRepositorySupply<HistoSMS>
    {
        /// <summary>
        /// ajout des rdv ds le system
        /// </summary>
        /// 
        bool AddHistoSMS(HistoSMS histoSMS, int userConnet, DateTime serverDate, int CurrentBranchID);
    }
}
