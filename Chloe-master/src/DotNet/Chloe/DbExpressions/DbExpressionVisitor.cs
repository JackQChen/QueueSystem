using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chloe.DbExpressions
{
    public class DbExpressionVisitor : DbExpressionVisitor<DbExpression>
    {
        public override DbExpression Visit(DbEqualExpression exp)
        {
            return new DbEqualExpression(this.MakeNewExpression(exp.Left), this.MakeNewExpression(exp.Right), exp.Method);
        }
        public override DbExpression Visit(DbNotEqualExpression exp)
        {
            return new DbNotEqualExpression(this.MakeNewExpression(exp.Left), this.MakeNewExpression(exp.Right), exp.Method);
        }
        // +            
        public override DbExpression Visit(DbAddExpression exp)
        {
            return new DbAddExpression(exp.Type, this.MakeNewExpression(exp.Left), this.MakeNewExpression(exp.Right), exp.Method);
        }
        // -            
        public override DbExpression Visit(DbSubtractExpression exp)
        {
            return new DbSubtractExpression(exp.Type, this.MakeNewExpression(exp.Left), this.MakeNewExpression(exp.Right), exp.Method);
        }
        // *            
        public override DbExpression Visit(DbMultiplyExpression exp)
        {
            return new DbMultiplyExpression(exp.Type, this.MakeNewExpression(exp.Left), this.MakeNewExpression(exp.Right), exp.Method);
        }
        // /            
        public override DbExpression Visit(DbDivideExpression exp)
        {
            return new DbDivideExpression(exp.Type, this.MakeNewExpression(exp.Left), this.MakeNewExpression(exp.Right), exp.Method);
        }
        // %            
        public override DbExpression Visit(DbModuloExpression exp)
        {
            return new DbModuloExpression(exp.Type, this.MakeNewExpression(exp.Left), this.MakeNewExpression(exp.Right), exp.Method);
        }

        public override DbExpression Visit(DbNegateExpression exp)
        {
            return new DbNegateExpression(exp.Type, this.MakeNewExpression(exp.Operand));
        }

        // <            
        public override DbExpression Visit(DbLessThanExpression exp)
        {
            return new DbLessThanExpression(this.MakeNewExpression(exp.Left), this.MakeNewExpression(exp.Right), exp.Method);
        }
        // <=           
        public override DbExpression Visit(DbLessThanOrEqualExpression exp)
        {
            return new DbLessThanOrEqualExpression(this.MakeNewExpression(exp.Left), this.MakeNewExpression(exp.Right), exp.Method);
        }
        // >            
        public override DbExpression Visit(DbGreaterThanExpression exp)
        {
            return new DbGreaterThanExpression(this.MakeNewExpression(exp.Left), this.MakeNewExpression(exp.Right), exp.Method);
        }
        // >=           
        public override DbExpression Visit(DbGreaterThanOrEqualExpression exp)
        {
            return new DbGreaterThanOrEqualExpression(this.MakeNewExpression(exp.Left), this.MakeNewExpression(exp.Right), exp.Method);
        }
        public override DbExpression Visit(DbBitAndExpression exp)
        {
            return new DbBitAndExpression(exp.Type, this.MakeNewExpression(exp.Left), this.MakeNewExpression(exp.Right));
        }
        public override DbExpression Visit(DbAndExpression exp)
        {
            return new DbAndExpression(this.MakeNewExpression(exp.Left), this.MakeNewExpression(exp.Right), exp.Method);
        }
        public override DbExpression Visit(DbBitOrExpression exp)
        {
            return new DbBitOrExpression(exp.Type, this.MakeNewExpression(exp.Left), this.MakeNewExpression(exp.Right));
        }
        public override DbExpression Visit(DbOrExpression exp)
        {
            return new DbOrExpression(this.MakeNewExpression(exp.Left), this.MakeNewExpression(exp.Right), exp.Method);
        }
        public override DbExpression Visit(DbConstantExpression exp)
        {
            return exp;
        }
        public override DbExpression Visit(DbMemberExpression exp)
        {
            return new DbMemberExpression(exp.Member, this.MakeNewExpression(exp.Expression));
        }
        public override DbExpression Visit(DbNotExpression exp)
        {
            return new DbNotExpression(this.MakeNewExpression(exp.Operand));
        }
        public override DbExpression Visit(DbConvertExpression exp)
        {
            return new DbConvertExpression(exp.Type, this.MakeNewExpression(exp.Operand));
        }
        public override DbExpression Visit(DbCoalesceExpression exp)
        {
            return new DbCoalesceExpression(this.MakeNewExpression(exp.CheckExpression), this.MakeNewExpression(exp.ReplacementValue));
        }
        public override DbExpression Visit(DbCaseWhenExpression exp)
        {
            var whenThenPairs = exp.WhenThenPairs.Select(a => new DbCaseWhenExpression.WhenThenExpressionPair(this.MakeNewExpression(a.When), this.MakeNewExpression(a.Then))).ToList();
            return new DbCaseWhenExpression(exp.Type, whenThenPairs, this.MakeNewExpression(exp.Else));
        }
        public override DbExpression Visit(DbMethodCallExpression exp)
        {
            IList<DbExpression> arguments = exp.Arguments.Select(a => this.MakeNewExpression(a)).ToList();
            return new DbMethodCallExpression(this.MakeNewExpression(exp.Object), exp.Method, arguments);
        }

        public override DbExpression Visit(DbTableExpression exp)
        {
            return exp;
        }
        public override DbExpression Visit(DbColumnAccessExpression exp)
        {
            return exp;
        }

        public override DbExpression Visit(DbParameterExpression exp)
        {
            return new DbParameterExpression(exp.Value, exp.Type) { DbType = exp.DbType };
        }
        public override DbExpression Visit(DbSubQueryExpression exp)
        {
            return new DbSubQueryExpression((DbSqlQueryExpression)this.MakeNewExpression(exp.SqlQuery));
        }
        public override DbExpression Visit(DbSqlQueryExpression exp)
        {
            DbSqlQueryExpression sqlQuery = new DbSqlQueryExpression(exp.Type)
            {
                TakeCount = exp.TakeCount,
                SkipCount = exp.SkipCount,
                Table = (DbFromTableExpression)this.MakeNewExpression(exp.Table),
                Condition = this.MakeNewExpression(exp.Condition),
                HavingCondition = this.MakeNewExpression(exp.HavingCondition),
                IsDistinct = exp.IsDistinct
            };

            sqlQuery.ColumnSegments.AddRange(exp.ColumnSegments.Select(a => new DbColumnSegment(this.MakeNewExpression(a.Body), a.Alias)));
            sqlQuery.GroupSegments.AddRange(exp.GroupSegments.Select(a => this.MakeNewExpression(a)));
            sqlQuery.Orderings.AddRange(exp.Orderings.Select(a => new DbOrdering(this.MakeNewExpression(a.Expression), a.OrderType)));

            return sqlQuery;
        }
        public override DbExpression Visit(DbFromTableExpression exp)
        {
            DbFromTableExpression ret = new DbFromTableExpression(new DbTableSegment(this.MakeNewExpression(exp.Table.Body), exp.Table.Alias));
            foreach (var item in exp.JoinTables)
            {
                ret.JoinTables.Add((DbJoinTableExpression)this.MakeNewExpression(item));
            }
            return ret;
        }
        public override DbExpression Visit(DbJoinTableExpression exp)
        {
            DbJoinTableExpression ret = new DbJoinTableExpression(exp.JoinType, new DbTableSegment(this.MakeNewExpression(exp.Table.Body), exp.Table.Alias), this.MakeNewExpression(exp.Condition));
            foreach (var item in exp.JoinTables)
            {
                ret.JoinTables.Add((DbJoinTableExpression)this.MakeNewExpression(item));
            }
            return ret;
        }
        public override DbExpression Visit(DbAggregateExpression exp)
        {
            IList<DbExpression> arguments = exp.Arguments.Select(a => this.MakeNewExpression(a)).ToList();
            return new DbAggregateExpression(exp.Type, exp.Method, arguments);
        }

        public override DbExpression Visit(DbInsertExpression exp)
        {
            DbInsertExpression ret = new DbInsertExpression(exp.Table);

            foreach (var kv in exp.InsertColumns)
            {
                ret.InsertColumns.Add(kv.Key, this.MakeNewExpression(kv.Value));
            }

            return ret;
        }
        public override DbExpression Visit(DbUpdateExpression exp)
        {
            DbUpdateExpression ret = new DbUpdateExpression(exp.Table, this.MakeNewExpression(exp.Condition));

            foreach (var kv in exp.UpdateColumns)
            {
                ret.UpdateColumns.Add(kv.Key, this.MakeNewExpression(kv.Value));
            }

            return ret;
        }
        public override DbExpression Visit(DbDeleteExpression exp)
        {
            return new DbDeleteExpression(exp.Table, this.MakeNewExpression(exp.Condition));
        }

        public override DbExpression Visit(DbExistsExpression exp)
        {
            return new DbExistsExpression((DbSqlQueryExpression)this.MakeNewExpression(exp.SqlQuery));
        }

        DbExpression MakeNewExpression(DbExpression exp)
        {
            if (exp == null)
                return null;

            return exp.Accept(this);
        }
    }
}
