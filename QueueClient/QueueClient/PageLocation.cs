using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QueueClient
{
    public enum PageLocation
    {
        /// <summary>
        /// 主页面
        /// </summary>
        Main,
        /// <summary>
        /// 办事刷身份证
        /// </summary>
        WorkReadCard,
        /// <summary>
        /// 办事身份证输入页面
        /// </summary>
        WorkInputIdCard,
        /// <summary>
        /// 办事电话号码输入页面
        /// </summary>
        WorkInputPhone,
        /// <summary>
        /// 预约列表选择
        /// </summary>
        WorkAppointment,
        /// <summary>
        /// 办事部门选择页面
        /// </summary>
        WorkSelectUnit,
        /// <summary>
        /// 办事业务选择页面
        /// </summary>
        WorkSelectBusy,

        /// <summary>
        /// 领卡刷身份证
        /// </summary>
        GetCardReadCard,
        /// <summary>
        /// 领卡输入身份证
        /// </summary>
        GetCardInputCard,

        /// <summary>
        /// 评价刷身份证页面
        /// </summary>
        EvaluateReadCard,
        /// <summary>
        /// 评价输入身份证
        /// </summary>
        EvaluateInputCard,
        /// <summary>
        /// 评价页面
        /// </summary>
        Evaluate,
        ///// <summary>
        ///// 咨询二级页面
        ///// </summary>
        //ConsultSecond,
        /// <summary>
        /// 咨询部门选择页面
        /// </summary>
        ConsultSelectUnit,
        /// <summary>
        /// 咨询业务选择页面
        /// </summary>
        ConsultSelectBusy,
        /// <summary>
        /// 退出页面
        /// </summary>
        Exit,
    }
}
