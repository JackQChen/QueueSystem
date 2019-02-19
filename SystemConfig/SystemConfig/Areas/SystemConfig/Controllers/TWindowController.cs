using System.Web.Mvc;
using BLL;
using Model;
using Newtonsoft.Json;
using System.Collections;
using SystemConfig.Controllers;
using System;

namespace SystemConfig.Areas.SystemConfig.Controllers
{
    public class TWindowController : BaseController
    {

        TWindowBLL bll;
        TWindowBusinessBLL winBusiBll;
        FDictionaryBLL dicBll;
        TWindowAreaBLL areaBll;
        TUnitBLL unitBll;
        TBusinessBLL busiBll;
        TUserBLL userBll;
        TLedWindowBLL ledWinBll;

        public TWindowController()
        {
            this.bll = new TWindowBLL("MySQL", this.AreaNo);
            this.winBusiBll = new TWindowBusinessBLL("MySQL", this.AreaNo);
            this.dicBll = new FDictionaryBLL("MySQL", this.AreaNo);
            this.areaBll = new TWindowAreaBLL("MySQL", this.AreaNo);
            this.unitBll = new TUnitBLL("MySQL", this.AreaNo);
            this.busiBll = new TBusinessBLL("MySQL", this.AreaNo);
            this.userBll = new TUserBLL("MySQL", this.AreaNo);
            this.ledWinBll = new TLedWindowBLL("MySQL", this.AreaNo);
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
                busiData = this.winBusiBll.GetGridBusiData(winId),
                userData = this.winBusiBll.GetGridUserData(winId)
            };
            return Content(JsonConvert.SerializeObject(data));
        }

        #region window
        public ActionResult Form(int id)
        {
            var model = this.bll.GetModel(id);
            if (model == null)
                model = new TWindowModel() { ID = -1 };
            this.ViewBag.State = dicBll.GetModelListByName(FDictionaryString.WorkState);
            this.ViewBag.areaList = JsonConvert.SerializeObject(this.areaBll.GetModelList());
            var areaModel = this.areaBll.GetModel(p => p.ID == model.AreaName);
            this.ViewBag.AreaText = areaModel == null ? "" : areaModel.areaName;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitForm(TWindowModel model)
        {
            if (this.bll.GetModel(p => p.Number == model.Number && p.ID != model.ID) != null)
                return Content("窗口号重复，请核查");
            if (model.ID == -1)
                this.bll.Insert(model);
            else
            {
                //通过原有窗口号更新
                var originModel = this.bll.GetModel(model.ID);
                var ledWinModels = this.ledWinBll.GetModelList(p => p.WindowNumber == originModel.Number);
                foreach (var ledWinModel in ledWinModels)
                {
                    ledWinModel.WindowNumber = model.Number;
                    this.ledWinBll.Update(ledWinModel);
                }
                this.bll.Update(model);
            }
            return Content("操作成功！");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteForm(int id)
        {
            this.bll.Delete(this.bll.GetModel(id));
            return Content("操作成功！");
        }
        #endregion

        #region busi
        public ActionResult BusiForm(int id)
        {
            var model = this.winBusiBll.GetModel(id);
            if (model == null)
                model = new TWindowBusinessModel()
                {
                    ID = -1,
                    WindowID = Convert.ToInt32(this.Request["winId"])
                };
            var winModel = this.bll.GetModel(model.WindowID);
            this.ViewBag.WindowName = winModel == null ? "" : winModel.Name;
            var unitModel = this.unitBll.GetModel(p => p.unitSeq == model.unitSeq);
            this.ViewBag.UnitName = unitModel == null ? "" : unitModel.unitName;
            var busiModel = this.busiBll.GetModel(p => p.busiSeq == model.busiSeq);
            this.ViewBag.BusiName = busiModel == null ? "" : busiModel.busiName;
            this.ViewBag.UnitList = JsonConvert.SerializeObject(this.unitBll.GetModelList());
            this.ViewBag.BusiList = JsonConvert.SerializeObject(this.busiBll.GetModelList());
            return View("FormBusi", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitBusiForm(TWindowBusinessModel model)
        {
            if (model.ID == -1)
            {
                if (this.winBusiBll.GetModel(
                    p => p.WindowID == model.WindowID
                        && p.unitSeq == model.unitSeq
                        && p.busiSeq == model.busiSeq) == null)
                    this.winBusiBll.Insert(model);
                else
                    return Content("记录重复！");
            }
            else
                this.winBusiBll.Update(model);
            return Content("操作成功！");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteBusiForm(int id)
        {
            this.winBusiBll.Delete(this.winBusiBll.GetModel(id));
            return Content("操作成功！");
        }
        #endregion
    }
}
