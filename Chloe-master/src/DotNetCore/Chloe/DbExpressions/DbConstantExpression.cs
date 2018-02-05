using System;
using System.Reflection;

namespace Chloe.DbExpressions
{
    public class DbConstantExpression : DbExpression
    {
        object _value;
        Type _type;

        public static readonly DbConstantExpression Null = new DbConstantExpression(null);
        public static readonly DbConstantExpression StringEmpty = new DbConstantExpression(string.Empty);
        public static readonly DbConstantExpression One = new DbConstantExpression(1);
        public static readonly DbConstantExpression Zero = new DbConstantExpression(0);
        public static readonly DbConstantExpression True = new DbConstantExpression(true);
        public static readonly DbConstantExpression False = new DbConstantExpression(false);

        public DbConstantExpression(object value)
            : base(DbExpressionType.Constant)
        {
            this._value = value;

            if (value != null)
                this._type = value.GetType();
            else
                this._type = UtilConstants.TypeOfObject;
        }

        public DbConstantExpression(object value, Type type)
            : base(DbExpressionType.Constant)
        {
            Utils.CheckNull(type);

            if (value != null)
            {
                Type t = value.GetType();

                if (!type.GetTypeInfo().IsAssignableFrom(t))
                    throw new ArgumentException();
            }

            this._value = value;
            this._type = type;
        }

        public override Type Type { get { return this._type; } }
        public object Value { get { return this._value; } }

        public override T Accept<T>(DbExpressionVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
