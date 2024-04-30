using System.Web.Mvc;

namespace CABOPMANAGEMENT.Areas.CashRegister
{
    public class CashRegisterAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "CashRegister";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "CashRegister_default",
                "CashRegister/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}