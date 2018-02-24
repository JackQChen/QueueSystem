using System.Web.Mvc;
using BLL;
using Newtonsoft.Json;

namespace SystemConfig.Areas.SystemConfig.Controllers
{
    public class TUserController : Controller
    {
        TUserBLL bll = new TUserBLL();
        //
        // GET: /SystemConfig/TUser/

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
