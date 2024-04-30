using FatSod.Supply.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FatSod.Supply.Abstracts
{
    public interface IRendezVous : IRepositorySupply<RendezVous>
    {
        /// <summary>
        /// ajout des rdv ds le system
        /// </summary>
        /// 
        bool AddRendezVous(RendezVous rendezVous, int SaleID, int userConnet,DateTime serverDate, int CurrentBranchID);
        /// <summary>
        /// modification du rev
        /// </summary>
        /// 
        bool UpdateRendezVous(int RendezVousID, int SaleID, DateTime newRdvDate, string raisonModif, int userConnet, DateTime serverDate, int CurrentBranchID);
    }
}
