using Chloe.Entity;
using System;

namespace Model
{
    /// <summary>
    /// 
    /// </summary> 
    [Serializable]
    [Table("F_WindowLock")]
    public class FLockModel : ModelBase
    {
        /// <summary>
        /// 窗口号
        /// </summary>
        public string windowNo { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public int isOccupy { get; set; }
    }
}

