using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;

namespace SystemConfig.Controllers
{
    public class HomeController : Controller
    {
        static Dictionary<string, string> dicMenu;
        //
        // GET: /Home/

        public ActionResult Index()
        {
            var strKey = this.Request["areaNo"];
            if (string.IsNullOrEmpty(strKey))
                Response.Redirect("/Error/Error500");
            else
                System.Web.HttpContext.Current.Session["areaNo"] = strKey;
            if (dicMenu == null)
            {
                dicMenu = new Dictionary<string, string>();
                dicMenu.Add("TUnit", "单位管理");
                dicMenu.Add("TBusiness", "业务维护");
                dicMenu.Add("TWindowArea", "区域维护");
                dicMenu.Add("TUser", "用户维护");
                dicMenu.Add("TWindow", "窗口维护");
                dicMenu.Add("TBusinessAttribute", "业务属性扩展");
                dicMenu.Add("TLedController", "LED控制卡维护");
                dicMenu.Add("TLedWindow", "LED屏幕维护");
                dicMenu.Add("TScreenConfig", "综合屏配置");
            }
            return View(dicMenu);
        }

    }
}
