using Chloe.Entity;

namespace Model
{
    [Table("T_LedWindow")]
    public class TLedWindowModel
    {

        /// <summary>
        /// 
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int ControllerID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string WindowNumber { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string DisplayText { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Position { get; set; }

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

