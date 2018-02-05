using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Chloe.DbExpressions
{
    public class DbCaseWhenExpression : DbExpression
    {
        ReadOnlyCollection<WhenThenExpressionPair> _whenThenPairs;
        DbExpression _else;
        public DbCaseWhenExpression(Type type, IList<WhenThenExpressionPair> whenThenPairs, DbExpression @else)
            : base(DbExpressionType.CaseWhen, type)
        {
            this._whenThenPairs = new ReadOnlyCollection<WhenThenExpressionPair>(whenThenPairs);
            this._else = @else;
        }

        public ReadOnlyCollection<WhenThenExpressionPair> WhenThenPairs { get { return this._whenThenPairs; } }
        public DbExpression Else { get { return this._else; } }

        public override T Accept<T>(DbExpressionVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public struct WhenThenExpressionPair
        {
            DbExpression _when;
            DbExpression _then;
            public WhenThenExpressionPair(DbExpression when, DbExpression then)
            {
                this._when = when;
                this._then = then;
            }

            public DbExpression When { get { return this._when; } }
            public DbExpression Then { get { return this._then; } }
        }
    }
}
