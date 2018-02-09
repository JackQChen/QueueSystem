using System;
using Chloe.Entity;

namespace Model
{
    /// <summary>
    /// 预约实体
    /// </summary>
    [Table("T_Appointment")]
    public class TAppointmentModel
    {

        /// <summary>
        /// 
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// 类型： 0 预约 1 申办
        /// </summary>
        public int type { get; set; }
        /// <summary>
        /// 身份证号
        /// </summary>
        public string custCardId { get; set; }

        /// <summary>
        /// 预约号/
        /// </summary>
        public string reserveSeq { get; set; }

        /// <summary>
        /// 预约业务编号
        /// </summary>
        public string busiCode { get; set; }

        /// <summary>
        /// 预约业务名称
        /// </summary>
        public string busiName { get; set; }

        /// <summary>
        /// 用户姓名
        /// </summary>
        public string userName { get; set; }

        /// <summary>
        /// 证件类型
        /// </summary>
        public string paperType { get; set; }

        /// <summary>
        /// 证件号码
        /// </summary>
        public string paperCode { get; set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        public string mobilePhone { get; set; }

        /// <summary>
        /// 企业名称
        /// </summary>
        public string comName { get; set; }

        /// <summary>
        /// 预约日期
        /// </summary>
        public DateTime reserveDate { get; set; }

        /// <summary>
        /// 预约开始时间
        /// </summary>
        public DateTime reserveStartTime { get; set; }

        /// <summary>
        /// 预约结束时间
        /// </summary>
        public DateTime reserveEndTime { get; set; }

        /// <summary>
        /// 预约状态
        /// </summary>
        public string isTake { get; set; }

        /// <summary>
        /// 排队号
        /// </summary>
        public string queueCode { get; set; }

        /// <summary>
        /// 事件流水号
        /// </summary>
        public string approveSeq { get; set; }

        /// <summary>
        /// 事项名称
        /// </summary>
        public string approveName { get; set; }

        /// <summary>
        /// 单位流水号
        /// </summary>
        public string unitCode { get; set; }

        /// <summary>
        /// 单位名称
        /// </summary>
        public string unitName { get; set; }

        /// <summary>
        /// 申办业务流水号
        /// </summary>
        public string controlSeq { get; set; }

        /// <summary>
        /// 申办企业名称
        /// </summary>
        public string custName { get; set; }

        /// <summary>
        /// 预约类型（0：后预约 1：先预约）
        /// </summary>
        public int appType { get; set; }

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

        [NotMapped]
        public bool isCheck { get; set; }
    }
}

