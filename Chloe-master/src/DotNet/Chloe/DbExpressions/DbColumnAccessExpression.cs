using System;

namespace Chloe.DbExpressions
{
    /// <summary>
    /// T.Id
    /// </summary>
    public class DbColumnAccessExpression : DbExpression
    {
        DbTable _table;
        DbColumn _column;

        public DbColumnAccessExpression(DbTable table, DbColumn column)
            : base(DbExpressionType.ColumnAccess, column.Type)
        {
            this._table = table;
            this._column = column;
        }

        public DbTable Table { get { return this._table; } }

        public DbColumn Column { get { return this._column; } }

        public override T Accept<T>(DbExpressionVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }

}
