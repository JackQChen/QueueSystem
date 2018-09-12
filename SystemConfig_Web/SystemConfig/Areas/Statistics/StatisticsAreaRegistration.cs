using System.Web.Mvc;

namespace SystemConfig.Areas.Statistics
{
    public class StatisticsAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Statistics";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Statistics_default",
                "Statistics/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
