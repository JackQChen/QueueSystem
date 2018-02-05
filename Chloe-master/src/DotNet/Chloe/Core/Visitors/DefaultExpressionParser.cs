using Chloe.Core.Visitors;
using Chloe.DbExpressions;
using Chloe.Descriptors;
using Chloe.Exceptions;
using Chloe.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Chloe.Core.Visitors
{
    public class DefaultExpressionParser : ExpressionVisitorBase
    {
        TypeDescriptor _typeDescriptor;
        DbTable _explicitDbTable;

        public DefaultExpressionParser(TypeDescriptor typeDescriptor, DbTable explicitDbTable)
        {
            this._typeDescriptor = typeDescriptor;
            this._explicitDbTable = explicitDbTable;
        }

        public DbExpression ParseFilterPredicate(LambdaExpression lambda)
        {
            lambda = ExpressionVisitorBase.ReBuildFilterPredicate(lambda);
            return this.Visit(lambda);
        }
        public DbExpression Parse(Expression exp)
        {
            return this.Visit(exp);
        }

        protected override DbExpression VisitMemberAccess(MemberExpression exp)
        {
            if (ExpressionExtension.IsDerivedFromParameter(exp))
            {
                Stack<MemberExpression> reversedExps = ExpressionExtension.Reverse(exp);

                DbExpression dbExp = null;
                bool first = true;
                foreach (var me in reversedExps)
                {
                    if (first)
                    {
                        DbColumnAccessExpression dbColumnAccessExpression = this._typeDescriptor.TryGetColumnAccessExpression(me.Member);
                        if (this._explicitDbTable != null)
                            dbColumnAccessExpression = new DbColumnAccessExpression(this._explicitDbTable, dbColumnAccessExpression.Column);

                        if (dbColumnAccessExpression == null)
                        {
                            throw new ChloeException(string.Format("The member '{0}' does not map any column.", me.Member.Name));
                        }

                        dbExp = dbColumnAccessExpression;
                        first = false;
                    }
                    else
                    {
                        DbMemberExpression dbMe = new DbMemberExpression(me.Member, dbExp);
                        dbExp = dbMe;
                    }
                }

                if (dbExp != null)
                {
                    return dbExp;
                }
                else
                    throw new Exception();
            }
            else
            {
                return base.VisitMemberAccess(exp);
            }
        }

        protected override DbExpression VisitParameter(ParameterExpression exp)
        {
            throw new NotSupportedException(exp.ToString());
        }
    }
}
