using Chloe.Core;
using Chloe.DbExpressions;
using Chloe.InternalExtensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Chloe.MySql
{
    partial class SqlGenerator : DbExpressionVisitor<DbExpression>
    {
        static Dictionary<string, Action<DbMethodCallExpression, SqlGenerator>> InitMethodHandlers()
        {
            var methodHandlers = new Dictionary<string, Action<DbMethodCallExpression, SqlGenerator>>();

            methodHandlers.Add("Equals", Method_Equals);
            methodHandlers.Add("NotEquals", Method_NotEquals);

            methodHandlers.Add("Trim", Method_Trim);
            methodHandlers.Add("TrimStart", Method_TrimStart);
            methodHandlers.Add("TrimEnd", Method_TrimEnd);
            methodHandlers.Add("StartsWith", Method_StartsWith);
            methodHandlers.Add("EndsWith", Method_EndsWith);
            methodHandlers.Add("ToUpper", Method_String_ToUpper);
            methodHandlers.Add("ToLower", Method_String_ToLower);
            methodHandlers.Add("Substring", Method_String_Substring);
            methodHandlers.Add("IsNullOrEmpty", Method_String_IsNullOrEmpty);
            methodHandlers.Add("Replace", Method_String_Replace);

            methodHandlers.Add("ToString", Method_ToString);
            methodHandlers.Add("Contains", Method_Contains);

            methodHandlers.Add("Count", Method_Count);
            methodHandlers.Add("LongCount", Method_LongCount);
            methodHandlers.Add("Sum", Method_Sum);
            methodHandlers.Add("Max", Method_Max);
            methodHandlers.Add("Min", Method_Min);
            methodHandlers.Add("Average", Method_Average);

            methodHandlers.Add("AddYears", Method_DateTime_AddYears);
            methodHandlers.Add("AddMonths", Method_DateTime_AddMonths);
            methodHandlers.Add("AddDays", Method_DateTime_AddDays);
            methodHandlers.Add("AddHours", Method_DateTime_AddHours);
            methodHandlers.Add("AddMinutes", Method_DateTime_AddMinutes);
            methodHandlers.Add("AddSeconds", Method_DateTime_AddSeconds);

            methodHandlers.Add("Parse", Method_Parse);

            methodHandlers.Add("NewGuid", Method_Guid_NewGuid);

            methodHandlers.Add("DiffYears", Method_DbFunctions_DiffYears);
            methodHandlers.Add("DiffMonths", Method_DbFunctions_DiffMonths);
            methodHandlers.Add("DiffDays", Method_DbFunctions_DiffDays);
            methodHandlers.Add("DiffHours", Method_DbFunctions_DiffHours);
            methodHandlers.Add("DiffMinutes", Method_DbFunctions_DiffMinutes);
            methodHandlers.Add("DiffSeconds", Method_DbFunctions_DiffSeconds);
            methodHandlers.Add("DiffMilliseconds", Method_DbFunctions_DiffMilliseconds);
            methodHandlers.Add("DiffMicroseconds", Method_DbFunctions_DiffMicroseconds);

            var ret = Utils.Clone(methodHandlers);
            return ret;
        }

        static void Method_Equals(DbMethodCallExpression exp, SqlGenerator generator)
        {
            MethodInfo method = exp.Method;
            if (method.DeclaringType == UtilConstants.TypeOfSql)
            {
                Method_Sql_Equals(exp, generator);
                return;
            }

            if (method.ReturnType != UtilConstants.TypeOfBoolean || method.IsStatic || method.GetParameters().Length != 1)
                throw UtilExceptions.NotSupportedMethod(method);

            DbExpression right = exp.Arguments[0];
            if (right.Type != exp.Object.Type)
            {
                right = DbExpression.Convert(right, exp.Object.Type);
            }

            DbExpression.Equal(exp.Object, right).Accept(generator);
        }
        static void Method_Sql_Equals(DbMethodCallExpression exp, SqlGenerator generator)
        {
            DbExpression left = exp.Arguments[0];
            DbExpression right = exp.Arguments[1];

            left = DbExpressionHelper.OptimizeDbExpression(left);
            right = DbExpressionHelper.OptimizeDbExpression(right);

            //明确 left right 其中一边一定为 null
            if (DbExpressionExtension.AffirmExpressionRetValueIsNull(right))
            {
                left.Accept(generator);
                generator._sqlBuilder.Append(" IS NULL");
                return;
            }

            if (DbExpressionExtension.AffirmExpressionRetValueIsNull(left))
            {
                right.Accept(generator);
                generator._sqlBuilder.Append(" IS NULL");
                return;
            }

            AmendDbInfo(left, right);

            left.Accept(generator);
            generator._sqlBuilder.Append(" = ");
            right.Accept(generator);

            return;
        }

        static void Method_NotEquals(DbMethodCallExpression exp, SqlGenerator generator)
        {
            MethodInfo method = exp.Method;
            if (method.DeclaringType != UtilConstants.TypeOfSql)
            {
                throw UtilExceptions.NotSupportedMethod(method);
            }

            DbExpression left = exp.Arguments[0];
            DbExpression right = exp.Arguments[1];

            left = DbExpressionHelper.OptimizeDbExpression(left);
            right = DbExpressionHelper.OptimizeDbExpression(right);

            //明确 left right 其中一边一定为 null
            if (DbExpressionExtension.AffirmExpressionRetValueIsNull(right))
            {
                left.Accept(generator);
                generator._sqlBuilder.Append(" IS NOT NULL");
                return;
            }

            if (DbExpressionExtension.AffirmExpressionRetValueIsNull(left))
            {
                right.Accept(generator);
                generator._sqlBuilder.Append(" IS NOT NULL");
                return;
            }

            AmendDbInfo(left, right);

            left.Accept(generator);
            generator._sqlBuilder.Append(" <> ");
            right.Accept(generator);

            return;
        }

        static void Method_Trim(DbMethodCallExpression exp, SqlGenerator generator)
        {
            EnsureMethod(exp, UtilConstants.MethodInfo_String_Trim);

            generator._sqlBuilder.Append("TRIM(");
            exp.Object.Accept(generator);
            generator._sqlBuilder.Append(")");
        }
        static void Method_TrimStart(DbMethodCallExpression exp, SqlGenerator generator)
        {
            EnsureMethod(exp, UtilConstants.MethodInfo_String_TrimStart);
            EnsureTrimCharArgumentIsSpaces(exp.Arguments[0]);

            generator._sqlBuilder.Append("LTRIM(");
            exp.Object.Accept(generator);
            generator._sqlBuilder.Append(")");
        }
        static void Method_TrimEnd(DbMethodCallExpression exp, SqlGenerator generator)
        {
            EnsureMethod(exp, UtilConstants.MethodInfo_String_TrimEnd);
            EnsureTrimCharArgumentIsSpaces(exp.Arguments[0]);

            generator._sqlBuilder.Append("RTRIM(");
            exp.Object.Accept(generator);
            generator._sqlBuilder.Append(")");
        }
        static void Method_StartsWith(DbMethodCallExpression exp, SqlGenerator generator)
        {
            EnsureMethod(exp, UtilConstants.MethodInfo_String_StartsWith);

            exp.Object.Accept(generator);
            generator._sqlBuilder.Append(" LIKE ");
            generator._sqlBuilder.Append("CONCAT(");
            exp.Arguments.First().Accept(generator);
            generator._sqlBuilder.Append(",'%'");
            generator._sqlBuilder.Append(")");
        }
        static void Method_EndsWith(DbMethodCallExpression exp, SqlGenerator generator)
        {
            EnsureMethod(exp, UtilConstants.MethodInfo_String_EndsWith);

            exp.Object.Accept(generator);

            generator._sqlBuilder.Append(" LIKE ");
            generator._sqlBuilder.Append("CONCAT(");
            generator._sqlBuilder.Append("'%',");
            exp.Arguments.First().Accept(generator);
            generator._sqlBuilder.Append(")");
        }
        static void Method_String_Contains(DbMethodCallExpression exp, SqlGenerator generator)
        {
            EnsureMethod(exp, UtilConstants.MethodInfo_String_Contains);

            exp.Object.Accept(generator);

            generator._sqlBuilder.Append(" LIKE ");
            generator._sqlBuilder.Append("CONCAT(");
            generator._sqlBuilder.Append("'%',");
            exp.Arguments.First().Accept(generator);
            generator._sqlBuilder.Append(",'%'");
            generator._sqlBuilder.Append(")");
        }
        static void Method_String_ToUpper(DbMethodCallExpression exp, SqlGenerator generator)
        {
            EnsureMethod(exp, UtilConstants.MethodInfo_String_ToUpper);

            generator._sqlBuilder.Append("UPPER(");
            exp.Object.Accept(generator);
            generator._sqlBuilder.Append(")");
        }
        static void Method_String_ToLower(DbMethodCallExpression exp, SqlGenerator generator)
        {
            EnsureMethod(exp, UtilConstants.MethodInfo_String_ToLower);

            generator._sqlBuilder.Append("LOWER(");
            exp.Object.Accept(generator);
            generator._sqlBuilder.Append(")");
        }
        static void Method_String_Substring(DbMethodCallExpression exp, SqlGenerator generator)
        {
            generator._sqlBuilder.Append("SUBSTRING(");
            exp.Object.Accept(generator);
            generator._sqlBuilder.Append(",");

            DbExpression arg1 = exp.Arguments[0];
            if (arg1.NodeType == DbExpressionType.Constant)
            {
                int startIndex = (int)(((DbConstantExpression)arg1).Value) + 1;
                generator._sqlBuilder.Append(startIndex.ToString());
            }
            else
            {
                arg1.Accept(generator);
                generator._sqlBuilder.Append(" + 1");
            }

            generator._sqlBuilder.Append(",");
            if (exp.Method == UtilConstants.MethodInfo_String_Substring_Int32)
            {
                var string_LengthExp = DbExpression.MemberAccess(UtilConstants.PropertyInfo_String_Length, exp.Object);
                string_LengthExp.Accept(generator);
            }
            else if (exp.Method == UtilConstants.MethodInfo_String_Substring_Int32_Int32)
            {
                exp.Arguments[1].Accept(generator);
            }
            else
                throw UtilExceptions.NotSupportedMethod(exp.Method);

            generator._sqlBuilder.Append(")");
        }
        static void Method_String_IsNullOrEmpty(DbMethodCallExpression exp, SqlGenerator generator)
        {
            EnsureMethod(exp, UtilConstants.MethodInfo_String_IsNullOrEmpty);

            DbExpression e = exp.Arguments.First();
            DbEqualExpression equalNullExpression = DbExpression.Equal(e, DbExpression.Constant(null, UtilConstants.TypeOfString));
            DbEqualExpression equalEmptyExpression = DbExpression.Equal(e, DbExpression.Constant(string.Empty));

            DbOrExpression orExpression = DbExpression.Or(equalNullExpression, equalEmptyExpression);

            DbCaseWhenExpression.WhenThenExpressionPair whenThenPair = new DbCaseWhenExpression.WhenThenExpressionPair(orExpression, DbConstantExpression.One);

            List<DbCaseWhenExpression.WhenThenExpressionPair> whenThenExps = new List<DbCaseWhenExpression.WhenThenExpressionPair>(1);
            whenThenExps.Add(whenThenPair);

            DbCaseWhenExpression caseWhenExpression = DbExpression.CaseWhen(whenThenExps, DbConstantExpression.Zero, UtilConstants.TypeOfBoolean);

            var eqExp = DbExpression.Equal(caseWhenExpression, DbConstantExpression.One);
            eqExp.Accept(generator);
        }
        static void Method_String_Replace(DbMethodCallExpression exp, SqlGenerator generator)
        {
            EnsureMethod(exp, UtilConstants.MethodInfo_String_Replace);

            generator._sqlBuilder.Append("REPLACE(");
            exp.Object.Accept(generator);
            generator._sqlBuilder.Append(",");
            exp.Arguments[0].Accept(generator);
            generator._sqlBuilder.Append(",");
            exp.Arguments[1].Accept(generator);
            generator._sqlBuilder.Append(")");
        }

        static void Method_ToString(DbMethodCallExpression exp, SqlGenerator generator)
        {
            if (exp.Method.Name != "ToString" && exp.Arguments.Count != 0)
            {
                throw UtilExceptions.NotSupportedMethod(exp.Method);
            }

            if (exp.Object.Type == UtilConstants.TypeOfString)
            {
                exp.Object.Accept(generator);
                return;
            }

            if (!NumericTypes.ContainsKey(exp.Object.Type.GetUnderlyingType()))
            {
                throw UtilExceptions.NotSupportedMethod(exp.Method);
            }

            DbConvertExpression c = DbExpression.Convert(exp.Object, UtilConstants.TypeOfString);
            c.Accept(generator);
        }
        static void Method_Contains(DbMethodCallExpression exp, SqlGenerator generator)
        {
            MethodInfo method = exp.Method;

            if (method.DeclaringType == UtilConstants.TypeOfString)
            {
                Method_String_Contains(exp, generator);
                return;
            }

            List<DbExpression> exps = new List<DbExpression>();
            IEnumerable values = null;
            DbExpression operand = null;

            Type declaringType = method.DeclaringType;

            if (typeof(IList).IsAssignableFrom(declaringType) || (declaringType.IsGenericType && typeof(ICollection<>).MakeGenericType(declaringType.GetGenericArguments()).IsAssignableFrom(declaringType)))
            {
                if (exp.Object.NodeType == DbExpressionType.SqlQuery)
                {
                    /* where Id in(select id from T) */

                    operand = exp.Arguments[0];
                    In(generator, (DbSqlQueryExpression)exp.Object, operand);
                    return;
                }

                DbMemberExpression memberExp = exp.Object as DbMemberExpression;

                if (memberExp == null || !memberExp.IsEvaluable())
                    throw new NotSupportedException(exp.ToString());

                values = DbExpressionExtension.Evaluate(memberExp) as IEnumerable; //Enumerable
                operand = exp.Arguments[0];
                goto constructInState;
            }
            if (method.IsStatic && declaringType == typeof(Enumerable) && exp.Arguments.Count == 2)
            {
                DbExpression arg0 = exp.Arguments[0];
                if (arg0.NodeType == DbExpressionType.SqlQuery)
                {
                    /* where Id in(select id from T) */

                    operand = exp.Arguments[1];
                    In(generator, (DbSqlQueryExpression)arg0, operand);
                    return;
                }

                DbMemberExpression memberExp = arg0 as DbMemberExpression;

                if (memberExp == null || !memberExp.IsEvaluable())
                    throw new NotSupportedException(exp.ToString());

                values = DbExpressionExtension.Evaluate(memberExp) as IEnumerable;
                operand = exp.Arguments[1];
                goto constructInState;
            }

            throw UtilExceptions.NotSupportedMethod(exp.Method);

            constructInState:
            foreach (object value in values)
            {
                if (value == null)
                    exps.Add(DbExpression.Constant(null, operand.Type));
                else
                {
                    Type valueType = value.GetType();
                    if (valueType.IsEnum)
                        valueType = Enum.GetUnderlyingType(valueType);

                    if (Utils.IsToStringableNumericType(valueType))
                        exps.Add(DbExpression.Constant(value));
                    else
                        exps.Add(DbExpression.Parameter(value));
                }
            }
            In(generator, exps, operand);
        }


        static void In(SqlGenerator generator, List<DbExpression> elementExps, DbExpression operand)
        {
            if (elementExps.Count == 0)
            {
                generator._sqlBuilder.Append("1=0");
                return;
            }

            operand.Accept(generator);
            generator._sqlBuilder.Append(" IN (");

            for (int i = 0; i < elementExps.Count; i++)
            {
                if (i > 0)
                    generator._sqlBuilder.Append(",");

                elementExps[i].Accept(generator);
            }

            generator._sqlBuilder.Append(")");

            return;
        }
        static void In(SqlGenerator generator, DbSqlQueryExpression sqlQuery, DbExpression operand)
        {
            operand.Accept(generator);
            generator._sqlBuilder.Append(" IN (");
            sqlQuery.Accept(generator);
            generator._sqlBuilder.Append(")");

            return;
        }

        static void Method_Count(DbMethodCallExpression exp, SqlGenerator generator)
        {
            EnsureMethodDeclaringType(exp, typeof(AggregateFunctions));
            Aggregate_Count(generator);
        }
        static void Method_LongCount(DbMethodCallExpression exp, SqlGenerator generator)
        {
            EnsureMethodDeclaringType(exp, typeof(AggregateFunctions));
            Aggregate_LongCount(generator);
        }
        static void Method_Sum(DbMethodCallExpression exp, SqlGenerator generator)
        {
            EnsureMethodDeclaringType(exp, typeof(AggregateFunctions));
            Aggregate_Sum(generator, exp.Arguments.First(), exp.Method.ReturnType);
        }
        static void Method_Max(DbMethodCallExpression exp, SqlGenerator generator)
        {
            EnsureMethodDeclaringType(exp, typeof(AggregateFunctions));
            Aggregate_Max(generator, exp.Arguments.First(), exp.Method.ReturnType);
        }
        static void Method_Min(DbMethodCallExpression exp, SqlGenerator generator)
        {
            EnsureMethodDeclaringType(exp, typeof(AggregateFunctions));
            Aggregate_Min(generator, exp.Arguments.First(), exp.Method.ReturnType);
        }
        static void Method_Average(DbMethodCallExpression exp, SqlGenerator generator)
        {
            EnsureMethodDeclaringType(exp, typeof(AggregateFunctions));
            Aggregate_Average(generator, exp.Arguments.First(), exp.Method.ReturnType);
        }


        static void Method_DateTime_AddYears(DbMethodCallExpression exp, SqlGenerator generator)
        {
            EnsureMethodDeclaringType(exp, UtilConstants.TypeOfDateTime);

            DbFunction_DATEADD(generator, "YEAR", exp);
        }
        static void Method_DateTime_AddMonths(DbMethodCallExpression exp, SqlGenerator generator)
        {
            EnsureMethodDeclaringType(exp, UtilConstants.TypeOfDateTime);

            DbFunction_DATEADD(generator, "MONTH", exp);
        }
        static void Method_DateTime_AddDays(DbMethodCallExpression exp, SqlGenerator generator)
        {
            EnsureMethodDeclaringType(exp, UtilConstants.TypeOfDateTime);

            DbFunction_DATEADD(generator, "DAY", exp);
        }
        static void Method_DateTime_AddHours(DbMethodCallExpression exp, SqlGenerator generator)
        {
            EnsureMethodDeclaringType(exp, UtilConstants.TypeOfDateTime);

            DbFunction_DATEADD(generator, "HOUR", exp);
        }
        static void Method_DateTime_AddMinutes(DbMethodCallExpression exp, SqlGenerator generator)
        {
            EnsureMethodDeclaringType(exp, UtilConstants.TypeOfDateTime);

            DbFunction_DATEADD(generator, "MINUTE", exp);
        }
        static void Method_DateTime_AddSeconds(DbMethodCallExpression exp, SqlGenerator generator)
        {
            EnsureMethodDeclaringType(exp, UtilConstants.TypeOfDateTime);

            DbFunction_DATEADD(generator, "SECOND", exp);
        }

        static void Method_Parse(DbMethodCallExpression exp, SqlGenerator generator)
        {
            if (exp.Arguments.Count != 1)
                throw UtilExceptions.NotSupportedMethod(exp.Method);

            DbExpression arg = exp.Arguments[0];
            if (arg.Type != UtilConstants.TypeOfString)
                throw UtilExceptions.NotSupportedMethod(exp.Method);

            Type retType = exp.Method.ReturnType;
            EnsureMethodDeclaringType(exp, retType);

            DbExpression e = DbExpression.Convert(arg, retType);
            if (retType == UtilConstants.TypeOfBoolean)
            {
                e.Accept(generator);
                generator._sqlBuilder.Append(" = ");
                DbConstantExpression.True.Accept(generator);
            }
            else
                e.Accept(generator);
        }

        static void Method_Guid_NewGuid(DbMethodCallExpression exp, SqlGenerator generator)
        {
            EnsureMethod(exp, UtilConstants.MethodInfo_Guid_NewGuid);

            generator._sqlBuilder.Append("UUID()");
        }


        static void Method_DbFunctions_DiffYears(DbMethodCallExpression exp, SqlGenerator generator)
        {
            EnsureMethod(exp, UtilConstants.MethodInfo_DbFunctions_DiffYears);

            DbFunction_DATEDIFF(generator, "YEAR", exp.Arguments[0], exp.Arguments[1]);
        }
        static void Method_DbFunctions_DiffMonths(DbMethodCallExpression exp, SqlGenerator generator)
        {
            EnsureMethod(exp, UtilConstants.MethodInfo_DbFunctions_DiffMonths);

            DbFunction_DATEDIFF(generator, "MONTH", exp.Arguments[0], exp.Arguments[1]);
        }
        static void Method_DbFunctions_DiffDays(DbMethodCallExpression exp, SqlGenerator generator)
        {
            EnsureMethod(exp, UtilConstants.MethodInfo_DbFunctions_DiffDays);

            DbFunction_DATEDIFF(generator, "DAY", exp.Arguments[0], exp.Arguments[1]);
        }
        static void Method_DbFunctions_DiffHours(DbMethodCallExpression exp, SqlGenerator generator)
        {
            EnsureMethod(exp, UtilConstants.MethodInfo_DbFunctions_DiffHours);

            DbFunction_DATEDIFF(generator, "HOUR", exp.Arguments[0], exp.Arguments[1]);
        }
        static void Method_DbFunctions_DiffMinutes(DbMethodCallExpression exp, SqlGenerator generator)
        {
            EnsureMethod(exp, UtilConstants.MethodInfo_DbFunctions_DiffMinutes);

            DbFunction_DATEDIFF(generator, "MINUTE", exp.Arguments[0], exp.Arguments[1]);
        }
        static void Method_DbFunctions_DiffSeconds(DbMethodCallExpression exp, SqlGenerator generator)
        {
            EnsureMethod(exp, UtilConstants.MethodInfo_DbFunctions_DiffSeconds);

            DbFunction_DATEDIFF(generator, "SECOND", exp.Arguments[0], exp.Arguments[1]);
        }
        static void Method_DbFunctions_DiffMilliseconds(DbMethodCallExpression exp, SqlGenerator generator)
        {
            EnsureMethod(exp, UtilConstants.MethodInfo_DbFunctions_DiffMilliseconds);

            throw UtilExceptions.NotSupportedMethod(exp.Method);
        }
        static void Method_DbFunctions_DiffMicroseconds(DbMethodCallExpression exp, SqlGenerator generator)
        {
            EnsureMethod(exp, UtilConstants.MethodInfo_DbFunctions_DiffMicroseconds);

            DbFunction_DATEDIFF(generator, "MICROSECOND", exp.Arguments[0], exp.Arguments[1]);
        }
    }
}
