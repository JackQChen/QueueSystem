using Chloe.Extensions;
using Chloe.DbExpressions;
using Chloe.Descriptors;
using Chloe.Query.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Chloe.Query
{
    public class MappingFieldExpression : IMappingObjectExpression
    {
        Type _type;
        DbExpression _exp;
        public MappingFieldExpression(Type type, DbExpression exp)
        {
            this._type = type;
            this._exp = exp;
        }

        public DbExpression Expression { get { return this._exp; } }

        public DbExpression NullChecking { get; set; }

        public void AddMappingConstructorParameter(ParameterInfo p, DbExpression exp)
        {
            throw new NotSupportedException();
        }
        public void AddComplexConstructorParameter(ParameterInfo p, IMappingObjectExpression exp)
        {
            throw new NotSupportedException();
        }
        public void AddMappingMemberExpression(MemberInfo p, DbExpression exp)
        {
            throw new NotSupportedException();
        }
        public void AddComplexMemberExpression(MemberInfo p, IMappingObjectExpression exp)
        {
            throw new NotSupportedException();
        }
        public DbExpression GetMappingMemberExpression(MemberInfo memberInfo)
        {
            throw new NotSupportedException();
        }
        public IMappingObjectExpression GetComplexMemberExpression(MemberInfo memberInfo)
        {
            throw new NotSupportedException();
        }
        public DbExpression GetDbExpression(MemberExpression memberExpressionDeriveParameter)
        {
            Stack<MemberExpression> memberExpressions = ExpressionExtension.Reverse(memberExpressionDeriveParameter);

            if (memberExpressions.Count == 0)
                throw new Exception();

            DbExpression ret = this._exp;

            foreach (MemberExpression memberExpression in memberExpressions)
            {
                MemberInfo member = memberExpression.Member;
                ret = DbExpression.MemberAccess(member, ret);
            }

            if (ret == null)
                throw new Exception(memberExpressionDeriveParameter.ToString());

            return ret;
        }
        public IMappingObjectExpression GetComplexMemberExpression(MemberExpression exp)
        {
            throw new NotSupportedException();
        }

        public IObjectActivatorCreator GenarateObjectActivatorCreator(DbSqlQueryExpression sqlQuery)
        {
            int ordinal;
            ordinal = MappingObjectExpressionHelper.TryGetOrAddColumn(sqlQuery, this._exp).Value;

            MappingField mf = new MappingField(this._type, ordinal);

            mf.CheckNullOrdinal = MappingObjectExpressionHelper.TryGetOrAddColumn(sqlQuery, this.NullChecking);

            return mf;
        }


        public IMappingObjectExpression ToNewObjectExpression(DbSqlQueryExpression sqlQuery, DbTable table)
        {
            DbColumnAccessExpression cae = null;
            cae = MappingObjectExpressionHelper.ParseColumnAccessExpression(sqlQuery, table, this._exp);

            MappingFieldExpression mf = new MappingFieldExpression(this._type, cae);

            mf.NullChecking = MappingObjectExpressionHelper.TryGetOrAddNullChecking(sqlQuery, table, this.NullChecking);

            return mf;
        }

        public void SetNullChecking(DbExpression exp)
        {
            if (this.NullChecking == null)
                this.NullChecking = exp;
        }

    }
}
