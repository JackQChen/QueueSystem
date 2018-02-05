using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Chloe.Extension;

namespace Chloe
{
    public static class QueryAsyncExtension
    {
        public static Task<T> FirstAsync<T>(this IQuery<T> q)
        {
            return Utils.MakeTask(q.First);
        }
        public static Task<T> FirstAsync<T>(this IQuery<T> q, Expression<Func<T, bool>> predicate)
        {
            return q.Where(predicate).FirstAsync();
        }
        public static Task<T> FirstOrDefaultAsync<T>(this IQuery<T> q)
        {
            return Utils.MakeTask(q.FirstOrDefault);
        }
        public static Task<T> FirstOrDefaultAsync<T>(this IQuery<T> q, Expression<Func<T, bool>> predicate)
        {
            return q.Where(predicate).FirstOrDefaultAsync();
        }

        public static Task<List<T>> ToListAsync<T>(this IQuery<T> q)
        {
            return Utils.MakeTask(q.ToList);
        }

        public static Task<bool> AnyAsync<T>(this IQuery<T> q)
        {
            return Utils.MakeTask(q.Any);
        }
        public static Task<bool> AnyAsync<T>(this IQuery<T> q, Expression<Func<T, bool>> predicate)
        {
            return q.Where(predicate).AnyAsync();
        }

        public static Task<int> CountAsync<T>(this IQuery<T> q)
        {
            return Utils.MakeTask(q.Count);
        }
        public static Task<long> LongCountAsync<T>(this IQuery<T> q)
        {
            return Utils.MakeTask(q.LongCount);
        }


        public static Task<TResult> MaxAsync<T, TResult>(this IQuery<T> q, Expression<Func<T, TResult>> selector)
        {
            return Utils.MakeTask(() => q.Max(selector));
        }
        public static Task<TResult> MinAsync<T, TResult>(this IQuery<T> q, Expression<Func<T, TResult>> selector)
        {
            return Utils.MakeTask(() => q.Min(selector));
        }

        public static Task<int> SumAsync<T>(this IQuery<T> q, Expression<Func<T, int>> selector)
        {
            return Utils.MakeTask(() => q.Sum(selector));
        }
        public static Task<int?> SumAsync<T>(this IQuery<T> q, Expression<Func<T, int?>> selector)
        {
            return Utils.MakeTask(() => q.Sum(selector));
        }
        public static Task<long> SumAsync<T>(this IQuery<T> q, Expression<Func<T, long>> selector)
        {
            return Utils.MakeTask(() => q.Sum(selector));
        }
        public static Task<long?> SumAsync<T>(this IQuery<T> q, Expression<Func<T, long?>> selector)
        {
            return Utils.MakeTask(() => q.Sum(selector));
        }
        public static Task<decimal> SumAsync<T>(this IQuery<T> q, Expression<Func<T, decimal>> selector)
        {
            return Utils.MakeTask(() => q.Sum(selector));
        }
        public static Task<decimal?> SumAsync<T>(this IQuery<T> q, Expression<Func<T, decimal?>> selector)
        {
            return Utils.MakeTask(() => q.Sum(selector));
        }
        public static Task<double> SumAsync<T>(this IQuery<T> q, Expression<Func<T, double>> selector)
        {
            return Utils.MakeTask(() => q.Sum(selector));
        }
        public static Task<double?> SumAsync<T>(this IQuery<T> q, Expression<Func<T, double?>> selector)
        {
            return Utils.MakeTask(() => q.Sum(selector));
        }
        public static Task<float> SumAsync<T>(this IQuery<T> q, Expression<Func<T, float>> selector)
        {
            return Utils.MakeTask(() => q.Sum(selector));
        }
        public static Task<float?> SumAsync<T>(this IQuery<T> q, Expression<Func<T, float?>> selector)
        {
            return Utils.MakeTask(() => q.Sum(selector));
        }

        public static Task<double> AverageAsync<T>(this IQuery<T> q, Expression<Func<T, int>> selector)
        {
            return Utils.MakeTask(() => q.Average(selector));
        }
        public static Task<double?> AverageAsync<T>(this IQuery<T> q, Expression<Func<T, int?>> selector)
        {
            return Utils.MakeTask(() => q.Average(selector));
        }
        public static Task<double> AverageAsync<T>(this IQuery<T> q, Expression<Func<T, long>> selector)
        {
            return Utils.MakeTask(() => q.Average(selector));
        }
        public static Task<double?> AverageAsync<T>(this IQuery<T> q, Expression<Func<T, long?>> selector)
        {
            return Utils.MakeTask(() => q.Average(selector));
        }
        public static Task<decimal> AverageAsync<T>(this IQuery<T> q, Expression<Func<T, decimal>> selector)
        {
            return Utils.MakeTask(() => q.Average(selector));
        }
        public static Task<decimal?> AverageAsync<T>(this IQuery<T> q, Expression<Func<T, decimal?>> selector)
        {
            return Utils.MakeTask(() => q.Average(selector));
        }
        public static Task<double> AverageAsync<T>(this IQuery<T> q, Expression<Func<T, double>> selector)
        {
            return Utils.MakeTask(() => q.Average(selector));
        }
        public static Task<double?> AverageAsync<T>(this IQuery<T> q, Expression<Func<T, double?>> selector)
        {
            return Utils.MakeTask(() => q.Average(selector));
        }
        public static Task<float> AverageAsync<T>(this IQuery<T> q, Expression<Func<T, float>> selector)
        {
            return Utils.MakeTask(() => q.Average(selector));
        }
        public static Task<float?> AverageAsync<T>(this IQuery<T> q, Expression<Func<T, float?>> selector)
        {
            return Utils.MakeTask(() => q.Average(selector));
        }
    }
}
