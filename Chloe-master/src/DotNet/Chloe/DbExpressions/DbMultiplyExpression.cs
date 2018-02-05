using System;
using System.Reflection;

namespace Chloe.DbExpressions
{
    public class DbMultiplyExpression : DbBinaryExpression
    {
        public DbMultiplyExpression(Type type, DbExpression left, DbExpression right)
            : this(type, left, right, null)
        {

        }
        public DbMultiplyExpression(Type type, DbExpression left, DbExpression right, MethodInfo method)
            : base(DbExpressionType.Multiply, type, left, right, method)
        {

        }

        public override T Accept<T>(DbExpressionVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }

}
