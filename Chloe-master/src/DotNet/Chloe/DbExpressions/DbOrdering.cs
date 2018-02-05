
namespace Chloe.DbExpressions
{
    public class DbOrdering
    {
        DbOrderType _orderType;
        DbExpression _expression;
        public DbOrdering(DbExpression expression, DbOrderType orderType)
        {
            this._expression = expression;
            this._orderType = orderType;
        }
        public DbExpression Expression { get { return this._expression; } }
        public DbOrderType OrderType { get { return this._orderType; } }
    }
}
