using Chloe.DbExpressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chloe.SqlServer
{
    class DbValueExpressionVisitor : DbExpressionVisitor<DbExpression>
    {
        SqlGenerator _generator = null;

        public DbValueExpressionVisitor(SqlGenerator generator)
        {
            if (generator == null)
                throw new ArgumentNullException("generator");

            this._generator = generator;
        }

        public override DbExpression Visit(DbEqualExpression exp)
        {
            return this.VisistDbBooleanExpression(exp);
        }
        public override DbExpression Visit(DbNotEqualExpression exp)
        {
            return this.VisistDbBooleanExpression(exp);
        }
        public override DbExpression Visit(DbNotExpression exp)
        {
            return this.VisistDbBooleanExpression(exp);
        }

        public override DbExpression Visit(DbBitAndExpression exp)
        {
            return exp.Accept(this._generator);
        }
        public override DbExpression Visit(DbAndExpression exp)
        {
            return this.VisistDbBooleanExpression(exp);
        }
        public override DbExpression Visit(DbBitOrExpression exp)
        {
            return exp.Accept(this._generator);
        }
        public override DbExpression Visit(DbOrExpression exp)
        {
            return this.VisistDbBooleanExpression(exp);
        }

        public override DbExpression Visit(DbConvertExpression exp)
        {
            return exp.Accept(this._generator);
        }
        // +
        public override DbExpression Visit(DbAddExpression exp)
        {
            return exp.Accept(this._generator);
        }
        // -
        public override DbExpression Visit(DbSubtractExpression exp)
        {
            return exp.Accept(this._generator);
        }
        // *
        public override DbExpression Visit(DbMultiplyExpression exp)
        {
            return exp.Accept(this._generator);
        }
        // /
        public override DbExpression Visit(DbDivideExpression exp)
        {
            return exp.Accept(this._generator);
        }
        // %
        public override DbExpression Visit(DbModuloExpression exp)
        {
            return exp.Accept(this._generator);
        }
        public override DbExpression Visit(DbNegateExpression exp)
        {
            return exp.Accept(this._generator);
        }
        // <
        public override DbExpression Visit(DbLessThanExpression exp)
        {
            return this.VisistDbBooleanExpression(exp);
        }
        // <=
        public override DbExpression Visit(DbLessThanOrEqualExpression exp)
        {
            return this.VisistDbBooleanExpression(exp);
        }
        // >
        public override DbExpression Visit(DbGreaterThanExpression exp)
        {
            return this.VisistDbBooleanExpression(exp);
        }
        // >=
        public override DbExpression Visit(DbGreaterThanOrEqualExpression exp)
        {
            return this.VisistDbBooleanExpression(exp);
        }

        public override DbExpression Visit(DbConstantExpression exp)
        {
            return exp.Accept(this._generator);
        }

        public override DbExpression Visit(DbCoalesceExpression exp)
        {
            return exp.Accept(this._generator);
        }

        public override DbExpression Visit(DbCaseWhenExpression exp)
        {
            return exp.Accept(this._generator);
        }

        public override DbExpression Visit(DbTableExpression exp)
        {
            return exp.Accept(this._generator);
        }

        public override DbExpression Visit(DbColumnAccessExpression exp)
        {
            return exp.Accept(this._generator);
        }

        public override DbExpression Visit(DbMemberExpression exp)
        {
            return exp.Accept(this._generator);
        }
        public override DbExpression Visit(DbParameterExpression exp)
        {
            return exp.Accept(this._generator);
        }

        public override DbExpression Visit(DbSubQueryExpression exp)
        {
            return exp.Accept(this._generator);
        }
        public override DbExpression Visit(DbSqlQueryExpression exp)
        {
            return exp.Accept(this._generator);
        }

        public override DbExpression Visit(DbMethodCallExpression exp)
        {
            if (exp.Type == UtilConstants.TypeOfBoolean || exp.Type == UtilConstants.TypeOfBoolean_Nullable)
                return this.VisistDbBooleanExpression(exp);
            else
                return exp.Accept(this._generator);
        }

        public override DbExpression Visit(DbFromTableExpression exp)
        {
            return exp.Accept(this._generator);
        }

        public override DbExpression Visit(DbJoinTableExpression exp)
        {
            return exp.Accept(this._generator);
        }
        public override DbExpression Visit(DbAggregateExpression exp)
        {
            return exp.Accept(this._generator);
        }

        public override DbExpression Visit(DbInsertExpression exp)
        {
            return exp.Accept(this._generator);
        }
        public override DbExpression Visit(DbUpdateExpression exp)
        {
            return exp.Accept(this._generator);
        }
        public override DbExpression Visit(DbDeleteExpression exp)
        {
            return exp.Accept(this._generator);
        }

        public override DbExpression Visit(DbExistsExpression exp)
        {
            return this.VisistDbBooleanExpression(exp);
        }

        DbExpression VisistDbBooleanExpression(DbExpression exp)
        {
            DbCaseWhenExpression caseWhenExpression = SqlGenerator.ConstructReturnCSharpBooleanCaseWhenExpression(exp);
            this.Visit(caseWhenExpression);
            return exp;
        }

    }

}
