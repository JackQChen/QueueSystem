using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chloe.DbExpressions
{
    public class DbNegateExpression : DbExpression
    {
        DbExpression _operand;

        public DbNegateExpression(Type type, DbExpression operand)
            : base(DbExpressionType.Negate, type)
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
