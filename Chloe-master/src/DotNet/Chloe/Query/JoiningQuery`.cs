using Chloe.Core;
using Chloe.DbExpressions;
using Chloe.Infrastructure;
using Chloe.Query.QueryExpressions;
using Chloe.Query.QueryState;
using Chloe.Query.Visitors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Chloe.Query
{
    class JoiningQuery<T1, T2> : IJoiningQuery<T1, T2>
    {
        DbContext _dbContext;

        QueryBase _rootQuery;
        List<JoiningQueryInfo> _joinedQueries;
        List<LambdaExpression> _filterPredicates;

        public DbContext DbContext { get { return this._dbContext; } }
        public QueryBase RootQuery { get { return this._rootQuery; } }
        public List<JoiningQueryInfo> JoinedQueries { get { return this._joinedQueries; } }
        public List<LambdaExpression> FilterPredicates { get { return this._filterPredicates; } }

        public JoiningQuery(Query<T1> q1, Query<T2> q2, JoinType joinType, Expression<Func<T1, T2, bool>> on)
        {
            this._dbContext = q1.DbContext;
            this._rootQuery = q1;
            this._joinedQueries = new List<JoiningQueryInfo>(1) { new JoiningQueryInfo(q2, joinType, on) };

            this._filterPredicates = new List<LambdaExpression>();
        }
        public JoiningQuery(JoiningQuery<T1, T2> jq, LambdaExpression filterPredicate)
        {
            this._dbContext = jq._dbContext;
            this._rootQuery = jq._rootQuery;
            this._joinedQueries = Utils.Clone(jq._joinedQueries);

            this._filterPredicates = Utils.CloneAndAppendOne(jq._filterPredicates, filterPredicate);
        }

        public IJoiningQuery<T1, T2> Where(Expression<Func<T1, T2, bool>> predicate)
        {
            return new JoiningQuery<T1, T2>(this, predicate);
        }

        public IJoiningQuery<T1, T2, T3> Join<T3>(JoinType joinType, Expression<Func<T1, T2, T3, bool>> on)
        {
            return this.Join<T3>(this._dbContext.Query<T3>(), joinType, on);
        }
        public IJoiningQuery<T1, T2, T3> Join<T3>(IQuery<T3> q, JoinType joinType, Expression<Func<T1, T2, T3, bool>> on)
        {
            return new JoiningQuery<T1, T2, T3>(this, (Query<T3>)q, joinType, on);
        }

        public IJoiningQuery<T1, T2, T3> InnerJoin<T3>(Expression<Func<T1, T2, T3, bool>> on)
        {
            return this.InnerJoin<T3>(this._dbContext.Query<T3>(), on);
        }
        public IJoiningQuery<T1, T2, T3> LeftJoin<T3>(Expression<Func<T1, T2, T3, bool>> on)
        {
            return this.LeftJoin<T3>(this._dbContext.Query<T3>(), on);
        }
        public IJoiningQuery<T1, T2, T3> RightJoin<T3>(Expression<Func<T1, T2, T3, bool>> on)
        {
            return this.RightJoin<T3>(this._dbContext.Query<T3>(), on);
        }
        public IJoiningQuery<T1, T2, T3> FullJoin<T3>(Expression<Func<T1, T2, T3, bool>> on)
        {
            return this.FullJoin<T3>(this._dbContext.Query<T3>(), on);
        }

        public IJoiningQuery<T1, T2, T3> InnerJoin<T3>(IQuery<T3> q, Expression<Func<T1, T2, T3, bool>> on)
        {
            return this.Join<T3>(q, JoinType.InnerJoin, on);
        }
        public IJoiningQuery<T1, T2, T3> LeftJoin<T3>(IQuery<T3> q, Expression<Func<T1, T2, T3, bool>> on)
        {
            return this.Join<T3>(q, JoinType.LeftJoin, on);
        }
        public IJoiningQuery<T1, T2, T3> RightJoin<T3>(IQuery<T3> q, Expression<Func<T1, T2, T3, bool>> on)
        {
            return this.Join<T3>(q, JoinType.RightJoin, on);
        }
        public IJoiningQuery<T1, T2, T3> FullJoin<T3>(IQuery<T3> q, Expression<Func<T1, T2, T3, bool>> on)
        {
            return this.Join<T3>(q, JoinType.FullJoin, on);
        }
        public IQuery<TResult> Select<TResult>(Expression<Func<T1, T2, TResult>> selector)
        {
            if (this._filterPredicates.Count == 0)
            {
                JoinQueryExpression e1 = new JoinQueryExpression(typeof(TResult), this._rootQuery.QueryExpression, this._joinedQueries, selector);
                return new Query<TResult>(this.DbContext, e1, this._rootQuery.TrackEntity);
            }

            /*
             * q.LeftJoin<City>((user, city) => user.CityId == city.Id).Where((user, city) => user.Name == "lu").Select((user, city) => new { user, city })
             * 转换成：
             * q.LeftJoin<City>((user, city) => user.CityId == city.Id)
             *  .Select((user, city) => new JoinResult<T1, T2>() { Item1 = user, Item2 = city })
             *  .Where(a => a.Item1.Name == "lu")
             *  .Select(a => new { user = a.user, city = a.city })
             */

            Type joinResultType = typeof(JoinResult<T1, T2>);

            Expression<Func<T1, T2, JoinResult<T1, T2>>> joinResultSelector = (t1, t2) => new JoinResult<T1, T2>() { Item1 = t1, Item2 = t2 };
            JoinQueryExpression e = new JoinQueryExpression(joinResultType, this._rootQuery.QueryExpression, this._joinedQueries, joinResultSelector);
            IQuery<JoinResult<T1, T2>> q = new Query<JoinResult<T1, T2>>(this.DbContext, e, this._rootQuery.TrackEntity);


            ParameterExpression parameter = Expression.Parameter(joinResultType, "a");
            Expression[] expressionSubstitutes = QueryHelper.MakeExpressionSubstitutes(joinResultType, parameter);

            Expression<Func<JoinResult<T1, T2>, bool>> predicate = QueryHelper.ComposePredicate<Func<JoinResult<T1, T2>, bool>>(this._filterPredicates, expressionSubstitutes, parameter);
            var q1 = q.Where(predicate);

            Expression<Func<JoinResult<T1, T2>, TResult>> selector2 = JoinQueryParameterExpressionReplacer.Replace(selector, expressionSubstitutes, parameter) as Expression<Func<JoinResult<T1, T2>, TResult>>;

            var ret = q1.Select(selector2);
            return ret;
        }
    }

    class JoiningQuery<T1, T2, T3> : IJoiningQuery<T1, T2, T3>
    {
        DbContext _dbContext;

        QueryBase _rootQuery;
        List<JoiningQueryInfo> _joinedQueries;
        List<LambdaExpression> _filterPredicates;

        public DbContext DbContext { get { return this._dbContext; } }
        public QueryBase RootQuery { get { return this._rootQuery; } }
        public List<JoiningQueryInfo> JoinedQueries { get { return this._joinedQueries; } }
        public List<LambdaExpression> FilterPredicates { get { return this._filterPredicates; } }

        public JoiningQuery(JoiningQuery<T1, T2> joiningQuery, Query<T3> q, JoinType joinType, Expression<Func<T1, T2, T3, bool>> on)
        {
            this._dbContext = joiningQuery.DbContext;
            this._rootQuery = joiningQuery.RootQuery;
            this._joinedQueries = Utils.CloneAndAppendOne(joiningQuery.JoinedQueries, new JoiningQueryInfo(q, joinType, on));

            this._filterPredicates = Utils.Clone(joiningQuery.FilterPredicates);
        }
        public JoiningQuery(JoiningQuery<T1, T2, T3> jq, LambdaExpression filterPredicate)
        {
            this._dbContext = jq._dbContext;
            this._rootQuery = jq._rootQuery;
            this._joinedQueries = Utils.Clone(jq._joinedQueries);

            this._filterPredicates = Utils.CloneAndAppendOne(jq._filterPredicates, filterPredicate);
        }

        public IJoiningQuery<T1, T2, T3> Where(Expression<Func<T1, T2, T3, bool>> predicate)
        {
            return new JoiningQuery<T1, T2, T3>(this, predicate);
        }

        public IJoiningQuery<T1, T2, T3, T4> Join<T4>(JoinType joinType, Expression<Func<T1, T2, T3, T4, bool>> on)
        {
            return this.Join<T4>(this._dbContext.Query<T4>(), joinType, on);
        }
        public IJoiningQuery<T1, T2, T3, T4> Join<T4>(IQuery<T4> q, JoinType joinType, Expression<Func<T1, T2, T3, T4, bool>> on)
        {
            return new JoiningQuery<T1, T2, T3, T4>(this, (Query<T4>)q, joinType, on);
        }

        public IJoiningQuery<T1, T2, T3, T4> InnerJoin<T4>(Expression<Func<T1, T2, T3, T4, bool>> on)
        {
            return this.InnerJoin<T4>(this._dbContext.Query<T4>(), on);
        }
        public IJoiningQuery<T1, T2, T3, T4> LeftJoin<T4>(Expression<Func<T1, T2, T3, T4, bool>> on)
        {
            return this.LeftJoin<T4>(this._dbContext.Query<T4>(), on);
        }
        public IJoiningQuery<T1, T2, T3, T4> RightJoin<T4>(Expression<Func<T1, T2, T3, T4, bool>> on)
        {
            return this.RightJoin<T4>(this._dbContext.Query<T4>(), on);
        }
        public IJoiningQuery<T1, T2, T3, T4> FullJoin<T4>(Expression<Func<T1, T2, T3, T4, bool>> on)
        {
            return this.FullJoin<T4>(this._dbContext.Query<T4>(), on);
        }

        public IJoiningQuery<T1, T2, T3, T4> InnerJoin<T4>(IQuery<T4> q, Expression<Func<T1, T2, T3, T4, bool>> on)
        {
            return this.Join<T4>(q, JoinType.InnerJoin, on);
        }
        public IJoiningQuery<T1, T2, T3, T4> LeftJoin<T4>(IQuery<T4> q, Expression<Func<T1, T2, T3, T4, bool>> on)
        {
            return this.Join<T4>(q, JoinType.LeftJoin, on);
        }
        public IJoiningQuery<T1, T2, T3, T4> RightJoin<T4>(IQuery<T4> q, Expression<Func<T1, T2, T3, T4, bool>> on)
        {
            return this.Join<T4>(q, JoinType.RightJoin, on);
        }
        public IJoiningQuery<T1, T2, T3, T4> FullJoin<T4>(IQuery<T4> q, Expression<Func<T1, T2, T3, T4, bool>> on)
        {
            return this.Join<T4>(q, JoinType.FullJoin, on);
        }

        public IQuery<TResult> Select<TResult>(Expression<Func<T1, T2, T3, TResult>> selector)
        {
            if (this._filterPredicates.Count == 0)
            {
                JoinQueryExpression e1 = new JoinQueryExpression(typeof(TResult), this._rootQuery.QueryExpression, this._joinedQueries, selector);
                return new Query<TResult>(this.DbContext, e1, this._rootQuery.TrackEntity);
            }

            Type joinResultType = typeof(JoinResult<T1, T2, T3>);

            Expression<Func<T1, T2, T3, JoinResult<T1, T2, T3>>> joinResultSelector = (t1, t2, t3) => new JoinResult<T1, T2, T3>() { Item1 = t1, Item2 = t2, Item3 = t3 };
            JoinQueryExpression e = new JoinQueryExpression(joinResultType, this._rootQuery.QueryExpression, this._joinedQueries, joinResultSelector);
            IQuery<JoinResult<T1, T2, T3>> q = new Query<JoinResult<T1, T2, T3>>(this.DbContext, e, this._rootQuery.TrackEntity);


            ParameterExpression parameter = Expression.Parameter(joinResultType, "a");
            Expression[] expressionSubstitutes = QueryHelper.MakeExpressionSubstitutes(joinResultType, parameter);

            Expression<Func<JoinResult<T1, T2, T3>, bool>> predicate = QueryHelper.ComposePredicate<Func<JoinResult<T1, T2, T3>, bool>>(this._filterPredicates, expressionSubstitutes, parameter);
            var q1 = q.Where(predicate);

            Expression<Func<JoinResult<T1, T2, T3>, TResult>> selector2 = JoinQueryParameterExpressionReplacer.Replace(selector, expressionSubstitutes, parameter) as Expression<Func<JoinResult<T1, T2, T3>, TResult>>;

            var ret = q1.Select(selector2);
            return ret;
        }
    }

    class JoiningQuery<T1, T2, T3, T4> : IJoiningQuery<T1, T2, T3, T4>
    {
        DbContext _dbContext;

        QueryBase _rootQuery;
        List<JoiningQueryInfo> _joinedQueries;
        List<LambdaExpression> _filterPredicates;

        public DbContext DbContext { get { return this._dbContext; } }
        public QueryBase RootQuery { get { return this._rootQuery; } }
        public List<JoiningQueryInfo> JoinedQueries { get { return this._joinedQueries; } }
        public List<LambdaExpression> FilterPredicates { get { return this._filterPredicates; } }

        public JoiningQuery(JoiningQuery<T1, T2, T3> joiningQuery, Query<T4> q, JoinType joinType, Expression<Func<T1, T2, T3, T4, bool>> on)
        {
            this._dbContext = joiningQuery.DbContext;
            this._rootQuery = joiningQuery.RootQuery;
            this._joinedQueries = Utils.CloneAndAppendOne(joiningQuery.JoinedQueries, new JoiningQueryInfo(q, joinType, on));

            this._filterPredicates = Utils.Clone(joiningQuery.FilterPredicates);
        }
        public JoiningQuery(JoiningQuery<T1, T2, T3, T4> jq, LambdaExpression filterPredicate)
        {
            this._dbContext = jq._dbContext;
            this._rootQuery = jq._rootQuery;
            this._joinedQueries = Utils.Clone(jq._joinedQueries);

            this._filterPredicates = Utils.CloneAndAppendOne(jq._filterPredicates, filterPredicate);
        }

        public IJoiningQuery<T1, T2, T3, T4> Where(Expression<Func<T1, T2, T3, T4, bool>> predicate)
        {
            return new JoiningQuery<T1, T2, T3, T4>(this, predicate);
        }

        public IJoiningQuery<T1, T2, T3, T4, T5> Join<T5>(JoinType joinType, Expression<Func<T1, T2, T3, T4, T5, bool>> on)
        {
            return this.Join<T5>(this._dbContext.Query<T5>(), joinType, on);
        }
        public IJoiningQuery<T1, T2, T3, T4, T5> Join<T5>(IQuery<T5> q, JoinType joinType, Expression<Func<T1, T2, T3, T4, T5, bool>> on)
        {
            return new JoiningQuery<T1, T2, T3, T4, T5>(this, (Query<T5>)q, joinType, on);
        }

        public IJoiningQuery<T1, T2, T3, T4, T5> InnerJoin<T5>(Expression<Func<T1, T2, T3, T4, T5, bool>> on)
        {
            return this.InnerJoin<T5>(this._dbContext.Query<T5>(), on);
        }
        public IJoiningQuery<T1, T2, T3, T4, T5> LeftJoin<T5>(Expression<Func<T1, T2, T3, T4, T5, bool>> on)
        {
            return this.LeftJoin<T5>(this._dbContext.Query<T5>(), on);
        }
        public IJoiningQuery<T1, T2, T3, T4, T5> RightJoin<T5>(Expression<Func<T1, T2, T3, T4, T5, bool>> on)
        {
            return this.RightJoin<T5>(this._dbContext.Query<T5>(), on);
        }
        public IJoiningQuery<T1, T2, T3, T4, T5> FullJoin<T5>(Expression<Func<T1, T2, T3, T4, T5, bool>> on)
        {
            return this.FullJoin<T5>(this._dbContext.Query<T5>(), on);
        }

        public IJoiningQuery<T1, T2, T3, T4, T5> InnerJoin<T5>(IQuery<T5> q, Expression<Func<T1, T2, T3, T4, T5, bool>> on)
        {
            return this.Join<T5>(q, JoinType.InnerJoin, on);
        }
        public IJoiningQuery<T1, T2, T3, T4, T5> LeftJoin<T5>(IQuery<T5> q, Expression<Func<T1, T2, T3, T4, T5, bool>> on)
        {
            return this.Join<T5>(q, JoinType.LeftJoin, on);
        }
        public IJoiningQuery<T1, T2, T3, T4, T5> RightJoin<T5>(IQuery<T5> q, Expression<Func<T1, T2, T3, T4, T5, bool>> on)
        {
            return this.Join<T5>(q, JoinType.RightJoin, on);
        }
        public IJoiningQuery<T1, T2, T3, T4, T5> FullJoin<T5>(IQuery<T5> q, Expression<Func<T1, T2, T3, T4, T5, bool>> on)
        {
            return this.Join<T5>(q, JoinType.FullJoin, on);
        }

        public IQuery<TResult> Select<TResult>(Expression<Func<T1, T2, T3, T4, TResult>> selector)
        {
            if (this._filterPredicates.Count == 0)
            {
                JoinQueryExpression e1 = new JoinQueryExpression(typeof(TResult), this._rootQuery.QueryExpression, this._joinedQueries, selector);
                return new Query<TResult>(this.DbContext, e1, this._rootQuery.TrackEntity);
            }

            Type joinResultType = typeof(JoinResult<T1, T2, T3, T4>);

            Expression<Func<T1, T2, T3, T4, JoinResult<T1, T2, T3, T4>>> joinResultSelector = (t1, t2, t3, t4) => new JoinResult<T1, T2, T3, T4>() { Item1 = t1, Item2 = t2, Item3 = t3, Item4 = t4 };
            JoinQueryExpression e = new JoinQueryExpression(joinResultType, this._rootQuery.QueryExpression, this._joinedQueries, joinResultSelector);
            IQuery<JoinResult<T1, T2, T3, T4>> q = new Query<JoinResult<T1, T2, T3, T4>>(this.DbContext, e, this._rootQuery.TrackEntity);


            ParameterExpression parameter = Expression.Parameter(joinResultType, "a");
            Expression[] expressionSubstitutes = QueryHelper.MakeExpressionSubstitutes(joinResultType, parameter);

            Expression<Func<JoinResult<T1, T2, T3, T4>, bool>> predicate = QueryHelper.ComposePredicate<Func<JoinResult<T1, T2, T3, T4>, bool>>(this._filterPredicates, expressionSubstitutes, parameter);
            var q1 = q.Where(predicate);

            Expression<Func<JoinResult<T1, T2, T3, T4>, TResult>> selector2 = JoinQueryParameterExpressionReplacer.Replace(selector, expressionSubstitutes, parameter) as Expression<Func<JoinResult<T1, T2, T3, T4>, TResult>>;

            var ret = q1.Select(selector2);
            return ret;
        }
    }

    class JoiningQuery<T1, T2, T3, T4, T5> : IJoiningQuery<T1, T2, T3, T4, T5>
    {
        DbContext _dbContext;

        QueryBase _rootQuery;
        List<JoiningQueryInfo> _joinedQueries;
        List<LambdaExpression> _filterPredicates;

        public DbContext DbContext { get { return this._dbContext; } }
        public QueryBase RootQuery { get { return this._rootQuery; } }
        public List<JoiningQueryInfo> JoinedQueries { get { return this._joinedQueries; } }
        public List<LambdaExpression> FilterPredicates { get { return this._filterPredicates; } }

        public JoiningQuery(JoiningQuery<T1, T2, T3, T4> joiningQuery, Query<T5> q, JoinType joinType, Expression<Func<T1, T2, T3, T4, T5, bool>> on)
        {
            this._dbContext = joiningQuery.DbContext;
            this._rootQuery = joiningQuery.RootQuery;
            this._joinedQueries = Utils.CloneAndAppendOne(joiningQuery.JoinedQueries, new JoiningQueryInfo(q, joinType, on));

            this._filterPredicates = Utils.Clone(joiningQuery.FilterPredicates);
        }
        public JoiningQuery(JoiningQuery<T1, T2, T3, T4, T5> jq, LambdaExpression filterPredicate)
        {
            this._dbContext = jq._dbContext;
            this._rootQuery = jq._rootQuery;
            this._joinedQueries = Utils.Clone(jq._joinedQueries);

            this._filterPredicates = Utils.CloneAndAppendOne(jq._filterPredicates, filterPredicate);
        }

        public IJoiningQuery<T1, T2, T3, T4, T5> Where(Expression<Func<T1, T2, T3, T4, T5, bool>> predicate)
        {
            return new JoiningQuery<T1, T2, T3, T4, T5>(this, predicate);
        }

        public IQuery<TResult> Select<TResult>(Expression<Func<T1, T2, T3, T4, T5, TResult>> selector)
        {
            if (this._filterPredicates.Count == 0)
            {
                JoinQueryExpression e1 = new JoinQueryExpression(typeof(TResult), this._rootQuery.QueryExpression, this._joinedQueries, selector);
                return new Query<TResult>(this.DbContext, e1, this._rootQuery.TrackEntity);
            }

            Type joinResultType = typeof(JoinResult<T1, T2, T3, T4, T5>);

            Expression<Func<T1, T2, T3, T4, T5, JoinResult<T1, T2, T3, T4, T5>>> joinResultSelector = (t1, t2, t3, t4, t5) => new JoinResult<T1, T2, T3, T4, T5>() { Item1 = t1, Item2 = t2, Item3 = t3, Item4 = t4, Item5 = t5 };
            JoinQueryExpression e = new JoinQueryExpression(joinResultType, this._rootQuery.QueryExpression, this._joinedQueries, joinResultSelector);
            IQuery<JoinResult<T1, T2, T3, T4, T5>> q = new Query<JoinResult<T1, T2, T3, T4, T5>>(this.DbContext, e, this._rootQuery.TrackEntity);


            ParameterExpression parameter = Expression.Parameter(joinResultType, "a");
            Expression[] expressionSubstitutes = QueryHelper.MakeExpressionSubstitutes(joinResultType, parameter);

            Expression<Func<JoinResult<T1, T2, T3, T4, T5>, bool>> predicate = QueryHelper.ComposePredicate<Func<JoinResult<T1, T2, T3, T4, T5>, bool>>(this._filterPredicates, expressionSubstitutes, parameter);
            var q1 = q.Where(predicate);

            Expression<Func<JoinResult<T1, T2, T3, T4, T5>, TResult>> selector2 = JoinQueryParameterExpressionReplacer.Replace(selector, expressionSubstitutes, parameter) as Expression<Func<JoinResult<T1, T2, T3, T4, T5>, TResult>>;

            var ret = q1.Select(selector2);
            return ret;
        }
    }

}
