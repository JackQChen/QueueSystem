using Chloe.Entity;

namespace Model
{
    [Table("T_WindowBusiness")]
    public class TWindowBusinessModel
    {

        /// <summary>
        /// 
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int WindowID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string unitSeq { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string busiSeq { get; set; }

        /// <summary>
        /// 优先级
        /// </summary>
        public int priorityLevel { get; set; }

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

