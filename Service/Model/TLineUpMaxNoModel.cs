using Chloe.Entity;
using System;

namespace Model
{
    [Table("T_LineUpMaxNo")]
    public class TLineUpMaxNoModel
    {

        /// <summary>
        /// 
        /// </summary>
        public int id { get; set; }

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

