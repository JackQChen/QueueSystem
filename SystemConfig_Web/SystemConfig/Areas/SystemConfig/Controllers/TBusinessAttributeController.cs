using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SystemConfig.Controllers;
using BLL;
using Newtonsoft.Json;

namespace SystemConfig.Areas.SystemConfig.Controllers
{
    public class TBusinessAttributeController : BaseController
    {
        TBusinessAttributeBLL bll;

        public TBusinessAttributeController()
        {
            this.bll = new TBusinessAttributeBLL(this.AreaNo);
        }

        //
        // GET: /SystemConfig/TBusinessAttribute/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetGridData(Pagination p)
        {
            var data = new
            {
                rows = bll.GetGridData()
            };
            return Content(JsonConvert.SerializeObject(data));
        }

        public ActionResult GetGridDataByUnitSeq(Pagination p, string unitSeq)
        {
            return Content(JsonConvert.SerializeObject(this.bll.GetGridDataByUnitSeq(unitSeq)));
        }

        public ActionResult GetGridDetailData(Pagination p, string unitSeq, string busiSeq)
        {
            return Content(JsonConvert.SerializeObject(this.bll.GetGridDetailData(unitSeq, busiSeq)));
        }

    }
}
