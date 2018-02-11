using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebFramework;
using BLL;

namespace SystemConfig.process
{
    public class EditorData : ProcessBase
    {
        public EditorData()
        {
        }

        public string GetEditorData(string bllName, int id)
        {
            var tp = Type.GetType("BLL." + bllName + "BLL,BLL");
            dynamic bll = Activator.CreateInstance(tp);
            object model = bll.GetModel(id);
            if (model == null)
                return new { }.ToJsonString();
            return model.ToJsonString();
        }

        public string SaveEditorData(string bllName, string formPara)
        {
            var tp = Type.GetType("BLL." + bllName + "BLL,BLL");
            dynamic bll = Activator.CreateInstance(tp);
            var dicPara = FormParams.GetFormParams(formPara);
            object model = bll.GetModel(Convert.ToInt32(dicPara["id"][0]));
            if (model == null)
                tp.GetMethod("Insert").Invoke(bll, new object[] { model });
            else
            {
                FormParams.UpdateEntityByFormParams(model, dicPara);
                tp.GetMethod("Update").Invoke(bll, new object[] { model });
            }
            return new { result = "保存成功！" }.ToJsonString();
        }
    }
}