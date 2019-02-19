using System.Web.Mvc;
using BLL;
using Model;
using Newtonsoft.Json;
using SystemConfig.Controllers;

namespace SystemConfig.Areas.SystemConfig.Controllers
{
    public class TBusinessAttributeController : BaseController
    {
        TBusinessAttributeBLL bll;
        TUnitBLL unitBll;
        TBusinessBLL busiBll;

        public TBusinessAttributeController()
        {
            this.bll = new TBusinessAttributeBLL("MySQL", this.AreaNo);
            this.unitBll = new TUnitBLL("MySQL", this.AreaNo);
            this.busiBll = new TBusinessBLL("MySQL", this.AreaNo);
        }

        //
        // GET: /SystemConfig/TBusinessAttribute/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetGridData(Pagination p)
        {
            var data = new
            {
                rows = busiBll.GetUnitList()
            };
            return Content(JsonConvert.SerializeObject(data));
        }

        public ActionResult GetGridDataByUnitSeq(Pagination p, string unitSeq)
        {
            return Content(JsonConvert.SerializeObject(this.busiBll.GetGridDataByUnitSeq(unitSeq)));
        }

        public ActionResult GetGridDetailData(Pagination p, string unitSeq, string busiSeq)
        {
            return Content(JsonConvert.SerializeObject(this.bll.GetGridDetailData(unitSeq, busiSeq)));
        }

        public ActionResult Form(int id, string unitSeq, string busiSeq)
        {
            var model = this.bll.GetModel(id);
            if (model == null)
                model = new TBusinessAttributeModel()
                {
                    ID = -1,
                    unitSeq = unitSeq,
                    busiSeq = busiSeq
                };
            this.ViewBag.GreenChannel = model.isGreenChannel == 1;
            this.ViewBag.UnitName = this.unitBll.GetModel(p => p.unitSeq == model.unitSeq).unitName;
            this.ViewBag.BusiName = this.busiBll.GetModel(p => p.unitSeq == model.unitSeq && p.busiSeq == model.busiSeq).busiName;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitForm(TBusinessAttributeModel model)
        {
            model.isGreenChannel = bool.Parse(this.Request["greenChannel"]) ? 1 : 0;
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
            return Content("操作成功！");
        }

    }
}
