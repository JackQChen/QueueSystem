using Chloe.Query.QueryExpressions;
using Chloe.Query.Mapping;
using Chloe.Query.Visitors;
using Chloe.DbExpressions;
using System.Linq.Expressions;
using System.Collections.Generic;
using Chloe.Utility;

namespace Chloe.Query.QueryState
{
    interface IQueryState
    {
        MappingData GenerateMappingData();

        ResultElement ToFromQueryResult();
        JoinQueryResult ToJoinQueryResult(JoinType joinType, LambdaExpression conditionExpression, ScopeParameterDictionary scopeParameters, KeyDictionary<string> scopeTables, string tableAlias);

        IQueryState Accept(WhereExpression exp);
        IQueryState Accept(OrderExpression exp);
        IQueryState Accept(SelectExpression exp);
        IQueryState Accept(SkipExpression exp);
        IQueryState Accept(TakeExpression exp);
        IQueryState Accept(AggregateQueryExpression exp);
        IQueryState Accept(GroupingQueryExpression exp);
        IQueryState Accept(DistinctExpression exp);
    }
}
