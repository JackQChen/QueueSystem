using Chloe.Entity;
using System;

namespace Model
{
    /// <summary>
    /// 
    /// </summary> 
    [Serializable]
    [Table("T_WindowArea")]
    public class TWindowAreaModel : ModelBase
    {
        /// <summary>
        /// 区域名称
        /// </summary>
        public string areaName { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string remark { get; set; }

    }
}

