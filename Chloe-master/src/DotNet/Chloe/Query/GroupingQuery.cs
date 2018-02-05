using Chloe.Core;
using Chloe.Query.QueryExpressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Chloe.Query
{
    class GroupingQuery<T> : IGroupingQuery<T>
    {
        protected Query<T> _fromQuery;
        protected List<LambdaExpression> _groupKeySelectors;
        protected List<LambdaExpression> _havingPredicates;
        protected List<GroupingQueryOrdering> _orderings;

        public GroupingQuery(Query<T> fromQuery, LambdaExpression keySelector)
        {
            this._fromQuery = fromQuery;
            this._groupKeySelectors = new List<LambdaExpression>(1) { keySelector };
            this._havingPredicates = new List<LambdaExpression>();
            this._orderings = new List<GroupingQueryOrdering>();
        }
        public GroupingQuery(Query<T> fromQuery, List<LambdaExpression> groupKeySelectors, List<LambdaExpression> havingPredicates, List<GroupingQueryOrdering> orderings)
        {
            this._fromQuery = fromQuery;
            this._groupKeySelectors = groupKeySelectors;
            this._havingPredicates = havingPredicates;
            this._orderings = orderings;
        }

        public IGroupingQuery<T> AndBy<K>(Expression<Func<T, K>> keySelector)
        {
            List<LambdaExpression> groupKeySelectors = Utils.CloneAndAppendOne(this._groupKeySelectors, keySelector);

            List<LambdaExpression> havingPredicates = Utils.Clone(this._havingPredicates);
            List<GroupingQueryOrdering> orderings = Utils.Clone(this._orderings);

            return new GroupingQuery<T>(this._fromQuery, groupKeySelectors, havingPredicates, orderings);
        }
        public IGroupingQuery<T> Having(Expression<Func<T, bool>> predicate)
        {
            List<LambdaExpression> groupKeySelectors = Utils.Clone(this._groupKeySelectors);

            List<LambdaExpression> havingPredicates = Utils.CloneAndAppendOne(this._havingPredicates, predicate);

            List<GroupingQueryOrdering> orderings = Utils.Clone(this._orderings);

            return new GroupingQuery<T>(this._fromQuery, groupKeySelectors, havingPredicates, orderings);
        }
        public IOrderedGroupingQuery<T> OrderBy<K>(Expression<Func<T, K>> keySelector)
        {
            return this.CreateOrderedGroupingQuery(keySelector, DbExpressions.DbOrderType.Asc, false);
        }
        public IOrderedGroupingQuery<T> OrderByDesc<K>(Expression<Func<T, K>> keySelector)
        {
            return this.CreateOrderedGroupingQuery(keySelector, DbExpressions.DbOrderType.Desc, false);
        }
        public IQuery<TResult> Select<TResult>(Expression<Func<T, TResult>> selector)
        {
            var e = new GroupingQueryExpression(typeof(TResult), this._fromQuery.QueryExpression, selector);
            e.GroupKeySelectors.AddRange(this._groupKeySelectors);
            e.HavingPredicates.AddRange(this._havingPredicates);
            e.Orderings.AddRange(this._orderings);
            return new Query<TResult>(this._fromQuery.DbContext, e, this._fromQuery._trackEntity);
        }

        protected IOrderedGroupingQuery<T> CreateOrderedGroupingQuery<K>(Expression<Func<T, K>> keySelector, DbExpressions.DbOrderType orderType, bool append)
        {
            List<LambdaExpression> groupKeySelectors = Utils.Clone(this._groupKeySelectors);
            List<LambdaExpression> havingPredicates = Utils.Clone(this._havingPredicates);

            List<GroupingQueryOrdering> orderings = new List<GroupingQueryOrdering>();
            if (append)
                orderings.AddRange(this._orderings);
            orderings.Add(new GroupingQueryOrdering(keySelector, orderType));

            return new OrderedGroupingQuery<T>(this._fromQuery, groupKeySelectors, havingPredicates, orderings);
        }
    }

    class OrderedGroupingQuery<T> : GroupingQuery<T>, IOrderedGroupingQuery<T>
    {
        public OrderedGroupingQuery(Query<T> fromQuery, List<LambdaExpression> groupKeySelectors, List<LambdaExpression> havingPredicates, List<GroupingQueryOrdering> orderings)
            : base(fromQuery, groupKeySelectors, havingPredicates, orderings)
        {
        }

        public IOrderedGroupingQuery<T> ThenBy<K>(Expression<Func<T, K>> keySelector)
        {
            return this.CreateOrderedGroupingQuery(keySelector, DbExpressions.DbOrderType.Asc, true);
        }
        public IOrderedGroupingQuery<T> ThenByDesc<K>(Expression<Func<T, K>> keySelector)
        {
            return this.CreateOrderedGroupingQuery(keySelector, DbExpressions.DbOrderType.Desc, true);
        }
    }
}
