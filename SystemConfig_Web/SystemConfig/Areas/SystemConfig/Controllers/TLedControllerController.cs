using System.Web.Mvc;
using BLL;
using Newtonsoft.Json;

namespace SystemConfig.Areas.SystemConfig.Controllers
{
    public class TLedControllerController : Controller
    {
        TLedControllerBLL bll = new TLedControllerBLL();
        //
        // GET: /SystemConfig/TLedController/

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
