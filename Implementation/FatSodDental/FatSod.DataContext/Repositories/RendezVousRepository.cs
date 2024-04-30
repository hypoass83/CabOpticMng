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
    public class RendezVousRepository : RepositorySupply<RendezVous>, IRendezVous
    {
        public RendezVousRepository(EFDbContext context)
        {
            this.context = context;
        }
        public RendezVousRepository()
            : base()
        {

        }

        public bool AddRendezVous(RendezVous rendezVous,int SaleID, int userConnet, DateTime serverDate,int CurrentBranchID)
        {
            bool res = false;
            try
            {
                //ajout table rdv
                RendezVous newRdv = new RendezVous();

                //ces 2 lignes qui suivent permettent de transformer un ProductModel en Product 
                //create a Map
                Mapper.CreateMap<RendezVous, RendezVous>();
                //use Map
                newRdv = Mapper.Map<RendezVous>(rendezVous);

                newRdv = context.RendezVous.Add(newRdv);
                context.SaveChanges();

                //RendezVous newRdv = context.RendezVous.Add(rendezVous);
                //context.SaveChanges();
                //fabrication de lhisto rdv
                HistoRendezVous histrdv = new HistoRendezVous()
                {
                    SaleID= SaleID,
                    DateRdv=rendezVous.DateRdv,
                    RendezVousID=newRdv.RendezVousID,
                    Remarque="Create RDV - " + rendezVous.RaisonRdv,
                    OperatorID=userConnet
                };
                context.HistoRendezVous.Add(histrdv);
                context.SaveChanges();

                //EcritureSneack
                IMouchar opSneak = new MoucharRepository(context);
                res = opSneak.InsertOperation(userConnet, "SUCCESS", "Add RDV- ID : " + newRdv.RendezVousID, "AddRendezVous", serverDate, CurrentBranchID);
                if (!res)
                {
                    throw new Exception("Une erreur s'est produite lors de la journalisation ");
                }

                res = true;
                return res;
            }
            catch (Exception ex)
            {
                res = false;
                throw new Exception(ex.Message);
            }
        }

       
        public bool UpdateRendezVous(int RendezVousID, int SaleID, DateTime newRdvDate, string raisonModif, int userConnet, DateTime serverDate,int CurrentBranchID)
        {
            bool res = false;
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    //recuperation du rdv
                    RendezVous OldRDV = context.RendezVous.Find(RendezVousID);
                    //modif
                    OldRDV.DateRdv = newRdvDate;
                    OldRDV.RaisonRdv = raisonModif;
                    context.SaveChanges();
                    //ecriture histo
                    //fabrication de lhisto rdv
                    HistoRendezVous histrdv = new HistoRendezVous()
                    {
                        SaleID = SaleID,
                        DateRdv = newRdvDate,
                        RendezVousID = RendezVousID,
                        Remarque = raisonModif,
                        OperatorID=userConnet
                    };
                    context.HistoRendezVous.Add(histrdv);
                    context.SaveChanges();

                    //EcritureSneack
                    IMouchar opSneak = new MoucharRepository(context);
                    res = opSneak.InsertOperation(userConnet, "SUCCESS", "UPdate RDV- ID : " + RendezVousID, "UpdateRendezVous", serverDate, CurrentBranchID);
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
