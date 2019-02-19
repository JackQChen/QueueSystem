using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using BLL;
using Newtonsoft.Json;
using SystemConfig.Controllers;

namespace SystemConfig.Areas.Statistics.Controllers
{
    public class SRate2Controller : BaseController
    {
        BEvaluateBLL bll;
        TUnitBLL unitBll;
        TUserBLL userBll;

        public SRate2Controller()
        {
            this.bll = new BEvaluateBLL("MySQL", this.AreaNo);
            this.unitBll = new TUnitBLL("MySQL", this.AreaNo);
            this.userBll = new TUserBLL("MySQL", this.AreaNo);
        }

        public ActionResult Index()
        {
            this.ViewBag.unitList = JsonConvert.SerializeObject(unitBll.GetModelList());
            this.ViewBag.userList = JsonConvert.SerializeObject(userBll.GetModelList());
            this.ViewBag.dtStart = DateTime.Now.ToString("yyyy-MM-01");
            this.ViewBag.dtEnd = DateTime.Now.ToString("yyyy-MM-dd");
            return View();
        }

        public ActionResult GetChartData(Pagination p, int type, DateTime startTime, DateTime endTime, string unitSeq, string userId)
        {
            return Content(JsonConvert.SerializeObject(this.bll.GetEvaluatePercent(type, startTime, endTime, unitSeq, userId)));
        }
    }
}
