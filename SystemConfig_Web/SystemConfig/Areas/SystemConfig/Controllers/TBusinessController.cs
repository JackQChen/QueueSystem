using System.Web.Mvc;
using BLL;
using Model;
using Newtonsoft.Json;
using SystemConfig.Controllers;

namespace SystemConfig.Areas.SystemConfig.Controllers
{
    public class TBusinessController : BaseController
    {
        TBusinessBLL bll;
        TDictionaryBLL dicBll;

        public TBusinessController()
        {
            bll = new TBusinessBLL(this.AreaNo);
            dicBll = new TDictionaryBLL(this.AreaNo);
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
                rows = bll.GetGridData()
            };
            return Content(JsonConvert.SerializeObject(data));
        }

        public ActionResult Form(int id)
        {
            var model = this.bll.GetModel(id);
            if (model == null)
                model = new TBusinessModel() { id = -1 };
            this.ViewBag.busiType = dicBll.GetModelList(DictionaryString.AppointmentType);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitForm(TBusinessModel model)
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
