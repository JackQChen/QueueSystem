using Chloe.Entity;
using System;

namespace Model
{
    [Serializable]
    [Table("B_LineUpMaxNo")]
    public class BLineUpMaxNoModel : ModelBase
    {

        /// <summary>
        /// 部门编码
        /// </summary>
        public string unitSeq { get; set; }

        /// <summary>
        /// 业务编码
        /// </summary>
        public string busiSeq { get; set; }

        /// <summary>
        /// 区域编码
        /// </summary>
        public string areaSeq { get; set; }

        /// <summary>
        /// 日期
        /// </summary>
        public DateTime lineDate { get; set; }

        /// <summary>
        /// 当天最大号
        /// </summary>
        public int maxNo { get; set; }

    }
}

