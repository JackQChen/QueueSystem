using Chloe.Entity;
using System.ComponentModel;

namespace Model
{
    /// <summary>
    /// 用户实体
    /// </summary>
    [Table("T_User")]
    public class TUserModel
    {
        /// <summary>
        /// ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 所属单位
        /// </summary>
        public string unitSeq { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        public string Sex { get; set; }

        /// <summary>
        /// 工号
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// 照片
        /// </summary>
        public byte[] Photo { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 同步状态：0:同步新增 1：同步修改 2：已同步 3：已删除
        /// </summary>
        public int sysFlag { get; set; }

        /// <summary>
        /// 区域编号
        /// </summary>
        public int areaCode { get; set; }

        /// <summary>
        /// 编号
        /// </summary>
        public int areaId { get; set; }
    }
}

