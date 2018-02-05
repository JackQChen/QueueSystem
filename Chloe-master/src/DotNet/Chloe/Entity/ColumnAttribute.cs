using System;
using System.Data;

namespace Chloe.Entity
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class ColumnAttribute : Attribute
    {
        DbType _dbType = (DbType)(-1);/* -1=Unspecified */
        int _length = -1;/* -1=Unspecified */
        public ColumnAttribute() { }
        public ColumnAttribute(string name)
        {
            this.Name = name;
        }
        public string Name { get; set; }
        public bool IsPrimaryKey { get; set; }
        /// <summary>
        /// -1 表示未指定确切的值，用该属性的时候务必做 -1 判断。
        /// </summary>
        public DbType DbType { get { return this._dbType; } set { this._dbType = value; } }
        /// <summary>
        /// -1 表示未指定确切的值，用该属性的时候务必做 -1 判断。
        /// </summary>
        public int Length { get { return this._length; } set { this._length = value; } }

        public bool HasDbType()
        {
            return (int)this._dbType != -1;
        }
        public DbType? GetDbType()
        {
            if (this.HasDbType())
                return this._dbType;

            return null;
        }

        public bool HasLength()
        {
            return this._length != -1;
        }
        public int? GetLength()
        {
            if (this.HasLength())
                return this._length;

            return null;
        }
    }
}
