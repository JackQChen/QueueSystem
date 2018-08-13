using Chloe.Entity;
using System;

namespace Model
{
    [Serializable]
    [Table("T_LedController")]
    public class TLedControllerModel : ModelBase
    {
        /// <summary>
        /// 
        /// </summary>
        public string IP { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Port { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string DeviceAddress { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }

    }
}

