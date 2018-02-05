using Chloe.DbExpressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Chloe.MySql
{
    class JoinConditionExpressionParser : DbExpressionVisitor
    {
        static readonly JoinConditionExpressionParser _joinConditionExpressionParser = new JoinConditionExpressionParser();
        public static DbExpression Parse(DbExpression exp)
        {
            return exp.Accept(_joinConditionExpressionParser);
        }
        public override DbExpression Visit(DbEqualExpression exp)
        {
            /*
             * join 的条件不考虑 null 问题
             */

            DbExpression left = exp.Left;
            DbExpression right = exp.Right;

            MethodInfo method_Sql_Equals = UtilConstants.MethodInfo_Sql_Equals.MakeGenericMethod(left.Type);

            /* Sql.Equals(left, right) */
            DbMethodCallExpression left_equals_right = DbExpression.MethodCall(null, method_Sql_Equals, new List<DbExpression>(2) { left.Accept(this), right.Accept(this) });

            return left_equals_right;
        }
        public override DbExpression Visit(DbNotEqualExpression exp)
        {
            /*
             * join 的条件不考虑 null 问题
             */

            DbExpression left = exp.Left;
            DbExpression right = exp.Right;

            MethodInfo method_Sql_NotEquals = UtilConstants.MethodInfo_Sql_NotEquals.MakeGenericMethod(left.Type);

            /* Sql.Equals(left, right) */
            DbMethodCallExpression left_not_equals_right = DbExpression.MethodCall(null, method_Sql_NotEquals, new List<DbExpression>(2) { left.Accept(this), right.Accept(this) });

            return left_not_equals_right;
        }
    }
}
