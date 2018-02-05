using Chloe.Descriptors;
using Chloe.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Chloe.Extension
{
    class IgnoreFieldsResolver
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fieldsLambdaExpression">a => new object[] { a.Name, a.Age }</param>
        /// <returns></returns>
        public static List<string> Resolve(LambdaExpression fieldsLambdaExpression)
        {
            ParameterExpression parameterExpression = fieldsLambdaExpression.Parameters[0];

            NewArrayExpression newArrayExpression = fieldsLambdaExpression.Body as NewArrayExpression;
            if (newArrayExpression == null)
                throw new NotSupportedException(fieldsLambdaExpression.ToString());

            Type entityType = parameterExpression.Type;
            TypeDescriptor typeDescriptor = TypeDescriptor.GetDescriptor(entityType);

            List<string> fields = new List<string>(newArrayExpression.Expressions.Count);

            foreach (var item in newArrayExpression.Expressions)
            {
                MemberExpression memberExp = ExpressionExtension.StripConvert(item) as MemberExpression;
                if (memberExp == null)
                    throw new NotSupportedException(item.ToString());

                if (memberExp.Expression != parameterExpression)
                    throw new NotSupportedException(item.ToString());

                fields.Add(memberExp.Member.Name);
            }

            return fields;
        }
    }
}
