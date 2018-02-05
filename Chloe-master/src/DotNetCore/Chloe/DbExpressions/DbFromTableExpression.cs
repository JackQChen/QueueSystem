using System.Collections.Generic;

namespace Chloe.DbExpressions
{
    public class DbFromTableExpression : DbMainTableExpression
    {
        public DbFromTableExpression(DbTableSegment table)
            : base(DbExpressionType.FromTable, table)
        {
        }
        public override T Accept<T>(DbExpressionVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
