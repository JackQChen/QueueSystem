using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chloe.DbExpressions
{
    public class DbBitAndExpression : DbBinaryExpression
    {
        public DbBitAndExpression(Type type, DbExpression left, DbExpression right)
            : base(DbExpressionType.BitAnd, type, left, right)
        {
        }

        public override T Accept<T>(DbExpressionVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
