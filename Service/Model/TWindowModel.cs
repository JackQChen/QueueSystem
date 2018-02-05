using Chloe.Entity;

namespace Model
{
    /// <summary>
    /// 
    /// </summary>
    [Table("T_Window")]
    public class TWindowModel
    {

        /// <summary>
        /// 
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 窗口名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 窗口号
        /// </summary>
        public string Number { get; set; }

        /// <summary>
        /// 窗口业务类型
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 窗口状态
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// 呼叫器编号
        /// </summary>
        public int CallNumber { get; set; }

        /// <summary>
        /// 区域名称
        /// </summary>
        public int AreaName { get; set; }

    }
}

