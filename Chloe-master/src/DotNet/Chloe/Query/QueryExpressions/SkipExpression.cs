using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Chloe.Query.QueryExpressions;
using Chloe.Query.QueryState;
using Chloe.Query.Visitors;

namespace Chloe.Query.QueryExpressions
{
    class SkipExpression : QueryExpression
    {
        int _count;
        public SkipExpression(Type elementType, QueryExpression prevExpression, int count)
            : base(QueryExpressionType.Skip, elementType, prevExpression)
        {
            this.CheckInputCount(count);
            this._count = count;
        }

        public int Count
        {
            get { return _count; }
        }
        void CheckInputCount(int count)
        {
            if (count < 0)
            {
                throw new ArgumentException("count 小于 0");
            }
        }

        public override T Accept<T>(QueryExpressionVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
