using System;
using System.Data;

namespace Chloe
{
    public class DbParam
    {
        string _name;
        object _value;
        Type _type;
        ParamDirection _direction = ParamDirection.Input;

        public DbParam()
        {
        }
        public DbParam(string name, object value)
        {
            this.Name = name;
            this.Value = value;
        }
        public DbParam(string name, object value, Type type)
        {
            this.Name = name;
            this.Value = value;
            this.Type = type;
        }

        public string Name { get { return this._name; } set { this._name = value; } }
        public object Value
        {
            get
            {
                return this._value;
            }
            set
            {
                this._value = value;
                if (value != null)
                    this._type = value.GetType();
            }
        }
        public DbType? DbType { get; set; }
        public byte? Precision { get; set; }
        public byte? Scale { get; set; }
        public int? Size { get; set; }
        public Type Type { get { return this._type; } set { this._type = value; } }
        public ParamDirection Direction { get { return this._direction; } set { this._direction = value; } }
        public IDbDataParameter ExplicitParameter { get; set; }/* 如果设置了该自定义参数，框架内部就会忽视 DbParam 类的其他属性，使用该属性值 */

        public static DbParam Create<T>(string name, T value)
        {
            var param = new DbParam(name, value);
            if (value == null)
                param.Type = typeof(T);
            return param;
        }
        public static DbParam Create(string name, object value)
        {
            return new DbParam(name, value);
        }
        public static DbParam Create(string name, object value, Type type)
        {
            return new DbParam(name, value, type);
        }
    }
}
