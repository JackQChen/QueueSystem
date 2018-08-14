using Chloe.Entity;
using System;

namespace Model
{
    /// <summary>
    /// 
    /// </summary> 
    [Serializable]
    [Table("T_ScreenConfig")]
    public class TScreenConfigModel : ModelBase
    {
        /// <summary>
        /// 屏名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 屏IP
        /// </summary>
        public string IP { get; set; }

        /// <summary>
        /// 屏配置
        /// </summary>
        public string Config { get; set; }

    }
}

