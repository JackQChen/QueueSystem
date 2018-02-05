using System;
using System.Data;
using System.Reflection;

namespace Chloe.DbExpressions
{
    [System.Diagnostics.DebuggerDisplay("Value = {Value}")]
    public class DbParameterExpression : DbExpression
    {
        object _value;
        Type _type;

        public DbParameterExpression(object value)
            : base(DbExpressionType.Parameter)
        {
            this._value = value;

            if (value != null)
                this._type = value.GetType();
            else
                this._type = UtilConstants.TypeOfObject;
        }

        public DbParameterExpression(object value, Type type)
            : base(DbExpressionType.Parameter)
        {
            Utils.CheckNull(type);

            if (value != null)
            {
                Type t = value.GetType();

                if (!type.IsAssignableFrom(t))
                    throw new ArgumentException();
            }

            this._value = value;
            this._type = type;
        }

        public override Type Type { get { return this._type; } }
        public object Value { get { return this._value; } }
        public DbType? DbType { get; set; }

        public override T Accept<T>(DbExpressionVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
