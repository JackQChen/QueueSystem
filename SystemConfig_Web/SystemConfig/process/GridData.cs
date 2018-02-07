using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebFramework;
using BLL;

namespace SystemConfig.process
{
    public class GridData : ProcessBase
    {
        public string GetGridData(string bllName)
        {
            var tp = Type.GetType("BLL." + bllName + "BLL,BLL");
            var obj = Activator.CreateInstance(tp);
            var strResult = tp.GetMethod("GetGridData").Invoke(obj, null).ToJsonString();
            return strResult;
        }
    }
}