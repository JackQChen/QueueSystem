using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SystemConfig.Controllers
{
    public class HomeController : Controller
    {
        static Dictionary<string, string> dicMenu;
        //
        // GET: /Home/

        public ActionResult Index()
        {
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
            }
            return View(dicMenu);
        }

    }
}
