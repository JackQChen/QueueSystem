using System.Web.Mvc;
using BLL;
using SystemConfig.Controllers;
using Newtonsoft.Json;
using Model;
using System;

namespace SystemConfig.Areas.SystemConfig.Controllers
{
    public class TLedWindowController : BaseController
    {
        TWindowBLL windowBll;
        TLedControllerBLL lcBll;
        TLedWindowBLL ledWinBll;

        public TLedWindowController()
        {
            windowBll = new TWindowBLL("MySQL", this.AreaNo);
            lcBll = new TLedControllerBLL("MySQL", this.AreaNo);
            ledWinBll = new TLedWindowBLL("MySQL", this.AreaNo);
        }

        //
        // GET: /SystemConfig/TLedWindow/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetGridData(Pagination p)
        {
            var data = new
            {
                rows = lcBll.GetGridData()
            };
            return Content(JsonConvert.SerializeObject(data));
        }

        public ActionResult GetGridDetailData(Pagination p, int controllerId)
        {
            return Content(JsonConvert.SerializeObject(ledWinBll.GetGridDataByControllerId(controllerId)));
        }

        public ActionResult Form(int id)
        {
            var model = this.ledWinBll.GetModel(id);
            if (model == null)
                model = new TLedWindowModel()
                {
                    ID = -1,
                    ControllerID = Convert.ToInt32(this.Request["ctlId"])
                };
            var ctlModel = this.lcBll.GetModel(p => p.ID == model.ControllerID);
            this.ViewBag.ControllerName = ctlModel == null ? "" : ctlModel.Name;
            var winModel = this.windowBll.GetModel(p => p.Number == model.WindowNumber);
            this.ViewBag.WinName = winModel == null ? "" : winModel.Name;
            this.ViewBag.WinList = JsonConvert.SerializeObject(this.windowBll.GetModelList());
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitForm(TLedWindowModel model)
        {
            if (model.ID == -1)
                this.ledWinBll.Insert(model);
            else
                this.ledWinBll.Update(model);
            return Content("操作成功！");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteForm(int id)
        {
            this.ledWinBll.Delete(this.ledWinBll.GetModel(id)); 
            return Content("操作成功！");
        }
    }
}
