using Chloe.Query.QueryState;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Chloe.Query.QueryExpressions
{
    class AggregateQueryExpression : QueryExpression
    {
        MethodInfo _method;
        ReadOnlyCollection<Expression> _arguments;

        public AggregateQueryExpression(QueryExpression prevExpression, MethodInfo method, IList<Expression> arguments)
            : base(QueryExpressionType.Aggregate, method.ReturnType, prevExpression)
        {
            this._method = method;
            this._arguments = new ReadOnlyCollection<Expression>(arguments);
        }

        public MethodInfo Method { get { return this._method; } }
        public ReadOnlyCollection<Expression> Arguments { get { return this._arguments; } }


        public override T Accept<T>(QueryExpressionVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
