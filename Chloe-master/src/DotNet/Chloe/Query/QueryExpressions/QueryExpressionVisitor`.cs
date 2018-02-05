using Chloe.Query.QueryExpressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chloe.Query.QueryExpressions
{
    abstract class QueryExpressionVisitor<T>
    {
        public virtual T Visit(RootQueryExpression exp)
        {
            throw new NotImplementedException();
        }
        public virtual T Visit(WhereExpression exp)
        {
            throw new NotImplementedException();
        }
        public virtual T Visit(SelectExpression exp)
        {
            throw new NotImplementedException();
        }
        public virtual T Visit(TakeExpression exp)
        {
            throw new NotImplementedException();
        }
        public virtual T Visit(SkipExpression exp)
        {
            throw new NotImplementedException();
        }
        public virtual T Visit(OrderExpression exp)
        {
            throw new NotImplementedException();
        }
        public virtual T Visit(AggregateQueryExpression exp)
        {
            throw new NotImplementedException();
        }
        public virtual T Visit(JoinQueryExpression exp)
        {
            throw new NotImplementedException();
        }
        public virtual T Visit(GroupingQueryExpression exp)
        {
            throw new NotImplementedException();
        }
        public virtual T Visit(DistinctExpression exp)
        {
            throw new NotImplementedException();
        }
    }
}
