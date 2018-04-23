using Chloe.Entity;

namespace Model
{
    [Table("T_CallState")]
    public class TCallStateModel
    {

        /// <summary>
        /// 窗口号
        /// </summary>
        [Column(IsPrimaryKey = true)]
        public string windowNo { get; set; }

        /// <summary>
        /// 窗口状态(0：默认 1：呼叫  2：已评价 3：暂停服务)
        /// </summary>
        public int workState { get; set; }

        /// <summary>
        /// 当前已叫号ID
        /// </summary>
        public int callId { get; set; }

        /// <summary>
        /// 当前挂起ID
        /// </summary>
        public int hangId { get; set; }

        /// <summary>
        /// 暂停前的状态
        /// </summary>
        public int pauseState { get; set; }

        /// <summary>
        /// 当前票号
        /// </summary>
        public string ticketNo { get; set; }

        /// <summary>
        /// '重呼次数'
        /// </summary>
        public int reCallTimes { get; set; }

    }
}

