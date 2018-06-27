using System;
using System.Web.Mvc;
using BLL;
using Model;
using Newtonsoft.Json;
using SystemConfig.Controllers;

namespace SystemConfig.Areas.SystemConfig.Controllers
{
    public class TUserController : BaseController
    {
        TUserBLL bll;
        TDictionaryBLL dicBll;
        TUnitBLL unitBll;

        public TUserController()
        {
            this.bll = new TUserBLL(this.AreaNo);
            this.dicBll = new TDictionaryBLL(this.AreaNo);
            this.unitBll = new TUnitBLL(this.AreaNo);
        }

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
            if (model.Photo != null)
                this.ViewBag.avatar = "data:image/png;base64," + Convert.ToBase64String(model.Photo);
            var unitModel = this.unitBll.GetModel(p => p.unitSeq == model.unitSeq);
            this.ViewBag.unitName = unitModel == null ? "" : unitModel.unitName;
            this.ViewBag.unitList = JsonConvert.SerializeObject(unitBll.GetModelList());
            this.ViewBag.Sex = dicBll.GetModelList(DictionaryString.UserSex);
            this.ViewBag.State = dicBll.GetModelList(DictionaryString.WorkState);
            return View(model);
        }

        public ActionResult QueryUserInfo(string code, string name)
        {
            name = name.Trim();
            var codeModel = this.bll.GetModel(p => p.Code == code);
            var nameModel = this.bll.GetModel(p => p.Name == name);
            return Content(JsonConvert.SerializeObject(new
            {
                code = codeModel == null ? "" : code,
                name = nameModel == null ? "" : name
            }));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitForm(TUserModel model, string avatar)
        {
            if (!string.IsNullOrEmpty(avatar))
                model.Photo = Convert.FromBase64String(avatar.Split(',')[1]);
            if (this.bll.GetModel(p => p.Code == model.Code && p.ID != model.ID) != null)
                return Content("用户编号重复，请核查");
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
