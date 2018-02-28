using System.Web.Mvc;
using BLL;
using Model;
using Newtonsoft.Json;

namespace SystemConfig.Areas.SystemConfig.Controllers
{
    public class TUserController : Controller
    {
        TUserBLL bll = new TUserBLL();
        TDictionaryBLL dicBll = new TDictionaryBLL();
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

        public ActionResult Form(int id)
        {
            var model = this.bll.GetModel(id);
            if (model == null)
                model = new TUserModel() { ID = -1 };
            this.ViewBag.Sex = dicBll.GetModelList(DictionaryString.UserSex);
            this.ViewBag.State = dicBll.GetModelList(DictionaryString.WorkState);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitForm(TUserModel model)
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
