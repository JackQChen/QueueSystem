using System.Web.Mvc;
using BLL;
using Newtonsoft.Json;
using Model;
using System.Web.SessionState;
using SystemConfig.Controllers;
using System;

namespace SystemConfig.Areas.Statistics.Controllers
{
    public class SSatisfactionController : BaseController
    {
        BEvaluateBLL bll;

        public SSatisfactionController()
        {
            this.bll = new BEvaluateBLL("MySQL", this.AreaNo);
        }

        public ActionResult Index()
        {
            this.ViewBag.dtStart = DateTime.Now.ToString("yyyy-MM-01");
            this.ViewBag.dtEnd = DateTime.Now.ToString("yyyy-MM-dd");
            return View();
        }

        public ActionResult GetGridData(int type, DateTime startTime, DateTime endTime)
        {
            var data = new
            {
                rows = this.bll.GetSatisfied(type, startTime, endTime)
            };
            return Content(JsonConvert.SerializeObject(data));
        }
    }
}
