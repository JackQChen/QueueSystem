using Chloe.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Chloe.Extension
{
    class ParameterTwoExpressionReplacer : ExpressionVisitor
    {
        LambdaExpression _lambda;
        object _replaceObj;
        Expression _expToReplace = null;

        ParameterTwoExpressionReplacer(LambdaExpression lambda, object replaceObj)
        {
            this._lambda = lambda;
            this._replaceObj = replaceObj;
        }

        public static LambdaExpression Replace(LambdaExpression lambda, object replaceObj)
        {
            LambdaExpression ret = new ParameterTwoExpressionReplacer(lambda, replaceObj).Replace();
            return ret;
        }

        LambdaExpression Replace()
        {
            Expression lambdaBody = this._lambda.Body;
            Expression newBody = this.Visit(lambdaBody);

            ParameterExpression firstParameterExp = this._lambda.Parameters[0];
            Type delegateType = typeof(Func<,>).MakeGenericType(firstParameterExp.Type, typeof(bool));
            LambdaExpression newLambda = Expression.Lambda(delegateType, newBody, firstParameterExp);
            return newLambda;
        }

        protected override Expression VisitParameter(ParameterExpression parameter)
        {
            Expression ret = parameter;
            if (parameter == this._lambda.Parameters[1])
            {
                if (this._expToReplace == null)
                    this._expToReplace = ExpressionExtension.MakeWrapperAccess(this._replaceObj, parameter.Type);
                ret = this._expToReplace;
            }

            return ret;
        }
    }
}
