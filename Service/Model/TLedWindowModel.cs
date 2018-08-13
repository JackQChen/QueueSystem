using Chloe.Entity;
using System;

namespace Model
{
    [Serializable]
    [Table("T_LedWindow")]
    public class TLedWindowModel : ModelBase
    {
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
    }
}

