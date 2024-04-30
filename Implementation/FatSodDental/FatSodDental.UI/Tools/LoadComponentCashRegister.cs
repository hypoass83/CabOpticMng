using System;
using System.Collections.Generic;
using System.Linq;
using FatSod.DataContext.Concrete;
using FatSod.Security.Entities;
using Ext.Net;
using FatSod.DataContext.Initializer;
using FatSod.Ressources;
using FatSod.Supply.Entities;

namespace FatSodDental.UI.Tools
{
    public static partial class LoadComponent
    {
        public static List<ListItem> UserForTill
        {
            get
            {
                context = new EFDbContext();
                List<ListItem> userList = new List<ListItem>();
                foreach (User user in context.People.OfType<User>().Where(u => u.IsConnected).ToArray())
                {
                    if(context.UserTills.FirstOrDefault(ut=>ut.UserID == user.GlobalPersonID && ut.HasAccess) == null)
                        userList.Add(new ListItem(user.Name, user.GlobalPersonID));
                }
                return userList;
            }
        }
				public static List<ListItem> UserTill
				{
					get
					{
						context = new EFDbContext();
						List<ListItem> userList = new List<ListItem>();
						foreach (Till user in context.PaymentMethods.OfType<Till>().ToArray())
						{
								userList.Add(new ListItem(user.Name, user.ID));
						}
						return userList;
					}
				}
        //public static Company Company(int userID)
        //{
        //    get
        //    {
        //        context = new EFDbContext();
        //        context.co

        //    }
        //}
    }
}