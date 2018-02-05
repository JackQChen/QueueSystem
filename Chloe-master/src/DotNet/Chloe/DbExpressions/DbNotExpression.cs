
namespace Chloe.DbExpressions
{
    public class DbNotExpression : DbExpression
    {
        DbExpression _exp;

        public DbNotExpression(DbExpression exp)
            : base(DbExpressionType.Not, UtilConstants.TypeOfBoolean)
        {
            this._exp = exp;
        }

        public DbExpression Operand { get { return this._exp; } }

        public override T Accept<T>(DbExpressionVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }

}
