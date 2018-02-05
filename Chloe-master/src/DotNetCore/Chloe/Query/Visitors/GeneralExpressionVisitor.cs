using Chloe.Core.Visitors;
using Chloe.DbExpressions;
using Chloe.Extensions;
using Chloe.Infrastructure;
using Chloe.Query.Mapping;
using Chloe.Query.QueryState;
using Chloe.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Chloe.Query.Visitors
{
    internal class GeneralExpressionVisitor : ExpressionVisitorBase
    {
        static List<string> AggregateMethods;

        LambdaExpression _lambda;
        ScopeParameterDictionary _scopeParameters;
        KeyDictionary<string> _scopeTables;

        static GeneralExpressionVisitor()
        {
            List<string> aggregateMethods = new List<string>();
            aggregateMethods.Add("Count");
            aggregateMethods.Add("LongCount");
            aggregateMethods.Add("Max");
            aggregateMethods.Add("Min");
            aggregateMethods.Add("Sum");
            aggregateMethods.Add("Average");
            aggregateMethods.TrimExcess();
            AggregateMethods = aggregateMethods;
        }

        public GeneralExpressionVisitor(LambdaExpression lambda, ScopeParameterDictionary scopeParameters, KeyDictionary<string> scopeTables) : this(scopeParameters, scopeTables)
        {
            this._lambda = lambda;
        }
        GeneralExpressionVisitor(ScopeParameterDictionary scopeParameters, KeyDictionary<string> scopeTables)
        {
            this._scopeParameters = scopeParameters;
            this._scopeTables = scopeTables;
        }
        public static DbExpression ParseLambda(LambdaExpression lambda, ScopeParameterDictionary scopeParameters, KeyDictionary<string> scopeTables)
        {
            GeneralExpressionVisitor visitor = new GeneralExpressionVisitor(scopeParameters, scopeTables);
            return visitor.Visit(lambda);
        }

        IMappingObjectExpression FindMoe(ParameterExpression exp)
        {
            IMappingObjectExpression moe = this._scopeParameters.Get(exp);
            return moe;
        }

        protected override DbExpression VisitLambda(LambdaExpression lambda)
        {
            this._lambda = lambda;
            return base.VisitLambda(lambda);
        }

        protected override DbExpression VisitMemberAccess(MemberExpression exp)
        {
            ParameterExpression p;
            if (ExpressionExtension.IsDerivedFromParameter(exp, out p))
            {
                IMappingObjectExpression moe = this.FindMoe(p);
                return moe.GetDbExpression(exp);
            }

            if (IsComeFrom_First_Or_FirstOrDefault(exp))
            {
                DbExpression dbExpression = this.Process_MemberAccess_Which_Link_First_Or_FirstOrDefault(exp);
                return dbExpression;
            }
            else
            {
                return base.VisitMemberAccess(exp);
            }
        }

        protected override DbExpression VisitParameter(ParameterExpression exp)
        {
            //TODO 只支持 MappingFieldExpression 类型，即类似 q.Select(a=> a.Id).Where(a=> a > 0) 这种情况，也就是 ParameterExpression.Type 为基本映射类型。

            if (MappingTypeSystem.IsMappingType(exp.Type))
            {
                IMappingObjectExpression moe = this.FindMoe(exp);
                MappingFieldExpression mfe = (MappingFieldExpression)moe;
                return mfe.Expression;
            }
            else
                throw new NotSupportedException(exp.ToString());
        }

        protected override DbExpression VisitMethodCall(MethodCallExpression exp)
        {
            /*
             * First()，FirstOrDefault() --> (select top 1 T.Name from T)，如果 First()，FirstOrDefault() 的泛型参数不是映射类型，则抛出不支持异常，First()，FirstOrDefault() 转成 q.Take(1)，然后转成 DbSqlQueryExpression，最后包装成 DbSubQueryExpression，return
             * query.Any() --> query.Select()，生成一个 DbSqlQueryExpression， 然后包装成 DbExistsExpression，DbExistsExpression.Query = DbSqlQueryExpression，泛型参数必须是映射类型
             * query.ToList() --> 将 query 对象转成 DbSqlQueryExpression，泛型参数必须是映射类型，然后 DbMethodCallExpression.Object = DbSqlQueryExpression
             */

            if (exp.Object != null && IsIQueryType(exp.Object.Type))
            {
                string methodName = exp.Method.Name;
                if (methodName == "First" || methodName == "FirstOrDefault")
                {
                    return this.Process_MethodCall_First_Or_FirstOrDefault(exp);
                }
                else if (methodName == "ToList")
                {
                    EnsureIsMappingType(exp.Type.GetGenericArguments()[0], exp);
                    return this.ConvertToDbSqlQueryExpression(exp.Object, exp.Type);
                }
                else if (methodName == "Any")
                {
                    /* query.Any() --> exists 查询 */
                    exp = OptimizeCondition(exp);
                    DbSqlQueryExpression sqlQuery = this.ConvertToDbSqlQueryExpression(exp.Object, exp.Type);
                    return new DbExistsExpression(sqlQuery);
                }
                else if (AggregateMethods.Contains(methodName))
                {
                    return this.Process_MethodCall_Aggregate(exp);
                }
            }

            return base.VisitMethodCall(exp);
        }

        DbExpression Process_MethodCall_First_Or_FirstOrDefault(MethodCallExpression exp)
        {
            /*
             * query.First() || query.First(a=> a.Id==1) || query.FirstOrDefault() || query.FirstOrDefault(a=> a.Id==1)
             */

            exp = OptimizeCondition(exp);

            EnsureIsMappingType(exp.Type, exp);

            var takeMethod = exp.Object.Type.GetMethod("Take");
            var takeMethodExp = Expression.Call(exp.Object, takeMethod, Expression.Constant(1));

            return this.ConvertToDbSubQueryExpression(takeMethodExp, exp.Type);
        }
        DbExpression Process_MethodCall_Aggregate(MethodCallExpression exp)
        {
            MethodInfo calledAggregateMethod = exp.Method;
            List<Expression> arguments = exp.Arguments.Select(a => a.StripQuotes()).ToList();

            /* 
             * query.Sum(a=> a.Price) --> query.CreateAggregateQuery(calledAggregateMethod, arguments)
             * exp.Object 为 Query<T> 对象，获取 Query<T>.CreateAggregateQuery<TResult>(MethodInfo method, List<Expression> arguments) 
             */

            var query = ExpressionEvaluator.Evaluate(exp.Object);
            var queryType = query.GetType();
            MethodInfo method_Query_CreateAggregateQuery = queryType.GetMethod("CreateAggregateQuery");
            method_Query_CreateAggregateQuery = method_Query_CreateAggregateQuery.MakeGenericMethod(calledAggregateMethod.ReturnType);

            /* query.CreateAggregateQuery(calledAggregateMethod, arguments) */
            var e = Expression.Call(Expression.Constant(query), method_Query_CreateAggregateQuery, Expression.Constant(calledAggregateMethod), Expression.Constant(arguments));
            return this.ConvertToDbSubQueryExpression(e, calledAggregateMethod.ReturnType);
        }
        DbExpression Process_MemberAccess_Which_Link_First_Or_FirstOrDefault(MemberExpression exp)
        {
            /* 
                * 判断是不是 First().xx，FirstOrDefault().xx 之类的
                * First().Name： 泛型参数如果不是复杂类型，则转成 Select(a=> a.Name).First()
                * First().xx.Name：  如果 xx 是复杂类型，则转成 Select(a=> a.xx.Name).First()
                * First().xx.Name.Length：  如果 xx 是复杂类型，则转成 Select(a=> a.xx.Name).First().Length
                */

            // 分割
            MethodCallExpression methodCall = null;
            Stack<MemberExpression> memberExps = new Stack<MemberExpression>();

            Expression e = exp;
            while (e.NodeType == ExpressionType.MemberAccess)
            {
                MemberExpression memberExpression = (MemberExpression)e;
                memberExps.Push(memberExpression);
                e = memberExpression.Expression;
            }

            methodCall = (MethodCallExpression)e;
            methodCall = OptimizeCondition(methodCall);

            if (!MappingTypeSystem.IsMappingType(methodCall.Type))
            {
                /*
                 * query.First().xx.Name.Length --> query.Select(a=> a.xx.Name).First().Length
                 */

                ParameterExpression parameter = Expression.Parameter(methodCall.Type, "a");
                Expression selectorBody = parameter;
                while (memberExps.Count != 0)
                {
                    MemberExpression memberExpression = memberExps.Pop();
                    selectorBody = Expression.MakeMemberAccess(parameter, memberExpression.Member);

                    if (MappingTypeSystem.IsMappingType(selectorBody.Type))
                        break;
                }

                Type delegateType = typeof(Func<,>).MakeGenericType(parameter.Type, selectorBody.Type);
                LambdaExpression selector = Expression.Lambda(delegateType, selectorBody, parameter);
                var selectMethod = methodCall.Object.Type.GetMethod("Select");
                selectMethod = selectMethod.MakeGenericMethod(selectorBody.Type);
                var selectMethodCall = Expression.Call(methodCall.Object, selectMethod, Expression.Quote(selector)); /* query.Select(a=> a.xx.Name) */

                var sameNameMethod = selectMethodCall.Type.GetMethod(methodCall.Method.Name, Type.EmptyTypes);
                var sameNameMethodCall = Expression.Call(selectMethodCall, sameNameMethod); /* query.Select(a=> a.xx.Name).First() */

                methodCall = sameNameMethodCall;
            }

            DbExpression dbExpression = this.Visit(methodCall);
            while (memberExps.Count != 0)
            {
                MemberExpression memberExpression = memberExps.Pop();
                dbExpression = DbExpression.MemberAccess(memberExpression.Member, dbExpression);
            }

            return dbExpression;
        }

        DbSubQueryExpression ConvertToDbSubQueryExpression(Expression exp, Type resultType)
        {
            DbSqlQueryExpression sqlQueryExpression = ConvertToDbSqlQueryExpression(exp, resultType);
            DbSubQueryExpression subQueryExpression = new DbSubQueryExpression(sqlQueryExpression);
            return subQueryExpression;
        }
        DbSqlQueryExpression ConvertToDbSqlQueryExpression(Expression exp, Type resultType)
        {
            if (!IsIQueryType(exp.Type))
            {
                throw new NotSupportedException(exp.ToString());
            }

            QueryBase query = ExpressionEvaluator.Evaluate(exp) as QueryBase;
            IQueryState qs = QueryExpressionVisitor.VisitQueryExpression(query.QueryExpression, this._scopeParameters, this._scopeTables);
            MappingData mappingData = qs.GenerateMappingData();

            DbSqlQueryExpression sqlQueryExpression = mappingData.SqlQuery.Update(resultType);
            return sqlQueryExpression;
        }

        static MethodCallExpression OptimizeCondition(MethodCallExpression exp)
        {
            if (exp.Arguments.Count == 1)
            {
                /* 
                 * query.First(a=> a.Id==1) --> query.Where(a=> a.Id==1).First()
                 * query.Any(a=> a.Id==1) --> query.Where(a=> a.Id==1).Any()
                 */
                Type queryType = exp.Object.Type;
                var whereMethod = queryType.GetMethod("Where");
                var whereMethodCall = Expression.Call(exp.Object, whereMethod, exp.Arguments[0]);
                var sameNameMethod = queryType.GetMethod(exp.Method.Name, Type.EmptyTypes);
                var sameNameMethodCall = Expression.Call(whereMethodCall, sameNameMethod);
                exp = sameNameMethodCall;
            }
            return exp;
        }

        static void EnsureIsMappingType(Type type, MethodCallExpression exp)
        {
            if (!MappingTypeSystem.IsMappingType(type))
                throw new NotSupportedException(string.Format("The generic parameter type of method {0} must be mapping type.", exp.Method.Name));
        }
        static bool IsIQueryType(Type type)
        {
            if (type.IsGenericType == false)
                return false;

            Type queryType = typeof(IQuery<>);
            if (queryType == type.GetGenericTypeDefinition())
                return true;

            Type implementedInterface = type.GetInterface("IQuery`1");
            if (implementedInterface == null)
                return false;
            implementedInterface = implementedInterface.GetGenericTypeDefinition();
            return queryType == implementedInterface;
        }

        static bool IsComeFrom_First_Or_FirstOrDefault(MemberExpression exp)
        {
            Expression e = exp;
            while (e.NodeType == ExpressionType.MemberAccess)
            {
                e = (e as MemberExpression).Expression;
                if (e == null)
                    return false;
            }

            if (e.NodeType != ExpressionType.Call)
            {
                return false;
            }

            MethodCallExpression methodCall = (MethodCallExpression)e;
            if (methodCall.Method.Name != "First" && methodCall.Method.Name != "FirstOrDefault")
                return false;

            return IsIQueryType(methodCall.Object.Type);
        }
        static bool IsInQuery(MethodCallExpression exp)
        {
            /* query.ToList().Contains(a.Id) */

            if (exp.Method.Name != "Contains")
                return false;

            if (exp.Object == null)
                return false;

            if (exp.Object.NodeType != ExpressionType.Call)
                return false;

            MethodCallExpression methodCall = (MethodCallExpression)exp.Object;

            return methodCall.Method.Name == "ToList" && IsIQueryType(methodCall.Object.Type);
        }
    }
}
