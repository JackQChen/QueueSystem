using Chloe.Entity;
using System;

namespace Model
{
    [Serializable]
    [Table("T_BusinessAttribute")]
    public class TBusinessAttributeModel : ModelBase
    {
        /// <summary>
        /// 
        /// </summary>
        public string unitSeq { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string busiSeq { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string busiCode { get; set; }

        /// <summary>
        /// 处理开始时间(时分秒)
        /// </summary>
        public string handleStartTime { get; set; }

        /// <summary>
        /// 处理结束时间(时分秒)
        /// </summary>
        public string handleEndTime { get; set; }

        /// <summary>
        /// 单日排队最多大数
        /// </summary>
        public int lineUpMax { get; set; }

        /// <summary>
        /// 等候人数预警值
        /// </summary>
        public int lineUpWarningMax { get; set; }

        /// <summary>
        /// 该业务票号前缀
        /// </summary>
        public string ticketPrefix { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string remark { get; set; }

        /// <summary>
        /// 出票时间段（一个时间区间中间用逗号，多个时间段用竖线| 时间需要标准时间格式[HH:mm:ss]）
        /// </summary>
        public string timeInterval { get; set; }

        /// <summary>
        /// 按业务类型限制出票（需要和timeInterval分段对应，中间用逗号分隔）
        /// </summary>
        public string ticketRestriction { get; set; }


        /// <summary>
        /// 是否绿色通道
        /// </summary>
        public int isGreenChannel { get; set; }

    }
}

