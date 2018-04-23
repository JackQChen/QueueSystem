using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CallClient
{
    public enum WorkState
    {
        /// <summary>
        /// 默认初始
        /// </summary>
        Defalt = 0,
        /// <summary>
        /// 呼叫
        /// </summary>
        Call = 1,
        /// <summary>
        /// 转移
        /// </summary>
        Transfer,
        /// <summary>
        /// 已评价
        /// </summary>
        Evaluate = 2,
        /// <summary>
        /// 暂停服务
        /// </summary>
        PauseService = 3

    }
}
