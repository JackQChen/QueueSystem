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

    }
}

