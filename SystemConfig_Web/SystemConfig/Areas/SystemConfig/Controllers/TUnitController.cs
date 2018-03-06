using System.Web.Mvc;
using BLL;
using Newtonsoft.Json;
using Model;
using System.Web.SessionState;
using SystemConfig.Controllers;

namespace SystemConfig.Areas.SystemConfig.Controllers
{
    public class TUnitController : BaseController
    {
        TUnitBLL bll;

        public TUnitController()
        {
            this.bll = new TUnitBLL(this.AreaNo);
        }

        //
        // GET: /SystemConfig/TUnit/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetGridData(Pagination p)
        {
            var data = new
            {
                rows = this.bll.GetGridData()
            };
            return Content(JsonConvert.SerializeObject(data));
        }

        public ActionResult Form(int id)
        {
            var model = this.bll.GetModel(id);
            if (model == null)
                model = new TUnitModel() { id = -1 };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitForm(TUnitModel model)
        {
            if (model.id == -1)
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
