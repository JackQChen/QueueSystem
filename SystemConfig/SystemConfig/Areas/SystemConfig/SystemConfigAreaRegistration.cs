using System.Web.Mvc;

namespace SystemConfig.Areas.SystemConfig
{
    public class SystemConfigAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "SystemConfig";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "SystemConfig_default",
                "SystemConfig/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
