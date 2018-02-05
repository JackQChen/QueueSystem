using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chloe.DbExpressions
{
    public class DbUpdateExpression : DbExpression
    {
        DbTable _table;
        DbExpression _condition;
        public DbUpdateExpression(DbTable table)
            : this(table, null)
        {
        }
        public DbUpdateExpression(DbTable table, DbExpression condition)
            : base(DbExpressionType.Update, UtilConstants.TypeOfVoid)
        {
            Utils.CheckNull(table);

            this._table = table;
            this._condition = condition;
            this.UpdateColumns = new Dictionary<DbColumn, DbExpression>();
        }

        public DbTable Table { get { return this._table; } }
        public Dictionary<DbColumn, DbExpression> UpdateColumns { get; private set; }
        public DbExpression Condition { get { return this._condition; } }

        public override T Accept<T>(DbExpressionVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
