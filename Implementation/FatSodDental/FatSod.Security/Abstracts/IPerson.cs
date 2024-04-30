using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FatSod.Security.Entities;

namespace FatSod.Security.Abstracts
{
    public interface IPerson : IRepository<Person>
    {
        void Remove(int personID);
        void Delete(Person person, int UserConect, DateTime OperationDate, int BranchID);
        Person Update2(Person person, int UserConect, DateTime OperationDate, int BranchID);
        Person Create2(Person person, int UserConect, DateTime OperationDate, int BranchID);
        Person Create2Assurance(Person person, int UserConect, DateTime OperationDate, int BranchID);
        Person Create2User(Person person, int UserConect, DateTime OperationDate, int BranchID);

        Person UpdatePersonForRdv(int GlobalPersonID, DateTime DateOfBirth, int UserConect, DateTime OperationDate, int BranchID, string RaisonRdv, int GestionnaireID, string PreferredLanguage);
    }
}
