

using FatSod.Supply.Abstracts;
using FatSod.Supply.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FastSod.Utilities.Util;
using AutoMapper;
using FatSod.DataContext.Concrete;
using System.Transactions;
using FatSod.Security.Abstracts;
using System.Data.Entity;
using FatSod.Ressources;
using FatSod.DataContext.Initializer;
using FatSod.Security.Entities;

namespace FatSod.DataContext.Repositories
{
    /// <summary>
    /// 
    /// </summary>
    public class PrescriptionRepository : RepositorySupply<PrescriptionLStep>, IPrescriptionLStep
    {
        /// <summary>
        /// 
        /// </summary>
        public PrescriptionRepository()
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctx"></param>
        public PrescriptionRepository(EFDbContext ctx)
            : base(ctx)
        {
            this.context = ctx;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="consultOldPrescr"></param>
        /// <param name="UserConect"></param>
        /// <param name="BranchID"></param>
        /// <returns></returns>
        public ConsultOldPrescr SaveConsultoldpres(ConsultOldPrescr consultOldPrescr, int UserConect, int BranchID)
        {
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {

                    if (consultOldPrescr.DateOfBirth!=null)
                    {
                        //update date of birth for customer
                        Customer existingcusto = context.Customers.Find(consultOldPrescr.CustomerNumber);
                        if (existingcusto!=null)
                        {
                            existingcusto.DateOfBirth = consultOldPrescr.DateOfBirth;
                            context.SaveChanges();
                        }
                    }
                    ConsultOldPrescr consultOldPrescrToSave = context.ConsultOldPrescrs.Add(consultOldPrescr);
                    context.SaveChanges();
                   

                    //EcritureSneack
                    IMouchar opSneak = new MoucharRepository(context);
                    bool res = opSneak.InsertOperation(UserConect, "SUCCESS", "consultOldPrescr-ID " + consultOldPrescr.ConsultOldPrescrID + " FOR CUSTOMER " + consultOldPrescr.CustomerNumber, "SaveConsultoldpres", consultOldPrescr.DateConsultOldPres, BranchID);
                    if (!res)
                    {
                        throw new Exception("Une erreur s'est produite lors de la journalisation ");
                    }

                    ts.Complete();
                }
            }
            catch (Exception e)
            {
                //If an errors occurs, we cancel all changes in database
                //transaction.Rollback();
                //EcritureSneack
                IMouchar opSneak = new MoucharRepository(context);
                bool res = opSneak.InsertOperation(UserConect, "ERROR", "consultOldPrescr-ID " + consultOldPrescr.ConsultOldPrescrID, "SaveConsultoldpres", consultOldPrescr.DateConsultOldPres, BranchID);
                if (!res)
                {
                    throw new Exception("Une erreur s'est produite lors de la journalisation ");
                }
                throw new Exception("Une erreur s'est produite lors de la Prescription : " + "e.Message = " + e.Message);
            }
            return consultOldPrescr;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="newDilation"></param>
        /// /// <param name="consultDilPresc"></param>
        /// <param name="UserConect"></param>
        /// <param name="BranchID"></param>
        /// <returns></returns>
        public ConsultDilatation SaveConsultDilatation(ConsultDilPresc consultDilPresc, ConsultDilatation newDilation, int UserConect, int BranchID)
        {
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {

                    ConsultDilPresc ConsultDilPresctoSave = context.ConsultDilPrescs.Add(consultDilPresc);
                    context.SaveChanges();
                    newDilation.ConsultDilPrescID = ConsultDilPresctoSave.ConsultDilPrescID;
                    ConsultDilatation ConsultDilatationToSave = context.ConsultDilatations.Add(newDilation);
                    context.SaveChanges();


                    //EcritureSneack
                    IMouchar opSneak = new MoucharRepository(context);
                    bool res = opSneak.InsertOperation(UserConect, "SUCCESS", "ConsultDilatation-ID " + newDilation.ConsultDilPrescID + " FOR CUSTOMER " + ConsultDilatationToSave.ConsultDilPresc.CustomerNumber, "SaveConsultDilatation", newDilation.DateDilation, BranchID);
                    if (!res)
                    {
                        throw new Exception("Une erreur s'est produite lors de la journalisation ");
                    }

                    ts.Complete();
                }
            }
            catch (Exception e)
            {
                //If an errors occurs, we cancel all changes in database
                //transaction.Rollback();
                //EcritureSneack
                IMouchar opSneak = new MoucharRepository(context);
                bool res = opSneak.InsertOperation(UserConect, "ERROR", "ConsultDilatation-ID " + newDilation.ConsultDilPrescID, "SaveConsultDilatation", newDilation.DateDilation, BranchID);
                if (!res)
                {
                    throw new Exception("Une erreur s'est produite lors de la journalisation ");
                }
                throw new Exception("Une erreur s'est produite lors de la Prescription : " + "e.Message = " + e.Message);
            }
            return newDilation;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="existDilation"></param>
        /// <param name="UserConect"></param>
        /// <param name="BranchID"></param>
        /// <returns></returns>
        public ConsultDilatation UpdateConsultDilatation(ConsultDilatation existDilation, int UserConect, int BranchID)
        {
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    ConsultDilatation existingConsultDilatation = context.ConsultDilatations.Find(existDilation.ConsultDilPrescID);
                    if (existingConsultDilatation!=null)
                    {
                        
                            existingConsultDilatation.CodeDilation = existDilation.CodeDilation;

                            context.SaveChanges();

                            //EcritureSneack
                            IMouchar opSneak = new MoucharRepository(context);
                            bool res = opSneak.InsertOperation(UserConect, "SUCCESS", "ConsultDilatation-ID " + existDilation.ConsultDilPrescID + " FOR CUSTOMER " + existingConsultDilatation.ConsultDilPresc.CustomerNumber, "UpdateConsultDilatation", existDilation.DateDilation, BranchID);
                            if (!res)
                            {
                                throw new Exception("Une erreur s'est produite lors de la journalisation ");
                            }
                        }
                    ts.Complete();
                }
            }
            catch (Exception e)
            {
                //EcritureSneack
                IMouchar opSneak = new MoucharRepository(context);
                bool res = opSneak.InsertOperation(UserConect, "ERROR", "consultOldPrescr-ID " + existDilation.ConsultDilPrescID, "UpdateConsultDilatation", existDilation.DateDilation, BranchID);
                if (!res)
                {
                    throw new Exception("Une erreur s'est produite lors de la journalisation ");
                }
                throw new Exception("Une erreur s'est produite lors de la Prescription : " + "e.Message = " + e.Message);
            }
            return existDilation;
        }

        public void DeleteConsultDilatation(int ConsultDilPrescID, int UserConect, int BranchID, DateTime DateOperation)
        {
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    ConsultDilatation existingConsultDilatation = context.ConsultDilatations.Find(ConsultDilPrescID);
                    if (existingConsultDilatation != null) { 
                        context.ConsultDilatations.Remove(existingConsultDilatation);
                        context.SaveChanges();
                        //EcritureSneack
                        IMouchar opSneak = new MoucharRepository(context);
                        bool res = opSneak.InsertOperation(UserConect, "SUCCESS", "ConsultDilatation-ID " + ConsultDilPrescID, "DeleteConsultDilatation", DateOperation, BranchID);
                        if (!res)
                        {
                            throw new Exception("Une erreur s'est produite lors de la journalisation ");
                        }
                    }
                    ts.Complete();
                }
            }
            catch (Exception e)
            {
                //EcritureSneack
                IMouchar opSneak = new MoucharRepository(context);
                bool res = opSneak.InsertOperation(UserConect, "ERROR", "consultOldPrescr-ID " + ConsultDilPrescID, "DeleteConsultDilatation", DateOperation, BranchID);
                if (!res)
                {
                    throw new Exception("Une erreur s'est produite lors de la journalisation ");
                }
                throw new Exception("Une erreur s'est produite lors de la suppression de la Prescription : " + "e.Message = " + e.Message);
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="newLensPrescrip"></param>
        /// <param name="consultDilPresc"></param>
        /// <param name="UserConect"></param>
        /// <param name="BranchID"></param>
        /// <returns></returns>
        public ConsultLensPrescription SaveConsultLensPrescription(ConsultDilPresc consultDilPresc, ConsultLensPrescription newLensPrescrip, int UserConect, int BranchID)
        {
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {

                    ConsultDilPresc ConsultDilPresctoSave = context.ConsultDilPrescs.Add(consultDilPresc);
                    context.SaveChanges();
                    newLensPrescrip.ConsultDilPrescID = ConsultDilPresctoSave.ConsultDilPrescID;

                    ConsultLensPrescription ConsultLensPrescriptionToSave = context.ConsultLensPrescriptions.Add(newLensPrescrip);
                    context.SaveChanges();


                    //EcritureSneack
                    IMouchar opSneak = new MoucharRepository(context);
                    bool res = opSneak.InsertOperation(UserConect, "SUCCESS", "ConsultLensPrescription-ID " + newLensPrescrip.ConsultDilPrescID + " FOR CUSTOMER " + ConsultLensPrescriptionToSave.ConsultDilPresc.CustomerNumber, "SaveConsultLensPrescription", newLensPrescrip.DatePrescription, BranchID);
                    if (!res)
                    {
                        throw new Exception("Une erreur s'est produite lors de la journalisation ");
                    }

                    ts.Complete();
                }
            }
            catch (Exception e)
            {
                //If an errors occurs, we cancel all changes in database
                //transaction.Rollback();
                //EcritureSneack
                IMouchar opSneak = new MoucharRepository(context);
                bool res = opSneak.InsertOperation(UserConect, "ERROR", "ConsultLensPrescription-ID " + newLensPrescrip.ConsultDilPrescID, "SaveConsultLensPrescription", newLensPrescrip.DatePrescription, BranchID);
                if (!res)
                {
                    throw new Exception("Une erreur s'est produite lors de la journalisation ");
                }
                throw new Exception("Une erreur s'est produite lors de la Prescription : " + "e.Message = " + e.Message);
            }
            return newLensPrescrip;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="newLensPrescrip"></param>
        /// <param name="UserConect"></param>
        /// <param name="BranchID"></param>
        /// <returns></returns>
        public ConsultLensPrescription UpdateConsultLensPrescription(ConsultLensPrescription newLensPrescrip, int UserConect, int BranchID)
        {
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    ConsultLensPrescription ConsultLensPrescriptionToSave = new ConsultLensPrescription();

                    ConsultLensPrescription ExistConsultLensPrescription = context.ConsultLensPrescriptions.Where(c => c.ConsultDilPrescID == newLensPrescrip.ConsultDilPrescID).FirstOrDefault();

                    if (ExistConsultLensPrescription==null)
                    {
                        ConsultLensPrescriptionToSave = context.ConsultLensPrescriptions.Add(newLensPrescrip);
                    }
                    else
                    {
                        // ExistConsultLensPrescription.ConsultDilPrescID = ExistConsultLensPrescription.ConsultDilPrescID;
                        ExistConsultLensPrescription.CategoryID = newLensPrescrip.CategoryID;
                        ExistConsultLensPrescription.SupplyingName = newLensPrescrip.SupplyingName;
                        ExistConsultLensPrescription.ConsultByID = newLensPrescrip.ConsultByID;
                        ExistConsultLensPrescription.DatePrescription = newLensPrescrip.DatePrescription;
                        ExistConsultLensPrescription.HeureLensPrescription = newLensPrescrip.HeureLensPrescription;
                        ExistConsultLensPrescription.LAddition = newLensPrescrip.LAddition;
                        ExistConsultLensPrescription.LAxis = newLensPrescrip.LAxis;
                        ExistConsultLensPrescription.LCylValue = newLensPrescrip.LCylValue;
                        ExistConsultLensPrescription.LIndex = newLensPrescrip.LIndex;
                        ExistConsultLensPrescription.LSphValue = newLensPrescrip.LSphValue;
                        ExistConsultLensPrescription.RAddition = newLensPrescrip.RAddition;
                        ExistConsultLensPrescription.RAxis = newLensPrescrip.RAxis;
                        ExistConsultLensPrescription.RCylValue = newLensPrescrip.RCylValue;
                        ExistConsultLensPrescription.RIndex = newLensPrescrip.RIndex;
                        ExistConsultLensPrescription.RSphValue = newLensPrescrip.RSphValue;
                        /*
                        ConsultLensPrescriptionToSave.ConsultDilPrescID = ExistConsultLensPrescription.ConsultDilPrescID;
                        ConsultLensPrescriptionToSave.CategoryID = newLensPrescrip.CategoryID;
                        ConsultLensPrescriptionToSave.SupplyingName = newLensPrescrip.SupplyingName;
                        ConsultLensPrescriptionToSave.ConsultByID = newLensPrescrip.ConsultByID;
                        ConsultLensPrescriptionToSave.DatePrescription = newLensPrescrip.DatePrescription;
                        ConsultLensPrescriptionToSave.HeureLensPrescription = newLensPrescrip.HeureLensPrescription;
                        ConsultLensPrescriptionToSave.LAddition = newLensPrescrip.LAddition;
                        ConsultLensPrescriptionToSave.LAxis = newLensPrescrip.LAxis;
                        ConsultLensPrescriptionToSave.LCylValue = newLensPrescrip.LCylValue;
                        ConsultLensPrescriptionToSave.LIndex = newLensPrescrip.LIndex;
                        ConsultLensPrescriptionToSave.LSphValue = newLensPrescrip.LSphValue;
                        ConsultLensPrescriptionToSave.RAddition = newLensPrescrip.RAddition;
                        ConsultLensPrescriptionToSave.RAxis = newLensPrescrip.RAxis;
                        ConsultLensPrescriptionToSave.RCylValue = newLensPrescrip.RCylValue;
                        ConsultLensPrescriptionToSave.RIndex = newLensPrescrip.RIndex;
                        ConsultLensPrescriptionToSave.RSphValue = newLensPrescrip.RSphValue;*/
                    }
                    
                    context.SaveChanges();


                    //EcritureSneack
                    IMouchar opSneak = new MoucharRepository(context);
                    bool res = opSneak.InsertOperation(UserConect, "SUCCESS", "ConsultLensPrescription-ID " + newLensPrescrip.ConsultDilPrescID + " FOR CATEGORY " + ConsultLensPrescriptionToSave.SupplyingName, "UpdateConsultLensPrescription", newLensPrescrip.DatePrescription, BranchID);
                    if (!res)
                    {
                        throw new Exception("Une erreur s'est produite lors de la journalisation ");
                    }

                    ts.Complete();
                }
            }
            catch (Exception e)
            {
                //If an errors occurs, we cancel all changes in database
                //transaction.Rollback();
                //EcritureSneack
                IMouchar opSneak = new MoucharRepository(context);
                bool res = opSneak.InsertOperation(UserConect, "ERROR", "ConsultLensPrescription-ID " + newLensPrescrip.ConsultDilPrescID, "UpdateConsultLensPrescription", newLensPrescrip.DatePrescription, BranchID);
                if (!res)
                {
                    throw new Exception("Une erreur s'est produite lors de la journalisation ");
                }
                throw new Exception("Une erreur s'est produite lors de la Prescription : " + "e.Message = " + e.Message);
            }
            return newLensPrescrip;
        }

        public void DeleteConsultLensPrescription(int ConsultDilPrescID, int UserConect, int BranchID, DateTime DateOperation)
        {
            try
            {
                
                using (TransactionScope ts = new TransactionScope())
                {
                    ConsultLensPrescription ExistConsultLensPrescription = context.ConsultLensPrescriptions.Where(c => c.ConsultDilPrescID == ConsultDilPrescID).FirstOrDefault();
                    if (ExistConsultLensPrescription != null) { 
                        context.ConsultLensPrescriptions.Remove(ExistConsultLensPrescription);
                        context.SaveChanges();
                        //EcritureSneack
                        IMouchar opSneak = new MoucharRepository(context);
                        bool res = opSneak.InsertOperation(UserConect, "SUCCESS", "ConsultLensPrescription-ID " + ConsultDilPrescID, "DeleteConsultDilatation", DateOperation, BranchID);
                        if (!res)
                        {
                            throw new Exception("Une erreur s'est produite lors de la journalisation ");
                        }
                    }
                    ts.Complete();
                }

            }
            catch (Exception e)
            {
                //If an errors occurs, we cancel all changes in database
                //transaction.Rollback();
                //EcritureSneack
                IMouchar opSneak = new MoucharRepository(context);
                bool res = opSneak.InsertOperation(UserConect, "ERROR", "ConsultLensPrescription-ID " + ConsultDilPrescID, "DeleteConsultLensPrescription", DateOperation, BranchID);
                if (!res)
                {
                    throw new Exception("Une erreur s'est produite lors de la journalisation ");
                }
                throw new Exception("Une erreur s'est produite lors de la Prescription : " + "e.Message = " + e.Message);
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="prescription"></param>
        /// <param name="HeureVente"></param>
        /// <param name="UserConect"></param>
        /// <param name="BranchID"></param>
        /// <param name="isPrescription"></param>
        /// <returns></returns>
        public PrescriptionLStep SaveChanges(PrescriptionLStep prescription, bool isPrescription, String HeureVente, int UserConect,int BranchID)
        {
            //Begin of transaction
           
                try
                {
                    using (TransactionScope ts = new TransactionScope())
                    {
                        //ajout de lheure de la vente
                        string[] tisys = HeureVente.Split(new char[] { ':' });
                        DateTime date = prescription.DatePrescriptionLStep;
                        date = date.AddHours(Convert.ToDouble(tisys[0]));
                        date = date.AddMinutes(Convert.ToDouble(tisys[1]));
                        date = date.AddSeconds(Convert.ToDouble(tisys[2]));
                        //we create a new command
                        prescription.DateHeurePrescriptionLStep = date;

                        prescription = PersistPrescription(prescription, isPrescription, UserConect, BranchID);
                        ts.Complete();
                    }
                }
                catch (Exception e)
                {
                    //If an errors occurs, we cancel all changes in database
                    //transaction.Rollback();
                    //EcritureSneack
                    IMouchar opSneak = new MoucharRepository(context);
                    bool res = opSneak.InsertOperation(UserConect, "ERROR", "Prescription-ID " + prescription.ConsultationID , "SaveChanges", prescription.DatePrescriptionLStep, BranchID);
                    if (!res)
                    {
                        throw new Exception("Une erreur s'est produite lors de la journalisation ");
                    }
                    throw new Exception("Une erreur s'est produite lors de la Prescription : " + "e.Message = " + e.Message );
                }
                return prescription;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="prescriptionLStep"></param>
        /// <param name="UserConect"></param>
        /// <param name="BranchID"></param>
        ///<param name="isPrescription"></param>
        /// <returns></returns>
        public PrescriptionLStep PersistPrescription(PrescriptionLStep prescriptionLStep, bool isPrescription,  int UserConect,int BranchID)
        {

            bool res = false;
           
            Consultation consultEntity = new Consultation();
            if (prescriptionLStep.ConsultationID == 0)
            {
                throw new Exception("Erreur : Please you must save the Customer on consultation Before Proceed ");
            }
            else
            {
                consultEntity= context.Consultations.Find(prescriptionLStep.ConsultationID);
                if (consultEntity == null)
                {
                    throw new Exception("Erreur : Please you must create the Customer Before Proceed ");
                }
                ////update de la prescription pr le client
                consultEntity.MedecintTraitant = prescriptionLStep.MedecinTraitant;
                consultEntity.isPrescritionValidate = (isPrescription) ? true : false;

                this.context.Consultations.Attach(consultEntity);
                context.Entry(consultEntity).State = EntityState.Modified;
                this.context.SaveChanges();

                //this.Update(consultEntity, consultEntity.ConsultationID);
                //context.SaveChanges();
            }
            

            prescriptionLStep.ConsultByID = UserConect;
           

            //persistance

            PrescriptionLStep PrescriptionToSave = context.PrescriptionLSteps.Add(prescriptionLStep);
            context.SaveChanges();
            

            
            //EcritureSneack
            IMouchar opSneak = new MoucharRepository(context);
            res = opSneak.InsertOperation(UserConect, "SUCCESS", "Prescription-ID " + prescriptionLStep.ConsultationID + " FOR CUSTOMER " + consultEntity.CustomerID, "PersistPrescription", prescriptionLStep.DatePrescriptionLStep, BranchID);
            if (!res)
            {
                throw new Exception("Une erreur s'est produite lors de la journalisation ");
            }
            
            return prescriptionLStep;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="consultPersonalMedHisto"></param>
        /// <param name="allATCDPerso"></param>
        /// <param name="allATCDFam"></param>
        /// <param name="UserConect"></param>
        /// <param name="BranchID"></param>
        /// <returns></returns>
        public ConsultPersonalMedHisto SaveChangesConsultPersonalMedHisto(ConsultPersonalMedHisto consultPersonalMedHisto, List<int> allATCDPerso, List<int> allATCDFam, int UserConect, int BranchID)
        {

            try
            {
                using (TransactionScope ts = new TransactionScope())
                {

                    //suppression de l'existant
                    List<ATCDPersonnel> lstAtcdPerso = context.ATCDPersonnels.Where(c => c.CustomerID == consultPersonalMedHisto.CustomerNumber).ToList();
                    context.ATCDPersonnels.RemoveRange(lstAtcdPerso);

                    List<ATCDFamilial> lstAtcdFam = context.ATCDFamiliaux.Where(c => c.CustomerID == consultPersonalMedHisto.CustomerNumber).ToList();
                    context.ATCDFamiliaux.RemoveRange(lstAtcdFam);

                    context.SaveChanges();

                    //int ATCDPersonnelID = 0;
                    //ajout antecedent Perso

                    foreach (var atcd in allATCDPerso)
                    {
                        ATCDPersonnel newatcdPerso = new ATCDPersonnel
                        {
                            ATCDID = atcd,
                            CustomerID = consultPersonalMedHisto.CustomerNumber,
                            Remarques = consultPersonalMedHisto.ATCDPersoAutre
                        };
                        context.ATCDPersonnels.Add(newatcdPerso);
                        context.SaveChanges();
                        //consultPersonalMedHisto.ATCDPersonnelID = newatcdPerso.ATCDPersonnelID;
                    }

                    //int ATCDFamilialID = 0;
                    //ajout antecedent familiaux
                    foreach (var atcd in allATCDFam)
                    {
                        ATCDFamilial newatcdFam = new ATCDFamilial
                        {
                            ATCDID = atcd,
                            CustomerID = consultPersonalMedHisto.CustomerNumber,
                            Remarques = consultPersonalMedHisto.ATCDFamAutre
                        };
                        context.ATCDFamiliaux.Add(newatcdFam);
                        context.SaveChanges();
                        //consultPersonalMedHisto.ATCDFamilialID = newatcdFam.ATCDFamilialID;
                    }
                    
                    ConsultPersonalMedHisto cconsultPersonalMedHistoToSave = context.ConsultPersonalMedHistos.Add(consultPersonalMedHisto);
                    context.SaveChanges();


                    //EcritureSneack
                    IMouchar opSneak = new MoucharRepository(context);
                    bool res = opSneak.InsertOperation(UserConect, "SUCCESS", "consultPersonalMedHisto-ID " + consultPersonalMedHisto.ConsultPersonalMedHistoID + " FOR CUSTOMER " + consultPersonalMedHisto.CustomerNumber, "SaveChangesConsultPersonalMedHisto", consultPersonalMedHisto.DateConsultPersonalMedHisto, BranchID);
                    if (!res)
                    {
                        throw new Exception("Une erreur s'est produite lors de la journalisation ");
                    }

                    ts.Complete();
                }
            }
            catch (Exception e)
            {
                //If an errors occurs, we cancel all changes in database
                IMouchar opSneak = new MoucharRepository(context);
                bool res = opSneak.InsertOperation(UserConect, "ERROR", "consultPersonalMedHisto-ID " + consultPersonalMedHisto.ConsultPersonalMedHistoID, "SaveChangesConsultPersonalMedHisto", consultPersonalMedHisto.DateConsultPersonalMedHisto, BranchID);
                if (!res)
                {
                    throw new Exception("Une erreur s'est produite lors de la journalisation ");
                }
                throw new Exception("Une erreur s'est produite lors de la Prescription ATCD : " + "e.Message = " + e.Message);
            }
            return consultPersonalMedHisto;

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="consultPersonalMedHisto"></param>
        /// <param name="allATCDPerso"></param>
        /// <param name="allATCDFam"></param>
        /// <param name="UserConect"></param>
        /// <param name="BranchID"></param>
        /// <returns></returns>
        public ConsultPersonalMedHisto UpdateChangesconsultPersonalMedHisto(ConsultPersonalMedHisto consultPersonalMedHisto, List<int> allATCDPerso, List<int> allATCDFam, int UserConect, int BranchID)
        {

            try
            {
                using (TransactionScope ts = new TransactionScope())
                {

                    ConsultPersonalMedHisto existConsultPersonalMedHisto = context.ConsultPersonalMedHistos.Find(consultPersonalMedHisto.ConsultPersonalMedHistoID);

                    //suppression de l'existant
                    List<ATCDPersonnel> lstAtcdPerso = context.ATCDPersonnels.Where(c => c.CustomerID == consultPersonalMedHisto.CustomerNumber).ToList();
                    context.ATCDPersonnels.RemoveRange(lstAtcdPerso);

                    List<ATCDFamilial> lstAtcdFam = context.ATCDFamiliaux.Where(c => c.CustomerID == consultPersonalMedHisto.CustomerNumber).ToList();
                    context.ATCDFamiliaux.RemoveRange(lstAtcdFam);

                    context.SaveChanges();

                    //int ATCDPersonnelID = 0;
                    //ajout antecedent Perso

                    foreach (var atcd in allATCDPerso)
                    {
                        ATCDPersonnel newatcdPerso = new ATCDPersonnel
                        {
                            ATCDID = atcd,
                            CustomerID = consultPersonalMedHisto.CustomerNumber,
                            Remarques = consultPersonalMedHisto.ATCDPersoAutre
                        };
                        context.ATCDPersonnels.Add(newatcdPerso);
                        context.SaveChanges();
                        //existConsultPersonalMedHisto.ATCDPersonnelID = newatcdPerso.ATCDPersonnelID;
                    }

                    //int ATCDFamilialID = 0;
                    //ajout antecedent familiaux
                    foreach (var atcd in allATCDFam)
                    {
                        ATCDFamilial newatcdFam = new ATCDFamilial
                        {
                            ATCDID = atcd,
                            CustomerID = consultPersonalMedHisto.CustomerNumber,
                            Remarques = consultPersonalMedHisto.ATCDFamAutre
                        };
                        context.ATCDFamiliaux.Add(newatcdFam);
                        context.SaveChanges();
                        //existConsultPersonalMedHisto.ATCDFamilialID = newatcdFam.ATCDFamilialID;
                    }


                    context.SaveChanges();


                    //EcritureSneack
                    IMouchar opSneak = new MoucharRepository(context);
                    bool res = opSneak.InsertOperation(UserConect, "SUCCESS", "consultPersonalMedHisto-ID " + consultPersonalMedHisto.ConsultPersonalMedHistoID + " FOR CUSTOMER " + consultPersonalMedHisto.CustomerNumber, "UpdateChangesconsultPersonalMedHisto", consultPersonalMedHisto.DateConsultPersonalMedHisto, BranchID);
                    if (!res)
                    {
                        throw new Exception("Une erreur s'est produite lors de la journalisation ");
                    }

                    ts.Complete();
                }
            }
            catch (Exception e)
            {
                //If an errors occurs, we cancel all changes in database
                IMouchar opSneak = new MoucharRepository(context);
                bool res = opSneak.InsertOperation(UserConect, "ERROR", "consultPersonalMedHisto-ID " + consultPersonalMedHisto.ConsultPersonalMedHistoID, "UpdateChangesconsultPersonalMedHisto", consultPersonalMedHisto.DateConsultPersonalMedHisto, BranchID);
                if (!res)
                {
                    throw new Exception("Une erreur s'est produite lors de la journalisation ");
                }
                throw new Exception("Une erreur s'est produite lors de la Prescription ATCD : " + "e.Message = " + e.Message);
            }
            return consultPersonalMedHisto;

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="consultOldPrescr"></param>
        /// <param name="UserConect"></param>
        /// <param name="BranchID"></param>
        /// <returns></returns>
        public ConsultOldPrescr UpdateConsultoldpres(ConsultOldPrescr consultOldPrescr, int UserConect, int BranchID)
        {
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    ConsultOldPrescr existingconsultOldPrescr = context.ConsultOldPrescrs.Find(consultOldPrescr.ConsultOldPrescrID);
                    //update de certains elements

                    existingconsultOldPrescr.ConsultByID = UserConect;
                    existingconsultOldPrescr.PlaintePatient = consultOldPrescr.PlaintePatient;
                    existingconsultOldPrescr.OldPlaintePatient = consultOldPrescr.OldPlaintePatient;
                    existingconsultOldPrescr.DateConsultOldPres = consultOldPrescr.DateConsultOldPres;

                    //info pr le verre
                    //left site
                    existingconsultOldPrescr.LAxis = consultOldPrescr.LAxis;
                    existingconsultOldPrescr.LAddition = consultOldPrescr.LAddition;
                    existingconsultOldPrescr.LIndex = consultOldPrescr.LIndex;
                    existingconsultOldPrescr.LCylValue = consultOldPrescr.LCylValue;
                    existingconsultOldPrescr.LSphValue = consultOldPrescr.LSphValue;
                    //right site
                    existingconsultOldPrescr.RAxis = consultOldPrescr.RAxis;
                    existingconsultOldPrescr.RAddition = consultOldPrescr.RAddition;
                    existingconsultOldPrescr.RIndex = consultOldPrescr.RIndex;
                    existingconsultOldPrescr.RCylValue = consultOldPrescr.RCylValue;
                    existingconsultOldPrescr.RSphValue = consultOldPrescr.RSphValue;
                    //product category
                    existingconsultOldPrescr.CategoryID = consultOldPrescr.CategoryID;

                    // les autres
                    existingconsultOldPrescr.DateDernierConsultation = consultOldPrescr.DateDernierConsultation;
                    existingconsultOldPrescr.IsDilatation = consultOldPrescr.IsDilatation;
                    existingconsultOldPrescr.IsCollyre = consultOldPrescr.IsCollyre;
                    existingconsultOldPrescr.NomCollyre = consultOldPrescr.NomCollyre;
                    existingconsultOldPrescr.HeureConsOldPres = consultOldPrescr.HeureConsOldPres;
                    existingconsultOldPrescr.CustomerNumber = consultOldPrescr.CustomerNumber;

                    // Les accuites visuelles
                    existingconsultOldPrescr.AcuiteVisuelLID = consultOldPrescr.AcuiteVisuelLID;
                    existingconsultOldPrescr.RAcuiteVisuelLID = consultOldPrescr.RAcuiteVisuelLID;
                    existingconsultOldPrescr.LAcuiteVisuelLID = consultOldPrescr.LAcuiteVisuelLID;

                    existingconsultOldPrescr.RAVLTSID = consultOldPrescr.RAVLTSID;
                    existingconsultOldPrescr.LAVLTSID = consultOldPrescr.LAVLTSID;

                    existingconsultOldPrescr.AcuiteVisuelPID = consultOldPrescr.AcuiteVisuelPID;
                    existingconsultOldPrescr.RAcuiteVisuelPID = consultOldPrescr.RAcuiteVisuelPID;
                    existingconsultOldPrescr.LAcuiteVisuelPID = consultOldPrescr.LAcuiteVisuelPID;

                    existingconsultOldPrescr.OldAcuiteVisuelLID = consultOldPrescr.OldAcuiteVisuelLID;
                    existingconsultOldPrescr.OldRAcuiteVisuelLID = consultOldPrescr.OldRAcuiteVisuelLID;
                    existingconsultOldPrescr.OldLAcuiteVisuelLID = consultOldPrescr.OldLAcuiteVisuelLID;

                    existingconsultOldPrescr.OldAcuiteVisuelPID = consultOldPrescr.OldAcuiteVisuelPID;
                    existingconsultOldPrescr.OldRAcuiteVisuelPID = consultOldPrescr.OldRAcuiteVisuelPID;
                    existingconsultOldPrescr.OldLAcuiteVisuelPID = consultOldPrescr.OldLAcuiteVisuelPID;
          
                    context.SaveChanges();

                    //EcritureSneack
                    IMouchar opSneak = new MoucharRepository(context);
                    bool res = opSneak.InsertOperation(UserConect, "SUCCESS", "consultOldPrescr-ID " + consultOldPrescr.ConsultOldPrescrID + " FOR CUSTOMER " + consultOldPrescr.CustomerNumber, "UpdateConsultoldpres", consultOldPrescr.DateConsultOldPres, BranchID);
                    if (!res)
                    {
                        throw new Exception("Une erreur s'est produite lors de la journalisation ");
                    }

                    ts.Complete();
                }
            }
            catch (Exception e)
            {
                //EcritureSneack
                IMouchar opSneak = new MoucharRepository(context);
                bool res = opSneak.InsertOperation(UserConect, "ERROR", "consultOldPrescr-ID " + consultOldPrescr.ConsultOldPrescrID, "UpdateConsultoldpres", consultOldPrescr.DateConsultOldPres, BranchID);
                if (!res)
                {
                    throw new Exception("Une erreur s'est produite lors de la journalisation ");
                }
                throw new Exception("Une erreur s'est produite lors de la Prescription : " + "e.Message = " + e.Message);
            }
            return consultOldPrescr;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="prescription"></param>
        /// <param name="HeureVente"></param>
        /// <param name="UserConect"></param>
        /// <param name="BranchID"></param>
        /// <param name="isPrescription"></param>

        /// <returns></returns>
        public PrescriptionLStep UpdateChanges(PrescriptionLStep prescription, bool isPrescription, String HeureVente, int UserConect, int BranchID)
        {
            //Begin of transaction

            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    //ajout de lheure de la vente
                    string[] tisys = HeureVente.Split(new char[] { ':' });
                    DateTime date = prescription.DatePrescriptionLStep;
                    date = date.AddHours(Convert.ToDouble(tisys[0]));
                    date = date.AddMinutes(Convert.ToDouble(tisys[1]));
                    date = date.AddSeconds(Convert.ToDouble(tisys[2]));
                    //we create a new command
                    prescription.DateHeurePrescriptionLStep = date;

                    prescription = UpdatePrescription(prescription, isPrescription, UserConect, BranchID);
                    ts.Complete();
                }
            }
            catch (Exception e)
            {
                //EcritureSneack
                IMouchar opSneak = new MoucharRepository(context);
                bool res = opSneak.InsertOperation(UserConect, "ERROR", "Prescription-ID " + prescription.ConsultationID, "UpdateChanges", prescription.DatePrescriptionLStep, BranchID);
                if (!res)
                {
                    throw new Exception("Une erreur s'est produite lors de la journalisation ");
                }
                throw new Exception("Une erreur s'est produite lors de la Prescription : " + "e.Message = " + e.Message);
            }
            return prescription;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="prescriptionLStep"></param>
        /// <param name="UserConect"></param>
        /// <param name="BranchID"></param>
        /// <param name="isPrescription"></param>
        /// <returns></returns>
        public PrescriptionLStep UpdatePrescription(PrescriptionLStep prescriptionLStep, bool isPrescription, int UserConect, int BranchID)
        {

            bool res = false;

            Consultation consultEntity = new Consultation();
            if (prescriptionLStep.ConsultationID == 0)
            {
                throw new Exception("Erreur : Please you must save the Customer on consultation Before Proceed ");
            }
            else
            {
                consultEntity = context.Consultations.Find(prescriptionLStep.ConsultationID);
                //update de la prescription pr le client
                consultEntity.MedecintTraitant = prescriptionLStep.MedecinTraitant;
                if (isPrescription)
                {
                    consultEntity.isPrescritionValidate = true;
                }
                else
                {
                    consultEntity.isPrescritionValidate = false;
                }
                context.SaveChanges();
            }
            if (consultEntity == null)
            {
                throw new Exception("Erreur : Please you must save the Customer on consultation Before Proceed ");
            }

            

            PrescriptionLStep existingPrescription = context.PrescriptionLSteps.Find(prescriptionLStep.PrescriptionLStepID);
            //update de certains elements

            existingPrescription.ConsultByID = UserConect;
            existingPrescription.DateHeurePrescriptionLStep = prescriptionLStep.DateHeurePrescriptionLStep;
            existingPrescription.Remarque = prescriptionLStep.Remarque;
            existingPrescription.PrescriptionCollyre = prescriptionLStep.PrescriptionCollyre;
            existingPrescription.CollyreName = prescriptionLStep.CollyreName;
            existingPrescription.DateRdv = (prescriptionLStep.DateRdv==null || prescriptionLStep.DateRdv.Year>1900) ? prescriptionLStep.DateRdv : existingPrescription.DateRdv;

            context.SaveChanges();
            

            //EcritureSneack
            IMouchar opSneak = new MoucharRepository(context);
            res = opSneak.InsertOperation(UserConect, "SUCCESS", "Prescription-ID " + consultEntity.ConsultationID + " FOR CUSTOMER " + consultEntity.CustomerID, "UpdatePrescription", prescriptionLStep.DatePrescriptionLStep, BranchID);
            if (!res)
            {
                throw new Exception("Une erreur s'est produite lors de la journalisation ");
            }
            
            return prescriptionLStep;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentProduct"></param>
        /// <returns></returns>
        //1-Création du Produit s'il n'existe pas
        public OrderLens CreateOrderLens(OrderLens currentProduct)
        {
            OrderLens product = context.OrderLenses.FirstOrDefault(pdt => pdt.ProductCode == currentProduct.ProductCode);

            if (product != null && product.ProductID > 0)
            {
                return product;
            }

            //1 - Création du numéro de Verre
            LensNumber currentLensNumber = LensConstruction.GetLensNumber(currentProduct.LensNumber, this.context);
            currentProduct.LensNumberID = currentLensNumber.LensNumberID;
            currentProduct.LensNumber = null;

            //2-Création de la catégorie du produit
            if (currentProduct.LensCategoryID <= 0)
            {
                LensCategory lensCategory = LensConstruction.PersistLensCategory(currentProduct.LensCategory.CategoryCode, context);
                currentProduct.LensCategoryID = lensCategory.CategoryID;
                currentProduct.CategoryID = lensCategory.CategoryID;
                currentProduct.LensCategory = lensCategory;
                currentProduct.Category = lensCategory;
            }

            //3 - Numéro de compte du produit

            CollectifAccount colAccount = currentProduct.LensCategory.CollectifAccount;
            //recuperation  du premier cpte de cette category
            Account Acct = (from a in context.Accounts
                            where a.CollectifAccountID == colAccount.CollectifAccountID
                            select a).FirstOrDefault();

            //currentProduct.AccountID = (from a in context.Accounts where a.CollectifAccountID==colAccount.CollectifAccountID
            //                                select a).FirstOrDefault().AccountID;
            if (Acct == null)
            {
                IAccount accountRepo = new AccountRepository(context);
                currentProduct.AccountID = accountRepo.GenerateAccountNumber(colAccount.CollectifAccountID, currentProduct.Category.CategoryCode, false).AccountID;
            }
            else
            {
                currentProduct.AccountID = Acct.AccountID;
            }

            currentProduct.Category = null;
            currentProduct.LensCategory = null;
            currentProduct = context.OrderLenses.Add(currentProduct);
            context.SaveChanges();

            return currentProduct;
        }
        

        

        

    }
}

