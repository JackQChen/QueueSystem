using System;
using Chloe.Entity;

namespace Model
{
    /// <summary>
    /// 
    /// </summary>
    [Table("T_Register")]
    public class TRegisterModel
    {

        /// <summary>
        /// 
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string custCardId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string userCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string paassword { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string mobilePhone { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime registerDateTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string registerAddress { get; set; }

    }
}

