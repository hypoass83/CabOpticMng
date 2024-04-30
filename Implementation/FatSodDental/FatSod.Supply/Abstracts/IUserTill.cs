using FatSod.Supply.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FatSod.Supply.Abstracts
{
    public interface IUserTill : IRepositorySupply<UserTill>
    {
        /// <summary>
        /// methode permettant de creer un nouveau caissier
        /// </summary>
        /// <param name="eGL"></param>
        /// <returns></returns>
        UserTill CreateUserTill(Till till,UserTill userTill, int UserConect, int BranchID, DateTime DateOperation);

        /// <summary>
        /// methode permettant de modifier un caissier
        /// </summary>
        /// <param name="eGL"></param>
        /// <returns></returns>
        UserTill UpdateUserTill(Till till, UserTill userTill, int UserConect, int BranchID, DateTime DateOperation);
    }
}
