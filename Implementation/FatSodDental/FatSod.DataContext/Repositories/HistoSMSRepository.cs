using AutoMapper;
using FatSod.DataContext.Concrete;
using FatSod.Security.Abstracts;
using FatSod.Supply.Abstracts;
using FatSod.Supply.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace FatSod.DataContext.Repositories
{
    public class HistoSMSRepository : RepositorySupply<HistoSMS>, IHistoSMS
    {
        public HistoSMSRepository(EFDbContext context)
        {
            this.context = context;
        }
        public HistoSMSRepository()
            : base()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="histoSMS"></param>
        /// <param name="userConnet"></param>
        /// <param name="serverDate"></param>
        /// <param name="CurrentBranchID"></param>
        /// <returns></returns>
        public bool AddHistoSMS(HistoSMS histoSMS, int userConnet, DateTime serverDate, int CurrentBranchID)
        {
            bool res = false;
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    //ajout table rdv
                    HistoSMS newRdv = new HistoSMS();

                    //ces 2 lignes qui suivent permettent de transformer un ProductModel en Product 
                    //create a Map
                    Mapper.CreateMap<HistoSMS, HistoSMS>();
                    //use Map
                    newRdv = Mapper.Map<HistoSMS>(histoSMS);

                    newRdv = context.HistoSMSs.Add(newRdv);
                    context.SaveChanges();

                
                    //EcritureSneack
                    IMouchar opSneak = new MoucharRepository(context);
                    res = opSneak.InsertOperation(userConnet, "SUCCESS", "Add SMS- ID : " + newRdv.HistoSMSID, "AddHistoSMS", serverDate, CurrentBranchID);
                    if (!res)
                    {
                        throw new Exception("Une erreur s'est produite lors de la journalisation ");
                    }

                    res = true;
                    ts.Complete();
                }
                return res;
            }
            catch (Exception ex)
            {
                res = false;
                throw new Exception(ex.Message);
            }
        }
        
    }
}
