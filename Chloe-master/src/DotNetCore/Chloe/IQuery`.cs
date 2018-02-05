using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Chloe
{
    public interface IQuery<T> : IQuery
    {
        IQuery<T> AsTracking();
        IEnumerable<T> AsEnumerable();
        IQuery<TResult> Select<TResult>(Expression<Func<T, TResult>> selector);

        IQuery<T> Where(Expression<Func<T, bool>> predicate);
        IOrderedQuery<T> OrderBy<K>(Expression<Func<T, K>> keySelector);
        IOrderedQuery<T> OrderByDesc<K>(Expression<Func<T, K>> keySelector);
        IQuery<T> Skip(int count);
        IQuery<T> Take(int count);
        IQuery<T> TakePage(int pageNumber, int pageSize);

        IGroupingQuery<T> GroupBy<K>(Expression<Func<T, K>> keySelector);
        IQuery<T> Distinct();


        IJoiningQuery<T, TOther> Join<TOther>(JoinType joinType, Expression<Func<T, TOther, bool>> on);
        IJoiningQuery<T, TOther> Join<TOther>(IQuery<TOther> q, JoinType joinType, Expression<Func<T, TOther, bool>> on);

        IJoiningQuery<T, TOther> InnerJoin<TOther>(Expression<Func<T, TOther, bool>> on);
        IJoiningQuery<T, TOther> LeftJoin<TOther>(Expression<Func<T, TOther, bool>> on);
        IJoiningQuery<T, TOther> RightJoin<TOther>(Expression<Func<T, TOther, bool>> on);
        IJoiningQuery<T, TOther> FullJoin<TOther>(Expression<Func<T, TOther, bool>> on);

        IJoiningQuery<T, TOther> InnerJoin<TOther>(IQuery<TOther> q, Expression<Func<T, TOther, bool>> on);
        IJoiningQuery<T, TOther> LeftJoin<TOther>(IQuery<TOther> q, Expression<Func<T, TOther, bool>> on);
        IJoiningQuery<T, TOther> RightJoin<TOther>(IQuery<TOther> q, Expression<Func<T, TOther, bool>> on);
        IJoiningQuery<T, TOther> FullJoin<TOther>(IQuery<TOther> q, Expression<Func<T, TOther, bool>> on);

        T First();
        T First(Expression<Func<T, bool>> predicate);
        T FirstOrDefault();
        T FirstOrDefault(Expression<Func<T, bool>> predicate);
        List<T> ToList();

        bool Any();
        bool Any(Expression<Func<T, bool>> predicate);

        int Count();
        long LongCount();

        TResult Max<TResult>(Expression<Func<T, TResult>> selector);
        TResult Min<TResult>(Expression<Func<T, TResult>> selector);

        int Sum(Expression<Func<T, int>> selector);
        int? Sum(Expression<Func<T, int?>> selector);
        long Sum(Expression<Func<T, long>> selector);
        long? Sum(Expression<Func<T, long?>> selector);
        decimal Sum(Expression<Func<T, decimal>> selector);
        decimal? Sum(Expression<Func<T, decimal?>> selector);
        double Sum(Expression<Func<T, double>> selector);
        double? Sum(Expression<Func<T, double?>> selector);
        float Sum(Expression<Func<T, float>> selector);
        float? Sum(Expression<Func<T, float?>> selector);

        double Average(Expression<Func<T, int>> selector);
        double? Average(Expression<Func<T, int?>> selector);
        double Average(Expression<Func<T, long>> selector);
        double? Average(Expression<Func<T, long?>> selector);
        decimal Average(Expression<Func<T, decimal>> selector);
        decimal? Average(Expression<Func<T, decimal?>> selector);
        double Average(Expression<Func<T, double>> selector);
        double? Average(Expression<Func<T, double?>> selector);
        float Average(Expression<Func<T, float>> selector);
        float? Average(Expression<Func<T, float?>> selector);
    }
}
