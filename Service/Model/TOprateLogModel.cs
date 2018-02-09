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

        /// <summary>
        /// 同步状态：0:同步新增 1：同步修改 2：已同步 3：已删除
        /// </summary>
        public int sysFlag { get; set; }

        /// <summary>
        /// 区域编号
        /// </summary>
        public int areaCode { get; set; }

        /// <summary>
        /// 编号
        /// </summary>
        public int areaId { get; set; }
    }
}

