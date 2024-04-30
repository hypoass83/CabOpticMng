using System.Web.Mvc;

namespace CABOPMANAGEMENT.Areas.BarCode
{
    public class BarCodeAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "BarCode";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "BarCode_default",
                "BarCode/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}