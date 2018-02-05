using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chloe.DbExpressions
{
    public class DbInsertExpression : DbExpression
    {
        DbTable _table;
        Dictionary<DbColumn, DbExpression> _insertColumns;
        public DbInsertExpression(DbTable table)
            : base(DbExpressionType.Insert, UtilConstants.TypeOfVoid)
        {
            Utils.CheckNull(table);

            this._table = table;
            this._insertColumns = new Dictionary<DbColumn, DbExpression>();
        }

        public DbTable Table { get { return this._table; } }
        public Dictionary<DbColumn, DbExpression> InsertColumns { get { return this._insertColumns; } }
        public override T Accept<T>(DbExpressionVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
