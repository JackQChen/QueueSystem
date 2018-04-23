using Chloe.Entity;

namespace Model
{
    /// <summary>
    /// 
    /// </summary>
    [Table("T_Unit")]
    public class TUnitModel
    {

        /// <summary>
        /// 
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// 单位编码
        /// </summary>
        public string unitSeq { get; set; }

        /// <summary>
        /// 单位名称
        /// </summary>
        public string unitName { get; set; }

        /// <summary>
        /// 排序号
        /// </summary>
        public int orderNum { get; set; }

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

        /// <summary>
        /// 是否投资的部门
        /// </summary>
        [NotMapped]
        public bool isInvestment { get; set; }
    }
}

