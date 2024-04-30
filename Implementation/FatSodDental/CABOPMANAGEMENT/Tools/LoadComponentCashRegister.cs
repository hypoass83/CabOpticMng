using System;
using System.Collections.Generic;
using System.Linq;
using FatSod.DataContext.Concrete;
using FatSod.Security.Entities;
using FatSod.DataContext.Initializer;
using FatSod.Ressources;
using FatSod.Supply.Entities;

namespace CABOPMANAGEMENT.Tools
{
    public static partial class LoadComponent
    {
        public static List<User> UserForTill
        {
            get
            {
                context = new EFDbContext();
                List<User> userList = new List<User>();
                foreach (User user in context.People.OfType<User>().Where(u => u.IsConnected).ToArray())
                {
                    if(context.UserTills.FirstOrDefault(ut=>ut.UserID == user.GlobalPersonID && ut.HasAccess) == null)
                        userList.Add(new User { Name=user.Name, GlobalPersonID=user.GlobalPersonID });
                }
                return userList;
            }
        }
        public static List<Till> UserTill
				{
					get
					{
						context = new EFDbContext();
                        List<Till> userList = new List<Till>();
						foreach (Till user in context.PaymentMethods.OfType<Till>().ToArray())
						{
                            userList.Add(new Till { Name=user.Name, ID = user.ID });
						}
						return userList;
					}
				}
        
    }
}