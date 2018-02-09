using Chloe.Entity;
using System;

namespace Model
{
    [Table("T_Call")]
    public class TCallModel
    {

        /// <summary>
        /// 
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// 处理业务编号
        /// </summary>
        public string handleId { get; set; }

        /// <summary>
        /// 身份证号码
        /// </summary>
        public string idCard { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public string qNmae { get; set; }

        /// <summary>
        /// 排队编号
        /// </summary>
        public int qId { get; set; }

        /// <summary>
        /// 票号
        /// </summary>
        public string ticketNumber { get; set; }

        /// <summary>
        /// 出票日期
        /// </summary>
        public DateTime ticketTime { get; set; }

        /// <summary>
        /// 业务编号
        /// </summary>
        public string busiSeq { get; set; }

        /// <summary>
        /// 部门编号
        /// </summary>
        public string unitSeq { get; set; }

        /// <summary>
        /// 窗口号
        /// </summary>
        public string windowNumber { get; set; }

        /// <summary>
        /// 状态（已弃号-1,、已叫号0、已完成1、已转移2、已挂起3）
        /// </summary>
        public int state { get; set; }

        /// <summary>
        /// 当前操作员
        /// </summary>
        public string windowUser { get; set; }

        /// <summary>
        /// 处理时间
        /// </summary>
        public DateTime handleTime { get; set; }

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

