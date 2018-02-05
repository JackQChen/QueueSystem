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

    }
}

