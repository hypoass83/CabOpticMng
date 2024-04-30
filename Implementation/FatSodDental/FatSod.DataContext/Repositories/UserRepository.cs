using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FatSod.Security.Abstracts;
using FatSod.Security.Entities;
using FatSod.DataContext.Concrete;

namespace FatSod.DataContext.Repositories
{
    public class UserRepository :Repository<User>, IUser 
    {
        public int Remove(int user)
        {
            Person userToDelete = context.People.Find(user);
            //Person person = context.People.Find(userToDelete.PersonID);
            Adress adress = context.Adresses.Find(userToDelete.AdressID);
            if (adress.AdressID != 0 && userToDelete.GlobalPersonID != 0)
            {
                context.Adresses.Remove(adress);
                //context.People.Remove(person);
                context.People.Remove(userToDelete);
                context.SaveChanges();
                return userToDelete.GlobalPersonID;
            }

            return 0;
        }
    }
}
