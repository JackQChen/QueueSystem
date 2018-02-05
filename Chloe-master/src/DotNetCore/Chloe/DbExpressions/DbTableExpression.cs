using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chloe.DbExpressions
{
    public class DbTableExpression : DbExpression
    {
        DbTable _table;
        public DbTableExpression(DbTable table)
            : base(DbExpressionType.Table, UtilConstants.TypeOfVoid)
        {
            this._table = table;
        }

        public DbTable Table { get { return this._table; } }

        public override T Accept<T>(DbExpressionVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
