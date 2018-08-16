using System.Web.Mvc;
using BLL;
using Model;
using Newtonsoft.Json;
using SystemConfig.Controllers;

namespace SystemConfig.Areas.SystemConfig.Controllers
{
    public class TScreenConfigController : BaseController
    {
        TScreenConfigBLL bll;

        public TScreenConfigController()
        {
            this.bll = new TScreenConfigBLL("MySQL", this.AreaNo);
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetGridData(Pagination p)
        {
            var data = new
            {
                rows = bll.GetModelList()
            };
            return Content(JsonConvert.SerializeObject(data));
        }

        public ActionResult Form(int id)
        {
            var model = this.bll.GetModel(id);
            if (model == null)
            {
                model = new TScreenConfigModel()
                {
                    ID = -1,
                    Config = JsonConvert.SerializeObject(new
                    {
                        winArea = "1",
                        columnSize = "10%,25%,10%,10%,45%",
                        scrollText = "取号后请在休息区等候，注意留意电视大屏和语音叫号！",
                        scrollSpeed = 10,
                        scrollColor = "#88F092",
                        scrollSize = 100,
                        lineHeight = 50,
                        fontSize = 80,
                        ticketColor = "#FFFF00",
                        winColor = "#88F092",
                        otherColor = "#EEEEEE",
                        oddBackColor = "#3D3C3C",
                        evenBackColor = "#207EC0"
                    })
                };
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitForm(TScreenConfigModel model)
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
            return Content("操作成功！");
        }

    }
}
