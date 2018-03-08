using System.Web.Mvc;
using BLL;
using Model;
using Newtonsoft.Json;
using System.Collections;
using SystemConfig.Controllers;

namespace SystemConfig.Areas.SystemConfig.Controllers
{
    public class TWindowController : BaseController
    {

        TWindowBLL bll;
        TWindowUserBLL winUserBll;
        TWindowBusinessBLL winBusiBll;
        TDictionaryBLL dicBll;
        TWindowAreaBLL areaBll;
        TUnitBLL unitBll;
        TBusinessBLL busiBll;
        TUserBLL userBll;

        public TWindowController()
        {
            this.bll = new TWindowBLL(this.AreaNo);
            this.winUserBll = new TWindowUserBLL(this.AreaNo);
            this.winBusiBll = new TWindowBusinessBLL(this.AreaNo);
            this.dicBll = new TDictionaryBLL(this.AreaNo);
            this.areaBll = new TWindowAreaBLL(this.AreaNo);
            this.unitBll = new TUnitBLL(this.AreaNo);
            this.busiBll = new TBusinessBLL(this.AreaNo);
            this.userBll = new TUserBLL(this.AreaNo);
        }

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

        #region window
        public ActionResult Form(int id)
        {
            var model = this.bll.GetModel(id);
            if (model == null)
                model = new TWindowModel() { ID = -1 };
            this.ViewBag.State = dicBll.GetModelList(DictionaryString.WorkState);
            this.ViewBag.areaList = JsonConvert.SerializeObject(this.areaBll.GetModelList());
            var areaModel = this.areaBll.GetModel(p => p.id == model.AreaName);
            this.ViewBag.AreaText = areaModel == null ? "" : areaModel.areaName;
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
        #endregion

        #region busi
        public ActionResult BusiForm(int id)
        {
            var model = this.winBusiBll.GetModel(id);
            var winModel = this.bll.GetModel(model.WindowID);
            this.ViewBag.WindowName = winModel == null ? "" : winModel.Name;
            var unitModel = this.unitBll.GetModel(p => p.unitSeq == model.unitSeq);
            this.ViewBag.UnitName = unitModel == null ? "" : unitModel.unitName;
            var busiModel = this.busiBll.GetModel(p => p.busiSeq == model.busiSeq);
            this.ViewBag.BusiName = busiModel == null ? "" : busiModel.busiName;
            if (model == null)
                model = new TWindowBusinessModel() { ID = -1 };
            this.ViewBag.UnitList = JsonConvert.SerializeObject(this.unitBll.GetModelList());
            this.ViewBag.BusiList = JsonConvert.SerializeObject(this.busiBll.GetModelList());
            return View("FormBusi", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitBusiForm(TWindowBusinessModel model)
        {
            if (model.ID == -1)
                this.winBusiBll.Insert(model);
            else
                this.winBusiBll.Update(model);
            return Content("操作成功！");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteBusiForm(int id)
        {
            this.winBusiBll.Delete(this.winBusiBll.GetModel(id));
            this.winBusiBll.ResetIndex();
            return Content("操作成功！");
        }
        #endregion

        #region user
        public ActionResult UserForm(int id)
        {
            var model = this.winUserBll.GetModel(id);
            var winModel = this.bll.GetModel(model.WindowID);
            this.ViewBag.WindowName = winModel == null ? "" : winModel.Name;
            var userModel = this.userBll.GetModel(p => p.ID == model.UserID);
            this.ViewBag.UserName = userModel == null ? "" : userModel.Name;
            if (model == null)
                model = new TWindowUserModel() { ID = -1 };
            this.ViewBag.State = dicBll.GetModelList(DictionaryString.WorkState);
            this.ViewBag.UserList = JsonConvert.SerializeObject(this.userBll.GetModelList());
            return View("FormUser", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitUserForm(TWindowUserModel model)
        {
            if (model.ID == -1)
                this.winUserBll.Insert(model);
            else
                this.winUserBll.Update(model);
            return Content("操作成功！");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteUserForm(int id)
        {
            this.winUserBll.Delete(this.winUserBll.GetModel(id));
            this.winUserBll.ResetIndex();
            return Content("操作成功！");
        }
        #endregion


    }
}
