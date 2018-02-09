using Chloe.Entity;
using System.ComponentModel;
using System;

namespace Model
{
    /// <summary>
    /// 窗口用户对应
    /// </summary>
    [Table("T_WindowUser")]
    public class TWindowUserModel
    {

        /// <summary>
        /// ID
        /// </summary> 
        public int ID { get; set; }

        /// <summary>
        /// 窗口ID
        /// </summary>
        public int WindowID { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public int UserID { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDateTime { get; set; }

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

