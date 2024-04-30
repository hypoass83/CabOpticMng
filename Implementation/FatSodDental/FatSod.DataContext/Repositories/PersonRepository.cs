using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FatSod.Security.Abstracts;
using FatSod.Security.Entities;
using FatSod.DataContext.Concrete;
using FatSod.Supply.Entities;
using System.Transactions;

namespace FatSod.DataContext.Repositories
{
    public class PersonRepository : Repository<Person>, IPerson
    {
        public void Remove(int personID)
        {
            //var em = context.Set<User>();
            //User userToDelete = em.Find(person.PersonID);
            //using (EFDbContext cont2 = new EFDbContext())
            {
                User personToDelete = new User();
                personToDelete = context.People.OfType<User>().SingleOrDefault(p => p.GlobalPersonID == personID);
                Adress adress = context.Adresses.Find(personToDelete.AdressID);
                if (personToDelete != null)
                {
                    foreach (var userBranch in context.UserBranches.Where(ub => ub.UserID == personID))
                    {
                        context.ObjectContext.DeleteObject(userBranch);
                    }
                    context.Adresses.Remove(adress);
                    context.ObjectContext.DeleteObject(personToDelete);
                    context.SaveChanges();
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="GlobalPersonID"></param>
        /// <param name="DateOfBirth"></param>
        /// <param name="UserConect"></param>
        /// <param name="OperationDate"></param>
        /// <param name="BranchID"></param>
        /// <returns></returns>
        public Person UpdatePersonForRdv(int GlobalPersonID,DateTime DateOfBirth, int UserConect, DateTime OperationDate, int BranchID, string RaisonRdv, int GestionnaireID, string PreferredLanguage)
        {
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    // Cette date doit prendre la valeur du jour ou cette fonctionnalite sera deployee chez le client
                    // sans cette date, pending consultation retourne une consultation qui n'est pas la bonne
                    DateTime pendingConsultationBeginingDate = new DateTime(2019, 12, 25);
                    var personToUpdate = context.People.Find(GlobalPersonID);
                    if (personToUpdate != null)
                    {
                        if (personToUpdate is Customer)
                        {
                            ((Customer)personToUpdate).DateOfBirth = DateOfBirth;
                            ((Customer)personToUpdate).IsInHouseCustomer = true;
                            ((Customer)personToUpdate).PreferredLanguage = PreferredLanguage;

                        }
                        var existingConsul = context.Consultations.Where(c => c.CustomerID == GlobalPersonID && c.DateConsultation == OperationDate).FirstOrDefault();
                        if (existingConsul!=null)
                        {
                            throw new Exception("The Customer " + personToUpdate.Name + " has already been validated for Appointment");
                        }

                        // Consultation du client qui n'a pas encore ete finalisee
                        var pendingConsultation = context.Consultations.Where(c =>/* c.DateConsultation >= pendingConsultationBeginingDate && */c.CustomerID == GlobalPersonID && (c.isPrescritionValidate == false || c.MedecintTraitant == null)).OrderByDescending(c => c.DateConsultation).FirstOrDefault();

                        if (pendingConsultation != null)
                        {
                            // Mise a jour de la consultation existante
                            if (pendingConsultation.RaisonRdv != RaisonRdv)
                            {
                                pendingConsultation.RaisonRdv = RaisonRdv;
                            }
                            if (pendingConsultation.GestionnaireID != GestionnaireID)
                            {
                                pendingConsultation.GestionnaireID = GestionnaireID;
                            }
                            pendingConsultation.DateConsultation = OperationDate;
                        }
                        else
                        {
                            //ajout ds la table des consultations
                            Consultation newConsult = new Consultation
                            {
                                CustomerID = GlobalPersonID,
                                DateConsultation = OperationDate,
                                IsNewCustomer = false,
                                isPrescritionValidate = false,
                                RaisonRdv = RaisonRdv,
                                GestionnaireID = GestionnaireID
                            };
                            context.Consultations.Add(newConsult);
                        }

                        //we save changes
                        context.SaveChanges();

                        //EcritureSneack/
                        IMouchar opSneak = new MoucharRepository(context);
                        bool res = opSneak.InsertOperation(UserConect, "SUCCESS", "UPDATE PERSON ID " + personToUpdate.GlobalPersonID + " NAME " + personToUpdate.Name, "UpdatePersonForRdv", OperationDate, BranchID);
                        if (!res)
                        {
                            throw new Exception("Une erreur s'est produite lors de la journalisation ");
                        }
                    }
                    
                    ts.Complete();
                    return personToUpdate;
                }
            }
            catch (Exception e)
            {
                //transaction.Rollback();
                //EcritureSneack
                IMouchar opSneak = new MoucharRepository(context);
                bool res = opSneak.InsertOperation(UserConect, "ERROR", "UPDATE PERSON ID" + GlobalPersonID, "UpdatePersonForRdv", OperationDate, BranchID);
                if (!res)
                {
                    throw new Exception("Une erreur s'est produite lors de la journalisation ");
                }
                throw new Exception("Error : " + e.Message);

            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="person"></param>
        /// <param name="UserConect"></param>
        /// <param name="OperationDate"></param>
        /// <param name="BranchID"></param>
        /// <returns></returns>
        public Person Create2(Person person, int UserConect, DateTime OperationDate, int BranchID)
        {
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    //assurons ns ke ne numero de client n'existe pas
                    GlobalPerson existcustomer = context.GlobalPeople.Where(c => c.CNI == person.CNI).FirstOrDefault();
                    if (existcustomer!=null)
                    {
                        throw new Exception("The Patien with ID "+ existcustomer.CNI + " Already exist" );
                    }

                    Person newperson = context.People.Add(person);
                    context.SaveChanges();
                    //ajout ds la table des consultations
                    Consultation newConsult = new Consultation
                    {
                        CustomerID= newperson.GlobalPersonID,
                        DateConsultation=OperationDate,
                        IsNewCustomer=true,
                        isPrescritionValidate=false,
                        GestionnaireID = ((Customer)person).GestionnaireID
                    };
                    context.Consultations.Add(newConsult);
                    context.SaveChanges();
                    //mise a jour du cpteur du transact number
                    TransactNumber trn = context.TransactNumbers.SingleOrDefault(t => t.TransactNumberCode == "CUST");
                    if (trn != null)
                    {
                        //persistance du compteur de l'objet TransactNumber
                        trn.Counter = trn.Counter + 1;
                    }
                    context.SaveChanges();
                    //EcritureSneack
                    IMouchar opSneak = new MoucharRepository(context);
                    bool res = opSneak.InsertOperation(UserConect, "SUCCESS", "INSERT PERSON " + person.CNI + " NAME " + person.Name, "Create2", OperationDate, BranchID);
                    if (!res)
                    {
                        throw new Exception("Une erreur s'est produite lors de la journalisation ");
                    }
                    ts.Complete();
                }
                return person;
            }
            catch (Exception e)
            {
                //transaction.Rollback();
                //EcritureSneack
                IMouchar opSneak = new MoucharRepository(context);
                bool res = opSneak.InsertOperation(UserConect, "ERROR", "INSERT PERSON " + person.CNI + " NAME " + person.Name, "Create2", OperationDate, BranchID);
                if (!res)
                {
                    throw new Exception("Une erreur s'est produite lors de la journalisation ");
                }
                throw new Exception("This entity is used or has been do an operation : " + e.Message);

            }

        }
        public Person Create2Assurance(Person person, int UserConect, DateTime OperationDate, int BranchID)
        {
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    
                    context.People.Add(person);
                    context.SaveChanges();
                    //creation du mode de payement par assurance
                    AssureurPM assPM = new AssureurPM
                    {
                        Code = person.Name + " " + person.CNI,
                        AssureurID = person.GlobalPersonID,
                        Description = "This is the Insurance account of " + person.Name,
                        Name = person.Name + " " + person.CNI + " Insurance Account",
                        BranchID = BranchID
                    };
                    //csa = this.Create(csa);
                    this.context.AssureurPMs.Add(assPM);
                    this.context.SaveChanges();
                    //EcritureSneack
                    IMouchar opSneak = new MoucharRepository(context);
                    bool res = opSneak.InsertOperation(UserConect, "SUCCESS", "INSERT INSURANCE NAME " + person.Name, "Create2", OperationDate, BranchID);
                    if (!res)
                    {
                        throw new Exception("Une erreur s'est produite lors de la journalisation ");
                    }

                    ts.Complete();
                }
                return person;
            }
            catch (Exception e)
            {
                throw new Exception("This entity is used or has been do an operation : " + e.Message);

            }

        }
        public Person Create2User(Person person, int UserConect, DateTime OperationDate, int BranchID)
        {
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    var existuser = context.Users.SingleOrDefault(u=>u.UserLogin== ((User)person).UserLogin);
                    if (existuser == null)
                    {
                        context.People.Add(person);
                        context.SaveChanges();
                    }
                    else
                    {
                        throw new Exception("This userlogin already exist ");
                    }
                    //EcritureSneack
                    IMouchar opSneak = new MoucharRepository(context);
                    bool res = opSneak.InsertOperation(UserConect, "SUCCESS", "INSERT USER NAME " + person.Name, "Create2User", OperationDate, BranchID);
                    if (!res)
                    {
                        throw new Exception("Une erreur s'est produite lors de la journalisation ");
                    }

                    ts.Complete();
                }
                return person;
            }
            catch (Exception e)
            {
                throw new Exception("This entity is used or has been do an operation : " + e.Message);

            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="person"></param>
        /// <param name="UserConect"></param>
        /// <param name="OperationDate"></param>
        /// <param name="BranchID"></param>
        /// <returns></returns>
        public Person Update2(Person person, int UserConect, DateTime OperationDate, int BranchID)
        {
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    var personToUpdate = context.People.Find(person.GlobalPersonID);
                    Adress adressToUpdate = context.Adresses.Find(personToUpdate.AdressID);
                    if (personToUpdate != null)
                    {
                        personToUpdate.CNI = person.CNI;
                        personToUpdate.Name = person.Name;
                        personToUpdate.Description = person.Description;
                        //adress properties
                        adressToUpdate.AdressEmail = person.Adress.AdressEmail;
                        adressToUpdate.AdressFax = person.Adress.AdressFax;
                        adressToUpdate.AdressPhoneNumber = person.Adress.AdressPhoneNumber;
                        adressToUpdate.AdressPOBox = person.Adress.AdressPOBox;
                        adressToUpdate.QuarterID = person.Adress.QuarterID;
                        if (personToUpdate is User)
                        {
                            ((User)personToUpdate).UserAccessLevel = ((User)person).UserAccessLevel;
                            ((User)personToUpdate).ProfileID = ((User)person).ProfileID;
                            ((User)personToUpdate).UserLogin = ((User)person).UserLogin;
                            ((User)personToUpdate).UserPassword = ((User)person).UserPassword;
                            ((User)personToUpdate).UserAccountState = ((User)person).UserAccountState;
                            ((User)personToUpdate).SexID = ((User)person).SexID;
                            ((User)personToUpdate).IsConnected = ((User)person).IsConnected;
                            ((User)personToUpdate).IsMarketer = ((User)person).IsMarketer;
                            ((User)personToUpdate).IsSeller = ((User)person).IsSeller;
                        }
                        if (personToUpdate is Customer)
                        {
                            //QUESTION est ce ke l'on peu changer le numero de cpte d'un client ?
                            //si oui il fodrait modifier le mm compte ds les ecritures comptables
                            ((Customer)personToUpdate).AccountID = ((Customer)person).AccountID;
                            ((Customer)personToUpdate).SexID = ((Customer)person).SexID;
                            ((Customer)personToUpdate).LimitAmount = ((Customer)person).LimitAmount;
                            ((Customer)personToUpdate).GestionnaireID = ((Customer)person).GestionnaireID;
                            ((Customer)personToUpdate).CompanyName = ((Customer)person).CompanyName;
                            ((Customer)personToUpdate).DateOfBirth = ((Customer)person).DateOfBirth;
                            ((Customer)personToUpdate).Profession = ((Customer)person).Profession;
                            ((Customer)personToUpdate).IsBillCustomer = ((Customer)person).IsBillCustomer;
                            ((Customer)personToUpdate).IsInHouseCustomer = ((Customer)person).IsInHouseCustomer;
                            ((Customer)personToUpdate).PreferredLanguage = ((Customer)person).PreferredLanguage;
                        }
                        if (personToUpdate is Assureur)
                        {
                            //QUESTION est ce ke l'on peu changer le numero de cpte d'un client ?
                            //si oui il fodrait modifier le mm compte ds les ecritures comptables
                            ((Assureur)personToUpdate).AccountID = ((Assureur)person).AccountID;
                            ((Assureur)personToUpdate).CompanyTradeRegister = ((Assureur)person).CompanyTradeRegister;
                            ((Assureur)personToUpdate).SexID = ((Assureur)person).SexID;
                            ((Assureur)personToUpdate).CompanySigle = ((Assureur)person).CompanySigle;
                            ((Assureur)personToUpdate).CompanySlogan = ((Assureur)person).CompanySlogan;
                            ((Assureur)personToUpdate).Description = ((Assureur)person).Description;
                            ((Assureur)personToUpdate).CompteurFacture = ((Assureur)person).CompteurFacture;
                            ((Assureur)personToUpdate).Remise = ((Assureur)person).Remise;
                            ((Assureur)personToUpdate).Matricule = ((Assureur)person).Matricule;
                        }
                        //we save changes
                        context.SaveChanges();

                        //EcritureSneack
                        IMouchar opSneak = new MoucharRepository(context);
                        bool res = opSneak.InsertOperation(UserConect, "SUCCESS", "UPDATE PERSON ID " + person.GlobalPersonID + " NAME " + person.Name, "Update2", OperationDate, BranchID);
                        if (!res)
                        {
                            throw new Exception("Une erreur s'est produite lors de la journalisation ");
                        }
                    }
                    //mise a jour du cpteur du transact number
                    TransactNumber trn = context.TransactNumbers.SingleOrDefault(t => t.TransactNumberCode == "CUST");
                    if (trn != null)
                    {
                        //persistance du compteur de l'objet TransactNumber
                        trn.Counter = trn.Counter + 1;
                    }
                    context.SaveChanges();
                    ts.Complete();
                    return personToUpdate;
                }
            }
            catch (Exception e)
            {
                //transaction.Rollback();
                //EcritureSneack
                IMouchar opSneak = new MoucharRepository(context);
                bool res = opSneak.InsertOperation(UserConect, "ERROR", "UPDATE PERSON ID " + person.GlobalPersonID + " NAME " + person.Name, "Update2", OperationDate, BranchID);
                if (!res)
                {
                    throw new Exception("Une erreur s'est produite lors de la journalisation ");
                }
                throw new Exception("This entity is used or has been do an operation : " + e.Message);

            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="person"></param>
        /// <param name="UserConect"></param>
        /// <param name="OperationDate"></param>
        /// <param name="BranchID"></param>
        public void Delete(Person person, int UserConect, DateTime OperationDate, int BranchID)
        {

                try
                {
                    using (TransactionScope ts = new TransactionScope())
                    {
                        Adress adressToDelete = context.Adresses.Find(person.AdressID);
                        context.Adresses.Remove(adressToDelete);
                        Person personToDelete = context.People.Find(person.GlobalPersonID);
                        context.People.Remove(personToDelete);
                        context.SaveChanges();
                        //transaction.Commit();
                        //EcritureSneack
                        IMouchar opSneak = new MoucharRepository(context);
                        bool res = opSneak.InsertOperation(UserConect, "SUCCESS", "DELETE PERSON " + person.CNI + " NAME " + person.Name, "Delete", OperationDate, BranchID);
                        if (!res)
                        {
                            throw new Exception("Une erreur s'est produite lors de la journalisation ");
                        }
                        ts.Complete();
                    }
                }
                catch (Exception e)
                {
                    //transaction.Rollback();
                    //EcritureSneack
                    IMouchar opSneak = new MoucharRepository(context);
                    bool res = opSneak.InsertOperation(UserConect, "ERROR", "DELETE PERSON " + person.CNI + " NAME " + person.Name, "Delete", OperationDate, BranchID);
                    if (!res)
                    {
                        throw new Exception("Une erreur s'est produite lors de la journalisation ");
                    }
                    throw new Exception("This entity is used or has been do an operation : " + e.Message);

                }

            //}

        }

    }
}
