using System;
using System.Reflection;

namespace Chloe.DbExpressions
{
    public class DbDivideExpression : DbBinaryExpression
    {
        public DbDivideExpression(Type type, DbExpression left, DbExpression right)
            : this(type, left, right, null)
        {

        }
        public DbDivideExpression(Type type, DbExpression left, DbExpression right, MethodInfo method)
            : base(DbExpressionType.Divide, type, left, right, method)
        {

        }

        public override T Accept<T>(DbExpressionVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }

}
