using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Chloe.Query
{
    static class QueryHelper
    {
        public static Expression<TDelegate> ComposePredicate<TDelegate>(List<LambdaExpression> filterPredicates, Expression[] expressionSubstitutes, ParameterExpression parameter)
        {
            Expression predicateBody = null;
            foreach (LambdaExpression filterPredicate in filterPredicates)
            {
                var body = JoinQueryParameterExpressionReplacer.Replace(filterPredicate, expressionSubstitutes, parameter).Body;
                if (predicateBody == null)
                {
                    predicateBody = body;
                }
                else
                {
                    predicateBody = Expression.AndAlso(predicateBody, body);
                }
            }

            Expression<TDelegate> predicate = Expression.Lambda<TDelegate>(predicateBody, parameter);

            return predicate;
        }

        public static Expression[] MakeExpressionSubstitutes(Type joinResultType, ParameterExpression parameter)
        {
            int joinResultTypeGenericArgumentCount = joinResultType.GetGenericArguments().Length;
            Expression[] expressionSubstitutes = new Expression[joinResultTypeGenericArgumentCount];
            for (int i = 0; i < joinResultTypeGenericArgumentCount; i++)
            {
                expressionSubstitutes[i] = Expression.MakeMemberAccess(parameter, joinResultType.GetProperty("Item" + (i + 1).ToString()));
            }

            return expressionSubstitutes;
        }
    }
}
