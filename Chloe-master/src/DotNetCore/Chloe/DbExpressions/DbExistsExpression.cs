using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chloe.DbExpressions
{
    public class DbExistsExpression : DbExpression
    {
        DbSqlQueryExpression _sqlQuery;

        public DbExistsExpression(DbSqlQueryExpression sqlQuery)
            : base(DbExpressionType.Exists, UtilConstants.TypeOfBoolean)
        {
            this._sqlQuery = sqlQuery;
        }

        public DbSqlQueryExpression SqlQuery { get { return this._sqlQuery; } }

        public override T Accept<T>(DbExpressionVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
