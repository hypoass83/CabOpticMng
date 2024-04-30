using FatSod.DataContext.Concrete;
using FatSod.Security.Abstracts;
using FatSod.Supply.Abstracts;
using FatSod.Supply.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace FatSod.DataContext.Repositories
{
    public class ExtractSMSRepository : RepositorySupply<ExtractSMS>, IExtractSMS
    {
        public ExtractSMSRepository(EFDbContext context)
        {
            this.context = context;
        }
        public ExtractSMSRepository()
            : base()
        {

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="AlertDescrip"></param>
        /// <param name="TypeAlert"></param>
        /// <param name="condition"></param>
        /// <param name="MoisOuJour"></param>
        /// <param name="SendSMSDate"></param>
        /// <param name="userConnet"></param>
        /// <param name="serverDate"></param>
        /// <param name="CurrentBranchID"></param>
        /// <returns></returns>
        public List<ExtractSMS> AddExtractSMS(string AlertDescrip, string TypeAlert, int condition, string MoisOuJour, DateTime SendSMSDate, int userConnet, DateTime serverDate, int CurrentBranchID)
        {
            ExtractSMS realDataTmp = new ExtractSMS();
            List<ExtractSMS> FinalDataTmp = new List<ExtractSMS>();
            

            try
            {
                //using (TransactionScope ts = new TransactionScope())
                //{
                    string FinalCondition = (condition==0) ? "TODAY" : condition.ToString() + " " + MoisOuJour;
                    switch (AlertDescrip)
                    {
                        case "EVENEMENT":
                            List<Customer> datacltTmp = new List<Customer>();

                            if (condition==1 || condition == 2 || condition == 3 || condition == 5 || condition == 6
                                || condition == 7 || condition == 10) {
                                datacltTmp = (from ls in context.Customers
                                           where (ls.Name.ToUpper() != "DEFAULT")
                                           select ls)
                                            .OrderBy(a => a.Name)
                                            .ToList();
                            } 
                            if (condition==4 || condition == 8) {
                                datacltTmp = (from ls in context.Customers
                                              where (ls.Name.ToUpper() != "DEFAULT" && ls.Sex.SexCode.ToUpper()=="F")
                                              select ls)
                                                .OrderBy(a => a.Name)
                                                .ToList();
                            } //WomensDay


                            if (condition==9) {
                                datacltTmp = (from ls in context.Customers
                                              where (ls.Name.ToUpper() != "DEFAULT" && ls.Sex.SexCode.ToUpper() == "M")
                                              select ls)
                                                   .OrderBy(a => a.Name)
                                                   .ToList();
                            }  //FatherDay

                            foreach (Customer rdvCust in datacltTmp)
                            {
                               
                                if (rdvCust.Name.ToUpper() != "DEFAULT")
                                {
                                    //verification de l'unicite

                                    ExtractSMS checkextract = context.ExtractSMSs.Where(e => e.AlertDescrip == AlertDescrip.Trim() && e.Condition == FinalCondition.Trim() &&
                                    e.CustomerID == rdvCust.GlobalPersonID && e.SendSMSDate == SendSMSDate && e.SaleDeliveryDate == SendSMSDate && e.TypeAlert == TypeAlert.Trim()
                                    ).FirstOrDefault();
                                    if (checkextract == null)
                                    {

                                        realDataTmp = new ExtractSMS
                                        {
                                            CustomerID = rdvCust.GlobalPersonID,
                                            CustomerName = rdvCust.Name.Trim(),
                                            CustomerPhone = rdvCust.Adress.AdressPhoneNumber,
                                            CustomerQuater = rdvCust.Adress.Quarter.QuarterCode,
                                            SaleDeliveryDate = serverDate,
                                            isSmsSent = false,
                                            AlertDescrip = AlertDescrip,
                                            TypeAlert = TypeAlert,
                                            Condition = FinalCondition,
                                            SendSMSDate = SendSMSDate
                                        };
                                        //validation
                                        context.ExtractSMSs.Add(realDataTmp);
                                        //context.SaveChanges();
                                    }
                                }

                            }
                            break;
                        case "GENERAL":
                            List<Sale> dataTmp = new List<Sale>();
                            if (condition > 0)
                            {

                                dataTmp = (from ls in context.Sales
                                           where ((DbFunctions.DiffMonths(ls.SaleDate, serverDate)) == condition /*&& (DbFunctions.DiffMonths(ls.SaleDate, serverDate)) <= condition*/)
                                           select ls)
                                            .OrderBy(a => a.SaleDeliveryDate)
                                            .ToList();
                            }


                            foreach (Sale p in dataTmp)
                            {
                                if (p.CustomerID != null)
                                {
                                    Customer rdvCust = context.Customers.Find(p.CustomerID.Value);
                                    if (rdvCust.Name.ToUpper() != "DEFAULT")
                                    {
                                        //verification de l'unicite
                                        ExtractSMS checkextract = context.ExtractSMSs.Where(e => e.AlertDescrip == AlertDescrip.Trim() && e.Condition == FinalCondition.Trim() &&
                                        e.CustomerID == rdvCust.GlobalPersonID && e.SendSMSDate == SendSMSDate && e.SaleDeliveryDate == p.SaleDeliveryDate && e.TypeAlert == TypeAlert.Trim()
                                        ).FirstOrDefault();
                                        if (checkextract == null)
                                        {

                                            realDataTmp = new ExtractSMS
                                            {
                                                CustomerID = rdvCust.GlobalPersonID,
                                                CustomerName = rdvCust.Name.Trim(),
                                                CustomerPhone = rdvCust.AdressPhoneNumber,
                                                CustomerQuater = rdvCust.Adress.Quarter.QuarterCode,
                                                SaleDeliveryDate = p.SaleDeliveryDate.Value,
                                                isSmsSent = false,
                                                AlertDescrip = AlertDescrip,
                                                TypeAlert = TypeAlert,
                                                Condition = FinalCondition,
                                                SendSMSDate = SendSMSDate
                                            };
                                            //validation
                                            context.ExtractSMSs.Add(realDataTmp);
                                            //context.SaveChanges();
                                        }
                                    }

                                }
                            }

                            break;
                        case "RENDEZVOUS":

                            if (condition == 0)
                            {
                                var lstLensRdv = context.RendezVous //.Join(context.Sales, r => r.SaleID, s => s.SaleID,
                                //(r, s) => new { r, s })
                                .Where(rs => /*!(rs.s.SaleDeliver) &&*/  ((DbFunctions.DiffDays(rs.DateRdv, serverDate)) <= condition))
                                .Select(rdv => new
                                {
                                    CustomerID = rdv.CustomerID,
                                    RendezVousID = rdv.RendezVousID,
                                    DateRdv = rdv.DateRdv,
                                    RaisonRdv = rdv.RaisonRdv
                                }).ToList();

                                foreach (var p in lstLensRdv)
                                {
                                    if (p.CustomerID != null)
                                    {
                                        Customer rdvCust = context.Customers.Find(p.CustomerID);
                                        if (rdvCust.Name.ToUpper() != "DEFAULT")
                                        {
                                            //verification de l'unicite
                                            ExtractSMS checkextract = context.ExtractSMSs.Where(e => e.AlertDescrip == AlertDescrip.Trim() && e.Condition == FinalCondition.Trim() &&
                                            e.CustomerID == rdvCust.GlobalPersonID && e.SendSMSDate == SendSMSDate && e.SaleDeliveryDate==p.DateRdv && e.TypeAlert == TypeAlert.Trim()
                                            ).FirstOrDefault();
                                            if (checkextract == null)
                                            {
                                                
                                                realDataTmp = new ExtractSMS
                                                {
                                                    CustomerID = rdvCust.GlobalPersonID,
                                                    CustomerName = rdvCust.Name.Trim(),
                                                    CustomerPhone = rdvCust.AdressPhoneNumber,
                                                    CustomerQuater = rdvCust.Adress.Quarter.QuarterCode,
                                                    SaleDeliveryDate = p.DateRdv,
                                                    isSmsSent = false,
                                                    AlertDescrip = AlertDescrip,
                                                    TypeAlert = TypeAlert,
                                                    Condition = FinalCondition,
                                                    SendSMSDate = SendSMSDate
                                                };
                                                //validation
                                                context.ExtractSMSs.Add(realDataTmp);
                                                //context.SaveChanges();
                                            }
                                        }

                                    }
                                }
                        }
                        break;
                        case "SPECIALORDER":
                            List<CumulSaleAndBill> sodataTmp = new List<CumulSaleAndBill>();
                            
                            sodataTmp = (from ls in context.CumulSaleAndBills
                                        where (!ls.IsCustomerRceive && ls.IsProductReveive)
                                        select ls)
                                        .OrderBy(a => a.SaleDate)
                                        .ToList();
                            
                            
                            foreach (CumulSaleAndBill p in sodataTmp)
                            {
                                if (p.CustomerID != null)
                                {
                                    Customer rdvCust = context.Customers.Find(p.CustomerID.Value);
                                    if (rdvCust.Name.ToUpper() != "DEFAULT")
                                    {
                                        //verification de l'unicite
                                        ExtractSMS checkextract = context.ExtractSMSs.Where(e => e.AlertDescrip == AlertDescrip.Trim() && e.Condition == FinalCondition.Trim() &&
                                        e.CustomerID == rdvCust.GlobalPersonID && e.SendSMSDate == SendSMSDate && e.SaleDeliveryDate == p.EffectiveReceiveDate && e.TypeAlert == TypeAlert.Trim()
                                        ).FirstOrDefault();
                                        if (checkextract == null)
                                        {

                                            realDataTmp = new ExtractSMS
                                            {
                                                CustomerID = rdvCust.GlobalPersonID,
                                                CustomerName = rdvCust.Name.Trim(),
                                                CustomerPhone = rdvCust.AdressPhoneNumber,
                                                CustomerQuater = rdvCust.Adress.Quarter.QuarterCode,
                                                SaleDeliveryDate = p.EffectiveReceiveDate.Value,
                                                isSmsSent = false,
                                                AlertDescrip = AlertDescrip,
                                                TypeAlert = TypeAlert,
                                                Condition = FinalCondition,
                                                SendSMSDate = SendSMSDate
                                            };
                                            //validation
                                            context.ExtractSMSs.Add(realDataTmp);
                                            //context.SaveChanges();
                                        }
                                    }

                                }
                            }
                            break;
                        default:
                            throw new Exception("Cannot extract data !!! Wrong information Contact your Administrator");
                    }

                    context.SaveChanges();

                    //EcritureSneack
                    IMouchar opSneak = new MoucharRepository(context);
                    bool res = opSneak.InsertOperation(userConnet, "SUCCESS", "EXTRACT DATA FOR " + TypeAlert + " FOR  " + FinalCondition, "AddExtractSMS", serverDate, CurrentBranchID);
                    if (!res)
                    {
                        throw new Exception("Une erreur s'est produite lors de la journalisation ");
                    }
                    FinalDataTmp = context.ExtractSMSs.Where(e=> (e.AlertDescrip == AlertDescrip.Trim() && e.Condition == FinalCondition.Trim() &&
                                        e.SendSMSDate == SendSMSDate && e.TypeAlert == TypeAlert.Trim()) && (!e.isSmsSent) && (!e.isDelete)).ToList();
                //    ts.Complete();

                   
                //}
            }
            catch (Exception e)
            {
                
                throw new Exception(e.Message);
            }
            return FinalDataTmp;
        }

        public bool UpdateExtractSMS(int ExtractSMSID, string Telephone, int userConnet, DateTime serverDate, int CurrentBranchID)
        {
            bool res = false;
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    //recuperation de la ligne a modifier
                    ExtractSMS updatedPhone = context.ExtractSMSs.Find(ExtractSMSID);
                    if (updatedPhone != null)
                    {
                        updatedPhone.CustomerPhone = Telephone;
                        context.SaveChanges();

                        //UPDATE DU NUMERO DS LA TABLE DES CUSTOMER
                        Customer Upcustomer = context.Customers.Find(updatedPhone.CustomerID);
                        Upcustomer.Adress.AdressPhoneNumber = Telephone;
                        context.SaveChanges();
                        //EcritureSneack
                        IMouchar opSneak = new MoucharRepository(context);
                        res = opSneak.InsertOperation(userConnet, "SUCCESS", "UPDATE PHONE " + Telephone + " FOR CUSTOMER " + updatedPhone.CustomerName, "UpdateExtractSMS", serverDate, CurrentBranchID);
                        if (!res)
                        {
                            throw new Exception("Une erreur s'est produite lors de la journalisation ");
                        }
                    }
                    else
                    {
                        res = false;
                        throw new Exception("No data to Update!!!");
                    }
                    
                    res = true;
                    ts.Complete();
                    
                    return res;
                }
            }
            catch (Exception e)
            {
                res = false;
                throw new Exception(e.Message);
            }
        }

        public bool DeleteExtractSMS(int ExtractSMSID, int userConnet, DateTime serverDate, int CurrentBranchID)
        {
            bool res = false;
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    //recuperation de la ligne a modifier
                    ExtractSMS updatedPhone = context.ExtractSMSs.Find(ExtractSMSID);
                    if (updatedPhone != null)
                    {
                        updatedPhone.isDelete = true;
                        context.SaveChanges();

                        //EcritureSneack
                        IMouchar opSneak = new MoucharRepository(context);
                        res = opSneak.InsertOperation(userConnet, "SUCCESS", "Delete ID " + ExtractSMSID + " FOR CUSTOMER " + updatedPhone.CustomerName, "DeleteExtractSMS", serverDate, CurrentBranchID);
                        if (!res)
                        {
                            throw new Exception("Une erreur s'est produite lors de la journalisation ");
                        }
                    }
                    else
                    {
                        res = false;
                        throw new Exception("No data to Delete!!!");
                    }

                    res = true;
                    ts.Complete();

                    return res;
                }
            }

            catch (Exception e)
            {
                res = false;
                throw new Exception(e.Message);
            }
        }
    }
}
