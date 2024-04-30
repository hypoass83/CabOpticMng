using System.Web.Mvc;

namespace CABOPMANAGEMENT.Areas.SMSMNG
{
    public class SMSMNGAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "SMSMNG";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "SMSMNG_default",
                "SMSMNG/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}