using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chloe.Query.QueryExpressions
{
    class DistinctExpression : QueryExpression
    {
        public DistinctExpression(Type elementType, QueryExpression prevExpression)
            : base(QueryExpressionType.Distinct, elementType, prevExpression)
        {
        }
        public override T Accept<T>(QueryExpressionVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
