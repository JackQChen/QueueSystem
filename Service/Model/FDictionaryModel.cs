using System;
using Chloe.Entity;

namespace Model
{
    /// <summary>
    /// 
    /// </summary> 
    [Serializable]
    [Table("F_Dictionary")]
    public class FDictionaryModel : ModelBase
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 分组
        /// </summary>
        public int Group { get; set; }

        /// <summary>
        /// 项目值
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
}

