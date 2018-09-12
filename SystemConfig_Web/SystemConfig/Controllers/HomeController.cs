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
        static Dictionary<int, Dictionary<string, string>> menu;
        //
        // GET: /Home/

        public ActionResult Index()
        {
            var strKey = this.Request["areaNo"];
            if (string.IsNullOrEmpty(strKey))
                Response.Redirect("/Error/Error500");
            else
                System.Web.HttpContext.Current.Session["areaNo"] = strKey;
            if (menu == null)
            {
                menu = new Dictionary<int, Dictionary<string, string>>();

                menu[0] = new Dictionary<string, string>();
                menu[0]["SystemConfig/TUnit"] = "单位管理";
                menu[0]["SystemConfig/TBusiness"] = "业务维护";
                menu[0]["SystemConfig/TWindowArea"] = "区域维护";
                menu[0]["SystemConfig/TUser"] = "用户维护";
                menu[0]["SystemConfig/TWindow"] = "窗口维护";
                menu[0]["SystemConfig/TBusinessAttribute"] = "业务属性扩展";
                menu[0]["SystemConfig/TBusinessItem"] = "业务事项维护";
                menu[0]["SystemConfig/TLedController"] = "LED控制卡维护";
                menu[0]["SystemConfig/TLedWindow"] = "LED屏幕维护";
                menu[0]["SystemConfig/TScreenConfig"] = "综合屏配置";

                menu[1] = new Dictionary<string, string>();
                menu[1]["Statistics/SWaitTime"] = "平均等候时间统计";
                menu[1]["Statistics/SSatisfaction"] = "满意度统计";
            }
            return View(menu);
        }

    }
}
