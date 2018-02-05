using Chloe.Core;
using Chloe.DbExpressions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Chloe.SqlServer
{
    partial class SqlGenerator : DbExpressionVisitor<DbExpression>
    {
        static Dictionary<string, Action<DbAggregateExpression, SqlGenerator>> InitAggregateHandlers()
        {
            var aggregateHandlers = new Dictionary<string, Action<DbAggregateExpression, SqlGenerator>>();
            aggregateHandlers.Add("Count", Aggregate_Count);
            aggregateHandlers.Add("LongCount", Aggregate_LongCount);
            aggregateHandlers.Add("Sum", Aggregate_Sum);
            aggregateHandlers.Add("Max", Aggregate_Max);
            aggregateHandlers.Add("Min", Aggregate_Min);
            aggregateHandlers.Add("Average", Aggregate_Average);

            var ret = Utils.Clone(aggregateHandlers, StringComparer.Ordinal);
            return ret;
        }

        static void Aggregate_Count(DbAggregateExpression exp, SqlGenerator generator)
        {
            Aggregate_Count(generator);
        }
        static void Aggregate_LongCount(DbAggregateExpression exp, SqlGenerator generator)
        {
            Aggregate_LongCount(generator);
        }
        static void Aggregate_Sum(DbAggregateExpression exp, SqlGenerator generator)
        {
            Aggregate_Sum(generator, exp.Arguments.First(), exp.Method.ReturnType);
        }
        static void Aggregate_Max(DbAggregateExpression exp, SqlGenerator generator)
        {
            Aggregate_Max(generator, exp.Arguments.First(), exp.Method.ReturnType);
        }
        static void Aggregate_Min(DbAggregateExpression exp, SqlGenerator generator)
        {
            Aggregate_Min(generator, exp.Arguments.First(), exp.Method.ReturnType);
        }
        static void Aggregate_Average(DbAggregateExpression exp, SqlGenerator generator)
        {
            Aggregate_Average(generator, exp.Arguments.First(), exp.Method.ReturnType);
        }
    }
}
