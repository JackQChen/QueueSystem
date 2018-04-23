using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WeChatService
{
    public enum StateInfo { Invalid, Authorized, Unauthorized }

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
                    dic.Add(StateInfo.Unauthorized, new { code = 999, message = "无效的消息类型，请检查" });
                    //1000 请求验证
                    dic.Add(StateInfo.Authorized, new { code = 1001, message = "客户端授权成功" });
                    dic.Add(StateInfo.Unauthorized, new { code = 1002, message = "未授权的客户端，请进行授权验证" });
                }
                return dic;
            }
        }

    }
}
