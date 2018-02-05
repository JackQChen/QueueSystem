using Chloe.Entity;

namespace Model
{
    [Table("T_LedController")]
    public class TLedControllerModel
    {

        /// <summary>
        /// 
        /// </summary>
        public int ID { get; set; }

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

