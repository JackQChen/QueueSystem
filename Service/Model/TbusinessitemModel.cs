using Chloe.Entity;

namespace Model
{
    [Table("T_BusinessItem")]
    public class TBusinessItemModel : ModelBase
    {
        /// <summary>
        /// 
        /// </summary>
        public string unitSeq { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string busiSeq { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string itemName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string remark { get; set; }

    }
}

