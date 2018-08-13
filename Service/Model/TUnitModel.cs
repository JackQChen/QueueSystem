using Chloe.Entity;
using System;

namespace Model
{
    /// <summary>
    /// 
    /// </summary> 
    [Serializable]
    [Table("T_Unit")]
    public class TUnitModel : ModelBase
    {
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
        /// 是否投资的部门
        /// </summary>
        [NotMapped]
        public bool isInvestment { get; set; }
    }
}

