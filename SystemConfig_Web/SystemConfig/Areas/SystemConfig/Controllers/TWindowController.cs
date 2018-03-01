using System.Web.Mvc;
using BLL;
using Model;
using Newtonsoft.Json;
using System.Collections;

namespace SystemConfig.Areas.SystemConfig.Controllers
{
    public class TWindowController : Controller
    {

        TWindowBLL bll = new TWindowBLL();
        TWindowUserBLL winUserBll = new TWindowUserBLL();
        TWindowBusinessBLL winBusiBll = new TWindowBusinessBLL();

        //
        // GET: /SystemConfig/TWindow/

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

        public ActionResult GetGridDetailData(int winId)
        {
            var data = new
            {
                busiData = this.winBusiBll.GetGridDetailData(winId),
                userData = this.winUserBll.GetGridDetailData(winId)
            };
            return Content(JsonConvert.SerializeObject(data));
        }

        public ActionResult Form(int id)
        {
            var model = this.bll.GetModel(id);
            if (model == null)
                model = new TWindowModel() { ID = -1 };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitForm(TWindowModel model)
        {
            if (model.ID == -1)
                this.bll.Insert(model);
            else
                this.bll.Update(model);
            return Content("操作成功！");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteForm(int id)
        {
            this.bll.Delete(this.bll.GetModel(id));
            this.bll.ResetIndex();
            return Content("操作成功！");
        }
    }
}
