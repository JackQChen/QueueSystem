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

    }
}

