using System;
using System.Reflection;

namespace Chloe.DbExpressions
{
    public class DbSubtractExpression : DbBinaryExpression
    {
        public DbSubtractExpression(Type type, DbExpression left, DbExpression right)
            : this(type, left, right, null)
        {

        }
        public DbSubtractExpression(Type type, DbExpression left, DbExpression right, MethodInfo method)
            : base(DbExpressionType.Subtract, type, left, right, method)
        {

        }

        public override T Accept<T>(DbExpressionVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }

}
