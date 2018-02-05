using Chloe.Entity;
using System;

namespace Model
{
    [Table("T_OprateLog")]
    public class TOprateLogModel
    {

        /// <summary>
        /// 
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// 客户端标识
        /// </summary>
        public string oprateFlag { get; set; }

        /// <summary>
        /// 客户端类型
        /// </summary>
        public string oprateType { get; set; }

        /// <summary>
        /// 日志日期
        /// </summary>
        public DateTime oprateTime { get; set; }

        /// <summary>
        /// 客户端分级类别
        /// </summary>
        public string oprateClassifyType { get; set; }

        /// <summary>
        /// 日志内容
        /// </summary>
        public string oprateLog { get; set; }

    }
}

