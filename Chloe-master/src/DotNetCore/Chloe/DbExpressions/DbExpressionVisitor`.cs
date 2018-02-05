using System;

namespace Chloe.DbExpressions
{
    public abstract class DbExpressionVisitor<T>
    {
        public abstract T Visit(DbEqualExpression exp);
        public abstract T Visit(DbNotEqualExpression exp);
        // +
        public abstract T Visit(DbAddExpression exp);
        // -
        public abstract T Visit(DbSubtractExpression exp);
        // *
        public abstract T Visit(DbMultiplyExpression exp);
        // /
        public abstract T Visit(DbDivideExpression exp);
        // %
        public abstract T Visit(DbModuloExpression exp);

        public abstract T Visit(DbNegateExpression exp);

        // <
        public abstract T Visit(DbLessThanExpression exp);
        // <=
        public abstract T Visit(DbLessThanOrEqualExpression exp);
        // >
        public abstract T Visit(DbGreaterThanExpression exp);
        // >=
        public abstract T Visit(DbGreaterThanOrEqualExpression exp);
        public abstract T Visit(DbBitAndExpression exp);
        public abstract T Visit(DbAndExpression exp);
        public abstract T Visit(DbBitOrExpression exp);
        public abstract T Visit(DbOrExpression exp);
        public abstract T Visit(DbConstantExpression exp);
        public abstract T Visit(DbMemberExpression exp);
        public abstract T Visit(DbNotExpression exp);
        public abstract T Visit(DbConvertExpression exp);
        public abstract T Visit(DbCoalesceExpression exp);
        public abstract T Visit(DbCaseWhenExpression exp);
        public abstract T Visit(DbMethodCallExpression exp);

        public abstract T Visit(DbTableExpression exp);
        public abstract T Visit(DbColumnAccessExpression exp);

        public abstract T Visit(DbParameterExpression exp);
        public abstract T Visit(DbSubQueryExpression exp);
        public abstract T Visit(DbSqlQueryExpression exp);
        public abstract T Visit(DbFromTableExpression exp);
        public abstract T Visit(DbJoinTableExpression exp);
        public abstract T Visit(DbAggregateExpression exp);

        public abstract T Visit(DbInsertExpression exp);
        public abstract T Visit(DbUpdateExpression exp);
        public abstract T Visit(DbDeleteExpression exp);

        public abstract T Visit(DbExistsExpression exp);
    }
}
