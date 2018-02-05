using Chloe.DbExpressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chloe.SqlServer
{
    class SqlGenerator_OffsetFetch : SqlGenerator
    {
        protected override void BuildLimitSql(DbExpressions.DbSqlQueryExpression exp)
        {
            //order by number offset 10 rows fetch next 20 rows only;
            this._sqlBuilder.Append("SELECT ");

            this.AppendDistinct(exp.IsDistinct);

            List<DbColumnSegment> columns = exp.ColumnSegments;
            for (int i = 0; i < columns.Count; i++)
            {
                DbColumnSegment column = columns[i];
                if (i > 0)
                    this._sqlBuilder.Append(",");

                this.AppendColumnSegment(column);
            }

            this._sqlBuilder.Append(" FROM ");
            exp.Table.Accept(this);
            this.BuildWhereState(exp.Condition);
            this.BuildGroupState(exp);

            List<DbOrdering> orderings = exp.Orderings;
            if (orderings.Count == 0)
            {
                DbExpression orderingExp = DbExpression.Add(UtilConstants.DbParameter_1, DbConstantExpression.Zero, DbConstantExpression.Zero.Type, null);
                DbOrdering ordering = new DbOrdering(orderingExp, DbOrderType.Asc);
                orderings = new List<DbOrdering>(1);
                orderings.Add(ordering);
            }

            this.BuildOrderState(orderings);

            this._sqlBuilder.Append(" OFFSET ", exp.SkipCount.Value.ToString(), " ROWS");
            if (exp.TakeCount != null)
            {
                this._sqlBuilder.Append(" FETCH NEXT ", exp.TakeCount.Value.ToString(), " ROWS ONLY");
            }
        }
    }
}
