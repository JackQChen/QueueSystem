using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Chloe.Query
{
    class JoinQueryParameterExpressionReplacer : ExpressionVisitor
    {
        LambdaExpression _lambda;
        Expression[] _expressionSubstitutes;
        ParameterExpression _newParameterExpression;

        JoinQueryParameterExpressionReplacer(LambdaExpression lambda, Expression[] expressionSubstitutes, ParameterExpression newParameterExpression)
        {
            this._lambda = lambda;
            this._expressionSubstitutes = expressionSubstitutes;
            this._newParameterExpression = newParameterExpression;
        }

        public static LambdaExpression Replace(LambdaExpression lambda, Expression[] expressionSubstitutes, ParameterExpression newParameterExpression)
        {
            LambdaExpression ret = new JoinQueryParameterExpressionReplacer(lambda, expressionSubstitutes, newParameterExpression).Replace();
            return ret;
        }

        LambdaExpression Replace()
        {
            Expression lambdaBody = this._lambda.Body;
            Expression newBody = this.Visit(lambdaBody);

            Type delegateType = typeof(Func<,>).MakeGenericType(this._newParameterExpression.Type, lambdaBody.Type);
            LambdaExpression newLambda = Expression.Lambda(delegateType, newBody, this._newParameterExpression);
            return newLambda;
        }

        protected override Expression VisitParameter(ParameterExpression parameter)
        {
            int parameterIndex = this._lambda.Parameters.IndexOf(parameter);
            if (parameterIndex == -1)
            {
                return parameter;
            }

            return this._expressionSubstitutes[parameterIndex];
        }
    }
}
