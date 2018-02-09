using System;
using Chloe.Entity;

namespace Model
{
    /// <summary>
    /// 
    /// </summary>
    [Table("T_GetCard")]
    public class TGetCardModel
    {

        /// <summary>
        /// 
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// 身份证
        /// </summary>
        public string custCardId { get; set; }

        /// <summary>
        /// 业务流水号
        /// </summary>
        public string controlSeq { get; set; }

        /// <summary>
        /// 出证时间
        /// </summary>
        public DateTime outCardTime { get; set; }

        /// <summary>
        /// 办理单位编码
        /// </summary>
        public string unitSeq { get; set; }

        /// <summary>
        /// 办理单位
        /// </summary>
        public string unitName { get; set; }

        /// <summary>
        /// 业务类型编码
        /// </summary>
        public string busTypeSeq { get; set; }

        /// <summary>
        /// 业务类型名称
        /// </summary>
        public string busTypeName { get; set; }

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

