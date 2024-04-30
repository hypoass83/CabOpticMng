using FatSod.DataContext.Concrete;
using FatSod.DataContext.Repositories;
using FatSod.Security.Abstracts;
using FatSod.Security.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace FatSodDental.UI.Filters
{
    public class TakeBusinessDay : AuthorizeAttribute
    {
        public string RedirectUrl { get; set; }
        //public override void OnActionExecuting(ActionExecutingContext filterContext)
        //{
        //    int userID = int.Parse(filterContext.HttpContext.User.Identity.Name);
        //    EFDbContext efContext = new EFDbContext();
        //    User user = efContext.People.OfType<User>().FirstOrDefault(u => u.GlobalPersonID == userID);
        //    IBusinessDay busDayRep = new BusinessDayRepository();
        //    List<BusinessDay> bDayList = busDayRep.GetOpenedBusinessDay(user);
        //    if (bDayList.Count() == 0 || bDayList == null)
        //    {
        //        filterContext.Result = new RedirectResult("~/Security/Login");
        //    }

        //}

        //public void OnAuthorization(AuthorizationContext filterContext)
        //{
        //    int userID = int.Parse(filterContext.HttpContext.User.Identity.Name);
        //    EFDbContext efContext = new EFDbContext();
        //    User user = efContext.People.OfType<User>().FirstOrDefault(u => u.GlobalPersonID == userID);
        //    IBusinessDay busDayRep = new BusinessDayRepository();
        //    List<BusinessDay> bDayList = busDayRep.GetOpenedBusinessDay(user);
        //    if (bDayList.Count() == 0 || bDayList == null)
        //    {
        //        filterContext.Result = new RedirectResult("~/Security/Login");
        //    }
        //}
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            bool isAuthorize = base.AuthorizeCore(httpContext);
            if (!isAuthorize)
            {
                return false;
            }
            /*
            int userID = int.Parse(httpContext.User.Identity.Name);
            EFDbContext efContext = new EFDbContext();
            //User user = efContext.People.OfType<User>().SingleOrDefault(u => u.GlobalPersonID == userID);
            User user = (from u in efContext.Users.Where(u => u.GlobalPersonID == userID)
                         select u).SingleOrDefault();
            IBusinessDay busDayRep = new BusinessDayRepository();
            List<BusinessDay> bDayList = busDayRep.GetOpenedBusinessDay(user);
            */
            List<BusinessDay> bDayList = (List<BusinessDay>)System.Web.HttpContext.Current.Session["UserBusDays"];
            if (bDayList == null || bDayList.Count() == 0 )
            {
                return false;
            }
            return true;
        }
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new RedirectToRouteResult(
                        new RouteValueDictionary(
                            new
                            {
                                controller = "../Session",
                                action = "NoBDOpened"
                            })
                        );
        }
    }
}