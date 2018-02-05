using System;
using System.Reflection;

namespace Chloe.DbExpressions
{
    public class DbConvertExpression : DbExpression
    {
        DbExpression _operand;

        public DbConvertExpression(Type type, DbExpression operand)
            : base(DbExpressionType.Convert, type)
        {
            this._operand = operand;
        }
        public DbExpression Operand { get { return this._operand; } }

        public override T Accept<T>(DbExpressionVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
