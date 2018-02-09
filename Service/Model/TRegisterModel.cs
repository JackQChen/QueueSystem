using System;
using Chloe.Entity;

namespace Model
{
    /// <summary>
    /// 
    /// </summary>
    [Table("T_Register")]
    public class TRegisterModel
    {

        /// <summary>
        /// 
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string custCardId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string userCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string paassword { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string mobilePhone { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime registerDateTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string registerAddress { get; set; }

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

