using FatSod.Security.Entities;
using FatSod.Supply.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FatSod.Supply.Abstracts
{
    public interface ICompteurBorderoDepot : IRepositorySupply<CompteurBorderoDepot>
    {
        string GenerateBDFCode(int AssuranceID, int CurrentYear, string CompanyID = "0", int LieuxdeDepotBorderoID = 0, bool isValidated = false, int Compteur = 0);

    }
}
