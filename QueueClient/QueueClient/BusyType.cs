using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QueueClient
{
    public enum BusyType
    {
        /// <summary>
        /// 初始化，默认值
        /// </summary>
        Default,
        /// <summary>
        /// 办事
        /// </summary>
        Work,
        /// <summary>
        /// 领卡
        /// </summary>
        GetCard,
        /// <summary>
        /// 评价
        /// </summary>
        Evaluate,
        /// <summary>
        /// 咨询
        /// </summary>
        Consult
    }
}
