using System.Web.Mvc;
using BLL;
using Newtonsoft.Json;

namespace SystemConfig.Areas.SystemConfig.Controllers
{
    public class TWindowAreaController : Controller
    {
        TWindowAreaBLL bll = new TWindowAreaBLL();
        //
        // GET: /SystemConfig/TWindowArea/

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

    }
}
