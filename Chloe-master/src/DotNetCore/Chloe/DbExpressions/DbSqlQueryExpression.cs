using System;
using System.Collections.Generic;
using System.Linq;

namespace Chloe.DbExpressions
{
    public class DbSqlQueryExpression : DbExpression
    {
        public DbSqlQueryExpression()
            : this(UtilConstants.TypeOfVoid)
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type">作为子句时，有时候可以指定返回的 Type，如 select A = (select id from users)，(select id from users) 就可以表示拥有一个返回的类型 Int</param>
        public DbSqlQueryExpression(Type type)
           : base(DbExpressionType.SqlQuery, type)
        {
            this.ColumnSegments = new List<DbColumnSegment>();
            this.GroupSegments = new List<DbExpression>();
            this.Orderings = new List<DbOrdering>();
        }
        public bool IsDistinct { get; set; }
        public int? TakeCount { get; set; }
        public int? SkipCount { get; set; }
        public List<DbColumnSegment> ColumnSegments { get; private set; }
        public DbFromTableExpression Table { get; set; }
        public DbExpression Condition { get; set; }
        public List<DbExpression> GroupSegments { get; private set; }
        public DbExpression HavingCondition { get; set; }
        public List<DbOrdering> Orderings { get; private set; }

        public override T Accept<T>(DbExpressionVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public DbSqlQueryExpression Update(Type type)
        {
            DbSqlQueryExpression sqlQuery = new DbSqlQueryExpression(type)
            {
                TakeCount = this.TakeCount,
                SkipCount = this.SkipCount,
                Table = this.Table,
                Condition = this.Condition,
                HavingCondition = this.HavingCondition,
                IsDistinct = this.IsDistinct
            };

            sqlQuery.ColumnSegments.AddRange(this.ColumnSegments);
            sqlQuery.GroupSegments.AddRange(this.GroupSegments);
            sqlQuery.Orderings.AddRange(this.Orderings);

            return sqlQuery;
        }
    }
}
