using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WeChatService
{
    public enum StateInfo { Invalid, Error, Authorized, Unauthorized }

    public class StateList
    {
        static Dictionary<StateInfo, object> dic;

        public static Dictionary<StateInfo, object> State
        {
            get
            {
                if (dic == null)
                {
                    dic = new Dictionary<StateInfo, object>();
                    //dic.Add(StateInfo.Request, new { code = 1000, message = "请进行身份验证", key = "" });
                    dic.Add(StateInfo.Authorized, new { code = 1001, message = "客户端授权成功" });
                    dic.Add(StateInfo.Unauthorized, new { code = 1002, message = "未授权的客户端，请进行授权验证" });
                    dic.Add(StateInfo.Invalid, new { code = 2001, message = "无效的消息类型，请检查" });
                    dic.Add(StateInfo.Error, new { code = 3001, message = "错误的参数或操作，请检查" });
                }
                return dic;
            }
        }

    }
}
