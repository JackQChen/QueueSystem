using System;
using Chloe.Entity;

namespace Model
{
    [Serializable]
    public class ModelBase
    {
        [Column(IsPrimaryKey = true)]
        public int ID { get; set; }
        [Column(IsPrimaryKey = true)]
        public string AreaNo { get; set; }
    }
}
