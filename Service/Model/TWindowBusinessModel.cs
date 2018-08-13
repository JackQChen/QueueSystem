using Chloe.Entity;
using System;

namespace Model
{
    [Serializable]
    [Table("T_WindowBusiness")]
    public class TWindowBusinessModel : ModelBase
    {
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

