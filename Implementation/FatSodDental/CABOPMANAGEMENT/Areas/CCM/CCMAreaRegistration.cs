using System.Web.Mvc;

namespace CABOPMANAGEMENT.Areas.CCM
{
    public class CCMAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "CCM";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "CCM_default",
                "CCM/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}