using FatSod.DataContext.Concrete;
using FatSod.Security.Abstracts;
using FatSod.Security.Entities;
using FatSod.Supply.Abstracts;
using FatSod.Supply.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FatSod.DataContext.Repositories
{
    /// <summary>
    /// 
    /// </summary>
    public class CompteurBorderoDepotRepository : RepositorySupply<CompteurBorderoDepot>, ICompteurBorderoDepot
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public CompteurBorderoDepotRepository(EFDbContext context)
        {
            this.context = context;
        }
        /// <summary>
        /// 
        /// </summary>
        public CompteurBorderoDepotRepository()
            : base()
        {

        }
        
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="AssuranceID"></param>
        /// <param name="CompanyID"></param>
        /// <param name="LieuxdeDepotBorderoID"></param>
        /// <param name="CurrentYear"></param>
        /// <param name="isValidated"></param>
        /// <param name="Compteur"></param>
        /// <returns></returns>
        public string GenerateBDFCode(int AssuranceID, int CurrentYear, string CompanyID = "0", int LieuxdeDepotBorderoID = 0, bool isValidated=false,int Compteur=0)
        {
            string res = "", CompteurBorderoDepotCode="";
            int Counter = 0;
            if (AssuranceID>0)
            {
                CompteurBorderoDepot existCpteBD = new CompteurBorderoDepot();
                if (CompanyID == "0" || CompanyID == null)
                {
                    if (LieuxdeDepotBorderoID == 0)
                    {
                        existCpteBD = context.CompteurBorderoDepots.Where(c => c.AssureurID == AssuranceID && c.YearOperation == CurrentYear).FirstOrDefault();
                    }
                    else
                    {
                        existCpteBD = context.CompteurBorderoDepots.Where(c => c.AssureurID == AssuranceID && c.YearOperation == CurrentYear && c.LieuxdeDepotBorderoID == LieuxdeDepotBorderoID).FirstOrDefault();
                    }
                }
                else
                {
                    if (LieuxdeDepotBorderoID == 0)
                    {
                        existCpteBD = context.CompteurBorderoDepots.Where(c => c.AssureurID == AssuranceID && c.YearOperation == CurrentYear && c.CompanyID == CompanyID).FirstOrDefault();
                    }
                    else
                    {
                        existCpteBD = context.CompteurBorderoDepots.Where(c => c.AssureurID == AssuranceID && c.YearOperation == CurrentYear && c.CompanyID == CompanyID && c.LieuxdeDepotBorderoID == LieuxdeDepotBorderoID).FirstOrDefault();
                    }
                }

                if (existCpteBD == null)
                {
                    Counter = (Compteur > 0) ? Compteur : 1;
                    //CompteurBorderoDepotCode = "0001";

                    int nbrezero = 0;
                    nbrezero = 4 - Counter.ToString().Length;
                    string valzero = "";
                    for (int nbre = 1; nbre <= nbrezero; nbre++)
                    {
                        valzero = String.Concat(valzero, "0");
                    }

                    CompteurBorderoDepotCode = valzero + Counter.ToString();


                    existCpteBD = new CompteurBorderoDepot()
                    {
                        AssureurID = AssuranceID,
                        CompanyID = CompanyID,
                        CompteurBorderoDepotCode = CompteurBorderoDepotCode,
                        Counter = Counter,
                        LieuxdeDepotBorderoID =  LieuxdeDepotBorderoID,
                        YearOperation = CurrentYear
                    };
                    context.CompteurBorderoDepots.Add(existCpteBD);
                    context.SaveChanges();
                }
                else
                {
                    Counter = (Compteur>0) ? Compteur : (isValidated || existCpteBD.Counter == 0) ? existCpteBD.Counter + 1 : existCpteBD.Counter;
                    int nbrezero = 0;
                    nbrezero = 4 - Counter.ToString().Length;
                    //nbrezero = 4 - existCpteBD.Counter.ToString().Length;
                    string valzero = "";
                    for (int nbre = 1; nbre <= nbrezero; nbre++)
                    {
                        valzero = String.Concat(valzero, "0");
                    }
                    //Counter = (isValidated || existCpteBD.Counter==0) ? existCpteBD.Counter + 1 : existCpteBD.Counter;
                    CompteurBorderoDepotCode = valzero + Counter.ToString();

                    existCpteBD.Counter = Counter;
                    existCpteBD.CompteurBorderoDepotCode = CompteurBorderoDepotCode;
                    existCpteBD.CompanyID = CompanyID;
                    existCpteBD.LieuxdeDepotBorderoID = LieuxdeDepotBorderoID;
                    existCpteBD.YearOperation = CurrentYear;
                    context.SaveChanges();
                }

                res = CompteurBorderoDepotCode + "/" + CurrentYear.ToString();
            }
            

            return res;
        }

    }
}
