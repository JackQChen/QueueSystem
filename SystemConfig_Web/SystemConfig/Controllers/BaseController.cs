using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SystemConfig.Controllers
{
    public abstract class BaseController : Controller
    {
        public string AreaNo
        {
            get
            {
                return System.Web.HttpContext.Current.Session["areaNo"].ToString();
            }
        }
    }
}
